using System;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class UserDefinedCompoundAssignment
	{
		public class Counter
		{
			public int Value;

			public void operator +=(int x)
			{
				Value += x;
			}

			public void operator -=(int x)
			{
				Value -= x;
			}

			public void operator *=(int x)
			{
				Value *= x;
			}

			public void operator /=(int x)
			{
				Value /= x;
			}

			public void operator %=(int x)
			{
				Value %= x;
			}

			public void operator &=(int x)
			{
				Value &= x;
			}

			public void operator |=(int x)
			{
				Value |= x;
			}

			public void operator ^=(int x)
			{
				Value ^= x;
			}

			public void operator <<=(int x)
			{
				Value <<= x;
			}

			public void operator >>=(int x)
			{
				Value >>= x;
			}

			public void operator >>>=(int x)
			{
				Value >>>= x;
			}

			public void operator ++()
			{
				Value++;
			}

			public void operator --()
			{
				Value--;
			}

			public void operator checked +=(int x)
			{
				checked
				{
					Value += x;
				}
			}

			public void operator checked ++()
			{
				checked
				{
					Value++;
				}
			}
		}

		public struct MutableStruct
		{
			public int Value;

			public void operator +=(int x)
			{
				Value += x;
			}

			public void operator ++()
			{
				Value++;
			}
		}

		public readonly struct ReadOnlyCounter
		{
			public readonly int Value;

			public void operator +=(int x)
			{
				Console.WriteLine(Value + x);
			}
		}

		public class Mixed
		{
			public int Value;

			public static Mixed operator +(Mixed a, int b)
			{
				return new Mixed {
					Value = a.Value + b
				};
			}

			public void operator +=(int b)
			{
				Value += b;
			}
		}

		public class GenericContainer<T>
		{
			public T Current;

			public void operator +=(T item)
			{
				Current = item;
			}
		}

		public interface IAddAssign<T> where T : IAddAssign<T>
		{
			void operator +=(int x);

			static abstract T operator +(T a, int b);
		}

		public class Impl : IAddAssign<Impl>
		{
			public int Value;

			public void operator +=(int x)
			{
				Value += x;
			}

			public static Impl operator +(Impl a, int b)
			{
				return new Impl {
					Value = a.Value + b
				};
			}
		}

		private Counter counterField;
		private MutableStruct structField;
		private MutableStruct[] structArray;

		public Mixed MixedProp { get; set; }

		public void UseClass(Counter c)
		{
			c += 5;
			c -= 3;
			c *= 2;
			c /= 4;
			c %= 7;
			c &= 6;
			c |= 8;
			c ^= 9;
			c <<= 1;
			c >>= 2;
			c >>>= 3;
			c++;
			c--;
			checked
			{
				c += 5;
				c++;
			}
			// force end of checked block:
			c -= 1;
		}

		public void UseStruct(ref MutableStruct s)
		{
			s += 5;
			s++;
		}

		public void UseReceivers()
		{
			counterField += 1;
			structField += 2;
			structArray[0] += 3;
			MutableStruct mutableStruct = default(MutableStruct);
			mutableStruct += 4;
			Console.WriteLine(mutableStruct.Value);
		}

		public void UseProperty()
		{
			// A property is not a variable, so the instance operator does not apply;
			// this binds to the classic static operator instead.
			MixedProp += 5;
		}

		public void UseReadOnly(ReadOnlyCounter r)
		{
			r += 1;
		}

		public void UseMixed(Mixed m)
		{
			m += 5;
			Console.WriteLine((m + 3).Value);
		}

		public void UseGenericContainer(GenericContainer<string> c)
		{
			c += "hello";
		}

		public void UseConstrainedGeneric<T>(T item) where T : class, IAddAssign<T>
		{
			item += 5;
		}
	}
}
