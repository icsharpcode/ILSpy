// Copyright (c) AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	class Loops
	{
		public class CustomClassEnumeratorWithIDisposable<T> : IDisposable
		{
			bool next = true;

			public T Current {
				get {
					return default(T);
				}
			}

			public void Dispose()
			{
				Console.WriteLine("CustomClassEnumeratorWithIDisposable<T>.Dispose()");
			}

			public bool MoveNext()
			{
				if (next) {
					next = false;
					return true;
				}
				return next;
			}

			public CustomClassEnumeratorWithIDisposable<T> GetEnumerator()
			{
				return this;
			}
		}

		static void Operation(ref int item)
		{
			item++;
		}

		static T CallWithSideEffect<T>()
		{
			Console.WriteLine("CallWithSideEffect");
			return default(T);
		}

		static void Main()
		{
			ForWithMultipleVariables();
			DoubleForEachWithSameVariable(new[] { "a", "b", "c" });
			ForeachExceptForNameCollision(new[] { 42, 43, 44, 45 });
			ForeachExceptForContinuedUse(new[] { 42, 43, 44, 45 });
			NonGenericForeachWithReturnFallbackTest(new object[] { "a", 42, "b", 43 });
			NonGenericForeachWithReturn(new object[] { "a", 42, "b", 43 });
			ForeachWithReturn(new[] { 42, 43, 44, 45 });
			ForeachWithRefUsage(new List<int> { 1, 2, 3, 4, 5 });
			Console.WriteLine(FirstOrDefault(new List<int> { 1, 2, 3, 4, 5 }));
			Console.WriteLine(NoForeachDueToMultipleCurrentAccess(new List<int> { 1, 2, 3, 4, 5 }));
			Console.WriteLine(NoForeachCallWithSideEffect(new CustomClassEnumeratorWithIDisposable<int>()));
			LoopWithGotoRepeat();
			Console.WriteLine("LoopFollowedByIf: {0}", LoopFollowedByIf());
			NoForeachDueToVariableAssignment();
		}

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

		public static void ForeachExceptForContinuedUse(IEnumerable<int> inputs)
		{
			Console.WriteLine("ForeachExceptForContinuedUse");
			int num = 0;
			using (IEnumerator<int> enumerator = inputs.GetEnumerator()) {
				while (enumerator.MoveNext()) {
					num = enumerator.Current;
					Console.WriteLine(num);
				}
			}
			Console.WriteLine("Last: " + num);
		}

		public static void NonGenericForeachWithReturnFallbackTest(IEnumerable e)
		{
			Console.WriteLine("NonGenericForeachWithReturnFallback:");
			IEnumerator enumerator = e.GetEnumerator();
			try {
				Console.WriteLine("MoveNext");
				if (enumerator.MoveNext()) {
					object current = enumerator.Current;
					Console.WriteLine("current: " + current);
				}
			} finally {
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null) {
					disposable.Dispose();
				}
			}
			Console.WriteLine("After finally!");
		}

		public static object NonGenericForeachWithReturn(IEnumerable enumerable)
		{
			Console.WriteLine("NonGenericForeachWithReturn:");
			foreach (var obj in enumerable) {
				Console.WriteLine("return: " + obj);
				return obj;
			}

			Console.WriteLine("return: null");
			return null;
		}

		public static int? ForeachWithReturn(IEnumerable<int> enumerable)
		{
			Console.WriteLine("ForeachWithReturn:");
			foreach (var obj in enumerable) {
				Console.WriteLine("return: " + obj);
				return obj;
			}

			Console.WriteLine("return: null");
			return null;
		}

		public static void ForeachWithRefUsage(List<int> items)
		{
			Console.WriteLine("ForeachWithRefUsage:");
			foreach (var item in items) {
				var itemToChange = item;
				Console.WriteLine("item: " + item);
				Operation(ref itemToChange);
				Console.WriteLine("item: " + itemToChange);
			}
		}

		public static T FirstOrDefault<T>(IEnumerable<T> items)
		{
			T result = default(T);
			foreach (T item in items) {
				result = item;
				break;
			}
			return result;
		}

		public static T NoForeachDueToMultipleCurrentAccess<T>(IEnumerable<T> items)
		{
			Console.WriteLine("NoForeachDueToMultipleCurrentAccess:");
			T result = default(T);
			using (IEnumerator<T> enumerator = items.GetEnumerator()) {
				while (enumerator.MoveNext()) {
					result = enumerator.Current;
					Console.WriteLine("result: " + result);
				}
				return enumerator.Current;
			}
		}

		public static T NoForeachCallWithSideEffect<T>(CustomClassEnumeratorWithIDisposable<T> items)
		{
			Console.WriteLine("NoForeachCallWithSideEffect:");
			using (CustomClassEnumeratorWithIDisposable<T> enumerator = items.GetEnumerator()) {
				while (enumerator.MoveNext()) {
					T result = enumerator.Current;
				}
				return CallWithSideEffect<T>();
			}
		}
		
		static bool GetBool(string text)
		{
			return false;
		}

		// https://github.com/icsharpcode/ILSpy/issues/915
		static void LoopWithGotoRepeat()
		{
			Console.WriteLine("LoopWithGotoRepeat:");
			try {
				REPEAT:
				Console.WriteLine("after repeat label");
				while (GetBool("Loop condition")) {
					if (GetBool("if1")) {
						if (GetBool("if3")) {
							goto REPEAT;
						}
						break;
					}
				}
				Console.WriteLine("after loop");
			} finally {
				Console.WriteLine("finally");
			}
			Console.WriteLine("after finally");
		}
		
		private static int LoopFollowedByIf()
		{
			int num = 0;
			while (num == 0) {
				num++;
			}
			if (num == 0) {
				return -1;
			}
			return num;
		}

		static void Issue1392ForWithNestedSwitchPlusGoto()
		{
			for (int i = 0; i < 100; i++) {
				again:
				switch (i) {
					case 10:
						Console.WriteLine("10");
						break;
					case 25:
						Console.WriteLine("25");
						break;
					case 50:
						Console.WriteLine("50");
						goto again;
				}
			}
		}

		private static void NoForeachDueToVariableAssignment()
		{
			try {
				int[] array = new int[] { 1, 2, 3 };
				for (int i = 0; i < array.Length; i++) {
					Console.WriteLine(array[i]);
					array = null;
				}
			} catch (Exception ex) {
				Console.WriteLine(ex.GetType() + ": " + ex.Message);
			}
		}
	}
}
