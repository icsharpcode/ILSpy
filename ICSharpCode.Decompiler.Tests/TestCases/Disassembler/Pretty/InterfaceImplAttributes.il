.assembly extern mscorlib
{
	.publickeytoken = (
		b7 7a 5c 56 19 34 e0 89
	)
	.ver 4:0:0:0
}
.assembly InterfaceImplAttributes
{
	.custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = (
		01 00 07 31 2e 30 2e 30 2e 30 00 00
	)
	.hash algorithm 0x00008004 // SHA1
	.ver 1:0:0:0
}

.module InterfaceImplAttributes.dll
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003 // WindowsCui
.corflags 0x00000001 // ILOnly

.class private auto ansi '<Module>'
{
} // end of class <Module>

.class public auto ansi beforefieldinit TestType
	extends [mscorlib]System.Object
	implements ITestInterfaceA
{
	.interfaceimpl type ITestInterfaceA
	.custom instance void TestAttributeA::.ctor() = (
		01 00 00 00
	)

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

.class interface public auto ansi abstract ITestInterfaceA
{
} // end of class ITestInterfaceA

.class public auto ansi beforefieldinit TestAttributeA
	extends [mscorlib]System.Attribute
{
	// Methods
	.method public hidebysig specialname rtspecialname
		instance void .ctor () cil managed
	{
		// Method begins at RVA 0x2058
		// Header size: 1
		// Code size: 7 (0x7)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Attribute::.ctor()
		IL_0006: ret
	} // end of method TestAttributeA::.ctor

} // end of class TestAttributeA

