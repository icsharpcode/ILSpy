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
		public void ASecondScopeDoesNotStackWithTheFirst()
		{
			// Two decompilations can overlap on the resolver a LoadedAssembly owns. Whichever scope
			// ends first takes the cache with it and the other reads the file system again, which
			// costs that one its head start and nothing else.
			var resolver = Resolver();
			int scans = 0;

			var outer = resolver.BeginSnapshot();
			var inner = resolver.BeginSnapshot();
			inner.Dispose();
			resolver.GetOrAddVersionFolder("/shared/pack", _ => { scans++; return "9.0.0"; });
			outer.Dispose();

			Assert.That(scans, Is.EqualTo(1), "the scan happens, it is simply not held any more");
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
