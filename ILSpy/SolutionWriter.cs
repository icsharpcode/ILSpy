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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Solution;
using ICSharpCode.ILSpyX;

using ICSharpCode.ILSpy.Languages;

namespace ICSharpCode.ILSpy
{
	/// <summary>
	/// The outcome of a <see cref="SolutionWriter.CreateSolutionAsync"/> run: whether a complete
	/// solution was produced and the human-readable status report (the same breadcrumb the WPF
	/// version printed into the decompiler text view).
	/// </summary>
	public sealed record SolutionExportResult(bool Success, string StatusText);

	/// <summary>
	/// Creates a Visual Studio solution containing one decompiled project per assembly. The
	/// solution directory must be empty or non-existent. UI-agnostic: callers supply an explicit
	/// target path (the "Save Code" entry picks it via a file dialog) and surface
	/// <see cref="SolutionExportResult.StatusText"/> however they like.
	/// </summary>
	internal sealed class SolutionWriter
	{
		/// <summary>
		/// Decompiles every assembly in <paramref name="assemblies"/> into its own project under the
		/// directory of <paramref name="solutionFilePath"/> and writes the solution file itself.
		/// </summary>
		/// <exception cref="ArgumentException">Thrown when <paramref name="solutionFilePath"/> is null
		/// or whitespace.</exception>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="language"/> or
		/// <paramref name="assemblies"/> is null.</exception>
		/// <param name="settings">Decompiler settings each project is decompiled with.</param>
		/// <param name="strongNameKeyFile">Optional <c>.snk</c> copied into every project and emitted
		/// as <c>&lt;AssemblyOriginatorKeyFile&gt;</c>.</param>
		public static Task<SolutionExportResult> CreateSolutionAsync(string solutionFilePath,
			Language language, IReadOnlyList<LoadedAssembly> assemblies,
			CancellationToken cancellationToken,
			DecompilerSettings settings, string? strongNameKeyFile = null,
			IProgress<DecompilationProgress>? progress = null)
		{
			if (string.IsNullOrWhiteSpace(solutionFilePath))
				throw new ArgumentException("The solution file path cannot be null or empty.", nameof(solutionFilePath));
			ArgumentNullException.ThrowIfNull(language);
			ArgumentNullException.ThrowIfNull(assemblies);
			ArgumentNullException.ThrowIfNull(settings);

			return new SolutionWriter(solutionFilePath, settings, strongNameKeyFile, progress)
				.CreateSolutionAsync(assemblies, language, cancellationToken);
		}

		readonly string solutionFilePath;
		readonly string solutionDirectory;
		readonly DecompilerSettings settings;
		readonly string? strongNameKeyFile;
		readonly IProgress<DecompilationProgress>? progress;
		readonly ConcurrentBag<ProjectItem> projects;
		readonly ConcurrentBag<string> statusOutput;
		int completedAssemblies;

		SolutionWriter(string solutionFilePath, DecompilerSettings settings, string? strongNameKeyFile,
			IProgress<DecompilationProgress>? progress)
		{
			this.solutionFilePath = solutionFilePath;
			this.settings = settings;
			this.strongNameKeyFile = strongNameKeyFile;
			this.progress = progress;
			solutionDirectory = Path.GetDirectoryName(solutionFilePath)!;
			statusOutput = new ConcurrentBag<string>();
			projects = new ConcurrentBag<ProjectItem>();
		}

		async Task<SolutionExportResult> CreateSolutionAsync(IReadOnlyList<LoadedAssembly> allAssemblies,
			Language language, CancellationToken ct)
		{
			var report = new StringBuilder();

			// Two assemblies that share a short name would decompile into the same project directory;
			// refuse rather than have one clobber the other.
			var assembliesByShortName = allAssemblies.GroupBy(a => a.ShortName).ToDictionary(g => g.Key, g => g.ToList());
			bool first = true;
			bool abort = false;
			foreach (var (_, assemblies) in assembliesByShortName)
			{
				if (assemblies.Count == 1)
					continue;
				if (first)
				{
					report.AppendLine("Duplicate assembly names selected, cannot generate a solution:");
					abort = true;
					first = false;
				}
				report.AppendLine("- " + assemblies[0].Text + " conflicts with "
					+ string.Join(", ", assemblies.Skip(1).Select(a => a.Text)));
			}

			if (abort)
				return new SolutionExportResult(false, report.ToString());

			try
			{
				// An explicit enumerable partitioner avoids Parallel.ForEach's list special-casing,
				// whose static partitioning is inefficient when assemblies decompile at different speeds.
				await Task.Run(() => System.Threading.Tasks.Parallel.ForEach(Partitioner.Create(allAssemblies),
					new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount, CancellationToken = ct },
					item => WriteProject(item, language, solutionDirectory, allAssemblies.Count, ct)))
					.ConfigureAwait(false);

				if (projects.Count == 0)
				{
					report.AppendLine();
					report.AppendLine("Solution could not be created, because none of the selected assemblies could be decompiled into a project.");
					return new SolutionExportResult(false, report.ToString());
				}

				await Task.Run(() => SolutionCreator.WriteSolutionFile(solutionFilePath, projects.ToList()), ct)
					.ConfigureAwait(false);
			}
			catch (AggregateException ae)
			{
				if (ae.Flatten().InnerExceptions.All(e => e is OperationCanceledException))
				{
					report.AppendLine();
					report.AppendLine("Generation was cancelled.");
					return new SolutionExportResult(false, report.ToString());
				}

				report.AppendLine();
				report.AppendLine("Failed to generate the Visual Studio Solution. Errors:");
				ae.Handle(e => {
					report.AppendLine(e.Message);
					return true;
				});
				return new SolutionExportResult(false, report.ToString());
			}
			catch (OperationCanceledException)
			{
				report.AppendLine();
				report.AppendLine("Generation was cancelled.");
				return new SolutionExportResult(false, report.ToString());
			}

