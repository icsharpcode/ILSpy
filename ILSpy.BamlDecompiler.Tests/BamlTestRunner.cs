// Copyright (c) 2020 AlphaSierraPapa for the SharpDevelop Team
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

		[Test]
		public void Issue2052()
		{
			RunTest("cases/issue2052");
		}

		[Test]
		public void Issue2097()
		{
			RunTest("cases/issue2097");
		}

		[Test]
		public void Issue2116()
		{
			RunTest("cases/issue2116");
		}

		#region RunTest
		void RunTest(string name)
		{
			RunTest(name, typeof(BamlTestRunner).Assembly.Location,
				Path.Combine(
					Path.GetDirectoryName(typeof(BamlTestRunner).Assembly.Location),
					"../../../..", name + ".xaml"));
		}

		void RunTest(string name, string asmPath, string sourcePath)
		{
			using (var fileStream = new FileStream(asmPath, FileMode.Open, FileAccess.Read))
			{
				var module = new PEFile(asmPath, fileStream);
				var resolver = new UniversalAssemblyResolver(asmPath, false, module.Reader.DetectTargetFrameworkId());
				resolver.RemoveSearchDirectory(".");
				resolver.AddSearchDirectory(Path.GetDirectoryName(asmPath));
				var res = module.Resources.First();
				Stream bamlStream = LoadBaml(res, name + ".baml");
				Assert.IsNotNull(bamlStream);

				BamlDecompilerTypeSystem typeSystem = new BamlDecompilerTypeSystem(module, resolver);
				var decompiler = new XamlDecompiler(typeSystem, new BamlDecompilerSettings());

				XDocument document = decompiler.Decompile(bamlStream).Xaml;

				XamlIsEqual(File.ReadAllText(sourcePath), document.ToString());
			}
		}

		void XamlIsEqual(string input1, string input2)
		{
			var diff = new StringWriter();
			if (!CodeComparer.Compare(input1, input2, diff, NormalizeLine))
			{
				Assert.Fail(diff.ToString());
			}
		}

		string NormalizeLine(string line)
		{
			return line.Trim();
		}

		Stream LoadBaml(Resource res, string name)
		{
			if (res.ResourceType != ResourceType.Embedded)
				return null;
			Stream s = res.TryOpenStream();
			if (s == null)
				return null;
			s.Position = 0;
			ResourcesFile resources;
			try
			{
				resources = new ResourcesFile(s);
			}
			catch (ArgumentException)
			{
				return null;
			}
			foreach (var entry in resources.OrderBy(e => e.Key))
			{
				if (entry.Key == name)
				{
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
