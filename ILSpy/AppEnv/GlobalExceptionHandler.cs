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

using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.VisualTree;

using CommunityToolkit.Mvvm.Input;

namespace ICSharpCode.ILSpy.AppEnv
{
	public static class GlobalExceptionHandler
	{
		[ThreadStatic]
		static bool showingError;

		public static void Install()
		{
			if (Debugger.IsAttached)
				return;

			AppDomain.CurrentDomain.UnhandledException += OnAppDomainUnhandled;
			TaskScheduler.UnobservedTaskException += OnUnobservedTask;
			Dispatcher.UIThread.UnhandledException += OnDispatcherUnhandled;
		}

		/// <summary>
		/// Surfaces <paramref name="exception"/> through the same UI as an unhandled exception.
		/// Lets call sites that swallow exceptions in a fire-and-forget continuation still report
		/// them through the standard dialog.
		/// </summary>
		public static void Show(Exception exception) => Report(exception);

		static void OnAppDomainUnhandled(object sender, UnhandledExceptionEventArgs e)
		{
			if (e.ExceptionObject is Exception ex)
				Report(ex);
		}

		static void OnUnobservedTask(object? sender, UnobservedTaskExceptionEventArgs e)
		{
			e.SetObserved();
			// The Linux DBus failures arrive here: a fire-and-forget Tmds.DBus call faults and the
			// AggregateException is rethrown by the finalizer. Log the unwrapped chain plus the
			// recent input trail so the triggering gesture can be read off (ILSPY_LOG=DBUSDEBUG).
			LogDBusDiagnostics(e.Exception);
			Report(e.Exception);
		}

		static void LogDBusDiagnostics(Exception exception)
		{
			if (!AppLog.IsEnabled(AppLog.Category.DBusDebug))
				return;
			AppLog.Write(AppLog.Category.DBusDebug,
				"UNOBSERVED TASK EXCEPTION (finalizer thread)" + Environment.NewLine
				+ InputDiagnostics.DescribeException(exception) + Environment.NewLine
				+ "Recent input trail (newest last):" + Environment.NewLine
				+ InputDiagnostics.DumpRecent());
		}

		static void OnDispatcherUnhandled(object sender, DispatcherUnhandledExceptionEventArgs e)
		{
			Report(e.Exception);
			e.Handled = true;
		}

		static void Report(Exception exception)
		{
			Debug.WriteLine(exception.ToString());
			for (var ex = exception; ex != null; ex = ex.InnerException)
			{
				if (ex is ReflectionTypeLoadException rtle && rtle.LoaderExceptions.Length > 0 && rtle.LoaderExceptions[0] is { } loader)
				{
					exception = loader;
					Debug.WriteLine(exception.ToString());
					break;
				}
			}

			if (showingError)
				return;
			showingError = true;
			try
			{
				ShowDialog(exception);
			}
			finally
			{
				showingError = false;
			}
		}

		static void ShowDialog(Exception exception)
		{
			// Marshal to the UI thread; nested calls during shutdown may not have a dispatcher,
			// in which case Debug.WriteLine above is the only signal we can offer.
			if (!Dispatcher.UIThread.CheckAccess())
			{
				try
				{
					Dispatcher.UIThread.Post(() => ShowDialog(exception));
				}
				catch
				{
					// Dispatcher already torn down — nothing to do.
				}
				return;
			}

			var window = new Window {
				Title = "ILSpy — unhandled exception",
				Width = 720,
				Height = 480,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
			};
			var clipboardText = FormatForClipboard(exception);
			var (root, details) = BuildContent(exception, window, clipboardText);
			window.Content = root;
			// Match the Win32 MessageBox keyboard contract: Ctrl+C copies the whole report,
			// Esc dismisses the dialog. Skip the whole-text copy when the user has a selection
			// inside the details TextBox so the standard text-selection copy wins.
			window.KeyBindings.Add(new KeyBinding {
				Gesture = new KeyGesture(Key.C, KeyModifiers.Control),
				Command = new RelayCommand(
					() => CopyToClipboard(window, clipboardText),
					() => string.IsNullOrEmpty(details.SelectedText)),
			});
			window.KeyBindings.Add(new KeyBinding {
				Gesture = new KeyGesture(Key.Escape),
				Command = new RelayCommand(window.Close),
			});

			Window? owner = null;
			if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
				owner = desktop.MainWindow;

			if (owner != null && owner.IsVisible)
				window.ShowDialog(owner);
			else
				window.Show();
		}

		static (Control root, TextBox details) BuildContent(Exception exception, Window window, string clipboardText)
		{
			// For DBus failures the bare ToString() hides the ServiceUnknown/ErrorName detail inside
			// nested aggregates; append the fully unwrapped chain so the dialog carries it too.
			var detailText = exception.ToString();
			if (InputDiagnostics.ContainsDBusException(exception))
				detailText += Environment.NewLine + Environment.NewLine
					+ "--- unwrapped DBus chain ---" + Environment.NewLine
					+ InputDiagnostics.DescribeException(exception);

			var details = new TextBox {
				IsReadOnly = true,
				AcceptsReturn = true,
				TextWrapping = TextWrapping.NoWrap,
				FontFamily = new FontFamily("Consolas, Menlo, Monospace"),
				FontSize = 12,
				Text = detailText,
			};

			var summary = new TextBlock {
				Margin = new Thickness(0, 0, 0, 8),
				FontWeight = FontWeight.Bold,
				TextWrapping = TextWrapping.Wrap,
				Text = exception.GetType().FullName + ": " + exception.Message,
			};

			var copy = new Button { Content = "Copy" };
			copy.Click += (_, _) => CopyToClipboard(window, clipboardText);

			var dismiss = new Button { Content = "Close", IsDefault = true };
			dismiss.Click += (_, _) => window.Close();

			var buttons = new StackPanel {
				Orientation = Orientation.Horizontal,
				HorizontalAlignment = HorizontalAlignment.Right,
				Margin = new Thickness(0, 8, 0, 0),
				Spacing = 8,
				Children = { copy, dismiss },
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
			return (grid, details);
		}

		static string FormatForClipboard(Exception exception)
		{
			var text = exception.GetType().FullName + ": " + exception.Message + Environment.NewLine + exception;
			if (InputDiagnostics.ContainsDBusException(exception))
				text += Environment.NewLine + Environment.NewLine
					+ "--- unwrapped DBus chain ---" + Environment.NewLine
					+ InputDiagnostics.DescribeException(exception);
			return text;
		}

		static void CopyToClipboard(Window window, string text)
		{
			var clipboard = TopLevel.GetTopLevel(window)?.Clipboard;
			if (clipboard != null)
				_ = clipboard.SetTextAsync(text);
		}
	}
}
