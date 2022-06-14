using DSharpPlus.Entities;
using Trinity.Shared;

namespace Trinity.DiscordSharpPlus
{
    internal class TrinityDiscordChannel : ITrinityChannel
    {
        private DiscordChannel x;

        public TrinityDiscordChannel(DiscordChannel x)
        {
            this.x = x;
        }

        public string? Name { get => x.Name; set => x.ModifyAsync(y => y.Name = value); }
        public TrinityGuid Id { get => new TrinityUlongGuid(x.Id); set => throw new NotSupportedException("Changing the ID of a discord channel is not supported"); }
        public TrinityChannelType Type { get => x.Type.ToTrinityChannelType(); set => x.ModifyAsync(y => y.Type = value.ToDiscordChannelType()); }
        public string? Topic { get => x.Topic; set => x.ModifyAsync(y => y.Topic = value); }
        public IList<ITrinityUser> Users { get => throw new NotImplementedException(); set => throw new NotSupportedException(); }
        public IList<ITrinityMessage> PinnedMessages { get => throw new NotImplementedException(); set => throw new NotSupportedException(); }

        public Task<ITrinityChannel> GetChannelAsync(TrinityGuid channelId)
        {
            throw new NotSupportedException("Discord does not support getting channels from a channel");
        }

        public async Task<IList<ITrinityMessage>> GetMessages(DateTime? before = null, DateTime? after = null, int? limit = null)
        {
            if (before == null && after == null && limit == null)
            {
                throw new NotSupportedException("At least one constraint is needed");
            }
            else if (before == null && after == null && limit != null)
            {
                return (await x.GetMessagesAsync((int)limit)).Select(x => (ITrinityMessage)new TrinityDiscordMessage(x)).ToList();
            }
            else if (before != null && after != null && limit == null)
            {
                //GetMessagesAfterAsync
                //GetMessagesBeforeAsync
                throw new NotImplementedException();
                //return (await x.GetMessagesAsync()).Select(x => (ITrinityMessage)new TrinityDiscordMessage(x)).ToList();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public async Task<ITrinityMessage> ModifyAsync(ITrinityMessage trinityDiscordMessage, TrinityMessageBuilder trinityMessageBuilder)
        {
            return new TrinityDiscordMessage(await ((TrinityDiscordMessage)trinityDiscordMessage).X.ModifyAsync(trinityMessageBuilder.ToDiscordMessageBuilder()));
        }

        public async Task<ITrinityMessage> SendMessageAsync(TrinityMessageBuilder trinityMessageBuilder) => new TrinityDiscordMessage(await x.SendMessageAsync(trinityMessageBuilder.ToDiscordMessageBuilder()));

        public async Task<ITrinityMessage> SendMessageAsync(string content) => new TrinityDiscordMessage(await x.SendMessageAsync(content));

        public async Task<ITrinityMessage> SendMessageAsync(TrinityEmbed embed) => new TrinityDiscordMessage(await x.SendMessageAsync(embed.ToDiscordEmbed()));

        public Task<ITrinityMessage> SendMessageAsync(string content, TrinityEmbed embed)
        {
            throw new NotImplementedException();
        }

        public Task TriggerTypingAsync()
        {
            return x.TriggerTypingAsync();
        }
    }
}