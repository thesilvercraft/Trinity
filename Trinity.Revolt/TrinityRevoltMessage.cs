using RevoltSharp;
using Trinity.Shared;

namespace Trinity.Revolt
{
    internal class TrinityRevoltMessage : ITrinityMessage
    {
        public Message Message { get; set; }
        public TrinityGuid Id { get => new TrinityRevoltStringGuid(Message.Id); set => throw new NotImplementedException(); }

        public ITrinityUser Author => new TrinityRevoltUser(Message.AuthorId, Message.Client);

        public string? PlainTextMessage { get => Message.; set => throw new NotImplementedException(); }
        public DateTime? Timestamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ITrinityChannel Channel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<TrinityEmbed>? Embeds { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public List<Mention> Mentions { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public TrinityRevoltMessage(Message message)
        {
            Message = message;
        }

        public Task<ITrinityMessage> RespondAsync(string content, TrinityEmbed embed)
        {
            throw new NotImplementedException();
        }

        public Task<ITrinityMessage> RespondAsync(TrinityEmbed embed)
        {
            throw new NotImplementedException();
        }

        public Task<ITrinityMessage> RespondAsync(string content)
        {
            throw new NotImplementedException();
        }

        public Task<ITrinityMessage> ModifyAsync(TrinityMessageBuilder trinityMessageBuilder)
        {
            throw new NotImplementedException();
        }

        public Task<ITrinityMessage> RespondAsync(TrinityMessageBuilder builder)
        {
            throw new NotImplementedException();
        }
    }
}