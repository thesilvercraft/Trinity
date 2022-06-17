using DSharpPlus.Entities;
using Trinity.Shared;

namespace Trinity.DiscordSharpPlus
{
    public class TrinityDiscordGuild : ITrinityGuild
    {
        public TrinityDiscordGuild(DiscordGuild guild)
        {
            Guild = guild ?? throw new ArgumentNullException(nameof(guild));
        }

        public TrinityGuid Id { get => new TrinityUlongGuid(Guild.Id); }
        public string? Name { get => Guild.Name; set => Guild.ModifyAsync(y => y.Name = value); }
        public string? Description { get => Guild.Description; set => Guild.ModifyAsync(y => y.Description = value); }

        public IList<ITrinityChannel> Channels => GetChannels();

        private IList<ITrinityChannel> GetChannels()
        {
            return Guild.Channels.Values.Select(x => (ITrinityChannel)new TrinityDiscordChannel(x)).ToList();
        }

        public ITrinityUser Owner { get => new TrinityDiscordMember(Guild.Owner); set => Guild.ModifyAsync(y => y.Owner = ((TrinityDiscordMember)value).Member); }
        private DiscordGuild Guild { get; set; }
    }
}