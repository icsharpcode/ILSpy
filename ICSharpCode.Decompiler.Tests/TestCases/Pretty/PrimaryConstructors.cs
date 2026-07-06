using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
#pragma warning disable CS9107, CS9124

	public class AsyncCapture(int delay)
	{
		public async Task<int> WaitAsync()
		{
			await Task.Delay(delay);
			return delay;
		}
	}

	public class BaseArgAndCapture(string message) : Exception(message)
	{
		public string Twice()
		{
			return message + message;
		}
	}

	public class CaptureOnly(int a, string b)
	{
		public string Describe()
		{
			return $"{a}: {b}";
		}
	}

	public class DoubleUse(int x)
	{
		public int Initial = x;

		public int Current()
		{
			return x;
		}
	}

#if EXPECTED_OUTPUT
	public class EmptyPrimary
#else
	public class EmptyPrimary()
#endif
	{
		public int X = 42;
	}

	public class GenericHolder<T>(T item, Func<T, string> formatter) where T : class
	{
		public T Item { get; } = item;

		public string Format()
		{
			return formatter(item);
		}
	}

	public class InParam(in int size)
	{
		public int Size = size;
	}

	public class IteratorCapture(int count)
	{
		public IEnumerable<int> Items()
		{
			for (int i = 0; i < count; i++)
			{
				yield return i;
			}
		}
	}

	public class LambdaCapture(int factor)
	{
		public Func<int, int> Scale => (int x) => x * factor;

		public int Factor => factor;
	}

	public class LocalFunctionCapture(int seed)
	{
		public int Compute()
		{
			return Next() + Next();
			int Next()
			{
				return seed++;
			}
		}
	}

	[AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
	public class MarkerAttribute : Attribute
	{
	}

	public class MemberShadows(int x)
	{
		private int x = x;

		public int M()
		{
			return x;
		}
	}

	public class MutateCapture(int counter)
	{
		public int Next()
		{
			return ++counter;
		}
	}

	public class Outer(int a)
	{
		public class Nested
		{
			public int B = 1;
		}

		public int A => a;
	}

	public class ParamAttr([Marker] int x)
	{
		public int X = x;
	}

	public readonly struct ReadonlyPoint(double x, double y)
	{
		public double X { get; } = x;

		public double Y { get; } = y;

		public double Magnitude => Math.Sqrt(X * X + Y * Y);
	}

	public class ShadowedByMember(int Value)
	{
		public int Value { get; } = Value;
	}

	public struct StructCapture(int seed)
	{
		public int Next()
		{
			return seed++;
		}
	}

	public unsafe class UnsafeCtor(int* p)
	{
		public unsafe int Value = *p;
	}

	public class WithStatics(string name)
	{
		public static int Instances;

		public string Name => name;
	}
}
