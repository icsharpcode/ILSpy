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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.VisualTree;

using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// Builds DataGrid columns for a metadata-row type by reflecting over its public
	/// properties. Reflection metadata is cached as <c>ColumnDescriptor</c>s; live
	/// <see cref="DataGridColumn"/> instances are constructed fresh on every call so each
	/// page can carry its own per-column filter inputs in the headers without sharing state
	/// with sibling pages.
	/// </summary>
	public static class MetadataColumnBuilder
	{
		static readonly ConcurrentDictionary<Type, ColumnDescriptor[]> descriptorCache = new();

		/// <summary>
		/// Convenience overload for tests and simple consumers — returns columns with plain
		/// string headers and no per-column filter wiring. Production code should call
		/// <see cref="Populate{TEntry}(MetadataTablePageModel)"/>, which also seeds the page's
		/// <c>ColumnFilters</c> collection and bakes filter inputs into each header.
		/// </summary>
		public static IReadOnlyList<DataGridColumn> For<TEntry>() => For(typeof(TEntry));

		public static IReadOnlyList<DataGridColumn> For(Type entryType)
		{
			ArgumentNullException.ThrowIfNull(entryType);
			return descriptorCache.GetOrAdd(entryType, BuildDescriptors)
				.Select(d => BuildColumn(d, filter: null, pageKey: null))
				.ToArray();
		}

		/// <summary>
		/// Populates <paramref name="page"/> with both <c>Columns</c> and
		/// <c>ColumnFilters</c> in matched order. Each column's header is a vertical pair —
		/// the column name above a small TextBox bound two-way to the column's filter — so
		/// the user can filter every column independently and the predicate ANDs the lot.
		/// </summary>
		public static void Populate<TEntry>(MetadataTablePageModel page) => Populate(page, typeof(TEntry));

		public static void Populate(MetadataTablePageModel page, Type entryType)
		{
			ArgumentNullException.ThrowIfNull(page);
			ArgumentNullException.ThrowIfNull(entryType);
			var descriptors = descriptorCache.GetOrAdd(entryType, BuildDescriptors);
			page.ColumnFilters.Clear();
			// Per-table page key for the FilterState persistence cache. Using the entry
			// type's full name means the same filter applies across all loaded assemblies'
			// instances of this table — which matches the UX the schema-driven dropdowns
			// already imply (predicate is column-shape-driven, not assembly-driven).
			var pageKey = entryType.FullName ?? entryType.Name;
			var columns = new List<DataGridColumn>(descriptors.Length);
			foreach (var d in descriptors)
			{
				var filter = new ColumnFilter(d.Name);
				page.ColumnFilters.Add(filter);
				columns.Add(BuildColumn(d, filter, pageKey));
			}
			page.Columns = columns;
		}

		static ColumnDescriptor[] BuildDescriptors(Type entryType)
		{
			var list = new List<ColumnDescriptor>();
			foreach (var prop in entryType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				if (prop.GetIndexParameters().Length > 0)
					continue;
				if (prop.Name == nameof(Entry.RowDetails))
					continue;
				if (prop.PropertyType == typeof(IList<BitEntry>))
					continue;
				// {Column}Tooltip properties are the hover text for their sibling column
				// (MetadataCellTooltip resolves them on cell hover), not data in their own
				// right -- they must not surface as standalone columns.
				if (prop.Name.EndsWith("Tooltip", StringComparison.Ordinal))
					continue;
				list.Add(new ColumnDescriptor(prop, prop.GetCustomAttribute<ColumnInfoAttribute>()));
			}
			return list.ToArray();
		}

		static DataGridColumn BuildColumn(ColumnDescriptor d, ColumnFilter? filter, string? pageKey)
		{
			var column = d.Info?.Kind == ColumnKind.Token
				? (DataGridColumn)BuildTokenColumn(d.Property, d.Info)
				: BuildTextColumn(d.Property, d.Info);
			column.Header = filter is null ? d.Name : (object)BuildHeader(d.Name, filter, d.Property.PropertyType, pageKey);
			// Header is a Panel when filters are wired, so callers can't recover the column
			// name via Header.ToString(). Stash the name on Tag for hover-tooltip lookup,
			// "Go to token" context-menu resolution, and any future cell-level navigation.
			column.Tag = d.Name;
			return column;
		}

		static Control BuildHeader(string columnName, ColumnFilter filter, Type propertyType, string? pageKey)
		{
			// Header is a single row: the funnel icon docks right; the remaining space is
			// shared by the column-name label and the filter input, with exactly one of
			// them visible at a time. The input only takes over the slot when the user
			// asks for it — clicking the funnel focuses the textbox, after which it stays
			// visible while focused or while a filter is set — so brushing the cursor
			// across the column name does not swap the label out. For [Flags] columns the
			// funnel is just the trigger for the filter flyout attached to it.
			var label = new TextBlock {
				Text = columnName,
				FontWeight = FontWeight.SemiBold,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				VerticalAlignment = VerticalAlignment.Center,
			};
			// Two geometries we toggle between based on filter state. Funnel = idle/inactive;
			// X = a filter is set, clicking the icon clears it.
			var funnelGeometry = global::Avalonia.Media.Geometry.Parse("M0,0 L10,0 L6,4 L6,9 L4,9 L4,4 Z");
			var xGeometry = global::Avalonia.Media.Geometry.Parse("M1,1 L9,8 M9,1 L1,8");

			var filterIcon = new global::Avalonia.Controls.Shapes.Path {
				// Funnel: 10×9 trapezoid above a 2×5 spout. Drawn directly because the
				// Simple theme doesn't ship a filter glyph and pulling FontAwesome for a
				// single icon isn't worth it.
				Width = 10,
				Height = 9,
				Stretch = Stretch.None,
				Fill = Brushes.Gray,
				Data = funnelGeometry,
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center,
				IsHitTestVisible = false,
			};
			// Wrap the funnel in a transparent-background Border with generous padding so the
			// hit area is comfortably larger than the 10×9 glyph itself; clicking the icon
			// opens the popup (flags columns) or focuses the textbox (text columns) without
			// having to wait for the hover-show. When a filter is active the icon turns into
			// an X and the click clears the filter instead.
			var filterIconHost = new Border {
				Background = Brushes.Transparent,
				Padding = new Thickness(8, 4),
				Child = filterIcon,
				VerticalAlignment = VerticalAlignment.Stretch,
				HorizontalAlignment = HorizontalAlignment.Right,
				Cursor = new global::Avalonia.Input.Cursor(global::Avalonia.Input.StandardCursorType.Hand),
			};
			// Tooltip on the funnel hit area. Avalonia 11+ ships ToolTip as an attached
			// property; setting it here means hovering the funnel (10×9 glyph + 8/4 padding)
			// surfaces "Filter <ColumnName>" — useful because the glyph itself has no text.
			global::Avalonia.Controls.ToolTip.SetTip(filterIconHost, "Filter " + columnName);

			bool isFlagsColumn = propertyType.IsEnum
				&& Attribute.IsDefined(propertyType, typeof(FlagsAttribute));

			Flyout? flyout = null;
			TextBox? textBox = null;
			Control headerContent;
			if (isFlagsColumn)
			{
				// Flags columns: label stays visible always — the funnel icon alone is the
				// flyout trigger, no separate dropdown chevron needed since the flyout
				// carries the entire filter UI. Attached to the funnel itself so the flyout
				// drops below the icon's hit area.
				flyout = BuildFlagsFlyout(filter, propertyType, pageKey, columnName);
				FlyoutBase.SetAttachedFlyout(filterIconHost, flyout);
				headerContent = label;
			}
			else
			{
				// Text columns: focusing the TextBox (via funnel click) or having an active
				// filter reveals it in the same slot as the label; the label hides while the
				// input is shown so the column header stays one row. The TextBox stays in
				// the layout pass at rest (Opacity=0 instead of IsVisible=false) so the
				// header row's height is pinned to the larger of the two children — without
				// this, showing the TextBox would grow the row and bounce the layout.
				textBox = BuildFilterTextBox(filter);
				textBox.Opacity = 0;
				textBox.IsHitTestVisible = false;
				var swap = new Panel { HorizontalAlignment = HorizontalAlignment.Stretch };
				swap.Children.Add(textBox);
				swap.Children.Add(label);
				headerContent = swap;
			}

			var headerRow = new DockPanel { LastChildFill = true };
			DockPanel.SetDock(filterIconHost, global::Avalonia.Controls.Dock.Right);
			headerRow.Children.Add(filterIconHost);
			headerRow.Children.Add(headerContent);

			filterIconHost.PointerPressed += (_, e) => {
				if (flyout != null)
				{
					// Flag columns: the funnel always opens the flyout. Modifying the filter
					// requires the chip UI inside the flyout, and clearing is owned by the
					// flyout's own Clear button — calling FlagsState.Clear() from out here
					// would leave the chip IsChecked state stale because we'd skip the
					// SyncFromState() pass that FlagsFilterPopup.Clear() does.
					FlyoutBase.ShowAttachedFlyout(filterIconHost);
				}
				else
				{
					// Text columns: the icon doubles as a clear button when a filter is active
					// (icon visually morphs into an X via Update()). Click while active wipes
					// the TextBox; click while inactive focuses it.
					bool isActive = !string.IsNullOrWhiteSpace(filter.Text);
					if (isActive)
						filter.Text = string.Empty;
					else if (textBox != null)
						textBox.Focus();
				}
				e.Handled = true;
			};

			var root = new StackPanel { Orientation = Orientation.Vertical };
			root.Children.Add(headerRow);

			bool popupOpen = false;
			bool focusInside = false;
			void Update()
			{
				bool active = !string.IsNullOrWhiteSpace(filter.Text)
					|| (filter.FlagsState != null && !filter.FlagsState.IsEmpty);
				bool showInput = active || popupOpen || focusInside;
				if (textBox != null)
				{
					// Text columns swap label / TextBox in the same slot. The TextBox stays in
					// the layout pass either way (see swap construction above); we toggle its
					// Opacity + hit-test instead of IsVisible so the row height is stable.
					textBox.Opacity = showInput ? 1 : 0;
					textBox.IsHitTestVisible = showInput;
					label.IsVisible = !showInput;
				}
				// Flag columns leave label IsVisible alone — it stays on for sort clicks while
				// the funnel icon owns the popup-open gesture.

				// Icon affordance: the X form indicates "click to clear". That only matches
				// the click behaviour on text columns (where clicking when active does clear);
				// for flag columns the click always opens the flyout, so we keep the funnel
				// shape and just tint it SteelBlue to signal the filter is active.
				bool useXForm = active && flyout is null;
				if (useXForm)
				{
					filterIcon.Data = xGeometry;
					filterIcon.Fill = null;
					filterIcon.Stroke = Brushes.SteelBlue;
					filterIcon.StrokeThickness = 1.5;
				}
				else
				{
					filterIcon.Data = funnelGeometry;
					filterIcon.Fill = active ? Brushes.SteelBlue : Brushes.Gray;
					filterIcon.Stroke = null;
				}
			}
			if (textBox != null)
			{
				// Focus tracking: clicking the funnel calls textBox.Focus(), so we need to
				// react to that to swap label → input. The TextBox also has to stay visible
				// until focus moves elsewhere (or a filter is set). Flag columns don't have
				// a TextBox so this hook is a no-op for them.
				textBox.GotFocus += (_, _) => { focusInside = true; Update(); };
				textBox.LostFocus += (_, _) => { focusInside = false; Update(); };
			}
			filter.PropertyChanged += (_, _) => Update();
			if (flyout != null)
			{
				flyout.Opened += (_, _) => { popupOpen = true; Update(); };
				flyout.Closed += (_, _) => { popupOpen = false; Update(); };
			}
			Update();

			return root;
		}

		static TextBox BuildFilterTextBox(ColumnFilter filter)
		{
			var box = new TextBox {
				MinHeight = 0,
				Padding = new Thickness(2, 1),
				FontWeight = FontWeight.Normal,
				HorizontalAlignment = HorizontalAlignment.Stretch,
				Text = filter.Text,
			};
			// Subscribe to the AvaloniaProperty change notification directly. GetObservable
			// also pushes the initial value at subscribe time, which can race with the
			// header's visual-tree-attach pass; AvaloniaObject.PropertyChanged only fires on
			// actual changes and reaches the same handler regardless of when the box joins
			// or leaves the visual tree.
			box.PropertyChanged += (_, e) => {
				if (e.Property == TextBox.TextProperty && filter.Text != box.Text)
					filter.Text = box.Text;
			};
			filter.PropertyChanged += (_, e) => {
				if (e.PropertyName == nameof(ColumnFilter.Text) && box.Text != filter.Text)
					box.Text = filter.Text;
			};
			return box;
		}

		static Flyout BuildFlagsFlyout(ColumnFilter filter, Type enumType, string? pageKey, string columnName)
		{
			// Schema-driven flyout: FlagsFilterPopup distinguishes mutex sub-ranges
			// (multi-select chips) from independent flags (tri-state pills) and drives
			// ColumnFilter.FlagsState. A Flyout (not a raw Popup) supplies light dismiss,
			// Escape-to-close, focus handling, and theme-correct presenter chrome.
			var schema = ICSharpCode.ILSpy.Metadata.Filters.FlagsSchemaInferer.For(enumType);
			bool freshlyCreated = filter.FlagsState is null;
			filter.FlagsState ??= new ICSharpCode.ILSpy.Metadata.Filters.FilterState(schema);
			// On first build, restore any persisted state for this (table, column).
			// Subsequent opens already carry the live state in-memory. Then subscribe so
			// later mutations write back to SessionSettings.
			if (freshlyCreated && pageKey != null)
				ApplyPersistedFilterState(filter.FlagsState, pageKey, columnName);
			var popupContent = new ICSharpCode.ILSpy.Views.Filters.FlagsFilterPopup(filter.FlagsState);

			var flyoutContent = new ScrollViewer {
				MaxHeight = 400,
				Content = popupContent,
				// Force the arrow cursor on the flyout surface — without this, the
				// EW-resize cursor set on the DataGrid column header's drag-grip can
				// leak into the flyout if the pointer enters from there before Avalonia
				// recomputes the cursor for the new hit-test target.
				Cursor = new global::Avalonia.Input.Cursor(global::Avalonia.Input.StandardCursorType.Arrow),
			};
			// Stop wheel events from bubbling out of the flyout. Without this, scrolling
			// inside the dropdown also scrolls the underlying DataGrid because the
			// PointerWheelChanged event keeps bubbling up the routed-event tree once the
			// inner ScrollViewer has consumed (or ignored) it.
			flyoutContent.AddHandler(global::Avalonia.Input.InputElement.PointerWheelChangedEvent,
				(_, e) => e.Handled = true,
				handledEventsToo: true);
			// Likewise for pointer clicks: the flyout's internal popup is logically
			// parented to the funnel inside the column header, so its routed events bubble
			// on into the DataGridColumnHeader, which treats an unhandled left-button
			// press/release pair as a sort click. Interactive children (chips, the Clear
			// button) handle their own pointer events; swallow whatever reaches the
			// content root unhandled — clicks on padding, hint, and summary text.
			flyoutContent.AddHandler(global::Avalonia.Input.InputElement.PointerPressedEvent,
				(_, e) => e.Handled = true);
			flyoutContent.AddHandler(global::Avalonia.Input.InputElement.PointerReleasedEvent,
				(_, e) => e.Handled = true);

			return new Flyout {
				Content = flyoutContent,
				Placement = PlacementMode.BottomEdgeAlignedLeft,
				ShowMode = FlyoutShowMode.Transient,
			};
		}

		/// <summary>
		/// Restores any saved FilterState XML from SessionSettings into <paramref name="state"/>
		/// on first creation, then subscribes to its property-changed stream so subsequent
		/// mutations write back. Wrapped in a try/catch — composition isn't available in
		/// design-time previews or in some isolated unit-test paths, and failing the
		/// restore is strictly less bad than failing the popup build entirely.
		/// </summary>
		static void ApplyPersistedFilterState(
			ICSharpCode.ILSpy.Metadata.Filters.FilterState state,
			string pageKey,
			string columnName)
		{
			SessionSettings settings;
			try
			{
				settings = AppEnv.AppComposition.Current
					.GetExport<SettingsService>()
					.SessionSettings;
			}
			catch
			{
				return;
			}
			var key = (pageKey, columnName);
			if (settings.FilterStates.TryGetValue(key, out var savedXml))
			{
				ICSharpCode.ILSpy.Metadata.Filters.FilterStatePersistence.ApplyXml(state, savedXml);
			}
			state.PropertyChanged += (_, _) => {
				if (state.IsEmpty)
					settings.FilterStates.Remove(key);
				else
					settings.FilterStates[key] = ICSharpCode.ILSpy.Metadata.Filters.FilterStatePersistence.ToXml(state);
			};
		}

		static DataGridTextColumn BuildTextColumn(PropertyInfo prop, ColumnInfoAttribute? info)
		{
			var binding = new Binding(prop.Name) { Mode = BindingMode.OneWay };
			if (!string.IsNullOrEmpty(info?.Format))
				binding.Converter = new HexFormatConverter(info.Format);
			return new DataGridTextColumn {
				IsReadOnly = true,
				Binding = binding,
				SortMemberPath = prop.Name,
			};
		}

		static DataGridTemplateColumn BuildTokenColumn(PropertyInfo prop, ColumnInfoAttribute? info)
		{
			var format = info?.Format;
			var template = new FuncDataTemplate<object>((row, _) => {
				var btn = new Button {
					Background = Brushes.Transparent,
					BorderThickness = default,
					Padding = new Thickness(2, 0),
					HorizontalAlignment = HorizontalAlignment.Left,
					VerticalAlignment = VerticalAlignment.Center,
					Foreground = Brushes.Blue,
					Cursor = new global::Avalonia.Input.Cursor(global::Avalonia.Input.StandardCursorType.Hand),
					Content = FormatTokenValue(row, prop, format),
				};
				btn.Click += (_, _) => {
					if (btn.FindAncestorOfType<Views.MetadataTablePage>()?.DataContext is MetadataTablePageModel page)
						page.RaiseNavigateToCell(row, prop.Name);
				};
				return btn;
			}, supportsRecycling: true);
			return new DataGridTemplateColumn {
				IsReadOnly = true,
				CellTemplate = template,
				SortMemberPath = prop.Name,
			};
		}

		static string FormatTokenValue(object row, PropertyInfo prop, string? format)
		{
			var value = prop.GetValue(row);
			if (value is null)
				return string.Empty;
			if (value is Enum enumValue)
				value = System.Convert.ChangeType(enumValue, enumValue.GetTypeCode(), CultureInfo.InvariantCulture);
			if (!string.IsNullOrEmpty(format) && value is IFormattable formattable)
				return formattable.ToString(format, CultureInfo.InvariantCulture);
			return value.ToString() ?? string.Empty;
		}

		sealed record ColumnDescriptor(PropertyInfo Property, ColumnInfoAttribute? Info)
		{
			public string Name => Property.Name;
		}
	}

	/// <summary>
	/// Applies a numeric format spec ("X8", "X4", …) at bind-time. Wraps enum values to their
	/// underlying integer first so digit-bearing hex specs don't trip <see cref="Enum.ToString"/>'s
	/// strict format-character whitelist.
	/// </summary>
	internal sealed class HexFormatConverter : IValueConverter
	{
		readonly string format;

		public HexFormatConverter(string format)
		{
			this.format = format;
		}

		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value is null)
				return string.Empty;
			if (value is Enum enumValue)
				value = System.Convert.ChangeType(enumValue, enumValue.GetTypeCode(), CultureInfo.InvariantCulture);
			return value is IFormattable formattable
				? formattable.ToString(format, CultureInfo.InvariantCulture)
				: value.ToString();
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> throw new NotSupportedException();
	}
}
