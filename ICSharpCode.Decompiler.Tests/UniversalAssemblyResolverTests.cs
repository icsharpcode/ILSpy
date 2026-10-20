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
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
		public void SharedFrameworkOfTheHostsOwnMajorVersionIsPickedForHostsWithoutOne()
		{
			// A framework-dependent net10 app runs on the installed 10.x runtime even when an 11.x
			// is installed too; a single-file or Native AOT build of it has to take its facades
			// from that same 10.x so that both resolve the same reference assemblies.
			string dotnetRoot = Path.Combine(gacDirectory, "dotnet");
			string shared = Path.Combine(dotnetRoot, "shared", "Microsoft.NETCore.App");
			foreach (string version in new[] { "9.0.1", "10.0.3", "10.0.0-preview.1", "11.0.0-rc.2.26504.105", "not-a-version" })
				Directory.CreateDirectory(Path.Combine(shared, version));

			using (Assert.EnterMultipleScope())
			{
				Assert.That(UniversalAssemblyResolver.FindSharedFramework(dotnetRoot, preferredMajor: 10), Is.EqualTo(Path.Combine(shared, "10.0.3")), "newest of the host's major");
				Assert.That(UniversalAssemblyResolver.FindSharedFramework(dotnetRoot, preferredMajor: 12), Is.EqualTo(Path.Combine(shared, "11.0.0-rc.2.26504.105")), "newest overall when the host's major is not installed");
				Assert.That(UniversalAssemblyResolver.FindSharedFramework(dotnetRoot, preferredMajor: null), Is.EqualTo(Path.Combine(shared, "11.0.0-rc.2.26504.105")), "newest overall without a preference");
			}
		}

		[Test]
		public void NoInstalledSharedFrameworkGivesNull()
		{
			Assert.That(UniversalAssemblyResolver.FindSharedFramework(gacDirectory, preferredMajor: 10), Is.Null);
		}

		[Test]
		public void RuntimeDirectoryWithFacadesIsUsedWhereverItIs()
		{
			// A self-contained app carries the framework facades in its own directory, which is not
			// named like a shared framework; a single-file or Native AOT app's directory holds no
			// assemblies at all. The facades decide, not the directory name.
			string selfContained = Path.Combine(gacDirectory, "app");
			Directory.CreateDirectory(selfContained);
			Assert.That(UniversalAssemblyResolver.HasFrameworkFacades(selfContained), Is.False, "application directory without assemblies");
			File.WriteAllBytes(Path.Combine(selfContained, "mscorlib.dll"), new byte[0]);
			Assert.That(UniversalAssemblyResolver.HasFrameworkFacades(selfContained), Is.True, "application directory with the facades");
		}

		[Test]
		public void DotNetDirectoryCandidatesFollowTheHostLookup()
		{
			// The .NET host finds the runtime of a framework-dependent app through DOTNET_ROOT and
			// the default install locations, never through PATH. The lookup for single-file and
			// Native AOT hosts checks the same places in the same order, so that it lands on the
			// runtime such an app would run on, and falls back to PATH only after those.
			var env = new Dictionary<string, string> {
				["PATH"] = string.Join(Path.PathSeparator.ToString(), "first", "second"),
				["DOTNET_ROOT"] = "root",
			};
			var candidates = DotNetCorePathFinder.CandidateDotNetDirectories(name => env.TryGetValue(name, out var value) ? value : null).ToList();
			using (Assert.EnterMultipleScope())
			{
				Assert.That(candidates.First(), Is.EqualTo("root"));
				Assert.That(candidates.Skip(candidates.Count - 2), Is.EqualTo(new[] { "first", "second" }), "PATH last");
				Assert.That(candidates.Count, Is.GreaterThan(3), "default install locations in between");
			}

			env.Remove("PATH");
			Assert.That(DotNetCorePathFinder.CandidateDotNetDirectories(name => env.TryGetValue(name, out var value) ? value : null).First(), Is.EqualTo("root"), "no PATH at all");
		}

		[Test]
		public void DotNetExecutableIsTakenFromTheFirstCandidateThatHasIt()
		{
			string empty = Path.Combine(gacDirectory, "empty");
			string root = Path.Combine(gacDirectory, "root");
			Directory.CreateDirectory(empty);
			Directory.CreateDirectory(root);
			string exeName = Environment.OSVersion.Platform == PlatformID.Unix ? "dotnet" : "dotnet.exe";
			File.WriteAllBytes(Path.Combine(root, exeName), new byte[0]);

			Assert.That(DotNetCorePathFinder.FindDotNetExeDirectory(new[] { empty, Path.Combine(gacDirectory, "missing"), root }), Is.EqualTo(root));
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
