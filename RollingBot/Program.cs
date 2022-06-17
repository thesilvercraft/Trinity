using Trinity.DiscordSharpPlus;
using Trinity.Shared;
using Trinity;
using Trinity.Revolt;
using Trinity.Commands;
using RollingBot;

Console.WriteLine("ROLLING START!!!");
TrinityDiscordClient client = new(new DSharpPlus.DiscordClient(new DSharpPlus.DiscordConfiguration() { Token = Environment.GetEnvironmentVariable("trinityd") }));
await client.ConnectAsync();
TrinityRevoltClient rc = new(new(), Revolt.TokenType.Bot, Environment.GetEnvironmentVariable("trinityr"));
await rc.ConnectAsync();
client.MessageRecieved += Client_MessageRecieved;
rc.MessageRecieved += Client_MessageRecieved;
CommandsNextExtension cn = new(new CommandsNextConfiguration() { StringPrefixes = new List<string> { "~" } });
cn.SetupEventHandlers(
    (x, y, z, j, h) => { Console.WriteLine("1:"); Console.WriteLine(y.Message); },
    (x, y, z, j, h) => { Console.WriteLine("2:"); Console.WriteLine(y.Message); });
cn.Setup(client);
cn.Setup(rc);
cn.RegisterCommands<TestCommandModule>();
async Task Client_MessageRecieved(IPlatformProvider sender, MessageCreatedEventArgs e)
{
    if (!e.Message.Author.IsAutomated && e.Message.PlainTextMessage.StartsWith("hey"))
    {
        await e.Channel.SendMessageAsync("Hello from trinity!");
    }
}

await Task.Delay(-1);