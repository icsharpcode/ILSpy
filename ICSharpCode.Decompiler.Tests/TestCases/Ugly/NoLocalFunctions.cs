using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	public sealed class Handle
	{
		private readonly Func<int> _func;
		public Handle(Func<int> func)
		{
			_func = func;
		}
	}

	public static class NoLocalFunctions
	{
		private static void UseLocalFunctionReference()
		{
			int F() => 42;
			var handle = new Handle(F);
		}
		private static void SimpleCapture()
		{
			int x = 1;
			int F() => 42 + x;
			F();
		}
		private static void SimpleCaptureWithRef()
		{
			int x = 1;
			int F() => 42 + x;
			var handle = new Handle(F);
		}
	}
}
