using System.IO;
using Autofac;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DbContextProvider.FirebaseRealtimeDb
{
    public class DI : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c =>
            {
                var configuration = c.Resolve<IConfiguration>();
                return GoogleCredential.FromJson(File.ReadAllText(configuration["gcreds"]));
            }).SingleInstance();
            builder.RegisterType<GoogleAuthHelper>().SingleInstance();
            builder.RegisterType<RefreshAndSetFirebaseAuthToken>().InstancePerLifetimeScope();
            builder.RegisterType<FirebaseClient>().InstancePerLifetimeScope();
            builder.RegisterType<FirebaseDbContext>().As<IDbContext>().InstancePerLifetimeScope();
        }
    }

    public class DIExt : DbContextProviderDI
    {
        public override IServiceCollection AddDependencies(IServiceCollection services)
        {
            services
                .AddHttpClient(FirebaseClient.FIREBASE_HTTP_CLIENT_NAME)
                .AddHttpMessageHandler(di => di.GetService<RefreshAndSetFirebaseAuthToken>());
            return services;
        }
    }
}