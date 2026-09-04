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
using System.Threading.Tasks;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Tests.Helpers;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.TypeSystem
{
	/// <summary>
	/// Obfuscated and merged assemblies can end up referencing two versions of the same
	/// assembly. Loading both splits every type they declare into two type definitions that
	/// compare unequal, which in turn breaks every signature comparison spanning the two
	/// references: a method whose parameter is named through one of them no longer matches the
	/// base method that names it through the other. The fixture reproduces that through the
	/// symptom it surfaced as in issue #2764 - a genuine override printed as virtual, because
	/// <see cref="InheritanceHelper"/> found no base member for it.
	/// </summary>
	[TestFixture]
	public class DuplicateAssemblyReferenceTests
	{
		/// <summary>
		/// Assembled twice, as version 1.0.0.0 and 2.0.0.0.
		/// </summary>
		const string LibraryIL = @"
.assembly extern System.Runtime
{
  .ver 8:0:0:0
}
.assembly Lib
{
  .ver {VERSION}:0:0:0
}
.class public auto ansi beforefieldinit Param
       extends [System.Runtime]System.Object
{
}
";

		/// <summary>
		/// Declares both versions of Lib as references, the way a merging obfuscator leaves
		/// them behind, and names Param through a different one in the base declaration than
		/// in the override. Both are the same type to the runtime, which unifies the two
		/// references, so Run really does override Base.Run.
		/// </summary>
		const string MainIL = @"
.assembly extern Lib
{
  .ver 2:0:0:0
}
.assembly extern Lib as Lib_v1
{
  .ver 1:0:0:0
}
.assembly extern System.Runtime
{
  .ver 8:0:0:0
}
.assembly Main { }

.class public abstract auto ansi beforefieldinit Base
       extends [System.Runtime]System.Object
{
  .method family hidebysig specialname rtspecialname instance void .ctor() cil managed
  {
    ldarg.0
    call instance void [System.Runtime]System.Object::.ctor()
    ret
  }
  .method public hidebysig newslot abstract virtual instance void Run(class [Lib_v1]Param p) cil managed
  {
  }
}

.class public auto ansi beforefieldinit Derived
       extends Base
{
  .method public hidebysig specialname rtspecialname instance void .ctor() cil managed
  {
    ldarg.0
    call instance void Base::.ctor()
    ret
  }
  .method public hidebysig virtual instance void Run(class [Lib]Param p) cil managed
  {
    ret
  }
}
";

		string directory;
		string mainAssemblyPath;

		[OneTimeSetUp]
		public async Task SetUp()
		{
			directory = Path.Combine(Path.GetTempPath(), "ILSpy-DuplicateAssemblyReference-" + Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(directory);
			foreach (var version in new[] { 1, 2 })
			{
				await AssembleAsync($"Lib.v{version}", LibraryIL.Replace("{VERSION}", version.ToString())).ConfigureAwait(false);
			}
			mainAssemblyPath = await AssembleAsync("Main", MainIL).ConfigureAwait(false);
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			Directory.Delete(directory, recursive: true);
		}

		Task<string> AssembleAsync(string name, string il)
		{
			string sourceFile = Path.Combine(directory, name + ".il");
			File.WriteAllText(sourceFile, il);
			return Tester.AssembleIL(sourceFile, AssemblerOptions.Library);
		}

		DecompilerTypeSystem CreateTypeSystem()
		{
			var mainModule = new PEFile(mainAssemblyPath);
			return new DecompilerTypeSystem(mainModule, new VersionedResolver(directory));
		}

		[Test]
		public void OnlyTheHighestVersionOfADuplicatedReferenceIsLoaded()
		{
			var libraries = CreateTypeSystem().Modules
				.Where(m => m.AssemblyName == "Lib")
				.Select(m => m.FullAssemblyName)
				.ToArray();

			Assert.That(libraries, Is.EqualTo(new[] { "Lib, Version=2.0.0.0, Culture=neutral, PublicKeyToken=null" }));
		}

		[Test]
		public void OverrideDeclaredThroughTheOtherReferenceStaysAnOverride()
		{
			var decompiler = new CSharpDecompiler(CreateTypeSystem(), new DecompilerSettings());

			string code = decompiler.DecompileTypeAsString(new FullTypeName("Derived"));

			Assert.That(code, Does.Contain("public override void Run(Param p)"));
		}

		/// <summary>
		/// Hands out a separate file per requested version of Lib, so that the type system has
		/// to deal with two candidates for the same assembly name; everything else comes from
		/// the runtime the tests execute on.
		/// </summary>
		class VersionedResolver : IAssemblyResolver
		{
			/// <inheritdoc/>
			public IDisposable BeginSnapshot() => null;

			static readonly string runtimeDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);

			readonly string directory;

			public VersionedResolver(string directory)
			{
				this.directory = directory;
			}

			public MetadataFile Resolve(IAssemblyReference reference)
			{
				string path = reference.Name == "Lib"
					? Path.Combine(directory, $"Lib.v{reference.Version.Major}.dll")
					: Path.Combine(runtimeDirectory, reference.Name + ".dll");
				return File.Exists(path) ? new PEFile(path) : null;
			}

			public MetadataFile ResolveModule(MetadataFile mainModule, string moduleName) => null;

			public Task<MetadataFile> ResolveAsync(IAssemblyReference reference) => Task.FromResult(Resolve(reference));

			public Task<MetadataFile> ResolveModuleAsync(MetadataFile mainModule, string moduleName) => Task.FromResult<MetadataFile>(null);
		}
	}
}
