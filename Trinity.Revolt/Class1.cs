using RevoltSharp;
using Trinity.Shared;

namespace Trinity.Revolt
{
    public class TrinityRevoltClient : IPlatformProvider
    {
        public TrinityRevoltClient(RevoltClient client)
        {
            Client = client;
        }

        public RevoltClient Client { get; private set; }

        public event AsyncEvent<IPlatformProvider, MessageRecievedArgs> MessageRecieved;

        public Task ConnectAsync()
        {
            return Client.StartAsync();
        }

        public Task DisconnectAsync()
        {
            return Client.StopAsync();
        }

        public List<ITrinityGuild> GetCachedGuilds()
        {
            return Client.Servers.Select(x => (ITrinityGuild)new TrinityRevoltGuild(x)).ToList();
        }

        public Task<ITrinityChannel?> GetChannelAsync(TrinityGuid channelId)
        {
            throw new NotImplementedException();
        }

        public Task<ITrinityGuild?> GetGuildAsync(TrinityGuid guildId)
        {
            throw new NotImplementedException();
        }

        public Task ReConnectAsync()
        {
            throw new NotImplementedException();
        }
    }
}