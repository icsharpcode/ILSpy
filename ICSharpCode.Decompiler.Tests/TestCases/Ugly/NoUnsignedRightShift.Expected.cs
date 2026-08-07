namespace ICSharpCode.Decompiler.Tests.TestCases.Ugly
{
	internal class NoUnsignedRightShift
	{
		public struct CustomStruct
		{
			public short ShortField;
		}

		public class CustomClass
		{
			public short ShortField;
		}

		public static void ClassField(CustomClass c)
		{
			c.ShortField = (short)((uint)c.ShortField >> 5);
		}

		public static void StructField(CustomStruct s)
		{
			s.ShortField = (short)((uint)s.ShortField >> 5);
		}

		public static void ArrayElement(short[] a)
		{
			a[0] = (short)((uint)a[0] >> 5);
		}
	}
}
