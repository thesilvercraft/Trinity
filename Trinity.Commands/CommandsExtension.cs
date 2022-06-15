using System.Diagnostics;
using Trinity.Shared;

namespace Trinity.Commands;

public class CommandsExtension : TrinityBaseExtension
{
    public bool ExecuteAutomatedCommands { get; set; } = false;
    public string[] DefaultPrefixes { get; set; } = new[] { "!" };

    public Func<IPlatformProvider, MessageRecievedArgs, CommandsExtension, string[]> DefaultGetPrefix { get; } =
        (a, b, c) =>
        {
            return c.DefaultPrefixes;
        };

    public Func<IPlatformProvider, MessageRecievedArgs, CommandsExtension, string[]> GetPrefix { get; set; }

    public CommandsExtension(params IPlatformProvider[] platforms)
    {
        Providers.AddRange(platforms);
        foreach (var platform in platforms)
        {
            platform.MessageRecieved += Platform_MessageRecieved;
        }
        GetPrefix = DefaultGetPrefix;
    }

    private async Task Platform_MessageRecieved(IPlatformProvider sender, MessageRecievedArgs e)
    {
        if (e.Author.IsAutomated && !ExecuteAutomatedCommands)
        {
            Debug.WriteLine($"Message {e.Message.Id} by {e.Author.Id} in {e.Channel.Id} was ignored because of CommandsExtension.ExecuteAutomatedCommands's value (FALSE)");
            return;
        }
        if (string.IsNullOrEmpty(e.Message.PlainTextMessage))
        {
            Debug.WriteLine($"Message {e.Message.Id} by {e.Author.Id} in {e.Channel.Id} was ignored because of e.Message.PlainTextMessage's value (NULL OR EMPTY)");

            return;
        }
        var prefixes = GetPrefix(sender, e, this);
        if (prefixes.Any(p => e.Message.PlainTextMessage.StartsWith(p)))
        {
            var command = e.Message.PlainTextMessage.Substring(prefixes.First().Length);
            var commandContext = new CommandContext()
            {
                Client = sender,
                Message = e.Message,
                User = e.Author,
                Channel = e.Channel,
            };
            Debug.WriteLine("Command " + command);
            //await BeforeCommandExecution(commandContext);
            // await AfterCommandExecution(commandContext);
        }
    }
}