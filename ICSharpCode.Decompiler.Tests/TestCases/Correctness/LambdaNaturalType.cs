using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	// A natural-typed lambda decides which overload a later call binds to: the variable's
	// type is the lambda's natural type, so a lost explicit return type or a dropped
	// parameter modifier silently rebinds the call instead of failing to compile. The
	// interesting part is the second compilation pass: the decompiled output must produce
	// the same natural types, and therefore reach the same overloads.
	internal class LambdaNaturalType
	{
		private static void Consume(Func<int, int> f)
		{
			Console.WriteLine("Func<int, int>: " + f(1));
		}

		private static void Consume(Func<int, long> f)
		{
			Console.WriteLine("Func<int, long>: " + f(2));
		}

		private static void Consume(Action<int> a)
		{
			Console.Write("Action<int>: ");
			a(3);
		}

		private static void Consume(Delegate d)
		{
			// The natural type of a lambda with a ref parameter is a synthesized delegate,
			// which no Func/Action overload accepts. Its name is compiler-generated, so
			// only its shape may be printed.
			Console.WriteLine("Delegate: " + d.Method.GetParameters().Length + " parameter(s)");
		}

		private static void Generic<T>(T value)
		{
			Console.WriteLine("Generic<" + typeof(T) + ">");
		}

		private static int SideEffect(int x)
		{
			Console.WriteLine("SideEffect(" + x + ")");
			return x;
		}

		private static void Main()
		{
			var inferred = (int x) => x + 1;
			Consume(inferred);
			Generic(inferred);

			var widened = long (int x) => x + 2;
			Consume(widened);
			Generic(widened);

			var discardedResult = void (int x) => SideEffect(x);
			Consume(discardedResult);

			var refParameter = (ref int x) => ++x;
			Consume(refParameter);
			int arg = 10;
			refParameter(ref arg);
			Console.WriteLine("ref parameter: " + arg);

			var refReadonlyResult = ref readonly int (ref int x) => ref x;
			int slot = 20;
			Console.WriteLine("ref readonly result: " + refReadonlyResult(ref slot));
			// 'ref readonly' lives in the synthesized delegate's signature as a modreq, not in
			// anything the invocation can observe, so read it back off the delegate type.
			Console.WriteLine("ref readonly modreqs: "
				+ refReadonlyResult.GetType().GetMethod("Invoke").ReturnParameter.GetRequiredCustomModifiers().Length);

			var optionalParameter = (int x = 5) => x * 2;
			Console.WriteLine("optional parameter: " + optionalParameter() + " " + optionalParameter(7));

#if CS140
			// Roslyn 4.14 emits no ParamArrayAttribute on a lambda's method, so 'params' cannot
			// be recovered there and the decompiled call would no longer bind in expanded form.
			var paramsParameter = (params int[] xs) => xs.Length;
			Console.WriteLine("params parameter: " + paramsParameter(1, 2, 3));
#endif
		}
	}
}
