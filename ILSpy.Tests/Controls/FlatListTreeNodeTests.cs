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
}
