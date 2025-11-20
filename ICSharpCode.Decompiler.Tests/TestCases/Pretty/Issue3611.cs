

using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class Issue3611
	{
		private class C4(string value)
		{
			public object Obj { get; } = new object();
			public string Value { get; } = value;
		}

		private class C5(C5.ValueArray array)
		{
			public struct ValueArray
			{
				private bool b;
				public bool[] ToArray()
				{
					return null;
				}
			}

			public bool[] Values = array.ToArray();
		}

		private class BaseClass
		{
			protected BaseClass(int value)
			{
			}
		}

		private class C6(C6.Data2 data) : BaseClass(data.Value)
		{
			public struct Data2
			{
				public int Value { get; set; }
			}

			public Data2 Data => data;
		}

		private struct S3<T>(T v)
		{
			public T Value => v;
		}

		private interface I1
		{
			int Number { get; }
		}

		// May be the same issue as S3
		private struct S4<T>(int number) : IComparable<T> where T : I1
		{
			public int CompareTo(T other)
			{
				return number.CompareTo(other.Number);
			}
		}
	}
}
