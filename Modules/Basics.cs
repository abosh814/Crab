using System.Threading.Tasks;
using Discord.Commands;
using Microsoft.Extensions.Configuration;

namespace Crab.Modules
{
    public class Basics : ModuleBase<SocketCommandContext>
    {
        [Command("Hi")]
        public Task Hi()
            => ReplyAsync($"Hi");

        [Command("Who am I")]
        public Task Who()
            => ReplyAsync($"You are {Context.User.Username}.");

        [Command("Say")]
        public Task Say([Remainder] string message)
            => ReplyAsync($"{message}");

        [Command("myid")]
        public Task MyID()
            => ReplyAsync($"{Context.User.Id}");

        [Command("admincheck")]
        public Task admincheck(string repo){
            IConfiguration config = Utils.GetConfig();
            /*if(!config["repos"][repo]){
                return ReplyAsync($"Unknown Repoprefix: \"{repo}\"");
            }*/


            return ReplyAsync($"{config["repos"]}");
        }
    }
}