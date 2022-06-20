namespace Trinity.Commands.Attributes;

public class RequireAttachmentAttribute : CheckBaseAttribute
{
    public RequireAttachmentAttribute(uint attachmentcount = 1, int argumentCountThatOverloadsCheck = -1, bool allowreply = true)
    {
        AttachmentCount = attachmentcount;
        OverloadCount = argumentCountThatOverloadsCheck;
        AllowReply = allowreply;
    }

    public uint AttachmentCount { get; init; }

    public int OverloadCount { get; set; }
    public bool AllowReply { get; set; }

    public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
    {
        return Task.FromResult(help || OverloadCount == (ctx.Overload == null ? ctx.RawArguments.Count : ctx.Overload.Arguments.Count) || ctx.Message.Attachments.Count == AttachmentCount || (AllowReply && ctx.Message.ReferencedMessage != null && ctx.Message.ReferencedMessage.Attachments.Count == AttachmentCount));
    }
}