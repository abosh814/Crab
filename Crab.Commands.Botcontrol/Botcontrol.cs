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

        [Command("modules")]
        public Task listModules(){
            string res = "Available Modules:\n";
            res += string.Join("\n",Utils.getModuleNames());
            return ReplyAsync(res);
        }

        [Group("unload")]
        public class UnloadModule : ModuleBase<SocketCommandContext>
        {
            [Command]
            //unload specific module
            public Task unload(string modulename){
                if(!Utils.isadmin(Context.User.Id)) return null; //admincheck
                //if(modulename == "all") return unloadAll(); //is it all?
                if(!Utils.isModule(modulename)) return ReplyAsync("Thats not a module!"); //That's not a module!

                Program.currentModuleManager.unloadModule(modulename);
                return ReplyAsync($"Unloaded module `{modulename}`");
            }

            /*public Task unloadAll(){
                Program.currentModuleManager.loadAllModules();
                return ReplyAsync("Unloaded all modules");
            }*/
        }

        [Group("reload")]
        public class ReloadModule : ModuleBase<SocketCommandContext>
        {
            [Command]
            //reload specific module
            public Task reload(string modulename){
                if(!Utils.isadmin(Context.User.Id)) return null; //admincheck
                if(modulename == "all") return reloadAll(); //is it all?
                if(!Utils.isModule(modulename)) return ReplyAsync("Thats not a module!"); //That's not a module!

                Program.currentModuleManager.loadModule(modulename);
                return ReplyAsync($"Reloaded module `{modulename}`");
            }

            public Task reloadAll(){
                Program.currentModuleManager.loadAllModules();
                return ReplyAsync("Reloaded all modules");
            }
        }
    }
}