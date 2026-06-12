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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;

namespace ICSharpCode.ILSpy.AppEnv
{
	/// <summary>
	/// The choice a developer makes when a <c>Debug.Assert</c> fires (see <see cref="ILSpyTraceListener"/>).
	/// </summary>
	enum AssertionAction
	{
		/// <summary>Raise an <see cref="AssertionFailedException"/> so the failure propagates up the call stack.</summary>
		Throw,
		/// <summary>Break into an attached debugger via <see cref="System.Diagnostics.Debugger.Break"/>.</summary>
		Debug,
		/// <summary>Continue past this one assert.</summary>
		Ignore,
		/// <summary>Continue and suppress every further assert raised from the same call site this session.</summary>
		IgnoreAll,
	}

	/// <summary>
	/// Modal dialog shown when a <c>Debug.Assert</c> fails, offering Throw / Debug / Ignore / Ignore All.
	/// Asserts fire on whichever thread is decompiling (usually a background thread), so <see cref="Show"/>
	/// marshals to the UI thread and blocks the caller via a nested dispatcher frame until the developer
	/// picks an action, then returns that choice to the asserting thread.
	/// </summary>
	static class AssertionFailedDialog
	{
		public static AssertionAction Show(string message, string? detailMessage, string stackTrace)
		{
			// Dispatcher.Invoke runs inline when already on the UI thread and otherwise blocks the
			// calling (decompiler) thread until the dialog closes.
			return Dispatcher.UIThread.Invoke(() => ShowOnUIThread(message, detailMessage, stackTrace));
		}

		static AssertionAction ShowOnUIThread(string message, string? detailMessage, string stackTrace)
		{
			Window? owner = null;
			if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
				owner = desktop.MainWindow;
			if (owner is not { IsVisible: true })
			{
				// No window to host the dialog (headless, startup, or shutdown). Surface the assert
				// as an exception rather than blocking on a frame that nothing will ever exit; in a
				// test run this fails the test instead of silently passing over the assertion.
				return AssertionAction.Throw;
			}

			// Esc / closing the window with no button maps to Ignore.
			var result = AssertionAction.Ignore;
			var window = new Window {
				Title = "Assertion Failed",
				Width = 720,
				Height = 480,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				ShowInTaskbar = true, // the assert interrupts decompilation; make the dialog easy to find
			};

			void Pick(AssertionAction action)
			{
				result = action;
				window.Close();
			}

			window.Content = BuildContent(message, detailMessage, stackTrace, Pick);
			window.KeyBindings.Add(new KeyBinding {
				Gesture = new KeyGesture(Key.Escape),
				Command = new CommunityToolkit.Mvvm.Input.RelayCommand(window.Close),
			});

			// A nested dispatcher frame keeps the UI responsive while we wait, without returning to
			// the caller (which may be a background decompiler thread blocked inside Dispatcher.Invoke).
			var frame = new DispatcherFrame();
			window.Closed += (_, _) => frame.Continue = false;
			window.Show(owner);
			Dispatcher.UIThread.PushFrame(frame);
			return result;
		}

		static Control BuildContent(string message, string? detailMessage, string stackTrace, Action<AssertionAction> pick)
		{
			var summary = new TextBlock {
				Margin = new Thickness(0, 0, 0, 8),
				FontWeight = FontWeight.Bold,
				TextWrapping = TextWrapping.Wrap,
				Text = message,
			};

			var detailText = string.IsNullOrEmpty(detailMessage)
				? stackTrace
				: detailMessage + Environment.NewLine + Environment.NewLine + stackTrace;
			var details = new TextBox {
				IsReadOnly = true,
				AcceptsReturn = true,
				TextWrapping = TextWrapping.NoWrap,
				FontFamily = new FontFamily("Consolas, Menlo, Monospace"),
				FontSize = 12,
				Text = detailText,
			};

			var buttons = new StackPanel {
				Orientation = Orientation.Horizontal,
				HorizontalAlignment = HorizontalAlignment.Right,
				Margin = new Thickness(0, 8, 0, 0),
				Spacing = 8,
				Children = {
					MakeButton("Throw", () => pick(AssertionAction.Throw), isDefault: false),
					MakeButton("Debug", () => pick(AssertionAction.Debug), isDefault: false),
					MakeButton("Ignore", () => pick(AssertionAction.Ignore), isDefault: true),
					MakeButton("Ignore All", () => pick(AssertionAction.IgnoreAll), isDefault: false),
				},
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
			return grid;
		}

		static Button MakeButton(string text, Action onClick, bool isDefault)
		{
			var button = new Button {
				Content = text,
				IsDefault = isDefault,
				MinWidth = 80,
			};
			button.Click += (_, _) => onClick();
			return button;
		}
	}
}
