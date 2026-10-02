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

using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.Decompiler;
using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Symbols;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Symbols;

[TestFixture]
public class SymbolServerHostTests
{
	sealed record Served(SymbolServerHost Host, HttpClient Client, SymbolFixture Fixture, SymbolKey PdbKey, SymbolKey PEKey);

	static async Task<Served> StartAsync(bool keepPdb)
	{
		var fixture = SymbolFixture.Create();
		if (!keepPdb)
			fixture.DetachPdb();
		var list = new AssemblyList { UseDebugSymbols = true };
		var assembly = list.OpenAssembly(fixture.AssemblyPath);
		await assembly.GetMetadataFileAsync();
		var store = new DecompiledSymbolStore(() => list.GetAssemblies(),
			_ => new DecompilerSettings(), SymbolFixture.NewTempDirectory());
		var host = new SymbolServerHost(store);
		host.Start();
		using var reader = new PEReader(File.OpenRead(fixture.AssemblyPath));
		return new Served(host, new HttpClient { BaseAddress = host.BaseAddress }, fixture,
			SymbolKey.GetPdbKeys(reader).Single(), SymbolKey.GetPEKey(reader, fixture.AssemblyPath)!);
	}

	[Test]
	public async Task Serves_a_generated_pdb_that_matches_the_assembly_and_embeds_the_decompiled_source()
	{
		var served = await StartAsync(keepPdb: false);
		using var host = served.Host;

		var response = await served.Client.GetAsync(served.PdbKey.Key);

		response.StatusCode.Should().Be(HttpStatusCode.OK);
		var pdbPath = Path.Combine(SymbolFixture.NewTempDirectory(), "served.pdb");
		await File.WriteAllBytesAsync(pdbPath, await response.Content.ReadAsByteArrayAsync());
		SymbolLocator.Matches(pdbPath, served.PdbKey).Should().BeTrue("a debugger only accepts a PDB whose id matches the CodeView entry");

		using var provider = MetadataReaderProvider.FromPortablePdbStream(File.OpenRead(pdbPath));
		var sources = new OriginalSourceProvider(provider.GetMetadataReader());
		var texts = sources.AllDocuments.Select(sources.GetEmbeddedSource).ToList();
		texts.Should().ContainSingle(t => t != null && t.Contains("class " + SymbolFixture.TypeName));
	}

	[Test]
	public async Task Serves_the_assemblys_own_pdb_when_it_has_one()
	{
		var served = await StartAsync(keepPdb: true);
		using var host = served.Host;

		var bytes = await served.Client.GetByteArrayAsync(served.PdbKey.Key.ToUpperInvariant());

		bytes.Should().Equal(File.ReadAllBytes(served.Fixture.PdbPath), "keys are case-insensitive and a real PDB beats a generated one");
	}

	[Test]
	public async Task Serves_the_PE_file_under_its_key()
	{
		var served = await StartAsync(keepPdb: false);
		using var host = served.Host;

		var bytes = await served.Client.GetByteArrayAsync(served.PEKey.Key);

		bytes.Should().Equal(File.ReadAllBytes(served.Fixture.AssemblyPath));
	}

	[Test]
	public async Task Unknown_keys_and_other_methods_are_rejected()
	{
		var served = await StartAsync(keepPdb: false);
		using var host = served.Host;

		(await served.Client.GetAsync("foo.pdb/0000ffffffff/foo.pdb")).StatusCode.Should().Be(HttpStatusCode.NotFound);
		(await served.Client.GetAsync("not-a-key")).StatusCode.Should().Be(HttpStatusCode.NotFound);
		(await served.Client.PostAsync(served.PdbKey.Key, new StringContent(""))).StatusCode.Should().Be(HttpStatusCode.MethodNotAllowed);
	}

	[Test]
	public async Task Stop_releases_the_port()
	{
		var served = await StartAsync(keepPdb: false);
		var port = served.Host.BaseAddress!.Port;

		await served.Host.StopAsync();

		served.Host.IsRunning.Should().BeFalse();
		served.Host.BaseAddress.Should().BeNull();
		using var again = new SymbolServerHost(new DecompiledSymbolStore(() => [], _ => new DecompilerSettings(), SymbolFixture.NewTempDirectory()), port);
		again.Start();
		again.BaseAddress!.Port.Should().Be(port);
	}

	[TestCase("/foo.pdb/abc/foo.pdb", "foo.pdb/abc/foo.pdb")]
	[TestCase("/Foo.PDB/ABC/Foo.PDB?x=1", "foo.pdb/abc/foo.pdb")]
	[TestCase("/foo.pdb/../foo.pdb", null)]
	[TestCase("/..%2F..%2Fsecret/abc/foo.pdb", null)]
	[TestCase("/a/b", null)]
	[TestCase("/c:/abc/foo.pdb", null)]
	public void Request_targets_are_normalized_to_keys(string target, string? expected)
	{
		SymbolServerHost.NormalizeKey(target).Should().Be(expected);
	}
}
