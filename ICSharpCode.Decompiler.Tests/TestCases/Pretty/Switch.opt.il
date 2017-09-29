
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly gz2l4xfo
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module gz2l4xfo.dll
// MVID: {FFA858C4-FC28-4EB1-BDB5-C80B304AD168}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x008F0000


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
    IL_001a:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{FFA858C4-FC28-4EB1-BDB5-C80B304AD168}'::'$$method0x6000003-1'
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
    IL_007d:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{FFA858C4-FC28-4EB1-BDB5-C80B304AD168}'::'$$method0x6000003-1'
    IL_0082:  volatile.
    IL_0084:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{FFA858C4-FC28-4EB1-BDB5-C80B304AD168}'::'$$method0x6000003-1'
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
    // Code size       323 (0x143)
    .maxstack  4
    .locals init (string V_0,
             int32 V_1)
    IL_0000:  ldstr      "SwitchOverString2:"
    IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000a:  call       string [mscorlib]System.Environment::get_UserName()
    IL_000f:  dup
    IL_0010:  stloc.0
    IL_0011:  brfalse    IL_013d

    IL_0016:  volatile.
    IL_0018:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{FFA858C4-FC28-4EB1-BDB5-C80B304AD168}'::'$$method0x6000004-1'
    IL_001d:  brtrue     IL_00b6

    IL_0022:  ldc.i4.s   11
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
    IL_0042:  ldstr      "Third case"
    IL_0047:  ldc.i4.2
    IL_0048:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_004d:  dup
    IL_004e:  ldstr      "Fourth case"
    IL_0053:  ldc.i4.3
    IL_0054:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0059:  dup
    IL_005a:  ldstr      "Fifth case"
    IL_005f:  ldc.i4.4
    IL_0060:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0065:  dup
    IL_0066:  ldstr      "Sixth case"
    IL_006b:  ldc.i4.5
    IL_006c:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0071:  dup
    IL_0072:  ldstr      "Seventh case"
    IL_0077:  ldc.i4.6
    IL_0078:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_007d:  dup
    IL_007e:  ldstr      "Eighth case"
    IL_0083:  ldc.i4.7
    IL_0084:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0089:  dup
    IL_008a:  ldstr      "Ninth case"
    IL_008f:  ldc.i4.8
    IL_0090:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0095:  dup
    IL_0096:  ldstr      "Tenth case"
    IL_009b:  ldc.i4.s   9
    IL_009d:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_00a2:  dup
    IL_00a3:  ldstr      "Eleventh case"
    IL_00a8:  ldc.i4.s   10
    IL_00aa:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_00af:  volatile.
    IL_00b1:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{FFA858C4-FC28-4EB1-BDB5-C80B304AD168}'::'$$method0x6000004-1'
    IL_00b6:  volatile.
    IL_00b8:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{FFA858C4-FC28-4EB1-BDB5-C80B304AD168}'::'$$method0x6000004-1'
    IL_00bd:  ldloc.0
    IL_00be:  ldloca.s   V_1
    IL_00c0:  call       instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::TryGetValue(!0,
                                                                                                                          !1&)
    IL_00c5:  brfalse.s  IL_013d

    IL_00c7:  ldloc.1
    IL_00c8:  switch     ( 
                          IL_00fb,
                          IL_0101,
                          IL_0107,
                          IL_010d,
                          IL_0113,
                          IL_0119,
                          IL_011f,
                          IL_0125,
                          IL_012b,
                          IL_0131,
                          IL_0137)
    IL_00f9:  br.s       IL_013d

    IL_00fb:  ldstr      "Text1"
    IL_0100:  ret

    IL_0101:  ldstr      "Text2"
    IL_0106:  ret

    IL_0107:  ldstr      "Text3"
    IL_010c:  ret

    IL_010d:  ldstr      "Text4"
    IL_0112:  ret

    IL_0113:  ldstr      "Text5"
    IL_0118:  ret

    IL_0119:  ldstr      "Text6"
    IL_011e:  ret

    IL_011f:  ldstr      "Text7"
    IL_0124:  ret

    IL_0125:  ldstr      "Text8"
    IL_012a:  ret

    IL_012b:  ldstr      "Text9"
    IL_0130:  ret

    IL_0131:  ldstr      "Text10"
    IL_0136:  ret

    IL_0137:  ldstr      "Text11"
    IL_013c:  ret

    IL_013d:  ldstr      "Default"
    IL_0142:  ret
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

.class private auto ansi '<PrivateImplementationDetails>{FFA858C4-FC28-4EB1-BDB5-C80B304AD168}'
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field static assembly class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '$$method0x6000003-1'
  .field static assembly class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '$$method0x6000004-1'
} // end of class '<PrivateImplementationDetails>{FFA858C4-FC28-4EB1-BDB5-C80B304AD168}'


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\Switch.opt.res
