using Revolt;
using Revolt.Channels;
using Trinity.Shared;

namespace Trinity.Revolt
{
    internal class TrinityRevoltMessage : ITrinityMessage
    {
        public Message Message { get; set; }
        public TrinityGuid Id { get => new TrinityRevoltStringGuid(Message._id); set => throw new NotImplementedException(); }

        public ITrinityUser Author => new TrinityRevoltUser(Message.AuthorId, Message.Client);

        public string? PlainTextMessage { get => Message.Content; set => throw new NotImplementedException(); }
        public DateTime? Timestamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ITrinityChannel Channel { get => new TrinityRevoltChannel(Message.ChannelId, Message.Client); set => throw new NotImplementedException(); }
        public List<TrinityEmbed>? Embeds { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<Mention> Mentions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ITrinityMessage? ReferencedMessage => throw new NotImplementedException();

        public List<ITrinityAttachment> Attachments => Message.Attachments.Select(x => (ITrinityAttachment)new TrinityRevoltAttachment(x)).ToList();

        private Channel channel;

        public TrinityRevoltMessage(Message message, Channel c)
        {
            Message = message;
            channel = c;
        }

        public Task<ITrinityMessage> RespondAsync(string content, TrinityEmbed embed)
        {
            throw new NotImplementedException();
        }

        public Task<ITrinityMessage> RespondAsync(TrinityEmbed embed)
        {
            throw new NotImplementedException();
        }

        public async Task<ITrinityMessage> RespondAsync(string content)
        {
            return new TrinityRevoltMessage(await channel.SendMessageAsync(content), channel);
        }

        public Task<ITrinityMessage> ModifyAsync(TrinityMessageBuilder trinityMessageBuilder)
        {
            throw new NotImplementedException();
        }

        public Task<ITrinityMessage> RespondAsync(TrinityMessageBuilder builder)
        {
            return builder.SendAsync(new TrinityRevoltChannel(channel));
        }

        public Task<ITrinityMessage> ModifyAsync(string content)
        {
            throw new NotImplementedException();
        }
    }
}