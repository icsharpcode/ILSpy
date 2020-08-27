using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using DataGridExtensions;

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// Interaction logic for HexFilterControl.xaml
	/// </summary>
	public partial class HexFilterControl
	{
		TextBox textBox;

		public HexFilterControl()
		{
			InitializeComponent();
		}

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();
			textBox = Template.FindName("textBox", this) as TextBox;
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			var text = ((TextBox)sender)?.Text;

			Filter = new ContentFilter(text);
		}

		public IContentFilter Filter {
			get { return (IContentFilter)GetValue(FilterProperty); }
			set { SetValue(FilterProperty, value); }
		}
		/// <summary>
		/// Identifies the Filter dependency property
		/// </summary>
		public static readonly DependencyProperty FilterProperty =
			DependencyProperty.Register("Filter", typeof(IContentFilter), typeof(HexFilterControl),
				new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (o, args) => ((HexFilterControl)o).Filter_Changed(args.NewValue)));

		void Filter_Changed(object newValue)
		{
			var textBox = this.textBox;
			if (textBox == null)
				return;

			textBox.Text = (newValue as ContentFilter)?.Value ?? string.Empty;
		}

		class ContentFilter : IContentFilter
		{
			readonly string filter;

			public ContentFilter(string filter)
			{
				this.filter = filter;
			}

			public bool IsMatch(object value)
			{
				if (value == null)
					return false;

				return string.Format("{0:x8}", value).IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0;
			}

			public string Value => filter;
		}
	}
}
