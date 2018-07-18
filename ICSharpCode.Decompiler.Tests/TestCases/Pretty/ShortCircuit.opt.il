



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly ShortCircuit.opt
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module ShortCircuit.opt.dll
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
          ExprMix4A() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  8
    IL_0000:  ldarg.0
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
    IL_001a:  brtrue.s   IL_0025

    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.3
    IL_001e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0023:  br.s       IL_0026

    IL_0025:  ldc.i4.1
    IL_0026:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_002b:  ret
  } // end of method ShortCircuit::ExprMix4A

  .method public hidebysig instance void 
          ExprMix4B() cil managed
  {
    // Code size       47 (0x2f)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brtrue.s   IL_0013

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  brfalse.s  IL_0028

    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.2
    IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001a:  brtrue.s   IL_0025

    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.3
    IL_001e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0023:  br.s       IL_0029

    IL_0025:  ldc.i4.1
    IL_0026:  br.s       IL_0029

    IL_0028:  ldc.i4.0
    IL_0029:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_002e:  ret
  } // end of method ShortCircuit::ExprMix4B

  .method public hidebysig instance void 
          ExprMix4C() cil managed
  {
    // Code size       47 (0x2f)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brfalse.s  IL_0013

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  brtrue.s   IL_0028

    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.2
    IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001a:  brfalse.s  IL_0025

    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.3
    IL_001e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0023:  br.s       IL_0029

    IL_0025:  ldc.i4.0
    IL_0026:  br.s       IL_0029

    IL_0028:  ldc.i4.1
    IL_0029:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_002e:  ret
  } // end of method ShortCircuit::ExprMix4C

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
          StmtOr2A() cil managed
  {
    // Code size       25 (0x19)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0007:  brtrue.s   IL_0012

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0010:  brfalse.s  IL_0018

    IL_0012:  ldarg.0
    IL_0013:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0018:  ret
  } // end of method ShortCircuit::StmtOr2A

  .method public hidebysig instance void 
          StmtOr2B() cil managed
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
  } // end of method ShortCircuit::StmtOr2B

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
  } // end of method ShortCircuit::StmtOr3

  .method public hidebysig instance void 
          StmtOr4() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::GetInt(int32)
    IL_0007:  brtrue.s   IL_0012

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::GetInt(int32)
    IL_0010:  brfalse.s  IL_001a

    IL_0012:  ldarg.0
    IL_0013:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0018:  br.s       IL_0020

    IL_001a:  ldarg.0
    IL_001b:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0020:  ldarg.0
    IL_0021:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_0026:  ret
  } // end of method ShortCircuit::StmtOr4

  .method public hidebysig instance void 
          StmtMix3A() cil managed
  {
    // Code size       34 (0x22)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0007:  brtrue.s   IL_0012

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0010:  brfalse.s  IL_0021

    IL_0012:  ldarg.0
    IL_0013:  ldc.i4.2
    IL_0014:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0019:  brfalse.s  IL_0021

    IL_001b:  ldarg.0
    IL_001c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0021:  ret
  } // end of method ShortCircuit::StmtMix3A

  .method public hidebysig instance void 
          StmtMix3B() cil managed
  {
    // Code size       41 (0x29)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0007:  brtrue.s   IL_0012

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0010:  brfalse.s  IL_0022

    IL_0012:  ldarg.0
    IL_0013:  ldc.i4.2
    IL_0014:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0019:  brfalse.s  IL_0022

    IL_001b:  ldarg.0
    IL_001c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0021:  ret

    IL_0022:  ldarg.0
    IL_0023:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0028:  ret
  } // end of method ShortCircuit::StmtMix3B

  .method public hidebysig instance void 
          StmtMix4V1A() cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0007:  brtrue.s   IL_0012

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0010:  brfalse.s  IL_001b

    IL_0012:  ldarg.0
    IL_0013:  ldc.i4.2
    IL_0014:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0019:  brtrue.s   IL_0024

    IL_001b:  ldarg.0
    IL_001c:  ldc.i4.3
    IL_001d:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0022:  brfalse.s  IL_002a

    IL_0024:  ldarg.0
    IL_0025:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_002a:  ret
  } // end of method ShortCircuit::StmtMix4V1A

  .method public hidebysig instance void 
          StmtMix4V1B() cil managed
  {
    // Code size       50 (0x32)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0007:  brtrue.s   IL_0012

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0010:  brfalse.s  IL_001b

    IL_0012:  ldarg.0
    IL_0013:  ldc.i4.2
    IL_0014:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0019:  brtrue.s   IL_0024

    IL_001b:  ldarg.0
    IL_001c:  ldc.i4.3
    IL_001d:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0022:  brfalse.s  IL_002b

    IL_0024:  ldarg.0
    IL_0025:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_002a:  ret

    IL_002b:  ldarg.0
    IL_002c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0031:  ret
  } // end of method ShortCircuit::StmtMix4V1B

  .method public hidebysig instance void 
          StmtMix4V2A() cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0007:  brtrue.s   IL_0012

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0010:  brfalse.s  IL_002a

    IL_0012:  ldarg.0
    IL_0013:  ldc.i4.2
    IL_0014:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0019:  brtrue.s   IL_0024

    IL_001b:  ldarg.0
    IL_001c:  ldc.i4.3
    IL_001d:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0022:  brfalse.s  IL_002a

    IL_0024:  ldarg.0
    IL_0025:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_002a:  ret
  } // end of method ShortCircuit::StmtMix4V2A

  .method public hidebysig instance void 
          StmtMix4V2B() cil managed
  {
    // Code size       50 (0x32)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0007:  brtrue.s   IL_0012

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0010:  brfalse.s  IL_002b

    IL_0012:  ldarg.0
    IL_0013:  ldc.i4.2
    IL_0014:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0019:  brtrue.s   IL_0024

    IL_001b:  ldarg.0
    IL_001c:  ldc.i4.3
    IL_001d:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0022:  brfalse.s  IL_002b

    IL_0024:  ldarg.0
    IL_0025:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_002a:  ret

    IL_002b:  ldarg.0
    IL_002c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0031:  ret
  } // end of method ShortCircuit::StmtMix4V2B

  .method public hidebysig instance void 
          StmtMix4V3A() cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0007:  brfalse.s  IL_0012

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0010:  brtrue.s   IL_0024

    IL_0012:  ldarg.0
    IL_0013:  ldc.i4.2
    IL_0014:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0019:  brfalse.s  IL_002a

    IL_001b:  ldarg.0
    IL_001c:  ldc.i4.3
    IL_001d:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0022:  brfalse.s  IL_002a

    IL_0024:  ldarg.0
    IL_0025:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_002a:  ret
  } // end of method ShortCircuit::StmtMix4V3A

  .method public hidebysig instance void 
          StmtMix4V3B() cil managed
  {
    // Code size       50 (0x32)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0007:  brfalse.s  IL_0012

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0010:  brtrue.s   IL_0024

    IL_0012:  ldarg.0
    IL_0013:  ldc.i4.2
    IL_0014:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0019:  brfalse.s  IL_002b

    IL_001b:  ldarg.0
    IL_001c:  ldc.i4.3
    IL_001d:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0022:  brfalse.s  IL_002b

    IL_0024:  ldarg.0
    IL_0025:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_002a:  ret

    IL_002b:  ldarg.0
    IL_002c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0031:  ret
  } // end of method ShortCircuit::StmtMix4V3B

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

  .method public hidebysig instance void 
          StmtComplex5() cil managed
  {
    // Code size       60 (0x3c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0007:  brfalse.s  IL_001c

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.1
    IL_000b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0010:  brtrue.s   IL_0035

    IL_0012:  ldarg.0
    IL_0013:  ldc.i4.2
    IL_0014:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0019:  brtrue.s   IL_0035

    IL_001b:  ret

    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.3
    IL_001e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0023:  brfalse.s  IL_002e

    IL_0025:  ldarg.0
    IL_0026:  ldc.i4.4
    IL_0027:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_002c:  brtrue.s   IL_0035

    IL_002e:  ldarg.0
    IL_002f:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0034:  ret

    IL_0035:  ldarg.0
    IL_0036:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_003b:  ret
  } // end of method ShortCircuit::StmtComplex5

  .method public hidebysig instance int32 
          StmtComplex6() cil managed
  {
    // Code size       37 (0x25)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0007:  brfalse.s  IL_0023

    IL_0009:  ldarg.0
    IL_000a:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_000f:  ldarg.0
    IL_0010:  ldc.i4.1
    IL_0011:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0016:  brtrue.s   IL_0021

    IL_0018:  ldarg.0
    IL_0019:  ldc.i4.2
    IL_001a:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001f:  brfalse.s  IL_0023

    IL_0021:  ldc.i4.1
    IL_0022:  ret

    IL_0023:  ldc.i4.2
    IL_0024:  ret
  } // end of method ShortCircuit::StmtComplex6

  .method public hidebysig instance int32 
          InferCorrectOrder() cil managed
  {
    // Code size       22 (0x16)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0007:  brtrue.s   IL_0012

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.2
    IL_000b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0010:  brfalse.s  IL_0014

    IL_0012:  ldc.i4.1
    IL_0013:  ret

    IL_0014:  ldc.i4.2
    IL_0015:  ret
  } // end of method ShortCircuit::InferCorrectOrder

  .method public hidebysig instance void 
          PreferLogicalToBitwise(bool a,
                                 bool b,
                                 int32 i,
                                 float32 f) cil managed
  {
    // Code size       79 (0x4f)
    .maxstack  4
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  brfalse.s  IL_0007

    IL_0004:  ldarg.2
    IL_0005:  br.s       IL_0008

    IL_0007:  ldc.i4.0
    IL_0008:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_000d:  ldarg.0
    IL_000e:  ldarg.1
    IL_000f:  brfalse.s  IL_0017

    IL_0011:  ldarg.3
    IL_0012:  ldc.i4.1
    IL_0013:  ceq
    IL_0015:  br.s       IL_0018

    IL_0017:  ldc.i4.0
    IL_0018:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_001d:  ldarg.0
    IL_001e:  ldarg.3
    IL_001f:  ldc.i4.1
    IL_0020:  bne.un.s   IL_0025

    IL_0022:  ldarg.1
    IL_0023:  br.s       IL_0026

    IL_0025:  ldc.i4.0
    IL_0026:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_002b:  ldarg.0
    IL_002c:  ldarg.3
    IL_002d:  ldarg.3
    IL_002e:  ldc.i4.3
    IL_002f:  sub
    IL_0030:  ble.s      IL_0035

    IL_0032:  ldarg.1
    IL_0033:  br.s       IL_0036

    IL_0035:  ldc.i4.0
    IL_0036:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_003b:  ldarg.0
    IL_003c:  ldarg.s    f
    IL_003e:  ldc.r4     0.1
    IL_0043:  bge.un.s   IL_0048

    IL_0045:  ldarg.1
    IL_0046:  br.s       IL_0049

    IL_0048:  ldc.i4.0
    IL_0049:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_004e:  ret
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
