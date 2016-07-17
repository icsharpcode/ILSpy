
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly kb1brdcl
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module kb1brdcl.exe
// MVID: {FA9A1DD3-2F44-4DB4-BC5E-6EF79C457314}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x01190000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest
       extends [mscorlib]System.Object
{
  .class sequential ansi sealed nested private beforefieldinit StructContainer
         extends [mscorlib]System.ValueType
  {
    .field public bool HasIndex
    .field public int32 Field
  } // end of class StructContainer

  .field private int32 test1
  .field private int32[] array1
  .field private valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer field1
  .method public hidebysig static void  Main() cil managed
  {
    .entrypoint
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method CompoundAssignmentTest::Main

  .method public hidebysig instance void 
          Int32_Local_Add(int32 i) cil managed
  {
    // Code size       48 (0x30)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.1
    IL_0003:  add
    IL_0004:  starg.s    i
    IL_0006:  ldarg.1
    IL_0007:  dup
    IL_0008:  ldc.i4.1
    IL_0009:  add
    IL_000a:  starg.s    i
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0011:  nop
    IL_0012:  ldarg.1
    IL_0013:  ldc.i4.1
    IL_0014:  add
    IL_0015:  dup
    IL_0016:  starg.s    i
    IL_0018:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001d:  nop
    IL_001e:  ldarg.1
    IL_001f:  ldc.i4.5
    IL_0020:  add
    IL_0021:  starg.s    i
    IL_0023:  ldarg.1
    IL_0024:  ldc.i4.5
    IL_0025:  add
    IL_0026:  dup
    IL_0027:  starg.s    i
    IL_0029:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_002e:  nop
    IL_002f:  ret
  } // end of method CompoundAssignmentTest::Int32_Local_Add

  .method public hidebysig instance void 
          Int32_Local_Sub(int32 i) cil managed
  {
    // Code size       48 (0x30)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.1
    IL_0003:  sub
    IL_0004:  starg.s    i
    IL_0006:  ldarg.1
    IL_0007:  dup
    IL_0008:  ldc.i4.1
    IL_0009:  sub
    IL_000a:  starg.s    i
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0011:  nop
    IL_0012:  ldarg.1
    IL_0013:  ldc.i4.1
    IL_0014:  sub
    IL_0015:  dup
    IL_0016:  starg.s    i
    IL_0018:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001d:  nop
    IL_001e:  ldarg.1
    IL_001f:  ldc.i4.5
    IL_0020:  sub
    IL_0021:  starg.s    i
    IL_0023:  ldarg.1
    IL_0024:  ldc.i4.5
    IL_0025:  sub
    IL_0026:  dup
    IL_0027:  starg.s    i
    IL_0029:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_002e:  nop
    IL_002f:  ret
  } // end of method CompoundAssignmentTest::Int32_Local_Sub

  .method public hidebysig instance void 
          Int32_Local_Mul(int32 i) cil managed
  {
    // Code size       19 (0x13)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.5
    IL_0003:  mul
    IL_0004:  starg.s    i
    IL_0006:  ldarg.1
    IL_0007:  ldc.i4.5
    IL_0008:  mul
    IL_0009:  dup
    IL_000a:  starg.s    i
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0011:  nop
    IL_0012:  ret
  } // end of method CompoundAssignmentTest::Int32_Local_Mul

  .method public hidebysig instance void 
          Int32_Local_Div(int32 i) cil managed
  {
    // Code size       19 (0x13)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.5
    IL_0003:  div
    IL_0004:  starg.s    i
    IL_0006:  ldarg.1
    IL_0007:  ldc.i4.5
    IL_0008:  div
    IL_0009:  dup
    IL_000a:  starg.s    i
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0011:  nop
    IL_0012:  ret
  } // end of method CompoundAssignmentTest::Int32_Local_Div

  .method public hidebysig instance void 
          Int32_Local_Rem(int32 i) cil managed
  {
    // Code size       19 (0x13)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.5
    IL_0003:  rem
    IL_0004:  starg.s    i
    IL_0006:  ldarg.1
    IL_0007:  ldc.i4.5
    IL_0008:  rem
    IL_0009:  dup
    IL_000a:  starg.s    i
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0011:  nop
    IL_0012:  ret
  } // end of method CompoundAssignmentTest::Int32_Local_Rem

  .method public hidebysig instance void 
          Int32_Local_BitAnd(int32 i) cil managed
  {
    // Code size       19 (0x13)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.5
    IL_0003:  and
    IL_0004:  starg.s    i
    IL_0006:  ldarg.1
    IL_0007:  ldc.i4.5
    IL_0008:  and
    IL_0009:  dup
    IL_000a:  starg.s    i
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0011:  nop
    IL_0012:  ret
  } // end of method CompoundAssignmentTest::Int32_Local_BitAnd

  .method public hidebysig instance void 
          Int32_Local_BitOr(int32 i) cil managed
  {
    // Code size       19 (0x13)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.5
    IL_0003:  or
    IL_0004:  starg.s    i
    IL_0006:  ldarg.1
    IL_0007:  ldc.i4.5
    IL_0008:  or
    IL_0009:  dup
    IL_000a:  starg.s    i
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0011:  nop
    IL_0012:  ret
  } // end of method CompoundAssignmentTest::Int32_Local_BitOr

  .method public hidebysig instance void 
          Int32_Local_BitXor(int32 i) cil managed
  {
    // Code size       19 (0x13)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.5
    IL_0003:  xor
    IL_0004:  starg.s    i
    IL_0006:  ldarg.1
    IL_0007:  ldc.i4.5
    IL_0008:  xor
    IL_0009:  dup
    IL_000a:  starg.s    i
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0011:  nop
    IL_0012:  ret
  } // end of method CompoundAssignmentTest::Int32_Local_BitXor

  .method public hidebysig instance void 
          Int32_Local_ShiftLeft(int32 i) cil managed
  {
    // Code size       19 (0x13)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.5
    IL_0003:  shl
    IL_0004:  starg.s    i
    IL_0006:  ldarg.1
    IL_0007:  ldc.i4.5
    IL_0008:  shl
    IL_0009:  dup
    IL_000a:  starg.s    i
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0011:  nop
    IL_0012:  ret
  } // end of method CompoundAssignmentTest::Int32_Local_ShiftLeft

  .method public hidebysig instance void 
          Int32_Local_ShiftRight(int32 i) cil managed
  {
    // Code size       19 (0x13)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.5
    IL_0003:  shr
    IL_0004:  starg.s    i
    IL_0006:  ldarg.1
    IL_0007:  ldc.i4.5
    IL_0008:  shr
    IL_0009:  dup
    IL_000a:  starg.s    i
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0011:  nop
    IL_0012:  ret
  } // end of method CompoundAssignmentTest::Int32_Local_ShiftRight

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

  .method public hidebysig instance void 
          Array(int32 i) cil managed
  {
    // Code size       74 (0x4a)
    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::array1
    IL_0007:  ldarg.1
    IL_0008:  ldelema    [mscorlib]System.Int32
    IL_000d:  dup
    IL_000e:  ldobj      [mscorlib]System.Int32
    IL_0013:  ldarg.1
    IL_0014:  add
    IL_0015:  dup
    IL_0016:  stloc.0
    IL_0017:  stobj      [mscorlib]System.Int32
    IL_001c:  ldloc.0
    IL_001d:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0022:  nop
    IL_0023:  ldarg.0
    IL_0024:  ldfld      int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::array1
    IL_0029:  ldarg.1
    IL_002a:  ldc.i4.2
    IL_002b:  mul
    IL_002c:  ldelema    [mscorlib]System.Int32
    IL_0031:  dup
    IL_0032:  ldobj      [mscorlib]System.Int32
    IL_0037:  ldarg.1
    IL_0038:  ldc.i4.2
    IL_0039:  mul
    IL_003a:  add
    IL_003b:  dup
    IL_003c:  stloc.0
    IL_003d:  stobj      [mscorlib]System.Int32
    IL_0042:  ldloc.0
    IL_0043:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0048:  nop
    IL_0049:  ret
  } // end of method CompoundAssignmentTest::Array

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
  } // end of method CompoundAssignmentTest::GetIndex

  .method public hidebysig instance int32[] 
          GetArray() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0006:  throw
  } // end of method CompoundAssignmentTest::GetArray

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
  } // end of method CompoundAssignmentTest::GetValue

  .method public hidebysig instance int32 
          ArrayUsageWithMethods() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::GetArray()
    IL_0007:  ldarg.0
    IL_0008:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::GetIndex()
    IL_000d:  ldelema    [mscorlib]System.Int32
    IL_0012:  dup
    IL_0013:  ldobj      [mscorlib]System.Int32
    IL_0018:  dup
    IL_0019:  stloc.1
    IL_001a:  ldc.i4.1
    IL_001b:  add
    IL_001c:  stobj      [mscorlib]System.Int32
    IL_0021:  ldloc.1
    IL_0022:  stloc.0
    IL_0023:  br.s       IL_0025

    IL_0025:  ldloc.0
    IL_0026:  ret
  } // end of method CompoundAssignmentTest::ArrayUsageWithMethods

  .method public hidebysig instance void 
          NestedField() cil managed
  {
    // Code size       50 (0x32)
    .maxstack  3
    .locals init (bool V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::field1
    IL_0007:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer::HasIndex
    IL_000c:  ldc.i4.0
    IL_000d:  ceq
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  brtrue.s   IL_0031

    IL_0013:  nop
    IL_0014:  ldarg.0
    IL_0015:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::field1
    IL_001a:  dup
    IL_001b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer::Field
    IL_0020:  dup
    IL_0021:  stloc.1
    IL_0022:  ldc.i4.1
    IL_0023:  add
    IL_0024:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer::Field
    IL_0029:  ldloc.1
    IL_002a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_002f:  nop
    IL_0030:  nop
    IL_0031:  ret
  } // end of method CompoundAssignmentTest::NestedField

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
