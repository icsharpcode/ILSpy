// Copyright (c) 2026 Masroor
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ICSharpCode.ILSpy.AI
{
	public partial class StreamingTextControl : UserControl
	{
		public static readonly StyledProperty<string> TextProperty =
			AvaloniaProperty.Register<StreamingTextControl, string>(nameof(Text), string.Empty);

		public string Text {
			get => GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		public StreamingTextControl()
		{
			InitializeComponent();
		}
	}
}
