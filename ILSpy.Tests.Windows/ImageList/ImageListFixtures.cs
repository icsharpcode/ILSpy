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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace ICSharpCode.ILSpy.Tests.Windows;

/// <summary>
/// Builds <see cref="System.Windows.Forms.ImageList"/> instances at runtime and emits the
/// exact NRBF wire bytes that <c>ResourceSerializedObject.GetBytes()</c> would return for
/// such an entry in a real <c>.resources</c> stream. No binaries are checked in — every
/// fixture exists only in memory for the lifetime of one test.
/// </summary>
/// <remarks>
/// Why this exists at all: <c>BinaryFormatter</c> is removed from net9+, so we can't ask
/// the BCL to serialise the streamer for us. We call the streamer's
/// <see cref="ISerializable.GetObjectData"/> to extract its single <c>byte[] Data</c> member,
/// then hand-write a minimal but spec-conformant NRBF envelope around that byte[] — the
/// same envelope <c>BinaryFormatter</c> would have written on .NET Framework.
/// </remarks>
internal static class ImageListFixtures
{
	/// <summary>Single source of truth for the test bitmap drawn into each frame.</summary>
	public static Bitmap BuildFrame(int width, int height, int frameIndex, ColorDepth depth)
	{
		var bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
		using (var g = Graphics.FromImage(bmp))
		{
			// Each frame gets a different hue so cross-frame mismatches are loud, plus an
			// X across the tile to exercise the alpha path on 32 bpp ImageLists.
			byte hue = (byte)((frameIndex * 37) & 0xFF);
			g.Clear(Color.FromArgb(255, hue, (byte)(255 - hue), 128));
			using var pen = new Pen(Color.Black, 1f);
			g.DrawLine(pen, 0, 0, width - 1, height - 1);
			g.DrawLine(pen, 0, height - 1, width - 1, 0);
		}
		// Punch a transparent pixel so the alpha path has something distinctive.
		// Skip palettised modes — those will quantise the alpha away.
		if (depth == ColorDepth.Depth32Bit)
			bmp.SetPixel(0, 0, Color.FromArgb(0, 0, 0, 0));
		return bmp;
	}

	public sealed record Fixture(
		byte[] NrbfBlob,
		string TypeName,
		Size FrameSize,
		Bitmap[] Frames,
		ColorDepth Depth,
		bool HasMask) : IDisposable
	{
		public void Dispose()
		{
			foreach (var f in Frames)
				f.Dispose();
		}
	}

	public static Fixture Build(ColorDepth depth, bool withMask, int count, int frameSize = 16)
	{
		var list = new System.Windows.Forms.ImageList {
			ColorDepth = depth,
			ImageSize = new Size(frameSize, frameSize),
			TransparentColor = withMask ? Color.Magenta : Color.Transparent,
		};
		// Sources stay alive until after the strip is materialised — ImageList.Add only
		// keeps a reference to the source bitmap and copies into the strip lazily.
		var sources = new Bitmap[count];
		for (int i = 0; i < count; i++)
		{
			sources[i] = BuildFrame(frameSize, frameSize, i, depth);
			list.Images.Add(sources[i]);
		}
		var streamer = list.ImageStream
			?? throw new InvalidOperationException("ImageList.ImageStream was null after adding frames.");
		// Capture frames AS STORED IN the strip. Sub-32 depths re-quantise so this is the
		// real ground truth for byte comparison; comparing against the originals would
		// measure WinForms's palettisation drift instead of decoder correctness.
		var frames = new Bitmap[count];
		for (int i = 0; i < count; i++)
			frames[i] = new Bitmap(list.Images[i]);
		foreach (var s in sources)
			s.Dispose();

		// Pull the raw "Data" byte[] out of the streamer via the ISerializable surface.
		// On .NET 10 we can't ask BinaryFormatter to do this for us, but ImageListStreamer
		// still implements ISerializable so GetObjectData is callable directly.
		// SYSLIB0050 marks the whole formatter-serialisation surface obsolete — fine here,
		// that's the only path the BCL offers for direct ISerializable invocation.
#pragma warning disable SYSLIB0050
		var info = new SerializationInfo(typeof(ImageListStreamer), new FormatterConverter());
		((ISerializable)streamer).GetObjectData(info, new StreamingContext(StreamingContextStates.All));
#pragma warning restore SYSLIB0050
		byte[] data = (byte[])info.GetValue("Data", typeof(byte[]))!;

		string typeName = typeof(ImageListStreamer).FullName!;
		string libraryName = typeof(ImageListStreamer).Assembly.FullName!;
		byte[] nrbf = BuildNrbfClassWithByteArrayMember(typeName, libraryName, "Data", data);

		list.Dispose();
		return new Fixture(nrbf, typeName, new Size(frameSize, frameSize), frames, depth, withMask);
	}

