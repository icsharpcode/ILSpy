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

using Avalonia.Controls;
using Avalonia.Headless.NUnit;
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.TreeView;
using ICSharpCode.ILSpyX.TreeView.PlatformAbstractions;

using ICSharpCode.ILSpy.Controls.TreeView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Controls;

// External Explorer drops landed only when the cursor was over a row -- ResolveDropTarget bails
// when e.Source has no SharpTreeViewItem ancestor, so a drop into the empty space below the last
// row never reached SharpTreeNode.InternalDrop. In the assembly tree that meant "drop a DLL onto
// an empty list" silently did nothing. The fallback is the same as what the WPF SharpTreeView
// did: target the root node at its end with DropPlace.Inside, then let CanDrop decide.
[TestFixture]
public class SharpTreeViewEmptySpaceDropTests
{
	sealed class RootNode : SharpTreeNode
	{
		public bool AcceptDrops { get; init; }
		public override object Text => "root";
		public override bool CanDrop(IPlatformDragEventArgs e, int index) => AcceptDrops;
		public override void Drop(IPlatformDragEventArgs e, int index) { }
	}

	static (Window window, SharpTreeView tree) Host(SharpTreeNode? root)
	{
		var tree = new SharpTreeView { ShowRoot = false, Root = root };
		var window = new Window { Content = tree, Width = 300, Height = 400 };
		window.Show();
		Dispatcher.UIThread.RunJobs();
		return (window, tree);
	}

	[AvaloniaTest]
	public void Empty_space_drop_resolves_to_root_inside_at_end()
	{
		var root = new RootNode { AcceptDrops = true };
		root.Children.Add(new RootNode { AcceptDrops = false });
		var (_, tree) = Host(root);

		var target = tree.ResolveEmptySpaceDropTarget();

		target.Should().NotBeNull();
		ReferenceEquals(target!.Value.Node, root).Should().BeTrue();
		target.Value.Index.Should().Be(root.Children.Count);
		target.Value.Place.Should().Be(SharpTreeView.DropPlace.Inside);
		target.Value.Item.Should().BeNull();
	}

	[AvaloniaTest]
	public void Empty_space_drop_returns_null_when_no_root()
	{
		var (_, tree) = Host(root: null);

		tree.ResolveEmptySpaceDropTarget().Should().BeNull();
	}

	// "Drop directly onto a row" lands DropPlace.Inside on the row's own node; most concrete
	// SharpTreeNodes inherit the base CanDrop (returns false) so the row would refuse the payload.
	// PickAcceptingTarget falls back to the root in that case so the file still gets opened.
	[AvaloniaTest]
	public void Drop_on_rejecting_row_falls_back_to_root()
	{
		var root = new RootNode { AcceptDrops = true };
		var child = new RootNode { AcceptDrops = false };
		root.Children.Add(child);
		var (_, tree) = Host(root);

		var initial = new SharpTreeView.DropTarget(child, child.Children.Count,
			SharpTreeView.DropPlace.Inside, Item: null);
		var picked = tree.PickAcceptingTarget(new AvaloniaPlatformDragEventArgs(new AvaloniaDataObject()), initial);

		picked.Should().NotBeNull();
		ReferenceEquals(picked!.Value.Node, root).Should().BeTrue();
		picked.Value.Index.Should().Be(root.Children.Count);
	}

	[AvaloniaTest]
	public void Drop_on_accepting_row_keeps_the_row_target()
	{
		var root = new RootNode { AcceptDrops = true };
		var child = new RootNode { AcceptDrops = true };
		root.Children.Add(child);
		var (_, tree) = Host(root);

		var initial = new SharpTreeView.DropTarget(child, child.Children.Count,
			SharpTreeView.DropPlace.Inside, Item: null);
		var picked = tree.PickAcceptingTarget(new AvaloniaPlatformDragEventArgs(new AvaloniaDataObject()), initial);

		picked.Should().NotBeNull();
		ReferenceEquals(picked!.Value.Node, child).Should().BeTrue();
	}

	[AvaloniaTest]
	public void Drop_with_no_target_and_no_root_returns_null()
	{
		var (_, tree) = Host(root: null);

		tree.PickAcceptingTarget(new AvaloniaPlatformDragEventArgs(new AvaloniaDataObject()), initial: null)
			.Should().BeNull();
	}
}
