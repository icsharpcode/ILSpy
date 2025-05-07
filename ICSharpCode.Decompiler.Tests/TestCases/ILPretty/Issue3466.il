.assembly extern System.Runtime
{
	.publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
	.ver 4:0:0:0
}

.assembly Issue3466
{
	.hash algorithm 0x00008004 // SHA1
	.ver 0:0:0:0
}

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3466`1<T>
	extends [System.Runtime]System.Object
{
	.method public hidebysig specialname static 
		class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3466`1<!T> op_Implicit (
			!T t
		) cil managed 
	{
		.maxstack 8

		IL_0000: ldnull
		IL_0001: ret
	}

	.method public final hidebysig static bool M(class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3466`1<object> x) cil managed 
	{
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: brtrue.s IL_0004
		IL_0002: ldc.i4.0
		IL_0003: br.s IL_0005
		IL_0004: ldc.i4.1
		IL_0005: ret
	}

	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [System.Runtime]System.Object::.ctor()
		IL_0006: ret
	}

}