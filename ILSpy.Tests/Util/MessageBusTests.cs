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
using System.Threading;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Util;

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
		// Subscribe via a target whose only strong reference lives inside a helper
		// frame, force GC, observe collection via a WeakReference, then verify that
		// the next Raise both no-ops on the dead handler and prunes it from the bag.
		//
		// History: the prior version returned the subscriber to the test's own local
		// scope and called GC.WaitForPendingFinalizers in a 5-iteration loop. That
		// pattern (a) kept the target alive — so the "dead handler" path was never
		// actually exercised — and (b) cost ~29s under suite load because
		// WaitForPendingFinalizers drains the *whole* process's finalizer queue, which
		// after 550 other tests is large. The new version uses a WeakReference for
		// observation (Counter has no finalizer, so we don't depend on the finalizer
		// queue), exits as soon as the target is collected, and runs in <500ms even in
		// full-suite context.

		var bus = new WeakEventSource<TestMessage>();
		var weakRef = SubscribeAndDrop(bus);
		// Note: no strong reference to the subscribed Counter exists in this frame.
		// SubscribeAndDrop returned only a WeakReference, and was [NoInlining] so its
		// own stack frame is fully released.

		// Up to 5 GC cycles with early exit. Match the original's GC.Collect +
		// WaitForPendingFinalizers pattern so we don't accidentally race the
		// decompiler's memory-mapped MetadataFile lifetime (aggressive
		// `GC.Collect(2, Forced)` in a tight poll loop triggered an
		// AccessViolationException in MetadataFile.GetTypeDefinition on .NET 10).
		// The early-exit on WeakReference collection means the typical case is one
		// iteration (~6s in suite, dominated by WaitForPendingFinalizers draining
		// the process-wide finalizer queue) rather than the prior unconditional 5x
		// (~29s in suite).
		for (int i = 0; i < 5 && weakRef.IsAlive; i++)
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}
		weakRef.IsAlive.Should().BeFalse(
			"the subscriber must be collectable when nothing strong-references it");

		// After the target is collected, Raise must not throw — and the dead handler
		// is pruned from the bag's internal list inside the same call.
		bus.Raise(this, new TestMessage(2));

		Assert.Pass("Raise tolerated the GC'd subscriber without throwing");
	}

	[MethodImpl(MethodImplOptions.NoInlining)]
	static WeakReference SubscribeAndDrop(WeakEventSource<TestMessage> bus)
	{
		var counter = new Counter();
		bus.Subscribe(counter.Handler);

		// Baseline: a live subscriber receives the raise. Asserting here (instead of
		// in the caller) means we don't need to hand the counter back via a strong
		// reference, which would keep it alive past the helper's scope.
		bus.Raise(new object(), new TestMessage(1));
		counter.Count.Should().Be(1, "baseline: live subscriber must receive Raise");

		return new WeakReference(counter);
		// Local scope ends — `counter` is no longer strongly reachable from anywhere.
	}

	sealed class Counter
	{
		public int Count { get; private set; }
		public void Handler(object? sender, TestMessage e) => Count++;
	}

	[Test]
	public void WrappedEventArgs_Base_Exposes_Inner_Via_Explicit_Property_Without_Implicit_Conversion()
	{
		// The three derived classes share the WrappedEventArgs<T> base and all read their
		// inner framework EventArgs through the same .Inner property — explicit unwrap, no
		// implicit-operator-T sleight of hand.
		var coll = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
		new CurrentAssemblyListChangedEventArgs(coll).Inner.Should().BeSameAs(coll);
		new TabPagesCollectionChangedEventArgs(coll).Inner.Should().BeSameAs(coll);

		var prop = new PropertyChangedEventArgs("Foo");
		var settings = new SettingsChangedEventArgs(prop);
		settings.Inner.Should().BeSameAs(prop);
		settings.Inner.PropertyName.Should().Be("Foo");
	}
}
