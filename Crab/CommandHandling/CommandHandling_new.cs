using System.Collections.Generic;
using System.Reflection;
using Discord.Commands;
using System.Text.RegularExpressions;
using System;
using Discord.WebSocket;
using Crab.Events;
using System.Threading.Tasks;

namespace Crab.Commands
{
    public class CommandHandler
    {
        private Dictionary<Assembly, List<CommandModule>> _loadedModules = new Dictionary<Assembly, List<CommandModule>>();
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;

        public CommandHandler(IServiceProvider services)
        {
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            _discord.MessageReceived += MessageReceivedAsync;

            ModuleEvents.onLoad += loadModuleAsync;
            ModuleEvents.onUnload += unloadModuleAsync;
        }

        public async Task loadAllModulesAsync()
        {
            //clear list
            _loadedModules = new Dictionary<Assembly, List<CommandModule>>();

            //populate it again
            foreach (Assembly ass in AppDomain.CurrentDomain.GetAssemblies())
            {
                await loadModuleAsync(ass);
            }
        }

        public async void loadModuleAsync(object sender, ModuleEventArgs args)
            => await loadModuleAsync(args.assembly);

        public async Task loadModuleAsync(Assembly ass)
        {
            //get all Commands in an assembly TODO
            //registering commands
            foreach (CommandModule module in modules)
            {
                if(!_loadedModules.ContainsKey(ass)){
                    List<ModuleInfo> list = new List<ModuleInfo>();
                    list.Add(module);
                    _loadedModules.Add(ass, list);
                }else{
                    _loadedModules[ass].Add(module);
                }
                Console.WriteLine($"loaded command module: {module.Name}");
            }
        }

        public async void unloadModuleAsync(object sender, ModuleEventArgs args)
            => await unloadModuleAsync(args.assembly);

        public async Task unloadModuleAsync(Assembly ass)
        {
            if(!_loadedModules.ContainsKey(ass))
                return;
            foreach (CommandModule mod in _loadedModules[ass])
            {
                Console.WriteLine($"unloaded command module: {mod.Name}");
            }
            _loadedModules.Remove(ass);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages, or messages from other bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;
            // (!message.HasCharPrefix('!', ref argPos)) -- make this a requirement?
            if (!message.HasMentionPrefix(_discord.CurrentUser, ref argPos)) return; //make this a requirement?

            var context = new SocketCommandContext(_discord, message);
            // Perform the execution of the command. In this method,
            // the command service will perform precondition and parsing check
            // then execute the command if one is matched.
            foreach (var assembly in _loadedModules)
            {
                foreach (CommandModule module in assembly.Value)
                {
                    if(await module.tryExecute(context))
                        return;
                }
            }
        }
    }

    public class CommandModule
    {
        public async Task<bool> tryExecute(SocketCommandContext context)
        {
            foreach (Command command in _commands)
            {
                if(await command.tryExecute(context))
                    return true;
            }
            return false;
        }

        public CommandModule(string n, List<Command> commands)
        {
            Name = n;
            _commands = commands;
        }

        public readonly string Name;
        private readonly List<Command> _commands;
    }

    public class Command
    {
        public async Task<bool> tryExecute(SocketCommandContext context)
        {
            //try requirements

            foreach (string alias in _aliases)
            {
                Match match = Regex.Match(context.Message.Content, alias);
                if(match.Success){
                    //try execute func with match & context TODO
                    //also make it async
                    return true;
                }
            }
            return false;
        }

        //aliases to run regex over
        public readonly List<string> _aliases;
        //requirementattributes TODO
        //function ref TODO
    }
}