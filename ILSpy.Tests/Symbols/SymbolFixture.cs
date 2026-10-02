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
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ICSharpCode.ILSpy.Tests.Symbols;

/// <summary>
/// Emits a small assembly the way a compiler does when it writes a separate portable PDB: the PE
/// carries a CodeView entry naming the PDB plus a PdbChecksum entry, and the PDB has one source
/// document, sequence points, and a Source Link map for that document.
/// </summary>
public sealed class SymbolFixture
{
	public const string TypeName = "Greeter";
	public const string DocumentPath = @"C:\src\SymbolsFixture\Greeter.cs";
	public const string SourceLinkJson = "{\"documents\":{\"C:\\\\src\\\\SymbolsFixture\\\\*\":\"https://example.invalid/raw/abc/*\"}}";
	public const string SourceLinkUri = "https://example.invalid/raw/abc/Greeter.cs";

	public string Directory { get; }
	public string AssemblyPath { get; }
	public string PdbPath { get; }
	public string Name { get; }

	SymbolFixture(string directory, string name)
	{
		Directory = directory;
		Name = name;
		AssemblyPath = Path.Combine(directory, name + ".dll");
		PdbPath = Path.Combine(directory, name + ".pdb");
	}

	public static SymbolFixture Create(string name = "SymbolsFixture")
	{
		var ab = new PersistedAssemblyBuilder(new AssemblyName(name), typeof(object).Assembly);
		var module = ab.DefineDynamicModule(name);
		var document = module.DefineDocument(DocumentPath, new Guid("3f5162f8-07c6-11d3-9053-00c04fa302a1"));
		var greeter = module.DefineType($"{name}.{TypeName}", TypeAttributes.Public | TypeAttributes.Class);
		var hello = greeter.DefineMethod("Hello", MethodAttributes.Public | MethodAttributes.Static,
			typeof(string), Type.EmptyTypes);
		var il = hello.GetILGenerator();
		var local = il.DeclareLocal(typeof(string));
		local.SetLocalSymInfo("greeting");
		il.MarkSequencePoint(document, 5, 3, 5, 30);
		il.Emit(OpCodes.Ldstr, "hello");
		il.Emit(OpCodes.Stloc_0);
		il.MarkSequencePoint(document, 6, 3, 6, 19);
		il.Emit(OpCodes.Ldloc_0);
		il.Emit(OpCodes.Ret);
		greeter.CreateType();

		var metadata = ab.GenerateMetadata(out var ilStream, out var fieldData, out var pdbMetadata);
		pdbMetadata.AddCustomDebugInformation(EntityHandle.ModuleDefinition,
			pdbMetadata.GetOrAddGuid(new Guid("CC110556-A091-4D38-9FEC-25AB9A351A6A")),
			pdbMetadata.GetOrAddBlob(Encoding.UTF8.GetBytes(SourceLinkJson)));

		var pdbBuilder = new PortablePdbBuilder(pdbMetadata, metadata.GetRowCounts(), default);
		var pdbBlob = new BlobBuilder();
		var pdbId = pdbBuilder.Serialize(pdbBlob);
		byte[] pdbBytes = pdbBlob.ToArray();

		var debugDirectory = new DebugDirectoryBuilder();
		debugDirectory.AddCodeViewEntry(@"C:\src\SymbolsFixture\obj\" + name + ".pdb", pdbId, pdbBuilder.FormatVersion);
		debugDirectory.AddPdbChecksumEntry("SHA256", SHA256.HashData(pdbBytes).ToImmutableArray());
		var peBuilder = new ManagedPEBuilder(PEHeaderBuilder.CreateLibraryHeader(), new MetadataRootBuilder(metadata),
			ilStream, fieldData, debugDirectoryBuilder: debugDirectory,
			deterministicIdProvider: content => new BlobContentId(Guid.NewGuid(), 0x12345678));
		var peBlob = new BlobBuilder();
		peBuilder.Serialize(peBlob);

		var dir = Path.Combine(Path.GetTempPath(), $"ILSpySymbolFixture_{Guid.NewGuid():N}");
		System.IO.Directory.CreateDirectory(dir);
		var fixture = new SymbolFixture(dir, name);
		File.WriteAllBytes(fixture.AssemblyPath, peBlob.ToArray());
		File.WriteAllBytes(fixture.PdbPath, pdbBytes);
		return fixture;
	}

	/// <summary>Moves the PDB away from the assembly so only a symbol path can find it.</summary>
	public string DetachPdb()
	{
		var dir = Path.Combine(Path.GetTempPath(), $"ILSpySymbolPdb_{Guid.NewGuid():N}");
		System.IO.Directory.CreateDirectory(dir);
		var target = Path.Combine(dir, Name + ".pdb");
		File.Move(PdbPath, target);
		return target;
	}

	public static string NewTempDirectory()
	{
		var dir = Path.Combine(Path.GetTempPath(), $"ILSpySymbols_{Guid.NewGuid():N}");
		System.IO.Directory.CreateDirectory(dir);
		return dir;
	}
}

/// <summary>An in-memory HTTP server: maps absolute URLs to bodies and records every request.</summary>
public sealed class FakeHttpHandler : HttpMessageHandler
{
	readonly Dictionary<string, byte[]> responses = new(StringComparer.OrdinalIgnoreCase);

	public List<HttpRequestMessage> Requests { get; } = new();

	public void Add(string url, byte[] body) => responses[url] = body;

	protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
	{
		Requests.Add(request);
		if (responses.TryGetValue(request.RequestUri!.ToString(), out var body))
			return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK) { Content = new ByteArrayContent(body) });
		return Task.FromResult(new HttpResponseMessage(HttpStatusCode.NotFound));
	}
}
