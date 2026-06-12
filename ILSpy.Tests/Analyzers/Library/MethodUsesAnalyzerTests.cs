// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using System.Linq;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Analyzers;
using ICSharpCode.ILSpyX.Analyzers.Builtin;

using ICSharpCode.ILSpy.Languages;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers.Library;

[TestFixture]
public class MethodUsesAnalyzerTests
{
	AssemblyList assemblyList = null!;
	CSharpLanguage language = null!;
	ICompilation testAssemblyTypeSystem = null!;
	ITypeDefinition typeDefinition = null!;

	[OneTimeSetUp]
	public void Setup()
	{
		assemblyList = new AssemblyList();
		var testAssembly = assemblyList.OpenAssembly(typeof(MethodUsesAnalyzerTests).Assembly.Location);
		assemblyList.OpenAssembly(typeof(void).Assembly.Location);
		testAssemblyTypeSystem = testAssembly.GetTypeSystemOrNull()!;
		language = new CSharpLanguage();
		typeDefinition = testAssemblyTypeSystem
			.FindType(typeof(TestCases.Main.MainAssembly))
			.GetDefinition()!;
	}

	[Test]
	public void MainAssemblyUsesSystemStringEmpty()
	{
		var context = new AnalyzerContext { AssemblyList = assemblyList, Language = language };
		var symbol = typeDefinition.Methods.First(m => m.Name == "UsesSystemStringEmpty");

		var results = new MethodUsesAnalyzer().Analyze(symbol, context).ToList();

		Assert.That(results.Count == 1);
		var field = results.Single() as IField;
		Assert.That(field, Is.Not.Null);
		Assert.That(!field!.MetadataToken.IsNil);
		Assert.That("System.String.Empty", Is.EqualTo(field.FullName));
	}
}
