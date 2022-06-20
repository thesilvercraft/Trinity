using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Commands;
using Trinity.Commands.Attributes;
using Trinity.DiscordSharpPlus;
using Trinity.Shared;

namespace RollingBot.Commands
{
    internal class TestCommandModule : BaseCommandModule
    {
        private const string UnsupportedPlatform = "Unsupported platform, try this command out on a different platform";

        [Command("ay")]
        public async Task Gaming(CommandContext ctx, ITrinityUser arg)
        {
            await ctx.RespondAsync($"{arg.Id.ToGuid()}");
        }

        [Command("dump")]
        [Description("Dump a messages raw content (useful for emote walls)")]
        public async Task DumpMessage(CommandContext ctx, ITrinityMessage message)
        {
            if (ctx.Channel is ITrinityChannelWithAdvancedSendingMethods c)
            {
                await using var outStream = new MemoryStream(Encoding.UTF8.GetBytes(message.PlainTextMessage))
                {
                    Position = 0
                };
                await new TrinityMessageBuilder()
                    // .WithReply(ctx.Message.Id)
                    .WithContent($"{ctx.User.Mention}")
                    .WithFile("message.txt", outStream)
                    .SendAsync(c);
            }
            else
            {
                await ctx.RespondAsync(UnsupportedPlatform);
            }
        }

        [Command("testping")]
        public async Task TestPing(CommandContext ctx)
        {
            await ctx.RespondAsync($"@everyone @here <@!264081339316305920> <&749743290705903719> ");
        }

        [Command("testping")]
        public async Task TestPing(CommandContext ctx, string userinput)
        {
            await ctx.RespondAsync(userinput);
        }

        [Command("s")]
        public async Task Stress(CommandContext ctx, string arg1, bool arg2, sbyte arg3, byte arg4, short arg5, ushort arg6, int arg7, uint arg8, long arg9, ulong arg10)
        {
            await ctx.RespondAsync("GREAT SUCCESS");
        }

        [Command("s2")]
        public async Task Stress2(CommandContext ctx, float arg1, double arg2, decimal arg3, TimeSpan arg4, Uri arg5)
        {
            await ctx.RespondAsync("GREAT SUCCESS2");
        }
    }
}