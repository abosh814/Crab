using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using Discord;
using System.Collections.Generic;
using System.Linq;

namespace Crab{
    public static class Utils
    {
        public static bool isadmin(ulong id){
            foreach(string admin in get_all_admin_keys()){
                if(admin == id.ToString()) return true;
            }
            return false;
        }

        public static string idinfo(ulong id){
            //header
            string info = $"ID: {id}\n";
            
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
            foreach(IConfigurationSection section in ConfigUtils.getConfig().GetChildren()){
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

        public static List<string> get_all_admin_keys(){
            List<string> keys = new List<string>();
            foreach (IConfigurationSection admin in ConfigUtils.getConfig().GetSection("devs").GetChildren())
            {
                keys.Add(admin.Value);
            }
            return keys;
        }
    }
}