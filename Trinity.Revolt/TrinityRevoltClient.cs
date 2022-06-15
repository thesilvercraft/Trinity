using Revolt;
using Trinity.Shared;

namespace Trinity.Revolt;

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

    public async Task<ITrinityChannel?> GetChannelAsync(TrinityGuid channelId)
    {
        return new TrinityRevoltChannel(((TrinityRevoltStringGuid)channelId).Value, Client);
    }

    public async Task<ITrinityGuild?> GetGuildAsync(TrinityGuid guildId)
    {
        return new TrinityRevoltGuild(Client.ServersCache.FirstOrDefault(x => x._id == ((TrinityRevoltStringGuid)guildId).Value) ?? throw new KeyNotFoundException("A channel with that id could not be found"), Client);
    }

    public Task ReConnectAsync()
    {
        Client.DisconnectWebsocket();
        return Client.ConnectWebSocketAsync();
    }
}