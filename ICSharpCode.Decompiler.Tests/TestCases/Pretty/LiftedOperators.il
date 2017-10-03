
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.18020
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly '35ltrsak'
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module '35ltrsak.dll'
// MVID: {E2680319-4EDA-4756-B327-523DC92BF0AB}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00ED0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedOperators
       extends [mscorlib]System.Object
{
  .method public hidebysig static void  BoolBasic(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                  valuetype [mscorlib]System.Nullable`1<bool> b) cil managed
  {
    // Code size       115 (0x73)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<bool> V_0,
             valuetype [mscorlib]System.Nullable`1<bool> V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldarg.1
    IL_0004:  stloc.1
    IL_0005:  ldloca.s   V_0
    IL_0007:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_000c:  ldloca.s   V_1
    IL_000e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0013:  bne.un.s   IL_0027

    IL_0015:  ldloca.s   V_0
    IL_0017:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001c:  ldloca.s   V_1
    IL_001e:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0023:  ceq
    IL_0025:  br.s       IL_0028

    IL_0027:  ldc.i4.0
    IL_0028:  nop
    IL_0029:  ldc.i4.0
    IL_002a:  ceq
    IL_002c:  stloc.2
    IL_002d:  ldloc.2
    IL_002e:  brtrue.s   IL_0038

    IL_0030:  nop
    IL_0031:  call       void [mscorlib]System.Console::WriteLine()
    IL_0036:  nop
    IL_0037:  nop
    IL_0038:  ldarg.0
    IL_0039:  stloc.0
    IL_003a:  ldarg.1
    IL_003b:  stloc.1
    IL_003c:  ldloca.s   V_0
    IL_003e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0043:  ldloca.s   V_1
    IL_0045:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_004a:  bne.un.s   IL_0061

    IL_004c:  ldloca.s   V_0
    IL_004e:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0053:  ldloca.s   V_1
    IL_0055:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_005a:  ceq
    IL_005c:  ldc.i4.0
    IL_005d:  ceq
    IL_005f:  br.s       IL_0062

    IL_0061:  ldc.i4.1
    IL_0062:  nop
    IL_0063:  ldc.i4.0
    IL_0064:  ceq
    IL_0066:  stloc.2
    IL_0067:  ldloc.2
    IL_0068:  brtrue.s   IL_0072

    IL_006a:  nop
    IL_006b:  call       void [mscorlib]System.Console::WriteLine()
    IL_0070:  nop
    IL_0071:  nop
    IL_0072:  ret
  } // end of method LiftedOperators::BoolBasic

  .method public hidebysig static void  BoolComplex(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                    class [mscorlib]System.Func`1<bool> x) cil managed
  {
    // Code size       188 (0xbc)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<bool> V_0,
             bool V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldarg.1
    IL_0004:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0009:  stloc.1
    IL_000a:  ldloca.s   V_0
    IL_000c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0011:  ldloc.1
    IL_0012:  bne.un.s   IL_001d

    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001b:  br.s       IL_001e

    IL_001d:  ldc.i4.0
    IL_001e:  nop
    IL_001f:  ldc.i4.0
    IL_0020:  ceq
    IL_0022:  stloc.2
    IL_0023:  ldloc.2
    IL_0024:  brtrue.s   IL_002e

    IL_0026:  nop
    IL_0027:  call       void [mscorlib]System.Console::WriteLine()
    IL_002c:  nop
    IL_002d:  nop
    IL_002e:  ldarg.0
    IL_002f:  stloc.0
    IL_0030:  ldarg.1
    IL_0031:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0036:  stloc.1
    IL_0037:  ldloca.s   V_0
    IL_0039:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_003e:  ldloc.1
    IL_003f:  bne.un.s   IL_004d

    IL_0041:  ldloca.s   V_0
    IL_0043:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0048:  ldc.i4.0
    IL_0049:  ceq
    IL_004b:  br.s       IL_004e

    IL_004d:  ldc.i4.1
    IL_004e:  nop
    IL_004f:  ldc.i4.0
    IL_0050:  ceq
    IL_0052:  stloc.2
    IL_0053:  ldloc.2
    IL_0054:  brtrue.s   IL_005e

    IL_0056:  nop
    IL_0057:  call       void [mscorlib]System.Console::WriteLine()
    IL_005c:  nop
    IL_005d:  nop
    IL_005e:  ldarg.1
    IL_005f:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0064:  stloc.1
    IL_0065:  ldarg.0
    IL_0066:  stloc.0
    IL_0067:  ldloc.1
    IL_0068:  ldloca.s   V_0
    IL_006a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_006f:  bne.un.s   IL_007a

    IL_0071:  ldloca.s   V_0
    IL_0073:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0078:  br.s       IL_007b

    IL_007a:  ldc.i4.0
    IL_007b:  nop
    IL_007c:  ldc.i4.0
    IL_007d:  ceq
    IL_007f:  stloc.2
    IL_0080:  ldloc.2
    IL_0081:  brtrue.s   IL_008b

    IL_0083:  nop
    IL_0084:  call       void [mscorlib]System.Console::WriteLine()
    IL_0089:  nop
    IL_008a:  nop
    IL_008b:  ldarg.1
    IL_008c:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0091:  stloc.1
    IL_0092:  ldarg.0
    IL_0093:  stloc.0
    IL_0094:  ldloc.1
    IL_0095:  ldloca.s   V_0
    IL_0097:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_009c:  bne.un.s   IL_00aa

    IL_009e:  ldloca.s   V_0
    IL_00a0:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00a5:  ldc.i4.0
    IL_00a6:  ceq
    IL_00a8:  br.s       IL_00ab

    IL_00aa:  ldc.i4.1
    IL_00ab:  nop
    IL_00ac:  ldc.i4.0
    IL_00ad:  ceq
    IL_00af:  stloc.2
    IL_00b0:  ldloc.2
    IL_00b1:  brtrue.s   IL_00bb

    IL_00b3:  nop
    IL_00b4:  call       void [mscorlib]System.Console::WriteLine()
    IL_00b9:  nop
    IL_00ba:  nop
    IL_00bb:  ret
  } // end of method LiftedOperators::BoolComplex

  .method public hidebysig static void  BoolConst(valuetype [mscorlib]System.Nullable`1<bool> a) cil managed
  {
    // Code size       230 (0xe6)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<bool> V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_000a:  brfalse.s  IL_0015

    IL_000c:  ldloca.s   V_0
    IL_000e:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0013:  br.s       IL_0016

    IL_0015:  ldc.i4.0
    IL_0016:  nop
    IL_0017:  ldc.i4.0
    IL_0018:  ceq
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  brtrue.s   IL_0026

    IL_001e:  nop
    IL_001f:  call       void [mscorlib]System.Console::WriteLine()
    IL_0024:  nop
    IL_0025:  nop
    IL_0026:  ldarg.0
    IL_0027:  stloc.0
    IL_0028:  ldloca.s   V_0
    IL_002a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_002f:  brfalse.s  IL_003d

    IL_0031:  ldloca.s   V_0
    IL_0033:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0038:  ldc.i4.0
    IL_0039:  ceq
    IL_003b:  br.s       IL_003e

    IL_003d:  ldc.i4.1
    IL_003e:  nop
    IL_003f:  ldc.i4.0
    IL_0040:  ceq
    IL_0042:  stloc.1
    IL_0043:  ldloc.1
    IL_0044:  brtrue.s   IL_004e

    IL_0046:  nop
    IL_0047:  call       void [mscorlib]System.Console::WriteLine()
    IL_004c:  nop
    IL_004d:  nop
    IL_004e:  ldarg.0
    IL_004f:  stloc.0
    IL_0050:  ldloca.s   V_0
    IL_0052:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0057:  brtrue.s   IL_0062

    IL_0059:  ldloca.s   V_0
    IL_005b:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0060:  br.s       IL_0063

    IL_0062:  ldc.i4.0
    IL_0063:  nop
    IL_0064:  ldc.i4.0
    IL_0065:  ceq
    IL_0067:  stloc.1
    IL_0068:  ldloc.1
    IL_0069:  brtrue.s   IL_0073

    IL_006b:  nop
    IL_006c:  call       void [mscorlib]System.Console::WriteLine()
    IL_0071:  nop
    IL_0072:  nop
    IL_0073:  ldarg.0
    IL_0074:  stloc.0
    IL_0075:  ldloca.s   V_0
    IL_0077:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_007c:  brtrue.s   IL_008a

    IL_007e:  ldloca.s   V_0
    IL_0080:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0085:  ldc.i4.0
    IL_0086:  ceq
    IL_0088:  br.s       IL_008b

    IL_008a:  ldc.i4.1
    IL_008b:  nop
    IL_008c:  ldc.i4.0
    IL_008d:  ceq
    IL_008f:  stloc.1
    IL_0090:  ldloc.1
    IL_0091:  brtrue.s   IL_009b

    IL_0093:  nop
    IL_0094:  call       void [mscorlib]System.Console::WriteLine()
    IL_0099:  nop
    IL_009a:  nop
    IL_009b:  ldarg.0
    IL_009c:  stloc.0
    IL_009d:  ldloca.s   V_0
    IL_009f:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00a4:  brtrue.s   IL_00a9

    IL_00a6:  ldc.i4.1
    IL_00a7:  br.s       IL_00b0

    IL_00a9:  ldloca.s   V_0
    IL_00ab:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00b0:  nop
    IL_00b1:  ldc.i4.0
    IL_00b2:  ceq
    IL_00b4:  stloc.1
    IL_00b5:  ldloc.1
    IL_00b6:  brtrue.s   IL_00c0

    IL_00b8:  nop
    IL_00b9:  call       void [mscorlib]System.Console::WriteLine()
    IL_00be:  nop
    IL_00bf:  nop
    IL_00c0:  ldarg.0
    IL_00c1:  stloc.0
    IL_00c2:  ldloca.s   V_0
    IL_00c4:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00c9:  brtrue.s   IL_00ce

    IL_00cb:  ldc.i4.0
    IL_00cc:  br.s       IL_00d5

    IL_00ce:  ldloca.s   V_0
    IL_00d0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00d5:  nop
    IL_00d6:  ldc.i4.0
    IL_00d7:  ceq
    IL_00d9:  stloc.1
    IL_00da:  ldloc.1
    IL_00db:  brtrue.s   IL_00e5

    IL_00dd:  nop
    IL_00de:  call       void [mscorlib]System.Console::WriteLine()
    IL_00e3:  nop
    IL_00e4:  nop
    IL_00e5:  ret
  } // end of method LiftedOperators::BoolConst

  .method public hidebysig static void  BoolValueBasic(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                       valuetype [mscorlib]System.Nullable`1<bool> b) cil managed
  {
    // Code size       473 (0x1d9)
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
    IL_0013:  bne.un.s   IL_0027

    IL_0015:  ldloca.s   V_0
    IL_0017:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001c:  ldloca.s   V_1
    IL_001e:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0023:  ceq
    IL_0025:  br.s       IL_0028

    IL_0027:  ldc.i4.0
    IL_0028:  nop
    IL_0029:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_002e:  nop
    IL_002f:  ldarg.0
    IL_0030:  stloc.0
    IL_0031:  ldarg.1
    IL_0032:  stloc.1
    IL_0033:  ldloca.s   V_0
    IL_0035:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_003a:  ldloca.s   V_1
    IL_003c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0041:  bne.un.s   IL_0058

    IL_0043:  ldloca.s   V_0
    IL_0045:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_004a:  ldloca.s   V_1
    IL_004c:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0051:  ceq
    IL_0053:  ldc.i4.0
    IL_0054:  ceq
    IL_0056:  br.s       IL_0059

    IL_0058:  ldc.i4.1
    IL_0059:  nop
    IL_005a:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_005f:  nop
    IL_0060:  ldarg.0
    IL_0061:  stloc.0
    IL_0062:  ldarg.1
    IL_0063:  stloc.1
    IL_0064:  ldloca.s   V_0
    IL_0066:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_006b:  brtrue.s   IL_0082

    IL_006d:  ldloca.s   V_1
    IL_006f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0074:  brtrue.s   IL_007f

    IL_0076:  ldloca.s   V_0
    IL_0078:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_007d:  brfalse.s  IL_0082

    IL_007f:  ldloc.0
    IL_0080:  br.s       IL_0083

    IL_0082:  ldloc.1
    IL_0083:  nop
    IL_0084:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0089:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_008e:  nop
    IL_008f:  ldarg.0
    IL_0090:  stloc.0
    IL_0091:  ldarg.1
    IL_0092:  stloc.1
    IL_0093:  ldloca.s   V_0
    IL_0095:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_009a:  brtrue.s   IL_00b1

    IL_009c:  ldloca.s   V_1
    IL_009e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00a3:  brtrue.s   IL_00ae

    IL_00a5:  ldloca.s   V_0
    IL_00a7:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00ac:  brfalse.s  IL_00b1

    IL_00ae:  ldloc.1
    IL_00af:  br.s       IL_00b2

    IL_00b1:  ldloc.0
    IL_00b2:  nop
    IL_00b3:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_00b8:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00bd:  nop
    IL_00be:  ldarg.0
    IL_00bf:  stloc.0
    IL_00c0:  ldarg.1
    IL_00c1:  stloc.1
    IL_00c2:  ldloca.s   V_0
    IL_00c4:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00c9:  ldloca.s   V_1
    IL_00cb:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00d0:  and
    IL_00d1:  brtrue.s   IL_00de

    IL_00d3:  ldloca.s   V_2
    IL_00d5:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_00db:  ldloc.2
    IL_00dc:  br.s       IL_00f2

    IL_00de:  ldloca.s   V_0
    IL_00e0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00e5:  ldloca.s   V_1
    IL_00e7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00ec:  xor
    IL_00ed:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_00f2:  nop
    IL_00f3:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_00f8:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00fd:  nop
    IL_00fe:  ldarg.0
    IL_00ff:  stloc.0
    IL_0100:  ldloca.s   V_0
    IL_0102:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0107:  brtrue.s   IL_010c

    IL_0109:  ldarg.1
    IL_010a:  br.s       IL_0118

    IL_010c:  ldloca.s   V_0
    IL_010e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0113:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0118:  nop
    IL_0119:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_011e:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0123:  nop
    IL_0124:  ldarg.0
    IL_0125:  stloc.0
    IL_0126:  ldloca.s   V_0
    IL_0128:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_012d:  brtrue.s   IL_013a

    IL_012f:  ldloca.s   V_1
    IL_0131:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_0137:  ldloc.1
    IL_0138:  br.s       IL_0149

    IL_013a:  ldloca.s   V_0
    IL_013c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0141:  ldc.i4.0
    IL_0142:  ceq
    IL_0144:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0149:  nop
    IL_014a:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_014f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0154:  nop
    IL_0155:  ldarg.0
    IL_0156:  stloc.0
    IL_0157:  ldarg.1
    IL_0158:  stloc.1
    IL_0159:  ldloca.s   V_0
    IL_015b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0160:  brtrue.s   IL_0177

    IL_0162:  ldloca.s   V_1
    IL_0164:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0169:  brtrue.s   IL_0174

    IL_016b:  ldloca.s   V_0
    IL_016d:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0172:  brfalse.s  IL_0177

    IL_0174:  ldloc.0
    IL_0175:  br.s       IL_0178

    IL_0177:  ldloc.1
    IL_0178:  nop
    IL_0179:  starg.s    a
    IL_017b:  ldarg.0
    IL_017c:  stloc.0
    IL_017d:  ldarg.1
    IL_017e:  stloc.1
    IL_017f:  ldloca.s   V_0
    IL_0181:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0186:  brtrue.s   IL_019d

    IL_0188:  ldloca.s   V_1
    IL_018a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_018f:  brtrue.s   IL_019a

    IL_0191:  ldloca.s   V_0
    IL_0193:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0198:  brfalse.s  IL_019d

    IL_019a:  ldloc.1
    IL_019b:  br.s       IL_019e

    IL_019d:  ldloc.0
    IL_019e:  nop
    IL_019f:  starg.s    a
    IL_01a1:  ldarg.0
    IL_01a2:  stloc.0
    IL_01a3:  ldarg.1
    IL_01a4:  stloc.1
    IL_01a5:  ldloca.s   V_0
    IL_01a7:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_01ac:  ldloca.s   V_1
    IL_01ae:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_01b3:  and
    IL_01b4:  brtrue.s   IL_01c1

    IL_01b6:  ldloca.s   V_2
    IL_01b8:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_01be:  ldloc.2
    IL_01bf:  br.s       IL_01d5

    IL_01c1:  ldloca.s   V_0
    IL_01c3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01c8:  ldloca.s   V_1
    IL_01ca:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01cf:  xor
    IL_01d0:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_01d5:  nop
    IL_01d6:  starg.s    a
    IL_01d8:  ret
  } // end of method LiftedOperators::BoolValueBasic

  .method public hidebysig static void  BoolValueComplex(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                         class [mscorlib]System.Func`1<bool> x) cil managed
  {
    // Code size       544 (0x220)
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
    IL_0012:  bne.un.s   IL_001d

    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001b:  br.s       IL_001e

    IL_001d:  ldc.i4.0
    IL_001e:  nop
    IL_001f:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0024:  nop
    IL_0025:  ldarg.0
    IL_0026:  stloc.0
    IL_0027:  ldarg.1
    IL_0028:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_002d:  stloc.1
    IL_002e:  ldloca.s   V_0
    IL_0030:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0035:  ldloc.1
    IL_0036:  bne.un.s   IL_0044

    IL_0038:  ldloca.s   V_0
    IL_003a:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_003f:  ldc.i4.0
    IL_0040:  ceq
    IL_0042:  br.s       IL_0045

    IL_0044:  ldc.i4.1
    IL_0045:  nop
    IL_0046:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_004b:  nop
    IL_004c:  ldarg.1
    IL_004d:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0052:  stloc.1
    IL_0053:  ldarg.0
    IL_0054:  stloc.0
    IL_0055:  ldloc.1
    IL_0056:  ldloca.s   V_0
    IL_0058:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_005d:  bne.un.s   IL_0068

    IL_005f:  ldloca.s   V_0
    IL_0061:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0066:  br.s       IL_0069

    IL_0068:  ldc.i4.0
    IL_0069:  nop
    IL_006a:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_006f:  nop
    IL_0070:  ldarg.1
    IL_0071:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0076:  stloc.1
    IL_0077:  ldarg.0
    IL_0078:  stloc.0
    IL_0079:  ldloc.1
    IL_007a:  ldloca.s   V_0
    IL_007c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0081:  bne.un.s   IL_008f

    IL_0083:  ldloca.s   V_0
    IL_0085:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_008a:  ldc.i4.0
    IL_008b:  ceq
    IL_008d:  br.s       IL_0090

    IL_008f:  ldc.i4.1
    IL_0090:  nop
    IL_0091:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0096:  nop
    IL_0097:  ldarg.0
    IL_0098:  stloc.0
    IL_0099:  ldarg.1
    IL_009a:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_009f:  stloc.1
    IL_00a0:  ldloca.s   V_0
    IL_00a2:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00a7:  brtrue.s   IL_00b4

    IL_00a9:  ldloca.s   V_2
    IL_00ab:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_00b1:  ldloc.2
    IL_00b2:  br.s       IL_00c2

    IL_00b4:  ldloca.s   V_0
    IL_00b6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00bb:  ldloc.1
    IL_00bc:  xor
    IL_00bd:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_00c2:  nop
    IL_00c3:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_00c8:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00cd:  nop
    IL_00ce:  ldarg.0
    IL_00cf:  stloc.0
    IL_00d0:  ldloca.s   V_0
    IL_00d2:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00d7:  brtrue.s   IL_00e1

    IL_00d9:  ldarg.1
    IL_00da:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_00df:  br.s       IL_00e8

    IL_00e1:  ldloca.s   V_0
    IL_00e3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00e8:  nop
    IL_00e9:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00ee:  nop
    IL_00ef:  ldarg.0
    IL_00f0:  stloc.0
    IL_00f1:  ldarg.1
    IL_00f2:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_00f7:  stloc.1
    IL_00f8:  ldloca.s   V_0
    IL_00fa:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00ff:  brtrue.s   IL_010c

    IL_0101:  ldloca.s   V_2
    IL_0103:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_0109:  ldloc.2
    IL_010a:  br.s       IL_011a

    IL_010c:  ldloca.s   V_0
    IL_010e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0113:  ldloc.1
    IL_0114:  xor
    IL_0115:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_011a:  nop
    IL_011b:  starg.s    a
    IL_011d:  ldarg.1
    IL_011e:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0123:  ldarg.0
    IL_0124:  stloc.0
    IL_0125:  brtrue.s   IL_012f

    IL_0127:  ldc.i4.0
    IL_0128:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_012d:  br.s       IL_0130

    IL_012f:  ldloc.0
    IL_0130:  nop
    IL_0131:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0136:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_013b:  nop
    IL_013c:  ldarg.1
    IL_013d:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0142:  ldarg.0
    IL_0143:  stloc.0
    IL_0144:  brtrue.s   IL_0149

    IL_0146:  ldloc.0
    IL_0147:  br.s       IL_014f

    IL_0149:  ldc.i4.1
    IL_014a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_014f:  nop
    IL_0150:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0155:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_015a:  nop
    IL_015b:  ldarg.1
    IL_015c:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0161:  stloc.1
    IL_0162:  ldarg.0
    IL_0163:  stloc.0
    IL_0164:  ldloca.s   V_0
    IL_0166:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_016b:  brtrue.s   IL_0178

    IL_016d:  ldloca.s   V_2
    IL_016f:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_0175:  ldloc.2
    IL_0176:  br.s       IL_0186

    IL_0178:  ldloc.1
    IL_0179:  ldloca.s   V_0
    IL_017b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0180:  xor
    IL_0181:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0186:  nop
    IL_0187:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_018c:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0191:  nop
    IL_0192:  ldc.i4.0
    IL_0193:  newarr     valuetype [mscorlib]System.Nullable`1<bool>
    IL_0198:  ldc.i4.0
    IL_0199:  ldelema    valuetype [mscorlib]System.Nullable`1<bool>
    IL_019e:  dup
    IL_019f:  ldobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_01a4:  stloc.0
    IL_01a5:  ldarg.1
    IL_01a6:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_01ab:  stloc.1
    IL_01ac:  ldloca.s   V_0
    IL_01ae:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_01b3:  brtrue.s   IL_01c0

    IL_01b5:  ldloca.s   V_2
    IL_01b7:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_01bd:  ldloc.2
    IL_01be:  br.s       IL_01ce

    IL_01c0:  ldloca.s   V_0
    IL_01c2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01c7:  ldloc.1
    IL_01c8:  xor
    IL_01c9:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_01ce:  nop
    IL_01cf:  stobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_01d4:  ldc.i4.0
    IL_01d5:  newarr     valuetype [mscorlib]System.Nullable`1<bool>
    IL_01da:  ldc.i4.0
    IL_01db:  ldelema    valuetype [mscorlib]System.Nullable`1<bool>
    IL_01e0:  dup
    IL_01e1:  ldobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_01e6:  stloc.0
    IL_01e7:  ldarg.0
    IL_01e8:  stloc.2
    IL_01e9:  ldloca.s   V_0
    IL_01eb:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_01f0:  ldloca.s   V_2
    IL_01f2:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_01f7:  and
    IL_01f8:  brtrue.s   IL_0205

    IL_01fa:  ldloca.s   V_3
    IL_01fc:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_0202:  ldloc.3
    IL_0203:  br.s       IL_0219

    IL_0205:  ldloca.s   V_0
    IL_0207:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_020c:  ldloca.s   V_2
    IL_020e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0213:  xor
    IL_0214:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0219:  nop
    IL_021a:  stobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_021f:  ret
  } // end of method LiftedOperators::BoolValueComplex

  .method public hidebysig static void  BoolValueConst(valuetype [mscorlib]System.Nullable`1<bool> a) cil managed
  {
    // Code size       176 (0xb0)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<bool> V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_000a:  brfalse.s  IL_0015

    IL_000c:  ldloca.s   V_0
    IL_000e:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0013:  br.s       IL_0016

    IL_0015:  ldc.i4.0
    IL_0016:  nop
    IL_0017:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_001c:  nop
    IL_001d:  ldarg.0
    IL_001e:  stloc.0
    IL_001f:  ldloca.s   V_0
    IL_0021:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0026:  brfalse.s  IL_0034

    IL_0028:  ldloca.s   V_0
    IL_002a:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_002f:  ldc.i4.0
    IL_0030:  ceq
    IL_0032:  br.s       IL_0035

    IL_0034:  ldc.i4.1
    IL_0035:  nop
    IL_0036:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_003b:  nop
    IL_003c:  ldarg.0
    IL_003d:  stloc.0
    IL_003e:  ldloca.s   V_0
    IL_0040:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0045:  brtrue.s   IL_0050

    IL_0047:  ldloca.s   V_0
    IL_0049:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_004e:  br.s       IL_0051

    IL_0050:  ldc.i4.0
    IL_0051:  nop
    IL_0052:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0057:  nop
    IL_0058:  ldarg.0
    IL_0059:  stloc.0
    IL_005a:  ldloca.s   V_0
    IL_005c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0061:  brtrue.s   IL_006f

    IL_0063:  ldloca.s   V_0
    IL_0065:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_006a:  ldc.i4.0
    IL_006b:  ceq
    IL_006d:  br.s       IL_0070

    IL_006f:  ldc.i4.1
    IL_0070:  nop
    IL_0071:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0076:  nop
    IL_0077:  ldarg.0
    IL_0078:  stloc.0
    IL_0079:  ldloca.s   V_0
    IL_007b:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0080:  brtrue.s   IL_0085

    IL_0082:  ldc.i4.1
    IL_0083:  br.s       IL_008c

    IL_0085:  ldloca.s   V_0
    IL_0087:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_008c:  nop
    IL_008d:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0092:  nop
    IL_0093:  ldarg.0
    IL_0094:  stloc.0
    IL_0095:  ldloca.s   V_0
    IL_0097:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_009c:  brtrue.s   IL_00a1

    IL_009e:  ldc.i4.0
    IL_009f:  br.s       IL_00a8

    IL_00a1:  ldloca.s   V_0
    IL_00a3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00a8:  nop
    IL_00a9:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00ae:  nop
    IL_00af:  ret
  } // end of method LiftedOperators::BoolValueConst

  .method public hidebysig static void  IntBasic(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                 valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       433 (0x1b1)
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
    IL_0013:  bne.un.s   IL_0027

    IL_0015:  ldloca.s   V_0
    IL_0017:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001c:  ldloca.s   V_1
    IL_001e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0023:  ceq
    IL_0025:  br.s       IL_0028

    IL_0027:  ldc.i4.0
    IL_0028:  nop
    IL_0029:  ldc.i4.0
    IL_002a:  ceq
    IL_002c:  stloc.2
    IL_002d:  ldloc.2
    IL_002e:  brtrue.s   IL_0038

    IL_0030:  nop
    IL_0031:  call       void [mscorlib]System.Console::WriteLine()
    IL_0036:  nop
    IL_0037:  nop
    IL_0038:  ldarg.0
    IL_0039:  stloc.0
    IL_003a:  ldarg.1
    IL_003b:  stloc.1
    IL_003c:  ldloca.s   V_0
    IL_003e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0043:  ldloca.s   V_1
    IL_0045:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_004a:  bne.un.s   IL_0061

    IL_004c:  ldloca.s   V_0
    IL_004e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0053:  ldloca.s   V_1
    IL_0055:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_005a:  ceq
    IL_005c:  ldc.i4.0
    IL_005d:  ceq
    IL_005f:  br.s       IL_0062

    IL_0061:  ldc.i4.1
    IL_0062:  nop
    IL_0063:  ldc.i4.0
    IL_0064:  ceq
    IL_0066:  stloc.2
    IL_0067:  ldloc.2
    IL_0068:  brtrue.s   IL_0072

    IL_006a:  nop
    IL_006b:  call       void [mscorlib]System.Console::WriteLine()
    IL_0070:  nop
    IL_0071:  nop
    IL_0072:  ldarg.0
    IL_0073:  stloc.0
    IL_0074:  ldarg.1
    IL_0075:  stloc.1
    IL_0076:  ldloca.s   V_0
    IL_0078:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_007d:  ldloca.s   V_1
    IL_007f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0084:  ble.s      IL_0097

    IL_0086:  ldloca.s   V_0
    IL_0088:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_008d:  ldloca.s   V_1
    IL_008f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0094:  and
    IL_0095:  br.s       IL_0098

    IL_0097:  ldc.i4.0
    IL_0098:  nop
    IL_0099:  ldc.i4.0
    IL_009a:  ceq
    IL_009c:  stloc.2
    IL_009d:  ldloc.2
    IL_009e:  brtrue.s   IL_00a8

    IL_00a0:  nop
    IL_00a1:  call       void [mscorlib]System.Console::WriteLine()
    IL_00a6:  nop
    IL_00a7:  nop
    IL_00a8:  ldarg.0
    IL_00a9:  stloc.0
    IL_00aa:  ldarg.1
    IL_00ab:  stloc.1
    IL_00ac:  ldloca.s   V_0
    IL_00ae:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00b3:  ldloca.s   V_1
    IL_00b5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00ba:  bge.s      IL_00cd

    IL_00bc:  ldloca.s   V_0
    IL_00be:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00c3:  ldloca.s   V_1
    IL_00c5:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ca:  and
    IL_00cb:  br.s       IL_00ce

    IL_00cd:  ldc.i4.0
    IL_00ce:  nop
    IL_00cf:  ldc.i4.0
    IL_00d0:  ceq
    IL_00d2:  stloc.2
    IL_00d3:  ldloc.2
    IL_00d4:  brtrue.s   IL_00de

    IL_00d6:  nop
    IL_00d7:  call       void [mscorlib]System.Console::WriteLine()
    IL_00dc:  nop
    IL_00dd:  nop
    IL_00de:  ldarg.0
    IL_00df:  stloc.0
    IL_00e0:  ldarg.1
    IL_00e1:  stloc.1
    IL_00e2:  ldloca.s   V_0
    IL_00e4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00e9:  ldloca.s   V_1
    IL_00eb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00f0:  blt.s      IL_0103

    IL_00f2:  ldloca.s   V_0
    IL_00f4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00f9:  ldloca.s   V_1
    IL_00fb:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0100:  and
    IL_0101:  br.s       IL_0104

    IL_0103:  ldc.i4.0
    IL_0104:  nop
    IL_0105:  ldc.i4.0
    IL_0106:  ceq
    IL_0108:  stloc.2
    IL_0109:  ldloc.2
    IL_010a:  brtrue.s   IL_0114

    IL_010c:  nop
    IL_010d:  call       void [mscorlib]System.Console::WriteLine()
    IL_0112:  nop
    IL_0113:  nop
    IL_0114:  ldarg.0
    IL_0115:  stloc.0
    IL_0116:  ldarg.1
    IL_0117:  stloc.1
    IL_0118:  ldloca.s   V_0
    IL_011a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_011f:  ldloca.s   V_1
    IL_0121:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0126:  bgt.s      IL_0139

    IL_0128:  ldloca.s   V_0
    IL_012a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_012f:  ldloca.s   V_1
    IL_0131:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0136:  and
    IL_0137:  br.s       IL_013a

    IL_0139:  ldc.i4.0
    IL_013a:  nop
    IL_013b:  ldc.i4.0
    IL_013c:  ceq
    IL_013e:  stloc.2
    IL_013f:  ldloc.2
    IL_0140:  brtrue.s   IL_014a

    IL_0142:  nop
    IL_0143:  call       void [mscorlib]System.Console::WriteLine()
    IL_0148:  nop
    IL_0149:  nop
    IL_014a:  ldarg.0
    IL_014b:  stloc.0
    IL_014c:  ldarg.1
    IL_014d:  stloc.1
    IL_014e:  ldloca.s   V_0
    IL_0150:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0155:  ldloca.s   V_1
    IL_0157:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_015c:  ble.s      IL_016f

    IL_015e:  ldloca.s   V_0
    IL_0160:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0165:  ldloca.s   V_1
    IL_0167:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_016c:  and
    IL_016d:  br.s       IL_0170

    IL_016f:  ldc.i4.0
    IL_0170:  nop
    IL_0171:  stloc.2
    IL_0172:  ldloc.2
    IL_0173:  brtrue.s   IL_017d

    IL_0175:  nop
    IL_0176:  call       void [mscorlib]System.Console::WriteLine()
    IL_017b:  nop
    IL_017c:  nop
    IL_017d:  ldarg.0
    IL_017e:  stloc.0
    IL_017f:  ldarg.1
    IL_0180:  stloc.1
    IL_0181:  ldloca.s   V_0
    IL_0183:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0188:  ldloca.s   V_1
    IL_018a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_018f:  bgt.s      IL_01a2

    IL_0191:  ldloca.s   V_0
    IL_0193:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0198:  ldloca.s   V_1
    IL_019a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_019f:  and
    IL_01a0:  br.s       IL_01a3

    IL_01a2:  ldc.i4.0
    IL_01a3:  nop
    IL_01a4:  stloc.2
    IL_01a5:  ldloc.2
    IL_01a6:  brtrue.s   IL_01b0

    IL_01a8:  nop
    IL_01a9:  call       void [mscorlib]System.Console::WriteLine()
    IL_01ae:  nop
    IL_01af:  nop
    IL_01b0:  ret
  } // end of method LiftedOperators::IntBasic

  .method public hidebysig static void  IntComplex(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                   class [mscorlib]System.Func`1<int32> x) cil managed
  {
    // Code size       362 (0x16a)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             int32 V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldarg.1
    IL_0004:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0009:  stloc.1
    IL_000a:  ldloca.s   V_0
    IL_000c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0011:  ldloc.1
    IL_0012:  bne.un.s   IL_001d

    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001b:  br.s       IL_001e

    IL_001d:  ldc.i4.0
    IL_001e:  nop
    IL_001f:  ldc.i4.0
    IL_0020:  ceq
    IL_0022:  stloc.2
    IL_0023:  ldloc.2
    IL_0024:  brtrue.s   IL_002e

    IL_0026:  nop
    IL_0027:  call       void [mscorlib]System.Console::WriteLine()
    IL_002c:  nop
    IL_002d:  nop
    IL_002e:  ldarg.0
    IL_002f:  stloc.0
    IL_0030:  ldarg.1
    IL_0031:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0036:  stloc.1
    IL_0037:  ldloca.s   V_0
    IL_0039:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_003e:  ldloc.1
    IL_003f:  bne.un.s   IL_004d

    IL_0041:  ldloca.s   V_0
    IL_0043:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0048:  ldc.i4.0
    IL_0049:  ceq
    IL_004b:  br.s       IL_004e

    IL_004d:  ldc.i4.1
    IL_004e:  nop
    IL_004f:  ldc.i4.0
    IL_0050:  ceq
    IL_0052:  stloc.2
    IL_0053:  ldloc.2
    IL_0054:  brtrue.s   IL_005e

    IL_0056:  nop
    IL_0057:  call       void [mscorlib]System.Console::WriteLine()
    IL_005c:  nop
    IL_005d:  nop
    IL_005e:  ldarg.0
    IL_005f:  stloc.0
    IL_0060:  ldarg.1
    IL_0061:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0066:  stloc.1
    IL_0067:  ldloca.s   V_0
    IL_0069:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_006e:  ldloc.1
    IL_006f:  ble.s      IL_007a

    IL_0071:  ldloca.s   V_0
    IL_0073:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0078:  br.s       IL_007b

    IL_007a:  ldc.i4.0
    IL_007b:  nop
    IL_007c:  ldc.i4.0
    IL_007d:  ceq
    IL_007f:  stloc.2
    IL_0080:  ldloc.2
    IL_0081:  brtrue.s   IL_008b

    IL_0083:  nop
    IL_0084:  call       void [mscorlib]System.Console::WriteLine()
    IL_0089:  nop
    IL_008a:  nop
    IL_008b:  ldarg.1
    IL_008c:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0091:  stloc.1
    IL_0092:  ldarg.0
    IL_0093:  stloc.0
    IL_0094:  ldloc.1
    IL_0095:  ldloca.s   V_0
    IL_0097:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_009c:  bne.un.s   IL_00a7

    IL_009e:  ldloca.s   V_0
    IL_00a0:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00a5:  br.s       IL_00a8

    IL_00a7:  ldc.i4.0
    IL_00a8:  nop
    IL_00a9:  ldc.i4.0
    IL_00aa:  ceq
    IL_00ac:  stloc.2
    IL_00ad:  ldloc.2
    IL_00ae:  brtrue.s   IL_00b8

    IL_00b0:  nop
    IL_00b1:  call       void [mscorlib]System.Console::WriteLine()
    IL_00b6:  nop
    IL_00b7:  nop
    IL_00b8:  ldarg.1
    IL_00b9:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00be:  stloc.1
    IL_00bf:  ldarg.0
    IL_00c0:  stloc.0
    IL_00c1:  ldloc.1
    IL_00c2:  ldloca.s   V_0
    IL_00c4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00c9:  bne.un.s   IL_00d7

    IL_00cb:  ldloca.s   V_0
    IL_00cd:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00d2:  ldc.i4.0
    IL_00d3:  ceq
    IL_00d5:  br.s       IL_00d8

    IL_00d7:  ldc.i4.1
    IL_00d8:  nop
    IL_00d9:  ldc.i4.0
    IL_00da:  ceq
    IL_00dc:  stloc.2
    IL_00dd:  ldloc.2
    IL_00de:  brtrue.s   IL_00e8

    IL_00e0:  nop
    IL_00e1:  call       void [mscorlib]System.Console::WriteLine()
    IL_00e6:  nop
    IL_00e7:  nop
    IL_00e8:  ldarg.1
    IL_00e9:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00ee:  stloc.1
    IL_00ef:  ldarg.0
    IL_00f0:  stloc.0
    IL_00f1:  ldloc.1
    IL_00f2:  ldloca.s   V_0
    IL_00f4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00f9:  ble.s      IL_0104

    IL_00fb:  ldloca.s   V_0
    IL_00fd:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0102:  br.s       IL_0105

    IL_0104:  ldc.i4.0
    IL_0105:  nop
    IL_0106:  ldc.i4.0
    IL_0107:  ceq
    IL_0109:  stloc.2
    IL_010a:  ldloc.2
    IL_010b:  brtrue.s   IL_0115

    IL_010d:  nop
    IL_010e:  call       void [mscorlib]System.Console::WriteLine()
    IL_0113:  nop
    IL_0114:  nop
    IL_0115:  ldarg.0
    IL_0116:  stloc.0
    IL_0117:  ldarg.1
    IL_0118:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_011d:  stloc.1
    IL_011e:  ldloca.s   V_0
    IL_0120:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0125:  ldloc.1
    IL_0126:  ble.s      IL_0131

    IL_0128:  ldloca.s   V_0
    IL_012a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_012f:  br.s       IL_0132

    IL_0131:  ldc.i4.0
    IL_0132:  nop
    IL_0133:  stloc.2
    IL_0134:  ldloc.2
    IL_0135:  brtrue.s   IL_013f

    IL_0137:  nop
    IL_0138:  call       void [mscorlib]System.Console::WriteLine()
    IL_013d:  nop
    IL_013e:  nop
    IL_013f:  ldarg.0
    IL_0140:  stloc.0
    IL_0141:  ldarg.1
    IL_0142:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0147:  stloc.1
    IL_0148:  ldloca.s   V_0
    IL_014a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_014f:  ldloc.1
    IL_0150:  bgt.s      IL_015b

    IL_0152:  ldloca.s   V_0
    IL_0154:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0159:  br.s       IL_015c

    IL_015b:  ldc.i4.0
    IL_015c:  nop
    IL_015d:  stloc.2
    IL_015e:  ldloc.2
    IL_015f:  brtrue.s   IL_0169

    IL_0161:  nop
    IL_0162:  call       void [mscorlib]System.Console::WriteLine()
    IL_0167:  nop
    IL_0168:  nop
    IL_0169:  ret
  } // end of method LiftedOperators::IntComplex

  .method public hidebysig static void  IntConst(valuetype [mscorlib]System.Nullable`1<int32> a) cil managed
  {
    // Code size       236 (0xec)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000a:  ldc.i4.2
    IL_000b:  bne.un.s   IL_0016

    IL_000d:  ldloca.s   V_0
    IL_000f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0014:  br.s       IL_0017

    IL_0016:  ldc.i4.0
    IL_0017:  nop
    IL_0018:  ldc.i4.0
    IL_0019:  ceq
    IL_001b:  stloc.1
    IL_001c:  ldloc.1
    IL_001d:  brtrue.s   IL_0027

    IL_001f:  nop
    IL_0020:  call       void [mscorlib]System.Console::WriteLine()
    IL_0025:  nop
    IL_0026:  nop
    IL_0027:  ldarg.0
    IL_0028:  stloc.0
    IL_0029:  ldloca.s   V_0
    IL_002b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0030:  ldc.i4.2
    IL_0031:  bne.un.s   IL_003f

    IL_0033:  ldloca.s   V_0
    IL_0035:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_003a:  ldc.i4.0
    IL_003b:  ceq
    IL_003d:  br.s       IL_0040

    IL_003f:  ldc.i4.1
    IL_0040:  nop
    IL_0041:  ldc.i4.0
    IL_0042:  ceq
    IL_0044:  stloc.1
    IL_0045:  ldloc.1
    IL_0046:  brtrue.s   IL_0050

    IL_0048:  nop
    IL_0049:  call       void [mscorlib]System.Console::WriteLine()
    IL_004e:  nop
    IL_004f:  nop
    IL_0050:  ldarg.0
    IL_0051:  stloc.0
    IL_0052:  ldloca.s   V_0
    IL_0054:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0059:  ldc.i4.2
    IL_005a:  ble.s      IL_0065

    IL_005c:  ldloca.s   V_0
    IL_005e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0063:  br.s       IL_0066

    IL_0065:  ldc.i4.0
    IL_0066:  nop
    IL_0067:  ldc.i4.0
    IL_0068:  ceq
    IL_006a:  stloc.1
    IL_006b:  ldloc.1
    IL_006c:  brtrue.s   IL_0076

    IL_006e:  nop
    IL_006f:  call       void [mscorlib]System.Console::WriteLine()
    IL_0074:  nop
    IL_0075:  nop
    IL_0076:  ldarg.0
    IL_0077:  stloc.0
    IL_0078:  ldc.i4.2
    IL_0079:  ldloca.s   V_0
    IL_007b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0080:  bne.un.s   IL_008b

    IL_0082:  ldloca.s   V_0
    IL_0084:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0089:  br.s       IL_008c

    IL_008b:  ldc.i4.0
    IL_008c:  nop
    IL_008d:  ldc.i4.0
    IL_008e:  ceq
    IL_0090:  stloc.1
    IL_0091:  ldloc.1
    IL_0092:  brtrue.s   IL_009c

    IL_0094:  nop
    IL_0095:  call       void [mscorlib]System.Console::WriteLine()
    IL_009a:  nop
    IL_009b:  nop
    IL_009c:  ldarg.0
    IL_009d:  stloc.0
    IL_009e:  ldc.i4.2
    IL_009f:  ldloca.s   V_0
    IL_00a1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00a6:  bne.un.s   IL_00b4

    IL_00a8:  ldloca.s   V_0
    IL_00aa:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00af:  ldc.i4.0
    IL_00b0:  ceq
    IL_00b2:  br.s       IL_00b5

    IL_00b4:  ldc.i4.1
    IL_00b5:  nop
    IL_00b6:  ldc.i4.0
    IL_00b7:  ceq
    IL_00b9:  stloc.1
    IL_00ba:  ldloc.1
    IL_00bb:  brtrue.s   IL_00c5

    IL_00bd:  nop
    IL_00be:  call       void [mscorlib]System.Console::WriteLine()
    IL_00c3:  nop
    IL_00c4:  nop
    IL_00c5:  ldarg.0
    IL_00c6:  stloc.0
    IL_00c7:  ldc.i4.2
    IL_00c8:  ldloca.s   V_0
    IL_00ca:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00cf:  ble.s      IL_00da

    IL_00d1:  ldloca.s   V_0
    IL_00d3:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00d8:  br.s       IL_00db

    IL_00da:  ldc.i4.0
    IL_00db:  nop
    IL_00dc:  ldc.i4.0
    IL_00dd:  ceq
    IL_00df:  stloc.1
    IL_00e0:  ldloc.1
    IL_00e1:  brtrue.s   IL_00eb

    IL_00e3:  nop
    IL_00e4:  call       void [mscorlib]System.Console::WriteLine()
    IL_00e9:  nop
    IL_00ea:  nop
    IL_00eb:  ret
  } // end of method LiftedOperators::IntConst

  .method public hidebysig static void  IntValueBasic(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                      valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       1674 (0x68a)
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
    IL_0013:  bne.un.s   IL_0027

    IL_0015:  ldloca.s   V_0
    IL_0017:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001c:  ldloca.s   V_1
    IL_001e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0023:  ceq
    IL_0025:  br.s       IL_0028

    IL_0027:  ldc.i4.0
    IL_0028:  nop
    IL_0029:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_002e:  nop
    IL_002f:  ldarg.0
    IL_0030:  stloc.0
    IL_0031:  ldarg.1
    IL_0032:  stloc.1
    IL_0033:  ldloca.s   V_0
    IL_0035:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_003a:  ldloca.s   V_1
    IL_003c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0041:  bne.un.s   IL_0058

    IL_0043:  ldloca.s   V_0
    IL_0045:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_004a:  ldloca.s   V_1
    IL_004c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0051:  ceq
    IL_0053:  ldc.i4.0
    IL_0054:  ceq
    IL_0056:  br.s       IL_0059

    IL_0058:  ldc.i4.1
    IL_0059:  nop
    IL_005a:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_005f:  nop
    IL_0060:  ldarg.0
    IL_0061:  stloc.0
    IL_0062:  ldarg.1
    IL_0063:  stloc.1
    IL_0064:  ldloca.s   V_0
    IL_0066:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_006b:  ldloca.s   V_1
    IL_006d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0072:  ble.s      IL_0085

    IL_0074:  ldloca.s   V_0
    IL_0076:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_007b:  ldloca.s   V_1
    IL_007d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0082:  and
    IL_0083:  br.s       IL_0086

    IL_0085:  ldc.i4.0
    IL_0086:  nop
    IL_0087:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_008c:  nop
    IL_008d:  ldarg.0
    IL_008e:  stloc.0
    IL_008f:  ldarg.1
    IL_0090:  stloc.1
    IL_0091:  ldloca.s   V_0
    IL_0093:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0098:  ldloca.s   V_1
    IL_009a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_009f:  ble.s      IL_00b2

    IL_00a1:  ldloca.s   V_0
    IL_00a3:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00a8:  ldloca.s   V_1
    IL_00aa:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00af:  and
    IL_00b0:  br.s       IL_00b3

    IL_00b2:  ldc.i4.0
    IL_00b3:  nop
    IL_00b4:  ldc.i4.0
    IL_00b5:  ceq
    IL_00b7:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00bc:  nop
    IL_00bd:  ldarg.0
    IL_00be:  stloc.0
    IL_00bf:  ldarg.1
    IL_00c0:  stloc.1
    IL_00c1:  ldloca.s   V_0
    IL_00c3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00c8:  ldloca.s   V_1
    IL_00ca:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00cf:  blt.s      IL_00e2

    IL_00d1:  ldloca.s   V_0
    IL_00d3:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00d8:  ldloca.s   V_1
    IL_00da:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00df:  and
    IL_00e0:  br.s       IL_00e3

    IL_00e2:  ldc.i4.0
    IL_00e3:  nop
    IL_00e4:  ldc.i4.0
    IL_00e5:  ceq
    IL_00e7:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00ec:  nop
    IL_00ed:  ldarg.0
    IL_00ee:  stloc.0
    IL_00ef:  ldarg.1
    IL_00f0:  stloc.1
    IL_00f1:  ldloca.s   V_0
    IL_00f3:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00f8:  ldloca.s   V_1
    IL_00fa:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ff:  and
    IL_0100:  brtrue.s   IL_010d

    IL_0102:  ldloca.s   V_2
    IL_0104:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_010a:  ldloc.2
    IL_010b:  br.s       IL_0121

    IL_010d:  ldloca.s   V_0
    IL_010f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0114:  ldloca.s   V_1
    IL_0116:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_011b:  add
    IL_011c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0121:  nop
    IL_0122:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0127:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_012c:  nop
    IL_012d:  ldarg.0
    IL_012e:  stloc.0
    IL_012f:  ldarg.1
    IL_0130:  stloc.1
    IL_0131:  ldloca.s   V_0
    IL_0133:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0138:  ldloca.s   V_1
    IL_013a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_013f:  and
    IL_0140:  brtrue.s   IL_014d

    IL_0142:  ldloca.s   V_2
    IL_0144:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_014a:  ldloc.2
    IL_014b:  br.s       IL_0161

    IL_014d:  ldloca.s   V_0
    IL_014f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0154:  ldloca.s   V_1
    IL_0156:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_015b:  sub
    IL_015c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0161:  nop
    IL_0162:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0167:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_016c:  nop
    IL_016d:  ldarg.0
    IL_016e:  stloc.0
    IL_016f:  ldarg.1
    IL_0170:  stloc.1
    IL_0171:  ldloca.s   V_0
    IL_0173:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0178:  ldloca.s   V_1
    IL_017a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_017f:  and
    IL_0180:  brtrue.s   IL_018d

    IL_0182:  ldloca.s   V_2
    IL_0184:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_018a:  ldloc.2
    IL_018b:  br.s       IL_01a1

    IL_018d:  ldloca.s   V_0
    IL_018f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0194:  ldloca.s   V_1
    IL_0196:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_019b:  mul
    IL_019c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01a1:  nop
    IL_01a2:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01a7:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01ac:  nop
    IL_01ad:  ldarg.0
    IL_01ae:  stloc.0
    IL_01af:  ldarg.1
    IL_01b0:  stloc.1
    IL_01b1:  ldloca.s   V_0
    IL_01b3:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01b8:  ldloca.s   V_1
    IL_01ba:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01bf:  and
    IL_01c0:  brtrue.s   IL_01cd

    IL_01c2:  ldloca.s   V_2
    IL_01c4:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01ca:  ldloc.2
    IL_01cb:  br.s       IL_01e1

    IL_01cd:  ldloca.s   V_0
    IL_01cf:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01d4:  ldloca.s   V_1
    IL_01d6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01db:  div
    IL_01dc:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01e1:  nop
    IL_01e2:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01e7:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01ec:  nop
    IL_01ed:  ldarg.0
    IL_01ee:  stloc.0
    IL_01ef:  ldarg.1
    IL_01f0:  stloc.1
    IL_01f1:  ldloca.s   V_0
    IL_01f3:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01f8:  ldloca.s   V_1
    IL_01fa:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01ff:  and
    IL_0200:  brtrue.s   IL_020d

    IL_0202:  ldloca.s   V_2
    IL_0204:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_020a:  ldloc.2
    IL_020b:  br.s       IL_0221

    IL_020d:  ldloca.s   V_0
    IL_020f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0214:  ldloca.s   V_1
    IL_0216:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_021b:  rem
    IL_021c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0221:  nop
    IL_0222:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0227:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_022c:  nop
    IL_022d:  ldarg.0
    IL_022e:  stloc.0
    IL_022f:  ldarg.1
    IL_0230:  stloc.1
    IL_0231:  ldloca.s   V_0
    IL_0233:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0238:  ldloca.s   V_1
    IL_023a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_023f:  and
    IL_0240:  brtrue.s   IL_024d

    IL_0242:  ldloca.s   V_2
    IL_0244:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_024a:  ldloc.2
    IL_024b:  br.s       IL_0261

    IL_024d:  ldloca.s   V_0
    IL_024f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0254:  ldloca.s   V_1
    IL_0256:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_025b:  and
    IL_025c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0261:  nop
    IL_0262:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0267:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_026c:  nop
    IL_026d:  ldarg.0
    IL_026e:  stloc.0
    IL_026f:  ldarg.1
    IL_0270:  stloc.1
    IL_0271:  ldloca.s   V_0
    IL_0273:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0278:  ldloca.s   V_1
    IL_027a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_027f:  and
    IL_0280:  brtrue.s   IL_028d

    IL_0282:  ldloca.s   V_2
    IL_0284:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_028a:  ldloc.2
    IL_028b:  br.s       IL_02a1

    IL_028d:  ldloca.s   V_0
    IL_028f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0294:  ldloca.s   V_1
    IL_0296:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_029b:  or
    IL_029c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02a1:  nop
    IL_02a2:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_02a7:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02ac:  nop
    IL_02ad:  ldarg.0
    IL_02ae:  stloc.0
    IL_02af:  ldarg.1
    IL_02b0:  stloc.1
    IL_02b1:  ldloca.s   V_0
    IL_02b3:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02b8:  ldloca.s   V_1
    IL_02ba:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02bf:  and
    IL_02c0:  brtrue.s   IL_02cd

    IL_02c2:  ldloca.s   V_2
    IL_02c4:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02ca:  ldloc.2
    IL_02cb:  br.s       IL_02e1

    IL_02cd:  ldloca.s   V_0
    IL_02cf:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02d4:  ldloca.s   V_1
    IL_02d6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02db:  xor
    IL_02dc:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02e1:  nop
    IL_02e2:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_02e7:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02ec:  nop
    IL_02ed:  ldarg.0
    IL_02ee:  stloc.0
    IL_02ef:  ldarg.1
    IL_02f0:  stloc.1
    IL_02f1:  ldloca.s   V_0
    IL_02f3:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02f8:  ldloca.s   V_1
    IL_02fa:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02ff:  and
    IL_0300:  brtrue.s   IL_030d

    IL_0302:  ldloca.s   V_2
    IL_0304:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_030a:  ldloc.2
    IL_030b:  br.s       IL_0324

    IL_030d:  ldloca.s   V_0
    IL_030f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0314:  ldloca.s   V_1
    IL_0316:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_031b:  ldc.i4.s   31
    IL_031d:  and
    IL_031e:  shl
    IL_031f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0324:  nop
    IL_0325:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_032a:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_032f:  nop
    IL_0330:  ldarg.0
    IL_0331:  stloc.0
    IL_0332:  ldarg.1
    IL_0333:  stloc.1
    IL_0334:  ldloca.s   V_0
    IL_0336:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_033b:  ldloca.s   V_1
    IL_033d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0342:  and
    IL_0343:  brtrue.s   IL_0350

    IL_0345:  ldloca.s   V_2
    IL_0347:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_034d:  ldloc.2
    IL_034e:  br.s       IL_0367

    IL_0350:  ldloca.s   V_0
    IL_0352:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0357:  ldloca.s   V_1
    IL_0359:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_035e:  ldc.i4.s   31
    IL_0360:  and
    IL_0361:  shr
    IL_0362:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0367:  nop
    IL_0368:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_036d:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0372:  nop
    IL_0373:  ldarg.0
    IL_0374:  stloc.0
    IL_0375:  ldloca.s   V_0
    IL_0377:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_037c:  brtrue.s   IL_0381

    IL_037e:  ldarg.1
    IL_037f:  br.s       IL_038d

    IL_0381:  ldloca.s   V_0
    IL_0383:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0388:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_038d:  nop
    IL_038e:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0393:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0398:  nop
    IL_0399:  ldarg.0
    IL_039a:  stloc.0
    IL_039b:  ldloca.s   V_0
    IL_039d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03a2:  brtrue.s   IL_03af

    IL_03a4:  ldloca.s   V_1
    IL_03a6:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03ac:  ldloc.1
    IL_03ad:  br.s       IL_03bc

    IL_03af:  ldloca.s   V_0
    IL_03b1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03b6:  neg
    IL_03b7:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03bc:  nop
    IL_03bd:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_03c2:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_03c7:  nop
    IL_03c8:  ldarg.0
    IL_03c9:  stloc.0
    IL_03ca:  ldloca.s   V_0
    IL_03cc:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03d1:  brtrue.s   IL_03de

    IL_03d3:  ldloca.s   V_1
    IL_03d5:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03db:  ldloc.1
    IL_03dc:  br.s       IL_03eb

    IL_03de:  ldloca.s   V_0
    IL_03e0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03e5:  not
    IL_03e6:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03eb:  nop
    IL_03ec:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_03f1:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_03f6:  nop
    IL_03f7:  ldarg.0
    IL_03f8:  stloc.0
    IL_03f9:  ldloca.s   V_0
    IL_03fb:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0400:  brtrue.s   IL_040d

    IL_0402:  ldloca.s   V_1
    IL_0404:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_040a:  ldloc.1
    IL_040b:  br.s       IL_041b

    IL_040d:  ldloca.s   V_0
    IL_040f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0414:  ldc.i4.1
    IL_0415:  add
    IL_0416:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_041b:  nop
    IL_041c:  dup
    IL_041d:  starg.s    a
    IL_041f:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0424:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0429:  nop
    IL_042a:  ldarg.0
    IL_042b:  stloc.0
    IL_042c:  ldloca.s   V_0
    IL_042e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0433:  brtrue.s   IL_0440

    IL_0435:  ldloca.s   V_1
    IL_0437:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_043d:  ldloc.1
    IL_043e:  br.s       IL_044e

    IL_0440:  ldloca.s   V_0
    IL_0442:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0447:  ldc.i4.1
    IL_0448:  sub
    IL_0449:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_044e:  nop
    IL_044f:  dup
    IL_0450:  starg.s    a
    IL_0452:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0457:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_045c:  nop
    IL_045d:  ldarg.0
    IL_045e:  stloc.0
    IL_045f:  ldarg.1
    IL_0460:  stloc.1
    IL_0461:  ldloca.s   V_0
    IL_0463:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0468:  ldloca.s   V_1
    IL_046a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_046f:  and
    IL_0470:  brtrue.s   IL_047d

    IL_0472:  ldloca.s   V_2
    IL_0474:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_047a:  ldloc.2
    IL_047b:  br.s       IL_0491

    IL_047d:  ldloca.s   V_0
    IL_047f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0484:  ldloca.s   V_1
    IL_0486:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_048b:  add
    IL_048c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0491:  nop
    IL_0492:  starg.s    a
    IL_0494:  ldarg.0
    IL_0495:  stloc.0
    IL_0496:  ldarg.1
    IL_0497:  stloc.1
    IL_0498:  ldloca.s   V_0
    IL_049a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_049f:  ldloca.s   V_1
    IL_04a1:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04a6:  and
    IL_04a7:  brtrue.s   IL_04b4

    IL_04a9:  ldloca.s   V_2
    IL_04ab:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04b1:  ldloc.2
    IL_04b2:  br.s       IL_04c8

    IL_04b4:  ldloca.s   V_0
    IL_04b6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04bb:  ldloca.s   V_1
    IL_04bd:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04c2:  sub
    IL_04c3:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04c8:  nop
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
    IL_04f9:  mul
    IL_04fa:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04ff:  nop
    IL_0500:  starg.s    a
    IL_0502:  ldarg.0
    IL_0503:  stloc.0
    IL_0504:  ldarg.1
    IL_0505:  stloc.1
    IL_0506:  ldloca.s   V_0
    IL_0508:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_050d:  ldloca.s   V_1
    IL_050f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0514:  and
    IL_0515:  brtrue.s   IL_0522

    IL_0517:  ldloca.s   V_2
    IL_0519:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_051f:  ldloc.2
    IL_0520:  br.s       IL_0536

    IL_0522:  ldloca.s   V_0
    IL_0524:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0529:  ldloca.s   V_1
    IL_052b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0530:  div
    IL_0531:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0536:  nop
    IL_0537:  starg.s    a
    IL_0539:  ldarg.0
    IL_053a:  stloc.0
    IL_053b:  ldarg.1
    IL_053c:  stloc.1
    IL_053d:  ldloca.s   V_0
    IL_053f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0544:  ldloca.s   V_1
    IL_0546:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_054b:  and
    IL_054c:  brtrue.s   IL_0559

    IL_054e:  ldloca.s   V_2
    IL_0550:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0556:  ldloc.2
    IL_0557:  br.s       IL_056d

    IL_0559:  ldloca.s   V_0
    IL_055b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0560:  ldloca.s   V_1
    IL_0562:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0567:  rem
    IL_0568:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_056d:  nop
    IL_056e:  starg.s    a
    IL_0570:  ldarg.0
    IL_0571:  stloc.0
    IL_0572:  ldarg.1
    IL_0573:  stloc.1
    IL_0574:  ldloca.s   V_0
    IL_0576:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_057b:  ldloca.s   V_1
    IL_057d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0582:  and
    IL_0583:  brtrue.s   IL_0590

    IL_0585:  ldloca.s   V_2
    IL_0587:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_058d:  ldloc.2
    IL_058e:  br.s       IL_05a4

    IL_0590:  ldloca.s   V_0
    IL_0592:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0597:  ldloca.s   V_1
    IL_0599:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_059e:  and
    IL_059f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_05a4:  nop
    IL_05a5:  starg.s    a
    IL_05a7:  ldarg.0
    IL_05a8:  stloc.0
    IL_05a9:  ldarg.1
    IL_05aa:  stloc.1
    IL_05ab:  ldloca.s   V_0
    IL_05ad:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_05b2:  ldloca.s   V_1
    IL_05b4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_05b9:  and
    IL_05ba:  brtrue.s   IL_05c7

    IL_05bc:  ldloca.s   V_2
    IL_05be:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_05c4:  ldloc.2
    IL_05c5:  br.s       IL_05db

    IL_05c7:  ldloca.s   V_0
    IL_05c9:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_05ce:  ldloca.s   V_1
    IL_05d0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_05d5:  or
    IL_05d6:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_05db:  nop
    IL_05dc:  starg.s    a
    IL_05de:  ldarg.0
    IL_05df:  stloc.0
    IL_05e0:  ldarg.1
    IL_05e1:  stloc.1
    IL_05e2:  ldloca.s   V_0
    IL_05e4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_05e9:  ldloca.s   V_1
    IL_05eb:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_05f0:  and
    IL_05f1:  brtrue.s   IL_05fe

    IL_05f3:  ldloca.s   V_2
    IL_05f5:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_05fb:  ldloc.2
    IL_05fc:  br.s       IL_0612

    IL_05fe:  ldloca.s   V_0
    IL_0600:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0605:  ldloca.s   V_1
    IL_0607:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_060c:  xor
    IL_060d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0612:  nop
    IL_0613:  starg.s    a
    IL_0615:  ldarg.0
    IL_0616:  stloc.0
    IL_0617:  ldarg.1
    IL_0618:  stloc.1
    IL_0619:  ldloca.s   V_0
    IL_061b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0620:  ldloca.s   V_1
    IL_0622:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0627:  and
    IL_0628:  brtrue.s   IL_0635

    IL_062a:  ldloca.s   V_2
    IL_062c:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0632:  ldloc.2
    IL_0633:  br.s       IL_064c

    IL_0635:  ldloca.s   V_0
    IL_0637:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_063c:  ldloca.s   V_1
    IL_063e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0643:  ldc.i4.s   31
    IL_0645:  and
    IL_0646:  shl
    IL_0647:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_064c:  nop
    IL_064d:  starg.s    a
    IL_064f:  ldarg.0
    IL_0650:  stloc.0
    IL_0651:  ldarg.1
    IL_0652:  stloc.1
    IL_0653:  ldloca.s   V_0
    IL_0655:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_065a:  ldloca.s   V_1
    IL_065c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0661:  and
    IL_0662:  brtrue.s   IL_066f

    IL_0664:  ldloca.s   V_2
    IL_0666:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_066c:  ldloc.2
    IL_066d:  br.s       IL_0686

    IL_066f:  ldloca.s   V_0
    IL_0671:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0676:  ldloca.s   V_1
    IL_0678:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_067d:  ldc.i4.s   31
    IL_067f:  and
    IL_0680:  shr
    IL_0681:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0686:  nop
    IL_0687:  starg.s    a
    IL_0689:  ret
  } // end of method LiftedOperators::IntValueBasic

  .method public hidebysig static void  IntValueComplex(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                        class [mscorlib]System.Func`1<int32> x) cil managed
  {
    // Code size       1400 (0x578)
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
    IL_0012:  bne.un.s   IL_001d

    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001b:  br.s       IL_001e

    IL_001d:  ldc.i4.0
    IL_001e:  nop
    IL_001f:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0024:  nop
    IL_0025:  ldarg.0
    IL_0026:  stloc.0
    IL_0027:  ldarg.1
    IL_0028:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_002d:  stloc.1
    IL_002e:  ldloca.s   V_0
    IL_0030:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0035:  ldloc.1
    IL_0036:  bne.un.s   IL_0044

    IL_0038:  ldloca.s   V_0
    IL_003a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_003f:  ldc.i4.0
    IL_0040:  ceq
    IL_0042:  br.s       IL_0045

    IL_0044:  ldc.i4.1
    IL_0045:  nop
    IL_0046:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_004b:  nop
    IL_004c:  ldarg.0
    IL_004d:  stloc.0
    IL_004e:  ldarg.1
    IL_004f:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0054:  stloc.1
    IL_0055:  ldloca.s   V_0
    IL_0057:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_005c:  ldloc.1
    IL_005d:  ble.s      IL_0068

    IL_005f:  ldloca.s   V_0
    IL_0061:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0066:  br.s       IL_0069

    IL_0068:  ldc.i4.0
    IL_0069:  nop
    IL_006a:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_006f:  nop
    IL_0070:  ldarg.1
    IL_0071:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0076:  stloc.1
    IL_0077:  ldarg.0
    IL_0078:  stloc.0
    IL_0079:  ldloc.1
    IL_007a:  ldloca.s   V_0
    IL_007c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0081:  bne.un.s   IL_008c

    IL_0083:  ldloca.s   V_0
    IL_0085:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_008a:  br.s       IL_008d

    IL_008c:  ldc.i4.0
    IL_008d:  nop
    IL_008e:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0093:  nop
    IL_0094:  ldarg.1
    IL_0095:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_009a:  stloc.1
    IL_009b:  ldarg.0
    IL_009c:  stloc.0
    IL_009d:  ldloc.1
    IL_009e:  ldloca.s   V_0
    IL_00a0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00a5:  bne.un.s   IL_00b3

    IL_00a7:  ldloca.s   V_0
    IL_00a9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ae:  ldc.i4.0
    IL_00af:  ceq
    IL_00b1:  br.s       IL_00b4

    IL_00b3:  ldc.i4.1
    IL_00b4:  nop
    IL_00b5:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00ba:  nop
    IL_00bb:  ldarg.1
    IL_00bc:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00c1:  stloc.1
    IL_00c2:  ldarg.0
    IL_00c3:  stloc.0
    IL_00c4:  ldloc.1
    IL_00c5:  ldloca.s   V_0
    IL_00c7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00cc:  ble.s      IL_00d7

    IL_00ce:  ldloca.s   V_0
    IL_00d0:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00d5:  br.s       IL_00d8

    IL_00d7:  ldc.i4.0
    IL_00d8:  nop
    IL_00d9:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00de:  nop
    IL_00df:  ldarg.0
    IL_00e0:  stloc.0
    IL_00e1:  ldarg.1
    IL_00e2:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00e7:  stloc.1
    IL_00e8:  ldloca.s   V_0
    IL_00ea:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ef:  brtrue.s   IL_00fc

    IL_00f1:  ldloca.s   V_2
    IL_00f3:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00f9:  ldloc.2
    IL_00fa:  br.s       IL_010a

    IL_00fc:  ldloca.s   V_0
    IL_00fe:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0103:  ldloc.1
    IL_0104:  add
    IL_0105:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_010a:  nop
    IL_010b:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0110:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0115:  nop
    IL_0116:  ldarg.0
    IL_0117:  stloc.0
    IL_0118:  ldarg.1
    IL_0119:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_011e:  stloc.1
    IL_011f:  ldloca.s   V_0
    IL_0121:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0126:  brtrue.s   IL_0133

    IL_0128:  ldloca.s   V_2
    IL_012a:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0130:  ldloc.2
    IL_0131:  br.s       IL_0141

    IL_0133:  ldloca.s   V_0
    IL_0135:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_013a:  ldloc.1
    IL_013b:  sub
    IL_013c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0141:  nop
    IL_0142:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0147:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_014c:  nop
    IL_014d:  ldarg.0
    IL_014e:  stloc.0
    IL_014f:  ldarg.1
    IL_0150:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0155:  stloc.1
    IL_0156:  ldloca.s   V_0
    IL_0158:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_015d:  brtrue.s   IL_016a

    IL_015f:  ldloca.s   V_2
    IL_0161:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0167:  ldloc.2
    IL_0168:  br.s       IL_0178

    IL_016a:  ldloca.s   V_0
    IL_016c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0171:  ldloc.1
    IL_0172:  mul
    IL_0173:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0178:  nop
    IL_0179:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_017e:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0183:  nop
    IL_0184:  ldarg.0
    IL_0185:  stloc.0
    IL_0186:  ldarg.1
    IL_0187:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_018c:  stloc.1
    IL_018d:  ldloca.s   V_0
    IL_018f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0194:  brtrue.s   IL_01a1

    IL_0196:  ldloca.s   V_2
    IL_0198:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_019e:  ldloc.2
    IL_019f:  br.s       IL_01af

    IL_01a1:  ldloca.s   V_0
    IL_01a3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01a8:  ldloc.1
    IL_01a9:  div
    IL_01aa:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01af:  nop
    IL_01b0:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01b5:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01ba:  nop
    IL_01bb:  ldarg.0
    IL_01bc:  stloc.0
    IL_01bd:  ldarg.1
    IL_01be:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_01c3:  stloc.1
    IL_01c4:  ldloca.s   V_0
    IL_01c6:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01cb:  brtrue.s   IL_01d8

    IL_01cd:  ldloca.s   V_2
    IL_01cf:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01d5:  ldloc.2
    IL_01d6:  br.s       IL_01e6

    IL_01d8:  ldloca.s   V_0
    IL_01da:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01df:  ldloc.1
    IL_01e0:  rem
    IL_01e1:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01e6:  nop
    IL_01e7:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01ec:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01f1:  nop
    IL_01f2:  ldarg.0
    IL_01f3:  stloc.0
    IL_01f4:  ldarg.1
    IL_01f5:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_01fa:  stloc.1
    IL_01fb:  ldloca.s   V_0
    IL_01fd:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0202:  brtrue.s   IL_020f

    IL_0204:  ldloca.s   V_2
    IL_0206:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_020c:  ldloc.2
    IL_020d:  br.s       IL_021d

    IL_020f:  ldloca.s   V_0
    IL_0211:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0216:  ldloc.1
    IL_0217:  and
    IL_0218:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_021d:  nop
    IL_021e:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0223:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0228:  nop
    IL_0229:  ldarg.0
    IL_022a:  stloc.0
    IL_022b:  ldarg.1
    IL_022c:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0231:  stloc.1
    IL_0232:  ldloca.s   V_0
    IL_0234:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0239:  brtrue.s   IL_0246

    IL_023b:  ldloca.s   V_2
    IL_023d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0243:  ldloc.2
    IL_0244:  br.s       IL_0254

    IL_0246:  ldloca.s   V_0
    IL_0248:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_024d:  ldloc.1
    IL_024e:  or
    IL_024f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0254:  nop
    IL_0255:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_025a:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_025f:  nop
    IL_0260:  ldarg.0
    IL_0261:  stloc.0
    IL_0262:  ldarg.1
    IL_0263:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0268:  stloc.1
    IL_0269:  ldloca.s   V_0
    IL_026b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0270:  brtrue.s   IL_027d

    IL_0272:  ldloca.s   V_2
    IL_0274:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_027a:  ldloc.2
    IL_027b:  br.s       IL_028b

    IL_027d:  ldloca.s   V_0
    IL_027f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0284:  ldloc.1
    IL_0285:  xor
    IL_0286:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_028b:  nop
    IL_028c:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0291:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0296:  nop
    IL_0297:  ldarg.0
    IL_0298:  stloc.0
    IL_0299:  ldarg.1
    IL_029a:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_029f:  stloc.1
    IL_02a0:  ldloca.s   V_0
    IL_02a2:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02a7:  brtrue.s   IL_02b4

    IL_02a9:  ldloca.s   V_2
    IL_02ab:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02b1:  ldloc.2
    IL_02b2:  br.s       IL_02c5

    IL_02b4:  ldloca.s   V_0
    IL_02b6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02bb:  ldloc.1
    IL_02bc:  ldc.i4.s   31
    IL_02be:  and
    IL_02bf:  shl
    IL_02c0:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02c5:  nop
    IL_02c6:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_02cb:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02d0:  nop
    IL_02d1:  ldarg.0
    IL_02d2:  stloc.0
    IL_02d3:  ldarg.1
    IL_02d4:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_02d9:  stloc.1
    IL_02da:  ldloca.s   V_0
    IL_02dc:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02e1:  brtrue.s   IL_02ee

    IL_02e3:  ldloca.s   V_2
    IL_02e5:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02eb:  ldloc.2
    IL_02ec:  br.s       IL_02ff

    IL_02ee:  ldloca.s   V_0
    IL_02f0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02f5:  ldloc.1
    IL_02f6:  ldc.i4.s   31
    IL_02f8:  and
    IL_02f9:  shr
    IL_02fa:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02ff:  nop
    IL_0300:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0305:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_030a:  nop
    IL_030b:  ldarg.0
    IL_030c:  stloc.0
    IL_030d:  ldloca.s   V_0
    IL_030f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0314:  brtrue.s   IL_031e

    IL_0316:  ldarg.1
    IL_0317:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_031c:  br.s       IL_0325

    IL_031e:  ldloca.s   V_0
    IL_0320:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0325:  nop
    IL_0326:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_032b:  nop
    IL_032c:  ldarg.0
    IL_032d:  stloc.0
    IL_032e:  ldarg.1
    IL_032f:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0334:  stloc.1
    IL_0335:  ldloca.s   V_0
    IL_0337:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_033c:  brtrue.s   IL_0349

    IL_033e:  ldloca.s   V_2
    IL_0340:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0346:  ldloc.2
    IL_0347:  br.s       IL_0357

    IL_0349:  ldloca.s   V_0
    IL_034b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0350:  ldloc.1
    IL_0351:  add
    IL_0352:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0357:  nop
    IL_0358:  starg.s    a
    IL_035a:  ldarg.0
    IL_035b:  stloc.0
    IL_035c:  ldarg.1
    IL_035d:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0362:  stloc.1
    IL_0363:  ldloca.s   V_0
    IL_0365:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_036a:  brtrue.s   IL_0377

    IL_036c:  ldloca.s   V_2
    IL_036e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0374:  ldloc.2
    IL_0375:  br.s       IL_0385

    IL_0377:  ldloca.s   V_0
    IL_0379:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_037e:  ldloc.1
    IL_037f:  sub
    IL_0380:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0385:  nop
    IL_0386:  starg.s    a
    IL_0388:  ldarg.0
    IL_0389:  stloc.0
    IL_038a:  ldarg.1
    IL_038b:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0390:  stloc.1
    IL_0391:  ldloca.s   V_0
    IL_0393:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0398:  brtrue.s   IL_03a5

    IL_039a:  ldloca.s   V_2
    IL_039c:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03a2:  ldloc.2
    IL_03a3:  br.s       IL_03b3

    IL_03a5:  ldloca.s   V_0
    IL_03a7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03ac:  ldloc.1
    IL_03ad:  mul
    IL_03ae:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03b3:  nop
    IL_03b4:  starg.s    a
    IL_03b6:  ldarg.0
    IL_03b7:  stloc.0
    IL_03b8:  ldarg.1
    IL_03b9:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_03be:  stloc.1
    IL_03bf:  ldloca.s   V_0
    IL_03c1:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03c6:  brtrue.s   IL_03d3

    IL_03c8:  ldloca.s   V_2
    IL_03ca:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03d0:  ldloc.2
    IL_03d1:  br.s       IL_03e1

    IL_03d3:  ldloca.s   V_0
    IL_03d5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03da:  ldloc.1
    IL_03db:  div
    IL_03dc:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03e1:  nop
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
    IL_0409:  rem
    IL_040a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_040f:  nop
    IL_0410:  starg.s    a
    IL_0412:  ldarg.0
    IL_0413:  stloc.0
    IL_0414:  ldarg.1
    IL_0415:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_041a:  stloc.1
    IL_041b:  ldloca.s   V_0
    IL_041d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0422:  brtrue.s   IL_042f

    IL_0424:  ldloca.s   V_2
    IL_0426:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_042c:  ldloc.2
    IL_042d:  br.s       IL_043d

    IL_042f:  ldloca.s   V_0
    IL_0431:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0436:  ldloc.1
    IL_0437:  and
    IL_0438:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_043d:  nop
    IL_043e:  starg.s    a
    IL_0440:  ldarg.0
    IL_0441:  stloc.0
    IL_0442:  ldarg.1
    IL_0443:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0448:  stloc.1
    IL_0449:  ldloca.s   V_0
    IL_044b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0450:  brtrue.s   IL_045d

    IL_0452:  ldloca.s   V_2
    IL_0454:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_045a:  ldloc.2
    IL_045b:  br.s       IL_046b

    IL_045d:  ldloca.s   V_0
    IL_045f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0464:  ldloc.1
    IL_0465:  or
    IL_0466:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_046b:  nop
    IL_046c:  starg.s    a
    IL_046e:  ldarg.0
    IL_046f:  stloc.0
    IL_0470:  ldarg.1
    IL_0471:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0476:  stloc.1
    IL_0477:  ldloca.s   V_0
    IL_0479:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_047e:  brtrue.s   IL_048b

    IL_0480:  ldloca.s   V_2
    IL_0482:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0488:  ldloc.2
    IL_0489:  br.s       IL_0499

    IL_048b:  ldloca.s   V_0
    IL_048d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0492:  ldloc.1
    IL_0493:  xor
    IL_0494:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0499:  nop
    IL_049a:  starg.s    a
    IL_049c:  ldarg.0
    IL_049d:  stloc.0
    IL_049e:  ldarg.1
    IL_049f:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_04a4:  stloc.1
    IL_04a5:  ldloca.s   V_0
    IL_04a7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04ac:  brtrue.s   IL_04b9

    IL_04ae:  ldloca.s   V_2
    IL_04b0:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04b6:  ldloc.2
    IL_04b7:  br.s       IL_04ca

    IL_04b9:  ldloca.s   V_0
    IL_04bb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04c0:  ldloc.1
    IL_04c1:  ldc.i4.s   31
    IL_04c3:  and
    IL_04c4:  shl
    IL_04c5:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04ca:  nop
    IL_04cb:  starg.s    a
    IL_04cd:  ldarg.0
    IL_04ce:  stloc.0
    IL_04cf:  ldarg.1
    IL_04d0:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_04d5:  stloc.1
    IL_04d6:  ldloca.s   V_0
    IL_04d8:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04dd:  brtrue.s   IL_04ea

    IL_04df:  ldloca.s   V_2
    IL_04e1:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04e7:  ldloc.2
    IL_04e8:  br.s       IL_04fb

    IL_04ea:  ldloca.s   V_0
    IL_04ec:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04f1:  ldloc.1
    IL_04f2:  ldc.i4.s   31
    IL_04f4:  and
    IL_04f5:  shr
    IL_04f6:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04fb:  nop
    IL_04fc:  starg.s    a
    IL_04fe:  ldarg.1
    IL_04ff:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0504:  stloc.1
    IL_0505:  ldarg.0
    IL_0506:  stloc.0
    IL_0507:  ldloca.s   V_0
    IL_0509:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_050e:  brtrue.s   IL_051b

    IL_0510:  ldloca.s   V_2
    IL_0512:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0518:  ldloc.2
    IL_0519:  br.s       IL_0529

    IL_051b:  ldloc.1
    IL_051c:  ldloca.s   V_0
    IL_051e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0523:  add
    IL_0524:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0529:  nop
    IL_052a:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_052f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0534:  nop
    IL_0535:  ldc.i4.0
    IL_0536:  newarr     valuetype [mscorlib]System.Nullable`1<int32>
    IL_053b:  ldc.i4.0
    IL_053c:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0541:  dup
    IL_0542:  ldobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0547:  stloc.0
    IL_0548:  ldarg.1
    IL_0549:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_054e:  stloc.1
    IL_054f:  ldloca.s   V_0
    IL_0551:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0556:  brtrue.s   IL_0563

    IL_0558:  ldloca.s   V_2
    IL_055a:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0560:  ldloc.2
    IL_0561:  br.s       IL_0571

    IL_0563:  ldloca.s   V_0
    IL_0565:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_056a:  ldloc.1
    IL_056b:  add
    IL_056c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0571:  nop
    IL_0572:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0577:  ret
  } // end of method LiftedOperators::IntValueComplex

  .method public hidebysig static void  IntValueConst(valuetype [mscorlib]System.Nullable`1<int32> a) cil managed
  {
    // Code size       1128 (0x468)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000a:  ldc.i4.2
    IL_000b:  bne.un.s   IL_0016

    IL_000d:  ldloca.s   V_0
    IL_000f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0014:  br.s       IL_0017

    IL_0016:  ldc.i4.0
    IL_0017:  nop
    IL_0018:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_001d:  nop
    IL_001e:  ldarg.0
    IL_001f:  stloc.0
    IL_0020:  ldloca.s   V_0
    IL_0022:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0027:  ldc.i4.2
    IL_0028:  bne.un.s   IL_0036

    IL_002a:  ldloca.s   V_0
    IL_002c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0031:  ldc.i4.0
    IL_0032:  ceq
    IL_0034:  br.s       IL_0037

    IL_0036:  ldc.i4.1
    IL_0037:  nop
    IL_0038:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_003d:  nop
    IL_003e:  ldarg.0
    IL_003f:  stloc.0
    IL_0040:  ldloca.s   V_0
    IL_0042:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0047:  ldc.i4.2
    IL_0048:  ble.s      IL_0053

    IL_004a:  ldloca.s   V_0
    IL_004c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0051:  br.s       IL_0054

    IL_0053:  ldc.i4.0
    IL_0054:  nop
    IL_0055:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_005a:  nop
    IL_005b:  ldarg.0
    IL_005c:  stloc.0
    IL_005d:  ldc.i4.2
    IL_005e:  ldloca.s   V_0
    IL_0060:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0065:  bne.un.s   IL_0070

    IL_0067:  ldloca.s   V_0
    IL_0069:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_006e:  br.s       IL_0071

    IL_0070:  ldc.i4.0
    IL_0071:  nop
    IL_0072:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0077:  nop
    IL_0078:  ldarg.0
    IL_0079:  stloc.0
    IL_007a:  ldc.i4.2
    IL_007b:  ldloca.s   V_0
    IL_007d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0082:  bne.un.s   IL_0090

    IL_0084:  ldloca.s   V_0
    IL_0086:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_008b:  ldc.i4.0
    IL_008c:  ceq
    IL_008e:  br.s       IL_0091

    IL_0090:  ldc.i4.1
    IL_0091:  nop
    IL_0092:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0097:  nop
    IL_0098:  ldarg.0
    IL_0099:  stloc.0
    IL_009a:  ldc.i4.2
    IL_009b:  ldloca.s   V_0
    IL_009d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00a2:  ble.s      IL_00ad

    IL_00a4:  ldloca.s   V_0
    IL_00a6:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ab:  br.s       IL_00ae

    IL_00ad:  ldc.i4.0
    IL_00ae:  nop
    IL_00af:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00b4:  nop
    IL_00b5:  ldarg.0
    IL_00b6:  stloc.0
    IL_00b7:  ldloca.s   V_0
    IL_00b9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00be:  brtrue.s   IL_00cb

    IL_00c0:  ldloca.s   V_1
    IL_00c2:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00c8:  ldloc.1
    IL_00c9:  br.s       IL_00d9

    IL_00cb:  ldloca.s   V_0
    IL_00cd:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00d2:  ldc.i4.2
    IL_00d3:  add
    IL_00d4:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00d9:  nop
    IL_00da:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_00df:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00e4:  nop
    IL_00e5:  ldarg.0
    IL_00e6:  stloc.0
    IL_00e7:  ldloca.s   V_0
    IL_00e9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ee:  brtrue.s   IL_00fb

    IL_00f0:  ldloca.s   V_1
    IL_00f2:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00f8:  ldloc.1
    IL_00f9:  br.s       IL_0109

    IL_00fb:  ldloca.s   V_0
    IL_00fd:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0102:  ldc.i4.2
    IL_0103:  sub
    IL_0104:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0109:  nop
    IL_010a:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_010f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0114:  nop
    IL_0115:  ldarg.0
    IL_0116:  stloc.0
    IL_0117:  ldloca.s   V_0
    IL_0119:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_011e:  brtrue.s   IL_012b

    IL_0120:  ldloca.s   V_1
    IL_0122:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0128:  ldloc.1
    IL_0129:  br.s       IL_0139

    IL_012b:  ldloca.s   V_0
    IL_012d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0132:  ldc.i4.2
    IL_0133:  mul
    IL_0134:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0139:  nop
    IL_013a:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_013f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0144:  nop
    IL_0145:  ldarg.0
    IL_0146:  stloc.0
    IL_0147:  ldloca.s   V_0
    IL_0149:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_014e:  brtrue.s   IL_015b

    IL_0150:  ldloca.s   V_1
    IL_0152:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0158:  ldloc.1
    IL_0159:  br.s       IL_0169

    IL_015b:  ldloca.s   V_0
    IL_015d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0162:  ldc.i4.2
    IL_0163:  div
    IL_0164:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0169:  nop
    IL_016a:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_016f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0174:  nop
    IL_0175:  ldarg.0
    IL_0176:  stloc.0
    IL_0177:  ldloca.s   V_0
    IL_0179:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_017e:  brtrue.s   IL_018b

    IL_0180:  ldloca.s   V_1
    IL_0182:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0188:  ldloc.1
    IL_0189:  br.s       IL_0199

    IL_018b:  ldloca.s   V_0
    IL_018d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0192:  ldc.i4.2
    IL_0193:  rem
    IL_0194:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0199:  nop
    IL_019a:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_019f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01a4:  nop
    IL_01a5:  ldarg.0
    IL_01a6:  stloc.0
    IL_01a7:  ldloca.s   V_0
    IL_01a9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01ae:  brtrue.s   IL_01bb

    IL_01b0:  ldloca.s   V_1
    IL_01b2:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01b8:  ldloc.1
    IL_01b9:  br.s       IL_01c9

    IL_01bb:  ldloca.s   V_0
    IL_01bd:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01c2:  ldc.i4.2
    IL_01c3:  and
    IL_01c4:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01c9:  nop
    IL_01ca:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01cf:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01d4:  nop
    IL_01d5:  ldarg.0
    IL_01d6:  stloc.0
    IL_01d7:  ldloca.s   V_0
    IL_01d9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01de:  brtrue.s   IL_01eb

    IL_01e0:  ldloca.s   V_1
    IL_01e2:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01e8:  ldloc.1
    IL_01e9:  br.s       IL_01f9

    IL_01eb:  ldloca.s   V_0
    IL_01ed:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01f2:  ldc.i4.2
    IL_01f3:  or
    IL_01f4:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01f9:  nop
    IL_01fa:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01ff:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0204:  nop
    IL_0205:  ldarg.0
    IL_0206:  stloc.0
    IL_0207:  ldloca.s   V_0
    IL_0209:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_020e:  brtrue.s   IL_021b

    IL_0210:  ldloca.s   V_1
    IL_0212:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0218:  ldloc.1
    IL_0219:  br.s       IL_0229

    IL_021b:  ldloca.s   V_0
    IL_021d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0222:  ldc.i4.2
    IL_0223:  xor
    IL_0224:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0229:  nop
    IL_022a:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_022f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0234:  nop
    IL_0235:  ldarg.0
    IL_0236:  stloc.0
    IL_0237:  ldloca.s   V_0
    IL_0239:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_023e:  brtrue.s   IL_024b

    IL_0240:  ldloca.s   V_1
    IL_0242:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0248:  ldloc.1
    IL_0249:  br.s       IL_0259

    IL_024b:  ldloca.s   V_0
    IL_024d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0252:  ldc.i4.2
    IL_0253:  shl
    IL_0254:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0259:  nop
    IL_025a:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_025f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0264:  nop
    IL_0265:  ldarg.0
    IL_0266:  stloc.0
    IL_0267:  ldloca.s   V_0
    IL_0269:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_026e:  brtrue.s   IL_027b

    IL_0270:  ldloca.s   V_1
    IL_0272:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0278:  ldloc.1
    IL_0279:  br.s       IL_0289

    IL_027b:  ldloca.s   V_0
    IL_027d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0282:  ldc.i4.2
    IL_0283:  shr
    IL_0284:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0289:  nop
    IL_028a:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_028f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0294:  nop
    IL_0295:  ldarg.0
    IL_0296:  stloc.0
    IL_0297:  ldloca.s   V_0
    IL_0299:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_029e:  brtrue.s   IL_02a3

    IL_02a0:  ldc.i4.2
    IL_02a1:  br.s       IL_02aa

    IL_02a3:  ldloca.s   V_0
    IL_02a5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02aa:  nop
    IL_02ab:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_02b0:  nop
    IL_02b1:  ldarg.0
    IL_02b2:  stloc.0
    IL_02b3:  ldloca.s   V_0
    IL_02b5:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02ba:  brtrue.s   IL_02c7

    IL_02bc:  ldloca.s   V_1
    IL_02be:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02c4:  ldloc.1
    IL_02c5:  br.s       IL_02d5

    IL_02c7:  ldloca.s   V_0
    IL_02c9:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02ce:  ldc.i4.2
    IL_02cf:  add
    IL_02d0:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02d5:  nop
    IL_02d6:  starg.s    a
    IL_02d8:  ldarg.0
    IL_02d9:  stloc.0
    IL_02da:  ldloca.s   V_0
    IL_02dc:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02e1:  brtrue.s   IL_02ee

    IL_02e3:  ldloca.s   V_1
    IL_02e5:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02eb:  ldloc.1
    IL_02ec:  br.s       IL_02fc

    IL_02ee:  ldloca.s   V_0
    IL_02f0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02f5:  ldc.i4.2
    IL_02f6:  sub
    IL_02f7:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02fc:  nop
    IL_02fd:  starg.s    a
    IL_02ff:  ldarg.0
    IL_0300:  stloc.0
    IL_0301:  ldloca.s   V_0
    IL_0303:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0308:  brtrue.s   IL_0315

    IL_030a:  ldloca.s   V_1
    IL_030c:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0312:  ldloc.1
    IL_0313:  br.s       IL_0323

    IL_0315:  ldloca.s   V_0
    IL_0317:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_031c:  ldc.i4.2
    IL_031d:  mul
    IL_031e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0323:  nop
    IL_0324:  starg.s    a
    IL_0326:  ldarg.0
    IL_0327:  stloc.0
    IL_0328:  ldloca.s   V_0
    IL_032a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_032f:  brtrue.s   IL_033c

    IL_0331:  ldloca.s   V_1
    IL_0333:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0339:  ldloc.1
    IL_033a:  br.s       IL_034a

    IL_033c:  ldloca.s   V_0
    IL_033e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0343:  ldc.i4.2
    IL_0344:  div
    IL_0345:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_034a:  nop
    IL_034b:  starg.s    a
    IL_034d:  ldarg.0
    IL_034e:  stloc.0
    IL_034f:  ldloca.s   V_0
    IL_0351:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0356:  brtrue.s   IL_0363

    IL_0358:  ldloca.s   V_1
    IL_035a:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0360:  ldloc.1
    IL_0361:  br.s       IL_0371

    IL_0363:  ldloca.s   V_0
    IL_0365:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_036a:  ldc.i4.2
    IL_036b:  rem
    IL_036c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0371:  nop
    IL_0372:  starg.s    a
    IL_0374:  ldarg.0
    IL_0375:  stloc.0
    IL_0376:  ldloca.s   V_0
    IL_0378:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_037d:  brtrue.s   IL_038a

    IL_037f:  ldloca.s   V_1
    IL_0381:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0387:  ldloc.1
    IL_0388:  br.s       IL_0398

    IL_038a:  ldloca.s   V_0
    IL_038c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0391:  ldc.i4.2
    IL_0392:  and
    IL_0393:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0398:  nop
    IL_0399:  starg.s    a
    IL_039b:  ldarg.0
    IL_039c:  stloc.0
    IL_039d:  ldloca.s   V_0
    IL_039f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03a4:  brtrue.s   IL_03b1

    IL_03a6:  ldloca.s   V_1
    IL_03a8:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03ae:  ldloc.1
    IL_03af:  br.s       IL_03bf

    IL_03b1:  ldloca.s   V_0
    IL_03b3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03b8:  ldc.i4.2
    IL_03b9:  or
    IL_03ba:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03bf:  nop
    IL_03c0:  starg.s    a
    IL_03c2:  ldarg.0
    IL_03c3:  stloc.0
    IL_03c4:  ldloca.s   V_0
    IL_03c6:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03cb:  brtrue.s   IL_03d8

    IL_03cd:  ldloca.s   V_1
    IL_03cf:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03d5:  ldloc.1
    IL_03d6:  br.s       IL_03e6

    IL_03d8:  ldloca.s   V_0
    IL_03da:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03df:  ldc.i4.2
    IL_03e0:  xor
    IL_03e1:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03e6:  nop
    IL_03e7:  starg.s    a
    IL_03e9:  ldarg.0
    IL_03ea:  stloc.0
    IL_03eb:  ldloca.s   V_0
    IL_03ed:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03f2:  brtrue.s   IL_03ff

    IL_03f4:  ldloca.s   V_1
    IL_03f6:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03fc:  ldloc.1
    IL_03fd:  br.s       IL_040d

    IL_03ff:  ldloca.s   V_0
    IL_0401:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0406:  ldc.i4.2
    IL_0407:  shl
    IL_0408:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_040d:  nop
    IL_040e:  starg.s    a
    IL_0410:  ldarg.0
    IL_0411:  stloc.0
    IL_0412:  ldloca.s   V_0
    IL_0414:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0419:  brtrue.s   IL_0426

    IL_041b:  ldloca.s   V_1
    IL_041d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0423:  ldloc.1
    IL_0424:  br.s       IL_0434

    IL_0426:  ldloca.s   V_0
    IL_0428:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_042d:  ldc.i4.2
    IL_042e:  shr
    IL_042f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0434:  nop
    IL_0435:  starg.s    a
    IL_0437:  ldarg.0
    IL_0438:  stloc.0
    IL_0439:  ldloca.s   V_0
    IL_043b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0440:  brtrue.s   IL_044d

    IL_0442:  ldloca.s   V_1
    IL_0444:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_044a:  ldloc.1
    IL_044b:  br.s       IL_045b

    IL_044d:  ldc.i4.2
    IL_044e:  ldloca.s   V_0
    IL_0450:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0455:  add
    IL_0456:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_045b:  nop
    IL_045c:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0461:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0466:  nop
    IL_0467:  ret
  } // end of method LiftedOperators::IntValueConst

  .method public hidebysig static void  NumberBasic(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> a,
                                                    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> b) cil managed
  {
    // Code size       473 (0x1d9)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_0,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_1,
             bool V_2)
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
    IL_0018:  brfalse.s  IL_002c

    IL_001a:  ldloca.s   V_0
    IL_001c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0021:  ldloca.s   V_1
    IL_0023:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0028:  ceq
    IL_002a:  br.s       IL_002d

    IL_002c:  ldc.i4.0
    IL_002d:  nop
    IL_002e:  ldc.i4.0
    IL_002f:  ceq
    IL_0031:  stloc.2
    IL_0032:  ldloc.2
    IL_0033:  brtrue.s   IL_003d

    IL_0035:  nop
    IL_0036:  call       void [mscorlib]System.Console::WriteLine()
    IL_003b:  nop
    IL_003c:  nop
    IL_003d:  ldarg.0
    IL_003e:  stloc.0
    IL_003f:  ldarg.1
    IL_0040:  stloc.1
    IL_0041:  ldloca.s   V_0
    IL_0043:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0048:  ldloca.s   V_1
    IL_004a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_004f:  call       bool [mscorlib]System.Decimal::op_Inequality(valuetype [mscorlib]System.Decimal,
                                                                      valuetype [mscorlib]System.Decimal)
    IL_0054:  brtrue.s   IL_006b

    IL_0056:  ldloca.s   V_0
    IL_0058:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_005d:  ldloca.s   V_1
    IL_005f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0064:  ceq
    IL_0066:  ldc.i4.0
    IL_0067:  ceq
    IL_0069:  br.s       IL_006c

    IL_006b:  ldc.i4.1
    IL_006c:  nop
    IL_006d:  ldc.i4.0
    IL_006e:  ceq
    IL_0070:  stloc.2
    IL_0071:  ldloc.2
    IL_0072:  brtrue.s   IL_007c

    IL_0074:  nop
    IL_0075:  call       void [mscorlib]System.Console::WriteLine()
    IL_007a:  nop
    IL_007b:  nop
    IL_007c:  ldarg.0
    IL_007d:  stloc.0
    IL_007e:  ldarg.1
    IL_007f:  stloc.1
    IL_0080:  ldloca.s   V_0
    IL_0082:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0087:  ldloca.s   V_1
    IL_0089:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_008e:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_0093:  brfalse.s  IL_00a6

    IL_0095:  ldloca.s   V_0
    IL_0097:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_009c:  ldloca.s   V_1
    IL_009e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00a3:  and
    IL_00a4:  br.s       IL_00a7

    IL_00a6:  ldc.i4.0
    IL_00a7:  nop
    IL_00a8:  ldc.i4.0
    IL_00a9:  ceq
    IL_00ab:  stloc.2
    IL_00ac:  ldloc.2
    IL_00ad:  brtrue.s   IL_00b7

    IL_00af:  nop
    IL_00b0:  call       void [mscorlib]System.Console::WriteLine()
    IL_00b5:  nop
    IL_00b6:  nop
    IL_00b7:  ldarg.0
    IL_00b8:  stloc.0
    IL_00b9:  ldarg.1
    IL_00ba:  stloc.1
    IL_00bb:  ldloca.s   V_0
    IL_00bd:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00c2:  ldloca.s   V_1
    IL_00c4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00c9:  call       bool [mscorlib]System.Decimal::op_LessThan(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_00ce:  brfalse.s  IL_00e1

    IL_00d0:  ldloca.s   V_0
    IL_00d2:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00d7:  ldloca.s   V_1
    IL_00d9:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00de:  and
    IL_00df:  br.s       IL_00e2

    IL_00e1:  ldc.i4.0
    IL_00e2:  nop
    IL_00e3:  ldc.i4.0
    IL_00e4:  ceq
    IL_00e6:  stloc.2
    IL_00e7:  ldloc.2
    IL_00e8:  brtrue.s   IL_00f2

    IL_00ea:  nop
    IL_00eb:  call       void [mscorlib]System.Console::WriteLine()
    IL_00f0:  nop
    IL_00f1:  nop
    IL_00f2:  ldarg.0
    IL_00f3:  stloc.0
    IL_00f4:  ldarg.1
    IL_00f5:  stloc.1
    IL_00f6:  ldloca.s   V_0
    IL_00f8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00fd:  ldloca.s   V_1
    IL_00ff:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0104:  call       bool [mscorlib]System.Decimal::op_GreaterThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                              valuetype [mscorlib]System.Decimal)
    IL_0109:  brfalse.s  IL_011c

    IL_010b:  ldloca.s   V_0
    IL_010d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0112:  ldloca.s   V_1
    IL_0114:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0119:  and
    IL_011a:  br.s       IL_011d

    IL_011c:  ldc.i4.0
    IL_011d:  nop
    IL_011e:  ldc.i4.0
    IL_011f:  ceq
    IL_0121:  stloc.2
    IL_0122:  ldloc.2
    IL_0123:  brtrue.s   IL_012d

    IL_0125:  nop
    IL_0126:  call       void [mscorlib]System.Console::WriteLine()
    IL_012b:  nop
    IL_012c:  nop
    IL_012d:  ldarg.0
    IL_012e:  stloc.0
    IL_012f:  ldarg.1
    IL_0130:  stloc.1
    IL_0131:  ldloca.s   V_0
    IL_0133:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0138:  ldloca.s   V_1
    IL_013a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_013f:  call       bool [mscorlib]System.Decimal::op_LessThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                           valuetype [mscorlib]System.Decimal)
    IL_0144:  brfalse.s  IL_0157

    IL_0146:  ldloca.s   V_0
    IL_0148:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_014d:  ldloca.s   V_1
    IL_014f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0154:  and
    IL_0155:  br.s       IL_0158

    IL_0157:  ldc.i4.0
    IL_0158:  nop
    IL_0159:  ldc.i4.0
    IL_015a:  ceq
    IL_015c:  stloc.2
    IL_015d:  ldloc.2
    IL_015e:  brtrue.s   IL_0168

    IL_0160:  nop
    IL_0161:  call       void [mscorlib]System.Console::WriteLine()
    IL_0166:  nop
    IL_0167:  nop
    IL_0168:  ldarg.0
    IL_0169:  stloc.0
    IL_016a:  ldarg.1
    IL_016b:  stloc.1
    IL_016c:  ldloca.s   V_0
    IL_016e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0173:  ldloca.s   V_1
    IL_0175:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_017a:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_017f:  brfalse.s  IL_0192

    IL_0181:  ldloca.s   V_0
    IL_0183:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0188:  ldloca.s   V_1
    IL_018a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_018f:  and
    IL_0190:  br.s       IL_0193

    IL_0192:  ldc.i4.0
    IL_0193:  nop
    IL_0194:  stloc.2
    IL_0195:  ldloc.2
    IL_0196:  brtrue.s   IL_01a0

    IL_0198:  nop
    IL_0199:  call       void [mscorlib]System.Console::WriteLine()
    IL_019e:  nop
    IL_019f:  nop
    IL_01a0:  ldarg.0
    IL_01a1:  stloc.0
    IL_01a2:  ldarg.1
    IL_01a3:  stloc.1
    IL_01a4:  ldloca.s   V_0
    IL_01a6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01ab:  ldloca.s   V_1
    IL_01ad:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01b2:  call       bool [mscorlib]System.Decimal::op_LessThan(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_01b7:  brfalse.s  IL_01ca

    IL_01b9:  ldloca.s   V_0
    IL_01bb:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_01c0:  ldloca.s   V_1
    IL_01c2:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_01c7:  and
    IL_01c8:  br.s       IL_01cb

    IL_01ca:  ldc.i4.0
    IL_01cb:  nop
    IL_01cc:  stloc.2
    IL_01cd:  ldloc.2
    IL_01ce:  brtrue.s   IL_01d8

    IL_01d0:  nop
    IL_01d1:  call       void [mscorlib]System.Console::WriteLine()
    IL_01d6:  nop
    IL_01d7:  nop
    IL_01d8:  ret
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
    // Code size       987 (0x3db)
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
    IL_0018:  brfalse.s  IL_002c

    IL_001a:  ldloca.s   V_0
    IL_001c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0021:  ldloca.s   V_1
    IL_0023:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0028:  ceq
    IL_002a:  br.s       IL_002d

    IL_002c:  ldc.i4.0
    IL_002d:  nop
    IL_002e:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0033:  nop
    IL_0034:  ldarg.0
    IL_0035:  stloc.0
    IL_0036:  ldarg.1
    IL_0037:  stloc.1
    IL_0038:  ldloca.s   V_0
    IL_003a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_003f:  ldloca.s   V_1
    IL_0041:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0046:  call       bool [mscorlib]System.Decimal::op_Inequality(valuetype [mscorlib]System.Decimal,
                                                                      valuetype [mscorlib]System.Decimal)
    IL_004b:  brtrue.s   IL_0062

    IL_004d:  ldloca.s   V_0
    IL_004f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0054:  ldloca.s   V_1
    IL_0056:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_005b:  ceq
    IL_005d:  ldc.i4.0
    IL_005e:  ceq
    IL_0060:  br.s       IL_0063

    IL_0062:  ldc.i4.1
    IL_0063:  nop
    IL_0064:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0069:  nop
    IL_006a:  ldarg.0
    IL_006b:  stloc.0
    IL_006c:  ldarg.1
    IL_006d:  stloc.1
    IL_006e:  ldloca.s   V_0
    IL_0070:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0075:  ldloca.s   V_1
    IL_0077:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_007c:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_0081:  brfalse.s  IL_0094

    IL_0083:  ldloca.s   V_0
    IL_0085:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_008a:  ldloca.s   V_1
    IL_008c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0091:  and
    IL_0092:  br.s       IL_0095

    IL_0094:  ldc.i4.0
    IL_0095:  nop
    IL_0096:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_009b:  nop
    IL_009c:  ldarg.0
    IL_009d:  stloc.0
    IL_009e:  ldarg.1
    IL_009f:  stloc.1
    IL_00a0:  ldloca.s   V_0
    IL_00a2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00a7:  ldloca.s   V_1
    IL_00a9:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00ae:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_00b3:  brfalse.s  IL_00c6

    IL_00b5:  ldloca.s   V_0
    IL_00b7:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00bc:  ldloca.s   V_1
    IL_00be:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00c3:  and
    IL_00c4:  br.s       IL_00c7

    IL_00c6:  ldc.i4.0
    IL_00c7:  nop
    IL_00c8:  ldc.i4.0
    IL_00c9:  ceq
    IL_00cb:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00d0:  nop
    IL_00d1:  ldarg.0
    IL_00d2:  stloc.0
    IL_00d3:  ldarg.1
    IL_00d4:  stloc.1
    IL_00d5:  ldloca.s   V_0
    IL_00d7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00dc:  ldloca.s   V_1
    IL_00de:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00e3:  call       bool [mscorlib]System.Decimal::op_LessThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                           valuetype [mscorlib]System.Decimal)
    IL_00e8:  brfalse.s  IL_00fb

    IL_00ea:  ldloca.s   V_0
    IL_00ec:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00f1:  ldloca.s   V_1
    IL_00f3:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00f8:  and
    IL_00f9:  br.s       IL_00fc

    IL_00fb:  ldc.i4.0
    IL_00fc:  nop
    IL_00fd:  ldc.i4.0
    IL_00fe:  ceq
    IL_0100:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0105:  nop
    IL_0106:  ldarg.0
    IL_0107:  stloc.0
    IL_0108:  ldarg.1
    IL_0109:  stloc.1
    IL_010a:  ldloca.s   V_0
    IL_010c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0111:  ldloca.s   V_1
    IL_0113:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0118:  and
    IL_0119:  brtrue.s   IL_0126

    IL_011b:  ldloca.s   V_2
    IL_011d:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0123:  ldloc.2
    IL_0124:  br.s       IL_013e

    IL_0126:  ldloca.s   V_0
    IL_0128:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_012d:  ldloca.s   V_1
    IL_012f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0134:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_0139:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_013e:  nop
    IL_013f:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0144:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0149:  nop
    IL_014a:  ldarg.0
    IL_014b:  stloc.0
    IL_014c:  ldarg.1
    IL_014d:  stloc.1
    IL_014e:  ldloca.s   V_0
    IL_0150:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0155:  ldloca.s   V_1
    IL_0157:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_015c:  and
    IL_015d:  brtrue.s   IL_016a

    IL_015f:  ldloca.s   V_2
    IL_0161:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0167:  ldloc.2
    IL_0168:  br.s       IL_0182

    IL_016a:  ldloca.s   V_0
    IL_016c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0171:  ldloca.s   V_1
    IL_0173:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0178:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Subtraction(valuetype [mscorlib]System.Decimal,
                                                                                                     valuetype [mscorlib]System.Decimal)
    IL_017d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0182:  nop
    IL_0183:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0188:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_018d:  nop
    IL_018e:  ldarg.0
    IL_018f:  stloc.0
    IL_0190:  ldarg.1
    IL_0191:  stloc.1
    IL_0192:  ldloca.s   V_0
    IL_0194:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0199:  ldloca.s   V_1
    IL_019b:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_01a0:  and
    IL_01a1:  brtrue.s   IL_01ae

    IL_01a3:  ldloca.s   V_2
    IL_01a5:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_01ab:  ldloc.2
    IL_01ac:  br.s       IL_01c6

    IL_01ae:  ldloca.s   V_0
    IL_01b0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01b5:  ldloca.s   V_1
    IL_01b7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01bc:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Multiply(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_01c1:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_01c6:  nop
    IL_01c7:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_01cc:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01d1:  nop
    IL_01d2:  ldarg.0
    IL_01d3:  stloc.0
    IL_01d4:  ldarg.1
    IL_01d5:  stloc.1
    IL_01d6:  ldloca.s   V_0
    IL_01d8:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_01dd:  ldloca.s   V_1
    IL_01df:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_01e4:  and
    IL_01e5:  brtrue.s   IL_01f2

    IL_01e7:  ldloca.s   V_2
    IL_01e9:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_01ef:  ldloc.2
    IL_01f0:  br.s       IL_020a

    IL_01f2:  ldloca.s   V_0
    IL_01f4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01f9:  ldloca.s   V_1
    IL_01fb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0200:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Division(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_0205:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_020a:  nop
    IL_020b:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0210:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0215:  nop
    IL_0216:  ldarg.0
    IL_0217:  stloc.0
    IL_0218:  ldarg.1
    IL_0219:  stloc.1
    IL_021a:  ldloca.s   V_0
    IL_021c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0221:  ldloca.s   V_1
    IL_0223:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0228:  and
    IL_0229:  brtrue.s   IL_0236

    IL_022b:  ldloca.s   V_2
    IL_022d:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0233:  ldloc.2
    IL_0234:  br.s       IL_024e

    IL_0236:  ldloca.s   V_0
    IL_0238:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_023d:  ldloca.s   V_1
    IL_023f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0244:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Modulus(valuetype [mscorlib]System.Decimal,
                                                                                                 valuetype [mscorlib]System.Decimal)
    IL_0249:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_024e:  nop
    IL_024f:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0254:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0259:  nop
    IL_025a:  ldarg.0
    IL_025b:  stloc.0
    IL_025c:  ldloca.s   V_0
    IL_025e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0263:  brtrue.s   IL_0268

    IL_0265:  ldarg.1
    IL_0266:  br.s       IL_0274

    IL_0268:  ldloca.s   V_0
    IL_026a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_026f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0274:  nop
    IL_0275:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_027a:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_027f:  nop
    IL_0280:  ldarg.0
    IL_0281:  stloc.0
    IL_0282:  ldloca.s   V_0
    IL_0284:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0289:  brtrue.s   IL_0296

    IL_028b:  ldloca.s   V_1
    IL_028d:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0293:  ldloc.1
    IL_0294:  br.s       IL_02a7

    IL_0296:  ldloca.s   V_0
    IL_0298:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_029d:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_UnaryNegation(valuetype [mscorlib]System.Decimal)
    IL_02a2:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_02a7:  nop
    IL_02a8:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_02ad:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02b2:  nop
    IL_02b3:  ldarg.0
    IL_02b4:  stloc.0
    IL_02b5:  ldarg.1
    IL_02b6:  stloc.1
    IL_02b7:  ldloca.s   V_0
    IL_02b9:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_02be:  ldloca.s   V_1
    IL_02c0:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_02c5:  and
    IL_02c6:  brtrue.s   IL_02d3

    IL_02c8:  ldloca.s   V_2
    IL_02ca:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_02d0:  ldloc.2
    IL_02d1:  br.s       IL_02eb

    IL_02d3:  ldloca.s   V_0
    IL_02d5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_02da:  ldloca.s   V_1
    IL_02dc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_02e1:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_02e6:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_02eb:  nop
    IL_02ec:  starg.s    a
    IL_02ee:  ldarg.0
    IL_02ef:  stloc.0
    IL_02f0:  ldarg.1
    IL_02f1:  stloc.1
    IL_02f2:  ldloca.s   V_0
    IL_02f4:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_02f9:  ldloca.s   V_1
    IL_02fb:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0300:  and
    IL_0301:  brtrue.s   IL_030e

    IL_0303:  ldloca.s   V_2
    IL_0305:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_030b:  ldloc.2
    IL_030c:  br.s       IL_0326

    IL_030e:  ldloca.s   V_0
    IL_0310:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0315:  ldloca.s   V_1
    IL_0317:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_031c:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Subtraction(valuetype [mscorlib]System.Decimal,
                                                                                                     valuetype [mscorlib]System.Decimal)
    IL_0321:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0326:  nop
    IL_0327:  starg.s    a
    IL_0329:  ldarg.0
    IL_032a:  stloc.0
    IL_032b:  ldarg.1
    IL_032c:  stloc.1
    IL_032d:  ldloca.s   V_0
    IL_032f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0334:  ldloca.s   V_1
    IL_0336:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_033b:  and
    IL_033c:  brtrue.s   IL_0349

    IL_033e:  ldloca.s   V_2
    IL_0340:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0346:  ldloc.2
    IL_0347:  br.s       IL_0361

    IL_0349:  ldloca.s   V_0
    IL_034b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0350:  ldloca.s   V_1
    IL_0352:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0357:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Multiply(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_035c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0361:  nop
    IL_0362:  starg.s    a
    IL_0364:  ldarg.0
    IL_0365:  stloc.0
    IL_0366:  ldarg.1
    IL_0367:  stloc.1
    IL_0368:  ldloca.s   V_0
    IL_036a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_036f:  ldloca.s   V_1
    IL_0371:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0376:  and
    IL_0377:  brtrue.s   IL_0384

    IL_0379:  ldloca.s   V_2
    IL_037b:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0381:  ldloc.2
    IL_0382:  br.s       IL_039c

    IL_0384:  ldloca.s   V_0
    IL_0386:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_038b:  ldloca.s   V_1
    IL_038d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0392:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Division(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_0397:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_039c:  nop
    IL_039d:  starg.s    a
    IL_039f:  ldarg.0
    IL_03a0:  stloc.0
    IL_03a1:  ldarg.1
    IL_03a2:  stloc.1
    IL_03a3:  ldloca.s   V_0
    IL_03a5:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_03aa:  ldloca.s   V_1
    IL_03ac:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_03b1:  and
    IL_03b2:  brtrue.s   IL_03bf

    IL_03b4:  ldloca.s   V_2
    IL_03b6:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_03bc:  ldloc.2
    IL_03bd:  br.s       IL_03d7

    IL_03bf:  ldloca.s   V_0
    IL_03c1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_03c6:  ldloca.s   V_1
    IL_03c8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_03cd:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Modulus(valuetype [mscorlib]System.Decimal,
                                                                                                 valuetype [mscorlib]System.Decimal)
    IL_03d2:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_03d7:  nop
    IL_03d8:  starg.s    a
    IL_03da:  ret
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
    // Code size       113 (0x71)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int64> V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldarg.1
    IL_0004:  stloc.1
    IL_0005:  ldloca.s   V_0
    IL_0007:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000c:  conv.i8
    IL_000d:  ldloca.s   V_1
    IL_000f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0014:  bge.s      IL_0027

    IL_0016:  ldloca.s   V_0
    IL_0018:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001d:  ldloca.s   V_1
    IL_001f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0024:  and
    IL_0025:  br.s       IL_0028

    IL_0027:  ldc.i4.0
    IL_0028:  nop
    IL_0029:  ldc.i4.0
    IL_002a:  ceq
    IL_002c:  stloc.2
    IL_002d:  ldloc.2
    IL_002e:  brtrue.s   IL_0038

    IL_0030:  nop
    IL_0031:  call       void [mscorlib]System.Console::WriteLine()
    IL_0036:  nop
    IL_0037:  nop
    IL_0038:  ldarg.0
    IL_0039:  stloc.0
    IL_003a:  ldarg.1
    IL_003b:  stloc.1
    IL_003c:  ldloca.s   V_0
    IL_003e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0043:  conv.i8
    IL_0044:  ldloca.s   V_1
    IL_0046:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_004b:  bne.un.s   IL_005f

    IL_004d:  ldloca.s   V_0
    IL_004f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0054:  ldloca.s   V_1
    IL_0056:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_005b:  ceq
    IL_005d:  br.s       IL_0060

    IL_005f:  ldc.i4.0
    IL_0060:  nop
    IL_0061:  ldc.i4.0
    IL_0062:  ceq
    IL_0064:  stloc.2
    IL_0065:  ldloc.2
    IL_0066:  brtrue.s   IL_0070

    IL_0068:  nop
    IL_0069:  call       void [mscorlib]System.Console::WriteLine()
    IL_006e:  nop
    IL_006f:  nop
    IL_0070:  ret
  } // end of method LiftedOperators::CompareWithImplictCast

  .method public hidebysig static void  CompareWithSignChange(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                              valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       124 (0x7c)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<uint32> V_1,
             valuetype [mscorlib]System.Nullable`1<uint32> V_2,
             bool V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_0017

    IL_000c:  ldloca.s   V_1
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_0014:  ldloc.1
    IL_0015:  br.s       IL_0023

    IL_0017:  ldloca.s   V_0
    IL_0019:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_0023:  nop
    IL_0024:  stloc.1
    IL_0025:  ldarg.1
    IL_0026:  stloc.0
    IL_0027:  ldloca.s   V_0
    IL_0029:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_002e:  brtrue.s   IL_003b

    IL_0030:  ldloca.s   V_2
    IL_0032:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_0038:  ldloc.2
    IL_0039:  br.s       IL_0047

    IL_003b:  ldloca.s   V_0
    IL_003d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0042:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_0047:  nop
    IL_0048:  stloc.2
    IL_0049:  ldloca.s   V_1
    IL_004b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint32>::GetValueOrDefault()
    IL_0050:  ldloca.s   V_2
    IL_0052:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint32>::GetValueOrDefault()
    IL_0057:  bge.un.s   IL_006a

    IL_0059:  ldloca.s   V_1
    IL_005b:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint32>::get_HasValue()
    IL_0060:  ldloca.s   V_2
    IL_0062:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint32>::get_HasValue()
    IL_0067:  and
    IL_0068:  br.s       IL_006b

    IL_006a:  ldc.i4.0
    IL_006b:  nop
    IL_006c:  ldc.i4.0
    IL_006d:  ceq
    IL_006f:  stloc.3
    IL_0070:  ldloc.3
    IL_0071:  brtrue.s   IL_007b

    IL_0073:  nop
    IL_0074:  call       void [mscorlib]System.Console::WriteLine()
    IL_0079:  nop
    IL_007a:  nop
    IL_007b:  ret
  } // end of method LiftedOperators::CompareWithSignChange

  .method public hidebysig static void  StructBasic(valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> a,
                                                    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> b) cil managed
  {
    // Code size       572 (0x23c)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_0,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS> V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldarg.1
    IL_0004:  stloc.1
    IL_0005:  ldloca.s   V_0
    IL_0007:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_000c:  ldloca.s   V_1
    IL_000e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0013:  bne.un.s   IL_0037

    IL_0015:  ldloca.s   V_0
    IL_0017:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_001c:  brfalse.s  IL_0033

    IL_001e:  ldloca.s   V_0
    IL_0020:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0025:  ldloca.s   V_1
    IL_0027:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_002c:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Equality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0031:  br.s       IL_0034

    IL_0033:  ldc.i4.1
    IL_0034:  nop
    IL_0035:  br.s       IL_0038

    IL_0037:  ldc.i4.0
    IL_0038:  nop
    IL_0039:  ldc.i4.0
    IL_003a:  ceq
    IL_003c:  stloc.2
    IL_003d:  ldloc.2
    IL_003e:  brtrue.s   IL_0048

    IL_0040:  nop
    IL_0041:  call       void [mscorlib]System.Console::WriteLine()
    IL_0046:  nop
    IL_0047:  nop
    IL_0048:  ldarg.0
    IL_0049:  stloc.0
    IL_004a:  ldarg.1
    IL_004b:  stloc.1
    IL_004c:  ldloca.s   V_0
    IL_004e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0053:  ldloca.s   V_1
    IL_0055:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_005a:  bne.un.s   IL_007e

    IL_005c:  ldloca.s   V_0
    IL_005e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0063:  brfalse.s  IL_007a

    IL_0065:  ldloca.s   V_0
    IL_0067:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_006c:  ldloca.s   V_1
    IL_006e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0073:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Inequality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0078:  br.s       IL_007b

    IL_007a:  ldc.i4.0
    IL_007b:  nop
    IL_007c:  br.s       IL_007f

    IL_007e:  ldc.i4.1
    IL_007f:  nop
    IL_0080:  ldc.i4.0
    IL_0081:  ceq
    IL_0083:  stloc.2
    IL_0084:  ldloc.2
    IL_0085:  brtrue.s   IL_008f

    IL_0087:  nop
    IL_0088:  call       void [mscorlib]System.Console::WriteLine()
    IL_008d:  nop
    IL_008e:  nop
    IL_008f:  ldarg.0
    IL_0090:  stloc.0
    IL_0091:  ldarg.1
    IL_0092:  stloc.1
    IL_0093:  ldloca.s   V_0
    IL_0095:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_009a:  ldloca.s   V_1
    IL_009c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00a1:  and
    IL_00a2:  brtrue.s   IL_00a7

    IL_00a4:  ldc.i4.0
    IL_00a5:  br.s       IL_00ba

    IL_00a7:  ldloca.s   V_0
    IL_00a9:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00ae:  ldloca.s   V_1
    IL_00b0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00b5:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_GreaterThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_00ba:  nop
    IL_00bb:  ldc.i4.0
    IL_00bc:  ceq
    IL_00be:  stloc.2
    IL_00bf:  ldloc.2
    IL_00c0:  brtrue.s   IL_00ca

    IL_00c2:  nop
    IL_00c3:  call       void [mscorlib]System.Console::WriteLine()
    IL_00c8:  nop
    IL_00c9:  nop
    IL_00ca:  ldarg.0
    IL_00cb:  stloc.0
    IL_00cc:  ldarg.1
    IL_00cd:  stloc.1
    IL_00ce:  ldloca.s   V_0
    IL_00d0:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00d5:  ldloca.s   V_1
    IL_00d7:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00dc:  and
    IL_00dd:  brtrue.s   IL_00e2

    IL_00df:  ldc.i4.0
    IL_00e0:  br.s       IL_00f5

    IL_00e2:  ldloca.s   V_0
    IL_00e4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00e9:  ldloca.s   V_1
    IL_00eb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00f0:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_LessThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_00f5:  nop
    IL_00f6:  ldc.i4.0
    IL_00f7:  ceq
    IL_00f9:  stloc.2
    IL_00fa:  ldloc.2
    IL_00fb:  brtrue.s   IL_0105

    IL_00fd:  nop
    IL_00fe:  call       void [mscorlib]System.Console::WriteLine()
    IL_0103:  nop
    IL_0104:  nop
    IL_0105:  ldarg.0
    IL_0106:  stloc.0
    IL_0107:  ldarg.1
    IL_0108:  stloc.1
    IL_0109:  ldloca.s   V_0
    IL_010b:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0110:  ldloca.s   V_1
    IL_0112:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0117:  and
    IL_0118:  brtrue.s   IL_011d

    IL_011a:  ldc.i4.0
    IL_011b:  br.s       IL_0130

    IL_011d:  ldloca.s   V_0
    IL_011f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0124:  ldloca.s   V_1
    IL_0126:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_012b:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_GreaterThanOrEqual(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0130:  nop
    IL_0131:  ldc.i4.0
    IL_0132:  ceq
    IL_0134:  stloc.2
    IL_0135:  ldloc.2
    IL_0136:  brtrue.s   IL_0140

    IL_0138:  nop
    IL_0139:  call       void [mscorlib]System.Console::WriteLine()
    IL_013e:  nop
    IL_013f:  nop
    IL_0140:  ldarg.0
    IL_0141:  stloc.0
    IL_0142:  ldarg.1
    IL_0143:  stloc.1
    IL_0144:  ldloca.s   V_0
    IL_0146:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_014b:  ldloca.s   V_1
    IL_014d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0152:  and
    IL_0153:  brtrue.s   IL_0158

    IL_0155:  ldc.i4.0
    IL_0156:  br.s       IL_016b

    IL_0158:  ldloca.s   V_0
    IL_015a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_015f:  ldloca.s   V_1
    IL_0161:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0166:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_LessThanOrEqual(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                   valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_016b:  nop
    IL_016c:  ldc.i4.0
    IL_016d:  ceq
    IL_016f:  stloc.2
    IL_0170:  ldloc.2
    IL_0171:  brtrue.s   IL_017b

    IL_0173:  nop
    IL_0174:  call       void [mscorlib]System.Console::WriteLine()
    IL_0179:  nop
    IL_017a:  nop
    IL_017b:  ldarg.0
    IL_017c:  stloc.0
    IL_017d:  ldarg.1
    IL_017e:  stloc.1
    IL_017f:  ldloca.s   V_0
    IL_0181:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0186:  ldloca.s   V_1
    IL_0188:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_018d:  bne.un.s   IL_01b1

    IL_018f:  ldloca.s   V_0
    IL_0191:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0196:  brfalse.s  IL_01ad

    IL_0198:  ldloca.s   V_0
    IL_019a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_019f:  ldloca.s   V_1
    IL_01a1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_01a6:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Equality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_01ab:  br.s       IL_01ae

    IL_01ad:  ldc.i4.1
    IL_01ae:  nop
    IL_01af:  br.s       IL_01b2

    IL_01b1:  ldc.i4.0
    IL_01b2:  nop
    IL_01b3:  stloc.2
    IL_01b4:  ldloc.2
    IL_01b5:  brtrue.s   IL_01bf

    IL_01b7:  nop
    IL_01b8:  call       void [mscorlib]System.Console::WriteLine()
    IL_01bd:  nop
    IL_01be:  nop
    IL_01bf:  ldarg.0
    IL_01c0:  stloc.0
    IL_01c1:  ldarg.1
    IL_01c2:  stloc.1
    IL_01c3:  ldloca.s   V_0
    IL_01c5:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01ca:  ldloca.s   V_1
    IL_01cc:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01d1:  bne.un.s   IL_01f5

    IL_01d3:  ldloca.s   V_0
    IL_01d5:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01da:  brfalse.s  IL_01f1

    IL_01dc:  ldloca.s   V_0
    IL_01de:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_01e3:  ldloca.s   V_1
    IL_01e5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_01ea:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Inequality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_01ef:  br.s       IL_01f2

    IL_01f1:  ldc.i4.0
    IL_01f2:  nop
    IL_01f3:  br.s       IL_01f6

    IL_01f5:  ldc.i4.1
    IL_01f6:  nop
    IL_01f7:  stloc.2
    IL_01f8:  ldloc.2
    IL_01f9:  brtrue.s   IL_0203

    IL_01fb:  nop
    IL_01fc:  call       void [mscorlib]System.Console::WriteLine()
    IL_0201:  nop
    IL_0202:  nop
    IL_0203:  ldarg.0
    IL_0204:  stloc.0
    IL_0205:  ldarg.1
    IL_0206:  stloc.1
    IL_0207:  ldloca.s   V_0
    IL_0209:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_020e:  ldloca.s   V_1
    IL_0210:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0215:  and
    IL_0216:  brtrue.s   IL_021b

    IL_0218:  ldc.i4.0
    IL_0219:  br.s       IL_022e

    IL_021b:  ldloca.s   V_0
    IL_021d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0222:  ldloca.s   V_1
    IL_0224:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0229:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_GreaterThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_022e:  nop
    IL_022f:  stloc.2
    IL_0230:  ldloc.2
    IL_0231:  brtrue.s   IL_023b

    IL_0233:  nop
    IL_0234:  call       void [mscorlib]System.Console::WriteLine()
    IL_0239:  nop
    IL_023a:  nop
    IL_023b:  ret
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
    // Code size       1871 (0x74f)
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
    IL_0013:  bne.un.s   IL_0037

    IL_0015:  ldloca.s   V_0
    IL_0017:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_001c:  brfalse.s  IL_0033

    IL_001e:  ldloca.s   V_0
    IL_0020:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0025:  ldloca.s   V_1
    IL_0027:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_002c:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Equality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0031:  br.s       IL_0034

    IL_0033:  ldc.i4.1
    IL_0034:  nop
    IL_0035:  br.s       IL_0038

    IL_0037:  ldc.i4.0
    IL_0038:  nop
    IL_0039:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_003e:  nop
    IL_003f:  ldarg.0
    IL_0040:  stloc.0
    IL_0041:  ldarg.1
    IL_0042:  stloc.1
    IL_0043:  ldloca.s   V_0
    IL_0045:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_004a:  ldloca.s   V_1
    IL_004c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0051:  bne.un.s   IL_0075

    IL_0053:  ldloca.s   V_0
    IL_0055:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_005a:  brfalse.s  IL_0071

    IL_005c:  ldloca.s   V_0
    IL_005e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0063:  ldloca.s   V_1
    IL_0065:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_006a:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Inequality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_006f:  br.s       IL_0072

    IL_0071:  ldc.i4.0
    IL_0072:  nop
    IL_0073:  br.s       IL_0076

    IL_0075:  ldc.i4.1
    IL_0076:  nop
    IL_0077:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_007c:  nop
    IL_007d:  ldarg.0
    IL_007e:  stloc.0
    IL_007f:  ldarg.1
    IL_0080:  stloc.1
    IL_0081:  ldloca.s   V_0
    IL_0083:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0088:  ldloca.s   V_1
    IL_008a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_008f:  and
    IL_0090:  brtrue.s   IL_0095

    IL_0092:  ldc.i4.0
    IL_0093:  br.s       IL_00a8

    IL_0095:  ldloca.s   V_0
    IL_0097:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_009c:  ldloca.s   V_1
    IL_009e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00a3:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_GreaterThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_00a8:  nop
    IL_00a9:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00ae:  nop
    IL_00af:  ldarg.0
    IL_00b0:  stloc.0
    IL_00b1:  ldarg.1
    IL_00b2:  stloc.1
    IL_00b3:  ldloca.s   V_0
    IL_00b5:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00ba:  ldloca.s   V_1
    IL_00bc:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00c1:  bne.un.s   IL_00e5

    IL_00c3:  ldloca.s   V_0
    IL_00c5:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00ca:  brfalse.s  IL_00e1

    IL_00cc:  ldloca.s   V_0
    IL_00ce:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00d3:  ldloca.s   V_1
    IL_00d5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_00da:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Equality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_00df:  br.s       IL_00e2

    IL_00e1:  ldc.i4.1
    IL_00e2:  nop
    IL_00e3:  br.s       IL_00e6

    IL_00e5:  ldc.i4.0
    IL_00e6:  nop
    IL_00e7:  ldc.i4.0
    IL_00e8:  ceq
    IL_00ea:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00ef:  nop
    IL_00f0:  ldarg.0
    IL_00f1:  stloc.0
    IL_00f2:  ldarg.1
    IL_00f3:  stloc.1
    IL_00f4:  ldloca.s   V_0
    IL_00f6:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_00fb:  ldloca.s   V_1
    IL_00fd:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0102:  bne.un.s   IL_0126

    IL_0104:  ldloca.s   V_0
    IL_0106:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_010b:  brfalse.s  IL_0122

    IL_010d:  ldloca.s   V_0
    IL_010f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0114:  ldloca.s   V_1
    IL_0116:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_011b:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Inequality(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0120:  br.s       IL_0123

    IL_0122:  ldc.i4.0
    IL_0123:  nop
    IL_0124:  br.s       IL_0127

    IL_0126:  ldc.i4.1
    IL_0127:  nop
    IL_0128:  ldc.i4.0
    IL_0129:  ceq
    IL_012b:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0130:  nop
    IL_0131:  ldarg.0
    IL_0132:  stloc.0
    IL_0133:  ldarg.1
    IL_0134:  stloc.1
    IL_0135:  ldloca.s   V_0
    IL_0137:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_013c:  ldloca.s   V_1
    IL_013e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0143:  and
    IL_0144:  brtrue.s   IL_0149

    IL_0146:  ldc.i4.0
    IL_0147:  br.s       IL_015c

    IL_0149:  ldloca.s   V_0
    IL_014b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0150:  ldloca.s   V_1
    IL_0152:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0157:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_GreaterThan(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_015c:  nop
    IL_015d:  ldc.i4.0
    IL_015e:  ceq
    IL_0160:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0165:  nop
    IL_0166:  ldarg.0
    IL_0167:  stloc.0
    IL_0168:  ldarg.1
    IL_0169:  stloc.1
    IL_016a:  ldloca.s   V_0
    IL_016c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0171:  ldloca.s   V_1
    IL_0173:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0178:  and
    IL_0179:  brtrue.s   IL_0186

    IL_017b:  ldloca.s   V_2
    IL_017d:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0183:  ldloc.2
    IL_0184:  br.s       IL_019e

    IL_0186:  ldloca.s   V_0
    IL_0188:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_018d:  ldloca.s   V_1
    IL_018f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0194:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Addition(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0199:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_019e:  nop
    IL_019f:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_01a4:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01a9:  nop
    IL_01aa:  ldarg.0
    IL_01ab:  stloc.0
    IL_01ac:  ldarg.1
    IL_01ad:  stloc.1
    IL_01ae:  ldloca.s   V_0
    IL_01b0:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01b5:  ldloca.s   V_1
    IL_01b7:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01bc:  and
    IL_01bd:  brtrue.s   IL_01ca

    IL_01bf:  ldloca.s   V_2
    IL_01c1:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_01c7:  ldloc.2
    IL_01c8:  br.s       IL_01e2

    IL_01ca:  ldloca.s   V_0
    IL_01cc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_01d1:  ldloca.s   V_1
    IL_01d3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_01d8:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Subtraction(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_01dd:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_01e2:  nop
    IL_01e3:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_01e8:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01ed:  nop
    IL_01ee:  ldarg.0
    IL_01ef:  stloc.0
    IL_01f0:  ldarg.1
    IL_01f1:  stloc.1
    IL_01f2:  ldloca.s   V_0
    IL_01f4:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_01f9:  ldloca.s   V_1
    IL_01fb:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0200:  and
    IL_0201:  brtrue.s   IL_020e

    IL_0203:  ldloca.s   V_2
    IL_0205:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_020b:  ldloc.2
    IL_020c:  br.s       IL_0226

    IL_020e:  ldloca.s   V_0
    IL_0210:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0215:  ldloca.s   V_1
    IL_0217:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_021c:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Multiply(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0221:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0226:  nop
    IL_0227:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_022c:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0231:  nop
    IL_0232:  ldarg.0
    IL_0233:  stloc.0
    IL_0234:  ldarg.1
    IL_0235:  stloc.1
    IL_0236:  ldloca.s   V_0
    IL_0238:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_023d:  ldloca.s   V_1
    IL_023f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0244:  and
    IL_0245:  brtrue.s   IL_0252

    IL_0247:  ldloca.s   V_2
    IL_0249:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_024f:  ldloc.2
    IL_0250:  br.s       IL_026a

    IL_0252:  ldloca.s   V_0
    IL_0254:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0259:  ldloca.s   V_1
    IL_025b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0260:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Division(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0265:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_026a:  nop
    IL_026b:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0270:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0275:  nop
    IL_0276:  ldarg.0
    IL_0277:  stloc.0
    IL_0278:  ldarg.1
    IL_0279:  stloc.1
    IL_027a:  ldloca.s   V_0
    IL_027c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0281:  ldloca.s   V_1
    IL_0283:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0288:  and
    IL_0289:  brtrue.s   IL_0296

    IL_028b:  ldloca.s   V_2
    IL_028d:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0293:  ldloc.2
    IL_0294:  br.s       IL_02ae

    IL_0296:  ldloca.s   V_0
    IL_0298:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_029d:  ldloca.s   V_1
    IL_029f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_02a4:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Modulus(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                 valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_02a9:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_02ae:  nop
    IL_02af:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_02b4:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02b9:  nop
    IL_02ba:  ldarg.0
    IL_02bb:  stloc.0
    IL_02bc:  ldarg.1
    IL_02bd:  stloc.1
    IL_02be:  ldloca.s   V_0
    IL_02c0:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_02c5:  ldloca.s   V_1
    IL_02c7:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_02cc:  and
    IL_02cd:  brtrue.s   IL_02da

    IL_02cf:  ldloca.s   V_2
    IL_02d1:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_02d7:  ldloc.2
    IL_02d8:  br.s       IL_02f2

    IL_02da:  ldloca.s   V_0
    IL_02dc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_02e1:  ldloca.s   V_1
    IL_02e3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_02e8:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                    valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_02ed:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_02f2:  nop
    IL_02f3:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_02f8:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02fd:  nop
    IL_02fe:  ldarg.0
    IL_02ff:  stloc.0
    IL_0300:  ldarg.1
    IL_0301:  stloc.1
    IL_0302:  ldloca.s   V_0
    IL_0304:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0309:  ldloca.s   V_1
    IL_030b:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0310:  and
    IL_0311:  brtrue.s   IL_031e

    IL_0313:  ldloca.s   V_2
    IL_0315:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_031b:  ldloc.2
    IL_031c:  br.s       IL_0336

    IL_031e:  ldloca.s   V_0
    IL_0320:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0325:  ldloca.s   V_1
    IL_0327:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_032c:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                   valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0331:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0336:  nop
    IL_0337:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_033c:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0341:  nop
    IL_0342:  ldarg.0
    IL_0343:  stloc.0
    IL_0344:  ldarg.1
    IL_0345:  stloc.1
    IL_0346:  ldloca.s   V_0
    IL_0348:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_034d:  ldloca.s   V_1
    IL_034f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0354:  and
    IL_0355:  brtrue.s   IL_0362

    IL_0357:  ldloca.s   V_2
    IL_0359:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_035f:  ldloc.2
    IL_0360:  br.s       IL_037a

    IL_0362:  ldloca.s   V_0
    IL_0364:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0369:  ldloca.s   V_1
    IL_036b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0370:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_ExclusiveOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0375:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_037a:  nop
    IL_037b:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0380:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0385:  nop
    IL_0386:  ldarg.0
    IL_0387:  stloc.0
    IL_0388:  ldarg.2
    IL_0389:  stloc.3
    IL_038a:  ldloca.s   V_0
    IL_038c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0391:  ldloca.s   V_3
    IL_0393:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0398:  and
    IL_0399:  brtrue.s   IL_03a6

    IL_039b:  ldloca.s   V_1
    IL_039d:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_03a3:  ldloc.1
    IL_03a4:  br.s       IL_03be

    IL_03a6:  ldloca.s   V_0
    IL_03a8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_03ad:  ldloca.s   V_3
    IL_03af:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03b4:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_LeftShift(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                   int32)
    IL_03b9:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_03be:  nop
    IL_03bf:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_03c4:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_03c9:  nop
    IL_03ca:  ldarg.0
    IL_03cb:  stloc.0
    IL_03cc:  ldarg.2
    IL_03cd:  stloc.3
    IL_03ce:  ldloca.s   V_0
    IL_03d0:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_03d5:  ldloca.s   V_3
    IL_03d7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03dc:  and
    IL_03dd:  brtrue.s   IL_03ea

    IL_03df:  ldloca.s   V_1
    IL_03e1:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_03e7:  ldloc.1
    IL_03e8:  br.s       IL_0402

    IL_03ea:  ldloca.s   V_0
    IL_03ec:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_03f1:  ldloca.s   V_3
    IL_03f3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03f8:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_RightShift(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                    int32)
    IL_03fd:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0402:  nop
    IL_0403:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0408:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_040d:  nop
    IL_040e:  ldarg.0
    IL_040f:  stloc.0
    IL_0410:  ldloca.s   V_0
    IL_0412:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0417:  brtrue.s   IL_041c

    IL_0419:  ldarg.1
    IL_041a:  br.s       IL_0428

    IL_041c:  ldloca.s   V_0
    IL_041e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0423:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0428:  nop
    IL_0429:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_042e:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0433:  nop
    IL_0434:  ldarg.0
    IL_0435:  stloc.0
    IL_0436:  ldloca.s   V_0
    IL_0438:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_043d:  brtrue.s   IL_044a

    IL_043f:  ldloca.s   V_1
    IL_0441:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0447:  ldloc.1
    IL_0448:  br.s       IL_045b

    IL_044a:  ldloca.s   V_0
    IL_044c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0451:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_UnaryPlus(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0456:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_045b:  nop
    IL_045c:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0461:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0466:  nop
    IL_0467:  ldarg.0
    IL_0468:  stloc.0
    IL_0469:  ldloca.s   V_0
    IL_046b:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0470:  brtrue.s   IL_047d

    IL_0472:  ldloca.s   V_1
    IL_0474:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_047a:  ldloc.1
    IL_047b:  br.s       IL_048e

    IL_047d:  ldloca.s   V_0
    IL_047f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0484:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_UnaryNegation(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0489:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_048e:  nop
    IL_048f:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0494:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0499:  nop
    IL_049a:  ldarg.0
    IL_049b:  stloc.0
    IL_049c:  ldloca.s   V_0
    IL_049e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_04a3:  brtrue.s   IL_04b0

    IL_04a5:  ldloca.s   V_1
    IL_04a7:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_04ad:  ldloc.1
    IL_04ae:  br.s       IL_04c1

    IL_04b0:  ldloca.s   V_0
    IL_04b2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_04b7:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_LogicalNot(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_04bc:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_04c1:  nop
    IL_04c2:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_04c7:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_04cc:  nop
    IL_04cd:  ldarg.0
    IL_04ce:  stloc.0
    IL_04cf:  ldloca.s   V_0
    IL_04d1:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_04d6:  brtrue.s   IL_04e3

    IL_04d8:  ldloca.s   V_1
    IL_04da:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_04e0:  ldloc.1
    IL_04e1:  br.s       IL_04f4

    IL_04e3:  ldloca.s   V_0
    IL_04e5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_04ea:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_OnesComplement(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_04ef:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_04f4:  nop
    IL_04f5:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_04fa:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_04ff:  nop
    IL_0500:  ldarg.0
    IL_0501:  stloc.0
    IL_0502:  ldarg.1
    IL_0503:  stloc.1
    IL_0504:  ldloca.s   V_0
    IL_0506:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_050b:  ldloca.s   V_1
    IL_050d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0512:  and
    IL_0513:  brtrue.s   IL_0520

    IL_0515:  ldloca.s   V_2
    IL_0517:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_051d:  ldloc.2
    IL_051e:  br.s       IL_0538

    IL_0520:  ldloca.s   V_0
    IL_0522:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0527:  ldloca.s   V_1
    IL_0529:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_052e:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Addition(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0533:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0538:  nop
    IL_0539:  starg.s    a
    IL_053b:  ldarg.0
    IL_053c:  stloc.0
    IL_053d:  ldarg.1
    IL_053e:  stloc.1
    IL_053f:  ldloca.s   V_0
    IL_0541:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0546:  ldloca.s   V_1
    IL_0548:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_054d:  and
    IL_054e:  brtrue.s   IL_055b

    IL_0550:  ldloca.s   V_2
    IL_0552:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0558:  ldloc.2
    IL_0559:  br.s       IL_0573

    IL_055b:  ldloca.s   V_0
    IL_055d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0562:  ldloca.s   V_1
    IL_0564:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0569:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Subtraction(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_056e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0573:  nop
    IL_0574:  starg.s    a
    IL_0576:  ldarg.0
    IL_0577:  stloc.0
    IL_0578:  ldarg.1
    IL_0579:  stloc.1
    IL_057a:  ldloca.s   V_0
    IL_057c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0581:  ldloca.s   V_1
    IL_0583:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0588:  and
    IL_0589:  brtrue.s   IL_0596

    IL_058b:  ldloca.s   V_2
    IL_058d:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0593:  ldloc.2
    IL_0594:  br.s       IL_05ae

    IL_0596:  ldloca.s   V_0
    IL_0598:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_059d:  ldloca.s   V_1
    IL_059f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_05a4:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Multiply(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_05a9:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_05ae:  nop
    IL_05af:  starg.s    a
    IL_05b1:  ldarg.0
    IL_05b2:  stloc.0
    IL_05b3:  ldarg.1
    IL_05b4:  stloc.1
    IL_05b5:  ldloca.s   V_0
    IL_05b7:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_05bc:  ldloca.s   V_1
    IL_05be:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_05c3:  and
    IL_05c4:  brtrue.s   IL_05d1

    IL_05c6:  ldloca.s   V_2
    IL_05c8:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_05ce:  ldloc.2
    IL_05cf:  br.s       IL_05e9

    IL_05d1:  ldloca.s   V_0
    IL_05d3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_05d8:  ldloca.s   V_1
    IL_05da:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_05df:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Division(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                  valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_05e4:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_05e9:  nop
    IL_05ea:  starg.s    a
    IL_05ec:  ldarg.0
    IL_05ed:  stloc.0
    IL_05ee:  ldarg.1
    IL_05ef:  stloc.1
    IL_05f0:  ldloca.s   V_0
    IL_05f2:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_05f7:  ldloca.s   V_1
    IL_05f9:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_05fe:  and
    IL_05ff:  brtrue.s   IL_060c

    IL_0601:  ldloca.s   V_2
    IL_0603:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0609:  ldloc.2
    IL_060a:  br.s       IL_0624

    IL_060c:  ldloca.s   V_0
    IL_060e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0613:  ldloca.s   V_1
    IL_0615:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_061a:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_Modulus(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                 valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_061f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0624:  nop
    IL_0625:  starg.s    a
    IL_0627:  ldarg.0
    IL_0628:  stloc.0
    IL_0629:  ldarg.1
    IL_062a:  stloc.1
    IL_062b:  ldloca.s   V_0
    IL_062d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0632:  ldloca.s   V_1
    IL_0634:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0639:  and
    IL_063a:  brtrue.s   IL_0647

    IL_063c:  ldloca.s   V_2
    IL_063e:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0644:  ldloc.2
    IL_0645:  br.s       IL_065f

    IL_0647:  ldloca.s   V_0
    IL_0649:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_064e:  ldloca.s   V_1
    IL_0650:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0655:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                    valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_065a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_065f:  nop
    IL_0660:  starg.s    a
    IL_0662:  ldarg.0
    IL_0663:  stloc.0
    IL_0664:  ldarg.1
    IL_0665:  stloc.1
    IL_0666:  ldloca.s   V_0
    IL_0668:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_066d:  ldloca.s   V_1
    IL_066f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_0674:  and
    IL_0675:  brtrue.s   IL_0682

    IL_0677:  ldloca.s   V_2
    IL_0679:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_067f:  ldloc.2
    IL_0680:  br.s       IL_069a

    IL_0682:  ldloca.s   V_0
    IL_0684:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0689:  ldloca.s   V_1
    IL_068b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_0690:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                   valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_0695:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_069a:  nop
    IL_069b:  starg.s    a
    IL_069d:  ldarg.0
    IL_069e:  stloc.0
    IL_069f:  ldarg.1
    IL_06a0:  stloc.1
    IL_06a1:  ldloca.s   V_0
    IL_06a3:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_06a8:  ldloca.s   V_1
    IL_06aa:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_06af:  and
    IL_06b0:  brtrue.s   IL_06bd

    IL_06b2:  ldloca.s   V_2
    IL_06b4:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_06ba:  ldloc.2
    IL_06bb:  br.s       IL_06d5

    IL_06bd:  ldloca.s   V_0
    IL_06bf:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_06c4:  ldloca.s   V_1
    IL_06c6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_06cb:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_ExclusiveOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS)
    IL_06d0:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_06d5:  nop
    IL_06d6:  starg.s    a
    IL_06d8:  ldarg.0
    IL_06d9:  stloc.0
    IL_06da:  ldarg.2
    IL_06db:  stloc.3
    IL_06dc:  ldloca.s   V_0
    IL_06de:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_06e3:  ldloca.s   V_3
    IL_06e5:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_06ea:  and
    IL_06eb:  brtrue.s   IL_06f8

    IL_06ed:  ldloca.s   V_1
    IL_06ef:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_06f5:  ldloc.1
    IL_06f6:  br.s       IL_0710

    IL_06f8:  ldloca.s   V_0
    IL_06fa:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_06ff:  ldloca.s   V_3
    IL_0701:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0706:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_LeftShift(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                   int32)
    IL_070b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_0710:  nop
    IL_0711:  starg.s    a
    IL_0713:  ldarg.0
    IL_0714:  stloc.0
    IL_0715:  ldarg.2
    IL_0716:  stloc.3
    IL_0717:  ldloca.s   V_0
    IL_0719:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::get_HasValue()
    IL_071e:  ldloca.s   V_3
    IL_0720:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0725:  and
    IL_0726:  brtrue.s   IL_0733

    IL_0728:  ldloca.s   V_1
    IL_072a:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>
    IL_0730:  ldloc.1
    IL_0731:  br.s       IL_074b

    IL_0733:  ldloca.s   V_0
    IL_0735:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::GetValueOrDefault()
    IL_073a:  ldloca.s   V_3
    IL_073c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0741:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS::op_RightShift(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS,
                                                                                                                                                    int32)
    IL_0746:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.TS>::.ctor(!0)
    IL_074b:  nop
    IL_074c:  starg.s    a
    IL_074e:  ret
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
    // Code size       46 (0x2e)
    .maxstack  2
    .locals init (bool V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldarg.1
    IL_0004:  stloc.2
    IL_0005:  ldloca.s   V_1
    IL_0007:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000c:  ldloca.s   V_2
    IL_000e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0013:  bne.un.s   IL_0027

    IL_0015:  ldloca.s   V_1
    IL_0017:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001c:  ldloca.s   V_2
    IL_001e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0023:  ceq
    IL_0025:  br.s       IL_0028

    IL_0027:  ldc.i4.0
    IL_0028:  nop
    IL_0029:  stloc.0
    IL_002a:  br.s       IL_002c

    IL_002c:  ldloc.0
    IL_002d:  ret
  } // end of method LiftedOperators::RetEq

  .method public hidebysig static bool  RetEqConv(valuetype [mscorlib]System.Nullable`1<int64> a,
                                                  valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       47 (0x2f)
    .maxstack  2
    .locals init (bool V_0,
             valuetype [mscorlib]System.Nullable`1<int64> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldarg.1
    IL_0004:  stloc.2
    IL_0005:  ldloca.s   V_1
    IL_0007:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_000c:  ldloca.s   V_2
    IL_000e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0013:  conv.i8
    IL_0014:  bne.un.s   IL_0028

    IL_0016:  ldloca.s   V_1
    IL_0018:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_001d:  ldloca.s   V_2
    IL_001f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0024:  ceq
    IL_0026:  br.s       IL_0029

    IL_0028:  ldc.i4.0
    IL_0029:  nop
    IL_002a:  stloc.0
    IL_002b:  br.s       IL_002d

    IL_002d:  ldloc.0
    IL_002e:  ret
  } // end of method LiftedOperators::RetEqConv

  .method public hidebysig static bool  RetEqConst(valuetype [mscorlib]System.Nullable`1<int64> a) cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  2
    .locals init (bool V_0,
             valuetype [mscorlib]System.Nullable`1<int64> V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldloca.s   V_1
    IL_0005:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_000a:  ldc.i4.s   10
    IL_000c:  conv.i8
    IL_000d:  bne.un.s   IL_0018

    IL_000f:  ldloca.s   V_1
    IL_0011:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0016:  br.s       IL_0019

    IL_0018:  ldc.i4.0
    IL_0019:  nop
    IL_001a:  stloc.0
    IL_001b:  br.s       IL_001d

    IL_001d:  ldloc.0
    IL_001e:  ret
  } // end of method LiftedOperators::RetEqConst

  .method public hidebysig static bool  RetIneqConst(valuetype [mscorlib]System.Nullable`1<int64> a) cil managed
  {
    // Code size       34 (0x22)
    .maxstack  2
    .locals init (bool V_0,
             valuetype [mscorlib]System.Nullable`1<int64> V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldloca.s   V_1
    IL_0005:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_000a:  ldc.i4.s   10
    IL_000c:  conv.i8
    IL_000d:  bne.un.s   IL_001b

    IL_000f:  ldloca.s   V_1
    IL_0011:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0016:  ldc.i4.0
    IL_0017:  ceq
    IL_0019:  br.s       IL_001c

    IL_001b:  ldc.i4.1
    IL_001c:  nop
    IL_001d:  stloc.0
    IL_001e:  br.s       IL_0020

    IL_0020:  ldloc.0
    IL_0021:  ret
  } // end of method LiftedOperators::RetIneqConst

  .method public hidebysig static bool  RetLt(valuetype [mscorlib]System.Nullable`1<int32> a,
                                              valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  2
    .locals init (bool V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldarg.1
    IL_0004:  stloc.2
    IL_0005:  ldloca.s   V_1
    IL_0007:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000c:  ldloca.s   V_2
    IL_000e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0013:  bge.s      IL_0026

    IL_0015:  ldloca.s   V_1
    IL_0017:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001c:  ldloca.s   V_2
    IL_001e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0023:  and
    IL_0024:  br.s       IL_0027

    IL_0026:  ldc.i4.0
    IL_0027:  nop
    IL_0028:  stloc.0
    IL_0029:  br.s       IL_002b

    IL_002b:  ldloc.0
    IL_002c:  ret
  } // end of method LiftedOperators::RetLt

  .method public hidebysig static bool  RetLtConst(valuetype [mscorlib]System.Nullable`1<int32> a) cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  2
    .locals init (bool V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldloca.s   V_1
    IL_0005:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000a:  ldc.i4.s   10
    IL_000c:  bge.s      IL_0017

    IL_000e:  ldloca.s   V_1
    IL_0010:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0015:  br.s       IL_0018

    IL_0017:  ldc.i4.0
    IL_0018:  nop
    IL_0019:  stloc.0
    IL_001a:  br.s       IL_001c

    IL_001c:  ldloc.0
    IL_001d:  ret
  } // end of method LiftedOperators::RetLtConst

  .method public hidebysig static bool  RetLtConv(valuetype [mscorlib]System.Nullable`1<int64> a,
                                                  valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  2
    .locals init (bool V_0,
             valuetype [mscorlib]System.Nullable`1<int64> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldarg.1
    IL_0004:  stloc.2
    IL_0005:  ldloca.s   V_1
    IL_0007:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_000c:  ldloca.s   V_2
    IL_000e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0013:  conv.i8
    IL_0014:  bge.s      IL_0027

    IL_0016:  ldloca.s   V_1
    IL_0018:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_001d:  ldloca.s   V_2
    IL_001f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0024:  and
    IL_0025:  br.s       IL_0028

    IL_0027:  ldc.i4.0
    IL_0028:  nop
    IL_0029:  stloc.0
    IL_002a:  br.s       IL_002c

    IL_002c:  ldloc.0
    IL_002d:  ret
  } // end of method LiftedOperators::RetLtConv

  .method public hidebysig static bool  RetNotLt(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                 valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       48 (0x30)
    .maxstack  2
    .locals init (bool V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldarg.1
    IL_0004:  stloc.2
    IL_0005:  ldloca.s   V_1
    IL_0007:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_000c:  ldloca.s   V_2
    IL_000e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0013:  bge.s      IL_0026

    IL_0015:  ldloca.s   V_1
    IL_0017:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001c:  ldloca.s   V_2
    IL_001e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0023:  and
    IL_0024:  br.s       IL_0027

    IL_0026:  ldc.i4.0
    IL_0027:  nop
    IL_0028:  ldc.i4.0
    IL_0029:  ceq
    IL_002b:  stloc.0
    IL_002c:  br.s       IL_002e

    IL_002e:  ldloc.0
    IL_002f:  ret
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
    // Code size       41 (0x29)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<uint8> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.1
    IL_0003:  ldloca.s   V_1
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint8>::get_HasValue()
    IL_000a:  brtrue.s   IL_0017

    IL_000c:  ldloca.s   V_2
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0014:  ldloc.2
    IL_0015:  br.s       IL_0023

    IL_0017:  ldloca.s   V_1
    IL_0019:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint8>::GetValueOrDefault()
    IL_001e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0023:  nop
    IL_0024:  stloc.0
    IL_0025:  br.s       IL_0027

    IL_0027:  ldloc.0
    IL_0028:  ret
  } // end of method LiftedImplicitConversions::ExtendI4

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int32> 
          ExtendToI4(valuetype [mscorlib]System.Nullable`1<int8> b) cil managed
  {
    // Code size       41 (0x29)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int8> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.1
    IL_0003:  ldloca.s   V_1
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int8>::get_HasValue()
    IL_000a:  brtrue.s   IL_0017

    IL_000c:  ldloca.s   V_2
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0014:  ldloc.2
    IL_0015:  br.s       IL_0023

    IL_0017:  ldloca.s   V_1
    IL_0019:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int8>::GetValueOrDefault()
    IL_001e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0023:  nop
    IL_0024:  stloc.0
    IL_0025:  br.s       IL_0027

    IL_0027:  ldloc.0
    IL_0028:  ret
  } // end of method LiftedImplicitConversions::ExtendToI4

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int64> 
          ExtendI8(valuetype [mscorlib]System.Nullable`1<uint8> b) cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int64> V_0,
             valuetype [mscorlib]System.Nullable`1<uint8> V_1,
             valuetype [mscorlib]System.Nullable`1<int64> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.1
    IL_0003:  ldloca.s   V_1
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint8>::get_HasValue()
    IL_000a:  brtrue.s   IL_0017

    IL_000c:  ldloca.s   V_2
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0014:  ldloc.2
    IL_0015:  br.s       IL_0024

    IL_0017:  ldloca.s   V_1
    IL_0019:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint8>::GetValueOrDefault()
    IL_001e:  conv.u8
    IL_001f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0024:  nop
    IL_0025:  stloc.0
    IL_0026:  br.s       IL_0028

    IL_0028:  ldloc.0
    IL_0029:  ret
  } // end of method LiftedImplicitConversions::ExtendI8

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int64> 
          ExtendToI8(valuetype [mscorlib]System.Nullable`1<int8> b) cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int64> V_0,
             valuetype [mscorlib]System.Nullable`1<int8> V_1,
             valuetype [mscorlib]System.Nullable`1<int64> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.1
    IL_0003:  ldloca.s   V_1
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int8>::get_HasValue()
    IL_000a:  brtrue.s   IL_0017

    IL_000c:  ldloca.s   V_2
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0014:  ldloc.2
    IL_0015:  br.s       IL_0024

    IL_0017:  ldloca.s   V_1
    IL_0019:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int8>::GetValueOrDefault()
    IL_001e:  conv.i8
    IL_001f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0024:  nop
    IL_0025:  stloc.0
    IL_0026:  br.s       IL_0028

    IL_0028:  ldloc.0
    IL_0029:  ret
  } // end of method LiftedImplicitConversions::ExtendToI8

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int64> 
          ExtendI8(valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int64> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int64> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.1
    IL_0003:  ldloca.s   V_1
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_0017

    IL_000c:  ldloca.s   V_2
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0014:  ldloc.2
    IL_0015:  br.s       IL_0024

    IL_0017:  ldloca.s   V_1
    IL_0019:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001e:  conv.i8
    IL_001f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0024:  nop
    IL_0025:  stloc.0
    IL_0026:  br.s       IL_0028

    IL_0028:  ldloc.0
    IL_0029:  ret
  } // end of method LiftedImplicitConversions::ExtendI8

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int64> 
          ExtendToI8(valuetype [mscorlib]System.Nullable`1<uint32> b) cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int64> V_0,
             valuetype [mscorlib]System.Nullable`1<uint32> V_1,
             valuetype [mscorlib]System.Nullable`1<int64> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.1
    IL_0003:  ldloca.s   V_1
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint32>::get_HasValue()
    IL_000a:  brtrue.s   IL_0017

    IL_000c:  ldloca.s   V_2
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0014:  ldloc.2
    IL_0015:  br.s       IL_0024

    IL_0017:  ldloca.s   V_1
    IL_0019:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint32>::GetValueOrDefault()
    IL_001e:  conv.u8
    IL_001f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0024:  nop
    IL_0025:  stloc.0
    IL_0026:  br.s       IL_0028

    IL_0028:  ldloc.0
    IL_0029:  ret
  } // end of method LiftedImplicitConversions::ExtendToI8

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int64> 
          AfterArithmetic(valuetype [mscorlib]System.Nullable`1<uint32> b) cil managed
  {
    // Code size       79 (0x4f)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int64> V_0,
             valuetype [mscorlib]System.Nullable`1<uint32> V_1,
             valuetype [mscorlib]System.Nullable`1<uint32> V_2,
             valuetype [mscorlib]System.Nullable`1<int64> V_3)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.1
    IL_0003:  ldloca.s   V_1
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint32>::get_HasValue()
    IL_000a:  brtrue.s   IL_0017

    IL_000c:  ldloca.s   V_2
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_0014:  ldloc.2
    IL_0015:  br.s       IL_0026

    IL_0017:  ldc.i4.s   100
    IL_0019:  ldloca.s   V_1
    IL_001b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint32>::GetValueOrDefault()
    IL_0020:  add
    IL_0021:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_0026:  nop
    IL_0027:  stloc.1
    IL_0028:  ldloca.s   V_1
    IL_002a:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint32>::get_HasValue()
    IL_002f:  brtrue.s   IL_003c

    IL_0031:  ldloca.s   V_3
    IL_0033:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0039:  ldloc.3
    IL_003a:  br.s       IL_0049

    IL_003c:  ldloca.s   V_1
    IL_003e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<uint32>::GetValueOrDefault()
    IL_0043:  conv.u8
    IL_0044:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0049:  nop
    IL_004a:  stloc.0
    IL_004b:  br.s       IL_004d

    IL_004d:  ldloc.0
    IL_004e:  ret
  } // end of method LiftedImplicitConversions::AfterArithmetic

  .method public hidebysig static valuetype [mscorlib]System.Nullable`1<int64> 
          InArithmetic3(valuetype [mscorlib]System.Nullable`1<int32> a,
                        valuetype [mscorlib]System.Nullable`1<int64> b,
                        valuetype [mscorlib]System.Nullable`1<int32> c,
                        int64 d) cil managed
  {
    // Code size       153 (0x99)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int64> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int64> V_2,
             valuetype [mscorlib]System.Nullable`1<int64> V_3,
             int64 V_4)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldarg.1
    IL_0004:  stloc.2
    IL_0005:  ldloca.s   V_1
    IL_0007:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000c:  ldloca.s   V_2
    IL_000e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0013:  and
    IL_0014:  brtrue.s   IL_0021

    IL_0016:  ldloca.s   V_3
    IL_0018:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_001e:  ldloc.3
    IL_001f:  br.s       IL_0036

    IL_0021:  ldloca.s   V_1
    IL_0023:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0028:  conv.i8
    IL_0029:  ldloca.s   V_2
    IL_002b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0030:  add
    IL_0031:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0036:  nop
    IL_0037:  stloc.2
    IL_0038:  ldarg.2
    IL_0039:  stloc.1
    IL_003a:  ldloca.s   V_2
    IL_003c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0041:  ldloca.s   V_1
    IL_0043:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0048:  and
    IL_0049:  brtrue.s   IL_0056

    IL_004b:  ldloca.s   V_3
    IL_004d:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0053:  ldloc.3
    IL_0054:  br.s       IL_006b

    IL_0056:  ldloca.s   V_2
    IL_0058:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_005d:  ldloca.s   V_1
    IL_005f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0064:  conv.i8
    IL_0065:  add
    IL_0066:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_006b:  nop
    IL_006c:  stloc.2
    IL_006d:  ldarg.3
    IL_006e:  stloc.s    V_4
    IL_0070:  ldloca.s   V_2
    IL_0072:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0077:  brtrue.s   IL_0084

    IL_0079:  ldloca.s   V_3
    IL_007b:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0081:  ldloc.3
    IL_0082:  br.s       IL_0093

    IL_0084:  ldloca.s   V_2
    IL_0086:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_008b:  ldloc.s    V_4
    IL_008d:  add
    IL_008e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0093:  nop
    IL_0094:  stloc.0
    IL_0095:  br.s       IL_0097

    IL_0097:  ldloc.0
    IL_0098:  ret
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
    // Code size       214 (0xd6)
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
    IL_0024:  nop
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
    IL_0048:  conv.i2
    IL_0049:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int16>::.ctor(!0)
    IL_004e:  nop
    IL_004f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<int16>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_0054:  nop
    IL_0055:  ldarg.0
    IL_0056:  stloc.0
    IL_0057:  ldloca.s   V_0
    IL_0059:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_005e:  brtrue.s   IL_006b

    IL_0060:  ldloca.s   V_3
    IL_0062:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_0068:  ldloc.3
    IL_0069:  br.s       IL_0077

    IL_006b:  ldloca.s   V_0
    IL_006d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0072:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_0077:  nop
    IL_0078:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_007d:  nop
    IL_007e:  ldarg.1
    IL_007f:  stloc.s    V_4
    IL_0081:  ldloca.s   V_4
    IL_0083:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0088:  brtrue.s   IL_0095

    IL_008a:  ldloca.s   V_3
    IL_008c:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_0092:  ldloc.3
    IL_0093:  br.s       IL_00a2

    IL_0095:  ldloca.s   V_4
    IL_0097:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_009c:  conv.u4
    IL_009d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_00a2:  nop
    IL_00a3:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_00a8:  nop
    IL_00a9:  ldarg.2
    IL_00aa:  stloc.s    V_5
    IL_00ac:  ldloca.s   V_5
    IL_00ae:  call       instance bool valuetype [mscorlib]System.Nullable`1<float32>::get_HasValue()
    IL_00b3:  brtrue.s   IL_00c0

    IL_00b5:  ldloca.s   V_3
    IL_00b7:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_00bd:  ldloc.3
    IL_00be:  br.s       IL_00ce

    IL_00c0:  ldloca.s   V_5
    IL_00c2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<float32>::GetValueOrDefault()
    IL_00c7:  conv.r4
    IL_00c8:  conv.u4
    IL_00c9:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_00ce:  nop
    IL_00cf:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_00d4:  nop
    IL_00d5:  ret
  } // end of method LiftedExplicitConversions::UncheckedCasts

  .method public hidebysig static void  CheckedCasts(valuetype [mscorlib]System.Nullable`1<int32> i4,
                                                     valuetype [mscorlib]System.Nullable`1<int64> i8,
                                                     valuetype [mscorlib]System.Nullable`1<float32> f) cil managed
  {
    // Code size       173 (0xad)
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
    IL_0025:  nop
    IL_0026:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint8>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_002b:  nop
    IL_002c:  ldarg.0
    IL_002d:  stloc.0
    IL_002e:  ldloca.s   V_0
    IL_0030:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0035:  brtrue.s   IL_0042

    IL_0037:  ldloca.s   V_2
    IL_0039:  initobj    valuetype [mscorlib]System.Nullable`1<int16>
    IL_003f:  ldloc.2
    IL_0040:  br.s       IL_004f

    IL_0042:  ldloca.s   V_0
    IL_0044:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0049:  conv.ovf.i2
    IL_004a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int16>::.ctor(!0)
    IL_004f:  nop
    IL_0050:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<int16>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_0055:  nop
    IL_0056:  ldarg.0
    IL_0057:  stloc.0
    IL_0058:  ldloca.s   V_0
    IL_005a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_005f:  brtrue.s   IL_006c

    IL_0061:  ldloca.s   V_3
    IL_0063:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_0069:  ldloc.3
    IL_006a:  br.s       IL_0079

    IL_006c:  ldloca.s   V_0
    IL_006e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0073:  conv.ovf.u4
    IL_0074:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_0079:  nop
    IL_007a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_007f:  nop
    IL_0080:  ldarg.1
    IL_0081:  stloc.s    V_4
    IL_0083:  ldloca.s   V_4
    IL_0085:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_008a:  brtrue.s   IL_0097

    IL_008c:  ldloca.s   V_3
    IL_008e:  initobj    valuetype [mscorlib]System.Nullable`1<uint32>
    IL_0094:  ldloc.3
    IL_0095:  br.s       IL_00a4

    IL_0097:  ldloca.s   V_4
    IL_0099:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_009e:  conv.ovf.u4
    IL_009f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<uint32>::.ctor(!0)
    IL_00a4:  nop
    IL_00a5:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedExplicitConversions::Print<uint32>(valuetype [mscorlib]System.Nullable`1<!!0>)
    IL_00aa:  nop
    IL_00ab:  nop
    IL_00ac:  ret
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
    // Code size       35 (0x23)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_000f

    IL_000c:  ldarg.1
    IL_000d:  br.s       IL_001b

    IL_000f:  ldloca.s   V_0
    IL_0011:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0016:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_001b:  nop
    IL_001c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullCoalescingTests::Print<valuetype [mscorlib]System.Nullable`1<int32>>(!!0)
    IL_0021:  nop
    IL_0022:  ret
  } // end of method NullCoalescingTests::Nullables

  .method public hidebysig static void  NullableWithNonNullableFallback(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                                        int32 b) cil managed
  {
    // Code size       30 (0x1e)
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
    IL_0016:  nop
    IL_0017:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullCoalescingTests::Print<int32>(!!0)
    IL_001c:  nop
    IL_001d:  ret
  } // end of method NullCoalescingTests::NullableWithNonNullableFallback

  .method public hidebysig static void  NullableWithImplicitConversion(valuetype [mscorlib]System.Nullable`1<int16> a,
                                                                       valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       35 (0x23)
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
    IL_001b:  nop
    IL_001c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullCoalescingTests::Print<valuetype [mscorlib]System.Nullable`1<int32>>(!!0)
    IL_0021:  nop
    IL_0022:  ret
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
    // Code size       72 (0x48)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloca.s   V_0
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_0039

    IL_000c:  ldarg.1
    IL_000d:  stloc.1
    IL_000e:  ldloca.s   V_1
    IL_0010:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0015:  brtrue.s   IL_002f

    IL_0017:  ldarg.2
    IL_0018:  stloc.2
    IL_0019:  ldloca.s   V_2
    IL_001b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0020:  brtrue.s   IL_0025

    IL_0022:  ldarg.3
    IL_0023:  br.s       IL_002c

    IL_0025:  ldloca.s   V_2
    IL_0027:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_002c:  nop
    IL_002d:  br.s       IL_0036

    IL_002f:  ldloca.s   V_1
    IL_0031:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0036:  nop
    IL_0037:  br.s       IL_0040

    IL_0039:  ldloca.s   V_0
    IL_003b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0040:  nop
    IL_0041:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullCoalescingTests::Print<int32>(!!0)
    IL_0046:  nop
    IL_0047:  ret
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
    // Code size       33 (0x21)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldloca.s   V_1
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_000f

    IL_000c:  ldarg.1
    IL_000d:  br.s       IL_001b

    IL_000f:  ldloca.s   V_1
    IL_0011:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0016:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_001b:  nop
    IL_001c:  stloc.0
    IL_001d:  br.s       IL_001f

    IL_001f:  ldloc.0
    IL_0020:  ret
  } // end of method NullCoalescingTests::ReturnNullables

  .method public hidebysig static int32  ReturnNullableWithNonNullableFallback(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                                               int32 b) cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  1
    .locals init (int32 V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldloca.s   V_1
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_000f

    IL_000c:  ldarg.1
    IL_000d:  br.s       IL_0016

    IL_000f:  ldloca.s   V_1
    IL_0011:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0016:  nop
    IL_0017:  stloc.0
    IL_0018:  br.s       IL_001a

    IL_001a:  ldloc.0
    IL_001b:  ret
  } // end of method NullCoalescingTests::ReturnNullableWithNonNullableFallback

  .method public hidebysig static int32  ReturnChain(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                     valuetype [mscorlib]System.Nullable`1<int32> b,
                                                     valuetype [mscorlib]System.Nullable`1<int32> c,
                                                     int32 d) cil managed
  {
    // Code size       70 (0x46)
    .maxstack  1
    .locals init (int32 V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             valuetype [mscorlib]System.Nullable`1<int32> V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldloca.s   V_1
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_0039

    IL_000c:  ldarg.1
    IL_000d:  stloc.2
    IL_000e:  ldloca.s   V_2
    IL_0010:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0015:  brtrue.s   IL_002f

    IL_0017:  ldarg.2
    IL_0018:  stloc.3
    IL_0019:  ldloca.s   V_3
    IL_001b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0020:  brtrue.s   IL_0025

    IL_0022:  ldarg.3
    IL_0023:  br.s       IL_002c

    IL_0025:  ldloca.s   V_3
    IL_0027:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_002c:  nop
    IL_002d:  br.s       IL_0036

    IL_002f:  ldloca.s   V_2
    IL_0031:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0036:  nop
    IL_0037:  br.s       IL_0040

    IL_0039:  ldloca.s   V_1
    IL_003b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0040:  nop
    IL_0041:  stloc.0
    IL_0042:  br.s       IL_0044

    IL_0044:  ldloc.0
    IL_0045:  ret
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
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method NullCoalescingTests::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullCoalescingTests


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\LiftedOperators.res
