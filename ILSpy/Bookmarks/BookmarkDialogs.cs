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

using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Layout;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Properties;

namespace ICSharpCode.ILSpy.Bookmarks
{
	/// <summary>
	/// The two small confirmations the bookmark feature needs: "remove a bookmark whose assembly is
	/// gone?" and "replace or merge on import?". Built in code (no XAML) since they are plain
	/// message + button prompts, and the app has no general-purpose message box.
	/// </summary>
	static class BookmarkDialogs
	{
		public static async Task<bool> ConfirmRemoveMissingAsync()
			=> "yes" == await ShowChoiceAsync(Resources.Bookmarks, Resources.BookmarkAssemblyMissing,
				(Resources._Remove, "yes"), (Resources.Cancel, "no"));

		public static Task InformImportFailedAsync()
			=> ShowChoiceAsync(Resources.BookmarkImportTitle, Resources.BookmarkImportFailed, (Resources.OK, "ok"));

		public static async Task<BookmarkImportMode?> AskImportModeAsync()
		{
			var result = await ShowChoiceAsync(Resources.BookmarkImportTitle, Resources.BookmarkImportReplaceOrMerge,
				(Resources.BookmarkImportReplace, "replace"), (Resources.BookmarkImportMerge, "merge"), (Resources.Cancel, "cancel"));
			return result switch {
				"replace" => BookmarkImportMode.Replace,
				"merge" => BookmarkImportMode.Merge,
				_ => null,
			};
		}

		// Shows a modal prompt with one button per choice; returns the chosen result, or null if the
		// window was closed without a choice.
		static async Task<string?> ShowChoiceAsync(string title, string message, params (string Label, string Result)[] choices)
		{
			var owner = UiContext.MainWindow;
			if (owner == null)
				return null;

			string? result = null;
			var buttons = new StackPanel {
				Orientation = Orientation.Horizontal,
				HorizontalAlignment = HorizontalAlignment.Right,
				Spacing = 8,
				Margin = new global::Avalonia.Thickness(0, 16, 0, 0),
			};
			var window = new Window {
				Title = title,
				SizeToContent = SizeToContent.WidthAndHeight,
				CanResize = false,
				WindowStartupLocation = WindowStartupLocation.CenterOwner,
				ShowInTaskbar = false,
			};
			foreach (var (label, value) in choices)
			{
				var button = new Button { Content = label, MinWidth = 80 };
				button.Click += (_, _) => {
					result = value;
					window.Close();
				};
				buttons.Children.Add(button);
			}
			window.Content = new StackPanel {
				Margin = new global::Avalonia.Thickness(16),
				MaxWidth = 480,
				Children = {
					new TextBlock { Text = message, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap },
					buttons,
				},
			};
			await window.ShowDialog(owner);
			return result;
		}
	}
}
