// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly ConstantBlobs
{
  .ver 1:0:0:0
}
.module ConstantBlobs.exe
// MVID: {B973FCD6-A9C4-48A9-8291-26DDC248E208}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00020003    //  ILONLY 32BITPREFERRED
// Image base: 0x000001C4B6C90000

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.ConstantBlobs
       extends [mscorlib]System.Object
{
.method public hidebysig static void Float_Int32(
            [opt] float32 f1,
            [opt] float32 f2,
            [opt] float32 f3,
            [opt] float32 f4
        ) cil managed
{
  .param [1] = int32(0)
  .param [2] = int32(-1)
  .param [3] = int32(2147483647)
  .param [4] = int32(-2147483648)
  
  // Code size 1 (0x1)
  .maxstack  8
	IL_0000: ret
} // end of method Program::Float_Int32
}