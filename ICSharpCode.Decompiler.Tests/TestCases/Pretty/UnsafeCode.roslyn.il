
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly UnsafeCode
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
.module UnsafeCode.dll
// MVID: {34429304-3762-4E40-977B-76B197A03CDA}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00C50000


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
    .class sequential ansi sealed nested public beforefieldinit '<Integers>e__FixedBuffer'
           extends [mscorlib]System.ValueType
    {
      .pack 0
      .size 400
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.UnsafeValueTypeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public int32 FixedElementField
    } // end of class '<Integers>e__FixedBuffer'

    .class sequential ansi sealed nested public beforefieldinit '<Doubles>e__FixedBuffer'
           extends [mscorlib]System.ValueType
    {
      .pack 0
      .size 1600
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.UnsafeValueTypeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public float64 FixedElementField
    } // end of class '<Doubles>e__FixedBuffer'

    .class sequential ansi sealed nested public beforefieldinit '<Old>e__FixedBuffer'
           extends [mscorlib]System.ValueType
    {
      .pack 0
      .size 1
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      .custom instance void [mscorlib]System.Runtime.CompilerServices.UnsafeValueTypeAttribute::.ctor() = ( 01 00 00 00 ) 
      .field public uint8 FixedElementField
    } // end of class '<Old>e__FixedBuffer'

    .field public valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer' Integers
    .custom instance void [mscorlib]System.Runtime.CompilerServices.FixedBufferAttribute::.ctor(class [mscorlib]System.Type,
                                                                                                int32) = ( 01 00 59 53 79 73 74 65 6D 2E 49 6E 74 33 32 2C   // ..YSystem.Int32,
                                                                                                           20 6D 73 63 6F 72 6C 69 62 2C 20 56 65 72 73 69   //  mscorlib, Versi
                                                                                                           6F 6E 3D 34 2E 30 2E 30 2E 30 2C 20 43 75 6C 74   // on=4.0.0.0, Cult
                                                                                                           75 72 65 3D 6E 65 75 74 72 61 6C 2C 20 50 75 62   // ure=neutral, Pub
                                                                                                           6C 69 63 4B 65 79 54 6F 6B 65 6E 3D 62 37 37 61   // licKeyToken=b77a
                                                                                                           35 63 35 36 31 39 33 34 65 30 38 39 64 00 00 00   // 5c561934e089d...
                                                                                                           00 00 ) 
    .field public int32 NormalMember
    .field public valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Doubles>e__FixedBuffer' Doubles
    .custom instance void [mscorlib]System.Runtime.CompilerServices.FixedBufferAttribute::.ctor(class [mscorlib]System.Type,
                                                                                                int32) = ( 01 00 5A 53 79 73 74 65 6D 2E 44 6F 75 62 6C 65   // ..ZSystem.Double
                                                                                                           2C 20 6D 73 63 6F 72 6C 69 62 2C 20 56 65 72 73   // , mscorlib, Vers
                                                                                                           69 6F 6E 3D 34 2E 30 2E 30 2E 30 2C 20 43 75 6C   // ion=4.0.0.0, Cul
                                                                                                           74 75 72 65 3D 6E 65 75 74 72 61 6C 2C 20 50 75   // ture=neutral, Pu
                                                                                                           62 6C 69 63 4B 65 79 54 6F 6B 65 6E 3D 62 37 37   // blicKeyToken=b77
                                                                                                           61 35 63 35 36 31 39 33 34 65 30 38 39 C8 00 00   // a5c561934e089...
                                                                                                           00 00 00 ) 
    .field public valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Old>e__FixedBuffer' Old
    .custom instance void [mscorlib]System.Runtime.CompilerServices.FixedBufferAttribute::.ctor(class [mscorlib]System.Type,
                                                                                                int32) = ( 01 00 58 53 79 73 74 65 6D 2E 42 79 74 65 2C 20   // ..XSystem.Byte, 
                                                                                                           6D 73 63 6F 72 6C 69 62 2C 20 56 65 72 73 69 6F   // mscorlib, Versio
                                                                                                           6E 3D 34 2E 30 2E 30 2E 30 2C 20 43 75 6C 74 75   // n=4.0.0.0, Cultu
                                                                                                           72 65 3D 6E 65 75 74 72 61 6C 2C 20 50 75 62 6C   // re=neutral, Publ
                                                                                                           69 63 4B 65 79 54 6F 6B 65 6E 3D 62 37 37 61 35   // icKeyToken=b77a5
                                                                                                           63 35 36 31 39 33 34 65 30 38 39 01 00 00 00 00   // c561934e089.....
                                                                                                           00 ) 
    .custom instance void [mscorlib]System.ObsoleteAttribute::.ctor(string) = ( 01 00 11 61 6E 6F 74 68 65 72 20 61 74 74 72 69   // ...another attri
                                                                                62 75 74 65 00 00 )                               // bute..
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
      // Code size       7 (0x7)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  stloc.0
      IL_0003:  br.s       IL_0005

      IL_0005:  ldloc.0
      IL_0006:  ret
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
  } // end of method UnsafeCode::get_NullPointer

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       19 (0x13)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UnsafeStaticMethod(uint8*)
    IL_0008:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/UnsafeDelegate::.ctor(object,
                                                                                                                      native int)
    IL_000d:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/UnsafeDelegate ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::staticUnsafeDelegate
    IL_0012:  ret
  } // end of method UnsafeCode::.cctor

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  nop
    IL_0008:  ldarg.0
    IL_0009:  ldarg.0
    IL_000a:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UnsafeMethod(uint8*)
    IL_0010:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/UnsafeDelegate::.ctor(object,
                                                                                                                      native int)
    IL_0015:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/UnsafeDelegate ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::unsafeDelegate
    IL_001a:  ret
  } // end of method UnsafeCode::.ctor

  .method public hidebysig instance int32 
          SizeOf() cil managed
  {
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method UnsafeCode::SizeOf

  .method private hidebysig static void  UseBool(bool b) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method UnsafeCode::UseBool

  .method private hidebysig instance void 
          UnsafeMethod(uint8* ptr) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method UnsafeCode::UnsafeMethod

  .method private hidebysig static void  UnsafeStaticMethod(uint8* ptr) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method UnsafeCode::UnsafeStaticMethod

  .method public hidebysig instance void 
          PointerComparison(int32* a,
                            float64* b) cil managed
  {
    // Code size       71 (0x47)
    .maxstack  2
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ceq
    IL_0005:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseBool(bool)
    IL_000a:  nop
    IL_000b:  ldarg.1
    IL_000c:  ldarg.2
    IL_000d:  ceq
    IL_000f:  ldc.i4.0
    IL_0010:  ceq
    IL_0012:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseBool(bool)
    IL_0017:  nop
    IL_0018:  ldarg.1
    IL_0019:  ldarg.2
    IL_001a:  clt.un
    IL_001c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseBool(bool)
    IL_0021:  nop
    IL_0022:  ldarg.1
    IL_0023:  ldarg.2
    IL_0024:  cgt.un
    IL_0026:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseBool(bool)
    IL_002b:  nop
    IL_002c:  ldarg.1
    IL_002d:  ldarg.2
    IL_002e:  cgt.un
    IL_0030:  ldc.i4.0
    IL_0031:  ceq
    IL_0033:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseBool(bool)
    IL_0038:  nop
    IL_0039:  ldarg.1
    IL_003a:  ldarg.2
    IL_003b:  clt.un
    IL_003d:  ldc.i4.0
    IL_003e:  ceq
    IL_0040:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseBool(bool)
    IL_0045:  nop
    IL_0046:  ret
  } // end of method UnsafeCode::PointerComparison

  .method public hidebysig instance void 
          PointerComparisonWithNull(int32* a) cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.0
    IL_0003:  conv.u
    IL_0004:  ceq
    IL_0006:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseBool(bool)
    IL_000b:  nop
    IL_000c:  ldarg.1
    IL_000d:  ldc.i4.0
    IL_000e:  conv.u
    IL_000f:  ceq
    IL_0011:  ldc.i4.0
    IL_0012:  ceq
    IL_0014:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseBool(bool)
    IL_0019:  nop
    IL_001a:  ret
  } // end of method UnsafeCode::PointerComparisonWithNull

  .method public hidebysig instance int32* 
          PointerCast(int64* p) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32* V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method UnsafeCode::PointerCast

  .method public hidebysig instance int64 
          ConvertDoubleToLong(float64 d) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (int64 V_0)
    IL_0000:  nop
    IL_0001:  ldarga.s   d
    IL_0003:  conv.u
    IL_0004:  ldind.i8
    IL_0005:  stloc.0
    IL_0006:  br.s       IL_0008

    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method UnsafeCode::ConvertDoubleToLong

  .method public hidebysig instance float64 
          ConvertLongToDouble(int64 d) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (float64 V_0)
    IL_0000:  nop
    IL_0001:  ldarga.s   d
    IL_0003:  conv.u
    IL_0004:  ldind.r8
    IL_0005:  stloc.0
    IL_0006:  br.s       IL_0008

    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method UnsafeCode::ConvertLongToDouble

  .method public hidebysig instance int32 
          ConvertFloatToInt(float32 d) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarga.s   d
    IL_0003:  conv.u
    IL_0004:  ldind.i4
    IL_0005:  stloc.0
    IL_0006:  br.s       IL_0008

    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method UnsafeCode::ConvertFloatToInt

  .method public hidebysig instance float32 
          ConvertIntToFloat(int32 d) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (float32 V_0)
    IL_0000:  nop
    IL_0001:  ldarga.s   d
    IL_0003:  conv.u
    IL_0004:  ldind.r4
    IL_0005:  stloc.0
    IL_0006:  br.s       IL_0008

    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method UnsafeCode::ConvertIntToFloat

  .method public hidebysig instance int32 
          PointerCasts() cil managed
  {
    // Code size       25 (0x19)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  conv.u
    IL_0006:  ldc.r4     0.5
    IL_000b:  stind.r4
    IL_000c:  ldloca.s   V_0
    IL_000e:  conv.u
    IL_000f:  ldc.i4.3
    IL_0010:  add
    IL_0011:  ldc.i4.3
    IL_0012:  stind.i1
    IL_0013:  ldloc.0
    IL_0014:  stloc.1
    IL_0015:  br.s       IL_0017

    IL_0017:  ldloc.1
    IL_0018:  ret
  } // end of method UnsafeCode::PointerCasts

  .method public hidebysig instance void 
          PassRefParameterAsPointer(int32& p) cil managed
  {
    // Code size       18 (0x12)
    .maxstack  2
    .locals init (int32& pinned V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  nop
    IL_0004:  ldarg.0
    IL_0005:  ldloc.0
    IL_0006:  conv.i
    IL_0007:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UsePointer(int32*)
    IL_000c:  pop
    IL_000d:  nop
    IL_000e:  ldc.i4.0
    IL_000f:  conv.u
    IL_0010:  stloc.0
    IL_0011:  ret
  } // end of method UnsafeCode::PassRefParameterAsPointer

  .method public hidebysig instance void 
          PassPointerAsRefParameter(int32* p) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.1
    IL_0003:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseReference(int32&)
    IL_0008:  nop
    IL_0009:  ret
  } // end of method UnsafeCode::PassPointerAsRefParameter

  .method public hidebysig instance void 
          PassPointerCastAsRefParameter(uint32* p) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.1
    IL_0003:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseReference(int32&)
    IL_0008:  nop
    IL_0009:  ret
  } // end of method UnsafeCode::PassPointerCastAsRefParameter

  .method public hidebysig instance void 
          AddressInMultiDimensionalArray(float64[0...,0...] matrix) cil managed
  {
    // Code size       34 (0x22)
    .maxstack  3
    .locals init (float64& pinned V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.1
    IL_0003:  ldc.i4.2
    IL_0004:  call       instance float64& float64[0...,0...]::Address(int32,
                                                                       int32)
    IL_0009:  stloc.0
    IL_000a:  nop
    IL_000b:  ldarg.0
    IL_000c:  ldloc.0
    IL_000d:  conv.i
    IL_000e:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::PointerReferenceExpression(float64*)
    IL_0013:  pop
    IL_0014:  ldarg.0
    IL_0015:  ldloc.0
    IL_0016:  conv.i
    IL_0017:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::PointerReferenceExpression(float64*)
    IL_001c:  pop
    IL_001d:  nop
    IL_001e:  ldc.i4.0
    IL_001f:  conv.u
    IL_0020:  stloc.0
    IL_0021:  ret
  } // end of method UnsafeCode::AddressInMultiDimensionalArray

  .method public hidebysig instance void 
          FixedStringAccess(string text) cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  2
    .locals init (char* V_0,
             string pinned V_1,
             char* V_2,
             bool V_3)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.1
    IL_0003:  ldloc.1
    IL_0004:  conv.i
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  brfalse.s  IL_0011

    IL_0009:  ldloc.0
    IL_000a:  call       int32 [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::get_OffsetToStringData()
    IL_000f:  add
    IL_0010:  stloc.0
    IL_0011:  nop
    IL_0012:  ldloc.0
    IL_0013:  stloc.2
    IL_0014:  br.s       IL_0020

    IL_0016:  nop
    IL_0017:  ldloc.2
    IL_0018:  ldc.i4.s   65
    IL_001a:  stind.i2
    IL_001b:  nop
    IL_001c:  ldloc.2
    IL_001d:  ldc.i4.2
    IL_001e:  add
    IL_001f:  stloc.2
    IL_0020:  ldloc.2
    IL_0021:  ldind.u2
    IL_0022:  ldc.i4.s   97
    IL_0024:  ceq
    IL_0026:  stloc.3
    IL_0027:  ldloc.3
    IL_0028:  brtrue.s   IL_0016

    IL_002a:  nop
    IL_002b:  ldnull
    IL_002c:  stloc.1
    IL_002d:  ret
  } // end of method UnsafeCode::FixedStringAccess

  .method public hidebysig instance void 
          PutDoubleIntoLongArray1(int64[] 'array',
                                  int32 index,
                                  float64 val) cil managed
  {
    // Code size       39 (0x27)
    .maxstack  3
    .locals init (int64& pinned V_0,
             int64[] V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  dup
    IL_0003:  stloc.1
    IL_0004:  brfalse.s  IL_000b

    IL_0006:  ldloc.1
    IL_0007:  ldlen
    IL_0008:  conv.i4
    IL_0009:  brtrue.s   IL_0010

    IL_000b:  ldc.i4.0
    IL_000c:  conv.u
    IL_000d:  stloc.0
    IL_000e:  br.s       IL_0018

    IL_0010:  ldloc.1
    IL_0011:  ldc.i4.0
    IL_0012:  ldelema    [mscorlib]System.Int64
    IL_0017:  stloc.0
    IL_0018:  nop
    IL_0019:  ldloc.0
    IL_001a:  conv.i
    IL_001b:  ldarg.2
    IL_001c:  conv.i
    IL_001d:  ldc.i4.8
    IL_001e:  mul
    IL_001f:  add
    IL_0020:  ldarg.3
    IL_0021:  stind.r8
    IL_0022:  nop
    IL_0023:  ldc.i4.0
    IL_0024:  conv.u
    IL_0025:  stloc.0
    IL_0026:  ret
  } // end of method UnsafeCode::PutDoubleIntoLongArray1

  .method public hidebysig instance void 
          PutDoubleIntoLongArray2(int64[] 'array',
                                  int32 index,
                                  float64 val) cil managed
  {
    // Code size       19 (0x13)
    .maxstack  2
    .locals init (int64& pinned V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldelema    [mscorlib]System.Int64
    IL_0008:  stloc.0
    IL_0009:  nop
    IL_000a:  ldloc.0
    IL_000b:  conv.i
    IL_000c:  ldarg.3
    IL_000d:  stind.r8
    IL_000e:  nop
    IL_000f:  ldc.i4.0
    IL_0010:  conv.u
    IL_0011:  stloc.0
    IL_0012:  ret
  } // end of method UnsafeCode::PutDoubleIntoLongArray2

  .method public hidebysig instance string 
          PointerReferenceExpression(float64* d) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  call       instance string [mscorlib]System.Double::ToString()
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method UnsafeCode::PointerReferenceExpression

  .method public hidebysig instance string 
          PointerReferenceExpression2(int64 addr) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  conv.u
    IL_0003:  call       instance string [mscorlib]System.Int32::ToString()
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method UnsafeCode::PointerReferenceExpression2

  .method public hidebysig instance int32* 
          PointerArithmetic(int32* p) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  3
    .locals init (int32* V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.2
    IL_0003:  conv.i
    IL_0004:  ldc.i4.4
    IL_0005:  mul
    IL_0006:  add
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method UnsafeCode::PointerArithmetic

  .method public hidebysig instance int64* 
          PointerArithmetic2(int64* p) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  2
    .locals init (int64* V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.3
    IL_0002:  conv.i
    IL_0003:  ldc.i4.8
    IL_0004:  mul
    IL_0005:  ldarg.1
    IL_0006:  add
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method UnsafeCode::PointerArithmetic2

  .method public hidebysig instance int64* 
          PointerArithmetic3(int64* p) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  2
    .locals init (int64* V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.3
    IL_0003:  add
    IL_0004:  stloc.0
    IL_0005:  br.s       IL_0007

    IL_0007:  ldloc.0
    IL_0008:  ret
  } // end of method UnsafeCode::PointerArithmetic3

  .method public hidebysig instance int64* 
          PointerArithmetic4(void* p) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  2
    .locals init (int64* V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.3
    IL_0003:  add
    IL_0004:  stloc.0
    IL_0005:  br.s       IL_0007

    IL_0007:  ldloc.0
    IL_0008:  ret
  } // end of method UnsafeCode::PointerArithmetic4

  .method public hidebysig instance int32 
          PointerArithmetic5(void* p,
                             uint8* q,
                             int32 i) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.2
    IL_0002:  ldarg.3
    IL_0003:  add
    IL_0004:  ldind.u1
    IL_0005:  ldarg.1
    IL_0006:  ldind.u1
    IL_0007:  add
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method UnsafeCode::PointerArithmetic5

  .method public hidebysig instance int32 
          PointerArithmetic6(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* p,
                             int32 i) cil managed
  {
    // Code size       22 (0x16)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  conv.i
    IL_0004:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct
    IL_000a:  mul
    IL_000b:  add
    IL_000c:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct::X
    IL_0011:  stloc.0
    IL_0012:  br.s       IL_0014

    IL_0014:  ldloc.0
    IL_0015:  ret
  } // end of method UnsafeCode::PointerArithmetic6

  .method public hidebysig instance int32* 
          PointerArithmeticLong1(int32* p,
                                 int64 offset) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  3
    .locals init (int32* V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldc.i4.4
    IL_0004:  conv.i8
    IL_0005:  mul
    IL_0006:  conv.i
    IL_0007:  add
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method UnsafeCode::PointerArithmeticLong1

  .method public hidebysig instance int32* 
          PointerArithmeticLong2(int32* p,
                                 int64 offset) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  2
    .locals init (int32* V_0)
    IL_0000:  nop
    IL_0001:  ldarg.2
    IL_0002:  ldc.i4.4
    IL_0003:  conv.i8
    IL_0004:  mul
    IL_0005:  conv.i
    IL_0006:  ldarg.1
    IL_0007:  add
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method UnsafeCode::PointerArithmeticLong2

  .method public hidebysig instance int32* 
          PointerArithmeticLong3(int32* p,
                                 int64 offset) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  3
    .locals init (int32* V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldc.i4.4
    IL_0004:  conv.i8
    IL_0005:  mul
    IL_0006:  conv.i
    IL_0007:  sub
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method UnsafeCode::PointerArithmeticLong3

  .method public hidebysig instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* 
          PointerArithmeticLong1s(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* p,
                                  int64 offset) cil managed
  {
    // Code size       18 (0x12)
    .maxstack  3
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct
    IL_0009:  conv.i8
    IL_000a:  mul
    IL_000b:  conv.i
    IL_000c:  add
    IL_000d:  stloc.0
    IL_000e:  br.s       IL_0010

    IL_0010:  ldloc.0
    IL_0011:  ret
  } // end of method UnsafeCode::PointerArithmeticLong1s

  .method public hidebysig instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* 
          PointerArithmeticLong2s(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* p,
                                  int64 offset) cil managed
  {
    // Code size       18 (0x12)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* V_0)
    IL_0000:  nop
    IL_0001:  ldarg.2
    IL_0002:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct
    IL_0008:  conv.i8
    IL_0009:  mul
    IL_000a:  conv.i
    IL_000b:  ldarg.1
    IL_000c:  add
    IL_000d:  stloc.0
    IL_000e:  br.s       IL_0010

    IL_0010:  ldloc.0
    IL_0011:  ret
  } // end of method UnsafeCode::PointerArithmeticLong2s

  .method public hidebysig instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* 
          PointerArithmeticLong3s(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* p,
                                  int64 offset) cil managed
  {
    // Code size       18 (0x12)
    .maxstack  3
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct
    IL_0009:  conv.i8
    IL_000a:  mul
    IL_000b:  conv.i
    IL_000c:  sub
    IL_000d:  stloc.0
    IL_000e:  br.s       IL_0010

    IL_0010:  ldloc.0
    IL_0011:  ret
  } // end of method UnsafeCode::PointerArithmeticLong3s

  .method public hidebysig instance int32 
          PointerSubtraction(int64* p,
                             int64* q) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  sub
    IL_0004:  ldc.i4.8
    IL_0005:  div
    IL_0006:  conv.i8
    IL_0007:  conv.i4
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method UnsafeCode::PointerSubtraction

  .method public hidebysig instance int64 
          PointerSubtractionLong(int64* p,
                                 int64* q) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  2
    .locals init (int64 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  sub
    IL_0004:  ldc.i4.8
    IL_0005:  div
    IL_0006:  conv.i8
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method UnsafeCode::PointerSubtractionLong

  .method public hidebysig instance int32 
          PointerSubtraction2(int64* p,
                              int16* q) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  sub
    IL_0004:  ldc.i4.1
    IL_0005:  div
    IL_0006:  conv.i8
    IL_0007:  conv.i4
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method UnsafeCode::PointerSubtraction2

  .method public hidebysig instance int32 
          PointerSubtraction3(void* p,
                              void* q) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  sub
    IL_0004:  ldc.i4.1
    IL_0005:  div
    IL_0006:  conv.i8
    IL_0007:  conv.i4
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method UnsafeCode::PointerSubtraction3

  .method public hidebysig instance int64 
          PointerSubtraction4(int8* p,
                              int8* q) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  2
    .locals init (int64 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  sub
    IL_0004:  ldc.i4.1
    IL_0005:  div
    IL_0006:  conv.i8
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method UnsafeCode::PointerSubtraction4

  .method public hidebysig instance int64 
          PointerSubtraction5(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* p,
                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* q) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  2
    .locals init (int64 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  sub
    IL_0004:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct
    IL_000a:  div
    IL_000b:  conv.i8
    IL_000c:  stloc.0
    IL_000d:  br.s       IL_000f

    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method UnsafeCode::PointerSubtraction5

  .method public hidebysig instance float64 
          FixedMemberAccess(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers* m,
                            int32 i) cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  4
    .locals init (float64 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer' ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers::Integers
    IL_0007:  ldflda     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer'::FixedElementField
    IL_000c:  conv.u
    IL_000d:  ldarg.2
    IL_000e:  conv.i
    IL_000f:  ldc.i4.4
    IL_0010:  mul
    IL_0011:  add
    IL_0012:  ldind.i4
    IL_0013:  conv.r8
    IL_0014:  ldarg.1
    IL_0015:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Doubles>e__FixedBuffer' ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers::Doubles
    IL_001a:  ldflda     float64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Doubles>e__FixedBuffer'::FixedElementField
    IL_001f:  conv.u
    IL_0020:  ldarg.2
    IL_0021:  conv.i
    IL_0022:  ldc.i4.8
    IL_0023:  mul
    IL_0024:  add
    IL_0025:  ldind.r8
    IL_0026:  add
    IL_0027:  stloc.0
    IL_0028:  br.s       IL_002a

    IL_002a:  ldloc.0
    IL_002b:  ret
  } // end of method UnsafeCode::FixedMemberAccess

  .method public hidebysig instance float64* 
          FixedMemberBasePointer(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers* m) cil managed
  {
    // Code size       18 (0x12)
    .maxstack  1
    .locals init (float64* V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Doubles>e__FixedBuffer' ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers::Doubles
    IL_0007:  ldflda     float64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Doubles>e__FixedBuffer'::FixedElementField
    IL_000c:  conv.u
    IL_000d:  stloc.0
    IL_000e:  br.s       IL_0010

    IL_0010:  ldloc.0
    IL_0011:  ret
  } // end of method UnsafeCode::FixedMemberBasePointer

  .method public hidebysig instance void 
          UseFixedMemberAsPointer(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers* m) cil managed
  {
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.1
    IL_0003:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer' ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers::Integers
    IL_0008:  ldflda     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer'::FixedElementField
    IL_000d:  conv.u
    IL_000e:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UsePointer(int32*)
    IL_0013:  pop
    IL_0014:  ret
  } // end of method UnsafeCode::UseFixedMemberAsPointer

  .method public hidebysig instance void 
          UseFixedMemberAsReference(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers* m) cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.1
    IL_0003:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer' ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers::Integers
    IL_0008:  ldflda     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer'::FixedElementField
    IL_000d:  conv.u
    IL_000e:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseReference(int32&)
    IL_0013:  nop
    IL_0014:  ldarg.0
    IL_0015:  ldarg.1
    IL_0016:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer' ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers::Integers
    IL_001b:  ldflda     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer'::FixedElementField
    IL_0020:  conv.u
    IL_0021:  ldc.i4.4
    IL_0022:  add
    IL_0023:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UseReference(int32&)
    IL_0028:  nop
    IL_0029:  ret
  } // end of method UnsafeCode::UseFixedMemberAsReference

  .method public hidebysig instance void 
          PinFixedMember(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers& m) cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  2
    .locals init (int32& pinned V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer' ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers::Integers
    IL_0007:  ldflda     int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/StructWithFixedSizeMembers/'<Integers>e__FixedBuffer'::FixedElementField
    IL_000c:  stloc.0
    IL_000d:  nop
    IL_000e:  ldarg.0
    IL_000f:  ldloc.0
    IL_0010:  conv.i
    IL_0011:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UsePointer(int32*)
    IL_0016:  pop
    IL_0017:  nop
    IL_0018:  ldc.i4.0
    IL_0019:  conv.u
    IL_001a:  stloc.0
    IL_001b:  ret
  } // end of method UnsafeCode::PinFixedMember

  .method private hidebysig instance void 
          UseReference(int32& i) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method UnsafeCode::UseReference

  .method public hidebysig instance string 
          UsePointer(int32* ptr) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  call       instance string [mscorlib]System.Int32::ToString()
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method UnsafeCode::UsePointer

  .method public hidebysig instance string 
          UsePointer(float64* ptr) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  call       instance string [mscorlib]System.Double::ToString()
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method UnsafeCode::UsePointer

  .method public hidebysig instance string 
          StackAlloc(int32 count) cil managed
  {
    // Code size       65 (0x41)
    .maxstack  3
    .locals init (char* V_0,
             char* V_1,
             int32 V_2,
             bool V_3,
             string V_4)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  conv.u
    IL_0003:  ldc.i4.2
    IL_0004:  mul.ovf.un
    IL_0005:  localloc
    IL_0007:  stloc.0
    IL_0008:  ldc.i4.s   100
    IL_000a:  conv.u
    IL_000b:  ldc.i4.2
    IL_000c:  mul.ovf.un
    IL_000d:  localloc
    IL_000f:  stloc.1
    IL_0010:  ldc.i4.0
    IL_0011:  stloc.2
    IL_0012:  br.s       IL_002b

    IL_0014:  nop
    IL_0015:  ldloc.0
    IL_0016:  ldloc.2
    IL_0017:  conv.i
    IL_0018:  ldc.i4.2
    IL_0019:  mul
    IL_001a:  add
    IL_001b:  ldloc.2
    IL_001c:  conv.u2
    IL_001d:  stind.i2
    IL_001e:  ldloc.1
    IL_001f:  ldloc.2
    IL_0020:  conv.i
    IL_0021:  ldc.i4.2
    IL_0022:  mul
    IL_0023:  add
    IL_0024:  ldc.i4.0
    IL_0025:  stind.i2
    IL_0026:  nop
    IL_0027:  ldloc.2
    IL_0028:  ldc.i4.1
    IL_0029:  add
    IL_002a:  stloc.2
    IL_002b:  ldloc.2
    IL_002c:  ldarg.1
    IL_002d:  clt
    IL_002f:  stloc.3
    IL_0030:  ldloc.3
    IL_0031:  brtrue.s   IL_0014

    IL_0033:  ldarg.0
    IL_0034:  ldloc.0
    IL_0035:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UsePointer(float64*)
    IL_003a:  stloc.s    V_4
    IL_003c:  br.s       IL_003e

    IL_003e:  ldloc.s    V_4
    IL_0040:  ret
  } // end of method UnsafeCode::StackAlloc

  .method public hidebysig instance string 
          StackAllocStruct(int32 count) cil managed
  {
    // Code size       110 (0x6e)
    .maxstack  3
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct* V_1,
             int32 V_2,
             bool V_3,
             string V_4)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.2
    IL_0003:  mul.ovf
    IL_0004:  conv.u
    IL_0005:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct
    IL_000b:  mul.ovf.un
    IL_000c:  localloc
    IL_000e:  stloc.0
    IL_000f:  ldc.i4.s   10
    IL_0011:  conv.u
    IL_0012:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct
    IL_0018:  mul.ovf.un
    IL_0019:  localloc
    IL_001b:  stloc.1
    IL_001c:  ldloc.0
    IL_001d:  ldarg.1
    IL_001e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct::X
    IL_0023:  ldloc.0
    IL_0024:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct
    IL_002a:  add
    IL_002b:  ldloc.0
    IL_002c:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct::X
    IL_0031:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct::X
    IL_0036:  ldc.i4.2
    IL_0037:  stloc.2
    IL_0038:  br.s       IL_0051

    IL_003a:  nop
    IL_003b:  ldloc.0
    IL_003c:  ldloc.2
    IL_003d:  conv.i
    IL_003e:  sizeof     ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct
    IL_0044:  mul
    IL_0045:  add
    IL_0046:  ldarg.1
    IL_0047:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct::X
    IL_004c:  nop
    IL_004d:  ldloc.2
    IL_004e:  ldc.i4.1
    IL_004f:  add
    IL_0050:  stloc.2
    IL_0051:  ldloc.2
    IL_0052:  ldc.i4.s   10
    IL_0054:  clt
    IL_0056:  stloc.3
    IL_0057:  ldloc.3
    IL_0058:  brtrue.s   IL_003a

    IL_005a:  ldarg.0
    IL_005b:  ldloc.0
    IL_005c:  ldflda     float64 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/SimpleStruct::Y
    IL_0061:  conv.u
    IL_0062:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::UsePointer(float64*)
    IL_0067:  stloc.s    V_4
    IL_0069:  br.s       IL_006b

    IL_006b:  ldloc.s    V_4
    IL_006d:  ret
  } // end of method UnsafeCode::StackAllocStruct

  .method family hidebysig virtual instance void 
          Finalize() cil managed
  {
    .override [mscorlib]System.Object::Finalize
    // Code size       26 (0x1a)
    .maxstack  2
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldarg.0
      IL_0003:  ldarg.0
      IL_0004:  call       instance int32* ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::get_NullPointer()
      IL_0009:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::PassPointerAsRefParameter(int32*)
      IL_000e:  nop
      IL_000f:  leave.s    IL_0019

    }  // end .try
    finally
    {
      IL_0011:  ldarg.0
      IL_0012:  call       instance void [mscorlib]System.Object::Finalize()
      IL_0017:  nop
      IL_0018:  endfinally
    }  // end handler
    IL_0019:  ret
  } // end of method UnsafeCode::Finalize

  .method private hidebysig instance void 
          Issue990() cil managed
  {
    // Code size       38 (0x26)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/Data V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/Data* V_1)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_0
    IL_0003:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/Data
    IL_0009:  ldloca.s   V_0
    IL_000b:  conv.u
    IL_000c:  stloc.1
    IL_000d:  ldarg.0
    IL_000e:  ldloc.1
    IL_000f:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/Vector ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/Data::Position
    IL_0014:  constrained. ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode/Vector
    IL_001a:  callvirt   instance int32 [mscorlib]System.Object::GetHashCode()
    IL_001f:  call       instance float32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::ConvertIntToFloat(int32)
    IL_0024:  pop
    IL_0025:  ret
  } // end of method UnsafeCode::Issue990

  .property instance int32* NullPointer()
  {
    .get instance int32* ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::get_NullPointer()
  } // end of property UnsafeCode::NullPointer
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
