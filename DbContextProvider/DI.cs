using System;
using System.Data;
using DbConnectionBuilderProvider;
using DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DbContextProvider
{
    public class DI : DIModule
    {
        public override IConfigurationBuilder AddConfigurationProvider(IConfigurationBuilder configurationBuilder)
        {
            return configurationBuilder;
        }

        public override IServiceCollection AddDependencies(IConfiguration configuration, IServiceCollection services)
        {
            services.AddScoped<Func<IDbConnection>>(c =>
            {
                return c.GetRequiredService<IDbConnectionBuilder>().GetConnection;
            });
            services.AddScoped<IDbContext, DefaultDbContext>();
            return services;
        }
    }
}
