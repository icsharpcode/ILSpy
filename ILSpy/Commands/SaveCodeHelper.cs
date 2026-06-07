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
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.Properties;

using ILSpy.Docking;
using ILSpy.Languages;
using ILSpy.TextView;
using ILSpy.TreeNodes;

namespace ILSpy.Commands
{
	/// <summary>
	/// The shared "Save Code" flow for a single tree node, used by both File -> Save Code (Ctrl+S)
	/// and the Save Code context-menu entry so they behave identically on the node they target.
	/// A node first gets a chance to handle its own save (<see cref="ILSpyTreeNode.Save"/> — e.g.
	/// resource nodes write raw bytes, assemblies offer a project/single-file picker); when it
	/// declines, fall through to a generic decompile-to-single-file save that, mirroring the prior
	/// version, runs in the active decompiler tab with its progress/cancel overlay and then shows a
	/// "decompilation complete" breadcrumb (with an Open-folder button).
	/// </summary>
	internal static class SaveCodeHelper
	{
		/// <summary>
		/// Saves <paramref name="node"/>: lets the node claim the request via its
		/// <see cref="ILSpyTreeNode.Save"/> override, otherwise prompts for a file and decompiles the
		/// node into it, showing progress (and allowing cancellation) in the active decompiler tab.
		/// Does nothing if the user cancels the picker.
		/// </summary>
		public static async Task SaveNodeAsync(ILSpyTreeNode node, LanguageService languageService, DockWorkspace dockWorkspace)
		{
			ArgumentNullException.ThrowIfNull(node);
			ArgumentNullException.ThrowIfNull(languageService);
			ArgumentNullException.ThrowIfNull(dockWorkspace);

			if (node.Save())
				return;

			var language = languageService.CurrentLanguage;
			var defaultName = SuggestedFileName(node) + language.FileExtension;
			var path = await FilePickers.SaveAsync(
				$"{language.Name} (*{language.FileExtension})|*{language.FileExtension}|All files|*.*",
				defaultName).ConfigureAwait(true);
			if (path == null)
				return;

			try
			{
				// Run the decompile-to-file behind the tab's cancellable progress overlay, then show a
				// result breadcrumb + Open-folder button -- the same UX the prior version's SaveToDisk had.
				var output = await dockWorkspace.RunWithCancellation(token => Task.Run(() => {
					var stopwatch = Stopwatch.StartNew();
					WriteNodeToFile(node, language, path, token);
					stopwatch.Stop();

					var o = new AvaloniaEditTextOutput { Title = node.Text?.ToString() ?? string.Empty };
					o.WriteLine(string.Format(Resources.DecompilationCompleteInF1Seconds, stopwatch.Elapsed.TotalSeconds));
					o.WriteLine();
					if (Path.GetDirectoryName(path) is { Length: > 0 } directory)
					{
						o.AddButton(null, Resources.OpenExplorer, (_, _) => OpenFolder(directory));
						o.WriteLine();
					}
					return o;
				}, token), node.Text?.ToString()).ConfigureAwait(true);
				dockWorkspace.ShowText(output);
			}
			catch (OperationCanceledException)
			{
				// User cancelled -- leave the tab content as it was.
			}
		}

		/// <summary>
		/// Re-decompiles <paramref name="node"/> with <see cref="DecompilationOptions.FullDecompilation"/>
		/// on and writes the output to <paramref name="path"/> as plain text. Public so callers (and
		/// tests) can bypass the file picker and the tab progress UI.
		/// </summary>
		public static Task WriteNodeToFileAsync(ILSpyTreeNode node, Language language, string path)
			=> Task.Run(() => WriteNodeToFile(node, language, path, CancellationToken.None));

		static void WriteNodeToFile(ILSpyTreeNode node, Language language, string path, CancellationToken ct)
		{
			ArgumentNullException.ThrowIfNull(node);
			ArgumentNullException.ThrowIfNull(language);

			var options = new DecompilationOptions {
				FullDecompilation = true,
				EscapeInvalidIdentifiers = true,
				CancellationToken = ct,
			};
			using var writer = new StreamWriter(path);
			var output = new PlainTextOutput(writer);
			try
			{
				node.Decompile(language, output, options);
			}
			catch (OperationCanceledException)
			{
				writer.WriteLine();
				writer.WriteLine("// " + Resources.DecompilationWasCancelled);
				throw;
			}
		}

		// A reasonable default file name for the save dialog: the node's text, cleaned of characters
		// that aren't valid in a file name; falls back to "output" when nothing usable remains.
		static string SuggestedFileName(ILSpyTreeNode node)
		{
			var text = node.Text?.ToString();
			if (string.IsNullOrWhiteSpace(text))
				return "output";
			var clean = text;
			foreach (var c in Path.GetInvalidFileNameChars())
				clean = clean.Replace(c, '_');
			clean = clean.Trim();
			return string.IsNullOrEmpty(clean) ? "output" : clean;
		}

		static void OpenFolder(string path)
		{
			try
			{
				if (OperatingSystem.IsWindows())
					Process.Start(new ProcessStartInfo("explorer.exe", $"\"{path}\"") { UseShellExecute = false });
				else if (OperatingSystem.IsMacOS())
					Process.Start(new ProcessStartInfo("open", $"\"{path}\"") { UseShellExecute = false });
				else
					Process.Start(new ProcessStartInfo("xdg-open", path) { UseShellExecute = false });
			}
			catch
			{
				// Best-effort.
			}
		}
	}
}
