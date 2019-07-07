using System.Reflection;
using System;

namespace Crab
{
    public static class InstanceHolder
    {
        
    }
    public class Program
    {
        public readonly static ModuleManager currentModuleManager = new ModuleManager();

        static void Main(string[] args)
        {
            MethodInfo coreMethod = currentModuleManager.loadAllModules(true);

            if(coreMethod == null){
                Console.WriteLine("PANIC EXIT, NO COREMETHOD FOUND");
                return;
            }

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
