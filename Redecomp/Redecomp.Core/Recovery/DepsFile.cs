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

using System.Text.Json;

namespace Redecomp.Core.Recovery;

public enum DepsLibraryType
{
	Project,
	Package,
	/// <summary>Any other kind, such as a plain assembly reference.</summary>
	Other,
}

/// <summary>A library listed in a <c>.deps.json</c> file.</summary>
/// <param name="Name">Library name: the project or NuGet package id.</param>
/// <param name="Version">Library version.</param>
/// <param name="Type">Whether the library is a project of the application or a NuGet package.</param>
/// <param name="RuntimeAssemblies">File names of the assemblies the library deploys.</param>
/// <param name="Dependencies">Direct dependencies, by name.</param>
public sealed record DepsLibrary(string Name, string Version, DepsLibraryType Type,
	IReadOnlyList<string> RuntimeAssemblies, IReadOnlyList<string> Dependencies);

/// <summary>The dependency manifest the SDK writes next to an application (<c>App.deps.json</c>).</summary>
public sealed class DepsFile
{
	public required IReadOnlyList<DepsLibrary> Libraries { get; init; }

	/// <summary>The application's own project, which comes first in the manifest.</summary>
	public DepsLibrary Root => Libraries[0];

	public DepsLibrary? Find(string name) => Libraries.FirstOrDefault(l => l.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

	public static DepsFile Parse(string json)
	{
		using var doc = JsonDocument.Parse(json);
		var root = doc.RootElement;
		string targetName = root.GetProperty("runtimeTarget").GetProperty("name").GetString()!;
		var target = root.GetProperty("targets").GetProperty(targetName);
		var libraries = root.TryGetProperty("libraries", out var l) ? l : default;

		var result = new List<DepsLibrary>();
		foreach (var entry in target.EnumerateObject())
		{
			int slash = entry.Name.IndexOf('/');
			string name = entry.Name[..slash];
			string version = entry.Name[(slash + 1)..];
			var type = DepsLibraryType.Other;
			if (libraries.ValueKind == JsonValueKind.Object && libraries.TryGetProperty(entry.Name, out var library)
				&& library.TryGetProperty("type", out var typeElement))
			{
				type = typeElement.GetString() switch {
					"project" => DepsLibraryType.Project,
					"package" => DepsLibraryType.Package,
					_ => DepsLibraryType.Other,
				};
			}
			var assemblies = entry.Value.TryGetProperty("runtime", out var runtime)
				? runtime.EnumerateObject().Select(r => Path.GetFileName(r.Name)).ToList()
				: [];
			var dependencies = entry.Value.TryGetProperty("dependencies", out var deps)
				? deps.EnumerateObject().Select(d => d.Name).ToList()
				: [];
			result.Add(new DepsLibrary(name, version, type, assemblies, dependencies));
		}
		return new DepsFile { Libraries = result };
	}

	public static DepsFile Load(string path) => Parse(File.ReadAllText(path));
}
