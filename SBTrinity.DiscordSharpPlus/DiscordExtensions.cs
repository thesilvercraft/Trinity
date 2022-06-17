using Trinity;
using DSharpPlus.Entities;
using Trinity.Shared;
using DSharpPlus;

namespace Trinity.DiscordSharpPlus
{
    public static class DiscordExtensions
    {
        public static TrinityChannelType ToTrinityChannelType(this ChannelType type)
        {
            return type switch
            {
                ChannelType.Text => TrinityChannelType.Text,
                ChannelType.Private => TrinityChannelType.Private,
                ChannelType.Voice => TrinityChannelType.Voice,
                ChannelType.Group => TrinityChannelType.Group,
                ChannelType.Category => TrinityChannelType.Category,
                ChannelType.News => TrinityChannelType.News,
                ChannelType.Store => TrinityChannelType.Store,
                ChannelType.NewsThread => TrinityChannelType.NewsThread,
                ChannelType.PublicThread => TrinityChannelType.PublicThread,
                ChannelType.PrivateThread => TrinityChannelType.PrivateThread,
                ChannelType.Stage => TrinityChannelType.Stage,
                ChannelType.Unknown => TrinityChannelType.Unknown,
                _ => TrinityChannelType.Unknown,
            };
        }

        public static ChannelType ToDiscordChannelType(this TrinityChannelType type)
        {
            return type switch
            {
                TrinityChannelType.Text => ChannelType.Text,
                TrinityChannelType.Private => ChannelType.Private,
                TrinityChannelType.Voice => ChannelType.Voice,
                TrinityChannelType.Group => ChannelType.Group,
                TrinityChannelType.Category => ChannelType.Category,
                TrinityChannelType.News => ChannelType.News,
                TrinityChannelType.Store => ChannelType.Store,
                TrinityChannelType.NewsThread => ChannelType.NewsThread,
                TrinityChannelType.PublicThread => ChannelType.PublicThread,
                TrinityChannelType.PrivateThread => ChannelType.PrivateThread,
                TrinityChannelType.Stage => ChannelType.Stage,
                TrinityChannelType.Unknown => ChannelType.Unknown,
                _ => ChannelType.Unknown,
            };
        }

        public static Mention? ToTrinityMention(this IMention mention)
        {
            if (mention is EveryoneMention)
            {
                return new Mention { Type = Shared.MentionType.Everyone };
            }
            else if (mention is RoleMention m && m.Id != null)
            {
                return new Mention { Type = Shared.MentionType.Role, Target = new TrinityUlongGuid(m.Id.Value) };
            }
            else if (mention is UserMention um && um.Id != null)
            {
                return new Mention { Type = Shared.MentionType.User, Target = new TrinityUlongGuid(um.Id.Value) };
            }
            return null;
        }

        public static IMention? ToDiscordMention(this Mention mention)
        {
            return mention.Type switch
            {
                Shared.MentionType.Everyone => new EveryoneMention(),
                Shared.MentionType.Role => new RoleMention(((TrinityUlongGuid)mention.Target).Value),
                Shared.MentionType.User => new UserMention(((TrinityUlongGuid)mention.Target).Value),
                _ => null,
            };
        }

        public static DiscordColor ToDiscordColor(this TrinityColor c)
        {
            return new DiscordColor(c.R, c.G, c.B);
        }

        public static DiscordEmbed ToDiscordEmbed(this TrinityEmbed embed)
        {
            var discordEmbed = new DiscordEmbedBuilder();
            if (embed.Title != null)
                discordEmbed.WithTitle(embed.Title);
            if (embed.Description != null)
                discordEmbed.WithDescription(embed.Description);
            if (embed.Color != null)
                discordEmbed.WithColor(embed.Color.ToDiscordColor());
            if (embed.Timestamp != null)
                discordEmbed.WithTimestamp(embed.Timestamp.Value);
            if (embed.Footer != null)
                discordEmbed.WithFooter(embed.Footer.Text, embed.Footer.IconUrl.ToString());
            if (embed.Thumbnail != null)
                discordEmbed.WithThumbnail(embed.Thumbnail.Url);
            if (embed.Image != null)
                discordEmbed.WithImageUrl(embed.Image.Url);
            if (embed.Author != null)
                discordEmbed.WithAuthor(embed.Author.Name, embed.Author.Url.ToString(), embed.Author.IconUrl.ToString());
            if (embed.Fields != null)
            {
                foreach (var field in embed.Fields)
                {
                    discordEmbed.AddField(field.Name, field.Value, field.Inline);
                }
            }
            return discordEmbed.Build();
        }

        public static DiscordMessageBuilder ToDiscordMessageBuilder(this TrinityMessageBuilder messageBuilder)
        {
            var builder = new DiscordMessageBuilder();
            if (messageBuilder.Content != null)
            {
                builder.Content = messageBuilder.Content;
            }
            if (messageBuilder.Embed != null)
            {
                builder.Embed = messageBuilder.Embed.ToDiscordEmbed();
            }
            if (messageBuilder.Embeds.Any())
            {
                foreach (var embed in messageBuilder.Embeds)
                {
                    builder.AddEmbed(embed.ToDiscordEmbed());
                }
            }
            if (messageBuilder.Mentions != null)
            {
                List<IMention> mentions = new();
                foreach (var mention in messageBuilder.Mentions)
                {
                    mentions.Add(mention.ToDiscordMention() ?? throw new InvalidOperationException());
                }
                builder.WithAllowedMentions(mentions);
            }
            if (messageBuilder.ReplyId != null)
            {
                builder.WithReply(((TrinityUlongGuid)messageBuilder.ReplyId).Value);
            }
            builder.IsTTS = messageBuilder.IsTTS;

            return builder;
        }
    }
}