			foreach (var item in statusOutput)
				report.AppendLine(item);

			// statusOutput only collects per-assembly failures; an empty bag means every project built.
			if (statusOutput.Count != 0)
				return new SolutionExportResult(false, report.ToString());

			report.AppendLine("Successfully decompiled the following assemblies into Visual Studio projects:");
			foreach (var n in allAssemblies)
				report.AppendLine(n.Text.ToString());
			report.AppendLine();
			if (allAssemblies.Count == projects.Count)
				report.AppendLine("Created the Visual Studio Solution file.");

			return new SolutionExportResult(true, report.ToString());
		}

		void WriteProject(LoadedAssembly loadedAssembly, Language language, string targetDirectory, int totalAssemblies, CancellationToken ct)
		{
			// Solution export decompiles assemblies in parallel, so per-file progress would race; report
			// at the coarser assembly granularity instead -- a determinate bar over the assembly count.
			void ReportDone() => progress?.Report(new DecompilationProgress {
				TotalUnits = totalAssemblies,
				UnitsCompleted = System.Threading.Interlocked.Increment(ref completedAssemblies),
				Status = loadedAssembly.ShortName,
			});
			targetDirectory = Path.Combine(targetDirectory, loadedAssembly.ShortName);

			if (language.ProjectFileExtension == null)
			{
				statusOutput.Add("-------------");
				statusOutput.Add($"Language '{language.Name}' does not support exporting assemblies as projects!");
				return;
			}

			if (File.Exists(targetDirectory))
			{
				statusOutput.Add("-------------");
				statusOutput.Add($"Failed to create a directory '{targetDirectory}':{Environment.NewLine}A file with the same name already exists!");
				return;
			}

			if (!Directory.Exists(targetDirectory))
			{
				try
				{
					Directory.CreateDirectory(targetDirectory);
				}
				catch (Exception e)
				{
					statusOutput.Add("-------------");
					statusOutput.Add($"Failed to create a directory '{targetDirectory}':{Environment.NewLine}{e}");
					return;
				}
			}

			try
			{
				var options = new DecompilationOptions(settings);
				options.FullDecompilation = true;
				options.EscapeInvalidIdentifiers = true;
				options.CancellationToken = ct;
				options.SaveAsProjectDirectory = targetDirectory;
				options.StrongNameKeyFile = strongNameKeyFile;

				// The project-export path writes the .csproj into SaveAsProjectDirectory itself; the
				// ITextOutput only receives a "Project written to ..." breadcrumb, which we discard here.
				var projectInfo = language.DecompileAssembly(loadedAssembly, new PlainTextOutput(new StringWriter()), options);
				if (projectInfo != null)
				{
					// SolutionCreator.FixAllProjectReferences parses each project file off disk, so the
					// ProjectItem must point at the .csproj the decompiler actually produced (its name is
					// derived from the module name, not necessarily the assembly short name).
					var projectFileName = Directory.EnumerateFiles(targetDirectory, "*" + language.ProjectFileExtension).FirstOrDefault()
						?? Path.Combine(targetDirectory, loadedAssembly.ShortName + language.ProjectFileExtension);
					projects.Add(new ProjectItem(projectFileName, projectInfo.PlatformName, projectInfo.Guid, projectInfo.TypeGuid));
				}
			}
			catch (NotSupportedException e)
			{
				statusOutput.Add("-------------");
				statusOutput.Add($"Failed to decompile the assembly '{loadedAssembly.FileName}':{Environment.NewLine}{e.Message}");
			}
			catch (PathTooLongException e)
			{
				statusOutput.Add("-------------");
				statusOutput.Add(string.Format(ICSharpCode.ILSpy.Properties.Resources.ProjectExportPathTooLong, loadedAssembly.FileName)
					+ Environment.NewLine + Environment.NewLine + e.ToString());
			}
			catch (Exception e) when (e is not OperationCanceledException)
			{
				statusOutput.Add("-------------");
				statusOutput.Add($"Failed to decompile the assembly '{loadedAssembly.FileName}':{Environment.NewLine}{e}");
			}
			ReportDone();
		}
	}
}
