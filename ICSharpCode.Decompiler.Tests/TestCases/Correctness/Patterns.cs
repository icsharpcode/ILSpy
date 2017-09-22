using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	/// <summary>
	/// This file contains special cases of some patterns that cannot be tested in pretty tests.
	/// </summary>
	class Patterns
	{
		static void Main()
		{
			SimpleUsingNullStatement();
		}

		/// <summary>
		/// Special case: Roslyn eliminates the try-finally altogether.
		/// </summary>
		public static void SimpleUsingNullStatement()
		{
			Console.WriteLine("before using");
			using (null) {
				Console.WriteLine("using (null)");
			}
			Console.WriteLine("after using");
		}
	}
}
