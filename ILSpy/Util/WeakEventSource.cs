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
using System.Collections.Generic;
using System.Reflection;

namespace ICSharpCode.ILSpy.Util
{
	/// <summary>
	/// A bag of event handlers whose target instances are held by <see cref="WeakReference{T}"/>.
	/// Subscribers whose target has been garbage-collected are pruned on the next
	/// <see cref="Raise"/>; the bus never roots them itself. Static-method handlers (handlers
	/// with a null <c>Target</c>) are kept until <see cref="Unsubscribe"/> removes them.
	/// </summary>
	/// <remarks>
	/// Each subscription stores the handler's <see cref="MethodInfo"/> plus a weak ref to its
	/// target. Invocation reflects through <see cref="MethodInfo.Invoke"/> after checking that
	/// the target is still alive. The overhead is fine for the MessageBus use case (a handful
	/// of subscribers, fired on settings / navigation / lifecycle events), and pays for itself
	/// by letting subscribers skip the boilerplate of explicit unsubscription.
	/// </remarks>
	internal sealed class WeakEventSource<T> where T : EventArgs
	{
		readonly object gate = new();
		readonly List<WeakDelegate> handlers = new();

		public void Subscribe(EventHandler<T> handler)
		{
			ArgumentNullException.ThrowIfNull(handler);
			lock (gate)
				handlers.Add(new WeakDelegate(handler));
		}

		public void Unsubscribe(EventHandler<T> handler)
		{
			ArgumentNullException.ThrowIfNull(handler);
			lock (gate)
			{
				for (int i = handlers.Count - 1; i >= 0; i--)
				{
					if (handlers[i].Matches(handler))
					{
						handlers.RemoveAt(i);
						break;
					}
				}
			}
		}

		public void Raise(object sender, T e)
		{
			// Snapshot under lock so handlers added or removed during a raise don't perturb
			// this dispatch — matches the .NET event-multicast convention.
			WeakDelegate[] snapshot;
			lock (gate)
				snapshot = handlers.ToArray();
			List<WeakDelegate>? dead = null;
			foreach (var d in snapshot)
			{
				if (!d.TryInvoke(sender, e))
					(dead ??= new()).Add(d);
			}
			if (dead != null)
			{
				lock (gate)
					foreach (var d in dead)
						handlers.Remove(d);
			}
		}

		sealed class WeakDelegate
		{
			readonly WeakReference<object>? targetRef;
			readonly MethodInfo method;

			public WeakDelegate(EventHandler<T> handler)
			{
				method = handler.Method;
				// Static handlers expose a null Target; we leave targetRef null and treat
				// the subscription as "always alive" until Unsubscribe drops it explicitly.
				targetRef = handler.Target is null ? null : new WeakReference<object>(handler.Target);
			}

			public bool Matches(EventHandler<T> handler)
			{
				if (method != handler.Method)
					return false;
				if (targetRef == null)
					return handler.Target == null;
				return targetRef.TryGetTarget(out var t) && ReferenceEquals(t, handler.Target);
			}

			public bool TryInvoke(object sender, T e)
			{
				object? target;
				if (targetRef == null)
				{
					target = null;
				}
				else if (!targetRef.TryGetTarget(out target!))
				{
					return false;
				}
				method.Invoke(target, new object?[] { sender, e });
				return true;
			}
		}
	}
}
