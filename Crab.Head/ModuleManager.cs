using System;
using System.Runtime.Loader;
using System.IO;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Crab
{
    class ModuleManager
    {
        private AssemblyLoadContext _moduleLoadContext;

        public void loadAllModules()
        {
            Console.WriteLine("Loading modules!");
            if (_moduleLoadContext != null)
            {
                Console.WriteLine("Unloading previous modules!");
                _moduleLoadContext.Unload();
                _moduleLoadContext = null;
            }
            // TODO: Maybe ensure no modules are running while reloading to prevent race conditions.
            _moduleLoadContext = new AssemblyLoadContext("Crab Modules", true);
            IConfiguration config = ConfigGetter.Get();
            foreach (IConfigurationSection item in config.GetSection("modules").GetChildren())
            {
                loadModuleByPath(item.GetValue<string>("path"));
            }
        }

        public void loadModuleByName(string name)
            => loadModuleByPath(ConfigGetter.getModulePath(name));

        public void loadModuleByPath(string modulePath){
            Assembly assembly;
            using (var file = File.OpenRead(modulePath))
            {
                assembly = _moduleLoadContext.LoadFromStream(file);

                foreach (var moduleType in assembly.GetTypes().Where(t => (t.GetInterface("CrabModule") != null)))
                {
                    //_sawmill.Debug("Found module {0}", moduleType);
                    Console.WriteLine($"Loaded module {moduleType}");
                }
            }
        }
    }
}
