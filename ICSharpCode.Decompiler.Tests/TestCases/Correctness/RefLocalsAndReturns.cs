using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Correctness
{
	class RefLocalsAndReturns
	{
		static int[] numbers = { 1, 3, 7, 15, 31, 63, 127, 255, 511, 1023 };
		static string[] strings = { "Hello", "World" };
		static string NullString = "";
		static int DefaultInt = 0;

		public delegate ref TReturn RefFunc<T1, TReturn>(T1 param1);

		public static TReturn Invoker<T1, TReturn>(RefFunc<T1, TReturn> action, T1 value)
		{
			return action(value);
		}

		public static ref int FindNumber(int target)
		{
			for (int ctr = 0; ctr < numbers.Length; ctr++) {
				if (numbers[ctr] >= target)
					return ref numbers[ctr];
			}
			return ref numbers[0];
		}

		public static ref int LastNumber()
		{
			return ref numbers[numbers.Length - 1];
		}

		public static ref int ElementAtOrDefault(int index)
		{
			return ref index < 0 || index >= numbers.Length ? ref DefaultInt : ref numbers[index];
		}

		public static ref int LastOrDefault()
		{
			return ref numbers.Length > 0 ? ref numbers[numbers.Length - 1] : ref DefaultInt;
		}

		public static void DoubleNumber(ref int num)
		{
			Console.WriteLine("old: " + num);
			num *= 2;
			Console.WriteLine("new: " + num);
		}

		public static ref string GetOrSetString(int index)
		{
			if (index < 0 || index >= strings.Length)
				return ref NullString;
			return ref strings[index];
		}

		public static void Main(string[] args)
		{
			DoubleNumber(ref FindNumber(32));
			Console.WriteLine(string.Join(", ", numbers));
			DoubleNumber(ref LastNumber());
			Console.WriteLine(string.Join(", ", numbers));
			Console.WriteLine(GetOrSetString(0));
			GetOrSetString(0) = "Goodbye";
			Console.WriteLine(string.Join(" ", strings));
			GetOrSetString(5) = "Here I mutated the null value!?";
			Console.WriteLine(GetOrSetString(-5));

			Console.WriteLine(Invoker(x => ref numbers[x], 0));
			Console.WriteLine(LastOrDefault());
			LastOrDefault() = 10000;
			Console.WriteLine(ElementAtOrDefault(-5));
		}
	}
}
