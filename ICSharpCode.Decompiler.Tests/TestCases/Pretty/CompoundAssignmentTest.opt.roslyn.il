
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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module CompoundAssignmentTest.dll
// MVID: {0CCA0C51-2F8D-424C-BDE8-57699BE33BC6}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00900000


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
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } // end of method MutableClass::get_Item

    .method public hidebysig specialname 
            instance void  set_Item(string name,
                                    uint32 'value') cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method MutableClass::set_Item

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
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
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::.ctor()
    IL_0005:  ret
  } // end of method CompoundAssignmentTest::M

  .method private hidebysig instance int32[0...,0...] 
          Array() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ret
  } // end of method CompoundAssignmentTest::Array

  .method private hidebysig instance int32* 
          GetPointer() cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  conv.u
    IL_0002:  ret
  } // end of method CompoundAssignmentTest::GetPointer

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
  } // end of method CompoundAssignmentTest::GetIndex

  .method public hidebysig instance int32[] 
          GetArray() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0005:  throw
  } // end of method CompoundAssignmentTest::GetArray

  .method public hidebysig instance int32 
          GetValue(int32 'value') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ret
  } // end of method CompoundAssignmentTest::GetValue

  .method public hidebysig instance bool 
          IsUpperCaseA(char a) cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.s   65
    IL_0003:  ceq
    IL_0005:  ret
  } // end of method CompoundAssignmentTest::IsUpperCaseA

  .method public hidebysig instance void 
          Int32_Local_Add(int32 i) cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.1
    IL_0002:  add
    IL_0003:  starg.s    i
    IL_0005:  ldarg.1
    IL_0006:  dup
    IL_0007:  ldc.i4.1
    IL_0008:  add
    IL_0009:  starg.s    i
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0010:  ldarg.1
    IL_0011:  ldc.i4.1
    IL_0012:  add
    IL_0013:  dup
    IL_0014:  starg.s    i
    IL_0016:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001b:  ldarg.1
    IL_001c:  ldc.i4.5
    IL_001d:  add
    IL_001e:  starg.s    i
    IL_0020:  ldarg.1
    IL_0021:  ldc.i4.5
    IL_0022:  add
    IL_0023:  dup
    IL_0024:  starg.s    i
    IL_0026:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_002b:  ret
  } // end of method CompoundAssignmentTest::Int32_Local_Add

  .method public hidebysig instance void 
          Int32_Local_Sub(int32 i) cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.1
    IL_0002:  sub
    IL_0003:  starg.s    i
    IL_0005:  ldarg.1
    IL_0006:  dup
    IL_0007:  ldc.i4.1
    IL_0008:  sub
    IL_0009:  starg.s    i
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0010:  ldarg.1
    IL_0011:  ldc.i4.1
    IL_0012:  sub
    IL_0013:  dup
    IL_0014:  starg.s    i
    IL_0016:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001b:  ldarg.1
    IL_001c:  ldc.i4.5
    IL_001d:  sub
    IL_001e:  starg.s    i
    IL_0020:  ldarg.1
    IL_0021:  ldc.i4.5
    IL_0022:  sub
    IL_0023:  dup
    IL_0024:  starg.s    i
    IL_0026:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_002b:  ret
  } // end of method CompoundAssignmentTest::Int32_Local_Sub

  .method public hidebysig instance void 
          Int32_Local_Mul(int32 i) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.5
    IL_0002:  mul
    IL_0003:  starg.s    i
    IL_0005:  ldarg.1
    IL_0006:  ldc.i4.5
    IL_0007:  mul
    IL_0008:  dup
    IL_0009:  starg.s    i
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0010:  ret
  } // end of method CompoundAssignmentTest::Int32_Local_Mul

  .method public hidebysig instance void 
          Int32_Local_Div(int32 i) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.5
    IL_0002:  div
    IL_0003:  starg.s    i
    IL_0005:  ldarg.1
    IL_0006:  ldc.i4.5
    IL_0007:  div
    IL_0008:  dup
    IL_0009:  starg.s    i
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0010:  ret
  } // end of method CompoundAssignmentTest::Int32_Local_Div

  .method public hidebysig instance void 
          Int32_Local_Rem(int32 i) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.5
    IL_0002:  rem
    IL_0003:  starg.s    i
    IL_0005:  ldarg.1
    IL_0006:  ldc.i4.5
    IL_0007:  rem
    IL_0008:  dup
    IL_0009:  starg.s    i
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0010:  ret
  } // end of method CompoundAssignmentTest::Int32_Local_Rem

  .method public hidebysig instance void 
          Int32_Local_BitAnd(int32 i) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.5
    IL_0002:  and
    IL_0003:  starg.s    i
    IL_0005:  ldarg.1
    IL_0006:  ldc.i4.5
    IL_0007:  and
    IL_0008:  dup
    IL_0009:  starg.s    i
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0010:  ret
  } // end of method CompoundAssignmentTest::Int32_Local_BitAnd

  .method public hidebysig instance void 
          Int32_Local_BitOr(int32 i) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.5
    IL_0002:  or
    IL_0003:  starg.s    i
    IL_0005:  ldarg.1
    IL_0006:  ldc.i4.5
    IL_0007:  or
    IL_0008:  dup
    IL_0009:  starg.s    i
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0010:  ret
  } // end of method CompoundAssignmentTest::Int32_Local_BitOr

  .method public hidebysig instance void 
          Int32_Local_BitXor(int32 i) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.5
    IL_0002:  xor
    IL_0003:  starg.s    i
    IL_0005:  ldarg.1
    IL_0006:  ldc.i4.5
    IL_0007:  xor
    IL_0008:  dup
    IL_0009:  starg.s    i
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0010:  ret
  } // end of method CompoundAssignmentTest::Int32_Local_BitXor

  .method public hidebysig instance void 
          Int32_Local_ShiftLeft(int32 i) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.5
    IL_0002:  shl
    IL_0003:  starg.s    i
    IL_0005:  ldarg.1
    IL_0006:  ldc.i4.5
    IL_0007:  shl
    IL_0008:  dup
    IL_0009:  starg.s    i
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0010:  ret
  } // end of method CompoundAssignmentTest::Int32_Local_ShiftLeft

  .method public hidebysig instance void 
          Int32_Local_ShiftRight(int32 i) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.5
    IL_0002:  shr
    IL_0003:  starg.s    i
    IL_0005:  ldarg.1
    IL_0006:  ldc.i4.5
    IL_0007:  shr
    IL_0008:  dup
    IL_0009:  starg.s    i
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0010:  ret
  } // end of method CompoundAssignmentTest::Int32_Local_ShiftRight

  .method public hidebysig instance void 
          IntegerWithInline(int32 i) cil managed
  {
    // Code size       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.5
    IL_0002:  add
    IL_0003:  dup
    IL_0004:  starg.s    i
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000b:  ldarg.1
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0011:  ret
  } // end of method CompoundAssignmentTest::IntegerWithInline

  .method public hidebysig instance void 
          IntegerField(int32 i) cil managed
  {
    // Code size       67 (0x43)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::test1
    IL_0007:  ldarg.1
    IL_0008:  add
    IL_0009:  dup
    IL_000a:  stloc.0
    IL_000b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::test1
    IL_0010:  ldloc.0
    IL_0011:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0016:  ldarg.0
    IL_0017:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::test1
    IL_001c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0021:  ldarg.0
    IL_0022:  ldarg.0
    IL_0023:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::test1
    IL_0028:  ldarg.1
    IL_0029:  sub
    IL_002a:  dup
    IL_002b:  stloc.0
    IL_002c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::test1
    IL_0031:  ldloc.0
    IL_0032:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0037:  ldarg.0
    IL_0038:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::test1
    IL_003d:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0042:  ret
  } // end of method CompoundAssignmentTest::IntegerField

  .method public hidebysig instance void 
          Array(int32 i) cil managed
  {
    // Code size       55 (0x37)
    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::array1
    IL_0006:  ldarg.1
    IL_0007:  ldelema    [mscorlib]System.Int32
    IL_000c:  dup
    IL_000d:  ldind.i4
    IL_000e:  ldarg.1
    IL_000f:  add
    IL_0010:  dup
    IL_0011:  stloc.0
    IL_0012:  stind.i4
    IL_0013:  ldloc.0
    IL_0014:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0019:  ldarg.0
    IL_001a:  ldfld      int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::array1
    IL_001f:  ldarg.1
    IL_0020:  ldc.i4.2
    IL_0021:  mul
    IL_0022:  ldelema    [mscorlib]System.Int32
    IL_0027:  dup
    IL_0028:  ldind.i4
    IL_0029:  ldarg.1
    IL_002a:  ldc.i4.2
    IL_002b:  mul
    IL_002c:  add
    IL_002d:  dup
    IL_002e:  stloc.0
    IL_002f:  stind.i4
    IL_0030:  ldloc.0
    IL_0031:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0036:  ret
  } // end of method CompoundAssignmentTest::Array

  .method public hidebysig instance int32 
          ArrayUsageWithMethods() cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance int32[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::GetArray()
    IL_0006:  ldarg.0
    IL_0007:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::GetIndex()
    IL_000c:  ldelema    [mscorlib]System.Int32
    IL_0011:  dup
    IL_0012:  ldind.i4
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.1
    IL_0016:  add
    IL_0017:  stind.i4
    IL_0018:  ldloc.0
    IL_0019:  ret
  } // end of method CompoundAssignmentTest::ArrayUsageWithMethods

  .method public hidebysig instance void 
          NestedField() cil managed
  {
    // Code size       38 (0x26)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::field1
    IL_0006:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer::HasIndex
    IL_000b:  brfalse.s  IL_0025

    IL_000d:  ldarg.0
    IL_000e:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::field1
    IL_0013:  ldflda     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer::Field
    IL_0018:  dup
    IL_0019:  ldind.i4
    IL_001a:  stloc.0
    IL_001b:  ldloc.0
    IL_001c:  ldc.i4.1
    IL_001d:  add
    IL_001e:  stind.i4
    IL_001f:  ldloc.0
    IL_0020:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0025:  ret
  } // end of method CompoundAssignmentTest::NestedField

  .method public hidebysig instance void 
          Enum() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::enumField
    IL_0007:  ldc.i4.2
    IL_0008:  or
    IL_0009:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::enumField
    IL_000e:  ldarg.0
    IL_000f:  ldarg.0
    IL_0010:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::enumField
    IL_0015:  ldc.i4.s   -5
    IL_0017:  and
    IL_0018:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::enumField
    IL_001d:  ret
  } // end of method CompoundAssignmentTest::Enum

  .method public hidebysig instance int32 
          PreIncrementInAddition(int32 i,
                                 int32 j) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldc.i4.1
    IL_0003:  add
    IL_0004:  dup
    IL_0005:  starg.s    j
    IL_0007:  add
    IL_0008:  ret
  } // end of method CompoundAssignmentTest::PreIncrementInAddition

  .method public hidebysig instance int32 
          PreIncrementArrayElement(int32[] 'array',
                                   int32 pos) cil managed
  {
    // Code size       16 (0x10)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldelema    [mscorlib]System.Int32
    IL_0007:  dup
    IL_0008:  ldind.i4
    IL_0009:  ldc.i4.1
    IL_000a:  sub
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  stind.i4
    IL_000e:  ldloc.0
    IL_000f:  ret
  } // end of method CompoundAssignmentTest::PreIncrementArrayElement

  .method public hidebysig instance int32 
          PreIncrementInstanceField() cil managed
  {
    // Code size       23 (0x17)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::M()
    IL_0006:  dup
    IL_0007:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::Field
    IL_000c:  ldc.i4.1
    IL_000d:  add
    IL_000e:  stloc.0
    IL_000f:  ldloc.0
    IL_0010:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::Field
    IL_0015:  ldloc.0
    IL_0016:  ret
  } // end of method CompoundAssignmentTest::PreIncrementInstanceField

  .method public hidebysig instance int32 
          PreIncrementInstanceField2(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass m) cil managed
  {
    // Code size       18 (0x12)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.1
    IL_0001:  dup
    IL_0002:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::Field
    IL_0007:  ldc.i4.1
    IL_0008:  add
    IL_0009:  stloc.0
    IL_000a:  ldloc.0
    IL_000b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::Field
    IL_0010:  ldloc.0
    IL_0011:  ret
  } // end of method CompoundAssignmentTest::PreIncrementInstanceField2

  .method public hidebysig instance int32 
          PreIncrementInstanceProperty() cil managed
  {
    // Code size       23 (0x17)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::M()
    IL_0006:  dup
    IL_0007:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::get_Property()
    IL_000c:  ldc.i4.1
    IL_000d:  add
    IL_000e:  stloc.0
    IL_000f:  ldloc.0
    IL_0010:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::set_Property(int32)
    IL_0015:  ldloc.0
    IL_0016:  ret
  } // end of method CompoundAssignmentTest::PreIncrementInstanceProperty

  .method public hidebysig instance int32 
          PreIncrementStaticField() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticField
    IL_0005:  ldc.i4.1
    IL_0006:  add
    IL_0007:  dup
    IL_0008:  stsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticField
    IL_000d:  ret
  } // end of method CompoundAssignmentTest::PreIncrementStaticField

  .method public hidebysig instance int32 
          PreIncrementStaticProperty() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::get_StaticProperty()
    IL_0005:  ldc.i4.1
    IL_0006:  add
    IL_0007:  dup
    IL_0008:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::set_StaticProperty(int32)
    IL_000d:  ret
  } // end of method CompoundAssignmentTest::PreIncrementStaticProperty

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method CompoundAssignmentTest::.ctor

  .property int32 StaticProperty()
  {
    .get int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::get_StaticProperty()
    .set void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::set_StaticProperty(int32)
  } // end of property CompoundAssignmentTest::StaticProperty
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
