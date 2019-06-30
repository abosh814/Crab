using Discord.Commands;
using System.Threading.Tasks;

namespace Crab
{
    [LogModule]
    public class ProcessControl : ModuleBase<SocketCommandContext>
    {
        [Command("shutdown")]
        public Task shutdown(){
            if(!Utils.isadmin(Context.User.Id))
                return null;
            ReplyAsync("Shutting down...");
            Core.exit(0);
            return null;
        }

        [Command("restart")]
        public Task restart(){
            if(!Utils.isadmin(Context.User.Id))
                return null;
            ReplyAsync("Restarting...");
            Core.exit(1);
            return null;
        }
    }
}