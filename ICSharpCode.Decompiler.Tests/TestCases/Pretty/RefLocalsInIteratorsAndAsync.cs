using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class RefLocalsInIteratorsAndAsync
	{
		public async Task<int> SpanStackAllocInAsync(int length)
		{
			await Task.Yield();
			Span<int> span = stackalloc int[length];
			int result = 0;
			for (int i = 0; i < span.Length; i++)
			{
				span[i] = i * i;
				result += span[i];
			}
			await Task.Yield();
			return result;
		}

		public async Task<int> SpanFromArrayInAsync(int[] array)
		{
			await Task.Delay(10);
			Span<int> span = array.AsSpan(1, 2);
			span[0] = 42;
			int result = span[0] + span[1];
			await Task.Delay(10);
			return result;
		}

		public async Task<int> ConstStackAllocAsync()
		{
			await Task.Yield();
			Span<byte> span = stackalloc byte[8];
			span[7] = 42;
			int result = span[7];
			await Task.Yield();
			return result;
		}

		public IEnumerable<int> RefLocalInIterator(int[] array)
		{
			yield return array[0];
			ref int reference = ref array[1];
			reference += 10;
			yield return reference;
			yield return array[1];
		}

		public async Task<int> RefReassignInAsync(int[] array)
		{
			await Task.Yield();
#if EXPECTED_OUTPUT
			array[0] = 1;
			array[1] = 2;
#else
			ref int reference = ref array[0];
			reference = 1;
			reference = ref array[1];
			reference = 2;
#endif
			int result = array[0] + array[1];
			await Task.Yield();
			return result;
		}

		public async Task<T> RefLocalGenericAsync<T>(T[] array)
		{
			await Task.Yield();
#if EXPECTED_OUTPUT
			T result = array[0];
#else
			ref T reference = ref array[0];
			T result = reference;
#endif
			await Task.Yield();
			return result;
		}

		public async Task<int> RefIntoSpanAsync(int[] array)
		{
			await Task.Yield();
			Span<int> span = array.AsSpan();
#if EXPECTED_OUTPUT
			span[1] += 5;
#else
			ref int reference = ref span[1];
			reference += 5;
#endif
			int result = span[1];
			await Task.Yield();
			return result;
		}

		public IEnumerable<int> RefReadonlyInIterator(int[] array)
		{
			yield return 1;
#if EXPECTED_OUTPUT
			yield return array[0] + 1;
#else
			ref readonly int reference = ref array[0];
			yield return reference + 1;
#endif
		}

		public IEnumerable<int> LocalFunctionWithRefInIterator(int[] array)
		{
			yield return Increment(array);
			yield return Increment(array);
			static int Increment(int[] a)
			{
				ref int reference = ref a[0];
				reference++;
				return reference;
			}
		}

		public Task<int> CallsAsyncLocalFunction(int length)
		{
			return Compute(length);
			static async Task<int> Compute(int size)
			{
				await Task.Yield();
				Span<int> span = stackalloc int[size];
				span[0] = size;
				int result = span[0];
				await Task.Yield();
				return result;
			}
		}

		public IEnumerable<int> CallsIteratorLocalFunction(int[] array)
		{
			return Iterate(array);
			static IEnumerable<int> Iterate(int[] source)
			{
				yield return source[0];
				ref int reference = ref source[1];
				reference++;
				yield return reference;
			}
		}

		public async IAsyncEnumerable<int> SpanInAsyncIterator(int[] array)
		{
			await Task.Yield();
			Span<int> span = array.AsSpan();
			int result = span[0] + span[span.Length - 1];
			yield return result;
			await Task.Yield();
			yield return result * 2;
		}

		public IEnumerable<char> ReadOnlySpanInIterator(string text)
		{
			yield return 'a';
			ReadOnlySpan<char> readOnlySpan = text.AsSpan();
			yield return readOnlySpan[readOnlySpan.Length - 1];
		}
	}
}
