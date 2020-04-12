// Copyright (c) 2020 Siegfried Pammer
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using ICSharpCode.ILSpy.Analyzers;
using ICSharpCode.ILSpy.Analyzers.Builtin;
using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers
{
	[TestFixture, Parallelizable(ParallelScope.All)]
	public class TypeUsedByAnalyzerTests
	{
		AssemblyList assemblyList;
		CSharpLanguage language;
		LoadedAssembly testAssembly;
		ICompilation testAssemblyTypeSystem;

		[OneTimeSetUp]
		public void Setup()
		{
			new Application();
			Options.DecompilerSettingsPanel.TestSetup(new Decompiler.DecompilerSettings());
			assemblyList = new AssemblyList("Test");
			testAssembly = assemblyList.OpenAssembly(typeof(MethodUsesAnalyzerTests).Assembly.Location);
			testAssemblyTypeSystem = new SimpleCompilation(testAssembly.GetPEFileOrNull(), assemblyList.OpenAssembly(typeof(void).Assembly.Location).GetPEFileOrNull());
			language = new CSharpLanguage();
		}

		[Test]
		public void SystemInt32UsedByMainAssembly()
		{
			var context = new AnalyzerContext { AssemblyList = assemblyList, Language = language };
			var symbol = testAssemblyTypeSystem.FindType(typeof(int)).GetDefinition();

			var results = new TypeUsedByAnalyzer().Analyze(symbol, context).ToList();

			Assert.IsNotEmpty(results);
			var method = results.OfType<IMethod>().SingleOrDefault(m => m.FullName == "ICSharpCode.ILSpy.Tests.Analyzers.TestCases.Main.MainAssembly.UsesInt32");
			Assert.IsNotNull(method);
			Assert.IsFalse(method.MetadataToken.IsNil);
		}
	}
}
