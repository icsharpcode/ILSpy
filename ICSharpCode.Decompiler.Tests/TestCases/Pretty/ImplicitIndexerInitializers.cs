using System;
using System.Collections.Generic;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class ImplicitIndexerInitializers
	{
		public class IndexedContainer
		{
			private readonly int[] store = new int[10];

			public int this[Index index] {
				get {
					return store[index];
				}
				set {
					store[index] = value;
				}
			}
		}

		public class CountedContainer
		{
			private readonly int[] store = new int[10];

			public int Length => store.Length;

			public int this[int index] {
				get {
					return store[index];
				}
				set {
					store[index] = value;
				}
			}
		}

		public class NestedData
		{
			public int X;

			public int Y { get; set; }
		}

		public struct StructWithArray(int size)
		{
			public int[] Data = new int[size];
		}

		public class GenericHolder<T>
		{
			public T[] Values { get; } = new T[4];
		}

		public class Container
		{
			private readonly int[] buffer = new int[10];

			public StructWithArray Struct;

			public int[] Buffer => buffer;

			public IndexedContainer Indexed { get; } = new IndexedContainer();

			public CountedContainer Counted { get; } = new CountedContainer();

			public List<int> List { get; } = new List<int> { 1, 2, 3 };

			public NestedData[] Items { get; } = new NestedData[3] {
				new NestedData(),
				new NestedData(),
				new NestedData()
			};

			public int Count { get; set; }
		}

		public static int GetInt(int i = 0)
		{
			return i;
		}

		public static Container ArrayFromEnd()
		{
			return new Container {
				Buffer = {
					[^1] = 5
				}
			};
		}

		public static Container ArrayMixedEntries()
		{
			return new Container {
				Buffer = {
					[0] = 1,
					[^2] = 6,
					[^1] = 5
				}
			};
		}

		public static Container IndexIndexerFromEnd()
		{
			return new Container {
				Indexed = {
					[^1] = 5,
					[^GetInt(1)] = 6,
					[2] = 4
				}
			};
		}

		public static Container CountPatternFromEnd()
		{
			return new Container {
				Counted = {
					[^1] = 5
				}
			};
		}

		public static Container ListFromEnd()
		{
			return new Container {
				List = {
					[0] = 1,
					[^1] = 3
				}
			};
		}

		public static Container NestedObjectInitializerFromEnd()
		{
			return new Container {
				Items = {
					[^1] = {
						X = 1,
						Y = 2
					}
				}
			};
		}

		public static Container MixedWithMemberInitializers()
		{
			return new Container {
				Count = 3,
				Buffer = {
					[^1] = 5
				},
				Indexed = {
					[2] = 4
				}
			};
		}

		public static Container SideEffectingOperands()
		{
			return new Container {
				Buffer = {
					[^GetInt(1)] = GetInt(2)
				}
			};
		}

		public static Container StructFieldArrayFromEnd()
		{
			return new Container {
				Struct = {
					Data = {
						[^1] = 7
					}
				}
			};
		}

		public static GenericHolder<string> GenericArrayFromEnd()
		{
			return new GenericHolder<string> {
				Values = {
					[^1] = "last"
				}
			};
		}

		public static List<Container> InsideCollectionInitializer()
		{
			return new List<Container> {
				new Container {
					Buffer = {
						[^1] = 1
					}
				}
			};
		}
	}
}
