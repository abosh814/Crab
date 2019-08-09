using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System;
using Discord.WebSocket;
using Crab.Events;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Discord;

namespace Crab.Commands
{
    public class CommandHandler
    {
        public Dictionary<Assembly, List<CommandModule>> _loadedModules = new Dictionary<Assembly, List<CommandModule>>();
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

        public void unloading()
        {
            _discord.MessageReceived -= MessageReceivedAsync;

            ModuleEvents.onLoad -= loadModuleAsync;
            ModuleEvents.onUnload -= unloadModuleAsync;
        }

        public async Task loadAllModulesAsync()
        {
            //clear list
            _loadedModules = new Dictionary<Assembly, List<CommandModule>>();

            //populate it again
            foreach (var item in Program.currentModuleManager._modules)
            {
                foreach (Assembly ass in item.Value.context.Assemblies)
                {
                    await loadModuleAsync(ass);
                }
            }
        }

        public async void loadModuleAsync(object sender, ModuleEventArgs args)
            => await loadModuleAsync(args.assembly);

        public Task loadModuleAsync(Assembly ass)
        {
            //get all Commands in an assembly TODO
            //registering commands
            foreach (Type module in ass.GetTypes().Where(t => (t.BaseType == typeof(CrabCommandModule))))
            {
                CommandModule c_module = new CommandModule(module);
                if(!_loadedModules.ContainsKey(ass)){
                    List<CommandModule> list = new List<CommandModule>();
                    list.Add(c_module);
                    _loadedModules.Add(ass, list);
                }else{
                    _loadedModules[ass].Add(c_module);
                }
                Console.WriteLine($"loaded command module: {c_module.Name}");
            }
            return Task.CompletedTask;
        }

        public async void unloadModuleAsync(object sender, ModuleEventArgs args)
            => await unloadModuleAsync(args.assembly);

        public Task unloadModuleAsync(Assembly ass)
        {
            if(!_loadedModules.ContainsKey(ass))
                return Task.CompletedTask;
            foreach (CommandModule mod in _loadedModules[ass])
            {
                Console.WriteLine($"unloaded command module: {mod.Name}");
            }
            _loadedModules.Remove(ass);
            return Task.CompletedTask;
        }

        public Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            // Ignore system messages, or messages from other bots
            if (!(rawMessage is SocketUserMessage message)) return Task.CompletedTask;
            if (message.Source != MessageSource.User) return Task.CompletedTask;

            var context = new CommandContext(_discord, message);
            // Perform the execution of the command. In this method,
            // the command service will perform precondition and parsing check
            // then execute the command if one is matched.
            foreach (var assembly in _loadedModules)
            {
                CommandResult latest = CommandResult.FromNeutral();
                try{
                    foreach (CommandModule module in assembly.Value)
                    {
                        CommandResult newresult = module.tryExecute(context);
                        if(latest.isSmallerThan(newresult))
                            latest = newresult;
                        if(latest.shouldExit())
                            break;
                    }
                }catch(Exception e)
                {
                    latest = CommandResult.FromException(e);
                }

                //handling the commandresult
                if(latest.emote != null){
                    message.AddReactionAsync(latest.emote);
                }

                if(latest.message != null){
                    context.Channel.SendMessageAsync(latest.message);
                }
                
                if(latest.result == ExecutionResult.EXCEPTION)
                {
                    //pinging admins
                    string mentions = "";
                    foreach (var key in Utils.get_all_admin_keys())
                    {
                        mentions += $"<@!{key}>";
                    }
                    context.Channel.SendMessageAsync($"{mentions} Exception occured in command execution, check logs.");

                    //logs
                    Console.WriteLine($"Exception occured while trying to execute command for message {message}");
                    Console.WriteLine(latest.exception);
                }
            }
            return Task.CompletedTask;
        }
    }

    public class CommandModule
    {
        //have modules be able to have requirements TODO
        public CommandResult tryExecute(CommandContext context)
        {
            CommandResult latest = CommandResult.FromNeutral();
            try{
                foreach (Command command in _commands)
                {
                    CommandResult newresult = command.tryExecute(context);
                    if(latest.isSmallerThan(newresult))
                        latest = newresult;
                    if(latest.shouldExit())
                        return latest;
                }
            }catch(Exception e)
            {
                latest = CommandResult.FromException(e);
            }
            return latest;
        }

        public CommandModule(string n, List<Command> commands)
        {
            Name = n;
            _commands = commands;
        }
        public CommandModule(Type module)
        {
            Name = module.Name;
            _commands = new List<Command>();

            foreach (MethodInfo func in module.GetMethods())
            {
                if(!Command.isCommand(func))
                    continue;

                _commands.Add(new Command(func));
            }
        }

        public readonly string Name;
        public readonly List<Command> _commands;
    }

    public class Command
    {
        public Command(MethodInfo m)
        {
            method = m;
            foreach (var att in m.GetCustomAttributes())
            {
                switch (att)
                {
                    case CrabCommandAttribute command:
                        _aliases.Add(command.pattern);
                        break;
                    case CrabPreconditionAttribute precondition:
                        preconditions.Add(precondition);
                        break;
                    case ContextManipulatorAttribute ccmanipulator:
                        manipulators.Add(ccmanipulator);
                        break;
                    default:
                        break;
                }
            }
        }
        public static bool isCommand(MethodInfo m)
             => m?.GetCustomAttribute(typeof(CrabCommandAttribute)) != null;
        public CommandResult tryExecute(CommandContext context)
        {
            try{
                foreach (string alias in _aliases)
                {
                    Match match = Regex.Match(context.Message.Content, alias);
                    if(match.Success){
                        //try requirements
                        foreach (CrabPreconditionAttribute precon in preconditions)
                        {
                            PreconditionResult precon_res = precon.check(context);
                            if(!precon_res.Success){
                                return CommandResult.FromFailure(precon_res);
                            }
                        }

                        //manipulate
                        foreach (ContextManipulatorAttribute ccm in manipulators)
                        {
                            ccm.modify(ref context);
                        }

                        //try execute func with match & context TODO
                        method.Invoke(null, new object[] {match, context});
                        //also make it async TODO
                        return CommandResult.FromSuccess();
                    }
                }
            }
            catch(Exception e)
            {
                return CommandResult.FromException(e);
            }
            return CommandResult.FromNeutral();
        }

        //aliases to run regex over
        public readonly List<string> _aliases = new List<string>();
        //Preconditions
        private List<CrabPreconditionAttribute> preconditions = new List<CrabPreconditionAttribute>();
        //function
        private MethodInfo method;
        //context manipulators
        private List<ContextManipulatorAttribute> manipulators = new List<ContextManipulatorAttribute>();
    }
}