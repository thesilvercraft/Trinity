using DSharpPlus.Entities;
using Trinity.Shared;

namespace Trinity.DiscordSharpPlus
{
    public class TrinityDiscordMessage : ITrinityMessage
    {
        public TrinityDiscordMessage(DiscordMessage x)
        {
            X = x;
        }

        public DiscordMessage X { get; }
        public TrinityGuid Id { get => new TrinityUlongGuid(X.Id); }

        public ITrinityUser Author => new TrinityDiscordUser(X.Author);

        public string? PlainTextMessage { get => X.Content; set => X.ModifyAsync(y => y.Content = value); }
        public DateTime? Timestamp { get => X.Timestamp.UtcDateTime; }
        public ITrinityChannel Channel { get => new TrinityDiscordChannel(X.Channel); }
        public List<TrinityEmbed>? Embeds { get => TrinityDiscordEmbed.CreateFromDiscordEmbedListAsTrinityEmbed(X.Embeds); set => throw new NotSupportedException("You can not change the embeds of an already sent message for now (TODO)"); }
        public List<Mention> Mentions { get => GetMentions(); }

        public ITrinityMessage? ReferencedMessage => X.ReferencedMessage == null ? null : new TrinityDiscordMessage(X.ReferencedMessage);

        private List<Mention> GetMentions()
        {
            List<Mention> m = new();
            foreach (var user in X.MentionedUsers)
            {
                m.Add(new() { Type = MentionType.User, Target = new TrinityUlongGuid(user.Id) });
            }
            foreach (var role in X.MentionedRoles)
            {
                m.Add(new() { Type = MentionType.Role, Target = new TrinityUlongGuid(role.Id) });
            }
            foreach (var channel in X.MentionedChannels)
            {
                m.Add(new() { Type = MentionType.Everyone, Target = new TrinityUlongGuid(channel.Id) });
            }
            return m;
        }

        public Task<ITrinityMessage> ModifyAsync(TrinityMessageBuilder trinityMessageBuilder)
        {
            if (Channel is ITrinityChannelWithAdvancedSendingMethods c)
            {
                return c.ModifyAsync(this, trinityMessageBuilder);
            }
            return null;
        }

        public Task<ITrinityMessage> RespondAsync(string content)
        {
            return Channel.SendMessageAsync(content);
        }

        public Task<ITrinityMessage>? RespondAsync(TrinityMessageBuilder builder)
        {
            if (Channel is ITrinityChannelWithAdvancedSendingMethods c)
            {
                return c.SendMessageAsync(builder);
            }
            return null;
        }

        public Task<ITrinityMessage> ModifyAsync(string content)
        {
            return Channel.ModifyAsync(this, content);
        }

        public Task<ITrinityMessage>? RespondAsync(TrinityEmbed embed)
        {
            if (Channel is ITrinityChannelWithAdvancedSendingMethods c)
            {
                return c.SendMessageAsync(embed);
            }
            return null;
        }

        public Task<ITrinityMessage>? RespondAsync(string content, TrinityEmbed embed)
        {
            if (Channel is ITrinityChannelWithAdvancedSendingMethods c)
            {
                return c.SendMessageAsync(content, embed);
            }
            return null;
        }
    }
}