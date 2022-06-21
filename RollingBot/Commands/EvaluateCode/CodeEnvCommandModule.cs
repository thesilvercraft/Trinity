using Humanizer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using RollingBot.Shared;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Trinity.Commands;
using Trinity.Commands.Attributes;
using Trinity.Shared;

namespace RollingBot.Commands.EvaluateCode
{
    [Category("Owner only")]
    [RequireOwner]
    internal class CodeEnvCommandModule : BaseCommandModule
    {
        public static string RemoveCodeBraces(string str)
        {
            if (str.StartsWith("```csharp"))
            {
                str = str.Remove(0, 9);
            }

            if (str.StartsWith("```cs"))
            {
                str = str.Remove(0, 5);
            }

            if (str.StartsWith("```js"))
            {
                str = str.Remove(0, 5);
            }

            if (str.StartsWith("```javascript"))
            {
                str = str.Remove(0, 14);
            }

            if (str.StartsWith("```"))
            {
                str = str.Remove(0, 3);
            }

            if (str.StartsWith("``"))
            {
                str = str.Remove(0, 2);
            }

            if (str.StartsWith("`"))
            {
                str = str.Remove(0, 1);
            }

            if (str.EndsWith("```"))
            {
                str = str.Remove(str.Length - 3, 3);
            }

            if (str.EndsWith("``"))
            {
                str = str.Remove(str.Length - 2, 2);
            }

            if (str.EndsWith("`"))
            {
                str = str.Remove(str.Length - 1, 1);
            }

            return str;
        }

        public static async Task SendStringFileWithContent(CommandContext ctx, string title, string file, ITrinityChannelWithAdvancedSendingMethods c,
     string filename = "message.txt")
        {
            await new TrinityMessageBuilder().WithContent(title)
                .WithFile(filename, new MemoryStream(Encoding.UTF8.GetBytes(file))).WithAllowedMentions(Array.Empty<Mention>())
                .SendAsync(c);
        }

        public static async Task SendBestRepresentationAsync(object ob, CommandContext ctx, ITrinityChannelWithAdvancedSendingMethods c)
        {
            try
            {
                var str = ob.ToString();
                if (ob is TimeSpan span)
                {
                    str = span.Humanize(20);
                }
                else if (ob is DateTime time)
                {
                    str = time.ToLongDateString();
                }
                else if (ob is string @string)
                {
                    str = $"```{@string}```";
                }
                else if (ob.GetType().IsSerializable || ob.GetType().IsArray || ob.GetType().IsEnum ||
                         ob.GetType().FullName == ob.ToString())
                {
                    str = JsonSerializer.Serialize(ob, Options);
                    if (str.Length >= 2000)
                    {
                        await SendStringFileWithContent(ctx, ob.GetType().FullName, str, c, "eval.txt");
                        return;
                    }
                    str = $"```json{str}```";
                }
                else
                {
                    str = $"```{str}```";
                }

                if (ob.ToString().Length >= 2000)
                {
                    await SendStringFileWithContent(ctx, ob.GetType().FullName, str, c, "eval.txt");
                    return;
                }

                await new TrinityMessageBuilder().WithContent($"{ob.GetType().FullName} {str}")
                    .WithAllowedMentions(Array.Empty<Mention>()).SendAsync(c);
            }
            catch (Exception e)
            {
                await new TrinityMessageBuilder()
                    .WithContent($"Failed to parse `{ob.GetType().FullName}` as a string, using the generic ToString. {ob}")
                    .WithAllowedMentions(Array.Empty<Mention>()).SendAsync(c);
            }
        }

        public static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true
        };

        private readonly string[] _imports =
        {
        "System", "System.Collections.Generic", "System.Diagnostics", "System.IO", "System.IO.Compression",
        "System.Text", "System.Text.RegularExpressions", "System.Threading.Tasks", "System.Linq", "Humanizer",
        "System.Globalization", "System.Text.Json",
        };

