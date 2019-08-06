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

        public static AsyncManualResetEvent exitEvent;

        static void Main(string[] args)
        {
            currentModuleManager.loadAllModules(true);

            exitEvent = new AsyncManualResetEvent();
            exitEvent.WaitAsync().GetAwaiter().GetResult();

            currentModuleManager.unloadAllModules();
        }

        public void terminateProgram()
        {
            exitEvent.Set();
        }
    }
}
