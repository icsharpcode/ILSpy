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

using ICSharpCode.ILSpy.Docking;

namespace ICSharpCode.ILSpy.ViewModels
{
	/// <summary>
	/// One Document hosted by the dock workspace. <see cref="Content"/> holds the active
	/// inner viewmodel (decompiler text, metadata grid, compare, or Options page). The wrapper
	/// view (<c>ContentTabPageView</c>) hosts it in a single ContentControl and lets the
	/// application-wide ViewLocator resolve it to its view, so the tab realises only the active
	/// content's view.
	/// </summary>
	public sealed partial class ContentTabPage : TabPageModel, IDeferredContentPresentation, IDockableViewOwner
	{
		public Control? OwnedView { get; set; }

		public ContentTabPage()
		{
			// Same reason as ToolPaneModel — Dock's chrome template binds against
			// DockCapabilityOverrides.{CanDrag, CanDrop, CanClose}, and the document tab's
			// owner.DockCapabilityPolicy.{CanDrag, CanDrop, CanClose}. Without these
			// pre-populated, every tab startup logs a [Binding] error per property.
			DockCapabilityOverrides = new DockCapabilityOverrides();
		}

		// Opt out of Dock's deferred presentation so the wrapper view (and the ContentControl
		// that hosts the active content) is realised immediately: headless tests can't reach
		// descendants of a control that's still queued for realisation.
		bool IDeferredContentPresentation.DeferContentPresentation => false;

		[ObservableProperty]
		private ContentPageModel? content;

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
		/// place. The user can freeze it via <c>DockWorkspace.FreezeCurrentTab</c>, which flips
		/// this to false and creates a new preview-state MainTab beside it. Carved-out
		/// tabs (created via <c>OpenNewTab</c> / "Open in new tab" / "Freeze tab") are
		/// born frozen — they're never replaced by tree-selection content.
		/// </summary>
		[ObservableProperty]
		private bool isPreview = true;

		// Bubble the inner content's Title up to the Document's Title so the tab strip
		// reflects whatever the active page chose (e.g. the decompiler tab's spinner glyph
		// or "DOS Header" for a metadata grid). Also mirror the Content's
		// SupportsLanguageSwitching flag so the toolbar pickers can bind to the wrapper
		// without drilling into Content's runtime type.
		partial void OnContentChanged(ContentPageModel? oldValue, ContentPageModel? newValue)
		{
			if (oldValue is not null)
				oldValue.PropertyChanged -= OnContentPropertyChanged;
			if (newValue is not null)
				newValue.PropertyChanged += OnContentPropertyChanged;
			SyncTitleFromContent();
			SyncSupportsLanguageSwitchingFromContent();
		}

		void OnContentPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Title))
				SyncTitleFromContent();
			else if (e.PropertyName == nameof(SupportsLanguageSwitching))
				SyncSupportsLanguageSwitchingFromContent();
		}

		void SyncSupportsLanguageSwitchingFromContent()
		{
			// The toolbar binds to this property. Default true when no content is set — "no
			// language-aware tab is active" leaves the pickers enabled; otherwise mirror the
			// content's own flag (metadata / compare / options report false).
			SupportsLanguageSwitching = Content?.SupportsLanguageSwitching ?? true;
		}

		void SyncTitleFromContent()
		{
			if (Content?.Title is { } s)
				Title = s;
		}

		// Document tab context-menu commands (bound from the DocumentTabStripItem in App.axaml,
		// whose DataContext is this tab). The DockWorkspace owns the dock, so resolve it from the
		// composition container rather than threading a reference through every tab creation site.
		[CommunityToolkit.Mvvm.Input.RelayCommand]
		private void Close() => ICSharpCode.ILSpy.AppEnv.AppComposition.TryGetExport<DockWorkspace>()?.CloseTab(this);

		[CommunityToolkit.Mvvm.Input.RelayCommand]
		private void CloseAllButThis() => ICSharpCode.ILSpy.AppEnv.AppComposition.TryGetExport<DockWorkspace>()?.CloseAllTabsExcept(this);

		[CommunityToolkit.Mvvm.Input.RelayCommand]
		private void CloseAll() => ICSharpCode.ILSpy.AppEnv.AppComposition.TryGetExport<DockWorkspace>()?.CloseAllTabs();
	}
}
