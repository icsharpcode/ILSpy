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
using Avalonia.Controls.Documents;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.Themes;
using ICSharpCode.ILSpyX.TreeView;

namespace ICSharpCode.ILSpy.Controls.TreeView
{
	/// <summary>
	/// Attached behaviour for the tree's label <see cref="TextBlock"/>: when the bound node is an
	/// <see cref="IRichTextNode"/> that yields rich text, it renders coloured / bold inlines;
	/// otherwise it shows the node's plain <c>Text</c>. Keeping this in one place lets the single
	/// shared SharpTreeView cell template support rich labels without every node becoming rich, and
	/// without changing the <c>SharpTreeNode.Text</c> string used for search, copy and navigation.
	/// </summary>
	public static class RichNodeText
	{
		public static readonly AttachedProperty<SharpTreeNode?> NodeProperty =
			AvaloniaProperty.RegisterAttached<TextBlock, SharpTreeNode?>("Node", typeof(RichNodeText));

		// Live PropertyChanged subscription on the bound node, so a recycled TextBlock detaches cleanly.
		static readonly AttachedProperty<PropertyChangedEventHandler?> TextHandlerProperty =
			AvaloniaProperty.RegisterAttached<TextBlock, PropertyChangedEventHandler?>("TextHandler", typeof(RichNodeText));

		// Live ThemeChanged / language-settings subscriptions (rich nodes only): the signature colours
		// and per-language formatting are baked into the node's cached RichText, so the inlines must be
		// rebuilt when the theme or the active language changes.
		static readonly AttachedProperty<EventHandler?> ThemeHandlerProperty =
			AvaloniaProperty.RegisterAttached<TextBlock, EventHandler?>("ThemeHandler", typeof(RichNodeText));

		static readonly AttachedProperty<PropertyChangedEventHandler?> LanguageHandlerProperty =
			AvaloniaProperty.RegisterAttached<TextBlock, PropertyChangedEventHandler?>("LanguageHandler", typeof(RichNodeText));

		static readonly AttachedProperty<bool> CleanupHookedProperty =
			AvaloniaProperty.RegisterAttached<TextBlock, bool>("CleanupHooked", typeof(RichNodeText));

		static LanguageSettings? languageSettings;
		static LanguageSettings? GetLanguageSettings()
			=> languageSettings ??= AppComposition.TryGetExport<SettingsService>()?.SessionSettings.LanguageSettings;

		static RichNodeText()
		{
			NodeProperty.Changed.AddClassHandler<TextBlock>(OnNodeChanged);
		}

		public static void SetNode(TextBlock element, SharpTreeNode? value) => element.SetValue(NodeProperty, value);
		public static SharpTreeNode? GetNode(TextBlock element) => element.GetValue(NodeProperty);

		static void OnNodeChanged(TextBlock textBlock, AvaloniaPropertyChangedEventArgs e)
		{
			Detach(textBlock, e.OldValue as SharpTreeNode);

			var node = e.NewValue as SharpTreeNode;
			if (node is INotifyPropertyChanged notify)
			{
				// Mirror the {Binding Text} the cell used to have: refresh when Text changes
				// (e.g. a lazily-loaded node swapping its placeholder for the real label).
				void TextHandler(object? sender, PropertyChangedEventArgs args)
				{
					if (args.PropertyName is nameof(SharpTreeNode.Text) or null or "")
						Render(textBlock, node);
				}

				notify.PropertyChanged += TextHandler;
				textBlock.SetValue(TextHandlerProperty, (PropertyChangedEventHandler)TextHandler);
			}

			if (node is IRichTextNode)
			{
				// Rebuild on theme change (palette) and on language change (signature syntax/colours).
				void ThemeHandler(object? sender, EventArgs args) => Render(textBlock, GetNode(textBlock));

				ThemeManager.Current.ThemeChanged += ThemeHandler;
				textBlock.SetValue(ThemeHandlerProperty, (EventHandler)ThemeHandler);

				if (GetLanguageSettings() is { } settings)
				{
					void LanguageHandler(object? sender, PropertyChangedEventArgs args) => Render(textBlock, GetNode(textBlock));

					settings.PropertyChanged += LanguageHandler;
					textBlock.SetValue(LanguageHandlerProperty, (PropertyChangedEventHandler)LanguageHandler);
				}
			}

			if (!textBlock.GetValue(CleanupHookedProperty))
			{
				textBlock.SetValue(CleanupHookedProperty, true);
				textBlock.DetachedFromVisualTree += (sender, args) => Detach(textBlock, GetNode(textBlock));
			}

			Render(textBlock, node);
		}

		// Drops the live subscriptions for <paramref name="node"/>; safe to call repeatedly.
		static void Detach(TextBlock textBlock, SharpTreeNode? node)
		{
			if (node is INotifyPropertyChanged notify && textBlock.GetValue(TextHandlerProperty) is { } textHandler)
				notify.PropertyChanged -= textHandler;
			textBlock.SetValue(TextHandlerProperty, null);

			if (textBlock.GetValue(ThemeHandlerProperty) is { } themeHandler)
				ThemeManager.Current.ThemeChanged -= themeHandler;
			textBlock.SetValue(ThemeHandlerProperty, null);

			if (textBlock.GetValue(LanguageHandlerProperty) is { } languageHandler && GetLanguageSettings() is { } settings)
				settings.PropertyChanged -= languageHandler;
			textBlock.SetValue(LanguageHandlerProperty, null);
		}

		static void Render(TextBlock textBlock, SharpTreeNode? node)
		{
			if (node is IRichTextNode richNode && richNode.CreateRichText() is { } rich)
			{
				var inlines = new InlineCollection();
				DocumentationRenderer.AppendRichText(inlines, rich);
				textBlock.Inlines = inlines;
			}
			else
			{
				textBlock.Inlines = null;
				textBlock.Text = node?.Text?.ToString();
			}
		}
	}
}
