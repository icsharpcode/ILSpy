// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;

using McMaster.Extensions.CommandLineUtils;

namespace ICSharpCode.ILSpy.AppEnv
{
	public sealed class CommandLineArguments
	{
		public List<string> AssembliesToLoad = new();
		public bool? SingleInstance;
		public string NavigateTo;
		public string Search;
		public string Language;
		public bool NoActivate;
		public string ConfigFile;

		public CommandLineApplication ArgumentsParser { get; }

		CommandLineArguments(CommandLineApplication app)
		{
			ArgumentsParser = app;
		}

		public static CommandLineArguments Create(IEnumerable<string> arguments)
		{
			var app = new CommandLineApplication {
				ResponseFileHandling = ResponseFileHandling.ParseArgsAsLineSeparated,
				UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue
			};
			app.HelpOption();

			var instance = new CommandLineArguments(app);

			try
			{
				var oForceNewInstance = app.Option("--newinstance",
					"Start a new instance of ILSpy even if the user configuration is set to single-instance",
					CommandOptionType.NoValue);

				var oNavigateTo = app.Option<string>("-n|--navigateto <TYPENAME>",
					"Navigates to the member specified by the given ID string.",
					CommandOptionType.SingleValue);

				var oSearch = app.Option<string>("-s|--search <SEARCHTERM>",
					"Search for t:TypeName, m:Member or c:Constant.",
					CommandOptionType.SingleValue);

				var oLanguage = app.Option<string>("-l|--language <LANGUAGEIDENTIFIER>",
					"Selects the specified language.",
					CommandOptionType.SingleValue);

				var oConfig = app.Option<string>("-c|--config <CONFIGFILENAME>",
					"Provide a specific configuration file.",
					CommandOptionType.SingleValue);

				var oNoActivate = app.Option("--noactivate",
					"Do not activate the existing ILSpy instance.",
					CommandOptionType.NoValue);

				var files = app.Argument("Assemblies", "Assemblies to load", multipleValues: true);

				app.Parse(arguments.ToArray());

				if (oForceNewInstance.HasValue())
					instance.SingleInstance = false;

				instance.NavigateTo = oNavigateTo.ParsedValue;
				instance.Search = oSearch.ParsedValue;
				instance.Language = oLanguage.ParsedValue;
				instance.ConfigFile = oConfig.ParsedValue;

				if (oNoActivate.HasValue())
					instance.NoActivate = true;

				foreach (var assembly in files.Values)
				{
					if (!string.IsNullOrWhiteSpace(assembly))
						instance.AssembliesToLoad.Add(assembly);
				}
			}
			catch (Exception)
			{
				// Intentionally swallowed: we want startup to never throw on bad args.
			}

			return instance;
		}
	}
}