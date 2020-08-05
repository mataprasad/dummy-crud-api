using System;
using Autofac;
using SqlKata.Compilers;

namespace DbConnectionBuilderProvider.SQLiteConnectionBuilder
{
    public class DI : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<SqliteCompiler>().As<Compiler>().SingleInstance();
            builder.RegisterType<SQLiteConnectionBuilder>().As<IDbConnectionBuilder>().SingleInstance();
        }
    }
}
