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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.TreeView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Controls;

[TestFixture]
public class FlatListTreeNodeTests
{
	sealed class TestNode : SharpTreeNode
	{
		readonly string text;
		public TestNode(string text) => this.text = text;
		public override object Text => text;
		public override string ToString() => text;
	}

	[Test]
	public void GetNodeByVisibleIndex_WalkingPastTheEnd_ThrowsNamingIndexAndLength()
	{
		var root = new TestNode("root");
		root.Children.Add(new TestNode("child"));
		root.IsExpanded = true;
		var listRoot = root.GetListRoot();
		listRoot.GetTotalListLength().Should().Be(2);

		// A restructure that happened under a reader leaves the augmented length disagreeing with
		// the structure it describes: the length says there is a node at this index, the descent
		// runs out of nodes before reaching it.
		listRoot.totalListLength = 5;

		var error = Assert.Throws<InvalidOperationException>(
			() => SharpTreeNode.GetNodeByVisibleIndex(listRoot, 4));

		error!.Message.Should().Contain("4").And.Contain("5");
	}

	[Test]
	public void GetNodeByVisibleIndex_WithinTheList_ReturnsTheNodeAtThatIndex()
	{
		var root = new TestNode("root");
		var child = new TestNode("child");
		root.Children.Add(child);
		root.IsExpanded = true;
		var listRoot = root.GetListRoot();

		Assert.That(SharpTreeNode.GetNodeByVisibleIndex(listRoot, 0), Is.SameAs(root));
		Assert.That(SharpTreeNode.GetNodeByVisibleIndex(listRoot, 1), Is.SameAs(child));
	}

	[Test]
	public void Move_ReordersChildrenAndRaisesOneMoveEvent()
	{
		var root = new TestNode("root");
		var a = new TestNode("a");
		var b = new TestNode("b");
		var c = new TestNode("c");
		root.Children.AddRange(new[] { a, b, c });
		var events = new List<NotifyCollectionChangedEventArgs>();
		root.Children.CollectionChanged += (_, e) => events.Add(e);

		root.Children.Move(2, 0);

		root.Children.Should().Equal(c, a, b);
		events.Should().ContainSingle();
		events[0].Action.Should().Be(NotifyCollectionChangedAction.Move);
		events[0].OldStartingIndex.Should().Be(2);
		events[0].NewStartingIndex.Should().Be(0);
		events[0].OldItems!.Cast<SharpTreeNode>().Should().Equal(c);
		events[0].NewItems!.Cast<SharpTreeNode>().Should().Equal(c);
	}

	[Test]
	public void Move_ToSameIndex_DoesNothing()
	{
		var root = new TestNode("root");
		var a = new TestNode("a");
		var b = new TestNode("b");
		root.Children.AddRange(new[] { a, b });
		var events = new List<NotifyCollectionChangedEventArgs>();
		root.Children.CollectionChanged += (_, e) => events.Add(e);

		root.Children.Move(1, 1);

		root.Children.Should().Equal(a, b);
		events.Should().BeEmpty();
	}

	[Test]
	public void Move_UpdatesTheFlattenedOrder()
	{
		var root = new TestNode("root");
		var a = new TestNode("a");
		var b = new TestNode("b");
		var c = new TestNode("c");
		root.Children.AddRange(new[] { a, b, c });
		root.IsExpanded = true;
		var flattener = new TreeFlattener(root, includeRoot: true);

		root.Children.Move(2, 0);

		Flatten(flattener).Should().Equal(root, c, a, b);
	}

	[Test]
	public void Move_OfAnExpandedNode_MovesItsWholeRun()
	{
		var root = new TestNode("root");
		var a = new TestNode("a");
		var a1 = new TestNode("a1");
		var a2 = new TestNode("a2");
		var b = new TestNode("b");
		a.Children.AddRange(new[] { a1, a2 });
		root.Children.AddRange(new[] { a, b });
		root.IsExpanded = true;
		a.IsExpanded = true;
		var flattener = new TreeFlattener(root, includeRoot: true);
		Flatten(flattener).Should().Equal(root, a, a1, a2, b);

		root.Children.Move(0, 1);

		Flatten(flattener).Should().Equal(root, b, a, a1, a2);
	}

	[Test]
	public void Move_RaisesOneRangedMoveOnTheFlattener()
	{
		var root = new TestNode("root");
		var a = new TestNode("a");
		var a1 = new TestNode("a1");
		var b = new TestNode("b");
		a.Children.Add(a1);
		root.Children.AddRange(new[] { a, b });
		root.IsExpanded = true;
		a.IsExpanded = true;
		var flattener = new TreeFlattener(root, includeRoot: true);
		var events = new List<NotifyCollectionChangedEventArgs>();
		flattener.CollectionChanged += (_, e) => events.Add(e);

		// root, a, a1, b  ->  root, b, a, a1: the run [a, a1] moves from index 1 to index 2.
		root.Children.Move(0, 1);

		events.Should().ContainSingle();
		events[0].Action.Should().Be(NotifyCollectionChangedAction.Move);
		events[0].OldItems!.Cast<SharpTreeNode>().Should().Equal(a, a1);
		events[0].OldStartingIndex.Should().Be(1);
		// A forward move of a multi-row run reports the row the run ends on, which is how the
		// consumer re-inserts it; a1 is the last row of [a, a1] and ends up at index 3.
		events[0].NewStartingIndex.Should().Be(3);
	}

	static List<object> Flatten(TreeFlattener flattener)
	{
		var result = new List<object>();
		for (int i = 0; i < flattener.Count; i++)
			result.Add(flattener[i]);
		return result;
	}
}
