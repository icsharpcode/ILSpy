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

using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Input;
using Avalonia.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.Controls.TreeView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Controls;

[TestFixture]
public class SharpTreeViewTests
{
	sealed class TestNode : SharpTreeNode
	{
		readonly string text;
		public TestNode(string text, params TestNode[] children)
		{
			this.text = text;
			foreach (var child in children)
				Children.Add(child);
		}
		public override object Text => text;
	}

	static (Window window, SharpTreeView tree, TestNode root) Host()
	{
		var b1 = new TestNode("B1");
		var b = new TestNode("B", b1);
		var root = new TestNode("root", new TestNode("A"), b, new TestNode("C"));
		var tree = new SharpTreeView { ShowRoot = false, Root = root };
		var window = new Window { Content = tree, Width = 300, Height = 400 };
		window.Show();
		Dispatcher.UIThread.RunJobs();
		return (window, tree, root);
	}

	[AvaloniaTest]
	public void Flattens_Visible_Nodes_And_Expand_Collapse_Updates_Row_Count()
	{
		var (_, tree, root) = Host();
		var b = (TestNode)root.Children[1];

		tree.ItemCount.Should().Be(3, "A, B (collapsed), C are the visible top-level rows");

		b.IsExpanded = true;
		Dispatcher.UIThread.RunJobs();
		tree.ItemCount.Should().Be(4, "expanding B reveals B1");

		b.IsExpanded = false;
		Dispatcher.UIThread.RunJobs();
		tree.ItemCount.Should().Be(3, "collapsing B hides B1 again");
	}

	[AvaloniaTest]
	public void Shift_Down_Extends_And_Shift_Up_Shrinks_From_The_Anchor()
	{
		// The whole reason for the rewrite: native ListBox anchor selection both extends AND
		// shrinks a shift-range -- the thing ProDataGrid's additive SetRowsSelection could not do.
		var nodes = Enumerable.Range(0, 5).Select(i => new TestNode(((char)('A' + i)).ToString())).ToArray();
		var root = new TestNode("root", nodes);
		var tree = new SharpTreeView { ShowRoot = false, Root = root };
		var window = new Window { Content = tree, Width = 300, Height = 400 };
		window.Show();
		Dispatcher.UIThread.RunJobs();

		tree.SelectedItem = nodes[0];
		Dispatcher.UIThread.RunJobs();
		(tree.ContainerFromItem(nodes[0]))?.Focus();
		Dispatcher.UIThread.RunJobs();

		window.KeyPress(Key.Down, RawInputModifiers.Shift, PhysicalKey.ArrowDown, null);
		Dispatcher.UIThread.RunJobs();
		window.KeyPress(Key.Down, RawInputModifiers.Shift, PhysicalKey.ArrowDown, null);
		Dispatcher.UIThread.RunJobs();
		tree.SelectedItems!.Cast<SharpTreeNode>().Should()
			.BeEquivalentTo(new[] { nodes[0], nodes[1], nodes[2] }, "Shift+Down twice extends A..C");

		window.KeyPress(Key.Up, RawInputModifiers.Shift, PhysicalKey.ArrowUp, null);
		Dispatcher.UIThread.RunJobs();
		tree.SelectedItems!.Cast<SharpTreeNode>().Should()
			.BeEquivalentTo(new[] { nodes[0], nodes[1] }, "Shift+Up shrinks the range back toward the anchor (A,B)");
	}

	[AvaloniaTest]
	public void Type_Ahead_Jumps_To_The_Visible_Node_Matching_The_Typed_Prefix()
	{
		var apple = new TestNode("Apple");
		var banana = new TestNode("Banana");
		var cherry = new TestNode("Cherry");
		var cranberry = new TestNode("Cranberry");
		var root = new TestNode("root", apple, banana, cherry, cranberry);
		var tree = new SharpTreeView { ShowRoot = false, Root = root };
		var window = new Window { Content = tree, Width = 300, Height = 400 };
		window.Show();
		Dispatcher.UIThread.RunJobs();

		// "cra": 'c' lands on Cherry, 'r' has to search forward to Cranberry, 'a' stays.
		foreach (char ch in "cra")
		{
			tree.RaiseEvent(new TextInputEventArgs {
				RoutedEvent = InputElement.TextInputEvent,
				Text = ch.ToString(),
				Source = tree,
			});
			Dispatcher.UIThread.RunJobs();
		}

		tree.SelectedItem.Should().BeSameAs(cranberry, "type-ahead refines forward to the Cr* match");
	}

	[AvaloniaTest]
	public void Selecting_A_Row_Sets_The_Node_IsSelected()
	{
		var (_, tree, root) = Host();
		var a = (TestNode)root.Children[0];
		var c = (TestNode)root.Children[2];

		tree.SelectedItem = a;
		Dispatcher.UIThread.RunJobs();
		a.IsSelected.Should().BeTrue();

		tree.SelectedItem = c;
		Dispatcher.UIThread.RunJobs();
		c.IsSelected.Should().BeTrue();
		a.IsSelected.Should().BeFalse("moving the selection clears the old node's flag");
	}
}
