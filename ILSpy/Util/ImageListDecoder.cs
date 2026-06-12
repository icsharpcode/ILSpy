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
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Formats.Nrbf;
using System.IO;
using System.Runtime.Serialization;

namespace ICSharpCode.ILSpy.ImageList
{
	/// <summary>
	/// One decoded ImageList frame: pixel dimensions plus the raw ARGB buffer
	/// (8-bit channels, B-G-R-A byte order, top-down, no scan-line padding so
	/// stride is exactly <c>Width × 4</c>). UI-framework-free so the decoder can
	/// be tested on Windows with WinForms <c>Bitmap.LockBits</c> for the reference
	/// comparison without dragging Avalonia into the test pipeline.
	/// </summary>
	public sealed record DecodedImage(int Width, int Height, byte[] BgraPixels);

	/// <summary>
	/// Cross-platform decoder for <c>System.Windows.Forms.ImageListStreamer</c>
	/// payloads. See <see href="FORMAT.md">FORMAT.md</see> alongside this file for
	/// the three-layer byte-format reference.
	/// </summary>
	public static class ImageListDecoder
	{
		/// <summary>The NRBF type-name prefix that <see cref="Decode(byte[])"/> claims.</summary>
		public const string TargetTypeNamePrefix = "System.Windows.Forms.ImageListStreamer";

		/// <summary>
		/// Cap on the decoded RLE output. Hostile payloads could otherwise expand a
		/// 4-byte input into gigabytes; the longest real ImageList payloads observed
		/// in practice are a few MB. 100 MB is well above the practical ceiling and
		/// well below "memory exhaustion".
		/// </summary>
		const int MaxDecodedRleSize = 100 * 1024 * 1024;

		// ILHEAD flags from CommCtrl.h.
		const ushort ILC_MASK = 0x0001;
		const ushort ILC_COLORMASK = 0x00FE;
		const ushort ILHEAD_MAGIC = 0x4C49;     // 'IL' little-endian

		/// <summary>
		/// Decodes the NRBF-wrapped ImageListStreamer payload <paramref name="nrbfBlob"/>
		/// into its constituent frames. See <c>FORMAT.md</c> for the byte format.
		/// </summary>
		public static IReadOnlyList<DecodedImage> Decode(byte[] nrbfBlob)
		{
			ArgumentNullException.ThrowIfNull(nrbfBlob);
			using var stream = new MemoryStream(nrbfBlob, writable: false);
			return Decode(stream);
		}

		/// <summary>
		/// Stream-based overload. Reads the NRBF payload from <paramref name="stream"/>;
		/// the stream is positioned to the first byte after the payload on return.
		/// </summary>
		public static IReadOnlyList<DecodedImage> Decode(Stream stream)
		{
			ArgumentNullException.ThrowIfNull(stream);

			byte[] data = UnwrapNrbf(stream);
			byte[] decompressed = TryDecodeMsftRle(data);
			return ParseIlheadAndSlice(decompressed);
		}

		// ---------------------------------------------------------------------------------
		// Layer 1: NRBF envelope -> raw byte[] payload from the streamer's "Data" member.
		// ---------------------------------------------------------------------------------
		static byte[] UnwrapNrbf(Stream stream)
		{
			SerializationRecord rootRecord;
			try
			{
				rootRecord = NrbfDecoder.Decode(stream);
			}
			catch (SerializationException ex)
			{
				throw new InvalidDataException("Malformed NRBF payload for an ImageListStreamer entry.", ex);
			}
			if (rootRecord is not ClassRecord classRecord)
				throw new InvalidDataException(
					"Expected an NRBF class record at the root of an ImageListStreamer payload, " +
					$"got {rootRecord.GetType().Name}.");

			string typeName = classRecord.TypeName.FullName;
			if (!typeName.StartsWith(TargetTypeNamePrefix, StringComparison.Ordinal))
				throw new InvalidDataException(
					$"Expected an {TargetTypeNamePrefix} payload but found '{typeName}'.");

			if (classRecord.GetSerializationRecord("Data") is not SZArrayRecord<byte> arrayRecord)
				throw new InvalidDataException(
					$"{TargetTypeNamePrefix} payload missing the byte[] 'Data' member.");

			return arrayRecord.GetArray();
		}

