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
using System.Collections.Concurrent;
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

		// Parsed SVG per icon file name. Parsing is the expensive part of an icon and needs no
		// UI thread, so it can run anywhere. The Lazy makes a name parse exactly once even
		// when two threads ask for it at the same time; the second one waits for the first
		// instead of parsing again.
		static readonly ConcurrentDictionary<string, Lazy<SvgSource>> svgSources = new();

		static SvgSource ParseSvg(string name)
			=> svgSources.GetOrAdd(name, static n => new Lazy<SvgSource>(
				() => SvgSource.Load(AssetBase + n + ".svg", null))).Value;

		static IImage Svg(string name) => new SvgIcon(ParseSvg(name));

		/// <summary>
		/// Parses every SVG in the icon folder, on whatever thread calls it. Started on a
		/// thread-pool thread as soon as Avalonia's asset loader exists, before App.axaml and
		/// the themes load, so that by the time the first view asks for an icon, the property
		/// getters only wrap sources that are already parsed. It competes for the JIT with
		/// the rest of startup, so it needs that much head start: measured on the eager
		/// version of this class, the first parse
		/// alone cost about 300 ms (one-time JIT and static initialization of the Svg.Skia
		/// stack), the remaining 75 about 2 ms each, all on the UI thread. An icon a getter
		/// asks for before the preload reaches it is parsed by the getter itself, so this is
		/// purely an optimization; a failure here is swallowed for the same reason.
		/// </summary>
		public static void PreloadSvgs()
		{
			try
			{
				using var _ = AppEnv.AppLog.Phase("Images: SVG preload");
				foreach (var uri in AssetLoader.GetAssets(new Uri(AssetBase), null))
				{
					var file = uri.Segments[^1];
					if (file.EndsWith(".svg", StringComparison.Ordinal))
						ParseSvg(file[..^".svg".Length]);
				}
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"[Images] PreloadSvgs failed: {ex}");
			}
		}

		internal static int ParsedSvgCount => svgSources.Count;

		static IImage Png(string name)
		{
			using var stream = AssetLoader.Open(new Uri(AssetBase + name + ".png"));
			return new Bitmap(stream);
		}

		// Assemblies / packages
		public static IImage Assembly => field ??= Svg(nameof(Assembly));
		public static IImage AssemblyList => field ??= Svg(nameof(AssemblyList));
		public static IImage AssemblyListGAC => field ??= Svg(nameof(AssemblyListGAC));
		public static IImage AssemblyLoading => field ??= Svg(nameof(AssemblyLoading));
		public static IImage AssemblyWarning => field ??= Svg(nameof(AssemblyWarning));
		public static IImage Warning => field ??= Svg(nameof(Warning));
		public static IImage FindAssembly => field ??= Svg(nameof(FindAssembly));
		public static IImage Search => field ??= Svg(nameof(Search));
		public static IImage Library => field ??= Svg(nameof(Library));
		public static IImage NuGet => field ??= Png(nameof(NuGet));
		public static IImage Process => field ??= Svg(nameof(Process));
		public static IImage MetadataFile => field ??= Svg(nameof(MetadataFile));
		public static IImage WebAssemblyFile => field ??= Svg("WebAssembly");
		public static IImage ProgramDebugDatabase => field ??= Svg(nameof(ProgramDebugDatabase));

		// Toolbar / navigation
		public static IImage Back => field ??= Svg(nameof(Back));
		public static IImage Forward => field ??= Svg(nameof(Forward));
		public static IImage Open => field ??= Svg(nameof(Open));
		public static IImage Save => field ??= Svg(nameof(Save));
		public static IImage Refresh => field ??= Svg(nameof(Refresh));
		public static IImage Sort => field ??= Svg(nameof(Sort));
		public static IImage CollapseAll => field ??= Svg(nameof(CollapseAll));
		public static IImage Delete => field ??= Svg(nameof(Delete));
		public static IImage ShowPublicOnly => field ??= Svg(nameof(ShowPublicOnly));
		public static IImage ShowPrivateInternal => field ??= Svg(nameof(ShowPrivateInternal));
		public static IImage ShowAll => field ??= Svg(nameof(ShowAll));

		// Bookmarks (file names carry dots, so the dotted variants can't use nameof).
		public static IImage Bookmark => field ??= Svg(nameof(Bookmark));
		public static IImage BookmarkDisable => field ??= Svg("Bookmark.Disable");
		public static IImage BookmarkNext => field ??= Svg("Bookmark.Next");
		public static IImage BookmarkPrevious => field ??= Svg("Bookmark.Previous");
		public static IImage BookmarkNextInFile => field ??= Svg("Bookmark.Next.File");
		public static IImage BookmarkPreviousInFile => field ??= Svg("Bookmark.Previous.File");
		public static IImage BookmarkClear => field ??= Svg("Bookmark.Clear");

		// Type-relation tree nodes (Base Types / Derived Types).
		public static IImage SuperTypes => field ??= Svg(nameof(SuperTypes));
		public static IImage SubTypes => field ??= Svg(nameof(SubTypes));

		// Containers
		public static IImage Namespace => field ??= Svg(nameof(Namespace));
		public static IImage ReferenceFolder => field ??= Svg(nameof(ReferenceFolder));
		public static IImage MetadataTable => field ??= Svg(nameof(MetadataTable));
		public static IImage MetadataTableGroup => field ??= Svg(nameof(MetadataTableGroup));
		public static IImage Metadata => field ??= Svg(nameof(Metadata));
		public static IImage Heap => field ??= Svg(nameof(Heap));
		public static IImage Header => field ??= Svg(nameof(Header));
		public static IImage FolderClosed => field ??= Svg(nameof(FolderClosed));
		public static IImage FolderOpen => field ??= Svg(nameof(FolderOpen));
		public static IImage ListFolder => field ??= Svg(nameof(ListFolder));
		public static IImage ListFolderOpen => field ??= Svg(nameof(ListFolderOpen));
		public static IImage Resource => field ??= Svg(nameof(Resource));
		public static IImage ResourceResourcesFile => field ??= Svg(nameof(ResourceResourcesFile));

		// Types
		public static IImage Class => field ??= Svg(nameof(Class));
		public static IImage Interface => field ??= Svg(nameof(Interface));
		public static IImage Struct => field ??= Svg(nameof(Struct));
		public static IImage Enum => field ??= Svg(nameof(Enum));
		public static IImage Delegate => field ??= Svg(nameof(Delegate));

		// Methods
		public static IImage Method => field ??= Svg(nameof(Method));
		public static IImage Constructor => field ??= Svg(nameof(Constructor));
		public static IImage Operator => field ??= Svg(nameof(Operator));
		public static IImage VirtualMethod => field ??= Svg(nameof(VirtualMethod));
		public static IImage ExtensionMethod => field ??= Svg(nameof(ExtensionMethod));
		public static IImage PInvokeMethod => field ??= Svg(nameof(PInvokeMethod));

		// Fields / properties / events
		public static IImage Field => field ??= Svg(nameof(Field));
		public static IImage FieldReadOnly => field ??= Svg(nameof(FieldReadOnly));
		public static IImage EnumValue => field ??= Svg(nameof(EnumValue));
		public static IImage Property => field ??= Svg(nameof(Property));
		public static IImage Indexer => field ??= Svg(nameof(Indexer));
		public static IImage Event => field ??= Svg(nameof(Event));
		public static IImage Literal => field ??= Svg(nameof(Literal));
		public static IImage ViewCode => field ??= Svg(nameof(ViewCode));

		// Editing / context-menu icons
		public static IImage Copy => field ??= Svg(nameof(Copy));

		// Reference overlays (small badge layered onto a base icon for TypeRef/MemberRef nodes).
		internal static IImage ReferenceOverlay => field ??= Svg(nameof(ReferenceOverlay));
		internal static IImage ExportOverlay => field ??= Svg(nameof(ExportOverlay));

		public static IImage TypeReference => field ??= new LayeredImage(Class, ReferenceOverlay);
		public static IImage MethodReference => field ??= new LayeredImage(Method, ReferenceOverlay);
		public static IImage FieldReference => field ??= new LayeredImage(Field, ReferenceOverlay);
		public static IImage ExportedType => field ??= new LayeredImage(Class, ExportOverlay);

		// Overlays
		internal static IImage OverlayProtected => field ??= Svg(nameof(OverlayProtected));
		internal static IImage OverlayInternal => field ??= Svg(nameof(OverlayInternal));
		internal static IImage OverlayProtectedInternal => field ??= Svg(nameof(OverlayProtectedInternal));
		internal static IImage OverlayPrivate => field ??= Svg(nameof(OverlayPrivate));
		internal static IImage OverlayPrivateProtected => field ??= Svg(nameof(OverlayPrivateProtected));
		internal static IImage OverlayCompilerControlled => field ??= Svg(nameof(OverlayCompilerControlled));
		internal static IImage OverlayStatic => field ??= Svg(nameof(OverlayStatic));
		internal static IImage OverlayExtension => field ??= Svg(nameof(OverlayExtension));

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
		/// property on <see cref="Images"/>. Returns <see langword="null"/> when the path is
		/// empty or the named property does not exist, so callers can skip-and-continue without
		/// breaking the menu/toolbar build.
		/// </summary>
		public static IImage? ResolveByPath(string? path)
		{
			if (string.IsNullOrEmpty(path))
				return null;
			var name = path.StartsWith("Images/", StringComparison.Ordinal)
				? path["Images/".Length..]
				: path;
			var property = typeof(Images).GetProperty(name, BindingFlags.Public | BindingFlags.Static);
			return property?.GetValue(null) as IImage;
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
	/// An icon drawn from a parsed SVG. Draws the way <see cref="SvgImage"/> does, but it is
	/// not an AvaloniaObject: tree nodes and search results resolve their icons on decompiler
	/// worker threads, and an AvaloniaObject built there is owned by that thread and throws
	/// as soon as the UI thread measures it. A plain object can be built on any thread.
	/// </summary>
	sealed class SvgIcon(SvgSource source) : IImage
	{
		public Size Size => source.Picture is { } picture
			? new Size(picture.CullRect.Width, picture.CullRect.Height)
			: default;

		public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
		{
			if (source.Picture is not { } picture || Size.Width <= 0 || Size.Height <= 0)
				return;
			var cullRect = picture.CullRect;
			var scale = Matrix.CreateScale(destRect.Width / sourceRect.Width, destRect.Height / sourceRect.Height);
			var translate = Matrix.CreateTranslation(
				-sourceRect.X + destRect.X - cullRect.Top,
				-sourceRect.Y + destRect.Y - cullRect.Left);
			using (context.PushClip(destRect))
			using (context.PushTransform(scale * translate))
				context.Custom(new SvgSourceCustomDrawOperation(new Rect(0, 0, cullRect.Width, cullRect.Height), source));
		}
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
