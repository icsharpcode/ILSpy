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
using System.Collections.Generic;
using System.Composition;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

using ICSharpCode.ILSpy.Properties;
using ICSharpCode.ILSpyX.TreeView;

using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// Right-click → "Open Command Line Here". Walks up the selection's parent chain to
	/// the enclosing <see cref="AssemblyTreeNode"/> and asks the OS to open a terminal
	/// session whose working directory is the assembly's containing folder. Useful for
	/// running `ilspycmd`, `dotnet`, or other CLI tools against the same file the user
	/// is inspecting in the UI.
	/// </summary>
	[ExportContextMenuEntry(Header = nameof(Resources._OpenCommandLineHere), Category = nameof(Resources.Shell), Icon = "Images/FolderOpen", Order = 510)]
	[Shared]
	public sealed class OpenCommandLineHereContextMenuEntry : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context) => GetDirectoriesToOpen(context).Count > 0;

		public bool IsEnabled(TextViewContext context) => GetDirectoriesToOpen(context).Count > 0;

		public void Execute(TextViewContext context)
		{
			foreach (var dir in GetDirectoriesToOpen(context))
				OpenTerminalAt(dir);
		}

		static IReadOnlyList<string> GetDirectoriesToOpen(TextViewContext context)
		{
			if (context.SelectedTreeNodes is not { Length: > 0 } nodes)
				return Array.Empty<string>();
			var dirs = new List<string>(nodes.Length);
			foreach (var node in nodes)
			{
				var assembly = AssemblyTreeNode.FindEnclosing(node);
				if (assembly is null)
					return Array.Empty<string>();
				var dir = Path.GetDirectoryName(assembly.LoadedAssembly.FileName);
				if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
					return Array.Empty<string>();
				dirs.Add(dir);
			}
			return dirs;
		}


		static void OpenTerminalAt(string directory)
		{
			try
			{
				if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
				{
					// cmd.exe /k pins the window open at the chosen directory.
					Process.Start(new ProcessStartInfo("cmd.exe", $"/k \"cd /d {directory}\"") {
						UseShellExecute = true,
					});
				}
				else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				{
					// `open -a Terminal <path>` launches Terminal.app cd'd to the path.
					Process.Start(new ProcessStartInfo("open", $"-a Terminal \"{directory}\"") {
						UseShellExecute = false,
					});
				}
				else
				{
					// Linux: try $TERMINAL first, then a handful of common emulators.
					// Each candidate is tried in turn; the first one that doesn't throw wins.
					var candidates = new List<string>();
					var env = Environment.GetEnvironmentVariable("TERMINAL");
					if (!string.IsNullOrEmpty(env))
						candidates.Add(env);
					candidates.AddRange(new[] {
						"x-terminal-emulator", "gnome-terminal", "konsole",
						"xfce4-terminal", "alacritty", "kitty", "xterm",
					});
					foreach (var term in candidates)
					{
						try
						{
							Process.Start(new ProcessStartInfo(term) {
								WorkingDirectory = directory,
								UseShellExecute = false,
							});
							return;
						}
						catch
						{
							// try the next candidate
						}
					}
				}
			}
			catch
			{
				// Failure to launch a terminal is non-fatal — the menu item already returned.
			}
		}
	}
}
