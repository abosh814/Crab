using System.Threading.Tasks;
using Discord.Commands;
using System;
using System.Text.RegularExpressions;
using Crab.Commands;

namespace Crab
{
    [LogModule]
    public class Basics : CrabCommandModule
    {
        [CrabCommand("Hi")]
        public Task Hi(Match m, SocketCommandContext context)
            => context.Channel.SendMessageAsync($"Hello!");

        [CrabCommand("Who am I")]
        public Task Who(Match m, SocketCommandContext context)
            => context.Channel.SendMessageAsync($"You are {context.User.Username}.");

        [CrabCommand("say .*")]
        public Task Say(Match m, SocketCommandContext context)
            => context.Channel.SendMessageAsync($"{m.Groups[1]}");

        [CrabCommand("id (my|\\d*)")]
        public Task Id(Match m, SocketCommandContext context){
            if(m.Groups.Count <= 1 || m.Groups[1].Value == "")
                return context.Channel.SendMessageAsync("You need to specify an ID");
            if(m.Groups[1].Value == "my")
                return context.Channel.SendMessageAsync(BasicsUtils.idinfo(context.User.Id));
            return context.Channel.SendMessageAsync(BasicsUtils.idinfo(Convert.ToUInt64(m.Groups[1].Value)));
        }

        //[AdminCommand]
        [CrabCommand("config")]
        public Task config(Match m, SocketCommandContext context)
            => context.Channel.SendMessageAsync(BasicsUtils.listConfig());
    }
}