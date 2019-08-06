using System.Threading.Tasks;
using System;
using System.Text.RegularExpressions;
using Crab.Commands;

namespace Crab
{
    [LogModule]
    public class Basics : CrabCommandModule
    {
        [MentionOnly]
        [CrabCommand("Hi")]
        public static Task Hi(Match m, CommandContext context)
            => context.Channel.SendMessageAsync($"Hello!");

        [MentionOnly]
        [CrabCommand("Who am I")]
        public static Task Who(Match m, CommandContext context)
            => context.Channel.SendMessageAsync($"You are {context.Invoker.Username}.");

        [MentionOnly]
        [CrabCommand("say (.*)")]
        public static Task Say(Match m, CommandContext context)
            => context.Channel.SendMessageAsync($"{m.Groups[1]}");

        [MentionOnly]
        [CrabCommand("id (my|\\d*)")]
        public static Task Id(Match m, CommandContext context){
            if(m.Groups.Count <= 1 || m.Groups[1].Value == "")
                return context.Channel.SendMessageAsync("You need to specify an ID");
            if(m.Groups[1].Value == "my")
                return context.Channel.SendMessageAsync(BasicsUtils.idinfo(context.Invoker.Id));
            return context.Channel.SendMessageAsync(BasicsUtils.idinfo(Convert.ToUInt64(m.Groups[1].Value)));
        }

        [MentionOnly]
        [AdminOnly]
        [CrabCommand("memorytest (.*)")]
        public static Task memorytest(Match m, CommandContext context){
            if(BasicInstance.saveTest != "")
                context.Channel.SendMessageAsync("Was remembering "+BasicInstance.saveTest);
            BasicInstance.saveTest = m.Groups[1].Value;
            return context.Channel.SendMessageAsync("Now remembering "+BasicInstance.saveTest);
        }

        [MentionOnly]
        [AdminOnly]
        [CrabCommand("config")]
        public static Task config(Match m, CommandContext context)
            => context.Invoker.GetOrCreateDMChannelAsync().GetAwaiter().GetResult().SendMessageAsync(BasicsUtils.listConfig());
    }
}