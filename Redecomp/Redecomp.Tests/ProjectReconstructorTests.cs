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

using Redecomp.Core.Reconstruction;

using Redecomp.Bench;

namespace Redecomp.Tests;

public static class ProjectReconstructorTests
{
	static readonly string[] SampleSources = ["Program.cs", "Models/Facture.cs", "Services/CalculateurFacture.cs"];

	[Test]
	public static void UsesEmbeddedSourcesVerbatimAtTheirOriginalPaths()
	{
		string assembly = SampleBuilder.Build("Facturation", BuildVariant.EmbeddedSources);
		string output = ProjectBuilder.NewTempDirectory();
		var result = ProjectReconstructor.Reconstruct(assembly, output);
		Assert.AreEqual(SourceOrigin.EmbeddedSources, result.Origin, "origin");
		foreach (string relative in SampleSources)
		{
			string recovered = Path.Combine(output, relative);
			Assert.That(File.Exists(recovered), $"{relative} missing from the reconstructed project");
			string original = Path.Combine(SampleBuilder.SourceDirectory("Facturation"), relative);
			Assert.That(File.ReadAllBytes(original).AsSpan().SequenceEqual(File.ReadAllBytes(recovered)), $"{relative} differs from the original");
		}
	}

	[Test]
	public static void EmbeddedSourcesProjectRebuilds()
	{
		AssertRebuilds(BuildVariant.EmbeddedSources);
	}

	[Test]
	public static void DecompiledProjectRebuildsWithoutPdb()
	{
		AssertRebuilds(BuildVariant.NoPdb);
		Assert.AreEqual(SourceOrigin.Decompiled, ProjectReconstructor.Reconstruct(SampleBuilder.Build("Facturation", BuildVariant.NoPdb), ProjectBuilder.NewTempDirectory()).Origin, "origin");
	}

	[Test]
	public static void DecompiledProjectUsesLocalNamesFromPortablePdb()
	{
		string assembly = SampleBuilder.Build("Facturation", BuildVariant.PortablePdbWithDocs);
		string output = ProjectBuilder.NewTempDirectory();
		var result = ProjectReconstructor.Reconstruct(assembly, output);
		Assert.That(result.DebugInfo != null, "the portable PDB was not loaded");
		string code = string.Concat(Directory.EnumerateFiles(output, "*.cs", SearchOption.AllDirectories).Select(File.ReadAllText));
		Assert.That(code.Contains("decimal totalHt"), "local variable name from the PDB is missing");
	}

	static string AssertRebuilds(BuildVariant variant)
	{
		string assembly = SampleBuilder.Build("Facturation", variant);
		string output = ProjectBuilder.NewTempDirectory();
		var result = ProjectReconstructor.Reconstruct(assembly, output);
		Assert.That(File.Exists(result.ProjectFile), "project file missing");
		string rebuilt = ProjectBuilder.BuildProject(output, "Facturation");
		Assert.That(File.Exists(rebuilt), $"rebuilt assembly missing for {variant}");
		return output;
	}

	[Test]
	public static void DropsFrameworkReferencesThatTheSdkProvides()
	{
		string output = ProjectBuilder.NewTempDirectory();
		var result = ProjectReconstructor.Reconstruct(SampleBuilder.Build("Facturation", BuildVariant.NoPdb), output);
		Assert.That(!File.ReadAllText(result.ProjectFile).Contains("<HintPath>"), "framework assemblies referenced by absolute path");
	}
}
