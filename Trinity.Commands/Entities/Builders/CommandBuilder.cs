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
using System.Collections.ObjectModel;
using System.Linq;
using Trinity.Commands.Attributes;
using Trinity.Commands.Entities;
using Trinity.Commands.Exceptions;

namespace Trinity.Commands.Builders
{
    /// <summary>
    /// Represents an interface to build a command.
    /// </summary>
    public class CommandBuilder
    {
        /// <summary>
        /// Gets the name set for this command.
        /// </summary>
        public string Name { get; private set; } = null!;

        /// <summary>
        /// Gets the aliases set for this command.
        /// </summary>
        public IReadOnlyList<string> Aliases { get; }
        private List<string> _aliasList { get; }

        /// <summary>
        /// Gets the description set for this command.
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// Gets whether this command will be hidden or not.
        /// </summary>
        public bool IsHidden { get; private set; }

        /// <summary>
        /// Gets the execution checks defined for this command.
        /// </summary>
        public IReadOnlyList<CheckBaseAttribute> ExecutionChecks { get; }
        private List<CheckBaseAttribute> _executionCheckList { get; }

        /// <summary>
        /// Gets the collection of this command's overloads.
        /// </summary>
        public IReadOnlyList<CommandOverloadBuilder> Overloads { get; }
        private List<CommandOverloadBuilder> _overloadList { get; }
        private HashSet<string> _overloadArgumentSets { get; }

        /// <summary>
        /// Gets the module on which this command is to be defined.
        /// </summary>
        public ICommandModule? Module { get; }

        /// <summary>
        /// Gets custom attributes defined on this command.
        /// </summary>
        public IReadOnlyList<Attribute> CustomAttributes { get; }
        private List<Attribute> _customAttributeList { get; }

        /// <summary>
        /// Creates a new module-less command builder.
        /// </summary>
        public CommandBuilder() : this(null) { }

        /// <summary>
        /// Creates a new command builder.
        /// </summary>
        /// <param name="module">Module on which this command is to be defined.</param>
        public CommandBuilder(ICommandModule? module)
        {
            _aliasList = new List<string>();
            Aliases = new ReadOnlyCollection<string>(_aliasList);

            _executionCheckList = new List<CheckBaseAttribute>();
            ExecutionChecks = new ReadOnlyCollection<CheckBaseAttribute>(_executionCheckList);

            _overloadArgumentSets = new HashSet<string>();
            _overloadList = new List<CommandOverloadBuilder>();
            Overloads = new ReadOnlyCollection<CommandOverloadBuilder>(_overloadList);

            Module = module;

            _customAttributeList = new List<Attribute>();
            CustomAttributes = new ReadOnlyCollection<Attribute>(_customAttributeList);
        }

        /// <summary>
        /// Sets the name for this command.
        /// </summary>
        /// <param name="name">Name for this command.</param>
        /// <returns>This builder.</returns>
        public CommandBuilder WithName(string name)
        {
            if (name == null || name.ToCharArray().Any(xc => char.IsWhiteSpace(xc)))
                throw new ArgumentException("Command name cannot be null or contain any whitespace characters.", nameof(name));
            else if (Name != null)
                throw new InvalidOperationException("This command already has a name.");
            else if (_aliasList.Contains(name))
                throw new ArgumentException("Command name cannot be one of its aliases.", nameof(name));

            Name = name;
            return this;
        }

        /// <summary>
        /// Adds aliases to this command.
        /// </summary>
        /// <param name="aliases">Aliases to add to the command.</param>
        /// <returns>This builder.</returns>
        public CommandBuilder WithAliases(params string[] aliases)
        {
            if (aliases == null || !aliases.Any())
                throw new ArgumentException("You need to pass at least one alias.", nameof(aliases));

            foreach (var alias in aliases)
                WithAlias(alias);

            return this;
        }

