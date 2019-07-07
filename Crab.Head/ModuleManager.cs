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
        public Dictionary<string, AssemblyLoadContext> _modules = new Dictionary<string, AssemblyLoadContext>();

        public MethodInfo loadAllModules()
            => loadAllModules(false);

        public MethodInfo loadAllModules(bool isInit)
        {
            Console.WriteLine("Loading modules!");
            MethodInfo coreMethod = null;
            foreach (string name in ConfigUtils.getAllModuleNames(isInit))
            {
                ModuleLoadResult mlr = loadModule(name, isInit);
                if(mlr.coreLoadedMethod != null && isInit)
                    coreMethod = mlr.coreLoadedMethod;
            }
            return coreMethod;
        }

        public ModuleLoadResult loadModule(string name)
            => loadModule(name, false);

        private ModuleLoadResult loadModule(string name, bool isInit)
        {
            if(!ConfigUtils.isModule(name))
                return ModuleLoadResult.Fail;

            if(ConfigUtils.needsRestart(name) && !isInit)
                //TODO
                return ModuleLoadResult.Fail;

            unloadModule(name); //we dont need to pass isinit to here, since we are initializing

            AssemblyLoadContext _moduleLoadContext = new AssemblyLoadContext(name, true);
            string modulePath = ConfigUtils.getModulePath(name);
            Assembly assembly;
            MethodInfo coreLoadedMethod = null;
            using (var file = File.OpenRead(modulePath))
            {
                assembly = _moduleLoadContext.LoadFromStream(file);
                foreach (var moduleType in assembly.GetTypes())
                {
                    //_sawmill.Debug("Found module {0}", moduleType);
                    if(moduleType.GetCustomAttribute(typeof(LogModule)) != null)
                        Console.WriteLine($"Loaded module {moduleType}");
                    if(moduleType.BaseType == typeof(CrabCore))
                        coreLoadedMethod = moduleType.GetMethod("loaded"); //this should never fail cause moduleType NEEDS to have been inherited from CrabModule
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
            return new ModuleLoadResult(true, coreLoadedMethod);;
        }

        public bool unloadModule(string name){
            if(!ConfigUtils.isModule(name))
                return false;

            if(ConfigUtils.needsRestart(name))
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

    public struct ModuleLoadResult
    {
        public static ModuleLoadResult Fail = new ModuleLoadResult(false,null);

        public ModuleLoadResult(bool s, MethodInfo c)
        {
            success = s;
            coreLoadedMethod = c;
        }
        public bool success;
        public MethodInfo coreLoadedMethod;
    }
}
