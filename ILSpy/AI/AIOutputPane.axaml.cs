// Copyright (c) 2026 Masroor
using System;
using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;

namespace ICSharpCode.ILSpy.AI
{
	public partial class AIOutputPane : UserControl
	{
		public AIOutputPane()
		{
			InitializeComponent();
		}

		async void CopyClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
		{
			if (DataContext is AIOutputPaneModel model)
				await model.CopyToClipboardAsync(TopLevel.GetTopLevel(this)?.Clipboard);
		}
	}
}
