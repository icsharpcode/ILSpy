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

using AwesomeAssertions;

using ICSharpCode.ILSpyX.Search;

using ICSharpCode.ILSpy.Search;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Search;

/// <summary>
/// Unit tests for the prefix-DSL parser exposed by <see cref="RunningSearch.ParseInput"/>.
/// The parser is the data-input side of the search pipeline — it converts the user's
/// freeform query string into a <see cref="SearchRequest"/> whose Mode / MemberSearchKind /
/// InAssembly / InNamespace / Keywords / RegEx / FullNameSearch / OmitGenerics drive the
/// strategy. Tests pin the prefixes advertised by the SearchPane watermark.
/// </summary>
[TestFixture]
public class SearchPrefixParsingTests
{
	static SearchRequest Parse(string input, SearchMode defaultMode = SearchMode.TypeAndMember)
		=> RunningSearch.ParseInput(input, defaultMode);

	[Test]
	public void Plain_Term_Falls_Through_To_The_Picker_Mode()
	{
		var r = Parse("Enumerable");
		r.Mode.Should().Be(SearchMode.TypeAndMember);
		r.Keywords.Should().BeEquivalentTo("Enumerable");
	}

	[Test]
	public void T_Prefix_Switches_To_Type_Mode()
	{
		var r = Parse("t:String");
		r.Mode.Should().Be(SearchMode.Type);
		r.Keywords.Should().BeEquivalentTo("String");
	}

	[Test]
	public void M_Prefix_Switches_To_Member_Mode()
	{
		var r = Parse("m:ToString");
		r.Mode.Should().Be(SearchMode.Member);
		r.Keywords.Should().BeEquivalentTo("ToString");
	}

	[Test]
	public void Md_Prefix_Switches_To_Method_Mode()
	{
		var r = Parse("md:Run");
		r.Mode.Should().Be(SearchMode.Method);
		r.Keywords.Should().BeEquivalentTo("Run");
	}

	[Test]
	public void F_Prefix_Switches_To_Field_Mode()
	{
		var r = Parse("f:value");
		r.Mode.Should().Be(SearchMode.Field);
		r.Keywords.Should().BeEquivalentTo("value");
	}

	[Test]
	public void P_Prefix_Switches_To_Property_Mode()
	{
		var r = Parse("p:Length");
		r.Mode.Should().Be(SearchMode.Property);
		r.Keywords.Should().BeEquivalentTo("Length");
	}

	[Test]
	public void E_Prefix_Switches_To_Event_Mode()
	{
		var r = Parse("e:Click");
		r.Mode.Should().Be(SearchMode.Event);
		r.Keywords.Should().BeEquivalentTo("Click");
	}

	[Test]
	public void C_Prefix_Switches_To_Literal_Mode()
	{
		var r = Parse("c:42");
		r.Mode.Should().Be(SearchMode.Literal);
		r.Keywords.Should().BeEquivalentTo("42");
	}

	[Test]
	public void N_Prefix_Switches_To_Namespace_Mode()
	{
		var r = Parse("n:System");
		r.Mode.Should().Be(SearchMode.Namespace);
		r.Keywords.Should().BeEquivalentTo("System");
	}

	[Test]
	public void A_Prefix_Switches_To_Assembly_Mode_With_NameOrFileName_Kind()
	{
		var r = Parse("a:mscorlib");
		r.Mode.Should().Be(SearchMode.Assembly);
		r.AssemblySearchKind.Should().Be(AssemblySearchKind.NameOrFileName);
		r.Keywords.Should().BeEquivalentTo("mscorlib");
	}

	[Test]
	public void Af_Prefix_Switches_To_Assembly_Mode_With_FilePath_Kind()
	{
		var r = Parse("af:foo.dll");
		r.Mode.Should().Be(SearchMode.Assembly);
		r.AssemblySearchKind.Should().Be(AssemblySearchKind.FilePath);
		r.Keywords.Should().BeEquivalentTo("foo.dll");
	}

	[Test]
	public void An_Prefix_Switches_To_Assembly_Mode_With_FullName_Kind()
	{
		var r = Parse("an:System.Linq");
		r.Mode.Should().Be(SearchMode.Assembly);
		r.AssemblySearchKind.Should().Be(AssemblySearchKind.FullName);
		r.Keywords.Should().BeEquivalentTo("System.Linq");
	}

