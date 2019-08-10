using System.IO;
using Newtonsoft.Json;
using System.Net;
using Discord;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Crab
{
    public static class GitUtils
    {
        public const string PR_closed = "<:PRclosed:593477127005798407>";
        public const string PR_merged = "<:PRmerged:593477080256348190>";
        public const string PR_opened = "<:PRopened:593477178151403521>";
        public const string ISS_opened = "<:ISSopened:593476746674831390>";
        public const string ISS_closed = "<:ISSclosed:593476827847327744>";
        public const string upvote = "<:upvote:593476691008159768>";
        public const string downvote = "<:downvote:593476558509834261>";

        public static string github_url(string sub){
            return $"https://api.github.com{sub}";
        }

        public static string get_request(string uri){
            return get_request(uri, "");
        }
    
        public static string get_request(string uri, string accept)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Headers["Authorization"] = ConfigUtils.getGitAuth();
            request.UserAgent = "Crab (@PaulRitter)";
            if(accept != ""){
                request.Accept = accept;
            }else{
                request.Accept = "application/vnd.github.v3+json";
            }
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            try{
                using(HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using(Stream stream = response.GetResponseStream())
                using(StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }catch(WebException) //if we 404
            {
                return "";
            }
        }

        public static dynamic get_json(string uri){
            return get_json(uri, "");
        }

        public static dynamic get_json(string uri, string accept){
            return JsonConvert.DeserializeObject(get_request(uri, accept));
        }

        public static string get_repo(string prefix){
            IConfiguration config = ConfigUtils.getConfig();
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
            string desc = format_desc($"{issue.body}");

            dynamic prcontent = null;
            if(issue.pull_request != null){
                prcontent = get_json(github_url($"/repos/{repo}/pulls/{issue.number}"));
            }

            EmbedBuilder embed = new EmbedBuilder();
            string emoji = "";
            if(issue.state == "open"){
                if(prcontent != null){
                    emoji = PR_opened;
                }else{
                    emoji = ISS_opened;
                }
                embed.WithColor(0x6CC644);
            }else if(prcontent != null){
                if(prcontent.merged != null){
                    emoji = PR_merged;
                    embed.WithColor(0x6E5494);
                }else{
                    emoji = PR_closed;
                    embed.WithColor(0xFF4444);
                }
            }else{
                emoji = ISS_closed;
                embed.WithColor(0xFF4444);
            }

            title = emoji + title;

            dynamic reactions = get_json($"{issue.url}/reactions?per_page=100", "application/vnd.github.squirrel-girl-preview+json");
            Dictionary<string, int> reactions_count = new Dictionary<string, int>();
            foreach (dynamic reaction in reactions)
            {
                if(reactions_count.ContainsKey($"{reaction.content}")){
                    reactions_count[$"{reaction.content}"] += 1;
                }else{
                    reactions_count.Add($"{reaction.content}", 1);
                }
            }

            desc += "\n";
            if(reactions_count.ContainsKey("+1")){
                desc += $"{upvote} {reactions_count["+1"]}   ";
            }
            if(reactions_count.ContainsKey("-1")){
                desc += $"{downvote} {reactions_count["-1"]}";
            }

            if(prcontent != null && prcontent.mergeable != true && prcontent.merged != true){
                desc += "\n**🚨 `CONFLICTS` 🚨**";
            }

            embed.WithTitle(title)
                .WithUrl($"{issue.html_url}")
                .WithDescription(desc)
                .WithFooter($"{repo}#{issue.number} by {issue.user.login}",$"{issue.user.avatar_url}");

            if(prcontent != null){
                //Checks
                string merge_sha = prcontent.head.sha;
                dynamic check_content = get_json(github_url($"/repos/{repo}/commits/{merge_sha}/check-runs"), "application/vnd.github.antiope-preview+json");

                string checks = "";
                foreach (dynamic check in check_content.check_runs)
                {
                    string status = "";
                    switch ($"{check.status}")
                    {
                        case "queued":
                            status = "In Queue";
                            break;
                        case "in_progress":
                            status = "Running";
                            break;
                        case "completed":
                            if(check.output != null && check.output.title != null){
                                status = $"{check.output.title}";
                            }else{
                                switch($"{check.conclusion}"){
                                    case "neutral":
                                        status = "Neutral Result";
                                        break;
                                    case "success":
                                        status = "Success";
                                        break;
                                    case "failure":
                                        status = "Failed";
                                        break;
                                    case "cancelled":
                                        status = "Cancelled";
                                        break;
                                    case "timed_out":
                                        status = "Timed out";
                                        break;
                                    case "action_required":
                                        status = $"[Action Required]({check.details_url})";
                                        break;
                                    default:
                                        status = $"UNKNOWN RESULT: {check.conclusion}";
                                        break;
                                }
                            }
                            break;
                        default:
                            status = $"UNKNOWN STATUS: {check.status}";
                            break;
                    }
                    string name = $"{check.name}";
                    if(check.app.name != null){
                        name = $"{check.app.name}";
                    }

                    checks += $"[{name}]({check.html_url}) - {status}\n";
                }

                //Statuses
                dynamic status_content = get_json(github_url($"/repos/{repo}/commits/{merge_sha}/status"));
                foreach(dynamic status in status_content.statuses){
                    string stat = "";
                    if(status.description != null){
                        stat = $"{status.description}";
                    }else{
                        switch($"{status.state}"){
                            case "success":
                                stat += "Success";
                                break;
                            case "pending":
                                stat += "Running";
                                break;
                            case "error":
                                stat += "Error";
                                break;
                            case "failure":
                                stat += "Failed";
                                break;
                            default:
                                stat += $"UNKNOWN STATUS: {status.state}";
                                break;
                        }
                    }
                    checks += $"[{status.context}]({status.target_url}) - {stat}\n";
                }


                if(checks != ""){
                    embed.AddField("Checks", checks);
                }
            }
            embed.WithAuthor($"{issue.user.login}", $"{issue.user.avatar_url}", $"{issue.user.html_url}");


            return embed.Build();
        }

        public static Embed embed_commit(string sha, string repo){
            dynamic obj = GitUtils.get_json(GitUtils.github_url($"/repos/{repo}/git/commits/{sha}"));
            if(obj == null)
                return null;
            string[] split = $"{obj.message}".Split("\n");
            string title = split[0];
            string desc = String.Join("\n", split.Skip(1));

            EmbedBuilder embed = new EmbedBuilder();
            return embed.WithFooter($"{repo} {sha} by {obj.author.name}")
                .WithUrl($"{obj.html_url}")
                .WithTitle(title)
                .WithDescription(format_desc(desc))
                .Build();
        }
    }
}