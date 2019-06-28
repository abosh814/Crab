using System.Threading.Tasks;
using Discord.Commands;

namespace Crab
{
    public class Botcontrol : ModuleBase<SocketCommandContext>, CrabModule
    {
        public void onLoad(){}

        [Command("shutdown")]
        public Task shutdown(){
            if(!Utils.isadmin(Context.User.Id))
                return null;
            ReplyAsync("Shutting down...");
            Context.Client.LogoutAsync();
            System.Environment.Exit(0);
            return null;
        }

        [Command("restart")]
        public Task restart(){
            if(!Utils.isadmin(Context.User.Id))
                return null;
            ReplyAsync("Restarting currently doesn't work.");
            //System.Diagnostics.Process.Start(Application.ExecutablePath);
            shutdown();
            return null;
        }

        [Group("reload")]
        public class ReloadModule : ModuleBase<SocketCommandContext>
        {
            [Command]
            //reload specific module
            public Task reload(string modulename){
                if(modulename == "all") return reloadAll();
                return ReplyAsync($"TODO: reloading module `{modulename}`");
            }

            public Task reloadAll(){
                return ReplyAsync("TODO: reloading all modules");
            }

        }
    }
}