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

using System.Linq;
using System.Threading.Tasks;

using AvaloniaEdit.Document;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Bookmarks;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Bookmarks;

// End-to-end anchoring against real decompiled output: a statement line yields a body anchor,
// a definition line yields a token anchor, and a comment line yields nothing.
[TestFixture]
public class BookmarkAnchoringTests
{
	[AvaloniaTest]
	public async Task Decompiled_type_offers_both_body_and_token_anchors()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var csharp = AppComposition.Current.GetExport<LanguageService>().Languages.First(l => l.Name == "C#");

		var asm = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		var typeSystem = asm!.LoadedAssembly.GetTypeSystemOrNull();
		var type = typeSystem!.MainModule.TypeDefinitions.First(t => t.FullName == "System.Linq.Enumerable");

		var output = new AvaloniaEditTextOutput();
		csharp.DecompileType(type, output, new DecompilationOptions(new DecompilerSettings()));

		var document = new TextDocument(output.GetText());
		var debugInfo = new DecompiledDebugInfo(output.MethodDebugInfos);

		Bookmark? body = null, token = null;
		for (int line = 1; line <= document.LineCount; line++)
		{
			var bookmark = BookmarkAnchoring.CreateForLine(debugInfo, output.References, document, line);
			if (bookmark?.Kind == BookmarkKind.Body)
				body ??= bookmark;
			else if (bookmark?.Kind == BookmarkKind.Token)
				token ??= bookmark;
		}

		body.Should().NotBeNull("statement lines must produce a body anchor");
		token.Should().NotBeNull("definition lines must produce a token anchor");

		// Line 1 is the "// <assembly name>" comment: neither a statement nor a definition.
		BookmarkAnchoring.CreateForLine(debugInfo, output.References, document, 1).Should().BeNull();
	}
}
