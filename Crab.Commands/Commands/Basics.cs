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
        public static Task Hi(Match m, SocketCommandContext context)
            => context.Channel.SendMessageAsync($"Hello!");

        [CrabCommand("Who am I")]
        public static Task Who(Match m, SocketCommandContext context)
            => context.Channel.SendMessageAsync($"You are {context.User.Username}.");

        [CrabCommand("say (.*)")]
        public static Task Say(Match m, SocketCommandContext context)
            => context.Channel.SendMessageAsync($"{m.Groups[1]}");

        [CrabCommand("id (my|\\d*)")]
        public static Task Id(Match m, SocketCommandContext context){
            if(m.Groups.Count <= 1 || m.Groups[1].Value == "")
                return context.Channel.SendMessageAsync("You need to specify an ID");
            if(m.Groups[1].Value == "my")
                return context.Channel.SendMessageAsync(BasicsUtils.idinfo(context.User.Id));
            return context.Channel.SendMessageAsync(BasicsUtils.idinfo(Convert.ToUInt64(m.Groups[1].Value)));
        }

        [CrabCommand("memorytest (.*)")]
        public static Task memorytest(Match m, SocketCommandContext context){
            if(BasicInstance.saveTest != "")
                context.Channel.SendMessageAsync("Was remembering "+BasicInstance.saveTest);
            BasicInstance.saveTest = m.Groups[1].Value;
            return context.Channel.SendMessageAsync("Now remembering "+BasicInstance.saveTest);
        }

        //[AdminCommand]
        [CrabCommand("config")]
        public static Task config(Match m, SocketCommandContext context)
            => context.User.GetOrCreateDMChannelAsync().GetAwaiter().GetResult().SendMessageAsync(BasicsUtils.listConfig());
    }
}