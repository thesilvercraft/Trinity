// This file contains parts of a file of the DSharpPlus project.
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Shared
{
    /// <summary>
    /// Constructs a Message to be sent.
    /// </summary>
    public sealed class TrinityMessageBuilder
    {
        /// <summary>
        /// Gets or Sets the Message to be sent.
        /// </summary>
        public string Content
        {
            get => _content;
            set
            {
                if (value != null && value.Length > 2000)
                    throw new ArgumentException("Content cannot exceed 2000 characters.", nameof(value));
                _content = value;
            }
        }

        private string _content;

        /// <summary>
        /// Gets or sets the embed for the builder. This will always set the builder to have one embed.
        /// </summary>
        public TrinityEmbed Embed
        {
            get => _embeds.Count > 0 ? _embeds[0] : null;
            set
            {
                _embeds.Clear();
                _embeds.Add(value);
            }
        }

        /// <summary>
        /// Gets the Embeds to be sent.
        /// </summary>
        public IReadOnlyList<TrinityEmbed> Embeds => _embeds;

        private readonly List<TrinityEmbed> _embeds = new();

        /// <summary>
        /// Gets or Sets if the message should be TTS.
        /// </summary>
        public bool IsTTS { get; set; } = false;

        /// <summary>
        /// Gets the Allowed Mentions for the message to be sent.
        /// </summary>
        public List<Mention> Mentions { get; private set; } = null;

        /// <summary>
        /// Gets the Reply Message ID.
        /// </summary>
        public TrinityGuid? ReplyId { get; private set; } = null;

        /// <summary>
        /// Gets if the Reply should mention the user.
        /// </summary>
        public bool MentionOnReply { get; private set; } = false;

        /// <summary>
        /// Gets if the Reply will error if the Reply Message Id does not reference a valid message.
        /// <para>If set to false, invalid replies are send as a regular message.</para>
        /// <para>Defaults to false.</para>
        /// </summary>
        public bool FailOnInvalidReply { get; set; }

        /// <summary>
        /// Sets the Content of the Message.
        /// </summary>
        /// <param name="content">The content to be set.</param>
        /// <returns>The current builder to be chained.</returns>
        public TrinityMessageBuilder WithContent(string content)
        {
            Content = content;
            return this;
        }

        /// <summary>
        /// Sets the embed for the current builder.
        /// </summary>
        /// <param name="embed">The embed that should be set.</param>
        /// <returns>The current builder to be chained.</returns>
        public TrinityMessageBuilder WithEmbed(TrinityEmbed embed)
        {
            if (embed == null)
                return this;

            Embed = embed;
            return this;
        }

        /// <summary>
        /// Appends an embed to the current builder.
        /// </summary>
        /// <param name="embed">The embed that should be appended.</param>
        /// <returns>The current builder to be chained.</returns>
        public TrinityMessageBuilder AddEmbed(TrinityEmbed embed)
        {
            if (embed == null)
                return this; //Providing null embeds will produce a 400 response from Discord.//
            _embeds.Add(embed);
            return this;
        }

        /// <summary>
        /// Gets the Files to be sent in the Message.
        /// </summary>
        public IReadOnlyCollection<TrinityMessageFile> Files => this._files;

        internal readonly List<TrinityMessageFile> _files = new();

        public TrinityMessageBuilder WithFile(string fileName, Stream stream, bool resetStreamPosition = false)
        {
            if (this.Files.Count >= 10)
                throw new ArgumentException("Cannot send more than 10 files with a single message.");

            if (this._files.Any(x => x.FileName == fileName))
                throw new ArgumentException("A File with that filename already exists");

            if (resetStreamPosition)
                this._files.Add(new TrinityMessageFile(fileName, stream, stream.Position));
            else
                this._files.Add(new TrinityMessageFile(fileName, stream, null));

            return this;
        }

        public TrinityMessageBuilder WithFile(FileStream stream, bool resetStreamPosition = false)
        {
            if (this.Files.Count >= 10)
                throw new ArgumentException("Cannot send more than 10 files with a single message.");

            if (this._files.Any(x => x.FileName == stream.Name))
                throw new ArgumentException("A File with that filename already exists");

            if (resetStreamPosition)
                this._files.Add(new TrinityMessageFile(stream.Name, stream, stream.Position));
            else
                this._files.Add(new TrinityMessageFile(stream.Name, stream, null));

            return this;
        }

        public TrinityMessageBuilder WithFiles(Dictionary<string, Stream> files, bool resetStreamPosition = false)
        {
            if (this.Files.Count + files.Count > 10)
                throw new ArgumentException("Cannot send more than 10 files with a single message.");

            foreach (var file in files)
            {
                if (this._files.Any(x => x.FileName == file.Key))
                    throw new ArgumentException("A File with that filename already exists");

                if (resetStreamPosition)
                    this._files.Add(new TrinityMessageFile(file.Key, file.Value, file.Value.Position));
                else
                    this._files.Add(new TrinityMessageFile(file.Key, file.Value, null));
            }

            return this;
        }

        /// <summary>
        /// Appends several embeds to the current builder.
        /// </summary>
        /// <param name="embeds">The embeds that should be appended.</param>
        /// <returns>The current builder to be chained.</returns>
        public TrinityMessageBuilder AddEmbeds(IEnumerable<TrinityEmbed> embeds)
        {
            _embeds.AddRange(embeds);
            return this;
        }

        /// <summary>
        /// Sets if the message has allowed mentions.
        /// </summary>
        /// <param name="allowedMention">The allowed Mention that should be sent.</param>
        /// <returns>The current builder to be chained.</returns>
        public TrinityMessageBuilder WithAllowedMention(Mention allowedMention)
        {
            if (Mentions != null)
                Mentions.Add(allowedMention);
            else
                Mentions = new List<Mention> { allowedMention };

            return this;
        }

        /// <summary>
        /// Sets if the message has allowed mentions.
        /// </summary>
        /// <param name="allowedMentions">The allowed Mentions that should be sent.</param>
        /// <returns>The current builder to be chained.</returns>
        public TrinityMessageBuilder WithAllowedMentions(IEnumerable<Mention> allowedMentions)
        {
            if (Mentions != null)
                Mentions.AddRange(allowedMentions);
            else
                Mentions = allowedMentions.ToList();

            return this;
        }

        /// <summary>
        /// Sends the Message to a specific channel
        /// </summary>
        /// <param name="channel">The channel the message should be sent to.</param>
        /// <returns>The current builder to be chained.</returns>
        public Task<ITrinityMessage> SendAsync(ITrinityChannelWithAdvancedSendingMethods channel) => channel.SendMessageAsync(this);

        /// <summary>
        /// Sends the modified message.
        /// <para>Note: Message replies cannot be modified. To clear the reply, simply pass <see langword="null"/> to <see cref="WithReply"/>.</para>
        /// </summary>
        /// <param name="msg">The original Message to modify.</param>
        /// <returns>The current builder to be chained.</returns>
        public Task<ITrinityMessage> ModifyAsync(ITrinityMessage msg) => msg.ModifyAsync(this);

        /// <summary>
        /// Allows for clearing the Message Builder so that it can be used again to send a new message.
        /// </summary>
        public void Clear()
        {
            Content = "";
            _embeds.Clear();
            IsTTS = false;
            Mentions = null;
            ReplyId = null;
            MentionOnReply = false;
        }
    }
}