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

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// A poor-man's equivalent of WPF's <c>CommandManager.RequerySuggested</c>. Avalonia has no
	/// global "re-query your CanExecute" signal, so <see cref="SimpleCommand"/> routes its
	/// <c>CanExecuteChanged</c> through this one (exactly as WPF's SimpleCommand routed it through
	/// <c>CommandManager.RequerySuggested</c>). Call <see cref="InvalidateRequerySuggested"/> whenever
	/// application state that a command's <c>CanExecute</c> reads might have changed (tree selection,
	/// the assembly list, a background load finishing); every bound command then re-evaluates.
	///
	/// This works on every platform: a bound <c>NativeMenuItem</c> subscribes to the command's
	/// CanExecuteChanged and, when it fires, sets <c>IsEnabled = CanExecute()</c>; the macOS Cocoa,
	/// Linux DBus, and in-window menu exporters all track that <c>IsEnabled</c> property.
	///
	/// Subscribers are held WEAKLY (by target + method, like <see cref="Util.WeakEventSource{T}"/>),
	/// so a rebuilt menu or toolbar -- whose items subscribe via the routed CanExecuteChanged -- is
	/// not pinned for the lifetime of this static type. NOTE: a handler with no other strong reference
	/// (e.g. an inline lambda) may be collected before the next raise; callers that need it to survive
	/// must keep it rooted, just as with WPF's weak RequerySuggested.
	/// </summary>
	public static class CommandManager
	{
		static readonly object gate = new();
		static readonly List<WeakHandler> handlers = new();

		/// <summary>Subscribes <paramref name="handler"/> (weakly) to the global re-query signal.</summary>
		public static void AddRequerySuggested(EventHandler handler)
		{
			ArgumentNullException.ThrowIfNull(handler);
			lock (gate)
				handlers.Add(new WeakHandler(handler));
		}

		/// <summary>Removes a previously added handler.</summary>
		public static void RemoveRequerySuggested(EventHandler handler)
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

		/// <summary>
		/// Asks every bound command to re-evaluate its <c>CanExecute</c>. Marshalled to the UI thread
		/// (handlers touch UI state such as <c>NativeMenuItem.IsEnabled</c>), so it is safe to call from
		/// a background thread (e.g. a load sweep finishing off the thread pool).
		/// </summary>
		public static void InvalidateRequerySuggested()
		{
			var dispatcher = global::Avalonia.Threading.Dispatcher.UIThread;
			if (dispatcher.CheckAccess())
				Raise();
			else
				dispatcher.Post(Raise);
		}

		static void Raise()
		{
			// Snapshot under lock so add/remove during dispatch doesn't perturb this raise.
			WeakHandler[] snapshot;
			lock (gate)
				snapshot = handlers.ToArray();
			List<WeakHandler>? dead = null;
			foreach (var h in snapshot)
			{
				if (!h.TryInvoke())
					(dead ??= new()).Add(h);
			}
			if (dead != null)
			{
				lock (gate)
					foreach (var h in dead)
						handlers.Remove(h);
			}
		}

		sealed class WeakHandler
		{
			readonly WeakReference<object>? targetRef;
			readonly MethodInfo method;

			public WeakHandler(EventHandler handler)
			{
				method = handler.Method;
				// Static handlers (null Target) are kept until Remove drops them explicitly.
				targetRef = handler.Target is null ? null : new WeakReference<object>(handler.Target);
			}

			public bool Matches(EventHandler handler)
			{
				if (method != handler.Method)
					return false;
				if (targetRef == null)
					return handler.Target == null;
				return targetRef.TryGetTarget(out var t) && ReferenceEquals(t, handler.Target);
			}

			public bool TryInvoke()
			{
				object? target;
				if (targetRef == null)
					target = null;
				else if (!targetRef.TryGetTarget(out target!))
					return false; // collected -> caller prunes
				method.Invoke(target, new object?[] { null, EventArgs.Empty });
				return true;
			}
		}
	}
}
