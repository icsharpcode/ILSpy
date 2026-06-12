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

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.Metadata.CorTables;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class MetadataRowActivationTests
{
	[AvaloniaTest]
	public async Task Activating_A_TypeDef_Row_Selects_The_Matching_TypeTreeNode()
	{
		// Open the TypeDef table for CoreLib, pick the row whose Name == "Object", fire
		// RowActivated, and confirm the assembly tree's TypeTreeNode for System.Object
		// is now selected — that's the gateway to the decompiler view.

		var (_, vm) = await TestHarness.BootAsync();

		var typeDefNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<MetadataTablesTreeNode>()
			.GetChild<TypeDefTableTreeNode>();

		vm.AssemblyTreeModel.SelectNode(typeDefNode);
		var tab = await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("typedef-grid");

		var objectRow = tab.Items.Cast<TypeDefTableTreeNode.TypeDefEntry>()
			.First(e => e.Name == "Object" && e.Namespace == "System");

		tab.RaiseRowActivated(objectRow);
		TestCapture.Step("activated-object-row");

		await Waiters.WaitForAsync(
			() => vm.AssemblyTreeModel.SelectedItem is TypeTreeNode tn
				&& tn.Member is ICSharpCode.Decompiler.TypeSystem.ITypeDefinition td
				&& td.FullName == "System.Object");
		(((TypeTreeNode)vm.AssemblyTreeModel.SelectedItem!).Member
			as ICSharpCode.Decompiler.TypeSystem.ITypeDefinition)!
			.FullName.Should().Be("System.Object");
	}

	[AvaloniaTest]
	public async Task Activating_A_Row_With_OpenInNewTab_Spawns_A_Fresh_Decompiler_Tab()
	{
		// Shift+double-click on a metadata row should NOT replace the active tab — it should
		// open a brand-new decompiler tab containing the row's entity. The single-tab-reuse
		// path stays put.
		var (_, vm) = await TestHarness.BootAsync();

		var typeDefNode = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<MetadataTablesTreeNode>()
			.GetChild<TypeDefTableTreeNode>();

		vm.AssemblyTreeModel.SelectNode(typeDefNode);
		var metadataTab = await vm.DockWorkspace.WaitForMetadataTabAsync();
		TestCapture.Step("typedef-grid");
		var objectRow = metadataTab.Items.Cast<TypeDefTableTreeNode.TypeDefEntry>()
			.First(e => e.Name == "Object" && e.Namespace == "System");

		var documents = ((ILSpyDockFactory)vm.DockWorkspace.Factory).Documents!;
		var initialCount = documents.VisibleDockables?.Count ?? 0;

		metadataTab.RaiseRowActivated(objectRow, openInNewTab: true);

		await Waiters.WaitForAsync(
			() => (documents.VisibleDockables?.Count ?? 0) > initialCount);
		var decompiledTab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("new-decompiler-tab");
		decompiledTab.Text.Should().Contain("System.Object");
	}

	[AvaloniaTest]
	public async Task DecompileMetadataRowInNewTab_Entry_Is_Registered()
	{
		// Right-click on a metadata-grid row shows a "Decompile to new tab" entry alongside
		// Go-to-token / Show-in-metadata. Verifies the [ExportContextMenuEntry] is discovered.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();

		registry.Entries.Should().Contain(
			e => e.Value is DecompileMetadataRowInNewTabCommand);
	}
}

internal static class MetadataRowActivationTestExtensions
{
	public static void RaiseRowActivated(this MetadataTablePageModel page, object row, bool openInNewTab = false) =>
		typeof(MetadataTablePageModel)
			.GetMethod("RaiseRowActivated", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
			.Invoke(page, [row, openInNewTab]);
}
