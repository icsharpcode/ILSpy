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

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using ICSharpCode.Decompiler;

using NUnit.Framework;

using static ICSharpCode.ILSpyCmd.Tests.CliTestRunner;

namespace ICSharpCode.ILSpyCmd.Tests
{
	/// <summary>
	/// A single-file bundle is a host executable with a payload appended to it: the embedded
	/// files, a manifest, and - as the last bytes of the file - the manifest offset followed by
	/// the bundle signature. Only that trailer and the manifest are read to detect a bundle and
	/// find its entries, so these tests build the payload around a real assembly and leave the
	/// host stub empty, rather than publishing a self-contained app at test time.
	/// </summary>
	[TestFixture]
	public class ILSpyCmdBundleEntryOptionTests
	{
		static readonly string testAssemblyPath = typeof(ILSpyCmdBundleEntryOptionTests).Assembly.Location;

		// The 32-byte bundle signature, as written by the .NET bundler.
		static readonly byte[] bundleSignature = {
			0x8b, 0x12, 0x02, 0xb9, 0x6a, 0x61, 0x20, 0x38,
			0x72, 0x7b, 0x93, 0x02, 0x14, 0xd7, 0xa0, 0x32,
			0x13, 0xf5, 0xb9, 0xe6, 0xef, 0xae, 0x33, 0x18,
			0xee, 0x3b, 0x2d, 0xce, 0x24, 0xb3, 0x6a, 0xae
		};

		string tempDirectory;
		string bundlePath;
		string bundleWithoutRuntimeConfigPath;

		[OneTimeSetUp]
		public void CreateBundles()
		{
			tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Directory.CreateDirectory(tempDirectory);
			bundlePath = Path.Combine(tempDirectory, "SampleApp.exe");
			bundleWithoutRuntimeConfigPath = Path.Combine(tempDirectory, "SampleAppV1.exe");
			WriteBundle(bundlePath, withRuntimeConfig: true);
			WriteBundle(bundleWithoutRuntimeConfigPath, withRuntimeConfig: false);
		}

		[OneTimeTearDown]
		public void DeleteBundles()
		{
			if (tempDirectory != null && Directory.Exists(tempDirectory))
				Directory.Delete(tempDirectory, recursive: true);
		}

		/// <summary>
		/// Writes a bundle containing two managed assemblies (both copies of this test assembly)
		/// and, optionally, the runtime-config entry that identifies "Sample.dll" as the app.
		/// </summary>
		static void WriteBundle(string path, bool withRuntimeConfig)
		{
			byte[] assemblyBytes = File.ReadAllBytes(testAssemblyPath);
			var entries = new List<(string Name, SingleFileBundle.FileType Type, long Offset, long Size)>();

			using var stream = File.Create(path);
			using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);

			// Stand-in for the host executable the payload is appended to. Bundle detection only
			// requires that the signature is not within the first eight bytes of the file.
			writer.Write(new byte[64]);

			void WriteEntry(string name, SingleFileBundle.FileType type, byte[] contents)
			{
				long offset = stream.Position;
				writer.Write(contents);
				entries.Add((name, type, offset, contents.Length));
			}

			WriteEntry("Sample.dll", SingleFileBundle.FileType.Assembly, assemblyBytes);
			WriteEntry("Helper.dll", SingleFileBundle.FileType.Assembly, assemblyBytes);
			if (withRuntimeConfig)
			{
				WriteEntry("Sample.runtimeconfig.json", SingleFileBundle.FileType.RuntimeConfigJson,
					Encoding.UTF8.GetBytes("{ \"runtimeOptions\": { } }"));
			}

			long headerOffset = stream.Position;
			writer.Write((uint)6);  // MajorVersion: entries carry a compressed size
			writer.Write((uint)0);  // MinorVersion
			writer.Write(entries.Count);
			writer.Write("bundle-id");
			writer.Write(0L);       // DepsJsonOffset
			writer.Write(0L);       // DepsJsonSize
			writer.Write(0L);       // RuntimeConfigJsonOffset
			writer.Write(0L);       // RuntimeConfigJsonSize
			writer.Write(0UL);      // Flags
			foreach (var entry in entries)
			{
				writer.Write(entry.Offset);
				writer.Write(entry.Size);
				writer.Write(0L);   // CompressedSize: entries are stored uncompressed
				writer.Write((byte)entry.Type);
				writer.Write(entry.Name);
			}

