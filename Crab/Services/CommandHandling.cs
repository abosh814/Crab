using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Generic;
using Crab.Events;
using System.Linq;
using System.Text.RegularExpressions;

namespace Crab.Services
{
    [LogModule]
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;
        private Dictionary<Assembly, List<ModuleInfo>> loadedModules = new Dictionary<Assembly, List<ModuleInfo>>();
        private List<RegexCommand> regexCommands = new List<RegexCommand>();

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

            ModuleEvents.onLoad += loadModuleAsync;
            ModuleEvents.onUnload += unloadModuleAsync;
        }

        public async Task loadAllModulesAsync()
        {
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                await loadModuleAsync(ass);
            }
        }

        public async void loadModuleAsync(object sender, ModuleEventArgs args)
            => await loadModuleAsync(args.assembly);

        public async Task loadModuleAsync(Assembly ass)
        {
            //registering commands
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

                foreach (CommandInfo com in module.Commands)
                {
                    foreach (Attribute att in com.Attributes.Where(t => (t.GetType() == typeof(RegexAlias))))
                    {
                        Console.WriteLine($"found regex command: {com.Name}");
                        RegexAlias regex = (RegexAlias)att;
                        regexCommands.Add(new RegexCommand(regex.pattern,com.Name,ass));
                    }
                }
            }

            
        }

        public async void unloadModuleAsync(object sender, ModuleEventArgs args)
            => await unloadModuleAsync(args.assembly);

        public async Task unloadModuleAsync(Assembly ass)
        {
            if(!loadedModules.ContainsKey(ass))
                return;
            foreach (ModuleInfo mod in loadedModules[ass])
            {
                await _commands.RemoveModuleAsync(mod);
                Console.WriteLine($"unloaded command module: {mod.Name}");
            }

            List<RegexCommand> toRemove = new List<RegexCommand>();
            foreach (RegexCommand com in regexCommands)
            {
                if(com.assembly == ass)
                    toRemove.Add(com);
            }
            foreach (RegexCommand com in toRemove){ regexCommands.Remove(com); }

            loadedModules[ass] = new List<ModuleInfo>();
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages, or messages from other bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            RegexCommandResult regexCommand = await tryRegex(message.Content);
            if(regexCommand != null){
                Console.WriteLine($"trying out {regexCommand.toCommand()}");
                await _commands.ExecuteAsync(new SocketCommandContext(_discord, message), regexCommand.toCommand(), _services);
                return;
            }

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

        private async Task<RegexCommandResult> tryRegex(string input)
        {
            Console.WriteLine($"Trying regex for '{input}'");
            foreach (RegexCommand com in regexCommands)
            {
                Console.Write($"pattern: {com.pattern} - ");
                Match match = Regex.Match(input, com.pattern);
                if(match.Success){
                    Console.WriteLine("success!");
                    return new RegexCommandResult(match, com);
                }
                Console.WriteLine("failure");
            }
            return null;
        }
    }

    
    public class RegexCommand
    {
        public string pattern;
        public string command;
        public Assembly assembly; //for unloading
        public RegexCommand(string p, string c, Assembly a)
        {
            pattern = p;
            command = c;
            assembly = a;
        }
    }

    public class RegexCommandResult
    {
        Match match;
        RegexCommand regexC;
        public RegexCommandResult(Match m, RegexCommand rc)
        {
            match = m;
            regexC = rc;
        }

        public string toCommand(){
            List<string> res = new List<string>();
            res.Add(regexC.command);
            List<Group> groups = match.Groups.Values.ToList();
            groups.RemoveAt(0);
            foreach (Group group in groups)
            {
                res.Add(group.Value);
            }
            return String.Join(" ", res);
        }
    }
}