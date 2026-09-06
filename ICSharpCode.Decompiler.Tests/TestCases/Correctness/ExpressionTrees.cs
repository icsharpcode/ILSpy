// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Linq.Expressions;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	class ExpressionTrees
	{
		static void Main()
		{
			Test();
			var twice = GetExpression(Expression.Constant(2)).Compile();
			Console.WriteLine(twice(21));
			TypeParameterComparisons();
			ValueTypeComparisons();
		}

		static void Test()
		{
			int i = 0;
			Expression<Func<int>> expression = () => i;
			i = 1;
			Console.WriteLine(expression.Compile()());
		}

		static Expression<Func<int, int>> GetExpression(Expression factor)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(int), "x");
			return Expression.Lambda<Func<int, int>>(Expression.Multiply(parameterExpression, factor), parameterExpression);
		}

		// A comparison of two type parameters, which no lambda can spell: `v == other` is CS0019
		// for a type parameter, and boxing both operands compiles but compares box identity,
		// where Equal on two T compares values once T is a value type. The int instantiation
		// below is what tells those two apart.
		static Expression<Func<T, bool>> EqualsValue<T>(T other)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "v");
			return Expression.Lambda<Func<T, bool>>(
				Expression.Equal(parameterExpression, Expression.Constant(other, typeof(T))), parameterExpression);
		}

		static Expression<Func<T, bool>> NotEqualsValue<T>(T other)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(T), "v");
			return Expression.Lambda<Func<T, bool>>(
				Expression.NotEqual(parameterExpression, Expression.Constant(other, typeof(T))), parameterExpression);
		}

		static void TypeParameterComparisons()
		{
			object a = new object();
			object b = new object();
			Console.WriteLine(EqualsValue(a).Compile()(a));
			Console.WriteLine(EqualsValue(a).Compile()(b));
			Console.WriteLine(NotEqualsValue(a).Compile()(a));
			Console.WriteLine(NotEqualsValue(a).Compile()(b));
			Console.WriteLine(EqualsValue(42).Compile()(42));
			Console.WriteLine(EqualsValue(42).Compile()(43));
		}

		// The same shape on value types that are not type parameters: decimal and a nullable
		// both compare as themselves, and boxing either would turn the comparison into a
		// reference comparison of two boxes.
		static Expression<Func<decimal, bool>> GreaterThanDecimal(decimal limit)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(decimal), "v");
			return Expression.Lambda<Func<decimal, bool>>(
				Expression.GreaterThan(parameterExpression, Expression.Constant(limit, typeof(decimal))), parameterExpression);
		}

		static Expression<Func<int?, bool>> EqualsNullable(int? other)
		{
			ParameterExpression parameterExpression = Expression.Parameter(typeof(int?), "v");
			return Expression.Lambda<Func<int?, bool>>(
				Expression.Equal(parameterExpression, Expression.Constant(other, typeof(int?))), parameterExpression);
		}

		static void ValueTypeComparisons()
		{
			Console.WriteLine(GreaterThanDecimal(1m).Compile()(2m));
			Console.WriteLine(GreaterThanDecimal(1m).Compile()(0m));
			Console.WriteLine(EqualsNullable(1).Compile()(1));
			Console.WriteLine(EqualsNullable(1).Compile()(2));
			Console.WriteLine(EqualsNullable(1).Compile()(null));
			Console.WriteLine(EqualsNullable(null).Compile()(null));
		}
	}
}
