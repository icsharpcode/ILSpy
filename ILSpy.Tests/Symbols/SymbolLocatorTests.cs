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
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

using AwesomeAssertions;

using ICSharpCode.ILSpyX;
using ICSharpCode.ILSpyX.Symbols;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Symbols;

[TestFixture]
public class SymbolLocatorTests
{
	static SymbolKey PdbKey(SymbolFixture fixture)
	{
		using var reader = new PEReader(File.OpenRead(fixture.AssemblyPath));
		return SymbolKey.GetPdbKeys(reader).Single();
	}

	static Guid PdbGuid(string pdbPath)
	{
		using var provider = MetadataReaderProvider.FromPortablePdbStream(File.OpenRead(pdbPath));
		return new Guid(provider.GetMetadataReader().DebugMetadataHeader!.Id.AsSpan(0, 16));
	}

	[Test]
	public void Portable_pdb_key_follows_the_SSQP_convention()
	{
		var fixture = SymbolFixture.Create();
		var key = PdbKey(fixture);

		key.Kind.Should().Be(SymbolFileKind.PortablePdb);
		key.FileName.Should().Be("SymbolsFixture.pdb", "the CodeView path's directory is the build machine's and is dropped");
		key.Guid.Should().Be(PdbGuid(fixture.PdbPath));
		key.Key.Should().Be($"symbolsfixture.pdb/{key.Guid:N}ffffffff/symbolsfixture.pdb");
		key.ChecksumHeader.Should().StartWith("SHA256:", "symbols.nuget.org requires the PdbChecksum as a SymbolChecksum header");
	}

	[Test]
	public void Windows_pdb_and_PE_keys_follow_the_SymSrv_convention()
	{
		var guid = new Guid("497b72f6-390a-44fc-878e-5a2d63b6cc4b");
		SymbolKey.ForWindowsPdb(@"D:\b\Foo.PDB", guid, 0x1a).Should().Be("foo.pdb/497b72f6390a44fc878e5a2d63b6cc4b1a/foo.pdb");
		SymbolKey.ForPE("Foo.DLL", 0x5f3e2a1b, 0x4000).Should().Be("foo.dll/5f3e2a1b4000/foo.dll");
	}

	[Test]
	public void Symbol_path_parses_servers_caches_and_directories()
	{
		var path = SymbolPath.Parse(@"C:\flat;srv*https://a.example/s;srv*D:\cache*https://b.example/s;cache*E:\c;srv*https://c.example;symsrv*symsrv.dll*F:\f*https://d.example", @"X:\default");

		path.Elements.Should().Equal(
			new SymbolPathElement(@"C:\flat", false, null),
			new SymbolPathElement("https://a.example/s", true, @"X:\default"),
			new SymbolPathElement("https://b.example/s", true, @"D:\cache"),
			new SymbolPathElement("https://c.example", true, @"E:\c"),
			new SymbolPathElement("https://d.example", true, @"F:\f"));
	}

	[Test]
	public void Empty_or_blank_symbol_path_has_no_elements()
	{
		SymbolPath.Parse(null).Elements.Should().BeEmpty();
		SymbolPath.Parse(" ; ;").Elements.Should().BeEmpty();
	}

	[Test]
	public async Task Finds_pdb_in_a_symbol_store_directory()
	{
		var fixture = SymbolFixture.Create();
		var key = PdbKey(fixture);
		var store = SymbolFixture.NewTempDirectory();
		var target = Path.Combine(store, key.Key.Replace('/', Path.DirectorySeparatorChar));
		Directory.CreateDirectory(Path.GetDirectoryName(target)!);
		File.Move(fixture.DetachPdb(), target);

		var locator = new SymbolLocator(SymbolPath.Parse(store));
		using var reader = new PEReader(File.OpenRead(fixture.AssemblyPath));

		(await locator.FindPdbAsync(reader)).Should().Be(target);
	}

	[Test]
	public async Task Finds_pdb_in_a_flat_directory()
	{
		var fixture = SymbolFixture.Create();
		var pdb = fixture.DetachPdb();

		var locator = new SymbolLocator(SymbolPath.Parse(Path.GetDirectoryName(pdb)));
		using var reader = new PEReader(File.OpenRead(fixture.AssemblyPath));

		(await locator.FindPdbAsync(reader)).Should().Be(pdb);
	}

	[Test]
	public async Task Rejects_a_pdb_with_the_right_name_but_another_id()
	{
		var fixture = SymbolFixture.Create();
		var other = SymbolFixture.Create();
		var dir = SymbolFixture.NewTempDirectory();
		File.Copy(other.PdbPath, Path.Combine(dir, "SymbolsFixture.pdb"));

		var locator = new SymbolLocator(SymbolPath.Parse(dir));
		using var reader = new PEReader(File.OpenRead(fixture.AssemblyPath));

		(await locator.FindPdbAsync(reader)).Should().BeNull("a PDB is only used when its id matches the CodeView entry");
	}

