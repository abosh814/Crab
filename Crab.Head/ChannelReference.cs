using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace Crab
{
    public class ChannelReference
    {
        public readonly bool isPrivate;
        public readonly ulong UserID; //incase we dont find the channel anymore or we are in a dm channel
        public readonly ulong GuildID;
        public readonly ulong ChannelID;
        public ChannelReference(ISocketMessageChannel channel, ulong userID)
        {
            isPrivate = (channel is IPrivateChannel);
            UserID = userID;
            if(!isPrivate){
                GuildID = (channel as SocketGuildChannel).Guild.Id;
                ChannelID = channel.Id;
            }
        }

        [JsonConstructor]
        public ChannelReference(bool isprivate, ulong userID, ulong guildID, ulong channelID)
        {
            isPrivate = isprivate;
            UserID = userID;
            GuildID = guildID;
            ChannelID = channelID;
        }

        public ISocketMessageChannel get_channel(DiscordSocketClient client)
        {
            if(!isPrivate && client.GetGuild(GuildID)?.GetChannel(ChannelID) != null){
                return client.GetGuild(GuildID)?.GetChannel(ChannelID) as ISocketMessageChannel;
            }
            return client.GetUser(UserID).GetOrCreateDMChannelAsync() as ISocketMessageChannel;
        }
    }
}