// Regression fixture for the CachedReadOnlySpanInitialization transform.
//
// This is real Roslyn codegen (net6.0, /optimize) for
//     public static ReadOnlySpan<char> NewLine => new char[] { '\r', '\n' };
// On target frameworks without RuntimeHelpers.CreateSpan (pre-.NET 7, or netstandard2.0 +
// System.Memory) Roslyn caches the backing array in a lazy <PrivateImplementationDetails> static
// field. Without the transform the decompiled output references that compiler-synthesized type,
// whose escaped name is not expressible in C# and is never declared, so it fails to recompile.
// The transform collapses the cache back to the ReadOnlySpan constructor.

.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )
  .ver 4:0:0:0
}
.assembly CachedReadOnlySpanFromLazyCache
{
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module CachedReadOnlySpanFromLazyCache.dll

.class public abstract auto ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.CachedReadOnlySpanFromLazyCache
       extends [System.Runtime]System.Object
{
  .method public hidebysig specialname static
          valuetype [System.Runtime]System.ReadOnlySpan`1<char>
          get_NewLine() cil managed
  {
    .maxstack  8
    IL_0000:  ldsfld     char[] '<PrivateImplementationDetails>'::B7F560303EE2CCA55615B53FCFF87C6AB2C55F9E71A6CEA93C61B572213E7075_A1
    IL_0005:  dup
    IL_0006:  brtrue.s   IL_0020

    IL_0008:  pop
    IL_0009:  ldc.i4.2
    IL_000a:  newarr     [System.Runtime]System.Char
    IL_000f:  dup
    IL_0010:  ldtoken    field int32 '<PrivateImplementationDetails>'::B7F560303EE2CCA55615B53FCFF87C6AB2C55F9E71A6CEA93C61B572213E7075
    IL_0015:  call       void [System.Runtime]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [System.Runtime]System.Array,
                                                                                                              valuetype [System.Runtime]System.RuntimeFieldHandle)
    IL_001a:  dup
    IL_001b:  stsfld     char[] '<PrivateImplementationDetails>'::B7F560303EE2CCA55615B53FCFF87C6AB2C55F9E71A6CEA93C61B572213E7075_A1
    IL_0020:  newobj     instance void valuetype [System.Runtime]System.ReadOnlySpan`1<char>::.ctor(!0[])
    IL_0025:  ret
  }

  .property valuetype [System.Runtime]System.ReadOnlySpan`1<char> NewLine()
  {
    .get valuetype [System.Runtime]System.ReadOnlySpan`1<char> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.CachedReadOnlySpanFromLazyCache::get_NewLine()
  }
}

.class private auto ansi sealed '<PrivateImplementationDetails>'
       extends [System.Runtime]System.Object
{
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 )
  .field static assembly initonly int32 B7F560303EE2CCA55615B53FCFF87C6AB2C55F9E71A6CEA93C61B572213E7075 at I_00002870
  .field static assembly char[] B7F560303EE2CCA55615B53FCFF87C6AB2C55F9E71A6CEA93C61B572213E7075_A1
}

.data cil I_00002870 = bytearray (
                 0D 00 0A 00)
