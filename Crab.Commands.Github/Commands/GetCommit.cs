using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crab.Commands.Github
{
    [LogModule]
    public class CommitModule : CrabCommandModule
    {
        [CrabCommand("\\[(?:(\\S+)@)?([0-9a-f]{40})\\]", RegexOptions.IgnoreCase)]
        public static Task getCommit(Match match, CommandContext context){
            string repo = GitUtils.get_repo(match.Groups[1].Value);
            if(repo == null)
                return context.Channel.SendMessageAsync("Invalid prefix");

            return context.Channel.SendMessageAsync(embed: GitUtils.embed_commit(match.Groups[2].Value, repo));
        }
    }
}