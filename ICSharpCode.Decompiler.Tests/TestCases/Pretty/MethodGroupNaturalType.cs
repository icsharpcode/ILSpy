using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class MethodGroupNaturalType
	{
		private static void RefAction(ref int x)
		{
		}

		private static int RefFunc(ref int x)
		{
			return x;
		}

		private static ref int RefReturn(int[] xs)
		{
			return ref xs[0];
		}

		private static T GenericRef<T>(ref T value)
		{
			return value;
		}

		private static void ParamsMethod(params int[] xs)
		{
		}

		private static int DefaultValueMethod(ref int x, int y = 42)
		{
			return x + y;
		}

		private int InstanceRef(ref int x)
		{
			return x;
		}

		public object VoidDelegateFamily()
		{
			var result = RefAction;
			Console.WriteLine("no inlining");
			return result;
		}

		public int ValueDelegateFamily()
		{
			var anon = RefFunc;
			Console.WriteLine("no inlining");
			int arg = 21;
			return anon(ref arg);
		}

		public int RefReturnDelegateFamily()
		{
			var anon = RefReturn;
			Console.WriteLine("no inlining");
			int[] array = new int[1];
			anon(array) = 5;
			return array[0];
		}

		public object ImplicitThisReceiver()
		{
			var result = InstanceRef;
			Console.WriteLine("no inlining");
			return result;
		}

		public object ExpressionReceiver()
		{
			var result = new MethodGroupNaturalType().InstanceRef;
			Console.WriteLine("no inlining");
			return result;
		}

		public object ExplicitTypeArguments()
		{
			var result = GenericRef<string>;
			Console.WriteLine("no inlining");
			return result;
		}

		public object ParamsParameter()
		{
			var result = ParamsMethod;
			Console.WriteLine("no inlining");
			return result;
		}

		public int DefaultParameterValue()
		{
			var anon = DefaultValueMethod;
			Console.WriteLine("no inlining");
			int arg = 0;
			return anon(ref arg);
		}

		public object CapturedMethodGroup()
		{
			var anon = RefAction;
			Console.WriteLine("no inlining");
			Func<string> result = () => anon.ToString();
			Console.WriteLine("no inlining");
			return result;
		}
	}
}
