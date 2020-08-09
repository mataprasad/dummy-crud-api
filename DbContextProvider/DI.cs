using System;
using System.Data;
using Autofac;
using DbConnectionBuilderProvider;

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
}
