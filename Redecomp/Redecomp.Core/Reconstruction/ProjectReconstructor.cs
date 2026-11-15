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

using System.Reflection.Metadata;
using System.Xml.Linq;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp.ProjectDecompiler;
using ICSharpCode.Decompiler.Metadata;

using Redecomp.Core.Recovery;
using Redecomp.Core.Vendor;

namespace Redecomp.Core.Reconstruction;

/// <summary>Rebuilds a complete project directory from a compiled assembly.</summary>
public static class ProjectReconstructor
{
	public static ReconstructionResult Reconstruct(string assemblyPath, string outputDirectory, ReconstructionOptions? options = null)
		=> Reconstruct(assemblyPath, outputDirectory, options ?? ReconstructionOptions.Default, FindDependencies(assemblyPath));

	/// <summary>Reconstructs one project, with the dependency information of the application it belongs to.</summary>
	public static ReconstructionResult Reconstruct(string assemblyPath, string outputDirectory, ReconstructionOptions options, DependencyContext? dependencies)
	{
		assemblyPath = Path.GetFullPath(assemblyPath);
		Directory.CreateDirectory(outputDirectory);

		using var module = new PEFile(assemblyPath);
		using var pdb = PortablePdbLocator.Open(assemblyPath);
		using var debugInfo = pdb == null ? null : new PortableDebugInfoProvider(assemblyPath, pdb.Provider, MetadataReaderOptions.Default, pdb.FilePath);

		// The decompiler always runs: it produces the project file, resources and assembly attributes,
		// which are not part of the embedded sources.
		var resolver = new UniversalAssemblyResolver(assemblyPath, false, module.Metadata.DetectTargetFrameworkId());
		var settings = new DecompilerSettings(ICSharpCode.Decompiler.CSharp.LanguageVersion.Latest) {
			UseSdkStyleProjectFormat = true,
		};
		var decompiler = new RedecompProjectDecompiler(settings, resolver, debugInfo, options);
		decompiler.DecompileProject(module, outputDirectory);
		string projectFile = Directory.EnumerateFiles(outputDirectory, "*.csproj").Single();

		var embedded = EmbeddedSourceReader.Read(assemblyPath);
		var layout = EmbeddedSourceLayout.Create(embedded);
		SourceOrigin origin = SourceOrigin.Decompiled;
		if (layout.UserFiles.Count > 0)
		{
			ReplaceDecompiledSources(outputDirectory, layout);
			if (layout.UsedImplicitUsings)
				SetProjectProperty(projectFile, "ImplicitUsings", "enable");
			origin = SourceOrigin.EmbeddedSources;
		}
		else if (pdb != null)
		{
			var pdbReader = pdb.Provider.GetMetadataReader();
			SourceRelocator.Apply(module.Metadata, pdbReader, outputDirectory, settings.UseNestedDirectoriesForNamespaces);
			if (options.SourceDownloader != null && ApplySourceLink(pdbReader, options.SourceDownloader, outputDirectory, projectFile))
				origin = SourceOrigin.SourceLink;
		}

		ReferenceRewriter.Apply(projectFile, dependencies);
		if (options.IncludeContentFiles)
			CopyContentFiles(assemblyPath, outputDirectory, projectFile);

		return new ReconstructionResult(origin, debugInfo?.Description, projectFile);
	}

