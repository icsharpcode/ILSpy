using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class StringInterpolationDefaultHandler
	{
		public string Basic(int x, int y)
		{
			return $"{x}{y}";
		}

		public string WithLiterals(int x, string s)
		{
			return $"x = {x}, s = {s}!";
		}

		public string LongOne(int a, int b, int c, int d, int e)
		{
			return $"a={a} b={b} c={c} d={d} e={e}";
		}

		public string AlignmentAndFormat(double d, int i)
		{
			return $"{d,5:N2} and {i:x8} and {i,-10}";
		}

		public string NestedInterpolation(int x, int y)
		{
			return $"outer {$"inner {x}"} end {y}";
		}

		public string TernaryHole(bool cond, int a, int b)
		{
			return $"value: {(cond ? a : b)}";
		}

		public string CharAndBool(char c, bool b)
		{
			return $"{c}{b}";
		}

		public string StringWithAlignment(string s)
		{
			return $"{s,10}|{s,-10}";
		}

		public string SpanValue(ReadOnlySpan<char> span)
		{
			return $"span: {span} end";
		}

		public string GenericValue<T>(T value)
		{
			return $"generic {value} end";
		}

		public string GenericValueWithFormat<T>(T value)
		{
			return $"generic {value:X4} end";
		}

		public string Objects(object o, IFormattable f)
		{
			return $"{o} {f}";
		}

		public string NullableValue(int? x)
		{
			return $"nullable {x} end";
		}

		public string DecimalAndDate(decimal m, DateTime dt)
		{
			return $"{m:C} {dt:yyyy-MM-dd}";
		}

		public string EscapesAndBraces(int x)
		{
			return $"{{v}} = {x}, tab\t {x:x}";
		}

		public string ConstantHoles(int x)
		{
			return $"{"p_"}{x} {"ConstantHoles"}";
		}

		public string ConstantIntHole()
		{
			return $"const {42} part";
		}

		public string InConcat(int x, int y)
		{
			return "a" + $"{x}b{y}" + "c";
		}

		public string InTernaryBranches(bool cond, int x, int y)
		{
#if EXPECTED_OUTPUT
			if (!cond)
			{
				return $"no {y}";
			}
			return $"yes {x}";
#else
			if (cond)
			{
				return $"yes {x}";
			}
			return $"no {y}";
#endif
		}
	}
}
