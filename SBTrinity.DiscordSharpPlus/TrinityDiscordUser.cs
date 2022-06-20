using DSharpPlus.Entities;
using Trinity.Shared;

namespace Trinity.DiscordSharpPlus
{
    public class TrinityDiscordUser : ITrinityUser
    {
        public TrinityDiscordUser(DiscordUser user)
        {
            User = user;
        }

        public TrinityGuid Id { get => new TrinityUlongGuid(User.Id); }
        public string? Name { get => User.Username; }
        public DiscordUser User { get; }
        public bool IsAutomated { get => User.IsBot; }

        public string Mention => User.Mention;
    }
}