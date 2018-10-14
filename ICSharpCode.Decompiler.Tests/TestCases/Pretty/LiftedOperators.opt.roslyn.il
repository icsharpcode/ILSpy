



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
    // Code size       89 (0x59)
    .maxstack  3
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
    IL_0012:  ceq
    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001b:  ldloca.s   V_1
    IL_001d:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0022:  ceq
    IL_0024:  and
    IL_0025:  brfalse.s  IL_002c

    IL_0027:  call       void [mscorlib]System.Console::WriteLine()
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
    IL_0051:  brtrue.s   IL_0058

    IL_0053:  call       void [mscorlib]System.Console::WriteLine()
    IL_0058:  ret
  } // end of method LiftedOperators::BoolBasic

  .method public hidebysig static void  BoolComplex(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                    class [mscorlib]System.Func`1<bool> x) cil managed
  {
    // Code size       133 (0x85)
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
    IL_0011:  ceq
    IL_0013:  ldloca.s   V_0
    IL_0015:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001a:  and
    IL_001b:  brfalse.s  IL_0022

    IL_001d:  call       void [mscorlib]System.Console::WriteLine()
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
    IL_003d:  brtrue.s   IL_0044

    IL_003f:  call       void [mscorlib]System.Console::WriteLine()
    IL_0044:  ldarg.1
    IL_0045:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_004a:  ldarg.0
    IL_004b:  stloc.0
    IL_004c:  ldloca.s   V_0
    IL_004e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0053:  ceq
    IL_0055:  ldloca.s   V_0
    IL_0057:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_005c:  and
    IL_005d:  brfalse.s  IL_0064

    IL_005f:  call       void [mscorlib]System.Console::WriteLine()
    IL_0064:  ldarg.1
    IL_0065:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_006a:  ldarg.0
    IL_006b:  stloc.0
    IL_006c:  ldloca.s   V_0
    IL_006e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0073:  ceq
    IL_0075:  ldloca.s   V_0
    IL_0077:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_007c:  and
    IL_007d:  brtrue.s   IL_0084

    IL_007f:  call       void [mscorlib]System.Console::WriteLine()
    IL_0084:  ret
  } // end of method LiftedOperators::BoolComplex

  .method public hidebysig static void  BoolConst(valuetype [mscorlib]System.Nullable`1<bool> a) cil managed
  {
    // Code size       173 (0xad)
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
    IL_000c:  ceq
    IL_000e:  ldloca.s   V_0
    IL_0010:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0015:  and
    IL_0016:  brfalse.s  IL_001d

    IL_0018:  call       void [mscorlib]System.Console::WriteLine()
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
    IL_0033:  brtrue.s   IL_003a

    IL_0035:  call       void [mscorlib]System.Console::WriteLine()
    IL_003a:  ldarg.0
    IL_003b:  stloc.0
    IL_003c:  ldc.i4.0
    IL_003d:  stloc.1
    IL_003e:  ldloca.s   V_0
    IL_0040:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0045:  ldloc.1
    IL_0046:  ceq
    IL_0048:  ldloca.s   V_0
    IL_004a:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_004f:  and
    IL_0050:  brfalse.s  IL_0057

    IL_0052:  call       void [mscorlib]System.Console::WriteLine()
    IL_0057:  ldarg.0
    IL_0058:  stloc.0
    IL_0059:  ldc.i4.0
    IL_005a:  stloc.1
    IL_005b:  ldloca.s   V_0
    IL_005d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0062:  ldloc.1
    IL_0063:  ceq
    IL_0065:  ldloca.s   V_0
    IL_0067:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_006c:  and
    IL_006d:  brtrue.s   IL_0074

    IL_006f:  call       void [mscorlib]System.Console::WriteLine()
    IL_0074:  ldarg.0
    IL_0075:  stloc.0
    IL_0076:  ldloca.s   V_0
    IL_0078:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_007d:  brtrue.s   IL_0082

    IL_007f:  ldc.i4.1
    IL_0080:  br.s       IL_0089

    IL_0082:  ldloca.s   V_0
    IL_0084:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0089:  brfalse.s  IL_0090

    IL_008b:  call       void [mscorlib]System.Console::WriteLine()
    IL_0090:  ldarg.0
    IL_0091:  stloc.0
    IL_0092:  ldloca.s   V_0
    IL_0094:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0099:  brtrue.s   IL_009e

    IL_009b:  ldc.i4.0
    IL_009c:  br.s       IL_00a5

    IL_009e:  ldloca.s   V_0
    IL_00a0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00a5:  brfalse.s  IL_00ac

    IL_00a7:  call       void [mscorlib]System.Console::WriteLine()
    IL_00ac:  ret
  } // end of method LiftedOperators::BoolConst

  .method public hidebysig static void  BoolValueBasic(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                       valuetype [mscorlib]System.Nullable`1<bool> b) cil managed
  {
    // Code size       440 (0x1b8)
    .maxstack  3
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
    IL_0012:  ceq
    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001b:  ldloca.s   V_1
    IL_001d:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0022:  ceq
    IL_0024:  and
    IL_0025:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_002a:  ldarg.0
    IL_002b:  stloc.1
    IL_002c:  ldarg.1
    IL_002d:  stloc.0
    IL_002e:  ldloca.s   V_1
    IL_0030:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0035:  ldloca.s   V_0
    IL_0037:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_003c:  ceq
    IL_003e:  ldloca.s   V_1
    IL_0040:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0045:  ldloca.s   V_0
    IL_0047:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_004c:  ceq
    IL_004e:  and
    IL_004f:  ldc.i4.0
    IL_0050:  ceq
    IL_0052:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0057:  ldarg.0
    IL_0058:  stloc.0
    IL_0059:  ldarg.1
    IL_005a:  stloc.1
    IL_005b:  ldloca.s   V_0
    IL_005d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0062:  brtrue.s   IL_0079

    IL_0064:  ldloca.s   V_1
    IL_0066:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_006b:  brtrue.s   IL_0076

    IL_006d:  ldloca.s   V_0
    IL_006f:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0074:  brfalse.s  IL_0079

    IL_0076:  ldloc.0
    IL_0077:  br.s       IL_007a

    IL_0079:  ldloc.1
    IL_007a:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_007f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0084:  ldarg.0
    IL_0085:  stloc.1
    IL_0086:  ldarg.1
    IL_0087:  stloc.0
    IL_0088:  ldloca.s   V_1
    IL_008a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_008f:  brtrue.s   IL_00a6

    IL_0091:  ldloca.s   V_0
    IL_0093:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0098:  brtrue.s   IL_00a3

    IL_009a:  ldloca.s   V_1
    IL_009c:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00a1:  brfalse.s  IL_00a6

    IL_00a3:  ldloc.0
    IL_00a4:  br.s       IL_00a7

    IL_00a6:  ldloc.1
    IL_00a7:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_00ac:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00b1:  ldarg.0
    IL_00b2:  stloc.0
    IL_00b3:  ldarg.1
    IL_00b4:  stloc.1
    IL_00b5:  ldloca.s   V_0
    IL_00b7:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00bc:  ldloca.s   V_1
    IL_00be:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00c3:  and
    IL_00c4:  brtrue.s   IL_00d1

    IL_00c6:  ldloca.s   V_2
    IL_00c8:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_00ce:  ldloc.2
    IL_00cf:  br.s       IL_00e5

    IL_00d1:  ldloca.s   V_0
    IL_00d3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00d8:  ldloca.s   V_1
    IL_00da:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00df:  xor
    IL_00e0:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_00e5:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_00ea:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00ef:  ldarg.0
    IL_00f0:  stloc.1
    IL_00f1:  ldloca.s   V_1
    IL_00f3:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00f8:  brtrue.s   IL_00fd

    IL_00fa:  ldarg.1
    IL_00fb:  br.s       IL_00fe

    IL_00fd:  ldloc.1
    IL_00fe:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0103:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0108:  ldarg.0
    IL_0109:  stloc.1
    IL_010a:  ldloca.s   V_1
    IL_010c:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0111:  brtrue.s   IL_011e

    IL_0113:  ldloca.s   V_0
    IL_0115:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_011b:  ldloc.0
    IL_011c:  br.s       IL_012d

    IL_011e:  ldloca.s   V_1
    IL_0120:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0125:  ldc.i4.0
    IL_0126:  ceq
    IL_0128:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_012d:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0132:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0137:  ldarg.0
    IL_0138:  stloc.1
    IL_0139:  ldarg.1
    IL_013a:  stloc.0
    IL_013b:  ldloca.s   V_1
    IL_013d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0142:  brtrue.s   IL_0159

    IL_0144:  ldloca.s   V_0
    IL_0146:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_014b:  brtrue.s   IL_0156

    IL_014d:  ldloca.s   V_1
    IL_014f:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0154:  brfalse.s  IL_0159

    IL_0156:  ldloc.1
    IL_0157:  br.s       IL_015a

    IL_0159:  ldloc.0
    IL_015a:  starg.s    a
    IL_015c:  ldarg.0
    IL_015d:  stloc.0
    IL_015e:  ldarg.1
    IL_015f:  stloc.1
    IL_0160:  ldloca.s   V_0
    IL_0162:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0167:  brtrue.s   IL_017e

    IL_0169:  ldloca.s   V_1
    IL_016b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0170:  brtrue.s   IL_017b

    IL_0172:  ldloca.s   V_0
    IL_0174:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0179:  brfalse.s  IL_017e

    IL_017b:  ldloc.1
    IL_017c:  br.s       IL_017f

    IL_017e:  ldloc.0
    IL_017f:  starg.s    a
    IL_0181:  ldarg.0
    IL_0182:  stloc.1
    IL_0183:  ldarg.1
    IL_0184:  stloc.0
    IL_0185:  ldloca.s   V_1
    IL_0187:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_018c:  ldloca.s   V_0
    IL_018e:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0193:  and
    IL_0194:  brtrue.s   IL_01a1

    IL_0196:  ldloca.s   V_2
    IL_0198:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_019e:  ldloc.2
    IL_019f:  br.s       IL_01b5

    IL_01a1:  ldloca.s   V_1
    IL_01a3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01a8:  ldloca.s   V_0
    IL_01aa:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01af:  xor
    IL_01b0:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_01b5:  starg.s    a
    IL_01b7:  ret
  } // end of method LiftedOperators::BoolValueBasic

  .method public hidebysig static void  BoolValueComplex(valuetype [mscorlib]System.Nullable`1<bool> a,
                                                         class [mscorlib]System.Func`1<bool> x) cil managed
  {
    // Code size       510 (0x1fe)
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
    IL_0011:  ceq
    IL_0013:  ldloca.s   V_0
    IL_0015:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_001a:  and
    IL_001b:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0020:  ldarg.0
    IL_0021:  stloc.0
    IL_0022:  ldarg.1
    IL_0023:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0028:  stloc.1
    IL_0029:  ldloca.s   V_0
    IL_002b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0030:  ldloc.1
    IL_0031:  ceq
    IL_0033:  ldloca.s   V_0
    IL_0035:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_003a:  and
    IL_003b:  ldc.i4.0
    IL_003c:  ceq
    IL_003e:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0043:  ldarg.1
    IL_0044:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0049:  ldarg.0
    IL_004a:  stloc.0
    IL_004b:  ldloca.s   V_0
    IL_004d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0052:  ceq
    IL_0054:  ldloca.s   V_0
    IL_0056:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_005b:  and
    IL_005c:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0061:  ldarg.1
    IL_0062:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0067:  ldarg.0
    IL_0068:  stloc.0
    IL_0069:  ldloca.s   V_0
    IL_006b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0070:  ceq
    IL_0072:  ldloca.s   V_0
    IL_0074:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0079:  and
    IL_007a:  ldc.i4.0
    IL_007b:  ceq
    IL_007d:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0082:  ldarg.0
    IL_0083:  stloc.0
    IL_0084:  ldarg.1
    IL_0085:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_008a:  stloc.1
    IL_008b:  ldloca.s   V_0
    IL_008d:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0092:  brtrue.s   IL_009f

    IL_0094:  ldloca.s   V_2
    IL_0096:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_009c:  ldloc.2
    IL_009d:  br.s       IL_00ad

    IL_009f:  ldloca.s   V_0
    IL_00a1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00a6:  ldloc.1
    IL_00a7:  xor
    IL_00a8:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_00ad:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_00b2:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00b7:  ldarg.0
    IL_00b8:  stloc.0
    IL_00b9:  ldloca.s   V_0
    IL_00bb:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00c0:  brtrue.s   IL_00ca

    IL_00c2:  ldarg.1
    IL_00c3:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_00c8:  br.s       IL_00d1

    IL_00ca:  ldloca.s   V_0
    IL_00cc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00d1:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00d6:  ldarg.0
    IL_00d7:  stloc.0
    IL_00d8:  ldarg.1
    IL_00d9:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_00de:  stloc.1
    IL_00df:  ldloca.s   V_0
    IL_00e1:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_00e6:  brtrue.s   IL_00f3

    IL_00e8:  ldloca.s   V_2
    IL_00ea:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_00f0:  ldloc.2
    IL_00f1:  br.s       IL_0101

    IL_00f3:  ldloca.s   V_0
    IL_00f5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00fa:  ldloc.1
    IL_00fb:  xor
    IL_00fc:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0101:  starg.s    a
    IL_0103:  ldarg.1
    IL_0104:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0109:  ldarg.0
    IL_010a:  stloc.0
    IL_010b:  brtrue.s   IL_0115

    IL_010d:  ldc.i4.0
    IL_010e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0113:  br.s       IL_0116

    IL_0115:  ldloc.0
    IL_0116:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_011b:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0120:  ldarg.1
    IL_0121:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0126:  ldarg.0
    IL_0127:  stloc.0
    IL_0128:  brtrue.s   IL_012d

    IL_012a:  ldloc.0
    IL_012b:  br.s       IL_0133

    IL_012d:  ldc.i4.1
    IL_012e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0133:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_0138:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_013d:  ldarg.1
    IL_013e:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_0143:  stloc.1
    IL_0144:  ldarg.0
    IL_0145:  stloc.0
    IL_0146:  ldloca.s   V_0
    IL_0148:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_014d:  brtrue.s   IL_015a

    IL_014f:  ldloca.s   V_2
    IL_0151:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_0157:  ldloc.2
    IL_0158:  br.s       IL_0168

    IL_015a:  ldloc.1
    IL_015b:  ldloca.s   V_0
    IL_015d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0162:  xor
    IL_0163:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0168:  box        valuetype [mscorlib]System.Nullable`1<bool>
    IL_016d:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0172:  ldc.i4.0
    IL_0173:  newarr     valuetype [mscorlib]System.Nullable`1<bool>
    IL_0178:  ldc.i4.0
    IL_0179:  ldelema    valuetype [mscorlib]System.Nullable`1<bool>
    IL_017e:  dup
    IL_017f:  ldobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_0184:  stloc.0
    IL_0185:  ldarg.1
    IL_0186:  callvirt   instance !0 class [mscorlib]System.Func`1<bool>::Invoke()
    IL_018b:  stloc.1
    IL_018c:  ldloca.s   V_0
    IL_018e:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0193:  brtrue.s   IL_01a0

    IL_0195:  ldloca.s   V_2
    IL_0197:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_019d:  ldloc.2
    IL_019e:  br.s       IL_01ae

    IL_01a0:  ldloca.s   V_0
    IL_01a2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01a7:  ldloc.1
    IL_01a8:  xor
    IL_01a9:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_01ae:  stobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_01b3:  ldc.i4.0
    IL_01b4:  newarr     valuetype [mscorlib]System.Nullable`1<bool>
    IL_01b9:  ldc.i4.0
    IL_01ba:  ldelema    valuetype [mscorlib]System.Nullable`1<bool>
    IL_01bf:  dup
    IL_01c0:  ldobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_01c5:  stloc.0
    IL_01c6:  ldarg.0
    IL_01c7:  stloc.2
    IL_01c8:  ldloca.s   V_0
    IL_01ca:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_01cf:  ldloca.s   V_2
    IL_01d1:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_01d6:  and
    IL_01d7:  brtrue.s   IL_01e4

    IL_01d9:  ldloca.s   V_3
    IL_01db:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_01e1:  ldloc.3
    IL_01e2:  br.s       IL_01f8

    IL_01e4:  ldloca.s   V_0
    IL_01e6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01eb:  ldloca.s   V_2
    IL_01ed:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_01f2:  xor
    IL_01f3:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_01f8:  stobj      valuetype [mscorlib]System.Nullable`1<bool>
    IL_01fd:  ret
  } // end of method LiftedOperators::BoolValueComplex

  .method public hidebysig static void  BoolValueConst(valuetype [mscorlib]System.Nullable`1<bool> a) cil managed
  {
    // Code size       167 (0xa7)
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
    IL_000c:  ceq
    IL_000e:  ldloca.s   V_0
    IL_0010:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0015:  and
    IL_0016:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_001b:  ldarg.0
    IL_001c:  stloc.0
    IL_001d:  ldc.i4.1
    IL_001e:  stloc.1
    IL_001f:  ldloca.s   V_0
    IL_0021:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0026:  ldloc.1
    IL_0027:  ceq
    IL_0029:  ldloca.s   V_0
    IL_002b:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0030:  and
    IL_0031:  ldc.i4.0
    IL_0032:  ceq
    IL_0034:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0039:  ldarg.0
    IL_003a:  stloc.0
    IL_003b:  ldc.i4.0
    IL_003c:  stloc.1
    IL_003d:  ldloca.s   V_0
    IL_003f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0044:  ldloc.1
    IL_0045:  ceq
    IL_0047:  ldloca.s   V_0
    IL_0049:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_004e:  and
    IL_004f:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0054:  ldarg.0
    IL_0055:  stloc.0
    IL_0056:  ldc.i4.0
    IL_0057:  stloc.1
    IL_0058:  ldloca.s   V_0
    IL_005a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_005f:  ldloc.1
    IL_0060:  ceq
    IL_0062:  ldloca.s   V_0
    IL_0064:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0069:  and
    IL_006a:  ldc.i4.0
    IL_006b:  ceq
    IL_006d:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0072:  ldarg.0
    IL_0073:  stloc.0
    IL_0074:  ldloca.s   V_0
    IL_0076:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_007b:  brtrue.s   IL_0080

    IL_007d:  ldc.i4.1
    IL_007e:  br.s       IL_0087

    IL_0080:  ldloca.s   V_0
    IL_0082:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0087:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_008c:  ldarg.0
    IL_008d:  stloc.0
    IL_008e:  ldloca.s   V_0
    IL_0090:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0095:  brtrue.s   IL_009a

    IL_0097:  ldc.i4.0
    IL_0098:  br.s       IL_00a1

    IL_009a:  ldloca.s   V_0
    IL_009c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_00a1:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00a6:  ret
  } // end of method LiftedOperators::BoolValueConst

  .method public hidebysig static void  IntBasic(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                 valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       356 (0x164)
    .maxstack  3
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
    IL_0012:  ceq
    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001b:  ldloca.s   V_1
    IL_001d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0022:  ceq
    IL_0024:  and
    IL_0025:  brfalse.s  IL_002c

    IL_0027:  call       void [mscorlib]System.Console::WriteLine()
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
    IL_0051:  brtrue.s   IL_0058

    IL_0053:  call       void [mscorlib]System.Console::WriteLine()
    IL_0058:  ldarg.0
    IL_0059:  stloc.0
    IL_005a:  ldarg.1
    IL_005b:  stloc.1
    IL_005c:  ldloca.s   V_0
    IL_005e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0063:  ldloca.s   V_1
    IL_0065:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_006a:  cgt
    IL_006c:  ldloca.s   V_0
    IL_006e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0073:  ldloca.s   V_1
    IL_0075:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_007a:  and
    IL_007b:  and
    IL_007c:  brfalse.s  IL_0083

    IL_007e:  call       void [mscorlib]System.Console::WriteLine()
    IL_0083:  ldarg.0
    IL_0084:  stloc.1
    IL_0085:  ldarg.1
    IL_0086:  stloc.0
    IL_0087:  ldloca.s   V_1
    IL_0089:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_008e:  ldloca.s   V_0
    IL_0090:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0095:  clt
    IL_0097:  ldloca.s   V_1
    IL_0099:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_009e:  ldloca.s   V_0
    IL_00a0:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00a5:  and
    IL_00a6:  and
    IL_00a7:  brfalse.s  IL_00ae

    IL_00a9:  call       void [mscorlib]System.Console::WriteLine()
    IL_00ae:  ldarg.0
    IL_00af:  stloc.0
    IL_00b0:  ldarg.1
    IL_00b1:  stloc.1
    IL_00b2:  ldloca.s   V_0
    IL_00b4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00b9:  ldloca.s   V_1
    IL_00bb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00c0:  clt
    IL_00c2:  ldc.i4.0
    IL_00c3:  ceq
    IL_00c5:  ldloca.s   V_0
    IL_00c7:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00cc:  ldloca.s   V_1
    IL_00ce:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00d3:  and
    IL_00d4:  and
    IL_00d5:  brfalse.s  IL_00dc

    IL_00d7:  call       void [mscorlib]System.Console::WriteLine()
    IL_00dc:  ldarg.0
    IL_00dd:  stloc.1
    IL_00de:  ldarg.1
    IL_00df:  stloc.0
    IL_00e0:  ldloca.s   V_1
    IL_00e2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00e7:  ldloca.s   V_0
    IL_00e9:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00ee:  cgt
    IL_00f0:  ldc.i4.0
    IL_00f1:  ceq
    IL_00f3:  ldloca.s   V_1
    IL_00f5:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00fa:  ldloca.s   V_0
    IL_00fc:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0101:  and
    IL_0102:  and
    IL_0103:  brfalse.s  IL_010a

    IL_0105:  call       void [mscorlib]System.Console::WriteLine()
    IL_010a:  ldarg.0
    IL_010b:  stloc.0
    IL_010c:  ldarg.1
    IL_010d:  stloc.1
    IL_010e:  ldloca.s   V_0
    IL_0110:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0115:  ldloca.s   V_1
    IL_0117:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_011c:  cgt
    IL_011e:  ldloca.s   V_0
    IL_0120:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0125:  ldloca.s   V_1
    IL_0127:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_012c:  and
    IL_012d:  and
    IL_012e:  brtrue.s   IL_0135

    IL_0130:  call       void [mscorlib]System.Console::WriteLine()
    IL_0135:  ldarg.0
    IL_0136:  stloc.1
    IL_0137:  ldarg.1
    IL_0138:  stloc.0
    IL_0139:  ldloca.s   V_1
    IL_013b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0140:  ldloca.s   V_0
    IL_0142:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0147:  cgt
    IL_0149:  ldc.i4.0
    IL_014a:  ceq
    IL_014c:  ldloca.s   V_1
    IL_014e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0153:  ldloca.s   V_0
    IL_0155:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_015a:  and
    IL_015b:  and
    IL_015c:  brtrue.s   IL_0163

    IL_015e:  call       void [mscorlib]System.Console::WriteLine()
    IL_0163:  ret
  } // end of method LiftedOperators::IntBasic

  .method public hidebysig static void  IntComplex(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                   class [mscorlib]System.Func`1<int32> x) cil managed
  {
    // Code size       270 (0x10e)
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
    IL_0011:  ceq
    IL_0013:  ldloca.s   V_0
    IL_0015:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001a:  and
    IL_001b:  brfalse.s  IL_0022

    IL_001d:  call       void [mscorlib]System.Console::WriteLine()
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
    IL_003d:  brtrue.s   IL_0044

    IL_003f:  call       void [mscorlib]System.Console::WriteLine()
    IL_0044:  ldarg.0
    IL_0045:  stloc.0
    IL_0046:  ldarg.1
    IL_0047:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_004c:  stloc.1
    IL_004d:  ldloca.s   V_0
    IL_004f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0054:  ldloc.1
    IL_0055:  cgt
    IL_0057:  ldloca.s   V_0
    IL_0059:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_005e:  and
    IL_005f:  brfalse.s  IL_0066

    IL_0061:  call       void [mscorlib]System.Console::WriteLine()
    IL_0066:  ldarg.1
    IL_0067:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_006c:  ldarg.0
    IL_006d:  stloc.0
    IL_006e:  ldloca.s   V_0
    IL_0070:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0075:  ceq
    IL_0077:  ldloca.s   V_0
    IL_0079:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_007e:  and
    IL_007f:  brfalse.s  IL_0086

    IL_0081:  call       void [mscorlib]System.Console::WriteLine()
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
    IL_009f:  brtrue.s   IL_00a6

    IL_00a1:  call       void [mscorlib]System.Console::WriteLine()
    IL_00a6:  ldarg.1
    IL_00a7:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00ac:  ldarg.0
    IL_00ad:  stloc.0
    IL_00ae:  ldloca.s   V_0
    IL_00b0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00b5:  cgt
    IL_00b7:  ldloca.s   V_0
    IL_00b9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00be:  and
    IL_00bf:  brfalse.s  IL_00c6

    IL_00c1:  call       void [mscorlib]System.Console::WriteLine()
    IL_00c6:  ldarg.0
    IL_00c7:  stloc.0
    IL_00c8:  ldarg.1
    IL_00c9:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00ce:  stloc.1
    IL_00cf:  ldloca.s   V_0
    IL_00d1:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00d6:  ldloc.1
    IL_00d7:  cgt
    IL_00d9:  ldloca.s   V_0
    IL_00db:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00e0:  and
    IL_00e1:  brtrue.s   IL_00e8

    IL_00e3:  call       void [mscorlib]System.Console::WriteLine()
    IL_00e8:  ldarg.0
    IL_00e9:  stloc.0
    IL_00ea:  ldarg.1
    IL_00eb:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00f0:  stloc.1
    IL_00f1:  ldloca.s   V_0
    IL_00f3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00f8:  ldloc.1
    IL_00f9:  cgt
    IL_00fb:  ldc.i4.0
    IL_00fc:  ceq
    IL_00fe:  ldloca.s   V_0
    IL_0100:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0105:  and
    IL_0106:  brtrue.s   IL_010d

    IL_0108:  call       void [mscorlib]System.Console::WriteLine()
    IL_010d:  ret
  } // end of method LiftedOperators::IntComplex

  .method public hidebysig static void  IntConst(valuetype [mscorlib]System.Nullable`1<int32> a) cil managed
  {
    // Code size       169 (0xa9)
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
    IL_000c:  ceq
    IL_000e:  ldloca.s   V_0
    IL_0010:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0015:  and
    IL_0016:  brfalse.s  IL_001d

    IL_0018:  call       void [mscorlib]System.Console::WriteLine()
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
    IL_0033:  brtrue.s   IL_003a

    IL_0035:  call       void [mscorlib]System.Console::WriteLine()
    IL_003a:  ldarg.0
    IL_003b:  stloc.0
    IL_003c:  ldc.i4.2
    IL_003d:  stloc.1
    IL_003e:  ldloca.s   V_0
    IL_0040:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0045:  ldloc.1
    IL_0046:  cgt
    IL_0048:  ldloca.s   V_0
    IL_004a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_004f:  and
    IL_0050:  brfalse.s  IL_0057

    IL_0052:  call       void [mscorlib]System.Console::WriteLine()
    IL_0057:  ldc.i4.2
    IL_0058:  ldarg.0
    IL_0059:  stloc.0
    IL_005a:  ldloca.s   V_0
    IL_005c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0061:  ceq
    IL_0063:  ldloca.s   V_0
    IL_0065:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_006a:  and
    IL_006b:  brfalse.s  IL_0072

    IL_006d:  call       void [mscorlib]System.Console::WriteLine()
    IL_0072:  ldc.i4.2
    IL_0073:  ldarg.0
    IL_0074:  stloc.0
    IL_0075:  ldloca.s   V_0
    IL_0077:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_007c:  ceq
    IL_007e:  ldloca.s   V_0
    IL_0080:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0085:  and
    IL_0086:  brtrue.s   IL_008d

    IL_0088:  call       void [mscorlib]System.Console::WriteLine()
    IL_008d:  ldc.i4.2
    IL_008e:  ldarg.0
    IL_008f:  stloc.0
    IL_0090:  ldloca.s   V_0
    IL_0092:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0097:  cgt
    IL_0099:  ldloca.s   V_0
    IL_009b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00a0:  and
    IL_00a1:  brfalse.s  IL_00a8

    IL_00a3:  call       void [mscorlib]System.Console::WriteLine()
    IL_00a8:  ret
  } // end of method LiftedOperators::IntConst

  .method public hidebysig static void  IntValueBasic(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                      valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       1605 (0x645)
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
    IL_0012:  ceq
    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001b:  ldloca.s   V_1
    IL_001d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0022:  ceq
    IL_0024:  and
    IL_0025:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_002a:  ldarg.0
    IL_002b:  stloc.1
    IL_002c:  ldarg.1
    IL_002d:  stloc.0
    IL_002e:  ldloca.s   V_1
    IL_0030:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0035:  ldloca.s   V_0
    IL_0037:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_003c:  ceq
    IL_003e:  ldloca.s   V_1
    IL_0040:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0045:  ldloca.s   V_0
    IL_0047:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_004c:  ceq
    IL_004e:  and
    IL_004f:  ldc.i4.0
    IL_0050:  ceq
    IL_0052:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0057:  ldarg.0
    IL_0058:  stloc.0
    IL_0059:  ldarg.1
    IL_005a:  stloc.1
    IL_005b:  ldloca.s   V_0
    IL_005d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0062:  ldloca.s   V_1
    IL_0064:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0069:  cgt
    IL_006b:  ldloca.s   V_0
    IL_006d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0072:  ldloca.s   V_1
    IL_0074:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0079:  and
    IL_007a:  and
    IL_007b:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0080:  ldarg.0
    IL_0081:  stloc.1
    IL_0082:  ldarg.1
    IL_0083:  stloc.0
    IL_0084:  ldloca.s   V_1
    IL_0086:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_008b:  ldloca.s   V_0
    IL_008d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0092:  cgt
    IL_0094:  ldloca.s   V_1
    IL_0096:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_009b:  ldloca.s   V_0
    IL_009d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00a2:  and
    IL_00a3:  and
    IL_00a4:  ldc.i4.0
    IL_00a5:  ceq
    IL_00a7:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00ac:  ldarg.0
    IL_00ad:  stloc.0
    IL_00ae:  ldarg.1
    IL_00af:  stloc.1
    IL_00b0:  ldloca.s   V_0
    IL_00b2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00b7:  ldloca.s   V_1
    IL_00b9:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00be:  clt
    IL_00c0:  ldc.i4.0
    IL_00c1:  ceq
    IL_00c3:  ldloca.s   V_0
    IL_00c5:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ca:  ldloca.s   V_1
    IL_00cc:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00d1:  and
    IL_00d2:  and
    IL_00d3:  ldc.i4.0
    IL_00d4:  ceq
    IL_00d6:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00db:  ldarg.0
    IL_00dc:  stloc.1
    IL_00dd:  ldarg.1
    IL_00de:  stloc.0
    IL_00df:  ldloca.s   V_1
    IL_00e1:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00e6:  ldloca.s   V_0
    IL_00e8:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ed:  and
    IL_00ee:  brtrue.s   IL_00fb

    IL_00f0:  ldloca.s   V_2
    IL_00f2:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00f8:  ldloc.2
    IL_00f9:  br.s       IL_010f

    IL_00fb:  ldloca.s   V_1
    IL_00fd:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0102:  ldloca.s   V_0
    IL_0104:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0109:  add
    IL_010a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_010f:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0114:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0119:  ldarg.0
    IL_011a:  stloc.0
    IL_011b:  ldarg.1
    IL_011c:  stloc.1
    IL_011d:  ldloca.s   V_0
    IL_011f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0124:  ldloca.s   V_1
    IL_0126:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_012b:  and
    IL_012c:  brtrue.s   IL_0139

    IL_012e:  ldloca.s   V_2
    IL_0130:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0136:  ldloc.2
    IL_0137:  br.s       IL_014d

    IL_0139:  ldloca.s   V_0
    IL_013b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0140:  ldloca.s   V_1
    IL_0142:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0147:  sub
    IL_0148:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_014d:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0152:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0157:  ldarg.0
    IL_0158:  stloc.1
    IL_0159:  ldarg.1
    IL_015a:  stloc.0
    IL_015b:  ldloca.s   V_1
    IL_015d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0162:  ldloca.s   V_0
    IL_0164:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0169:  and
    IL_016a:  brtrue.s   IL_0177

    IL_016c:  ldloca.s   V_2
    IL_016e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0174:  ldloc.2
    IL_0175:  br.s       IL_018b

    IL_0177:  ldloca.s   V_1
    IL_0179:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_017e:  ldloca.s   V_0
    IL_0180:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0185:  mul
    IL_0186:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_018b:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0190:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0195:  ldarg.0
    IL_0196:  stloc.0
    IL_0197:  ldarg.1
    IL_0198:  stloc.1
    IL_0199:  ldloca.s   V_0
    IL_019b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01a0:  ldloca.s   V_1
    IL_01a2:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01a7:  and
    IL_01a8:  brtrue.s   IL_01b5

    IL_01aa:  ldloca.s   V_2
    IL_01ac:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01b2:  ldloc.2
    IL_01b3:  br.s       IL_01c9

    IL_01b5:  ldloca.s   V_0
    IL_01b7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01bc:  ldloca.s   V_1
    IL_01be:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01c3:  div
    IL_01c4:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01c9:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01ce:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01d3:  ldarg.0
    IL_01d4:  stloc.1
    IL_01d5:  ldarg.1
    IL_01d6:  stloc.0
    IL_01d7:  ldloca.s   V_1
    IL_01d9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01de:  ldloca.s   V_0
    IL_01e0:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01e5:  and
    IL_01e6:  brtrue.s   IL_01f3

    IL_01e8:  ldloca.s   V_2
    IL_01ea:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01f0:  ldloc.2
    IL_01f1:  br.s       IL_0207

    IL_01f3:  ldloca.s   V_1
    IL_01f5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01fa:  ldloca.s   V_0
    IL_01fc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0201:  rem
    IL_0202:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0207:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_020c:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0211:  ldarg.0
    IL_0212:  stloc.0
    IL_0213:  ldarg.1
    IL_0214:  stloc.1
    IL_0215:  ldloca.s   V_0
    IL_0217:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_021c:  ldloca.s   V_1
    IL_021e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0223:  and
    IL_0224:  brtrue.s   IL_0231

    IL_0226:  ldloca.s   V_2
    IL_0228:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_022e:  ldloc.2
    IL_022f:  br.s       IL_0245

    IL_0231:  ldloca.s   V_0
    IL_0233:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0238:  ldloca.s   V_1
    IL_023a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_023f:  and
    IL_0240:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0245:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_024a:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_024f:  ldarg.0
    IL_0250:  stloc.1
    IL_0251:  ldarg.1
    IL_0252:  stloc.0
    IL_0253:  ldloca.s   V_1
    IL_0255:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_025a:  ldloca.s   V_0
    IL_025c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0261:  and
    IL_0262:  brtrue.s   IL_026f

    IL_0264:  ldloca.s   V_2
    IL_0266:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_026c:  ldloc.2
    IL_026d:  br.s       IL_0283

    IL_026f:  ldloca.s   V_1
    IL_0271:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0276:  ldloca.s   V_0
    IL_0278:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_027d:  or
    IL_027e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0283:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0288:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_028d:  ldarg.0
    IL_028e:  stloc.0
    IL_028f:  ldarg.1
    IL_0290:  stloc.1
    IL_0291:  ldloca.s   V_0
    IL_0293:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0298:  ldloca.s   V_1
    IL_029a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_029f:  and
    IL_02a0:  brtrue.s   IL_02ad

    IL_02a2:  ldloca.s   V_2
    IL_02a4:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02aa:  ldloc.2
    IL_02ab:  br.s       IL_02c1

    IL_02ad:  ldloca.s   V_0
    IL_02af:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02b4:  ldloca.s   V_1
    IL_02b6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02bb:  xor
    IL_02bc:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02c1:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_02c6:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02cb:  ldarg.0
    IL_02cc:  stloc.1
    IL_02cd:  ldarg.1
    IL_02ce:  stloc.0
    IL_02cf:  ldloca.s   V_1
    IL_02d1:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02d6:  ldloca.s   V_0
    IL_02d8:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02dd:  and
    IL_02de:  brtrue.s   IL_02eb

    IL_02e0:  ldloca.s   V_2
    IL_02e2:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02e8:  ldloc.2
    IL_02e9:  br.s       IL_0302

    IL_02eb:  ldloca.s   V_1
    IL_02ed:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02f2:  ldloca.s   V_0
    IL_02f4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02f9:  ldc.i4.s   31
    IL_02fb:  and
    IL_02fc:  shl
    IL_02fd:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0302:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0307:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_030c:  ldarg.0
    IL_030d:  stloc.0
    IL_030e:  ldarg.1
    IL_030f:  stloc.1
    IL_0310:  ldloca.s   V_0
    IL_0312:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0317:  ldloca.s   V_1
    IL_0319:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_031e:  and
    IL_031f:  brtrue.s   IL_032c

    IL_0321:  ldloca.s   V_2
    IL_0323:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0329:  ldloc.2
    IL_032a:  br.s       IL_0343

    IL_032c:  ldloca.s   V_0
    IL_032e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0333:  ldloca.s   V_1
    IL_0335:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_033a:  ldc.i4.s   31
    IL_033c:  and
    IL_033d:  shr
    IL_033e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0343:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0348:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_034d:  ldarg.0
    IL_034e:  stloc.1
    IL_034f:  ldloca.s   V_1
    IL_0351:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0356:  brtrue.s   IL_035b

    IL_0358:  ldarg.1
    IL_0359:  br.s       IL_035c

    IL_035b:  ldloc.1
    IL_035c:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0361:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0366:  ldarg.0
    IL_0367:  stloc.1
    IL_0368:  ldloca.s   V_1
    IL_036a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_036f:  brtrue.s   IL_037c

    IL_0371:  ldloca.s   V_0
    IL_0373:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0379:  ldloc.0
    IL_037a:  br.s       IL_0389

    IL_037c:  ldloca.s   V_1
    IL_037e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0383:  neg
    IL_0384:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0389:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_038e:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0393:  ldarg.0
    IL_0394:  stloc.1
    IL_0395:  ldloca.s   V_1
    IL_0397:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_039c:  brtrue.s   IL_03a9

    IL_039e:  ldloca.s   V_0
    IL_03a0:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03a6:  ldloc.0
    IL_03a7:  br.s       IL_03b6

    IL_03a9:  ldloca.s   V_1
    IL_03ab:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03b0:  not
    IL_03b1:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03b6:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_03bb:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_03c0:  ldarg.0
    IL_03c1:  stloc.1
    IL_03c2:  ldloca.s   V_1
    IL_03c4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03c9:  brtrue.s   IL_03d6

    IL_03cb:  ldloca.s   V_0
    IL_03cd:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03d3:  ldloc.0
    IL_03d4:  br.s       IL_03e4

    IL_03d6:  ldloca.s   V_1
    IL_03d8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03dd:  ldc.i4.1
    IL_03de:  add
    IL_03df:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03e4:  dup
    IL_03e5:  starg.s    a
    IL_03e7:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_03ec:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_03f1:  ldarg.0
    IL_03f2:  stloc.1
    IL_03f3:  ldloca.s   V_1
    IL_03f5:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03fa:  brtrue.s   IL_0407

    IL_03fc:  ldloca.s   V_0
    IL_03fe:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0404:  ldloc.0
    IL_0405:  br.s       IL_0415

    IL_0407:  ldloca.s   V_1
    IL_0409:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_040e:  ldc.i4.1
    IL_040f:  sub
    IL_0410:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0415:  dup
    IL_0416:  starg.s    a
    IL_0418:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_041d:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0422:  ldarg.0
    IL_0423:  stloc.1
    IL_0424:  ldarg.1
    IL_0425:  stloc.0
    IL_0426:  ldloca.s   V_1
    IL_0428:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_042d:  ldloca.s   V_0
    IL_042f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0434:  and
    IL_0435:  brtrue.s   IL_0442

    IL_0437:  ldloca.s   V_2
    IL_0439:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_043f:  ldloc.2
    IL_0440:  br.s       IL_0456

    IL_0442:  ldloca.s   V_1
    IL_0444:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0449:  ldloca.s   V_0
    IL_044b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0450:  add
    IL_0451:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0456:  starg.s    a
    IL_0458:  ldarg.0
    IL_0459:  stloc.0
    IL_045a:  ldarg.1
    IL_045b:  stloc.1
    IL_045c:  ldloca.s   V_0
    IL_045e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0463:  ldloca.s   V_1
    IL_0465:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_046a:  and
    IL_046b:  brtrue.s   IL_0478

    IL_046d:  ldloca.s   V_2
    IL_046f:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0475:  ldloc.2
    IL_0476:  br.s       IL_048c

    IL_0478:  ldloca.s   V_0
    IL_047a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_047f:  ldloca.s   V_1
    IL_0481:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0486:  sub
    IL_0487:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_048c:  starg.s    a
    IL_048e:  ldarg.0
    IL_048f:  stloc.1
    IL_0490:  ldarg.1
    IL_0491:  stloc.0
    IL_0492:  ldloca.s   V_1
    IL_0494:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0499:  ldloca.s   V_0
    IL_049b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04a0:  and
    IL_04a1:  brtrue.s   IL_04ae

    IL_04a3:  ldloca.s   V_2
    IL_04a5:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04ab:  ldloc.2
    IL_04ac:  br.s       IL_04c2

    IL_04ae:  ldloca.s   V_1
    IL_04b0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04b5:  ldloca.s   V_0
    IL_04b7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04bc:  mul
    IL_04bd:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04c2:  starg.s    a
    IL_04c4:  ldarg.0
    IL_04c5:  stloc.0
    IL_04c6:  ldarg.1
    IL_04c7:  stloc.1
    IL_04c8:  ldloca.s   V_0
    IL_04ca:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04cf:  ldloca.s   V_1
    IL_04d1:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04d6:  and
    IL_04d7:  brtrue.s   IL_04e4

    IL_04d9:  ldloca.s   V_2
    IL_04db:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04e1:  ldloc.2
    IL_04e2:  br.s       IL_04f8

    IL_04e4:  ldloca.s   V_0
    IL_04e6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04eb:  ldloca.s   V_1
    IL_04ed:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04f2:  div
    IL_04f3:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04f8:  starg.s    a
    IL_04fa:  ldarg.0
    IL_04fb:  stloc.1
    IL_04fc:  ldarg.1
    IL_04fd:  stloc.0
    IL_04fe:  ldloca.s   V_1
    IL_0500:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0505:  ldloca.s   V_0
    IL_0507:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_050c:  and
    IL_050d:  brtrue.s   IL_051a

    IL_050f:  ldloca.s   V_2
    IL_0511:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0517:  ldloc.2
    IL_0518:  br.s       IL_052e

    IL_051a:  ldloca.s   V_1
    IL_051c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0521:  ldloca.s   V_0
    IL_0523:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0528:  rem
    IL_0529:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_052e:  starg.s    a
    IL_0530:  ldarg.0
    IL_0531:  stloc.0
    IL_0532:  ldarg.1
    IL_0533:  stloc.1
    IL_0534:  ldloca.s   V_0
    IL_0536:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_053b:  ldloca.s   V_1
    IL_053d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0542:  and
    IL_0543:  brtrue.s   IL_0550

    IL_0545:  ldloca.s   V_2
    IL_0547:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_054d:  ldloc.2
    IL_054e:  br.s       IL_0564

    IL_0550:  ldloca.s   V_0
    IL_0552:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0557:  ldloca.s   V_1
    IL_0559:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_055e:  and
    IL_055f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0564:  starg.s    a
    IL_0566:  ldarg.0
    IL_0567:  stloc.1
    IL_0568:  ldarg.1
    IL_0569:  stloc.0
    IL_056a:  ldloca.s   V_1
    IL_056c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0571:  ldloca.s   V_0
    IL_0573:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0578:  and
    IL_0579:  brtrue.s   IL_0586

    IL_057b:  ldloca.s   V_2
    IL_057d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0583:  ldloc.2
    IL_0584:  br.s       IL_059a

    IL_0586:  ldloca.s   V_1
    IL_0588:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_058d:  ldloca.s   V_0
    IL_058f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0594:  or
    IL_0595:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_059a:  starg.s    a
    IL_059c:  ldarg.0
    IL_059d:  stloc.0
    IL_059e:  ldarg.1
    IL_059f:  stloc.1
    IL_05a0:  ldloca.s   V_0
    IL_05a2:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_05a7:  ldloca.s   V_1
    IL_05a9:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_05ae:  and
    IL_05af:  brtrue.s   IL_05bc

    IL_05b1:  ldloca.s   V_2
    IL_05b3:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_05b9:  ldloc.2
    IL_05ba:  br.s       IL_05d0

    IL_05bc:  ldloca.s   V_0
    IL_05be:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_05c3:  ldloca.s   V_1
    IL_05c5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_05ca:  xor
    IL_05cb:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_05d0:  starg.s    a
    IL_05d2:  ldarg.0
    IL_05d3:  stloc.1
    IL_05d4:  ldarg.1
    IL_05d5:  stloc.0
    IL_05d6:  ldloca.s   V_1
    IL_05d8:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_05dd:  ldloca.s   V_0
    IL_05df:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_05e4:  and
    IL_05e5:  brtrue.s   IL_05f2

    IL_05e7:  ldloca.s   V_2
    IL_05e9:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_05ef:  ldloc.2
    IL_05f0:  br.s       IL_0609

    IL_05f2:  ldloca.s   V_1
    IL_05f4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_05f9:  ldloca.s   V_0
    IL_05fb:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0600:  ldc.i4.s   31
    IL_0602:  and
    IL_0603:  shl
    IL_0604:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0609:  starg.s    a
    IL_060b:  ldarg.0
    IL_060c:  stloc.0
    IL_060d:  ldarg.1
    IL_060e:  stloc.1
    IL_060f:  ldloca.s   V_0
    IL_0611:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0616:  ldloca.s   V_1
    IL_0618:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_061d:  and
    IL_061e:  brtrue.s   IL_062b

    IL_0620:  ldloca.s   V_2
    IL_0622:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0628:  ldloc.2
    IL_0629:  br.s       IL_0642

    IL_062b:  ldloca.s   V_0
    IL_062d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0632:  ldloca.s   V_1
    IL_0634:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0639:  ldc.i4.s   31
    IL_063b:  and
    IL_063c:  shr
    IL_063d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0642:  starg.s    a
    IL_0644:  ret
  } // end of method LiftedOperators::IntValueBasic

  .method public hidebysig static void  IntValueComplex(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                        class [mscorlib]System.Func`1<int32> x) cil managed
  {
    // Code size       1334 (0x536)
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
    IL_0011:  ceq
    IL_0013:  ldloca.s   V_0
    IL_0015:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001a:  and
    IL_001b:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0020:  ldarg.0
    IL_0021:  stloc.0
    IL_0022:  ldarg.1
    IL_0023:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0028:  stloc.1
    IL_0029:  ldloca.s   V_0
    IL_002b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0030:  ldloc.1
    IL_0031:  ceq
    IL_0033:  ldloca.s   V_0
    IL_0035:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_003a:  and
    IL_003b:  ldc.i4.0
    IL_003c:  ceq
    IL_003e:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0043:  ldarg.0
    IL_0044:  stloc.0
    IL_0045:  ldarg.1
    IL_0046:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_004b:  stloc.1
    IL_004c:  ldloca.s   V_0
    IL_004e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0053:  ldloc.1
    IL_0054:  cgt
    IL_0056:  ldloca.s   V_0
    IL_0058:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_005d:  and
    IL_005e:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0063:  ldarg.1
    IL_0064:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0069:  ldarg.0
    IL_006a:  stloc.0
    IL_006b:  ldloca.s   V_0
    IL_006d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0072:  ceq
    IL_0074:  ldloca.s   V_0
    IL_0076:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_007b:  and
    IL_007c:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0081:  ldarg.1
    IL_0082:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0087:  ldarg.0
    IL_0088:  stloc.0
    IL_0089:  ldloca.s   V_0
    IL_008b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0090:  ceq
    IL_0092:  ldloca.s   V_0
    IL_0094:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0099:  and
    IL_009a:  ldc.i4.0
    IL_009b:  ceq
    IL_009d:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00a2:  ldarg.1
    IL_00a3:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00a8:  ldarg.0
    IL_00a9:  stloc.0
    IL_00aa:  ldloca.s   V_0
    IL_00ac:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00b1:  cgt
    IL_00b3:  ldloca.s   V_0
    IL_00b5:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ba:  and
    IL_00bb:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00c0:  ldarg.0
    IL_00c1:  stloc.0
    IL_00c2:  ldarg.1
    IL_00c3:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00c8:  stloc.1
    IL_00c9:  ldloca.s   V_0
    IL_00cb:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00d0:  brtrue.s   IL_00dd

    IL_00d2:  ldloca.s   V_2
    IL_00d4:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00da:  ldloc.2
    IL_00db:  br.s       IL_00eb

    IL_00dd:  ldloca.s   V_0
    IL_00df:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00e4:  ldloc.1
    IL_00e5:  add
    IL_00e6:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00eb:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_00f0:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00f5:  ldarg.0
    IL_00f6:  stloc.0
    IL_00f7:  ldarg.1
    IL_00f8:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_00fd:  stloc.1
    IL_00fe:  ldloca.s   V_0
    IL_0100:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0105:  brtrue.s   IL_0112

    IL_0107:  ldloca.s   V_2
    IL_0109:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_010f:  ldloc.2
    IL_0110:  br.s       IL_0120

    IL_0112:  ldloca.s   V_0
    IL_0114:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0119:  ldloc.1
    IL_011a:  sub
    IL_011b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0120:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0125:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_012a:  ldarg.0
    IL_012b:  stloc.0
    IL_012c:  ldarg.1
    IL_012d:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0132:  stloc.1
    IL_0133:  ldloca.s   V_0
    IL_0135:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_013a:  brtrue.s   IL_0147

    IL_013c:  ldloca.s   V_2
    IL_013e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0144:  ldloc.2
    IL_0145:  br.s       IL_0155

    IL_0147:  ldloca.s   V_0
    IL_0149:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_014e:  ldloc.1
    IL_014f:  mul
    IL_0150:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0155:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_015a:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_015f:  ldarg.0
    IL_0160:  stloc.0
    IL_0161:  ldarg.1
    IL_0162:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0167:  stloc.1
    IL_0168:  ldloca.s   V_0
    IL_016a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_016f:  brtrue.s   IL_017c

    IL_0171:  ldloca.s   V_2
    IL_0173:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0179:  ldloc.2
    IL_017a:  br.s       IL_018a

    IL_017c:  ldloca.s   V_0
    IL_017e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0183:  ldloc.1
    IL_0184:  div
    IL_0185:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_018a:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_018f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0194:  ldarg.0
    IL_0195:  stloc.0
    IL_0196:  ldarg.1
    IL_0197:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_019c:  stloc.1
    IL_019d:  ldloca.s   V_0
    IL_019f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01a4:  brtrue.s   IL_01b1

    IL_01a6:  ldloca.s   V_2
    IL_01a8:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01ae:  ldloc.2
    IL_01af:  br.s       IL_01bf

    IL_01b1:  ldloca.s   V_0
    IL_01b3:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01b8:  ldloc.1
    IL_01b9:  rem
    IL_01ba:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01bf:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01c4:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01c9:  ldarg.0
    IL_01ca:  stloc.0
    IL_01cb:  ldarg.1
    IL_01cc:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_01d1:  stloc.1
    IL_01d2:  ldloca.s   V_0
    IL_01d4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01d9:  brtrue.s   IL_01e6

    IL_01db:  ldloca.s   V_2
    IL_01dd:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01e3:  ldloc.2
    IL_01e4:  br.s       IL_01f4

    IL_01e6:  ldloca.s   V_0
    IL_01e8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01ed:  ldloc.1
    IL_01ee:  and
    IL_01ef:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01f4:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01f9:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01fe:  ldarg.0
    IL_01ff:  stloc.0
    IL_0200:  ldarg.1
    IL_0201:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0206:  stloc.1
    IL_0207:  ldloca.s   V_0
    IL_0209:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_020e:  brtrue.s   IL_021b

    IL_0210:  ldloca.s   V_2
    IL_0212:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0218:  ldloc.2
    IL_0219:  br.s       IL_0229

    IL_021b:  ldloca.s   V_0
    IL_021d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0222:  ldloc.1
    IL_0223:  or
    IL_0224:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0229:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_022e:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0233:  ldarg.0
    IL_0234:  stloc.0
    IL_0235:  ldarg.1
    IL_0236:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_023b:  stloc.1
    IL_023c:  ldloca.s   V_0
    IL_023e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0243:  brtrue.s   IL_0250

    IL_0245:  ldloca.s   V_2
    IL_0247:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_024d:  ldloc.2
    IL_024e:  br.s       IL_025e

    IL_0250:  ldloca.s   V_0
    IL_0252:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0257:  ldloc.1
    IL_0258:  xor
    IL_0259:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_025e:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0263:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0268:  ldarg.0
    IL_0269:  stloc.0
    IL_026a:  ldarg.1
    IL_026b:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0270:  stloc.1
    IL_0271:  ldloca.s   V_0
    IL_0273:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0278:  brtrue.s   IL_0285

    IL_027a:  ldloca.s   V_2
    IL_027c:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0282:  ldloc.2
    IL_0283:  br.s       IL_0296

    IL_0285:  ldloca.s   V_0
    IL_0287:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_028c:  ldloc.1
    IL_028d:  ldc.i4.s   31
    IL_028f:  and
    IL_0290:  shl
    IL_0291:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0296:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_029b:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02a0:  ldarg.0
    IL_02a1:  stloc.0
    IL_02a2:  ldarg.1
    IL_02a3:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_02a8:  stloc.1
    IL_02a9:  ldloca.s   V_0
    IL_02ab:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02b0:  brtrue.s   IL_02bd

    IL_02b2:  ldloca.s   V_2
    IL_02b4:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02ba:  ldloc.2
    IL_02bb:  br.s       IL_02ce

    IL_02bd:  ldloca.s   V_0
    IL_02bf:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02c4:  ldloc.1
    IL_02c5:  ldc.i4.s   31
    IL_02c7:  and
    IL_02c8:  shr
    IL_02c9:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02ce:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_02d3:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_02d8:  ldarg.0
    IL_02d9:  stloc.0
    IL_02da:  ldloca.s   V_0
    IL_02dc:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02e1:  brtrue.s   IL_02eb

    IL_02e3:  ldarg.1
    IL_02e4:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_02e9:  br.s       IL_02f2

    IL_02eb:  ldloca.s   V_0
    IL_02ed:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02f2:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_02f7:  ldarg.0
    IL_02f8:  stloc.0
    IL_02f9:  ldarg.1
    IL_02fa:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_02ff:  stloc.1
    IL_0300:  ldloca.s   V_0
    IL_0302:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0307:  brtrue.s   IL_0314

    IL_0309:  ldloca.s   V_2
    IL_030b:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0311:  ldloc.2
    IL_0312:  br.s       IL_0322

    IL_0314:  ldloca.s   V_0
    IL_0316:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_031b:  ldloc.1
    IL_031c:  add
    IL_031d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0322:  starg.s    a
    IL_0324:  ldarg.0
    IL_0325:  stloc.0
    IL_0326:  ldarg.1
    IL_0327:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_032c:  stloc.1
    IL_032d:  ldloca.s   V_0
    IL_032f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0334:  brtrue.s   IL_0341

    IL_0336:  ldloca.s   V_2
    IL_0338:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_033e:  ldloc.2
    IL_033f:  br.s       IL_034f

    IL_0341:  ldloca.s   V_0
    IL_0343:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0348:  ldloc.1
    IL_0349:  sub
    IL_034a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_034f:  starg.s    a
    IL_0351:  ldarg.0
    IL_0352:  stloc.0
    IL_0353:  ldarg.1
    IL_0354:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0359:  stloc.1
    IL_035a:  ldloca.s   V_0
    IL_035c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0361:  brtrue.s   IL_036e

    IL_0363:  ldloca.s   V_2
    IL_0365:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_036b:  ldloc.2
    IL_036c:  br.s       IL_037c

    IL_036e:  ldloca.s   V_0
    IL_0370:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0375:  ldloc.1
    IL_0376:  mul
    IL_0377:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_037c:  starg.s    a
    IL_037e:  ldarg.0
    IL_037f:  stloc.0
    IL_0380:  ldarg.1
    IL_0381:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0386:  stloc.1
    IL_0387:  ldloca.s   V_0
    IL_0389:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_038e:  brtrue.s   IL_039b

    IL_0390:  ldloca.s   V_2
    IL_0392:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0398:  ldloc.2
    IL_0399:  br.s       IL_03a9

    IL_039b:  ldloca.s   V_0
    IL_039d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03a2:  ldloc.1
    IL_03a3:  div
    IL_03a4:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03a9:  starg.s    a
    IL_03ab:  ldarg.0
    IL_03ac:  stloc.0
    IL_03ad:  ldarg.1
    IL_03ae:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_03b3:  stloc.1
    IL_03b4:  ldloca.s   V_0
    IL_03b6:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03bb:  brtrue.s   IL_03c8

    IL_03bd:  ldloca.s   V_2
    IL_03bf:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03c5:  ldloc.2
    IL_03c6:  br.s       IL_03d6

    IL_03c8:  ldloca.s   V_0
    IL_03ca:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03cf:  ldloc.1
    IL_03d0:  rem
    IL_03d1:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03d6:  starg.s    a
    IL_03d8:  ldarg.0
    IL_03d9:  stloc.0
    IL_03da:  ldarg.1
    IL_03db:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_03e0:  stloc.1
    IL_03e1:  ldloca.s   V_0
    IL_03e3:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03e8:  brtrue.s   IL_03f5

    IL_03ea:  ldloca.s   V_2
    IL_03ec:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03f2:  ldloc.2
    IL_03f3:  br.s       IL_0403

    IL_03f5:  ldloca.s   V_0
    IL_03f7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03fc:  ldloc.1
    IL_03fd:  and
    IL_03fe:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0403:  starg.s    a
    IL_0405:  ldarg.0
    IL_0406:  stloc.0
    IL_0407:  ldarg.1
    IL_0408:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_040d:  stloc.1
    IL_040e:  ldloca.s   V_0
    IL_0410:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0415:  brtrue.s   IL_0422

    IL_0417:  ldloca.s   V_2
    IL_0419:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_041f:  ldloc.2
    IL_0420:  br.s       IL_0430

    IL_0422:  ldloca.s   V_0
    IL_0424:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0429:  ldloc.1
    IL_042a:  or
    IL_042b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0430:  starg.s    a
    IL_0432:  ldarg.0
    IL_0433:  stloc.0
    IL_0434:  ldarg.1
    IL_0435:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_043a:  stloc.1
    IL_043b:  ldloca.s   V_0
    IL_043d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0442:  brtrue.s   IL_044f

    IL_0444:  ldloca.s   V_2
    IL_0446:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_044c:  ldloc.2
    IL_044d:  br.s       IL_045d

    IL_044f:  ldloca.s   V_0
    IL_0451:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0456:  ldloc.1
    IL_0457:  xor
    IL_0458:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_045d:  starg.s    a
    IL_045f:  ldarg.0
    IL_0460:  stloc.0
    IL_0461:  ldarg.1
    IL_0462:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0467:  stloc.1
    IL_0468:  ldloca.s   V_0
    IL_046a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_046f:  brtrue.s   IL_047c

    IL_0471:  ldloca.s   V_2
    IL_0473:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0479:  ldloc.2
    IL_047a:  br.s       IL_048d

    IL_047c:  ldloca.s   V_0
    IL_047e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0483:  ldloc.1
    IL_0484:  ldc.i4.s   31
    IL_0486:  and
    IL_0487:  shl
    IL_0488:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_048d:  starg.s    a
    IL_048f:  ldarg.0
    IL_0490:  stloc.0
    IL_0491:  ldarg.1
    IL_0492:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0497:  stloc.1
    IL_0498:  ldloca.s   V_0
    IL_049a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_049f:  brtrue.s   IL_04ac

    IL_04a1:  ldloca.s   V_2
    IL_04a3:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04a9:  ldloc.2
    IL_04aa:  br.s       IL_04bd

    IL_04ac:  ldloca.s   V_0
    IL_04ae:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04b3:  ldloc.1
    IL_04b4:  ldc.i4.s   31
    IL_04b6:  and
    IL_04b7:  shr
    IL_04b8:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04bd:  starg.s    a
    IL_04bf:  ldarg.1
    IL_04c0:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_04c5:  stloc.1
    IL_04c6:  ldarg.0
    IL_04c7:  stloc.0
    IL_04c8:  ldloca.s   V_0
    IL_04ca:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_04cf:  brtrue.s   IL_04dc

    IL_04d1:  ldloca.s   V_2
    IL_04d3:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_04d9:  ldloc.2
    IL_04da:  br.s       IL_04ea

    IL_04dc:  ldloc.1
    IL_04dd:  ldloca.s   V_0
    IL_04df:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_04e4:  add
    IL_04e5:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_04ea:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_04ef:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_04f4:  ldc.i4.0
    IL_04f5:  newarr     valuetype [mscorlib]System.Nullable`1<int32>
    IL_04fa:  ldc.i4.0
    IL_04fb:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0500:  dup
    IL_0501:  ldobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0506:  stloc.0
    IL_0507:  ldarg.1
    IL_0508:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_050d:  stloc.1
    IL_050e:  ldloca.s   V_0
    IL_0510:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0515:  brtrue.s   IL_0522

    IL_0517:  ldloca.s   V_2
    IL_0519:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_051f:  ldloc.2
    IL_0520:  br.s       IL_0530

    IL_0522:  ldloca.s   V_0
    IL_0524:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0529:  ldloc.1
    IL_052a:  add
    IL_052b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0530:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0535:  ret
  } // end of method LiftedOperators::IntValueComplex

  .method public hidebysig static void  IntValueConst(valuetype [mscorlib]System.Nullable`1<int32> a) cil managed
  {
    // Code size       1075 (0x433)
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
    IL_000c:  ceq
    IL_000e:  ldloca.s   V_0
    IL_0010:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0015:  and
    IL_0016:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_001b:  ldarg.0
    IL_001c:  stloc.0
    IL_001d:  ldc.i4.2
    IL_001e:  stloc.1
    IL_001f:  ldloca.s   V_0
    IL_0021:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0026:  ldloc.1
    IL_0027:  ceq
    IL_0029:  ldloca.s   V_0
    IL_002b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0030:  and
    IL_0031:  ldc.i4.0
    IL_0032:  ceq
    IL_0034:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0039:  ldarg.0
    IL_003a:  stloc.0
    IL_003b:  ldc.i4.2
    IL_003c:  stloc.1
    IL_003d:  ldloca.s   V_0
    IL_003f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0044:  ldloc.1
    IL_0045:  cgt
    IL_0047:  ldloca.s   V_0
    IL_0049:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_004e:  and
    IL_004f:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0054:  ldc.i4.2
    IL_0055:  ldarg.0
    IL_0056:  stloc.0
    IL_0057:  ldloca.s   V_0
    IL_0059:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_005e:  ceq
    IL_0060:  ldloca.s   V_0
    IL_0062:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0067:  and
    IL_0068:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_006d:  ldc.i4.2
    IL_006e:  ldarg.0
    IL_006f:  stloc.0
    IL_0070:  ldloca.s   V_0
    IL_0072:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0077:  ceq
    IL_0079:  ldloca.s   V_0
    IL_007b:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0080:  and
    IL_0081:  ldc.i4.0
    IL_0082:  ceq
    IL_0084:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0089:  ldc.i4.2
    IL_008a:  ldarg.0
    IL_008b:  stloc.0
    IL_008c:  ldloca.s   V_0
    IL_008e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0093:  cgt
    IL_0095:  ldloca.s   V_0
    IL_0097:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_009c:  and
    IL_009d:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00a2:  ldarg.0
    IL_00a3:  stloc.0
    IL_00a4:  ldloca.s   V_0
    IL_00a6:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00ab:  brtrue.s   IL_00b8

    IL_00ad:  ldloca.s   V_2
    IL_00af:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00b5:  ldloc.2
    IL_00b6:  br.s       IL_00c6

    IL_00b8:  ldloca.s   V_0
    IL_00ba:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00bf:  ldc.i4.2
    IL_00c0:  add
    IL_00c1:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00c6:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_00cb:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00d0:  ldarg.0
    IL_00d1:  stloc.0
    IL_00d2:  ldloca.s   V_0
    IL_00d4:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_00d9:  brtrue.s   IL_00e6

    IL_00db:  ldloca.s   V_2
    IL_00dd:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00e3:  ldloc.2
    IL_00e4:  br.s       IL_00f4

    IL_00e6:  ldloca.s   V_0
    IL_00e8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_00ed:  ldc.i4.2
    IL_00ee:  sub
    IL_00ef:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00f4:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_00f9:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00fe:  ldarg.0
    IL_00ff:  stloc.0
    IL_0100:  ldloca.s   V_0
    IL_0102:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0107:  brtrue.s   IL_0114

    IL_0109:  ldloca.s   V_2
    IL_010b:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0111:  ldloc.2
    IL_0112:  br.s       IL_0122

    IL_0114:  ldloca.s   V_0
    IL_0116:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_011b:  ldc.i4.2
    IL_011c:  mul
    IL_011d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0122:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0127:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_012c:  ldarg.0
    IL_012d:  stloc.0
    IL_012e:  ldloca.s   V_0
    IL_0130:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0135:  brtrue.s   IL_0142

    IL_0137:  ldloca.s   V_2
    IL_0139:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_013f:  ldloc.2
    IL_0140:  br.s       IL_0150

    IL_0142:  ldloca.s   V_0
    IL_0144:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0149:  ldc.i4.2
    IL_014a:  div
    IL_014b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0150:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0155:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_015a:  ldarg.0
    IL_015b:  stloc.0
    IL_015c:  ldloca.s   V_0
    IL_015e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0163:  brtrue.s   IL_0170

    IL_0165:  ldloca.s   V_2
    IL_0167:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_016d:  ldloc.2
    IL_016e:  br.s       IL_017e

    IL_0170:  ldloca.s   V_0
    IL_0172:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0177:  ldc.i4.2
    IL_0178:  rem
    IL_0179:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_017e:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0183:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0188:  ldarg.0
    IL_0189:  stloc.0
    IL_018a:  ldloca.s   V_0
    IL_018c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0191:  brtrue.s   IL_019e

    IL_0193:  ldloca.s   V_2
    IL_0195:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_019b:  ldloc.2
    IL_019c:  br.s       IL_01ac

    IL_019e:  ldloca.s   V_0
    IL_01a0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01a5:  ldc.i4.2
    IL_01a6:  and
    IL_01a7:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01ac:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01b1:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01b6:  ldarg.0
    IL_01b7:  stloc.0
    IL_01b8:  ldloca.s   V_0
    IL_01ba:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01bf:  brtrue.s   IL_01cc

    IL_01c1:  ldloca.s   V_2
    IL_01c3:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01c9:  ldloc.2
    IL_01ca:  br.s       IL_01da

    IL_01cc:  ldloca.s   V_0
    IL_01ce:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_01d3:  ldc.i4.2
    IL_01d4:  or
    IL_01d5:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01da:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_01df:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01e4:  ldarg.0
    IL_01e5:  stloc.0
    IL_01e6:  ldloca.s   V_0
    IL_01e8:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_01ed:  brtrue.s   IL_01fa

    IL_01ef:  ldloca.s   V_2
    IL_01f1:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01f7:  ldloc.2
    IL_01f8:  br.s       IL_0208

    IL_01fa:  ldloca.s   V_0
    IL_01fc:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0201:  ldc.i4.2
    IL_0202:  xor
    IL_0203:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0208:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_020d:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0212:  ldarg.0
    IL_0213:  stloc.0
    IL_0214:  ldloca.s   V_0
    IL_0216:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_021b:  brtrue.s   IL_0228

    IL_021d:  ldloca.s   V_2
    IL_021f:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0225:  ldloc.2
    IL_0226:  br.s       IL_0236

    IL_0228:  ldloca.s   V_0
    IL_022a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_022f:  ldc.i4.2
    IL_0230:  shl
    IL_0231:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0236:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_023b:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0240:  ldarg.0
    IL_0241:  stloc.0
    IL_0242:  ldloca.s   V_0
    IL_0244:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0249:  brtrue.s   IL_0256

    IL_024b:  ldloca.s   V_2
    IL_024d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0253:  ldloc.2
    IL_0254:  br.s       IL_0264

    IL_0256:  ldloca.s   V_0
    IL_0258:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_025d:  ldc.i4.2
    IL_025e:  shr
    IL_025f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0264:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0269:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_026e:  ldarg.0
    IL_026f:  stloc.0
    IL_0270:  ldloca.s   V_0
    IL_0272:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0277:  brtrue.s   IL_027c

    IL_0279:  ldc.i4.2
    IL_027a:  br.s       IL_0283

    IL_027c:  ldloca.s   V_0
    IL_027e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0283:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0288:  ldarg.0
    IL_0289:  stloc.0
    IL_028a:  ldloca.s   V_0
    IL_028c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0291:  brtrue.s   IL_029e

    IL_0293:  ldloca.s   V_2
    IL_0295:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_029b:  ldloc.2
    IL_029c:  br.s       IL_02ac

    IL_029e:  ldloca.s   V_0
    IL_02a0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02a5:  ldc.i4.2
    IL_02a6:  add
    IL_02a7:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02ac:  starg.s    a
    IL_02ae:  ldarg.0
    IL_02af:  stloc.0
    IL_02b0:  ldloca.s   V_0
    IL_02b2:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02b7:  brtrue.s   IL_02c4

    IL_02b9:  ldloca.s   V_2
    IL_02bb:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02c1:  ldloc.2
    IL_02c2:  br.s       IL_02d2

    IL_02c4:  ldloca.s   V_0
    IL_02c6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02cb:  ldc.i4.2
    IL_02cc:  sub
    IL_02cd:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02d2:  starg.s    a
    IL_02d4:  ldarg.0
    IL_02d5:  stloc.0
    IL_02d6:  ldloca.s   V_0
    IL_02d8:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_02dd:  brtrue.s   IL_02ea

    IL_02df:  ldloca.s   V_2
    IL_02e1:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_02e7:  ldloc.2
    IL_02e8:  br.s       IL_02f8

    IL_02ea:  ldloca.s   V_0
    IL_02ec:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_02f1:  ldc.i4.2
    IL_02f2:  mul
    IL_02f3:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_02f8:  starg.s    a
    IL_02fa:  ldarg.0
    IL_02fb:  stloc.0
    IL_02fc:  ldloca.s   V_0
    IL_02fe:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0303:  brtrue.s   IL_0310

    IL_0305:  ldloca.s   V_2
    IL_0307:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_030d:  ldloc.2
    IL_030e:  br.s       IL_031e

    IL_0310:  ldloca.s   V_0
    IL_0312:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0317:  ldc.i4.2
    IL_0318:  div
    IL_0319:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_031e:  starg.s    a
    IL_0320:  ldarg.0
    IL_0321:  stloc.0
    IL_0322:  ldloca.s   V_0
    IL_0324:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0329:  brtrue.s   IL_0336

    IL_032b:  ldloca.s   V_2
    IL_032d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0333:  ldloc.2
    IL_0334:  br.s       IL_0344

    IL_0336:  ldloca.s   V_0
    IL_0338:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_033d:  ldc.i4.2
    IL_033e:  rem
    IL_033f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0344:  starg.s    a
    IL_0346:  ldarg.0
    IL_0347:  stloc.0
    IL_0348:  ldloca.s   V_0
    IL_034a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_034f:  brtrue.s   IL_035c

    IL_0351:  ldloca.s   V_2
    IL_0353:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0359:  ldloc.2
    IL_035a:  br.s       IL_036a

    IL_035c:  ldloca.s   V_0
    IL_035e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0363:  ldc.i4.2
    IL_0364:  and
    IL_0365:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_036a:  starg.s    a
    IL_036c:  ldarg.0
    IL_036d:  stloc.0
    IL_036e:  ldloca.s   V_0
    IL_0370:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0375:  brtrue.s   IL_0382

    IL_0377:  ldloca.s   V_2
    IL_0379:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_037f:  ldloc.2
    IL_0380:  br.s       IL_0390

    IL_0382:  ldloca.s   V_0
    IL_0384:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0389:  ldc.i4.2
    IL_038a:  or
    IL_038b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0390:  starg.s    a
    IL_0392:  ldarg.0
    IL_0393:  stloc.0
    IL_0394:  ldloca.s   V_0
    IL_0396:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_039b:  brtrue.s   IL_03a8

    IL_039d:  ldloca.s   V_2
    IL_039f:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03a5:  ldloc.2
    IL_03a6:  br.s       IL_03b6

    IL_03a8:  ldloca.s   V_0
    IL_03aa:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03af:  ldc.i4.2
    IL_03b0:  xor
    IL_03b1:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03b6:  starg.s    a
    IL_03b8:  ldarg.0
    IL_03b9:  stloc.0
    IL_03ba:  ldloca.s   V_0
    IL_03bc:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03c1:  brtrue.s   IL_03ce

    IL_03c3:  ldloca.s   V_2
    IL_03c5:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03cb:  ldloc.2
    IL_03cc:  br.s       IL_03dc

    IL_03ce:  ldloca.s   V_0
    IL_03d0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03d5:  ldc.i4.2
    IL_03d6:  shl
    IL_03d7:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_03dc:  starg.s    a
    IL_03de:  ldarg.0
    IL_03df:  stloc.0
    IL_03e0:  ldloca.s   V_0
    IL_03e2:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_03e7:  brtrue.s   IL_03f4

    IL_03e9:  ldloca.s   V_2
    IL_03eb:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_03f1:  ldloc.2
    IL_03f2:  br.s       IL_0402

    IL_03f4:  ldloca.s   V_0
    IL_03f6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_03fb:  ldc.i4.2
    IL_03fc:  shr
    IL_03fd:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0402:  starg.s    a
    IL_0404:  ldarg.0
    IL_0405:  stloc.0
    IL_0406:  ldloca.s   V_0
    IL_0408:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_040d:  brtrue.s   IL_041a

    IL_040f:  ldloca.s   V_2
    IL_0411:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0417:  ldloc.2
    IL_0418:  br.s       IL_0428

    IL_041a:  ldc.i4.2
    IL_041b:  ldloca.s   V_0
    IL_041d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0422:  add
    IL_0423:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0428:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_042d:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0432:  ret
  } // end of method LiftedOperators::IntValueConst

  .method public hidebysig static void  NumberBasic(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> a,
                                                    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> b) cil managed
  {
    // Code size       371 (0x173)
    .maxstack  3
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
    IL_0017:  ldloca.s   V_0
    IL_0019:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_001e:  ldloca.s   V_1
    IL_0020:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0025:  ceq
    IL_0027:  and
    IL_0028:  brfalse.s  IL_002f

    IL_002a:  call       void [mscorlib]System.Console::WriteLine()
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
    IL_0057:  brtrue.s   IL_005e

    IL_0059:  call       void [mscorlib]System.Console::WriteLine()
    IL_005e:  ldarg.0
    IL_005f:  stloc.0
    IL_0060:  ldarg.1
    IL_0061:  stloc.1
    IL_0062:  ldloca.s   V_0
    IL_0064:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0069:  ldloca.s   V_1
    IL_006b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0070:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_0075:  ldloca.s   V_0
    IL_0077:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_007c:  ldloca.s   V_1
    IL_007e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0083:  and
    IL_0084:  and
    IL_0085:  brfalse.s  IL_008c

    IL_0087:  call       void [mscorlib]System.Console::WriteLine()
    IL_008c:  ldarg.0
    IL_008d:  stloc.1
    IL_008e:  ldarg.1
    IL_008f:  stloc.0
    IL_0090:  ldloca.s   V_1
    IL_0092:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0097:  ldloca.s   V_0
    IL_0099:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_009e:  call       bool [mscorlib]System.Decimal::op_LessThan(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_00a3:  ldloca.s   V_1
    IL_00a5:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00aa:  ldloca.s   V_0
    IL_00ac:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00b1:  and
    IL_00b2:  and
    IL_00b3:  brfalse.s  IL_00ba

    IL_00b5:  call       void [mscorlib]System.Console::WriteLine()
    IL_00ba:  ldarg.0
    IL_00bb:  stloc.0
    IL_00bc:  ldarg.1
    IL_00bd:  stloc.1
    IL_00be:  ldloca.s   V_0
    IL_00c0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00c5:  ldloca.s   V_1
    IL_00c7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00cc:  call       bool [mscorlib]System.Decimal::op_GreaterThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                              valuetype [mscorlib]System.Decimal)
    IL_00d1:  ldloca.s   V_0
    IL_00d3:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00d8:  ldloca.s   V_1
    IL_00da:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00df:  and
    IL_00e0:  and
    IL_00e1:  brfalse.s  IL_00e8

    IL_00e3:  call       void [mscorlib]System.Console::WriteLine()
    IL_00e8:  ldarg.0
    IL_00e9:  stloc.1
    IL_00ea:  ldarg.1
    IL_00eb:  stloc.0
    IL_00ec:  ldloca.s   V_1
    IL_00ee:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00f3:  ldloca.s   V_0
    IL_00f5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00fa:  call       bool [mscorlib]System.Decimal::op_LessThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                           valuetype [mscorlib]System.Decimal)
    IL_00ff:  ldloca.s   V_1
    IL_0101:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0106:  ldloca.s   V_0
    IL_0108:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_010d:  and
    IL_010e:  and
    IL_010f:  brfalse.s  IL_0116

    IL_0111:  call       void [mscorlib]System.Console::WriteLine()
    IL_0116:  ldarg.0
    IL_0117:  stloc.0
    IL_0118:  ldarg.1
    IL_0119:  stloc.1
    IL_011a:  ldloca.s   V_0
    IL_011c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0121:  ldloca.s   V_1
    IL_0123:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0128:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_012d:  ldloca.s   V_0
    IL_012f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0134:  ldloca.s   V_1
    IL_0136:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_013b:  and
    IL_013c:  and
    IL_013d:  brtrue.s   IL_0144

    IL_013f:  call       void [mscorlib]System.Console::WriteLine()
    IL_0144:  ldarg.0
    IL_0145:  stloc.1
    IL_0146:  ldarg.1
    IL_0147:  stloc.0
    IL_0148:  ldloca.s   V_1
    IL_014a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_014f:  ldloca.s   V_0
    IL_0151:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0156:  call       bool [mscorlib]System.Decimal::op_LessThan(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_015b:  ldloca.s   V_1
    IL_015d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0162:  ldloca.s   V_0
    IL_0164:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0169:  and
    IL_016a:  and
    IL_016b:  brtrue.s   IL_0172

    IL_016d:  call       void [mscorlib]System.Console::WriteLine()
    IL_0172:  ret
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
    // Code size       926 (0x39e)
    .maxstack  3
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
    IL_0017:  ldloca.s   V_0
    IL_0019:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_001e:  ldloca.s   V_1
    IL_0020:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0025:  ceq
    IL_0027:  and
    IL_0028:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_002d:  ldarg.0
    IL_002e:  stloc.1
    IL_002f:  ldarg.1
    IL_0030:  stloc.0
    IL_0031:  ldloca.s   V_1
    IL_0033:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0038:  ldloca.s   V_0
    IL_003a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_003f:  call       bool [mscorlib]System.Decimal::op_Equality(valuetype [mscorlib]System.Decimal,
                                                                    valuetype [mscorlib]System.Decimal)
    IL_0044:  ldloca.s   V_1
    IL_0046:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_004b:  ldloca.s   V_0
    IL_004d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0052:  ceq
    IL_0054:  and
    IL_0055:  ldc.i4.0
    IL_0056:  ceq
    IL_0058:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_005d:  ldarg.0
    IL_005e:  stloc.0
    IL_005f:  ldarg.1
    IL_0060:  stloc.1
    IL_0061:  ldloca.s   V_0
    IL_0063:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0068:  ldloca.s   V_1
    IL_006a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_006f:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_0074:  ldloca.s   V_0
    IL_0076:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_007b:  ldloca.s   V_1
    IL_007d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0082:  and
    IL_0083:  and
    IL_0084:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0089:  ldarg.0
    IL_008a:  stloc.1
    IL_008b:  ldarg.1
    IL_008c:  stloc.0
    IL_008d:  ldloca.s   V_1
    IL_008f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0094:  ldloca.s   V_0
    IL_0096:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_009b:  call       bool [mscorlib]System.Decimal::op_GreaterThan(valuetype [mscorlib]System.Decimal,
                                                                       valuetype [mscorlib]System.Decimal)
    IL_00a0:  ldloca.s   V_1
    IL_00a2:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00a7:  ldloca.s   V_0
    IL_00a9:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00ae:  and
    IL_00af:  and
    IL_00b0:  ldc.i4.0
    IL_00b1:  ceq
    IL_00b3:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00b8:  ldarg.0
    IL_00b9:  stloc.0
    IL_00ba:  ldarg.1
    IL_00bb:  stloc.1
    IL_00bc:  ldloca.s   V_0
    IL_00be:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00c3:  ldloca.s   V_1
    IL_00c5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_00ca:  call       bool [mscorlib]System.Decimal::op_LessThanOrEqual(valuetype [mscorlib]System.Decimal,
                                                                           valuetype [mscorlib]System.Decimal)
    IL_00cf:  ldloca.s   V_0
    IL_00d1:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00d6:  ldloca.s   V_1
    IL_00d8:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00dd:  and
    IL_00de:  and
    IL_00df:  ldc.i4.0
    IL_00e0:  ceq
    IL_00e2:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_00e7:  ldarg.0
    IL_00e8:  stloc.1
    IL_00e9:  ldarg.1
    IL_00ea:  stloc.0
    IL_00eb:  ldloca.s   V_1
    IL_00ed:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00f2:  ldloca.s   V_0
    IL_00f4:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_00f9:  and
    IL_00fa:  brtrue.s   IL_0107

    IL_00fc:  ldloca.s   V_2
    IL_00fe:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0104:  ldloc.2
    IL_0105:  br.s       IL_011f

    IL_0107:  ldloca.s   V_1
    IL_0109:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_010e:  ldloca.s   V_0
    IL_0110:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0115:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_011a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_011f:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0124:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0129:  ldarg.0
    IL_012a:  stloc.0
    IL_012b:  ldarg.1
    IL_012c:  stloc.1
    IL_012d:  ldloca.s   V_0
    IL_012f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0134:  ldloca.s   V_1
    IL_0136:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_013b:  and
    IL_013c:  brtrue.s   IL_0149

    IL_013e:  ldloca.s   V_2
    IL_0140:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0146:  ldloc.2
    IL_0147:  br.s       IL_0161

    IL_0149:  ldloca.s   V_0
    IL_014b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0150:  ldloca.s   V_1
    IL_0152:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0157:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Subtraction(valuetype [mscorlib]System.Decimal,
                                                                                                     valuetype [mscorlib]System.Decimal)
    IL_015c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0161:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0166:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_016b:  ldarg.0
    IL_016c:  stloc.1
    IL_016d:  ldarg.1
    IL_016e:  stloc.0
    IL_016f:  ldloca.s   V_1
    IL_0171:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0176:  ldloca.s   V_0
    IL_0178:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_017d:  and
    IL_017e:  brtrue.s   IL_018b

    IL_0180:  ldloca.s   V_2
    IL_0182:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0188:  ldloc.2
    IL_0189:  br.s       IL_01a3

    IL_018b:  ldloca.s   V_1
    IL_018d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0192:  ldloca.s   V_0
    IL_0194:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0199:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Multiply(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_019e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_01a3:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_01a8:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01ad:  ldarg.0
    IL_01ae:  stloc.0
    IL_01af:  ldarg.1
    IL_01b0:  stloc.1
    IL_01b1:  ldloca.s   V_0
    IL_01b3:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_01b8:  ldloca.s   V_1
    IL_01ba:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_01bf:  and
    IL_01c0:  brtrue.s   IL_01cd

    IL_01c2:  ldloca.s   V_2
    IL_01c4:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_01ca:  ldloc.2
    IL_01cb:  br.s       IL_01e5

    IL_01cd:  ldloca.s   V_0
    IL_01cf:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01d4:  ldloca.s   V_1
    IL_01d6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_01db:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Division(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_01e0:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_01e5:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_01ea:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_01ef:  ldarg.0
    IL_01f0:  stloc.1
    IL_01f1:  ldarg.1
    IL_01f2:  stloc.0
    IL_01f3:  ldloca.s   V_1
    IL_01f5:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_01fa:  ldloca.s   V_0
    IL_01fc:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0201:  and
    IL_0202:  brtrue.s   IL_020f

    IL_0204:  ldloca.s   V_2
    IL_0206:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_020c:  ldloc.2
    IL_020d:  br.s       IL_0227

    IL_020f:  ldloca.s   V_1
    IL_0211:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0216:  ldloca.s   V_0
    IL_0218:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_021d:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Modulus(valuetype [mscorlib]System.Decimal,
                                                                                                 valuetype [mscorlib]System.Decimal)
    IL_0222:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0227:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_022c:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0231:  ldarg.0
    IL_0232:  stloc.0
    IL_0233:  ldloca.s   V_0
    IL_0235:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_023a:  brtrue.s   IL_023f

    IL_023c:  ldarg.1
    IL_023d:  br.s       IL_0240

    IL_023f:  ldloc.0
    IL_0240:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0245:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_024a:  ldarg.0
    IL_024b:  stloc.0
    IL_024c:  ldloca.s   V_0
    IL_024e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0253:  brtrue.s   IL_0260

    IL_0255:  ldloca.s   V_1
    IL_0257:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_025d:  ldloc.1
    IL_025e:  br.s       IL_0271

    IL_0260:  ldloca.s   V_0
    IL_0262:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0267:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_UnaryNegation(valuetype [mscorlib]System.Decimal)
    IL_026c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0271:  box        valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0276:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_027b:  ldarg.0
    IL_027c:  stloc.0
    IL_027d:  ldarg.1
    IL_027e:  stloc.1
    IL_027f:  ldloca.s   V_0
    IL_0281:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0286:  ldloca.s   V_1
    IL_0288:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_028d:  and
    IL_028e:  brtrue.s   IL_029b

    IL_0290:  ldloca.s   V_2
    IL_0292:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0298:  ldloc.2
    IL_0299:  br.s       IL_02b3

    IL_029b:  ldloca.s   V_0
    IL_029d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_02a2:  ldloca.s   V_1
    IL_02a4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_02a9:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Addition(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_02ae:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_02b3:  starg.s    a
    IL_02b5:  ldarg.0
    IL_02b6:  stloc.1
    IL_02b7:  ldarg.1
    IL_02b8:  stloc.0
    IL_02b9:  ldloca.s   V_1
    IL_02bb:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_02c0:  ldloca.s   V_0
    IL_02c2:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_02c7:  and
    IL_02c8:  brtrue.s   IL_02d5

    IL_02ca:  ldloca.s   V_2
    IL_02cc:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_02d2:  ldloc.2
    IL_02d3:  br.s       IL_02ed

    IL_02d5:  ldloca.s   V_1
    IL_02d7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_02dc:  ldloca.s   V_0
    IL_02de:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_02e3:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Subtraction(valuetype [mscorlib]System.Decimal,
                                                                                                     valuetype [mscorlib]System.Decimal)
    IL_02e8:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_02ed:  starg.s    a
    IL_02ef:  ldarg.0
    IL_02f0:  stloc.0
    IL_02f1:  ldarg.1
    IL_02f2:  stloc.1
    IL_02f3:  ldloca.s   V_0
    IL_02f5:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_02fa:  ldloca.s   V_1
    IL_02fc:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0301:  and
    IL_0302:  brtrue.s   IL_030f

    IL_0304:  ldloca.s   V_2
    IL_0306:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_030c:  ldloc.2
    IL_030d:  br.s       IL_0327

    IL_030f:  ldloca.s   V_0
    IL_0311:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0316:  ldloca.s   V_1
    IL_0318:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_031d:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Multiply(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_0322:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0327:  starg.s    a
    IL_0329:  ldarg.0
    IL_032a:  stloc.1
    IL_032b:  ldarg.1
    IL_032c:  stloc.0
    IL_032d:  ldloca.s   V_1
    IL_032f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0334:  ldloca.s   V_0
    IL_0336:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_033b:  and
    IL_033c:  brtrue.s   IL_0349

    IL_033e:  ldloca.s   V_2
    IL_0340:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0346:  ldloc.2
    IL_0347:  br.s       IL_0361

    IL_0349:  ldloca.s   V_1
    IL_034b:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0350:  ldloca.s   V_0
    IL_0352:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0357:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Division(valuetype [mscorlib]System.Decimal,
                                                                                                  valuetype [mscorlib]System.Decimal)
    IL_035c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0361:  starg.s    a
    IL_0363:  ldarg.0
    IL_0364:  stloc.0
    IL_0365:  ldarg.1
    IL_0366:  stloc.1
    IL_0367:  ldloca.s   V_0
    IL_0369:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_036e:  ldloca.s   V_1
    IL_0370:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::get_HasValue()
    IL_0375:  and
    IL_0376:  brtrue.s   IL_0383

    IL_0378:  ldloca.s   V_2
    IL_037a:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0380:  ldloc.2
    IL_0381:  br.s       IL_039b

    IL_0383:  ldloca.s   V_0
    IL_0385:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_038a:  ldloca.s   V_1
    IL_038c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::GetValueOrDefault()
    IL_0391:  call       valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Modulus(valuetype [mscorlib]System.Decimal,
                                                                                                 valuetype [mscorlib]System.Decimal)
    IL_0396:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_039b:  starg.s    a
    IL_039d:  ret
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
    // Code size       156 (0x9c)
    .maxstack  3
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
    IL_0034:  clt
    IL_0036:  ldloca.s   V_0
    IL_0038:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_003d:  ldloca.s   V_1
    IL_003f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0044:  and
    IL_0045:  and
    IL_0046:  brfalse.s  IL_004d

    IL_0048:  call       void [mscorlib]System.Console::WriteLine()
    IL_004d:  ldarg.0
    IL_004e:  stloc.2
    IL_004f:  ldloca.s   V_2
    IL_0051:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0056:  brtrue.s   IL_0063

    IL_0058:  ldloca.s   V_3
    IL_005a:  initobj    valuetype [mscorlib]System.Nullable`1<int64>
    IL_0060:  ldloc.3
    IL_0061:  br.s       IL_0070

    IL_0063:  ldloca.s   V_2
    IL_0065:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_006a:  conv.i8
    IL_006b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int64>::.ctor(!0)
    IL_0070:  stloc.1
    IL_0071:  ldarg.1
    IL_0072:  stloc.0
    IL_0073:  ldloca.s   V_1
    IL_0075:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_007a:  ldloca.s   V_0
    IL_007c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int64>::GetValueOrDefault()
    IL_0081:  ceq
    IL_0083:  ldloca.s   V_1
    IL_0085:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_008a:  ldloca.s   V_0
    IL_008c:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0091:  ceq
    IL_0093:  and
    IL_0094:  brfalse.s  IL_009b

    IL_0096:  call       void [mscorlib]System.Console::WriteLine()
    IL_009b:  ret
  } // end of method LiftedOperators::CompareWithImplictCast

  .method public hidebysig static void  CompareWithSignChange(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                              valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       110 (0x6e)
    .maxstack  3
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
    IL_0054:  clt.un
    IL_0056:  ldloca.s   V_0
    IL_0058:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint32>::get_HasValue()
    IL_005d:  ldloca.s   V_1
    IL_005f:  call       instance bool valuetype [mscorlib]System.Nullable`1<uint32>::get_HasValue()
    IL_0064:  and
    IL_0065:  and
    IL_0066:  brfalse.s  IL_006d

    IL_0068:  call       void [mscorlib]System.Console::WriteLine()
    IL_006d:  ret
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
    // Code size       38 (0x26)
    .maxstack  3
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
    IL_0012:  ceq
    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001b:  ldloca.s   V_1
    IL_001d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0022:  ceq
    IL_0024:  and
    IL_0025:  ret
  } // end of method LiftedOperators::RetEq

  .method public hidebysig static bool  RetEqConv(valuetype [mscorlib]System.Nullable`1<int64> a,
                                                  valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       72 (0x48)
    .maxstack  3
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
    IL_0034:  ceq
    IL_0036:  ldloca.s   V_0
    IL_0038:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_003d:  ldloca.s   V_1
    IL_003f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0044:  ceq
    IL_0046:  and
    IL_0047:  ret
  } // end of method LiftedOperators::RetEqConv

  .method public hidebysig static bool  RetEqConst(valuetype [mscorlib]System.Nullable`1<int64> a) cil managed
  {
    // Code size       25 (0x19)
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
    IL_000e:  ceq
    IL_0010:  ldloca.s   V_0
    IL_0012:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0017:  and
    IL_0018:  ret
  } // end of method LiftedOperators::RetEqConst

  .method public hidebysig static bool  RetIneqConst(valuetype [mscorlib]System.Nullable`1<int64> a) cil managed
  {
    // Code size       28 (0x1c)
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
    IL_000e:  ceq
    IL_0010:  ldloca.s   V_0
    IL_0012:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0017:  and
    IL_0018:  ldc.i4.0
    IL_0019:  ceq
    IL_001b:  ret
  } // end of method LiftedOperators::RetIneqConst

  .method public hidebysig static bool  RetLt(valuetype [mscorlib]System.Nullable`1<int32> a,
                                              valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       37 (0x25)
    .maxstack  3
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
    IL_0012:  clt
    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001b:  ldloca.s   V_1
    IL_001d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0022:  and
    IL_0023:  and
    IL_0024:  ret
  } // end of method LiftedOperators::RetLt

  .method public hidebysig static bool  RetLtConst(valuetype [mscorlib]System.Nullable`1<int32> a) cil managed
  {
    // Code size       24 (0x18)
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
    IL_000d:  clt
    IL_000f:  ldloca.s   V_0
    IL_0011:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0016:  and
    IL_0017:  ret
  } // end of method LiftedOperators::RetLtConst

  .method public hidebysig static bool  RetLtConv(valuetype [mscorlib]System.Nullable`1<int64> a,
                                                  valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       71 (0x47)
    .maxstack  3
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
    IL_0034:  clt
    IL_0036:  ldloca.s   V_0
    IL_0038:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_003d:  ldloca.s   V_1
    IL_003f:  call       instance bool valuetype [mscorlib]System.Nullable`1<int64>::get_HasValue()
    IL_0044:  and
    IL_0045:  and
    IL_0046:  ret
  } // end of method LiftedOperators::RetLtConv

  .method public hidebysig static bool  RetNotLt(valuetype [mscorlib]System.Nullable`1<int32> a,
                                                 valuetype [mscorlib]System.Nullable`1<int32> b) cil managed
  {
    // Code size       40 (0x28)
    .maxstack  3
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
    IL_0012:  clt
    IL_0014:  ldloca.s   V_0
    IL_0016:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_001b:  ldloca.s   V_1
    IL_001d:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0022:  and
    IL_0023:  and
    IL_0024:  ldc.i4.0
    IL_0025:  ceq
    IL_0027:  ret
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
