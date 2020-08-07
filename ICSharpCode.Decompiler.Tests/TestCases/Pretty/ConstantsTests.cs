using System.Threading.Tasks;

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

		public void Enum_Flag_Check(TaskCreationOptions v)
		{
			Test((v & TaskCreationOptions.AttachedToParent) != 0);
			Test((v & TaskCreationOptions.AttachedToParent) == 0);
		}

		private void Test(bool expr)
		{
		}

		private void Test(decimal expr)
		{
		}

		public void Decimal()
		{
			// Roslyn and legacy csc both normalize the decimal constant references,
			// but to a different representation (ctor call vs. field use)
			Test(0m);
			Test(1m);
			Test(-1m);
			Test(decimal.MinValue);
			Test(decimal.MaxValue);
		}
	}
}
