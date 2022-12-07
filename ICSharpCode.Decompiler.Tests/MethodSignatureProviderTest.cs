// Copyright (c) 2022 Tom-Englert
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

using ICSharpCode.Decompiler.Disassembler;
using ICSharpCode.Decompiler.Metadata;

using NUnit.Framework;

namespace ICSharpCode.Decompiler.Tests
{
	[TestFixture, Parallelizable(ParallelScope.All)]
	public class MethodSignatureProviderTest
	{
		[TestCase("ICSharpCode.Decompiler.Tests.MethodSignatureProviderTest", nameof(Method1), "uint", new[] { "string", "int", "double", "string[]", "int[,]", "ref ICSharpCode.Decompiler.Disassembler.IMethodSignature", "ICSharpCode.Decompiler.Tests.MethodSignatureProviderTest", "System.Func<int, double>" })]
		[TestCase("ICSharpCode.Decompiler.Tests.MethodSignatureProviderTest", "Method1`1", "object", new[] { "string", "int" })]
		[TestCase("ICSharpCode.Decompiler.Tests.MethodSignatureProviderTest", nameof(Method3), "void", new[] { "ref string", "ref string", "ref string" })]
		[TestCase("ICSharpCode.Decompiler.Tests.MethodSignatureProviderTest.GenericClass`1", "Method1`1", "void", new[] { "TOuter", "TInner", "System.Func<TOuter, TInner>", "ICSharpCode.Decompiler.Tests.MethodSignatureProviderTest.GenericClass<TInner>" })]
		public void GetCorrectMethodSignature(string typeName, string methodName, string returnType, string[] parameterTypes)
		{
			using var module = new PEFile(typeof(MethodSignatureProviderTest).Assembly.Location);

			var method = module.FindMethod(typeName, methodName);

			var signature = method.GetMethodSignature(module);

			Assert.AreEqual(methodName, signature.Name);
			Assert.AreEqual(returnType, signature.ReturnType);
			Assert.AreEqual(string.Join(", ", parameterTypes), string.Join(", ", signature.ArgumentTypes));
		}

		[Test]
		public void FindOverloadedMethodByCondition()
		{
			using var module = new PEFile(typeof(MethodSignatureProviderTest).Assembly.Location);

			Assert.Throws<InvalidOperationException>(() => module.FindMethod(typeof(MethodSignatureProviderTest).FullName, "Method2`1"));
			Assert.Throws<InvalidOperationException>(() => module.FindMethod(typeof(MethodSignatureProviderTest).FullName, "Method2`1", signature => signature.ArgumentTypes.Count == 2));

			var method = module.FindMethod(typeof(MethodSignatureProviderTest).FullName, "Method2`1", signature => signature.ArgumentTypes.Count == 2 && signature.ArgumentTypes[1] == "double");

			Assert.NotNull(method);
		}

#pragma warning disable CA1822 // Mark members as static

		private UInt32 Method1(string s, int i, double d, string[] arr, int[,] arr2, ref IMethodSignature refStruct, MethodSignatureProviderTest type, Func<int, double> func)
		{
			return default;
		}

		private object Method1<T>(string s, int i)
		{
			return default;
		}

		private object Method2<T>(string s, int i)
		{
			return default;
		}

		private void Method2<T>(string s, double i)
		{
		}

		private void Method2<T>(string s, double i, byte b)
		{
		}

		private void Method3(in string a, out string b, ref string c)
		{
			b = string.Empty;
		}

		private class GenericClass<TOuter>
		{
			public void Method1<TInner>(TOuter a, TInner b, Func<TOuter, TInner> c, GenericClass<TInner> d)
			{
			}
		}
	}
}
