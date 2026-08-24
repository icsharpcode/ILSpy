.assembly extern System.Runtime
{
	.publickeytoken = (B7 7A 5C 56 19 34 E0 89)
	.ver 4:0:0:0
}

.assembly LateBaseConstructorCall
{
	.hash algorithm 0x00008004
	.ver 0:0:0:0
}

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LateBaseConstructorCall
	extends [System.Runtime]System.Object
{
	.method private hidebysig static void Initialize() cil managed
	{
		.maxstack 8
		IL_0000: ret
	}

	.method public hidebysig specialname rtspecialname instance void .ctor() cil managed
	{
		.maxstack 8
		IL_0000: call void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.LateBaseConstructorCall::Initialize()
		IL_0005: ldarg.0
		IL_0006: call instance void [System.Runtime]System.Object::.ctor()
		IL_000b: ret
	}
}
