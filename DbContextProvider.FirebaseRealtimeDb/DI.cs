using System;
using System.Collections.Generic;
using System.IO;
using DependencyInjection.Extensions;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DbContextProvider.FirebaseRealtimeDb
{
    public class DI : DIModule
    {
        public const string FIREBASE_HTTP_CLIENT = "FIREBASE_HTTP_CLIENT";
        public const string GOOGLE_HTTP_CLIENT = "GOOGLE_HTTP_CLIENT";

        public override IConfigurationBuilder AddConfigurationProvider(IConfigurationBuilder configurationBuilder)
        {
            var envSubstitution = new Dictionary<string, string>();
            envSubstitution["FirebaseAuthOption:ApiKey"] = Environment.GetEnvironmentVariable("FirebaseAuthOption_ApiKey");
            envSubstitution["FirebaseAuthOption:Email"] = Environment.GetEnvironmentVariable("FirebaseAuthOption_Email");
            envSubstitution["FirebaseAuthOption:Password"] = Environment.GetEnvironmentVariable("FirebaseAuthOption_Password");
            configurationBuilder.AddInMemoryCollection(envSubstitution);
            return configurationBuilder;
        }

        public override IServiceCollection AddDependencies(IConfiguration configuration, IServiceCollection services)
        {
            var firebaseAuthOption = new FirebaseAuthOption();
            configuration.Bind(nameof(FirebaseAuthOption), firebaseAuthOption);
            
            services.AddSingleton(s => firebaseAuthOption);
            services.AddSingleton(c =>
            {
                return new MemoryCache(new MemoryCacheOptions());
            });
            services.AddSingleton(c =>
            {
                var firebaseAuthOptions = c.GetService<FirebaseAuthOption>();
                return GoogleCredential.FromJson(File.ReadAllText(Path.GetFullPath(firebaseAuthOptions.ServiceAccountCredFileFullPath)));
            });
            services.AddSingleton<GoogleAuthHelper>();
            services.AddScoped<RefreshAndSetGoogleAuthToken>();
            services.AddScoped<FirebaseClient>();
            services.AddScoped<IDbContext, FirebaseDbContext>();
            services
                .AddHttpClient(GOOGLE_HTTP_CLIENT)
                .AddHttpMessageHandler(di => di.GetService<RefreshAndSetGoogleAuthToken>());

            services.AddSingleton<FirebaseAuthHelper>();
            services.AddScoped<RefreshAndSetFirebaseAuthToken>();
            services
                .AddHttpClient(FIREBASE_HTTP_CLIENT)
                .AddHttpMessageHandler(di => di.GetService<RefreshAndSetFirebaseAuthToken>());

            services.AddScoped<FirebaseCloudImageStorage>();

            return services;
        }
    }
}