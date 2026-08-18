// Copyright (c) 2026 Dr. Masroor Ehsan

using System.Linq;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AI;
using ICSharpCode.ILSpy.AI.Controls;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class MarkdownTextEditorTests
{
	[AvaloniaTest]
	public void Editor_Is_Configured_For_Markdown_And_ReadOnly()
	{
		var editor = new MarkdownTextEditor();
		editor.IsReadOnly.Should().BeTrue();
		editor.WordWrap.Should().BeTrue();
		editor.ShowLineNumbers.Should().BeFalse();
		editor.SyntaxHighlighting.Should().NotBeNull();
		editor.SyntaxHighlighting!.Name.Should().NotBeNullOrWhiteSpace();
	}

	[AvaloniaTest]
	public void SetText_Replaces_Document_Content()
	{
		var editor = new MarkdownTextEditor();
		editor.SetText("# Heading\n\nSome **bold** text.");
		editor.Document!.Text.Should().Be("# Heading\n\nSome **bold** text.");
	}

	[AvaloniaTest]
	public void AppendChunk_Appends_To_End_Of_Document()
	{
		var editor = new MarkdownTextEditor();
		editor.SetText("# Heading");
		editor.AppendChunk("\n\n```csharp\nvar x = 1;\n```");
		editor.Document!.Text.Should().EndWith("```csharp\nvar x = 1;\n```");
	}

	[AvaloniaTest]
	public void AppendChunk_Ignores_Empty()
	{
		var editor = new MarkdownTextEditor();
		editor.SetText("start");
		editor.AppendChunk(string.Empty);
		editor.Document!.Text.Should().Be("start");
	}

	[AvaloniaTest]
	public void StreamingTextControl_Mirrors_Text_Property_Into_Editor()
	{
		var control = new StreamingTextControl();
		control.Text = "# Hello";
		control.Editor.Document!.Text.Should().Be("# Hello");
		control.AppendText(" world");
		control.Editor.Document.Text.Should().Be("# Hello world");
		control.Clear();
		control.Editor.Document.Text.Should().BeEmpty();
	}
}
