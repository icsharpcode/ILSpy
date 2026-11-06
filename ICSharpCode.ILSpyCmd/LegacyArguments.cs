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

namespace ICSharpCode.ILSpyCmd
{
	/// <summary>
	/// Translates the verb-less command line into the form ConsoleAppFramework parses. Its parser
	/// reads whole tokens: it knows no ':' or '=' delimiters, keeps only the last occurrence of a
	/// repeated option (several values go into one comma-separated token), and the trailing
	/// "params" argument that receives the input files takes every token after the first one. The
	/// verb-less command line attaches values with ':' or '=', accumulates repeated options and
	/// allows options and input files in any order. The rewrite splits delimited values off,
	/// joins repeated options, and emits the options first and the input files last.
	/// </summary>
	static class LegacyArguments
	{
		/// <summary>
		/// The options that take a value, with every spelling; the first spelling is the one
		/// emitted. Any other token starting with a dash is a flag, or an unknown option that the
		/// parser reports.
		/// </summary>
		static readonly (string[] Spellings, bool Repeatable)[] valuedOptions = {
			(["--outputdir", "-o"], false),
			(["--type", "-t"], false),
			(["--member", "-m"], false),
#if DEBUG
			(["--after-transform"], false),
#endif
			(["--list", "-l"], true),
			(["--resource"], false),
			(["--dump-table"], false),
			(["--languageversion", "-lv"], false),
			(["--ilspy-settingsfile"], false),
			(["--decompiler-setting", "-ds"], true),
			(["--referencepath", "-r"], true),
			(["--bundle-entry"], false),
			(["--generate-diagrammer-include"], false),
			(["--generate-diagrammer-exclude"], false),
			(["--generate-diagrammer-docs"], false),
			(["--generate-diagrammer-strip-namespaces"], true),
		};

		/// <summary>
		/// Takes a value only when one is attached with ':' or '='; a bare token after it is an
		/// input file. The parser has no such option kind, so the bare form is handed over with
		/// an empty value.
		/// </summary>
		static readonly string[] optionalValueSpellings = ["--use-varnames-from-pdb", "-usepdb"];

		public static bool TryRewrite(string[] args, out string[] rewritten, out string error)
		{
			var options = new List<string>();
			var repeated = new Dictionary<string, List<string>>();
			var inputFiles = new List<string>();
			rewritten = null;
			error = null;

			for (int i = 0; i < args.Length; i++)
			{
				string arg = args[i];
				if (arg.Length < 2 || arg[0] != '-')
				{
					inputFiles.Add(arg);
					continue;
				}
				if (arg == "--")
				{
					// The parser would stop reading options here; the verb-less command line
					// has no such token.
					error = "Unrecognized option '--'";
					return false;
				}

				int separator = arg.IndexOfAny([':', '=']);
				string name = separator < 0 ? arg : arg[..separator];
				string inlineValue = separator < 0 ? null : arg[(separator + 1)..];

				if (Array.Exists(optionalValueSpellings, s => s.Equals(name, StringComparison.OrdinalIgnoreCase)))
				{
					options.Add(optionalValueSpellings[0]);
					options.Add(inlineValue ?? "");
					continue;
				}

				var valued = Array.Find(valuedOptions, o => Array.Exists(o.Spellings, s => s.Equals(name, StringComparison.OrdinalIgnoreCase)));
				if (valued.Spellings is null || (inlineValue is null && i + 1 >= args.Length))
				{
					// a flag, an unknown option or a missing value: the parser reports those
					options.Add(arg);
					continue;
				}

				string value = inlineValue ?? args[++i];
				if (valued.Repeatable)
				{
					if (!repeated.TryGetValue(valued.Spellings[0], out List<string> values))
						repeated[valued.Spellings[0]] = values = [];
					values.Add(value);
				}
				else
				{
					options.Add(valued.Spellings[0]);
					options.Add(value);
				}
			}

			foreach (var (name, values) in repeated)
			{
				options.Add(name);
				// shortcut: a value containing ',' is split by the parser; upgrade if it ever accumulates repeated options itself
				options.Add(string.Join(',', values));
			}

			rewritten = [.. options, .. inputFiles];
			return true;
		}
	}
}
