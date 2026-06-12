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
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AvaloniaEdit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Copying the editor selection produces a syntax-coloured HTML fragment (so a paste into an
/// HTML-aware target keeps the highlighting). AvaloniaEdit's built-in copy is plain text only.
/// </summary>
[TestFixture]
public class HtmlClipboardCopyTests
{
	[AvaloniaTest]
	public async Task Copy_Includes_ILSpy_Semantic_Highlighting_Not_Just_Syntax()
	{
		var (window, vm) = await TestHarness.BootAsync(3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		Assert.That(typeNode, Is.Not.Null);
		typeNode!.IsExpanded = true;
		var methodNode = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "AsEnumerable");
		vm.AssemblyTreeModel.SelectNode(methodNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var view = await window.WaitForComponent<DecompilerTextView>();
		await view.WaitForComponent<TextEditor>();
		view.Editor.SelectAll();

		var semanticModel = view.SemanticHighlightingModel;
		Assert.That(semanticModel, Is.Not.Null, "the decompiled output has a semantic highlighting model");

		var withSemantic = HtmlClipboardCopy.CreateHtmlFragmentFromSelection(view.Editor, semanticModel);
		var syntaxOnly = HtmlClipboardCopy.CreateHtmlFragmentFromSelection(view.Editor, null);

		withSemantic.Should().NotBeNullOrEmpty();
		withSemantic.Should().Contain("<span", "the HTML fragment carries colours");
		withSemantic.Should().NotBe(syntaxOnly,
			"the semantic RichTextModel contributes the decompiler colours that the xshd highlighter alone doesn't");
	}

	[AvaloniaTest]
	public async Task No_Selection_Yields_No_Fragment()
	{
		var (window, vm) = await TestHarness.BootAsync(3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode!.IsExpanded = true;
		var methodNode = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "AsEnumerable");
		vm.AssemblyTreeModel.SelectNode(methodNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var view = await window.WaitForComponent<DecompilerTextView>();
		await view.WaitForComponent<TextEditor>();
		view.Editor.Select(0, 0);

		HtmlClipboardCopy.CreateHtmlFragmentFromSelection(view.Editor).Should().BeNull();
		HtmlClipboardCopy.Copy(view.Editor).Should().BeFalse("nothing to copy with an empty selection");
	}
}
