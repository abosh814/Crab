using System;
using Microsoft.VisualStudio.Threading;

namespace Crab
{
    public static class InstanceHolder
    {
        
    }
    public class Program
    {
        public readonly static ModuleManager currentModuleManager = new ModuleManager();

        private static AsyncManualResetEvent exitEvent;

        private static bool active;

        static void Main(string[] args)
        {
            active = false;
            currentModuleManager.loadAllModules(true);

            exitEvent = new AsyncManualResetEvent();
            exitEvent.WaitAsync().GetAwaiter().GetResult();

            currentModuleManager.unloadAllModules();
        }

        public static void shutdown()
        {
            exitEvent.Set();
        }
    }
}
