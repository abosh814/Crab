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
        public Dictionary<string, LoadedModule> _modules = new Dictionary<string, LoadedModule>();

        public void loadAllModules()
            => loadAllModules(false);

        public void loadAllModules(bool isInit)
        {
            Console.WriteLine("Loading modules!");
            foreach (string name in ConfigUtils.getAllModuleNames(isInit))
            {
                loadModule(name, isInit);
            }
        }

        public bool loadModule(string name)
            => loadModule(name, false);

        private bool loadModule(string name, bool isInit)
        {
            if(!ConfigUtils.isModule(name))
                return false;

            unloadModule(name, true); //we dont need to pass isinit to here, since we are initializing

            AssemblyLoadContext _moduleLoadContext = new AssemblyLoadContext(name, true);
            string modulePath = ConfigUtils.getModulePath(name);
            Assembly assembly;
            List<ModuleInstance> instances = new List<ModuleInstance>();
            using (var file = File.OpenRead(modulePath))
            {
                assembly = _moduleLoadContext.LoadFromStream(file);
                foreach (Type moduleType in assembly.GetTypes())
                {
                    //_sawmill.Debug("Found module {0}", moduleType);
                    if(moduleType.GetCustomAttribute(typeof(LogModule)) != null)
                        Console.WriteLine($"Loaded module {moduleType}");
                    if(moduleType.BaseType == typeof(ModuleInstance)){
                        ModuleInstance t_module = (ModuleInstance)Activator.CreateInstance(moduleType); //this should never fail cause moduleType NEEDS to have been inherited from CrabModule
                        t_module.mainAsync().GetAwaiter();
                        instances.Add(t_module);
                    }
                }
            }
            _modules.Add(name, new LoadedModule(_moduleLoadContext, instances));

            foreach (Assembly ass in _moduleLoadContext.Assemblies)
            {
                ModuleEventArgs args = new ModuleEventArgs();
                args.name = name;
                args.assembly = ass;
                ModuleEvents.moduleLoaded(this, args);
            }
            return true;
        }

        public void unloadAllModules()
        {
            List<string> names = new List<string>();
            foreach (var item in _modules)
            {
                names.Add(item.Key);
            }
            foreach (var item in names)
            {
                unloadModule(item);
            }
        }

        public bool unloadModule(string name, bool isreloading = false){
            if(!ConfigUtils.isModule(name))
                return false;

            if(_modules.ContainsKey(name)){
                foreach (Assembly ass in _modules[name].context.Assemblies)
                {
                    ModuleEventArgs args = new ModuleEventArgs();
                    args.name = name;
                    args.assembly = ass;
                    ModuleEvents.moduleUnloaded(this, args);
                }
                _modules[name].context.Unload();
                foreach (ModuleInstance instance in _modules[name].instances)
                {
                    instance.exit(ModuleInstanceResult.SHUTDOWN);
                }
                _modules.Remove(name);

                //modules that need restarts are vital and cannot be unloaded, only reloaded
                if(ConfigUtils.only_reload(name) && !isreloading){
                    loadModule(name);
                }
                return true; 
            }
            return false;
        }
    }

    public struct LoadedModule{
        public LoadedModule(AssemblyLoadContext c, List<ModuleInstance> i)
        {
            context = c;
            instances = i;
        }
        public AssemblyLoadContext context;

        public List<ModuleInstance> instances;
    }
}
