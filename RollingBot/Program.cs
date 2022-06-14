using Trinity.DiscordSharpPlus;
using Trinity.Shared;
using Trinity;

Console.WriteLine("ROLLING START!!!");
TrinityDiscordClient client = new(new DSharpPlus.DiscordClient(new DSharpPlus.DiscordConfiguration() { Token = "" }));
await client.ConnectAsync();
client.MessageRecieved += Client_MessageRecieved;
async Task Client_MessageRecieved(IPlatformProvider sender, MessageRecievedArgs e)
{
    if (!e.Message.Author.IsAutomated && e.Message.PlainTextMessage.StartsWith("hey"))
    {
        await e.Channel.SendMessageAsync("Hello from trinity!");
    }
}

await Task.Delay(-1);