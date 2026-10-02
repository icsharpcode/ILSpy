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

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Navigation;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class GoToSymbolTests
{
	static async Task<MainWindowViewModel> BootWithFixtureAsync()
	{
		var (_, vm) = await TestHarness.BootAsync();
		await GoToFixture.OpenAsync(vm);
		return vm;
	}

	static string[] Find(MainWindowViewModel vm, GoToKind kind, IEntity entity)
		=> SymbolHierarchy.Find(kind, entity, vm.AssemblyTreeModel.AssemblyList!, CancellationToken.None)
			.Select(GoToFixture.Describe).OrderBy(s => s).ToArray();

	[AvaloniaTest]
	public async Task Implementations_Of_An_Interface_Method_Are_The_Concrete_Overrides()
	{
		var vm = await BootWithFixtureAsync();
		Find(vm, GoToKind.Implementation, GoToFixture.Method(vm, "IShape", "Area"))
			.Should().Equal("Circle.Area", "Square.Area");
	}

	[AvaloniaTest]
	public async Task Derived_Symbols_Of_An_Interface_Method_Are_Its_Direct_Implementations()
	{
		var vm = await BootWithFixtureAsync();
		Find(vm, GoToKind.DerivedSymbols, GoToFixture.Method(vm, "IShape", "Area"))
			.Should().Equal("ShapeBase.Area");
		Find(vm, GoToKind.DerivedSymbols, GoToFixture.Method(vm, "ShapeBase", "Area"))
			.Should().Equal("Circle.Area", "Square.Area");
	}

	[AvaloniaTest]
	public async Task Base_Symbols_Of_An_Override_Are_The_Overridden_And_Implemented_Members()
	{
		var vm = await BootWithFixtureAsync();
		Find(vm, GoToKind.BaseSymbols, GoToFixture.Method(vm, "Circle", "Area"))
			.Should().Equal("IShape.Area", "ShapeBase.Area");
	}

	[AvaloniaTest]
	public async Task Type_Hierarchy_Is_Resolved_Across_The_Assembly_List()
	{
		var vm = await BootWithFixtureAsync();
		Find(vm, GoToKind.Implementation, GoToFixture.TypeDef(vm, "IShape")).Should().Equal("Circle", "Square");
		Find(vm, GoToKind.DerivedSymbols, GoToFixture.TypeDef(vm, "IShape")).Should().Equal("ShapeBase");
		Find(vm, GoToKind.DerivedSymbols, GoToFixture.TypeDef(vm, "ShapeBase")).Should().Equal("Circle", "Square");
		Find(vm, GoToKind.BaseSymbols, GoToFixture.TypeDef(vm, "Circle")).Should().Equal("ShapeBase");
		Find(vm, GoToKind.BaseSymbols, GoToFixture.TypeDef(vm, "ShapeBase")).Should().Equal("IShape", "Object");
	}

	[AvaloniaTest]
	public async Task Hierarchy_Entries_Are_Hidden_For_Symbols_That_Cannot_Have_Inheritors()
	{
		var vm = await BootWithFixtureAsync();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		var solo = GoToFixture.MethodNode(vm, "Lonely", "Solo");
		var context = new TextViewContext { SelectedTreeNodes = [solo] };

		registry.GetEntry(nameof(Resources.GoToImplementation)).IsVisible(context).Should().BeFalse();
		registry.GetEntry(nameof(Resources.GoToDerivedSymbols)).IsVisible(context).Should().BeFalse();
		registry.GetEntry(nameof(Resources.GoToBaseSymbols)).IsVisible(context).Should().BeFalse(
			"a non-virtual method declared on a class with only System.Object as base has no base symbol");
		registry.GetEntry(nameof(Resources.GoToDeclaration)).IsVisible(context).Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task Navigation_Entries_Share_The_Navigation_Category()
	{
		await TestHarness.BootAsync();
		var registry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>();
		foreach (var header in new[] {
			nameof(Resources.GoToDeclaration), nameof(Resources.GoToImplementation),
			nameof(Resources.GoToBaseSymbols), nameof(Resources.GoToDerivedSymbols),
			nameof(Resources.LocateInAssemblyExplorer) })
		{
			registry.Entries.Single(e => e.Metadata.Header == header).Metadata.Category
				.Should().Be("Navigation", $"{header} must surface in the Navigate To popup");
		}
	}

	[AvaloniaTest]
	public async Task Go_To_Base_On_A_Tree_Node_With_One_Target_Navigates_Directly()
	{
		var vm = await BootWithFixtureAsync();
		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>().GetEntry(nameof(Resources.GoToBaseSymbols));
		var navigator = AppComposition.Current.GetExport<GoToNavigator>();
		var circle = GoToFixture.TypeNode(vm, "Circle");
		vm.AssemblyTreeModel.SelectNode(circle);
		var context = new TextViewContext { SelectedTreeNodes = [circle] };

		entry.IsVisible(context).Should().BeTrue();
		entry.Execute(context);
		await navigator.LastOperation;
		Dispatcher.UIThread.RunJobs();

		var selected = (vm.AssemblyTreeModel.SelectedItem as IMemberTreeNode)?.Member;
		selected.Should().NotBeNull("a single base type is navigated to without a chooser");
		GoToFixture.Describe(selected!).Should().Be("ShapeBase");
		navigator.ActiveChooser.Should().BeNull("one target needs no chooser");
	}

	[AvaloniaTest]
	public async Task Go_To_Implementation_On_A_Code_Reference_With_Several_Targets_Opens_A_Searchable_Chooser()
	{
		var vm = await BootWithFixtureAsync();
		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>().GetEntry(nameof(Resources.GoToImplementation));
		var navigator = AppComposition.Current.GetExport<GoToNavigator>();
		var area = GoToFixture.Method(vm, "IShape", "Area");
		var context = new TextViewContext { Reference = new ReferenceSegment { Reference = area } };

		entry.IsVisible(context).Should().BeTrue();
		entry.Execute(context);
		await navigator.LastOperation;
		Dispatcher.UIThread.RunJobs();

		var chooser = navigator.ActiveChooser;
		chooser.Should().NotBeNull("two implementations need a chooser");
		chooser!.IsOpen.Should().BeTrue();
		chooser.VisibleChoices.Should().HaveCount(2);

		chooser.Filter = "Square";
		chooser.VisibleChoices.Should().ContainSingle().Which.Text.Should().Contain("Square");

		chooser.FilterBox.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Enter });
		Dispatcher.UIThread.RunJobs();

		chooser.IsOpen.Should().BeFalse("accepting a choice closes the chooser");
		var selected = (vm.AssemblyTreeModel.SelectedItem as IMemberTreeNode)?.Member;
		selected.Should().NotBeNull();
		GoToFixture.Describe(selected!).Should().Be("Square.Area");
	}

	[AvaloniaTest]
	public async Task Escape_Closes_The_Chooser_Without_Navigating()
	{
		var vm = await BootWithFixtureAsync();
		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>().GetEntry(nameof(Resources.GoToDerivedSymbols));
		var navigator = AppComposition.Current.GetExport<GoToNavigator>();
		var shapeBase = GoToFixture.TypeNode(vm, "ShapeBase");
		vm.AssemblyTreeModel.SelectNode(shapeBase);
		entry.Execute(new TextViewContext { SelectedTreeNodes = [shapeBase] });
		await navigator.LastOperation;
		Dispatcher.UIThread.RunJobs();

		var chooser = navigator.ActiveChooser;
		chooser.Should().NotBeNull();
		chooser!.FilterBox.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Escape });
		Dispatcher.UIThread.RunJobs();

		chooser.IsOpen.Should().BeFalse();
		ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, shapeBase).Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task No_Targets_Shows_A_Notice_Instead_Of_Navigating()
	{
		var vm = await BootWithFixtureAsync();
		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>().GetEntry(nameof(Resources.GoToDerivedSymbols));
		var navigator = AppComposition.Current.GetExport<GoToNavigator>();
		var circle = GoToFixture.TypeNode(vm, "Circle");
		vm.AssemblyTreeModel.SelectNode(circle);
		entry.Execute(new TextViewContext { SelectedTreeNodes = [circle] });
		await navigator.LastOperation;
		Dispatcher.UIThread.RunJobs();

		navigator.ActiveChooser.Should().BeNull();
		navigator.LastNotice.Should().Be("No derived symbols found");
		ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, circle).Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task Go_To_Declaration_On_A_Code_Reference_Navigates_To_The_Definition()
	{
		var vm = await BootWithFixtureAsync();
		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>().GetEntry(nameof(Resources.GoToDeclaration));
		var navigator = AppComposition.Current.GetExport<GoToNavigator>();
		vm.AssemblyTreeModel.SelectNode(GoToFixture.TypeNode(vm, "Lonely"));
		var square = GoToFixture.TypeDef(vm, "Square");

		entry.Execute(new TextViewContext { Reference = new ReferenceSegment { Reference = square } });
		await navigator.LastOperation;
		Dispatcher.UIThread.RunJobs();

		GoToFixture.Describe(((IMemberTreeNode)vm.AssemblyTreeModel.SelectedItem!).Member!).Should().Be("Square");
	}
}
