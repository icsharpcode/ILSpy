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
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.TypeSystem
{
	/// <summary>
	/// Checks bounds and error reporting for unresolved generic method references.
	/// </summary>
	[TestFixture]
	public class FakeMethodGenericParameterLimitTests
	{
		[Test]
		public void AcceptsGenericMethodWith64TypeParameters()
		{
			using var assembly = BuildAssembly("FakeMethod");
			var compilation = new SimpleCompilation(assembly, MinimalCorlib.Instance);
			var method = ResolveMissingGenericMethod(compilation, 64);

			Assert.That(method.TypeParameters, Has.Count.EqualTo(64));
		}

		[Test]
		public void RejectsGenericMethodWithMoreThan64TypeParameters()
		{
			using var assembly = BuildAssembly("FakeMethod");
			var compilation = new SimpleCompilation(assembly, MinimalCorlib.Instance);

			Assert.That(() => ResolveMissingGenericMethod(compilation, 65), Throws.TypeOf<BadImageFormatException>());
		}

		[Test]
		public void RejectsGenericMethodWithMaximumEncodedTypeParameterCount()
		{
			using var assembly = BuildAssembly("FakeMethod");
			var compilation = new SimpleCompilation(assembly, MinimalCorlib.Instance);

			Assert.That(() => ResolveMissingGenericMethod(compilation, 0x1fffffff), Throws.TypeOf<BadImageFormatException>());
		}

		[Test]
		public void OversizedAttributeConstructorProducesAnErrorAttribute()
		{
			using var assembly = BuildAssemblyWithOversizedAttributeConstructor();
			var compilation = new SimpleCompilation(assembly, MinimalCorlib.Instance);
			var attribute = compilation.MainModule.GetAssemblyAttributes().Single(a => a.AttributeType.Name == "FooAttribute");

			Assert.That(attribute.Constructor, Is.Null);
			Assert.That(attribute.HasDecodeErrors, Is.True);
			Assert.That(attribute.AttributeType.Name, Is.EqualTo("FooAttribute"));

			var resolver = new UniversalAssemblyResolver(assembly.FileName, false, assembly.Metadata.DetectTargetFrameworkId());
			var decompiler = new CSharpDecompiler(assembly, resolver, new DecompilerSettings());

			string output = decompiler.DecompileWholeModuleAsString();

			Assert.That(output, Does.Contain("Foo"));
			Assert.That(output, Does.Contain("Could not decode attribute arguments."));
		}

		static IMethod ResolveMissingGenericMethod(ICompilation compilation, int genericParameterCount)
		{
			var signature = new MethodSignature<IType>(
				new SignatureHeader(SignatureKind.Method, SignatureCallingConvention.Default, SignatureAttributes.Generic),
				compilation.FindType(KnownTypeCode.Void), requiredParameterCount: 0, genericParameterCount,
				ImmutableArray<IType>.Empty);
			return ((MetadataModule)compilation.MainModule).ResolveMethod(
				compilation.FindType(KnownTypeCode.Object), "NoSuchMethod", signature);
		}

		/// <summary>
		/// Builds an assembly containing only the required module type.
		/// </summary>
		static PEFile BuildAssembly(string name, Action<MetadataBuilder, AssemblyDefinitionHandle> addMetadata = null)
		{
			var metadata = new MetadataBuilder();
			metadata.AddModule(0, metadata.GetOrAddString(name + ".dll"),
				metadata.GetOrAddGuid(Guid.NewGuid()), default, default);
			var assembly = metadata.AddAssembly(metadata.GetOrAddString(name), new Version(1, 0, 0, 0),
				default, default, 0, AssemblyHashAlgorithm.None);
			metadata.AddTypeDefinition(default, default, metadata.GetOrAddString("<Module>"),
				baseType: default, fieldList: MetadataTokens.FieldDefinitionHandle(1),
				methodList: MetadataTokens.MethodDefinitionHandle(1));
			addMetadata?.Invoke(metadata, assembly);

			var peBlob = new BlobBuilder();
			new ManagedPEBuilder(PEHeaderBuilder.CreateLibraryHeader(),
				new MetadataRootBuilder(metadata), ilStream: new BlobBuilder()).Serialize(peBlob);
			return new PEFile(name + ".dll", new MemoryStream(peBlob.ToArray()));
		}

		static PEFile BuildAssemblyWithOversizedAttributeConstructor()
		{
			return BuildAssembly("OversizedAttribute", (metadata, assembly) => {
				var mscorlib = metadata.AddAssemblyReference(metadata.GetOrAddString("mscorlib"),
					new Version(4, 0, 0, 0), default, default, 0, default);
				var attributeType = metadata.AddTypeReference(mscorlib,
					metadata.GetOrAddString("Example"), metadata.GetOrAddString("FooAttribute"));
				var signature = new BlobBuilder();
				signature.WriteByte(0x30);
				signature.WriteByte(65);
				signature.WriteByte(0);
				signature.WriteByte(0x01);
				var constructor = metadata.AddMemberReference(attributeType,
					metadata.GetOrAddString(".ctor"), metadata.GetOrAddBlob(signature));
				var value = new BlobBuilder();
				value.WriteUInt16(1);
				value.WriteUInt16(0);
				metadata.AddCustomAttribute(assembly, constructor, metadata.GetOrAddBlob(value));
			});
		}
	}
}
