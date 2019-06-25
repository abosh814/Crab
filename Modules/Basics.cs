using System.Threading.Tasks;
using Discord.Commands;
using System;

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

        [Group("id")]
        public class IdModule : ModuleBase<SocketCommandContext>
        {

            [Command]
            public Task Other(){
                return ReplyAsync("You need to specify an ID");
            }

            [Command]
            public Task Other(string id){
                if(id == "my")
                    return ReplyAsync(Utils.idinfo(Context.User.Id));
                return ReplyAsync(Utils.idinfo(Convert.ToUInt64(id)));
            }
        }
    }
}