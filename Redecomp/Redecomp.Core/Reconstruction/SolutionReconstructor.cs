// Copyright (c) 2026 Franck
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

using System.Xml.Linq;

using Redecomp.Core.Recovery;

namespace Redecomp.Core.Reconstruction;

/// <summary>A project of a reconstructed solution.</summary>
/// <param name="Name">Project name, as recorded in the dependency manifest.</param>
/// <param name="AssemblyFileName">File name of the project's assembly in the publish directory.</param>
/// <param name="Result">Outcome of the project reconstruction.</param>
public sealed record SolutionProject(string Name, string AssemblyFileName, ReconstructionResult Result);

/// <summary>Outcome of a solution reconstruction.</summary>
/// <param name="SolutionFile">Path of the generated <c>.slnx</c> file.</param>
/// <param name="Projects">The reconstructed projects, the application's own project first.</param>
public sealed record SolutionResult(string SolutionFile, IReadOnlyList<SolutionProject> Projects);

/// <summary>
/// Rebuilds a whole solution from a published application directory: one project per project
/// assembly listed in the <c>.deps.json</c>, linked by project references, with NuGet package
/// references in place of the package assemblies.
/// </summary>
public static class SolutionReconstructor
{
	public static SolutionResult Reconstruct(string publishDirectory, string outputDirectory, ReconstructionOptions? options = null)
	{
		options ??= ReconstructionOptions.Default;
		publishDirectory = Path.GetFullPath(publishDirectory);
		string depsPath = FindDepsFile(publishDirectory);
		var deps = DepsFile.Load(depsPath);

		var projects = deps.Libraries
			.Where(l => l.Type == DepsLibraryType.Project && l.RuntimeAssemblies.Any(a => File.Exists(Path.Combine(publishDirectory, a))))
			.ToList();
		if (projects.Count == 0)
			throw new InvalidOperationException($"{Path.GetFileName(depsPath)} lists no project assembly present in {publishDirectory}.");

		string ProjectPath(DepsLibrary library) => Path.Combine(library.Name, library.Name + ".csproj");

		var results = new List<SolutionProject>();
		foreach (var project in projects)
		{
			string assembly = Path.Combine(publishDirectory, project.RuntimeAssemblies.First(a => File.Exists(Path.Combine(publishDirectory, a))));
			string projectDirectory = Path.Combine(outputDirectory, project.Name);
			var context = new DependencyContext(deps, project, other => Path.Combine("..", ProjectPath(other)));
			// Files deployed next to the application belong to the application's project only.
			var projectOptions = options with { IncludeContentFiles = options.IncludeContentFiles && project == deps.Root };
			var result = ProjectReconstructor.Reconstruct(assembly, projectDirectory, projectOptions, context);
			string expected = Path.Combine(outputDirectory, ProjectPath(project));
			if (result.ProjectFile != expected)
			{
				File.Move(result.ProjectFile, expected);
				result = result with { ProjectFile = expected };
			}
			results.Add(new SolutionProject(project.Name, Path.GetFileName(assembly), result));
		}

		string solutionFile = Path.Combine(outputDirectory, deps.Root.Name + ".slnx");
		new XDocument(new XElement("Solution",
			projects.Select(p => new XElement("Project", new XAttribute("Path", ProjectPath(p).Replace('\\', '/'))))))
			.Save(solutionFile);
		return new SolutionResult(solutionFile, results);
	}

	/// <summary>True when the directory holds a published application whose manifest lists several projects.</summary>
	public static bool IsMultiProjectApplication(string directory)
	{
		var depsFiles = Directory.GetFiles(directory, "*.deps.json");
		return depsFiles.Length == 1 && DepsFile.Load(depsFiles[0]).Libraries.Count(l => l.Type == DepsLibraryType.Project) > 1;
	}

	static string FindDepsFile(string publishDirectory)
	{
		var depsFiles = Directory.GetFiles(publishDirectory, "*.deps.json");
		return depsFiles.Length switch {
			1 => depsFiles[0],
			0 => throw new FileNotFoundException($"No .deps.json in {publishDirectory}: not a published .NET application."),
			_ => throw new InvalidOperationException($"Several .deps.json files in {publishDirectory}; reconstruct each application separately."),
		};
	}
}
