namespace Trinity.Shared
{
    public interface ITrinityGuild
    {
        /// <summary>
        /// The ID of the guild.
        /// </summary>
        public TrinityGuid Id { get; }

        /// <summary>
        /// The name of the guild.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The description of the guild.
        /// </summary>
        public string? Description { get; set; }

        public IList<ITrinityChannel> Channels { get; }
        public ITrinityUser Owner { get; set; }
    }
}