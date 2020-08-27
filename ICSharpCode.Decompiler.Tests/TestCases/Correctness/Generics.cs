// Copyright (c) 2010-2013 AlphaSierraPapa for the SharpDevelop Team
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

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	/// <summary>
	/// Description of Generics.
	/// </summary>
	public class Generics
	{
		public static void Main()
		{
			Console.WriteLine(TestGenericReturn<int>());
			Console.WriteLine(TestGenericReturn<string>());
			TestGenericParam<string>();
			TestGenericParam<int, int>();
		}

		public static T TestGenericReturn<T>()
		{
			return default(T);
		}

		public static void TestGenericParam<T>()
		{
			Console.WriteLine(typeof(T));
		}

		public static void TestGenericParam<T1, T2>()
		{
			Console.WriteLine(typeof(T1) + " " + typeof(T2));
		}
	}

	class GenericClass<T>
	{
		public void M(out GenericClass<T> self)
		{
			self = this;
		}
	}

	public abstract class BaseClass
	{
		protected abstract void Method1<T>(T test);
	}

	public class DerivedClass : BaseClass
	{
		protected override void Method1<T>(T test) { }

		private void Method2()
		{
			this.Method1(0);
		}
	}

	#region Issue 958 - Invalid cast in generic class for abstract base call
	internal abstract class BaseClass<T>
	{
		protected abstract void Method1();
	}

	internal class DerivedClass<T> : BaseClass<T>
	{
		protected override void Method1() { }

		private void Method2()
		{
			this.Method1();
		}
	}
	#endregion
}
