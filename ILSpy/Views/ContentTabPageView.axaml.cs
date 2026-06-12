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

using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Views
{
	/// <summary>
	/// View for <see cref="ContentTabPage"/>. Hosts the active <see cref="ContentTabPage.Content"/>
	/// (a <see cref="TabPageModel"/>) in a single ContentControl; the application-wide ViewLocator
	/// resolves it to the matching view. Only the active content's view is realised, so each tab
	/// carries exactly one inner view rather than one of every kind.
	/// </summary>
	public partial class ContentTabPageView : UserControl
	{
		ContentTabPage? boundPage;
		ContentControl contentHost = null!;

		public ContentTabPageView()
		{
			InitializeComponent();
			contentHost = this.FindControl<ContentControl>("ContentHost")!;
			DataContextChanged += (_, _) => RebindPage();
		}

		void InitializeComponent() => AvaloniaXamlLoader.Load(this);

		// Exposed for tests: the ContentTabPage this view is currently bound to. The RebindPage
		// guard keeps this stable across the transient null / same-page DataContext churn Dock
		// produces on a tab switch.
		internal ContentTabPage? BoundPage => boundPage;

		void RebindPage()
		{
			var newPage = DataContext as ContentTabPage;
			// Each ContentTabPage owns its own view, but Dock still nulls and re-sets this view's
			// DataContext as it detaches/reattaches the active document on a tab switch. Ignore the
			// transient null, and ignore a rebind to the SAME page: reassigning Content would rebuild
			// the inner view and re-fire DecompilerTextView.ApplyDocument, resetting scroll / foldings
			// / caret on a mere tab switch. Only a genuine change of page rebinds.
			if (newPage is null || ReferenceEquals(newPage, boundPage))
				return;
			if (boundPage != null)
				boundPage.PropertyChanged -= OnPagePropertyChanged;
			boundPage = newPage;
			boundPage.PropertyChanged += OnPagePropertyChanged;
			ApplyContent();
		}

		void OnPagePropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(ContentTabPage.Content))
				ApplyContent();
		}

		// Hand the active content to the host. The ViewLocator (App.axaml DataTemplates) resolves it
		// to its view and the ContentPresenter sets that view's DataContext. Same-type navigation
		// reuses the same Content instance, so the ContentControl keeps the existing view (and its
		// scroll / foldings); a content-type change swaps in the new type's view.
		void ApplyContent() => contentHost.Content = boundPage?.Content;
	}
}
