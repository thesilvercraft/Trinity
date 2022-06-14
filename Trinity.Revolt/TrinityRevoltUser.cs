using RevoltSharp;
using Trinity.Shared;

namespace Trinity.Revolt
{
    internal class TrinityRevoltUser : ITrinityUser
    {
        public TrinityRevoltUser(string ownerId, RevoltClient client)
        {
            OwnerId = ownerId;
            Client = client;
        }

        public string OwnerId { get; }
        public RevoltClient Client { get; }
    }
}