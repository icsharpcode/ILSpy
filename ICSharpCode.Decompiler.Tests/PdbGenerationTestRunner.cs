using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Tests.Helpers;
using ICSharpCode.Decompiler.TypeSystem;

using Microsoft.CodeAnalysis.CSharp;
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

		private void TestGeneratePdb([CallerMemberName] string testName = null)
		{
			const PdbToXmlOptions options = PdbToXmlOptions.IncludeEmbeddedSources | PdbToXmlOptions.ThrowOnError | PdbToXmlOptions.IncludeTokens | PdbToXmlOptions.ResolveTokens | PdbToXmlOptions.IncludeMethodSpans;

			string xmlFile = Path.Combine(TestCasePath, testName + ".xml");
			string xmlContent = File.ReadAllText(xmlFile);
			XDocument document = XDocument.Parse(xmlContent);
			var files = document.Descendants("file").ToDictionary(f => f.Attribute("name").Value, f => f.Value);
			Tester.CompileCSharpWithPdb(Path.Combine(TestCasePath, testName + ".expected"), files);

			string peFileName = Path.Combine(TestCasePath, testName + ".expected.dll");
			string pdbFileName = Path.Combine(TestCasePath, testName + ".expected.pdb");
			var moduleDefinition = new PEFile(peFileName);
			var resolver = new UniversalAssemblyResolver(peFileName, false, moduleDefinition.Metadata.DetectTargetFrameworkId(), null, PEStreamOptions.PrefetchEntireImage);
			var decompiler = new CSharpDecompiler(moduleDefinition, resolver, new DecompilerSettings());
			using (FileStream pdbStream = File.Open(Path.Combine(TestCasePath, testName + ".pdb"), FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				pdbStream.SetLength(0);
				PortablePdbWriter.WritePdb(moduleDefinition, decompiler, new DecompilerSettings(), pdbStream, noLogo: true);
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
			Assert.AreEqual(Normalize(expectedFileName), Normalize(generatedFileName));
		}

		private void ProcessXmlFile(string fileName)
		{
			var document = XDocument.Load(fileName);
			foreach (var file in document.Descendants("file"))
			{
				file.Attribute("checksum").Remove();
				file.Attribute("embeddedSourceLength")?.Remove();
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
