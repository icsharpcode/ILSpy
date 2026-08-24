.assembly extern System.Runtime
{
	.publickeytoken = (B7 7A 5C 56 19 34 E0 89)
	.ver 4:0:0:0
}

.assembly CompilerGeneratedAutoProperty
{
	.hash algorithm 0x00008004
	.ver 0:0:0:0
}

.class public auto ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.CompilerGeneratedAutoProperty
	extends [System.Runtime]System.Object
{
	.custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (01 00 00 00)
	.field private initonly string '<Name>k__BackingField'

	.method public hidebysig specialname instance string get_Name() cil managed
	{
		.maxstack 8
		IL_0000: ldarg.0
		IL_0001: ldfld string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.CompilerGeneratedAutoProperty::'<Name>k__BackingField'
		IL_0006: ret
	}

	.method public hidebysig specialname rtspecialname instance void .ctor(string name) cil managed
	{
		.maxstack 8
		IL_0000: ldarg.0
		IL_0001: call instance void [System.Runtime]System.Object::.ctor()
		IL_0006: ldarg.0
		IL_0007: ldarg.1
		IL_0008: stfld string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.CompilerGeneratedAutoProperty::'<Name>k__BackingField'
		IL_000d: ret
	}

	.property instance string Name()
	{
		.get instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.CompilerGeneratedAutoProperty::get_Name()
	}
}
