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
using System.Collections.Generic;

using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Svg.Skia;

using ICSharpCode.Decompiler.TypeSystem;

namespace ILSpy.Images
{
	public enum AccessOverlayIcon
	{
		Public,
		Protected,
		Internal,
		ProtectedInternal,
		Private,
		PrivateProtected,
		CompilerControlled,
	}

	public static class Images
	{
		const string AssetBase = "avares://ILSpy/Assets/Icons/";

		static IImage LoadSvg(string name)
		{
			return new SvgImage {
				Source = SvgSource.Load(AssetBase + name + ".svg", null)
			};
		}

		static IImage LoadPng(string name)
		{
			using var stream = AssetLoader.Open(new Uri(AssetBase + name + ".png"));
			return new Bitmap(stream);
		}

		// Assemblies / packages
		public static readonly IImage Assembly = LoadSvg(nameof(Assembly));
		public static readonly IImage AssemblyLoading = LoadSvg(nameof(AssemblyLoading));
		public static readonly IImage AssemblyWarning = LoadSvg(nameof(AssemblyWarning));
		public static readonly IImage Warning = LoadSvg(nameof(Warning));
		public static readonly IImage FindAssembly = LoadSvg(nameof(FindAssembly));
		public static readonly IImage Library = LoadSvg(nameof(Library));
		public static readonly IImage NuGet = LoadPng(nameof(NuGet));
		public static readonly IImage MetadataFile = LoadSvg(nameof(MetadataFile));
		public static readonly IImage WebAssemblyFile = LoadSvg("WebAssembly");
		public static readonly IImage ProgramDebugDatabase = LoadSvg(nameof(ProgramDebugDatabase));

		// Toolbar / navigation
		public static readonly IImage Back = LoadSvg(nameof(Back));
		public static readonly IImage Forward = LoadSvg(nameof(Forward));
		public static readonly IImage Open = LoadSvg(nameof(Open));
		public static readonly IImage Save = LoadSvg(nameof(Save));
		public static readonly IImage ShowPublicOnly = LoadSvg(nameof(ShowPublicOnly));
		public static readonly IImage ShowPrivateInternal = LoadSvg(nameof(ShowPrivateInternal));
		public static readonly IImage ShowAll = LoadSvg(nameof(ShowAll));

		// Containers
		public static readonly IImage Namespace = LoadSvg(nameof(Namespace));
		public static readonly IImage ReferenceFolder = LoadSvg(nameof(ReferenceFolder));
		public static readonly IImage MetadataTable = LoadSvg(nameof(MetadataTable));
		public static readonly IImage FolderClosed = LoadSvg(nameof(FolderClosed));
		public static readonly IImage FolderOpen = LoadSvg(nameof(FolderOpen));
		public static readonly IImage Resource = LoadSvg(nameof(Resource));
		public static readonly IImage ResourceResourcesFile = LoadSvg(nameof(ResourceResourcesFile));

		// Types
		public static readonly IImage Class = LoadSvg(nameof(Class));
		public static readonly IImage Interface = LoadSvg(nameof(Interface));
		public static readonly IImage Struct = LoadSvg(nameof(Struct));
		public static readonly IImage Enum = LoadSvg(nameof(Enum));
		public static readonly IImage Delegate = LoadSvg(nameof(Delegate));

		// Methods
		public static readonly IImage Method = LoadSvg(nameof(Method));
		public static readonly IImage Constructor = LoadSvg(nameof(Constructor));
		public static readonly IImage Operator = LoadSvg(nameof(Operator));

		// Fields / properties / events
		public static readonly IImage Field = LoadSvg(nameof(Field));
		public static readonly IImage Property = LoadSvg(nameof(Property));
		public static readonly IImage Event = LoadSvg(nameof(Event));

		// Reference overlays (small badge layered onto a base icon for TypeRef/MemberRef nodes).
		internal static readonly IImage ReferenceOverlay = LoadSvg(nameof(ReferenceOverlay));
		internal static readonly IImage ExportOverlay = LoadSvg(nameof(ExportOverlay));

		public static readonly IImage TypeReference = new LayeredImage(Class, ReferenceOverlay);
		public static readonly IImage MethodReference = new LayeredImage(Method, ReferenceOverlay);
		public static readonly IImage FieldReference = new LayeredImage(Field, ReferenceOverlay);
		public static readonly IImage ExportedType = new LayeredImage(Class, ExportOverlay);

