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
using System.Text;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.Metadata;

using K4os.Compression.LZ4;

using NUnit.Framework;

using static ICSharpCode.ILSpyCmd.Tests.CliTestRunner;

namespace ICSharpCode.ILSpyCmd.Tests
{
	/// <summary>
	/// A Xamarin.Android app ships every assembly LZ4-compressed behind a 12-byte "XALZ" header.
	/// These tests lay out a directory the way such an app does - the input assembly and the
	/// assembly it references, both compressed - because decompiling one file of an app only
	/// gives good output when its references next to it load too.
	/// </summary>
	[TestFixture]
	public class ILSpyCmdXamarinCompressedInputTests
	{
		// Magic used for the Xamarin compressed module header ('XALZ', little-endian).
		const uint CompressedDataMagic = 0x5A4C4158;

		string tempDirectory;
		string compressedInputPath;

		[OneTimeSetUp]
		public void CreateCompressedAssemblies()
		{
			tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Directory.CreateDirectory(tempDirectory);

			string inputAssembly = typeof(ILSpyCmdXamarinCompressedInputTests).Assembly.Location;
			string referencedAssembly = typeof(TargetFrameworkIdentifier).Assembly.Location;
			compressedInputPath = WriteCompressed(inputAssembly);
			WriteCompressed(referencedAssembly);
		}

		[OneTimeTearDown]
		public void DeleteCompressedAssemblies()
		{
			if (tempDirectory != null && Directory.Exists(tempDirectory))
				Directory.Delete(tempDirectory, recursive: true);
		}

		/// <summary>
		/// Writes the XALZ form of <paramref name="assemblyPath"/> into the temp directory under
		/// the same file name, so references resolve by name next to the input, and returns it.
		/// </summary>
		string WriteCompressed(string assemblyPath)
		{
			byte[] original = File.ReadAllBytes(assemblyPath);
			byte[] compressed = new byte[LZ4Codec.MaximumOutputSize(original.Length)];
			int compressedLength = LZ4Codec.Encode(original, 0, original.Length, compressed, 0, compressed.Length);

			string path = Path.Combine(tempDirectory, Path.GetFileName(assemblyPath));
			using var stream = File.Create(path);
			using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
			writer.Write(CompressedDataMagic);
			writer.Write((uint)0); // descriptor table index, unused by the loader
			writer.Write((uint)original.Length);
			writer.Write(compressed, 0, compressedLength);
			return path;
		}

		[Test]
		public async Task CompressedInputIsDecompiled()
		{
			var result = await RunAsync(compressedInputPath, "--disable-updatecheck",
				"-m", "M:ICSharpCode.ILSpyCmd.Tests.MemberOptionSample.Add(System.Int32,System.Int32)");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("int Add(int a, int b)"));
		}

		/// <summary>
		/// An enum from an unresolved reference is an unknown type, so a comparison against one of
		/// its members prints as a cast of the raw value. The member name appearing proves the
		/// compressed reference was loaded.
		/// </summary>
		[Test]
		public async Task CompressedReferenceIsResolved()
		{
			var result = await RunAsync(compressedInputPath, "--disable-updatecheck",
				"-m", "M:ICSharpCode.ILSpyCmd.Tests.XamarinReferenceSample.IsCore(ICSharpCode.Decompiler.Metadata.TargetFrameworkIdentifier)");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("id == TargetFrameworkIdentifier.NETCoreApp"));
		}
	}

	public static class XamarinReferenceSample
	{
		public static bool IsCore(TargetFrameworkIdentifier id)
		{
			return id == TargetFrameworkIdentifier.NETCoreApp;
		}
	}
}
