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
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Visibility/enablement of the dedicated "Export Project/Solution..." entry (context menu + File
/// menu): one or more valid assemblies qualify; any other selection does not. The quick Save Code
/// entry is unaffected.
/// </summary>
[TestFixture]
public class ExportProjectEntryTests
{
	static IContextMenuEntry Entry()
		=> AppComposition.Current.GetExport<ContextMenuEntryRegistry>().GetEntry(nameof(Resources.ExportProjectSolution));

	static TextViewContext Context(params SharpTreeNode[] nodes)
		=> new() { SelectedTreeNodes = nodes };

	[AvaloniaTest]
	public async Task Single_Assembly_Is_Visible()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var assembly = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		Assert.That(assembly, Is.Not.Null);

		Entry().IsVisible(Context(assembly!)).Should().BeTrue("a single valid assembly offers project export");
	}

	[AvaloniaTest]
	public async Task Multiple_Assemblies_Are_Visible()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var assemblies = vm.AssemblyTreeModel.AssemblyList!.GetAssemblies()
			.Where(a => a.IsLoadedAsValidAssembly)
			.GroupBy(a => a.ShortName).Select(g => g.First())
			.Take(2)
			.Select(a => (SharpTreeNode)new AssemblyTreeNode(a))
			.ToArray();
		assemblies.Should().HaveCount(2);

		Entry().IsVisible(Context(assemblies)).Should().BeTrue("several valid assemblies offer solution export");
	}

	[AvaloniaTest]
	public async Task Mixed_Selection_Is_Hidden()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var assembly = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		var type = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		Assert.That(assembly, Is.Not.Null);
		Assert.That(type, Is.Not.Null);

		Entry().IsVisible(Context(assembly!, type!)).Should().BeFalse(
			"only assembly selections map onto a project/solution export");
	}

	[AvaloniaTest]
	public async Task Empty_Selection_Is_Hidden()
	{
		await TestHarness.BootAsync(1);
		Entry().IsVisible(Context()).Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task Main_Menu_Command_Enables_Only_For_Assembly_Selections()
	{
		var (_, vm) = await TestHarness.BootAsync(3);
		var command = AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources.ExportProjectSolution));

		command.CanExecute(null).Should().BeFalse("nothing selected at startup");

		var assembly = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		Assert.That(assembly, Is.Not.Null);
		vm.AssemblyTreeModel.SelectNode(assembly);

		command.CanExecute(null).Should().BeTrue("a selected assembly enables Export Project/Solution");
	}
}
