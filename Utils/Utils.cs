using System.IO;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net;
using Discord;

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

        public static string github_url(string sub){
            return $"https://api.github.com{sub}";
        }

        public static string get_request(string uri){
            return get_request(uri, "");
        }
    
        public static string get_request(string uri, string accept)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            IConfiguration config = GetConfig();
            request.Headers["Authorization"] = config["git_auth"];
            request.UserAgent = "Crab (@PaulRitter)";
            if(accept != ""){
                request.Accept = accept;
            }else{
                request.Accept = "application/vnd.github.v3+json";
            }
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using(Stream stream = response.GetResponseStream())
            using(StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        public static dynamic get_json(string uri){
            return get_json(uri, "");
        }

        public static dynamic get_json(string uri, string accept){
            return JsonConvert.DeserializeObject(get_request(uri, accept));
        }

        public static string get_repo(string prefix){
            IConfiguration config = GetConfig();
            foreach (IConfigurationSection repo in config.GetSection("repos").GetChildren()){
                if(repo.GetValue<string>("prefix") == prefix) return repo.GetValue<string>("repo");
            }
            return null;
        }

        public static string format_desc(string desc){
            string res = desc; //remove html comments
            if(res.Length > 500)
                res = res.Substring(0, 500)+"...";
            return res;
        }

        public static Embed embed_issue(dynamic issue, string repo){
            string title = issue.title;
            string desc = issue.body;
            string avatar_url = issue.avatar_url;

            dynamic prcontent = null;
            if(issue.pull_request != null){
                prcontent = get_json(github_url($"/repos/{repo}/pulls/{issue.number}"));
            }

            EmbedBuilder embed = new EmbedBuilder();
            string emoji = "";
            if(issue.state == "open"){
                if(prcontent != null){
                    emoji = "<:PRopened:245910125041287168>";
                }else{
                    emoji = "<:ISSopened:246037149873340416>";
                }
                embed.WithColor(0x6CC644);
            }else if(prcontent != null){
                if(prcontent.merged){
                    emoji = "<:PRmerged:437316952772444170>";
                    embed.WithColor(0x6E5494);
                }else{
                    emoji = "<:PRclosed:246037149839917056>";
                    embed.WithColor(0xFF4444);
                }
            }else{
                emoji = "<:ISSclosed:246037286322569216>";
                embed.WithColor(0xFF4444);
            }

            embed.WithTitle(emoji + $"{issue.title}")
                .WithDescription(format_desc($"{issue.body}"))
                .AddField("Checks", "todo: success\ntodo: failure\ntodo: neutral")
                .WithFooter($"{repo}#{issue.number} by {issue.user.login}",$"{issue.avatar_url}");
            return embed.Build();
        }
    }
}