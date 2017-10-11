
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation. Alle Rechte vorbehalten.



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
// MVID: {B5801C4A-C979-4EC9-B5B2-295587FEEF30}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x005D0000


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
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
    .method public hidebysig specialname 
            instance int32  get_Set() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::'<Set>k__BackingField'
      IL_0006:  ret
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
      // Code size       16 (0x10)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  nop
      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  stfld      class [mscorlib]System.Reflection.PropertyInfo ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::Property
      IL_000f:  ret
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
          SwitchOverNullableInt(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       87 (0x57)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             int32 V_2,
             string V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldloc.1
    IL_0004:  stloc.0
    IL_0005:  ldloca.s   V_0
    IL_0007:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000c:  brfalse.s  IL_0028

    IL_000e:  ldloca.s   V_0
    IL_0010:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0015:  stloc.2
    IL_0016:  ldloc.2
    IL_0017:  brfalse.s  IL_0031

    IL_0019:  br.s       IL_001b

    IL_001b:  ldloc.2
    IL_001c:  ldc.i4.5
    IL_001d:  beq.s      IL_003a

    IL_001f:  br.s       IL_0021

    IL_0021:  ldloc.2
    IL_0022:  ldc.i4.s   10
    IL_0024:  beq.s      IL_0043

    IL_0026:  br.s       IL_004c

    IL_0028:  nop
    IL_0029:  ldstr      "null"
    IL_002e:  stloc.3
    IL_002f:  br.s       IL_0055

    IL_0031:  nop
    IL_0032:  ldstr      "zero"
    IL_0037:  stloc.3
    IL_0038:  br.s       IL_0055

    IL_003a:  nop
    IL_003b:  ldstr      "five"
    IL_0040:  stloc.3
    IL_0041:  br.s       IL_0055

    IL_0043:  nop
    IL_0044:  ldstr      "ten"
    IL_0049:  stloc.3
    IL_004a:  br.s       IL_0055

    IL_004c:  nop
    IL_004d:  ldstr      "large"
    IL_0052:  stloc.3
    IL_0053:  br.s       IL_0055

    IL_0055:  ldloc.3
    IL_0056:  ret
  } // end of method Switch::SwitchOverNullableInt

  .method public hidebysig static string 
          SwitchOverNullableIntShifted(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       132 (0x84)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             valuetype [mscorlib]System.Nullable`1<int32> V_3,
             int32 V_4,
             string V_5)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.2
    IL_0003:  ldloca.s   V_2
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_0017

    IL_000c:  ldloca.s   V_3
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0014:  ldloc.3
    IL_0015:  br.s       IL_0025

    IL_0017:  ldloca.s   V_2
    IL_0019:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001e:  ldc.i4.5
    IL_001f:  add
    IL_0020:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0025:  stloc.1
    IL_0026:  ldloc.1
    IL_0027:  stloc.0
    IL_0028:  ldloca.s   V_0
    IL_002a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_002f:  brfalse.s  IL_004f

    IL_0031:  ldloca.s   V_0
    IL_0033:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0038:  stloc.s    V_4
    IL_003a:  ldloc.s    V_4
    IL_003c:  brfalse.s  IL_0059

    IL_003e:  br.s       IL_0040

    IL_0040:  ldloc.s    V_4
    IL_0042:  ldc.i4.5
    IL_0043:  beq.s      IL_0063

    IL_0045:  br.s       IL_0047

    IL_0047:  ldloc.s    V_4
    IL_0049:  ldc.i4.s   10
    IL_004b:  beq.s      IL_006d

    IL_004d:  br.s       IL_0077

    IL_004f:  nop
    IL_0050:  ldstr      "null"
    IL_0055:  stloc.s    V_5
    IL_0057:  br.s       IL_0081

    IL_0059:  nop
    IL_005a:  ldstr      "zero"
    IL_005f:  stloc.s    V_5
    IL_0061:  br.s       IL_0081

    IL_0063:  nop
    IL_0064:  ldstr      "five"
    IL_0069:  stloc.s    V_5
    IL_006b:  br.s       IL_0081

    IL_006d:  nop
    IL_006e:  ldstr      "ten"
    IL_0073:  stloc.s    V_5
    IL_0075:  br.s       IL_0081

    IL_0077:  nop
    IL_0078:  ldstr      "large"
    IL_007d:  stloc.s    V_5
    IL_007f:  br.s       IL_0081

    IL_0081:  ldloc.s    V_5
    IL_0083:  ret
  } // end of method Switch::SwitchOverNullableIntShifted

  .method public hidebysig static string 
          SwitchOverNullableIntNoNullCase(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       78 (0x4e)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             int32 V_2,
             string V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldloc.1
    IL_0004:  stloc.0
    IL_0005:  ldloca.s   V_0
    IL_0007:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000c:  brfalse.s  IL_0043

    IL_000e:  ldloca.s   V_0
    IL_0010:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0015:  stloc.2
    IL_0016:  ldloc.2
    IL_0017:  brfalse.s  IL_0028

    IL_0019:  br.s       IL_001b

    IL_001b:  ldloc.2
    IL_001c:  ldc.i4.5
    IL_001d:  beq.s      IL_0031

    IL_001f:  br.s       IL_0021

    IL_0021:  ldloc.2
    IL_0022:  ldc.i4.s   10
    IL_0024:  beq.s      IL_003a

    IL_0026:  br.s       IL_0043

    IL_0028:  nop
    IL_0029:  ldstr      "zero"
    IL_002e:  stloc.3
    IL_002f:  br.s       IL_004c

    IL_0031:  nop
    IL_0032:  ldstr      "five"
    IL_0037:  stloc.3
    IL_0038:  br.s       IL_004c

    IL_003a:  nop
    IL_003b:  ldstr      "ten"
    IL_0040:  stloc.3
    IL_0041:  br.s       IL_004c

    IL_0043:  nop
    IL_0044:  ldstr      "other"
    IL_0049:  stloc.3
    IL_004a:  br.s       IL_004c

    IL_004c:  ldloc.3
    IL_004d:  ret
  } // end of method Switch::SwitchOverNullableIntNoNullCase

  .method public hidebysig static string 
          SwitchOverNullableIntNoNullCaseShifted(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       122 (0x7a)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             valuetype [mscorlib]System.Nullable`1<int32> V_3,
             int32 V_4,
             string V_5)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.2
    IL_0003:  ldloca.s   V_2
    IL_0005:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000a:  brtrue.s   IL_0017

    IL_000c:  ldloca.s   V_3
    IL_000e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0014:  ldloc.3
    IL_0015:  br.s       IL_0025

    IL_0017:  ldloca.s   V_2
    IL_0019:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001e:  ldc.i4.5
    IL_001f:  add
    IL_0020:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0025:  stloc.1
    IL_0026:  ldloc.1
    IL_0027:  stloc.0
    IL_0028:  ldloca.s   V_0
    IL_002a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_002f:  brfalse.s  IL_006d

    IL_0031:  ldloca.s   V_0
    IL_0033:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0038:  stloc.s    V_4
    IL_003a:  ldloc.s    V_4
    IL_003c:  brfalse.s  IL_004f

    IL_003e:  br.s       IL_0040

    IL_0040:  ldloc.s    V_4
    IL_0042:  ldc.i4.5
    IL_0043:  beq.s      IL_0059

    IL_0045:  br.s       IL_0047

    IL_0047:  ldloc.s    V_4
    IL_0049:  ldc.i4.s   10
    IL_004b:  beq.s      IL_0063

    IL_004d:  br.s       IL_006d

    IL_004f:  nop
    IL_0050:  ldstr      "zero"
    IL_0055:  stloc.s    V_5
    IL_0057:  br.s       IL_0077

    IL_0059:  nop
    IL_005a:  ldstr      "five"
    IL_005f:  stloc.s    V_5
    IL_0061:  br.s       IL_0077

    IL_0063:  nop
    IL_0064:  ldstr      "ten"
    IL_0069:  stloc.s    V_5
    IL_006b:  br.s       IL_0077

    IL_006d:  nop
    IL_006e:  ldstr      "other"
    IL_0073:  stloc.s    V_5
    IL_0075:  br.s       IL_0077

    IL_0077:  ldloc.s    V_5
    IL_0079:  ret
  } // end of method Switch::SwitchOverNullableIntNoNullCaseShifted

  .method public hidebysig static void  SwitchOverInt(int32 i) cil managed
  {
    // Code size       161 (0xa1)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  ldc.i4.s   10
    IL_0006:  bgt.s      IL_001d

    IL_0008:  ldloc.0
    IL_0009:  brfalse.s  IL_003e

    IL_000b:  br.s       IL_000d

    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.5
    IL_000f:  beq.s      IL_004c

    IL_0011:  br.s       IL_0013

    IL_0013:  ldloc.0
    IL_0014:  ldc.i4.s   10
    IL_0016:  beq.s      IL_005a

    IL_0018:  br         IL_00a0

    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.s   20
    IL_0020:  bgt.s      IL_0030

    IL_0022:  ldloc.0
    IL_0023:  ldc.i4.s   15
    IL_0025:  beq.s      IL_0068

    IL_0027:  br.s       IL_0029

    IL_0029:  ldloc.0
    IL_002a:  ldc.i4.s   20
    IL_002c:  beq.s      IL_0076

    IL_002e:  br.s       IL_00a0

    IL_0030:  ldloc.0
    IL_0031:  ldc.i4.s   25
    IL_0033:  beq.s      IL_0084

    IL_0035:  br.s       IL_0037

    IL_0037:  ldloc.0
    IL_0038:  ldc.i4.s   30
    IL_003a:  beq.s      IL_0092

    IL_003c:  br.s       IL_00a0

    IL_003e:  nop
    IL_003f:  ldstr      "zero"
    IL_0044:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0049:  nop
    IL_004a:  br.s       IL_00a0

    IL_004c:  nop
    IL_004d:  ldstr      "five"
    IL_0052:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0057:  nop
    IL_0058:  br.s       IL_00a0

    IL_005a:  nop
    IL_005b:  ldstr      "ten"
    IL_0060:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0065:  nop
    IL_0066:  br.s       IL_00a0

    IL_0068:  nop
    IL_0069:  ldstr      "fifteen"
    IL_006e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0073:  nop
    IL_0074:  br.s       IL_00a0

    IL_0076:  nop
    IL_0077:  ldstr      "twenty"
    IL_007c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0081:  nop
    IL_0082:  br.s       IL_00a0

    IL_0084:  nop
    IL_0085:  ldstr      "twenty-five"
    IL_008a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_008f:  nop
    IL_0090:  br.s       IL_00a0

    IL_0092:  nop
    IL_0093:  ldstr      "thirty"
    IL_0098:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_009d:  nop
    IL_009e:  br.s       IL_00a0

    IL_00a0:  ret
  } // end of method Switch::SwitchOverInt

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
    // Code size       520 (0x208)
    .maxstack  2
    .locals init (string V_0,
             string V_1,
             uint32 V_2,
             string V_3)
    IL_0000:  nop
    IL_0001:  ldstr      "SwitchOverString2:"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  call       string [mscorlib]System.Environment::get_UserName()
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  stloc.1
    IL_0014:  ldloc.1
    IL_0015:  call       uint32 '<PrivateImplementationDetails>'::ComputeStringHash(string)
    IL_001a:  stloc.2
    IL_001b:  ldloc.2
    IL_001c:  ldc.i4     0x4c7c71f6
    IL_0021:  bgt.un.s   IL_0072

    IL_0023:  ldloc.2
    IL_0024:  ldc.i4     0xc9a8f4f
    IL_0029:  bgt.un.s   IL_0048

    IL_002b:  ldloc.2
    IL_002c:  ldc.i4     0x8861b86
    IL_0031:  beq        IL_011c

    IL_0036:  br.s       IL_0038

    IL_0038:  ldloc.2
    IL_0039:  ldc.i4     0xc9a8f4f
    IL_003e:  beq        IL_00c8

    IL_0043:  br         IL_01fd

    IL_0048:  ldloc.2
    IL_0049:  ldc.i4     0xf3d44a6
    IL_004e:  beq        IL_00f2

    IL_0053:  br.s       IL_0055

    IL_0055:  ldloc.2
    IL_0056:  ldc.i4     0x20289804
    IL_005b:  beq        IL_0158

    IL_0060:  br.s       IL_0062

    IL_0062:  ldloc.2
    IL_0063:  ldc.i4     0x4c7c71f6
    IL_0068:  beq        IL_016a

    IL_006d:  br         IL_01fd

    IL_0072:  ldloc.2
    IL_0073:  ldc.i4     0xa151b28a
    IL_0078:  bgt.un.s   IL_00a4

    IL_007a:  ldloc.2
    IL_007b:  ldc.i4     0x4d0cea48
    IL_0080:  beq        IL_018b

    IL_0085:  br.s       IL_0087

    IL_0087:  ldloc.2
    IL_0088:  ldc.i4     0x51650fb9
    IL_008d:  beq        IL_0131

    IL_0092:  br.s       IL_0094

    IL_0094:  ldloc.2
    IL_0095:  ldc.i4     0xa151b28a
    IL_009a:  beq        IL_0146

    IL_009f:  br         IL_01fd

    IL_00a4:  ldloc.2
    IL_00a5:  ldc.i4     0xea3d096b
    IL_00aa:  beq.s      IL_00dd

    IL_00ac:  br.s       IL_00ae

    IL_00ae:  ldloc.2
    IL_00af:  ldc.i4     0xed5134d4
    IL_00b4:  beq        IL_017c

    IL_00b9:  br.s       IL_00bb

    IL_00bb:  ldloc.2
    IL_00bc:  ldc.i4     0xf701cc7f
    IL_00c1:  beq.s      IL_0107

    IL_00c3:  br         IL_01fd

    IL_00c8:  ldloc.1
    IL_00c9:  ldstr      "First case"
    IL_00ce:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00d3:  brtrue     IL_019a

    IL_00d8:  br         IL_01fd

    IL_00dd:  ldloc.1
    IL_00de:  ldstr      "Second case"
    IL_00e3:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00e8:  brtrue     IL_01a3

    IL_00ed:  br         IL_01fd

    IL_00f2:  ldloc.1
    IL_00f3:  ldstr      "Third case"
    IL_00f8:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00fd:  brtrue     IL_01ac

    IL_0102:  br         IL_01fd

    IL_0107:  ldloc.1
    IL_0108:  ldstr      "Fourth case"
    IL_010d:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0112:  brtrue     IL_01b5

    IL_0117:  br         IL_01fd

    IL_011c:  ldloc.1
    IL_011d:  ldstr      "Fifth case"
    IL_0122:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0127:  brtrue     IL_01be

    IL_012c:  br         IL_01fd

    IL_0131:  ldloc.1
    IL_0132:  ldstr      "Sixth case"
    IL_0137:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_013c:  brtrue     IL_01c7

    IL_0141:  br         IL_01fd

    IL_0146:  ldloc.1
    IL_0147:  ldstr      "Seventh case"
    IL_014c:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0151:  brtrue.s   IL_01d0

    IL_0153:  br         IL_01fd

    IL_0158:  ldloc.1
    IL_0159:  ldstr      "Eighth case"
    IL_015e:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0163:  brtrue.s   IL_01d9

    IL_0165:  br         IL_01fd

    IL_016a:  ldloc.1
    IL_016b:  ldstr      "Ninth case"
    IL_0170:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0175:  brtrue.s   IL_01e2

    IL_0177:  br         IL_01fd

    IL_017c:  ldloc.1
    IL_017d:  ldstr      "Tenth case"
    IL_0182:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0187:  brtrue.s   IL_01eb

    IL_0189:  br.s       IL_01fd

    IL_018b:  ldloc.1
    IL_018c:  ldstr      "Eleventh case"
    IL_0191:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0196:  brtrue.s   IL_01f4

    IL_0198:  br.s       IL_01fd

    IL_019a:  nop
    IL_019b:  ldstr      "Text1"
    IL_01a0:  stloc.3
    IL_01a1:  br.s       IL_0206

    IL_01a3:  nop
    IL_01a4:  ldstr      "Text2"
    IL_01a9:  stloc.3
    IL_01aa:  br.s       IL_0206

    IL_01ac:  nop
    IL_01ad:  ldstr      "Text3"
    IL_01b2:  stloc.3
    IL_01b3:  br.s       IL_0206

    IL_01b5:  nop
    IL_01b6:  ldstr      "Text4"
    IL_01bb:  stloc.3
    IL_01bc:  br.s       IL_0206

    IL_01be:  nop
    IL_01bf:  ldstr      "Text5"
    IL_01c4:  stloc.3
    IL_01c5:  br.s       IL_0206

    IL_01c7:  nop
    IL_01c8:  ldstr      "Text6"
    IL_01cd:  stloc.3
    IL_01ce:  br.s       IL_0206

    IL_01d0:  nop
    IL_01d1:  ldstr      "Text7"
    IL_01d6:  stloc.3
    IL_01d7:  br.s       IL_0206

    IL_01d9:  nop
    IL_01da:  ldstr      "Text8"
    IL_01df:  stloc.3
    IL_01e0:  br.s       IL_0206

    IL_01e2:  nop
    IL_01e3:  ldstr      "Text9"
    IL_01e8:  stloc.3
    IL_01e9:  br.s       IL_0206

    IL_01eb:  nop
    IL_01ec:  ldstr      "Text10"
    IL_01f1:  stloc.3
    IL_01f2:  br.s       IL_0206

    IL_01f4:  nop
    IL_01f5:  ldstr      "Text11"
    IL_01fa:  stloc.3
    IL_01fb:  br.s       IL_0206

    IL_01fd:  nop
    IL_01fe:  ldstr      "Default"
    IL_0203:  stloc.3
    IL_0204:  br.s       IL_0206

    IL_0206:  ldloc.3
    IL_0207:  ret
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
    // Code size       265 (0x109)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty> V_0,
             class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty> V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty[] V_2,
             int32 V_3,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty V_4,
             string V_5,
             string V_6,
             bool V_7)
    IL_0000:  nop
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::.ctor()
    IL_0006:  stloc.0
    IL_0007:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::.ctor()
    IL_000c:  stloc.1
    IL_000d:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch::GetProperties()
    IL_0012:  stloc.2
    IL_0013:  ldc.i4.0
    IL_0014:  stloc.3
    IL_0015:  br         IL_00f9

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
    IL_0034:  ldstr      "Name1"
    IL_0039:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_003e:  brtrue.s   IL_0088

    IL_0040:  ldloc.s    V_6
    IL_0042:  ldstr      "Name2"
    IL_0047:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_004c:  brtrue.s   IL_009d

    IL_004e:  ldloc.s    V_6
    IL_0050:  ldstr      "Name3"
    IL_0055:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_005a:  brtrue.s   IL_00b2

    IL_005c:  ldloc.s    V_6
    IL_005e:  ldstr      "Name4"
    IL_0063:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0068:  brtrue.s   IL_00c7

    IL_006a:  ldloc.s    V_6
    IL_006c:  ldstr      "Name5"
    IL_0071:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0076:  brtrue.s   IL_00dc

    IL_0078:  ldloc.s    V_6
    IL_007a:  ldstr      "Name6"
    IL_007f:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0084:  brtrue.s   IL_00dc

    IL_0086:  br.s       IL_00e8

    IL_0088:  nop
    IL_0089:  ldloc.s    V_4
    IL_008b:  ldc.i4.1
    IL_008c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_0091:  nop
    IL_0092:  ldloc.0
    IL_0093:  ldloc.s    V_4
    IL_0095:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_009a:  nop
    IL_009b:  br.s       IL_00f4

    IL_009d:  nop
    IL_009e:  ldloc.s    V_4
    IL_00a0:  ldc.i4.2
    IL_00a1:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00a6:  nop
    IL_00a7:  ldloc.0
    IL_00a8:  ldloc.s    V_4
    IL_00aa:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00af:  nop
    IL_00b0:  br.s       IL_00f4

    IL_00b2:  nop
    IL_00b3:  ldloc.s    V_4
    IL_00b5:  ldc.i4.3
    IL_00b6:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00bb:  nop
    IL_00bc:  ldloc.0
    IL_00bd:  ldloc.s    V_4
    IL_00bf:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00c4:  nop
    IL_00c5:  br.s       IL_00f4

    IL_00c7:  nop
    IL_00c8:  ldloc.s    V_4
    IL_00ca:  ldc.i4.4
    IL_00cb:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00d0:  nop
    IL_00d1:  ldloc.0
    IL_00d2:  ldloc.s    V_4
    IL_00d4:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00d9:  nop
    IL_00da:  br.s       IL_00f4

    IL_00dc:  nop
    IL_00dd:  ldloc.0
    IL_00de:  ldloc.s    V_4
    IL_00e0:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00e5:  nop
    IL_00e6:  br.s       IL_00f4

    IL_00e8:  nop
    IL_00e9:  ldloc.1
    IL_00ea:  ldloc.s    V_4
    IL_00ec:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00f1:  nop
    IL_00f2:  br.s       IL_00f4

    IL_00f4:  nop
    IL_00f5:  ldloc.3
    IL_00f6:  ldc.i4.1
    IL_00f7:  add
    IL_00f8:  stloc.3
    IL_00f9:  ldloc.3
    IL_00fa:  ldloc.2
    IL_00fb:  ldlen
    IL_00fc:  conv.i4
    IL_00fd:  clt
    IL_00ff:  stloc.s    V_7
    IL_0101:  ldloc.s    V_7
    IL_0103:  brtrue     IL_001a

    IL_0108:  ret
  } // end of method Switch::SwitchOnStringInForLoop

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
