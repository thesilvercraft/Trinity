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
        public TrinityGuid Id { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ITrinityUser Author => new TrinityDiscordUser(X.Author);

        public string? PlainTextMessage { get => X.Content; set => X.ModifyAsync(y => y.Content = value); }
        public DateTime? Timestamp { get => X.Timestamp.UtcDateTime; set => throw new NotSupportedException("Editing of a timestamp is not supported"); }
        public ITrinityChannel Channel { get => new TrinityDiscordChannel(X.Channel); set => throw new NotImplementedException(); }
        public List<TrinityEmbed>? Embeds { get => TrinityDiscordEmbed.CreateFromDiscordEmbedListAsTrinityEmbed(X.Embeds); set => throw new NotImplementedException(); }
        public List<Mention> Mentions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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

    internal class TrinityDiscordUser : ITrinityUser
    {
        public TrinityDiscordUser(DiscordUser user)
        {
            User = user;
        }

        public TrinityGuid Id { get => new TrinityUlongGuid(User.Id); set => throw new NotSupportedException("Changing the ID of a user is not supported"); }
        public string? Name { get => User.Username; set => throw new NotSupportedException("Changing the username of a different user is not supported"); }
        public DiscordUser User { get; }
        public bool IsAutomated { get => User.IsBot; set => throw new NotSupportedException("You can not change the IsBot value of any user"); }
    }
}