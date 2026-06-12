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

using System.Collections.Generic;

using Avalonia.Media;

namespace ICSharpCode.ILSpy.Themes
{
	/// <summary>
	/// Per-theme syntax-colour palettes, keyed by the named <c>&lt;Color&gt;</c> from the .xshd.
	/// Only the dark C# palette is hand-authored today (the light scheme stays the .xshd default);
	/// any token without an entry falls back to the algorithmic dark conversion in
	/// <see cref="ThemeManager.GetColorForDarkTheme"/>. These values are a first cut and meant to
	/// be tuned -- they live here so the palette is one easy-to-edit place.
	/// </summary>
	public static class SyntaxColorPalettes
	{
		static Color C(string hex) => Color.Parse(hex);

		static SyntaxColor Fg(string hex) => new() { Foreground = C(hex) };
		static SyntaxColor Bold(string hex) => new() { Foreground = C(hex), FontWeight = FontWeight.Bold };
		static SyntaxColor Italic(string hex) => new() { Foreground = C(hex), FontStyle = FontStyle.Italic };

		// Token families on a #1E1E1E editor background:
		//   keyword blue  #569CD6   control-flow  #C586C0   type teal  #4EC9B0
		//   method yellow #DCDCAA   local/param   #9CDCFE   string     #CE9178
		//   number green  #B5CEA8   comment       #6A9955
		public static IReadOnlyDictionary<string, SyntaxColor> CSharpDark { get; } = new Dictionary<string, SyntaxColor> {
			["Comment"] = Fg("#6A9955"),
			["String"] = Fg("#CE9178"),
			["StringInterpolation"] = Fg("#D7BA7D"),
			["Char"] = Fg("#CE9178"),
			["Preprocessor"] = Fg("#9B9B9B"),
			["Punctuation"] = Fg("#D4D4D4"),
			["NumberLiteral"] = Fg("#B5CEA8"),

			["Keywords"] = Fg("#569CD6"),
			["ValueTypeKeywords"] = Fg("#569CD6"),
			["ReferenceTypeKeywords"] = Fg("#569CD6"),
			["GotoKeywords"] = Fg("#C586C0"),
			["QueryKeywords"] = Fg("#C586C0"),
			["ExceptionKeywords"] = Fg("#C586C0"),
			["CheckedKeyword"] = Fg("#569CD6"),
			["UnsafeKeywords"] = Fg("#569CD6"),
			["OperatorKeywords"] = Fg("#569CD6"),
			["ParameterModifiers"] = Fg("#569CD6"),
			["Modifiers"] = Fg("#569CD6"),
			["Visibility"] = Fg("#569CD6"),
			["NamespaceKeywords"] = Fg("#569CD6"),
			["GetSetAddRemove"] = Fg("#569CD6"),
			["TrueFalse"] = Fg("#569CD6"),
			["TypeKeywords"] = Fg("#569CD6"),
			["AttributeKeywords"] = Fg("#C586C0"),
			["ThisOrBaseReference"] = Fg("#569CD6"),
			["NullOrValueKeywords"] = Fg("#569CD6"),

			["ReferenceTypes"] = Fg("#4EC9B0"),
			["InterfaceTypes"] = Fg("#4EC9B0"),
			["TypeParameters"] = Fg("#4EC9B0"),
			["DelegateTypes"] = Fg("#4EC9B0"),
			["ValueTypes"] = Fg("#4EC9B0"),
			["EnumTypes"] = Fg("#4EC9B0"),

			["MethodDeclaration"] = Bold("#DCDCAA"),
			["MethodCall"] = Bold("#DCDCAA"),
			["FieldDeclaration"] = Italic("#9CDCFE"),
			["FieldAccess"] = Italic("#9CDCFE"),
			["Variable"] = Fg("#9CDCFE"),
			["Parameter"] = Fg("#9CDCFE"),

			["InactiveCode"] = Fg("#808080"),
			["SemanticError"] = Fg("#F44747"),
		};
	}
}
