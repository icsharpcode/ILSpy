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

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Navigation;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

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
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		// Dismiss the startup welcome page first: it renders the same About content in the reusable
		// main tab, and while it is visible Help > About activates it rather than opening a tab.
		// Selecting a node replaces the welcome content so About then opens its own tab as asserted.
		vm.AssemblyTreeModel.SelectNode(vm.AssemblyTreeModel.Root!.Children[0]);
		await Waiters.WaitForAsync(() => !vm.DockWorkspace.IsWelcomePageVisible,
			description: "the welcome page must be dismissed so Help > About opens a fresh tab");

		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var aboutCmd = registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources._About))
			.CreateExport().Value;

		var documents = ((ILSpyDockFactory)vm.DockWorkspace.Factory).Documents!;
		var initialTabCount = documents.VisibleDockables?.Count ?? 0;

		// Act — fire the About command.
		aboutCmd.Execute(null);

		// Assert — a new DecompilerTabPageModel landed in the document dock, titled "About",
		// containing the version line and the MIT License mention from the embedded blurb.
		await Waiters.WaitForAsync(
			() => (documents.VisibleDockables?.Count ?? 0) > initialTabCount
				&& documents.ActiveDockable is ContentTabPage { Content: DecompilerTabPageModel { Text.Length: > 0 } });

		var aboutTab = (DecompilerTabPageModel)((ContentTabPage)documents.ActiveDockable!).Content!;
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
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var registry = AppComposition.Current.GetExport<MainMenuCommandRegistry>();
		var aboutCmd = registry.Commands
			.Single(c => c.Metadata.Header == nameof(Resources._About))
			.CreateExport().Value;
		var documents = ((ILSpyDockFactory)vm.DockWorkspace.Factory).Documents!;
		aboutCmd.Execute(null);
		await Waiters.WaitForAsync(
			() => documents.ActiveDockable is ContentTabPage { Content: DecompilerTabPageModel { Text.Length: > 0 } });
		var aboutTab = (DecompilerTabPageModel)((ContentTabPage)documents.ActiveDockable!).Content!;

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
				&& documents.ActiveDockable is ContentTabPage { Content: DecompilerTabPageModel licenseInner }
				&& !ReferenceEquals(licenseInner, aboutTab)
				&& licenseInner.Text.Length > 0);
		var licenseTab = (DecompilerTabPageModel)((ContentTabPage)documents.ActiveDockable!).Content!;
		licenseTab.Text.Should().Contain("Permission is hereby granted");
	}

	[Test]
	public void ResourceLinkGenerator_Does_Not_Require_Ctrl_Modifier_For_Click()
	{
		// AvaloniaEdit's TextEditorOptions.RequireControlModifierForHyperlinkClick only
		// propagates to the *built-in* LinkElementGenerator via IBuiltinElementGenerator.
		// FetchOptions; user-added subclasses keep whatever the base parameterless ctor sets
		// (default true). The About-page generator must opt out explicitly so its links
		// activate on a plain click.
		var gen = new ResourceLinkGenerator(
			"MIT License",
			new System.Uri("resource:license.txt"));
		gen.RequireControlModifierForClick.Should().BeFalse();
	}

	[AvaloniaTest]
	public async Task Decompiler_Editor_Activates_Hyperlinks_Without_Ctrl_Modifier()
	{
		// AvaloniaEdit defaults RequireControlModifierForHyperlinkClick=true on its
		// TextEditorOptions; decompiler output uses hyperlinks for in-app navigation, so a
		// plain click is the expected affordance. Verifies the option is flipped on the
		// realised editor — the value AvaloniaEdit's LinkElementGenerator (and our own
		// ResourceLinkGenerator) both consult when constructing VisualLineLinkText.

		// Arrange — boot, select a node so a decompiler viewmodel is alive on the active
		// tab. Construct the view explicitly (the dock's deferred content presenter doesn't
		// materialise its templated children in Avalonia.Headless mode, so walking the
		// window's visual tree wouldn't find the editor — building one from the live
		// viewmodel mirrors what the live app's DataTemplate produces).
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);
		var node = vm.AssemblyTreeModel.FindNode<AssemblyTreeNode>("System.Linq");
		vm.AssemblyTreeModel.SelectNode(node);
		var content = await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var view = new DecompilerTextView { DataContext = content };
		// Force layout so the templated TextEditor child is realised — the view's
		// DataContextChanged-driven setup runs synchronously on assignment.
		var host = new Window { Content = view, Width = 600, Height = 400 };
		host.Show();
		var editor = await view.WaitForComponent<AvaloniaEdit.TextEditor>();

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
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var method = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "AsEnumerable");
		vm.AssemblyTreeModel.SelectNode(method);
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
		var mainTab = ((ILSpyDockFactory)vm.DockWorkspace.Factory).MainTab!;
		aboutCmd.Execute(null);
		await Waiters.WaitForAsync(
			() => documents.ActiveDockable is ContentTabPage { Content: DecompilerTabPageModel { IsStaticContent: true } });
		var aboutWrapper = (ContentTabPage)documents.ActiveDockable!;
		var aboutTab = (DecompilerTabPageModel)aboutWrapper.Content!;
		var aboutText = aboutTab.Text;

		// Assert (mid-test) — the back stack now ends with the tree-node entry; the About
		// page is the current entry (so Back goes there).
		vm.DockWorkspace.BackHistory.Should().NotBeEmpty();
		vm.DockWorkspace.BackHistory.Last().Should().BeOfType<TreeNodeEntry>();
		var lastBack = (TreeNodeEntry)vm.DockWorkspace.BackHistory.Last();
		lastBack.Node.GetType().Should().Be(typeof(MethodTreeNode));

		// Act 2 — press Back from the About page.
		vm.DockWorkspace.NavigateBackCommand.CanExecute(null).Should().BeTrue();
		vm.DockWorkspace.NavigateBackCommand.Execute(null);
		await Waiters.WaitForAsync(
			() => ReferenceEquals(documents.ActiveDockable, mainTab));

		// Assert — the main tab (with the decompiler content) is active again; the About
		// sibling still exists with content preserved.
		ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, method).Should().BeTrue();
		decompilerTab.Text.Should().Be(methodText);
		mainTab.Content.Should().BeSameAs(decompilerTab);
		documents.VisibleDockables!.Should().Contain(aboutWrapper);
		aboutTab.Text.Should().Be(aboutText, "the About tab content must survive a Back navigation");

		// Act 3 — press Forward to return to the About page.
		vm.DockWorkspace.NavigateForwardCommand.CanExecute(null).Should().BeTrue();
		vm.DockWorkspace.NavigateForwardCommand.Execute(null);
		await Waiters.WaitForAsync(
			() => ReferenceEquals(documents.ActiveDockable, aboutWrapper));

		// Assert — About is active again, content intact.
		aboutTab.Text.Should().Be(aboutText);
	}
}
