// Copyright (c) 2026 Dr. Masroor Ehsan

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaEdit.Document;

namespace ICSharpCode.ILSpy.AI
{
	/// <summary>
	/// Hosts a <see cref="Controls.MarkdownTextEditor"/> so AI output gets syntax-highlighted
	/// markdown, and adapts the previous TextBox-based control's public surface: a bindable
	/// <see cref="Text"/> property for whole-content replacement plus <see cref="AppendText"/>
	/// for streaming. Scrolls to the end whenever content is set or appended so the latest
	/// streamed text stays visible.
	/// </summary>
	public partial class StreamingTextControl : UserControl
	{
		public static readonly StyledProperty<string> TextProperty =
			AvaloniaProperty.Register<StreamingTextControl, string>(nameof(Text), string.Empty);

		/// <summary>The whole markdown content shown in the embedded editor.</summary>
		public string Text {
			get => GetValue(TextProperty);
			set => SetValue(TextProperty, value);
		}

		public StreamingTextControl()
		{
			InitializeComponent();
		}

		protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
		{
			base.OnPropertyChanged(change);
			if (change.Property == TextProperty)
			{
				Editor.SetText(GetValue(TextProperty));
				Editor.ScrollToEnd();
			}
		}

		/// <summary>
		/// Appends text to the end of the document — more efficient than replacing the whole
		/// <see cref="Text"/> property while streaming.
		/// </summary>
		public void AppendText(string text)
		{
			Editor.AppendText(text);
			Editor.ScrollToEnd();
		}

		/// <summary>Clears all content from the embedded editor.</summary>
		public void Clear()
		{
			Editor.SetText(string.Empty);
		}
	}
}
