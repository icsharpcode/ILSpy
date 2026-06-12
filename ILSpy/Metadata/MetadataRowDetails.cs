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
using System.Collections;
using System.Collections.Generic;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Media;

using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// Hosts the per-row details content of a metadata DataGrid. The DataGrid builds its
	/// <c>RowDetailsTemplate</c> once per details element and keeps the built control across
	/// row recycling, only swapping the DataContext — a template that picked its control tree
	/// at build time would keep showing the presentation of whatever row it was first built
	/// for. This shell re-runs its content factory on every DataContext change instead, so
	/// the factory can return a different control per row (text blob vs. sub-grid) or none
	/// at all (a nulled DataContext on a recycled row drops the stale content).
	/// </summary>
	public sealed class MetadataRowDetailsControl : ContentControl
	{
		readonly Func<object?, Control?> contentFactory;

		public MetadataRowDetailsControl(Func<object?, Control?> contentFactory)
		{
			this.contentFactory = contentFactory ?? throw new ArgumentNullException(nameof(contentFactory));
		}

		protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
		{
			base.OnPropertyChanged(change);
			if (change.Property == DataContextProperty)
				Content = contentFactory(change.NewValue);
		}
	}

	/// <summary>
	/// Factories for the row-details area of metadata grids: the DataContext-tracking shell
	/// template plus the content shapes shared across tables (flag-bit breakdown, decoded
	/// text blob, typed sub-grid).
	/// </summary>
	public static class MetadataRowDetails
	{
		/// <summary>
		/// Wraps <paramref name="contentFactory"/> in a row-details template. The factory
		/// receives the row item (or <see langword="null"/> while the row is recycled) and
		/// returns the details content for that row, or <see langword="null"/> for rows
		/// without details.
		/// </summary>
		public static IDataTemplate CreateTemplate(Func<object?, Control?> contentFactory)
		{
			ArgumentNullException.ThrowIfNull(contentFactory);
			return new FuncDataTemplate<object?>(
				(_, _) => new MetadataRowDetailsControl(contentFactory),
				supportsRecycling: false);
		}

		/// <summary>
		/// Configures <paramref name="page"/> so every <see cref="Entry"/> row carrying a
		/// flag-bit breakdown (<see cref="Entry.RowDetails"/>) renders it permanently
		/// expanded beneath the row, while all other rows stay detail-less.
		/// </summary>
		public static void ConfigureEntryFlagsDetails(MetadataTablePageModel page)
		{
			ArgumentNullException.ThrowIfNull(page);
			page.RowDetailsTemplate = CreateTemplate(
				static item => item is Entry { RowDetails: { } bits } ? BuildFlagsGrid(bits) : null);
			page.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.Collapsed;
			page.IsRowDetailsVisible = static item => item is Entry { RowDetails: not null };
		}

		/// <summary>Headerless two-column (set? / meaning) breakdown of a flags word.</summary>
		public static Control BuildFlagsGrid(IList<BitEntry> bits)
		{
			ArgumentNullException.ThrowIfNull(bits);
			var grid = CreateInnerGrid(bits);
			grid.HeadersVisibility = DataGridHeadersVisibility.None;
			grid.Columns.Add(new DataGridCheckBoxColumn {
				Binding = new Binding(nameof(BitEntry.Value)),
				IsReadOnly = true,
			});
			grid.Columns.Add(new DataGridTextColumn {
				Binding = new Binding(nameof(BitEntry.Meaning)),
				IsReadOnly = true,
			});
			return grid;
		}

		/// <summary>Read-only, word-wrapped view of a decoded text blob (source, JSON, hex).</summary>
		public static Control BuildTextBlob(string text)
		{
			ArgumentNullException.ThrowIfNull(text);
			return new TextBox {
				Text = text,
				IsReadOnly = true,
				TextWrapping = TextWrapping.Wrap,
				MaxWidth = 800,
				MaxHeight = 400,
				HorizontalAlignment = HorizontalAlignment.Left,
			};
		}

		/// <summary>
		/// Sub-grid over parsed blob rows; one text column per (header, property) pair.
		/// </summary>
		public static Control BuildDetailsGrid(IEnumerable rows, params (string Header, string PropertyName)[] columns)
		{
			ArgumentNullException.ThrowIfNull(rows);
			ArgumentNullException.ThrowIfNull(columns);
			var grid = CreateInnerGrid(rows);
			grid.HeadersVisibility = DataGridHeadersVisibility.Column;
			// Bound, because structured blobs (hoisted scopes, metadata references) can run
			// to hundreds of rows; the sub-grid scrolls instead of growing the host row.
			grid.MaxHeight = 250;
			foreach (var (header, propertyName) in columns)
			{
				grid.Columns.Add(new DataGridTextColumn {
					Header = header,
					Binding = new Binding(propertyName),
					IsReadOnly = true,
				});
			}
			return grid;
		}

		static DataGrid CreateInnerGrid(IEnumerable rows) => new() {
			ItemsSource = rows,
			IsReadOnly = true,
			AutoGenerateColumns = false,
			GridLinesVisibility = DataGridGridLinesVisibility.None,
			CanUserReorderColumns = false,
			CanUserSortColumns = false,
			SelectionMode = DataGridSelectionMode.Single,
			HorizontalAlignment = HorizontalAlignment.Left,
		};
	}
}
