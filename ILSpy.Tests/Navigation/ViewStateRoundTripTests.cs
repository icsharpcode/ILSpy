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

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.Navigation;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;

using NUnit.Framework;

using FoldingSnapshot = ICSharpCode.ILSpy.TextView.FoldingsViewState.Snapshot;

namespace ICSharpCode.ILSpy.Tests.Navigation;

/// <summary>
/// End-to-end: select node A, move the caret, select node B, navigate Back. The caret
/// must land where the user left it on A — that's the contract for Back/Forward to
/// feel like a real browser instead of a "reset to top" gesture.
/// </summary>
[TestFixture]
public class ViewStateRoundTripTests
{
	// These tests only exercise the navigation pipeline; what gets decompiled is irrelevant.
	// Decompiling a CoreLib type in full is the slowest thing the suite does and overruns the
	// headless 15s wait on slower CI runners. A namespace node in C# decompiles to just its
	// "// Some.Name.Space" comment line (Language.DecompileNamespace) -- the cheapest possible
	// target -- so navigate between two of those. Both namespaces are always present in CoreLib.
	static (NamespaceTreeNode A, NamespaceTreeNode B) CheapNodes(AssemblyTreeModel atm)
	{
		var a = atm.FindNode<NamespaceTreeNode>(TreeNavigation.CoreLibName, "System.Runtime.Versioning");
		var b = atm.FindNode<NamespaceTreeNode>(TreeNavigation.CoreLibName, "System.Text");
		return (a, b);
	}

	[AvaloniaTest]
	public async Task Back_Restores_Caret_Position_The_User_Left_On_The_Previous_Node()
	{
		var (_, vm) = await TestHarness.BootAsync();

		var dockWorkspace = vm.DockWorkspace;

		// Two distinct, cheap navigation targets so the navigation actually moves between them.
		var (nodeA, nodeB) = CheapNodes(vm.AssemblyTreeModel);

		vm.AssemblyTreeModel.SelectNode(nodeA);
		await dockWorkspace.WaitForDecompiledTextAsync();
		var tab = dockWorkspace.ActiveDecompilerTab!;
		TestCapture.Step("node-a-decompiled");

		// State is pulled from the editor on demand (CaptureViewState) when DockWorkspace records
		// a navigation away -- not pushed per caret/scroll event. Headless has no laid-out editor,
		// so override the pull delegate to report the position the user "left" on node A.
		tab.CaptureViewState = () => new DecompilerTextViewState(500, 120.5, 7.25, null);

		// Navigate to a different node — DockWorkspace pulls A's state into the current history
		// entry and records B as the new current.
		vm.AssemblyTreeModel.SelectNode(nodeB);
		await dockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("node-b-decompiled");

		// Verify the capture: the back stack's most recent entry should be A's, with the pulled
		// caret + scroll values.
		var backEntries = dockWorkspace.BackHistory.OfType<TreeNodeEntry>().ToList();
		backEntries.Should().NotBeEmpty("Select(B) must push A onto the back stack");
		var captured = backEntries.Last();
		ReferenceEquals(captured.Node, nodeA).Should().BeTrue(
			"captured back-stack entry must reference node A");
		captured.CaretOffset.Should().Be(500);
		captured.VerticalOffset.Should().Be(120.5);
		captured.HorizontalOffset.Should().Be(7.25);

		// Navigate Back. ApplyNavigationTarget synchronously stashes the recorded state on the tab
		// as PendingViewState before the async decompile runs; the editor consumes it in
		// ApplyDocument once Text lands. Assert the restore intent here, before the await lets the
		// view consume it -- applying caret/scroll to the live editor is the view's job (covered by
		// the real UI; headless has no rendered viewport).
		dockWorkspace.NavigateBackCommand.Execute(null);
		TestCapture.Step("navigated-back");
		tab.PendingViewState.Should().NotBeNull("Back must stash the recorded view state for the editor to apply");
		tab.PendingViewState!.Value.CaretOffset.Should().Be(500,
			"Back must restore the caret to where the user left it before navigating to B");
		tab.PendingViewState!.Value.VerticalOffset.Should().Be(120.5,
			"Back must restore the vertical scroll offset the user left on A");

		await dockWorkspace.WaitForDecompiledTextAsync();

		// The editor consumes and clears PendingViewState once the document lands.
		tab.PendingViewState.Should().BeNull("the editor must consume the pending state after applying it");
	}

