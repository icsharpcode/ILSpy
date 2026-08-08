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
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

using ICSharpCode.Decompiler.CSharp;
using ICSharpCode.Decompiler.CSharp.Syntax;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Semantics;
using ICSharpCode.Decompiler.Tests.Helpers;
using ICSharpCode.Decompiler.TypeSystem;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	/// <summary>
	/// The Pretty tests compare printed text, which says nothing about the resolve results the
	/// decompiler annotates the syntax tree with. Those annotations are what the UI navigates by,
	/// so attribute arguments written in expanded params form are checked here directly.
	/// </summary>
	[TestFixture]
	public class AttributeAnnotationTests
	{
		static async Task<SyntaxTree> DecompileCustomAttributes()
		{
			var sourceFile = Path.Combine(Tester.TestCasePath, "Pretty", "CustomAttributes.cs");
			var results = await Tester.CompileCSharp(sourceFile,
				CompilerOptions.Library | CompilerOptions.UseRoslynLatest).ConfigureAwait(false);

			var settings = new DecompilerSettings();
			using var stream = new FileStream(results.PathToAssembly, FileMode.Open, FileAccess.Read);
			var module = new PEFile(results.PathToAssembly, stream, PEStreamOptions.PrefetchEntireImage);
			var targetFramework = module.Metadata.DetectTargetFrameworkId();
			var resolver = new UniversalAssemblyResolver(results.PathToAssembly, false, targetFramework,
				null, PEStreamOptions.PrefetchMetadata);
			resolver.AddSearchDirectory(Tester.RefAssembliesToolset.GetPath(targetFramework));
			var typeSystem = new DecompilerTypeSystem(module, resolver, settings);
			var decompiler = new CSharpDecompiler(typeSystem, settings);
			return decompiler.DecompileType(new FullTypeName("CustomAttributes.CustomAttributes"));
		}

		/// <summary>
		/// Picks one of a method's attributes by the string literal it starts with.
		/// </summary>
		static Attribute FindAttribute(SyntaxTree tree, string methodName, string firstArgument)
		{
			var method = tree.Descendants.OfType<MethodDeclaration>()
				.Single(m => m.Name == methodName);
			return method.Attributes.SelectMany(section => section.Attributes)
				.Single(a => a.Arguments.FirstOrDefault() is PrimitiveExpression { Value: string text }
					&& text == firstArgument);
		}

		[Test]
		public async Task ExpandedParamsAttributeResolvesToParamsConstructor()
		{
			var tree = await DecompileCustomAttributes().ConfigureAwait(false);
			// [GenericParams<int>("two values", 47, 11)]
			var attribute = FindAttribute(tree, "UseGenericParamsAttribute", "two values");

			Assert.That(attribute.Arguments.Count, Is.EqualTo(3), "arguments must be in expanded form");

			var rr = attribute.GetResolveResult() as MemberResolveResult;
			Assert.That(rr, Is.Not.Null, "attribute must be annotated with its constructor");
			var ctor = (IMethod)rr.Member;
			Assert.That(ctor.IsConstructor);
			Assert.That(ctor.DeclaringType.ReflectionName,
				Is.EqualTo("CustomAttributes.CustomAttributes+GenericParamsAttribute`1[[System.Int32]]"));
			// The annotation must name the params constructor, not an overload that the shortened
			// argument list happens to fit.
			Assert.That(ctor.Parameters.Count, Is.EqualTo(2));
			Assert.That(ctor.Parameters[1].IsParams);
		}

		[Test]
		public async Task ExpandedParamsArgumentsAreAnnotatedWithTheElementType()
		{
			var tree = await DecompileCustomAttributes().ConfigureAwait(false);
			var attribute = FindAttribute(tree, "UseGenericParamsAttribute", "two values");

			var text = attribute.Arguments.First().GetResolveResult();
			Assert.That(text.Type.IsKnownType(KnownTypeCode.String));
			Assert.That(text.ConstantValue, Is.EqualTo("two values"));

			// The two expanded elements are arguments in their own right now, so each must carry
			// the element type -- not the int[] of the array they were encoded in.
			foreach (var (argument, value) in attribute.Arguments.Skip(1).Zip(new object[] { 47, 11 }))
			{
				var elementRR = argument.GetResolveResult();
				Assert.That(elementRR.Type.IsKnownType(KnownTypeCode.Int32));
				Assert.That(elementRR.ConstantValue, Is.EqualTo(value));
			}
		}

		[Test]
		public async Task SuppressedExpansionKeepsTheArrayConstructor()
		{
			var tree = await DecompileCustomAttributes().ConfigureAwait(false);
			// [GenericParamsOverloaded<int>("no values", new int[] { })] -- expanding this would
			// bind to GenericParamsOverloadedAttribute<int>(string) instead.
			var attribute = FindAttribute(tree, "UseGenericParamsOverloadedAttribute", "no values");

			var rr = attribute.GetResolveResult() as MemberResolveResult;
			Assert.That(rr, Is.Not.Null);
			var ctor = (IMethod)rr.Member;
			Assert.That(ctor.Parameters.Count, Is.EqualTo(2));
			Assert.That(attribute.Arguments.Last(), Is.InstanceOf<ArrayCreateExpression>());
		}
	}
}
