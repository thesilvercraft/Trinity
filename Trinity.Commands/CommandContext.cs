// This file contains a substantial modified portion of code made by the DSharpPlus project.
//
// Copyright (c) 2015 Mike Santiago
// Copyright (c) 2016-2021 DSharpPlus Contributors
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
using Trinity.Shared;

namespace Trinity.Commands
{
    public class CommandContext
    {
        public IPlatformProvider Client { get; internal set; } = null!;
        public List<IPlatformProvider> Clients { get; internal set; } = null!;
        public ITrinityChannel? Channel { get; internal set; } = null;
        public ITrinityGuild? Guild { get; internal set; } = null;
        public ITrinityUser? User { get; internal set; } = null;
        public ITrinityMessage Message { get; internal set; } = null;
        public Command? Command { get; internal set; }

        public CommandOverload Overload { get; internal set; } = null!;

        public string Prefix { get; internal set; } = string.Empty;
        public IReadOnlyList<string> RawArguments { get; internal set; } = Array.Empty<string>();

        /// <summary>
        /// Quickly respond to the message that triggered the command.
        /// </summary>
        /// <param name="content">Message to respond with.</param>
        /// <returns></returns>
        public Task<ITrinityMessage> RespondAsync(string content)
            => Message.RespondAsync(content);

        /// <summary>
        /// Quickly respond to the message that triggered the command.
        /// </summary>
        /// <param name="embed">Embed to attach.</param>
        /// <returns></returns>
        public Task<ITrinityMessage> RespondAsync(TrinityEmbed embed)
            => Message.RespondAsync(embed);

        /// <summary>
        /// Quickly respond to the message that triggered the command.
        /// </summary>
        /// <param name="content">Message to respond with.</param>
        /// <param name="embed">Embed to attach.</param>
        /// <returns></returns>
        public Task<ITrinityMessage> RespondAsync(string content, TrinityEmbed embed)
            => Message.RespondAsync(content, embed);

        /// <summary>
        /// Quickly respond to the message that triggered the command.
        /// </summary>
        /// <param name="builder">The Discord Message builder.</param>
        /// <returns></returns>
        public Task<ITrinityMessage> RespondAsync(TrinityMessageBuilder builder)
            => Message.RespondAsync(builder);

        /// <summary>
        /// Triggers typing in the channel containing the message that triggered the command.
        /// </summary>
        /// <returns></returns>
        public Task TriggerTypingAsync()
            => Channel.TriggerTypingAsync();
    }
}