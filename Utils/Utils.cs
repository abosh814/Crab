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
    }
}