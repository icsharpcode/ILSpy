
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly CompoundAssignmentTest
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
.module CompoundAssignmentTest.dll
// MVID: {572A9208-925A-4E92-9D86-8EC36FE271FC}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00AF0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest
       extends [mscorlib]System.Object
{
  .class auto ansi sealed nested private MyEnum
         extends [mscorlib]System.Enum
  {
    .custom instance void [mscorlib]System.FlagsAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public specialname rtspecialname int32 value__
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum None = int32(0x00000000)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum One = int32(0x00000001)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum Two = int32(0x00000002)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum Four = int32(0x00000004)
  } // end of class MyEnum

  .class sequential ansi sealed nested private beforefieldinit StructContainer
         extends [mscorlib]System.ValueType
  {
    .field public bool HasIndex
    .field public int32 Field
  } // end of class StructContainer

  .class auto ansi nested public beforefieldinit MutableClass
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
    .field public int32 Field
    .field private int32 '<Property>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public hidebysig specialname 
            instance int32  get_Property() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::'<Property>k__BackingField'
      IL_0006:  ret
    } // end of method MutableClass::get_Property

    .method public hidebysig specialname 
            instance void  set_Property(int32 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::'<Property>k__BackingField'
      IL_0007:  ret
    } // end of method MutableClass::set_Property

    .method public hidebysig specialname 
            instance uint32  get_Item(string name) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  1
      .locals init (uint32 V_0)
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  stloc.0
      IL_0003:  br.s       IL_0005

      IL_0005:  ldloc.0
      IL_0006:  ret
    } // end of method MutableClass::get_Item

    .method public hidebysig specialname 
            instance void  set_Item(string name,
                                    uint32 'value') cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method MutableClass::set_Item

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method MutableClass::.ctor

    .property instance int32 Property()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::get_Property()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::set_Property(int32)
    } // end of property MutableClass::Property
    .property instance uint32 Item(string)
    {
      .get instance uint32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::get_Item(string)
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::set_Item(string,
                                                                                                                     uint32)
    } // end of property MutableClass::Item
  } // end of class MutableClass

  .field private int32 test1
  .field private int32[] array1
  .field private valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer field1
  .field private valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum enumField
  .field public static int32 StaticField
  .field private static int32 '<StaticProperty>k__BackingField'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname static 
          int32  get_StaticProperty() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::'<StaticProperty>k__BackingField'
    IL_0005:  ret
  } // end of method CompoundAssignmentTest::get_StaticProperty

  .method public hidebysig specialname static 
          void  set_StaticProperty(int32 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::'<StaticProperty>k__BackingField'
    IL_0006:  ret
  } // end of method CompoundAssignmentTest::set_StaticProperty

  .method private hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass 
          M() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass V_0)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::.ctor()
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method CompoundAssignmentTest::M

  .method private hidebysig instance int32[0...,0...] 
          Array() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32[0...,0...] V_0)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method CompoundAssignmentTest::Array

  .method private hidebysig instance int32* 
          GetPointer() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  1
    .locals init (int32* V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  conv.u
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method CompoundAssignmentTest::GetPointer

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

  .method public hidebysig instance bool 
          IsUpperCaseA(char a) cil managed
  {
    // Code size       11 (0xb)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.s   65
    IL_0004:  ceq
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method CompoundAssignmentTest::IsUpperCaseA

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
    IL_0002:  ldarg.0
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
    IL_0025:  ldarg.0
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
    // Code size       58 (0x3a)
    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::array1
    IL_0007:  ldarg.1
    IL_0008:  ldelema    [mscorlib]System.Int32
    IL_000d:  dup
    IL_000e:  ldind.i4
    IL_000f:  ldarg.1
    IL_0010:  add
    IL_0011:  dup
    IL_0012:  stloc.0
    IL_0013:  stind.i4
    IL_0014:  ldloc.0
    IL_0015:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001a:  nop
    IL_001b:  ldarg.0
    IL_001c:  ldfld      int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::array1
    IL_0021:  ldarg.1
    IL_0022:  ldc.i4.2
    IL_0023:  mul
    IL_0024:  ldelema    [mscorlib]System.Int32
    IL_0029:  dup
    IL_002a:  ldind.i4
    IL_002b:  ldarg.1
    IL_002c:  ldc.i4.2
    IL_002d:  mul
    IL_002e:  add
    IL_002f:  dup
    IL_0030:  stloc.0
    IL_0031:  stind.i4
    IL_0032:  ldloc.0
    IL_0033:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0038:  nop
    IL_0039:  ret
  } // end of method CompoundAssignmentTest::Array

  .method public hidebysig instance int32 
          ArrayUsageWithMethods() cil managed
  {
    // Code size       31 (0x1f)
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
    IL_0013:  ldind.i4
    IL_0014:  stloc.0
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.1
    IL_0017:  add
    IL_0018:  stind.i4
    IL_0019:  ldloc.0
    IL_001a:  stloc.1
    IL_001b:  br.s       IL_001d

    IL_001d:  ldloc.1
    IL_001e:  ret
  } // end of method CompoundAssignmentTest::ArrayUsageWithMethods

  .method public hidebysig instance void 
          NestedField() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  3
    .locals init (bool V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::field1
    IL_0007:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer::HasIndex
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  brfalse.s  IL_002b

    IL_0010:  nop
    IL_0011:  ldarg.0
    IL_0012:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::field1
    IL_0017:  ldflda     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer::Field
    IL_001c:  dup
    IL_001d:  ldind.i4
    IL_001e:  stloc.1
    IL_001f:  ldloc.1
    IL_0020:  ldc.i4.1
    IL_0021:  add
    IL_0022:  stind.i4
    IL_0023:  ldloc.1
    IL_0024:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0029:  nop
    IL_002a:  nop
    IL_002b:  ret
  } // end of method CompoundAssignmentTest::NestedField

  .method public hidebysig instance void 
          Enum() cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.0
    IL_0003:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::enumField
    IL_0008:  ldc.i4.2
    IL_0009:  or
    IL_000a:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::enumField
    IL_000f:  ldarg.0
    IL_0010:  ldarg.0
    IL_0011:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::enumField
    IL_0016:  ldc.i4.s   -5
    IL_0018:  and
    IL_0019:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::enumField
    IL_001e:  ret
  } // end of method CompoundAssignmentTest::Enum

  .method public hidebysig instance int32 
          PreIncrementInAddition(int32 i,
                                 int32 j) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldc.i4.1
    IL_0004:  add
    IL_0005:  dup
    IL_0006:  starg.s    j
    IL_0008:  add
    IL_0009:  stloc.0
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.0
    IL_000d:  ret
  } // end of method CompoundAssignmentTest::PreIncrementInAddition

  .method public hidebysig instance int32 
          PreIncrementArrayElement(int32[] 'array',
                                   int32 pos) cil managed
  {
    // Code size       21 (0x15)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldelema    [mscorlib]System.Int32
    IL_0008:  dup
    IL_0009:  ldind.i4
    IL_000a:  ldc.i4.1
    IL_000b:  sub
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  stind.i4
    IL_000f:  ldloc.0
    IL_0010:  stloc.1
    IL_0011:  br.s       IL_0013

    IL_0013:  ldloc.1
    IL_0014:  ret
  } // end of method CompoundAssignmentTest::PreIncrementArrayElement

  .method public hidebysig instance int32 
          PreIncrementInstanceField() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::M()
    IL_0007:  dup
    IL_0008:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::Field
    IL_000d:  ldc.i4.1
    IL_000e:  add
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::Field
    IL_0016:  ldloc.0
    IL_0017:  stloc.1
    IL_0018:  br.s       IL_001a

    IL_001a:  ldloc.1
    IL_001b:  ret
  } // end of method CompoundAssignmentTest::PreIncrementInstanceField

  .method public hidebysig instance int32 
          PreIncrementInstanceField2(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass m) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  dup
    IL_0003:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::Field
    IL_0008:  ldc.i4.1
    IL_0009:  add
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::Field
    IL_0011:  ldloc.0
    IL_0012:  stloc.1
    IL_0013:  br.s       IL_0015

    IL_0015:  ldloc.1
    IL_0016:  ret
  } // end of method CompoundAssignmentTest::PreIncrementInstanceField2

  .method public hidebysig instance int32 
          PreIncrementInstanceProperty() cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::M()
    IL_0007:  dup
    IL_0008:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::get_Property()
    IL_000d:  ldc.i4.1
    IL_000e:  add
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::set_Property(int32)
    IL_0016:  nop
    IL_0017:  ldloc.0
    IL_0018:  stloc.1
    IL_0019:  br.s       IL_001b

    IL_001b:  ldloc.1
    IL_001c:  ret
  } // end of method CompoundAssignmentTest::PreIncrementInstanceProperty

  .method public hidebysig instance int32 
          PreIncrementStaticField() cil managed
  {
    // Code size       19 (0x13)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticField
    IL_0006:  ldc.i4.1
    IL_0007:  add
    IL_0008:  dup
    IL_0009:  stsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticField
    IL_000e:  stloc.0
    IL_000f:  br.s       IL_0011

    IL_0011:  ldloc.0
    IL_0012:  ret
  } // end of method CompoundAssignmentTest::PreIncrementStaticField

  .method public hidebysig instance int32 
          PreIncrementStaticProperty() cil managed
  {
    // Code size       20 (0x14)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::get_StaticProperty()
    IL_0006:  ldc.i4.1
    IL_0007:  add
    IL_0008:  dup
    IL_0009:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::set_StaticProperty(int32)
    IL_000e:  nop
    IL_000f:  stloc.0
    IL_0010:  br.s       IL_0012

    IL_0012:  ldloc.0
    IL_0013:  ret
  } // end of method CompoundAssignmentTest::PreIncrementStaticProperty

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method CompoundAssignmentTest::.ctor

  .property int32 StaticProperty()
  {
    .get int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::get_StaticProperty()
    .set void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::set_StaticProperty(int32)
  } // end of property CompoundAssignmentTest::StaticProperty
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
