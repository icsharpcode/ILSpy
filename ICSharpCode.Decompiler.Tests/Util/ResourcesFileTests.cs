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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using ICSharpCode.Decompiler.Util;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Util
{
	/// <summary>
	/// Every count and length in a .resources file is attacker-controlled once a crafted
	/// assembly is opened. These tests pin down that each one is validated against the data
	/// that actually exists before it drives an allocation, so a malformed file fails with a
	/// BadImageFormatException instead of an out-of-memory condition.
	/// </summary>
	[TestFixture]
	public class ResourcesFileTests
	{
		const string DefaultReaderType = "System.Resources.ResourceReader, mscorlib";
		const string DeserializingReaderType = "System.Resources.Extensions.DeserializingResourceReader, System.Resources.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51";

		// ResourceTypeCode values as written into the data section.
		const int TypeCodeString = 1;
		const int TypeCodeByteArray = 0x20;
		const int TypeCodeStream = 33;
		const int TypeCodeStartOfUserTypes = 0x40;

		[Test]
		public void WellFormedFile_ReadsAllEntryKinds()
		{
			var blob = Build(DeserializingReaderType, new[] { "MyType, MyAssembly" }, new[] {
				("text", StringPayload("hello")),
				("blob", LengthPrefixedPayload(TypeCodeByteArray, new byte[] { 1, 2, 3 })),
				("stream", LengthPrefixedPayload(TypeCodeStream, new byte[] { 4, 5 })),
				("obj", SerializedObjectPayload(typeIndex: 0, kind: 2, new byte[] { 9, 8, 7 })),
			});

			var entries = new ResourcesFile(new MemoryStream(blob)).ToDictionary(e => e.Key, e => e.Value);

			Assert.That(entries["text"], Is.EqualTo("hello"));
			Assert.That(entries["blob"], Is.EqualTo(new byte[] { 1, 2, 3 }));
			Assert.That(((MemoryStream)entries["stream"]!).ToArray(), Is.EqualTo(new byte[] { 4, 5 }));
			var obj = (ResourceSerializedObject)entries["obj"]!;
			Assert.That(obj.TypeName, Is.EqualTo("MyType, MyAssembly"));
			using var objStream = obj.GetStream();
			Assert.That(ReadAll(objStream), Is.EqualTo(new byte[] { 9, 8, 7 }));
		}

		[Test]
		public void TypeCountBeyondStream_ThrowsInsteadOfAllocating()
		{
			// int.MaxValue string references is a multi-gigabyte allocation request; the
			// header only has room for a handful of type names.
			var blob = BuildHeaderOnly(numResources: 0, numTypes: int.MaxValue);

			Assert.Throws<BadImageFormatException>(() => new ResourcesFile(new MemoryStream(blob)));
		}

		[Test]
		public void ResourceCountBeyondStream_ThrowsBeforeReadingPositions()
		{
			// Each resource needs at least a 4-byte name hash and a 4-byte name position, so a
			// count larger than the remaining bytes can be rejected up front rather than after
			// the position array has been allocated and the read has run off the end.
			var blob = BuildHeaderOnly(numResources: 100_000, numTypes: 0);

			Assert.Throws<BadImageFormatException>(() => new ResourcesFile(new MemoryStream(blob)));
		}

		[Test]
		public void ResourceNameLengthBeyondStream_ThrowsInsteadOfAllocating()
		{
			var blob = Build(DefaultReaderType, Array.Empty<string>(), new[] {
				("name", StringPayload("value")),
			}, forcedNameByteLength: int.MaxValue);

			Assert.Throws<BadImageFormatException>(() => new ResourcesFile(new MemoryStream(blob)).ToList());
		}

		[TestCase(TypeCodeByteArray)]
		[TestCase(TypeCodeStream)]
		public void BinaryResourceLengthBeyondStream_ThrowsInsteadOfAllocating(int typeCode)
		{
			var payload = new MemoryStream();
			using (var w = new BinaryWriter(payload, Encoding.UTF8, leaveOpen: true))
			{
				w.Write7BitEncodedInt(typeCode);
				w.Write(int.MaxValue); // declared length, far beyond the file
				w.Write(new byte[] { 1, 2, 3 });
			}
			var blob = Build(DefaultReaderType, Array.Empty<string>(), new[] { ("bin", payload.ToArray()) });

			Assert.Throws<BadImageFormatException>(() => new ResourcesFile(new MemoryStream(blob)).ToList());
		}

		[Test]
		public void SerializedObjectLengthBeyondStream_ThrowsInsteadOfAllocating()
		{
			var blob = Build(DeserializingReaderType, new[] { "MyType, MyAssembly" }, new[] {
				("obj", SerializedObjectPayload(typeIndex: 0, kind: 1, new byte[] { 1 }, forcedLength: int.MaxValue)),
			});
			var obj = (ResourceSerializedObject)new ResourcesFile(new MemoryStream(blob)).Single().Value!;

			Assert.Throws<BadImageFormatException>(() => obj.GetStream());
		}

		[Test]
		public void SerializedObjectWithUnknownFormatKind_Throws()
		{
			// Only the four SerializationFormat kinds defined by System.Resources.Extensions
			// are valid; anything else means the data section is not what the header claims.
			var blob = Build(DeserializingReaderType, new[] { "MyType, MyAssembly" }, new[] {
				("obj", SerializedObjectPayload(typeIndex: 0, kind: 99, new byte[] { 1 })),
			});
			var obj = (ResourceSerializedObject)new ResourcesFile(new MemoryStream(blob)).Single().Value!;

			Assert.Throws<BadImageFormatException>(() => obj.GetStream());
		}

		static byte[] StringPayload(string value)
		{
			var ms = new MemoryStream();
			using (var w = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
			{
				w.Write7BitEncodedInt(TypeCodeString);
				w.Write(value);
			}
			return ms.ToArray();
		}

		static byte[] LengthPrefixedPayload(int typeCode, byte[] data)
		{
			var ms = new MemoryStream();
			using (var w = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
			{
				w.Write7BitEncodedInt(typeCode);
				w.Write(data.Length);
				w.Write(data);
			}
			return ms.ToArray();
		}

		static byte[] SerializedObjectPayload(int typeIndex, int kind, byte[] data, int? forcedLength = null)
		{
			var ms = new MemoryStream();
			using (var w = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
			{
				w.Write7BitEncodedInt(TypeCodeStartOfUserTypes + typeIndex);
				w.Write7BitEncodedInt(kind);
				w.Write7BitEncodedInt(forcedLength ?? data.Length);
				w.Write(data);
			}
			return ms.ToArray();
		}

		static byte[] ReadAll(Stream stream)
		{
			var ms = new MemoryStream();
			stream.CopyTo(ms);
			return ms.ToArray();
		}

		/// <summary>
		/// Writes the ResourceManager and RuntimeResourceSet headers up to and including the
		/// resource and type counts, then stops. The stream ends where the type names would be.
		/// </summary>
		static byte[] BuildHeaderOnly(int numResources, int numTypes)
		{
			var ms = new MemoryStream();
			using (var w = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
			{
				WriteHeader(w, DefaultReaderType, numResources, numTypes);
			}
			return ms.ToArray();
		}

		static void WriteHeader(BinaryWriter w, string readerType, int numResources, int numTypes)
		{
			w.Write(ResourcesFile.MagicNumber);
			w.Write(1); // ResourceManager header version
			w.Write(0); // bytes to skip (unused for header version 1)
			w.Write(readerType);
			w.Write("System.Resources.RuntimeResourceSet");
			w.Write(2); // RuntimeResourceSet version
			w.Write(numResources);
			w.Write(numTypes);
		}

		/// <summary>
		/// Builds a complete version-2 .resources file. The data of each entry is the raw
		/// data section record (type code followed by payload).
		/// </summary>
		static byte[] Build(string readerType, string[] typeNames, IList<(string Name, byte[] Data)> entries, int? forcedNameByteLength = null)
		{
			var nameSection = new MemoryStream();
			var dataSection = new MemoryStream();
			var namePositions = new List<int>();
			using (var nameWriter = new BinaryWriter(nameSection, Encoding.UTF8, leaveOpen: true))
			{
				foreach (var (name, data) in entries)
				{
					namePositions.Add((int)nameSection.Position);
					byte[] nameBytes = Encoding.Unicode.GetBytes(name);
					nameWriter.Write7BitEncodedInt(forcedNameByteLength ?? nameBytes.Length);
					nameWriter.Write(nameBytes);
					nameWriter.Write((int)dataSection.Position);
					dataSection.Write(data);
				}
			}

			var ms = new MemoryStream();
			using (var w = new BinaryWriter(ms, Encoding.UTF8, leaveOpen: true))
			{
				WriteHeader(w, readerType, entries.Count, typeNames.Length);
				foreach (var typeName in typeNames)
					w.Write(typeName);
				while (ms.Position % 8 != 0)
					w.Write((byte)0);
				foreach (var (name, _) in entries)
					w.Write(name.GetHashCode()); // name hashes are not consulted by ResourcesFile
				foreach (int position in namePositions)
					w.Write(position);
				// name section starts right after this data section offset field
				int nameSectionStart = (int)ms.Position + sizeof(int);
				w.Write(nameSectionStart + (int)nameSection.Length);
				w.Write(nameSection.ToArray());
				w.Write(dataSection.ToArray());
			}
			return ms.ToArray();
		}
	}
}
