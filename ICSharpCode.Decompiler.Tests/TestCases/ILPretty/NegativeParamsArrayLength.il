.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A)
  .ver 4:0:0:0
}
.assembly NegativeParamsArrayLength
{
  .ver 1:0:0:0
}
.module NegativeParamsArrayLength.dll

.class public auto ansi beforefieldinit
       ICSharpCode.Decompiler.Tests.TestCases.ILPretty.NegativeParamsArrayLength
       extends [System.Runtime]System.Object
{
  .method public hidebysig static
          string M() cil managed
  {
    .maxstack  1
    IL_0000:  ldc.i4.m1
    IL_0001:  newarr     [System.Runtime]System.String
    IL_0006:  call       string [System.Runtime]System.String::Concat(string[])
    IL_000b:  ret
  }
}
