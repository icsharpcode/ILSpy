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
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.Metadata;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	[TestFixture]
	public class ResolutionSnapshotTests
	{
		static UniversalAssemblyResolver Resolver()
		{
			return new UniversalAssemblyResolver(null, throwOnError: false, targetFramework: null);
		}

		[Test]
		public void DirectoryIsScannedOnceWhileASnapshotIsOpen()
		{
			var resolver = Resolver();
			int scans = 0;

			using (resolver.BeginSnapshot())
			{
				for (int i = 0; i < 3; i++)
				{
					Assert.That(resolver.GetOrAddVersionFolder("/shared/pack", _ => { scans++; return "9.0.0"; }),
						Is.EqualTo("9.0.0"));
				}
			}

			Assert.That(scans, Is.EqualTo(1));
		}

		[Test]
		public void DirectoryIsScannedEveryTimeWithoutASnapshot()
		{
			var resolver = Resolver();
			int scans = 0;

			for (int i = 0; i < 3; i++)
			{
				resolver.GetOrAddVersionFolder("/shared/pack", _ => { scans++; return "9.0.0"; });
			}

			Assert.That(scans, Is.EqualTo(3));
		}

		[Test]
		public void WhatWasScannedIsForgottenWhenTheSnapshotEnds()
		{
			// The point of the scope: outside it the file system is read afresh, so an assembly
			// list reloaded after a runtime was installed or removed sees the new state.
			var resolver = Resolver();
			int scans = 0;

			using (resolver.BeginSnapshot())
			{
				resolver.GetOrAddVersionFolder("/shared/pack", _ => { scans++; return "9.0.0"; });
			}
			using (resolver.BeginSnapshot())
			{
				resolver.GetOrAddVersionFolder("/shared/pack", _ => { scans++; return "10.0.0"; });
			}

			Assert.That(scans, Is.EqualTo(2));
		}

		[Test]
		public void OverlappingScopesShareTheCacheUntilTheLastScopeEnds()
		{
			string decompilerFile = typeof(UniversalAssemblyResolver).Assembly.Location;
			var resolver = new UniversalAssemblyResolver(decompilerFile, throwOnError: false, targetFramework: null,
				streamOptions: PEStreamOptions.PrefetchMetadata);
			var firstReference = AssemblyNameReference.Parse("ICSharpCode.Decompiler, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");
			var secondReference = AssemblyNameReference.Parse("ICSharpCode.Decompiler, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null");

			var firstScope = resolver.BeginSnapshot();
			var secondScope = resolver.BeginSnapshot();
			int scans = 0;
			string firstVersion = resolver.GetOrAddVersionFolder("/shared/pack", _ => { scans++; return "9.0.0"; });
			var first = resolver.Resolve(firstReference);
			secondScope?.Dispose();
			string secondVersion = resolver.GetOrAddVersionFolder("/shared/pack", _ => { scans++; return "10.0.0"; });
			var second = resolver.Resolve(secondReference);
			firstScope?.Dispose();

			Assert.That(firstVersion, Is.EqualTo("9.0.0"));
			Assert.That(secondVersion, Is.EqualTo(firstVersion));
			Assert.That(scans, Is.EqualTo(1));
			using (first)
			using (second)
			{
				Assert.That(first, Is.Not.Null);
				Assert.That(second, Is.SameAs(first));
			}
		}

		static readonly string decompilerFile = typeof(UniversalAssemblyResolver).Assembly.Location;

		static AssemblyNameReference DecompilerReference(string version)
		{
			return AssemblyNameReference.Parse($"ICSharpCode.Decompiler, Version={version}, Culture=neutral, PublicKeyToken=null");
		}

		[Test]
		public void FileIsLoadedOnceWhileASnapshotIsOpen()
		{
			var resolver = new UniversalAssemblyResolver(decompilerFile, throwOnError: false, targetFramework: null,
				streamOptions: PEStreamOptions.PrefetchMetadata);

			using (resolver.BeginSnapshot())
			{
				using var file = resolver.Resolve(DecompilerReference("1.0.0.0"));

				Assert.That(file, Is.Not.Null);
				Assert.That(resolver.Resolve(DecompilerReference("2.0.0.0")), Is.SameAs(file));
				Assert.That(resolver.ResolveModule(file, Path.GetFileName(decompilerFile)), Is.SameAs(file));
			}
		}

		[Test]
		public async Task ConcurrentReferencesToOneFileShareOneInstance()
		{
			var resolver = new UniversalAssemblyResolver(decompilerFile, throwOnError: false, targetFramework: null,
				streamOptions: PEStreamOptions.PrefetchMetadata);

			using (resolver.BeginSnapshot())
			{
				var resolutions = Enumerable.Range(1, 16)
					.Select(version => resolver.ResolveAsync(DecompilerReference($"1.0.0.{version}")));
				var files = await Task.WhenAll(resolutions);

				Assert.That(files[0], Is.Not.Null);
				Assert.That(files.All(file => ReferenceEquals(file, files[0])), Is.True);
				files[0]?.Dispose();
			}
		}

		[Test]
		public void FileIsLoadedEveryTimeWithoutASnapshot()
		{
			var resolver = new UniversalAssemblyResolver(decompilerFile, throwOnError: false, targetFramework: null,
				streamOptions: PEStreamOptions.PrefetchMetadata);

			using var first = resolver.Resolve(DecompilerReference("1.0.0.0"));
			using var second = resolver.Resolve(DecompilerReference("1.0.0.0"));

			Assert.That(first, Is.Not.Null);
			Assert.That(second, Is.Not.Null.And.Not.SameAs(first));
		}

		[Test]
		public void WhatWasLoadedIsForgottenWhenTheSnapshotEnds()
		{
			var resolver = new UniversalAssemblyResolver(decompilerFile, throwOnError: false, targetFramework: null,
				streamOptions: PEStreamOptions.PrefetchMetadata);
			MetadataFile first, second;

			using (resolver.BeginSnapshot())
			{
				first = resolver.Resolve(DecompilerReference("1.0.0.0"))!;
			}
			using (resolver.BeginSnapshot())
			{
				second = resolver.Resolve(DecompilerReference("1.0.0.0"))!;
			}

			using (first)
			using (second)
			{
				Assert.That(first, Is.Not.Null);
				Assert.That(second, Is.Not.Null.And.Not.SameAs(first));
			}
		}

		[Test]
		public void FailureToLoadIsReportedForEveryReferenceWhileASnapshotIsOpen()
		{
			string directory = Path.Combine(Path.GetTempPath(), "ILSpySnapshotTest_" + Path.GetRandomFileName());
			Directory.CreateDirectory(directory);
			try
			{
				File.WriteAllBytes(Path.Combine(directory, "SomeLibrary.dll"), Array.Empty<byte>());

				var resolver = new UniversalAssemblyResolver(null, throwOnError: true, targetFramework: null,
					streamOptions: PEStreamOptions.PrefetchMetadata);
				resolver.AddSearchDirectory(directory);

				using (resolver.BeginSnapshot())
				{
					foreach (string version in new[] { "1.0.0.0", "2.0.0.0" })
					{
						var reference = AssemblyNameReference.Parse($"SomeLibrary, Version={version}, Culture=neutral, PublicKeyToken=null");
						var exception = Assert.Throws<ResolutionException>(() => resolver.Resolve(reference));
						Assert.That(exception.Reference, Is.SameAs(reference));
					}
				}
			}
			finally
			{
				Directory.Delete(directory, recursive: true);
			}
		}

		[Test]
		public void ResolverFindsTheSameFileInsideAndOutsideASnapshot()
		{
			string directory = Path.Combine(Path.GetTempPath(), "ILSpySnapshotTest_" + Path.GetRandomFileName());
			Directory.CreateDirectory(directory);
			try
			{
				string file = Path.Combine(directory, "SomeLibrary.dll");
				File.WriteAllBytes(file, new byte[0]);

				var resolver = new UniversalAssemblyResolver(null, throwOnError: false, targetFramework: null);
				resolver.AddSearchDirectory(directory);
				var reference = AssemblyNameReference.Parse("SomeLibrary, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null");

				string outside = resolver.FindAssemblyFile(reference);
				string inside;
				using (resolver.BeginSnapshot())
				{
					inside = resolver.FindAssemblyFile(reference);
				}

				Assert.That(outside, Is.EqualTo(file));
				Assert.That(inside, Is.EqualTo(file));
			}
			finally
			{
				Directory.Delete(directory, recursive: true);
			}
		}
	}
}
