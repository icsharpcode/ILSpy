// Copyright (c) 2016 Daniel Grunwald
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
using System.Xml.Linq;

namespace ICSharpCode.Decompiler.CSharp
{
	/// <summary>
	/// A container class that holds information about a Visual Studio project.
	/// </summary>
	public sealed class ProjectItem : ProjectId
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectItem"/> class.
		/// </summary>
		/// <param name="projectFile">The full path of the project file.</param>
		/// <param name="projectPlatform">The project platform.</param>
		/// <param name="projectGuid">The project GUID.</param>
		/// 
		/// <exception cref="ArgumentException">Thrown when <paramref name="projectFile"/> 
		/// or <paramref name="projectPlatform"/> is null or empty.</exception>
		public ProjectItem(string projectFile, string projectPlatform, Guid projectGuid)
			: base(projectPlatform, projectGuid)
		{
			ProjectName = Path.GetFileNameWithoutExtension(projectFile);
			FilePath = projectFile;
		}

		/// <summary>
		/// Gets the name of the project.
		/// </summary>
		public string ProjectName { get; }

		/// <summary>
		/// Gets the full path to the project file.
		/// </summary>
		public string FilePath { get; }
	}

	/// <summary>
	/// A container class that holds platform and GUID information about a Visual Studio project.
	/// </summary>
	public class ProjectId
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProjectId"/> class.
		/// </summary>
		/// <param name="projectPlatform">The project platform.</param>
		/// <param name="projectGuid">The project GUID.</param>
		/// 
		/// <exception cref="ArgumentException">Thrown when <paramref name="projectFile"/> 
		/// or <paramref name="projectPlatform"/> is null or empty.</exception>
		public ProjectId(string projectPlatform, Guid projectGuid)
		{
			if (string.IsNullOrWhiteSpace(projectPlatform)) {
				throw new ArgumentException("The platform cannot be null or empty.", nameof(projectPlatform));
			}

			Guid = projectGuid;
			PlatformName = projectPlatform;
		}

		/// <summary>
		/// Gets the GUID of this project.
		/// </summary>
		public Guid Guid { get; }

		/// <summary>
		/// Gets the platform name of this project. Only single platform per project is supported.
		/// </summary>
		public string PlatformName { get; }
	}

	/// <summary>
	/// A helper class that can write a Visual Studio Solution file for the provided projects.
	/// </summary>
	public static class SolutionCreator
	{
		private static readonly XNamespace ProjectFileNamespace = XNamespace.Get("http://schemas.microsoft.com/developer/msbuild/2003");

		/// <summary>
		/// Writes a solution file to the specified <paramref name="targetFile"/>.
		/// </summary>
		/// <param name="targetFile">The full path of the file to write.</param>
		/// <param name="projects">The projects contained in this solution.</param>
		/// 
		/// <exception cref="ArgumentException">Thrown when <paramref name="targetFile"/> is null or empty.</exception>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="projects"/> is null.</exception>
		/// <exception cref="InvalidOperationException">Thrown when <paramref name="projects"/> contains no items.</exception>
		public static void WriteSolutionFile(string targetFile, IEnumerable<ProjectItem> projects)
		{
			if (string.IsNullOrWhiteSpace(targetFile)) {
				throw new ArgumentException("The target file cannot be null or empty.", nameof(targetFile));
			}

			if (projects == null) {
				throw new ArgumentNullException(nameof(projects));
			}

			if (!projects.Any()) {
				throw new InvalidOperationException("At least one project is expected.");
			}

			using (var writer = new StreamWriter(targetFile)) {
				WriteSolutionFile(writer, projects, Path.GetDirectoryName(targetFile));
			}

			FixProjectReferences(projects);
		}

		private static void WriteSolutionFile(TextWriter writer, IEnumerable<ProjectItem> projects, string solutionPath)
		{
			WriteHeader(writer);
			WriteProjects(writer, projects, solutionPath);

			writer.WriteLine("Global");

			var platforms = WriteSolutionConfigurations(writer, projects);
			WriteProjectConfigurations(writer, projects, platforms);

			writer.WriteLine("\tGlobalSection(SolutionProperties) = preSolution");
			writer.WriteLine("\t\tHideSolutionNode = FALSE");
			writer.WriteLine("\tEndGlobalSection");

			writer.WriteLine("EndGlobal");
		}

		private static void WriteHeader(TextWriter writer)
		{
			writer.WriteLine("Microsoft Visual Studio Solution File, Format Version 12.00");
			writer.WriteLine("# Visual Studio 14");
			writer.WriteLine("VisualStudioVersion = 14.0.24720.0");
			writer.WriteLine("MinimumVisualStudioVersion = 10.0.40219.1");
		}

		private static void WriteProjects(TextWriter writer, IEnumerable<ProjectItem> projects, string solutionPath)
		{
			var solutionGuid = Guid.NewGuid().ToString("B").ToUpperInvariant();

			foreach (var project in projects) {
				var projectRelativePath = GetRelativePath(solutionPath, project.FilePath);
				var projectGuid = project.Guid.ToString("B").ToUpperInvariant();

				writer.WriteLine($"Project(\"{solutionGuid}\") = \"{project.ProjectName}\", \"{projectRelativePath}\", \"{projectGuid}\"");
				writer.WriteLine("EndProject");
			}
		}

		private static IEnumerable<string> WriteSolutionConfigurations(TextWriter writer, IEnumerable<ProjectItem> projects)
		{
			var platforms = projects.GroupBy(p => p.PlatformName).Select(g => g.Key).ToList();

			platforms.Sort();

			writer.WriteLine("\tGlobalSection(SolutionConfigurationPlatforms) = preSolution");
			foreach (var platform in platforms) {
				writer.WriteLine($"\t\tDebug|{platform} = Debug|{platform}");
			}

			foreach (var platform in platforms) {
				writer.WriteLine($"\t\tRelease|{platform} = Release|{platform}");
			}

			writer.WriteLine("\tEndGlobalSection");

			return platforms;
		}

		private static void WriteProjectConfigurations(
			TextWriter writer,
			IEnumerable<ProjectItem> projects,
			IEnumerable<string> solutionPlatforms)
		{
			writer.WriteLine("\tGlobalSection(ProjectConfigurationPlatforms) = postSolution");

			foreach (var project in projects) {
				var projectGuid = project.Guid.ToString("B").ToUpperInvariant();

				foreach (var platform in solutionPlatforms) {
					writer.WriteLine($"\t\t{projectGuid}.Debug|{platform}.ActiveCfg = Debug|{project.PlatformName}");
					writer.WriteLine($"\t\t{projectGuid}.Debug|{platform}.Build.0 = Debug|{project.PlatformName}");
				}

				foreach (var platform in solutionPlatforms) {
					writer.WriteLine($"\t\t{projectGuid}.Release|{platform}.ActiveCfg = Release|{project.PlatformName}");
					writer.WriteLine($"\t\t{projectGuid}.Release|{platform}.Build.0 = Release|{project.PlatformName}");
				}
			}

			writer.WriteLine("\tEndGlobalSection");
		}

		private static void FixProjectReferences(IEnumerable<ProjectItem> projects)
		{
			var projectsMap = projects.ToDictionary(p => p.ProjectName, p => p);

			foreach (var project in projects) {
				var projectDirectory = Path.GetDirectoryName(project.FilePath);
				XDocument projectDoc = XDocument.Load(project.FilePath);

				var referencesItemGroups = projectDoc.Root
					.Elements(ProjectFileNamespace + "ItemGroup")
					.Where(e => e.Elements(ProjectFileNamespace + "Reference").Any());

				foreach (var itemGroup in referencesItemGroups) {
					FixProjectReferences(projectDirectory, itemGroup, projectsMap);
				}

				projectDoc.Save(project.FilePath);
			}
		}

		private static void FixProjectReferences(string projectDirectory, XElement itemGroup, IDictionary<string, ProjectItem> projects)
		{
			foreach (var item in itemGroup.Elements(ProjectFileNamespace + "Reference").ToList()) {
				var assemblyName = item.Attribute("Include")?.Value;
				if (assemblyName != null && projects.TryGetValue(assemblyName, out var referencedProject)) {
					item.Remove();

					var projectReference = new XElement(ProjectFileNamespace + "ProjectReference",
						new XElement(ProjectFileNamespace + "Project", referencedProject.Guid.ToString("B").ToUpperInvariant()),
						new XElement(ProjectFileNamespace + "Name", referencedProject.ProjectName));
					projectReference.SetAttributeValue("Include", GetRelativePath(projectDirectory, referencedProject.FilePath));

					itemGroup.Add(projectReference);
				}
			}
		}

		private static string GetRelativePath(string fromPath, string toPath)
		{
			Uri fromUri = new Uri(AppendDirectorySeparatorChar(fromPath));
			Uri toUri = new Uri(AppendDirectorySeparatorChar(toPath));

			if (fromUri.Scheme != toUri.Scheme) {
				return toPath;
			}

			Uri relativeUri = fromUri.MakeRelativeUri(toUri);
			string relativePath = Uri.UnescapeDataString(relativeUri.ToString());

			if (string.Equals(toUri.Scheme, Uri.UriSchemeFile, StringComparison.OrdinalIgnoreCase)) {
				relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			}

			return relativePath;
		}

		private static string AppendDirectorySeparatorChar(string path)
		{
			return Path.HasExtension(path) || path.EndsWith(Path.DirectorySeparatorChar.ToString())
				? path
				: path + Path.DirectorySeparatorChar;
		}
	}
}
