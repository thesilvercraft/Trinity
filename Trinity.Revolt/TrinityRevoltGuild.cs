using Revolt;
using Trinity.Shared;

namespace Trinity.Revolt
{
    internal class TrinityRevoltGuild : ITrinityGuild
    {
        public TrinityRevoltGuild(Server guild, RevoltClient client)
        {
            Guild = guild;
            Client = client;
        }

        public RevoltClient Client;
        public Server Guild { get; }
        public TrinityGuid Id { get => new TrinityRevoltStringGuid(Guild._id); }
        public string? Name { get => Guild.Name; set => throw new NotSupportedException(); }
        public string? Description { get => Guild.Description; set => throw new NotSupportedException(); }

        public IList<ITrinityChannel> Channels => Guild.ChannelIds.Select(x => (ITrinityChannel)new TrinityRevoltChannel(x, Client)).ToList();

        public ITrinityUser Owner { get => new TrinityRevoltUser(Guild.OwnerId, Client); set => throw new NotImplementedException(); }
    }
}