// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Resources;
using System.Threading;
using System.Xml.Linq;
using ICSharpCode.Decompiler.Tests.Helpers;
using Mono.Cecil;
using NUnit.Framework;

namespace ILSpy.BamlDecompiler.Tests
{
	[TestFixture]
	public class BamlTestRunner
	{
		[Test]
		public void Simple()
		{
			RunTest("cases/simple");
		}

		[Test]
		public void SimpleDictionary()
		{
			RunTest("cases/simpledictionary");
		}

		[Test]
		public void Resources()
		{
			RunTest("cases/resources");
		}

		[Test]
		public void SimpleNames()
		{
			RunTest("cases/simplenames");
		}

		[Test]
		public void AvalonDockBrushes()
		{
			RunTest("cases/avalondockbrushes");
		}

		[Test]
		public void AvalonDockCommon()
		{
			RunTest("cases/avalondockcommon");
		}

		[Test]
		public void AttachedEvent()
		{
			RunTest("cases/attachedevent");
		}

		[Test]
		public void Dictionary1()
		{
			RunTest("cases/dictionary1");
		}

		[Test]
		public void Issue775()
		{
			RunTest("cases/issue775");
		}

		[Test]
		public void MarkupExtension()
		{
			RunTest("cases/markupextension");
		}

		[Test]
		public void SimplePropertyElement()
		{
			RunTest("cases/simplepropertyelement");
		}

		[Test]
		public void Issue445()
		{
			RunTest("cases/issue445");
		}

		[Test]
		public void NamespacePrefix()
		{
			RunTest("cases/namespaceprefix");
		}

		[Test]
		public void EscapeSequence()
		{
			RunTest("cases/escapesequence");
		}

		#region RunTest
		void RunTest(string name)
		{
			RunTest(name, typeof(BamlTestRunner).Assembly.Location, Path.Combine("..\\..\\..\\..\\ILSpy.BamlDecompiler.Tests", name + ".xaml"));
		}

		void RunTest(string name, string asmPath, string sourcePath)
		{
			var resolver = new DefaultAssemblyResolver();
			var assembly = AssemblyDefinition.ReadAssembly(asmPath, new ReaderParameters { AssemblyResolver = resolver, InMemory = true });
			Resource res = assembly.MainModule.Resources.First();
			Stream bamlStream = LoadBaml(res, name + ".baml");
			Assert.IsNotNull(bamlStream);
			XDocument document = BamlResourceEntryNode.LoadIntoDocument(resolver, assembly, bamlStream, CancellationToken.None);

			XamlIsEqual(File.ReadAllText(sourcePath), document.ToString());
		}

		void XamlIsEqual(string input1, string input2)
		{
			var diff = new StringWriter();
			if (!CodeComparer.Compare(input1, input2, diff, NormalizeLine)) {
				Assert.Fail(diff.ToString());
			}
		}

		string NormalizeLine(string line)
		{
			return line.Trim();
		}

		Stream LoadBaml(Resource res, string name)
		{
			EmbeddedResource er = res as EmbeddedResource;
			if (er != null) {
				Stream s = er.GetResourceStream();
				s.Position = 0;
				ResourceReader reader;
				try {
					reader = new ResourceReader(s);
				} catch (ArgumentException) {
					return null;
				}
				foreach (DictionaryEntry entry in reader.Cast<DictionaryEntry>().OrderBy(e => e.Key.ToString())) {
					if (entry.Key.ToString() == name) {
						if (entry.Value is Stream)
							return (Stream)entry.Value;
						if (entry.Value is byte[])
							return new MemoryStream((byte[])entry.Value);
					}
				}
			}

			return null;
		}
		#endregion
	}
}
