using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Tests.Helpers;

using Microsoft.DiaSymReader.Tools;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	[TestFixture, Parallelizable(ParallelScope.All)]
	public class PdbGenerationTestRunner
	{
		static readonly string TestCasePath = Tester.TestCasePath + "/PdbGen";

		[Test]
		public void HelloWorld()
		{
			TestGeneratePdb();
		}

		[Test]
		[Ignore("Missing nested local scopes for loops, differences in IL ranges")]
		public void ForLoopTests()
		{
			TestGeneratePdb();
		}

		[Test]
		[Ignore("Differences in IL ranges")]
		public void LambdaCapturing()
		{
			TestGeneratePdb();
		}

		[Test]
		[Ignore("Duplicate sequence points for local function")]
		public void Members()
		{
			TestGeneratePdb();
		}

		[Test]
		public void CustomPdbId()
		{
			// Generate a PDB for an assembly using a randomly-generated ID, then validate that the PDB uses the specified ID
			(string peFileName, string pdbFileName) = CompileTestCase(nameof(CustomPdbId));

			var moduleDefinition = new PEFile(peFileName);
			var resolver = new UniversalAssemblyResolver(peFileName, false, moduleDefinition.Metadata.DetectTargetFrameworkId(), null, PEStreamOptions.PrefetchEntireImage);
			var decompiler = new CSharpDecompiler(moduleDefinition, resolver, new DecompilerSettings());
			var expectedPdbId = new BlobContentId(Guid.NewGuid(), (uint)Random.Shared.Next());

			using (FileStream pdbStream = File.Open(Path.Combine(TestCasePath, nameof(CustomPdbId) + ".pdb"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				pdbStream.SetLength(0);
				new PortablePdbWriter { NoLogo = true, PdbId = expectedPdbId }
					.WritePdb(moduleDefinition, decompiler, new DecompilerSettings(), pdbStream);

				pdbStream.Position = 0;
				var metadataReader = MetadataReaderProvider.FromPortablePdbStream(pdbStream).GetMetadataReader();
				var generatedPdbId = new BlobContentId(metadataReader.DebugMetadataHeader.Id);

				Assert.That(generatedPdbId.Guid, Is.EqualTo(expectedPdbId.Guid));
				Assert.That(generatedPdbId.Stamp, Is.EqualTo(expectedPdbId.Stamp));
			}
		}

		[Test]
		public void EmbedSourceFiles_False_Omits_Embedded_Source()
		{
			// Generating a PDB alongside a project export (where the .cs are already on disk) should be
			// able to skip embedding the source again. EmbedSourceFiles = false must drop the
			// EmbeddedSource custom debug info (smaller PDB, no embeddedSourceLength) while keeping the
			// document checksum/hash intact.

			// Compile HelloWorld's source into a uniquely-named assembly so this test never collides
			// with the HelloWorld fixture's .expected files under the fixture's parallel scope.
			string sourceXml = Path.Combine(TestCasePath, nameof(HelloWorld) + ".xml");
			var files = XDocument.Parse(File.ReadAllText(sourceXml))
				.Descendants("file").ToDictionary(f => f.Attribute("name").Value, f => f.Value);
			string outputBase = Path.Combine(TestCasePath, nameof(EmbedSourceFiles_False_Omits_Embedded_Source) + ".expected");
			Tester.CompileCSharpWithPdb(outputBase, files);
			string peFileName = outputBase + ".dll";

			var module = new PEFile(peFileName);
			var resolver = new UniversalAssemblyResolver(peFileName, false,
				module.Metadata.DetectTargetFrameworkId(), null, PEStreamOptions.PrefetchEntireImage);

			byte[] WritePdbBytes(bool embedSources)
			{
				var decompiler = new CSharpDecompiler(module, resolver, new DecompilerSettings());
				using var ms = new MemoryStream();
				new PortablePdbWriter { NoLogo = true, EmbedSourceFiles = embedSources }
					.WritePdb(module, decompiler, new DecompilerSettings(), ms);
				return ms.ToArray();
			}

			// Inspect the portable PDB metadata directly (environment-independent): count the
			// EmbeddedSource custom-debug-information rows and the source documents.
			(int EmbeddedSources, int Documents) Inspect(byte[] pdb)
			{
				using var ms = new MemoryStream(pdb);
				var reader = MetadataReaderProvider.FromPortablePdbStream(ms).GetMetadataReader();
				int embedded = 0;
				foreach (var handle in reader.CustomDebugInformation)
				{
					var cdi = reader.GetCustomDebugInformation(handle);
					if (reader.GetGuid(cdi.Kind) == KnownGuids.EmbeddedSource)
						embedded++;
				}
				return (embedded, reader.Documents.Count);
			}

			var withEmbed = WritePdbBytes(embedSources: true);
			var withoutEmbed = WritePdbBytes(embedSources: false);

			var embedInfo = Inspect(withEmbed);
			var noEmbedInfo = Inspect(withoutEmbed);

			Assert.That(embedInfo.EmbeddedSources, Is.GreaterThan(0),
				"the default (embed = true) keeps the embedded-source blobs");
			Assert.That(noEmbedInfo.EmbeddedSources, Is.EqualTo(0),
				"embed = false must omit every embedded-source blob");
			Assert.That(noEmbedInfo.Documents, Is.EqualTo(embedInfo.Documents).And.GreaterThan(0),
				"the source documents (with their checksum/hash) must remain even without embedded source");
			Assert.That(withoutEmbed.Length, Is.LessThan(withEmbed.Length),
				"omitting embedded source must produce a strictly smaller PDB");
		}

		[Test]
		public void ProgressReporting()
		{
			// Generate a PDB for an assembly and validate that the progress reporter is called with reasonable values
			(string peFileName, string pdbFileName) = CompileTestCase(nameof(ProgressReporting));

			var moduleDefinition = new PEFile(peFileName);
			var resolver = new UniversalAssemblyResolver(peFileName, false, moduleDefinition.Metadata.DetectTargetFrameworkId(), null, PEStreamOptions.PrefetchEntireImage);
			var decompiler = new CSharpDecompiler(moduleDefinition, resolver, new DecompilerSettings());

			var lastFilesWritten = 0;
			var totalFiles = -1;

			Action<DecompilationProgress> reportFunc = progress => {
				if (totalFiles == -1)
				{
					// Initialize value on first call
					totalFiles = progress.TotalUnits;
				}

				Assert.That(totalFiles, Is.EqualTo(progress.TotalUnits));
				Assert.That(lastFilesWritten + 1, Is.EqualTo(progress.UnitsCompleted));

				lastFilesWritten = progress.UnitsCompleted;
			};

			using (FileStream pdbStream = File.Open(Path.Combine(TestCasePath, nameof(ProgressReporting) + ".pdb"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				pdbStream.SetLength(0);
				new PortablePdbWriter { NoLogo = true, Progress = new TestProgressReporter(reportFunc) }
					.WritePdb(moduleDefinition, decompiler, new DecompilerSettings(), pdbStream);

				pdbStream.Position = 0;
				var metadataReader = MetadataReaderProvider.FromPortablePdbStream(pdbStream).GetMetadataReader();
				var generatedPdbId = new BlobContentId(metadataReader.DebugMetadataHeader.Id);
			}

			Assert.That(lastFilesWritten, Is.EqualTo(totalFiles));
		}

		private class TestProgressReporter : IProgress<DecompilationProgress>
		{
			private Action<DecompilationProgress> reportFunc;

			public TestProgressReporter(Action<DecompilationProgress> reportFunc)
			{
				this.reportFunc = reportFunc;
			}

			public void Report(DecompilationProgress value)
			{
				reportFunc(value);
			}
		}

		private void TestGeneratePdb([CallerMemberName] string testName = null)
		{
			const PdbToXmlOptions options = PdbToXmlOptions.IncludeEmbeddedSources | PdbToXmlOptions.ThrowOnError | PdbToXmlOptions.IncludeTokens | PdbToXmlOptions.ResolveTokens | PdbToXmlOptions.IncludeMethodSpans;

			string xmlFile = Path.Combine(TestCasePath, testName + ".xml");
			(string peFileName, string pdbFileName) = CompileTestCase(testName);

			var moduleDefinition = new PEFile(peFileName);
			var resolver = new UniversalAssemblyResolver(peFileName, false, moduleDefinition.Metadata.DetectTargetFrameworkId(), null, PEStreamOptions.PrefetchEntireImage);
			var decompiler = new CSharpDecompiler(moduleDefinition, resolver, new DecompilerSettings());
			using (FileStream pdbStream = File.Open(Path.Combine(TestCasePath, testName + ".pdb"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				pdbStream.SetLength(0);
				new PortablePdbWriter { NoLogo = true }
					.WritePdb(moduleDefinition, decompiler, new DecompilerSettings(), pdbStream);
				pdbStream.Position = 0;
				using (Stream peStream = File.OpenRead(peFileName))
				using (Stream expectedPdbStream = File.OpenRead(pdbFileName))
				{
					using (StreamWriter writer = new StreamWriter(Path.ChangeExtension(pdbFileName, ".xml"), false, Encoding.UTF8))
					{
						PdbToXmlConverter.ToXml(writer, expectedPdbStream, peStream, options);
					}
					peStream.Position = 0;
					using (StreamWriter writer = new StreamWriter(Path.ChangeExtension(xmlFile, ".generated.xml"), false, Encoding.UTF8))
					{
						PdbToXmlConverter.ToXml(writer, pdbStream, peStream, options);
					}
				}
			}
			string expectedFileName = Path.ChangeExtension(xmlFile, ".expected.xml");
			ProcessXmlFile(expectedFileName);
			string generatedFileName = Path.ChangeExtension(xmlFile, ".generated.xml");
			ProcessXmlFile(generatedFileName);
			CodeAssert.AreEqual(Normalize(expectedFileName), Normalize(generatedFileName));
		}

		private (string peFileName, string pdbFileName) CompileTestCase(string testName)
		{
			string xmlFile = Path.Combine(TestCasePath, testName + ".xml");
			string xmlContent = File.ReadAllText(xmlFile);
			XDocument document = XDocument.Parse(xmlContent);
			var files = document.Descendants("file").ToDictionary(f => f.Attribute("name").Value, f => f.Value);
			Tester.CompileCSharpWithPdb(Path.Combine(TestCasePath, testName + ".expected"), files);

			string peFileName = Path.Combine(TestCasePath, testName + ".expected.dll");
			string pdbFileName = Path.Combine(TestCasePath, testName + ".expected.pdb");

			return (peFileName, pdbFileName);
		}

		private void ProcessXmlFile(string fileName)
		{
			var document = XDocument.Load(fileName);
			foreach (var file in document.Descendants("file"))
			{
				file.Attribute("checksum").Remove();
				file.Attribute("embeddedSourceLength")?.Remove();
				var name = file.Attribute("name");
				if (name != null)
				{
					// Generated document names use the platform's directory separator;
					// the expected files were produced on Windows.
					name.Value = name.Value.Replace('/', '\\');
				}
				file.ReplaceNodes(new XCData(file.Value.Replace("\uFEFF", "")));
			}
			document.Save(fileName, SaveOptions.None);
		}

		private string Normalize(string inputFileName)
		{
			return File.ReadAllText(inputFileName).Replace("\r\n", "\n").Replace("\r", "\n");
		}
	}

	class StringWriterWithEncoding : StringWriter
	{
		readonly Encoding encoding;

		public StringWriterWithEncoding(Encoding encoding)
		{
			this.encoding = encoding ?? throw new ArgumentNullException("encoding");
		}

		public override Encoding Encoding => encoding;
	}
}
