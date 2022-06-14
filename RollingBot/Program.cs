using Trinity.DiscordSharpPlus;
using Trinity.Shared;
using Trinity;
using Trinity.Revolt;

Console.WriteLine("ROLLING START!!!");
TrinityDiscordClient client = new(new DSharpPlus.DiscordClient(new DSharpPlus.DiscordConfiguration() { Token = "" }));
await client.ConnectAsync();
TrinityRevoltClient rc = new(new(), Revolt.TokenType.Bot, "");
await rc.ConnectAsync();

client.MessageRecieved += Client_MessageRecieved;
rc.MessageRecieved += Client_MessageRecieved;

async Task Client_MessageRecieved(IPlatformProvider sender, MessageRecievedArgs e)
{
    if (!e.Message.Author.IsAutomated && e.Message.PlainTextMessage.StartsWith("hey"))
    {
        await e.Channel.SendMessageAsync("Hello from trinity!");
    }
}

await Task.Delay(-1);