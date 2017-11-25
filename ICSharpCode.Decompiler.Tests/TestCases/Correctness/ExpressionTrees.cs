using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	class ExpressionTrees
	{
		static void Main()
		{
			Test();
			var twice = GetExpression(Expression.Constant(2)).Compile();
			Console.WriteLine(twice(21));
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
	}
}
