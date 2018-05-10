
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v1.1.4322
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 1:0:5000:0
}
.assembly CS1xSwitch_Release
{
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module CS1xSwitch_Release.dll
// MVID: {E26201E5-5EC1-412E-BE6D-AE48AFAE6535}
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
      // Code size       7 (0x7)
      .maxstack  1
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::_set
      IL_0006:  ret
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

  .method public hidebysig static void  SwitchOverInt(int32 i) cil managed
  {
    // Code size       125 (0x7d)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  ldc.i4.s   10
    IL_0005:  bgt.s      IL_0015

    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.0
    IL_0009:  beq.s      IL_0030

    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.5
    IL_000d:  beq.s      IL_003b

    IL_000f:  ldloc.0
    IL_0010:  ldc.i4.s   10
    IL_0012:  beq.s      IL_0046

    IL_0014:  ret

    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.s   20
    IL_0018:  bgt.s      IL_0025

    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.s   15
    IL_001d:  beq.s      IL_0051

    IL_001f:  ldloc.0
    IL_0020:  ldc.i4.s   20
    IL_0022:  beq.s      IL_005c

    IL_0024:  ret

    IL_0025:  ldloc.0
    IL_0026:  ldc.i4.s   25
    IL_0028:  beq.s      IL_0067

    IL_002a:  ldloc.0
    IL_002b:  ldc.i4.s   30
    IL_002d:  beq.s      IL_0072

    IL_002f:  ret

    IL_0030:  ldstr      "zero"
    IL_0035:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003a:  ret

    IL_003b:  ldstr      "five"
    IL_0040:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0045:  ret

    IL_0046:  ldstr      "ten"
    IL_004b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0050:  ret

    IL_0051:  ldstr      "fifteen"
    IL_0056:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005b:  ret

    IL_005c:  ldstr      "twenty"
    IL_0061:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0066:  ret

    IL_0067:  ldstr      "twenty-five"
    IL_006c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0071:  ret

    IL_0072:  ldstr      "thirty"
    IL_0077:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007c:  ret
  } // end of method Switch::SwitchOverInt

  .method public hidebysig static string 
          ShortSwitchOverString(string text) cil managed
  {
    // Code size       95 (0x5f)
    .maxstack  3
    .locals init (string V_0)
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
    IL_0023:  stloc.0
    IL_0024:  brfalse.s  IL_0059

    IL_0026:  ldloc.0
    IL_0027:  call       string [mscorlib]System.String::IsInterned(string)
    IL_002c:  stloc.0
    IL_002d:  ldloc.0
    IL_002e:  ldstr      "First case"
    IL_0033:  beq.s      IL_0047

    IL_0035:  ldloc.0
    IL_0036:  ldstr      "Second case"
    IL_003b:  beq.s      IL_004d

    IL_003d:  ldloc.0
    IL_003e:  ldstr      "Third case"
    IL_0043:  beq.s      IL_0053

    IL_0045:  br.s       IL_0059

    IL_0047:  ldstr      "Text1"
    IL_004c:  ret

    IL_004d:  ldstr      "Text2"
    IL_0052:  ret

    IL_0053:  ldstr      "Text3"
    IL_0058:  ret

    IL_0059:  ldstr      "Default"
    IL_005e:  ret
  } // end of method Switch::ShortSwitchOverString

  .method public hidebysig static string 
          ShortSwitchOverStringWithNullCase(string text) cil managed
  {
    // Code size       82 (0x52)
    .maxstack  2
    .locals init (string V_0)
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
    IL_001e:  stloc.0
    IL_001f:  brfalse.s  IL_0046

    IL_0021:  ldloc.0
    IL_0022:  call       string [mscorlib]System.String::IsInterned(string)
    IL_0027:  stloc.0
    IL_0028:  ldloc.0
    IL_0029:  ldstr      "First case"
    IL_002e:  beq.s      IL_003a

    IL_0030:  ldloc.0
    IL_0031:  ldstr      "Second case"
    IL_0036:  beq.s      IL_0040

    IL_0038:  br.s       IL_004c

    IL_003a:  ldstr      "Text1"
    IL_003f:  ret

    IL_0040:  ldstr      "Text2"
    IL_0045:  ret

    IL_0046:  ldstr      "null"
    IL_004b:  ret

    IL_004c:  ldstr      "Default"
    IL_0051:  ret
  } // end of method Switch::ShortSwitchOverStringWithNullCase

  .method public hidebysig static string 
          SwitchOverString1(string text) cil managed
  {
    // Code size       167 (0xa7)
    .maxstack  7
    .locals init (string V_0)
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
    IL_0037:  stloc.0
    IL_0038:  brfalse.s  IL_009f

    IL_003a:  ldloc.0
    IL_003b:  call       string [mscorlib]System.String::IsInterned(string)
    IL_0040:  stloc.0
    IL_0041:  ldloc.0
    IL_0042:  ldstr      "First case"
    IL_0047:  beq.s      IL_007b

    IL_0049:  ldloc.0
    IL_004a:  ldstr      "Second case"
    IL_004f:  beq.s      IL_0081

    IL_0051:  ldloc.0
    IL_0052:  ldstr      "2nd case"
    IL_0057:  beq.s      IL_0081

    IL_0059:  ldloc.0
    IL_005a:  ldstr      "Third case"
    IL_005f:  beq.s      IL_0087

    IL_0061:  ldloc.0
    IL_0062:  ldstr      "Fourth case"
    IL_0067:  beq.s      IL_008d

    IL_0069:  ldloc.0
    IL_006a:  ldstr      "Fifth case"
    IL_006f:  beq.s      IL_0093

    IL_0071:  ldloc.0
    IL_0072:  ldstr      "Sixth case"
    IL_0077:  beq.s      IL_0099

    IL_0079:  br.s       IL_00a1

    IL_007b:  ldstr      "Text1"
    IL_0080:  ret

    IL_0081:  ldstr      "Text2"
    IL_0086:  ret

    IL_0087:  ldstr      "Text3"
    IL_008c:  ret

    IL_008d:  ldstr      "Text4"
    IL_0092:  ret

    IL_0093:  ldstr      "Text5"
    IL_0098:  ret

    IL_0099:  ldstr      "Text6"
    IL_009e:  ret

    IL_009f:  ldnull
    IL_00a0:  ret

    IL_00a1:  ldstr      "Default"
    IL_00a6:  ret
  } // end of method Switch::SwitchOverString1

  .method public hidebysig static string 
          SwitchOverString2() cil managed
  {
    // Code size       389 (0x185)
    .maxstack  4
    .locals init (object V_0)
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
    IL_00ec:  stloc.0
    IL_00ed:  brfalse    IL_017f

    IL_00f2:  volatile.
    IL_00f4:  ldsfld     class [mscorlib]System.Collections.Hashtable '<PrivateImplementationDetails>'::'$$method0x6000006-1'
    IL_00f9:  ldloc.0
    IL_00fa:  call       instance object [mscorlib]System.Collections.Hashtable::get_Item(object)
    IL_00ff:  dup
    IL_0100:  stloc.0
    IL_0101:  brfalse.s  IL_017f

    IL_0103:  ldloc.0
    IL_0104:  unbox      [mscorlib]System.Int32
    IL_0109:  ldind.i4
    IL_010a:  switch     ( 
                          IL_013d,
                          IL_0143,
                          IL_0149,
                          IL_014f,
                          IL_0155,
                          IL_015b,
                          IL_0161,
                          IL_0167,
                          IL_016d,
                          IL_0173,
                          IL_0179)
    IL_013b:  br.s       IL_017f

    IL_013d:  ldstr      "Text1"
    IL_0142:  ret

    IL_0143:  ldstr      "Text2"
    IL_0148:  ret

    IL_0149:  ldstr      "Text3"
    IL_014e:  ret

    IL_014f:  ldstr      "Text4"
    IL_0154:  ret

    IL_0155:  ldstr      "Text5"
    IL_015a:  ret

    IL_015b:  ldstr      "Text6"
    IL_0160:  ret

    IL_0161:  ldstr      "Text7"
    IL_0166:  ret

    IL_0167:  ldstr      "Text8"
    IL_016c:  ret

    IL_016d:  ldstr      "Text9"
    IL_0172:  ret

    IL_0173:  ldstr      "Text10"
    IL_0178:  ret

    IL_0179:  ldstr      "Text11"
    IL_017e:  ret

    IL_017f:  ldstr      "Default"
    IL_0184:  ret
  } // end of method Switch::SwitchOverString2

  .method public hidebysig static string 
          TwoDifferentSwitchBlocksInTryFinally() cil managed
  {
    // Code size       923 (0x39b)
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
      IL_0222:  leave      IL_0399

      IL_0227:  ldstr      "Text2"
      IL_022c:  stloc.0
      IL_022d:  leave      IL_0399

      IL_0232:  ldstr      "Text3"
      IL_0237:  stloc.0
      IL_0238:  leave      IL_0399

      IL_023d:  ldstr      "Text4"
      IL_0242:  stloc.0
      IL_0243:  leave      IL_0399

      IL_0248:  ldstr      "Text5"
      IL_024d:  stloc.0
      IL_024e:  leave      IL_0399

      IL_0253:  ldstr      "Text6"
      IL_0258:  stloc.0
      IL_0259:  leave      IL_0399

      IL_025e:  ldstr      "Text7"
      IL_0263:  stloc.0
      IL_0264:  leave      IL_0399

      IL_0269:  ldstr      "Text8"
      IL_026e:  stloc.0
      IL_026f:  leave      IL_0399

      IL_0274:  ldstr      "Text9"
      IL_0279:  stloc.0
      IL_027a:  leave      IL_0399

      IL_027f:  ldstr      "Text10"
      IL_0284:  stloc.0
      IL_0285:  leave      IL_0399

      IL_028a:  ldstr      "Text11"
      IL_028f:  stloc.0
      IL_0290:  leave      IL_0399

      IL_0295:  ldstr      "Default"
      IL_029a:  stloc.0
      IL_029b:  leave      IL_0399

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
      IL_0311:  br         IL_0398

      IL_0316:  ldstr      "Te223443xt2"
      IL_031b:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0320:  br.s       IL_0398

      IL_0322:  ldstr      "Te234xt3"
      IL_0327:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_032c:  br.s       IL_0398

      IL_032e:  ldstr      "Tex243t4"
      IL_0333:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0338:  br.s       IL_0398

      IL_033a:  ldstr      "Tex243t5"
      IL_033f:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0344:  br.s       IL_0398

      IL_0346:  ldstr      "Text2346"
      IL_034b:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0350:  br.s       IL_0398

      IL_0352:  ldstr      "Text234234"
      IL_0357:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_035c:  br.s       IL_0398

      IL_035e:  ldstr      "Text8234"
      IL_0363:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0368:  br.s       IL_0398

      IL_036a:  ldstr      "Text923423"
      IL_036f:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0374:  br.s       IL_0398

      IL_0376:  ldstr      "Text10"
      IL_037b:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0380:  br.s       IL_0398

      IL_0382:  ldstr      "Text1134123"
      IL_0387:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_038c:  br.s       IL_0398

      IL_038e:  ldstr      "Defa234234ult"
      IL_0393:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0398:  endfinally
    }  // end handler
    IL_0399:  ldloc.0
    IL_039a:  ret
  } // end of method Switch::TwoDifferentSwitchBlocksInTryFinally

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
    // Code size       112 (0x70)
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
                          IL_0054,
                          IL_0049)
    IL_002f:  br.s       IL_0054

    IL_0031:  ldstr      "one"
    IL_0036:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003b:  br.s       IL_0069

    IL_003d:  ldstr      "two"
    IL_0042:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0047:  br.s       IL_0069

    IL_0049:  ldstr      "four"
    IL_004e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0053:  ret

    IL_0054:  ldstr      "default"
    IL_0059:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005e:  ldstr      "more code"
    IL_0063:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0068:  ret

    IL_0069:  ldarg.0
    IL_006a:  ldc.i4.1
    IL_006b:  add
    IL_006c:  starg.s    i
    IL_006e:  br.s       IL_0015
  } // end of method Switch::SwitchInLoop

  .method public hidebysig static void  SwitchWithGoto(int32 i) cil managed
  {
    // Code size       115 (0x73)
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
                          IL_0053)
    IL_002f:  br.s       IL_005e

    IL_0031:  ldstr      "one"
    IL_0036:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003b:  br.s       IL_005e

    IL_003d:  ldstr      "two"
    IL_0042:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0047:  ldstr      "three"
    IL_004c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0051:  br.s       IL_0068

    IL_0053:  ldstr      "four"
    IL_0058:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005d:  ret

    IL_005e:  ldstr      "default"
    IL_0063:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0068:  ldstr      "End of method"
    IL_006d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0072:  ret
  } // end of method Switch::SwitchWithGoto

  .method private hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty[] 
          GetProperties() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    IL_0000:  ldc.i4.0
    IL_0001:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty
    IL_0006:  ret
  } // end of method Switch::GetProperties

  .method public hidebysig static void  SwitchOnStringInForLoop() cil managed
  {
    // Code size       267 (0x10b)
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
    IL_0014:  br         IL_0101

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
    IL_00ae:  br.s       IL_00fd

    IL_00b0:  ldloc.s    V_4
    IL_00b2:  ldc.i4.2
    IL_00b3:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00b8:  ldloc.0
    IL_00b9:  ldloc.s    V_4
    IL_00bb:  callvirt   instance int32 [mscorlib]System.Collections.ArrayList::Add(object)
    IL_00c0:  pop
    IL_00c1:  br.s       IL_00fd

    IL_00c3:  ldloc.s    V_4
    IL_00c5:  ldc.i4.3
    IL_00c6:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00cb:  ldloc.0
    IL_00cc:  ldloc.s    V_4
    IL_00ce:  callvirt   instance int32 [mscorlib]System.Collections.ArrayList::Add(object)
    IL_00d3:  pop
    IL_00d4:  br.s       IL_00fd

    IL_00d6:  ldloc.s    V_4
    IL_00d8:  ldc.i4.4
    IL_00d9:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00de:  ldloc.0
    IL_00df:  ldloc.s    V_4
    IL_00e1:  callvirt   instance int32 [mscorlib]System.Collections.ArrayList::Add(object)
    IL_00e6:  pop
    IL_00e7:  br.s       IL_00fd

    IL_00e9:  ldloc.0
    IL_00ea:  ldloc.s    V_4
    IL_00ec:  callvirt   instance int32 [mscorlib]System.Collections.ArrayList::Add(object)
    IL_00f1:  pop
    IL_00f2:  br.s       IL_00fd

    IL_00f4:  ldloc.1
    IL_00f5:  ldloc.s    V_4
    IL_00f7:  callvirt   instance int32 [mscorlib]System.Collections.ArrayList::Add(object)
    IL_00fc:  pop
    IL_00fd:  ldloc.3
    IL_00fe:  ldc.i4.1
    IL_00ff:  add
    IL_0100:  stloc.3
    IL_0101:  ldloc.3
    IL_0102:  ldloc.2
    IL_0103:  ldlen
    IL_0104:  conv.i4
    IL_0105:  blt        IL_0019

    IL_010a:  ret
  } // end of method Switch::SwitchOnStringInForLoop

  .method public hidebysig static void  SwitchWithComplexCondition(string[] args) cil managed
  {
    // Code size       139 (0x8b)
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
    IL_0027:  brfalse.s  IL_0080

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

    IL_0050:  br.s       IL_0080

    IL_0052:  ldstr      "a"
    IL_0057:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005c:  br.s       IL_0080

    IL_005e:  ldstr      "b"
    IL_0063:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0068:  br.s       IL_0080

    IL_006a:  ldstr      "c"
    IL_006f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0074:  br.s       IL_0080

    IL_0076:  ldstr      "d"
    IL_007b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0080:  ldstr      "end"
    IL_0085:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_008a:  ret
  } // end of method Switch::SwitchWithComplexCondition

  .method public hidebysig static void  SwitchWithArray(string[] args) cil managed
  {
    // Code size       127 (0x7f)
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
    IL_001b:  brfalse.s  IL_0074

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

    IL_0044:  br.s       IL_0074

    IL_0046:  ldstr      "a"
    IL_004b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0050:  br.s       IL_0074

    IL_0052:  ldstr      "b"
    IL_0057:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005c:  br.s       IL_0074

    IL_005e:  ldstr      "c"
    IL_0063:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0068:  br.s       IL_0074

    IL_006a:  ldstr      "d"
    IL_006f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0074:  ldstr      "end"
    IL_0079:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007e:  ret
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
// WARNING: Created Win32 resource file C:\Users\Siegfried\Projects\ILSpy master\ICSharpCode.Decompiler.Tests\TestCases\ILPretty\CS1xSwitch_Release.res
