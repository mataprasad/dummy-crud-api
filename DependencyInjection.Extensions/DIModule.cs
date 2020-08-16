using System;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjection.Extensions
{
    public abstract class DIModule
    {
        public DIModule() { }

        public abstract IServiceCollection AddDependencies(IServiceCollection services);
    }
}
