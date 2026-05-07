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
using Avalonia.Markup.Xaml;

using ILSpy.TextView;
using ILSpy.ViewModels;

namespace ILSpy.Views
{
	/// <summary>
	/// View for <see cref="ContentTabPage"/>. Toggles which pre-realised inner view shows
	/// (decompiler text or metadata grid) based on <see cref="ContentTabPage.Content"/>'s
	/// runtime type — both inner views live in the visual tree from construction time so
	/// the dock's visual swap is just a visibility flip.
	/// </summary>
	public partial class ContentTabPageView : UserControl
	{
		ContentTabPage? boundPage;
		DecompilerTextView decompilerView = null!;
		MetadataTablePage metadataView = null!;

		public ContentTabPageView()
		{
			InitializeComponent();
			decompilerView = this.FindControl<DecompilerTextView>("DecompilerView")!;
			metadataView = this.FindControl<MetadataTablePage>("MetadataView")!;
			DataContextChanged += (_, _) => RebindPage();
		}

		void InitializeComponent() => AvaloniaXamlLoader.Load(this);

		void RebindPage()
		{
			if (boundPage != null)
				boundPage.PropertyChanged -= OnPagePropertyChanged;
			boundPage = DataContext as ContentTabPage;
			if (boundPage != null)
				boundPage.PropertyChanged += OnPagePropertyChanged;
			ApplyContent();
		}

		void OnPagePropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ContentTabPage.Content))
				ApplyContent();
		}

		void ApplyContent()
		{
			var content = boundPage?.Content;

			if (content is DecompilerTabPageModel decompiler)
			{
				decompilerView.DataContext = decompiler;
				decompilerView.IsVisible = true;
			}
			else
			{
				decompilerView.IsVisible = false;
				decompilerView.DataContext = null;
			}

			if (content is MetadataTablePageModel metadata)
			{
				metadataView.DataContext = metadata;
				metadataView.IsVisible = true;
			}
			else
			{
				metadataView.IsVisible = false;
				metadataView.DataContext = null;
			}
		}
	}
}
