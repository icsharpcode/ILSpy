// Regression fixture for rejecting a negative ReadOnlySpan length before unsafe pointer access.
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )
  .ver 4:0:0:0
}
.assembly NegativeReadOnlySpanLength {}

.class public ICSharpCode.Decompiler.Tests.TestCases.ILPretty.NegativeReadOnlySpanLength extends [System.Runtime]System.Object
{
  // A field with HasFieldRVA and no data gives GetInitialValue an empty BlobReader.
  .field flags(0x0133) int32 F

  .method public hidebysig static valuetype [System.Runtime]System.ReadOnlySpan`1<uint8> M() cil managed
  {
    ldsflda int32 ICSharpCode.Decompiler.Tests.TestCases.ILPretty.NegativeReadOnlySpanLength::F
    ldc.i4.m1
    newobj instance void valuetype [System.Runtime]System.ReadOnlySpan`1<uint8>::.ctor(void*, int32)
    ret
  }
}
