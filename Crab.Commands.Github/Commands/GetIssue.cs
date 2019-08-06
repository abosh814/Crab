using System.Threading.Tasks;
using Crab.Commands;
using System.Text.RegularExpressions;

namespace Crab
{
    [LogModule]
    public class IssueModule : CrabCommandModule
    {
        [CrabCommand("\\[(\\d+)\\]")]
        public static Task getIssueWithoutPrefix(Match m, CommandContext context)
            => getIssue("", m.Groups[1].Value, context);

        [CrabCommand("\\[(\\w+)#(\\d+)\\]")]
        public static Task getIssueWithPrefix(Match m, CommandContext context)
            => getIssue(m.Groups[1].Value, m.Groups[2].Value, context);


        public static Task getIssue(string prefix, string issueid, CommandContext context){
            string repo = GitUtils.get_repo(prefix);
            if(repo == null)
                return context.Channel.SendMessageAsync("Invalid prefix");

            dynamic obj = GitUtils.get_json(GitUtils.github_url($"/repos/{repo}/issues/{issueid}"));
            if(obj == null)
                return context.Channel.SendMessageAsync("Issue not found");

            return context.Channel.SendMessageAsync(embed: GitUtils.embed_issue(obj, repo));
        }
    }
}