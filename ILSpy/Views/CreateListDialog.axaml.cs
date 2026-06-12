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

using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ICSharpCode.ILSpy.Views
{
	/// <summary>
	/// Modal prompt for a single text field — used by ManageAssemblyListsDialog's
	/// New / Clone / Rename / Add-Preconfigured flows. Returns the entered name via
	/// <see cref="Window.ShowDialog{TResult}"/>'s string result, or <c>null</c> if
	/// the user cancelled. Title and initial text are passed in by the caller so the
	/// same window backs all four flows.
	/// </summary>
	public partial class CreateListDialog : Window
	{
		TextBox listNameBox = null!;
		Button okButton = null!;

		public CreateListDialog()
		{
			InitializeComponent();
			listNameBox = this.FindControl<TextBox>("ListNameBox")!;
			okButton = this.FindControl<Button>("OkButton")!;
			listNameBox.TextChanged += (_, _) => okButton.IsEnabled = !string.IsNullOrWhiteSpace(listNameBox.Text);
			okButton.Click += (_, _) => Close(listNameBox.Text);
			((Button)this.FindControl<Button>("CancelButton")!).Click += (_, _) => Close(null);
		}

		public CreateListDialog(string title, string? initialText = null) : this()
		{
			Title = title;
			if (!string.IsNullOrEmpty(initialText))
			{
				listNameBox.Text = initialText;
				listNameBox.SelectAll();
			}
		}

		void InitializeComponent() => AvaloniaXamlLoader.Load(this);
	}
}
