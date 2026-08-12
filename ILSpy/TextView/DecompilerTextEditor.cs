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
using System.ComponentModel;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

using AvaloniaEdit;
using AvaloniaEdit.Editing;
using AvaloniaEdit.Highlighting;
using AvaloniaEdit.Rendering;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.Themes;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// <see cref="TextEditor"/> subclass carrying the decompiler-view look, so every surface
	/// showing code (the main text view, metadata row details) renders identically:
	/// (a) overrides <see cref="TextEditor.CreateColorizer"/> so syntax highlighting goes
	///     through <see cref="ThemeAwareHighlightingColorizer"/> and adapts to the active theme;
	/// (b) listens for <see cref="ThemeManager.ThemeChanged"/> and forces a TextView redraw
	///     so an already-rendered editor picks up the new palette without needing the user
	///     to scroll or reselect;
	/// (c) follows the user-selected editor font (<see cref="DisplaySettings.SelectedFont"/> /
	///     <see cref="DisplaySettings.SelectedFontSize"/>) live while attached;
	/// (d) uses the themed editor background and selection highlight.
	/// </summary>
	public class DecompilerTextEditor : TextEditor
	{
		// Avalonia resolves the control template via the runtime type; subclasses of a
		// templated control inherit the base template only when StyleKeyOverride is
		// pointed at the base. Without this override AvaloniaEdit's template doesn't
		// apply to us — meaning no ScrollViewer is installed, scroll offsets stay 0,
		// and Copy can't reach the editor's TextArea via the template lookup chain.
		protected override Type StyleKeyOverride => typeof(TextEditor);

		DisplaySettings? displaySettings;

		public DecompilerTextEditor()
		{
			// Fallback font for hosts without display settings (e.g. bare test compositions);
			// overwritten from DisplaySettings on attach.
			FontFamily = new FontFamily("Consolas, Menlo, Monospace");
			FontSize = 13;
			// Selected text keeps its syntax colours (ports icsharpcode/ILSpy#2938):
			// SelectionForeground stays unset, and the selection is a flat, translucent
			// highlight (square corners, no border) instead of a recoloured run.
			TextArea.SelectionCornerRadius = 0;
			TextArea.Bind(TextArea.SelectionBrushProperty, this.GetResourceObservable("ILSpy.EditorSelectionBrush"));
			this.Bind(BackgroundProperty, this.GetResourceObservable("ILSpy.EditorBackground"));
		}

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

		void OnDisplaySettingsChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName is nameof(DisplaySettings.SelectedFont) or nameof(DisplaySettings.SelectedFontSize)
				or nameof(DisplaySettings.EditorZoomFactor))
				ApplyFontSettings();
		}

		void ApplyFontSettings()
		{
			if (displaySettings == null)
				return;
			if (!string.IsNullOrEmpty(displaySettings.SelectedFont))
				FontFamily = new FontFamily(displaySettings.SelectedFont);
			if (displaySettings.SelectedFontSize > 0)
				FontSize = EditorZoom.EffectiveFontSize(displaySettings);
		}

		static DisplaySettings? TryGetDisplaySettings()
		{
			try
			{ return AppComposition.Current.GetExport<SettingsService>().DisplaySettings; }
			catch { return null; }
		}

		protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
		{
			base.OnAttachedToVisualTree(e);
			ThemeManager.Current.ThemeChanged += OnThemeChanged;
			// (Re-)apply the font on every attach: editors inside recycled containers
			// (metadata row details) detach and re-attach, and settings may have changed
			// while the editor was off the tree.
			displaySettings = TryGetDisplaySettings();
			if (displaySettings != null)
			{
				ApplyFontSettings();
				displaySettings.PropertyChanged += OnDisplaySettingsChanged;
			}
			TextArea?.TextView?.Redraw();
		}

		protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
		{
			if (displaySettings != null)
			{
				displaySettings.PropertyChanged -= OnDisplaySettingsChanged;
				displaySettings = null;
			}
			ThemeManager.Current.ThemeChanged -= OnThemeChanged;
			base.OnDetachedFromVisualTree(e);
		}
	}
}
