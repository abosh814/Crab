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
        private Dictionary<string, LoadedModule> _modules = new Dictionary<string, LoadedModule>();

        public MethodInfo loadAllModules()
            => loadAllModules(false);

        public MethodInfo loadAllModules(bool isInit)
        {
            Console.WriteLine("Loading all modules!");
            MethodInfo coreMethod = null;
            foreach (string name in ConfigUtils.getAllModuleNames())
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

            Console.WriteLine($"Starting to load {name}");
            ModuleUnloadResult unloadRes = unloadModule(name); //we dont need to pass isinit to here, since we are initializing

            List<string> toLoad = new List<string>();
            toLoad.Add(name); //first cause we need to properly handle dependencies
            toLoad.AddRange(unloadRes.leechesUnloaded);
            MethodInfo coreLoadedMethod = null;
            AssemblyLoadContext _moduleLoadContext = new AssemblyLoadContext(name, true);
            foreach (string module in toLoad)
            {
                Console.WriteLine($"Loading module {module}");
                string modulePath = ConfigUtils.getModulePath(module);
                Assembly assembly;
                using (var file = File.OpenRead(modulePath))
                {
                    assembly = _moduleLoadContext.LoadFromStream(file);
                    //getting the coreLoadedMethod and logging
                    foreach (var moduleType in assembly.GetTypes())
                    {
                        if(moduleType.GetCustomAttribute(typeof(LogModule)) != null) //logging
                            Console.WriteLine($"Loaded module {moduleType}");
                        if(moduleType.BaseType == typeof(CrabCore)) //coremethod
                            coreLoadedMethod = moduleType.GetMethod("loaded"); //this should never fail cause moduleType NEEDS to have been inherited from CrabCore
                    }
                }

                foreach (Assembly ass in _moduleLoadContext.Assemblies)
                {
                    ModuleEventArgs args = new ModuleEventArgs();
                    args.name = module;
                    args.assembly = ass;
                    ModuleEvents.moduleLoaded(this, args);
                }
            }

            foreach (string dependency in ConfigUtils.getDependencies(name))
            {
                Console.WriteLine($"Loading dependency {dependency}");
                if(!_modules.ContainsKey(dependency))
                    continue;
                Console.WriteLine($"Found AssemblyLoadContext: {_modules[dependency].context.Name}");
                foreach (Assembly ass in _modules[dependency].context.Assemblies)
                {
                    Console.WriteLine($"Loading assembly {ass.GetName().Name}");
                    if(AppDomain.CurrentDomain.GetAssemblies().Where(t => (t == ass)).Any()) Console.WriteLine("something fucky is afoot");
                    _moduleLoadContext.LoadFromAssemblyName(ass.GetName());
                }
                _modules[dependency].leeches.Add(name);
            }

            LoadedModule lm = new LoadedModule(_moduleLoadContext);
            ModuleEvents.onUnload += lm.leechUnloadedHook;

            _modules.Add(name, lm);
            Console.WriteLine($"Finished loading {name}");
            return new ModuleLoadResult(true, coreLoadedMethod);
        }

        public ModuleUnloadResult unloadModule(string name){
            if(!ConfigUtils.isModule(name))
                return ModuleUnloadResult.Fail;

            if(ConfigUtils.needsRestart(name))
                //modules that need restarts are vital and cannot be unloaded, only reloaded
                return ModuleUnloadResult.Fail;

            if(_modules.ContainsKey(name)){
                List<string> unloadedLeeches = unloadLeeches(name);

                foreach (Assembly ass in _modules[name].context.Assemblies)
                {
                    ModuleEventArgs args = new ModuleEventArgs();
                    args.name = name;
                    args.assembly = ass;
                    ModuleEvents.moduleUnloaded(this, args);
                }
                _modules[name].context.Unload();
                _modules.Remove(name);
                
                ModuleUnloadResult res = new ModuleUnloadResult(true);
                res.leechesUnloaded = unloadedLeeches;
                return res;
            }
            return ModuleUnloadResult.Fail;
        }

        private List<string> unloadLeeches(string name)
        {
            if(!ConfigUtils.isModule(name))
                return new List<string>();

            if(_modules.ContainsKey(name)){
                List<string> unloaded = new List<string>();
                foreach (string leech in _modules[name].leeches)
                {
                    ModuleUnloadResult res = unloadModule(leech);
                    if(res.success){
                        unloaded.Add(leech);
                        unloaded.AddRange(res.leechesUnloaded);
                    }
                }
                return unloaded;
            }
            return new List<string>();
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

    public struct ModuleUnloadResult
    {
        public static ModuleUnloadResult Fail = new ModuleUnloadResult(false);
        public bool success;
        public List<string> leechesUnloaded;
        public ModuleUnloadResult(bool s)
        {
            success = s;
            leechesUnloaded = new List<string>();
        }
    }

    public struct LoadedModule
    {
        public LoadedModule(AssemblyLoadContext c)
        {
            context = c;
            leeches = new List<string>();
        }
        public AssemblyLoadContext context;
        public List<string> leeches; //points to modules in _modules that depend on this one

        public void leechUnloadedHook(object sender, ModuleEventArgs args)
        {
            if(leeches.Contains(args.name)) leeches.Remove(args.name);
        }
    }
}
