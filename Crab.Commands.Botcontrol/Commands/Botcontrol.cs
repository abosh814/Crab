using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Crab.Commands;

namespace Crab
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
            if(!ConfigUtils.isModule(m.Groups[1].Value)) return context.Channel.SendMessageAsync("Thats not a module!"); //That's not a module!

            if(Program.currentModuleManager.unloadModule(m.Groups[1].Value))
            {
                return context.Channel.SendMessageAsync($"Unloaded module `{m.Groups[1].Value}`");
            }
            else
            {
                return context.Channel.SendMessageAsync($"Couldn't unload module `{m.Groups[1].Value}`");
            }
        }

        [MentionOnly]
        [AdminOnly]
        [CrabCommand("reload (.+)")]
        //reload specific module
        public static Task reload(Match m, CommandContext context){
            string modulename = m.Groups[1].Value;
            if(modulename == "all") return reloadAll(context); //is it all?
            if(!ConfigUtils.isModule(modulename)) return context.Channel.SendMessageAsync("Thats not a module!"); //That's not a module!

            if(Program.currentModuleManager.loadModule(modulename)){
                return context.Channel.SendMessageAsync($"Reloaded module `{modulename}`");
            }
            else
            {
                return context.Channel.SendMessageAsync($"Couldn't reload module {modulename}");
            }
        }

        private static Task reloadAll(CommandContext context){
            Program.currentModuleManager.loadAllModules();
            return context.Channel.SendMessageAsync("Reloaded all modules");
        }
    }
}