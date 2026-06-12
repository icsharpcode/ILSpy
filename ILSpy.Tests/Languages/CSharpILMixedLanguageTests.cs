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

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Languages;

/// <summary>
/// The "IL with C#" language disassembles IL with the decompiled C# source interleaved as gray
/// comments. (Regression: the language was missing entirely from the Avalonia port.)
/// </summary>
[TestFixture]
public class CSharpILMixedLanguageTests
{
	[AvaloniaTest]
	public async Task ILWithCSharp_Is_Registered_And_Interleaves_CSharp_Into_The_IL()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var languages = AppComposition.Current.GetExport<LanguageService>().Languages;

		var mixed = languages.FirstOrDefault(l => l.Name == "IL with C#");
		Assert.That(mixed, Is.Not.Null, "the 'IL with C#' language must be registered");
		var plainIL = languages.First(l => l.Name == "IL");

		var asm = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		Assert.That(asm, Is.Not.Null);
		var typeSystem = asm!.LoadedAssembly.GetTypeSystemOrNull();
		Assert.That(typeSystem, Is.Not.Null);
		var type = typeSystem!.MainModule.TypeDefinitions.First(t => t.FullName == "System.Linq.Enumerable");
		var method = type.Methods.First(m => m.HasBody);

		string Decompile(Language language)
		{
			var output = new AvaloniaEditTextOutput();
			language.DecompileMethod(method, output, new DecompilationOptions(new DecompilerSettings()));
			return output.GetText();
		}

		var mixedText = Decompile(mixed!);
		var ilText = Decompile(plainIL);

		mixedText.Should().Contain("//", "the decompiled C# is interleaved as comments");
		mixedText.Length.Should().BeGreaterThan(ilText.Length,
			"the interleaved C# source makes the mixed output longer than plain IL");
	}
}
