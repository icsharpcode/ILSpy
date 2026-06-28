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
using System.Runtime.InteropServices;

using ICSharpCode.Decompiler.Util;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Util
{
	// Exercises the bounds, recursion-depth and cycle guards in Win32Resources against crafted
	// .rsrc section bytes. Each test hands a hand-built directory tree to the parser through a
	// resolver that maps an RVA to an offset inside the same pinned buffer (a single-section PE).
	[TestFixture]
	public unsafe class Win32ResourcesTests
	{
		const int DirectorySize = 16;   // IMAGE_RESOURCE_DIRECTORY
		const int EntrySize = 8;        // IMAGE_RESOURCE_DIRECTORY_ENTRY
		const int DataEntrySize = 16;   // IMAGE_RESOURCE_DATA_ENTRY
		const uint SubdirectoryFlag = 0x80000000;

		// Pins the buffer, parses it as a resource section, and runs the assertions while the data
		// pointers captured during parsing still point into the pinned buffer.
		static void Parse(byte[] buffer, Action<Win32ResourceDirectory> assert)
		{
			var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
			try
			{
				byte* pRoot = (byte*)handle.AddrOfPinnedObject();
				var resolver = new BufferResolver(pRoot, buffer.Length);
				var root = Win32ResourceDirectory.ReadDirectoryTree(pRoot, buffer.Length, resolver.Resolve);
				assert(root);
			}
			finally
			{
				handle.Free();
			}
		}

		// Resolves a data RVA to a pointer inside the buffer, returning the bytes that remain from
		// that offset to the end - the same "length to end of section" contract PEReader.GetSectionData
		// provides, so a crafted Size larger than the data can be bounded.
		sealed class BufferResolver
		{
			readonly byte* pRoot;
			readonly int length;

			public BufferResolver(byte* pRoot, int length)
			{
				this.pRoot = pRoot;
				this.length = length;
			}

			public byte* Resolve(int rva, out int dataLength)
			{
				if (rva < 0 || rva > length)
				{
					dataLength = 0;
					return null;
				}
				dataLength = length - rva;
				return pRoot + rva;
			}
		}

		static void WriteDirectory(byte[] buffer, int offset, ushort namedEntries, ushort idEntries)
		{
			BitConverter.GetBytes(namedEntries).CopyTo(buffer, offset + 12);
			BitConverter.GetBytes(idEntries).CopyTo(buffer, offset + 14);
		}

		static void WriteEntry(byte[] buffer, int offset, uint name, uint offsetToData)
		{
			BitConverter.GetBytes(name).CopyTo(buffer, offset);
			BitConverter.GetBytes(offsetToData).CopyTo(buffer, offset + 4);
		}

		static void WriteDataEntry(byte[] buffer, int offset, uint rva, uint size)
		{
			BitConverter.GetBytes(rva).CopyTo(buffer, offset);
			BitConverter.GetBytes(size).CopyTo(buffer, offset + 4);
		}

		[Test]
		public void SelfReferentialSubdirectory_DoesNotRecurseInfinitely()
		{
			// One directory with a single subdirectory entry that points back at itself (offset 0).
			// The unfixed parser follows it forever, yielding an uncatchable StackOverflowException.
			byte[] buffer = new byte[DirectorySize + EntrySize];
			WriteDirectory(buffer, 0, namedEntries: 0, idEntries: 1);
			WriteEntry(buffer, DirectorySize, name: 1, offsetToData: SubdirectoryFlag /* offset 0 */);

			Parse(buffer, root => {
				Assert.That(root.Directories.Count, Is.EqualTo(1));
				var child = root.Directories[0];
				Assert.That(child.Directories.Count, Is.EqualTo(0), "the cycle back to the root must be cut");
				Assert.That(child.Datas.Count, Is.EqualTo(0));
			});
		}

		[Test]
		public void DeeplyNestedDirectories_AreBoundedByDepthLimit()
		{
			// A long chain of distinct nested directories. Even without a cycle this would recurse
			// as deep as the chain; the depth cap must stop it well before that.
			const int chainLength = 40;
			byte[] buffer = new byte[chainLength * (DirectorySize + EntrySize)];
			for (int k = 0; k < chainLength; k++)
			{
				int dirOffset = k * (DirectorySize + EntrySize);
				bool hasChild = k < chainLength - 1;
				WriteDirectory(buffer, dirOffset, namedEntries: 0, idEntries: (ushort)(hasChild ? 1 : 0));
				if (hasChild)
				{
					uint childOffset = (uint)((k + 1) * (DirectorySize + EntrySize));
					WriteEntry(buffer, dirOffset + DirectorySize, name: (uint)(k + 1), offsetToData: SubdirectoryFlag | childOffset);
				}
			}

			Parse(buffer, root => {
				int depth = 0;
				var current = root;
				while (current != null && current.Directories.Count > 0)
				{
					current = current.Directories[0];
					depth++;
				}
				// The parser caps nesting at a small constant (well above any real resource tree),
				// so the measured depth must be far below the crafted chain length.
				Assert.That(depth, Is.LessThanOrEqualTo(17));
			});
		}

		[Test]
		public void EntryCountBeyondSection_IsClamped()
		{
			// A directory header that claims far more entries than the section can hold. The unfixed
			// parser walks the declared count straight off the end of the section.
			byte[] buffer = new byte[DirectorySize];
			WriteDirectory(buffer, 0, namedEntries: 0, idEntries: 0xFFFF);

			Parse(buffer, root => {
				Assert.That(root.Directories.Count, Is.EqualTo(0));
				Assert.That(root.Datas.Count, Is.EqualTo(0));
			});
		}

		[Test]
		public void DataSizeBeyondSection_IsClampedToAvailable()
		{
			// A data leaf whose declared Size dwarfs the bytes actually present. The unfixed Data
			// getter copies the full Size, reading gigabytes past the section base.
			const int dataBytes = 8;
			int dataEntryOffset = DirectorySize + EntrySize;
			int dataOffset = dataEntryOffset + DataEntrySize;
			byte[] buffer = new byte[dataOffset + dataBytes];
			WriteDirectory(buffer, 0, namedEntries: 0, idEntries: 1);
			WriteEntry(buffer, DirectorySize, name: 1, offsetToData: (uint)dataEntryOffset /* data leaf */);
			WriteDataEntry(buffer, dataEntryOffset, rva: (uint)dataOffset, size: 0xFFFFFFF0);

			Parse(buffer, root => {
				Assert.That(root.Datas.Count, Is.EqualTo(1));
				var data = root.Datas[0];
				Assert.That(data.Size, Is.EqualTo(0xFFFFFFF0));
				Assert.That(data.Data.Length, Is.EqualTo(dataBytes), "the copy must be bounded to the bytes that exist");
			});
		}

		[Test]
		public void NegativeDataRva_YieldsEmptyDataWithoutThrowing()
		{
			// The data entry's RVA is a file uint; with the high bit set it casts to a negative int.
			// The resolver must reject it (PEReader.GetSectionData throws on a negative RVA) so the
			// leaf yields empty data rather than aborting the parse.
			int dataEntryOffset = DirectorySize + EntrySize;
			byte[] buffer = new byte[dataEntryOffset + DataEntrySize];
			WriteDirectory(buffer, 0, namedEntries: 0, idEntries: 1);
			WriteEntry(buffer, DirectorySize, name: 1, offsetToData: (uint)dataEntryOffset /* data leaf */);
			WriteDataEntry(buffer, dataEntryOffset, rva: 0xFFFFFFFF /* negative as int */, size: 0x100);

			Parse(buffer, root => {
				Assert.That(root.Datas.Count, Is.EqualTo(1));
				Assert.That(root.Datas[0].Data, Is.Empty);
			});
		}

		[Test]
		public void OutOfRangeStringName_DoesNotReadOutOfBounds()
		{
			// A named entry whose name-string offset lies past the section end. The unfixed parser
			// dereferences it directly, reading the length prefix and characters out of bounds.
			int dataEntryOffset = DirectorySize + EntrySize;
			byte[] buffer = new byte[dataEntryOffset + DataEntrySize];
			WriteDirectory(buffer, 0, namedEntries: 1, idEntries: 0);
			WriteEntry(buffer, DirectorySize, name: SubdirectoryFlag | 0x100 /* string offset past the buffer */, offsetToData: (uint)dataEntryOffset);
			WriteDataEntry(buffer, dataEntryOffset, rva: 0, size: 0);

			Parse(buffer, root => {
				Assert.That(root.Datas.Count, Is.EqualTo(1));
				var name = root.Datas[0].Name;
				Assert.That(name.HasName, Is.True);
				Assert.That(name.Name, Is.Empty, "an out-of-range string name must resolve to empty, not an OOB read");
			});
		}

		[Test]
		public void ValidResourceTree_ParsesAndReadsData()
		{
			// A well-formed Type -> Name -> (language data leaf) tree, mirroring how a manifest is
			// laid out, to prove the bounds checks do not break normal parsing.
			const int RT_MANIFEST = 24;
			int typeDir = 0;
			int rootEntry = typeDir + DirectorySize;             // 16
			int nameDir = rootEntry + EntrySize;                 // 24
			int typeEntry = nameDir + DirectorySize;             // 40
			int leafDir = typeEntry + EntrySize;                 // 48
			int nameEntry = leafDir + DirectorySize;             // 64
			int dataEntry = nameEntry + EntrySize;               // 72
			int dataOffset = dataEntry + DataEntrySize;          // 88
			byte[] payload = { 0xDE, 0xAD, 0xBE, 0xEF };
			byte[] buffer = new byte[dataOffset + payload.Length];

			WriteDirectory(buffer, typeDir, namedEntries: 0, idEntries: 1);
			WriteEntry(buffer, rootEntry, name: RT_MANIFEST, offsetToData: SubdirectoryFlag | (uint)nameDir);
			WriteDirectory(buffer, nameDir, namedEntries: 0, idEntries: 1);
			WriteEntry(buffer, typeEntry, name: 1, offsetToData: SubdirectoryFlag | (uint)leafDir);
			WriteDirectory(buffer, leafDir, namedEntries: 0, idEntries: 1);
			WriteEntry(buffer, nameEntry, name: 1033, offsetToData: (uint)dataEntry /* data leaf */);
			WriteDataEntry(buffer, dataEntry, rva: (uint)dataOffset, size: (uint)payload.Length);
			payload.CopyTo(buffer, dataOffset);

			Parse(buffer, root => {
				var manifest = root.Find(new Win32ResourceName(RT_MANIFEST))?.FirstDirectory()?.FirstData()?.Data;
				Assert.That(manifest, Is.Not.Null);
				Assert.That(manifest, Is.EqualTo(payload));
			});
		}
	}
}
