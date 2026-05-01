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

		// Containers
		public static readonly IImage Namespace = LoadSvg(nameof(Namespace));
		public static readonly IImage ReferenceFolder = LoadSvg(nameof(ReferenceFolder));
		public static readonly IImage MetadataTable = LoadSvg(nameof(MetadataTable));

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
			var built = new OverlayImage(baseImage, overlay, isStatic, isExtension);
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
	/// Composed tree-node icon: a base symbol with optional accessibility overlay, static badge, or extension badge.
	/// Mirrors the layered drawing scheme from <see cref="ICSharpCode.ILSpy.Images"/> in the WPF host.
	/// </summary>
	sealed class OverlayImage : IImage
	{
		static readonly Size IconSize = new(16, 16);

		readonly IImage baseImage;
		readonly IImage? overlayImage;
		readonly bool isStatic;
		readonly bool isExtension;
		readonly bool scaleBase;

		public OverlayImage(IImage baseImage, AccessOverlayIcon overlay, bool isStatic, bool isExtension)
		{
			this.baseImage = baseImage;
			this.overlayImage = Images.OverlayFor(overlay);
			this.isStatic = isStatic;
			this.isExtension = isExtension;
			// When any overlay shrinks the base out of the way, base draws at 80% in the top-left.
			this.scaleBase = overlayImage != null || isExtension;
		}

		public Size Size => IconSize;

		public void Draw(DrawingContext context, Rect sourceRect, Rect destRect)
		{
			var baseDest = scaleBase
				? new Rect(destRect.X, destRect.Y, destRect.Width * 0.8, destRect.Height * 0.8)
				: destRect;
			baseImage.Draw(context, new Rect(baseImage.Size), baseDest);

			if (isExtension)
				Images.OverlayExtension.Draw(context, new Rect(Images.OverlayExtension.Size), destRect);

			if (isStatic)
				Images.OverlayStatic.Draw(context, new Rect(Images.OverlayStatic.Size), destRect);

			if (overlayImage != null)
				overlayImage.Draw(context, new Rect(overlayImage.Size), destRect);
		}
	}
}
