.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1256
	extends [mscorlib]System.Object
{
	// Methods
	.method public hidebysig 
		instance void Method (
			class [mscorlib]System.Enum e,
			object o,
			string s
		) cil managed 
	{
		// Method begins at RVA 0x2050
		// Code size 41 (0x29)
		.maxstack 1
		.locals init (
			[0] int32,
			[1] object,
			[2] int32,
			[3] int64,
			[4] int32,
			[5] int32
		)

		IL_0000: nop
		IL_0001: ldarg.1
		IL_0002: unbox.any [mscorlib]System.Int32
		IL_0007: stloc.0
		IL_0008: newobj instance void [mscorlib]System.Object::.ctor()
		IL_000d: stloc.1
		IL_000e: ldloc.1
		IL_000f: unbox.any [mscorlib]System.Int32
		IL_0014: stloc.2
		IL_0015: ldarg.2
		IL_0016: unbox.any [mscorlib]System.Int64
		IL_001b: stloc.3
		IL_001c: ldarg.3
		IL_001d: unbox.any [mscorlib]System.Int32
		IL_0022: stloc.s 4
		IL_0024: ldloc.3
		IL_0025: conv.i4
		IL_0026: stloc.s 5
		IL_0028: ret
	} // end of method Issue1256::Method

	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x2085
		// Code size 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: nop
		IL_0007: ret
	} // end of method Issue1256::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1256
