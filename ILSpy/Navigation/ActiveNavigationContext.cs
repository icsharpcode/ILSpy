// Copyright (c) 2026 Piero Viano
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
using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.VisualTree;

using ICSharpCode.ILSpy.AssemblyTree;
using ICSharpCode.ILSpy.TextView;

namespace ICSharpCode.ILSpy.Navigation
{
	/// <summary>
	/// Builds the <see cref="TextViewContext"/> that keyboard and main-menu navigation commands act
	/// on, mirroring what a right-click would produce: the symbol under the caret when a decompiler
	/// text view was the last surface to hold keyboard focus, otherwise the assembly-tree selection.
	/// </summary>
	public static class ActiveNavigationContext
	{
		static WeakReference<DecompilerTextView>? lastFocusedTextView;
		static bool focusTrackingInstalled;

		/// <summary>
		/// Starts recording which surface holds keyboard focus. Idempotent; focus changes inside
		/// popups (menus, flyouts, the chooser) and in menu bars are ignored, so opening the main
		/// menu does not make the commands lose the surface they were invoked for.
		/// </summary>
		public static void EnsureFocusTracking()
		{
			if (focusTrackingInstalled)
				return;
			focusTrackingInstalled = true;
			InputElement.GotFocusEvent.AddClassHandler<TopLevel>((topLevel, e) => OnGotFocus(topLevel, e.Source), handledEventsToo: true);
		}

		static void OnGotFocus(TopLevel topLevel, object? source)
		{
			if (topLevel is PopupRoot || source is not Visual visual)
				return;
			if (visual is MenuItem || visual.FindAncestorOfType<Menu>() != null || visual.FindAncestorOfType<NativeMenuBar>() != null)
				return;
			var textView = visual as DecompilerTextView ?? visual.FindAncestorOfType<DecompilerTextView>();
			lastFocusedTextView = textView == null ? null : new WeakReference<DecompilerTextView>(textView);
		}

		/// <summary>Forgets the last focused text view, so the next context comes from the tree selection.</summary>
		internal static void ClearLastFocusedTextView() => lastFocusedTextView = null;

		/// <summary>The decompiler text view that last held keyboard focus, while it is still on screen.</summary>
		public static DecompilerTextView? LastFocusedTextView {
			get {
				if (lastFocusedTextView == null || !lastFocusedTextView.TryGetTarget(out var view))
					return null;
				return TopLevel.GetTopLevel(view) != null && view.IsEffectivelyVisible ? view : null;
			}
		}

		/// <summary>The context for the surface the user is working in; see the type summary.</summary>
		public static TextViewContext Current(AssemblyTreeModel assemblyTreeModel)
		{
			ArgumentNullException.ThrowIfNull(assemblyTreeModel);
			return LastFocusedTextView is { } view ? ForTextViewCaret(view) : ForTreeSelection(assemblyTreeModel);
		}

		/// <summary>The context for the reference under the caret of <paramref name="view"/>.</summary>
		public static TextViewContext ForTextViewCaret(DecompilerTextView view)
		{
			ArgumentNullException.ThrowIfNull(view);
			var offset = view.Editor.TextArea.Caret.Offset;
			ReferenceSegment? segment = null;
			if (view.DataContext is DecompilerTabPageModel { References: { } references })
			{
				// A caret placed just after an identifier still counts as being on it.
				segment = references.FindSegmentsContaining(offset).FirstOrDefault(r => r.Reference != null)
					?? (offset > 0 ? references.FindSegmentsContaining(offset - 1).FirstOrDefault(r => r.Reference != null) : null);
			}
			return new TextViewContext {
				TextView = view,
				Reference = segment,
				TextLocation = offset,
			};
		}

		/// <summary>The context for the current assembly-tree selection.</summary>
		public static TextViewContext ForTreeSelection(AssemblyTreeModel assemblyTreeModel)
		{
			ArgumentNullException.ThrowIfNull(assemblyTreeModel);
			return new TextViewContext {
				SelectedTreeNodes = assemblyTreeModel.SelectedItems.ToArray(),
			};
		}

		/// <summary>The control a popup for <paramref name="context"/> is shown over, when the context names one.</summary>
		public static Control? AnchorOf(TextViewContext context)
		{
			ArgumentNullException.ThrowIfNull(context);
			return context.TextView ?? context.TreeGrid ?? context.ListBox ?? (Control?)context.DataGrid ?? context.OriginalSource as Control;
		}
	}
}
