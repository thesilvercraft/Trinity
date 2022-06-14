namespace Trinity.Shared;

public interface ITrinityMessage
{
    public TrinityGuid Id { get; set; }
    public ITrinityUser Author { get; }
    public string? PlainTextMessage { get; set; }
    public DateTime? Timestamp { get; set; }
    public ITrinityChannel Channel { get; set; }
    public List<TrinityEmbed>? Embeds { get; set; }
    public List<Mention> Mentions { get; set; }

    Task<ITrinityMessage> RespondAsync(string content, TrinityEmbed embed);

    Task<ITrinityMessage> RespondAsync(TrinityEmbed embed);

    Task<ITrinityMessage> RespondAsync(string content);

    Task<ITrinityMessage> ModifyAsync(TrinityMessageBuilder trinityMessageBuilder);
    Task<ITrinityMessage> RespondAsync(TrinityMessageBuilder builder);
}