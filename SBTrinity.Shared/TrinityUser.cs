namespace Trinity.Shared
{
    public interface ITrinityUser
    {
        public TrinityGuid Id { get; }
        public string? Name { get; }
        public bool IsAutomated { get; }
        string Mention { get; }
    }
}