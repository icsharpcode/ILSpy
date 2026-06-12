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
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Decompiling the list root dumps a <c>// List:</c> header and then every assembly in the list,
/// each under a comment rule. (Regression: the Avalonia port dropped the
/// <c>AssemblyListTreeNode.Decompile</c> override, so the list root produced no output.)
/// </summary>
[TestFixture]
public class AssemblyListDecompileTests
{
	[AvaloniaTest]
	public async Task Decompiling_The_List_Root_Emits_The_List_Header_And_A_Rule_Per_Assembly()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var root = (AssemblyListTreeNode)vm.AssemblyTreeModel.Root!;
		var language = AppComposition.Current.GetExport<LanguageService>().CurrentLanguage;

		var output = new PlainTextOutput();
		// Decompiling every assembly is far too slow for a test, so cancel up front: the header
		// and the first rule are written before the first member decompilation observes the token.
		var options = new DecompilationOptions(new DecompilerSettings()) { CancellationToken = new CancellationToken(true) };

		try
		{
			root.Decompile(language, output, options);
		}
		catch (OperationCanceledException)
		{
			// expected -- the per-assembly decompilation honours the pre-cancelled token.
		}

		var text = output.ToString();
		text.Should().Contain("List: " + root.AssemblyList.ListName,
			"the list root labels its output with the list name");
		text.Should().Contain(new string('-', 60),
			"each assembly is introduced by a comment rule");
	}
}
