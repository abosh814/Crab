using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System;

namespace Crab.Commands.Reminders
{
    [LogModule]
    public class ReminderModule : CrabCommandModule
    {
        [MentionOnly]
        [CrabCommand("remind(?: ?me|er)?\\s+(\\S+)\\s+(.+)")]
        public static Task remind(Match m, CommandContext context)
        {
            DateTime time = ReminderUtils.parseTime(m.Groups[1].Value);
            if(time < DateTime.Now){
                return context.Channel.SendMessageAsync("_Buzz_ no time travel, nerd.");
            }
            
            int id = ReminderInstance.add_reminder(context, time, m.Groups[2].Value);

            return context.Channel.SendMessageAsync($"#{id} coming in at {time.ToString("R")}");
        }

        [MentionOnly]
        [CrabCommand("unremind #?(\\d+)")]
        public static Task unremind(Match match, CommandContext context)
        {
            bool success = false;
            int id = Convert.ToInt32(match.Groups[1].Value);

            if(Utils.isadmin(context.Invoker.Id))
            {
                success = ReminderInstance.remove_reminder_without_permcheck(id);
            }else{
                success = ReminderInstance.remove_reminder(id, context.Invoker.Id);
            }

            if(success){
                return context.Channel.SendMessageAsync($"Successfully removed #{id}");
            }else{
                return context.Channel.SendMessageAsync($"Couldn't remove #{id}, maybe its not your Reminder? Try `remindlist` to see your reminders.");
            }
        }

        [AdminOnly]
        [MentionOnly]
        [CrabCommand("remindlist (\\d+)")]
        public static Task remindListAdmin(Match match, CommandContext context)
            => context.Channel.SendMessageAsync(embed: ReminderUtils.remindListEmbed(Convert.ToUInt64(match.Groups[1].Value)));

        [MentionOnly]
        [CrabCommand("remindlist")]
        public static Task remindListUser(Match match, CommandContext context)
            => context.Channel.SendMessageAsync(embed: ReminderUtils.remindListEmbed(context.Invoker.Id));

    }
}