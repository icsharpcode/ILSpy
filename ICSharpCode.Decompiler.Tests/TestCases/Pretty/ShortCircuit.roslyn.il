



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

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

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
    // Code size       28 (0x1c)
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
    IL_0015:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_001a:  nop
    IL_001b:  ret
  } // end of method ShortCircuit::ExprAnd

  .method public hidebysig instance void 
          ExprOr() cil managed
  {
    // Code size       28 (0x1c)
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
    IL_0015:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_001a:  nop
    IL_001b:  ret
  } // end of method ShortCircuit::ExprOr

  .method public hidebysig instance void 
          ExprCond() cil managed
  {
    // Code size       34 (0x22)
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
    IL_001b:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_0020:  nop
    IL_0021:  ret
  } // end of method ShortCircuit::ExprCond

  .method public hidebysig instance void 
          ExprCondAnd() cil managed
  {
    // Code size       43 (0x2b)
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
    IL_0024:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_0029:  nop
    IL_002a:  ret
  } // end of method ShortCircuit::ExprCondAnd

  .method public hidebysig instance void 
          ExprMix4A() cil managed
  {
    // Code size       46 (0x2e)
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
    IL_0027:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_002c:  nop
    IL_002d:  ret
  } // end of method ShortCircuit::ExprMix4A

  .method public hidebysig instance void 
          ExprMix4B() cil managed
  {
    // Code size       49 (0x31)
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
    IL_0012:  brfalse.s  IL_0029

    IL_0014:  ldarg.0
    IL_0015:  ldc.i4.2
    IL_0016:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001b:  brtrue.s   IL_0026

    IL_001d:  ldarg.0
    IL_001e:  ldc.i4.3
    IL_001f:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0024:  br.s       IL_0027

    IL_0026:  ldc.i4.1
    IL_0027:  br.s       IL_002a

    IL_0029:  ldc.i4.0
    IL_002a:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_002f:  nop
    IL_0030:  ret
  } // end of method ShortCircuit::ExprMix4B

  .method public hidebysig instance void 
          ExprMix4C() cil managed
  {
    // Code size       49 (0x31)
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
    IL_0012:  brtrue.s   IL_0029

    IL_0014:  ldarg.0
    IL_0015:  ldc.i4.2
    IL_0016:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001b:  brfalse.s  IL_0026

    IL_001d:  ldarg.0
    IL_001e:  ldc.i4.3
    IL_001f:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0024:  br.s       IL_0027

    IL_0026:  ldc.i4.0
    IL_0027:  br.s       IL_002a

    IL_0029:  ldc.i4.1
    IL_002a:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_002f:  nop
    IL_0030:  ret
  } // end of method ShortCircuit::ExprMix4C

  .method public hidebysig instance void 
          StmtAnd2() cil managed
  {
    // Code size       52 (0x34)
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
    IL_0011:  br.s       IL_0014

    IL_0013:  ldc.i4.0
    IL_0014:  stloc.0
    IL_0015:  ldloc.0
    IL_0016:  brfalse.s  IL_0023

    IL_0018:  nop
    IL_0019:  ldarg.0
    IL_001a:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_001f:  nop
    IL_0020:  nop
    IL_0021:  br.s       IL_002c

    IL_0023:  nop
    IL_0024:  ldarg.0
    IL_0025:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_002a:  nop
    IL_002b:  nop
    IL_002c:  ldarg.0
    IL_002d:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_0032:  nop
    IL_0033:  ret
  } // end of method ShortCircuit::StmtAnd2

  .method public hidebysig instance void 
          StmtOr2A() cil managed
  {
    // Code size       34 (0x22)
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
    IL_0011:  br.s       IL_0014

    IL_0013:  ldc.i4.1
    IL_0014:  stloc.0
    IL_0015:  ldloc.0
    IL_0016:  brfalse.s  IL_0021

    IL_0018:  nop
    IL_0019:  ldarg.0
    IL_001a:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_001f:  nop
    IL_0020:  nop
    IL_0021:  ret
  } // end of method ShortCircuit::StmtOr2A

  .method public hidebysig instance void 
          StmtOr2B() cil managed
  {
    // Code size       52 (0x34)
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
    IL_0011:  br.s       IL_0014

    IL_0013:  ldc.i4.1
    IL_0014:  stloc.0
    IL_0015:  ldloc.0
    IL_0016:  brfalse.s  IL_0023

    IL_0018:  nop
    IL_0019:  ldarg.0
    IL_001a:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_001f:  nop
    IL_0020:  nop
    IL_0021:  br.s       IL_002c

    IL_0023:  nop
    IL_0024:  ldarg.0
    IL_0025:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_002a:  nop
    IL_002b:  nop
    IL_002c:  ldarg.0
    IL_002d:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_0032:  nop
    IL_0033:  ret
  } // end of method ShortCircuit::StmtOr2B

  .method public hidebysig instance void 
          StmtAnd3() cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brfalse.s  IL_001c

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  brfalse.s  IL_001c

    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.2
    IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001a:  br.s       IL_001d

    IL_001c:  ldc.i4.0
    IL_001d:  stloc.0
    IL_001e:  ldloc.0
    IL_001f:  brfalse.s  IL_002c

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
  } // end of method ShortCircuit::StmtAnd3

  .method public hidebysig instance void 
          StmtOr3() cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brtrue.s   IL_001c

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  brtrue.s   IL_001c

    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.2
    IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001a:  br.s       IL_001d

    IL_001c:  ldc.i4.1
    IL_001d:  stloc.0
    IL_001e:  ldloc.0
    IL_001f:  brfalse.s  IL_002c

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
  } // end of method ShortCircuit::StmtOr3

  .method public hidebysig instance void 
          StmtOr4() cil managed
  {
    // Code size       55 (0x37)
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
    IL_0012:  cgt.un
    IL_0014:  br.s       IL_0017

    IL_0016:  ldc.i4.1
    IL_0017:  stloc.0
    IL_0018:  ldloc.0
    IL_0019:  brfalse.s  IL_0026

    IL_001b:  nop
    IL_001c:  ldarg.0
    IL_001d:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0022:  nop
    IL_0023:  nop
    IL_0024:  br.s       IL_002f

    IL_0026:  nop
    IL_0027:  ldarg.0
    IL_0028:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_002d:  nop
    IL_002e:  nop
    IL_002f:  ldarg.0
    IL_0030:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_0035:  nop
    IL_0036:  ret
  } // end of method ShortCircuit::StmtOr4

  .method public hidebysig instance void 
          StmtMix3A() cil managed
  {
    // Code size       43 (0x2b)
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
    IL_001a:  br.s       IL_001d

    IL_001c:  ldc.i4.0
    IL_001d:  stloc.0
    IL_001e:  ldloc.0
    IL_001f:  brfalse.s  IL_002a

    IL_0021:  nop
    IL_0022:  ldarg.0
    IL_0023:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0028:  nop
    IL_0029:  nop
    IL_002a:  ret
  } // end of method ShortCircuit::StmtMix3A

  .method public hidebysig instance void 
          StmtMix3B() cil managed
  {
    // Code size       54 (0x36)
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
    IL_001a:  br.s       IL_001d

    IL_001c:  ldc.i4.0
    IL_001d:  stloc.0
    IL_001e:  ldloc.0
    IL_001f:  brfalse.s  IL_002c

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
    IL_0035:  ret
  } // end of method ShortCircuit::StmtMix3B

  .method public hidebysig instance void 
          StmtMix4V1A() cil managed
  {
    // Code size       52 (0x34)
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
    IL_001a:  brtrue.s   IL_0025

    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.3
    IL_001e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0023:  br.s       IL_0026

    IL_0025:  ldc.i4.1
    IL_0026:  stloc.0
    IL_0027:  ldloc.0
    IL_0028:  brfalse.s  IL_0033

    IL_002a:  nop
    IL_002b:  ldarg.0
    IL_002c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0031:  nop
    IL_0032:  nop
    IL_0033:  ret
  } // end of method ShortCircuit::StmtMix4V1A

  .method public hidebysig instance void 
          StmtMix4V1B() cil managed
  {
    // Code size       63 (0x3f)
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
    IL_001a:  brtrue.s   IL_0025

    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.3
    IL_001e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0023:  br.s       IL_0026

    IL_0025:  ldc.i4.1
    IL_0026:  stloc.0
    IL_0027:  ldloc.0
    IL_0028:  brfalse.s  IL_0035

    IL_002a:  nop
    IL_002b:  ldarg.0
    IL_002c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0031:  nop
    IL_0032:  nop
    IL_0033:  br.s       IL_003e

    IL_0035:  nop
    IL_0036:  ldarg.0
    IL_0037:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_003c:  nop
    IL_003d:  nop
    IL_003e:  ret
  } // end of method ShortCircuit::StmtMix4V1B

  .method public hidebysig instance void 
          StmtMix4V2A() cil managed
  {
    // Code size       55 (0x37)
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
    IL_0011:  brfalse.s  IL_0028

    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.2
    IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001a:  brtrue.s   IL_0025

    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.3
    IL_001e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0023:  br.s       IL_0026

    IL_0025:  ldc.i4.1
    IL_0026:  br.s       IL_0029

    IL_0028:  ldc.i4.0
    IL_0029:  stloc.0
    IL_002a:  ldloc.0
    IL_002b:  brfalse.s  IL_0036

    IL_002d:  nop
    IL_002e:  ldarg.0
    IL_002f:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0034:  nop
    IL_0035:  nop
    IL_0036:  ret
  } // end of method ShortCircuit::StmtMix4V2A

  .method public hidebysig instance void 
          StmtMix4V2B() cil managed
  {
    // Code size       66 (0x42)
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
    IL_0011:  brfalse.s  IL_0028

    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.2
    IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001a:  brtrue.s   IL_0025

    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.3
    IL_001e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0023:  br.s       IL_0026

    IL_0025:  ldc.i4.1
    IL_0026:  br.s       IL_0029

    IL_0028:  ldc.i4.0
    IL_0029:  stloc.0
    IL_002a:  ldloc.0
    IL_002b:  brfalse.s  IL_0038

    IL_002d:  nop
    IL_002e:  ldarg.0
    IL_002f:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0034:  nop
    IL_0035:  nop
    IL_0036:  br.s       IL_0041

    IL_0038:  nop
    IL_0039:  ldarg.0
    IL_003a:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_003f:  nop
    IL_0040:  nop
    IL_0041:  ret
  } // end of method ShortCircuit::StmtMix4V2B

  .method public hidebysig instance void 
          StmtMix4V3A() cil managed
  {
    // Code size       55 (0x37)
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
    IL_0011:  brtrue.s   IL_0028

    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.2
    IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001a:  brfalse.s  IL_0025

    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.3
    IL_001e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0023:  br.s       IL_0026

    IL_0025:  ldc.i4.0
    IL_0026:  br.s       IL_0029

    IL_0028:  ldc.i4.1
    IL_0029:  stloc.0
    IL_002a:  ldloc.0
    IL_002b:  brfalse.s  IL_0036

    IL_002d:  nop
    IL_002e:  ldarg.0
    IL_002f:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0034:  nop
    IL_0035:  nop
    IL_0036:  ret
  } // end of method ShortCircuit::StmtMix4V3A

  .method public hidebysig instance void 
          StmtMix4V3B() cil managed
  {
    // Code size       66 (0x42)
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
    IL_0011:  brtrue.s   IL_0028

    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.2
    IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001a:  brfalse.s  IL_0025

    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.3
    IL_001e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0023:  br.s       IL_0026

    IL_0025:  ldc.i4.0
    IL_0026:  br.s       IL_0029

    IL_0028:  ldc.i4.1
    IL_0029:  stloc.0
    IL_002a:  ldloc.0
    IL_002b:  brfalse.s  IL_0038

    IL_002d:  nop
    IL_002e:  ldarg.0
    IL_002f:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0034:  nop
    IL_0035:  nop
    IL_0036:  br.s       IL_0041

    IL_0038:  nop
    IL_0039:  ldarg.0
    IL_003a:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_003f:  nop
    IL_0040:  nop
    IL_0041:  ret
  } // end of method ShortCircuit::StmtMix4V3B

  .method public hidebysig instance void 
          StmtComplex() cil managed
  {
    // Code size       82 (0x52)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brfalse.s  IL_0031

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  brfalse.s  IL_0031

    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.2
    IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001a:  brtrue.s   IL_0031

    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.3
    IL_001e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0023:  brtrue.s   IL_002e

    IL_0025:  ldarg.0
    IL_0026:  ldc.i4.4
    IL_0027:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_002c:  br.s       IL_002f

    IL_002e:  ldc.i4.1
    IL_002f:  br.s       IL_0032

    IL_0031:  ldc.i4.0
    IL_0032:  stloc.0
    IL_0033:  ldloc.0
    IL_0034:  brfalse.s  IL_0041

    IL_0036:  nop
    IL_0037:  ldarg.0
    IL_0038:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_003d:  nop
    IL_003e:  nop
    IL_003f:  br.s       IL_004a

    IL_0041:  nop
    IL_0042:  ldarg.0
    IL_0043:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0048:  nop
    IL_0049:  nop
    IL_004a:  ldarg.0
    IL_004b:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_0050:  nop
    IL_0051:  ret
  } // end of method ShortCircuit::StmtComplex

  .method public hidebysig instance void 
          StmtComplex2(int32 i) cil managed
  {
    // Code size       57 (0x39)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4     0x3e8
    IL_0007:  bgt.s      IL_0018

    IL_0009:  ldarg.1
    IL_000a:  ldc.i4.1
    IL_000b:  blt.s      IL_0011

    IL_000d:  ldarg.1
    IL_000e:  ldc.i4.8
    IL_000f:  ble.s      IL_0018

    IL_0011:  ldarg.1
    IL_0012:  ldc.i4.s   42
    IL_0014:  ceq
    IL_0016:  br.s       IL_0019

    IL_0018:  ldc.i4.1
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  brfalse.s  IL_0028

    IL_001d:  nop
    IL_001e:  ldarg.0
    IL_001f:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0024:  nop
    IL_0025:  nop
    IL_0026:  br.s       IL_0031

    IL_0028:  nop
    IL_0029:  ldarg.0
    IL_002a:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_002f:  nop
    IL_0030:  nop
    IL_0031:  ldarg.0
    IL_0032:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_0037:  nop
    IL_0038:  ret
  } // end of method ShortCircuit::StmtComplex2

  .method public hidebysig instance void 
          StmtComplex3(int32 i) cil managed
  {
    // Code size       70 (0x46)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4     0x3e8
    IL_0007:  bgt.s      IL_0025

    IL_0009:  ldarg.1
    IL_000a:  ldc.i4.1
    IL_000b:  blt.s      IL_0011

    IL_000d:  ldarg.1
    IL_000e:  ldc.i4.8
    IL_000f:  ble.s      IL_0025

    IL_0011:  ldarg.1
    IL_0012:  ldc.i4.s   100
    IL_0014:  blt.s      IL_001e

    IL_0016:  ldarg.1
    IL_0017:  ldc.i4     0xc8
    IL_001c:  ble.s      IL_0025

    IL_001e:  ldarg.1
    IL_001f:  ldc.i4.s   42
    IL_0021:  ceq
    IL_0023:  br.s       IL_0026

    IL_0025:  ldc.i4.1
    IL_0026:  stloc.0
    IL_0027:  ldloc.0
    IL_0028:  brfalse.s  IL_0035

    IL_002a:  nop
    IL_002b:  ldarg.0
    IL_002c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0031:  nop
    IL_0032:  nop
    IL_0033:  br.s       IL_003e

    IL_0035:  nop
    IL_0036:  ldarg.0
    IL_0037:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_003c:  nop
    IL_003d:  nop
    IL_003e:  ldarg.0
    IL_003f:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_0044:  nop
    IL_0045:  ret
  } // end of method ShortCircuit::StmtComplex3

  .method public hidebysig instance void 
          StmtComplex4(int32 i) cil managed
  {
    // Code size       62 (0x3e)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4     0x3e8
    IL_0007:  bgt.s      IL_001d

    IL_0009:  ldarg.1
    IL_000a:  ldc.i4.1
    IL_000b:  blt.s      IL_0011

    IL_000d:  ldarg.1
    IL_000e:  ldc.i4.8
    IL_000f:  ble.s      IL_001d

    IL_0011:  ldarg.1
    IL_0012:  ldc.i4.s   42
    IL_0014:  beq.s      IL_001d

    IL_0016:  ldarg.1
    IL_0017:  ldc.i4.s   23
    IL_0019:  ceq
    IL_001b:  br.s       IL_001e

    IL_001d:  ldc.i4.1
    IL_001e:  stloc.0
    IL_001f:  ldloc.0
    IL_0020:  brfalse.s  IL_002d

    IL_0022:  nop
    IL_0023:  ldarg.0
    IL_0024:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0029:  nop
    IL_002a:  nop
    IL_002b:  br.s       IL_0036

    IL_002d:  nop
    IL_002e:  ldarg.0
    IL_002f:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_0034:  nop
    IL_0035:  nop
    IL_0036:  ldarg.0
    IL_0037:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_003c:  nop
    IL_003d:  ret
  } // end of method ShortCircuit::StmtComplex4

  .method public hidebysig instance void 
          StmtComplex5() cil managed
  {
    // Code size       89 (0x59)
    .maxstack  2
    .locals init (bool V_0,
             bool V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  brfalse.s  IL_002d

    IL_000c:  nop
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.1
    IL_000f:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0014:  brtrue.s   IL_0022

    IL_0016:  ldarg.0
    IL_0017:  ldc.i4.2
    IL_0018:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001d:  ldc.i4.0
    IL_001e:  ceq
    IL_0020:  br.s       IL_0023

    IL_0022:  ldc.i4.0
    IL_0023:  stloc.1
    IL_0024:  ldloc.1
    IL_0025:  brfalse.s  IL_002a

    IL_0027:  nop
    IL_0028:  br.s       IL_0058

    IL_002a:  nop
    IL_002b:  br.s       IL_0051

    IL_002d:  ldarg.0
    IL_002e:  ldc.i4.3
    IL_002f:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0034:  brfalse.s  IL_0042

    IL_0036:  ldarg.0
    IL_0037:  ldc.i4.4
    IL_0038:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_003d:  ldc.i4.0
    IL_003e:  ceq
    IL_0040:  br.s       IL_0043

    IL_0042:  ldc.i4.1
    IL_0043:  stloc.2
    IL_0044:  ldloc.2
    IL_0045:  brfalse.s  IL_0051

    IL_0047:  nop
    IL_0048:  ldarg.0
    IL_0049:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M2()
    IL_004e:  nop
    IL_004f:  br.s       IL_0058

    IL_0051:  ldarg.0
    IL_0052:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_0057:  nop
    IL_0058:  ret
  } // end of method ShortCircuit::StmtComplex5

  .method public hidebysig instance int32 
          StmtComplex6() cil managed
  {
    // Code size       55 (0x37)
    .maxstack  2
    .locals init (bool V_0,
             bool V_1,
             int32 V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  brfalse.s  IL_0031

    IL_000c:  nop
    IL_000d:  ldarg.0
    IL_000e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::M1()
    IL_0013:  nop
    IL_0014:  ldarg.0
    IL_0015:  ldc.i4.1
    IL_0016:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_001b:  brtrue.s   IL_0026

    IL_001d:  ldarg.0
    IL_001e:  ldc.i4.2
    IL_001f:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0024:  br.s       IL_0027

    IL_0026:  ldc.i4.1
    IL_0027:  stloc.1
    IL_0028:  ldloc.1
    IL_0029:  brfalse.s  IL_0030

    IL_002b:  nop
    IL_002c:  ldc.i4.1
    IL_002d:  stloc.2
    IL_002e:  br.s       IL_0035

    IL_0030:  nop
    IL_0031:  ldc.i4.2
    IL_0032:  stloc.2
    IL_0033:  br.s       IL_0035

    IL_0035:  ldloc.2
    IL_0036:  ret
  } // end of method ShortCircuit::StmtComplex6

  .method public hidebysig instance int32 
          InferCorrectOrder() cil managed
  {
    // Code size       35 (0x23)
    .maxstack  2
    .locals init (bool V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.1
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  brtrue.s   IL_0013

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.2
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0011:  br.s       IL_0014

    IL_0013:  ldc.i4.1
    IL_0014:  stloc.0
    IL_0015:  ldloc.0
    IL_0016:  brfalse.s  IL_001d

    IL_0018:  nop
    IL_0019:  ldc.i4.1
    IL_001a:  stloc.1
    IL_001b:  br.s       IL_0021

    IL_001d:  ldc.i4.2
    IL_001e:  stloc.1
    IL_001f:  br.s       IL_0021

    IL_0021:  ldloc.1
    IL_0022:  ret
  } // end of method ShortCircuit::InferCorrectOrder

  .method public hidebysig instance void 
          EmptyIf() cil managed
  {
    // Code size       145 (0x91)
    .maxstack  2
    .locals init (bool V_0,
             bool V_1,
             bool V_2,
             bool V_3,
             bool V_4)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  brfalse.s  IL_000e

    IL_000c:  nop
    IL_000d:  nop
    IL_000e:  ldarg.0
    IL_000f:  ldc.i4.1
    IL_0010:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0015:  ldc.i4.0
    IL_0016:  ceq
    IL_0018:  stloc.1
    IL_0019:  ldloc.1
    IL_001a:  brfalse.s  IL_001e

    IL_001c:  nop
    IL_001d:  nop
    IL_001e:  ldarg.0
    IL_001f:  ldc.i4.2
    IL_0020:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0025:  brfalse.s  IL_0030

    IL_0027:  ldarg.0
    IL_0028:  ldc.i4.3
    IL_0029:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_002e:  br.s       IL_0031

    IL_0030:  ldc.i4.0
    IL_0031:  stloc.2
    IL_0032:  ldloc.2
    IL_0033:  brfalse.s  IL_0037

    IL_0035:  nop
    IL_0036:  nop
    IL_0037:  ldarg.0
    IL_0038:  ldc.i4.4
    IL_0039:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_003e:  brtrue.s   IL_0049

    IL_0040:  ldarg.0
    IL_0041:  ldc.i4.5
    IL_0042:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0047:  br.s       IL_004a

    IL_0049:  ldc.i4.1
    IL_004a:  stloc.3
    IL_004b:  ldloc.3
    IL_004c:  brfalse.s  IL_0050

    IL_004e:  nop
    IL_004f:  nop
    IL_0050:  ldarg.0
    IL_0051:  ldc.i4.0
    IL_0052:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0057:  brfalse.s  IL_0080

    IL_0059:  ldarg.0
    IL_005a:  ldc.i4.1
    IL_005b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0060:  brfalse.s  IL_0080

    IL_0062:  ldarg.0
    IL_0063:  ldc.i4.2
    IL_0064:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0069:  brtrue.s   IL_0080

    IL_006b:  ldarg.0
    IL_006c:  ldc.i4.3
    IL_006d:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_0072:  brtrue.s   IL_007d

    IL_0074:  ldarg.0
    IL_0075:  ldc.i4.4
    IL_0076:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::F(int32)
    IL_007b:  br.s       IL_007e

    IL_007d:  ldc.i4.1
    IL_007e:  br.s       IL_0081

    IL_0080:  ldc.i4.0
    IL_0081:  stloc.s    V_4
    IL_0083:  ldloc.s    V_4
    IL_0085:  brfalse.s  IL_0089

    IL_0087:  nop
    IL_0088:  nop
    IL_0089:  ldarg.0
    IL_008a:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::E()
    IL_008f:  nop
    IL_0090:  ret
  } // end of method ShortCircuit::EmptyIf

  .method public hidebysig instance void 
          PreferLogicalToBitwise(bool a,
                                 bool b,
                                 int32 i,
                                 float32 f) cil managed
  {
    // Code size       75 (0x4b)
    .maxstack  4
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.1
    IL_0003:  ldarg.2
    IL_0004:  and
    IL_0005:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_000a:  nop
    IL_000b:  ldarg.0
    IL_000c:  ldarg.1
    IL_000d:  brfalse.s  IL_0015

    IL_000f:  ldarg.3
    IL_0010:  ldc.i4.1
    IL_0011:  ceq
    IL_0013:  br.s       IL_0016

    IL_0015:  ldc.i4.0
    IL_0016:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_001b:  nop
    IL_001c:  ldarg.0
    IL_001d:  ldarg.3
    IL_001e:  ldc.i4.1
    IL_001f:  ceq
    IL_0021:  ldarg.1
    IL_0022:  and
    IL_0023:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_0028:  nop
    IL_0029:  ldarg.0
    IL_002a:  ldarg.3
    IL_002b:  ldarg.3
    IL_002c:  ldc.i4.3
    IL_002d:  sub
    IL_002e:  cgt
    IL_0030:  ldarg.1
    IL_0031:  and
    IL_0032:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_0037:  nop
    IL_0038:  ldarg.0
    IL_0039:  ldarg.s    f
    IL_003b:  ldc.r4     0.1
    IL_0040:  clt
    IL_0042:  ldarg.1
    IL_0043:  and
    IL_0044:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit::B(bool)
    IL_0049:  nop
    IL_004a:  ret
  } // end of method ShortCircuit::PreferLogicalToBitwise

  .method family hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method ShortCircuit::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ShortCircuit


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
