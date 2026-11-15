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

using Redecomp.Bench;
using Redecomp.Core.Reconstruction;

namespace Redecomp.Tests;

public static class SolutionReconstructorTests
{
	[Test]
	public static void CreatesOneProjectPerProjectAssemblyInASolution()
	{
		string output = Reconstruct(BuildVariant.NoPdb, out var result);
		Assert.AreEqual("Portail.App,Portail.Metier", string.Join(",", result.Projects.Select(p => p.Name)), "projects");
		Assert.AreEqual("Portail.App.slnx", Path.GetFileName(result.SolutionFile), "solution file name");
		var listed = XDocument.Load(result.SolutionFile).Descendants("Project").Select(p => p.Attribute("Path")!.Value.Replace('\\', '/')).Order();
		Assert.AreEqual("Portail.App/Portail.App.csproj,Portail.Metier/Portail.Metier.csproj", string.Join(",", listed), "projects in the solution");
		Assert.That(!Directory.Exists(Path.Combine(output, "Agl.Outils")), "a package assembly was decompiled as a project");
	}

	[Test]
	public static void LinksProjectsWithProjectReferences()
	{
		string output = Reconstruct(BuildVariant.NoPdb, out _);
		var app = XDocument.Load(Path.Combine(output, "Portail.App", "Portail.App.csproj"));
		var references = app.Descendants("ProjectReference").Select(r => r.Attribute("Include")!.Value.Replace('\\', '/')).ToList();
		Assert.AreEqual("../Portail.Metier/Portail.Metier.csproj", string.Join(",", references), "project references of Portail.App");
		Assert.That(!app.Descendants("Reference").Any(r => r.Attribute("Include")!.Value == "Portail.Metier"), "assembly reference to a project left behind");
	}

	[Test]
	public static void ReferencesNuGetPackagesWithTheirVersion()
	{
		string output = Reconstruct(BuildVariant.NoPdb, out _);
		var metier = XDocument.Load(Path.Combine(output, "Portail.Metier", "Portail.Metier.csproj"));
		var packages = metier.Descendants("PackageReference").Select(p => $"{p.Attribute("Include")!.Value}@{p.Attribute("Version")?.Value}").ToList();
		Assert.AreEqual("Agl.Outils@1.2.0", string.Join(",", packages), "package references of Portail.Metier");
	}

	[Test]
	public static void LeavesNoMachineSpecificHintPath()
	{
		string output = Reconstruct(BuildVariant.NoPdb, out _);
		foreach (string project in Directory.EnumerateFiles(output, "*.csproj", SearchOption.AllDirectories))
			Assert.That(!File.ReadAllText(project).Contains("<HintPath>"), $"{Path.GetFileName(project)} still has a HintPath");
	}

	[Test]
	public static void RebuiltSolutionBehavesLikeThePublishedApplication()
	{
		foreach (var variant in Enum.GetValues<BuildVariant>())
		{
			string publish = SampleBuilder.Publish("Portail", variant);
			string output = Reconstruct(variant, out _);
			string rebuilt = ProjectBuilder.BuildProject(Path.Combine(output, "Portail.App"), "Portail.App", SampleBuilder.LocalFeed);
			Assert.AreEqual(ProjectBuilder.RunAssembly(Path.Combine(publish, "Portail.App.dll")), ProjectBuilder.RunAssembly(rebuilt), $"output of the rebuilt {variant} solution");
		}
	}

	static string Reconstruct(BuildVariant variant, out SolutionResult result)
	{
		string output = ProjectBuilder.NewTempDirectory();
		result = SolutionReconstructor.Reconstruct(SampleBuilder.Publish("Portail", variant), output);
		return output;
	}
}