        /// <summary>
        /// Adds an alias to this command.
        /// </summary>
        /// <param name="alias">Alias to add to the command.</param>
        /// <returns>This builder.</returns>
        public CommandBuilder WithAlias(string alias)
        {
            if (alias.ToCharArray().Any(xc => char.IsWhiteSpace(xc)))
                throw new ArgumentException("Aliases cannot contain whitespace characters or null strings.", nameof(alias));

            if (Name == alias || _aliasList.Contains(alias))
                throw new ArgumentException("Aliases cannot contain the command name, and cannot be duplicate.", nameof(alias));

            _aliasList.Add(alias);
            return this;
        }

        /// <summary>
        /// Sets the description for this command.
        /// </summary>
        /// <param name="description">Description to use for this command.</param>
        /// <returns>This builder.</returns>
        public CommandBuilder WithDescription(string description)
        {
            Description = description;
            return this;
        }

        /// <summary>
        /// Sets whether this command is to be hidden.
        /// </summary>
        /// <param name="hidden">Whether the command is to be hidden.</param>
        /// <returns>This builder.</returns>
        public CommandBuilder WithHiddenStatus(bool hidden)
        {
            IsHidden = hidden;
            return this;
        }

        /// <summary>
        /// Adds pre-execution checks to this command.
        /// </summary>
        /// <param name="checks">Pre-execution checks to add to this command.</param>
        /// <returns>This builder.</returns>
        public CommandBuilder WithExecutionChecks(params CheckBaseAttribute[] checks)
        {
            _executionCheckList.AddRange(checks.Except(_executionCheckList));
            return this;
        }

        /// <summary>
        /// Adds a pre-execution check to this command.
        /// </summary>
        /// <param name="check">Pre-execution check to add to this command.</param>
        /// <returns>This builder.</returns>
        public CommandBuilder WithExecutionCheck(CheckBaseAttribute check)
        {
            if (!_executionCheckList.Contains(check))
                _executionCheckList.Add(check);
            return this;
        }

        /// <summary>
        /// Adds overloads to this command. An executable command needs to have at least one overload.
        /// </summary>
        /// <param name="overloads">Overloads to add to this command.</param>
        /// <returns>This builder.</returns>
        public CommandBuilder WithOverloads(params CommandOverloadBuilder[] overloads)
        {
            foreach (var overload in overloads)
                WithOverload(overload);

            return this;
        }

        /// <summary>
        /// Adds an overload to this command. An executable command needs to have at least one overload.
        /// </summary>
        /// <param name="overload">Overload to add to this command.</param>
        /// <returns>This builder.</returns>
        public CommandBuilder WithOverload(CommandOverloadBuilder overload)
        {
            if (_overloadArgumentSets.Contains(overload._argumentSet))
                throw new DuplicateOverloadException(Name, overload.Arguments.Select(x => x.Type).ToList(), overload._argumentSet);

            _overloadArgumentSets.Add(overload._argumentSet);
            _overloadList.Add(overload);

            return this;
        }

        /// <summary>
        /// Adds a custom attribute to this command. This can be used to indicate various custom information about a command.
        /// </summary>
        /// <param name="attribute">Attribute to add.</param>
        /// <returns>This builder.</returns>
        public CommandBuilder WithCustomAttribute(Attribute attribute)
        {
            _customAttributeList.Add(attribute);
            return this;
        }

        /// <summary>
        /// Adds multiple custom attributes to this command. This can be used to indicate various custom information about a command.
        /// </summary>
        /// <param name="attributes">Attributes to add.</param>
        /// <returns>This builder.</returns>
        public CommandBuilder WithCustomAttributes(params Attribute[] attributes)
        {
            foreach (var attr in attributes)
                WithCustomAttribute(attr);

            return this;
        }

        internal virtual Command Build(CommandGroup? parent)
        {
            var cmd = new Command
            {
                Name = string.IsNullOrWhiteSpace(Name)
                    ? throw new InvalidOperationException($"Cannot build a command with an invalid name. Use the method {nameof(this.WithName)} to set a valid name.")
                    : Name,

                Description = Description,
                Aliases = Aliases,
                ExecutionChecks = ExecutionChecks,
                IsHidden = IsHidden,
                Parent = parent,
                Overloads = new ReadOnlyCollection<CommandOverload>(Overloads.Select(xo => xo.Build()).ToList()),
                Module = Module,
                CustomAttributes = CustomAttributes
            };

            return cmd;
        }
    }
}
