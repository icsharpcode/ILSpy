// Copyright (c) 2024 AlphaSierraPapa for the SharpDevelop Team
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
using System.IO;
using System.Text;

namespace ICSharpCode.ILSpy.AppEnv
{
	public class CommandLineTools
	{
		/// <summary>
		/// Decodes a command line into an array of arguments according to the CommandLineToArgvW rules.
		/// </summary>
		/// <remarks>
		/// Command line parsing rules:
		/// - 2n backslashes followed by a quotation mark produce n backslashes, and the quotation mark is considered to be the end of the argument.
		/// - (2n) + 1 backslashes followed by a quotation mark again produce n backslashes followed by a quotation mark.
		/// - n backslashes not followed by a quotation mark simply produce n backslashes.
		/// </remarks>
		public static string[] CommandLineToArgumentArray(string commandLine)
		{
			if (string.IsNullOrEmpty(commandLine))
				return Array.Empty<string>();

			var results = new List<string>();
			ParseArgv.ParseArgumentsIntoList(commandLine, results);

			return results.ToArray();
		}

		static readonly char[] charsNeedingQuoting = { ' ', '\t', '\n', '\v', '"' };

		/// <summary>
		/// Escapes a set of arguments according to the CommandLineToArgvW rules.
		/// </summary>
		/// <remarks>
		/// Command line parsing rules:
		/// - 2n backslashes followed by a quotation mark produce n backslashes, and the quotation mark is considered to be the end of the argument.
		/// - (2n) + 1 backslashes followed by a quotation mark again produce n backslashes followed by a quotation mark.
		/// - n backslashes not followed by a quotation mark simply produce n backslashes.
		/// </remarks>
		public static string ArgumentArrayToCommandLine(params string[] arguments)
		{
			if (arguments == null)
				return null;
			StringBuilder b = new StringBuilder();
			for (int i = 0; i < arguments.Length; i++)
			{
				if (i > 0)
					b.Append(' ');
				AppendArgument(b, arguments[i]);
			}
			return b.ToString();
		}

		static void AppendArgument(StringBuilder b, string arg)
		{
			if (arg == null)
			{
				return;
			}

			if (arg.Length > 0 && arg.IndexOfAny(charsNeedingQuoting) < 0)
			{
				b.Append(arg);
			}
			else
			{
				b.Append('"');
				for (int j = 0; ; j++)
				{
					int backslashCount = 0;
					while (j < arg.Length && arg[j] == '\\')
					{
						backslashCount++;
						j++;
					}
					if (j == arg.Length)
					{
						b.Append('\\', backslashCount * 2);
						break;
					}
					else if (arg[j] == '"')
					{
						b.Append('\\', backslashCount * 2 + 1);
						b.Append('"');
					}
					else
					{
						b.Append('\\', backslashCount);
						b.Append(arg[j]);
					}
				}
				b.Append('"');
			}
		}

		public static string FullyQualifyPath(string argument)
		{
			// Fully qualify the paths before passing them to another process,
			// because that process might use a different current directory.
			if (string.IsNullOrEmpty(argument) || argument[0] == '-')
				return argument;
			try
			{
				if (argument.StartsWith("@"))
				{
					return "@" + FullyQualifyPath(argument.Substring(1));
				}
				return Path.Combine(Environment.CurrentDirectory, argument);
			}
			catch (ArgumentException)
			{
				return argument;
			}
		}
	}

	// Source: https://github.com/dotnet/runtime/blob/bc9fc5a774d96f95abe0ea5c90fac48b38ed2e67/src/libraries/System.Diagnostics.Process/src/System/Diagnostics/Process.Unix.cs#L574-L606
	// Minor adaptations in GetNextArgument (ValueStringBuilder replaced), otherwise kept as identical as possible
	public static class ParseArgv
	{
		/// <summary>Parses a command-line argument string into a list of arguments.</summary>
		/// <param name="arguments">The argument string.</param>
		/// <param name="results">The list into which the component arguments should be stored.</param>
		/// <remarks>
		/// This follows the rules outlined in "Parsing C++ Command-Line Arguments" at
		/// https://msdn.microsoft.com/en-us/library/17w5ykft.aspx.
		/// </remarks>
		public static void ParseArgumentsIntoList(string arguments, List<string> results)
		{
			// Iterate through all of the characters in the argument string.
			for (int i = 0; i < arguments.Length; i++)
			{
				while (i < arguments.Length && (arguments[i] == ' ' || arguments[i] == '\t'))
					i++;

				if (i == arguments.Length)
					break;

				results.Add(GetNextArgument(arguments, ref i));
			}
		}

		private static string GetNextArgument(string arguments, ref int i)
		{
			var currentArgument = new StringBuilder();
			bool inQuotes = false;

			while (i < arguments.Length)
			{
				// From the current position, iterate through contiguous backslashes.
				int backslashCount = 0;
				while (i < arguments.Length && arguments[i] == '\\')
				{
					i++;
					backslashCount++;
				}

				if (backslashCount > 0)
				{
					if (i >= arguments.Length || arguments[i] != '"')
					{
						// Backslashes not followed by a double quote:
						// they should all be treated as literal backslashes.
						currentArgument.Append('\\', backslashCount);
					}
					else
					{
						// Backslashes followed by a double quote:
						// - Output a literal slash for each complete pair of slashes
						// - If one remains, use it to make the subsequent quote a literal.
						currentArgument.Append('\\', backslashCount / 2);
						if (backslashCount % 2 != 0)
						{
							currentArgument.Append('"');
							i++;
						}
					}

					continue;
				}

				char c = arguments[i];

				// If this is a double quote, track whether we're inside of quotes or not.
				// Anything within quotes will be treated as a single argument, even if
				// it contains spaces.
				if (c == '"')
				{
					if (inQuotes && i < arguments.Length - 1 && arguments[i + 1] == '"')
					{
						// Two consecutive double quotes inside an inQuotes region should result in a literal double quote
						// (the parser is left in the inQuotes region).
						// This behavior is not part of the spec of code:ParseArgumentsIntoList, but is compatible with CRT
						// and .NET Framework.
						currentArgument.Append('"');
						i++;
					}
					else
					{
						inQuotes = !inQuotes;
					}

					i++;
					continue;
				}

				// If this is a space/tab and we're not in quotes, we're done with the current
				// argument, it should be added to the results and then reset for the next one.
				if ((c == ' ' || c == '\t') && !inQuotes)
				{
					break;
				}

				// Nothing special; add the character to the current argument.
				currentArgument.Append(c);
				i++;
			}

			return currentArgument.ToString();
		}
	}
}
