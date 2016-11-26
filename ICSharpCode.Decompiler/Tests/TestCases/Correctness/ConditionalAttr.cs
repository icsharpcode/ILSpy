#define PRINT

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
