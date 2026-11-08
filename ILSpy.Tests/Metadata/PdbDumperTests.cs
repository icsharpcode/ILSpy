// Copyright (c) 2026 Christoph Wille
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
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;

using ICSharpCode.ILSpyX.Metadata;
using ICSharpCode.ILSpyX.PdbProvider;

using Mono.Cecil.Cil;
using Mono.Cecil.Pdb;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.Metadata;

/// <summary>
/// Cecil's own symbol discovery (ReadSymbols without a provider) locates the Windows PDB reader
/// by type name through Type.GetType, which fails in a Native AOT build of ilspycmd. The dumper
/// therefore always names its provider; these tests keep it that way.
/// </summary>
[TestFixture]
public class PdbDumperTests
{
	[Test]
	public void ReaderParametersAlwaysCarryAnExplicitSymbolReaderProvider()
	{
		var parameters = PdbDumper.CreateReaderParameters(pdbFileName: null);

		Assert.That(parameters.ReadSymbols, Is.True);
		Assert.That(parameters.SymbolReaderProvider, Is.InstanceOf<PdbReaderProvider>(),
			"without an explicit PDB the Mono.Cecil.Pdb provider must decide between embedded, Portable and Windows PDBs itself");
	}

	[Test]
	public void ExplicitPortablePdbSelectsThePortableProvider()
	{
		// The test assembly ships with a Portable PDB next to it.
		string pdbFileName = Path.ChangeExtension(typeof(PdbDumperTests).Assembly.Location, ".pdb");
		Assert.That(File.Exists(pdbFileName), $"expected a Portable PDB at {pdbFileName}");

		var parameters = PdbDumper.CreateReaderParameters(pdbFileName);

		Assert.That(parameters.SymbolReaderProvider, Is.InstanceOf<PortablePdbReaderProvider>());
		Assert.That(parameters.SymbolStream, Is.Not.Null);
		parameters.SymbolStream.Dispose();
	}

	[Test]
	public void InvalidAssemblyImageIsReportedAsNoDebugSymbols()
	{
		using var assemblyStream = new MemoryStream(new byte[1]);
		using var output = new StringWriter();

		Assert.Throws<PdbDumper.NoDebugSymbolsException>(
			() => PdbDumper.Dump(assemblyStream, "invalid.dll", null, output, asJson: false));
	}

	[Test]
	public void ExplicitPdbCanBeUsedWhenAssemblyHasOversizedEmbeddedPdb()
	{
		string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(directory);
		string assemblyPath = Path.Combine(directory, "module.dll");
		string pdbPath = Path.ChangeExtension(assemblyPath, ".pdb");
		try
		{
			CreateAssemblyWithOversizedEmbeddedPdb(assemblyPath, pdbPath);
			using var output = new StringWriter();

			Assert.DoesNotThrow(() => PdbDumper.Dump(assemblyPath, pdbPath, output, asJson: false));
			Assert.That(output.ToString(), Is.Not.Empty);

			using var assemblyStream = new MemoryStream(File.ReadAllBytes(assemblyPath));
			using var streamOutput = new StringWriter();
			Assert.DoesNotThrow(
				() => PdbDumper.Dump(assemblyStream, assemblyPath, pdbPath, streamOutput, asJson: false));
			Assert.That(streamOutput.ToString(), Is.Not.Empty);
		}
		finally
		{
			Directory.Delete(directory, recursive: true);
		}
	}

	[Test]
	public void DumpRewindsAssemblyStreamBeforeReadingPeHeaders()
	{
		string assemblyPath = typeof(PdbDumperTests).Assembly.Location;
		string pdbPath = Path.ChangeExtension(assemblyPath, ".pdb");
		using var assemblyStream = new MemoryStream(File.ReadAllBytes(assemblyPath));
		assemblyStream.Position = assemblyStream.Length;
		using var output = new StringWriter();

		Assert.DoesNotThrow(
			() => PdbDumper.Dump(assemblyStream, assemblyPath, pdbPath, output, asJson: false));
		Assert.That(output.ToString(), Is.Not.Empty);
	}

	static void CreateAssemblyWithOversizedEmbeddedPdb(string assemblyPath, string pdbPath)
	{
		string sourceAssembly = typeof(PdbDumperTests).Assembly.Location;
		byte[] image = File.ReadAllBytes(sourceAssembly);
		int entryOffset;
		using (var reader = new PEReader(new MemoryStream(image)))
		{
			var entries = reader.ReadDebugDirectory();
			int entryIndex = Array.FindIndex(entries.ToArray(), entry => entry.Type != DebugDirectoryEntryType.CodeView);
			Assert.That(entryIndex, Is.GreaterThanOrEqualTo(0));

			int debugDirectoryRva = reader.PEHeaders.PEHeader!.DebugTableDirectory.RelativeVirtualAddress;
			var section = reader.PEHeaders.SectionHeaders.Single(s =>
				debugDirectoryRva >= s.VirtualAddress
				&& debugDirectoryRva - s.VirtualAddress < s.SizeOfRawData);
			int debugDirectoryOffset = section.PointerToRawData
				+ debugDirectoryRva - section.VirtualAddress;
			entryOffset = debugDirectoryOffset + entryIndex * 28;
		}

		int appendedDataOffset = image.Length;
		Array.Resize(ref image, image.Length + 8);
		image[appendedDataOffset] = (byte)'M';
		image[appendedDataOffset + 1] = (byte)'P';
		image[appendedDataOffset + 2] = (byte)'D';
		image[appendedDataOffset + 3] = (byte)'B';
		BinaryPrimitives.WriteUInt32LittleEndian(
			image.AsSpan(appendedDataOffset + 4, 4),
			(uint)(DebugInfoUtils.MaxEmbeddedPortablePdbSize + 1));
		BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(entryOffset + 12, 4),
			(uint)DebugDirectoryEntryType.EmbeddedPortablePdb);
		BinaryPrimitives.WriteInt32LittleEndian(image.AsSpan(entryOffset + 16, 4), 8);
		BinaryPrimitives.WriteInt32LittleEndian(image.AsSpan(entryOffset + 24, 4), appendedDataOffset);
		File.WriteAllBytes(assemblyPath, image);
		File.Copy(Path.ChangeExtension(sourceAssembly, ".pdb"), pdbPath);
	}
}
