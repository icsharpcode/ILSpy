using System;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class CS9_ExtensionGetEnumerator
	{
		public class NonGeneric
		{
		}

		public class Generic<T>
		{
		}

		public void Test(NonGeneric c)
		{
			foreach (object? item in c)
			{
				Console.WriteLine(item);
			}
		}

		public void Test(Generic<int> c)
		{
			foreach (int item in c)
			{
				Console.WriteLine(item);
			}
		}

		public async void TestAsync(Generic<int> c)
		{
			await foreach (int item in c)
			{
				Console.WriteLine(item);
			}
		}
	}

	public static class CS9_ExtensionGetEnumerator_Ext
	{
		public static IEnumerator GetEnumerator(this CS9_ExtensionGetEnumerator.NonGeneric c)
		{
			throw null;
		}
		public static IEnumerator<T> GetEnumerator<T>(this CS9_ExtensionGetEnumerator.Generic<T> c)
		{
			throw null;
		}
		public static IAsyncEnumerator<T> GetAsyncEnumerator<T>(this CS9_ExtensionGetEnumerator.Generic<T> c)
		{
			throw null;
		}
	}
}