using Trinity.Shared;

namespace Trinity
{
    public interface IPlatformProvider
    {
        ITrinityUser CurrentUser { get; }

        public Task ConnectAsync();

        public Task ReConnectAsync();

        public Task DisconnectAsync();

        public Task<ITrinityChannel?> GetChannelAsync(TrinityGuid channelId);

        public Task<ITrinityGuild?> GetGuildAsync(TrinityGuid guildId);

        public List<ITrinityGuild> GetCachedGuilds();

        public event AsyncEvent<IPlatformProvider, MessageCreatedEventArgs> MessageRecieved;

        Task<bool> IsOwnerAsync(TrinityGuid id);
    }
}