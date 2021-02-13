using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using DataGridExtensions;

using ICSharpCode.Decompiler.Util;

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// Interaction logic for FlagsFilterControl.xaml
	/// </summary>
	public partial class FlagsFilterControl
	{
		ListBox listBox;

		public FlagsFilterControl()
		{
			InitializeComponent();
		}

		public FlagsContentFilter Filter {
			get { return (FlagsContentFilter)GetValue(FilterProperty); }
			set { SetValue(FilterProperty, value); }
		}

		public Type FlagsType { get; set; }

		/// <summary>
		/// Identifies the Filter dependency property
		/// </summary>
		public static readonly DependencyProperty FilterProperty =
			DependencyProperty.Register("Filter", typeof(FlagsContentFilter), typeof(FlagsFilterControl),
				new FrameworkPropertyMetadata(new FlagsContentFilter(-1), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (sender, e) => ((FlagsFilterControl)sender).Filter_Changed()));

		/// <summary>When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.</summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			listBox = Template.FindName("ListBox", this) as ListBox;
			if (listBox != null)
			{
				listBox.ItemsSource = FlagGroup.GetFlags(FlagsType, neutralItem: "<All>");
			}

			var filter = Filter;

			if (filter == null || filter.Mask == -1)
			{
				listBox?.SelectAll();
			}
		}

		private void Filter_Changed()
		{
			var filter = Filter;

			if (filter == null || filter.Mask == -1)
			{
				listBox?.SelectAll();
				return;
			}

			if (listBox?.SelectedItems.Count != 0)
				return;

			foreach (var item in listBox.Items.Cast<Flag>())
			{
				if ((item.Value & filter.Mask) != 0 || item.Value == 0)
				{
					listBox.SelectedItems.Add(item);
				}
			}
		}

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.RemovedItems?.OfType<Flag>().Any(f => f.Value == -1) == true)
			{
				Filter = new FlagsContentFilter(0);
				listBox.UnselectAll();
				return;
			}
			if (e.AddedItems?.OfType<Flag>().Any(f => f.Value == -1) == true)
			{
				Filter = new FlagsContentFilter(-1);
				listBox.SelectAll();
				return;
			}

			bool deselectAny = e.RemovedItems?.OfType<Flag>().Any(f => f.Value != -1) == true;

			int mask = 0;
			foreach (var item in listBox.SelectedItems.Cast<Flag>())
			{
				if (deselectAny && item.Value == -1)
					continue;
				mask |= item.Value;
			}

			Filter = new FlagsContentFilter(mask);
		}
	}

	public class FlagsContentFilter : IContentFilter
	{
		public int Mask { get; }

		public FlagsContentFilter(int mask)
		{
			this.Mask = mask;
		}

		public bool IsMatch(object value)
		{
			if (value == null)
				return true;

			return Mask == -1 || (Mask & (int)value) != 0;
		}
	}
}
