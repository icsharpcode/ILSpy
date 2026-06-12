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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;

using Dock.Avalonia.Controls;

using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Themes
{
	/// <summary>
	/// Attached property that wires a per-instance context menu onto every
	/// <see cref="DocumentTabStripItem"/> via Dock's <c>DocumentContextMenu</c> (the property Dock
	/// actually shows on a document-tab right-click — it overrides the standard
	/// <see cref="Control.ContextMenu"/>). The menu carries Close / Close all but this / Close all
	/// (routing to the tab's commands) plus a "Freeze tab" entry shown only while the tab is a
	/// preview (<see cref="ContentTabPage.IsPreview"/>); invoking Freeze routes to
	/// <see cref="DockWorkspace.FreezeCurrentTab"/>. Each tab gets a fresh ContextMenu so the popup
	/// has a unique parent (Avalonia visuals must have at most one parent).
	/// </summary>
	public static class PreviewTabContextMenuBehavior
	{
		public static readonly AttachedProperty<bool> EnableProperty =
			AvaloniaProperty.RegisterAttached<DocumentTabStripItem, bool>(
				"Enable",
				typeof(PreviewTabContextMenuBehavior));

		public static void SetEnable(DocumentTabStripItem element, bool value)
			=> element.SetValue(EnableProperty, value);

		public static bool GetEnable(DocumentTabStripItem element)
			=> element.GetValue(EnableProperty);

		static PreviewTabContextMenuBehavior()
		{
			EnableProperty.Changed.AddClassHandler<DocumentTabStripItem>(OnEnableChanged);
		}

		static void OnEnableChanged(DocumentTabStripItem item, AvaloniaPropertyChangedEventArgs e)
		{
			if (e.NewValue is not true)
				return;

			// Bind the menu's per-tab state once on DataContext settlement. ContentTabPage
			// is the only DataContext we know how to model here; other dockable types
			// (tool panes) fall through to no menu.
			item.DataContextChanged += (_, _) => Rebind(item);
			Rebind(item);
		}

		static void Rebind(DocumentTabStripItem item)
		{
			if (item.DataContext is not ContentTabPage tab)
			{
				item.DocumentContextMenu = null;
				return;
			}
			item.DocumentContextMenu = BuildDocumentContextMenu(tab);
		}

		/// <summary>
		/// Builds the document-tab context menu for <paramref name="tab"/>: Close / Close all but
		/// this / Close all, plus a "Freeze tab" entry (and its separator) shown only while the tab
		/// is a preview. Exposed for tests so the menu's composition can be verified without the
		/// realised tab-strip visual.
		/// </summary>
		internal static ContextMenu BuildDocumentContextMenu(ContentTabPage tab)
		{
			var close = new MenuItem { Header = Resources.Close, Command = tab.CloseCommand };
			// The last remaining document is made un-closeable (DockWorkspace keeps CanClose false so
			// the document area can't be emptied). Mirror that on the Close entry, the way Dock's own
			// close button does -- CanClose is observable (DockableBase : ReactiveBase), so this stays
			// in sync as tabs open and close.
			close.Bind(InputElement.IsEnabledProperty,
				new Binding(nameof(ContentTabPage.CanClose)) { Source = tab });
			var closeAllButThis = new MenuItem { Header = Resources.CloseAllButThisTab, Command = tab.CloseAllButThisCommand };
			var closeAll = new MenuItem { Header = Resources.CloseAllTabs, Command = tab.CloseAllCommand };

			var freezeSeparator = new Separator { IsVisible = tab.IsPreview };
			var freezeItem = new MenuItem {
				Header = "Freeze tab",
				IsVisible = tab.IsPreview,
			};
			freezeItem.Click += (_, _) => AppComposition.TryGetExport<DockWorkspace>()?.FreezeCurrentTab();

			// Keep the Freeze entry (and its separator) in sync with IsPreview so they hide once the
			// tab is frozen (freeze is one-way — there's no matching unfreeze entry).
			tab.PropertyChanged += OnTabPropertyChanged;
			void OnTabPropertyChanged(object? _, PropertyChangedEventArgs args)
			{
				if (args.PropertyName == nameof(ContentTabPage.IsPreview))
				{
					freezeItem.IsVisible = tab.IsPreview;
					freezeSeparator.IsVisible = tab.IsPreview;
				}
			}

			return new ContextMenu {
				ItemsSource = new Control[] {
					close,
					new Separator(),
					closeAllButThis,
					closeAll,
					freezeSeparator,
					freezeItem,
				},
			};
		}
	}
}
