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
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Metadata;

[TestFixture]
public class NestedClassConsistencyTests
{
	static PEFile BuildAssembly(Action<MetadataBuilder, TypeDefinitionHandle[]> addNestedTypes,
		Action<byte[]> patchImage = null)
	{
		var metadata = new MetadataBuilder();
		metadata.AddModule(0, metadata.GetOrAddString("test.dll"),
			metadata.GetOrAddGuid(Guid.NewGuid()), default, default);
		metadata.AddAssembly(metadata.GetOrAddString("test"), new Version(1, 0, 0, 0),
			default, default, 0, AssemblyHashAlgorithm.None);
		metadata.AddTypeDefinition(default, default, metadata.GetOrAddString("<Module>"),
			baseType: default, fieldList: MetadataTokens.FieldDefinitionHandle(1),
			methodList: MetadataTokens.MethodDefinitionHandle(1));
		var types = new TypeDefinitionHandle[3];
		types[0] = metadata.AddTypeDefinition(TypeAttributes.Public, default,
			metadata.GetOrAddString("C"), baseType: default,
			fieldList: MetadataTokens.FieldDefinitionHandle(1), methodList: MetadataTokens.MethodDefinitionHandle(1));
		types[1] = metadata.AddTypeDefinition(TypeAttributes.Public, default,
			metadata.GetOrAddString("D"), baseType: default,
			fieldList: MetadataTokens.FieldDefinitionHandle(1), methodList: MetadataTokens.MethodDefinitionHandle(1));
		types[2] = metadata.AddTypeDefinition(TypeAttributes.Public, default,
			metadata.GetOrAddString("E"), baseType: default,
			fieldList: MetadataTokens.FieldDefinitionHandle(1), methodList: MetadataTokens.MethodDefinitionHandle(1));

		addNestedTypes(metadata, types);

		var peBlob = new BlobBuilder();
		new ManagedPEBuilder(PEHeaderBuilder.CreateLibraryHeader(),
			new MetadataRootBuilder(metadata), ilStream: new BlobBuilder()).Serialize(peBlob);
		byte[] image = peBlob.ToArray();
		patchImage?.Invoke(image);
		return new PEFile("test.dll", new MemoryStream(image));
	}

	// MetadataBuilder rejects malformed NestedClass ordering, so these tests patch a valid image.
	static int GetNestedClassTableOffset(byte[] image)
	{
		using var peReader = new PEReader(new MemoryStream(image));
		int metadataRva = peReader.PEHeaders.CorHeader!.MetadataDirectory.RelativeVirtualAddress;
		var section = peReader.PEHeaders.SectionHeaders.Single(section =>
			metadataRva >= section.VirtualAddress && metadataRva < section.VirtualAddress + section.SizeOfRawData);
		int metadataOffset = section.PointerToRawData + metadataRva - section.VirtualAddress;
		int versionLength = BinaryPrimitives.ReadInt32LittleEndian(image.AsSpan(metadataOffset + 12));
		int streamHeaderOffset = (metadataOffset + 16 + versionLength + 3) & ~3;
		ushort streamCount = BinaryPrimitives.ReadUInt16LittleEndian(image.AsSpan(streamHeaderOffset + 2));
		int streamOffset = streamHeaderOffset + 4;
		int tablesOffset = -1;
		for (int i = 0; i < streamCount; i++)
		{
			int relativeOffset = BinaryPrimitives.ReadInt32LittleEndian(image.AsSpan(streamOffset));
			int nameOffset = streamOffset + 8;
			int nameEnd = nameOffset;
			while (image[nameEnd] != 0)
				nameEnd++;
			string name = System.Text.Encoding.ASCII.GetString(image, nameOffset, nameEnd - nameOffset);
			if (name == "#~")
				tablesOffset = metadataOffset + relativeOffset;
			streamOffset = (nameEnd + 1 + 3) & ~3;
		}
		Assert.That(tablesOffset, Is.GreaterThanOrEqualTo(0));

		ulong validTables = BinaryPrimitives.ReadUInt64LittleEndian(image.AsSpan(tablesOffset + 8));
		ulong expectedTables = (1UL << 0) | (1UL << 2) | (1UL << 32) | (1UL << 41);
		Assert.That(validTables, Is.EqualTo(expectedTables));
		byte heapSizes = image[tablesOffset + 6];
		Assert.That(heapSizes & 0x07, Is.Zero);

		int rowCountOffset = tablesOffset + 24;
		int[] rowCounts = new int[64];
		for (int table = 0; table < 64; table++)
		{
			if ((validTables & (1UL << table)) == 0)
				continue;
			rowCounts[table] = BinaryPrimitives.ReadInt32LittleEndian(image.AsSpan(rowCountOffset));
			rowCountOffset += sizeof(int);
		}
		Assert.That(rowCounts[41], Is.EqualTo(2));

		int tableDataOffset = rowCountOffset;
		// The fixture has only Module, TypeDef, Assembly, and NestedClass tables with 2-byte indices.
		int moduleRowSize = 2 + 2 + 3 * 2;
		int typeDefRowSize = 4 + 2 + 2 + 2 + 2 + 2;
		int assemblyRowSize = 4 + 4 * 2 + 4 + 3 * 2;
		return tableDataOffset + rowCounts[0] * moduleRowSize
			+ rowCounts[2] * typeDefRowSize + rowCounts[32] * assemblyRowSize;
	}

