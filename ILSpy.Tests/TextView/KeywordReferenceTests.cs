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

using System;
using System.Linq;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.OutputVisitor;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.TypeSystem;

using ICSharpCode.ILSpy.TextView;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.TextView;

/// <summary>
/// Pins which keywords the C# output links as navigable references:
/// 'this' and 'base' primary expressions reference the current and base type,
/// and the 'override' modifier references the overridden base member.
/// </summary>
[TestFixture]
public class KeywordReferenceTests
{
	AvaloniaEditTextOutput output = null!;
	string text = null!;

	[OneTimeSetUp]
	public void Setup()
	{
		var fileName = GetType().Assembly.Location;
		var decompiler = new CSharpDecompiler(fileName, new DecompilerSettings());
		var syntaxTree = decompiler.DecompileType(new FullTypeName(typeof(KeywordDerived).FullName!));
		syntaxTree.AcceptVisitor(new InsertParenthesesVisitor { InsertParenthesesForReadability = true });
		var settings = new DecompilerSettings();
		output = new AvaloniaEditTextOutput();
		var tokenWriter = new ICSharpCode.Decompiler.TextTokenWriter(output, settings);
		syntaxTree.AcceptVisitor(new CSharpOutputVisitor(tokenWriter, settings.CSharpFormattingOptions));
		text = output.GetText();
	}

	[Test]
	public void BaseKeywordReferencesTheBaseType()
	{
		var reference = ReferencesFor("base").Select(r => r.Reference).OfType<IType>().FirstOrDefault();
		Assert.That(reference, Is.Not.Null, "the 'base' keyword must carry a type reference");
		Assert.That(reference!.Name, Is.EqualTo(nameof(KeywordBase)));
	}

	[Test]
	public void ThisKeywordReferencesTheCurrentType()
	{
		var reference = ReferencesFor("this").Select(r => r.Reference).OfType<IType>().FirstOrDefault();
		Assert.That(reference, Is.Not.Null, "the 'this' keyword must carry a type reference");
		Assert.That(reference!.Name, Is.EqualTo(nameof(KeywordDerived)));
	}

	[Test]
	public void OverrideModifierReferencesTheOverriddenMember()
	{
		var reference = ReferencesFor("override").Select(r => r.Reference).OfType<IMember>().FirstOrDefault();
		Assert.That(reference, Is.Not.Null, "the 'override' modifier must carry a member reference");
		Assert.That(reference!.DeclaringTypeDefinition!.Name, Is.EqualTo(nameof(KeywordBase)));
	}

	System.Collections.Generic.IEnumerable<ReferenceSegment> ReferencesFor(string keyword)
	{
		return output.References.Where(r => text.Substring(r.StartOffset, r.Length) == keyword);
	}
}

public class KeywordBase
{
	public virtual void TestMethod() => throw new NotImplementedException();
}

public class KeywordDerived : KeywordBase
{
	public int TestField;

	public override void TestMethod()
	{
		base.TestMethod();
	}

	public void Assign(int TestField)
	{
		this.TestField = TestField;
	}
}
