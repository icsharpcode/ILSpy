// Copyright (c) 2026 Dr. Masroor Ehsan

using System;

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;

using AvaloniaEdit.Document;

using ICSharpCode.ILSpy.AppEnv;
using ICSharpCode.ILSpy.Docking;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpyX.AI;

namespace ICSharpCode.ILSpy.AI.Controls
{
	/// <summary>
	/// Read-only AvaloniaEdit editor configured for markdown syntax highlighting, used by the
	/// AI panes (output, chat, explanation) instead of plain <see cref="Avalonia.Controls.TextBox"/>.
	/// Reuses <see cref="DecompilerTextEditor"/> so highlighting colours flip with the active
	/// theme and the user's font settings are followed live. Also hosts the code-fence actions
	/// ("Open in Decompiler" and "Copy Code Block") that operate on the fenced block under the caret.
	/// </summary>
	public class MarkdownTextEditor : DecompilerTextEditor
	{
		const string OpenInDecompilerHeader = "Open in Decompiler";
		const string CopyCodeBlockHeader = "Copy Code Block";
		const string CodeBlockCopiedMessage = "Code block copied to clipboard";

		/// <summary>
		/// Raised when the user asks to open the code fence under the caret in a new decompiler tab.
		/// Hosting panes can subscribe to surface their own handling; the default handler opens the
		/// block in a frozen tab via <see cref="DockWorkspace.ShowTextInNewTab"/>.
		/// </summary>
		public event EventHandler<CodeFenceEventArgs>? OpenCodeFenceRequested;

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

			BuildContextMenu();
		}

		/// <summary>Sets the whole document content, replacing whatever was there.</summary>
		public void SetText(string? text)
		{
			Document.Text = text ?? string.Empty;
		}

		/// <summary>Appends <paramref name="text"/> to the end of the document for streaming.</summary>
		public void AppendChunk(string text)
		{
			if (!string.IsNullOrEmpty(text))
				AppendText(text);
		}

		/// <summary>
		/// Opens the code fence under the caret in a new decompiler tab. No-op when the caret is not
		/// inside a fenced code block or the dock workspace is unavailable.
		/// </summary>
		public void OpenCodeFenceAtCaret()
		{
			var fence = GetFenceAtCaret();
			if (fence == null || Document == null)
				return;

			var args = new CodeFenceEventArgs { Fence = fence, SourceMarkdown = Document.Text };
			var handler = OpenCodeFenceRequested;
			if (handler != null)
				handler(this, args);
			else
				OpenInDecompiler(fence);
		}

		/// <summary>
		/// Copies the code fence under the caret (without the surrounding fence marker) to the
		/// clipboard. No-op when the caret is not inside a fenced code block.
		/// </summary>
		public void CopyCodeFenceAtCaret()
		{
			var fence = GetFenceAtCaret();
			if (fence == null)
				return;
			_ = TopLevel.GetTopLevel(this)?.Clipboard?.SetTextAsync(fence.Code);
			ShowTransientTooltip(CodeBlockCopiedMessage);
		}

		MarkdownCodeFenceExtractor.CodeFence? GetFenceAtCaret()
		{
			if (Document == null)
				return null;
			int zeroBasedLine = Document.GetLineByOffset(CaretOffset).LineNumber - 1;
			return MarkdownCodeFenceExtractor.FindFenceAtLine(Document.Text, zeroBasedLine);
		}

		void OpenInDecompiler(MarkdownCodeFenceExtractor.CodeFence fence)
		{
			var dockWorkspace = AppComposition.TryGetExport<DockWorkspace>();
			if (dockWorkspace == null)
				return;

			var output = new AvaloniaEditTextOutput {
				Title = "AI Code Snippet",
				SyntaxExtensionOverride = fence.IsCSharp ? ".cs" : fence.IsIL ? ".il" : ".txt",
			};
			output.Write(fence.Code);
			dockWorkspace.ShowTextInNewTab("AI Code Snippet", output);
		}

		/// <summary>
		/// Shows a short-lived tooltip on the editor. Used for lightweight feedback that an action
		/// performed on a code fence succeeded.
		/// </summary>
		void ShowTransientTooltip(string message)
		{
			ToolTip.SetTip(this, message);
			ToolTip.SetIsOpen(this, true);
			var timer = new global::System.Threading.Timer(_ => {
				Avalonia.Threading.Dispatcher.UIThread.Post(() => ToolTip.SetIsOpen(this, false));
			}, null, 2000, global::System.Threading.Timeout.Infinite);
		}

		void BuildContextMenu()
		{
			var openInDecompilerItem = new MenuItem { Header = OpenInDecompilerHeader };
			openInDecompilerItem.Click += (_, _) => OpenCodeFenceAtCaret();
			openInDecompilerItem.InputGesture = new KeyGesture(Key.O, KeyModifiers.Control | KeyModifiers.Shift);

			var copyCodeItem = new MenuItem { Header = CopyCodeBlockHeader };
			copyCodeItem.Click += (_, _) => CopyCodeFenceAtCaret();
			copyCodeItem.InputGesture = new KeyGesture(Key.C, KeyModifiers.Control | KeyModifiers.Shift);

			var copySelectionItem = new MenuItem { Header = "Copy" };
			copySelectionItem.Click += (_, _) => Copy();

			ContextMenu = new ContextMenu { Items = { openInDecompilerItem, copyCodeItem, new Separator(), copySelectionItem } };
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			bool openShortcut = e.Key == Key.O && e.KeyModifiers == (KeyModifiers.Control | KeyModifiers.Shift);
			bool copyShortcut = e.Key == Key.C && e.KeyModifiers == (KeyModifiers.Control | KeyModifiers.Shift);
			if (openShortcut)
			{
				OpenCodeFenceAtCaret();
				e.Handled = true;
			}
			else if (copyShortcut)
			{
				CopyCodeFenceAtCaret();
				e.Handled = true;
			}
		}

		/// <summary>Arguments describing the code fence a user action targets.</summary>
		public sealed class CodeFenceEventArgs : EventArgs
		{
			/// <summary>The fenced block the action applies to.</summary>
			public required MarkdownCodeFenceExtractor.CodeFence Fence { get; init; }

			/// <summary>The full source markdown the fence was extracted from.</summary>
			public required string SourceMarkdown { get; init; }
		}
	}
}