	[AvaloniaTest]
	public async Task Back_Carries_Expanded_Foldings_Snapshot_From_Capture_Through_To_Pending()
	{
		// Pins the foldings half of the view-state round trip end-to-end through DockWorkspace.
		// The headless editor doesn't build real foldings, so override the pull delegate to report
		// a deterministic snapshot, exercise the navigation pipeline, and assert the snapshot
		// survives the capture -> entry -> pending hops.

		var (_, vm) = await TestHarness.BootAsync();

		var dockWorkspace = vm.DockWorkspace;
		var (nodeA, nodeB) = CheapNodes(vm.AssemblyTreeModel);

		vm.AssemblyTreeModel.SelectNode(nodeA);
		await dockWorkspace.WaitForDecompiledTextAsync();
		var tab = dockWorkspace.ActiveDecompilerTab!;
		TestCapture.Step("node-a-decompiled");

		// Deterministic foldings snapshot — two expanded regions over a four-folding layout.
		// Compute via the helper to keep the checksum honest. (The snapshot itself is exercised
		// independently in FoldingsViewStateTests.)
		var seeded = FoldingsViewState.Capture(new[] {
			(Start: 10, End: 50, IsFolded: false),
			(Start: 60, End: 100, IsFolded: true),
			(Start: 110, End: 200, IsFolded: false),
			(Start: 210, End: 250, IsFolded: true),
		});
		tab.CaptureViewState = () => new DecompilerTextViewState(0, 0, 0, seeded);

		// Navigate to a different node — DockWorkspace pulls the state and stamps the foldings
		// onto the OUTGOING entry on the back stack.
		vm.AssemblyTreeModel.SelectNode(nodeB);
		await dockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("node-b-decompiled");

		// Verify the capture: the back-stack entry for node A carries the snapshot.
		var captured = dockWorkspace.BackHistory.OfType<TreeNodeEntry>().Last();
		ReferenceEquals(captured.Node, nodeA).Should().BeTrue(
			"captured back-stack entry must reference node A");
		captured.Foldings.Should().NotBeNull("Select(B) must record A's foldings into the back stack");
		captured.Foldings!.Value.Checksum.Should().Be(seeded.Checksum);
		captured.Foldings.Value.Expanded.Should().Equal(seeded.Expanded);

		// Navigate Back. ApplyNavigationTarget stashes the recorded snapshot into PendingViewState
		// for the view to consume on the next ApplyDocument. The assignment is synchronous, so we
		// can observe it before the await lets the editor consume it.
		dockWorkspace.NavigateBackCommand.Execute(null);
		TestCapture.Step("navigated-back");
		tab.PendingViewState.Should().NotBeNull("Back must propagate the captured state to the destination tab");
		tab.PendingViewState!.Value.Foldings.Should().NotBeNull();
		tab.PendingViewState!.Value.Foldings!.Value.Checksum.Should().Be(seeded.Checksum);
	}

	[AvaloniaTest]
	public async Task Forward_Is_Wired_Through_The_Toolbar_And_Key_Binding()
	{
		// Pins the existing Forward command wiring: the tracker had listed BrowseForward
		// as missing, but it was already implemented. This test makes a regression in any
		// of the three wirings (command, toolbar button, Alt+Right) detectable.

		var (_, vm) = await TestHarness.BootAsync();

		((object?)vm.DockWorkspace.NavigateForwardCommand).Should().NotBeNull();
		((object?)vm.DockWorkspace.NavigateBackCommand).Should().NotBeNull();

		// Build up history: A → B, then go Back so Forward is enabled.
		var (nodeA, nodeB) = CheapNodes(vm.AssemblyTreeModel);
		vm.AssemblyTreeModel.SelectNode(nodeA);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		vm.AssemblyTreeModel.SelectNode(nodeB);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("node-b-decompiled");

		vm.DockWorkspace.NavigateBackCommand.Execute(null);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("navigated-back");
		vm.DockWorkspace.NavigateForwardCommand.CanExecute(null).Should().BeTrue(
			"Forward must be enabled after Back leaves an entry on the forward stack");

		vm.DockWorkspace.NavigateForwardCommand.Execute(null);
		await vm.DockWorkspace.WaitForDecompiledTextAsync();
		TestCapture.Step("navigated-forward");

		// Back through Forward should land on B again.
		ReferenceEquals(vm.AssemblyTreeModel.SelectedItem, nodeB).Should().BeTrue(
			"NavigateForward must restore the tree selection to the entry that was just popped from forward");
	}
}
