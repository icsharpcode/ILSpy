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
		}

		public Expression<Func<int, int>> GetExpression(int i)
		{
			return a => a + i;
		}

		public Expression<Func<int, Func<int, int>>> GetExpression2()
		{
			return a => b => a + b;
		}

		public static void Test()
		{
			int i = 0;
			Expression<Func<int>> expression = () => i;
			i = 1;
			Console.WriteLine(expression.Compile()());
		}
	}
}
