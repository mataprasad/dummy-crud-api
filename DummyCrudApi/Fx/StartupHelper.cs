using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DummyCrudApi.Fx
{
    public static class StartupHelper
    {
        public static IServiceCollection AddDynamicRegistartion(this IServiceCollection services, IConfiguration configuration, string providerKeyInConfig)
        {
            CallMethodsFromReflection<DIModule>(providerKeyInConfig, (module) => module.AddDependencies(configuration, services), configuration);
            return services;
        }

        public static IConfigurationBuilder AddConfigurationProvider(IConfigurationBuilder configurationBuilder, IConfiguration configuration, string providerKeyInConfig)
        {
            CallMethodsFromReflection<DIModule>(providerKeyInConfig, (module) => module.AddConfigurationProvider(configurationBuilder), configuration);
            return configurationBuilder;
        }

        //public static void LoadDynamicAutofacModule(IWebHostEnvironment env, IConfiguration configuration, ContainerBuilder builder, string providerKeyInConfig)
        //{
        //    CallMethodsFromReflection<IModule>(configuration, providerKeyInConfig, (module) => builder.RegisterModule(module));
        //}

        private static void CallMethodsFromReflection<T>(string providerKeyInConfig, Action<T> invoker, IConfiguration configuration)
        {
            var path = Path.GetDirectoryName(typeof(StartupHelper).Assembly.Location);
            if (String.IsNullOrWhiteSpace(path))
            {
                return;
            }
            var assemblyName = String.Format("{0}.dll", configuration[providerKeyInConfig]);

            //  Gets all compiled assemblies.
            //  This is particularly useful when extending applications functionality from 3rd parties,
            //  if there are interfaces within the modules.
            var assemblies = Directory.GetFiles(path, assemblyName, SearchOption.TopDirectoryOnly)
                                      .Select(Assembly.LoadFrom);



            foreach (var assembly in assemblies)
            {
                //  Gets the all modules from each assembly to be registered.
                //  Make sure that each module **MUST** have a parameterless constructor.
                var modules = assembly.GetTypes()
                                      .Where(p => typeof(T).IsAssignableFrom(p)
                                                  && !p.IsAbstract)
                                      .Select(p => (T)Activator.CreateInstance(p));

                //  Regsiters each module.
                foreach (var module in modules)
                {
                    invoker?.Invoke(module);
                }
            }
        }
    }
}
