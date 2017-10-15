
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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module UnsafeCode.dll
// MVID: {858D67FE-2C39-4F0B-B696-FA10954BDC00}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x01270000


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
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  conv.u
    IL_0002:  ret
  } // end of method UnsafeCode::get_NullPointer

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
    // Code size       20 (0x14)
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
    IL_000f:  add
    IL_0010:  ldc.i4.3
    IL_0011:  stind.i1
    IL_0012:  ldloc.0
    IL_0013:  ret
  } // end of method UnsafeCode::PointerCasts

  .method public hidebysig instance void 
          PassRefParameterAsPointer(int32& p) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  2
    .locals init (int32& pinned V_0)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldarg.0
    IL_0003:  ldloc.0
    IL_0004:  conv.i
    IL_0005:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::PassPointerAsRefParameter(int32*)
    IL_000a:  ldc.i4.0
    IL_000b:  conv.u
    IL_000c:  stloc.0
    IL_000d:  ret
  } // end of method UnsafeCode::PassRefParameterAsPointer

  .method public hidebysig instance void 
          PassPointerAsRefParameter(int32* p) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::PassRefParameterAsPointer(int32&)
    IL_0007:  ret
  } // end of method UnsafeCode::PassPointerAsRefParameter

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
    // Code size       37 (0x25)
    .maxstack  2
    .locals init (char* V_0,
             string pinned V_1,
             char* V_2)
    IL_0000:  ldarg.1
    IL_0001:  stloc.1
    IL_0002:  ldloc.1
    IL_0003:  conv.i
    IL_0004:  stloc.0
    IL_0005:  ldloc.0
    IL_0006:  brfalse.s  IL_0010

    IL_0008:  ldloc.0
    IL_0009:  call       int32 [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::get_OffsetToStringData()
    IL_000e:  add
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  stloc.2
    IL_0012:  br.s       IL_001c

    IL_0014:  ldloc.2
    IL_0015:  ldc.i4.s   65
    IL_0017:  stind.i2
    IL_0018:  ldloc.2
    IL_0019:  ldc.i4.2
    IL_001a:  add
    IL_001b:  stloc.2
    IL_001c:  ldloc.2
    IL_001d:  ldind.u2
    IL_001e:  ldc.i4.s   97
    IL_0020:  beq.s      IL_0014

    IL_0022:  ldnull
    IL_0023:  stloc.1
    IL_0024:  ret
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
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.2
    IL_0002:  conv.i
    IL_0003:  ldc.i4.4
    IL_0004:  mul
    IL_0005:  add
    IL_0006:  ret
  } // end of method UnsafeCode::PointerArithmetic

  .method public hidebysig instance int64* 
          PointerArithmetic2(int64* p,
                             int32 y,
                             int32 x) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldc.i4.3
    IL_0001:  conv.i
    IL_0002:  ldc.i4.8
    IL_0003:  mul
    IL_0004:  ldarg.1
    IL_0005:  add
    IL_0006:  ret
  } // end of method UnsafeCode::PointerArithmetic2

  .method public hidebysig instance int64* 
          PointerArithmetic3(int64* p) cil managed
  {
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.3
    IL_0002:  add
    IL_0003:  ret
  } // end of method UnsafeCode::PointerArithmetic3

  .method public hidebysig instance int64* 
          PointerArithmetic4(void* p) cil managed
  {
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.3
    IL_0002:  add
    IL_0003:  ret
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

  .method family hidebysig virtual instance void 
          Finalize() cil managed
  {
    .override [mscorlib]System.Object::Finalize
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

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method UnsafeCode::.ctor

  .property instance int32* NullPointer()
  {
    .get instance int32* ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::get_NullPointer()
  } // end of property UnsafeCode::NullPointer
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
