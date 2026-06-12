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

using Dock.Model.Controls;
using Dock.Model.Core;

namespace ICSharpCode.ILSpy.ViewModels
{
	/// <summary>
	/// Window-menu adapter for a single document tab. Forwards <see cref="Title"/> from the
	/// underlying <see cref="ContentTabPage"/> and surfaces an <see cref="IsActive"/> bool
	/// the menu can two-way-bind to so clicking the entry activates the tab.
	/// </summary>
	public class TabPageMenuItem : ObservableObject
	{
		readonly IFactory factory;
		readonly IDocumentDock owner;

		public TabPageMenuItem(ContentTabPage tab, IFactory factory, IDocumentDock owner)
		{
			Tab = tab;
			this.factory = factory;
			this.owner = owner;
			tab.PropertyChanged += OnTabPropertyChanged;
		}

		public ContentTabPage Tab { get; }

		public string? Title => Tab.Title;

		public bool IsActive {
			get => ReferenceEquals(owner.ActiveDockable, Tab);
			set {
				if (!value || IsActive)
					return;
				factory.SetActiveDockable(Tab);
				factory.SetFocusedDockable(owner, Tab);
				// IsActive's new value flows out via NotifyActiveChanged once the owner's
				// ActiveDockable swap fires its PropertyChanged.
			}
		}

		/// <summary>
		/// Called by <see cref="Docking.DockWorkspace"/> when the documents dock's
		/// <c>ActiveDockable</c> changes — re-raises <see cref="IsActive"/> so the menu
		/// item's bound checkmark updates without each item having to subscribe to the
		/// dock's own PropertyChanged.
		/// </summary>
		public void NotifyActiveChanged() => OnPropertyChanged(nameof(IsActive));

		internal void Detach() => Tab.PropertyChanged -= OnTabPropertyChanged;

		void OnTabPropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(Tab.Title))
				OnPropertyChanged(nameof(Title));
		}
	}
}
