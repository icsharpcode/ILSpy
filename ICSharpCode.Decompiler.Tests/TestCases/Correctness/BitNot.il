.assembly extern mscorlib
{
	.publickeytoken = ( b7 7a 5c 56 19 34 e0 89 )
	.ver 4:0:0:0
}

.assembly 'ConvTest'
{
	.ver 0:0:0:0
}

.module ConvTest.exe
.corflags 0x00000001 // ILOnly

.class private auto ansi abstract sealed beforefieldinit Program
	extends [mscorlib]System.Object
{
	.method public hidebysig static void Main (string[] args) cil managed 
	{
		.maxstack 8
		.entrypoint

		ldc.i8 0xCCCCCCCCCCCCCCCC
		call int64 Program::BitwiseComplementInNativeSize(int64)
		call void Program::PrintHex(int64)
		
		ldc.i4 0x99999999
		call int64 Program::BitwiseComplementWithUndersizedValue(int32)
		call void Program::PrintHex(int64)
		
		ldc.i4 0x9999
		call int32 Program::BitwiseComplementWithSmallInteger(uint16)
		call void Program::PrintHex(int32)
		
		ldc.i4 0x9999
		call int32 Program::BitwiseComplementWithSmallEnum(valuetype Enums.EUInt16)
		call void Program::PrintHex(int32)
		
		ret
	} // end of method Main
	
	.method public static int64 BitwiseComplementInNativeSize(int64 val)
	{
		ldarg.0
		conv.i    // truncate 64-bits to native-size bits
		not       // negate those native size bits
		conv.i8   // sign extend back to 64-bits
		ret
	}
	
	.method public static int64 BitwiseComplementWithUndersizedValue(int32 val)
	{
		ldarg.0
		conv.u8   // zero extend up to 64-bits
		not       // negate those 64-bits
		ret
	}
	
	.method public static int32 BitwiseComplementWithSmallInteger(uint16 val)
	{
		ldarg.0  // zero extend up to 32-bits
		not      // negate those 32-bits
		ret
	}
	
	.method public static int32 BitwiseComplementWithSmallEnum(valuetype Enums.EUInt16 val)
	{
		ldarg.0  // zero extend up to 32-bits
		not      // negate those 32-bits
		ret
	}
	
	.method public static void PrintHex(int32 val)
	{
		ldstr "{0:x8}"
		ldarg.0
		box valuetype [mscorlib]System.UInt32
		call void [mscorlib]System.Console::WriteLine(string, object)
		ret
	}
	
	.method public static void PrintHex(int64 val)
	{
		ldstr "{0:x16}"
		ldarg.0
		box valuetype [mscorlib]System.UInt64
		call void [mscorlib]System.Console::WriteLine(string, object)
		ret
	}
}

.class public auto ansi sealed Enums.EUInt16
	extends [mscorlib]System.Enum
{
	.field public specialname uint16 __value
}
