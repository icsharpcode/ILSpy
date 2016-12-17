
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly b1jpssom
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module b1jpssom.dll
// MVID: {7C6502B0-8536-4BED-9395-765EB2F77653}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02B00000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest
       extends [mscorlib]System.Object
{
  .field private int32 field1
  .field private static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest field2
  .field private int32[] field3
  .method public hidebysig instance void 
          SimpleInlineWithLocals() cil managed
  {
    // Code size       33 (0x21)
    .maxstack  2
    .locals init (int32 V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest V_1)
    IL_0000:  ldc.i4.5
    IL_0001:  dup
    IL_0002:  stloc.0
    IL_0003:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0008:  ldloc.0
    IL_0009:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::.ctor()
    IL_0013:  dup
    IL_0014:  stloc.1
    IL_0015:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_001a:  ldloc.1
    IL_001b:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0020:  ret
  } // end of method InlineAssignmentTest::SimpleInlineWithLocals

  .method public hidebysig instance void 
          SimpleInlineWithFields() cil managed
  {
    // Code size       32 (0x20)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.5
    IL_0002:  dup
    IL_0003:  stloc.0
    IL_0004:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field1
    IL_0009:  ldloc.0
    IL_000a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000f:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::.ctor()
    IL_0014:  dup
    IL_0015:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field2
    IL_001a:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_001f:  ret
  } // end of method InlineAssignmentTest::SimpleInlineWithFields

  .method public hidebysig instance void 
          SimpleInlineWithFields2() cil managed
  {
    // Code size       53 (0x35)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.5
    IL_0002:  dup
    IL_0003:  stloc.0
    IL_0004:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field1
    IL_0009:  ldloc.0
    IL_000a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000f:  ldarg.0
    IL_0010:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field1
    IL_0015:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001a:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::.ctor()
    IL_001f:  dup
    IL_0020:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field2
    IL_0025:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_002a:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field2
    IL_002f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0034:  ret
  } // end of method InlineAssignmentTest::SimpleInlineWithFields2

  .method public hidebysig instance void 
          AccessArray(int32[] a) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  4
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.0
    IL_0002:  ldelem.i4
    IL_0003:  dup
    IL_0004:  stloc.0
    IL_0005:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000a:  ldarg.1
    IL_000b:  ldloc.0
    IL_000c:  ldloc.0
    IL_000d:  dup
    IL_000e:  stloc.1
    IL_000f:  stelem.i4
    IL_0010:  ldloc.1
    IL_0011:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0016:  ret
  } // end of method InlineAssignmentTest::AccessArray

  .method public hidebysig instance int32 
          Return(int32& a) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.3
    IL_0002:  dup
    IL_0003:  stloc.0
    IL_0004:  stind.i4
    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method InlineAssignmentTest::Return

  .method public hidebysig instance int32 
          Array(int32[] a,
                int32 i) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldarg.2
    IL_0003:  dup
    IL_0004:  stloc.0
    IL_0005:  stelem.i4
    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method InlineAssignmentTest::Array

  .method public hidebysig instance int32 
          Array2(int32 i) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field3
    IL_0006:  ldarg.1
    IL_0007:  ldc.i4.1
    IL_0008:  dup
    IL_0009:  stloc.0
    IL_000a:  stelem.i4
    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method InlineAssignmentTest::Array2

  .method public hidebysig instance int32 
          GetIndex() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  newobj     instance void [mscorlib]System.Random::.ctor()
    IL_0005:  ldc.i4.0
    IL_0006:  ldc.i4.s   100
    IL_0008:  callvirt   instance int32 [mscorlib]System.Random::Next(int32,
                                                                      int32)
    IL_000d:  ret
  } // end of method InlineAssignmentTest::GetIndex

  .method public hidebysig instance int32[] 
          GetArray() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0005:  throw
  } // end of method InlineAssignmentTest::GetArray

  .method public hidebysig instance int32 
          GetValue(int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ret
  } // end of method InlineAssignmentTest::GetValue

  .method public hidebysig instance int32 
          ArrayUsageWithMethods() cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetArray()
    IL_0006:  ldarg.0
    IL_0007:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetIndex()
    IL_000c:  ldarg.0
    IL_000d:  ldarg.0
    IL_000e:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetIndex()
    IL_0013:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetValue(int32)
    IL_0018:  dup
    IL_0019:  stloc.0
    IL_001a:  stelem.i4
    IL_001b:  ldloc.0
    IL_001c:  ret
  } // end of method InlineAssignmentTest::ArrayUsageWithMethods

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method InlineAssignmentTest::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../Tests/TestCases/Pretty\InlineAssignmentTest.opt.res
