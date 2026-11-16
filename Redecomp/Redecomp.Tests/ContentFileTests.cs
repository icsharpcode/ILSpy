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

public static class ContentFileTests
{
	[Test]
	public static void CopiesContentFilesShippedNextToTheAssembly()
	{
		string assembly = SampleBuilder.Build("Facturation", BuildVariant.NoPdb);
		string output = ProjectBuilder.NewTempDirectory();
		ProjectReconstructor.Reconstruct(assembly, output);
		string original = Path.Combine(SampleBuilder.SourceDirectory("Facturation"), "appsettings.json");
		string recovered = Path.Combine(output, "appsettings.json");
		Assert.That(File.Exists(recovered), "appsettings.json was not recovered");
		Assert.AreEqual(File.ReadAllText(original), File.ReadAllText(recovered), "appsettings.json content");
		Assert.That(!File.Exists(Path.Combine(output, "Facturation.runtimeconfig.json")), "build artifacts must not be copied");
	}

	[Test]
	public static void RebuiltApplicationBehavesLikeTheOriginal()
	{
		foreach (var variant in Enum.GetValues<BuildVariant>())
		{
			string assembly = SampleBuilder.Build("Facturation", variant);
			string output = ProjectBuilder.NewTempDirectory();
			ProjectReconstructor.Reconstruct(assembly, output);
			string rebuilt = ProjectBuilder.BuildProject(output, "Facturation");
			Assert.AreEqual(ProjectBuilder.RunAssembly(assembly), ProjectBuilder.RunAssembly(rebuilt), $"output of the rebuilt {variant} application");
		}
	}
}
