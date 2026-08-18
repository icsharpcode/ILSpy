// Copyright (c) 2026 Dr. Masroor Ehsan

using System;

using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;

using ICSharpCode.ILSpyX.AI;

namespace ICSharpCode.ILSpy.AI.Controls
{
	/// <summary>
	/// Displays a single chat message: role label, timestamp, and message content with markdown
	/// syntax highlighting via <see cref="MarkdownTextEditor"/>. Replaces the plain TextBlock
	/// row in AIChatPane so streamed assistant replies are re-highlighted as their Content grows.
	/// </summary>
	public sealed partial class ChatMessageControl : UserControl
	{
		const string UserRoleLabel = "You";
		const string AssistantRoleLabel = "Assistant";

		ChatMessage? message;
		string renderedContent = string.Empty;

		public ChatMessageControl()
		{
			InitializeComponent();
			DataContextChanged += OnDataContextChanged;
		}

		void OnDataContextChanged(object? sender, EventArgs e)
		{
			if (message != null)
				message.PropertyChanged -= OnMessagePropertyChanged;

			message = DataContext as ChatMessage;
			if (message != null)
			{
				message.PropertyChanged += OnMessagePropertyChanged;
				ApplyMessage();
			}
			else
			{
				RoleLabel.Text = string.Empty;
				TimestampLabel.Text = string.Empty;
				ContentEditor.SetText(string.Empty);
				renderedContent = string.Empty;
			}
		}

		void OnMessagePropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			// React to Content mutations (streaming) plus Role/Timestamp changes so the row
			// always mirrors the underlying message.
			if (e.PropertyName is nameof(ChatMessage.Content)
				or nameof(ChatMessage.Role)
				or nameof(ChatMessage.TimestampUtc))
			{
				ApplyMessage();
			}
		}

		void ApplyMessage()
		{
			if (message == null)
				return;
			RoleLabel.Text = message.IsAssistant ? AssistantRoleLabel : UserRoleLabel;
			TimestampLabel.Text = FormatTimestamp(message.TimestampUtc);
			if (!string.Equals(renderedContent, message.Content, StringComparison.Ordinal))
			{
				if (message.Content.StartsWith(renderedContent, StringComparison.Ordinal))
				{
					ContentEditor.AppendChunk(message.Content[renderedContent.Length..]);
				}
				else
				{
					var snapshot = AIEditorScrollState.Capture(ContentEditor.EditorScrollViewer, followTail: false);
					ContentEditor.SetText(message.Content);
					if (ContentEditor.EditorScrollViewer is { } viewer)
						Dispatcher.UIThread.Post(() => AIEditorScrollState.Restore(viewer, snapshot), DispatcherPriority.Loaded);
				}
				renderedContent = message.Content;
			}
			// A streamed reply that hasn't produced content yet still needs a stable row; hide
			// the editor only so the empty assistant bubble doesn't collapse to a thin sliver.
			ContentEditor.IsVisible = message.Content.Length != 0;
		}

		static string FormatTimestamp(DateTimeOffset value)
		{
			return value.ToLocalTime().ToString("HH:mm");
		}
	}
}
