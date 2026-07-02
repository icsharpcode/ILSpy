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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;

using ICSharpCode.Decompiler.Metadata;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Metadata
{
	// Exercises WebCilFile against crafted Wasm/WebCIL input. The section-header fields driving
	// the memory-mapped pointer arithmetic come straight from the file, so a crafted module must
	// be rejected rather than reading outside the mapped view (CWE-125) or crashing the loader.
	[TestFixture]
	public class WebCilFileTests
	{
		const uint WASM_MAGIC = 0x6d736100u;   // "\0asm"
		const uint WEBCIL_MAGIC = 0x4c496257u; // "WbIL"

		struct SectionHeaderRaw
		{
			public uint VirtualSize;
			public uint VirtualAddress;
			public uint RawDataSize;
			public uint RawDataPtr;
		}

		static void WriteULEB128(List<byte> buffer, uint value)
		{
			do
			{
				byte b = (byte)(value & 0x7F);
				value >>= 7;
				if (value != 0)
					b |= 0x80;
				buffer.Add(b);
			}
			while (value != 0);
		}

		static byte[] BuildWebcilPayload(ushort coffSections, uint peCliHeaderRVA, SectionHeaderRaw[] sections)
		{
			using var ms = new MemoryStream();
			using var w = new BinaryWriter(ms);
			w.Write(WEBCIL_MAGIC);
			w.Write((ushort)0); // VersionMajor
			w.Write((ushort)0); // VersionMinor
			w.Write(coffSections);
			w.Write((ushort)0); // reserved0
			w.Write(peCliHeaderRVA);
			w.Write(0u);        // PECliHeaderSize
			w.Write(0u);        // PEDebugRVA
			w.Write(0u);        // PEDebugSize
			foreach (var s in sections)
			{
				w.Write(s.VirtualSize);
				w.Write(s.VirtualAddress);
				w.Write(s.RawDataSize);
				w.Write(s.RawDataPtr);
			}
			w.Flush();
			return ms.ToArray();
		}

		// Wraps a WebCIL payload in the minimal Wasm container WebCilFile.FromFile recognizes:
		// a Data section whose two data segments are the (skipped) first segment and the WebCIL blob.
		static byte[] BuildWasmContainer(byte[] webcilPayload)
		{
			var content = new List<byte>();
			WriteULEB128(content, 2);                          // number of data segments
			content.Add(1);                                    // segment 1 kind
			WriteULEB128(content, 0);                          // segment 1 length
			content.Add(1);                                    // segment 2 kind
			WriteULEB128(content, (uint)webcilPayload.Length); // segment 2 length
			content.AddRange(webcilPayload);

			var file = new List<byte>();
			file.AddRange(BitConverter.GetBytes(WASM_MAGIC));
			file.AddRange(BitConverter.GetBytes(1u));          // Wasm version
			file.Add(11);                                      // WasmSectionId.Data
			WriteULEB128(file, (uint)content.Count);
			file.AddRange(content);
			return file.ToArray();
		}

		static WebCilFile FromBytes(byte[] bytes)
		{
			string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".wasm");
			File.WriteAllBytes(path, bytes);
			try
			{
				return WebCilFile.FromFile(path);
			}
			finally
			{
				File.Delete(path);
			}
		}

		[Test]
		public void TruncatedSectionHeaders_ReturnsNullWithoutReadingPastView()
		{
			// CoffSections claims far more section headers than the file (or mapped view) holds.
			byte[] payload = BuildWebcilPayload(coffSections: 0xFFFF, peCliHeaderRVA: 0, sections: Array.Empty<SectionHeaderRaw>());
			Assert.That(FromBytes(BuildWasmContainer(payload)), Is.Null);
		}

		[Test]
		public void CliHeaderRvaInNoSection_ReturnsNull()
		{
			var sections = new[] {
				new SectionHeaderRaw { VirtualAddress = 0, VirtualSize = 0x10, RawDataSize = 0x10, RawDataPtr = 0 }
			};
			// PECliHeaderRVA points outside the single section, so RVA translation cannot resolve it.
			byte[] payload = BuildWebcilPayload(coffSections: 1, peCliHeaderRVA: 0x1000, sections: sections);
			Assert.That(FromBytes(BuildWasmContainer(payload)), Is.Null);
		}

		static ImmutableArray<SectionHeader> Section(uint virtualAddress, uint virtualSize, uint rawDataPtr, uint rawDataSize)
		{
			return ImmutableArray.Create(new SectionHeader {
				VirtualAddress = virtualAddress,
				VirtualSize = virtualSize,
				RawDataPtr = rawDataPtr,
				RawDataSize = rawDataSize
			});
		}

		[Test]
		public void OobSectionHeader_IsRejected()
		{
			// The exploit shape from the finding: a section spanning the whole RVA space whose raw-data
			// pointer/size point far outside a small mapped view.
			var headers = Section(virtualAddress: 0, virtualSize: 0xFFFFFFFF, rawDataPtr: 0xFFFFFFFF, rawDataSize: 0x7FFFFFFF);
			bool ok = WebCilFile.TryGetSectionDataRange(headers, webcilOffset: 0, viewLength: 0x1000, rva: 0x10, out _, out _);
			Assert.That(ok, Is.False);
		}

		[Test]
		public void RawDataSizeBeyondInt32_IsRejected()
		{
			var headers = Section(virtualAddress: 0, virtualSize: 0x100, rawDataPtr: 0, rawDataSize: 0x80000000);
			bool ok = WebCilFile.TryGetSectionDataRange(headers, webcilOffset: 0, viewLength: 0x100000000L, rva: 0, out _, out _);
			Assert.That(ok, Is.False);
		}

		[Test]
		public void RawDataRangePastView_IsRejected()
		{
			var headers = Section(virtualAddress: 0, virtualSize: 0x100, rawDataPtr: 0xF0, rawDataSize: 0x20);
			bool ok = WebCilFile.TryGetSectionDataRange(headers, webcilOffset: 0, viewLength: 0x100, rva: 0, out _, out _);
			Assert.That(ok, Is.False);
		}

		[Test]
		public void RvaInNoSection_IsRejected()
		{
			var headers = Section(virtualAddress: 0x10, virtualSize: 0x10, rawDataPtr: 0, rawDataSize: 0x10);
			bool ok = WebCilFile.TryGetSectionDataRange(headers, webcilOffset: 0, viewLength: 0x1000, rva: 0x100, out _, out _);
			Assert.That(ok, Is.False);
		}

		[Test]
		public void RvaPastRawData_IsRejected()
		{
			// The RVA sits inside the section's virtual size but past its (smaller) raw data, so there
			// are no bytes to read from it.
			var headers = Section(virtualAddress: 0, virtualSize: 0x100, rawDataPtr: 0, rawDataSize: 0x10);
			bool ok = WebCilFile.TryGetSectionDataRange(headers, webcilOffset: 0, viewLength: 0x1000, rva: 0x20, out _, out _);
			Assert.That(ok, Is.False);
		}

		[Test]
		public void InBoundsSection_IsResolved()
		{
			var headers = Section(virtualAddress: 0x20, virtualSize: 0x40, rawDataPtr: 0x80, rawDataSize: 0x40);
			bool ok = WebCilFile.TryGetSectionDataRange(headers, webcilOffset: 0x10, viewLength: 0x1000, rva: 0x30, out long offset, out int length);
			Assert.That(ok, Is.True);
			// offset = RawDataPtr + webcilOffset + (rva - VirtualAddress) = 0x80 + 0x10 + 0x10
			Assert.That(offset, Is.EqualTo(0xA0));
			// length = remaining raw data from the RVA = RawDataSize - (rva - VirtualAddress) = 0x40 - 0x10
			Assert.That(length, Is.EqualTo(0x30));
		}
	}
}
