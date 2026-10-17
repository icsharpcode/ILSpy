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
using ICSharpCode.Decompiler.Tests.Helpers;

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
				Tester.RepeatOnIOError(() => Directory.Delete(gacDirectory, recursive: true));
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
		public void RuntimeDirectoryIsTheDirectoryOfTheRunningCorlib()
		{
			// The resolver probes the shared framework of the runtime executing the decompiler. Finding
			// that directory through the corlib's file path breaks in single-file and Native AOT apps,
			// where the corlib has no file; the helper has to give the same answer without it.
			string expected = Path.GetDirectoryName(typeof(object).Assembly.Location);
			Assert.That(UniversalAssemblyResolver.GetRuntimeDirectory(), Is.EqualTo(expected));
		}

		[Test]
		public void SharedFrameworkDirectoryIsTheRuntimeDirectoryOnAManagedHost()
		{
			// A managed host runs from dotnet/shared/Microsoft.NETCore.App/<version>, which is where
			// the framework facades live; only single-file and Native AOT hosts, which run from their
			// application directory, need the fallback to an installed shared framework.
			Assert.That(UniversalAssemblyResolver.GetSharedFrameworkDirectory(),
				Is.EqualTo(UniversalAssemblyResolver.GetRuntimeDirectory()));
		}

		[Test]
		public void NewestInstalledSharedFrameworkIsPickedForHostsWithoutOne()
		{
			string dotnetRoot = Path.Combine(gacDirectory, "dotnet");
			foreach (string version in new[] { "9.0.1", "10.0.3", "10.0.0-preview.1", "not-a-version" })
				Directory.CreateDirectory(Path.Combine(dotnetRoot, "shared", "Microsoft.NETCore.App", version));

			Assert.That(UniversalAssemblyResolver.FindNewestSharedFramework(dotnetRoot),
				Is.EqualTo(Path.Combine(dotnetRoot, "shared", "Microsoft.NETCore.App", "10.0.3")));
		}

		[Test]
		public void NoInstalledSharedFrameworkGivesNull()
		{
			Assert.That(UniversalAssemblyResolver.FindNewestSharedFramework(gacDirectory), Is.Null);
		}

		[Test]
		public void CorlibVersionWithoutAFrameworkDirectoryDoesNotAbortTheLookup()
		{
			// The corlib lookup by .NET Framework directory is one step of a chain that goes on with
			// the GAC, the 4.0 framework directory and the shared framework of the running .NET;
			// a version it has no directory for must hand over to those steps instead of throwing,
			// which on hosts without a .NET Framework installation was the fate of every corlib
			// reference once ThrowOnAssemblyResolveErrors was set.
			var resolver = new UniversalAssemblyResolver(null, throwOnError: true, ".NETFramework,Version=v4.8");

			Assert.That(() => resolver.FindAssemblyFile(Reference("mscorlib", "3.0.0.0", "b77a5c561934e089")), Throws.Nothing);
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
