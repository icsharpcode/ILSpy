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
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy.Commands
{
	/// <summary>
	/// The UI-agnostic engine behind the Export Project/Solution dialog. Unifies single-assembly
	/// project export and multi-assembly solution export (the latter via <see cref="SolutionWriter"/>),
	/// applies the dialog's overrides onto a settings clone, and optionally emits a portable PDB per
	/// assembly. Returns a <see cref="SolutionExportResult"/> the caller surfaces in the text view.
	/// Kept separate from the dialog so it is headless-testable.
	/// </summary>
	internal static class ProjectExporter
	{
		public static async Task<SolutionExportResult> ExportAsync(
			IReadOnlyList<LoadedAssembly> assemblies, bool solutionMode, ProjectExportOptions options,
			DecompilerSettings settingsClone, Language language, IProgress<DecompilationProgress>? progress,
			CancellationToken ct)
		{
			ArgumentNullException.ThrowIfNull(assemblies);
			ArgumentNullException.ThrowIfNull(options);
			ArgumentNullException.ThrowIfNull(settingsClone);
			ArgumentNullException.ThrowIfNull(language);

			ApplyOverrides(settingsClone, options);

			if (solutionMode)
			{
				var solutionFilePath = Path.Combine(options.OutputDirectory, SolutionFileName(options.OutputDirectory));
				var solution = await SolutionWriter.CreateSolutionAsync(
					solutionFilePath, language, assemblies, ct, settingsClone, options.StrongNameKeyFile, progress)
					.ConfigureAwait(false);

				var report = new StringBuilder(solution.StatusText);
				if (options.GeneratePdb && solution.Success)
				{
					await Task.Run(() => GeneratePdbs(assemblies,
						a => Path.Combine(options.OutputDirectory, a.ShortName), settingsClone, options, report, ct), ct)
						.ConfigureAwait(false);
				}
				return new SolutionExportResult(solution.Success, report.ToString());
			}

			return await Task.Run(() => ExportProject(assemblies[0], options, settingsClone, language, progress, ct), ct)
				.ConfigureAwait(false);
		}

		static SolutionExportResult ExportProject(LoadedAssembly assembly, ProjectExportOptions options,
			DecompilerSettings settingsClone, Language language, IProgress<DecompilationProgress>? progress, CancellationToken ct)
		{
			var report = new StringBuilder();
			bool success;
			try
			{
				var decompileOptions = new DecompilationOptions(settingsClone) {
					FullDecompilation = true,
					EscapeInvalidIdentifiers = true,
					CancellationToken = ct,
					SaveAsProjectDirectory = options.OutputDirectory,
					StrongNameKeyFile = options.StrongNameKeyFile,
					ProgressIndicator = progress,
				};
				language.DecompileAssembly(assembly, new PlainTextOutput(new StringWriter()), decompileOptions);
				report.AppendLine("Project written to " + options.OutputDirectory);
				success = true;
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception e)
			{
				report.AppendLine($"Failed to decompile the assembly '{assembly.FileName}':{Environment.NewLine}{e.Message}");
				success = false;
			}

			if (options.GeneratePdb && success)
				GeneratePdbs(new[] { assembly }, _ => options.OutputDirectory, settingsClone, options, report, ct);

			return new SolutionExportResult(success, report.ToString());
		}

		static void GeneratePdbs(IReadOnlyList<LoadedAssembly> assemblies,
			Func<LoadedAssembly, string> projectDirectory, DecompilerSettings settingsClone,
			ProjectExportOptions options, StringBuilder report, CancellationToken ct)
		{
			report.AppendLine();
			foreach (var assembly in assemblies)
			{
				ct.ThrowIfCancellationRequested();
				if (assembly.GetMetadataFileOrNull() is not PEFile file
					|| !PortablePdbWriter.HasCodeViewDebugDirectoryEntry(file))
				{
					report.AppendLine($"Skipped PDB for '{assembly.ShortName}': no CodeView debug directory entry.");
					continue;
				}

				var pdbFileName = Path.Combine(projectDirectory(assembly),
					WholeProjectDecompiler.CleanUpFileName(assembly.ShortName, ".pdb"));
				try
				{
					using var stream = new FileStream(pdbFileName, FileMode.Create, FileAccess.Write);
					var resolver = assembly.GetAssemblyResolver();
					var decompiler = new CSharpDecompiler(file, resolver, settingsClone) {
						CancellationToken = ct,
					};
					new PortablePdbWriter { EmbedSourceFiles = options.EmbedSourceFilesInPdb }
						.WritePdb(file, decompiler, settingsClone, stream);
					report.AppendLine("Generated PDB: " + pdbFileName);
				}
				catch (OperationCanceledException)
				{
					throw;
				}
				catch (Exception e)
				{
					report.AppendLine($"Failed to generate a PDB for '{assembly.ShortName}': {e.Message}");
				}
			}
		}

		static void ApplyOverrides(DecompilerSettings settings, ProjectExportOptions options)
		{
			settings.UseSdkStyleProjectFormat = options.UseSdkStyleProjectFormat;
			settings.UseNestedDirectoriesForNamespaces = options.UseNestedDirectoriesForNamespaces;
			settings.RemoveDeadCode = options.RemoveDeadCode;
			settings.RemoveDeadStores = options.RemoveDeadStores;
			settings.UseDebugSymbols = options.UseDebugSymbols;
		}

		// The .sln is named after the chosen output folder, falling back to "Solution.sln".
		static string SolutionFileName(string outputDirectory)
		{
			var name = Path.GetFileName(outputDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
			return (string.IsNullOrEmpty(name) ? "Solution" : name) + ".sln";
		}
	}
}
