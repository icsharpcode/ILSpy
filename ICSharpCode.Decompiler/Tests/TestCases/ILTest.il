// C:\Users\Siegfried\Documents\SharpDevelop Projects\HelloWorld\HelloWorld\bin\Debug\HelloWorld.exe

.assembly extern mscorlib
{
	.publickeytoken = (
		b7 7a 5c 56 19 34 e0 89
	)
	.ver 4:0:0:0
}
.assembly HelloWorld
{
	.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = (
		01 00 08 00 00 00 00 00
	)
	.custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = (
		01 00 01 00 54 02 16 57 72 61 70 4e 6f 6e 45 78
		63 65 70 74 69 6f 6e 54 68 72 6f 77 73 01
	)
	.custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = (
		01 00 07 01 00 00 00 00
	)
	.custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = (
		01 00 0a 48 65 6c 6c 6f 57 6f 72 6c 64 00 00
	)
	.custom instance void [mscorlib]System.Reflection.AssemblyDescriptionAttribute::.ctor(string) = (
		01 00 00 00 00
	)
	.custom instance void [mscorlib]System.Reflection.AssemblyConfigurationAttribute::.ctor(string) = (
		01 00 00 00 00
	)
	.custom instance void [mscorlib]System.Reflection.AssemblyCompanyAttribute::.ctor(string) = (
		01 00 00 00 00
	)
	.custom instance void [mscorlib]System.Reflection.AssemblyProductAttribute::.ctor(string) = (
		01 00 0a 48 65 6c 6c 6f 57 6f 72 6c 64 00 00
	)
	.custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = (
		01 00 0e 43 6f 70 79 72 69 67 68 74 20 32 30 31
		36 00 00
	)
	.custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string) = (
		01 00 00 00 00
	)
	.custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = (
		01 00 00 00 00
	)
	.custom instance void [mscorlib]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = (
		01 00 1a 2e 4e 45 54 46 72 61 6d 65 77 6f 72 6b
		2c 56 65 72 73 69 6f 6e 3d 76 34 2e 30 01 00 54
		0e 14 46 72 61 6d 65 77 6f 72 6b 44 69 73 70 6c
		61 79 4e 61 6d 65 10 2e 4e 45 54 20 46 72 61 6d
		65 77 6f 72 6b 20 34
	)
	.hash algorithm 0x00008004 // SHA1
	.ver 1:0:6020:38157
}

.module HelloWorld.exe
// MVID: {987E1A15-519A-400C-B879-759CFB7F990B}
.corflags 0x00000003 // ILOnly, Required32Bit


.class private auto ansi '<Module>'
{
} // end of class <Module>

.class private auto ansi beforefieldinit HelloWorld.Program
	extends [mscorlib]System.Object
{
	// Methods
	.method public hidebysig static 
		void Main (
			string[] args
		) cil managed 
	{
		// Method begins at RVA 0x2050
		// Code size 13 (0xd)
		.maxstack 8
		.entrypoint

		IL_0000: nop
		IL_0001: ldstr "Hello World!"
		IL_0006: call void [mscorlib]System.Console::WriteLine(string)
		IL_000b: nop
		IL_000c: ret
	} // end of method Program::Main

	.method public hidebysig specialname rtspecialname 
		instance void .ctor () cil managed 
	{
		// Method begins at RVA 0x205e
		// Code size 8 (0x8)
		.maxstack 8

		IL_0000: ldarg.0
		IL_0001: call instance void [mscorlib]System.Object::.ctor()
		IL_0006: nop
		IL_0007: ret
	} // end of method Program::.ctor

} // end of class HelloWorld.Program

