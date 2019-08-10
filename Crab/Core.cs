using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Crab.Services;
using Microsoft.VisualStudio.Threading;
using Crab.Commands;
using Newtonsoft.Json.Linq;

namespace Crab
{
    [LogModule]
    public class MainCore : ModuleInstance
    {
        public static MainCore activeCore;

        public IServiceProvider _services;

        public async override Task startAsync()
        {
            activeCore = this;
            Program.client = new DiscordSocketClient();
            IConfiguration _config = ConfigUtils.getConfig();
            _services = ConfigureServices();
            _services.GetRequiredService<LogService>();
            await _services.GetRequiredService<CommandHandler>().loadAllModulesAsync();

            await Program.client.LoginAsync(TokenType.Bot, _config["token"]);
            await Program.client.StartAsync();
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                // Base
                .AddSingleton(Program.client)
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                // Logging
                .AddLogging()
                .AddSingleton<LogService>()
                // Extra
                .AddSingleton(ConfigUtils.getConfig())
                // Add additional services here...
                .BuildServiceProvider();
        }

        public override void shutdown()
        {
            _services.GetRequiredService<CommandHandler>().unloading();

            Program.client.LogoutAsync().GetAwaiter().GetResult();
            Program.client = null;
        }

        public override JObject get_jobject(){ return null; }

        public override void load_jobject(JObject obj){}
    }
}
