using System;
using System.Collections.Generic;
using System.Globalization;
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

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// Interaction logic for FlagsTooltip.xaml
	/// </summary>
	public partial class FlagsTooltip
	{
		public FlagsTooltip(int value, Type flagsType)
		{
			this.Flags = FlagsFilterControl.CreateFlags(flagsType, includeAll: false);
			InitializeComponent();
			((FlagActiveConverter)Resources["flagActiveConv"]).Value = value;
		}

		public IEnumerable<Flag> Flags { get; }
	}

	class FlagActiveConverter : DependencyObject, IValueConverter
	{
		public int Value {
			get { return (int)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(int), typeof(FlagActiveConverter), new PropertyMetadata(0));

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return Value == (int)value || ((int)value & Value) != 0;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}

	public readonly struct Flag
	{
		public string Name { get; }
		public int Value { get; }

		public Flag(string name, int value)
		{
			this.Name = name;
			this.Value = value;
		}
	}
}
