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

namespace Redecomp.Tests;

public static class OriginalLayoutTests
{
	[Test]
	public static void PlacesDecompiledTypesInTheFilesNamedByThePdb()
	{
		string assembly = SampleBuilder.Build("Facturation", BuildVariant.PortablePdbWithDocs);
		string output = ProjectBuilder.NewTempDirectory();
		ProjectReconstructor.Reconstruct(assembly, output);
		AssertFileDeclares(output, "Services/CalculateurFacture.cs", "class CalculateurFacture");
		// Facture.cs declares two records: they must end up together in that single file.
		AssertFileDeclares(output, "Models/Facture.cs", "record Facture(");
		AssertFileDeclares(output, "Models/Facture.cs", "record LigneFacture(");
		AssertFileDeclares(output, "Program.cs", "class Program");
		Assert.That(!Directory.Exists(Path.Combine(output, "Agl.Facturation.Services")), "namespace-named directory left behind");
	}

	[Test]
	public static void KeepsNamespaceLayoutWithoutPdb()
	{
		string assembly = SampleBuilder.Build("Facturation", BuildVariant.NoPdb);
		string output = ProjectBuilder.NewTempDirectory();
		ProjectReconstructor.Reconstruct(assembly, output);
		AssertFileDeclares(output, "Agl.Facturation.Services/CalculateurFacture.cs", "class CalculateurFacture");
	}

	static void AssertFileDeclares(string root, string relativePath, string declaration)
	{
		string path = Path.Combine(root, relativePath);
		Assert.That(File.Exists(path), $"{relativePath} missing");
		Assert.That(File.ReadAllText(path).Contains(declaration), $"{relativePath} does not declare '{declaration}'");
	}
}
