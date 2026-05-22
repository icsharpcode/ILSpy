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

using CommunityToolkit.Mvvm.ComponentModel;

using Dock.Controls.DeferredContentControl;
using Dock.Model.Core;

using ICSharpCode.ILSpyX.TreeView;

using ILSpy.Docking;

namespace ILSpy.ViewModels
{
	/// <summary>
	/// One Document hosted by the dock workspace. <see cref="Content"/> holds the active
	/// inner viewmodel (decompiler text or metadata grid). The wrapper view
	/// (<c>ContentTabPageView</c>) keeps both possible inner views pre-realised and toggles
	/// which is visible — Dock.Avalonia's add+close-in-the-same-tick semantics otherwise
	/// leave the previous view rendered when the tab type changes.
	/// </summary>
	public sealed partial class ContentTabPage : TabPageModel, IDeferredContentPresentation, IDockableViewOwner
	{
		// Each document tab owns its view; it is released for GC when the tab is dropped.
		public Control? OwnedView { get; set; }

		public ContentTabPage()
		{
			// Same reason as ToolPaneModel — Dock's chrome template binds against
			// DockCapabilityOverrides.{CanDrag, CanDrop, CanClose}, and the document tab's
			// owner.DockCapabilityPolicy.{CanDrag, CanDrop, CanClose}. Without these
			// pre-populated, every tab startup logs a [Binding] error per property.
			DockCapabilityOverrides = new DockCapabilityOverrides();
		}

		// Opt out of Dock's deferred presentation: the inner views are already pre-realised
		// by the wrapper view, and headless tests can't reach descendants of a control
		// that's still queued for realisation.
		bool IDeferredContentPresentation.DeferContentPresentation => false;

		[ObservableProperty]
		private object? content;

		/// <summary>
		/// The assembly-tree node this tab represents. Used by <c>DockWorkspace</c> to
		/// synchronise the tree's selection when this tab becomes active — switching to a
		/// tab pulls the tree to whatever entity the tab is showing. Null on the static
		/// "(no selection)" placeholder.
		/// </summary>
		[ObservableProperty]
		private SharpTreeNode? sourceNode;

		/// <summary>
		/// VS-style "preview" / transient tab flag. The persistent <c>MainTab</c> starts as
		/// preview (true): every tree-node click replaces its <see cref="Content"/> in
		/// place. The user can pin it via <c>DockWorkspace.PinCurrentTab</c>, which flips
		/// this to false and creates a new preview-state MainTab beside it. Carved-out
		/// tabs (created via <c>OpenNewTab</c> / "Open in new tab" / "Freeze tab") are
		/// born pinned — they're never replaced by tree-selection content.
		/// </summary>
		[ObservableProperty]
		private bool isPreview = true;

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
