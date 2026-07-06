using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class PatternMatchingCombinators
	{
		public enum Color
		{
			Red,
			Green,
			Blue
		}

		public class X
		{
			public int I { get; set; }
			public char Ch { get; set; }
			public bool B { get; set; }
			public float F { get; set; }
			public double D { get; set; }
			public Color Col { get; set; }
			public string Text { get; set; }
			public object Obj { get; set; }
			public X Next { get; set; }
		}

		public void NotConstantInt(object x)
		{
			if (x is X { I: not 5 } x2)
			{
				Console.WriteLine(x2.Text);
			}
		}

		public void NotConstantChar(object x)
		{
			if (x is X { Ch: not 'a' } x2)
			{
				Console.WriteLine(x2.Text);
			}
		}

		public void NotConstantEnum(object x)
		{
			if (x is X { Col: not Color.Red } x2)
			{
				Console.WriteLine(x2.Text);
			}
		}

		public void NotConstantFloat(object x)
		{
			if (x is X { F: not 1.5f } x2)
			{
				Console.WriteLine(x2.Text);
			}
		}

		public void NotConstantDouble(object x)
		{
			if (x is X { D: not 2.5 } x2)
			{
				Console.WriteLine(x2.Text);
			}
		}

		public void NotConstantNested(object x)
		{
			if (x is X { Next: { I: not 5 } } x2)
			{
				Console.WriteLine(x2.Text);
			}
		}

		public void NotConstantAndNotNull(object x)
		{
			if (x is X { I: not 0, Obj: not null } x2)
			{
				Console.WriteLine(x2.Text);
			}
		}

		public void NotTruePrintsAsFalse(object x)
		{
#if EXPECTED_OUTPUT
			if (x is X { B: false } x2)
#else
			if (x is X { B: not true } x2)
#endif
			{
				Console.WriteLine(x2.Text);
			}
		}

		public bool IsNotNull(object x)
		{
#if EXPECTED_OUTPUT
			return x != null;
#else
			return x is not null;
#endif
		}

		public bool IsNotNullNullableInt(int? x)
		{
#if EXPECTED_OUTPUT
			return x.HasValue;
#else
			return x is not null;
#endif
		}

		public bool IsNotType(object x)
		{
#if EXPECTED_OUTPUT
			return !(x is string);
#else
			return x is not string;
#endif
		}

		public bool RelationalOnInt(int i)
		{
#if EXPECTED_OUTPUT
			return i > 5;
#else
			return i is > 5;
#endif
		}

		public bool RelationalOnEnum(Color c)
		{
#if EXPECTED_OUTPUT
			return c > Color.Red;
#else
			return c is > Color.Red;
#endif
		}

		public string OrPatternMergesToCaseLabels(int i)
		{
			switch (i)
			{
#if EXPECTED_OUTPUT
			case 1:
			case 2:
			case 3:
				return "small";
#else
				case 1 or 2 or 3:
					return "small";
#endif
				case 4:
				case 5:
					return "medium";
				default:
					return "other";
			}
		}
	}
}
