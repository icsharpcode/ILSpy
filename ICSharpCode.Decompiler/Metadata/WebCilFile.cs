// Copyright (c) 2024 Siegfried Pammer
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Reflection.Metadata;
using System.Text;

using ICSharpCode.Decompiler.TypeSystem;

#nullable enable

namespace ICSharpCode.Decompiler.Metadata
{
	public sealed class WebCilFile : MetadataFile, IModuleReference
	{
		readonly MemoryMappedViewAccessor view;
		readonly long webcilOffset;
		readonly long viewLength;
		unsafe byte* basePointer;

		private unsafe WebCilFile(string fileName, long webcilOffset, long metadataOffset, MemoryMappedViewAccessor view, ImmutableArray<SectionHeader> sectionHeaders, ImmutableArray<WasmSection> wasmSections, MetadataReaderProvider provider, MetadataReaderOptions metadataOptions = MetadataReaderOptions.Default)
			: base(MetadataFileKind.WebCIL, fileName, provider, metadataOptions, 0)
		{
			this.webcilOffset = webcilOffset;
			this.MetadataOffset = (int)metadataOffset;
			this.view = view;
			this.viewLength = checked((long)view.SafeMemoryMappedViewHandle.ByteLength);
			// Pin the mapped view once for the lifetime of this instance; SectionData hands out raw
			// pointers into it, so the pointer must stay valid until Dispose releases it.
			byte* ptr = null;
			view.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
			this.basePointer = ptr;
			this.SectionHeaders = sectionHeaders;
			this.WasmSections = wasmSections;
		}

		public static WebCilFile? FromFile(string fileName, MetadataReaderOptions metadataOptions = MetadataReaderOptions.Default)
		{
			using var memoryMappedFile = TryCreateFromFile(fileName);
			if (memoryMappedFile == null)
			{
				return null;
			}

			var view = memoryMappedFile.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
			try
			{
				// read magic "\0asm"
				if (view.ReadUInt32(0) != WASM_MAGIC)
					return null;

				// read version
				if (view.ReadUInt32(4) != 1)
					return null;

				using var stream = view.AsStream();
				using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);

				stream.Position += 8;

				long metadataOffset = -1;
				List<WasmSection> sections = new List<WasmSection>();

				while (stream.Position < stream.Length)
				{
					WasmSectionId id = (WasmSectionId)reader.ReadByte();
					uint size = reader.ReadULEB128();
					sections.Add(new WasmSection(id, stream.Position, size, view));

					if (id == WasmSectionId.Custom && size == 0)
					{
						break;
					}
					stream.Seek(size, SeekOrigin.Current);
				}

				foreach (var section in sections)
				{
					if (section.Id != WasmSectionId.Data || metadataOffset > -1)
						continue;

					stream.Seek(section.Offset, SeekOrigin.Begin);

					uint numSegments = reader.ReadULEB128();
					if (numSegments != 2)
						continue;

					// skip the first segment
					if (reader.ReadByte() != 1)
						continue;

					long segmentLength = reader.ReadULEB128();
					long segmentStart = reader.BaseStream.Position;

					reader.BaseStream.Seek(segmentLength, SeekOrigin.Current);

					if (reader.ReadByte() != 1)
						continue;

					segmentLength = reader.ReadULEB128();
					if (TryReadWebCilSegment(reader, out var header, out metadataOffset, out var webcilOffset, out var sectionHeaders))
					{
						stream.Seek(metadataOffset, SeekOrigin.Begin);
						var metadata = MetadataReaderProvider.FromMetadataStream(stream, MetadataStreamOptions.LeaveOpen | MetadataStreamOptions.PrefetchMetadata);

						var result = new WebCilFile(fileName, webcilOffset, metadataOffset, view, ImmutableArray.Create(sectionHeaders), sections.ToImmutableArray(), metadata, metadataOptions);

						view = null; // don't dispose the view, we're still using it in the sections
						return result;
					}
				}

				return null;
			}
			catch (Exception ex) when (ex is EndOfStreamException or OverflowException or BadImageFormatException)
			{
				// A crafted or truncated module can drive the structural reads (Wasm sections, data
				// segments, the WebCIL header and RVA translation) past the mapped view or produce an
				// invalid metadata stream. Treat any such file as "not a WebCIL file" instead of
				// letting the exception escape the loader.
				return null;
			}
			finally
			{
				view?.Dispose();
			}

			static MemoryMappedFile? TryCreateFromFile(string fileName)
			{
				try
				{
					return MemoryMappedFile.CreateFromFile(fileName, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
				}
				catch (IOException)
				{
					return null;
				}
			}
		}

		static unsafe bool TryReadWebCilSegment(BinaryReader reader, out WebcilHeader webcilHeader, out long metadataOffset, out long webcilOffset, [NotNullWhen(true)] out SectionHeader[]? sectionHeaders)
		{
			webcilHeader = default;
			metadataOffset = -1;
			sectionHeaders = null;

			webcilOffset = reader.BaseStream.Position;

			if (reader.ReadUInt32() != WEBCIL_MAGIC)
				return false;

			webcilHeader.VersionMajor = reader.ReadUInt16();
			webcilHeader.VersionMinor = reader.ReadUInt16();
			webcilHeader.CoffSections = reader.ReadUInt16();
			_ = reader.ReadUInt16(); // reserved0
			webcilHeader.PECliHeaderRVA = reader.ReadUInt32();
			webcilHeader.PECliHeaderSize = reader.ReadUInt32();
			webcilHeader.PEDebugRVA = reader.ReadUInt32();
			webcilHeader.PEDebugSize = reader.ReadUInt32();

			sectionHeaders = new SectionHeader[webcilHeader.CoffSections];
			for (int i = 0; i < webcilHeader.CoffSections; i++)
			{
				sectionHeaders[i].VirtualSize = reader.ReadUInt32();
				sectionHeaders[i].VirtualAddress = reader.ReadUInt32();
				sectionHeaders[i].RawDataSize = reader.ReadUInt32();
				sectionHeaders[i].RawDataPtr = reader.ReadUInt32();
			}

			long corHeaderStart = TranslateRVA(sectionHeaders, webcilOffset, webcilHeader.PECliHeaderRVA);
			if (reader.BaseStream.Seek(corHeaderStart, SeekOrigin.Begin) != corHeaderStart)
				return false;
			int byteCount = reader.ReadInt32();
			int majorVersion = reader.ReadUInt16();
			int minorVersion = reader.ReadUInt16();
			metadataOffset = TranslateRVA(sectionHeaders, webcilOffset, (uint)reader.ReadInt32());
			return reader.BaseStream.Seek(metadataOffset, SeekOrigin.Begin) == metadataOffset;
		}

