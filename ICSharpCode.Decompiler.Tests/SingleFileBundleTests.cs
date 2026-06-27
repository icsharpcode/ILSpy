// Copyright (c) 2026 Siegfried Pammer
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

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	[TestFixture]
	public class SingleFileBundleTests
	{
		// The 32-byte bundle signature (SHA-256 of ".net core bundle"), as in SingleFileBundle.IsBundle.
		static readonly byte[] Signature = new byte[] {
			0x8b, 0x12, 0x02, 0xb9, 0x6a, 0x61, 0x20, 0x38,
			0x72, 0x7b, 0x93, 0x02, 0x14, 0xd7, 0xa0, 0x32,
			0x13, 0xf5, 0xb9, 0xe6, 0xef, 0xae, 0x33, 0x18,
			0xee, 0x3b, 0x2d, 0xce, 0x24, 0xb3, 0x6a, 0xae
		};

		[Test]
		public unsafe void IsBundle_SignatureAtStart_DoesNotReadBeforeBuffer()
		{
			// A genuine bundle stores the 8-byte header offset immediately before the signature.
			// A crafted file can instead place the signature at the very start, leaving no such
			// bytes. The buffer below puts a sentinel where that backward read would land: if the
			// scanner reads before the data pointer it returns the sentinel as a (valid-looking)
			// header offset. The guard must reject the match without performing that read.
			byte[] buffer = new byte[8 + Signature.Length + 64];
			long sentinel = 0x29; // > 0 and < size, so an out-of-bounds read would look like a valid offset
			BitConverter.GetBytes(sentinel).CopyTo(buffer, 0);
			Signature.CopyTo(buffer, 8);

			fixed (byte* p = buffer)
			{
				byte* data = p + 8; // signature now sits at offset 0 of the region we hand to IsBundle
				long size = buffer.Length - 8;
				bool result = SingleFileBundle.IsBundle(data, size, out long headerOffset);

				Assert.That(result, Is.False, "signature at file offset 0 must not be accepted as a bundle");
				Assert.That(headerOffset, Is.EqualTo(0));
			}
		}

		[Test]
		public unsafe void IsBundle_ValidLayout_DetectsBundle()
		{
			// Mirrors a real bundle: the signature is preceded by its 8-byte header offset.
			const long expectedOffset = 24;
			byte[] buffer = new byte[8 + 8 + Signature.Length + 8]; // pad + headerOffset + signature + trailing
			int headerOffsetPos = 8;
			int signaturePos = headerOffsetPos + 8;
			BitConverter.GetBytes(expectedOffset).CopyTo(buffer, headerOffsetPos);
			Signature.CopyTo(buffer, signaturePos);

			fixed (byte* p = buffer)
			{
				bool result = SingleFileBundle.IsBundle(p, buffer.Length, out long headerOffset);

				Assert.That(result, Is.True, "a well-formed bundle signature must still be detected");
				Assert.That(headerOffset, Is.EqualTo(expectedOffset));
			}
		}

		[Test]
		public void ReadManifest_HugeFileCount_Throws()
		{
			// A crafted manifest declaring a huge FileCount must be rejected before it is used to
			// pre-size the entry array, rather than attempting a multi-gigabyte allocation.
			using var ms = new MemoryStream();
			using (var writer = new BinaryWriter(ms, System.Text.Encoding.UTF8, leaveOpen: true))
			{
				writer.Write((uint)1);          // MajorVersion (v1: no extended header block)
				writer.Write((uint)0);          // MinorVersion
				writer.Write(int.MaxValue);     // FileCount - far more entries than the stream can hold
			}
			ms.Position = 0;

			Assert.Throws<InvalidDataException>(() => SingleFileBundle.ReadManifest(ms));
		}
	}
}
