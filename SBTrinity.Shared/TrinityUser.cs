namespace Trinity.Shared
{
    public interface ITrinityUser
    {
        public TrinityGuid Id { get; set; }
        public string? Name { get; set; }
        public bool IsAutomated { get; set; }
    }
}