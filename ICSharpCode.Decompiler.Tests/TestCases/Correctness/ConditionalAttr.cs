#define PRINT

using System;
using System.Diagnostics;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	class ConditionalAttr
	{
		[Conditional("PRINT")]
		static void Print()
		{
			Console.WriteLine("Text!");
		}

		static void Main()
		{
			Print();
		}
	}
}
