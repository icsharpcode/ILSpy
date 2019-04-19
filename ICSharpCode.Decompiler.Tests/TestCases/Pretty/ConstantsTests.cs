namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class ConstantsTests
	{
		public ulong Issue1308(ulong u = 8uL)
		{
			Test((u & uint.MaxValue) != 0);
			return 18446744069414584320uL;
		}

		public void Byte_BitmaskingInCondition(byte v)
		{
			Test((v & 0xF) == 0);
			Test((v & 0x123) == 0);
			Test((v | 0xF) == 0);
			Test((v | 0x123) == 0);
		}

		public void SByte_BitmaskingInCondition(sbyte v)
		{
			Test((v & 0xF) == 0);
			Test((v & 0x123) == 0);
			Test((v | 0xF) == 0);
			Test((v | 0x123) == 0);
		}

		private void Test(bool expr)
		{
		}
	}
}
