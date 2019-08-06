using Discord.WebSocket;

namespace Crab.Commands
{
    public class CommandContext
    {
        public readonly DiscordSocketClient Crab;
        //public SocketGuild Guild { get; }
        public ISocketMessageChannel Channel; //this can be changed to allow [DMResponse] attribute to change the channel to a dm channel
        
        public readonly SocketUser Invoker;
        public readonly SocketUserMessage Message;
        //public bool IsPrivate { get; }
        public CommandContext(DiscordSocketClient client, SocketUserMessage msg)
        {
            Crab = client;
            Channel = msg.Channel;
            Invoker = msg.Author;
            Message = msg;
        }

    }
}