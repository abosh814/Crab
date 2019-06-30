using System.Threading.Tasks;
using Discord.Commands;

namespace Crab
{
    [LogModule]
    public class Botcontrol : ModuleBase<SocketCommandContext>
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
            if(!Utils.isadmin(Context.User.Id))
                return null;
            string res = "Available Modules:\n";
            res += string.Join("\n",ConfigUtils.getModuleList());
            return ReplyAsync(res);
        }

        [LogModule]
        [Group("unload")]
        public class UnloadModule : ModuleBase<SocketCommandContext>
        {
            [Command]
            //unload specific module
            public Task unload(string modulename){
                if(!Utils.isadmin(Context.User.Id)) return null; //admincheck
                if(!ConfigUtils.isModule(modulename)) return ReplyAsync("Thats not a module!"); //That's not a module!

                if(Program.currentModuleManager.unloadModule(modulename))
                {
                    return ReplyAsync($"Unloaded module `{modulename}`");
                }
                else
                {
                    return ReplyAsync($"Couldn't unload module `{modulename}`");
                }
            }
        }

        [LogModule]
        [Group("reload")]
        public class ReloadModule : ModuleBase<SocketCommandContext>
        {
            [Command]
            //reload specific module
            public Task reload(string modulename){
                if(!Utils.isadmin(Context.User.Id)) return null; //admincheck
                if(modulename == "all") return reloadAll(); //is it all?
                if(!ConfigUtils.isModule(modulename)) return ReplyAsync("Thats not a module!"); //That's not a module!

                if(Program.currentModuleManager.loadModule(modulename).success){
                    return ReplyAsync($"Reloaded module `{modulename}`");
                }
                else
                {
                    return ReplyAsync($"Couldn't reload module {modulename}");
                }
            }

            public Task reloadAll(){
                Program.currentModuleManager.loadAllModules();
                return ReplyAsync("Reloaded all modules");
            }
        }
    }
}