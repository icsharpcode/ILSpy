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
using System.Runtime.InteropServices;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class Loops
	{
		#region foreach
		public class CustomClassEnumerator
		{
			public object Current {
				get {
					throw new NotImplementedException();
				}
			}

			public bool MoveNext()
			{
				throw new NotImplementedException();
			}

			public void Reset()
			{
				throw new NotImplementedException();
			}

			public CustomClassEnumerator GetEnumerator()
			{
				return this;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CustomStructEnumerator
		{
			public object Current {
				get {
					throw new NotImplementedException();
				}
			}

			public bool MoveNext()
			{
				throw new NotImplementedException();
			}

			public void Reset()
			{
				throw new NotImplementedException();
			}

			public CustomStructEnumerator GetEnumerator()
			{
				return this;
			}
		}

		public class CustomClassEnumerator<T>
		{
			public T Current {
				get {
					throw new NotImplementedException();
				}
			}

			public void Dispose()
			{
				throw new NotImplementedException();
			}

			public bool MoveNext()
			{
				throw new NotImplementedException();
			}

			public void Reset()
			{
				throw new NotImplementedException();
			}

			public CustomClassEnumerator<T> GetEnumerator()
			{
				return this;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CustomStructEnumerator<T>
		{
			public T Current {
				get {
					throw new NotImplementedException();
				}
			}

			public void Dispose()
			{
				throw new NotImplementedException();
			}

			public bool MoveNext()
			{
				throw new NotImplementedException();
			}

			public void Reset()
			{
				throw new NotImplementedException();
			}

			public CustomStructEnumerator<T> GetEnumerator()
			{
				return this;
			}
		}

		public class CustomClassEnumeratorWithIDisposable : IDisposable
		{
			public object Current {
				get {
					throw new NotImplementedException();
				}
			}

			public void Dispose()
			{
				throw new NotImplementedException();
			}

			public bool MoveNext()
			{
				throw new NotImplementedException();
			}

			public void Reset()
			{
				throw new NotImplementedException();
			}

			public CustomClassEnumeratorWithIDisposable GetEnumerator()
			{
				return this;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CustomStructEnumeratorWithIDisposable : IDisposable
		{
			public object Current {
				get {
					throw new NotImplementedException();
				}
			}

			public void Dispose()
			{
				throw new NotImplementedException();
			}

			public bool MoveNext()
			{
				throw new NotImplementedException();
			}

			public void Reset()
			{
				throw new NotImplementedException();
			}

			public CustomStructEnumeratorWithIDisposable GetEnumerator()
			{
				return this;
			}
		}

		public class CustomClassEnumeratorWithIDisposable<T> : IDisposable
		{
			public T Current {
				get {
					throw new NotImplementedException();
				}
			}

			public void Dispose()
			{
				throw new NotImplementedException();
			}

			public bool MoveNext()
			{
				throw new NotImplementedException();
			}

			public void Reset()
			{
				throw new NotImplementedException();
			}

			public CustomClassEnumeratorWithIDisposable<T> GetEnumerator()
			{
				return this;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct CustomStructEnumeratorWithIDisposable<T> : IDisposable
		{
			public T Current {
				get {
					throw new NotImplementedException();
				}
			}

			public void Dispose()
			{
				throw new NotImplementedException();
			}

			public bool MoveNext()
			{
				throw new NotImplementedException();
			}

			public void Reset()
			{
				throw new NotImplementedException();
			}

			public CustomStructEnumeratorWithIDisposable<T> GetEnumerator()
			{
				return this;
			}
		}

#if MCS
		[StructLayout(LayoutKind.Sequential, Size = 1)]
#endif
		public struct DataItem
		{
			public int Property { get; set; }

			public void TestCall()
			{
			}
		}

		public class Item
		{

		}

		private IEnumerable<string> alternatives;
		private object someObject;

		private void TryGetItem(int id, out Item item)
		{
			item = null;
		}

		private static void Operation(ref int i)
		{
		}

		private static void Operation(Func<bool> f)
		{
		}

		public void ForEachOnField()
		{
			foreach (string alternative in alternatives)
			{
				alternative.ToLower();
			}
		}

		public void ForEach(IEnumerable<string> alternatives)
		{
			foreach (string alternative in alternatives)
			{
				alternative.ToLower();
			}
		}

		public void ForEachOverList(List<string> list)
		{
			// List has a struct as enumerator, so produces quite different IL than foreach over the IEnumerable interface
			foreach (string item in list)
			{
				item.ToLower();
			}
		}

		public void ForEachOverNonGenericEnumerable(IEnumerable enumerable)
		{
			foreach (object item in enumerable)
			{
				item.ToString();
			}
		}

		public void ForEachOverNonGenericEnumerableWithAutomaticCastValueType(IEnumerable enumerable)
		{
			foreach (int item in enumerable)
			{
				item.ToString();
			}
		}

		public void ForEachOverNonGenericEnumerableWithAutomaticCastRefType(IEnumerable enumerable)
		{
			foreach (string item in enumerable)
			{
				Console.WriteLine(item);
			}
		}

		public void ForEachOnCustomClassEnumerator(CustomClassEnumerator e)
		{
			foreach (object item in e)
			{
				Console.WriteLine(item);
			}
		}

		// TODO : Needs additional pattern detection
		// CustomStructEnumerator does not implement IDisposable
		// No try-finally-Dispose is generated.
		//public void ForEachOnCustomStructEnumerator(CustomStructEnumerator e)
		//{
		//	foreach (object item in e) {
		//		Console.WriteLine(item);
		//	}
		//}

		public void ForEachOnGenericCustomClassEnumerator<T>(CustomClassEnumerator<T> e)
		{
			foreach (T item in e)
			{
				Console.WriteLine(item);
			}
		}

		// TODO : Needs additional pattern detection
		// CustomStructEnumerator does not implement IDisposable
		// No try-finally-Dispose is generated.
		//public void ForEachOnGenericCustomStructEnumerator<T>(CustomStructEnumerator<T> e)
		//{
		//	foreach (T item in e) {
		//		Console.WriteLine(item);
		//	}
		//}

		public void ForEachOnCustomClassEnumeratorWithIDisposable(CustomClassEnumeratorWithIDisposable e)
		{
			foreach (object item in e)
			{
				Console.WriteLine(item);
			}
		}

		public void ForEachOnCustomStructEnumeratorWithIDisposable(CustomStructEnumeratorWithIDisposable e)
		{
			foreach (object item in e)
			{
				Console.WriteLine(item);
			}
		}

		public void ForEachOnGenericCustomClassEnumeratorWithIDisposable<T>(CustomClassEnumeratorWithIDisposable<T> e)
		{
			foreach (T item in e)
			{
				Console.WriteLine(item);
			}
		}

		public void ForEachOnGenericCustomStructEnumeratorWithIDisposable<T>(CustomStructEnumeratorWithIDisposable<T> e)
		{
			foreach (T item in e)
			{
				Console.WriteLine(item);
			}
		}

		public static void NonGenericForeachWithReturnFallbackTest(IEnumerable e)
		{
			Console.WriteLine("NonGenericForeachWithReturnFallback:");
			IEnumerator enumerator = e.GetEnumerator();
			try
			{
				Console.WriteLine("MoveNext");
				if (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					Console.WriteLine("please don't inline 'current'");
					Console.WriteLine(current);
				}
			}
			finally
			{
				IDisposable disposable = enumerator as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
			Console.WriteLine("After finally!");
		}

		public static void ForeachWithRefUsage(List<int> items)
		{
			foreach (int item in items)
			{
#if ROSLYN && OPT
				// The variable name differs based on whether roslyn optimizes out the 'item' variable
				int i = item;
				Operation(ref i);
#else
				int i = item;
				Operation(ref i);
#endif
			}
		}

		public static void ForeachWithCapturedVariable(List<int> items)
		{
			foreach (int item in items)
			{
				int c = item;
				Operation(() => c == 5);
			}
		}

		public static T LastOrDefault<T>(IEnumerable<T> items)
		{
			T result = default(T);
			foreach (T item in items)
			{
				result = item;
			}
			return result;
		}

		public void ForEachOverArray(string[] array)
		{
			foreach (string text in array)
			{
				Console.WriteLine(text.ToLower() + text.ToUpper());
			}
		}

		public unsafe void ForEachOverArrayOfPointers(int*[] array)
		{
			foreach (int* value in array)
			{
				Console.WriteLine(new IntPtr(value));
				Console.WriteLine(new IntPtr(value));
			}
		}

		public void ForEachBreakWhenFound(string name, ref StringComparison output)
		{
#if MCS
			foreach (int value in Enum.GetValues(typeof(StringComparison)))
			{
				if (((StringComparison)value).ToString() == name)
				{
					output = (StringComparison)value;
					break;
				}
			}
#else
			foreach (StringComparison value in Enum.GetValues(typeof(StringComparison)))
			{
				if (value.ToString() == name)
				{
					output = value;
					break;
				}
			}
#endif
		}

		public void ForEachOverListOfStruct(List<DataItem> items, int value)
		{
			foreach (DataItem item in items)
			{
#if ROSLYN && OPT
				// The variable name differs based on whether roslyn optimizes out the 'item' variable
				DataItem current = item;
				current.Property = value;
#else
				DataItem dataItem = item;
				dataItem.Property = value;
#endif
			}
		}

		public void ForEachOverListOfStruct2(List<DataItem> items, int value)
		{
			foreach (DataItem item in items)
			{
#if ROSLYN && OPT
				// The variable name differs based on whether roslyn optimizes out the 'item' variable
				DataItem current = item;
				current.TestCall();
				current.Property = value;
#else
				DataItem dataItem = item;
				dataItem.TestCall();
				dataItem.Property = value;
#endif
			}
		}

		public void ForEachOverListOfStruct3(List<DataItem> items, int value)
		{
			foreach (DataItem item in items)
			{
				item.TestCall();
			}
		}

#if !MCS
		public void ForEachOverMultiDimArray(int[,] items)
		{
			foreach (int value in items)
			{
				Console.WriteLine(value);
				Console.WriteLine(value);
			}
		}

		public void ForEachOverMultiDimArray2(int[,,] items)
		{
			foreach (int value in items)
			{
				Console.WriteLine(value);
				Console.WriteLine(value);
			}
		}

		public unsafe void ForEachOverMultiDimArray3(int*[,] items)
		{
#if ROSLYN && OPT
			foreach (int* intPtr in items)
			{
				Console.WriteLine(*intPtr);
				Console.WriteLine(*intPtr);
			}
#else
			foreach (int* ptr in items)
			{
				Console.WriteLine(*ptr);
				Console.WriteLine(*ptr);
			}
#endif
		}
#endif

		#endregion

		public void ForOverArray(string[] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i].ToLower();
			}
		}

		public void NoForeachOverArray(string[] array)
		{
			for (int i = 0; i < array.Length; i++)
			{
				string value = array[i];
				if (i % 5 == 0)
				{
					Console.WriteLine(value);
				}
			}
		}

		public void NestedLoops()
		{
			for (int i = 0; i < 10; i++)
			{
				if (i % 2 == 0)
				{
					for (int j = 0; j < 5; j++)
					{
						Console.WriteLine("Y");
					}
				}
				else
				{
					Console.WriteLine("X");
				}
			}
		}

		public int MultipleExits()
		{
			int num = 0;
			while (true)
			{
				if (num % 4 == 0)
				{
					return 4;
				}
				if (num % 7 == 0)
				{
					break;
				}
				if (num % 9 == 0)
				{
					return 5;
				}
				if (num % 11 == 0)
				{
					break;
				}
				num++;
			}
			return int.MinValue;
		}

		//public int InterestingLoop()
		//{
		//	int num = 0;
		//	if (num % 11 == 0) {
		//		while (true) {
		//			if (num % 4 == 0) {
		//				if (num % 7 == 0) {
		//					if (num % 11 == 0) {
		//						// use a continue here to prevent moving the if (i%7) outside the loop
		//						continue;
		//					}
		//					Console.WriteLine("7");
		//				} else {
		//					// this block is not part of the natural loop
		//					Console.WriteLine("!7");
		//				}
		//				break;
		//			}
		//			num++;
		//		}
		//		// This instruction is still dominated by the loop header
		//		num = int.MinValue;
		//	}
		//	return num;
		//}

		public int InterestingLoop()
		{
			int num = 0;
			if (num % 11 == 0)
			{
				while (true)
				{
					if (num % 4 == 0)
					{
						if (num % 7 != 0)
						{
							Console.WriteLine("!7");
							break;
						}
						if (num % 11 != 0)
						{
							Console.WriteLine("7");
							break;
						}
					}
					else
					{
						num++;
					}
				}
				num = int.MinValue;
			}
			return num;
		}

		private bool Condition(string arg)
		{
			Console.WriteLine("Condition: " + arg);
			return false;
		}

		public void WhileLoop()
		{
			Console.WriteLine("Initial");
			if (Condition("if"))
			{
				while (Condition("while"))
				{
					Console.WriteLine("Loop Body");
					if (Condition("test"))
					{
						if (Condition("continue"))
						{
							continue;
						}
						if (!Condition("break"))
						{
							break;
						}
					}
					Console.WriteLine("End of loop body");
				}
				Console.WriteLine("After loop");
			}
			Console.WriteLine("End of method");
		}

		//other configurations work fine, just with different labels
#if OPT && !MCS
		public void WhileWithGoto()
		{
			while (Condition("Main Loop"))
			{
				if (Condition("Condition"))
				{
					goto IL_000f;
				}
				// TODO reorder branches with successive block?
				goto IL_0026;
				IL_000f:
				Console.WriteLine("Block1");
				if (Condition("Condition2"))
				{
					continue;
				}
				// TODO remove redundant goto?
				goto IL_0026;
				IL_0026:
				Console.WriteLine("Block2");
				goto IL_000f;
			}
		}
#endif

		public void DoWhileLoop()
		{
			Console.WriteLine("Initial");
			if (Condition("if"))
			{
				do
				{
					Console.WriteLine("Loop Body");
					if (Condition("test"))
					{
						if (Condition("continue"))
						{
							continue;
						}
						if (!Condition("break"))
						{
							break;
						}
					}
					Console.WriteLine("End of loop body");
				} while (Condition("while"));
				Console.WriteLine("After loop");
			}
			Console.WriteLine("End of method");
		}

		public void Issue1395(int count)
		{
			Environment.GetCommandLineArgs();
			for (int i = 0; i < count; i++)
			{
				Environment.GetCommandLineArgs();
				do
				{
#if OPT || MCS
					IL_0013:
#else
					IL_0016:
#endif
					Environment.GetCommandLineArgs();
					if (Condition("part1"))
					{
						Environment.GetEnvironmentVariables();
						if (Condition("restart"))
						{
#if OPT || MCS
							goto IL_0013;
#else
							goto IL_0016;
#endif
						}
					}
					else
					{
						Environment.GetLogicalDrives();
					}
					Environment.GetCommandLineArgs();
					while (count > 0)
					{
						switch (count)
						{
							case 0:
							case 1:
							case 2:
								Environment.GetCommandLineArgs();
								break;
							case 3:
							case 5:
							case 6:
								Environment.GetEnvironmentVariables();
								break;
							default:
								Environment.GetLogicalDrives();
								break;
						}
					}
					count++;
				} while (Condition("do-while"));
				Environment.GetCommandLineArgs();
			}
			Environment.GetCommandLineArgs();
		}

		public void ForLoop()
		{
			Console.WriteLine("Initial");
			if (Condition("if"))
			{
				for (int i = 0; Condition("for"); i++)
				{
					Console.WriteLine("Loop Body");
					if (Condition("test"))
					{
						if (Condition("continue"))
						{
							continue;
						}
						if (!Condition("not-break"))
						{
							break;
						}
					}
					Console.WriteLine("End of loop body");
				}
				Console.WriteLine("After loop");
			}
			Console.WriteLine("End of method");
		}

		public void ReturnFromDoWhileInTryFinally()
		{
			try
			{
				do
				{
					if (Condition("return"))
					{
						return;
					}
				} while (Condition("repeat"));

				Environment.GetCommandLineArgs();
			}
			finally
			{
				Environment.GetCommandLineArgs();
			}

			Environment.GetCommandLineArgs();
		}

		public void ForLoopWithEarlyReturn(int[] ids)
		{
			for (int i = 0; i < ids.Length; i++)
			{
				Item item = null;
				TryGetItem(ids[i], out item);
				if (item == null)
				{
					break;
				}
			}
		}

		public void ForeachLoopWithEarlyReturn(List<object> items)
		{
			foreach (object item in items)
			{
				if ((someObject = item) == null)
				{
					break;
				}
			}
		}

		public void NestedForeach(List<object> items1, List<object> items2)
		{
			foreach (object item in items1)
			{
				bool flag = false;
				foreach (object item2 in items2)
				{
					if (item2 == item)
					{
						flag = true;
						break;
					}
				}

				if (!flag)
				{
					Console.WriteLine(item);
				}
			}
			Console.WriteLine("end");
		}

		public void MergeAroundContinue()
		{
			for (int i = 0; i < 20; i++)
			{
				if (i % 3 == 0)
				{
					if (i != 6)
					{
						continue;
					}
				}
				else if (i % 5 == 0)
				{
					if (i != 5)
					{
						continue;
					}
				}
				else if (i % 7 == 0)
				{
					if (i != 7)
					{
						continue;
					}
				}
				else if (i % 11 == 0)
				{
					continue;
				}

				Console.WriteLine(i);
			}
			Console.WriteLine("end");
		}
	}
}
