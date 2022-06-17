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

        public TrinityGuid Id { get => new TrinityUlongGuid(Member.Id); }
        public string? Name { get => Member.Username; }
        public bool IsAutomated { get => Member.IsBot; }
    }
}