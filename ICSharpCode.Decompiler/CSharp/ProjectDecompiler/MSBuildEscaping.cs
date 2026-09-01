// Copyright (c) 2026 Siegfried Pammer
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
using System.Globalization;
using System.Text;

#nullable enable

namespace ICSharpCode.Decompiler.CSharp.ProjectDecompiler
{
	/// <summary>
	/// Quotes text that goes into a generated project file so MSBuild reads it back as the literal
	/// it was, and not as syntax.
	/// </summary>
	/// <remarks>
	/// An exported project carries a lot of text the decompiled assembly chose: resource names in
	/// LogicalName metadata, the assembly name, the names of referenced assemblies. MSBuild expands
	/// property references, item references and property functions inside attribute and element
	/// values, so an assembly whose resource is named
	/// "$([System.IO.File]::ReadAllText('/etc/passwd'))" gets that evaluated the moment the exported
	/// project is opened or built. Assemblies get decompiled precisely because nobody trusts them,
	/// so none of their text may reach MSBuild unquoted.
	/// <para>
	/// The escape form is MSBuild's own: a percent sign and the character's two hex digits, which
	/// MSBuild resolves back to the character before anything else looks at the value. Quoting text
	/// that has nothing to quote returns it unchanged, so this is safe to apply to any value that is
	/// not meant to be MSBuild syntax.
	/// </para>
	/// </remarks>
	static class MSBuildEscaping
	{
		/// <summary>
		/// The characters MSBuild treats as syntax, matching the set its own escaping uses.
		/// </summary>
		static readonly char[] specialCharacters = { '%', '*', '?', '@', '$', '(', ')', ';', '\'' };

		public static string Escape(string? value)
		{
			if (string.IsNullOrEmpty(value))
				return string.Empty;
			if (value!.IndexOfAny(specialCharacters) < 0)
				return value;
			StringBuilder b = new StringBuilder(value.Length + 8);
			foreach (char c in value)
			{
				// '%' is in the set, so escaping in one pass cannot produce a sequence that gets
				// unescaped twice.
				if (Array.IndexOf(specialCharacters, c) >= 0)
					b.Append('%').Append(((int)c).ToString("X2", CultureInfo.InvariantCulture));
				else
					b.Append(c);
			}
			return b.ToString();
		}
	}
}
