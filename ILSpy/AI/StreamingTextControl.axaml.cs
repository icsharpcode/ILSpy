// Copyright (c) 2026 Dr. Masroor Ehsan

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.VisualTree;

using AvaloniaEdit.Document;

using ICSharpCode.ILSpy.AI.Controls;

namespace ICSharpCode.ILSpy.AI
{
	/// <summary>
	/// Hosts a <see cref="Controls.MarkdownTextEditor"/> so AI output gets syntax-highlighted
	/// markdown, and adapts the previous TextBox-based control's public surface: a bindable
	/// <see cref="Text"/> property for whole-content replacement plus <see cref="AppendText"/>
	/// for streaming.
	/// </summary>
	public partial class StreamingTextControl : UserControl
	{
		readonly AIFollowTailController followTail = new();
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
			Editor.FollowTailStateProvider = () => followTail.IsFollowingTail;
			Editor.FollowTailStateRestored = followTail.SetFollowingTail;
		}

		protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
		{
			base.OnAttachedToVisualTree(e);
			Avalonia.Threading.Dispatcher.UIThread.Post(() => followTail.Attach(AIEditorScrollState.FindViewer(Editor)), Avalonia.Threading.DispatcherPriority.Loaded);
		}

		protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
		{
			base.OnPropertyChanged(change);
			if (change.Property == TextProperty)
			{
				var snapshot = followTail.Capture();
				Editor.SetText(GetValue(TextProperty));
				followTail.RestoreLater(snapshot);
			}
		}

		/// <summary>
		/// Appends text to the end of the document — more efficient than replacing the whole
		/// <see cref="Text"/> property while streaming.
		/// </summary>
		public void AppendText(string text)
		{
			var snapshot = followTail.Capture();
			Editor.AppendText(text);
			followTail.RestoreLater(snapshot);
		}

		/// <summary>Clears all content from the embedded editor.</summary>
		public void Clear()
		{
			Editor.SetText(string.Empty);
			Avalonia.Threading.Dispatcher.UIThread.Post(followTail.ResetFromViewport, Avalonia.Threading.DispatcherPriority.Loaded);
		}

		protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
		{
			followTail.Dispose();
			base.OnDetachedFromVisualTree(e);
		}
	}
}
