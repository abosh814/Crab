using System.Reflection;

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

            coreMethod.Invoke(null, null);
            //new Core().MainAsync().GetAwaiter().GetResult();
        }
    }
}
