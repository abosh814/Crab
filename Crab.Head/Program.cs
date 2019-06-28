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
            currentModuleManager.loadAllModules();

            new Core().MainAsync().GetAwaiter().GetResult();
        }
    }
}
