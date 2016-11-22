
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.18020
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly '0hc4wf1j'
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module '0hc4wf1j.dll'
// MVID: {BB9907D8-9A9B-40EA-99C2-170F6DA15820}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x017B0000


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
    // Code size       29 (0x1d)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.0
    IL_0003:  ldc.i4.0
    IL_0004:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0009:  brfalse.s  IL_0014

    IL_000b:  ldarg.0
    IL_000c:  ldc.i4.1
    IL_000d:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0012:  br.s       IL_0015

    IL_0014:  ldc.i4.0
    IL_0015:  nop
    IL_0016:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_001b:  nop
    IL_001c:  ret
  } // end of method ShortCircuit::ExprAnd

  .method public hidebysig instance void 
          ExprOr() cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.0
    IL_0003:  ldc.i4.0
    IL_0004:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0009:  brtrue.s   IL_0014

    IL_000b:  ldarg.0
    IL_000c:  ldc.i4.1
    IL_000d:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0012:  br.s       IL_0015

    IL_0014:  ldc.i4.1
    IL_0015:  nop
    IL_0016:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_001b:  nop
    IL_001c:  ret
  } // end of method ShortCircuit::ExprOr

  .method public hidebysig instance void 
          ExprCond() cil managed
  {
    // Code size       35 (0x23)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.0
    IL_0003:  ldc.i4.0
    IL_0004:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0009:  brtrue.s   IL_0014

    IL_000b:  ldarg.0
    IL_000c:  ldc.i4.2
    IL_000d:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0012:  br.s       IL_001b

    IL_0014:  ldarg.0
    IL_0015:  ldc.i4.1
    IL_0016:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001b:  nop
    IL_001c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_0021:  nop
    IL_0022:  ret
  } // end of method ShortCircuit::ExprCond

  .method public hidebysig instance void 
          StmtAnd2() cil managed
  {
    // Code size       56 (0x38)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brfalse.s  IL_0016

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  ldc.i4.0
    IL_0012:  ceq
    IL_0014:  br.s       IL_0017

    IL_0016:  ldc.i4.1
    IL_0017:  nop
    IL_0018:  stloc.0
    IL_0019:  ldloc.0
    IL_001a:  brtrue.s   IL_0027

    IL_001c:  nop
    IL_001d:  ldarg.0
    IL_001e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0023:  nop
    IL_0024:  nop
    IL_0025:  br.s       IL_0030

    IL_0027:  nop
    IL_0028:  ldarg.0
    IL_0029:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_002e:  nop
    IL_002f:  nop
    IL_0030:  ldarg.0
    IL_0031:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_0036:  nop
    IL_0037:  ret
  } // end of method ShortCircuit::StmtAnd2

  .method public hidebysig instance void 
          StmtOr2() cil managed
  {
    // Code size       56 (0x38)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brtrue.s   IL_0016

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  ldc.i4.0
    IL_0012:  ceq
    IL_0014:  br.s       IL_0017

    IL_0016:  ldc.i4.0
    IL_0017:  nop
    IL_0018:  stloc.0
    IL_0019:  ldloc.0
    IL_001a:  brtrue.s   IL_0027

    IL_001c:  nop
    IL_001d:  ldarg.0
    IL_001e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0023:  nop
    IL_0024:  nop
    IL_0025:  br.s       IL_0030

    IL_0027:  nop
    IL_0028:  ldarg.0
    IL_0029:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_002e:  nop
    IL_002f:  nop
    IL_0030:  ldarg.0
    IL_0031:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_0036:  nop
    IL_0037:  ret
  } // end of method ShortCircuit::StmtOr2

  .method public hidebysig instance void 
          StmtAnd3() cil managed
  {
    // Code size       65 (0x41)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brfalse.s  IL_001f

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  brfalse.s  IL_001f

    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.2
    IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001a:  ldc.i4.0
    IL_001b:  ceq
    IL_001d:  br.s       IL_0020

    IL_001f:  ldc.i4.1
    IL_0020:  nop
    IL_0021:  stloc.0
    IL_0022:  ldloc.0
    IL_0023:  brtrue.s   IL_0030

    IL_0025:  nop
    IL_0026:  ldarg.0
    IL_0027:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_002c:  nop
    IL_002d:  nop
    IL_002e:  br.s       IL_0039

    IL_0030:  nop
    IL_0031:  ldarg.0
    IL_0032:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0037:  nop
    IL_0038:  nop
    IL_0039:  ldarg.0
    IL_003a:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_003f:  nop
    IL_0040:  ret
  } // end of method ShortCircuit::StmtAnd3

  .method public hidebysig instance void 
          StmtOr3() cil managed
  {
    // Code size       65 (0x41)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brtrue.s   IL_001f

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  brtrue.s   IL_001f

    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.3
    IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001a:  ldc.i4.0
    IL_001b:  ceq
    IL_001d:  br.s       IL_0020

    IL_001f:  ldc.i4.0
    IL_0020:  nop
    IL_0021:  stloc.0
    IL_0022:  ldloc.0
    IL_0023:  brtrue.s   IL_0030

    IL_0025:  nop
    IL_0026:  ldarg.0
    IL_0027:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_002c:  nop
    IL_002d:  nop
    IL_002e:  br.s       IL_0039

    IL_0030:  nop
    IL_0031:  ldarg.0
    IL_0032:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0037:  nop
    IL_0038:  nop
    IL_0039:  ldarg.0
    IL_003a:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_003f:  nop
    IL_0040:  ret
  } // end of method ShortCircuit::StmtOr3

  .method public hidebysig instance void 
          StmtComplex() cil managed
  {
    // Code size       87 (0x57)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brfalse.s  IL_0035

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  brfalse.s  IL_0035

    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.2
    IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001a:  brtrue.s   IL_0035

    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.3
    IL_001e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0023:  brtrue.s   IL_0031

    IL_0025:  ldarg.0
    IL_0026:  ldc.i4.4
    IL_0027:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_002c:  ldc.i4.0
    IL_002d:  ceq
    IL_002f:  br.s       IL_0032

    IL_0031:  ldc.i4.0
    IL_0032:  nop
    IL_0033:  br.s       IL_0036

    IL_0035:  ldc.i4.1
    IL_0036:  nop
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  brtrue.s   IL_0046

    IL_003b:  nop
    IL_003c:  ldarg.0
    IL_003d:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0042:  nop
    IL_0043:  nop
    IL_0044:  br.s       IL_004f

    IL_0046:  nop
    IL_0047:  ldarg.0
    IL_0048:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_004d:  nop
    IL_004e:  nop
    IL_004f:  ldarg.0
    IL_0050:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_0055:  nop
    IL_0056:  ret
  } // end of method ShortCircuit::StmtComplex

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
// WARNING: Created Win32 resource file ../../Tests/TestCases/Pretty\ShortCircuit.res
