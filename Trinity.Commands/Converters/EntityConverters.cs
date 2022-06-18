// This file is part of the DSharpPlus project.
//
// Copyright (c) 2015 Mike Santiago
// Copyright (c) 2016-2022 DSharpPlus Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using Emzi0767;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Trinity.Shared;

namespace Trinity.Commands.Converters
{
    /*  public class DiscordEmojiConverter : IArgumentConverter<DiscordEmoji>
      {
          private static Regex EmoteRegex { get; }

          static DiscordEmojiConverter()
          {
  #if NETSTANDARD1_3
              EmoteRegex = new Regex(@"^<a?:([a-zA-Z0-9_]+?):(\d+?)>$", RegexOptions.ECMAScript);
  #else
              EmoteRegex = new Regex(@"^<(?<animated>a)?:(?<name>[a-zA-Z0-9_]+?):(?<id>\d+?)>$", RegexOptions.ECMAScript | RegexOptions.Compiled);
  #endif
          }

          Task<Optional<DiscordEmoji>> IArgumentConverter<DiscordEmoji>.ConvertAsync(string value, CommandContext ctx)
          {
              if (DiscordEmoji.TryFromUnicode(ctx.Client, value, out var emoji))
              {
                  var result = emoji;
                  var ret = Optional.FromValue(result);
                  return Task.FromResult(ret);
              }

              var m = EmoteRegex.Match(value);
              if (m.Success)
              {
                  var sid = m.Groups["id"].Value;
                  var name = m.Groups["name"].Value;
                  var anim = m.Groups["animated"].Success;

                  if (!ulong.TryParse(sid, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id))
                      return Task.FromResult(Optional.FromNoValue<DiscordEmoji>());

                  return DiscordEmoji.TryFromGuildEmote(ctx.Client, id, out emoji)
                      ? Task.FromResult(Optional.FromValue(emoji))
                      : Task.FromResult(Optional.FromValue(new DiscordEmoji
                      {
                          Discord = ctx.Client,
                          Id = id,
                          Name = name,
                          IsAnimated = anim,
                          RequiresColons = true,
                          IsManaged = false
                      }));
              }

              return Task.FromResult(Optional.FromNoValue<DiscordEmoji>());
          }
      }*/

    public class TrinityColorConverter : IArgumentConverter<TrinityColor>
    {
        private static Regex ColorRegexHex { get; }
        private static Regex ColorRegexRgb { get; }

        static TrinityColorConverter()
        {
            ColorRegexHex = new Regex(@"^#?([a-fA-F0-9]{6})$", RegexOptions.ECMAScript | RegexOptions.Compiled);
            ColorRegexRgb = new Regex(@"^(\d{1,3})\s*?,\s*?(\d{1,3}),\s*?(\d{1,3})$", RegexOptions.ECMAScript | RegexOptions.Compiled);
        }

        public Task<Optional<TrinityColor>> ConvertAsync(string value, CommandContext ctx)
        {
            var m = ColorRegexHex.Match(value);
            if (m.Success && int.TryParse(m.Groups[1].Value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var clr))
                return Task.FromResult(Optional.FromValue<TrinityColor>(new(clr)));

            m = ColorRegexRgb.Match(value);
            if (m.Success)
            {
                var p1 = byte.TryParse(m.Groups[1].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var r);
                var p2 = byte.TryParse(m.Groups[2].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var g);
                var p3 = byte.TryParse(m.Groups[3].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var b);

                return !(p1 && p2 && p3)
                    ? Task.FromResult(Optional.FromNoValue<TrinityColor>())
                    : Task.FromResult(Optional.FromValue(new TrinityColor(r, g, b)));
            }

            return Task.FromResult(Optional.FromNoValue<TrinityColor>());
        }
    }
}