using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// Interaction logic for Create.xaml
	/// </summary>
	public partial class CreateListDialog : Window
	{
		public CreateListDialog(string title)
		{
			InitializeComponent();
			this.Title = title;
		}

		private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			okButton.IsEnabled = !string.IsNullOrWhiteSpace(ListNameBox.Text);
		}

		private void OKButton_Click(object sender, RoutedEventArgs e)
		{
			if (!string.IsNullOrWhiteSpace(ListNameBox.Text))
			{
				this.DialogResult = true;
			}
		}

		public string ListName {
			get => ListNameBox.Text;
			set => ListNameBox.Text = value;
		}
	}
}