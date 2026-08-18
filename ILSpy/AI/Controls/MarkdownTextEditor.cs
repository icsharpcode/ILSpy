// Copyright (c) 2026 Dr. Masroor Ehsan

using Avalonia;

using ICSharpCode.ILSpy.TextView;

namespace ICSharpCode.ILSpy.AI.Controls
{
	/// <summary>
	/// Read-only AvaloniaEdit editor configured for markdown syntax highlighting, used by the
	/// AI panes (output, chat, explanation) instead of plain <see cref="Avalonia.Controls.TextBox"/>.
	/// Reuses <see cref="DecompilerTextEditor"/> so highlighting colours flip with the active
	/// theme and the user's font settings are followed live — nothing is re-implemented here.
	/// </summary>
	public class MarkdownTextEditor : DecompilerTextEditor
	{
		public MarkdownTextEditor()
		{
			IsReadOnly = true;
			WordWrap = true;
			ShowLineNumbers = false;
			BorderThickness = new Thickness(0);
			Padding = new Thickness(4);

			// Resolve the built-in MarkDown definition and hand it to the theme manager, exactly
			// like the decompiler surfaces resolve their XSHD definitions. GetByExtension both
			// looks the definition up in AvaloniaEdit's registry (.md -> MarkDown) and registers
			// it as themeable, so the colours follow Light/Dark switches.
			SyntaxHighlighting = HighlightingService.GetByExtension(".md");
		}

		/// <summary>
		/// Sets the whole document content, replacing whatever was there.
		/// </summary>
		public void SetText(string? text)
		{
			Document.Text = text ?? string.Empty;
		}

		/// <summary>
		/// Appends <paramref name="text"/> to the end of the document for streaming.
		/// </summary>
		public void AppendChunk(string text)
		{
			if (!string.IsNullOrEmpty(text))
				AppendText(text);
		}
	}
}

