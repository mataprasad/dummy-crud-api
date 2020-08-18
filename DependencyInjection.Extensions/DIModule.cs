using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Extensions
{
    public abstract class DIModule
    {
        public DIModule() { }

        public abstract IConfigurationBuilder AddConfigurationProvider(IConfigurationBuilder configurationBuilder);

        public abstract IServiceCollection AddDependencies(IConfiguration configuration, IServiceCollection services);
    }
}
