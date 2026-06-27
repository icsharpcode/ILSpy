// A C++/CLI-style pinned pointer whose pinned region is reachable both inside and outside the
// `fixed` statement. Decompiling it makes DetectPinnedRegions duplicate the shared block and
// record a warning, which is emitted as a comment-carrying EmptyStatement at the top of the
// method body. That statement has no text location, which used to crash PDB generation in
// SequencePointBuilder. Regression fixture for that crash.
.assembly extern System.Runtime { }
.assembly PinnedRegionWarning { }
.module PinnedRegionWarning.dll

.class public auto ansi beforefieldinit C extends [System.Runtime]System.Object
{
  .method public hidebysig instance void PinPtrTest(int32[] a, int32[] b) cil managed
  {
    .maxstack 3
    .locals (
      [0] int32& pinned p
    )
    IL_0000: ldarg.1
    IL_0001: ldc.i4.0
    IL_0002: ldelema [System.Runtime]System.Int32
    IL_0007: stloc.0
    IL_0008: ldloc.0
    IL_0009: ldind.i4
    IL_000a: ldc.i4.0
    IL_000b: ble.s IL_0016

    IL_000d: ldarg.2
    IL_000e: ldloc.0
    IL_000f: ldind.i4
    IL_0010: ldelema [System.Runtime]System.Int32
    IL_0015: stloc.0

    IL_0016: ldloc.0
    IL_0017: ldc.i4.4
    IL_0018: ldc.i4.0
    IL_0019: mul
    IL_001a: add
    IL_001b: ldc.i4.1
    IL_001c: stind.i4
    IL_001d: ret
  }

  .method public hidebysig specialname rtspecialname instance void .ctor() cil managed
  {
    .maxstack 1
    ldarg.0
    call instance void [System.Runtime]System.Object::.ctor()
    ret
  }
}
