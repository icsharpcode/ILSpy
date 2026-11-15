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
	public static ReconstructionResult Reconstruct(string assemblyPath, string outputDirectory)
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
		var decompiler = new WholeProjectDecompiler(settings, resolver, null, null, debugInfo);
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

		return new ReconstructionResult(origin, debugInfo?.Description, projectFile);
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
