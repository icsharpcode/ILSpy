.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
  .ver 4:0:0:0
}
.assembly Issue1922
{
  .hash algorithm 0x00008004
  .ver 1:0:4059:39717
}
.module Issue1389.dll
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000003    //  ILONLY 32BITREQUIRED

.class public auto ansi beforefieldinit Issue1922.Program
	extends [mscorlib]System.Object
{
	// Methods
  .method public hidebysig static int64 fnWorks (int32 x, int32 y) cil managed 
  {
    .maxstack 8

    ldarg.1
    conv.i8
    ldc.i4 16777215
    conv.i8
    and
    ldc.i4.s 24
    shl
    ldarg.0
    conv.i8
    ldc.i4 16777215
    conv.i8
    and
    or
    ret
  }

  .method public hidebysig static int64 fnFails (int32 x, int32 y) cil managed 
  {
    .maxstack 8

    ldarg.1
    conv.i8
    ldc.i8 16777215
    and
    ldc.i4.s 24
    shl
    ldarg.0
    conv.i8
    ldc.i8 16777215
    and
    or
    ret
  }
} // end of class Issue1922.Program