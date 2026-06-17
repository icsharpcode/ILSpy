// Copyright (c) 2026 AlphaSierraPapa for the SharpDevelop Team
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
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Tests.Helpers;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	/// <summary>
	/// Decompiling a single member in isolation (the "IL with C#" view does this, decompiling one
	/// method handle at a time) gives the transforms only a partial syntax tree. This is the mirror
	/// image of the whole-file ILPretty tests: same assembled-IL fixture, but the assertion is about
	/// one method's output rather than the full module.
	/// </summary>
	[TestFixture]
	public class IsolatedMethodDecompilationTests
	{
		static readonly string TestCasePath = Tester.TestCasePath + "/IsolatedDecompilation";

		[Test]
		public async Task StaticConstructorKeepsFieldInitializers()
		{
			var ilFile = Path.Combine(TestCasePath, "IsolatedStaticCtor.il");
			var assembly = await Tester.AssembleIL(ilFile, AssemblerOptions.Library).ConfigureAwait(false);

			var settings = new DecompilerSettings();
			using var file = new FileStream(assembly, FileMode.Open, FileAccess.Read);
			var module = new PEFile(assembly, file, PEStreamOptions.PrefetchEntireImage);
			var targetFramework = module.Metadata.DetectTargetFrameworkId();
			var resolver = new UniversalAssemblyResolver(assembly, false, targetFramework, null, PEStreamOptions.PrefetchMetadata);
			resolver.AddSearchDirectory(Tester.RefAssembliesToolset.GetPath(targetFramework));
			var typeSystem = new DecompilerTypeSystem(module, resolver, settings);
			var decompiler = new CSharpDecompiler(typeSystem, settings);

			var staticCtor = typeSystem.MainModule.TypeDefinitions
				.Single(t => t.Name == "C")
				.Methods.Single(m => m.IsStatic && m.IsConstructor);

			// Decompile just the .cctor handle: the type's field declarations are not part of the
			// resulting syntax tree, so the static field initializers have nowhere to move to.
			var code = decompiler.Decompile(staticCtor.MetadataToken).ToString();

			// They must therefore remain as assignments in the constructor body rather than being
			// dropped (#3774).
			Assert.That(code, Does.Contain("Number = 42"));
			Assert.That(code, Does.Contain("Text = \"hello\""));
		}
	}
}
