using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal static class ExtensionMemberUseSites
	{
		public static void TestInstancePropertyUseSites(string s, List<string> list)
		{
			Console.WriteLine(s.IsLong);
			Console.WriteLine(s.Doubled);
			s.Doubled = 5;
			s.Doubled++;
			Console.WriteLine(s?.IsLong);
			Console.WriteLine(list.FirstOrNull);
			Console.WriteLine(list.Select((string x) => x.IsLong).Count());
		}

		public static void TestStaticMemberUseSites()
		{
			Console.WriteLine(string.Fallback);
			string.StaticCounter = 7;
			Console.WriteLine(string.StaticCounter);
			Console.WriteLine(string.Glue("a", "b"));
			Console.WriteLine(List<string>.RepeatedValue("x", 3).Count);
			Console.WriteLine(List<int>.Empty.Count);
		}

		public static void TestByRefReceiverPropertyUseSites()
		{
			int i = 10;
			Console.WriteLine(i.Squared);
			Console.WriteLine(DateTime.Now.TicksDoubled);
			Guid g = Guid.NewGuid();
			Console.WriteLine(g.IsZeroGuid);
		}
	}

	internal static class ExtensionMemberUseSitesExtensions
	{
		extension(string s)
		{
			public bool IsLong => s.Length > 10;

			public int Doubled {
				get {
					return s.Length * 2;
				}
				set {
					Console.WriteLine(value);
				}
			}
		}

		extension(string)
		{
			public static string Fallback => "fallback";

			public static int StaticCounter {
				get {
					return counter;
				}
				set {
					counter = value;
				}
			}

			public static string Glue(string a, string b)
			{
				return a + b;
			}
		}

		extension(ref int i)
		{
			public int Squared => i * i;
		}

		extension(in DateTime d)
		{
			public long TicksDoubled => d.Ticks * 2;
		}

		extension(ref readonly Guid g)
		{
			public bool IsZeroGuid => g == Guid.Empty;
		}

		extension<T>(List<T>)
		{
			public static List<T> Empty => new List<T>();

			public static List<T> RepeatedValue(T item, int count)
			{
				List<T> list = new List<T>(count);
				for (int i = 0; i < count; i++)
				{
					list.Add(item);
				}
				return list;
			}
		}

		extension<T>(List<T> list) where T : class
		{
			public T? FirstOrNull {
				get {
					if (list.Count > 0)
					{
						return list[0];
					}
					return null;
				}
			}
		}

		private static int counter;
	}
}
