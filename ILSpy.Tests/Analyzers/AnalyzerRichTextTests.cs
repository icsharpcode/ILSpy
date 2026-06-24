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
using Avalonia.Media;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;

using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.Analyzers.TreeNodes;
using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Controls.TreeView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers;

[TestFixture]
public class AnalyzerRichTextTests
{
	static IMethod GetEnumerableMethod(AssemblyTreeModel model, string name)
	{
		var typeNode = model.FindNode<TypeTreeNode>("System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.EnsureLazyChildren();
		return (IMethod)typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == name).Member!;
	}

	[AvaloniaTest]
	public async Task AnalyzerEntityNode_Produces_RichText_With_Bold_Type_Names()
	{
		// Issue #2164: analyzer-pane signatures colour the C# syntax and render type-name spans
		// bold so the key info stands out in long lists. The node exposes this via IRichTextNode.
		var (_, vm) = await TestHarness.BootAsync();
		var method = GetEnumerableMethod(vm.AssemblyTreeModel, "Empty");

		var node = new AnalyzedMethodTreeNode(method, source: null);

		var richNode = node as IRichTextNode;
		richNode.Should().NotBeNull("analyzer entity nodes opt into rich rendering via IRichTextNode");
		var rich = richNode!.CreateRichText();
		rich.Should().NotBeNull();

		// The colored signature text and the plain Text used for search/copy stay identical.
		rich!.Text.Should().Be(node.Text.ToString());

		// At least one span (the declaring/return type, e.g. Enumerable / IEnumerable) is bold.
		var hasBoldSpan = rich.ToRichTextModel()
			.GetHighlightedSections(0, rich.Length)
			.Any(s => s.Color?.FontWeight == FontWeight.Bold);
		hasBoldSpan.Should().BeTrue("type names in the analyzer signature must be emboldened (#2164)");
	}

	[AvaloniaTest]
	public async Task RichText_Does_Not_Change_The_Plain_Text_Contract()
	{
		// The string Text (and ToString) must stay the plain signature so search, copy and
		// keyboard navigation keep working unchanged.
		var (_, vm) = await TestHarness.BootAsync();
		var method = GetEnumerableMethod(vm.AssemblyTreeModel, "Empty");

		var node = new AnalyzedMethodTreeNode(method, source: null);

		node.Text.Should().BeOfType<string>();
		node.ToString().Should().Be(node.Text.ToString());
		node.ToString().Should().Contain("Empty").And.Contain("Enumerable");
	}
}
