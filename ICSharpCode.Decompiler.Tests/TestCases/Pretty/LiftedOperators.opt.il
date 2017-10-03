
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.18020
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly n45w5r1e
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module n45w5r1e.dll
// MVID: {A02D3ECA-E4C6-46ED-A628-8A99594A9C16}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02DA0000


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
             valuetype [mscorlib]System.Nullable`1<bool> V_1,
             valuetype [mscorlib]System.Nullable`1<bool> V_2,
             valuetype [mscorlib]System.Nullable`1<bool> V_3)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_000b:  ldloca.s   V_1
    IL_000d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0012:  bne.un.s   IL_0026

    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001b:  ldloca.s   V_1
    IL_001d:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0022:  ceq
    IL_0024:  br.s       IL_0027

    IL_0026:  ldc.i4.0
    IL_0027:  brfalse.s  IL_002e

    IL_0029:  call       void [mscorlib]System.Console::WriteLine()
    IL_002e:  ldarg.0
    IL_002f:  stloc.2
    IL_0030:  ldarg.1
    IL_0031:  stloc.3
    IL_0032:  ldloca.s   V_2
    IL_0034:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0039:  ldloca.s   V_3
    IL_003b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0040:  bne.un.s   IL_0057

    IL_0042:  ldloca.s   V_2
    IL_0044:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0049:  ldloca.s   V_3
    IL_004b:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0050:  ceq
    IL_0052:  ldc.i4.0
    IL_0053:  ceq
    IL_0055:  br.s       IL_0058

    IL_0057:  ldc.i4.1
    IL_0058:  brfalse.s  IL_005f

    IL_005a:  call       void [mscorlib]System.Console::WriteLine()
    IL_005f:  ret
  } // end of method LiftedOperators::BoolBasic

  .method public hidebysig static void  BoolComplex(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                    class [mscorlib]System.Func`1<bool> x) cil managed
  {
    // Code size       157 (0x9d)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<bool> V_0,
             bool V_1,
             valuetype [mscorlib]System.Nullable`1<bool> V_2,
             bool V_3,
             bool V_4,
             valuetype [mscorlib]System.Nullable`1<bool> V_5,
             bool V_6,
             valuetype [mscorlib]System.Nullable`1<bool> V_7)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0008:  stloc.1
    IL_0009:  ldloca.s   V_0
    IL_000b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0010:  ldloc.1
    IL_0011:  bne.un.s   IL_001c

    IL_0013:  ldloca.s   V_0
    IL_0015:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001a:  br.s       IL_001d

    IL_001c:  ldc.i4.0
    IL_001d:  brfalse.s  IL_0024

    IL_001f:  call       void [mscorlib]System.Console::WriteLine()
    IL_0024:  ldarg.0
    IL_0025:  stloc.2
    IL_0026:  ldarg.1
    IL_0027:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_002c:  stloc.3
    IL_002d:  ldloca.s   V_2
    IL_002f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0034:  ldloc.3
    IL_0035:  bne.un.s   IL_0043

    IL_0037:  ldloca.s   V_2
    IL_0039:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_003e:  ldc.i4.0
    IL_003f:  ceq
    IL_0041:  br.s       IL_0044

    IL_0043:  ldc.i4.1
    IL_0044:  brfalse.s  IL_004b

    IL_0046:  call       void [mscorlib]System.Console::WriteLine()
    IL_004b:  ldarg.1
    IL_004c:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0051:  stloc.s    V_4
    IL_0053:  ldarg.0
    IL_0054:  stloc.s    V_5
    IL_0056:  ldloc.s    V_4
    IL_0058:  ldloca.s   V_5
    IL_005a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_005f:  bne.un.s   IL_006a

    IL_0061:  ldloca.s   V_5
    IL_0063:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0068:  br.s       IL_006b

    IL_006a:  ldc.i4.0
    IL_006b:  brfalse.s  IL_0072

    IL_006d:  call       void [mscorlib]System.Console::WriteLine()
    IL_0072:  ldarg.1
    IL_0073:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0078:  stloc.s    V_6
    IL_007a:  ldarg.0
    IL_007b:  stloc.s    V_7
    IL_007d:  ldloc.s    V_6
    IL_007f:  ldloca.s   V_7
    IL_0081:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0086:  bne.un.s   IL_0094

    IL_0088:  ldloca.s   V_7
    IL_008a:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_008f:  ldc.i4.0
    IL_0090:  ceq
    IL_0092:  br.s       IL_0095

    IL_0094:  ldc.i4.1
    IL_0095:  brfalse.s  IL_009c

    IL_0097:  call       void [mscorlib]System.Console::WriteLine()
    IL_009c:  ret
  } // end of method LiftedOperators::BoolComplex

  .method public hidebysig static void  BoolConst(valuetype [mscorlib]System.Nullable`1<bool> a) cil managed
  {
    // Code size       177 (0xb1)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<bool> V_0,
             valuetype [mscorlib]System.Nullable`1<bool> V_1,
             valuetype [mscorlib]System.Nullable`1<bool> V_2,
             valuetype [mscorlib]System.Nullable`1<bool> V_3,
             valuetype [mscorlib]System.Nullable`1<bool> V_4,
             valuetype [mscorlib]System.Nullable`1<bool> V_5)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0009:  brfalse.s  IL_0014

    IL_000b:  ldloca.s   V_0
    IL_000d:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0012:  br.s       IL_0015

    IL_0014:  ldc.i4.0
    IL_0015:  brfalse.s  IL_001c

    IL_0017:  call       void [mscorlib]System.Console::WriteLine()
    IL_001c:  ldarg.0
    IL_001d:  stloc.1
    IL_001e:  ldloca.s   V_1
    IL_0020:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0025:  brfalse.s  IL_0033

    IL_0027:  ldloca.s   V_1
    IL_0029:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_002e:  ldc.i4.0
    IL_002f:  ceq
    IL_0031:  br.s       IL_0034

    IL_0033:  ldc.i4.1
    IL_0034:  brfalse.s  IL_003b

    IL_0036:  call       void [mscorlib]System.Console::WriteLine()
    IL_003b:  ldarg.0
    IL_003c:  stloc.2
    IL_003d:  ldloca.s   V_2
    IL_003f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0044:  brtrue.s   IL_004f

    IL_0046:  ldloca.s   V_2
    IL_0048:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_004d:  br.s       IL_0050

    IL_004f:  ldc.i4.0
    IL_0050:  brfalse.s  IL_0057

    IL_0052:  call       void [mscorlib]System.Console::WriteLine()
    IL_0057:  ldarg.0
    IL_0058:  stloc.3
    IL_0059:  ldloca.s   V_3
    IL_005b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0060:  brtrue.s   IL_006e

    IL_0062:  ldloca.s   V_3
    IL_0064:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0069:  ldc.i4.0
    IL_006a:  ceq
    IL_006c:  br.s       IL_006f

    IL_006e:  ldc.i4.1
    IL_006f:  brfalse.s  IL_0076

    IL_0071:  call       void [mscorlib]System.Console::WriteLine()
    IL_0076:  ldarg.0
    IL_0077:  stloc.s    V_4
    IL_0079:  ldloca.s   V_4
    IL_007b:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0080:  brtrue.s   IL_0085

    IL_0082:  ldc.i4.1
    IL_0083:  br.s       IL_008c

    IL_0085:  ldloca.s   V_4
    IL_0087:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_008c:  brfalse.s  IL_0093

    IL_008e:  call       void [mscorlib]System.Console::WriteLine()
    IL_0093:  ldarg.0
    IL_0094:  stloc.s    V_5
    IL_0096:  ldloca.s   V_5
    IL_0098:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_009d:  brtrue.s   IL_00a2

    IL_009f:  ldc.i4.0
    IL_00a0:  br.s       IL_00a9

    IL_00a2:  ldloca.s   V_5
    IL_00a4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00a9:  brfalse.s  IL_00b0

    IL_00ab:  call       void [mscorlib]System.Console::WriteLine()
    IL_00b0:  ret
  } // end of method LiftedOperators::BoolConst

  .method public hidebysig static void  BoolValueBasic(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                       valuetype [mscorlib]System.Nullable`1<bool> b) cil managed
  {
    // Code size       480 (0x1e0)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<bool> V_0,
             valuetype [mscorlib]System.Nullable`1<bool> V_1,
             valuetype [mscorlib]System.Nullable`1<bool> V_2,
             valuetype [mscorlib]System.Nullable`1<bool> V_3,
             valuetype [mscorlib]System.Nullable`1<bool> V_4,
             valuetype [mscorlib]System.Nullable`1<bool> V_5,
             valuetype [mscorlib]System.Nullable`1<bool> V_6,
             valuetype [mscorlib]System.Nullable`1<bool> V_7,
             valuetype [mscorlib]System.Nullable`1<bool> V_8,
             valuetype [mscorlib]System.Nullable`1<bool> V_9,
             valuetype [mscorlib]System.Nullable`1<bool> V_10,
             valuetype [mscorlib]System.Nullable`1<bool> V_11,
             valuetype [mscorlib]System.Nullable`1<bool> V_12,
             valuetype [mscorlib]System.Nullable`1<bool> V_13,
             valuetype [mscorlib]System.Nullable`1<bool> V_14,
             valuetype [mscorlib]System.Nullable`1<bool> V_15,
             valuetype [mscorlib]System.Nullable`1<bool> V_16,
             valuetype [mscorlib]System.Nullable`1<bool> V_17,
             valuetype [mscorlib]System.Nullable`1<bool> V_18,
             valuetype [mscorlib]System.Nullable`1<bool> V_19,
             valuetype [mscorlib]System.Nullable`1<bool> V_20)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_000b:  ldloca.s   V_1
    IL_000d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0012:  bne.un.s   IL_0026

    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001b:  ldloca.s   V_1
    IL_001d:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0022:  ceq
    IL_0024:  br.s       IL_0027

    IL_0026:  ldc.i4.0
    IL_0027:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_002c:  ldarg.0
    IL_002d:  stloc.2
    IL_002e:  ldarg.1
    IL_002f:  stloc.3
    IL_0030:  ldloca.s   V_2
    IL_0032:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0037:  ldloca.s   V_3
    IL_0039:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_003e:  bne.un.s   IL_0055

    IL_0040:  ldloca.s   V_2
    IL_0042:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0047:  ldloca.s   V_3
    IL_0049:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_004e:  ceq
    IL_0050:  ldc.i4.0
    IL_0051:  ceq
    IL_0053:  br.s       IL_0056

    IL_0055:  ldc.i4.1
    IL_0056:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_005b:  ldarg.0
    IL_005c:  stloc.s    V_4
    IL_005e:  ldarg.1
    IL_005f:  stloc.s    V_5
    IL_0061:  ldloca.s   V_4
    IL_0063:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0068:  brtrue.s   IL_0080

    IL_006a:  ldloca.s   V_5
    IL_006c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0071:  brtrue.s   IL_007c

    IL_0073:  ldloca.s   V_4
    IL_0075:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_007a:  brfalse.s  IL_0080

    IL_007c:  ldloc.s    V_4
    IL_007e:  br.s       IL_0082

    IL_0080:  ldloc.s    V_5
    IL_0082:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0087:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_008c:  ldarg.0
    IL_008d:  stloc.s    V_6
    IL_008f:  ldarg.1
    IL_0090:  stloc.s    V_7
    IL_0092:  ldloca.s   V_6
    IL_0094:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0099:  brtrue.s   IL_00b1

    IL_009b:  ldloca.s   V_7
    IL_009d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00a2:  brtrue.s   IL_00ad

    IL_00a4:  ldloca.s   V_6
    IL_00a6:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00ab:  brfalse.s  IL_00b1

    IL_00ad:  ldloc.s    V_7
    IL_00af:  br.s       IL_00b3

    IL_00b1:  ldloc.s    V_6
    IL_00b3:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_00b8:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00bd:  ldarg.0
    IL_00be:  stloc.s    V_8
    IL_00c0:  ldarg.1
    IL_00c1:  stloc.s    V_9
    IL_00c3:  ldloca.s   V_8
    IL_00c5:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00ca:  ldloca.s   V_9
    IL_00cc:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00d1:  and
    IL_00d2:  brtrue.s   IL_00e0

    IL_00d4:  ldloca.s   V_10
    IL_00d6:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_00dc:  ldloc.s    V_10
    IL_00de:  br.s       IL_00f4

    IL_00e0:  ldloca.s   V_8
    IL_00e2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00e7:  ldloca.s   V_9
    IL_00e9:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00ee:  xor
    IL_00ef:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_00f4:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_00f9:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00fe:  ldarg.0
    IL_00ff:  stloc.s    V_11
    IL_0101:  ldloca.s   V_11
    IL_0103:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0108:  brtrue.s   IL_010d

    IL_010a:  ldarg.1
    IL_010b:  br.s       IL_0119

    IL_010d:  ldloca.s   V_11
    IL_010f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0114:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0119:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_011e:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0123:  ldarg.0
    IL_0124:  stloc.s    V_12
    IL_0126:  ldloca.s   V_12
    IL_0128:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_012d:  brtrue.s   IL_013b

    IL_012f:  ldloca.s   V_13
    IL_0131:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_0137:  ldloc.s    V_13
    IL_0139:  br.s       IL_014a

    IL_013b:  ldloca.s   V_12
    IL_013d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0142:  ldc.i4.0
    IL_0143:  ceq
    IL_0145:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_014a:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_014f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0154:  ldarg.0
    IL_0155:  stloc.s    V_14
    IL_0157:  ldarg.1
    IL_0158:  stloc.s    V_15
    IL_015a:  ldloca.s   V_14
    IL_015c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0161:  brtrue.s   IL_0179

    IL_0163:  ldloca.s   V_15
    IL_0165:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_016a:  brtrue.s   IL_0175

    IL_016c:  ldloca.s   V_14
    IL_016e:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0173:  brfalse.s  IL_0179

    IL_0175:  ldloc.s    V_14
    IL_0177:  br.s       IL_017b

    IL_0179:  ldloc.s    V_15
    IL_017b:  starg.s    a
    IL_017d:  ldarg.0
    IL_017e:  stloc.s    V_16
    IL_0180:  ldarg.1
    IL_0181:  stloc.s    V_17
    IL_0183:  ldloca.s   V_16
    IL_0185:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_018a:  brtrue.s   IL_01a2

    IL_018c:  ldloca.s   V_17
    IL_018e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0193:  brtrue.s   IL_019e

    IL_0195:  ldloca.s   V_16
    IL_0197:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_019c:  brfalse.s  IL_01a2

    IL_019e:  ldloc.s    V_17
    IL_01a0:  br.s       IL_01a4

    IL_01a2:  ldloc.s    V_16
    IL_01a4:  starg.s    a
    IL_01a6:  ldarg.0
    IL_01a7:  stloc.s    V_18
    IL_01a9:  ldarg.1
    IL_01aa:  stloc.s    V_19
    IL_01ac:  ldloca.s   V_18
    IL_01ae:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_01b3:  ldloca.s   V_19
    IL_01b5:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_01ba:  and
    IL_01bb:  brtrue.s   IL_01c9

    IL_01bd:  ldloca.s   V_20
    IL_01bf:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_01c5:  ldloc.s    V_20
    IL_01c7:  br.s       IL_01dd

    IL_01c9:  ldloca.s   V_18
    IL_01cb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01d0:  ldloca.s   V_19
    IL_01d2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01d7:  xor
    IL_01d8:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_01dd:  starg.s    a
    IL_01df:  ret
  } // end of method LiftedOperators::BoolValueBasic

  .method public hidebysig static void  BoolValueComplex(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                         class [mscorlib]System.Func`1<bool> x) cil managed
  {
    // Code size       552 (0x228)
    .maxstack  3
    .locals init (valuetype [mscorlib]System.Nullable`1<bool> V_0,
             bool V_1,
             valuetype [mscorlib]System.Nullable`1<bool> V_2,
             bool V_3,
             bool V_4,
             valuetype [mscorlib]System.Nullable`1<bool> V_5,
             bool V_6,
             valuetype [mscorlib]System.Nullable`1<bool> V_7,
             valuetype [mscorlib]System.Nullable`1<bool> V_8,
             bool V_9,
             valuetype [mscorlib]System.Nullable`1<bool> V_10,
             valuetype [mscorlib]System.Nullable`1<bool> V_11,
             valuetype [mscorlib]System.Nullable`1<bool> V_12,
             bool V_13,
             valuetype [mscorlib]System.Nullable`1<bool> V_14,
             valuetype [mscorlib]System.Nullable`1<bool> V_15,
             valuetype [mscorlib]System.Nullable`1<bool> V_16,
             bool V_17,
             valuetype [mscorlib]System.Nullable`1<bool> V_18,
             valuetype [mscorlib]System.Nullable`1<bool> V_19,
             valuetype [mscorlib]System.Nullable`1<bool> V_20,
             bool V_21,
             valuetype [mscorlib]System.Nullable`1<bool> V_22,
             valuetype [mscorlib]System.Nullable`1<bool> V_23,
             valuetype [mscorlib]System.Nullable`1<bool> V_24,
             valuetype [mscorlib]System.Nullable`1<bool> V_25)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0008:  stloc.1
    IL_0009:  ldloca.s   V_0
    IL_000b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0010:  ldloc.1
    IL_0011:  bne.un.s   IL_001c

    IL_0013:  ldloca.s   V_0
    IL_0015:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001a:  br.s       IL_001d

    IL_001c:  ldc.i4.0
    IL_001d:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0022:  ldarg.0
    IL_0023:  stloc.2
    IL_0024:  ldarg.1
    IL_0025:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_002a:  stloc.3
    IL_002b:  ldloca.s   V_2
    IL_002d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0032:  ldloc.3
    IL_0033:  bne.un.s   IL_0041

    IL_0035:  ldloca.s   V_2
    IL_0037:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_003c:  ldc.i4.0
    IL_003d:  ceq
    IL_003f:  br.s       IL_0042

    IL_0041:  ldc.i4.1
    IL_0042:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0047:  ldarg.1
    IL_0048:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_004d:  stloc.s    V_4
    IL_004f:  ldarg.0
    IL_0050:  stloc.s    V_5
    IL_0052:  ldloc.s    V_4
    IL_0054:  ldloca.s   V_5
    IL_0056:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_005b:  bne.un.s   IL_0066

    IL_005d:  ldloca.s   V_5
    IL_005f:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0064:  br.s       IL_0067

    IL_0066:  ldc.i4.0
    IL_0067:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_006c:  ldarg.1
    IL_006d:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0072:  stloc.s    V_6
    IL_0074:  ldarg.0
    IL_0075:  stloc.s    V_7
    IL_0077:  ldloc.s    V_6
    IL_0079:  ldloca.s   V_7
    IL_007b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0080:  bne.un.s   IL_008e

    IL_0082:  ldloca.s   V_7
    IL_0084:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0089:  ldc.i4.0
    IL_008a:  ceq
    IL_008c:  br.s       IL_008f

    IL_008e:  ldc.i4.1
    IL_008f:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0094:  ldarg.0
    IL_0095:  stloc.s    V_8
    IL_0097:  ldarg.1
    IL_0098:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_009d:  stloc.s    V_9
    IL_009f:  ldloca.s   V_8
    IL_00a1:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00a6:  brtrue.s   IL_00b4

    IL_00a8:  ldloca.s   V_10
    IL_00aa:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_00b0:  ldloc.s    V_10
    IL_00b2:  br.s       IL_00c3

    IL_00b4:  ldloca.s   V_8
    IL_00b6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00bb:  ldloc.s    V_9
    IL_00bd:  xor
    IL_00be:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_00c3:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_00c8:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00cd:  ldarg.0
    IL_00ce:  stloc.s    V_11
    IL_00d0:  ldloca.s   V_11
    IL_00d2:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00d7:  brtrue.s   IL_00e1

    IL_00d9:  ldarg.1
    IL_00da:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_00df:  br.s       IL_00e8

    IL_00e1:  ldloca.s   V_11
    IL_00e3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00e8:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00ed:  ldarg.0
    IL_00ee:  stloc.s    V_12
    IL_00f0:  ldarg.1
    IL_00f1:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_00f6:  stloc.s    V_13
    IL_00f8:  ldloca.s   V_12
    IL_00fa:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00ff:  brtrue.s   IL_010d

    IL_0101:  ldloca.s   V_14
    IL_0103:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_0109:  ldloc.s    V_14
    IL_010b:  br.s       IL_011c

    IL_010d:  ldloca.s   V_12
    IL_010f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0114:  ldloc.s    V_13
    IL_0116:  xor
    IL_0117:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_011c:  starg.s    a
    IL_011e:  ldarg.1
    IL_011f:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0124:  ldarg.0
    IL_0125:  stloc.s    V_15
    IL_0127:  brtrue.s   IL_0131

    IL_0129:  ldc.i4.0
    IL_012a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_012f:  br.s       IL_0133

    IL_0131:  ldloc.s    V_15
    IL_0133:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0138:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_013d:  ldarg.1
    IL_013e:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0143:  ldarg.0
    IL_0144:  stloc.s    V_16
    IL_0146:  brtrue.s   IL_014c

    IL_0148:  ldloc.s    V_16
    IL_014a:  br.s       IL_0152

    IL_014c:  ldc.i4.1
    IL_014d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0152:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0157:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_015c:  ldarg.1
    IL_015d:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0162:  stloc.s    V_17
    IL_0164:  ldarg.0
    IL_0165:  stloc.s    V_18
    IL_0167:  ldloca.s   V_18
    IL_0169:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_016e:  brtrue.s   IL_017c

    IL_0170:  ldloca.s   V_19
    IL_0172:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_0178:  ldloc.s    V_19
    IL_017a:  br.s       IL_018b

    IL_017c:  ldloc.s    V_17
    IL_017e:  ldloca.s   V_18
    IL_0180:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0185:  xor
    IL_0186:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_018b:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0190:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0195:  ldc.i4.0
    IL_0196:  newarr     valuetype [mscorlib]System.Nullable`1<bool>
    IL_019b:  ldc.i4.0
    IL_019c:  ldelema    valuetype [mscorlib]System.Nullable`1<bool>
    IL_01a1:  dup
    IL_01a2:  ldobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_01a7:  stloc.s    V_20
    IL_01a9:  ldarg.1
    IL_01aa:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_01af:  stloc.s    V_21
    IL_01b1:  ldloca.s   V_20
    IL_01b3:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_01b8:  brtrue.s   IL_01c6

    IL_01ba:  ldloca.s   V_22
    IL_01bc:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_01c2:  ldloc.s    V_22
    IL_01c4:  br.s       IL_01d5

    IL_01c6:  ldloca.s   V_20
    IL_01c8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01cd:  ldloc.s    V_21
    IL_01cf:  xor
    IL_01d0:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_01d5:  stobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_01da:  ldc.i4.0
    IL_01db:  newarr     valuetype [mscorlib]System.Nullable`1<bool>
    IL_01e0:  ldc.i4.0
    IL_01e1:  ldelema    valuetype [mscorlib]System.Nullable`1<bool>
    IL_01e6:  dup
    IL_01e7:  ldobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_01ec:  stloc.s    V_23
    IL_01ee:  ldarg.0
    IL_01ef:  stloc.s    V_24
    IL_01f1:  ldloca.s   V_23
    IL_01f3:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_01f8:  ldloca.s   V_24
    IL_01fa:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_01ff:  and
    IL_0200:  brtrue.s   IL_020e

    IL_0202:  ldloca.s   V_25
    IL_0204:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_020a:  ldloc.s    V_25
    IL_020c:  br.s       IL_0222

    IL_020e:  ldloca.s   V_23
    IL_0210:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0215:  ldloca.s   V_24
    IL_0217:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_021c:  xor
    IL_021d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0222:  stobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_0227:  ret
  } // end of method LiftedOperators::BoolValueComplex

  .method public hidebysig static void  BoolValueConst(valuetype [mscorlib]System.Nullable`1<bool> a) cil managed
  {
    // Code size       165 (0xa5)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<bool> V_0,
             valuetype [mscorlib]System.Nullable`1<bool> V_1,
             valuetype [mscorlib]System.Nullable`1<bool> V_2,
             valuetype [mscorlib]System.Nullable`1<bool> V_3,
             valuetype [mscorlib]System.Nullable`1<bool> V_4,
             valuetype [mscorlib]System.Nullable`1<bool> V_5)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0009:  brfalse.s  IL_0014

    IL_000b:  ldloca.s   V_0
    IL_000d:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0012:  br.s       IL_0015

    IL_0014:  ldc.i4.0
    IL_0015:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_001a:  ldarg.0
    IL_001b:  stloc.1
    IL_001c:  ldloca.s   V_1
    IL_001e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0023:  brfalse.s  IL_0031

    IL_0025:  ldloca.s   V_1
    IL_0027:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_002c:  ldc.i4.0
    IL_002d:  ceq
    IL_002f:  br.s       IL_0032

    IL_0031:  ldc.i4.1
    IL_0032:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0037:  ldarg.0
    IL_0038:  stloc.2
    IL_0039:  ldloca.s   V_2
    IL_003b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0040:  brtrue.s   IL_004b

    IL_0042:  ldloca.s   V_2
    IL_0044:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0049:  br.s       IL_004c

    IL_004b:  ldc.i4.0
    IL_004c:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0051:  ldarg.0
    IL_0052:  stloc.3
    IL_0053:  ldloca.s   V_3
    IL_0055:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_005a:  brtrue.s   IL_0068

    IL_005c:  ldloca.s   V_3
    IL_005e:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0063:  ldc.i4.0
    IL_0064:  ceq
    IL_0066:  br.s       IL_0069

    IL_0068:  ldc.i4.1
    IL_0069:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_006e:  ldarg.0
    IL_006f:  stloc.s    V_4
    IL_0071:  ldloca.s   V_4
    IL_0073:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0078:  brtrue.s   IL_007d

    IL_007a:  ldc.i4.1
    IL_007b:  br.s       IL_0084

    IL_007d:  ldloca.s   V_4
    IL_007f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0084:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0089:  ldarg.0
    IL_008a:  stloc.s    V_5
    IL_008c:  ldloca.s   V_5
    IL_008e:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0093:  brtrue.s   IL_0098

    IL_0095:  ldc.i4.0
    IL_0096:  br.s       IL_009f

    IL_0098:  ldloca.s   V_5
    IL_009a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_009f:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00a4:  ret
  } // end of method LiftedOperators::BoolValueConst

  .method public hidebysig static void  IntBasic(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                 valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       378 (0x17a)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             valuetype [mscorlib]System.Nullable`1<int32> V_3,
             valuetype [mscorlib]System.Nullable`1<int32> V_4,
             valuetype [mscorlib]System.Nullable`1<int32> V_5,
             valuetype [mscorlib]System.Nullable`1<int32> V_6,
             valuetype [mscorlib]System.Nullable`1<int32> V_7,
             valuetype [mscorlib]System.Nullable`1<int32> V_8,
             valuetype [mscorlib]System.Nullable`1<int32> V_9,
             valuetype [mscorlib]System.Nullable`1<int32> V_10,
             valuetype [mscorlib]System.Nullable`1<int32> V_11,
             valuetype [mscorlib]System.Nullable`1<int32> V_12,
             valuetype [mscorlib]System.Nullable`1<int32> V_13,
             valuetype [mscorlib]System.Nullable`1<int32> V_14,
             valuetype [mscorlib]System.Nullable`1<int32> V_15)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000b:  ldloca.s   V_1
    IL_000d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0012:  bne.un.s   IL_0026

    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001b:  ldloca.s   V_1
    IL_001d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0022:  ceq
    IL_0024:  br.s       IL_0027

    IL_0026:  ldc.i4.0
    IL_0027:  brfalse.s  IL_002e

    IL_0029:  call       void [mscorlib]System.Console::WriteLine()
    IL_002e:  ldarg.0
    IL_002f:  stloc.2
    IL_0030:  ldarg.1
    IL_0031:  stloc.3
    IL_0032:  ldloca.s   V_2
    IL_0034:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0039:  ldloca.s   V_3
    IL_003b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0040:  bne.un.s   IL_0057

    IL_0042:  ldloca.s   V_2
    IL_0044:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0049:  ldloca.s   V_3
    IL_004b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0050:  ceq
    IL_0052:  ldc.i4.0
    IL_0053:  ceq
    IL_0055:  br.s       IL_0058

    IL_0057:  ldc.i4.1
    IL_0058:  brfalse.s  IL_005f

    IL_005a:  call       void [mscorlib]System.Console::WriteLine()
    IL_005f:  ldarg.0
    IL_0060:  stloc.s    V_4
    IL_0062:  ldarg.1
    IL_0063:  stloc.s    V_5
    IL_0065:  ldloca.s   V_4
    IL_0067:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_006c:  ldloca.s   V_5
    IL_006e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0073:  ble.s      IL_0086

    IL_0075:  ldloca.s   V_4
    IL_0077:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_007c:  ldloca.s   V_5
    IL_007e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0083:  and
    IL_0084:  br.s       IL_0087

    IL_0086:  ldc.i4.0
    IL_0087:  brfalse.s  IL_008e

    IL_0089:  call       void [mscorlib]System.Console::WriteLine()
    IL_008e:  ldarg.0
    IL_008f:  stloc.s    V_6
    IL_0091:  ldarg.1
    IL_0092:  stloc.s    V_7
    IL_0094:  ldloca.s   V_6
    IL_0096:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_009b:  ldloca.s   V_7
    IL_009d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00a2:  bge.s      IL_00b5

    IL_00a4:  ldloca.s   V_6
    IL_00a6:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ab:  ldloca.s   V_7
    IL_00ad:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00b2:  and
    IL_00b3:  br.s       IL_00b6

    IL_00b5:  ldc.i4.0
    IL_00b6:  brfalse.s  IL_00bd

    IL_00b8:  call       void [mscorlib]System.Console::WriteLine()
    IL_00bd:  ldarg.0
    IL_00be:  stloc.s    V_8
    IL_00c0:  ldarg.1
    IL_00c1:  stloc.s    V_9
    IL_00c3:  ldloca.s   V_8
    IL_00c5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00ca:  ldloca.s   V_9
    IL_00cc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00d1:  blt.s      IL_00e4

    IL_00d3:  ldloca.s   V_8
    IL_00d5:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00da:  ldloca.s   V_9
    IL_00dc:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00e1:  and
    IL_00e2:  br.s       IL_00e5

    IL_00e4:  ldc.i4.0
    IL_00e5:  brfalse.s  IL_00ec

    IL_00e7:  call       void [mscorlib]System.Console::WriteLine()
    IL_00ec:  ldarg.0
    IL_00ed:  stloc.s    V_10
    IL_00ef:  ldarg.1
    IL_00f0:  stloc.s    V_11
    IL_00f2:  ldloca.s   V_10
    IL_00f4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00f9:  ldloca.s   V_11
    IL_00fb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0100:  bgt.s      IL_0113

    IL_0102:  ldloca.s   V_10
    IL_0104:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0109:  ldloca.s   V_11
    IL_010b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0110:  and
    IL_0111:  br.s       IL_0114

    IL_0113:  ldc.i4.0
    IL_0114:  brfalse.s  IL_011b

    IL_0116:  call       void [mscorlib]System.Console::WriteLine()
    IL_011b:  ldarg.0
    IL_011c:  stloc.s    V_12
    IL_011e:  ldarg.1
    IL_011f:  stloc.s    V_13
    IL_0121:  ldloca.s   V_12
    IL_0123:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0128:  ldloca.s   V_13
    IL_012a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_012f:  ble.s      IL_0142

    IL_0131:  ldloca.s   V_12
    IL_0133:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0138:  ldloca.s   V_13
    IL_013a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_013f:  and
    IL_0140:  br.s       IL_0143

    IL_0142:  ldc.i4.0
    IL_0143:  brtrue.s   IL_014a

    IL_0145:  call       void [mscorlib]System.Console::WriteLine()
    IL_014a:  ldarg.0
    IL_014b:  stloc.s    V_14
    IL_014d:  ldarg.1
    IL_014e:  stloc.s    V_15
    IL_0150:  ldloca.s   V_14
    IL_0152:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0157:  ldloca.s   V_15
    IL_0159:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_015e:  bgt.s      IL_0171

    IL_0160:  ldloca.s   V_14
    IL_0162:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0167:  ldloca.s   V_15
    IL_0169:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_016e:  and
    IL_016f:  br.s       IL_0172

    IL_0171:  ldc.i4.0
    IL_0172:  brtrue.s   IL_0179

    IL_0174:  call       void [mscorlib]System.Console::WriteLine()
    IL_0179:  ret
  } // end of method LiftedOperators::IntBasic

  .method public hidebysig static void  IntComplex(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                   class [mscorlib]System.Func`1<int32> x) cil managed
  {
    // Code size       313 (0x139)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             int32 V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             int32 V_3,
             valuetype [mscorlib]System.Nullable`1<int32> V_4,
             int32 V_5,
             int32 V_6,
             valuetype [mscorlib]System.Nullable`1<int32> V_7,
             int32 V_8,
             valuetype [mscorlib]System.Nullable`1<int32> V_9,
             int32 V_10,
             valuetype [mscorlib]System.Nullable`1<int32> V_11,
             valuetype [mscorlib]System.Nullable`1<int32> V_12,
             int32 V_13,
             valuetype [mscorlib]System.Nullable`1<int32> V_14,
             int32 V_15)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0008:  stloc.1
    IL_0009:  ldloca.s   V_0
    IL_000b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0010:  ldloc.1
    IL_0011:  bne.un.s   IL_001c

    IL_0013:  ldloca.s   V_0
    IL_0015:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001a:  br.s       IL_001d

    IL_001c:  ldc.i4.0
    IL_001d:  brfalse.s  IL_0024

    IL_001f:  call       void [mscorlib]System.Console::WriteLine()
    IL_0024:  ldarg.0
    IL_0025:  stloc.2
    IL_0026:  ldarg.1
    IL_0027:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_002c:  stloc.3
    IL_002d:  ldloca.s   V_2
    IL_002f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0034:  ldloc.3
    IL_0035:  bne.un.s   IL_0043

    IL_0037:  ldloca.s   V_2
    IL_0039:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_003e:  ldc.i4.0
    IL_003f:  ceq
    IL_0041:  br.s       IL_0044

    IL_0043:  ldc.i4.1
    IL_0044:  brfalse.s  IL_004b

    IL_0046:  call       void [mscorlib]System.Console::WriteLine()
    IL_004b:  ldarg.0
    IL_004c:  stloc.s    V_4
    IL_004e:  ldarg.1
    IL_004f:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0054:  stloc.s    V_5
    IL_0056:  ldloca.s   V_4
    IL_0058:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_005d:  ldloc.s    V_5
    IL_005f:  ble.s      IL_006a

    IL_0061:  ldloca.s   V_4
    IL_0063:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0068:  br.s       IL_006b

    IL_006a:  ldc.i4.0
    IL_006b:  brfalse.s  IL_0072

    IL_006d:  call       void [mscorlib]System.Console::WriteLine()
    IL_0072:  ldarg.1
    IL_0073:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0078:  stloc.s    V_6
    IL_007a:  ldarg.0
    IL_007b:  stloc.s    V_7
    IL_007d:  ldloc.s    V_6
    IL_007f:  ldloca.s   V_7
    IL_0081:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0086:  bne.un.s   IL_0091

    IL_0088:  ldloca.s   V_7
    IL_008a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_008f:  br.s       IL_0092

    IL_0091:  ldc.i4.0
    IL_0092:  brfalse.s  IL_0099

    IL_0094:  call       void [mscorlib]System.Console::WriteLine()
    IL_0099:  ldarg.1
    IL_009a:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_009f:  stloc.s    V_8
    IL_00a1:  ldarg.0
    IL_00a2:  stloc.s    V_9
    IL_00a4:  ldloc.s    V_8
    IL_00a6:  ldloca.s   V_9
    IL_00a8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00ad:  bne.un.s   IL_00bb

    IL_00af:  ldloca.s   V_9
    IL_00b1:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00b6:  ldc.i4.0
    IL_00b7:  ceq
    IL_00b9:  br.s       IL_00bc

    IL_00bb:  ldc.i4.1
    IL_00bc:  brfalse.s  IL_00c3

    IL_00be:  call       void [mscorlib]System.Console::WriteLine()
    IL_00c3:  ldarg.1
    IL_00c4:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00c9:  stloc.s    V_10
    IL_00cb:  ldarg.0
    IL_00cc:  stloc.s    V_11
    IL_00ce:  ldloc.s    V_10
    IL_00d0:  ldloca.s   V_11
    IL_00d2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00d7:  ble.s      IL_00e2

    IL_00d9:  ldloca.s   V_11
    IL_00db:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00e0:  br.s       IL_00e3

    IL_00e2:  ldc.i4.0
    IL_00e3:  brfalse.s  IL_00ea

    IL_00e5:  call       void [mscorlib]System.Console::WriteLine()
    IL_00ea:  ldarg.0
    IL_00eb:  stloc.s    V_12
    IL_00ed:  ldarg.1
    IL_00ee:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00f3:  stloc.s    V_13
    IL_00f5:  ldloca.s   V_12
    IL_00f7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00fc:  ldloc.s    V_13
    IL_00fe:  ble.s      IL_0109

    IL_0100:  ldloca.s   V_12
    IL_0102:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0107:  br.s       IL_010a

    IL_0109:  ldc.i4.0
    IL_010a:  brtrue.s   IL_0111

    IL_010c:  call       void [mscorlib]System.Console::WriteLine()
    IL_0111:  ldarg.0
    IL_0112:  stloc.s    V_14
    IL_0114:  ldarg.1
    IL_0115:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_011a:  stloc.s    V_15
    IL_011c:  ldloca.s   V_14
    IL_011e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0123:  ldloc.s    V_15
    IL_0125:  bgt.s      IL_0130

    IL_0127:  ldloca.s   V_14
    IL_0129:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_012e:  br.s       IL_0131

    IL_0130:  ldc.i4.0
    IL_0131:  brtrue.s   IL_0138

    IL_0133:  call       void [mscorlib]System.Console::WriteLine()
    IL_0138:  ret
  } // end of method LiftedOperators::IntComplex

  .method public hidebysig static void  IntConst(valuetype [mscorlib]System.Nullable`1<int32> a) cil managed
  {
    // Code size       183 (0xb7)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             valuetype [mscorlib]System.Nullable`1<int32> V_3,
             valuetype [mscorlib]System.Nullable`1<int32> V_4,
             valuetype [mscorlib]System.Nullable`1<int32> V_5)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0009:  ldc.i4.2
    IL_000a:  bne.un.s   IL_0015

    IL_000c:  ldloca.s   V_0
    IL_000e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0013:  br.s       IL_0016

    IL_0015:  ldc.i4.0
    IL_0016:  brfalse.s  IL_001d

    IL_0018:  call       void [mscorlib]System.Console::WriteLine()
    IL_001d:  ldarg.0
    IL_001e:  stloc.1
    IL_001f:  ldloca.s   V_1
    IL_0021:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0026:  ldc.i4.2
    IL_0027:  bne.un.s   IL_0035

    IL_0029:  ldloca.s   V_1
    IL_002b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0030:  ldc.i4.0
    IL_0031:  ceq
    IL_0033:  br.s       IL_0036

    IL_0035:  ldc.i4.1
    IL_0036:  brfalse.s  IL_003d

    IL_0038:  call       void [mscorlib]System.Console::WriteLine()
    IL_003d:  ldarg.0
    IL_003e:  stloc.2
    IL_003f:  ldloca.s   V_2
    IL_0041:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0046:  ldc.i4.2
    IL_0047:  ble.s      IL_0052

    IL_0049:  ldloca.s   V_2
    IL_004b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0050:  br.s       IL_0053

    IL_0052:  ldc.i4.0
    IL_0053:  brfalse.s  IL_005a

    IL_0055:  call       void [mscorlib]System.Console::WriteLine()
    IL_005a:  ldarg.0
    IL_005b:  stloc.3
    IL_005c:  ldc.i4.2
    IL_005d:  ldloca.s   V_3
    IL_005f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0064:  bne.un.s   IL_006f

    IL_0066:  ldloca.s   V_3
    IL_0068:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_006d:  br.s       IL_0070

    IL_006f:  ldc.i4.0
    IL_0070:  brfalse.s  IL_0077

    IL_0072:  call       void [mscorlib]System.Console::WriteLine()
    IL_0077:  ldarg.0
    IL_0078:  stloc.s    V_4
    IL_007a:  ldc.i4.2
    IL_007b:  ldloca.s   V_4
    IL_007d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0082:  bne.un.s   IL_0090

    IL_0084:  ldloca.s   V_4
    IL_0086:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_008b:  ldc.i4.0
    IL_008c:  ceq
    IL_008e:  br.s       IL_0091

    IL_0090:  ldc.i4.1
    IL_0091:  brfalse.s  IL_0098

    IL_0093:  call       void [mscorlib]System.Console::WriteLine()
    IL_0098:  ldarg.0
    IL_0099:  stloc.s    V_5
    IL_009b:  ldc.i4.2
    IL_009c:  ldloca.s   V_5
    IL_009e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00a3:  ble.s      IL_00ae

    IL_00a5:  ldloca.s   V_5
    IL_00a7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ac:  br.s       IL_00af

    IL_00ae:  ldc.i4.0
    IL_00af:  brfalse.s  IL_00b6

    IL_00b1:  call       void [mscorlib]System.Console::WriteLine()
    IL_00b6:  ret
  } // end of method LiftedOperators::IntConst

  .method public hidebysig static void  IntValueBasic(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                      valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       1680 (0x690)
    .maxstack  3
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             valuetype [mscorlib]System.Nullable`1<int32> V_3,
             valuetype [mscorlib]System.Nullable`1<int32> V_4,
             valuetype [mscorlib]System.Nullable`1<int32> V_5,
             valuetype [mscorlib]System.Nullable`1<int32> V_6,
             valuetype [mscorlib]System.Nullable`1<int32> V_7,
             valuetype [mscorlib]System.Nullable`1<int32> V_8,
             valuetype [mscorlib]System.Nullable`1<int32> V_9,
             valuetype [mscorlib]System.Nullable`1<int32> V_10,
             valuetype [mscorlib]System.Nullable`1<int32> V_11,
             valuetype [mscorlib]System.Nullable`1<int32> V_12,
             valuetype [mscorlib]System.Nullable`1<int32> V_13,
             valuetype [mscorlib]System.Nullable`1<int32> V_14,
             valuetype [mscorlib]System.Nullable`1<int32> V_15,
             valuetype [mscorlib]System.Nullable`1<int32> V_16,
             valuetype [mscorlib]System.Nullable`1<int32> V_17,
             valuetype [mscorlib]System.Nullable`1<int32> V_18,
             valuetype [mscorlib]System.Nullable`1<int32> V_19,
             valuetype [mscorlib]System.Nullable`1<int32> V_20,
             valuetype [mscorlib]System.Nullable`1<int32> V_21,
             valuetype [mscorlib]System.Nullable`1<int32> V_22,
             valuetype [mscorlib]System.Nullable`1<int32> V_23,
             valuetype [mscorlib]System.Nullable`1<int32> V_24,
             valuetype [mscorlib]System.Nullable`1<int32> V_25,
             valuetype [mscorlib]System.Nullable`1<int32> V_26,
             valuetype [mscorlib]System.Nullable`1<int32> V_27,
             valuetype [mscorlib]System.Nullable`1<int32> V_28,
             valuetype [mscorlib]System.Nullable`1<int32> V_29,
             valuetype [mscorlib]System.Nullable`1<int32> V_30,
             valuetype [mscorlib]System.Nullable`1<int32> V_31,
             valuetype [mscorlib]System.Nullable`1<int32> V_32,
             valuetype [mscorlib]System.Nullable`1<int32> V_33,
             valuetype [mscorlib]System.Nullable`1<int32> V_34,
             valuetype [mscorlib]System.Nullable`1<int32> V_35,
             valuetype [mscorlib]System.Nullable`1<int32> V_36,
             valuetype [mscorlib]System.Nullable`1<int32> V_37,
             valuetype [mscorlib]System.Nullable`1<int32> V_38,
             valuetype [mscorlib]System.Nullable`1<int32> V_39,
             valuetype [mscorlib]System.Nullable`1<int32> V_40,
             valuetype [mscorlib]System.Nullable`1<int32> V_41,
             valuetype [mscorlib]System.Nullable`1<int32> V_42,
             valuetype [mscorlib]System.Nullable`1<int32> V_43,
             valuetype [mscorlib]System.Nullable`1<int32> V_44,
             valuetype [mscorlib]System.Nullable`1<int32> V_45,
             valuetype [mscorlib]System.Nullable`1<int32> V_46,
             valuetype [mscorlib]System.Nullable`1<int32> V_47,
             valuetype [mscorlib]System.Nullable`1<int32> V_48,
             valuetype [mscorlib]System.Nullable`1<int32> V_49,
             valuetype [mscorlib]System.Nullable`1<int32> V_50,
             valuetype [mscorlib]System.Nullable`1<int32> V_51,
             valuetype [mscorlib]System.Nullable`1<int32> V_52,
             valuetype [mscorlib]System.Nullable`1<int32> V_53,
             valuetype [mscorlib]System.Nullable`1<int32> V_54,
             valuetype [mscorlib]System.Nullable`1<int32> V_55,
             valuetype [mscorlib]System.Nullable`1<int32> V_56,
             valuetype [mscorlib]System.Nullable`1<int32> V_57,
             valuetype [mscorlib]System.Nullable`1<int32> V_58,
             valuetype [mscorlib]System.Nullable`1<int32> V_59,
             valuetype [mscorlib]System.Nullable`1<int32> V_60)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000b:  ldloca.s   V_1
    IL_000d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0012:  bne.un.s   IL_0026

    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001b:  ldloca.s   V_1
    IL_001d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0022:  ceq
    IL_0024:  br.s       IL_0027

    IL_0026:  ldc.i4.0
    IL_0027:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_002c:  ldarg.0
    IL_002d:  stloc.2
    IL_002e:  ldarg.1
    IL_002f:  stloc.3
    IL_0030:  ldloca.s   V_2
    IL_0032:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0037:  ldloca.s   V_3
    IL_0039:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_003e:  bne.un.s   IL_0055

    IL_0040:  ldloca.s   V_2
    IL_0042:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0047:  ldloca.s   V_3
    IL_0049:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_004e:  ceq
    IL_0050:  ldc.i4.0
    IL_0051:  ceq
    IL_0053:  br.s       IL_0056

    IL_0055:  ldc.i4.1
    IL_0056:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_005b:  ldarg.0
    IL_005c:  stloc.s    V_4
    IL_005e:  ldarg.1
    IL_005f:  stloc.s    V_5
    IL_0061:  ldloca.s   V_4
    IL_0063:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0068:  ldloca.s   V_5
    IL_006a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_006f:  ble.s      IL_0082

    IL_0071:  ldloca.s   V_4
    IL_0073:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0078:  ldloca.s   V_5
    IL_007a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_007f:  and
    IL_0080:  br.s       IL_0083

    IL_0082:  ldc.i4.0
    IL_0083:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0088:  ldarg.0
    IL_0089:  stloc.s    V_6
    IL_008b:  ldarg.1
    IL_008c:  stloc.s    V_7
    IL_008e:  ldloca.s   V_6
    IL_0090:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0095:  ldloca.s   V_7
    IL_0097:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_009c:  ble.s      IL_00af

    IL_009e:  ldloca.s   V_6
    IL_00a0:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00a5:  ldloca.s   V_7
    IL_00a7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ac:  and
    IL_00ad:  br.s       IL_00b0

    IL_00af:  ldc.i4.0
    IL_00b0:  ldc.i4.0
    IL_00b1:  ceq
    IL_00b3:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00b8:  ldarg.0
    IL_00b9:  stloc.s    V_8
    IL_00bb:  ldarg.1
    IL_00bc:  stloc.s    V_9
    IL_00be:  ldloca.s   V_8
    IL_00c0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00c5:  ldloca.s   V_9
    IL_00c7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00cc:  blt.s      IL_00df

    IL_00ce:  ldloca.s   V_8
    IL_00d0:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00d5:  ldloca.s   V_9
    IL_00d7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00dc:  and
    IL_00dd:  br.s       IL_00e0

    IL_00df:  ldc.i4.0
    IL_00e0:  ldc.i4.0
    IL_00e1:  ceq
    IL_00e3:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00e8:  ldarg.0
    IL_00e9:  stloc.s    V_10
    IL_00eb:  ldarg.1
    IL_00ec:  stloc.s    V_11
    IL_00ee:  ldloca.s   V_10
    IL_00f0:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00f5:  ldloca.s   V_11
    IL_00f7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00fc:  and
    IL_00fd:  brtrue.s   IL_010b

    IL_00ff:  ldloca.s   V_12
    IL_0101:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0107:  ldloc.s    V_12
    IL_0109:  br.s       IL_011f

    IL_010b:  ldloca.s   V_10
    IL_010d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0112:  ldloca.s   V_11
    IL_0114:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0119:  add
    IL_011a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_011f:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0124:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0129:  ldarg.0
    IL_012a:  stloc.s    V_13
    IL_012c:  ldarg.1
    IL_012d:  stloc.s    V_14
    IL_012f:  ldloca.s   V_13
    IL_0131:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0136:  ldloca.s   V_14
    IL_0138:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_013d:  and
    IL_013e:  brtrue.s   IL_014c

    IL_0140:  ldloca.s   V_15
    IL_0142:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0148:  ldloc.s    V_15
    IL_014a:  br.s       IL_0160

    IL_014c:  ldloca.s   V_13
    IL_014e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0153:  ldloca.s   V_14
    IL_0155:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_015a:  sub
    IL_015b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0160:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0165:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_016a:  ldarg.0
    IL_016b:  stloc.s    V_16
    IL_016d:  ldarg.1
    IL_016e:  stloc.s    V_17
    IL_0170:  ldloca.s   V_16
    IL_0172:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0177:  ldloca.s   V_17
    IL_0179:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_017e:  and
    IL_017f:  brtrue.s   IL_018d

    IL_0181:  ldloca.s   V_18
    IL_0183:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0189:  ldloc.s    V_18
    IL_018b:  br.s       IL_01a1

    IL_018d:  ldloca.s   V_16
    IL_018f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0194:  ldloca.s   V_17
    IL_0196:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_019b:  mul
    IL_019c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01a1:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01a6:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01ab:  ldarg.0
    IL_01ac:  stloc.s    V_19
    IL_01ae:  ldarg.1
    IL_01af:  stloc.s    V_20
    IL_01b1:  ldloca.s   V_19
    IL_01b3:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01b8:  ldloca.s   V_20
    IL_01ba:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01bf:  and
    IL_01c0:  brtrue.s   IL_01ce

    IL_01c2:  ldloca.s   V_21
    IL_01c4:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01ca:  ldloc.s    V_21
    IL_01cc:  br.s       IL_01e2

    IL_01ce:  ldloca.s   V_19
    IL_01d0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01d5:  ldloca.s   V_20
    IL_01d7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01dc:  div
    IL_01dd:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01e2:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01e7:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01ec:  ldarg.0
    IL_01ed:  stloc.s    V_22
    IL_01ef:  ldarg.1
    IL_01f0:  stloc.s    V_23
    IL_01f2:  ldloca.s   V_22
    IL_01f4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01f9:  ldloca.s   V_23
    IL_01fb:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0200:  and
    IL_0201:  brtrue.s   IL_020f

    IL_0203:  ldloca.s   V_24
    IL_0205:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_020b:  ldloc.s    V_24
    IL_020d:  br.s       IL_0223

    IL_020f:  ldloca.s   V_22
    IL_0211:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0216:  ldloca.s   V_23
    IL_0218:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_021d:  rem
    IL_021e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0223:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0228:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_022d:  ldarg.0
    IL_022e:  stloc.s    V_25
    IL_0230:  ldarg.1
    IL_0231:  stloc.s    V_26
    IL_0233:  ldloca.s   V_25
    IL_0235:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_023a:  ldloca.s   V_26
    IL_023c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0241:  and
    IL_0242:  brtrue.s   IL_0250

    IL_0244:  ldloca.s   V_27
    IL_0246:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_024c:  ldloc.s    V_27
    IL_024e:  br.s       IL_0264

    IL_0250:  ldloca.s   V_25
    IL_0252:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0257:  ldloca.s   V_26
    IL_0259:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_025e:  and
    IL_025f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0264:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0269:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_026e:  ldarg.0
    IL_026f:  stloc.s    V_28
    IL_0271:  ldarg.1
    IL_0272:  stloc.s    V_29
    IL_0274:  ldloca.s   V_28
    IL_0276:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_027b:  ldloca.s   V_29
    IL_027d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0282:  and
    IL_0283:  brtrue.s   IL_0291

    IL_0285:  ldloca.s   V_30
    IL_0287:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_028d:  ldloc.s    V_30
    IL_028f:  br.s       IL_02a5

    IL_0291:  ldloca.s   V_28
    IL_0293:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0298:  ldloca.s   V_29
    IL_029a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_029f:  or
    IL_02a0:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02a5:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_02aa:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02af:  ldarg.0
    IL_02b0:  stloc.s    V_31
    IL_02b2:  ldarg.1
    IL_02b3:  stloc.s    V_32
    IL_02b5:  ldloca.s   V_31
    IL_02b7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02bc:  ldloca.s   V_32
    IL_02be:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02c3:  and
    IL_02c4:  brtrue.s   IL_02d2

    IL_02c6:  ldloca.s   V_33
    IL_02c8:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02ce:  ldloc.s    V_33
    IL_02d0:  br.s       IL_02e6

    IL_02d2:  ldloca.s   V_31
    IL_02d4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02d9:  ldloca.s   V_32
    IL_02db:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02e0:  xor
    IL_02e1:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02e6:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_02eb:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02f0:  ldarg.0
    IL_02f1:  stloc.s    V_34
    IL_02f3:  ldarg.1
    IL_02f4:  stloc.s    V_35
    IL_02f6:  ldloca.s   V_34
    IL_02f8:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02fd:  ldloca.s   V_35
    IL_02ff:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0304:  and
    IL_0305:  brtrue.s   IL_0313

    IL_0307:  ldloca.s   V_36
    IL_0309:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_030f:  ldloc.s    V_36
    IL_0311:  br.s       IL_032a

    IL_0313:  ldloca.s   V_34
    IL_0315:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_031a:  ldloca.s   V_35
    IL_031c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0321:  ldc.i4.s   31
    IL_0323:  and
    IL_0324:  shl
    IL_0325:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_032a:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_032f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0334:  ldarg.0
    IL_0335:  stloc.s    V_37
    IL_0337:  ldarg.1
    IL_0338:  stloc.s    V_38
    IL_033a:  ldloca.s   V_37
    IL_033c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0341:  ldloca.s   V_38
    IL_0343:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0348:  and
    IL_0349:  brtrue.s   IL_0357

    IL_034b:  ldloca.s   V_39
    IL_034d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0353:  ldloc.s    V_39
    IL_0355:  br.s       IL_036e

    IL_0357:  ldloca.s   V_37
    IL_0359:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_035e:  ldloca.s   V_38
    IL_0360:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0365:  ldc.i4.s   31
    IL_0367:  and
    IL_0368:  shr
    IL_0369:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_036e:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0373:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0378:  ldarg.0
    IL_0379:  stloc.s    V_40
    IL_037b:  ldloca.s   V_40
    IL_037d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0382:  brtrue.s   IL_0387

    IL_0384:  ldarg.1
    IL_0385:  br.s       IL_0393

    IL_0387:  ldloca.s   V_40
    IL_0389:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_038e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0393:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0398:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_039d:  ldarg.0
    IL_039e:  stloc.s    V_41
    IL_03a0:  ldloca.s   V_41
    IL_03a2:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03a7:  brtrue.s   IL_03b5

    IL_03a9:  ldloca.s   V_42
    IL_03ab:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03b1:  ldloc.s    V_42
    IL_03b3:  br.s       IL_03c2

    IL_03b5:  ldloca.s   V_41
    IL_03b7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03bc:  neg
    IL_03bd:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03c2:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_03c7:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_03cc:  ldarg.0
    IL_03cd:  stloc.s    V_43
    IL_03cf:  ldloca.s   V_43
    IL_03d1:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03d6:  brtrue.s   IL_03e4

    IL_03d8:  ldloca.s   V_44
    IL_03da:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03e0:  ldloc.s    V_44
    IL_03e2:  br.s       IL_03f1

    IL_03e4:  ldloca.s   V_43
    IL_03e6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03eb:  not
    IL_03ec:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03f1:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_03f6:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_03fb:  ldarg.0
    IL_03fc:  stloc.s    V_45
    IL_03fe:  ldloca.s   V_45
    IL_0400:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0405:  brtrue.s   IL_0413

    IL_0407:  ldloca.s   V_46
    IL_0409:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_040f:  ldloc.s    V_46
    IL_0411:  br.s       IL_0421

    IL_0413:  ldloca.s   V_45
    IL_0415:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_041a:  ldc.i4.1
    IL_041b:  add
    IL_041c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0421:  dup
    IL_0422:  starg.s    a
    IL_0424:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0429:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_042e:  ldarg.0
    IL_042f:  stloc.s    V_47
    IL_0431:  ldloca.s   V_47
    IL_0433:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0438:  brtrue.s   IL_0446

    IL_043a:  ldloca.s   V_48
    IL_043c:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0442:  ldloc.s    V_48
    IL_0444:  br.s       IL_0454

    IL_0446:  ldloca.s   V_47
    IL_0448:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_044d:  ldc.i4.1
    IL_044e:  sub
    IL_044f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0454:  dup
    IL_0455:  starg.s    a
    IL_0457:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_045c:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0461:  ldarg.0
    IL_0462:  stloc.s    V_49
    IL_0464:  ldarg.1
    IL_0465:  stloc.s    V_50
    IL_0467:  ldloca.s   V_49
    IL_0469:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_046e:  ldloca.s   V_50
    IL_0470:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0475:  and
    IL_0476:  brtrue.s   IL_0484

    IL_0478:  ldloca.s   V_51
    IL_047a:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0480:  ldloc.s    V_51
    IL_0482:  br.s       IL_0498

    IL_0484:  ldloca.s   V_49
    IL_0486:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_048b:  ldloca.s   V_50
    IL_048d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0492:  add
    IL_0493:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0498:  starg.s    a
    IL_049a:  ldarg.0
    IL_049b:  stloc.s    V_52
    IL_049d:  ldarg.1
    IL_049e:  stloc.s    V_53
    IL_04a0:  ldloca.s   V_52
    IL_04a2:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04a7:  ldloca.s   V_53
    IL_04a9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04ae:  and
    IL_04af:  brtrue.s   IL_04bd

    IL_04b1:  ldloca.s   V_54
    IL_04b3:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04b9:  ldloc.s    V_54
    IL_04bb:  br.s       IL_04d1

    IL_04bd:  ldloca.s   V_52
    IL_04bf:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04c4:  ldloca.s   V_53
    IL_04c6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04cb:  sub
    IL_04cc:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04d1:  starg.s    a
    IL_04d3:  ldarg.0
    IL_04d4:  stloc.s    V_55
    IL_04d6:  ldarg.1
    IL_04d7:  stloc.s    V_56
    IL_04d9:  ldloca.s   V_55
    IL_04db:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04e0:  ldloca.s   V_56
    IL_04e2:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04e7:  and
    IL_04e8:  brtrue.s   IL_04f6

    IL_04ea:  ldloca.s   V_57
    IL_04ec:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04f2:  ldloc.s    V_57
    IL_04f4:  br.s       IL_050a

    IL_04f6:  ldloca.s   V_55
    IL_04f8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04fd:  ldloca.s   V_56
    IL_04ff:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0504:  mul
    IL_0505:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_050a:  starg.s    a
    IL_050c:  ldarg.0
    IL_050d:  stloc.s    V_58
    IL_050f:  ldarg.1
    IL_0510:  stloc.s    V_59
    IL_0512:  ldloca.s   V_58
    IL_0514:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0519:  ldloca.s   V_59
    IL_051b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0520:  and
    IL_0521:  brtrue.s   IL_052f

    IL_0523:  ldloca.s   V_60
    IL_0525:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_052b:  ldloc.s    V_60
    IL_052d:  br.s       IL_0543

    IL_052f:  ldloca.s   V_58
    IL_0531:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0536:  ldloca.s   V_59
    IL_0538:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_053d:  div
    IL_053e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0543:  starg.s    a
    IL_0545:  ldarg.0
    IL_0546:  stloc.0
    IL_0547:  ldarg.1
    IL_0548:  stloc.1
    IL_0549:  ldloca.s   V_0
    IL_054b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0550:  ldloca.s   V_1
    IL_0552:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0557:  and
    IL_0558:  brtrue.s   IL_0565

    IL_055a:  ldloca.s   V_2
    IL_055c:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0562:  ldloc.2
    IL_0563:  br.s       IL_0579

    IL_0565:  ldloca.s   V_0
    IL_0567:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_056c:  ldloca.s   V_1
    IL_056e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0573:  rem
    IL_0574:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0579:  starg.s    a
    IL_057b:  ldarg.0
    IL_057c:  stloc.0
    IL_057d:  ldarg.1
    IL_057e:  stloc.1
    IL_057f:  ldloca.s   V_0
    IL_0581:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0586:  ldloca.s   V_1
    IL_0588:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_058d:  and
    IL_058e:  brtrue.s   IL_059b

    IL_0590:  ldloca.s   V_2
    IL_0592:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0598:  ldloc.2
    IL_0599:  br.s       IL_05af

    IL_059b:  ldloca.s   V_0
    IL_059d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_05a2:  ldloca.s   V_1
    IL_05a4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_05a9:  and
    IL_05aa:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_05af:  starg.s    a
    IL_05b1:  ldarg.0
    IL_05b2:  stloc.0
    IL_05b3:  ldarg.1
    IL_05b4:  stloc.1
    IL_05b5:  ldloca.s   V_0
    IL_05b7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_05bc:  ldloca.s   V_1
    IL_05be:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_05c3:  and
    IL_05c4:  brtrue.s   IL_05d1

    IL_05c6:  ldloca.s   V_2
    IL_05c8:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_05ce:  ldloc.2
    IL_05cf:  br.s       IL_05e5

    IL_05d1:  ldloca.s   V_0
    IL_05d3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_05d8:  ldloca.s   V_1
    IL_05da:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_05df:  or
    IL_05e0:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_05e5:  starg.s    a
    IL_05e7:  ldarg.0
    IL_05e8:  stloc.0
    IL_05e9:  ldarg.1
    IL_05ea:  stloc.1
    IL_05eb:  ldloca.s   V_0
    IL_05ed:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_05f2:  ldloca.s   V_1
    IL_05f4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_05f9:  and
    IL_05fa:  brtrue.s   IL_0607

    IL_05fc:  ldloca.s   V_2
    IL_05fe:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0604:  ldloc.2
    IL_0605:  br.s       IL_061b

    IL_0607:  ldloca.s   V_0
    IL_0609:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_060e:  ldloca.s   V_1
    IL_0610:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0615:  xor
    IL_0616:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_061b:  starg.s    a
    IL_061d:  ldarg.0
    IL_061e:  stloc.0
    IL_061f:  ldarg.1
    IL_0620:  stloc.1
    IL_0621:  ldloca.s   V_0
    IL_0623:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0628:  ldloca.s   V_1
    IL_062a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_062f:  and
    IL_0630:  brtrue.s   IL_063d

    IL_0632:  ldloca.s   V_2
    IL_0634:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_063a:  ldloc.2
    IL_063b:  br.s       IL_0654

    IL_063d:  ldloca.s   V_0
    IL_063f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0644:  ldloca.s   V_1
    IL_0646:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_064b:  ldc.i4.s   31
    IL_064d:  and
    IL_064e:  shl
    IL_064f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0654:  starg.s    a
    IL_0656:  ldarg.0
    IL_0657:  stloc.0
    IL_0658:  ldarg.1
    IL_0659:  stloc.1
    IL_065a:  ldloca.s   V_0
    IL_065c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0661:  ldloca.s   V_1
    IL_0663:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0668:  and
    IL_0669:  brtrue.s   IL_0676

    IL_066b:  ldloca.s   V_2
    IL_066d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0673:  ldloc.2
    IL_0674:  br.s       IL_068d

    IL_0676:  ldloca.s   V_0
    IL_0678:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_067d:  ldloca.s   V_1
    IL_067f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0684:  ldc.i4.s   31
    IL_0686:  and
    IL_0687:  shr
    IL_0688:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_068d:  starg.s    a
    IL_068f:  ret
  } // end of method LiftedOperators::IntValueBasic

  .method public hidebysig static void  IntValueComplex(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                        class [mscorlib]System.Func`1<int32> x) cil managed
  {
    // Code size       1429 (0x595)
    .maxstack  3
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             int32 V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             int32 V_3,
             valuetype [mscorlib]System.Nullable`1<int32> V_4,
             int32 V_5,
             int32 V_6,
             valuetype [mscorlib]System.Nullable`1<int32> V_7,
             int32 V_8,
             valuetype [mscorlib]System.Nullable`1<int32> V_9,
             int32 V_10,
             valuetype [mscorlib]System.Nullable`1<int32> V_11,
             valuetype [mscorlib]System.Nullable`1<int32> V_12,
             int32 V_13,
             valuetype [mscorlib]System.Nullable`1<int32> V_14,
             valuetype [mscorlib]System.Nullable`1<int32> V_15,
             int32 V_16,
             valuetype [mscorlib]System.Nullable`1<int32> V_17,
             valuetype [mscorlib]System.Nullable`1<int32> V_18,
             int32 V_19,
             valuetype [mscorlib]System.Nullable`1<int32> V_20,
             valuetype [mscorlib]System.Nullable`1<int32> V_21,
             int32 V_22,
             valuetype [mscorlib]System.Nullable`1<int32> V_23,
             valuetype [mscorlib]System.Nullable`1<int32> V_24,
             int32 V_25,
             valuetype [mscorlib]System.Nullable`1<int32> V_26,
             valuetype [mscorlib]System.Nullable`1<int32> V_27,
             int32 V_28,
             valuetype [mscorlib]System.Nullable`1<int32> V_29,
             valuetype [mscorlib]System.Nullable`1<int32> V_30,
             int32 V_31,
             valuetype [mscorlib]System.Nullable`1<int32> V_32,
             valuetype [mscorlib]System.Nullable`1<int32> V_33,
             int32 V_34,
             valuetype [mscorlib]System.Nullable`1<int32> V_35,
             valuetype [mscorlib]System.Nullable`1<int32> V_36,
             int32 V_37,
             valuetype [mscorlib]System.Nullable`1<int32> V_38,
             valuetype [mscorlib]System.Nullable`1<int32> V_39,
             int32 V_40,
             valuetype [mscorlib]System.Nullable`1<int32> V_41,
             valuetype [mscorlib]System.Nullable`1<int32> V_42,
             valuetype [mscorlib]System.Nullable`1<int32> V_43,
             int32 V_44,
             valuetype [mscorlib]System.Nullable`1<int32> V_45,
             valuetype [mscorlib]System.Nullable`1<int32> V_46,
             int32 V_47,
             valuetype [mscorlib]System.Nullable`1<int32> V_48,
             valuetype [mscorlib]System.Nullable`1<int32> V_49,
             int32 V_50,
             valuetype [mscorlib]System.Nullable`1<int32> V_51,
             valuetype [mscorlib]System.Nullable`1<int32> V_52,
             int32 V_53,
             valuetype [mscorlib]System.Nullable`1<int32> V_54,
             valuetype [mscorlib]System.Nullable`1<int32> V_55,
             int32 V_56,
             valuetype [mscorlib]System.Nullable`1<int32> V_57,
             valuetype [mscorlib]System.Nullable`1<int32> V_58,
             int32 V_59,
             valuetype [mscorlib]System.Nullable`1<int32> V_60)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0008:  stloc.1
    IL_0009:  ldloca.s   V_0
    IL_000b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0010:  ldloc.1
    IL_0011:  bne.un.s   IL_001c

    IL_0013:  ldloca.s   V_0
    IL_0015:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001a:  br.s       IL_001d

    IL_001c:  ldc.i4.0
    IL_001d:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0022:  ldarg.0
    IL_0023:  stloc.2
    IL_0024:  ldarg.1
    IL_0025:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_002a:  stloc.3
    IL_002b:  ldloca.s   V_2
    IL_002d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0032:  ldloc.3
    IL_0033:  bne.un.s   IL_0041

    IL_0035:  ldloca.s   V_2
    IL_0037:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_003c:  ldc.i4.0
    IL_003d:  ceq
    IL_003f:  br.s       IL_0042

    IL_0041:  ldc.i4.1
    IL_0042:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0047:  ldarg.0
    IL_0048:  stloc.s    V_4
    IL_004a:  ldarg.1
    IL_004b:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0050:  stloc.s    V_5
    IL_0052:  ldloca.s   V_4
    IL_0054:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0059:  ldloc.s    V_5
    IL_005b:  ble.s      IL_0066

    IL_005d:  ldloca.s   V_4
    IL_005f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0064:  br.s       IL_0067

    IL_0066:  ldc.i4.0
    IL_0067:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_006c:  ldarg.1
    IL_006d:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0072:  stloc.s    V_6
    IL_0074:  ldarg.0
    IL_0075:  stloc.s    V_7
    IL_0077:  ldloc.s    V_6
    IL_0079:  ldloca.s   V_7
    IL_007b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0080:  bne.un.s   IL_008b

    IL_0082:  ldloca.s   V_7
    IL_0084:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0089:  br.s       IL_008c

    IL_008b:  ldc.i4.0
    IL_008c:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0091:  ldarg.1
    IL_0092:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0097:  stloc.s    V_8
    IL_0099:  ldarg.0
    IL_009a:  stloc.s    V_9
    IL_009c:  ldloc.s    V_8
    IL_009e:  ldloca.s   V_9
    IL_00a0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00a5:  bne.un.s   IL_00b3

    IL_00a7:  ldloca.s   V_9
    IL_00a9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ae:  ldc.i4.0
    IL_00af:  ceq
    IL_00b1:  br.s       IL_00b4

    IL_00b3:  ldc.i4.1
    IL_00b4:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00b9:  ldarg.1
    IL_00ba:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00bf:  stloc.s    V_10
    IL_00c1:  ldarg.0
    IL_00c2:  stloc.s    V_11
    IL_00c4:  ldloc.s    V_10
    IL_00c6:  ldloca.s   V_11
    IL_00c8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00cd:  ble.s      IL_00d8

    IL_00cf:  ldloca.s   V_11
    IL_00d1:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00d6:  br.s       IL_00d9

    IL_00d8:  ldc.i4.0
    IL_00d9:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00de:  ldarg.0
    IL_00df:  stloc.s    V_12
    IL_00e1:  ldarg.1
    IL_00e2:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00e7:  stloc.s    V_13
    IL_00e9:  ldloca.s   V_12
    IL_00eb:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00f0:  brtrue.s   IL_00fe

    IL_00f2:  ldloca.s   V_14
    IL_00f4:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00fa:  ldloc.s    V_14
    IL_00fc:  br.s       IL_010d

    IL_00fe:  ldloca.s   V_12
    IL_0100:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0105:  ldloc.s    V_13
    IL_0107:  add
    IL_0108:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_010d:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0112:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0117:  ldarg.0
    IL_0118:  stloc.s    V_15
    IL_011a:  ldarg.1
    IL_011b:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0120:  stloc.s    V_16
    IL_0122:  ldloca.s   V_15
    IL_0124:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0129:  brtrue.s   IL_0137

    IL_012b:  ldloca.s   V_17
    IL_012d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0133:  ldloc.s    V_17
    IL_0135:  br.s       IL_0146

    IL_0137:  ldloca.s   V_15
    IL_0139:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_013e:  ldloc.s    V_16
    IL_0140:  sub
    IL_0141:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0146:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_014b:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0150:  ldarg.0
    IL_0151:  stloc.s    V_18
    IL_0153:  ldarg.1
    IL_0154:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0159:  stloc.s    V_19
    IL_015b:  ldloca.s   V_18
    IL_015d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0162:  brtrue.s   IL_0170

    IL_0164:  ldloca.s   V_20
    IL_0166:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_016c:  ldloc.s    V_20
    IL_016e:  br.s       IL_017f

    IL_0170:  ldloca.s   V_18
    IL_0172:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0177:  ldloc.s    V_19
    IL_0179:  mul
    IL_017a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_017f:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0184:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0189:  ldarg.0
    IL_018a:  stloc.s    V_21
    IL_018c:  ldarg.1
    IL_018d:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0192:  stloc.s    V_22
    IL_0194:  ldloca.s   V_21
    IL_0196:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_019b:  brtrue.s   IL_01a9

    IL_019d:  ldloca.s   V_23
    IL_019f:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01a5:  ldloc.s    V_23
    IL_01a7:  br.s       IL_01b8

    IL_01a9:  ldloca.s   V_21
    IL_01ab:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01b0:  ldloc.s    V_22
    IL_01b2:  div
    IL_01b3:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01b8:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01bd:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01c2:  ldarg.0
    IL_01c3:  stloc.s    V_24
    IL_01c5:  ldarg.1
    IL_01c6:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_01cb:  stloc.s    V_25
    IL_01cd:  ldloca.s   V_24
    IL_01cf:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01d4:  brtrue.s   IL_01e2

    IL_01d6:  ldloca.s   V_26
    IL_01d8:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01de:  ldloc.s    V_26
    IL_01e0:  br.s       IL_01f1

    IL_01e2:  ldloca.s   V_24
    IL_01e4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01e9:  ldloc.s    V_25
    IL_01eb:  rem
    IL_01ec:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01f1:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01f6:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01fb:  ldarg.0
    IL_01fc:  stloc.s    V_27
    IL_01fe:  ldarg.1
    IL_01ff:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0204:  stloc.s    V_28
    IL_0206:  ldloca.s   V_27
    IL_0208:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_020d:  brtrue.s   IL_021b

    IL_020f:  ldloca.s   V_29
    IL_0211:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0217:  ldloc.s    V_29
    IL_0219:  br.s       IL_022a

    IL_021b:  ldloca.s   V_27
    IL_021d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0222:  ldloc.s    V_28
    IL_0224:  and
    IL_0225:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_022a:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_022f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0234:  ldarg.0
    IL_0235:  stloc.s    V_30
    IL_0237:  ldarg.1
    IL_0238:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_023d:  stloc.s    V_31
    IL_023f:  ldloca.s   V_30
    IL_0241:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0246:  brtrue.s   IL_0254

    IL_0248:  ldloca.s   V_32
    IL_024a:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0250:  ldloc.s    V_32
    IL_0252:  br.s       IL_0263

    IL_0254:  ldloca.s   V_30
    IL_0256:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_025b:  ldloc.s    V_31
    IL_025d:  or
    IL_025e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0263:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0268:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_026d:  ldarg.0
    IL_026e:  stloc.s    V_33
    IL_0270:  ldarg.1
    IL_0271:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0276:  stloc.s    V_34
    IL_0278:  ldloca.s   V_33
    IL_027a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_027f:  brtrue.s   IL_028d

    IL_0281:  ldloca.s   V_35
    IL_0283:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0289:  ldloc.s    V_35
    IL_028b:  br.s       IL_029c

    IL_028d:  ldloca.s   V_33
    IL_028f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0294:  ldloc.s    V_34
    IL_0296:  xor
    IL_0297:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_029c:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_02a1:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02a6:  ldarg.0
    IL_02a7:  stloc.s    V_36
    IL_02a9:  ldarg.1
    IL_02aa:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_02af:  stloc.s    V_37
    IL_02b1:  ldloca.s   V_36
    IL_02b3:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02b8:  brtrue.s   IL_02c6

    IL_02ba:  ldloca.s   V_38
    IL_02bc:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02c2:  ldloc.s    V_38
    IL_02c4:  br.s       IL_02d8

    IL_02c6:  ldloca.s   V_36
    IL_02c8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02cd:  ldloc.s    V_37
    IL_02cf:  ldc.i4.s   31
    IL_02d1:  and
    IL_02d2:  shl
    IL_02d3:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02d8:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_02dd:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02e2:  ldarg.0
    IL_02e3:  stloc.s    V_39
    IL_02e5:  ldarg.1
    IL_02e6:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_02eb:  stloc.s    V_40
    IL_02ed:  ldloca.s   V_39
    IL_02ef:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02f4:  brtrue.s   IL_0302

    IL_02f6:  ldloca.s   V_41
    IL_02f8:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02fe:  ldloc.s    V_41
    IL_0300:  br.s       IL_0314

    IL_0302:  ldloca.s   V_39
    IL_0304:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0309:  ldloc.s    V_40
    IL_030b:  ldc.i4.s   31
    IL_030d:  and
    IL_030e:  shr
    IL_030f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0314:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0319:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_031e:  ldarg.0
    IL_031f:  stloc.s    V_42
    IL_0321:  ldloca.s   V_42
    IL_0323:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0328:  brtrue.s   IL_0332

    IL_032a:  ldarg.1
    IL_032b:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0330:  br.s       IL_0339

    IL_0332:  ldloca.s   V_42
    IL_0334:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0339:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_033e:  ldarg.0
    IL_033f:  stloc.s    V_43
    IL_0341:  ldarg.1
    IL_0342:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0347:  stloc.s    V_44
    IL_0349:  ldloca.s   V_43
    IL_034b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0350:  brtrue.s   IL_035e

    IL_0352:  ldloca.s   V_45
    IL_0354:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_035a:  ldloc.s    V_45
    IL_035c:  br.s       IL_036d

    IL_035e:  ldloca.s   V_43
    IL_0360:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0365:  ldloc.s    V_44
    IL_0367:  add
    IL_0368:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_036d:  starg.s    a
    IL_036f:  ldarg.0
    IL_0370:  stloc.s    V_46
    IL_0372:  ldarg.1
    IL_0373:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0378:  stloc.s    V_47
    IL_037a:  ldloca.s   V_46
    IL_037c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0381:  brtrue.s   IL_038f

    IL_0383:  ldloca.s   V_48
    IL_0385:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_038b:  ldloc.s    V_48
    IL_038d:  br.s       IL_039e

    IL_038f:  ldloca.s   V_46
    IL_0391:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0396:  ldloc.s    V_47
    IL_0398:  sub
    IL_0399:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_039e:  starg.s    a
    IL_03a0:  ldarg.0
    IL_03a1:  stloc.s    V_49
    IL_03a3:  ldarg.1
    IL_03a4:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_03a9:  stloc.s    V_50
    IL_03ab:  ldloca.s   V_49
    IL_03ad:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03b2:  brtrue.s   IL_03c0

    IL_03b4:  ldloca.s   V_51
    IL_03b6:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03bc:  ldloc.s    V_51
    IL_03be:  br.s       IL_03cf

    IL_03c0:  ldloca.s   V_49
    IL_03c2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03c7:  ldloc.s    V_50
    IL_03c9:  mul
    IL_03ca:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03cf:  starg.s    a
    IL_03d1:  ldarg.0
    IL_03d2:  stloc.s    V_52
    IL_03d4:  ldarg.1
    IL_03d5:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_03da:  stloc.s    V_53
    IL_03dc:  ldloca.s   V_52
    IL_03de:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03e3:  brtrue.s   IL_03f1

    IL_03e5:  ldloca.s   V_54
    IL_03e7:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03ed:  ldloc.s    V_54
    IL_03ef:  br.s       IL_0400

    IL_03f1:  ldloca.s   V_52
    IL_03f3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03f8:  ldloc.s    V_53
    IL_03fa:  div
    IL_03fb:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0400:  starg.s    a
    IL_0402:  ldarg.0
    IL_0403:  stloc.s    V_55
    IL_0405:  ldarg.1
    IL_0406:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_040b:  stloc.s    V_56
    IL_040d:  ldloca.s   V_55
    IL_040f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0414:  brtrue.s   IL_0422

    IL_0416:  ldloca.s   V_57
    IL_0418:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_041e:  ldloc.s    V_57
    IL_0420:  br.s       IL_0431

    IL_0422:  ldloca.s   V_55
    IL_0424:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0429:  ldloc.s    V_56
    IL_042b:  rem
    IL_042c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0431:  starg.s    a
    IL_0433:  ldarg.0
    IL_0434:  stloc.s    V_58
    IL_0436:  ldarg.1
    IL_0437:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_043c:  stloc.s    V_59
    IL_043e:  ldloca.s   V_58
    IL_0440:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0445:  brtrue.s   IL_0453

    IL_0447:  ldloca.s   V_60
    IL_0449:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_044f:  ldloc.s    V_60
    IL_0451:  br.s       IL_0462

    IL_0453:  ldloca.s   V_58
    IL_0455:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_045a:  ldloc.s    V_59
    IL_045c:  and
    IL_045d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0462:  starg.s    a
    IL_0464:  ldarg.0
    IL_0465:  stloc.0
    IL_0466:  ldarg.1
    IL_0467:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_046c:  stloc.1
    IL_046d:  ldloca.s   V_0
    IL_046f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0474:  brtrue.s   IL_0481

    IL_0476:  ldloca.s   V_2
    IL_0478:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_047e:  ldloc.2
    IL_047f:  br.s       IL_048f

    IL_0481:  ldloca.s   V_0
    IL_0483:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0488:  ldloc.1
    IL_0489:  or
    IL_048a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_048f:  starg.s    a
    IL_0491:  ldarg.0
    IL_0492:  stloc.0
    IL_0493:  ldarg.1
    IL_0494:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0499:  stloc.1
    IL_049a:  ldloca.s   V_0
    IL_049c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04a1:  brtrue.s   IL_04ae

    IL_04a3:  ldloca.s   V_2
    IL_04a5:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04ab:  ldloc.2
    IL_04ac:  br.s       IL_04bc

    IL_04ae:  ldloca.s   V_0
    IL_04b0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04b5:  ldloc.1
    IL_04b6:  xor
    IL_04b7:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04bc:  starg.s    a
    IL_04be:  ldarg.0
    IL_04bf:  stloc.0
    IL_04c0:  ldarg.1
    IL_04c1:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_04c6:  stloc.1
    IL_04c7:  ldloca.s   V_0
    IL_04c9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04ce:  brtrue.s   IL_04db

    IL_04d0:  ldloca.s   V_2
    IL_04d2:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04d8:  ldloc.2
    IL_04d9:  br.s       IL_04ec

    IL_04db:  ldloca.s   V_0
    IL_04dd:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04e2:  ldloc.1
    IL_04e3:  ldc.i4.s   31
    IL_04e5:  and
    IL_04e6:  shl
    IL_04e7:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04ec:  starg.s    a
    IL_04ee:  ldarg.0
    IL_04ef:  stloc.0
    IL_04f0:  ldarg.1
    IL_04f1:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_04f6:  stloc.1
    IL_04f7:  ldloca.s   V_0
    IL_04f9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04fe:  brtrue.s   IL_050b

    IL_0500:  ldloca.s   V_2
    IL_0502:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0508:  ldloc.2
    IL_0509:  br.s       IL_051c

    IL_050b:  ldloca.s   V_0
    IL_050d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0512:  ldloc.1
    IL_0513:  ldc.i4.s   31
    IL_0515:  and
    IL_0516:  shr
    IL_0517:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_051c:  starg.s    a
    IL_051e:  ldarg.1
    IL_051f:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0524:  stloc.1
    IL_0525:  ldarg.0
    IL_0526:  stloc.0
    IL_0527:  ldloca.s   V_0
    IL_0529:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_052e:  brtrue.s   IL_053b

    IL_0530:  ldloca.s   V_2
    IL_0532:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0538:  ldloc.2
    IL_0539:  br.s       IL_0549

    IL_053b:  ldloc.1
    IL_053c:  ldloca.s   V_0
    IL_053e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0543:  add
    IL_0544:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0549:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_054e:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0553:  ldc.i4.0
    IL_0554:  newarr     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0559:  ldc.i4.0
    IL_055a:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_055f:  dup
    IL_0560:  ldobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0565:  stloc.0
    IL_0566:  ldarg.1
    IL_0567:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_056c:  stloc.1
    IL_056d:  ldloca.s   V_0
    IL_056f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0574:  brtrue.s   IL_0581

    IL_0576:  ldloca.s   V_2
    IL_0578:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_057e:  ldloc.2
    IL_057f:  br.s       IL_058f

    IL_0581:  ldloca.s   V_0
    IL_0583:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0588:  ldloc.1
    IL_0589:  add
    IL_058a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_058f:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0594:  ret
  } // end of method LiftedOperators::IntValueComplex

  .method public hidebysig static void  IntValueConst(valuetype [mscorlib]System.Nullable`1<int32> a) cil managed
  {
    // Code size       1126 (0x466)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             valuetype [mscorlib]System.Nullable`1<int32> V_3,
             valuetype [mscorlib]System.Nullable`1<int32> V_4,
             valuetype [mscorlib]System.Nullable`1<int32> V_5,
             valuetype [mscorlib]System.Nullable`1<int32> V_6,
             valuetype [mscorlib]System.Nullable`1<int32> V_7,
             valuetype [mscorlib]System.Nullable`1<int32> V_8,
             valuetype [mscorlib]System.Nullable`1<int32> V_9,
             valuetype [mscorlib]System.Nullable`1<int32> V_10,
             valuetype [mscorlib]System.Nullable`1<int32> V_11,
             valuetype [mscorlib]System.Nullable`1<int32> V_12,
             valuetype [mscorlib]System.Nullable`1<int32> V_13,
             valuetype [mscorlib]System.Nullable`1<int32> V_14,
             valuetype [mscorlib]System.Nullable`1<int32> V_15,
             valuetype [mscorlib]System.Nullable`1<int32> V_16,
             valuetype [mscorlib]System.Nullable`1<int32> V_17,
             valuetype [mscorlib]System.Nullable`1<int32> V_18,
             valuetype [mscorlib]System.Nullable`1<int32> V_19,
             valuetype [mscorlib]System.Nullable`1<int32> V_20,
             valuetype [mscorlib]System.Nullable`1<int32> V_21,
             valuetype [mscorlib]System.Nullable`1<int32> V_22,
             valuetype [mscorlib]System.Nullable`1<int32> V_23,
             valuetype [mscorlib]System.Nullable`1<int32> V_24,
             valuetype [mscorlib]System.Nullable`1<int32> V_25,
             valuetype [mscorlib]System.Nullable`1<int32> V_26,
             valuetype [mscorlib]System.Nullable`1<int32> V_27,
             valuetype [mscorlib]System.Nullable`1<int32> V_28,
             valuetype [mscorlib]System.Nullable`1<int32> V_29,
             valuetype [mscorlib]System.Nullable`1<int32> V_30,
             valuetype [mscorlib]System.Nullable`1<int32> V_31,
             valuetype [mscorlib]System.Nullable`1<int32> V_32,
             valuetype [mscorlib]System.Nullable`1<int32> V_33,
             valuetype [mscorlib]System.Nullable`1<int32> V_34,
             valuetype [mscorlib]System.Nullable`1<int32> V_35,
             valuetype [mscorlib]System.Nullable`1<int32> V_36,
             valuetype [mscorlib]System.Nullable`1<int32> V_37,
             valuetype [mscorlib]System.Nullable`1<int32> V_38,
             valuetype [mscorlib]System.Nullable`1<int32> V_39,
             valuetype [mscorlib]System.Nullable`1<int32> V_40,
             valuetype [mscorlib]System.Nullable`1<int32> V_41,
             valuetype [mscorlib]System.Nullable`1<int32> V_42,
             valuetype [mscorlib]System.Nullable`1<int32> V_43,
             valuetype [mscorlib]System.Nullable`1<int32> V_44,
             valuetype [mscorlib]System.Nullable`1<int32> V_45,
             valuetype [mscorlib]System.Nullable`1<int32> V_46,
             valuetype [mscorlib]System.Nullable`1<int32> V_47,
             valuetype [mscorlib]System.Nullable`1<int32> V_48)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0009:  ldc.i4.2
    IL_000a:  bne.un.s   IL_0015

    IL_000c:  ldloca.s   V_0
    IL_000e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0013:  br.s       IL_0016

    IL_0015:  ldc.i4.0
    IL_0016:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_001b:  ldarg.0
    IL_001c:  stloc.1
    IL_001d:  ldloca.s   V_1
    IL_001f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0024:  ldc.i4.2
    IL_0025:  bne.un.s   IL_0033

    IL_0027:  ldloca.s   V_1
    IL_0029:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_002e:  ldc.i4.0
    IL_002f:  ceq
    IL_0031:  br.s       IL_0034

    IL_0033:  ldc.i4.1
    IL_0034:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0039:  ldarg.0
    IL_003a:  stloc.2
    IL_003b:  ldloca.s   V_2
    IL_003d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0042:  ldc.i4.2
    IL_0043:  ble.s      IL_004e

    IL_0045:  ldloca.s   V_2
    IL_0047:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_004c:  br.s       IL_004f

    IL_004e:  ldc.i4.0
    IL_004f:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0054:  ldarg.0
    IL_0055:  stloc.3
    IL_0056:  ldc.i4.2
    IL_0057:  ldloca.s   V_3
    IL_0059:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_005e:  bne.un.s   IL_0069

    IL_0060:  ldloca.s   V_3
    IL_0062:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0067:  br.s       IL_006a

    IL_0069:  ldc.i4.0
    IL_006a:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_006f:  ldarg.0
    IL_0070:  stloc.s    V_4
    IL_0072:  ldc.i4.2
    IL_0073:  ldloca.s   V_4
    IL_0075:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_007a:  bne.un.s   IL_0088

    IL_007c:  ldloca.s   V_4
    IL_007e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0083:  ldc.i4.0
    IL_0084:  ceq
    IL_0086:  br.s       IL_0089

    IL_0088:  ldc.i4.1
    IL_0089:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_008e:  ldarg.0
    IL_008f:  stloc.s    V_5
    IL_0091:  ldc.i4.2
    IL_0092:  ldloca.s   V_5
    IL_0094:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0099:  ble.s      IL_00a4

    IL_009b:  ldloca.s   V_5
    IL_009d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00a2:  br.s       IL_00a5

    IL_00a4:  ldc.i4.0
    IL_00a5:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00aa:  ldarg.0
    IL_00ab:  stloc.s    V_6
    IL_00ad:  ldloca.s   V_6
    IL_00af:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00b4:  brtrue.s   IL_00c2

    IL_00b6:  ldloca.s   V_7
    IL_00b8:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00be:  ldloc.s    V_7
    IL_00c0:  br.s       IL_00d0

    IL_00c2:  ldloca.s   V_6
    IL_00c4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00c9:  ldc.i4.2
    IL_00ca:  add
    IL_00cb:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00d0:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_00d5:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00da:  ldarg.0
    IL_00db:  stloc.s    V_8
    IL_00dd:  ldloca.s   V_8
    IL_00df:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00e4:  brtrue.s   IL_00f2

    IL_00e6:  ldloca.s   V_9
    IL_00e8:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00ee:  ldloc.s    V_9
    IL_00f0:  br.s       IL_0100

    IL_00f2:  ldloca.s   V_8
    IL_00f4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00f9:  ldc.i4.2
    IL_00fa:  sub
    IL_00fb:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0100:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0105:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_010a:  ldarg.0
    IL_010b:  stloc.s    V_10
    IL_010d:  ldloca.s   V_10
    IL_010f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0114:  brtrue.s   IL_0122

    IL_0116:  ldloca.s   V_11
    IL_0118:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_011e:  ldloc.s    V_11
    IL_0120:  br.s       IL_0130

    IL_0122:  ldloca.s   V_10
    IL_0124:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0129:  ldc.i4.2
    IL_012a:  mul
    IL_012b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0130:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0135:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_013a:  ldarg.0
    IL_013b:  stloc.s    V_12
    IL_013d:  ldloca.s   V_12
    IL_013f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0144:  brtrue.s   IL_0152

    IL_0146:  ldloca.s   V_13
    IL_0148:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_014e:  ldloc.s    V_13
    IL_0150:  br.s       IL_0160

    IL_0152:  ldloca.s   V_12
    IL_0154:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0159:  ldc.i4.2
    IL_015a:  div
    IL_015b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0160:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0165:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_016a:  ldarg.0
    IL_016b:  stloc.s    V_14
    IL_016d:  ldloca.s   V_14
    IL_016f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0174:  brtrue.s   IL_0182

    IL_0176:  ldloca.s   V_15
    IL_0178:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_017e:  ldloc.s    V_15
    IL_0180:  br.s       IL_0190

    IL_0182:  ldloca.s   V_14
    IL_0184:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0189:  ldc.i4.2
    IL_018a:  rem
    IL_018b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0190:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0195:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_019a:  ldarg.0
    IL_019b:  stloc.s    V_16
    IL_019d:  ldloca.s   V_16
    IL_019f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01a4:  brtrue.s   IL_01b2

    IL_01a6:  ldloca.s   V_17
    IL_01a8:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01ae:  ldloc.s    V_17
    IL_01b0:  br.s       IL_01c0

    IL_01b2:  ldloca.s   V_16
    IL_01b4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01b9:  ldc.i4.2
    IL_01ba:  and
    IL_01bb:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01c0:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01c5:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01ca:  ldarg.0
    IL_01cb:  stloc.s    V_18
    IL_01cd:  ldloca.s   V_18
    IL_01cf:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01d4:  brtrue.s   IL_01e2

    IL_01d6:  ldloca.s   V_19
    IL_01d8:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01de:  ldloc.s    V_19
    IL_01e0:  br.s       IL_01f0

    IL_01e2:  ldloca.s   V_18
    IL_01e4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01e9:  ldc.i4.2
    IL_01ea:  or
    IL_01eb:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01f0:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01f5:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01fa:  ldarg.0
    IL_01fb:  stloc.s    V_20
    IL_01fd:  ldloca.s   V_20
    IL_01ff:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0204:  brtrue.s   IL_0212

    IL_0206:  ldloca.s   V_21
    IL_0208:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_020e:  ldloc.s    V_21
    IL_0210:  br.s       IL_0220

    IL_0212:  ldloca.s   V_20
    IL_0214:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0219:  ldc.i4.2
    IL_021a:  xor
    IL_021b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0220:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0225:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_022a:  ldarg.0
    IL_022b:  stloc.s    V_22
    IL_022d:  ldloca.s   V_22
    IL_022f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0234:  brtrue.s   IL_0242

    IL_0236:  ldloca.s   V_23
    IL_0238:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_023e:  ldloc.s    V_23
    IL_0240:  br.s       IL_0250

    IL_0242:  ldloca.s   V_22
    IL_0244:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0249:  ldc.i4.2
    IL_024a:  shl
    IL_024b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0250:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0255:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_025a:  ldarg.0
    IL_025b:  stloc.s    V_24
    IL_025d:  ldloca.s   V_24
    IL_025f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0264:  brtrue.s   IL_0272

    IL_0266:  ldloca.s   V_25
    IL_0268:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_026e:  ldloc.s    V_25
    IL_0270:  br.s       IL_0280

    IL_0272:  ldloca.s   V_24
    IL_0274:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0279:  ldc.i4.2
    IL_027a:  shr
    IL_027b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0280:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0285:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_028a:  ldarg.0
    IL_028b:  stloc.s    V_26
    IL_028d:  ldloca.s   V_26
    IL_028f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0294:  brtrue.s   IL_0299

    IL_0296:  ldc.i4.2
    IL_0297:  br.s       IL_02a0

    IL_0299:  ldloca.s   V_26
    IL_029b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02a0:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_02a5:  ldarg.0
    IL_02a6:  stloc.s    V_27
    IL_02a8:  ldloca.s   V_27
    IL_02aa:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02af:  brtrue.s   IL_02bd

    IL_02b1:  ldloca.s   V_28
    IL_02b3:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02b9:  ldloc.s    V_28
    IL_02bb:  br.s       IL_02cb

    IL_02bd:  ldloca.s   V_27
    IL_02bf:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02c4:  ldc.i4.2
    IL_02c5:  add
    IL_02c6:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02cb:  starg.s    a
    IL_02cd:  ldarg.0
    IL_02ce:  stloc.s    V_29
    IL_02d0:  ldloca.s   V_29
    IL_02d2:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02d7:  brtrue.s   IL_02e5

    IL_02d9:  ldloca.s   V_30
    IL_02db:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02e1:  ldloc.s    V_30
    IL_02e3:  br.s       IL_02f3

    IL_02e5:  ldloca.s   V_29
    IL_02e7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02ec:  ldc.i4.2
    IL_02ed:  sub
    IL_02ee:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02f3:  starg.s    a
    IL_02f5:  ldarg.0
    IL_02f6:  stloc.s    V_31
    IL_02f8:  ldloca.s   V_31
    IL_02fa:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02ff:  brtrue.s   IL_030d

    IL_0301:  ldloca.s   V_32
    IL_0303:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0309:  ldloc.s    V_32
    IL_030b:  br.s       IL_031b

    IL_030d:  ldloca.s   V_31
    IL_030f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0314:  ldc.i4.2
    IL_0315:  mul
    IL_0316:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_031b:  starg.s    a
    IL_031d:  ldarg.0
    IL_031e:  stloc.s    V_33
    IL_0320:  ldloca.s   V_33
    IL_0322:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0327:  brtrue.s   IL_0335

    IL_0329:  ldloca.s   V_34
    IL_032b:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0331:  ldloc.s    V_34
    IL_0333:  br.s       IL_0343

    IL_0335:  ldloca.s   V_33
    IL_0337:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_033c:  ldc.i4.2
    IL_033d:  div
    IL_033e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0343:  starg.s    a
    IL_0345:  ldarg.0
    IL_0346:  stloc.s    V_35
    IL_0348:  ldloca.s   V_35
    IL_034a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_034f:  brtrue.s   IL_035d

    IL_0351:  ldloca.s   V_36
    IL_0353:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0359:  ldloc.s    V_36
    IL_035b:  br.s       IL_036b

    IL_035d:  ldloca.s   V_35
    IL_035f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0364:  ldc.i4.2
    IL_0365:  rem
    IL_0366:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_036b:  starg.s    a
    IL_036d:  ldarg.0
    IL_036e:  stloc.s    V_37
    IL_0370:  ldloca.s   V_37
    IL_0372:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0377:  brtrue.s   IL_0385

    IL_0379:  ldloca.s   V_38
    IL_037b:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0381:  ldloc.s    V_38
    IL_0383:  br.s       IL_0393

    IL_0385:  ldloca.s   V_37
    IL_0387:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_038c:  ldc.i4.2
    IL_038d:  and
    IL_038e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0393:  starg.s    a
    IL_0395:  ldarg.0
    IL_0396:  stloc.s    V_39
    IL_0398:  ldloca.s   V_39
    IL_039a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_039f:  brtrue.s   IL_03ad

    IL_03a1:  ldloca.s   V_40
    IL_03a3:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03a9:  ldloc.s    V_40
    IL_03ab:  br.s       IL_03bb

    IL_03ad:  ldloca.s   V_39
    IL_03af:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03b4:  ldc.i4.2
    IL_03b5:  or
    IL_03b6:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03bb:  starg.s    a
    IL_03bd:  ldarg.0
    IL_03be:  stloc.s    V_41
    IL_03c0:  ldloca.s   V_41
    IL_03c2:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03c7:  brtrue.s   IL_03d5

    IL_03c9:  ldloca.s   V_42
    IL_03cb:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03d1:  ldloc.s    V_42
    IL_03d3:  br.s       IL_03e3

    IL_03d5:  ldloca.s   V_41
    IL_03d7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03dc:  ldc.i4.2
    IL_03dd:  xor
    IL_03de:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03e3:  starg.s    a
    IL_03e5:  ldarg.0
    IL_03e6:  stloc.s    V_43
    IL_03e8:  ldloca.s   V_43
    IL_03ea:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03ef:  brtrue.s   IL_03fd

    IL_03f1:  ldloca.s   V_44
    IL_03f3:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03f9:  ldloc.s    V_44
    IL_03fb:  br.s       IL_040b

    IL_03fd:  ldloca.s   V_43
    IL_03ff:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0404:  ldc.i4.2
    IL_0405:  shl
    IL_0406:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_040b:  starg.s    a
    IL_040d:  ldarg.0
    IL_040e:  stloc.s    V_45
    IL_0410:  ldloca.s   V_45
    IL_0412:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0417:  brtrue.s   IL_0425

    IL_0419:  ldloca.s   V_46
    IL_041b:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0421:  ldloc.s    V_46
    IL_0423:  br.s       IL_0433

    IL_0425:  ldloca.s   V_45
    IL_0427:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_042c:  ldc.i4.2
    IL_042d:  shr
    IL_042e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0433:  starg.s    a
    IL_0435:  ldarg.0
    IL_0436:  stloc.s    V_47
    IL_0438:  ldloca.s   V_47
    IL_043a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_043f:  brtrue.s   IL_044d

    IL_0441:  ldloca.s   V_48
    IL_0443:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0449:  ldloc.s    V_48
    IL_044b:  br.s       IL_045b

    IL_044d:  ldc.i4.2
    IL_044e:  ldloca.s   V_47
    IL_0450:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0455:  add
    IL_0456:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_045b:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0460:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0465:  ret
  } // end of method LiftedOperators::IntValueConst

  .method public hidebysig static void  NumberBasic(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> a,
                                                    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> b) cil managed
  {
    // Code size       418 (0x1a2)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_0,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_1,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_2,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_3,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_4,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_5,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_6,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_7,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_8,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_9,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_10,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_11,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_12,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_13,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_14,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_15)
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
    IL_0017:  brfalse.s  IL_002b

    IL_0019:  ldloca.s   V_0
    IL_001b:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0020:  ldloca.s   V_1
    IL_0022:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0027:  ceq
    IL_0029:  br.s       IL_002c

    IL_002b:  ldc.i4.0
    IL_002c:  brfalse.s  IL_0033

    IL_002e:  call       void [mscorlib]System.Console::WriteLine()
    IL_0033:  ldarg.0
    IL_0034:  stloc.2
    IL_0035:  ldarg.1
    IL_0036:  stloc.3
    IL_0037:  ldloca.s   V_2
    IL_0039:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_003e:  ldloca.s   V_3
    IL_0040:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0045:  call       bool [mscorlib]System.Decimal::op_Inequality(valuetype [mscorlib]System.Decimal,
                                                                      valuetype [mscorlib]System.Decimal)
    IL_004a:  brtrue.s   IL_0061

    IL_004c:  ldloca.s   V_2
    IL_004e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0053:  ldloca.s   V_3
    IL_0055:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_005a:  ceq
    IL_005c:  ldc.i4.0
    IL_005d:  ceq
    IL_005f:  br.s       IL_0062

    IL_0061:  ldc.i4.1
    IL_0062:  brfalse.s  IL_0069

    IL_0064:  call       void [mscorlib]System.Console::WriteLine()
    IL_0069:  ldarg.0
    IL_006a:  stloc.s    V_4
    IL_006c:  ldarg.1
    IL_006d:  stloc.s    V_5
    IL_006f:  ldloca.s   V_4
    IL_0071:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0076:  ldloca.s   V_5
    IL_0078:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_007d:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_0082:  brfalse.s  IL_0095

    IL_0084:  ldloca.s   V_4
    IL_0086:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_008b:  ldloca.s   V_5
    IL_008d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0092:  and
    IL_0093:  br.s       IL_0096

    IL_0095:  ldc.i4.0
    IL_0096:  brfalse.s  IL_009d

    IL_0098:  call       void [mscorlib]System.Console::WriteLine()
    IL_009d:  ldarg.0
    IL_009e:  stloc.s    V_6
    IL_00a0:  ldarg.1
    IL_00a1:  stloc.s    V_7
    IL_00a3:  ldloca.s   V_6
    IL_00a5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00aa:  ldloca.s   V_7
    IL_00ac:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00b1:  call       bool [mscorlib]System.Decimal::op_LessThan(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_00b6:  brfalse.s  IL_00c9

    IL_00b8:  ldloca.s   V_6
    IL_00ba:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00bf:  ldloca.s   V_7
    IL_00c1:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00c6:  and
    IL_00c7:  br.s       IL_00ca

    IL_00c9:  ldc.i4.0
    IL_00ca:  brfalse.s  IL_00d1

    IL_00cc:  call       void [mscorlib]System.Console::WriteLine()
    IL_00d1:  ldarg.0
    IL_00d2:  stloc.s    V_8
    IL_00d4:  ldarg.1
    IL_00d5:  stloc.s    V_9
    IL_00d7:  ldloca.s   V_8
    IL_00d9:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00de:  ldloca.s   V_9
    IL_00e0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00e5:  call       bool [mscorlib]System.Decimal::op_GreaterThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                              valuetype [mscorlib]System.Decimal)
    IL_00ea:  brfalse.s  IL_00fd

    IL_00ec:  ldloca.s   V_8
    IL_00ee:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00f3:  ldloca.s   V_9
    IL_00f5:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00fa:  and
    IL_00fb:  br.s       IL_00fe

    IL_00fd:  ldc.i4.0
    IL_00fe:  brfalse.s  IL_0105

    IL_0100:  call       void [mscorlib]System.Console::WriteLine()
    IL_0105:  ldarg.0
    IL_0106:  stloc.s    V_10
    IL_0108:  ldarg.1
    IL_0109:  stloc.s    V_11
    IL_010b:  ldloca.s   V_10
    IL_010d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0112:  ldloca.s   V_11
    IL_0114:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0119:  call       bool [mscorlib]System.Decimal::op_LessThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                           valuetype [mscorlib]System.Decimal)
    IL_011e:  brfalse.s  IL_0131

    IL_0120:  ldloca.s   V_10
    IL_0122:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0127:  ldloca.s   V_11
    IL_0129:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_012e:  and
    IL_012f:  br.s       IL_0132

    IL_0131:  ldc.i4.0
    IL_0132:  brfalse.s  IL_0139

    IL_0134:  call       void [mscorlib]System.Console::WriteLine()
    IL_0139:  ldarg.0
    IL_013a:  stloc.s    V_12
    IL_013c:  ldarg.1
    IL_013d:  stloc.s    V_13
    IL_013f:  ldloca.s   V_12
    IL_0141:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0146:  ldloca.s   V_13
    IL_0148:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_014d:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_0152:  brfalse.s  IL_0165

    IL_0154:  ldloca.s   V_12
    IL_0156:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_015b:  ldloca.s   V_13
    IL_015d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0162:  and
    IL_0163:  br.s       IL_0166

    IL_0165:  ldc.i4.0
    IL_0166:  brtrue.s   IL_016d

    IL_0168:  call       void [mscorlib]System.Console::WriteLine()
    IL_016d:  ldarg.0
    IL_016e:  stloc.s    V_14
    IL_0170:  ldarg.1
    IL_0171:  stloc.s    V_15
    IL_0173:  ldloca.s   V_14
    IL_0175:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_017a:  ldloca.s   V_15
    IL_017c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0181:  call       bool [mscorlib]System.Decimal::op_LessThan(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_0186:  brfalse.s  IL_0199

    IL_0188:  ldloca.s   V_14
    IL_018a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_018f:  ldloca.s   V_15
    IL_0191:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0196:  and
    IL_0197:  br.s       IL_019a

    IL_0199:  ldc.i4.0
    IL_019a:  brtrue.s   IL_01a1

    IL_019c:  call       void [mscorlib]System.Console::WriteLine()
    IL_01a1:  ret
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
    // Code size       996 (0x3e4)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_0,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_1,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_2,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_3,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_4,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_5,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_6,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_7,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_8,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_9,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_10,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_11,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_12,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_13,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_14,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_15,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_16,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_17,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_18,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_19,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_20,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_21,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_22,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_23,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_24,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_25,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_26,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_27,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_28,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_29,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_30,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_31,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_32,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_33,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_34,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_35,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_36,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_37,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_38,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_39,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_40,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_41,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_42)
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
    IL_0017:  brfalse.s  IL_002b

    IL_0019:  ldloca.s   V_0
    IL_001b:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0020:  ldloca.s   V_1
    IL_0022:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0027:  ceq
    IL_0029:  br.s       IL_002c

    IL_002b:  ldc.i4.0
    IL_002c:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0031:  ldarg.0
    IL_0032:  stloc.2
    IL_0033:  ldarg.1
    IL_0034:  stloc.3
    IL_0035:  ldloca.s   V_2
    IL_0037:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_003c:  ldloca.s   V_3
    IL_003e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0043:  call       bool [mscorlib]System.Decimal::op_Inequality(valuetype [mscorlib]System.Decimal,
                                                                      valuetype [mscorlib]System.Decimal)
    IL_0048:  brtrue.s   IL_005f

    IL_004a:  ldloca.s   V_2
    IL_004c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0051:  ldloca.s   V_3
    IL_0053:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0058:  ceq
    IL_005a:  ldc.i4.0
    IL_005b:  ceq
    IL_005d:  br.s       IL_0060

    IL_005f:  ldc.i4.1
    IL_0060:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0065:  ldarg.0
    IL_0066:  stloc.s    V_4
    IL_0068:  ldarg.1
    IL_0069:  stloc.s    V_5
    IL_006b:  ldloca.s   V_4
    IL_006d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0072:  ldloca.s   V_5
    IL_0074:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0079:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_007e:  brfalse.s  IL_0091

    IL_0080:  ldloca.s   V_4
    IL_0082:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0087:  ldloca.s   V_5
    IL_0089:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_008e:  and
    IL_008f:  br.s       IL_0092

    IL_0091:  ldc.i4.0
    IL_0092:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0097:  ldarg.0
    IL_0098:  stloc.s    V_6
    IL_009a:  ldarg.1
    IL_009b:  stloc.s    V_7
    IL_009d:  ldloca.s   V_6
    IL_009f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00a4:  ldloca.s   V_7
    IL_00a6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00ab:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_00b0:  brfalse.s  IL_00c3

    IL_00b2:  ldloca.s   V_6
    IL_00b4:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00b9:  ldloca.s   V_7
    IL_00bb:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00c0:  and
    IL_00c1:  br.s       IL_00c4

    IL_00c3:  ldc.i4.0
    IL_00c4:  ldc.i4.0
    IL_00c5:  ceq
    IL_00c7:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00cc:  ldarg.0
    IL_00cd:  stloc.s    V_8
    IL_00cf:  ldarg.1
    IL_00d0:  stloc.s    V_9
    IL_00d2:  ldloca.s   V_8
    IL_00d4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00d9:  ldloca.s   V_9
    IL_00db:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00e0:  call       bool [mscorlib]System.Decimal::op_LessThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                           valuetype [mscorlib]System.Decimal)
    IL_00e5:  brfalse.s  IL_00f8

    IL_00e7:  ldloca.s   V_8
    IL_00e9:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00ee:  ldloca.s   V_9
    IL_00f0:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00f5:  and
    IL_00f6:  br.s       IL_00f9

    IL_00f8:  ldc.i4.0
    IL_00f9:  ldc.i4.0
    IL_00fa:  ceq
    IL_00fc:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0101:  ldarg.0
    IL_0102:  stloc.s    V_10
    IL_0104:  ldarg.1
    IL_0105:  stloc.s    V_11
    IL_0107:  ldloca.s   V_10
    IL_0109:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_010e:  ldloca.s   V_11
    IL_0110:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0115:  and
    IL_0116:  brtrue.s   IL_0124

    IL_0118:  ldloca.s   V_12
    IL_011a:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0120:  ldloc.s    V_12
    IL_0122:  br.s       IL_013c

    IL_0124:  ldloca.s   V_10
    IL_0126:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_012b:  ldloca.s   V_11
    IL_012d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0132:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_0137:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_013c:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0141:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0146:  ldarg.0
    IL_0147:  stloc.s    V_13
    IL_0149:  ldarg.1
    IL_014a:  stloc.s    V_14
    IL_014c:  ldloca.s   V_13
    IL_014e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0153:  ldloca.s   V_14
    IL_0155:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_015a:  and
    IL_015b:  brtrue.s   IL_0169

    IL_015d:  ldloca.s   V_15
    IL_015f:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0165:  ldloc.s    V_15
    IL_0167:  br.s       IL_0181

    IL_0169:  ldloca.s   V_13
    IL_016b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0170:  ldloca.s   V_14
    IL_0172:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0177:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Subtraction(valuetype [mscorlib]System.Decimal,
                                                                                                     valuetype [mscorlib]System.Decimal)
    IL_017c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0181:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0186:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_018b:  ldarg.0
    IL_018c:  stloc.s    V_16
    IL_018e:  ldarg.1
    IL_018f:  stloc.s    V_17
    IL_0191:  ldloca.s   V_16
    IL_0193:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0198:  ldloca.s   V_17
    IL_019a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_019f:  and
    IL_01a0:  brtrue.s   IL_01ae

    IL_01a2:  ldloca.s   V_18
    IL_01a4:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_01aa:  ldloc.s    V_18
    IL_01ac:  br.s       IL_01c6

    IL_01ae:  ldloca.s   V_16
    IL_01b0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01b5:  ldloca.s   V_17
    IL_01b7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01bc:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Multiply(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_01c1:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_01c6:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_01cb:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01d0:  ldarg.0
    IL_01d1:  stloc.s    V_19
    IL_01d3:  ldarg.1
    IL_01d4:  stloc.s    V_20
    IL_01d6:  ldloca.s   V_19
    IL_01d8:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_01dd:  ldloca.s   V_20
    IL_01df:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_01e4:  and
    IL_01e5:  brtrue.s   IL_01f3

    IL_01e7:  ldloca.s   V_21
    IL_01e9:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_01ef:  ldloc.s    V_21
    IL_01f1:  br.s       IL_020b

    IL_01f3:  ldloca.s   V_19
    IL_01f5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01fa:  ldloca.s   V_20
    IL_01fc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0201:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Division(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_0206:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_020b:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0210:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0215:  ldarg.0
    IL_0216:  stloc.s    V_22
    IL_0218:  ldarg.1
    IL_0219:  stloc.s    V_23
    IL_021b:  ldloca.s   V_22
    IL_021d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0222:  ldloca.s   V_23
    IL_0224:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0229:  and
    IL_022a:  brtrue.s   IL_0238

    IL_022c:  ldloca.s   V_24
    IL_022e:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0234:  ldloc.s    V_24
    IL_0236:  br.s       IL_0250

    IL_0238:  ldloca.s   V_22
    IL_023a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_023f:  ldloca.s   V_23
    IL_0241:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0246:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Modulus(valuetype [mscorlib]System.Decimal,
                                                                                                 valuetype [mscorlib]System.Decimal)
    IL_024b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0250:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0255:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_025a:  ldarg.0
    IL_025b:  stloc.s    V_25
    IL_025d:  ldloca.s   V_25
    IL_025f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0264:  brtrue.s   IL_0269

    IL_0266:  ldarg.1
    IL_0267:  br.s       IL_0275

    IL_0269:  ldloca.s   V_25
    IL_026b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0270:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0275:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_027a:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_027f:  ldarg.0
    IL_0280:  stloc.s    V_26
    IL_0282:  ldloca.s   V_26
    IL_0284:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0289:  brtrue.s   IL_0297

    IL_028b:  ldloca.s   V_27
    IL_028d:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0293:  ldloc.s    V_27
    IL_0295:  br.s       IL_02a8

    IL_0297:  ldloca.s   V_26
    IL_0299:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_029e:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_UnaryNegation(valuetype [mscorlib]System.Decimal)
    IL_02a3:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_02a8:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_02ad:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02b2:  ldarg.0
    IL_02b3:  stloc.s    V_28
    IL_02b5:  ldarg.1
    IL_02b6:  stloc.s    V_29
    IL_02b8:  ldloca.s   V_28
    IL_02ba:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_02bf:  ldloca.s   V_29
    IL_02c1:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_02c6:  and
    IL_02c7:  brtrue.s   IL_02d5

    IL_02c9:  ldloca.s   V_30
    IL_02cb:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_02d1:  ldloc.s    V_30
    IL_02d3:  br.s       IL_02ed

    IL_02d5:  ldloca.s   V_28
    IL_02d7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_02dc:  ldloca.s   V_29
    IL_02de:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_02e3:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_02e8:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_02ed:  starg.s    a
    IL_02ef:  ldarg.0
    IL_02f0:  stloc.s    V_31
    IL_02f2:  ldarg.1
    IL_02f3:  stloc.s    V_32
    IL_02f5:  ldloca.s   V_31
    IL_02f7:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_02fc:  ldloca.s   V_32
    IL_02fe:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0303:  and
    IL_0304:  brtrue.s   IL_0312

    IL_0306:  ldloca.s   V_33
    IL_0308:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_030e:  ldloc.s    V_33
    IL_0310:  br.s       IL_032a

    IL_0312:  ldloca.s   V_31
    IL_0314:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0319:  ldloca.s   V_32
    IL_031b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0320:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Subtraction(valuetype [mscorlib]System.Decimal,
                                                                                                     valuetype [mscorlib]System.Decimal)
    IL_0325:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_032a:  starg.s    a
    IL_032c:  ldarg.0
    IL_032d:  stloc.s    V_34
    IL_032f:  ldarg.1
    IL_0330:  stloc.s    V_35
    IL_0332:  ldloca.s   V_34
    IL_0334:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0339:  ldloca.s   V_35
    IL_033b:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0340:  and
    IL_0341:  brtrue.s   IL_034f

    IL_0343:  ldloca.s   V_36
    IL_0345:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_034b:  ldloc.s    V_36
    IL_034d:  br.s       IL_0367

    IL_034f:  ldloca.s   V_34
    IL_0351:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0356:  ldloca.s   V_35
    IL_0358:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_035d:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Multiply(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_0362:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0367:  starg.s    a
    IL_0369:  ldarg.0
    IL_036a:  stloc.s    V_37
    IL_036c:  ldarg.1
    IL_036d:  stloc.s    V_38
    IL_036f:  ldloca.s   V_37
    IL_0371:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0376:  ldloca.s   V_38
    IL_0378:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_037d:  and
    IL_037e:  brtrue.s   IL_038c

    IL_0380:  ldloca.s   V_39
    IL_0382:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0388:  ldloc.s    V_39
    IL_038a:  br.s       IL_03a4

    IL_038c:  ldloca.s   V_37
    IL_038e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0393:  ldloca.s   V_38
    IL_0395:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_039a:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Division(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_039f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_03a4:  starg.s    a
    IL_03a6:  ldarg.0
    IL_03a7:  stloc.s    V_40
    IL_03a9:  ldarg.1
    IL_03aa:  stloc.s    V_41
    IL_03ac:  ldloca.s   V_40
    IL_03ae:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_03b3:  ldloca.s   V_41
    IL_03b5:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_03ba:  and
    IL_03bb:  brtrue.s   IL_03c9

    IL_03bd:  ldloca.s   V_42
    IL_03bf:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_03c5:  ldloc.s    V_42
    IL_03c7:  br.s       IL_03e1

    IL_03c9:  ldloca.s   V_40
    IL_03cb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_03d0:  ldloca.s   V_41
    IL_03d2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_03d7:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Modulus(valuetype [mscorlib]System.Decimal,
                                                                                                 valuetype [mscorlib]System.Decimal)
    IL_03dc:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_03e1:  starg.s    a
    IL_03e3:  ret
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
    // Code size       94 (0x5e)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int64> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             valuetype [mscorlib]System.Nullable`1<int64> V_3)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000b:  conv.i8
    IL_000c:  ldloca.s   V_1
    IL_000e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0013:  bge.s      IL_0026

    IL_0015:  ldloca.s   V_0
    IL_0017:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001c:  ldloca.s   V_1
    IL_001e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0023:  and
    IL_0024:  br.s       IL_0027

    IL_0026:  ldc.i4.0
    IL_0027:  brfalse.s  IL_002e

    IL_0029:  call       void [mscorlib]System.Console::WriteLine()
    IL_002e:  ldarg.0
    IL_002f:  stloc.2
    IL_0030:  ldarg.1
    IL_0031:  stloc.3
    IL_0032:  ldloca.s   V_2
    IL_0034:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0039:  conv.i8
    IL_003a:  ldloca.s   V_3
    IL_003c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0041:  bne.un.s   IL_0055

    IL_0043:  ldloca.s   V_2
    IL_0045:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_004a:  ldloca.s   V_3
    IL_004c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0051:  ceq
    IL_0053:  br.s       IL_0056

    IL_0055:  ldc.i4.0
    IL_0056:  brfalse.s  IL_005d

    IL_0058:  call       void [mscorlib]System.Console::WriteLine()
    IL_005d:  ret
  } // end of method LiftedOperators::CompareWithImplictCast

  .method public hidebysig static void  CompareWithSignChange(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                              valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       114 (0x72)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<uint32> V_1,
             valuetype [mscorlib]System.Nullable`1<uint32> V_2,
             valuetype [mscorlib]System.Nullable`1<int32> V_3,
             valuetype [mscorlib]System.Nullable`1<uint32> V_4,
             valuetype [mscorlib]System.Nullable`1<uint32> V_5)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brtrue.s   IL_0016

    IL_000b:  ldloca.s   V_1
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_0013:  ldloc.1
    IL_0014:  br.s       IL_0022

    IL_0016:  ldloca.s   V_0
    IL_0018:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_0022:  stloc.2
    IL_0023:  ldarg.1
    IL_0024:  stloc.3
    IL_0025:  ldloca.s   V_3
    IL_0027:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_002c:  brtrue.s   IL_003a

    IL_002e:  ldloca.s   V_4
    IL_0030:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_0036:  ldloc.s    V_4
    IL_0038:  br.s       IL_0046

    IL_003a:  ldloca.s   V_3
    IL_003c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0041:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_0046:  stloc.s    V_5
    IL_0048:  ldloca.s   V_2
    IL_004a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint32>::GetValueOrDefault()
    IL_004f:  ldloca.s   V_5
    IL_0051:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint32>::GetValueOrDefault()
    IL_0056:  bge.un.s   IL_0069

    IL_0058:  ldloca.s   V_2
    IL_005a:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint32>::get_HasValue()
    IL_005f:  ldloca.s   V_5
    IL_0061:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint32>::get_HasValue()
    IL_0066:  and
    IL_0067:  br.s       IL_006a

    IL_0069:  ldc.i4.0
    IL_006a:  brfalse.s  IL_0071

    IL_006c:  call       void [mscorlib]System.Console::WriteLine()
    IL_0071:  ret
  } // end of method LiftedOperators::CompareWithSignChange

  .method public hidebysig static void  StructBasic(valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> a,
                                                    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> b) cil managed
  {
    // Code size       509 (0x1fd)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_0,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_1,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_2,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_3,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_4,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_5,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_6,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_7,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_8,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_9,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_10,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_11,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_12,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_13,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_14,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_15,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_16,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_17)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_000b:  ldloca.s   V_1
    IL_000d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0012:  bne.un.s   IL_0035

    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_001b:  brfalse.s  IL_0032

    IL_001d:  ldloca.s   V_0
    IL_001f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0024:  ldloca.s   V_1
    IL_0026:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_002b:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Equality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0030:  br.s       IL_0036

    IL_0032:  ldc.i4.1
    IL_0033:  br.s       IL_0036

    IL_0035:  ldc.i4.0
    IL_0036:  brfalse.s  IL_003d

    IL_0038:  call       void [mscorlib]System.Console::WriteLine()
    IL_003d:  ldarg.0
    IL_003e:  stloc.2
    IL_003f:  ldarg.1
    IL_0040:  stloc.3
    IL_0041:  ldloca.s   V_2
    IL_0043:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0048:  ldloca.s   V_3
    IL_004a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_004f:  bne.un.s   IL_0072

    IL_0051:  ldloca.s   V_2
    IL_0053:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0058:  brfalse.s  IL_006f

    IL_005a:  ldloca.s   V_2
    IL_005c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0061:  ldloca.s   V_3
    IL_0063:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0068:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Inequality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_006d:  br.s       IL_0073

    IL_006f:  ldc.i4.0
    IL_0070:  br.s       IL_0073

    IL_0072:  ldc.i4.1
    IL_0073:  brfalse.s  IL_007a

    IL_0075:  call       void [mscorlib]System.Console::WriteLine()
    IL_007a:  ldarg.0
    IL_007b:  stloc.s    V_4
    IL_007d:  ldarg.1
    IL_007e:  stloc.s    V_5
    IL_0080:  ldloca.s   V_4
    IL_0082:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0087:  ldloca.s   V_5
    IL_0089:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_008e:  and
    IL_008f:  brtrue.s   IL_0094

    IL_0091:  ldc.i4.0
    IL_0092:  br.s       IL_00a7

    IL_0094:  ldloca.s   V_4
    IL_0096:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_009b:  ldloca.s   V_5
    IL_009d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00a2:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_GreaterThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_00a7:  brfalse.s  IL_00ae

    IL_00a9:  call       void [mscorlib]System.Console::WriteLine()
    IL_00ae:  ldarg.0
    IL_00af:  stloc.s    V_6
    IL_00b1:  ldarg.1
    IL_00b2:  stloc.s    V_7
    IL_00b4:  ldloca.s   V_6
    IL_00b6:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00bb:  ldloca.s   V_7
    IL_00bd:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00c2:  and
    IL_00c3:  brtrue.s   IL_00c8

    IL_00c5:  ldc.i4.0
    IL_00c6:  br.s       IL_00db

    IL_00c8:  ldloca.s   V_6
    IL_00ca:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00cf:  ldloca.s   V_7
    IL_00d1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00d6:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_LessThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_00db:  brfalse.s  IL_00e2

    IL_00dd:  call       void [mscorlib]System.Console::WriteLine()
    IL_00e2:  ldarg.0
    IL_00e3:  stloc.s    V_8
    IL_00e5:  ldarg.1
    IL_00e6:  stloc.s    V_9
    IL_00e8:  ldloca.s   V_8
    IL_00ea:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00ef:  ldloca.s   V_9
    IL_00f1:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00f6:  and
    IL_00f7:  brtrue.s   IL_00fc

    IL_00f9:  ldc.i4.0
    IL_00fa:  br.s       IL_010f

    IL_00fc:  ldloca.s   V_8
    IL_00fe:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0103:  ldloca.s   V_9
    IL_0105:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_010a:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_GreaterThanOrEqual(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_010f:  brfalse.s  IL_0116

    IL_0111:  call       void [mscorlib]System.Console::WriteLine()
    IL_0116:  ldarg.0
    IL_0117:  stloc.s    V_10
    IL_0119:  ldarg.1
    IL_011a:  stloc.s    V_11
    IL_011c:  ldloca.s   V_10
    IL_011e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0123:  ldloca.s   V_11
    IL_0125:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_012a:  and
    IL_012b:  brtrue.s   IL_0130

    IL_012d:  ldc.i4.0
    IL_012e:  br.s       IL_0143

    IL_0130:  ldloca.s   V_10
    IL_0132:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0137:  ldloca.s   V_11
    IL_0139:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_013e:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_LessThanOrEqual(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                   valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0143:  brfalse.s  IL_014a

    IL_0145:  call       void [mscorlib]System.Console::WriteLine()
    IL_014a:  ldarg.0
    IL_014b:  stloc.s    V_12
    IL_014d:  ldarg.1
    IL_014e:  stloc.s    V_13
    IL_0150:  ldloca.s   V_12
    IL_0152:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0157:  ldloca.s   V_13
    IL_0159:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_015e:  bne.un.s   IL_0181

    IL_0160:  ldloca.s   V_12
    IL_0162:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0167:  brfalse.s  IL_017e

    IL_0169:  ldloca.s   V_12
    IL_016b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0170:  ldloca.s   V_13
    IL_0172:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0177:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Equality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_017c:  br.s       IL_0182

    IL_017e:  ldc.i4.1
    IL_017f:  br.s       IL_0182

    IL_0181:  ldc.i4.0
    IL_0182:  brtrue.s   IL_0189

    IL_0184:  call       void [mscorlib]System.Console::WriteLine()
    IL_0189:  ldarg.0
    IL_018a:  stloc.s    V_14
    IL_018c:  ldarg.1
    IL_018d:  stloc.s    V_15
    IL_018f:  ldloca.s   V_14
    IL_0191:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0196:  ldloca.s   V_15
    IL_0198:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_019d:  bne.un.s   IL_01c0

    IL_019f:  ldloca.s   V_14
    IL_01a1:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01a6:  brfalse.s  IL_01bd

    IL_01a8:  ldloca.s   V_14
    IL_01aa:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_01af:  ldloca.s   V_15
    IL_01b1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_01b6:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Inequality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_01bb:  br.s       IL_01c1

    IL_01bd:  ldc.i4.0
    IL_01be:  br.s       IL_01c1

    IL_01c0:  ldc.i4.1
    IL_01c1:  brtrue.s   IL_01c8

    IL_01c3:  call       void [mscorlib]System.Console::WriteLine()
    IL_01c8:  ldarg.0
    IL_01c9:  stloc.s    V_16
    IL_01cb:  ldarg.1
    IL_01cc:  stloc.s    V_17
    IL_01ce:  ldloca.s   V_16
    IL_01d0:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01d5:  ldloca.s   V_17
    IL_01d7:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01dc:  and
    IL_01dd:  brtrue.s   IL_01e2

    IL_01df:  ldc.i4.0
    IL_01e0:  br.s       IL_01f5

    IL_01e2:  ldloca.s   V_16
    IL_01e4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_01e9:  ldloca.s   V_17
    IL_01eb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_01f0:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_GreaterThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_01f5:  brtrue.s   IL_01fc

    IL_01f7:  call       void [mscorlib]System.Console::WriteLine()
    IL_01fc:  ret
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
    // Code size       1873 (0x751)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_0,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_1,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_2,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_3,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_4,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_5,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_6,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_7,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_8,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_9,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_10,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_11,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_12,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_13,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_14,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_15,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_16,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_17,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_18,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_19,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_20,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_21,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_22,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_23,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_24,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_25,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_26,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_27,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_28,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_29,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_30,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_31,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_32,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_33,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_34,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_35,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_36,
             valuetype [mscorlib]System.Nullable`1<int32> V_37,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_38,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_39,
             valuetype [mscorlib]System.Nullable`1<int32> V_40,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_41,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_42,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_43,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_44,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_45,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_46,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_47,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_48,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_49,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_50,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_51,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_52,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_53,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_54,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_55,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_56,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_57,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_58,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_59,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_60)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_000b:  ldloca.s   V_1
    IL_000d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0012:  bne.un.s   IL_0035

    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_001b:  brfalse.s  IL_0032

    IL_001d:  ldloca.s   V_0
    IL_001f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0024:  ldloca.s   V_1
    IL_0026:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_002b:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Equality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0030:  br.s       IL_0036

    IL_0032:  ldc.i4.1
    IL_0033:  br.s       IL_0036

    IL_0035:  ldc.i4.0
    IL_0036:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_003b:  ldarg.0
    IL_003c:  stloc.2
    IL_003d:  ldarg.1
    IL_003e:  stloc.3
    IL_003f:  ldloca.s   V_2
    IL_0041:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0046:  ldloca.s   V_3
    IL_0048:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_004d:  bne.un.s   IL_0070

    IL_004f:  ldloca.s   V_2
    IL_0051:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0056:  brfalse.s  IL_006d

    IL_0058:  ldloca.s   V_2
    IL_005a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_005f:  ldloca.s   V_3
    IL_0061:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0066:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Inequality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_006b:  br.s       IL_0071

    IL_006d:  ldc.i4.0
    IL_006e:  br.s       IL_0071

    IL_0070:  ldc.i4.1
    IL_0071:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0076:  ldarg.0
    IL_0077:  stloc.s    V_4
    IL_0079:  ldarg.1
    IL_007a:  stloc.s    V_5
    IL_007c:  ldloca.s   V_4
    IL_007e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0083:  ldloca.s   V_5
    IL_0085:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_008a:  and
    IL_008b:  brtrue.s   IL_0090

    IL_008d:  ldc.i4.0
    IL_008e:  br.s       IL_00a3

    IL_0090:  ldloca.s   V_4
    IL_0092:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0097:  ldloca.s   V_5
    IL_0099:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_009e:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_GreaterThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_00a3:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00a8:  ldarg.0
    IL_00a9:  stloc.s    V_6
    IL_00ab:  ldarg.1
    IL_00ac:  stloc.s    V_7
    IL_00ae:  ldloca.s   V_6
    IL_00b0:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00b5:  ldloca.s   V_7
    IL_00b7:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00bc:  bne.un.s   IL_00df

    IL_00be:  ldloca.s   V_6
    IL_00c0:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00c5:  brfalse.s  IL_00dc

    IL_00c7:  ldloca.s   V_6
    IL_00c9:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00ce:  ldloca.s   V_7
    IL_00d0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00d5:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Equality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_00da:  br.s       IL_00e0

    IL_00dc:  ldc.i4.1
    IL_00dd:  br.s       IL_00e0

    IL_00df:  ldc.i4.0
    IL_00e0:  ldc.i4.0
    IL_00e1:  ceq
    IL_00e3:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00e8:  ldarg.0
    IL_00e9:  stloc.s    V_8
    IL_00eb:  ldarg.1
    IL_00ec:  stloc.s    V_9
    IL_00ee:  ldloca.s   V_8
    IL_00f0:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00f5:  ldloca.s   V_9
    IL_00f7:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00fc:  bne.un.s   IL_011f

    IL_00fe:  ldloca.s   V_8
    IL_0100:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0105:  brfalse.s  IL_011c

    IL_0107:  ldloca.s   V_8
    IL_0109:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_010e:  ldloca.s   V_9
    IL_0110:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0115:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Inequality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_011a:  br.s       IL_0120

    IL_011c:  ldc.i4.0
    IL_011d:  br.s       IL_0120

    IL_011f:  ldc.i4.1
    IL_0120:  ldc.i4.0
    IL_0121:  ceq
    IL_0123:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0128:  ldarg.0
    IL_0129:  stloc.s    V_10
    IL_012b:  ldarg.1
    IL_012c:  stloc.s    V_11
    IL_012e:  ldloca.s   V_10
    IL_0130:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0135:  ldloca.s   V_11
    IL_0137:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_013c:  and
    IL_013d:  brtrue.s   IL_0142

    IL_013f:  ldc.i4.0
    IL_0140:  br.s       IL_0155

    IL_0142:  ldloca.s   V_10
    IL_0144:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0149:  ldloca.s   V_11
    IL_014b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0150:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_GreaterThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0155:  ldc.i4.0
    IL_0156:  ceq
    IL_0158:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_015d:  ldarg.0
    IL_015e:  stloc.s    V_12
    IL_0160:  ldarg.1
    IL_0161:  stloc.s    V_13
    IL_0163:  ldloca.s   V_12
    IL_0165:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_016a:  ldloca.s   V_13
    IL_016c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0171:  and
    IL_0172:  brtrue.s   IL_0180

    IL_0174:  ldloca.s   V_14
    IL_0176:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_017c:  ldloc.s    V_14
    IL_017e:  br.s       IL_0198

    IL_0180:  ldloca.s   V_12
    IL_0182:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0187:  ldloca.s   V_13
    IL_0189:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_018e:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Addition(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0193:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0198:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_019d:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01a2:  ldarg.0
    IL_01a3:  stloc.s    V_15
    IL_01a5:  ldarg.1
    IL_01a6:  stloc.s    V_16
    IL_01a8:  ldloca.s   V_15
    IL_01aa:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01af:  ldloca.s   V_16
    IL_01b1:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01b6:  and
    IL_01b7:  brtrue.s   IL_01c5

    IL_01b9:  ldloca.s   V_17
    IL_01bb:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_01c1:  ldloc.s    V_17
    IL_01c3:  br.s       IL_01dd

    IL_01c5:  ldloca.s   V_15
    IL_01c7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_01cc:  ldloca.s   V_16
    IL_01ce:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_01d3:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Subtraction(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_01d8:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_01dd:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_01e2:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01e7:  ldarg.0
    IL_01e8:  stloc.s    V_18
    IL_01ea:  ldarg.1
    IL_01eb:  stloc.s    V_19
    IL_01ed:  ldloca.s   V_18
    IL_01ef:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01f4:  ldloca.s   V_19
    IL_01f6:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01fb:  and
    IL_01fc:  brtrue.s   IL_020a

    IL_01fe:  ldloca.s   V_20
    IL_0200:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0206:  ldloc.s    V_20
    IL_0208:  br.s       IL_0222

    IL_020a:  ldloca.s   V_18
    IL_020c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0211:  ldloca.s   V_19
    IL_0213:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0218:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Multiply(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_021d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0222:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0227:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_022c:  ldarg.0
    IL_022d:  stloc.s    V_21
    IL_022f:  ldarg.1
    IL_0230:  stloc.s    V_22
    IL_0232:  ldloca.s   V_21
    IL_0234:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0239:  ldloca.s   V_22
    IL_023b:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0240:  and
    IL_0241:  brtrue.s   IL_024f

    IL_0243:  ldloca.s   V_23
    IL_0245:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_024b:  ldloc.s    V_23
    IL_024d:  br.s       IL_0267

    IL_024f:  ldloca.s   V_21
    IL_0251:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0256:  ldloca.s   V_22
    IL_0258:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_025d:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Division(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0262:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0267:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_026c:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0271:  ldarg.0
    IL_0272:  stloc.s    V_24
    IL_0274:  ldarg.1
    IL_0275:  stloc.s    V_25
    IL_0277:  ldloca.s   V_24
    IL_0279:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_027e:  ldloca.s   V_25
    IL_0280:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0285:  and
    IL_0286:  brtrue.s   IL_0294

    IL_0288:  ldloca.s   V_26
    IL_028a:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0290:  ldloc.s    V_26
    IL_0292:  br.s       IL_02ac

    IL_0294:  ldloca.s   V_24
    IL_0296:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_029b:  ldloca.s   V_25
    IL_029d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_02a2:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Modulus(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                 valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_02a7:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_02ac:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_02b1:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02b6:  ldarg.0
    IL_02b7:  stloc.s    V_27
    IL_02b9:  ldarg.1
    IL_02ba:  stloc.s    V_28
    IL_02bc:  ldloca.s   V_27
    IL_02be:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_02c3:  ldloca.s   V_28
    IL_02c5:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_02ca:  and
    IL_02cb:  brtrue.s   IL_02d9

    IL_02cd:  ldloca.s   V_29
    IL_02cf:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_02d5:  ldloc.s    V_29
    IL_02d7:  br.s       IL_02f1

    IL_02d9:  ldloca.s   V_27
    IL_02db:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_02e0:  ldloca.s   V_28
    IL_02e2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_02e7:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                    valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_02ec:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_02f1:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_02f6:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02fb:  ldarg.0
    IL_02fc:  stloc.s    V_30
    IL_02fe:  ldarg.1
    IL_02ff:  stloc.s    V_31
    IL_0301:  ldloca.s   V_30
    IL_0303:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0308:  ldloca.s   V_31
    IL_030a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_030f:  and
    IL_0310:  brtrue.s   IL_031e

    IL_0312:  ldloca.s   V_32
    IL_0314:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_031a:  ldloc.s    V_32
    IL_031c:  br.s       IL_0336

    IL_031e:  ldloca.s   V_30
    IL_0320:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0325:  ldloca.s   V_31
    IL_0327:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_032c:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                   valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0331:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0336:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_033b:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0340:  ldarg.0
    IL_0341:  stloc.s    V_33
    IL_0343:  ldarg.1
    IL_0344:  stloc.s    V_34
    IL_0346:  ldloca.s   V_33
    IL_0348:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_034d:  ldloca.s   V_34
    IL_034f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0354:  and
    IL_0355:  brtrue.s   IL_0363

    IL_0357:  ldloca.s   V_35
    IL_0359:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_035f:  ldloc.s    V_35
    IL_0361:  br.s       IL_037b

    IL_0363:  ldloca.s   V_33
    IL_0365:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_036a:  ldloca.s   V_34
    IL_036c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0371:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_ExclusiveOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0376:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_037b:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0380:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0385:  ldarg.0
    IL_0386:  stloc.s    V_36
    IL_0388:  ldarg.2
    IL_0389:  stloc.s    V_37
    IL_038b:  ldloca.s   V_36
    IL_038d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0392:  ldloca.s   V_37
    IL_0394:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0399:  and
    IL_039a:  brtrue.s   IL_03a8

    IL_039c:  ldloca.s   V_38
    IL_039e:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_03a4:  ldloc.s    V_38
    IL_03a6:  br.s       IL_03c0

    IL_03a8:  ldloca.s   V_36
    IL_03aa:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_03af:  ldloca.s   V_37
    IL_03b1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03b6:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_LeftShift(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                   int32)
    IL_03bb:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_03c0:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_03c5:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_03ca:  ldarg.0
    IL_03cb:  stloc.s    V_39
    IL_03cd:  ldarg.2
    IL_03ce:  stloc.s    V_40
    IL_03d0:  ldloca.s   V_39
    IL_03d2:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_03d7:  ldloca.s   V_40
    IL_03d9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03de:  and
    IL_03df:  brtrue.s   IL_03ed

    IL_03e1:  ldloca.s   V_41
    IL_03e3:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_03e9:  ldloc.s    V_41
    IL_03eb:  br.s       IL_0405

    IL_03ed:  ldloca.s   V_39
    IL_03ef:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_03f4:  ldloca.s   V_40
    IL_03f6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03fb:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_RightShift(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                    int32)
    IL_0400:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0405:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_040a:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_040f:  ldarg.0
    IL_0410:  stloc.s    V_42
    IL_0412:  ldloca.s   V_42
    IL_0414:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0419:  brtrue.s   IL_041e

    IL_041b:  ldarg.1
    IL_041c:  br.s       IL_042a

    IL_041e:  ldloca.s   V_42
    IL_0420:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0425:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_042a:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_042f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0434:  ldarg.0
    IL_0435:  stloc.s    V_43
    IL_0437:  ldloca.s   V_43
    IL_0439:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_043e:  brtrue.s   IL_044c

    IL_0440:  ldloca.s   V_44
    IL_0442:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0448:  ldloc.s    V_44
    IL_044a:  br.s       IL_045d

    IL_044c:  ldloca.s   V_43
    IL_044e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0453:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_UnaryPlus(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0458:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_045d:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0462:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0467:  ldarg.0
    IL_0468:  stloc.s    V_45
    IL_046a:  ldloca.s   V_45
    IL_046c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0471:  brtrue.s   IL_047f

    IL_0473:  ldloca.s   V_46
    IL_0475:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_047b:  ldloc.s    V_46
    IL_047d:  br.s       IL_0490

    IL_047f:  ldloca.s   V_45
    IL_0481:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0486:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_UnaryNegation(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_048b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0490:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0495:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_049a:  ldarg.0
    IL_049b:  stloc.s    V_47
    IL_049d:  ldloca.s   V_47
    IL_049f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_04a4:  brtrue.s   IL_04b2

    IL_04a6:  ldloca.s   V_48
    IL_04a8:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_04ae:  ldloc.s    V_48
    IL_04b0:  br.s       IL_04c3

    IL_04b2:  ldloca.s   V_47
    IL_04b4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_04b9:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_LogicalNot(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_04be:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_04c3:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_04c8:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_04cd:  ldarg.0
    IL_04ce:  stloc.s    V_49
    IL_04d0:  ldloca.s   V_49
    IL_04d2:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_04d7:  brtrue.s   IL_04e5

    IL_04d9:  ldloca.s   V_50
    IL_04db:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_04e1:  ldloc.s    V_50
    IL_04e3:  br.s       IL_04f6

    IL_04e5:  ldloca.s   V_49
    IL_04e7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_04ec:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_OnesComplement(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_04f1:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_04f6:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_04fb:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0500:  ldarg.0
    IL_0501:  stloc.s    V_51
    IL_0503:  ldarg.1
    IL_0504:  stloc.s    V_52
    IL_0506:  ldloca.s   V_51
    IL_0508:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_050d:  ldloca.s   V_52
    IL_050f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0514:  and
    IL_0515:  brtrue.s   IL_0523

    IL_0517:  ldloca.s   V_53
    IL_0519:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_051f:  ldloc.s    V_53
    IL_0521:  br.s       IL_053b

    IL_0523:  ldloca.s   V_51
    IL_0525:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_052a:  ldloca.s   V_52
    IL_052c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0531:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Addition(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0536:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_053b:  starg.s    a
    IL_053d:  ldarg.0
    IL_053e:  stloc.s    V_54
    IL_0540:  ldarg.1
    IL_0541:  stloc.s    V_55
    IL_0543:  ldloca.s   V_54
    IL_0545:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_054a:  ldloca.s   V_55
    IL_054c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0551:  and
    IL_0552:  brtrue.s   IL_0560

    IL_0554:  ldloca.s   V_56
    IL_0556:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_055c:  ldloc.s    V_56
    IL_055e:  br.s       IL_0578

    IL_0560:  ldloca.s   V_54
    IL_0562:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0567:  ldloca.s   V_55
    IL_0569:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_056e:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Subtraction(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0573:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0578:  starg.s    a
    IL_057a:  ldarg.0
    IL_057b:  stloc.s    V_57
    IL_057d:  ldarg.1
    IL_057e:  stloc.s    V_58
    IL_0580:  ldloca.s   V_57
    IL_0582:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0587:  ldloca.s   V_58
    IL_0589:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_058e:  and
    IL_058f:  brtrue.s   IL_059d

    IL_0591:  ldloca.s   V_59
    IL_0593:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0599:  ldloc.s    V_59
    IL_059b:  br.s       IL_05b5

    IL_059d:  ldloca.s   V_57
    IL_059f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_05a4:  ldloca.s   V_58
    IL_05a6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_05ab:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Multiply(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_05b0:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_05b5:  starg.s    a
    IL_05b7:  ldarg.0
    IL_05b8:  stloc.s    V_60
    IL_05ba:  ldarg.1
    IL_05bb:  stloc.0
    IL_05bc:  ldloca.s   V_60
    IL_05be:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_05c3:  ldloca.s   V_0
    IL_05c5:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_05ca:  and
    IL_05cb:  brtrue.s   IL_05d8

    IL_05cd:  ldloca.s   V_1
    IL_05cf:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_05d5:  ldloc.1
    IL_05d6:  br.s       IL_05f0

    IL_05d8:  ldloca.s   V_60
    IL_05da:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_05df:  ldloca.s   V_0
    IL_05e1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_05e6:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Division(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_05eb:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_05f0:  starg.s    a
    IL_05f2:  ldarg.0
    IL_05f3:  stloc.0
    IL_05f4:  ldarg.1
    IL_05f5:  stloc.1
    IL_05f6:  ldloca.s   V_0
    IL_05f8:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_05fd:  ldloca.s   V_1
    IL_05ff:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0604:  and
    IL_0605:  brtrue.s   IL_0612

    IL_0607:  ldloca.s   V_2
    IL_0609:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_060f:  ldloc.2
    IL_0610:  br.s       IL_062a

    IL_0612:  ldloca.s   V_0
    IL_0614:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0619:  ldloca.s   V_1
    IL_061b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0620:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Modulus(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                 valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0625:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_062a:  starg.s    a
    IL_062c:  ldarg.0
    IL_062d:  stloc.0
    IL_062e:  ldarg.1
    IL_062f:  stloc.1
    IL_0630:  ldloca.s   V_0
    IL_0632:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0637:  ldloca.s   V_1
    IL_0639:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_063e:  and
    IL_063f:  brtrue.s   IL_064c

    IL_0641:  ldloca.s   V_2
    IL_0643:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0649:  ldloc.2
    IL_064a:  br.s       IL_0664

    IL_064c:  ldloca.s   V_0
    IL_064e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0653:  ldloca.s   V_1
    IL_0655:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_065a:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                    valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_065f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0664:  starg.s    a
    IL_0666:  ldarg.0
    IL_0667:  stloc.0
    IL_0668:  ldarg.1
    IL_0669:  stloc.1
    IL_066a:  ldloca.s   V_0
    IL_066c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0671:  ldloca.s   V_1
    IL_0673:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0678:  and
    IL_0679:  brtrue.s   IL_0686

    IL_067b:  ldloca.s   V_2
    IL_067d:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0683:  ldloc.2
    IL_0684:  br.s       IL_069e

    IL_0686:  ldloca.s   V_0
    IL_0688:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_068d:  ldloca.s   V_1
    IL_068f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0694:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                   valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0699:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_069e:  starg.s    a
    IL_06a0:  ldarg.0
    IL_06a1:  stloc.0
    IL_06a2:  ldarg.1
    IL_06a3:  stloc.1
    IL_06a4:  ldloca.s   V_0
    IL_06a6:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_06ab:  ldloca.s   V_1
    IL_06ad:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_06b2:  and
    IL_06b3:  brtrue.s   IL_06c0

    IL_06b5:  ldloca.s   V_2
    IL_06b7:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_06bd:  ldloc.2
    IL_06be:  br.s       IL_06d8

    IL_06c0:  ldloca.s   V_0
    IL_06c2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_06c7:  ldloca.s   V_1
    IL_06c9:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_06ce:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_ExclusiveOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_06d3:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_06d8:  starg.s    a
    IL_06da:  ldarg.0
    IL_06db:  stloc.0
    IL_06dc:  ldarg.2
    IL_06dd:  stloc.s    V_37
    IL_06df:  ldloca.s   V_0
    IL_06e1:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_06e6:  ldloca.s   V_37
    IL_06e8:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_06ed:  and
    IL_06ee:  brtrue.s   IL_06fb

    IL_06f0:  ldloca.s   V_1
    IL_06f2:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_06f8:  ldloc.1
    IL_06f9:  br.s       IL_0713

    IL_06fb:  ldloca.s   V_0
    IL_06fd:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0702:  ldloca.s   V_37
    IL_0704:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0709:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_LeftShift(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                   int32)
    IL_070e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0713:  starg.s    a
    IL_0715:  ldarg.0
    IL_0716:  stloc.0
    IL_0717:  ldarg.2
    IL_0718:  stloc.s    V_37
    IL_071a:  ldloca.s   V_0
    IL_071c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0721:  ldloca.s   V_37
    IL_0723:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0728:  and
    IL_0729:  brtrue.s   IL_0736

    IL_072b:  ldloca.s   V_1
    IL_072d:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0733:  ldloc.1
    IL_0734:  br.s       IL_074e

    IL_0736:  ldloca.s   V_0
    IL_0738:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_073d:  ldloca.s   V_37
    IL_073f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0744:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_RightShift(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                    int32)
    IL_0749:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_074e:  starg.s    a
    IL_0750:  ret
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
    IL_0012:  bne.un.s   IL_0025

    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001b:  ldloca.s   V_1
    IL_001d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0022:  ceq
    IL_0024:  ret

    IL_0025:  ldc.i4.0
    IL_0026:  ret
  } // end of method LiftedOperators::RetEq

  .method public hidebysig static bool  RetEqConv(valuetype [mscorlib]System.Nullable`1<int64> a,
                                                  valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       40 (0x28)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int64> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_000b:  ldloca.s   V_1
    IL_000d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0012:  conv.i8
    IL_0013:  bne.un.s   IL_0026

    IL_0015:  ldloca.s   V_0
    IL_0017:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_001c:  ldloca.s   V_1
    IL_001e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0023:  ceq
    IL_0025:  ret

    IL_0026:  ldc.i4.0
    IL_0027:  ret
  } // end of method LiftedOperators::RetEqConv

  .method public hidebysig static bool  RetEqConst(valuetype [mscorlib]System.Nullable`1<int64> a) cil managed
  {
    // Code size       24 (0x18)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int64> V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0009:  ldc.i4.s   10
    IL_000b:  conv.i8
    IL_000c:  bne.un.s   IL_0016

    IL_000e:  ldloca.s   V_0
    IL_0010:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0015:  ret

    IL_0016:  ldc.i4.0
    IL_0017:  ret
  } // end of method LiftedOperators::RetEqConst

  .method public hidebysig static bool  RetIneqConst(valuetype [mscorlib]System.Nullable`1<int64> a) cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int64> V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0009:  ldc.i4.s   10
    IL_000b:  conv.i8
    IL_000c:  bne.un.s   IL_0019

    IL_000e:  ldloca.s   V_0
    IL_0010:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0015:  ldc.i4.0
    IL_0016:  ceq
    IL_0018:  ret

    IL_0019:  ldc.i4.1
    IL_001a:  ret
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
    IL_0012:  bge.s      IL_0024

    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001b:  ldloca.s   V_1
    IL_001d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0022:  and
    IL_0023:  ret

    IL_0024:  ldc.i4.0
    IL_0025:  ret
  } // end of method LiftedOperators::RetLt

  .method public hidebysig static bool  RetLtConst(valuetype [mscorlib]System.Nullable`1<int32> a) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0009:  ldc.i4.s   10
    IL_000b:  bge.s      IL_0015

    IL_000d:  ldloca.s   V_0
    IL_000f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0014:  ret

    IL_0015:  ldc.i4.0
    IL_0016:  ret
  } // end of method LiftedOperators::RetLtConst

  .method public hidebysig static bool  RetLtConv(valuetype [mscorlib]System.Nullable`1<int64> a,
                                                  valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       39 (0x27)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int64> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_000b:  ldloca.s   V_1
    IL_000d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0012:  conv.i8
    IL_0013:  bge.s      IL_0025

    IL_0015:  ldloca.s   V_0
    IL_0017:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_001c:  ldloca.s   V_1
    IL_001e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0023:  and
    IL_0024:  ret

    IL_0025:  ldc.i4.0
    IL_0026:  ret
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
    IL_0012:  bge.s      IL_0025

    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001b:  ldloca.s   V_1
    IL_001d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0022:  and
    IL_0023:  br.s       IL_0026

    IL_0025:  ldc.i4.0
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
    // Code size       71 (0x47)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<uint32> V_0,
             valuetype [mscorlib]System.Nullable`1<uint32> V_1,
             valuetype [mscorlib]System.Nullable`1<uint32> V_2,
             valuetype [mscorlib]System.Nullable`1<int64> V_3)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint32>::get_HasValue()
    IL_0009:  brtrue.s   IL_0016

    IL_000b:  ldloca.s   V_1
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_0013:  ldloc.1
    IL_0014:  br.s       IL_0025

    IL_0016:  ldc.i4.s   100
    IL_0018:  ldloca.s   V_0
    IL_001a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint32>::GetValueOrDefault()
    IL_001f:  add
    IL_0020:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_0025:  stloc.2
    IL_0026:  ldloca.s   V_2
    IL_0028:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint32>::get_HasValue()
    IL_002d:  brtrue.s   IL_0039

    IL_002f:  ldloca.s   V_3
    IL_0031:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0037:  ldloc.3
    IL_0038:  ret

    IL_0039:  ldloca.s   V_2
    IL_003b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint32>::GetValueOrDefault()
    IL_0040:  conv.u8
    IL_0041:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0046:  ret
  } // end of method LiftedImplicitConversions::AfterArithmetic

  .method public hidebysig static valuetype [mscorlib]System.Nullable`1<int64> 
          InArithmetic3(valuetype [mscorlib]System.Nullable`1<int32> a,
                        valuetype [mscorlib]System.Nullable`1<int64> b,
                        valuetype [mscorlib]System.Nullable`1<int32> c,
                        int64 d) cil managed
  {
    // Code size       148 (0x94)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int64> V_1,
             valuetype [mscorlib]System.Nullable`1<int64> V_2,
             valuetype [mscorlib]System.Nullable`1<int64> V_3,
             valuetype [mscorlib]System.Nullable`1<int32> V_4,
             valuetype [mscorlib]System.Nullable`1<int64> V_5,
             valuetype [mscorlib]System.Nullable`1<int64> V_6,
             int64 V_7,
             valuetype [mscorlib]System.Nullable`1<int64> V_8)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldarg.1
    IL_0003:  stloc.1
    IL_0004:  ldloca.s   V_0
    IL_0006:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000b:  ldloca.s   V_1
    IL_000d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0012:  and
    IL_0013:  brtrue.s   IL_0020

    IL_0015:  ldloca.s   V_2
    IL_0017:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_001d:  ldloc.2
    IL_001e:  br.s       IL_0035

    IL_0020:  ldloca.s   V_0
    IL_0022:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0027:  conv.i8
    IL_0028:  ldloca.s   V_1
    IL_002a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_002f:  add
    IL_0030:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0035:  stloc.3
    IL_0036:  ldarg.2
    IL_0037:  stloc.s    V_4
    IL_0039:  ldloca.s   V_3
    IL_003b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0040:  ldloca.s   V_4
    IL_0042:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0047:  and
    IL_0048:  brtrue.s   IL_0056

    IL_004a:  ldloca.s   V_5
    IL_004c:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0052:  ldloc.s    V_5
    IL_0054:  br.s       IL_006b

    IL_0056:  ldloca.s   V_3
    IL_0058:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_005d:  ldloca.s   V_4
    IL_005f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0064:  conv.i8
    IL_0065:  add
    IL_0066:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_006b:  stloc.s    V_6
    IL_006d:  ldarg.3
    IL_006e:  stloc.s    V_7
    IL_0070:  ldloca.s   V_6
    IL_0072:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0077:  brtrue.s   IL_0084

    IL_0079:  ldloca.s   V_8
    IL_007b:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0081:  ldloc.s    V_8
    IL_0083:  ret

    IL_0084:  ldloca.s   V_6
    IL_0086:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_008b:  ldloc.s    V_7
    IL_008d:  add
    IL_008e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0093:  ret
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
    // Code size       207 (0xcf)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<uint8> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             valuetype [mscorlib]System.Nullable`1<int16> V_3,
             valuetype [mscorlib]System.Nullable`1<int32> V_4,
             valuetype [mscorlib]System.Nullable`1<uint32> V_5,
             valuetype [mscorlib]System.Nullable`1<int64> V_6,
             valuetype [mscorlib]System.Nullable`1<uint32> V_7,
             valuetype [mscorlib]System.Nullable`1<float32> V_8,
             valuetype [mscorlib]System.Nullable`1<uint32> V_9)
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
    IL_0029:  stloc.2
    IL_002a:  ldloca.s   V_2
    IL_002c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0031:  brtrue.s   IL_003e

    IL_0033:  ldloca.s   V_3
    IL_0035:  initobj    valuetype [mscorlib]System.Nullable`1<int16>
    IL_003b:  ldloc.3
    IL_003c:  br.s       IL_004b

    IL_003e:  ldloca.s   V_2
    IL_0040:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0045:  conv.i2
    IL_0046:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int16>::.ctor(!0)
    IL_004b:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<int16>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_0050:  ldarg.0
    IL_0051:  stloc.s    V_4
    IL_0053:  ldloca.s   V_4
    IL_0055:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_005a:  brtrue.s   IL_0068

    IL_005c:  ldloca.s   V_5
    IL_005e:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_0064:  ldloc.s    V_5
    IL_0066:  br.s       IL_0074

    IL_0068:  ldloca.s   V_4
    IL_006a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_006f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_0074:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_0079:  ldarg.1
    IL_007a:  stloc.s    V_6
    IL_007c:  ldloca.s   V_6
    IL_007e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0083:  brtrue.s   IL_0091

    IL_0085:  ldloca.s   V_7
    IL_0087:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_008d:  ldloc.s    V_7
    IL_008f:  br.s       IL_009e

    IL_0091:  ldloca.s   V_6
    IL_0093:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0098:  conv.u4
    IL_0099:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_009e:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_00a3:  ldarg.2
    IL_00a4:  stloc.s    V_8
    IL_00a6:  ldloca.s   V_8
    IL_00a8:  call       instance bool valuetype [mscorlib]System.Nullable`1<float32>::get_HasValue()
    IL_00ad:  brtrue.s   IL_00bb

    IL_00af:  ldloca.s   V_9
    IL_00b1:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_00b7:  ldloc.s    V_9
    IL_00b9:  br.s       IL_00c9

    IL_00bb:  ldloca.s   V_8
    IL_00bd:  call       instance !0 valuetype [mscorlib]System.Nullable`1<float32>::GetValueOrDefault()
    IL_00c2:  conv.r4
    IL_00c3:  conv.u4
    IL_00c4:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_00c9:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_00ce:  ret
  } // end of method LiftedExplicitConversions::UncheckedCasts

  .method public hidebysig static void  CheckedCasts(valuetype [mscorlib]System.Nullable`1<int32> i4,
                                                     valuetype [mscorlib]System.Nullable`1<int64> i8,
                                                     valuetype [mscorlib]System.Nullable`1<float32> f) cil managed
  {
    // Code size       165 (0xa5)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<uint8> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             valuetype [mscorlib]System.Nullable`1<int16> V_3,
             valuetype [mscorlib]System.Nullable`1<int32> V_4,
             valuetype [mscorlib]System.Nullable`1<uint32> V_5,
             valuetype [mscorlib]System.Nullable`1<int64> V_6,
             valuetype [mscorlib]System.Nullable`1<uint32> V_7)
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
    IL_0029:  stloc.2
    IL_002a:  ldloca.s   V_2
    IL_002c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0031:  brtrue.s   IL_003e

    IL_0033:  ldloca.s   V_3
    IL_0035:  initobj    valuetype [mscorlib]System.Nullable`1<int16>
    IL_003b:  ldloc.3
    IL_003c:  br.s       IL_004b

    IL_003e:  ldloca.s   V_2
    IL_0040:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0045:  conv.ovf.i2
    IL_0046:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int16>::.ctor(!0)
    IL_004b:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<int16>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_0050:  ldarg.0
    IL_0051:  stloc.s    V_4
    IL_0053:  ldloca.s   V_4
    IL_0055:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_005a:  brtrue.s   IL_0068

    IL_005c:  ldloca.s   V_5
    IL_005e:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_0064:  ldloc.s    V_5
    IL_0066:  br.s       IL_0075

    IL_0068:  ldloca.s   V_4
    IL_006a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_006f:  conv.ovf.u4
    IL_0070:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_0075:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_007a:  ldarg.1
    IL_007b:  stloc.s    V_6
    IL_007d:  ldloca.s   V_6
    IL_007f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0084:  brtrue.s   IL_0092

    IL_0086:  ldloca.s   V_7
    IL_0088:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_008e:  ldloc.s    V_7
    IL_0090:  br.s       IL_009f

    IL_0092:  ldloca.s   V_6
    IL_0094:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0099:  conv.ovf.u4
    IL_009a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_009f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_00a4:  ret
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
    // Code size       32 (0x20)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brtrue.s   IL_000e

    IL_000b:  ldarg.1
    IL_000c:  br.s       IL_001a

    IL_000e:  ldloca.s   V_0
    IL_0010:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0015:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_001a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullCoalescingTests::Print<valuetype [mscorlib]System.Nullable`1<int32>>(!!0)
    IL_001f:  ret
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
    // Code size       26 (0x1a)
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
    IL_0014:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0019:  ret
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
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\LiftedOperators.opt.res
