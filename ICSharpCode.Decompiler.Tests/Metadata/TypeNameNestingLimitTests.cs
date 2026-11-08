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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Metadata;

[TestFixture]
public class TypeNameNestingLimitTests
{
	const int NestingLimit = 128;

	static PEFile BuildAssembly(string name, Action<MetadataBuilder> addMetadata)
	{
		var metadata = new MetadataBuilder();
		metadata.AddModule(0, metadata.GetOrAddString(name + ".dll"),
			metadata.GetOrAddGuid(Guid.NewGuid()), default, default);
		metadata.AddAssembly(metadata.GetOrAddString(name), new Version(1, 0, 0, 0),
			default, default, 0, AssemblyHashAlgorithm.None);
		metadata.AddTypeDefinition(default, default, metadata.GetOrAddString("<Module>"),
			baseType: default, fieldList: MetadataTokens.FieldDefinitionHandle(1),
			methodList: MetadataTokens.MethodDefinitionHandle(1));

		addMetadata(metadata);

		var peBlob = new BlobBuilder();
		new ManagedPEBuilder(PEHeaderBuilder.CreateLibraryHeader(),
			new MetadataRootBuilder(metadata), ilStream: new BlobBuilder()).Serialize(peBlob);
		return new PEFile(name + ".dll", new MemoryStream(peBlob.ToArray()));
	}

	static PEFile BuildAssemblyWithNestedTypes(int nestingDepth)
	{
		return BuildAssembly("NestedTypes", metadata => {
			var types = new TypeDefinitionHandle[nestingDepth + 1];
			for (int i = 0; i < types.Length; i++)
			{
				var attributes = i == 0 ? TypeAttributes.Public : TypeAttributes.NestedPublic;
				types[i] = metadata.AddTypeDefinition(attributes, default,
					metadata.GetOrAddString($"Level{i}"), baseType: default,
					fieldList: MetadataTokens.FieldDefinitionHandle(1),
					methodList: MetadataTokens.MethodDefinitionHandle(1));
			}
			for (int i = 1; i < types.Length; i++)
				metadata.AddNestedType(types[i], types[i - 1]);
		});
	}

	static PEFile BuildAssemblyWithSelfNestedType()
	{
		return BuildAssembly("SelfNestedType", metadata => {
			var type = metadata.AddTypeDefinition(TypeAttributes.NestedPublic, default,
				metadata.GetOrAddString("Self"), baseType: default,
				fieldList: MetadataTokens.FieldDefinitionHandle(1),
				methodList: MetadataTokens.MethodDefinitionHandle(1));
			metadata.AddNestedType(type, type);
		});
	}

	static PEFile BuildAssemblyWithMutuallyNestedTypes()
	{
		return BuildAssembly("MutuallyNestedTypes", metadata => {
			var first = metadata.AddTypeDefinition(TypeAttributes.NestedPublic, default,
				metadata.GetOrAddString("First"), baseType: default,
				fieldList: MetadataTokens.FieldDefinitionHandle(1),
				methodList: MetadataTokens.MethodDefinitionHandle(1));
			var second = metadata.AddTypeDefinition(TypeAttributes.NestedPublic, default,
				metadata.GetOrAddString("Second"), baseType: default,
				fieldList: MetadataTokens.FieldDefinitionHandle(1),
				methodList: MetadataTokens.MethodDefinitionHandle(1));
			metadata.AddNestedType(first, second);
			metadata.AddNestedType(second, first);
		});
	}

	static PEFile BuildAssemblyWithSelfScopedTypeReference()
	{
		return BuildAssembly("SelfScopedTypeReference", metadata => {
			var typeReference = metadata.AddTypeReference(MetadataTokens.TypeReferenceHandle(1),
				metadata.GetOrAddString("Example"), metadata.GetOrAddString("Self"));
		});
	}

	static PEFile BuildAssemblyWithTypeExtendingSelfScopedTypeReference()
	{
		return BuildAssembly("SelfScopedTypeReferenceBase", metadata => {
			var typeReference = metadata.AddTypeReference(MetadataTokens.TypeReferenceHandle(1),
				metadata.GetOrAddString("Example"), metadata.GetOrAddString("Self"));
			metadata.AddTypeDefinition(TypeAttributes.Public, default, metadata.GetOrAddString("Derived"),
				baseType: typeReference, fieldList: MetadataTokens.FieldDefinitionHandle(1),
				methodList: MetadataTokens.MethodDefinitionHandle(1));
		});
	}

