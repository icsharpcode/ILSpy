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

using ILSpy.Controls.TreeView;

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
