// Copyright (c) 2026 Dr. Masroor Ehsan

using System.Collections.Generic;

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.ILSpy.AI.Controls;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class MarkdownTextEditorFenceTests
{
	[AvaloniaTest]
	public void ContextMenu_Has_Fence_And_Copy_Actions()
	{
		var editor = new MarkdownTextEditor();
		var menu = editor.ContextMenu;
		menu.Should().NotBeNull();
		var headers = GetMenuHeaders(menu!);
		headers.Should().Contain("Open in Decompiler");
		headers.Should().Contain("Copy Code Block");
		headers.Should().Contain("Copy");
	}

	static List<string> GetMenuHeaders(Avalonia.Controls.ContextMenu menu)
	{
		var result = new List<string>();
		foreach (var item in menu.Items)
		{
			if (item is Avalonia.Controls.MenuItem mi && mi.Header is string header)
				result.Add(header);
		}
		return result;
	}

	[AvaloniaTest]
	public void OpenCodeFenceAtCaret_OutsideFence_DoesNotRaise()
	{
		var editor = new MarkdownTextEditor();
		editor.SetText("# Heading only, no fences.");
		bool raised = false;
		editor.OpenCodeFenceRequested += (_, _) => raised = true;
		editor.CaretOffset = 2;
		editor.OpenCodeFenceAtCaret();
		raised.Should().BeFalse();
	}

	[AvaloniaTest]
	public void OpenCodeFenceAtCaret_Raises_With_CSharp_Fence()
	{
		var editor = new MarkdownTextEditor();
		editor.SetText("# Intro\n\nProse.\n\n```csharp\npublic class A { }\n```\n\nTail.\n");
		MarkdownTextEditor.CodeFenceEventArgs? captured = null;
		editor.OpenCodeFenceRequested += (_, args) => captured = (MarkdownTextEditor.CodeFenceEventArgs)args;
		// Code content line: 0 Heading, 1 blank, 2 Prose, 3 blank, 4 opening fence, 5 code.
		var codeLine = editor.Document!.GetLineByNumber(6);
		editor.CaretOffset = codeLine.Offset;
		editor.OpenCodeFenceAtCaret();
		captured.Should().NotBeNull();
		captured!.Fence.Code.Should().Contain("public class A");
		captured.Fence.IsCSharp.Should().BeTrue();
		captured.SourceMarkdown.Should().Contain("csharp");
	}
}

