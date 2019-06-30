using Discord.Commands;
using System.Threading.Tasks;

namespace Crab
{
    [LogModule]
    public class ProcessControl : ModuleBase<SocketCommandContext>
    {
        [Command("shutdown")]
        [AdminCommand]
        public Task shutdown(){
            ReplyAsync("Shutting down...");
            Core.exit(0);
            return null;
        }

        [Command("restart")]
        [AdminCommand]
        public Task restart(){
            ReplyAsync("Restarting...");
            Core.exit(1);
            return null;
        }
    }
}