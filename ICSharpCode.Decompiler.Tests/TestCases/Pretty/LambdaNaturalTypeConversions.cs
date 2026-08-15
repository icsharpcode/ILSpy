using System;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	// A lambda with a natural type needs no cast where the target type only asks for that
	// natural type: System.Delegate and the other base types and interfaces of
	// MulticastDelegate, or Expression and LambdaExpression for an expression tree. Under /o
	// the local's declared type is erased, so the natural type is written out instead.
	internal class LambdaNaturalTypeConversions
	{
		public Delegate ToDelegate()
		{
#if OPT
			Func<int, int> func = (int x) => x + 1;
			Console.WriteLine(func);
			return func;
#else
			Delegate obj = (int x) => x + 1;
			Console.WriteLine(obj);
			return obj;
#endif
		}

		public MulticastDelegate ToMulticastDelegate()
		{
#if OPT
			Func<int, int> func = (int x) => x + 2;
			Console.WriteLine(func);
			return func;
#else
			MulticastDelegate multicastDelegate = (int x) => x + 2;
			Console.WriteLine(multicastDelegate);
			return multicastDelegate;
#endif
		}

		public ISerializable ToDelegateInterface()
		{
#if OPT
			Func<int, int> func = (int x) => x + 3;
			Console.WriteLine(func);
			return func;
#else
			ISerializable serializable = (int x) => x + 3;
			Console.WriteLine(serializable);
			return serializable;
#endif
		}

		public object ToObject()
		{
#if OPT
			Func<int, int> func = (int x) => x + 4;
			Console.WriteLine(func);
			return func;
#else
			object obj = (int x) => x + 4;
			Console.WriteLine(obj);
			return obj;
#endif
		}

		public Expression ToExpression()
		{
#if OPT
			Expression<Func<int, int>> expression = (int x) => x + 5;
#else
			Expression expression = (int x) => x + 5;
#endif
			Console.WriteLine(expression);
			return expression;
		}

		public LambdaExpression ToLambdaExpression()
		{
#if OPT
			Expression<Func<int, int>> expression = (int x) => x + 6;
			Console.WriteLine(expression);
			return expression;
#else
			LambdaExpression lambdaExpression = (int x) => x + 6;
			Console.WriteLine(lambdaExpression);
			return lambdaExpression;
#endif
		}

		public Expression<Func<int, int>> ToExpressionOfDelegate()
		{
			// The tree's own type is written out: 'var' here would infer the delegate that the
			// Expression<> wraps, not the tree.
			Expression<Func<int, int>> expression = (int x) => x + 7;
			Console.WriteLine(expression);
			return expression;
		}
	}
}
