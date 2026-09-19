// Copyright (c) 2018 Daniel Grunwald
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

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class NamedArguments
	{
		private class ClassWithNamedArgCtor
		{
			internal ClassWithNamedArgCtor(bool arg1 = false, bool arg2 = false)
			{
			}

			internal ClassWithNamedArgCtor()
				: this(arg2: Get(1) != 1, arg1: Get(2) == 2)
			{
			}
		}

		private class MustNotUseNamedArgsInCtor
		{
			public MustNotUseNamedArgsInCtor(string start = "", bool enable = false)
			{
			}

			public MustNotUseNamedArgsInCtor(bool enable, string start = "")
			{
			}

			public static MustNotUseNamedArgsInCtor Use()
			{
				// second overload
				MustNotUseNamedArgsInCall(true);
				// first overload
				MustNotUseNamedArgsInCall();
				return new MustNotUseNamedArgsInCtor(true);
			}

			public static void MustNotUseNamedArgsInCall(string start = "", bool enable = false)
			{
			}

			public static void MustNotUseNamedArgsInCall(bool enable, string start = "")
			{
			}
		}

		public class BaseNames
		{
			public virtual int this[int x, int y] {
				get {
					return x;
				}
				set {
				}
			}
		}

		public class DerivedNames : BaseNames
		{
			public override int this[int a, int b] {
				get {
					return a;
				}
				set {
				}
			}
		}

		public int this[int x, int y] {
			get {
				return x;
			}
			set {
			}
		}

		public int this[int i, object o] {
			get {
				return i;
			}
			set {
			}
		}

		public int this[int i, string o] {
			get {
				return i;
			}
			set {
			}
		}

		public void Use(int a, int b, int c)
		{
		}

		public static int Get(int i)
		{
			return i;
		}

		public void Test()
		{
			Use(Get(1), Get(2), Get(3));
			Use(Get(1), c: Get(2), b: Get(3));
			Use(b: Get(1), a: Get(2), c: Get(3));
		}

		public void NotNamedArgs()
		{
			int b = Get(1);
			Use(Get(2), b, Get(3));
		}

		public void NamedArgsForIndexer()
		{
			Use(this[y: Get(1), x: Get(2)], 0, 0);
			this[y: Get(1), x: Get(2)] = 3;
		}

		public void NamedArgsForIndexerNeedingCast()
		{
			Use(this[o: (object)((Get(1) == 1) ? "a" : "b"), i: Get(2)], 0, 0);
		}
		public void NamedArgsForOverriddenIndexer(DerivedNames derived)
		{
			// The names are the base indexer's, which is what the call instruction names.
			Use(((BaseNames)derived)[y: Get(1), x: Get(2)], 0, 0);
		}
	}
}
