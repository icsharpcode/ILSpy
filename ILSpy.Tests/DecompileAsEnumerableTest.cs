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

using AwesomeAssertions;

using Avalonia.Headless.NUnit;

using global::ILSpy.AppEnv;
using global::ILSpy.TreeNodes;
using global::ILSpy.ViewModels;
using global::ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class DecompileAsEnumerableTest
{
	[AvaloniaTest]
	public async Task Selecting_AsEnumerable_Method_Decompiles_Into_Document_View()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;

		// MainWindow.Opened triggers AssemblyTreeModel.Initialize, which queues the default
		// assembly list (System.Private.CoreLib, System, System.Linq) for async loading.
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		// Walk via the production FindNodeByPath (used by SessionSettings.ActiveTreeViewPath
		// save/restore) down to the type. First segment is the assembly short name; the helper
		// substitutes the file path AssemblyTreeNode.ToString returns.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");

		// Method-level stable identities are built via ILAmbience and impractical to author by
		// hand, so we expand the type and pick the overload by name. Enumerable has only one
		// AsEnumerable, so this is unambiguous.
		typeNode.EnsureLazyChildren();
		var methodNode = typeNode.Children.OfType<MethodTreeNode>()
			.Single(m => m.MethodDefinition.Name == "AsEnumerable");
		methodNode.MethodDefinition.IsExtensionMethod.Should().BeTrue();

		// Selecting the node routes through DockWorkspace -> the active decompiler tab.
		vm.AssemblyTreeModel.SelectedItem = methodNode;

		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		tab.Text.Should().Contain("AsEnumerable");
		tab.Text.Should().Contain("IEnumerable<TSource>");
		tab.Text.Should().Contain("return source");

		// Selecting a deeply-nested node must scroll the assembly tree so it's centered.
		methodNode.Should().Be().CenteredInView();

		window.CaptureAndShow();
	}
}
