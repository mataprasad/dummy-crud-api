using DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using SqlKata.Compilers;

namespace DbConnectionBuilderProvider.SQLiteConnectionBuilder
{
    public class DI : DIModule
    {

        public override IServiceCollection AddDependencies(IServiceCollection services)
        {
            services.AddSingleton<Compiler, SqliteCompiler>();
            services.AddSingleton<IDbConnectionBuilder, SQLiteConnectionBuilder>();
            return services;
        }
    }
}
