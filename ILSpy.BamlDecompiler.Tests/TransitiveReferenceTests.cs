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
using System.Linq;
using System.Reflection.PortableExecutable;

using ICSharpCode.BamlDecompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

namespace ILSpy.BamlDecompiler.Tests
{
	/// <summary>
	/// Deciding whether an element is a markup extension means walking its base types, and those
	/// leave the assemblies a document's own assembly names. A base type landing in an assembly
	/// nobody loaded resolves to nothing, the extension is not recognised, and it goes out as an
	/// element tree carrying the decompiler's own placeholder namespace,
	/// "https://github.com/icsharpcode/ILSpy", into the XAML (issue #2930).
	/// </summary>
	[TestFixture]
	public class TransitiveReferenceTests
	{
		/// <summary>
		/// ICSharpCode.BamlDecompiler stands in for a document's assembly here: it has references of
		/// its own, and those references have references it does not name itself.
		/// </summary>
		static readonly string MainModulePath = Path.Combine(
			Path.GetDirectoryName(typeof(TransitiveReferenceTests).Assembly.Location),
			"ICSharpCode.BamlDecompiler.dll");

		static PEFile Load(string path)
		{
			using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
			return new PEFile(path, stream, streamOptions: PEStreamOptions.PrefetchEntireImage);
		}

		static ICompilation TypeSystemOf(PEFile file)
		{
			var resolver = new UniversalAssemblyResolver(file.FileName, throwOnError: false,
				file.DetectTargetFrameworkId(), file.DetectRuntimePack());
			return new BamlDecompilerTypeSystem(file, resolver);
		}

		/// <summary>
		/// An assembly the main module reaches only through one of its own references. Derived from
		/// the metadata rather than named here, so that changing what either assembly references
		/// cannot quietly turn this test into one that proves nothing.
		/// </summary>
		static string IndirectlyReferencedAssembly(PEFile mainModule)
		{
			var direct = new HashSet<string>(mainModule.AssemblyReferences.Select(r => r.Name));
			foreach (var reference in mainModule.AssemblyReferences)
			{
				if (reference.Name != "ICSharpCode.Decompiler")
					continue;
				using var referenced = Load(Path.Combine(
					Path.GetDirectoryName(mainModule.FileName), reference.Name + ".dll"));
				string indirect = referenced.AssemblyReferences
					.Select(r => r.Name)
					.FirstOrDefault(name => !direct.Contains(name));
				Assert.That(indirect, Is.Not.Null,
					"the premise of this test is gone: every assembly ICSharpCode.Decompiler "
					+ "references is now referenced by ICSharpCode.BamlDecompiler as well");
				return indirect;
			}
			Assert.Fail("ICSharpCode.BamlDecompiler no longer references ICSharpCode.Decompiler");
			return null;
		}

		[Test]
		public void AnAssemblyReachedOnlyThroughAReferenceIsLoaded()
		{
			using var mainModule = Load(MainModulePath);
			string indirect = IndirectlyReferencedAssembly(mainModule);

			var compilation = TypeSystemOf(mainModule);

			Assert.That(compilation.Modules.Select(m => m.AssemblyName), Does.Contain(indirect));
		}

		[Test]
		public void TheAssembliesTheModuleNamesItselfAreStillLoaded()
		{
			using var mainModule = Load(MainModulePath);

			var compilation = TypeSystemOf(mainModule);

			Assert.That(compilation.Modules.Select(m => m.AssemblyName),
				Does.Contain("ICSharpCode.Decompiler"));
		}
	}
}
