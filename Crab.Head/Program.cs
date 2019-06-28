using System;
using System.IO;

namespace Crab
{
    class Program
    {
        static void Main(string[] args)
        {
            new ModuleManager().loadAllModules();

            try
            {
                new Core().MainAsync().GetAwaiter().GetResult();
            }
            catch (System.Exception e)
            {
                Console.WriteLine("Erroryes: ", e.Message);
                throw;
            }
        }
    }
}
