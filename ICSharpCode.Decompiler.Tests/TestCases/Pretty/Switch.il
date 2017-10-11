
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation. Alle Rechte vorbehalten.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly nyniamwr
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module nyniamwr.dll
// MVID: {95C99B41-CBA3-42E4-A4DE-27E535671AB2}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03080000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch
       extends [mscorlib]System.Object
{
  .class auto ansi nested public beforefieldinit SetProperty
         extends [mscorlib]System.Object
  {
    .field public initonly class [mscorlib]System.Reflection.PropertyInfo Property
    .field private int32 '<Set>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig specialname 
            instance int32  get_Set() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::'<Set>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method SetProperty::get_Set

    .method public hidebysig specialname 
            instance void  set_Set(int32 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::'<Set>k__BackingField'
      IL_0007:  ret
    } // end of method SetProperty::set_Set

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(class [mscorlib]System.Reflection.PropertyInfo 'property') cil managed
    {
      // Code size       17 (0x11)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  nop
      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  stfld      class [mscorlib]System.Reflection.PropertyInfo ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::Property
      IL_000f:  nop
      IL_0010:  ret
    } // end of method SetProperty::.ctor

    .property instance int32 Set()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::get_Set()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    } // end of property SetProperty::Set
  } // end of class SetProperty

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
          SwitchOverNullableInt(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       79 (0x4f)
    .maxstack  2
    .locals init (string V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarga.s   i
    IL_0003:  dup
    IL_0004:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0009:  stloc.1
    IL_000a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000f:  brfalse.s  IL_0020

    IL_0011:  ldloc.1
    IL_0012:  ldc.i4.0
    IL_0013:  beq.s      IL_0029

    IL_0015:  ldloc.1
    IL_0016:  ldc.i4.5
    IL_0017:  beq.s      IL_0032

    IL_0019:  ldloc.1
    IL_001a:  ldc.i4.s   10
    IL_001c:  beq.s      IL_003b

    IL_001e:  br.s       IL_0044

    IL_0020:  nop
    IL_0021:  ldstr      "null"
    IL_0026:  stloc.0
    IL_0027:  br.s       IL_004d

    IL_0029:  nop
    IL_002a:  ldstr      "zero"
    IL_002f:  stloc.0
    IL_0030:  br.s       IL_004d

    IL_0032:  nop
    IL_0033:  ldstr      "five"
    IL_0038:  stloc.0
    IL_0039:  br.s       IL_004d

    IL_003b:  nop
    IL_003c:  ldstr      "ten"
    IL_0041:  stloc.0
    IL_0042:  br.s       IL_004d

    IL_0044:  nop
    IL_0045:  ldstr      "large"
    IL_004a:  stloc.0
    IL_004b:  br.s       IL_004d

    IL_004d:  ldloc.0
    IL_004e:  ret
  } // end of method Switch::SwitchOverNullableInt

  .method public hidebysig static string 
          SwitchOverNullableIntShifted(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       117 (0x75)
    .maxstack  2
    .locals init (string V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             int32 V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldloca.s   V_1
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_0017

    IL_000c:  ldloca.s   V_2
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0014:  ldloc.2
    IL_0015:  br.s       IL_0025

    IL_0017:  ldloca.s   V_1
    IL_0019:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001e:  ldc.i4.5
    IL_001f:  add
    IL_0020:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0025:  nop
    IL_0026:  stloc.2
    IL_0027:  ldloca.s   V_2
    IL_0029:  dup
    IL_002a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_002f:  stloc.3
    IL_0030:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0035:  brfalse.s  IL_0046

    IL_0037:  ldloc.3
    IL_0038:  ldc.i4.0
    IL_0039:  beq.s      IL_004f

    IL_003b:  ldloc.3
    IL_003c:  ldc.i4.5
    IL_003d:  beq.s      IL_0058

    IL_003f:  ldloc.3
    IL_0040:  ldc.i4.s   10
    IL_0042:  beq.s      IL_0061

    IL_0044:  br.s       IL_006a

    IL_0046:  nop
    IL_0047:  ldstr      "null"
    IL_004c:  stloc.0
    IL_004d:  br.s       IL_0073

    IL_004f:  nop
    IL_0050:  ldstr      "zero"
    IL_0055:  stloc.0
    IL_0056:  br.s       IL_0073

    IL_0058:  nop
    IL_0059:  ldstr      "five"
    IL_005e:  stloc.0
    IL_005f:  br.s       IL_0073

    IL_0061:  nop
    IL_0062:  ldstr      "ten"
    IL_0067:  stloc.0
    IL_0068:  br.s       IL_0073

    IL_006a:  nop
    IL_006b:  ldstr      "large"
    IL_0070:  stloc.0
    IL_0071:  br.s       IL_0073

    IL_0073:  ldloc.0
    IL_0074:  ret
  } // end of method Switch::SwitchOverNullableIntShifted

  .method public hidebysig static string 
          SwitchOverNullableIntNoNullCase(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       70 (0x46)
    .maxstack  2
    .locals init (string V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarga.s   i
    IL_0003:  dup
    IL_0004:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0009:  stloc.1
    IL_000a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000f:  brfalse.s  IL_003b

    IL_0011:  ldloc.1
    IL_0012:  ldc.i4.0
    IL_0013:  beq.s      IL_0020

    IL_0015:  ldloc.1
    IL_0016:  ldc.i4.5
    IL_0017:  beq.s      IL_0029

    IL_0019:  ldloc.1
    IL_001a:  ldc.i4.s   10
    IL_001c:  beq.s      IL_0032

    IL_001e:  br.s       IL_003b

    IL_0020:  nop
    IL_0021:  ldstr      "zero"
    IL_0026:  stloc.0
    IL_0027:  br.s       IL_0044

    IL_0029:  nop
    IL_002a:  ldstr      "five"
    IL_002f:  stloc.0
    IL_0030:  br.s       IL_0044

    IL_0032:  nop
    IL_0033:  ldstr      "ten"
    IL_0038:  stloc.0
    IL_0039:  br.s       IL_0044

    IL_003b:  nop
    IL_003c:  ldstr      "other"
    IL_0041:  stloc.0
    IL_0042:  br.s       IL_0044

    IL_0044:  ldloc.0
    IL_0045:  ret
  } // end of method Switch::SwitchOverNullableIntNoNullCase

  .method public hidebysig static string 
          SwitchOverNullableIntNoNullCaseShifted(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       108 (0x6c)
    .maxstack  2
    .locals init (string V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             int32 V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldloca.s   V_1
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_0017

    IL_000c:  ldloca.s   V_2
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0014:  ldloc.2
    IL_0015:  br.s       IL_0025

    IL_0017:  ldloca.s   V_1
    IL_0019:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001e:  ldc.i4.5
    IL_001f:  add
    IL_0020:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0025:  nop
    IL_0026:  stloc.2
    IL_0027:  ldloca.s   V_2
    IL_0029:  dup
    IL_002a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_002f:  stloc.3
    IL_0030:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0035:  brfalse.s  IL_0061

    IL_0037:  ldloc.3
    IL_0038:  ldc.i4.0
    IL_0039:  beq.s      IL_0046

    IL_003b:  ldloc.3
    IL_003c:  ldc.i4.5
    IL_003d:  beq.s      IL_004f

    IL_003f:  ldloc.3
    IL_0040:  ldc.i4.s   10
    IL_0042:  beq.s      IL_0058

    IL_0044:  br.s       IL_0061

    IL_0046:  nop
    IL_0047:  ldstr      "zero"
    IL_004c:  stloc.0
    IL_004d:  br.s       IL_006a

    IL_004f:  nop
    IL_0050:  ldstr      "five"
    IL_0055:  stloc.0
    IL_0056:  br.s       IL_006a

    IL_0058:  nop
    IL_0059:  ldstr      "ten"
    IL_005e:  stloc.0
    IL_005f:  br.s       IL_006a

    IL_0061:  nop
    IL_0062:  ldstr      "other"
    IL_0067:  stloc.0
    IL_0068:  br.s       IL_006a

    IL_006a:  ldloc.0
    IL_006b:  ret
  } // end of method Switch::SwitchOverNullableIntNoNullCaseShifted

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
    IL_001c:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{95C99B41-CBA3-42E4-A4DE-27E535671AB2}'::'$$method0x6000007-1'
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
    IL_007f:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{95C99B41-CBA3-42E4-A4DE-27E535671AB2}'::'$$method0x6000007-1'
    IL_0084:  volatile.
    IL_0086:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{95C99B41-CBA3-42E4-A4DE-27E535671AB2}'::'$$method0x6000007-1'
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
    // Code size       368 (0x170)
    .maxstack  4
    .locals init (string V_0,
             string V_1,
             string V_2,
             int32 V_3)
    IL_0000:  nop
    IL_0001:  ldstr      "SwitchOverString2:"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  call       string [mscorlib]System.Environment::get_UserName()
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  stloc.2
    IL_0014:  ldloc.2
    IL_0015:  brfalse    IL_0165

    IL_001a:  volatile.
    IL_001c:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{95C99B41-CBA3-42E4-A4DE-27E535671AB2}'::'$$method0x6000008-1'
    IL_0021:  brtrue     IL_00ba

    IL_0026:  ldc.i4.s   11
    IL_0028:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::.ctor(int32)
    IL_002d:  dup
    IL_002e:  ldstr      "First case"
    IL_0033:  ldc.i4.0
    IL_0034:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0039:  dup
    IL_003a:  ldstr      "Second case"
    IL_003f:  ldc.i4.1
    IL_0040:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0045:  dup
    IL_0046:  ldstr      "Third case"
    IL_004b:  ldc.i4.2
    IL_004c:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0051:  dup
    IL_0052:  ldstr      "Fourth case"
    IL_0057:  ldc.i4.3
    IL_0058:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_005d:  dup
    IL_005e:  ldstr      "Fifth case"
    IL_0063:  ldc.i4.4
    IL_0064:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0069:  dup
    IL_006a:  ldstr      "Sixth case"
    IL_006f:  ldc.i4.5
    IL_0070:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0075:  dup
    IL_0076:  ldstr      "Seventh case"
    IL_007b:  ldc.i4.6
    IL_007c:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0081:  dup
    IL_0082:  ldstr      "Eighth case"
    IL_0087:  ldc.i4.7
    IL_0088:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_008d:  dup
    IL_008e:  ldstr      "Ninth case"
    IL_0093:  ldc.i4.8
    IL_0094:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0099:  dup
    IL_009a:  ldstr      "Tenth case"
    IL_009f:  ldc.i4.s   9
    IL_00a1:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_00a6:  dup
    IL_00a7:  ldstr      "Eleventh case"
    IL_00ac:  ldc.i4.s   10
    IL_00ae:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_00b3:  volatile.
    IL_00b5:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{95C99B41-CBA3-42E4-A4DE-27E535671AB2}'::'$$method0x6000008-1'
    IL_00ba:  volatile.
    IL_00bc:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{95C99B41-CBA3-42E4-A4DE-27E535671AB2}'::'$$method0x6000008-1'
    IL_00c1:  ldloc.2
    IL_00c2:  ldloca.s   V_3
    IL_00c4:  call       instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::TryGetValue(!0,
                                                                                                                          !1&)
    IL_00c9:  brfalse    IL_0165

    IL_00ce:  ldloc.3
    IL_00cf:  switch     ( 
                          IL_0102,
                          IL_010b,
                          IL_0114,
                          IL_011d,
                          IL_0126,
                          IL_012f,
                          IL_0138,
                          IL_0141,
                          IL_014a,
                          IL_0153,
                          IL_015c)
    IL_0100:  br.s       IL_0165

    IL_0102:  nop
    IL_0103:  ldstr      "Text1"
    IL_0108:  stloc.1
    IL_0109:  br.s       IL_016e

    IL_010b:  nop
    IL_010c:  ldstr      "Text2"
    IL_0111:  stloc.1
    IL_0112:  br.s       IL_016e

    IL_0114:  nop
    IL_0115:  ldstr      "Text3"
    IL_011a:  stloc.1
    IL_011b:  br.s       IL_016e

    IL_011d:  nop
    IL_011e:  ldstr      "Text4"
    IL_0123:  stloc.1
    IL_0124:  br.s       IL_016e

    IL_0126:  nop
    IL_0127:  ldstr      "Text5"
    IL_012c:  stloc.1
    IL_012d:  br.s       IL_016e

    IL_012f:  nop
    IL_0130:  ldstr      "Text6"
    IL_0135:  stloc.1
    IL_0136:  br.s       IL_016e

    IL_0138:  nop
    IL_0139:  ldstr      "Text7"
    IL_013e:  stloc.1
    IL_013f:  br.s       IL_016e

    IL_0141:  nop
    IL_0142:  ldstr      "Text8"
    IL_0147:  stloc.1
    IL_0148:  br.s       IL_016e

    IL_014a:  nop
    IL_014b:  ldstr      "Text9"
    IL_0150:  stloc.1
    IL_0151:  br.s       IL_016e

    IL_0153:  nop
    IL_0154:  ldstr      "Text10"
    IL_0159:  stloc.1
    IL_015a:  br.s       IL_016e

    IL_015c:  nop
    IL_015d:  ldstr      "Text11"
    IL_0162:  stloc.1
    IL_0163:  br.s       IL_016e

    IL_0165:  nop
    IL_0166:  ldstr      "Default"
    IL_016b:  stloc.1
    IL_016c:  br.s       IL_016e

    IL_016e:  ldloc.1
    IL_016f:  ret
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
    // Code size       146 (0x92)
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
    IL_0017:  br.s       IL_008d

    IL_0019:  nop
    IL_001a:  ldarg.0
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.1
    IL_001e:  sub
    IL_001f:  switch     ( 
                          IL_0036,
                          IL_0044,
                          IL_0052,
                          IL_0060)
    IL_0034:  br.s       IL_006e

    IL_0036:  nop
    IL_0037:  ldstr      "one"
    IL_003c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0041:  nop
    IL_0042:  br.s       IL_0087

    IL_0044:  nop
    IL_0045:  ldstr      "two"
    IL_004a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004f:  nop
    IL_0050:  br.s       IL_0087

    IL_0052:  nop
    IL_0053:  ldstr      "three"
    IL_0058:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005d:  nop
    IL_005e:  br.s       IL_008d

    IL_0060:  nop
    IL_0061:  ldstr      "four"
    IL_0066:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006b:  nop
    IL_006c:  br.s       IL_0091

    IL_006e:  nop
    IL_006f:  ldstr      "default"
    IL_0074:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0079:  nop
    IL_007a:  ldstr      "more code"
    IL_007f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0084:  nop
    IL_0085:  br.s       IL_0091

    IL_0087:  ldarg.0
    IL_0088:  ldc.i4.1
    IL_0089:  add
    IL_008a:  starg.s    i
    IL_008c:  nop
    IL_008d:  ldc.i4.1
    IL_008e:  stloc.1
    IL_008f:  br.s       IL_0019

    IL_0091:  ret
  } // end of method Switch::SwitchInLoop

  .method public hidebysig static void  SwitchWithGoto(int32 i) cil managed
  {
    // Code size       122 (0x7a)
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
                          IL_0041,
                          IL_004f,
                          IL_005d)
    IL_0031:  br.s       IL_006b

    IL_0033:  nop
    IL_0034:  ldstr      "one"
    IL_0039:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003e:  nop
    IL_003f:  br.s       IL_006b

    IL_0041:  nop
    IL_0042:  ldstr      "two"
    IL_0047:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004c:  nop
    IL_004d:  br.s       IL_004f

    IL_004f:  nop
    IL_0050:  ldstr      "three"
    IL_0055:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005a:  nop
    IL_005b:  br.s       IL_0079

    IL_005d:  nop
    IL_005e:  ldstr      "four"
    IL_0063:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0068:  nop
    IL_0069:  br.s       IL_0079

    IL_006b:  nop
    IL_006c:  ldstr      "default"
    IL_0071:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0076:  nop
    IL_0077:  br.s       IL_0079

    IL_0079:  ret
  } // end of method Switch::SwitchWithGoto

  .method private hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty[] 
          GetProperties() cil managed
  {
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty[] V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method Switch::GetProperties

  .method public hidebysig static void  SwitchOnStringInForLoop() cil managed
  {
    // Code size       334 (0x14e)
    .maxstack  4
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty> V_0,
             class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty> V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty[] V_2,
             int32 V_3,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty V_4,
             string V_5,
             string V_6,
             int32 V_7,
             bool V_8)
    IL_0000:  nop
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::.ctor()
    IL_0006:  stloc.0
    IL_0007:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::.ctor()
    IL_000c:  stloc.1
    IL_000d:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch::GetProperties()
    IL_0012:  stloc.2
    IL_0013:  ldc.i4.0
    IL_0014:  stloc.3
    IL_0015:  br         IL_013e

    IL_001a:  nop
    IL_001b:  ldloc.2
    IL_001c:  ldloc.3
    IL_001d:  ldelem.ref
    IL_001e:  stloc.s    V_4
    IL_0020:  ldloc.s    V_4
    IL_0022:  ldfld      class [mscorlib]System.Reflection.PropertyInfo ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::Property
    IL_0027:  callvirt   instance string [mscorlib]System.Reflection.MemberInfo::get_Name()
    IL_002c:  stloc.s    V_5
    IL_002e:  ldloc.s    V_5
    IL_0030:  stloc.s    V_6
    IL_0032:  ldloc.s    V_6
    IL_0034:  brfalse    IL_012d

    IL_0039:  volatile.
    IL_003b:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{95C99B41-CBA3-42E4-A4DE-27E535671AB2}'::'$$method0x600000d-1'
    IL_0040:  brtrue.s   IL_0097

    IL_0042:  ldc.i4.6
    IL_0043:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::.ctor(int32)
    IL_0048:  dup
    IL_0049:  ldstr      "Name1"
    IL_004e:  ldc.i4.0
    IL_004f:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0054:  dup
    IL_0055:  ldstr      "Name2"
    IL_005a:  ldc.i4.1
    IL_005b:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0060:  dup
    IL_0061:  ldstr      "Name3"
    IL_0066:  ldc.i4.2
    IL_0067:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_006c:  dup
    IL_006d:  ldstr      "Name4"
    IL_0072:  ldc.i4.3
    IL_0073:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0078:  dup
    IL_0079:  ldstr      "Name5"
    IL_007e:  ldc.i4.4
    IL_007f:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0084:  dup
    IL_0085:  ldstr      "Name6"
    IL_008a:  ldc.i4.5
    IL_008b:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0090:  volatile.
    IL_0092:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{95C99B41-CBA3-42E4-A4DE-27E535671AB2}'::'$$method0x600000d-1'
    IL_0097:  volatile.
    IL_0099:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>{95C99B41-CBA3-42E4-A4DE-27E535671AB2}'::'$$method0x600000d-1'
    IL_009e:  ldloc.s    V_6
    IL_00a0:  ldloca.s   V_7
    IL_00a2:  call       instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::TryGetValue(!0,
                                                                                                                          !1&)
    IL_00a7:  brfalse    IL_012d

    IL_00ac:  ldloc.s    V_7
    IL_00ae:  switch     ( 
                          IL_00cd,
                          IL_00e2,
                          IL_00f7,
                          IL_010c,
                          IL_0121,
                          IL_0121)
    IL_00cb:  br.s       IL_012d

    IL_00cd:  nop
    IL_00ce:  ldloc.s    V_4
    IL_00d0:  ldc.i4.1
    IL_00d1:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00d6:  nop
    IL_00d7:  ldloc.0
    IL_00d8:  ldloc.s    V_4
    IL_00da:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00df:  nop
    IL_00e0:  br.s       IL_0139

    IL_00e2:  nop
    IL_00e3:  ldloc.s    V_4
    IL_00e5:  ldc.i4.2
    IL_00e6:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00eb:  nop
    IL_00ec:  ldloc.0
    IL_00ed:  ldloc.s    V_4
    IL_00ef:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00f4:  nop
    IL_00f5:  br.s       IL_0139

    IL_00f7:  nop
    IL_00f8:  ldloc.s    V_4
    IL_00fa:  ldc.i4.3
    IL_00fb:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_0100:  nop
    IL_0101:  ldloc.0
    IL_0102:  ldloc.s    V_4
    IL_0104:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_0109:  nop
    IL_010a:  br.s       IL_0139

    IL_010c:  nop
    IL_010d:  ldloc.s    V_4
    IL_010f:  ldc.i4.4
    IL_0110:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_0115:  nop
    IL_0116:  ldloc.0
    IL_0117:  ldloc.s    V_4
    IL_0119:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_011e:  nop
    IL_011f:  br.s       IL_0139

    IL_0121:  nop
    IL_0122:  ldloc.0
    IL_0123:  ldloc.s    V_4
    IL_0125:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_012a:  nop
    IL_012b:  br.s       IL_0139

    IL_012d:  nop
    IL_012e:  ldloc.1
    IL_012f:  ldloc.s    V_4
    IL_0131:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_0136:  nop
    IL_0137:  br.s       IL_0139

    IL_0139:  nop
    IL_013a:  ldloc.3
    IL_013b:  ldc.i4.1
    IL_013c:  add
    IL_013d:  stloc.3
    IL_013e:  ldloc.3
    IL_013f:  ldloc.2
    IL_0140:  ldlen
    IL_0141:  conv.i4
    IL_0142:  clt
    IL_0144:  stloc.s    V_8
    IL_0146:  ldloc.s    V_8
    IL_0148:  brtrue     IL_001a

    IL_014d:  ret
  } // end of method Switch::SwitchOnStringInForLoop

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch

.class private auto ansi '<PrivateImplementationDetails>{95C99B41-CBA3-42E4-A4DE-27E535671AB2}'
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field static assembly class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '$$method0x6000007-1'
  .field static assembly class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '$$method0x6000008-1'
  .field static assembly class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '$$method0x600000d-1'
} // end of class '<PrivateImplementationDetails>{95C99B41-CBA3-42E4-A4DE-27E535671AB2}'


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// Warnung: Win32-Ressourcendatei "../../../TestCases/Pretty\Switch.res" wurde erstellt.
