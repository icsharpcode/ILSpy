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
using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class HideEmptyMetadataTablesUiBindingTests
{
	[AvaloniaTest]
	public async Task Toggling_DisplaySettings_HideEmptyMetadataTables_Affects_The_Tree()
	{
		// The Display Settings "Hide empty metadata tables" checkbox binds to
		// DisplaySettings.HideEmptyMetadataTables. The tree must read from the SAME source —
		// otherwise the checkbox is dead UI. Regression: prior to this fix the tree read
		// SessionSettings.HideEmptyMetadataTables (an orphaned parallel setting).

		var settings = AppComposition.Current.GetExport<SettingsService>();
		var (_, vm) = await TestHarness.BootAsync();

		var coreLibName = typeof(object).Assembly.GetName().Name!;

		try
		{
			// Disable via the same property the checkbox writes to.
			settings.DisplaySettings.HideEmptyMetadataTables = false;
			var withAll = CountVisibleTables(vm, coreLibName);
			TestCapture.Step("hide-empty-off");

			settings.DisplaySettings.HideEmptyMetadataTables = true;
			var withoutEmpty = CountVisibleTables(vm, coreLibName);
			TestCapture.Step("hide-empty-on");

			withAll.Should().BeGreaterThan(withoutEmpty,
				"flipping the Display-Settings flag must cause additional empty tables to surface in the tree");
		}
		finally
		{
			settings.DisplaySettings.HideEmptyMetadataTables = true;
		}
	}

	static int CountVisibleTables(MainWindowViewModel vm, string assemblyName)
	{
		// Each test phase needs a fresh tree fragment because LoadChildren is idempotent —
		// the children dictionary is captured the first time the node is expanded, not on
		// every read. Re-finding the assembly node + re-walking is the lightweight way to
		// re-trigger the metadata-table enumeration against the current setting.
		var assemblyNode = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>(assemblyName);
		assemblyNode.Children.Clear();
		assemblyNode.LazyLoading = true;
		var tables = assemblyNode
			.GetChild<MetadataTreeNode>()
			.GetChild<MetadataTablesTreeNode>();
		tables.EnsureLazyChildren();
		return tables.Children.OfType<MetadataTableTreeNode>().Count();
	}
}
