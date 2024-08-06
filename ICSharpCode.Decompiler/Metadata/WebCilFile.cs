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
	public class WebCilFile : MetadataFile, IDisposable, IModuleReference
	{
		readonly MemoryMappedViewAccessor view;
		readonly long webcilOffset;

		private WebCilFile(string fileName, long webcilOffset, long metadataOffset, MemoryMappedViewAccessor view, ImmutableArray<SectionHeader> sectionHeaders, ImmutableArray<WasmSection> wasmSections, MetadataReaderProvider provider, MetadataReaderOptions metadataOptions = MetadataReaderOptions.Default)
			: base(MetadataFileKind.WebCIL, fileName, provider, metadataOptions, 0)
		{
			this.webcilOffset = webcilOffset;
			this.MetadataOffset = (int)metadataOffset;
			this.view = view;
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
				if (rva >= section.VirtualAddress && rva < section.VirtualAddress + section.VirtualSize)
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
				if (rva >= section.VirtualAddress && rva < section.VirtualAddress + section.VirtualSize)
				{
					return section.RawDataPtr + (rva - section.VirtualAddress) + webcilOffset;
				}
			}
			throw new BadImageFormatException("RVA not found in any section");
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
			foreach (var section in SectionHeaders)
			{
				if (rva >= section.VirtualAddress && rva < section.VirtualAddress + section.VirtualSize)
				{
					byte* ptr = (byte*)0;
					view.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
					return new SectionData(ptr + section.RawDataPtr + webcilOffset + (rva - section.VirtualAddress), (int)section.RawDataSize);
				}
			}
			throw new BadImageFormatException("RVA not found in any section");
		}

		public override ImmutableArray<SectionHeader> SectionHeaders { get; }

		public ImmutableArray<WasmSection> WasmSections { get; }

		IModule? IModuleReference.Resolve(ITypeResolveContext context)
		{
			return new MetadataModule(context.Compilation, this, TypeSystemOptions.Default);
		}

		public void Dispose()
		{
			view.Dispose();
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
