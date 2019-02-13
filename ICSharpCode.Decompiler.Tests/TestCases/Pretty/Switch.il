



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern System.Core
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly Switch
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module Switch.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


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
    // Code size       53 (0x35)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarga.s   'value'
    IL_0003:  dup
    IL_0004:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0009:  stloc.1
    IL_000a:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_000f:  brfalse.s  IL_0029

    IL_0011:  ldloc.1
    IL_0012:  switch     ( 
                          IL_0021,
                          IL_0025)
    IL_001f:  br.s       IL_002d

    IL_0021:  ldc.i4.0
    IL_0022:  stloc.0
    IL_0023:  br.s       IL_0033

    IL_0025:  ldc.i4.1
    IL_0026:  stloc.0
    IL_0027:  br.s       IL_0033

    IL_0029:  ldc.i4.2
    IL_002a:  stloc.0
    IL_002b:  br.s       IL_0033

    IL_002d:  newobj     instance void [mscorlib]System.InvalidOperationException::.ctor()
    IL_0032:  throw

    IL_0033:  ldloc.0
    IL_0034:  ret
  } // end of method Switch::SwitchOverNullableBool

  .method public hidebysig static valuetype [mscorlib]System.Nullable`1<bool> 
          SwitchOverNullableEnum(valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State> state) cil managed
  {
    // Code size       75 (0x4b)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<bool> V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State V_1,
             valuetype [mscorlib]System.Nullable`1<bool> V_2)
    IL_0000:  nop
    IL_0001:  ldarga.s   state
    IL_0003:  dup
    IL_0004:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State>::GetValueOrDefault()
    IL_0009:  stloc.1
    IL_000a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State>::get_HasValue()
    IL_000f:  brfalse.s  IL_0043

    IL_0011:  ldloc.1
    IL_0012:  switch     ( 
                          IL_0025,
                          IL_002e,
                          IL_0037)
    IL_0023:  br.s       IL_0043

    IL_0025:  ldc.i4.0
    IL_0026:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_002b:  stloc.0
    IL_002c:  br.s       IL_0049

    IL_002e:  ldc.i4.1
    IL_002f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0034:  stloc.0
    IL_0035:  br.s       IL_0049

    IL_0037:  ldloca.s   V_2
    IL_0039:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_003f:  ldloc.2
    IL_0040:  stloc.0
    IL_0041:  br.s       IL_0049

    IL_0043:  newobj     instance void [mscorlib]System.InvalidOperationException::.ctor()
    IL_0048:  throw

    IL_0049:  ldloc.0
    IL_004a:  ret
  } // end of method Switch::SwitchOverNullableEnum

  .method public hidebysig static string 
          SparseIntegerSwitch(int32 i) cil managed
  {
    // Code size       209 (0xd1)
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
    IL_001b:  bgt.s      IL_004c

    IL_001d:  ldloc.1
    IL_001e:  ldc.i4     0xff676980
    IL_0023:  beq.s      IL_006f

    IL_0025:  ldloc.1
    IL_0026:  ldc.i4.s   -100
    IL_0028:  beq.s      IL_0077

    IL_002a:  ldloc.1
    IL_002b:  ldc.i4.m1
    IL_002c:  sub
    IL_002d:  switch     ( 
                          IL_007f,
                          IL_0087,
                          IL_008f,
                          IL_0097,
                          IL_00c7,
                          IL_009f)
    IL_004a:  br.s       IL_00c7

    IL_004c:  ldloc.1
    IL_004d:  ldc.i4.s   100
    IL_004f:  beq.s      IL_00a7

    IL_0051:  ldloc.1
    IL_0052:  ldc.i4     0x2710
    IL_0057:  sub
    IL_0058:  switch     ( 
                          IL_00af,
                          IL_00b7)
    IL_0065:  ldloc.1
    IL_0066:  ldc.i4     0x7fffffff
    IL_006b:  beq.s      IL_00bf

    IL_006d:  br.s       IL_00c7

    IL_006f:  ldstr      "-10 mln"
    IL_0074:  stloc.0
    IL_0075:  br.s       IL_00cf

    IL_0077:  ldstr      "-hundred"
    IL_007c:  stloc.0
    IL_007d:  br.s       IL_00cf

    IL_007f:  ldstr      "-1"
    IL_0084:  stloc.0
    IL_0085:  br.s       IL_00cf

    IL_0087:  ldstr      "0"
    IL_008c:  stloc.0
    IL_008d:  br.s       IL_00cf

    IL_008f:  ldstr      "1"
    IL_0094:  stloc.0
    IL_0095:  br.s       IL_00cf

    IL_0097:  ldstr      "2"
    IL_009c:  stloc.0
    IL_009d:  br.s       IL_00cf

    IL_009f:  ldstr      "4"
    IL_00a4:  stloc.0
    IL_00a5:  br.s       IL_00cf

    IL_00a7:  ldstr      "hundred"
    IL_00ac:  stloc.0
    IL_00ad:  br.s       IL_00cf

    IL_00af:  ldstr      "ten thousand"
    IL_00b4:  stloc.0
    IL_00b5:  br.s       IL_00cf

    IL_00b7:  ldstr      "ten thousand and one"
    IL_00bc:  stloc.0
    IL_00bd:  br.s       IL_00cf

    IL_00bf:  ldstr      "int.MaxValue"
    IL_00c4:  stloc.0
    IL_00c5:  br.s       IL_00cf

    IL_00c7:  ldstr      "something else"
    IL_00cc:  stloc.0
    IL_00cd:  br.s       IL_00cf

    IL_00cf:  ldloc.0
    IL_00d0:  ret
  } // end of method Switch::SparseIntegerSwitch

  .method public hidebysig static void  SparseIntegerSwitch2(int32 i) cil managed
  {
    // Code size       94 (0x5e)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  ldc.i4.s   21
    IL_0006:  bgt.s      IL_002c

    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.4
    IL_000a:  beq.s      IL_0055

    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.s   10
    IL_000f:  sub
    IL_0010:  switch     ( 
                          IL_0055,
                          IL_0055,
                          IL_005d,
                          IL_0055)
    IL_0025:  ldloc.0
    IL_0026:  ldc.i4.s   21
    IL_0028:  beq.s      IL_0055

    IL_002a:  br.s       IL_005d

    IL_002c:  ldloc.0
    IL_002d:  ldc.i4.s   33
    IL_002f:  bgt.s      IL_003d

    IL_0031:  ldloc.0
    IL_0032:  ldc.i4.s   29
    IL_0034:  beq.s      IL_0055

    IL_0036:  ldloc.0
    IL_0037:  ldc.i4.s   33
    IL_0039:  beq.s      IL_0055

    IL_003b:  br.s       IL_005d

    IL_003d:  ldloc.0
    IL_003e:  ldc.i4.s   49
    IL_0040:  sub
    IL_0041:  switch     ( 
                          IL_0055,
                          IL_0055)
    IL_004e:  ldloc.0
    IL_004f:  ldc.i4.s   55
    IL_0051:  beq.s      IL_0055

    IL_0053:  br.s       IL_005d

    IL_0055:  call       void [mscorlib]System.Console::WriteLine()
    IL_005a:  nop
    IL_005b:  br.s       IL_005d

    IL_005d:  ret
  } // end of method Switch::SparseIntegerSwitch2

  .method public hidebysig static bool  SparseIntegerSwitch3(int32 i) cil managed
  {
    // Code size       72 (0x48)
    .maxstack  2
    .locals init (bool V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldloc.1
    IL_0004:  ldc.i4.s   12
    IL_0006:  bgt.s      IL_0023

    IL_0008:  ldloc.1
    IL_0009:  ldc.i4.0
    IL_000a:  beq.s      IL_003e

    IL_000c:  ldloc.1
    IL_000d:  ldc.i4.s   10
    IL_000f:  sub
    IL_0010:  switch     ( 
                          IL_003e,
                          IL_003e,
                          IL_003e)
    IL_0021:  br.s       IL_0042

    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.s   100
    IL_0026:  sub
    IL_0027:  switch     ( 
                          IL_003e,
                          IL_003e)
    IL_0034:  ldloc.1
    IL_0035:  ldc.i4     0xc8
    IL_003a:  beq.s      IL_003e

    IL_003c:  br.s       IL_0042

    IL_003e:  ldc.i4.1
    IL_003f:  stloc.0
    IL_0040:  br.s       IL_0046

    IL_0042:  ldc.i4.0
    IL_0043:  stloc.0
    IL_0044:  br.s       IL_0046

    IL_0046:  ldloc.0
    IL_0047:  ret
  } // end of method Switch::SparseIntegerSwitch3

  .method public hidebysig static string 
          SwitchOverNullableInt(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       74 (0x4a)
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
    IL_0013:  beq.s      IL_0028

    IL_0015:  ldloc.1
    IL_0016:  ldc.i4.5
    IL_0017:  beq.s      IL_0030

    IL_0019:  ldloc.1
    IL_001a:  ldc.i4.s   10
    IL_001c:  beq.s      IL_0038

    IL_001e:  br.s       IL_0040

    IL_0020:  ldstr      "null"
    IL_0025:  stloc.0
    IL_0026:  br.s       IL_0048

    IL_0028:  ldstr      "zero"
    IL_002d:  stloc.0
    IL_002e:  br.s       IL_0048

    IL_0030:  ldstr      "five"
    IL_0035:  stloc.0
    IL_0036:  br.s       IL_0048

    IL_0038:  ldstr      "ten"
    IL_003d:  stloc.0
    IL_003e:  br.s       IL_0048

    IL_0040:  ldstr      "large"
    IL_0045:  stloc.0
    IL_0046:  br.s       IL_0048

    IL_0048:  ldloc.0
    IL_0049:  ret
  } // end of method Switch::SwitchOverNullableInt

  .method public hidebysig static string 
          SwitchOverNullableIntNullCaseCombined(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       66 (0x42)
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
    IL_0013:  beq.s      IL_0020

    IL_0015:  ldloc.1
    IL_0016:  ldc.i4.5
    IL_0017:  beq.s      IL_0028

    IL_0019:  ldloc.1
    IL_001a:  ldc.i4.s   10
    IL_001c:  beq.s      IL_0030

    IL_001e:  br.s       IL_0038

    IL_0020:  ldstr      "zero"
    IL_0025:  stloc.0
    IL_0026:  br.s       IL_0040

    IL_0028:  ldstr      "five"
    IL_002d:  stloc.0
    IL_002e:  br.s       IL_0040

    IL_0030:  ldstr      "ten"
    IL_0035:  stloc.0
    IL_0036:  br.s       IL_0040

    IL_0038:  ldstr      "large"
    IL_003d:  stloc.0
    IL_003e:  br.s       IL_0040

    IL_0040:  ldloc.0
    IL_0041:  ret
  } // end of method Switch::SwitchOverNullableIntNullCaseCombined

  .method public hidebysig static string 
          SwitchOverNullableIntShifted(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       112 (0x70)
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
    IL_0039:  beq.s      IL_004e

    IL_003b:  ldloc.3
    IL_003c:  ldc.i4.5
    IL_003d:  beq.s      IL_0056

    IL_003f:  ldloc.3
    IL_0040:  ldc.i4.s   10
    IL_0042:  beq.s      IL_005e

    IL_0044:  br.s       IL_0066

    IL_0046:  ldstr      "null"
    IL_004b:  stloc.0
    IL_004c:  br.s       IL_006e

    IL_004e:  ldstr      "zero"
    IL_0053:  stloc.0
    IL_0054:  br.s       IL_006e

    IL_0056:  ldstr      "five"
    IL_005b:  stloc.0
    IL_005c:  br.s       IL_006e

    IL_005e:  ldstr      "ten"
    IL_0063:  stloc.0
    IL_0064:  br.s       IL_006e

    IL_0066:  ldstr      "large"
    IL_006b:  stloc.0
    IL_006c:  br.s       IL_006e

    IL_006e:  ldloc.0
    IL_006f:  ret
  } // end of method Switch::SwitchOverNullableIntShifted

  .method public hidebysig static string 
          SwitchOverNullableIntShiftedNullCaseCombined(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       104 (0x68)
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
    IL_0039:  beq.s      IL_0046

    IL_003b:  ldloc.3
    IL_003c:  ldc.i4.5
    IL_003d:  beq.s      IL_004e

    IL_003f:  ldloc.3
    IL_0040:  ldc.i4.s   10
    IL_0042:  beq.s      IL_0056

    IL_0044:  br.s       IL_005e

    IL_0046:  ldstr      "zero"
    IL_004b:  stloc.0
    IL_004c:  br.s       IL_0066

    IL_004e:  ldstr      "five"
    IL_0053:  stloc.0
    IL_0054:  br.s       IL_0066

    IL_0056:  ldstr      "ten"
    IL_005b:  stloc.0
    IL_005c:  br.s       IL_0066

    IL_005e:  ldstr      "large"
    IL_0063:  stloc.0
    IL_0064:  br.s       IL_0066

    IL_0066:  ldloc.0
    IL_0067:  ret
  } // end of method Switch::SwitchOverNullableIntShiftedNullCaseCombined

  .method public hidebysig static string 
          SwitchOverNullableIntNoNullCase(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       66 (0x42)
    .maxstack  2
    .locals init (string V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarga.s   i
    IL_0003:  dup
    IL_0004:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0009:  stloc.1
    IL_000a:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000f:  brfalse.s  IL_0038

    IL_0011:  ldloc.1
    IL_0012:  ldc.i4.0
    IL_0013:  beq.s      IL_0020

    IL_0015:  ldloc.1
    IL_0016:  ldc.i4.5
    IL_0017:  beq.s      IL_0028

    IL_0019:  ldloc.1
    IL_001a:  ldc.i4.s   10
    IL_001c:  beq.s      IL_0030

    IL_001e:  br.s       IL_0038

    IL_0020:  ldstr      "zero"
    IL_0025:  stloc.0
    IL_0026:  br.s       IL_0040

    IL_0028:  ldstr      "five"
    IL_002d:  stloc.0
    IL_002e:  br.s       IL_0040

    IL_0030:  ldstr      "ten"
    IL_0035:  stloc.0
    IL_0036:  br.s       IL_0040

    IL_0038:  ldstr      "other"
    IL_003d:  stloc.0
    IL_003e:  br.s       IL_0040

    IL_0040:  ldloc.0
    IL_0041:  ret
  } // end of method Switch::SwitchOverNullableIntNoNullCase

  .method public hidebysig static string 
          SwitchOverNullableIntNoNullCaseShifted(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       104 (0x68)
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
    IL_0035:  brfalse.s  IL_005e

    IL_0037:  ldloc.3
    IL_0038:  ldc.i4.0
    IL_0039:  beq.s      IL_0046

    IL_003b:  ldloc.3
    IL_003c:  ldc.i4.5
    IL_003d:  beq.s      IL_004e

    IL_003f:  ldloc.3
    IL_0040:  ldc.i4.s   10
    IL_0042:  beq.s      IL_0056

    IL_0044:  br.s       IL_005e

    IL_0046:  ldstr      "zero"
    IL_004b:  stloc.0
    IL_004c:  br.s       IL_0066

    IL_004e:  ldstr      "five"
    IL_0053:  stloc.0
    IL_0054:  br.s       IL_0066

    IL_0056:  ldstr      "ten"
    IL_005b:  stloc.0
    IL_005c:  br.s       IL_0066

    IL_005e:  ldstr      "other"
    IL_0063:  stloc.0
    IL_0064:  br.s       IL_0066

    IL_0066:  ldloc.0
    IL_0067:  ret
  } // end of method Switch::SwitchOverNullableIntNoNullCaseShifted

  .method public hidebysig static void  SwitchOverInt(int32 i) cil managed
  {
    // Code size       144 (0x90)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  ldc.i4.s   10
    IL_0006:  bgt.s      IL_0017

    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  beq.s      IL_0034

    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.5
    IL_000e:  beq.s      IL_0041

    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.s   10
    IL_0013:  beq.s      IL_004e

    IL_0015:  br.s       IL_008f

    IL_0017:  ldloc.0
    IL_0018:  ldc.i4.s   20
    IL_001a:  bgt.s      IL_0028

    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.s   15
    IL_001f:  beq.s      IL_005b

    IL_0021:  ldloc.0
    IL_0022:  ldc.i4.s   20
    IL_0024:  beq.s      IL_0068

    IL_0026:  br.s       IL_008f

    IL_0028:  ldloc.0
    IL_0029:  ldc.i4.s   25
    IL_002b:  beq.s      IL_0075

    IL_002d:  ldloc.0
    IL_002e:  ldc.i4.s   30
    IL_0030:  beq.s      IL_0082

    IL_0032:  br.s       IL_008f

    IL_0034:  ldstr      "zero"
    IL_0039:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003e:  nop
    IL_003f:  br.s       IL_008f

    IL_0041:  ldstr      "five"
    IL_0046:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004b:  nop
    IL_004c:  br.s       IL_008f

    IL_004e:  ldstr      "ten"
    IL_0053:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0058:  nop
    IL_0059:  br.s       IL_008f

    IL_005b:  ldstr      "fifteen"
    IL_0060:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0065:  nop
    IL_0066:  br.s       IL_008f

    IL_0068:  ldstr      "twenty"
    IL_006d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0072:  nop
    IL_0073:  br.s       IL_008f

    IL_0075:  ldstr      "twenty-five"
    IL_007a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007f:  nop
    IL_0080:  br.s       IL_008f

    IL_0082:  ldstr      "thirty"
    IL_0087:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_008c:  nop
    IL_008d:  br.s       IL_008f

    IL_008f:  ret
  } // end of method Switch::SwitchOverInt

  .method public hidebysig static void  CompactSwitchOverInt(int32 i) cil managed
  {
    // Code size       78 (0x4e)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  switch     ( 
                          IL_001b,
                          IL_001b,
                          IL_001b,
                          IL_0028)
    IL_0019:  br.s       IL_0035

    IL_001b:  ldstr      "012"
    IL_0020:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0025:  nop
    IL_0026:  br.s       IL_0042

    IL_0028:  ldstr      "3"
    IL_002d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0032:  nop
    IL_0033:  br.s       IL_0042

    IL_0035:  ldstr      "default"
    IL_003a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003f:  nop
    IL_0040:  br.s       IL_0042

    IL_0042:  ldstr      "end"
    IL_0047:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004c:  nop
    IL_004d:  ret
  } // end of method Switch::CompactSwitchOverInt

  .method public hidebysig static string 
          ShortSwitchOverString(string text) cil managed
  {
    // Code size       98 (0x62)
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
    IL_0015:  brfalse.s  IL_0058

    IL_0017:  ldloc.1
    IL_0018:  ldstr      "First case"
    IL_001d:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0022:  brtrue.s   IL_0040

    IL_0024:  ldloc.1
    IL_0025:  ldstr      "Second case"
    IL_002a:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_002f:  brtrue.s   IL_0048

    IL_0031:  ldloc.1
    IL_0032:  ldstr      "Third case"
    IL_0037:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_003c:  brtrue.s   IL_0050

    IL_003e:  br.s       IL_0058

    IL_0040:  ldstr      "Text1"
    IL_0045:  stloc.0
    IL_0046:  br.s       IL_0060

    IL_0048:  ldstr      "Text2"
    IL_004d:  stloc.0
    IL_004e:  br.s       IL_0060

    IL_0050:  ldstr      "Text3"
    IL_0055:  stloc.0
    IL_0056:  br.s       IL_0060

    IL_0058:  ldstr      "Default"
    IL_005d:  stloc.0
    IL_005e:  br.s       IL_0060

    IL_0060:  ldloc.0
    IL_0061:  ret
  } // end of method Switch::ShortSwitchOverString

  .method public hidebysig static string 
          ShortSwitchOverStringWithNullCase(string text) cil managed
  {
    // Code size       85 (0x55)
    .maxstack  2
    .locals init (string V_0,
             string V_1)
    IL_0000:  nop
    IL_0001:  ldstr      "ShortSwitchOverStringWithNullCase: "
    IL_0006:  ldarg.0
    IL_0007:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0011:  nop
    IL_0012:  ldarg.0
    IL_0013:  stloc.1
    IL_0014:  ldloc.1
    IL_0015:  brfalse.s  IL_0043

    IL_0017:  ldloc.1
    IL_0018:  ldstr      "First case"
    IL_001d:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0022:  brtrue.s   IL_0033

    IL_0024:  ldloc.1
    IL_0025:  ldstr      "Second case"
    IL_002a:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_002f:  brtrue.s   IL_003b

    IL_0031:  br.s       IL_004b

    IL_0033:  ldstr      "Text1"
    IL_0038:  stloc.0
    IL_0039:  br.s       IL_0053

    IL_003b:  ldstr      "Text2"
    IL_0040:  stloc.0
    IL_0041:  br.s       IL_0053

    IL_0043:  ldstr      "null"
    IL_0048:  stloc.0
    IL_0049:  br.s       IL_0053

    IL_004b:  ldstr      "Default"
    IL_0050:  stloc.0
    IL_0051:  br.s       IL_0053

    IL_0053:  ldloc.0
    IL_0054:  ret
  } // end of method Switch::ShortSwitchOverStringWithNullCase

  .method public hidebysig static string 
          SwitchOverString1(string text) cil managed
  {
    // Code size       247 (0xf7)
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
    IL_0015:  brfalse    IL_00e9

    IL_001a:  volatile.
    IL_001c:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000010-1'
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
    IL_007f:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000010-1'
    IL_0084:  volatile.
    IL_0086:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000010-1'
    IL_008b:  ldloc.1
    IL_008c:  ldloca.s   V_2
    IL_008e:  call       instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::TryGetValue(!0,
                                                                                                                          !1&)
    IL_0093:  brfalse.s  IL_00ed

    IL_0095:  ldloc.2
    IL_0096:  switch     ( 
                          IL_00b9,
                          IL_00c1,
                          IL_00c1,
                          IL_00c9,
                          IL_00d1,
                          IL_00d9,
                          IL_00e1)
    IL_00b7:  br.s       IL_00ed

    IL_00b9:  ldstr      "Text1"
    IL_00be:  stloc.0
    IL_00bf:  br.s       IL_00f5

    IL_00c1:  ldstr      "Text2"
    IL_00c6:  stloc.0
    IL_00c7:  br.s       IL_00f5

    IL_00c9:  ldstr      "Text3"
    IL_00ce:  stloc.0
    IL_00cf:  br.s       IL_00f5

    IL_00d1:  ldstr      "Text4"
    IL_00d6:  stloc.0
    IL_00d7:  br.s       IL_00f5

    IL_00d9:  ldstr      "Text5"
    IL_00de:  stloc.0
    IL_00df:  br.s       IL_00f5

    IL_00e1:  ldstr      "Text6"
    IL_00e6:  stloc.0
    IL_00e7:  br.s       IL_00f5

    IL_00e9:  ldnull
    IL_00ea:  stloc.0
    IL_00eb:  br.s       IL_00f5

    IL_00ed:  ldstr      "Default"
    IL_00f2:  stloc.0
    IL_00f3:  br.s       IL_00f5

    IL_00f5:  ldloc.0
    IL_00f6:  ret
  } // end of method Switch::SwitchOverString1

  .method public hidebysig static string 
          SwitchOverString2() cil managed
  {
    // Code size       354 (0x162)
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
    IL_0013:  brfalse    IL_0158

    IL_0018:  volatile.
    IL_001a:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000011-1'
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
    IL_00b3:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000011-1'
    IL_00b8:  volatile.
    IL_00ba:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000011-1'
    IL_00bf:  ldloc.1
    IL_00c0:  ldloca.s   V_2
    IL_00c2:  call       instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::TryGetValue(!0,
                                                                                                                          !1&)
    IL_00c7:  brfalse    IL_0158

    IL_00cc:  ldloc.2
    IL_00cd:  switch     ( 
                          IL_0100,
                          IL_0108,
                          IL_0110,
                          IL_0118,
                          IL_0120,
                          IL_0128,
                          IL_0130,
                          IL_0138,
                          IL_0140,
                          IL_0148,
                          IL_0150)
    IL_00fe:  br.s       IL_0158

    IL_0100:  ldstr      "Text1"
    IL_0105:  stloc.0
    IL_0106:  br.s       IL_0160

    IL_0108:  ldstr      "Text2"
    IL_010d:  stloc.0
    IL_010e:  br.s       IL_0160

    IL_0110:  ldstr      "Text3"
    IL_0115:  stloc.0
    IL_0116:  br.s       IL_0160

    IL_0118:  ldstr      "Text4"
    IL_011d:  stloc.0
    IL_011e:  br.s       IL_0160

    IL_0120:  ldstr      "Text5"
    IL_0125:  stloc.0
    IL_0126:  br.s       IL_0160

    IL_0128:  ldstr      "Text6"
    IL_012d:  stloc.0
    IL_012e:  br.s       IL_0160

    IL_0130:  ldstr      "Text7"
    IL_0135:  stloc.0
    IL_0136:  br.s       IL_0160

    IL_0138:  ldstr      "Text8"
    IL_013d:  stloc.0
    IL_013e:  br.s       IL_0160

    IL_0140:  ldstr      "Text9"
    IL_0145:  stloc.0
    IL_0146:  br.s       IL_0160

    IL_0148:  ldstr      "Text10"
    IL_014d:  stloc.0
    IL_014e:  br.s       IL_0160

    IL_0150:  ldstr      "Text11"
    IL_0155:  stloc.0
    IL_0156:  br.s       IL_0160

    IL_0158:  ldstr      "Default"
    IL_015d:  stloc.0
    IL_015e:  br.s       IL_0160

    IL_0160:  ldloc.0
    IL_0161:  ret
  } // end of method Switch::SwitchOverString2

  .method public hidebysig static string 
          SwitchOverBool(bool b) cil managed
  {
    // Code size       64 (0x40)
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
                          IL_0032,
                          IL_002a)
    IL_0028:  br.s       IL_003a

    IL_002a:  ldsfld     string [mscorlib]System.Boolean::TrueString
    IL_002f:  stloc.0
    IL_0030:  br.s       IL_003e

    IL_0032:  ldsfld     string [mscorlib]System.Boolean::FalseString
    IL_0037:  stloc.0
    IL_0038:  br.s       IL_003e

    IL_003a:  ldnull
    IL_003b:  stloc.0
    IL_003c:  br.s       IL_003e

    IL_003e:  ldloc.0
    IL_003f:  ret
  } // end of method Switch::SwitchOverBool

  .method public hidebysig static void  SwitchInLoop(int32 i) cil managed
  {
    // Code size       128 (0x80)
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
    IL_0017:  br.s       IL_007b

    IL_0019:  nop
    IL_001a:  ldarg.0
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.1
    IL_001e:  sub
    IL_001f:  switch     ( 
                          IL_0036,
                          IL_0043,
                          IL_005d,
                          IL_0050)
    IL_0034:  br.s       IL_005d

    IL_0036:  ldstr      "one"
    IL_003b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0040:  nop
    IL_0041:  br.s       IL_0075

    IL_0043:  ldstr      "two"
    IL_0048:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004d:  nop
    IL_004e:  br.s       IL_0075

    IL_0050:  ldstr      "four"
    IL_0055:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005a:  nop
    IL_005b:  br.s       IL_007f

    IL_005d:  ldstr      "default"
    IL_0062:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0067:  nop
    IL_0068:  ldstr      "more code"
    IL_006d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0072:  nop
    IL_0073:  br.s       IL_007f

    IL_0075:  ldarg.0
    IL_0076:  ldc.i4.1
    IL_0077:  add
    IL_0078:  starg.s    i
    IL_007a:  nop
    IL_007b:  ldc.i4.1
    IL_007c:  stloc.1
    IL_007d:  br.s       IL_0019

    IL_007f:  ret
  } // end of method Switch::SwitchInLoop

  .method public hidebysig static void  SwitchWithGoto(int32 i) cil managed
  {
    // Code size       128 (0x80)
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
    IL_0065:  br.s       IL_007f

    IL_0067:  ldstr      "default"
    IL_006c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0071:  nop
    IL_0072:  br.s       IL_0074

    IL_0074:  ldstr      "End of method"
    IL_0079:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007e:  nop
    IL_007f:  ret
  } // end of method Switch::SwitchWithGoto

  .method public hidebysig static void  SwitchWithGotoString(string s) cil managed
  {
    // Code size       363 (0x16b)
    .maxstack  4
    .locals init (string V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldstr      "SwitchWithGotoString: "
    IL_0006:  ldarg.0
    IL_0007:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0011:  nop
    IL_0012:  ldarg.0
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  brfalse    IL_0152

    IL_001a:  volatile.
    IL_001c:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000015-1'
    IL_0021:  brtrue.s   IL_009d

    IL_0023:  ldc.i4.s   9
    IL_0025:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::.ctor(int32)
    IL_002a:  dup
    IL_002b:  ldstr      "1"
    IL_0030:  ldc.i4.0
    IL_0031:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0036:  dup
    IL_0037:  ldstr      "2"
    IL_003c:  ldc.i4.1
    IL_003d:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0042:  dup
    IL_0043:  ldstr      "3"
    IL_0048:  ldc.i4.2
    IL_0049:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_004e:  dup
    IL_004f:  ldstr      "4"
    IL_0054:  ldc.i4.3
    IL_0055:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_005a:  dup
    IL_005b:  ldstr      "5"
    IL_0060:  ldc.i4.4
    IL_0061:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0066:  dup
    IL_0067:  ldstr      "6"
    IL_006c:  ldc.i4.5
    IL_006d:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0072:  dup
    IL_0073:  ldstr      "7"
    IL_0078:  ldc.i4.6
    IL_0079:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_007e:  dup
    IL_007f:  ldstr      "8"
    IL_0084:  ldc.i4.7
    IL_0085:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_008a:  dup
    IL_008b:  ldstr      "9"
    IL_0090:  ldc.i4.8
    IL_0091:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0096:  volatile.
    IL_0098:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000015-1'
    IL_009d:  volatile.
    IL_009f:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000015-1'
    IL_00a4:  ldloc.0
    IL_00a5:  ldloca.s   V_1
    IL_00a7:  call       instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::TryGetValue(!0,
                                                                                                                          !1&)
    IL_00ac:  brfalse    IL_0152

    IL_00b1:  ldloc.1
    IL_00b2:  switch     ( 
                          IL_00dd,
                          IL_00ea,
                          IL_00f7,
                          IL_0104,
                          IL_0111,
                          IL_011e,
                          IL_012b,
                          IL_0138,
                          IL_0145)
    IL_00db:  br.s       IL_0152

    IL_00dd:  ldstr      "one"
    IL_00e2:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00e7:  nop
    IL_00e8:  br.s       IL_0152

    IL_00ea:  ldstr      "two"
    IL_00ef:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00f4:  nop
    IL_00f5:  br.s       IL_00f7

    IL_00f7:  ldstr      "three"
    IL_00fc:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0101:  nop
    IL_0102:  br.s       IL_015f

    IL_0104:  ldstr      "four"
    IL_0109:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_010e:  nop
    IL_010f:  br.s       IL_016a

    IL_0111:  ldstr      "five"
    IL_0116:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_011b:  nop
    IL_011c:  br.s       IL_016a

    IL_011e:  ldstr      "six"
    IL_0123:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0128:  nop
    IL_0129:  br.s       IL_016a

    IL_012b:  ldstr      "seven"
    IL_0130:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0135:  nop
    IL_0136:  br.s       IL_016a

    IL_0138:  ldstr      "eight"
    IL_013d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0142:  nop
    IL_0143:  br.s       IL_016a

    IL_0145:  ldstr      "nine"
    IL_014a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_014f:  nop
    IL_0150:  br.s       IL_016a

    IL_0152:  ldstr      "default"
    IL_0157:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_015c:  nop
    IL_015d:  br.s       IL_015f

    IL_015f:  ldstr      "End of method"
    IL_0164:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0169:  nop
    IL_016a:  ret
  } // end of method Switch::SwitchWithGotoString

  .method public hidebysig static void  SwitchWithGotoComplex(string s) cil managed
  {
    // Code size       338 (0x152)
    .maxstack  4
    .locals init (string V_0,
             int32 V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldstr      "SwitchWithGotoComplex: "
    IL_0006:  ldarg.0
    IL_0007:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0011:  nop
    IL_0012:  ldarg.0
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  brfalse    IL_0137

    IL_001a:  volatile.
    IL_001c:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000016-1'
    IL_0021:  brtrue.s   IL_0090

    IL_0023:  ldc.i4.8
    IL_0024:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::.ctor(int32)
    IL_0029:  dup
    IL_002a:  ldstr      "1"
    IL_002f:  ldc.i4.0
    IL_0030:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0035:  dup
    IL_0036:  ldstr      "2"
    IL_003b:  ldc.i4.1
    IL_003c:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0041:  dup
    IL_0042:  ldstr      "3"
    IL_0047:  ldc.i4.2
    IL_0048:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_004d:  dup
    IL_004e:  ldstr      "4"
    IL_0053:  ldc.i4.3
    IL_0054:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0059:  dup
    IL_005a:  ldstr      "5"
    IL_005f:  ldc.i4.4
    IL_0060:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0065:  dup
    IL_0066:  ldstr      "6"
    IL_006b:  ldc.i4.5
    IL_006c:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0071:  dup
    IL_0072:  ldstr      "8"
    IL_0077:  ldc.i4.6
    IL_0078:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_007d:  dup
    IL_007e:  ldstr      "7"
    IL_0083:  ldc.i4.7
    IL_0084:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0089:  volatile.
    IL_008b:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000016-1'
    IL_0090:  volatile.
    IL_0092:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000016-1'
    IL_0097:  ldloc.0
    IL_0098:  ldloca.s   V_1
    IL_009a:  call       instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::TryGetValue(!0,
                                                                                                                          !1&)
    IL_009f:  brfalse    IL_0137

    IL_00a4:  ldloc.1
    IL_00a5:  switch     ( 
                          IL_00cc,
                          IL_00d9,
                          IL_00e6,
                          IL_0103,
                          IL_0110,
                          IL_011d,
                          IL_012a,
                          IL_0144)
    IL_00ca:  br.s       IL_0137

    IL_00cc:  ldstr      "one"
    IL_00d1:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00d6:  nop
    IL_00d7:  br.s       IL_012a

    IL_00d9:  ldstr      "two"
    IL_00de:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00e3:  nop
    IL_00e4:  br.s       IL_00e6

    IL_00e6:  ldstr      "three"
    IL_00eb:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00f0:  nop
    IL_00f1:  ldarg.0
    IL_00f2:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_00f7:  ldc.i4.2
    IL_00f8:  ceq
    IL_00fa:  stloc.2
    IL_00fb:  ldloc.2
    IL_00fc:  brtrue.s   IL_0101

    IL_00fe:  nop
    IL_00ff:  br.s       IL_0146

    IL_0101:  br.s       IL_0110

    IL_0103:  ldstr      "four"
    IL_0108:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_010d:  nop
    IL_010e:  br.s       IL_0110

    IL_0110:  ldstr      "five"
    IL_0115:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_011a:  nop
    IL_011b:  br.s       IL_012a

    IL_011d:  ldstr      "six"
    IL_0122:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0127:  nop
    IL_0128:  br.s       IL_0110

    IL_012a:  ldstr      "eight"
    IL_012f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0134:  nop
    IL_0135:  br.s       IL_0151

    IL_0137:  ldstr      "default"
    IL_013c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0141:  nop
    IL_0142:  br.s       IL_0146

    IL_0144:  br.s       IL_0146

    IL_0146:  ldstr      "End of method"
    IL_014b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0150:  nop
    IL_0151:  ret
  } // end of method Switch::SwitchWithGotoComplex

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
    // Code size       332 (0x14c)
    .maxstack  4
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty> V_0,
             class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty> V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty[] V_2,
             int32 V_3,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty V_4,
             string V_5,
             int32 V_6,
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
    IL_0015:  br         IL_013c

    IL_001a:  nop
    IL_001b:  ldstr      "In for-loop"
    IL_0020:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0025:  nop
    IL_0026:  ldloc.2
    IL_0027:  ldloc.3
    IL_0028:  ldelem.ref
    IL_0029:  stloc.s    V_4
    IL_002b:  ldloc.s    V_4
    IL_002d:  ldfld      class [mscorlib]System.Reflection.PropertyInfo ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::Property
    IL_0032:  callvirt   instance string [mscorlib]System.Reflection.MemberInfo::get_Name()
    IL_0037:  stloc.s    V_5
    IL_0039:  ldloc.s    V_5
    IL_003b:  brfalse    IL_012c

    IL_0040:  volatile.
    IL_0042:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000018-1'
    IL_0047:  brtrue.s   IL_009e

    IL_0049:  ldc.i4.6
    IL_004a:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::.ctor(int32)
    IL_004f:  dup
    IL_0050:  ldstr      "Name1"
    IL_0055:  ldc.i4.0
    IL_0056:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_005b:  dup
    IL_005c:  ldstr      "Name2"
    IL_0061:  ldc.i4.1
    IL_0062:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0067:  dup
    IL_0068:  ldstr      "Name3"
    IL_006d:  ldc.i4.2
    IL_006e:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0073:  dup
    IL_0074:  ldstr      "Name4"
    IL_0079:  ldc.i4.3
    IL_007a:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_007f:  dup
    IL_0080:  ldstr      "Name5"
    IL_0085:  ldc.i4.4
    IL_0086:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_008b:  dup
    IL_008c:  ldstr      "Name6"
    IL_0091:  ldc.i4.5
    IL_0092:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0097:  volatile.
    IL_0099:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000018-1'
    IL_009e:  volatile.
    IL_00a0:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000018-1'
    IL_00a5:  ldloc.s    V_5
    IL_00a7:  ldloca.s   V_6
    IL_00a9:  call       instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::TryGetValue(!0,
                                                                                                                          !1&)
    IL_00ae:  brfalse.s  IL_012c

    IL_00b0:  ldloc.s    V_6
    IL_00b2:  switch     ( 
                          IL_00d1,
                          IL_00e5,
                          IL_00f9,
                          IL_010d,
                          IL_0121,
                          IL_0121)
    IL_00cf:  br.s       IL_012c

    IL_00d1:  ldloc.s    V_4
    IL_00d3:  ldc.i4.1
    IL_00d4:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00d9:  nop
    IL_00da:  ldloc.0
    IL_00db:  ldloc.s    V_4
    IL_00dd:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00e2:  nop
    IL_00e3:  br.s       IL_0137

    IL_00e5:  ldloc.s    V_4
    IL_00e7:  ldc.i4.2
    IL_00e8:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00ed:  nop
    IL_00ee:  ldloc.0
    IL_00ef:  ldloc.s    V_4
    IL_00f1:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00f6:  nop
    IL_00f7:  br.s       IL_0137

    IL_00f9:  ldloc.s    V_4
    IL_00fb:  ldc.i4.3
    IL_00fc:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_0101:  nop
    IL_0102:  ldloc.0
    IL_0103:  ldloc.s    V_4
    IL_0105:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_010a:  nop
    IL_010b:  br.s       IL_0137

    IL_010d:  ldloc.s    V_4
    IL_010f:  ldc.i4.4
    IL_0110:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_0115:  nop
    IL_0116:  ldloc.0
    IL_0117:  ldloc.s    V_4
    IL_0119:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_011e:  nop
    IL_011f:  br.s       IL_0137

    IL_0121:  ldloc.0
    IL_0122:  ldloc.s    V_4
    IL_0124:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_0129:  nop
    IL_012a:  br.s       IL_0137

    IL_012c:  ldloc.1
    IL_012d:  ldloc.s    V_4
    IL_012f:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_0134:  nop
    IL_0135:  br.s       IL_0137

    IL_0137:  nop
    IL_0138:  ldloc.3
    IL_0139:  ldc.i4.1
    IL_013a:  add
    IL_013b:  stloc.3
    IL_013c:  ldloc.3
    IL_013d:  ldloc.2
    IL_013e:  ldlen
    IL_013f:  conv.i4
    IL_0140:  clt
    IL_0142:  stloc.s    V_7
    IL_0144:  ldloc.s    V_7
    IL_0146:  brtrue     IL_001a

    IL_014b:  ret
  } // end of method Switch::SwitchOnStringInForLoop

  .method public hidebysig static void  SwitchInTryBlock(string 'value') cil managed
  {
    // Code size       258 (0x102)
    .maxstack  4
    .locals init (string V_0,
             int32 V_1)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldarg.0
      IL_0003:  ldc.i4.5
      IL_0004:  callvirt   instance string [mscorlib]System.String::Substring(int32)
      IL_0009:  stloc.0
      IL_000a:  ldloc.0
      IL_000b:  brfalse    IL_00e0

      IL_0010:  volatile.
      IL_0012:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000019-1'
      IL_0017:  brtrue.s   IL_006e

      IL_0019:  ldc.i4.6
      IL_001a:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::.ctor(int32)
      IL_001f:  dup
      IL_0020:  ldstr      "Name1"
      IL_0025:  ldc.i4.0
      IL_0026:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                    !1)
      IL_002b:  dup
      IL_002c:  ldstr      "Name2"
      IL_0031:  ldc.i4.1
      IL_0032:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                    !1)
      IL_0037:  dup
      IL_0038:  ldstr      "Name3"
      IL_003d:  ldc.i4.2
      IL_003e:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                    !1)
      IL_0043:  dup
      IL_0044:  ldstr      "Name4"
      IL_0049:  ldc.i4.3
      IL_004a:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                    !1)
      IL_004f:  dup
      IL_0050:  ldstr      "Name5"
      IL_0055:  ldc.i4.4
      IL_0056:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                    !1)
      IL_005b:  dup
      IL_005c:  ldstr      "Name6"
      IL_0061:  ldc.i4.5
      IL_0062:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                    !1)
      IL_0067:  volatile.
      IL_0069:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000019-1'
      IL_006e:  volatile.
      IL_0070:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000019-1'
      IL_0075:  ldloc.0
      IL_0076:  ldloca.s   V_1
      IL_0078:  call       instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::TryGetValue(!0,
                                                                                                                            !1&)
      IL_007d:  brfalse.s  IL_00e0

      IL_007f:  ldloc.1
      IL_0080:  switch     ( 
                            IL_009f,
                            IL_00ac,
                            IL_00b9,
                            IL_00c6,
                            IL_00d3,
                            IL_00d3)
      IL_009d:  br.s       IL_00e0

      IL_009f:  ldstr      "1"
      IL_00a4:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00a9:  nop
      IL_00aa:  br.s       IL_00ed

      IL_00ac:  ldstr      "Name_2"
      IL_00b1:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00b6:  nop
      IL_00b7:  br.s       IL_00ed

      IL_00b9:  ldstr      "Name_3"
      IL_00be:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00c3:  nop
      IL_00c4:  br.s       IL_00ed

      IL_00c6:  ldstr      "No. 4"
      IL_00cb:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00d0:  nop
      IL_00d1:  br.s       IL_00ed

      IL_00d3:  ldstr      "5+6"
      IL_00d8:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00dd:  nop
      IL_00de:  br.s       IL_00ed

      IL_00e0:  ldstr      "default"
      IL_00e5:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00ea:  nop
      IL_00eb:  br.s       IL_00ed

      IL_00ed:  nop
      IL_00ee:  leave.s    IL_0100

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_00f0:  pop
      IL_00f1:  nop
      IL_00f2:  ldstr      "catch block"
      IL_00f7:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00fc:  nop
      IL_00fd:  nop
      IL_00fe:  leave.s    IL_0100

    }  // end handler
    IL_0100:  nop
    IL_0101:  ret
  } // end of method Switch::SwitchInTryBlock

  .method public hidebysig static void  SwitchWithComplexCondition(string[] args) cil managed
  {
    // Code size       139 (0x8b)
    .maxstack  2
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldlen
    IL_0003:  conv.i4
    IL_0004:  brfalse.s  IL_000b

    IL_0006:  ldarg.0
    IL_0007:  ldc.i4.0
    IL_0008:  ldelem.ref
    IL_0009:  br.s       IL_0010

    IL_000b:  ldstr      "dummy"
    IL_0010:  nop
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  brfalse.s  IL_007f

    IL_0015:  ldloc.0
    IL_0016:  ldstr      "a"
    IL_001b:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0020:  brtrue.s   IL_004b

    IL_0022:  ldloc.0
    IL_0023:  ldstr      "b"
    IL_0028:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_002d:  brtrue.s   IL_0058

    IL_002f:  ldloc.0
    IL_0030:  ldstr      "c"
    IL_0035:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_003a:  brtrue.s   IL_0065

    IL_003c:  ldloc.0
    IL_003d:  ldstr      "d"
    IL_0042:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0047:  brtrue.s   IL_0072

    IL_0049:  br.s       IL_007f

    IL_004b:  ldstr      "a"
    IL_0050:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0055:  nop
    IL_0056:  br.s       IL_007f

    IL_0058:  ldstr      "b"
    IL_005d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0062:  nop
    IL_0063:  br.s       IL_007f

    IL_0065:  ldstr      "c"
    IL_006a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006f:  nop
    IL_0070:  br.s       IL_007f

    IL_0072:  ldstr      "d"
    IL_0077:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007c:  nop
    IL_007d:  br.s       IL_007f

    IL_007f:  ldstr      "end"
    IL_0084:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0089:  nop
    IL_008a:  ret
  } // end of method Switch::SwitchWithComplexCondition

  .method public hidebysig static void  SwitchWithArray(string[] args) cil managed
  {
    // Code size       126 (0x7e)
    .maxstack  2
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  ldelem.ref
    IL_0004:  stloc.0
    IL_0005:  ldloc.0
    IL_0006:  brfalse.s  IL_0072

    IL_0008:  ldloc.0
    IL_0009:  ldstr      "a"
    IL_000e:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0013:  brtrue.s   IL_003e

    IL_0015:  ldloc.0
    IL_0016:  ldstr      "b"
    IL_001b:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0020:  brtrue.s   IL_004b

    IL_0022:  ldloc.0
    IL_0023:  ldstr      "c"
    IL_0028:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_002d:  brtrue.s   IL_0058

    IL_002f:  ldloc.0
    IL_0030:  ldstr      "d"
    IL_0035:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_003a:  brtrue.s   IL_0065

    IL_003c:  br.s       IL_0072

    IL_003e:  ldstr      "a"
    IL_0043:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0048:  nop
    IL_0049:  br.s       IL_0072

    IL_004b:  ldstr      "b"
    IL_0050:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0055:  nop
    IL_0056:  br.s       IL_0072

    IL_0058:  ldstr      "c"
    IL_005d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0062:  nop
    IL_0063:  br.s       IL_0072

    IL_0065:  ldstr      "d"
    IL_006a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006f:  nop
    IL_0070:  br.s       IL_0072

    IL_0072:  ldstr      "end"
    IL_0077:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007c:  nop
    IL_007d:  ret
  } // end of method Switch::SwitchWithArray

  .method public hidebysig static void  SwitchWithContinue1(int32 i,
                                                            bool b) cil managed
  {
    // Code size       62 (0x3e)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  br.s       IL_003a

    IL_0003:  nop
    IL_0004:  ldarg.0
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  switch     ( 
                          IL_001a,
                          IL_0031,
                          IL_0027)
    IL_0018:  br.s       IL_0033

    IL_001a:  ldarg.1
    IL_001b:  ldc.i4.0
    IL_001c:  ceq
    IL_001e:  stloc.1
    IL_001f:  ldloc.1
    IL_0020:  brtrue.s   IL_0025

    IL_0022:  nop
    IL_0023:  br.s       IL_003a

    IL_0025:  br.s       IL_0033

    IL_0027:  ldarg.1
    IL_0028:  stloc.1
    IL_0029:  ldloc.1
    IL_002a:  brtrue.s   IL_002f

    IL_002c:  nop
    IL_002d:  br.s       IL_003a

    IL_002f:  br.s       IL_0033

    IL_0031:  br.s       IL_003a

    IL_0033:  call       void [mscorlib]System.Console::WriteLine()
    IL_0038:  nop
    IL_0039:  nop
    IL_003a:  ldc.i4.1
    IL_003b:  stloc.1
    IL_003c:  br.s       IL_0003
  } // end of method Switch::SwitchWithContinue1

  .method public hidebysig static void  SwitchWithContinue2(int32 i,
                                                            bool b) cil managed
  {
    // Code size       147 (0x93)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  br         IL_0086

    IL_0006:  nop
    IL_0007:  ldarg.0
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  switch     ( 
                          IL_0021,
                          IL_0073,
                          IL_0044,
                          IL_0071)
    IL_001f:  br.s       IL_0064

    IL_0021:  ldarg.1
    IL_0022:  ldc.i4.0
    IL_0023:  ceq
    IL_0025:  stloc.1
    IL_0026:  ldloc.1
    IL_0027:  brtrue.s   IL_0037

    IL_0029:  nop
    IL_002a:  ldstr      "0b"
    IL_002f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0034:  nop
    IL_0035:  br.s       IL_0086

    IL_0037:  ldstr      "0!b"
    IL_003c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0041:  nop
    IL_0042:  br.s       IL_0075

    IL_0044:  ldarg.1
    IL_0045:  stloc.1
    IL_0046:  ldloc.1
    IL_0047:  brtrue.s   IL_0057

    IL_0049:  nop
    IL_004a:  ldstr      "2!b"
    IL_004f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0054:  nop
    IL_0055:  br.s       IL_0086

    IL_0057:  ldstr      "2b"
    IL_005c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0061:  nop
    IL_0062:  br.s       IL_0092

    IL_0064:  ldstr      "default"
    IL_0069:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006e:  nop
    IL_006f:  br.s       IL_0075

    IL_0071:  br.s       IL_0075

    IL_0073:  br.s       IL_0086

    IL_0075:  ldstr      "loop-tail"
    IL_007a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007f:  nop
    IL_0080:  ldarg.0
    IL_0081:  ldc.i4.1
    IL_0082:  add
    IL_0083:  starg.s    i
    IL_0085:  nop
    IL_0086:  ldarg.0
    IL_0087:  ldc.i4.s   10
    IL_0089:  clt
    IL_008b:  stloc.1
    IL_008c:  ldloc.1
    IL_008d:  brtrue     IL_0006

    IL_0092:  ret
  } // end of method Switch::SwitchWithContinue2

  .method public hidebysig static void  SwitchWithContinue3(bool b) cil managed
  {
    // Code size       145 (0x91)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0084

    IL_0005:  nop
    IL_0006:  ldloc.0
    IL_0007:  stloc.1
    IL_0008:  ldloc.1
    IL_0009:  switch     ( 
                          IL_0020,
                          IL_0072,
                          IL_0043,
                          IL_0070)
    IL_001e:  br.s       IL_0063

    IL_0020:  ldarg.0
    IL_0021:  ldc.i4.0
    IL_0022:  ceq
    IL_0024:  stloc.2
    IL_0025:  ldloc.2
    IL_0026:  brtrue.s   IL_0036

    IL_0028:  nop
    IL_0029:  ldstr      "0b"
    IL_002e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0033:  nop
    IL_0034:  br.s       IL_0080

    IL_0036:  ldstr      "0!b"
    IL_003b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0040:  nop
    IL_0041:  br.s       IL_0074

    IL_0043:  ldarg.0
    IL_0044:  stloc.2
    IL_0045:  ldloc.2
    IL_0046:  brtrue.s   IL_0056

    IL_0048:  nop
    IL_0049:  ldstr      "2!b"
    IL_004e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0053:  nop
    IL_0054:  br.s       IL_0080

    IL_0056:  ldstr      "2b"
    IL_005b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0060:  nop
    IL_0061:  br.s       IL_0090

    IL_0063:  ldstr      "default"
    IL_0068:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006d:  nop
    IL_006e:  br.s       IL_0074

    IL_0070:  br.s       IL_0074

    IL_0072:  br.s       IL_0080

    IL_0074:  ldstr      "loop-tail"
    IL_0079:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007e:  nop
    IL_007f:  nop
    IL_0080:  ldloc.0
    IL_0081:  ldc.i4.1
    IL_0082:  add
    IL_0083:  stloc.0
    IL_0084:  ldloc.0
    IL_0085:  ldc.i4.s   10
    IL_0087:  clt
    IL_0089:  stloc.2
    IL_008a:  ldloc.2
    IL_008b:  brtrue     IL_0005

    IL_0090:  ret
  } // end of method Switch::SwitchWithContinue3

  .method public hidebysig static void  SwitchWithContinue4(bool b) cil managed
  {
    // Code size       263 (0x107)
    .maxstack  2
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
             int32 V_2,
             bool V_3)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldc.i4.0
    IL_0003:  ldc.i4.s   10
    IL_0005:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [System.Core]System.Linq.Enumerable::Range(int32,
                                                                                                                                    int32)
    IL_000a:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_000f:  stloc.1
    .try
    {
      IL_0010:  br         IL_00e5

      IL_0015:  ldloc.1
      IL_0016:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_001b:  stloc.0
      IL_001c:  nop
      IL_001d:  ldstr      "loop: "
      IL_0022:  ldloc.0
      IL_0023:  box        [mscorlib]System.Int32
      IL_0028:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_002d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0032:  nop
      IL_0033:  ldloc.0
      IL_0034:  stloc.2
      IL_0035:  ldloc.2
      IL_0036:  ldc.i4.1
      IL_0037:  sub
      IL_0038:  switch     ( 
                            IL_005f,
                            IL_00cc,
                            IL_006c,
                            IL_0079,
                            IL_0082,
                            IL_008b,
                            IL_0098,
                            IL_00b2)
      IL_005d:  br.s       IL_00bf

      IL_005f:  ldarg.0
      IL_0060:  ldc.i4.0
      IL_0061:  ceq
      IL_0063:  stloc.3
      IL_0064:  ldloc.3
      IL_0065:  brtrue.s   IL_006a

      IL_0067:  nop
      IL_0068:  br.s       IL_00e5

      IL_006a:  br.s       IL_00ce

      IL_006c:  ldarg.0
      IL_006d:  stloc.3
      IL_006e:  ldloc.3
      IL_006f:  brtrue.s   IL_0074

      IL_0071:  nop
      IL_0072:  br.s       IL_00e5

      IL_0074:  leave      IL_0105

      IL_0079:  ldc.i4.4
      IL_007a:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_007f:  nop
      IL_0080:  br.s       IL_0098

      IL_0082:  ldc.i4.5
      IL_0083:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_0088:  nop
      IL_0089:  br.s       IL_00bf

      IL_008b:  ldarg.0
      IL_008c:  ldc.i4.0
      IL_008d:  ceq
      IL_008f:  stloc.3
      IL_0090:  ldloc.3
      IL_0091:  brtrue.s   IL_0096

      IL_0093:  nop
      IL_0094:  br.s       IL_00e5

      IL_0096:  br.s       IL_006c

      IL_0098:  ldloc.0
      IL_0099:  ldc.i4.2
      IL_009a:  rem
      IL_009b:  ldc.i4.0
      IL_009c:  ceq
      IL_009e:  ldc.i4.0
      IL_009f:  ceq
      IL_00a1:  stloc.3
      IL_00a2:  ldloc.3
      IL_00a3:  brtrue.s   IL_00a8

      IL_00a5:  nop
      IL_00a6:  br.s       IL_006c

      IL_00a8:  ldarg.0
      IL_00a9:  stloc.3
      IL_00aa:  ldloc.3
      IL_00ab:  brtrue.s   IL_00b0

      IL_00ad:  nop
      IL_00ae:  br.s       IL_00e5

      IL_00b0:  br.s       IL_00b2

      IL_00b2:  ldarg.0
      IL_00b3:  ldc.i4.0
      IL_00b4:  ceq
      IL_00b6:  stloc.3
      IL_00b7:  ldloc.3
      IL_00b8:  brtrue.s   IL_00bd

      IL_00ba:  nop
      IL_00bb:  br.s       IL_00e5

      IL_00bd:  br.s       IL_0082

      IL_00bf:  ldstr      "default"
      IL_00c4:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00c9:  nop
      IL_00ca:  br.s       IL_00ce

      IL_00cc:  br.s       IL_00e5

      IL_00ce:  ldstr      "break: "
      IL_00d3:  ldloc.0
      IL_00d4:  box        [mscorlib]System.Int32
      IL_00d9:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_00de:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00e3:  nop
      IL_00e4:  nop
      IL_00e5:  ldloc.1
      IL_00e6:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_00eb:  stloc.3
      IL_00ec:  ldloc.3
      IL_00ed:  brtrue     IL_0015

      IL_00f2:  leave.s    IL_0104

    }  // end .try
    finally
    {
      IL_00f4:  ldloc.1
      IL_00f5:  ldnull
      IL_00f6:  ceq
      IL_00f8:  stloc.3
      IL_00f9:  ldloc.3
      IL_00fa:  brtrue.s   IL_0103

      IL_00fc:  ldloc.1
      IL_00fd:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0102:  nop
      IL_0103:  endfinally
    }  // end handler
    IL_0104:  nop
    IL_0105:  nop
    IL_0106:  ret
  } // end of method Switch::SwitchWithContinue4

  .method public hidebysig static void  SwitchWithContinue5(bool b) cil managed
  {
    // Code size       172 (0xac)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1,
             int32 V_2)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br         IL_009f

    IL_0008:  nop
    IL_0009:  ldloc.0
    IL_000a:  ldc.i4.5
    IL_000b:  clt
    IL_000d:  ldc.i4.0
    IL_000e:  ceq
    IL_0010:  stloc.1
    IL_0011:  ldloc.1
    IL_0012:  brtrue.s   IL_008f

    IL_0014:  nop
    IL_0015:  ldloc.0
    IL_0016:  stloc.2
    IL_0017:  ldloc.2
    IL_0018:  switch     ( 
                          IL_002f,
                          IL_0081,
                          IL_0052,
                          IL_007f)
    IL_002d:  br.s       IL_0072

    IL_002f:  ldarg.0
    IL_0030:  ldc.i4.0
    IL_0031:  ceq
    IL_0033:  stloc.1
    IL_0034:  ldloc.1
    IL_0035:  brtrue.s   IL_0045

    IL_0037:  nop
    IL_0038:  ldstr      "0b"
    IL_003d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0042:  nop
    IL_0043:  br.s       IL_009b

    IL_0045:  ldstr      "0!b"
    IL_004a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004f:  nop
    IL_0050:  br.s       IL_0083

    IL_0052:  ldarg.0
    IL_0053:  stloc.1
    IL_0054:  ldloc.1
    IL_0055:  brtrue.s   IL_0065

    IL_0057:  nop
    IL_0058:  ldstr      "2!b"
    IL_005d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0062:  nop
    IL_0063:  br.s       IL_009b

    IL_0065:  ldstr      "2b"
    IL_006a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006f:  nop
    IL_0070:  br.s       IL_00ab

    IL_0072:  ldstr      "default"
    IL_0077:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007c:  nop
    IL_007d:  br.s       IL_0083

    IL_007f:  br.s       IL_0083

    IL_0081:  br.s       IL_009b

    IL_0083:  ldstr      "break-target"
    IL_0088:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_008d:  nop
    IL_008e:  nop
    IL_008f:  ldstr      "loop-tail"
    IL_0094:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0099:  nop
    IL_009a:  nop
    IL_009b:  ldloc.0
    IL_009c:  ldc.i4.1
    IL_009d:  add
    IL_009e:  stloc.0
    IL_009f:  ldloc.0
    IL_00a0:  ldc.i4.s   10
    IL_00a2:  clt
    IL_00a4:  stloc.1
    IL_00a5:  ldloc.1
    IL_00a6:  brtrue     IL_0008

    IL_00ab:  ret
  } // end of method Switch::SwitchWithContinue5

  .method public hidebysig static void  SwitchWithContinue6(int32 i,
                                                            bool b) cil managed
  {
    // Code size       142 (0x8e)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.0
    IL_0003:  stloc.0
    IL_0004:  ldloc.0
    IL_0005:  switch     ( 
                          IL_001c,
                          IL_006e,
                          IL_003c,
                          IL_006c)
    IL_001a:  br.s       IL_005f

    IL_001c:  ldarg.1
    IL_001d:  stloc.1
    IL_001e:  ldloc.1
    IL_001f:  brtrue.s   IL_002f

    IL_0021:  nop
    IL_0022:  ldstr      "0!b"
    IL_0027:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002c:  nop
    IL_002d:  br.s       IL_0070

    IL_002f:  ldstr      "0b"
    IL_0034:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0039:  nop
    IL_003a:  br.s       IL_007c

    IL_003c:  ldarg.1
    IL_003d:  ldc.i4.0
    IL_003e:  ceq
    IL_0040:  stloc.1
    IL_0041:  ldloc.1
    IL_0042:  brtrue.s   IL_0052

    IL_0044:  nop
    IL_0045:  ldstr      "2b"
    IL_004a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004f:  nop
    IL_0050:  br.s       IL_008d

    IL_0052:  ldstr      "2!b"
    IL_0057:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005c:  nop
    IL_005d:  br.s       IL_007c

    IL_005f:  ldstr      "default"
    IL_0064:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0069:  nop
    IL_006a:  br.s       IL_0070

    IL_006c:  br.s       IL_0070

    IL_006e:  br.s       IL_007c

    IL_0070:  ldstr      "loop-tail"
    IL_0075:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007a:  nop
    IL_007b:  nop
    IL_007c:  ldarg.0
    IL_007d:  ldc.i4.1
    IL_007e:  add
    IL_007f:  dup
    IL_0080:  starg.s    i
    IL_0082:  ldc.i4.s   10
    IL_0084:  clt
    IL_0086:  stloc.1
    IL_0087:  ldloc.1
    IL_0088:  brtrue     IL_0001

    IL_008d:  ret
  } // end of method Switch::SwitchWithContinue6

  .method public hidebysig static void  SwitchWithContinue7() cil managed
  {
    // Code size       81 (0x51)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_003a

    IL_0005:  nop
    IL_0006:  ldstr      "loop-head"
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0010:  nop
    IL_0011:  ldloc.0
    IL_0012:  stloc.1
    IL_0013:  ldloc.1
    IL_0014:  switch     ( 
                          IL_0030,
                          IL_0032)
    IL_0021:  br.s       IL_0023

    IL_0023:  ldstr      "default"
    IL_0028:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002d:  nop
    IL_002e:  br.s       IL_0034

    IL_0030:  br.s       IL_0036

    IL_0032:  br.s       IL_0034

    IL_0034:  br.s       IL_0045

    IL_0036:  ldloc.0
    IL_0037:  ldc.i4.1
    IL_0038:  sub
    IL_0039:  stloc.0
    IL_003a:  ldloc.0
    IL_003b:  ldc.i4.0
    IL_003c:  clt
    IL_003e:  ldc.i4.0
    IL_003f:  ceq
    IL_0041:  stloc.2
    IL_0042:  ldloc.2
    IL_0043:  brtrue.s   IL_0005

    IL_0045:  ldstr      "end"
    IL_004a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004f:  nop
    IL_0050:  ret
  } // end of method Switch::SwitchWithContinue7

  .method public hidebysig static void  SwitchWithContinueInDoubleLoop() cil managed
  {
    // Code size       128 (0x80)
    .maxstack  2
    .locals init (bool V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3,
             bool V_4)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.0
    IL_0004:  stloc.1
    IL_0005:  br.s       IL_006d

    IL_0007:  nop
    IL_0008:  ldc.i4.0
    IL_0009:  stloc.2
    IL_000a:  br.s       IL_005d

    IL_000c:  nop
    IL_000d:  ldloc.1
    IL_000e:  ldloc.2
    IL_000f:  add
    IL_0010:  stloc.3
    IL_0011:  ldloc.3
    IL_0012:  ldc.i4.1
    IL_0013:  sub
    IL_0014:  switch     ( 
                          IL_0051,
                          IL_0053,
                          IL_0051,
                          IL_0053,
                          IL_0051,
                          IL_0053,
                          IL_0051)
    IL_0035:  ldloc.3
    IL_0036:  ldc.i4.s   11
    IL_0038:  sub
    IL_0039:  switch     ( 
                          IL_0051,
                          IL_0053,
                          IL_0051)
    IL_004a:  ldloc.3
    IL_004b:  ldc.i4.s   17
    IL_004d:  beq.s      IL_0051

    IL_004f:  br.s       IL_0053

    IL_0051:  br.s       IL_0055

    IL_0053:  br.s       IL_0059

    IL_0055:  ldc.i4.1
    IL_0056:  stloc.0
    IL_0057:  br.s       IL_0068

    IL_0059:  ldloc.2
    IL_005a:  ldc.i4.1
    IL_005b:  add
    IL_005c:  stloc.2
    IL_005d:  ldloc.2
    IL_005e:  ldc.i4.s   10
    IL_0060:  clt
    IL_0062:  stloc.s    V_4
    IL_0064:  ldloc.s    V_4
    IL_0066:  brtrue.s   IL_000c

    IL_0068:  nop
    IL_0069:  ldloc.1
    IL_006a:  ldc.i4.1
    IL_006b:  add
    IL_006c:  stloc.1
    IL_006d:  ldloc.1
    IL_006e:  ldc.i4.s   10
    IL_0070:  clt
    IL_0072:  stloc.s    V_4
    IL_0074:  ldloc.s    V_4
    IL_0076:  brtrue.s   IL_0007

    IL_0078:  ldloc.0
    IL_0079:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_007e:  nop
    IL_007f:  ret
  } // end of method Switch::SwitchWithContinueInDoubleLoop

  .method public hidebysig static void  SwitchLoopNesting() cil managed
  {
    // Code size       153 (0x99)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br         IL_008c

    IL_0008:  nop
    IL_0009:  ldloc.0
    IL_000a:  stloc.1
    IL_000b:  ldloc.1
    IL_000c:  switch     ( 
                          IL_001b,
                          IL_0024)
    IL_0019:  br.s       IL_002d

    IL_001b:  ldc.i4.0
    IL_001c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0021:  nop
    IL_0022:  br.s       IL_0060

    IL_0024:  ldc.i4.1
    IL_0025:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_002a:  nop
    IL_002b:  br.s       IL_0060

    IL_002d:  ldloc.0
    IL_002e:  ldc.i4.2
    IL_002f:  rem
    IL_0030:  ldc.i4.0
    IL_0031:  ceq
    IL_0033:  ldc.i4.0
    IL_0034:  ceq
    IL_0036:  stloc.2
    IL_0037:  ldloc.2
    IL_0038:  brtrue.s   IL_0058

    IL_003a:  nop
    IL_003b:  br.s       IL_004a

    IL_003d:  nop
    IL_003e:  ldloc.0
    IL_003f:  dup
    IL_0040:  ldc.i4.1
    IL_0041:  add
    IL_0042:  stloc.0
    IL_0043:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0048:  nop
    IL_0049:  nop
    IL_004a:  ldloc.0
    IL_004b:  ldc.i4.3
    IL_004c:  rem
    IL_004d:  ldc.i4.0
    IL_004e:  ceq
    IL_0050:  ldc.i4.0
    IL_0051:  ceq
    IL_0053:  stloc.2
    IL_0054:  ldloc.2
    IL_0055:  brtrue.s   IL_003d

    IL_0057:  nop
    IL_0058:  call       void [mscorlib]System.Console::WriteLine()
    IL_005d:  nop
    IL_005e:  br.s       IL_0060

    IL_0060:  ldloc.0
    IL_0061:  ldc.i4.4
    IL_0062:  cgt
    IL_0064:  ldc.i4.0
    IL_0065:  ceq
    IL_0067:  stloc.2
    IL_0068:  ldloc.2
    IL_0069:  brtrue.s   IL_007a

    IL_006b:  nop
    IL_006c:  ldstr      "high"
    IL_0071:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0076:  nop
    IL_0077:  nop
    IL_0078:  br.s       IL_0087

    IL_007a:  nop
    IL_007b:  ldstr      "low"
    IL_0080:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0085:  nop
    IL_0086:  nop
    IL_0087:  nop
    IL_0088:  ldloc.0
    IL_0089:  ldc.i4.1
    IL_008a:  add
    IL_008b:  stloc.0
    IL_008c:  ldloc.0
    IL_008d:  ldc.i4.s   10
    IL_008f:  clt
    IL_0091:  stloc.2
    IL_0092:  ldloc.2
    IL_0093:  brtrue     IL_0008

    IL_0098:  ret
  } // end of method Switch::SwitchLoopNesting

  .method public hidebysig static void  SingleIf2(int32 i,
                                                  bool a,
                                                  bool b) cil managed
  {
    // Code size       49 (0x31)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.1
    IL_0003:  beq.s      IL_001a

    IL_0005:  ldarg.0
    IL_0006:  ldc.i4.2
    IL_0007:  bne.un.s   IL_000c

    IL_0009:  ldarg.1
    IL_000a:  brtrue.s   IL_001a

    IL_000c:  ldarg.0
    IL_000d:  ldc.i4.3
    IL_000e:  bne.un.s   IL_0016

    IL_0010:  ldarg.2
    IL_0011:  ldc.i4.0
    IL_0012:  ceq
    IL_0014:  br.s       IL_0017

    IL_0016:  ldc.i4.1
    IL_0017:  nop
    IL_0018:  br.s       IL_001b

    IL_001a:  ldc.i4.0
    IL_001b:  nop
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  brtrue.s   IL_0029

    IL_0020:  nop
    IL_0021:  ldc.i4.1
    IL_0022:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0027:  nop
    IL_0028:  nop
    IL_0029:  ldc.i4.2
    IL_002a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_002f:  nop
    IL_0030:  ret
  } // end of method Switch::SingleIf2

  .method public hidebysig static void  SingleIf3(int32 i,
                                                  bool a,
                                                  bool b) cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  brtrue.s   IL_0016

    IL_0004:  ldarg.0
    IL_0005:  ldc.i4.1
    IL_0006:  beq.s      IL_0016

    IL_0008:  ldarg.0
    IL_0009:  ldc.i4.2
    IL_000a:  bne.un.s   IL_0012

    IL_000c:  ldarg.2
    IL_000d:  ldc.i4.0
    IL_000e:  ceq
    IL_0010:  br.s       IL_0013

    IL_0012:  ldc.i4.1
    IL_0013:  nop
    IL_0014:  br.s       IL_0017

    IL_0016:  ldc.i4.0
    IL_0017:  nop
    IL_0018:  stloc.0
    IL_0019:  ldloc.0
    IL_001a:  brtrue.s   IL_0025

    IL_001c:  nop
    IL_001d:  ldc.i4.1
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0023:  nop
    IL_0024:  nop
    IL_0025:  ldc.i4.2
    IL_0026:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_002b:  nop
    IL_002c:  ret
  } // end of method Switch::SingleIf3

  .method public hidebysig static void  SingleIf4(int32 i,
                                                  bool a) cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.1
    IL_0003:  beq.s      IL_0016

    IL_0005:  ldarg.0
    IL_0006:  ldc.i4.2
    IL_0007:  beq.s      IL_0016

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.3
    IL_000b:  beq.s      IL_0010

    IL_000d:  ldarg.1
    IL_000e:  brtrue.s   IL_0016

    IL_0010:  ldarg.0
    IL_0011:  ldc.i4.4
    IL_0012:  ceq
    IL_0014:  br.s       IL_0017

    IL_0016:  ldc.i4.0
    IL_0017:  nop
    IL_0018:  stloc.0
    IL_0019:  ldloc.0
    IL_001a:  brtrue.s   IL_0025

    IL_001c:  nop
    IL_001d:  ldc.i4.1
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0023:  nop
    IL_0024:  nop
    IL_0025:  ldc.i4.2
    IL_0026:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_002b:  nop
    IL_002c:  ret
  } // end of method Switch::SingleIf4

  .method public hidebysig static void  NestedIf(int32 i) cil managed
  {
    // Code size       49 (0x31)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.1
    IL_0003:  ceq
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  brtrue.s   IL_002a

    IL_0009:  nop
    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.2
    IL_000c:  ceq
    IL_000e:  ldc.i4.0
    IL_000f:  ceq
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  brtrue.s   IL_001e

    IL_0015:  nop
    IL_0016:  ldc.i4.2
    IL_0017:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001c:  nop
    IL_001d:  nop
    IL_001e:  ldstr      "default"
    IL_0023:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0028:  nop
    IL_0029:  nop
    IL_002a:  call       void [mscorlib]System.Console::WriteLine()
    IL_002f:  nop
    IL_0030:  ret
  } // end of method Switch::NestedIf

  .method public hidebysig static void  IfChainWithCondition(int32 i) cil managed
  {
    // Code size       169 (0xa9)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  ceq
    IL_0005:  ldc.i4.0
    IL_0006:  ceq
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  brtrue.s   IL_001a

    IL_000c:  nop
    IL_000d:  ldc.i4.0
    IL_000e:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0013:  nop
    IL_0014:  nop
    IL_0015:  br         IL_00a2

    IL_001a:  ldarg.0
    IL_001b:  ldc.i4.1
    IL_001c:  ceq
    IL_001e:  ldc.i4.0
    IL_001f:  ceq
    IL_0021:  stloc.0
    IL_0022:  ldloc.0
    IL_0023:  brtrue.s   IL_0030

    IL_0025:  nop
    IL_0026:  ldc.i4.1
    IL_0027:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_002c:  nop
    IL_002d:  nop
    IL_002e:  br.s       IL_00a2

    IL_0030:  ldarg.0
    IL_0031:  ldc.i4.2
    IL_0032:  ceq
    IL_0034:  ldc.i4.0
    IL_0035:  ceq
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  brtrue.s   IL_0046

    IL_003b:  nop
    IL_003c:  ldc.i4.2
    IL_003d:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0042:  nop
    IL_0043:  nop
    IL_0044:  br.s       IL_00a2

    IL_0046:  ldarg.0
    IL_0047:  ldc.i4.3
    IL_0048:  ceq
    IL_004a:  ldc.i4.0
    IL_004b:  ceq
    IL_004d:  stloc.0
    IL_004e:  ldloc.0
    IL_004f:  brtrue.s   IL_005c

    IL_0051:  nop
    IL_0052:  ldc.i4.3
    IL_0053:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0058:  nop
    IL_0059:  nop
    IL_005a:  br.s       IL_00a2

    IL_005c:  ldarg.0
    IL_005d:  ldc.i4.4
    IL_005e:  ceq
    IL_0060:  ldc.i4.0
    IL_0061:  ceq
    IL_0063:  stloc.0
    IL_0064:  ldloc.0
    IL_0065:  brtrue.s   IL_0072

    IL_0067:  nop
    IL_0068:  ldc.i4.4
    IL_0069:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_006e:  nop
    IL_006f:  nop
    IL_0070:  br.s       IL_00a2

    IL_0072:  ldarg.0
    IL_0073:  ldc.i4.5
    IL_0074:  bne.un.s   IL_0080

    IL_0076:  call       bool [mscorlib]System.Console::get_CapsLock()
    IL_007b:  ldc.i4.0
    IL_007c:  ceq
    IL_007e:  br.s       IL_0081

    IL_0080:  ldc.i4.1
    IL_0081:  nop
    IL_0082:  stloc.0
    IL_0083:  ldloc.0
    IL_0084:  brtrue.s   IL_0095

    IL_0086:  nop
    IL_0087:  ldstr      "5A"
    IL_008c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0091:  nop
    IL_0092:  nop
    IL_0093:  br.s       IL_00a2

    IL_0095:  nop
    IL_0096:  ldstr      "default"
    IL_009b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00a0:  nop
    IL_00a1:  nop
    IL_00a2:  call       void [mscorlib]System.Console::WriteLine()
    IL_00a7:  nop
    IL_00a8:  ret
  } // end of method Switch::IfChainWithCondition

  .method public hidebysig static bool  SwitchlikeIf(int32 i,
                                                     int32 j) cil managed
  {
    // Code size       280 (0x118)
    .maxstack  2
    .locals init (bool V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  brfalse.s  IL_000a

    IL_0004:  ldarg.1
    IL_0005:  ldc.i4.0
    IL_0006:  ceq
    IL_0008:  br.s       IL_000b

    IL_000a:  ldc.i4.1
    IL_000b:  nop
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  brtrue     IL_0098

    IL_0013:  nop
    IL_0014:  ldarg.0
    IL_0015:  ldc.i4.m1
    IL_0016:  bne.un.s   IL_0021

    IL_0018:  ldarg.1
    IL_0019:  ldc.i4.m1
    IL_001a:  ceq
    IL_001c:  ldc.i4.0
    IL_001d:  ceq
    IL_001f:  br.s       IL_0022

    IL_0021:  ldc.i4.1
    IL_0022:  nop
    IL_0023:  stloc.1
    IL_0024:  ldloc.1
    IL_0025:  brtrue.s   IL_0034

    IL_0027:  nop
    IL_0028:  ldstr      "-1, -1"
    IL_002d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0032:  nop
    IL_0033:  nop
    IL_0034:  ldarg.0
    IL_0035:  ldc.i4.m1
    IL_0036:  bne.un.s   IL_0041

    IL_0038:  ldarg.1
    IL_0039:  ldc.i4.1
    IL_003a:  ceq
    IL_003c:  ldc.i4.0
    IL_003d:  ceq
    IL_003f:  br.s       IL_0042

    IL_0041:  ldc.i4.1
    IL_0042:  nop
    IL_0043:  stloc.1
    IL_0044:  ldloc.1
    IL_0045:  brtrue.s   IL_0054

    IL_0047:  nop
    IL_0048:  ldstr      "-1, 1"
    IL_004d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0052:  nop
    IL_0053:  nop
    IL_0054:  ldarg.0
    IL_0055:  ldc.i4.1
    IL_0056:  bne.un.s   IL_0061

    IL_0058:  ldarg.1
    IL_0059:  ldc.i4.m1
    IL_005a:  ceq
    IL_005c:  ldc.i4.0
    IL_005d:  ceq
    IL_005f:  br.s       IL_0062

    IL_0061:  ldc.i4.1
    IL_0062:  nop
    IL_0063:  stloc.1
    IL_0064:  ldloc.1
    IL_0065:  brtrue.s   IL_0074

    IL_0067:  nop
    IL_0068:  ldstr      "1, -1"
    IL_006d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0072:  nop
    IL_0073:  nop
    IL_0074:  ldarg.0
    IL_0075:  ldc.i4.1
    IL_0076:  bne.un.s   IL_0081

    IL_0078:  ldarg.1
    IL_0079:  ldc.i4.1
    IL_007a:  ceq
    IL_007c:  ldc.i4.0
    IL_007d:  ceq
    IL_007f:  br.s       IL_0082

    IL_0081:  ldc.i4.1
    IL_0082:  nop
    IL_0083:  stloc.1
    IL_0084:  ldloc.1
    IL_0085:  brtrue.s   IL_0094

    IL_0087:  nop
    IL_0088:  ldstr      "1, 1"
    IL_008d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0092:  nop
    IL_0093:  nop
    IL_0094:  ldc.i4.0
    IL_0095:  stloc.0
    IL_0096:  br.s       IL_0116

    IL_0098:  ldarg.0
    IL_0099:  ldc.i4.0
    IL_009a:  ceq
    IL_009c:  stloc.1
    IL_009d:  ldloc.1
    IL_009e:  brtrue.s   IL_00d5

    IL_00a0:  nop
    IL_00a1:  ldarg.0
    IL_00a2:  ldc.i4.m1
    IL_00a3:  ceq
    IL_00a5:  ldc.i4.0
    IL_00a6:  ceq
    IL_00a8:  stloc.1
    IL_00a9:  ldloc.1
    IL_00aa:  brtrue.s   IL_00b9

    IL_00ac:  nop
    IL_00ad:  ldstr      "-1, 0"
    IL_00b2:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00b7:  nop
    IL_00b8:  nop
    IL_00b9:  ldarg.0
    IL_00ba:  ldc.i4.1
    IL_00bb:  ceq
    IL_00bd:  ldc.i4.0
    IL_00be:  ceq
    IL_00c0:  stloc.1
    IL_00c1:  ldloc.1
    IL_00c2:  brtrue.s   IL_00d1

    IL_00c4:  nop
    IL_00c5:  ldstr      "1, 0"
    IL_00ca:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00cf:  nop
    IL_00d0:  nop
    IL_00d1:  ldc.i4.0
    IL_00d2:  stloc.0
    IL_00d3:  br.s       IL_0116

    IL_00d5:  ldarg.1
    IL_00d6:  ldc.i4.0
    IL_00d7:  ceq
    IL_00d9:  stloc.1
    IL_00da:  ldloc.1
    IL_00db:  brtrue.s   IL_0112

    IL_00dd:  nop
    IL_00de:  ldarg.1
    IL_00df:  ldc.i4.m1
    IL_00e0:  ceq
    IL_00e2:  ldc.i4.0
    IL_00e3:  ceq
    IL_00e5:  stloc.1
    IL_00e6:  ldloc.1
    IL_00e7:  brtrue.s   IL_00f6

    IL_00e9:  nop
    IL_00ea:  ldstr      "0, -1"
    IL_00ef:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00f4:  nop
    IL_00f5:  nop
    IL_00f6:  ldarg.1
    IL_00f7:  ldc.i4.1
    IL_00f8:  ceq
    IL_00fa:  ldc.i4.0
    IL_00fb:  ceq
    IL_00fd:  stloc.1
    IL_00fe:  ldloc.1
    IL_00ff:  brtrue.s   IL_010e

    IL_0101:  nop
    IL_0102:  ldstr      "0, 1"
    IL_0107:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_010c:  nop
    IL_010d:  nop
    IL_010e:  ldc.i4.0
    IL_010f:  stloc.0
    IL_0110:  br.s       IL_0116

    IL_0112:  ldc.i4.1
    IL_0113:  stloc.0
    IL_0114:  br.s       IL_0116

    IL_0116:  ldloc.0
    IL_0117:  ret
  } // end of method Switch::SwitchlikeIf

  .method public hidebysig static bool  SwitchlikeIf2(int32 i) cil managed
  {
    // Code size       80 (0x50)
    .maxstack  2
    .locals init (bool V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  ceq
    IL_0005:  stloc.1
    IL_0006:  ldloc.1
    IL_0007:  brtrue.s   IL_004a

    IL_0009:  nop
    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  ceq
    IL_000e:  ldc.i4.0
    IL_000f:  ceq
    IL_0011:  stloc.1
    IL_0012:  ldloc.1
    IL_0013:  brtrue.s   IL_001e

    IL_0015:  nop
    IL_0016:  ldc.i4.1
    IL_0017:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001c:  nop
    IL_001d:  nop
    IL_001e:  ldarg.0
    IL_001f:  ldc.i4.2
    IL_0020:  ceq
    IL_0022:  ldc.i4.0
    IL_0023:  ceq
    IL_0025:  stloc.1
    IL_0026:  ldloc.1
    IL_0027:  brtrue.s   IL_0032

    IL_0029:  nop
    IL_002a:  ldc.i4.2
    IL_002b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0030:  nop
    IL_0031:  nop
    IL_0032:  ldarg.0
    IL_0033:  ldc.i4.3
    IL_0034:  ceq
    IL_0036:  ldc.i4.0
    IL_0037:  ceq
    IL_0039:  stloc.1
    IL_003a:  ldloc.1
    IL_003b:  brtrue.s   IL_0046

    IL_003d:  nop
    IL_003e:  ldc.i4.3
    IL_003f:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0044:  nop
    IL_0045:  nop
    IL_0046:  ldc.i4.0
    IL_0047:  stloc.0
    IL_0048:  br.s       IL_004e

    IL_004a:  ldc.i4.0
    IL_004b:  stloc.0
    IL_004c:  br.s       IL_004e

    IL_004e:  ldloc.0
    IL_004f:  ret
  } // end of method Switch::SwitchlikeIf2

  .method public hidebysig static void  SingleIntervalIf(char c) cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.s   65
    IL_0004:  blt.s      IL_000d

    IL_0006:  ldarg.0
    IL_0007:  ldc.i4.s   90
    IL_0009:  cgt
    IL_000b:  br.s       IL_000e

    IL_000d:  ldc.i4.1
    IL_000e:  nop
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  brtrue.s   IL_0020

    IL_0013:  nop
    IL_0014:  ldstr      "alphabet"
    IL_0019:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001e:  nop
    IL_001f:  nop
    IL_0020:  ldstr      "end"
    IL_0025:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002a:  nop
    IL_002b:  ret
  } // end of method Switch::SingleIntervalIf

  .method public hidebysig static bool  Loop8(char c,
                                              bool b,
                                              class [mscorlib]System.Func`1<char> getChar) cil managed
  {
    // Code size       64 (0x40)
    .maxstack  2
    .locals init (bool V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.0
    IL_0003:  ceq
    IL_0005:  stloc.1
    IL_0006:  ldloc.1
    IL_0007:  brtrue.s   IL_003a

    IL_0009:  nop
    IL_000a:  br.s       IL_0016

    IL_000c:  nop
    IL_000d:  ldarg.2
    IL_000e:  callvirt   instance !0 class [mscorlib]System.Func`1<char>::Invoke()
    IL_0013:  starg.s    c
    IL_0015:  nop
    IL_0016:  ldarg.0
    IL_0017:  ldc.i4.s   97
    IL_0019:  blt.s      IL_0020

    IL_001b:  ldarg.0
    IL_001c:  ldc.i4.s   122
    IL_001e:  ble.s      IL_0033

    IL_0020:  ldarg.0
    IL_0021:  ldc.i4.s   65
    IL_0023:  blt.s      IL_002f

    IL_0025:  ldarg.0
    IL_0026:  ldc.i4.s   90
    IL_0028:  cgt
    IL_002a:  ldc.i4.0
    IL_002b:  ceq
    IL_002d:  br.s       IL_0030

    IL_002f:  ldc.i4.0
    IL_0030:  nop
    IL_0031:  br.s       IL_0034

    IL_0033:  ldc.i4.1
    IL_0034:  nop
    IL_0035:  stloc.1
    IL_0036:  ldloc.1
    IL_0037:  brtrue.s   IL_000c

    IL_0039:  nop
    IL_003a:  ldc.i4.1
    IL_003b:  stloc.0
    IL_003c:  br.s       IL_003e

    IL_003e:  ldloc.0
    IL_003f:  ret
  } // end of method Switch::Loop8

  .method public hidebysig static void  Loop9(class [mscorlib]System.Func`1<char> getChar) cil managed
  {
    // Code size       47 (0x2f)
    .maxstack  2
    .locals init (char V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.0
    IL_0003:  callvirt   instance !0 class [mscorlib]System.Func`1<char>::Invoke()
    IL_0008:  stloc.0
    IL_0009:  nop
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.m1
    IL_000c:  beq.s      IL_0028

    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.s   10
    IL_0011:  beq.s      IL_0028

    IL_0013:  ldloc.0
    IL_0014:  ldc.i4     0x2028
    IL_0019:  beq.s      IL_0028

    IL_001b:  ldloc.0
    IL_001c:  ldc.i4     0x2029
    IL_0021:  ceq
    IL_0023:  ldc.i4.0
    IL_0024:  ceq
    IL_0026:  br.s       IL_0029

    IL_0028:  ldc.i4.0
    IL_0029:  nop
    IL_002a:  stloc.1
    IL_002b:  ldloc.1
    IL_002c:  brtrue.s   IL_0001

    IL_002e:  ret
  } // end of method Switch::Loop9

  .method public hidebysig static void  SwitchWithBreakCase(int32 i,
                                                            bool b) cil managed
  {
    // Code size       78 (0x4e)
    .maxstack  2
    .locals init (bool V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.0
    IL_0003:  ceq
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  brtrue.s   IL_0042

    IL_0009:  nop
    IL_000a:  ldarg.0
    IL_000b:  stloc.1
    IL_000c:  ldloc.1
    IL_000d:  ldc.i4.1
    IL_000e:  sub
    IL_000f:  switch     ( 
                          IL_001e,
                          IL_0034)
    IL_001c:  br.s       IL_0027

    IL_001e:  ldc.i4.1
    IL_001f:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0024:  nop
    IL_0025:  br.s       IL_0036

    IL_0027:  ldstr      "default"
    IL_002c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0031:  nop
    IL_0032:  br.s       IL_0036

    IL_0034:  br.s       IL_0036

    IL_0036:  ldstr      "b"
    IL_003b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0040:  nop
    IL_0041:  nop
    IL_0042:  ldstr      "end"
    IL_0047:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004c:  nop
    IL_004d:  ret
  } // end of method Switch::SwitchWithBreakCase

  .method public hidebysig static void  SwitchWithReturnAndBreak(int32 i,
                                                                 bool b) cil managed
  {
    // Code size       49 (0x31)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  switch     ( 
                          IL_0013,
                          IL_0020)
    IL_0011:  br.s       IL_002a

    IL_0013:  ldarg.1
    IL_0014:  ldc.i4.0
    IL_0015:  ceq
    IL_0017:  stloc.1
    IL_0018:  ldloc.1
    IL_0019:  brtrue.s   IL_001e

    IL_001b:  nop
    IL_001c:  br.s       IL_0030

    IL_001e:  br.s       IL_002a

    IL_0020:  ldarg.1
    IL_0021:  stloc.1
    IL_0022:  ldloc.1
    IL_0023:  brtrue.s   IL_0028

    IL_0025:  nop
    IL_0026:  br.s       IL_0030

    IL_0028:  br.s       IL_002a

    IL_002a:  call       void [mscorlib]System.Console::WriteLine()
    IL_002f:  nop
    IL_0030:  ret
  } // end of method Switch::SwitchWithReturnAndBreak

  .method public hidebysig static int32  SwitchWithReturnAndBreak2(int32 i,
                                                                   bool b) cil managed
  {
    // Code size       101 (0x65)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldloc.1
    IL_0004:  ldc.i4     0x14e
    IL_0009:  bgt.s      IL_001e

    IL_000b:  ldloc.1
    IL_000c:  ldc.i4.4
    IL_000d:  beq.s      IL_0038

    IL_000f:  ldloc.1
    IL_0010:  ldc.i4.s   33
    IL_0012:  beq.s      IL_0038

    IL_0014:  ldloc.1
    IL_0015:  ldc.i4     0x14e
    IL_001a:  beq.s      IL_0042

    IL_001c:  br.s       IL_0059

    IL_001e:  ldloc.1
    IL_001f:  ldc.i4     0x18b
    IL_0024:  beq.s      IL_0051

    IL_0026:  ldloc.1
    IL_0027:  ldc.i4     0x19a
    IL_002c:  beq.s      IL_0051

    IL_002e:  ldloc.1
    IL_002f:  ldc.i4     0x1c7
    IL_0034:  beq.s      IL_0051

    IL_0036:  br.s       IL_0059

    IL_0038:  call       void [mscorlib]System.Console::WriteLine()
    IL_003d:  nop
    IL_003e:  ldc.i4.1
    IL_003f:  stloc.0
    IL_0040:  br.s       IL_0063

    IL_0042:  ldarg.1
    IL_0043:  ldc.i4.0
    IL_0044:  ceq
    IL_0046:  stloc.2
    IL_0047:  ldloc.2
    IL_0048:  brtrue.s   IL_004f

    IL_004a:  nop
    IL_004b:  ldc.i4.2
    IL_004c:  stloc.0
    IL_004d:  br.s       IL_0063

    IL_004f:  br.s       IL_0059

    IL_0051:  call       void [mscorlib]System.Console::WriteLine()
    IL_0056:  nop
    IL_0057:  br.s       IL_0059

    IL_0059:  call       void [mscorlib]System.Console::WriteLine()
    IL_005e:  nop
    IL_005f:  ldc.i4.0
    IL_0060:  stloc.0
    IL_0061:  br.s       IL_0063

    IL_0063:  ldloc.0
    IL_0064:  ret
  } // end of method Switch::SwitchWithReturnAndBreak2

  .method public hidebysig static void  SwitchWithReturnAndBreak3(int32 i) cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  switch     ( 
                          IL_0015,
                          IL_001e)
    IL_0011:  br.s       IL_0013

    IL_0013:  br.s       IL_002d

    IL_0015:  ldc.i4.0
    IL_0016:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001b:  nop
    IL_001c:  br.s       IL_0027

    IL_001e:  ldc.i4.1
    IL_001f:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0024:  nop
    IL_0025:  br.s       IL_0027

    IL_0027:  call       void [mscorlib]System.Console::WriteLine()
    IL_002c:  nop
    IL_002d:  ret
  } // end of method Switch::SwitchWithReturnAndBreak3

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch

.class private auto ansi '<PrivateImplementationDetails>'
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field static assembly class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '$$method0x6000010-1'
  .field static assembly class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '$$method0x6000011-1'
  .field static assembly class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '$$method0x6000015-1'
  .field static assembly class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '$$method0x6000016-1'
  .field static assembly class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '$$method0x6000018-1'
  .field static assembly class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '$$method0x6000019-1'
} // end of class '<PrivateImplementationDetails>'


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
