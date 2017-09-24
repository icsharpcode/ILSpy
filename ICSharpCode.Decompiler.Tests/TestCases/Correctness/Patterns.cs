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
	static class Patterns
	{
		#region Test Helpers
		class PrintOnDispose : IDisposable
		{
			private string v;

			public PrintOnDispose(string v)
			{
				this.v = v;
			}

			public void Dispose()
			{
				Console.WriteLine(this.v);
			}
		}
		#endregion

		static void Main()
		{
			SimpleUsingNullStatement();
			NoUsing();
			NoUsing2();
			ForWithMultipleVariables();
			DoubleForEachWithSameVariable(new[] { "a", "b", "c" });
			ForeachExceptForNameCollision(new[] { 42, 43, 44, 45 });
		}

		#region Using
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

		public static void NoUsing()
		{
			PrintOnDispose printOnDispose = new PrintOnDispose("Wrong");
			try {
				printOnDispose = new PrintOnDispose("Correct");
			} finally {
				printOnDispose.Dispose();
			}
		}

		public static void NoUsing2()
		{
			object printOnDispose = new PrintOnDispose("NoUsing(): Wrong");
			try {
				printOnDispose = new PrintOnDispose("NoUsing(): Correct");
			} finally {
				IDisposable disposable = printOnDispose as IDisposable;
				if (disposable != null) {
					disposable.Dispose();
				}
			}
		}
		#endregion

		#region Loops
		public static void ForWithMultipleVariables()
		{
			int x, y;
			Console.WriteLine("before for");
			for (x = y = 0; x < 10; x++) {
				y++;
				Console.WriteLine("x = " + x + ", y = " + y);
			}
			Console.WriteLine("after for");
		}

		public static void DoubleForEachWithSameVariable(IEnumerable<string> enumerable)
		{
			Console.WriteLine("DoubleForEachWithSameVariable:");
			foreach (string current in enumerable) {
				Console.WriteLine(current.ToLower());
			}
			Console.WriteLine("after first loop");
			foreach (string current in enumerable) {
				Console.WriteLine(current.ToUpper());
			}
			Console.WriteLine("after second loop");
		}

		public static void ForeachExceptForNameCollision(IEnumerable<int> inputs)
		{
			Console.WriteLine("ForeachWithNameCollision:");
			int current;
			using (IEnumerator<int> enumerator = inputs.GetEnumerator()) {
				while (enumerator.MoveNext()) {
					current = enumerator.Current;
					Console.WriteLine(current);
				}
			}
			current = 1;
			Console.WriteLine(current);
		}
		#endregion
	}
}
