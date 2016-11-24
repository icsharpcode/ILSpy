
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.18020
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly kcfcwzhz
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module kcfcwzhz.dll
// MVID: {9375913F-19AE-4F80-AC4E-1691FD0225AD}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x01220000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit
       extends [mscorlib]System.Object
{
  .method public hidebysig newslot abstract virtual 
          instance void  B(bool b) cil managed
  {
  } // end of method ShortCircuit::B

  .method public hidebysig newslot abstract virtual 
          instance bool  F(int32 i) cil managed
  {
  } // end of method ShortCircuit::F

  .method public hidebysig newslot abstract virtual 
          instance void  M1() cil managed
  {
  } // end of method ShortCircuit::M1

  .method public hidebysig newslot abstract virtual 
          instance void  M2() cil managed
  {
  } // end of method ShortCircuit::M2

  .method public hidebysig newslot abstract virtual 
          instance void  E() cil managed
  {
  } // end of method ShortCircuit::E

  .method public hidebysig instance void 
          ExprAnd() cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brfalse.s  IL_0013

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  br.s       IL_0014

    IL_0013:  ldc.i4.0
    IL_0014:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_0019:  ret
  } // end of method ShortCircuit::ExprAnd

  .method public hidebysig instance void 
          ExprOr() cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brtrue.s   IL_0013

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  br.s       IL_0014

    IL_0013:  ldc.i4.1
    IL_0014:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_0019:  ret
  } // end of method ShortCircuit::ExprOr

  .method public hidebysig instance void 
          ExprCond() cil managed
  {
    // Code size       32 (0x20)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brtrue.s   IL_0013

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.2
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  br.s       IL_001a

    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.1
    IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001a:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_001f:  ret
  } // end of method ShortCircuit::ExprCond

  .method public hidebysig instance void 
          ExprCondAnd() cil managed
  {
    // Code size       41 (0x29)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brfalse.s  IL_0013

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  brtrue.s   IL_001c

    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.3
    IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001a:  br.s       IL_0023

    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.2
    IL_001e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0023:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_0028:  ret
  } // end of method ShortCircuit::ExprCondAnd

  .method public hidebysig instance void 
          StmtAnd2() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0007:  brfalse.s  IL_001a

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0010:  brfalse.s  IL_001a

    IL_0012:  ldarg.0
    IL_0013:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0018:  br.s       IL_0020

    IL_001a:  ldarg.0
    IL_001b:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0020:  ldarg.0
    IL_0021:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_0026:  ret
  } // end of method ShortCircuit::StmtAnd2

  .method public hidebysig instance void 
          StmtOr2() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0007:  brtrue.s   IL_0012

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0010:  brfalse.s  IL_001a

    IL_0012:  ldarg.0
    IL_0013:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0018:  br.s       IL_0020

    IL_001a:  ldarg.0
    IL_001b:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0020:  ldarg.0
    IL_0021:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_0026:  ret
  } // end of method ShortCircuit::StmtOr2

  .method public hidebysig instance void 
          StmtAnd3() cil managed
  {
    // Code size       48 (0x30)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0007:  brfalse.s  IL_0023

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0010:  brfalse.s  IL_0023

    IL_0012:  ldarg.0
    IL_0013:  ldc.i4.2
    IL_0014:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0019:  brfalse.s  IL_0023

    IL_001b:  ldarg.0
    IL_001c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0021:  br.s       IL_0029

    IL_0023:  ldarg.0
    IL_0024:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0029:  ldarg.0
    IL_002a:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_002f:  ret
  } // end of method ShortCircuit::StmtAnd3

  .method public hidebysig instance void 
          StmtOr3() cil managed
  {
    // Code size       48 (0x30)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0007:  brtrue.s   IL_001b

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0010:  brtrue.s   IL_001b

    IL_0012:  ldarg.0
    IL_0013:  ldc.i4.3
    IL_0014:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0019:  brfalse.s  IL_0023

    IL_001b:  ldarg.0
    IL_001c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0021:  br.s       IL_0029

    IL_0023:  ldarg.0
    IL_0024:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0029:  ldarg.0
    IL_002a:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_002f:  ret
  } // end of method ShortCircuit::StmtOr3

  .method public hidebysig instance void 
          StmtComplex() cil managed
  {
    // Code size       66 (0x42)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0007:  brfalse.s  IL_0035

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0010:  brfalse.s  IL_0035

    IL_0012:  ldarg.0
    IL_0013:  ldc.i4.2
    IL_0014:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0019:  brtrue.s   IL_0035

    IL_001b:  ldarg.0
    IL_001c:  ldc.i4.3
    IL_001d:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0022:  brtrue.s   IL_002d

    IL_0024:  ldarg.0
    IL_0025:  ldc.i4.4
    IL_0026:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_002b:  brfalse.s  IL_0035

    IL_002d:  ldarg.0
    IL_002e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0033:  br.s       IL_003b

    IL_0035:  ldarg.0
    IL_0036:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_003b:  ldarg.0
    IL_003c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_0041:  ret
  } // end of method ShortCircuit::StmtComplex

  .method public hidebysig instance void 
          StmtComplex2(int32 i) cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4     0x3e8
    IL_0006:  bgt.s      IL_0015

    IL_0008:  ldarg.1
    IL_0009:  ldc.i4.1
    IL_000a:  blt.s      IL_0010

    IL_000c:  ldarg.1
    IL_000d:  ldc.i4.8
    IL_000e:  ble.s      IL_0015

    IL_0010:  ldarg.1
    IL_0011:  ldc.i4.s   42
    IL_0013:  bne.un.s   IL_001d

    IL_0015:  ldarg.0
    IL_0016:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_001b:  br.s       IL_0023

    IL_001d:  ldarg.0
    IL_001e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0023:  ldarg.0
    IL_0024:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_0029:  ret
  } // end of method ShortCircuit::StmtComplex2

  .method public hidebysig instance void 
          StmtComplex3(int32 i) cil managed
  {
    // Code size       55 (0x37)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4     0x3e8
    IL_0006:  bgt.s      IL_0022

    IL_0008:  ldarg.1
    IL_0009:  ldc.i4.1
    IL_000a:  blt.s      IL_0010

    IL_000c:  ldarg.1
    IL_000d:  ldc.i4.8
    IL_000e:  ble.s      IL_0022

    IL_0010:  ldarg.1
    IL_0011:  ldc.i4.s   100
    IL_0013:  blt.s      IL_001d

    IL_0015:  ldarg.1
    IL_0016:  ldc.i4     0xc8
    IL_001b:  ble.s      IL_0022

    IL_001d:  ldarg.1
    IL_001e:  ldc.i4.s   42
    IL_0020:  bne.un.s   IL_002a

    IL_0022:  ldarg.0
    IL_0023:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0028:  br.s       IL_0030

    IL_002a:  ldarg.0
    IL_002b:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0030:  ldarg.0
    IL_0031:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_0036:  ret
  } // end of method ShortCircuit::StmtComplex3

  .method public hidebysig instance void 
          StmtComplex4(int32 i) cil managed
  {
    // Code size       47 (0x2f)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4     0x3e8
    IL_0006:  bgt.s      IL_001a

    IL_0008:  ldarg.1
    IL_0009:  ldc.i4.1
    IL_000a:  blt.s      IL_0010

    IL_000c:  ldarg.1
    IL_000d:  ldc.i4.8
    IL_000e:  ble.s      IL_001a

    IL_0010:  ldarg.1
    IL_0011:  ldc.i4.s   42
    IL_0013:  beq.s      IL_001a

    IL_0015:  ldarg.1
    IL_0016:  ldc.i4.s   23
    IL_0018:  bne.un.s   IL_0022

    IL_001a:  ldarg.0
    IL_001b:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0020:  br.s       IL_0028

    IL_0022:  ldarg.0
    IL_0023:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0028:  ldarg.0
    IL_0029:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_002e:  ret
  } // end of method ShortCircuit::StmtComplex4

  .method family hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method ShortCircuit::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../Tests/TestCases/Pretty\ShortCircuit.opt.res