	// Records from [MS-NRBF] §2.1.2 (RecordTypeEnumeration). Spelt out as constants here
	// instead of referencing the BCL enum because System.Formats.Nrbf doesn't expose the
	// writer side — only the reader.
	const byte RecSerializationHeader = 0;
	const byte RecClassWithMembersAndTypes = 5;
	const byte RecMemberReference = 9;
	const byte RecMessageEnd = 11;
	const byte RecBinaryLibrary = 12;
	const byte RecArraySinglePrimitive = 15;
	const byte BinaryTypePrimitiveArray = 7;
	const byte PrimitiveTypeByte = 2;       // PrimitiveTypeEnumeration.Byte per [MS-NRBF] §2.1.2.3 (Boolean=1, Byte=2)

	/// <summary>
	/// Emits the smallest valid NRBF stream representing a single
	/// <c>SerializationInfo</c>-style class with one <c>byte[]</c> member. Mirrors what
	/// BinaryFormatter on .NET Framework would have produced for an ImageListStreamer.
	/// </summary>
	public static byte[] BuildNrbfClassWithByteArrayMember(string className, string libraryName, string memberName, byte[] data)
	{
		const int rootObjectId = 1;
		const int arrayObjectId = 2;
		const int libraryId = 3;

		using var ms = new MemoryStream();
		using var w = new BinaryWriter(ms);

		// SerializationHeaderRecord — §2.6.1.
		w.Write(RecSerializationHeader);
		w.Write(rootObjectId);
		w.Write(-1);                    // headerId — opaque on read
		w.Write(1);                     // majorVersion
		w.Write(0);                     // minorVersion

		// BinaryLibrary — §2.6.2. Must precede the ClassWithMembersAndTypes that
		// references it, even though logically the class "owns" the library reference.
		w.Write(RecBinaryLibrary);
		w.Write(libraryId);
		w.Write(libraryName);

		// ClassWithMembersAndTypes — §2.3.2.2. ClassInfo + MemberTypeInfo + LibraryId,
		// followed by member values.
		w.Write(RecClassWithMembersAndTypes);
		w.Write(rootObjectId);          // ClassInfo.ObjectId
		w.Write(className);             // ClassInfo.Name
		w.Write(1);                     // ClassInfo.MemberCount
		w.Write(memberName);            // ClassInfo.MemberNames[0]
		w.Write(BinaryTypePrimitiveArray);  // MemberTypeInfo.BinaryTypeEnums[0]
		w.Write(PrimitiveTypeByte);     // MemberTypeInfo.AdditionalInfos[0]
		w.Write(libraryId);             // LibraryId

		// Member value for a PrimitiveArray member: MemberReference pointing at the
		// ArraySinglePrimitive record that follows. Inline-array would also be legal
		// per §2.7 but MemberReference is the canonical BinaryFormatter shape.
		w.Write(RecMemberReference);
		w.Write(arrayObjectId);

		// ArraySinglePrimitive — §2.4.3.3.
		w.Write(RecArraySinglePrimitive);
		w.Write(arrayObjectId);
		w.Write(data.Length);
		w.Write(PrimitiveTypeByte);
		w.Write(data);

		// MessageEnd — §2.6.3.
		w.Write(RecMessageEnd);

		return ms.ToArray();
	}

	/// <summary>
	/// Materialises a frame's ARGB pixels into a top-down BGRA byte[] that matches the
	/// shape the decoder returns — for byte-by-byte comparison in the tests.
	/// </summary>
	public static byte[] ExtractTopDownBgra(Bitmap frame)
	{
		var rect = new Rectangle(0, 0, frame.Width, frame.Height);
		var locked = frame.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
		try
		{
			int rowBytes = frame.Width * 4;
			var buffer = new byte[frame.Height * rowBytes];
			var row = new byte[Math.Max(locked.Stride, rowBytes)];
			for (int y = 0; y < frame.Height; y++)
			{
				System.Runtime.InteropServices.Marshal.Copy(locked.Scan0 + y * locked.Stride, row, 0, rowBytes);
				Buffer.BlockCopy(row, 0, buffer, y * rowBytes, rowBytes);
			}
			return buffer;
		}
		finally
		{
			frame.UnlockBits(locked);
		}
	}
}
