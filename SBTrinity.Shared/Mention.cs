namespace Trinity.Shared
{
    public enum MentionType
    {
        Noone,
        Everyone,
        Active,
        Role,
        User
    }

    public class Mention
    {
        public MentionType Type { get; set; }
        public TrinityGuid Target { get; set; }
    }
}