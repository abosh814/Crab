using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Crab{
    public static class Utils
    {
        public static bool isadmin(ulong id){
            foreach(string admin in get_all_admin_keys()){
                if(admin == id.ToString()) return true;
            }
            return false;
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