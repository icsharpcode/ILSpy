// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Tests.Helpers;
using ICSharpCode.Decompiler.Util;
using NUnit.Framework;

namespace ILSpy.BamlDecompiler.Tests
{
	[TestFixture, Parallelizable(ParallelScope.All)]
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

		[Test]
		public void Issue1435()
		{
			RunTest("cases/issue1435");
		}

		[Test]
		public void Issue1546()
		{
			RunTest("cases/issue1546");
		}

		[Test]
		public void Issue1547()
		{
			RunTest("cases/issue1547");
		}

		#region RunTest
		void RunTest(string name)
		{
			RunTest(name, typeof(BamlTestRunner).Assembly.Location,
				Path.Combine(
					Path.GetDirectoryName(typeof(BamlTestRunner).Assembly.Location),
					"../../../../ILSpy.BamlDecompiler.Tests", name + ".xaml"));
		}

		void RunTest(string name, string asmPath, string sourcePath)
		{
			using (var fileStream = new FileStream(asmPath, FileMode.Open, FileAccess.Read)) {
				var module = new PEFile(asmPath, fileStream);
				var resolver = new UniversalAssemblyResolver(asmPath, false, module.Reader.DetectTargetFrameworkId());
				resolver.RemoveSearchDirectory(".");
				resolver.AddSearchDirectory(Path.GetDirectoryName(asmPath));
				var res = module.Resources.First();
				Stream bamlStream = LoadBaml(res, name + ".baml");
				Assert.IsNotNull(bamlStream);
				XDocument document = BamlResourceEntryNode.LoadIntoDocument(module, resolver, bamlStream, CancellationToken.None);

				XamlIsEqual(File.ReadAllText(sourcePath), document.ToString());
			}
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
			if (res.ResourceType != ResourceType.Embedded) return null;
			Stream s = res.TryOpenStream();
			if (s == null) return null;
			s.Position = 0;
			ResourcesFile resources;
			try {
				resources = new ResourcesFile(s);
			} catch (ArgumentException) {
				return null;
			}
			foreach (var entry in resources.OrderBy(e => e.Key)) {
				if (entry.Key == name) {
					if (entry.Value is Stream)
						return (Stream)entry.Value;
					if (entry.Value is byte[])
						return new MemoryStream((byte[])entry.Value);
				}
			}
			return null;
		}
		#endregion
	}
}
