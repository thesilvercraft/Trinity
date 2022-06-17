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

namespace Trinity.Shared;

public class TrinityEmbed
{
    /// <summary>
    /// Gets the embed's title.
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// Gets the embed's type.
    /// </summary>
    public string? Type { get; set; }

    /// <summary>
    /// Gets the embed's description.
    /// </summary>

    public string? Description { get; set; }

    /// <summary>
    /// Gets the embed's url.
    /// </summary>

    public Uri? Url { get; set; }

    /// <summary>
    /// Gets the embed's timestamp.
    /// </summary>

    public DateTimeOffset? Timestamp { get; set; }

    /// <summary>
    /// Gets the embed's color.
    /// </summary>

    public TrinityColor? Color { get; set; }

    /// <summary>
    /// Gets the embed's footer.
    /// </summary>

    public TrinityEmbedFooter? Footer { get; set; }

    /// <summary>
    /// Gets the embed's image.
    /// </summary>

    public TrinityEmbedImage? Image { get; set; }

    /// <summary>
    /// Gets the embed's thumbnail.
    /// </summary>

    public TrinityEmbedThumbnail? Thumbnail { get; set; }

    /// <summary>
    /// Gets the embed's video.
    /// </summary>

    public TrinityEmbedVideo? Video { get; set; }

    /// <summary>
    /// Gets the embed's author.
    /// </summary>

    public TrinityEmbedAuthor? Author { get; set; }

    /// <summary>
    /// Gets the embed's fields.
    /// </summary>

    public List<TrinityEmbedField>? Fields { get; set; } = new();
}