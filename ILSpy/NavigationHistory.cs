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

namespace ICSharpCode.ILSpy.Navigation
{
	/// <summary>
	/// Two-stack browser-style history. A rapid successive <see cref="Record"/> of the SAME entry
	/// (within 0.5 s) replaces the current entry instead of pushing, so the double-fire a single
	/// click produces -- and tree refreshes that re-select the same node -- don't pollute the back
	/// stack with duplicates. A rapid selection of a DIFFERENT entry still records normally.
	/// Equality is delegated to the entry's own <see cref="IEquatable{T}.Equals"/> implementation.
	/// </summary>
	internal sealed class NavigationHistory<T> where T : class, IEquatable<T?>
	{
		const double NavigationSecondsBeforeNewEntry = 0.5;

		readonly List<T> back = new();
		readonly List<T> forward = new();
		T? current;
		DateTime lastNavigationTime = DateTime.MinValue;

		public bool CanNavigateBack => back.Count > 0;
		public bool CanNavigateForward => forward.Count > 0;

		/// <summary>
		/// The most recently navigated-to entry, or null if no navigation has happened
		/// yet. Mutable callers (e.g. <c>DockWorkspace.CaptureCurrentViewState</c>) reach
		/// in here to stamp the editor's caret + scroll state into the entry just before
		/// it gets pushed onto the back stack — so a subsequent Back restores the position.
		/// </summary>
		public T? Current => current;

		// Read-only views over the history stacks for the toolbar's split-button dropdowns.
		// Both lists are oldest-first (matches push/append order); the UI reverses for "newest
		// first" display.
		public IReadOnlyList<T> BackEntries => back;
		public IReadOnlyList<T> ForwardEntries => forward;

		public T GoBack()
		{
			if (current != null)
				forward.Add(current);
			current = back[^1];
			back.RemoveAt(back.Count - 1);
			return current;
		}

		public T GoForward()
		{
			if (current != null)
				back.Add(current);
			current = forward[^1];
			forward.RemoveAt(forward.Count - 1);
			return current;
		}

		/// <summary>
		/// Pops entries off the matching stack until <paramref name="target"/> becomes the current
		/// entry. Lets the dropdown jump multiple steps at once (web-browser style). Returns the
		/// new current entry, or null if <paramref name="target"/> isn't on the requested stack.
		/// </summary>
		public T? GoTo(T target, bool forward)
		{
			var stack = forward ? this.forward : back;
			if (!stack.Contains(target))
				return null;
			while (!stack[^1].Equals(target))
			{
				if (forward)
					GoForward();
				else
					GoBack();
			}
			return forward ? GoForward() : GoBack();
		}

		public void Clear()
		{
			back.Clear();
			forward.Clear();
			current = null;
		}

		/// <summary>
		/// Removes every entry matched by <paramref name="predicate"/> from both stacks
		/// AND from <see cref="Current"/>. Used by the assembly-list-changed handler to
		/// drop history entries pointing at tree nodes whose containing assembly was
		/// just unloaded — without this, Back/Forward could surface a stale entry whose
		/// Node reference is now detached from the live tree.
		/// </summary>
		public void RemoveAll(Predicate<T> predicate)
		{
			ArgumentNullException.ThrowIfNull(predicate);
			back.RemoveAll(predicate);
			forward.RemoveAll(predicate);
			if (current != null && predicate(current))
				current = null;
		}

		/// <summary>Records a new history entry. Discards the forward stack.</summary>
		public void Record(T entry)
		{
			var navigationTime = DateTime.Now;
			var period = navigationTime - lastNavigationTime;

			if (period.TotalSeconds < NavigationSecondsBeforeNewEntry && current != null && current.Equals(entry))
			{
				// A rapid RE-SELECTION of the SAME target collapses into the current entry. This
				// swallows the double-fire a single click produces (SelectedItems.CollectionChanged
				// and SelectedItem PropertyChanged both fan in) and tree refreshes that re-issue the
				// same SelectedItem. A rapid selection of a DIFFERENT node is a genuine navigation
				// and must fall through to push the old current onto the back stack -- gating only on
				// elapsed time (the previous behaviour) silently dropped fast navigations whenever
				// the decompile finished inside the window (cheap targets, fast machines).
				current = entry;
			}
			else
			{
				if (current != null && !current.Equals(entry))
					back.Add(current);
				// Avoid duplicate stack entries for the same target.
				back.RemoveAll(n => n.Equals(entry));
				current = entry;
			}

			forward.Clear();
			lastNavigationTime = navigationTime;
		}
	}
}
