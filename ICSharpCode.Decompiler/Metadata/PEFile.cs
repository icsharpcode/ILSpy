// Copyright (c) 2018 Siegfried Pammer
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

#nullable enable

using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;

using ICSharpCode.Decompiler.TypeSystem;

namespace ICSharpCode.Decompiler.Metadata
{
	[DebuggerDisplay("{FileName}")]
	public class PEFile : MetadataFile, IDisposable, IModuleReference
	{
		public PEReader Reader { get; }

		public PEFile(string fileName, PEStreamOptions streamOptions = PEStreamOptions.Default, MetadataReaderOptions metadataOptions = MetadataReaderOptions.Default)
			: this(fileName, new PEReader(new FileStream(fileName, FileMode.Open, FileAccess.Read), streamOptions), metadataOptions)
		{
		}

		public PEFile(string fileName, Stream stream, PEStreamOptions streamOptions = PEStreamOptions.Default, MetadataReaderOptions metadataOptions = MetadataReaderOptions.Default)
			: this(fileName, new PEReader(stream, streamOptions), metadataOptions)
		{
		}

		public PEFile(string fileName, PEReader reader, MetadataReaderOptions metadataOptions = MetadataReaderOptions.Default)
			: base(MetadataFileKind.PortableExecutable, fileName, reader, metadataOptions)
		{
			this.Reader = reader;
		}

		public override bool IsEmbedded => false;
		public override int MetadataOffset => Reader.PEHeaders.MetadataStartOffset;
		public override bool IsMetadataOnly => false;

		public void Dispose()
		{
			Reader.Dispose();
		}

		IModule TypeSystem.IModuleReference.Resolve(ITypeResolveContext context)
		{
			return new MetadataModule(context.Compilation, this, TypeSystemOptions.Default);
		}

		public override MethodBodyBlock GetMethodBody(int rva)
		{
			return Reader.GetMethodBody(rva);
		}

		public override SectionData GetSectionData(int rva)
		{
			return new SectionData(Reader.GetSectionData(rva));
		}

		public override int GetContainingSectionIndex(int rva)
		{
			return Reader.PEHeaders.GetContainingSectionIndex(rva);
		}

		ImmutableArray<SectionHeader> sectionHeaders;
		public override ImmutableArray<SectionHeader> SectionHeaders {
			get {
				var value = sectionHeaders;
				if (value.IsDefault)
				{
					value = Reader.PEHeaders.SectionHeaders
						.Select(h => new SectionHeader {
							Name = h.Name,
							RawDataPtr = unchecked((uint)h.PointerToRawData),
							RawDataSize = unchecked((uint)h.SizeOfRawData),
							VirtualAddress = unchecked((uint)h.VirtualAddress),
							VirtualSize = unchecked((uint)h.VirtualSize)
						}).ToImmutableArray();
					sectionHeaders = value;
				}
				return value;
			}
		}

		public override CorHeader? CorHeader => Reader.PEHeaders.CorHeader;
	}
}
