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
	[TestFixture]
	public class InterfaceUsedAsAttributeTests
	{
		[TestCase(false)]
		[TestCase(true)]
		public void InterfaceAttributedWithItself(bool comImport)
		{
			var compilation = new SimpleCompilation(
				BuildAssemblyWithInterfaces(comImport, ("", "I", AttributeType: 0)),
				MinimalCorlib.Instance);

			var typeDef = compilation.MainModule.TopLevelTypeDefinitions.Single(t => t.Name == "I");
			Assert.That(typeDef.GetConstructors(), Is.Empty);
			Assert.That(typeDef.HasAttribute(KnownAttribute.ComImport), Is.EqualTo(comImport));
			Assert.That(typeDef.GetAttributes().Select(a => a.AttributeType.FullName),
				Is.EqualTo(comImport ? ["System.Runtime.InteropServices.ComImportAttribute", "I"] : new[] { "I" }));
		}

		[Test]
		public void InterfacesAttributedWithEachOther()
		{
			var compilation = new SimpleCompilation(
				BuildAssemblyWithInterfaces(comImport: false, ("", "I", AttributeType: 1), ("", "J", AttributeType: 0)),
				MinimalCorlib.Instance);

			var i = compilation.MainModule.TopLevelTypeDefinitions.Single(t => t.Name == "I");
			var j = compilation.MainModule.TopLevelTypeDefinitions.Single(t => t.Name == "J");
			Assert.That(i.GetAttributes().Single().AttributeType, Is.SameAs(j));
			Assert.That(j.GetAttributes().Single().AttributeType, Is.SameAs(i));
		}

		[Test]
		public void ComInterfaceNamedCoClassAttributeAttributedWithItself()
		{
			var compilation = new SimpleCompilation(
				BuildAssemblyWithInterfaces(comImport: true, ("System.Runtime.InteropServices", "CoClassAttribute", AttributeType: 0)),
				MinimalCorlib.Instance);

			var typeDef = compilation.MainModule.TopLevelTypeDefinitions.Single(t => t.Name == "CoClassAttribute");
			Assert.That(typeDef.GetConstructors(), Is.Empty);
			Assert.That(typeDef.GetAttribute(KnownAttribute.CoClass).AttributeType, Is.SameAs(typeDef));
		}

		static PEFile BuildAssemblyWithInterfaces(bool comImport, params (string Namespace, string Name, int AttributeType)[] interfaces)
		{
			var metadata = new MetadataBuilder();
			metadata.AddModule(0, metadata.GetOrAddString("test.dll"),
				metadata.GetOrAddGuid(Guid.NewGuid()), default, default);
			metadata.AddAssembly(metadata.GetOrAddString("test"), new Version(1, 0, 0, 0),
				default, default, 0, AssemblyHashAlgorithm.None);

			metadata.AddTypeDefinition(default, default,
				metadata.GetOrAddString("<Module>"), baseType: default,
				fieldList: MetadataTokens.FieldDefinitionHandle(1), methodList: MetadataTokens.MethodDefinitionHandle(1));
			var attributes = TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract;
			if (comImport)
				attributes |= TypeAttributes.Import;
			var handles = interfaces.Select(i => metadata.AddTypeDefinition(attributes,
				metadata.GetOrAddString(i.Namespace), metadata.GetOrAddString(i.Name), baseType: default,
				fieldList: MetadataTokens.FieldDefinitionHandle(1), methodList: MetadataTokens.MethodDefinitionHandle(1))).ToArray();

			var signature = new BlobBuilder();
			new BlobEncoder(signature).MethodSignature(isInstanceMethod: true)
				.Parameters(0, returnType => returnType.Void(), parameters => { });
			for (int i = 0; i < interfaces.Length; i++)
			{
				var ctor = metadata.AddMemberReference(handles[interfaces[i].AttributeType],
					metadata.GetOrAddString(".ctor"), metadata.GetOrAddBlob(signature));
				metadata.AddCustomAttribute(handles[i], ctor, metadata.GetOrAddBlob(new byte[] { 1, 0, 0, 0 }));
			}

			var peBlob = new BlobBuilder();
			new ManagedPEBuilder(PEHeaderBuilder.CreateLibraryHeader(),
				new MetadataRootBuilder(metadata), ilStream: new BlobBuilder()).Serialize(peBlob);
			return new PEFile("test.dll", new MemoryStream(peBlob.ToArray()));
		}
	}
}