		// ---------------------------------------------------------------------------------
		// Layer 2: optional MSFt-prefixed RLE. Pre-Win2000 payloads omit this layer entirely
		// and we just pass the buffer through, matching ImageListStreamer.Decompress.
		// ---------------------------------------------------------------------------------
		static byte[] TryDecodeMsftRle(byte[] data)
		{
			if (data.Length < 4
				|| data[0] != (byte)'M' || data[1] != (byte)'S'
				|| data[2] != (byte)'F' || data[3] != (byte)'t')
			{
				return data;
			}
			// (count, value) pairs until end-of-buffer.
			long decodedLen = 0;
			for (int i = 4; i + 1 < data.Length; i += 2)
				decodedLen += data[i];
			if (decodedLen > MaxDecodedRleSize)
				throw new InvalidDataException(
					$"MSFt RLE decoded size {decodedLen:N0} exceeds safety cap {MaxDecodedRleSize:N0}.");

			byte[] output = new byte[(int)decodedLen];
			int pos = 0;
			for (int i = 4; i + 1 < data.Length; i += 2)
			{
				byte count = data[i];
				byte value = data[i + 1];
				if (value == 0)
				{
					pos += count;       // zero-fill — array starts zeroed
				}
				else
				{
					for (int j = 0; j < count; j++)
						output[pos++] = value;
				}
			}
			return output;
		}

