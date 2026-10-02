// Copyright (c) 2026 Piero Viano
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
using System.Collections.Immutable;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Symbols;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Symbols;

[TestFixture]
public class SourceLinkTests
{
	static readonly Guid Sha256 = new("8829d00f-11b8-4213-878b-770e8597ac16");
	static readonly Guid CSharp = new("3f5162f8-07c6-11d3-9053-00c04fa302a1");

	[Test]
	public void Map_prefers_exact_then_longest_prefix_and_rewrites_separators()
	{
		var map = SourceLinkMap.Parse("""
			{"documents":{
				"C:\\src\\*":"https://a/*",
				"C:\\src\\lib\\*":"https://b/*",
				"C:\\src\\lib\\x.cs":"https://exact/x.cs"
			}}
			""")!;

		map.GetUri(@"C:\src\app\Program.cs").Should().Be("https://a/app/Program.cs");
		map.GetUri(@"C:\src\lib\sub\A B.cs").Should().Be("https://b/sub/A%20B.cs");
		map.GetUri(@"C:\src\lib\x.cs").Should().Be("https://exact/x.cs");
		map.GetUri(@"D:\elsewhere\y.cs").Should().BeNull();
	}

	[TestCase("not json")]
	[TestCase("{}")]
	[TestCase("{\"documents\":{\"C:\\\\*\":\"https://no-star\"}}")]
	[TestCase("{\"documents\":{\"C:\\\\a.cs\":\"https://a/*\"}}")]
	public void Invalid_maps_are_rejected(string json)
	{
		SourceLinkMap.Parse(json).Should().BeNull();
	}

	[Test]
	public async Task Downloads_documents_of_a_method_through_the_pdbs_source_link_map()
	{
		var fixture = SymbolFixture.Create();
		var handler = new FakeHttpHandler();
		handler.Add(SymbolFixture.SourceLinkUri, Encoding.UTF8.GetBytes("class Greeter {}"));
		var list = new AssemblyList { UseDebugSymbols = true };
		var assembly = list.OpenAssembly(fixture.AssemblyPath);
		var module = await assembly.GetMetadataFileAsync();
		var sources = OriginalSourceProvider.TryCreate(assembly.GetDebugInfoOrNull(), new HttpClient(handler))!;

		var documents = sources.GetDocuments(module.Metadata.MethodDefinitions);
		documents.Should().ContainSingle();
		sources.GetDocumentPath(documents[0]).Should().Be(SymbolFixture.DocumentPath);

		var source = await sources.GetSourceAsync(documents[0]);
		source.Should().NotBeNull();
		source!.Origin.Should().Be(OriginalSourceOrigin.SourceLink);
		source.Uri.Should().Be(SymbolFixture.SourceLinkUri);
		source.Text.Should().Be("class Greeter {}");
		source.ChecksumMatches.Should().BeNull("the fixture's document has no checksum");
	}

	[Test]
	public async Task Embedded_source_wins_over_source_link_and_is_decompressed()
	{
		const string text = "// embedded\nclass C {}\n";
		var pdb = BuildPdb(text, embed: true);
		var handler = new FakeHttpHandler();

		var sources = new OriginalSourceProvider(pdb, new HttpClient(handler));
		var source = await sources.GetSourceAsync(sources.AllDocuments.Single());

		source!.Origin.Should().Be(OriginalSourceOrigin.Embedded);
		source.Text.Should().Be(text);
		handler.Requests.Should().BeEmpty();
	}

	[Test]
	public async Task Downloaded_source_is_checked_against_the_document_hash_and_cached_when_it_matches()
	{
		const string text = "class C {}\n";
		var pdb = BuildPdb(text, embed: false);
		var handler = new FakeHttpHandler();
		handler.Add("https://src.example/C.cs", Encoding.UTF8.GetBytes(text));
		var cache = SymbolFixture.NewTempDirectory();

		var source = await new OriginalSourceProvider(pdb, new HttpClient(handler), cache).GetSourceAsync(MetadataTokens.DocumentHandle(1));
		source!.ChecksumMatches.Should().BeTrue();

		var again = await new OriginalSourceProvider(pdb, new HttpClient(handler), cache).GetSourceAsync(MetadataTokens.DocumentHandle(1));
		again!.Text.Should().Be(text);
		handler.Requests.Should().ContainSingle("a verified download is served from the cache");
	}

	[Test]
	public async Task Mismatching_download_is_returned_flagged_and_not_cached()
	{
		var pdb = BuildPdb("class C {}\n", embed: false);
		var handler = new FakeHttpHandler();
		handler.Add("https://src.example/C.cs", Encoding.UTF8.GetBytes("class C {}\r\n"));
		var cache = SymbolFixture.NewTempDirectory();

		var source = await new OriginalSourceProvider(pdb, new HttpClient(handler), cache).GetSourceAsync(MetadataTokens.DocumentHandle(1));

		source!.ChecksumMatches.Should().BeFalse();
		Directory.EnumerateFiles(cache, "*", SearchOption.AllDirectories).Should().BeEmpty();
	}

	static MetadataReader BuildPdb(string text, bool embed)
	{
		var bytes = Encoding.UTF8.GetBytes(text);
		var metadata = new MetadataBuilder();
		var document = metadata.AddDocument(
			metadata.GetOrAddDocumentName(@"C:\src\C.cs"),
			metadata.GetOrAddGuid(Sha256),
			metadata.GetOrAddBlob(SHA256.HashData(bytes)),
			metadata.GetOrAddGuid(CSharp));
		if (embed)
		{
			var compressed = new MemoryStream();
			using (var deflate = new DeflateStream(compressed, CompressionLevel.Optimal, leaveOpen: true))
				deflate.Write(bytes);
			var blob = new BlobBuilder();
			blob.WriteInt32(bytes.Length);
			blob.WriteBytes(compressed.ToArray());
			metadata.AddCustomDebugInformation(document,
				metadata.GetOrAddGuid(new Guid("0E8A571B-6926-466E-B4AD-8AB04611F5FE")), metadata.GetOrAddBlob(blob));
		}
		metadata.AddCustomDebugInformation(EntityHandle.ModuleDefinition,
			metadata.GetOrAddGuid(new Guid("CC110556-A091-4D38-9FEC-25AB9A351A6A")),
			metadata.GetOrAddBlob(Encoding.UTF8.GetBytes("{\"documents\":{\"C:\\\\src\\\\*\":\"https://src.example/*\"}}")));
		var builder = new PortablePdbBuilder(metadata, ImmutableArray.CreateRange(new int[MetadataTokens.TableCount]), default);
		var output = new BlobBuilder();
		builder.Serialize(output);
		return MetadataReaderProvider.FromPortablePdbImage(output.ToImmutableArray()).GetMetadataReader();
	}
}
