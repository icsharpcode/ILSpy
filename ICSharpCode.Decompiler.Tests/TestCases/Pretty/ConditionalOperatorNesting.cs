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

using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class ConditionalOperatorNesting
	{
		private static void Use(object value)
		{
		}

		public int ShallowChainStaysInline(int n, int[] a, int[] b)
		{
			return ((n > 20) ? a[0] : b[0]) + 1;
		}

		// One conditional is under the limit and stays where it is, whatever the branch types.
		public void MismatchedBranchTypesUnderTheLimitStayInline(int n, string s, object o)
		{
			Use((n > 10) ? s : o);
		}

		public void LogicOperatorsAreNotCounted(int n, int[] a)
		{
			Use(n > 40 && a[0] > 1 && a[1] > 2 && a[2] > 3 && a[3] > 4);
		}

		public int FilterKeepsExpression(int n, int[] a, int[] b)
		{
			try
			{
				throw new Exception();
			}
			catch (Exception) when (((n > 40) ? a[0] : ((n > 30) ? b[0] : ((n > 20) ? a[1] : ((n > 10) ? b[1] : a[2])))) > 5)
			{
				return 1;
			}
		}
	}

	public class ConditionalOperatorNestingBase
	{
		public ConditionalOperatorNestingBase(int x)
		{
		}
	}

	public class ConditionalOperatorNestingCtor : ConditionalOperatorNestingBase
	{
		public ConditionalOperatorNestingCtor(int n, int[] a, int[] b)
			: base((n > 40) ? a[0] : ((n > 30) ? b[0] : ((n > 20) ? a[1] : ((n > 10) ? b[1] : a[2]))))
		{
		}

		public ConditionalOperatorNestingCtor(int n, int[] a)
			: this(n, a, a)
		{
		}
	}

#if CS90
	public class ConditionalOperatorNestingInit
	{
		public int P { get; init; }
	}

	public record ConditionalOperatorNestingRecord
	{
		public int P { get; init; }
	}
#endif

	// A nested conditional feeding a construct that has to stay a single expression must not be
	// expanded: the transform that recognizes the construct matches on the expression, and an
	// if-else between the statements silently stops it matching - or produces code that does not
	// compile, as an object initializer assigning an init-only member would.
	public class ConditionalOperatorNestingSingleExpression
	{
		public int[] ArrayInitializer(int n, int a, int b, int c)
		{
			return new int[2] {
				(n > 2) ? a : ((n > 1) ? b : c),
				5
			};
		}

		public IEnumerable<int> Query(int[] xs, int n, int a, int b, int c)
		{
			return xs.Where((int x) => x > ((n > 2) ? a : ((n > 1) ? b : c)));
		}
#if CS70
		public int RefLocal(int n, int[] a, int[] b)
		{
			ref int reference = ref n > 2 ? ref a[0] : ref n > 1 ? ref b[0] : ref a[1];
			reference++;
			return reference;
		}
#endif
#if CS80
		public int SwitchExpression(int k, int n, int a, int b, int c)
		{
			return k switch {
				1 => (n > 2) ? a : ((n > 1) ? b : c),
				2 => a,
				_ => b,
			};
		}
#endif
#if CS90
		public ConditionalOperatorNestingInit ObjectInitializer(int n, int a, int b, int c)
		{
			return new ConditionalOperatorNestingInit {
				P = ((n > 2) ? a : ((n > 1) ? b : c))
			};
		}

		public ConditionalOperatorNestingRecord WithExpression(ConditionalOperatorNestingRecord i, int n, int a, int b, int c)
		{
			return i with {
				P = ((n > 2) ? a : ((n > 1) ? b : c))
			};
		}
#endif
	}
}
