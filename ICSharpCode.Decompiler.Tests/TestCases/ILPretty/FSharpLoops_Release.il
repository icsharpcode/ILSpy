// C:\Users\Siegfried\Documents\Visual Studio 2017\Projects\ConsoleApp13\ConsoleApplication1\bin\Release\ConsoleApplication1.exe

.assembly extern mscorlib
{
	.publickeytoken = (
		b7 7a 5c 56 19 34 e0 89
	)
	.ver 4:0:0:0
}
.assembly extern FSharp.Core
{
	.publickeytoken = (
		b0 3f 5f 7f 11 d5 0a 3a
	)
	.ver 4:4:1:0
}
.assembly ConsoleApplication1
{
	.custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32, int32, int32) = (
		01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00
	)
	.custom instance void [mscorlib]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = (
		01 00 1c 2e 4e 45 54 46 72 61 6d 65 77 6f 72 6b
		2c 56 65 72 73 69 6f 6e 3d 76 34 2e 36 2e 31 01
		00 54 0e 14 46 72 61 6d 65 77 6f 72 6b 44 69 73
		70 6c 61 79 4e 61 6d 65 14 2e 4e 45 54 20 46 72
		61 6d 65 77 6f 72 6b 20 34 2e 36 2e 31
	)
	.custom instance void [mscorlib]System.Reflection.AssemblyTitleAttribute::.ctor(string) = (
		01 00 13 43 6f 6e 73 6f 6c 65 41 70 70 6c 69 63
		61 74 69 6f 6e 31 00 00
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
		01 00 13 43 6f 6e 73 6f 6c 65 41 70 70 6c 69 63
		61 74 69 6f 6e 31 00 00
	)
	.custom instance void [mscorlib]System.Reflection.AssemblyCopyrightAttribute::.ctor(string) = (
		01 00 12 43 6f 70 79 72 69 67 68 74 20 c2 a9 20
		20 32 30 31 37 00 00
	)
	.custom instance void [mscorlib]System.Reflection.AssemblyTrademarkAttribute::.ctor(string) = (
		01 00 00 00 00
	)
	.custom instance void [mscorlib]System.Reflection.AssemblyCultureAttribute::.ctor(string) = (
		01 00 00 00 00
	)
	.custom instance void [mscorlib]System.Runtime.InteropServices.ComVisibleAttribute::.ctor(bool) = (
		01 00 00 00 00
	)
	.custom instance void [mscorlib]System.Runtime.InteropServices.GuidAttribute::.ctor(string) = (
		01 00 24 65 30 36 37 34 66 66 35 2d 35 65 38 66
		2d 34 64 34 65 2d 61 38 38 66 2d 65 34 34 37 31
		39 32 34 35 34 63 37 00 00
	)
	.custom instance void [mscorlib]System.Reflection.AssemblyVersionAttribute::.ctor(string) = (
		01 00 07 31 2e 30 2e 30 2e 30 00 00
	)
	.custom instance void [mscorlib]System.Reflection.AssemblyFileVersionAttribute::.ctor(string) = (
		01 00 07 31 2e 30 2e 30 2e 30 00 00
	)
	.custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = (
		01 00 00 00 00 00 00 00
	)
	.hash algorithm 0x00008004 // SHA1
	.ver 1:0:0:0
}

.module ConsoleApplication1.exe
// MVID: {59F64D28-6A1F-D4CE-A745-0383284DF659}
.corflags 0x00020003 // ILOnly, Required32Bit, Preferred32Bit


.class private auto ansi '<Module>'
	extends [mscorlib]System.Object
{
} // end of class <Module>

