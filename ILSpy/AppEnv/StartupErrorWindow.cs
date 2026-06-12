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

using System.Collections.Generic;

using global::Avalonia;
using global::Avalonia.Controls;
using global::Avalonia.Input;
using global::Avalonia.Input.Platform;
using global::Avalonia.Layout;
using global::Avalonia.Media;

using CommunityToolkit.Mvvm.Input;

namespace ICSharpCode.ILSpy.AppEnv
{
	/// <summary>
	/// Shown in place of the main window when startup fails before the dispatcher pump begins.
	/// Without this, a CompositionFailedException (e.g. an unresolvable plugin dependency)
	/// would propagate out of App.OnFrameworkInitializationCompleted and the process would
	/// die before <see cref="GlobalExceptionHandler"/> could surface a dialog.
	/// </summary>
	internal sealed class StartupErrorWindow : Window
	{
		public StartupErrorWindow(IList<ExceptionData> exceptions)
		{
			Title = "ILSpy -- startup failed";
			Width = 720;
			Height = 480;
			WindowStartupLocation = WindowStartupLocation.CenterScreen;

			var clipboardText = StartupExceptions.Format();
			var details = new TextBox {
				IsReadOnly = true,
				AcceptsReturn = true,
				TextWrapping = TextWrapping.NoWrap,
				FontFamily = new FontFamily("Consolas, Menlo, Monospace"),
				FontSize = 12,
				Text = clipboardText,
			};

			var summary = new TextBlock {
				Margin = new Thickness(0, 0, 0, 8),
				FontWeight = FontWeight.Bold,
				TextWrapping = TextWrapping.Wrap,
				Text = exceptions.Count == 1
					? "ILSpy could not start because of a startup error."
					: $"ILSpy could not start: {exceptions.Count} startup errors occurred.",
			};

			var copy = new Button { Content = "Copy" };
			copy.Click += (_, _) => CopyToClipboard(clipboardText);

			var exit = new Button { Content = "Exit", IsDefault = true };
			exit.Click += (_, _) => Close();

			var buttons = new StackPanel {
				Orientation = Orientation.Horizontal,
				HorizontalAlignment = HorizontalAlignment.Right,
				Margin = new Thickness(0, 8, 0, 0),
				Spacing = 8,
				Children = { copy, exit },
			};

			var grid = new Grid {
				Margin = new Thickness(12),
				RowDefinitions = new RowDefinitions("Auto,*,Auto"),
			};
			Grid.SetRow(summary, 0);
			Grid.SetRow(details, 1);
			Grid.SetRow(buttons, 2);
			grid.Children.Add(summary);
			grid.Children.Add(details);
			grid.Children.Add(buttons);

			Content = grid;

			KeyBindings.Add(new KeyBinding {
				Gesture = new KeyGesture(Key.C, KeyModifiers.Control),
				Command = new RelayCommand(
					() => CopyToClipboard(clipboardText),
					() => string.IsNullOrEmpty(details.SelectedText)),
			});
			KeyBindings.Add(new KeyBinding {
				Gesture = new KeyGesture(Key.Escape),
				Command = new RelayCommand(Close),
			});
		}

		void CopyToClipboard(string text)
		{
			var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
			if (clipboard != null)
				_ = clipboard.SetTextAsync(text);
		}
	}
}
