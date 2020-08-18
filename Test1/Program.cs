using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DbContextProvider.FirebaseRealtimeDb;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Test1
{
    class Program
    {

        static void Main(string[] args)
        {
            var stream = File.OpenRead("/Users/chauhan/Desktop/Screenshot 2020-07-23 at 1.47.48 PM.png");
            var envSubstitution = new Dictionary<string, string>();
            envSubstitution["FirebaseAuthOption:ApiKey"] = Environment.GetEnvironmentVariable("FirebaseAuthOption_ApiKey");
            envSubstitution["FirebaseAuthOption:Email"] = Environment.GetEnvironmentVariable("FirebaseAuthOption_Email");
            envSubstitution["FirebaseAuthOption:Password"] = Environment.GetEnvironmentVariable("FirebaseAuthOption_Password");


            var builder = new ConfigurationBuilder()
                .AddJsonFile("/Users/chauhan/Projects/DummyCrudApi/Test1/appsettings.json", optional: true, reloadOnChange: true)
                .AddInMemoryCollection(envSubstitution);

            var configuration = builder.Build();

            var di = new DI();

            var services = new ServiceCollection();
            di.AddDependencies(configuration, services);

            var conatiner = services.BuildServiceProvider();
            var firebaseStorage = conatiner.GetService<FirebaseCloudImageStorage>();
            var aa = conatiner.GetService<FirebaseAuthOption>();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("FirebaseAuthOption : =>");
            Console.WriteLine(JsonConvert.SerializeObject(aa));

            var fileName = String.Concat(Guid.NewGuid().ToString().ToLower(), ".png");

            var postedData = firebaseStorage.Insert(fileName, stream);

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("postedData : =>");
            Console.WriteLine(JsonConvert.SerializeObject(postedData));
            var getData = firebaseStorage.Get(fileName);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("getData : =>");
            Console.WriteLine(JsonConvert.SerializeObject(getData));
            var deleteData = firebaseStorage.Delete(fileName);
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("deleteData : =>");
            Console.WriteLine(JsonConvert.SerializeObject(deleteData));



        }
    }
}
