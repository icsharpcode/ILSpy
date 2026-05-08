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

using ILSpy.AppEnv;
using ILSpy.Metadata;
using ILSpy.Metadata.CorTables;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

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

		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(coreLibName);
		assemblyNode.EnsureLazyChildren();
		var metadataNode = assemblyNode.Children.OfType<MetadataTreeNode>().Single();
		metadataNode.EnsureLazyChildren();
		var tablesNode = metadataNode.Children.OfType<MetadataTablesTreeNode>().Single();
		tablesNode.EnsureLazyChildren();
		var typeDefNode = tablesNode.Children.OfType<TypeDefTableTreeNode>().Single();

		vm.AssemblyTreeModel.SelectNode(typeDefNode);
		var tab = await vm.DockWorkspace.WaitForMetadataTabAsync();

		var objectRow = tab.Items.Cast<TypeDefTableTreeNode.TypeDefEntry>()
			.First(e => e.Name == "Object" && e.Namespace == "System");

		tab.RaiseRowActivated(objectRow);

		await Waiters.WaitForAsync(
			() => vm.AssemblyTreeModel.SelectedItem is TypeTreeNode tn
				&& tn.Member is global::ICSharpCode.Decompiler.TypeSystem.ITypeDefinition td
				&& td.FullName == "System.Object");
		(((TypeTreeNode)vm.AssemblyTreeModel.SelectedItem!).Member
			as global::ICSharpCode.Decompiler.TypeSystem.ITypeDefinition)!
			.FullName.Should().Be("System.Object");
	}
}

internal static class MetadataRowActivationTestExtensions
{
	public static void RaiseRowActivated(this MetadataTablePageModel page, object row) =>
		typeof(MetadataTablePageModel)
			.GetMethod("RaiseRowActivated", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
			.Invoke(page, [row]);
}
