
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly NoArrayInitializers
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module NoArrayInitializers.dll
// MVID: {C1B38171-55C8-4DDA-ACC8-78EFA1168D40}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03D10000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoArrayInitializers
       extends [mscorlib]System.Object
{
  .method public hidebysig instance int32[] 
          LiteralArray() cil managed
  {
    // Code size       23 (0x17)
    .maxstack  3
    .locals init (int32[] V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.3
    IL_0002:  newarr     [mscorlib]System.Int32
    IL_0007:  dup
    IL_0008:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=12' '<PrivateImplementationDetails>'::'4636993D3E1DA4E9D6B8F87B79E8F7C6D018580D52661950EABC3845C5897A4D'
    IL_000d:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0012:  stloc.0
    IL_0013:  br.s       IL_0015

    IL_0015:  ldloc.0
    IL_0016:  ret
  } // end of method NoArrayInitializers::LiteralArray

  .method public hidebysig instance int32[] 
          VariableArray(int32 a,
                        int32 b) cil managed
  {
    // Code size       20 (0x14)
    .maxstack  4
    .locals init (int32[] V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.2
    IL_0002:  newarr     [mscorlib]System.Int32
    IL_0007:  dup
    IL_0008:  ldc.i4.0
    IL_0009:  ldarg.1
    IL_000a:  stelem.i4
    IL_000b:  dup
    IL_000c:  ldc.i4.1
    IL_000d:  ldarg.2
    IL_000e:  stelem.i4
    IL_000f:  stloc.0
    IL_0010:  br.s       IL_0012

    IL_0012:  ldloc.0
    IL_0013:  ret
  } // end of method NoArrayInitializers::VariableArray

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method NoArrayInitializers::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Ugly.NoArrayInitializers

.class private auto ansi sealed '<PrivateImplementationDetails>'
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=12'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 12
  } // end of class '__StaticArrayInitTypeSize=12'

  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=12' '4636993D3E1DA4E9D6B8F87B79E8F7C6D018580D52661950EABC3845C5897A4D' at I_000026C0
} // end of class '<PrivateImplementationDetails>'


// =============================================================

.data cil I_000026C0 = bytearray (
                 01 00 00 00 02 00 00 00 03 00 00 00) 
// *********** DISASSEMBLY COMPLETE ***********************
