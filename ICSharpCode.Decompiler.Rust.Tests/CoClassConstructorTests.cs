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
using System.IO;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;

using ICSharpCode.Decompiler.Rust.Interop;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Rust.Tests;

/// <summary>
/// Constructor references on ComImport interfaces resolve to the constructors of the CoClass.
/// No compiler emits a constructor reference whose parent is the interface itself, so the
/// fixture is written with <see cref="MetadataBuilder"/>.
/// </summary>
[TestFixture]
public class CoClassConstructorTests
{
	[Test]
	public void ConstructorReferencesOnComImportInterfacesResolveThroughTheCoClass()
	{
		string path = WriteFixture();
		var ts = TypeSystemListingEqualityTests.Load(path, TypeSystemOptions.Default);
		using var table = new RustModuleTable(ts);
		string listing = table.DebugTypeListing();
		Assert.That(Corpus.FirstDifference(TypeSystemListingEqualityTests.ManagedListing(ts), listing), Is.Null);
		string[] lines = listing.Split('\n');
		// IFoo..ctor(int) is Foo..ctor(int) declared by IFoo.
		Assert.That(lines, Has.Some.StartsWith("R ").And.Contains("method=06000001 decl=Fixture.IFoo@CoClassFixture name=.ctor"));
		// IFoo..ctor(string) matches no Foo constructor; IBar is its own CoClass.
		Assert.That(lines, Has.Some.StartsWith("R ").And.Contains("method=fake decl=Fixture.IFoo@CoClassFixture name=.ctor"));
		Assert.That(lines, Has.Some.StartsWith("R ").And.Contains("method=fake decl=Fixture.IBar@CoClassFixture name=.ctor"));
		Assert.That(lines, Has.None.Contains("!bad"));
	}

	static string WriteFixture()
	{
		var metadata = new MetadataBuilder();
		StringHandle Str(string s) => metadata.GetOrAddString(s);
		var name = Str("CoClassFixture");
		metadata.AddModule(0, name, metadata.GetOrAddGuid(Guid.NewGuid()), default, default);
		metadata.AddAssembly(name, new Version(1, 0, 0, 0), default, default, default, AssemblyHashAlgorithm.Sha1);
		var runtime = metadata.AddAssemblyReference(Str("System.Runtime"), new Version(11, 0, 0, 0), default,
			metadata.GetOrAddBlob(new byte[] { 0xb0, 0x3f, 0x5f, 0x7f, 0x11, 0xd5, 0x0a, 0x3a }), default, default);
		TypeReferenceHandle Ref(string ns, string n) => metadata.AddTypeReference(runtime, Str(ns), Str(n));
		var obj = Ref("System", "Object");
		var type = Ref("System", "Type");

		BlobHandle Ctor(Action<ParametersEncoder> parameters, int count)
		{
			var sig = new BlobBuilder();
			new BlobEncoder(sig).MethodSignature(isInstanceMethod: true).Parameters(count, r => r.Void(), parameters);
			return metadata.GetOrAddBlob(sig);
		}
		var ctorInt = Ctor(p => p.AddParameter().Type().Int32(), 1);
		var ctorString = Ctor(p => p.AddParameter().Type().String(), 1);
		var ctorVoid = Ctor(_ => { }, 0);
		var ctorType = Ctor(p => p.AddParameter().Type().Type(type, isValueType: false), 1);
		var coClassCtor = metadata.AddMemberReference(Ref("System.Runtime.InteropServices", "CoClassAttribute"), Str(".ctor"), ctorType);
		var targetFrameworkCtor = metadata.AddMemberReference(Ref("System.Runtime.Versioning", "TargetFrameworkAttribute"), Str(".ctor"), ctorString);

		BlobHandle AttributeValue(string s)
		{
			var value = new BlobBuilder();
			value.WriteUInt16(1);
			value.WriteSerializedString(s);
			value.WriteUInt16(0);
			return metadata.GetOrAddBlob(value);
		}
		metadata.AddCustomAttribute(EntityHandle.AssemblyDefinition, targetFrameworkCtor, AttributeValue(".NETCoreApp,Version=v11.0"));

		var firstField = MetadataTokens.FieldDefinitionHandle(1);
		var firstMethod = MetadataTokens.MethodDefinitionHandle(1);
		const TypeAttributes comInterface = TypeAttributes.Public | TypeAttributes.Interface | TypeAttributes.Abstract | TypeAttributes.Import;
		metadata.AddTypeDefinition(default, default, Str("<Module>"), default, firstField, firstMethod);
		var iFoo = metadata.AddTypeDefinition(comInterface, Str("Fixture"), Str("IFoo"), default, firstField, firstMethod);
		var iBar = metadata.AddTypeDefinition(comInterface, Str("Fixture"), Str("IBar"), default, firstField, firstMethod);
		metadata.AddTypeDefinition(TypeAttributes.Public, Str("Fixture"), Str("Foo"), obj, firstField, firstMethod);
		metadata.AddCustomAttribute(iFoo, coClassCtor, AttributeValue("Fixture.Foo"));
		metadata.AddCustomAttribute(iBar, coClassCtor, AttributeValue("Fixture.IBar"));

		var il = new BlobBuilder();
		var code = new InstructionEncoder(new BlobBuilder());
		code.OpCode(ILOpCode.Ret);
		int body = new MethodBodyStreamEncoder(il).AddMethodBody(code);
		metadata.AddMethodDefinition(MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName,
			MethodImplAttributes.IL, Str(".ctor"), ctorInt, body, MetadataTokens.ParameterHandle(1));

		metadata.AddMemberReference(iFoo, Str(".ctor"), ctorInt);
		metadata.AddMemberReference(iFoo, Str(".ctor"), ctorString);
		metadata.AddMemberReference(iBar, Str(".ctor"), ctorVoid);

		var image = new BlobBuilder();
		new ManagedPEBuilder(PEHeaderBuilder.CreateLibraryHeader(), new MetadataRootBuilder(metadata), il).Serialize(image);
		string directory = Path.Combine(TestContext.CurrentContext.WorkDirectory, "CoClassFixture");
		Directory.CreateDirectory(directory);
		string path = Path.Combine(directory, "CoClassFixture.dll");
		File.WriteAllBytes(path, image.ToArray());
		return path;
	}
}
