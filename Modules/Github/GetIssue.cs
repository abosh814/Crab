using System.Threading.Tasks;
using Discord.Commands;

namespace Crab.Modules
{
    public class IssueModule : ModuleBase<SocketCommandContext>
    {
        [Command("issue")]
        public Task Info()
            => ReplyAsync($"{Context.Message}");
    }
}