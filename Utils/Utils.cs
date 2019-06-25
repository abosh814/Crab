using System.IO;
using Microsoft.Extensions.Configuration;

namespace Crab{
    static class Utils {
        public static IConfiguration GetConfig(){
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .Build();
        }

        public static bool isadmin(ulong id){
            IConfiguration config = GetConfig();

            foreach(IConfigurationSection admin in config.GetSection("devs").GetChildren()){
                if(admin.Value == id.ToString()) return true;
            }
            return false;
        }

        public static string idinfo(ulong id){
            //header
            string info = $"ID: {id}\n";
            IConfiguration config = GetConfig();
            
            //admin?
            info += "Admin: ";
            if(isadmin(id)){
                info += "Yes";
            }else{
                info += "No";
            }
            info += "\n";

            return info;
        }

        public static string listConfig(){
            string info = "";
            foreach(IConfigurationSection section in GetConfig().GetChildren()){
                info += ConfigSectionToString(section);
            }
            return info;
        }

        public static string ConfigSectionToString(IConfigurationSection section){
            var children = section.GetChildren();
            string text = "";
            foreach(IConfigurationSection child in children){
                text += ConfigSectionToString(child);
            }
            if(text != ""){
                return $"{section.Key}:\n{text}\n";
            }else{
                return $"{section.Key}: {section.Value}\n";
            }
        }
    }
}