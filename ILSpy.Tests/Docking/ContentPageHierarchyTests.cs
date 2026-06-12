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

using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

/// <summary>
/// Locks the single inner-content hierarchy: every viewmodel that may occupy a
/// <see cref="ContentTabPage"/>'s Content slot derives from the one <see cref="ContentPageModel"/>
/// base, and <c>ContentTabPage.Content</c> is typed to it (not <c>object</c>). Guards against a
/// future content type sneaking back in as a bare ObservableObject or the slot weakening to object.
/// </summary>
[TestFixture]
public class ContentPageHierarchyTests
{
	[Test]
	public void All_Four_Content_Types_Derive_From_ContentPageModel()
	{
		typeof(ICSharpCode.ILSpy.TextView.DecompilerTabPageModel).Should().BeAssignableTo<ContentPageModel>();
		typeof(MetadataTablePageModel).Should().BeAssignableTo<ContentPageModel>();
		typeof(ICSharpCode.ILSpy.Compare.CompareTabPageModel).Should().BeAssignableTo<ContentPageModel>();
		typeof(ICSharpCode.ILSpy.Options.OptionsPageModel).Should().BeAssignableTo<ContentPageModel>(
			"OptionsPageModel must join the content hierarchy, not stay a bare ObservableObject");
	}

	[Test]
	public void ContentTabPage_Content_Is_Typed_To_ContentPageModel()
	{
		typeof(ContentTabPage).GetProperty(nameof(ContentTabPage.Content))!.PropertyType
			.Should().Be(typeof(ContentPageModel), "the Content slot must be strongly typed, not object");
	}

	[Test]
	public void IsStaticContent_Is_A_Single_Inherited_Member()
	{
		// One IsStaticContent, declared on the base -- not duck-typed across unrelated classes.
		typeof(ContentPageModel).GetProperty(nameof(ContentPageModel.IsStaticContent))
			.Should().NotBeNull("IsStaticContent lives on ContentPageModel");
		typeof(ICSharpCode.ILSpy.Options.OptionsPageModel).GetProperty("IsStaticContent")!.DeclaringType
			.Should().Be(typeof(ContentPageModel), "OptionsPageModel must inherit IsStaticContent, not redeclare it");
	}
}