		// ---------------------------------------------------------------------------------
		// Layer 3: ILHEAD + color DIB + optional mask DIB -> sliced frames.
		// ---------------------------------------------------------------------------------
		static IReadOnlyList<DecodedImage> ParseIlheadAndSlice(byte[] data)
		{
			if (data.Length < 28)
				throw new InvalidDataException(
					$"Buffer too small for an ILHEAD: have {data.Length} bytes, need at least 28.");

			var span = data.AsSpan();
			ushort magic = BinaryPrimitives.ReadUInt16LittleEndian(span);
			if (magic != ILHEAD_MAGIC)
				throw new InvalidDataException(
					$"Unexpected ILHEAD magic 0x{magic:X4}, expected 0x{ILHEAD_MAGIC:X4} ('IL').");

			// ushort usVersion at offset 2 — value isn't validated; observed: 0x0101 and 0x0600
			ushort cCurImage = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(4));
			// cMaxImage (6), cGrow (8) — capacities, irrelevant to decode
			ushort cx = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(10));
			ushort cy = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(12));
			// bkcolor at 14 (uint32 COLORREF) — only meaningful when frames render against
			// a window background; the decoded buffer reports it as if the strip's own
			// alpha channel is canonical, so bkcolor is ignored.
			ushort flags = BinaryPrimitives.ReadUInt16LittleEndian(span.Slice(18));
			// 4 × ushort ovls follow at offset 20..27.

			bool hasMask = (flags & ILC_MASK) != 0;

			int offset = 28;
			var (colorDib, colorEnd) = ReadDib(data, offset);
			offset = colorEnd;

			Dib? maskDib = null;
			if (hasMask)
			{
				if (offset >= data.Length)
					throw new InvalidDataException("ILHEAD claims a mask but no mask DIB followed the color DIB.");
				var (parsedMask, _) = ReadDib(data, offset);
				if (parsedMask.BitCount != 1)
					throw new InvalidDataException($"Expected a 1bpp mask DIB, got {parsedMask.BitCount}bpp.");
				maskDib = parsedMask;
			}

			return SliceFrames(cCurImage, cx, cy, colorDib, maskDib);
		}

		// ---------------------------------------------------------------------------------
		// DIB reader. Lays out BITMAPFILEHEADER(14) + BITMAPINFOHEADER(40) + palette + pixels,
		// returns a Dib struct plus the byte offset that immediately follows the pixel data.
		// ---------------------------------------------------------------------------------
		readonly record struct Dib(
			int Width,
			int Height,
			int BitCount,
			byte[] Pixels,          // raw scanlines, length = Stride * abs(Height)
			int Stride,
			bool IsTopDown,
			byte[] Palette          // 4-byte BGRA entries, empty for >8bpp
		);

		static (Dib dib, int endOffset) ReadDib(byte[] data, int offset)
		{
			const int FileHeaderSize = 14;
			const int InfoHeaderSize = 40;
			if (offset + FileHeaderSize + InfoHeaderSize > data.Length)
				throw new InvalidDataException("Truncated DIB headers.");

			// BITMAPFILEHEADER — 'BM' magic + total size + 2x reserved + offset to pixels.
			if (data[offset] != (byte)'B' || data[offset + 1] != (byte)'M')
				throw new InvalidDataException("DIB missing 'BM' file-header magic.");
			uint bfSize = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset + 2));
			uint bfOffBits = BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(offset + 10));

			// BITMAPINFOHEADER.
			var info = data.AsSpan(offset + FileHeaderSize);
			uint biSize = BinaryPrimitives.ReadUInt32LittleEndian(info);
			if (biSize < InfoHeaderSize)
				throw new InvalidDataException($"Unexpected biSize {biSize} (need >= {InfoHeaderSize}).");
			int biWidth = BinaryPrimitives.ReadInt32LittleEndian(info.Slice(4));
			int biHeight = BinaryPrimitives.ReadInt32LittleEndian(info.Slice(8));
			ushort biBitCount = BinaryPrimitives.ReadUInt16LittleEndian(info.Slice(14));
			uint biSizeImage = BinaryPrimitives.ReadUInt32LittleEndian(info.Slice(20));
			uint biClrUsed = BinaryPrimitives.ReadUInt32LittleEndian(info.Slice(32));

			bool topDown = biHeight < 0;
			int absHeight = topDown ? -biHeight : biHeight;
			int stride = ((biWidth * biBitCount + 31) / 32) * 4;
			int pixelByteCount = stride * absHeight;
			if (biSizeImage != 0 && biSizeImage < pixelByteCount)
				pixelByteCount = (int)biSizeImage;     // trust the writer when it's smaller

			int paletteEntryCount = biBitCount <= 8
				? (biClrUsed != 0 ? (int)biClrUsed : (1 << biBitCount))
				: 0;
			int paletteBytes = paletteEntryCount * 4;

			int pixelsOffset = (int)bfOffBits != 0
				? offset + (int)bfOffBits
				: offset + FileHeaderSize + (int)biSize + paletteBytes;
			int paletteOffset = offset + FileHeaderSize + (int)biSize;

			if (paletteOffset + paletteBytes > data.Length)
				throw new InvalidDataException("Truncated DIB palette.");
			if (pixelsOffset + pixelByteCount > data.Length)
				throw new InvalidDataException("Truncated DIB pixel block.");

			byte[] palette = paletteBytes == 0
				? Array.Empty<byte>()
				: data.AsSpan(paletteOffset, paletteBytes).ToArray();
			byte[] pixels = data.AsSpan(pixelsOffset, pixelByteCount).ToArray();

			int endOffset = pixelsOffset + pixelByteCount;
			// Match the file-header's bfSize when present so RLE-padding tails don't trip us.
			if (bfSize > 0)
				endOffset = Math.Max(endOffset, offset + (int)bfSize);

			return (new Dib(biWidth, absHeight, biBitCount, pixels, stride, topDown, palette), endOffset);
		}

		// ---------------------------------------------------------------------------------
		// Tile slicing. Each frame is cx × cy; the strip lays them out row-major in a grid
		// of cols = bmWidth / cx columns. Output is always top-down 32bpp BGRA.
		// ---------------------------------------------------------------------------------
		static IReadOnlyList<DecodedImage> SliceFrames(ushort count, ushort cx, ushort cy, Dib color, Dib? mask)
		{
			if (cx == 0 || cy == 0)
				throw new InvalidDataException($"ILHEAD reports zero frame size: cx={cx}, cy={cy}.");
			int cols = color.Width / cx;
			if (cols <= 0)
				throw new InvalidDataException(
					$"Color DIB width {color.Width} is smaller than a single frame ({cx}).");

			var result = new DecodedImage[count];
			for (int i = 0; i < count; i++)
			{
				int col = i % cols;
				int row = i / cols;
				int srcX = col * cx;
				int srcY = row * cy;
				result[i] = ExtractFrame(color, mask, srcX, srcY, cx, cy);
			}
			return result;
		}

		static DecodedImage ExtractFrame(Dib color, Dib? mask, int srcX, int srcY, int cx, int cy)
		{
			byte[] bgra = new byte[cx * cy * 4];

			for (int y = 0; y < cy; y++)
			{
				// Map our top-down row y to the source DIB row, accounting for bottom-up storage.
				int colorRow = color.IsTopDown ? (srcY + y) : (color.Height - 1 - (srcY + y));
				int colorRowOffset = colorRow * color.Stride;

				for (int x = 0; x < cx; x++)
				{
					int srcPixelX = srcX + x;
					ReadColorPixel(color, colorRowOffset, srcPixelX,
						out byte b, out byte g, out byte r, out byte aFromColor);

					byte alpha;
					if (color.BitCount == 32)
					{
						alpha = aFromColor;
					}
					else if (mask is { } m)
					{
						int maskRow = m.IsTopDown ? (srcY + y) : (m.Height - 1 - (srcY + y));
						int maskRowOffset = maskRow * m.Stride;
						// 1bpp: bit set in mask DIB means transparent (Win32 convention),
						// so we invert here to produce a sensible alpha channel.
						byte maskByte = m.Pixels[maskRowOffset + (srcPixelX >> 3)];
						int maskBit = (maskByte >> (7 - (srcPixelX & 7))) & 1;
						alpha = maskBit == 0 ? (byte)0xFF : (byte)0x00;
					}
					else
					{
						alpha = 0xFF;
					}

					int dst = (y * cx + x) * 4;
					bgra[dst + 0] = b;
					bgra[dst + 1] = g;
					bgra[dst + 2] = r;
					bgra[dst + 3] = alpha;
				}
			}
			return new DecodedImage(cx, cy, bgra);
		}

		static void ReadColorPixel(Dib color, int rowOffset, int x, out byte b, out byte g, out byte r, out byte a)
		{
			switch (color.BitCount)
			{
				case 32:
				{
					int o = rowOffset + x * 4;
					b = color.Pixels[o + 0];
					g = color.Pixels[o + 1];
					r = color.Pixels[o + 2];
					a = color.Pixels[o + 3];
					return;
				}
				case 24:
				{
					int o = rowOffset + x * 3;
					b = color.Pixels[o + 0];
					g = color.Pixels[o + 1];
					r = color.Pixels[o + 2];
					a = 0xFF;
					return;
				}
				case 16:
				{
					// BI_RGB at 16bpp is XRGB1555 little-endian per Win32 DIB convention.
					ushort px = BinaryPrimitives.ReadUInt16LittleEndian(color.Pixels.AsSpan(rowOffset + x * 2, 2));
					b = (byte)(((px >> 0) & 0x1F) << 3);
					g = (byte)(((px >> 5) & 0x1F) << 3);
					r = (byte)(((px >> 10) & 0x1F) << 3);
					a = 0xFF;
					return;
				}
				case 8:
				{
					int idx = color.Pixels[rowOffset + x];
					int p = idx * 4;
					b = color.Palette[p + 0];
					g = color.Palette[p + 1];
					r = color.Palette[p + 2];
					a = 0xFF;
					return;
				}
				case 4:
				{
					byte packed = color.Pixels[rowOffset + (x >> 1)];
					int idx = (x & 1) == 0 ? packed >> 4 : packed & 0x0F;
					int p = idx * 4;
					b = color.Palette[p + 0];
					g = color.Palette[p + 1];
					r = color.Palette[p + 2];
					a = 0xFF;
					return;
				}
				default:
					throw new InvalidDataException($"Unsupported color depth {color.BitCount} bpp.");
			}
		}
	}
}