	[Test]
	public void At_Prefix_Switches_To_Token_Mode()
	{
		// @ token prefix is special: single character, no colon.
		var r = Parse("@0x06000001");
		r.Mode.Should().Be(SearchMode.Token);
		r.Keywords.Should().BeEquivalentTo("0x06000001");
	}

	[Test]
	public void InAssembly_Prefix_Sets_The_Filter()
	{
		// The scope value is a filter only; it must not become a keyword, or every
		// entity name would have to contain the assembly name to match.
		var r = Parse("inassembly:mscorlib Foo");
		r.InAssembly.Should().Be("mscorlib");
		r.Keywords.Should().BeEquivalentTo("Foo");
	}

	[Test]
	public void InNamespace_Prefix_Sets_The_Filter()
	{
		var r = Parse("innamespace:System.Linq Enumerable");
		r.InNamespace.Should().Be("System.Linq");
		r.Keywords.Should().BeEquivalentTo("Enumerable");
		// The dot in the scope value is not a request for full-name matching.
		r.FullNameSearch.Should().BeFalse();
	}

	[Test]
	public void InAssembly_Value_Never_Leaks_Into_Keywords()
	{
		// Regression test for #4246: an assembly whose name is not a prefix of its
		// namespaces (Unity's Assembly-CSharp, global namespace) found nothing because the
		// scope value was matched against every entity name as a keyword.
		var r = Parse("inassembly:Assembly-CSharp Chip");
		r.InAssembly.Should().Be("Assembly-CSharp");
		r.Keywords.Should().BeEquivalentTo("Chip");
		r.FullNameSearch.Should().BeFalse();
	}

	[Test]
	public void Lone_InAssembly_Prefix_Yields_Zero_Keywords()
	{
		// A scope filter without a search term is not a search: the strategy factory
		// returns null for an empty keyword list, matching the WPF-era behaviour.
		var r = Parse("inassembly:Foo");
		r.InAssembly.Should().Be("Foo");
		r.Keywords.Should().BeEmpty();
	}

	[Test]
	public void Regex_Term_Wrapped_In_Slashes_Populates_RegEx()
	{
		var r = Parse("/^Enumerable$/");
		r.RegEx.Should().NotBeNull();
		r.RegEx!.IsMatch("Enumerable").Should().BeTrue();
		r.RegEx.IsMatch("EnumerableExtensions").Should().BeFalse();
	}

	[Test]
	public void FullNameSearch_Enabled_When_Term_Contains_Dot()
	{
		// A term like System.Linq.Enumerable signals the user wants a full-name match.
		var r = Parse("System.Linq.Enumerable");
		r.FullNameSearch.Should().BeTrue();
	}

	[Test]
	public void FullNameSearch_Off_For_Plain_Identifier()
	{
		var r = Parse("Enumerable");
		r.FullNameSearch.Should().BeFalse();
	}

	[Test]
	public void OmitGenerics_True_When_Term_Has_No_Angle_Or_Backtick()
	{
		var r = Parse("List");
		r.OmitGenerics.Should().BeTrue();
	}

	[Test]
	public void OmitGenerics_False_When_Term_Has_Generic_Angle()
	{
		var r = Parse("List<int>");
		r.OmitGenerics.Should().BeFalse();
	}

	[Test]
	public void Empty_Input_Yields_Zero_Keywords()
	{
		var r = Parse("   ");
		r.Keywords.Should().BeEmpty();
	}

	[Test]
	public void Quoted_Term_Survives_Whitespace_Split()
	{
		var r = Parse("\"hello world\"");
		r.Keywords.Should().BeEquivalentTo("hello world");
	}

	[Test]
	public void Match_Operators_Are_Passed_Through_To_Keywords()
	{
		// =, +, -, ~ are per-keyword prefixes interpreted by AbstractSearchStrategy.IsMatch
		// — the parser only has to deliver them in the keyword string untouched.
		var r = Parse("=Foo +Bar -Baz");
		r.Keywords.Should().BeEquivalentTo("=Foo", "+Bar", "-Baz");
	}
}
