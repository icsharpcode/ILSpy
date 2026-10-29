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

using System;
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
		public async Task PropertyAccessorUsedAsDelegateTarget()
		{
			using var context = await AssembleIL("AccessorDelegateReference.il").ConfigureAwait(false);
			var method = context.Method("C", "GetGetter");

			var code = context.Decompile(method);
			Assert.That(code, Does.Contain("get_P"));
			Assert.That(code, Does.Not.Contain("NullReferenceException"));

			var propertyCode = context.Decompile(context.Method("C", "GetProperty"));
			Assert.That(propertyCode, Does.Contain(".P"));
			Assert.That(propertyCode, Does.Not.Contain("get_P"));
		}

		[Test]
		public async Task StaticConstructorKeepsFieldInitializers()
		{
			using var context = await AssembleIL("IsolatedStaticCtor.il").ConfigureAwait(false);

			var staticCtor = context.Type("C")
				.Methods.Single(m => m.IsStatic && m.IsConstructor);

			// Decompile just the .cctor handle: the type's field declarations are not part of the
			// resulting syntax tree, so the static field initializers have nowhere to move to.
			var code = context.Decompile(staticCtor);

			// They must therefore remain as assignments in the constructor body rather than being
			// dropped (#3774).
			Assert.That(code, Does.Contain("Number = 42"));
			Assert.That(code, Does.Contain("Text = \"hello\""));
		}

		[Test]
		public async Task NestedTypeMemberInClassicExtensionClass()
		{
			// #3938: [Extension] on the container alone must not make members of an ordinary
			// nested type extension members; here the container has no extension blocks at all.
			await DecompileNestedGetMethod("ClassicExtensions").ConfigureAwait(false);
		}

		[Test]
		public async Task NestedTypeMemberInExtensionBlockContainer()
		{
			// Same as above, but the container also has a real C# 14 extension block whose
			// ExtensionInfo must not be attributed to the nested type's members.
			await DecompileNestedGetMethod("BlockExtensions").ConfigureAwait(false);
		}

		static async Task DecompileNestedGetMethod(string containerName)
		{
			var csFile = Path.Combine(TestCasePath, "ExtensionContainerNestedType.cs");
			var compilation = await Tester.CompileCSharp(csFile, CompilerOptions.UseRoslynLatest | CompilerOptions.Optimize | CompilerOptions.Library).ConfigureAwait(false);
			try
			{
				using var context = Open(compilation.PathToAssembly);

				var getMethod = context.Type(containerName)
					.NestedTypes.Single(t => t.Name == "Nested")
					.Methods.Single(m => m.Name == "Get");

				var code = context.Decompile(getMethod);

				Assert.That(code, Does.Contain("return 42;"));
				// The nested type's method must not be rendered inside an extension block.
				Assert.That(code, Does.Not.Contain("extension("));
			}
			finally
			{
				compilation.DeleteTempFiles();
			}
		}

		static async Task<IsolatedDecompiler> AssembleIL(string fileName)
		{
			var ilFile = Path.Combine(TestCasePath, fileName);
			var assembly = await Tester.AssembleIL(ilFile, AssemblerOptions.Library).ConfigureAwait(false);
			return Open(assembly);
		}

		static IsolatedDecompiler Open(string assembly)
		{
			var settings = new DecompilerSettings();
			var file = new FileStream(assembly, FileMode.Open, FileAccess.Read);
			var module = new PEFile(assembly, file, PEStreamOptions.PrefetchEntireImage);
			var targetFramework = module.Metadata.DetectTargetFrameworkId();
			var resolver = new UniversalAssemblyResolver(assembly, false, targetFramework, null, PEStreamOptions.PrefetchMetadata);
			resolver.AddSearchDirectory(Tester.RefAssembliesToolset.GetPath(targetFramework));
			var typeSystem = new DecompilerTypeSystem(module, resolver, settings);
			return new IsolatedDecompiler(file, module, typeSystem, new CSharpDecompiler(typeSystem, settings));
		}

		sealed class IsolatedDecompiler : IDisposable
		{
			readonly FileStream file;
			readonly PEFile module;

			public IsolatedDecompiler(FileStream file, PEFile module, DecompilerTypeSystem typeSystem, CSharpDecompiler decompiler)
			{
				this.file = file;
				this.module = module;
				TypeSystem = typeSystem;
				Decompiler = decompiler;
			}

			public DecompilerTypeSystem TypeSystem { get; }

			public CSharpDecompiler Decompiler { get; }

			public ITypeDefinition Type(string name)
			{
				return TypeSystem.MainModule.TypeDefinitions.Single(t => t.Name == name);
			}

			public IMethod Method(string typeName, string methodName)
			{
				return Type(typeName).Methods.Single(m => m.Name == methodName);
			}

			public string Decompile(IMethod method)
			{
				return Decompiler.Decompile(method.MetadataToken).ToString();
			}

			public void Dispose()
			{
				module.Dispose();
				file.Dispose();
			}
		}
	}
}
