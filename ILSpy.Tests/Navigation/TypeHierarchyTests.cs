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
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.TypeHierarchy;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class TypeHierarchyTests
{
	static async Task<(MainWindowViewModel Vm, TypeHierarchyViewModel Pane)> ShowForSelectedAsync(string typeName)
	{
		var (_, vm) = await TestHarness.BootAsync();
		await HierarchyFixture.OpenAsync(vm);
		vm.AssemblyTreeModel.SelectNode(HierarchyFixture.FindType(vm, typeName));
		AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources.TypeHierarchy)).Execute(null);
		var pane = AppComposition.Current.GetExport<TypeHierarchyViewModel>();
		pane.Target.Should().NotBeNull("the command must target the selected type");
		await pane.Target!.LoadChildrenAsync();
		return (vm, pane);
	}

	static TypeHierarchyNode Group(TypeHierarchyNode target, TypeHierarchyNodeKind kind)
		=> target.Children.Single(c => c.Kind == kind);

	static string[] TypeNames(TypeHierarchyNode group)
		=> group.Children.Select(c => c.Type!.FullName).ToArray();

	[AvaloniaTest]
	public async Task Command_Shows_Pane_With_Base_Types_And_Derived_Types_Of_Selected_Type()
	{
		// Arrange + Act -- select ShapeBase in the tree and run Navigate > Type Hierarchy.
		var (vm, pane) = await ShowForSelectedAsync("ShapeBase");

		// Assert -- the pane is docked, targets ShapeBase, lists its direct supertypes (the base
		// class and the implemented interface) and, once the background search finishes, its
		// direct subclasses.
		vm.DockWorkspace.ToolPaneMenuItems.Single(i => i.Title == Resources.TypeHierarchy)
			.IsPaneVisible.Should().BeTrue();
		pane.Target!.Type!.FullName.Should().Be("Shapes.ShapeBase");
		pane.Roots.Should().ContainSingle().Which.Should().BeSameAs(pane.Target);

		var bases = Group(pane.Target, TypeHierarchyNodeKind.BaseTypesGroup);
		TypeNames(bases).Should().BeEquivalentTo("System.Object", "Shapes.IShape");

		var derived = Group(pane.Target, TypeHierarchyNodeKind.DerivedTypesGroup);
		await derived.LoadChildrenAsync();
		TypeNames(derived).Should().BeEquivalentTo("Shapes.Circle", "Shapes.Square");
		derived.Children.Should().OnlyContain(c => c.Kind == TypeHierarchyNodeKind.DerivedType && c.Icon != null);
	}

	[AvaloniaTest]
	public async Task Derived_Nodes_Expand_To_Their_Own_Subtypes_And_Base_Nodes_To_Their_Supertypes()
	{
		var (_, pane) = await ShowForSelectedAsync("ShapeBase");

		// Act -- drill one level further both ways.
		var derived = Group(pane.Target!, TypeHierarchyNodeKind.DerivedTypesGroup);
		await derived.LoadChildrenAsync();
		var circle = derived.Children.Single(c => c.Type!.Name == "Circle");
		await circle.LoadChildrenAsync();

		var bases = Group(pane.Target!, TypeHierarchyNodeKind.BaseTypesGroup);
		var iShape = bases.Children.Single(c => c.Type!.Name == "IShape");
		await iShape.LoadChildrenAsync();

		// Assert -- Circle's subtypes are listed below it; IShape has no supertypes.
		TypeNames(circle).Should().Equal("Shapes.BigCircle");
		iShape.Children.Should().BeEmpty();
	}

	[AvaloniaTest]
	public async Task Interface_Target_Lists_Its_Implementations()
	{
		var (_, pane) = await ShowForSelectedAsync("IShape");

		var derived = Group(pane.Target!, TypeHierarchyNodeKind.DerivedTypesGroup);
		await derived.LoadChildrenAsync();

		TypeNames(derived).Should().Equal("Shapes.ShapeBase");
		Group(pane.Target!, TypeHierarchyNodeKind.BaseTypesGroup).Children.Should().BeEmpty();
	}

	[AvaloniaTest]
	public async Task Member_Selection_Targets_The_Declaring_Type()
	{
		// Arrange -- select a method of Circle rather than the type itself.
		var (_, vm) = await TestHarness.BootAsync();
		await HierarchyFixture.OpenAsync(vm);
		var circleNode = HierarchyFixture.FindType(vm, "Circle");
		var method = circleNode.GetChild<MethodTreeNode>(m => m.MethodDefinition.Name == "Diameter");
		vm.AssemblyTreeModel.SelectNode(method);

		// Act
		AppComposition.Current.GetExport<MainMenuCommandRegistry>()
			.GetCommand(nameof(Resources.TypeHierarchy)).Execute(null);

		// Assert
		var pane = AppComposition.Current.GetExport<TypeHierarchyViewModel>();
		pane.Target!.Type!.FullName.Should().Be("Shapes.Circle");
	}

	[AvaloniaTest]
	public async Task Activating_A_Node_Navigates_The_Assembly_Tree_And_Records_History()
	{
		var (vm, pane) = await ShowForSelectedAsync("ShapeBase");
		var derived = Group(pane.Target!, TypeHierarchyNodeKind.DerivedTypesGroup);
		await derived.LoadChildrenAsync();
		var square = derived.Children.Single(c => c.Type!.Name == "Square");
		// NavigationHistory merges selections within 0.5s into one entry.
		await Task.Delay(600);

		// Act
		pane.Activate(square).Should().BeTrue();

		// Assert -- the tree selection moves to Square through the normal navigation path, so
		// Back returns to the previous selection.
		var selected = vm.AssemblyTreeModel.SelectedItem as TypeTreeNode;
		Assert.That(selected, Is.Not.Null, "the selection must be a type node");
		selected!.TypeDefinition.FullName.Should().Be("Shapes.Square");
		vm.DockWorkspace.NavigateBackCommand.CanExecute(null).Should().BeTrue();
	}

	[AvaloniaTest]
	public async Task Pane_View_Renders_The_Hierarchy_And_Enter_Navigates_To_The_Selected_Type()
	{
		var (vm, pane) = await ShowForSelectedAsync("ShapeBase");
		var derived = Group(pane.Target!, TypeHierarchyNodeKind.DerivedTypesGroup);
		await derived.LoadChildrenAsync();
		var circle = derived.Children.Single(c => c.Type!.Name == "Circle");
		var window = AppComposition.Current.GetExport<global::ICSharpCode.ILSpy.Views.MainWindow>();
		var view = await window.WaitForComponent<TypeHierarchyView>();
		var tree = view.FindControl<TreeView>("HierarchyTree")!;
		tree.ItemsSource.Should().BeSameAs(pane.Roots);
		await Task.Delay(600);

		// Act -- select Circle and press Enter in the tree.
		pane.SelectedNode = circle;
		Dispatcher.UIThread.RunJobs();
		tree.RaiseEvent(new KeyEventArgs { RoutedEvent = InputElement.KeyDownEvent, Key = Key.Enter, Source = tree });

		// Assert
		var selected = vm.AssemblyTreeModel.SelectedItem as TypeTreeNode;
		Assert.That(selected, Is.Not.Null, "the selection must be a type node");
		selected!.TypeDefinition.FullName.Should().Be("Shapes.Circle");
	}

	[AvaloniaTest]
	public async Task Retargeting_Cancels_The_Previous_Derived_Type_Search()
	{
		var (_, vm) = await TestHarness.BootAsync();
		await HierarchyFixture.OpenAsync(vm);
		var pane = AppComposition.Current.GetExport<TypeHierarchyViewModel>();
		var first = HierarchyFixture.FindType(vm, "ShapeBase").TypeDefinition;
		var second = HierarchyFixture.FindType(vm, "Circle").TypeDefinition;

		// Act -- start a search, then retarget before it can be observed.
		pane.ShowHierarchy(first);
		var firstToken = pane.SearchCancellationToken;
		pane.ShowHierarchy(second);

		// Assert
		firstToken.IsCancellationRequested.Should().BeTrue();
		pane.SearchCancellationToken.IsCancellationRequested.Should().BeFalse();
		pane.Target!.Type!.FullName.Should().Be("Shapes.Circle");
	}

	[AvaloniaTest]
	public async Task Code_Reference_Resolves_To_A_Type_For_The_Context_Menu_Entry()
	{
		var (_, vm) = await TestHarness.BootAsync();
		await HierarchyFixture.OpenAsync(vm);
		var square = HierarchyFixture.FindType(vm, "Square").TypeDefinition;
		var area = square.Methods.Single(m => m.Name == "Area");
		var entry = AppComposition.Current.GetExport<ContextMenuEntryRegistry>().Entries
			.Single(e => e.Metadata.Header == nameof(Resources.TypeHierarchy));

		// The Navigation category is what lists the entry in the Navigate To popup.
		entry.Metadata.Category.Should().Be("Navigation");

		var memberContext = new TextViewContext { Reference = new ReferenceSegment { Reference = area } };
		entry.Value.IsVisible(memberContext).Should().BeTrue();
		TypeHierarchyViewModel.ResolveType(area)!.FullName.Should().Be("Shapes.Square");
		TypeHierarchyViewModel.ResolveType((IType)square)!.FullName.Should().Be("Shapes.Square");

		var localContext = new TextViewContext { Reference = new ReferenceSegment { Reference = "local" } };
		entry.Value.IsVisible(localContext).Should().BeFalse();

		// Act
		entry.Value.Execute(memberContext);

		// Assert
		AppComposition.Current.GetExport<TypeHierarchyViewModel>().Target!.Type!.FullName.Should().Be("Shapes.Square");
	}

	[Test]
	public void Gesture_Is_Ctrl_Alt_H()
	{
		var gesture = KeyGesture.Parse(TypeHierarchyCommand.Gesture);
		gesture.Key.Should().Be(Key.H);
		gesture.KeyModifiers.Should().Be(KeyModifiers.Control | KeyModifiers.Alt);
	}
}
