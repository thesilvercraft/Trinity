using DSharpPlus.Entities;
using Trinity.Shared;

namespace Trinity.DiscordSharpPlus
{
    internal class TrinityDiscordMessage : ITrinityMessage
    {
        public TrinityDiscordMessage(DiscordMessage x)
        {
            X = x;
        }

        public DiscordMessage X { get; }
        public TrinityGuid Id { get => new TrinityUlongGuid(X.Id); set => throw new NotSupportedException("You can not change the id of a message"); }

        public ITrinityUser Author => new TrinityDiscordUser(X.Author);

        public string? PlainTextMessage { get => X.Content; set => X.ModifyAsync(y => y.Content = value); }
        public DateTime? Timestamp { get => X.Timestamp.UtcDateTime; set => throw new NotSupportedException("Editing of a timestamp is not supported"); }
        public ITrinityChannel Channel { get => new TrinityDiscordChannel(X.Channel); set => throw new NotSupportedException("You cant transfer a message from a channel to a different one"); }
        public List<TrinityEmbed>? Embeds { get => TrinityDiscordEmbed.CreateFromDiscordEmbedListAsTrinityEmbed(X.Embeds); set => throw new NotSupportedException("You can not change the embeds of an already sent message for now (TODO)"); }
        public List<Mention> Mentions { get => GetMentions(); set => throw new NotSupportedException("You can not explicitly change the mentions of a message for now"); }

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
            return Channel.ModifyAsync(this, trinityMessageBuilder);
        }

        public Task<ITrinityMessage> RespondAsync(string content, TrinityEmbed embed)
        {
            return Channel.SendMessageAsync(content, embed);
        }

        public Task<ITrinityMessage> RespondAsync(TrinityEmbed embed)
        {
            return Channel.SendMessageAsync(embed);
        }

        public Task<ITrinityMessage> RespondAsync(string content)
        {
            return Channel.SendMessageAsync(content);
        }

        public Task<ITrinityMessage> RespondAsync(TrinityMessageBuilder builder)
        {
            return Channel.SendMessageAsync(builder);
        }
    }
}