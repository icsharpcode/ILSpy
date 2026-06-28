// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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

using System.IO;
using System.Text;

using ICSharpCode.BamlDecompiler;
using ICSharpCode.Decompiler.Metadata;

using NUnit.Framework;

namespace ILSpy.BamlDecompiler.Tests
{
	/// <summary>
	/// Hardening tests for the BAML reader: a crafted or corrupt resource must surface a
	/// single catchable exception rather than crashing the process (uncontrolled recursion
	/// terminating with an uncatchable StackOverflowException) or allocating gigabytes.
	/// These drive the public <see cref="XamlDecompiler.Decompile(Stream)"/> entry point,
	/// the same one resource browsing uses.
	/// </summary>
	[TestFixture]
	public class BamlSecurityTests
	{
		// BAML record type ids (see BamlRecordType in the decompiler).
		const byte DefAttributeKeyString = 0x26;
		const byte StaticResourceStart = 0x30;

		static readonly string AssemblyPath = typeof(BamlSecurityTests).Assembly.Location;

		static XamlDecompiler CreateDecompiler(PEFile module)
		{
			var resolver = new UniversalAssemblyResolver(AssemblyPath, false, module.Metadata.DetectTargetFrameworkId());
			var typeSystem = new BamlDecompilerTypeSystem(module, resolver);
			return new XamlDecompiler(typeSystem, new BamlDecompilerSettings());
		}

		/// <summary>
		/// Writes the smallest header the reader accepts: the "MSBAML" signature block and
		/// the three (reader/updater/writer) 0.0x60 version structs.
		/// </summary>
		static void WriteValidHeader(BinaryWriter w)
		{
			byte[] sig = Encoding.Unicode.GetBytes("MSBAML");
			w.Write((uint)sig.Length);   // byte count; the reader takes len >> 1 characters
			w.Write(sig);                // 12 bytes, already 4-byte aligned so no padding follows
			for (int i = 0; i < 3; i++)
			{
				w.Write((ushort)0x0000); // Major
				w.Write((ushort)0x0060); // Minor
			}
		}

		static void WriteDefAttributeKeyString(BinaryWriter w)
		{
			w.Write(DefAttributeKeyString);
			w.Write((byte)9);     // SizedBamlRecord size prefix (1 byte size + 8 bytes data)
			w.Write((ushort)0);   // ValueId
			w.Write((uint)0);     // pos
			w.Write(false);       // Shared
			w.Write(false);       // SharedSet
		}

		static void WriteStaticResourceStart(BinaryWriter w)
		{
			w.Write(StaticResourceStart);
			w.Write((ushort)0);   // TypeId
			w.Write((byte)0);     // Flags
		}

		[Test]
		public void DeeplyNestedDeferBlock_DoesNotStackOverflow()
		{
			// A DefAttributeKeyString defer block followed by tens of thousands of nested
			// StaticResourceStart records makes the unfixed NavigateTree recurse without a
			// depth cap, terminating the process with an uncatchable StackOverflowException.
			var ms = new MemoryStream();
			using (var w = new BinaryWriter(ms, Encoding.Unicode, leaveOpen: true))
			{
				WriteValidHeader(w);
				WriteDefAttributeKeyString(w);
				for (int i = 0; i < 20000; i++)
					WriteStaticResourceStart(w);
			}
			ms.Position = 0;

			using var module = new PEFile(AssemblyPath);
			Assert.Throws<InvalidDataException>(() => CreateDecompiler(module).Decompile(ms));
		}

		[Test]
		public void UnterminatedDeferBlock_ThrowsInvalidData()
		{
			// A single StaticResourceStart with no matching StaticResourceEnd walks the
			// record index off the end of the list (ArgumentOutOfRangeException before the
			// fix); the reader must report it as malformed data instead.
			var ms = new MemoryStream();
			using (var w = new BinaryWriter(ms, Encoding.Unicode, leaveOpen: true))
			{
				WriteValidHeader(w);
				WriteDefAttributeKeyString(w);
				WriteStaticResourceStart(w);
			}
			ms.Position = 0;

			using var module = new PEFile(AssemblyPath);
			Assert.Throws<InvalidDataException>(() => CreateDecompiler(module).Decompile(ms));
		}

		[Test]
		public void OversizedSignatureLength_ThrowsInvalidData()
		{
			// The signature length is attacker-controlled and read before the MSBAML check;
			// an enormous value must be rejected before it drives a multi-gigabyte allocation.
			var ms = new MemoryStream();
			using (var w = new BinaryWriter(ms, Encoding.Unicode, leaveOpen: true))
			{
				w.Write((uint)0x7FFFFFFF);
			}
			ms.Position = 0;

			using var module = new PEFile(AssemblyPath);
			Assert.Throws<InvalidDataException>(() => CreateDecompiler(module).Decompile(ms));
		}
	}
}
