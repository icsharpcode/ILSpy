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

using System;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ILSpy.AppEnv;
using ILSpy.Commands;
using ILSpy.Docking;
using ILSpy.Navigation;
using ILSpy.TextView;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;
using ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class HelpCommandTests
{
	[AvaloniaTest]
	public async Task About_Command_Opens_New_Tab_With_Version_Info()
	{
		// Help → About is exported as a [ExportMainMenuCommand] under ParentMenuID="_Help".
		// Executing it must open a new document tab in the dock workspace whose title is the
		// localized "About" string and whose body contains the ILSpy version line plus the
		// embedded ILSpy-about-page blurb (MIT License is the easiest stable phrase to match).

		// Arrange — boot the window so the dock workspace + document dock are realised.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		// wait for assemblies to load
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var aboutCmd = registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources._About))
			.CreateExport().Value;

		var documents = ((ILSpyDockFactory)vm.DockWorkspace.Factory).Documents!;
		var initialTabCount = documents.VisibleDockables?.Count ?? 0;

		// Act — fire the About command.
		aboutCmd.Execute(null);
		// execute aboutCmd

		// Assert — a new DecompilerTabPageModel landed in the document dock, titled "About",
		// containing the version line and the MIT License mention from the embedded blurb.
		await Waiters.WaitForAsync(
			() => (documents.VisibleDockables?.Count ?? 0) > initialTabCount
				&& documents.ActiveDockable is DecompilerTabPageModel { Text.Length: > 0 });

		var aboutTab = (DecompilerTabPageModel)documents.ActiveDockable!;
		aboutTab.Title.Should().Be(Resources.About);
		aboutTab.Text.Should().Contain(Resources.ILSpyVersion);
		aboutTab.Text.Should().Contain("MIT License");
	}

	[AvaloniaTest]
	public async Task About_Page_MIT_License_Link_Opens_License_Tab_On_Click()
	{
		// "MIT License" inside the About blurb must be rendered as an AvaloniaEdit hyperlink
		// pointing at "resource:license.txt", and activating it must open the embedded LICENSE
		// text in a new tab. Tests the full pipeline: the tab carries a custom element
		// generator (matching the WPF host's LinkElementGenerator pattern), and the tab's
		// OpenUriRequested handler resolves the resource: URI to a new tab.

		// Arrange — boot, open the About page.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		// wait for assemblies to load
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var aboutCmd = registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources._About))
			.CreateExport().Value;
		var documents = ((ILSpyDockFactory)vm.DockWorkspace.Factory).Documents!;
		// execute aboutCmd
		aboutCmd.Execute(null);
		// wait for the predicate
		await Waiters.WaitForAsync(
			() => documents.ActiveDockable is DecompilerTabPageModel { Text.Length: > 0 });
		var aboutTab = (DecompilerTabPageModel)documents.ActiveDockable!;

		// Assert (mid-test) — the tab carries the custom hyperlink generators that
		// DecompilerTextView installs alongside the document.
		aboutTab.CustomElementGenerators.Should().NotBeNull();
		aboutTab.CustomElementGenerators!.Should().NotBeEmpty(
			"the About page must contribute LinkElementGenerator(s) for MIT License + third-party notices");

		var beforeCount = documents.VisibleDockables?.Count ?? 0;

		// Act — fire the same OpenUri routed event that AvaloniaEdit would raise on click.
		aboutTab.RaiseOpenUriRequested(new Uri("resource:license.txt"))
			.Should().BeTrue("the About tab must claim the resource: URI as handled");

		// Assert — a new tab opens with the license body.
		await Waiters.WaitForAsync(
			() => (documents.VisibleDockables?.Count ?? 0) > beforeCount
				&& documents.ActiveDockable is DecompilerTabPageModel licenseTab
				&& !ReferenceEquals(licenseTab, aboutTab)
				&& licenseTab.Text.Length > 0);
		var licenseTab = (DecompilerTabPageModel)documents.ActiveDockable!;
		licenseTab.Text.Should().Contain("Permission is hereby granted");
	}

	[AvaloniaTest]
	public async Task Decompiler_Editor_Activates_Hyperlinks_Without_Ctrl_Modifier()
	{
		// AvaloniaEdit defaults RequireControlModifierForHyperlinkClick=true on its
		// TextEditorOptions; decompiler output uses hyperlinks for in-app navigation, so a
		// plain click is the expected affordance. Verifies the option is flipped on the
		// realised editor — the value AvaloniaEdit's LinkElementGenerator (and our own
		// ResourceLinkGenerator) both consult when constructing VisualLineLinkText.

		// Arrange — boot, select a node so the decompiler tab is realised + the editor inside
		// it is actually constructed (the view is data-templated lazily on first activation).
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		// wait for assemblies to load
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		var node = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		// select node
		vm.AssemblyTreeModel.SelectNode(node);
		// wait for decompile to finish
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		// wait for the predicate
		await Waiters.WaitForAsync(
			() => window.GetVisualDescendants().OfType<DecompilerTextView>().Any());

		// Act — locate the editor inside the realised DecompilerTextView.
		var view = window.GetVisualDescendants().OfType<DecompilerTextView>().First();
		var editor = view.GetVisualDescendants().OfType<AvaloniaEdit.TextEditor>().Single();

		// Assert — the editor's hyperlink-click option is off.
		editor.Options.RequireControlModifierForHyperlinkClick.Should().BeFalse(
			"hyperlinks in the decompiler view must activate on a plain click");
	}

	[AvaloniaTest]
	public async Task About_Page_Lands_On_Back_History_And_Round_Trips_Through_Navigation()
	{
		// Opening the About page records a StaticPageEntry on the back stack and the About
		// tab is marked IsStaticContent so that subsequent tree-node selections route to
		// (or open) a real decompiler tab without overwriting the About content. Pressing
		// Back returns to the previous tree-node selection (re-activating its decompiler tab),
		// pressing Forward re-activates the About tab with content intact.

		// Arrange — boot, select a tree node so there's a tree-node entry to compare against.
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		// wait for assemblies to load
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		// expand typeNode
		typeNode.IsExpanded = true;
		var method = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "AsEnumerable");
		// select method
		vm.AssemblyTreeModel.SelectNode(method);
		// wait for decompile to finish
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		var decompilerTab = vm.DockWorkspace.ActiveDecompilerTab!;
		var methodText = decompilerTab.Text;

		// NavigationHistory collapses entries within 0.5s — wait past the window so opening
		// About records its own entry instead of replacing the tree-node entry.
		await Task.Delay(600);

		// Act 1 — open the About page.
		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var aboutCmd = registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources._About))
			.CreateExport().Value;
		var documents = ((ILSpyDockFactory)vm.DockWorkspace.Factory).Documents!;
		// execute aboutCmd
		aboutCmd.Execute(null);
		// wait for the predicate
		await Waiters.WaitForAsync(
			() => documents.ActiveDockable is DecompilerTabPageModel { IsStaticContent: true });
		var aboutTab = (DecompilerTabPageModel)documents.ActiveDockable!;
		var aboutText = aboutTab.Text;

		// Assert (mid-test) — the back stack now ends with the tree-node entry; the About
		// page is the current entry (so Back goes there).
		vm.DockWorkspace.BackHistory.Should().NotBeEmpty();
		vm.DockWorkspace.BackHistory.Last().Should().BeOfType<TreeNodeEntry>();
		var lastBack = (TreeNodeEntry)vm.DockWorkspace.BackHistory.Last();
		lastBack.Node.GetType().Should().Be(typeof(MethodTreeNode));

		// Act 2 — press Back from the About page.
		vm.DockWorkspace.NavigateBackCommand.CanExecute(null).Should().BeTrue();
		// execute vm.DockWorkspace.NavigateBackCommand
		vm.DockWorkspace.NavigateBackCommand.Execute(null);
		// wait for the predicate
		await Waiters.WaitForAsync(
			() => ReferenceEquals(documents.ActiveDockable, decompilerTab));

		// Assert — the decompiler tab is active again; About tab still exists with content
		// preserved.
		ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, method).Should().BeTrue();
		decompilerTab.Text.Should().Be(methodText);
		documents.VisibleDockables!.Should().Contain(aboutTab);
		aboutTab.Text.Should().Be(aboutText, "the About tab content must survive a Back navigation");

		// Act 3 — press Forward to return to the About page.
		vm.DockWorkspace.NavigateForwardCommand.CanExecute(null).Should().BeTrue();
		// execute vm.DockWorkspace.NavigateForwardCommand
		vm.DockWorkspace.NavigateForwardCommand.Execute(null);
		// wait for the predicate
		await Waiters.WaitForAsync(
			() => ReferenceEquals(documents.ActiveDockable, aboutTab));

		// Assert — About is active again, content intact.
		aboutTab.Text.Should().Be(aboutText);
	}
}
