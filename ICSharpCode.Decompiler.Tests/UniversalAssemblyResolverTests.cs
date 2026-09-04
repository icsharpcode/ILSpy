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

using System.IO;

using ICSharpCode.Decompiler.Metadata;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	/// <summary>
	/// The GAC layout is the same on every platform ILSpy runs on, so these tests build one in a
	/// temporary directory instead of requiring a .NET Framework installation.
	/// </summary>
	[TestFixture]
	public class UniversalAssemblyResolverTests
	{
		string gacDirectory;

		[SetUp]
		public void SetUp()
		{
			gacDirectory = Path.Combine(Path.GetTempPath(), "ILSpyGacTest_" + Path.GetRandomFileName());
			Directory.CreateDirectory(gacDirectory);
		}

		[TearDown]
		public void TearDown()
		{
			if (Directory.Exists(gacDirectory))
				Directory.Delete(gacDirectory, recursive: true);
		}

		string AddAssembly(string name, string version, string publicKeyToken, string culture = "")
		{
			string directory = Path.Combine(gacDirectory, name, $"v4.0_{version}_{culture}_{publicKeyToken}");
			Directory.CreateDirectory(directory);
			string file = Path.Combine(directory, name + ".dll");
			File.WriteAllBytes(file, new byte[0]);
			return file;
		}

		static IAssemblyReference Reference(string name, string version, string publicKeyToken)
		{
			return AssemblyNameReference.Parse($"{name}, Version={version}, Culture=neutral, PublicKeyToken={publicKeyToken}");
		}

		[Test]
		public void HigherReferenceVersionResolvesToTheVersionInstalledInTheGac()
		{
			// The 4.8 reference assembly of System.IO.Compression is 4.2.0.0, but the GAC only
			// ever holds 4.0.0.0; the runtime unifies the reference onto the installed version.
			string file = AddAssembly("System.IO.Compression", "4.0.0.0", "b77a5c561934e089");

			var reference = Reference("System.IO.Compression", "4.2.0.0", "b77a5c561934e089");

			Assert.That(UniversalAssemblyResolver.FindUnifiedAssemblyInGacFolder(reference, "v4.0_", gacDirectory),
				Is.EqualTo(file));
		}

		[Test]
		public void UnificationPicksTheHighestInstalledVersion()
		{
			AddAssembly("Microsoft.Build.Framework", "15.1.0.0", "b03f5f7f11d50a3a");
			string expected = AddAssembly("Microsoft.Build.Framework", "15.2.0.0", "b03f5f7f11d50a3a");

			var reference = Reference("Microsoft.Build.Framework", "15.0.0.0", "b03f5f7f11d50a3a");

			Assert.That(UniversalAssemblyResolver.FindUnifiedAssemblyInGacFolder(reference, "v4.0_", gacDirectory),
				Is.EqualTo(expected));
		}

		[Test]
		public void UnificationDoesNotCrossMajorVersions()
		{
			// Microsoft.Build.Framework 4.0.0.0 and 15.x are different products sharing a name.
			AddAssembly("Microsoft.Build.Framework", "4.0.0.0", "b03f5f7f11d50a3a");

			var reference = Reference("Microsoft.Build.Framework", "15.0.0.0", "b03f5f7f11d50a3a");

			Assert.That(UniversalAssemblyResolver.FindUnifiedAssemblyInGacFolder(reference, "v4.0_", gacDirectory),
				Is.Null);
		}

		[Test]
		public void UnificationRequiresAMatchingPublicKeyToken()
		{
			AddAssembly("System.IO.Compression", "4.0.0.0", "31bf3856ad364e35");

			var reference = Reference("System.IO.Compression", "4.2.0.0", "b77a5c561934e089");

			Assert.That(UniversalAssemblyResolver.FindUnifiedAssemblyInGacFolder(reference, "v4.0_", gacDirectory),
				Is.Null);
		}

		[Test]
		public void SatelliteAssembliesAreNotUsedForUnification()
		{
			AddAssembly("System.IO.Compression", "4.0.0.0", "b77a5c561934e089", culture: "de");

			var reference = Reference("System.IO.Compression", "4.2.0.0", "b77a5c561934e089");

			Assert.That(UniversalAssemblyResolver.FindUnifiedAssemblyInGacFolder(reference, "v4.0_", gacDirectory),
				Is.Null);
		}
	}
}
