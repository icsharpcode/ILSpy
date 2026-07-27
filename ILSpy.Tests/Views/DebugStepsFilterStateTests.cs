// Copyright (c) 2026 Siegfried Pammer
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

#if DEBUG

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.Decompiler.DebugSteps;

using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Views;

/// <summary>
/// The Debug Steps filter box must be a transient view on the step tree: it may expand and
/// hide rows while active, but expansion states the user established before filtering and
/// the selection made while filtering must survive clearing it. Rows are located through the
/// visual tree (header text -> nearest TreeViewItem) and user gestures are simulated with
/// SetCurrentValue, which is what every real expander gesture (toggle-arrow, double-tap,
/// arrow keys) ends in — a plain local SetValue would mask style-driven bindings and test a
/// gesture that cannot occur.
/// </summary>
[TestFixture]
public class DebugStepsFilterStateTests
{
	const string MatchGroupDescription = "CombineQueryExpressions";
	const string OtherGroupDescription = "TransformExpressionTrees";
	const string MatchingLeafDescription = "3: Introduce query continuation";
	const string SiblingLeafDescription = "4: Flatten switch section block";
	const string OtherLeafDescription = "7: Copy annotations";

	static IList<Stepper.Node> BuildStepTree()
	{
		var matchGroup = new Stepper.Node(MatchGroupDescription);
		matchGroup.Children.Add(new Stepper.Node(MatchingLeafDescription));
		matchGroup.Children.Add(new Stepper.Node(SiblingLeafDescription));
		var otherGroup = new Stepper.Node(OtherGroupDescription);
		otherGroup.Children.Add(new Stepper.Node(OtherLeafDescription));
		return new[] { matchGroup, otherGroup };
	}

	static (Window Window, DebugStepsPaneModel Vm, TreeView Tree) ShowPane()
	{
		var vm = new DebugStepsPaneModel();
		SetSteps(vm, BuildStepTree());
		vm.IsAvailable = true;
		var window = new Window { Width = 400, Height = 300, Content = new DebugSteps { DataContext = vm } };
		window.Show();
		Dispatcher.UIThread.RunJobs();
		var tree = window.GetVisualDescendants().OfType<TreeView>().First();
		return (window, vm, tree);
	}

	static void SetSteps(DebugStepsPaneModel vm, IList<Stepper.Node> steps)
	{
		vm.SetStepsSource(steps);
	}

	// Rows are looked up by DataContext rather than by header text: a row hidden by the filter
	// is never measured, so its header TextBlock may not have been templated yet, but its
	// container still exists in the visual tree.
	static TreeViewItem RowFor(TreeView tree, string description)
	{
		var row = tree.GetVisualDescendants().OfType<TreeViewItem>()
			.FirstOrDefault(item => DescriptionOf(item.DataContext) == description);
		row.Should().NotBeNull($"a row with the description '{description}' must be materialised");
		return row!;
	}

	static string? DescriptionOf(object? dataContext) => dataContext switch {
		StepNodeViewModel node => node.Description,
		_ => null,
	};

	[AvaloniaTest]
	public Task Manual_Expansion_Survives_A_Filter_Round_Trip()
	{
		var (window, vm, tree) = ShowPane();
		var matchGroup = RowFor(tree, MatchGroupDescription);
		var otherGroup = RowFor(tree, OtherGroupDescription);

		matchGroup.SetCurrentValue(TreeViewItem.IsExpandedProperty, true);
		Dispatcher.UIThread.RunJobs();

		vm.FilterText = "continuation";
		Dispatcher.UIThread.RunJobs();
		vm.FilterText = "";
		Dispatcher.UIThread.RunJobs();

		matchGroup.IsExpanded.Should().BeTrue(
			"a group the user expanded before filtering must still be expanded after the filter is cleared");
		otherGroup.IsExpanded.Should().BeFalse(
			"a group the user never expanded must not stay expanded after the filter is cleared");

		window.Close();
		return Task.CompletedTask;
	}

	[AvaloniaTest]
	public Task Filtering_Expands_Groups_Leading_To_Matches_And_Hides_The_Rest()
	{
		var (window, vm, tree) = ShowPane();

		vm.FilterText = "continuation";
		Dispatcher.UIThread.RunJobs();

		var matchGroup = RowFor(tree, MatchGroupDescription);
		matchGroup.IsExpanded.Should().BeTrue(
			"a group containing a match must expand so the match is revealed");
		matchGroup.IsVisible.Should().BeTrue("the path to a match must stay visible");
		RowFor(tree, MatchingLeafDescription).IsVisible.Should().BeTrue("the match itself must be visible");
		RowFor(tree, SiblingLeafDescription).IsVisible.Should().BeFalse(
			"a sibling that does not match must be hidden");
		RowFor(tree, OtherGroupDescription).IsVisible.Should().BeFalse(
			"a group without any match must be hidden");

		window.Close();
		return Task.CompletedTask;
	}

	[AvaloniaTest]
	public Task Selection_Made_While_Filtering_Stays_Visible_After_Clearing()
	{
		var (window, vm, tree) = ShowPane();

		vm.FilterText = "continuation";
		Dispatcher.UIThread.RunJobs();

		var leaf = RowFor(tree, MatchingLeafDescription);
		tree.SelectedItem = leaf.DataContext;
		Dispatcher.UIThread.RunJobs();

		vm.FilterText = "";
		Dispatcher.UIThread.RunJobs();

		tree.SelectedItem.Should().BeSameAs(leaf.DataContext,
			"clearing the filter must not change the selection");
		RowFor(tree, MatchGroupDescription).IsExpanded.Should().BeTrue(
			"the selected step's group must stay expanded so the selection remains visible");
		leaf.IsEffectivelyVisible.Should().BeTrue(
			"the step selected while filtering must still be on screen after the filter is cleared");

		window.Close();
		return Task.CompletedTask;
	}
}

#endif
