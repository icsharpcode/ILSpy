// Copyright (c) 2026 Christoph Wille
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

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

using NUnit.Framework;

using static ICSharpCode.ILSpyCmd.Tests.CliTestRunner;

namespace ICSharpCode.ILSpyCmd.Tests
{
	/// <summary>
	/// The HTML diagrammer's script reads a JSON model of the assembly; these tests pin the
	/// shape of that model as written by ilspycmd.
	/// </summary>
	[TestFixture]
	public class GenerateDiagrammerOptionTests
	{
		static readonly string testAssemblyPath = typeof(GenerateDiagrammerOptionTests).Assembly.Location;

		string outputDirectory;

		[SetUp]
		public void SetUp()
		{
			outputDirectory = Path.Combine(Path.GetTempPath(), "ILSpyDiagrammerTest_" + Path.GetRandomFileName());
		}

		[TearDown]
		public void TearDown()
		{
			if (Directory.Exists(outputDirectory))
				Directory.Delete(outputDirectory, recursive: true);
		}

		[Test]
		public async Task JsonOnlyOutputWrapsTheModelWithTheTemplateData()
		{
			var result = await RunAsync(testAssemblyPath, "--generate-diagrammer", "--generate-diagrammer-json-only", "-o", outputDirectory);

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			using var bundle = JsonDocument.Parse(File.ReadAllText(Path.Combine(outputDirectory, "model.json")));
			var root = bundle.RootElement;
			Assert.That(root.GetProperty("SourceAssemblyName").GetString(), Is.EqualTo("ICSharpCode.ILSpyCmd.Tests"));
			Assert.That(root.GetProperty("SourceAssemblyVersion").GetString(), Is.Not.Empty);
			Assert.That(root.GetProperty("BuilderVersion").GetString(), Is.Not.Empty);
			// The model is pre-serialized to a string so a JS build task can splice it into the template.
			using var model = JsonDocument.Parse(root.GetProperty("Model").GetString());
			AssertModelDescribesThisAssembly(model.RootElement);
		}

		[Test]
		public async Task HtmlOutputEmbedsTheModel()
		{
			var result = await RunAsync(testAssemblyPath, "--generate-diagrammer", "-o", outputDirectory);

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(File.Exists(Path.Combine(outputDirectory, "script.js")), "script.js");
			string html = File.ReadAllText(Path.Combine(outputDirectory, "index.html"));
			Assert.That(html, Does.Contain("\"TypesByNamespace\""));
			Assert.That(html, Does.Contain("\"ICSharpCode.ILSpyCmd.Tests\""));
			Assert.That(html, Does.Contain("\"" + nameof(DumpTableSample) + "\""));
		}

		[Test]
		public async Task HtmlOutputEscapesAssemblyNameAndDoesNotReprocessTemplateTokens()
		{
			const string assemblyName = "Lib<img src=x onerror=alert(1)> {{Model}}";
			Directory.CreateDirectory(outputDirectory);
			string assemblyPath = Path.Combine(outputDirectory, "HostileAssembly.dll");
			string diagramDirectory = Path.Combine(outputDirectory, "diagram");
			CreateAssembly(assemblyPath, assemblyName);

			var result = await RunAsync(assemblyPath, "--generate-diagrammer", "-o", diagramDirectory);

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			string html = File.ReadAllText(Path.Combine(diagramDirectory, "index.html"));
			Assert.That(html, Does.Not.Contain("<img src=x onerror=alert(1)>"));
			Assert.That(html, Does.Contain("Lib&lt;img src=x onerror=alert(1)&gt; {{Model}}"));
			Assert.That(html.Split("\"TypesByNamespace\"").Length - 1, Is.EqualTo(1));
		}

		static void CreateAssembly(string assemblyPath, string assemblyName)
		{
			var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
				.Split(Path.PathSeparator)
				.Select(path => MetadataReference.CreateFromFile(path));
			var compilation = CSharpCompilation.Create(assemblyName,
				[CSharpSyntaxTree.ParseText("public class TestType { }")],
				references,
				new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
			using var output = File.Create(assemblyPath);
			var result = compilation.Emit(output);
			Assert.That(result.Success, Is.True, () => string.Join(Environment.NewLine, result.Diagnostics));
		}

		static void AssertModelDescribesThisAssembly(JsonElement model)
		{
			Assert.That(model.TryGetProperty("OutsideReferences", out var outsideReferences), "OutsideReferences");
			Assert.That(outsideReferences.ValueKind, Is.EqualTo(JsonValueKind.Object));
			var types = model.GetProperty("TypesByNamespace").GetProperty("ICSharpCode.ILSpyCmd.Tests");
			var sample = types.GetProperty(nameof(DumpTableSample));
			// "Name" is only written for types whose display name differs from the key, e.g. generic ones.
			Assert.That(sample.GetProperty("Body").GetString(), Does.Contain(nameof(DumpTableSample.SampleProperty)));
		}
	}
}
