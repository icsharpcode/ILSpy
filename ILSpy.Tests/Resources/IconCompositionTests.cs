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

using System;
using System.Linq;
using System.Threading.Tasks;

using Avalonia;
using Avalonia.Headless.NUnit;
using Avalonia.Platform;

using AwesomeAssertions;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests;

[TestFixture]
public class IconCompositionTests
{
	[AvaloniaTest]
	public async Task Preload_Parses_Every_Svg_Icon_Off_The_UI_Thread_And_The_Getters_Reuse_It()
	{
		// Startup parses the icon set on a thread-pool thread; the property getters on the UI
		// thread then only wrap what is already parsed. The preload must therefore cover every
		// SVG shipped in the icon folder, and a getter must hand out one stable instance.
		int svgAssets = AssetLoader.GetAssets(new Uri("avares://ILSpy/Assets/Icons/"), null)
			.Count(uri => uri.AbsolutePath.EndsWith(".svg", StringComparison.Ordinal));
		svgAssets.Should().BeGreaterThan(0, "the icon folder is an Avalonia resource");

		await Task.Run(Images.PreloadSvgs);

		Images.ParsedSvgCount.Should().Be(svgAssets, "every SVG icon must be parsed by the preload");
		Images.Assembly.Should().BeSameAs(Images.Assembly, "getters cache their image");
		Images.ResolveByPath("Images/Assembly").Should().BeSameAs(Images.Assembly,
			"metadata icon paths resolve to the same cached image");
	}

	[AvaloniaTest]
	public async Task An_Icon_First_Requested_On_A_Worker_Thread_Still_Renders_On_The_UI_Thread()
	{
		// Tree nodes and search results resolve their icons on decompiler worker threads, so
		// a getter may well run there first. The image it builds must not be owned by that
		// thread, or the UI thread throws when it measures and draws it.
		var icon = await Task.Run(() => Images.Interface);

		icon.Should().BeSameAs(Images.Interface, "the worker's instance is the cached one");
		icon.Size.Should().NotBe(default(Size), "the icon exposes its SVG dimensions");
		Images.RenderToBitmap(icon).Should().NotBeNull("the UI thread can draw it");
	}

	[AvaloniaTest]
	public void Public_Method_Has_No_Overlays_And_Full_Sized_Base()
	{
		// Public members are the unmarked default — no accessibility overlay glyph and no
		// shrinkdown; the base image renders 1:1.

		// Arrange + Act — request a Public Method icon.
		var icon = (LayeredImage)Images.GetIcon(Images.Method, AccessOverlayIcon.Public);

		// Assert — base is the Method bitmap, no overlays, full scale.
		icon.BaseImage.Should().BeSameAs(Images.Method);
		icon.Overlays.Should().BeEmpty();
		icon.BaseScale.Should().Be(1.0);
	}

	[AvaloniaTest]
	public void Private_Method_Composes_Method_Plus_OverlayPrivate_At_80_Percent()
	{
		// Adding an accessibility overlay (Private here) shrinks the base to 80% so the overlay
		// glyph has visual room to sit in the bottom-right corner.

		// Arrange + Act — request a Private Method icon.
		var icon = (LayeredImage)Images.GetIcon(Images.Method, AccessOverlayIcon.Private);

		// Assert — base shrinks to 0.8, exactly one overlay (the Private padlock).
		icon.BaseImage.Should().BeSameAs(Images.Method);
		icon.BaseScale.Should().Be(0.8);
		icon.Overlays.Should().Equal(Images.OverlayPrivate);
	}

	[AvaloniaTest]
	public void Static_Public_Field_Composes_Field_Plus_OverlayStatic_At_80_Percent()
	{
		// `static` adds a Static overlay glyph even when the member is Public — any non-default
		// modifier triggers the 0.8 base shrink.

		// Arrange + Act — request a static Public Field icon.
		var icon = (LayeredImage)Images.GetIcon(Images.Field, AccessOverlayIcon.Public, isStatic: true);

		// Assert — base shrinks to 0.8 with just the Static glyph layered on top.
		icon.BaseImage.Should().BeSameAs(Images.Field);
		icon.BaseScale.Should().Be(0.8);
		icon.Overlays.Should().Equal(Images.OverlayStatic);
	}

