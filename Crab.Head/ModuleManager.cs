using System;
using System.Runtime.Loader;
using System.IO;
using System.Reflection;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Crab.Events;

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

        public bool loadModule(string name)
        {
            if(!Utils.isModule(name))
                return false;

            if(Utils.needsRestart(name))
                //TODO
                return false;

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

            foreach (Assembly ass in _moduleLoadContext.Assemblies)
            {
                ModuleEventArgs args = new ModuleEventArgs();
                args.name = name;
                args.assembly = ass;
                ModuleEvents.moduleLoaded(this, args);
            }
            return true;
        }

        public bool unloadModule(string name){
            if(!Utils.isModule(name))
                return false;

            if(Utils.needsRestart(name))
                //modules that need restarts are vital and cannot be unloaded, only reloaded
                return false;

            if(_modules.ContainsKey(name)){
                foreach (Assembly ass in _modules[name].Assemblies)
                {
                    ModuleEventArgs args = new ModuleEventArgs();
                    args.name = name;
                    args.assembly = ass;
                    ModuleEvents.moduleUnloaded(this, args);
                }
                _modules[name].Unload();
                _modules.Remove(name);
                return true; 
            }
            return false;
        }
    }
}
