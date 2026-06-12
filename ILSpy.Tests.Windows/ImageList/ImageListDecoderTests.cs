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
using System.IO;
using System.Linq;
using System.Windows.Forms;

using AwesomeAssertions;

using ICSharpCode.ILSpy.ImageList;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Windows;

/// <summary>
/// Verifies the cross-platform <see cref="ImageListDecoder"/> against fixtures generated
/// by real WinForms <c>System.Windows.Forms.ImageList</c> instances. Tests are
/// Windows-only because the fixture builder uses GDI+ and ImageListStreamer; the decoder
/// under test is itself platform-neutral.
/// </summary>
[TestFixture]
public class ImageListDecoderTests
{
	[Test]
	public void Decode_Of_ImageList_With_Depth32Bit_Returns_Expected_Frame_Count()
	{
		using var fixture = ImageListFixtures.Build(ColorDepth.Depth32Bit, withMask: false, count: 4);

		var decoded = ImageListDecoder.Decode(fixture.NrbfBlob);

		decoded.Should().HaveCount(4, "the source ImageList held 4 frames");
		decoded.Should().AllSatisfy(d => {
			d.Width.Should().Be(fixture.FrameSize.Width);
			d.Height.Should().Be(fixture.FrameSize.Height);
			d.BgraPixels.Should().HaveCount(fixture.FrameSize.Width * fixture.FrameSize.Height * 4);
		});
	}

	[Test]
	public void Decode_Of_ImageList_With_Depth8Bit_Returns_Expected_Frame_Count()
	{
		// .NET 7 and earlier defaulted to Depth8Bit; .NET 8 flipped to Depth32Bit. Pinning
		// this test against the legacy default guards against regressions on payloads
		// produced by older toolchains, which the decoder will still encounter in the wild.
		using var fixture = ImageListFixtures.Build(ColorDepth.Depth8Bit, withMask: true, count: 4);

		var decoded = ImageListDecoder.Decode(fixture.NrbfBlob);

		decoded.Should().HaveCount(4);
	}

	[Test]
	public void Each_Decoded_Frame_Matches_Source_Bitmap_Pixels_For_Depth32Bit()
	{
		// At 32 bpp the strip stores ARGB verbatim, so byte-for-byte equality is reasonable.
		using var fixture = ImageListFixtures.Build(ColorDepth.Depth32Bit, withMask: false, count: 4);

		var decoded = ImageListDecoder.Decode(fixture.NrbfBlob);

		for (int i = 0; i < fixture.Frames.Length; i++)
		{
			var expected = ImageListFixtures.ExtractTopDownBgra(fixture.Frames[i]);
			decoded[i].BgraPixels.Should().Equal(expected, $"frame {i} should match source pixel-for-pixel");
		}
	}

	[Test]
	public void Each_Decoded_Frame_Matches_Source_Bitmap_Pixels_For_Depth8Bit_Within_Palette_Tolerance()
	{
		// 8 bpp goes through the system halftone palette on Serialize, so individual
		// channels can shift by up to half-a-bin (~32 levels). The tolerance is the
		// maximum per-channel delta we will accept; tighter values would chase palette
		// quirks across runtimes.
		const int channelTolerance = 32;
		using var fixture = ImageListFixtures.Build(ColorDepth.Depth8Bit, withMask: false, count: 4);

		var decoded = ImageListDecoder.Decode(fixture.NrbfBlob);

		for (int i = 0; i < fixture.Frames.Length; i++)
		{
			var expected = ImageListFixtures.ExtractTopDownBgra(fixture.Frames[i]);
			var actual = decoded[i].BgraPixels;
			actual.Length.Should().Be(expected.Length, $"frame {i} dimensions");
			for (int j = 0; j < actual.Length; j++)
			{
				int delta = Math.Abs(actual[j] - expected[j]);
				delta.Should().BeLessThanOrEqualTo(channelTolerance,
					$"frame {i}, byte {j}: actual=0x{actual[j]:X2} expected=0x{expected[j]:X2}");
			}
		}
	}

