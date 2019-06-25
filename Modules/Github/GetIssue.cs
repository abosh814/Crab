using System.Threading.Tasks;
using Discord.Commands;

namespace Crab.Modules
{
    public class IssueModule : ModuleBase<SocketCommandContext>
    {
        [Command("issue")]
        public Task getIssue(string issueid){
            return getIssue(issueid, "");
        }

        [Command("issue")]
        public Task getIssue(string issueid, string prefix){
            string repo = Utils.get_repo(prefix);
            if(repo == null)
                return ReplyAsync("Invalid prefix");

            dynamic obj = Utils.get_json(Utils.github_url($"/repos/{repo}/issues/{issueid}"));
            if(obj == null)
                return ReplyAsync("Issue not found");

            return ReplyAsync(embed: Utils.embed_issue(obj, repo));
        }
    }
}