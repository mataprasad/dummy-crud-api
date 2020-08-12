using System;
using Microsoft.Extensions.DependencyInjection;

namespace DbContextProvider
{
    public abstract class DbContextProviderDI
    {
        public abstract IServiceCollection AddDependencies(IServiceCollection services);
    }
}
