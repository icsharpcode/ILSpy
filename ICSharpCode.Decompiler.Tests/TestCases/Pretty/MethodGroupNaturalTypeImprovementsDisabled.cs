using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty.MethodGroupNaturalTypeImprovementsDisabled
{
	internal class MethodGroups
	{
		public static int Square(int x)
		{
			return x * x;
		}

		public object InstanceBeforeExtensionScope()
		{
#if EXPECTED_OUTPUT
			Action result = new Receiver().Basic;
#else
			var result = new Receiver().Basic;
#endif
			Console.WriteLine("no inlining");
			return result;
		}

		public object PruneByGenericArity()
		{
#if EXPECTED_OUTPUT
			Action result = new Receiver().ByArity<object>;
#else
			var result = new Receiver().ByArity<object>;
#endif
			Console.WriteLine("no inlining");
			return result;
		}

		public object PruneByConstraint()
		{
#if EXPECTED_OUTPUT
			Action<int> result = new Receiver().ByConstraint<int>;
#else
			var result = new Receiver().ByConstraint<int>;
#endif
			Console.WriteLine("no inlining");
			return result;
		}

		public object ExtensionScopeOnly()
		{
			// A unique extension method already provides the natural type under the C# 10 rules.
			var result = new Receiver().ExtensionOnly;
			Console.WriteLine("no inlining");
			return result;
		}

		public object PruneStaticOnInstanceReceiver()
		{
#if EXPECTED_OUTPUT
			Action result = new MixedStaticInstance().Mixed;
#else
			var result = new MixedStaticInstance().Mixed;
#endif
			Console.WriteLine("no inlining");
			return result;
		}

		public object PruneInstanceOnTypeReceiver()
		{
#if EXPECTED_OUTPUT
			Action<int> result = MixedStaticInstance.Mixed;
#else
			var result = MixedStaticInstance.Mixed;
#endif
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