	[AvaloniaTest]
	public void Extension_Method_Composes_Method_Plus_OverlayExtension_At_80_Percent()
	{
		// Extension methods get the dedicated Extension overlay even when the member is Public.

		// Arrange + Act — request a Public extension Method icon.
		var icon = (LayeredImage)Images.GetIcon(Images.Method, AccessOverlayIcon.Public, isExtension: true);

		// Assert — base shrinks to 0.8 with the Extension glyph as the lone overlay.
		icon.BaseImage.Should().BeSameAs(Images.Method);
		icon.BaseScale.Should().Be(0.8);
		icon.Overlays.Should().Equal(Images.OverlayExtension);
	}

	[AvaloniaTest]
	public void Static_Private_Method_Layers_Static_Then_Private_Over_Method_At_80_Percent()
	{
		// Order matters: the layering sequence (extension → static → accessibility) determines
		// which glyphs end up on top when corners overlap. A static private method must layer
		// Static under Private so the accessibility glyph remains visible.

		// Arrange + Act — request a static Private Method icon.
		var icon = (LayeredImage)Images.GetIcon(Images.Method, AccessOverlayIcon.Private, isStatic: true);

		// Assert — overlays are Static followed by Private (back-to-front draw order).
		icon.BaseImage.Should().BeSameAs(Images.Method);
		icon.BaseScale.Should().Be(0.8);
		icon.Overlays.Should().Equal(Images.OverlayStatic, Images.OverlayPrivate);
	}

	[AvaloniaTest]
	public void Static_Extension_Method_Layers_Extension_Then_Static_Then_Internal()
	{
		// All three modifiers at once exercises the full layering rule: extension → static →
		// accessibility, in that draw order.

		// Arrange + Act — request an internal static extension Method icon.
		var icon = (LayeredImage)Images.GetIcon(Images.Method, AccessOverlayIcon.Internal,
			isStatic: true, isExtension: true);

		// Assert — three overlays in the documented order.
		icon.BaseImage.Should().BeSameAs(Images.Method);
		icon.BaseScale.Should().Be(0.8);
		icon.Overlays.Should().Equal(Images.OverlayExtension, Images.OverlayStatic, Images.OverlayInternal);
	}

	[AvaloniaTest]
	public void Protected_Event_Composes_Event_Plus_OverlayProtected()
	{
		// Sanity-check that the same composition rules apply across base bitmaps (Event here,
		// not just Method/Field).

		// Arrange + Act — request a Protected Event icon.
		var icon = (LayeredImage)Images.GetIcon(Images.Event, AccessOverlayIcon.Protected);

		// Assert — base shrinks to 0.8 with the Protected glyph as the lone overlay.
		icon.BaseImage.Should().BeSameAs(Images.Event);
		icon.BaseScale.Should().Be(0.8);
		icon.Overlays.Should().Equal(Images.OverlayProtected);
	}

	[AvaloniaTest]
	public void GetIcon_Caches_Same_Composition()
	{
		// Decompiling a large assembly produces hundreds of identically-decorated members;
		// GetIcon must memoise on (base, overlay, isStatic, isExtension) so we don't recreate
		// the same LayeredImage repeatedly.

		// Arrange + Act — request the same composition twice.
		var first = Images.GetIcon(Images.Property, AccessOverlayIcon.Internal);
		var second = Images.GetIcon(Images.Property, AccessOverlayIcon.Internal);

		// Assert — reference equality proves caching.
		first.Should().BeSameAs(second, "GetIcon must memoise by (base, overlay, isStatic, isExtension)");
	}
}
