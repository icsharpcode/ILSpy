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

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Metadata;

// Resource lengths are used to create an unmanaged stream over the loaded PE image and must stay
// within the resource section.
[TestFixture]
public class MetadataResourceTests
{
	const int PaddingLength = 64;
	const int TailOffset = sizeof(int) + PaddingLength;
	static readonly byte[] TailContent = "TAILMARK"u8.ToArray();

	static PEFile BuildAssemblyWithResources(int claimedTailLength)
	{
		var metadata = new MetadataBuilder();
		metadata.AddModule(0, metadata.GetOrAddString("test.dll"),
			metadata.GetOrAddGuid(Guid.NewGuid()), default, default);
		metadata.AddAssembly(metadata.GetOrAddString("test"), new Version(1, 0, 0, 0),
			default, default, 0, AssemblyHashAlgorithm.None);
		metadata.AddTypeDefinition(default, default,
			metadata.GetOrAddString("<Module>"), baseType: default,
			fieldList: MetadataTokens.FieldDefinitionHandle(1), methodList: MetadataTokens.MethodDefinitionHandle(1));

		var resources = new BlobBuilder();
		metadata.AddManifestResource(ManifestResourceAttributes.Public,
			metadata.GetOrAddString("padding.bin"), default, 0);
		resources.WriteInt32(PaddingLength);
		resources.WriteBytes((byte)'P', PaddingLength);
		metadata.AddManifestResource(ManifestResourceAttributes.Public,
			metadata.GetOrAddString("tail.bin"), default, TailOffset);
		resources.WriteInt32(claimedTailLength);
		resources.WriteBytes(TailContent);

		var peBlob = new BlobBuilder();
		new ManagedPEBuilder(PEHeaderBuilder.CreateLibraryHeader(),
			new MetadataRootBuilder(metadata), ilStream: new BlobBuilder(),
			managedResources: resources).Serialize(peBlob);
		return new PEFile("test.dll", new MemoryStream(peBlob.ToArray()));
	}

	static Resource GetTail(PEFile module) => module.Resources.Single(r => r.Name == "tail.bin");

	static int GetBytesAvailableForTail()
	{
		using var module = BuildAssemblyWithResources(TailContent.Length);
		int rva = module.CorHeader!.ResourcesDirectory.RelativeVirtualAddress;
		return module.GetSectionData(rva).Length - TailOffset - sizeof(int);
	}

	[Test]
	public void WellFormedResource_IsRead()
	{
		using var module = BuildAssemblyWithResources(TailContent.Length);
		var tail = GetTail(module);
		Assert.That(tail.TryGetLength(), Is.EqualTo(TailContent.Length));
		using var stream = tail.TryOpenStream();
		Assert.That(stream, Is.Not.Null);
		var content = new MemoryStream();
		stream.CopyTo(content);
		Assert.That(content.ToArray(), Is.EqualTo(TailContent));
	}

	[Test]
	public void LengthUpToEndOfSection_IsAccepted()
	{
		int available = GetBytesAvailableForTail();
		using var module = BuildAssemblyWithResources(available);
		var tail = GetTail(module);
		Assert.That(tail.TryGetLength(), Is.EqualTo(available));
		using var stream = tail.TryOpenStream();
		Assert.That(stream, Is.Not.Null);
		Assert.That(stream.Length, Is.EqualTo(available));
	}

	[Test]
	public void LengthPastEndOfSection_IsRejected()
	{
		using var module = BuildAssemblyWithResources(GetBytesAvailableForTail() + 1);
		var tail = GetTail(module);
		Assert.That(tail.TryGetLength(), Is.Null);
		Assert.That(tail.TryOpenStream(), Is.Null);
	}

	[Test]
	public void NegativeLength_IsRejected()
	{
		using var module = BuildAssemblyWithResources(-1);
		var tail = GetTail(module);
		Assert.That(tail.TryGetLength(), Is.Null);
		Assert.That(tail.TryOpenStream(), Is.Null);
	}
}
