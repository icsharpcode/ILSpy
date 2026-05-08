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

using CommunityToolkit.Mvvm.ComponentModel;

using Dock.Controls.DeferredContentControl;

namespace ILSpy.ViewModels
{
	/// <summary>
	/// The single Document the docking host puts in its document area. <see cref="Content"/>
	/// holds the active inner viewmodel (decompiler text or metadata grid). The wrapper
	/// view (<c>ContentTabPageView</c>) keeps both possible inner views pre-realised and
	/// toggles which is visible — Dock.Avalonia's add+close-in-the-same-tick semantics
	/// otherwise leave the previous view rendered when the tab type changes.
	/// </summary>
	public sealed partial class ContentTabPage : TabPageModel, IDeferredContentPresentation
	{
		// Opt out of Dock's deferred presentation: there's exactly one document tab in this
		// app, the inner views are already pre-realised by the wrapper view, and headless
		// tests can't reach descendants of a control that's still queued for realisation.
		bool IDeferredContentPresentation.DeferContentPresentation => false;

		[ObservableProperty]
		private object? content;

		// Bubble the inner content's Title up to the Document's Title so the tab strip
		// reflects whatever the active page chose (e.g. the decompiler tab's spinner glyph
		// or "DOS Header" for a metadata grid).
		partial void OnContentChanged(object? oldValue, object? newValue)
		{
			if (oldValue is INotifyPropertyChanged oldNotify)
				oldNotify.PropertyChanged -= OnContentPropertyChanged;
			if (newValue is INotifyPropertyChanged newNotify)
				newNotify.PropertyChanged += OnContentPropertyChanged;
			SyncTitleFromContent();
		}

		void OnContentPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Title))
				SyncTitleFromContent();
		}

		void SyncTitleFromContent()
		{
			if (Content is null)
				return;
			var titleProp = Content.GetType().GetProperty(nameof(Title), typeof(string));
			if (titleProp?.GetValue(Content) is string s)
				Title = s;
		}
	}
}
