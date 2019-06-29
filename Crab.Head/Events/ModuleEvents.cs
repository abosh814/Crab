using System;
using System.Reflection;

namespace Crab.Events
{
    public static class ModuleEvents
    {
        public static EventHandler<ModuleEventArgs> onLoad;
        public static EventHandler<ModuleEventArgs> onUnload;

        public static void moduleLoaded(object sender, ModuleEventArgs args)
        {
            if(onLoad != null)
                onLoad(sender, args);
        }
        public static void moduleUnloaded(object sender, ModuleEventArgs args)
        {
            if(onUnload != null)
                onUnload(sender, args);
        }
    }

    public class ModuleEventArgs : EventArgs
    {
        public string name;
        public Assembly assembly;
    }
}