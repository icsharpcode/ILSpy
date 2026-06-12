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

using System;
using System.ComponentModel;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

using ICSharpCode.ILSpy.Options;

namespace ICSharpCode.ILSpy.TextView
{
	/// <summary>
	/// Editor-corner overlay with zoom in / out / reset buttons plus a live "133%" label.
	/// Bound to <see cref="DisplaySettings.SelectedFontSize"/>; the overlay auto-hides at
	/// the default font size unless <see cref="AlwaysShowZoomButtons"/> is set to <c>true</c>.
	/// </summary>
	public partial class ZoomButtons : UserControl
	{
		/// <summary>
		/// When <c>false</c> (the default), the overlay is hidden while the zoom level is at
		/// 100% and only appears once the user actually zooms. When <c>true</c>, it stays
		/// visible permanently. Matches WPF's <c>ZoomScrollViewer.AlwaysShowZoomButtons</c>.
		/// </summary>
		public static readonly StyledProperty<bool> AlwaysShowZoomButtonsProperty =
			AvaloniaProperty.Register<ZoomButtons, bool>(nameof(AlwaysShowZoomButtons));

		public bool AlwaysShowZoomButtons {
			get => GetValue(AlwaysShowZoomButtonsProperty);
			set => SetValue(AlwaysShowZoomButtonsProperty, value);
		}

		DisplaySettings? settings;

		public ZoomButtons()
		{
			InitializeComponent();
			RefreshVisibility();
			// Defer named-control lookup to AttachedToVisualTree: at construction time the
			// UserControl's own NameScope isn't yet attached to `this`, so FindControl walks
			// upward and throws "Could not find parent name scope". After visual-tree attach
			// the scope is in place and the named children resolve normally.
			AttachedToVisualTree += (_, _) => AttachButtonHandlers();
		}

		void InitializeComponent() => AvaloniaXamlLoader.Load(this);

		bool handlersAttached;

		void AttachButtonHandlers()
		{
			if (handlersAttached)
				return;
			handlersAttached = true;
			if (this.FindControl<Button>("MinusButton") is { } minus)
				minus.Click += (_, _) => Zoom(EditorZoom.ZoomOut);
			if (this.FindControl<Button>("PlusButton") is { } plus)
				plus.Click += (_, _) => Zoom(EditorZoom.ZoomIn);
			if (this.FindControl<Button>("ResetButton") is { } reset)
				reset.Click += (_, _) => Zoom(_ => EditorZoom.Reset());
		}

		/// <summary>
		/// Binds this widget to a live <see cref="DisplaySettings"/>. Re-renders the
		/// percent label and visibility on each <c>SelectedFontSize</c> change. Safe to
		/// call multiple times — the previous subscription is dropped before the new one
		/// is wired.
		/// </summary>
		public void Bind(DisplaySettings settings)
		{
			if (this.settings != null)
				this.settings.PropertyChanged -= OnSettingsChanged;
			this.settings = settings;
			if (settings != null)
				settings.PropertyChanged += OnSettingsChanged;
			RefreshLabel();
			RefreshVisibility();
		}

		void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(DisplaySettings.SelectedFontSize))
			{
				RefreshLabel();
				RefreshVisibility();
			}
		}

		protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
		{
			base.OnPropertyChanged(e);
			if (e.Property == AlwaysShowZoomButtonsProperty)
				RefreshVisibility();
		}

		void Zoom(Func<double, double> step)
		{
			if (settings == null)
				return;
			settings.SelectedFontSize = step(settings.SelectedFontSize);
		}

		void RefreshLabel()
		{
			if (settings == null || this.FindControl<TextBlock>("PercentLabel") is not { } label)
				return;
			var pct = (int)Math.Round(settings.SelectedFontSize / EditorZoom.DefaultFontSize * 100.0);
			label.Text = pct + "%";
		}

		void RefreshVisibility()
		{
			IsVisible = settings != null
				&& (AlwaysShowZoomButtons
					|| Math.Abs(settings.SelectedFontSize - EditorZoom.DefaultFontSize) > 0.001);
		}
	}
}
