using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Crab.Commands.Botcontrol
{
    [LogModule]
    public class ModuleControl : CrabCommandModule
    {
        
        [MentionOnly]
        [AdminOnly]
        [CrabCommand("modules")]
        //[AdminCommand]
        public static Task listModules(Match m, CommandContext context){
            string res = "Available Modules:\n";
            res += string.Join("\n",ConfigUtils.getModuleList());
            return context.Channel.SendMessageAsync(res);
        }

        [MentionOnly]
        [AdminOnly]
        [CrabCommand("unload (.+)")]
        //[AdminCommand]
        //unload specific module
        public static Task unload(Match m, CommandContext context){
            string name = m.Groups[1].Value;
            if(!ConfigUtils.isModule(name)) return context.Channel.SendMessageAsync("Thats not a module!"); //That's not a module!

            if(Program.currentModuleManager.unloadModule(name))
            {
                if(ConfigUtils.only_reload(name)){
                    return context.Channel.SendMessageAsync($"Reloaded module `{name}` (reload only)");
                }else{
                    return context.Channel.SendMessageAsync($"Unloaded module `{name}`");
                }
            }
            else
            {
                return context.Channel.SendMessageAsync($"Couldn't unload module `{name}`");
            }
        }

        [MentionOnly]
        [AdminOnly]
        [CrabCommand("reload (.+)")]
        //reload specific module
        public static Task reload(Match m, CommandContext context){
            string modulename = m.Groups[1].Value;
            if(modulename == "all") return reloadAll(m, context); //is it all?
            if(!ConfigUtils.isModule(modulename)) return context.Channel.SendMessageAsync("Thats not a module!"); //That's not a module!

            if(Program.currentModuleManager.loadModule(modulename)){
                return context.Channel.SendMessageAsync($"Reloaded module `{modulename}`");
            }
            else
            {
                return context.Channel.SendMessageAsync($"Couldn't reload module {modulename}");
            }
        }

        [MentionOnly]
        [AdminOnly]
        [CrabCommand("restart")]
        public static Task reloadAll(Match m, CommandContext context){
            context.Channel.SendMessageAsync("Restarting...").GetAwaiter().GetResult();
            Program.currentModuleManager.loadAllModules();
            return Task.CompletedTask;
        }

        [MentionOnly]
        [AdminOnly]
        [CrabCommand("shutdown")]
        public static Task shutdown(Match m, CommandContext context){
            context.Channel.SendMessageAsync("Shutting down...").GetAwaiter().GetResult();
            Program.shutdown();
            return Task.CompletedTask;
        }
    }
}