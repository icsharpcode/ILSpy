using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class MethodGroupSynthesizedDelegates
	{
		public static void RefParameter(ref int x)
		{
			x++;
		}

		public static int RefParameterWithReturn(ref int x)
		{
			return x + 1;
		}

		public static int InParameter(in int x)
		{
			return x;
		}

		public static bool OutParameter(out int x)
		{
			x = 42;
			return true;
		}

		public static void DefaultParameterValue(int x = 5)
		{
			Console.WriteLine(x);
		}

		public static void ParamsArray(params int[] xs)
		{
			Console.WriteLine(xs.Length);
		}

		public object UseRefParameter()
		{
			int x = 0;
			var f = RefParameter;
			f(ref x);
			Console.WriteLine(x);
			return f;
		}

		public object UseRefParameterWithReturn()
		{
			int x = 1;
			var f = RefParameterWithReturn;
			Console.WriteLine(f(ref x));
			return f;
		}

		public object UseInParameter()
		{
			int x = 2;
			var f = InParameter;
			Console.WriteLine(f(in x));
			return f;
		}

		public object UseOutParameter()
		{
			var f = OutParameter;
			if (f(out var x))
			{
				Console.WriteLine(x);
			}
			return f;
		}

		public object UseDefaultParameterValue()
		{
			var f = DefaultParameterValue;
			f();
			return f;
		}

		public object UseParamsArray()
		{
			var f = ParamsArray;
			f(1, 2, 3);
			return f;
		}
	}
}