	static void WriteNestedClassRow(byte[] image, int rowIndex, int nestedTypeRow, int enclosingTypeRow)
	{
		int rowOffset = GetNestedClassTableOffset(image) + rowIndex * 2 * sizeof(ushort);
		BinaryPrimitives.WriteUInt16LittleEndian(image.AsSpan(rowOffset), (ushort)nestedTypeRow);
		BinaryPrimitives.WriteUInt16LittleEndian(image.AsSpan(rowOffset + sizeof(ushort)), (ushort)enclosingTypeRow);
	}

	static TypeDefinitionHandle GetTypeHandle(MetadataReader metadata, string name)
	{
		return metadata.TypeDefinitions.Single(handle =>
			metadata.GetString(metadata.GetTypeDefinition(handle).Name) == name);
	}

	[Test]
	public void DuplicateNestedClassRowsDoNotMakeTypeNestedInItself()
	{
		using var module = BuildAssembly((metadata, types) => {
			metadata.AddNestedType(types[1], types[0]);
			metadata.AddNestedType(types[2], types[0]);
		}, image => WriteNestedClassRow(image, 1, 3, 3));

		var metadataReader = module.Metadata;
		var c = GetTypeHandle(metadataReader, "C");
		var d = GetTypeHandle(metadataReader, "D");

		Assert.That(metadataReader.GetTypeDefinition(d).GetDeclaringType(), Is.EqualTo(c));
		Assert.That(metadataReader.GetTypeDefinition(d).GetNestedTypes(metadataReader, d), Is.Empty);

		var typeSystem = new DecompilerTypeSystem(module,
			new UniversalAssemblyResolver(module.FileName, throwOnError: false, targetFramework: null));
		var type = typeSystem.MainModule.TypeDefinitions.Single(type => type.Name == "D");
		Assert.That(type.DeclaringTypeDefinition.Name, Is.EqualTo("C"));
		Assert.That(type.NestedTypes, Is.Empty);
	}

	[Test]
	public void ExactDuplicateNestedClassRowsReturnOneNestedType()
	{
		using var module = BuildAssembly((metadata, types) => {
			metadata.AddNestedType(types[1], types[0]);
			metadata.AddNestedType(types[2], types[0]);
		}, image => WriteNestedClassRow(image, 1, 3, 2));

		var metadataReader = module.Metadata;
		var c = GetTypeHandle(metadataReader, "C");
		var d = GetTypeHandle(metadataReader, "D");
		var nestedTypes = metadataReader.GetTypeDefinition(c).GetNestedTypes(metadataReader, c);

		Assert.That(nestedTypes, Is.EqualTo(new[] { d }));
	}

	[Test]
	public void UnsortedNestedClassRowsAreFilteredByDeclaringType()
	{
		using var module = BuildAssembly((metadata, types) => {
			metadata.AddNestedType(types[1], types[0]);
			metadata.AddNestedType(types[2], types[0]);
		}, image => {
			WriteNestedClassRow(image, 0, 4, 2);
			WriteNestedClassRow(image, 1, 3, 3);
		});

		var metadataReader = module.Metadata;
		var c = GetTypeHandle(metadataReader, "C");
		var d = GetTypeHandle(metadataReader, "D");

		Assert.That(metadataReader.GetTypeDefinition(c).GetNestedTypes(metadataReader, c), Does.Not.Contain(d));
	}

	[Test]
	public void DirectSelfParentRowIsExcludedFromNestedTypes()
	{
		using var module = BuildAssembly((metadata, types) => {
			metadata.AddNestedType(types[1], types[1]);
		});

		var metadataReader = module.Metadata;
		var typeHandle = GetTypeHandle(metadataReader, "D");
		var nestedTypes = metadataReader.GetTypeDefinition(typeHandle).GetNestedTypes(metadataReader, typeHandle);

		Assert.That(nestedTypes, Is.Empty);
	}

	[Test]
	public void LongerEnclosingCycleIsExcludedFromNestedTypes()
	{
		using var module = BuildAssembly((metadata, types) => {
			metadata.AddNestedType(types[0], types[1]);
			metadata.AddNestedType(types[1], types[0]);
		});

		var metadataReader = module.Metadata;
		var c = GetTypeHandle(metadataReader, "C");
		var d = GetTypeHandle(metadataReader, "D");

		Assert.That(metadataReader.GetTypeDefinition(c).GetNestedTypes(metadataReader, c), Is.Empty);
		Assert.That(metadataReader.GetTypeDefinition(d).GetNestedTypes(metadataReader, d), Is.Empty);
	}
}
