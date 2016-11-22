
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.18020
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly ibag1kve
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module ibag1kve.dll
// MVID: {C798271E-70DB-47E6-B4B1-8CCE369C32D9}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x01380000


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
    // Code size       38 (0x26)
    .maxstack  2
    .locals init (int32 V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.5
    IL_0002:  dup
    IL_0003:  stloc.0
    IL_0004:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0009:  nop
    IL_000a:  ldloc.0
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0010:  nop
    IL_0011:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::.ctor()
    IL_0016:  dup
    IL_0017:  stloc.1
    IL_0018:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_001d:  nop
    IL_001e:  ldloc.1
    IL_001f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0024:  nop
    IL_0025:  ret
  } // end of method InlineAssignmentTest::SimpleInlineWithLocals

  .method public hidebysig instance void 
          SimpleInlineWithFields() cil managed
  {
    // Code size       35 (0x23)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.5
    IL_0003:  dup
    IL_0004:  stloc.0
    IL_0005:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field1
    IL_000a:  ldloc.0
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0010:  nop
    IL_0011:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::.ctor()
    IL_0016:  dup
    IL_0017:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field2
    IL_001c:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0021:  nop
    IL_0022:  ret
  } // end of method InlineAssignmentTest::SimpleInlineWithFields

  .method public hidebysig instance void 
          SimpleInlineWithFields2() cil managed
  {
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.5
    IL_0003:  dup
    IL_0004:  stloc.0
    IL_0005:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field1
    IL_000a:  ldloc.0
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0010:  nop
    IL_0011:  ldarg.0
    IL_0012:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field1
    IL_0017:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001c:  nop
    IL_001d:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::.ctor()
    IL_0022:  dup
    IL_0023:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field2
    IL_0028:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_002d:  nop
    IL_002e:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field2
    IL_0033:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0038:  nop
    IL_0039:  ret
  } // end of method InlineAssignmentTest::SimpleInlineWithFields2

  .method public hidebysig instance void 
          AccessArray(int32[] a) cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  4
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.0
    IL_0003:  ldelem.i4
    IL_0004:  dup
    IL_0005:  stloc.0
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000b:  nop
    IL_000c:  ldarg.1
    IL_000d:  ldloc.0
    IL_000e:  ldloc.0
    IL_000f:  dup
    IL_0010:  stloc.1
    IL_0011:  stelem.i4
    IL_0012:  ldloc.1
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0018:  nop
    IL_0019:  ret
  } // end of method InlineAssignmentTest::AccessArray

  .method public hidebysig instance int32 
          Return(int32& a) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.3
    IL_0003:  dup
    IL_0004:  stloc.1
    IL_0005:  stind.i4
    IL_0006:  ldloc.1
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method InlineAssignmentTest::Return

  .method public hidebysig instance int32 
          Array(int32[] a,
                int32 i) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  4
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldarg.2
    IL_0004:  dup
    IL_0005:  stloc.1
    IL_0006:  stelem.i4
    IL_0007:  ldloc.1
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method InlineAssignmentTest::Array

  .method public hidebysig instance int32 
          Array2(int32 i) cil managed
  {
    // Code size       18 (0x12)
    .maxstack  4
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field3
    IL_0007:  ldarg.1
    IL_0008:  ldc.i4.1
    IL_0009:  dup
    IL_000a:  stloc.1
    IL_000b:  stelem.i4
    IL_000c:  ldloc.1
    IL_000d:  stloc.0
    IL_000e:  br.s       IL_0010

    IL_0010:  ldloc.0
    IL_0011:  ret
  } // end of method InlineAssignmentTest::Array2

  .method public hidebysig instance int32 
          GetIndex() cil managed
  {
    // Code size       19 (0x13)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  newobj     instance void [mscorlib]System.Random::.ctor()
    IL_0006:  ldc.i4.0
    IL_0007:  ldc.i4.s   100
    IL_0009:  callvirt   instance int32 [mscorlib]System.Random::Next(int32,
                                                                      int32)
    IL_000e:  stloc.0
    IL_000f:  br.s       IL_0011

    IL_0011:  ldloc.0
    IL_0012:  ret
  } // end of method InlineAssignmentTest::GetIndex

  .method public hidebysig instance int32[] 
          GetArray() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0006:  throw
  } // end of method InlineAssignmentTest::GetArray

  .method public hidebysig instance int32 
          GetValue(int32 'value') cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method InlineAssignmentTest::GetValue

  .method public hidebysig instance int32 
          ArrayUsageWithMethods() cil managed
  {
    // Code size       34 (0x22)
    .maxstack  4
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetArray()
    IL_0007:  ldarg.0
    IL_0008:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetIndex()
    IL_000d:  ldarg.0
    IL_000e:  ldarg.0
    IL_000f:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetIndex()
    IL_0014:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::GetValue(int32)
    IL_0019:  dup
    IL_001a:  stloc.1
    IL_001b:  stelem.i4
    IL_001c:  ldloc.1
    IL_001d:  stloc.0
    IL_001e:  br.s       IL_0020

    IL_0020:  ldloc.0
    IL_0021:  ret
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
// WARNING: Created Win32 resource file ../../Tests/TestCases/Pretty\InlineAssignmentTest.res
