using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Discord;

namespace Crab.Commands.Github
{
    [LogModule]
    public class PathModule : CrabCommandModule
    {
        [CrabCommand("\\[(?:(\\S+)\\/\\/)?(.+?)(?:(?::|#L)(\\d+)(?:-L?(\\d+))?)?\\]", RegexOptions.IgnoreCase)]
        public static Task getPath(Match match, CommandContext context){
            string prefix = match.Groups[1].Value.ToLower();
            string repo = GitUtils.get_repo(prefix);
            if(repo == null)
                return context.Channel.SendMessageAsync("Invalid prefix");

            if(!match.Groups[2].Success)
                return Task.CompletedTask;

            string path = match.Groups[2].Value.ToLower();
            bool rooted = path.StartsWith("^");
            if(rooted)
                path = path.Substring(1);

            string branchname = GitUtils.getMasterBranch(prefix);

            dynamic branch = GitUtils.get_json(GitUtils.github_url($"/repos/{repo}/branches/{branchname}"));
            if(branch == null)
                return context.Channel.SendMessageAsync("Invalid master repo config -- TODO make this an exception");
            
            dynamic tree = GitUtils.get_json(GitUtils.github_url($"/repos/{repo}/git/trees/{branch.commit.sha}?recursive=1"));
            if(tree == null)
                return context.Channel.SendMessageAsync("errrm couldn't fetch a tree for the head commit sha wtf. tell paul");

            Dictionary<string, string> hits = new Dictionary<string, string>();
            foreach (dynamic filehash in tree.tree)
            {
                string hashpath = $"{filehash.path}";
                if(rooted){
                    if(!hashpath.ToLower().StartsWith(path)){
                        continue;
                    }
                }else{
                    if(!hashpath.ToLower().EndsWith(path)){
                        continue;
                    }
                }

                string file_url = Uri.EscapeUriString(hashpath);
                string title = hashpath;
                if(match.Groups[3].Success){
                    file_url += $"#L{match.Groups[3].Value}";
                    title += $" line {match.Groups[3].Value}";
                    if(match.Groups[4].Success){
                        file_url += $"-L{match.Groups[4].Value}";
                        title = $"{hashpath} lines {match.Groups[3].Value}-{match.Groups[4].Value}";
                    }
                }

                string url = $"https://github.com/{repo}/blob/{branchname}/{file_url}";
                hits.Add(title, url);
            }

            EmbedBuilder embed = new EmbedBuilder();
            string value = "";
            int count = 0;
            foreach (var item in hits)
            {
                if(value.Length > 800){
                    if(count == 0){
                        value = $"Good job even a single entry is too long to fit within Discord's embed field limits. There were {hits.Count} hits.";
                        break;
                    }
                    value += $"...and {hits.Count-count} more.";
                    break;
                }

                value += $"[`{item.Key}`]({item.Value})\n";
                count++;
            }
            if(value == "")
                return Task.CompletedTask;

            embed.AddField(repo, value);

            return context.Channel.SendMessageAsync(embed: embed.Build());
        }
    }
}