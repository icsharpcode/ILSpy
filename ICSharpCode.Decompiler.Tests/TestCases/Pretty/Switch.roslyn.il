
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly Switch
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
.module Switch.dll
// MVID: {08E636DF-9FF0-4BF4-9F8F-69859C40E218}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02E70000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch
       extends [mscorlib]System.Object
{
  .method public hidebysig static string 
          SparseIntegerSwitch(int32 i) cil managed
  {
    // Code size       238 (0xee)
    .maxstack  2
    .locals init (int32 V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldstr      "SparseIntegerSwitch: "
    IL_0006:  ldarg.0
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  call       string [mscorlib]System.String::Concat(object,
                                                                object)
    IL_0011:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0016:  nop
    IL_0017:  ldarg.0
    IL_0018:  stloc.0
    IL_0019:  ldloc.0
    IL_001a:  ldc.i4.4
    IL_001b:  bgt.s      IL_0053

    IL_001d:  ldloc.0
    IL_001e:  ldc.i4     0xff676980
    IL_0023:  beq.s      IL_0080

    IL_0025:  br.s       IL_0027

    IL_0027:  ldloc.0
    IL_0028:  ldc.i4.s   -100
    IL_002a:  beq.s      IL_0089

    IL_002c:  br.s       IL_002e

    IL_002e:  ldloc.0
    IL_002f:  ldc.i4.m1
    IL_0030:  sub
    IL_0031:  switch     ( 
                          IL_0092,
                          IL_009b,
                          IL_00a4,
                          IL_00ad,
                          IL_00e3,
                          IL_00b6)
    IL_004e:  br         IL_00e3

    IL_0053:  ldloc.0
    IL_0054:  ldc.i4     0x2710
    IL_0059:  bgt.s      IL_006c

    IL_005b:  ldloc.0
    IL_005c:  ldc.i4.s   100
    IL_005e:  beq.s      IL_00bf

    IL_0060:  br.s       IL_0062

    IL_0062:  ldloc.0
    IL_0063:  ldc.i4     0x2710
    IL_0068:  beq.s      IL_00c8

    IL_006a:  br.s       IL_00e3

    IL_006c:  ldloc.0
    IL_006d:  ldc.i4     0x2711
    IL_0072:  beq.s      IL_00d1

    IL_0074:  br.s       IL_0076

    IL_0076:  ldloc.0
    IL_0077:  ldc.i4     0x7fffffff
    IL_007c:  beq.s      IL_00da

    IL_007e:  br.s       IL_00e3

    IL_0080:  nop
    IL_0081:  ldstr      "-10 mln"
    IL_0086:  stloc.1
    IL_0087:  br.s       IL_00ec

    IL_0089:  nop
    IL_008a:  ldstr      "-hundred"
    IL_008f:  stloc.1
    IL_0090:  br.s       IL_00ec

    IL_0092:  nop
    IL_0093:  ldstr      "-1"
    IL_0098:  stloc.1
    IL_0099:  br.s       IL_00ec

    IL_009b:  nop
    IL_009c:  ldstr      "0"
    IL_00a1:  stloc.1
    IL_00a2:  br.s       IL_00ec

    IL_00a4:  nop
    IL_00a5:  ldstr      "1"
    IL_00aa:  stloc.1
    IL_00ab:  br.s       IL_00ec

    IL_00ad:  nop
    IL_00ae:  ldstr      "2"
    IL_00b3:  stloc.1
    IL_00b4:  br.s       IL_00ec

    IL_00b6:  nop
    IL_00b7:  ldstr      "4"
    IL_00bc:  stloc.1
    IL_00bd:  br.s       IL_00ec

    IL_00bf:  nop
    IL_00c0:  ldstr      "hundred"
    IL_00c5:  stloc.1
    IL_00c6:  br.s       IL_00ec

    IL_00c8:  nop
    IL_00c9:  ldstr      "ten thousand"
    IL_00ce:  stloc.1
    IL_00cf:  br.s       IL_00ec

    IL_00d1:  nop
    IL_00d2:  ldstr      "ten thousand and one"
    IL_00d7:  stloc.1
    IL_00d8:  br.s       IL_00ec

    IL_00da:  nop
    IL_00db:  ldstr      "int.MaxValue"
    IL_00e0:  stloc.1
    IL_00e1:  br.s       IL_00ec

    IL_00e3:  nop
    IL_00e4:  ldstr      "something else"
    IL_00e9:  stloc.1
    IL_00ea:  br.s       IL_00ec

    IL_00ec:  ldloc.1
    IL_00ed:  ret
  } // end of method Switch::SparseIntegerSwitch

  .method public hidebysig static string 
          ShortSwitchOverString(string text) cil managed
  {
    // Code size       99 (0x63)
    .maxstack  2
    .locals init (string V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldstr      "ShortSwitchOverString: "
    IL_0006:  ldarg.0
    IL_0007:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0011:  nop
    IL_0012:  ldarg.0
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  ldstr      "First case"
    IL_001a:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_001f:  brtrue.s   IL_003d

    IL_0021:  ldloc.0
    IL_0022:  ldstr      "Second case"
    IL_0027:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_002c:  brtrue.s   IL_0046

    IL_002e:  ldloc.0
    IL_002f:  ldstr      "Third case"
    IL_0034:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0039:  brtrue.s   IL_004f

    IL_003b:  br.s       IL_0058

    IL_003d:  nop
    IL_003e:  ldstr      "Text1"
    IL_0043:  stloc.1
    IL_0044:  br.s       IL_0061

    IL_0046:  nop
    IL_0047:  ldstr      "Text2"
    IL_004c:  stloc.1
    IL_004d:  br.s       IL_0061

    IL_004f:  nop
    IL_0050:  ldstr      "Text3"
    IL_0055:  stloc.1
    IL_0056:  br.s       IL_0061

    IL_0058:  nop
    IL_0059:  ldstr      "Default"
    IL_005e:  stloc.1
    IL_005f:  br.s       IL_0061

    IL_0061:  ldloc.1
    IL_0062:  ret
  } // end of method Switch::ShortSwitchOverString

  .method public hidebysig static string 
          SwitchOverString1(string text) cil managed
  {
    // Code size       333 (0x14d)
    .maxstack  2
    .locals init (string V_0,
             uint32 V_1,
             string V_2)
    IL_0000:  nop
    IL_0001:  ldstr      "SwitchOverString1: "
    IL_0006:  ldarg.0
    IL_0007:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0011:  nop
    IL_0012:  ldarg.0
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  call       uint32 '<PrivateImplementationDetails>'::ComputeStringHash(string)
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4     0xf3d44a6
    IL_0021:  bgt.un.s   IL_005a

    IL_0023:  ldloc.1
    IL_0024:  ldc.i4     0x8861b86
    IL_0029:  bgt.un.s   IL_0043

    IL_002b:  ldloc.1
    IL_002c:  brfalse    IL_0102

    IL_0031:  br.s       IL_0033

    IL_0033:  ldloc.1
    IL_0034:  ldc.i4     0x8861b86
    IL_0039:  beq        IL_00e4

    IL_003e:  br         IL_0142

    IL_0043:  ldloc.1
    IL_0044:  ldc.i4     0xc9a8f4f
    IL_0049:  beq.s      IL_0093

    IL_004b:  br.s       IL_004d

    IL_004d:  ldloc.1
    IL_004e:  ldc.i4     0xf3d44a6
    IL_0053:  beq.s      IL_00c6

    IL_0055:  br         IL_0142

    IL_005a:  ldloc.1
    IL_005b:  ldc.i4     0x652a1179
    IL_0060:  bgt.un.s   IL_007c

    IL_0062:  ldloc.1
    IL_0063:  ldc.i4     0x51650fb9
    IL_0068:  beq        IL_00f3

    IL_006d:  br.s       IL_006f

    IL_006f:  ldloc.1
    IL_0070:  ldc.i4     0x652a1179
    IL_0075:  beq.s      IL_00b7

    IL_0077:  br         IL_0142

    IL_007c:  ldloc.1
    IL_007d:  ldc.i4     0xea3d096b
    IL_0082:  beq.s      IL_00a5

    IL_0084:  br.s       IL_0086

    IL_0086:  ldloc.1
    IL_0087:  ldc.i4     0xf701cc7f
    IL_008c:  beq.s      IL_00d5

    IL_008e:  br         IL_0142

    IL_0093:  ldloc.0
    IL_0094:  ldstr      "First case"
    IL_0099:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_009e:  brtrue.s   IL_0107

    IL_00a0:  br         IL_0142

    IL_00a5:  ldloc.0
    IL_00a6:  ldstr      "Second case"
    IL_00ab:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00b0:  brtrue.s   IL_0110

    IL_00b2:  br         IL_0142

    IL_00b7:  ldloc.0
    IL_00b8:  ldstr      "2nd case"
    IL_00bd:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00c2:  brtrue.s   IL_0110

    IL_00c4:  br.s       IL_0142

    IL_00c6:  ldloc.0
    IL_00c7:  ldstr      "Third case"
    IL_00cc:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00d1:  brtrue.s   IL_0119

    IL_00d3:  br.s       IL_0142

    IL_00d5:  ldloc.0
    IL_00d6:  ldstr      "Fourth case"
    IL_00db:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00e0:  brtrue.s   IL_0122

    IL_00e2:  br.s       IL_0142

    IL_00e4:  ldloc.0
    IL_00e5:  ldstr      "Fifth case"
    IL_00ea:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00ef:  brtrue.s   IL_012b

    IL_00f1:  br.s       IL_0142

    IL_00f3:  ldloc.0
    IL_00f4:  ldstr      "Sixth case"
    IL_00f9:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00fe:  brtrue.s   IL_0134

    IL_0100:  br.s       IL_0142

    IL_0102:  ldloc.0
    IL_0103:  brfalse.s  IL_013d

    IL_0105:  br.s       IL_0142

    IL_0107:  nop
    IL_0108:  ldstr      "Text1"
    IL_010d:  stloc.2
    IL_010e:  br.s       IL_014b

    IL_0110:  nop
    IL_0111:  ldstr      "Text2"
    IL_0116:  stloc.2
    IL_0117:  br.s       IL_014b

    IL_0119:  nop
    IL_011a:  ldstr      "Text3"
    IL_011f:  stloc.2
    IL_0120:  br.s       IL_014b

    IL_0122:  nop
    IL_0123:  ldstr      "Text4"
    IL_0128:  stloc.2
    IL_0129:  br.s       IL_014b

    IL_012b:  nop
    IL_012c:  ldstr      "Text5"
    IL_0131:  stloc.2
    IL_0132:  br.s       IL_014b

    IL_0134:  nop
    IL_0135:  ldstr      "Text6"
    IL_013a:  stloc.2
    IL_013b:  br.s       IL_014b

    IL_013d:  nop
    IL_013e:  ldnull
    IL_013f:  stloc.2
    IL_0140:  br.s       IL_014b

    IL_0142:  nop
    IL_0143:  ldstr      "Default"
    IL_0148:  stloc.2
    IL_0149:  br.s       IL_014b

    IL_014b:  ldloc.2
    IL_014c:  ret
  } // end of method Switch::SwitchOverString1

  .method public hidebysig static string 
          SwitchOverString2() cil managed
  {
    // Code size       518 (0x206)
    .maxstack  2
    .locals init (string V_0,
             uint32 V_1,
             string V_2)
    IL_0000:  nop
    IL_0001:  ldstr      "SwitchOverString2:"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  call       string [mscorlib]System.Environment::get_UserName()
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  call       uint32 '<PrivateImplementationDetails>'::ComputeStringHash(string)
    IL_0018:  stloc.1
    IL_0019:  ldloc.1
    IL_001a:  ldc.i4     0x4c7c71f6
    IL_001f:  bgt.un.s   IL_0070

    IL_0021:  ldloc.1
    IL_0022:  ldc.i4     0xc9a8f4f
    IL_0027:  bgt.un.s   IL_0046

    IL_0029:  ldloc.1
    IL_002a:  ldc.i4     0x8861b86
    IL_002f:  beq        IL_011a

    IL_0034:  br.s       IL_0036

    IL_0036:  ldloc.1
    IL_0037:  ldc.i4     0xc9a8f4f
    IL_003c:  beq        IL_00c6

    IL_0041:  br         IL_01fb

    IL_0046:  ldloc.1
    IL_0047:  ldc.i4     0xf3d44a6
    IL_004c:  beq        IL_00f0

    IL_0051:  br.s       IL_0053

    IL_0053:  ldloc.1
    IL_0054:  ldc.i4     0x20289804
    IL_0059:  beq        IL_0156

    IL_005e:  br.s       IL_0060

    IL_0060:  ldloc.1
    IL_0061:  ldc.i4     0x4c7c71f6
    IL_0066:  beq        IL_0168

    IL_006b:  br         IL_01fb

    IL_0070:  ldloc.1
    IL_0071:  ldc.i4     0xa151b28a
    IL_0076:  bgt.un.s   IL_00a2

    IL_0078:  ldloc.1
    IL_0079:  ldc.i4     0x4d0cea48
    IL_007e:  beq        IL_0189

    IL_0083:  br.s       IL_0085

    IL_0085:  ldloc.1
    IL_0086:  ldc.i4     0x51650fb9
    IL_008b:  beq        IL_012f

    IL_0090:  br.s       IL_0092

    IL_0092:  ldloc.1
    IL_0093:  ldc.i4     0xa151b28a
    IL_0098:  beq        IL_0144

    IL_009d:  br         IL_01fb

    IL_00a2:  ldloc.1
    IL_00a3:  ldc.i4     0xea3d096b
    IL_00a8:  beq.s      IL_00db

    IL_00aa:  br.s       IL_00ac

    IL_00ac:  ldloc.1
    IL_00ad:  ldc.i4     0xed5134d4
    IL_00b2:  beq        IL_017a

    IL_00b7:  br.s       IL_00b9

    IL_00b9:  ldloc.1
    IL_00ba:  ldc.i4     0xf701cc7f
    IL_00bf:  beq.s      IL_0105

    IL_00c1:  br         IL_01fb

    IL_00c6:  ldloc.0
    IL_00c7:  ldstr      "First case"
    IL_00cc:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00d1:  brtrue     IL_0198

    IL_00d6:  br         IL_01fb

    IL_00db:  ldloc.0
    IL_00dc:  ldstr      "Second case"
    IL_00e1:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00e6:  brtrue     IL_01a1

    IL_00eb:  br         IL_01fb

    IL_00f0:  ldloc.0
    IL_00f1:  ldstr      "Third case"
    IL_00f6:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00fb:  brtrue     IL_01aa

    IL_0100:  br         IL_01fb

    IL_0105:  ldloc.0
    IL_0106:  ldstr      "Fourth case"
    IL_010b:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0110:  brtrue     IL_01b3

    IL_0115:  br         IL_01fb

    IL_011a:  ldloc.0
    IL_011b:  ldstr      "Fifth case"
    IL_0120:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0125:  brtrue     IL_01bc

    IL_012a:  br         IL_01fb

    IL_012f:  ldloc.0
    IL_0130:  ldstr      "Sixth case"
    IL_0135:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_013a:  brtrue     IL_01c5

    IL_013f:  br         IL_01fb

    IL_0144:  ldloc.0
    IL_0145:  ldstr      "Seventh case"
    IL_014a:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_014f:  brtrue.s   IL_01ce

    IL_0151:  br         IL_01fb

    IL_0156:  ldloc.0
    IL_0157:  ldstr      "Eighth case"
    IL_015c:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0161:  brtrue.s   IL_01d7

    IL_0163:  br         IL_01fb

    IL_0168:  ldloc.0
    IL_0169:  ldstr      "Ninth case"
    IL_016e:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0173:  brtrue.s   IL_01e0

    IL_0175:  br         IL_01fb

    IL_017a:  ldloc.0
    IL_017b:  ldstr      "Tenth case"
    IL_0180:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0185:  brtrue.s   IL_01e9

    IL_0187:  br.s       IL_01fb

    IL_0189:  ldloc.0
    IL_018a:  ldstr      "Eleventh case"
    IL_018f:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0194:  brtrue.s   IL_01f2

    IL_0196:  br.s       IL_01fb

    IL_0198:  nop
    IL_0199:  ldstr      "Text1"
    IL_019e:  stloc.2
    IL_019f:  br.s       IL_0204

    IL_01a1:  nop
    IL_01a2:  ldstr      "Text2"
    IL_01a7:  stloc.2
    IL_01a8:  br.s       IL_0204

    IL_01aa:  nop
    IL_01ab:  ldstr      "Text3"
    IL_01b0:  stloc.2
    IL_01b1:  br.s       IL_0204

    IL_01b3:  nop
    IL_01b4:  ldstr      "Text4"
    IL_01b9:  stloc.2
    IL_01ba:  br.s       IL_0204

    IL_01bc:  nop
    IL_01bd:  ldstr      "Text5"
    IL_01c2:  stloc.2
    IL_01c3:  br.s       IL_0204

    IL_01c5:  nop
    IL_01c6:  ldstr      "Text6"
    IL_01cb:  stloc.2
    IL_01cc:  br.s       IL_0204

    IL_01ce:  nop
    IL_01cf:  ldstr      "Text7"
    IL_01d4:  stloc.2
    IL_01d5:  br.s       IL_0204

    IL_01d7:  nop
    IL_01d8:  ldstr      "Text8"
    IL_01dd:  stloc.2
    IL_01de:  br.s       IL_0204

    IL_01e0:  nop
    IL_01e1:  ldstr      "Text9"
    IL_01e6:  stloc.2
    IL_01e7:  br.s       IL_0204

    IL_01e9:  nop
    IL_01ea:  ldstr      "Text10"
    IL_01ef:  stloc.2
    IL_01f0:  br.s       IL_0204

    IL_01f2:  nop
    IL_01f3:  ldstr      "Text11"
    IL_01f8:  stloc.2
    IL_01f9:  br.s       IL_0204

    IL_01fb:  nop
    IL_01fc:  ldstr      "Default"
    IL_0201:  stloc.2
    IL_0202:  br.s       IL_0204

    IL_0204:  ldloc.2
    IL_0205:  ret
  } // end of method Switch::SwitchOverString2

  .method public hidebysig static string 
          SwitchOverBool(bool b) cil managed
  {
    // Code size       62 (0x3e)
    .maxstack  2
    .locals init (bool V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldstr      "SwitchOverBool: "
    IL_0006:  ldarga.s   b
    IL_0008:  call       instance string [mscorlib]System.Boolean::ToString()
    IL_000d:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0012:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0017:  nop
    IL_0018:  ldarg.0
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  brfalse.s  IL_002e

    IL_001d:  br.s       IL_001f

    IL_001f:  ldloc.0
    IL_0020:  ldc.i4.1
    IL_0021:  beq.s      IL_0025

    IL_0023:  br.s       IL_0037

    IL_0025:  nop
    IL_0026:  ldsfld     string [mscorlib]System.Boolean::TrueString
    IL_002b:  stloc.1
    IL_002c:  br.s       IL_003c

    IL_002e:  nop
    IL_002f:  ldsfld     string [mscorlib]System.Boolean::FalseString
    IL_0034:  stloc.1
    IL_0035:  br.s       IL_003c

    IL_0037:  nop
    IL_0038:  ldnull
    IL_0039:  stloc.1
    IL_003a:  br.s       IL_003c

    IL_003c:  ldloc.1
    IL_003d:  ret
  } // end of method Switch::SwitchOverBool

  .method public hidebysig static void  SwitchInLoop(int32 i) cil managed
  {
    // Code size       141 (0x8d)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldstr      "SwitchInLoop: "
    IL_0006:  ldarg.0
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  call       string [mscorlib]System.String::Concat(object,
                                                                object)
    IL_0011:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0016:  nop
    IL_0017:  br.s       IL_0088

    IL_0019:  nop
    IL_001a:  ldarg.0
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.1
    IL_001e:  sub
    IL_001f:  switch     ( 
                          IL_0036,
                          IL_0043,
                          IL_0050,
                          IL_005d)
    IL_0034:  br.s       IL_006a

    IL_0036:  ldstr      "one"
    IL_003b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0040:  nop
    IL_0041:  br.s       IL_0082

    IL_0043:  ldstr      "two"
    IL_0048:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004d:  nop
    IL_004e:  br.s       IL_0082

    IL_0050:  ldstr      "three"
    IL_0055:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005a:  nop
    IL_005b:  br.s       IL_0088

    IL_005d:  ldstr      "four"
    IL_0062:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0067:  nop
    IL_0068:  br.s       IL_008c

    IL_006a:  ldstr      "default"
    IL_006f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0074:  nop
    IL_0075:  ldstr      "more code"
    IL_007a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007f:  nop
    IL_0080:  br.s       IL_008c

    IL_0082:  ldarg.0
    IL_0083:  ldc.i4.1
    IL_0084:  add
    IL_0085:  starg.s    i
    IL_0087:  nop
    IL_0088:  ldc.i4.1
    IL_0089:  stloc.1
    IL_008a:  br.s       IL_0019

    IL_008c:  ret
  } // end of method Switch::SwitchInLoop

  .method public hidebysig static void  SwitchWithGoto(int32 i) cil managed
  {
    // Code size       117 (0x75)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldstr      "SwitchWithGoto: "
    IL_0006:  ldarg.0
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  call       string [mscorlib]System.String::Concat(object,
                                                                object)
    IL_0011:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0016:  nop
    IL_0017:  ldarg.0
    IL_0018:  stloc.0
    IL_0019:  ldloc.0
    IL_001a:  ldc.i4.1
    IL_001b:  sub
    IL_001c:  switch     ( 
                          IL_0033,
                          IL_0040,
                          IL_004d,
                          IL_005a)
    IL_0031:  br.s       IL_0067

    IL_0033:  ldstr      "one"
    IL_0038:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003d:  nop
    IL_003e:  br.s       IL_0067

    IL_0040:  ldstr      "two"
    IL_0045:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004a:  nop
    IL_004b:  br.s       IL_004d

    IL_004d:  ldstr      "three"
    IL_0052:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0057:  nop
    IL_0058:  br.s       IL_0074

    IL_005a:  ldstr      "four"
    IL_005f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0064:  nop
    IL_0065:  br.s       IL_0074

    IL_0067:  ldstr      "default"
    IL_006c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0071:  nop
    IL_0072:  br.s       IL_0074

    IL_0074:  ret
  } // end of method Switch::SwitchWithGoto

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch

.class private auto ansi sealed '<PrivateImplementationDetails>'
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method assembly hidebysig static uint32 
          ComputeStringHash(string s) cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  2
    .locals init (uint32 V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_002c

    IL_0003:  ldc.i4     0x811c9dc5
    IL_0008:  stloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  stloc.1
    IL_000b:  br.s       IL_0021

    IL_000d:  ldarg.0
    IL_000e:  ldloc.1
    IL_000f:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0014:  ldloc.0
    IL_0015:  xor
    IL_0016:  ldc.i4     0x1000193
    IL_001b:  mul
    IL_001c:  stloc.0
    IL_001d:  ldloc.1
    IL_001e:  ldc.i4.1
    IL_001f:  add
    IL_0020:  stloc.1
    IL_0021:  ldloc.1
    IL_0022:  ldarg.0
    IL_0023:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_0028:  bge.s      IL_002c

    IL_002a:  br.s       IL_000d

    IL_002c:  ldloc.0
    IL_002d:  ret
  } // end of method '<PrivateImplementationDetails>'::ComputeStringHash

} // end of class '<PrivateImplementationDetails>'


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
