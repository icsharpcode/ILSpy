// Copyright (c) 2011 AlphaSierraPapa for the SharpDevelop Team
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ICSharpCode.Decompiler;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.TreeNodes;
using Microsoft.Win32;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// An utility class that creates a Visual Studio solution containing projects for the
	/// decompiled assemblies.
	/// </summary>
	internal static class SolutionWriter
	{
		private const string SolutionExtension = ".sln";
		private const string DefaultSolutionName = "Solution";

		/// <summary>
		/// Shows a File Selection dialog where the user can select the target file for the solution.
		/// </summary>
		/// <param name="path">The initial path to show in the dialog. If not specified, the 'Documents' directory
		/// will be used.</param>
		/// 
		/// <returns>The full path of the selected target file, or <c>null</c> if the user canceled.</returns>
		public static string SelectSolutionFile(string path)
		{
			if (string.IsNullOrWhiteSpace(path)) {
				path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			}

			SaveFileDialog dlg = new SaveFileDialog();
			dlg.InitialDirectory = path;
			dlg.FileName = Path.Combine(path, DefaultSolutionName + SolutionExtension);
			dlg.Filter = "Visual Studio Solution file|*" + SolutionExtension;

			bool targetInvalid;
			do {
				if (dlg.ShowDialog() != true) {
					return null;
				}

				string selectedPath = Path.GetDirectoryName(dlg.FileName);
				try {
					targetInvalid = Directory.EnumerateFileSystemEntries(selectedPath).Any();
				} catch (Exception e) when (e is IOException || e is UnauthorizedAccessException || e is SecurityException) {
					MessageBox.Show(
						"The directory cannot be accessed. Please ensure it exists and you have sufficient rights to access it.",
						"Solution directory not accessible",
						MessageBoxButton.OK, MessageBoxImage.Error);
					targetInvalid = true;
					continue;
				}
				
				if (targetInvalid) {
					MessageBox.Show(
						"The directory is not empty. Please select an empty directory.",
						"Solution directory not empty",
						MessageBoxButton.OK, MessageBoxImage.Warning);
				}
			} while (targetInvalid);

			return dlg.FileName;
		}

		/// <summary>
		/// Creates a Visual Studio solution that contains projects with decompiled code
		/// of the specified <paramref name="assemblyNodes"/>. The solution file will be saved
		/// to the <paramref name="solutionFilePath"/>. The directory of this file must either
		/// be empty or not exist.
		/// </summary>
		/// <param name="textView">A reference to the <see cref="DecompilerTextView"/> instance.</param>
		/// <param name="solutionFilePath">The target file path of the solution file.</param>
		/// <param name="assemblyNodes">The assembly nodes to decompile.</param>
		/// 
		/// <exception cref="ArgumentException">Thrown when <paramref name="solutionFilePath"/> is null,
		/// an empty or a whitespace string.</exception>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="textView"/>> or
		/// <paramref name="assemblyNodes"/> is null.</exception>
		public static void CreateSolution(DecompilerTextView textView, string solutionFilePath, IEnumerable<AssemblyTreeNode> assemblyNodes)
		{
			if (textView == null) {
				throw new ArgumentNullException(nameof(textView));
			}

			if (string.IsNullOrWhiteSpace(solutionFilePath)) {
				throw new ArgumentException("The solution file path cannot be null or empty.", nameof(solutionFilePath));
			}

			if (assemblyNodes == null) {
				throw new ArgumentNullException(nameof(assemblyNodes));
			}

			textView
				.RunWithCancellation(ct => CreateSolution(solutionFilePath, assemblyNodes, ct))
				.Then(output => textView.ShowText(output))
				.HandleExceptions();
		}

		private static async Task<AvalonEditTextOutput> CreateSolution(
			string solutionFilePath,
			IEnumerable<AssemblyTreeNode> assemblyNodes,
			CancellationToken ct)
		{
			var solutionDirectory = Path.GetDirectoryName(solutionFilePath);
			var statusOutput = new ConcurrentBag<string>();
			var result = new AvalonEditTextOutput();

			var duplicates = new HashSet<string>();
			if (assemblyNodes.Any(n => !duplicates.Add(n.LoadedAssembly.ShortName))) {
				result.WriteLine("Duplicate assembly names selected, cannot generate a solution.");
				return result;
			}

			Stopwatch stopwatch = Stopwatch.StartNew();

			await Task.Run(() => Parallel.ForEach(assemblyNodes, n => WriteProject(n, solutionDirectory, statusOutput, ct)))
				.ConfigureAwait(false);


			foreach (var item in statusOutput) {
				result.WriteLine(item);
			}

			if (statusOutput.Count == 0) {
				result.WriteLine("Successfully decompiled the following assemblies to a Visual Studio Solution:");
				foreach (var item in assemblyNodes.Select(n => n.Text.ToString())) {
					result.WriteLine(item);
				}

				result.WriteLine();
				result.WriteLine("Elapsed time: " + stopwatch.Elapsed.TotalSeconds.ToString("F1") + " seconds.");
				result.WriteLine();
				result.AddButton(null, "Open Explorer", delegate { Process.Start("explorer", "/select,\"" + solutionFilePath + "\""); });
			}

			return result;
		}

		private static void WriteProject(AssemblyTreeNode assemblyNode, string targetDirectory, ConcurrentBag<string> statusOutput, CancellationToken ct)
		{
			var loadedAssembly = assemblyNode.LoadedAssembly;

			targetDirectory = Path.Combine(targetDirectory, loadedAssembly.ShortName);
			string projectFileName = Path.Combine(targetDirectory, loadedAssembly.ShortName + assemblyNode.Language.ProjectFileExtension);

			if (!Directory.Exists(targetDirectory)) {
				try {
					Directory.CreateDirectory(targetDirectory);
				} catch (Exception e) {
					statusOutput.Add($"Failed to create a directory '{targetDirectory}':{Environment.NewLine}{e}");
					return;
				}
			}

			try {
				using (var projectFileWriter = new StreamWriter(projectFileName)) {
				var projectFileOutput = new PlainTextOutput(projectFileWriter);
				var options = new DecompilationOptions() {
					FullDecompilation = true,
					CancellationToken = ct,
					SaveAsProjectDirectory = targetDirectory };
				
					assemblyNode.Decompile(assemblyNode.Language, projectFileOutput, options);
				}
			} catch (Exception e) {
				statusOutput.Add($"Failed to decompile the assembly '{loadedAssembly.FileName}':{Environment.NewLine}{e}");
				return;
			}
		}
	}
}
