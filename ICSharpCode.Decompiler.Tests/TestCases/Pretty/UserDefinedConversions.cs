// Copyright (c) 2019 Daniel Grunwald
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
	internal class T01Issue1574
	{
		private struct A
		{
			private bool val;

			public static implicit operator bool(A a)
			{
				return a.val;
			}
		}

		private struct C
		{
			private int val;

			public static implicit operator C(bool b)
			{
				return default(C);
			}
		}

		private C ChainedConversion()
		{
			return (bool)default(A);
		}

		public void Call_Overloaded()
		{
			Overloaded((bool)default(A));
		}

		private void Overloaded(A a)
		{
		}

		private void Overloaded(bool a)
		{
		}
	}

	internal class T02BothDirectAndChainedConversionPossible
	{
		private struct A
		{
			private bool val;

			public static implicit operator bool(A a)
			{
				return a.val;
			}
		}

		private struct C
		{
			private int val;

			public static implicit operator C(bool b)
			{
				return default(C);
			}

			public static implicit operator C(A a)
			{
				return default(C);
			}

			public static bool operator ==(C a, C b)
			{
				return true;
			}
			public static bool operator !=(C a, C b)
			{
				return false;
			}
		}

		private C DirectConvert(A a)
		{
			return a;
		}

		private C IndirectConvert(A a)
		{
			return (bool)a;
		}

		private C? LiftedDirectConvert(A? a)
		{
			return a;
		}

		private C? LiftedIndirectConvert(A? a)
		{
			return (bool?)a;
		}

		private bool Compare(A a, C c)
		{
			return a == c;
		}

		private void LiftedCompare(A? a, C? c)
		{
			UseBool(a == c);
			UseBool(a == default(C));
			UseBool(c == default(A));
		}

		private void UseBool(bool b)
		{
		}
	}
}
