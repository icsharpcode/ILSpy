// Copyright (c) 2026 Siegfried Pammer
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

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Analyzers;
using ICSharpCode.ILSpyX.Analyzers.Builtin;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Analyzers.Library;

/// <summary>
/// Loading a field's address says only that something needed a reference to it, not what was
/// done through that reference: `flag.ToString()` on a value-type field emits ldflda and writes
/// nothing. Counting it as an assignment put read-only uses under "Assigned By" (issue #2372),
/// so it is reported on its own instead.
/// </summary>
[TestFixture]
public class FieldAccessAnalyzerTests
{
	AssemblyList assemblyList = null!;
	CSharpLanguage language = null!;
	ITypeDefinition typeDefinition = null!;

	[OneTimeSetUp]
	public void Setup()
	{
		assemblyList = new AssemblyList();
		var testAssembly = assemblyList.OpenAssembly(typeof(FieldAccessAnalyzerTests).Assembly.Location);
		assemblyList.OpenAssembly(typeof(void).Assembly.Location);
		language = new CSharpLanguage();
		typeDefinition = testAssembly.GetTypeSystemOrNull()!
			.FindType(typeof(TestCases.Main.FieldAccess))
			.GetDefinition()!;
	}

	string[] Analyze(IAnalyzer analyzer, string fieldName)
	{
		var context = new AnalyzerContext { AssemblyList = assemblyList, Language = language };
		var field = typeDefinition.Fields.Single(f => f.Name == fieldName);
		return analyzer.Analyze(field, context).OfType<IEntity>().Select(e => e.Name).ToArray();
	}

	[TestCase("instanceFlag", "ReadsInstanceFlagByAddress")]
	[TestCase("staticFlag", "ReadsStaticFlagByAddress")]
	public void An_Address_Load_Is_Not_An_Assignment(string fieldName, string addressUser)
	{
		Analyze(new AssignedByFieldAccessAnalyzer(), fieldName)
			.Should().NotContain(addressUser, "taking the address is not a write");
	}

	[TestCase("instanceFlag", "ReadsInstanceFlagByAddress")]
	[TestCase("staticFlag", "ReadsStaticFlagByAddress")]
	public void An_Address_Load_Is_Not_A_Read_Either(string fieldName, string addressUser)
	{
		Analyze(new ReadByFieldAccessAnalyzer(), fieldName)
			.Should().NotContain(addressUser, "what the address is used for is not known here");
	}

	[TestCase("instanceFlag", "ReadsInstanceFlagByAddress")]
	[TestCase("staticFlag", "ReadsStaticFlagByAddress")]
	public void An_Address_Load_Is_Reported_On_Its_Own(string fieldName, string addressUser)
	{
		Analyze(new AddressTakenByFieldAccessAnalyzer(), fieldName)
			.Should().Contain(addressUser);
	}

	[Test]
	public void Plain_Reads_And_Writes_Are_Unaffected()
	{
		Analyze(new ReadByFieldAccessAnalyzer(), "instanceFlag").Should().Contain("ReadsInstanceFlag");
		Analyze(new AssignedByFieldAccessAnalyzer(), "instanceFlag").Should().Contain("WritesInstanceFlag");
		Analyze(new AssignedByFieldAccessAnalyzer(), "staticFlag").Should().Contain("WritesStaticFlag");
		Analyze(new AddressTakenByFieldAccessAnalyzer(), "instanceFlag")
			.Should().NotContain("WritesInstanceFlag", "a plain stfld takes no address");
	}
}
