using DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SqlKata.Compilers;

namespace DbConnectionBuilderProvider.SQLiteConnectionBuilder
{
    public class DI : DIModule
    {
        public override IConfigurationBuilder AddConfigurationProvider(IConfigurationBuilder configurationBuilder)
        {
            return configurationBuilder;
        }

        public override IServiceCollection AddDependencies(IConfiguration configuration,IServiceCollection services)
        {
            services.AddSingleton<Compiler, SqliteCompiler>();
            services.AddSingleton<IDbConnectionBuilder, SQLiteConnectionBuilder>();
            return services;
        }
    }
}
