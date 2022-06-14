namespace Trinity.Shared
{
    /// <summary>
    /// An abstraction layer that facilitates the basic operations of a channel (Discord) or room (Matrix).
    /// </summary>
    public interface ITrinityChannel
    {
        /// <summary>
        /// The name of the channel.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The ID of the channel.
        /// </summary>
        public TrinityGuid Id { get; set; }

        /// <summary>
        /// The type of the channel.
        /// </summary>
        public TrinityChannelType Type { get; set; }

        /// <summary>
        /// The topic of the channel.
        /// </summary>
        public string? Topic { get; set; }

        /// <summary>
        /// The list of users in the channel.
        /// </summary>
        public IList<ITrinityUser> Users { get; set; }

        /// <summary>
        /// The list of messages in the channel.
        /// </summary>
        public Task<IList<ITrinityMessage>> GetMessages(DateTime? before = null, DateTime? after = null, int? limit = null);

        /// <summary>
        /// The list of pinned messages in the channel.
        /// </summary>
        public IList<ITrinityMessage> PinnedMessages { get; set; }

        public Task<ITrinityChannel> GetChannelAsync(TrinityGuid channelId);

        Task<ITrinityMessage> ModifyAsync(ITrinityMessage trinityDiscordMessage, TrinityMessageBuilder trinityMessageBuilder);

        Task TriggerTypingAsync();

        Task<ITrinityMessage> SendMessageAsync(TrinityMessageBuilder trinityMessageBuilder);

        Task<ITrinityMessage> SendMessageAsync(string content, TrinityEmbed embed);

        Task<ITrinityMessage> SendMessageAsync(string content);

        Task<ITrinityMessage> SendMessageAsync(TrinityEmbed embed);
    }
}