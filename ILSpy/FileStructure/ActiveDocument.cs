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

using System.Linq;

using Avalonia;
using Avalonia.Controls;
using Avalonia.VisualTree;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.Views;

namespace ICSharpCode.ILSpy.FileStructure
{
	/// <summary>
	/// Locates the decompiler document that navigation commands act on, and the symbol under its
	/// caret. Resolved through the composition host on every call, so the result always reflects
	/// the current layout.
	/// </summary>
	internal static class ActiveDocument
	{
		/// <summary>The decompiler document of the active document tab, or null when the active tab
		/// shows something else (metadata table, options, ...).</summary>
		public static DecompilerTabPageModel? Get()
		{
			var workspace = AppComposition.TryGetExport<DockWorkspace>();
			if (workspace == null)
				return null;
			if (workspace.ActiveContentTabPage?.Content is DecompilerTabPageModel { IsStaticContent: false } active)
				return active;
			return workspace.ActiveDecompilerTab;
		}

		/// <summary>The decompiler document whose editor holds the keyboard focus, or null.</summary>
		public static DecompilerTabPageModel? GetFocused()
		{
			var window = AppComposition.TryGetExport<MainWindow>();
			if (window?.FocusManager?.GetFocusedElement() is not Visual focused)
				return null;
			var view = focused.GetSelfAndVisualAncestors().OfType<DecompilerTextView>().FirstOrDefault();
			return view?.DataContext as DecompilerTabPageModel;
		}

		/// <summary>
		/// The navigable reference (type, member or metadata reference) at the caret of
		/// <paramref name="document"/>, or null when the caret is not on one or no editor is attached.
		/// </summary>
		public static object? GetReferenceAtCaret(DecompilerTabPageModel document)
		{
			if (document.CaptureViewState?.Invoke() is not { } state || document.References is not { } references)
				return null;
			int offset = state.CaretOffset;
			// A caret right after the last character of a name still counts as being on it.
			var segment = references.FindSegmentsContaining(offset)
				.Concat(offset > 0 ? references.FindSegmentsContaining(offset - 1) : Enumerable.Empty<ReferenceSegment>())
				.FirstOrDefault(s => s.Kind == ReferenceMode.Link && s.Reference != null);
			return segment?.Reference;
		}
	}
}
