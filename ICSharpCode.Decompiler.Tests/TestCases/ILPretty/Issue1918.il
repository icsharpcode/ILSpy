// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly Issue1918
{
  .ver 1:0:0:0
}
.module Issue1918.exe
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00020003    //  ILONLY 32BITPREFERRED

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1918
       extends [mscorlib]System.Object
{

.method public hidebysig 
	instance void ProblemFunction (valuetype [mscorlib]System.Guid[] '', int32 ''
	) cil managed 
{
	.maxstack 2
	.locals init (
		[0] valuetype [mscorlib]System.Guid[],
		[1] int32,
		[2] void*,
		[3] valuetype [mscorlib]System.Guid[] pinned,
		[4] native uint*,
		[5] native uint
	)

	IL_0000: ldarg.1
	stloc.0

	IL_0010:
	ldarg.2
	stloc.1
	ldloc.0
	dup
	stloc.3
	brfalse.s IL_0026

	ldloc.3
	ldlen
	conv.i4
	brtrue.s IL_002b

	IL_0026: ldc.i4.0
	conv.u
	stloc.2
	br.s IL_0034

	IL_002b: ldloc.3
	ldc.i4.0
	ldelema [mscorlib]System.Guid
	conv.u
	stloc.2

	IL_0034: ldloc.2
	sizeof [mscorlib]System.UIntPtr
	sub
	stloc.s 4
	ldloc.s 4
	ldind.i
	stloc.s 5
	.try
	{
		ldloc.s 4
		ldloc.1
		conv.i8
		call native uint [mscorlib]System.UIntPtr::op_Explicit(uint64)
		stind.i
		ldarg.1
		
		leave.s IL_005c
	} // end .try
	finally
	{
		ldloc.s 4
		ldloc.s 5
		stind.i
		endfinally
	} // end handler

	IL_005c: ldsfld valuetype [mscorlib]System.Guid[] ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1918::NullVal
	stloc.3
	ret
}

.field public static valuetype [mscorlib]System.Guid[] NullVal

}