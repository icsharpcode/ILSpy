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
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

using Avalonia.Controls;

using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;

using ICSharpCode.ILSpy.ViewModels;

namespace ICSharpCode.ILSpy.Metadata.DebugTables
{
	/// <summary>One hoisted-local scope (IL offset range) of a state-machine method.</summary>
	public sealed record HoistedLocalScopeDetail(uint StartOffset, uint Length);

	/// <summary>One name/value pair from a compilation-options blob.</summary>
	public sealed record CompilationOptionDetail(string Name, string Value);

	/// <summary>One reference from a compilation-metadata-references blob.</summary>
	public sealed record MetadataReferenceDetail(string FileName, string Aliases, byte Flags, uint Timestamp, uint FileSize, Guid Mvid);

	/// <summary>One element name from a tuple-element-names blob.</summary>
	public sealed record TupleElementNameDetail(string ElementName);

	/// <summary>
	/// View of the CustomDebugInformation table — extensible per-entity payloads used for
	/// async/iterator state-machine info, embedded source, source-link JSON, and similar
	/// debug-time data. Each row carries a Parent token, a Kind GUID (surfaced as heap
	/// offset, raw GUID, and decoded friendly name in the Kind / KindGUID / KindString
	/// columns), and an opaque Value blob whose parsed contents show as the row's details.
	/// </summary>
	public sealed class CustomDebugInformationTableTreeNode : MetadataTableTreeNode<CustomDebugInformationTableTreeNode.CustomDebugInformationEntry>
	{
		public CustomDebugInformationTableTreeNode(MetadataFile metadataFile)
			: base(TableIndex.CustomDebugInformation, metadataFile)
		{
		}

		protected override IReadOnlyList<CustomDebugInformationEntry> LoadTable()
		{
			var list = new List<CustomDebugInformationEntry>();
			foreach (var row in metadataFile.Metadata.CustomDebugInformation)
				list.Add(new CustomDebugInformationEntry(metadataFile, row));
			return list;
		}

		protected override void ConfigurePage(MetadataTablePageModel page)
		{
			// Selecting a row previews its Value blob beneath it: structured kinds as a typed
			// sub-grid, source-link JSON decoded, everything else as a hex dump.
			page.RowDetailsVisibilityMode = DataGridRowDetailsVisibilityMode.VisibleWhenSelected;
			page.RowDetailsTemplate = MetadataRowDetails.CreateTemplate(BuildRowDetailsContent);
		}

		static Control? BuildRowDetailsContent(object? item)
		{
			return (item as CustomDebugInformationEntry)?.RowDetails switch {
				string text => MetadataRowDetails.BuildTextBlob(text),
				IReadOnlyList<HoistedLocalScopeDetail> rows => MetadataRowDetails.BuildDetailsGrid(rows,
					("Start Offset", nameof(HoistedLocalScopeDetail.StartOffset)),
					("Length", nameof(HoistedLocalScopeDetail.Length))),
				IReadOnlyList<CompilationOptionDetail> rows => MetadataRowDetails.BuildDetailsGrid(rows,
					("Name", nameof(CompilationOptionDetail.Name)),
					("Value", nameof(CompilationOptionDetail.Value))),
				IReadOnlyList<MetadataReferenceDetail> rows => MetadataRowDetails.BuildDetailsGrid(rows,
					("File Name", nameof(MetadataReferenceDetail.FileName)),
					("Aliases", nameof(MetadataReferenceDetail.Aliases)),
					("Flags", nameof(MetadataReferenceDetail.Flags)),
					("Timestamp", nameof(MetadataReferenceDetail.Timestamp)),
					("File Size", nameof(MetadataReferenceDetail.FileSize)),
					("MVID", nameof(MetadataReferenceDetail.Mvid))),
				IReadOnlyList<TupleElementNameDetail> rows => MetadataRowDetails.BuildDetailsGrid(rows,
					("Element Name", nameof(TupleElementNameDetail.ElementName))),
				_ => null,
			};
		}

