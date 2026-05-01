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

using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ILSpy.Images;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class IconCompositionTests
{
	[AvaloniaTest]
	public void Public_Method_Has_No_Overlays_And_Full_Sized_Base()
	{
		var icon = (LayeredImage)Images.GetIcon(Images.Method, AccessOverlayIcon.Public);
		icon.BaseImage.Should().BeSameAs(Images.Method);
		icon.Overlays.Should().BeEmpty();
		icon.BaseScale.Should().Be(1.0);
	}

	[AvaloniaTest]
	public void Private_Method_Composes_Method_Plus_OverlayPrivate_At_80_Percent()
	{
		var icon = (LayeredImage)Images.GetIcon(Images.Method, AccessOverlayIcon.Private);
		icon.BaseImage.Should().BeSameAs(Images.Method);
		icon.BaseScale.Should().Be(0.8);
		icon.Overlays.Should().Equal(Images.OverlayPrivate);
	}

	[AvaloniaTest]
	public void Static_Public_Field_Composes_Field_Plus_OverlayStatic_At_80_Percent()
	{
		var icon = (LayeredImage)Images.GetIcon(Images.Field, AccessOverlayIcon.Public, isStatic: true);
		icon.BaseImage.Should().BeSameAs(Images.Field);
		icon.BaseScale.Should().Be(0.8);
		icon.Overlays.Should().Equal(Images.OverlayStatic);
	}

	[AvaloniaTest]
	public void Extension_Method_Composes_Method_Plus_OverlayExtension_At_80_Percent()
	{
		var icon = (LayeredImage)Images.GetIcon(Images.Method, AccessOverlayIcon.Public, isExtension: true);
		icon.BaseImage.Should().BeSameAs(Images.Method);
		icon.BaseScale.Should().Be(0.8);
		icon.Overlays.Should().Equal(Images.OverlayExtension);
	}

	[AvaloniaTest]
	public void Static_Private_Method_Layers_Static_Then_Private_Over_Method_At_80_Percent()
	{
		// Order matches the WPF host's draw sequence: extension → static → accessibility.
		var icon = (LayeredImage)Images.GetIcon(Images.Method, AccessOverlayIcon.Private, isStatic: true);
		icon.BaseImage.Should().BeSameAs(Images.Method);
		icon.BaseScale.Should().Be(0.8);
		icon.Overlays.Should().Equal(Images.OverlayStatic, Images.OverlayPrivate);
	}

	[AvaloniaTest]
	public void Static_Extension_Method_Layers_Extension_Then_Static_Then_Internal()
	{
		var icon = (LayeredImage)Images.GetIcon(Images.Method, AccessOverlayIcon.Internal,
			isStatic: true, isExtension: true);
		icon.BaseImage.Should().BeSameAs(Images.Method);
		icon.BaseScale.Should().Be(0.8);
		icon.Overlays.Should().Equal(Images.OverlayExtension, Images.OverlayStatic, Images.OverlayInternal);
	}

	[AvaloniaTest]
	public void Protected_Event_Composes_Event_Plus_OverlayProtected()
	{
		var icon = (LayeredImage)Images.GetIcon(Images.Event, AccessOverlayIcon.Protected);
		icon.BaseImage.Should().BeSameAs(Images.Event);
		icon.BaseScale.Should().Be(0.8);
		icon.Overlays.Should().Equal(Images.OverlayProtected);
	}

	[AvaloniaTest]
	public void GetIcon_Caches_Same_Composition()
	{
		var first = Images.GetIcon(Images.Property, AccessOverlayIcon.Internal);
		var second = Images.GetIcon(Images.Property, AccessOverlayIcon.Internal);
		first.Should().BeSameAs(second, "GetIcon must memoise by (base, overlay, isStatic, isExtension)");
	}
}
