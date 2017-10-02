
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly utwwumxi
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module utwwumxi.dll
// MVID: {30E98C35-5F99-4742-941F-78E7F27D8BD5}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02C70000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch
       extends [mscorlib]System.Object
{
  .method public hidebysig static string 
          SparseIntegerSwitch(int32 i) cil managed
  {
    // Code size       181 (0xb5)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldstr      "SparseIntegerSwitch: "
    IL_0005:  ldarg.0
    IL_0006:  box        [mscorlib]System.Int32
    IL_000b:  call       string [mscorlib]System.String::Concat(object,
                                                                object)
    IL_0010:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0015:  ldarg.0
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4.4
    IL_0019:  bgt.s      IL_004a

    IL_001b:  ldloc.0
    IL_001c:  ldc.i4     0xff676980
    IL_0021:  beq.s      IL_006d

    IL_0023:  ldloc.0
    IL_0024:  ldc.i4.s   -100
    IL_0026:  beq.s      IL_0073

    IL_0028:  ldloc.0
    IL_0029:  ldc.i4.m1
    IL_002a:  sub
    IL_002b:  switch     ( 
                          IL_0079,
                          IL_007f,
                          IL_0085,
                          IL_008b,
                          IL_00af,
                          IL_0091)
    IL_0048:  br.s       IL_00af

    IL_004a:  ldloc.0
    IL_004b:  ldc.i4.s   100
    IL_004d:  beq.s      IL_0097

    IL_004f:  ldloc.0
    IL_0050:  ldc.i4     0x2710
    IL_0055:  sub
    IL_0056:  switch     ( 
                          IL_009d,
                          IL_00a3)
    IL_0063:  ldloc.0
    IL_0064:  ldc.i4     0x7fffffff
    IL_0069:  beq.s      IL_00a9

    IL_006b:  br.s       IL_00af

    IL_006d:  ldstr      "-10 mln"
    IL_0072:  ret

    IL_0073:  ldstr      "-hundred"
    IL_0078:  ret

    IL_0079:  ldstr      "-1"
    IL_007e:  ret

    IL_007f:  ldstr      "0"
    IL_0084:  ret

    IL_0085:  ldstr      "1"
    IL_008a:  ret

    IL_008b:  ldstr      "2"
    IL_0090:  ret

    IL_0091:  ldstr      "4"
    IL_0096:  ret

    IL_0097:  ldstr      "hundred"
    IL_009c:  ret

    IL_009d:  ldstr      "ten thousand"
    IL_00a2:  ret

    IL_00a3:  ldstr      "ten thousand and one"
    IL_00a8:  ret

    IL_00a9:  ldstr      "int.MaxValue"
    IL_00ae:  ret

    IL_00af:  ldstr      "something else"
    IL_00b4:  ret
  } // end of method Switch::SparseIntegerSwitch

  .method public hidebysig static string 
          SwitchOverNullableInt(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarga.s   i
    IL_0002:  dup
    IL_0003:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0008:  stloc.0
    IL_0009:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000e:  brfalse.s  IL_001f

    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.0
    IL_0012:  beq.s      IL_0025

    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.5
    IL_0016:  beq.s      IL_002b

    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.s   10
    IL_001b:  beq.s      IL_0031

    IL_001d:  br.s       IL_0037

    IL_001f:  ldstr      "null"
    IL_0024:  ret

    IL_0025:  ldstr      "zero"
    IL_002a:  ret

    IL_002b:  ldstr      "five"
    IL_0030:  ret

    IL_0031:  ldstr      "ten"
    IL_0036:  ret

    IL_0037:  ldstr      "large"
    IL_003c:  ret
  } // end of method Switch::SwitchOverNullableInt

  .method public hidebysig static string 
          SwitchOverNullableIntShifted(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       98 (0x62)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             int32 V_3)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brtrue.s   IL_0016

    IL_000b:  ldloca.s   V_1
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0013:  ldloc.1
    IL_0014:  br.s       IL_0024

    IL_0016:  ldloca.s   V_0
    IL_0018:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001d:  ldc.i4.5
    IL_001e:  add
    IL_001f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0024:  stloc.2
    IL_0025:  ldloca.s   V_2
    IL_0027:  dup
    IL_0028:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_002d:  stloc.3
    IL_002e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0033:  brfalse.s  IL_0044

    IL_0035:  ldloc.3
    IL_0036:  ldc.i4.0
    IL_0037:  beq.s      IL_004a

    IL_0039:  ldloc.3
    IL_003a:  ldc.i4.5
    IL_003b:  beq.s      IL_0050

    IL_003d:  ldloc.3
    IL_003e:  ldc.i4.s   10
    IL_0040:  beq.s      IL_0056

    IL_0042:  br.s       IL_005c

    IL_0044:  ldstr      "null"
    IL_0049:  ret

    IL_004a:  ldstr      "zero"
    IL_004f:  ret

    IL_0050:  ldstr      "five"
    IL_0055:  ret

    IL_0056:  ldstr      "ten"
    IL_005b:  ret

    IL_005c:  ldstr      "large"
    IL_0061:  ret
  } // end of method Switch::SwitchOverNullableIntShifted

  .method public hidebysig static string 
          SwitchOverNullableIntNoNullCase(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       55 (0x37)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarga.s   i
    IL_0002:  dup
    IL_0003:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0008:  stloc.0
    IL_0009:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000e:  brfalse.s  IL_0031

    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.0
    IL_0012:  beq.s      IL_001f

    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.5
    IL_0016:  beq.s      IL_0025

    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.s   10
    IL_001b:  beq.s      IL_002b

    IL_001d:  br.s       IL_0031

    IL_001f:  ldstr      "zero"
    IL_0024:  ret

    IL_0025:  ldstr      "five"
    IL_002a:  ret

    IL_002b:  ldstr      "ten"
    IL_0030:  ret

    IL_0031:  ldstr      "other"
    IL_0036:  ret
  } // end of method Switch::SwitchOverNullableIntNoNullCase

  .method public hidebysig static string 
          SwitchOverNullableIntNoNullCaseShifted(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       92 (0x5c)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             int32 V_3)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brtrue.s   IL_0016

    IL_000b:  ldloca.s   V_1
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0013:  ldloc.1
    IL_0014:  br.s       IL_0024

    IL_0016:  ldloca.s   V_0
    IL_0018:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001d:  ldc.i4.5
    IL_001e:  add
    IL_001f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0024:  stloc.2
    IL_0025:  ldloca.s   V_2
    IL_0027:  dup
    IL_0028:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_002d:  stloc.3
    IL_002e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0033:  brfalse.s  IL_0056

    IL_0035:  ldloc.3
    IL_0036:  ldc.i4.0
    IL_0037:  beq.s      IL_0044

    IL_0039:  ldloc.3
    IL_003a:  ldc.i4.5
    IL_003b:  beq.s      IL_004a

    IL_003d:  ldloc.3
    IL_003e:  ldc.i4.s   10
    IL_0040:  beq.s      IL_0050

    IL_0042:  br.s       IL_0056

    IL_0044:  ldstr      "zero"
    IL_0049:  ret

    IL_004a:  ldstr      "five"
    IL_004f:  ret

    IL_0050:  ldstr      "ten"
    IL_0055:  ret

    IL_0056:  ldstr      "other"
    IL_005b:  ret
  } // end of method Switch::SwitchOverNullableIntNoNullCaseShifted

  .method public hidebysig static string 
          ShortSwitchOverString(string text) cil managed
  {
    // Code size       86 (0x56)
    .maxstack  2
    .locals init (string V_0)
    IL_0000:  ldstr      "ShortSwitchOverString: "
    IL_0005:  ldarg.0
    IL_0006:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0010:  ldarg.0
    IL_0011:  dup
    IL_0012:  stloc.0
    IL_0013:  brfalse.s  IL_0050

    IL_0015:  ldloc.0
    IL_0016:  ldstr      "First case"
    IL_001b:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0020:  brtrue.s   IL_003e

    IL_0022:  ldloc.0
    IL_0023:  ldstr      "Second case"
    IL_0028:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_002d:  brtrue.s   IL_0044

    IL_002f:  ldloc.0
    IL_0030:  ldstr      "Third case"
    IL_0035:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_003a:  brtrue.s   IL_004a

    IL_003c:  br.s       IL_0050

    IL_003e:  ldstr      "Text1"
    IL_0043:  ret

    IL_0044:  ldstr      "Text2"
    IL_0049:  ret

    IL_004a:  ldstr      "Text3"
    IL_004f:  ret

    IL_0050:  ldstr      "Default"
    IL_0055:  ret
  } // end of method Switch::ShortSwitchOverString

  .method public hidebysig static string 
          SwitchOverString1(string text) cil managed
  {
    // Code size       227 (0xe3)
    .maxstack  4
    .locals init (string V_0,
             int32 V_1)
    IL_0000:  ldstr      "SwitchOverString1: "
    IL_0005:  ldarg.0
    IL_0006:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0010:  ldarg.0
    IL_0011:  dup
    IL_0012:  stloc.0
    IL_0013:  brfalse    IL_00db

    IL_0018:  volatile.
    IL_001a:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{30E98C35-5F99-4742-941F-78E7F27D8BD5}'::'$$method0x6000007-1'
    IL_001f:  brtrue.s   IL_0082

    IL_0021:  ldc.i4.7
    IL_0022:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::.ctor(int32)
    IL_0027:  dup
    IL_0028:  ldstr      "First case"
    IL_002d:  ldc.i4.0
    IL_002e:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0033:  dup
    IL_0034:  ldstr      "Second case"
    IL_0039:  ldc.i4.1
    IL_003a:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_003f:  dup
    IL_0040:  ldstr      "2nd case"
    IL_0045:  ldc.i4.2
    IL_0046:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_004b:  dup
    IL_004c:  ldstr      "Third case"
    IL_0051:  ldc.i4.3
    IL_0052:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0057:  dup
    IL_0058:  ldstr      "Fourth case"
    IL_005d:  ldc.i4.4
    IL_005e:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0063:  dup
    IL_0064:  ldstr      "Fifth case"
    IL_0069:  ldc.i4.5
    IL_006a:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_006f:  dup
    IL_0070:  ldstr      "Sixth case"
    IL_0075:  ldc.i4.6
    IL_0076:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_007b:  volatile.
    IL_007d:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{30E98C35-5F99-4742-941F-78E7F27D8BD5}'::'$$method0x6000007-1'
    IL_0082:  volatile.
    IL_0084:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{30E98C35-5F99-4742-941F-78E7F27D8BD5}'::'$$method0x6000007-1'
    IL_0089:  ldloc.0
    IL_008a:  ldloca.s   V_1
    IL_008c:  call       instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::TryGetValue(!0,
                                                                                                                          !1&)
    IL_0091:  brfalse.s  IL_00dd

    IL_0093:  ldloc.1
    IL_0094:  switch     ( 
                          IL_00b7,
                          IL_00bd,
                          IL_00bd,
                          IL_00c3,
                          IL_00c9,
                          IL_00cf,
                          IL_00d5)
    IL_00b5:  br.s       IL_00dd

    IL_00b7:  ldstr      "Text1"
    IL_00bc:  ret

    IL_00bd:  ldstr      "Text2"
    IL_00c2:  ret

    IL_00c3:  ldstr      "Text3"
    IL_00c8:  ret

    IL_00c9:  ldstr      "Text4"
    IL_00ce:  ret

    IL_00cf:  ldstr      "Text5"
    IL_00d4:  ret

    IL_00d5:  ldstr      "Text6"
    IL_00da:  ret

    IL_00db:  ldnull
    IL_00dc:  ret

    IL_00dd:  ldstr      "Default"
    IL_00e2:  ret
  } // end of method Switch::SwitchOverString1

  .method public hidebysig static string 
          SwitchOverString2() cil managed
  {
    // Code size       325 (0x145)
    .maxstack  4
    .locals init (string V_0,
             string V_1,
             int32 V_2)
    IL_0000:  ldstr      "SwitchOverString2:"
    IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000a:  call       string [mscorlib]System.Environment::get_UserName()
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  dup
    IL_0012:  stloc.1
    IL_0013:  brfalse    IL_013f

    IL_0018:  volatile.
    IL_001a:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{30E98C35-5F99-4742-941F-78E7F27D8BD5}'::'$$method0x6000008-1'
    IL_001f:  brtrue     IL_00b8

    IL_0024:  ldc.i4.s   11
    IL_0026:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::.ctor(int32)
    IL_002b:  dup
    IL_002c:  ldstr      "First case"
    IL_0031:  ldc.i4.0
    IL_0032:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0037:  dup
    IL_0038:  ldstr      "Second case"
    IL_003d:  ldc.i4.1
    IL_003e:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0043:  dup
    IL_0044:  ldstr      "Third case"
    IL_0049:  ldc.i4.2
    IL_004a:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_004f:  dup
    IL_0050:  ldstr      "Fourth case"
    IL_0055:  ldc.i4.3
    IL_0056:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_005b:  dup
    IL_005c:  ldstr      "Fifth case"
    IL_0061:  ldc.i4.4
    IL_0062:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0067:  dup
    IL_0068:  ldstr      "Sixth case"
    IL_006d:  ldc.i4.5
    IL_006e:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0073:  dup
    IL_0074:  ldstr      "Seventh case"
    IL_0079:  ldc.i4.6
    IL_007a:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_007f:  dup
    IL_0080:  ldstr      "Eighth case"
    IL_0085:  ldc.i4.7
    IL_0086:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_008b:  dup
    IL_008c:  ldstr      "Ninth case"
    IL_0091:  ldc.i4.8
    IL_0092:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0097:  dup
    IL_0098:  ldstr      "Tenth case"
    IL_009d:  ldc.i4.s   9
    IL_009f:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_00a4:  dup
    IL_00a5:  ldstr      "Eleventh case"
    IL_00aa:  ldc.i4.s   10
    IL_00ac:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_00b1:  volatile.
    IL_00b3:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{30E98C35-5F99-4742-941F-78E7F27D8BD5}'::'$$method0x6000008-1'
    IL_00b8:  volatile.
    IL_00ba:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{30E98C35-5F99-4742-941F-78E7F27D8BD5}'::'$$method0x6000008-1'
    IL_00bf:  ldloc.1
    IL_00c0:  ldloca.s   V_2
    IL_00c2:  call       instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::TryGetValue(!0,
                                                                                                                          !1&)
    IL_00c7:  brfalse.s  IL_013f

    IL_00c9:  ldloc.2
    IL_00ca:  switch     ( 
                          IL_00fd,
                          IL_0103,
                          IL_0109,
                          IL_010f,
                          IL_0115,
                          IL_011b,
                          IL_0121,
                          IL_0127,
                          IL_012d,
                          IL_0133,
                          IL_0139)
    IL_00fb:  br.s       IL_013f

    IL_00fd:  ldstr      "Text1"
    IL_0102:  ret

    IL_0103:  ldstr      "Text2"
    IL_0108:  ret

    IL_0109:  ldstr      "Text3"
    IL_010e:  ret

    IL_010f:  ldstr      "Text4"
    IL_0114:  ret

    IL_0115:  ldstr      "Text5"
    IL_011a:  ret

    IL_011b:  ldstr      "Text6"
    IL_0120:  ret

    IL_0121:  ldstr      "Text7"
    IL_0126:  ret

    IL_0127:  ldstr      "Text8"
    IL_012c:  ret

    IL_012d:  ldstr      "Text9"
    IL_0132:  ret

    IL_0133:  ldstr      "Text10"
    IL_0138:  ret

    IL_0139:  ldstr      "Text11"
    IL_013e:  ret

    IL_013f:  ldstr      "Default"
    IL_0144:  ret
  } // end of method Switch::SwitchOverString2

  .method public hidebysig static string 
          SwitchOverBool(bool b) cil managed
  {
    // Code size       54 (0x36)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  ldstr      "SwitchOverBool: "
    IL_0005:  ldarga.s   b
    IL_0007:  call       instance string [mscorlib]System.Boolean::ToString()
    IL_000c:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0011:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0016:  ldarg.0
    IL_0017:  stloc.0
    IL_0018:  ldloc.0
    IL_0019:  switch     ( 
                          IL_002e,
                          IL_0028)
    IL_0026:  br.s       IL_0034

    IL_0028:  ldsfld     string [mscorlib]System.Boolean::TrueString
    IL_002d:  ret

    IL_002e:  ldsfld     string [mscorlib]System.Boolean::FalseString
    IL_0033:  ret

    IL_0034:  ldnull
    IL_0035:  ret
  } // end of method Switch::SwitchOverBool

  .method public hidebysig static void  SwitchInLoop(int32 i) cil managed
  {
    // Code size       124 (0x7c)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldstr      "SwitchInLoop: "
    IL_0005:  ldarg.0
    IL_0006:  box        [mscorlib]System.Int32
    IL_000b:  call       string [mscorlib]System.String::Concat(object,
                                                                object)
    IL_0010:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0015:  ldarg.0
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4.1
    IL_0019:  sub
    IL_001a:  switch     ( 
                          IL_0031,
                          IL_003d,
                          IL_0049,
                          IL_0055)
    IL_002f:  br.s       IL_0060

    IL_0031:  ldstr      "one"
    IL_0036:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003b:  br.s       IL_0075

    IL_003d:  ldstr      "two"
    IL_0042:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0047:  br.s       IL_0075

    IL_0049:  ldstr      "three"
    IL_004e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0053:  br.s       IL_0015

    IL_0055:  ldstr      "four"
    IL_005a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005f:  ret

    IL_0060:  ldstr      "default"
    IL_0065:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006a:  ldstr      "more code"
    IL_006f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0074:  ret

    IL_0075:  ldarg.0
    IL_0076:  ldc.i4.1
    IL_0077:  add
    IL_0078:  starg.s    i
    IL_007a:  br.s       IL_0015
  } // end of method Switch::SwitchInLoop

  .method public hidebysig static void  SwitchWithGoto(int32 i) cil managed
  {
    // Code size       104 (0x68)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldstr      "SwitchWithGoto: "
    IL_0005:  ldarg.0
    IL_0006:  box        [mscorlib]System.Int32
    IL_000b:  call       string [mscorlib]System.String::Concat(object,
                                                                object)
    IL_0010:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0015:  ldarg.0
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4.1
    IL_0019:  sub
    IL_001a:  switch     ( 
                          IL_0031,
                          IL_003d,
                          IL_0047,
                          IL_0052)
    IL_002f:  br.s       IL_005d

    IL_0031:  ldstr      "one"
    IL_0036:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003b:  br.s       IL_005d

    IL_003d:  ldstr      "two"
    IL_0042:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0047:  ldstr      "three"
    IL_004c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0051:  ret

    IL_0052:  ldstr      "four"
    IL_0057:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005c:  ret

    IL_005d:  ldstr      "default"
    IL_0062:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0067:  ret
  } // end of method Switch::SwitchWithGoto

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch

.class private auto ansi '<PrivateImplementationDetails>{30E98C35-5F99-4742-941F-78E7F27D8BD5}'
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field static assembly class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '$$method0x6000007-1'
  .field static assembly class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '$$method0x6000008-1'
} // end of class '<PrivateImplementationDetails>{30E98C35-5F99-4742-941F-78E7F27D8BD5}'


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\Switch.opt.res
