using Revolt;
using Revolt.Channels;
using Trinity.Shared;

namespace Trinity.Revolt
{
    internal class TrinityRevoltChannel : ITrinityChannel
    {
        public TrinityRevoltChannel(string x, RevoltClient client)
        {
            X = x;
            Client = client;
            Channel = Client.Channels.Get(x);
        }

        public string X { get; }
        public RevoltClient Client { get; }
        public Channel Channel { get; private set; }
        public string? Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TrinityGuid Id { get => new TrinityRevoltStringGuid(X); set => throw new NotImplementedException(); }
        public TrinityChannelType Type { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string? Topic { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IList<ITrinityUser> Users { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IList<ITrinityMessage> PinnedMessages { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task<ITrinityChannel> GetChannelAsync(TrinityGuid channelId)
        {
            throw new NotImplementedException();
        }

        public Task<IList<ITrinityMessage>> GetMessages(DateTime? before = null, DateTime? after = null, int? limit = null)
        {
            throw new NotImplementedException();
        }

        public Task<ITrinityMessage> ModifyAsync(ITrinityMessage trinityDiscordMessage, TrinityMessageBuilder trinityMessageBuilder)
        {
            throw new NotImplementedException();
        }

        public Task<ITrinityMessage> SendMessageAsync(TrinityMessageBuilder trinityMessageBuilder)
        {
            throw new NotImplementedException();
        }

        public Task<ITrinityMessage> SendMessageAsync(string content, TrinityEmbed embed)
        {
            throw new NotImplementedException();
        }

        public async Task<ITrinityMessage> SendMessageAsync(string content)
        {
            return new TrinityRevoltMessage(await Channel.SendMessageAsync(content));
        }

        public Task<ITrinityMessage> SendMessageAsync(TrinityEmbed embed)
        {
            throw new NotImplementedException();
        }

        public Task TriggerTypingAsync()
        {
            throw new NotImplementedException();
        }
    }
}