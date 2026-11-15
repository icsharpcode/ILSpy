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

/// <summary>What is known about the dependencies of the project being reconstructed.</summary>
/// <param name="Deps">The application's dependency manifest.</param>
/// <param name="Library">The manifest entry of the project being reconstructed.</param>
/// <param name="ProjectFileOf">
/// Path of the reconstructed project file of another project library, relative to this project's
/// directory; null when the other projects are not reconstructed alongside this one.
/// </param>
public sealed record DependencyContext(DepsFile Deps, DepsLibrary Library, Func<DepsLibrary, string>? ProjectFileOf);

/// <summary>
/// Replaces the assembly references the decompiler writes, which point at files on the machine that
/// ran it, by what the original project declared: framework references implied by the SDK, NuGet
/// package references and project references.
/// </summary>
public static class ReferenceRewriter
{
	static readonly string DotnetRoot = FindDotnetRoot();

	public static void Apply(string projectFile, DependencyContext? dependencies)
	{
		var doc = XDocument.Load(projectFile);
		var references = doc.Descendants("Reference").ToList();
		foreach (var reference in references)
		{
			string name = reference.Attribute("Include")?.Value ?? "";
			string? hintPath = reference.Element("HintPath")?.Value;
			if (hintPath != null && IsFrameworkAssembly(Path.GetDirectoryName(Path.GetFullPath(projectFile))!, hintPath))
			{
				reference.Remove();
				continue;
			}
			var owner = dependencies?.Deps.Libraries.FirstOrDefault(l => l.RuntimeAssemblies.Any(a => Path.GetFileNameWithoutExtension(a).Equals(name, StringComparison.OrdinalIgnoreCase)));
			if (owner?.Type == DepsLibraryType.Package
				|| (owner?.Type == DepsLibraryType.Project && dependencies!.ProjectFileOf != null))
			{
				reference.Remove();
			}
		}

		if (dependencies != null)
		{
			var items = new XElement("ItemGroup");
			foreach (string dependencyName in dependencies.Library.Dependencies)
			{
				var dependency = dependencies.Deps.Find(dependencyName);
				if (dependency?.Type == DepsLibraryType.Package)
				{
					items.Add(new XElement("PackageReference",
						new XAttribute("Include", dependency.Name), new XAttribute("Version", dependency.Version)));
				}
				else if (dependency?.Type == DepsLibraryType.Project && dependencies.ProjectFileOf != null)
				{
					items.Add(new XElement("ProjectReference", new XAttribute("Include", dependencies.ProjectFileOf(dependency))));
				}
			}
			if (items.HasElements)
				doc.Root!.Add(items);
		}

		foreach (var group in doc.Root!.Elements().Where(g => g.Name.LocalName is "ItemGroup" or "PropertyGroup" && !g.HasElements && !g.HasAttributes).ToList())
			group.Remove();
		doc.Save(projectFile);
	}

	// The decompiler writes hint paths relative to the project directory when it can.
	static bool IsFrameworkAssembly(string projectDirectory, string hintPath)
	{
		string full = Path.GetFullPath(hintPath, projectDirectory);
		return full.StartsWith(DotnetRoot, StringComparison.OrdinalIgnoreCase);
	}

	// The shared framework lives in <dotnet root>/shared/<framework>/<version>/.
	static string FindDotnetRoot()
	{
		string frameworkDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
		return Path.GetFullPath(Path.Combine(frameworkDirectory, "..", "..", "..")) + Path.DirectorySeparatorChar;
	}
}