		public override int MetadataOffset { get; }
		public override bool IsMetadataOnly => false;

		private static int GetContainingSectionIndex(IEnumerable<SectionHeader> sections, int rva)
		{
			int i = 0;
			foreach (var section in sections)
			{
				if (rva >= section.VirtualAddress && rva < (long)section.VirtualAddress + section.VirtualSize)
				{
					return i;
				}
				i++;
			}
			return -1;
		}

		private static long TranslateRVA(IEnumerable<SectionHeader> sections, long webcilOffset, uint rva)
		{
			foreach (var section in sections)
			{
				if (rva >= section.VirtualAddress && rva < (long)section.VirtualAddress + section.VirtualSize)
				{
					return (long)section.RawDataPtr + (rva - section.VirtualAddress) + webcilOffset;
				}
			}
			throw new BadImageFormatException("RVA not found in any section");
		}

		// Resolves an RVA to the (file offset, length) of its section's raw data inside the mapped
		// view, or returns false when no section contains the RVA or the raw-data range falls outside
		// the view. All arithmetic is widened to long so crafted uint header fields cannot wrap the
		// range check or narrow into a length that looks valid.
		internal static bool TryGetSectionDataRange(ImmutableArray<SectionHeader> sectionHeaders, long webcilOffset, long viewLength, int rva, out long offset, out int length)
		{
			offset = 0;
			length = 0;
			foreach (var section in sectionHeaders)
			{
				if (rva >= section.VirtualAddress && rva < (long)section.VirtualAddress + section.VirtualSize)
				{
					long delta = (long)rva - section.VirtualAddress;
					long o = (long)section.RawDataPtr + webcilOffset + delta;
					// Length is the raw data remaining from the RVA to the end of the section, matching
					// PEReader.GetSectionData (callers such as GetInitialValue treat SectionData.Length
					// as the bytes available starting at the RVA). A section whose virtual size exceeds
					// its raw data can place the RVA past the raw bytes, leaving nothing to read.
					long remaining = (long)section.RawDataSize - delta;
					if (o < 0 || remaining < 0 || remaining > int.MaxValue || o > viewLength || remaining > viewLength - o)
						return false;
					offset = o;
					length = (int)remaining;
					return true;
				}
			}
			return false;
		}

		public override MethodBodyBlock GetMethodBody(int rva)
		{
			var reader = GetSectionData(rva).GetReader();
			return MethodBodyBlock.Create(reader);
		}

		public override int GetContainingSectionIndex(int rva)
		{
			return GetContainingSectionIndex(SectionHeaders, rva);
		}

		public override unsafe SectionData GetSectionData(int rva)
		{
			if (TryGetSectionDataRange(SectionHeaders, webcilOffset, viewLength, rva, out long offset, out int length))
			{
				return new SectionData(basePointer + offset, length);
			}
			throw new BadImageFormatException("RVA not found in any section");
		}

		public override ImmutableArray<SectionHeader> SectionHeaders { get; }

		public ImmutableArray<WasmSection> WasmSections { get; }

		IModule? IModuleReference.Resolve(ITypeResolveContext context)
		{
			return new MetadataModule(context.Compilation, this, TypeSystemOptions.Default);
		}

		protected override unsafe void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (basePointer != null)
				{
					view.SafeMemoryMappedViewHandle.ReleasePointer();
					basePointer = null;
				}
				view.Dispose();
			}
			base.Dispose(disposing);
		}

		public struct WebcilHeader
		{
			public ushort VersionMajor;
			public ushort VersionMinor;
			public ushort CoffSections;
			public uint PECliHeaderRVA;
			public uint PECliHeaderSize;
			public uint PEDebugRVA;
			public uint PEDebugSize;
		}

		const uint WASM_MAGIC = 0x6d736100u; // "\0asm"
		const uint WEBCIL_MAGIC = 0x4c496257u; // "WbIL"

		[DebuggerDisplay("WasmSection {Id}: {Offset} {Size}")]
		public class WasmSection
		{
			public WasmSectionId Id;
			public long Offset;
			public uint Size;
			private MemoryMappedViewAccessor view;

			public WasmSection(WasmSectionId id, long offset, uint size, MemoryMappedViewAccessor view)
			{
				this.Id = id;
				this.Size = size;
				this.Offset = offset;
				this.view = view;
			}
		}

		public enum WasmSectionId : byte
		{
			// order matters: enum values must match the WebAssembly spec
			Custom = 0,
			Type = 1,
			Import = 2,
			Function = 3,
			Table = 4,
			Memory = 5,
			Global = 6,
			Export = 7,
			Start = 8,
			Element = 9,
			Code = 10,
			Data = 11,
			DataCount = 12,
		}
	}
}
