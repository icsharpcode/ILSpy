
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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module LiftedOperators.dll
// MVID: {E2E991E4-7C1D-40D9-8678-641E8A59EDAC}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x014C0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedOperators
       extends [mscorlib]System.Object
{
  .method public hidebysig static void  BoolBasic(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                  valuetype [mscorlib]System.Nullable`1<bool> b) cil managed
  {
    // Code size       96 (0x60)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<bool> V_0,
             valuetype [mscorlib]System.Nullable`1<bool> V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_000b:  ldloca.s   V_1
    IL_000d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0012:  beq.s      IL_0017

    IL_0014:  ldc.i4.0
    IL_0015:  br.s       IL_0027

    IL_0017:  ldloca.s   V_0
    IL_0019:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001e:  ldloca.s   V_1
    IL_0020:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0025:  ceq
    IL_0027:  brfalse.s  IL_002e

    IL_0029:  call       void [mscorlib]System.Console::WriteLine()
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
    IL_0058:  brfalse.s  IL_005f

    IL_005a:  call       void [mscorlib]System.Console::WriteLine()
    IL_005f:  ret
  } // end of method LiftedOperators::BoolBasic

  .method public hidebysig static void  BoolComplex(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                    class [mscorlib]System.Func`1<bool> x) cil managed
  {
    // Code size       147 (0x93)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<bool> V_0,
             bool V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0008:  stloc.1
    IL_0009:  ldloca.s   V_0
    IL_000b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0010:  ldloc.1
    IL_0011:  beq.s      IL_0016

    IL_0013:  ldc.i4.0
    IL_0014:  br.s       IL_001d

    IL_0016:  ldloca.s   V_0
    IL_0018:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001d:  brfalse.s  IL_0024

    IL_001f:  call       void [mscorlib]System.Console::WriteLine()
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
    IL_0044:  brfalse.s  IL_004b

    IL_0046:  call       void [mscorlib]System.Console::WriteLine()
    IL_004b:  ldarg.1
    IL_004c:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0051:  ldarg.0
    IL_0052:  stloc.0
    IL_0053:  ldloca.s   V_0
    IL_0055:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_005a:  beq.s      IL_005f

    IL_005c:  ldc.i4.0
    IL_005d:  br.s       IL_0066

    IL_005f:  ldloca.s   V_0
    IL_0061:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0066:  brfalse.s  IL_006d

    IL_0068:  call       void [mscorlib]System.Console::WriteLine()
    IL_006d:  ldarg.1
    IL_006e:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0073:  ldarg.0
    IL_0074:  stloc.0
    IL_0075:  ldloca.s   V_0
    IL_0077:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_007c:  beq.s      IL_0081

    IL_007e:  ldc.i4.1
    IL_007f:  br.s       IL_008b

    IL_0081:  ldloca.s   V_0
    IL_0083:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0088:  ldc.i4.0
    IL_0089:  ceq
    IL_008b:  brfalse.s  IL_0092

    IL_008d:  call       void [mscorlib]System.Console::WriteLine()
    IL_0092:  ret
  } // end of method LiftedOperators::BoolComplex

  .method public hidebysig static void  BoolConst(valuetype [mscorlib]System.Nullable`1<bool> a) cil managed
  {
    // Code size       187 (0xbb)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<bool> V_0,
             bool V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_000b:  ldloc.1
    IL_000c:  beq.s      IL_0011

    IL_000e:  ldc.i4.0
    IL_000f:  br.s       IL_0018

    IL_0011:  ldloca.s   V_0
    IL_0013:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0018:  brfalse.s  IL_001f

    IL_001a:  call       void [mscorlib]System.Console::WriteLine()
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
    IL_003a:  brfalse.s  IL_0041

    IL_003c:  call       void [mscorlib]System.Console::WriteLine()
    IL_0041:  ldarg.0
    IL_0042:  stloc.0
    IL_0043:  ldc.i4.0
    IL_0044:  stloc.1
    IL_0045:  ldloca.s   V_0
    IL_0047:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_004c:  ldloc.1
    IL_004d:  beq.s      IL_0052

    IL_004f:  ldc.i4.0
    IL_0050:  br.s       IL_0059

    IL_0052:  ldloca.s   V_0
    IL_0054:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0059:  brfalse.s  IL_0060

    IL_005b:  call       void [mscorlib]System.Console::WriteLine()
    IL_0060:  ldarg.0
    IL_0061:  stloc.0
    IL_0062:  ldc.i4.0
    IL_0063:  stloc.1
    IL_0064:  ldloca.s   V_0
    IL_0066:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_006b:  ldloc.1
    IL_006c:  beq.s      IL_0071

    IL_006e:  ldc.i4.1
    IL_006f:  br.s       IL_007b

    IL_0071:  ldloca.s   V_0
    IL_0073:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0078:  ldc.i4.0
    IL_0079:  ceq
    IL_007b:  brfalse.s  IL_0082

    IL_007d:  call       void [mscorlib]System.Console::WriteLine()
    IL_0082:  ldarg.0
    IL_0083:  stloc.0
    IL_0084:  ldloca.s   V_0
    IL_0086:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_008b:  brtrue.s   IL_0090

    IL_008d:  ldc.i4.1
    IL_008e:  br.s       IL_0097

    IL_0090:  ldloca.s   V_0
    IL_0092:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0097:  brfalse.s  IL_009e

    IL_0099:  call       void [mscorlib]System.Console::WriteLine()
    IL_009e:  ldarg.0
    IL_009f:  stloc.0
    IL_00a0:  ldloca.s   V_0
    IL_00a2:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00a7:  brtrue.s   IL_00ac

    IL_00a9:  ldc.i4.0
    IL_00aa:  br.s       IL_00b3

    IL_00ac:  ldloca.s   V_0
    IL_00ae:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00b3:  brfalse.s  IL_00ba

    IL_00b5:  call       void [mscorlib]System.Console::WriteLine()
    IL_00ba:  ret
  } // end of method LiftedOperators::BoolConst

  .method public hidebysig static void  BoolValueBasic(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                       valuetype [mscorlib]System.Nullable`1<bool> b) cil managed
  {
    // Code size       444 (0x1bc)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<bool> V_0,
             valuetype [mscorlib]System.Nullable`1<bool> V_1,
             valuetype [mscorlib]System.Nullable`1<bool> V_2)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_000b:  ldloca.s   V_1
    IL_000d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0012:  beq.s      IL_0017

    IL_0014:  ldc.i4.0
    IL_0015:  br.s       IL_0027

    IL_0017:  ldloca.s   V_0
    IL_0019:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001e:  ldloca.s   V_1
    IL_0020:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0025:  ceq
    IL_0027:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_002c:  ldarg.0
    IL_002d:  stloc.1
    IL_002e:  ldarg.1
    IL_002f:  stloc.0
    IL_0030:  ldloca.s   V_1
    IL_0032:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0037:  ldloca.s   V_0
    IL_0039:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_003e:  beq.s      IL_0043

    IL_0040:  ldc.i4.1
    IL_0041:  br.s       IL_0056

    IL_0043:  ldloca.s   V_1
    IL_0045:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_004a:  ldloca.s   V_0
    IL_004c:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0051:  ceq
    IL_0053:  ldc.i4.0
    IL_0054:  ceq
    IL_0056:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_005b:  ldarg.0
    IL_005c:  stloc.0
    IL_005d:  ldarg.1
    IL_005e:  stloc.1
    IL_005f:  ldloca.s   V_0
    IL_0061:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0066:  brtrue.s   IL_007d

    IL_0068:  ldloca.s   V_1
    IL_006a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_006f:  brtrue.s   IL_007a

    IL_0071:  ldloca.s   V_0
    IL_0073:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0078:  brfalse.s  IL_007d

    IL_007a:  ldloc.0
    IL_007b:  br.s       IL_007e

    IL_007d:  ldloc.1
    IL_007e:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0083:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0088:  ldarg.0
    IL_0089:  stloc.1
    IL_008a:  ldarg.1
    IL_008b:  stloc.0
    IL_008c:  ldloca.s   V_1
    IL_008e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0093:  brtrue.s   IL_00aa

    IL_0095:  ldloca.s   V_0
    IL_0097:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_009c:  brtrue.s   IL_00a7

    IL_009e:  ldloca.s   V_1
    IL_00a0:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00a5:  brfalse.s  IL_00aa

    IL_00a7:  ldloc.0
    IL_00a8:  br.s       IL_00ab

    IL_00aa:  ldloc.1
    IL_00ab:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_00b0:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00b5:  ldarg.0
    IL_00b6:  stloc.0
    IL_00b7:  ldarg.1
    IL_00b8:  stloc.1
    IL_00b9:  ldloca.s   V_0
    IL_00bb:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00c0:  ldloca.s   V_1
    IL_00c2:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00c7:  and
    IL_00c8:  brtrue.s   IL_00d5

    IL_00ca:  ldloca.s   V_2
    IL_00cc:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_00d2:  ldloc.2
    IL_00d3:  br.s       IL_00e9

    IL_00d5:  ldloca.s   V_0
    IL_00d7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00dc:  ldloca.s   V_1
    IL_00de:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00e3:  xor
    IL_00e4:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_00e9:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_00ee:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00f3:  ldarg.0
    IL_00f4:  stloc.1
    IL_00f5:  ldloca.s   V_1
    IL_00f7:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00fc:  brtrue.s   IL_0101

    IL_00fe:  ldarg.1
    IL_00ff:  br.s       IL_0102

    IL_0101:  ldloc.1
    IL_0102:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0107:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_010c:  ldarg.0
    IL_010d:  stloc.1
    IL_010e:  ldloca.s   V_1
    IL_0110:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0115:  brtrue.s   IL_0122

    IL_0117:  ldloca.s   V_0
    IL_0119:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_011f:  ldloc.0
    IL_0120:  br.s       IL_0131

    IL_0122:  ldloca.s   V_1
    IL_0124:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0129:  ldc.i4.0
    IL_012a:  ceq
    IL_012c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0131:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0136:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_013b:  ldarg.0
    IL_013c:  stloc.1
    IL_013d:  ldarg.1
    IL_013e:  stloc.0
    IL_013f:  ldloca.s   V_1
    IL_0141:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0146:  brtrue.s   IL_015d

    IL_0148:  ldloca.s   V_0
    IL_014a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_014f:  brtrue.s   IL_015a

    IL_0151:  ldloca.s   V_1
    IL_0153:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0158:  brfalse.s  IL_015d

    IL_015a:  ldloc.1
    IL_015b:  br.s       IL_015e

    IL_015d:  ldloc.0
    IL_015e:  starg.s    a
    IL_0160:  ldarg.0
    IL_0161:  stloc.0
    IL_0162:  ldarg.1
    IL_0163:  stloc.1
    IL_0164:  ldloca.s   V_0
    IL_0166:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_016b:  brtrue.s   IL_0182

    IL_016d:  ldloca.s   V_1
    IL_016f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0174:  brtrue.s   IL_017f

    IL_0176:  ldloca.s   V_0
    IL_0178:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_017d:  brfalse.s  IL_0182

    IL_017f:  ldloc.1
    IL_0180:  br.s       IL_0183

    IL_0182:  ldloc.0
    IL_0183:  starg.s    a
    IL_0185:  ldarg.0
    IL_0186:  stloc.1
    IL_0187:  ldarg.1
    IL_0188:  stloc.0
    IL_0189:  ldloca.s   V_1
    IL_018b:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0190:  ldloca.s   V_0
    IL_0192:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0197:  and
    IL_0198:  brtrue.s   IL_01a5

    IL_019a:  ldloca.s   V_2
    IL_019c:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_01a2:  ldloc.2
    IL_01a3:  br.s       IL_01b9

    IL_01a5:  ldloca.s   V_1
    IL_01a7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01ac:  ldloca.s   V_0
    IL_01ae:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01b3:  xor
    IL_01b4:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_01b9:  starg.s    a
    IL_01bb:  ret
  } // end of method LiftedOperators::BoolValueBasic

  .method public hidebysig static void  BoolValueComplex(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                         class [mscorlib]System.Func`1<bool> x) cil managed
  {
    // Code size       518 (0x206)
    .maxstack  3
    .locals init (valuetype [mscorlib]System.Nullable`1<bool> V_0,
             bool V_1,
             valuetype [mscorlib]System.Nullable`1<bool> V_2,
             valuetype [mscorlib]System.Nullable`1<bool> V_3)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0008:  stloc.1
    IL_0009:  ldloca.s   V_0
    IL_000b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0010:  ldloc.1
    IL_0011:  beq.s      IL_0016

    IL_0013:  ldc.i4.0
    IL_0014:  br.s       IL_001d

    IL_0016:  ldloca.s   V_0
    IL_0018:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001d:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0022:  ldarg.0
    IL_0023:  stloc.0
    IL_0024:  ldarg.1
    IL_0025:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_002a:  stloc.1
    IL_002b:  ldloca.s   V_0
    IL_002d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0032:  ldloc.1
    IL_0033:  beq.s      IL_0038

    IL_0035:  ldc.i4.1
    IL_0036:  br.s       IL_0042

    IL_0038:  ldloca.s   V_0
    IL_003a:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_003f:  ldc.i4.0
    IL_0040:  ceq
    IL_0042:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0047:  ldarg.1
    IL_0048:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_004d:  ldarg.0
    IL_004e:  stloc.0
    IL_004f:  ldloca.s   V_0
    IL_0051:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0056:  beq.s      IL_005b

    IL_0058:  ldc.i4.0
    IL_0059:  br.s       IL_0062

    IL_005b:  ldloca.s   V_0
    IL_005d:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0062:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0067:  ldarg.1
    IL_0068:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_006d:  ldarg.0
    IL_006e:  stloc.0
    IL_006f:  ldloca.s   V_0
    IL_0071:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0076:  beq.s      IL_007b

    IL_0078:  ldc.i4.1
    IL_0079:  br.s       IL_0085

    IL_007b:  ldloca.s   V_0
    IL_007d:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0082:  ldc.i4.0
    IL_0083:  ceq
    IL_0085:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_008a:  ldarg.0
    IL_008b:  stloc.0
    IL_008c:  ldarg.1
    IL_008d:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0092:  stloc.1
    IL_0093:  ldloca.s   V_0
    IL_0095:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_009a:  brtrue.s   IL_00a7

    IL_009c:  ldloca.s   V_2
    IL_009e:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_00a4:  ldloc.2
    IL_00a5:  br.s       IL_00b5

    IL_00a7:  ldloca.s   V_0
    IL_00a9:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00ae:  ldloc.1
    IL_00af:  xor
    IL_00b0:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_00b5:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_00ba:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00bf:  ldarg.0
    IL_00c0:  stloc.0
    IL_00c1:  ldloca.s   V_0
    IL_00c3:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00c8:  brtrue.s   IL_00d2

    IL_00ca:  ldarg.1
    IL_00cb:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_00d0:  br.s       IL_00d9

    IL_00d2:  ldloca.s   V_0
    IL_00d4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00d9:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00de:  ldarg.0
    IL_00df:  stloc.0
    IL_00e0:  ldarg.1
    IL_00e1:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_00e6:  stloc.1
    IL_00e7:  ldloca.s   V_0
    IL_00e9:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00ee:  brtrue.s   IL_00fb

    IL_00f0:  ldloca.s   V_2
    IL_00f2:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_00f8:  ldloc.2
    IL_00f9:  br.s       IL_0109

    IL_00fb:  ldloca.s   V_0
    IL_00fd:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0102:  ldloc.1
    IL_0103:  xor
    IL_0104:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0109:  starg.s    a
    IL_010b:  ldarg.1
    IL_010c:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0111:  ldarg.0
    IL_0112:  stloc.0
    IL_0113:  brtrue.s   IL_011d

    IL_0115:  ldc.i4.0
    IL_0116:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_011b:  br.s       IL_011e

    IL_011d:  ldloc.0
    IL_011e:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0123:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0128:  ldarg.1
    IL_0129:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_012e:  ldarg.0
    IL_012f:  stloc.0
    IL_0130:  brtrue.s   IL_0135

    IL_0132:  ldloc.0
    IL_0133:  br.s       IL_013b

    IL_0135:  ldc.i4.1
    IL_0136:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_013b:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0140:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0145:  ldarg.1
    IL_0146:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_014b:  stloc.1
    IL_014c:  ldarg.0
    IL_014d:  stloc.0
    IL_014e:  ldloca.s   V_0
    IL_0150:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0155:  brtrue.s   IL_0162

    IL_0157:  ldloca.s   V_2
    IL_0159:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_015f:  ldloc.2
    IL_0160:  br.s       IL_0170

    IL_0162:  ldloc.1
    IL_0163:  ldloca.s   V_0
    IL_0165:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_016a:  xor
    IL_016b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0170:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0175:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_017a:  ldc.i4.0
    IL_017b:  newarr     valuetype [mscorlib]System.Nullable`1<bool>
    IL_0180:  ldc.i4.0
    IL_0181:  ldelema    valuetype [mscorlib]System.Nullable`1<bool>
    IL_0186:  dup
    IL_0187:  ldobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_018c:  stloc.0
    IL_018d:  ldarg.1
    IL_018e:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0193:  stloc.1
    IL_0194:  ldloca.s   V_0
    IL_0196:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_019b:  brtrue.s   IL_01a8

    IL_019d:  ldloca.s   V_2
    IL_019f:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_01a5:  ldloc.2
    IL_01a6:  br.s       IL_01b6

    IL_01a8:  ldloca.s   V_0
    IL_01aa:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01af:  ldloc.1
    IL_01b0:  xor
    IL_01b1:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_01b6:  stobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_01bb:  ldc.i4.0
    IL_01bc:  newarr     valuetype [mscorlib]System.Nullable`1<bool>
    IL_01c1:  ldc.i4.0
    IL_01c2:  ldelema    valuetype [mscorlib]System.Nullable`1<bool>
    IL_01c7:  dup
    IL_01c8:  ldobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_01cd:  stloc.0
    IL_01ce:  ldarg.0
    IL_01cf:  stloc.2
    IL_01d0:  ldloca.s   V_0
    IL_01d2:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_01d7:  ldloca.s   V_2
    IL_01d9:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_01de:  and
    IL_01df:  brtrue.s   IL_01ec

    IL_01e1:  ldloca.s   V_3
    IL_01e3:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_01e9:  ldloc.3
    IL_01ea:  br.s       IL_0200

    IL_01ec:  ldloca.s   V_0
    IL_01ee:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01f3:  ldloca.s   V_2
    IL_01f5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01fa:  xor
    IL_01fb:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0200:  stobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_0205:  ret
  } // end of method LiftedOperators::BoolValueComplex

  .method public hidebysig static void  BoolValueConst(valuetype [mscorlib]System.Nullable`1<bool> a) cil managed
  {
    // Code size       175 (0xaf)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<bool> V_0,
             bool V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_000b:  ldloc.1
    IL_000c:  beq.s      IL_0011

    IL_000e:  ldc.i4.0
    IL_000f:  br.s       IL_0018

    IL_0011:  ldloca.s   V_0
    IL_0013:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0018:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_001d:  ldarg.0
    IL_001e:  stloc.0
    IL_001f:  ldc.i4.1
    IL_0020:  stloc.1
    IL_0021:  ldloca.s   V_0
    IL_0023:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0028:  ldloc.1
    IL_0029:  beq.s      IL_002e

    IL_002b:  ldc.i4.1
    IL_002c:  br.s       IL_0038

    IL_002e:  ldloca.s   V_0
    IL_0030:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0035:  ldc.i4.0
    IL_0036:  ceq
    IL_0038:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_003d:  ldarg.0
    IL_003e:  stloc.0
    IL_003f:  ldc.i4.0
    IL_0040:  stloc.1
    IL_0041:  ldloca.s   V_0
    IL_0043:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0048:  ldloc.1
    IL_0049:  beq.s      IL_004e

    IL_004b:  ldc.i4.0
    IL_004c:  br.s       IL_0055

    IL_004e:  ldloca.s   V_0
    IL_0050:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0055:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_005a:  ldarg.0
    IL_005b:  stloc.0
    IL_005c:  ldc.i4.0
    IL_005d:  stloc.1
    IL_005e:  ldloca.s   V_0
    IL_0060:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0065:  ldloc.1
    IL_0066:  beq.s      IL_006b

    IL_0068:  ldc.i4.1
    IL_0069:  br.s       IL_0075

    IL_006b:  ldloca.s   V_0
    IL_006d:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0072:  ldc.i4.0
    IL_0073:  ceq
    IL_0075:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_007a:  ldarg.0
    IL_007b:  stloc.0
    IL_007c:  ldloca.s   V_0
    IL_007e:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0083:  brtrue.s   IL_0088

    IL_0085:  ldc.i4.1
    IL_0086:  br.s       IL_008f

    IL_0088:  ldloca.s   V_0
    IL_008a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_008f:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0094:  ldarg.0
    IL_0095:  stloc.0
    IL_0096:  ldloca.s   V_0
    IL_0098:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_009d:  brtrue.s   IL_00a2

    IL_009f:  ldc.i4.0
    IL_00a0:  br.s       IL_00a9

    IL_00a2:  ldloca.s   V_0
    IL_00a4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00a9:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00ae:  ret
  } // end of method LiftedOperators::BoolValueConst

  .method public hidebysig static void  IntBasic(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                 valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       366 (0x16e)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000b:  ldloca.s   V_1
    IL_000d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0012:  beq.s      IL_0017

    IL_0014:  ldc.i4.0
    IL_0015:  br.s       IL_0027

    IL_0017:  ldloca.s   V_0
    IL_0019:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001e:  ldloca.s   V_1
    IL_0020:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0025:  ceq
    IL_0027:  brfalse.s  IL_002e

    IL_0029:  call       void [mscorlib]System.Console::WriteLine()
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
    IL_0058:  brfalse.s  IL_005f

    IL_005a:  call       void [mscorlib]System.Console::WriteLine()
    IL_005f:  ldarg.0
    IL_0060:  stloc.0
    IL_0061:  ldarg.1
    IL_0062:  stloc.1
    IL_0063:  ldloca.s   V_0
    IL_0065:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_006a:  ldloca.s   V_1
    IL_006c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0071:  bgt.s      IL_0076

    IL_0073:  ldc.i4.0
    IL_0074:  br.s       IL_0085

    IL_0076:  ldloca.s   V_0
    IL_0078:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_007d:  ldloca.s   V_1
    IL_007f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0084:  and
    IL_0085:  brfalse.s  IL_008c

    IL_0087:  call       void [mscorlib]System.Console::WriteLine()
    IL_008c:  ldarg.0
    IL_008d:  stloc.1
    IL_008e:  ldarg.1
    IL_008f:  stloc.0
    IL_0090:  ldloca.s   V_1
    IL_0092:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0097:  ldloca.s   V_0
    IL_0099:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_009e:  blt.s      IL_00a3

    IL_00a0:  ldc.i4.0
    IL_00a1:  br.s       IL_00b2

    IL_00a3:  ldloca.s   V_1
    IL_00a5:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00aa:  ldloca.s   V_0
    IL_00ac:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00b1:  and
    IL_00b2:  brfalse.s  IL_00b9

    IL_00b4:  call       void [mscorlib]System.Console::WriteLine()
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
    IL_00df:  brfalse.s  IL_00e6

    IL_00e1:  call       void [mscorlib]System.Console::WriteLine()
    IL_00e6:  ldarg.0
    IL_00e7:  stloc.1
    IL_00e8:  ldarg.1
    IL_00e9:  stloc.0
    IL_00ea:  ldloca.s   V_1
    IL_00ec:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00f1:  ldloca.s   V_0
    IL_00f3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00f8:  ble.s      IL_00fd

    IL_00fa:  ldc.i4.0
    IL_00fb:  br.s       IL_010c

    IL_00fd:  ldloca.s   V_1
    IL_00ff:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0104:  ldloca.s   V_0
    IL_0106:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_010b:  and
    IL_010c:  brfalse.s  IL_0113

    IL_010e:  call       void [mscorlib]System.Console::WriteLine()
    IL_0113:  ldarg.0
    IL_0114:  stloc.0
    IL_0115:  ldarg.1
    IL_0116:  stloc.1
    IL_0117:  ldloca.s   V_0
    IL_0119:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_011e:  ldloca.s   V_1
    IL_0120:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0125:  bgt.s      IL_012a

    IL_0127:  ldc.i4.0
    IL_0128:  br.s       IL_0139

    IL_012a:  ldloca.s   V_0
    IL_012c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0131:  ldloca.s   V_1
    IL_0133:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0138:  and
    IL_0139:  brtrue.s   IL_0140

    IL_013b:  call       void [mscorlib]System.Console::WriteLine()
    IL_0140:  ldarg.0
    IL_0141:  stloc.1
    IL_0142:  ldarg.1
    IL_0143:  stloc.0
    IL_0144:  ldloca.s   V_1
    IL_0146:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_014b:  ldloca.s   V_0
    IL_014d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0152:  ble.s      IL_0157

    IL_0154:  ldc.i4.0
    IL_0155:  br.s       IL_0166

    IL_0157:  ldloca.s   V_1
    IL_0159:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_015e:  ldloca.s   V_0
    IL_0160:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0165:  and
    IL_0166:  brtrue.s   IL_016d

    IL_0168:  call       void [mscorlib]System.Console::WriteLine()
    IL_016d:  ret
  } // end of method LiftedOperators::IntBasic

  .method public hidebysig static void  IntComplex(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                   class [mscorlib]System.Func`1<int32> x) cil managed
  {
    // Code size       289 (0x121)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0008:  stloc.1
    IL_0009:  ldloca.s   V_0
    IL_000b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0010:  ldloc.1
    IL_0011:  beq.s      IL_0016

    IL_0013:  ldc.i4.0
    IL_0014:  br.s       IL_001d

    IL_0016:  ldloca.s   V_0
    IL_0018:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001d:  brfalse.s  IL_0024

    IL_001f:  call       void [mscorlib]System.Console::WriteLine()
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
    IL_0044:  brfalse.s  IL_004b

    IL_0046:  call       void [mscorlib]System.Console::WriteLine()
    IL_004b:  ldarg.0
    IL_004c:  stloc.0
    IL_004d:  ldarg.1
    IL_004e:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0053:  stloc.1
    IL_0054:  ldloca.s   V_0
    IL_0056:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_005b:  ldloc.1
    IL_005c:  bgt.s      IL_0061

    IL_005e:  ldc.i4.0
    IL_005f:  br.s       IL_0068

    IL_0061:  ldloca.s   V_0
    IL_0063:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0068:  brfalse.s  IL_006f

    IL_006a:  call       void [mscorlib]System.Console::WriteLine()
    IL_006f:  ldarg.1
    IL_0070:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0075:  ldarg.0
    IL_0076:  stloc.0
    IL_0077:  ldloca.s   V_0
    IL_0079:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_007e:  beq.s      IL_0083

    IL_0080:  ldc.i4.0
    IL_0081:  br.s       IL_008a

    IL_0083:  ldloca.s   V_0
    IL_0085:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_008a:  brfalse.s  IL_0091

    IL_008c:  call       void [mscorlib]System.Console::WriteLine()
    IL_0091:  ldarg.1
    IL_0092:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0097:  ldarg.0
    IL_0098:  stloc.0
    IL_0099:  ldloca.s   V_0
    IL_009b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00a0:  beq.s      IL_00a5

    IL_00a2:  ldc.i4.1
    IL_00a3:  br.s       IL_00af

    IL_00a5:  ldloca.s   V_0
    IL_00a7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ac:  ldc.i4.0
    IL_00ad:  ceq
    IL_00af:  brfalse.s  IL_00b6

    IL_00b1:  call       void [mscorlib]System.Console::WriteLine()
    IL_00b6:  ldarg.1
    IL_00b7:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00bc:  ldarg.0
    IL_00bd:  stloc.0
    IL_00be:  ldloca.s   V_0
    IL_00c0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00c5:  bgt.s      IL_00ca

    IL_00c7:  ldc.i4.0
    IL_00c8:  br.s       IL_00d1

    IL_00ca:  ldloca.s   V_0
    IL_00cc:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00d1:  brfalse.s  IL_00d8

    IL_00d3:  call       void [mscorlib]System.Console::WriteLine()
    IL_00d8:  ldarg.0
    IL_00d9:  stloc.0
    IL_00da:  ldarg.1
    IL_00db:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00e0:  stloc.1
    IL_00e1:  ldloca.s   V_0
    IL_00e3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00e8:  ldloc.1
    IL_00e9:  bgt.s      IL_00ee

    IL_00eb:  ldc.i4.0
    IL_00ec:  br.s       IL_00f5

    IL_00ee:  ldloca.s   V_0
    IL_00f0:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00f5:  brtrue.s   IL_00fc

    IL_00f7:  call       void [mscorlib]System.Console::WriteLine()
    IL_00fc:  ldarg.0
    IL_00fd:  stloc.0
    IL_00fe:  ldarg.1
    IL_00ff:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0104:  stloc.1
    IL_0105:  ldloca.s   V_0
    IL_0107:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_010c:  ldloc.1
    IL_010d:  ble.s      IL_0112

    IL_010f:  ldc.i4.0
    IL_0110:  br.s       IL_0119

    IL_0112:  ldloca.s   V_0
    IL_0114:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0119:  brtrue.s   IL_0120

    IL_011b:  call       void [mscorlib]System.Console::WriteLine()
    IL_0120:  ret
  } // end of method LiftedOperators::IntComplex

  .method public hidebysig static void  IntConst(valuetype [mscorlib]System.Nullable`1<int32> a) cil managed
  {
    // Code size       187 (0xbb)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.2
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000b:  ldloc.1
    IL_000c:  beq.s      IL_0011

    IL_000e:  ldc.i4.0
    IL_000f:  br.s       IL_0018

    IL_0011:  ldloca.s   V_0
    IL_0013:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0018:  brfalse.s  IL_001f

    IL_001a:  call       void [mscorlib]System.Console::WriteLine()
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
    IL_003a:  brfalse.s  IL_0041

    IL_003c:  call       void [mscorlib]System.Console::WriteLine()
    IL_0041:  ldarg.0
    IL_0042:  stloc.0
    IL_0043:  ldc.i4.2
    IL_0044:  stloc.1
    IL_0045:  ldloca.s   V_0
    IL_0047:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_004c:  ldloc.1
    IL_004d:  bgt.s      IL_0052

    IL_004f:  ldc.i4.0
    IL_0050:  br.s       IL_0059

    IL_0052:  ldloca.s   V_0
    IL_0054:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0059:  brfalse.s  IL_0060

    IL_005b:  call       void [mscorlib]System.Console::WriteLine()
    IL_0060:  ldc.i4.2
    IL_0061:  ldarg.0
    IL_0062:  stloc.0
    IL_0063:  ldloca.s   V_0
    IL_0065:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_006a:  beq.s      IL_006f

    IL_006c:  ldc.i4.0
    IL_006d:  br.s       IL_0076

    IL_006f:  ldloca.s   V_0
    IL_0071:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0076:  brfalse.s  IL_007d

    IL_0078:  call       void [mscorlib]System.Console::WriteLine()
    IL_007d:  ldc.i4.2
    IL_007e:  ldarg.0
    IL_007f:  stloc.0
    IL_0080:  ldloca.s   V_0
    IL_0082:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0087:  beq.s      IL_008c

    IL_0089:  ldc.i4.1
    IL_008a:  br.s       IL_0096

    IL_008c:  ldloca.s   V_0
    IL_008e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0093:  ldc.i4.0
    IL_0094:  ceq
    IL_0096:  brfalse.s  IL_009d

    IL_0098:  call       void [mscorlib]System.Console::WriteLine()
    IL_009d:  ldc.i4.2
    IL_009e:  ldarg.0
    IL_009f:  stloc.0
    IL_00a0:  ldloca.s   V_0
    IL_00a2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00a7:  bgt.s      IL_00ac

    IL_00a9:  ldc.i4.0
    IL_00aa:  br.s       IL_00b3

    IL_00ac:  ldloca.s   V_0
    IL_00ae:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00b3:  brfalse.s  IL_00ba

    IL_00b5:  call       void [mscorlib]System.Console::WriteLine()
    IL_00ba:  ret
  } // end of method LiftedOperators::IntConst

  .method public hidebysig static void  IntValueBasic(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                      valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       1612 (0x64c)
    .maxstack  3
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000b:  ldloca.s   V_1
    IL_000d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0012:  beq.s      IL_0017

    IL_0014:  ldc.i4.0
    IL_0015:  br.s       IL_0027

    IL_0017:  ldloca.s   V_0
    IL_0019:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001e:  ldloca.s   V_1
    IL_0020:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0025:  ceq
    IL_0027:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_002c:  ldarg.0
    IL_002d:  stloc.1
    IL_002e:  ldarg.1
    IL_002f:  stloc.0
    IL_0030:  ldloca.s   V_1
    IL_0032:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0037:  ldloca.s   V_0
    IL_0039:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_003e:  beq.s      IL_0043

    IL_0040:  ldc.i4.1
    IL_0041:  br.s       IL_0056

    IL_0043:  ldloca.s   V_1
    IL_0045:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_004a:  ldloca.s   V_0
    IL_004c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0051:  ceq
    IL_0053:  ldc.i4.0
    IL_0054:  ceq
    IL_0056:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_005b:  ldarg.0
    IL_005c:  stloc.0
    IL_005d:  ldarg.1
    IL_005e:  stloc.1
    IL_005f:  ldloca.s   V_0
    IL_0061:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0066:  ldloca.s   V_1
    IL_0068:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_006d:  bgt.s      IL_0072

    IL_006f:  ldc.i4.0
    IL_0070:  br.s       IL_0081

    IL_0072:  ldloca.s   V_0
    IL_0074:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0079:  ldloca.s   V_1
    IL_007b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0080:  and
    IL_0081:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0086:  ldarg.0
    IL_0087:  stloc.1
    IL_0088:  ldarg.1
    IL_0089:  stloc.0
    IL_008a:  ldloca.s   V_1
    IL_008c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0091:  ldloca.s   V_0
    IL_0093:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0098:  bgt.s      IL_009d

    IL_009a:  ldc.i4.0
    IL_009b:  br.s       IL_00ac

    IL_009d:  ldloca.s   V_1
    IL_009f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00a4:  ldloca.s   V_0
    IL_00a6:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ab:  and
    IL_00ac:  ldc.i4.0
    IL_00ad:  ceq
    IL_00af:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00b4:  ldarg.0
    IL_00b5:  stloc.0
    IL_00b6:  ldarg.1
    IL_00b7:  stloc.1
    IL_00b8:  ldloca.s   V_0
    IL_00ba:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00bf:  ldloca.s   V_1
    IL_00c1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00c6:  bge.s      IL_00cb

    IL_00c8:  ldc.i4.0
    IL_00c9:  br.s       IL_00da

    IL_00cb:  ldloca.s   V_0
    IL_00cd:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00d2:  ldloca.s   V_1
    IL_00d4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00d9:  and
    IL_00da:  ldc.i4.0
    IL_00db:  ceq
    IL_00dd:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00e2:  ldarg.0
    IL_00e3:  stloc.1
    IL_00e4:  ldarg.1
    IL_00e5:  stloc.0
    IL_00e6:  ldloca.s   V_1
    IL_00e8:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ed:  ldloca.s   V_0
    IL_00ef:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00f4:  and
    IL_00f5:  brtrue.s   IL_0102

    IL_00f7:  ldloca.s   V_2
    IL_00f9:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00ff:  ldloc.2
    IL_0100:  br.s       IL_0116

    IL_0102:  ldloca.s   V_1
    IL_0104:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0109:  ldloca.s   V_0
    IL_010b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0110:  add
    IL_0111:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0116:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_011b:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0120:  ldarg.0
    IL_0121:  stloc.0
    IL_0122:  ldarg.1
    IL_0123:  stloc.1
    IL_0124:  ldloca.s   V_0
    IL_0126:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_012b:  ldloca.s   V_1
    IL_012d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0132:  and
    IL_0133:  brtrue.s   IL_0140

    IL_0135:  ldloca.s   V_2
    IL_0137:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_013d:  ldloc.2
    IL_013e:  br.s       IL_0154

    IL_0140:  ldloca.s   V_0
    IL_0142:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0147:  ldloca.s   V_1
    IL_0149:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_014e:  sub
    IL_014f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0154:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0159:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_015e:  ldarg.0
    IL_015f:  stloc.1
    IL_0160:  ldarg.1
    IL_0161:  stloc.0
    IL_0162:  ldloca.s   V_1
    IL_0164:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0169:  ldloca.s   V_0
    IL_016b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0170:  and
    IL_0171:  brtrue.s   IL_017e

    IL_0173:  ldloca.s   V_2
    IL_0175:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_017b:  ldloc.2
    IL_017c:  br.s       IL_0192

    IL_017e:  ldloca.s   V_1
    IL_0180:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0185:  ldloca.s   V_0
    IL_0187:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_018c:  mul
    IL_018d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0192:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0197:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_019c:  ldarg.0
    IL_019d:  stloc.0
    IL_019e:  ldarg.1
    IL_019f:  stloc.1
    IL_01a0:  ldloca.s   V_0
    IL_01a2:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01a7:  ldloca.s   V_1
    IL_01a9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01ae:  and
    IL_01af:  brtrue.s   IL_01bc

    IL_01b1:  ldloca.s   V_2
    IL_01b3:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01b9:  ldloc.2
    IL_01ba:  br.s       IL_01d0

    IL_01bc:  ldloca.s   V_0
    IL_01be:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01c3:  ldloca.s   V_1
    IL_01c5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01ca:  div
    IL_01cb:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01d0:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01d5:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01da:  ldarg.0
    IL_01db:  stloc.1
    IL_01dc:  ldarg.1
    IL_01dd:  stloc.0
    IL_01de:  ldloca.s   V_1
    IL_01e0:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01e5:  ldloca.s   V_0
    IL_01e7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01ec:  and
    IL_01ed:  brtrue.s   IL_01fa

    IL_01ef:  ldloca.s   V_2
    IL_01f1:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01f7:  ldloc.2
    IL_01f8:  br.s       IL_020e

    IL_01fa:  ldloca.s   V_1
    IL_01fc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0201:  ldloca.s   V_0
    IL_0203:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0208:  rem
    IL_0209:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_020e:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0213:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0218:  ldarg.0
    IL_0219:  stloc.0
    IL_021a:  ldarg.1
    IL_021b:  stloc.1
    IL_021c:  ldloca.s   V_0
    IL_021e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0223:  ldloca.s   V_1
    IL_0225:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_022a:  and
    IL_022b:  brtrue.s   IL_0238

    IL_022d:  ldloca.s   V_2
    IL_022f:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0235:  ldloc.2
    IL_0236:  br.s       IL_024c

    IL_0238:  ldloca.s   V_0
    IL_023a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_023f:  ldloca.s   V_1
    IL_0241:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0246:  and
    IL_0247:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_024c:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0251:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0256:  ldarg.0
    IL_0257:  stloc.1
    IL_0258:  ldarg.1
    IL_0259:  stloc.0
    IL_025a:  ldloca.s   V_1
    IL_025c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0261:  ldloca.s   V_0
    IL_0263:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0268:  and
    IL_0269:  brtrue.s   IL_0276

    IL_026b:  ldloca.s   V_2
    IL_026d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0273:  ldloc.2
    IL_0274:  br.s       IL_028a

    IL_0276:  ldloca.s   V_1
    IL_0278:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_027d:  ldloca.s   V_0
    IL_027f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0284:  or
    IL_0285:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_028a:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_028f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0294:  ldarg.0
    IL_0295:  stloc.0
    IL_0296:  ldarg.1
    IL_0297:  stloc.1
    IL_0298:  ldloca.s   V_0
    IL_029a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_029f:  ldloca.s   V_1
    IL_02a1:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02a6:  and
    IL_02a7:  brtrue.s   IL_02b4

    IL_02a9:  ldloca.s   V_2
    IL_02ab:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02b1:  ldloc.2
    IL_02b2:  br.s       IL_02c8

    IL_02b4:  ldloca.s   V_0
    IL_02b6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02bb:  ldloca.s   V_1
    IL_02bd:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02c2:  xor
    IL_02c3:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02c8:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_02cd:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02d2:  ldarg.0
    IL_02d3:  stloc.1
    IL_02d4:  ldarg.1
    IL_02d5:  stloc.0
    IL_02d6:  ldloca.s   V_1
    IL_02d8:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02dd:  ldloca.s   V_0
    IL_02df:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02e4:  and
    IL_02e5:  brtrue.s   IL_02f2

    IL_02e7:  ldloca.s   V_2
    IL_02e9:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02ef:  ldloc.2
    IL_02f0:  br.s       IL_0309

    IL_02f2:  ldloca.s   V_1
    IL_02f4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02f9:  ldloca.s   V_0
    IL_02fb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0300:  ldc.i4.s   31
    IL_0302:  and
    IL_0303:  shl
    IL_0304:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0309:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_030e:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0313:  ldarg.0
    IL_0314:  stloc.0
    IL_0315:  ldarg.1
    IL_0316:  stloc.1
    IL_0317:  ldloca.s   V_0
    IL_0319:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_031e:  ldloca.s   V_1
    IL_0320:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0325:  and
    IL_0326:  brtrue.s   IL_0333

    IL_0328:  ldloca.s   V_2
    IL_032a:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0330:  ldloc.2
    IL_0331:  br.s       IL_034a

    IL_0333:  ldloca.s   V_0
    IL_0335:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_033a:  ldloca.s   V_1
    IL_033c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0341:  ldc.i4.s   31
    IL_0343:  and
    IL_0344:  shr
    IL_0345:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_034a:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_034f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0354:  ldarg.0
    IL_0355:  stloc.1
    IL_0356:  ldloca.s   V_1
    IL_0358:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_035d:  brtrue.s   IL_0362

    IL_035f:  ldarg.1
    IL_0360:  br.s       IL_0363

    IL_0362:  ldloc.1
    IL_0363:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0368:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_036d:  ldarg.0
    IL_036e:  stloc.1
    IL_036f:  ldloca.s   V_1
    IL_0371:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0376:  brtrue.s   IL_0383

    IL_0378:  ldloca.s   V_0
    IL_037a:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0380:  ldloc.0
    IL_0381:  br.s       IL_0390

    IL_0383:  ldloca.s   V_1
    IL_0385:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_038a:  neg
    IL_038b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0390:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0395:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_039a:  ldarg.0
    IL_039b:  stloc.1
    IL_039c:  ldloca.s   V_1
    IL_039e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03a3:  brtrue.s   IL_03b0

    IL_03a5:  ldloca.s   V_0
    IL_03a7:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03ad:  ldloc.0
    IL_03ae:  br.s       IL_03bd

    IL_03b0:  ldloca.s   V_1
    IL_03b2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03b7:  not
    IL_03b8:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03bd:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_03c2:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_03c7:  ldarg.0
    IL_03c8:  stloc.1
    IL_03c9:  ldloca.s   V_1
    IL_03cb:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03d0:  brtrue.s   IL_03dd

    IL_03d2:  ldloca.s   V_0
    IL_03d4:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03da:  ldloc.0
    IL_03db:  br.s       IL_03eb

    IL_03dd:  ldloca.s   V_1
    IL_03df:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03e4:  ldc.i4.1
    IL_03e5:  add
    IL_03e6:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03eb:  dup
    IL_03ec:  starg.s    a
    IL_03ee:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_03f3:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_03f8:  ldarg.0
    IL_03f9:  stloc.1
    IL_03fa:  ldloca.s   V_1
    IL_03fc:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0401:  brtrue.s   IL_040e

    IL_0403:  ldloca.s   V_0
    IL_0405:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_040b:  ldloc.0
    IL_040c:  br.s       IL_041c

    IL_040e:  ldloca.s   V_1
    IL_0410:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0415:  ldc.i4.1
    IL_0416:  sub
    IL_0417:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_041c:  dup
    IL_041d:  starg.s    a
    IL_041f:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0424:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0429:  ldarg.0
    IL_042a:  stloc.1
    IL_042b:  ldarg.1
    IL_042c:  stloc.0
    IL_042d:  ldloca.s   V_1
    IL_042f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0434:  ldloca.s   V_0
    IL_0436:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_043b:  and
    IL_043c:  brtrue.s   IL_0449

    IL_043e:  ldloca.s   V_2
    IL_0440:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0446:  ldloc.2
    IL_0447:  br.s       IL_045d

    IL_0449:  ldloca.s   V_1
    IL_044b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0450:  ldloca.s   V_0
    IL_0452:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0457:  add
    IL_0458:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_045d:  starg.s    a
    IL_045f:  ldarg.0
    IL_0460:  stloc.0
    IL_0461:  ldarg.1
    IL_0462:  stloc.1
    IL_0463:  ldloca.s   V_0
    IL_0465:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_046a:  ldloca.s   V_1
    IL_046c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0471:  and
    IL_0472:  brtrue.s   IL_047f

    IL_0474:  ldloca.s   V_2
    IL_0476:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_047c:  ldloc.2
    IL_047d:  br.s       IL_0493

    IL_047f:  ldloca.s   V_0
    IL_0481:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0486:  ldloca.s   V_1
    IL_0488:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_048d:  sub
    IL_048e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0493:  starg.s    a
    IL_0495:  ldarg.0
    IL_0496:  stloc.1
    IL_0497:  ldarg.1
    IL_0498:  stloc.0
    IL_0499:  ldloca.s   V_1
    IL_049b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04a0:  ldloca.s   V_0
    IL_04a2:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04a7:  and
    IL_04a8:  brtrue.s   IL_04b5

    IL_04aa:  ldloca.s   V_2
    IL_04ac:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04b2:  ldloc.2
    IL_04b3:  br.s       IL_04c9

    IL_04b5:  ldloca.s   V_1
    IL_04b7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04bc:  ldloca.s   V_0
    IL_04be:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04c3:  mul
    IL_04c4:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04c9:  starg.s    a
    IL_04cb:  ldarg.0
    IL_04cc:  stloc.0
    IL_04cd:  ldarg.1
    IL_04ce:  stloc.1
    IL_04cf:  ldloca.s   V_0
    IL_04d1:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04d6:  ldloca.s   V_1
    IL_04d8:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04dd:  and
    IL_04de:  brtrue.s   IL_04eb

    IL_04e0:  ldloca.s   V_2
    IL_04e2:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04e8:  ldloc.2
    IL_04e9:  br.s       IL_04ff

    IL_04eb:  ldloca.s   V_0
    IL_04ed:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04f2:  ldloca.s   V_1
    IL_04f4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04f9:  div
    IL_04fa:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04ff:  starg.s    a
    IL_0501:  ldarg.0
    IL_0502:  stloc.1
    IL_0503:  ldarg.1
    IL_0504:  stloc.0
    IL_0505:  ldloca.s   V_1
    IL_0507:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_050c:  ldloca.s   V_0
    IL_050e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0513:  and
    IL_0514:  brtrue.s   IL_0521

    IL_0516:  ldloca.s   V_2
    IL_0518:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_051e:  ldloc.2
    IL_051f:  br.s       IL_0535

    IL_0521:  ldloca.s   V_1
    IL_0523:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0528:  ldloca.s   V_0
    IL_052a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_052f:  rem
    IL_0530:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0535:  starg.s    a
    IL_0537:  ldarg.0
    IL_0538:  stloc.0
    IL_0539:  ldarg.1
    IL_053a:  stloc.1
    IL_053b:  ldloca.s   V_0
    IL_053d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0542:  ldloca.s   V_1
    IL_0544:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0549:  and
    IL_054a:  brtrue.s   IL_0557

    IL_054c:  ldloca.s   V_2
    IL_054e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0554:  ldloc.2
    IL_0555:  br.s       IL_056b

    IL_0557:  ldloca.s   V_0
    IL_0559:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_055e:  ldloca.s   V_1
    IL_0560:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0565:  and
    IL_0566:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_056b:  starg.s    a
    IL_056d:  ldarg.0
    IL_056e:  stloc.1
    IL_056f:  ldarg.1
    IL_0570:  stloc.0
    IL_0571:  ldloca.s   V_1
    IL_0573:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0578:  ldloca.s   V_0
    IL_057a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_057f:  and
    IL_0580:  brtrue.s   IL_058d

    IL_0582:  ldloca.s   V_2
    IL_0584:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_058a:  ldloc.2
    IL_058b:  br.s       IL_05a1

    IL_058d:  ldloca.s   V_1
    IL_058f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0594:  ldloca.s   V_0
    IL_0596:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_059b:  or
    IL_059c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_05a1:  starg.s    a
    IL_05a3:  ldarg.0
    IL_05a4:  stloc.0
    IL_05a5:  ldarg.1
    IL_05a6:  stloc.1
    IL_05a7:  ldloca.s   V_0
    IL_05a9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_05ae:  ldloca.s   V_1
    IL_05b0:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_05b5:  and
    IL_05b6:  brtrue.s   IL_05c3

    IL_05b8:  ldloca.s   V_2
    IL_05ba:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_05c0:  ldloc.2
    IL_05c1:  br.s       IL_05d7

    IL_05c3:  ldloca.s   V_0
    IL_05c5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_05ca:  ldloca.s   V_1
    IL_05cc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_05d1:  xor
    IL_05d2:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_05d7:  starg.s    a
    IL_05d9:  ldarg.0
    IL_05da:  stloc.1
    IL_05db:  ldarg.1
    IL_05dc:  stloc.0
    IL_05dd:  ldloca.s   V_1
    IL_05df:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_05e4:  ldloca.s   V_0
    IL_05e6:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_05eb:  and
    IL_05ec:  brtrue.s   IL_05f9

    IL_05ee:  ldloca.s   V_2
    IL_05f0:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_05f6:  ldloc.2
    IL_05f7:  br.s       IL_0610

    IL_05f9:  ldloca.s   V_1
    IL_05fb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0600:  ldloca.s   V_0
    IL_0602:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0607:  ldc.i4.s   31
    IL_0609:  and
    IL_060a:  shl
    IL_060b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0610:  starg.s    a
    IL_0612:  ldarg.0
    IL_0613:  stloc.0
    IL_0614:  ldarg.1
    IL_0615:  stloc.1
    IL_0616:  ldloca.s   V_0
    IL_0618:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_061d:  ldloca.s   V_1
    IL_061f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0624:  and
    IL_0625:  brtrue.s   IL_0632

    IL_0627:  ldloca.s   V_2
    IL_0629:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_062f:  ldloc.2
    IL_0630:  br.s       IL_0649

    IL_0632:  ldloca.s   V_0
    IL_0634:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0639:  ldloca.s   V_1
    IL_063b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0640:  ldc.i4.s   31
    IL_0642:  and
    IL_0643:  shr
    IL_0644:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0649:  starg.s    a
    IL_064b:  ret
  } // end of method LiftedOperators::IntValueBasic

  .method public hidebysig static void  IntValueComplex(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                        class [mscorlib]System.Func`1<int32> x) cil managed
  {
    // Code size       1346 (0x542)
    .maxstack  3
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             int32 V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0008:  stloc.1
    IL_0009:  ldloca.s   V_0
    IL_000b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0010:  ldloc.1
    IL_0011:  beq.s      IL_0016

    IL_0013:  ldc.i4.0
    IL_0014:  br.s       IL_001d

    IL_0016:  ldloca.s   V_0
    IL_0018:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001d:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0022:  ldarg.0
    IL_0023:  stloc.0
    IL_0024:  ldarg.1
    IL_0025:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_002a:  stloc.1
    IL_002b:  ldloca.s   V_0
    IL_002d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0032:  ldloc.1
    IL_0033:  beq.s      IL_0038

    IL_0035:  ldc.i4.1
    IL_0036:  br.s       IL_0042

    IL_0038:  ldloca.s   V_0
    IL_003a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_003f:  ldc.i4.0
    IL_0040:  ceq
    IL_0042:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0047:  ldarg.0
    IL_0048:  stloc.0
    IL_0049:  ldarg.1
    IL_004a:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_004f:  stloc.1
    IL_0050:  ldloca.s   V_0
    IL_0052:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0057:  ldloc.1
    IL_0058:  bgt.s      IL_005d

    IL_005a:  ldc.i4.0
    IL_005b:  br.s       IL_0064

    IL_005d:  ldloca.s   V_0
    IL_005f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0064:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0069:  ldarg.1
    IL_006a:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_006f:  ldarg.0
    IL_0070:  stloc.0
    IL_0071:  ldloca.s   V_0
    IL_0073:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0078:  beq.s      IL_007d

    IL_007a:  ldc.i4.0
    IL_007b:  br.s       IL_0084

    IL_007d:  ldloca.s   V_0
    IL_007f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0084:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0089:  ldarg.1
    IL_008a:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_008f:  ldarg.0
    IL_0090:  stloc.0
    IL_0091:  ldloca.s   V_0
    IL_0093:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0098:  beq.s      IL_009d

    IL_009a:  ldc.i4.1
    IL_009b:  br.s       IL_00a7

    IL_009d:  ldloca.s   V_0
    IL_009f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00a4:  ldc.i4.0
    IL_00a5:  ceq
    IL_00a7:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00ac:  ldarg.1
    IL_00ad:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00b2:  ldarg.0
    IL_00b3:  stloc.0
    IL_00b4:  ldloca.s   V_0
    IL_00b6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00bb:  bgt.s      IL_00c0

    IL_00bd:  ldc.i4.0
    IL_00be:  br.s       IL_00c7

    IL_00c0:  ldloca.s   V_0
    IL_00c2:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00c7:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00cc:  ldarg.0
    IL_00cd:  stloc.0
    IL_00ce:  ldarg.1
    IL_00cf:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00d4:  stloc.1
    IL_00d5:  ldloca.s   V_0
    IL_00d7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00dc:  brtrue.s   IL_00e9

    IL_00de:  ldloca.s   V_2
    IL_00e0:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00e6:  ldloc.2
    IL_00e7:  br.s       IL_00f7

    IL_00e9:  ldloca.s   V_0
    IL_00eb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00f0:  ldloc.1
    IL_00f1:  add
    IL_00f2:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00f7:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_00fc:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0101:  ldarg.0
    IL_0102:  stloc.0
    IL_0103:  ldarg.1
    IL_0104:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0109:  stloc.1
    IL_010a:  ldloca.s   V_0
    IL_010c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0111:  brtrue.s   IL_011e

    IL_0113:  ldloca.s   V_2
    IL_0115:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_011b:  ldloc.2
    IL_011c:  br.s       IL_012c

    IL_011e:  ldloca.s   V_0
    IL_0120:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0125:  ldloc.1
    IL_0126:  sub
    IL_0127:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_012c:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0131:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0136:  ldarg.0
    IL_0137:  stloc.0
    IL_0138:  ldarg.1
    IL_0139:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_013e:  stloc.1
    IL_013f:  ldloca.s   V_0
    IL_0141:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0146:  brtrue.s   IL_0153

    IL_0148:  ldloca.s   V_2
    IL_014a:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0150:  ldloc.2
    IL_0151:  br.s       IL_0161

    IL_0153:  ldloca.s   V_0
    IL_0155:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_015a:  ldloc.1
    IL_015b:  mul
    IL_015c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0161:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0166:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_016b:  ldarg.0
    IL_016c:  stloc.0
    IL_016d:  ldarg.1
    IL_016e:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0173:  stloc.1
    IL_0174:  ldloca.s   V_0
    IL_0176:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_017b:  brtrue.s   IL_0188

    IL_017d:  ldloca.s   V_2
    IL_017f:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0185:  ldloc.2
    IL_0186:  br.s       IL_0196

    IL_0188:  ldloca.s   V_0
    IL_018a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_018f:  ldloc.1
    IL_0190:  div
    IL_0191:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0196:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_019b:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01a0:  ldarg.0
    IL_01a1:  stloc.0
    IL_01a2:  ldarg.1
    IL_01a3:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_01a8:  stloc.1
    IL_01a9:  ldloca.s   V_0
    IL_01ab:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01b0:  brtrue.s   IL_01bd

    IL_01b2:  ldloca.s   V_2
    IL_01b4:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01ba:  ldloc.2
    IL_01bb:  br.s       IL_01cb

    IL_01bd:  ldloca.s   V_0
    IL_01bf:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01c4:  ldloc.1
    IL_01c5:  rem
    IL_01c6:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01cb:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01d0:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01d5:  ldarg.0
    IL_01d6:  stloc.0
    IL_01d7:  ldarg.1
    IL_01d8:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_01dd:  stloc.1
    IL_01de:  ldloca.s   V_0
    IL_01e0:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01e5:  brtrue.s   IL_01f2

    IL_01e7:  ldloca.s   V_2
    IL_01e9:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01ef:  ldloc.2
    IL_01f0:  br.s       IL_0200

    IL_01f2:  ldloca.s   V_0
    IL_01f4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01f9:  ldloc.1
    IL_01fa:  and
    IL_01fb:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0200:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0205:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_020a:  ldarg.0
    IL_020b:  stloc.0
    IL_020c:  ldarg.1
    IL_020d:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0212:  stloc.1
    IL_0213:  ldloca.s   V_0
    IL_0215:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_021a:  brtrue.s   IL_0227

    IL_021c:  ldloca.s   V_2
    IL_021e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0224:  ldloc.2
    IL_0225:  br.s       IL_0235

    IL_0227:  ldloca.s   V_0
    IL_0229:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_022e:  ldloc.1
    IL_022f:  or
    IL_0230:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0235:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_023a:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_023f:  ldarg.0
    IL_0240:  stloc.0
    IL_0241:  ldarg.1
    IL_0242:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0247:  stloc.1
    IL_0248:  ldloca.s   V_0
    IL_024a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_024f:  brtrue.s   IL_025c

    IL_0251:  ldloca.s   V_2
    IL_0253:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0259:  ldloc.2
    IL_025a:  br.s       IL_026a

    IL_025c:  ldloca.s   V_0
    IL_025e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0263:  ldloc.1
    IL_0264:  xor
    IL_0265:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_026a:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_026f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0274:  ldarg.0
    IL_0275:  stloc.0
    IL_0276:  ldarg.1
    IL_0277:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_027c:  stloc.1
    IL_027d:  ldloca.s   V_0
    IL_027f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0284:  brtrue.s   IL_0291

    IL_0286:  ldloca.s   V_2
    IL_0288:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_028e:  ldloc.2
    IL_028f:  br.s       IL_02a2

    IL_0291:  ldloca.s   V_0
    IL_0293:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0298:  ldloc.1
    IL_0299:  ldc.i4.s   31
    IL_029b:  and
    IL_029c:  shl
    IL_029d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02a2:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_02a7:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02ac:  ldarg.0
    IL_02ad:  stloc.0
    IL_02ae:  ldarg.1
    IL_02af:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_02b4:  stloc.1
    IL_02b5:  ldloca.s   V_0
    IL_02b7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02bc:  brtrue.s   IL_02c9

    IL_02be:  ldloca.s   V_2
    IL_02c0:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02c6:  ldloc.2
    IL_02c7:  br.s       IL_02da

    IL_02c9:  ldloca.s   V_0
    IL_02cb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02d0:  ldloc.1
    IL_02d1:  ldc.i4.s   31
    IL_02d3:  and
    IL_02d4:  shr
    IL_02d5:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02da:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_02df:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02e4:  ldarg.0
    IL_02e5:  stloc.0
    IL_02e6:  ldloca.s   V_0
    IL_02e8:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02ed:  brtrue.s   IL_02f7

    IL_02ef:  ldarg.1
    IL_02f0:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_02f5:  br.s       IL_02fe

    IL_02f7:  ldloca.s   V_0
    IL_02f9:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02fe:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0303:  ldarg.0
    IL_0304:  stloc.0
    IL_0305:  ldarg.1
    IL_0306:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_030b:  stloc.1
    IL_030c:  ldloca.s   V_0
    IL_030e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0313:  brtrue.s   IL_0320

    IL_0315:  ldloca.s   V_2
    IL_0317:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_031d:  ldloc.2
    IL_031e:  br.s       IL_032e

    IL_0320:  ldloca.s   V_0
    IL_0322:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0327:  ldloc.1
    IL_0328:  add
    IL_0329:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_032e:  starg.s    a
    IL_0330:  ldarg.0
    IL_0331:  stloc.0
    IL_0332:  ldarg.1
    IL_0333:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0338:  stloc.1
    IL_0339:  ldloca.s   V_0
    IL_033b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0340:  brtrue.s   IL_034d

    IL_0342:  ldloca.s   V_2
    IL_0344:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_034a:  ldloc.2
    IL_034b:  br.s       IL_035b

    IL_034d:  ldloca.s   V_0
    IL_034f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0354:  ldloc.1
    IL_0355:  sub
    IL_0356:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_035b:  starg.s    a
    IL_035d:  ldarg.0
    IL_035e:  stloc.0
    IL_035f:  ldarg.1
    IL_0360:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0365:  stloc.1
    IL_0366:  ldloca.s   V_0
    IL_0368:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_036d:  brtrue.s   IL_037a

    IL_036f:  ldloca.s   V_2
    IL_0371:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0377:  ldloc.2
    IL_0378:  br.s       IL_0388

    IL_037a:  ldloca.s   V_0
    IL_037c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0381:  ldloc.1
    IL_0382:  mul
    IL_0383:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0388:  starg.s    a
    IL_038a:  ldarg.0
    IL_038b:  stloc.0
    IL_038c:  ldarg.1
    IL_038d:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0392:  stloc.1
    IL_0393:  ldloca.s   V_0
    IL_0395:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_039a:  brtrue.s   IL_03a7

    IL_039c:  ldloca.s   V_2
    IL_039e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03a4:  ldloc.2
    IL_03a5:  br.s       IL_03b5

    IL_03a7:  ldloca.s   V_0
    IL_03a9:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03ae:  ldloc.1
    IL_03af:  div
    IL_03b0:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03b5:  starg.s    a
    IL_03b7:  ldarg.0
    IL_03b8:  stloc.0
    IL_03b9:  ldarg.1
    IL_03ba:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_03bf:  stloc.1
    IL_03c0:  ldloca.s   V_0
    IL_03c2:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03c7:  brtrue.s   IL_03d4

    IL_03c9:  ldloca.s   V_2
    IL_03cb:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03d1:  ldloc.2
    IL_03d2:  br.s       IL_03e2

    IL_03d4:  ldloca.s   V_0
    IL_03d6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03db:  ldloc.1
    IL_03dc:  rem
    IL_03dd:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03e2:  starg.s    a
    IL_03e4:  ldarg.0
    IL_03e5:  stloc.0
    IL_03e6:  ldarg.1
    IL_03e7:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_03ec:  stloc.1
    IL_03ed:  ldloca.s   V_0
    IL_03ef:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03f4:  brtrue.s   IL_0401

    IL_03f6:  ldloca.s   V_2
    IL_03f8:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03fe:  ldloc.2
    IL_03ff:  br.s       IL_040f

    IL_0401:  ldloca.s   V_0
    IL_0403:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0408:  ldloc.1
    IL_0409:  and
    IL_040a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_040f:  starg.s    a
    IL_0411:  ldarg.0
    IL_0412:  stloc.0
    IL_0413:  ldarg.1
    IL_0414:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0419:  stloc.1
    IL_041a:  ldloca.s   V_0
    IL_041c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0421:  brtrue.s   IL_042e

    IL_0423:  ldloca.s   V_2
    IL_0425:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_042b:  ldloc.2
    IL_042c:  br.s       IL_043c

    IL_042e:  ldloca.s   V_0
    IL_0430:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0435:  ldloc.1
    IL_0436:  or
    IL_0437:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_043c:  starg.s    a
    IL_043e:  ldarg.0
    IL_043f:  stloc.0
    IL_0440:  ldarg.1
    IL_0441:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0446:  stloc.1
    IL_0447:  ldloca.s   V_0
    IL_0449:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_044e:  brtrue.s   IL_045b

    IL_0450:  ldloca.s   V_2
    IL_0452:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0458:  ldloc.2
    IL_0459:  br.s       IL_0469

    IL_045b:  ldloca.s   V_0
    IL_045d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0462:  ldloc.1
    IL_0463:  xor
    IL_0464:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0469:  starg.s    a
    IL_046b:  ldarg.0
    IL_046c:  stloc.0
    IL_046d:  ldarg.1
    IL_046e:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0473:  stloc.1
    IL_0474:  ldloca.s   V_0
    IL_0476:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_047b:  brtrue.s   IL_0488

    IL_047d:  ldloca.s   V_2
    IL_047f:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0485:  ldloc.2
    IL_0486:  br.s       IL_0499

    IL_0488:  ldloca.s   V_0
    IL_048a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_048f:  ldloc.1
    IL_0490:  ldc.i4.s   31
    IL_0492:  and
    IL_0493:  shl
    IL_0494:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0499:  starg.s    a
    IL_049b:  ldarg.0
    IL_049c:  stloc.0
    IL_049d:  ldarg.1
    IL_049e:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_04a3:  stloc.1
    IL_04a4:  ldloca.s   V_0
    IL_04a6:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04ab:  brtrue.s   IL_04b8

    IL_04ad:  ldloca.s   V_2
    IL_04af:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04b5:  ldloc.2
    IL_04b6:  br.s       IL_04c9

    IL_04b8:  ldloca.s   V_0
    IL_04ba:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04bf:  ldloc.1
    IL_04c0:  ldc.i4.s   31
    IL_04c2:  and
    IL_04c3:  shr
    IL_04c4:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04c9:  starg.s    a
    IL_04cb:  ldarg.1
    IL_04cc:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_04d1:  stloc.1
    IL_04d2:  ldarg.0
    IL_04d3:  stloc.0
    IL_04d4:  ldloca.s   V_0
    IL_04d6:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04db:  brtrue.s   IL_04e8

    IL_04dd:  ldloca.s   V_2
    IL_04df:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04e5:  ldloc.2
    IL_04e6:  br.s       IL_04f6

    IL_04e8:  ldloc.1
    IL_04e9:  ldloca.s   V_0
    IL_04eb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04f0:  add
    IL_04f1:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04f6:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_04fb:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0500:  ldc.i4.0
    IL_0501:  newarr     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0506:  ldc.i4.0
    IL_0507:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_050c:  dup
    IL_050d:  ldobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0512:  stloc.0
    IL_0513:  ldarg.1
    IL_0514:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0519:  stloc.1
    IL_051a:  ldloca.s   V_0
    IL_051c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0521:  brtrue.s   IL_052e

    IL_0523:  ldloca.s   V_2
    IL_0525:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_052b:  ldloc.2
    IL_052c:  br.s       IL_053c

    IL_052e:  ldloca.s   V_0
    IL_0530:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0535:  ldloc.1
    IL_0536:  add
    IL_0537:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_053c:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0541:  ret
  } // end of method LiftedOperators::IntValueComplex

  .method public hidebysig static void  IntValueConst(valuetype [mscorlib]System.Nullable`1<int32> a) cil managed
  {
    // Code size       1087 (0x43f)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             int32 V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.2
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000b:  ldloc.1
    IL_000c:  beq.s      IL_0011

    IL_000e:  ldc.i4.0
    IL_000f:  br.s       IL_0018

    IL_0011:  ldloca.s   V_0
    IL_0013:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0018:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_001d:  ldarg.0
    IL_001e:  stloc.0
    IL_001f:  ldc.i4.2
    IL_0020:  stloc.1
    IL_0021:  ldloca.s   V_0
    IL_0023:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0028:  ldloc.1
    IL_0029:  beq.s      IL_002e

    IL_002b:  ldc.i4.1
    IL_002c:  br.s       IL_0038

    IL_002e:  ldloca.s   V_0
    IL_0030:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0035:  ldc.i4.0
    IL_0036:  ceq
    IL_0038:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_003d:  ldarg.0
    IL_003e:  stloc.0
    IL_003f:  ldc.i4.2
    IL_0040:  stloc.1
    IL_0041:  ldloca.s   V_0
    IL_0043:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0048:  ldloc.1
    IL_0049:  bgt.s      IL_004e

    IL_004b:  ldc.i4.0
    IL_004c:  br.s       IL_0055

    IL_004e:  ldloca.s   V_0
    IL_0050:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0055:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_005a:  ldc.i4.2
    IL_005b:  ldarg.0
    IL_005c:  stloc.0
    IL_005d:  ldloca.s   V_0
    IL_005f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0064:  beq.s      IL_0069

    IL_0066:  ldc.i4.0
    IL_0067:  br.s       IL_0070

    IL_0069:  ldloca.s   V_0
    IL_006b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0070:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0075:  ldc.i4.2
    IL_0076:  ldarg.0
    IL_0077:  stloc.0
    IL_0078:  ldloca.s   V_0
    IL_007a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_007f:  beq.s      IL_0084

    IL_0081:  ldc.i4.1
    IL_0082:  br.s       IL_008e

    IL_0084:  ldloca.s   V_0
    IL_0086:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_008b:  ldc.i4.0
    IL_008c:  ceq
    IL_008e:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0093:  ldc.i4.2
    IL_0094:  ldarg.0
    IL_0095:  stloc.0
    IL_0096:  ldloca.s   V_0
    IL_0098:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_009d:  bgt.s      IL_00a2

    IL_009f:  ldc.i4.0
    IL_00a0:  br.s       IL_00a9

    IL_00a2:  ldloca.s   V_0
    IL_00a4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00a9:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00ae:  ldarg.0
    IL_00af:  stloc.0
    IL_00b0:  ldloca.s   V_0
    IL_00b2:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00b7:  brtrue.s   IL_00c4

    IL_00b9:  ldloca.s   V_2
    IL_00bb:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00c1:  ldloc.2
    IL_00c2:  br.s       IL_00d2

    IL_00c4:  ldloca.s   V_0
    IL_00c6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00cb:  ldc.i4.2
    IL_00cc:  add
    IL_00cd:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00d2:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_00d7:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00dc:  ldarg.0
    IL_00dd:  stloc.0
    IL_00de:  ldloca.s   V_0
    IL_00e0:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00e5:  brtrue.s   IL_00f2

    IL_00e7:  ldloca.s   V_2
    IL_00e9:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00ef:  ldloc.2
    IL_00f0:  br.s       IL_0100

    IL_00f2:  ldloca.s   V_0
    IL_00f4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00f9:  ldc.i4.2
    IL_00fa:  sub
    IL_00fb:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0100:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0105:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_010a:  ldarg.0
    IL_010b:  stloc.0
    IL_010c:  ldloca.s   V_0
    IL_010e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0113:  brtrue.s   IL_0120

    IL_0115:  ldloca.s   V_2
    IL_0117:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_011d:  ldloc.2
    IL_011e:  br.s       IL_012e

    IL_0120:  ldloca.s   V_0
    IL_0122:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0127:  ldc.i4.2
    IL_0128:  mul
    IL_0129:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_012e:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0133:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0138:  ldarg.0
    IL_0139:  stloc.0
    IL_013a:  ldloca.s   V_0
    IL_013c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0141:  brtrue.s   IL_014e

    IL_0143:  ldloca.s   V_2
    IL_0145:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_014b:  ldloc.2
    IL_014c:  br.s       IL_015c

    IL_014e:  ldloca.s   V_0
    IL_0150:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0155:  ldc.i4.2
    IL_0156:  div
    IL_0157:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_015c:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0161:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0166:  ldarg.0
    IL_0167:  stloc.0
    IL_0168:  ldloca.s   V_0
    IL_016a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_016f:  brtrue.s   IL_017c

    IL_0171:  ldloca.s   V_2
    IL_0173:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0179:  ldloc.2
    IL_017a:  br.s       IL_018a

    IL_017c:  ldloca.s   V_0
    IL_017e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0183:  ldc.i4.2
    IL_0184:  rem
    IL_0185:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_018a:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_018f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0194:  ldarg.0
    IL_0195:  stloc.0
    IL_0196:  ldloca.s   V_0
    IL_0198:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_019d:  brtrue.s   IL_01aa

    IL_019f:  ldloca.s   V_2
    IL_01a1:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01a7:  ldloc.2
    IL_01a8:  br.s       IL_01b8

    IL_01aa:  ldloca.s   V_0
    IL_01ac:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01b1:  ldc.i4.2
    IL_01b2:  and
    IL_01b3:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01b8:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01bd:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01c2:  ldarg.0
    IL_01c3:  stloc.0
    IL_01c4:  ldloca.s   V_0
    IL_01c6:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01cb:  brtrue.s   IL_01d8

    IL_01cd:  ldloca.s   V_2
    IL_01cf:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01d5:  ldloc.2
    IL_01d6:  br.s       IL_01e6

    IL_01d8:  ldloca.s   V_0
    IL_01da:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01df:  ldc.i4.2
    IL_01e0:  or
    IL_01e1:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01e6:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01eb:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01f0:  ldarg.0
    IL_01f1:  stloc.0
    IL_01f2:  ldloca.s   V_0
    IL_01f4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01f9:  brtrue.s   IL_0206

    IL_01fb:  ldloca.s   V_2
    IL_01fd:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0203:  ldloc.2
    IL_0204:  br.s       IL_0214

    IL_0206:  ldloca.s   V_0
    IL_0208:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_020d:  ldc.i4.2
    IL_020e:  xor
    IL_020f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0214:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0219:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_021e:  ldarg.0
    IL_021f:  stloc.0
    IL_0220:  ldloca.s   V_0
    IL_0222:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0227:  brtrue.s   IL_0234

    IL_0229:  ldloca.s   V_2
    IL_022b:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0231:  ldloc.2
    IL_0232:  br.s       IL_0242

    IL_0234:  ldloca.s   V_0
    IL_0236:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_023b:  ldc.i4.2
    IL_023c:  shl
    IL_023d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0242:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0247:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_024c:  ldarg.0
    IL_024d:  stloc.0
    IL_024e:  ldloca.s   V_0
    IL_0250:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0255:  brtrue.s   IL_0262

    IL_0257:  ldloca.s   V_2
    IL_0259:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_025f:  ldloc.2
    IL_0260:  br.s       IL_0270

    IL_0262:  ldloca.s   V_0
    IL_0264:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0269:  ldc.i4.2
    IL_026a:  shr
    IL_026b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0270:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0275:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_027a:  ldarg.0
    IL_027b:  stloc.0
    IL_027c:  ldloca.s   V_0
    IL_027e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0283:  brtrue.s   IL_0288

    IL_0285:  ldc.i4.2
    IL_0286:  br.s       IL_028f

    IL_0288:  ldloca.s   V_0
    IL_028a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_028f:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0294:  ldarg.0
    IL_0295:  stloc.0
    IL_0296:  ldloca.s   V_0
    IL_0298:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_029d:  brtrue.s   IL_02aa

    IL_029f:  ldloca.s   V_2
    IL_02a1:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02a7:  ldloc.2
    IL_02a8:  br.s       IL_02b8

    IL_02aa:  ldloca.s   V_0
    IL_02ac:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02b1:  ldc.i4.2
    IL_02b2:  add
    IL_02b3:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02b8:  starg.s    a
    IL_02ba:  ldarg.0
    IL_02bb:  stloc.0
    IL_02bc:  ldloca.s   V_0
    IL_02be:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02c3:  brtrue.s   IL_02d0

    IL_02c5:  ldloca.s   V_2
    IL_02c7:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02cd:  ldloc.2
    IL_02ce:  br.s       IL_02de

    IL_02d0:  ldloca.s   V_0
    IL_02d2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02d7:  ldc.i4.2
    IL_02d8:  sub
    IL_02d9:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02de:  starg.s    a
    IL_02e0:  ldarg.0
    IL_02e1:  stloc.0
    IL_02e2:  ldloca.s   V_0
    IL_02e4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02e9:  brtrue.s   IL_02f6

    IL_02eb:  ldloca.s   V_2
    IL_02ed:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02f3:  ldloc.2
    IL_02f4:  br.s       IL_0304

    IL_02f6:  ldloca.s   V_0
    IL_02f8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02fd:  ldc.i4.2
    IL_02fe:  mul
    IL_02ff:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0304:  starg.s    a
    IL_0306:  ldarg.0
    IL_0307:  stloc.0
    IL_0308:  ldloca.s   V_0
    IL_030a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_030f:  brtrue.s   IL_031c

    IL_0311:  ldloca.s   V_2
    IL_0313:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0319:  ldloc.2
    IL_031a:  br.s       IL_032a

    IL_031c:  ldloca.s   V_0
    IL_031e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0323:  ldc.i4.2
    IL_0324:  div
    IL_0325:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_032a:  starg.s    a
    IL_032c:  ldarg.0
    IL_032d:  stloc.0
    IL_032e:  ldloca.s   V_0
    IL_0330:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0335:  brtrue.s   IL_0342

    IL_0337:  ldloca.s   V_2
    IL_0339:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_033f:  ldloc.2
    IL_0340:  br.s       IL_0350

    IL_0342:  ldloca.s   V_0
    IL_0344:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0349:  ldc.i4.2
    IL_034a:  rem
    IL_034b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0350:  starg.s    a
    IL_0352:  ldarg.0
    IL_0353:  stloc.0
    IL_0354:  ldloca.s   V_0
    IL_0356:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_035b:  brtrue.s   IL_0368

    IL_035d:  ldloca.s   V_2
    IL_035f:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0365:  ldloc.2
    IL_0366:  br.s       IL_0376

    IL_0368:  ldloca.s   V_0
    IL_036a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_036f:  ldc.i4.2
    IL_0370:  and
    IL_0371:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0376:  starg.s    a
    IL_0378:  ldarg.0
    IL_0379:  stloc.0
    IL_037a:  ldloca.s   V_0
    IL_037c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0381:  brtrue.s   IL_038e

    IL_0383:  ldloca.s   V_2
    IL_0385:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_038b:  ldloc.2
    IL_038c:  br.s       IL_039c

    IL_038e:  ldloca.s   V_0
    IL_0390:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0395:  ldc.i4.2
    IL_0396:  or
    IL_0397:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_039c:  starg.s    a
    IL_039e:  ldarg.0
    IL_039f:  stloc.0
    IL_03a0:  ldloca.s   V_0
    IL_03a2:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03a7:  brtrue.s   IL_03b4

    IL_03a9:  ldloca.s   V_2
    IL_03ab:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03b1:  ldloc.2
    IL_03b2:  br.s       IL_03c2

    IL_03b4:  ldloca.s   V_0
    IL_03b6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03bb:  ldc.i4.2
    IL_03bc:  xor
    IL_03bd:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03c2:  starg.s    a
    IL_03c4:  ldarg.0
    IL_03c5:  stloc.0
    IL_03c6:  ldloca.s   V_0
    IL_03c8:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03cd:  brtrue.s   IL_03da

    IL_03cf:  ldloca.s   V_2
    IL_03d1:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03d7:  ldloc.2
    IL_03d8:  br.s       IL_03e8

    IL_03da:  ldloca.s   V_0
    IL_03dc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03e1:  ldc.i4.2
    IL_03e2:  shl
    IL_03e3:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03e8:  starg.s    a
    IL_03ea:  ldarg.0
    IL_03eb:  stloc.0
    IL_03ec:  ldloca.s   V_0
    IL_03ee:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03f3:  brtrue.s   IL_0400

    IL_03f5:  ldloca.s   V_2
    IL_03f7:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03fd:  ldloc.2
    IL_03fe:  br.s       IL_040e

    IL_0400:  ldloca.s   V_0
    IL_0402:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0407:  ldc.i4.2
    IL_0408:  shr
    IL_0409:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_040e:  starg.s    a
    IL_0410:  ldarg.0
    IL_0411:  stloc.0
    IL_0412:  ldloca.s   V_0
    IL_0414:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0419:  brtrue.s   IL_0426

    IL_041b:  ldloca.s   V_2
    IL_041d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0423:  ldloc.2
    IL_0424:  br.s       IL_0434

    IL_0426:  ldc.i4.2
    IL_0427:  ldloca.s   V_0
    IL_0429:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_042e:  add
    IL_042f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0434:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0439:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_043e:  ret
  } // end of method LiftedOperators::IntValueConst

  .method public hidebysig static void  NumberBasic(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> a,
                                                    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> b) cil managed
  {
    // Code size       406 (0x196)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_0,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_000b:  ldloca.s   V_1
    IL_000d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0012:  call       bool [mscorlib]System.Decimal::op_Equality(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_0017:  brtrue.s   IL_001c

    IL_0019:  ldc.i4.0
    IL_001a:  br.s       IL_002c

    IL_001c:  ldloca.s   V_0
    IL_001e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0023:  ldloca.s   V_1
    IL_0025:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_002a:  ceq
    IL_002c:  brfalse.s  IL_0033

    IL_002e:  call       void [mscorlib]System.Console::WriteLine()
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
    IL_0062:  brfalse.s  IL_0069

    IL_0064:  call       void [mscorlib]System.Console::WriteLine()
    IL_0069:  ldarg.0
    IL_006a:  stloc.0
    IL_006b:  ldarg.1
    IL_006c:  stloc.1
    IL_006d:  ldloca.s   V_0
    IL_006f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0074:  ldloca.s   V_1
    IL_0076:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_007b:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_0080:  brtrue.s   IL_0085

    IL_0082:  ldc.i4.0
    IL_0083:  br.s       IL_0094

    IL_0085:  ldloca.s   V_0
    IL_0087:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_008c:  ldloca.s   V_1
    IL_008e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0093:  and
    IL_0094:  brfalse.s  IL_009b

    IL_0096:  call       void [mscorlib]System.Console::WriteLine()
    IL_009b:  ldarg.0
    IL_009c:  stloc.1
    IL_009d:  ldarg.1
    IL_009e:  stloc.0
    IL_009f:  ldloca.s   V_1
    IL_00a1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00a6:  ldloca.s   V_0
    IL_00a8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00ad:  call       bool [mscorlib]System.Decimal::op_LessThan(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_00b2:  brtrue.s   IL_00b7

    IL_00b4:  ldc.i4.0
    IL_00b5:  br.s       IL_00c6

    IL_00b7:  ldloca.s   V_1
    IL_00b9:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00be:  ldloca.s   V_0
    IL_00c0:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00c5:  and
    IL_00c6:  brfalse.s  IL_00cd

    IL_00c8:  call       void [mscorlib]System.Console::WriteLine()
    IL_00cd:  ldarg.0
    IL_00ce:  stloc.0
    IL_00cf:  ldarg.1
    IL_00d0:  stloc.1
    IL_00d1:  ldloca.s   V_0
    IL_00d3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00d8:  ldloca.s   V_1
    IL_00da:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00df:  call       bool [mscorlib]System.Decimal::op_GreaterThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                              valuetype [mscorlib]System.Decimal)
    IL_00e4:  brtrue.s   IL_00e9

    IL_00e6:  ldc.i4.0
    IL_00e7:  br.s       IL_00f8

    IL_00e9:  ldloca.s   V_0
    IL_00eb:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00f0:  ldloca.s   V_1
    IL_00f2:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00f7:  and
    IL_00f8:  brfalse.s  IL_00ff

    IL_00fa:  call       void [mscorlib]System.Console::WriteLine()
    IL_00ff:  ldarg.0
    IL_0100:  stloc.1
    IL_0101:  ldarg.1
    IL_0102:  stloc.0
    IL_0103:  ldloca.s   V_1
    IL_0105:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_010a:  ldloca.s   V_0
    IL_010c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0111:  call       bool [mscorlib]System.Decimal::op_LessThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                           valuetype [mscorlib]System.Decimal)
    IL_0116:  brtrue.s   IL_011b

    IL_0118:  ldc.i4.0
    IL_0119:  br.s       IL_012a

    IL_011b:  ldloca.s   V_1
    IL_011d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0122:  ldloca.s   V_0
    IL_0124:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0129:  and
    IL_012a:  brfalse.s  IL_0131

    IL_012c:  call       void [mscorlib]System.Console::WriteLine()
    IL_0131:  ldarg.0
    IL_0132:  stloc.0
    IL_0133:  ldarg.1
    IL_0134:  stloc.1
    IL_0135:  ldloca.s   V_0
    IL_0137:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_013c:  ldloca.s   V_1
    IL_013e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0143:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_0148:  brtrue.s   IL_014d

    IL_014a:  ldc.i4.0
    IL_014b:  br.s       IL_015c

    IL_014d:  ldloca.s   V_0
    IL_014f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0154:  ldloca.s   V_1
    IL_0156:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_015b:  and
    IL_015c:  brtrue.s   IL_0163

    IL_015e:  call       void [mscorlib]System.Console::WriteLine()
    IL_0163:  ldarg.0
    IL_0164:  stloc.1
    IL_0165:  ldarg.1
    IL_0166:  stloc.0
    IL_0167:  ldloca.s   V_1
    IL_0169:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_016e:  ldloca.s   V_0
    IL_0170:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0175:  call       bool [mscorlib]System.Decimal::op_LessThan(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_017a:  brtrue.s   IL_017f

    IL_017c:  ldc.i4.0
    IL_017d:  br.s       IL_018e

    IL_017f:  ldloca.s   V_1
    IL_0181:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0186:  ldloca.s   V_0
    IL_0188:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_018d:  and
    IL_018e:  brtrue.s   IL_0195

    IL_0190:  call       void [mscorlib]System.Console::WriteLine()
    IL_0195:  ret
  } // end of method LiftedOperators::NumberBasic

  .method public hidebysig static void  NumberComplex(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> a,
                                                      class [mscorlib]System.Func`1<valuetype [mscorlib]System.Decimal> x) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method LiftedOperators::NumberComplex

  .method public hidebysig static void  NumberConst(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> a) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method LiftedOperators::NumberConst

  .method public hidebysig static void  NumberValueBasic(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> a,
                                                         valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> b) cil managed
  {
    // Code size       946 (0x3b2)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_0,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_1,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_2)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_000b:  ldloca.s   V_1
    IL_000d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0012:  call       bool [mscorlib]System.Decimal::op_Equality(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_0017:  brtrue.s   IL_001c

    IL_0019:  ldc.i4.0
    IL_001a:  br.s       IL_002c

    IL_001c:  ldloca.s   V_0
    IL_001e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0023:  ldloca.s   V_1
    IL_0025:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_002a:  ceq
    IL_002c:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0031:  ldarg.0
    IL_0032:  stloc.1
    IL_0033:  ldarg.1
    IL_0034:  stloc.0
    IL_0035:  ldloca.s   V_1
    IL_0037:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_003c:  ldloca.s   V_0
    IL_003e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0043:  call       bool [mscorlib]System.Decimal::op_Equality(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_0048:  brtrue.s   IL_004d

    IL_004a:  ldc.i4.1
    IL_004b:  br.s       IL_0060

    IL_004d:  ldloca.s   V_1
    IL_004f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0054:  ldloca.s   V_0
    IL_0056:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_005b:  ceq
    IL_005d:  ldc.i4.0
    IL_005e:  ceq
    IL_0060:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0065:  ldarg.0
    IL_0066:  stloc.0
    IL_0067:  ldarg.1
    IL_0068:  stloc.1
    IL_0069:  ldloca.s   V_0
    IL_006b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0070:  ldloca.s   V_1
    IL_0072:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0077:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_007c:  brtrue.s   IL_0081

    IL_007e:  ldc.i4.0
    IL_007f:  br.s       IL_0090

    IL_0081:  ldloca.s   V_0
    IL_0083:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0088:  ldloca.s   V_1
    IL_008a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_008f:  and
    IL_0090:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0095:  ldarg.0
    IL_0096:  stloc.1
    IL_0097:  ldarg.1
    IL_0098:  stloc.0
    IL_0099:  ldloca.s   V_1
    IL_009b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00a0:  ldloca.s   V_0
    IL_00a2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00a7:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_00ac:  brtrue.s   IL_00b1

    IL_00ae:  ldc.i4.0
    IL_00af:  br.s       IL_00c0

    IL_00b1:  ldloca.s   V_1
    IL_00b3:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00b8:  ldloca.s   V_0
    IL_00ba:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00bf:  and
    IL_00c0:  ldc.i4.0
    IL_00c1:  ceq
    IL_00c3:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00c8:  ldarg.0
    IL_00c9:  stloc.0
    IL_00ca:  ldarg.1
    IL_00cb:  stloc.1
    IL_00cc:  ldloca.s   V_0
    IL_00ce:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00d3:  ldloca.s   V_1
    IL_00d5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00da:  call       bool [mscorlib]System.Decimal::op_LessThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                           valuetype [mscorlib]System.Decimal)
    IL_00df:  brtrue.s   IL_00e4

    IL_00e1:  ldc.i4.0
    IL_00e2:  br.s       IL_00f3

    IL_00e4:  ldloca.s   V_0
    IL_00e6:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00eb:  ldloca.s   V_1
    IL_00ed:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00f2:  and
    IL_00f3:  ldc.i4.0
    IL_00f4:  ceq
    IL_00f6:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00fb:  ldarg.0
    IL_00fc:  stloc.1
    IL_00fd:  ldarg.1
    IL_00fe:  stloc.0
    IL_00ff:  ldloca.s   V_1
    IL_0101:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0106:  ldloca.s   V_0
    IL_0108:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_010d:  and
    IL_010e:  brtrue.s   IL_011b

    IL_0110:  ldloca.s   V_2
    IL_0112:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0118:  ldloc.2
    IL_0119:  br.s       IL_0133

    IL_011b:  ldloca.s   V_1
    IL_011d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0122:  ldloca.s   V_0
    IL_0124:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0129:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_012e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0133:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0138:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_013d:  ldarg.0
    IL_013e:  stloc.0
    IL_013f:  ldarg.1
    IL_0140:  stloc.1
    IL_0141:  ldloca.s   V_0
    IL_0143:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0148:  ldloca.s   V_1
    IL_014a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_014f:  and
    IL_0150:  brtrue.s   IL_015d

    IL_0152:  ldloca.s   V_2
    IL_0154:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_015a:  ldloc.2
    IL_015b:  br.s       IL_0175

    IL_015d:  ldloca.s   V_0
    IL_015f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0164:  ldloca.s   V_1
    IL_0166:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_016b:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Subtraction(valuetype [mscorlib]System.Decimal,
                                                                                                     valuetype [mscorlib]System.Decimal)
    IL_0170:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0175:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_017a:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_017f:  ldarg.0
    IL_0180:  stloc.1
    IL_0181:  ldarg.1
    IL_0182:  stloc.0
    IL_0183:  ldloca.s   V_1
    IL_0185:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_018a:  ldloca.s   V_0
    IL_018c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0191:  and
    IL_0192:  brtrue.s   IL_019f

    IL_0194:  ldloca.s   V_2
    IL_0196:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_019c:  ldloc.2
    IL_019d:  br.s       IL_01b7

    IL_019f:  ldloca.s   V_1
    IL_01a1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01a6:  ldloca.s   V_0
    IL_01a8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01ad:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Multiply(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_01b2:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_01b7:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_01bc:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01c1:  ldarg.0
    IL_01c2:  stloc.0
    IL_01c3:  ldarg.1
    IL_01c4:  stloc.1
    IL_01c5:  ldloca.s   V_0
    IL_01c7:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_01cc:  ldloca.s   V_1
    IL_01ce:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_01d3:  and
    IL_01d4:  brtrue.s   IL_01e1

    IL_01d6:  ldloca.s   V_2
    IL_01d8:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_01de:  ldloc.2
    IL_01df:  br.s       IL_01f9

    IL_01e1:  ldloca.s   V_0
    IL_01e3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01e8:  ldloca.s   V_1
    IL_01ea:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01ef:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Division(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_01f4:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_01f9:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_01fe:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0203:  ldarg.0
    IL_0204:  stloc.1
    IL_0205:  ldarg.1
    IL_0206:  stloc.0
    IL_0207:  ldloca.s   V_1
    IL_0209:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_020e:  ldloca.s   V_0
    IL_0210:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0215:  and
    IL_0216:  brtrue.s   IL_0223

    IL_0218:  ldloca.s   V_2
    IL_021a:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0220:  ldloc.2
    IL_0221:  br.s       IL_023b

    IL_0223:  ldloca.s   V_1
    IL_0225:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_022a:  ldloca.s   V_0
    IL_022c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0231:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Modulus(valuetype [mscorlib]System.Decimal,
                                                                                                 valuetype [mscorlib]System.Decimal)
    IL_0236:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_023b:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0240:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0245:  ldarg.0
    IL_0246:  stloc.0
    IL_0247:  ldloca.s   V_0
    IL_0249:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_024e:  brtrue.s   IL_0253

    IL_0250:  ldarg.1
    IL_0251:  br.s       IL_0254

    IL_0253:  ldloc.0
    IL_0254:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0259:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_025e:  ldarg.0
    IL_025f:  stloc.0
    IL_0260:  ldloca.s   V_0
    IL_0262:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0267:  brtrue.s   IL_0274

    IL_0269:  ldloca.s   V_1
    IL_026b:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0271:  ldloc.1
    IL_0272:  br.s       IL_0285

    IL_0274:  ldloca.s   V_0
    IL_0276:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_027b:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_UnaryNegation(valuetype [mscorlib]System.Decimal)
    IL_0280:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0285:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_028a:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_028f:  ldarg.0
    IL_0290:  stloc.0
    IL_0291:  ldarg.1
    IL_0292:  stloc.1
    IL_0293:  ldloca.s   V_0
    IL_0295:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_029a:  ldloca.s   V_1
    IL_029c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_02a1:  and
    IL_02a2:  brtrue.s   IL_02af

    IL_02a4:  ldloca.s   V_2
    IL_02a6:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_02ac:  ldloc.2
    IL_02ad:  br.s       IL_02c7

    IL_02af:  ldloca.s   V_0
    IL_02b1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_02b6:  ldloca.s   V_1
    IL_02b8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_02bd:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_02c2:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_02c7:  starg.s    a
    IL_02c9:  ldarg.0
    IL_02ca:  stloc.1
    IL_02cb:  ldarg.1
    IL_02cc:  stloc.0
    IL_02cd:  ldloca.s   V_1
    IL_02cf:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_02d4:  ldloca.s   V_0
    IL_02d6:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_02db:  and
    IL_02dc:  brtrue.s   IL_02e9

    IL_02de:  ldloca.s   V_2
    IL_02e0:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_02e6:  ldloc.2
    IL_02e7:  br.s       IL_0301

    IL_02e9:  ldloca.s   V_1
    IL_02eb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_02f0:  ldloca.s   V_0
    IL_02f2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_02f7:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Subtraction(valuetype [mscorlib]System.Decimal,
                                                                                                     valuetype [mscorlib]System.Decimal)
    IL_02fc:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0301:  starg.s    a
    IL_0303:  ldarg.0
    IL_0304:  stloc.0
    IL_0305:  ldarg.1
    IL_0306:  stloc.1
    IL_0307:  ldloca.s   V_0
    IL_0309:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_030e:  ldloca.s   V_1
    IL_0310:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0315:  and
    IL_0316:  brtrue.s   IL_0323

    IL_0318:  ldloca.s   V_2
    IL_031a:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0320:  ldloc.2
    IL_0321:  br.s       IL_033b

    IL_0323:  ldloca.s   V_0
    IL_0325:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_032a:  ldloca.s   V_1
    IL_032c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0331:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Multiply(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_0336:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_033b:  starg.s    a
    IL_033d:  ldarg.0
    IL_033e:  stloc.1
    IL_033f:  ldarg.1
    IL_0340:  stloc.0
    IL_0341:  ldloca.s   V_1
    IL_0343:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0348:  ldloca.s   V_0
    IL_034a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_034f:  and
    IL_0350:  brtrue.s   IL_035d

    IL_0352:  ldloca.s   V_2
    IL_0354:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_035a:  ldloc.2
    IL_035b:  br.s       IL_0375

    IL_035d:  ldloca.s   V_1
    IL_035f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0364:  ldloca.s   V_0
    IL_0366:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_036b:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Division(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_0370:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0375:  starg.s    a
    IL_0377:  ldarg.0
    IL_0378:  stloc.0
    IL_0379:  ldarg.1
    IL_037a:  stloc.1
    IL_037b:  ldloca.s   V_0
    IL_037d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0382:  ldloca.s   V_1
    IL_0384:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0389:  and
    IL_038a:  brtrue.s   IL_0397

    IL_038c:  ldloca.s   V_2
    IL_038e:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0394:  ldloc.2
    IL_0395:  br.s       IL_03af

    IL_0397:  ldloca.s   V_0
    IL_0399:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_039e:  ldloca.s   V_1
    IL_03a0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_03a5:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Modulus(valuetype [mscorlib]System.Decimal,
                                                                                                 valuetype [mscorlib]System.Decimal)
    IL_03aa:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_03af:  starg.s    a
    IL_03b1:  ret
  } // end of method LiftedOperators::NumberValueBasic

  .method public hidebysig static void  NumberValueComplex(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> a,
                                                           class [mscorlib]System.Func`1<valuetype [mscorlib]System.Decimal> x) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method LiftedOperators::NumberValueComplex

  .method public hidebysig static void  NumberValueConst(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> a) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method LiftedOperators::NumberValueConst

  .method public hidebysig static void  CompareWithImplictCast(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                               valuetype [mscorlib]System.Nullable`1<int64> b) cil managed
  {
    // Code size       160 (0xa0)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int64> V_0,
             valuetype [mscorlib]System.Nullable`1<int64> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             valuetype [mscorlib]System.Nullable`1<int64> V_3)
    IL_0000:  ldarg.0
    IL_0001:  stloc.2
    IL_0002:  ldloca.s   V_2
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brtrue.s   IL_0016

    IL_000b:  ldloca.s   V_3
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0013:  ldloc.3
    IL_0014:  br.s       IL_0023

    IL_0016:  ldloca.s   V_2
    IL_0018:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001d:  conv.i8
    IL_001e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0023:  stloc.0
    IL_0024:  ldarg.1
    IL_0025:  stloc.1
    IL_0026:  ldloca.s   V_0
    IL_0028:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_002d:  ldloca.s   V_1
    IL_002f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0034:  blt.s      IL_0039

    IL_0036:  ldc.i4.0
    IL_0037:  br.s       IL_0048

    IL_0039:  ldloca.s   V_0
    IL_003b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0040:  ldloca.s   V_1
    IL_0042:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0047:  and
    IL_0048:  brfalse.s  IL_004f

    IL_004a:  call       void [mscorlib]System.Console::WriteLine()
    IL_004f:  ldarg.0
    IL_0050:  stloc.2
    IL_0051:  ldloca.s   V_2
    IL_0053:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0058:  brtrue.s   IL_0065

    IL_005a:  ldloca.s   V_3
    IL_005c:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0062:  ldloc.3
    IL_0063:  br.s       IL_0072

    IL_0065:  ldloca.s   V_2
    IL_0067:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_006c:  conv.i8
    IL_006d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0072:  stloc.1
    IL_0073:  ldarg.1
    IL_0074:  stloc.0
    IL_0075:  ldloca.s   V_1
    IL_0077:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_007c:  ldloca.s   V_0
    IL_007e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0083:  beq.s      IL_0088

    IL_0085:  ldc.i4.0
    IL_0086:  br.s       IL_0098

    IL_0088:  ldloca.s   V_1
    IL_008a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_008f:  ldloca.s   V_0
    IL_0091:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0096:  ceq
    IL_0098:  brfalse.s  IL_009f

    IL_009a:  call       void [mscorlib]System.Console::WriteLine()
    IL_009f:  ret
  } // end of method LiftedOperators::CompareWithImplictCast

  .method public hidebysig static void  CompareWithSignChange(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                              valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       112 (0x70)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<uint32> V_0,
             valuetype [mscorlib]System.Nullable`1<uint32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             valuetype [mscorlib]System.Nullable`1<uint32> V_3)
    IL_0000:  ldarg.0
    IL_0001:  stloc.2
    IL_0002:  ldloca.s   V_2
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brtrue.s   IL_0016

    IL_000b:  ldloca.s   V_3
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_0013:  ldloc.3
    IL_0014:  br.s       IL_0022

    IL_0016:  ldloca.s   V_2
    IL_0018:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_0022:  stloc.0
    IL_0023:  ldarg.1
    IL_0024:  stloc.2
    IL_0025:  ldloca.s   V_2
    IL_0027:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_002c:  brtrue.s   IL_0039

    IL_002e:  ldloca.s   V_3
    IL_0030:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_0036:  ldloc.3
    IL_0037:  br.s       IL_0045

    IL_0039:  ldloca.s   V_2
    IL_003b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0040:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_0045:  stloc.1
    IL_0046:  ldloca.s   V_0
    IL_0048:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint32>::GetValueOrDefault()
    IL_004d:  ldloca.s   V_1
    IL_004f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint32>::GetValueOrDefault()
    IL_0054:  blt.un.s   IL_0059

    IL_0056:  ldc.i4.0
    IL_0057:  br.s       IL_0068

    IL_0059:  ldloca.s   V_0
    IL_005b:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint32>::get_HasValue()
    IL_0060:  ldloca.s   V_1
    IL_0062:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint32>::get_HasValue()
    IL_0067:  and
    IL_0068:  brfalse.s  IL_006f

    IL_006a:  call       void [mscorlib]System.Console::WriteLine()
    IL_006f:  ret
  } // end of method LiftedOperators::CompareWithSignChange

  .method public hidebysig static void  StructBasic(valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> a,
                                                    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> b) cil managed
  {
    // Code size       495 (0x1ef)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_0,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_000b:  ldloca.s   V_1
    IL_000d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0012:  beq.s      IL_0017

    IL_0014:  ldc.i4.0
    IL_0015:  br.s       IL_0036

    IL_0017:  ldloca.s   V_0
    IL_0019:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_001e:  brtrue.s   IL_0023

    IL_0020:  ldc.i4.1
    IL_0021:  br.s       IL_0036

    IL_0023:  ldloca.s   V_0
    IL_0025:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_002a:  ldloca.s   V_1
    IL_002c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0031:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Equality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0036:  brfalse.s  IL_003d

    IL_0038:  call       void [mscorlib]System.Console::WriteLine()
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
    IL_0073:  brfalse.s  IL_007a

    IL_0075:  call       void [mscorlib]System.Console::WriteLine()
    IL_007a:  ldarg.0
    IL_007b:  stloc.0
    IL_007c:  ldarg.1
    IL_007d:  stloc.1
    IL_007e:  ldloca.s   V_0
    IL_0080:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0085:  ldloca.s   V_1
    IL_0087:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_008c:  and
    IL_008d:  brtrue.s   IL_0092

    IL_008f:  ldc.i4.0
    IL_0090:  br.s       IL_00a5

    IL_0092:  ldloca.s   V_0
    IL_0094:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0099:  ldloca.s   V_1
    IL_009b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00a0:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_GreaterThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_00a5:  brfalse.s  IL_00ac

    IL_00a7:  call       void [mscorlib]System.Console::WriteLine()
    IL_00ac:  ldarg.0
    IL_00ad:  stloc.1
    IL_00ae:  ldarg.1
    IL_00af:  stloc.0
    IL_00b0:  ldloca.s   V_1
    IL_00b2:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00b7:  ldloca.s   V_0
    IL_00b9:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00be:  and
    IL_00bf:  brtrue.s   IL_00c4

    IL_00c1:  ldc.i4.0
    IL_00c2:  br.s       IL_00d7

    IL_00c4:  ldloca.s   V_1
    IL_00c6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00cb:  ldloca.s   V_0
    IL_00cd:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00d2:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_LessThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_00d7:  brfalse.s  IL_00de

    IL_00d9:  call       void [mscorlib]System.Console::WriteLine()
    IL_00de:  ldarg.0
    IL_00df:  stloc.0
    IL_00e0:  ldarg.1
    IL_00e1:  stloc.1
    IL_00e2:  ldloca.s   V_0
    IL_00e4:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00e9:  ldloca.s   V_1
    IL_00eb:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00f0:  and
    IL_00f1:  brtrue.s   IL_00f6

    IL_00f3:  ldc.i4.0
    IL_00f4:  br.s       IL_0109

    IL_00f6:  ldloca.s   V_0
    IL_00f8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00fd:  ldloca.s   V_1
    IL_00ff:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0104:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_GreaterThanOrEqual(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0109:  brfalse.s  IL_0110

    IL_010b:  call       void [mscorlib]System.Console::WriteLine()
    IL_0110:  ldarg.0
    IL_0111:  stloc.1
    IL_0112:  ldarg.1
    IL_0113:  stloc.0
    IL_0114:  ldloca.s   V_1
    IL_0116:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_011b:  ldloca.s   V_0
    IL_011d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0122:  and
    IL_0123:  brtrue.s   IL_0128

    IL_0125:  ldc.i4.0
    IL_0126:  br.s       IL_013b

    IL_0128:  ldloca.s   V_1
    IL_012a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_012f:  ldloca.s   V_0
    IL_0131:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0136:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_LessThanOrEqual(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                   valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_013b:  brfalse.s  IL_0142

    IL_013d:  call       void [mscorlib]System.Console::WriteLine()
    IL_0142:  ldarg.0
    IL_0143:  stloc.0
    IL_0144:  ldarg.1
    IL_0145:  stloc.1
    IL_0146:  ldloca.s   V_0
    IL_0148:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_014d:  ldloca.s   V_1
    IL_014f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0154:  beq.s      IL_0159

    IL_0156:  ldc.i4.0
    IL_0157:  br.s       IL_0178

    IL_0159:  ldloca.s   V_0
    IL_015b:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0160:  brtrue.s   IL_0165

    IL_0162:  ldc.i4.1
    IL_0163:  br.s       IL_0178

    IL_0165:  ldloca.s   V_0
    IL_0167:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_016c:  ldloca.s   V_1
    IL_016e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0173:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Equality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0178:  brtrue.s   IL_017f

    IL_017a:  call       void [mscorlib]System.Console::WriteLine()
    IL_017f:  ldarg.0
    IL_0180:  stloc.1
    IL_0181:  ldarg.1
    IL_0182:  stloc.0
    IL_0183:  ldloca.s   V_1
    IL_0185:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_018a:  ldloca.s   V_0
    IL_018c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0191:  beq.s      IL_0196

    IL_0193:  ldc.i4.1
    IL_0194:  br.s       IL_01b5

    IL_0196:  ldloca.s   V_1
    IL_0198:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_019d:  brtrue.s   IL_01a2

    IL_019f:  ldc.i4.0
    IL_01a0:  br.s       IL_01b5

    IL_01a2:  ldloca.s   V_1
    IL_01a4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_01a9:  ldloca.s   V_0
    IL_01ab:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_01b0:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Inequality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_01b5:  brtrue.s   IL_01bc

    IL_01b7:  call       void [mscorlib]System.Console::WriteLine()
    IL_01bc:  ldarg.0
    IL_01bd:  stloc.0
    IL_01be:  ldarg.1
    IL_01bf:  stloc.1
    IL_01c0:  ldloca.s   V_0
    IL_01c2:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01c7:  ldloca.s   V_1
    IL_01c9:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01ce:  and
    IL_01cf:  brtrue.s   IL_01d4

    IL_01d1:  ldc.i4.0
    IL_01d2:  br.s       IL_01e7

    IL_01d4:  ldloca.s   V_0
    IL_01d6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_01db:  ldloca.s   V_1
    IL_01dd:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_01e2:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_GreaterThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_01e7:  brtrue.s   IL_01ee

    IL_01e9:  call       void [mscorlib]System.Console::WriteLine()
    IL_01ee:  ret
  } // end of method LiftedOperators::StructBasic

  .method public hidebysig static void  StructComplex(valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> a,
                                                      class [mscorlib]System.Func`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> x) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method LiftedOperators::StructComplex

  .method public hidebysig static void  StructValueBasic(valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> a,
                                                         valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> b,
                                                         valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       1803 (0x70b)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_0,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_1,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_2,
             valuetype [mscorlib]System.Nullable`1<int32> V_3)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_000b:  ldloca.s   V_1
    IL_000d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0012:  beq.s      IL_0017

    IL_0014:  ldc.i4.0
    IL_0015:  br.s       IL_0036

    IL_0017:  ldloca.s   V_0
    IL_0019:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_001e:  brtrue.s   IL_0023

    IL_0020:  ldc.i4.1
    IL_0021:  br.s       IL_0036

    IL_0023:  ldloca.s   V_0
    IL_0025:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_002a:  ldloca.s   V_1
    IL_002c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0031:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Equality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0036:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_003b:  ldarg.0
    IL_003c:  stloc.1
    IL_003d:  ldarg.1
    IL_003e:  stloc.0
    IL_003f:  ldloca.s   V_1
    IL_0041:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0046:  ldloca.s   V_0
    IL_0048:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_004d:  beq.s      IL_0052

    IL_004f:  ldc.i4.1
    IL_0050:  br.s       IL_0071

    IL_0052:  ldloca.s   V_1
    IL_0054:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0059:  brtrue.s   IL_005e

    IL_005b:  ldc.i4.0
    IL_005c:  br.s       IL_0071

    IL_005e:  ldloca.s   V_1
    IL_0060:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0065:  ldloca.s   V_0
    IL_0067:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_006c:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Inequality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0071:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0076:  ldarg.0
    IL_0077:  stloc.0
    IL_0078:  ldarg.1
    IL_0079:  stloc.1
    IL_007a:  ldloca.s   V_0
    IL_007c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0081:  ldloca.s   V_1
    IL_0083:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0088:  and
    IL_0089:  brtrue.s   IL_008e

    IL_008b:  ldc.i4.0
    IL_008c:  br.s       IL_00a1

    IL_008e:  ldloca.s   V_0
    IL_0090:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0095:  ldloca.s   V_1
    IL_0097:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_009c:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_GreaterThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_00a1:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00a6:  ldarg.0
    IL_00a7:  stloc.1
    IL_00a8:  ldarg.1
    IL_00a9:  stloc.0
    IL_00aa:  ldloca.s   V_1
    IL_00ac:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00b1:  ldloca.s   V_0
    IL_00b3:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00b8:  beq.s      IL_00bd

    IL_00ba:  ldc.i4.0
    IL_00bb:  br.s       IL_00dc

    IL_00bd:  ldloca.s   V_1
    IL_00bf:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00c4:  brtrue.s   IL_00c9

    IL_00c6:  ldc.i4.1
    IL_00c7:  br.s       IL_00dc

    IL_00c9:  ldloca.s   V_1
    IL_00cb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00d0:  ldloca.s   V_0
    IL_00d2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00d7:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Equality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_00dc:  ldc.i4.0
    IL_00dd:  ceq
    IL_00df:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00e4:  ldarg.0
    IL_00e5:  stloc.0
    IL_00e6:  ldarg.1
    IL_00e7:  stloc.1
    IL_00e8:  ldloca.s   V_0
    IL_00ea:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00ef:  ldloca.s   V_1
    IL_00f1:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00f6:  beq.s      IL_00fb

    IL_00f8:  ldc.i4.1
    IL_00f9:  br.s       IL_011a

    IL_00fb:  ldloca.s   V_0
    IL_00fd:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0102:  brtrue.s   IL_0107

    IL_0104:  ldc.i4.0
    IL_0105:  br.s       IL_011a

    IL_0107:  ldloca.s   V_0
    IL_0109:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_010e:  ldloca.s   V_1
    IL_0110:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0115:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Inequality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_011a:  ldc.i4.0
    IL_011b:  ceq
    IL_011d:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0122:  ldarg.0
    IL_0123:  stloc.1
    IL_0124:  ldarg.1
    IL_0125:  stloc.0
    IL_0126:  ldloca.s   V_1
    IL_0128:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_012d:  ldloca.s   V_0
    IL_012f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0134:  and
    IL_0135:  brtrue.s   IL_013a

    IL_0137:  ldc.i4.0
    IL_0138:  br.s       IL_014d

    IL_013a:  ldloca.s   V_1
    IL_013c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0141:  ldloca.s   V_0
    IL_0143:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0148:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_GreaterThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_014d:  ldc.i4.0
    IL_014e:  ceq
    IL_0150:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0155:  ldarg.0
    IL_0156:  stloc.0
    IL_0157:  ldarg.1
    IL_0158:  stloc.1
    IL_0159:  ldloca.s   V_0
    IL_015b:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0160:  ldloca.s   V_1
    IL_0162:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0167:  and
    IL_0168:  brtrue.s   IL_0175

    IL_016a:  ldloca.s   V_2
    IL_016c:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0172:  ldloc.2
    IL_0173:  br.s       IL_018d

    IL_0175:  ldloca.s   V_0
    IL_0177:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_017c:  ldloca.s   V_1
    IL_017e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0183:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Addition(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0188:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_018d:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0192:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0197:  ldarg.0
    IL_0198:  stloc.1
    IL_0199:  ldarg.1
    IL_019a:  stloc.0
    IL_019b:  ldloca.s   V_1
    IL_019d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01a2:  ldloca.s   V_0
    IL_01a4:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01a9:  and
    IL_01aa:  brtrue.s   IL_01b7

    IL_01ac:  ldloca.s   V_2
    IL_01ae:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_01b4:  ldloc.2
    IL_01b5:  br.s       IL_01cf

    IL_01b7:  ldloca.s   V_1
    IL_01b9:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_01be:  ldloca.s   V_0
    IL_01c0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_01c5:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Subtraction(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_01ca:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_01cf:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_01d4:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01d9:  ldarg.0
    IL_01da:  stloc.0
    IL_01db:  ldarg.1
    IL_01dc:  stloc.1
    IL_01dd:  ldloca.s   V_0
    IL_01df:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01e4:  ldloca.s   V_1
    IL_01e6:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01eb:  and
    IL_01ec:  brtrue.s   IL_01f9

    IL_01ee:  ldloca.s   V_2
    IL_01f0:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_01f6:  ldloc.2
    IL_01f7:  br.s       IL_0211

    IL_01f9:  ldloca.s   V_0
    IL_01fb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0200:  ldloca.s   V_1
    IL_0202:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0207:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Multiply(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_020c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0211:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0216:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_021b:  ldarg.0
    IL_021c:  stloc.1
    IL_021d:  ldarg.1
    IL_021e:  stloc.0
    IL_021f:  ldloca.s   V_1
    IL_0221:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0226:  ldloca.s   V_0
    IL_0228:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_022d:  and
    IL_022e:  brtrue.s   IL_023b

    IL_0230:  ldloca.s   V_2
    IL_0232:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0238:  ldloc.2
    IL_0239:  br.s       IL_0253

    IL_023b:  ldloca.s   V_1
    IL_023d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0242:  ldloca.s   V_0
    IL_0244:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0249:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Division(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_024e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0253:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0258:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_025d:  ldarg.0
    IL_025e:  stloc.0
    IL_025f:  ldarg.1
    IL_0260:  stloc.1
    IL_0261:  ldloca.s   V_0
    IL_0263:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0268:  ldloca.s   V_1
    IL_026a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_026f:  and
    IL_0270:  brtrue.s   IL_027d

    IL_0272:  ldloca.s   V_2
    IL_0274:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_027a:  ldloc.2
    IL_027b:  br.s       IL_0295

    IL_027d:  ldloca.s   V_0
    IL_027f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0284:  ldloca.s   V_1
    IL_0286:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_028b:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Modulus(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                 valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0290:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0295:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_029a:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_029f:  ldarg.0
    IL_02a0:  stloc.1
    IL_02a1:  ldarg.1
    IL_02a2:  stloc.0
    IL_02a3:  ldloca.s   V_1
    IL_02a5:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_02aa:  ldloca.s   V_0
    IL_02ac:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_02b1:  and
    IL_02b2:  brtrue.s   IL_02bf

    IL_02b4:  ldloca.s   V_2
    IL_02b6:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_02bc:  ldloc.2
    IL_02bd:  br.s       IL_02d7

    IL_02bf:  ldloca.s   V_1
    IL_02c1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_02c6:  ldloca.s   V_0
    IL_02c8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_02cd:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                    valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_02d2:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_02d7:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_02dc:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02e1:  ldarg.0
    IL_02e2:  stloc.0
    IL_02e3:  ldarg.1
    IL_02e4:  stloc.1
    IL_02e5:  ldloca.s   V_0
    IL_02e7:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_02ec:  ldloca.s   V_1
    IL_02ee:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_02f3:  and
    IL_02f4:  brtrue.s   IL_0301

    IL_02f6:  ldloca.s   V_2
    IL_02f8:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_02fe:  ldloc.2
    IL_02ff:  br.s       IL_0319

    IL_0301:  ldloca.s   V_0
    IL_0303:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0308:  ldloca.s   V_1
    IL_030a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_030f:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                   valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0314:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0319:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_031e:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0323:  ldarg.0
    IL_0324:  stloc.1
    IL_0325:  ldarg.1
    IL_0326:  stloc.0
    IL_0327:  ldloca.s   V_1
    IL_0329:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_032e:  ldloca.s   V_0
    IL_0330:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0335:  and
    IL_0336:  brtrue.s   IL_0343

    IL_0338:  ldloca.s   V_2
    IL_033a:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0340:  ldloc.2
    IL_0341:  br.s       IL_035b

    IL_0343:  ldloca.s   V_1
    IL_0345:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_034a:  ldloca.s   V_0
    IL_034c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0351:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_ExclusiveOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0356:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_035b:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0360:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0365:  ldarg.0
    IL_0366:  stloc.0
    IL_0367:  ldarg.2
    IL_0368:  stloc.3
    IL_0369:  ldloca.s   V_0
    IL_036b:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0370:  ldloca.s   V_3
    IL_0372:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0377:  and
    IL_0378:  brtrue.s   IL_0385

    IL_037a:  ldloca.s   V_1
    IL_037c:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0382:  ldloc.1
    IL_0383:  br.s       IL_039d

    IL_0385:  ldloca.s   V_0
    IL_0387:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_038c:  ldloca.s   V_3
    IL_038e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0393:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_LeftShift(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                   int32)
    IL_0398:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_039d:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_03a2:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_03a7:  ldarg.0
    IL_03a8:  stloc.0
    IL_03a9:  ldarg.2
    IL_03aa:  stloc.3
    IL_03ab:  ldloca.s   V_0
    IL_03ad:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_03b2:  ldloca.s   V_3
    IL_03b4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03b9:  and
    IL_03ba:  brtrue.s   IL_03c7

    IL_03bc:  ldloca.s   V_1
    IL_03be:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_03c4:  ldloc.1
    IL_03c5:  br.s       IL_03df

    IL_03c7:  ldloca.s   V_0
    IL_03c9:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_03ce:  ldloca.s   V_3
    IL_03d0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03d5:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_RightShift(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                    int32)
    IL_03da:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_03df:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_03e4:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_03e9:  ldarg.0
    IL_03ea:  stloc.0
    IL_03eb:  ldloca.s   V_0
    IL_03ed:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_03f2:  brtrue.s   IL_03f7

    IL_03f4:  ldarg.1
    IL_03f5:  br.s       IL_03f8

    IL_03f7:  ldloc.0
    IL_03f8:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_03fd:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0402:  ldarg.0
    IL_0403:  stloc.0
    IL_0404:  ldloca.s   V_0
    IL_0406:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_040b:  brtrue.s   IL_0418

    IL_040d:  ldloca.s   V_1
    IL_040f:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0415:  ldloc.1
    IL_0416:  br.s       IL_0429

    IL_0418:  ldloca.s   V_0
    IL_041a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_041f:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_UnaryPlus(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0424:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0429:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_042e:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0433:  ldarg.0
    IL_0434:  stloc.0
    IL_0435:  ldloca.s   V_0
    IL_0437:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_043c:  brtrue.s   IL_0449

    IL_043e:  ldloca.s   V_1
    IL_0440:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0446:  ldloc.1
    IL_0447:  br.s       IL_045a

    IL_0449:  ldloca.s   V_0
    IL_044b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0450:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_UnaryNegation(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0455:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_045a:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_045f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0464:  ldarg.0
    IL_0465:  stloc.0
    IL_0466:  ldloca.s   V_0
    IL_0468:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_046d:  brtrue.s   IL_047a

    IL_046f:  ldloca.s   V_1
    IL_0471:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0477:  ldloc.1
    IL_0478:  br.s       IL_048b

    IL_047a:  ldloca.s   V_0
    IL_047c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0481:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_LogicalNot(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0486:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_048b:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0490:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0495:  ldarg.0
    IL_0496:  stloc.0
    IL_0497:  ldloca.s   V_0
    IL_0499:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_049e:  brtrue.s   IL_04ab

    IL_04a0:  ldloca.s   V_1
    IL_04a2:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_04a8:  ldloc.1
    IL_04a9:  br.s       IL_04bc

    IL_04ab:  ldloca.s   V_0
    IL_04ad:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_04b2:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_OnesComplement(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_04b7:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_04bc:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_04c1:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_04c6:  ldarg.0
    IL_04c7:  stloc.0
    IL_04c8:  ldarg.1
    IL_04c9:  stloc.1
    IL_04ca:  ldloca.s   V_0
    IL_04cc:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_04d1:  ldloca.s   V_1
    IL_04d3:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_04d8:  and
    IL_04d9:  brtrue.s   IL_04e6

    IL_04db:  ldloca.s   V_2
    IL_04dd:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_04e3:  ldloc.2
    IL_04e4:  br.s       IL_04fe

    IL_04e6:  ldloca.s   V_0
    IL_04e8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_04ed:  ldloca.s   V_1
    IL_04ef:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_04f4:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Addition(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_04f9:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_04fe:  starg.s    a
    IL_0500:  ldarg.0
    IL_0501:  stloc.1
    IL_0502:  ldarg.1
    IL_0503:  stloc.0
    IL_0504:  ldloca.s   V_1
    IL_0506:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_050b:  ldloca.s   V_0
    IL_050d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0512:  and
    IL_0513:  brtrue.s   IL_0520

    IL_0515:  ldloca.s   V_2
    IL_0517:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_051d:  ldloc.2
    IL_051e:  br.s       IL_0538

    IL_0520:  ldloca.s   V_1
    IL_0522:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0527:  ldloca.s   V_0
    IL_0529:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_052e:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Subtraction(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0533:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0538:  starg.s    a
    IL_053a:  ldarg.0
    IL_053b:  stloc.0
    IL_053c:  ldarg.1
    IL_053d:  stloc.1
    IL_053e:  ldloca.s   V_0
    IL_0540:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0545:  ldloca.s   V_1
    IL_0547:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_054c:  and
    IL_054d:  brtrue.s   IL_055a

    IL_054f:  ldloca.s   V_2
    IL_0551:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0557:  ldloc.2
    IL_0558:  br.s       IL_0572

    IL_055a:  ldloca.s   V_0
    IL_055c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0561:  ldloca.s   V_1
    IL_0563:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0568:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Multiply(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_056d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0572:  starg.s    a
    IL_0574:  ldarg.0
    IL_0575:  stloc.1
    IL_0576:  ldarg.1
    IL_0577:  stloc.0
    IL_0578:  ldloca.s   V_1
    IL_057a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_057f:  ldloca.s   V_0
    IL_0581:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0586:  and
    IL_0587:  brtrue.s   IL_0594

    IL_0589:  ldloca.s   V_2
    IL_058b:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0591:  ldloc.2
    IL_0592:  br.s       IL_05ac

    IL_0594:  ldloca.s   V_1
    IL_0596:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_059b:  ldloca.s   V_0
    IL_059d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_05a2:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Division(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_05a7:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_05ac:  starg.s    a
    IL_05ae:  ldarg.0
    IL_05af:  stloc.0
    IL_05b0:  ldarg.1
    IL_05b1:  stloc.1
    IL_05b2:  ldloca.s   V_0
    IL_05b4:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_05b9:  ldloca.s   V_1
    IL_05bb:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_05c0:  and
    IL_05c1:  brtrue.s   IL_05ce

    IL_05c3:  ldloca.s   V_2
    IL_05c5:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_05cb:  ldloc.2
    IL_05cc:  br.s       IL_05e6

    IL_05ce:  ldloca.s   V_0
    IL_05d0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_05d5:  ldloca.s   V_1
    IL_05d7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_05dc:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Modulus(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                 valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_05e1:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_05e6:  starg.s    a
    IL_05e8:  ldarg.0
    IL_05e9:  stloc.1
    IL_05ea:  ldarg.1
    IL_05eb:  stloc.0
    IL_05ec:  ldloca.s   V_1
    IL_05ee:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_05f3:  ldloca.s   V_0
    IL_05f5:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_05fa:  and
    IL_05fb:  brtrue.s   IL_0608

    IL_05fd:  ldloca.s   V_2
    IL_05ff:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0605:  ldloc.2
    IL_0606:  br.s       IL_0620

    IL_0608:  ldloca.s   V_1
    IL_060a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_060f:  ldloca.s   V_0
    IL_0611:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0616:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                    valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_061b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0620:  starg.s    a
    IL_0622:  ldarg.0
    IL_0623:  stloc.0
    IL_0624:  ldarg.1
    IL_0625:  stloc.1
    IL_0626:  ldloca.s   V_0
    IL_0628:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_062d:  ldloca.s   V_1
    IL_062f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0634:  and
    IL_0635:  brtrue.s   IL_0642

    IL_0637:  ldloca.s   V_2
    IL_0639:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_063f:  ldloc.2
    IL_0640:  br.s       IL_065a

    IL_0642:  ldloca.s   V_0
    IL_0644:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0649:  ldloca.s   V_1
    IL_064b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0650:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                   valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0655:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_065a:  starg.s    a
    IL_065c:  ldarg.0
    IL_065d:  stloc.1
    IL_065e:  ldarg.1
    IL_065f:  stloc.0
    IL_0660:  ldloca.s   V_1
    IL_0662:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0667:  ldloca.s   V_0
    IL_0669:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_066e:  and
    IL_066f:  brtrue.s   IL_067c

    IL_0671:  ldloca.s   V_2
    IL_0673:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0679:  ldloc.2
    IL_067a:  br.s       IL_0694

    IL_067c:  ldloca.s   V_1
    IL_067e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0683:  ldloca.s   V_0
    IL_0685:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_068a:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_ExclusiveOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_068f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0694:  starg.s    a
    IL_0696:  ldarg.0
    IL_0697:  stloc.0
    IL_0698:  ldarg.2
    IL_0699:  stloc.3
    IL_069a:  ldloca.s   V_0
    IL_069c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_06a1:  ldloca.s   V_3
    IL_06a3:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_06a8:  and
    IL_06a9:  brtrue.s   IL_06b6

    IL_06ab:  ldloca.s   V_1
    IL_06ad:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_06b3:  ldloc.1
    IL_06b4:  br.s       IL_06ce

    IL_06b6:  ldloca.s   V_0
    IL_06b8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_06bd:  ldloca.s   V_3
    IL_06bf:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_06c4:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_LeftShift(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                   int32)
    IL_06c9:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_06ce:  starg.s    a
    IL_06d0:  ldarg.0
    IL_06d1:  stloc.0
    IL_06d2:  ldarg.2
    IL_06d3:  stloc.3
    IL_06d4:  ldloca.s   V_0
    IL_06d6:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_06db:  ldloca.s   V_3
    IL_06dd:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_06e2:  and
    IL_06e3:  brtrue.s   IL_06f0

    IL_06e5:  ldloca.s   V_1
    IL_06e7:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_06ed:  ldloc.1
    IL_06ee:  br.s       IL_0708

    IL_06f0:  ldloca.s   V_0
    IL_06f2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_06f7:  ldloca.s   V_3
    IL_06f9:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_06fe:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_RightShift(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                    int32)
    IL_0703:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0708:  starg.s    a
    IL_070a:  ret
  } // end of method LiftedOperators::StructValueBasic

  .method public hidebysig static void  StructValueComplex(valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> a,
                                                           class [mscorlib]System.Func`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> x,
                                                           class [mscorlib]System.Func`1<int32> i) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method LiftedOperators::StructValueComplex

  .method public hidebysig static bool  RetEq(valuetype [mscorlib]System.Nullable`1<int32> a,
                                              valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       39 (0x27)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000b:  ldloca.s   V_1
    IL_000d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0012:  beq.s      IL_0016

    IL_0014:  ldc.i4.0
    IL_0015:  ret

    IL_0016:  ldloca.s   V_0
    IL_0018:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001d:  ldloca.s   V_1
    IL_001f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0024:  ceq
    IL_0026:  ret
  } // end of method LiftedOperators::RetEq

  .method public hidebysig static bool  RetEqConv(valuetype [mscorlib]System.Nullable`1<int64> a,
                                                  valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       73 (0x49)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int64> V_0,
             valuetype [mscorlib]System.Nullable`1<int64> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             valuetype [mscorlib]System.Nullable`1<int64> V_3)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.2
    IL_0004:  ldloca.s   V_2
    IL_0006:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000b:  brtrue.s   IL_0018

    IL_000d:  ldloca.s   V_3
    IL_000f:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0015:  ldloc.3
    IL_0016:  br.s       IL_0025

    IL_0018:  ldloca.s   V_2
    IL_001a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001f:  conv.i8
    IL_0020:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0025:  stloc.1
    IL_0026:  ldloca.s   V_0
    IL_0028:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_002d:  ldloca.s   V_1
    IL_002f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0034:  beq.s      IL_0038

    IL_0036:  ldc.i4.0
    IL_0037:  ret

    IL_0038:  ldloca.s   V_0
    IL_003a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_003f:  ldloca.s   V_1
    IL_0041:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0046:  ceq
    IL_0048:  ret
  } // end of method LiftedOperators::RetEqConv

  .method public hidebysig static bool  RetEqConst(valuetype [mscorlib]System.Nullable`1<int64> a) cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int64> V_0,
             int64 V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.s   10
    IL_0004:  conv.i8
    IL_0005:  stloc.1
    IL_0006:  ldloca.s   V_0
    IL_0008:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_000d:  ldloc.1
    IL_000e:  beq.s      IL_0012

    IL_0010:  ldc.i4.0
    IL_0011:  ret

    IL_0012:  ldloca.s   V_0
    IL_0014:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0019:  ret
  } // end of method LiftedOperators::RetEqConst

  .method public hidebysig static bool  RetIneqConst(valuetype [mscorlib]System.Nullable`1<int64> a) cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int64> V_0,
             int64 V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.s   10
    IL_0004:  conv.i8
    IL_0005:  stloc.1
    IL_0006:  ldloca.s   V_0
    IL_0008:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_000d:  ldloc.1
    IL_000e:  beq.s      IL_0012

    IL_0010:  ldc.i4.1
    IL_0011:  ret

    IL_0012:  ldloca.s   V_0
    IL_0014:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0019:  ldc.i4.0
    IL_001a:  ceq
    IL_001c:  ret
  } // end of method LiftedOperators::RetIneqConst

  .method public hidebysig static bool  RetLt(valuetype [mscorlib]System.Nullable`1<int32> a,
                                              valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       38 (0x26)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000b:  ldloca.s   V_1
    IL_000d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0012:  blt.s      IL_0016

    IL_0014:  ldc.i4.0
    IL_0015:  ret

    IL_0016:  ldloca.s   V_0
    IL_0018:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001d:  ldloca.s   V_1
    IL_001f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0024:  and
    IL_0025:  ret
  } // end of method LiftedOperators::RetLt

  .method public hidebysig static bool  RetLtConst(valuetype [mscorlib]System.Nullable`1<int32> a) cil managed
  {
    // Code size       25 (0x19)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.s   10
    IL_0004:  stloc.1
    IL_0005:  ldloca.s   V_0
    IL_0007:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000c:  ldloc.1
    IL_000d:  blt.s      IL_0011

    IL_000f:  ldc.i4.0
    IL_0010:  ret

    IL_0011:  ldloca.s   V_0
    IL_0013:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0018:  ret
  } // end of method LiftedOperators::RetLtConst

  .method public hidebysig static bool  RetLtConv(valuetype [mscorlib]System.Nullable`1<int64> a,
                                                  valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       72 (0x48)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int64> V_0,
             valuetype [mscorlib]System.Nullable`1<int64> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             valuetype [mscorlib]System.Nullable`1<int64> V_3)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.2
    IL_0004:  ldloca.s   V_2
    IL_0006:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000b:  brtrue.s   IL_0018

    IL_000d:  ldloca.s   V_3
    IL_000f:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0015:  ldloc.3
    IL_0016:  br.s       IL_0025

    IL_0018:  ldloca.s   V_2
    IL_001a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001f:  conv.i8
    IL_0020:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0025:  stloc.1
    IL_0026:  ldloca.s   V_0
    IL_0028:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_002d:  ldloca.s   V_1
    IL_002f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0034:  blt.s      IL_0038

    IL_0036:  ldc.i4.0
    IL_0037:  ret

    IL_0038:  ldloca.s   V_0
    IL_003a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_003f:  ldloca.s   V_1
    IL_0041:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0046:  and
    IL_0047:  ret
  } // end of method LiftedOperators::RetLtConv

  .method public hidebysig static bool  RetNotLt(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                 valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000b:  ldloca.s   V_1
    IL_000d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0012:  blt.s      IL_0017

    IL_0014:  ldc.i4.0
    IL_0015:  br.s       IL_0026

    IL_0017:  ldloca.s   V_0
    IL_0019:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001e:  ldloca.s   V_1
    IL_0020:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0025:  and
    IL_0026:  ldc.i4.0
    IL_0027:  ceq
    IL_0029:  ret
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
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_UnaryPlus

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_UnaryNegation(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_UnaryNegation

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_LogicalNot(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_LogicalNot

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_OnesComplement(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_OnesComplement

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_Increment(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_Increment

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_Decrement(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_Decrement

  .method public hidebysig specialname static 
          int32  op_Explicit(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_Explicit

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_Addition(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_Addition

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_Subtraction(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                         valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_Subtraction

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_Multiply(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_Multiply

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_Division(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_Division

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_Modulus(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_Modulus

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                        valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_BitwiseAnd

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_BitwiseOr

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_ExclusiveOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                         valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_ExclusiveOr

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_LeftShift(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                       int32 b) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_LeftShift

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS 
          op_RightShift(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                        int32 b) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_RightShift

  .method public hidebysig specialname static 
          bool  op_Equality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_Equality

  .method public hidebysig specialname static 
          bool  op_Inequality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_Inequality

  .method public hidebysig specialname static 
          bool  op_LessThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_LessThan

  .method public hidebysig specialname static 
          bool  op_LessThanOrEqual(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                                   valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_LessThanOrEqual

  .method public hidebysig specialname static 
          bool  op_GreaterThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_GreaterThan

  .method public hidebysig specialname static 
          bool  op_GreaterThanOrEqual(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS a,
                                      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS b) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::op_GreaterThanOrEqual

  .method public hidebysig virtual instance bool 
          Equals(object obj) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  throw
  } // end of method TS::GetHashCode

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedImplicitConversions
       extends [mscorlib]System.Object
{
  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int32> 
          ExtendI4(valuetype [mscorlib]System.Nullable`1<uint8> b) cil managed
  {
    // Code size       34 (0x22)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<uint8> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint8>::get_HasValue()
    IL_0009:  brtrue.s   IL_0015

    IL_000b:  ldloca.s   V_1
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0013:  ldloc.1
    IL_0014:  ret

    IL_0015:  ldloca.s   V_0
    IL_0017:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint8>::GetValueOrDefault()
    IL_001c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0021:  ret
  } // end of method LiftedImplicitConversions::ExtendI4

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int32> 
          ExtendToI4(valuetype [mscorlib]System.Nullable`1<int8> b) cil managed
  {
    // Code size       34 (0x22)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int8> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int8>::get_HasValue()
    IL_0009:  brtrue.s   IL_0015

    IL_000b:  ldloca.s   V_1
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0013:  ldloc.1
    IL_0014:  ret

    IL_0015:  ldloca.s   V_0
    IL_0017:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int8>::GetValueOrDefault()
    IL_001c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0021:  ret
  } // end of method LiftedImplicitConversions::ExtendToI4

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int64> 
          ExtendI8(valuetype [mscorlib]System.Nullable`1<uint8> b) cil managed
  {
    // Code size       35 (0x23)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<uint8> V_0,
             valuetype [mscorlib]System.Nullable`1<int64> V_1)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint8>::get_HasValue()
    IL_0009:  brtrue.s   IL_0015

    IL_000b:  ldloca.s   V_1
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0013:  ldloc.1
    IL_0014:  ret

    IL_0015:  ldloca.s   V_0
    IL_0017:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint8>::GetValueOrDefault()
    IL_001c:  conv.u8
    IL_001d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0022:  ret
  } // end of method LiftedImplicitConversions::ExtendI8

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int64> 
          ExtendToI8(valuetype [mscorlib]System.Nullable`1<int8> b) cil managed
  {
    // Code size       35 (0x23)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int8> V_0,
             valuetype [mscorlib]System.Nullable`1<int64> V_1)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int8>::get_HasValue()
    IL_0009:  brtrue.s   IL_0015

    IL_000b:  ldloca.s   V_1
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0013:  ldloc.1
    IL_0014:  ret

    IL_0015:  ldloca.s   V_0
    IL_0017:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int8>::GetValueOrDefault()
    IL_001c:  conv.i8
    IL_001d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0022:  ret
  } // end of method LiftedImplicitConversions::ExtendToI8

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int64> 
          ExtendI8(valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       35 (0x23)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int64> V_1)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brtrue.s   IL_0015

    IL_000b:  ldloca.s   V_1
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0013:  ldloc.1
    IL_0014:  ret

    IL_0015:  ldloca.s   V_0
    IL_0017:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001c:  conv.i8
    IL_001d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0022:  ret
  } // end of method LiftedImplicitConversions::ExtendI8

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int64> 
          ExtendToI8(valuetype [mscorlib]System.Nullable`1<uint32> b) cil managed
  {
    // Code size       35 (0x23)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<uint32> V_0,
             valuetype [mscorlib]System.Nullable`1<int64> V_1)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint32>::get_HasValue()
    IL_0009:  brtrue.s   IL_0015

    IL_000b:  ldloca.s   V_1
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0013:  ldloc.1
    IL_0014:  ret

    IL_0015:  ldloca.s   V_0
    IL_0017:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint32>::GetValueOrDefault()
    IL_001c:  conv.u8
    IL_001d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0022:  ret
  } // end of method LiftedImplicitConversions::ExtendToI8

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int64> 
          AfterArithmetic(valuetype [mscorlib]System.Nullable`1<uint32> b) cil managed
  {
    // Code size       40 (0x28)
    .maxstack  2
    .locals init (uint32 V_0,
             valuetype [mscorlib]System.Nullable`1<uint32> V_1,
             valuetype [mscorlib]System.Nullable`1<int64> V_2)
    IL_0000:  ldc.i4.s   100
    IL_0002:  stloc.0
    IL_0003:  ldarg.1
    IL_0004:  stloc.1
    IL_0005:  ldloca.s   V_1
    IL_0007:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint32>::get_HasValue()
    IL_000c:  brtrue.s   IL_0018

    IL_000e:  ldloca.s   V_2
    IL_0010:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0016:  ldloc.2
    IL_0017:  ret

    IL_0018:  ldloc.0
    IL_0019:  ldloca.s   V_1
    IL_001b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint32>::GetValueOrDefault()
    IL_0020:  add
    IL_0021:  conv.u8
    IL_0022:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0027:  ret
  } // end of method LiftedImplicitConversions::AfterArithmetic

  .method public hidebysig static valuetype [mscorlib]System.Nullable`1<int64> 
          InArithmetic3(valuetype [mscorlib]System.Nullable`1<int32> a,
                        valuetype [mscorlib]System.Nullable`1<int64> b,
                        valuetype [mscorlib]System.Nullable`1<int32> c,
                        int64 d) cil managed
  {
    // Code size       216 (0xd8)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int64> V_0,
             int64 V_1,
             valuetype [mscorlib]System.Nullable`1<int64> V_2,
             valuetype [mscorlib]System.Nullable`1<int64> V_3,
             valuetype [mscorlib]System.Nullable`1<int64> V_4,
             valuetype [mscorlib]System.Nullable`1<int64> V_5,
             valuetype [mscorlib]System.Nullable`1<int32> V_6,
             valuetype [mscorlib]System.Nullable`1<int64> V_7)
    IL_0000:  ldarg.0
    IL_0001:  stloc.s    V_6
    IL_0003:  ldloca.s   V_6
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_0018

    IL_000c:  ldloca.s   V_7
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0014:  ldloc.s    V_7
    IL_0016:  br.s       IL_0025

    IL_0018:  ldloca.s   V_6
    IL_001a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001f:  conv.i8
    IL_0020:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0025:  stloc.s    V_4
    IL_0027:  ldarg.1
    IL_0028:  stloc.s    V_5
    IL_002a:  ldloca.s   V_4
    IL_002c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0031:  ldloca.s   V_5
    IL_0033:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0038:  and
    IL_0039:  brtrue.s   IL_0047

    IL_003b:  ldloca.s   V_7
    IL_003d:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0043:  ldloc.s    V_7
    IL_0045:  br.s       IL_005b

    IL_0047:  ldloca.s   V_4
    IL_0049:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_004e:  ldloca.s   V_5
    IL_0050:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0055:  add
    IL_0056:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_005b:  stloc.2
    IL_005c:  ldarg.2
    IL_005d:  stloc.s    V_6
    IL_005f:  ldloca.s   V_6
    IL_0061:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0066:  brtrue.s   IL_0074

    IL_0068:  ldloca.s   V_5
    IL_006a:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0070:  ldloc.s    V_5
    IL_0072:  br.s       IL_0081

    IL_0074:  ldloca.s   V_6
    IL_0076:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_007b:  conv.i8
    IL_007c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0081:  stloc.3
    IL_0082:  ldloca.s   V_2
    IL_0084:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0089:  ldloca.s   V_3
    IL_008b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0090:  and
    IL_0091:  brtrue.s   IL_009f

    IL_0093:  ldloca.s   V_5
    IL_0095:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_009b:  ldloc.s    V_5
    IL_009d:  br.s       IL_00b3

    IL_009f:  ldloca.s   V_2
    IL_00a1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_00a6:  ldloca.s   V_3
    IL_00a8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_00ad:  add
    IL_00ae:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_00b3:  stloc.0
    IL_00b4:  ldarg.3
    IL_00b5:  stloc.1
    IL_00b6:  ldloca.s   V_0
    IL_00b8:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_00bd:  brtrue.s   IL_00c9

    IL_00bf:  ldloca.s   V_3
    IL_00c1:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_00c7:  ldloc.3
    IL_00c8:  ret

    IL_00c9:  ldloca.s   V_0
    IL_00cb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_00d0:  ldloc.1
    IL_00d1:  add
    IL_00d2:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_00d7:  ret
  } // end of method LiftedImplicitConversions::InArithmetic3

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method LiftedImplicitConversions::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedImplicitConversions

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions
       extends [mscorlib]System.Object
{
  .method private hidebysig static void  Print<valuetype .ctor ([mscorlib]System.ValueType) T>(valuetype [mscorlib]System.Nullable`1<!!T> x) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  box        valuetype [mscorlib]System.Nullable`1<!!T>
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_000b:  ret
  } // end of method LiftedExplicitConversions::Print

  .method public hidebysig static void  UncheckedCasts(valuetype [mscorlib]System.Nullable`1<int32> i4,
                                                       valuetype [mscorlib]System.Nullable`1<int64> i8,
                                                       valuetype [mscorlib]System.Nullable`1<float32> f) cil managed
  {
    // Code size       202 (0xca)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<uint8> V_1,
             valuetype [mscorlib]System.Nullable`1<int16> V_2,
             valuetype [mscorlib]System.Nullable`1<uint32> V_3,
             valuetype [mscorlib]System.Nullable`1<int64> V_4,
             valuetype [mscorlib]System.Nullable`1<float32> V_5)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brtrue.s   IL_0016

    IL_000b:  ldloca.s   V_1
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<uint8>
    IL_0013:  ldloc.1
    IL_0014:  br.s       IL_0023

    IL_0016:  ldloca.s   V_0
    IL_0018:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001d:  conv.u1
    IL_001e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint8>::.ctor(!0)
    IL_0023:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint8>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_0028:  ldarg.0
    IL_0029:  stloc.0
    IL_002a:  ldloca.s   V_0
    IL_002c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0031:  brtrue.s   IL_003e

    IL_0033:  ldloca.s   V_2
    IL_0035:  initobj    valuetype [mscorlib]System.Nullable`1<int16>
    IL_003b:  ldloc.2
    IL_003c:  br.s       IL_004b

    IL_003e:  ldloca.s   V_0
    IL_0040:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0045:  conv.i2
    IL_0046:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int16>::.ctor(!0)
    IL_004b:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<int16>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_0050:  ldarg.0
    IL_0051:  stloc.0
    IL_0052:  ldloca.s   V_0
    IL_0054:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0059:  brtrue.s   IL_0066

    IL_005b:  ldloca.s   V_3
    IL_005d:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_0063:  ldloc.3
    IL_0064:  br.s       IL_0072

    IL_0066:  ldloca.s   V_0
    IL_0068:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_006d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_0072:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_0077:  ldarg.1
    IL_0078:  stloc.s    V_4
    IL_007a:  ldloca.s   V_4
    IL_007c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0081:  brtrue.s   IL_008e

    IL_0083:  ldloca.s   V_3
    IL_0085:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_008b:  ldloc.3
    IL_008c:  br.s       IL_009b

    IL_008e:  ldloca.s   V_4
    IL_0090:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0095:  conv.u4
    IL_0096:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_009b:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_00a0:  ldarg.2
    IL_00a1:  stloc.s    V_5
    IL_00a3:  ldloca.s   V_5
    IL_00a5:  call       instance bool valuetype [mscorlib]System.Nullable`1<float32>::get_HasValue()
    IL_00aa:  brtrue.s   IL_00b7

    IL_00ac:  ldloca.s   V_3
    IL_00ae:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_00b4:  ldloc.3
    IL_00b5:  br.s       IL_00c4

    IL_00b7:  ldloca.s   V_5
    IL_00b9:  call       instance !0 valuetype [mscorlib]System.Nullable`1<float32>::GetValueOrDefault()
    IL_00be:  conv.u4
    IL_00bf:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_00c4:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_00c9:  ret
  } // end of method LiftedExplicitConversions::UncheckedCasts

  .method public hidebysig static void  CheckedCasts(valuetype [mscorlib]System.Nullable`1<int32> i4,
                                                     valuetype [mscorlib]System.Nullable`1<int64> i8,
                                                     valuetype [mscorlib]System.Nullable`1<float32> f) cil managed
  {
    // Code size       162 (0xa2)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<uint8> V_1,
             valuetype [mscorlib]System.Nullable`1<int16> V_2,
             valuetype [mscorlib]System.Nullable`1<uint32> V_3,
             valuetype [mscorlib]System.Nullable`1<int64> V_4)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brtrue.s   IL_0016

    IL_000b:  ldloca.s   V_1
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<uint8>
    IL_0013:  ldloc.1
    IL_0014:  br.s       IL_0023

    IL_0016:  ldloca.s   V_0
    IL_0018:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001d:  conv.ovf.u1
    IL_001e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint8>::.ctor(!0)
    IL_0023:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint8>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_0028:  ldarg.0
    IL_0029:  stloc.0
    IL_002a:  ldloca.s   V_0
    IL_002c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0031:  brtrue.s   IL_003e

    IL_0033:  ldloca.s   V_2
    IL_0035:  initobj    valuetype [mscorlib]System.Nullable`1<int16>
    IL_003b:  ldloc.2
    IL_003c:  br.s       IL_004b

    IL_003e:  ldloca.s   V_0
    IL_0040:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0045:  conv.ovf.i2
    IL_0046:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int16>::.ctor(!0)
    IL_004b:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<int16>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_0050:  ldarg.0
    IL_0051:  stloc.0
    IL_0052:  ldloca.s   V_0
    IL_0054:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0059:  brtrue.s   IL_0066

    IL_005b:  ldloca.s   V_3
    IL_005d:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_0063:  ldloc.3
    IL_0064:  br.s       IL_0073

    IL_0066:  ldloca.s   V_0
    IL_0068:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_006d:  conv.ovf.u4
    IL_006e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_0073:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_0078:  ldarg.1
    IL_0079:  stloc.s    V_4
    IL_007b:  ldloca.s   V_4
    IL_007d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0082:  brtrue.s   IL_008f

    IL_0084:  ldloca.s   V_3
    IL_0086:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_008c:  ldloc.3
    IL_008d:  br.s       IL_009c

    IL_008f:  ldloca.s   V_4
    IL_0091:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0096:  conv.ovf.u4
    IL_0097:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_009c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_00a1:  ret
  } // end of method LiftedExplicitConversions::CheckedCasts

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method LiftedExplicitConversions::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullCoalescingTests
       extends [mscorlib]System.Object
{
  .method private hidebysig static void  Print<T>(!!T x) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  box        !!T
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_000b:  ret
  } // end of method NullCoalescingTests::Print

  .method public hidebysig static void  Objects(object a,
                                                object b) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  brtrue.s   IL_0006

    IL_0004:  pop
    IL_0005:  ldarg.1
    IL_0006:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullCoalescingTests::Print<object>(!!0)
    IL_000b:  ret
  } // end of method NullCoalescingTests::Objects

  .method public hidebysig static void  Nullables(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                  valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       21 (0x15)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brtrue.s   IL_000e

    IL_000b:  ldarg.1
    IL_000c:  br.s       IL_000f

    IL_000e:  ldloc.0
    IL_000f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullCoalescingTests::Print<valuetype [mscorlib]System.Nullable`1<int32>>(!!0)
    IL_0014:  ret
  } // end of method NullCoalescingTests::Nullables

  .method public hidebysig static void  NullableWithNonNullableFallback(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                                        int32 b) cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brtrue.s   IL_000e

    IL_000b:  ldarg.1
    IL_000c:  br.s       IL_0015

    IL_000e:  ldloca.s   V_0
    IL_0010:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0015:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullCoalescingTests::Print<int32>(!!0)
    IL_001a:  ret
  } // end of method NullCoalescingTests::NullableWithNonNullableFallback

  .method public hidebysig static void  NullableWithImplicitConversion(valuetype [mscorlib]System.Nullable`1<int16> a,
                                                                       valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       32 (0x20)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int16> V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int16>::get_HasValue()
    IL_0009:  brtrue.s   IL_000e

    IL_000b:  ldarg.1
    IL_000c:  br.s       IL_001a

    IL_000e:  ldloca.s   V_0
    IL_0010:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int16>::GetValueOrDefault()
    IL_0015:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_001a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullCoalescingTests::Print<valuetype [mscorlib]System.Nullable`1<int32>>(!!0)
    IL_001f:  ret
  } // end of method NullCoalescingTests::NullableWithImplicitConversion

  .method public hidebysig static void  NullableWithImplicitConversionAndNonNullableFallback(valuetype [mscorlib]System.Nullable`1<int16> a,
                                                                                             int32 b) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method NullCoalescingTests::NullableWithImplicitConversionAndNonNullableFallback

  .method public hidebysig static void  Chain(valuetype [mscorlib]System.Nullable`1<int32> a,
                                              valuetype [mscorlib]System.Nullable`1<int32> b,
                                              valuetype [mscorlib]System.Nullable`1<int32> c,
                                              int32 d) cil managed
  {
    // Code size       67 (0x43)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brtrue.s   IL_0036

    IL_000b:  ldarg.1
    IL_000c:  stloc.1
    IL_000d:  ldloca.s   V_1
    IL_000f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0014:  brtrue.s   IL_002d

    IL_0016:  ldarg.2
    IL_0017:  stloc.2
    IL_0018:  ldloca.s   V_2
    IL_001a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001f:  brtrue.s   IL_0024

    IL_0021:  ldarg.3
    IL_0022:  br.s       IL_003d

    IL_0024:  ldloca.s   V_2
    IL_0026:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_002b:  br.s       IL_003d

    IL_002d:  ldloca.s   V_1
    IL_002f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0034:  br.s       IL_003d

    IL_0036:  ldloca.s   V_0
    IL_0038:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_003d:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullCoalescingTests::Print<int32>(!!0)
    IL_0042:  ret
  } // end of method NullCoalescingTests::Chain

  .method public hidebysig static void  ChainWithImplicitConversions(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                                     valuetype [mscorlib]System.Nullable`1<int16> b,
                                                                     valuetype [mscorlib]System.Nullable`1<int64> c,
                                                                     uint8 d) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method NullCoalescingTests::ChainWithImplicitConversions

  .method public hidebysig static void  ChainWithComputation(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                             valuetype [mscorlib]System.Nullable`1<int16> b,
                                                             valuetype [mscorlib]System.Nullable`1<int64> c,
                                                             uint8 d) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method NullCoalescingTests::ChainWithComputation

  .method public hidebysig static object 
          ReturnObjects(object a,
                        object b) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  brtrue.s   IL_0006

    IL_0004:  pop
    IL_0005:  ldarg.1
    IL_0006:  ret
  } // end of method NullCoalescingTests::ReturnObjects

  .method public hidebysig static valuetype [mscorlib]System.Nullable`1<int32> 
          ReturnNullables(valuetype [mscorlib]System.Nullable`1<int32> a,
                          valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       15 (0xf)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brtrue.s   IL_000d

    IL_000b:  ldarg.1
    IL_000c:  ret

    IL_000d:  ldloc.0
    IL_000e:  ret
  } // end of method NullCoalescingTests::ReturnNullables

  .method public hidebysig static int32  ReturnNullableWithNonNullableFallback(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                                               int32 b) cil managed
  {
    // Code size       21 (0x15)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brtrue.s   IL_000d

    IL_000b:  ldarg.1
    IL_000c:  ret

    IL_000d:  ldloca.s   V_0
    IL_000f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0014:  ret
  } // end of method NullCoalescingTests::ReturnNullableWithNonNullableFallback

  .method public hidebysig static int32  ReturnChain(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                     valuetype [mscorlib]System.Nullable`1<int32> b,
                                                     valuetype [mscorlib]System.Nullable`1<int32> c,
                                                     int32 d) cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brtrue.s   IL_0033

    IL_000b:  ldarg.1
    IL_000c:  stloc.1
    IL_000d:  ldloca.s   V_1
    IL_000f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0014:  brtrue.s   IL_002b

    IL_0016:  ldarg.2
    IL_0017:  stloc.2
    IL_0018:  ldloca.s   V_2
    IL_001a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001f:  brtrue.s   IL_0023

    IL_0021:  ldarg.3
    IL_0022:  ret

    IL_0023:  ldloca.s   V_2
    IL_0025:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_002a:  ret

    IL_002b:  ldloca.s   V_1
    IL_002d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0032:  ret

    IL_0033:  ldloca.s   V_0
    IL_0035:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_003a:  ret
  } // end of method NullCoalescingTests::ReturnChain

  .method public hidebysig static int64  ReturnChainWithImplicitConversions(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                                            valuetype [mscorlib]System.Nullable`1<int16> b,
                                                                            valuetype [mscorlib]System.Nullable`1<int64> c,
                                                                            uint8 d) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  ret
  } // end of method NullCoalescingTests::ReturnChainWithImplicitConversions

  .method public hidebysig static int64  ReturnChainWithComputation(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                                    valuetype [mscorlib]System.Nullable`1<int16> b,
                                                                    valuetype [mscorlib]System.Nullable`1<int64> c,
                                                                    uint8 d) cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  conv.i8
    IL_0002:  ret
  } // end of method NullCoalescingTests::ReturnChainWithComputation

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method NullCoalescingTests::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullCoalescingTests


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
