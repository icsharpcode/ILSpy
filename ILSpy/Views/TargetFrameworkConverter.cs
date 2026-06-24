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

using System;
using System.Collections.Generic;

using ICSharpCode.ILSpy.Properties;

using NuGet.Frameworks;

namespace ICSharpCode.ILSpy.Views
{
	/// <summary>
	/// Bridges the short target-framework monikers users type (net48, net6.0, netstandard2.0)
	/// and the long FrameworkName form ILSpy resolves references with
	/// (.NETFramework,Version=v4.8). Parsing/validation goes through NuGet so the dialog accepts
	/// exactly what the ecosystem considers a valid TFM.
	/// </summary>
	internal static class TargetFrameworkConverter
	{
		// NuGet's parser is lenient and coerces arbitrary text into a generic framework, so accept
		// only identifiers ILSpy's resolver actually understands (NuGet keeps .NET 5+ as .NETCoreApp).
		static readonly HashSet<string> SupportedIdentifiers = new(StringComparer.OrdinalIgnoreCase) {
			FrameworkConstants.FrameworkIdentifiers.Net,         // .NETFramework
			FrameworkConstants.FrameworkIdentifiers.NetCoreApp,  // .NETCoreApp (incl. net5.0+)
			FrameworkConstants.FrameworkIdentifiers.NetStandard, // .NETStandard
		};

		/// <summary>
		/// Parses a short TFM (e.g. "net48") or an already-long FrameworkName (e.g.
		/// ".NETFramework,Version=v4.8") into the long FrameworkName form. Returns false with a
		/// human-readable <paramref name="error"/> when the input is blank or not a recognised
		/// framework.
		/// </summary>
		public static bool TryParseToFrameworkName(string? input, out string? frameworkName, out string? error)
		{
			frameworkName = null;
			error = null;
			if (string.IsNullOrWhiteSpace(input))
			{
				error = Resources.InvalidTargetFramework;
				return false;
			}

			var framework = TryParse(input.Trim());
			if (framework == null || framework.IsUnsupported || framework.IsAgnostic || framework.IsAny
				|| !SupportedIdentifiers.Contains(framework.Framework))
			{
				error = Resources.InvalidTargetFramework;
				return false;
			}

			frameworkName = framework.DotNetFrameworkName;
			return true;
		}

		/// <summary>
		/// Converts a long FrameworkName to its short folder name for display (e.g.
		/// ".NETFramework,Version=v4.8" -> "net48"), or returns null if it cannot be parsed.
		/// </summary>
		public static string? ToShortFolderName(string? frameworkName)
		{
			if (string.IsNullOrWhiteSpace(frameworkName))
				return null;
			try
			{
				var framework = NuGetFramework.ParseFrameworkName(frameworkName, DefaultFrameworkNameProvider.Instance);
				if (framework.IsUnsupported)
					return null;
				return framework.GetShortFolderName();
			}
			catch (ArgumentException)
			{
				return null;
			}
		}

		// Accepts both the short folder form ("net48") and the long FrameworkName form
		// (".NETFramework,Version=v4.8"); NuGet's two parsers cover one each.
		static NuGetFramework? TryParse(string input)
		{
			try
			{
				var framework = NuGetFramework.Parse(input);
				if (!framework.IsUnsupported)
					return framework;
			}
			catch (ArgumentException)
			{
				// fall through to the FrameworkName parser
			}
			try
			{
				return NuGetFramework.ParseFrameworkName(input, DefaultFrameworkNameProvider.Instance);
			}
			catch (ArgumentException)
			{
				return null;
			}
		}
	}
}
