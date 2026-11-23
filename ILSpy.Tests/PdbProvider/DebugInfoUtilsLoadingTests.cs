// Copyright (c) 2026 Siegfried Pammer
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
using System.Text;

using ICSharpCode.Decompiler.Metadata;

using ICSharpCode.ILSpyX.PdbProvider;

using NUnit.Framework;

namespace ICSharpCode.ILSpy.Tests.PdbProvider;

[TestFixture]
public class DebugInfoUtilsLoadingTests
{
	[Test]
	public void OpenStreamCopiesPdbFileIntoMemory()
	{
		string path = Path.GetTempFileName();
		File.WriteAllBytes(path, [1, 2, 3, 4]);
		try
		{
			using var stream = DebugInfoUtils.OpenStream(path);
			Assert.That(stream, Is.TypeOf<MemoryStream>());
			File.WriteAllBytes(path, [5, 6, 7, 8]);
			Assert.That(stream!.ReadByte(), Is.EqualTo(1));
		}
		finally
		{
			File.Delete(path);
		}
	}

	[Test]
	public void LoadSymbolsDoesNotFallBackToCecilForOversizedEmbeddedPdb()
	{
		byte[] image = CreateImageWithOversizedEmbeddedPdbHeader();
		string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(directory);
		string assemblyPath = Path.Combine(directory, "module.dll");
		File.WriteAllBytes(assemblyPath, image);
		File.WriteAllBytes(Path.ChangeExtension(assemblyPath, ".pdb"), Array.Empty<byte>());

		try
		{
			using var module = new PEFile(assemblyPath);
			Assert.That(DebugInfoUtils.LoadSymbols(module), Is.Null);
		}
		finally
		{
			Directory.Delete(directory, recursive: true);
		}
	}

	[Test]
	public void FromFileNativePdbDoesNotSelectOversizedEmbeddedPdb()
	{
		string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(directory);
		string assemblyPath = Path.Combine(directory, "module.dll");
		string pdbPath = Path.ChangeExtension(assemblyPath, ".pdb");
		try
		{
			File.WriteAllBytes(assemblyPath, CreateImageWithOversizedEmbeddedPdbHeader());
			File.WriteAllBytes(pdbPath, Encoding.ASCII.GetBytes("Microsoft C/C++ MSF 7.00"));
			using var module = new PEFile(assemblyPath);

			var exception = Assert.Catch(() => DebugInfoUtils.FromFile(module, pdbPath));

			Assert.That(exception, Is.Not.Null);
			Assert.That(exception!.ToString(), Does.Contain("PdbFile.LoadFunctions"));
		}
		finally
		{
			Directory.Delete(directory, recursive: true);
		}
	}

	[Test]
	public void LoadSymbolsUsesPortableSidecarWithNonPortableCodeView()
	{
		byte[] image = File.ReadAllBytes(typeof(MetadataFile).Assembly.Location);
		int codeViewOffset;
		int embeddedEntryOffset;
		using (var reader = new PEReader(new MemoryStream(image)))
		{
			var entries = reader.ReadDebugDirectory();
			int codeViewIndex = Array.FindIndex(entries.ToArray(),
				entry => entry.Type == DebugDirectoryEntryType.CodeView);
			int embeddedEntryIndex = Array.FindIndex(entries.ToArray(),
				entry => entry.Type == DebugDirectoryEntryType.EmbeddedPortablePdb);
			Assert.That(codeViewIndex, Is.GreaterThanOrEqualTo(0));
			Assert.That(embeddedEntryIndex, Is.GreaterThanOrEqualTo(0));
			int debugDirectoryRva = reader.PEHeaders.PEHeader!.DebugTableDirectory.RelativeVirtualAddress;
			var section = reader.PEHeaders.SectionHeaders.Single(s =>
				debugDirectoryRva >= s.VirtualAddress
				&& debugDirectoryRva - s.VirtualAddress < s.SizeOfRawData);
			int debugDirectoryOffset = section.PointerToRawData + debugDirectoryRva - section.VirtualAddress;
			codeViewOffset = debugDirectoryOffset + codeViewIndex * 28;
			embeddedEntryOffset = debugDirectoryOffset + embeddedEntryIndex * 28;
		}
		BinaryPrimitives.WriteUInt16LittleEndian(image.AsSpan(codeViewOffset + 8, 2), 0);
		BinaryPrimitives.WriteUInt16LittleEndian(image.AsSpan(codeViewOffset + 10, 2), 0);
		BinaryPrimitives.WriteUInt32LittleEndian(image.AsSpan(embeddedEntryOffset + 12, 4), 0);
		using (var patchedReader = new PEReader(new MemoryStream(image)))
		{
			Assert.That(patchedReader.ReadDebugDirectory()
				.Single(entry => entry.Type == DebugDirectoryEntryType.CodeView).IsPortableCodeView, Is.False);
		}

		string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(directory);
		string assemblyPath = Path.Combine(directory, "module.dll");
		File.WriteAllBytes(assemblyPath, image);
		string sidecarPath = Path.ChangeExtension(typeof(DebugInfoUtilsLoadingTests).Assembly.Location, ".pdb");
		Assert.That(File.Exists(sidecarPath), Is.True);
		File.Copy(sidecarPath, Path.ChangeExtension(assemblyPath, ".pdb"));

		try
		{
			using var module = new PEFile(assemblyPath);
			using var provider = DebugInfoUtils.LoadSymbols(module) as PortableDebugInfoProvider;
			Assert.That(provider, Is.Not.Null);
			Assert.That(provider!.IsEmbedded, Is.False);
		}
		finally
		{
			Directory.Delete(directory, recursive: true);
		}
	}

	static byte[] CreateImageWithOversizedEmbeddedPdbHeader()
	{
		byte[] image = File.ReadAllBytes(typeof(MetadataFile).Assembly.Location);
		using var reader = new PEReader(new MemoryStream(image));
		var entries = reader.ReadDebugDirectory();
		int embeddedEntryIndex = Array.FindIndex(entries.ToArray(),
			entry => entry.Type == DebugDirectoryEntryType.EmbeddedPortablePdb);
		Assert.That(embeddedEntryIndex, Is.GreaterThanOrEqualTo(0));

		int debugDirectoryRva = reader.PEHeaders.PEHeader!.DebugTableDirectory.RelativeVirtualAddress;
		var section = reader.PEHeaders.SectionHeaders.Single(s =>
			debugDirectoryRva >= s.VirtualAddress
			&& debugDirectoryRva - s.VirtualAddress < s.SizeOfRawData);
		int debugDirectoryOffset = section.PointerToRawData
			+ debugDirectoryRva - section.VirtualAddress;
		int dataPointerFieldOffset = debugDirectoryOffset + embeddedEntryIndex * 28 + 24;

		int appendedDataOffset = image.Length;
		Array.Resize(ref image, image.Length + 8);
		image[appendedDataOffset] = (byte)'M';
		image[appendedDataOffset + 1] = (byte)'P';
		image[appendedDataOffset + 2] = (byte)'D';
		image[appendedDataOffset + 3] = (byte)'B';
		BinaryPrimitives.WriteUInt32LittleEndian(
			image.AsSpan(appendedDataOffset + 4, 4),
			(uint)(DebugInfoUtils.MaxEmbeddedPortablePdbSize + 1));
		BinaryPrimitives.WriteInt32LittleEndian(
			image.AsSpan(dataPointerFieldOffset, 4), appendedDataOffset);
		return image;
	}
}
