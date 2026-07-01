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

#if DEBUG

using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;
using Avalonia.VisualTree;

using AwesomeAssertions;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.CSharp.Transforms;
using ICSharpCode.Decompiler.DebugSteps;

using ICSharpCode.ILSpy;
using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using ICSharpCode.ILSpy.ViewModels;
using ICSharpCode.ILSpy.Views;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Views;

/// <summary>
/// Pins the Debug Steps wiring at the composition layer. Tests are gated by DEBUG since
/// the feature is itself DEBUG-only — Release builds wouldn't have the types these
/// assertions reference.
/// </summary>
[TestFixture]
public class DebugStepsTests
{
	[AvaloniaTest]
	public Task DebugStepsPaneModel_Is_Exported_As_A_ToolPane()
	{
		// MEF tool-pane registry contract: DebugStepsPaneModel registers under its
		// PaneContentId so DockWorkspace.ShowToolPane(...) can surface it.
		var pane = AppComposition.Current.GetExport<DebugStepsPaneModel>();
		((object?)pane).Should().NotBeNull("DebugStepsPaneModel must resolve as a [Shared] export");
		pane.Id.Should().Be(DebugStepsPaneModel.PaneContentId);
		pane.Title.Should().Be("Debug Steps");
		return Task.CompletedTask;
	}

	[AvaloniaTest]
	public Task DebugStepsPaneModel_WritingOptions_Default_Enables_Field_And_LogicOperation_Sugar()
	{
		// The default writing options match the WPF baseline: field sugar and
		// logic-operation sugar on; IL ranges and child-index-in-block off. The four
		// CheckBoxes in DebugSteps.axaml bind two-way against these defaults.
		var options = DebugStepsPaneModel.WritingOptions;
		options.UseFieldSugar.Should().BeTrue();
		options.UseLogicOperationSugar.Should().BeTrue();
		options.ShowILRanges.Should().BeFalse();
		options.ShowChildIndexInBlock.Should().BeFalse();
		return Task.CompletedTask;
	}

	[AvaloniaTest]
	public async Task Debug_Steps_VM_Populates_After_ILAst_Decompile_Regardless_Of_View_Lifecycle()
	{
		// End-to-end repro of the user-reported "Debug Steps pane is empty" bug:
		// 1. Boot the window, load assemblies.
		// 2. Select a method.
		// 3. Switch the active language to BlockIL (ILAst).
		// 4. Wait for the BlockIL decompile to finish — its OnStepperUpdated event fires.
		// 5. Assert the DebugStepsPaneModel's Steps property is populated.
		//
		// Asserting against the VM (not the View) decouples this test from the dock layout's
		// view-realisation timing — which is the whole point of the fix that moved state
		// from the View into the VM. If `Steps` is populated, any view that binds to it (now
		// or later) will render the correct content.

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

		var languageService = AppComposition.Current.GetExport<LanguageService>();
		var blockIL = languageService.Languages.OfType<ILAstLanguage>()
			.Single(l => l.Name == "ILAst");
		languageService.CurrentLanguage = blockIL;
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		blockIL.Stepper.Steps.Should().NotBeEmpty(
			"BlockILLanguage.DecompileMethod must populate context.Stepper.Steps when STEP is defined");

		var debugStepsVm = AppComposition.Current.GetExport<DebugStepsPaneModel>();
		await Waiters.WaitForAsync(
			() => debugStepsVm.Steps?.Count > 0,
			description: "DebugStepsPaneModel.Steps to be populated after the ILAst decompile");

		debugStepsVm.Steps.Should().NotBeNullOrEmpty(
			"after switching to ILAst and decompiling, the VM's Steps must list the stepper's recorded transforms");
	}

	[AvaloniaTest]
	public async Task CSharp_DebugSteps_Are_Grouped_By_Ast_Transform()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var languageService = AppComposition.Current.GetExport<LanguageService>();
		var csharp = languageService.Languages.OfType<CSharpLanguage>().First();
		languageService.CurrentLanguage = csharp;

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var method = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Range");
		vm.AssemblyTreeModel.SelectNode(method);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var debugStepsVm = AppComposition.Current.GetExport<DebugStepsPaneModel>();
		await Waiters.WaitForAsync(
			() => debugStepsVm.Steps?.Count > 0,
			description: "DebugStepsPaneModel.Steps to be populated after the C# decompile");

