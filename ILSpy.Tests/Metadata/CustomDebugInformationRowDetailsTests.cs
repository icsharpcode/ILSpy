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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Text;

using Avalonia.Controls;
using Avalonia.Headless.NUnit;

using AwesomeAssertions;

using ICSharpCode.Decompiler.DebugInfo;
using ICSharpCode.Decompiler.Metadata;

using ICSharpCode.ILSpy.Metadata;
using ICSharpCode.ILSpy.Metadata.DebugTables;
using ICSharpCode.ILSpy.TextView;
using ICSharpCode.ILSpy.ViewModels;

using NUnit.Framework;

using CustomDebugInformationEntry = ICSharpCode.ILSpy.Metadata.DebugTables.CustomDebugInformationTableTreeNode.CustomDebugInformationEntry;

namespace ICSharpCode.ILSpy.Tests.Metadata;

[TestFixture]
public class CustomDebugInformationRowDetailsTests
{
	const string SourceLinkJson = /*lang=json,strict*/ """{"documents":{"/src/*":"https://example.invalid/*"}}""";
	const string EmbeddedSourceText = "class Embedded { void M() { } }";
	static readonly Guid UnknownKindGuid = new Guid("1c46e7c9-0631-44eb-a78a-f8e8e1e0c0a1");
	static readonly Guid ReferenceMvid = new Guid("8e2c021c-1f4e-4a40-a3a8-9785b2a99f9c");
	static readonly byte[] EmbeddedSourceDeflateBlob = BuildEmbeddedSourceDeflateBlob(EmbeddedSourceText);

	static MetadataFile BuildPdbFixture()
	{
		// Synthesize a standalone portable PDB whose CustomDebugInformation table holds one
		// row per row-details scenario: a text kind (Source Link), an unrecognized kind
		// GUID, the four structured kinds that parse into typed rows, and embedded source in
		// both formats (raw and DEFLATE), each parented to a document whose name carries the
		// source-language extension. MetadataBuilder sorts the table by its
		// HasCustomDebugInformation coded index on serialize, interleaving method- and
		// document-parented rows, so tests locate rows by kind GUID rather than by position.
		var builder = new MetadataBuilder();

		var csharpDocument = AddDocument("/src/Example.cs", KnownGuids.CSharpLanguageGuid);
		var vbDocument = AddDocument("/src/Module1.vb", KnownGuids.VBLanguageGuid);

		AddRow(MethodRow(1), KnownGuids.SourceLink, Encoding.UTF8.GetBytes(SourceLinkJson));
		AddRow(MethodRow(2), UnknownKindGuid, new byte[] { 0x01, 0x02, 0x03 });
		AddRow(MethodRow(3), KnownGuids.StateMachineHoistedLocalScopes, HoistedScopesBlob((0, 10), (16, 32)));
		AddRow(MethodRow(4), KnownGuids.CompilationOptions, NullTerminatedStrings("language", "C#", "version", "2"));
		AddRow(MethodRow(5), KnownGuids.CompilationMetadataReferences, MetadataReferenceBlob(
			"System.Runtime.dll", "global", flags: 1, timestamp: 0x12345678, fileSize: 1024, ReferenceMvid));
		AddRow(MethodRow(6), KnownGuids.TupleElementNames, NullTerminatedStrings("Item1", "Name"));
		AddRow(csharpDocument, KnownGuids.EmbeddedSource, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x41 });
		AddRow(vbDocument, KnownGuids.EmbeddedSource, EmbeddedSourceDeflateBlob);

		var rowCounts = new int[MetadataTokens.TableCount];
		rowCounts[(int)TableIndex.MethodDef] = 6;
		var pdbBuilder = new PortablePdbBuilder(builder, rowCounts.ToImmutableArray(), entryPoint: default);
		var blob = new BlobBuilder();
		pdbBuilder.Serialize(blob);

		var provider = MetadataReaderProvider.FromPortablePdbImage(blob.ToImmutableArray());
		return new MetadataFile(MetadataFile.MetadataFileKind.ProgramDebugDatabase, "synthetic.pdb", provider);

		static EntityHandle MethodRow(int methodRow)
			=> MetadataTokens.EntityHandle(TableIndex.MethodDef, methodRow);

		void AddRow(EntityHandle parent, Guid kind, byte[] value)
		{
			builder.AddCustomDebugInformation(
				parent,
				builder.GetOrAddGuid(kind),
				builder.GetOrAddBlob(value));
		}

