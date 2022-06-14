using RevoltSharp;
using Trinity.Shared;

namespace Trinity.Revolt
{
    internal class TrinityRevoltChannel : ITrinityChannel
    {
        private string x;
        private RevoltClient client;
        public Channel Channel { get; set; }
        private TextChannel? text { get; set; }
        private VoiceChannel? voice { get; set; }
        private Server Server { get; set; }

        public TrinityRevoltChannel(string x, RevoltClient client, Server server)
        {
            this.x = x;
            this.client = client;
            Channel = client.GetChannel(x);
            if (Channel.Type == ChannelType.Text)
            {
                text = server.GetTextChannel(x);
            }
            else if (Channel.Type == ChannelType.Voice)
            {
                voice = server.GetVoiceChannel(x);
            }

            Server = server;
        }

        public string? Name { get => text?.Name ?? voice?.Name; set => throw new NotImplementedException(); }
        public TrinityGuid Id { get => new TrinityRevoltStringGuid(x); set => throw new NotImplementedException(); }
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
            return new TrinityRevoltMessage(await text?.SendMessageAsync(content) ?? await voice?.SendMessageAsync(content));
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