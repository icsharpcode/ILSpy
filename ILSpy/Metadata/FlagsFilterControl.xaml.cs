using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using DataGridExtensions;

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
			listBox.ItemsSource = CreateFlags(FlagsType);

			var filter = Filter;

			if (filter == null || filter.Mask == -1) {
				listBox?.SelectAll();
			}
		}

		internal static IEnumerable<Flag> CreateFlags(Type flagsType, bool includeAll = true)
		{
			if (includeAll)
				yield return new Flag("<All>", -1);

			foreach (var item in flagsType.GetFields(BindingFlags.Static | BindingFlags.Public)) {
				if (item.Name.EndsWith("Mask", StringComparison.Ordinal))
					continue;
				int value = (int)item.GetRawConstantValue();
				yield return new Flag($"{item.Name} ({value:X4})", value);
			}
		}

		private void Filter_Changed()
		{
			var filter = Filter;

			if (filter == null || filter.Mask == -1) {
				listBox?.SelectAll();
				return;
			}

			if (listBox?.SelectedItems.Count != 0)
				return;

			foreach (var item in listBox.Items.Cast<Flag>()) {
				if ((item.Value & filter.Mask) != 0 || item.Value == 0) {
					listBox.SelectedItems.Add(item);
				}
			}
		}

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.RemovedItems?.OfType<Flag>().Any(f => f.Value == -1) == true) {
				Filter = new FlagsContentFilter(0);
				listBox.UnselectAll();
				return;
			}

			int mask = 0;
			foreach (var item in listBox.SelectedItems.Cast<Flag>()) {
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
			return (Mask & (int)value) != 0;
		}
	}
}
