using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

		public class WithStructEnumerator
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct StructEnumerator
		{
			public int Current => 0;

			public bool MoveNext()
			{
				return false;
			}
		}

		public void Test(NonGeneric c)
		{
			foreach (object item in c)
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

		public void Test(WithStructEnumerator c)
		{
			foreach (int item in c)
			{
				Console.WriteLine(item);
			}
		}
#if !NET40
		public async void TestAsync(Generic<int> c)
		{
			await foreach (int item in c)
			{
				Console.WriteLine(item);
			}
		}
#endif
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
		public static CS9_ExtensionGetEnumerator.StructEnumerator GetEnumerator(this CS9_ExtensionGetEnumerator.WithStructEnumerator c)
		{
			return default(CS9_ExtensionGetEnumerator.StructEnumerator);
		}
#if !NET40
		public static IAsyncEnumerator<T> GetAsyncEnumerator<T>(this CS9_ExtensionGetEnumerator.Generic<T> c)
		{
			throw null;
		}
#endif
	}
}