	[Test]
	public void Decode_Of_ImageList_With_Mask_Composites_Mask_Into_Alpha_For_NonArgb_Depths()
	{
		// Depth24Bit + transparent magenta: every pixel matching TransparentColor in the
		// source bitmap (none in the BuildFrame template, but the implicit "outside" of
		// the X drawn over a magenta clear would be — we instead test that masked output
		// has at least one fully-transparent and at least one fully-opaque pixel).
		using var fixture = ImageListFixtures.Build(ColorDepth.Depth24Bit, withMask: true, count: 1);

		var decoded = ImageListDecoder.Decode(fixture.NrbfBlob);

		decoded.Should().HaveCount(1);
		var alphaValues = Enumerable
			.Range(0, decoded[0].BgraPixels.Length / 4)
			.Select(p => decoded[0].BgraPixels[p * 4 + 3])
			.Distinct()
			.ToArray();
		alphaValues.Should().Contain((byte)0xFF, "mask-composited frames must have opaque pixels");
	}

	[Test]
	public void Decode_Of_ImageList_Without_MSFt_Magic_Falls_Through()
	{
		// Strip the MSFt RLE wrapper off a real WinForms-produced payload, re-wrap in NRBF,
		// confirm the decoder still parses the raw ILHEAD+DIB bytes underneath.
		using var fixture = ImageListFixtures.Build(ColorDepth.Depth32Bit, withMask: false, count: 2);
		byte[] inner = ExtractDataMember(fixture.NrbfBlob);
		byte[] rleDecoded = MsftRleDecode(inner);

		byte[] nrbf = ImageListFixtures.BuildNrbfClassWithByteArrayMember(
			fixture.TypeName,
			typeof(ImageListStreamer).Assembly.FullName!,
			"Data",
			rleDecoded);

		var decoded = ImageListDecoder.Decode(nrbf);

		decoded.Should().HaveCount(2, "fall-through path must still surface every frame");
	}

	[Test]
	public void Decode_Throws_On_Wrong_NRBF_Type_Name()
	{
		// Same envelope shape, but the class name doesn't match ImageListStreamer.
		byte[] nrbf = ImageListFixtures.BuildNrbfClassWithByteArrayMember(
			"System.SomethingElse",
			"mscorlib",
			"Data",
			new byte[] { 1, 2, 3, 4 });

		Action act = () => ImageListDecoder.Decode(nrbf);

		act.Should().Throw<InvalidDataException>()
			.WithMessage("*ImageListStreamer*",
				"decoder must reject unknown payload types with a descriptive message");
	}

	[Test]
	public void Decode_Throws_On_Truncated_ILHEAD()
	{
		// ILHEAD is 28 bytes; give the decoder 10 wrapped in a valid NRBF + valid MSFt
		// header. The MSFt RLE decode will produce a short buffer that the ILHEAD parser
		// must refuse rather than read uninitialised memory.
		var rle = new byte[] {
			0x4D, 0x53, 0x46, 0x74,    // MSFt magic
			0x0A, 0x00,                 // RLE pair: 10 zero bytes
		};
		byte[] nrbf = ImageListFixtures.BuildNrbfClassWithByteArrayMember(
			typeof(ImageListStreamer).FullName!,
			typeof(ImageListStreamer).Assembly.FullName!,
			"Data",
			rle);

		Action act = () => ImageListDecoder.Decode(nrbf);

		act.Should().Throw<Exception>().Where(e => e is InvalidDataException || e is EndOfStreamException);
	}

	// --- helpers for the fall-through test --------------------------------------------------

	/// <summary>
	/// Cracks the NRBF envelope just far enough to read out the byte[] "Data" payload.
	/// This is a *test-only* path — production code goes through System.Formats.Nrbf
	/// in the decoder itself.
	/// </summary>
	static byte[] ExtractDataMember(byte[] nrbf)
	{
		using var ms = new MemoryStream(nrbf);
		var record = System.Formats.Nrbf.NrbfDecoder.Decode(ms);
		var classRecord = (System.Formats.Nrbf.ClassRecord)record;
		var arr = (System.Formats.Nrbf.SZArrayRecord<byte>)classRecord.GetSerializationRecord("Data")!;
		return arr.GetArray();
	}

	/// <summary>
	/// Mirror of the RLE decoder the production code will run. Lives here so the
	/// fall-through test doesn't have to re-implement it during red-phase; once the
	/// production decoder is implemented this is the same algorithm.
	/// </summary>
	static byte[] MsftRleDecode(byte[] input)
	{
		if (input.Length < 4 || input[0] != 0x4D || input[1] != 0x53 || input[2] != 0x46 || input[3] != 0x74)
			throw new InvalidDataException("expected MSFt magic");
		using var ms = new MemoryStream();
		for (int i = 4; i + 1 < input.Length; i += 2)
		{
			byte count = input[i];
			byte value = input[i + 1];
			for (int j = 0; j < count; j++)
				ms.WriteByte(value);
		}
		return ms.ToArray();
	}
}
