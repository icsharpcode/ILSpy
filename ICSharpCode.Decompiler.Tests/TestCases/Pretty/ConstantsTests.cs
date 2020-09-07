using System.Threading.Tasks;

namespace ICSharpCode.Decompiler.Tests.TestCases.Pretty
{
	internal class ConstantsTests
	{
		public ulong Issue1308(ulong u = 8uL)
		{
			Test((u & 0xFFFFFFFFu) != 0);
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

		public void BitwiseAndWithConstantUInt64(ulong a)
		{
			ExpectUInt64(a & 7);
			ExpectUInt64(a & 0x7FFFFFFF);
			ExpectUInt64(a & 0xFFFFFFFFu);
			ExpectUInt64(a & 0x7FFFFFFFFFFFFFFFuL);
			ExpectUInt64(a & 0xFFFFFFFFFFFFFFFFuL);
		}

		public void BitwiseAndWithConstantInt64(long a)
		{
			ExpectInt64(a & 7);
			ExpectInt64(a & 0x7FFFFFFF);
			ExpectInt64(a & 0xFFFFFFFFu);
			ExpectInt64(a & 0x7FFFFFFFFFFFFFFFL);
		}

		public void BitwiseAndWithConstantUInt32(uint a)
		{
			ExpectUInt32(a & 7u);
			ExpectUInt32(a & 0x7FFFFFFFu);
			ExpectUInt32(a & 0xFFFFFFFFu);
		}

		public void BitwiseAndWithConstantInt32(int a)
		{
			ExpectInt32(a & 7);
			ExpectInt32(a & 0x7FFFFFFF);
		}

		public void BitwiseAndWithConstantUInt16(ushort a)
		{
			ExpectUInt16((ushort)(a & 7u));
			ExpectUInt16((ushort)(a & 0x7FFFu));
			ExpectUInt16((ushort)(a & 0xFFFFu));
		}

		public void BitwiseAndWithConstantInt16(short a)
		{
			ExpectInt16((short)(a & 7));
			ExpectInt16((short)(a & 0x7FFF));
		}

		public void BitwiseAndWithConstantUInt8(byte a)
		{
			ExpectUInt8((byte)(a & 7u));
			ExpectUInt8((byte)(a & 0x7Fu));
			ExpectUInt8((byte)(a & 0xFFu));
		}

		public void BitwiseAndWithConstantInt8(sbyte a)
		{
			ExpectInt8((sbyte)(a & 7));
			ExpectInt8((sbyte)(a & 0x7F));
		}

		public void BitwiseOrWithConstantUInt64(ulong a)
		{
			ExpectUInt64(a | 7);
			ExpectUInt64(a | 0x7FFFFFFF);
			ExpectUInt64(a | 0xFFFFFFFFu);
			ExpectUInt64(a | 0x7FFFFFFFFFFFFFFFuL);
			ExpectUInt64(a | 0xFFFFFFFFFFFFFFFFuL);
		}

		public void BitwiseOrWithConstantInt64(long a)
		{
			ExpectInt64(a | 7);
			ExpectInt64(a | 0x7FFFFFFF);
			ExpectInt64(a | 0xFFFFFFFFu);
			ExpectInt64(a | 0x7FFFFFFFFFFFFFFFL);
		}

		public void BitwiseOrWithConstantUInt32(uint a)
		{
			ExpectUInt32(a | 7u);
			ExpectUInt32(a | 0x7FFFFFFFu);
			ExpectUInt32(a | 0xFFFFFFFFu);
		}

		public void BitwiseOrWithConstantInt32(int a)
		{
			ExpectInt32(a | 7);
			ExpectInt32(a | 0x7FFFFFFF);
		}

		public void BitwiseOrWithConstantUInt16(ushort a)
		{
			ExpectUInt16((ushort)(a | 7u));
			ExpectUInt16((ushort)(a | 0x7FFFu));
			ExpectUInt16((ushort)(a | 0xFFFFu));
		}

		public void BitwiseOrWithConstantInt16(short a)
		{
			ExpectInt16((short)(a | 7));
			ExpectInt16((short)(a | 0x7FFF));
		}

		public void BitwiseOrWithConstantUInt8(byte a)
		{
			ExpectUInt8((byte)(a | 7u));
			ExpectUInt8((byte)(a | 0x7Fu));
			ExpectUInt8((byte)(a | 0xFFu));
		}

		public void BitwiseOrWithConstantInt8(sbyte a)
		{
			ExpectInt8((sbyte)(a | 7));
			ExpectInt8((sbyte)(a | 0x7F));
		}

		public void BitwiseXorWithConstantUInt64(ulong a)
		{
			ExpectUInt64(a ^ 7);
			ExpectUInt64(a ^ 0x7FFFFFFF);
			ExpectUInt64(a ^ 0xFFFFFFFFu);
			ExpectUInt64(a ^ 0x7FFFFFFFFFFFFFFFuL);
			ExpectUInt64(a ^ 0xFFFFFFFFFFFFFFFFuL);
		}

		public void BitwiseXorWithConstantInt64(long a)
		{
			ExpectInt64(a ^ 7);
			ExpectInt64(a ^ 0x7FFFFFFF);
			ExpectInt64(a ^ 0xFFFFFFFFu);
			ExpectInt64(a ^ 0x7FFFFFFFFFFFFFFFL);
		}

		public void BitwiseXorWithConstantUInt32(uint a)
		{
			ExpectUInt32(a ^ 7u);
			ExpectUInt32(a ^ 0x7FFFFFFFu);
			ExpectUInt32(a ^ 0xFFFFFFFFu);
		}

		public void BitwiseXorWithConstantInt32(int a)
		{
			ExpectInt32(a ^ 7);
			ExpectInt32(a ^ 0x7FFFFFFF);
		}

		public void BitwiseXorWithConstantUInt16(ushort a)
		{
			ExpectUInt16((ushort)(a ^ 7u));
			ExpectUInt16((ushort)(a ^ 0x7FFFu));
			ExpectUInt16((ushort)(a ^ 0xFFFFu));
		}

		public void BitwiseXorWithConstantInt16(short a)
		{
			ExpectInt16((short)(a ^ 7));
			ExpectInt16((short)(a ^ 0x7FFF));
		}

		public void BitwiseXorWithConstantUInt8(byte a)
		{
			ExpectUInt8((byte)(a ^ 7u));
			ExpectUInt8((byte)(a ^ 0x7Fu));
			ExpectUInt8((byte)(a ^ 0xFFu));
		}

		public void BitwiseXorWithConstantInt8(sbyte a)
		{
			ExpectInt8((sbyte)(a ^ 7));
			ExpectInt8((sbyte)(a ^ 0x7F));
		}

		private void ExpectUInt64(ulong _)
		{

		}

		private void ExpectInt64(long _)
		{

		}

		private void ExpectUInt32(uint _)
		{

		}

		private void ExpectInt32(int _)
		{

		}

		private void ExpectUInt16(ushort _)
		{

		}

		private void ExpectInt16(short _)
		{

		}

		private void ExpectUInt8(byte _)
		{
		}

		private void ExpectInt8(sbyte _)
		{

		}
	}
}
