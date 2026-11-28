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

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.TypeSystem
{
	/// <summary>
	/// The underlying type of an enum is resolved by name while its type definition is being
	/// constructed, which can recurse if the enum is named after its own underlying type.
	/// </summary>
	[TestFixture]
	public class EnumNamedLikeKnownTypeTests
	{
		[Test]
		public void EnumNamedLikeItsOwnUnderlyingType()
		{
			var compilation = new SimpleCompilation(
				BuildAssemblyWithEnums(("System", "String", PrimitiveTypeCode.String), ("System", "Int32", PrimitiveTypeCode.Int32)),
				MinimalCorlib.Instance);

			foreach (var typeDef in compilation.MainModule.TopLevelTypeDefinitions.Where(t => t.Namespace == "System"))
			{
				Assert.That(typeDef.Kind, Is.EqualTo(TypeKind.Struct), typeDef.FullName);
				Assert.That(compilation.FindType(typeDef.KnownTypeCode), Is.SameAs(typeDef), typeDef.FullName);
			}
		}

		[Test]
		public void EnumNamedLikeEachPrimitiveUnderlyingType()
		{
			var enumTypes = new[]
			{
				("Boolean", PrimitiveTypeCode.Boolean),
				("Char", PrimitiveTypeCode.Char),
				("SByte", PrimitiveTypeCode.SByte),
				("Byte", PrimitiveTypeCode.Byte),
				("Int16", PrimitiveTypeCode.Int16),
				("UInt16", PrimitiveTypeCode.UInt16),
				("Int32", PrimitiveTypeCode.Int32),
				("UInt32", PrimitiveTypeCode.UInt32),
				("Int64", PrimitiveTypeCode.Int64),
				("UInt64", PrimitiveTypeCode.UInt64),
				("Single", PrimitiveTypeCode.Single),
				("Double", PrimitiveTypeCode.Double)
			};
			var compilation = new SimpleCompilation(
				BuildAssemblyWithEnums(enumTypes.Select(t => ("System", t.Item1, t.Item2)).ToArray()),
				MinimalCorlib.Instance);

			foreach (var (name, _) in enumTypes)
			{
				var typeDef = compilation.MainModule.TopLevelTypeDefinitions.Single(t => t.Name == name);
				Assert.That(typeDef.Kind, Is.EqualTo(TypeKind.Struct), typeDef.FullName);
				Assert.That(compilation.FindType(typeDef.KnownTypeCode), Is.SameAs(typeDef), typeDef.FullName);
			}
		}

		[Test]
		public void EnumsNamedLikeEachOthersUnderlyingType()
		{
			var compilation = new SimpleCompilation(
				BuildAssemblyWithEnums(("System", "Int32", PrimitiveTypeCode.Int64), ("System", "Int64", PrimitiveTypeCode.Int32)),
				MinimalCorlib.Instance);

			foreach (var typeDef in compilation.MainModule.TopLevelTypeDefinitions.Where(t => t.Namespace == "System"))
			{
				Assert.That(typeDef.Kind, Is.EqualTo(TypeKind.Struct), typeDef.FullName);
			}
		}

		[Test]
		public void OrdinaryEnumNextToEnumNamedLikeKnownType()
		{
			var compilation = new SimpleCompilation(
				BuildAssemblyWithEnums(("System", "Int32", PrimitiveTypeCode.Int32), ("", "E", PrimitiveTypeCode.Int32)),
				MinimalCorlib.Instance);

			var typeDef = compilation.MainModule.TopLevelTypeDefinitions.Single(t => t.Name == "E");
			Assert.That(typeDef.Kind, Is.EqualTo(TypeKind.Enum));
			Assert.That(typeDef.EnumUnderlyingType, Is.SameAs(compilation.FindType(KnownTypeCode.Int32)));
		}

		/// <summary>
		/// Builds a minimal in-memory assembly with enum definitions and their underlying field types.
		/// </summary>
		static PEFile BuildAssemblyWithEnums(params (string Namespace, string Name, PrimitiveTypeCode UnderlyingType)[] enums)
		{
			var metadata = new MetadataBuilder();
			metadata.AddModule(0, metadata.GetOrAddString("test.dll"),
				metadata.GetOrAddGuid(Guid.NewGuid()), default, default);
			metadata.AddAssembly(metadata.GetOrAddString("test"), new Version(1, 0, 0, 0),
				default, default, 0, AssemblyHashAlgorithm.None);
			var mscorlib = metadata.AddAssemblyReference(metadata.GetOrAddString("mscorlib"),
				new Version(4, 0, 0, 0), default, default, 0, default);
			var enumBaseType = metadata.AddTypeReference(mscorlib,
				metadata.GetOrAddString("System"), metadata.GetOrAddString("Enum"));

			metadata.AddTypeDefinition(default, default,
				metadata.GetOrAddString("<Module>"), baseType: default,
				fieldList: MetadataTokens.FieldDefinitionHandle(1), methodList: MetadataTokens.MethodDefinitionHandle(1));
			foreach (var (ns, name, underlyingType) in enums)
			{
				var signature = new BlobBuilder();
				new BlobEncoder(signature).FieldSignature().PrimitiveType(underlyingType);
				var field = metadata.AddFieldDefinition(
					FieldAttributes.Public | FieldAttributes.SpecialName | FieldAttributes.RTSpecialName,
					metadata.GetOrAddString("value__"), metadata.GetOrAddBlob(signature));
				metadata.AddTypeDefinition(TypeAttributes.Public | TypeAttributes.Sealed,
					metadata.GetOrAddString(ns), metadata.GetOrAddString(name), enumBaseType,
					fieldList: field, methodList: MetadataTokens.MethodDefinitionHandle(1));
			}

			var peBlob = new BlobBuilder();
			new ManagedPEBuilder(PEHeaderBuilder.CreateLibraryHeader(),
				new MetadataRootBuilder(metadata), ilStream: new BlobBuilder()).Serialize(peBlob);
			return new PEFile("test.dll", new MemoryStream(peBlob.ToArray()));
		}
	}
}
