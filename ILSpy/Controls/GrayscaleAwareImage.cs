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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace ICSharpCode.ILSpy.Views.Controls
{
	/// <summary>
	/// An <see cref="Image"/> that swaps in a desaturated copy of its <see cref="Image.Source"/>
	/// whenever the control is effectively disabled — i.e. the templated parent (a toolbar Button
	/// / ToggleButton / SplitButton) is disabled, propagated through <c>IsEffectivelyEnabled</c>.
	/// The grayscale copy is baked once per source by rendering the original <see cref="IImage"/>
	/// into a <see cref="RenderTargetBitmap"/> and applying a BT.601 luma transform to the pixels.
	/// Caching is per instance: the same <see cref="Image.Source"/> reference will be re-baked if
	/// the user reuses the control with a different image.
	/// </summary>
	public sealed class GrayscaleAwareImage : Image
	{
		static GrayscaleAwareImage()
		{
			SourceProperty.Changed.AddClassHandler<GrayscaleAwareImage>(static (c, e) => c.OnSourceChanged(e));
			IsEffectivelyEnabledProperty.Changed.AddClassHandler<GrayscaleAwareImage>(static (c, _) => c.UpdateEffectiveSource());
		}

		IImage? originalSource;
		Bitmap? grayscaleCache;
		bool suppressSourceCallback;

		void OnSourceChanged(AvaloniaPropertyChangedEventArgs e)
		{
			// Re-entrancy guard: SetSourceInternal triggers the property-changed handler too.
			if (suppressSourceCallback)
				return;
			originalSource = e.NewValue as IImage;
			grayscaleCache = null;
			UpdateEffectiveSource();
		}

		void UpdateEffectiveSource()
		{
			if (originalSource is null)
				return;
			if (IsEffectivelyEnabled)
			{
				SetSourceInternal(originalSource);
			}
			else
			{
				grayscaleCache ??= TryBakeGrayscale(originalSource);
				// Fall back to the original on bake failure (e.g. zero-sized source) so the
				// button still has a recognisable icon, just not desaturated.
				SetSourceInternal((IImage?)grayscaleCache ?? originalSource);
			}
		}

		void SetSourceInternal(IImage source)
		{
			suppressSourceCallback = true;
			try
			{ Source = source; }
			finally { suppressSourceCallback = false; }
		}

		static Bitmap? TryBakeGrayscale(IImage source)
		{
			var size = source.Size;
			if (size.Width <= 0 || size.Height <= 0)
				return null;

			var pixelSize = new PixelSize(
				Math.Max(1, (int)Math.Ceiling(size.Width)),
				Math.Max(1, (int)Math.Ceiling(size.Height)));
			var dpi = new Vector(96, 96);

			// Render into a RenderTargetBitmap at the source's intrinsic size so vector
			// sources (SVG) rasterize at native resolution. The Image control then scales the
			// result to its own bounds, same as the live IImage path.
			using var rtb = new RenderTargetBitmap(pixelSize, dpi);
			using (var ctx = rtb.CreateDrawingContext())
			{
				source.Draw(ctx, new Rect(size), new Rect(0, 0, pixelSize.Width, pixelSize.Height));
			}

			var target = new WriteableBitmap(pixelSize, dpi, PixelFormat.Bgra8888, AlphaFormat.Premul);
			using (var fb = target.Lock())
			{
				int stride = fb.RowBytes;
				int byteCount = stride * pixelSize.Height;
				rtb.CopyPixels(new PixelRect(pixelSize), fb.Address, byteCount, stride);
				ApplyLuma(fb.Address, pixelSize, stride);
			}
			return target;
		}

		static unsafe void ApplyLuma(IntPtr address, PixelSize pixelSize, int stride)
		{
			// BT.601 luma weights × 256 = (77, 150, 29) for R, G, B. In premultiplied BGRA the
			// channels are (B, G, R, A) and luma is linear in α, so the same weighted sum works
			// without an unpremul/repremul round-trip.
			byte* basePtr = (byte*)address;
			for (int y = 0; y < pixelSize.Height; y++)
			{
				byte* row = basePtr + y * stride;
				for (int x = 0; x < pixelSize.Width; x++)
				{
					byte* px = row + x * 4;
					int luma = (77 * px[2] + 150 * px[1] + 29 * px[0]) >> 8;
					if (luma > 255)
						luma = 255;
					byte y8 = (byte)luma;
					px[0] = y8;
					px[1] = y8;
					px[2] = y8;
				}
			}
		}
	}
}
