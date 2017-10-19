
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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module Switch.dll
// MVID: {3AC777A3-6335-478F-868C-4297490D6D49}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x01380000


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
      // Code size       14 (0xe)
      .maxstack  8
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

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State 
          SwitchOverNullableBool(valuetype [mscorlib]System.Nullable`1<bool> 'value') cil managed
  {
    // Code size       40 (0x28)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<bool> V_0,
             bool V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_0009:  brfalse.s  IL_0020

    IL_000b:  ldloca.s   V_0
    IL_000d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0012:  stloc.1
    IL_0013:  ldloc.1
    IL_0014:  brfalse.s  IL_001c

    IL_0016:  ldloc.1
    IL_0017:  ldc.i4.1
    IL_0018:  beq.s      IL_001e

    IL_001a:  br.s       IL_0022

    IL_001c:  ldc.i4.0
    IL_001d:  ret

    IL_001e:  ldc.i4.1
    IL_001f:  ret

    IL_0020:  ldc.i4.2
    IL_0021:  ret

    IL_0022:  newobj     instance void [mscorlib]System.InvalidOperationException::.ctor()
    IL_0027:  throw
  } // end of method Switch::SwitchOverNullableBool

  .method public hidebysig static valuetype [mscorlib]System.Nullable`1<bool> 
          SwitchOverNullableEnum(valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State> state) cil managed
  {
    // Code size       69 (0x45)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State> V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State V_1,
             valuetype [mscorlib]System.Nullable`1<bool> V_2)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State>::get_HasValue()
    IL_0009:  brfalse.s  IL_003f

    IL_000b:  ldloca.s   V_0
    IL_000d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State>::GetValueOrDefault()
    IL_0012:  stloc.1
    IL_0013:  ldloc.1
    IL_0014:  switch     ( 
                          IL_0027,
                          IL_002e,
                          IL_0035)
    IL_0025:  br.s       IL_003f

    IL_0027:  ldc.i4.0
    IL_0028:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_002d:  ret

    IL_002e:  ldc.i4.1
    IL_002f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0034:  ret

    IL_0035:  ldloca.s   V_2
    IL_0037:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_003d:  ldloc.2
    IL_003e:  ret

    IL_003f:  newobj     instance void [mscorlib]System.InvalidOperationException::.ctor()
    IL_0044:  throw
  } // end of method Switch::SwitchOverNullableEnum

  .method public hidebysig static string 
          SparseIntegerSwitch(int32 i) cil managed
  {
    // Code size       185 (0xb9)
    .maxstack  2
    IL_0000:  ldstr      "SparseIntegerSwitch: "
    IL_0005:  ldarg.0
    IL_0006:  box        [mscorlib]System.Int32
    IL_000b:  call       string [mscorlib]System.String::Concat(object,
                                                                object)
    IL_0010:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0015:  ldarg.0
    IL_0016:  ldc.i4.4
    IL_0017:  bgt.s      IL_0048

    IL_0019:  ldarg.0
    IL_001a:  ldc.i4     0xff676980
    IL_001f:  beq.s      IL_0071

    IL_0021:  ldarg.0
    IL_0022:  ldc.i4.s   -100
    IL_0024:  beq.s      IL_0077

    IL_0026:  ldarg.0
    IL_0027:  ldc.i4.m1
    IL_0028:  sub
    IL_0029:  switch     ( 
                          IL_007d,
                          IL_0083,
                          IL_0089,
                          IL_008f,
                          IL_00b3,
                          IL_0095)
    IL_0046:  br.s       IL_00b3

    IL_0048:  ldarg.0
    IL_0049:  ldc.i4     0x2710
    IL_004e:  bgt.s      IL_005f

    IL_0050:  ldarg.0
    IL_0051:  ldc.i4.s   100
    IL_0053:  beq.s      IL_009b

    IL_0055:  ldarg.0
    IL_0056:  ldc.i4     0x2710
    IL_005b:  beq.s      IL_00a1

    IL_005d:  br.s       IL_00b3

    IL_005f:  ldarg.0
    IL_0060:  ldc.i4     0x2711
    IL_0065:  beq.s      IL_00a7

    IL_0067:  ldarg.0
    IL_0068:  ldc.i4     0x7fffffff
    IL_006d:  beq.s      IL_00ad

    IL_006f:  br.s       IL_00b3

    IL_0071:  ldstr      "-10 mln"
    IL_0076:  ret

    IL_0077:  ldstr      "-hundred"
    IL_007c:  ret

    IL_007d:  ldstr      "-1"
    IL_0082:  ret

    IL_0083:  ldstr      "0"
    IL_0088:  ret

    IL_0089:  ldstr      "1"
    IL_008e:  ret

    IL_008f:  ldstr      "2"
    IL_0094:  ret

    IL_0095:  ldstr      "4"
    IL_009a:  ret

    IL_009b:  ldstr      "hundred"
    IL_00a0:  ret

    IL_00a1:  ldstr      "ten thousand"
    IL_00a6:  ret

    IL_00a7:  ldstr      "ten thousand and one"
    IL_00ac:  ret

    IL_00ad:  ldstr      "int.MaxValue"
    IL_00b2:  ret

    IL_00b3:  ldstr      "something else"
    IL_00b8:  ret
  } // end of method Switch::SparseIntegerSwitch

  .method public hidebysig static string 
          SwitchOverNullableInt(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       63 (0x3f)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brfalse.s  IL_0021

    IL_000b:  ldloca.s   V_0
    IL_000d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0012:  stloc.1
    IL_0013:  ldloc.1
    IL_0014:  brfalse.s  IL_0027

    IL_0016:  ldloc.1
    IL_0017:  ldc.i4.5
    IL_0018:  beq.s      IL_002d

    IL_001a:  ldloc.1
    IL_001b:  ldc.i4.s   10
    IL_001d:  beq.s      IL_0033

    IL_001f:  br.s       IL_0039

    IL_0021:  ldstr      "null"
    IL_0026:  ret

    IL_0027:  ldstr      "zero"
    IL_002c:  ret

    IL_002d:  ldstr      "five"
    IL_0032:  ret

    IL_0033:  ldstr      "ten"
    IL_0038:  ret

    IL_0039:  ldstr      "large"
    IL_003e:  ret
  } // end of method Switch::SwitchOverNullableInt

  .method public hidebysig static string 
          SwitchOverNullableIntNullCaseCombined(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       57 (0x39)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brfalse.s  IL_0021

    IL_000b:  ldloca.s   V_0
    IL_000d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0012:  stloc.1
    IL_0013:  ldloc.1
    IL_0014:  brfalse.s  IL_0021

    IL_0016:  ldloc.1
    IL_0017:  ldc.i4.5
    IL_0018:  beq.s      IL_0027

    IL_001a:  ldloc.1
    IL_001b:  ldc.i4.s   10
    IL_001d:  beq.s      IL_002d

    IL_001f:  br.s       IL_0033

    IL_0021:  ldstr      "zero"
    IL_0026:  ret

    IL_0027:  ldstr      "five"
    IL_002c:  ret

    IL_002d:  ldstr      "ten"
    IL_0032:  ret

    IL_0033:  ldstr      "large"
    IL_0038:  ret
  } // end of method Switch::SwitchOverNullableIntNullCaseCombined

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
    IL_0001:  stloc.1
    IL_0002:  ldloca.s   V_1
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brtrue.s   IL_0016

    IL_000b:  ldloca.s   V_2
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0013:  ldloc.2
    IL_0014:  br.s       IL_0024

    IL_0016:  ldloca.s   V_1
    IL_0018:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001d:  ldc.i4.5
    IL_001e:  add
    IL_001f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0024:  stloc.0
    IL_0025:  ldloca.s   V_0
    IL_0027:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_002c:  brfalse.s  IL_0044

    IL_002e:  ldloca.s   V_0
    IL_0030:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0035:  stloc.3
    IL_0036:  ldloc.3
    IL_0037:  brfalse.s  IL_004a

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
          SwitchOverNullableIntShiftedNullCaseCombined(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       92 (0x5c)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             int32 V_3)
    IL_0000:  ldarg.0
    IL_0001:  stloc.1
    IL_0002:  ldloca.s   V_1
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brtrue.s   IL_0016

    IL_000b:  ldloca.s   V_2
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0013:  ldloc.2
    IL_0014:  br.s       IL_0024

    IL_0016:  ldloca.s   V_1
    IL_0018:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001d:  ldc.i4.5
    IL_001e:  add
    IL_001f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0024:  stloc.0
    IL_0025:  ldloca.s   V_0
    IL_0027:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_002c:  brfalse.s  IL_0044

    IL_002e:  ldloca.s   V_0
    IL_0030:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0035:  stloc.3
    IL_0036:  ldloc.3
    IL_0037:  brfalse.s  IL_0044

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

    IL_0056:  ldstr      "large"
    IL_005b:  ret
  } // end of method Switch::SwitchOverNullableIntShiftedNullCaseCombined

  .method public hidebysig static string 
          SwitchOverNullableIntNoNullCase(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       57 (0x39)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brfalse.s  IL_0033

    IL_000b:  ldloca.s   V_0
    IL_000d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0012:  stloc.1
    IL_0013:  ldloc.1
    IL_0014:  brfalse.s  IL_0021

    IL_0016:  ldloc.1
    IL_0017:  ldc.i4.5
    IL_0018:  beq.s      IL_0027

    IL_001a:  ldloc.1
    IL_001b:  ldc.i4.s   10
    IL_001d:  beq.s      IL_002d

    IL_001f:  br.s       IL_0033

    IL_0021:  ldstr      "zero"
    IL_0026:  ret

    IL_0027:  ldstr      "five"
    IL_002c:  ret

    IL_002d:  ldstr      "ten"
    IL_0032:  ret

    IL_0033:  ldstr      "other"
    IL_0038:  ret
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
    IL_0001:  stloc.1
    IL_0002:  ldloca.s   V_1
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brtrue.s   IL_0016

    IL_000b:  ldloca.s   V_2
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0013:  ldloc.2
    IL_0014:  br.s       IL_0024

    IL_0016:  ldloca.s   V_1
    IL_0018:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001d:  ldc.i4.5
    IL_001e:  add
    IL_001f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0024:  stloc.0
    IL_0025:  ldloca.s   V_0
    IL_0027:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_002c:  brfalse.s  IL_0056

    IL_002e:  ldloca.s   V_0
    IL_0030:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0035:  stloc.3
    IL_0036:  ldloc.3
    IL_0037:  brfalse.s  IL_0044

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

  .method public hidebysig static void  SwitchOverInt(int32 i) cil managed
  {
    // Code size       122 (0x7a)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.s   10
    IL_0003:  bgt.s      IL_0012

    IL_0005:  ldarg.0
    IL_0006:  brfalse.s  IL_002d

    IL_0008:  ldarg.0
    IL_0009:  ldc.i4.5
    IL_000a:  beq.s      IL_0038

    IL_000c:  ldarg.0
    IL_000d:  ldc.i4.s   10
    IL_000f:  beq.s      IL_0043

    IL_0011:  ret

    IL_0012:  ldarg.0
    IL_0013:  ldc.i4.s   20
    IL_0015:  bgt.s      IL_0022

    IL_0017:  ldarg.0
    IL_0018:  ldc.i4.s   15
    IL_001a:  beq.s      IL_004e

    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.s   20
    IL_001f:  beq.s      IL_0059

    IL_0021:  ret

    IL_0022:  ldarg.0
    IL_0023:  ldc.i4.s   25
    IL_0025:  beq.s      IL_0064

    IL_0027:  ldarg.0
    IL_0028:  ldc.i4.s   30
    IL_002a:  beq.s      IL_006f

    IL_002c:  ret

    IL_002d:  ldstr      "zero"
    IL_0032:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0037:  ret

    IL_0038:  ldstr      "five"
    IL_003d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0042:  ret

    IL_0043:  ldstr      "ten"
    IL_0048:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004d:  ret

    IL_004e:  ldstr      "fifteen"
    IL_0053:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0058:  ret

    IL_0059:  ldstr      "twenty"
    IL_005e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0063:  ret

    IL_0064:  ldstr      "twenty-five"
    IL_0069:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006e:  ret

    IL_006f:  ldstr      "thirty"
    IL_0074:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0079:  ret
  } // end of method Switch::SwitchOverInt

  .method public hidebysig static string 
          ShortSwitchOverString(string text) cil managed
  {
    // Code size       81 (0x51)
    .maxstack  2
    IL_0000:  ldstr      "ShortSwitchOverString: "
    IL_0005:  ldarg.0
    IL_0006:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0010:  ldarg.0
    IL_0011:  ldstr      "First case"
    IL_0016:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_001b:  brtrue.s   IL_0039

    IL_001d:  ldarg.0
    IL_001e:  ldstr      "Second case"
    IL_0023:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0028:  brtrue.s   IL_003f

    IL_002a:  ldarg.0
    IL_002b:  ldstr      "Third case"
    IL_0030:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0035:  brtrue.s   IL_0045

    IL_0037:  br.s       IL_004b

    IL_0039:  ldstr      "Text1"
    IL_003e:  ret

    IL_003f:  ldstr      "Text2"
    IL_0044:  ret

    IL_0045:  ldstr      "Text3"
    IL_004a:  ret

    IL_004b:  ldstr      "Default"
    IL_0050:  ret
  } // end of method Switch::ShortSwitchOverString

  .method public hidebysig static string 
          ShortSwitchOverStringWithNullCase(string text) cil managed
  {
    // Code size       71 (0x47)
    .maxstack  2
    IL_0000:  ldstr      "ShortSwitchOverStringWithNullCase: "
    IL_0005:  ldarg.0
    IL_0006:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0010:  ldarg.0
    IL_0011:  ldstr      "First case"
    IL_0016:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_001b:  brtrue.s   IL_002f

    IL_001d:  ldarg.0
    IL_001e:  ldstr      "Second case"
    IL_0023:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0028:  brtrue.s   IL_0035

    IL_002a:  ldarg.0
    IL_002b:  brfalse.s  IL_003b

    IL_002d:  br.s       IL_0041

    IL_002f:  ldstr      "Text1"
    IL_0034:  ret

    IL_0035:  ldstr      "Text2"
    IL_003a:  ret

    IL_003b:  ldstr      "null"
    IL_0040:  ret

    IL_0041:  ldstr      "Default"
    IL_0046:  ret
  } // end of method Switch::ShortSwitchOverStringWithNullCase

  .method public hidebysig static string 
          SwitchOverString1(string text) cil managed
  {
    // Code size       289 (0x121)
    .maxstack  2
    .locals init (uint32 V_0)
    IL_0000:  ldstr      "SwitchOverString1: "
    IL_0005:  ldarg.0
    IL_0006:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0010:  ldarg.0
    IL_0011:  call       uint32 '<PrivateImplementationDetails>'::ComputeStringHash(string)
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4     0xf3d44a6
    IL_001d:  bgt.un.s   IL_0052

    IL_001f:  ldloc.0
    IL_0020:  ldc.i4     0x8861b86
    IL_0025:  bgt.un.s   IL_003d

    IL_0027:  ldloc.0
    IL_0028:  brfalse    IL_00f0

    IL_002d:  ldloc.0
    IL_002e:  ldc.i4     0x8861b86
    IL_0033:  beq        IL_00d2

    IL_0038:  br         IL_011b

    IL_003d:  ldloc.0
    IL_003e:  ldc.i4     0xc9a8f4f
    IL_0043:  beq.s      IL_0084

    IL_0045:  ldloc.0
    IL_0046:  ldc.i4     0xf3d44a6
    IL_004b:  beq.s      IL_00b4

    IL_004d:  br         IL_011b

    IL_0052:  ldloc.0
    IL_0053:  ldc.i4     0x652a1179
    IL_0058:  bgt.un.s   IL_006f

    IL_005a:  ldloc.0
    IL_005b:  ldc.i4     0x51650fb9
    IL_0060:  beq.s      IL_00e1

    IL_0062:  ldloc.0
    IL_0063:  ldc.i4     0x652a1179
    IL_0068:  beq.s      IL_00a5

    IL_006a:  br         IL_011b

    IL_006f:  ldloc.0
    IL_0070:  ldc.i4     0xea3d096b
    IL_0075:  beq.s      IL_0096

    IL_0077:  ldloc.0
    IL_0078:  ldc.i4     0xf701cc7f
    IL_007d:  beq.s      IL_00c3

    IL_007f:  br         IL_011b

    IL_0084:  ldarg.0
    IL_0085:  ldstr      "First case"
    IL_008a:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_008f:  brtrue.s   IL_00f5

    IL_0091:  br         IL_011b

    IL_0096:  ldarg.0
    IL_0097:  ldstr      "Second case"
    IL_009c:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00a1:  brtrue.s   IL_00fb

    IL_00a3:  br.s       IL_011b

    IL_00a5:  ldarg.0
    IL_00a6:  ldstr      "2nd case"
    IL_00ab:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00b0:  brtrue.s   IL_00fb

    IL_00b2:  br.s       IL_011b

    IL_00b4:  ldarg.0
    IL_00b5:  ldstr      "Third case"
    IL_00ba:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00bf:  brtrue.s   IL_0101

    IL_00c1:  br.s       IL_011b

    IL_00c3:  ldarg.0
    IL_00c4:  ldstr      "Fourth case"
    IL_00c9:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00ce:  brtrue.s   IL_0107

    IL_00d0:  br.s       IL_011b

    IL_00d2:  ldarg.0
    IL_00d3:  ldstr      "Fifth case"
    IL_00d8:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00dd:  brtrue.s   IL_010d

    IL_00df:  br.s       IL_011b

    IL_00e1:  ldarg.0
    IL_00e2:  ldstr      "Sixth case"
    IL_00e7:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00ec:  brtrue.s   IL_0113

    IL_00ee:  br.s       IL_011b

    IL_00f0:  ldarg.0
    IL_00f1:  brfalse.s  IL_0119

    IL_00f3:  br.s       IL_011b

    IL_00f5:  ldstr      "Text1"
    IL_00fa:  ret

    IL_00fb:  ldstr      "Text2"
    IL_0100:  ret

    IL_0101:  ldstr      "Text3"
    IL_0106:  ret

    IL_0107:  ldstr      "Text4"
    IL_010c:  ret

    IL_010d:  ldstr      "Text5"
    IL_0112:  ret

    IL_0113:  ldstr      "Text6"
    IL_0118:  ret

    IL_0119:  ldnull
    IL_011a:  ret

    IL_011b:  ldstr      "Default"
    IL_0120:  ret
  } // end of method Switch::SwitchOverString1

  .method public hidebysig static string 
          SwitchOverString2() cil managed
  {
    // Code size       446 (0x1be)
    .maxstack  2
    .locals init (string V_0,
             uint32 V_1)
    IL_0000:  ldstr      "SwitchOverString2:"
    IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000a:  call       string [mscorlib]System.Environment::get_UserName()
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  call       uint32 '<PrivateImplementationDetails>'::ComputeStringHash(string)
    IL_0016:  stloc.1
    IL_0017:  ldloc.1
    IL_0018:  ldc.i4     0x4c7c71f6
    IL_001d:  bgt.un.s   IL_0065

    IL_001f:  ldloc.1
    IL_0020:  ldc.i4     0xc9a8f4f
    IL_0025:  bgt.un.s   IL_003f

    IL_0027:  ldloc.1
    IL_0028:  ldc.i4     0x8861b86
    IL_002d:  beq        IL_0107

    IL_0032:  ldloc.1
    IL_0033:  ldc.i4     0xc9a8f4f
    IL_0038:  beq.s      IL_00b3

    IL_003a:  br         IL_01b8

    IL_003f:  ldloc.1
    IL_0040:  ldc.i4     0xf3d44a6
    IL_0045:  beq        IL_00dd

    IL_004a:  ldloc.1
    IL_004b:  ldc.i4     0x20289804
    IL_0050:  beq        IL_013a

    IL_0055:  ldloc.1
    IL_0056:  ldc.i4     0x4c7c71f6
    IL_005b:  beq        IL_0149

    IL_0060:  br         IL_01b8

    IL_0065:  ldloc.1
    IL_0066:  ldc.i4     0xa151b28a
    IL_006b:  bgt.un.s   IL_0093

    IL_006d:  ldloc.1
    IL_006e:  ldc.i4     0x4d0cea48
    IL_0073:  beq        IL_0167

    IL_0078:  ldloc.1
    IL_0079:  ldc.i4     0x51650fb9
    IL_007e:  beq        IL_0119

    IL_0083:  ldloc.1
    IL_0084:  ldc.i4     0xa151b28a
    IL_0089:  beq        IL_012b

    IL_008e:  br         IL_01b8

    IL_0093:  ldloc.1
    IL_0094:  ldc.i4     0xea3d096b
    IL_0099:  beq.s      IL_00c8

    IL_009b:  ldloc.1
    IL_009c:  ldc.i4     0xed5134d4
    IL_00a1:  beq        IL_0158

    IL_00a6:  ldloc.1
    IL_00a7:  ldc.i4     0xf701cc7f
    IL_00ac:  beq.s      IL_00f2

    IL_00ae:  br         IL_01b8

    IL_00b3:  ldloc.0
    IL_00b4:  ldstr      "First case"
    IL_00b9:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00be:  brtrue     IL_0176

    IL_00c3:  br         IL_01b8

    IL_00c8:  ldloc.0
    IL_00c9:  ldstr      "Second case"
    IL_00ce:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00d3:  brtrue     IL_017c

    IL_00d8:  br         IL_01b8

    IL_00dd:  ldloc.0
    IL_00de:  ldstr      "Third case"
    IL_00e3:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00e8:  brtrue     IL_0182

    IL_00ed:  br         IL_01b8

    IL_00f2:  ldloc.0
    IL_00f3:  ldstr      "Fourth case"
    IL_00f8:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00fd:  brtrue     IL_0188

    IL_0102:  br         IL_01b8

    IL_0107:  ldloc.0
    IL_0108:  ldstr      "Fifth case"
    IL_010d:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0112:  brtrue.s   IL_018e

    IL_0114:  br         IL_01b8

    IL_0119:  ldloc.0
    IL_011a:  ldstr      "Sixth case"
    IL_011f:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0124:  brtrue.s   IL_0194

    IL_0126:  br         IL_01b8

    IL_012b:  ldloc.0
    IL_012c:  ldstr      "Seventh case"
    IL_0131:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0136:  brtrue.s   IL_019a

    IL_0138:  br.s       IL_01b8

    IL_013a:  ldloc.0
    IL_013b:  ldstr      "Eighth case"
    IL_0140:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0145:  brtrue.s   IL_01a0

    IL_0147:  br.s       IL_01b8

    IL_0149:  ldloc.0
    IL_014a:  ldstr      "Ninth case"
    IL_014f:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0154:  brtrue.s   IL_01a6

    IL_0156:  br.s       IL_01b8

    IL_0158:  ldloc.0
    IL_0159:  ldstr      "Tenth case"
    IL_015e:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0163:  brtrue.s   IL_01ac

    IL_0165:  br.s       IL_01b8

    IL_0167:  ldloc.0
    IL_0168:  ldstr      "Eleventh case"
    IL_016d:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0172:  brtrue.s   IL_01b2

    IL_0174:  br.s       IL_01b8

    IL_0176:  ldstr      "Text1"
    IL_017b:  ret

    IL_017c:  ldstr      "Text2"
    IL_0181:  ret

    IL_0182:  ldstr      "Text3"
    IL_0187:  ret

    IL_0188:  ldstr      "Text4"
    IL_018d:  ret

    IL_018e:  ldstr      "Text5"
    IL_0193:  ret

    IL_0194:  ldstr      "Text6"
    IL_0199:  ret

    IL_019a:  ldstr      "Text7"
    IL_019f:  ret

    IL_01a0:  ldstr      "Text8"
    IL_01a5:  ret

    IL_01a6:  ldstr      "Text9"
    IL_01ab:  ret

    IL_01ac:  ldstr      "Text10"
    IL_01b1:  ret

    IL_01b2:  ldstr      "Text11"
    IL_01b7:  ret

    IL_01b8:  ldstr      "Default"
    IL_01bd:  ret
  } // end of method Switch::SwitchOverString2

  .method public hidebysig static string 
          SwitchOverBool(bool b) cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  8
    IL_0000:  ldstr      "SwitchOverBool: "
    IL_0005:  ldarga.s   b
    IL_0007:  call       instance string [mscorlib]System.Boolean::ToString()
    IL_000c:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_0011:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0016:  ldarg.0
    IL_0017:  brfalse.s  IL_0023

    IL_0019:  ldarg.0
    IL_001a:  ldc.i4.1
    IL_001b:  bne.un.s   IL_0029

    IL_001d:  ldsfld     string [mscorlib]System.Boolean::TrueString
    IL_0022:  ret

    IL_0023:  ldsfld     string [mscorlib]System.Boolean::FalseString
    IL_0028:  ret

    IL_0029:  ldnull
    IL_002a:  ret
  } // end of method Switch::SwitchOverBool

  .method public hidebysig static void  SwitchInLoop(int32 i) cil managed
  {
    // Code size       110 (0x6e)
    .maxstack  2
    IL_0000:  ldstr      "SwitchInLoop: "
    IL_0005:  ldarg.0
    IL_0006:  box        [mscorlib]System.Int32
    IL_000b:  call       string [mscorlib]System.String::Concat(object,
                                                                object)
    IL_0010:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0015:  ldarg.0
    IL_0016:  ldc.i4.1
    IL_0017:  sub
    IL_0018:  switch     ( 
                          IL_002f,
                          IL_003b,
                          IL_0052,
                          IL_0047)
    IL_002d:  br.s       IL_0052

    IL_002f:  ldstr      "one"
    IL_0034:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0039:  br.s       IL_0067

    IL_003b:  ldstr      "two"
    IL_0040:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0045:  br.s       IL_0067

    IL_0047:  ldstr      "four"
    IL_004c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0051:  ret

    IL_0052:  ldstr      "default"
    IL_0057:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005c:  ldstr      "more code"
    IL_0061:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0066:  ret

    IL_0067:  ldarg.0
    IL_0068:  ldc.i4.1
    IL_0069:  add
    IL_006a:  starg.s    i
    IL_006c:  br.s       IL_0015
  } // end of method Switch::SwitchInLoop

  .method public hidebysig static void  SwitchWithGoto(int32 i) cil managed
  {
    // Code size       113 (0x71)
    .maxstack  2
    IL_0000:  ldstr      "SwitchWithGoto: "
    IL_0005:  ldarg.0
    IL_0006:  box        [mscorlib]System.Int32
    IL_000b:  call       string [mscorlib]System.String::Concat(object,
                                                                object)
    IL_0010:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0015:  ldarg.0
    IL_0016:  ldc.i4.1
    IL_0017:  sub
    IL_0018:  switch     ( 
                          IL_002f,
                          IL_003b,
                          IL_0045,
                          IL_0051)
    IL_002d:  br.s       IL_005c

    IL_002f:  ldstr      "one"
    IL_0034:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0039:  br.s       IL_005c

    IL_003b:  ldstr      "two"
    IL_0040:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0045:  ldstr      "three"
    IL_004a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004f:  br.s       IL_0066

    IL_0051:  ldstr      "four"
    IL_0056:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005b:  ret

    IL_005c:  ldstr      "default"
    IL_0061:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0066:  ldstr      "End of method"
    IL_006b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0070:  ret
  } // end of method Switch::SwitchWithGoto

  .method private hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty[] 
          GetProperties() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty
    IL_0006:  ret
  } // end of method Switch::GetProperties

  .method public hidebysig static void  SwitchOnStringInForLoop() cil managed
  {
    // Code size       244 (0xf4)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty> V_0,
             class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty> V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty[] V_2,
             int32 V_3,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty V_4,
             string V_5)
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::.ctor()
    IL_000b:  stloc.1
    IL_000c:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch::GetProperties()
    IL_0011:  stloc.2
    IL_0012:  ldc.i4.0
    IL_0013:  stloc.3
    IL_0014:  br         IL_00ea

    IL_0019:  ldstr      "In for-loop"
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0023:  ldloc.2
    IL_0024:  ldloc.3
    IL_0025:  ldelem.ref
    IL_0026:  stloc.s    V_4
    IL_0028:  ldloc.s    V_4
    IL_002a:  ldfld      class [mscorlib]System.Reflection.PropertyInfo ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::Property
    IL_002f:  callvirt   instance string [mscorlib]System.Reflection.MemberInfo::get_Name()
    IL_0034:  stloc.s    V_5
    IL_0036:  ldloc.s    V_5
    IL_0038:  ldstr      "Name1"
    IL_003d:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0042:  brtrue.s   IL_008c

    IL_0044:  ldloc.s    V_5
    IL_0046:  ldstr      "Name2"
    IL_004b:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0050:  brtrue.s   IL_009e

    IL_0052:  ldloc.s    V_5
    IL_0054:  ldstr      "Name3"
    IL_0059:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_005e:  brtrue.s   IL_00b0

    IL_0060:  ldloc.s    V_5
    IL_0062:  ldstr      "Name4"
    IL_0067:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_006c:  brtrue.s   IL_00c2

    IL_006e:  ldloc.s    V_5
    IL_0070:  ldstr      "Name5"
    IL_0075:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_007a:  brtrue.s   IL_00d4

    IL_007c:  ldloc.s    V_5
    IL_007e:  ldstr      "Name6"
    IL_0083:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0088:  brtrue.s   IL_00d4

    IL_008a:  br.s       IL_00de

    IL_008c:  ldloc.s    V_4
    IL_008e:  ldc.i4.1
    IL_008f:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_0094:  ldloc.0
    IL_0095:  ldloc.s    V_4
    IL_0097:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_009c:  br.s       IL_00e6

    IL_009e:  ldloc.s    V_4
    IL_00a0:  ldc.i4.2
    IL_00a1:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00a6:  ldloc.0
    IL_00a7:  ldloc.s    V_4
    IL_00a9:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00ae:  br.s       IL_00e6

    IL_00b0:  ldloc.s    V_4
    IL_00b2:  ldc.i4.3
    IL_00b3:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00b8:  ldloc.0
    IL_00b9:  ldloc.s    V_4
    IL_00bb:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00c0:  br.s       IL_00e6

    IL_00c2:  ldloc.s    V_4
    IL_00c4:  ldc.i4.4
    IL_00c5:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00ca:  ldloc.0
    IL_00cb:  ldloc.s    V_4
    IL_00cd:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00d2:  br.s       IL_00e6

    IL_00d4:  ldloc.0
    IL_00d5:  ldloc.s    V_4
    IL_00d7:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00dc:  br.s       IL_00e6

    IL_00de:  ldloc.1
    IL_00df:  ldloc.s    V_4
    IL_00e1:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00e6:  ldloc.3
    IL_00e7:  ldc.i4.1
    IL_00e8:  add
    IL_00e9:  stloc.3
    IL_00ea:  ldloc.3
    IL_00eb:  ldloc.2
    IL_00ec:  ldlen
    IL_00ed:  conv.i4
    IL_00ee:  blt        IL_0019

    IL_00f3:  ret
  } // end of method Switch::SwitchOnStringInForLoop

  .method public hidebysig static void  SwitchWithComplexCondition(string[] args) cil managed
  {
    // Code size       126 (0x7e)
    .maxstack  2
    .locals init (string V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldlen
    IL_0002:  brfalse.s  IL_0009

    IL_0004:  ldarg.0
    IL_0005:  ldc.i4.0
    IL_0006:  ldelem.ref
    IL_0007:  br.s       IL_000e

    IL_0009:  ldstr      "dummy"
    IL_000e:  stloc.0
    IL_000f:  ldloc.0
    IL_0010:  ldstr      "a"
    IL_0015:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_001a:  brtrue.s   IL_0045

    IL_001c:  ldloc.0
    IL_001d:  ldstr      "b"
    IL_0022:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0027:  brtrue.s   IL_0051

    IL_0029:  ldloc.0
    IL_002a:  ldstr      "c"
    IL_002f:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0034:  brtrue.s   IL_005d

    IL_0036:  ldloc.0
    IL_0037:  ldstr      "d"
    IL_003c:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0041:  brtrue.s   IL_0069

    IL_0043:  br.s       IL_0073

    IL_0045:  ldstr      "a"
    IL_004a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004f:  br.s       IL_0073

    IL_0051:  ldstr      "b"
    IL_0056:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005b:  br.s       IL_0073

    IL_005d:  ldstr      "c"
    IL_0062:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0067:  br.s       IL_0073

    IL_0069:  ldstr      "d"
    IL_006e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0073:  ldstr      "end"
    IL_0078:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007d:  ret
  } // end of method Switch::SwitchWithComplexCondition

  .method public hidebysig static void  SwitchWithArray(string[] args) cil managed
  {
    // Code size       115 (0x73)
    .maxstack  2
    .locals init (string V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  ldelem.ref
    IL_0003:  stloc.0
    IL_0004:  ldloc.0
    IL_0005:  ldstr      "a"
    IL_000a:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_000f:  brtrue.s   IL_003a

    IL_0011:  ldloc.0
    IL_0012:  ldstr      "b"
    IL_0017:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_001c:  brtrue.s   IL_0046

    IL_001e:  ldloc.0
    IL_001f:  ldstr      "c"
    IL_0024:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0029:  brtrue.s   IL_0052

    IL_002b:  ldloc.0
    IL_002c:  ldstr      "d"
    IL_0031:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0036:  brtrue.s   IL_005e

    IL_0038:  br.s       IL_0068

    IL_003a:  ldstr      "a"
    IL_003f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0044:  br.s       IL_0068

    IL_0046:  ldstr      "b"
    IL_004b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0050:  br.s       IL_0068

    IL_0052:  ldstr      "c"
    IL_0057:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005c:  br.s       IL_0068

    IL_005e:  ldstr      "d"
    IL_0063:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0068:  ldstr      "end"
    IL_006d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0072:  ret
  } // end of method Switch::SwitchWithArray

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch

.class private auto ansi sealed '<PrivateImplementationDetails>'
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method assembly hidebysig static uint32 
          ComputeStringHash(string s) cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  2
    .locals init (uint32 V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_002a

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
    IL_0028:  blt.s      IL_000d

    IL_002a:  ldloc.0
    IL_002b:  ret
  } // end of method '<PrivateImplementationDetails>'::ComputeStringHash

} // end of class '<PrivateImplementationDetails>'


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
