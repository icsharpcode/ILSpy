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
using System.Reflection;

using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Svg.Skia;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.ILSpy
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
		public static readonly IImage AssemblyList = LoadSvg(nameof(AssemblyList));
		public static readonly IImage AssemblyListGAC = LoadSvg(nameof(AssemblyListGAC));
		public static readonly IImage AssemblyLoading = LoadSvg(nameof(AssemblyLoading));
		public static readonly IImage AssemblyWarning = LoadSvg(nameof(AssemblyWarning));
		public static readonly IImage Warning = LoadSvg(nameof(Warning));
		public static readonly IImage FindAssembly = LoadSvg(nameof(FindAssembly));
		public static readonly IImage Search = LoadSvg(nameof(Search));
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
		public static readonly IImage Refresh = LoadSvg(nameof(Refresh));
		public static readonly IImage Sort = LoadSvg(nameof(Sort));
		public static readonly IImage CollapseAll = LoadSvg(nameof(CollapseAll));
		public static readonly IImage Delete = LoadSvg(nameof(Delete));
		public static readonly IImage ShowPublicOnly = LoadSvg(nameof(ShowPublicOnly));
		public static readonly IImage ShowPrivateInternal = LoadSvg(nameof(ShowPrivateInternal));
		public static readonly IImage ShowAll = LoadSvg(nameof(ShowAll));

		// Type-relation tree nodes (Base Types / Derived Types).
		public static readonly IImage SuperTypes = LoadSvg(nameof(SuperTypes));
		public static readonly IImage SubTypes = LoadSvg(nameof(SubTypes));

		// Containers
		public static readonly IImage Namespace = LoadSvg(nameof(Namespace));
		public static readonly IImage ReferenceFolder = LoadSvg(nameof(ReferenceFolder));
		public static readonly IImage MetadataTable = LoadSvg(nameof(MetadataTable));
		public static readonly IImage MetadataTableGroup = LoadSvg(nameof(MetadataTableGroup));
		public static readonly IImage Metadata = LoadSvg(nameof(Metadata));
		public static readonly IImage Heap = LoadSvg(nameof(Heap));
		public static readonly IImage Header = LoadSvg(nameof(Header));
		public static readonly IImage FolderClosed = LoadSvg(nameof(FolderClosed));
		public static readonly IImage FolderOpen = LoadSvg(nameof(FolderOpen));
		public static readonly IImage ListFolder = LoadSvg(nameof(ListFolder));
		public static readonly IImage ListFolderOpen = LoadSvg(nameof(ListFolderOpen));
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
		public static readonly IImage VirtualMethod = LoadSvg(nameof(VirtualMethod));
		public static readonly IImage ExtensionMethod = LoadSvg(nameof(ExtensionMethod));
		public static readonly IImage PInvokeMethod = LoadSvg(nameof(PInvokeMethod));

		// Fields / properties / events
		public static readonly IImage Field = LoadSvg(nameof(Field));
		public static readonly IImage FieldReadOnly = LoadSvg(nameof(FieldReadOnly));
		public static readonly IImage EnumValue = LoadSvg(nameof(EnumValue));
		public static readonly IImage Property = LoadSvg(nameof(Property));
		public static readonly IImage Indexer = LoadSvg(nameof(Indexer));
		public static readonly IImage Event = LoadSvg(nameof(Event));
		public static readonly IImage Literal = LoadSvg(nameof(Literal));
		public static readonly IImage ViewCode = LoadSvg(nameof(ViewCode));

		// Editing / context-menu icons
		public static readonly IImage Copy = LoadSvg(nameof(Copy));

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

		/// <summary>
		/// Resolve an "Images/Foo" string (from MEF metadata like <c>MenuIcon</c>,
		/// <c>Icon</c>, or <c>ToolbarIcon</c>) to the corresponding static <see cref="IImage"/>
		/// field on <see cref="Images"/>. Returns <see langword="null"/> when the path is
		/// empty or the named field doesn't exist, so callers can skip-and-continue without
		/// breaking the menu/toolbar build.
		/// </summary>
		public static IImage? ResolveByPath(string? path)
		{
			if (string.IsNullOrEmpty(path))
				return null;
			var name = path.StartsWith("Images/", StringComparison.Ordinal)
				? path["Images/".Length..]
				: path;
			var field = typeof(Images).GetField(name, BindingFlags.Public | BindingFlags.Static);
			return field?.GetValue(null) as IImage;
		}

		/// <summary>
		/// Rasterise an <see cref="IImage"/> (typically an <see cref="SvgImage"/>) into a
		/// fixed-size <see cref="Bitmap"/>. Needed for surfaces whose Icon contract is
		/// <see cref="Bitmap"/> rather than the broader <see cref="IImage"/>, notably
		/// <c>NativeMenuItem.Icon</c>, where the macOS native-menu bridge talks to
		/// <c>NSImage</c> which has no vector form.
		/// </summary>
		public static Bitmap? RenderToBitmap(IImage? image, int size = 16)
		{
			if (image == null)
				return null;
			var bmp = new RenderTargetBitmap(new PixelSize(size, size));
			using var ctx = bmp.CreateDrawingContext();
			image.Draw(ctx, new Rect(image.Size), new Rect(0, 0, size, size));
			return bmp;
		}

		/// <summary>
		/// Convenience: <see cref="ResolveByPath"/> composed with <see cref="RenderToBitmap"/>.
		/// For call sites that need a Bitmap straight from the "Images/Foo" metadata string.
		/// </summary>
		public static Bitmap? LoadBitmap(string? path, int size = 16)
			=> RenderToBitmap(ResolveByPath(path), size);
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