        /// <summary>
        ///     Stolen idea from
        ///     https://github.com/Voxel-Fox-Ltd/VoxelBotUtils/blob/master/voxelbotutils/cogs/owner_only.py#L172-L252
        /// </summary>
        [Command("evaluate")]
        [Description("EVALUATE SOME C# CODE")]
        [Aliases("eval", "ev")]
        public async Task Eval(CommandContext ctx, [RemainingText] string code)
        {
            if (ctx.Channel is ITrinityChannelWithAdvancedSendingMethods c)
            {
                var console = Console.Out;
                try
                {
                    using var sw = new StringWriter();
                    Console.SetOut(sw);
                    var sw1 = Stopwatch.StartNew();
                    var script = CSharpScript.Create(RemoveCodeBraces(code),
                        ScriptOptions.Default
                            .WithReferences(AppDomain.CurrentDomain.GetAssemblies()
                                .Where(xa => !xa.IsDynamic && !string.IsNullOrWhiteSpace(xa.Location))).WithImports(_imports),
                        typeof(CodeEnv));
                    var diag = script.Compile();
                    sw1.Stop();
                    if (diag.Length != 0)
                    {
                        if (diag.Humanize().Length > 1958)
                        {
                            await SendStringFileWithContent(ctx, "Compilation Diagnostics showed up:", diag.Humanize(),
                                c, "diag.txt");
                        }
                        else
                        {
                            await new TrinityMessageBuilder()
                                .WithContent(
                                    $"Compilation Diagnostics showed up: ```cs{RemoveCodeBraces(diag.Humanize())}```")
                                .SendAsync(c);
                        }

                        var errcount = diag.LongCount(x => x.Severity == DiagnosticSeverity.Error);
                        if (errcount != 0)
                        {
                            await new TrinityMessageBuilder()
                                .WithContent(
                                    $"I found {errcount} {(errcount == 1 ? "error" : "errors")}. I will **NOT** attempt to run this code.")
                                .SendAsync(c);
                            Console.SetOut(console);
                            return;
                        }
                    }

                    await new TrinityMessageBuilder().WithContent($"Compiled the code in {sw1.Elapsed.Humanize(6)}")
                        .SendAsync(c);
                    sw1.Start();
                    var sw2 = Stopwatch.StartNew();
                    var result = await script.RunAsync(new CodeEnv(ctx));
                    if (result.ReturnValue is not null)
                    {
                        await SendBestRepresentationAsync(result.ReturnValue, ctx, c);
                    }
                    else
                    {
                        await new TrinityMessageBuilder().WithContent("The evaluated code returned a `null`.")
                            .SendAsync(c);
                    }

                    if (!string.IsNullOrEmpty(sw.ToString()))
                    {
                        if (sw.ToString().Length > 1978)
                        {
                            await SendStringFileWithContent(ctx, "Console Output:", sw.ToString(), c, "console.txt");
                        }
                        else
                        {
                            await new TrinityMessageBuilder()
                                .WithContent($"Console Output: ```{sw}```").SendAsync(c);
                        }
                    }

                    sw.Close();
                    Console.SetOut(console);
                    await new TrinityMessageBuilder()
                        .WithContent(
                            $"Executed the code in **{sw2.Elapsed.Humanize(6)}** excluding compile time, or **{(sw1.Elapsed.Humanize(6))}** including it.")
                        .SendAsync(c);
                    sw1.Stop();
                    sw2.Stop();
                    result = null;
                    script = null;
                }
                catch (CompilationErrorException e)
                {
                    Console.SetOut(console);
                    if (e.Diagnostics.Humanize().Length > 1958)
                    {
                        await SendStringFileWithContent(ctx, "Compilation Error occurred:", e.Diagnostics.Humanize(),
                            c, "error.txt");
                    }
                    else
                    {
                        await new TrinityMessageBuilder()
                            .WithContent(
                                $"Compilation Error occurred: ```cs{RemoveCodeBraces(e.Diagnostics.Humanize())}```")
                            .SendAsync(c);
                    }

                    throw;
                }
                catch (Exception)
                {
                    Console.SetOut(console);
                    throw;
                }
            }
        }
    }
}