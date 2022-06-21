using RollingBot.Shared;
using System.Text.Json;
using System.Text.Json.Serialization;
using Trinity.Commands;
using Trinity.Commands.Attributes;
using Trinity.Shared;

namespace SilverBotDS.Anime;

[Category("Anime")]
public class AnimeModule : BaseCommandModule
{
    private const string BaseUrl = "https://anime-api.hisoka17.repl.co/";
    public HttpClient Client { private get; set; }

    private async Task<string> GetAnimeUrl(string endpoint)
    {
        return JsonSerializer
            .Deserialize<RootObject>(await (await Client.GetAsync(BaseUrl + endpoint)).Content.ReadAsStringAsync())!.Url;
    }

    private async Task SendImage(CommandContext ctx, string url)
    {
        await new TrinityMessageBuilder().WithContent(url)
            .SendAsync((ITrinityChannelWithAdvancedSendingMethods)ctx.Channel);
    }

    [Command("hug")]
    public async Task Hug(CommandContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/hug"));
    }

    [Command("kiss")]
    public async Task Kiss(CommandContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/kiss"));
    }

    [Command("slap")]
    public async Task Slap(CommandContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/slap"));
    }

    [Command("wink")]
    public async Task Wink(CommandContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/wink"));
    }

    [Command("pat")]
    public async Task Pat(CommandContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/pat"));
    }

    [Command("kill")]
    public async Task Kill(CommandContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/kill"));
    }

    [Command("cuddle")]
    public async Task Cuddle(CommandContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/cuddle"));
    }

    [Command("punch")]
    public async Task Punch(CommandContext ctx)
    {
        await SendImage(ctx, await GetAnimeUrl("img/punch"));
    }

    public class RootObject
    {
        [JsonPropertyName("url")] public string Url { get; set; }
    }
}