// Copyright (c) 2026 Piero Viano
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
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Navigation;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class GoToCommandTests
{
	// Decompiles the fixture's Circle type and puts the caret (with keyboard focus) on the
	// "ShapeBase" reference in its base-type list.
	static async Task<(MainWindow Window, MainWindowViewModel ViewModel, DecompilerTextView View)> CaretOnBaseTypeReferenceAsync()
	{
		var (window, vm) = await TestHarness.BootAsync();
		await GoToFixture.OpenAsync(vm);
		vm.AssemblyTreeModel.SelectNode(GoToFixture.TypeNode(vm, "Circle"));
		var tab = await vm.DockWorkspace.WaitForDecompiledTextAsync();
		var view = window.GetVisualDescendants().OfType<DecompilerTextView>().First(v => v.DataContext == tab);
		AvaloniaHeadlessPlatform.ForceRenderTimerTick();
		Dispatcher.UIThread.RunJobs();

		var segment = tab.References!.First(r => !r.IsDefinition && r.Reference is ITypeDefinition { Name: "ShapeBase" });
		view.Editor.TextArea.Focus();
		view.Editor.TextArea.Caret.Offset = segment.StartOffset + 1;
		Dispatcher.UIThread.RunJobs();
		return (window, vm, view);
	}

	static NativeMenuItem NavigateMenu(MainWindow window)
	{
		var menu = NativeMenu.GetMenu(window) ?? throw new InvalidOperationException("main menu not attached");
		return menu.Items.OfType<NativeMenuItem>().Single(i => i.Header == Resources._Navigate);
	}

	[AvaloniaTest]
	public async Task Navigate_Menu_Lists_The_Go_To_Commands_With_Their_Shortcuts()
	{
		var (window, _) = await TestHarness.BootAsync();
		var items = NavigateMenu(window).Menu!.Items.OfType<NativeMenuItem>()
			.Where(i => i is not NativeMenuItemSeparator)
			.ToDictionary(i => i.Header!, i => i.Gesture);

		items[Resources.NavigateTo].Should().Be(KeyGesture.Parse(OperatingSystem.IsMacOS() ? "Cmd+Shift+G" : "Ctrl+Shift+G"));
		items[Resources.GoToDeclaration].Should().Be(KeyGesture.Parse("F12"));
		items[Resources.GoToImplementation].Should().Be(KeyGesture.Parse(OperatingSystem.IsMacOS() ? "Cmd+F12" : "Ctrl+F12"));
		items[Resources.GoToBaseSymbols].Should().Be(KeyGesture.Parse("Alt+Home"));
		items[Resources.GoToDerivedSymbols].Should().Be(KeyGesture.Parse("Alt+End"));
		items[Resources.Analyze].Should().Be(KeyGesture.Parse("Shift+F12"));
	}

	[AvaloniaTest]
	public async Task Main_Menu_Go_To_Declaration_Acts_On_The_Symbol_Under_The_Caret()
	{
		var (_, vm, _) = await CaretOnBaseTypeReferenceAsync();
		var command = AppComposition.Current.GetExport<MainMenuCommandRegistry>().GetCommand(nameof(Resources.GoToDeclaration));
		var navigator = AppComposition.Current.GetExport<GoToNavigator>();

		command.CanExecute(null).Should().BeTrue();
		command.Execute(null);
		await navigator.LastOperation;
		Dispatcher.UIThread.RunJobs();

		var selected = (vm.AssemblyTreeModel.SelectedItem as IMemberTreeNode)?.Member;
		selected.Should().NotBeNull();
		GoToFixture.Describe(selected!).Should().Be("ShapeBase");
	}

	[AvaloniaTest]
	public async Task Main_Menu_Go_To_Derived_Acts_On_The_Selected_Tree_Node_When_The_Tree_Has_Focus()
	{
		var (_, vm) = await TestHarness.BootAsync();
		await GoToFixture.OpenAsync(vm);
		var command = AppComposition.Current.GetExport<MainMenuCommandRegistry>().GetCommand(nameof(Resources.GoToDerivedSymbols));
		var navigator = AppComposition.Current.GetExport<GoToNavigator>();
		var shape = GoToFixture.TypeNode(vm, "IShape");
		vm.AssemblyTreeModel.SelectNode(shape);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		ActiveNavigationContext.ClearLastFocusedTextView();

		command.Execute(null);
		await navigator.LastOperation;
		Dispatcher.UIThread.RunJobs();

		GoToFixture.Describe(((IMemberTreeNode)vm.AssemblyTreeModel.SelectedItem!).Member!).Should().Be("ShapeBase");
	}

	[AvaloniaTest]
	public async Task Caret_Context_Resolves_The_Reference_At_The_Caret()
	{
		var (_, _, view) = await CaretOnBaseTypeReferenceAsync();
		var context = ActiveNavigationContext.ForTextViewCaret(view);
		context.TextView.Should().BeSameAs(view);
		(context.Reference?.Reference as ITypeDefinition)?.Name.Should().Be("ShapeBase");
	}

	[AvaloniaTest]
	public async Task Caret_Context_Ignores_Hover_Only_References()
	{
		var (_, _, view) = await CaretOnBaseTypeReferenceAsync();
		var tab = (DecompilerTabPageModel)view.DataContext!;
		var offset = view.Editor.TextArea.Caret.Offset;
		// Tooltip-only segments (synthesized dynamic members) are not navigation targets.
		foreach (var segment in tab.References!.FindSegmentsContaining(offset))
			segment.Kind = ReferenceMode.HoverOnly;

		ActiveNavigationContext.ForTextViewCaret(view).Reference.Should().BeNull();
	}

	[AvaloniaTest]
	public async Task Navigate_To_Lists_Visible_Navigation_Entries_And_Runs_The_Chosen_One()
	{
		var (_, vm, _) = await CaretOnBaseTypeReferenceAsync();
		var command = AppComposition.Current.GetExport<MainMenuCommandRegistry>().GetCommand(nameof(Resources.NavigateTo));
		var navigator = AppComposition.Current.GetExport<GoToNavigator>();

		command.Execute(null);
		Dispatcher.UIThread.RunJobs();

		var chooser = navigator.ActiveChooser;
		chooser.Should().NotBeNull("Navigate To opens the action chooser");
		var texts = chooser!.VisibleChoices.Select(c => c.Text).ToList();
		texts.Should().Contain(new[] {
			Resources.GoToDeclaration, Resources.GoToImplementation, Resources.GoToDerivedSymbols,
			Resources.GoToBaseSymbols, Resources.Decompile });
		texts.Should().NotContain(Resources.Analyze, "only Navigation-category entries are offered");

		chooser.Filter = Resources.GoToDeclaration;
		chooser.Accept();
		await navigator.LastOperation;
		Dispatcher.UIThread.RunJobs();

		GoToFixture.Describe(((IMemberTreeNode)vm.AssemblyTreeModel.SelectedItem!).Member!).Should().Be("ShapeBase");
	}

	[AvaloniaTest]
	public async Task Find_Usages_Menu_Command_Analyzes_The_Caret_Symbol()
	{
		var (_, _, _) = await CaretOnBaseTypeReferenceAsync();
		var command = AppComposition.Current.GetExport<MainMenuCommandRegistry>().GetCommand(nameof(Resources.Analyze));
		var analyzer = AppComposition.Current.GetExport<global::ICSharpCode.ILSpy.Analyzers.AnalyzerTreeViewModel>();

		command.Execute(null);
		Dispatcher.UIThread.RunJobs();

		analyzer.Root.Children.OfType<global::ICSharpCode.ILSpy.Analyzers.AnalyzerEntityTreeNode>()
			.Should().Contain(n => n.Member != null && n.Member.Name == "ShapeBase");
	}
}
