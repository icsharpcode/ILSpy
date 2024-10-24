// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Composition;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml.Linq;

using ICSharpCode.ILSpyX.Settings;

using TomsToolbox.Wpf.Composition.AttributedModel;
using TomsToolbox.Wpf.Converters;

namespace ICSharpCode.ILSpy.Options
{
	/// <summary>
	/// Interaction logic for DisplaySettingsPanel.xaml
	/// </summary>
	[NonShared]
	[DataTemplate(typeof(DisplaySettingsViewModel))]
	public partial class DisplaySettingsPanel
	{
		public DisplaySettingsPanel()
		{
			InitializeComponent();

			DataObject.AddPastingHandler(tabSizeTextBox, OnPaste);
			DataObject.AddPastingHandler(indentSizeTextBox, OnPaste);
		}

		private void TextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
		{
			if (!e.Text.All(char.IsDigit))
				e.Handled = true;
		}

		private static void OnPaste(object sender, DataObjectPastingEventArgs e)
		{
			if (!e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true))
				return;

			var text = (string)e.SourceDataObject.GetData(DataFormats.UnicodeText, true) ?? string.Empty;

			if (!text.All(char.IsDigit))
				e.CancelCommand();
		}
	}

	public sealed class FontSizeConverter : ValueConverter
	{
		protected override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is double d)
			{
				return Math.Round(d / 4 * 3);
			}

			return DependencyProperty.UnsetValue;
		}

		protected override object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is not string s)
				return DependencyProperty.UnsetValue;

			if (double.TryParse(s, out double d))
				return d * 4 / 3;

			return 11.0 * 4 / 3;
		}
	}
}