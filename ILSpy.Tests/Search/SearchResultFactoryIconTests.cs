// Copyright (c) 2026 Christoph Wille
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

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.Search;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Search;

// Search results must carry the same composed icons as the assembly tree: base symbol by
// entity kind, plus accessibility and static overlays. The factory therefore has to go
// through Images.GetIcon (via the tree nodes' GetIcon helpers), not hand out bare base
// images — a bare Images.Field for a private static field loses both mini-overlays.
[TestFixture]
public class SearchResultFactoryIconTests
{
	AvaloniaSearchResultFactory factory = null!;
	ITypeDefinition fixtureType = null!;

	[OneTimeSetUp]
	public void LoadFixtureTypeFromOwnAssembly()
	{
		// Read the fixture entities from this test assembly's own metadata, so the
		// accessibility/static flags come from a real type system, not mocks.
		var path = typeof(SearchResultFactoryIconTests).Assembly.Location;
		var file = new PEFile(path);
		var resolver = new UniversalAssemblyResolver(path, throwOnError: false,
			targetFramework: file.DetectTargetFrameworkId());
		var typeSystem = new DecompilerTypeSystem(file, resolver);
		fixtureType = typeSystem.MainModule.TypeDefinitions
			.Single(t => t.Name == nameof(SearchIconFixture));
		factory = new AvaloniaSearchResultFactory(new CSharpLanguage());
	}

	[AvaloniaTest]
	public void Private_Static_Field_Result_Composes_Static_And_Private_Overlays()
	{
		var field = fixtureType.Fields.Single(f => f.Name == "privateStaticField");

		var result = factory.Create(field);

		var icon = result.Image.Should().BeOfType<LayeredImage>().Subject;
		icon.BaseImage.Should().BeSameAs(Images.Field);
		icon.BaseScale.Should().Be(0.8);
		icon.Overlays.Should().Equal(Images.OverlayStatic, Images.OverlayPrivate);
	}

	[AvaloniaTest]
	public void Nested_Interface_Result_Uses_Interface_Base_Icon()
	{
		var nested = fixtureType.NestedTypes
			.Single(t => t.Name == nameof(SearchIconFixture.INested)).GetDefinition()!;

		var icon = (LayeredImage)factory.Create(nested).Image;

		icon.BaseImage.Should().BeSameAs(Images.Interface);
	}

	[AvaloniaTest]
	public void Nested_Enum_Result_Uses_Enum_Base_Icon()
	{
		var nested = fixtureType.NestedTypes
			.Single(t => t.Name == "NestedEnum").GetDefinition()!;

		var icon = (LayeredImage)factory.Create(nested).Image;

		icon.BaseImage.Should().BeSameAs(Images.Enum);
	}

	[AvaloniaTest]
	public void Location_Image_Reflects_Declaring_Type_Icon()
	{
		var field = fixtureType.Fields.Single(f => f.Name == "privateStaticField");

		var result = factory.Create(field);

		// The declaring type (SearchIconFixture) is a top-level internal class.
		var location = result.LocationImage.Should().BeOfType<LayeredImage>().Subject;
		location.BaseImage.Should().BeSameAs(Images.Class);
		location.Overlays.Should().Equal(Images.OverlayInternal);
	}
}

#pragma warning disable CS0169 // fields exist only as metadata probes for the tests above
class SearchIconFixture
{
	static int privateStaticField;

	internal interface INested { }

	enum NestedEnum { None }
}
#pragma warning restore CS0169
