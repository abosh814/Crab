using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Crab.Services;
using Microsoft.VisualStudio.Threading;

namespace Crab
{
    [LogModule]
    public class Core : CrabCore
    {
        private DiscordSocketClient _client;

        public IServiceProvider _services;

        public new static int loaded()
            => new Core().MainAsync().GetAwaiter().GetResult();

        public async Task<int> MainAsync()
        {
            _client = new DiscordSocketClient();
            IConfiguration _config = ConfigUtils.getConfig();
            _services = ConfigureServices();
            _services.GetRequiredService<LogService>();
            await _services.GetRequiredService<CommandHandlingService>().loadAllModulesAsync();

            await _client.LoginAsync(TokenType.Bot, _config["token"]);
            await _client.StartAsync();

            if(exitEvent != null)
                exitEvent.Set();
            exitEvent = new AsyncManualResetEvent();
            await exitEvent.WaitAsync();
            await _client.LogoutAsync();
            return exitCode;
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                // Base
                .AddSingleton(_client)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                // Logging
                .AddLogging()
                .AddSingleton<LogService>()
                // Extra
                .AddSingleton(ConfigUtils.getConfig())
                // Add additional services here...
                .BuildServiceProvider();
        }

        private static int exitCode;
        private static AsyncManualResetEvent exitEvent;

        public static void exit(int code)
        {
            exitCode = code;
            exitEvent.Set();
        }
    }
}
