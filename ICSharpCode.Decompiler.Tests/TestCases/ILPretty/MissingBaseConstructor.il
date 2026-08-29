.assembly extern System.Runtime
{
	.publickeytoken = (B7 7A 5C 56 19 34 E0 89)
	.ver 4:0:0:0
}

.assembly extern MissingBaseAssembly
{
	.ver 1:0:0:0
}

.assembly MissingBaseConstructor
{
	.hash algorithm 0x00008004
	.ver 0:0:0:0
}

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.MissingBaseConstructor
	extends [MissingBaseAssembly]MissingBase
{
	.method public hidebysig specialname rtspecialname instance void .ctor(
		valuetype [MissingBaseAssembly]MissingRole role,
		string content
	) cil managed
	{
		.maxstack 8
		IL_0000: ldarg.0
		IL_0001: ldarg.1
		IL_0002: ldarg.2
		IL_0003: call instance void [MissingBaseAssembly]MissingBase::.ctor(
			valuetype [MissingBaseAssembly]MissingRole,
			string
		)
		IL_0008: ret
	}
}
