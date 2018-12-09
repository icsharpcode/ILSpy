



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly ReduceNesting
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
.module ReduceNesting.dll
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
    // Code size       76 (0x4c)
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
    IL_0023:  brfalse.s  IL_002b

    IL_0025:  ldloc.0
    IL_0026:  ldc.i4.1
    IL_0027:  beq.s      IL_0036

    IL_0029:  br.s       IL_0041

    IL_002b:  ldstr      "case 0"
    IL_0030:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0035:  ret

    IL_0036:  ldstr      "case 1"
    IL_003b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0040:  ret

    IL_0041:  ldstr      "end"
    IL_0046:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004b:  ret
  } // end of method ReduceNesting::IfSwitch

  .method public hidebysig instance void 
          IfSwitchSwitch() cil managed
  {
    // Code size       125 (0x7d)
    .maxstack  2
    .locals init (int32 V_0)
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
    IL_0023:  brfalse.s  IL_002b

    IL_0025:  ldloc.0
    IL_0026:  ldc.i4.1
    IL_0027:  beq.s      IL_0036

    IL_0029:  br.s       IL_0041

    IL_002b:  ldstr      "case 0"
    IL_0030:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0035:  ret

    IL_0036:  ldstr      "case 1"
    IL_003b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0040:  ret

    IL_0041:  ldstr      "switch 1"
    IL_0046:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004b:  ldarg.0
    IL_004c:  ldc.i4.1
    IL_004d:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::I(int32)
    IL_0052:  stloc.0
    IL_0053:  ldloc.0
    IL_0054:  brfalse.s  IL_005c

    IL_0056:  ldloc.0
    IL_0057:  ldc.i4.1
    IL_0058:  beq.s      IL_0067

    IL_005a:  br.s       IL_0072

    IL_005c:  ldstr      "case 0"
    IL_0061:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0066:  ret

    IL_0067:  ldstr      "case 1"
    IL_006c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0071:  ret

    IL_0072:  ldstr      "end"
    IL_0077:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007c:  ret
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
    // Code size       65 (0x41)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::I(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  brfalse.s  IL_0011

    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.1
    IL_000d:  beq.s      IL_001c

    IL_000f:  br.s       IL_0027

    IL_0011:  ldstr      "case 0"
    IL_0016:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001b:  ret

    IL_001c:  ldstr      "case 1"
    IL_0021:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0026:  ret

    IL_0027:  ldarg.0
    IL_0028:  ldc.i4.0
    IL_0029:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_002e:  brfalse.s  IL_0036

    IL_0030:  ldc.i4.0
    IL_0031:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0036:  ldstr      "end"
    IL_003b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0040:  ret
  } // end of method ReduceNesting::SwitchIf

  .method public hidebysig instance void 
          NestedSwitchIf() cil managed
  {
    // Code size       75 (0x4b)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0007:  brfalse.s  IL_0040

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.0
    IL_000b:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::I(int32)
    IL_0010:  stloc.0
    IL_0011:  ldloc.0
    IL_0012:  brfalse.s  IL_001a

    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.1
    IL_0016:  beq.s      IL_0025

    IL_0018:  br.s       IL_0030

    IL_001a:  ldstr      "case 0"
    IL_001f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0024:  ret

    IL_0025:  ldstr      "case 1"
    IL_002a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002f:  ret

    IL_0030:  ldarg.0
    IL_0031:  ldc.i4.1
    IL_0032:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0037:  brfalse.s  IL_004a

    IL_0039:  ldc.i4.1
    IL_003a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_003f:  ret

    IL_0040:  ldstr      "else"
    IL_0045:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004a:  ret
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
    // Code size       178 (0xb2)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0007:  brfalse.s  IL_000b

    IL_0009:  ldarg.1
    IL_000a:  ret

    IL_000b:  ldc.i4.0
    IL_000c:  stloc.0
    IL_000d:  br         IL_00a4

    IL_0012:  ldarg.0
    IL_0013:  ldc.i4.1
    IL_0014:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0019:  brfalse.s  IL_0023

    IL_001b:  ldc.i4.1
    IL_001c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0021:  br.s       IL_00a0

    IL_0023:  ldarg.0
    IL_0024:  ldc.i4.2
    IL_0025:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_002a:  brfalse.s  IL_0076

    IL_002c:  ldloc.0
    IL_002d:  ldc.i4.1
    IL_002e:  beq.s      IL_0038

    IL_0030:  ldloc.0
    IL_0031:  ldc.i4.2
    IL_0032:  sub
    IL_0033:  ldc.i4.1
    IL_0034:  ble.un.s   IL_0064

    IL_0036:  br.s       IL_006e

    IL_0038:  ldarg.0
    IL_0039:  ldc.i4.3
    IL_003a:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_003f:  brfalse.s  IL_0049

    IL_0041:  ldc.i4.3
    IL_0042:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0047:  br.s       IL_006e

    IL_0049:  ldstr      "case1"
    IL_004e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0053:  ldarg.0
    IL_0054:  ldc.i4.4
    IL_0055:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_005a:  brfalse.s  IL_006e

    IL_005c:  ldc.i4.4
    IL_005d:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0062:  br.s       IL_006e

    IL_0064:  ldstr      "case23"
    IL_0069:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006e:  ldc.i4.2
    IL_006f:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0074:  br.s       IL_00a0

    IL_0076:  ldarg.0
    IL_0077:  ldc.i4.5
    IL_0078:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_007d:  brfalse.s  IL_0087

    IL_007f:  ldc.i4.5
    IL_0080:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0085:  br.s       IL_00a0

    IL_0087:  ldarg.0
    IL_0088:  ldc.i4.6
    IL_0089:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_008e:  brfalse.s  IL_0096

    IL_0090:  ldc.i4.6
    IL_0091:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0096:  ldstr      "else"
    IL_009b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00a0:  ldloc.0
    IL_00a1:  ldc.i4.1
    IL_00a2:  add
    IL_00a3:  stloc.0
    IL_00a4:  ldloc.0
    IL_00a5:  ldarg.1
    IL_00a6:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_00ab:  blt        IL_0012

    IL_00b0:  ldarg.1
    IL_00b1:  ret
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
