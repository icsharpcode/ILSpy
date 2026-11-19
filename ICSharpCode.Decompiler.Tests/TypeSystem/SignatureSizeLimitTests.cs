// Copyright (c) 2026 Christoph Wille
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

using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Tests.Helpers;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.TypeSystem
{
	[TestFixture]
	public class SignatureSizeLimitTests
	{
		[Test]
		public async Task HugeFieldSignature()
		{
			var csFile = Path.Combine(Tester.TestCasePath, "IsolatedDecompilation", "HugeGenericSignature.cs");
			var compilation = await Tester.CompileCSharp(csFile, CompilerOptions.UseRoslynLatest | CompilerOptions.Optimize | CompilerOptions.Library).ConfigureAwait(false);
			try
			{
				using var file = new FileStream(compilation.PathToAssembly, FileMode.Open, FileAccess.Read);
				var module = new PEFile(compilation.PathToAssembly, file, PEStreamOptions.PrefetchEntireImage);

				// The options of the type system that backs the assembly tree (LoadedAssembly.GetTypeSystemOrNull).
				var limited = FieldType(module, TypeSystemOptions.Default | TypeSystemOptions.Uncached
					| TypeSystemOptions.KeepModifiers | TypeSystemOptions.LimitSignatureSize);
				Assert.That(limited.Kind, Is.EqualTo(TypeKind.Unknown));

				// Without the option the signature is decoded in full, as the decompiler needs it.
				var full = FieldType(module, TypeSystemOptions.Default);
				Assert.That(full.Kind, Is.EqualTo(TypeKind.Class));
				Assert.That(full.TypeArguments, Has.Count.EqualTo(5));
			}
			finally
			{
				compilation.DeleteTempFiles();
			}
		}

		static IType FieldType(PEFile module, TypeSystemOptions options)
		{
			var typeSystem = new SimpleCompilation(module.WithOptions(options), MinimalCorlib.Instance);
			return typeSystem.MainModule.TypeDefinitions.Single(t => t.Name == "Y").Fields.Single().Type;
		}
	}
}
