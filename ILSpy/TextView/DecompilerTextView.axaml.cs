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

using System.ComponentModel;

using Avalonia.Controls;

using AvaloniaEdit.Highlighting;

namespace ILSpy.TextView
{
	public partial class DecompilerTextView : UserControl
	{
		RichTextColorizer? activeColorizer;

		public DecompilerTextView()
		{
			InitializeComponent();
		}

		protected override void OnDataContextChanged(System.EventArgs e)
		{
			base.OnDataContextChanged(e);
			if (DataContext is DecompilerTabPageModel model)
			{
				model.PropertyChanged += OnModelPropertyChanged;
				ApplyDocument(model);
			}
		}

		void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (sender is DecompilerTabPageModel model
				&& (e.PropertyName == nameof(DecompilerTabPageModel.Text)
					|| e.PropertyName == nameof(DecompilerTabPageModel.SyntaxExtension)
					|| e.PropertyName == nameof(DecompilerTabPageModel.HighlightingModel)))
			{
				ApplyDocument(model);
			}
		}

		void ApplyDocument(DecompilerTabPageModel model)
		{
			Editor.SyntaxHighlighting = HighlightingService.GetByExtension(model.SyntaxExtension);
			Editor.Document.Text = model.Text;

			// Swap the semantic-highlighting colorizer. AvaloniaEdit only exposes Add/Remove on
			// LineTransformers, so we keep a reference to the previous one.
			var transformers = Editor.TextArea.TextView.LineTransformers;
			if (activeColorizer != null)
			{
				transformers.Remove(activeColorizer);
				activeColorizer = null;
			}
			if (model.HighlightingModel is { } richModel)
			{
				activeColorizer = new RichTextColorizer(richModel);
				transformers.Add(activeColorizer);
			}

			Editor.ScrollToHome();
		}
	}
}
