using System.Collections.Generic;
using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	public class UnsafeInIteratorsAndAsync
	{
		public async Task<int> UnsafeBlockInAsync(int[] array)
		{
			await Task.Yield();
			int result;
			unsafe
			{
				fixed (int* ptr = array)
				{
					result = *ptr + ptr[1];
				}
			}
			await Task.Yield();
			return result;
		}

		public async Task<int> PointerStackAllocInAsync()
		{
			await Task.Yield();
			int result;
			unsafe
			{
				int* ptr = stackalloc int[4];
				*ptr = 1;
				ptr[3] = 4;
				result = *ptr + ptr[3];
			}
			await Task.Yield();
			return result;
		}

		public IEnumerable<int> FixedInIterator(int[] array)
		{
			yield return 1;
			int value;
			unsafe
			{
				fixed (int* ptr = array)
				{
					value = *ptr;
				}
			}
			yield return value;
		}

		public IEnumerable<char> FixedStringInIterator(string text)
		{
			yield return 'x';
			char value;
			unsafe
			{
				fixed (char* ptr = text)
				{
					value = *ptr;
				}
			}
			yield return value;
		}
	}
}
