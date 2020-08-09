using System.IO;
using Autofac;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Retrofit;
using Retrofit.Net;

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
            builder.RegisterType<FirebaseCallInterceptor>().As<IHttpMiddleware>().SingleInstance();
            builder.Register(c =>
            {
                var dbUrl = c.Resolve<IConfiguration>()[FirebaseClient.FIREBASE_DB_URL_CONFIG_KEY];
                return new RestAdapter(dbUrl, new[] { c.Resolve<IHttpMiddleware>() });
            }).SingleInstance();
            builder.RegisterType<FirebaseClientFactory>().SingleInstance();
            builder.RegisterType<FirebaseDbContext>().As<IDbContext>().InstancePerLifetimeScope();
        }
    }
}