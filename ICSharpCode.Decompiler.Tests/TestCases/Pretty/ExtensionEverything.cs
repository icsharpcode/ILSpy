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
	}

	internal class ExtensionEverythingTestUseSites
	{
		public record struct Point(int X, int Y);

		public static void TestExtensionProperty()
		{
			Point point = new Point(3, 4);
			Console.WriteLine(point.X);
			Console.WriteLine(point.Y);
			// TODO implement use-site transformation
			//Console.WriteLine(point.Magnitude);
		}

		public static void TestExtensionMethods()
		{
			List<string> collection = new List<string>();
			// TODO implement use-site transformation
			//Console.WriteLine(collection.IsEmpty);
			collection.AddIfNotNull("Hello");
			collection.AddIfNotNull(null);
			//Console.WriteLine(collection.IsEmpty);
			//Console.WriteLine(collection.Test);
			//collection.Test = 100;
			//List<string>.StaticExtension();
		}
	}
}
