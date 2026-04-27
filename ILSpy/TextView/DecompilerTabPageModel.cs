// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Threading;

using AvaloniaEdit.Highlighting;

using CommunityToolkit.Mvvm.ComponentModel;

using ICSharpCode.Decompiler;

using ILSpy.Languages;
using ILSpy.TreeNodes;
using ILSpy.ViewModels;

namespace ILSpy.TextView
{
	/// <summary>
	/// A document tab that hosts decompiled output for a single tree node. Re-decompiles when
	/// <see cref="CurrentNode"/> changes; previous in-flight decompilations are cancelled so a
	/// rapid tree-selection sweep doesn't pile up background work.
	/// </summary>
	public sealed partial class DecompilerTabPageModel : TabPageModel
	{
		CancellationTokenSource? activeCts;

		[ObservableProperty]
		private string text = string.Empty;

		/// <summary>
		/// File extension driving syntax highlighting (e.g. ".cs"). Updated alongside <see cref="Text"/>.
		/// </summary>
		[ObservableProperty]
		private string syntaxExtension = ".cs";

		/// <summary>
		/// Semantic-highlighting spans collected during decompilation; the view feeds this into
		/// AvaloniaEdit's <c>RichTextColorizer</c>.
		/// </summary>
		[ObservableProperty]
		private RichTextModel? highlightingModel;

		ILSpyTreeNode? currentNode;

		public ILSpyTreeNode? CurrentNode {
			get => currentNode;
			set {
				if (currentNode == value)
					return;
				currentNode = value;
				_ = DecompileAsync();
			}
		}

		public DecompilerTabPageModel()
		{
			Title = "Empty";
		}

		public Language Language { get; set; } = null!;

		async Task DecompileAsync()
		{
			activeCts?.Cancel();
			var cts = activeCts = new CancellationTokenSource();
			var node = currentNode;
			var language = Language;
			if (node == null || language == null)
			{
				Text = string.Empty;
				return;
			}

			Title = node.Text?.ToString() ?? "(unnamed)";
			Text = "Decompiling…";
			SyntaxExtension = language.FileExtension;

			try
			{
				var (output, _) = await Task.Run(() => {
					var output = new AvaloniaEditTextOutput();
					var options = new DecompilationOptions { CancellationToken = cts.Token };
					try
					{
						node.Decompile(language, output, options);
					}
					catch (OperationCanceledException)
					{
						// expected on cancel — just return whatever we got
					}
					catch (Exception ex)
					{
						output.WriteLine();
						output.WriteLine("/* Decompilation failed:");
						output.WriteLine(ex.ToString());
						output.WriteLine("*/");
					}
					return (output, cts.Token);
				}, cts.Token).ConfigureAwait(true);

				if (cts.Token.IsCancellationRequested)
					return;

				var rendered = output.GetText();
				var model = output.HighlightingModel;
				await Dispatcher.UIThread.InvokeAsync(() => {
					HighlightingModel = model;
					Text = rendered;
				});
			}
			catch (OperationCanceledException)
			{
				// stale request — drop silently
			}
		}
	}
}
