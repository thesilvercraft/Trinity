using Revolt;
using Trinity.Shared;

namespace Trinity.Revolt
{
    internal class TrinityRevoltAttachment : ITrinityAttachment
    {
        public TrinityRevoltAttachment(Attachment x)
        {
            X = x;
        }

        public Attachment X { get; init; }

        public string? Name => X.Filename;

        public string? Description => X.Tag;

        public string? Url => X.Url;

        public string? ProxyUrl => X.Url;

        public Stream GetStream()
        {
            throw new NotImplementedException();
        }
    }
}