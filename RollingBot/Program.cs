/*
    RollingBot
    Copyright (C) 2022 SilverDiamond

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using Trinity.DiscordSharpPlus;
using Trinity.Shared;
using Trinity;
using Trinity.Revolt;
using Trinity.Commands;
using RollingBot.Commands;
using RollingBot.Commands.EvaluateCode;
using Trinity.DSharpPlus.Commands;
using SilverBotDS.Converters;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Diagnostics;
using SilverBotDS.Anime;
using Serilog;
using Microsoft.Extensions.Logging;
using SilverBotDS.Objects.Classes;

Console.WriteLine("ROLLING START!!!");
Log.Logger = new LoggerConfiguration()
     .WriteTo.Async(a => a.Console())
    .CreateLogger();
var logFactory = new LoggerFactory().AddSerilog(Log.Logger);
TrinityDiscordClient client = new(new DSharpPlus.DiscordClient(new DSharpPlus.DiscordConfiguration() { Token = Environment.GetEnvironmentVariable("trinityd"), LoggerFactory = logFactory }));
await client.ConnectAsync();
TrinityRevoltClient rc = new(new(), Revolt.TokenType.Bot, Environment.GetEnvironmentVariable("trinityr"));
await rc.ConnectAsync();
var services = new ServiceCollection();
services.AddSingleton(new HttpClient());
services.AddLogging(builder => builder.AddSerilog(Log.Logger));
CommandsNextExtension cn = new(new CommandsNextConfiguration() { StringPrefixes = new List<string> { "~" }, Services = services.BuildServiceProvider() });
cn.SetupEventHandlers(
    (x, y, z, j, h) => { Console.WriteLine("1:"); Console.WriteLine(y.Message); },
    (x, y, z, j, h) => { Console.WriteLine("2:"); Console.WriteLine(y.Message); });
cn.Setup(client, logFactory.CreateLogger("Trinity.CommandsNext"));
cn.Setup(rc);
cn.SetHelpFormatter<CustomHelpFormatter>();
cn.RegisterCommands<TestCommandModule>();
cn.RegisterCommands<CodeEnvCommandModule>();
cn.RegisterCommands<AnimeModule>();
cn.RegisterDiscordConverters();
cn.AddSdImage();
await Task.Delay(-1);