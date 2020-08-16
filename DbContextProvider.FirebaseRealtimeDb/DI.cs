using System.IO;
using DependencyInjection.Extensions;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DbContextProvider.FirebaseRealtimeDb
{
    public class DI : DIModule
    {
        public override IServiceCollection AddDependencies(IServiceCollection services)
        {
            services.AddSingleton(c =>
            {
                var configuration = c.GetService<IConfiguration>();
                return GoogleCredential.FromJson(File.ReadAllText(configuration["gcreds"]));
            });
            services.AddSingleton<GoogleAuthHelper>();
            services.AddScoped<RefreshAndSetFirebaseAuthToken>();
            services.AddScoped<FirebaseClient>();
            services.AddScoped<IDbContext, FirebaseDbContext>();
            services
                .AddHttpClient(FirebaseClient.FIREBASE_HTTP_CLIENT_NAME)
                .AddHttpMessageHandler(di => di.GetService<RefreshAndSetFirebaseAuthToken>());
            return services;
        }
    }
}