		// Overlays
		internal static readonly IImage OverlayProtected = LoadSvg(nameof(OverlayProtected));
		internal static readonly IImage OverlayInternal = LoadSvg(nameof(OverlayInternal));
		internal static readonly IImage OverlayProtectedInternal = LoadSvg(nameof(OverlayProtectedInternal));
		internal static readonly IImage OverlayPrivate = LoadSvg(nameof(OverlayPrivate));
		internal static readonly IImage OverlayPrivateProtected = LoadSvg(nameof(OverlayPrivateProtected));
		internal static readonly IImage OverlayCompilerControlled = LoadSvg(nameof(OverlayCompilerControlled));
		internal static readonly IImage OverlayStatic = LoadSvg(nameof(OverlayStatic));
		internal static readonly IImage OverlayExtension = LoadSvg(nameof(OverlayExtension));

		static readonly Dictionary<(IImage Base, AccessOverlayIcon Overlay, bool Static, bool Extension), IImage> iconCache = new();

		/// <summary>
		/// Returns a composed icon — base symbol + accessibility overlay + optional static / extension overlays.
		/// Caches results so each unique combination is only built once.
		/// </summary>
		public static IImage GetIcon(IImage baseImage, AccessOverlayIcon overlay, bool isStatic = false, bool isExtension = false)
		{
			var key = (baseImage, overlay, isStatic, isExtension);
			if (iconCache.TryGetValue(key, out var cached))
				return cached;
			// Layer order: extension behind static behind accessibility, on top of an
			// 80%-scaled base when any overlay shrinks it out of the way.
			var overlays = new List<IImage>(3);
			if (isExtension)
				overlays.Add(OverlayExtension);
			if (isStatic)
				overlays.Add(OverlayStatic);
			if (OverlayFor(overlay) is { } accessibilityOverlay)
				overlays.Add(accessibilityOverlay);
			bool scaleBase = overlays.Count > 0 || isExtension;
			var built = scaleBase
				? new LayeredImage(baseImage, baseScale: 0.8, overlays.ToArray())
				: new LayeredImage(baseImage, overlays.ToArray());
			iconCache[key] = built;
			return built;
		}

		public static AccessOverlayIcon GetOverlay(Accessibility accessibility) => accessibility switch {
			Accessibility.Public => AccessOverlayIcon.Public,
			Accessibility.Internal => AccessOverlayIcon.Internal,
			Accessibility.ProtectedAndInternal => AccessOverlayIcon.PrivateProtected,
			Accessibility.Protected => AccessOverlayIcon.Protected,
			Accessibility.ProtectedOrInternal => AccessOverlayIcon.ProtectedInternal,
			Accessibility.Private => AccessOverlayIcon.Private,
			_ => AccessOverlayIcon.CompilerControlled,
		};

		internal static IImage? OverlayFor(AccessOverlayIcon overlay) => overlay switch {
			AccessOverlayIcon.Protected => OverlayProtected,
			AccessOverlayIcon.Internal => OverlayInternal,
			AccessOverlayIcon.ProtectedInternal => OverlayProtectedInternal,
			AccessOverlayIcon.Private => OverlayPrivate,
			AccessOverlayIcon.PrivateProtected => OverlayPrivateProtected,
			AccessOverlayIcon.CompilerControlled => OverlayCompilerControlled,
			_ => null, // Public — no overlay
		};
	}

	/// <summary>
	/// Composed tree-node icon: a base symbol with N overlays drawn on top in order. The
	/// base can optionally be scaled to a fraction of the cell — used to shrink the base
	/// to 80% top-left when an accessibility overlay sits in the bottom-right corner.
	/// </summary>
	sealed class LayeredImage : IImage
	{
		static readonly Size IconSize = new(16, 16);

		public IImage BaseImage { get; }
		public double BaseScale { get; }
		public IImage[] Overlays { get; }

		public LayeredImage(IImage baseImage, params IImage[] overlays)
			: this(baseImage, 1.0, overlays)
		{
		}

		public LayeredImage(IImage baseImage, double baseScale, params IImage[] overlays)
		{
			BaseImage = baseImage;
			BaseScale = baseScale;
			Overlays = overlays;
		}

		public Size Size => IconSize;

		public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
		{
			var baseDest = BaseScale < 1.0
				? new Rect(destRect.X, destRect.Y, destRect.Width * BaseScale, destRect.Height * BaseScale)
				: destRect;
			BaseImage.Draw(context, new Rect(BaseImage.Size), baseDest);
			foreach (var overlay in Overlays)
				overlay.Draw(context, new Rect(overlay.Size), destRect);
		}
	}
}
