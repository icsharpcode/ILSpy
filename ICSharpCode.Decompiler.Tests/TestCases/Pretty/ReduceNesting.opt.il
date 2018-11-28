



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly ReduceNesting.opt
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module ReduceNesting.opt.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting
       extends [mscorlib]System.Object
{
  .method public hidebysig newslot abstract virtual 
          instance bool  B(int32 i) cil managed
  {
  } // end of method ReduceNesting::B

  .method public hidebysig newslot abstract virtual 
          instance int32  I(int32 i) cil managed
  {
  } // end of method ReduceNesting::I

  .method public hidebysig instance void 
          IfIf() cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0007:  brfalse.s  IL_0010

    IL_0009:  ldc.i4.0
    IL_000a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000f:  ret

    IL_0010:  ldarg.0
    IL_0011:  ldc.i4.1
    IL_0012:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0017:  brfalse.s  IL_001f

    IL_0019:  ldc.i4.1
    IL_001a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001f:  ldstr      "end"
    IL_0024:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0029:  ret
  } // end of method ReduceNesting::IfIf

  .method public hidebysig instance void 
          IfSwitch() cil managed
  {
    // Code size       83 (0x53)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0007:  brfalse.s  IL_0010

    IL_0009:  ldc.i4.0
    IL_000a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000f:  ret

    IL_0010:  ldstr      "switch"
    IL_0015:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001a:  ldarg.0
    IL_001b:  ldc.i4.0
    IL_001c:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::I(int32)
    IL_0021:  stloc.0
    IL_0022:  ldloc.0
    IL_0023:  switch     ( 
                          IL_0032,
                          IL_003d)
    IL_0030:  br.s       IL_0048

    IL_0032:  ldstr      "case 0"
    IL_0037:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003c:  ret

    IL_003d:  ldstr      "case 1"
    IL_0042:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0047:  ret

    IL_0048:  ldstr      "end"
    IL_004d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0052:  ret
  } // end of method ReduceNesting::IfSwitch

  .method public hidebysig instance void 
          IfSwitchSwitch() cil managed
  {
    // Code size       139 (0x8b)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0007:  brfalse.s  IL_0010

    IL_0009:  ldc.i4.0
    IL_000a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000f:  ret

    IL_0010:  ldstr      "switch 0"
    IL_0015:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001a:  ldarg.0
    IL_001b:  ldc.i4.1
    IL_001c:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::I(int32)
    IL_0021:  stloc.0
    IL_0022:  ldloc.0
    IL_0023:  switch     ( 
                          IL_0032,
                          IL_003d)
    IL_0030:  br.s       IL_0048

    IL_0032:  ldstr      "case 0"
    IL_0037:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003c:  ret

    IL_003d:  ldstr      "case 1"
    IL_0042:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0047:  ret

    IL_0048:  ldstr      "switch 1"
    IL_004d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0052:  ldarg.0
    IL_0053:  ldc.i4.1
    IL_0054:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::I(int32)
    IL_0059:  stloc.1
    IL_005a:  ldloc.1
    IL_005b:  switch     ( 
                          IL_006a,
                          IL_0075)
    IL_0068:  br.s       IL_0080

    IL_006a:  ldstr      "case 0"
    IL_006f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0074:  ret

    IL_0075:  ldstr      "case 1"
    IL_007a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007f:  ret

    IL_0080:  ldstr      "end"
    IL_0085:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_008a:  ret
  } // end of method ReduceNesting::IfSwitchSwitch

  .method public hidebysig instance void 
          IfLoop() cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0007:  brfalse.s  IL_0010

    IL_0009:  ldc.i4.0
    IL_000a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000f:  ret

    IL_0010:  ldc.i4.0
    IL_0011:  stloc.0
    IL_0012:  br.s       IL_001e

    IL_0014:  ldloc.0
    IL_0015:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.1
    IL_001c:  add
    IL_001d:  stloc.0
    IL_001e:  ldloc.0
    IL_001f:  ldc.i4.s   10
    IL_0021:  blt.s      IL_0014

    IL_0023:  ldstr      "end"
    IL_0028:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002d:  ret
  } // end of method ReduceNesting::IfLoop

  .method public hidebysig instance void 
          LoopContinue() cil managed
  {
    // Code size       62 (0x3e)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0038

    IL_0004:  ldloc.0
    IL_0005:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.0
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0011:  brfalse.s  IL_001b

    IL_0013:  ldc.i4.0
    IL_0014:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0019:  br.s       IL_0034

    IL_001b:  ldarg.0
    IL_001c:  ldc.i4.1
    IL_001d:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0022:  brfalse.s  IL_002a

    IL_0024:  ldc.i4.1
    IL_0025:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_002a:  ldstr      "loop-tail"
    IL_002f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0034:  ldloc.0
    IL_0035:  ldc.i4.1
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ldc.i4.s   10
    IL_003b:  blt.s      IL_0004

    IL_003d:  ret
  } // end of method ReduceNesting::LoopContinue

  .method public hidebysig instance void 
          LoopBreak() cil managed
  {
    // Code size       91 (0x5b)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_004b

    IL_0004:  ldloc.0
    IL_0005:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.0
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0011:  brfalse.s  IL_001b

    IL_0013:  ldc.i4.0
    IL_0014:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0019:  br.s       IL_0047

    IL_001b:  ldarg.0
    IL_001c:  ldc.i4.1
    IL_001d:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0022:  brfalse.s  IL_002c

    IL_0024:  ldc.i4.1
    IL_0025:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_002a:  br.s       IL_0050

    IL_002c:  ldarg.0
    IL_002d:  ldc.i4.2
    IL_002e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0033:  brfalse.s  IL_003b

    IL_0035:  ldc.i4.2
    IL_0036:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_003b:  ldstr      "break"
    IL_0040:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0045:  br.s       IL_0050

    IL_0047:  ldloc.0
    IL_0048:  ldc.i4.1
    IL_0049:  add
    IL_004a:  stloc.0
    IL_004b:  ldloc.0
    IL_004c:  ldc.i4.s   10
    IL_004e:  blt.s      IL_0004

    IL_0050:  ldstr      "end"
    IL_0055:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005a:  ret
  } // end of method ReduceNesting::LoopBreak

  .method public hidebysig instance void 
          LoopBreakElseIf() cil managed
  {
    // Code size       81 (0x51)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0041

    IL_0004:  ldloc.0
    IL_0005:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.0
    IL_000c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0011:  brfalse.s  IL_001b

    IL_0013:  ldc.i4.0
    IL_0014:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0019:  br.s       IL_003d

    IL_001b:  ldarg.0
    IL_001c:  ldc.i4.1
    IL_001d:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0022:  brfalse.s  IL_002c

    IL_0024:  ldc.i4.1
    IL_0025:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_002a:  br.s       IL_0046

    IL_002c:  ldarg.0
    IL_002d:  ldc.i4.2
    IL_002e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0033:  brfalse.s  IL_0046

    IL_0035:  ldc.i4.2
    IL_0036:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_003b:  br.s       IL_0046

    IL_003d:  ldloc.0
    IL_003e:  ldc.i4.1
    IL_003f:  add
    IL_0040:  stloc.0
    IL_0041:  ldloc.0
    IL_0042:  ldc.i4.s   10
    IL_0044:  blt.s      IL_0004

    IL_0046:  ldstr      "end"
    IL_004b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0050:  ret
  } // end of method ReduceNesting::LoopBreakElseIf

  .method public hidebysig instance void 
          SwitchIf() cil managed
  {
    // Code size       72 (0x48)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::I(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  switch     ( 
                          IL_0018,
                          IL_0023)
    IL_0016:  br.s       IL_002e

    IL_0018:  ldstr      "case 0"
    IL_001d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0022:  ret

    IL_0023:  ldstr      "case 1"
    IL_0028:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002d:  ret

    IL_002e:  ldarg.0
    IL_002f:  ldc.i4.0
    IL_0030:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0035:  brfalse.s  IL_003d

    IL_0037:  ldc.i4.0
    IL_0038:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_003d:  ldstr      "end"
    IL_0042:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0047:  ret
  } // end of method ReduceNesting::SwitchIf

  .method public hidebysig instance void 
          NestedSwitchIf() cil managed
  {
    // Code size       82 (0x52)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0007:  brfalse.s  IL_0047

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.0
    IL_000b:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::I(int32)
    IL_0010:  stloc.0
    IL_0011:  ldloc.0
    IL_0012:  switch     ( 
                          IL_0021,
                          IL_002c)
    IL_001f:  br.s       IL_0037

    IL_0021:  ldstr      "case 0"
    IL_0026:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002b:  ret

    IL_002c:  ldstr      "case 1"
    IL_0031:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0036:  ret

    IL_0037:  ldarg.0
    IL_0038:  ldc.i4.1
    IL_0039:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_003e:  brfalse.s  IL_0051

    IL_0040:  ldc.i4.1
    IL_0041:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0046:  ret

    IL_0047:  ldstr      "else"
    IL_004c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0051:  ret
  } // end of method ReduceNesting::NestedSwitchIf

  .method public hidebysig instance void 
          EarlyExit1() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0007:  brtrue.s   IL_0026

    IL_0009:  ldc.i4.0
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_0017

    IL_000d:  ldloc.0
    IL_000e:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0013:  ldloc.0
    IL_0014:  ldc.i4.1
    IL_0015:  add
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4.s   10
    IL_001a:  blt.s      IL_000d

    IL_001c:  ldstr      "end"
    IL_0021:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0026:  ret
  } // end of method ReduceNesting::EarlyExit1

  .method public hidebysig instance void 
          EarlyExit2() cil managed
  {
    // Code size       55 (0x37)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0007:  brfalse.s  IL_000a

    IL_0009:  ret

    IL_000a:  ldc.i4.0
    IL_000b:  stloc.0
    IL_000c:  br.s       IL_0027

    IL_000e:  ldloc.0
    IL_000f:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.2
    IL_0016:  rem
    IL_0017:  brtrue.s   IL_0023

    IL_0019:  ldstr      "even"
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0023:  ldloc.0
    IL_0024:  ldc.i4.1
    IL_0025:  add
    IL_0026:  stloc.0
    IL_0027:  ldloc.0
    IL_0028:  ldc.i4.s   10
    IL_002a:  blt.s      IL_000e

    IL_002c:  ldstr      "end"
    IL_0031:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0036:  ret
  } // end of method ReduceNesting::EarlyExit2

  .method public hidebysig instance void 
          BalancedIf() cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0007:  brfalse.s  IL_0023

    IL_0009:  ldstr      "true"
    IL_000e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.1
    IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_001a:  brfalse.s  IL_003c

    IL_001c:  ldc.i4.1
    IL_001d:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0022:  ret

    IL_0023:  ldarg.0
    IL_0024:  ldc.i4.2
    IL_0025:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_002a:  brfalse.s  IL_0032

    IL_002c:  ldc.i4.2
    IL_002d:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0032:  ldstr      "false"
    IL_0037:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003c:  ret
  } // end of method ReduceNesting::BalancedIf

  .method public hidebysig instance string 
          ComplexCase1(string s) cil managed
  {
    // Code size       193 (0xc1)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0007:  brfalse.s  IL_000b

    IL_0009:  ldarg.1
    IL_000a:  ret

    IL_000b:  ldc.i4.0
    IL_000c:  stloc.0
    IL_000d:  br         IL_00b3

    IL_0012:  ldarg.0
    IL_0013:  ldc.i4.1
    IL_0014:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0019:  brfalse.s  IL_0026

    IL_001b:  ldc.i4.1
    IL_001c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0021:  br         IL_00af

    IL_0026:  ldarg.0
    IL_0027:  ldc.i4.2
    IL_0028:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_002d:  brfalse.s  IL_0085

    IL_002f:  ldloc.0
    IL_0030:  stloc.1
    IL_0031:  ldloc.1
    IL_0032:  ldc.i4.1
    IL_0033:  sub
    IL_0034:  switch     ( 
                          IL_0047,
                          IL_0073,
                          IL_0073)
    IL_0045:  br.s       IL_007d

    IL_0047:  ldarg.0
    IL_0048:  ldc.i4.3
    IL_0049:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_004e:  brfalse.s  IL_0058

    IL_0050:  ldc.i4.3
    IL_0051:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0056:  br.s       IL_007d

    IL_0058:  ldstr      "case1"
    IL_005d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0062:  ldarg.0
    IL_0063:  ldc.i4.4
    IL_0064:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0069:  brfalse.s  IL_007d

    IL_006b:  ldc.i4.4
    IL_006c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0071:  br.s       IL_007d

    IL_0073:  ldstr      "case23"
    IL_0078:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007d:  ldc.i4.2
    IL_007e:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0083:  br.s       IL_00af

    IL_0085:  ldarg.0
    IL_0086:  ldc.i4.5
    IL_0087:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_008c:  brfalse.s  IL_0096

    IL_008e:  ldc.i4.5
    IL_008f:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0094:  br.s       IL_00af

    IL_0096:  ldarg.0
    IL_0097:  ldc.i4.6
    IL_0098:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_009d:  brfalse.s  IL_00a5

    IL_009f:  ldc.i4.6
    IL_00a0:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_00a5:  ldstr      "else"
    IL_00aa:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00af:  ldloc.0
    IL_00b0:  ldc.i4.1
    IL_00b1:  add
    IL_00b2:  stloc.0
    IL_00b3:  ldloc.0
    IL_00b4:  ldarg.1
    IL_00b5:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_00ba:  blt        IL_0012

    IL_00bf:  ldarg.1
    IL_00c0:  ret
  } // end of method ReduceNesting::ComplexCase1

  .method family hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method ReduceNesting::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
