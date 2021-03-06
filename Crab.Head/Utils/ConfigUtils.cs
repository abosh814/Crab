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
            => getAllModuleNames(false);

        public static List<string> getAllModuleNames(bool ignore_not_on_init_modules)
        {
            List<string> names = new List<string>();
            foreach (IConfigurationSection module in getConfig().GetSection("modules").GetChildren())
            {
                if(module.GetValue<bool>("dont_load_on_init"))
                    continue;
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

        public static bool only_reload(string name)
        {
            if(!isModule(name)) return false;
            IConfiguration config = getConfig();
            IEnumerable<IConfigurationSection> sections = config.GetSection("modules").GetChildren().Where(t => (t.GetValue<string>("name") == name));

            foreach (IConfigurationSection sec in sections)
            {
                if(sec.GetValue<bool>("only_reload"))
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
                names.Add(name + " - " + (only_reload(name) ? "Only reloadable (vital module)" : "Can be unloaded"));
            }
            return names;
        }

        public static string getGitAuth()
        {
            return getConfig().GetValue<string>("git_auth");
        }
    }
}