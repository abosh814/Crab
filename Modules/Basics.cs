using System.Threading.Tasks;
using Discord.Commands;

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
        public Task Say(string message)
            => ReplyAsync($"{message}");
    }
}