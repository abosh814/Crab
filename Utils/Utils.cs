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

            return false;
        }

        public static string idinfo(ulong id){
            //header
            string info = $"ID: {id}\n";
            
            //admin?
            info += "Admin: ";
            if(isadmin(id))
                info += "Yes";
            else
                info += "No";
            info += "\n";

            return info
        }
    }
}