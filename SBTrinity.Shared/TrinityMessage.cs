namespace Trinity.Shared;

public interface ITrinityMessage
{
    public TrinityGuid Id { get; }
    public ITrinityUser Author { get; }
    public string? PlainTextMessage { get; set; }
    public DateTime? Timestamp { get; }
    public ITrinityChannel Channel { get; }
    public List<TrinityEmbed>? Embeds { get; set; }
    public List<Mention> Mentions { get; }
    public ITrinityMessage? ReferencedMessage { get; }

    Task<ITrinityMessage> ModifyAsync(string content);

    /// <summary>
    /// MAY BE NULL IF CHANNEL DOES NOT SUPPORT IT
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    Task<ITrinityMessage>? ModifyAsync(TrinityMessageBuilder trinityMessageBuilder);

    Task<ITrinityMessage> RespondAsync(string content);

    /// <summary>
    /// MAY BE NULL IF CHANNEL DOES NOT SUPPORT IT
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    Task<ITrinityMessage>? RespondAsync(TrinityMessageBuilder builder);

    /// <summary>
    /// MAY BE NULL IF CHANNEL DOES NOT SUPPORT IT
    /// </summary>
    /// <param name="embed"></param>
    /// <returns></returns>
    Task<ITrinityMessage>? RespondAsync(TrinityEmbed embed);

    /// <summary>
    /// MAY BE NULL IF CHANNEL DOES NOT SUPPORT IT
    /// </summary>
    /// <param name="embed"></param>
    /// <returns></returns>
    Task<ITrinityMessage>? RespondAsync(string content, TrinityEmbed embed);
}