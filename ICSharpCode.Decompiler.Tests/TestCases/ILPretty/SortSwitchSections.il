// Test fixture for the SortSwitchSections decompiler setting.
//
// The IL `switch` table targets are placed at non-monotonic offsets,
// simulating obfuscator block-reordering. With SortSwitchSections=false
// (default) the decompiler would emit cases in IL-offset order (3, 0, 4, 1, 2);
// with the setting enabled they are emitted in label-value order (0..4).
// The matching .cs file is the expected output with the setting enabled.

.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
  .ver 4:0:0:0
}
.assembly SortSwitchSections
{
  .ver 1:0:0:0
}
.module SortSwitchSections.dll
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY

.class public auto ansi abstract sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.SortSwitchSections
    extends [mscorlib]System.Object
{
    .method public hidebysig static int32 PickValue(int32 n) cil managed
    {
        .maxstack 1
        IL_0000: ldarg.0
        IL_0001: switch (
            LBL_0,
            LBL_1,
            LBL_2,
            LBL_3,
            LBL_4)
        IL_001a: br.s LBL_DEFAULT

    LBL_3:
        IL_001c: ldc.i4 400
        IL_0021: ret

    LBL_0:
        IL_0022: ldc.i4 100
        IL_0027: ret

    LBL_4:
        IL_0028: ldc.i4 500
        IL_002d: ret

    LBL_1:
        IL_002e: ldc.i4 200
        IL_0033: ret

    LBL_2:
        IL_0034: ldc.i4 300
        IL_0039: ret

    LBL_DEFAULT:
        IL_003a: ldc.i4.m1
        IL_003b: ret
    }
}
