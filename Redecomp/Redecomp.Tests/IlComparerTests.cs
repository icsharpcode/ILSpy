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

using Redecomp.Bench;
using Redecomp.Core.Reconstruction;
using Redecomp.Core.Verification;

namespace Redecomp.Tests;

public static class IlComparerTests
{
	[Test]
	public static void FindsEveryMethodIdenticalWhenComparingAnAssemblyWithItself()
	{
		string assembly = SampleBuilder.Build("Facturation", BuildVariant.NoPdb);
		var comparison = IlComparer.Compare(assembly, assembly);
		Assert.That(comparison.Total > 5, $"too few methods compared: {comparison.Total}");
		Assert.That(comparison.IsIdentical, $"self comparison: {comparison}");
	}

	[Test]
	public static void RebuiltOriginalSourcesProduceIdenticalIl()
	{
		string original = SampleBuilder.Build("Facturation", BuildVariant.EmbeddedSources);
		string output = ProjectBuilder.NewTempDirectory();
		ProjectReconstructor.Reconstruct(original, output);
		var comparison = IlComparer.Compare(original, ProjectBuilder.BuildProject(output, "Facturation"));
		Assert.That(comparison.IsIdentical, $"{comparison}; differing: {string.Join(" | ", comparison.Differing)}; missing: {string.Join(" | ", comparison.Missing)}");
	}

	[Test]
	public static void ReportsTheMethodWhoseBehaviorChanged()
	{
		string original = SampleBuilder.Build("Facturation", BuildVariant.EmbeddedSources);
		string output = ProjectBuilder.NewTempDirectory();
		ProjectReconstructor.Reconstruct(original, output);
		string calculator = Path.Combine(output, "Services", "CalculateurFacture.cs");
		File.WriteAllText(calculator, File.ReadAllText(calculator).Replace("0.05m", "0.06m"));

		var comparison = IlComparer.Compare(original, ProjectBuilder.BuildProject(output, "Facturation"));
		Assert.AreEqual(1, comparison.Differing.Count, $"differing methods: {string.Join(" | ", comparison.Differing)}");
		Assert.That(comparison.Differing[0].Contains("CalculerTotalTtc"), $"wrong method reported: {comparison.Differing[0]}");
		Assert.AreEqual(0, comparison.Missing.Count, "missing methods");
	}

	[Test]
	public static void VerifierBuildsTheProjectAndComparesIt()
	{
		string original = SampleBuilder.Build("Facturation", BuildVariant.EmbeddedSources);
		string output = ProjectBuilder.NewTempDirectory();
		var result = ProjectReconstructor.Reconstruct(original, output);
		var comparison = FidelityVerifier.Verify(original, result.ProjectFile);
		Assert.That(comparison.IsIdentical, $"verification: {comparison}");
	}
}
