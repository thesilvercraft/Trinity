using DSharpPlus.Entities;
using Trinity.Shared;

namespace Trinity.DiscordSharpPlus
{
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