using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace DummyCrudApi.Fx
{
    public class StartupHelper
    {
        public static void LoadDynamicAutofacModule(IWebHostEnvironment env, IConfiguration configuration, ContainerBuilder builder,string providerKeyInConfig)
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
                                      .Where(p => typeof(IModule).IsAssignableFrom(p)
                                                  && !p.IsAbstract)
                                      .Select(p => (IModule)Activator.CreateInstance(p));

                //  Regsiters each module.
                foreach (var module in modules)
                {
                    builder.RegisterModule(module);
                }
            }
        }
    }
}
