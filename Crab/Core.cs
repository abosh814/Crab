using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Crab.Services;

namespace Crab
{
    public class Core : CrabModule
    {
        private DiscordSocketClient _client;

        public IServiceProvider _services;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            IConfiguration _config = Utils.getConfig();
            _services = ConfigureServices();
            _services.GetRequiredService<LogService>();
            await _services.GetRequiredService<CommandHandlingService>().loadAllModulesAsync();

            await _client.LoginAsync(TokenType.Bot, _config["token"]);
            await _client.StartAsync();

            await Task.Delay(-1);
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
                .AddSingleton(Utils.getConfig())
                // Add additional services here...
                .BuildServiceProvider();
        }
    }
}