		public sealed class CustomDebugInformationEntry
		{
			readonly MetadataFile metadataFile;
			readonly CustomDebugInformationHandle handle;
			readonly CustomDebugInformation debugInfo;

			public int RID => MetadataTokens.GetRowNumber(handle);

			[ColumnInfo("X8")]
			public int Token => MetadataTokens.GetToken(handle);

			[ColumnInfo("X8")]
			public int Offset => GetRowOffset(metadataFile, TableIndex.CustomDebugInformation, RID);

			[ColumnInfo("X8", Kind = ColumnKind.Token)]
			public int Parent => MetadataTokens.GetToken(debugInfo.Parent);

			string? parentTooltip;
			public string? ParentTooltip => GenerateTooltip(ref parentTooltip, metadataFile, debugInfo.Parent);

			static readonly (Guid Guid, string Name)[] knownKindNames = {
				(KnownGuids.StateMachineHoistedLocalScopes, "State Machine Hoisted Local Scopes (C# / VB)"),
				(KnownGuids.DynamicLocalVariables, "Dynamic Local Variables (C#)"),
				(KnownGuids.DefaultNamespaces, "Default Namespaces (VB)"),
				(KnownGuids.EditAndContinueLocalSlotMap, "Edit And Continue Local Slot Map (C# / VB)"),
				(KnownGuids.EditAndContinueLambdaAndClosureMap, "Edit And Continue Lambda And Closure Map (C# / VB)"),
				(KnownGuids.EncStateMachineStateMap, "Edit And Continue State Machine State Map (C# / VB)"),
				(KnownGuids.EmbeddedSource, "Embedded Source (C# / VB)"),
				(KnownGuids.SourceLink, "Source Link (C# / VB)"),
				(KnownGuids.MethodSteppingInformation, "Method Stepping Information (C# / VB)"),
				(KnownGuids.CompilationOptions, "Compilation Options (C# / VB)"),
				(KnownGuids.CompilationMetadataReferences, "Compilation Metadata References (C# / VB)"),
				(KnownGuids.TupleElementNames, "Tuple Element Names (C#)"),
				(KnownGuids.TypeDefinitionDocuments, "Type Definition Documents (C# / VB)"),
			};

			[ColumnInfo("X2", Kind = ColumnKind.HeapOffset)]
			public int Kind => MetadataTokens.GetHeapOffset(debugInfo.Kind);

			[ColumnInfo("D")]
			public Guid KindGUID => metadataFile.Metadata.GetGuid(debugInfo.Kind);

			string? kindString;

			public string KindString {
				get {
					if (kindString != null)
						return kindString;
					if (debugInfo.Kind.IsNil)
						return kindString = "";
					var guid = metadataFile.Metadata.GetGuid(debugInfo.Kind);
					foreach (var (knownGuid, knownName) in knownKindNames)
					{
						if (guid == knownGuid)
							return kindString = knownName;
					}
					return kindString = "Unknown";
				}
			}

			public string? Info => !debugInfo.Kind.IsNil && KindGUID == KnownGuids.EmbeddedSource
				? GetEmbeddedSourceFormat()
				: null;

			string GetEmbeddedSourceFormat()
			{
				if (debugInfo.Value.IsNil)
					return "{nil blob}";

				var reader = metadataFile.Metadata.GetBlobReader(debugInfo.Value);

				if (reader.RemainingBytes < 4)
					return "{blob too short}";

				var format = reader.ReadInt32();

				return format switch {
					< 0 => $"Unknown format '{format}', {reader.Length} bytes",
					0 => $"Raw, {reader.Length} bytes",
					> 0 => $"DEFLATE, {reader.Length} bytes, {format} uncompressed",
				};
			}

			[ColumnInfo("X8", Kind = ColumnKind.HeapOffset)]
			public int Value => MetadataTokens.GetHeapOffset(debugInfo.Value);

			[ColumnInfo("X8")]
			public int ValueLength => metadataFile.Metadata.GetBlobReader(debugInfo.Value).Length;

			public string ValueTooltip {
				get {
					if (debugInfo.Value.IsNil)
						return "<nil>";
					return metadataFile.Metadata.GetBlobReader(debugInfo.Value).ToHexString();
				}
			}

