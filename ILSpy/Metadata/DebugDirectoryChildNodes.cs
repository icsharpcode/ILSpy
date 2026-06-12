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
using System.Reflection.PortableExecutable;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;

using ICSharpCode.ILSpy.Languages;
using ICSharpCode.ILSpy.TreeNodes;

namespace ICSharpCode.ILSpy.Metadata
{
	/// <summary>
	/// Per-entry tree node under <see cref="DebugDirectoryTreeNode"/>. Specialised typed
	/// subclasses (<see cref="CodeViewTreeNode"/>, <see cref="PdbChecksumTreeNode"/>)
	/// surface their fields as a tab page; this fallback shows the raw entry bytes for
	/// types we don't decode further (Reproducible, EmbeddedPortablePdb, vendor entries).
	/// </summary>
	public class DebugDirectoryEntryTreeNode : ILSpyTreeNode
	{
		readonly PEFile module;
		readonly DebugDirectoryEntry entry;

		public DebugDirectoryEntryTreeNode(PEFile module, DebugDirectoryEntry entry)
		{
			this.module = module ?? throw new ArgumentNullException(nameof(module));
			this.entry = entry;
		}

		public override object Text => entry.Type.ToString();
		public override object Icon => Images.MetadataTable;
		public override string ToString() => entry.Type.ToString();

		protected DebugDirectoryEntry Entry => entry;
		protected PEFile Module => module;

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, entry.Type.ToString());
			if (entry.DataSize > 0)
			{
				language.WriteCommentLine(output, $"Raw Data ({entry.DataSize}):");
				int dataOffset = module.Reader.IsLoadedImage
					? entry.DataRelativeVirtualAddress
					: entry.DataPointer;
				var data = module.Reader.GetEntireImage().GetContent(dataOffset, entry.DataSize);
				language.WriteCommentLine(output, data.ToHexString(data.Length));
			}
			else
			{
				language.WriteCommentLine(output, "(no data)");
			}
		}
	}

	/// <summary>
	/// CodeView debug-directory entry — names the associated PDB. Surfaces the GUID,
	/// age, and original symbol-file path.
	/// </summary>
	public sealed class CodeViewTreeNode : DebugDirectoryEntryTreeNode
	{
		readonly CodeViewDebugDirectoryData data;

		public CodeViewTreeNode(PEFile module, DebugDirectoryEntry entry, CodeViewDebugDirectoryData data)
			: base(module, entry)
		{
			this.data = data;
		}

		public override object Text => nameof(DebugDirectoryEntryType.CodeView);

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, Text.ToString()!);
			language.WriteCommentLine(output, $"GUID: {data.Guid}");
			language.WriteCommentLine(output, $"Age: {data.Age}");
			language.WriteCommentLine(output, $"Path: {data.Path}");
		}
	}

	/// <summary>
	/// PdbChecksum debug-directory entry — a cryptographic hash of the symbol file's
	/// contents, used to validate that a PDB matches its PE/COFF parent. Multiple entries
	/// can coexist when the build emits both public and private PDBs.
	/// </summary>
	public sealed class PdbChecksumTreeNode : DebugDirectoryEntryTreeNode
	{
		readonly PdbChecksumDebugDirectoryData data;

		public PdbChecksumTreeNode(PEFile module, DebugDirectoryEntry entry, PdbChecksumDebugDirectoryData data)
			: base(module, entry)
		{
			this.data = data;
		}

		public override object Text => nameof(DebugDirectoryEntryType.PdbChecksum);

		public string AlgorithmName => data.AlgorithmName;
		public string Checksum => data.Checksum.ToHexString(data.Checksum.Length);

		public override void Decompile(Language language, ITextOutput output, DecompilationOptions options)
		{
			language.WriteCommentLine(output, Text.ToString()!);
			language.WriteCommentLine(output, $"AlgorithmName: {AlgorithmName}");
			language.WriteCommentLine(output, $"Checksum: {Checksum}");
		}
	}
}
