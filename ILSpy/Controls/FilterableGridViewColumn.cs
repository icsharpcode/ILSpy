using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ICSharpCode.ILSpy.Controls
{
	class FilterableGridViewColumn : SortableGridViewColumn
	{
		static readonly ComponentResourceKey headerTemplateKey = new ComponentResourceKey(typeof(FilterableGridViewColumn), "ColumnHeaderTemplate");


		public FilterableGridViewColumn()
		{
			this.SetValueToExtension(HeaderTemplateProperty, new DynamicResourceExtension(headerTemplateKey));
		}

		string filterBy;

		public string FilterBy {
			get { return filterBy; }
			set {
				if (filterBy != value) {
					filterBy = value;
					OnPropertyChanged(new PropertyChangedEventArgs("FilterBy"));
				}
			}
		}

		string filterFormatString;

		public string FilterFormatString {
			get { return filterFormatString; }
			set {
				if (filterFormatString != value) {
					filterFormatString = value;
					OnPropertyChanged(new PropertyChangedEventArgs("FilterFormatString"));
				}
			}
		}

		public static ListView GetParentView(DependencyObject obj)
		{
			return (ListView)obj.GetValue(ParentViewProperty);
		}

		public static void SetParentView(DependencyObject obj, ListView value)
		{
			obj.SetValue(ParentViewProperty, value);
		}

		public static readonly DependencyProperty ParentViewProperty =
			DependencyProperty.RegisterAttached("ParentView", typeof(ListView), typeof(FilterableGridViewColumn), new PropertyMetadata(null));

		public string FilterExpression {
			get { return (string)GetValue(FilterExpressionProperty); }
			set { SetValue(FilterExpressionProperty, value); }
		}

		public static readonly DependencyProperty FilterExpressionProperty =
			DependencyProperty.Register("FilterExpression", typeof(string), typeof(FilterableGridViewColumn), new PropertyMetadata(null, OnFilterExpressionChanged));

		private static void OnFilterExpressionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var header = d as FilterableGridViewColumn;
			var view = GetParentView(header);
			if (header == null || view == null)
				return;
			header.Filter(view, e.NewValue as string);
		}

		void Filter(ListView grid, string filterExpression)
		{
			ColumnSortDirection currentDirection = GetSortDirection(grid);
			ICollectionView dataView = CollectionViewSource.GetDefaultView(grid.ItemsSource);

			if (dataView == null) return;

			string filterBy = FilterBy;
			if (filterBy == null) {
				Binding binding = DisplayMemberBinding as Binding;
				if (binding != null && binding.Path != null) {
					filterBy = binding.Path.Path;
				}
			}

			dataView.Filter = delegate (object item) {
				if (filterBy == null)
					return true;
				var pInfo = item.GetType().GetProperty(filterBy);
				if (pInfo == null)
					return false;
				return Matches(filterExpression, string.Format("{0:" + FilterFormatString + "}", pInfo.GetValue(item)));
			};

			dataView.Refresh();
		}

		bool Matches(string filterExpression, string value)
		{
			return value?.Contains(filterExpression) == true;
		}
	}
}
