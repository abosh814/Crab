using System.Threading.Tasks;
using Discord.Commands;

namespace Crab
{
    [LogModule]
    public class IssueModule : ModuleBase<SocketCommandContext>
    {
        public void onLoad(){}
        
        [Command("issue")]
        public Task getIssue(string issueid){
            return getIssue(issueid, "");
        }

        [Command("issue")]
        public Task getIssue(string issueid, string prefix){
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