
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v1.1.4322
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 1:0:5000:0
}
.assembly CS1xSwitch_Debug
{

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(bool,
  //                                                                                bool) = ( 01 00 00 01 00 00 ) 

  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module CS1xSwitch_Debug.dll
// MVID: {3797A9DC-06AE-42B7-994E-BDC70F8FF69B}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x07560000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch
       extends [mscorlib]System.Object
{
  .class auto ansi nested public beforefieldinit SetProperty
         extends [mscorlib]System.Object
  {
    .field public initonly class [mscorlib]System.Reflection.PropertyInfo Property
    .field private int32 _set
    .method public hidebysig specialname 
            instance int32  get_Set() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::_set
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method SetProperty::get_Set

    .method public hidebysig specialname 
            instance void  set_Set(int32 'value') cil managed
    {
      // Code size       8 (0x8)
      .maxstack  2
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::_set
      IL_0007:  ret
    } // end of method SetProperty::set_Set

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(class [mscorlib]System.Reflection.PropertyInfo 'property') cil managed
    {
      // Code size       14 (0xe)
      .maxstack  2
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      class [mscorlib]System.Reflection.PropertyInfo ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::Property
      IL_000d:  ret
    } // end of method SetProperty::.ctor

    .property instance int32 Set()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::get_Set()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    } // end of property SetProperty::Set
  } // end of class SetProperty

  .class auto ansi sealed nested public State
         extends [mscorlib]System.Enum
  {
    .field public specialname rtspecialname int32 value__
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State False = int32(0x00000000)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State True = int32(0x00000001)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State Null = int32(0x00000002)
  } // end of class State

  .method public hidebysig static string 
          SparseIntegerSwitch(int32 i) cil managed
  {
    // Code size       207 (0xcf)
    .maxstack  2
    .locals init (string V_0,
             int32 V_1)
    IL_0000:  ldstr      "SparseIntegerSwitch: "
    IL_0005:  ldarg.0
    IL_0006:  box        [mscorlib]System.Int32
    IL_000b:  call       string [mscorlib]System.String::Concat(object,
                                                                object)
    IL_0010:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0015:  ldarg.0
    IL_0016:  stloc.1
    IL_0017:  ldloc.1
    IL_0018:  ldc.i4.4
    IL_0019:  bgt.s      IL_004a

    IL_001b:  ldloc.1
    IL_001c:  ldc.i4     0xff676980
    IL_0021:  beq.s      IL_006d

    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.s   -100
    IL_0026:  beq.s      IL_0075

    IL_0028:  ldloc.1
    IL_0029:  ldc.i4.m1
    IL_002a:  sub
    IL_002b:  switch     ( 
                          IL_007d,
                          IL_0085,
                          IL_008d,
                          IL_0095,
                          IL_00c5,
                          IL_009d)
    IL_0048:  br.s       IL_00c5

    IL_004a:  ldloc.1
    IL_004b:  ldc.i4.s   100
    IL_004d:  beq.s      IL_00a5

    IL_004f:  ldloc.1
    IL_0050:  ldc.i4     0x2710
    IL_0055:  sub
    IL_0056:  switch     ( 
                          IL_00ad,
                          IL_00b5)
    IL_0063:  ldloc.1
    IL_0064:  ldc.i4     0x7fffffff
    IL_0069:  beq.s      IL_00bd

    IL_006b:  br.s       IL_00c5

    IL_006d:  ldstr      "-10 mln"
    IL_0072:  stloc.0
    IL_0073:  br.s       IL_00cd

    IL_0075:  ldstr      "-hundred"
    IL_007a:  stloc.0
    IL_007b:  br.s       IL_00cd

    IL_007d:  ldstr      "-1"
    IL_0082:  stloc.0
    IL_0083:  br.s       IL_00cd

    IL_0085:  ldstr      "0"
    IL_008a:  stloc.0
    IL_008b:  br.s       IL_00cd

    IL_008d:  ldstr      "1"
    IL_0092:  stloc.0
    IL_0093:  br.s       IL_00cd

    IL_0095:  ldstr      "2"
    IL_009a:  stloc.0
    IL_009b:  br.s       IL_00cd

    IL_009d:  ldstr      "4"
    IL_00a2:  stloc.0
    IL_00a3:  br.s       IL_00cd

    IL_00a5:  ldstr      "hundred"
    IL_00aa:  stloc.0
    IL_00ab:  br.s       IL_00cd

    IL_00ad:  ldstr      "ten thousand"
    IL_00b2:  stloc.0
    IL_00b3:  br.s       IL_00cd

    IL_00b5:  ldstr      "ten thousand and one"
    IL_00ba:  stloc.0
    IL_00bb:  br.s       IL_00cd

    IL_00bd:  ldstr      "int.MaxValue"
    IL_00c2:  stloc.0
    IL_00c3:  br.s       IL_00cd

    IL_00c5:  ldstr      "something else"
    IL_00ca:  stloc.0
    IL_00cb:  br.s       IL_00cd

    IL_00cd:  ldloc.0
    IL_00ce:  ret
  } // end of method Switch::SparseIntegerSwitch

  .method public hidebysig static void  SwitchOverInt(int32 i) cil managed
  {
    // Code size       136 (0x88)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  ldc.i4.s   10
    IL_0005:  bgt.s      IL_0016

    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.0
    IL_0009:  beq.s      IL_0033

    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.5
    IL_000d:  beq.s      IL_003f

    IL_000f:  ldloc.0
    IL_0010:  ldc.i4.s   10
    IL_0012:  beq.s      IL_004b

    IL_0014:  br.s       IL_0087

    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.s   20
    IL_0019:  bgt.s      IL_0027

    IL_001b:  ldloc.0
    IL_001c:  ldc.i4.s   15
    IL_001e:  beq.s      IL_0057

    IL_0020:  ldloc.0
    IL_0021:  ldc.i4.s   20
    IL_0023:  beq.s      IL_0063

    IL_0025:  br.s       IL_0087

    IL_0027:  ldloc.0
    IL_0028:  ldc.i4.s   25
    IL_002a:  beq.s      IL_006f

    IL_002c:  ldloc.0
    IL_002d:  ldc.i4.s   30
    IL_002f:  beq.s      IL_007b

    IL_0031:  br.s       IL_0087

    IL_0033:  ldstr      "zero"
    IL_0038:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003d:  br.s       IL_0087

    IL_003f:  ldstr      "five"
    IL_0044:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0049:  br.s       IL_0087

    IL_004b:  ldstr      "ten"
    IL_0050:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0055:  br.s       IL_0087

    IL_0057:  ldstr      "fifteen"
    IL_005c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0061:  br.s       IL_0087

    IL_0063:  ldstr      "twenty"
    IL_0068:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006d:  br.s       IL_0087

    IL_006f:  ldstr      "twenty-five"
    IL_0074:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0079:  br.s       IL_0087

    IL_007b:  ldstr      "thirty"
    IL_0080:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0085:  br.s       IL_0087

    IL_0087:  ret
  } // end of method Switch::SwitchOverInt

  .method public hidebysig static string 
          ShortSwitchOverString(string text) cil managed
  {
    // Code size       105 (0x69)
    .maxstack  3
    .locals init (string V_0,
             string V_1)
    IL_0000:  ldstr      "ShortSwitchOverString: "
    IL_0005:  ldarg.0
    IL_0006:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0010:  ldstr      "First case"
    IL_0015:  ldstr      "Second case"
    IL_001a:  ldstr      "Third case"
    IL_001f:  leave.s    IL_0021

    IL_0021:  ldarg.0
    IL_0022:  dup
    IL_0023:  stloc.1
    IL_0024:  brfalse.s  IL_005f

    IL_0026:  ldloc.1
    IL_0027:  call       string [mscorlib]System.String::IsInterned(string)
    IL_002c:  stloc.1
    IL_002d:  ldloc.1
    IL_002e:  ldstr      "First case"
    IL_0033:  beq.s      IL_0047

    IL_0035:  ldloc.1
    IL_0036:  ldstr      "Second case"
    IL_003b:  beq.s      IL_004f

    IL_003d:  ldloc.1
    IL_003e:  ldstr      "Third case"
    IL_0043:  beq.s      IL_0057

    IL_0045:  br.s       IL_005f

    IL_0047:  ldstr      "Text1"
    IL_004c:  stloc.0
    IL_004d:  br.s       IL_0067

    IL_004f:  ldstr      "Text2"
    IL_0054:  stloc.0
    IL_0055:  br.s       IL_0067

    IL_0057:  ldstr      "Text3"
    IL_005c:  stloc.0
    IL_005d:  br.s       IL_0067

    IL_005f:  ldstr      "Default"
    IL_0064:  stloc.0
    IL_0065:  br.s       IL_0067

    IL_0067:  ldloc.0
    IL_0068:  ret
  } // end of method Switch::ShortSwitchOverString

  .method public hidebysig static string 
          ShortSwitchOverStringWithNullCase(string text) cil managed
  {
    // Code size       92 (0x5c)
    .maxstack  2
    .locals init (string V_0,
             string V_1)
    IL_0000:  ldstr      "ShortSwitchOverStringWithNullCase: "
    IL_0005:  ldarg.0
    IL_0006:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0010:  ldstr      "First case"
    IL_0015:  ldstr      "Second case"
    IL_001a:  leave.s    IL_001c

    IL_001c:  ldarg.0
    IL_001d:  dup
    IL_001e:  stloc.1
    IL_001f:  brfalse.s  IL_004a

    IL_0021:  ldloc.1
    IL_0022:  call       string [mscorlib]System.String::IsInterned(string)
    IL_0027:  stloc.1
    IL_0028:  ldloc.1
    IL_0029:  ldstr      "First case"
    IL_002e:  beq.s      IL_003a

    IL_0030:  ldloc.1
    IL_0031:  ldstr      "Second case"
    IL_0036:  beq.s      IL_0042

    IL_0038:  br.s       IL_0052

    IL_003a:  ldstr      "Text1"
    IL_003f:  stloc.0
    IL_0040:  br.s       IL_005a

    IL_0042:  ldstr      "Text2"
    IL_0047:  stloc.0
    IL_0048:  br.s       IL_005a

    IL_004a:  ldstr      "null"
    IL_004f:  stloc.0
    IL_0050:  br.s       IL_005a

    IL_0052:  ldstr      "Default"
    IL_0057:  stloc.0
    IL_0058:  br.s       IL_005a

    IL_005a:  ldloc.0
    IL_005b:  ret
  } // end of method Switch::ShortSwitchOverStringWithNullCase

  .method public hidebysig static string 
          SwitchOverString1(string text) cil managed
  {
    // Code size       185 (0xb9)
    .maxstack  7
    .locals init (string V_0,
             string V_1)
    IL_0000:  ldstr      "SwitchOverString1: "
    IL_0005:  ldarg.0
    IL_0006:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0010:  ldstr      "First case"
    IL_0015:  ldstr      "Second case"
    IL_001a:  ldstr      "2nd case"
    IL_001f:  ldstr      "Third case"
    IL_0024:  ldstr      "Fourth case"
    IL_0029:  ldstr      "Fifth case"
    IL_002e:  ldstr      "Sixth case"
    IL_0033:  leave.s    IL_0035

    IL_0035:  ldarg.0
    IL_0036:  dup
    IL_0037:  stloc.1
    IL_0038:  brfalse.s  IL_00ab

    IL_003a:  ldloc.1
    IL_003b:  call       string [mscorlib]System.String::IsInterned(string)
    IL_0040:  stloc.1
    IL_0041:  ldloc.1
    IL_0042:  ldstr      "First case"
    IL_0047:  beq.s      IL_007b

    IL_0049:  ldloc.1
    IL_004a:  ldstr      "Second case"
    IL_004f:  beq.s      IL_0083

    IL_0051:  ldloc.1
    IL_0052:  ldstr      "2nd case"
    IL_0057:  beq.s      IL_0083

    IL_0059:  ldloc.1
    IL_005a:  ldstr      "Third case"
    IL_005f:  beq.s      IL_008b

    IL_0061:  ldloc.1
    IL_0062:  ldstr      "Fourth case"
    IL_0067:  beq.s      IL_0093

    IL_0069:  ldloc.1
    IL_006a:  ldstr      "Fifth case"
    IL_006f:  beq.s      IL_009b

    IL_0071:  ldloc.1
    IL_0072:  ldstr      "Sixth case"
    IL_0077:  beq.s      IL_00a3

    IL_0079:  br.s       IL_00af

    IL_007b:  ldstr      "Text1"
    IL_0080:  stloc.0
    IL_0081:  br.s       IL_00b7

    IL_0083:  ldstr      "Text2"
    IL_0088:  stloc.0
    IL_0089:  br.s       IL_00b7

    IL_008b:  ldstr      "Text3"
    IL_0090:  stloc.0
    IL_0091:  br.s       IL_00b7

    IL_0093:  ldstr      "Text4"
    IL_0098:  stloc.0
    IL_0099:  br.s       IL_00b7

    IL_009b:  ldstr      "Text5"
    IL_00a0:  stloc.0
    IL_00a1:  br.s       IL_00b7

    IL_00a3:  ldstr      "Text6"
    IL_00a8:  stloc.0
    IL_00a9:  br.s       IL_00b7

    IL_00ab:  ldnull
    IL_00ac:  stloc.0
    IL_00ad:  br.s       IL_00b7

    IL_00af:  ldstr      "Default"
    IL_00b4:  stloc.0
    IL_00b5:  br.s       IL_00b7

    IL_00b7:  ldloc.0
    IL_00b8:  ret
  } // end of method Switch::SwitchOverString1

  .method public hidebysig static string 
          SwitchOverString2() cil managed
  {
    // Code size       418 (0x1a2)
    .maxstack  4
    .locals init (string V_0,
             object V_1)
    IL_0000:  volatile.
    IL_0002:  ldsfld     class [mscorlib]System.Collections.Hashtable '<PrivateImplementationDetails>'::'$$method0x6000006-1'
    IL_0007:  brtrue     IL_00dc

    IL_000c:  ldc.i4.s   24
    IL_000e:  ldc.r4     0.5
    IL_0013:  newobj     instance void [mscorlib]System.Collections.Hashtable::.ctor(int32,
                                                                                     float32)
    IL_0018:  dup
    IL_0019:  ldstr      "First case"
    IL_001e:  ldc.i4.0
    IL_001f:  box        [mscorlib]System.Int32
    IL_0024:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_0029:  dup
    IL_002a:  ldstr      "Second case"
    IL_002f:  ldc.i4.1
    IL_0030:  box        [mscorlib]System.Int32
    IL_0035:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_003a:  dup
    IL_003b:  ldstr      "Third case"
    IL_0040:  ldc.i4.2
    IL_0041:  box        [mscorlib]System.Int32
    IL_0046:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_004b:  dup
    IL_004c:  ldstr      "Fourth case"
    IL_0051:  ldc.i4.3
    IL_0052:  box        [mscorlib]System.Int32
    IL_0057:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_005c:  dup
    IL_005d:  ldstr      "Fifth case"
    IL_0062:  ldc.i4.4
    IL_0063:  box        [mscorlib]System.Int32
    IL_0068:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_006d:  dup
    IL_006e:  ldstr      "Sixth case"
    IL_0073:  ldc.i4.5
    IL_0074:  box        [mscorlib]System.Int32
    IL_0079:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_007e:  dup
    IL_007f:  ldstr      "Seventh case"
    IL_0084:  ldc.i4.6
    IL_0085:  box        [mscorlib]System.Int32
    IL_008a:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_008f:  dup
    IL_0090:  ldstr      "Eighth case"
    IL_0095:  ldc.i4.7
    IL_0096:  box        [mscorlib]System.Int32
    IL_009b:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_00a0:  dup
    IL_00a1:  ldstr      "Ninth case"
    IL_00a6:  ldc.i4.8
    IL_00a7:  box        [mscorlib]System.Int32
    IL_00ac:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_00b1:  dup
    IL_00b2:  ldstr      "Tenth case"
    IL_00b7:  ldc.i4.s   9
    IL_00b9:  box        [mscorlib]System.Int32
    IL_00be:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_00c3:  dup
    IL_00c4:  ldstr      "Eleventh case"
    IL_00c9:  ldc.i4.s   10
    IL_00cb:  box        [mscorlib]System.Int32
    IL_00d0:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_00d5:  volatile.
    IL_00d7:  stsfld     class [mscorlib]System.Collections.Hashtable '<PrivateImplementationDetails>'::'$$method0x6000006-1'
    IL_00dc:  ldstr      "SwitchOverString2:"
    IL_00e1:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00e6:  call       string [mscorlib]System.Environment::get_UserName()
    IL_00eb:  dup
    IL_00ec:  stloc.1
    IL_00ed:  brfalse    IL_0198

    IL_00f2:  volatile.
    IL_00f4:  ldsfld     class [mscorlib]System.Collections.Hashtable '<PrivateImplementationDetails>'::'$$method0x6000006-1'
    IL_00f9:  ldloc.1
    IL_00fa:  call       instance object [mscorlib]System.Collections.Hashtable::get_Item(object)
    IL_00ff:  dup
    IL_0100:  stloc.1
    IL_0101:  brfalse    IL_0198

    IL_0106:  ldloc.1
    IL_0107:  unbox      [mscorlib]System.Int32
    IL_010c:  ldind.i4
    IL_010d:  switch     ( 
                          IL_0140,
                          IL_0148,
                          IL_0150,
                          IL_0158,
                          IL_0160,
                          IL_0168,
                          IL_0170,
                          IL_0178,
                          IL_0180,
                          IL_0188,
                          IL_0190)
    IL_013e:  br.s       IL_0198

    IL_0140:  ldstr      "Text1"
    IL_0145:  stloc.0
    IL_0146:  br.s       IL_01a0

    IL_0148:  ldstr      "Text2"
    IL_014d:  stloc.0
    IL_014e:  br.s       IL_01a0

    IL_0150:  ldstr      "Text3"
    IL_0155:  stloc.0
    IL_0156:  br.s       IL_01a0

    IL_0158:  ldstr      "Text4"
    IL_015d:  stloc.0
    IL_015e:  br.s       IL_01a0

    IL_0160:  ldstr      "Text5"
    IL_0165:  stloc.0
    IL_0166:  br.s       IL_01a0

    IL_0168:  ldstr      "Text6"
    IL_016d:  stloc.0
    IL_016e:  br.s       IL_01a0

    IL_0170:  ldstr      "Text7"
    IL_0175:  stloc.0
    IL_0176:  br.s       IL_01a0

    IL_0178:  ldstr      "Text8"
    IL_017d:  stloc.0
    IL_017e:  br.s       IL_01a0

    IL_0180:  ldstr      "Text9"
    IL_0185:  stloc.0
    IL_0186:  br.s       IL_01a0

    IL_0188:  ldstr      "Text10"
    IL_018d:  stloc.0
    IL_018e:  br.s       IL_01a0

    IL_0190:  ldstr      "Text11"
    IL_0195:  stloc.0
    IL_0196:  br.s       IL_01a0

    IL_0198:  ldstr      "Default"
    IL_019d:  stloc.0
    IL_019e:  br.s       IL_01a0

    IL_01a0:  ldloc.0
    IL_01a1:  ret
  } // end of method Switch::SwitchOverString2

  .method public hidebysig static string 
          TwoDifferentSwitchBlocksInTryFinally() cil managed
  {
    // Code size       925 (0x39d)
    .maxstack  4
    .locals init (string V_0,
             object V_1)
    IL_0000:  volatile.
    IL_0002:  ldsfld     class [mscorlib]System.Collections.Hashtable '<PrivateImplementationDetails>'::'$$method0x6000007-1'
    IL_0007:  brtrue     IL_01b8

    IL_000c:  ldc.i4.s   24
    IL_000e:  ldc.r4     0.5
    IL_0013:  newobj     instance void [mscorlib]System.Collections.Hashtable::.ctor(int32,
                                                                                     float32)
    IL_0018:  dup
    IL_0019:  ldstr      "12"
    IL_001e:  ldc.i4.0
    IL_001f:  box        [mscorlib]System.Int32
    IL_0024:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_0029:  dup
    IL_002a:  ldstr      "13"
    IL_002f:  ldc.i4.1
    IL_0030:  box        [mscorlib]System.Int32
    IL_0035:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_003a:  dup
    IL_003b:  ldstr      "14"
    IL_0040:  ldc.i4.2
    IL_0041:  box        [mscorlib]System.Int32
    IL_0046:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_004b:  dup
    IL_004c:  ldstr      "15"
    IL_0051:  ldc.i4.3
    IL_0052:  box        [mscorlib]System.Int32
    IL_0057:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_005c:  dup
    IL_005d:  ldstr      "16"
    IL_0062:  ldc.i4.4
    IL_0063:  box        [mscorlib]System.Int32
    IL_0068:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_006d:  dup
    IL_006e:  ldstr      "17"
    IL_0073:  ldc.i4.5
    IL_0074:  box        [mscorlib]System.Int32
    IL_0079:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_007e:  dup
    IL_007f:  ldstr      "18"
    IL_0084:  ldc.i4.6
    IL_0085:  box        [mscorlib]System.Int32
    IL_008a:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_008f:  dup
    IL_0090:  ldstr      "19 case"
    IL_0095:  ldc.i4.7
    IL_0096:  box        [mscorlib]System.Int32
    IL_009b:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_00a0:  dup
    IL_00a1:  ldstr      "20 case"
    IL_00a6:  ldc.i4.8
    IL_00a7:  box        [mscorlib]System.Int32
    IL_00ac:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_00b1:  dup
    IL_00b2:  ldstr      "21 case"
    IL_00b7:  ldc.i4.s   9
    IL_00b9:  box        [mscorlib]System.Int32
    IL_00be:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_00c3:  dup
    IL_00c4:  ldstr      "22 case"
    IL_00c9:  ldc.i4.s   10
    IL_00cb:  box        [mscorlib]System.Int32
    IL_00d0:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_00d5:  volatile.
    IL_00d7:  stsfld     class [mscorlib]System.Collections.Hashtable '<PrivateImplementationDetails>'::'$$method0x6000007-1'
    IL_00dc:  volatile.
    IL_00de:  ldsfld     class [mscorlib]System.Collections.Hashtable '<PrivateImplementationDetails>'::'$$method0x6000007-2'
    IL_00e3:  brtrue     IL_01b8

    IL_00e8:  ldc.i4.s   24
    IL_00ea:  ldc.r4     0.5
    IL_00ef:  newobj     instance void [mscorlib]System.Collections.Hashtable::.ctor(int32,
                                                                                     float32)
    IL_00f4:  dup
    IL_00f5:  ldstr      "First case"
    IL_00fa:  ldc.i4.0
    IL_00fb:  box        [mscorlib]System.Int32
    IL_0100:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_0105:  dup
    IL_0106:  ldstr      "Second case"
    IL_010b:  ldc.i4.1
    IL_010c:  box        [mscorlib]System.Int32
    IL_0111:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_0116:  dup
    IL_0117:  ldstr      "Third case"
    IL_011c:  ldc.i4.2
    IL_011d:  box        [mscorlib]System.Int32
    IL_0122:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_0127:  dup
    IL_0128:  ldstr      "Fourth case"
    IL_012d:  ldc.i4.3
    IL_012e:  box        [mscorlib]System.Int32
    IL_0133:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_0138:  dup
    IL_0139:  ldstr      "Fifth case"
    IL_013e:  ldc.i4.4
    IL_013f:  box        [mscorlib]System.Int32
    IL_0144:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_0149:  dup
    IL_014a:  ldstr      "Sixth case"
    IL_014f:  ldc.i4.5
    IL_0150:  box        [mscorlib]System.Int32
    IL_0155:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_015a:  dup
    IL_015b:  ldstr      "Seventh case"
    IL_0160:  ldc.i4.6
    IL_0161:  box        [mscorlib]System.Int32
    IL_0166:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_016b:  dup
    IL_016c:  ldstr      "Eighth case"
    IL_0171:  ldc.i4.7
    IL_0172:  box        [mscorlib]System.Int32
    IL_0177:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_017c:  dup
    IL_017d:  ldstr      "Ninth case"
    IL_0182:  ldc.i4.8
    IL_0183:  box        [mscorlib]System.Int32
    IL_0188:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_018d:  dup
    IL_018e:  ldstr      "Tenth case"
    IL_0193:  ldc.i4.s   9
    IL_0195:  box        [mscorlib]System.Int32
    IL_019a:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_019f:  dup
    IL_01a0:  ldstr      "Eleventh case"
    IL_01a5:  ldc.i4.s   10
    IL_01a7:  box        [mscorlib]System.Int32
    IL_01ac:  call       instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_01b1:  volatile.
    IL_01b3:  stsfld     class [mscorlib]System.Collections.Hashtable '<PrivateImplementationDetails>'::'$$method0x6000007-2'
    .try
    {
      IL_01b8:  ldstr      "TwoDifferentSwitchBlocks:"
      IL_01bd:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_01c2:  call       string [mscorlib]System.Environment::get_UserName()
      IL_01c7:  dup
      IL_01c8:  stloc.1
      IL_01c9:  brfalse    IL_0295

      IL_01ce:  volatile.
      IL_01d0:  ldsfld     class [mscorlib]System.Collections.Hashtable '<PrivateImplementationDetails>'::'$$method0x6000007-2'
      IL_01d5:  ldloc.1
      IL_01d6:  call       instance object [mscorlib]System.Collections.Hashtable::get_Item(object)
      IL_01db:  dup
      IL_01dc:  stloc.1
      IL_01dd:  brfalse    IL_0295

      IL_01e2:  ldloc.1
      IL_01e3:  unbox      [mscorlib]System.Int32
      IL_01e8:  ldind.i4
      IL_01e9:  switch     ( 
                            IL_021c,
                            IL_0227,
                            IL_0232,
                            IL_023d,
                            IL_0248,
                            IL_0253,
                            IL_025e,
                            IL_0269,
                            IL_0274,
                            IL_027f,
                            IL_028a)
      IL_021a:  br.s       IL_0295

      IL_021c:  ldstr      "Text1"
      IL_0221:  stloc.0
      IL_0222:  leave      IL_039b

      IL_0227:  ldstr      "Text2"
      IL_022c:  stloc.0
      IL_022d:  leave      IL_039b

      IL_0232:  ldstr      "Text3"
      IL_0237:  stloc.0
      IL_0238:  leave      IL_039b

      IL_023d:  ldstr      "Text4"
      IL_0242:  stloc.0
      IL_0243:  leave      IL_039b

      IL_0248:  ldstr      "Text5"
      IL_024d:  stloc.0
      IL_024e:  leave      IL_039b

      IL_0253:  ldstr      "Text6"
      IL_0258:  stloc.0
      IL_0259:  leave      IL_039b

      IL_025e:  ldstr      "Text7"
      IL_0263:  stloc.0
      IL_0264:  leave      IL_039b

      IL_0269:  ldstr      "Text8"
      IL_026e:  stloc.0
      IL_026f:  leave      IL_039b

      IL_0274:  ldstr      "Text9"
      IL_0279:  stloc.0
      IL_027a:  leave      IL_039b

      IL_027f:  ldstr      "Text10"
      IL_0284:  stloc.0
      IL_0285:  leave      IL_039b

      IL_028a:  ldstr      "Text11"
      IL_028f:  stloc.0
      IL_0290:  leave      IL_039b

      IL_0295:  ldstr      "Default"
      IL_029a:  stloc.0
      IL_029b:  leave      IL_039b

    }  // end .try
    finally
    {
      IL_02a0:  ldstr      "Second switch:"
      IL_02a5:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_02aa:  call       string [mscorlib]System.Console::ReadLine()
      IL_02af:  dup
      IL_02b0:  stloc.1
      IL_02b1:  brfalse    IL_038e

      IL_02b6:  volatile.
      IL_02b8:  ldsfld     class [mscorlib]System.Collections.Hashtable '<PrivateImplementationDetails>'::'$$method0x6000007-1'
      IL_02bd:  ldloc.1
      IL_02be:  call       instance object [mscorlib]System.Collections.Hashtable::get_Item(object)
      IL_02c3:  dup
      IL_02c4:  stloc.1
      IL_02c5:  brfalse    IL_038e

      IL_02ca:  ldloc.1
      IL_02cb:  unbox      [mscorlib]System.Int32
      IL_02d0:  ldind.i4
      IL_02d1:  switch     ( 
                            IL_0307,
                            IL_0316,
                            IL_0322,
                            IL_032e,
                            IL_033a,
                            IL_0346,
                            IL_0352,
                            IL_035e,
                            IL_036a,
                            IL_0376,
                            IL_0382)
      IL_0302:  br         IL_038e

      IL_0307:  ldstr      "Te43234xt1"
      IL_030c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0311:  br         IL_039a

      IL_0316:  ldstr      "Te223443xt2"
      IL_031b:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0320:  br.s       IL_039a

      IL_0322:  ldstr      "Te234xt3"
      IL_0327:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_032c:  br.s       IL_039a

      IL_032e:  ldstr      "Tex243t4"
      IL_0333:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0338:  br.s       IL_039a

      IL_033a:  ldstr      "Tex243t5"
      IL_033f:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0344:  br.s       IL_039a

      IL_0346:  ldstr      "Text2346"
      IL_034b:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0350:  br.s       IL_039a

      IL_0352:  ldstr      "Text234234"
      IL_0357:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_035c:  br.s       IL_039a

      IL_035e:  ldstr      "Text8234"
      IL_0363:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0368:  br.s       IL_039a

      IL_036a:  ldstr      "Text923423"
      IL_036f:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0374:  br.s       IL_039a

      IL_0376:  ldstr      "Text10"
      IL_037b:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0380:  br.s       IL_039a

      IL_0382:  ldstr      "Text1134123"
      IL_0387:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_038c:  br.s       IL_039a

      IL_038e:  ldstr      "Defa234234ult"
      IL_0393:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0398:  br.s       IL_039a

      IL_039a:  endfinally
    }  // end handler
    IL_039b:  ldloc.0
    IL_039c:  ret
  } // end of method Switch::TwoDifferentSwitchBlocksInTryFinally

  .method public hidebysig static string 
          SwitchOverBool(bool b) cil managed
  {
    // Code size       62 (0x3e)
    .maxstack  2
    .locals init (string V_0,
             bool V_1)
    IL_0000:  ldstr      "SwitchOverBool: "
    IL_0005:  ldarga.s   b
    IL_0007:  call       instance string [mscorlib]System.Boolean::ToString()
    IL_000c:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0011:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0016:  ldarg.0
    IL_0017:  stloc.1
    IL_0018:  ldloc.1
    IL_0019:  switch     ( 
                          IL_0030,
                          IL_0028)
    IL_0026:  br.s       IL_0038

    IL_0028:  ldsfld     string [mscorlib]System.Boolean::TrueString
    IL_002d:  stloc.0
    IL_002e:  br.s       IL_003c

    IL_0030:  ldsfld     string [mscorlib]System.Boolean::FalseString
    IL_0035:  stloc.0
    IL_0036:  br.s       IL_003c

    IL_0038:  ldnull
    IL_0039:  stloc.0
    IL_003a:  br.s       IL_003c

    IL_003c:  ldloc.0
    IL_003d:  ret
  } // end of method Switch::SwitchOverBool

  .method public hidebysig static void  SwitchInLoop(int32 i) cil managed
  {
    // Code size       117 (0x75)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldstr      "SwitchInLoop: "
    IL_0005:  ldarg.0
    IL_0006:  box        [mscorlib]System.Int32
    IL_000b:  call       string [mscorlib]System.String::Concat(object,
                                                                object)
    IL_0010:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0015:  br.s       IL_0072

    IL_0017:  ldarg.0
    IL_0018:  stloc.0
    IL_0019:  ldloc.0
    IL_001a:  ldc.i4.1
    IL_001b:  sub
    IL_001c:  switch     ( 
                          IL_0033,
                          IL_003f,
                          IL_0057,
                          IL_004b)
    IL_0031:  br.s       IL_0057

    IL_0033:  ldstr      "one"
    IL_0038:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003d:  br.s       IL_006d

    IL_003f:  ldstr      "two"
    IL_0044:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0049:  br.s       IL_006d

    IL_004b:  ldstr      "four"
    IL_0050:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0055:  br.s       IL_0074

    IL_0057:  ldstr      "default"
    IL_005c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0061:  ldstr      "more code"
    IL_0066:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006b:  br.s       IL_0074

    IL_006d:  ldarg.0
    IL_006e:  ldc.i4.1
    IL_006f:  add
    IL_0070:  starg.s    i
    IL_0072:  br.s       IL_0017

    IL_0074:  ret
  } // end of method Switch::SwitchInLoop

  .method public hidebysig static void  SwitchWithGoto(int32 i) cil managed
  {
    // Code size       120 (0x78)
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
                          IL_0049,
                          IL_0055)
    IL_002f:  br.s       IL_0061

    IL_0031:  ldstr      "one"
    IL_0036:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003b:  br.s       IL_0061

    IL_003d:  ldstr      "two"
    IL_0042:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0047:  br.s       IL_0049

    IL_0049:  ldstr      "three"
    IL_004e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0053:  br.s       IL_006d

    IL_0055:  ldstr      "four"
    IL_005a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005f:  br.s       IL_0077

    IL_0061:  ldstr      "default"
    IL_0066:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006b:  br.s       IL_006d

    IL_006d:  ldstr      "End of method"
    IL_0072:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0077:  ret
  } // end of method Switch::SwitchWithGoto

  .method private hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty[] 
          GetProperties() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty[] V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method Switch::GetProperties

  .method public hidebysig static void  SwitchOnStringInForLoop() cil managed
  {
    // Code size       269 (0x10d)
    .maxstack  6
    .locals init (class [mscorlib]System.Collections.ArrayList V_0,
             class [mscorlib]System.Collections.ArrayList V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty[] V_2,
             int32 V_3,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty V_4,
             string V_5)
    IL_0000:  newobj     instance void [mscorlib]System.Collections.ArrayList::.ctor()
    IL_0005:  stloc.0
    IL_0006:  newobj     instance void [mscorlib]System.Collections.ArrayList::.ctor()
    IL_000b:  stloc.1
    IL_000c:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch::GetProperties()
    IL_0011:  stloc.2
    IL_0012:  ldc.i4.0
    IL_0013:  stloc.3
    IL_0014:  br         IL_0103

    IL_0019:  ldstr      "In for-loop"
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0023:  ldloc.2
    IL_0024:  ldloc.3
    IL_0025:  ldelem.ref
    IL_0026:  stloc.s    V_4
    IL_0028:  ldstr      "Name1"
    IL_002d:  ldstr      "Name2"
    IL_0032:  ldstr      "Name3"
    IL_0037:  ldstr      "Name4"
    IL_003c:  ldstr      "Name5"
    IL_0041:  ldstr      "Name6"
    IL_0046:  leave.s    IL_0048

    IL_0048:  ldloc.s    V_4
    IL_004a:  ldfld      class [mscorlib]System.Reflection.PropertyInfo ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::Property
    IL_004f:  callvirt   instance string [mscorlib]System.Reflection.MemberInfo::get_Name()
    IL_0054:  dup
    IL_0055:  stloc.s    V_5
    IL_0057:  brfalse    IL_00f4

    IL_005c:  ldloc.s    V_5
    IL_005e:  call       string [mscorlib]System.String::IsInterned(string)
    IL_0063:  stloc.s    V_5
    IL_0065:  ldloc.s    V_5
    IL_0067:  ldstr      "Name1"
    IL_006c:  beq.s      IL_009d

    IL_006e:  ldloc.s    V_5
    IL_0070:  ldstr      "Name2"
    IL_0075:  beq.s      IL_00b0

    IL_0077:  ldloc.s    V_5
    IL_0079:  ldstr      "Name3"
    IL_007e:  beq.s      IL_00c3

    IL_0080:  ldloc.s    V_5
    IL_0082:  ldstr      "Name4"
    IL_0087:  beq.s      IL_00d6

    IL_0089:  ldloc.s    V_5
    IL_008b:  ldstr      "Name5"
    IL_0090:  beq.s      IL_00e9

    IL_0092:  ldloc.s    V_5
    IL_0094:  ldstr      "Name6"
    IL_0099:  beq.s      IL_00e9

    IL_009b:  br.s       IL_00f4

    IL_009d:  ldloc.s    V_4
    IL_009f:  ldc.i4.1
    IL_00a0:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00a5:  ldloc.0
    IL_00a6:  ldloc.s    V_4
    IL_00a8:  callvirt   instance int32 [mscorlib]System.Collections.ArrayList::Add(object)
    IL_00ad:  pop
    IL_00ae:  br.s       IL_00ff

    IL_00b0:  ldloc.s    V_4
    IL_00b2:  ldc.i4.2
    IL_00b3:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00b8:  ldloc.0
    IL_00b9:  ldloc.s    V_4
    IL_00bb:  callvirt   instance int32 [mscorlib]System.Collections.ArrayList::Add(object)
    IL_00c0:  pop
    IL_00c1:  br.s       IL_00ff

    IL_00c3:  ldloc.s    V_4
    IL_00c5:  ldc.i4.3
    IL_00c6:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00cb:  ldloc.0
    IL_00cc:  ldloc.s    V_4
    IL_00ce:  callvirt   instance int32 [mscorlib]System.Collections.ArrayList::Add(object)
    IL_00d3:  pop
    IL_00d4:  br.s       IL_00ff

    IL_00d6:  ldloc.s    V_4
    IL_00d8:  ldc.i4.4
    IL_00d9:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00de:  ldloc.0
    IL_00df:  ldloc.s    V_4
    IL_00e1:  callvirt   instance int32 [mscorlib]System.Collections.ArrayList::Add(object)
    IL_00e6:  pop
    IL_00e7:  br.s       IL_00ff

    IL_00e9:  ldloc.0
    IL_00ea:  ldloc.s    V_4
    IL_00ec:  callvirt   instance int32 [mscorlib]System.Collections.ArrayList::Add(object)
    IL_00f1:  pop
    IL_00f2:  br.s       IL_00ff

    IL_00f4:  ldloc.1
    IL_00f5:  ldloc.s    V_4
    IL_00f7:  callvirt   instance int32 [mscorlib]System.Collections.ArrayList::Add(object)
    IL_00fc:  pop
    IL_00fd:  br.s       IL_00ff

    IL_00ff:  ldloc.3
    IL_0100:  ldc.i4.1
    IL_0101:  add
    IL_0102:  stloc.3
    IL_0103:  ldloc.3
    IL_0104:  ldloc.2
    IL_0105:  ldlen
    IL_0106:  conv.i4
    IL_0107:  blt        IL_0019

    IL_010c:  ret
  } // end of method Switch::SwitchOnStringInForLoop

  .method public hidebysig static void  SwitchWithComplexCondition(string[] args) cil managed
  {
    // Code size       141 (0x8d)
    .maxstack  4
    .locals init (string V_0)
    IL_0000:  ldstr      "a"
    IL_0005:  ldstr      "b"
    IL_000a:  ldstr      "c"
    IL_000f:  ldstr      "d"
    IL_0014:  leave.s    IL_0016

    IL_0016:  ldarg.0
    IL_0017:  ldlen
    IL_0018:  conv.i4
    IL_0019:  brfalse.s  IL_0020

    IL_001b:  ldarg.0
    IL_001c:  ldc.i4.0
    IL_001d:  ldelem.ref
    IL_001e:  br.s       IL_0025

    IL_0020:  ldstr      "dummy"
    IL_0025:  dup
    IL_0026:  stloc.0
    IL_0027:  brfalse.s  IL_0082

    IL_0029:  ldloc.0
    IL_002a:  call       string [mscorlib]System.String::IsInterned(string)
    IL_002f:  stloc.0
    IL_0030:  ldloc.0
    IL_0031:  ldstr      "a"
    IL_0036:  beq.s      IL_0052

    IL_0038:  ldloc.0
    IL_0039:  ldstr      "b"
    IL_003e:  beq.s      IL_005e

    IL_0040:  ldloc.0
    IL_0041:  ldstr      "c"
    IL_0046:  beq.s      IL_006a

    IL_0048:  ldloc.0
    IL_0049:  ldstr      "d"
    IL_004e:  beq.s      IL_0076

    IL_0050:  br.s       IL_0082

    IL_0052:  ldstr      "a"
    IL_0057:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005c:  br.s       IL_0082

    IL_005e:  ldstr      "b"
    IL_0063:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0068:  br.s       IL_0082

    IL_006a:  ldstr      "c"
    IL_006f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0074:  br.s       IL_0082

    IL_0076:  ldstr      "d"
    IL_007b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0080:  br.s       IL_0082

    IL_0082:  ldstr      "end"
    IL_0087:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_008c:  ret
  } // end of method Switch::SwitchWithComplexCondition

  .method public hidebysig static void  SwitchWithArray(string[] args) cil managed
  {
    // Code size       129 (0x81)
    .maxstack  4
    .locals init (string V_0)
    IL_0000:  ldstr      "a"
    IL_0005:  ldstr      "b"
    IL_000a:  ldstr      "c"
    IL_000f:  ldstr      "d"
    IL_0014:  leave.s    IL_0016

    IL_0016:  ldarg.0
    IL_0017:  ldc.i4.0
    IL_0018:  ldelem.ref
    IL_0019:  dup
    IL_001a:  stloc.0
    IL_001b:  brfalse.s  IL_0076

    IL_001d:  ldloc.0
    IL_001e:  call       string [mscorlib]System.String::IsInterned(string)
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldstr      "a"
    IL_002a:  beq.s      IL_0046

    IL_002c:  ldloc.0
    IL_002d:  ldstr      "b"
    IL_0032:  beq.s      IL_0052

    IL_0034:  ldloc.0
    IL_0035:  ldstr      "c"
    IL_003a:  beq.s      IL_005e

    IL_003c:  ldloc.0
    IL_003d:  ldstr      "d"
    IL_0042:  beq.s      IL_006a

    IL_0044:  br.s       IL_0076

    IL_0046:  ldstr      "a"
    IL_004b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0050:  br.s       IL_0076

    IL_0052:  ldstr      "b"
    IL_0057:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005c:  br.s       IL_0076

    IL_005e:  ldstr      "c"
    IL_0063:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0068:  br.s       IL_0076

    IL_006a:  ldstr      "d"
    IL_006f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0074:  br.s       IL_0076

    IL_0076:  ldstr      "end"
    IL_007b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0080:  ret
  } // end of method Switch::SwitchWithArray

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Switch::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch

.class private auto ansi '<PrivateImplementationDetails>'
       extends [mscorlib]System.Object
{
  .field static assembly class [mscorlib]System.Collections.Hashtable '$$method0x6000006-1'
  .field static assembly class [mscorlib]System.Collections.Hashtable '$$method0x6000007-1'
  .field static assembly class [mscorlib]System.Collections.Hashtable '$$method0x6000007-2'
} // end of class '<PrivateImplementationDetails>'


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file C:\Users\Siegfried\Projects\ILSpy master\ICSharpCode.Decompiler.Tests\TestCases\ILPretty\CS1xSwitch_Debug.res
