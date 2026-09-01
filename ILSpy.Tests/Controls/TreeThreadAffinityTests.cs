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
using System.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpyX.TreeView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Controls;

[TestFixture]
public class TreeThreadAffinityTests
{
	sealed class TestNode : SharpTreeNode
	{
		readonly string text;
		public TestNode(string text) => this.text = text;
		public override object Text => text;
		public override string ToString() => text;
	}

	bool oldFailFast;
	string? oldLogFilePath;

	[SetUp]
	public void SetUp()
	{
		oldFailFast = TreeThreadAffinity.FailFast;
		oldLogFilePath = TreeThreadAffinity.LogFilePath;
		// Tests must not append to whatever log an exploratory session is collecting, and must not
		// inherit the mode from the environment: the ones that need a throw opt in individually.
		TreeThreadAffinity.LogFilePath = null;
		TreeThreadAffinity.FailFast = false;
		TreeThreadAffinity.Clear();
	}

	[TearDown]
	public void TearDown()
	{
		TreeThreadAffinity.FailFast = oldFailFast;
		TreeThreadAffinity.LogFilePath = oldLogFilePath;
		TreeThreadAffinity.Clear();
	}

	/// <summary>
	/// Runs <paramref name="action"/> to completion on a dedicated thread and returns whatever it
	/// threw. Join() makes this deterministic: no sleeps, no polling.
	/// </summary>
	static Exception? RunOnOtherThread(Action action)
	{
		Exception? error = null;
		var thread = new Thread(() => {
			try
			{
				action();
			}
			catch (Exception ex)
			{
				error = ex;
			}
		}) { Name = "affinity-test-worker", IsBackground = true };
		thread.Start();
		thread.Join();
		return error;
	}

#if DEBUG

	[Test]
	public void ChildrenAddFromNonOwningThread_IsReported()
	{
		TreeThreadAffinity.FailFast = true;
		var root = new TestNode("root");
		root.SetOwner();

		var error = RunOnOtherThread(() => root.Children.Add(new TestNode("child")));

		error.Should().BeOfType<InvalidOperationException>();
		error!.Message.Should().Contain("Children.Add").And.Contain("\"root\"");
	}

	[Test]
	public void IsExpandedFromNonOwningThread_IsReported()
	{
		TreeThreadAffinity.FailFast = true;
		var root = new TestNode("root");
		root.Children.Add(new TestNode("child"));
		root.SetOwner();

		var error = RunOnOtherThread(() => root.IsExpanded = true);

		error.Should().BeOfType<InvalidOperationException>();
		error!.Message.Should().Contain(nameof(SharpTreeNode.IsExpanded));
	}

	[Test]
	public void IsHiddenFromNonOwningThread_IsReported()
	{
		TreeThreadAffinity.FailFast = true;
		var root = new TestNode("root");
		var child = new TestNode("child");
		root.Children.Add(child);
		root.SetOwner();

		var error = RunOnOtherThread(() => child.IsHidden = true);

		error.Should().BeOfType<InvalidOperationException>();
		error!.Message.Should().Contain(nameof(SharpTreeNode.IsHidden));
	}

	[Test]
	public void UnownedTree_IsNotChecked()
	{
		TreeThreadAffinity.FailFast = true;
		var root = new TestNode("root");

		var error = RunOnOtherThread(() => {
			root.Children.Add(new TestNode("child"));
			root.IsExpanded = true;
		});

		error.Should().BeNull();
		TreeThreadAffinity.Violations.Should().BeEmpty();
	}

	[Test]
	public void SubtreeBuiltOffThreadThenPublishedByOwner_IsNotReported()
	{
		// The pattern the analyzers use: assemble a subtree on a worker, hand it to the owning
		// thread, attach it there.
		TreeThreadAffinity.FailFast = true;
		var root = new TestNode("root");
		root.SetOwner();

		TestNode? built = null;
		var error = RunOnOtherThread(() => {
			built = new TestNode("built");
			built.Children.Add(new TestNode("grandchild"));
			built.IsExpanded = true;
		});

		error.Should().BeNull();
		root.Children.Add(built!);
		TreeThreadAffinity.Violations.Should().BeEmpty();
	}

	[Test]
	public void OwnershipIsInheritedByChildrenAttachedLater()
	{
		TreeThreadAffinity.FailFast = true;
		var root = new TestNode("root");
		root.SetOwner();
		var child = new TestNode("child");
		root.Children.Add(child);

		var error = RunOnOtherThread(() => child.Children.Add(new TestNode("grandchild")));

		error.Should().BeOfType<InvalidOperationException>();
		error!.Message.Should().Contain("\"child\"");
	}

	[Test]
	public void AttachingSubtreeOwnedByAnotherThread_IsReportedOnceThenInherits()
	{
		var root = new TestNode("root");
		root.SetOwner();
		var foreign = new TestNode("foreign");
		RunOnOtherThread(() => foreign.SetOwner()).Should().BeNull();

		root.Children.Add(foreign);

		TreeThreadAffinity.Violations.Should().ContainSingle()
			.Which.Operation.Should().Contain("owned by another thread");

		// The conflicting owner is dropped, so the subtree now inherits the root's owner: further
		// mutation from the owning thread is clean, and from any other thread is a violation.
		TreeThreadAffinity.Clear();
		foreign.Children.Add(new TestNode("grandchild"));
		TreeThreadAffinity.Violations.Should().BeEmpty();

		TreeThreadAffinity.FailFast = true;
		RunOnOtherThread(() => foreign.Children.Add(new TestNode("other")))
			.Should().BeOfType<InvalidOperationException>();
	}

	[Test]
	public void RepeatedViolationsFromOneCallSite_AreDeduplicatedAndCounted()
	{
		var root = new TestNode("root");
		root.SetOwner();

		var error = RunOnOtherThread(() => {
			for (int i = 0; i < 5; i++)
				root.Children.Add(new TestNode("child" + i));
		});

		error.Should().BeNull();
		var violation = TreeThreadAffinity.Violations.Should().ContainSingle().Subject;
		violation.Count.Should().Be(5);
		violation.StackTrace.Should().Contain(nameof(RepeatedViolationsFromOneCallSite_AreDeduplicatedAndCounted));
		violation.ActualThread.Should().Contain("affinity-test-worker");
	}

	[Test]
	public void OwnershipHandoffFromNonOwningThread_IsReported()
	{
		TreeThreadAffinity.FailFast = true;
		var root = new TestNode("root");
		root.SetOwner();

		var error = RunOnOtherThread(() => root.SetOwner());

		error.Should().BeOfType<InvalidOperationException>();
		error!.Message.Should().Contain(nameof(SharpTreeNode.SetOwner));
	}

#else

	[Test]
	public void ThreadAffinityChecksAreDebugOnly()
	{
		Assert.Ignore("SharpTreeNode thread-affinity checking is compiled out in release builds.");
	}

#endif
}
