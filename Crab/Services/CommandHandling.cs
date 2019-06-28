using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;

namespace Crab.Services
{
    public class CommandHandlingService : CrabModule
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;
        private Dictionary<Assembly, List<ModuleInfo>> loadedModules = new Dictionary<Assembly, List<ModuleInfo>>();

        public CommandHandlingService(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            // Hook CommandExecuted to handle post-command-execution logic.
            _commands.CommandExecuted += CommandExecutedAsync;
            // Hook MessageReceived so we can process each message to see
            // if it qualifies as a command.
            _discord.MessageReceived += MessageReceivedAsync;
        }

        public async void loadModuleAsync(Assembly ass)
        {
            IEnumerable<ModuleInfo> modules = await _commands.AddModulesAsync(ass, _services);
            foreach (ModuleInfo module in modules)
            {
                if(!loadedModules.ContainsKey(ass)){
                    List<ModuleInfo> list = new List<ModuleInfo>();
                    list.Add(module);
                    loadedModules.Add(ass, list);
                }else{
                    loadedModules[ass].Add(module);
                }
                Console.WriteLine($"loaded command module: {module.Name}");
            }
        }

        public async void unloadModuleAsync(Assembly ass)
        {
            if(!loadedModules.ContainsKey(ass))
                return;
            foreach (ModuleInfo mod in loadedModules[ass])
            {
                await _commands.RemoveModuleAsync(mod);
                loadedModules[ass].Remove(mod);
                Console.WriteLine($"unloaded command module: {mod.Name}");
            }
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages, or messages from other bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            // This value holds the offset where the prefix ends
            var argPos = 0;
            // Perform prefix check. You may want to replace this with
            // (!message.HasCharPrefix('!', ref argPos))
            // for a more traditional command format like !help.
            if (!message.HasMentionPrefix(_discord.CurrentUser, ref argPos)) return;

            var context = new SocketCommandContext(_discord, message);
            // Perform the execution of the command. In this method,
            // the command service will perform precondition and parsing check
            // then execute the command if one is matched.
            await _commands.ExecuteAsync(context, argPos, _services); 
            // Note that normally a result will be returned by this format, but here
            // we will handle the result in CommandExecutedAsync,
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // command is unspecified when there was a search failure (command not found); we don't care about these errors
            if (!command.IsSpecified)
                return;

            // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
            if (result.IsSuccess)
                return;

            // the command failed, let's notify the user that something happened.
            string mentions = "";
            foreach (var key in Utils.get_all_admin_keys())
            {
                mentions += $"<@!{key}>";
            }
            await context.Channel.SendMessageAsync($"{mentions} we got an error");
        }
    }
}