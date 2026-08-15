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
			int arg = 0;
			var anon = RefParameter;
			anon(ref arg);
			Console.WriteLine(arg);
			return anon;
		}

		public object UseRefParameterWithReturn()
		{
			int arg = 1;
			var anon = RefParameterWithReturn;
			Console.WriteLine(anon(ref arg));
			return anon;
		}

		public object UseInParameter()
		{
			int arg = 2;
			var anon = InParameter;
			Console.WriteLine(anon(in arg));
			return anon;
		}

		public object UseOutParameter()
		{
			var anon = OutParameter;
			if (anon(out var arg))
			{
				Console.WriteLine(arg);
			}
			return anon;
		}

		public object UseDefaultParameterValue()
		{
			var anon = DefaultParameterValue;
			anon();
			return anon;
		}

		public object UseParamsArray()
		{
			var anon = ParamsArray;
			anon(1, 2, 3);
			return anon;
		}
	}
}
