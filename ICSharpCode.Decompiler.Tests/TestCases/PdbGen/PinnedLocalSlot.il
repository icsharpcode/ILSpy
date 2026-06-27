// A simple pinned pointer (fixed statement). The metadata local signature types the slot as
// `int32& pinned`, but the decompiler models a pinned-region local as a pointer (`int*`). The two
// never match, yet the slot index is faithful to the IL ldloc/stloc operand, so PDB generation
// must not assert on the type difference. Regression fixture for that assertion.
.assembly extern System.Runtime { }
.assembly PinnedLocalSlot { }
.module PinnedLocalSlot.dll

.class public auto ansi beforefieldinit C extends [System.Runtime]System.Object
{
  .method public hidebysig instance int32 M(int32[] a) cil managed
  {
    .maxstack 2
    .locals ( [0] int32& pinned p )
    ldarg.1
    ldc.i4.0
    ldelema [System.Runtime]System.Int32
    stloc.0
    ldloc.0
    ldind.i4
    ldc.i4.0
    conv.u
    stloc.0
    ret
  }

  .method public hidebysig specialname rtspecialname instance void .ctor() cil managed
  {
    .maxstack 1
    ldarg.0
    call instance void [System.Runtime]System.Object::.ctor()
    ret
  }
}
