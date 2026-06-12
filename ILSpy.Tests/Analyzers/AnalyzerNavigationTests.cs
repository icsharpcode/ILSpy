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
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

using ICSharpCode.ILSpy.Analyzers.TreeNodes;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers;

[TestFixture]
public class AnalyzerNavigationTests
{
	sealed class StubRoutedEventArgs : IPlatformRoutedEventArgs
	{
		public bool Handled { get; set; }
	}

	[AvaloniaTest]
	public async Task ActivateItem_On_An_AnalyzedTypeTreeNode_Selects_That_TypeTreeNode_In_The_Assembly_Tree()
	{
		// Double-click on an analyser result must navigate back to the entity's home in
		// the assembly tree — same UX as clicking a hyperlink in the decompiler view.

		var (_, vm) = await TestHarness.BootAsync();

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		var entity = (ICSharpCode.Decompiler.TypeSystem.ITypeDefinition)typeNode.Member!;

		var analyzed = new AnalyzedTypeTreeNode(entity, source: null);
		analyzed.ActivateItem(new StubRoutedEventArgs());
		TestCapture.Step("navigated-to-type-node");

		((object?)vm.AssemblyTreeModel.SelectedItem).Should().BeSameAs(typeNode,
			"ActivateItem must move the assembly-tree selection to the entity's tree node");
	}

	[AvaloniaTest]
	public async Task ActivateItem_On_An_AnalyzedMethodTreeNode_Selects_The_MethodTreeNode_Sibling()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.EnsureLazyChildren();
		var methodTreeNode = typeNode.Children
			.OfType<MethodTreeNode>().First(n => n.MethodDefinition.Name == "Empty");
		var method = (ICSharpCode.Decompiler.TypeSystem.IMethod)methodTreeNode.Member!;

		var analyzed = new AnalyzedMethodTreeNode(method, source: null);
		analyzed.ActivateItem(new StubRoutedEventArgs());
		TestCapture.Step("navigated-to-method-node");

		((object?)vm.AssemblyTreeModel.SelectedItem).Should().BeSameAs(methodTreeNode,
			"ActivateItem must walk down to the right method tree-node under its declaring type");
	}
}
