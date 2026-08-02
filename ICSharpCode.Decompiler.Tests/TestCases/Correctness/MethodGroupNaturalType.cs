using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	// Exercises the C# 13 "method group natural type improvements": candidates are
	// pruned scope-by-scope (instance before extension scopes), and candidates with
	// mismatched generic arity, violated constraints or a static/instance mismatch
	// are pruned early. The interesting part is the second compilation pass: the
	// decompiled output must re-bind every method group to the same target method.
	internal class MethodGroupNaturalType
	{
		public class Receiver
		{
			public void Basic()
			{
				Console.WriteLine("instance Basic()");
			}

			public void ByArity<T>()
			{
				Console.WriteLine("instance ByArity<T>()");
			}

			public void ByArity(int i)
			{
				Console.WriteLine("instance ByArity(int)");
			}

			public void ByConstraint<T>(T t) where T : class
			{
				Console.WriteLine("instance ByConstraint<T>(T) where T : class");
			}
		}

		public class MixedStaticInstance
		{
			public void Mixed()
			{
				Console.WriteLine("instance Mixed()");
			}

			public static void Mixed(int i)
			{
				Console.WriteLine("static Mixed(int)");
			}
		}

		private static int Square(int x)
		{
			return x * x;
		}

		private static void Main()
		{
			var instanceBeforeExtension = new Receiver().Basic;
			instanceBeforeExtension();

			var pruneByArity = new Receiver().ByArity<object>;
			pruneByArity();

			var pruneByConstraint = new Receiver().ByConstraint<int>;
			pruneByConstraint(1);

			var extensionOnly = new Receiver().ExtensionOnly;
			extensionOnly(2);

			var pruneStatic = new MixedStaticInstance().Mixed;
			pruneStatic();

			var pruneInstance = MixedStaticInstance.Mixed;
			pruneInstance(3);

			Delegate viaDelegate = Square;
			Console.WriteLine(((Func<int, int>)viaDelegate)(4));

#pragma warning disable CS8974 // Converting method group to non-delegate type
			object viaObject = Square;
#pragma warning restore CS8974
			Console.WriteLine(((Func<int, int>)viaObject)(5));

			var invokeInferred = Square;
			Console.WriteLine(invokeInferred(6));
		}
	}

	internal static class MethodGroupNaturalTypeExtensions
	{
		public static void Basic(this MethodGroupNaturalType.Receiver r, int i)
		{
			Console.WriteLine("extension Basic(int)");
		}

		public static void ByConstraint<T>(this MethodGroupNaturalType.Receiver r, T t)
		{
			Console.WriteLine("extension ByConstraint<T>(T)");
		}

		public static void ExtensionOnly(this MethodGroupNaturalType.Receiver r, int i)
		{
			Console.WriteLine("extension ExtensionOnly(int)");
		}
	}
}
