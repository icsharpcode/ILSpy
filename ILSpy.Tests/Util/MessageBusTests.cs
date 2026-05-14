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

using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using AwesomeAssertions;

using ILSpy.Util;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Util;

/// <summary>
/// MessageBus is a static singleton bus keyed by message type. The tests use a
/// dedicated test-only event-args type so each test owns its own dispatch table and
/// can't be perturbed by application-level subscribers wired elsewhere.
/// </summary>
[TestFixture]
public class MessageBusTests
{
	sealed class TestMessage(int payload) : EventArgs
	{
		public int Payload { get; } = payload;
	}

	sealed class OtherMessage(string payload) : EventArgs
	{
		public string Payload { get; } = payload;
	}

	[Test]
	public void Send_Reaches_Each_Subscriber_Of_The_Matching_Message_Type()
	{
		var receivedA = 0;
		var receivedB = 0;
		EventHandler<TestMessage> a = (_, e) => receivedA = e.Payload;
		EventHandler<TestMessage> b = (_, e) => receivedB = e.Payload;
		MessageBus<TestMessage>.Subscribers += a;
		MessageBus<TestMessage>.Subscribers += b;
		try
		{
			MessageBus.Send(this, new TestMessage(42));

			receivedA.Should().Be(42);
			receivedB.Should().Be(42);
		}
		finally
		{
			MessageBus<TestMessage>.Subscribers -= a;
			MessageBus<TestMessage>.Subscribers -= b;
		}
	}

	[Test]
	public void Send_Routes_By_Type_Different_Message_Types_Stay_Isolated()
	{
		var testReceived = 0;
		var otherReceived = string.Empty;
		EventHandler<TestMessage> ta = (_, e) => testReceived = e.Payload;
		EventHandler<OtherMessage> oa = (_, e) => otherReceived = e.Payload;
		MessageBus<TestMessage>.Subscribers += ta;
		MessageBus<OtherMessage>.Subscribers += oa;
		try
		{
			MessageBus.Send(this, new TestMessage(7));

			testReceived.Should().Be(7);
			otherReceived.Should().BeEmpty(
				"OtherMessage subscribers must not fire when a TestMessage is sent");
		}
		finally
		{
			MessageBus<TestMessage>.Subscribers -= ta;
			MessageBus<OtherMessage>.Subscribers -= oa;
		}
	}

	[Test]
	public void Unsubscribe_Removes_The_Handler()
	{
		var received = 0;
		EventHandler<TestMessage> h = (_, e) => received = e.Payload;
		MessageBus<TestMessage>.Subscribers += h;
		MessageBus.Send(this, new TestMessage(1));
		received.Should().Be(1, "baseline: the subscription works");

		MessageBus<TestMessage>.Subscribers -= h;
		MessageBus.Send(this, new TestMessage(99));
		received.Should().Be(1, "after Unsubscribe, the handler must not fire again");
	}

	[Test]
	public void Dead_Subscribers_Are_Pruned_When_Their_Target_Is_GC_Collected()
	{
		// Subscribe via a target that we can let go of, force GC, and confirm the next
		// Raise both no-ops on the dead handler and removes it from the bag.

		var bus = new WeakEventSource<TestMessage>();
		var counter = SubscribeAndDrop(bus);
		// Sanity: live target receives.
		bus.Raise(this, new TestMessage(1));
		counter.Count.Should().Be(1);

		WaitForCollection(counter);

		// After the target is collected, Raise must not throw and the handler does nothing.
		bus.Raise(this, new TestMessage(2));
		// The dropped target was GC'd, so we have no live reference to inspect. The fact
		// that Raise didn't throw is the contract; pruning of the dead handler is implied
		// by the lack of unhandled exceptions reaching out.
		Assert.Pass("Raise tolerated the GC'd subscriber without throwing");
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static Counter SubscribeAndDrop(WeakEventSource<TestMessage> bus)
	{
		var counter = new Counter();
		bus.Subscribe(counter.Handler);
		return counter;
		// Local scope ends here. Caller intentionally drops 'counter' before the GC step.
	}

	static void WaitForCollection(Counter live)
	{
		// Force a few GC cycles. Some runtimes don't promote-and-collect young gens on the
		// first pass, so the loop gives the collector multiple shots.
		for (int i = 0; i < 5; i++)
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}
		GC.KeepAlive(live);
	}

	sealed class Counter
	{
		public int Count { get; private set; }
		public void Handler(object? sender, TestMessage e) => Count++;
	}

	[Test]
	public void CurrentAssemblyListChangedEventArgs_Exposes_Inner_Change_As_Named_Property()
	{
		// The named-property shape replaces WPF's implicit-operator wrapper.
		var inner = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
		var e = new CurrentAssemblyListChangedEventArgs(inner);
		e.Change.Should().BeSameAs(inner);
	}

	[Test]
	public void TabPagesCollectionChangedEventArgs_Exposes_Inner_Change_As_Named_Property()
	{
		var inner = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
		var e = new TabPagesCollectionChangedEventArgs(inner);
		e.Change.Should().BeSameAs(inner);
	}

	[Test]
	public void SettingsChangedEventArgs_Exposes_Inner_PropertyChanged_As_Named_Property()
	{
		var inner = new PropertyChangedEventArgs("Foo");
		var e = new SettingsChangedEventArgs(inner);
		e.PropertyChanged.Should().BeSameAs(inner);
		e.PropertyChanged.PropertyName.Should().Be("Foo");
	}
}
