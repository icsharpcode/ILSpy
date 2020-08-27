// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.ILSpy.Controls
{
	/// <summary>
	/// This class adds the AutoWidth property to the WPF ListView.
	/// It supports a semi-colon-separated list of values, for each defined cell.
	/// Each value can either be a fixed size double, or a percentage.
	/// The sizes of columns with a percentage will be calculated from the
	/// remaining width (after assigning the fixed sizes).
	/// Examples: 50%;25%;25% or 30;100%;50
	/// </summary>
	public class GridViewColumnAutoSize
	{
		// This class was copied from ICSharpCode.Core.Presentation.

		public static readonly DependencyProperty AutoWidthProperty =
			DependencyProperty.RegisterAttached("AutoWidth", typeof(string), typeof(GridViewColumnAutoSize),
												new FrameworkPropertyMetadata(null, AutoWidthPropertyChanged));

		public static string GetAutoWidth(DependencyObject obj)
		{
			return (string)obj.GetValue(AutoWidthProperty);
		}

		public static void SetAutoWidth(DependencyObject obj, string value)
		{
			obj.SetValue(AutoWidthProperty, value);
		}

		static void AutoWidthPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
		{
			ListView grid = sender as ListView;
			if (grid == null)
				return;
			grid.SizeChanged += delegate (object listView, SizeChangedEventArgs e) {
				ListView lv = listView as ListView;
				if (lv == null)
					return;
				GridView v = lv.View as GridView;
				if (v == null)
					return;
				CalculateSizes(v, GetAutoWidth(lv), e.NewSize.Width);
			};
			GridView view = grid.View as GridView;
			if (view == null)
				return;
			CalculateSizes(view, args.NewValue as string, grid.ActualWidth);
		}

		static void CalculateSizes(GridView view, string sizeValue, double fullWidth)
		{
			string[] sizes = (sizeValue ?? "").Split(';');

			if (sizes.Length != view.Columns.Count)
				return;
			Dictionary<int, Func<double, double>> percentages = new Dictionary<int, Func<double, double>>();
			double remainingWidth = fullWidth - 30; // 30 is a good offset for the scrollbar

			for (int i = 0; i < view.Columns.Count; i++)
			{
				var column = view.Columns[i];
				double size;
				bool isPercentage = !double.TryParse(sizes[i], out size);
				if (isPercentage)
				{
					size = double.Parse(sizes[i].TrimEnd('%', ' '));
					percentages.Add(i, w => w * size / 100.0);
				}
				else
				{
					column.Width = size;
					remainingWidth -= size;
				}
			}

			if (remainingWidth < 0)
				return;
			foreach (var p in percentages)
			{
				var column = view.Columns[p.Key];
				column.Width = p.Value(remainingWidth);
			}
		}
	}
}
