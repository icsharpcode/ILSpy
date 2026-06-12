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

using System.Collections.Generic;
using System.Linq;

using AwesomeAssertions;

using ICSharpCode.Decompiler.TypeSystem;

using ICSharpCode.ILSpy.Compare;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Compare;

/// <summary>
/// Unit tests for the assembly-comparison engine. The diff logic is pure data — synthetic
/// <see cref="Entry"/> trees, no decompiler required — so each scenario runs in
/// milliseconds.
/// </summary>
[TestFixture]
public class CompareEngineTests
{
	static Entry Leaf(string signature) => new Entry { Signature = signature, Entity = null!, };

	static Entry Branch(string signature, params Entry[] children) => new Entry {
		Signature = signature,
		Entity = null!,
		Children = children.ToList(),
	};

	[Test]
	public void CalculateDiff_Pairs_Identical_Signatures()
	{
		var left = new List<Entry> { Leaf("M1"), Leaf("M2") };
		var right = new List<Entry> { Leaf("M1"), Leaf("M2") };
		var diff = CompareEngine.CalculateDiff(left, right);
		diff.Should().HaveCount(2);
		diff.Should().OnlyContain(p => p.Left != null && p.Right != null);
		diff.All(p => p.Left!.Kind == DiffKind.None).Should().BeTrue();
	}

	[Test]
	public void CalculateDiff_Marks_Left_Only_As_Remove()
	{
		var left = new List<Entry> { Leaf("Stays"), Leaf("Gone") };
		var right = new List<Entry> { Leaf("Stays") };
		var diff = CompareEngine.CalculateDiff(left, right);
		var gone = diff.Single(p => p.Left?.Signature == "Gone");
		((object?)gone.Right).Should().BeNull();
		gone.Left!.Kind.Should().Be(DiffKind.Remove);
	}

	[Test]
	public void CalculateDiff_Marks_Right_Only_As_Add()
	{
		var left = new List<Entry> { Leaf("Stays") };
		var right = new List<Entry> { Leaf("Stays"), Leaf("New") };
		var diff = CompareEngine.CalculateDiff(left, right);
		var added = diff.Single(p => p.Right?.Signature == "New");
		((object?)added.Left).Should().BeNull();
		added.Right!.Kind.Should().Be(DiffKind.Add);
	}

	[Test]
	public void MergeTrees_Aligns_Identical_Subtrees_And_Tags_Diff_On_Children()
	{
		var left = Branch("Type",
			Leaf("Stays"),
			Leaf("Gone"));
		var right = Branch("Type",
			Leaf("Stays"),
			Leaf("New"));
		var merged = CompareEngine.MergeTrees(left, right);
		merged.Children.Should().NotBeNull();
		merged.Children!.Should().HaveCount(3,
			"the merge keeps one slot per distinct signature across both sides");
		merged.Children.Should().Contain(c => c.Signature == "Gone" && c.Kind == DiffKind.Remove);
		merged.Children.Should().Contain(c => c.Signature == "New" && c.Kind == DiffKind.Add);
		merged.Children.Should().Contain(c => c.Signature == "Stays" && c.Kind == DiffKind.None);
	}

	[Test]
	public void RecursiveKind_Summarises_All_Removed_Children_As_Remove()
	{
		// A branch whose every leaf is gone reads as Remove at the branch level —
		// matches WPF's "if every child is the same change kind, propagate it".
		var entry = Branch("T",
			new Entry { Signature = "a", Entity = null!, Kind = DiffKind.Remove },
			new Entry { Signature = "b", Entity = null!, Kind = DiffKind.Remove });
		entry.RecursiveKind.Should().Be(DiffKind.Remove);
	}

	[Test]
	public void RecursiveKind_Mixed_Changes_Becomes_Update()
	{
		var entry = Branch("T",
			new Entry { Signature = "a", Entity = null!, Kind = DiffKind.Add },
			new Entry { Signature = "b", Entity = null!, Kind = DiffKind.Remove });
		entry.RecursiveKind.Should().Be(DiffKind.Update);
	}

	[Test]
	public void RecursiveKind_All_Identical_Children_Is_None()
	{
		var entry = Branch("T",
			new Entry { Signature = "a", Entity = null!, Kind = DiffKind.None },
			new Entry { Signature = "b", Entity = null!, Kind = DiffKind.None });
		entry.RecursiveKind.Should().Be(DiffKind.None);
	}

	[Test]
	public void EntryComparer_Keys_On_Signature_String()
	{
		var left = Leaf("public void Foo()");
		var right = Leaf("public void Foo()");
		EntryComparer.Instance.Equals(left, right).Should().BeTrue();
		EntryComparer.Instance.GetHashCode(left).Should().Be(EntryComparer.Instance.GetHashCode(right));
	}
}