		var astTransformNames = CSharpDecompiler.GetAstTransforms()
			.Select(transform => transform.GetType().Name)
			.ToArray();

		debugStepsVm.Steps!
			.Select(step => StripStepNumber(step.Description))
			.Should().Equal(astTransformNames,
				"C# debug steps must use AST transforms as top-level groups");

		var transformGroupWithChanges = debugStepsVm.Steps!
			.FirstOrDefault(step => step.Children.Count > 0);
		transformGroupWithChanges.Should().NotBeNull(
			"individual C# AST mutation steps must be nested under their transform group");
		transformGroupWithChanges!.Children
			.Select(step => StripStepNumber(step.Description))
			.Should().Contain(
				description => !astTransformNames.Contains(description),
				"nested C# debug steps must describe individual AST mutation points");

		var collectedSteps = debugStepsVm.Steps;
		var replayStep = transformGroupWithChanges.Children.First();
		var tab = vm.DockWorkspace.ActiveDecompilerTab!;

		await tab.RestartDecompileWithStepLimit(replayStep.BeginStep, isDebug: false, replayStep.BeginStep);
		tab.Text.Should().NotBeNullOrWhiteSpace("C# replay before a selected AST mutation step must still emit code");
		AssertPreciseHighlight(tab, "C# replay before a selected AST mutation step must locate the changed node");
		debugStepsVm.Steps.Should().BeSameAs(collectedSteps,
			"a step-limited C# replay must not replace the full step tree shown by the pane");

		await tab.RestartDecompileWithStepLimit(replayStep.EndStep, isDebug: false, replayStep.BeginStep);
		tab.Text.Should().NotBeNullOrWhiteSpace("C# replay after a selected AST mutation step must still emit code");
		AssertPreciseHighlight(tab, "C# replay after a selected AST mutation step must locate the changed node");
		debugStepsVm.Steps.Should().BeSameAs(collectedSteps,
			"a step-limited C# replay must preserve the current full-run step tree and selection context");

