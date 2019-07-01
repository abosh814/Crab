using System.Threading.Tasks;
using Discord.Commands;

namespace Crab
{
    [LogModule]
    public class IssueModule : ModuleBase<SocketCommandContext>
    {
        [Command("issue")]
        [RegexAlias("\\[(\\d+)\\]")]
        public Task getIssue(string issueid){
            return getIssue("", issueid);
        }

        [Command("issue")]
        [RegexAlias("\\[(\\w+)#(\\d+)\\]")]
        public Task getIssue(string prefix, string issueid){
            string repo = GitUtils.get_repo(prefix);
            if(repo == null)
                return ReplyAsync("Invalid prefix");

            dynamic obj = GitUtils.get_json(GitUtils.github_url($"/repos/{repo}/issues/{issueid}"));
            if(obj == null)
                return ReplyAsync("Issue not found");

            return ReplyAsync(embed: GitUtils.embed_issue(obj, repo));
        }
    }
}