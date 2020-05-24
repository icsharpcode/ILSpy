// Copyright (c) 2020 Daniel Grunwald
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
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class CustomList
	{
		public int Count => 0;
		public int this[int index] => 0;

		public CustomList Slice(int start, int length)
		{
			return this;
		}
	}

	internal class CustomList2
	{
		public int Count => 0;
		public int this[int index] => 0;
		public int this[Index index] => 0;
		public CustomList2 this[Range range] => this;

		public CustomList2 Slice(int start, int length)
		{
			return this;
		}
	}

	internal class IndexRangeTest
	{
		public static int[] GetArray()
		{
			throw null;
		}
		public static List<int> GetList()
		{
			throw null;
		}
		public static Span<int> GetSpan()
		{
			throw null;
		}
		public static string GetString()
		{
			throw null;
		}
		public static Index GetIndex(int i = 0)
		{
			return i;
		}
		public static Range GetRange(int i = 0)
		{
			return i..^i;
		}
		public static int GetInt(int i = 0)
		{
			return i;
		}
		public static Range[] SeveralRanges()
		{
			// Some of these are semantically identical, but we can still distinguish them in the IL code:
			return new Range[14] {
				..,
				0..,
				^0..,
				GetInt(1)..,
				^GetInt(2)..,
				..0,
				..^0,
				..GetInt(3),
				..^GetInt(4),
				0..^0,
				^0..0,
				0..0,
				GetInt(5)..GetInt(6),
				0..(GetInt(7) + GetInt(8))
			};
		}

		public static void UseIndex()
		{
			Console.WriteLine(GetArray()[GetIndex()]);
			Console.WriteLine(GetList()[GetIndex()]);
			Console.WriteLine(GetSpan()[GetIndex()]);
			Console.WriteLine(GetString()[GetIndex()]);
			Console.WriteLine(new CustomList()[GetIndex()]);
			Console.WriteLine(new CustomList2()[GetIndex()]);
		}

		public static void UseIndexFromEnd()
		{
			Console.WriteLine(GetArray()[^GetInt()]);
			Console.WriteLine(GetList()[^GetInt()]);
			Console.WriteLine(GetSpan()[^GetInt()]);
			Console.WriteLine(GetString()[^GetInt()]);
			Console.WriteLine(new CustomList()[^GetInt()]);
			Console.WriteLine(new CustomList2()[^GetInt()]);
		}

		public static void UseIndexForWrite()
		{
			GetArray()[GetIndex()] = GetInt();
			GetList()[GetIndex()] = GetInt();
			GetSpan()[GetIndex()] = GetInt();
		}

		private static void UseRef(ref int i)
		{
		}

		public static void UseIndexForRef()
		{
			UseRef(ref GetArray()[GetIndex()]);
			UseRef(ref GetArray()[^GetInt()]);
			UseRef(ref GetSpan()[GetIndex()]);
			UseRef(ref GetSpan()[^GetInt()]);
		}

		public static void UseRange()
		{
			Console.WriteLine(GetArray()[GetRange()]);
			//Console.WriteLine(GetList()[GetRange()]); // fails to compile
			Console.WriteLine(GetSpan()[GetRange()].ToString());
			Console.WriteLine(GetString()[GetRange()]);
			Console.WriteLine(new CustomList()[GetRange()]);
			Console.WriteLine(new CustomList2()[GetRange()]);
		}
		public static void UseNewRangeFromIndex()
		{
			Console.WriteLine(GetArray()[GetIndex(1)..GetIndex(2)]);
			//Console.WriteLine(GetList()[GetIndex(1)..GetIndex(2)]); // fails to compile
			Console.WriteLine(GetSpan()[GetIndex(1)..GetIndex(2)].ToString());
			Console.WriteLine(GetString()[GetIndex(1)..GetIndex(2)]);
			Console.WriteLine(new CustomList()[GetIndex(1)..GetIndex(2)]);
			Console.WriteLine(new CustomList2()[GetIndex(1)..GetIndex(2)]);
		}
		public static void UseNewRangeFromIntegers_BothFromStart()
		{
			Console.WriteLine(GetArray()[GetInt(1)..GetInt(2)]);
			//Console.WriteLine(GetList()[GetInt()..GetInt()]); // fails to compile
			Console.WriteLine(GetSpan()[GetInt(1)..GetInt(2)].ToString());
			Console.WriteLine(GetString()[GetInt(1)..GetInt(2)]);
			Console.WriteLine(new CustomList()[GetInt(1)..GetInt(2)]);
			Console.WriteLine(new CustomList2()[GetInt(1)..GetInt(2)]);
		}
		public static void UseNewRangeFromIntegers_BothFromEnd()
		{
			Console.WriteLine(GetArray()[^GetInt(1)..^GetInt(2)]);
			//Console.WriteLine(GetList()[^GetInt()..^GetInt()]); // fails to compile
			Console.WriteLine(GetSpan()[^GetInt(1)..^GetInt(2)].ToString());
			Console.WriteLine(GetString()[^GetInt(1)..^GetInt(2)]);
			Console.WriteLine(new CustomList()[^GetInt(1)..^GetInt(2)]);
			Console.WriteLine(new CustomList2()[^GetInt(1)..^GetInt(2)]);
		}
		public static void UseNewRangeFromIntegers_FromStartAndEnd()
		{
			Console.WriteLine(GetArray()[GetInt(1)..^GetInt(2)]);
			//Console.WriteLine(GetList()[GetInt()..^GetInt()]); // fails to compile
			Console.WriteLine(GetSpan()[GetInt(1)..^GetInt(2)].ToString());
			Console.WriteLine(GetString()[GetInt(1)..^GetInt(2)]);
			Console.WriteLine(new CustomList()[GetInt(1)..^GetInt(2)]);
			Console.WriteLine(new CustomList2()[GetInt(1)..^GetInt(2)]);
		}
		public static void UseNewRangeFromIntegers_FromEndAndStart()
		{
			Console.WriteLine(GetArray()[^GetInt(1)..GetInt(2)]);
			//Console.WriteLine(GetList()[^GetInt()..GetInt()]);  // fails to compile
			Console.WriteLine(GetSpan()[^GetInt(1)..GetInt(2)].ToString());
			Console.WriteLine(GetString()[^GetInt(1)..GetInt(2)]);
			Console.WriteLine(new CustomList()[^GetInt(1)..GetInt(2)]);
			Console.WriteLine(new CustomList2()[^GetInt(1)..GetInt(2)]);
		}

		public static void UseNewRangeFromIntegers_OnlyEndPoint()
		{
			Console.WriteLine(GetArray()[..GetInt(2)]);
			//Console.WriteLine(GetList()[..GetInt()]);  // fails to compile
			Console.WriteLine(GetSpan()[..GetInt(2)].ToString());
			Console.WriteLine(GetString()[..GetInt(2)]);
			Console.WriteLine(new CustomList()[..GetInt(2)]);
			Console.WriteLine(new CustomList2()[..GetInt(2)]);
		}

		public static void UseNewRangeFromIntegers_OnlyEndPoint_FromEnd()
		{
			Console.WriteLine(GetArray()[..^GetInt(2)]);
			//Console.WriteLine(GetList()[..^GetInt()]);  // fails to compile
			Console.WriteLine(GetSpan()[..^GetInt(2)].ToString());
			Console.WriteLine(GetString()[..^GetInt(2)]);
			Console.WriteLine(new CustomList()[..^GetInt(2)]);
			Console.WriteLine(new CustomList2()[..^GetInt(2)]);
		}

		public static void UseNewRangeFromIntegers_OnlyStartPoint()
		{
			Console.WriteLine(GetArray()[GetInt(1)..]);
			//Console.WriteLine(GetList()[GetInt()..]); // fails to compile
			Console.WriteLine(GetSpan()[GetInt(1)..].ToString());
			Console.WriteLine(GetString()[GetInt(1)..]);
			Console.WriteLine(new CustomList()[GetInt(1)..]);
			Console.WriteLine(new CustomList2()[GetInt(1)..]);
		}

		public static void UseNewRangeFromIntegers_OnlyStartPoint_FromEnd()
		{
			Console.WriteLine(GetArray()[^GetInt(1)..]);
			//Console.WriteLine(GetList()[^GetInt()..]); // fails to compile
			Console.WriteLine(GetSpan()[^GetInt(1)..].ToString());
			Console.WriteLine(GetString()[^GetInt(1)..]);
			Console.WriteLine(new CustomList()[^GetInt(1)..]);
			Console.WriteLine(new CustomList2()[^GetInt(1)..]);
		}

		public static void UseConstantRange()
		{
			// Fortunately the C# compiler doesn't optimize
			// "str.Length - 2 - 1" here, so the normal pattern applies.
			Console.WriteLine(GetString()[1..2]);
			Console.WriteLine(GetString()[1..^1]);
			Console.WriteLine(GetString()[^2..^1]);
			
			Console.WriteLine(GetString()[..1]);
			Console.WriteLine(GetString()[..^1]);
			Console.WriteLine(GetString()[1..]);
			Console.WriteLine(GetString()[^1..]);
		}

		public static void UseWholeRange()
		{
			Console.WriteLine(GetArray()[..]);
			//Console.WriteLine(GetList()[..]); // fails to compile
			Console.WriteLine(GetSpan()[..].ToString());
			Console.WriteLine(GetString()[..]);
			Console.WriteLine(new CustomList()[..]);
			Console.WriteLine(new CustomList2()[..]);
		}

		public static void UseIndexForIntIndexerWhenIndexIndexerIsAvailable()
		{
			// Same code as the compiler emits for CustomList,
			// but here we can't translate it back to `customList[GetIndex()]`
			// because that would call a different overload.
			CustomList2 customList = new CustomList2();
			int count = customList.Count;
			int offset = GetIndex().GetOffset(count);
			Console.WriteLine(customList[offset]);
		}

		public static void UseSliceWhenRangeIndexerIsAvailable()
		{
			// Same code as the compiler emits for CustomList,
			// but here we can't translate it back to `customList[GetIndex()]`
			// because that would call a different overload.
			CustomList2 customList = new CustomList2();
			int count = customList.Count;
			Range range = GetRange();
			int offset = range.Start.GetOffset(count);
			int length = range.End.GetOffset(count) - offset;
			Console.WriteLine(customList.Slice(offset, length));
		}
	}
}
