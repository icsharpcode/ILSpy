



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
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.LiftedOperators
       extends [mscorlib]System.Object
{
  .method public hidebysig static void  BoolBasic(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                  valuetype [mscorlib]System.Nullable`1<bool> b) cil managed
  {
    // Code size       103 (0x67)
    .maxstack  3
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
    IL_0013:  ceq
    IL_0015:  ldloca.s   V_1
    IL_0017:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001c:  ldloca.s   V_2
    IL_001e:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0023:  ceq
    IL_0025:  and
    IL_0026:  stloc.0
    IL_0027:  ldloc.0
    IL_0028:  brfalse.s  IL_0032

    IL_002a:  nop
    IL_002b:  call       void [mscorlib]System.Console::WriteLine()
    IL_0030:  nop
    IL_0031:  nop
    IL_0032:  ldarg.0
    IL_0033:  stloc.2
    IL_0034:  ldarg.1
    IL_0035:  stloc.1
    IL_0036:  ldloca.s   V_2
    IL_0038:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_003d:  ldloca.s   V_1
    IL_003f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0044:  ceq
    IL_0046:  ldloca.s   V_2
    IL_0048:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_004d:  ldloca.s   V_1
    IL_004f:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0054:  ceq
    IL_0056:  and
    IL_0057:  ldc.i4.0
    IL_0058:  ceq
    IL_005a:  stloc.3
    IL_005b:  ldloc.3
    IL_005c:  brfalse.s  IL_0066

    IL_005e:  nop
    IL_005f:  call       void [mscorlib]System.Console::WriteLine()
    IL_0064:  nop
    IL_0065:  nop
    IL_0066:  ret
  } // end of method LiftedOperators::BoolBasic

  .method public hidebysig static void  BoolComplex(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                    class [mscorlib]System.Func`1<bool> x) cil managed
  {
    // Code size       164 (0xa4)
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
    IL_0012:  ceq
    IL_0014:  ldloca.s   V_1
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001b:  and
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  brfalse.s  IL_0028

    IL_0020:  nop
    IL_0021:  call       void [mscorlib]System.Console::WriteLine()
    IL_0026:  nop
    IL_0027:  nop
    IL_0028:  ldarg.0
    IL_0029:  stloc.1
    IL_002a:  ldarg.1
    IL_002b:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0030:  stloc.2
    IL_0031:  ldloca.s   V_1
    IL_0033:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0038:  ldloc.2
    IL_0039:  ceq
    IL_003b:  ldloca.s   V_1
    IL_003d:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0042:  and
    IL_0043:  ldc.i4.0
    IL_0044:  ceq
    IL_0046:  stloc.3
    IL_0047:  ldloc.3
    IL_0048:  brfalse.s  IL_0052

    IL_004a:  nop
    IL_004b:  call       void [mscorlib]System.Console::WriteLine()
    IL_0050:  nop
    IL_0051:  nop
    IL_0052:  ldarg.1
    IL_0053:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0058:  ldarg.0
    IL_0059:  stloc.1
    IL_005a:  ldloca.s   V_1
    IL_005c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0061:  ceq
    IL_0063:  ldloca.s   V_1
    IL_0065:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_006a:  and
    IL_006b:  stloc.s    V_4
    IL_006d:  ldloc.s    V_4
    IL_006f:  brfalse.s  IL_0079

    IL_0071:  nop
    IL_0072:  call       void [mscorlib]System.Console::WriteLine()
    IL_0077:  nop
    IL_0078:  nop
    IL_0079:  ldarg.1
    IL_007a:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_007f:  ldarg.0
    IL_0080:  stloc.1
    IL_0081:  ldloca.s   V_1
    IL_0083:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0088:  ceq
    IL_008a:  ldloca.s   V_1
    IL_008c:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0091:  and
    IL_0092:  ldc.i4.0
    IL_0093:  ceq
    IL_0095:  stloc.s    V_5
    IL_0097:  ldloc.s    V_5
    IL_0099:  brfalse.s  IL_00a3

    IL_009b:  nop
    IL_009c:  call       void [mscorlib]System.Console::WriteLine()
    IL_00a1:  nop
    IL_00a2:  nop
    IL_00a3:  ret
  } // end of method LiftedOperators::BoolComplex

  .method public hidebysig static void  BoolConst(valuetype [mscorlib]System.Nullable`1<bool> a) cil managed
  {
    // Code size       218 (0xda)
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
    IL_000d:  ceq
    IL_000f:  ldloca.s   V_1
    IL_0011:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0016:  and
    IL_0017:  stloc.0
    IL_0018:  ldloc.0
    IL_0019:  brfalse.s  IL_0023

    IL_001b:  nop
    IL_001c:  call       void [mscorlib]System.Console::WriteLine()
    IL_0021:  nop
    IL_0022:  nop
    IL_0023:  ldarg.0
    IL_0024:  stloc.1
    IL_0025:  ldc.i4.1
    IL_0026:  stloc.2
    IL_0027:  ldloca.s   V_1
    IL_0029:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_002e:  ldloc.2
    IL_002f:  ceq
    IL_0031:  ldloca.s   V_1
    IL_0033:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0038:  and
    IL_0039:  ldc.i4.0
    IL_003a:  ceq
    IL_003c:  stloc.3
    IL_003d:  ldloc.3
    IL_003e:  brfalse.s  IL_0048

    IL_0040:  nop
    IL_0041:  call       void [mscorlib]System.Console::WriteLine()
    IL_0046:  nop
    IL_0047:  nop
    IL_0048:  ldarg.0
    IL_0049:  stloc.1
    IL_004a:  ldc.i4.0
    IL_004b:  stloc.2
    IL_004c:  ldloca.s   V_1
    IL_004e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0053:  ldloc.2
    IL_0054:  ceq
    IL_0056:  ldloca.s   V_1
    IL_0058:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_005d:  and
    IL_005e:  stloc.s    V_4
    IL_0060:  ldloc.s    V_4
    IL_0062:  brfalse.s  IL_006c

    IL_0064:  nop
    IL_0065:  call       void [mscorlib]System.Console::WriteLine()
    IL_006a:  nop
    IL_006b:  nop
    IL_006c:  ldarg.0
    IL_006d:  stloc.1
    IL_006e:  ldc.i4.0
    IL_006f:  stloc.2
    IL_0070:  ldloca.s   V_1
    IL_0072:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0077:  ldloc.2
    IL_0078:  ceq
    IL_007a:  ldloca.s   V_1
    IL_007c:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0081:  and
    IL_0082:  ldc.i4.0
    IL_0083:  ceq
    IL_0085:  stloc.s    V_5
    IL_0087:  ldloc.s    V_5
    IL_0089:  brfalse.s  IL_0093

    IL_008b:  nop
    IL_008c:  call       void [mscorlib]System.Console::WriteLine()
    IL_0091:  nop
    IL_0092:  nop
    IL_0093:  ldarg.0
    IL_0094:  stloc.1
    IL_0095:  ldloca.s   V_1
    IL_0097:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_009c:  brtrue.s   IL_00a1

    IL_009e:  ldc.i4.1
    IL_009f:  br.s       IL_00a8

    IL_00a1:  ldloca.s   V_1
    IL_00a3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00a8:  stloc.s    V_6
    IL_00aa:  ldloc.s    V_6
    IL_00ac:  brfalse.s  IL_00b6

    IL_00ae:  nop
    IL_00af:  call       void [mscorlib]System.Console::WriteLine()
    IL_00b4:  nop
    IL_00b5:  nop
    IL_00b6:  ldarg.0
    IL_00b7:  stloc.1
    IL_00b8:  ldloca.s   V_1
    IL_00ba:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00bf:  brtrue.s   IL_00c4

    IL_00c1:  ldc.i4.0
    IL_00c2:  br.s       IL_00cb

    IL_00c4:  ldloca.s   V_1
    IL_00c6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00cb:  stloc.s    V_7
    IL_00cd:  ldloc.s    V_7
    IL_00cf:  brfalse.s  IL_00d9

    IL_00d1:  nop
    IL_00d2:  call       void [mscorlib]System.Console::WriteLine()
    IL_00d7:  nop
    IL_00d8:  nop
    IL_00d9:  ret
  } // end of method LiftedOperators::BoolConst

  .method public hidebysig static void  BoolValueBasic(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                       valuetype [mscorlib]System.Nullable`1<bool> b) cil managed
  {
    // Code size       448 (0x1c0)
    .maxstack  3
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
    IL_0013:  ceq
    IL_0015:  ldloca.s   V_0
    IL_0017:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001c:  ldloca.s   V_1
    IL_001e:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0023:  ceq
    IL_0025:  and
    IL_0026:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_002b:  nop
    IL_002c:  ldarg.0
    IL_002d:  stloc.1
    IL_002e:  ldarg.1
    IL_002f:  stloc.0
    IL_0030:  ldloca.s   V_1
    IL_0032:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0037:  ldloca.s   V_0
    IL_0039:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_003e:  ceq
    IL_0040:  ldloca.s   V_1
    IL_0042:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0047:  ldloca.s   V_0
    IL_0049:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_004e:  ceq
    IL_0050:  and
    IL_0051:  ldc.i4.0
    IL_0052:  ceq
    IL_0054:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0059:  nop
    IL_005a:  ldarg.0
    IL_005b:  stloc.0
    IL_005c:  ldarg.1
    IL_005d:  stloc.1
    IL_005e:  ldloca.s   V_0
    IL_0060:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0065:  brtrue.s   IL_007c

    IL_0067:  ldloca.s   V_1
    IL_0069:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_006e:  brtrue.s   IL_0079

    IL_0070:  ldloca.s   V_0
    IL_0072:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0077:  brfalse.s  IL_007c

    IL_0079:  ldloc.0
    IL_007a:  br.s       IL_007d

    IL_007c:  ldloc.1
    IL_007d:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0082:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0087:  nop
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
    IL_00b5:  nop
    IL_00b6:  ldarg.0
    IL_00b7:  stloc.0
    IL_00b8:  ldarg.1
    IL_00b9:  stloc.1
    IL_00ba:  ldloca.s   V_0
    IL_00bc:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00c1:  ldloca.s   V_1
    IL_00c3:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00c8:  and
    IL_00c9:  brtrue.s   IL_00d6

    IL_00cb:  ldloca.s   V_2
    IL_00cd:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_00d3:  ldloc.2
    IL_00d4:  br.s       IL_00ea

    IL_00d6:  ldloca.s   V_0
    IL_00d8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00dd:  ldloca.s   V_1
    IL_00df:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00e4:  xor
    IL_00e5:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_00ea:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_00ef:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00f4:  nop
    IL_00f5:  ldarg.0
    IL_00f6:  stloc.1
    IL_00f7:  ldloca.s   V_1
    IL_00f9:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00fe:  brtrue.s   IL_0103

    IL_0100:  ldarg.1
    IL_0101:  br.s       IL_0104

    IL_0103:  ldloc.1
    IL_0104:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0109:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_010e:  nop
    IL_010f:  ldarg.0
    IL_0110:  stloc.1
    IL_0111:  ldloca.s   V_1
    IL_0113:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0118:  brtrue.s   IL_0125

    IL_011a:  ldloca.s   V_0
    IL_011c:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_0122:  ldloc.0
    IL_0123:  br.s       IL_0134

    IL_0125:  ldloca.s   V_1
    IL_0127:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_012c:  ldc.i4.0
    IL_012d:  ceq
    IL_012f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0134:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0139:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_013e:  nop
    IL_013f:  ldarg.0
    IL_0140:  stloc.1
    IL_0141:  ldarg.1
    IL_0142:  stloc.0
    IL_0143:  ldloca.s   V_1
    IL_0145:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_014a:  brtrue.s   IL_0161

    IL_014c:  ldloca.s   V_0
    IL_014e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0153:  brtrue.s   IL_015e

    IL_0155:  ldloca.s   V_1
    IL_0157:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_015c:  brfalse.s  IL_0161

    IL_015e:  ldloc.1
    IL_015f:  br.s       IL_0162

    IL_0161:  ldloc.0
    IL_0162:  starg.s    a
    IL_0164:  ldarg.0
    IL_0165:  stloc.0
    IL_0166:  ldarg.1
    IL_0167:  stloc.1
    IL_0168:  ldloca.s   V_0
    IL_016a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_016f:  brtrue.s   IL_0186

    IL_0171:  ldloca.s   V_1
    IL_0173:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0178:  brtrue.s   IL_0183

    IL_017a:  ldloca.s   V_0
    IL_017c:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0181:  brfalse.s  IL_0186

    IL_0183:  ldloc.1
    IL_0184:  br.s       IL_0187

    IL_0186:  ldloc.0
    IL_0187:  starg.s    a
    IL_0189:  ldarg.0
    IL_018a:  stloc.1
    IL_018b:  ldarg.1
    IL_018c:  stloc.0
    IL_018d:  ldloca.s   V_1
    IL_018f:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0194:  ldloca.s   V_0
    IL_0196:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_019b:  and
    IL_019c:  brtrue.s   IL_01a9

    IL_019e:  ldloca.s   V_2
    IL_01a0:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_01a6:  ldloc.2
    IL_01a7:  br.s       IL_01bd

    IL_01a9:  ldloca.s   V_1
    IL_01ab:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01b0:  ldloca.s   V_0
    IL_01b2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01b7:  xor
    IL_01b8:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_01bd:  starg.s    a
    IL_01bf:  ret
  } // end of method LiftedOperators::BoolValueBasic

  .method public hidebysig static void  BoolValueComplex(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                         class [mscorlib]System.Func`1<bool> x) cil managed
  {
    // Code size       520 (0x208)
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
    IL_0012:  ceq
    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001b:  and
    IL_001c:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0021:  nop
    IL_0022:  ldarg.0
    IL_0023:  stloc.0
    IL_0024:  ldarg.1
    IL_0025:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_002a:  stloc.1
    IL_002b:  ldloca.s   V_0
    IL_002d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0032:  ldloc.1
    IL_0033:  ceq
    IL_0035:  ldloca.s   V_0
    IL_0037:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_003c:  and
    IL_003d:  ldc.i4.0
    IL_003e:  ceq
    IL_0040:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0045:  nop
    IL_0046:  ldarg.1
    IL_0047:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_004c:  ldarg.0
    IL_004d:  stloc.0
    IL_004e:  ldloca.s   V_0
    IL_0050:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0055:  ceq
    IL_0057:  ldloca.s   V_0
    IL_0059:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_005e:  and
    IL_005f:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0064:  nop
    IL_0065:  ldarg.1
    IL_0066:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_006b:  ldarg.0
    IL_006c:  stloc.0
    IL_006d:  ldloca.s   V_0
    IL_006f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0074:  ceq
    IL_0076:  ldloca.s   V_0
    IL_0078:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_007d:  and
    IL_007e:  ldc.i4.0
    IL_007f:  ceq
    IL_0081:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0086:  nop
    IL_0087:  ldarg.0
    IL_0088:  stloc.0
    IL_0089:  ldarg.1
    IL_008a:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_008f:  stloc.1
    IL_0090:  ldloca.s   V_0
    IL_0092:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0097:  brtrue.s   IL_00a4

    IL_0099:  ldloca.s   V_2
    IL_009b:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_00a1:  ldloc.2
    IL_00a2:  br.s       IL_00b2

    IL_00a4:  ldloca.s   V_0
    IL_00a6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00ab:  ldloc.1
    IL_00ac:  xor
    IL_00ad:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_00b2:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_00b7:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00bc:  nop
    IL_00bd:  ldarg.0
    IL_00be:  stloc.0
    IL_00bf:  ldloca.s   V_0
    IL_00c1:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00c6:  brtrue.s   IL_00d0

    IL_00c8:  ldarg.1
    IL_00c9:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_00ce:  br.s       IL_00d7

    IL_00d0:  ldloca.s   V_0
    IL_00d2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00d7:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00dc:  nop
    IL_00dd:  ldarg.0
    IL_00de:  stloc.0
    IL_00df:  ldarg.1
    IL_00e0:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_00e5:  stloc.1
    IL_00e6:  ldloca.s   V_0
    IL_00e8:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00ed:  brtrue.s   IL_00fa

    IL_00ef:  ldloca.s   V_2
    IL_00f1:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_00f7:  ldloc.2
    IL_00f8:  br.s       IL_0108

    IL_00fa:  ldloca.s   V_0
    IL_00fc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0101:  ldloc.1
    IL_0102:  xor
    IL_0103:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0108:  starg.s    a
    IL_010a:  ldarg.1
    IL_010b:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0110:  ldarg.0
    IL_0111:  stloc.0
    IL_0112:  brtrue.s   IL_011c

    IL_0114:  ldc.i4.0
    IL_0115:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_011a:  br.s       IL_011d

    IL_011c:  ldloc.0
    IL_011d:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0122:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0127:  nop
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
    IL_0145:  nop
    IL_0146:  ldarg.1
    IL_0147:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_014c:  stloc.1
    IL_014d:  ldarg.0
    IL_014e:  stloc.0
    IL_014f:  ldloca.s   V_0
    IL_0151:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0156:  brtrue.s   IL_0163

    IL_0158:  ldloca.s   V_2
    IL_015a:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_0160:  ldloc.2
    IL_0161:  br.s       IL_0171

    IL_0163:  ldloc.1
    IL_0164:  ldloca.s   V_0
    IL_0166:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_016b:  xor
    IL_016c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0171:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0176:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_017b:  nop
    IL_017c:  ldc.i4.0
    IL_017d:  newarr     valuetype [mscorlib]System.Nullable`1<bool>
    IL_0182:  ldc.i4.0
    IL_0183:  ldelema    valuetype [mscorlib]System.Nullable`1<bool>
    IL_0188:  dup
    IL_0189:  ldobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_018e:  stloc.0
    IL_018f:  ldarg.1
    IL_0190:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0195:  stloc.1
    IL_0196:  ldloca.s   V_0
    IL_0198:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_019d:  brtrue.s   IL_01aa

    IL_019f:  ldloca.s   V_2
    IL_01a1:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_01a7:  ldloc.2
    IL_01a8:  br.s       IL_01b8

    IL_01aa:  ldloca.s   V_0
    IL_01ac:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01b1:  ldloc.1
    IL_01b2:  xor
    IL_01b3:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_01b8:  stobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_01bd:  ldc.i4.0
    IL_01be:  newarr     valuetype [mscorlib]System.Nullable`1<bool>
    IL_01c3:  ldc.i4.0
    IL_01c4:  ldelema    valuetype [mscorlib]System.Nullable`1<bool>
    IL_01c9:  dup
    IL_01ca:  ldobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_01cf:  stloc.0
    IL_01d0:  ldarg.0
    IL_01d1:  stloc.2
    IL_01d2:  ldloca.s   V_0
    IL_01d4:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_01d9:  ldloca.s   V_2
    IL_01db:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_01e0:  and
    IL_01e1:  brtrue.s   IL_01ee

    IL_01e3:  ldloca.s   V_3
    IL_01e5:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_01eb:  ldloc.3
    IL_01ec:  br.s       IL_0202

    IL_01ee:  ldloca.s   V_0
    IL_01f0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01f5:  ldloca.s   V_2
    IL_01f7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01fc:  xor
    IL_01fd:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0202:  stobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_0207:  ret
  } // end of method LiftedOperators::BoolValueComplex

  .method public hidebysig static void  BoolValueConst(valuetype [mscorlib]System.Nullable`1<bool> a) cil managed
  {
    // Code size       174 (0xae)
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
    IL_000d:  ceq
    IL_000f:  ldloca.s   V_0
    IL_0011:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0016:  and
    IL_0017:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_001c:  nop
    IL_001d:  ldarg.0
    IL_001e:  stloc.0
    IL_001f:  ldc.i4.1
    IL_0020:  stloc.1
    IL_0021:  ldloca.s   V_0
    IL_0023:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0028:  ldloc.1
    IL_0029:  ceq
    IL_002b:  ldloca.s   V_0
    IL_002d:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0032:  and
    IL_0033:  ldc.i4.0
    IL_0034:  ceq
    IL_0036:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_003b:  nop
    IL_003c:  ldarg.0
    IL_003d:  stloc.0
    IL_003e:  ldc.i4.0
    IL_003f:  stloc.1
    IL_0040:  ldloca.s   V_0
    IL_0042:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0047:  ldloc.1
    IL_0048:  ceq
    IL_004a:  ldloca.s   V_0
    IL_004c:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0051:  and
    IL_0052:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0057:  nop
    IL_0058:  ldarg.0
    IL_0059:  stloc.0
    IL_005a:  ldc.i4.0
    IL_005b:  stloc.1
    IL_005c:  ldloca.s   V_0
    IL_005e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0063:  ldloc.1
    IL_0064:  ceq
    IL_0066:  ldloca.s   V_0
    IL_0068:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_006d:  and
    IL_006e:  ldc.i4.0
    IL_006f:  ceq
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
    IL_008c:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0091:  nop
    IL_0092:  ldarg.0
    IL_0093:  stloc.0
    IL_0094:  ldloca.s   V_0
    IL_0096:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_009b:  brtrue.s   IL_00a0

    IL_009d:  ldc.i4.0
    IL_009e:  br.s       IL_00a7

    IL_00a0:  ldloca.s   V_0
    IL_00a2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00a7:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00ac:  nop
    IL_00ad:  ret
  } // end of method LiftedOperators::BoolValueConst

  .method public hidebysig static void  IntBasic(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                 valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       418 (0x1a2)
    .maxstack  3
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
    IL_0013:  ceq
    IL_0015:  ldloca.s   V_1
    IL_0017:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001c:  ldloca.s   V_2
    IL_001e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0023:  ceq
    IL_0025:  and
    IL_0026:  stloc.0
    IL_0027:  ldloc.0
    IL_0028:  brfalse.s  IL_0032

    IL_002a:  nop
    IL_002b:  call       void [mscorlib]System.Console::WriteLine()
    IL_0030:  nop
    IL_0031:  nop
    IL_0032:  ldarg.0
    IL_0033:  stloc.2
    IL_0034:  ldarg.1
    IL_0035:  stloc.1
    IL_0036:  ldloca.s   V_2
    IL_0038:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_003d:  ldloca.s   V_1
    IL_003f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0044:  ceq
    IL_0046:  ldloca.s   V_2
    IL_0048:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_004d:  ldloca.s   V_1
    IL_004f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0054:  ceq
    IL_0056:  and
    IL_0057:  ldc.i4.0
    IL_0058:  ceq
    IL_005a:  stloc.3
    IL_005b:  ldloc.3
    IL_005c:  brfalse.s  IL_0066

    IL_005e:  nop
    IL_005f:  call       void [mscorlib]System.Console::WriteLine()
    IL_0064:  nop
    IL_0065:  nop
    IL_0066:  ldarg.0
    IL_0067:  stloc.1
    IL_0068:  ldarg.1
    IL_0069:  stloc.2
    IL_006a:  ldloca.s   V_1
    IL_006c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0071:  ldloca.s   V_2
    IL_0073:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0078:  cgt
    IL_007a:  ldloca.s   V_1
    IL_007c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0081:  ldloca.s   V_2
    IL_0083:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0088:  and
    IL_0089:  and
    IL_008a:  stloc.s    V_4
    IL_008c:  ldloc.s    V_4
    IL_008e:  brfalse.s  IL_0098

    IL_0090:  nop
    IL_0091:  call       void [mscorlib]System.Console::WriteLine()
    IL_0096:  nop
    IL_0097:  nop
    IL_0098:  ldarg.0
    IL_0099:  stloc.2
    IL_009a:  ldarg.1
    IL_009b:  stloc.1
    IL_009c:  ldloca.s   V_2
    IL_009e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00a3:  ldloca.s   V_1
    IL_00a5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00aa:  clt
    IL_00ac:  ldloca.s   V_2
    IL_00ae:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00b3:  ldloca.s   V_1
    IL_00b5:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ba:  and
    IL_00bb:  and
    IL_00bc:  stloc.s    V_5
    IL_00be:  ldloc.s    V_5
    IL_00c0:  brfalse.s  IL_00ca

    IL_00c2:  nop
    IL_00c3:  call       void [mscorlib]System.Console::WriteLine()
    IL_00c8:  nop
    IL_00c9:  nop
    IL_00ca:  ldarg.0
    IL_00cb:  stloc.1
    IL_00cc:  ldarg.1
    IL_00cd:  stloc.2
    IL_00ce:  ldloca.s   V_1
    IL_00d0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00d5:  ldloca.s   V_2
    IL_00d7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00dc:  clt
    IL_00de:  ldc.i4.0
    IL_00df:  ceq
    IL_00e1:  ldloca.s   V_1
    IL_00e3:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00e8:  ldloca.s   V_2
    IL_00ea:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ef:  and
    IL_00f0:  and
    IL_00f1:  stloc.s    V_6
    IL_00f3:  ldloc.s    V_6
    IL_00f5:  brfalse.s  IL_00ff

    IL_00f7:  nop
    IL_00f8:  call       void [mscorlib]System.Console::WriteLine()
    IL_00fd:  nop
    IL_00fe:  nop
    IL_00ff:  ldarg.0
    IL_0100:  stloc.2
    IL_0101:  ldarg.1
    IL_0102:  stloc.1
    IL_0103:  ldloca.s   V_2
    IL_0105:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_010a:  ldloca.s   V_1
    IL_010c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0111:  cgt
    IL_0113:  ldc.i4.0
    IL_0114:  ceq
    IL_0116:  ldloca.s   V_2
    IL_0118:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_011d:  ldloca.s   V_1
    IL_011f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0124:  and
    IL_0125:  and
    IL_0126:  stloc.s    V_7
    IL_0128:  ldloc.s    V_7
    IL_012a:  brfalse.s  IL_0134

    IL_012c:  nop
    IL_012d:  call       void [mscorlib]System.Console::WriteLine()
    IL_0132:  nop
    IL_0133:  nop
    IL_0134:  ldarg.0
    IL_0135:  stloc.1
    IL_0136:  ldarg.1
    IL_0137:  stloc.2
    IL_0138:  ldloca.s   V_1
    IL_013a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_013f:  ldloca.s   V_2
    IL_0141:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0146:  cgt
    IL_0148:  ldloca.s   V_1
    IL_014a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_014f:  ldloca.s   V_2
    IL_0151:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0156:  and
    IL_0157:  and
    IL_0158:  ldc.i4.0
    IL_0159:  ceq
    IL_015b:  stloc.s    V_8
    IL_015d:  ldloc.s    V_8
    IL_015f:  brfalse.s  IL_0169

    IL_0161:  nop
    IL_0162:  call       void [mscorlib]System.Console::WriteLine()
    IL_0167:  nop
    IL_0168:  nop
    IL_0169:  ldarg.0
    IL_016a:  stloc.2
    IL_016b:  ldarg.1
    IL_016c:  stloc.1
    IL_016d:  ldloca.s   V_2
    IL_016f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0174:  ldloca.s   V_1
    IL_0176:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_017b:  cgt
    IL_017d:  ldc.i4.0
    IL_017e:  ceq
    IL_0180:  ldloca.s   V_2
    IL_0182:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0187:  ldloca.s   V_1
    IL_0189:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_018e:  and
    IL_018f:  and
    IL_0190:  ldc.i4.0
    IL_0191:  ceq
    IL_0193:  stloc.s    V_9
    IL_0195:  ldloc.s    V_9
    IL_0197:  brfalse.s  IL_01a1

    IL_0199:  nop
    IL_019a:  call       void [mscorlib]System.Console::WriteLine()
    IL_019f:  nop
    IL_01a0:  nop
    IL_01a1:  ret
  } // end of method LiftedOperators::IntBasic

  .method public hidebysig static void  IntComplex(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                   class [mscorlib]System.Func`1<int32> x) cil managed
  {
    // Code size       335 (0x14f)
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
    IL_0012:  ceq
    IL_0014:  ldloca.s   V_1
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001b:  and
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  brfalse.s  IL_0028

    IL_0020:  nop
    IL_0021:  call       void [mscorlib]System.Console::WriteLine()
    IL_0026:  nop
    IL_0027:  nop
    IL_0028:  ldarg.0
    IL_0029:  stloc.1
    IL_002a:  ldarg.1
    IL_002b:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0030:  stloc.2
    IL_0031:  ldloca.s   V_1
    IL_0033:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0038:  ldloc.2
    IL_0039:  ceq
    IL_003b:  ldloca.s   V_1
    IL_003d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0042:  and
    IL_0043:  ldc.i4.0
    IL_0044:  ceq
    IL_0046:  stloc.3
    IL_0047:  ldloc.3
    IL_0048:  brfalse.s  IL_0052

    IL_004a:  nop
    IL_004b:  call       void [mscorlib]System.Console::WriteLine()
    IL_0050:  nop
    IL_0051:  nop
    IL_0052:  ldarg.0
    IL_0053:  stloc.1
    IL_0054:  ldarg.1
    IL_0055:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_005a:  stloc.2
    IL_005b:  ldloca.s   V_1
    IL_005d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0062:  ldloc.2
    IL_0063:  cgt
    IL_0065:  ldloca.s   V_1
    IL_0067:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_006c:  and
    IL_006d:  stloc.s    V_4
    IL_006f:  ldloc.s    V_4
    IL_0071:  brfalse.s  IL_007b

    IL_0073:  nop
    IL_0074:  call       void [mscorlib]System.Console::WriteLine()
    IL_0079:  nop
    IL_007a:  nop
    IL_007b:  ldarg.1
    IL_007c:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0081:  ldarg.0
    IL_0082:  stloc.1
    IL_0083:  ldloca.s   V_1
    IL_0085:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_008a:  ceq
    IL_008c:  ldloca.s   V_1
    IL_008e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0093:  and
    IL_0094:  stloc.s    V_5
    IL_0096:  ldloc.s    V_5
    IL_0098:  brfalse.s  IL_00a2

    IL_009a:  nop
    IL_009b:  call       void [mscorlib]System.Console::WriteLine()
    IL_00a0:  nop
    IL_00a1:  nop
    IL_00a2:  ldarg.1
    IL_00a3:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00a8:  ldarg.0
    IL_00a9:  stloc.1
    IL_00aa:  ldloca.s   V_1
    IL_00ac:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00b1:  ceq
    IL_00b3:  ldloca.s   V_1
    IL_00b5:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ba:  and
    IL_00bb:  ldc.i4.0
    IL_00bc:  ceq
    IL_00be:  stloc.s    V_6
    IL_00c0:  ldloc.s    V_6
    IL_00c2:  brfalse.s  IL_00cc

    IL_00c4:  nop
    IL_00c5:  call       void [mscorlib]System.Console::WriteLine()
    IL_00ca:  nop
    IL_00cb:  nop
    IL_00cc:  ldarg.1
    IL_00cd:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00d2:  ldarg.0
    IL_00d3:  stloc.1
    IL_00d4:  ldloca.s   V_1
    IL_00d6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00db:  cgt
    IL_00dd:  ldloca.s   V_1
    IL_00df:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00e4:  and
    IL_00e5:  stloc.s    V_7
    IL_00e7:  ldloc.s    V_7
    IL_00e9:  brfalse.s  IL_00f3

    IL_00eb:  nop
    IL_00ec:  call       void [mscorlib]System.Console::WriteLine()
    IL_00f1:  nop
    IL_00f2:  nop
    IL_00f3:  ldarg.0
    IL_00f4:  stloc.1
    IL_00f5:  ldarg.1
    IL_00f6:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00fb:  stloc.2
    IL_00fc:  ldloca.s   V_1
    IL_00fe:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0103:  ldloc.2
    IL_0104:  cgt
    IL_0106:  ldloca.s   V_1
    IL_0108:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_010d:  and
    IL_010e:  ldc.i4.0
    IL_010f:  ceq
    IL_0111:  stloc.s    V_8
    IL_0113:  ldloc.s    V_8
    IL_0115:  brfalse.s  IL_011f

    IL_0117:  nop
    IL_0118:  call       void [mscorlib]System.Console::WriteLine()
    IL_011d:  nop
    IL_011e:  nop
    IL_011f:  ldarg.0
    IL_0120:  stloc.1
    IL_0121:  ldarg.1
    IL_0122:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0127:  stloc.2
    IL_0128:  ldloca.s   V_1
    IL_012a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_012f:  ldloc.2
    IL_0130:  cgt
    IL_0132:  ldc.i4.0
    IL_0133:  ceq
    IL_0135:  ldloca.s   V_1
    IL_0137:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_013c:  and
    IL_013d:  ldc.i4.0
    IL_013e:  ceq
    IL_0140:  stloc.s    V_9
    IL_0142:  ldloc.s    V_9
    IL_0144:  brfalse.s  IL_014e

    IL_0146:  nop
    IL_0147:  call       void [mscorlib]System.Console::WriteLine()
    IL_014c:  nop
    IL_014d:  nop
    IL_014e:  ret
  } // end of method LiftedOperators::IntComplex

  .method public hidebysig static void  IntConst(valuetype [mscorlib]System.Nullable`1<int32> a) cil managed
  {
    // Code size       214 (0xd6)
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
    IL_000d:  ceq
    IL_000f:  ldloca.s   V_1
    IL_0011:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0016:  and
    IL_0017:  stloc.0
    IL_0018:  ldloc.0
    IL_0019:  brfalse.s  IL_0023

    IL_001b:  nop
    IL_001c:  call       void [mscorlib]System.Console::WriteLine()
    IL_0021:  nop
    IL_0022:  nop
    IL_0023:  ldarg.0
    IL_0024:  stloc.1
    IL_0025:  ldc.i4.2
    IL_0026:  stloc.2
    IL_0027:  ldloca.s   V_1
    IL_0029:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_002e:  ldloc.2
    IL_002f:  ceq
    IL_0031:  ldloca.s   V_1
    IL_0033:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0038:  and
    IL_0039:  ldc.i4.0
    IL_003a:  ceq
    IL_003c:  stloc.3
    IL_003d:  ldloc.3
    IL_003e:  brfalse.s  IL_0048

    IL_0040:  nop
    IL_0041:  call       void [mscorlib]System.Console::WriteLine()
    IL_0046:  nop
    IL_0047:  nop
    IL_0048:  ldarg.0
    IL_0049:  stloc.1
    IL_004a:  ldc.i4.2
    IL_004b:  stloc.2
    IL_004c:  ldloca.s   V_1
    IL_004e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0053:  ldloc.2
    IL_0054:  cgt
    IL_0056:  ldloca.s   V_1
    IL_0058:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_005d:  and
    IL_005e:  stloc.s    V_4
    IL_0060:  ldloc.s    V_4
    IL_0062:  brfalse.s  IL_006c

    IL_0064:  nop
    IL_0065:  call       void [mscorlib]System.Console::WriteLine()
    IL_006a:  nop
    IL_006b:  nop
    IL_006c:  ldc.i4.2
    IL_006d:  ldarg.0
    IL_006e:  stloc.1
    IL_006f:  ldloca.s   V_1
    IL_0071:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0076:  ceq
    IL_0078:  ldloca.s   V_1
    IL_007a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_007f:  and
    IL_0080:  stloc.s    V_5
    IL_0082:  ldloc.s    V_5
    IL_0084:  brfalse.s  IL_008e

    IL_0086:  nop
    IL_0087:  call       void [mscorlib]System.Console::WriteLine()
    IL_008c:  nop
    IL_008d:  nop
    IL_008e:  ldc.i4.2
    IL_008f:  ldarg.0
    IL_0090:  stloc.1
    IL_0091:  ldloca.s   V_1
    IL_0093:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0098:  ceq
    IL_009a:  ldloca.s   V_1
    IL_009c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00a1:  and
    IL_00a2:  ldc.i4.0
    IL_00a3:  ceq
    IL_00a5:  stloc.s    V_6
    IL_00a7:  ldloc.s    V_6
    IL_00a9:  brfalse.s  IL_00b3

    IL_00ab:  nop
    IL_00ac:  call       void [mscorlib]System.Console::WriteLine()
    IL_00b1:  nop
    IL_00b2:  nop
    IL_00b3:  ldc.i4.2
    IL_00b4:  ldarg.0
    IL_00b5:  stloc.1
    IL_00b6:  ldloca.s   V_1
    IL_00b8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00bd:  cgt
    IL_00bf:  ldloca.s   V_1
    IL_00c1:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00c6:  and
    IL_00c7:  stloc.s    V_7
    IL_00c9:  ldloc.s    V_7
    IL_00cb:  brfalse.s  IL_00d5

    IL_00cd:  nop
    IL_00ce:  call       void [mscorlib]System.Console::WriteLine()
    IL_00d3:  nop
    IL_00d4:  nop
    IL_00d5:  ret
  } // end of method LiftedOperators::IntConst

  .method public hidebysig static void  IntValueBasic(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                      valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       1626 (0x65a)
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
    IL_0013:  ceq
    IL_0015:  ldloca.s   V_0
    IL_0017:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001c:  ldloca.s   V_1
    IL_001e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0023:  ceq
    IL_0025:  and
    IL_0026:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_002b:  nop
    IL_002c:  ldarg.0
    IL_002d:  stloc.1
    IL_002e:  ldarg.1
    IL_002f:  stloc.0
    IL_0030:  ldloca.s   V_1
    IL_0032:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0037:  ldloca.s   V_0
    IL_0039:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_003e:  ceq
    IL_0040:  ldloca.s   V_1
    IL_0042:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0047:  ldloca.s   V_0
    IL_0049:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_004e:  ceq
    IL_0050:  and
    IL_0051:  ldc.i4.0
    IL_0052:  ceq
    IL_0054:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0059:  nop
    IL_005a:  ldarg.0
    IL_005b:  stloc.0
    IL_005c:  ldarg.1
    IL_005d:  stloc.1
    IL_005e:  ldloca.s   V_0
    IL_0060:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0065:  ldloca.s   V_1
    IL_0067:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_006c:  cgt
    IL_006e:  ldloca.s   V_0
    IL_0070:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0075:  ldloca.s   V_1
    IL_0077:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_007c:  and
    IL_007d:  and
    IL_007e:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0083:  nop
    IL_0084:  ldarg.0
    IL_0085:  stloc.1
    IL_0086:  ldarg.1
    IL_0087:  stloc.0
    IL_0088:  ldloca.s   V_1
    IL_008a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_008f:  ldloca.s   V_0
    IL_0091:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0096:  cgt
    IL_0098:  ldloca.s   V_1
    IL_009a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_009f:  ldloca.s   V_0
    IL_00a1:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00a6:  and
    IL_00a7:  and
    IL_00a8:  ldc.i4.0
    IL_00a9:  ceq
    IL_00ab:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00b0:  nop
    IL_00b1:  ldarg.0
    IL_00b2:  stloc.0
    IL_00b3:  ldarg.1
    IL_00b4:  stloc.1
    IL_00b5:  ldloca.s   V_0
    IL_00b7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00bc:  ldloca.s   V_1
    IL_00be:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00c3:  clt
    IL_00c5:  ldc.i4.0
    IL_00c6:  ceq
    IL_00c8:  ldloca.s   V_0
    IL_00ca:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00cf:  ldloca.s   V_1
    IL_00d1:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00d6:  and
    IL_00d7:  and
    IL_00d8:  ldc.i4.0
    IL_00d9:  ceq
    IL_00db:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00e0:  nop
    IL_00e1:  ldarg.0
    IL_00e2:  stloc.1
    IL_00e3:  ldarg.1
    IL_00e4:  stloc.0
    IL_00e5:  ldloca.s   V_1
    IL_00e7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ec:  ldloca.s   V_0
    IL_00ee:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00f3:  and
    IL_00f4:  brtrue.s   IL_0101

    IL_00f6:  ldloca.s   V_2
    IL_00f8:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00fe:  ldloc.2
    IL_00ff:  br.s       IL_0115

    IL_0101:  ldloca.s   V_1
    IL_0103:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0108:  ldloca.s   V_0
    IL_010a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_010f:  add
    IL_0110:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0115:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_011a:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_011f:  nop
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
    IL_015e:  nop
    IL_015f:  ldarg.0
    IL_0160:  stloc.1
    IL_0161:  ldarg.1
    IL_0162:  stloc.0
    IL_0163:  ldloca.s   V_1
    IL_0165:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_016a:  ldloca.s   V_0
    IL_016c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0171:  and
    IL_0172:  brtrue.s   IL_017f

    IL_0174:  ldloca.s   V_2
    IL_0176:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_017c:  ldloc.2
    IL_017d:  br.s       IL_0193

    IL_017f:  ldloca.s   V_1
    IL_0181:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0186:  ldloca.s   V_0
    IL_0188:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_018d:  mul
    IL_018e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0193:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0198:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_019d:  nop
    IL_019e:  ldarg.0
    IL_019f:  stloc.0
    IL_01a0:  ldarg.1
    IL_01a1:  stloc.1
    IL_01a2:  ldloca.s   V_0
    IL_01a4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01a9:  ldloca.s   V_1
    IL_01ab:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01b0:  and
    IL_01b1:  brtrue.s   IL_01be

    IL_01b3:  ldloca.s   V_2
    IL_01b5:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01bb:  ldloc.2
    IL_01bc:  br.s       IL_01d2

    IL_01be:  ldloca.s   V_0
    IL_01c0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01c5:  ldloca.s   V_1
    IL_01c7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01cc:  div
    IL_01cd:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01d2:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01d7:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01dc:  nop
    IL_01dd:  ldarg.0
    IL_01de:  stloc.1
    IL_01df:  ldarg.1
    IL_01e0:  stloc.0
    IL_01e1:  ldloca.s   V_1
    IL_01e3:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01e8:  ldloca.s   V_0
    IL_01ea:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01ef:  and
    IL_01f0:  brtrue.s   IL_01fd

    IL_01f2:  ldloca.s   V_2
    IL_01f4:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01fa:  ldloc.2
    IL_01fb:  br.s       IL_0211

    IL_01fd:  ldloca.s   V_1
    IL_01ff:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0204:  ldloca.s   V_0
    IL_0206:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_020b:  rem
    IL_020c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0211:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0216:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_021b:  nop
    IL_021c:  ldarg.0
    IL_021d:  stloc.0
    IL_021e:  ldarg.1
    IL_021f:  stloc.1
    IL_0220:  ldloca.s   V_0
    IL_0222:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0227:  ldloca.s   V_1
    IL_0229:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_022e:  and
    IL_022f:  brtrue.s   IL_023c

    IL_0231:  ldloca.s   V_2
    IL_0233:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0239:  ldloc.2
    IL_023a:  br.s       IL_0250

    IL_023c:  ldloca.s   V_0
    IL_023e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0243:  ldloca.s   V_1
    IL_0245:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_024a:  and
    IL_024b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0250:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0255:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_025a:  nop
    IL_025b:  ldarg.0
    IL_025c:  stloc.1
    IL_025d:  ldarg.1
    IL_025e:  stloc.0
    IL_025f:  ldloca.s   V_1
    IL_0261:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0266:  ldloca.s   V_0
    IL_0268:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_026d:  and
    IL_026e:  brtrue.s   IL_027b

    IL_0270:  ldloca.s   V_2
    IL_0272:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0278:  ldloc.2
    IL_0279:  br.s       IL_028f

    IL_027b:  ldloca.s   V_1
    IL_027d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0282:  ldloca.s   V_0
    IL_0284:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0289:  or
    IL_028a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_028f:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0294:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0299:  nop
    IL_029a:  ldarg.0
    IL_029b:  stloc.0
    IL_029c:  ldarg.1
    IL_029d:  stloc.1
    IL_029e:  ldloca.s   V_0
    IL_02a0:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02a5:  ldloca.s   V_1
    IL_02a7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02ac:  and
    IL_02ad:  brtrue.s   IL_02ba

    IL_02af:  ldloca.s   V_2
    IL_02b1:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02b7:  ldloc.2
    IL_02b8:  br.s       IL_02ce

    IL_02ba:  ldloca.s   V_0
    IL_02bc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02c1:  ldloca.s   V_1
    IL_02c3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02c8:  xor
    IL_02c9:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02ce:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_02d3:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02d8:  nop
    IL_02d9:  ldarg.0
    IL_02da:  stloc.1
    IL_02db:  ldarg.1
    IL_02dc:  stloc.0
    IL_02dd:  ldloca.s   V_1
    IL_02df:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02e4:  ldloca.s   V_0
    IL_02e6:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02eb:  and
    IL_02ec:  brtrue.s   IL_02f9

    IL_02ee:  ldloca.s   V_2
    IL_02f0:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02f6:  ldloc.2
    IL_02f7:  br.s       IL_0310

    IL_02f9:  ldloca.s   V_1
    IL_02fb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0300:  ldloca.s   V_0
    IL_0302:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0307:  ldc.i4.s   31
    IL_0309:  and
    IL_030a:  shl
    IL_030b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0310:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0315:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_031a:  nop
    IL_031b:  ldarg.0
    IL_031c:  stloc.0
    IL_031d:  ldarg.1
    IL_031e:  stloc.1
    IL_031f:  ldloca.s   V_0
    IL_0321:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0326:  ldloca.s   V_1
    IL_0328:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_032d:  and
    IL_032e:  brtrue.s   IL_033b

    IL_0330:  ldloca.s   V_2
    IL_0332:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0338:  ldloc.2
    IL_0339:  br.s       IL_0352

    IL_033b:  ldloca.s   V_0
    IL_033d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0342:  ldloca.s   V_1
    IL_0344:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0349:  ldc.i4.s   31
    IL_034b:  and
    IL_034c:  shr
    IL_034d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0352:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0357:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_035c:  nop
    IL_035d:  ldarg.0
    IL_035e:  stloc.1
    IL_035f:  ldloca.s   V_1
    IL_0361:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0366:  brtrue.s   IL_036b

    IL_0368:  ldarg.1
    IL_0369:  br.s       IL_036c

    IL_036b:  ldloc.1
    IL_036c:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0371:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0376:  nop
    IL_0377:  ldarg.0
    IL_0378:  stloc.1
    IL_0379:  ldloca.s   V_1
    IL_037b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0380:  brtrue.s   IL_038d

    IL_0382:  ldloca.s   V_0
    IL_0384:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_038a:  ldloc.0
    IL_038b:  br.s       IL_039a

    IL_038d:  ldloca.s   V_1
    IL_038f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0394:  neg
    IL_0395:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_039a:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_039f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_03a4:  nop
    IL_03a5:  ldarg.0
    IL_03a6:  stloc.1
    IL_03a7:  ldloca.s   V_1
    IL_03a9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03ae:  brtrue.s   IL_03bb

    IL_03b0:  ldloca.s   V_0
    IL_03b2:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03b8:  ldloc.0
    IL_03b9:  br.s       IL_03c8

    IL_03bb:  ldloca.s   V_1
    IL_03bd:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03c2:  not
    IL_03c3:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03c8:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_03cd:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_03d2:  nop
    IL_03d3:  ldarg.0
    IL_03d4:  stloc.1
    IL_03d5:  ldloca.s   V_1
    IL_03d7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03dc:  brtrue.s   IL_03e9

    IL_03de:  ldloca.s   V_0
    IL_03e0:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03e6:  ldloc.0
    IL_03e7:  br.s       IL_03f7

    IL_03e9:  ldloca.s   V_1
    IL_03eb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03f0:  ldc.i4.1
    IL_03f1:  add
    IL_03f2:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03f7:  dup
    IL_03f8:  starg.s    a
    IL_03fa:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_03ff:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0404:  nop
    IL_0405:  ldarg.0
    IL_0406:  stloc.1
    IL_0407:  ldloca.s   V_1
    IL_0409:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_040e:  brtrue.s   IL_041b

    IL_0410:  ldloca.s   V_0
    IL_0412:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0418:  ldloc.0
    IL_0419:  br.s       IL_0429

    IL_041b:  ldloca.s   V_1
    IL_041d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0422:  ldc.i4.1
    IL_0423:  sub
    IL_0424:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0429:  dup
    IL_042a:  starg.s    a
    IL_042c:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0431:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0436:  nop
    IL_0437:  ldarg.0
    IL_0438:  stloc.1
    IL_0439:  ldarg.1
    IL_043a:  stloc.0
    IL_043b:  ldloca.s   V_1
    IL_043d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0442:  ldloca.s   V_0
    IL_0444:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0449:  and
    IL_044a:  brtrue.s   IL_0457

    IL_044c:  ldloca.s   V_2
    IL_044e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0454:  ldloc.2
    IL_0455:  br.s       IL_046b

    IL_0457:  ldloca.s   V_1
    IL_0459:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_045e:  ldloca.s   V_0
    IL_0460:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0465:  add
    IL_0466:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_046b:  starg.s    a
    IL_046d:  ldarg.0
    IL_046e:  stloc.0
    IL_046f:  ldarg.1
    IL_0470:  stloc.1
    IL_0471:  ldloca.s   V_0
    IL_0473:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0478:  ldloca.s   V_1
    IL_047a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_047f:  and
    IL_0480:  brtrue.s   IL_048d

    IL_0482:  ldloca.s   V_2
    IL_0484:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_048a:  ldloc.2
    IL_048b:  br.s       IL_04a1

    IL_048d:  ldloca.s   V_0
    IL_048f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0494:  ldloca.s   V_1
    IL_0496:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_049b:  sub
    IL_049c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04a1:  starg.s    a
    IL_04a3:  ldarg.0
    IL_04a4:  stloc.1
    IL_04a5:  ldarg.1
    IL_04a6:  stloc.0
    IL_04a7:  ldloca.s   V_1
    IL_04a9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04ae:  ldloca.s   V_0
    IL_04b0:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04b5:  and
    IL_04b6:  brtrue.s   IL_04c3

    IL_04b8:  ldloca.s   V_2
    IL_04ba:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04c0:  ldloc.2
    IL_04c1:  br.s       IL_04d7

    IL_04c3:  ldloca.s   V_1
    IL_04c5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04ca:  ldloca.s   V_0
    IL_04cc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04d1:  mul
    IL_04d2:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04d7:  starg.s    a
    IL_04d9:  ldarg.0
    IL_04da:  stloc.0
    IL_04db:  ldarg.1
    IL_04dc:  stloc.1
    IL_04dd:  ldloca.s   V_0
    IL_04df:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04e4:  ldloca.s   V_1
    IL_04e6:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04eb:  and
    IL_04ec:  brtrue.s   IL_04f9

    IL_04ee:  ldloca.s   V_2
    IL_04f0:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04f6:  ldloc.2
    IL_04f7:  br.s       IL_050d

    IL_04f9:  ldloca.s   V_0
    IL_04fb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0500:  ldloca.s   V_1
    IL_0502:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0507:  div
    IL_0508:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_050d:  starg.s    a
    IL_050f:  ldarg.0
    IL_0510:  stloc.1
    IL_0511:  ldarg.1
    IL_0512:  stloc.0
    IL_0513:  ldloca.s   V_1
    IL_0515:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_051a:  ldloca.s   V_0
    IL_051c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0521:  and
    IL_0522:  brtrue.s   IL_052f

    IL_0524:  ldloca.s   V_2
    IL_0526:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_052c:  ldloc.2
    IL_052d:  br.s       IL_0543

    IL_052f:  ldloca.s   V_1
    IL_0531:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0536:  ldloca.s   V_0
    IL_0538:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_053d:  rem
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
    IL_0573:  and
    IL_0574:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0579:  starg.s    a
    IL_057b:  ldarg.0
    IL_057c:  stloc.1
    IL_057d:  ldarg.1
    IL_057e:  stloc.0
    IL_057f:  ldloca.s   V_1
    IL_0581:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0586:  ldloca.s   V_0
    IL_0588:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_058d:  and
    IL_058e:  brtrue.s   IL_059b

    IL_0590:  ldloca.s   V_2
    IL_0592:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0598:  ldloc.2
    IL_0599:  br.s       IL_05af

    IL_059b:  ldloca.s   V_1
    IL_059d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_05a2:  ldloca.s   V_0
    IL_05a4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_05a9:  or
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
    IL_05df:  xor
    IL_05e0:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_05e5:  starg.s    a
    IL_05e7:  ldarg.0
    IL_05e8:  stloc.1
    IL_05e9:  ldarg.1
    IL_05ea:  stloc.0
    IL_05eb:  ldloca.s   V_1
    IL_05ed:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_05f2:  ldloca.s   V_0
    IL_05f4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_05f9:  and
    IL_05fa:  brtrue.s   IL_0607

    IL_05fc:  ldloca.s   V_2
    IL_05fe:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0604:  ldloc.2
    IL_0605:  br.s       IL_061e

    IL_0607:  ldloca.s   V_1
    IL_0609:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_060e:  ldloca.s   V_0
    IL_0610:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0615:  ldc.i4.s   31
    IL_0617:  and
    IL_0618:  shl
    IL_0619:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_061e:  starg.s    a
    IL_0620:  ldarg.0
    IL_0621:  stloc.0
    IL_0622:  ldarg.1
    IL_0623:  stloc.1
    IL_0624:  ldloca.s   V_0
    IL_0626:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_062b:  ldloca.s   V_1
    IL_062d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0632:  and
    IL_0633:  brtrue.s   IL_0640

    IL_0635:  ldloca.s   V_2
    IL_0637:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_063d:  ldloc.2
    IL_063e:  br.s       IL_0657

    IL_0640:  ldloca.s   V_0
    IL_0642:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0647:  ldloca.s   V_1
    IL_0649:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_064e:  ldc.i4.s   31
    IL_0650:  and
    IL_0651:  shr
    IL_0652:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0657:  starg.s    a
    IL_0659:  ret
  } // end of method LiftedOperators::IntValueBasic

  .method public hidebysig static void  IntValueComplex(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                        class [mscorlib]System.Func`1<int32> x) cil managed
  {
    // Code size       1353 (0x549)
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
    IL_0012:  ceq
    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001b:  and
    IL_001c:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0021:  nop
    IL_0022:  ldarg.0
    IL_0023:  stloc.0
    IL_0024:  ldarg.1
    IL_0025:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_002a:  stloc.1
    IL_002b:  ldloca.s   V_0
    IL_002d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0032:  ldloc.1
    IL_0033:  ceq
    IL_0035:  ldloca.s   V_0
    IL_0037:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_003c:  and
    IL_003d:  ldc.i4.0
    IL_003e:  ceq
    IL_0040:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0045:  nop
    IL_0046:  ldarg.0
    IL_0047:  stloc.0
    IL_0048:  ldarg.1
    IL_0049:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_004e:  stloc.1
    IL_004f:  ldloca.s   V_0
    IL_0051:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0056:  ldloc.1
    IL_0057:  cgt
    IL_0059:  ldloca.s   V_0
    IL_005b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0060:  and
    IL_0061:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0066:  nop
    IL_0067:  ldarg.1
    IL_0068:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_006d:  ldarg.0
    IL_006e:  stloc.0
    IL_006f:  ldloca.s   V_0
    IL_0071:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0076:  ceq
    IL_0078:  ldloca.s   V_0
    IL_007a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_007f:  and
    IL_0080:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0085:  nop
    IL_0086:  ldarg.1
    IL_0087:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_008c:  ldarg.0
    IL_008d:  stloc.0
    IL_008e:  ldloca.s   V_0
    IL_0090:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0095:  ceq
    IL_0097:  ldloca.s   V_0
    IL_0099:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_009e:  and
    IL_009f:  ldc.i4.0
    IL_00a0:  ceq
    IL_00a2:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00a7:  nop
    IL_00a8:  ldarg.1
    IL_00a9:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00ae:  ldarg.0
    IL_00af:  stloc.0
    IL_00b0:  ldloca.s   V_0
    IL_00b2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00b7:  cgt
    IL_00b9:  ldloca.s   V_0
    IL_00bb:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00c0:  and
    IL_00c1:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00c6:  nop
    IL_00c7:  ldarg.0
    IL_00c8:  stloc.0
    IL_00c9:  ldarg.1
    IL_00ca:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00cf:  stloc.1
    IL_00d0:  ldloca.s   V_0
    IL_00d2:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00d7:  brtrue.s   IL_00e4

    IL_00d9:  ldloca.s   V_2
    IL_00db:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00e1:  ldloc.2
    IL_00e2:  br.s       IL_00f2

    IL_00e4:  ldloca.s   V_0
    IL_00e6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00eb:  ldloc.1
    IL_00ec:  add
    IL_00ed:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00f2:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_00f7:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00fc:  nop
    IL_00fd:  ldarg.0
    IL_00fe:  stloc.0
    IL_00ff:  ldarg.1
    IL_0100:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0105:  stloc.1
    IL_0106:  ldloca.s   V_0
    IL_0108:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_010d:  brtrue.s   IL_011a

    IL_010f:  ldloca.s   V_2
    IL_0111:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0117:  ldloc.2
    IL_0118:  br.s       IL_0128

    IL_011a:  ldloca.s   V_0
    IL_011c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0121:  ldloc.1
    IL_0122:  sub
    IL_0123:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0128:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_012d:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0132:  nop
    IL_0133:  ldarg.0
    IL_0134:  stloc.0
    IL_0135:  ldarg.1
    IL_0136:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_013b:  stloc.1
    IL_013c:  ldloca.s   V_0
    IL_013e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0143:  brtrue.s   IL_0150

    IL_0145:  ldloca.s   V_2
    IL_0147:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_014d:  ldloc.2
    IL_014e:  br.s       IL_015e

    IL_0150:  ldloca.s   V_0
    IL_0152:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0157:  ldloc.1
    IL_0158:  mul
    IL_0159:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_015e:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0163:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0168:  nop
    IL_0169:  ldarg.0
    IL_016a:  stloc.0
    IL_016b:  ldarg.1
    IL_016c:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0171:  stloc.1
    IL_0172:  ldloca.s   V_0
    IL_0174:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0179:  brtrue.s   IL_0186

    IL_017b:  ldloca.s   V_2
    IL_017d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0183:  ldloc.2
    IL_0184:  br.s       IL_0194

    IL_0186:  ldloca.s   V_0
    IL_0188:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_018d:  ldloc.1
    IL_018e:  div
    IL_018f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0194:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0199:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_019e:  nop
    IL_019f:  ldarg.0
    IL_01a0:  stloc.0
    IL_01a1:  ldarg.1
    IL_01a2:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_01a7:  stloc.1
    IL_01a8:  ldloca.s   V_0
    IL_01aa:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01af:  brtrue.s   IL_01bc

    IL_01b1:  ldloca.s   V_2
    IL_01b3:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01b9:  ldloc.2
    IL_01ba:  br.s       IL_01ca

    IL_01bc:  ldloca.s   V_0
    IL_01be:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01c3:  ldloc.1
    IL_01c4:  rem
    IL_01c5:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01ca:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01cf:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01d4:  nop
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
    IL_020a:  nop
    IL_020b:  ldarg.0
    IL_020c:  stloc.0
    IL_020d:  ldarg.1
    IL_020e:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0213:  stloc.1
    IL_0214:  ldloca.s   V_0
    IL_0216:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_021b:  brtrue.s   IL_0228

    IL_021d:  ldloca.s   V_2
    IL_021f:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0225:  ldloc.2
    IL_0226:  br.s       IL_0236

    IL_0228:  ldloca.s   V_0
    IL_022a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_022f:  ldloc.1
    IL_0230:  or
    IL_0231:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0236:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_023b:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0240:  nop
    IL_0241:  ldarg.0
    IL_0242:  stloc.0
    IL_0243:  ldarg.1
    IL_0244:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0249:  stloc.1
    IL_024a:  ldloca.s   V_0
    IL_024c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0251:  brtrue.s   IL_025e

    IL_0253:  ldloca.s   V_2
    IL_0255:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_025b:  ldloc.2
    IL_025c:  br.s       IL_026c

    IL_025e:  ldloca.s   V_0
    IL_0260:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0265:  ldloc.1
    IL_0266:  xor
    IL_0267:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_026c:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0271:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0276:  nop
    IL_0277:  ldarg.0
    IL_0278:  stloc.0
    IL_0279:  ldarg.1
    IL_027a:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_027f:  stloc.1
    IL_0280:  ldloca.s   V_0
    IL_0282:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0287:  brtrue.s   IL_0294

    IL_0289:  ldloca.s   V_2
    IL_028b:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0291:  ldloc.2
    IL_0292:  br.s       IL_02a5

    IL_0294:  ldloca.s   V_0
    IL_0296:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_029b:  ldloc.1
    IL_029c:  ldc.i4.s   31
    IL_029e:  and
    IL_029f:  shl
    IL_02a0:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02a5:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_02aa:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02af:  nop
    IL_02b0:  ldarg.0
    IL_02b1:  stloc.0
    IL_02b2:  ldarg.1
    IL_02b3:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_02b8:  stloc.1
    IL_02b9:  ldloca.s   V_0
    IL_02bb:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02c0:  brtrue.s   IL_02cd

    IL_02c2:  ldloca.s   V_2
    IL_02c4:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02ca:  ldloc.2
    IL_02cb:  br.s       IL_02de

    IL_02cd:  ldloca.s   V_0
    IL_02cf:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02d4:  ldloc.1
    IL_02d5:  ldc.i4.s   31
    IL_02d7:  and
    IL_02d8:  shr
    IL_02d9:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02de:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_02e3:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02e8:  nop
    IL_02e9:  ldarg.0
    IL_02ea:  stloc.0
    IL_02eb:  ldloca.s   V_0
    IL_02ed:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02f2:  brtrue.s   IL_02fc

    IL_02f4:  ldarg.1
    IL_02f5:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_02fa:  br.s       IL_0303

    IL_02fc:  ldloca.s   V_0
    IL_02fe:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0303:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0308:  nop
    IL_0309:  ldarg.0
    IL_030a:  stloc.0
    IL_030b:  ldarg.1
    IL_030c:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0311:  stloc.1
    IL_0312:  ldloca.s   V_0
    IL_0314:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0319:  brtrue.s   IL_0326

    IL_031b:  ldloca.s   V_2
    IL_031d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0323:  ldloc.2
    IL_0324:  br.s       IL_0334

    IL_0326:  ldloca.s   V_0
    IL_0328:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_032d:  ldloc.1
    IL_032e:  add
    IL_032f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0334:  starg.s    a
    IL_0336:  ldarg.0
    IL_0337:  stloc.0
    IL_0338:  ldarg.1
    IL_0339:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_033e:  stloc.1
    IL_033f:  ldloca.s   V_0
    IL_0341:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0346:  brtrue.s   IL_0353

    IL_0348:  ldloca.s   V_2
    IL_034a:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0350:  ldloc.2
    IL_0351:  br.s       IL_0361

    IL_0353:  ldloca.s   V_0
    IL_0355:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_035a:  ldloc.1
    IL_035b:  sub
    IL_035c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0361:  starg.s    a
    IL_0363:  ldarg.0
    IL_0364:  stloc.0
    IL_0365:  ldarg.1
    IL_0366:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_036b:  stloc.1
    IL_036c:  ldloca.s   V_0
    IL_036e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0373:  brtrue.s   IL_0380

    IL_0375:  ldloca.s   V_2
    IL_0377:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_037d:  ldloc.2
    IL_037e:  br.s       IL_038e

    IL_0380:  ldloca.s   V_0
    IL_0382:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0387:  ldloc.1
    IL_0388:  mul
    IL_0389:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_038e:  starg.s    a
    IL_0390:  ldarg.0
    IL_0391:  stloc.0
    IL_0392:  ldarg.1
    IL_0393:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0398:  stloc.1
    IL_0399:  ldloca.s   V_0
    IL_039b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03a0:  brtrue.s   IL_03ad

    IL_03a2:  ldloca.s   V_2
    IL_03a4:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03aa:  ldloc.2
    IL_03ab:  br.s       IL_03bb

    IL_03ad:  ldloca.s   V_0
    IL_03af:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03b4:  ldloc.1
    IL_03b5:  div
    IL_03b6:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03bb:  starg.s    a
    IL_03bd:  ldarg.0
    IL_03be:  stloc.0
    IL_03bf:  ldarg.1
    IL_03c0:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_03c5:  stloc.1
    IL_03c6:  ldloca.s   V_0
    IL_03c8:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03cd:  brtrue.s   IL_03da

    IL_03cf:  ldloca.s   V_2
    IL_03d1:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03d7:  ldloc.2
    IL_03d8:  br.s       IL_03e8

    IL_03da:  ldloca.s   V_0
    IL_03dc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03e1:  ldloc.1
    IL_03e2:  rem
    IL_03e3:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03e8:  starg.s    a
    IL_03ea:  ldarg.0
    IL_03eb:  stloc.0
    IL_03ec:  ldarg.1
    IL_03ed:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_03f2:  stloc.1
    IL_03f3:  ldloca.s   V_0
    IL_03f5:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03fa:  brtrue.s   IL_0407

    IL_03fc:  ldloca.s   V_2
    IL_03fe:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0404:  ldloc.2
    IL_0405:  br.s       IL_0415

    IL_0407:  ldloca.s   V_0
    IL_0409:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_040e:  ldloc.1
    IL_040f:  and
    IL_0410:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0415:  starg.s    a
    IL_0417:  ldarg.0
    IL_0418:  stloc.0
    IL_0419:  ldarg.1
    IL_041a:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_041f:  stloc.1
    IL_0420:  ldloca.s   V_0
    IL_0422:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0427:  brtrue.s   IL_0434

    IL_0429:  ldloca.s   V_2
    IL_042b:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0431:  ldloc.2
    IL_0432:  br.s       IL_0442

    IL_0434:  ldloca.s   V_0
    IL_0436:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_043b:  ldloc.1
    IL_043c:  or
    IL_043d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0442:  starg.s    a
    IL_0444:  ldarg.0
    IL_0445:  stloc.0
    IL_0446:  ldarg.1
    IL_0447:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_044c:  stloc.1
    IL_044d:  ldloca.s   V_0
    IL_044f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0454:  brtrue.s   IL_0461

    IL_0456:  ldloca.s   V_2
    IL_0458:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_045e:  ldloc.2
    IL_045f:  br.s       IL_046f

    IL_0461:  ldloca.s   V_0
    IL_0463:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0468:  ldloc.1
    IL_0469:  xor
    IL_046a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_046f:  starg.s    a
    IL_0471:  ldarg.0
    IL_0472:  stloc.0
    IL_0473:  ldarg.1
    IL_0474:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0479:  stloc.1
    IL_047a:  ldloca.s   V_0
    IL_047c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0481:  brtrue.s   IL_048e

    IL_0483:  ldloca.s   V_2
    IL_0485:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_048b:  ldloc.2
    IL_048c:  br.s       IL_049f

    IL_048e:  ldloca.s   V_0
    IL_0490:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0495:  ldloc.1
    IL_0496:  ldc.i4.s   31
    IL_0498:  and
    IL_0499:  shl
    IL_049a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_049f:  starg.s    a
    IL_04a1:  ldarg.0
    IL_04a2:  stloc.0
    IL_04a3:  ldarg.1
    IL_04a4:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_04a9:  stloc.1
    IL_04aa:  ldloca.s   V_0
    IL_04ac:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04b1:  brtrue.s   IL_04be

    IL_04b3:  ldloca.s   V_2
    IL_04b5:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04bb:  ldloc.2
    IL_04bc:  br.s       IL_04cf

    IL_04be:  ldloca.s   V_0
    IL_04c0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04c5:  ldloc.1
    IL_04c6:  ldc.i4.s   31
    IL_04c8:  and
    IL_04c9:  shr
    IL_04ca:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04cf:  starg.s    a
    IL_04d1:  ldarg.1
    IL_04d2:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_04d7:  stloc.1
    IL_04d8:  ldarg.0
    IL_04d9:  stloc.0
    IL_04da:  ldloca.s   V_0
    IL_04dc:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04e1:  brtrue.s   IL_04ee

    IL_04e3:  ldloca.s   V_2
    IL_04e5:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04eb:  ldloc.2
    IL_04ec:  br.s       IL_04fc

    IL_04ee:  ldloc.1
    IL_04ef:  ldloca.s   V_0
    IL_04f1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04f6:  add
    IL_04f7:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04fc:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0501:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0506:  nop
    IL_0507:  ldc.i4.0
    IL_0508:  newarr     valuetype [mscorlib]System.Nullable`1<int32>
    IL_050d:  ldc.i4.0
    IL_050e:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0513:  dup
    IL_0514:  ldobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0519:  stloc.0
    IL_051a:  ldarg.1
    IL_051b:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0520:  stloc.1
    IL_0521:  ldloca.s   V_0
    IL_0523:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0528:  brtrue.s   IL_0535

    IL_052a:  ldloca.s   V_2
    IL_052c:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0532:  ldloc.2
    IL_0533:  br.s       IL_0543

    IL_0535:  ldloca.s   V_0
    IL_0537:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_053c:  ldloc.1
    IL_053d:  add
    IL_053e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0543:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0548:  ret
  } // end of method LiftedOperators::IntValueComplex

  .method public hidebysig static void  IntValueConst(valuetype [mscorlib]System.Nullable`1<int32> a) cil managed
  {
    // Code size       1094 (0x446)
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
    IL_000d:  ceq
    IL_000f:  ldloca.s   V_0
    IL_0011:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0016:  and
    IL_0017:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_001c:  nop
    IL_001d:  ldarg.0
    IL_001e:  stloc.0
    IL_001f:  ldc.i4.2
    IL_0020:  stloc.1
    IL_0021:  ldloca.s   V_0
    IL_0023:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0028:  ldloc.1
    IL_0029:  ceq
    IL_002b:  ldloca.s   V_0
    IL_002d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0032:  and
    IL_0033:  ldc.i4.0
    IL_0034:  ceq
    IL_0036:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_003b:  nop
    IL_003c:  ldarg.0
    IL_003d:  stloc.0
    IL_003e:  ldc.i4.2
    IL_003f:  stloc.1
    IL_0040:  ldloca.s   V_0
    IL_0042:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0047:  ldloc.1
    IL_0048:  cgt
    IL_004a:  ldloca.s   V_0
    IL_004c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0051:  and
    IL_0052:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0057:  nop
    IL_0058:  ldc.i4.2
    IL_0059:  ldarg.0
    IL_005a:  stloc.0
    IL_005b:  ldloca.s   V_0
    IL_005d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0062:  ceq
    IL_0064:  ldloca.s   V_0
    IL_0066:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_006b:  and
    IL_006c:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0071:  nop
    IL_0072:  ldc.i4.2
    IL_0073:  ldarg.0
    IL_0074:  stloc.0
    IL_0075:  ldloca.s   V_0
    IL_0077:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_007c:  ceq
    IL_007e:  ldloca.s   V_0
    IL_0080:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0085:  and
    IL_0086:  ldc.i4.0
    IL_0087:  ceq
    IL_0089:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_008e:  nop
    IL_008f:  ldc.i4.2
    IL_0090:  ldarg.0
    IL_0091:  stloc.0
    IL_0092:  ldloca.s   V_0
    IL_0094:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0099:  cgt
    IL_009b:  ldloca.s   V_0
    IL_009d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00a2:  and
    IL_00a3:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00a8:  nop
    IL_00a9:  ldarg.0
    IL_00aa:  stloc.0
    IL_00ab:  ldloca.s   V_0
    IL_00ad:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00b2:  brtrue.s   IL_00bf

    IL_00b4:  ldloca.s   V_2
    IL_00b6:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00bc:  ldloc.2
    IL_00bd:  br.s       IL_00cd

    IL_00bf:  ldloca.s   V_0
    IL_00c1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00c6:  ldc.i4.2
    IL_00c7:  add
    IL_00c8:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00cd:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_00d2:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00d7:  nop
    IL_00d8:  ldarg.0
    IL_00d9:  stloc.0
    IL_00da:  ldloca.s   V_0
    IL_00dc:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00e1:  brtrue.s   IL_00ee

    IL_00e3:  ldloca.s   V_2
    IL_00e5:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00eb:  ldloc.2
    IL_00ec:  br.s       IL_00fc

    IL_00ee:  ldloca.s   V_0
    IL_00f0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00f5:  ldc.i4.2
    IL_00f6:  sub
    IL_00f7:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00fc:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0101:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0106:  nop
    IL_0107:  ldarg.0
    IL_0108:  stloc.0
    IL_0109:  ldloca.s   V_0
    IL_010b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0110:  brtrue.s   IL_011d

    IL_0112:  ldloca.s   V_2
    IL_0114:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_011a:  ldloc.2
    IL_011b:  br.s       IL_012b

    IL_011d:  ldloca.s   V_0
    IL_011f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0124:  ldc.i4.2
    IL_0125:  mul
    IL_0126:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_012b:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0130:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0135:  nop
    IL_0136:  ldarg.0
    IL_0137:  stloc.0
    IL_0138:  ldloca.s   V_0
    IL_013a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_013f:  brtrue.s   IL_014c

    IL_0141:  ldloca.s   V_2
    IL_0143:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0149:  ldloc.2
    IL_014a:  br.s       IL_015a

    IL_014c:  ldloca.s   V_0
    IL_014e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0153:  ldc.i4.2
    IL_0154:  div
    IL_0155:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_015a:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_015f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0164:  nop
    IL_0165:  ldarg.0
    IL_0166:  stloc.0
    IL_0167:  ldloca.s   V_0
    IL_0169:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_016e:  brtrue.s   IL_017b

    IL_0170:  ldloca.s   V_2
    IL_0172:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0178:  ldloc.2
    IL_0179:  br.s       IL_0189

    IL_017b:  ldloca.s   V_0
    IL_017d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0182:  ldc.i4.2
    IL_0183:  rem
    IL_0184:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0189:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_018e:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0193:  nop
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
    IL_01c2:  nop
    IL_01c3:  ldarg.0
    IL_01c4:  stloc.0
    IL_01c5:  ldloca.s   V_0
    IL_01c7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01cc:  brtrue.s   IL_01d9

    IL_01ce:  ldloca.s   V_2
    IL_01d0:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01d6:  ldloc.2
    IL_01d7:  br.s       IL_01e7

    IL_01d9:  ldloca.s   V_0
    IL_01db:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01e0:  ldc.i4.2
    IL_01e1:  or
    IL_01e2:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01e7:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01ec:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01f1:  nop
    IL_01f2:  ldarg.0
    IL_01f3:  stloc.0
    IL_01f4:  ldloca.s   V_0
    IL_01f6:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01fb:  brtrue.s   IL_0208

    IL_01fd:  ldloca.s   V_2
    IL_01ff:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0205:  ldloc.2
    IL_0206:  br.s       IL_0216

    IL_0208:  ldloca.s   V_0
    IL_020a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_020f:  ldc.i4.2
    IL_0210:  xor
    IL_0211:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0216:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_021b:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0220:  nop
    IL_0221:  ldarg.0
    IL_0222:  stloc.0
    IL_0223:  ldloca.s   V_0
    IL_0225:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_022a:  brtrue.s   IL_0237

    IL_022c:  ldloca.s   V_2
    IL_022e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0234:  ldloc.2
    IL_0235:  br.s       IL_0245

    IL_0237:  ldloca.s   V_0
    IL_0239:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_023e:  ldc.i4.2
    IL_023f:  shl
    IL_0240:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0245:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_024a:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_024f:  nop
    IL_0250:  ldarg.0
    IL_0251:  stloc.0
    IL_0252:  ldloca.s   V_0
    IL_0254:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0259:  brtrue.s   IL_0266

    IL_025b:  ldloca.s   V_2
    IL_025d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0263:  ldloc.2
    IL_0264:  br.s       IL_0274

    IL_0266:  ldloca.s   V_0
    IL_0268:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_026d:  ldc.i4.2
    IL_026e:  shr
    IL_026f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0274:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0279:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_027e:  nop
    IL_027f:  ldarg.0
    IL_0280:  stloc.0
    IL_0281:  ldloca.s   V_0
    IL_0283:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0288:  brtrue.s   IL_028d

    IL_028a:  ldc.i4.2
    IL_028b:  br.s       IL_0294

    IL_028d:  ldloca.s   V_0
    IL_028f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0294:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0299:  nop
    IL_029a:  ldarg.0
    IL_029b:  stloc.0
    IL_029c:  ldloca.s   V_0
    IL_029e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02a3:  brtrue.s   IL_02b0

    IL_02a5:  ldloca.s   V_2
    IL_02a7:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02ad:  ldloc.2
    IL_02ae:  br.s       IL_02be

    IL_02b0:  ldloca.s   V_0
    IL_02b2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02b7:  ldc.i4.2
    IL_02b8:  add
    IL_02b9:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02be:  starg.s    a
    IL_02c0:  ldarg.0
    IL_02c1:  stloc.0
    IL_02c2:  ldloca.s   V_0
    IL_02c4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02c9:  brtrue.s   IL_02d6

    IL_02cb:  ldloca.s   V_2
    IL_02cd:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02d3:  ldloc.2
    IL_02d4:  br.s       IL_02e4

    IL_02d6:  ldloca.s   V_0
    IL_02d8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02dd:  ldc.i4.2
    IL_02de:  sub
    IL_02df:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02e4:  starg.s    a
    IL_02e6:  ldarg.0
    IL_02e7:  stloc.0
    IL_02e8:  ldloca.s   V_0
    IL_02ea:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02ef:  brtrue.s   IL_02fc

    IL_02f1:  ldloca.s   V_2
    IL_02f3:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02f9:  ldloc.2
    IL_02fa:  br.s       IL_030a

    IL_02fc:  ldloca.s   V_0
    IL_02fe:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0303:  ldc.i4.2
    IL_0304:  mul
    IL_0305:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_030a:  starg.s    a
    IL_030c:  ldarg.0
    IL_030d:  stloc.0
    IL_030e:  ldloca.s   V_0
    IL_0310:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0315:  brtrue.s   IL_0322

    IL_0317:  ldloca.s   V_2
    IL_0319:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_031f:  ldloc.2
    IL_0320:  br.s       IL_0330

    IL_0322:  ldloca.s   V_0
    IL_0324:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0329:  ldc.i4.2
    IL_032a:  div
    IL_032b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0330:  starg.s    a
    IL_0332:  ldarg.0
    IL_0333:  stloc.0
    IL_0334:  ldloca.s   V_0
    IL_0336:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_033b:  brtrue.s   IL_0348

    IL_033d:  ldloca.s   V_2
    IL_033f:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0345:  ldloc.2
    IL_0346:  br.s       IL_0356

    IL_0348:  ldloca.s   V_0
    IL_034a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_034f:  ldc.i4.2
    IL_0350:  rem
    IL_0351:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0356:  starg.s    a
    IL_0358:  ldarg.0
    IL_0359:  stloc.0
    IL_035a:  ldloca.s   V_0
    IL_035c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0361:  brtrue.s   IL_036e

    IL_0363:  ldloca.s   V_2
    IL_0365:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_036b:  ldloc.2
    IL_036c:  br.s       IL_037c

    IL_036e:  ldloca.s   V_0
    IL_0370:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0375:  ldc.i4.2
    IL_0376:  and
    IL_0377:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_037c:  starg.s    a
    IL_037e:  ldarg.0
    IL_037f:  stloc.0
    IL_0380:  ldloca.s   V_0
    IL_0382:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0387:  brtrue.s   IL_0394

    IL_0389:  ldloca.s   V_2
    IL_038b:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0391:  ldloc.2
    IL_0392:  br.s       IL_03a2

    IL_0394:  ldloca.s   V_0
    IL_0396:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_039b:  ldc.i4.2
    IL_039c:  or
    IL_039d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03a2:  starg.s    a
    IL_03a4:  ldarg.0
    IL_03a5:  stloc.0
    IL_03a6:  ldloca.s   V_0
    IL_03a8:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03ad:  brtrue.s   IL_03ba

    IL_03af:  ldloca.s   V_2
    IL_03b1:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03b7:  ldloc.2
    IL_03b8:  br.s       IL_03c8

    IL_03ba:  ldloca.s   V_0
    IL_03bc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03c1:  ldc.i4.2
    IL_03c2:  xor
    IL_03c3:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03c8:  starg.s    a
    IL_03ca:  ldarg.0
    IL_03cb:  stloc.0
    IL_03cc:  ldloca.s   V_0
    IL_03ce:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03d3:  brtrue.s   IL_03e0

    IL_03d5:  ldloca.s   V_2
    IL_03d7:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03dd:  ldloc.2
    IL_03de:  br.s       IL_03ee

    IL_03e0:  ldloca.s   V_0
    IL_03e2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03e7:  ldc.i4.2
    IL_03e8:  shl
    IL_03e9:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03ee:  starg.s    a
    IL_03f0:  ldarg.0
    IL_03f1:  stloc.0
    IL_03f2:  ldloca.s   V_0
    IL_03f4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03f9:  brtrue.s   IL_0406

    IL_03fb:  ldloca.s   V_2
    IL_03fd:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0403:  ldloc.2
    IL_0404:  br.s       IL_0414

    IL_0406:  ldloca.s   V_0
    IL_0408:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_040d:  ldc.i4.2
    IL_040e:  shr
    IL_040f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0414:  starg.s    a
    IL_0416:  ldarg.0
    IL_0417:  stloc.0
    IL_0418:  ldloca.s   V_0
    IL_041a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_041f:  brtrue.s   IL_042c

    IL_0421:  ldloca.s   V_2
    IL_0423:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0429:  ldloc.2
    IL_042a:  br.s       IL_043a

    IL_042c:  ldc.i4.2
    IL_042d:  ldloca.s   V_0
    IL_042f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0434:  add
    IL_0435:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_043a:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_043f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0444:  nop
    IL_0445:  ret
  } // end of method LiftedOperators::IntValueConst

  .method public hidebysig static void  NumberBasic(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> a,
                                                    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> b) cil managed
  {
    // Code size       433 (0x1b1)
    .maxstack  3
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
    IL_0018:  ldloca.s   V_1
    IL_001a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_001f:  ldloca.s   V_2
    IL_0021:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0026:  ceq
    IL_0028:  and
    IL_0029:  stloc.0
    IL_002a:  ldloc.0
    IL_002b:  brfalse.s  IL_0035

    IL_002d:  nop
    IL_002e:  call       void [mscorlib]System.Console::WriteLine()
    IL_0033:  nop
    IL_0034:  nop
    IL_0035:  ldarg.0
    IL_0036:  stloc.2
    IL_0037:  ldarg.1
    IL_0038:  stloc.1
    IL_0039:  ldloca.s   V_2
    IL_003b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0040:  ldloca.s   V_1
    IL_0042:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0047:  call       bool [mscorlib]System.Decimal::op_Equality(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_004c:  ldloca.s   V_2
    IL_004e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0053:  ldloca.s   V_1
    IL_0055:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_005a:  ceq
    IL_005c:  and
    IL_005d:  ldc.i4.0
    IL_005e:  ceq
    IL_0060:  stloc.3
    IL_0061:  ldloc.3
    IL_0062:  brfalse.s  IL_006c

    IL_0064:  nop
    IL_0065:  call       void [mscorlib]System.Console::WriteLine()
    IL_006a:  nop
    IL_006b:  nop
    IL_006c:  ldarg.0
    IL_006d:  stloc.1
    IL_006e:  ldarg.1
    IL_006f:  stloc.2
    IL_0070:  ldloca.s   V_1
    IL_0072:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0077:  ldloca.s   V_2
    IL_0079:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_007e:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_0083:  ldloca.s   V_1
    IL_0085:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_008a:  ldloca.s   V_2
    IL_008c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0091:  and
    IL_0092:  and
    IL_0093:  stloc.s    V_4
    IL_0095:  ldloc.s    V_4
    IL_0097:  brfalse.s  IL_00a1

    IL_0099:  nop
    IL_009a:  call       void [mscorlib]System.Console::WriteLine()
    IL_009f:  nop
    IL_00a0:  nop
    IL_00a1:  ldarg.0
    IL_00a2:  stloc.2
    IL_00a3:  ldarg.1
    IL_00a4:  stloc.1
    IL_00a5:  ldloca.s   V_2
    IL_00a7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00ac:  ldloca.s   V_1
    IL_00ae:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00b3:  call       bool [mscorlib]System.Decimal::op_LessThan(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_00b8:  ldloca.s   V_2
    IL_00ba:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00bf:  ldloca.s   V_1
    IL_00c1:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00c6:  and
    IL_00c7:  and
    IL_00c8:  stloc.s    V_5
    IL_00ca:  ldloc.s    V_5
    IL_00cc:  brfalse.s  IL_00d6

    IL_00ce:  nop
    IL_00cf:  call       void [mscorlib]System.Console::WriteLine()
    IL_00d4:  nop
    IL_00d5:  nop
    IL_00d6:  ldarg.0
    IL_00d7:  stloc.1
    IL_00d8:  ldarg.1
    IL_00d9:  stloc.2
    IL_00da:  ldloca.s   V_1
    IL_00dc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00e1:  ldloca.s   V_2
    IL_00e3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00e8:  call       bool [mscorlib]System.Decimal::op_GreaterThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                              valuetype [mscorlib]System.Decimal)
    IL_00ed:  ldloca.s   V_1
    IL_00ef:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00f4:  ldloca.s   V_2
    IL_00f6:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00fb:  and
    IL_00fc:  and
    IL_00fd:  stloc.s    V_6
    IL_00ff:  ldloc.s    V_6
    IL_0101:  brfalse.s  IL_010b

    IL_0103:  nop
    IL_0104:  call       void [mscorlib]System.Console::WriteLine()
    IL_0109:  nop
    IL_010a:  nop
    IL_010b:  ldarg.0
    IL_010c:  stloc.2
    IL_010d:  ldarg.1
    IL_010e:  stloc.1
    IL_010f:  ldloca.s   V_2
    IL_0111:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0116:  ldloca.s   V_1
    IL_0118:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_011d:  call       bool [mscorlib]System.Decimal::op_LessThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                           valuetype [mscorlib]System.Decimal)
    IL_0122:  ldloca.s   V_2
    IL_0124:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0129:  ldloca.s   V_1
    IL_012b:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0130:  and
    IL_0131:  and
    IL_0132:  stloc.s    V_7
    IL_0134:  ldloc.s    V_7
    IL_0136:  brfalse.s  IL_0140

    IL_0138:  nop
    IL_0139:  call       void [mscorlib]System.Console::WriteLine()
    IL_013e:  nop
    IL_013f:  nop
    IL_0140:  ldarg.0
    IL_0141:  stloc.1
    IL_0142:  ldarg.1
    IL_0143:  stloc.2
    IL_0144:  ldloca.s   V_1
    IL_0146:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_014b:  ldloca.s   V_2
    IL_014d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0152:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_0157:  ldloca.s   V_1
    IL_0159:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_015e:  ldloca.s   V_2
    IL_0160:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0165:  and
    IL_0166:  and
    IL_0167:  ldc.i4.0
    IL_0168:  ceq
    IL_016a:  stloc.s    V_8
    IL_016c:  ldloc.s    V_8
    IL_016e:  brfalse.s  IL_0178

    IL_0170:  nop
    IL_0171:  call       void [mscorlib]System.Console::WriteLine()
    IL_0176:  nop
    IL_0177:  nop
    IL_0178:  ldarg.0
    IL_0179:  stloc.2
    IL_017a:  ldarg.1
    IL_017b:  stloc.1
    IL_017c:  ldloca.s   V_2
    IL_017e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0183:  ldloca.s   V_1
    IL_0185:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_018a:  call       bool [mscorlib]System.Decimal::op_LessThan(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_018f:  ldloca.s   V_2
    IL_0191:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0196:  ldloca.s   V_1
    IL_0198:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_019d:  and
    IL_019e:  and
    IL_019f:  ldc.i4.0
    IL_01a0:  ceq
    IL_01a2:  stloc.s    V_9
    IL_01a4:  ldloc.s    V_9
    IL_01a6:  brfalse.s  IL_01b0

    IL_01a8:  nop
    IL_01a9:  call       void [mscorlib]System.Console::WriteLine()
    IL_01ae:  nop
    IL_01af:  nop
    IL_01b0:  ret
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
    // Code size       939 (0x3ab)
    .maxstack  3
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
    IL_0018:  ldloca.s   V_0
    IL_001a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_001f:  ldloca.s   V_1
    IL_0021:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0026:  ceq
    IL_0028:  and
    IL_0029:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_002e:  nop
    IL_002f:  ldarg.0
    IL_0030:  stloc.1
    IL_0031:  ldarg.1
    IL_0032:  stloc.0
    IL_0033:  ldloca.s   V_1
    IL_0035:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_003a:  ldloca.s   V_0
    IL_003c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0041:  call       bool [mscorlib]System.Decimal::op_Equality(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_0046:  ldloca.s   V_1
    IL_0048:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_004d:  ldloca.s   V_0
    IL_004f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0054:  ceq
    IL_0056:  and
    IL_0057:  ldc.i4.0
    IL_0058:  ceq
    IL_005a:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_005f:  nop
    IL_0060:  ldarg.0
    IL_0061:  stloc.0
    IL_0062:  ldarg.1
    IL_0063:  stloc.1
    IL_0064:  ldloca.s   V_0
    IL_0066:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_006b:  ldloca.s   V_1
    IL_006d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0072:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_0077:  ldloca.s   V_0
    IL_0079:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_007e:  ldloca.s   V_1
    IL_0080:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0085:  and
    IL_0086:  and
    IL_0087:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_008c:  nop
    IL_008d:  ldarg.0
    IL_008e:  stloc.1
    IL_008f:  ldarg.1
    IL_0090:  stloc.0
    IL_0091:  ldloca.s   V_1
    IL_0093:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0098:  ldloca.s   V_0
    IL_009a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_009f:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_00a4:  ldloca.s   V_1
    IL_00a6:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00ab:  ldloca.s   V_0
    IL_00ad:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00b2:  and
    IL_00b3:  and
    IL_00b4:  ldc.i4.0
    IL_00b5:  ceq
    IL_00b7:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00bc:  nop
    IL_00bd:  ldarg.0
    IL_00be:  stloc.0
    IL_00bf:  ldarg.1
    IL_00c0:  stloc.1
    IL_00c1:  ldloca.s   V_0
    IL_00c3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00c8:  ldloca.s   V_1
    IL_00ca:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00cf:  call       bool [mscorlib]System.Decimal::op_LessThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                           valuetype [mscorlib]System.Decimal)
    IL_00d4:  ldloca.s   V_0
    IL_00d6:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00db:  ldloca.s   V_1
    IL_00dd:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00e2:  and
    IL_00e3:  and
    IL_00e4:  ldc.i4.0
    IL_00e5:  ceq
    IL_00e7:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00ec:  nop
    IL_00ed:  ldarg.0
    IL_00ee:  stloc.1
    IL_00ef:  ldarg.1
    IL_00f0:  stloc.0
    IL_00f1:  ldloca.s   V_1
    IL_00f3:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00f8:  ldloca.s   V_0
    IL_00fa:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00ff:  and
    IL_0100:  brtrue.s   IL_010d

    IL_0102:  ldloca.s   V_2
    IL_0104:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_010a:  ldloc.2
    IL_010b:  br.s       IL_0125

    IL_010d:  ldloca.s   V_1
    IL_010f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0114:  ldloca.s   V_0
    IL_0116:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_011b:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_0120:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0125:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_012a:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_012f:  nop
    IL_0130:  ldarg.0
    IL_0131:  stloc.0
    IL_0132:  ldarg.1
    IL_0133:  stloc.1
    IL_0134:  ldloca.s   V_0
    IL_0136:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_013b:  ldloca.s   V_1
    IL_013d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0142:  and
    IL_0143:  brtrue.s   IL_0150

    IL_0145:  ldloca.s   V_2
    IL_0147:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_014d:  ldloc.2
    IL_014e:  br.s       IL_0168

    IL_0150:  ldloca.s   V_0
    IL_0152:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0157:  ldloca.s   V_1
    IL_0159:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_015e:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Subtraction(valuetype [mscorlib]System.Decimal,
                                                                                                     valuetype [mscorlib]System.Decimal)
    IL_0163:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0168:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_016d:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0172:  nop
    IL_0173:  ldarg.0
    IL_0174:  stloc.1
    IL_0175:  ldarg.1
    IL_0176:  stloc.0
    IL_0177:  ldloca.s   V_1
    IL_0179:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_017e:  ldloca.s   V_0
    IL_0180:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0185:  and
    IL_0186:  brtrue.s   IL_0193

    IL_0188:  ldloca.s   V_2
    IL_018a:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0190:  ldloc.2
    IL_0191:  br.s       IL_01ab

    IL_0193:  ldloca.s   V_1
    IL_0195:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_019a:  ldloca.s   V_0
    IL_019c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01a1:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Multiply(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_01a6:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_01ab:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_01b0:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01b5:  nop
    IL_01b6:  ldarg.0
    IL_01b7:  stloc.0
    IL_01b8:  ldarg.1
    IL_01b9:  stloc.1
    IL_01ba:  ldloca.s   V_0
    IL_01bc:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_01c1:  ldloca.s   V_1
    IL_01c3:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_01c8:  and
    IL_01c9:  brtrue.s   IL_01d6

    IL_01cb:  ldloca.s   V_2
    IL_01cd:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_01d3:  ldloc.2
    IL_01d4:  br.s       IL_01ee

    IL_01d6:  ldloca.s   V_0
    IL_01d8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01dd:  ldloca.s   V_1
    IL_01df:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01e4:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Division(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_01e9:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_01ee:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_01f3:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01f8:  nop
    IL_01f9:  ldarg.0
    IL_01fa:  stloc.1
    IL_01fb:  ldarg.1
    IL_01fc:  stloc.0
    IL_01fd:  ldloca.s   V_1
    IL_01ff:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0204:  ldloca.s   V_0
    IL_0206:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_020b:  and
    IL_020c:  brtrue.s   IL_0219

    IL_020e:  ldloca.s   V_2
    IL_0210:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0216:  ldloc.2
    IL_0217:  br.s       IL_0231

    IL_0219:  ldloca.s   V_1
    IL_021b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0220:  ldloca.s   V_0
    IL_0222:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0227:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Modulus(valuetype [mscorlib]System.Decimal,
                                                                                                 valuetype [mscorlib]System.Decimal)
    IL_022c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0231:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0236:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_023b:  nop
    IL_023c:  ldarg.0
    IL_023d:  stloc.0
    IL_023e:  ldloca.s   V_0
    IL_0240:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0245:  brtrue.s   IL_024a

    IL_0247:  ldarg.1
    IL_0248:  br.s       IL_024b

    IL_024a:  ldloc.0
    IL_024b:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0250:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0255:  nop
    IL_0256:  ldarg.0
    IL_0257:  stloc.0
    IL_0258:  ldloca.s   V_0
    IL_025a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_025f:  brtrue.s   IL_026c

    IL_0261:  ldloca.s   V_1
    IL_0263:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0269:  ldloc.1
    IL_026a:  br.s       IL_027d

    IL_026c:  ldloca.s   V_0
    IL_026e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0273:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_UnaryNegation(valuetype [mscorlib]System.Decimal)
    IL_0278:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_027d:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0282:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0287:  nop
    IL_0288:  ldarg.0
    IL_0289:  stloc.0
    IL_028a:  ldarg.1
    IL_028b:  stloc.1
    IL_028c:  ldloca.s   V_0
    IL_028e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0293:  ldloca.s   V_1
    IL_0295:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_029a:  and
    IL_029b:  brtrue.s   IL_02a8

    IL_029d:  ldloca.s   V_2
    IL_029f:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_02a5:  ldloc.2
    IL_02a6:  br.s       IL_02c0

    IL_02a8:  ldloca.s   V_0
    IL_02aa:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_02af:  ldloca.s   V_1
    IL_02b1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_02b6:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_02bb:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_02c0:  starg.s    a
    IL_02c2:  ldarg.0
    IL_02c3:  stloc.1
    IL_02c4:  ldarg.1
    IL_02c5:  stloc.0
    IL_02c6:  ldloca.s   V_1
    IL_02c8:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_02cd:  ldloca.s   V_0
    IL_02cf:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_02d4:  and
    IL_02d5:  brtrue.s   IL_02e2

    IL_02d7:  ldloca.s   V_2
    IL_02d9:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_02df:  ldloc.2
    IL_02e0:  br.s       IL_02fa

    IL_02e2:  ldloca.s   V_1
    IL_02e4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_02e9:  ldloca.s   V_0
    IL_02eb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_02f0:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Subtraction(valuetype [mscorlib]System.Decimal,
                                                                                                     valuetype [mscorlib]System.Decimal)
    IL_02f5:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_02fa:  starg.s    a
    IL_02fc:  ldarg.0
    IL_02fd:  stloc.0
    IL_02fe:  ldarg.1
    IL_02ff:  stloc.1
    IL_0300:  ldloca.s   V_0
    IL_0302:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0307:  ldloca.s   V_1
    IL_0309:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_030e:  and
    IL_030f:  brtrue.s   IL_031c

    IL_0311:  ldloca.s   V_2
    IL_0313:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0319:  ldloc.2
    IL_031a:  br.s       IL_0334

    IL_031c:  ldloca.s   V_0
    IL_031e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0323:  ldloca.s   V_1
    IL_0325:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_032a:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Multiply(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_032f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0334:  starg.s    a
    IL_0336:  ldarg.0
    IL_0337:  stloc.1
    IL_0338:  ldarg.1
    IL_0339:  stloc.0
    IL_033a:  ldloca.s   V_1
    IL_033c:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0341:  ldloca.s   V_0
    IL_0343:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0348:  and
    IL_0349:  brtrue.s   IL_0356

    IL_034b:  ldloca.s   V_2
    IL_034d:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0353:  ldloc.2
    IL_0354:  br.s       IL_036e

    IL_0356:  ldloca.s   V_1
    IL_0358:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_035d:  ldloca.s   V_0
    IL_035f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0364:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Division(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_0369:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_036e:  starg.s    a
    IL_0370:  ldarg.0
    IL_0371:  stloc.0
    IL_0372:  ldarg.1
    IL_0373:  stloc.1
    IL_0374:  ldloca.s   V_0
    IL_0376:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_037b:  ldloca.s   V_1
    IL_037d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0382:  and
    IL_0383:  brtrue.s   IL_0390

    IL_0385:  ldloca.s   V_2
    IL_0387:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_038d:  ldloc.2
    IL_038e:  br.s       IL_03a8

    IL_0390:  ldloca.s   V_0
    IL_0392:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0397:  ldloca.s   V_1
    IL_0399:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_039e:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Modulus(valuetype [mscorlib]System.Decimal,
                                                                                                 valuetype [mscorlib]System.Decimal)
    IL_03a3:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_03a8:  starg.s    a
    IL_03aa:  ret
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
    // Code size       171 (0xab)
    .maxstack  3
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
    IL_0036:  clt
    IL_0038:  ldloca.s   V_1
    IL_003a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_003f:  ldloca.s   V_2
    IL_0041:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0046:  and
    IL_0047:  and
    IL_0048:  stloc.0
    IL_0049:  ldloc.0
    IL_004a:  brfalse.s  IL_0054

    IL_004c:  nop
    IL_004d:  call       void [mscorlib]System.Console::WriteLine()
    IL_0052:  nop
    IL_0053:  nop
    IL_0054:  ldarg.0
    IL_0055:  stloc.3
    IL_0056:  ldloca.s   V_3
    IL_0058:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_005d:  brtrue.s   IL_006b

    IL_005f:  ldloca.s   V_4
    IL_0061:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0067:  ldloc.s    V_4
    IL_0069:  br.s       IL_0078

    IL_006b:  ldloca.s   V_3
    IL_006d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0072:  conv.i8
    IL_0073:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0078:  stloc.2
    IL_0079:  ldarg.1
    IL_007a:  stloc.1
    IL_007b:  ldloca.s   V_2
    IL_007d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0082:  ldloca.s   V_1
    IL_0084:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0089:  ceq
    IL_008b:  ldloca.s   V_2
    IL_008d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0092:  ldloca.s   V_1
    IL_0094:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0099:  ceq
    IL_009b:  and
    IL_009c:  stloc.s    V_5
    IL_009e:  ldloc.s    V_5
    IL_00a0:  brfalse.s  IL_00aa

    IL_00a2:  nop
    IL_00a3:  call       void [mscorlib]System.Console::WriteLine()
    IL_00a8:  nop
    IL_00a9:  nop
    IL_00aa:  ret
  } // end of method LiftedOperators::CompareWithImplictCast

  .method public hidebysig static void  CompareWithSignChange(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                              valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       118 (0x76)
    .maxstack  3
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
    IL_0057:  clt.un
    IL_0059:  ldloca.s   V_1
    IL_005b:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint32>::get_HasValue()
    IL_0060:  ldloca.s   V_2
    IL_0062:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint32>::get_HasValue()
    IL_0067:  and
    IL_0068:  and
    IL_0069:  stloc.0
    IL_006a:  ldloc.0
    IL_006b:  brfalse.s  IL_0075

    IL_006d:  nop
    IL_006e:  call       void [mscorlib]System.Console::WriteLine()
    IL_0073:  nop
    IL_0074:  nop
    IL_0075:  ret
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
    // Code size       43 (0x2b)
    .maxstack  3
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
    IL_0013:  ceq
    IL_0015:  ldloca.s   V_0
    IL_0017:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001c:  ldloca.s   V_1
    IL_001e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0023:  ceq
    IL_0025:  and
    IL_0026:  stloc.2
    IL_0027:  br.s       IL_0029

    IL_0029:  ldloc.2
    IL_002a:  ret
  } // end of method LiftedOperators::RetEq

  .method public hidebysig static bool  RetEqConv(valuetype [mscorlib]System.Nullable`1<int64> a,
                                                  valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       79 (0x4f)
    .maxstack  3
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
    IL_0035:  ceq
    IL_0037:  ldloca.s   V_0
    IL_0039:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_003e:  ldloca.s   V_1
    IL_0040:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0045:  ceq
    IL_0047:  and
    IL_0048:  stloc.s    V_4
    IL_004a:  br.s       IL_004c

    IL_004c:  ldloc.s    V_4
    IL_004e:  ret
  } // end of method LiftedOperators::RetEqConv

  .method public hidebysig static bool  RetEqConst(valuetype [mscorlib]System.Nullable`1<int64> a) cil managed
  {
    // Code size       30 (0x1e)
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
    IL_000f:  ceq
    IL_0011:  ldloca.s   V_0
    IL_0013:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0018:  and
    IL_0019:  stloc.2
    IL_001a:  br.s       IL_001c

    IL_001c:  ldloc.2
    IL_001d:  ret
  } // end of method LiftedOperators::RetEqConst

  .method public hidebysig static bool  RetIneqConst(valuetype [mscorlib]System.Nullable`1<int64> a) cil managed
  {
    // Code size       33 (0x21)
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
    IL_000f:  ceq
    IL_0011:  ldloca.s   V_0
    IL_0013:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0018:  and
    IL_0019:  ldc.i4.0
    IL_001a:  ceq
    IL_001c:  stloc.2
    IL_001d:  br.s       IL_001f

    IL_001f:  ldloc.2
    IL_0020:  ret
  } // end of method LiftedOperators::RetIneqConst

  .method public hidebysig static bool  RetLt(valuetype [mscorlib]System.Nullable`1<int32> a,
                                              valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  3
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
    IL_0013:  clt
    IL_0015:  ldloca.s   V_0
    IL_0017:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001c:  ldloca.s   V_1
    IL_001e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0023:  and
    IL_0024:  and
    IL_0025:  stloc.2
    IL_0026:  br.s       IL_0028

    IL_0028:  ldloc.2
    IL_0029:  ret
  } // end of method LiftedOperators::RetLt

  .method public hidebysig static bool  RetLtConst(valuetype [mscorlib]System.Nullable`1<int32> a) cil managed
  {
    // Code size       29 (0x1d)
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
    IL_000e:  clt
    IL_0010:  ldloca.s   V_0
    IL_0012:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0017:  and
    IL_0018:  stloc.2
    IL_0019:  br.s       IL_001b

    IL_001b:  ldloc.2
    IL_001c:  ret
  } // end of method LiftedOperators::RetLtConst

  .method public hidebysig static bool  RetLtConv(valuetype [mscorlib]System.Nullable`1<int64> a,
                                                  valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       78 (0x4e)
    .maxstack  3
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
    IL_0035:  clt
    IL_0037:  ldloca.s   V_0
    IL_0039:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_003e:  ldloca.s   V_1
    IL_0040:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0045:  and
    IL_0046:  and
    IL_0047:  stloc.s    V_4
    IL_0049:  br.s       IL_004b

    IL_004b:  ldloc.s    V_4
    IL_004d:  ret
  } // end of method LiftedOperators::RetLtConv

  .method public hidebysig static bool  RetNotLt(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                 valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  3
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
    IL_0013:  clt
    IL_0015:  ldloca.s   V_0
    IL_0017:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001c:  ldloca.s   V_1
    IL_001e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0023:  and
    IL_0024:  and
    IL_0025:  ldc.i4.0
    IL_0026:  ceq
    IL_0028:  stloc.2
    IL_0029:  br.s       IL_002b

    IL_002b:  ldloc.2
    IL_002c:  ret
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
