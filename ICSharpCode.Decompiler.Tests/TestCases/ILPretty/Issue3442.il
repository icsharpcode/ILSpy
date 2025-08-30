.assembly extern System.Runtime
{
	.publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
	.ver 4:0:0:0
}

.assembly Issue3442
{
	.custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = (
		01 00 08 00 00 00 00 00
	)
	.custom instance void [System.Runtime]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = (
		01 00 01 00 54 02 16 57 72 61 70 4e 6f 6e 45 78
		63 65 70 74 69 6f 6e 54 68 72 6f 77 73 01
	)
	.custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = (
		01 00 02 00 00 00 00 00
	)
	.permissionset reqmin = (
		2e 01 80 8a 53 79 73 74 65 6d 2e 53 65 63 75 72
		69 74 79 2e 50 65 72 6d 69 73 73 69 6f 6e 73 2e
		53 65 63 75 72 69 74 79 50 65 72 6d 69 73 73 69
		6f 6e 41 74 74 72 69 62 75 74 65 2c 20 53 79 73
		74 65 6d 2e 52 75 6e 74 69 6d 65 2c 20 56 65 72
		73 69 6f 6e 3d 39 2e 30 2e 30 2e 30 2c 20 43 75
		6c 74 75 72 65 3d 6e 65 75 74 72 61 6c 2c 20 50
		75 62 6c 69 63 4b 65 79 54 6f 6b 65 6e 3d 62 30
		33 66 35 66 37 66 31 31 64 35 30 61 33 61 15 01
		54 02 10 53 6b 69 70 56 65 72 69 66 69 63 61 74
		69 6f 6e 01
	)
	.hash algorithm 0x00008004 // SHA1
	.ver 0:0:0:0
}

.class private auto ansi '<Module>'
{
} // end of class <Module>

.class interface public auto ansi abstract beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3442.Interface
{
	// Methods
	.method public hidebysig newslot abstract virtual 
		instance void M<(ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3442.Interface) T> () cil managed 
	{
	} // end of method Interface::M

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3442.Interface

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3442.Class
	extends [System.Runtime]System.Object
	implements ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3442.Interface
{
	// Methods
	.method private final hidebysig newslot virtual 
		instance void M<(ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3442.Interface) T> () cil managed 
	{
		.override method instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3442.Interface::M()
		// Method begins at RVA 0x2050
		// Code size 1 (0x1)
		.maxstack 8

		IL_0000: ret
	} // end of method Class::M

	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x2052
		// Code size 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [System.Runtime]System.Object::.ctor()
		IL_0006: ret
	} // end of method Class::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue3442.Class