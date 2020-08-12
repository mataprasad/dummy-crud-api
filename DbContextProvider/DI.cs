using System;
using System.Data;
using Autofac;
using Autofac.Core;
using Autofac.Core.Registration;
using DbConnectionBuilderProvider;
using Microsoft.Extensions.DependencyInjection;

namespace DbContextProvider
{
    public class DI : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<Func<IDbConnection>>(ctx => ctx.Resolve<IDbConnectionBuilder>().GetConnection);
            builder.RegisterType<DefaultDbContext>().As<IDbContext>();
        }
    }

    public class DIExt : DbContextProviderDI
    {
        public override IServiceCollection AddDependencies(IServiceCollection services)
        {
            return services;
        }
    }
}
