
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly p1cal2uj
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module p1cal2uj.dll
// MVID: {BD577496-1116-4E18-AF25-DE99056CECA8}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03220000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode
       extends [mscorlib]System.Object
{
  .class sequential ansi sealed nested public beforefieldinit SimpleStruct
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
    // Code size       26 (0x1a)
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
    IL_0010:  conv.i
    IL_0011:  add
    IL_0012:  ldc.i4.3
    IL_0013:  stind.i1
    IL_0014:  ldloc.0
    IL_0015:  stloc.1
    IL_0016:  br.s       IL_0018

    IL_0018:  ldloc.1
    IL_0019:  ret
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
    // Code size       45 (0x2d)
    .maxstack  2
    .locals init (char* V_0,
             char* V_1,
             string pinned V_2,
             bool V_3)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.2
    IL_0003:  ldloc.2
    IL_0004:  conv.i
    IL_0005:  dup
    IL_0006:  brfalse.s  IL_000e

    IL_0008:  call       int32 [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::get_OffsetToStringData()
    IL_000d:  add
    IL_000e:  stloc.0
    IL_000f:  nop
    IL_0010:  ldloc.0
    IL_0011:  stloc.1
    IL_0012:  br.s       IL_001f

    IL_0014:  nop
    IL_0015:  ldloc.1
    IL_0016:  ldc.i4.s   65
    IL_0018:  stind.i2
    IL_0019:  ldloc.1
    IL_001a:  ldc.i4.2
    IL_001b:  conv.i
    IL_001c:  add
    IL_001d:  stloc.1
    IL_001e:  nop
    IL_001f:  ldloc.1
    IL_0020:  ldind.u2
    IL_0021:  ldc.i4.s   97
    IL_0023:  ceq
    IL_0025:  stloc.3
    IL_0026:  ldloc.3
    IL_0027:  brtrue.s   IL_0014

    IL_0029:  nop
    IL_002a:  ldnull
    IL_002b:  stloc.2
    IL_002c:  ret
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
    // Code size       10 (0xa)
    .maxstack  2
    .locals init (int32* V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.8
    IL_0003:  conv.i
    IL_0004:  add
    IL_0005:  stloc.0
    IL_0006:  br.s       IL_0008

    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method UnsafeCode::PointerArithmetic

  .method public hidebysig instance int64* 
          PointerArithmetic2(int64* p) cil managed
  {
    // Code size       11 (0xb)
    .maxstack  2
    .locals init (int64* V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   24
    IL_0003:  conv.i
    IL_0004:  ldarg.1
    IL_0005:  add
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method UnsafeCode::PointerArithmetic2

  .method public hidebysig instance int64* 
          PointerArithmetic3(int64* p) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  2
    .locals init (int64* V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.3
    IL_0003:  conv.i
    IL_0004:  add
    IL_0005:  stloc.0
    IL_0006:  br.s       IL_0008

    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method UnsafeCode::PointerArithmetic3

  .method public hidebysig instance int64* 
          PointerArithmetic4(void* p) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  2
    .locals init (int64* V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.3
    IL_0003:  conv.i
    IL_0004:  add
    IL_0005:  stloc.0
    IL_0006:  br.s       IL_0008

    IL_0008:  ldloc.0
    IL_0009:  ret
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

  .method family hidebysig virtual instance void 
          Finalize() cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  2
    .try
    {
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldarg.0
      IL_0003:  call       instance int32* ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::get_NullPointer()
      IL_0008:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.UnsafeCode::PassPointerAsRefParameter(int32*)
      IL_000d:  nop
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
    IL_0019:  nop
    IL_001a:  ret
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
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\UnsafeCode.res
