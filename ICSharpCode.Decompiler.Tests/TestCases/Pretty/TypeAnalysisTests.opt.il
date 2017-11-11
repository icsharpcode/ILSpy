
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly '31uigzt1'
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module '31uigzt1.dll'
// MVID: {77B7D9D4-A6DF-4CC8-B1E3-7B3B553633FF}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x01360000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.TypeAnalysisTests
       extends [mscorlib]System.Object
{
  .field private uint8[] byteArray
  .method public hidebysig instance uint8 
          SubtractFrom256(uint8 b) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldc.i4     0x100
    IL_0005:  ldarg.1
    IL_0006:  sub
    IL_0007:  conv.u1
    IL_0008:  ret
  } // end of method TypeAnalysisTests::SubtractFrom256

  .method public hidebysig instance int32 
          LShiftInteger(int32 num1,
                        int32 num2) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldc.i4.s   31
    IL_0004:  and
    IL_0005:  shl
    IL_0006:  ret
  } // end of method TypeAnalysisTests::LShiftInteger

  .method public hidebysig instance uint32 
          LShiftUnsignedInteger(uint32 num1,
                                uint32 num2) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldc.i4.s   31
    IL_0004:  and
    IL_0005:  shl
    IL_0006:  ret
  } // end of method TypeAnalysisTests::LShiftUnsignedInteger

  .method public hidebysig instance int64 
          LShiftLong(int64 num1,
                     int64 num2) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  conv.i4
    IL_0003:  ldc.i4.s   63
    IL_0005:  and
    IL_0006:  shl
    IL_0007:  ret
  } // end of method TypeAnalysisTests::LShiftLong

  .method public hidebysig instance uint64 
          LShiftUnsignedLong(uint64 num1,
                             uint64 num2) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  conv.i4
    IL_0003:  ldc.i4.s   63
    IL_0005:  and
    IL_0006:  shl
    IL_0007:  ret
  } // end of method TypeAnalysisTests::LShiftUnsignedLong

  .method public hidebysig instance int32 
          RShiftInteger(int32 num1,
                        int32 num2) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldc.i4.s   31
    IL_0004:  and
    IL_0005:  shr
    IL_0006:  ret
  } // end of method TypeAnalysisTests::RShiftInteger

  .method public hidebysig instance uint32 
          RShiftUnsignedInteger(uint32 num1,
                                int32 num2) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ldc.i4.s   31
    IL_0004:  and
    IL_0005:  shr.un
    IL_0006:  ret
  } // end of method TypeAnalysisTests::RShiftUnsignedInteger

  .method public hidebysig instance int64 
          RShiftLong(int64 num1,
                     int64 num2) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  conv.i4
    IL_0003:  ldc.i4.s   63
    IL_0005:  and
    IL_0006:  shr
    IL_0007:  ret
  } // end of method TypeAnalysisTests::RShiftLong

  .method public hidebysig instance uint64 
          RShiftUnsignedLong(uint64 num1,
                             uint64 num2) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  conv.i4
    IL_0003:  ldc.i4.s   63
    IL_0005:  and
    IL_0006:  shr.un
    IL_0007:  ret
  } // end of method TypeAnalysisTests::RShiftUnsignedLong

  .method public hidebysig instance int32 
          ShiftByte(uint8 num) cil managed
  {
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.8
    IL_0002:  shl
    IL_0003:  ret
  } // end of method TypeAnalysisTests::ShiftByte

  .method public hidebysig instance int32 
          RShiftByte(uint8 num) cil managed
  {
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.8
    IL_0002:  shr
    IL_0003:  ret
  } // end of method TypeAnalysisTests::RShiftByte

  .method public hidebysig instance uint32 
          RShiftByteWithZeroExtension(uint8 num) cil managed
  {
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.8
    IL_0002:  shr.un
    IL_0003:  ret
  } // end of method TypeAnalysisTests::RShiftByteWithZeroExtension

  .method public hidebysig instance int32 
          RShiftByteAsSByte(uint8 num) cil managed
  {
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  conv.i1
    IL_0002:  ldc.i4.8
    IL_0003:  shr
    IL_0004:  ret
  } // end of method TypeAnalysisTests::RShiftByteAsSByte

  .method public hidebysig instance int32 
          RShiftSByte(int8 num) cil managed
  {
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.8
    IL_0002:  shr
    IL_0003:  ret
  } // end of method TypeAnalysisTests::RShiftSByte

  .method public hidebysig instance uint32 
          RShiftSByteWithZeroExtension(int8 num) cil managed
  {
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.8
    IL_0002:  shr.un
    IL_0003:  ret
  } // end of method TypeAnalysisTests::RShiftSByteWithZeroExtension

  .method public hidebysig instance int32 
          RShiftSByteAsByte(int8 num) cil managed
  {
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  conv.u1
    IL_0002:  ldc.i4.8
    IL_0003:  shr
    IL_0004:  ret
  } // end of method TypeAnalysisTests::RShiftSByteAsByte

  .method public hidebysig instance int32 
          GetHashCode(int64 num) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  conv.i4
    IL_0002:  ldarg.1
    IL_0003:  ldc.i4.s   32
    IL_0005:  shr
    IL_0006:  conv.i4
    IL_0007:  xor
    IL_0008:  ret
  } // end of method TypeAnalysisTests::GetHashCode

  .method public hidebysig instance void 
          TernaryOp(class [mscorlib]System.Random a,
                    class [mscorlib]System.Random b,
                    bool c) cil managed
  {
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldarg.3
    IL_0001:  brtrue.s   IL_0006

    IL_0003:  ldarg.2
    IL_0004:  br.s       IL_0007

    IL_0006:  ldarg.1
    IL_0007:  brtrue.s   IL_000e

    IL_0009:  call       void [mscorlib]System.Console::WriteLine()
    IL_000e:  ret
  } // end of method TypeAnalysisTests::TernaryOp

  .method public hidebysig instance void 
          OperatorIs(object o) cil managed
  {
    // Code size       32 (0x20)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  isinst     [mscorlib]System.Random
    IL_0006:  ldnull
    IL_0007:  cgt.un
    IL_0009:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_000e:  ldarg.1
    IL_000f:  isinst     [mscorlib]System.Random
    IL_0014:  ldnull
    IL_0015:  cgt.un
    IL_0017:  ldc.i4.0
    IL_0018:  ceq
    IL_001a:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_001f:  ret
  } // end of method TypeAnalysisTests::OperatorIs

  .method public hidebysig instance uint8[] 
          CreateArrayWithInt(int32 length) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  newarr     [mscorlib]System.Byte
    IL_0006:  ret
  } // end of method TypeAnalysisTests::CreateArrayWithInt

  .method public hidebysig instance uint8[] 
          CreateArrayWithLong(int64 length) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  conv.ovf.i
    IL_0002:  newarr     [mscorlib]System.Byte
    IL_0007:  ret
  } // end of method TypeAnalysisTests::CreateArrayWithLong

  .method public hidebysig instance uint8[] 
          CreateArrayWithUInt(uint32 length) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  conv.u
    IL_0002:  newarr     [mscorlib]System.Byte
    IL_0007:  ret
  } // end of method TypeAnalysisTests::CreateArrayWithUInt

  .method public hidebysig instance uint8[] 
          CreateArrayWithULong(uint64 length) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  conv.ovf.i.un
    IL_0002:  newarr     [mscorlib]System.Byte
    IL_0007:  ret
  } // end of method TypeAnalysisTests::CreateArrayWithULong

  .method public hidebysig instance uint8[] 
          CreateArrayWithShort(int16 length) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  newarr     [mscorlib]System.Byte
    IL_0006:  ret
  } // end of method TypeAnalysisTests::CreateArrayWithShort

  .method public hidebysig instance uint8[] 
          CreateArrayWithUShort(uint16 length) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  newarr     [mscorlib]System.Byte
    IL_0006:  ret
  } // end of method TypeAnalysisTests::CreateArrayWithUShort

  .method public hidebysig instance uint8 
          UseArrayWithInt(int32 i) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      uint8[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.TypeAnalysisTests::byteArray
    IL_0006:  ldarg.1
    IL_0007:  ldelem.u1
    IL_0008:  ret
  } // end of method TypeAnalysisTests::UseArrayWithInt

  .method public hidebysig instance uint8 
          UseArrayWithUInt(uint32 i) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      uint8[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.TypeAnalysisTests::byteArray
    IL_0006:  ldarg.1
    IL_0007:  conv.u
    IL_0008:  ldelem.u1
    IL_0009:  ret
  } // end of method TypeAnalysisTests::UseArrayWithUInt

  .method public hidebysig instance uint8 
          UseArrayWithLong(int64 i) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      uint8[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.TypeAnalysisTests::byteArray
    IL_0006:  ldarg.1
    IL_0007:  conv.ovf.i
    IL_0008:  ldelem.u1
    IL_0009:  ret
  } // end of method TypeAnalysisTests::UseArrayWithLong

  .method public hidebysig instance uint8 
          UseArrayWithULong(uint64 i) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      uint8[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.TypeAnalysisTests::byteArray
    IL_0006:  ldarg.1
    IL_0007:  conv.ovf.i.un
    IL_0008:  ldelem.u1
    IL_0009:  ret
  } // end of method TypeAnalysisTests::UseArrayWithULong

  .method public hidebysig instance uint8 
          UseArrayWithShort(int16 i) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      uint8[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.TypeAnalysisTests::byteArray
    IL_0006:  ldarg.1
    IL_0007:  ldelem.u1
    IL_0008:  ret
  } // end of method TypeAnalysisTests::UseArrayWithShort

  .method public hidebysig instance uint8 
          UseArrayWithUShort(uint16 i) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      uint8[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.TypeAnalysisTests::byteArray
    IL_0006:  ldarg.1
    IL_0007:  ldelem.u1
    IL_0008:  ret
  } // end of method TypeAnalysisTests::UseArrayWithUShort

  .method public hidebysig instance uint8 
          UseArrayWithCastToUShort(int32 i) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      uint8[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.TypeAnalysisTests::byteArray
    IL_0006:  ldarg.1
    IL_0007:  conv.u2
    IL_0008:  ldelem.u1
    IL_0009:  ret
  } // end of method TypeAnalysisTests::UseArrayWithCastToUShort

  .method public hidebysig instance valuetype [mscorlib]System.StringComparison 
          EnumDiffNumber(valuetype [mscorlib]System.StringComparison data) cil managed
  {
    // Code size       4 (0x4)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.1
    IL_0002:  sub
    IL_0003:  ret
  } // end of method TypeAnalysisTests::EnumDiffNumber

  .method public hidebysig instance int32 
          EnumDiff(valuetype [mscorlib]System.StringComparison a,
                   valuetype [mscorlib]System.StringComparison b) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  sub
    IL_0003:  call       int32 [mscorlib]System.Math::Abs(int32)
    IL_0008:  ret
  } // end of method TypeAnalysisTests::EnumDiff

  .method public hidebysig instance bool 
          CompareDelegatesByValue(class [mscorlib]System.Action a,
                                  class [mscorlib]System.Action b) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  call       bool [mscorlib]System.Delegate::op_Equality(class [mscorlib]System.Delegate,
                                                                     class [mscorlib]System.Delegate)
    IL_0007:  ret
  } // end of method TypeAnalysisTests::CompareDelegatesByValue

  .method public hidebysig instance bool 
          CompareDelegatesByReference(class [mscorlib]System.Action a,
                                      class [mscorlib]System.Action b) cil managed
  {
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ceq
    IL_0004:  ret
  } // end of method TypeAnalysisTests::CompareDelegatesByReference

  .method public hidebysig instance bool 
          CompareDelegateWithNull(class [mscorlib]System.Action a) cil managed
  {
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldnull
    IL_0002:  ceq
    IL_0004:  ret
  } // end of method TypeAnalysisTests::CompareDelegateWithNull

  .method public hidebysig instance bool 
          CompareStringsByValue(string a,
                                string b) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0007:  ret
  } // end of method TypeAnalysisTests::CompareStringsByValue

  .method public hidebysig instance bool 
          CompareStringsByReference(string a,
                                    string b) cil managed
  {
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  ceq
    IL_0004:  ret
  } // end of method TypeAnalysisTests::CompareStringsByReference

  .method public hidebysig instance bool 
          CompareStringWithNull(string a) cil managed
  {
    // Code size       5 (0x5)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldnull
    IL_0002:  ceq
    IL_0004:  ret
  } // end of method TypeAnalysisTests::CompareStringWithNull

  .method public hidebysig instance valuetype [mscorlib]System.Decimal 
          ImplicitConversionToDecimal(uint8 v) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(uint8)
    IL_0006:  ret
  } // end of method TypeAnalysisTests::ImplicitConversionToDecimal

  .method public hidebysig instance valuetype [mscorlib]System.Decimal 
          ImplicitConversionToDecimal(uint64 v) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(uint64)
    IL_0006:  ret
  } // end of method TypeAnalysisTests::ImplicitConversionToDecimal

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method TypeAnalysisTests::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.TypeAnalysisTests


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\TypeAnalysisTests.opt.res
