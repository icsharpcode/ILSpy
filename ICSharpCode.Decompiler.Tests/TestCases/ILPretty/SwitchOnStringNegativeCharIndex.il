// Regression fixture: real Roslyn length-1 "switch on string" codegen (switch on s[0]) with the
// get_Chars index edited to -1 - a value no compiler emits. Without a negative-index guard the
// transform silently miscompiles this to `switch (s)` even though the IL reads s[-1]; the fixed
// transform declines and preserves the raw `switch (s[-1])`.

.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )
  .ver 4:0:0:0
}
.assembly SwitchOnStringNegativeCharIndex
{
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}
.module SwitchOnStringNegativeCharIndex.dll

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.SwitchOnStringNegativeCharIndex
       extends [mscorlib]System.Object
{
  .method public hidebysig static int32  M(string s) cil managed
  {
    
    .maxstack  2
    .locals init (int32 V_0,
             char V_1)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0051

    IL_0003:  ldarg.0
    IL_0004:  call       instance int32 [mscorlib]System.String::get_Length()
    IL_0009:  stloc.0
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.1
    IL_000c:  bne.un.s   IL_0051

    IL_000e:  ldarg.0
    IL_000f:  ldc.i4.m1
    IL_0010:  call       instance char [mscorlib]System.String::get_Chars(int32)
    IL_0015:  stloc.1
    IL_0016:  ldloc.1
    IL_0017:  ldc.i4.s   97
    IL_0019:  sub
    IL_001a:  switch     ( 
                          IL_0041,
                          IL_0043,
                          IL_0045,
                          IL_0047,
                          IL_0049,
                          IL_004b,
                          IL_004d,
                          IL_004f)
    IL_003f:  br.s       IL_0051

    IL_0041:  ldc.i4.1
    IL_0042:  ret

    IL_0043:  ldc.i4.2
    IL_0044:  ret

    IL_0045:  ldc.i4.3
    IL_0046:  ret

    IL_0047:  ldc.i4.4
    IL_0048:  ret

    IL_0049:  ldc.i4.5
    IL_004a:  ret

    IL_004b:  ldc.i4.6
    IL_004c:  ret

    IL_004d:  ldc.i4.7
    IL_004e:  ret

    IL_004f:  ldc.i4.8
    IL_0050:  ret

    IL_0051:  ldc.i4.0
    IL_0052:  ret
  } // end of method M

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method .ctor

} // end of class SwitchOnStringNegativeCharIndex

