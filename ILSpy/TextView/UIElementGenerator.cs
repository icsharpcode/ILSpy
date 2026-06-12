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

using Avalonia.Controls;

using AvaloniaEdit.Rendering;

namespace ICSharpCode.ILSpy.TextView
{
	using Pair = KeyValuePair<int, Func<Control>>;

	/// <summary>
	/// Embeds inline UI elements produced by <see cref="ISmartTextOutput.AddUIElement"/> in
	/// the rendered text. Each element is stored as a factory; this generator builds and caches
	/// one control instance per offset locally, so the control belongs to a single editor. The
	/// model only carries the factory, never a shared control -- otherwise a reused or reopened
	/// tab rendering into a different TextView would hit AvaloniaEdit's "already has a visual
	/// parent" guard.
	/// </summary>
	sealed class UIElementGenerator : VisualLineElementGenerator, IComparer<Pair>
	{
		// Controls realised so far, keyed by document offset, so re-laying out the same line in
		// THIS editor reuses the instance (preserving its state, and letting AvaloniaEdit's
		// per-TextView dedup handle it) rather than building a fresh one each time.
		readonly Dictionary<int, Control> realised = new();

		IReadOnlyList<Pair>? uiElements;

		public IReadOnlyList<Pair>? UIElements {
			get => uiElements;
			set {
				// Rebinding to a new document: drop controls built for the previous one so they
				// are rebuilt from the new model's factories.
				realised.Clear();
				uiElements = value;
			}
		}

		public override int GetFirstInterestedOffset(int startOffset)
		{
			if (uiElements == null)
				return -1;
			int r = BinarySearch(uiElements, new Pair(startOffset, null!));
			if (r < 0)
				r = ~r;
			return r < uiElements.Count ? uiElements[r].Key : -1;
		}

		public override VisualLineElement? ConstructElement(int offset)
		{
			if (uiElements == null)
				return null;
			int r = BinarySearch(uiElements, new Pair(offset, null!));
			if (r < 0)
				return null;
			if (!realised.TryGetValue(offset, out var control))
			{
				control = uiElements[r].Value();
				realised[offset] = control;
			}
			return new InlineObjectElement(0, control);
		}

		int IComparer<Pair>.Compare(Pair x, Pair y) => x.Key.CompareTo(y.Key);

		// IReadOnlyList has no BinarySearch. Hand-rolled — list is offset-sorted by
		// AvaloniaEditTextOutput.AddUIElement.
		int BinarySearch(IReadOnlyList<Pair> list, Pair value)
		{
			int lo = 0, hi = list.Count - 1;
			while (lo <= hi)
			{
				int mid = lo + ((hi - lo) >> 1);
				int cmp = ((IComparer<Pair>)this).Compare(list[mid], value);
				if (cmp == 0)
					return mid;
				if (cmp < 0)
					lo = mid + 1;
				else
					hi = mid - 1;
			}
			return ~lo;
		}
	}
}
