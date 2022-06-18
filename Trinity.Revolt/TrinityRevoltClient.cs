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
        await MessageRecieved?.Invoke(this, new MessageCreatedEventArgs(new TrinityRevoltMessage(arg, Client.Channels.Get(arg.ChannelId))));
    }

    public RevoltClient Client { get; private set; }

    public ITrinityUser CurrentUser => new TrinityRevoltUser(Client.User);

    public event AsyncEvent<IPlatformProvider, MessageCreatedEventArgs> MessageRecieved;

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

    public Task<bool> IsOwnerAsync(TrinityGuid id)
    {
        if (id is TrinityRevoltStringGuid s)
        {
            return Task.FromResult(Client.User.Bot.OwnerId == s.Value);
        }
        return Task.FromResult(false);
    }
}