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

using ICSharpCode.Decompiler.TypeSystem;

using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.Analyzers.TreeNodes;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers;

[TestFixture]
public class AnalyzedMethodTreeNodeTests
{
	[AvaloniaTest]
	public async Task Wrapping_A_Method_Shows_It_With_Its_Declaring_Type_Prefix()
	{
		// AnalyzedMethodTreeNode renders the analysed method with `ShowDeclaringType` +
		// `UseFullyQualifiedEntityNames` so users see "System.Linq.Enumerable.Empty<TSource>"
		// in the pane rather than the bare method name. An optional `prefix` argument lets
		// accessor / event-helper nodes prepend a tag without touching the entity string.

		var (_, vm) = await TestHarness.BootAsync();

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.EnsureLazyChildren();
		var emptyMethod = (IMethod)typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Empty").Member!;

		var node = new AnalyzedMethodTreeNode(emptyMethod, source: null);
		TestCapture.Step("before-method-text");
		node.Member.Should().BeSameAs(emptyMethod);
		node.Text.ToString().Should().Contain("Empty");
		node.Text.ToString().Should().Contain("Enumerable",
			"AnalyzedMethodTreeNode must show the declaring type so the user can disambiguate "
			+ "identically-named methods across the analyser pane");
	}

	[AvaloniaTest]
	public async Task LoadChildren_On_A_Method_Materialises_Uses_And_Used_By_Headers()
	{
		// Expanding an analysed method materialises one AnalyzerSearchTreeNode per analyser
		// whose Show(method) returns true. The two canonical entries for any callable method
		// are "Uses" (MethodUsesAnalyzer, body scanner) and "Used By" (MethodUsedByAnalyzer,
		// call-site scanner).

		var (_, vm) = await TestHarness.BootAsync();

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.EnsureLazyChildren();
		var firstMethod = (IMethod)typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "AsEnumerable").Member!;

		var node = new AnalyzedMethodTreeNode(firstMethod, source: null);
		node.EnsureLazyChildren();

		var headers = node.Children.OfType<AnalyzerSearchTreeNode>()
			.Select(c => c.AnalyzerHeader)
			.ToArray();

		TestCapture.Step("before-uses-and-used-by-headers");
		headers.Should().Contain("Used By",
			"MethodUsedByAnalyzer is the headline reverse-lookup for any concrete method");
		headers.Should().Contain("Uses",
			"MethodUsesAnalyzer scans the body for callees and field/type touches");
	}
}
