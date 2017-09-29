
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly wau4yvxd
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module wau4yvxd.dll
// MVID: {102ACC94-685A-42C5-9229-AC386C0A78B1}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x01440000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch
       extends [mscorlib]System.Object
{
  .method public hidebysig static string 
          SparseIntegerSwitch(int32 i) cil managed
  {
    // Code size       224 (0xe0)
    .maxstack  2
    .locals init (string V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldstr      "SparseIntegerSwitch: "
    IL_0006:  ldarg.0
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  call       string [mscorlib]System.String::Concat(object,
                                                                object)
    IL_0011:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0016:  nop
    IL_0017:  ldarg.0
    IL_0018:  stloc.1
    IL_0019:  ldloc.1
    IL_001a:  ldc.i4.4
    IL_001b:  bgt.s      IL_004f

    IL_001d:  ldloc.1
    IL_001e:  ldc.i4     0xff676980
    IL_0023:  beq.s      IL_0072

    IL_0025:  ldloc.1
    IL_0026:  ldc.i4.s   -100
    IL_0028:  beq.s      IL_007b

    IL_002a:  ldloc.1
    IL_002b:  ldc.i4.m1
    IL_002c:  sub
    IL_002d:  switch     ( 
                          IL_0084,
                          IL_008d,
                          IL_0096,
                          IL_009f,
                          IL_00d5,
                          IL_00a8)
    IL_004a:  br         IL_00d5

    IL_004f:  ldloc.1
    IL_0050:  ldc.i4.s   100
    IL_0052:  beq.s      IL_00b1

    IL_0054:  ldloc.1
    IL_0055:  ldc.i4     0x2710
    IL_005a:  sub
    IL_005b:  switch     ( 
                          IL_00ba,
                          IL_00c3)
    IL_0068:  ldloc.1
    IL_0069:  ldc.i4     0x7fffffff
    IL_006e:  beq.s      IL_00cc

    IL_0070:  br.s       IL_00d5

    IL_0072:  nop
    IL_0073:  ldstr      "-10 mln"
    IL_0078:  stloc.0
    IL_0079:  br.s       IL_00de

    IL_007b:  nop
    IL_007c:  ldstr      "-hundred"
    IL_0081:  stloc.0
    IL_0082:  br.s       IL_00de

    IL_0084:  nop
    IL_0085:  ldstr      "-1"
    IL_008a:  stloc.0
    IL_008b:  br.s       IL_00de

    IL_008d:  nop
    IL_008e:  ldstr      "0"
    IL_0093:  stloc.0
    IL_0094:  br.s       IL_00de

    IL_0096:  nop
    IL_0097:  ldstr      "1"
    IL_009c:  stloc.0
    IL_009d:  br.s       IL_00de

    IL_009f:  nop
    IL_00a0:  ldstr      "2"
    IL_00a5:  stloc.0
    IL_00a6:  br.s       IL_00de

    IL_00a8:  nop
    IL_00a9:  ldstr      "4"
    IL_00ae:  stloc.0
    IL_00af:  br.s       IL_00de

    IL_00b1:  nop
    IL_00b2:  ldstr      "hundred"
    IL_00b7:  stloc.0
    IL_00b8:  br.s       IL_00de

    IL_00ba:  nop
    IL_00bb:  ldstr      "ten thousand"
    IL_00c0:  stloc.0
    IL_00c1:  br.s       IL_00de

    IL_00c3:  nop
    IL_00c4:  ldstr      "ten thousand and one"
    IL_00c9:  stloc.0
    IL_00ca:  br.s       IL_00de

    IL_00cc:  nop
    IL_00cd:  ldstr      "int.MaxValue"
    IL_00d2:  stloc.0
    IL_00d3:  br.s       IL_00de

    IL_00d5:  nop
    IL_00d6:  ldstr      "something else"
    IL_00db:  stloc.0
    IL_00dc:  br.s       IL_00de

    IL_00de:  ldloc.0
    IL_00df:  ret
  } // end of method Switch::SparseIntegerSwitch

  .method public hidebysig static string 
          ShortSwitchOverString(string text) cil managed
  {
    // Code size       102 (0x66)
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
    IL_0013:  stloc.1
    IL_0014:  ldloc.1
    IL_0015:  brfalse.s  IL_005b

    IL_0017:  ldloc.1
    IL_0018:  ldstr      "First case"
    IL_001d:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0022:  brtrue.s   IL_0040

    IL_0024:  ldloc.1
    IL_0025:  ldstr      "Second case"
    IL_002a:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_002f:  brtrue.s   IL_0049

    IL_0031:  ldloc.1
    IL_0032:  ldstr      "Third case"
    IL_0037:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_003c:  brtrue.s   IL_0052

    IL_003e:  br.s       IL_005b

    IL_0040:  nop
    IL_0041:  ldstr      "Text1"
    IL_0046:  stloc.0
    IL_0047:  br.s       IL_0064

    IL_0049:  nop
    IL_004a:  ldstr      "Text2"
    IL_004f:  stloc.0
    IL_0050:  br.s       IL_0064

    IL_0052:  nop
    IL_0053:  ldstr      "Text3"
    IL_0058:  stloc.0
    IL_0059:  br.s       IL_0064

    IL_005b:  nop
    IL_005c:  ldstr      "Default"
    IL_0061:  stloc.0
    IL_0062:  br.s       IL_0064

    IL_0064:  ldloc.0
    IL_0065:  ret
  } // end of method Switch::ShortSwitchOverString

  .method public hidebysig static string 
          SwitchOverString1(string text) cil managed
  {
    // Code size       255 (0xff)
    .maxstack  4
    .locals init (string V_0,
             string V_1,
             int32 V_2)
    IL_0000:  nop
    IL_0001:  ldstr      "SwitchOverString1: "
    IL_0006:  ldarg.0
    IL_0007:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0011:  nop
    IL_0012:  ldarg.0
    IL_0013:  stloc.1
    IL_0014:  ldloc.1
    IL_0015:  brfalse    IL_00ef

    IL_001a:  volatile.
    IL_001c:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{102ACC94-685A-42C5-9229-AC386C0A78B1}'::'$$method0x6000003-1'
    IL_0021:  brtrue.s   IL_0084

    IL_0023:  ldc.i4.7
    IL_0024:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::.ctor(int32)
    IL_0029:  dup
    IL_002a:  ldstr      "First case"
    IL_002f:  ldc.i4.0
    IL_0030:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0035:  dup
    IL_0036:  ldstr      "Second case"
    IL_003b:  ldc.i4.1
    IL_003c:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0041:  dup
    IL_0042:  ldstr      "2nd case"
    IL_0047:  ldc.i4.2
    IL_0048:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_004d:  dup
    IL_004e:  ldstr      "Third case"
    IL_0053:  ldc.i4.3
    IL_0054:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0059:  dup
    IL_005a:  ldstr      "Fourth case"
    IL_005f:  ldc.i4.4
    IL_0060:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0065:  dup
    IL_0066:  ldstr      "Fifth case"
    IL_006b:  ldc.i4.5
    IL_006c:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0071:  dup
    IL_0072:  ldstr      "Sixth case"
    IL_0077:  ldc.i4.6
    IL_0078:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_007d:  volatile.
    IL_007f:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{102ACC94-685A-42C5-9229-AC386C0A78B1}'::'$$method0x6000003-1'
    IL_0084:  volatile.
    IL_0086:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{102ACC94-685A-42C5-9229-AC386C0A78B1}'::'$$method0x6000003-1'
    IL_008b:  ldloc.1
    IL_008c:  ldloca.s   V_2
    IL_008e:  call       instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::TryGetValue(!0,
                                                                                                                          !1&)
    IL_0093:  brfalse.s  IL_00f4

    IL_0095:  ldloc.2
    IL_0096:  switch     ( 
                          IL_00b9,
                          IL_00c2,
                          IL_00c2,
                          IL_00cb,
                          IL_00d4,
                          IL_00dd,
                          IL_00e6)
    IL_00b7:  br.s       IL_00f4

    IL_00b9:  nop
    IL_00ba:  ldstr      "Text1"
    IL_00bf:  stloc.0
    IL_00c0:  br.s       IL_00fd

    IL_00c2:  nop
    IL_00c3:  ldstr      "Text2"
    IL_00c8:  stloc.0
    IL_00c9:  br.s       IL_00fd

    IL_00cb:  nop
    IL_00cc:  ldstr      "Text3"
    IL_00d1:  stloc.0
    IL_00d2:  br.s       IL_00fd

    IL_00d4:  nop
    IL_00d5:  ldstr      "Text4"
    IL_00da:  stloc.0
    IL_00db:  br.s       IL_00fd

    IL_00dd:  nop
    IL_00de:  ldstr      "Text5"
    IL_00e3:  stloc.0
    IL_00e4:  br.s       IL_00fd

    IL_00e6:  nop
    IL_00e7:  ldstr      "Text6"
    IL_00ec:  stloc.0
    IL_00ed:  br.s       IL_00fd

    IL_00ef:  nop
    IL_00f0:  ldnull
    IL_00f1:  stloc.0
    IL_00f2:  br.s       IL_00fd

    IL_00f4:  nop
    IL_00f5:  ldstr      "Default"
    IL_00fa:  stloc.0
    IL_00fb:  br.s       IL_00fd

    IL_00fd:  ldloc.0
    IL_00fe:  ret
  } // end of method Switch::SwitchOverString1

  .method public hidebysig static string 
          SwitchOverString2() cil managed
  {
    // Code size       366 (0x16e)
    .maxstack  4
    .locals init (string V_0,
             string V_1,
             int32 V_2)
    IL_0000:  nop
    IL_0001:  ldstr      "SwitchOverString2:"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  call       string [mscorlib]System.Environment::get_UserName()
    IL_0011:  stloc.1
    IL_0012:  ldloc.1
    IL_0013:  brfalse    IL_0163

    IL_0018:  volatile.
    IL_001a:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{102ACC94-685A-42C5-9229-AC386C0A78B1}'::'$$method0x6000004-1'
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
    IL_00b3:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{102ACC94-685A-42C5-9229-AC386C0A78B1}'::'$$method0x6000004-1'
    IL_00b8:  volatile.
    IL_00ba:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{102ACC94-685A-42C5-9229-AC386C0A78B1}'::'$$method0x6000004-1'
    IL_00bf:  ldloc.1
    IL_00c0:  ldloca.s   V_2
    IL_00c2:  call       instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::TryGetValue(!0,
                                                                                                                          !1&)
    IL_00c7:  brfalse    IL_0163

    IL_00cc:  ldloc.2
    IL_00cd:  switch     ( 
                          IL_0100,
                          IL_0109,
                          IL_0112,
                          IL_011b,
                          IL_0124,
                          IL_012d,
                          IL_0136,
                          IL_013f,
                          IL_0148,
                          IL_0151,
                          IL_015a)
    IL_00fe:  br.s       IL_0163

    IL_0100:  nop
    IL_0101:  ldstr      "Text1"
    IL_0106:  stloc.0
    IL_0107:  br.s       IL_016c

    IL_0109:  nop
    IL_010a:  ldstr      "Text2"
    IL_010f:  stloc.0
    IL_0110:  br.s       IL_016c

    IL_0112:  nop
    IL_0113:  ldstr      "Text3"
    IL_0118:  stloc.0
    IL_0119:  br.s       IL_016c

    IL_011b:  nop
    IL_011c:  ldstr      "Text4"
    IL_0121:  stloc.0
    IL_0122:  br.s       IL_016c

    IL_0124:  nop
    IL_0125:  ldstr      "Text5"
    IL_012a:  stloc.0
    IL_012b:  br.s       IL_016c

    IL_012d:  nop
    IL_012e:  ldstr      "Text6"
    IL_0133:  stloc.0
    IL_0134:  br.s       IL_016c

    IL_0136:  nop
    IL_0137:  ldstr      "Text7"
    IL_013c:  stloc.0
    IL_013d:  br.s       IL_016c

    IL_013f:  nop
    IL_0140:  ldstr      "Text8"
    IL_0145:  stloc.0
    IL_0146:  br.s       IL_016c

    IL_0148:  nop
    IL_0149:  ldstr      "Text9"
    IL_014e:  stloc.0
    IL_014f:  br.s       IL_016c

    IL_0151:  nop
    IL_0152:  ldstr      "Text10"
    IL_0157:  stloc.0
    IL_0158:  br.s       IL_016c

    IL_015a:  nop
    IL_015b:  ldstr      "Text11"
    IL_0160:  stloc.0
    IL_0161:  br.s       IL_016c

    IL_0163:  nop
    IL_0164:  ldstr      "Default"
    IL_0169:  stloc.0
    IL_016a:  br.s       IL_016c

    IL_016c:  ldloc.0
    IL_016d:  ret
  } // end of method Switch::SwitchOverString2

  .method public hidebysig static string 
          SwitchOverBool(bool b) cil managed
  {
    // Code size       67 (0x43)
    .maxstack  2
    .locals init (string V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldstr      "SwitchOverBool: "
    IL_0006:  ldarga.s   b
    IL_0008:  call       instance string [mscorlib]System.Boolean::ToString()
    IL_000d:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0012:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0017:  nop
    IL_0018:  ldarg.0
    IL_0019:  stloc.1
    IL_001a:  ldloc.1
    IL_001b:  switch     ( 
                          IL_0033,
                          IL_002a)
    IL_0028:  br.s       IL_003c

    IL_002a:  nop
    IL_002b:  ldsfld     string [mscorlib]System.Boolean::TrueString
    IL_0030:  stloc.0
    IL_0031:  br.s       IL_0041

    IL_0033:  nop
    IL_0034:  ldsfld     string [mscorlib]System.Boolean::FalseString
    IL_0039:  stloc.0
    IL_003a:  br.s       IL_0041

    IL_003c:  nop
    IL_003d:  ldnull
    IL_003e:  stloc.0
    IL_003f:  br.s       IL_0041

    IL_0041:  ldloc.0
    IL_0042:  ret
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

.class private auto ansi '<PrivateImplementationDetails>{102ACC94-685A-42C5-9229-AC386C0A78B1}'
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field static assembly class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '$$method0x6000003-1'
  .field static assembly class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '$$method0x6000004-1'
} // end of class '<PrivateImplementationDetails>{102ACC94-685A-42C5-9229-AC386C0A78B1}'


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\Switch.res
