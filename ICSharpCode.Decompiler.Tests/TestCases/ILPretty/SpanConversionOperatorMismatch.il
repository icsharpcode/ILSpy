// Regression fixture: an implicit span conversion must not be folded into a call to a conversion
// operator that does not perform it.
//
// C# cannot declare 'implicit operator ReadOnlySpan<char>(object)' - neither operand type is the
// declaring type - but IL can. An implicit span conversion from string to ReadOnlySpan<char> does
// exist, yet the compiler emits it as MemoryExtensions.AsSpan, not as this operator, so the call
// below is a user-defined conversion and the cast to its parameter type has to survive.

.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )
  .ver 4:0:0:0
}
.assembly SpanConversionOperatorMismatch
{
  .custom instance void [System.Runtime]System.Runtime.Versioning.TargetFrameworkAttribute::.ctor(string) = {
      string('.NETCoreApp,Version=11.0')
  }
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module SpanConversionOperatorMismatch.dll

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.SpanConversionOperatorMismatch
       extends [System.Runtime]System.Object
{
  .method public hidebysig specialname static
          valuetype [System.Runtime]System.ReadOnlySpan`1<char>
          op_Implicit(object o) cil managed
  {
    .maxstack  1
    .locals init (valuetype [System.Runtime]System.ReadOnlySpan`1<char> V_0)
    IL_0000:  ldloca.s   V_0
    IL_0002:  initobj    valuetype [System.Runtime]System.ReadOnlySpan`1<char>
    IL_0008:  ldloc.0
    IL_0009:  ret
  }

  .method public hidebysig static
          valuetype [System.Runtime]System.ReadOnlySpan`1<char>
          ConvertString(string s) cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       valuetype [System.Runtime]System.ReadOnlySpan`1<char> ICSharpCode.Decompiler.Tests.TestCases.ILPretty.SpanConversionOperatorMismatch::op_Implicit(object)
    IL_0006:  ret
  }

  .method public hidebysig specialname rtspecialname
          instance void  .ctor() cil managed
  {
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
    IL_0006:  ret
  }
}
