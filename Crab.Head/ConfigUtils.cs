using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using Discord;
using System.Collections.Generic;
using System.Linq;

namespace Crab
{
    public static class ConfigUtils
    {
        public static IConfiguration getConfig(){
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();
        }

        public static List<string> getAllModuleNames()
        {
            List<string> names = new List<string>();
            foreach (IConfigurationSection module in getConfig().GetSection("modules").GetChildren())
            {
                names.Add(module.GetValue<string>("name"));
            }
            return names;
        }
        public static string getModulePath(string name){
            IConfiguration config = getConfig();
            if(!isModule(name))
                return null;
            //boy oh boy
            // two ?. cause i dunno if it will give me an empty enum or null
            return config.GetSection("modules").GetChildren().Where(t => (t.GetValue<string>("name") == name))?.First()?.GetValue<string>("path");
        }

        public static bool needsRestart(string name)
        {
            if(!isModule(name)) return false;
            IConfiguration config = getConfig();
            IEnumerable<IConfigurationSection> sections = config.GetSection("modules").GetChildren().Where(t => (t.GetValue<string>("name") == name));

            foreach (IConfigurationSection sec in sections)
            {
                if(sec.GetValue<bool>("needs_restart"))
                    return true;
            }
            return false;
        }

        public static bool isModule(string name){
            IConfiguration config = getConfig();
            
            return (config.GetSection("modules").GetChildren().Where(t => (t.GetValue<string>("name") == name)).Any());
        }

        public static List<string> getModuleList(){
            IConfiguration config = getConfig();
            List<string> names = new List<string>();
            foreach (IConfigurationSection item in config.GetSection("modules").GetChildren())
            {
                string name = item.GetValue<string>("name");
                names.Add(name + " - " + (needsRestart(name) ? "Needs Restart" : "Can reload at runtime"));
            }
            return names;
        }
    }
}