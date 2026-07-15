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
using System.Linq;
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
	/// applies the dialog's overrides onto a settings clone, skips (and reports) the entries with no code
	/// behind them, and optionally emits a portable PDB per assembly. Returns a
	/// <see cref="SolutionExportResult"/> the caller surfaces in the text view. Kept separate from the
	/// dialog so it is headless-testable.
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

			// Resolve each entry by awaiting its load. IsLoadedAsValidAssembly cannot stand in for this:
			// it is a non-blocking status poll that reads false for a load still running or not yet
			// started (LoadedAssembly builds its entries lazily), so filtering on it here would drop
			// assemblies that decompile perfectly well once awaited. The callers' selection predicate has
			// to poll -- it answers IsEnabled on the UI thread -- but this runs off it and can wait.
			var loaded = await Task.WhenAll(assemblies.Select(async a => (
				Assembly: a,
				File: await a.GetMetadataFileOrNullAsync().ConfigureAwait(false)
			))).ConfigureAwait(false);

			// Anything without a PE image behind it has nothing to decompile: a file that failed to load,
			// or one that carries metadata only (a standalone PDB, say). Leave those out and name them in
			// the report, so a mixed selection still exports what it can and the user is told what is
			// missing rather than having to notice it.
			var exportable = loaded.Where(e => e.File is { IsMetadataOnly: false }).Select(e => e.Assembly).ToList();
			var skipReport = SkipReport(loaded.Where(e => e.File is not { IsMetadataOnly: false }));
			if (exportable.Count == 0)
			{
				return new SolutionExportResult(false,
					"Nothing to export." + Environment.NewLine + skipReport);
			}

			if (solutionMode)
			{
				var solutionFilePath = Path.Combine(options.OutputDirectory,
					options.SolutionFileName ?? SolutionFileNameFor(options.OutputDirectory));
				var solution = await SolutionWriter.CreateSolutionAsync(
					solutionFilePath, language, exportable, ct, settingsClone, options.StrongNameKeyFile, progress)
					.ConfigureAwait(false);

				var report = new StringBuilder(solution.StatusText);
				if (options.GeneratePdb && solution.Success)
				{
					await Task.Run(() => GeneratePdbs(exportable,
						a => Path.Combine(options.OutputDirectory, a.ShortName), settingsClone, options, report, ct), ct)
						.ConfigureAwait(false);
				}
				AppendSkipReport(report, skipReport);
				return new SolutionExportResult(solution.Success, report.ToString());
			}

			var projectResult = await Task
				.Run(() => ExportProject(exportable[0], options, settingsClone, language, progress, ct), ct)
				.ConfigureAwait(false);
			if (skipReport.Length == 0)
				return projectResult;

			var projectReport = new StringBuilder(projectResult.StatusText);
			AppendSkipReport(projectReport, skipReport);
			return projectResult with { StatusText = projectReport.ToString() };
		}

		// One line per assembly left out of the export, saying which of the two reasons applies: a file
		// that never loaded is a different problem from one that loaded and holds no code, and sending
		// the user after a corrupt file that is not there wastes their time.
		static string SkipReport(IEnumerable<(LoadedAssembly Assembly, MetadataFile? File)> skipped)
		{
			var report = new StringBuilder();
			foreach (var (assembly, file) in skipped)
			{
				var reason = file == null
					? "the assembly failed to load"
					: "it holds metadata only, with no code to decompile";
				report.AppendLine($"Skipped '{assembly.ShortName}': {reason}.");
			}
			return report.ToString();
		}

		static void AppendSkipReport(StringBuilder report, string skipReport)
		{
			if (skipReport.Length == 0)
				return;
			report.AppendLine();
			report.Append(skipReport);
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

		/// <summary>
		/// The <c>.sln</c> name used when <see cref="ProjectExportOptions.SolutionFileName"/> is unset:
		/// after the chosen output folder, falling back to "Solution.sln". Internal so the export dialog
		/// can show the same name as its watermark instead of predicting it.
		/// </summary>
		internal static string SolutionFileNameFor(string outputDirectory)
		{
			var name = Path.GetFileName(outputDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
			return (string.IsNullOrEmpty(name) ? "Solution" : name) + ".sln";
		}
	}
}
