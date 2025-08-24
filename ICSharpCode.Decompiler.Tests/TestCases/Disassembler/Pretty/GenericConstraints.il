.assembly extern mscorlib
{
	.publickeytoken = (
		b7 7a 5c 56 19 34 e0 89
	)
	.ver 4:0:0:0
}
.assembly GenericConstraints
{
	.custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = (
		01 00 07 31 2e 30 2e 30 2e 30 00 00
	)
	.hash algorithm 0x00008004 // SHA1
	.ver 1:0:0:0
}

.module GenericConstraints.dll
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003 // WindowsCui
.corflags 0x00000001 // ILOnly

.class private auto ansi '<Module>'
{
} // end of class <Module>

.class public auto ansi beforefieldinit TestType`1<byreflike T>
	extends [mscorlib]System.Object
{
	// Methods
	.method public hidebysig specialname rtspecialname
		instance void .ctor () cil managed
	{
		// Method begins at RVA 0x2050
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: ret
	} // end of method TestType::.ctor

} // end of class TestType
