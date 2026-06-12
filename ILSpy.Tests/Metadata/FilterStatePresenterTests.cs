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
public class FilterStatePresenterTests
{
	static FilterState NewState() => new(FlagsSchemaInferer.For(typeof(TypeAttributes)));

	[Test]
	public void Empty_State_Renders_As_Any()
	{
		FilterStatePresenter.Describe(NewState()).Should().Be("(any)");
	}

	[Test]
	public void Single_Mutex_Choice_Renders_With_Equals_Sign()
	{
		var state = NewState();
		state.SetMutexSelection("Visibility", ImmutableHashSet.Create((uint)TypeAttributes.Public));
		FilterStatePresenter.Describe(state).Should().Be("Visibility = Public");
	}

	[Test]
	public void Multiple_Mutex_Values_Render_As_Set_Membership()
	{
		var state = NewState();
		state.SetMutexSelection("Visibility", ImmutableHashSet.Create(
			(uint)TypeAttributes.Public,
			(uint)TypeAttributes.NestedPublic));
		FilterStatePresenter.Describe(state).Should().Be("Visibility ∈ {Public, NestedPublic}");
	}

	[Test]
	public void Required_And_Excluded_Flags_Render_As_AND_Of_Atoms()
	{
		var state = NewState();
		state.SetFlagState(nameof(TypeAttributes.Sealed), TriState.Required);
		state.SetFlagState(nameof(TypeAttributes.Abstract), TriState.Excluded);
		FilterStatePresenter.Describe(state).Should().Be("Sealed ∧ ¬Abstract");
	}

	[Test]
	public void Compound_Filter_Renders_All_Clauses_Joined_By_AND()
	{
		var state = NewState();
		state.SetMutexSelection("Visibility", ImmutableHashSet.Create((uint)TypeAttributes.Public));
		state.SetFlagState(nameof(TypeAttributes.Sealed), TriState.Required);
		state.SetFlagState(nameof(TypeAttributes.Abstract), TriState.Excluded);
		FilterStatePresenter.Describe(state).Should().Be("Visibility = Public ∧ Sealed ∧ ¬Abstract");
	}

	[Test]
	public void Multiple_Required_Flags_Use_Group_Mode_Operator()
	{
		var state = NewState();
		state.SetFlagState(nameof(TypeAttributes.Sealed), TriState.Required);
		state.SetFlagState(nameof(TypeAttributes.SpecialName), TriState.Required);
		state.IndependentMode = MatchMode.All;
		FilterStatePresenter.Describe(state).Should().Be("(Sealed ∧ SpecialName)");

		state.IndependentMode = MatchMode.Any;
		FilterStatePresenter.Describe(state).Should().Be("(Sealed ∨ SpecialName)");
	}

	[Test]
	public void Empty_Mutex_Set_Renders_With_Empty_Set_Symbol()
	{
		var state = NewState();
		state.SetMutexSelection("Visibility", ImmutableHashSet<uint>.Empty);
		FilterStatePresenter.Describe(state).Should().Be("Visibility ∈ ∅");
	}
}
