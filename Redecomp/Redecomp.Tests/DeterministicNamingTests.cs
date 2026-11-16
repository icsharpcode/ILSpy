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

public static class DeterministicNamingTests
{
	[Test]
	public static void NamesLocalsFromReturnedValueCallsAndAggregates()
	{
		string depot = DecompiledFile("Inventaire", BuildVariant.NoPdb, "Depot.cs");
		AssertContains(depot, "int ruptures = ", "local returned by CompterRuptures");
		AssertContains(depot, "int quantiteTotale = ", "local returned by CalculerQuantiteTotale");
		AssertContains(depot, "decimal valeurStock = ", "local returned by CalculerValeurStock");
		AssertContains(depot, "StringBuilder rapport = ", "local whose ToString() is returned by GenererRapport");
		AssertContains(depot, "int maxQuantite = ", "Max over the Quantite selector");
		AssertContains(depot, "string rapport = GenererRapport()", "result of GenererRapport");

		string program = DecompiledFile("Inventaire", BuildVariant.NoPdb, "Program.cs");
		AssertContains(program, "bool sauvegarde = await depot.SauvegarderAsync(", "boolean result of SauvegarderAsync");

		string calculateur = DecompiledFile("Facturation", BuildVariant.NoPdb, "CalculateurFacture.cs");
		AssertContains(calculateur, "decimal totalLignes = ", "Sum over the Lignes collection");
		string facturation = DecompiledFile("Facturation", BuildVariant.NoPdb, "Program.cs");
		AssertContains(facturation, "decimal totalTtc = calculateur.CalculerTotalTtc(", "result of CalculerTotalTtc");
	}

	[Test]
	public static void NeverRenamesVariablesNamedByThePdb()
	{
		string calculateur = DecompiledFile("Facturation", BuildVariant.PortablePdbWithDocs, "CalculateurFacture.cs");
		AssertContains(calculateur, "decimal totalHt = ", "PDB name totalHt");
		Assert.That(!calculateur.Contains("totalLignes"), "a PDB-named local was renamed");
	}

	[Test]
	public static void ProducesIdenticalOutputOnEveryRun()
	{
		string first = DecompiledFile("Inventaire", BuildVariant.NoPdb, "Depot.cs");
		string second = DecompiledFile("Inventaire", BuildVariant.NoPdb, "Depot.cs");
		Assert.AreEqual(first, second, "decompiled Depot.cs");
	}

	static string DecompiledFile(string sample, BuildVariant variant, string fileName)
	{
		string output = ProjectBuilder.NewTempDirectory();
		ProjectReconstructor.Reconstruct(SampleBuilder.Build(sample, variant), output);
		string path = Directory.EnumerateFiles(output, fileName, SearchOption.AllDirectories).Single();
		return File.ReadAllText(path);
	}

	static void AssertContains(string code, string expected, string what)
		=> Assert.That(code.Contains(expected), $"{what}: '{expected}' not found");
}
