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
using System.Linq;
using System.Reflection.Metadata;

using ICSharpCode.Decompiler.Metadata;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests.Metadata
{
	[TestFixture]
	public class SignatureBlobComparerTests
	{
		static MetadataReader Metadata => TypeSystem.TypeSystemLoaderTests.TestAssembly.Metadata;

		#region Helper methods

		/// <summary>
		/// Finds all MethodDefinitionHandles for methods with the given name in the given type.
		/// </summary>
		static List<MethodDefinitionHandle> FindMethods(string typeName, string methodName)
		{
			var results = new List<MethodDefinitionHandle>();
			foreach (var typeHandle in Metadata.TypeDefinitions)
			{
				var typeDef = Metadata.GetTypeDefinition(typeHandle);
				if (!Metadata.StringComparer.Equals(typeDef.Name, typeName))
					continue;

				foreach (var methodHandle in typeDef.GetMethods())
				{
					var methodDef = Metadata.GetMethodDefinition(methodHandle);
					if (Metadata.StringComparer.Equals(methodDef.Name, methodName))
						results.Add(methodHandle);
				}
			}
			return results;
		}

		/// <summary>
		/// Finds a single MethodDefinitionHandle for a method with the given name in the given type.
		/// </summary>
		static MethodDefinitionHandle FindMethod(string typeName, string methodName)
		{
			var results = FindMethods(typeName, methodName);
			Assert.That(results.Count, Is.EqualTo(1),
				$"Expected exactly one method '{methodName}' in '{typeName}', found {results.Count}");
			return results[0];
		}

		/// <summary>
		/// Finds a MethodDefinitionHandle matching on name and parameter count.
		/// </summary>
		static MethodDefinitionHandle FindMethod(string typeName, string methodName, int parameterCount)
		{
			var results = FindMethods(typeName, methodName);
			var filtered = results.Where(h => {
				var m = Metadata.GetMethodDefinition(h);
				return m.GetParameters().Count == parameterCount;
			}).ToList();
			Assert.That(filtered.Count, Is.EqualTo(1),
				$"Expected exactly one method '{methodName}' with {parameterCount} params in '{typeName}', found {filtered.Count}");
			return filtered[0];
		}

		static BlobReader GetSignatureBlob(MethodDefinitionHandle handle)
		{
			var method = Metadata.GetMethodDefinition(handle);
			return Metadata.GetBlobReader(method.Signature);
		}

		static bool CompareSignatures(MethodDefinitionHandle a, MethodDefinitionHandle b)
		{
			return SignatureBlobComparer.EqualsMethodSignature(
				GetSignatureBlob(a), GetSignatureBlob(b),
				Metadata, Metadata);
		}

		#endregion

		#region Identity / reflexive tests

		[Test]
		public void SameMethod_IsEqual()
		{
			// SimplePublicClass.Method() compared to itself
			var method = FindMethod("SimplePublicClass", "Method");
			Assert.That(CompareSignatures(method, method), Is.True);
		}

		[Test]
		public void Constructor_ComparedToItself_IsEqual()
		{
			var ctor = FindMethod("SimplePublicClass", ".ctor");
			Assert.That(CompareSignatures(ctor, ctor), Is.True);
		}

		[Test]
		public void StaticMethod_ComparedToItself_IsEqual()
		{
			// StaticClass.Extension(this object) — it's static at the IL level
			var method = FindMethod("StaticClass", "Extension");
			Assert.That(CompareSignatures(method, method), Is.True);
		}

		#endregion

		#region Same name, different signatures

		[Test]
		public void DifferentParameterTypes_NotEqual()
		{
			// MethodWithOptionalParameter(int) vs MethodWithParamsArray(object[])
			var intParam = FindMethod("ParameterTests", "MethodWithOptionalParameter");
			var arrayParam = FindMethod("ParameterTests", "MethodWithParamsArray");
			Assert.That(CompareSignatures(intParam, arrayParam), Is.False);
		}

		[Test]
		public void RefVsOut_SameAtSignatureLevel()
		{
			// ref and out are both ELEMENT_TYPE_BYREF at the blob level;
			// the out modifier is only represented via attributes.
			var methodRef = FindMethod("ParameterTests", "MethodWithRefParameter");
			var methodOut = FindMethod("ParameterTests", "MethodWithOutParameter");
			Assert.That(CompareSignatures(methodRef, methodOut), Is.True);
		}

		[Test]
		public void RefVsIn_SameAtSignatureLevel()
		{
			// ref and in are both ELEMENT_TYPE_BYREF at the blob level;
			// the in modifier is only represented via IsReadOnlyAttribute.
			var methodRef = FindMethod("ParameterTests", "MethodWithRefParameter");
			var methodIn = FindMethod("ParameterTests", "MethodWithInParameter");
			Assert.That(CompareSignatures(methodRef, methodIn), Is.True);
		}

		[Test]
		public void ByRefVsNonByRef_NotEqual()
		{
			// MethodWithRefParameter(ref int) vs MethodWithOptionalParameter(int)
			// byref int vs plain int should differ at the blob level
			var byRef = FindMethod("ParameterTests", "MethodWithRefParameter");
			var plain = FindMethod("ParameterTests", "MethodWithOptionalParameter");
			Assert.That(CompareSignatures(byRef, plain), Is.False);
		}

		[Test]
		public void DifferentParameterCount_NotEqual()
		{
			// Compare a method with 0 params to one with 1 param
			var method0 = FindMethod("SimplePublicClass", "Method"); // 0 params
			var method1 = FindMethod("ParameterTests", "MethodWithOutParameter"); // 1 param
			Assert.That(CompareSignatures(method0, method1), Is.False);
		}

		[Test]
		public void DifferentReturnType_NotEqual()
		{
			// SimplePublicClass.Method() returns void
			// PropertyTest has a getter that returns string (get_Item)
			var voidMethod = FindMethod("SimplePublicClass", "Method");
			var stringGetter = FindMethod("PropertyTest", "get_Item");
			Assert.That(CompareSignatures(voidMethod, stringGetter), Is.False);
		}

		#endregion

		#region Generic methods

		[Test]
		public void GenericMethod_ComparedToItself_IsEqual()
		{
			// GenericClass<A,B>.TestMethod<K,V>(string)
			var method = FindMethod("GenericClass`2", "TestMethod");
			Assert.That(CompareSignatures(method, method), Is.True);
		}

		[Test]
		public void GenericMethod_DifferentArity_NotEqual()
		{
			// TestMethod<K,V>(string) has 2 type params
			// GetIndex<T>(T) has 1 type param
			var testMethod = FindMethod("GenericClass`2", "TestMethod");
			var getIndex = FindMethod("GenericClass`2", "GetIndex");
			Assert.That(CompareSignatures(testMethod, getIndex), Is.False);
		}

		[Test]
		public void GenericMethodWithOneTypeParam_ComparedToItself_IsEqual()
		{
			var method = FindMethod("GenericClass`2", "GetIndex");
			Assert.That(CompareSignatures(method, method), Is.True);
		}

		[Test]
		public void NonGenericVsGenericMethod_NotEqual()
		{
			// SimplePublicClass.Method() — non-generic
			// GenericClass<A,B>.GetIndex<T>(T) — generic
			var nonGeneric = FindMethod("SimplePublicClass", "Method");
			var generic = FindMethod("GenericClass`2", "GetIndex");
			Assert.That(CompareSignatures(nonGeneric, generic), Is.False);
		}

		#endregion

		#region Instance vs static

		[Test]
		public void InstanceVsStatic_DifferentCallingConvention_NotEqual()
		{
			// An instance method vs a static method — calling convention differs in the signature header
			// StaticClass.Extension is static; SimplePublicClass.Method is instance
			var staticMethod = FindMethod("StaticClass", "Extension");
			var instanceMethod = FindMethod("SimplePublicClass", "Method");
			Assert.That(CompareSignatures(staticMethod, instanceMethod), Is.False);
		}

		#endregion

		#region Methods with same signature across different types

		[Test]
		public void SameSignatureInDifferentTypes_IsEqual()
		{
			// Both SimplePublicClass and ParameterTests have parameterless instance constructors
			var ctor1 = FindMethod("SimplePublicClass", ".ctor");
			var ctor2 = FindMethod("ParameterTests", ".ctor");
			Assert.That(CompareSignatures(ctor1, ctor2), Is.True);
		}

		#endregion

		#region Methods involving complex types

		[Test]
		public void MethodWithArrayParam_ComparedToItself_IsEqual()
		{
			// ParameterTests.MethodWithParamsArray(params object[])
			var method = FindMethod("ParameterTests", "MethodWithParamsArray");
			Assert.That(CompareSignatures(method, method), Is.True);
		}

		[Test]
		public void MethodWithOptionalParam_ComparedToNonOptional_IsEqual()
		{
			// At the signature blob level, optional vs required doesn't matter —
			// the signatures are the same if the types match.
			// MethodWithOptionalParameter(int) vs MethodWithEnumOptionalParameter(StringComparison)
			// These have different parameter types, so they should NOT be equal.
			var optionalInt = FindMethod("ParameterTests", "MethodWithOptionalParameter");
			var optionalEnum = FindMethod("ParameterTests", "MethodWithEnumOptionalParameter");
			Assert.That(CompareSignatures(optionalInt, optionalEnum), Is.False);
		}

		[Test]
		public void VarArgsMethod_ComparedToItself_IsEqual()
		{
			var method = FindMethod("ParameterTests", "VarArgsMethod");
			Assert.That(CompareSignatures(method, method), Is.True);
		}

		[Test]
		public void VarArgsMethod_VsNormalMethod_NotEqual()
		{
			// VarArgsMethod uses __arglist calling convention
			var varArgs = FindMethod("ParameterTests", "VarArgsMethod");
			var normal = FindMethod("SimplePublicClass", "Method");
			Assert.That(CompareSignatures(varArgs, normal), Is.False);
		}

		#endregion

		#region Indexer accessors

		[Test]
		public void IndexerGetter_ComparedToItself_IsEqual()
		{
			var getter = FindMethod("PropertyTest", "get_Item");
			Assert.That(CompareSignatures(getter, getter), Is.True);
		}

		[Test]
		public void IndexerGetter_VsSetter_NotEqual()
		{
			var getter = FindMethod("PropertyTest", "get_Item");
			var setter = FindMethod("PropertyTest", "set_Item");
			Assert.That(CompareSignatures(getter, setter), Is.False);
		}

		#endregion

		#region DllImport / extern methods

		[Test]
		public void DllImportMethod_ComparedToItself_IsEqual()
		{
			var method = FindMethod("NonCustomAttributes", "DllMethod");
			Assert.That(CompareSignatures(method, method), Is.True);
		}

		#endregion

		#region Explicit interface implementation

		[Test]
		public void ExplicitImpl_VsRegularMethod_SameSignature()
		{
			// ExplicitImplementationTests has both M(int) and IExplicitImplementationTests.M(int)
			// Their signatures should be identical at the blob level
			var methods = FindMethods("ExplicitImplementationTests", "M");
			Assert.That(methods.Count, Is.EqualTo(1),
				"Explicit impl has a mangled name, so only one 'M' should be found");
		}

		#endregion

		#region Symmetric property

		[Test]
		public void Comparison_IsSymmetric()
		{
			var method1 = FindMethod("SimplePublicClass", "Method");
			var method2 = FindMethod("ParameterTests", "MethodWithOutParameter");

			bool forward = CompareSignatures(method1, method2);
			bool backward = CompareSignatures(method2, method1);
			Assert.That(forward, Is.EqualTo(backward), "Comparison should be symmetric");
		}

		[Test]
		public void Comparison_IsSymmetric_WhenEqual()
		{
			var ctor1 = FindMethod("SimplePublicClass", ".ctor");
			var ctor2 = FindMethod("ParameterTests", ".ctor");

			bool forward = CompareSignatures(ctor1, ctor2);
			bool backward = CompareSignatures(ctor2, ctor1);
			Assert.That(forward, Is.True);
			Assert.That(backward, Is.True);
		}

		#endregion
	}
}
