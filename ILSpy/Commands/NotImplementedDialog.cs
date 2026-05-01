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

using System.Linq;

using global::Avalonia.Controls;
using global::Avalonia.Controls.ApplicationLifetimes;
using global::Avalonia.Layout;

namespace ILSpy.Commands
{
	/// <summary>
	/// Lightweight modal "Not yet implemented" dialog. Used by menu commands whose underlying
	/// feature (Options dialog, About page, GAC browser, save dialogs, &hellip;) hasn't been ported
	/// from WPF to Avalonia yet — the menu still surfaces the entry so the structure mirrors
	/// the WPF host, but invocation tells the user clearly that the feature is unavailable.
	/// </summary>
	internal static class NotImplementedDialog
	{
		public static void Show(string feature)
		{
			var owner = (global::Avalonia.Application.Current?.ApplicationLifetime
				as IClassicDesktopStyleApplicationLifetime)?.MainWindow;

			var dialog = new Window {
				Title = "ILSpy",
				Width = 380,
				Height = 140,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				CanResize = false,
				ShowInTaskbar = false,
			};

			var ok = new Button {
				Content = "OK",
				IsDefault = true,
				HorizontalAlignment = HorizontalAlignment.Right,
				MinWidth = 80,
				Margin = new global::Avalonia.Thickness(0, 16, 0, 0),
			};
			ok.Click += (_, _) => dialog.Close();

			var stack = new StackPanel {
				Margin = new global::Avalonia.Thickness(16),
				Children = {
					new TextBlock {
						Text = $"\"{feature}\" hasn't been ported to the Avalonia UI yet.",
						TextWrapping = global::Avalonia.Media.TextWrapping.Wrap,
					},
					ok,
				},
			};

			dialog.Content = stack;

			if (owner != null)
				dialog.ShowDialog(owner);
			else
				dialog.Show();
		}
	}
}