	/// <summary>
	/// Overwrites the relocated decompiled files with the originals SourceLink can recover.
	/// Returns true when every source file of the project was recovered.
	/// </summary>
	static bool ApplySourceLink(MetadataReader pdb, ISourceDownloader downloader, string outputDirectory, string projectFile)
	{
		var recovered = SourceLinkRecovery.Download(pdb, downloader);
		if (recovered.Count == 0)
			return false;
		var documents = pdb.Documents.Select(h => pdb.GetString(pdb.GetDocument(h).Name)).ToList();
		var userDocuments = documents.Where(d => !ProjectPaths.IsGenerated(d)).ToList();
		var relative = ProjectPaths.ToProjectRelative(userDocuments);
		foreach (var file in recovered)
		{
			string target = Path.Combine(outputDirectory, relative[file.OriginalPath]);
			Directory.CreateDirectory(Path.GetDirectoryName(target)!);
			File.WriteAllBytes(target, file.Content);
		}
		// Original files rely on the global usings the SDK generated for them.
		if (documents.Any(d => ProjectPaths.IsGenerated(d) && d.EndsWith(".GlobalUsings.g.cs", StringComparison.OrdinalIgnoreCase)))
			SetProjectProperty(projectFile, "ImplicitUsings", "enable");
		return recovered.Count == userDocuments.Count;
	}

	/// <summary>The manifest entry of a standalone assembly, from the .deps.json deployed next to it.</summary>
	static DependencyContext? FindDependencies(string assemblyPath)
	{
		string depsPath = Path.ChangeExtension(Path.GetFullPath(assemblyPath), ".deps.json");
		if (!File.Exists(depsPath))
			return null;
		var deps = DepsFile.Load(depsPath);
		string fileName = Path.GetFileName(assemblyPath);
		var library = deps.Libraries.FirstOrDefault(l => l.RuntimeAssemblies.Contains(fileName, StringComparer.OrdinalIgnoreCase));
		return library == null ? null : new DependencyContext(deps, library, null);
	}

	static void CopyContentFiles(string assemblyPath, string outputDirectory, string projectFile)
	{
		string assemblyDirectory = Path.GetDirectoryName(assemblyPath)!;
		var copied = new List<string>();
		foreach (string relative in ContentFileCollector.Collect(assemblyPath))
		{
			string target = Path.Combine(outputDirectory, relative);
			if (File.Exists(target))
				continue;
			Directory.CreateDirectory(Path.GetDirectoryName(target)!);
			File.Copy(Path.Combine(assemblyDirectory, relative), target);
			copied.Add(relative);
		}
		if (copied.Count == 0)
			return;
		// The files were found in the output directory, so the original project copied them there.
		var doc = XDocument.Load(projectFile);
		doc.Root!.Add(new XElement("ItemGroup",
			copied.Select(f => new XElement("None",
				new XAttribute("Update", f.Replace('/', '\\')),
				new XAttribute("CopyToOutputDirectory", "PreserveNewest")))));
		doc.Save(projectFile);
	}

	static void ReplaceDecompiledSources(string outputDirectory, EmbeddedSourceLayout layout)
	{
		string assemblyInfo = Path.Combine(outputDirectory, "Properties", "AssemblyInfo.cs");
		foreach (string file in Directory.EnumerateFiles(outputDirectory, "*.cs", SearchOption.AllDirectories).ToList())
		{
			// The decompiled assembly attributes stand in for the ones the SDK generated under obj/.
			if (file != assemblyInfo)
				File.Delete(file);
		}
		DeleteEmptyDirectories(outputDirectory);
		foreach (var (relativePath, content) in layout.UserFiles)
		{
			string target = Path.Combine(outputDirectory, relativePath);
			Directory.CreateDirectory(Path.GetDirectoryName(target)!);
			File.WriteAllBytes(target, content);
		}
	}

	static void DeleteEmptyDirectories(string root)
	{
		foreach (string dir in Directory.EnumerateDirectories(root, "*", SearchOption.AllDirectories).OrderByDescending(d => d.Length))
		{
			if (!Directory.EnumerateFileSystemEntries(dir).Any())
				Directory.Delete(dir);
		}
	}

	internal static void SetProjectProperty(string projectFile, string name, string value)
	{
		var doc = XDocument.Load(projectFile);
		var group = doc.Root!.Elements("PropertyGroup").First();
		var existing = group.Element(name);
		if (existing != null)
			existing.Value = value;
		else
			group.Add(new XElement(name, value));
		doc.Save(projectFile);
	}
}
