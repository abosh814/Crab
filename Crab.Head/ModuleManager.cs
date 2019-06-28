using System;
using System.Runtime.Loader;
using System.IO;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Crab
{
    public class ModuleManager
    {
        private Dictionary<string, AssemblyLoadContext> _modules = new Dictionary<string, AssemblyLoadContext>();

        public void loadAllModules()
        {
            Console.WriteLine("Loading modules!");
            IConfiguration config = Utils.getConfig();
            foreach (IConfigurationSection item in config.GetSection("modules").GetChildren())
            {
                loadModule(item.GetValue<string>("name"));
            }
        }

        public void loadModule(string name)
        {
            unloadModule(name);

            AssemblyLoadContext _moduleLoadContext = new AssemblyLoadContext(name, true);
            string modulePath = Utils.getModulePath(name);
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
            _modules.Add(name, _moduleLoadContext);

            if(Core._command != null){
                Core._command.loadModuleAsync(assembly);
            }else{
                Console.WriteLine("help why no commandhandler");
            }

        }

        public bool unloadModule(string name){
            if(_modules.ContainsKey(name)){
                if(Core._command != null){
                    foreach (Assembly ass in _modules[name].Assemblies)
                    {
                        Core._command.unloadModuleAsync(ass);
                    }
                }
                _modules[name].Unload();
                _modules.Remove(name);
                return true; 
            }
            return false;

        }
    }
}
