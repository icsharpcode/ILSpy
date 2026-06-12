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

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Visibility/enablement rules for "Save Code": a single node always qualifies (its own
/// <c>Save</c> override decides the format), several valid assemblies qualify (the solution
/// export path), and any other multi-selection does not.
/// </summary>
[TestFixture]
public class SaveCodeSolutionContextMenuTests
{
	static IContextMenuEntry Entry()
		=> AppComposition.Current.GetExport<ContextMenuEntryRegistry>().GetEntry(nameof(Resources._SaveCode));

	static TextViewContext Context(params SharpTreeNode[] nodes)
		=> new() { SelectedTreeNodes = nodes };

	[AvaloniaTest]
	public async Task Single_Node_Is_Visible()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var assembly = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		Assert.That(assembly, Is.Not.Null);

		var entry = Entry();
		entry.IsVisible(Context(assembly!)).Should().BeTrue("a single node delegates to its own Save override");
		entry.IsEnabled(Context(assembly!)).Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task Multiple_Valid_Assemblies_Are_Visible()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var assemblies = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.Where(a => a.IsLoadedAsValidAssembly)
			.GroupBy(a => a.ShortName).Select(g => g.First())
			.Take(2)
			.Select(a => (SharpTreeNode)new AssemblyTreeNode(a))
			.ToArray();
		assemblies.Should().HaveCount(2);

		Entry().IsVisible(Context(assemblies)).Should().BeTrue(
			"several valid assemblies offer the solution export path");
	}

	[AvaloniaTest]
	public async Task Mixed_Multi_Selection_Is_Hidden()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var assembly = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		var type = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		Assert.That(assembly, Is.Not.Null);
		Assert.That(type, Is.Not.Null);

		Entry().IsVisible(Context(assembly!, type!)).Should().BeFalse(
			"a mixed assembly + type selection has no well-defined Save Code action");
	}

	[AvaloniaTest]
	public async Task Empty_Selection_Is_Hidden()
	{
		await TestHarness.BootAsync(1);
		Entry().IsVisible(Context()).Should().BeFalse();
	}
}
