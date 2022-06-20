using DSharpPlus.Entities;
using Trinity.Shared;

namespace Trinity.DiscordSharpPlus
{
    internal class TrinityDiscordAttachment : ITrinityAttachment
    {
        private DiscordAttachment x;

        public TrinityDiscordAttachment(DiscordAttachment x)
        {
            this.x = x;
        }

        public string? Name => x.FileName;

        public string? Description => null;

        public string? Url => x.Url;

        public string? ProxyUrl => x.ProxyUrl;

        public Stream GetStream()
        {
            return null;
        }
    }
}