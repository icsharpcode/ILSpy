using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class PatternMatchingImprovements
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
			public double D { get; set; }
			public decimal Dec { get; set; }
			public Color Col { get; set; }
			public string Text { get; set; }
			public object Obj { get; set; }
			public X Next { get; set; }
		}

		public void RelationalSubpatternInt(object x)
		{
			if (x is X { I: > 5 } x2)
			{
				Console.WriteLine(x2.Text);
			}
		}

		public void RelationalSubpatternChar(object x)
		{
			if (x is X { Ch: >= 'a' } x2)
			{
				Console.WriteLine(x2.Text);
			}
		}

		public void RelationalSubpatternDouble(object x)
		{
			if (x is X { D: <= 2.5 } x2)
			{
				Console.WriteLine(x2.Text);
			}
		}

		public void RelationalSubpatternEnum(object x)
		{
			if (x is X { Col: > Color.Red } x2)
			{
				Console.WriteLine(x2.Text);
			}
		}

		public void RelationalSubpatternNested(object x)
		{
			if (x is X { Next: { I: < 10 } } x2)
			{
				Console.WriteLine(x2.Text);
			}
		}

		public void RelationalSubpatternTwoProperties(object x)
		{
			if (x is X { I: > 5, Ch: < 'x' } x2)
			{
				Console.WriteLine(x2.Text);
			}
		}

		public void NotStringConstant(object x)
		{
			if (x is X { Text: not "abc" } x2)
			{
				Console.WriteLine(x2.I);
			}
		}

		public void NotDecimalConstant(object x)
		{
			if (x is X { Dec: not 3.5m } x2)
			{
				Console.WriteLine(x2.Text);
			}
		}

		public bool TypeAndRelational(object x)
		{
			return x is int and > 5;
		}

		public bool TypeAndRelationalRange(object x)
		{
			return x is int and > 0 and < 10;
		}

		public bool SubpatternOrConstants(object x)
		{
			return x is X { I: 1 or 2 };
		}

		public bool SubpatternRelationalRange(object x)
		{
			return x is X { I: > 0 and < 10 };
		}

		public bool TypeOrType(object x)
		{
			return x is int or double;
		}

		public bool NullOrEmptyString(string s)
		{
			return s is null or "";
		}

		public string NegatedTypePatternWithDesignator(object x)
		{
			if (x is not string text)
			{
				return "no";
			}
			return text;
		}

		public bool NegatedRecursivePattern(object x)
		{
			return x is not X { I: 5 };
		}

		public string SwitchStatementRelational(int i)
		{
			switch (i)
			{
				case < 0:
					return "negative";
				case 0:
					return "zero";
				case > 100:
					return "big";
				default:
					return "small";
			}
		}

		public string SwitchStatementTypeAndRelational(object o)
		{
			switch (o)
			{
				case int and > 5:
					return "big int";
				case int:
					return "int";
				case string { Length: > 3 } text:
					return text;
				default:
					return "other";
			}
		}

		public string SwitchExpressionRelational(int i)
		{
			return i switch {
				< 0 => "negative",
				0 => "zero",
				> 100 => "big",
				_ => "small"
			};
		}

		public string SwitchExpressionCharRange(char c)
		{
			return c switch {
				>= 'a' and <= 'z' => "lower",
				>= 'A' and <= 'Z' => "upper",
				_ => "other"
			};
		}
	}
}
