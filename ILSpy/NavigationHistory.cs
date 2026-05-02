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

namespace ILSpy.Navigation
{
	/// <summary>
	/// Two-stack browser-style history. Rapid successive <see cref="Record"/> calls (within
	/// 0.5 s) replace the current entry instead of pushing, so a tree refresh that re-selects
	/// the same node doesn't pollute the back stack with duplicates. Equality is reference-
	/// based — fine for tree nodes which are reused for the lifetime of an assembly load.
	/// </summary>
	internal sealed class NavigationHistory<T> where T : class
	{
		const double NavigationSecondsBeforeNewEntry = 0.5;

		readonly List<T> back = new();
		readonly List<T> forward = new();
		T? current;
		DateTime lastNavigationTime = DateTime.MinValue;

		public bool CanNavigateBack => back.Count > 0;
		public bool CanNavigateForward => forward.Count > 0;

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
			while (!ReferenceEquals(stack[^1], target))
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

		/// <summary>Records a new history entry. Discards the forward stack.</summary>
		public void Record(T node)
		{
			var navigationTime = DateTime.Now;
			var period = navigationTime - lastNavigationTime;

			if (period.TotalSeconds < NavigationSecondsBeforeNewEntry)
			{
				// Rapid successive selections collapse into a single entry — protects against
				// tree refreshes that re-issue SelectedItem.
				current = node;
			}
			else
			{
				if (current != null && !ReferenceEquals(current, node))
					back.Add(current);
				// Avoid duplicate stack entries for the same target.
				back.RemoveAll(n => ReferenceEquals(n, node));
				current = node;
			}

			forward.Clear();
			lastNavigationTime = navigationTime;
		}
	}
}
