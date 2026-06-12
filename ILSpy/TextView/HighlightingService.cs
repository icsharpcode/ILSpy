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
using System.IO;
using System.Xml;

using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Highlighting.Xshd;

using ICSharpCode.ILSpy.Themes;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Loads our XSHD highlighting definitions (CSharp / IL / Asm / XML) from embedded resources
	/// once and registers them with AvaloniaEdit's <see cref="HighlightingManager"/>. Lookup by
	/// file extension (".cs" / ".il" / ".asm" / ".xml") matches what <see cref="Languages.Language.FileExtension"/>
	/// returns.
	/// </summary>
	static class HighlightingService
	{
		static bool registered;
		static readonly object gate = new();

		// Embedded resource names follow the csproj RootNamespace ("ICSharpCode.ILSpy"),
		// not the file's C# namespace ("ILSpy").
		const string ResourcePrefix = "ICSharpCode.ILSpy.TextView.";

		public static void EnsureRegistered()
		{
			if (registered)
				return;
			lock (gate)
			{
				if (registered)
					return;
				Register("C#", new[] { ".cs" }, "CSharp-Mode.xshd");
				Register("IL", new[] { ".il" }, "ILAsm-Mode.xshd");
				Register("Asm", new[] { ".asm" }, "Asm-Mode.xshd");
				Register("XML", new[] { ".xml", ".xaml" }, "XML-Mode.xshd");
				registered = true;
			}
		}

		public static IHighlightingDefinition? GetByExtension(string fileExtension)
		{
			EnsureRegistered();
			return HighlightingManager.Instance.GetDefinitionByExtension(fileExtension);
		}

		static void Register(string name, string[] extensions, string resourceName)
		{
			HighlightingManager.Instance.RegisterHighlighting(name, extensions, () => Load(resourceName));
		}

		static IHighlightingDefinition Load(string resourceName)
		{
			using var stream = typeof(HighlightingService).Assembly
				.GetManifestResourceStream(ResourcePrefix + resourceName)
				?? throw new InvalidOperationException($"Highlighting resource not found: {resourceName}");
			using var reader = XmlReader.Create(stream);
			var definition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
			// Hand the freshly-loaded definition to the theme manager: it applies the active
			// theme's colours now (so the first decompile is themed) and re-themes it on every
			// later theme switch.
			ThemeManager.Current.RegisterThemableDefinition(definition);
			return definition;
		}
	}
}
