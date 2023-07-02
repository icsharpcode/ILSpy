.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
  .ver 4:0:0:0
}
.assembly MonoFixed
{
  .ver 1:0:0:0
}

.module MonoFixed.exe
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003 // WindowsCui
.corflags 0x00000001 // ILOnly
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = (
	01 00 00 00
)

.class public auto ansi beforefieldinit MonoFixed
	   extends [mscorlib]System.Object
{
  .method public hidebysig instance void FixMultipleStrings (string text) cil managed
	{
		.maxstack 7
		.locals init (
			[0] char* pinned,
			[1] char* pinned,
			[2] char* pinned,
			[3] string pinned,
			[4] string pinned,
			[5] string pinned
		)

		IL_0000: ldarg.1
		IL_0001: stloc.3
		IL_0002: ldloc.3
		IL_0003: conv.i
		IL_0004: call int32 [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::get_OffsetToStringData()
		IL_0009: add
		IL_000a: stloc.0
		IL_000b: call string [mscorlib]System.Environment::get_UserName()
		IL_0010: stloc.s 4
		IL_0012: ldloc.s 4
		IL_0014: conv.i
		IL_0015: call int32 [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::get_OffsetToStringData()
		IL_001a: add
		IL_001b: stloc.1
		IL_001c: ldarg.1
		IL_001d: stloc.s 5
		IL_001f: ldloc.s 5
		IL_0021: conv.i
		IL_0022: call int32 [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::get_OffsetToStringData()
		IL_0027: add
		IL_0028: stloc.2
		IL_0029: ldloc.0
		IL_002a: ldc.i4.s 99
		IL_002c: stind.i2
		IL_002d: ldloc.1
		IL_002e: ldc.i4.s 100
		IL_0030: stind.i2
		IL_0031: ldloc.2
		IL_0032: ldc.i4.s 101
		IL_0034: stind.i2
		IL_0035: ldnull
		IL_0036: stloc.3
		IL_0037: ldnull
		IL_0038: stloc.s 4
		IL_003a: ldnull
		IL_003b: stloc.s 5
		IL_003d: ret
	}
}
