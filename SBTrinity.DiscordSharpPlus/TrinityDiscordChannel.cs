using DSharpPlus.Entities;
using Trinity.Shared;

namespace Trinity.DiscordSharpPlus
{
    public class TrinityDiscordChannel : ITrinityChannelWithPinnedMessages, ITrinityChannelWithAdvancedSendingMethods
    {
        public DiscordChannel x { get; internal set; }

        public TrinityDiscordChannel(DiscordChannel x)
        {
            this.x = x;
        }

        public string? Name { get => x.Name; set => x.ModifyAsync(y => y.Name = value); }
        public TrinityGuid Id { get => new TrinityUlongGuid(x.Id); }
        public TrinityChannelType Type { get => x.Type.ToTrinityChannelType(); set => x.ModifyAsync(y => y.Type = value.ToDiscordChannelType()); }
        public string? Topic { get => x.Topic; set => x.ModifyAsync(y => y.Topic = value); }
        public IList<ITrinityUser> Users { get => x.Users.Select(y => (ITrinityUser)new TrinityDiscordMember(y)).ToList(); }
        public IList<ITrinityMessage> PinnedMessages { get => x.GetPinnedMessagesAsync().GetAwaiter().GetResult().Select(y => (ITrinityMessage)new TrinityDiscordMessage(y)).ToList(); }

        public ITrinityGuild? Guild => x.Guild == null ? null : new TrinityDiscordGuild(x.Guild);

        public bool IsNSFW { get => x.IsNSFW; set => throw new NotImplementedException(); }

        public bool IsPrivate => x.IsPrivate;

        public async Task<IList<ITrinityMessage>> GetMessages(TrinityGuid? before = null, TrinityGuid? after = null, int? limit = null)
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

        public async Task<ITrinityMessage> ModifyAsync(ITrinityMessage trinityDiscordMessage, string content)
        {
            return new TrinityDiscordMessage(await ((TrinityDiscordMessage)trinityDiscordMessage).X.ModifyAsync(content));
        }

        public async Task<ITrinityMessage> SendMessageAsync(TrinityMessageBuilder trinityMessageBuilder) => new TrinityDiscordMessage(await x.SendMessageAsync(trinityMessageBuilder.ToDiscordMessageBuilder()));

        public async Task<ITrinityMessage> SendMessageAsync(string content) => new TrinityDiscordMessage(await x.SendMessageAsync(new DiscordMessageBuilder().WithContent(content).WithAllowedMentions(Mentions.None)));

        public async Task<ITrinityMessage> SendMessageAsync(TrinityEmbed embed) => new TrinityDiscordMessage(await x.SendMessageAsync(embed.ToDiscordEmbed()));

        public async Task<ITrinityMessage> SendMessageAsync(string content, TrinityEmbed embed) => new TrinityDiscordMessage(await x.SendMessageAsync(new DiscordMessageBuilder().WithContent(content).WithAllowedMentions(Mentions.None).WithEmbed(embed.ToDiscordEmbed())));

        public Task TriggerTypingAsync() => x.TriggerTypingAsync();
    }
}