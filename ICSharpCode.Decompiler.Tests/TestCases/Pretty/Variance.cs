using System;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class ExplicitVariantImpl : IVariant<object, string>
	{
		string IVariant<object, string>.Convert(object value)
		{
			return null;
		}
	}

	public interface IConstrainedCovariant<out T> where T : class
	{
		T Get();
	}

	public interface IContravariant<in T>
	{
		void Set(T value);
	}

	public interface ICovariant<out T>
	{
		T Get();
	}

	public interface IVariant<in TIn, out TOut>
	{
		TOut Convert(TIn value);
	}

	public static class VarianceConversions
	{
		public static IEnumerable<object> Widen(IEnumerable<string> xs)
		{
			return xs;
		}

		public static Action<string> Narrow(Action<object> a)
		{
			return a;
		}

		public static Func<object> ToObjectFunc(Func<string> f)
		{
			return f;
		}

		public static ICovariant<object> WidenInterface(ICovariant<string> c)
		{
			return c;
		}

		public static IContravariant<string> NarrowInterface(IContravariant<object> c)
		{
			return c;
		}

		public static VariantFunc<string, object> ConvertDelegate(VariantFunc<object, string> f)
		{
			return f;
		}

		public static void ArgumentConversion(ICovariant<string> c, IContravariant<object> c2)
		{
			UseCovariant(c);
			UseContravariant(c2);
		}

		private static void UseCovariant(ICovariant<object> c)
		{
		}

		private static void UseContravariant(IContravariant<string> c)
		{
		}
	}

	public delegate TResult VariantFunc<in TArg, out TResult>(TArg arg);
}
