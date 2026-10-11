// Copyright (c) 2026 Piero Viano
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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Commands;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpy.TextView;

namespace ICSharpCode.ILSpy.Navigation
{
	/// <summary>Parses Go to Line input: "line", "line:column" or "line,column", all 1-based.</summary>
	public static class GoToLineInput
	{
		public static bool TryParse(string? text, out int line, out int? column)
		{
			line = 0;
			column = null;
			if (string.IsNullOrWhiteSpace(text))
				return false;
			var parts = text.Trim().Split(':', ',');
			if (parts.Length > 2 || !TryParsePositive(parts[0], out line))
				return false;
			if (parts.Length == 2)
			{
				if (!TryParsePositive(parts[1], out int col))
					return false;
				column = col;
			}
			return true;
		}

		static bool TryParsePositive(string text, out int value)
			=> int.TryParse(text.Trim(), System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out value)
				&& value > 0;
	}

	/// <summary>
	/// The small Go to Line prompt: a text box prefilled with the current line. Enter jumps (and
	/// closes) when the input parses, Escape closes.
	/// </summary>
	public sealed class GoToLineWindow : Window
	{
		readonly TextBox input;
		readonly TextBlock hint;
		readonly Func<int, int?, bool> goToLine;

		public GoToLineWindow(int lineCount, int currentLine, Func<int, int?, bool> goToLine)
		{
			this.goToLine = goToLine ?? throw new ArgumentNullException(nameof(goToLine));
			Title = Properties.Resources.GoToLine.TrimEnd('.');
			SizeToContent = SizeToContent.WidthAndHeight;
			CanResize = false;
			ShowInTaskbar = false;
			WindowStartupLocation = WindowStartupLocation.CenterOwner;

			hint = new TextBlock { Text = $"Line number (1 - {lineCount}), optionally followed by :column" };
			input = new TextBox { Text = currentLine.ToString(System.Globalization.CultureInfo.InvariantCulture), MinWidth = 280 };
			var ok = new Button { Content = Properties.Resources.OK, MinWidth = 80, IsDefault = true };
			var cancel = new Button { Content = Properties.Resources.Cancel, MinWidth = 80, IsCancel = true };
			ok.Click += (_, _) => Accept();
			cancel.Click += (_, _) => Close();
			Content = new StackPanel {
				Margin = new Thickness(12),
				Spacing = 8,
				Children = {
					hint,
					input,
					new StackPanel {
						Orientation = Orientation.Horizontal,
						HorizontalAlignment = HorizontalAlignment.Right,
						Spacing = 8,
						Children = { ok, cancel },
					},
				},
			};
			AddHandler(KeyDownEvent, OnPreviewKeyDown, RoutingStrategies.Tunnel);
			Opened += (_, _) => Dispatcher.UIThread.Post(() => {
				input.Focus();
				input.SelectAll();
			});
		}

		/// <summary>The text typed into the prompt.</summary>
		public string? InputText {
			get => input.Text;
			set => input.Text = value;
		}

		/// <summary>Jumps to the entered position and closes; returns false (and keeps the prompt
		/// open) when the input does not parse.</summary>
		public bool Accept()
		{
			if (!GoToLineInput.TryParse(input.Text, out int line, out int? column))
			{
				hint.Foreground = Brushes.Red;
				return false;
			}
			Close();
			return goToLine(line, column);
		}

		void OnPreviewKeyDown(object? sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				Accept();
				e.Handled = true;
			}
			else if (e.Key == Key.Escape)
			{
				Close();
				e.Handled = true;
			}
		}
	}

	/// <summary>Finds the main window and the decompiler view the in-file commands act on.</summary>
	static class ActiveTextViewLocator
	{
		public static Window? MainWindow
			=> UiContext.MainWindow ?? AppComposition.TryGetExport<Views.MainWindow>();

		/// <summary>The decompiler view holding keyboard focus, if any.</summary>
		public static DecompilerTextView? Focused() => DecompilerTextView.FocusedView;

		/// <summary>The focused decompiler view, or else the one showing the active document.</summary>
		public static DecompilerTextView? FocusedOrActive(Window? window, DockWorkspace dockWorkspace)
		{
			if (Focused() is { } focused)
				return focused;
			if (window == null || dockWorkspace.ActiveContentTabPage?.Content is not DecompilerTabPageModel content)
				return null;
			return window.GetVisualDescendants().OfType<DecompilerTextView>()
				.FirstOrDefault(v => ReferenceEquals(v.DataContext, content) && v.IsEffectivelyVisible);
		}
	}

	/// <summary>
	/// Navigate &gt; Go to Line. Ctrl+G is bound inside the code view only (see
	/// <see cref="DecompilerTextView"/>), because the same gesture is "Go to token" in metadata
	/// grids; the menu entry therefore carries no window-wide gesture.
	/// </summary>
	[ExportMainMenuCommand(ParentMenuID = nameof(Resources._Navigate), Header = nameof(Resources.GoToLine), MenuCategory = "InFile", MenuOrder = 60)]
	[Shared]
	[method: ImportingConstructor]
	sealed class GoToLineCommand(DockWorkspace dockWorkspace) : SimpleCommand
	{
		public override bool CanExecute(object? parameter)
			=> dockWorkspace.ActiveContentTabPage?.Content is DecompilerTabPageModel { Text.Length: > 0 };

		public override void Execute(object? parameter)
			=> ActiveTextViewLocator.FocusedOrActive(ActiveTextViewLocator.MainWindow, dockWorkspace)?.ShowGoToLinePrompt();
	}
}
