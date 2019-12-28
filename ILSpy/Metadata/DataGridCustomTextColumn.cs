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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace ICSharpCode.ILSpy.Metadata
{
	class DataGridCustomTextColumn : DataGridTextColumn
	{
		public BindingBase ToolTipBinding { get; set; }

		protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
		{
			TextBox textBox = new TextBox() { Style = (Style)MetadataTableViews.Instance["DataGridCustomTextColumnTextBoxStyle"] };
			BindingOperations.SetBinding(textBox, TextBox.TextProperty, Binding);
			if (ToolTipBinding != null) {
				textBox.MouseMove += TextBox_MouseMove;
			}
			textBox.GotFocus += TextBox_GotFocus;
			return textBox;
		}

		private void TextBox_GotFocus(object sender, RoutedEventArgs e)
		{
			TextBox tb = (TextBox)sender;
			var cell = tb.GetParent<DataGridCell>();
			var row = cell.GetParent<DataGridRow>();
			row.IsSelected = cell.IsSelected = true;
		}

		private void TextBox_MouseMove(object sender, MouseEventArgs e)
		{
			e.Handled = true;
			var textBox = (TextBox)sender;
			BindingOperations.SetBinding(textBox, TextBox.ToolTipProperty, ToolTipBinding);
			textBox.MouseMove -= TextBox_MouseMove;
		}
	}
}
