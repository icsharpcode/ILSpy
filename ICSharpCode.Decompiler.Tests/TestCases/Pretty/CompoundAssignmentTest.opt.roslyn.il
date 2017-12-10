
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
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
// MVID: {A41AE390-D37A-4425-A0E0-8C87CC28D8D9}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x04BC0000


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

  .class auto ansi sealed nested public ShortEnum
         extends [mscorlib]System.Enum
  {
    .field public specialname rtspecialname int16 value__
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum None = int16(0x0000)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum One = int16(0x0001)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum Two = int16(0x0002)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum Four = int16(0x0004)
  } // end of class ShortEnum

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
    .field public int16 ShortField
    .field private int32 '<Property>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private uint8 '<ByteProperty>k__BackingField'
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
            instance uint8  get_ByteProperty() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::'<ByteProperty>k__BackingField'
      IL_0006:  ret
    } // end of method MutableClass::get_ByteProperty

    .method public hidebysig specialname 
            instance void  set_ByteProperty(uint8 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::'<ByteProperty>k__BackingField'
      IL_0007:  ret
    } // end of method MutableClass::set_ByteProperty

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
    .property instance uint8 ByteProperty()
    {
      .get instance uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::get_ByteProperty()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::set_ByteProperty(uint8)
    } // end of property MutableClass::ByteProperty
    .property instance uint32 Item(string)
    {
      .get instance uint32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::get_Item(string)
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::set_Item(string,
                                                                                                                     uint32)
    } // end of property MutableClass::Item
  } // end of class MutableClass

  .class auto ansi nested private beforefieldinit Item
         extends [mscorlib]System.Object
  {
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/Item Self
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method Item::.ctor

  } // end of class Item

  .field private int32 test1
  .field private int32[] array1
  .field private valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer field1
  .field private valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum enumField
  .field private class [mscorlib]System.Collections.Generic.Dictionary`2<uint16,uint16> ushortDict
  .field private uint16 ushortField
  .field private valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum shortEnumField
  .field public static int32 StaticField
  .field public static int16 StaticShortField
  .field private static int32 '<StaticProperty>k__BackingField'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum '<StaticShortProperty>k__BackingField'
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

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum 
          get_StaticShortProperty() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::'<StaticShortProperty>k__BackingField'
    IL_0005:  ret
  } // end of method CompoundAssignmentTest::get_StaticShortProperty

  .method public hidebysig specialname static 
          void  set_StaticShortProperty(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  stsfld     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::'<StaticShortProperty>k__BackingField'
    IL_0006:  ret
  } // end of method CompoundAssignmentTest::set_StaticShortProperty

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
    // Code size       78 (0x4e)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::field1
    IL_0006:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer::HasIndex
    IL_000b:  brfalse.s  IL_004d

    IL_000d:  ldarg.0
    IL_000e:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::field1
    IL_0013:  ldflda     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer::Field
    IL_0018:  dup
    IL_0019:  ldind.i4
    IL_001a:  ldc.i4.2
    IL_001b:  mul
    IL_001c:  dup
    IL_001d:  stloc.0
    IL_001e:  stind.i4
    IL_001f:  ldloc.0
    IL_0020:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0025:  ldarg.0
    IL_0026:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::field1
    IL_002b:  ldflda     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer::Field
    IL_0030:  dup
    IL_0031:  ldind.i4
    IL_0032:  ldc.i4.1
    IL_0033:  add
    IL_0034:  stind.i4
    IL_0035:  ldarg.0
    IL_0036:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::field1
    IL_003b:  ldflda     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/StructContainer::Field
    IL_0040:  dup
    IL_0041:  ldind.i4
    IL_0042:  stloc.0
    IL_0043:  ldloc.0
    IL_0044:  ldc.i4.1
    IL_0045:  add
    IL_0046:  stind.i4
    IL_0047:  ldloc.0
    IL_0048:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_004d:  ret
  } // end of method CompoundAssignmentTest::NestedField

  .method public hidebysig instance void 
          Enum() cil managed
  {
    // Code size       58 (0x3a)
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
    IL_001d:  ldarg.0
    IL_001e:  ldarg.0
    IL_001f:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::enumField
    IL_0024:  ldc.i4.2
    IL_0025:  add
    IL_0026:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::enumField
    IL_002b:  ldarg.0
    IL_002c:  ldarg.0
    IL_002d:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::enumField
    IL_0032:  ldc.i4.3
    IL_0033:  sub
    IL_0034:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::enumField
    IL_0039:  ret
  } // end of method CompoundAssignmentTest::Enum

  .method public hidebysig instance void 
          ShortEnumTest() cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::shortEnumField
    IL_0007:  ldc.i4.2
    IL_0008:  or
    IL_0009:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::shortEnumField
    IL_000e:  ldarg.0
    IL_000f:  ldarg.0
    IL_0010:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::shortEnumField
    IL_0015:  ldc.i4.4
    IL_0016:  and
    IL_0017:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::shortEnumField
    IL_001c:  ldarg.0
    IL_001d:  ldarg.0
    IL_001e:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::shortEnumField
    IL_0023:  ldc.i4.2
    IL_0024:  add
    IL_0025:  conv.i2
    IL_0026:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::shortEnumField
    IL_002b:  ldarg.0
    IL_002c:  ldarg.0
    IL_002d:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::shortEnumField
    IL_0032:  ldc.i4.3
    IL_0033:  sub
    IL_0034:  conv.i2
    IL_0035:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::shortEnumField
    IL_003a:  ret
  } // end of method CompoundAssignmentTest::ShortEnumTest

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
          PostIncrementArrayElement(int32[] 'array',
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
    IL_0009:  stloc.0
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.1
    IL_000c:  add
    IL_000d:  stind.i4
    IL_000e:  ldloc.0
    IL_000f:  ret
  } // end of method CompoundAssignmentTest::PostIncrementArrayElement

  .method public hidebysig instance void 
          IncrementArrayElement(int32[] 'array',
                                int32 pos) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldelema    [mscorlib]System.Int32
    IL_0007:  dup
    IL_0008:  ldind.i4
    IL_0009:  ldc.i4.1
    IL_000a:  add
    IL_000b:  stind.i4
    IL_000c:  ret
  } // end of method CompoundAssignmentTest::IncrementArrayElement

  .method public hidebysig instance void 
          DoubleArrayElement(int32[] 'array',
                             int32 pos) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldelema    [mscorlib]System.Int32
    IL_0007:  dup
    IL_0008:  ldind.i4
    IL_0009:  ldc.i4.2
    IL_000a:  mul
    IL_000b:  stind.i4
    IL_000c:  ret
  } // end of method CompoundAssignmentTest::DoubleArrayElement

  .method public hidebysig instance int32 
          DoubleArrayElementAndReturn(int32[] 'array',
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
    IL_0009:  ldc.i4.2
    IL_000a:  mul
    IL_000b:  dup
    IL_000c:  stloc.0
    IL_000d:  stind.i4
    IL_000e:  ldloc.0
    IL_000f:  ret
  } // end of method CompoundAssignmentTest::DoubleArrayElementAndReturn

  .method public hidebysig instance int32 
          PreIncrementArrayElementShort(int16[] 'array',
                                        int32 pos) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  3
    .locals init (int16 V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldelema    [mscorlib]System.Int16
    IL_0007:  dup
    IL_0008:  ldind.i2
    IL_0009:  ldc.i4.1
    IL_000a:  sub
    IL_000b:  conv.i2
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  stind.i2
    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method CompoundAssignmentTest::PreIncrementArrayElementShort

  .method public hidebysig instance int32 
          PostIncrementArrayElementShort(int16[] 'array',
                                         int32 pos) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  3
    .locals init (int16 V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldelema    [mscorlib]System.Int16
    IL_0007:  dup
    IL_0008:  ldind.i2
    IL_0009:  stloc.0
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.1
    IL_000c:  add
    IL_000d:  conv.i2
    IL_000e:  stind.i2
    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method CompoundAssignmentTest::PostIncrementArrayElementShort

  .method public hidebysig instance void 
          IncrementArrayElementShort(int16[] 'array',
                                     int32 pos) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldelema    [mscorlib]System.Int16
    IL_0007:  dup
    IL_0008:  ldind.i2
    IL_0009:  ldc.i4.1
    IL_000a:  add
    IL_000b:  conv.i2
    IL_000c:  stind.i2
    IL_000d:  ret
  } // end of method CompoundAssignmentTest::IncrementArrayElementShort

  .method public hidebysig instance void 
          DoubleArrayElementShort(int16[] 'array',
                                  int32 pos) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldelema    [mscorlib]System.Int16
    IL_0007:  dup
    IL_0008:  ldind.i2
    IL_0009:  ldc.i4.2
    IL_000a:  mul
    IL_000b:  conv.i2
    IL_000c:  stind.i2
    IL_000d:  ret
  } // end of method CompoundAssignmentTest::DoubleArrayElementShort

  .method public hidebysig instance int16 
          DoubleArrayElementShortAndReturn(int16[] 'array',
                                           int32 pos) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  3
    .locals init (int16 V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldelema    [mscorlib]System.Int16
    IL_0007:  dup
    IL_0008:  ldind.i2
    IL_0009:  ldc.i4.2
    IL_000a:  mul
    IL_000b:  conv.i2
    IL_000c:  dup
    IL_000d:  stloc.0
    IL_000e:  stind.i2
    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method CompoundAssignmentTest::DoubleArrayElementShortAndReturn

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
          PostIncrementInstanceField() cil managed
  {
    // Code size       23 (0x17)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::M()
    IL_0006:  dup
    IL_0007:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::Field
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.1
    IL_000f:  add
    IL_0010:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::Field
    IL_0015:  ldloc.0
    IL_0016:  ret
  } // end of method CompoundAssignmentTest::PostIncrementInstanceField

  .method public hidebysig instance void 
          IncrementInstanceField() cil managed
  {
    // Code size       20 (0x14)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::M()
    IL_0006:  dup
    IL_0007:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::Field
    IL_000c:  ldc.i4.1
    IL_000d:  add
    IL_000e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::Field
    IL_0013:  ret
  } // end of method CompoundAssignmentTest::IncrementInstanceField

  .method public hidebysig instance void 
          DoubleInstanceField() cil managed
  {
    // Code size       20 (0x14)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::M()
    IL_0006:  dup
    IL_0007:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::Field
    IL_000c:  ldc.i4.2
    IL_000d:  mul
    IL_000e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::Field
    IL_0013:  ret
  } // end of method CompoundAssignmentTest::DoubleInstanceField

  .method public hidebysig instance int32 
          DoubleInstanceFieldAndReturn() cil managed
  {
    // Code size       23 (0x17)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::M()
    IL_0006:  dup
    IL_0007:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::Field
    IL_000c:  ldc.i4.2
    IL_000d:  mul
    IL_000e:  dup
    IL_000f:  stloc.0
    IL_0010:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::Field
    IL_0015:  ldloc.0
    IL_0016:  ret
  } // end of method CompoundAssignmentTest::DoubleInstanceFieldAndReturn

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
          PostIncrementInstanceField2(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass m) cil managed
  {
    // Code size       18 (0x12)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.1
    IL_0001:  dup
    IL_0002:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::Field
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.1
    IL_000a:  add
    IL_000b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::Field
    IL_0010:  ldloc.0
    IL_0011:  ret
  } // end of method CompoundAssignmentTest::PostIncrementInstanceField2

  .method public hidebysig instance void 
          IncrementInstanceField2(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass m) cil managed
  {
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  dup
    IL_0002:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::Field
    IL_0007:  ldc.i4.1
    IL_0008:  add
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::Field
    IL_000e:  ret
  } // end of method CompoundAssignmentTest::IncrementInstanceField2

  .method public hidebysig instance int32 
          PreIncrementInstanceFieldShort() cil managed
  {
    // Code size       24 (0x18)
    .maxstack  3
    .locals init (int16 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::M()
    IL_0006:  dup
    IL_0007:  ldfld      int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::ShortField
    IL_000c:  ldc.i4.1
    IL_000d:  add
    IL_000e:  conv.i2
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  stfld      int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::ShortField
    IL_0016:  ldloc.0
    IL_0017:  ret
  } // end of method CompoundAssignmentTest::PreIncrementInstanceFieldShort

  .method public hidebysig instance int32 
          PostIncrementInstanceFieldShort() cil managed
  {
    // Code size       24 (0x18)
    .maxstack  3
    .locals init (int16 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::M()
    IL_0006:  dup
    IL_0007:  ldfld      int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::ShortField
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.1
    IL_000f:  add
    IL_0010:  conv.i2
    IL_0011:  stfld      int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::ShortField
    IL_0016:  ldloc.0
    IL_0017:  ret
  } // end of method CompoundAssignmentTest::PostIncrementInstanceFieldShort

  .method public hidebysig instance void 
          IncrementInstanceFieldShort() cil managed
  {
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::M()
    IL_0006:  dup
    IL_0007:  ldfld      int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::ShortField
    IL_000c:  ldc.i4.1
    IL_000d:  add
    IL_000e:  conv.i2
    IL_000f:  stfld      int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::ShortField
    IL_0014:  ret
  } // end of method CompoundAssignmentTest::IncrementInstanceFieldShort

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
          PostIncrementInstanceProperty() cil managed
  {
    // Code size       23 (0x17)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::M()
    IL_0006:  dup
    IL_0007:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::get_Property()
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.1
    IL_000f:  add
    IL_0010:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::set_Property(int32)
    IL_0015:  ldloc.0
    IL_0016:  ret
  } // end of method CompoundAssignmentTest::PostIncrementInstanceProperty

  .method public hidebysig instance void 
          IncrementInstanceProperty() cil managed
  {
    // Code size       22 (0x16)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::M()
    IL_0006:  dup
    IL_0007:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::get_Property()
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.1
    IL_000f:  add
    IL_0010:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::set_Property(int32)
    IL_0015:  ret
  } // end of method CompoundAssignmentTest::IncrementInstanceProperty

  .method public hidebysig instance void 
          DoubleInstanceProperty() cil managed
  {
    // Code size       20 (0x14)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::M()
    IL_0006:  dup
    IL_0007:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::get_Property()
    IL_000c:  ldc.i4.2
    IL_000d:  mul
    IL_000e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::set_Property(int32)
    IL_0013:  ret
  } // end of method CompoundAssignmentTest::DoubleInstanceProperty

  .method public hidebysig instance int32 
          DoubleInstancePropertyAndReturn() cil managed
  {
    // Code size       23 (0x17)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::M()
    IL_0006:  dup
    IL_0007:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::get_Property()
    IL_000c:  ldc.i4.2
    IL_000d:  mul
    IL_000e:  dup
    IL_000f:  stloc.0
    IL_0010:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::set_Property(int32)
    IL_0015:  ldloc.0
    IL_0016:  ret
  } // end of method CompoundAssignmentTest::DoubleInstancePropertyAndReturn

  .method public hidebysig instance int32 
          PreIncrementInstancePropertyByte() cil managed
  {
    // Code size       24 (0x18)
    .maxstack  3
    .locals init (uint8 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::M()
    IL_0006:  dup
    IL_0007:  callvirt   instance uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::get_ByteProperty()
    IL_000c:  ldc.i4.1
    IL_000d:  add
    IL_000e:  conv.u1
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::set_ByteProperty(uint8)
    IL_0016:  ldloc.0
    IL_0017:  ret
  } // end of method CompoundAssignmentTest::PreIncrementInstancePropertyByte

  .method public hidebysig instance int32 
          PostIncrementInstancePropertyByte() cil managed
  {
    // Code size       24 (0x18)
    .maxstack  3
    .locals init (uint8 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::M()
    IL_0006:  dup
    IL_0007:  callvirt   instance uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::get_ByteProperty()
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.1
    IL_000f:  add
    IL_0010:  conv.u1
    IL_0011:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::set_ByteProperty(uint8)
    IL_0016:  ldloc.0
    IL_0017:  ret
  } // end of method CompoundAssignmentTest::PostIncrementInstancePropertyByte

  .method public hidebysig instance void 
          IncrementInstancePropertyByte() cil managed
  {
    // Code size       23 (0x17)
    .maxstack  3
    .locals init (uint8 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::M()
    IL_0006:  dup
    IL_0007:  callvirt   instance uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::get_ByteProperty()
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.1
    IL_000f:  add
    IL_0010:  conv.u1
    IL_0011:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::set_ByteProperty(uint8)
    IL_0016:  ret
  } // end of method CompoundAssignmentTest::IncrementInstancePropertyByte

  .method public hidebysig instance void 
          DoubleInstancePropertyByte() cil managed
  {
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::M()
    IL_0006:  dup
    IL_0007:  callvirt   instance uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::get_ByteProperty()
    IL_000c:  ldc.i4.2
    IL_000d:  mul
    IL_000e:  conv.u1
    IL_000f:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::set_ByteProperty(uint8)
    IL_0014:  ret
  } // end of method CompoundAssignmentTest::DoubleInstancePropertyByte

  .method public hidebysig instance int32 
          DoubleInstancePropertyByteAndReturn() cil managed
  {
    // Code size       24 (0x18)
    .maxstack  3
    .locals init (uint8 V_0)
    IL_0000:  ldarg.0
    IL_0001:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::M()
    IL_0006:  dup
    IL_0007:  callvirt   instance uint8 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::get_ByteProperty()
    IL_000c:  ldc.i4.2
    IL_000d:  mul
    IL_000e:  conv.u1
    IL_000f:  dup
    IL_0010:  stloc.0
    IL_0011:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MutableClass::set_ByteProperty(uint8)
    IL_0016:  ldloc.0
    IL_0017:  ret
  } // end of method CompoundAssignmentTest::DoubleInstancePropertyByteAndReturn

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
          PostIncrementStaticField() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticField
    IL_0005:  dup
    IL_0006:  ldc.i4.1
    IL_0007:  add
    IL_0008:  stsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticField
    IL_000d:  ret
  } // end of method CompoundAssignmentTest::PostIncrementStaticField

  .method public hidebysig instance void 
          IncrementStaticField() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticField
    IL_0005:  ldc.i4.1
    IL_0006:  add
    IL_0007:  stsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticField
    IL_000c:  ret
  } // end of method CompoundAssignmentTest::IncrementStaticField

  .method public hidebysig instance void 
          DoubleStaticField() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticField
    IL_0005:  ldc.i4.2
    IL_0006:  mul
    IL_0007:  stsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticField
    IL_000c:  ret
  } // end of method CompoundAssignmentTest::DoubleStaticField

  .method public hidebysig instance int32 
          DoubleStaticFieldAndReturn() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticField
    IL_0005:  ldc.i4.2
    IL_0006:  mul
    IL_0007:  dup
    IL_0008:  stsfld     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticField
    IL_000d:  ret
  } // end of method CompoundAssignmentTest::DoubleStaticFieldAndReturn

  .method public hidebysig instance int32 
          PreIncrementStaticFieldShort() cil managed
  {
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldsfld     int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticShortField
    IL_0005:  ldc.i4.1
    IL_0006:  add
    IL_0007:  conv.i2
    IL_0008:  dup
    IL_0009:  stsfld     int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticShortField
    IL_000e:  ret
  } // end of method CompoundAssignmentTest::PreIncrementStaticFieldShort

  .method public hidebysig instance int32 
          PostIncrementStaticFieldShort() cil managed
  {
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldsfld     int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticShortField
    IL_0005:  dup
    IL_0006:  ldc.i4.1
    IL_0007:  add
    IL_0008:  conv.i2
    IL_0009:  stsfld     int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticShortField
    IL_000e:  ret
  } // end of method CompoundAssignmentTest::PostIncrementStaticFieldShort

  .method public hidebysig instance void 
          IncrementStaticFieldShort() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldsfld     int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticShortField
    IL_0005:  ldc.i4.1
    IL_0006:  add
    IL_0007:  conv.i2
    IL_0008:  stsfld     int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticShortField
    IL_000d:  ret
  } // end of method CompoundAssignmentTest::IncrementStaticFieldShort

  .method public hidebysig instance void 
          DoubleStaticFieldShort() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  ldsfld     int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticShortField
    IL_0005:  ldc.i4.2
    IL_0006:  mul
    IL_0007:  conv.i2
    IL_0008:  stsfld     int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticShortField
    IL_000d:  ret
  } // end of method CompoundAssignmentTest::DoubleStaticFieldShort

  .method public hidebysig instance int16 
          DoubleStaticFieldAndReturnShort() cil managed
  {
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldsfld     int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticShortField
    IL_0005:  ldc.i4.2
    IL_0006:  mul
    IL_0007:  conv.i2
    IL_0008:  dup
    IL_0009:  stsfld     int16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::StaticShortField
    IL_000e:  ret
  } // end of method CompoundAssignmentTest::DoubleStaticFieldAndReturnShort

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

  .method public hidebysig instance int32 
          PostIncrementStaticProperty() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::get_StaticProperty()
    IL_0005:  dup
    IL_0006:  ldc.i4.1
    IL_0007:  add
    IL_0008:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::set_StaticProperty(int32)
    IL_000d:  ret
  } // end of method CompoundAssignmentTest::PostIncrementStaticProperty

  .method public hidebysig instance void 
          IncrementStaticProperty() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::get_StaticProperty()
    IL_0005:  ldc.i4.1
    IL_0006:  add
    IL_0007:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::set_StaticProperty(int32)
    IL_000c:  ret
  } // end of method CompoundAssignmentTest::IncrementStaticProperty

  .method public hidebysig instance void 
          DoubleStaticProperty() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::get_StaticProperty()
    IL_0005:  ldc.i4.2
    IL_0006:  mul
    IL_0007:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::set_StaticProperty(int32)
    IL_000c:  ret
  } // end of method CompoundAssignmentTest::DoubleStaticProperty

  .method public hidebysig instance int32 
          DoubleStaticPropertyAndReturn() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::get_StaticProperty()
    IL_0005:  ldc.i4.2
    IL_0006:  mul
    IL_0007:  dup
    IL_0008:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::set_StaticProperty(int32)
    IL_000d:  ret
  } // end of method CompoundAssignmentTest::DoubleStaticPropertyAndReturn

  .method public hidebysig instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum 
          PreIncrementStaticPropertyShort() cil managed
  {
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::get_StaticShortProperty()
    IL_0005:  ldc.i4.1
    IL_0006:  add
    IL_0007:  conv.i2
    IL_0008:  dup
    IL_0009:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::set_StaticShortProperty(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum)
    IL_000e:  ret
  } // end of method CompoundAssignmentTest::PreIncrementStaticPropertyShort

  .method public hidebysig instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum 
          PostIncrementStaticPropertyShort() cil managed
  {
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::get_StaticShortProperty()
    IL_0005:  dup
    IL_0006:  ldc.i4.1
    IL_0007:  add
    IL_0008:  conv.i2
    IL_0009:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::set_StaticShortProperty(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum)
    IL_000e:  ret
  } // end of method CompoundAssignmentTest::PostIncrementStaticPropertyShort

  .method public hidebysig instance void 
          IncrementStaticPropertyShort() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::get_StaticShortProperty()
    IL_0005:  ldc.i4.1
    IL_0006:  add
    IL_0007:  conv.i2
    IL_0008:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::set_StaticShortProperty(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum)
    IL_000d:  ret
  } // end of method CompoundAssignmentTest::IncrementStaticPropertyShort

  .method private hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/Item 
          GetItem(object obj) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ret
  } // end of method CompoundAssignmentTest::GetItem

  .method private hidebysig static void  Issue882() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/Item ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::GetItem(object)
    IL_0006:  dup
    IL_0007:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/Item ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/Item::Self
    IL_000c:  ret
  } // end of method CompoundAssignmentTest::Issue882

  .method private hidebysig instance void 
          Issue954(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum& a,
                   valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum b) cil managed
  {
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.1
    IL_0002:  ldind.i4
    IL_0003:  ldarg.2
    IL_0004:  rem
    IL_0005:  stind.i4
    IL_0006:  ldarg.0
    IL_0007:  ldarg.0
    IL_0008:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::enumField
    IL_000d:  ldarg.2
    IL_000e:  rem
    IL_000f:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::enumField
    IL_0014:  ret
  } // end of method CompoundAssignmentTest::Issue954

  .method private hidebysig instance void 
          Issue588(uint16 val) cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  4
    .locals init (uint16 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.Dictionary`2<uint16,uint16> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::ushortDict
    IL_0006:  ldarg.0
    IL_0007:  ldarg.0
    IL_0008:  ldfld      uint16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::ushortField
    IL_000d:  stloc.0
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.1
    IL_0010:  add
    IL_0011:  conv.u2
    IL_0012:  stfld      uint16 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::ushortField
    IL_0017:  ldloc.0
    IL_0018:  ldarg.1
    IL_0019:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<uint16,uint16>::Add(!0,
                                                                                                                   !1)
    IL_001e:  ret
  } // end of method CompoundAssignmentTest::Issue588

  .method private hidebysig instance void 
          Issue1007(valuetype [mscorlib]System.TimeSpan[] items,
                    int32 startIndex,
                    valuetype [mscorlib]System.TimeSpan item) cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  ldarg.2
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  ldloc.0
    IL_0004:  dup
    IL_0005:  ldc.i4.1
    IL_0006:  add
    IL_0007:  stloc.0
    IL_0008:  ldarg.3
    IL_0009:  stelem     [mscorlib]System.TimeSpan
    IL_000e:  ldarg.1
    IL_000f:  ldloc.0
    IL_0010:  dup
    IL_0011:  ldc.i4.1
    IL_0012:  add
    IL_0013:  stloc.0
    IL_0014:  ldarg.3
    IL_0015:  stelem     [mscorlib]System.TimeSpan
    IL_001a:  ret
  } // end of method CompoundAssignmentTest::Issue1007

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<uint16,uint16>::.ctor()
    IL_0006:  stfld      class [mscorlib]System.Collections.Generic.Dictionary`2<uint16,uint16> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::ushortDict
    IL_000b:  ldarg.0
    IL_000c:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0011:  ret
  } // end of method CompoundAssignmentTest::.ctor

  .property int32 StaticProperty()
  {
    .get int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::get_StaticProperty()
    .set void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::set_StaticProperty(int32)
  } // end of property CompoundAssignmentTest::StaticProperty
  .property valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum
          StaticShortProperty()
  {
    .get valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::get_StaticShortProperty()
    .set void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest::set_StaticShortProperty(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest/ShortEnum)
  } // end of property CompoundAssignmentTest::StaticShortProperty
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CompoundAssignmentTest


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