			writer.Write(headerOffset);
			writer.Write(bundleSignature);
		}

		[Test]
		public async Task BundleWithoutEntryListsManagedEntries()
		{
			var result = await RunAsync(bundlePath, "--disable-updatecheck");

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_USAGE));
			Assert.That(result.Error, Does.Contain("--bundle-entry"));
			Assert.That(result.Error, Does.Contain("Sample.dll"));
			Assert.That(result.Error, Does.Contain("Helper.dll"));
			// The manifest entry that is not an assembly is not something to decompile.
			Assert.That(result.Error, Does.Not.Contain("Sample.runtimeconfig.json"));
		}

		[Test]
		public async Task BundleWithoutEntryMarksEntryPoint()
		{
			var result = await RunAsync(bundlePath, "--disable-updatecheck");

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_USAGE));
			Assert.That(result.Error, Does.Contain("Sample.dll  (entry point)"));
			Assert.That(result.Error, Does.Not.Contain("Helper.dll  (entry point)"));
		}

		/// <summary>
		/// Without a runtime-config entry the app assembly cannot be derived. The listing is
		/// still the answer to the question asked, it just carries no annotation.
		/// </summary>
		[Test]
		public async Task BundleWithoutRuntimeConfigListsEntriesUnmarked()
		{
			var result = await RunAsync(bundleWithoutRuntimeConfigPath, "--disable-updatecheck");

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_USAGE));
			Assert.That(result.Error, Does.Contain("Sample.dll"));
			Assert.That(result.Error, Does.Contain("Helper.dll"));
			Assert.That(result.Error, Does.Not.Contain("(entry point)"));
		}

		[Test]
		public async Task NamedEntryIsDecompiled()
		{
			var result = await RunAsync(bundlePath, "--disable-updatecheck", "--bundle-entry", "Sample.dll",
				"-m", "M:ICSharpCode.ILSpyCmd.Tests.MemberOptionSample.Add(System.Int32,System.Int32)");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("int Add(int a, int b)"));
		}

		[Test]
		public async Task EntryNameIsMatchedCaseInsensitively()
		{
			var result = await RunAsync(bundlePath, "--disable-updatecheck", "--bundle-entry", "sample.DLL",
				"-m", "M:ICSharpCode.ILSpyCmd.Tests.MemberOptionSample.Add(System.Int32,System.Int32)");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("int Add(int a, int b)"));
		}

		/// <summary>
		/// The five call sites that loaded the input file separately are the reason every mode
		/// failed on a bundle; -il is one of the modes that never reaches the default path.
		/// </summary>
		[Test]
		public async Task NamedEntryWorksInILMode()
		{
			var result = await RunAsync(bundlePath, "--disable-updatecheck", "--bundle-entry", "Sample.dll", "-il");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain(".class"));
			Assert.That(result.Output, Does.Contain("MemberOptionSample"));
		}

		[Test]
		public async Task UnknownEntryNameListsValidEntries()
		{
			var result = await RunAsync(bundlePath, "--disable-updatecheck", "--bundle-entry", "NotThere.dll");

			Assert.That(result.ExitCode, Is.EqualTo(ProgramExitCodes.EX_DATAERR));
			Assert.That(result.Error, Does.Contain("NotThere.dll"));
			Assert.That(result.Error, Does.Contain("Sample.dll"));
			Assert.That(result.Error, Does.Contain("Helper.dll"));
		}

		[Test]
		public async Task DumpPackageStillWorksWithoutBundleEntry()
		{
			string outputDir = Path.Combine(tempDirectory, Path.GetRandomFileName());

			var result = await RunAsync(bundlePath, "--disable-updatecheck", "-d", "-o", outputDir);

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(File.Exists(Path.Combine(outputDir, "Sample.dll")), Is.True);
			Assert.That(File.Exists(Path.Combine(outputDir, "Helper.dll")), Is.True);
		}

		[Test]
		public async Task OrdinaryAssemblyIsUnaffected()
		{
			var result = await RunAsync(testAssemblyPath, "--disable-updatecheck",
				"-m", "M:ICSharpCode.ILSpyCmd.Tests.MemberOptionSample.Add(System.Int32,System.Int32)");

			Assert.That(result.ExitCode, Is.EqualTo(0), result.Error);
			Assert.That(result.Output, Does.Contain("int Add(int a, int b)"));
			Assert.That(result.Error, Does.Not.Contain("--bundle-entry"));
		}
	}
}