		DocumentHandle AddDocument(string name, Guid language)
		{
			return builder.AddDocument(
				builder.GetOrAddDocumentName(name),
				builder.GetOrAddGuid(KnownGuids.HashAlgorithmSHA256),
				builder.GetOrAddBlob(new byte[32]),
				builder.GetOrAddGuid(language));
		}
	}

	static byte[] BuildEmbeddedSourceDeflateBlob(string text)
	{
		// Embedded-source blob: an int32 format header, then the document bytes. A positive
		// header is the uncompressed size and marks the payload as DEFLATE-compressed.
		var content = Encoding.UTF8.GetBytes(text);
		using var ms = new MemoryStream();
		ms.Write(BitConverter.GetBytes(content.Length), 0, 4);
		using (var deflate = new DeflateStream(ms, CompressionMode.Compress, leaveOpen: true))
		{
			deflate.Write(content, 0, content.Length);
		}
		return ms.ToArray();
	}

	static byte[] HoistedScopesBlob(params (uint StartOffset, uint Length)[] scopes)
	{
		// State-machine hoisted local scopes: a sequence of (start offset, length) uint32 pairs.
		using var ms = new MemoryStream();
		using var writer = new BinaryWriter(ms);
		foreach (var (start, length) in scopes)
		{
			writer.Write(start);
			writer.Write(length);
		}
		writer.Flush();
		return ms.ToArray();
	}

	static byte[] NullTerminatedStrings(params string[] values)
	{
		// Compilation options and tuple element names are flat sequences of null-terminated
		// UTF-8 strings (options interpret them pairwise as name/value).
		using var ms = new MemoryStream();
		foreach (var value in values)
		{
			var bytes = Encoding.UTF8.GetBytes(value);
			ms.Write(bytes, 0, bytes.Length);
			ms.WriteByte(0);
		}
		return ms.ToArray();
	}

	static byte[] MetadataReferenceBlob(string fileName, string aliases, byte flags, uint timestamp, uint fileSize, Guid mvid)
	{
		using var ms = new MemoryStream();
		var name = Encoding.UTF8.GetBytes(fileName);
		ms.Write(name, 0, name.Length);
		ms.WriteByte(0);
		var alias = Encoding.UTF8.GetBytes(aliases);
		ms.Write(alias, 0, alias.Length);
		ms.WriteByte(0);
		using var writer = new BinaryWriter(ms);
		writer.Write(flags);
		writer.Write(timestamp);
		writer.Write(fileSize);
		writer.Write(mvid.ToByteArray());
		writer.Flush();
		return ms.ToArray();
	}

	static List<CustomDebugInformationEntry> LoadEntries(MetadataFile metadataFile)
		=> metadataFile.Metadata.CustomDebugInformation
			.Select(h => new CustomDebugInformationEntry(metadataFile, h))
			.ToList();

	static CustomDebugInformationEntry ByKind(IEnumerable<CustomDebugInformationEntry> entries, Guid kind)
		=> entries.Single(e => e.KindGUID == kind);

	/// <summary>The embedded-source rows in table order: raw (".cs" document) first, DEFLATE (".vb" document) second.</summary>
	static List<CustomDebugInformationEntry> EmbeddedSourceRows(IEnumerable<CustomDebugInformationEntry> entries)
		=> entries.Where(e => e.KindGUID == KnownGuids.EmbeddedSource).ToList();

	[Test]
	public void Offset_And_Split_Kind_Columns_Match_The_Tables_Conventions()
	{
		// The kind surfaces as three columns — the GUID heap offset (Kind), the raw GUID
		// (KindGUID), and the decoded friendly name (KindString) — so each is independently
		// sortable and filterable. Offset positions the row within the metadata stream like
		// every other table's Offset column.
		var metadataFile = BuildPdbFixture();
		var entries = LoadEntries(metadataFile);

		ByKind(entries, KnownGuids.SourceLink).Kind.Should().BePositive("the Kind column is the 1-based GUID heap offset");
		ByKind(entries, KnownGuids.SourceLink).KindString.Should().Be("Source Link (C# / VB)");
		ByKind(entries, UnknownKindGuid).KindString.Should().Be("Unknown");
		ByKind(entries, KnownGuids.StateMachineHoistedLocalScopes).KindString.Should().Be("State Machine Hoisted Local Scopes (C# / VB)");

		int rowSize = metadataFile.Metadata.GetTableRowSize(TableIndex.CustomDebugInformation);
		entries[0].Offset.Should().BePositive("the table lives at a real offset inside the PDB metadata");
		entries.Select(e => e.Offset).Should().BeInAscendingOrder()
			.And.HaveCount(8);
		(entries[1].Offset - entries[0].Offset).Should().Be(rowSize);
	}

	[Test]
	public void RowDetails_Parses_The_Structured_Kinds_Into_Typed_Rows()
	{
		var metadataFile = BuildPdbFixture();
		var entries = LoadEntries(metadataFile);

		ByKind(entries, KnownGuids.StateMachineHoistedLocalScopes).RowDetails
			.Should().BeAssignableTo<IEnumerable<HoistedLocalScopeDetail>>()
			.Which.Should().Equal(
				new HoistedLocalScopeDetail(0, 10),
				new HoistedLocalScopeDetail(16, 32));

		ByKind(entries, KnownGuids.CompilationOptions).RowDetails
			.Should().BeAssignableTo<IEnumerable<CompilationOptionDetail>>()
			.Which.Should().Equal(
				new CompilationOptionDetail("language", "C#"),
				new CompilationOptionDetail("version", "2"));

		ByKind(entries, KnownGuids.CompilationMetadataReferences).RowDetails
			.Should().BeAssignableTo<IEnumerable<MetadataReferenceDetail>>()
			.Which.Should().Equal(
				new MetadataReferenceDetail("System.Runtime.dll", "global", 1, 0x12345678, 1024, ReferenceMvid));

		ByKind(entries, KnownGuids.TupleElementNames).RowDetails
			.Should().BeAssignableTo<IEnumerable<TupleElementNameDetail>>()
			.Which.Should().Equal(
				new TupleElementNameDetail("Item1"),
				new TupleElementNameDetail("Name"));
	}

	[Test]
	public void RowDetails_Shows_Text_For_Source_Link_And_Hex_For_Opaque_Blobs()
	{
		var metadataFile = BuildPdbFixture();
		var entries = LoadEntries(metadataFile);

		ByKind(entries, KnownGuids.SourceLink).RowDetails.Should().Be(new TextBlobDetail(SourceLinkJson, ".json"),
			"source link blobs are UTF-8 JSON");
		ByKind(entries, UnknownKindGuid).RowDetails.Should().Be(new TextBlobDetail("01-02-03"),
			"unrecognized kinds degrade to a plain hex dump");
	}

	[Test]
	public void RowDetails_Decodes_Embedded_Source_To_The_Document_Text()
	{
		// The parent document's file extension travels with the decoded text so the details
		// area can highlight the source in its own language.
		var metadataFile = BuildPdbFixture();
		var entries = LoadEntries(metadataFile);

		var embedded = EmbeddedSourceRows(entries);
		embedded[0].RowDetails.Should().Be(new TextBlobDetail("A", ".cs"),
			"a zero format header means the document bytes follow uncompressed");
		embedded[1].RowDetails.Should().Be(new TextBlobDetail(EmbeddedSourceText, ".vb"),
			"a positive format header means the document is DEFLATE-compressed");
	}

	[Test]
	public void Info_Summarizes_The_Embedded_Source_Format()
	{
		var metadataFile = BuildPdbFixture();
		var entries = LoadEntries(metadataFile);

		ByKind(entries, KnownGuids.SourceLink).Info.Should().BeNull(
			"only embedded source carries a format header to summarize");
		var embedded = EmbeddedSourceRows(entries);
		embedded[0].Info.Should().Be("Raw, 5 bytes");
		embedded[1].Info.Should().Be(
			$"DEFLATE, {EmbeddedSourceDeflateBlob.Length} bytes, {Encoding.UTF8.GetByteCount(EmbeddedSourceText)} uncompressed");
	}

	[AvaloniaTest]
	public void Tab_Configures_Selection_Driven_Row_Details_That_Route_By_Blob_Shape()
	{
		// The details area swaps presentation per row: text blobs land in a read-only
		// syntax-highlighting editor, structured kinds in a sub-grid. Selection drives
		// visibility, so scanning the table with the arrow keys previews each blob.
		var metadataFile = BuildPdbFixture();
		var node = new CustomDebugInformationTableTreeNode(metadataFile);
		var tab = (MetadataTablePageModel)node.CreateTab();

		tab.RowDetailsVisibilityMode.Should().Be(DataGridRowDetailsVisibilityMode.VisibleWhenSelected);
		tab.RowDetailsTemplate.Should().NotBeNull();

		var entries = tab.Items.Cast<CustomDebugInformationEntry>().ToList();
		var sourceLink = ByKind(entries, KnownGuids.SourceLink);
		var shell = tab.RowDetailsTemplate!.Build(sourceLink)
			.Should().BeOfType<MetadataRowDetailsControl>().Subject;

		shell.DataContext = sourceLink;
		var editor = shell.Content.Should().BeOfType<DecompilerTextEditor>().Subject;
		editor.Text.Should().Be(SourceLinkJson);
		editor.SyntaxHighlighting.Should().NotBeNull("source-link JSON gets JSON highlighting");

		shell.DataContext = ByKind(entries, KnownGuids.CompilationOptions);
		var optionsGrid = shell.Content.Should().BeOfType<DataGrid>().Subject;
		((IEnumerable)optionsGrid.ItemsSource!).Cast<CompilationOptionDetail>().Should().HaveCount(2);
	}
}