		static string StripStepNumber(string description)
		{
			var separatorIndex = description.IndexOf(": ", StringComparison.Ordinal);
			return separatorIndex >= 0 ? description[(separatorIndex + 2)..] : description;
		}
	}

	[AvaloniaTest]
	public async Task ILAst_DebugStep_Replay_Highlights_Changed_Instruction()
	{
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 3);

		var languageService = AppComposition.Current.GetExport<LanguageService>();
		var blockIL = languageService.Languages.OfType<ILAstLanguage>().Single(l => l.Name == "ILAst");
		languageService.CurrentLanguage = blockIL;

		var typeNode = vm.AssemblyTreeModel.FindNode<TypeTreeNode>(
			"System.Linq", "System.Linq", "System.Linq.Enumerable");
		typeNode.IsExpanded = true;
		var method = typeNode.Children.OfType<MethodTreeNode>()
			.First(m => m.MethodDefinition.Name == "Range");
		vm.AssemblyTreeModel.SelectNode(method);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();

		var debugStepsVm = AppComposition.Current.GetExport<DebugStepsPaneModel>();
		await Waiters.WaitForAsync(
			() => debugStepsVm.Steps?.Count > 0,
			description: "DebugStepsPaneModel.Steps to be populated after the ILAst decompile");

		// Replaying an individual mutation step is what surfaces a single IL change; the leaf
		// step's changed instruction (or a surviving ancestor) must map to a rendered text range.
		var replayStep = FirstLeafStep(debugStepsVm.Steps!);
		replayStep.Should().NotBeNull("the ILAst stepper must record individual mutation steps");

		var collectedSteps = debugStepsVm.Steps;
		var tab = vm.DockWorkspace.ActiveDecompilerTab!;

		await tab.RestartDecompileWithStepLimit(replayStep!.BeginStep, isDebug: false, replayStep.BeginStep);
		tab.Text.Should().NotBeNullOrWhiteSpace("ILAst replay before a selected step must still emit IL");
		AssertPreciseHighlight(tab, "ILAst replay before a selected step must locate the changed instruction");
		debugStepsVm.Steps.Should().BeSameAs(collectedSteps,
			"a step-limited ILAst replay must not replace the full step tree shown by the pane");

		await tab.RestartDecompileWithStepLimit(replayStep.EndStep, isDebug: false, replayStep.BeginStep);
		tab.Text.Should().NotBeNullOrWhiteSpace("ILAst replay after a selected step must still emit IL");
		AssertPreciseHighlight(tab, "ILAst replay after a selected step must locate the changed instruction");

		// The first leaf step that acts on a concrete instruction; a step whose Position is null
		// (e.g. an empty transform group) has nothing to highlight and is not what a user replays.
		static Stepper.Node? FirstLeafStep(System.Collections.Generic.IEnumerable<Stepper.Node> steps)
		{
			foreach (var step in steps)
			{
				if (step.Children.Count == 0)
				{
					if (step.Position != null)
						return step;
					continue;
				}
				var leaf = FirstLeafStep(step.Children);
				if (leaf != null)
					return leaf;
			}
			return null;
		}
	}

	[AvaloniaTest]
	public Task ILAst_And_TypedIL_Languages_Are_Registered_In_Debug_Builds()
	{
		// Two ILAstLanguage subclasses: BlockILLanguage ("ILAst") drives the stepper,
		// TypedILLanguage ("Typed IL") writes type-annotated raw IL without transforms.
		// Both register via [Export(typeof(Language))]; the language picker uses them
		// in addition to C# and the disassembler-IL language.
		var languageService = AppComposition.Current.GetExport<LanguageService>();
		languageService.Languages.OfType<ILAstLanguage>().Should().HaveCount(2,
			"both BlockIL and TypedIL must be registered when DEBUG is defined");
		languageService.Languages.Should().Contain(l => l.Name == "ILAst");
		languageService.Languages.Should().Contain(l => l.Name == "Typed IL");
		return Task.CompletedTask;
	}

	[AvaloniaTest]
	public Task NodeLookup_Resolves_Copied_Ast_Annotations()
	{
		var marker = new DebugStepMarker();
		var original = new IdentifierExpression("old");
		original.AddAnnotation(marker);
		var replacement = new IdentifierExpression("new").CopyAnnotationsFrom(original);
		var lookup = new NodeLookup();

		lookup.AddNode(replacement, 12, 3);

		lookup.TryGetRange(marker, out var range).Should().BeTrue(
			"C# debug-step markers copied by AST replacements must still resolve to emitted text");
		range.Start.Should().Be(12);
		range.Length.Should().Be(3);

		// A non-marker annotation is not bridged: only the debug-step marker is queried by the
		// resolver, and indexing arbitrary shared annotations would collide by reference identity.
		var otherAnnotation = new object();
		var otherNode = new IdentifierExpression("x");
		otherNode.AddAnnotation(otherAnnotation);
		lookup.AddNode(otherNode, 30, 1);
		lookup.TryGetRange(otherAnnotation, out _).Should().BeFalse(
			"only DebugStepMarker annotations are bridged to a text range");
		return Task.CompletedTask;
	}

	[AvaloniaTest]
	public Task DebugStepHighlighter_Removal_Resolves_To_Seam_Caret()
	{
		// A step whose node was removed has no precise range and (in this fixture) no rendered
		// ancestor, only surviving neighbors. It must resolve to a zero-length caret at the gap:
		// the successor's start when one survives, otherwise the predecessor's end.
		var successor = new object();
		var predecessor = new object();

		var successorLookup = new NodeLookup();
		successorLookup.AddNode(successor, 40, 6);
		var removalWithSuccessor = new Stepper.Node("0: Remove statement") {
			BeginStep = 0,
			EndStep = 1
		};
		removalWithSuccessor.SeamAnchors.Add((successor, false));
		removalWithSuccessor.SeamAnchors.Add((predecessor, true));
		var successorStepper = new Stepper();
		successorStepper.Steps.Add(removalWithSuccessor);

		DebugStepHighlighter.TryResolve(successorStepper, stepLimit: 1, highlightStep: 0, successorLookup, out var caret)
			.Should().BeTrue("a removed node must still resolve to a surviving seam neighbor");
		caret.Length.Should().Be(0, "a removal has no text to highlight, only a caret at the gap");
		caret.Start.Should().Be(40, "the caret sits at the successor's start, where the node was");

		// Only the predecessor survives: the caret sits at its end (10 + 5).
		var predecessorLookup = new NodeLookup();
		predecessorLookup.AddNode(predecessor, 10, 5);
		var removalWithPredecessor = new Stepper.Node("0: Remove statement") {
			BeginStep = 0,
			EndStep = 1
		};
		removalWithPredecessor.SeamAnchors.Add((successor, false));
		removalWithPredecessor.SeamAnchors.Add((predecessor, true));
		var predecessorStepper = new Stepper();
		predecessorStepper.Steps.Add(removalWithPredecessor);

		DebugStepHighlighter.TryResolve(predecessorStepper, stepLimit: 1, highlightStep: 0, predecessorLookup, out var endCaret)
			.Should().BeTrue("a removed node must fall back to a surviving predecessor");
		endCaret.Length.Should().Be(0);
		endCaret.Start.Should().Be(15, "with no successor, the caret sits at the predecessor's end");
		return Task.CompletedTask;
	}

	[AvaloniaTest]
	public Task DebugStepFilter_Keeps_Matches_And_The_Path_To_Them()
	{
		var converter = new DebugStepFilterConverter();
		var matchingLeaf = new Stepper.Node("3: Introduce query continuation");
		var otherLeaf = new Stepper.Node("4: Flatten switch section block");
		var group = new Stepper.Node("CombineQueryExpressions");
		group.Children.Add(matchingLeaf);
		group.Children.Add(otherLeaf);

		// An empty filter shows every row.
		Filter(group, "").Should().BeTrue();
		Filter(otherLeaf, "  ").Should().BeTrue();
		// A group survives because a descendant matches, keeping the path to the match.
		Filter(group, "continuation").Should().BeTrue();
		// The matching leaf survives, case-insensitively.
		Filter(matchingLeaf, "CONTINUATION").Should().BeTrue();
		// A sibling that neither matches nor leads to a match is hidden.
		Filter(otherLeaf, "continuation").Should().BeFalse();
		return Task.CompletedTask;

		bool Filter(Stepper.Node node, string filter)
			=> (bool)converter.Convert(new object?[] { node, filter }, typeof(bool), null, CultureInfo.InvariantCulture);
	}

	[AvaloniaTest]
	public Task DebugSteps_View_Loads_With_Filter_Applied()
	{
		// Guards the filter wiring in the XAML -- a MultiBinding inside a TreeViewItem style Setter
		// plus the RelativeSource lookups -- against a structural break that x:CompileBindings="False"
		// would not catch at build time. Realising the view with a populated tree and a live filter
		// must not throw.
		var vm = new DebugStepsPaneModel();
		var group = new Stepper.Node("CombineQueryExpressions");
		group.Children.Add(new Stepper.Node("3: Introduce query continuation"));
		group.Children.Add(new Stepper.Node("4: Flatten switch section block"));
		vm.Steps = new[] { group };
		vm.IsAvailable = true;

		var window = new Window { Width = 400, Height = 300, Content = new DebugSteps { DataContext = vm } };
		window.Show();
		vm.FilterText = "continuation";
		Dispatcher.UIThread.RunJobs();
		window.Close();
		return Task.CompletedTask;
	}

	// A replay highlight must land on the changed node, not merely be non-null: in bounds, not a
	// flood of the whole document, and (unless it is a zero-length removal caret) on rendered code
	// rather than whitespace. This is what keeps the ancestor fallback from silently widening every
	// highlight to the enclosing method undetected.
	static void AssertPreciseHighlight(DecompilerTabPageModel tab, string because)
	{
		tab.DebugStepHighlight.Should().NotBeNull(because);
		var range = tab.DebugStepHighlight!.Value;
		var text = tab.Text!;
		(range.Start >= 0 && range.Start + range.Length <= text.Length).Should()
			.BeTrue("the debug-step highlight must lie within the emitted document");
		range.Length.Should().BeLessThan(text.Length,
			"the highlight must mark a specific node, not flood the whole document");
		if (range.Length > 0)
		{
			text.Substring(range.Start, range.Length).Trim().Should()
				.NotBeEmpty("a non-caret highlight must cover rendered code, not just whitespace");
		}
	}

	[AvaloniaTest]
	public Task MarkNodeStart_Does_Not_Record_When_Node_Tracking_Is_Disabled()
	{
		var output = new AvaloniaEditTextOutput();
		var node = new object();

		output.MarkNodeStart(node);
		output.Write("statement;");
		output.MarkNodeEnd(node);

		output.NodeLookup.TryGetRange(node, out _).Should().BeFalse();
		return Task.CompletedTask;
	}

	[AvaloniaTest]
	public Task MarkNodeStart_Excludes_Leading_Indentation()
	{
		// A node opened at the start of an indented line must record its range from the first real
		// character, so the debug-step highlight does not extend across the indentation to column 0.
		var output = new AvaloniaEditTextOutput { EnableNodeTracking = true };
		output.Indent();
		output.WriteLine();

		var node = new object();
		output.MarkNodeStart(node);
		output.Write("statement;");
		output.MarkNodeEnd(node);

		output.NodeLookup.TryGetRange(node, out var range).Should().BeTrue();
		output.GetText().Substring(range.Start, range.Length).Should().Be("statement;");
		return Task.CompletedTask;
	}

	[AvaloniaTest]
	public Task MarkNodeEnd_Records_Nodes_Regardless_Of_Close_Order()
	{
		// Node spans are keyed by identity, so closing an outer node before the inner one it still
		// contains must not discard either range. A stack that popped by position would lose both.
		var output = new AvaloniaEditTextOutput { EnableNodeTracking = true };
		var outer = new object();
		var inner = new object();

		output.MarkNodeStart(outer);
		output.Write("a(");
		output.MarkNodeStart(inner);
		output.Write("b");
		output.MarkNodeEnd(outer);
		output.Write(")");
		output.MarkNodeEnd(inner);

		output.NodeLookup.TryGetRange(outer, out var outerRange).Should().BeTrue();
		output.GetText().Substring(outerRange.Start, outerRange.Length).Should().Be("a(b");
		output.NodeLookup.TryGetRange(inner, out var innerRange).Should().BeTrue();
		output.GetText().Substring(innerRange.Start, innerRange.Length).Should().Be("b)");
		return Task.CompletedTask;
	}

	[AvaloniaTest]
	public Task Pane_Reports_Not_Available_For_Languages_Without_Debug_Steps()
	{
		// The step tree only makes sense for IDebugStepProvider languages (C#, ILAst, Typed IL).
		// For the plain IL disassembler the pane must not keep showing the previous language's
		// stale step tree (whose commands would trigger pointless re-decompiles); it reports
		// unavailability so the view swaps in a "not available" message instead.
		var languageService = AppComposition.Current.GetExport<LanguageService>();
		var debugStepsVm = AppComposition.Current.GetExport<DebugStepsPaneModel>();

		languageService.CurrentLanguage = languageService.Languages.OfType<CSharpLanguage>().First();
		global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();
		debugStepsVm.IsAvailable.Should().BeTrue("C# provides debug steps");

		// Simulate a populated tree from the C# run, then flip to the disassembler language.
		debugStepsVm.Steps = new[] { new Stepper.Node("stale") };
		languageService.CurrentLanguage = languageService.Languages.OfType<ILLanguage>().First(l => l.Name == "IL");
		global::Avalonia.Threading.Dispatcher.UIThread.RunJobs();

		debugStepsVm.IsAvailable.Should().BeFalse("the IL disassembler provides no debug steps");
		debugStepsVm.Steps.Should().BeNull("the previous language's step tree must not linger");
		return Task.CompletedTask;
	}

	[AvaloniaTest]
	public async Task Window_Menu_Toggle_Surfaces_The_Default_Hidden_Debug_Steps_Pane()
	{
		// Repro of "Window > Debug Steps does nothing": the menu toggles ToolPaneMenuItem.IsPaneVisible,
		// which used factory.RestoreDockable — a no-op for a pane that is hidden by default (never
		// placed in the layout, so there's nothing to restore). Toggling it on must actually surface
		// the pane (ShowToolPane materialises it and creates its home dock).
		var window = AppComposition.Current.GetExport<MainWindow>();
		window.Show();
		var vm = (MainWindowViewModel)window.DataContext!;
		await vm.AssemblyTreeModel.WaitForAssembliesAsync(minimumCount: 1);

		var menuItem = vm.DockWorkspace.ToolPaneMenuItems
			.Single(p => p.Title == "Debug Steps");
		menuItem.IsPaneVisible.Should().BeFalse("Debug Steps is hidden by default");

		menuItem.IsPaneVisible = true;

		menuItem.IsPaneVisible.Should().BeTrue(
			"toggling Window > Debug Steps on must make the pane visible in the layout");
	}

}

#endif
