



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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

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
    // Code size       54 (0x36)
    .maxstack  2
    .locals init (bool V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  brfalse.s  IL_0016

    IL_000c:  nop
    IL_000d:  ldc.i4.0
    IL_000e:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0013:  nop
    IL_0014:  br.s       IL_0035

    IL_0016:  ldarg.0
    IL_0017:  ldc.i4.1
    IL_0018:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_001d:  stloc.1
    IL_001e:  ldloc.1
    IL_001f:  brfalse.s  IL_002a

    IL_0021:  nop
    IL_0022:  ldc.i4.1
    IL_0023:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0028:  nop
    IL_0029:  nop
    IL_002a:  ldstr      "end"
    IL_002f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0034:  nop
    IL_0035:  ret
  } // end of method ReduceNesting::IfIf

  .method public hidebysig instance void 
          IfSwitch() cil managed
  {
    // Code size       92 (0x5c)
    .maxstack  2
    .locals init (bool V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  brfalse.s  IL_0016

    IL_000c:  nop
    IL_000d:  ldc.i4.0
    IL_000e:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0013:  nop
    IL_0014:  br.s       IL_005b

    IL_0016:  ldstr      "switch"
    IL_001b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0020:  nop
    IL_0021:  ldarg.0
    IL_0022:  ldc.i4.0
    IL_0023:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::I(int32)
    IL_0028:  stloc.1
    IL_0029:  ldloc.1
    IL_002a:  brfalse.s  IL_0034

    IL_002c:  br.s       IL_002e

    IL_002e:  ldloc.1
    IL_002f:  ldc.i4.1
    IL_0030:  beq.s      IL_0041

    IL_0032:  br.s       IL_004e

    IL_0034:  ldstr      "case 0"
    IL_0039:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003e:  nop
    IL_003f:  br.s       IL_005b

    IL_0041:  ldstr      "case 1"
    IL_0046:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004b:  nop
    IL_004c:  br.s       IL_005b

    IL_004e:  ldstr      "end"
    IL_0053:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0058:  nop
    IL_0059:  br.s       IL_005b

    IL_005b:  ret
  } // end of method ReduceNesting::IfSwitch

  .method public hidebysig instance void 
          IfSwitchSwitch() cil managed
  {
    // Code size       148 (0x94)
    .maxstack  2
    .locals init (bool V_0,
             int32 V_1,
             int32 V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  brfalse.s  IL_0016

    IL_000c:  nop
    IL_000d:  ldc.i4.0
    IL_000e:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0013:  nop
    IL_0014:  br.s       IL_0093

    IL_0016:  ldstr      "switch 0"
    IL_001b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0020:  nop
    IL_0021:  ldarg.0
    IL_0022:  ldc.i4.1
    IL_0023:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::I(int32)
    IL_0028:  stloc.1
    IL_0029:  ldloc.1
    IL_002a:  brfalse.s  IL_0034

    IL_002c:  br.s       IL_002e

    IL_002e:  ldloc.1
    IL_002f:  ldc.i4.1
    IL_0030:  beq.s      IL_0041

    IL_0032:  br.s       IL_004e

    IL_0034:  ldstr      "case 0"
    IL_0039:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003e:  nop
    IL_003f:  br.s       IL_0093

    IL_0041:  ldstr      "case 1"
    IL_0046:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004b:  nop
    IL_004c:  br.s       IL_0093

    IL_004e:  ldstr      "switch 1"
    IL_0053:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0058:  nop
    IL_0059:  ldarg.0
    IL_005a:  ldc.i4.1
    IL_005b:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::I(int32)
    IL_0060:  stloc.2
    IL_0061:  ldloc.2
    IL_0062:  brfalse.s  IL_006c

    IL_0064:  br.s       IL_0066

    IL_0066:  ldloc.2
    IL_0067:  ldc.i4.1
    IL_0068:  beq.s      IL_0079

    IL_006a:  br.s       IL_0086

    IL_006c:  ldstr      "case 0"
    IL_0071:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0076:  nop
    IL_0077:  br.s       IL_0093

    IL_0079:  ldstr      "case 1"
    IL_007e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0083:  nop
    IL_0084:  br.s       IL_0093

    IL_0086:  ldstr      "end"
    IL_008b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0090:  nop
    IL_0091:  br.s       IL_0093

    IL_0093:  ret
  } // end of method ReduceNesting::IfSwitchSwitch

  .method public hidebysig instance void 
          IfLoop() cil managed
  {
    // Code size       60 (0x3c)
    .maxstack  2
    .locals init (bool V_0,
             int32 V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  brfalse.s  IL_0016

    IL_000c:  nop
    IL_000d:  ldc.i4.0
    IL_000e:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0013:  nop
    IL_0014:  br.s       IL_003b

    IL_0016:  ldc.i4.0
    IL_0017:  stloc.1
    IL_0018:  br.s       IL_0027

    IL_001a:  nop
    IL_001b:  ldloc.1
    IL_001c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0021:  nop
    IL_0022:  nop
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.1
    IL_0025:  add
    IL_0026:  stloc.1
    IL_0027:  ldloc.1
    IL_0028:  ldc.i4.s   10
    IL_002a:  clt
    IL_002c:  stloc.2
    IL_002d:  ldloc.2
    IL_002e:  brtrue.s   IL_001a

    IL_0030:  ldstr      "end"
    IL_0035:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003a:  nop
    IL_003b:  ret
  } // end of method ReduceNesting::IfLoop

  .method public hidebysig instance void 
          LoopContinue() cil managed
  {
    // Code size       80 (0x50)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1,
             bool V_2,
             bool V_3)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0046

    IL_0005:  nop
    IL_0006:  ldloc.0
    IL_0007:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000c:  nop
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.0
    IL_000f:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0014:  stloc.1
    IL_0015:  ldloc.1
    IL_0016:  brfalse.s  IL_0022

    IL_0018:  nop
    IL_0019:  ldc.i4.0
    IL_001a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001f:  nop
    IL_0020:  br.s       IL_0042

    IL_0022:  ldarg.0
    IL_0023:  ldc.i4.1
    IL_0024:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0029:  stloc.2
    IL_002a:  ldloc.2
    IL_002b:  brfalse.s  IL_0036

    IL_002d:  nop
    IL_002e:  ldc.i4.1
    IL_002f:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0034:  nop
    IL_0035:  nop
    IL_0036:  ldstr      "loop-tail"
    IL_003b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0040:  nop
    IL_0041:  nop
    IL_0042:  ldloc.0
    IL_0043:  ldc.i4.1
    IL_0044:  add
    IL_0045:  stloc.0
    IL_0046:  ldloc.0
    IL_0047:  ldc.i4.s   10
    IL_0049:  clt
    IL_004b:  stloc.3
    IL_004c:  ldloc.3
    IL_004d:  brtrue.s   IL_0005

    IL_004f:  ret
  } // end of method ReduceNesting::LoopContinue

  .method public hidebysig instance void 
          LoopBreak() cil managed
  {
    // Code size       115 (0x73)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1,
             bool V_2,
             bool V_3,
             bool V_4)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_005c

    IL_0005:  nop
    IL_0006:  ldloc.0
    IL_0007:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000c:  nop
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.0
    IL_000f:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0014:  stloc.1
    IL_0015:  ldloc.1
    IL_0016:  brfalse.s  IL_0022

    IL_0018:  nop
    IL_0019:  ldc.i4.0
    IL_001a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001f:  nop
    IL_0020:  br.s       IL_0058

    IL_0022:  ldarg.0
    IL_0023:  ldc.i4.1
    IL_0024:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0029:  stloc.2
    IL_002a:  ldloc.2
    IL_002b:  brfalse.s  IL_0037

    IL_002d:  nop
    IL_002e:  ldc.i4.1
    IL_002f:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0034:  nop
    IL_0035:  br.s       IL_0067

    IL_0037:  ldarg.0
    IL_0038:  ldc.i4.2
    IL_0039:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_003e:  stloc.3
    IL_003f:  ldloc.3
    IL_0040:  brfalse.s  IL_004b

    IL_0042:  nop
    IL_0043:  ldc.i4.2
    IL_0044:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0049:  nop
    IL_004a:  nop
    IL_004b:  ldstr      "break"
    IL_0050:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0055:  nop
    IL_0056:  br.s       IL_0067

    IL_0058:  ldloc.0
    IL_0059:  ldc.i4.1
    IL_005a:  add
    IL_005b:  stloc.0
    IL_005c:  ldloc.0
    IL_005d:  ldc.i4.s   10
    IL_005f:  clt
    IL_0061:  stloc.s    V_4
    IL_0063:  ldloc.s    V_4
    IL_0065:  brtrue.s   IL_0005

    IL_0067:  ldstr      "end"
    IL_006c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0071:  nop
    IL_0072:  ret
  } // end of method ReduceNesting::LoopBreak

  .method public hidebysig instance void 
          LoopBreakElseIf() cil managed
  {
    // Code size       105 (0x69)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1,
             bool V_2,
             bool V_3,
             bool V_4)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0052

    IL_0005:  nop
    IL_0006:  ldloc.0
    IL_0007:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000c:  nop
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.0
    IL_000f:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0014:  stloc.1
    IL_0015:  ldloc.1
    IL_0016:  brfalse.s  IL_0022

    IL_0018:  nop
    IL_0019:  ldc.i4.0
    IL_001a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001f:  nop
    IL_0020:  br.s       IL_004e

    IL_0022:  ldarg.0
    IL_0023:  ldc.i4.1
    IL_0024:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0029:  stloc.2
    IL_002a:  ldloc.2
    IL_002b:  brfalse.s  IL_0038

    IL_002d:  nop
    IL_002e:  ldc.i4.1
    IL_002f:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0034:  nop
    IL_0035:  nop
    IL_0036:  br.s       IL_004c

    IL_0038:  ldarg.0
    IL_0039:  ldc.i4.2
    IL_003a:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_003f:  stloc.3
    IL_0040:  ldloc.3
    IL_0041:  brfalse.s  IL_004c

    IL_0043:  nop
    IL_0044:  ldc.i4.2
    IL_0045:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_004a:  nop
    IL_004b:  nop
    IL_004c:  br.s       IL_005d

    IL_004e:  ldloc.0
    IL_004f:  ldc.i4.1
    IL_0050:  add
    IL_0051:  stloc.0
    IL_0052:  ldloc.0
    IL_0053:  ldc.i4.s   10
    IL_0055:  clt
    IL_0057:  stloc.s    V_4
    IL_0059:  ldloc.s    V_4
    IL_005b:  brtrue.s   IL_0005

    IL_005d:  ldstr      "end"
    IL_0062:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0067:  nop
    IL_0068:  ret
  } // end of method ReduceNesting::LoopBreakElseIf

  .method public hidebysig instance void 
          SwitchIf() cil managed
  {
    // Code size       78 (0x4e)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::I(int32)
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  brfalse.s  IL_0014

    IL_000c:  br.s       IL_000e

    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.1
    IL_0010:  beq.s      IL_0021

    IL_0012:  br.s       IL_002e

    IL_0014:  ldstr      "case 0"
    IL_0019:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001e:  nop
    IL_001f:  br.s       IL_004d

    IL_0021:  ldstr      "case 1"
    IL_0026:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002b:  nop
    IL_002c:  br.s       IL_004d

    IL_002e:  ldarg.0
    IL_002f:  ldc.i4.0
    IL_0030:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0035:  stloc.1
    IL_0036:  ldloc.1
    IL_0037:  brfalse.s  IL_0042

    IL_0039:  nop
    IL_003a:  ldc.i4.0
    IL_003b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0040:  nop
    IL_0041:  nop
    IL_0042:  ldstr      "end"
    IL_0047:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004c:  nop
    IL_004d:  ret
  } // end of method ReduceNesting::SwitchIf

  .method public hidebysig instance void 
          NestedSwitchIf() cil managed
  {
    // Code size       95 (0x5f)
    .maxstack  2
    .locals init (bool V_0,
             int32 V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  brfalse.s  IL_0051

    IL_000c:  nop
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.0
    IL_000f:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::I(int32)
    IL_0014:  stloc.1
    IL_0015:  ldloc.1
    IL_0016:  brfalse.s  IL_0020

    IL_0018:  br.s       IL_001a

    IL_001a:  ldloc.1
    IL_001b:  ldc.i4.1
    IL_001c:  beq.s      IL_002d

    IL_001e:  br.s       IL_003a

    IL_0020:  ldstr      "case 0"
    IL_0025:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002a:  nop
    IL_002b:  br.s       IL_005e

    IL_002d:  ldstr      "case 1"
    IL_0032:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0037:  nop
    IL_0038:  br.s       IL_005e

    IL_003a:  ldarg.0
    IL_003b:  ldc.i4.1
    IL_003c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0041:  stloc.2
    IL_0042:  ldloc.2
    IL_0043:  brfalse.s  IL_004e

    IL_0045:  nop
    IL_0046:  ldc.i4.1
    IL_0047:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_004c:  nop
    IL_004d:  nop
    IL_004e:  nop
    IL_004f:  br.s       IL_005e

    IL_0051:  nop
    IL_0052:  ldstr      "else"
    IL_0057:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005c:  nop
    IL_005d:  nop
    IL_005e:  ret
  } // end of method ReduceNesting::NestedSwitchIf

  .method public hidebysig instance void 
          EarlyExit1() cil managed
  {
    // Code size       55 (0x37)
    .maxstack  2
    .locals init (bool V_0,
             int32 V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0008:  ldc.i4.0
    IL_0009:  ceq
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  brfalse.s  IL_0036

    IL_000f:  nop
    IL_0010:  ldc.i4.0
    IL_0011:  stloc.1
    IL_0012:  br.s       IL_0021

    IL_0014:  nop
    IL_0015:  ldloc.1
    IL_0016:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001b:  nop
    IL_001c:  nop
    IL_001d:  ldloc.1
    IL_001e:  ldc.i4.1
    IL_001f:  add
    IL_0020:  stloc.1
    IL_0021:  ldloc.1
    IL_0022:  ldc.i4.s   10
    IL_0024:  clt
    IL_0026:  stloc.2
    IL_0027:  ldloc.2
    IL_0028:  brtrue.s   IL_0014

    IL_002a:  ldstr      "end"
    IL_002f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0034:  nop
    IL_0035:  nop
    IL_0036:  ret
  } // end of method ReduceNesting::EarlyExit1

  .method public hidebysig instance void 
          EarlyExit2() cil managed
  {
    // Code size       76 (0x4c)
    .maxstack  2
    .locals init (bool V_0,
             int32 V_1,
             bool V_2,
             bool V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  brfalse.s  IL_000f

    IL_000c:  nop
    IL_000d:  br.s       IL_004b

    IL_000f:  ldc.i4.0
    IL_0010:  stloc.1
    IL_0011:  br.s       IL_0037

    IL_0013:  nop
    IL_0014:  ldloc.1
    IL_0015:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001a:  nop
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4.2
    IL_001d:  rem
    IL_001e:  ldc.i4.0
    IL_001f:  ceq
    IL_0021:  stloc.2
    IL_0022:  ldloc.2
    IL_0023:  brfalse.s  IL_0032

    IL_0025:  nop
    IL_0026:  ldstr      "even"
    IL_002b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0030:  nop
    IL_0031:  nop
    IL_0032:  nop
    IL_0033:  ldloc.1
    IL_0034:  ldc.i4.1
    IL_0035:  add
    IL_0036:  stloc.1
    IL_0037:  ldloc.1
    IL_0038:  ldc.i4.s   10
    IL_003a:  clt
    IL_003c:  stloc.3
    IL_003d:  ldloc.3
    IL_003e:  brtrue.s   IL_0013

    IL_0040:  ldstr      "end"
    IL_0045:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004a:  nop
    IL_004b:  ret
  } // end of method ReduceNesting::EarlyExit2

  .method public hidebysig instance void 
          BalancedIf() cil managed
  {
    // Code size       81 (0x51)
    .maxstack  2
    .locals init (bool V_0,
             bool V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  brfalse.s  IL_002f

    IL_000c:  nop
    IL_000d:  ldstr      "true"
    IL_0012:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0017:  nop
    IL_0018:  ldarg.0
    IL_0019:  ldc.i4.1
    IL_001a:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_001f:  stloc.1
    IL_0020:  ldloc.1
    IL_0021:  brfalse.s  IL_002c

    IL_0023:  nop
    IL_0024:  ldc.i4.1
    IL_0025:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_002a:  nop
    IL_002b:  nop
    IL_002c:  nop
    IL_002d:  br.s       IL_0050

    IL_002f:  nop
    IL_0030:  ldarg.0
    IL_0031:  ldc.i4.2
    IL_0032:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0037:  stloc.2
    IL_0038:  ldloc.2
    IL_0039:  brfalse.s  IL_0044

    IL_003b:  nop
    IL_003c:  ldc.i4.2
    IL_003d:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0042:  nop
    IL_0043:  nop
    IL_0044:  ldstr      "false"
    IL_0049:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004e:  nop
    IL_004f:  nop
    IL_0050:  ret
  } // end of method ReduceNesting::BalancedIf

  .method public hidebysig instance string 
          ComplexCase1(string s) cil managed
  {
    // Code size       255 (0xff)
    .maxstack  2
    .locals init (bool V_0,
             string V_1,
             int32 V_2,
             bool V_3,
             bool V_4,
             int32 V_5,
             bool V_6,
             bool V_7,
             bool V_8,
             bool V_9,
             bool V_10)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  brfalse.s  IL_0014

    IL_000c:  nop
    IL_000d:  ldarg.1
    IL_000e:  stloc.1
    IL_000f:  br         IL_00fd

    IL_0014:  ldc.i4.0
    IL_0015:  stloc.2
    IL_0016:  br         IL_00e7

    IL_001b:  nop
    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.1
    IL_001e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0023:  stloc.3
    IL_0024:  ldloc.3
    IL_0025:  brfalse.s  IL_0035

    IL_0027:  nop
    IL_0028:  ldc.i4.1
    IL_0029:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_002e:  nop
    IL_002f:  nop
    IL_0030:  br         IL_00e2

    IL_0035:  ldarg.0
    IL_0036:  ldc.i4.2
    IL_0037:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_003c:  stloc.s    V_4
    IL_003e:  ldloc.s    V_4
    IL_0040:  brfalse.s  IL_00a7

    IL_0042:  nop
    IL_0043:  ldloc.2
    IL_0044:  stloc.s    V_5
    IL_0046:  ldloc.s    V_5
    IL_0048:  ldc.i4.1
    IL_0049:  beq.s      IL_0056

    IL_004b:  br.s       IL_004d

    IL_004d:  ldloc.s    V_5
    IL_004f:  ldc.i4.2
    IL_0050:  sub
    IL_0051:  ldc.i4.1
    IL_0052:  ble.un.s   IL_0090

    IL_0054:  br.s       IL_009d

    IL_0056:  ldarg.0
    IL_0057:  ldc.i4.3
    IL_0058:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_005d:  stloc.s    V_6
    IL_005f:  ldloc.s    V_6
    IL_0061:  brfalse.s  IL_006d

    IL_0063:  nop
    IL_0064:  ldc.i4.3
    IL_0065:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_006a:  nop
    IL_006b:  br.s       IL_009d

    IL_006d:  ldstr      "case1"
    IL_0072:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0077:  nop
    IL_0078:  ldarg.0
    IL_0079:  ldc.i4.4
    IL_007a:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_007f:  stloc.s    V_7
    IL_0081:  ldloc.s    V_7
    IL_0083:  brfalse.s  IL_008e

    IL_0085:  nop
    IL_0086:  ldc.i4.4
    IL_0087:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_008c:  nop
    IL_008d:  nop
    IL_008e:  br.s       IL_009d

    IL_0090:  ldstr      "case23"
    IL_0095:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_009a:  nop
    IL_009b:  br.s       IL_009d

    IL_009d:  ldc.i4.2
    IL_009e:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_00a3:  nop
    IL_00a4:  nop
    IL_00a5:  br.s       IL_00e2

    IL_00a7:  ldarg.0
    IL_00a8:  ldc.i4.5
    IL_00a9:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_00ae:  stloc.s    V_8
    IL_00b0:  ldloc.s    V_8
    IL_00b2:  brfalse.s  IL_00bf

    IL_00b4:  nop
    IL_00b5:  ldc.i4.5
    IL_00b6:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_00bb:  nop
    IL_00bc:  nop
    IL_00bd:  br.s       IL_00e2

    IL_00bf:  nop
    IL_00c0:  ldarg.0
    IL_00c1:  ldc.i4.6
    IL_00c2:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_00c7:  stloc.s    V_9
    IL_00c9:  ldloc.s    V_9
    IL_00cb:  brfalse.s  IL_00d6

    IL_00cd:  nop
    IL_00ce:  ldc.i4.6
    IL_00cf:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_00d4:  nop
    IL_00d5:  nop
    IL_00d6:  ldstr      "else"
    IL_00db:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00e0:  nop
    IL_00e1:  nop
    IL_00e2:  nop
    IL_00e3:  ldloc.2
    IL_00e4:  ldc.i4.1
    IL_00e5:  add
    IL_00e6:  stloc.2
    IL_00e7:  ldloc.2
    IL_00e8:  ldarg.1
    IL_00e9:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_00ee:  clt
    IL_00f0:  stloc.s    V_10
    IL_00f2:  ldloc.s    V_10
    IL_00f4:  brtrue     IL_001b

    IL_00f9:  ldarg.1
    IL_00fa:  stloc.1
    IL_00fb:  br.s       IL_00fd

    IL_00fd:  ldloc.1
    IL_00fe:  ret
  } // end of method ReduceNesting::ComplexCase1

  .method family hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method ReduceNesting::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
