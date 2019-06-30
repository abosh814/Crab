using System.Reflection;
using System;

namespace Crab
{
    public static class InstanceHolder
    {
        
    }
    public class Program
    {
        public static ModuleManager currentModuleManager;

        static void Main(string[] args)
        {
            currentModuleManager = new ModuleManager();
            MethodInfo coreMethod = currentModuleManager.loadAllModules(true);

            int response;
            do
            {
                response = (int)coreMethod.Invoke(null, null);
                Console.WriteLine($"Core exited with code {response}");
            } while (response == 1);
            //new Core().MainAsync().GetAwaiter().GetResult();
        }
    }
}
