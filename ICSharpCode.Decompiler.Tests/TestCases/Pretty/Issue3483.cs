namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal static class Issue3483
	{
		public static int Add_Checked(int x, int y)
		{
			return x + y;
		}
		public static int Add_Unchecked(int x, int y)
		{
			return unchecked(x + y);
		}
		public static int Add_CheckedAndUnchecked_1(int x, int y, int z)
		{
			return x + unchecked(y + z);
		}
		public static int Add_CheckedAndUnchecked_2(int x, int y, int z)
		{
			unchecked
			{
				return x + checked(y + z);
			}
		}
		public static uint Cast_Checked(int x)
		{
			return (uint)x;
		}
		public static uint Cast_Unchecked(int x)
		{
			return unchecked((uint)x);
		}
		public static int Cast_CheckedAndUnchecked_1(int x)
		{
			return (int)unchecked((uint)x);
		}
		public static int Cast_CheckedAndUnchecked_2(int x)
		{
			unchecked
			{
				return (int)checked((uint)x);
			}
		}
	}
}
