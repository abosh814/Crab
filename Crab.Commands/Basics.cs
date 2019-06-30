using System.Threading.Tasks;
using Discord.Commands;
using System;

namespace Crab
{
    [LogModule]
    public class Basics : ModuleBase<SocketCommandContext>
    {
        [Command("Hi")]
        public Task Hi()
            => ReplyAsync($"Hello!");

        [Command("Who am I")]
        public Task Who()
            => ReplyAsync($"You are {Context.User.Username}.");

        [Command("Say")]
        public Task Say([Remainder] string message)
            => ReplyAsync($"{message}");

        [LogModule]
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
                    return ReplyAsync(BasicsUtils.idinfo(Context.User.Id));
                return ReplyAsync(BasicsUtils.idinfo(Convert.ToUInt64(id)));
            }
        }

        [AdminCommand]
        [Command("config")]
        public Task config()
            => ReplyAsync(BasicsUtils.listConfig());
    }
}