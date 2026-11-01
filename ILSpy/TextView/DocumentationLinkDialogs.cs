// Copyright (c) 2026 Siegfried Pammer
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
using System.Threading.Tasks;

using Avalonia.Controls;
using Avalonia.Layout;

using ICSharpCode.ILSpy.Options;
using ICSharpCode.ILSpy.Properties;

namespace ICSharpCode.ILSpy.TextView;

static class DocumentationLinkDialogs
{
	public static async Task<bool> ConfirmOpenAsync(Uri uri)
	{
		var settings = AppEnv.AppComposition.TryGetExport<SettingsService>();
		if (settings?.MiscSettings.IsDocumentationLinkOriginAllowed(uri) == true)
			return true;

		var owner = AppEnv.UiContext.MainWindow;
		if (owner == null)
			return false;

		bool open = false;
		bool allowOrigin = false;
		var allowOriginCheckBox = new CheckBox {
			Content = Resources.DocumentationLinkAllowOrigin,
			IsVisible = uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps,
			Margin = new global::Avalonia.Thickness(0, 12, 0, 0),
		};
		var window = new Window {
			Title = Resources.DocumentationLinkConfirmationTitle,
			SizeToContent = SizeToContent.WidthAndHeight,
			CanResize = false,
			WindowStartupLocation = WindowStartupLocation.CenterOwner,
			ShowInTaskbar = false,
		};
		var buttons = new StackPanel {
			Orientation = Orientation.Horizontal,
			HorizontalAlignment = HorizontalAlignment.Right,
			Spacing = 8,
			Margin = new global::Avalonia.Thickness(0, 16, 0, 0),
			Children = {
				new Button { Content = Resources.Cancel, MinWidth = 80, IsCancel = true },
				new Button { Content = Resources.Open, MinWidth = 80 },
			},
		};
		((Button)buttons.Children[0]).Click += (_, _) => window.Close();
		((Button)buttons.Children[1]).Click += (_, _) => {
			open = true;
			allowOrigin = allowOriginCheckBox.IsChecked == true;
			window.Close();
		};
		window.Content = new StackPanel {
			Margin = new global::Avalonia.Thickness(16),
			MaxWidth = 640,
			Children = {
				new TextBlock { Text = Resources.DocumentationLinkConfirmationMessage, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap },
				new SelectableTextBlock { Text = uri.AbsoluteUri, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap, Margin = new global::Avalonia.Thickness(0, 8, 0, 0) },
				allowOriginCheckBox,
				buttons,
			},
		};
		await window.ShowDialog(owner);

		if (open && allowOrigin && settings != null)
			settings.MiscSettings.AllowDocumentationLinkOrigin(uri);
		return open;
	}
}
