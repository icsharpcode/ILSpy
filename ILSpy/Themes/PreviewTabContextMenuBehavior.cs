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

using Dock.Avalonia.Controls;

using ILSpy.AppEnv;
using ILSpy.Docking;
using ILSpy.ViewModels;

namespace ILSpy.Themes
{
	/// <summary>
	/// Attached property that wires a per-instance <see cref="ContextMenu"/> onto every
	/// <see cref="DocumentTabStripItem"/>. The menu hosts a single "Pin tab" entry whose
	/// <see cref="MenuItem.IsVisible"/> tracks <see cref="ContentTabPage.IsPreview"/> on
	/// the tab's underlying viewmodel — invoking it routes to
	/// <see cref="DockWorkspace.PinCurrentTab"/>. Each tab gets a fresh ContextMenu so
	/// the popup has a unique parent (Avalonia visuals must have at most one parent).
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

			var pinItem = new MenuItem {
				Header = "Pin tab",
				IsVisible = tab.IsPreview,
			};
			pinItem.Click += (_, _) => TryGetDockWorkspace()?.PinCurrentTab();

			// Keep IsVisible in sync with IsPreview so the entry hides once the tab is
			// pinned (or after promotion happens through some other code path).
			tab.PropertyChanged += OnTabPropertyChanged;
			void OnTabPropertyChanged(object? _, PropertyChangedEventArgs args)
			{
				if (args.PropertyName == nameof(ContentTabPage.IsPreview))
					pinItem.IsVisible = tab.IsPreview;
			}

			item.DocumentContextMenu = new ContextMenu {
				ItemsSource = new[] { pinItem },
			};
		}

		static DockWorkspace? TryGetDockWorkspace()
		{
			try
			{ return AppComposition.Current.GetExport<DockWorkspace>(); }
			catch { return null; }
		}
	}
}
