// Copyright (c) 2026 Christoph Wille
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

using System;
using System.Collections.Generic;
using System.Linq;

using CliFx.Binding;

namespace ICSharpCode.ILSpyCmd
{
	/// <summary>
	/// Translates the verb-less command line into the form CliFx parses. CliFx reads positional
	/// parameters only up to the first option, lets an option collect every following bare token,
	/// splits every single-dash token into one-letter options and knows no ':' or '=' delimiters.
	/// The verb-less command line allows options and input files in any order, gives an option
	/// exactly one value per occurrence, spells some aliases with several letters after one dash,
	/// and attaches values with ':' or '='. The rewrite emits the input files first, then every
	/// option as "--name" followed by its one value, and rejects what CliFx would misread.
	/// </summary>
	static class LegacyArgumentRewriter
	{
		// Single-dash spellings longer than one letter; a CliFx short name is one character.
		static readonly Dictionary<string, string> multiLetterAliases = new() {
			["il"] = "ilcode",
			["lv"] = "languageversion",
			["ds"] = "decompiler-setting",
			["genpdb"] = "generate-pdb",
			["usepdb"] = "use-varnames-from-pdb",
		};

		// Takes a value only when one is attached with ':' or '='; a bare token after it is an input file.
		const string optionalValueOption = "use-varnames-from-pdb";

		// A developer switch kept out of the help; CliFx has no hidden options.
		const string jsonOnlyOption = "--generate-diagrammer-json-only";

		public static bool TryRewrite(string[] args, CommandDescriptor command, out string[] rewritten, out bool jsonOnly, out string error)
		{
			var options = command.Inputs.OfType<CommandOptionDescriptor>().ToList();
			var inputFiles = new List<string>();
			var optionTokens = new List<string>();
			rewritten = null;
			jsonOnly = false;
			error = null;

			for (int i = 0; i < args.Length; i++)
			{
				string arg = args[i];
				if (arg.Length < 2 || arg[0] != '-')
				{
					inputFiles.Add(arg);
					continue;
				}
				if (arg == jsonOnlyOption)
				{
					jsonOnly = true;
					continue;
				}

				bool isLong = arg.StartsWith("--", StringComparison.Ordinal);
				string body = arg[(isLong ? 2 : 1)..];
				int separator = body.IndexOfAny([':', '=']);
				string name = separator < 0 ? body : body[..separator];
				string inlineValue = separator < 0 ? null : body[(separator + 1)..];

				CommandOptionDescriptor option = isLong
					? options.Find(o => o.Name == name)
					: multiLetterAliases.TryGetValue(name, out string longName)
						? options.Find(o => o.Name == longName)
						: name.Length == 1 ? options.Find(o => o.ShortName == name[0]) : null;
				if (option is null)
				{
					error = $"Unrecognized option '{arg}'";
					return false;
				}

				optionTokens.Add("--" + option.Name);
				bool takesValue = option.Property.Type != typeof(bool) && option.Name != optionalValueOption;
				if (inlineValue is not null)
				{
					optionTokens.Add(inlineValue);
				}
				else if (takesValue)
				{
					if (i + 1 >= args.Length)
					{
						error = $"Missing value for option '{arg}'";
						return false;
					}
					optionTokens.Add(args[++i]);
				}
			}

			rewritten = [.. inputFiles, .. optionTokens];
			return true;
		}
	}
}
