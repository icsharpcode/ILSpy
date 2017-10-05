// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern System
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern Microsoft.VisualBasic
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 10:0:0:0
}
.assembly ConsoleApp11
{
  .ver 1:0:0:0
}
.module ConsoleApp11.exe
// MVID: {B973FCD6-A9C4-48A9-8291-26DDC248E208}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00020003    //  ILONLY 32BITPREFERRED
// Image base: 0x000001C4B6C90000

.class private auto ansi sealed ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue646
	extends [mscorlib]System.Object
{
	.custom instance void [Microsoft.VisualBasic]Microsoft.VisualBasic.CompilerServices.StandardModuleAttribute::.ctor() = (
		01 00 00 00
	)
	// Methods
	.method public static 
		void Main () cil managed 
	{
		.custom instance void [mscorlib]System.STAThreadAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x21b4
		// Code size 61 (0x3d)
		.maxstack 1
		.entrypoint
		.locals init (
			[0] class [mscorlib]System.Collections.Generic.List`1<string>,
			[1] valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>,
			[2] string,
			[3] bool
		)

		IL_0000: nop
		IL_0001: newobj instance void class [mscorlib]System.Collections.Generic.List`1<string>::.ctor()
		IL_0006: stloc.0
		.try
		{
			IL_0007: ldloc.0
			IL_0008: callvirt instance valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<!0> class [mscorlib]System.Collections.Generic.List`1<string>::GetEnumerator()
			IL_000d: stloc.1
			IL_000e: br.s IL_0020
			// loop start (head: IL_0020)
				IL_0010: ldloca.s 1
				IL_0012: call instance !0 valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>::get_Current()
				IL_0017: stloc.2
				IL_0018: ldloc.2
				IL_0019: call void [System]System.Diagnostics.Debug::WriteLine(string)
				IL_001e: nop
				IL_001f: nop

				IL_0020: ldloca.s 1
				IL_0022: call instance bool valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>::MoveNext()
				IL_0027: stloc.3
				IL_0028: ldloc.3
				IL_0029: brtrue.s IL_0010
			// end loop

			IL_002b: leave.s IL_003c
		} // end .try
		finally
		{
			IL_002d: ldloca.s 1
			IL_002f: constrained. valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<string>
			IL_0035: callvirt instance void [mscorlib]System.IDisposable::Dispose()
			IL_003a: nop
			IL_003b: endfinally
		} // end handler

		IL_003c: ret
	} // end of method Module1::Main

} // end of class ConsoleApp11.Module1
