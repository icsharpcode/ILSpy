using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class MethodGroupNaturalType
	{
		private object anonymousDelegate;

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

		private static void SixteenParameters(int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, int a9, int a10, int a11, int a12, int a13, int a14, int a15, int a16)
		{
		}

		private static int SeventeenParameters(int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, int a9, int a10, int a11, int a12, int a13, int a14, int a15, int a16, int a17)
		{
			return a1;
		}

		private static void SeventeenParametersVoid(int a1, int a2, int a3, int a4, int a5, int a6, int a7, int a8, int a9, int a10, int a11, int a12, int a13, int a14, int a15, int a16, int a17)
		{
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

		public object ArityWithinActionFamily()
		{
			var result = SixteenParameters;
			Console.WriteLine("no inlining");
			return result;
		}

		public object ArityBeyondFuncFamily()
		{
			var result = SeventeenParameters;
			Console.WriteLine("no inlining");
			return result;
		}

		public object ArityBeyondActionFamily()
		{
			var result = SeventeenParametersVoid;
			Console.WriteLine("no inlining");
			return result;
		}

		public void AssignedToObjectField()
		{
			anonymousDelegate = (ref int x) => x;
		}

		public object ReturnedAsObject()
		{
			return (ref int x) => x;
		}

		public Delegate ReturnedAsDelegate()
		{
			return RefFunc;
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

	internal class MethodGroups
	{
		public static int Square(int x)
		{
			return x * x;
		}

		public object InstanceBeforeExtensionScope()
		{
			var result = new Receiver().Basic;
			Console.WriteLine("no inlining");
			return result;
		}

		public object PruneByGenericArity()
		{
			var result = new Receiver().ByArity<object>;
			Console.WriteLine("no inlining");
			return result;
		}

		public object PruneByConstraint()
		{
			var result = new Receiver().ByConstraint<int>;
			Console.WriteLine("no inlining");
			return result;
		}

		public object ExtensionScopeOnly()
		{
			var result = new Receiver().ExtensionOnly;
			Console.WriteLine("no inlining");
			return result;
		}

		public object PruneStaticOnInstanceReceiver()
		{
			var result = new MixedStaticInstance().Mixed;
			Console.WriteLine("no inlining");
			return result;
		}

		public object PruneInstanceOnTypeReceiver()
		{
			var result = MixedStaticInstance.Mixed;
			Console.WriteLine("no inlining");
			return result;
		}

		public object NaturalTypeAssignedToDelegate()
		{
#if OPT
			var result = Square;
#else
			Delegate result = Square;
#endif
			Console.WriteLine("no inlining");
			return result;
		}

		public object NaturalTypeAssignedToObject()
		{
#if OPT
			var result = Square;
#else
#pragma warning disable CS8974 // Converting method group to non-delegate type
			object result = Square;
#pragma warning restore CS8974
#endif
			Console.WriteLine("no inlining");
			return result;
		}

		public int InvokeInferredMethodGroup()
		{
			var func = Square;
			Console.WriteLine("no inlining");
			return func(21);
		}
	}

	internal class MixedStaticInstance
	{
		public void Mixed()
		{
		}

		public static void Mixed(int i)
		{
		}
	}

	internal class Receiver
	{
		public void Basic()
		{
		}

		public void ByArity<T>()
		{
		}

		public void ByArity(int i)
		{
		}

		public void ByConstraint<T>(T t) where T : class
		{
		}
	}

	internal static class ReceiverExtensions
	{
		public static void Basic(this Receiver r, int i)
		{
		}

		public static void ByConstraint<T>(this Receiver r, T t)
		{
		}

		public static void ExtensionOnly(this Receiver r, int i)
		{
		}
	}
}
