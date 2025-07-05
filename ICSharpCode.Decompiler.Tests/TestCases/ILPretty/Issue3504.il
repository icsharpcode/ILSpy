.class private auto ansi beforefieldinit Issue3504
	extends [System.Runtime]System.Object
{
	// Methods
	.method private hidebysig 
		instance void Method (class [System.Console]System.Console console) cil managed 
	{
		// Method begins at RVA 0x2050
		// Header size: 1
		// Code size: 13 (0xd)
		.maxstack 8

		IL_0000: nop
		IL_0001: ldarg.1
        IL_0002: ldstr "Hello."
		IL_0007: call instance void [System.Console]System.Console::WriteLine(string)
		IL_000b: nop
		IL_000c: ret
	} // end of method Issue3504::Method

	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x205e
		// Header size: 1
		// Code size: 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [System.Runtime]System.Object::.ctor()
		IL_0006: nop
		IL_0007: ret
	} // end of method Issue3504::.ctor

} // end of class Issue3504

