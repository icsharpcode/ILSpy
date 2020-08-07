.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
  .ver 4:0:0:0
}
.assembly Issue1454
{
  .hash algorithm 0x00008004
  .ver 1:0:4059:39717
}
.module Issue1454.dll
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000003    //  ILONLY 32BITREQUIRED

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1454 extends [mscorlib]System.Object
{

.method public hidebysig static 
	int32 GetCardinality (
		class [mscorlib]System.Collections.BitArray bitArray
	) cil managed 
{
	.maxstack 5
	.locals init (
		[0] int32[],
		[1] int32,
		[2] int32,
		[3] int32
	)

	IL_0000: ldarg.0
	IL_0001: callvirt instance int32 [mscorlib]System.Collections.BitArray::get_Count()
	IL_0006: ldc.i4.5
	IL_0007: shr
	IL_0008: ldc.i4.1
	IL_0009: add
	IL_000a: newarr [mscorlib]System.Int32
	IL_000f: stloc.0
	IL_0010: ldarg.0
	IL_0011: ldloc.0
	IL_0012: ldc.i4.0
	IL_0013: callvirt instance void [mscorlib]System.Collections.BitArray::CopyTo(class [mscorlib]System.Array, int32)
	IL_0018: ldc.i4.0
	IL_0019: stloc.1
	IL_001a: ldloc.0
	IL_001b: ldloc.0
	IL_001c: ldlen
	IL_001d: conv.i4
	IL_001e: ldc.i4.1
	IL_001f: sub
	IL_0020: ldelema [mscorlib]System.Int32
	IL_0025: dup
	IL_0026: ldind.i4
	IL_0027: ldc.i4.m1
	IL_0028: ldarg.0
	IL_0029: callvirt instance int32 [mscorlib]System.Collections.BitArray::get_Count()
	IL_002e: ldc.i4.s 32
	IL_0030: rem
	IL_0031: ldc.i4.s 31
	IL_0033: and
	IL_0034: shl
	IL_0035: not
	IL_0036: and
	IL_0037: stind.i4
	IL_0038: ldc.i4.0
	IL_0039: stloc.2
	IL_003a: br.s IL_007b
	// loop start (head: IL_007b)
		IL_003c: ldloc.0
		IL_003d: ldloc.2
		IL_003e: ldelem.i4
		IL_003f: stloc.3
		IL_0040: ldloc.3
		IL_0041: ldloc.3
		IL_0042: ldc.i4.1
		IL_0043: shr
		IL_0044: ldc.i4 1431655765
		IL_0049: and
		IL_004a: sub
		IL_004b: stloc.3
		IL_004c: ldloc.3
		IL_004d: ldc.i4 858993459
		IL_0052: and
		IL_0053: ldloc.3
		IL_0054: ldc.i4.2
		IL_0055: shr
		IL_0056: ldc.i4 858993459
		IL_005b: and
		IL_005c: add
		IL_005d: stloc.3
		IL_005e: ldloc.3
		IL_005f: ldloc.3
		IL_0060: ldc.i4.4
		IL_0061: shr
		IL_0062: add
		IL_0063: ldc.i4 252645135
		IL_0068: and
		IL_0069: ldc.i4 16843009
		IL_006e: mul
		IL_006f: ldc.i4.s 24
		IL_0071: shr
		IL_0072: stloc.3
		IL_0073: ldloc.1
		IL_0074: ldloc.3
		IL_0075: add
		IL_0076: stloc.1
		IL_0077: ldloc.2
		IL_0078: ldc.i4.1
		IL_0079: add
		IL_007a: stloc.2

		IL_007b: ldloc.2
		IL_007c: ldloc.0
		IL_007d: ldlen
		IL_007e: conv.i4
		IL_007f: blt.s IL_003c
	// end loop

	IL_0081: ldloc.1
	IL_0082: ret
}
}