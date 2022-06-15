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
        public string? Name { get => throw new NotSupportedException("NOP"); set => throw new NotSupportedException("NOP"); }
        public TrinityGuid Id { get => new TrinityRevoltStringGuid(X); set => throw new NotSupportedException("NOP"); }
        public TrinityChannelType Type { get => Channel.ToTrinityChannelType(); set => throw new NotSupportedException("CANT"); }
        public string? Topic { get => throw new NotSupportedException("NOP"); set => throw new NotSupportedException("NOP"); }
        public IList<ITrinityUser> Users { get => Channel.GetMembersAsync().GetAwaiter().GetResult().Select(y => (ITrinityUser)new TrinityRevoltUser(y)).ToList(); set => throw new NotSupportedException("NOP"); }
        public IList<ITrinityMessage> PinnedMessages { get => throw new NotSupportedException("NOP"); set => throw new NotSupportedException("NOP"); }

        public Task<ITrinityChannel> GetChannelAsync(TrinityGuid channelId)
        {
            throw new NotSupportedException("NOP");
        }

        public Task<IList<ITrinityMessage>> GetMessages(DateTime? before = null, DateTime? after = null, int? limit = null)
        {
            throw new NotSupportedException("NOP");
        }

        public Task<ITrinityMessage> ModifyAsync(ITrinityMessage trinityDiscordMessage, TrinityMessageBuilder trinityMessageBuilder)
        {
            throw new NotSupportedException("NOP");
        }

        public Task<ITrinityMessage> SendMessageAsync(TrinityMessageBuilder trinityMessageBuilder)
        {
            throw new NotSupportedException("NOP");
        }

        public Task<ITrinityMessage> SendMessageAsync(string content, TrinityEmbed embed)
        {
            throw new NotSupportedException("NOP");
        }

        public async Task<ITrinityMessage> SendMessageAsync(string content)
        {
            return new TrinityRevoltMessage(await Channel.SendMessageAsync(content));
        }

        public Task<ITrinityMessage> SendMessageAsync(TrinityEmbed embed)
        {
            throw new NotSupportedException("NOP");
        }

        public Task TriggerTypingAsync()
        {
            return Channel.BeginTypingAsync();
        }
    }

    public static class RevoltHelpers
    {
        public static TrinityChannelType ToTrinityChannelType(this Channel channel)
        {
            return channel.ChannelType switch

            {
                "TextChannel" => TrinityChannelType.Text,
                "VoiceChannel" => TrinityChannelType.Voice,
                _ => TrinityChannelType.Unknown
            };
        }
    }
}