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
		public static string[] GetArray()
        {
			throw null;
        }
		public static List<string> GetList()
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

		public static void UseIndex()
		{
#if TODO
			Console.WriteLine(GetArray()[GetIndex()]);
			Console.WriteLine(GetList()[GetIndex()]);
			Console.WriteLine(GetSpan()[GetIndex()]);
			Console.WriteLine(GetString()[GetIndex()]);
			Console.WriteLine(new CustomList()[GetIndex()]);
#endif
			Console.WriteLine(new CustomList2()[GetIndex()]);
		}
		public static void UseRange()
		{
#if TODO
			Console.WriteLine(GetArray()[GetRange()]);
			//Console.WriteLine(GetList()[GetRange()]); // fails to compile
			Console.WriteLine(GetSpan()[GetRange()].ToString());
			Console.WriteLine(GetString()[GetRange()]);
			Console.WriteLine(new CustomList()[GetRange()]);
#endif
			Console.WriteLine(new CustomList2()[GetRange()]);
		}
		public static void UseNewRangeFromIndex()
		{
#if TODO
			Console.WriteLine(GetArray()[GetIndex()..GetIndex()]);
			//Console.WriteLine(GetList()[GetIndex()..GetIndex()]); // fails to compile
			Console.WriteLine(GetSpan()[GetIndex()..GetIndex()].ToString());
			Console.WriteLine(GetString()[GetIndex()..GetIndex()]);
			Console.WriteLine(new CustomList()[GetIndex()..GetIndex()]);
#endif
			Console.WriteLine(new CustomList2()[GetIndex()..GetIndex()]);
		}
		public static void UseNewRangeFromIntegers_BothFromStart()
		{
#if TODO
			Console.WriteLine(GetArray()[GetInt(1)..GetInt(2)]);
			//Console.WriteLine(GetList()[GetInt()..GetInt()]); // fails to compile
			Console.WriteLine(GetSpan()[GetInt(1)..GetInt(2)].ToString());
			Console.WriteLine(GetString()[GetInt(1)..GetInt(2)]);
			Console.WriteLine(new CustomList()[GetInt(1)..GetInt(2)]);
#endif
			Console.WriteLine(new CustomList2()[GetInt(1)..GetInt(2)]);
		}
		public static void UseNewRangeFromIntegers_BothFromEnd()
		{
#if TODO
			Console.WriteLine(GetArray()[^GetInt(1)..^GetInt(2)]);
			//Console.WriteLine(GetList()[^GetInt()..^GetInt()]); // fails to compile
			Console.WriteLine(GetSpan()[^GetInt(1)..^GetInt(2)].ToString());
			Console.WriteLine(GetString()[^GetInt(1)..^GetInt(2)]);
			Console.WriteLine(new CustomList()[^GetInt(1)..^GetInt(2)]);
#endif
			Console.WriteLine(new CustomList2()[^GetInt(1)..^GetInt(2)]);
		}
		public static void UseNewRangeFromIntegers_FromStartAndEnd()
		{
#if TODO
			Console.WriteLine(GetArray()[GetInt(1)..^GetInt(2)]);
			//Console.WriteLine(GetList()[GetInt()..^GetInt()]); // fails to compile
			Console.WriteLine(GetSpan()[GetInt(1)..^GetInt(2)].ToString());
			Console.WriteLine(GetString()[GetInt(1)..^GetInt(2)]);
			Console.WriteLine(new CustomList()[GetInt(1)..^GetInt(2)]);
#endif
			Console.WriteLine(new CustomList2()[GetInt(1)..^GetInt(2)]);
		}
		public static void UseNewRangeFromIntegers_FromEndAndStart()
		{
#if TODO
			Console.WriteLine(GetArray()[^GetInt(1)..GetInt(2)]);
			//Console.WriteLine(GetList()[^GetInt()..GetInt()]);  // fails to compile
			Console.WriteLine(GetSpan()[^GetInt(1)..GetInt(2)].ToString());
			Console.WriteLine(GetString()[^GetInt(1)..GetInt(2)]);
			Console.WriteLine(new CustomList()[^GetInt(1)..GetInt(2)]);
#endif
			Console.WriteLine(new CustomList2()[^GetInt(1)..GetInt(2)]);
		}

		public static void UseNewRangeFromIntegers_OnlyEndPoint()
		{
#if TODO
			Console.WriteLine(GetArray()[..GetInt(2)]);
			//Console.WriteLine(GetList()[..GetInt()]);  // fails to compile
			Console.WriteLine(GetSpan()[..GetInt(2)].ToString());
			Console.WriteLine(GetString()[..GetInt(2)]);
			Console.WriteLine(new CustomList()[..GetInt(2)]);
#endif
			Console.WriteLine(new CustomList2()[..GetInt(2)]);
		}

		public static void UseNewRangeFromIntegers_OnlyStartPoint()
		{
#if TODO
			Console.WriteLine(GetArray()[GetInt(1)..]);
			//Console.WriteLine(GetList()[GetInt()..]); // fails to compile
			Console.WriteLine(GetSpan()[GetInt(1)..].ToString());
			Console.WriteLine(GetString()[GetInt(1)..]);
			Console.WriteLine(new CustomList()[GetInt(1)..]);
#endif
			Console.WriteLine(new CustomList2()[GetInt(1)..]);
		}

		public static void UseWholeRange()
		{
#if TODO
			Console.WriteLine(GetArray()[..]);
			//Console.WriteLine(GetList()[..]); // fails to compile
			Console.WriteLine(GetSpan()[..].ToString());
			Console.WriteLine(GetString()[..]);
			Console.WriteLine(new CustomList()[..]);
#endif
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
