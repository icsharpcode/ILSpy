
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly fpugcpej
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module fpugcpej.dll
// MVID: {F42443CE-6C18-4149-B04E-77437B350B2C}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x01650000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.TypeAnalysisTests
       extends [mscorlib]System.Object
{
  .field private uint8[] byteArray
  .method public hidebysig instance uint8 
          SubtractFrom256(uint8 b) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  2
    .locals init (uint8 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4     0x100
    IL_0006:  ldarg.1
    IL_0007:  sub
    IL_0008:  conv.u1
    IL_0009:  stloc.0
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.0
    IL_000d:  ret
  } // end of method TypeAnalysisTests::SubtractFrom256

  .method public hidebysig instance int32 
          LShiftInteger(int32 num1,
                        int32 num2) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldc.i4.s   31
    IL_0005:  and
    IL_0006:  shl
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method TypeAnalysisTests::LShiftInteger

  .method public hidebysig instance uint32 
          LShiftUnsignedInteger(uint32 num1,
                                uint32 num2) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  3
    .locals init (uint32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldc.i4.s   31
    IL_0005:  and
    IL_0006:  shl
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method TypeAnalysisTests::LShiftUnsignedInteger

  .method public hidebysig instance int64 
          LShiftLong(int64 num1,
                     int64 num2) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  3
    .locals init (int64 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  conv.i4
    IL_0004:  ldc.i4.s   63
    IL_0006:  and
    IL_0007:  shl
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method TypeAnalysisTests::LShiftLong

  .method public hidebysig instance uint64 
          LShiftUnsignedLong(uint64 num1,
                             uint64 num2) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  3
    .locals init (uint64 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  conv.i4
    IL_0004:  ldc.i4.s   63
    IL_0006:  and
    IL_0007:  shl
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method TypeAnalysisTests::LShiftUnsignedLong

  .method public hidebysig instance int32 
          RShiftInteger(int32 num1,
                        int32 num2) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldc.i4.s   31
    IL_0005:  and
    IL_0006:  shr
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method TypeAnalysisTests::RShiftInteger

  .method public hidebysig instance uint32 
          RShiftUnsignedInteger(uint32 num1,
                                int32 num2) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  3
    .locals init (uint32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ldc.i4.s   31
    IL_0005:  and
    IL_0006:  shr.un
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method TypeAnalysisTests::RShiftUnsignedInteger

  .method public hidebysig instance int64 
          RShiftLong(int64 num1,
                     int64 num2) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  3
    .locals init (int64 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  conv.i4
    IL_0004:  ldc.i4.s   63
    IL_0006:  and
    IL_0007:  shr
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method TypeAnalysisTests::RShiftLong

  .method public hidebysig instance uint64 
          RShiftUnsignedLong(uint64 num1,
                             uint64 num2) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  3
    .locals init (uint64 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  conv.i4
    IL_0004:  ldc.i4.s   63
    IL_0006:  and
    IL_0007:  shr.un
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method TypeAnalysisTests::RShiftUnsignedLong

  .method public hidebysig instance int32 
          ShiftByte(uint8 num) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.8
    IL_0003:  shl
    IL_0004:  stloc.0
    IL_0005:  br.s       IL_0007

    IL_0007:  ldloc.0
    IL_0008:  ret
  } // end of method TypeAnalysisTests::ShiftByte

  .method public hidebysig instance int32 
          RShiftByte(uint8 num) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.8
    IL_0003:  shr
    IL_0004:  stloc.0
    IL_0005:  br.s       IL_0007

    IL_0007:  ldloc.0
    IL_0008:  ret
  } // end of method TypeAnalysisTests::RShiftByte

  .method public hidebysig instance uint32 
          RShiftByteWithZeroExtension(uint8 num) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  2
    .locals init (uint32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.8
    IL_0003:  shr.un
    IL_0004:  stloc.0
    IL_0005:  br.s       IL_0007

    IL_0007:  ldloc.0
    IL_0008:  ret
  } // end of method TypeAnalysisTests::RShiftByteWithZeroExtension

  .method public hidebysig instance int32 
          RShiftByteAsSByte(uint8 num) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  conv.i1
    IL_0003:  ldc.i4.8
    IL_0004:  shr
    IL_0005:  stloc.0
    IL_0006:  br.s       IL_0008

    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method TypeAnalysisTests::RShiftByteAsSByte

  .method public hidebysig instance int32 
          RShiftSByte(int8 num) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.8
    IL_0003:  shr
    IL_0004:  stloc.0
    IL_0005:  br.s       IL_0007

    IL_0007:  ldloc.0
    IL_0008:  ret
  } // end of method TypeAnalysisTests::RShiftSByte

  .method public hidebysig instance uint32 
          RShiftSByteWithZeroExtension(int8 num) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  2
    .locals init (uint32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.8
    IL_0003:  shr.un
    IL_0004:  stloc.0
    IL_0005:  br.s       IL_0007

    IL_0007:  ldloc.0
    IL_0008:  ret
  } // end of method TypeAnalysisTests::RShiftSByteWithZeroExtension

  .method public hidebysig instance int32 
          RShiftSByteAsByte(int8 num) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  conv.u1
    IL_0003:  ldc.i4.8
    IL_0004:  shr
    IL_0005:  stloc.0
    IL_0006:  br.s       IL_0008

    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method TypeAnalysisTests::RShiftSByteAsByte

  .method public hidebysig instance int32 
          GetHashCode(int64 num) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  conv.i4
    IL_0003:  ldarg.1
    IL_0004:  ldc.i4.s   32
    IL_0006:  shr
    IL_0007:  conv.i4
    IL_0008:  xor
    IL_0009:  stloc.0
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.0
    IL_000d:  ret
  } // end of method TypeAnalysisTests::GetHashCode

  .method public hidebysig instance void 
          TernaryOp(class [mscorlib]System.Random a,
                    class [mscorlib]System.Random b,
                    bool c) cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.3
    IL_0002:  brtrue.s   IL_0007

    IL_0004:  ldarg.2
    IL_0005:  br.s       IL_0008

    IL_0007:  ldarg.1
    IL_0008:  nop
    IL_0009:  ldnull
    IL_000a:  ceq
    IL_000c:  ldc.i4.0
    IL_000d:  ceq
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  brtrue.s   IL_001b

    IL_0013:  nop
    IL_0014:  call       void [mscorlib]System.Console::WriteLine()
    IL_0019:  nop
    IL_001a:  nop
    IL_001b:  ret
  } // end of method TypeAnalysisTests::TernaryOp

  .method public hidebysig instance void 
          OperatorIs(object o) cil managed
  {
    // Code size       35 (0x23)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  isinst     [mscorlib]System.Random
    IL_0007:  ldnull
    IL_0008:  cgt.un
    IL_000a:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_000f:  nop
    IL_0010:  ldarg.1
    IL_0011:  isinst     [mscorlib]System.Random
    IL_0016:  ldnull
    IL_0017:  cgt.un
    IL_0019:  ldc.i4.0
    IL_001a:  ceq
    IL_001c:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0021:  nop
    IL_0022:  ret
  } // end of method TypeAnalysisTests::OperatorIs

  .method public hidebysig instance uint8[] 
          CreateArrayWithInt(int32 length) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (uint8[] V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  newarr     [mscorlib]System.Byte
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method TypeAnalysisTests::CreateArrayWithInt

  .method public hidebysig instance uint8[] 
          CreateArrayWithLong(int64 length) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  1
    .locals init (uint8[] V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  conv.ovf.i
    IL_0003:  newarr     [mscorlib]System.Byte
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method TypeAnalysisTests::CreateArrayWithLong

  .method public hidebysig instance uint8[] 
          CreateArrayWithUInt(uint32 length) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  1
    .locals init (uint8[] V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  conv.u
    IL_0003:  newarr     [mscorlib]System.Byte
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method TypeAnalysisTests::CreateArrayWithUInt

  .method public hidebysig instance uint8[] 
          CreateArrayWithULong(uint64 length) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  1
    .locals init (uint8[] V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  conv.ovf.i.un
    IL_0003:  newarr     [mscorlib]System.Byte
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method TypeAnalysisTests::CreateArrayWithULong

  .method public hidebysig instance uint8[] 
          CreateArrayWithShort(int16 length) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (uint8[] V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  newarr     [mscorlib]System.Byte
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method TypeAnalysisTests::CreateArrayWithShort

  .method public hidebysig instance uint8[] 
          CreateArrayWithUShort(uint16 length) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (uint8[] V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  newarr     [mscorlib]System.Byte
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method TypeAnalysisTests::CreateArrayWithUShort

  .method public hidebysig instance uint8 
          UseArrayWithInt(int32 i) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  2
    .locals init (uint8 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      uint8[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.TypeAnalysisTests::byteArray
    IL_0007:  ldarg.1
    IL_0008:  ldelem.u1
    IL_0009:  stloc.0
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.0
    IL_000d:  ret
  } // end of method TypeAnalysisTests::UseArrayWithInt

  .method public hidebysig instance uint8 
          UseArrayWithUInt(uint32 i) cil managed
  {
    // Code size       15 (0xf)
    .maxstack  2
    .locals init (uint8 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      uint8[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.TypeAnalysisTests::byteArray
    IL_0007:  ldarg.1
    IL_0008:  conv.u
    IL_0009:  ldelem.u1
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_000d

    IL_000d:  ldloc.0
    IL_000e:  ret
  } // end of method TypeAnalysisTests::UseArrayWithUInt

  .method public hidebysig instance uint8 
          UseArrayWithLong(int64 i) cil managed
  {
    // Code size       15 (0xf)
    .maxstack  2
    .locals init (uint8 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      uint8[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.TypeAnalysisTests::byteArray
    IL_0007:  ldarg.1
    IL_0008:  conv.ovf.i
    IL_0009:  ldelem.u1
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_000d

    IL_000d:  ldloc.0
    IL_000e:  ret
  } // end of method TypeAnalysisTests::UseArrayWithLong

  .method public hidebysig instance uint8 
          UseArrayWithULong(uint64 i) cil managed
  {
    // Code size       15 (0xf)
    .maxstack  2
    .locals init (uint8 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      uint8[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.TypeAnalysisTests::byteArray
    IL_0007:  ldarg.1
    IL_0008:  conv.ovf.i.un
    IL_0009:  ldelem.u1
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_000d

    IL_000d:  ldloc.0
    IL_000e:  ret
  } // end of method TypeAnalysisTests::UseArrayWithULong

  .method public hidebysig instance uint8 
          UseArrayWithShort(int16 i) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  2
    .locals init (uint8 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      uint8[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.TypeAnalysisTests::byteArray
    IL_0007:  ldarg.1
    IL_0008:  ldelem.u1
    IL_0009:  stloc.0
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.0
    IL_000d:  ret
  } // end of method TypeAnalysisTests::UseArrayWithShort

  .method public hidebysig instance uint8 
          UseArrayWithUShort(uint16 i) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  2
    .locals init (uint8 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      uint8[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.TypeAnalysisTests::byteArray
    IL_0007:  ldarg.1
    IL_0008:  ldelem.u1
    IL_0009:  stloc.0
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.0
    IL_000d:  ret
  } // end of method TypeAnalysisTests::UseArrayWithUShort

  .method public hidebysig instance uint8 
          UseArrayWithCastToUShort(int32 i) cil managed
  {
    // Code size       15 (0xf)
    .maxstack  2
    .locals init (uint8 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      uint8[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.TypeAnalysisTests::byteArray
    IL_0007:  ldarg.1
    IL_0008:  conv.u2
    IL_0009:  ldelem.u1
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_000d

    IL_000d:  ldloc.0
    IL_000e:  ret
  } // end of method TypeAnalysisTests::UseArrayWithCastToUShort

  .method public hidebysig instance valuetype [mscorlib]System.StringComparison 
          EnumDiffNumber(valuetype [mscorlib]System.StringComparison data) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.StringComparison V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.1
    IL_0003:  sub
    IL_0004:  stloc.0
    IL_0005:  br.s       IL_0007

    IL_0007:  ldloc.0
    IL_0008:  ret
  } // end of method TypeAnalysisTests::EnumDiffNumber

  .method public hidebysig instance int32 
          EnumDiff(valuetype [mscorlib]System.StringComparison a,
                   valuetype [mscorlib]System.StringComparison b) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  sub
    IL_0004:  call       int32 [mscorlib]System.Math::Abs(int32)
    IL_0009:  stloc.0
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.0
    IL_000d:  ret
  } // end of method TypeAnalysisTests::EnumDiff

  .method public hidebysig instance bool 
          CompareDelegatesByValue(class [mscorlib]System.Action a,
                                  class [mscorlib]System.Action b) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  call       bool [mscorlib]System.Delegate::op_Equality(class [mscorlib]System.Delegate,
                                                                     class [mscorlib]System.Delegate)
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method TypeAnalysisTests::CompareDelegatesByValue

  .method public hidebysig instance bool 
          CompareDelegatesByReference(class [mscorlib]System.Action a,
                                      class [mscorlib]System.Action b) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ceq
    IL_0005:  stloc.0
    IL_0006:  br.s       IL_0008

    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method TypeAnalysisTests::CompareDelegatesByReference

  .method public hidebysig instance bool 
          CompareDelegateWithNull(class [mscorlib]System.Action a) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldnull
    IL_0003:  ceq
    IL_0005:  stloc.0
    IL_0006:  br.s       IL_0008

    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method TypeAnalysisTests::CompareDelegateWithNull

  .method public hidebysig instance bool 
          CompareStringsByValue(string a,
                                string b) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method TypeAnalysisTests::CompareStringsByValue

  .method public hidebysig instance bool 
          CompareStringsByReference(string a,
                                    string b) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  ceq
    IL_0005:  stloc.0
    IL_0006:  br.s       IL_0008

    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method TypeAnalysisTests::CompareStringsByReference

  .method public hidebysig instance bool 
          CompareStringWithNull(string a) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldnull
    IL_0003:  ceq
    IL_0005:  stloc.0
    IL_0006:  br.s       IL_0008

    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method TypeAnalysisTests::CompareStringWithNull

  .method public hidebysig instance valuetype [mscorlib]System.Decimal 
          ImplicitConversionToDecimal(uint8 v) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Decimal V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(uint8)
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method TypeAnalysisTests::ImplicitConversionToDecimal

  .method public hidebysig instance valuetype [mscorlib]System.Decimal 
          ImplicitConversionToDecimal(uint64 v) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Decimal V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(uint64)
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
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
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\TypeAnalysisTests.res
