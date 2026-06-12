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

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class MetadataDisablesLanguageSwitchingTests
{
	[AvaloniaTest]
	public async Task MetadataTablePageModel_Defaults_SupportsLanguageSwitching_False()
	{
		// Unit-level guard: every fresh MetadataTablePageModel must declare itself
		// language-agnostic in its ctor. Mirrors WPF's CoffHeaderTreeNode / DataDirectories /
		// MetadataTreeNode etc. setting tabPage.SupportsLanguageSwitching=false.
		var model = new MetadataTablePageModel();
		model.SupportsLanguageSwitching.Should().BeFalse(
			"metadata grids render PE-header / table fields straight from metadata — language choice doesn't affect what's shown");
	}

	[AvaloniaTest]
	public async Task DecompilerTabPageModel_Defaults_SupportsLanguageSwitching_True()
	{
		// Decompiler tabs ARE the place where Language / Language-Version matter. The
		// default-true is what enables the toolbar pickers on a fresh decompile tab.
		var model = new ICSharpCode.ILSpy.TextView.DecompilerTabPageModel();
		model.SupportsLanguageSwitching.Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task ContentTabPage_Mirrors_Its_Contents_SupportsLanguageSwitching_Flag()
	{
		// The toolbar pickers bind through ContentTabPage (the dockable wrapper), not the
		// inner Content viewmodel. ContentTabPage must reflect whatever the current Content
		// declared — and re-evaluate on Content swap so a single tab going decompiler →
		// metadata → decompiler over its lifetime keeps the toolbar in sync.
		var tab = new ContentTabPage();
		tab.SupportsLanguageSwitching.Should().BeTrue("default before any content is set");

		var meta = new MetadataTablePageModel();
		tab.Content = meta;
		tab.SupportsLanguageSwitching.Should().BeFalse(
			"swapping in a MetadataTablePageModel must propagate its false flag up to the wrapper");

		var dec = new ICSharpCode.ILSpy.TextView.DecompilerTabPageModel();
		tab.Content = dec;
		tab.SupportsLanguageSwitching.Should().BeTrue(
			"swapping back to a decompiler tab must restore the true flag");
	}

	[AvaloniaTest]
	public async Task LanguageComboBox_Disables_When_Metadata_Tab_Active()
	{
		// End-to-end: selecting a metadata node populates the MainTab with a
		// MetadataTablePageModel; the toolbar's LanguageComboBox.IsEnabled binding through
		// DockWorkspace.ActiveContentTabPage.SupportsLanguageSwitching must flip to false.
		// Selecting a decompilable type after that must flip it back to true.

		var (window, vm) = await TestHarness.BootAsync();

		var toolbar = await window.WaitForComponent<MainToolBar>();
		var languageCombo = toolbar.GetVisualDescendants().OfType<ComboBox>()
			.Single(c => c.Name == "LanguageComboBox");

		// Baseline: no metadata tab active yet, picker should be enabled.
		languageCombo.IsEnabled.Should().BeTrue(
			"baseline: with no metadata tab active, the LanguageComboBox is enabled");

		// Navigate to a metadata node (DOS Header is a stable choice — every PE has one).
		var coreLibName = typeof(object).Assembly.GetName().Name!;
		var dosHeader = vm.AssemblyTreeModel.FindCoreLib()
			.GetChild<MetadataTreeNode>()
			.GetChild<DosHeaderTreeNode>();

		vm.AssemblyTreeModel.SelectNode(dosHeader);
		await Waiters.WaitForAsync(
			() => vm.DockWorkspace.ActiveContentTabPage?.Content is MetadataTablePageModel,
			System.TimeSpan.FromSeconds(10));
		TestCapture.Step("metadata-tab-active");

		languageCombo.IsEnabled.Should().BeFalse(
			"with a MetadataTablePageModel as the active content, the LanguageComboBox must disable");

		// Now navigate to a decompilable type node and verify the picker re-enables.
		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			coreLibName, "System", "System.Object");
		vm.AssemblyTreeModel.SelectNode(typeNode);
		await vm.DockWorkspace.WaitForDecompiledTextAsync(System.TimeSpan.FromSeconds(30));
		TestCapture.Step("decompiler-tab-active");

		languageCombo.IsEnabled.Should().BeTrue(
			"swapping back to a decompiler tab must re-enable the LanguageComboBox");
	}
}
