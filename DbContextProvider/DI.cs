using System;
using System.Data;
using DbConnectionBuilderProvider;
using DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace DbContextProvider
{
    public class DI : DIModule
    {
        public override IServiceCollection AddDependencies(IServiceCollection services)
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
