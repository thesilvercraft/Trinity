using Revolt;
using Trinity.Shared;

namespace Trinity.Revolt
{
    internal class TrinityRevoltUser : ITrinityUser
    {
        public TrinityRevoltUser(string id, RevoltClient client)
        {
            SId = id;
            Client = client;
            User = Client.Users.Get(id);
        }

        public string SId { get; }
        public RevoltClient Client { get; }
        public User User { get; }
        public TrinityGuid Id { get => new TrinityRevoltStringGuid(SId); set => throw new NotImplementedException(); }
        public string? Name { get => User.Username; set => throw new NotImplementedException(); }
        public bool IsAutomated { get => User.Bot != null; set => throw new NotImplementedException(); }
    }
}