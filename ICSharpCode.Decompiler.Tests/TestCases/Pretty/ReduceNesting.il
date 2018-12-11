



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
    // Code size       60 (0x3c)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0008:  ldc.i4.0
    IL_0009:  ceq
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  brtrue.s   IL_0019

    IL_000f:  nop
    IL_0010:  ldc.i4.0
    IL_0011:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0016:  nop
    IL_0017:  br.s       IL_003b

    IL_0019:  ldarg.0
    IL_001a:  ldc.i4.1
    IL_001b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0020:  ldc.i4.0
    IL_0021:  ceq
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  brtrue.s   IL_0030

    IL_0027:  nop
    IL_0028:  ldc.i4.1
    IL_0029:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_002e:  nop
    IL_002f:  nop
    IL_0030:  ldstr      "end"
    IL_0035:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003a:  nop
    IL_003b:  ret
  } // end of method ReduceNesting::IfIf

  .method public hidebysig instance void 
          IfSwitch() cil managed
  {
    // Code size       100 (0x64)
    .maxstack  2
    .locals init (bool V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0008:  ldc.i4.0
    IL_0009:  ceq
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  brtrue.s   IL_0019

    IL_000f:  nop
    IL_0010:  ldc.i4.0
    IL_0011:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0016:  nop
    IL_0017:  br.s       IL_0063

    IL_0019:  ldstr      "switch"
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0023:  nop
    IL_0024:  ldarg.0
    IL_0025:  ldc.i4.0
    IL_0026:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::I(int32)
    IL_002b:  stloc.1
    IL_002c:  ldloc.1
    IL_002d:  switch     ( 
                          IL_003c,
                          IL_0049)
    IL_003a:  br.s       IL_0056

    IL_003c:  ldstr      "case 0"
    IL_0041:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0046:  nop
    IL_0047:  br.s       IL_0063

    IL_0049:  ldstr      "case 1"
    IL_004e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0053:  nop
    IL_0054:  br.s       IL_0063

    IL_0056:  ldstr      "end"
    IL_005b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0060:  nop
    IL_0061:  br.s       IL_0063

    IL_0063:  ret
  } // end of method ReduceNesting::IfSwitch

  .method public hidebysig instance void 
          IfSwitchSwitch() cil managed
  {
    // Code size       164 (0xa4)
    .maxstack  2
    .locals init (bool V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0008:  ldc.i4.0
    IL_0009:  ceq
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  brtrue.s   IL_001c

    IL_000f:  nop
    IL_0010:  ldc.i4.0
    IL_0011:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0016:  nop
    IL_0017:  br         IL_00a3

    IL_001c:  ldstr      "switch 0"
    IL_0021:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0026:  nop
    IL_0027:  ldarg.0
    IL_0028:  ldc.i4.1
    IL_0029:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::I(int32)
    IL_002e:  stloc.1
    IL_002f:  ldloc.1
    IL_0030:  switch     ( 
                          IL_003f,
                          IL_004c)
    IL_003d:  br.s       IL_0059

    IL_003f:  ldstr      "case 0"
    IL_0044:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0049:  nop
    IL_004a:  br.s       IL_00a3

    IL_004c:  ldstr      "case 1"
    IL_0051:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0056:  nop
    IL_0057:  br.s       IL_00a3

    IL_0059:  ldstr      "switch 1"
    IL_005e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0063:  nop
    IL_0064:  ldarg.0
    IL_0065:  ldc.i4.1
    IL_0066:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::I(int32)
    IL_006b:  stloc.1
    IL_006c:  ldloc.1
    IL_006d:  switch     ( 
                          IL_007c,
                          IL_0089)
    IL_007a:  br.s       IL_0096

    IL_007c:  ldstr      "case 0"
    IL_0081:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0086:  nop
    IL_0087:  br.s       IL_00a3

    IL_0089:  ldstr      "case 1"
    IL_008e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0093:  nop
    IL_0094:  br.s       IL_00a3

    IL_0096:  ldstr      "end"
    IL_009b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00a0:  nop
    IL_00a1:  br.s       IL_00a3

    IL_00a3:  ret
  } // end of method ReduceNesting::IfSwitchSwitch

  .method public hidebysig instance void 
          IfLoop() cil managed
  {
    // Code size       63 (0x3f)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0008:  ldc.i4.0
    IL_0009:  ceq
    IL_000b:  stloc.1
    IL_000c:  ldloc.1
    IL_000d:  brtrue.s   IL_0019

    IL_000f:  nop
    IL_0010:  ldc.i4.0
    IL_0011:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0016:  nop
    IL_0017:  br.s       IL_003e

    IL_0019:  ldc.i4.0
    IL_001a:  stloc.0
    IL_001b:  br.s       IL_002a

    IL_001d:  nop
    IL_001e:  ldloc.0
    IL_001f:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0024:  nop
    IL_0025:  nop
    IL_0026:  ldloc.0
    IL_0027:  ldc.i4.1
    IL_0028:  add
    IL_0029:  stloc.0
    IL_002a:  ldloc.0
    IL_002b:  ldc.i4.s   10
    IL_002d:  clt
    IL_002f:  stloc.1
    IL_0030:  ldloc.1
    IL_0031:  brtrue.s   IL_001d

    IL_0033:  ldstr      "end"
    IL_0038:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003d:  nop
    IL_003e:  ret
  } // end of method ReduceNesting::IfLoop

  .method public hidebysig instance void 
          LoopContinue() cil managed
  {
    // Code size       86 (0x56)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_004c

    IL_0005:  nop
    IL_0006:  ldloc.0
    IL_0007:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000c:  nop
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.0
    IL_000f:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0014:  ldc.i4.0
    IL_0015:  ceq
    IL_0017:  stloc.1
    IL_0018:  ldloc.1
    IL_0019:  brtrue.s   IL_0025

    IL_001b:  nop
    IL_001c:  ldc.i4.0
    IL_001d:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0022:  nop
    IL_0023:  br.s       IL_0048

    IL_0025:  ldarg.0
    IL_0026:  ldc.i4.1
    IL_0027:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_002c:  ldc.i4.0
    IL_002d:  ceq
    IL_002f:  stloc.1
    IL_0030:  ldloc.1
    IL_0031:  brtrue.s   IL_003c

    IL_0033:  nop
    IL_0034:  ldc.i4.1
    IL_0035:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_003a:  nop
    IL_003b:  nop
    IL_003c:  ldstr      "loop-tail"
    IL_0041:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0046:  nop
    IL_0047:  nop
    IL_0048:  ldloc.0
    IL_0049:  ldc.i4.1
    IL_004a:  add
    IL_004b:  stloc.0
    IL_004c:  ldloc.0
    IL_004d:  ldc.i4.s   10
    IL_004f:  clt
    IL_0051:  stloc.1
    IL_0052:  ldloc.1
    IL_0053:  brtrue.s   IL_0005

    IL_0055:  ret
  } // end of method ReduceNesting::LoopContinue

  .method public hidebysig instance void 
          LoopBreak() cil managed
  {
    // Code size       122 (0x7a)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0065

    IL_0005:  nop
    IL_0006:  ldloc.0
    IL_0007:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000c:  nop
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.0
    IL_000f:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0014:  ldc.i4.0
    IL_0015:  ceq
    IL_0017:  stloc.1
    IL_0018:  ldloc.1
    IL_0019:  brtrue.s   IL_0025

    IL_001b:  nop
    IL_001c:  ldc.i4.0
    IL_001d:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0022:  nop
    IL_0023:  br.s       IL_0061

    IL_0025:  ldarg.0
    IL_0026:  ldc.i4.1
    IL_0027:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_002c:  ldc.i4.0
    IL_002d:  ceq
    IL_002f:  stloc.1
    IL_0030:  ldloc.1
    IL_0031:  brtrue.s   IL_003d

    IL_0033:  nop
    IL_0034:  ldc.i4.1
    IL_0035:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_003a:  nop
    IL_003b:  br.s       IL_006e

    IL_003d:  ldarg.0
    IL_003e:  ldc.i4.2
    IL_003f:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0044:  ldc.i4.0
    IL_0045:  ceq
    IL_0047:  stloc.1
    IL_0048:  ldloc.1
    IL_0049:  brtrue.s   IL_0054

    IL_004b:  nop
    IL_004c:  ldc.i4.2
    IL_004d:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0052:  nop
    IL_0053:  nop
    IL_0054:  ldstr      "break"
    IL_0059:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005e:  nop
    IL_005f:  br.s       IL_006e

    IL_0061:  ldloc.0
    IL_0062:  ldc.i4.1
    IL_0063:  add
    IL_0064:  stloc.0
    IL_0065:  ldloc.0
    IL_0066:  ldc.i4.s   10
    IL_0068:  clt
    IL_006a:  stloc.1
    IL_006b:  ldloc.1
    IL_006c:  brtrue.s   IL_0005

    IL_006e:  ldstr      "end"
    IL_0073:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0078:  nop
    IL_0079:  ret
  } // end of method ReduceNesting::LoopBreak

  .method public hidebysig instance void 
          LoopBreakElseIf() cil managed
  {
    // Code size       112 (0x70)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_005b

    IL_0005:  nop
    IL_0006:  ldloc.0
    IL_0007:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000c:  nop
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.0
    IL_000f:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0014:  ldc.i4.0
    IL_0015:  ceq
    IL_0017:  stloc.1
    IL_0018:  ldloc.1
    IL_0019:  brtrue.s   IL_0025

    IL_001b:  nop
    IL_001c:  ldc.i4.0
    IL_001d:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0022:  nop
    IL_0023:  br.s       IL_0057

    IL_0025:  ldarg.0
    IL_0026:  ldc.i4.1
    IL_0027:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_002c:  ldc.i4.0
    IL_002d:  ceq
    IL_002f:  stloc.1
    IL_0030:  ldloc.1
    IL_0031:  brtrue.s   IL_003e

    IL_0033:  nop
    IL_0034:  ldc.i4.1
    IL_0035:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_003a:  nop
    IL_003b:  nop
    IL_003c:  br.s       IL_0055

    IL_003e:  ldarg.0
    IL_003f:  ldc.i4.2
    IL_0040:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0045:  ldc.i4.0
    IL_0046:  ceq
    IL_0048:  stloc.1
    IL_0049:  ldloc.1
    IL_004a:  brtrue.s   IL_0055

    IL_004c:  nop
    IL_004d:  ldc.i4.2
    IL_004e:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0053:  nop
    IL_0054:  nop
    IL_0055:  br.s       IL_0064

    IL_0057:  ldloc.0
    IL_0058:  ldc.i4.1
    IL_0059:  add
    IL_005a:  stloc.0
    IL_005b:  ldloc.0
    IL_005c:  ldc.i4.s   10
    IL_005e:  clt
    IL_0060:  stloc.1
    IL_0061:  ldloc.1
    IL_0062:  brtrue.s   IL_0005

    IL_0064:  ldstr      "end"
    IL_0069:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006e:  nop
    IL_006f:  ret
  } // end of method ReduceNesting::LoopBreakElseIf

  .method public hidebysig instance void 
          SwitchIf() cil managed
  {
    // Code size       86 (0x56)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::I(int32)
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  switch     ( 
                          IL_0019,
                          IL_0026)
    IL_0017:  br.s       IL_0033

    IL_0019:  ldstr      "case 0"
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0023:  nop
    IL_0024:  br.s       IL_0055

    IL_0026:  ldstr      "case 1"
    IL_002b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0030:  nop
    IL_0031:  br.s       IL_0055

    IL_0033:  ldarg.0
    IL_0034:  ldc.i4.0
    IL_0035:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_003a:  ldc.i4.0
    IL_003b:  ceq
    IL_003d:  stloc.1
    IL_003e:  ldloc.1
    IL_003f:  brtrue.s   IL_004a

    IL_0041:  nop
    IL_0042:  ldc.i4.0
    IL_0043:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0048:  nop
    IL_0049:  nop
    IL_004a:  ldstr      "end"
    IL_004f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0054:  nop
    IL_0055:  ret
  } // end of method ReduceNesting::SwitchIf

  .method public hidebysig instance void 
          NestedSwitchIf() cil managed
  {
    // Code size       106 (0x6a)
    .maxstack  2
    .locals init (bool V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0008:  ldc.i4.0
    IL_0009:  ceq
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  brtrue.s   IL_005c

    IL_000f:  nop
    IL_0010:  ldarg.0
    IL_0011:  ldc.i4.0
    IL_0012:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::I(int32)
    IL_0017:  stloc.1
    IL_0018:  ldloc.1
    IL_0019:  switch     ( 
                          IL_0028,
                          IL_0035)
    IL_0026:  br.s       IL_0042

    IL_0028:  ldstr      "case 0"
    IL_002d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0032:  nop
    IL_0033:  br.s       IL_0069

    IL_0035:  ldstr      "case 1"
    IL_003a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003f:  nop
    IL_0040:  br.s       IL_0069

    IL_0042:  ldarg.0
    IL_0043:  ldc.i4.1
    IL_0044:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0049:  ldc.i4.0
    IL_004a:  ceq
    IL_004c:  stloc.0
    IL_004d:  ldloc.0
    IL_004e:  brtrue.s   IL_0059

    IL_0050:  nop
    IL_0051:  ldc.i4.1
    IL_0052:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0057:  nop
    IL_0058:  nop
    IL_0059:  nop
    IL_005a:  br.s       IL_0069

    IL_005c:  nop
    IL_005d:  ldstr      "else"
    IL_0062:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0067:  nop
    IL_0068:  nop
    IL_0069:  ret
  } // end of method ReduceNesting::NestedSwitchIf

  .method public hidebysig instance void 
          EarlyExit1() cil managed
  {
    // Code size       52 (0x34)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  brtrue.s   IL_0033

    IL_000c:  nop
    IL_000d:  ldc.i4.0
    IL_000e:  stloc.0
    IL_000f:  br.s       IL_001e

    IL_0011:  nop
    IL_0012:  ldloc.0
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0018:  nop
    IL_0019:  nop
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.1
    IL_001c:  add
    IL_001d:  stloc.0
    IL_001e:  ldloc.0
    IL_001f:  ldc.i4.s   10
    IL_0021:  clt
    IL_0023:  stloc.1
    IL_0024:  ldloc.1
    IL_0025:  brtrue.s   IL_0011

    IL_0027:  ldstr      "end"
    IL_002c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0031:  nop
    IL_0032:  nop
    IL_0033:  ret
  } // end of method ReduceNesting::EarlyExit1

  .method public hidebysig instance void 
          EarlyExit2() cil managed
  {
    // Code size       82 (0x52)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0008:  ldc.i4.0
    IL_0009:  ceq
    IL_000b:  stloc.1
    IL_000c:  ldloc.1
    IL_000d:  brtrue.s   IL_0012

    IL_000f:  nop
    IL_0010:  br.s       IL_0051

    IL_0012:  ldc.i4.0
    IL_0013:  stloc.0
    IL_0014:  br.s       IL_003d

    IL_0016:  nop
    IL_0017:  ldloc.0
    IL_0018:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001d:  nop
    IL_001e:  ldloc.0
    IL_001f:  ldc.i4.2
    IL_0020:  rem
    IL_0021:  ldc.i4.0
    IL_0022:  ceq
    IL_0024:  ldc.i4.0
    IL_0025:  ceq
    IL_0027:  stloc.1
    IL_0028:  ldloc.1
    IL_0029:  brtrue.s   IL_0038

    IL_002b:  nop
    IL_002c:  ldstr      "even"
    IL_0031:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0036:  nop
    IL_0037:  nop
    IL_0038:  nop
    IL_0039:  ldloc.0
    IL_003a:  ldc.i4.1
    IL_003b:  add
    IL_003c:  stloc.0
    IL_003d:  ldloc.0
    IL_003e:  ldc.i4.s   10
    IL_0040:  clt
    IL_0042:  stloc.1
    IL_0043:  ldloc.1
    IL_0044:  brtrue.s   IL_0016

    IL_0046:  ldstr      "end"
    IL_004b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0050:  nop
    IL_0051:  ret
  } // end of method ReduceNesting::EarlyExit2

  .method public hidebysig instance void 
          BalancedIf() cil managed
  {
    // Code size       90 (0x5a)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0008:  ldc.i4.0
    IL_0009:  ceq
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  brtrue.s   IL_0035

    IL_000f:  nop
    IL_0010:  ldstr      "true"
    IL_0015:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001a:  nop
    IL_001b:  ldarg.0
    IL_001c:  ldc.i4.1
    IL_001d:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0022:  ldc.i4.0
    IL_0023:  ceq
    IL_0025:  stloc.0
    IL_0026:  ldloc.0
    IL_0027:  brtrue.s   IL_0032

    IL_0029:  nop
    IL_002a:  ldc.i4.1
    IL_002b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0030:  nop
    IL_0031:  nop
    IL_0032:  nop
    IL_0033:  br.s       IL_0059

    IL_0035:  nop
    IL_0036:  ldarg.0
    IL_0037:  ldc.i4.2
    IL_0038:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_003d:  ldc.i4.0
    IL_003e:  ceq
    IL_0040:  stloc.0
    IL_0041:  ldloc.0
    IL_0042:  brtrue.s   IL_004d

    IL_0044:  nop
    IL_0045:  ldc.i4.2
    IL_0046:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_004b:  nop
    IL_004c:  nop
    IL_004d:  ldstr      "false"
    IL_0052:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0057:  nop
    IL_0058:  nop
    IL_0059:  ret
  } // end of method ReduceNesting::BalancedIf

  .method public hidebysig instance string 
          ComplexCase1(string s) cil managed
  {
    // Code size       269 (0x10d)
    .maxstack  2
    .locals init (int32 V_0,
             string V_1,
             bool V_2,
             int32 V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0008:  ldc.i4.0
    IL_0009:  ceq
    IL_000b:  stloc.2
    IL_000c:  ldloc.2
    IL_000d:  brtrue.s   IL_0017

    IL_000f:  nop
    IL_0010:  ldarg.1
    IL_0011:  stloc.1
    IL_0012:  br         IL_010b

    IL_0017:  ldc.i4.0
    IL_0018:  stloc.0
    IL_0019:  br         IL_00f7

    IL_001e:  nop
    IL_001f:  ldarg.0
    IL_0020:  ldc.i4.1
    IL_0021:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0026:  ldc.i4.0
    IL_0027:  ceq
    IL_0029:  stloc.2
    IL_002a:  ldloc.2
    IL_002b:  brtrue.s   IL_003b

    IL_002d:  nop
    IL_002e:  ldc.i4.1
    IL_002f:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0034:  nop
    IL_0035:  nop
    IL_0036:  br         IL_00f2

    IL_003b:  ldarg.0
    IL_003c:  ldc.i4.2
    IL_003d:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0042:  ldc.i4.0
    IL_0043:  ceq
    IL_0045:  stloc.2
    IL_0046:  ldloc.2
    IL_0047:  brtrue.s   IL_00b5

    IL_0049:  nop
    IL_004a:  ldloc.0
    IL_004b:  stloc.3
    IL_004c:  ldloc.3
    IL_004d:  ldc.i4.1
    IL_004e:  sub
    IL_004f:  switch     ( 
                          IL_0062,
                          IL_009e,
                          IL_009e)
    IL_0060:  br.s       IL_00ab

    IL_0062:  ldarg.0
    IL_0063:  ldc.i4.3
    IL_0064:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_0069:  ldc.i4.0
    IL_006a:  ceq
    IL_006c:  stloc.2
    IL_006d:  ldloc.2
    IL_006e:  brtrue.s   IL_007a

    IL_0070:  nop
    IL_0071:  ldc.i4.3
    IL_0072:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0077:  nop
    IL_0078:  br.s       IL_00ab

    IL_007a:  ldstr      "case1"
    IL_007f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0084:  nop
    IL_0085:  ldarg.0
    IL_0086:  ldc.i4.4
    IL_0087:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_008c:  ldc.i4.0
    IL_008d:  ceq
    IL_008f:  stloc.2
    IL_0090:  ldloc.2
    IL_0091:  brtrue.s   IL_009c

    IL_0093:  nop
    IL_0094:  ldc.i4.4
    IL_0095:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_009a:  nop
    IL_009b:  nop
    IL_009c:  br.s       IL_00ab

    IL_009e:  ldstr      "case23"
    IL_00a3:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00a8:  nop
    IL_00a9:  br.s       IL_00ab

    IL_00ab:  ldc.i4.2
    IL_00ac:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_00b1:  nop
    IL_00b2:  nop
    IL_00b3:  br.s       IL_00f2

    IL_00b5:  ldarg.0
    IL_00b6:  ldc.i4.5
    IL_00b7:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_00bc:  ldc.i4.0
    IL_00bd:  ceq
    IL_00bf:  stloc.2
    IL_00c0:  ldloc.2
    IL_00c1:  brtrue.s   IL_00ce

    IL_00c3:  nop
    IL_00c4:  ldc.i4.5
    IL_00c5:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_00ca:  nop
    IL_00cb:  nop
    IL_00cc:  br.s       IL_00f2

    IL_00ce:  nop
    IL_00cf:  ldarg.0
    IL_00d0:  ldc.i4.6
    IL_00d1:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ReduceNesting::B(int32)
    IL_00d6:  ldc.i4.0
    IL_00d7:  ceq
    IL_00d9:  stloc.2
    IL_00da:  ldloc.2
    IL_00db:  brtrue.s   IL_00e6

    IL_00dd:  nop
    IL_00de:  ldc.i4.6
    IL_00df:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_00e4:  nop
    IL_00e5:  nop
    IL_00e6:  ldstr      "else"
    IL_00eb:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00f0:  nop
    IL_00f1:  nop
    IL_00f2:  nop
    IL_00f3:  ldloc.0
    IL_00f4:  ldc.i4.1
    IL_00f5:  add
    IL_00f6:  stloc.0
    IL_00f7:  ldloc.0
    IL_00f8:  ldarg.1
    IL_00f9:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_00fe:  clt
    IL_0100:  stloc.2
    IL_0101:  ldloc.2
    IL_0102:  brtrue     IL_001e

    IL_0107:  ldarg.1
    IL_0108:  stloc.1
    IL_0109:  br.s       IL_010b

    IL_010b:  ldloc.1
    IL_010c:  ret
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
