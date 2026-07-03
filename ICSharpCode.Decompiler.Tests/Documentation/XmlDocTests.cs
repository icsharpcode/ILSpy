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
using System.Threading.Tasks;

using ICSharpCode.Decompiler.Documentation;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.Tests.Helpers;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Documentation
{
	[TestFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable",
		Justification = "The PEFile is disposed by the OneTimeTearDown method.")]
	public class XmlDocTests
	{
		// Compiled by Tester.CompileCSharp with CompilerOptions.ProcessXmlDoc, so the member
		// ID strings in the documentation file come from the compiler itself. Looking an
		// entity's documentation up therefore checks IdStringProvider.GetIdString against
		// the compiler-generated ID for the same member.
		const string SampleCode = """
			using System;

			namespace XmlDocSamples
			{
				/// <summary>Documentation for the sample class.</summary>
				public class Sample
				{
					/// <summary>Documentation for the field.</summary>
					public int Field;

					/// <summary>Documentation for the event.</summary>
					public event EventHandler Event;

					/// <summary>Documentation for the constructor.</summary>
					public Sample(int x)
					{
					}

					/// <summary>Documentation for the indexer.</summary>
					public string this[int index] => null;

					/// <summary>Documentation for ref/out parameters.</summary>
					public void MethodWithRefOutParams(ref int x, out string y)
					{
						y = null;
					}

					/// <summary>Documentation for params.</summary>
					public void MethodWithParamsArray(params object[] args)
					{
					}

					/// <summary>Documentation for pointers.</summary>
					public unsafe void MethodWithPointer(int* p)
					{
					}

					/// <summary>Documentation for arrays.</summary>
					public void MethodWithArrays(int[] a, string[,] b, int[][] c)
					{
					}

					/// <summary>Documentation for the addition operator.</summary>
					public static Sample operator +(Sample a, Sample b)
					{
						return a;
					}

					/// <summary>Documentation for the implicit conversion.</summary>
					public static implicit operator int(Sample a)
					{
						return 0;
					}

					/// <summary>Documentation for the explicit conversion.</summary>
					public static explicit operator Sample(int a)
					{
						return null;
					}
				}

				/// <summary>Documentation for the generic sample class.</summary>
				public class GenericSample<A, B>
				{
					/// <summary>Documentation for the nested generic class.</summary>
					public class Nested<C>
					{
						/// <summary>Documentation for the nested method.</summary>
						public void NestedMethod(A a, C c)
						{
						}
					}

					/// <summary>Documentation for the method using class type parameters.</summary>
					public void Method(A a, B b)
					{
					}

					/// <summary>Documentation for the generic method.</summary>
					public C GenericMethod<C>(C input, A other)
					{
						return input;
					}
				}
			}
			""";

		const string NS = "XmlDocSamples";

		string sourceFile;
		CompilerResults results;
		PEFile peFile;
		ICompilation compilation;
		XmlDocumentationProvider documentation;

		[OneTimeSetUp]
		public async Task CompileSample()
		{
			sourceFile = Path.Combine(Path.GetTempPath(), $"XmlDocSamples-{Guid.NewGuid():N}.cs");
			File.WriteAllText(sourceFile, SampleCode);
			results = await Tester.CompileCSharp(sourceFile,
				CompilerOptions.UseRoslynLatest | CompilerOptions.Optimize | CompilerOptions.Library | CompilerOptions.ProcessXmlDoc);
			peFile = new PEFile(results.PathToAssembly,
				new FileStream(results.PathToAssembly, FileMode.Open, FileAccess.Read));
			compilation = new SimpleCompilation(peFile.WithOptions(TypeSystemOptions.Default), MinimalCorlib.Instance);
			documentation = new XmlDocumentationProvider(Path.ChangeExtension(results.PathToAssembly, ".xml"));
		}

		[OneTimeTearDown]
		public void Cleanup()
		{
			peFile?.Dispose();
			if (results != null)
			{
				string xmlFile = Path.ChangeExtension(results.PathToAssembly, ".xml");
				results.DeleteTempFiles();
				if (File.Exists(xmlFile))
					File.Delete(xmlFile);
			}
			if (sourceFile != null && File.Exists(sourceFile))
				File.Delete(sourceFile);
		}

		ITypeDefinition GetTypeDefinition(string reflectionName)
		{
			var typeDef = compilation.FindType(new FullTypeName(reflectionName)).GetDefinition();
			Assert.That(typeDef, Is.Not.Null, $"Could not resolve {reflectionName}");
			return typeDef;
		}

		void AssertIdStringRoundTrip(IEntity entity, string expectedIdString)
		{
			Assert.That(entity.GetIdString(), Is.EqualTo(expectedIdString));
			var found = IdStringProvider.FindEntity(expectedIdString, new SimpleTypeResolveContext(compilation));
			Assert.That(found, Is.EqualTo(entity), $"FindEntity did not round-trip {expectedIdString}");
		}

		void AssertDocumentation(IEntity entity, string expectedText)
		{
			Assert.That(documentation.GetDocumentation(entity), Does.Contain(expectedText),
				$"Documentation not found for {entity.GetIdString()}");
		}

		[Test]
		public void TypeIdStrings()
		{
			AssertIdStringRoundTrip(GetTypeDefinition($"{NS}.Sample"),
				$"T:{NS}.Sample");
			AssertIdStringRoundTrip(GetTypeDefinition($"{NS}.GenericSample`2"),
				$"T:{NS}.GenericSample`2");
			AssertIdStringRoundTrip(GetTypeDefinition($"{NS}.GenericSample`2+Nested`1"),
				$"T:{NS}.GenericSample`2.Nested`1");
		}

		[Test]
		public void FieldEventAndConstructorIdStrings()
		{
			var sample = GetTypeDefinition($"{NS}.Sample");
			AssertIdStringRoundTrip(sample.Fields.Single(f => f.Name == "Field"),
				$"F:{NS}.Sample.Field");
			AssertIdStringRoundTrip(sample.Events.Single(),
				$"E:{NS}.Sample.Event");
			AssertIdStringRoundTrip(sample.Methods.Single(m => m.IsConstructor),
				$"M:{NS}.Sample.#ctor(System.Int32)");
		}

		[Test]
		public void MethodIdStrings()
		{
			var sample = GetTypeDefinition($"{NS}.Sample");
			AssertIdStringRoundTrip(sample.Methods.Single(m => m.Name == "MethodWithRefOutParams"),
				$"M:{NS}.Sample.MethodWithRefOutParams(System.Int32@,System.String@)");
			AssertIdStringRoundTrip(sample.Methods.Single(m => m.Name == "MethodWithParamsArray"),
				$"M:{NS}.Sample.MethodWithParamsArray(System.Object[])");
			AssertIdStringRoundTrip(sample.Methods.Single(m => m.Name == "MethodWithPointer"),
				$"M:{NS}.Sample.MethodWithPointer(System.Int32*)");
			AssertIdStringRoundTrip(sample.Methods.Single(m => m.Name == "MethodWithArrays"),
				$"M:{NS}.Sample.MethodWithArrays(System.Int32[],System.String[0:,0:],System.Int32[][])");
		}

		[Test]
		public void IndexerIdString()
		{
			var sample = GetTypeDefinition($"{NS}.Sample");
			AssertIdStringRoundTrip(sample.Properties.Single(p => p.IsIndexer),
				$"P:{NS}.Sample.Item(System.Int32)");
		}

		[Test]
		public void OperatorIdStrings()
		{
			var sample = GetTypeDefinition($"{NS}.Sample");
			AssertIdStringRoundTrip(sample.Methods.Single(m => m.Name == "op_Addition"),
				$"M:{NS}.Sample.op_Addition({NS}.Sample,{NS}.Sample)");
			AssertIdStringRoundTrip(sample.Methods.Single(m => m.Name == "op_Implicit"),
				$"M:{NS}.Sample.op_Implicit({NS}.Sample)~System.Int32");
			AssertIdStringRoundTrip(sample.Methods.Single(m => m.Name == "op_Explicit"),
				$"M:{NS}.Sample.op_Explicit(System.Int32)~{NS}.Sample");
		}

		[Test]
		public void GenericMemberIdStrings()
		{
			var generic = GetTypeDefinition($"{NS}.GenericSample`2");
			AssertIdStringRoundTrip(generic.Methods.Single(m => m.Name == "Method"),
				$"M:{NS}.GenericSample`2.Method(`0,`1)");
			AssertIdStringRoundTrip(generic.Methods.Single(m => m.Name == "GenericMethod"),
				$"M:{NS}.GenericSample`2.GenericMethod``1(``0,`0)");
			var nested = GetTypeDefinition($"{NS}.GenericSample`2+Nested`1");
			AssertIdStringRoundTrip(nested.Methods.Single(m => m.Name == "NestedMethod"),
				$"M:{NS}.GenericSample`2.Nested`1.NestedMethod(`0,`2)");
		}

		[Test]
		public void LookupByIdString()
		{
			Assert.That(documentation.GetDocumentation($"T:{NS}.Sample"),
				Does.Contain("Documentation for the sample class."));
			Assert.That(documentation.GetDocumentation($"T:{NS}.DoesNotExist"), Is.Null);
		}

		[Test]
		public void LookupByEntityMatchesCompilerGeneratedIds()
		{
			var sample = GetTypeDefinition($"{NS}.Sample");
			AssertDocumentation(sample, "Documentation for the sample class.");
			AssertDocumentation(sample.Fields.Single(f => f.Name == "Field"), "Documentation for the field.");
			AssertDocumentation(sample.Events.Single(), "Documentation for the event.");
			AssertDocumentation(sample.Methods.Single(m => m.IsConstructor), "Documentation for the constructor.");
			AssertDocumentation(sample.Properties.Single(p => p.IsIndexer), "Documentation for the indexer.");
			AssertDocumentation(sample.Methods.Single(m => m.Name == "MethodWithRefOutParams"), "Documentation for ref/out parameters.");
			AssertDocumentation(sample.Methods.Single(m => m.Name == "MethodWithParamsArray"), "Documentation for params.");
			AssertDocumentation(sample.Methods.Single(m => m.Name == "MethodWithPointer"), "Documentation for pointers.");
			AssertDocumentation(sample.Methods.Single(m => m.Name == "MethodWithArrays"), "Documentation for arrays.");
			AssertDocumentation(sample.Methods.Single(m => m.Name == "op_Addition"), "Documentation for the addition operator.");
			AssertDocumentation(sample.Methods.Single(m => m.Name == "op_Implicit"), "Documentation for the implicit conversion.");
			AssertDocumentation(sample.Methods.Single(m => m.Name == "op_Explicit"), "Documentation for the explicit conversion.");

			var generic = GetTypeDefinition($"{NS}.GenericSample`2");
			AssertDocumentation(generic, "Documentation for the generic sample class.");
			AssertDocumentation(generic.Methods.Single(m => m.Name == "Method"), "Documentation for the method using class type parameters.");
			AssertDocumentation(generic.Methods.Single(m => m.Name == "GenericMethod"), "Documentation for the generic method.");
			var nested = GetTypeDefinition($"{NS}.GenericSample`2+Nested`1");
			AssertDocumentation(nested, "Documentation for the nested generic class.");
			AssertDocumentation(nested.Methods.Single(m => m.Name == "NestedMethod"), "Documentation for the nested method.");
		}
	}
}
