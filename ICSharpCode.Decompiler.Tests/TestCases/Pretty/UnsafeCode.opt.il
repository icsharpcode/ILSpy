
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly apfrp3oh
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module apfrp3oh.dll
// MVID: {18CFEE2F-79CA-4464-BA60-BA8E54BE2D99}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x04CA0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode
       extends [mscorlib]System.Object
{
  .class sequential ansi sealed nested public beforefieldinit SimpleStruct
         extends [mscorlib]System.ValueType
  {
    .field public int32 X
    .field public float64 Y
  } // end of class SimpleStruct

  .class sequential ansi sealed nested public beforefieldinit StructWithFixedSizeMembers
         extends [mscorlib]System.ValueType
  {
    .class sequential ansi sealed nested public beforefieldinit '<Integers>e__FixedBuffer0'
           extends [mscorlib]System.ValueType
    {
      .pack 0
      .size 400
      .custom instance void [mscorlib]System.Runtime.CompilerServices.UnsafeValueTypeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 FixedElementField
    } // end of class '<Integers>e__FixedBuffer0'

    .class sequential ansi sealed nested public beforefieldinit '<Doubles>e__FixedBuffer1'
           extends [mscorlib]System.ValueType
    {
      .pack 0
      .size 1600
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.UnsafeValueTypeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public float64 FixedElementField
    } // end of class '<Doubles>e__FixedBuffer1'

    .class sequential ansi sealed nested public beforefieldinit '<Old>e__FixedBuffer2'
           extends [mscorlib]System.ValueType
    {
      .pack 0
      .size 1
      .custom instance void [mscorlib]System.Runtime.CompilerServices.UnsafeValueTypeAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public uint8 FixedElementField
    } // end of class '<Old>e__FixedBuffer2'

    .field public valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer0' Integers
    .custom instance void [mscorlib]System.Runtime.CompilerServices.FixedBufferAttribute::.ctor(class [mscorlib]System.Type,
                                                                                                int32) = ( 01 00 59 53 79 73 74 65 6D 2E 49 6E 74 33 32 2C   // ..YSystem.Int32,
                                                                                                           20 6D 73 63 6F 72 6C 69 62 2C 20 56 65 72 73 69   //  mscorlib, Versi
                                                                                                           6F 6E 3D 34 2E 30 2E 30 2E 30 2C 20 43 75 6C 74   // on=4.0.0.0, Cult
                                                                                                           75 72 65 3D 6E 65 75 74 72 61 6C 2C 20 50 75 62   // ure=neutral, Pub
                                                                                                           6C 69 63 4B 65 79 54 6F 6B 65 6E 3D 62 37 37 61   // licKeyToken=b77a
                                                                                                           35 63 35 36 31 39 33 34 65 30 38 39 64 00 00 00   // 5c561934e089d...
                                                                                                           00 00 ) 
    .field public int32 NormalMember
    .field public valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Doubles>e__FixedBuffer1' Doubles
    .custom instance void [mscorlib]System.Runtime.CompilerServices.FixedBufferAttribute::.ctor(class [mscorlib]System.Type,
                                                                                                int32) = ( 01 00 5A 53 79 73 74 65 6D 2E 44 6F 75 62 6C 65   // ..ZSystem.Double
                                                                                                           2C 20 6D 73 63 6F 72 6C 69 62 2C 20 56 65 72 73   // , mscorlib, Vers
                                                                                                           69 6F 6E 3D 34 2E 30 2E 30 2E 30 2C 20 43 75 6C   // ion=4.0.0.0, Cul
                                                                                                           74 75 72 65 3D 6E 65 75 74 72 61 6C 2C 20 50 75   // ture=neutral, Pu
                                                                                                           62 6C 69 63 4B 65 79 54 6F 6B 65 6E 3D 62 37 37   // blicKeyToken=b77
                                                                                                           61 35 63 35 36 31 39 33 34 65 30 38 39 C8 00 00   // a5c561934e089...
                                                                                                           00 00 00 ) 
    .field public valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Old>e__FixedBuffer2' Old
    .custom instance void [mscorlib]System.ObsoleteAttribute::.ctor(string) = ( 01 00 11 61 6E 6F 74 68 65 72 20 61 74 74 72 69   // ...another attri
                                                                                62 75 74 65 00 00 )                               // bute..
    .custom instance void [mscorlib]System.Runtime.CompilerServices.FixedBufferAttribute::.ctor(class [mscorlib]System.Type,
                                                                                                int32) = ( 01 00 58 53 79 73 74 65 6D 2E 42 79 74 65 2C 20   // ..XSystem.Byte, 
                                                                                                           6D 73 63 6F 72 6C 69 62 2C 20 56 65 72 73 69 6F   // mscorlib, Versio
                                                                                                           6E 3D 34 2E 30 2E 30 2E 30 2C 20 43 75 6C 74 75   // n=4.0.0.0, Cultu
                                                                                                           72 65 3D 6E 65 75 74 72 61 6C 2C 20 50 75 62 6C   // re=neutral, Publ
                                                                                                           69 63 4B 65 79 54 6F 6B 65 6E 3D 62 37 37 61 35   // icKeyToken=b77a5
                                                                                                           63 35 36 31 39 33 34 65 30 38 39 01 00 00 00 00   // c561934e089.....
                                                                                                           00 ) 
  } // end of class StructWithFixedSizeMembers

  .class sequential ansi sealed nested private beforefieldinit Data
         extends [mscorlib]System.ValueType
  {
    .field public valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/Vector Position
  } // end of class Data

  .class sequential ansi sealed nested private beforefieldinit Vector
         extends [mscorlib]System.ValueType
  {
    .pack 0
    .size 1
    .method public hidebysig virtual instance int32 
            GetHashCode() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } // end of method Vector::GetHashCode

  } // end of class Vector

  .class auto ansi sealed nested public UnsafeDelegate
         extends [mscorlib]System.MulticastDelegate
  {
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(object 'object',
                                 native int 'method') runtime managed
    {
    } // end of method UnsafeDelegate::.ctor

    .method public hidebysig newslot virtual 
            instance void  Invoke(uint8* ptr) runtime managed
    {
    } // end of method UnsafeDelegate::Invoke

    .method public hidebysig newslot virtual 
            instance class [mscorlib]System.IAsyncResult 
            BeginInvoke(uint8* ptr,
                        class [mscorlib]System.AsyncCallback callback,
                        object 'object') runtime managed
    {
    } // end of method UnsafeDelegate::BeginInvoke

    .method public hidebysig newslot virtual 
            instance void  EndInvoke(class [mscorlib]System.IAsyncResult result) runtime managed
    {
    } // end of method UnsafeDelegate::EndInvoke

  } // end of class UnsafeDelegate

  .field private class ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/UnsafeDelegate unsafeDelegate
  .field private static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/UnsafeDelegate staticUnsafeDelegate
  .method public hidebysig specialname instance int32* 
          get_NullPointer() cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  conv.u
    IL_0002:  ret
  } // end of method UnsafeCode::get_NullPointer

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       18 (0x12)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UnsafeStaticMethod(uint8*)
    IL_0007:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/UnsafeDelegate::.ctor(object,
                                                                                                                      native int)
    IL_000c:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/UnsafeDelegate ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::staticUnsafeDelegate
    IL_0011:  ret
  } // end of method UnsafeCode::.cctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       25 (0x19)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.0
    IL_0008:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UnsafeMethod(uint8*)
    IL_000e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/UnsafeDelegate::.ctor(object,
                                                                                                                      native int)
    IL_0013:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/UnsafeDelegate ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::unsafeDelegate
    IL_0018:  ret
  } // end of method UnsafeCode::.ctor

  .method public hidebysig instance int32 
          SizeOf() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct
    IL_0006:  ret
  } // end of method UnsafeCode::SizeOf

  .method private hidebysig static void  UseBool(bool b) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method UnsafeCode::UseBool

  .method private hidebysig instance void 
          UnsafeMethod(uint8* ptr) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method UnsafeCode::UnsafeMethod

  .method private hidebysig static void  UnsafeStaticMethod(uint8* ptr) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method UnsafeCode::UnsafeStaticMethod

  .method public hidebysig instance void 
          PointerComparison(int32* a,
                            float64* b) cil managed
  {
    // Code size       64 (0x40)
    .maxstack  2
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ceq
    IL_0004:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseBool(bool)
    IL_0009:  ldarg.1
    IL_000a:  ldarg.2
    IL_000b:  ceq
    IL_000d:  ldc.i4.0
    IL_000e:  ceq
    IL_0010:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseBool(bool)
    IL_0015:  ldarg.1
    IL_0016:  ldarg.2
    IL_0017:  clt.un
    IL_0019:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseBool(bool)
    IL_001e:  ldarg.1
    IL_001f:  ldarg.2
    IL_0020:  cgt.un
    IL_0022:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseBool(bool)
    IL_0027:  ldarg.1
    IL_0028:  ldarg.2
    IL_0029:  cgt.un
    IL_002b:  ldc.i4.0
    IL_002c:  ceq
    IL_002e:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseBool(bool)
    IL_0033:  ldarg.1
    IL_0034:  ldarg.2
    IL_0035:  clt.un
    IL_0037:  ldc.i4.0
    IL_0038:  ceq
    IL_003a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseBool(bool)
    IL_003f:  ret
  } // end of method UnsafeCode::PointerComparison

  .method public hidebysig instance void 
          PointerComparisonWithNull(int32* a) cil managed
  {
    // Code size       24 (0x18)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.0
    IL_0002:  conv.u
    IL_0003:  ceq
    IL_0005:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseBool(bool)
    IL_000a:  ldarg.1
    IL_000b:  ldc.i4.0
    IL_000c:  conv.u
    IL_000d:  ceq
    IL_000f:  ldc.i4.0
    IL_0010:  ceq
    IL_0012:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseBool(bool)
    IL_0017:  ret
  } // end of method UnsafeCode::PointerComparisonWithNull

  .method public hidebysig instance int32* 
          PointerCast(int64* p) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ret
  } // end of method UnsafeCode::PointerCast

  .method public hidebysig instance int64 
          ConvertDoubleToLong(float64 d) cil managed
  {
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  ldarga.s   d
    IL_0002:  conv.u
    IL_0003:  ldind.i8
    IL_0004:  ret
  } // end of method UnsafeCode::ConvertDoubleToLong

  .method public hidebysig instance float64 
          ConvertLongToDouble(int64 d) cil managed
  {
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  ldarga.s   d
    IL_0002:  conv.u
    IL_0003:  ldind.r8
    IL_0004:  ret
  } // end of method UnsafeCode::ConvertLongToDouble

  .method public hidebysig instance int32 
          ConvertFloatToInt(float32 d) cil managed
  {
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  ldarga.s   d
    IL_0002:  conv.u
    IL_0003:  ldind.i4
    IL_0004:  ret
  } // end of method UnsafeCode::ConvertFloatToInt

  .method public hidebysig instance float32 
          ConvertIntToFloat(int32 d) cil managed
  {
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  ldarga.s   d
    IL_0002:  conv.u
    IL_0003:  ldind.r4
    IL_0004:  ret
  } // end of method UnsafeCode::ConvertIntToFloat

  .method public hidebysig instance int32 
          PointerCasts() cil managed
  {
    // Code size       21 (0x15)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  conv.u
    IL_0005:  ldc.r4     0.5
    IL_000a:  stind.r4
    IL_000b:  ldloca.s   V_0
    IL_000d:  conv.u
    IL_000e:  ldc.i4.3
    IL_000f:  conv.i
    IL_0010:  add
    IL_0011:  ldc.i4.3
    IL_0012:  stind.i1
    IL_0013:  ldloc.0
    IL_0014:  ret
  } // end of method UnsafeCode::PointerCasts

  .method public hidebysig instance void 
          PassRefParameterAsPointer(int32& p) cil managed
  {
    // Code size       15 (0xf)
    .maxstack  2
    .locals init (int32& pinned V_0)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldarg.0
    IL_0003:  ldloc.0
    IL_0004:  conv.i
    IL_0005:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UsePointer(int32*)
    IL_000a:  pop
    IL_000b:  ldc.i4.0
    IL_000c:  conv.u
    IL_000d:  stloc.0
    IL_000e:  ret
  } // end of method UnsafeCode::PassRefParameterAsPointer

  .method public hidebysig instance void 
          PassPointerAsRefParameter(int32* p) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseReference(int32&)
    IL_0007:  ret
  } // end of method UnsafeCode::PassPointerAsRefParameter

  .method public hidebysig instance void 
          PassPointerCastAsRefParameter(uint32* p) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseReference(int32&)
    IL_0007:  ret
  } // end of method UnsafeCode::PassPointerCastAsRefParameter

  .method public hidebysig instance void 
          AddressInMultiDimensionalArray(float64[0...,0...] matrix) cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  3
    .locals init (float64& pinned V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.1
    IL_0002:  ldc.i4.2
    IL_0003:  call       instance float64& float64[0...,0...]::Address(int32,
                                                                       int32)
    IL_0008:  stloc.0
    IL_0009:  ldarg.0
    IL_000a:  ldloc.0
    IL_000b:  conv.i
    IL_000c:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::PointerReferenceExpression(float64*)
    IL_0011:  pop
    IL_0012:  ldarg.0
    IL_0013:  ldloc.0
    IL_0014:  conv.i
    IL_0015:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::PointerReferenceExpression(float64*)
    IL_001a:  pop
    IL_001b:  ldc.i4.0
    IL_001c:  conv.u
    IL_001d:  stloc.0
    IL_001e:  ret
  } // end of method UnsafeCode::AddressInMultiDimensionalArray

  .method public hidebysig instance void 
          FixedStringAccess(string text) cil managed
  {
    // Code size       36 (0x24)
    .maxstack  2
    .locals init (char* V_0,
             char* V_1,
             string pinned V_2)
    IL_0000:  ldarg.1
    IL_0001:  stloc.2
    IL_0002:  ldloc.2
    IL_0003:  conv.i
    IL_0004:  dup
    IL_0005:  brfalse.s  IL_000d

    IL_0007:  call       int32 [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::get_OffsetToStringData()
    IL_000c:  add
    IL_000d:  stloc.0
    IL_000e:  ldloc.0
    IL_000f:  stloc.1
    IL_0010:  br.s       IL_001b

    IL_0012:  ldloc.1
    IL_0013:  ldc.i4.s   65
    IL_0015:  stind.i2
    IL_0016:  ldloc.1
    IL_0017:  ldc.i4.2
    IL_0018:  conv.i
    IL_0019:  add
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  ldind.u2
    IL_001d:  ldc.i4.s   97
    IL_001f:  beq.s      IL_0012

    IL_0021:  ldnull
    IL_0022:  stloc.2
    IL_0023:  ret
  } // end of method UnsafeCode::FixedStringAccess

  .method public hidebysig instance void 
          PutDoubleIntoLongArray1(int64[] 'array',
                                  int32 index,
                                  float64 val) cil managed
  {
    // Code size       36 (0x24)
    .maxstack  3
    .locals init (int64& pinned V_0,
             int64[] V_1)
    IL_0000:  ldarg.1
    IL_0001:  dup
    IL_0002:  stloc.1
    IL_0003:  brfalse.s  IL_000a

    IL_0005:  ldloc.1
    IL_0006:  ldlen
    IL_0007:  conv.i4
    IL_0008:  brtrue.s   IL_000f

    IL_000a:  ldc.i4.0
    IL_000b:  conv.u
    IL_000c:  stloc.0
    IL_000d:  br.s       IL_0017

    IL_000f:  ldloc.1
    IL_0010:  ldc.i4.0
    IL_0011:  ldelema    [mscorlib]System.Int64
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  conv.i
    IL_0019:  ldarg.2
    IL_001a:  conv.i
    IL_001b:  ldc.i4.8
    IL_001c:  mul
    IL_001d:  add
    IL_001e:  ldarg.3
    IL_001f:  stind.r8
    IL_0020:  ldc.i4.0
    IL_0021:  conv.u
    IL_0022:  stloc.0
    IL_0023:  ret
  } // end of method UnsafeCode::PutDoubleIntoLongArray1

  .method public hidebysig instance void 
          PutDoubleIntoLongArray2(int64[] 'array',
                                  int32 index,
                                  float64 val) cil managed
  {
    // Code size       16 (0x10)
    .maxstack  2
    .locals init (int64& pinned V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldelema    [mscorlib]System.Int64
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  conv.i
    IL_000a:  ldarg.3
    IL_000b:  stind.r8
    IL_000c:  ldc.i4.0
    IL_000d:  conv.u
    IL_000e:  stloc.0
    IL_000f:  ret
  } // end of method UnsafeCode::PutDoubleIntoLongArray2

  .method public hidebysig instance string 
          PointerReferenceExpression(float64* d) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  call       instance string [mscorlib]System.Double::ToString()
    IL_0006:  ret
  } // end of method UnsafeCode::PointerReferenceExpression

  .method public hidebysig instance string 
          PointerReferenceExpression2(int64 addr) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  conv.u
    IL_0002:  call       instance string [mscorlib]System.Int32::ToString()
    IL_0007:  ret
  } // end of method UnsafeCode::PointerReferenceExpression2

  .method public hidebysig instance int32* 
          PointerArithmetic(int32* p) cil managed
  {
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.8
    IL_0002:  conv.i
    IL_0003:  add
    IL_0004:  ret
  } // end of method UnsafeCode::PointerArithmetic

  .method public hidebysig instance int64* 
          PointerArithmetic2(int64* p) cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldc.i4.s   24
    IL_0002:  conv.i
    IL_0003:  ldarg.1
    IL_0004:  add
    IL_0005:  ret
  } // end of method UnsafeCode::PointerArithmetic2

  .method public hidebysig instance int64* 
          PointerArithmetic3(int64* p) cil managed
  {
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.3
    IL_0002:  conv.i
    IL_0003:  add
    IL_0004:  ret
  } // end of method UnsafeCode::PointerArithmetic3

  .method public hidebysig instance int64* 
          PointerArithmetic4(void* p) cil managed
  {
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.3
    IL_0002:  conv.i
    IL_0003:  add
    IL_0004:  ret
  } // end of method UnsafeCode::PointerArithmetic4

  .method public hidebysig instance int32 
          PointerArithmetic5(void* p,
                             uint8* q,
                             int32 i) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.2
    IL_0001:  ldarg.3
    IL_0002:  add
    IL_0003:  ldind.u1
    IL_0004:  ldarg.1
    IL_0005:  ldind.u1
    IL_0006:  add
    IL_0007:  ret
  } // end of method UnsafeCode::PointerArithmetic5

  .method public hidebysig instance int32 
          PointerArithmetic6(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* p,
                             int32 i) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  conv.i
    IL_0003:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct
    IL_0009:  mul
    IL_000a:  add
    IL_000b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct::X
    IL_0010:  ret
  } // end of method UnsafeCode::PointerArithmetic6

  .method public hidebysig instance int32* 
          PointerArithmeticLong1(int32* p,
                                 int64 offset) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldc.i4.4
    IL_0003:  conv.i8
    IL_0004:  mul
    IL_0005:  conv.i
    IL_0006:  add
    IL_0007:  ret
  } // end of method UnsafeCode::PointerArithmeticLong1

  .method public hidebysig instance int32* 
          PointerArithmeticLong2(int32* p,
                                 int64 offset) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.2
    IL_0001:  ldc.i4.4
    IL_0002:  conv.i8
    IL_0003:  mul
    IL_0004:  conv.i
    IL_0005:  ldarg.1
    IL_0006:  add
    IL_0007:  ret
  } // end of method UnsafeCode::PointerArithmeticLong2

  .method public hidebysig instance int32* 
          PointerArithmeticLong3(int32* p,
                                 int64 offset) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldc.i4.4
    IL_0003:  conv.i8
    IL_0004:  mul
    IL_0005:  conv.i
    IL_0006:  sub
    IL_0007:  ret
  } // end of method UnsafeCode::PointerArithmeticLong3

  .method public hidebysig instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* 
          PointerArithmeticLong1s(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* p,
                                  int64 offset) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct
    IL_0008:  conv.i8
    IL_0009:  mul
    IL_000a:  conv.i
    IL_000b:  add
    IL_000c:  ret
  } // end of method UnsafeCode::PointerArithmeticLong1s

  .method public hidebysig instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* 
          PointerArithmeticLong2s(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* p,
                                  int64 offset) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.2
    IL_0001:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct
    IL_0007:  conv.i8
    IL_0008:  mul
    IL_0009:  conv.i
    IL_000a:  ldarg.1
    IL_000b:  add
    IL_000c:  ret
  } // end of method UnsafeCode::PointerArithmeticLong2s

  .method public hidebysig instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* 
          PointerArithmeticLong3s(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* p,
                                  int64 offset) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct
    IL_0008:  conv.i8
    IL_0009:  mul
    IL_000a:  conv.i
    IL_000b:  sub
    IL_000c:  ret
  } // end of method UnsafeCode::PointerArithmeticLong3s

  .method public hidebysig instance int32 
          PointerSubtraction(int64* p,
                             int64* q) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  sub
    IL_0003:  ldc.i4.8
    IL_0004:  div
    IL_0005:  conv.i8
    IL_0006:  conv.i4
    IL_0007:  ret
  } // end of method UnsafeCode::PointerSubtraction

  .method public hidebysig instance int64 
          PointerSubtractionLong(int64* p,
                                 int64* q) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  sub
    IL_0003:  ldc.i4.8
    IL_0004:  div
    IL_0005:  conv.i8
    IL_0006:  ret
  } // end of method UnsafeCode::PointerSubtractionLong

  .method public hidebysig instance int32 
          PointerSubtraction2(int64* p,
                              int16* q) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  sub
    IL_0003:  ldc.i4.1
    IL_0004:  div
    IL_0005:  conv.i8
    IL_0006:  conv.i4
    IL_0007:  ret
  } // end of method UnsafeCode::PointerSubtraction2

  .method public hidebysig instance int32 
          PointerSubtraction3(void* p,
                              void* q) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  sub
    IL_0003:  ldc.i4.1
    IL_0004:  div
    IL_0005:  conv.i8
    IL_0006:  conv.i4
    IL_0007:  ret
  } // end of method UnsafeCode::PointerSubtraction3

  .method public hidebysig instance int64 
          PointerSubtraction4(int8* p,
                              int8* q) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  sub
    IL_0003:  ldc.i4.1
    IL_0004:  div
    IL_0005:  conv.i8
    IL_0006:  ret
  } // end of method UnsafeCode::PointerSubtraction4

  .method public hidebysig instance int64 
          PointerSubtraction5(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* p,
                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* q) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  sub
    IL_0003:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct
    IL_0009:  div
    IL_000a:  conv.i8
    IL_000b:  ret
  } // end of method UnsafeCode::PointerSubtraction5

  .method public hidebysig instance float64 
          FixedMemberAccess(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers* m,
                            int32 i) cil managed
  {
    // Code size       39 (0x27)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers::Integers
    IL_0006:  ldflda     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer0'::FixedElementField
    IL_000b:  conv.u
    IL_000c:  ldarg.2
    IL_000d:  conv.i
    IL_000e:  ldc.i4.4
    IL_000f:  mul
    IL_0010:  add
    IL_0011:  ldind.i4
    IL_0012:  conv.r8
    IL_0013:  ldarg.1
    IL_0014:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Doubles>e__FixedBuffer1' ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers::Doubles
    IL_0019:  ldflda     float64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Doubles>e__FixedBuffer1'::FixedElementField
    IL_001e:  conv.u
    IL_001f:  ldarg.2
    IL_0020:  conv.i
    IL_0021:  ldc.i4.8
    IL_0022:  mul
    IL_0023:  add
    IL_0024:  ldind.r8
    IL_0025:  add
    IL_0026:  ret
  } // end of method UnsafeCode::FixedMemberAccess

  .method public hidebysig instance float64* 
          FixedMemberBasePointer(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers* m) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Doubles>e__FixedBuffer1' ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers::Doubles
    IL_0006:  ldflda     float64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Doubles>e__FixedBuffer1'::FixedElementField
    IL_000b:  conv.u
    IL_000c:  ret
  } // end of method UnsafeCode::FixedMemberBasePointer

  .method public hidebysig instance void 
          UseFixedMemberAsPointer(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers* m) cil managed
  {
    // Code size       20 (0x14)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers::Integers
    IL_0007:  ldflda     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer0'::FixedElementField
    IL_000c:  conv.u
    IL_000d:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UsePointer(int32*)
    IL_0012:  pop
    IL_0013:  ret
  } // end of method UnsafeCode::UseFixedMemberAsPointer

  .method public hidebysig instance void 
          UseFixedMemberAsReference(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers* m) cil managed
  {
    // Code size       40 (0x28)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers::Integers
    IL_0007:  ldflda     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer0'::FixedElementField
    IL_000c:  conv.u
    IL_000d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseReference(int32&)
    IL_0012:  ldarg.0
    IL_0013:  ldarg.1
    IL_0014:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers::Integers
    IL_0019:  ldflda     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer0'::FixedElementField
    IL_001e:  conv.u
    IL_001f:  ldc.i4.4
    IL_0020:  conv.i
    IL_0021:  add
    IL_0022:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseReference(int32&)
    IL_0027:  ret
  } // end of method UnsafeCode::UseFixedMemberAsReference

  .method public hidebysig instance void 
          PinFixedMember(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers& m) cil managed
  {
    // Code size       25 (0x19)
    .maxstack  2
    .locals init (int32& pinned V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer0' ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers::Integers
    IL_0006:  ldflda     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer0'::FixedElementField
    IL_000b:  stloc.0
    IL_000c:  ldarg.0
    IL_000d:  ldloc.0
    IL_000e:  conv.i
    IL_000f:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UsePointer(int32*)
    IL_0014:  pop
    IL_0015:  ldc.i4.0
    IL_0016:  conv.u
    IL_0017:  stloc.0
    IL_0018:  ret
  } // end of method UnsafeCode::PinFixedMember

  .method private hidebysig instance void 
          UseReference(int32& i) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method UnsafeCode::UseReference

  .method public hidebysig instance string 
          UsePointer(int32* ptr) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  call       instance string [mscorlib]System.Int32::ToString()
    IL_0006:  ret
  } // end of method UnsafeCode::UsePointer

  .method public hidebysig instance string 
          UsePointer(float64* ptr) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  call       instance string [mscorlib]System.Double::ToString()
    IL_0006:  ret
  } // end of method UnsafeCode::UsePointer

  .method public hidebysig instance string 
          StackAlloc(int32 count) cil managed
  {
    // Code size       52 (0x34)
    .maxstack  3
    .locals init (char* V_0,
             char* V_1,
             int32 V_2)
    IL_0000:  ldarg.1
    IL_0001:  conv.u
    IL_0002:  ldc.i4.2
    IL_0003:  mul.ovf.un
    IL_0004:  localloc
    IL_0006:  stloc.0
    IL_0007:  ldc.i4.s   100
    IL_0009:  conv.u
    IL_000a:  ldc.i4.2
    IL_000b:  mul.ovf.un
    IL_000c:  localloc
    IL_000e:  stloc.1
    IL_000f:  ldc.i4.0
    IL_0010:  stloc.2
    IL_0011:  br.s       IL_0028

    IL_0013:  ldloc.0
    IL_0014:  ldloc.2
    IL_0015:  conv.i
    IL_0016:  ldc.i4.2
    IL_0017:  mul
    IL_0018:  add
    IL_0019:  ldloc.2
    IL_001a:  conv.u2
    IL_001b:  stind.i2
    IL_001c:  ldloc.1
    IL_001d:  ldloc.2
    IL_001e:  conv.i
    IL_001f:  ldc.i4.2
    IL_0020:  mul
    IL_0021:  add
    IL_0022:  ldc.i4.0
    IL_0023:  stind.i2
    IL_0024:  ldloc.2
    IL_0025:  ldc.i4.1
    IL_0026:  add
    IL_0027:  stloc.2
    IL_0028:  ldloc.2
    IL_0029:  ldarg.1
    IL_002a:  blt.s      IL_0013

    IL_002c:  ldarg.0
    IL_002d:  ldloc.0
    IL_002e:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UsePointer(float64*)
    IL_0033:  ret
  } // end of method UnsafeCode::StackAlloc

  .method public hidebysig instance string 
          StackAllocStruct(int32 count) cil managed
  {
    // Code size       97 (0x61)
    .maxstack  3
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* V_0,
             int32 V_1)
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.2
    IL_0002:  mul.ovf
    IL_0003:  conv.u
    IL_0004:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct
    IL_000a:  mul.ovf.un
    IL_000b:  localloc
    IL_000d:  stloc.0
    IL_000e:  ldc.i4.s   10
    IL_0010:  conv.u
    IL_0011:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct
    IL_0017:  mul.ovf.un
    IL_0018:  localloc
    IL_001a:  pop
    IL_001b:  ldloc.0
    IL_001c:  ldarg.1
    IL_001d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct::X
    IL_0022:  ldloc.0
    IL_0023:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct
    IL_0029:  add
    IL_002a:  ldloc.0
    IL_002b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct::X
    IL_0030:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct::X
    IL_0035:  ldc.i4.2
    IL_0036:  stloc.1
    IL_0037:  br.s       IL_004e

    IL_0039:  ldloc.0
    IL_003a:  ldloc.1
    IL_003b:  conv.i
    IL_003c:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct
    IL_0042:  mul
    IL_0043:  add
    IL_0044:  ldarg.1
    IL_0045:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct::X
    IL_004a:  ldloc.1
    IL_004b:  ldc.i4.1
    IL_004c:  add
    IL_004d:  stloc.1
    IL_004e:  ldloc.1
    IL_004f:  ldc.i4.s   10
    IL_0051:  blt.s      IL_0039

    IL_0053:  ldarg.0
    IL_0054:  ldloc.0
    IL_0055:  ldflda     float64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct::Y
    IL_005a:  conv.u
    IL_005b:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UsePointer(float64*)
    IL_0060:  ret
  } // end of method UnsafeCode::StackAllocStruct

  .method family hidebysig virtual instance void 
          Finalize() cil managed
  {
    // Code size       22 (0x16)
    .maxstack  2
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  ldarg.0
      IL_0002:  call       instance int32* ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::get_NullPointer()
      IL_0007:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::PassPointerAsRefParameter(int32*)
      IL_000c:  leave.s    IL_0015

    }  // end .try
    finally
    {
      IL_000e:  ldarg.0
      IL_000f:  call       instance void [mscorlib]System.Object::Finalize()
      IL_0014:  endfinally
    }  // end handler
    IL_0015:  ret
  } // end of method UnsafeCode::Finalize

  .method private hidebysig instance void 
          Issue990() cil managed
  {
    // Code size       37 (0x25)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/Data V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/Data* V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/Data
    IL_0008:  ldloca.s   V_0
    IL_000a:  conv.u
    IL_000b:  stloc.1
    IL_000c:  ldarg.0
    IL_000d:  ldloc.1
    IL_000e:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/Vector ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/Data::Position
    IL_0013:  constrained. ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/Vector
    IL_0019:  callvirt   instance int32 [mscorlib]System.Object::GetHashCode()
    IL_001e:  call       instance float32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::ConvertIntToFloat(int32)
    IL_0023:  pop
    IL_0024:  ret
  } // end of method UnsafeCode::Issue990

  .property instance int32* NullPointer()
  {
    .get instance int32* ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::get_NullPointer()
  } // end of property UnsafeCode::NullPointer
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\UnsafeCode.opt.res
