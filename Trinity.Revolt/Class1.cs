using Revolt;
using Trinity.Shared;

namespace Trinity.Revolt
{
    public class TrinityRevoltClient : IPlatformProvider
    {
        private readonly TokenType tokenType;
        private readonly string token;

        public TrinityRevoltClient(RevoltClient client, TokenType tokenType, string token)
        {
            Client = client;
            this.tokenType = tokenType;
            this.token = token;
            Client.MessageReceived += Client_MessageReceived;
        }

        private async Task Client_MessageReceived(Message arg)
        {
            await MessageRecieved?.Invoke(this, new MessageRecievedArgs(new TrinityRevoltMessage(arg)));
        }

        public RevoltClient Client { get; private set; }

        public event AsyncEvent<IPlatformProvider, MessageRecievedArgs> MessageRecieved;

        public async Task ConnectAsync()
        {
            await Client.LoginAsync(tokenType, token);
            await Client.ConnectWebSocketAsync();
            await Client.CacheAll();
        }

        public Task DisconnectAsync()
        {
            Client.DisconnectWebsocket();
            return Task.CompletedTask;
        }

        public List<ITrinityGuild> GetCachedGuilds()
        {
            return Client.ServersCache.Select(x => (ITrinityGuild)new TrinityRevoltGuild(x, Client)).ToList();
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