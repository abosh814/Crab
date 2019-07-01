using System.Threading.Tasks;
using Discord.Commands;

namespace Crab
{
    [LogModule]
    public class ModuleControl : ModuleBase<SocketCommandContext>
    {

        [Command("modules")]
        [AdminCommand]
        public Task listModules(){
            string res = "Available Modules:\n";
            res += string.Join("\n",ConfigUtils.getModuleList());
            return ReplyAsync(res);
        }

        [Command("unload")]
        [AdminCommand]
        //unload specific module
        public Task unload(string modulename){
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

        [LogModule]
        [Group("reload")]
        public class ReloadModule : ModuleBase<SocketCommandContext>
        {
            [Command]
            [AdminCommand]
            //reload specific module
            public Task reload(string modulename){
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

            private Task reloadAll(){
                Program.currentModuleManager.loadAllModules();
                return ReplyAsync("Reloaded all modules");
            }
        }
    }
}