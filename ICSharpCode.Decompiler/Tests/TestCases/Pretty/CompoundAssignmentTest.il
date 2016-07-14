
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly baqif21x
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module baqif21x.exe
// MVID: {1149F1EE-8A3E-4309-9B0E-D81380AC971E}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x011D0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest
       extends [mscorlib]System.Object
{
  .field private int32 test1
  .method public hidebysig static void  Main() cil managed
  {
    .entrypoint
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method CompoundAssignmentTest::Main

  .method public hidebysig instance void 
          Integer(int32 i) cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  ldarg.1
    IL_0004:  ldc.i4.5
    IL_0005:  cgt
    IL_0007:  ldc.i4.0
    IL_0008:  ceq
    IL_000a:  stloc.1
    IL_000b:  ldloc.1
    IL_000c:  brtrue.s   IL_0016

    IL_000e:  nop
    IL_000f:  ldloc.0
    IL_0010:  ldarg.1
    IL_0011:  add
    IL_0012:  stloc.0
    IL_0013:  nop
    IL_0014:  br.s       IL_001c

    IL_0016:  nop
    IL_0017:  ldloc.0
    IL_0018:  ldarg.1
    IL_0019:  sub
    IL_001a:  stloc.0
    IL_001b:  nop
    IL_001c:  ret
  } // end of method CompoundAssignmentTest::Integer

  .method public hidebysig instance void 
          IntegerWithInline(int32 i) cil managed
  {
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.5
    IL_0003:  add
    IL_0004:  dup
    IL_0005:  starg.s    i
    IL_0007:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000c:  nop
    IL_000d:  ldarg.1
    IL_000e:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0013:  nop
    IL_0014:  ret
  } // end of method CompoundAssignmentTest::IntegerWithInline

  .method public hidebysig instance void 
          IntegerField(int32 i) cil managed
  {
    // Code size       72 (0x48)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  dup
    IL_0003:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::test1
    IL_0008:  ldarg.1
    IL_0009:  add
    IL_000a:  dup
    IL_000b:  stloc.0
    IL_000c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::test1
    IL_0011:  ldloc.0
    IL_0012:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0017:  nop
    IL_0018:  ldarg.0
    IL_0019:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::test1
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0023:  nop
    IL_0024:  ldarg.0
    IL_0025:  dup
    IL_0026:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::test1
    IL_002b:  ldarg.1
    IL_002c:  sub
    IL_002d:  dup
    IL_002e:  stloc.0
    IL_002f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::test1
    IL_0034:  ldloc.0
    IL_0035:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_003a:  nop
    IL_003b:  ldarg.0
    IL_003c:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::test1
    IL_0041:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0046:  nop
    IL_0047:  ret
  } // end of method CompoundAssignmentTest::IntegerField

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method CompoundAssignmentTest::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../Tests/TestCases/Pretty\CompoundAssignmentTest.res