	static CSharpDecompiler CreateDecompiler(PEFile module)
	{
		var resolver = new UniversalAssemblyResolver(module.FileName, false, module.Metadata.DetectTargetFrameworkId());
		return new CSharpDecompiler(module, resolver, new DecompilerSettings());
	}

	static PEFile BuildAssemblyWithSelfImplementedExportedType()
	{
		return BuildAssembly("SelfImplementedExportedType", metadata =>
			metadata.AddExportedType(TypeAttributes.Public, metadata.GetOrAddString("Example"),
				metadata.GetOrAddString("Self"), MetadataTokens.ExportedTypeHandle(1), 1));
	}

	static PEFile BuildAssemblyWithNestedTypeReferences(int nestingDepth)
	{
		return BuildAssembly("NestedTypeReferences", metadata => {
			var assemblyReference = metadata.AddAssemblyReference(metadata.GetOrAddString("Outer"),
				new Version(1, 0, 0, 0), default, default, 0, default);
			TypeReferenceHandle outer = default;
			for (int i = 0; i <= nestingDepth; i++)
			{
				EntityHandle scope = i == 0 ? assemblyReference : outer;
				outer = metadata.AddTypeReference(scope, default, metadata.GetOrAddString($"Level{i}"));
			}
		});
	}

	static PEFile BuildAssemblyWithNestedExportedTypes(int nestingDepth)
	{
		return BuildAssembly("NestedExportedTypes", metadata => {
			var assemblyReference = metadata.AddAssemblyReference(metadata.GetOrAddString("Outer"),
				new Version(1, 0, 0, 0), default, default, 0, default);
			EntityHandle implementation = assemblyReference;
			for (int i = 0; i <= nestingDepth; i++)
			{
				var attributes = i == 0 ? TypeAttributes.Public : TypeAttributes.NestedPublic;
				implementation = metadata.AddExportedType(attributes,
					i == 0 ? metadata.GetOrAddString("Example") : default,
					metadata.GetOrAddString($"Level{i}"), implementation, i + 1);
			}
		});
	}

	static PEFile BuildAssemblyWithMalformedAndValidInterfaces()
	{
		return BuildAssembly("Interfaces", metadata => {
			var goodInterface = metadata.AddTypeDefinition(TypeAttributes.Interface | TypeAttributes.Abstract
				| TypeAttributes.Public, default, metadata.GetOrAddString("IGood"), baseType: default,
				fieldList: MetadataTokens.FieldDefinitionHandle(1),
				methodList: MetadataTokens.MethodDefinitionHandle(1));
			var type = metadata.AddTypeDefinition(TypeAttributes.Public, default,
				metadata.GetOrAddString("C"), baseType: default,
				fieldList: MetadataTokens.FieldDefinitionHandle(1),
				methodList: MetadataTokens.MethodDefinitionHandle(1));
			var malformedInterface = metadata.AddTypeReference(MetadataTokens.TypeReferenceHandle(1),
				metadata.GetOrAddString("Example"), metadata.GetOrAddString("IBad"));
			metadata.AddInterfaceImplementation(type, goodInterface);
			metadata.AddInterfaceImplementation(type, malformedInterface);
		});
	}

	[Test]
	public void SelfNestedTypeIsRejected()
	{
		using var module = BuildAssemblyWithSelfNestedType();
		var handle = module.Metadata.TypeDefinitions.Last();

		Assert.Throws<BadImageFormatException>(() => handle.GetFullTypeName(module.Metadata));
		Assert.Throws<BadImageFormatException>(() => module.GetIdString(handle));
	}

	[Test]
	public void FullDecompilationRejectsSelfNestedType()
	{
		using var module = BuildAssemblyWithSelfNestedType();
		var decompiler = CreateDecompiler(module);

		var exception = Assert.Throws<BadImageFormatException>(() => decompiler.DecompileWholeModuleAsString());

		Assert.That(exception!.Message, Does.Contain("nesting chain contains a cycle"));
	}

	[Test]
	public void MutuallyNestedTypesAreRejected()
	{
		using var module = BuildAssemblyWithMutuallyNestedTypes();
		var handle = module.Metadata.TypeDefinitions.Last();

		Assert.Throws<BadImageFormatException>(() => handle.GetFullTypeName(module.Metadata));
	}

	[Test]
	public void FullDecompilationRejectsMutuallyNestedTypes()
	{
		using var module = BuildAssemblyWithMutuallyNestedTypes();
		var decompiler = CreateDecompiler(module);

		var exception = Assert.Throws<BadImageFormatException>(() => decompiler.DecompileWholeModuleAsString());

		Assert.That(exception!.Message, Does.Contain("nesting chain contains a cycle"));
	}

