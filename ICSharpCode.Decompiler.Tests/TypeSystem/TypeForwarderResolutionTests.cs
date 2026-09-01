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

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Tests.Helpers;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.TypeSystem
{
	/// <summary>
	/// A facade defines no types; it forwards them onwards. Resolving its references relative to the
	/// assembly being decompiled - which is what every probe does by default - can take a chain of
	/// forwarders out of the framework it started in and into an unrelated set of facades, where it
	/// forwards in circles until the cycle guard gives up and the type is lost. That is issue #2054,
	/// where a .NET Standard 2.0 assembly sitting among .NET Framework 4.6.1 facades lost
	/// System.Linq.Enumerable and every LINQ call decompiled as a cast-laden static call.
	///
	/// The layout below is that graph in miniature: the input directory holds a facade chain that
	/// closes on itself, and only the second directory holds an assembly that defines the type.
	/// </summary>
	[TestFixture]
	public class TypeForwarderResolutionTests
	{
		/// <summary>Forwards to Mid, which only the framework directory has.</summary>
		const string ShimIL = @"
.assembly extern Mid { .ver 1:0:0:0 }
.assembly Shim { .ver 1:0:0:0 }
.class extern forwarder Ns.T
{
  .assembly extern Mid
}
";

		/// <summary>The framework's facade: forwards to Impl, which exists in both directories.</summary>
		const string MidIL = @"
.assembly extern Impl { .ver 1:0:0:0 }
.assembly Mid { .ver 1:0:0:0 }
.class extern forwarder Ns.T
{
  .assembly extern Impl
}
";

		/// <summary>
		/// The copy next to the input assembly: a facade that forwards back to Shim and closes the
		/// cycle. Deliberately the higher version, so that picking the right assembly cannot be an
		/// accident of the highest-version-wins deduplication of referenced assemblies.
		/// </summary>
		const string ImplCycleIL = @"
.assembly extern Shim { .ver 1:0:0:0 }
.assembly Impl { .ver 2:0:0:0 }
.class extern forwarder Ns.T
{
  .assembly extern Shim
}
";

		/// <summary>The copy next to Mid: the only assembly in the graph that defines the type.</summary>
		const string ImplRealIL = @"
.assembly extern System.Runtime { .ver 8:0:0:0 }
.assembly Impl { .ver 1:0:0:0 }
.class public auto ansi beforefieldinit Ns.T
       extends [System.Runtime]System.Object
{
}
";

		/// <summary>
		/// Exists in both directories and defines its type in both, so it is nobody's facade. Main
		/// references it directly; an ordinary reference must keep resolving next to the assembly
		/// being decompiled.
		/// </summary>
		const string DupIL = @"
.assembly extern System.Runtime { .ver 8:0:0:0 }
.assembly Dup { .ver {VERSION}:0:0:0 }
.class public auto ansi beforefieldinit Ns.D
       extends [System.Runtime]System.Object
{
}
";

		/// <summary>
		/// A second cycle, for a type nothing in the graph declares: Shim2 -> Mid2 -> Impl2 -> Shim2.
		/// </summary>
		const string Shim2IL = @"
.assembly extern Mid2 { .ver 1:0:0:0 }
.assembly Shim2 { .ver 1:0:0:0 }
.class extern forwarder Ns.U
{
  .assembly extern Mid2
}
";

		const string Mid2IL = @"
.assembly extern Impl2 { .ver 1:0:0:0 }
.assembly Mid2 { .ver 1:0:0:0 }
.class extern forwarder Ns.U
{
  .assembly extern Impl2
}
";

		/// <summary>The copy next to the input assembly, closing the cycle.</summary>
		const string Impl2CycleIL = @"
.assembly extern Shim2 { .ver 1:0:0:0 }
.assembly Impl2 { .ver 2:0:0:0 }
.class extern forwarder Ns.U
{
  .assembly extern Shim2
}
";

		/// <summary>
		/// The copy next to Mid2. It ends the chain - it forwards nothing - but it does not declare
		/// Ns.U either, so the repair must leave it alone rather than load it over the Impl2 that the
		/// input directory holds.
		/// </summary>
		const string Impl2DecoyIL = @"
.assembly extern System.Runtime { .ver 8:0:0:0 }
.assembly Impl2 { .ver 1:0:0:0 }
.class public auto ansi beforefieldinit Ns.SomethingElse
       extends [System.Runtime]System.Object
{
}
";

		const string MainIL = @"
.assembly extern System.Runtime { .ver 8:0:0:0 }
.assembly extern Shim { .ver 1:0:0:0 }
.assembly extern Dup { .ver 1:0:0:0 }
.assembly extern Shim2 { .ver 1:0:0:0 }
.assembly Main { }

.class public auto ansi beforefieldinit Consumer
       extends [System.Runtime]System.Object
{
  .method public hidebysig instance void UseForwarded(class [Shim]Ns.T t) cil managed
  {
    ret
  }
  .method public hidebysig instance void UseDuplicated(class [Dup]Ns.D d) cil managed
  {
    ret
  }

  .method public hidebysig instance void UseUndeclared(class [Shim2]Ns.U u) cil managed
  {
    ret
  }
}
";

		string inputDirectory;
		string frameworkDirectory;
		string mainAssemblyPath;

		[OneTimeSetUp]
		public async Task SetUp()
		{
			string root = Path.Combine(Path.GetTempPath(), "ILSpy-TypeForwarderResolution-" + Guid.NewGuid().ToString("N"));
			inputDirectory = Path.Combine(root, "input");
			frameworkDirectory = Path.Combine(root, "framework");
			Directory.CreateDirectory(inputDirectory);
			Directory.CreateDirectory(frameworkDirectory);

			await AssembleAsync(inputDirectory, "Shim", ShimIL).ConfigureAwait(false);
			await AssembleAsync(inputDirectory, "Impl", ImplCycleIL).ConfigureAwait(false);
			await AssembleAsync(inputDirectory, "Dup", DupIL.Replace("{VERSION}", "2")).ConfigureAwait(false);
			await AssembleAsync(frameworkDirectory, "Mid", MidIL).ConfigureAwait(false);
			await AssembleAsync(frameworkDirectory, "Impl", ImplRealIL).ConfigureAwait(false);
			await AssembleAsync(frameworkDirectory, "Dup", DupIL.Replace("{VERSION}", "1")).ConfigureAwait(false);
			await AssembleAsync(inputDirectory, "Shim2", Shim2IL).ConfigureAwait(false);
			await AssembleAsync(inputDirectory, "Impl2", Impl2CycleIL).ConfigureAwait(false);
			await AssembleAsync(frameworkDirectory, "Mid2", Mid2IL).ConfigureAwait(false);
			await AssembleAsync(frameworkDirectory, "Impl2", Impl2DecoyIL).ConfigureAwait(false);

			mainAssemblyPath = await AssembleAsync(inputDirectory, "Main", MainIL).ConfigureAwait(false);
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			Directory.Delete(Path.GetDirectoryName(inputDirectory), recursive: true);
		}

		static Task<string> AssembleAsync(string directory, string name, string il)
		{
			string sourceFile = Path.Combine(directory, name + ".il");
			File.WriteAllText(sourceFile, il);
			return Tester.AssembleIL(sourceFile, AssemblerOptions.Library);
		}

		DecompilerTypeSystem CreateTypeSystem()
		{
			var mainModule = new PEFile(mainAssemblyPath);
			// The target framework decides which probe runs first, and the bug only shows on the
			// .NET Core path finder, which the .NET Standard identifier selects.
			var resolver = new UniversalAssemblyResolver(mainAssemblyPath, throwOnError: false,
				".NETStandard,Version=v2.0");
			resolver.AddSearchDirectory(frameworkDirectory);
			return new DecompilerTypeSystem(mainModule, resolver);
		}

		IParameter GetParameterOf(DecompilerTypeSystem typeSystem, string methodName)
		{
			var consumer = typeSystem.MainModule.GetTypeDefinition(new TopLevelTypeName(string.Empty, "Consumer"));
			Assert.That(consumer, Is.Not.Null, "the fixture assembly must declare Consumer");
			var method = consumer.Methods.SingleOrDefault(m => m.Name == methodName);
			Assert.That(method, Is.Not.Null, $"Consumer must declare {methodName}");
			return method.Parameters.Single();
		}

		[Test]
		public void ForwarderChainLeavingTheInputDirectoryResolvesInTheDirectoryItReached()
		{
			var parameter = GetParameterOf(CreateTypeSystem(), "UseForwarded");

			using (Assert.EnterMultipleScope())
			{
				Assert.That(parameter.Type.Kind, Is.Not.EqualTo(TypeKind.Unknown),
					"the forwarded type must resolve; the chain used to cycle back into the input directory");
				Assert.That(parameter.Type.FullName, Is.EqualTo("Ns.T"));
				Assert.That(parameter.Type.GetDefinition().ParentModule.FullAssemblyName, Does.Contain("Version=1.0.0.0"),
					"it must come from the assembly that defines it, not from the higher-versioned facade next to the input");
			}
		}

		[Test]
		public void ChainThatCannotBeRepairedLeavesTheResolvedAssembliesAlone()
		{
			// The repaired chain ends at an assembly that forwards nothing - but declares nothing
			// either. Loading it would displace the assembly of the same name the input directory
			// holds, on the strength of an assumption that does not hold, so the repair declines and
			// the type stays unresolved exactly as it was.
			var typeSystem = CreateTypeSystem();
			var parameter = GetParameterOf(typeSystem, "UseUndeclared");

			using (Assert.EnterMultipleScope())
			{
				Assert.That(parameter.Type.Kind, Is.EqualTo(TypeKind.Unknown),
					"nothing declares Ns.U, so no repair can find it");
				var impl2 = typeSystem.Modules.SingleOrDefault(m => m.AssemblyName == "Impl2");
				Assert.That(impl2, Is.Not.Null, "Impl2 must still be loaded");
				Assert.That(impl2.FullAssemblyName, Does.Contain("Version=2.0.0.0"),
					"the copy next to the input assembly must not be displaced by one that declares nothing");
			}
		}

		[Test]
		public void OrdinaryReferenceStillResolvesNextToTheAssemblyBeingDecompiled()
		{
			var parameter = GetParameterOf(CreateTypeSystem(), "UseDuplicated");

			using (Assert.EnterMultipleScope())
			{
				Assert.That(parameter.Type.FullName, Is.EqualTo("Ns.D"));
				Assert.That(parameter.Type.GetDefinition().ParentModule.FullAssemblyName, Does.Contain("Version=2.0.0.0"),
					"a reference from an assembly that is not a facade keeps resolving next to the input assembly");
			}
		}
	}
}
