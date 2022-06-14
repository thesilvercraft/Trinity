﻿// This file contains a substantial modified portion of code made by the DSharpPlus project.
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
namespace Trinity.Commands
{
    public class Command
    {
        public string[] Names { get; set; } = new[] { string.Empty };
        public string? Description { get; set; }
        public string? Usage { get; set; }
        public string? Help { get; set; }
        public string? Category { get; set; }
        public bool Hidden { get; set; }

        public IReadOnlyList<CommandOverload> Overloads { get; internal set; } = Array.Empty<CommandOverload>();
        public IReadOnlyList<CheckBaseAttribute> ExecutionChecks { get; internal set; } = Array.Empty<CheckBaseAttribute>();

        public virtual async Task<bool> Execute(CommandContext ctx, string[] args)
        {
            return await Task.FromResult(false);
        }
    }
}