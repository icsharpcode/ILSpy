using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class ExtendedPropertyPatterns
	{
		public class Root
		{
			public int? NullableInt;

			public A A { get; set; }
		}

		public class A
		{
			public B B { get; set; }

			public int I { get; set; }

			public object Obj { get; set; }
		}

		public class B
		{
			public C C { get; set; }

			public int I { get; set; }

			public string Text { get; set; }
		}

		public class C
		{
			public int I;
		}

		public void TwoLinksConstant(object x)
		{
			if (x is Root { A.I: 5 })
			{
				Console.WriteLine("match");
			}
			else
			{
				Console.WriteLine("no match");
			}
		}

		public void TwoLinksNegatedConstant(object x)
		{
			if (x is Root { A.I: not 5 })
			{
				Console.WriteLine("match");
			}
			else
			{
				Console.WriteLine("no match");
			}
		}

		public void ThreeLinksNotNull(object x)
		{
			if (x is Root { A.B.C: not null })
			{
				Console.WriteLine("match");
			}
			else
			{
				Console.WriteLine("no match");
			}
		}

		public void VarCapture(object x)
		{
			if (x is Root { A.B: var b })
			{
				Console.WriteLine(b);
			}
			else
			{
				Console.WriteLine("no match");
			}
		}

		public void TypePattern(object x)
		{
			if (x is Root { A.Obj: string obj })
			{
				Console.WriteLine(obj);
			}
			else
			{
				Console.WriteLine("no match");
			}
		}

		public void StringLengthConstant(object x)
		{
			if (x is Root { A.B.Text.Length: 0 })
			{
				Console.WriteLine("match");
			}
			else
			{
				Console.WriteLine("no match");
			}
		}

		public void StringLengthVar(object x)
		{
			if (x is Root { A.B.Text.Length: var length })
			{
				Console.WriteLine(length);
			}
			else
			{
				Console.WriteLine("no match");
			}
		}

		public void MixedDottedAndPlain(object x)
		{
			if (x is Root { A.I: 5, NullableInt: 42 })
			{
				Console.WriteLine("match");
			}
			else
			{
				Console.WriteLine("no match");
			}
		}

		public void TopLevelDesignation(object obj)
		{
			if (obj is Root { A.I: 5 } root)
			{
				Console.WriteLine(root);
			}
			else
			{
				Console.WriteLine("no match");
			}
		}

		public void SubpatternCapture(object x)
		{
			if (x is Root { A.B: { I: 5, Text: "Hello" } b })
			{
				Console.WriteLine(b);
			}
			else
			{
				Console.WriteLine("no match");
			}
		}
	}
}
