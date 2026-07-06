using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal static class EmptyGroups
	{
		extension(int)
		{
		}

		extension(int x)
		{
		}

		extension(int y)
		{
		}

		extension<T>(IEnumerable<T>)
		{
		}

		extension<T>(IEnumerable<T> x)
		{
		}

		extension<T>(IEnumerable<T> y)
		{
		}

		extension<TKey, TValue>(Dictionary<TKey, TValue>)
		{
		}

		extension<TKey, TValue>(Dictionary<TKey, TValue> x)
		{
		}

		extension<TKey, TValue>(Dictionary<TKey, TValue> y)
		{
		}
	}

	internal static class ExtensionEverything
	{
		extension<T>(ICollection<T> collection) where T : notnull
		{
			public bool IsEmpty => collection.Count == 0;

			public int Test {
				get {
					return 42;
				}
				set {
				}
			}

			public void AddIfNotNull(T item)
			{
				if (item != null)
				{
					collection.Add(item);
				}
			}

			public T2 CastElementAt<T2>(int index) where T2 : T
			{
				return (T2)(object)collection.ElementAt(index);
			}

			public static void StaticExtension()
			{
			}
		}

		extension(ExtensionEverythingTestUseSites.Point point)
		{
			public double Magnitude => Math.Sqrt(point.X * point.X + point.Y * point.Y);
		}

		extension(string s)
		{
			public string Shout()
			{
				return s.ToUpperInvariant();
			}

			public void TakesRefIn(ref int x, in int y, out int z)
			{
				z = x + y;
				x++;
			}

			public int SumAll(params int[] xs)
			{
				int num = s.Length;
				for (int i = 0; i < xs.Length; i++)
				{
					num += xs[i];
				}
				return num;
			}

			public string WithSuffix(string suffix = "!")
			{
				return s + suffix;
			}

			public T2 ConvertTo<T2>() where T2 : class
			{
				return (T2)(object)s;
			}
		}

		extension(string)
		{
			public static int Counter {
				get {
					return counter;
				}
				set {
					counter = value;
				}
			}

			public static string Combine(string a, string b)
			{
				return a + b;
			}
		}

		extension(ref int i)
		{
			public void Increment()
			{
				i++;
			}
		}

		extension(in DateTime d)
		{
			public long TicksTimesTwo()
			{
				return d.Ticks * 2;
			}
		}

		extension(ref readonly Guid g)
		{
			public bool IsEmptyGuid()
			{
				return g == Guid.Empty;
			}
		}

		extension(int? maybe)
		{
			public int OrElse(int fallback)
			{
				return maybe ?? fallback;
			}
		}

		extension(int[] arr)
		{
			public int CountTwice()
			{
				return arr.Length * 2;
			}
		}

		extension<T>(List<T>)
		{
			public static List<T> Repeated(T item, int count)
			{
				List<T> list = new List<T>(count);
				for (int i = 0; i < count; i++)
				{
					list.Add(item);
				}
				return list;
			}
		}

		private static int counter;

		public static string Classic(this string s)
		{
			return s + "!";
		}
	}

	internal class ExtensionEverythingTestUseSites
	{
		public record struct Point(int X, int Y);

		private static Func<string> storedShout;

		public static void TestExtensionProperty()
		{
			Point point = new Point(3, 4);
			Console.WriteLine(point.X);
			Console.WriteLine(point.Y);
			// Extension property use sites are not decompiled back to member-access
			// form yet; they are specced by the ExtensionMemberUseSites fixture.
			//Console.WriteLine(point.Magnitude);
		}

		public static void TestExtensionMethods()
		{
			List<string> collection = new List<string>();
			// Extension property use sites are not decompiled back to member-access
			// form yet; they are specced by the ExtensionMemberUseSites fixture.
			//Console.WriteLine(collection.IsEmpty);
			collection.AddIfNotNull("Hello");
			collection.AddIfNotNull(null);
			//Console.WriteLine(collection.IsEmpty);
			//Console.WriteLine(collection.Test);
			//collection.Test = 100;
			//List<string>.StaticExtension();
		}

		public static void TestMethodUseSites(string s, int? maybe, int[] arr)
		{
			Console.WriteLine(s.Shout());
			int x = 1;
			s.TakesRefIn(ref x, 2, out var z);
			Console.WriteLine(z);
			Console.WriteLine(s.SumAll(1, 2, 3));
			Console.WriteLine(s.WithSuffix());
			Console.WriteLine(s.ConvertTo<object>());
			Console.WriteLine(s?.Shout());
			Console.WriteLine(s.Classic());
			Console.WriteLine(maybe.OrElse(42));
			Console.WriteLine(arr.CountTwice());
			storedShout = s.Shout;
		}

		public static void TestByRefReceiverUseSites()
		{
			int i = 10;
			i.Increment();
			Console.WriteLine(i);
			Console.WriteLine(DateTime.Now.TicksTimesTwo());
			Guid g = Guid.NewGuid();
			Console.WriteLine(g.IsEmptyGuid());
		}

		public static void TestStaticMemberUseSites()
		{
			Console.WriteLine(ExtensionEverything.Combine("a", "b"));
			Console.WriteLine(ExtensionEverything.Repeated("x", 3).Count);
		}

		public static void TestOperatorUseSites(int[] a, int[] b)
		{
			Console.WriteLine((a + b).Length);
			Console.WriteLine((-a).Length);
			if (a)
			{
				Console.WriteLine("nonempty");
			}
		}
	}

	internal static class ExtensionMembersWithAttributes
	{
		extension([Marker] string s)
		{
			[Marker]
			public int AttributedLength {
				[Marker]
				get {
					return s.Length;
				}
			}

			[Marker]
			[return: Marker]
			public string AttributedMethod([Marker] int x)
			{
				return s + x;
			}
		}
	}

	internal static class ExtensionOperators
	{
		extension(int[] arr)
		{
			public static int[] operator +(int[] a, int[] b)
			{
				int[] array = new int[a.Length];
				for (int i = 0; i < a.Length; i++)
				{
					array[i] = a[i] + b[i];
				}
				return array;
			}

			public static int[] operator -(int[] a)
			{
				int[] array = new int[a.Length];
				for (int i = 0; i < a.Length; i++)
				{
					array[i] = -a[i];
				}
				return array;
			}

			public static bool operator true(int[] a)
			{
				return a.Length != 0;
			}

			public static bool operator false(int[] a)
			{
				return a.Length == 0;
			}
		}
	}

	internal class MarkerAttribute : Attribute
	{
	}
}
