using Microsoft.Extensions.Configuration;
using System.Linq;
using System.IO;

namespace Crab{
    public static class ConfigGetter
    {
        public static IConfiguration Get(){
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();
        }

        public static string getModulePath(string name){
            IConfiguration config = Get();
            //boy oh boy
            // two ?. cause i dunno if it will give me an empty enum or null
            return config.GetSection("modules").GetChildren().Where(t => (t.GetValue<string>("name") == name))?.First()?.GetValue<string>("path");
        }
    }
}