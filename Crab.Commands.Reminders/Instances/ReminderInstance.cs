using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;
using System.Linq;
using Crab.Commands;
using Discord.WebSocket;
using Discord;
using Newtonsoft.Json;

namespace Crab
{
    [HasDataFileAttribute("reminders")]
    public class ReminderInstance : ModuleInstance
    {
        private static List<Reminder> reminders = new List<Reminder>();

        public static int add_reminder(CommandContext context, DateTime time, string Message)
        {
            Reminder r = new Reminder(context, Message, time);
            reminders.Add(r);
            return reminders.IndexOf(r);
        }

        public static bool remove_reminder(int index, ulong UserID)
        {
            if((reminders.ElementAtOrDefault(index) != null) && (reminders[index].UserID == UserID))
            {
                return remove_reminder_without_permcheck(index);
            }
            return false;
        }

        public static bool remove_reminder_without_permcheck(int index)
        {
            try{
                reminders.RemoveAt(index);
            }catch(ArgumentOutOfRangeException)
            {
                return false;
            }
            return true;
        }

        public static List<Reminder> get_reminders_for(ulong UID)
        {
            return reminders.Where(t => (t.UserID == UID)).ToList();
        }

        public override async Task startAsync(){
            while(!shutting_down){
                List<Reminder> toRemove = new List<Reminder>();
                foreach(Reminder reminder in reminders.Where(t => (t.expired())))
                {
                    ISocketMessageChannel channel = reminder.Channel.get_channel(Program.client);

                    channel.SendMessageAsync($"_Buzz_ {Utils.mention(reminder.UserID)} {reminder.Message}");

                    toRemove.Add(reminder);
                }

                foreach (Reminder re in toRemove)
                {
                    reminders.Remove(re);
                }

                await Task.Delay(5000); //the loop will fire 5 seconds
            }
        }

        public override void shutdown(){}

        public override void load_jobject(JObject obj)
        {
            reminders = obj["reminders"].ToObject<List<Reminder>>();
        }

        public override JObject get_jobject()
        {
            JObject obj = new JObject();
            obj["reminders"] = JToken.FromObject(reminders);
            return obj;
        }
    }

    public class Reminder
    {
        public ulong UserID;
        public string Message;

        public DateTime Time;

        public ChannelReference Channel;

        public Reminder(CommandContext context, string message, DateTime time)
        {
            UserID = context.Invoker.Id;
            Message = message;
            Time = time;
            Channel = new ChannelReference(context.Channel as ISocketMessageChannel, context.Invoker.Id);
        }

        [JsonConstructor]
        public Reminder(ulong userID, string message, DateTime time, ChannelReference channel)
        {
            UserID = userID;
            Message = message;
            Time = time;
            Channel = channel;
        }

        public bool expired()
        {
            return (DateTime.Now > Time);
        }

    }
}