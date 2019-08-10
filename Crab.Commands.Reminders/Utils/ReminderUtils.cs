using System;
using System.Text.RegularExpressions;
using Discord;
using System.Collections.Generic;
using System.Reflection;

namespace Crab
{
    public static class ReminderUtils
    {
        public static string regex_date = "(?:(\\d{4})[\\/-](\\d\\d)[\\/-](\\d\\d))?(?:(?(1)@)(\\d\\d)(?::(\\d\\d)(?::(\\d\\d))?)?)?";
        public static string regex_relative_date = "(?:\\d+[dhmsw])+";
        public static string regex_relative_date_section = "(\\d+)([dhmsw])";
        public static DateTime parseTime(string timeString)
        {
            DateTime delta = DateTime.Now;
            Match match = Regex.Match(timeString, regex_date);
            Console.WriteLine(timeString);
            Console.WriteLine(match.Groups[0].Value.Replace("@", " "));
            if(match.Success && ((match.Groups[1].Success && match.Groups[2].Success && match.Groups[3].Success) || (match.Groups[4].Success && match.Groups[5].Success))){
                //group 3 and 6 doesn't matter
                delta = DateTime.Parse(match.Groups[0].Value.Replace("@", " "));
            }else{
                match = Regex.Match(timeString, regex_relative_date);
                if(match.Success){
                    MatchCollection sections = Regex.Matches(timeString, regex_relative_date_section);
                    foreach (Match item in sections)
                    {
                        switch (item.Groups[2].Value)
                        {
                            case "d":
                                delta = delta.AddDays(Convert.ToDouble(item.Groups[1].Value));
                                break;
                            case "h":
                                delta = delta.AddHours(Convert.ToDouble(item.Groups[1].Value));
                                break;
                            case "m":
                                delta = delta.AddMinutes(Convert.ToDouble(item.Groups[1].Value));
                                break;
                            case "s":
                                delta = delta.AddSeconds(Convert.ToDouble(item.Groups[1].Value));
                                break;
                            case "w":
                                delta = delta.AddDays(Convert.ToDouble(item.Groups[1].Value)*7);
                                break;
                        }
                    }
                }
            }
            
            return delta;
        }

        public static Embed remindListEmbed(ulong ID)
        {
            List<Reminder> reminders = ReminderInstance.get_reminders_for(ID);
            EmbedBuilder embed = new EmbedBuilder();

            if(reminders.Count != 0){
                embed.WithTitle($"Reminders for UID {ID}");
                foreach (Reminder reminder in reminders)
                {
                    embed.AddField($"#{reminders.IndexOf(reminder)} Reminder at {reminder.Time.ToString("R")}", (reminder.Message != null) ? reminder.Message : "No Message");
                }
            }else{
                embed.WithTitle($"No reminders for UID {ID}");
            }
            return embed.Build();
        }
    }
}