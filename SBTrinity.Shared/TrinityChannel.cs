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
        public TrinityGuid Id { get; }

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
        public IList<ITrinityUser> Users { get; }

        public ITrinityGuild? Guild { get; }
        public bool IsNSFW { get; set; }
        bool IsPrivate { get; }

        /// <summary>
        /// The list of messages in the channel.
        /// </summary>
        public Task<IList<ITrinityMessage>> GetMessages(TrinityGuid? before = null, TrinityGuid? after = null, int? limit = null);

        Task<ITrinityMessage> ModifyAsync(ITrinityMessage trinityDiscordMessage, string content);

        Task TriggerTypingAsync();

        Task<ITrinityMessage> SendMessageAsync(string content);
    }

    public interface ITrinityChannelWithAdvancedSendingMethods : ITrinityChannel
    {
        Task<ITrinityMessage> SendMessageAsync(TrinityEmbed embed);

        Task<ITrinityMessage> SendMessageAsync(TrinityMessageBuilder trinityMessageBuilder);

        Task<ITrinityMessage> SendMessageAsync(string content, TrinityEmbed embed);

        Task<ITrinityMessage> ModifyAsync(ITrinityMessage trinityDiscordMessage, TrinityMessageBuilder trinityMessageBuilder);
    }

    public interface ITrinityChannelWithPinnedMessages : ITrinityChannel
    {
        /// <summary>
        /// The list of pinned messages in the channel.
        /// MAY BE NULL.
        /// </summary>
        public IList<ITrinityMessage> PinnedMessages { get; }
    }

    public interface ITrinityChannelWithSubChannels : ITrinityChannel
    {
        public Task<ITrinityChannel> GetChannelAsync(TrinityGuid channelId);
    }
}