.assembly extern mscorlib
{
	.publickeytoken = (B7 7A 5C 56 19 34 E0 89)
	.ver 4:0:0:0
}

.assembly Issue4180
{
	.hash algorithm 0x00008004
	.ver 1:0:0:0
}

.module Issue4180.dll

.class public auto ansi beforefieldinit Helper
	extends [mscorlib]System.Object
{
	.method public hidebysig static int32 Repro() cil managed
	{
		.maxstack 10

		ldc.i4 -0x2B79C052
		neg
		ldc.i4 -0x24C3820C
		add
		ldc.i4 1
		shr
		ldc.i4 0x7D8BF5D
		add
		ldc.i4 0x127F837B
		xor
		neg
		ldc.i4 0x194C5D74
		xor
		not
		ret
	}

	.method public hidebysig static int32 ReproSimple() cil managed
	{
		.maxstack 1

		ldc.i4 -0x2B79C052
		neg
		ret
	}
}
