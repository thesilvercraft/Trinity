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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Trinity.Shared;
using Microsoft.Extensions.DependencyInjection;

namespace Trinity.Commands
{
    /// <summary>
    /// Represents a context in which a command is executed.
    /// </summary>
    public sealed class CommandContext
    {
        /// <summary>
        /// Gets the client which received the message.
        /// </summary>
        public IPlatformProvider Client { get; internal set; } = null!;

        /// <summary>
        /// Gets the message that triggered the execution.
        /// </summary>
        public ITrinityMessage Message { get; internal set; } = null!;

        /// <summary>
        /// Gets the channel in which the execution was triggered,
        /// </summary>
        public ITrinityChannel Channel
            => Message.Channel;

        /// <summary>
        /// Gets the guild in which the execution was triggered. This property is null for commands sent over direct messages.
        /// </summary>
        public ITrinityGuild Guild
            => Channel.Guild;

        /// <summary>
        /// Gets the user who triggered the execution.
        /// </summary>
        public ITrinityUser User
            => Message.Author;

        /// <summary>
        /// Gets the member who triggered the execution. This property is null for commands sent over direct messages.
        /// </summary>
        public ITrinityUser? Member
            => _lazyMember.Value;

        private readonly Lazy<ITrinityUser?> _lazyMember;

        /// <summary>
        /// Gets the CommandsNext service instance that handled this command.
        /// </summary>
        public CommandsNextExtension CommandsNext { get; internal set; } = null!;

        /// <summary>
        /// Gets the service provider for this CNext instance.
        /// </summary>
        public IServiceProvider Services { get; internal set; } = null!;

        /// <summary>
        /// Gets the command that is being executed.
        /// </summary>
        public Command? Command { get; internal set; }

        /// <summary>
        /// Gets the overload of the command that is being executed.
        /// </summary>
        public CommandOverload Overload { get; internal set; } = null!;

        /// <summary>
        /// Gets the list of raw arguments passed to the command.
        /// </summary>
        public IReadOnlyList<string> RawArguments { get; internal set; } = Array.Empty<string>();

        /// <summary>
        /// Gets the raw string from which the arguments were extracted.
        /// </summary>
        public string RawArgumentString { get; internal set; } = string.Empty;

        /// <summary>
        /// Gets the prefix used to invoke the command.
        /// </summary>
        public string Prefix { get; internal set; } = string.Empty;

        public CommandsNextConfiguration Config { get; internal set; } = null!;

        internal ServiceContext ServiceScopeContext { get; set; }

        public CommandContext()
        {
            // _lazyMember = new Lazy<ITrinityUser?>(() => Guild is not null && Guild.Members.TryGetValue(User.Id, out var member) ? member : Guild?.GetMemberAsync(User.Id).ConfigureAwait(false).GetAwaiter().GetResult());
        }

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
        {
            return Message.RespondAsync(embed);
        }

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

        internal struct ServiceContext : IDisposable
        {
            public IServiceProvider Provider { get; }
            public IServiceScope Scope { get; }
            public bool IsInitialized { get; }

            public ServiceContext(IServiceProvider services, IServiceScope scope)
            {
                Provider = services;
                Scope = scope;
                IsInitialized = true;
            }

            public void Dispose() => Scope?.Dispose();
        }
    }
}