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
		// GUID, embedded source in both formats (raw and DEFLATE), and the four structured
		// kinds that parse into typed rows. Parents use ascending MethodDef tokens so the
		// (parent-sorted) table preserves this row order.
		var builder = new MetadataBuilder();

		AddRow(1, KnownGuids.SourceLink, Encoding.UTF8.GetBytes(SourceLinkJson));
		AddRow(2, UnknownKindGuid, new byte[] { 0x01, 0x02, 0x03 });
		AddRow(3, KnownGuids.EmbeddedSource, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x41 });
		AddRow(4, KnownGuids.StateMachineHoistedLocalScopes, HoistedScopesBlob((0, 10), (16, 32)));
		AddRow(5, KnownGuids.CompilationOptions, NullTerminatedStrings("language", "C#", "version", "2"));
		AddRow(6, KnownGuids.CompilationMetadataReferences, MetadataReferenceBlob(
			"System.Runtime.dll", "global", flags: 1, timestamp: 0x12345678, fileSize: 1024, ReferenceMvid));
		AddRow(7, KnownGuids.TupleElementNames, NullTerminatedStrings("Item1", "Name"));
		AddRow(8, KnownGuids.EmbeddedSource, EmbeddedSourceDeflateBlob);

		var rowCounts = new int[MetadataTokens.TableCount];
		rowCounts[(int)TableIndex.MethodDef] = 8;
		var pdbBuilder = new PortablePdbBuilder(builder, rowCounts.ToImmutableArray(), entryPoint: default);
		var blob = new BlobBuilder();
		pdbBuilder.Serialize(blob);

		var provider = MetadataReaderProvider.FromPortablePdbImage(blob.ToImmutableArray());
		return new MetadataFile(MetadataFile.MetadataFileKind.ProgramDebugDatabase, "synthetic.pdb", provider);

		void AddRow(int methodRow, Guid kind, byte[] value)
		{
			builder.AddCustomDebugInformation(
				MetadataTokens.EntityHandle(TableIndex.MethodDef, methodRow),
				builder.GetOrAddGuid(kind),
				builder.GetOrAddBlob(value));
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

	[Test]
	public void Offset_And_Split_Kind_Columns_Match_The_Tables_Conventions()
	{
		// The kind surfaces as three columns — the GUID heap offset (Kind), the raw GUID
		// (KindGUID), and the decoded friendly name (KindString) — so each is independently
		// sortable and filterable. Offset positions the row within the metadata stream like
		// every other table's Offset column.
		var metadataFile = BuildPdbFixture();
		var entries = LoadEntries(metadataFile);

		entries[0].Kind.Should().BePositive("the Kind column is the 1-based GUID heap offset");
		entries[0].KindGUID.Should().Be(KnownGuids.SourceLink);
		entries[0].KindString.Should().Be("Source Link (C# / VB)");
		entries[1].KindGUID.Should().Be(UnknownKindGuid);
		entries[1].KindString.Should().Be("Unknown");
		entries[3].KindString.Should().Be("State Machine Hoisted Local Scopes (C# / VB)");

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

		entries[3].RowDetails.Should().BeAssignableTo<IEnumerable<HoistedLocalScopeDetail>>()
			.Which.Should().Equal(
				new HoistedLocalScopeDetail(0, 10),
				new HoistedLocalScopeDetail(16, 32));

		entries[4].RowDetails.Should().BeAssignableTo<IEnumerable<CompilationOptionDetail>>()
			.Which.Should().Equal(
				new CompilationOptionDetail("language", "C#"),
				new CompilationOptionDetail("version", "2"));

		entries[5].RowDetails.Should().BeAssignableTo<IEnumerable<MetadataReferenceDetail>>()
			.Which.Should().Equal(
				new MetadataReferenceDetail("System.Runtime.dll", "global", 1, 0x12345678, 1024, ReferenceMvid));

		entries[6].RowDetails.Should().BeAssignableTo<IEnumerable<TupleElementNameDetail>>()
			.Which.Should().Equal(
				new TupleElementNameDetail("Item1"),
				new TupleElementNameDetail("Name"));
	}

	[Test]
	public void RowDetails_Shows_Text_For_Source_Link_And_Hex_For_Opaque_Blobs()
	{
		var metadataFile = BuildPdbFixture();
		var entries = LoadEntries(metadataFile);

		entries[0].RowDetails.Should().Be(SourceLinkJson, "source link blobs are UTF-8 JSON");
		entries[1].RowDetails.Should().Be("01-02-03", "unrecognized kinds degrade to a hex dump");
	}

	[Test]
	public void RowDetails_Decodes_Embedded_Source_To_The_Document_Text()
	{
		var metadataFile = BuildPdbFixture();
		var entries = LoadEntries(metadataFile);

		entries[2].RowDetails.Should().Be("A", "a zero format header means the document bytes follow uncompressed");
		entries[7].RowDetails.Should().Be(EmbeddedSourceText, "a positive format header means the document is DEFLATE-compressed");
	}

	[Test]
	public void Info_Summarizes_The_Embedded_Source_Format()
	{
		var metadataFile = BuildPdbFixture();
		var entries = LoadEntries(metadataFile);

		entries[0].Info.Should().BeNull("only embedded source carries a format header to summarize");
		entries[2].Info.Should().Be("Raw, 5 bytes");
		entries[7].Info.Should().Be(
			$"DEFLATE, {EmbeddedSourceDeflateBlob.Length} bytes, {Encoding.UTF8.GetByteCount(EmbeddedSourceText)} uncompressed");
	}

	[AvaloniaTest]
	public void Tab_Configures_Selection_Driven_Row_Details_That_Route_By_Blob_Shape()
	{
		// The details area swaps presentation per row: text blobs land in a read-only
		// TextBox, structured kinds in a sub-grid. Selection drives visibility, so scanning
		// the table with the arrow keys previews each blob.
		var metadataFile = BuildPdbFixture();
		var node = new CustomDebugInformationTableTreeNode(metadataFile);
		var tab = (MetadataTablePageModel)node.CreateTab();

		tab.RowDetailsVisibilityMode.Should().Be(DataGridRowDetailsVisibilityMode.VisibleWhenSelected);
		tab.RowDetailsTemplate.Should().NotBeNull();

		var entries = tab.Items.Cast<CustomDebugInformationEntry>().ToList();
		var shell = tab.RowDetailsTemplate!.Build(entries[0])
			.Should().BeOfType<MetadataRowDetailsControl>().Subject;

		shell.DataContext = entries[0];
		shell.Content.Should().BeOfType<TextBox>().Which.Text.Should().Be(SourceLinkJson);

		shell.DataContext = entries[4];
		var optionsGrid = shell.Content.Should().BeOfType<DataGrid>().Subject;
		((IEnumerable)optionsGrid.ItemsSource!).Cast<CompilationOptionDetail>().Should().HaveCount(2);
	}
}
