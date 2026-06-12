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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.Properties;

using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.Util;

namespace ICSharpCode.ILSpy
{
	public static class SmartTextOutputExtensions
	{
		/// <summary>
		/// Writes the full exception (type, message, stack trace, inner exceptions) inside a
		/// default-collapsed "Exception details" fold. Callers write their own header line first; this
		/// keeps a failure report scannable while leaving the trace one click away when a failure is not
		/// self-explanatory.
		/// </summary>
		public static void WriteExceptionDetails(this ITextOutput output, Exception ex)
		{
			ArgumentNullException.ThrowIfNull(output);
			ArgumentNullException.ThrowIfNull(ex);
			// Blank line between the caller's header and the fold so the collapsed "Exception details"
			// marker is visually separated from the message above it.
			output.WriteLine();
			output.MarkFoldStart("Exception details", true);
			// The fold span is counted in WriteLine() calls, not in the '\n' characters embedded in a
			// single Write(). Emit the trace one line per WriteLine() so the fold genuinely spans
			// multiple lines; otherwise it collapses to a single line and is dropped as noise.
			var lines = ex.ToString().Split('\n');
			for (int i = 0; i < lines.Length; i++)
			{
				if (i > 0)
					output.WriteLine();
				output.Write(lines[i].TrimEnd('\r'));
			}
			output.MarkFoldEnd();
			output.WriteLine();
		}

		/// <summary>
		/// Appends the standard "Open Explorer" result button that opens <paramref name="folder"/> in
		/// the OS file manager, followed by a blank line. Shared tail of the export / save / PDB outputs.
		/// </summary>
		public static void AddOpenFolderButton(this ISmartTextOutput output, string folder)
		{
			output.AddButton(null, Resources.OpenExplorer, (_, _) => ShellHelper.OpenFolder(folder));
			output.WriteLine();
		}

		/// <summary>
		/// Appends an "Open Explorer" result button that reveals <paramref name="file"/> in the OS file
		/// manager (selecting it where supported), followed by a blank line.
		/// </summary>
		public static void AddRevealFileButton(this ISmartTextOutput output, string file)
		{
			output.AddButton(null, Resources.OpenExplorer, (_, _) => ShellHelper.RevealFile(file));
			output.WriteLine();
		}

		public static void AddButton(this ISmartTextOutput output, IImage? icon, string text, EventHandler<RoutedEventArgs> click)
		{
			ArgumentNullException.ThrowIfNull(output);
			ArgumentNullException.ThrowIfNull(click);

			output.AddUIElement(() => {
				var button = new Button {
					Cursor = new Cursor(StandardCursorType.Arrow),
					Margin = new Thickness(2),
					Padding = new Thickness(9, 1, 9, 1),
					MinWidth = 73,
				};
				if (icon != null)
				{
					button.Content = new StackPanel {
						Orientation = Orientation.Horizontal,
						Children = {
							new Image { Width = 16, Height = 16, Source = icon, Margin = new Thickness(0, 0, 4, 0) },
							new TextBlock { Text = text },
						},
					};
				}
				else
				{
					button.Content = text;
				}
				button.Click += click;
				return button;
			});
		}
	}
}