	[Test]
	public void SelfScopedTypeReferenceIsRejected()
	{
		using var module = BuildAssemblyWithSelfScopedTypeReference();
		var handle = module.Metadata.TypeReferences.Single();
		var resolver = new UniversalAssemblyResolver(module.FileName, false, module.Metadata.DetectTargetFrameworkId());
		var decompiler = new CSharpDecompiler(module, resolver, new DecompilerSettings());

		Assert.Throws<BadImageFormatException>(() => decompiler.TypeSystem.MainModule.GetDeclaringModule(handle));
		Assert.Throws<BadImageFormatException>(() => handle.GetFullTypeName(module.Metadata));
		Assert.Throws<BadImageFormatException>(() =>
			((EntityHandle)handle).WriteTo(module, new PlainTextOutput(), default));
	}

	[Test]
	public void FullDecompilationOfTypeWithSelfScopedBaseDoesNotOverflow()
	{
		using var module = BuildAssemblyWithTypeExtendingSelfScopedTypeReference();
		var decompiler = CreateDecompiler(module);

		Assert.DoesNotThrow(() => decompiler.DecompileWholeModuleAsString());
	}

	[Test]
	public void SelfImplementedExportedTypeIsRejected()
	{
		using var module = BuildAssemblyWithSelfImplementedExportedType();
		var exportedType = module.Metadata.GetExportedType(MetadataTokens.ExportedTypeHandle(1));

		Assert.Throws<BadImageFormatException>(() => exportedType.GetFullTypeName(module.Metadata));
	}

	[Test]
	public void DeepTypeReferenceChainRetainsItsFullName()
	{
		using var module = BuildAssemblyWithNestedTypeReferences(NestingLimit + 1);
		var deepestTypeReference = module.Metadata.TypeReferences.Last();

		var fullTypeName = deepestTypeReference.GetFullTypeName(module.Metadata);

		Assert.That(fullTypeName.TopLevelTypeName.Name, Is.EqualTo("Level0"));
		Assert.That(fullTypeName.Name, Is.EqualTo($"Level{NestingLimit + 1}"));
	}

	[Test]
	public void DeepExportedTypeChainRetainsItsFullName()
	{
		using var module = BuildAssemblyWithNestedExportedTypes(NestingLimit + 1);
		var deepestExportedType = module.Metadata.GetExportedType(
			MetadataTokens.ExportedTypeHandle(NestingLimit + 2));

		var fullTypeName = deepestExportedType.GetFullTypeName(module.Metadata);

		Assert.That(fullTypeName.TopLevelTypeName.Name, Is.EqualTo("Level0"));
		Assert.That(fullTypeName.Name, Is.EqualTo($"Level{NestingLimit + 1}"));
	}

	[Test]
	public void MalformedInterfaceDoesNotHideValidInterfaces()
	{
		using var module = BuildAssemblyWithMalformedAndValidInterfaces();
		var resolver = new UniversalAssemblyResolver(module.FileName, false, module.Metadata.DetectTargetFrameworkId());
		var typeSystem = new DecompilerTypeSystem(module, resolver);
		var type = typeSystem.MainModule.TypeDefinitions.Single(t => t.Name == "C");

		var baseTypes = type.DirectBaseTypes.ToArray();

		Assert.That(baseTypes, Has.Length.EqualTo(2));
		Assert.That(baseTypes.Any(t => t.Name == "IGood"), Is.True);
		Assert.That(baseTypes.Any(t => t.Kind == TypeKind.Unknown), Is.True);
		Assert.DoesNotThrow(() => type.GetAllBaseTypes().ToArray());
		Assert.DoesNotThrow(() => type.GetNonInterfaceBaseTypes().ToArray());
	}

	[Test]
	public void AssemblyWithTypesBeyondNestingLimitIsDecompiled()
	{
		using var module = BuildAssemblyWithNestedTypes(NestingLimit + 1);
		var deepestType = module.Metadata.TypeDefinitions.Last();
		var fullTypeName = deepestType.GetFullTypeName(module.Metadata);
		Assert.That(fullTypeName.TopLevelTypeName.Name, Is.EqualTo("Level0"));

		var resolver = new UniversalAssemblyResolver(module.FileName, false, module.Metadata.DetectTargetFrameworkId());
		var decompiler = new CSharpDecompiler(module, resolver, new DecompilerSettings());

		string code = decompiler.DecompileWholeModuleAsString();

		Assert.That(code, Does.Contain("Level129"));
	}
}
