
//  .NET IL Disassembler.  Version 10.0.11



// Metadata version: v4.0.30319
.assembly extern System.Runtime
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:0:0:0
}
.assembly extern System.Console
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:0:0:0
}
.assembly ConditionalChain
{
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [System.Runtime]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                                   63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [System.Runtime]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [System.Runtime]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 
}
.module ConditionalChain.dll
// MVID: {db181304-d3cd-4187-9018-312550b7e28d}
.custom instance void [System.Runtime]System.Runtime.CompilerServices.RefSafetyRulesAttribute::.ctor(int32) = ( 01 00 0B 00 00 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x0x7fc5d693d000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ConditionalChain
       extends [System.Runtime]System.Object
{
  .method public hidebysig instance int32 
          Ladder(int32 n,
                 int32[] a,
                 int32[] b) cil managed
  {
    // Code size       58 (0x3a)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.s   40
    IL_0004:  bgt.s      IL_0029

    IL_0006:  ldarg.1
    IL_0007:  ldc.i4.s   30
    IL_0009:  bgt.s      IL_0024

    IL_000b:  ldarg.1
    IL_000c:  ldc.i4.s   20
    IL_000e:  bgt.s      IL_001f

    IL_0010:  ldarg.1
    IL_0011:  ldc.i4.s   10
    IL_0013:  bgt.s      IL_001a

    IL_0015:  ldarg.2
    IL_0016:  ldc.i4.2
    IL_0017:  ldelem.i4
    IL_0018:  br.s       IL_001d

    IL_001a:  ldarg.3
    IL_001b:  ldc.i4.1
    IL_001c:  ldelem.i4
    IL_001d:  br.s       IL_0022

    IL_001f:  ldarg.2
    IL_0020:  ldc.i4.1
    IL_0021:  ldelem.i4
    IL_0022:  br.s       IL_0027

    IL_0024:  ldarg.3
    IL_0025:  ldc.i4.0
    IL_0026:  ldelem.i4
    IL_0027:  br.s       IL_002c

    IL_0029:  ldarg.2
    IL_002a:  ldc.i4.0
    IL_002b:  ldelem.i4
    IL_002c:  stloc.0
    IL_002d:  ldloc.0
    IL_002e:  call       void [System.Console]System.Console::WriteLine(int32)
    IL_0033:  nop
    IL_0034:  ldloc.0
    IL_0035:  stloc.1
    IL_0036:  br.s       IL_0038

    IL_0038:  ldloc.1
    IL_0039:  ret
  } // end of method ConditionalChain::Ladder

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System.Runtime]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method ConditionalChain::.ctor

} // end of class ConditionalChain


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
