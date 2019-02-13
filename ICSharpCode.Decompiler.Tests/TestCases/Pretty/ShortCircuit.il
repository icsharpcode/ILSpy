



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly ShortCircuit
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module ShortCircuit.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


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
          instance int32  GetInt(int32 i) cil managed
  {
  } // end of method ShortCircuit::GetInt

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
          ExprCondAnd() cil managed
  {
    // Code size       44 (0x2c)
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
    IL_0012:  brtrue.s   IL_001d

    IL_0014:  ldarg.0
    IL_0015:  ldc.i4.3
    IL_0016:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001b:  br.s       IL_0024

    IL_001d:  ldarg.0
    IL_001e:  ldc.i4.2
    IL_001f:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0024:  nop
    IL_0025:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_002a:  nop
    IL_002b:  ret
  } // end of method ShortCircuit::ExprCondAnd

  .method public hidebysig instance void 
          ExprMix4A() cil managed
  {
    // Code size       47 (0x2f)
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
    IL_0012:  brfalse.s  IL_001d

    IL_0014:  ldarg.0
    IL_0015:  ldc.i4.2
    IL_0016:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001b:  brtrue.s   IL_0026

    IL_001d:  ldarg.0
    IL_001e:  ldc.i4.3
    IL_001f:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0024:  br.s       IL_0027

    IL_0026:  ldc.i4.1
    IL_0027:  nop
    IL_0028:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_002d:  nop
    IL_002e:  ret
  } // end of method ShortCircuit::ExprMix4A

  .method public hidebysig instance void 
          ExprMix4B() cil managed
  {
    // Code size       51 (0x33)
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
    IL_0012:  brfalse.s  IL_002a

    IL_0014:  ldarg.0
    IL_0015:  ldc.i4.2
    IL_0016:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001b:  brtrue.s   IL_0026

    IL_001d:  ldarg.0
    IL_001e:  ldc.i4.3
    IL_001f:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0024:  br.s       IL_0027

    IL_0026:  ldc.i4.1
    IL_0027:  nop
    IL_0028:  br.s       IL_002b

    IL_002a:  ldc.i4.0
    IL_002b:  nop
    IL_002c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_0031:  nop
    IL_0032:  ret
  } // end of method ShortCircuit::ExprMix4B

  .method public hidebysig instance void 
          ExprMix4C() cil managed
  {
    // Code size       51 (0x33)
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
    IL_0012:  brtrue.s   IL_002a

    IL_0014:  ldarg.0
    IL_0015:  ldc.i4.2
    IL_0016:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001b:  brfalse.s  IL_0026

    IL_001d:  ldarg.0
    IL_001e:  ldc.i4.3
    IL_001f:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0024:  br.s       IL_0027

    IL_0026:  ldc.i4.0
    IL_0027:  nop
    IL_0028:  br.s       IL_002b

    IL_002a:  ldc.i4.1
    IL_002b:  nop
    IL_002c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_0031:  nop
    IL_0032:  ret
  } // end of method ShortCircuit::ExprMix4C

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
          StmtOr2A() cil managed
  {
    // Code size       38 (0x26)
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
    IL_001a:  brtrue.s   IL_0025

    IL_001c:  nop
    IL_001d:  ldarg.0
    IL_001e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0023:  nop
    IL_0024:  nop
    IL_0025:  ret
  } // end of method ShortCircuit::StmtOr2A

  .method public hidebysig instance void 
          StmtOr2B() cil managed
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
  } // end of method ShortCircuit::StmtOr2B

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
    IL_0014:  ldc.i4.2
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
          StmtOr4() cil managed
  {
    // Code size       56 (0x38)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::GetInt(int32)
    IL_0008:  brtrue.s   IL_0016

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::GetInt(int32)
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
  } // end of method ShortCircuit::StmtOr4

  .method public hidebysig instance void 
          StmtMix3A() cil managed
  {
    // Code size       47 (0x2f)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brtrue.s   IL_0013

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
    IL_0023:  brtrue.s   IL_002e

    IL_0025:  nop
    IL_0026:  ldarg.0
    IL_0027:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_002c:  nop
    IL_002d:  nop
    IL_002e:  ret
  } // end of method ShortCircuit::StmtMix3A

  .method public hidebysig instance void 
          StmtMix3B() cil managed
  {
    // Code size       58 (0x3a)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brtrue.s   IL_0013

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
    IL_0039:  ret
  } // end of method ShortCircuit::StmtMix3B

  .method public hidebysig instance void 
          StmtMix4V1A() cil managed
  {
    // Code size       56 (0x38)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brtrue.s   IL_0013

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  brfalse.s  IL_001c

    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.2
    IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001a:  brtrue.s   IL_0028

    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.3
    IL_001e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0023:  ldc.i4.0
    IL_0024:  ceq
    IL_0026:  br.s       IL_0029

    IL_0028:  ldc.i4.0
    IL_0029:  nop
    IL_002a:  stloc.0
    IL_002b:  ldloc.0
    IL_002c:  brtrue.s   IL_0037

    IL_002e:  nop
    IL_002f:  ldarg.0
    IL_0030:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0035:  nop
    IL_0036:  nop
    IL_0037:  ret
  } // end of method ShortCircuit::StmtMix4V1A

  .method public hidebysig instance void 
          StmtMix4V1B() cil managed
  {
    // Code size       67 (0x43)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brtrue.s   IL_0013

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  brfalse.s  IL_001c

    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.2
    IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001a:  brtrue.s   IL_0028

    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.3
    IL_001e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0023:  ldc.i4.0
    IL_0024:  ceq
    IL_0026:  br.s       IL_0029

    IL_0028:  ldc.i4.0
    IL_0029:  nop
    IL_002a:  stloc.0
    IL_002b:  ldloc.0
    IL_002c:  brtrue.s   IL_0039

    IL_002e:  nop
    IL_002f:  ldarg.0
    IL_0030:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0035:  nop
    IL_0036:  nop
    IL_0037:  br.s       IL_0042

    IL_0039:  nop
    IL_003a:  ldarg.0
    IL_003b:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0040:  nop
    IL_0041:  nop
    IL_0042:  ret
  } // end of method ShortCircuit::StmtMix4V1B

  .method public hidebysig instance void 
          StmtMix4V2A() cil managed
  {
    // Code size       60 (0x3c)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brtrue.s   IL_0013

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  brfalse.s  IL_002c

    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.2
    IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001a:  brtrue.s   IL_0028

    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.3
    IL_001e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0023:  ldc.i4.0
    IL_0024:  ceq
    IL_0026:  br.s       IL_0029

    IL_0028:  ldc.i4.0
    IL_0029:  nop
    IL_002a:  br.s       IL_002d

    IL_002c:  ldc.i4.1
    IL_002d:  nop
    IL_002e:  stloc.0
    IL_002f:  ldloc.0
    IL_0030:  brtrue.s   IL_003b

    IL_0032:  nop
    IL_0033:  ldarg.0
    IL_0034:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0039:  nop
    IL_003a:  nop
    IL_003b:  ret
  } // end of method ShortCircuit::StmtMix4V2A

  .method public hidebysig instance void 
          StmtMix4V2B() cil managed
  {
    // Code size       71 (0x47)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brtrue.s   IL_0013

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  brfalse.s  IL_002c

    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.2
    IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001a:  brtrue.s   IL_0028

    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.3
    IL_001e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0023:  ldc.i4.0
    IL_0024:  ceq
    IL_0026:  br.s       IL_0029

    IL_0028:  ldc.i4.0
    IL_0029:  nop
    IL_002a:  br.s       IL_002d

    IL_002c:  ldc.i4.1
    IL_002d:  nop
    IL_002e:  stloc.0
    IL_002f:  ldloc.0
    IL_0030:  brtrue.s   IL_003d

    IL_0032:  nop
    IL_0033:  ldarg.0
    IL_0034:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0039:  nop
    IL_003a:  nop
    IL_003b:  br.s       IL_0046

    IL_003d:  nop
    IL_003e:  ldarg.0
    IL_003f:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0044:  nop
    IL_0045:  nop
    IL_0046:  ret
  } // end of method ShortCircuit::StmtMix4V2B

  .method public hidebysig instance void 
          StmtMix4V3A() cil managed
  {
    // Code size       60 (0x3c)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brfalse.s  IL_0013

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  brtrue.s   IL_002c

    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.2
    IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001a:  brfalse.s  IL_0028

    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.3
    IL_001e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0023:  ldc.i4.0
    IL_0024:  ceq
    IL_0026:  br.s       IL_0029

    IL_0028:  ldc.i4.1
    IL_0029:  nop
    IL_002a:  br.s       IL_002d

    IL_002c:  ldc.i4.0
    IL_002d:  nop
    IL_002e:  stloc.0
    IL_002f:  ldloc.0
    IL_0030:  brtrue.s   IL_003b

    IL_0032:  nop
    IL_0033:  ldarg.0
    IL_0034:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0039:  nop
    IL_003a:  nop
    IL_003b:  ret
  } // end of method ShortCircuit::StmtMix4V3A

  .method public hidebysig instance void 
          StmtMix4V3B() cil managed
  {
    // Code size       71 (0x47)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brfalse.s  IL_0013

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  brtrue.s   IL_002c

    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.2
    IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001a:  brfalse.s  IL_0028

    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.3
    IL_001e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0023:  ldc.i4.0
    IL_0024:  ceq
    IL_0026:  br.s       IL_0029

    IL_0028:  ldc.i4.1
    IL_0029:  nop
    IL_002a:  br.s       IL_002d

    IL_002c:  ldc.i4.0
    IL_002d:  nop
    IL_002e:  stloc.0
    IL_002f:  ldloc.0
    IL_0030:  brtrue.s   IL_003d

    IL_0032:  nop
    IL_0033:  ldarg.0
    IL_0034:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0039:  nop
    IL_003a:  nop
    IL_003b:  br.s       IL_0046

    IL_003d:  nop
    IL_003e:  ldarg.0
    IL_003f:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0044:  nop
    IL_0045:  nop
    IL_0046:  ret
  } // end of method ShortCircuit::StmtMix4V3B

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

  .method public hidebysig instance void 
          StmtComplex2(int32 i) cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4     0x3e8
    IL_0007:  bgt.s      IL_001b

    IL_0009:  ldarg.1
    IL_000a:  ldc.i4.1
    IL_000b:  blt.s      IL_0011

    IL_000d:  ldarg.1
    IL_000e:  ldc.i4.8
    IL_000f:  ble.s      IL_001b

    IL_0011:  ldarg.1
    IL_0012:  ldc.i4.s   42
    IL_0014:  ceq
    IL_0016:  ldc.i4.0
    IL_0017:  ceq
    IL_0019:  br.s       IL_001c

    IL_001b:  ldc.i4.0
    IL_001c:  nop
    IL_001d:  stloc.0
    IL_001e:  ldloc.0
    IL_001f:  brtrue.s   IL_002c

    IL_0021:  nop
    IL_0022:  ldarg.0
    IL_0023:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0028:  nop
    IL_0029:  nop
    IL_002a:  br.s       IL_0035

    IL_002c:  nop
    IL_002d:  ldarg.0
    IL_002e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0033:  nop
    IL_0034:  nop
    IL_0035:  ldarg.0
    IL_0036:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_003b:  nop
    IL_003c:  ret
  } // end of method ShortCircuit::StmtComplex2

  .method public hidebysig instance void 
          StmtComplex3(int32 i) cil managed
  {
    // Code size       74 (0x4a)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4     0x3e8
    IL_0007:  bgt.s      IL_0028

    IL_0009:  ldarg.1
    IL_000a:  ldc.i4.1
    IL_000b:  blt.s      IL_0011

    IL_000d:  ldarg.1
    IL_000e:  ldc.i4.8
    IL_000f:  ble.s      IL_0028

    IL_0011:  ldarg.1
    IL_0012:  ldc.i4.s   100
    IL_0014:  blt.s      IL_001e

    IL_0016:  ldarg.1
    IL_0017:  ldc.i4     0xc8
    IL_001c:  ble.s      IL_0028

    IL_001e:  ldarg.1
    IL_001f:  ldc.i4.s   42
    IL_0021:  ceq
    IL_0023:  ldc.i4.0
    IL_0024:  ceq
    IL_0026:  br.s       IL_0029

    IL_0028:  ldc.i4.0
    IL_0029:  nop
    IL_002a:  stloc.0
    IL_002b:  ldloc.0
    IL_002c:  brtrue.s   IL_0039

    IL_002e:  nop
    IL_002f:  ldarg.0
    IL_0030:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0035:  nop
    IL_0036:  nop
    IL_0037:  br.s       IL_0042

    IL_0039:  nop
    IL_003a:  ldarg.0
    IL_003b:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0040:  nop
    IL_0041:  nop
    IL_0042:  ldarg.0
    IL_0043:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_0048:  nop
    IL_0049:  ret
  } // end of method ShortCircuit::StmtComplex3

  .method public hidebysig instance void 
          StmtComplex4(int32 i) cil managed
  {
    // Code size       66 (0x42)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4     0x3e8
    IL_0007:  bgt.s      IL_0020

    IL_0009:  ldarg.1
    IL_000a:  ldc.i4.1
    IL_000b:  blt.s      IL_0011

    IL_000d:  ldarg.1
    IL_000e:  ldc.i4.8
    IL_000f:  ble.s      IL_0020

    IL_0011:  ldarg.1
    IL_0012:  ldc.i4.s   42
    IL_0014:  beq.s      IL_0020

    IL_0016:  ldarg.1
    IL_0017:  ldc.i4.s   23
    IL_0019:  ceq
    IL_001b:  ldc.i4.0
    IL_001c:  ceq
    IL_001e:  br.s       IL_0021

    IL_0020:  ldc.i4.0
    IL_0021:  nop
    IL_0022:  stloc.0
    IL_0023:  ldloc.0
    IL_0024:  brtrue.s   IL_0031

    IL_0026:  nop
    IL_0027:  ldarg.0
    IL_0028:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_002d:  nop
    IL_002e:  nop
    IL_002f:  br.s       IL_003a

    IL_0031:  nop
    IL_0032:  ldarg.0
    IL_0033:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0038:  nop
    IL_0039:  nop
    IL_003a:  ldarg.0
    IL_003b:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_0040:  nop
    IL_0041:  ret
  } // end of method ShortCircuit::StmtComplex4

  .method public hidebysig instance void 
          StmtComplex5() cil managed
  {
    // Code size       88 (0x58)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  ldc.i4.0
    IL_0009:  ceq
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  brtrue.s   IL_002e

    IL_000f:  nop
    IL_0010:  ldarg.0
    IL_0011:  ldc.i4.1
    IL_0012:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0017:  brtrue.s   IL_0022

    IL_0019:  ldarg.0
    IL_001a:  ldc.i4.2
    IL_001b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0020:  br.s       IL_0023

    IL_0022:  ldc.i4.1
    IL_0023:  nop
    IL_0024:  stloc.0
    IL_0025:  ldloc.0
    IL_0026:  brtrue.s   IL_002b

    IL_0028:  nop
    IL_0029:  br.s       IL_0057

    IL_002b:  nop
    IL_002c:  br.s       IL_0050

    IL_002e:  ldarg.0
    IL_002f:  ldc.i4.3
    IL_0030:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0035:  brfalse.s  IL_0040

    IL_0037:  ldarg.0
    IL_0038:  ldc.i4.4
    IL_0039:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_003e:  br.s       IL_0041

    IL_0040:  ldc.i4.0
    IL_0041:  nop
    IL_0042:  stloc.0
    IL_0043:  ldloc.0
    IL_0044:  brtrue.s   IL_0050

    IL_0046:  nop
    IL_0047:  ldarg.0
    IL_0048:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_004d:  nop
    IL_004e:  br.s       IL_0057

    IL_0050:  ldarg.0
    IL_0051:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_0056:  nop
    IL_0057:  ret
  } // end of method ShortCircuit::StmtComplex5

  .method public hidebysig instance int32 
          StmtComplex6() cil managed
  {
    // Code size       62 (0x3e)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  ldc.i4.0
    IL_0009:  ceq
    IL_000b:  stloc.1
    IL_000c:  ldloc.1
    IL_000d:  brtrue.s   IL_0038

    IL_000f:  nop
    IL_0010:  ldarg.0
    IL_0011:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0016:  nop
    IL_0017:  ldarg.0
    IL_0018:  ldc.i4.1
    IL_0019:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001e:  brtrue.s   IL_002c

    IL_0020:  ldarg.0
    IL_0021:  ldc.i4.2
    IL_0022:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0027:  ldc.i4.0
    IL_0028:  ceq
    IL_002a:  br.s       IL_002d

    IL_002c:  ldc.i4.0
    IL_002d:  nop
    IL_002e:  stloc.1
    IL_002f:  ldloc.1
    IL_0030:  brtrue.s   IL_0037

    IL_0032:  nop
    IL_0033:  ldc.i4.1
    IL_0034:  stloc.0
    IL_0035:  br.s       IL_003c

    IL_0037:  nop
    IL_0038:  ldc.i4.2
    IL_0039:  stloc.0
    IL_003a:  br.s       IL_003c

    IL_003c:  ldloc.0
    IL_003d:  ret
  } // end of method ShortCircuit::StmtComplex6

  .method public hidebysig instance int32 
          InferCorrectOrder() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.1
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brtrue.s   IL_0016

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.2
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  ldc.i4.0
    IL_0012:  ceq
    IL_0014:  br.s       IL_0017

    IL_0016:  ldc.i4.0
    IL_0017:  nop
    IL_0018:  stloc.1
    IL_0019:  ldloc.1
    IL_001a:  brtrue.s   IL_0021

    IL_001c:  nop
    IL_001d:  ldc.i4.1
    IL_001e:  stloc.0
    IL_001f:  br.s       IL_0025

    IL_0021:  ldc.i4.2
    IL_0022:  stloc.0
    IL_0023:  br.s       IL_0025

    IL_0025:  ldloc.0
    IL_0026:  ret
  } // end of method ShortCircuit::InferCorrectOrder

  .method public hidebysig instance void 
          EmptyIf() cil managed
  {
    // Code size       156 (0x9c)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  ldc.i4.0
    IL_0009:  ceq
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  brtrue.s   IL_0011

    IL_000f:  nop
    IL_0010:  nop
    IL_0011:  ldarg.0
    IL_0012:  ldc.i4.1
    IL_0013:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0018:  stloc.0
    IL_0019:  ldloc.0
    IL_001a:  brtrue.s   IL_001e

    IL_001c:  nop
    IL_001d:  nop
    IL_001e:  ldarg.0
    IL_001f:  ldc.i4.2
    IL_0020:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0025:  brfalse.s  IL_0033

    IL_0027:  ldarg.0
    IL_0028:  ldc.i4.3
    IL_0029:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_002e:  ldc.i4.0
    IL_002f:  ceq
    IL_0031:  br.s       IL_0034

    IL_0033:  ldc.i4.1
    IL_0034:  nop
    IL_0035:  stloc.0
    IL_0036:  ldloc.0
    IL_0037:  brtrue.s   IL_003b

    IL_0039:  nop
    IL_003a:  nop
    IL_003b:  ldarg.0
    IL_003c:  ldc.i4.4
    IL_003d:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0042:  brtrue.s   IL_0050

    IL_0044:  ldarg.0
    IL_0045:  ldc.i4.5
    IL_0046:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_004b:  ldc.i4.0
    IL_004c:  ceq
    IL_004e:  br.s       IL_0051

    IL_0050:  ldc.i4.0
    IL_0051:  nop
    IL_0052:  stloc.0
    IL_0053:  ldloc.0
    IL_0054:  brtrue.s   IL_0058

    IL_0056:  nop
    IL_0057:  nop
    IL_0058:  ldarg.0
    IL_0059:  ldc.i4.0
    IL_005a:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_005f:  brfalse.s  IL_008c

    IL_0061:  ldarg.0
    IL_0062:  ldc.i4.1
    IL_0063:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0068:  brfalse.s  IL_008c

    IL_006a:  ldarg.0
    IL_006b:  ldc.i4.2
    IL_006c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0071:  brtrue.s   IL_008c

    IL_0073:  ldarg.0
    IL_0074:  ldc.i4.3
    IL_0075:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_007a:  brtrue.s   IL_0088

    IL_007c:  ldarg.0
    IL_007d:  ldc.i4.4
    IL_007e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0083:  ldc.i4.0
    IL_0084:  ceq
    IL_0086:  br.s       IL_0089

    IL_0088:  ldc.i4.0
    IL_0089:  nop
    IL_008a:  br.s       IL_008d

    IL_008c:  ldc.i4.1
    IL_008d:  nop
    IL_008e:  stloc.0
    IL_008f:  ldloc.0
    IL_0090:  brtrue.s   IL_0094

    IL_0092:  nop
    IL_0093:  nop
    IL_0094:  ldarg.0
    IL_0095:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_009a:  nop
    IL_009b:  ret
  } // end of method ShortCircuit::EmptyIf

  .method public hidebysig instance void 
          PreferLogicalToBitwise(bool a,
                                 bool b,
                                 int32 i,
                                 float32 f) cil managed
  {
    // Code size       90 (0x5a)
    .maxstack  4
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.1
    IL_0003:  brfalse.s  IL_0008

    IL_0005:  ldarg.2
    IL_0006:  br.s       IL_0009

    IL_0008:  ldc.i4.0
    IL_0009:  nop
    IL_000a:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_000f:  nop
    IL_0010:  ldarg.0
    IL_0011:  ldarg.1
    IL_0012:  brfalse.s  IL_001a

    IL_0014:  ldarg.3
    IL_0015:  ldc.i4.1
    IL_0016:  ceq
    IL_0018:  br.s       IL_001b

    IL_001a:  ldc.i4.0
    IL_001b:  nop
    IL_001c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_0021:  nop
    IL_0022:  ldarg.0
    IL_0023:  ldarg.3
    IL_0024:  ldc.i4.1
    IL_0025:  bne.un.s   IL_002a

    IL_0027:  ldarg.1
    IL_0028:  br.s       IL_002b

    IL_002a:  ldc.i4.0
    IL_002b:  nop
    IL_002c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_0031:  nop
    IL_0032:  ldarg.0
    IL_0033:  ldarg.3
    IL_0034:  ldarg.3
    IL_0035:  ldc.i4.3
    IL_0036:  sub
    IL_0037:  ble.s      IL_003c

    IL_0039:  ldarg.1
    IL_003a:  br.s       IL_003d

    IL_003c:  ldc.i4.0
    IL_003d:  nop
    IL_003e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_0043:  nop
    IL_0044:  ldarg.0
    IL_0045:  ldarg.s    f
    IL_0047:  ldc.r4     0.1
    IL_004c:  bge.un.s   IL_0051

    IL_004e:  ldarg.1
    IL_004f:  br.s       IL_0052

    IL_0051:  ldc.i4.0
    IL_0052:  nop
    IL_0053:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_0058:  nop
    IL_0059:  ret
  } // end of method ShortCircuit::PreferLogicalToBitwise

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
