.assembly extern System.Runtime
{
	.publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
	.ver 4:0:0:0
}

.assembly Issue3344
{
	.hash algorithm 0x00008004 // SHA1
	.ver 0:0:0:0
}

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3344
	extends [System.Runtime]System.Object
{
    .method private hidebysig static 
        float32 GetFloat () cil managed 
    {
        .maxstack 8

        IL_0000: ldc.r4 3.5
        IL_0005: ret
    }

    .method private hidebysig static 
        float32 CkFinite () cil managed 
    {
        .maxstack 1
        .locals init (
            [0] float32
        )

        call float32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3344::GetFloat()
        ckfinite
        ret
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