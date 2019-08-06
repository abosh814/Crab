using Crab.Commands;
using Discord;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crab
{
    public class CommandHandlerCommands : CrabCommandModule
    {
        [MentionOnly]
        [CrabCommand("(?>commands|help)")]
        public static Task listCommands(Match m, CommandContext context){
            CommandHandler ch = (CommandHandler)MainCore.activeCore?._services.GetService(typeof(CommandHandler));
            EmbedBuilder embed = new EmbedBuilder();
            embed.WithTitle("All Commands");
            foreach (var item in ch._loadedModules)
            {
                foreach (CommandModule module in item.Value)
                {
                    string command_list = "";
                    foreach (var command in module._commands)
                    {
                        command_list += $"{String.Join(", ", command._aliases)}\n";
                    }
                    embed.AddField(module.Name, command_list);
                }
            }
            return context.Channel.SendMessageAsync(embed: embed.Build());
        }
    }
}