using DSharpPlus.Entities;
using Trinity.Shared;

namespace Trinity.DiscordSharpPlus
{
    public class TrinityDiscordMember : ITrinityUser
    {
        public DiscordMember Member;

        public TrinityDiscordMember(DiscordMember member)
        {
            Member = member;
        }

        public TrinityGuid Id { get => new TrinityUlongGuid(Member.Id); set => throw new NotSupportedException("Changing the ID of a different user is not supported"); }
        public string? Name { get => Member.Username; set => throw new NotSupportedException("Changing the username of a different user is not supported"); }
        public bool IsAutomated { get => Member.IsBot; set => throw new NotSupportedException("You can not change the IsBot value of any user"); }
    }
}