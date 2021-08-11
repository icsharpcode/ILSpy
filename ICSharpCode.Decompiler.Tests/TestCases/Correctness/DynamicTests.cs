using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	class DynamicTests
	{
		delegate void RefAction<T>(ref T arg);

		static void Main(string[] args)
		{
			PrintResult((ref dynamic x) => x = x + 2.0, 5.0);
			PrintResult((ref dynamic x) => x = x + 2.0, 5);
			PrintResult((ref dynamic x) => x = x + 2, 5.0);
			PrintResult((ref dynamic x) => x = x + 2, 5);
			PrintResult((ref dynamic x) => x = x - 2.0, 5.0);
			PrintResult((ref dynamic x) => x = x - 2.0, 5);
			PrintResult((ref dynamic x) => x = x - 2, 5.0);
			PrintResult((ref dynamic x) => x = x - 2, 5);
			PrintResult((ref dynamic x) => x = x * 2.0, 5.0);
			PrintResult((ref dynamic x) => x = x * 2.0, 5);
			PrintResult((ref dynamic x) => x = x * 2, 5.0);
			PrintResult((ref dynamic x) => x = x * 2, 5);
			PrintResult((ref dynamic x) => x = x / 2.0, 5.0);
			PrintResult((ref dynamic x) => x = x / 2.0, 5);
			PrintResult((ref dynamic x) => x = x / 2, 5.0);
			PrintResult((ref dynamic x) => x = x / 2, 5);
		}

		private static void PrintResult(RefAction<dynamic> p, dynamic arg)
		{
			p(ref arg);
			Console.WriteLine(arg);
		}
	}
}