.class public auto ansi abstract sealed Program
	extends [mscorlib]System.Object
{
	.custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = (
		01 00 07 00 00 00 00 00
	)
	// Nested Types
	.class nested assembly auto auto sealed specialname serializable beforefieldinit disposable@3
		extends [mscorlib]System.Object
		implements [mscorlib]System.IDisposable
	{
		.custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = (
			01 00 06 00 00 00 00 00
		)
		// Methods
		.method public specialname rtspecialname 
			instance void .ctor () cil managed 
		{
			// Method begins at RVA 0x21bc
			// Code size 9 (0x9)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: callvirt instance void [mscorlib]System.Object::.ctor()
			IL_0006: ldarg.0
			IL_0007: pop
			IL_0008: ret
		} // end of method disposable@3::.ctor

		.method private final hidebysig newslot virtual 
			instance void 'System-IDisposable-Dispose' () cil managed 
		{
			.override method instance void [mscorlib]System.IDisposable::Dispose()
			// Method begins at RVA 0x21c8
			// Code size 1 (0x1)
			.maxstack 8

			IL_0000: ret
		} // end of method disposable@3::'System-IDisposable-Dispose'

	} // end of class disposable@3

	.class nested assembly auto auto sealed specialname serializable beforefieldinit getSeq@5
		extends class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>
	{
		.custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = (
			01 00 06 00 00 00 00 00
		)
		// Fields
		.field public int32 pc
		.custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = (
			01 00 00 00 00 00 00 00
		)
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		.custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = (
			01 00 00 00
		)
		.field public int32 current
		.custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = (
			01 00 00 00 00 00 00 00
		)
		.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
			01 00 00 00
		)
		.custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = (
			01 00 00 00
		)

		// Methods
		.method public specialname rtspecialname 
			instance void .ctor (
				int32 pc,
				int32 current
			) cil managed 
		{
			// Method begins at RVA 0x21cc
			// Code size 21 (0x15)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: ldarg.1
			IL_0002: stfld int32 Program/getSeq@5::pc
			IL_0007: ldarg.0
			IL_0008: ldarg.2
			IL_0009: stfld int32 Program/getSeq@5::current
			IL_000e: ldarg.0
			IL_000f: call instance void class [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.GeneratedSequenceBase`1<int32>::.ctor()
			IL_0014: ret
		} // end of method getSeq@5::.ctor

		.method public strict virtual 
			instance int32 GenerateNext (
				class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>& next
			) cil managed 
		{
			// Method begins at RVA 0x21e4
			// Code size 62 (0x3e)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: ldfld int32 Program/getSeq@5::pc
			IL_0006: ldc.i4.1
			IL_0007: sub
			IL_0008: switch (IL_0018, IL_001b)

			IL_0015: nop
			IL_0016: br.s IL_001e

			IL_0018: nop
			IL_0019: br.s IL_002e

			IL_001b: nop
			IL_001c: br.s IL_0035

			IL_001e: ldarg.0
			IL_001f: ldc.i4.1
			IL_0020: stfld int32 Program/getSeq@5::pc
			IL_0025: ldarg.0
			IL_0026: ldc.i4.1
			IL_0027: stfld int32 Program/getSeq@5::current
			IL_002c: ldc.i4.1
			IL_002d: ret

			IL_002e: ldarg.0
			IL_002f: ldc.i4.2
			IL_0030: stfld int32 Program/getSeq@5::pc

			IL_0035: ldarg.0
			IL_0036: ldc.i4.0
			IL_0037: stfld int32 Program/getSeq@5::current
			IL_003c: ldc.i4.0
			IL_003d: ret
		} // end of method getSeq@5::GenerateNext

		.method public strict virtual 
			instance void Close () cil managed 
		{
			// Method begins at RVA 0x2224
			// Code size 8 (0x8)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: ldc.i4.2
			IL_0002: stfld int32 Program/getSeq@5::pc
			IL_0007: ret
		} // end of method getSeq@5::Close

		.method public strict virtual 
			instance bool get_CheckClose () cil managed 
		{
			// Method begins at RVA 0x2230
			// Code size 39 (0x27)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: ldfld int32 Program/getSeq@5::pc
			IL_0006: switch (IL_001a, IL_001d, IL_0020)

			IL_0017: nop
			IL_0018: br.s IL_0023

			IL_001a: nop
			IL_001b: br.s IL_0025

			IL_001d: nop
			IL_001e: br.s IL_0023

			IL_0020: nop
			IL_0021: br.s IL_0025

			IL_0023: ldc.i4.0
			IL_0024: ret

			IL_0025: ldc.i4.0
			IL_0026: ret
		} // end of method getSeq@5::get_CheckClose

		.method public strict virtual 
			instance int32 get_LastGenerated () cil managed 
		{
			.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
				01 00 00 00
			)
			.custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = (
				01 00 00 00
			)
			// Method begins at RVA 0x2258
			// Code size 7 (0x7)
			.maxstack 8

			IL_0000: ldarg.0
			IL_0001: ldfld int32 Program/getSeq@5::current
			IL_0006: ret
		} // end of method getSeq@5::get_LastGenerated

		.method public strict virtual 
			instance class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> GetFreshEnumerator () cil managed 
		{
			.custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = (
				01 00 00 00
			)
			.custom instance void [mscorlib]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = (
				01 00 00 00
			)
			// Method begins at RVA 0x2260
			// Code size 8 (0x8)
			.maxstack 8

			IL_0000: ldc.i4.0
			IL_0001: ldc.i4.0
			IL_0002: newobj instance void Program/getSeq@5::.ctor(int32, int32)
			IL_0007: ret
		} // end of method getSeq@5::GetFreshEnumerator

	} // end of class getSeq@5


	// Methods
	.method public static 
		class [mscorlib]System.IDisposable disposable () cil managed 
	{
		// Method begins at RVA 0x2050
		// Code size 6 (0x6)
		.maxstack 8

		IL_0000: newobj instance void Program/disposable@3::.ctor()
		IL_0005: ret
	} // end of method Program::disposable

	.method public static 
		class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> getSeq () cil managed 
	{
		// Method begins at RVA 0x2058
		// Code size 8 (0x8)
		.maxstack 8

		IL_0000: ldc.i4.0
		IL_0001: ldc.i4.0
		IL_0002: newobj instance void Program/getSeq@5::.ctor(int32, int32)
		IL_0007: ret
	} // end of method Program::getSeq

	.method public static 
		class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> getList () cil managed 
	{
		// Method begins at RVA 0x2064
		// Code size 12 (0xc)
		.maxstack 8

		IL_0000: ldc.i4.1
		IL_0001: call class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
		IL_0006: call class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0, class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
		IL_000b: ret
	} // end of method Program::getList

	.method public static 
		int32[] getArray () cil managed 
	{
		// Method begins at RVA 0x2074
		// Code size 15 (0xf)
		.maxstack 8

		IL_0000: ldc.i4.1
		IL_0001: newarr [mscorlib]System.Int32
		IL_0006: dup
		IL_0007: ldc.i4.0
		IL_0008: ldc.i4.1
		IL_0009: stelem.any [mscorlib]System.Int32
		IL_000e: ret
	} // end of method Program::getArray

	.method public static 
		int32 main (
			string[] argv
		) cil managed 
	{
		.custom instance void [FSharp.Core]Microsoft.FSharp.Core.EntryPointAttribute::.ctor() = (
			01 00 00 00
		)
		// Method begins at RVA 0x2084
		// Code size 259 (0x103)
		.maxstack 6
		.entrypoint
		.locals init (
			[0] class [mscorlib]System.IDisposable,
			[1] class [mscorlib]System.IDisposable,
			[2] class [mscorlib]System.IDisposable,
			[3] class [mscorlib]System.IDisposable,
			[4] int32,
			[5] class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>,
			[6] class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>,
			[7] class [FSharp.Core]Microsoft.FSharp.Core.Unit,
			[8] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,
			[9] class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>,
			[10] int32,
			[11] int32[]
		)

		IL_0000: call class [mscorlib]System.IDisposable Program::disposable()
		IL_0005: stloc.1
		.try
		{
			IL_0006: ldstr "Hello 1"
			IL_000b: call void [mscorlib]System.Console::WriteLine(string)
			IL_0010: call class [mscorlib]System.IDisposable Program::disposable()
			IL_0015: stloc.2
			IL_0016: leave.s IL_002e
		} // end .try
		finally
		{
			IL_0018: ldloc.1
			IL_0019: isinst [mscorlib]System.IDisposable
			IL_001e: stloc.3
			IL_001f: ldloc.3
			IL_0020: brfalse.s IL_002b

			IL_0022: ldloc.3
			IL_0023: callvirt instance void [mscorlib]System.IDisposable::Dispose()
			IL_0028: ldnull
			IL_0029: pop
			IL_002a: endfinally

			IL_002b: ldnull
			IL_002c: pop
			IL_002d: endfinally
		} // end handler

		IL_002e: ldloc.2
		IL_002f: stloc.0
		.try
		{
			IL_0030: call class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> Program::getSeq()
			IL_0035: stloc.s 5
			IL_0037: ldloc.s 5
			IL_0039: callvirt instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
			IL_003e: stloc.s 6
			.try
			{
				// loop start (head: IL_0040)
					IL_0040: ldloc.s 6
					IL_0042: callvirt instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
					IL_0047: brfalse.s IL_0058

					IL_0049: ldloc.s 6
					IL_004b: callvirt instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
					IL_0050: call void [mscorlib]System.Console::WriteLine(int32)
					IL_0055: nop
					IL_0056: br.s IL_0040
				// end loop

				IL_0058: ldnull
				IL_0059: stloc.s 7
				IL_005b: leave.s IL_0074
			} // end .try
			finally
			{
				IL_005d: ldloc.s 6
				IL_005f: isinst [mscorlib]System.IDisposable
				IL_0064: stloc.1
				IL_0065: ldloc.1
				IL_0066: brfalse.s IL_0071

				IL_0068: ldloc.1
				IL_0069: callvirt instance void [mscorlib]System.IDisposable::Dispose()
				IL_006e: ldnull
				IL_006f: pop
				IL_0070: endfinally

				IL_0071: ldnull
				IL_0072: pop
				IL_0073: endfinally
			} // end handler

			IL_0074: ldloc.s 7
			IL_0076: pop
			IL_0077: ldc.i4.1
			IL_0078: call class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
			IL_007d: call class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0, class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
			IL_0082: stloc.s 8
			IL_0084: ldloc.s 8
			IL_0086: call instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
			IL_008b: stloc.s 9
			// loop start (head: IL_008d)
				IL_008d: ldloc.s 9
				IL_008f: ldnull
				IL_0090: cgt.un
				IL_0092: brfalse.s IL_00b4

				IL_0094: ldloc.s 8
				IL_0096: call instance !0 class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_HeadOrDefault()
				IL_009b: stloc.s 10
				IL_009d: ldloc.s 10
				IL_009f: call void [mscorlib]System.Console::WriteLine(int32)
				IL_00a4: ldloc.s 9
				IL_00a6: stloc.s 8
				IL_00a8: ldloc.s 8
				IL_00aa: call instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_TailOrNull()
				IL_00af: stloc.s 9
				IL_00b1: nop
				IL_00b2: br.s IL_008d
			// end loop

			IL_00b4: ldc.i4.1
			IL_00b5: newarr [mscorlib]System.Int32
			IL_00ba: dup
			IL_00bb: ldc.i4.0
			IL_00bc: ldc.i4.1
			IL_00bd: stelem.any [mscorlib]System.Int32
			IL_00c2: stloc.s 11
			IL_00c4: ldc.i4.0
			IL_00c5: stloc.s 10
			IL_00c7: br.s IL_00dd
			// loop start (head: IL_00dd)
				IL_00c9: ldloc.s 11
				IL_00cb: ldloc.s 10
				IL_00cd: ldelem.any [mscorlib]System.Int32
				IL_00d2: call void [mscorlib]System.Console::WriteLine(int32)
				IL_00d7: ldloc.s 10
				IL_00d9: ldc.i4.1
				IL_00da: add
				IL_00db: stloc.s 10

				IL_00dd: ldloc.s 10
				IL_00df: ldloc.s 11
				IL_00e1: ldlen
				IL_00e2: conv.i4
				IL_00e3: blt.s IL_00c9
			// end loop

			IL_00e5: ldc.i4.0
			IL_00e6: stloc.s 4
			IL_00e8: leave.s IL_0100
		} // end .try
		finally
		{
			IL_00ea: ldloc.0
			IL_00eb: isinst [mscorlib]System.IDisposable
			IL_00f0: stloc.1
			IL_00f1: ldloc.1
			IL_00f2: brfalse.s IL_00fd

			IL_00f4: ldloc.1
			IL_00f5: callvirt instance void [mscorlib]System.IDisposable::Dispose()
			IL_00fa: ldnull
			IL_00fb: pop
			IL_00fc: endfinally

			IL_00fd: ldnull
			IL_00fe: pop
			IL_00ff: endfinally
		} // end handler

		IL_0100: ldloc.s 4
		IL_0102: ret
	} // end of method Program::main

} // end of class Program

.class private auto ansi abstract sealed '<StartupCode$ConsoleApplication1>.$Program'
	extends [mscorlib]System.Object
{
} // end of class <StartupCode$ConsoleApplication1>.$Program

.class private auto ansi abstract sealed '<StartupCode$ConsoleApplication1>.$AssemblyInfo'
	extends [mscorlib]System.Object
{
} // end of class <StartupCode$ConsoleApplication1>.$AssemblyInfo

.class private auto ansi abstract sealed '<StartupCode$ConsoleApplication1>.$.NETFramework,Version=v4.6.1.AssemblyAttributes'
	extends [mscorlib]System.Object
{
} // end of class <StartupCode$ConsoleApplication1>.$.NETFramework,Version=v4.6.1.AssemblyAttributes

