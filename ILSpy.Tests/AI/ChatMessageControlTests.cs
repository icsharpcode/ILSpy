// Copyright (c) 2026 Dr. Masroor Ehsan

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AI;
using ICSharpCode.ILSpy.AI.Controls;
using ICSharpCode.ILSpyX.AI;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class ChatMessageControlTests
{
	[AvaloniaTest]
	public void Assistant_Message_Shows_Role_And_Highlighted_Content()
	{
		var control = new ChatMessageControl();
		control.DataContext = new ChatMessage { Role = "assistant", Content = "# Heading\n\n```csharp\nvar x = 1;\n```" };
		control.RoleLabel.Text.Should().Be("Assistant");
		control.ContentEditor.Document!.Text.Should()
			.Be("# Heading\n\n```csharp\nvar x = 1;\n```");
		control.ContentEditor.IsVisible.Should().BeTrue();
	}

	[AvaloniaTest]
	public void User_Message_Shows_You_Role()
	{
		var control = new ChatMessageControl();
		control.DataContext = new ChatMessage { Role = "user", Content = "How does this work?" };
		control.RoleLabel.Text.Should().Be("You");
	}

	[AvaloniaTest]
	public void Content_Change_Refreshes_Editor_While_Streaming()
	{
		var message = new ChatMessage { Role = "assistant" };
		var control = new ChatMessageControl();
		control.DataContext = message;

		message.Content = "First line";
		control.ContentEditor.Document!.Text.Should().Be("First line");
		control.ContentEditor.IsVisible.Should().BeTrue();

		message.Content = "First line\n\nSecond";
		control.ContentEditor.Document.Text.Should().Be("First line\n\nSecond");
	}

	[AvaloniaTest]
	public void Empty_Content_Hides_Editor()
	{
		var control = new ChatMessageControl();
		control.DataContext = new ChatMessage { Role = "assistant" };
		control.ContentEditor.IsVisible.Should().BeFalse();
	}

	[AvaloniaTest]
	public void Editor_Is_Markdown_Configured_And_ReadOnly()
	{
		var control = new ChatMessageControl();
		control.DataContext = new ChatMessage { Role = "user", Content = "text" };
		control.ContentEditor.IsReadOnly.Should().BeTrue();
		control.ContentEditor.SyntaxHighlighting.Should().NotBeNull();
		control.ContentEditor.WordWrap.Should().BeTrue();
	}
}
