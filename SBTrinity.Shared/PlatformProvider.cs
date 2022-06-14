using Trinity.Shared;

namespace Trinity
{
    public interface IPlatformProvider
    {
        public Task ConnectAsync();

        public Task ReConnectAsync();

        public Task DisconnectAsync();

        public Task<ITrinityChannel?> GetChannelAsync(TrinityGuid channelId);

        public Task<ITrinityGuild?> GetGuildAsync(TrinityGuid guildId);

        public List<ITrinityGuild> GetCachedGuilds();

        public event AsyncEvent<IPlatformProvider, MessageRecievedArgs> MessageRecieved;
    }

    public class MessageRecievedArgs : TrinityEventArgs
    {
        public MessageRecievedArgs(ITrinityMessage m)
        {
            Message = m;
        }

        public ITrinityMessage Message;
        public ITrinityChannel Channel => Message.Channel;
        public ITrinityUser Author => Message.Author;
    }
}