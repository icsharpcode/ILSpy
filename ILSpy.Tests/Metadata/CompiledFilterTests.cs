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

using System.Collections.Immutable;
using System.Reflection;

using AwesomeAssertions;

using ICSharpCode.ILSpy.Metadata.Filters;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class CompiledFilterTests
{
	static FilterState NewState() => new(FlagsSchemaInferer.For(typeof(TypeAttributes)));

	static CompiledFilter Compile(FilterState state) => CompiledFilter.Compile(state);

	[Test]
	public void Empty_State_Compiles_To_The_Empty_Filter_Which_Matches_Every_Row()
	{
		var compiled = Compile(NewState());
		compiled.IsEmpty.Should().BeTrue();
		compiled.Matches((uint)TypeAttributes.Public).Should().BeTrue();
		compiled.Matches(0u).Should().BeTrue();
		compiled.Matches(0xFFFFFFFFu).Should().BeTrue();
	}

	[Test]
	public void Mutex_Selection_Of_One_Value_Restricts_That_Axis_Only()
	{
		// Pick "Public" (0x01) on the Visibility axis. Every row must have exactly that
		// visibility; other axes (Layout, ClassSemantics, …) stay unconstrained.
		var state = NewState();
		state.SetMutexSelection("Visibility", ImmutableHashSet.Create((uint)TypeAttributes.Public));
		var compiled = Compile(state);

		compiled.Matches((uint)TypeAttributes.Public).Should().BeTrue();
		compiled.Matches((uint)(TypeAttributes.Public | TypeAttributes.Sealed)).Should().BeTrue();
		compiled.Matches((uint)(TypeAttributes.Public | TypeAttributes.SequentialLayout)).Should().BeTrue();

		compiled.Matches((uint)TypeAttributes.NotPublic).Should().BeFalse();
		compiled.Matches((uint)TypeAttributes.NestedPublic).Should().BeFalse();
	}

	[Test]
	public void Multiple_Mutex_Values_OR_Within_A_Group()
	{
		// Visibility ∈ {Public, NestedPublic}. Within a group the user picks an
		// inclusive set of values; across rows any of them passes.
		var state = NewState();
		state.SetMutexSelection("Visibility", ImmutableHashSet.Create(
			(uint)TypeAttributes.Public,
			(uint)TypeAttributes.NestedPublic));
		var compiled = Compile(state);

		compiled.Matches((uint)TypeAttributes.Public).Should().BeTrue();
		compiled.Matches((uint)TypeAttributes.NestedPublic).Should().BeTrue();
		compiled.Matches((uint)TypeAttributes.NestedPrivate).Should().BeFalse();
		compiled.Matches((uint)TypeAttributes.NotPublic).Should().BeFalse();
	}

	[Test]
	public void Empty_Mutex_Selection_Hides_Every_Row_For_That_Axis()
	{
		// User unticks every chip including (none) → ImmutableHashSet.Empty. The axis
		// has no allowed values, so every row fails. Matches WPF's Mask=0 behaviour.
		var state = NewState();
		state.SetMutexSelection("Visibility", ImmutableHashSet<uint>.Empty);
		var compiled = Compile(state);

		compiled.Matches((uint)TypeAttributes.Public).Should().BeFalse();
		compiled.Matches((uint)TypeAttributes.NotPublic).Should().BeFalse();
	}

	[Test]
	public void Required_Independent_Flag_Narrows_To_Rows_Where_That_Bit_Is_Set()
	{
		var state = NewState();
		state.SetFlagState(nameof(TypeAttributes.Sealed), TriState.Required);
		var compiled = Compile(state);

		compiled.Matches((uint)TypeAttributes.Sealed).Should().BeTrue();
		compiled.Matches((uint)(TypeAttributes.Sealed | TypeAttributes.Public)).Should().BeTrue();
		compiled.Matches((uint)TypeAttributes.Public).Should().BeFalse();
	}

	[Test]
	public void Excluded_Independent_Flag_Hides_Rows_Where_That_Bit_Is_Set()
	{
		// "Excluded is always strict" — even when IndependentMode is Any, an excluded
		// bit set rejects the row.
		var state = NewState();
		state.SetFlagState(nameof(TypeAttributes.Abstract), TriState.Excluded);
		var compiled = Compile(state);

		compiled.Matches((uint)TypeAttributes.Public).Should().BeTrue();
		compiled.Matches((uint)(TypeAttributes.Public | TypeAttributes.Sealed)).Should().BeTrue();
		compiled.Matches((uint)(TypeAttributes.Public | TypeAttributes.Abstract)).Should().BeFalse();
	}

	[Test]
	public void Required_Independent_Mode_All_Requires_Every_Required_Bit()
	{
		var state = NewState();
		state.SetFlagState(nameof(TypeAttributes.Sealed), TriState.Required);
		state.SetFlagState(nameof(TypeAttributes.SpecialName), TriState.Required);
		state.IndependentMode = MatchMode.All;
		var compiled = Compile(state);

		compiled.Matches((uint)(TypeAttributes.Sealed | TypeAttributes.SpecialName)).Should().BeTrue();
		compiled.Matches((uint)TypeAttributes.Sealed).Should().BeFalse();
		compiled.Matches((uint)TypeAttributes.SpecialName).Should().BeFalse();
	}

	[Test]
	public void Required_Independent_Mode_Any_Requires_At_Least_One_Required_Bit()
	{
		var state = NewState();
		state.SetFlagState(nameof(TypeAttributes.Sealed), TriState.Required);
		state.SetFlagState(nameof(TypeAttributes.SpecialName), TriState.Required);
		state.IndependentMode = MatchMode.Any;
		var compiled = Compile(state);

		compiled.Matches((uint)(TypeAttributes.Sealed | TypeAttributes.SpecialName)).Should().BeTrue();
		compiled.Matches((uint)TypeAttributes.Sealed).Should().BeTrue();
		compiled.Matches((uint)TypeAttributes.SpecialName).Should().BeTrue();
		compiled.Matches((uint)TypeAttributes.Public).Should().BeFalse();
	}

	[Test]
	public void Excluded_Wins_Even_When_Mode_Is_Any()
	{
		// MatchMode.Any only relaxes the *required* path; excluded stays strict.
		var state = NewState();
		state.SetFlagState(nameof(TypeAttributes.Sealed), TriState.Required);
		state.SetFlagState(nameof(TypeAttributes.Abstract), TriState.Excluded);
		state.IndependentMode = MatchMode.Any;
		var compiled = Compile(state);

		compiled.Matches((uint)TypeAttributes.Sealed).Should().BeTrue();
		compiled.Matches((uint)(TypeAttributes.Sealed | TypeAttributes.Abstract)).Should().BeFalse(
			"Abstract is excluded — mode=Any doesn't unlock excluded bits");
	}

	[Test]
	public void Mutex_And_Independent_AND_Together_Across_Groups()
	{
		// "Public types that are sealed and not abstract" — the canonical compound query.
		var state = NewState();
		state.SetMutexSelection("Visibility", ImmutableHashSet.Create((uint)TypeAttributes.Public));
		state.SetFlagState(nameof(TypeAttributes.Sealed), TriState.Required);
		state.SetFlagState(nameof(TypeAttributes.Abstract), TriState.Excluded);
		var compiled = Compile(state);

		compiled.Matches((uint)(TypeAttributes.Public | TypeAttributes.Sealed)).Should().BeTrue();
		compiled.Matches((uint)(TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Abstract))
			.Should().BeFalse();
		compiled.Matches((uint)TypeAttributes.Public).Should().BeFalse();
		compiled.Matches((uint)(TypeAttributes.NotPublic | TypeAttributes.Sealed)).Should().BeFalse();
	}

	[Test]
	public void Wide_Mutex_Mask_Falls_Back_To_HashSet_When_Bit_Range_Exceeds_64()
	{
		// Synthetic enum whose mask spans more than 6 bits when normalised. The compiler
		// switches to ImmutableHashSet<uint> instead of a 64-bit bitmap.
		var schema = FlagsSchemaInferer.For(typeof(WideMaskEnum));
		var state = new FilterState(schema);
		state.SetMutexSelection("Code", ImmutableHashSet.Create(0x80u, 0x7Fu));
		var compiled = Compile(state);

		compiled.MutexChecks.Should().HaveCount(1);
		compiled.MutexChecks[0].AllowedSet.Should().NotBeNull();
		compiled.Matches(0x80).Should().BeTrue();
		compiled.Matches(0x7F).Should().BeTrue();
		compiled.Matches(0x40).Should().BeFalse();
	}

	[System.Flags]
	enum WideMaskEnum : uint
	{
		// Mask is 0xFF — 8 raw bits, 256 possible values normalised. Compiler must use
		// the AllowedSet path because 256 > 64.
		CodeMask = 0xFF,
		Zero = 0x00,
		Lower = 0x40,
		Mid = 0x7F,
		Upper = 0x80,
	}
}