			object? rowDetails;

			/// <summary>
			/// Parsed view of the Value blob for the row-details area. Structured kinds become
			/// typed row lists, source-link blobs the decoded JSON text, embedded source the
			/// (decompressed) document text, everything else (including malformed blobs) a hex
			/// dump. Cached — the details area re-requests it on every selection change.
			/// </summary>
			public object? RowDetails {
				get {
					if (rowDetails != null)
						return rowDetails;
					if (debugInfo.Value.IsNil || debugInfo.Kind.IsNil)
						return null;

					var reader = metadataFile.Metadata.GetBlobReader(debugInfo.Value);
					try
					{
						return rowDetails = ParseRowDetails(ref reader);
					}
					catch (Exception ex) when (ex is BadImageFormatException or InvalidDataException)
					{
						return rowDetails = metadataFile.Metadata.GetBlobReader(debugInfo.Value).ToHexString();
					}
				}
			}

			object ParseRowDetails(ref BlobReader reader)
			{
				var kind = metadataFile.Metadata.GetGuid(debugInfo.Kind);
				if (kind == KnownGuids.StateMachineHoistedLocalScopes)
				{
					var list = new List<HoistedLocalScopeDetail>();
					while (reader.RemainingBytes > 0)
						list.Add(new HoistedLocalScopeDetail(reader.ReadUInt32(), reader.ReadUInt32()));
					return list;
				}
				if (kind == KnownGuids.SourceLink)
					return reader.ReadUTF8(reader.RemainingBytes);
				if (kind == KnownGuids.EmbeddedSource)
				{
					var embeddedSourceFormat = reader.ReadInt32();

					if (embeddedSourceFormat < 0) // unknown format, show raw data as hex
						return reader.ToHexString();

					var embeddedSourceBytes = reader.ReadBytes(reader.RemainingBytes);
					Stream embeddedSourceByteStream = new MemoryStream(embeddedSourceBytes);

					if (embeddedSourceFormat > 0) // positive length means the data is compressed using DEFLATE
						embeddedSourceByteStream = new System.IO.Compression.DeflateStream(embeddedSourceByteStream, System.IO.Compression.CompressionMode.Decompress);

					var textReader = new StreamReader(embeddedSourceByteStream, detectEncodingFromByteOrderMarks: true);
					return textReader.ReadToEnd();
				}
				if (kind == KnownGuids.CompilationOptions)
				{
					var list = new List<CompilationOptionDetail>();
					while (reader.RemainingBytes > 0)
					{
						string name = reader.ReadUTF8StringNullTerminated();
						string value = reader.ReadUTF8StringNullTerminated();
						list.Add(new CompilationOptionDetail(name, value));
					}
					return list;
				}
				if (kind == KnownGuids.CompilationMetadataReferences)
				{
					var list = new List<MetadataReferenceDetail>();
					while (reader.RemainingBytes > 0)
					{
						string fileName = reader.ReadUTF8StringNullTerminated();
						string aliases = reader.ReadUTF8StringNullTerminated();
						byte flags = reader.ReadByte();
						uint timestamp = reader.ReadUInt32();
						uint fileSize = reader.ReadUInt32();
						Guid mvid = reader.ReadGuid();
						list.Add(new MetadataReferenceDetail(fileName, aliases, flags, timestamp, fileSize, mvid));
					}
					return list;
				}
				if (kind == KnownGuids.TupleElementNames)
				{
					var list = new List<TupleElementNameDetail>();
					while (reader.RemainingBytes > 0)
						list.Add(new TupleElementNameDetail(reader.ReadUTF8StringNullTerminated()));
					return list;
				}
				return reader.ToHexString();
			}

			public CustomDebugInformationEntry(MetadataFile metadataFile, CustomDebugInformationHandle handle)
			{
				this.metadataFile = metadataFile;
				this.handle = handle;
				debugInfo = metadataFile.Metadata.GetCustomDebugInformation(handle);
			}
		}
	}
}
