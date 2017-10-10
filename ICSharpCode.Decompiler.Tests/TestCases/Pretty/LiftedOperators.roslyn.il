
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.18020
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly LiftedOperators
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
.module LiftedOperators.dll
// MVID: {CCC80DB9-B737-4485-B80C-251AB92EEEDC}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03060000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedOperators
       extends [mscorlib]System.Object
{
  .method public hidebysig static void  BoolBasic(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                  valuetype [mscorlib]System.Nullable`1<bool> b) cil managed
  {
    // Code size       107 (0x6b)
    .maxstack  2
    .locals init (bool V_0,
             valuetype [mscorlib]System.Nullable`1<bool> V_1,
             valuetype [mscorlib]System.Nullable`1<bool> V_2,
             bool V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldarg.1
    IL_0004:  stloc.2
    IL_0005:  ldloca.s   V_1
    IL_0007:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_000c:  ldloca.s   V_2
    IL_000e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0013:  beq.s      IL_0018

    IL_0015:  ldc.i4.0
    IL_0016:  br.s       IL_0028

    IL_0018:  ldloca.s   V_1
    IL_001a:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001f:  ldloca.s   V_2
    IL_0021:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0026:  ceq
    IL_0028:  stloc.0
    IL_0029:  ldloc.0
    IL_002a:  brfalse.s  IL_0034

    IL_002c:  nop
    IL_002d:  call       void [mscorlib]System.Console::WriteLine()
    IL_0032:  nop
    IL_0033:  nop
    IL_0034:  ldarg.0
    IL_0035:  stloc.2
    IL_0036:  ldarg.1
    IL_0037:  stloc.1
    IL_0038:  ldloca.s   V_2
    IL_003a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_003f:  ldloca.s   V_1
    IL_0041:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0046:  beq.s      IL_004b

    IL_0048:  ldc.i4.1
    IL_0049:  br.s       IL_005e

    IL_004b:  ldloca.s   V_2
    IL_004d:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0052:  ldloca.s   V_1
    IL_0054:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0059:  ceq
    IL_005b:  ldc.i4.0
    IL_005c:  ceq
    IL_005e:  stloc.3
    IL_005f:  ldloc.3
    IL_0060:  brfalse.s  IL_006a

    IL_0062:  nop
    IL_0063:  call       void [mscorlib]System.Console::WriteLine()
    IL_0068:  nop
    IL_0069:  nop
    IL_006a:  ret
  } // end of method LiftedOperators::BoolBasic

  .method public hidebysig static void  BoolComplex(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                    class [mscorlib]System.Func`1<bool> x) cil managed
  {
    // Code size       172 (0xac)
    .maxstack  2
    .locals init (bool V_0,
             valuetype [mscorlib]System.Nullable`1<bool> V_1,
             bool V_2,
             bool V_3,
             bool V_4,
             bool V_5)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldarg.1
    IL_0004:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0009:  stloc.2
    IL_000a:  ldloca.s   V_1
    IL_000c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0011:  ldloc.2
    IL_0012:  beq.s      IL_0017

    IL_0014:  ldc.i4.0
    IL_0015:  br.s       IL_001e

    IL_0017:  ldloca.s   V_1
    IL_0019:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001e:  stloc.0
    IL_001f:  ldloc.0
    IL_0020:  brfalse.s  IL_002a

    IL_0022:  nop
    IL_0023:  call       void [mscorlib]System.Console::WriteLine()
    IL_0028:  nop
    IL_0029:  nop
    IL_002a:  ldarg.0
    IL_002b:  stloc.1
    IL_002c:  ldarg.1
    IL_002d:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0032:  stloc.2
    IL_0033:  ldloca.s   V_1
    IL_0035:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_003a:  ldloc.2
    IL_003b:  beq.s      IL_0040

    IL_003d:  ldc.i4.1
    IL_003e:  br.s       IL_004a

    IL_0040:  ldloca.s   V_1
    IL_0042:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0047:  ldc.i4.0
    IL_0048:  ceq
    IL_004a:  stloc.3
    IL_004b:  ldloc.3
    IL_004c:  brfalse.s  IL_0056

    IL_004e:  nop
    IL_004f:  call       void [mscorlib]System.Console::WriteLine()
    IL_0054:  nop
    IL_0055:  nop
    IL_0056:  ldarg.1
    IL_0057:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_005c:  ldarg.0
    IL_005d:  stloc.1
    IL_005e:  ldloca.s   V_1
    IL_0060:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0065:  beq.s      IL_006a

    IL_0067:  ldc.i4.0
    IL_0068:  br.s       IL_0071

    IL_006a:  ldloca.s   V_1
    IL_006c:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0071:  stloc.s    V_4
    IL_0073:  ldloc.s    V_4
    IL_0075:  brfalse.s  IL_007f

    IL_0077:  nop
    IL_0078:  call       void [mscorlib]System.Console::WriteLine()
    IL_007d:  nop
    IL_007e:  nop
    IL_007f:  ldarg.1
    IL_0080:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0085:  ldarg.0
    IL_0086:  stloc.1
    IL_0087:  ldloca.s   V_1
    IL_0089:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_008e:  beq.s      IL_0093

    IL_0090:  ldc.i4.1
    IL_0091:  br.s       IL_009d

    IL_0093:  ldloca.s   V_1
    IL_0095:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_009a:  ldc.i4.0
    IL_009b:  ceq
    IL_009d:  stloc.s    V_5
    IL_009f:  ldloc.s    V_5
    IL_00a1:  brfalse.s  IL_00ab

    IL_00a3:  nop
    IL_00a4:  call       void [mscorlib]System.Console::WriteLine()
    IL_00a9:  nop
    IL_00aa:  nop
    IL_00ab:  ret
  } // end of method LiftedOperators::BoolComplex

  .method public hidebysig static void  BoolConst(valuetype [mscorlib]System.Nullable`1<bool> a) cil managed
  {
    // Code size       226 (0xe2)
    .maxstack  2
    .locals init (bool V_0,
             valuetype [mscorlib]System.Nullable`1<bool> V_1,
             bool V_2,
             bool V_3,
             bool V_4,
             bool V_5,
             bool V_6,
             bool V_7)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.2
    IL_0005:  ldloca.s   V_1
    IL_0007:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_000c:  ldloc.2
    IL_000d:  beq.s      IL_0012

    IL_000f:  ldc.i4.0
    IL_0010:  br.s       IL_0019

    IL_0012:  ldloca.s   V_1
    IL_0014:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  brfalse.s  IL_0025

    IL_001d:  nop
    IL_001e:  call       void [mscorlib]System.Console::WriteLine()
    IL_0023:  nop
    IL_0024:  nop
    IL_0025:  ldarg.0
    IL_0026:  stloc.1
    IL_0027:  ldc.i4.1
    IL_0028:  stloc.2
    IL_0029:  ldloca.s   V_1
    IL_002b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0030:  ldloc.2
    IL_0031:  beq.s      IL_0036

    IL_0033:  ldc.i4.1
    IL_0034:  br.s       IL_0040

    IL_0036:  ldloca.s   V_1
    IL_0038:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_003d:  ldc.i4.0
    IL_003e:  ceq
    IL_0040:  stloc.3
    IL_0041:  ldloc.3
    IL_0042:  brfalse.s  IL_004c

    IL_0044:  nop
    IL_0045:  call       void [mscorlib]System.Console::WriteLine()
    IL_004a:  nop
    IL_004b:  nop
    IL_004c:  ldarg.0
    IL_004d:  stloc.1
    IL_004e:  ldc.i4.0
    IL_004f:  stloc.2
    IL_0050:  ldloca.s   V_1
    IL_0052:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0057:  ldloc.2
    IL_0058:  beq.s      IL_005d

    IL_005a:  ldc.i4.0
    IL_005b:  br.s       IL_0064

    IL_005d:  ldloca.s   V_1
    IL_005f:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0064:  stloc.s    V_4
    IL_0066:  ldloc.s    V_4
    IL_0068:  brfalse.s  IL_0072

    IL_006a:  nop
    IL_006b:  call       void [mscorlib]System.Console::WriteLine()
    IL_0070:  nop
    IL_0071:  nop
    IL_0072:  ldarg.0
    IL_0073:  stloc.1
    IL_0074:  ldc.i4.0
    IL_0075:  stloc.2
    IL_0076:  ldloca.s   V_1
    IL_0078:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_007d:  ldloc.2
    IL_007e:  beq.s      IL_0083

    IL_0080:  ldc.i4.1
    IL_0081:  br.s       IL_008d

    IL_0083:  ldloca.s   V_1
    IL_0085:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_008a:  ldc.i4.0
    IL_008b:  ceq
    IL_008d:  stloc.s    V_5
    IL_008f:  ldloc.s    V_5
    IL_0091:  brfalse.s  IL_009b

    IL_0093:  nop
    IL_0094:  call       void [mscorlib]System.Console::WriteLine()
    IL_0099:  nop
    IL_009a:  nop
    IL_009b:  ldarg.0
    IL_009c:  stloc.1
    IL_009d:  ldloca.s   V_1
    IL_009f:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00a4:  brtrue.s   IL_00a9

    IL_00a6:  ldc.i4.1
    IL_00a7:  br.s       IL_00b0

    IL_00a9:  ldloca.s   V_1
    IL_00ab:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00b0:  stloc.s    V_6
    IL_00b2:  ldloc.s    V_6
    IL_00b4:  brfalse.s  IL_00be

    IL_00b6:  nop
    IL_00b7:  call       void [mscorlib]System.Console::WriteLine()
    IL_00bc:  nop
    IL_00bd:  nop
    IL_00be:  ldarg.0
    IL_00bf:  stloc.1
    IL_00c0:  ldloca.s   V_1
    IL_00c2:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00c7:  brtrue.s   IL_00cc

    IL_00c9:  ldc.i4.0
    IL_00ca:  br.s       IL_00d3

    IL_00cc:  ldloca.s   V_1
    IL_00ce:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00d3:  stloc.s    V_7
    IL_00d5:  ldloc.s    V_7
    IL_00d7:  brfalse.s  IL_00e1

    IL_00d9:  nop
    IL_00da:  call       void [mscorlib]System.Console::WriteLine()
    IL_00df:  nop
    IL_00e0:  nop
    IL_00e1:  ret
  } // end of method LiftedOperators::BoolConst

  .method public hidebysig static void  BoolValueBasic(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                       valuetype [mscorlib]System.Nullable`1<bool> b) cil managed
  {
    // Code size       452 (0x1c4)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<bool> V_0,
             valuetype [mscorlib]System.Nullable`1<bool> V_1,
             valuetype [mscorlib]System.Nullable`1<bool> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldarg.1
    IL_0004:  stloc.1
    IL_0005:  ldloca.s   V_0
    IL_0007:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_000c:  ldloca.s   V_1
    IL_000e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0013:  beq.s      IL_0018

    IL_0015:  ldc.i4.0
    IL_0016:  br.s       IL_0028

    IL_0018:  ldloca.s   V_0
    IL_001a:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001f:  ldloca.s   V_1
    IL_0021:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0026:  ceq
    IL_0028:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_002d:  nop
    IL_002e:  ldarg.0
    IL_002f:  stloc.1
    IL_0030:  ldarg.1
    IL_0031:  stloc.0
    IL_0032:  ldloca.s   V_1
    IL_0034:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0039:  ldloca.s   V_0
    IL_003b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0040:  beq.s      IL_0045

    IL_0042:  ldc.i4.1
    IL_0043:  br.s       IL_0058

    IL_0045:  ldloca.s   V_1
    IL_0047:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_004c:  ldloca.s   V_0
    IL_004e:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0053:  ceq
    IL_0055:  ldc.i4.0
    IL_0056:  ceq
    IL_0058:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_005d:  nop
    IL_005e:  ldarg.0
    IL_005f:  stloc.0
    IL_0060:  ldarg.1
    IL_0061:  stloc.1
    IL_0062:  ldloca.s   V_0
    IL_0064:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0069:  brtrue.s   IL_0080

    IL_006b:  ldloca.s   V_1
    IL_006d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0072:  brtrue.s   IL_007d

    IL_0074:  ldloca.s   V_0
    IL_0076:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_007b:  brfalse.s  IL_0080

    IL_007d:  ldloc.0
    IL_007e:  br.s       IL_0081

    IL_0080:  ldloc.1
    IL_0081:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0086:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_008b:  nop
    IL_008c:  ldarg.0
    IL_008d:  stloc.1
    IL_008e:  ldarg.1
    IL_008f:  stloc.0
    IL_0090:  ldloca.s   V_1
    IL_0092:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0097:  brtrue.s   IL_00ae

    IL_0099:  ldloca.s   V_0
    IL_009b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00a0:  brtrue.s   IL_00ab

    IL_00a2:  ldloca.s   V_1
    IL_00a4:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00a9:  brfalse.s  IL_00ae

    IL_00ab:  ldloc.0
    IL_00ac:  br.s       IL_00af

    IL_00ae:  ldloc.1
    IL_00af:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_00b4:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00b9:  nop
    IL_00ba:  ldarg.0
    IL_00bb:  stloc.0
    IL_00bc:  ldarg.1
    IL_00bd:  stloc.1
    IL_00be:  ldloca.s   V_0
    IL_00c0:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00c5:  ldloca.s   V_1
    IL_00c7:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00cc:  and
    IL_00cd:  brtrue.s   IL_00da

    IL_00cf:  ldloca.s   V_2
    IL_00d1:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_00d7:  ldloc.2
    IL_00d8:  br.s       IL_00ee

    IL_00da:  ldloca.s   V_0
    IL_00dc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00e1:  ldloca.s   V_1
    IL_00e3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00e8:  xor
    IL_00e9:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_00ee:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_00f3:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00f8:  nop
    IL_00f9:  ldarg.0
    IL_00fa:  stloc.1
    IL_00fb:  ldloca.s   V_1
    IL_00fd:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0102:  brtrue.s   IL_0107

    IL_0104:  ldarg.1
    IL_0105:  br.s       IL_0108

    IL_0107:  ldloc.1
    IL_0108:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_010d:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0112:  nop
    IL_0113:  ldarg.0
    IL_0114:  stloc.1
    IL_0115:  ldloca.s   V_1
    IL_0117:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_011c:  brtrue.s   IL_0129

    IL_011e:  ldloca.s   V_0
    IL_0120:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_0126:  ldloc.0
    IL_0127:  br.s       IL_0138

    IL_0129:  ldloca.s   V_1
    IL_012b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0130:  ldc.i4.0
    IL_0131:  ceq
    IL_0133:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0138:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_013d:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0142:  nop
    IL_0143:  ldarg.0
    IL_0144:  stloc.1
    IL_0145:  ldarg.1
    IL_0146:  stloc.0
    IL_0147:  ldloca.s   V_1
    IL_0149:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_014e:  brtrue.s   IL_0165

    IL_0150:  ldloca.s   V_0
    IL_0152:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0157:  brtrue.s   IL_0162

    IL_0159:  ldloca.s   V_1
    IL_015b:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0160:  brfalse.s  IL_0165

    IL_0162:  ldloc.1
    IL_0163:  br.s       IL_0166

    IL_0165:  ldloc.0
    IL_0166:  starg.s    a
    IL_0168:  ldarg.0
    IL_0169:  stloc.0
    IL_016a:  ldarg.1
    IL_016b:  stloc.1
    IL_016c:  ldloca.s   V_0
    IL_016e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0173:  brtrue.s   IL_018a

    IL_0175:  ldloca.s   V_1
    IL_0177:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_017c:  brtrue.s   IL_0187

    IL_017e:  ldloca.s   V_0
    IL_0180:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0185:  brfalse.s  IL_018a

    IL_0187:  ldloc.1
    IL_0188:  br.s       IL_018b

    IL_018a:  ldloc.0
    IL_018b:  starg.s    a
    IL_018d:  ldarg.0
    IL_018e:  stloc.1
    IL_018f:  ldarg.1
    IL_0190:  stloc.0
    IL_0191:  ldloca.s   V_1
    IL_0193:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0198:  ldloca.s   V_0
    IL_019a:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_019f:  and
    IL_01a0:  brtrue.s   IL_01ad

    IL_01a2:  ldloca.s   V_2
    IL_01a4:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_01aa:  ldloc.2
    IL_01ab:  br.s       IL_01c1

    IL_01ad:  ldloca.s   V_1
    IL_01af:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01b4:  ldloca.s   V_0
    IL_01b6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01bb:  xor
    IL_01bc:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_01c1:  starg.s    a
    IL_01c3:  ret
  } // end of method LiftedOperators::BoolValueBasic

  .method public hidebysig static void  BoolValueComplex(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                         class [mscorlib]System.Func`1<bool> x) cil managed
  {
    // Code size       528 (0x210)
    .maxstack  3
    .locals init (valuetype [mscorlib]System.Nullable`1<bool> V_0,
             bool V_1,
             valuetype [mscorlib]System.Nullable`1<bool> V_2,
             valuetype [mscorlib]System.Nullable`1<bool> V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldarg.1
    IL_0004:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0009:  stloc.1
    IL_000a:  ldloca.s   V_0
    IL_000c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0011:  ldloc.1
    IL_0012:  beq.s      IL_0017

    IL_0014:  ldc.i4.0
    IL_0015:  br.s       IL_001e

    IL_0017:  ldloca.s   V_0
    IL_0019:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0023:  nop
    IL_0024:  ldarg.0
    IL_0025:  stloc.0
    IL_0026:  ldarg.1
    IL_0027:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_0
    IL_002f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0034:  ldloc.1
    IL_0035:  beq.s      IL_003a

    IL_0037:  ldc.i4.1
    IL_0038:  br.s       IL_0044

    IL_003a:  ldloca.s   V_0
    IL_003c:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0041:  ldc.i4.0
    IL_0042:  ceq
    IL_0044:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0049:  nop
    IL_004a:  ldarg.1
    IL_004b:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0050:  ldarg.0
    IL_0051:  stloc.0
    IL_0052:  ldloca.s   V_0
    IL_0054:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0059:  beq.s      IL_005e

    IL_005b:  ldc.i4.0
    IL_005c:  br.s       IL_0065

    IL_005e:  ldloca.s   V_0
    IL_0060:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0065:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_006a:  nop
    IL_006b:  ldarg.1
    IL_006c:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0071:  ldarg.0
    IL_0072:  stloc.0
    IL_0073:  ldloca.s   V_0
    IL_0075:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_007a:  beq.s      IL_007f

    IL_007c:  ldc.i4.1
    IL_007d:  br.s       IL_0089

    IL_007f:  ldloca.s   V_0
    IL_0081:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0086:  ldc.i4.0
    IL_0087:  ceq
    IL_0089:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_008e:  nop
    IL_008f:  ldarg.0
    IL_0090:  stloc.0
    IL_0091:  ldarg.1
    IL_0092:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0097:  stloc.1
    IL_0098:  ldloca.s   V_0
    IL_009a:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_009f:  brtrue.s   IL_00ac

    IL_00a1:  ldloca.s   V_2
    IL_00a3:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_00a9:  ldloc.2
    IL_00aa:  br.s       IL_00ba

    IL_00ac:  ldloca.s   V_0
    IL_00ae:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00b3:  ldloc.1
    IL_00b4:  xor
    IL_00b5:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_00ba:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_00bf:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00c4:  nop
    IL_00c5:  ldarg.0
    IL_00c6:  stloc.0
    IL_00c7:  ldloca.s   V_0
    IL_00c9:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00ce:  brtrue.s   IL_00d8

    IL_00d0:  ldarg.1
    IL_00d1:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_00d6:  br.s       IL_00df

    IL_00d8:  ldloca.s   V_0
    IL_00da:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00df:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00e4:  nop
    IL_00e5:  ldarg.0
    IL_00e6:  stloc.0
    IL_00e7:  ldarg.1
    IL_00e8:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_00ed:  stloc.1
    IL_00ee:  ldloca.s   V_0
    IL_00f0:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00f5:  brtrue.s   IL_0102

    IL_00f7:  ldloca.s   V_2
    IL_00f9:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_00ff:  ldloc.2
    IL_0100:  br.s       IL_0110

    IL_0102:  ldloca.s   V_0
    IL_0104:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0109:  ldloc.1
    IL_010a:  xor
    IL_010b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0110:  starg.s    a
    IL_0112:  ldarg.1
    IL_0113:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0118:  ldarg.0
    IL_0119:  stloc.0
    IL_011a:  brtrue.s   IL_0124

    IL_011c:  ldc.i4.0
    IL_011d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0122:  br.s       IL_0125

    IL_0124:  ldloc.0
    IL_0125:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_012a:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_012f:  nop
    IL_0130:  ldarg.1
    IL_0131:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0136:  ldarg.0
    IL_0137:  stloc.0
    IL_0138:  brtrue.s   IL_013d

    IL_013a:  ldloc.0
    IL_013b:  br.s       IL_0143

    IL_013d:  ldc.i4.1
    IL_013e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0143:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0148:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_014d:  nop
    IL_014e:  ldarg.1
    IL_014f:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0154:  stloc.1
    IL_0155:  ldarg.0
    IL_0156:  stloc.0
    IL_0157:  ldloca.s   V_0
    IL_0159:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_015e:  brtrue.s   IL_016b

    IL_0160:  ldloca.s   V_2
    IL_0162:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_0168:  ldloc.2
    IL_0169:  br.s       IL_0179

    IL_016b:  ldloc.1
    IL_016c:  ldloca.s   V_0
    IL_016e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0173:  xor
    IL_0174:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0179:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_017e:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0183:  nop
    IL_0184:  ldc.i4.0
    IL_0185:  newarr     valuetype [mscorlib]System.Nullable`1<bool>
    IL_018a:  ldc.i4.0
    IL_018b:  ldelema    valuetype [mscorlib]System.Nullable`1<bool>
    IL_0190:  dup
    IL_0191:  ldobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_0196:  stloc.0
    IL_0197:  ldarg.1
    IL_0198:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_019d:  stloc.1
    IL_019e:  ldloca.s   V_0
    IL_01a0:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_01a5:  brtrue.s   IL_01b2

    IL_01a7:  ldloca.s   V_2
    IL_01a9:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_01af:  ldloc.2
    IL_01b0:  br.s       IL_01c0

    IL_01b2:  ldloca.s   V_0
    IL_01b4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01b9:  ldloc.1
    IL_01ba:  xor
    IL_01bb:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_01c0:  stobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_01c5:  ldc.i4.0
    IL_01c6:  newarr     valuetype [mscorlib]System.Nullable`1<bool>
    IL_01cb:  ldc.i4.0
    IL_01cc:  ldelema    valuetype [mscorlib]System.Nullable`1<bool>
    IL_01d1:  dup
    IL_01d2:  ldobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_01d7:  stloc.0
    IL_01d8:  ldarg.0
    IL_01d9:  stloc.2
    IL_01da:  ldloca.s   V_0
    IL_01dc:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_01e1:  ldloca.s   V_2
    IL_01e3:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_01e8:  and
    IL_01e9:  brtrue.s   IL_01f6

    IL_01eb:  ldloca.s   V_3
    IL_01ed:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_01f3:  ldloc.3
    IL_01f4:  br.s       IL_020a

    IL_01f6:  ldloca.s   V_0
    IL_01f8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01fd:  ldloca.s   V_2
    IL_01ff:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0204:  xor
    IL_0205:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_020a:  stobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_020f:  ret
  } // end of method LiftedOperators::BoolValueComplex

  .method public hidebysig static void  BoolValueConst(valuetype [mscorlib]System.Nullable`1<bool> a) cil managed
  {
    // Code size       182 (0xb6)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<bool> V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.1
    IL_0004:  stloc.1
    IL_0005:  ldloca.s   V_0
    IL_0007:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_000c:  ldloc.1
    IL_000d:  beq.s      IL_0012

    IL_000f:  ldc.i4.0
    IL_0010:  br.s       IL_0019

    IL_0012:  ldloca.s   V_0
    IL_0014:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0019:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_001e:  nop
    IL_001f:  ldarg.0
    IL_0020:  stloc.0
    IL_0021:  ldc.i4.1
    IL_0022:  stloc.1
    IL_0023:  ldloca.s   V_0
    IL_0025:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_002a:  ldloc.1
    IL_002b:  beq.s      IL_0030

    IL_002d:  ldc.i4.1
    IL_002e:  br.s       IL_003a

    IL_0030:  ldloca.s   V_0
    IL_0032:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0037:  ldc.i4.0
    IL_0038:  ceq
    IL_003a:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_003f:  nop
    IL_0040:  ldarg.0
    IL_0041:  stloc.0
    IL_0042:  ldc.i4.0
    IL_0043:  stloc.1
    IL_0044:  ldloca.s   V_0
    IL_0046:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_004b:  ldloc.1
    IL_004c:  beq.s      IL_0051

    IL_004e:  ldc.i4.0
    IL_004f:  br.s       IL_0058

    IL_0051:  ldloca.s   V_0
    IL_0053:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0058:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_005d:  nop
    IL_005e:  ldarg.0
    IL_005f:  stloc.0
    IL_0060:  ldc.i4.0
    IL_0061:  stloc.1
    IL_0062:  ldloca.s   V_0
    IL_0064:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0069:  ldloc.1
    IL_006a:  beq.s      IL_006f

    IL_006c:  ldc.i4.1
    IL_006d:  br.s       IL_0079

    IL_006f:  ldloca.s   V_0
    IL_0071:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0076:  ldc.i4.0
    IL_0077:  ceq
    IL_0079:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_007e:  nop
    IL_007f:  ldarg.0
    IL_0080:  stloc.0
    IL_0081:  ldloca.s   V_0
    IL_0083:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0088:  brtrue.s   IL_008d

    IL_008a:  ldc.i4.1
    IL_008b:  br.s       IL_0094

    IL_008d:  ldloca.s   V_0
    IL_008f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0094:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0099:  nop
    IL_009a:  ldarg.0
    IL_009b:  stloc.0
    IL_009c:  ldloca.s   V_0
    IL_009e:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00a3:  brtrue.s   IL_00a8

    IL_00a5:  ldc.i4.0
    IL_00a6:  br.s       IL_00af

    IL_00a8:  ldloca.s   V_0
    IL_00aa:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00af:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00b4:  nop
    IL_00b5:  ret
  } // end of method LiftedOperators::BoolValueConst

  .method public hidebysig static void  IntBasic(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                 valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       425 (0x1a9)
    .maxstack  2
    .locals init (bool V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             bool V_3,
             bool V_4,
             bool V_5,
             bool V_6,
             bool V_7,
             bool V_8,
             bool V_9)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldarg.1
    IL_0004:  stloc.2
    IL_0005:  ldloca.s   V_1
    IL_0007:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000c:  ldloca.s   V_2
    IL_000e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0013:  beq.s      IL_0018

    IL_0015:  ldc.i4.0
    IL_0016:  br.s       IL_0028

    IL_0018:  ldloca.s   V_1
    IL_001a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001f:  ldloca.s   V_2
    IL_0021:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0026:  ceq
    IL_0028:  stloc.0
    IL_0029:  ldloc.0
    IL_002a:  brfalse.s  IL_0034

    IL_002c:  nop
    IL_002d:  call       void [mscorlib]System.Console::WriteLine()
    IL_0032:  nop
    IL_0033:  nop
    IL_0034:  ldarg.0
    IL_0035:  stloc.2
    IL_0036:  ldarg.1
    IL_0037:  stloc.1
    IL_0038:  ldloca.s   V_2
    IL_003a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_003f:  ldloca.s   V_1
    IL_0041:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0046:  beq.s      IL_004b

    IL_0048:  ldc.i4.1
    IL_0049:  br.s       IL_005e

    IL_004b:  ldloca.s   V_2
    IL_004d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0052:  ldloca.s   V_1
    IL_0054:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0059:  ceq
    IL_005b:  ldc.i4.0
    IL_005c:  ceq
    IL_005e:  stloc.3
    IL_005f:  ldloc.3
    IL_0060:  brfalse.s  IL_006a

    IL_0062:  nop
    IL_0063:  call       void [mscorlib]System.Console::WriteLine()
    IL_0068:  nop
    IL_0069:  nop
    IL_006a:  ldarg.0
    IL_006b:  stloc.1
    IL_006c:  ldarg.1
    IL_006d:  stloc.2
    IL_006e:  ldloca.s   V_1
    IL_0070:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0075:  ldloca.s   V_2
    IL_0077:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_007c:  bgt.s      IL_0081

    IL_007e:  ldc.i4.0
    IL_007f:  br.s       IL_0090

    IL_0081:  ldloca.s   V_1
    IL_0083:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0088:  ldloca.s   V_2
    IL_008a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_008f:  and
    IL_0090:  stloc.s    V_4
    IL_0092:  ldloc.s    V_4
    IL_0094:  brfalse.s  IL_009e

    IL_0096:  nop
    IL_0097:  call       void [mscorlib]System.Console::WriteLine()
    IL_009c:  nop
    IL_009d:  nop
    IL_009e:  ldarg.0
    IL_009f:  stloc.2
    IL_00a0:  ldarg.1
    IL_00a1:  stloc.1
    IL_00a2:  ldloca.s   V_2
    IL_00a4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00a9:  ldloca.s   V_1
    IL_00ab:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00b0:  blt.s      IL_00b5

    IL_00b2:  ldc.i4.0
    IL_00b3:  br.s       IL_00c4

    IL_00b5:  ldloca.s   V_2
    IL_00b7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00bc:  ldloca.s   V_1
    IL_00be:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00c3:  and
    IL_00c4:  stloc.s    V_5
    IL_00c6:  ldloc.s    V_5
    IL_00c8:  brfalse.s  IL_00d2

    IL_00ca:  nop
    IL_00cb:  call       void [mscorlib]System.Console::WriteLine()
    IL_00d0:  nop
    IL_00d1:  nop
    IL_00d2:  ldarg.0
    IL_00d3:  stloc.1
    IL_00d4:  ldarg.1
    IL_00d5:  stloc.2
    IL_00d6:  ldloca.s   V_1
    IL_00d8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00dd:  ldloca.s   V_2
    IL_00df:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00e4:  bge.s      IL_00e9

    IL_00e6:  ldc.i4.0
    IL_00e7:  br.s       IL_00f8

    IL_00e9:  ldloca.s   V_1
    IL_00eb:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00f0:  ldloca.s   V_2
    IL_00f2:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00f7:  and
    IL_00f8:  stloc.s    V_6
    IL_00fa:  ldloc.s    V_6
    IL_00fc:  brfalse.s  IL_0106

    IL_00fe:  nop
    IL_00ff:  call       void [mscorlib]System.Console::WriteLine()
    IL_0104:  nop
    IL_0105:  nop
    IL_0106:  ldarg.0
    IL_0107:  stloc.2
    IL_0108:  ldarg.1
    IL_0109:  stloc.1
    IL_010a:  ldloca.s   V_2
    IL_010c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0111:  ldloca.s   V_1
    IL_0113:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0118:  ble.s      IL_011d

    IL_011a:  ldc.i4.0
    IL_011b:  br.s       IL_012c

    IL_011d:  ldloca.s   V_2
    IL_011f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0124:  ldloca.s   V_1
    IL_0126:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_012b:  and
    IL_012c:  stloc.s    V_7
    IL_012e:  ldloc.s    V_7
    IL_0130:  brfalse.s  IL_013a

    IL_0132:  nop
    IL_0133:  call       void [mscorlib]System.Console::WriteLine()
    IL_0138:  nop
    IL_0139:  nop
    IL_013a:  ldarg.0
    IL_013b:  stloc.1
    IL_013c:  ldarg.1
    IL_013d:  stloc.2
    IL_013e:  ldloca.s   V_1
    IL_0140:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0145:  ldloca.s   V_2
    IL_0147:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_014c:  bgt.s      IL_0151

    IL_014e:  ldc.i4.0
    IL_014f:  br.s       IL_0160

    IL_0151:  ldloca.s   V_1
    IL_0153:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0158:  ldloca.s   V_2
    IL_015a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_015f:  and
    IL_0160:  ldc.i4.0
    IL_0161:  ceq
    IL_0163:  stloc.s    V_8
    IL_0165:  ldloc.s    V_8
    IL_0167:  brfalse.s  IL_0171

    IL_0169:  nop
    IL_016a:  call       void [mscorlib]System.Console::WriteLine()
    IL_016f:  nop
    IL_0170:  nop
    IL_0171:  ldarg.0
    IL_0172:  stloc.2
    IL_0173:  ldarg.1
    IL_0174:  stloc.1
    IL_0175:  ldloca.s   V_2
    IL_0177:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_017c:  ldloca.s   V_1
    IL_017e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0183:  ble.s      IL_0188

    IL_0185:  ldc.i4.0
    IL_0186:  br.s       IL_0197

    IL_0188:  ldloca.s   V_2
    IL_018a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_018f:  ldloca.s   V_1
    IL_0191:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0196:  and
    IL_0197:  ldc.i4.0
    IL_0198:  ceq
    IL_019a:  stloc.s    V_9
    IL_019c:  ldloc.s    V_9
    IL_019e:  brfalse.s  IL_01a8

    IL_01a0:  nop
    IL_01a1:  call       void [mscorlib]System.Console::WriteLine()
    IL_01a6:  nop
    IL_01a7:  nop
    IL_01a8:  ret
  } // end of method LiftedOperators::IntBasic

  .method public hidebysig static void  IntComplex(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                   class [mscorlib]System.Func`1<int32> x) cil managed
  {
    // Code size       348 (0x15c)
    .maxstack  2
    .locals init (bool V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             int32 V_2,
             bool V_3,
             bool V_4,
             bool V_5,
             bool V_6,
             bool V_7,
             bool V_8,
             bool V_9)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldarg.1
    IL_0004:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0009:  stloc.2
    IL_000a:  ldloca.s   V_1
    IL_000c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0011:  ldloc.2
    IL_0012:  beq.s      IL_0017

    IL_0014:  ldc.i4.0
    IL_0015:  br.s       IL_001e

    IL_0017:  ldloca.s   V_1
    IL_0019:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001e:  stloc.0
    IL_001f:  ldloc.0
    IL_0020:  brfalse.s  IL_002a

    IL_0022:  nop
    IL_0023:  call       void [mscorlib]System.Console::WriteLine()
    IL_0028:  nop
    IL_0029:  nop
    IL_002a:  ldarg.0
    IL_002b:  stloc.1
    IL_002c:  ldarg.1
    IL_002d:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0032:  stloc.2
    IL_0033:  ldloca.s   V_1
    IL_0035:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_003a:  ldloc.2
    IL_003b:  beq.s      IL_0040

    IL_003d:  ldc.i4.1
    IL_003e:  br.s       IL_004a

    IL_0040:  ldloca.s   V_1
    IL_0042:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0047:  ldc.i4.0
    IL_0048:  ceq
    IL_004a:  stloc.3
    IL_004b:  ldloc.3
    IL_004c:  brfalse.s  IL_0056

    IL_004e:  nop
    IL_004f:  call       void [mscorlib]System.Console::WriteLine()
    IL_0054:  nop
    IL_0055:  nop
    IL_0056:  ldarg.0
    IL_0057:  stloc.1
    IL_0058:  ldarg.1
    IL_0059:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_005e:  stloc.2
    IL_005f:  ldloca.s   V_1
    IL_0061:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0066:  ldloc.2
    IL_0067:  bgt.s      IL_006c

    IL_0069:  ldc.i4.0
    IL_006a:  br.s       IL_0073

    IL_006c:  ldloca.s   V_1
    IL_006e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0073:  stloc.s    V_4
    IL_0075:  ldloc.s    V_4
    IL_0077:  brfalse.s  IL_0081

    IL_0079:  nop
    IL_007a:  call       void [mscorlib]System.Console::WriteLine()
    IL_007f:  nop
    IL_0080:  nop
    IL_0081:  ldarg.1
    IL_0082:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0087:  ldarg.0
    IL_0088:  stloc.1
    IL_0089:  ldloca.s   V_1
    IL_008b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0090:  beq.s      IL_0095

    IL_0092:  ldc.i4.0
    IL_0093:  br.s       IL_009c

    IL_0095:  ldloca.s   V_1
    IL_0097:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_009c:  stloc.s    V_5
    IL_009e:  ldloc.s    V_5
    IL_00a0:  brfalse.s  IL_00aa

    IL_00a2:  nop
    IL_00a3:  call       void [mscorlib]System.Console::WriteLine()
    IL_00a8:  nop
    IL_00a9:  nop
    IL_00aa:  ldarg.1
    IL_00ab:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00b0:  ldarg.0
    IL_00b1:  stloc.1
    IL_00b2:  ldloca.s   V_1
    IL_00b4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00b9:  beq.s      IL_00be

    IL_00bb:  ldc.i4.1
    IL_00bc:  br.s       IL_00c8

    IL_00be:  ldloca.s   V_1
    IL_00c0:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00c5:  ldc.i4.0
    IL_00c6:  ceq
    IL_00c8:  stloc.s    V_6
    IL_00ca:  ldloc.s    V_6
    IL_00cc:  brfalse.s  IL_00d6

    IL_00ce:  nop
    IL_00cf:  call       void [mscorlib]System.Console::WriteLine()
    IL_00d4:  nop
    IL_00d5:  nop
    IL_00d6:  ldarg.1
    IL_00d7:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00dc:  ldarg.0
    IL_00dd:  stloc.1
    IL_00de:  ldloca.s   V_1
    IL_00e0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00e5:  bgt.s      IL_00ea

    IL_00e7:  ldc.i4.0
    IL_00e8:  br.s       IL_00f1

    IL_00ea:  ldloca.s   V_1
    IL_00ec:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00f1:  stloc.s    V_7
    IL_00f3:  ldloc.s    V_7
    IL_00f5:  brfalse.s  IL_00ff

    IL_00f7:  nop
    IL_00f8:  call       void [mscorlib]System.Console::WriteLine()
    IL_00fd:  nop
    IL_00fe:  nop
    IL_00ff:  ldarg.0
    IL_0100:  stloc.1
    IL_0101:  ldarg.1
    IL_0102:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0107:  stloc.2
    IL_0108:  ldloca.s   V_1
    IL_010a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_010f:  ldloc.2
    IL_0110:  bgt.s      IL_0115

    IL_0112:  ldc.i4.0
    IL_0113:  br.s       IL_011c

    IL_0115:  ldloca.s   V_1
    IL_0117:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_011c:  ldc.i4.0
    IL_011d:  ceq
    IL_011f:  stloc.s    V_8
    IL_0121:  ldloc.s    V_8
    IL_0123:  brfalse.s  IL_012d

    IL_0125:  nop
    IL_0126:  call       void [mscorlib]System.Console::WriteLine()
    IL_012b:  nop
    IL_012c:  nop
    IL_012d:  ldarg.0
    IL_012e:  stloc.1
    IL_012f:  ldarg.1
    IL_0130:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0135:  stloc.2
    IL_0136:  ldloca.s   V_1
    IL_0138:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_013d:  ldloc.2
    IL_013e:  ble.s      IL_0143

    IL_0140:  ldc.i4.0
    IL_0141:  br.s       IL_014a

    IL_0143:  ldloca.s   V_1
    IL_0145:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_014a:  ldc.i4.0
    IL_014b:  ceq
    IL_014d:  stloc.s    V_9
    IL_014f:  ldloc.s    V_9
    IL_0151:  brfalse.s  IL_015b

    IL_0153:  nop
    IL_0154:  call       void [mscorlib]System.Console::WriteLine()
    IL_0159:  nop
    IL_015a:  nop
    IL_015b:  ret
  } // end of method LiftedOperators::IntComplex

  .method public hidebysig static void  IntConst(valuetype [mscorlib]System.Nullable`1<int32> a) cil managed
  {
    // Code size       226 (0xe2)
    .maxstack  2
    .locals init (bool V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             int32 V_2,
             bool V_3,
             bool V_4,
             bool V_5,
             bool V_6,
             bool V_7)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldc.i4.2
    IL_0004:  stloc.2
    IL_0005:  ldloca.s   V_1
    IL_0007:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000c:  ldloc.2
    IL_000d:  beq.s      IL_0012

    IL_000f:  ldc.i4.0
    IL_0010:  br.s       IL_0019

    IL_0012:  ldloca.s   V_1
    IL_0014:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  brfalse.s  IL_0025

    IL_001d:  nop
    IL_001e:  call       void [mscorlib]System.Console::WriteLine()
    IL_0023:  nop
    IL_0024:  nop
    IL_0025:  ldarg.0
    IL_0026:  stloc.1
    IL_0027:  ldc.i4.2
    IL_0028:  stloc.2
    IL_0029:  ldloca.s   V_1
    IL_002b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0030:  ldloc.2
    IL_0031:  beq.s      IL_0036

    IL_0033:  ldc.i4.1
    IL_0034:  br.s       IL_0040

    IL_0036:  ldloca.s   V_1
    IL_0038:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_003d:  ldc.i4.0
    IL_003e:  ceq
    IL_0040:  stloc.3
    IL_0041:  ldloc.3
    IL_0042:  brfalse.s  IL_004c

    IL_0044:  nop
    IL_0045:  call       void [mscorlib]System.Console::WriteLine()
    IL_004a:  nop
    IL_004b:  nop
    IL_004c:  ldarg.0
    IL_004d:  stloc.1
    IL_004e:  ldc.i4.2
    IL_004f:  stloc.2
    IL_0050:  ldloca.s   V_1
    IL_0052:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0057:  ldloc.2
    IL_0058:  bgt.s      IL_005d

    IL_005a:  ldc.i4.0
    IL_005b:  br.s       IL_0064

    IL_005d:  ldloca.s   V_1
    IL_005f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0064:  stloc.s    V_4
    IL_0066:  ldloc.s    V_4
    IL_0068:  brfalse.s  IL_0072

    IL_006a:  nop
    IL_006b:  call       void [mscorlib]System.Console::WriteLine()
    IL_0070:  nop
    IL_0071:  nop
    IL_0072:  ldc.i4.2
    IL_0073:  ldarg.0
    IL_0074:  stloc.1
    IL_0075:  ldloca.s   V_1
    IL_0077:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_007c:  beq.s      IL_0081

    IL_007e:  ldc.i4.0
    IL_007f:  br.s       IL_0088

    IL_0081:  ldloca.s   V_1
    IL_0083:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0088:  stloc.s    V_5
    IL_008a:  ldloc.s    V_5
    IL_008c:  brfalse.s  IL_0096

    IL_008e:  nop
    IL_008f:  call       void [mscorlib]System.Console::WriteLine()
    IL_0094:  nop
    IL_0095:  nop
    IL_0096:  ldc.i4.2
    IL_0097:  ldarg.0
    IL_0098:  stloc.1
    IL_0099:  ldloca.s   V_1
    IL_009b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00a0:  beq.s      IL_00a5

    IL_00a2:  ldc.i4.1
    IL_00a3:  br.s       IL_00af

    IL_00a5:  ldloca.s   V_1
    IL_00a7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ac:  ldc.i4.0
    IL_00ad:  ceq
    IL_00af:  stloc.s    V_6
    IL_00b1:  ldloc.s    V_6
    IL_00b3:  brfalse.s  IL_00bd

    IL_00b5:  nop
    IL_00b6:  call       void [mscorlib]System.Console::WriteLine()
    IL_00bb:  nop
    IL_00bc:  nop
    IL_00bd:  ldc.i4.2
    IL_00be:  ldarg.0
    IL_00bf:  stloc.1
    IL_00c0:  ldloca.s   V_1
    IL_00c2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00c7:  bgt.s      IL_00cc

    IL_00c9:  ldc.i4.0
    IL_00ca:  br.s       IL_00d3

    IL_00cc:  ldloca.s   V_1
    IL_00ce:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00d3:  stloc.s    V_7
    IL_00d5:  ldloc.s    V_7
    IL_00d7:  brfalse.s  IL_00e1

    IL_00d9:  nop
    IL_00da:  call       void [mscorlib]System.Console::WriteLine()
    IL_00df:  nop
    IL_00e0:  nop
    IL_00e1:  ret
  } // end of method LiftedOperators::IntConst

  .method public hidebysig static void  IntValueBasic(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                      valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       1633 (0x661)
    .maxstack  3
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldarg.1
    IL_0004:  stloc.1
    IL_0005:  ldloca.s   V_0
    IL_0007:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000c:  ldloca.s   V_1
    IL_000e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0013:  beq.s      IL_0018

    IL_0015:  ldc.i4.0
    IL_0016:  br.s       IL_0028

    IL_0018:  ldloca.s   V_0
    IL_001a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001f:  ldloca.s   V_1
    IL_0021:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0026:  ceq
    IL_0028:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_002d:  nop
    IL_002e:  ldarg.0
    IL_002f:  stloc.1
    IL_0030:  ldarg.1
    IL_0031:  stloc.0
    IL_0032:  ldloca.s   V_1
    IL_0034:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0039:  ldloca.s   V_0
    IL_003b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0040:  beq.s      IL_0045

    IL_0042:  ldc.i4.1
    IL_0043:  br.s       IL_0058

    IL_0045:  ldloca.s   V_1
    IL_0047:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_004c:  ldloca.s   V_0
    IL_004e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0053:  ceq
    IL_0055:  ldc.i4.0
    IL_0056:  ceq
    IL_0058:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_005d:  nop
    IL_005e:  ldarg.0
    IL_005f:  stloc.0
    IL_0060:  ldarg.1
    IL_0061:  stloc.1
    IL_0062:  ldloca.s   V_0
    IL_0064:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0069:  ldloca.s   V_1
    IL_006b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0070:  bgt.s      IL_0075

    IL_0072:  ldc.i4.0
    IL_0073:  br.s       IL_0084

    IL_0075:  ldloca.s   V_0
    IL_0077:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_007c:  ldloca.s   V_1
    IL_007e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0083:  and
    IL_0084:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0089:  nop
    IL_008a:  ldarg.0
    IL_008b:  stloc.1
    IL_008c:  ldarg.1
    IL_008d:  stloc.0
    IL_008e:  ldloca.s   V_1
    IL_0090:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0095:  ldloca.s   V_0
    IL_0097:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_009c:  bgt.s      IL_00a1

    IL_009e:  ldc.i4.0
    IL_009f:  br.s       IL_00b0

    IL_00a1:  ldloca.s   V_1
    IL_00a3:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00a8:  ldloca.s   V_0
    IL_00aa:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00af:  and
    IL_00b0:  ldc.i4.0
    IL_00b1:  ceq
    IL_00b3:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00b8:  nop
    IL_00b9:  ldarg.0
    IL_00ba:  stloc.0
    IL_00bb:  ldarg.1
    IL_00bc:  stloc.1
    IL_00bd:  ldloca.s   V_0
    IL_00bf:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00c4:  ldloca.s   V_1
    IL_00c6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00cb:  bge.s      IL_00d0

    IL_00cd:  ldc.i4.0
    IL_00ce:  br.s       IL_00df

    IL_00d0:  ldloca.s   V_0
    IL_00d2:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00d7:  ldloca.s   V_1
    IL_00d9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00de:  and
    IL_00df:  ldc.i4.0
    IL_00e0:  ceq
    IL_00e2:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00e7:  nop
    IL_00e8:  ldarg.0
    IL_00e9:  stloc.1
    IL_00ea:  ldarg.1
    IL_00eb:  stloc.0
    IL_00ec:  ldloca.s   V_1
    IL_00ee:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00f3:  ldloca.s   V_0
    IL_00f5:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00fa:  and
    IL_00fb:  brtrue.s   IL_0108

    IL_00fd:  ldloca.s   V_2
    IL_00ff:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0105:  ldloc.2
    IL_0106:  br.s       IL_011c

    IL_0108:  ldloca.s   V_1
    IL_010a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_010f:  ldloca.s   V_0
    IL_0111:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0116:  add
    IL_0117:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_011c:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0121:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0126:  nop
    IL_0127:  ldarg.0
    IL_0128:  stloc.0
    IL_0129:  ldarg.1
    IL_012a:  stloc.1
    IL_012b:  ldloca.s   V_0
    IL_012d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0132:  ldloca.s   V_1
    IL_0134:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0139:  and
    IL_013a:  brtrue.s   IL_0147

    IL_013c:  ldloca.s   V_2
    IL_013e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0144:  ldloc.2
    IL_0145:  br.s       IL_015b

    IL_0147:  ldloca.s   V_0
    IL_0149:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_014e:  ldloca.s   V_1
    IL_0150:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0155:  sub
    IL_0156:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_015b:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0160:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0165:  nop
    IL_0166:  ldarg.0
    IL_0167:  stloc.1
    IL_0168:  ldarg.1
    IL_0169:  stloc.0
    IL_016a:  ldloca.s   V_1
    IL_016c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0171:  ldloca.s   V_0
    IL_0173:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0178:  and
    IL_0179:  brtrue.s   IL_0186

    IL_017b:  ldloca.s   V_2
    IL_017d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0183:  ldloc.2
    IL_0184:  br.s       IL_019a

    IL_0186:  ldloca.s   V_1
    IL_0188:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_018d:  ldloca.s   V_0
    IL_018f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0194:  mul
    IL_0195:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_019a:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_019f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01a4:  nop
    IL_01a5:  ldarg.0
    IL_01a6:  stloc.0
    IL_01a7:  ldarg.1
    IL_01a8:  stloc.1
    IL_01a9:  ldloca.s   V_0
    IL_01ab:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01b0:  ldloca.s   V_1
    IL_01b2:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01b7:  and
    IL_01b8:  brtrue.s   IL_01c5

    IL_01ba:  ldloca.s   V_2
    IL_01bc:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01c2:  ldloc.2
    IL_01c3:  br.s       IL_01d9

    IL_01c5:  ldloca.s   V_0
    IL_01c7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01cc:  ldloca.s   V_1
    IL_01ce:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01d3:  div
    IL_01d4:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01d9:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01de:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01e3:  nop
    IL_01e4:  ldarg.0
    IL_01e5:  stloc.1
    IL_01e6:  ldarg.1
    IL_01e7:  stloc.0
    IL_01e8:  ldloca.s   V_1
    IL_01ea:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01ef:  ldloca.s   V_0
    IL_01f1:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01f6:  and
    IL_01f7:  brtrue.s   IL_0204

    IL_01f9:  ldloca.s   V_2
    IL_01fb:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0201:  ldloc.2
    IL_0202:  br.s       IL_0218

    IL_0204:  ldloca.s   V_1
    IL_0206:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_020b:  ldloca.s   V_0
    IL_020d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0212:  rem
    IL_0213:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0218:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_021d:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0222:  nop
    IL_0223:  ldarg.0
    IL_0224:  stloc.0
    IL_0225:  ldarg.1
    IL_0226:  stloc.1
    IL_0227:  ldloca.s   V_0
    IL_0229:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_022e:  ldloca.s   V_1
    IL_0230:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0235:  and
    IL_0236:  brtrue.s   IL_0243

    IL_0238:  ldloca.s   V_2
    IL_023a:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0240:  ldloc.2
    IL_0241:  br.s       IL_0257

    IL_0243:  ldloca.s   V_0
    IL_0245:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_024a:  ldloca.s   V_1
    IL_024c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0251:  and
    IL_0252:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0257:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_025c:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0261:  nop
    IL_0262:  ldarg.0
    IL_0263:  stloc.1
    IL_0264:  ldarg.1
    IL_0265:  stloc.0
    IL_0266:  ldloca.s   V_1
    IL_0268:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_026d:  ldloca.s   V_0
    IL_026f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0274:  and
    IL_0275:  brtrue.s   IL_0282

    IL_0277:  ldloca.s   V_2
    IL_0279:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_027f:  ldloc.2
    IL_0280:  br.s       IL_0296

    IL_0282:  ldloca.s   V_1
    IL_0284:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0289:  ldloca.s   V_0
    IL_028b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0290:  or
    IL_0291:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0296:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_029b:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02a0:  nop
    IL_02a1:  ldarg.0
    IL_02a2:  stloc.0
    IL_02a3:  ldarg.1
    IL_02a4:  stloc.1
    IL_02a5:  ldloca.s   V_0
    IL_02a7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02ac:  ldloca.s   V_1
    IL_02ae:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02b3:  and
    IL_02b4:  brtrue.s   IL_02c1

    IL_02b6:  ldloca.s   V_2
    IL_02b8:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02be:  ldloc.2
    IL_02bf:  br.s       IL_02d5

    IL_02c1:  ldloca.s   V_0
    IL_02c3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02c8:  ldloca.s   V_1
    IL_02ca:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02cf:  xor
    IL_02d0:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02d5:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_02da:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02df:  nop
    IL_02e0:  ldarg.0
    IL_02e1:  stloc.1
    IL_02e2:  ldarg.1
    IL_02e3:  stloc.0
    IL_02e4:  ldloca.s   V_1
    IL_02e6:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02eb:  ldloca.s   V_0
    IL_02ed:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02f2:  and
    IL_02f3:  brtrue.s   IL_0300

    IL_02f5:  ldloca.s   V_2
    IL_02f7:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02fd:  ldloc.2
    IL_02fe:  br.s       IL_0317

    IL_0300:  ldloca.s   V_1
    IL_0302:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0307:  ldloca.s   V_0
    IL_0309:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_030e:  ldc.i4.s   31
    IL_0310:  and
    IL_0311:  shl
    IL_0312:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0317:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_031c:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0321:  nop
    IL_0322:  ldarg.0
    IL_0323:  stloc.0
    IL_0324:  ldarg.1
    IL_0325:  stloc.1
    IL_0326:  ldloca.s   V_0
    IL_0328:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_032d:  ldloca.s   V_1
    IL_032f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0334:  and
    IL_0335:  brtrue.s   IL_0342

    IL_0337:  ldloca.s   V_2
    IL_0339:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_033f:  ldloc.2
    IL_0340:  br.s       IL_0359

    IL_0342:  ldloca.s   V_0
    IL_0344:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0349:  ldloca.s   V_1
    IL_034b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0350:  ldc.i4.s   31
    IL_0352:  and
    IL_0353:  shr
    IL_0354:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0359:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_035e:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0363:  nop
    IL_0364:  ldarg.0
    IL_0365:  stloc.1
    IL_0366:  ldloca.s   V_1
    IL_0368:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_036d:  brtrue.s   IL_0372

    IL_036f:  ldarg.1
    IL_0370:  br.s       IL_0373

    IL_0372:  ldloc.1
    IL_0373:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0378:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_037d:  nop
    IL_037e:  ldarg.0
    IL_037f:  stloc.1
    IL_0380:  ldloca.s   V_1
    IL_0382:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0387:  brtrue.s   IL_0394

    IL_0389:  ldloca.s   V_0
    IL_038b:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0391:  ldloc.0
    IL_0392:  br.s       IL_03a1

    IL_0394:  ldloca.s   V_1
    IL_0396:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_039b:  neg
    IL_039c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03a1:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_03a6:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_03ab:  nop
    IL_03ac:  ldarg.0
    IL_03ad:  stloc.1
    IL_03ae:  ldloca.s   V_1
    IL_03b0:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03b5:  brtrue.s   IL_03c2

    IL_03b7:  ldloca.s   V_0
    IL_03b9:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03bf:  ldloc.0
    IL_03c0:  br.s       IL_03cf

    IL_03c2:  ldloca.s   V_1
    IL_03c4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03c9:  not
    IL_03ca:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03cf:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_03d4:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_03d9:  nop
    IL_03da:  ldarg.0
    IL_03db:  stloc.1
    IL_03dc:  ldloca.s   V_1
    IL_03de:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03e3:  brtrue.s   IL_03f0

    IL_03e5:  ldloca.s   V_0
    IL_03e7:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03ed:  ldloc.0
    IL_03ee:  br.s       IL_03fe

    IL_03f0:  ldloca.s   V_1
    IL_03f2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03f7:  ldc.i4.1
    IL_03f8:  add
    IL_03f9:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03fe:  dup
    IL_03ff:  starg.s    a
    IL_0401:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0406:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_040b:  nop
    IL_040c:  ldarg.0
    IL_040d:  stloc.1
    IL_040e:  ldloca.s   V_1
    IL_0410:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0415:  brtrue.s   IL_0422

    IL_0417:  ldloca.s   V_0
    IL_0419:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_041f:  ldloc.0
    IL_0420:  br.s       IL_0430

    IL_0422:  ldloca.s   V_1
    IL_0424:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0429:  ldc.i4.1
    IL_042a:  sub
    IL_042b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0430:  dup
    IL_0431:  starg.s    a
    IL_0433:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0438:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_043d:  nop
    IL_043e:  ldarg.0
    IL_043f:  stloc.1
    IL_0440:  ldarg.1
    IL_0441:  stloc.0
    IL_0442:  ldloca.s   V_1
    IL_0444:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0449:  ldloca.s   V_0
    IL_044b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0450:  and
    IL_0451:  brtrue.s   IL_045e

    IL_0453:  ldloca.s   V_2
    IL_0455:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_045b:  ldloc.2
    IL_045c:  br.s       IL_0472

    IL_045e:  ldloca.s   V_1
    IL_0460:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0465:  ldloca.s   V_0
    IL_0467:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_046c:  add
    IL_046d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0472:  starg.s    a
    IL_0474:  ldarg.0
    IL_0475:  stloc.0
    IL_0476:  ldarg.1
    IL_0477:  stloc.1
    IL_0478:  ldloca.s   V_0
    IL_047a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_047f:  ldloca.s   V_1
    IL_0481:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0486:  and
    IL_0487:  brtrue.s   IL_0494

    IL_0489:  ldloca.s   V_2
    IL_048b:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0491:  ldloc.2
    IL_0492:  br.s       IL_04a8

    IL_0494:  ldloca.s   V_0
    IL_0496:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_049b:  ldloca.s   V_1
    IL_049d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04a2:  sub
    IL_04a3:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04a8:  starg.s    a
    IL_04aa:  ldarg.0
    IL_04ab:  stloc.1
    IL_04ac:  ldarg.1
    IL_04ad:  stloc.0
    IL_04ae:  ldloca.s   V_1
    IL_04b0:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04b5:  ldloca.s   V_0
    IL_04b7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04bc:  and
    IL_04bd:  brtrue.s   IL_04ca

    IL_04bf:  ldloca.s   V_2
    IL_04c1:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04c7:  ldloc.2
    IL_04c8:  br.s       IL_04de

    IL_04ca:  ldloca.s   V_1
    IL_04cc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04d1:  ldloca.s   V_0
    IL_04d3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04d8:  mul
    IL_04d9:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04de:  starg.s    a
    IL_04e0:  ldarg.0
    IL_04e1:  stloc.0
    IL_04e2:  ldarg.1
    IL_04e3:  stloc.1
    IL_04e4:  ldloca.s   V_0
    IL_04e6:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04eb:  ldloca.s   V_1
    IL_04ed:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04f2:  and
    IL_04f3:  brtrue.s   IL_0500

    IL_04f5:  ldloca.s   V_2
    IL_04f7:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04fd:  ldloc.2
    IL_04fe:  br.s       IL_0514

    IL_0500:  ldloca.s   V_0
    IL_0502:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0507:  ldloca.s   V_1
    IL_0509:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_050e:  div
    IL_050f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0514:  starg.s    a
    IL_0516:  ldarg.0
    IL_0517:  stloc.1
    IL_0518:  ldarg.1
    IL_0519:  stloc.0
    IL_051a:  ldloca.s   V_1
    IL_051c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0521:  ldloca.s   V_0
    IL_0523:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0528:  and
    IL_0529:  brtrue.s   IL_0536

    IL_052b:  ldloca.s   V_2
    IL_052d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0533:  ldloc.2
    IL_0534:  br.s       IL_054a

    IL_0536:  ldloca.s   V_1
    IL_0538:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_053d:  ldloca.s   V_0
    IL_053f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0544:  rem
    IL_0545:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_054a:  starg.s    a
    IL_054c:  ldarg.0
    IL_054d:  stloc.0
    IL_054e:  ldarg.1
    IL_054f:  stloc.1
    IL_0550:  ldloca.s   V_0
    IL_0552:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0557:  ldloca.s   V_1
    IL_0559:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_055e:  and
    IL_055f:  brtrue.s   IL_056c

    IL_0561:  ldloca.s   V_2
    IL_0563:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0569:  ldloc.2
    IL_056a:  br.s       IL_0580

    IL_056c:  ldloca.s   V_0
    IL_056e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0573:  ldloca.s   V_1
    IL_0575:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_057a:  and
    IL_057b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0580:  starg.s    a
    IL_0582:  ldarg.0
    IL_0583:  stloc.1
    IL_0584:  ldarg.1
    IL_0585:  stloc.0
    IL_0586:  ldloca.s   V_1
    IL_0588:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_058d:  ldloca.s   V_0
    IL_058f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0594:  and
    IL_0595:  brtrue.s   IL_05a2

    IL_0597:  ldloca.s   V_2
    IL_0599:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_059f:  ldloc.2
    IL_05a0:  br.s       IL_05b6

    IL_05a2:  ldloca.s   V_1
    IL_05a4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_05a9:  ldloca.s   V_0
    IL_05ab:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_05b0:  or
    IL_05b1:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_05b6:  starg.s    a
    IL_05b8:  ldarg.0
    IL_05b9:  stloc.0
    IL_05ba:  ldarg.1
    IL_05bb:  stloc.1
    IL_05bc:  ldloca.s   V_0
    IL_05be:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_05c3:  ldloca.s   V_1
    IL_05c5:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_05ca:  and
    IL_05cb:  brtrue.s   IL_05d8

    IL_05cd:  ldloca.s   V_2
    IL_05cf:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_05d5:  ldloc.2
    IL_05d6:  br.s       IL_05ec

    IL_05d8:  ldloca.s   V_0
    IL_05da:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_05df:  ldloca.s   V_1
    IL_05e1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_05e6:  xor
    IL_05e7:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_05ec:  starg.s    a
    IL_05ee:  ldarg.0
    IL_05ef:  stloc.1
    IL_05f0:  ldarg.1
    IL_05f1:  stloc.0
    IL_05f2:  ldloca.s   V_1
    IL_05f4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_05f9:  ldloca.s   V_0
    IL_05fb:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0600:  and
    IL_0601:  brtrue.s   IL_060e

    IL_0603:  ldloca.s   V_2
    IL_0605:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_060b:  ldloc.2
    IL_060c:  br.s       IL_0625

    IL_060e:  ldloca.s   V_1
    IL_0610:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0615:  ldloca.s   V_0
    IL_0617:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_061c:  ldc.i4.s   31
    IL_061e:  and
    IL_061f:  shl
    IL_0620:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0625:  starg.s    a
    IL_0627:  ldarg.0
    IL_0628:  stloc.0
    IL_0629:  ldarg.1
    IL_062a:  stloc.1
    IL_062b:  ldloca.s   V_0
    IL_062d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0632:  ldloca.s   V_1
    IL_0634:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0639:  and
    IL_063a:  brtrue.s   IL_0647

    IL_063c:  ldloca.s   V_2
    IL_063e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0644:  ldloc.2
    IL_0645:  br.s       IL_065e

    IL_0647:  ldloca.s   V_0
    IL_0649:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_064e:  ldloca.s   V_1
    IL_0650:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0655:  ldc.i4.s   31
    IL_0657:  and
    IL_0658:  shr
    IL_0659:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_065e:  starg.s    a
    IL_0660:  ret
  } // end of method LiftedOperators::IntValueBasic

  .method public hidebysig static void  IntValueComplex(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                        class [mscorlib]System.Func`1<int32> x) cil managed
  {
    // Code size       1365 (0x555)
    .maxstack  3
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             int32 V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldarg.1
    IL_0004:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0009:  stloc.1
    IL_000a:  ldloca.s   V_0
    IL_000c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0011:  ldloc.1
    IL_0012:  beq.s      IL_0017

    IL_0014:  ldc.i4.0
    IL_0015:  br.s       IL_001e

    IL_0017:  ldloca.s   V_0
    IL_0019:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0023:  nop
    IL_0024:  ldarg.0
    IL_0025:  stloc.0
    IL_0026:  ldarg.1
    IL_0027:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_002c:  stloc.1
    IL_002d:  ldloca.s   V_0
    IL_002f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0034:  ldloc.1
    IL_0035:  beq.s      IL_003a

    IL_0037:  ldc.i4.1
    IL_0038:  br.s       IL_0044

    IL_003a:  ldloca.s   V_0
    IL_003c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0041:  ldc.i4.0
    IL_0042:  ceq
    IL_0044:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0049:  nop
    IL_004a:  ldarg.0
    IL_004b:  stloc.0
    IL_004c:  ldarg.1
    IL_004d:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0052:  stloc.1
    IL_0053:  ldloca.s   V_0
    IL_0055:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_005a:  ldloc.1
    IL_005b:  bgt.s      IL_0060

    IL_005d:  ldc.i4.0
    IL_005e:  br.s       IL_0067

    IL_0060:  ldloca.s   V_0
    IL_0062:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0067:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_006c:  nop
    IL_006d:  ldarg.1
    IL_006e:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0073:  ldarg.0
    IL_0074:  stloc.0
    IL_0075:  ldloca.s   V_0
    IL_0077:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_007c:  beq.s      IL_0081

    IL_007e:  ldc.i4.0
    IL_007f:  br.s       IL_0088

    IL_0081:  ldloca.s   V_0
    IL_0083:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0088:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_008d:  nop
    IL_008e:  ldarg.1
    IL_008f:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0094:  ldarg.0
    IL_0095:  stloc.0
    IL_0096:  ldloca.s   V_0
    IL_0098:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_009d:  beq.s      IL_00a2

    IL_009f:  ldc.i4.1
    IL_00a0:  br.s       IL_00ac

    IL_00a2:  ldloca.s   V_0
    IL_00a4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00a9:  ldc.i4.0
    IL_00aa:  ceq
    IL_00ac:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00b1:  nop
    IL_00b2:  ldarg.1
    IL_00b3:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00b8:  ldarg.0
    IL_00b9:  stloc.0
    IL_00ba:  ldloca.s   V_0
    IL_00bc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00c1:  bgt.s      IL_00c6

    IL_00c3:  ldc.i4.0
    IL_00c4:  br.s       IL_00cd

    IL_00c6:  ldloca.s   V_0
    IL_00c8:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00cd:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00d2:  nop
    IL_00d3:  ldarg.0
    IL_00d4:  stloc.0
    IL_00d5:  ldarg.1
    IL_00d6:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00db:  stloc.1
    IL_00dc:  ldloca.s   V_0
    IL_00de:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00e3:  brtrue.s   IL_00f0

    IL_00e5:  ldloca.s   V_2
    IL_00e7:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00ed:  ldloc.2
    IL_00ee:  br.s       IL_00fe

    IL_00f0:  ldloca.s   V_0
    IL_00f2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00f7:  ldloc.1
    IL_00f8:  add
    IL_00f9:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00fe:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0103:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0108:  nop
    IL_0109:  ldarg.0
    IL_010a:  stloc.0
    IL_010b:  ldarg.1
    IL_010c:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0111:  stloc.1
    IL_0112:  ldloca.s   V_0
    IL_0114:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0119:  brtrue.s   IL_0126

    IL_011b:  ldloca.s   V_2
    IL_011d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0123:  ldloc.2
    IL_0124:  br.s       IL_0134

    IL_0126:  ldloca.s   V_0
    IL_0128:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_012d:  ldloc.1
    IL_012e:  sub
    IL_012f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0134:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0139:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_013e:  nop
    IL_013f:  ldarg.0
    IL_0140:  stloc.0
    IL_0141:  ldarg.1
    IL_0142:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0147:  stloc.1
    IL_0148:  ldloca.s   V_0
    IL_014a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_014f:  brtrue.s   IL_015c

    IL_0151:  ldloca.s   V_2
    IL_0153:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0159:  ldloc.2
    IL_015a:  br.s       IL_016a

    IL_015c:  ldloca.s   V_0
    IL_015e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0163:  ldloc.1
    IL_0164:  mul
    IL_0165:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_016a:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_016f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0174:  nop
    IL_0175:  ldarg.0
    IL_0176:  stloc.0
    IL_0177:  ldarg.1
    IL_0178:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_017d:  stloc.1
    IL_017e:  ldloca.s   V_0
    IL_0180:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0185:  brtrue.s   IL_0192

    IL_0187:  ldloca.s   V_2
    IL_0189:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_018f:  ldloc.2
    IL_0190:  br.s       IL_01a0

    IL_0192:  ldloca.s   V_0
    IL_0194:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0199:  ldloc.1
    IL_019a:  div
    IL_019b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01a0:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01a5:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01aa:  nop
    IL_01ab:  ldarg.0
    IL_01ac:  stloc.0
    IL_01ad:  ldarg.1
    IL_01ae:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_01b3:  stloc.1
    IL_01b4:  ldloca.s   V_0
    IL_01b6:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01bb:  brtrue.s   IL_01c8

    IL_01bd:  ldloca.s   V_2
    IL_01bf:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01c5:  ldloc.2
    IL_01c6:  br.s       IL_01d6

    IL_01c8:  ldloca.s   V_0
    IL_01ca:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01cf:  ldloc.1
    IL_01d0:  rem
    IL_01d1:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01d6:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01db:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01e0:  nop
    IL_01e1:  ldarg.0
    IL_01e2:  stloc.0
    IL_01e3:  ldarg.1
    IL_01e4:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_01e9:  stloc.1
    IL_01ea:  ldloca.s   V_0
    IL_01ec:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01f1:  brtrue.s   IL_01fe

    IL_01f3:  ldloca.s   V_2
    IL_01f5:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01fb:  ldloc.2
    IL_01fc:  br.s       IL_020c

    IL_01fe:  ldloca.s   V_0
    IL_0200:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0205:  ldloc.1
    IL_0206:  and
    IL_0207:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_020c:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0211:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0216:  nop
    IL_0217:  ldarg.0
    IL_0218:  stloc.0
    IL_0219:  ldarg.1
    IL_021a:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_021f:  stloc.1
    IL_0220:  ldloca.s   V_0
    IL_0222:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0227:  brtrue.s   IL_0234

    IL_0229:  ldloca.s   V_2
    IL_022b:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0231:  ldloc.2
    IL_0232:  br.s       IL_0242

    IL_0234:  ldloca.s   V_0
    IL_0236:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_023b:  ldloc.1
    IL_023c:  or
    IL_023d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0242:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0247:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_024c:  nop
    IL_024d:  ldarg.0
    IL_024e:  stloc.0
    IL_024f:  ldarg.1
    IL_0250:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0255:  stloc.1
    IL_0256:  ldloca.s   V_0
    IL_0258:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_025d:  brtrue.s   IL_026a

    IL_025f:  ldloca.s   V_2
    IL_0261:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0267:  ldloc.2
    IL_0268:  br.s       IL_0278

    IL_026a:  ldloca.s   V_0
    IL_026c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0271:  ldloc.1
    IL_0272:  xor
    IL_0273:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0278:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_027d:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0282:  nop
    IL_0283:  ldarg.0
    IL_0284:  stloc.0
    IL_0285:  ldarg.1
    IL_0286:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_028b:  stloc.1
    IL_028c:  ldloca.s   V_0
    IL_028e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0293:  brtrue.s   IL_02a0

    IL_0295:  ldloca.s   V_2
    IL_0297:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_029d:  ldloc.2
    IL_029e:  br.s       IL_02b1

    IL_02a0:  ldloca.s   V_0
    IL_02a2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02a7:  ldloc.1
    IL_02a8:  ldc.i4.s   31
    IL_02aa:  and
    IL_02ab:  shl
    IL_02ac:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02b1:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_02b6:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02bb:  nop
    IL_02bc:  ldarg.0
    IL_02bd:  stloc.0
    IL_02be:  ldarg.1
    IL_02bf:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_02c4:  stloc.1
    IL_02c5:  ldloca.s   V_0
    IL_02c7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02cc:  brtrue.s   IL_02d9

    IL_02ce:  ldloca.s   V_2
    IL_02d0:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02d6:  ldloc.2
    IL_02d7:  br.s       IL_02ea

    IL_02d9:  ldloca.s   V_0
    IL_02db:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02e0:  ldloc.1
    IL_02e1:  ldc.i4.s   31
    IL_02e3:  and
    IL_02e4:  shr
    IL_02e5:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02ea:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_02ef:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02f4:  nop
    IL_02f5:  ldarg.0
    IL_02f6:  stloc.0
    IL_02f7:  ldloca.s   V_0
    IL_02f9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02fe:  brtrue.s   IL_0308

    IL_0300:  ldarg.1
    IL_0301:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0306:  br.s       IL_030f

    IL_0308:  ldloca.s   V_0
    IL_030a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_030f:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0314:  nop
    IL_0315:  ldarg.0
    IL_0316:  stloc.0
    IL_0317:  ldarg.1
    IL_0318:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_031d:  stloc.1
    IL_031e:  ldloca.s   V_0
    IL_0320:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0325:  brtrue.s   IL_0332

    IL_0327:  ldloca.s   V_2
    IL_0329:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_032f:  ldloc.2
    IL_0330:  br.s       IL_0340

    IL_0332:  ldloca.s   V_0
    IL_0334:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0339:  ldloc.1
    IL_033a:  add
    IL_033b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0340:  starg.s    a
    IL_0342:  ldarg.0
    IL_0343:  stloc.0
    IL_0344:  ldarg.1
    IL_0345:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_034a:  stloc.1
    IL_034b:  ldloca.s   V_0
    IL_034d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0352:  brtrue.s   IL_035f

    IL_0354:  ldloca.s   V_2
    IL_0356:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_035c:  ldloc.2
    IL_035d:  br.s       IL_036d

    IL_035f:  ldloca.s   V_0
    IL_0361:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0366:  ldloc.1
    IL_0367:  sub
    IL_0368:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_036d:  starg.s    a
    IL_036f:  ldarg.0
    IL_0370:  stloc.0
    IL_0371:  ldarg.1
    IL_0372:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0377:  stloc.1
    IL_0378:  ldloca.s   V_0
    IL_037a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_037f:  brtrue.s   IL_038c

    IL_0381:  ldloca.s   V_2
    IL_0383:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0389:  ldloc.2
    IL_038a:  br.s       IL_039a

    IL_038c:  ldloca.s   V_0
    IL_038e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0393:  ldloc.1
    IL_0394:  mul
    IL_0395:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_039a:  starg.s    a
    IL_039c:  ldarg.0
    IL_039d:  stloc.0
    IL_039e:  ldarg.1
    IL_039f:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_03a4:  stloc.1
    IL_03a5:  ldloca.s   V_0
    IL_03a7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03ac:  brtrue.s   IL_03b9

    IL_03ae:  ldloca.s   V_2
    IL_03b0:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03b6:  ldloc.2
    IL_03b7:  br.s       IL_03c7

    IL_03b9:  ldloca.s   V_0
    IL_03bb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03c0:  ldloc.1
    IL_03c1:  div
    IL_03c2:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03c7:  starg.s    a
    IL_03c9:  ldarg.0
    IL_03ca:  stloc.0
    IL_03cb:  ldarg.1
    IL_03cc:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_03d1:  stloc.1
    IL_03d2:  ldloca.s   V_0
    IL_03d4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03d9:  brtrue.s   IL_03e6

    IL_03db:  ldloca.s   V_2
    IL_03dd:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03e3:  ldloc.2
    IL_03e4:  br.s       IL_03f4

    IL_03e6:  ldloca.s   V_0
    IL_03e8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03ed:  ldloc.1
    IL_03ee:  rem
    IL_03ef:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03f4:  starg.s    a
    IL_03f6:  ldarg.0
    IL_03f7:  stloc.0
    IL_03f8:  ldarg.1
    IL_03f9:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_03fe:  stloc.1
    IL_03ff:  ldloca.s   V_0
    IL_0401:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0406:  brtrue.s   IL_0413

    IL_0408:  ldloca.s   V_2
    IL_040a:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0410:  ldloc.2
    IL_0411:  br.s       IL_0421

    IL_0413:  ldloca.s   V_0
    IL_0415:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_041a:  ldloc.1
    IL_041b:  and
    IL_041c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0421:  starg.s    a
    IL_0423:  ldarg.0
    IL_0424:  stloc.0
    IL_0425:  ldarg.1
    IL_0426:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_042b:  stloc.1
    IL_042c:  ldloca.s   V_0
    IL_042e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0433:  brtrue.s   IL_0440

    IL_0435:  ldloca.s   V_2
    IL_0437:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_043d:  ldloc.2
    IL_043e:  br.s       IL_044e

    IL_0440:  ldloca.s   V_0
    IL_0442:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0447:  ldloc.1
    IL_0448:  or
    IL_0449:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_044e:  starg.s    a
    IL_0450:  ldarg.0
    IL_0451:  stloc.0
    IL_0452:  ldarg.1
    IL_0453:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0458:  stloc.1
    IL_0459:  ldloca.s   V_0
    IL_045b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0460:  brtrue.s   IL_046d

    IL_0462:  ldloca.s   V_2
    IL_0464:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_046a:  ldloc.2
    IL_046b:  br.s       IL_047b

    IL_046d:  ldloca.s   V_0
    IL_046f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0474:  ldloc.1
    IL_0475:  xor
    IL_0476:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_047b:  starg.s    a
    IL_047d:  ldarg.0
    IL_047e:  stloc.0
    IL_047f:  ldarg.1
    IL_0480:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0485:  stloc.1
    IL_0486:  ldloca.s   V_0
    IL_0488:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_048d:  brtrue.s   IL_049a

    IL_048f:  ldloca.s   V_2
    IL_0491:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0497:  ldloc.2
    IL_0498:  br.s       IL_04ab

    IL_049a:  ldloca.s   V_0
    IL_049c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04a1:  ldloc.1
    IL_04a2:  ldc.i4.s   31
    IL_04a4:  and
    IL_04a5:  shl
    IL_04a6:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04ab:  starg.s    a
    IL_04ad:  ldarg.0
    IL_04ae:  stloc.0
    IL_04af:  ldarg.1
    IL_04b0:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_04b5:  stloc.1
    IL_04b6:  ldloca.s   V_0
    IL_04b8:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04bd:  brtrue.s   IL_04ca

    IL_04bf:  ldloca.s   V_2
    IL_04c1:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04c7:  ldloc.2
    IL_04c8:  br.s       IL_04db

    IL_04ca:  ldloca.s   V_0
    IL_04cc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04d1:  ldloc.1
    IL_04d2:  ldc.i4.s   31
    IL_04d4:  and
    IL_04d5:  shr
    IL_04d6:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04db:  starg.s    a
    IL_04dd:  ldarg.1
    IL_04de:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_04e3:  stloc.1
    IL_04e4:  ldarg.0
    IL_04e5:  stloc.0
    IL_04e6:  ldloca.s   V_0
    IL_04e8:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04ed:  brtrue.s   IL_04fa

    IL_04ef:  ldloca.s   V_2
    IL_04f1:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04f7:  ldloc.2
    IL_04f8:  br.s       IL_0508

    IL_04fa:  ldloc.1
    IL_04fb:  ldloca.s   V_0
    IL_04fd:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0502:  add
    IL_0503:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0508:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_050d:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0512:  nop
    IL_0513:  ldc.i4.0
    IL_0514:  newarr     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0519:  ldc.i4.0
    IL_051a:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_051f:  dup
    IL_0520:  ldobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0525:  stloc.0
    IL_0526:  ldarg.1
    IL_0527:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_052c:  stloc.1
    IL_052d:  ldloca.s   V_0
    IL_052f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0534:  brtrue.s   IL_0541

    IL_0536:  ldloca.s   V_2
    IL_0538:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_053e:  ldloc.2
    IL_053f:  br.s       IL_054f

    IL_0541:  ldloca.s   V_0
    IL_0543:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0548:  ldloc.1
    IL_0549:  add
    IL_054a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_054f:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0554:  ret
  } // end of method LiftedOperators::IntValueComplex

  .method public hidebysig static void  IntValueConst(valuetype [mscorlib]System.Nullable`1<int32> a) cil managed
  {
    // Code size       1106 (0x452)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             int32 V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.2
    IL_0004:  stloc.1
    IL_0005:  ldloca.s   V_0
    IL_0007:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000c:  ldloc.1
    IL_000d:  beq.s      IL_0012

    IL_000f:  ldc.i4.0
    IL_0010:  br.s       IL_0019

    IL_0012:  ldloca.s   V_0
    IL_0014:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0019:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_001e:  nop
    IL_001f:  ldarg.0
    IL_0020:  stloc.0
    IL_0021:  ldc.i4.2
    IL_0022:  stloc.1
    IL_0023:  ldloca.s   V_0
    IL_0025:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_002a:  ldloc.1
    IL_002b:  beq.s      IL_0030

    IL_002d:  ldc.i4.1
    IL_002e:  br.s       IL_003a

    IL_0030:  ldloca.s   V_0
    IL_0032:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0037:  ldc.i4.0
    IL_0038:  ceq
    IL_003a:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_003f:  nop
    IL_0040:  ldarg.0
    IL_0041:  stloc.0
    IL_0042:  ldc.i4.2
    IL_0043:  stloc.1
    IL_0044:  ldloca.s   V_0
    IL_0046:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_004b:  ldloc.1
    IL_004c:  bgt.s      IL_0051

    IL_004e:  ldc.i4.0
    IL_004f:  br.s       IL_0058

    IL_0051:  ldloca.s   V_0
    IL_0053:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0058:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_005d:  nop
    IL_005e:  ldc.i4.2
    IL_005f:  ldarg.0
    IL_0060:  stloc.0
    IL_0061:  ldloca.s   V_0
    IL_0063:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0068:  beq.s      IL_006d

    IL_006a:  ldc.i4.0
    IL_006b:  br.s       IL_0074

    IL_006d:  ldloca.s   V_0
    IL_006f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0074:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0079:  nop
    IL_007a:  ldc.i4.2
    IL_007b:  ldarg.0
    IL_007c:  stloc.0
    IL_007d:  ldloca.s   V_0
    IL_007f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0084:  beq.s      IL_0089

    IL_0086:  ldc.i4.1
    IL_0087:  br.s       IL_0093

    IL_0089:  ldloca.s   V_0
    IL_008b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0090:  ldc.i4.0
    IL_0091:  ceq
    IL_0093:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0098:  nop
    IL_0099:  ldc.i4.2
    IL_009a:  ldarg.0
    IL_009b:  stloc.0
    IL_009c:  ldloca.s   V_0
    IL_009e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00a3:  bgt.s      IL_00a8

    IL_00a5:  ldc.i4.0
    IL_00a6:  br.s       IL_00af

    IL_00a8:  ldloca.s   V_0
    IL_00aa:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00af:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00b4:  nop
    IL_00b5:  ldarg.0
    IL_00b6:  stloc.0
    IL_00b7:  ldloca.s   V_0
    IL_00b9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00be:  brtrue.s   IL_00cb

    IL_00c0:  ldloca.s   V_2
    IL_00c2:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00c8:  ldloc.2
    IL_00c9:  br.s       IL_00d9

    IL_00cb:  ldloca.s   V_0
    IL_00cd:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00d2:  ldc.i4.2
    IL_00d3:  add
    IL_00d4:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00d9:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_00de:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00e3:  nop
    IL_00e4:  ldarg.0
    IL_00e5:  stloc.0
    IL_00e6:  ldloca.s   V_0
    IL_00e8:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ed:  brtrue.s   IL_00fa

    IL_00ef:  ldloca.s   V_2
    IL_00f1:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00f7:  ldloc.2
    IL_00f8:  br.s       IL_0108

    IL_00fa:  ldloca.s   V_0
    IL_00fc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0101:  ldc.i4.2
    IL_0102:  sub
    IL_0103:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0108:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_010d:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0112:  nop
    IL_0113:  ldarg.0
    IL_0114:  stloc.0
    IL_0115:  ldloca.s   V_0
    IL_0117:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_011c:  brtrue.s   IL_0129

    IL_011e:  ldloca.s   V_2
    IL_0120:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0126:  ldloc.2
    IL_0127:  br.s       IL_0137

    IL_0129:  ldloca.s   V_0
    IL_012b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0130:  ldc.i4.2
    IL_0131:  mul
    IL_0132:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0137:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_013c:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0141:  nop
    IL_0142:  ldarg.0
    IL_0143:  stloc.0
    IL_0144:  ldloca.s   V_0
    IL_0146:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_014b:  brtrue.s   IL_0158

    IL_014d:  ldloca.s   V_2
    IL_014f:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0155:  ldloc.2
    IL_0156:  br.s       IL_0166

    IL_0158:  ldloca.s   V_0
    IL_015a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_015f:  ldc.i4.2
    IL_0160:  div
    IL_0161:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0166:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_016b:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0170:  nop
    IL_0171:  ldarg.0
    IL_0172:  stloc.0
    IL_0173:  ldloca.s   V_0
    IL_0175:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_017a:  brtrue.s   IL_0187

    IL_017c:  ldloca.s   V_2
    IL_017e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0184:  ldloc.2
    IL_0185:  br.s       IL_0195

    IL_0187:  ldloca.s   V_0
    IL_0189:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_018e:  ldc.i4.2
    IL_018f:  rem
    IL_0190:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0195:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_019a:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_019f:  nop
    IL_01a0:  ldarg.0
    IL_01a1:  stloc.0
    IL_01a2:  ldloca.s   V_0
    IL_01a4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01a9:  brtrue.s   IL_01b6

    IL_01ab:  ldloca.s   V_2
    IL_01ad:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01b3:  ldloc.2
    IL_01b4:  br.s       IL_01c4

    IL_01b6:  ldloca.s   V_0
    IL_01b8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01bd:  ldc.i4.2
    IL_01be:  and
    IL_01bf:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01c4:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01c9:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01ce:  nop
    IL_01cf:  ldarg.0
    IL_01d0:  stloc.0
    IL_01d1:  ldloca.s   V_0
    IL_01d3:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01d8:  brtrue.s   IL_01e5

    IL_01da:  ldloca.s   V_2
    IL_01dc:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01e2:  ldloc.2
    IL_01e3:  br.s       IL_01f3

    IL_01e5:  ldloca.s   V_0
    IL_01e7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01ec:  ldc.i4.2
    IL_01ed:  or
    IL_01ee:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01f3:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01f8:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01fd:  nop
    IL_01fe:  ldarg.0
    IL_01ff:  stloc.0
    IL_0200:  ldloca.s   V_0
    IL_0202:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0207:  brtrue.s   IL_0214

    IL_0209:  ldloca.s   V_2
    IL_020b:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0211:  ldloc.2
    IL_0212:  br.s       IL_0222

    IL_0214:  ldloca.s   V_0
    IL_0216:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_021b:  ldc.i4.2
    IL_021c:  xor
    IL_021d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0222:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0227:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_022c:  nop
    IL_022d:  ldarg.0
    IL_022e:  stloc.0
    IL_022f:  ldloca.s   V_0
    IL_0231:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0236:  brtrue.s   IL_0243

    IL_0238:  ldloca.s   V_2
    IL_023a:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0240:  ldloc.2
    IL_0241:  br.s       IL_0251

    IL_0243:  ldloca.s   V_0
    IL_0245:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_024a:  ldc.i4.2
    IL_024b:  shl
    IL_024c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0251:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0256:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_025b:  nop
    IL_025c:  ldarg.0
    IL_025d:  stloc.0
    IL_025e:  ldloca.s   V_0
    IL_0260:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0265:  brtrue.s   IL_0272

    IL_0267:  ldloca.s   V_2
    IL_0269:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_026f:  ldloc.2
    IL_0270:  br.s       IL_0280

    IL_0272:  ldloca.s   V_0
    IL_0274:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0279:  ldc.i4.2
    IL_027a:  shr
    IL_027b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0280:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0285:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_028a:  nop
    IL_028b:  ldarg.0
    IL_028c:  stloc.0
    IL_028d:  ldloca.s   V_0
    IL_028f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0294:  brtrue.s   IL_0299

    IL_0296:  ldc.i4.2
    IL_0297:  br.s       IL_02a0

    IL_0299:  ldloca.s   V_0
    IL_029b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02a0:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_02a5:  nop
    IL_02a6:  ldarg.0
    IL_02a7:  stloc.0
    IL_02a8:  ldloca.s   V_0
    IL_02aa:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02af:  brtrue.s   IL_02bc

    IL_02b1:  ldloca.s   V_2
    IL_02b3:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02b9:  ldloc.2
    IL_02ba:  br.s       IL_02ca

    IL_02bc:  ldloca.s   V_0
    IL_02be:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02c3:  ldc.i4.2
    IL_02c4:  add
    IL_02c5:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02ca:  starg.s    a
    IL_02cc:  ldarg.0
    IL_02cd:  stloc.0
    IL_02ce:  ldloca.s   V_0
    IL_02d0:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02d5:  brtrue.s   IL_02e2

    IL_02d7:  ldloca.s   V_2
    IL_02d9:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02df:  ldloc.2
    IL_02e0:  br.s       IL_02f0

    IL_02e2:  ldloca.s   V_0
    IL_02e4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02e9:  ldc.i4.2
    IL_02ea:  sub
    IL_02eb:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02f0:  starg.s    a
    IL_02f2:  ldarg.0
    IL_02f3:  stloc.0
    IL_02f4:  ldloca.s   V_0
    IL_02f6:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02fb:  brtrue.s   IL_0308

    IL_02fd:  ldloca.s   V_2
    IL_02ff:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0305:  ldloc.2
    IL_0306:  br.s       IL_0316

    IL_0308:  ldloca.s   V_0
    IL_030a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_030f:  ldc.i4.2
    IL_0310:  mul
    IL_0311:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0316:  starg.s    a
    IL_0318:  ldarg.0
    IL_0319:  stloc.0
    IL_031a:  ldloca.s   V_0
    IL_031c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0321:  brtrue.s   IL_032e

    IL_0323:  ldloca.s   V_2
    IL_0325:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_032b:  ldloc.2
    IL_032c:  br.s       IL_033c

    IL_032e:  ldloca.s   V_0
    IL_0330:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0335:  ldc.i4.2
    IL_0336:  div
    IL_0337:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_033c:  starg.s    a
    IL_033e:  ldarg.0
    IL_033f:  stloc.0
    IL_0340:  ldloca.s   V_0
    IL_0342:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0347:  brtrue.s   IL_0354

    IL_0349:  ldloca.s   V_2
    IL_034b:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0351:  ldloc.2
    IL_0352:  br.s       IL_0362

    IL_0354:  ldloca.s   V_0
    IL_0356:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_035b:  ldc.i4.2
    IL_035c:  rem
    IL_035d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0362:  starg.s    a
    IL_0364:  ldarg.0
    IL_0365:  stloc.0
    IL_0366:  ldloca.s   V_0
    IL_0368:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_036d:  brtrue.s   IL_037a

    IL_036f:  ldloca.s   V_2
    IL_0371:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0377:  ldloc.2
    IL_0378:  br.s       IL_0388

    IL_037a:  ldloca.s   V_0
    IL_037c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0381:  ldc.i4.2
    IL_0382:  and
    IL_0383:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0388:  starg.s    a
    IL_038a:  ldarg.0
    IL_038b:  stloc.0
    IL_038c:  ldloca.s   V_0
    IL_038e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0393:  brtrue.s   IL_03a0

    IL_0395:  ldloca.s   V_2
    IL_0397:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_039d:  ldloc.2
    IL_039e:  br.s       IL_03ae

    IL_03a0:  ldloca.s   V_0
    IL_03a2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03a7:  ldc.i4.2
    IL_03a8:  or
    IL_03a9:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03ae:  starg.s    a
    IL_03b0:  ldarg.0
    IL_03b1:  stloc.0
    IL_03b2:  ldloca.s   V_0
    IL_03b4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03b9:  brtrue.s   IL_03c6

    IL_03bb:  ldloca.s   V_2
    IL_03bd:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03c3:  ldloc.2
    IL_03c4:  br.s       IL_03d4

    IL_03c6:  ldloca.s   V_0
    IL_03c8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03cd:  ldc.i4.2
    IL_03ce:  xor
    IL_03cf:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03d4:  starg.s    a
    IL_03d6:  ldarg.0
    IL_03d7:  stloc.0
    IL_03d8:  ldloca.s   V_0
    IL_03da:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03df:  brtrue.s   IL_03ec

    IL_03e1:  ldloca.s   V_2
    IL_03e3:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03e9:  ldloc.2
    IL_03ea:  br.s       IL_03fa

    IL_03ec:  ldloca.s   V_0
    IL_03ee:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03f3:  ldc.i4.2
    IL_03f4:  shl
    IL_03f5:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03fa:  starg.s    a
    IL_03fc:  ldarg.0
    IL_03fd:  stloc.0
    IL_03fe:  ldloca.s   V_0
    IL_0400:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0405:  brtrue.s   IL_0412

    IL_0407:  ldloca.s   V_2
    IL_0409:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_040f:  ldloc.2
    IL_0410:  br.s       IL_0420

    IL_0412:  ldloca.s   V_0
    IL_0414:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0419:  ldc.i4.2
    IL_041a:  shr
    IL_041b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0420:  starg.s    a
    IL_0422:  ldarg.0
    IL_0423:  stloc.0
    IL_0424:  ldloca.s   V_0
    IL_0426:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_042b:  brtrue.s   IL_0438

    IL_042d:  ldloca.s   V_2
    IL_042f:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0435:  ldloc.2
    IL_0436:  br.s       IL_0446

    IL_0438:  ldc.i4.2
    IL_0439:  ldloca.s   V_0
    IL_043b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0440:  add
    IL_0441:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0446:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_044b:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0450:  nop
    IL_0451:  ret
  } // end of method LiftedOperators::IntValueConst

  .method public hidebysig static void  NumberBasic(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> a,
                                                    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> b) cil managed
  {
    // Code size       465 (0x1d1)
    .maxstack  2
    .locals init (bool V_0,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_1,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_2,
             bool V_3,
             bool V_4,
             bool V_5,
             bool V_6,
             bool V_7,
             bool V_8,
             bool V_9)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldarg.1
    IL_0004:  stloc.2
    IL_0005:  ldloca.s   V_1
    IL_0007:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_000c:  ldloca.s   V_2
    IL_000e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0013:  call       bool [mscorlib]System.Decimal::op_Equality(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_0018:  brtrue.s   IL_001d

    IL_001a:  ldc.i4.0
    IL_001b:  br.s       IL_002d

    IL_001d:  ldloca.s   V_1
    IL_001f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0024:  ldloca.s   V_2
    IL_0026:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_002b:  ceq
    IL_002d:  stloc.0
    IL_002e:  ldloc.0
    IL_002f:  brfalse.s  IL_0039

    IL_0031:  nop
    IL_0032:  call       void [mscorlib]System.Console::WriteLine()
    IL_0037:  nop
    IL_0038:  nop
    IL_0039:  ldarg.0
    IL_003a:  stloc.2
    IL_003b:  ldarg.1
    IL_003c:  stloc.1
    IL_003d:  ldloca.s   V_2
    IL_003f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0044:  ldloca.s   V_1
    IL_0046:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_004b:  call       bool [mscorlib]System.Decimal::op_Equality(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_0050:  brtrue.s   IL_0055

    IL_0052:  ldc.i4.1
    IL_0053:  br.s       IL_0068

    IL_0055:  ldloca.s   V_2
    IL_0057:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_005c:  ldloca.s   V_1
    IL_005e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0063:  ceq
    IL_0065:  ldc.i4.0
    IL_0066:  ceq
    IL_0068:  stloc.3
    IL_0069:  ldloc.3
    IL_006a:  brfalse.s  IL_0074

    IL_006c:  nop
    IL_006d:  call       void [mscorlib]System.Console::WriteLine()
    IL_0072:  nop
    IL_0073:  nop
    IL_0074:  ldarg.0
    IL_0075:  stloc.1
    IL_0076:  ldarg.1
    IL_0077:  stloc.2
    IL_0078:  ldloca.s   V_1
    IL_007a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_007f:  ldloca.s   V_2
    IL_0081:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0086:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_008b:  brtrue.s   IL_0090

    IL_008d:  ldc.i4.0
    IL_008e:  br.s       IL_009f

    IL_0090:  ldloca.s   V_1
    IL_0092:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0097:  ldloca.s   V_2
    IL_0099:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_009e:  and
    IL_009f:  stloc.s    V_4
    IL_00a1:  ldloc.s    V_4
    IL_00a3:  brfalse.s  IL_00ad

    IL_00a5:  nop
    IL_00a6:  call       void [mscorlib]System.Console::WriteLine()
    IL_00ab:  nop
    IL_00ac:  nop
    IL_00ad:  ldarg.0
    IL_00ae:  stloc.2
    IL_00af:  ldarg.1
    IL_00b0:  stloc.1
    IL_00b1:  ldloca.s   V_2
    IL_00b3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00b8:  ldloca.s   V_1
    IL_00ba:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00bf:  call       bool [mscorlib]System.Decimal::op_LessThan(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_00c4:  brtrue.s   IL_00c9

    IL_00c6:  ldc.i4.0
    IL_00c7:  br.s       IL_00d8

    IL_00c9:  ldloca.s   V_2
    IL_00cb:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00d0:  ldloca.s   V_1
    IL_00d2:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00d7:  and
    IL_00d8:  stloc.s    V_5
    IL_00da:  ldloc.s    V_5
    IL_00dc:  brfalse.s  IL_00e6

    IL_00de:  nop
    IL_00df:  call       void [mscorlib]System.Console::WriteLine()
    IL_00e4:  nop
    IL_00e5:  nop
    IL_00e6:  ldarg.0
    IL_00e7:  stloc.1
    IL_00e8:  ldarg.1
    IL_00e9:  stloc.2
    IL_00ea:  ldloca.s   V_1
    IL_00ec:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00f1:  ldloca.s   V_2
    IL_00f3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00f8:  call       bool [mscorlib]System.Decimal::op_GreaterThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                              valuetype [mscorlib]System.Decimal)
    IL_00fd:  brtrue.s   IL_0102

    IL_00ff:  ldc.i4.0
    IL_0100:  br.s       IL_0111

    IL_0102:  ldloca.s   V_1
    IL_0104:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0109:  ldloca.s   V_2
    IL_010b:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0110:  and
    IL_0111:  stloc.s    V_6
    IL_0113:  ldloc.s    V_6
    IL_0115:  brfalse.s  IL_011f

    IL_0117:  nop
    IL_0118:  call       void [mscorlib]System.Console::WriteLine()
    IL_011d:  nop
    IL_011e:  nop
    IL_011f:  ldarg.0
    IL_0120:  stloc.2
    IL_0121:  ldarg.1
    IL_0122:  stloc.1
    IL_0123:  ldloca.s   V_2
    IL_0125:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_012a:  ldloca.s   V_1
    IL_012c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0131:  call       bool [mscorlib]System.Decimal::op_LessThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                           valuetype [mscorlib]System.Decimal)
    IL_0136:  brtrue.s   IL_013b

    IL_0138:  ldc.i4.0
    IL_0139:  br.s       IL_014a

    IL_013b:  ldloca.s   V_2
    IL_013d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0142:  ldloca.s   V_1
    IL_0144:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0149:  and
    IL_014a:  stloc.s    V_7
    IL_014c:  ldloc.s    V_7
    IL_014e:  brfalse.s  IL_0158

    IL_0150:  nop
    IL_0151:  call       void [mscorlib]System.Console::WriteLine()
    IL_0156:  nop
    IL_0157:  nop
    IL_0158:  ldarg.0
    IL_0159:  stloc.1
    IL_015a:  ldarg.1
    IL_015b:  stloc.2
    IL_015c:  ldloca.s   V_1
    IL_015e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0163:  ldloca.s   V_2
    IL_0165:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_016a:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_016f:  brtrue.s   IL_0174

    IL_0171:  ldc.i4.0
    IL_0172:  br.s       IL_0183

    IL_0174:  ldloca.s   V_1
    IL_0176:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_017b:  ldloca.s   V_2
    IL_017d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0182:  and
    IL_0183:  ldc.i4.0
    IL_0184:  ceq
    IL_0186:  stloc.s    V_8
    IL_0188:  ldloc.s    V_8
    IL_018a:  brfalse.s  IL_0194

    IL_018c:  nop
    IL_018d:  call       void [mscorlib]System.Console::WriteLine()
    IL_0192:  nop
    IL_0193:  nop
    IL_0194:  ldarg.0
    IL_0195:  stloc.2
    IL_0196:  ldarg.1
    IL_0197:  stloc.1
    IL_0198:  ldloca.s   V_2
    IL_019a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_019f:  ldloca.s   V_1
    IL_01a1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01a6:  call       bool [mscorlib]System.Decimal::op_LessThan(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_01ab:  brtrue.s   IL_01b0

    IL_01ad:  ldc.i4.0
    IL_01ae:  br.s       IL_01bf

    IL_01b0:  ldloca.s   V_2
    IL_01b2:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_01b7:  ldloca.s   V_1
    IL_01b9:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_01be:  and
    IL_01bf:  ldc.i4.0
    IL_01c0:  ceq
    IL_01c2:  stloc.s    V_9
    IL_01c4:  ldloc.s    V_9
    IL_01c6:  brfalse.s  IL_01d0

    IL_01c8:  nop
    IL_01c9:  call       void [mscorlib]System.Console::WriteLine()
    IL_01ce:  nop
    IL_01cf:  nop
    IL_01d0:  ret
  } // end of method LiftedOperators::NumberBasic

  .method public hidebysig static void  NumberComplex(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> a,
                                                      class [mscorlib]System.Func`1<valuetype [mscorlib]System.Decimal> x) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method LiftedOperators::NumberComplex

  .method public hidebysig static void  NumberConst(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> a) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method LiftedOperators::NumberConst

  .method public hidebysig static void  NumberValueBasic(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> a,
                                                         valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> b) cil managed
  {
    // Code size       959 (0x3bf)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_0,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_1,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldarg.1
    IL_0004:  stloc.1
    IL_0005:  ldloca.s   V_0
    IL_0007:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_000c:  ldloca.s   V_1
    IL_000e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0013:  call       bool [mscorlib]System.Decimal::op_Equality(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_0018:  brtrue.s   IL_001d

    IL_001a:  ldc.i4.0
    IL_001b:  br.s       IL_002d

    IL_001d:  ldloca.s   V_0
    IL_001f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0024:  ldloca.s   V_1
    IL_0026:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_002b:  ceq
    IL_002d:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0032:  nop
    IL_0033:  ldarg.0
    IL_0034:  stloc.1
    IL_0035:  ldarg.1
    IL_0036:  stloc.0
    IL_0037:  ldloca.s   V_1
    IL_0039:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_003e:  ldloca.s   V_0
    IL_0040:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0045:  call       bool [mscorlib]System.Decimal::op_Equality(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_004a:  brtrue.s   IL_004f

    IL_004c:  ldc.i4.1
    IL_004d:  br.s       IL_0062

    IL_004f:  ldloca.s   V_1
    IL_0051:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0056:  ldloca.s   V_0
    IL_0058:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_005d:  ceq
    IL_005f:  ldc.i4.0
    IL_0060:  ceq
    IL_0062:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0067:  nop
    IL_0068:  ldarg.0
    IL_0069:  stloc.0
    IL_006a:  ldarg.1
    IL_006b:  stloc.1
    IL_006c:  ldloca.s   V_0
    IL_006e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0073:  ldloca.s   V_1
    IL_0075:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_007a:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_007f:  brtrue.s   IL_0084

    IL_0081:  ldc.i4.0
    IL_0082:  br.s       IL_0093

    IL_0084:  ldloca.s   V_0
    IL_0086:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_008b:  ldloca.s   V_1
    IL_008d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0092:  and
    IL_0093:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0098:  nop
    IL_0099:  ldarg.0
    IL_009a:  stloc.1
    IL_009b:  ldarg.1
    IL_009c:  stloc.0
    IL_009d:  ldloca.s   V_1
    IL_009f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00a4:  ldloca.s   V_0
    IL_00a6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00ab:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_00b0:  brtrue.s   IL_00b5

    IL_00b2:  ldc.i4.0
    IL_00b3:  br.s       IL_00c4

    IL_00b5:  ldloca.s   V_1
    IL_00b7:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00bc:  ldloca.s   V_0
    IL_00be:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00c3:  and
    IL_00c4:  ldc.i4.0
    IL_00c5:  ceq
    IL_00c7:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00cc:  nop
    IL_00cd:  ldarg.0
    IL_00ce:  stloc.0
    IL_00cf:  ldarg.1
    IL_00d0:  stloc.1
    IL_00d1:  ldloca.s   V_0
    IL_00d3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00d8:  ldloca.s   V_1
    IL_00da:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00df:  call       bool [mscorlib]System.Decimal::op_LessThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                           valuetype [mscorlib]System.Decimal)
    IL_00e4:  brtrue.s   IL_00e9

    IL_00e6:  ldc.i4.0
    IL_00e7:  br.s       IL_00f8

    IL_00e9:  ldloca.s   V_0
    IL_00eb:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00f0:  ldloca.s   V_1
    IL_00f2:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00f7:  and
    IL_00f8:  ldc.i4.0
    IL_00f9:  ceq
    IL_00fb:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0100:  nop
    IL_0101:  ldarg.0
    IL_0102:  stloc.1
    IL_0103:  ldarg.1
    IL_0104:  stloc.0
    IL_0105:  ldloca.s   V_1
    IL_0107:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_010c:  ldloca.s   V_0
    IL_010e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0113:  and
    IL_0114:  brtrue.s   IL_0121

    IL_0116:  ldloca.s   V_2
    IL_0118:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_011e:  ldloc.2
    IL_011f:  br.s       IL_0139

    IL_0121:  ldloca.s   V_1
    IL_0123:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0128:  ldloca.s   V_0
    IL_012a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_012f:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_0134:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0139:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_013e:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0143:  nop
    IL_0144:  ldarg.0
    IL_0145:  stloc.0
    IL_0146:  ldarg.1
    IL_0147:  stloc.1
    IL_0148:  ldloca.s   V_0
    IL_014a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_014f:  ldloca.s   V_1
    IL_0151:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0156:  and
    IL_0157:  brtrue.s   IL_0164

    IL_0159:  ldloca.s   V_2
    IL_015b:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0161:  ldloc.2
    IL_0162:  br.s       IL_017c

    IL_0164:  ldloca.s   V_0
    IL_0166:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_016b:  ldloca.s   V_1
    IL_016d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0172:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Subtraction(valuetype [mscorlib]System.Decimal,
                                                                                                     valuetype [mscorlib]System.Decimal)
    IL_0177:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_017c:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0181:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0186:  nop
    IL_0187:  ldarg.0
    IL_0188:  stloc.1
    IL_0189:  ldarg.1
    IL_018a:  stloc.0
    IL_018b:  ldloca.s   V_1
    IL_018d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0192:  ldloca.s   V_0
    IL_0194:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0199:  and
    IL_019a:  brtrue.s   IL_01a7

    IL_019c:  ldloca.s   V_2
    IL_019e:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_01a4:  ldloc.2
    IL_01a5:  br.s       IL_01bf

    IL_01a7:  ldloca.s   V_1
    IL_01a9:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01ae:  ldloca.s   V_0
    IL_01b0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01b5:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Multiply(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_01ba:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_01bf:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_01c4:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01c9:  nop
    IL_01ca:  ldarg.0
    IL_01cb:  stloc.0
    IL_01cc:  ldarg.1
    IL_01cd:  stloc.1
    IL_01ce:  ldloca.s   V_0
    IL_01d0:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_01d5:  ldloca.s   V_1
    IL_01d7:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_01dc:  and
    IL_01dd:  brtrue.s   IL_01ea

    IL_01df:  ldloca.s   V_2
    IL_01e1:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_01e7:  ldloc.2
    IL_01e8:  br.s       IL_0202

    IL_01ea:  ldloca.s   V_0
    IL_01ec:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01f1:  ldloca.s   V_1
    IL_01f3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01f8:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Division(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_01fd:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0202:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0207:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_020c:  nop
    IL_020d:  ldarg.0
    IL_020e:  stloc.1
    IL_020f:  ldarg.1
    IL_0210:  stloc.0
    IL_0211:  ldloca.s   V_1
    IL_0213:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0218:  ldloca.s   V_0
    IL_021a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_021f:  and
    IL_0220:  brtrue.s   IL_022d

    IL_0222:  ldloca.s   V_2
    IL_0224:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_022a:  ldloc.2
    IL_022b:  br.s       IL_0245

    IL_022d:  ldloca.s   V_1
    IL_022f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0234:  ldloca.s   V_0
    IL_0236:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_023b:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Modulus(valuetype [mscorlib]System.Decimal,
                                                                                                 valuetype [mscorlib]System.Decimal)
    IL_0240:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0245:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_024a:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_024f:  nop
    IL_0250:  ldarg.0
    IL_0251:  stloc.0
    IL_0252:  ldloca.s   V_0
    IL_0254:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0259:  brtrue.s   IL_025e

    IL_025b:  ldarg.1
    IL_025c:  br.s       IL_025f

    IL_025e:  ldloc.0
    IL_025f:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0264:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0269:  nop
    IL_026a:  ldarg.0
    IL_026b:  stloc.0
    IL_026c:  ldloca.s   V_0
    IL_026e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0273:  brtrue.s   IL_0280

    IL_0275:  ldloca.s   V_1
    IL_0277:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_027d:  ldloc.1
    IL_027e:  br.s       IL_0291

    IL_0280:  ldloca.s   V_0
    IL_0282:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0287:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_UnaryNegation(valuetype [mscorlib]System.Decimal)
    IL_028c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0291:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0296:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_029b:  nop
    IL_029c:  ldarg.0
    IL_029d:  stloc.0
    IL_029e:  ldarg.1
    IL_029f:  stloc.1
    IL_02a0:  ldloca.s   V_0
    IL_02a2:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_02a7:  ldloca.s   V_1
    IL_02a9:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_02ae:  and
    IL_02af:  brtrue.s   IL_02bc

    IL_02b1:  ldloca.s   V_2
    IL_02b3:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_02b9:  ldloc.2
    IL_02ba:  br.s       IL_02d4

    IL_02bc:  ldloca.s   V_0
    IL_02be:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_02c3:  ldloca.s   V_1
    IL_02c5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_02ca:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_02cf:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_02d4:  starg.s    a
    IL_02d6:  ldarg.0
    IL_02d7:  stloc.1
    IL_02d8:  ldarg.1
    IL_02d9:  stloc.0
    IL_02da:  ldloca.s   V_1
    IL_02dc:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_02e1:  ldloca.s   V_0
    IL_02e3:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_02e8:  and
    IL_02e9:  brtrue.s   IL_02f6

    IL_02eb:  ldloca.s   V_2
    IL_02ed:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_02f3:  ldloc.2
    IL_02f4:  br.s       IL_030e

    IL_02f6:  ldloca.s   V_1
    IL_02f8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_02fd:  ldloca.s   V_0
    IL_02ff:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0304:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Subtraction(valuetype [mscorlib]System.Decimal,
                                                                                                     valuetype [mscorlib]System.Decimal)
    IL_0309:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_030e:  starg.s    a
    IL_0310:  ldarg.0
    IL_0311:  stloc.0
    IL_0312:  ldarg.1
    IL_0313:  stloc.1
    IL_0314:  ldloca.s   V_0
    IL_0316:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_031b:  ldloca.s   V_1
    IL_031d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0322:  and
    IL_0323:  brtrue.s   IL_0330

    IL_0325:  ldloca.s   V_2
    IL_0327:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_032d:  ldloc.2
    IL_032e:  br.s       IL_0348

    IL_0330:  ldloca.s   V_0
    IL_0332:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0337:  ldloca.s   V_1
    IL_0339:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_033e:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Multiply(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_0343:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0348:  starg.s    a
    IL_034a:  ldarg.0
    IL_034b:  stloc.1
    IL_034c:  ldarg.1
    IL_034d:  stloc.0
    IL_034e:  ldloca.s   V_1
    IL_0350:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0355:  ldloca.s   V_0
    IL_0357:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_035c:  and
    IL_035d:  brtrue.s   IL_036a

    IL_035f:  ldloca.s   V_2
    IL_0361:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0367:  ldloc.2
    IL_0368:  br.s       IL_0382

    IL_036a:  ldloca.s   V_1
    IL_036c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0371:  ldloca.s   V_0
    IL_0373:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0378:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Division(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_037d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0382:  starg.s    a
    IL_0384:  ldarg.0
    IL_0385:  stloc.0
    IL_0386:  ldarg.1
    IL_0387:  stloc.1
    IL_0388:  ldloca.s   V_0
    IL_038a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_038f:  ldloca.s   V_1
    IL_0391:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0396:  and
    IL_0397:  brtrue.s   IL_03a4

    IL_0399:  ldloca.s   V_2
    IL_039b:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_03a1:  ldloc.2
    IL_03a2:  br.s       IL_03bc

    IL_03a4:  ldloca.s   V_0
    IL_03a6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_03ab:  ldloca.s   V_1
    IL_03ad:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_03b2:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Modulus(valuetype [mscorlib]System.Decimal,
                                                                                                 valuetype [mscorlib]System.Decimal)
    IL_03b7:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_03bc:  starg.s    a
    IL_03be:  ret
  } // end of method LiftedOperators::NumberValueBasic

  .method public hidebysig static void  NumberValueComplex(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> a,
                                                           class [mscorlib]System.Func`1<valuetype [mscorlib]System.Decimal> x) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method LiftedOperators::NumberValueComplex

  .method public hidebysig static void  NumberValueConst(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> a) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method LiftedOperators::NumberValueConst

  .method public hidebysig static void  CompareWithImplictCast(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                               valuetype [mscorlib]System.Nullable`1<int64> b) cil managed
  {
    // Code size       175 (0xaf)
    .maxstack  2
    .locals init (bool V_0,
             valuetype [mscorlib]System.Nullable`1<int64> V_1,
             valuetype [mscorlib]System.Nullable`1<int64> V_2,
             valuetype [mscorlib]System.Nullable`1<int32> V_3,
             valuetype [mscorlib]System.Nullable`1<int64> V_4,
             bool V_5)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.3
    IL_0003:  ldloca.s   V_3
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_0018

    IL_000c:  ldloca.s   V_4
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0014:  ldloc.s    V_4
    IL_0016:  br.s       IL_0025

    IL_0018:  ldloca.s   V_3
    IL_001a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001f:  conv.i8
    IL_0020:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0025:  stloc.1
    IL_0026:  ldarg.1
    IL_0027:  stloc.2
    IL_0028:  ldloca.s   V_1
    IL_002a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_002f:  ldloca.s   V_2
    IL_0031:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0036:  blt.s      IL_003b

    IL_0038:  ldc.i4.0
    IL_0039:  br.s       IL_004a

    IL_003b:  ldloca.s   V_1
    IL_003d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0042:  ldloca.s   V_2
    IL_0044:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0049:  and
    IL_004a:  stloc.0
    IL_004b:  ldloc.0
    IL_004c:  brfalse.s  IL_0056

    IL_004e:  nop
    IL_004f:  call       void [mscorlib]System.Console::WriteLine()
    IL_0054:  nop
    IL_0055:  nop
    IL_0056:  ldarg.0
    IL_0057:  stloc.3
    IL_0058:  ldloca.s   V_3
    IL_005a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_005f:  brtrue.s   IL_006d

    IL_0061:  ldloca.s   V_4
    IL_0063:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0069:  ldloc.s    V_4
    IL_006b:  br.s       IL_007a

    IL_006d:  ldloca.s   V_3
    IL_006f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0074:  conv.i8
    IL_0075:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_007a:  stloc.2
    IL_007b:  ldarg.1
    IL_007c:  stloc.1
    IL_007d:  ldloca.s   V_2
    IL_007f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0084:  ldloca.s   V_1
    IL_0086:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_008b:  beq.s      IL_0090

    IL_008d:  ldc.i4.0
    IL_008e:  br.s       IL_00a0

    IL_0090:  ldloca.s   V_2
    IL_0092:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0097:  ldloca.s   V_1
    IL_0099:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_009e:  ceq
    IL_00a0:  stloc.s    V_5
    IL_00a2:  ldloc.s    V_5
    IL_00a4:  brfalse.s  IL_00ae

    IL_00a6:  nop
    IL_00a7:  call       void [mscorlib]System.Console::WriteLine()
    IL_00ac:  nop
    IL_00ad:  nop
    IL_00ae:  ret
  } // end of method LiftedOperators::CompareWithImplictCast

  .method public hidebysig static void  CompareWithSignChange(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                              valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       120 (0x78)
    .maxstack  2
    .locals init (bool V_0,
             valuetype [mscorlib]System.Nullable`1<uint32> V_1,
             valuetype [mscorlib]System.Nullable`1<uint32> V_2,
             valuetype [mscorlib]System.Nullable`1<int32> V_3,
             valuetype [mscorlib]System.Nullable`1<uint32> V_4)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.3
    IL_0003:  ldloca.s   V_3
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_0018

    IL_000c:  ldloca.s   V_4
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_0014:  ldloc.s    V_4
    IL_0016:  br.s       IL_0024

    IL_0018:  ldloca.s   V_3
    IL_001a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_0024:  stloc.1
    IL_0025:  ldarg.1
    IL_0026:  stloc.3
    IL_0027:  ldloca.s   V_3
    IL_0029:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_002e:  brtrue.s   IL_003c

    IL_0030:  ldloca.s   V_4
    IL_0032:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_0038:  ldloc.s    V_4
    IL_003a:  br.s       IL_0048

    IL_003c:  ldloca.s   V_3
    IL_003e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0043:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_0048:  stloc.2
    IL_0049:  ldloca.s   V_1
    IL_004b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint32>::GetValueOrDefault()
    IL_0050:  ldloca.s   V_2
    IL_0052:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint32>::GetValueOrDefault()
    IL_0057:  blt.un.s   IL_005c

    IL_0059:  ldc.i4.0
    IL_005a:  br.s       IL_006b

    IL_005c:  ldloca.s   V_1
    IL_005e:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint32>::get_HasValue()
    IL_0063:  ldloca.s   V_2
    IL_0065:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint32>::get_HasValue()
    IL_006a:  and
    IL_006b:  stloc.0
    IL_006c:  ldloc.0
    IL_006d:  brfalse.s  IL_0077

    IL_006f:  nop
    IL_0070:  call       void [mscorlib]System.Console::WriteLine()
    IL_0075:  nop
    IL_0076:  nop
    IL_0077:  ret
  } // end of method LiftedOperators::CompareWithSignChange

  .method public hidebysig static void  StructBasic(valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> a,
                                                    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> b) cil managed
  {
    // Code size       564 (0x234)
    .maxstack  2
    .locals init (bool V_0,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_1,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_2,
             bool V_3,
             bool V_4,
             bool V_5,
             bool V_6,
             bool V_7,
             bool V_8,
             bool V_9,
             bool V_10)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldarg.1
    IL_0004:  stloc.2
    IL_0005:  ldloca.s   V_1
    IL_0007:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_000c:  ldloca.s   V_2
    IL_000e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0013:  beq.s      IL_0018

    IL_0015:  ldc.i4.0
    IL_0016:  br.s       IL_0037

    IL_0018:  ldloca.s   V_1
    IL_001a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_001f:  brtrue.s   IL_0024

    IL_0021:  ldc.i4.1
    IL_0022:  br.s       IL_0037

    IL_0024:  ldloca.s   V_1
    IL_0026:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_002b:  ldloca.s   V_2
    IL_002d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0032:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Equality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  brfalse.s  IL_0043

    IL_003b:  nop
    IL_003c:  call       void [mscorlib]System.Console::WriteLine()
    IL_0041:  nop
    IL_0042:  nop
    IL_0043:  ldarg.0
    IL_0044:  stloc.2
    IL_0045:  ldarg.1
    IL_0046:  stloc.1
    IL_0047:  ldloca.s   V_2
    IL_0049:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_004e:  ldloca.s   V_1
    IL_0050:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0055:  beq.s      IL_005a

    IL_0057:  ldc.i4.1
    IL_0058:  br.s       IL_0079

    IL_005a:  ldloca.s   V_2
    IL_005c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0061:  brtrue.s   IL_0066

    IL_0063:  ldc.i4.0
    IL_0064:  br.s       IL_0079

    IL_0066:  ldloca.s   V_2
    IL_0068:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_006d:  ldloca.s   V_1
    IL_006f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0074:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Inequality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0079:  stloc.3
    IL_007a:  ldloc.3
    IL_007b:  brfalse.s  IL_0085

    IL_007d:  nop
    IL_007e:  call       void [mscorlib]System.Console::WriteLine()
    IL_0083:  nop
    IL_0084:  nop
    IL_0085:  ldarg.0
    IL_0086:  stloc.1
    IL_0087:  ldarg.1
    IL_0088:  stloc.2
    IL_0089:  ldloca.s   V_1
    IL_008b:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0090:  ldloca.s   V_2
    IL_0092:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0097:  and
    IL_0098:  brtrue.s   IL_009d

    IL_009a:  ldc.i4.0
    IL_009b:  br.s       IL_00b0

    IL_009d:  ldloca.s   V_1
    IL_009f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00a4:  ldloca.s   V_2
    IL_00a6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00ab:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_GreaterThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_00b0:  stloc.s    V_4
    IL_00b2:  ldloc.s    V_4
    IL_00b4:  brfalse.s  IL_00be

    IL_00b6:  nop
    IL_00b7:  call       void [mscorlib]System.Console::WriteLine()
    IL_00bc:  nop
    IL_00bd:  nop
    IL_00be:  ldarg.0
    IL_00bf:  stloc.2
    IL_00c0:  ldarg.1
    IL_00c1:  stloc.1
    IL_00c2:  ldloca.s   V_2
    IL_00c4:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00c9:  ldloca.s   V_1
    IL_00cb:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00d0:  and
    IL_00d1:  brtrue.s   IL_00d6

    IL_00d3:  ldc.i4.0
    IL_00d4:  br.s       IL_00e9

    IL_00d6:  ldloca.s   V_2
    IL_00d8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00dd:  ldloca.s   V_1
    IL_00df:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00e4:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_LessThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_00e9:  stloc.s    V_5
    IL_00eb:  ldloc.s    V_5
    IL_00ed:  brfalse.s  IL_00f7

    IL_00ef:  nop
    IL_00f0:  call       void [mscorlib]System.Console::WriteLine()
    IL_00f5:  nop
    IL_00f6:  nop
    IL_00f7:  ldarg.0
    IL_00f8:  stloc.1
    IL_00f9:  ldarg.1
    IL_00fa:  stloc.2
    IL_00fb:  ldloca.s   V_1
    IL_00fd:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0102:  ldloca.s   V_2
    IL_0104:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0109:  and
    IL_010a:  brtrue.s   IL_010f

    IL_010c:  ldc.i4.0
    IL_010d:  br.s       IL_0122

    IL_010f:  ldloca.s   V_1
    IL_0111:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0116:  ldloca.s   V_2
    IL_0118:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_011d:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_GreaterThanOrEqual(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0122:  stloc.s    V_6
    IL_0124:  ldloc.s    V_6
    IL_0126:  brfalse.s  IL_0130

    IL_0128:  nop
    IL_0129:  call       void [mscorlib]System.Console::WriteLine()
    IL_012e:  nop
    IL_012f:  nop
    IL_0130:  ldarg.0
    IL_0131:  stloc.2
    IL_0132:  ldarg.1
    IL_0133:  stloc.1
    IL_0134:  ldloca.s   V_2
    IL_0136:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_013b:  ldloca.s   V_1
    IL_013d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0142:  and
    IL_0143:  brtrue.s   IL_0148

    IL_0145:  ldc.i4.0
    IL_0146:  br.s       IL_015b

    IL_0148:  ldloca.s   V_2
    IL_014a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_014f:  ldloca.s   V_1
    IL_0151:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0156:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_LessThanOrEqual(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                   valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_015b:  stloc.s    V_7
    IL_015d:  ldloc.s    V_7
    IL_015f:  brfalse.s  IL_0169

    IL_0161:  nop
    IL_0162:  call       void [mscorlib]System.Console::WriteLine()
    IL_0167:  nop
    IL_0168:  nop
    IL_0169:  ldarg.0
    IL_016a:  stloc.1
    IL_016b:  ldarg.1
    IL_016c:  stloc.2
    IL_016d:  ldloca.s   V_1
    IL_016f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0174:  ldloca.s   V_2
    IL_0176:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_017b:  beq.s      IL_0180

    IL_017d:  ldc.i4.0
    IL_017e:  br.s       IL_019f

    IL_0180:  ldloca.s   V_1
    IL_0182:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0187:  brtrue.s   IL_018c

    IL_0189:  ldc.i4.1
    IL_018a:  br.s       IL_019f

    IL_018c:  ldloca.s   V_1
    IL_018e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0193:  ldloca.s   V_2
    IL_0195:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_019a:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Equality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_019f:  ldc.i4.0
    IL_01a0:  ceq
    IL_01a2:  stloc.s    V_8
    IL_01a4:  ldloc.s    V_8
    IL_01a6:  brfalse.s  IL_01b0

    IL_01a8:  nop
    IL_01a9:  call       void [mscorlib]System.Console::WriteLine()
    IL_01ae:  nop
    IL_01af:  nop
    IL_01b0:  ldarg.0
    IL_01b1:  stloc.2
    IL_01b2:  ldarg.1
    IL_01b3:  stloc.1
    IL_01b4:  ldloca.s   V_2
    IL_01b6:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01bb:  ldloca.s   V_1
    IL_01bd:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01c2:  beq.s      IL_01c7

    IL_01c4:  ldc.i4.1
    IL_01c5:  br.s       IL_01e6

    IL_01c7:  ldloca.s   V_2
    IL_01c9:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01ce:  brtrue.s   IL_01d3

    IL_01d0:  ldc.i4.0
    IL_01d1:  br.s       IL_01e6

    IL_01d3:  ldloca.s   V_2
    IL_01d5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_01da:  ldloca.s   V_1
    IL_01dc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_01e1:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Inequality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_01e6:  ldc.i4.0
    IL_01e7:  ceq
    IL_01e9:  stloc.s    V_9
    IL_01eb:  ldloc.s    V_9
    IL_01ed:  brfalse.s  IL_01f7

    IL_01ef:  nop
    IL_01f0:  call       void [mscorlib]System.Console::WriteLine()
    IL_01f5:  nop
    IL_01f6:  nop
    IL_01f7:  ldarg.0
    IL_01f8:  stloc.1
    IL_01f9:  ldarg.1
    IL_01fa:  stloc.2
    IL_01fb:  ldloca.s   V_1
    IL_01fd:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0202:  ldloca.s   V_2
    IL_0204:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0209:  and
    IL_020a:  brtrue.s   IL_020f

    IL_020c:  ldc.i4.0
    IL_020d:  br.s       IL_0222

    IL_020f:  ldloca.s   V_1
    IL_0211:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0216:  ldloca.s   V_2
    IL_0218:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_021d:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_GreaterThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0222:  ldc.i4.0
    IL_0223:  ceq
    IL_0225:  stloc.s    V_10
    IL_0227:  ldloc.s    V_10
    IL_0229:  brfalse.s  IL_0233

    IL_022b:  nop
    IL_022c:  call       void [mscorlib]System.Console::WriteLine()
    IL_0231:  nop
    IL_0232:  nop
    IL_0233:  ret
  } // end of method LiftedOperators::StructBasic

  .method public hidebysig static void  StructComplex(valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> a,
                                                      class [mscorlib]System.Func`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> x) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method LiftedOperators::StructComplex

  .method public hidebysig static void  StructValueBasic(valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> a,
                                                         valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> b,
                                                         valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       1825 (0x721)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_0,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_1,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_2,
             valuetype [mscorlib]System.Nullable`1<int32> V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldarg.1
    IL_0004:  stloc.1
    IL_0005:  ldloca.s   V_0
    IL_0007:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_000c:  ldloca.s   V_1
    IL_000e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0013:  beq.s      IL_0018

    IL_0015:  ldc.i4.0
    IL_0016:  br.s       IL_0037

    IL_0018:  ldloca.s   V_0
    IL_001a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_001f:  brtrue.s   IL_0024

    IL_0021:  ldc.i4.1
    IL_0022:  br.s       IL_0037

    IL_0024:  ldloca.s   V_0
    IL_0026:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_002b:  ldloca.s   V_1
    IL_002d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0032:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Equality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0037:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_003c:  nop
    IL_003d:  ldarg.0
    IL_003e:  stloc.1
    IL_003f:  ldarg.1
    IL_0040:  stloc.0
    IL_0041:  ldloca.s   V_1
    IL_0043:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0048:  ldloca.s   V_0
    IL_004a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_004f:  beq.s      IL_0054

    IL_0051:  ldc.i4.1
    IL_0052:  br.s       IL_0073

    IL_0054:  ldloca.s   V_1
    IL_0056:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_005b:  brtrue.s   IL_0060

    IL_005d:  ldc.i4.0
    IL_005e:  br.s       IL_0073

    IL_0060:  ldloca.s   V_1
    IL_0062:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0067:  ldloca.s   V_0
    IL_0069:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_006e:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Inequality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0073:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0078:  nop
    IL_0079:  ldarg.0
    IL_007a:  stloc.0
    IL_007b:  ldarg.1
    IL_007c:  stloc.1
    IL_007d:  ldloca.s   V_0
    IL_007f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0084:  ldloca.s   V_1
    IL_0086:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_008b:  and
    IL_008c:  brtrue.s   IL_0091

    IL_008e:  ldc.i4.0
    IL_008f:  br.s       IL_00a4

    IL_0091:  ldloca.s   V_0
    IL_0093:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0098:  ldloca.s   V_1
    IL_009a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_009f:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_GreaterThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_00a4:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00a9:  nop
    IL_00aa:  ldarg.0
    IL_00ab:  stloc.1
    IL_00ac:  ldarg.1
    IL_00ad:  stloc.0
    IL_00ae:  ldloca.s   V_1
    IL_00b0:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00b5:  ldloca.s   V_0
    IL_00b7:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00bc:  beq.s      IL_00c1

    IL_00be:  ldc.i4.0
    IL_00bf:  br.s       IL_00e0

    IL_00c1:  ldloca.s   V_1
    IL_00c3:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00c8:  brtrue.s   IL_00cd

    IL_00ca:  ldc.i4.1
    IL_00cb:  br.s       IL_00e0

    IL_00cd:  ldloca.s   V_1
    IL_00cf:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00d4:  ldloca.s   V_0
    IL_00d6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00db:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Equality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_00e0:  ldc.i4.0
    IL_00e1:  ceq
    IL_00e3:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00e8:  nop
    IL_00e9:  ldarg.0
    IL_00ea:  stloc.0
    IL_00eb:  ldarg.1
    IL_00ec:  stloc.1
    IL_00ed:  ldloca.s   V_0
    IL_00ef:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00f4:  ldloca.s   V_1
    IL_00f6:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00fb:  beq.s      IL_0100

    IL_00fd:  ldc.i4.1
    IL_00fe:  br.s       IL_011f

    IL_0100:  ldloca.s   V_0
    IL_0102:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0107:  brtrue.s   IL_010c

    IL_0109:  ldc.i4.0
    IL_010a:  br.s       IL_011f

    IL_010c:  ldloca.s   V_0
    IL_010e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0113:  ldloca.s   V_1
    IL_0115:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_011a:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Inequality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_011f:  ldc.i4.0
    IL_0120:  ceq
    IL_0122:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0127:  nop
    IL_0128:  ldarg.0
    IL_0129:  stloc.1
    IL_012a:  ldarg.1
    IL_012b:  stloc.0
    IL_012c:  ldloca.s   V_1
    IL_012e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0133:  ldloca.s   V_0
    IL_0135:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_013a:  and
    IL_013b:  brtrue.s   IL_0140

    IL_013d:  ldc.i4.0
    IL_013e:  br.s       IL_0153

    IL_0140:  ldloca.s   V_1
    IL_0142:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0147:  ldloca.s   V_0
    IL_0149:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_014e:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_GreaterThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0153:  ldc.i4.0
    IL_0154:  ceq
    IL_0156:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_015b:  nop
    IL_015c:  ldarg.0
    IL_015d:  stloc.0
    IL_015e:  ldarg.1
    IL_015f:  stloc.1
    IL_0160:  ldloca.s   V_0
    IL_0162:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0167:  ldloca.s   V_1
    IL_0169:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_016e:  and
    IL_016f:  brtrue.s   IL_017c

    IL_0171:  ldloca.s   V_2
    IL_0173:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0179:  ldloc.2
    IL_017a:  br.s       IL_0194

    IL_017c:  ldloca.s   V_0
    IL_017e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0183:  ldloca.s   V_1
    IL_0185:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_018a:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Addition(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_018f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0194:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0199:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_019e:  nop
    IL_019f:  ldarg.0
    IL_01a0:  stloc.1
    IL_01a1:  ldarg.1
    IL_01a2:  stloc.0
    IL_01a3:  ldloca.s   V_1
    IL_01a5:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01aa:  ldloca.s   V_0
    IL_01ac:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01b1:  and
    IL_01b2:  brtrue.s   IL_01bf

    IL_01b4:  ldloca.s   V_2
    IL_01b6:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_01bc:  ldloc.2
    IL_01bd:  br.s       IL_01d7

    IL_01bf:  ldloca.s   V_1
    IL_01c1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_01c6:  ldloca.s   V_0
    IL_01c8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_01cd:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Subtraction(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_01d2:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_01d7:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_01dc:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01e1:  nop
    IL_01e2:  ldarg.0
    IL_01e3:  stloc.0
    IL_01e4:  ldarg.1
    IL_01e5:  stloc.1
    IL_01e6:  ldloca.s   V_0
    IL_01e8:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01ed:  ldloca.s   V_1
    IL_01ef:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01f4:  and
    IL_01f5:  brtrue.s   IL_0202

    IL_01f7:  ldloca.s   V_2
    IL_01f9:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_01ff:  ldloc.2
    IL_0200:  br.s       IL_021a

    IL_0202:  ldloca.s   V_0
    IL_0204:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0209:  ldloca.s   V_1
    IL_020b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0210:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Multiply(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0215:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_021a:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_021f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0224:  nop
    IL_0225:  ldarg.0
    IL_0226:  stloc.1
    IL_0227:  ldarg.1
    IL_0228:  stloc.0
    IL_0229:  ldloca.s   V_1
    IL_022b:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0230:  ldloca.s   V_0
    IL_0232:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0237:  and
    IL_0238:  brtrue.s   IL_0245

    IL_023a:  ldloca.s   V_2
    IL_023c:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0242:  ldloc.2
    IL_0243:  br.s       IL_025d

    IL_0245:  ldloca.s   V_1
    IL_0247:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_024c:  ldloca.s   V_0
    IL_024e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0253:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Division(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0258:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_025d:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0262:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0267:  nop
    IL_0268:  ldarg.0
    IL_0269:  stloc.0
    IL_026a:  ldarg.1
    IL_026b:  stloc.1
    IL_026c:  ldloca.s   V_0
    IL_026e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0273:  ldloca.s   V_1
    IL_0275:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_027a:  and
    IL_027b:  brtrue.s   IL_0288

    IL_027d:  ldloca.s   V_2
    IL_027f:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0285:  ldloc.2
    IL_0286:  br.s       IL_02a0

    IL_0288:  ldloca.s   V_0
    IL_028a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_028f:  ldloca.s   V_1
    IL_0291:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0296:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Modulus(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                 valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_029b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_02a0:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_02a5:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02aa:  nop
    IL_02ab:  ldarg.0
    IL_02ac:  stloc.1
    IL_02ad:  ldarg.1
    IL_02ae:  stloc.0
    IL_02af:  ldloca.s   V_1
    IL_02b1:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_02b6:  ldloca.s   V_0
    IL_02b8:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_02bd:  and
    IL_02be:  brtrue.s   IL_02cb

    IL_02c0:  ldloca.s   V_2
    IL_02c2:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_02c8:  ldloc.2
    IL_02c9:  br.s       IL_02e3

    IL_02cb:  ldloca.s   V_1
    IL_02cd:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_02d2:  ldloca.s   V_0
    IL_02d4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_02d9:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                    valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_02de:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_02e3:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_02e8:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02ed:  nop
    IL_02ee:  ldarg.0
    IL_02ef:  stloc.0
    IL_02f0:  ldarg.1
    IL_02f1:  stloc.1
    IL_02f2:  ldloca.s   V_0
    IL_02f4:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_02f9:  ldloca.s   V_1
    IL_02fb:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0300:  and
    IL_0301:  brtrue.s   IL_030e

    IL_0303:  ldloca.s   V_2
    IL_0305:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_030b:  ldloc.2
    IL_030c:  br.s       IL_0326

    IL_030e:  ldloca.s   V_0
    IL_0310:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0315:  ldloca.s   V_1
    IL_0317:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_031c:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                   valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0321:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0326:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_032b:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0330:  nop
    IL_0331:  ldarg.0
    IL_0332:  stloc.1
    IL_0333:  ldarg.1
    IL_0334:  stloc.0
    IL_0335:  ldloca.s   V_1
    IL_0337:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_033c:  ldloca.s   V_0
    IL_033e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0343:  and
    IL_0344:  brtrue.s   IL_0351

    IL_0346:  ldloca.s   V_2
    IL_0348:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_034e:  ldloc.2
    IL_034f:  br.s       IL_0369

    IL_0351:  ldloca.s   V_1
    IL_0353:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0358:  ldloca.s   V_0
    IL_035a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_035f:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_ExclusiveOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0364:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0369:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_036e:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0373:  nop
    IL_0374:  ldarg.0
    IL_0375:  stloc.0
    IL_0376:  ldarg.2
    IL_0377:  stloc.3
    IL_0378:  ldloca.s   V_0
    IL_037a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_037f:  ldloca.s   V_3
    IL_0381:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0386:  and
    IL_0387:  brtrue.s   IL_0394

    IL_0389:  ldloca.s   V_1
    IL_038b:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0391:  ldloc.1
    IL_0392:  br.s       IL_03ac

    IL_0394:  ldloca.s   V_0
    IL_0396:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_039b:  ldloca.s   V_3
    IL_039d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03a2:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_LeftShift(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                   int32)
    IL_03a7:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_03ac:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_03b1:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_03b6:  nop
    IL_03b7:  ldarg.0
    IL_03b8:  stloc.0
    IL_03b9:  ldarg.2
    IL_03ba:  stloc.3
    IL_03bb:  ldloca.s   V_0
    IL_03bd:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_03c2:  ldloca.s   V_3
    IL_03c4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03c9:  and
    IL_03ca:  brtrue.s   IL_03d7

    IL_03cc:  ldloca.s   V_1
    IL_03ce:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_03d4:  ldloc.1
    IL_03d5:  br.s       IL_03ef

    IL_03d7:  ldloca.s   V_0
    IL_03d9:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_03de:  ldloca.s   V_3
    IL_03e0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03e5:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_RightShift(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                    int32)
    IL_03ea:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_03ef:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_03f4:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_03f9:  nop
    IL_03fa:  ldarg.0
    IL_03fb:  stloc.0
    IL_03fc:  ldloca.s   V_0
    IL_03fe:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0403:  brtrue.s   IL_0408

    IL_0405:  ldarg.1
    IL_0406:  br.s       IL_0409

    IL_0408:  ldloc.0
    IL_0409:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_040e:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0413:  nop
    IL_0414:  ldarg.0
    IL_0415:  stloc.0
    IL_0416:  ldloca.s   V_0
    IL_0418:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_041d:  brtrue.s   IL_042a

    IL_041f:  ldloca.s   V_1
    IL_0421:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0427:  ldloc.1
    IL_0428:  br.s       IL_043b

    IL_042a:  ldloca.s   V_0
    IL_042c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0431:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_UnaryPlus(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0436:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_043b:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0440:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0445:  nop
    IL_0446:  ldarg.0
    IL_0447:  stloc.0
    IL_0448:  ldloca.s   V_0
    IL_044a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_044f:  brtrue.s   IL_045c

    IL_0451:  ldloca.s   V_1
    IL_0453:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0459:  ldloc.1
    IL_045a:  br.s       IL_046d

    IL_045c:  ldloca.s   V_0
    IL_045e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0463:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_UnaryNegation(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0468:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_046d:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0472:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0477:  nop
    IL_0478:  ldarg.0
    IL_0479:  stloc.0
    IL_047a:  ldloca.s   V_0
    IL_047c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0481:  brtrue.s   IL_048e

    IL_0483:  ldloca.s   V_1
    IL_0485:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_048b:  ldloc.1
    IL_048c:  br.s       IL_049f

    IL_048e:  ldloca.s   V_0
    IL_0490:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0495:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_LogicalNot(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_049a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_049f:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_04a4:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_04a9:  nop
    IL_04aa:  ldarg.0
    IL_04ab:  stloc.0
    IL_04ac:  ldloca.s   V_0
    IL_04ae:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_04b3:  brtrue.s   IL_04c0

    IL_04b5:  ldloca.s   V_1
    IL_04b7:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_04bd:  ldloc.1
    IL_04be:  br.s       IL_04d1

    IL_04c0:  ldloca.s   V_0
    IL_04c2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_04c7:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_OnesComplement(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_04cc:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_04d1:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_04d6:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_04db:  nop
    IL_04dc:  ldarg.0
    IL_04dd:  stloc.0
    IL_04de:  ldarg.1
    IL_04df:  stloc.1
    IL_04e0:  ldloca.s   V_0
    IL_04e2:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_04e7:  ldloca.s   V_1
    IL_04e9:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_04ee:  and
    IL_04ef:  brtrue.s   IL_04fc

    IL_04f1:  ldloca.s   V_2
    IL_04f3:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_04f9:  ldloc.2
    IL_04fa:  br.s       IL_0514

    IL_04fc:  ldloca.s   V_0
    IL_04fe:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0503:  ldloca.s   V_1
    IL_0505:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_050a:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Addition(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_050f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0514:  starg.s    a
    IL_0516:  ldarg.0
    IL_0517:  stloc.1
    IL_0518:  ldarg.1
    IL_0519:  stloc.0
    IL_051a:  ldloca.s   V_1
    IL_051c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0521:  ldloca.s   V_0
    IL_0523:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0528:  and
    IL_0529:  brtrue.s   IL_0536

    IL_052b:  ldloca.s   V_2
    IL_052d:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0533:  ldloc.2
    IL_0534:  br.s       IL_054e

    IL_0536:  ldloca.s   V_1
    IL_0538:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_053d:  ldloca.s   V_0
    IL_053f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0544:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Subtraction(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0549:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_054e:  starg.s    a
    IL_0550:  ldarg.0
    IL_0551:  stloc.0
    IL_0552:  ldarg.1
    IL_0553:  stloc.1
    IL_0554:  ldloca.s   V_0
    IL_0556:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_055b:  ldloca.s   V_1
    IL_055d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0562:  and
    IL_0563:  brtrue.s   IL_0570

    IL_0565:  ldloca.s   V_2
    IL_0567:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_056d:  ldloc.2
    IL_056e:  br.s       IL_0588

    IL_0570:  ldloca.s   V_0
    IL_0572:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0577:  ldloca.s   V_1
    IL_0579:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_057e:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Multiply(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0583:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0588:  starg.s    a
    IL_058a:  ldarg.0
    IL_058b:  stloc.1
    IL_058c:  ldarg.1
    IL_058d:  stloc.0
    IL_058e:  ldloca.s   V_1
    IL_0590:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0595:  ldloca.s   V_0
    IL_0597:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_059c:  and
    IL_059d:  brtrue.s   IL_05aa

    IL_059f:  ldloca.s   V_2
    IL_05a1:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_05a7:  ldloc.2
    IL_05a8:  br.s       IL_05c2

    IL_05aa:  ldloca.s   V_1
    IL_05ac:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_05b1:  ldloca.s   V_0
    IL_05b3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_05b8:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Division(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_05bd:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_05c2:  starg.s    a
    IL_05c4:  ldarg.0
    IL_05c5:  stloc.0
    IL_05c6:  ldarg.1
    IL_05c7:  stloc.1
    IL_05c8:  ldloca.s   V_0
    IL_05ca:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_05cf:  ldloca.s   V_1
    IL_05d1:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_05d6:  and
    IL_05d7:  brtrue.s   IL_05e4

    IL_05d9:  ldloca.s   V_2
    IL_05db:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_05e1:  ldloc.2
    IL_05e2:  br.s       IL_05fc

    IL_05e4:  ldloca.s   V_0
    IL_05e6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_05eb:  ldloca.s   V_1
    IL_05ed:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_05f2:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Modulus(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                 valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_05f7:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_05fc:  starg.s    a
    IL_05fe:  ldarg.0
    IL_05ff:  stloc.1
    IL_0600:  ldarg.1
    IL_0601:  stloc.0
    IL_0602:  ldloca.s   V_1
    IL_0604:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0609:  ldloca.s   V_0
    IL_060b:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0610:  and
    IL_0611:  brtrue.s   IL_061e

    IL_0613:  ldloca.s   V_2
    IL_0615:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_061b:  ldloc.2
    IL_061c:  br.s       IL_0636

    IL_061e:  ldloca.s   V_1
    IL_0620:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0625:  ldloca.s   V_0
    IL_0627:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_062c:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                    valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0631:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0636:  starg.s    a
    IL_0638:  ldarg.0
    IL_0639:  stloc.0
    IL_063a:  ldarg.1
    IL_063b:  stloc.1
    IL_063c:  ldloca.s   V_0
    IL_063e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0643:  ldloca.s   V_1
    IL_0645:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_064a:  and
    IL_064b:  brtrue.s   IL_0658

    IL_064d:  ldloca.s   V_2
    IL_064f:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0655:  ldloc.2
    IL_0656:  br.s       IL_0670

    IL_0658:  ldloca.s   V_0
    IL_065a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_065f:  ldloca.s   V_1
    IL_0661:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0666:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                   valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_066b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0670:  starg.s    a
    IL_0672:  ldarg.0
    IL_0673:  stloc.1
    IL_0674:  ldarg.1
    IL_0675:  stloc.0
    IL_0676:  ldloca.s   V_1
    IL_0678:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_067d:  ldloca.s   V_0
    IL_067f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0684:  and
    IL_0685:  brtrue.s   IL_0692

    IL_0687:  ldloca.s   V_2
    IL_0689:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_068f:  ldloc.2
    IL_0690:  br.s       IL_06aa

    IL_0692:  ldloca.s   V_1
    IL_0694:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0699:  ldloca.s   V_0
    IL_069b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_06a0:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_ExclusiveOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_06a5:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_06aa:  starg.s    a
    IL_06ac:  ldarg.0
    IL_06ad:  stloc.0
    IL_06ae:  ldarg.2
    IL_06af:  stloc.3
    IL_06b0:  ldloca.s   V_0
    IL_06b2:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_06b7:  ldloca.s   V_3
    IL_06b9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_06be:  and
    IL_06bf:  brtrue.s   IL_06cc

    IL_06c1:  ldloca.s   V_1
    IL_06c3:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_06c9:  ldloc.1
    IL_06ca:  br.s       IL_06e4

    IL_06cc:  ldloca.s   V_0
    IL_06ce:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_06d3:  ldloca.s   V_3
    IL_06d5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_06da:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_LeftShift(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                   int32)
    IL_06df:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_06e4:  starg.s    a
    IL_06e6:  ldarg.0
    IL_06e7:  stloc.0
    IL_06e8:  ldarg.2
    IL_06e9:  stloc.3
    IL_06ea:  ldloca.s   V_0
    IL_06ec:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_06f1:  ldloca.s   V_3
    IL_06f3:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_06f8:  and
    IL_06f9:  brtrue.s   IL_0706

    IL_06fb:  ldloca.s   V_1
    IL_06fd:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0703:  ldloc.1
    IL_0704:  br.s       IL_071e

    IL_0706:  ldloca.s   V_0
    IL_0708:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_070d:  ldloca.s   V_3
    IL_070f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0714:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_RightShift(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                    int32)
    IL_0719:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_071e:  starg.s    a
    IL_0720:  ret
  } // end of method LiftedOperators::StructValueBasic

  .method public hidebysig static void  StructValueComplex(valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> a,
                                                           class [mscorlib]System.Func`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> x,
                                                           class [mscorlib]System.Func`1<int32> i) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method LiftedOperators::StructValueComplex

  .method public hidebysig static bool  RetEq(valuetype [mscorlib]System.Nullable`1<int32> a,
                                              valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldarg.1
    IL_0004:  stloc.1
    IL_0005:  ldloca.s   V_0
    IL_0007:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000c:  ldloca.s   V_1
    IL_000e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0013:  beq.s      IL_0018

    IL_0015:  ldc.i4.0
    IL_0016:  br.s       IL_0028

    IL_0018:  ldloca.s   V_0
    IL_001a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001f:  ldloca.s   V_1
    IL_0021:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0026:  ceq
    IL_0028:  stloc.2
    IL_0029:  br.s       IL_002b

    IL_002b:  ldloc.2
    IL_002c:  ret
  } // end of method LiftedOperators::RetEq

  .method public hidebysig static bool  RetEqConv(valuetype [mscorlib]System.Nullable`1<int64> a,
                                                  valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       81 (0x51)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int64> V_0,
             valuetype [mscorlib]System.Nullable`1<int64> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             valuetype [mscorlib]System.Nullable`1<int64> V_3,
             bool V_4)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldarg.1
    IL_0004:  stloc.2
    IL_0005:  ldloca.s   V_2
    IL_0007:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000c:  brtrue.s   IL_0019

    IL_000e:  ldloca.s   V_3
    IL_0010:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0016:  ldloc.3
    IL_0017:  br.s       IL_0026

    IL_0019:  ldloca.s   V_2
    IL_001b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0020:  conv.i8
    IL_0021:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0026:  stloc.1
    IL_0027:  ldloca.s   V_0
    IL_0029:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_002e:  ldloca.s   V_1
    IL_0030:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0035:  beq.s      IL_003a

    IL_0037:  ldc.i4.0
    IL_0038:  br.s       IL_004a

    IL_003a:  ldloca.s   V_0
    IL_003c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0041:  ldloca.s   V_1
    IL_0043:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0048:  ceq
    IL_004a:  stloc.s    V_4
    IL_004c:  br.s       IL_004e

    IL_004e:  ldloc.s    V_4
    IL_0050:  ret
  } // end of method LiftedOperators::RetEqConv

  .method public hidebysig static bool  RetEqConst(valuetype [mscorlib]System.Nullable`1<int64> a) cil managed
  {
    // Code size       32 (0x20)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int64> V_0,
             int64 V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.s   10
    IL_0005:  conv.i8
    IL_0006:  stloc.1
    IL_0007:  ldloca.s   V_0
    IL_0009:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_000e:  ldloc.1
    IL_000f:  beq.s      IL_0014

    IL_0011:  ldc.i4.0
    IL_0012:  br.s       IL_001b

    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_001b:  stloc.2
    IL_001c:  br.s       IL_001e

    IL_001e:  ldloc.2
    IL_001f:  ret
  } // end of method LiftedOperators::RetEqConst

  .method public hidebysig static bool  RetIneqConst(valuetype [mscorlib]System.Nullable`1<int64> a) cil managed
  {
    // Code size       35 (0x23)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int64> V_0,
             int64 V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.s   10
    IL_0005:  conv.i8
    IL_0006:  stloc.1
    IL_0007:  ldloca.s   V_0
    IL_0009:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_000e:  ldloc.1
    IL_000f:  beq.s      IL_0014

    IL_0011:  ldc.i4.1
    IL_0012:  br.s       IL_001e

    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_001b:  ldc.i4.0
    IL_001c:  ceq
    IL_001e:  stloc.2
    IL_001f:  br.s       IL_0021

    IL_0021:  ldloc.2
    IL_0022:  ret
  } // end of method LiftedOperators::RetIneqConst

  .method public hidebysig static bool  RetLt(valuetype [mscorlib]System.Nullable`1<int32> a,
                                              valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldarg.1
    IL_0004:  stloc.1
    IL_0005:  ldloca.s   V_0
    IL_0007:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000c:  ldloca.s   V_1
    IL_000e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0013:  blt.s      IL_0018

    IL_0015:  ldc.i4.0
    IL_0016:  br.s       IL_0027

    IL_0018:  ldloca.s   V_0
    IL_001a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001f:  ldloca.s   V_1
    IL_0021:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0026:  and
    IL_0027:  stloc.2
    IL_0028:  br.s       IL_002a

    IL_002a:  ldloc.2
    IL_002b:  ret
  } // end of method LiftedOperators::RetLt

  .method public hidebysig static bool  RetLtConst(valuetype [mscorlib]System.Nullable`1<int32> a) cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             int32 V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.s   10
    IL_0005:  stloc.1
    IL_0006:  ldloca.s   V_0
    IL_0008:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000d:  ldloc.1
    IL_000e:  blt.s      IL_0013

    IL_0010:  ldc.i4.0
    IL_0011:  br.s       IL_001a

    IL_0013:  ldloca.s   V_0
    IL_0015:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001a:  stloc.2
    IL_001b:  br.s       IL_001d

    IL_001d:  ldloc.2
    IL_001e:  ret
  } // end of method LiftedOperators::RetLtConst

  .method public hidebysig static bool  RetLtConv(valuetype [mscorlib]System.Nullable`1<int64> a,
                                                  valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       80 (0x50)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int64> V_0,
             valuetype [mscorlib]System.Nullable`1<int64> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             valuetype [mscorlib]System.Nullable`1<int64> V_3,
             bool V_4)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldarg.1
    IL_0004:  stloc.2
    IL_0005:  ldloca.s   V_2
    IL_0007:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000c:  brtrue.s   IL_0019

    IL_000e:  ldloca.s   V_3
    IL_0010:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0016:  ldloc.3
    IL_0017:  br.s       IL_0026

    IL_0019:  ldloca.s   V_2
    IL_001b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0020:  conv.i8
    IL_0021:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0026:  stloc.1
    IL_0027:  ldloca.s   V_0
    IL_0029:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_002e:  ldloca.s   V_1
    IL_0030:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0035:  blt.s      IL_003a

    IL_0037:  ldc.i4.0
    IL_0038:  br.s       IL_0049

    IL_003a:  ldloca.s   V_0
    IL_003c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0041:  ldloca.s   V_1
    IL_0043:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0048:  and
    IL_0049:  stloc.s    V_4
    IL_004b:  br.s       IL_004d

    IL_004d:  ldloc.s    V_4
    IL_004f:  ret
  } // end of method LiftedOperators::RetLtConv

  .method public hidebysig static bool  RetNotLt(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                 valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       47 (0x2f)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldarg.1
    IL_0004:  stloc.1
    IL_0005:  ldloca.s   V_0
    IL_0007:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000c:  ldloca.s   V_1
    IL_000e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0013:  blt.s      IL_0018

    IL_0015:  ldc.i4.0
    IL_0016:  br.s       IL_0027

    IL_0018:  ldloca.s   V_0
    IL_001a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001f:  ldloca.s   V_1
    IL_0021:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0026:  and
    IL_0027:  ldc.i4.0
    IL_0028:  ceq
    IL_002a:  stloc.2
    IL_002b:  br.s       IL_002d

    IL_002d:  ldloc.2
    IL_002e:  ret
  } // end of method LiftedOperators::RetNotLt

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedOperators

.class public sequential ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS
       extends [mscorlib]System.ValueType
{
  .pack 0
  .size 1
  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_UnaryPlus(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_UnaryPlus

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_UnaryNegation(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_UnaryNegation

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_LogicalNot(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_LogicalNot

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_OnesComplement(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_OnesComplement

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_Increment(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_Increment

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_Decrement(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_Decrement

  .method public hidebysig specialname static 
          int32  op_Explicit(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_Explicit

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_Addition(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_Addition

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_Subtraction(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                         valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_Subtraction

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_Multiply(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_Multiply

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_Division(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_Division

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_Modulus(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_Modulus

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                        valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_BitwiseAnd

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_BitwiseOr

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_ExclusiveOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                         valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_ExclusiveOr

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_LeftShift(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                       int32 b) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_LeftShift

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_RightShift(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                        int32 b) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_RightShift

  .method public hidebysig specialname static 
          bool  op_Equality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_Equality

  .method public hidebysig specialname static 
          bool  op_Inequality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_Inequality

  .method public hidebysig specialname static 
          bool  op_LessThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_LessThan

  .method public hidebysig specialname static 
          bool  op_LessThanOrEqual(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                                   valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_LessThanOrEqual

  .method public hidebysig specialname static 
          bool  op_GreaterThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_GreaterThan

  .method public hidebysig specialname static 
          bool  op_GreaterThanOrEqual(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                                      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::op_GreaterThanOrEqual

  .method public hidebysig virtual instance bool 
          Equals(object obj) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  throw
  } // end of method TS::GetHashCode

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedImplicitConversions
       extends [mscorlib]System.Object
{
  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int32> 
          ExtendI4(valuetype [mscorlib]System.Nullable`1<uint8> b) cil managed
  {
    // Code size       40 (0x28)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<uint8> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint8>::get_HasValue()
    IL_000a:  brtrue.s   IL_0017

    IL_000c:  ldloca.s   V_1
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0014:  ldloc.1
    IL_0015:  br.s       IL_0023

    IL_0017:  ldloca.s   V_0
    IL_0019:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint8>::GetValueOrDefault()
    IL_001e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0023:  stloc.2
    IL_0024:  br.s       IL_0026

    IL_0026:  ldloc.2
    IL_0027:  ret
  } // end of method LiftedImplicitConversions::ExtendI4

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int32> 
          ExtendToI4(valuetype [mscorlib]System.Nullable`1<int8> b) cil managed
  {
    // Code size       40 (0x28)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int8> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int8>::get_HasValue()
    IL_000a:  brtrue.s   IL_0017

    IL_000c:  ldloca.s   V_1
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0014:  ldloc.1
    IL_0015:  br.s       IL_0023

    IL_0017:  ldloca.s   V_0
    IL_0019:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int8>::GetValueOrDefault()
    IL_001e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0023:  stloc.2
    IL_0024:  br.s       IL_0026

    IL_0026:  ldloc.2
    IL_0027:  ret
  } // end of method LiftedImplicitConversions::ExtendToI4

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int64> 
          ExtendI8(valuetype [mscorlib]System.Nullable`1<uint8> b) cil managed
  {
    // Code size       41 (0x29)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<uint8> V_0,
             valuetype [mscorlib]System.Nullable`1<int64> V_1,
             valuetype [mscorlib]System.Nullable`1<int64> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint8>::get_HasValue()
    IL_000a:  brtrue.s   IL_0017

    IL_000c:  ldloca.s   V_1
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0014:  ldloc.1
    IL_0015:  br.s       IL_0024

    IL_0017:  ldloca.s   V_0
    IL_0019:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint8>::GetValueOrDefault()
    IL_001e:  conv.u8
    IL_001f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0024:  stloc.2
    IL_0025:  br.s       IL_0027

    IL_0027:  ldloc.2
    IL_0028:  ret
  } // end of method LiftedImplicitConversions::ExtendI8

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int64> 
          ExtendToI8(valuetype [mscorlib]System.Nullable`1<int8> b) cil managed
  {
    // Code size       41 (0x29)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int8> V_0,
             valuetype [mscorlib]System.Nullable`1<int64> V_1,
             valuetype [mscorlib]System.Nullable`1<int64> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int8>::get_HasValue()
    IL_000a:  brtrue.s   IL_0017

    IL_000c:  ldloca.s   V_1
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0014:  ldloc.1
    IL_0015:  br.s       IL_0024

    IL_0017:  ldloca.s   V_0
    IL_0019:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int8>::GetValueOrDefault()
    IL_001e:  conv.i8
    IL_001f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0024:  stloc.2
    IL_0025:  br.s       IL_0027

    IL_0027:  ldloc.2
    IL_0028:  ret
  } // end of method LiftedImplicitConversions::ExtendToI8

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int64> 
          ExtendI8(valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       41 (0x29)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int64> V_1,
             valuetype [mscorlib]System.Nullable`1<int64> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_0017

    IL_000c:  ldloca.s   V_1
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0014:  ldloc.1
    IL_0015:  br.s       IL_0024

    IL_0017:  ldloca.s   V_0
    IL_0019:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001e:  conv.i8
    IL_001f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0024:  stloc.2
    IL_0025:  br.s       IL_0027

    IL_0027:  ldloc.2
    IL_0028:  ret
  } // end of method LiftedImplicitConversions::ExtendI8

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int64> 
          ExtendToI8(valuetype [mscorlib]System.Nullable`1<uint32> b) cil managed
  {
    // Code size       41 (0x29)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<uint32> V_0,
             valuetype [mscorlib]System.Nullable`1<int64> V_1,
             valuetype [mscorlib]System.Nullable`1<int64> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint32>::get_HasValue()
    IL_000a:  brtrue.s   IL_0017

    IL_000c:  ldloca.s   V_1
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0014:  ldloc.1
    IL_0015:  br.s       IL_0024

    IL_0017:  ldloca.s   V_0
    IL_0019:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint32>::GetValueOrDefault()
    IL_001e:  conv.u8
    IL_001f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0024:  stloc.2
    IL_0025:  br.s       IL_0027

    IL_0027:  ldloc.2
    IL_0028:  ret
  } // end of method LiftedImplicitConversions::ExtendToI8

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int64> 
          AfterArithmetic(valuetype [mscorlib]System.Nullable`1<uint32> b) cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  2
    .locals init (uint32 V_0,
             valuetype [mscorlib]System.Nullable`1<uint32> V_1,
             valuetype [mscorlib]System.Nullable`1<int64> V_2,
             valuetype [mscorlib]System.Nullable`1<int64> V_3)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   100
    IL_0003:  stloc.0
    IL_0004:  ldarg.1
    IL_0005:  stloc.1
    IL_0006:  ldloca.s   V_1
    IL_0008:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint32>::get_HasValue()
    IL_000d:  brtrue.s   IL_001a

    IL_000f:  ldloca.s   V_2
    IL_0011:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0017:  ldloc.2
    IL_0018:  br.s       IL_0029

    IL_001a:  ldloc.0
    IL_001b:  ldloca.s   V_1
    IL_001d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint32>::GetValueOrDefault()
    IL_0022:  add
    IL_0023:  conv.u8
    IL_0024:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0029:  stloc.3
    IL_002a:  br.s       IL_002c

    IL_002c:  ldloc.3
    IL_002d:  ret
  } // end of method LiftedImplicitConversions::AfterArithmetic

  .method public hidebysig static valuetype [mscorlib]System.Nullable`1<int64> 
          InArithmetic3(valuetype [mscorlib]System.Nullable`1<int32> a,
                        valuetype [mscorlib]System.Nullable`1<int64> b,
                        valuetype [mscorlib]System.Nullable`1<int32> c,
                        int64 d) cil managed
  {
    // Code size       224 (0xe0)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int64> V_0,
             int64 V_1,
             valuetype [mscorlib]System.Nullable`1<int64> V_2,
             valuetype [mscorlib]System.Nullable`1<int64> V_3,
             valuetype [mscorlib]System.Nullable`1<int64> V_4,
             valuetype [mscorlib]System.Nullable`1<int64> V_5,
             valuetype [mscorlib]System.Nullable`1<int32> V_6,
             valuetype [mscorlib]System.Nullable`1<int64> V_7,
             valuetype [mscorlib]System.Nullable`1<int64> V_8)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.s    V_6
    IL_0004:  ldloca.s   V_6
    IL_0006:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000b:  brtrue.s   IL_0019

    IL_000d:  ldloca.s   V_7
    IL_000f:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0015:  ldloc.s    V_7
    IL_0017:  br.s       IL_0026

    IL_0019:  ldloca.s   V_6
    IL_001b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0020:  conv.i8
    IL_0021:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0026:  stloc.s    V_4
    IL_0028:  ldarg.1
    IL_0029:  stloc.s    V_5
    IL_002b:  ldloca.s   V_4
    IL_002d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0032:  ldloca.s   V_5
    IL_0034:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0039:  and
    IL_003a:  brtrue.s   IL_0048

    IL_003c:  ldloca.s   V_7
    IL_003e:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0044:  ldloc.s    V_7
    IL_0046:  br.s       IL_005c

    IL_0048:  ldloca.s   V_4
    IL_004a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_004f:  ldloca.s   V_5
    IL_0051:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0056:  add
    IL_0057:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_005c:  stloc.2
    IL_005d:  ldarg.2
    IL_005e:  stloc.s    V_6
    IL_0060:  ldloca.s   V_6
    IL_0062:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0067:  brtrue.s   IL_0075

    IL_0069:  ldloca.s   V_5
    IL_006b:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0071:  ldloc.s    V_5
    IL_0073:  br.s       IL_0082

    IL_0075:  ldloca.s   V_6
    IL_0077:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_007c:  conv.i8
    IL_007d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0082:  stloc.3
    IL_0083:  ldloca.s   V_2
    IL_0085:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_008a:  ldloca.s   V_3
    IL_008c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0091:  and
    IL_0092:  brtrue.s   IL_00a0

    IL_0094:  ldloca.s   V_5
    IL_0096:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_009c:  ldloc.s    V_5
    IL_009e:  br.s       IL_00b4

    IL_00a0:  ldloca.s   V_2
    IL_00a2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_00a7:  ldloca.s   V_3
    IL_00a9:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_00ae:  add
    IL_00af:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_00b4:  stloc.0
    IL_00b5:  ldarg.3
    IL_00b6:  stloc.1
    IL_00b7:  ldloca.s   V_0
    IL_00b9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_00be:  brtrue.s   IL_00cb

    IL_00c0:  ldloca.s   V_3
    IL_00c2:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_00c8:  ldloc.3
    IL_00c9:  br.s       IL_00d9

    IL_00cb:  ldloca.s   V_0
    IL_00cd:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_00d2:  ldloc.1
    IL_00d3:  add
    IL_00d4:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_00d9:  stloc.s    V_8
    IL_00db:  br.s       IL_00dd

    IL_00dd:  ldloc.s    V_8
    IL_00df:  ret
  } // end of method LiftedImplicitConversions::InArithmetic3

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method LiftedImplicitConversions::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedImplicitConversions

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions
       extends [mscorlib]System.Object
{
  .method private hidebysig static void  Print<valuetype .ctor ([mscorlib]System.ValueType) T>(valuetype [mscorlib]System.Nullable`1<!!T> x) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  box        valuetype [mscorlib]System.Nullable`1<!!T>
    IL_0007:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_000c:  nop
    IL_000d:  ret
  } // end of method LiftedExplicitConversions::Print

  .method public hidebysig static void  UncheckedCasts(valuetype [mscorlib]System.Nullable`1<int32> i4,
                                                       valuetype [mscorlib]System.Nullable`1<int64> i8,
                                                       valuetype [mscorlib]System.Nullable`1<float32> f) cil managed
  {
    // Code size       208 (0xd0)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<uint8> V_1,
             valuetype [mscorlib]System.Nullable`1<int16> V_2,
             valuetype [mscorlib]System.Nullable`1<uint32> V_3,
             valuetype [mscorlib]System.Nullable`1<int64> V_4,
             valuetype [mscorlib]System.Nullable`1<float32> V_5)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_0017

    IL_000c:  ldloca.s   V_1
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<uint8>
    IL_0014:  ldloc.1
    IL_0015:  br.s       IL_0024

    IL_0017:  ldloca.s   V_0
    IL_0019:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001e:  conv.u1
    IL_001f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint8>::.ctor(!0)
    IL_0024:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint8>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_0029:  nop
    IL_002a:  ldarg.0
    IL_002b:  stloc.0
    IL_002c:  ldloca.s   V_0
    IL_002e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0033:  brtrue.s   IL_0040

    IL_0035:  ldloca.s   V_2
    IL_0037:  initobj    valuetype [mscorlib]System.Nullable`1<int16>
    IL_003d:  ldloc.2
    IL_003e:  br.s       IL_004d

    IL_0040:  ldloca.s   V_0
    IL_0042:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0047:  conv.i2
    IL_0048:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int16>::.ctor(!0)
    IL_004d:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<int16>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_0052:  nop
    IL_0053:  ldarg.0
    IL_0054:  stloc.0
    IL_0055:  ldloca.s   V_0
    IL_0057:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_005c:  brtrue.s   IL_0069

    IL_005e:  ldloca.s   V_3
    IL_0060:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_0066:  ldloc.3
    IL_0067:  br.s       IL_0075

    IL_0069:  ldloca.s   V_0
    IL_006b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0070:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_0075:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_007a:  nop
    IL_007b:  ldarg.1
    IL_007c:  stloc.s    V_4
    IL_007e:  ldloca.s   V_4
    IL_0080:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0085:  brtrue.s   IL_0092

    IL_0087:  ldloca.s   V_3
    IL_0089:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_008f:  ldloc.3
    IL_0090:  br.s       IL_009f

    IL_0092:  ldloca.s   V_4
    IL_0094:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0099:  conv.u4
    IL_009a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_009f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_00a4:  nop
    IL_00a5:  ldarg.2
    IL_00a6:  stloc.s    V_5
    IL_00a8:  ldloca.s   V_5
    IL_00aa:  call       instance bool valuetype [mscorlib]System.Nullable`1<float32>::get_HasValue()
    IL_00af:  brtrue.s   IL_00bc

    IL_00b1:  ldloca.s   V_3
    IL_00b3:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_00b9:  ldloc.3
    IL_00ba:  br.s       IL_00c9

    IL_00bc:  ldloca.s   V_5
    IL_00be:  call       instance !0 valuetype [mscorlib]System.Nullable`1<float32>::GetValueOrDefault()
    IL_00c3:  conv.u4
    IL_00c4:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_00c9:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_00ce:  nop
    IL_00cf:  ret
  } // end of method LiftedExplicitConversions::UncheckedCasts

  .method public hidebysig static void  CheckedCasts(valuetype [mscorlib]System.Nullable`1<int32> i4,
                                                     valuetype [mscorlib]System.Nullable`1<int64> i8,
                                                     valuetype [mscorlib]System.Nullable`1<float32> f) cil managed
  {
    // Code size       169 (0xa9)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<uint8> V_1,
             valuetype [mscorlib]System.Nullable`1<int16> V_2,
             valuetype [mscorlib]System.Nullable`1<uint32> V_3,
             valuetype [mscorlib]System.Nullable`1<int64> V_4)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.0
    IL_0003:  stloc.0
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000b:  brtrue.s   IL_0018

    IL_000d:  ldloca.s   V_1
    IL_000f:  initobj    valuetype [mscorlib]System.Nullable`1<uint8>
    IL_0015:  ldloc.1
    IL_0016:  br.s       IL_0025

    IL_0018:  ldloca.s   V_0
    IL_001a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001f:  conv.ovf.u1
    IL_0020:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint8>::.ctor(!0)
    IL_0025:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint8>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_002a:  nop
    IL_002b:  ldarg.0
    IL_002c:  stloc.0
    IL_002d:  ldloca.s   V_0
    IL_002f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0034:  brtrue.s   IL_0041

    IL_0036:  ldloca.s   V_2
    IL_0038:  initobj    valuetype [mscorlib]System.Nullable`1<int16>
    IL_003e:  ldloc.2
    IL_003f:  br.s       IL_004e

    IL_0041:  ldloca.s   V_0
    IL_0043:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0048:  conv.ovf.i2
    IL_0049:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int16>::.ctor(!0)
    IL_004e:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<int16>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_0053:  nop
    IL_0054:  ldarg.0
    IL_0055:  stloc.0
    IL_0056:  ldloca.s   V_0
    IL_0058:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_005d:  brtrue.s   IL_006a

    IL_005f:  ldloca.s   V_3
    IL_0061:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_0067:  ldloc.3
    IL_0068:  br.s       IL_0077

    IL_006a:  ldloca.s   V_0
    IL_006c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0071:  conv.ovf.u4
    IL_0072:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_0077:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_007c:  nop
    IL_007d:  ldarg.1
    IL_007e:  stloc.s    V_4
    IL_0080:  ldloca.s   V_4
    IL_0082:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0087:  brtrue.s   IL_0094

    IL_0089:  ldloca.s   V_3
    IL_008b:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_0091:  ldloc.3
    IL_0092:  br.s       IL_00a1

    IL_0094:  ldloca.s   V_4
    IL_0096:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_009b:  conv.ovf.u4
    IL_009c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_00a1:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_00a6:  nop
    IL_00a7:  nop
    IL_00a8:  ret
  } // end of method LiftedExplicitConversions::CheckedCasts

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method LiftedExplicitConversions::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullCoalescingTests
       extends [mscorlib]System.Object
{
  .method private hidebysig static void  Print<T>(!!T x) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  box        !!T
    IL_0007:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_000c:  nop
    IL_000d:  ret
  } // end of method NullCoalescingTests::Print

  .method public hidebysig static void  Objects(object a,
                                                object b) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  dup
    IL_0003:  brtrue.s   IL_0007

    IL_0005:  pop
    IL_0006:  ldarg.1
    IL_0007:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullCoalescingTests::Print<object>(!!0)
    IL_000c:  nop
    IL_000d:  ret
  } // end of method NullCoalescingTests::Objects

  .method public hidebysig static void  Nullables(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                  valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_000f

    IL_000c:  ldarg.1
    IL_000d:  br.s       IL_0010

    IL_000f:  ldloc.0
    IL_0010:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullCoalescingTests::Print<valuetype [mscorlib]System.Nullable`1<int32>>(!!0)
    IL_0015:  nop
    IL_0016:  ret
  } // end of method NullCoalescingTests::Nullables

  .method public hidebysig static void  NullableWithNonNullableFallback(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                                        int32 b) cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_000f

    IL_000c:  ldarg.1
    IL_000d:  br.s       IL_0016

    IL_000f:  ldloca.s   V_0
    IL_0011:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0016:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullCoalescingTests::Print<int32>(!!0)
    IL_001b:  nop
    IL_001c:  ret
  } // end of method NullCoalescingTests::NullableWithNonNullableFallback

  .method public hidebysig static void  NullableWithImplicitConversion(valuetype [mscorlib]System.Nullable`1<int16> a,
                                                                       valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       34 (0x22)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int16> V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int16>::get_HasValue()
    IL_000a:  brtrue.s   IL_000f

    IL_000c:  ldarg.1
    IL_000d:  br.s       IL_001b

    IL_000f:  ldloca.s   V_0
    IL_0011:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int16>::GetValueOrDefault()
    IL_0016:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_001b:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullCoalescingTests::Print<valuetype [mscorlib]System.Nullable`1<int32>>(!!0)
    IL_0020:  nop
    IL_0021:  ret
  } // end of method NullCoalescingTests::NullableWithImplicitConversion

  .method public hidebysig static void  NullableWithImplicitConversionAndNonNullableFallback(valuetype [mscorlib]System.Nullable`1<int16> a,
                                                                                             int32 b) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method NullCoalescingTests::NullableWithImplicitConversionAndNonNullableFallback

  .method public hidebysig static void  Chain(valuetype [mscorlib]System.Nullable`1<int32> a,
                                              valuetype [mscorlib]System.Nullable`1<int32> b,
                                              valuetype [mscorlib]System.Nullable`1<int32> c,
                                              int32 d) cil managed
  {
    // Code size       69 (0x45)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_0037

    IL_000c:  ldarg.1
    IL_000d:  stloc.1
    IL_000e:  ldloca.s   V_1
    IL_0010:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0015:  brtrue.s   IL_002e

    IL_0017:  ldarg.2
    IL_0018:  stloc.2
    IL_0019:  ldloca.s   V_2
    IL_001b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0020:  brtrue.s   IL_0025

    IL_0022:  ldarg.3
    IL_0023:  br.s       IL_002c

    IL_0025:  ldloca.s   V_2
    IL_0027:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_002c:  br.s       IL_0035

    IL_002e:  ldloca.s   V_1
    IL_0030:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0035:  br.s       IL_003e

    IL_0037:  ldloca.s   V_0
    IL_0039:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_003e:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullCoalescingTests::Print<int32>(!!0)
    IL_0043:  nop
    IL_0044:  ret
  } // end of method NullCoalescingTests::Chain

  .method public hidebysig static void  ChainWithImplicitConversions(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                                     valuetype [mscorlib]System.Nullable`1<int16> b,
                                                                     valuetype [mscorlib]System.Nullable`1<int64> c,
                                                                     uint8 d) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method NullCoalescingTests::ChainWithImplicitConversions

  .method public hidebysig static void  ChainWithComputation(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                             valuetype [mscorlib]System.Nullable`1<int16> b,
                                                             valuetype [mscorlib]System.Nullable`1<int64> c,
                                                             uint8 d) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method NullCoalescingTests::ChainWithComputation

  .method public hidebysig static object 
          ReturnObjects(object a,
                        object b) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  2
    .locals init (object V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  dup
    IL_0003:  brtrue.s   IL_0007

    IL_0005:  pop
    IL_0006:  ldarg.1
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method NullCoalescingTests::ReturnObjects

  .method public hidebysig static valuetype [mscorlib]System.Nullable`1<int32> 
          ReturnNullables(valuetype [mscorlib]System.Nullable`1<int32> a,
                          valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       21 (0x15)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_000f

    IL_000c:  ldarg.1
    IL_000d:  br.s       IL_0010

    IL_000f:  ldloc.0
    IL_0010:  stloc.1
    IL_0011:  br.s       IL_0013

    IL_0013:  ldloc.1
    IL_0014:  ret
  } // end of method NullCoalescingTests::ReturnNullables

  .method public hidebysig static int32  ReturnNullableWithNonNullableFallback(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                                               int32 b) cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_000f

    IL_000c:  ldarg.1
    IL_000d:  br.s       IL_0016

    IL_000f:  ldloca.s   V_0
    IL_0011:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0016:  stloc.1
    IL_0017:  br.s       IL_0019

    IL_0019:  ldloc.1
    IL_001a:  ret
  } // end of method NullCoalescingTests::ReturnNullableWithNonNullableFallback

  .method public hidebysig static int32  ReturnChain(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                     valuetype [mscorlib]System.Nullable`1<int32> b,
                                                     valuetype [mscorlib]System.Nullable`1<int32> c,
                                                     int32 d) cil managed
  {
    // Code size       67 (0x43)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             int32 V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_0037

    IL_000c:  ldarg.1
    IL_000d:  stloc.1
    IL_000e:  ldloca.s   V_1
    IL_0010:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0015:  brtrue.s   IL_002e

    IL_0017:  ldarg.2
    IL_0018:  stloc.2
    IL_0019:  ldloca.s   V_2
    IL_001b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0020:  brtrue.s   IL_0025

    IL_0022:  ldarg.3
    IL_0023:  br.s       IL_002c

    IL_0025:  ldloca.s   V_2
    IL_0027:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_002c:  br.s       IL_0035

    IL_002e:  ldloca.s   V_1
    IL_0030:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0035:  br.s       IL_003e

    IL_0037:  ldloca.s   V_0
    IL_0039:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_003e:  stloc.3
    IL_003f:  br.s       IL_0041

    IL_0041:  ldloc.3
    IL_0042:  ret
  } // end of method NullCoalescingTests::ReturnChain

  .method public hidebysig static int64  ReturnChainWithImplicitConversions(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                                            valuetype [mscorlib]System.Nullable`1<int16> b,
                                                                            valuetype [mscorlib]System.Nullable`1<int64> c,
                                                                            uint8 d) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  1
    .locals init (int64 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  conv.i8
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method NullCoalescingTests::ReturnChainWithImplicitConversions

  .method public hidebysig static int64  ReturnChainWithComputation(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                                    valuetype [mscorlib]System.Nullable`1<int16> b,
                                                                    valuetype [mscorlib]System.Nullable`1<int64> c,
                                                                    uint8 d) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  1
    .locals init (int64 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  conv.i8
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method NullCoalescingTests::ReturnChainWithComputation

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method NullCoalescingTests::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullCoalescingTests


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