	[Test]
	public async Task Downloads_from_http_server_into_the_cache_and_sends_the_checksum()
	{
		var fixture = SymbolFixture.Create();
		var key = PdbKey(fixture);
		var handler = new FakeHttpHandler();
		handler.Add("https://symbols.example/download/symbols/" + key.Key, File.ReadAllBytes(fixture.DetachPdb()));
		var cache = SymbolFixture.NewTempDirectory();
		var symbolPath = SymbolPath.Parse("srv*https://symbols.example/download/symbols", cache);

		var locator = new SymbolLocator(symbolPath, new HttpClient(handler));
		using var reader = new PEReader(File.OpenRead(fixture.AssemblyPath));
		var path = await locator.FindPdbAsync(reader);

		path.Should().Be(Path.Combine(cache, key.Key.Replace('/', Path.DirectorySeparatorChar)));
		File.Exists(path).Should().BeTrue();
		handler.Requests.Should().ContainSingle();
		handler.Requests[0].Headers.GetValues("SymbolChecksum").Single().Should().Be(key.ChecksumHeader);

		var second = new SymbolLocator(symbolPath, new HttpClient(handler));
		(await second.FindPdbAsync(reader)).Should().Be(path);
		handler.Requests.Should().ContainSingle("a cached PDB is used without asking the server again");
	}

	[Test]
	public async Task Server_without_the_file_yields_null_and_caches_nothing()
	{
		var fixture = SymbolFixture.Create();
		fixture.DetachPdb();
		var handler = new FakeHttpHandler();
		var cache = SymbolFixture.NewTempDirectory();

		var locator = new SymbolLocator(SymbolPath.Parse("srv*https://symbols.example", cache), new HttpClient(handler));
		using var reader = new PEReader(File.OpenRead(fixture.AssemblyPath));

		(await locator.FindPdbAsync(reader)).Should().BeNull();
		Directory.EnumerateFileSystemEntries(cache).Should().BeEmpty();
	}

	[Test]
	public async Task Server_answer_with_a_mismatching_pdb_is_discarded()
	{
		var fixture = SymbolFixture.Create();
		var other = SymbolFixture.Create();
		var key = PdbKey(fixture);
		var handler = new FakeHttpHandler();
		handler.Add("https://symbols.example/" + key.Key, File.ReadAllBytes(other.PdbPath));
		var cache = SymbolFixture.NewTempDirectory();

		var locator = new SymbolLocator(SymbolPath.Parse("srv*https://symbols.example", cache), new HttpClient(handler));
		using var reader = new PEReader(File.OpenRead(fixture.AssemblyPath));

		(await locator.FindPdbAsync(reader)).Should().BeNull();
		Directory.EnumerateFiles(cache, "*", SearchOption.AllDirectories).Should().BeEmpty();
	}

	[Test]
	public async Task Opened_assembly_downloads_its_pdb_when_auto_download_is_on()
	{
		var fixture = SymbolFixture.Create();
		var key = PdbKey(fixture);
		var handler = new FakeHttpHandler();
		handler.Add("https://symbols.example/" + key.Key, File.ReadAllBytes(fixture.DetachPdb()));
		var list = new AssemblyList {
			UseDebugSymbols = true,
			SymbolLocator = new SymbolLocator(SymbolPath.Parse("srv*https://symbols.example", SymbolFixture.NewTempDirectory()),
				new HttpClient(handler)) { AutoDownload = true }
		};

		var assembly = list.OpenAssembly(fixture.AssemblyPath);
		await assembly.GetMetadataFileAsync();

		var debugInfo = assembly.GetDebugInfoOrNull();
		debugInfo.Should().NotBeNull();
		assembly.PdbFileName.Should().Be(debugInfo!.SourceFileName);
	}

	[Test]
	public async Task Opened_assembly_does_not_touch_the_network_when_auto_download_is_off()
	{
		var fixture = SymbolFixture.Create();
		fixture.DetachPdb();
		var handler = new FakeHttpHandler();
		var locator = new SymbolLocator(SymbolPath.Parse("srv*https://symbols.example", SymbolFixture.NewTempDirectory()),
			new HttpClient(handler));
		var list = new AssemblyList { UseDebugSymbols = true, SymbolLocator = locator };

		var assembly = list.OpenAssembly(fixture.AssemblyPath);
		await assembly.GetMetadataFileAsync();

		assembly.GetDebugInfoOrNull().Should().BeNull();
		handler.Requests.Should().BeEmpty();
	}

	[Test]
	public async Task Explicit_symbol_path_request_loads_the_pdb()
	{
		var fixture = SymbolFixture.Create();
		var pdb = fixture.DetachPdb();
		var list = new AssemblyList { UseDebugSymbols = false };
		var assembly = list.OpenAssembly(fixture.AssemblyPath);
		await assembly.GetMetadataFileAsync();

		var provider = await assembly.LoadDebugInfoFromSymbolPathAsync(new SymbolLocator(SymbolPath.Parse(Path.GetDirectoryName(pdb))));

		provider.Should().NotBeNull("an explicit request applies even when debug symbols are off by default");
		assembly.GetDebugInfoOrNull().Should().BeSameAs(provider);
		provider!.GetVariables(MethodHandle(assembly, "Hello")).Select(v => v.Name).Should().Contain("greeting");
	}

	static MethodDefinitionHandle MethodHandle(LoadedAssembly assembly, string name)
	{
		var metadata = assembly.GetMetadataFileOrNull()!.Metadata;
		return metadata.MethodDefinitions.Single(h => metadata.GetString(metadata.GetMethodDefinition(h).Name) == name);
	}
}
