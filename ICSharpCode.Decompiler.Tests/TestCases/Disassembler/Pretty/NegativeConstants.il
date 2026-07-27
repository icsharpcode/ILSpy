.assembly extern mscorlib
{
	.publickeytoken = (
		b7 7a 5c 56 19 34 e0 89
	)
	.ver 4:0:0:0
}
.assembly NegativeConstants
{
	.ver 1:0:0:0
}

.module NegativeConstants.dll
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003 // WindowsCui
.corflags 0x00000001 // ILOnly

.class private auto ansi '<Module>'
{
} // end of class <Module>

.class public auto ansi abstract sealed beforefieldinit NegativeConstants
	extends [mscorlib]System.Object
{
	// Methods
	.method public hidebysig static 
		int32 Constants () cil managed 
	{
		// Method begins at RVA 0x2050
		// Header size: 1
		// Code size: 22 (0x16)
		.maxstack 8

		IL_0000: ldc.i4 -501 // 0xfffffe0b
		IL_0005: ldc.i4 300
		IL_000a: add
		IL_000b: conv.i8
		IL_000c: ldc.i8 -2 // 0xfffffffffffffffe
		IL_0015: ret
	} // end of method NegativeConstants::Constants

} // end of class NegativeConstants

