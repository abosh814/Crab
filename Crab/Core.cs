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

        public static CommandHandlingService _command;

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            IConfiguration _config = Utils.getConfig();

            var services = ConfigureServices();
            services.GetRequiredService<LogService>();
            //await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
            _command = services.GetRequiredService<CommandHandlingService>();

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
