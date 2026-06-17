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
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Bookmarks;

// Proves the sequence-point capture in CSharpLanguage.WriteCode actually populates the
// per-document debug map against real decompiled output -- the foundation of the body anchor.
[TestFixture]
public class BookmarkDebugInfoCaptureTests
{
	[AvaloniaTest]
	public async Task CSharp_decompile_captures_a_method_map_that_round_trips()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var csharp = AppComposition.Current.GetExport<LanguageService>().Languages.First(l => l.Name == "C#");

		var asm = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		Assert.That(asm, Is.Not.Null);
		var typeSystem = asm!.LoadedAssembly.GetTypeSystemOrNull();
		Assert.That(typeSystem, Is.Not.Null);
		var type = typeSystem!.MainModule.TypeDefinitions.First(t => t.FullName == "System.Linq.Enumerable");
		var method = type.Methods.First(m => m.HasBody && !m.IsConstructor);

		var output = new AvaloniaEditTextOutput();
		csharp.DecompileMethod(method, output, new DecompilationOptions(new DecompilerSettings()));

		output.MethodDebugInfos.Should().NotBeEmpty("a method with a body emits sequence points");

		uint token = (uint)MetadataTokens.GetToken(method.MetadataToken);
		var map = output.MethodDebugInfos.FirstOrDefault(m => m.Token == token);
		map.Should().NotBeNull("the decompiled method itself must be in the map");

		// The first statement (IL offset 0) maps to a real line, and that line maps back to offset 0.
		map!.TryGetLineForOffset(0, out var line).Should().BeTrue();
		int lineCount = output.GetText().Replace("\r\n", "\n").Split('\n').Length;
		line.Should().BeInRange(1, lineCount);
		map.TryGetOffsetForLine(line, out var offset).Should().BeTrue();
		offset.Should().Be(0);
	}
}
