// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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

using McMaster.Extensions.CommandLineUtils;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.ILSpy
{
	internal enum InstancingMode
	{
		Single,
		Multi
	}

	sealed class CommandLineArguments
	{
		// see /doc/Command Line.txt for details
		public List<string> AssembliesToLoad = new List<string>();
		public bool? SingleInstance;
		public string NavigateTo;
		public string Search;
		public string Language;
		public bool NoActivate;
		public string ConfigFile;

		public CommandLineArguments(IEnumerable<string> arguments)
		{
			var app = new CommandLineApplication() {
				ResponseFileHandling = ResponseFileHandling.ParseArgsAsSpaceSeparated,
			};

			var oInstancing = app.Option("-i|--instancing <single/multi>",
				"Single or multi instance",
				CommandOptionType.SingleValue);

			var oNavigateTo = app.Option<string>("-n|--navigateto <TYPENAME>",
				"Navigates to the member specified by the given ID string.\r\nThe member is searched for only in the assemblies specified on the command line.\r\nExample: 'ILSpy ILSpy.exe --navigateTo:T:ICSharpCode.ILSpy.CommandLineArguments'",
				CommandOptionType.SingleValue);
			oNavigateTo.DefaultValue = null;

			var oSearch = app.Option<string>("-s|--search <SEARCHTERM>",
				"Search for t:TypeName, m:Member or c:Constant; use exact match (=term), 'should not contain' (-term) or 'must contain' (+term); use /reg(ular)?Ex(pressions)?/ or both - t:/Type(Name)?/...",
				CommandOptionType.SingleValue);
			oSearch.DefaultValue = null;

			var oLanguage = app.Option<string>("-l|--language <LANGUAGEIDENTIFIER>",
				"Selects the specified language.\r\nExample: 'ILSpy --language:C#' or 'ILSpy --language:IL'",
				CommandOptionType.SingleValue);
			oLanguage.DefaultValue = null;

			var oConfig = app.Option<string>("-c|--config <CONFIGFILENAME>",
				"Provide a specific configuration file.\r\nExample: 'ILSpy --config:myconfig.xml'",
				CommandOptionType.SingleValue);
			oConfig.DefaultValue = null;

			var oNoActivate = app.Option("--noactivate",
				"Do not activate the existing ILSpy instance. This option has no effect if a new ILSpy instance is being started.",
				CommandOptionType.NoValue);

			// https://natemcmaster.github.io/CommandLineUtils/docs/arguments.html#variable-numbers-of-arguments
			// To enable this, MultipleValues must be set to true, and the argument must be the last one specified.
			var files = app.Argument("Assemblies", "Assemblies to load", multipleValues: true);


			// string helptext = app.GetHelpText();

			app.Parse(arguments.ToArray());

			if (oInstancing.Value != null)
			{
				if (Enum.TryParse<InstancingMode>(oInstancing.Value(), true, out var mode))
				{
					switch (mode)
					{
						case InstancingMode.Single:
							SingleInstance = true;
							break;
						case InstancingMode.Multi:
							SingleInstance = false;
							break;
					}
				}
			}

			NavigateTo = oNavigateTo.ParsedValue;
			Search = oSearch.ParsedValue;
			Language = oLanguage.ParsedValue;
			ConfigFile = oConfig.ParsedValue;

			if (oNoActivate.HasValue())
				NoActivate = true;

			foreach (var assembly in files.Values)
			{
				if (!string.IsNullOrWhiteSpace(assembly))
					AssembliesToLoad.Add(assembly);
			}
		}
	}
}
