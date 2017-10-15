
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
// MVID: {BCDFA7E7-0E94-44F0-8A8C-85049FECA738}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x01330000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode
       extends [mscorlib]System.Object
{
  .class sequential ansi sealed nested private beforefieldinit SimpleStruct
         extends [mscorlib]System.ValueType
  {
    .field public int32 X
    .field public float64 Y
  } // end of class SimpleStruct

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
    IL_0007:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::PassPointerAsRefParameter(int32*)
    IL_000c:  nop
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
    IL_0003:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::PassRefParameterAsPointer(int32&)
    IL_0008:  nop
    IL_0009:  ret
  } // end of method UnsafeCode::PassPointerAsRefParameter

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
    IL_001b:  ldloc.2
    IL_001c:  ldc.i4.2
    IL_001d:  add
    IL_001e:  stloc.2
    IL_001f:  nop
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
          PointerArithmetic2(int64* p,
                             int32 y,
                             int32 x) cil managed
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

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method UnsafeCode::.ctor

  .property instance int32* NullPointer()
  {
    .get instance int32* ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::get_NullPointer()
  } // end of property UnsafeCode::NullPointer
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
