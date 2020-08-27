using System.Collections;

namespace ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
	public class Issue1454
	{
		public static int GetCardinality(BitArray bitArray)
		{
			int[] array = new int[(bitArray.Count >> 5) + 1];
			bitArray.CopyTo(array, 0);
			int num = 0;
			array[array.Length - 1] &= ~(-1 << bitArray.Count % 32);
			for (int i = 0; i < array.Length; i++)
			{
				int num2 = array[i];
				num2 -= ((num2 >> 1) & 0x55555555);
				num2 = (num2 & 0x33333333) + ((num2 >> 2) & 0x33333333);
				num2 = ((num2 + (num2 >> 4)) & 0xF0F0F0F) * 16843009 >> 24;
				num += num2;
			}
			return num;
		}
	}
}