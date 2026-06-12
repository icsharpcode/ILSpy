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

using Dock.Avalonia.Controls;

using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Themes
{
	/// <summary>
	/// Attached property that toggles the <c>previewTab</c> style class on a
	/// <see cref="DocumentTabStripItem"/> to mirror <see cref="ContentTabPage.IsPreview"/> on its
	/// underlying viewmodel. The class -- rather than a per-setter data binding -- is what App.axaml
	/// keys the One's purple / italic / hover states off, so those selectors match ONLY the One and
	/// never touch frozen tabs (whose theme/blue states are then left completely intact). Freeze is
	/// one-way, so the class is removed when IsPreview flips false. Dock recycles tab containers onto
	/// different dockables, so the subscription is re-established (and the old one detached) on every
	/// DataContextChanged.
	/// </summary>
	public static class PreviewTabClassBehavior
	{
		const string PreviewClass = "previewTab";

		public static readonly AttachedProperty<bool> EnableProperty =
			AvaloniaProperty.RegisterAttached<DocumentTabStripItem, bool>(
				"Enable",
				typeof(PreviewTabClassBehavior));

		public static void SetEnable(DocumentTabStripItem element, bool value)
			=> element.SetValue(EnableProperty, value);

		public static bool GetEnable(DocumentTabStripItem element)
			=> element.GetValue(EnableProperty);

		// The tab and handler the item is currently subscribed to, so the exact handler instance can
		// be detached when the container is recycled onto a different dockable (a fresh closure each
		// Rebind means `-=` must target the stored delegate, not a new one).
		static readonly AttachedProperty<ContentTabPage?> SubscribedTabProperty =
			AvaloniaProperty.RegisterAttached<DocumentTabStripItem, ContentTabPage?>(
				"SubscribedTab",
				typeof(PreviewTabClassBehavior));

		static readonly AttachedProperty<PropertyChangedEventHandler?> HandlerProperty =
			AvaloniaProperty.RegisterAttached<DocumentTabStripItem, PropertyChangedEventHandler?>(
				"Handler",
				typeof(PreviewTabClassBehavior));

		static PreviewTabClassBehavior()
		{
			EnableProperty.Changed.AddClassHandler<DocumentTabStripItem>(OnEnableChanged);
		}

		static void OnEnableChanged(DocumentTabStripItem item, AvaloniaPropertyChangedEventArgs e)
		{
			if (e.NewValue is not true)
				return;

			item.DataContextChanged += (_, _) => Rebind(item);
			Rebind(item);
		}

		static void Rebind(DocumentTabStripItem item)
		{
			var previousTab = item.GetValue(SubscribedTabProperty);
			var previousHandler = item.GetValue(HandlerProperty);
			if (previousTab is not null && previousHandler is not null)
				previousTab.PropertyChanged -= previousHandler;

			if (item.DataContext is not ContentTabPage tab)
			{
				item.SetValue(SubscribedTabProperty, null);
				item.SetValue(HandlerProperty, null);
				item.Classes.Set(PreviewClass, false);
				return;
			}

			item.Classes.Set(PreviewClass, tab.IsPreview);

			PropertyChangedEventHandler handler = (_, args) => {
				if (args.PropertyName == nameof(ContentTabPage.IsPreview))
					item.Classes.Set(PreviewClass, tab.IsPreview);
			};
			tab.PropertyChanged += handler;
			item.SetValue(SubscribedTabProperty, tab);
			item.SetValue(HandlerProperty, handler);
		}
	}
}
