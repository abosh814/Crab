using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Crab.Commands.Basics
{
    public class ChanceModule : CrabCommandModule
    {
        [MentionOnly]
        [CrabCommand("(?:pick|choose)\\s*\\((.*?)\\)")]
        public static Task pick(Match match, CommandContext context)
        {
            string[] choices = match.Groups[1].Value.Split(",");
            int index = new Random().Next(choices.Length);
            return context.Channel.SendMessageAsync($"**{choices[index].Trim()}**");
        }

        [MentionOnly]
        [CrabCommand("(\\d+)d(\\d+)(?:\\+(\\d+))?")]
        public static Task roll(Match match, CommandContext context)
        {
            int count = Convert.ToInt32(match.Groups[1].Value);
            if(count > 100)
                return context.Channel.SendMessageAsync("Ok look dude. A minute or two after this dice command got implemented bobda ran a god damn 10000000000000000000000000000d10. Now because it has to ITERATE those dice and 10000000000000000000000000000 is a giant fucking number, that locked up MoMMI completely because no amount of asyncio is gonna save this madness. Thank god for SIGKILL. THEN I got pinged by Intigracy telling me MoMMI locked up. *sigh*\n*I just copied this from mommiv2 cause it made sense and was funny*");

            List<int> results = new List<int>(); 
            for (int i = 0; i < count; i++)
            {
                int roll = new Random().Next(1, Convert.ToInt32(match.Groups[2].Value));
                results.Add(roll);
            }

            int mod = 0;
            if(match.Groups[3].Success)
                mod = Convert.ToInt32(match.Groups[3].Value);

            return context.Channel.SendMessageAsync($"Results: {String.Join(", ",results.ConvertAll<string>(Convert.ToString))}{((mod != 0) ? " + "+mod : "")} = {results.Sum()}");
        }

        [MentionOnly]
        [CrabCommand("rand\\s*(-?\\d+)\\s*(-?\\d+)")]
        public static Task rand(Match match, CommandContext context)
        {
            return context.Channel.SendMessageAsync(new Random().Next(Convert.ToInt32(match.Groups[1].Value), Convert.ToInt32(match.Groups[2].Value)).ToString());
        }

        [MentionOnly]
        [CrabCommand("(?:magic|magic8ball)")]
        public static Task magic8ball(Match match, CommandContext context)
        {
            string[] choices = {
                "It is certain",
                "It is decidedly so",
                "Without a doubt",
                "Yes, definitely",
                "You may rely on it",
                "As I see it, yes",
                "Most likely",
                "Outlook: Positive",
                "Yes",
                "Signs point to: Yes",
                "Reply hazy, try again",
                "Ask again later",
                "Better to not tell you right now",
                "Cannot predict now",
                "Concentrate, then ask again",
                "Do not count on it",
                "My reply is: no",
                "My sources say: no",
                "Outlook: Negative",
                "Very doubtful"
            };
            return context.Channel.SendMessageAsync(choices[new Random().Next(choices.Length)]);
        }
    }
}