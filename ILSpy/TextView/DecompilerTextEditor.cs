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

using AvaloniaEdit;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Rendering;

using ICSharpCode.ILSpy.Themes;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// <see cref="TextEditor"/> subclass that hooks two theme concerns:
	/// (a) overrides <see cref="TextEditor.CreateColorizer"/> so syntax highlighting goes
	///     through <see cref="ThemeAwareHighlightingColorizer"/> and adapts to the active theme;
	/// (b) listens for <see cref="ThemeManager.ThemeChanged"/> and forces a TextView redraw
	///     so an already-rendered editor picks up the new palette without needing the user
	///     to scroll or reselect.
	/// </summary>
	public class DecompilerTextEditor : TextEditor
	{
		public DecompilerTextEditor()
		{
			ThemeManager.Current.ThemeChanged += OnThemeChanged;
		}

		// Avalonia resolves the control template via the runtime type; subclasses of a
		// templated control inherit the base template only when StyleKeyOverride is
		// pointed at the base. Without this override AvaloniaEdit's template doesn't
		// apply to us — meaning no ScrollViewer is installed, scroll offsets stay 0,
		// and Copy can't reach the editor's TextArea via the template lookup chain.
		protected override System.Type StyleKeyOverride => typeof(TextEditor);

		protected override IVisualLineTransformer CreateColorizer(IHighlightingDefinition highlightingDefinition)
		{
			ArgumentNullException.ThrowIfNull(highlightingDefinition);
			return new ThemeAwareHighlightingColorizer(highlightingDefinition);
		}

		void OnThemeChanged(object? sender, EventArgs e)
		{
			// Already-painted lines cache their colour decisions; a Redraw discards those
			// caches and re-runs the colorizer pipeline against the new IsDarkTheme value.
			TextArea?.TextView?.Redraw();
		}

		protected override void OnDetachedFromVisualTree(global::Avalonia.VisualTreeAttachmentEventArgs e)
		{
			ThemeManager.Current.ThemeChanged -= OnThemeChanged;
			base.OnDetachedFromVisualTree(e);
		}
	}
}
