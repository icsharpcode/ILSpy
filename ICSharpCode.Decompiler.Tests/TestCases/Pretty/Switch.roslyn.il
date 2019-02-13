



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
    .locals init (valuetype [mscorlib]System.Nullable`1<bool> V_0,
             valuetype [mscorlib]System.Nullable`1<bool> V_1,
             bool V_2,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State V_3)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldloc.1
    IL_0004:  stloc.0
    IL_0005:  ldloca.s   V_0
    IL_0007:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_000c:  brfalse.s  IL_0029

    IL_000e:  ldloca.s   V_0
    IL_0010:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0015:  stloc.2
    IL_0016:  ldloc.2
    IL_0017:  brfalse.s  IL_0021

    IL_0019:  br.s       IL_001b

    IL_001b:  ldloc.2
    IL_001c:  ldc.i4.1
    IL_001d:  beq.s      IL_0025

    IL_001f:  br.s       IL_002d

    IL_0021:  ldc.i4.0
    IL_0022:  stloc.3
    IL_0023:  br.s       IL_0033

    IL_0025:  ldc.i4.1
    IL_0026:  stloc.3
    IL_0027:  br.s       IL_0033

    IL_0029:  ldc.i4.2
    IL_002a:  stloc.3
    IL_002b:  br.s       IL_0033

    IL_002d:  newobj     instance void [mscorlib]System.InvalidOperationException::.ctor()
    IL_0032:  throw

    IL_0033:  ldloc.3
    IL_0034:  ret
  } // end of method Switch::SwitchOverNullableBool

  .method public hidebysig static valuetype [mscorlib]System.Nullable`1<bool> 
          SwitchOverNullableEnum(valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State> state) cil managed
  {
    // Code size       81 (0x51)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State> V_0,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State> V_1,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State V_2,
             valuetype [mscorlib]System.Nullable`1<bool> V_3,
             valuetype [mscorlib]System.Nullable`1<bool> V_4)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.1
    IL_0003:  ldloc.1
    IL_0004:  stloc.0
    IL_0005:  ldloca.s   V_0
    IL_0007:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State>::get_HasValue()
    IL_000c:  brfalse.s  IL_0049

    IL_000e:  ldloca.s   V_0
    IL_0010:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State>::GetValueOrDefault()
    IL_0015:  stloc.2
    IL_0016:  ldloc.2
    IL_0017:  switch     ( 
                          IL_002a,
                          IL_0033,
                          IL_003c)
    IL_0028:  br.s       IL_0049

    IL_002a:  ldc.i4.0
    IL_002b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0030:  stloc.3
    IL_0031:  br.s       IL_004f

    IL_0033:  ldc.i4.1
    IL_0034:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0039:  stloc.3
    IL_003a:  br.s       IL_004f

    IL_003c:  ldloca.s   V_4
    IL_003e:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_0044:  ldloc.s    V_4
    IL_0046:  stloc.3
    IL_0047:  br.s       IL_004f

    IL_0049:  newobj     instance void [mscorlib]System.InvalidOperationException::.ctor()
    IL_004e:  throw

    IL_004f:  ldloc.3
    IL_0050:  ret
  } // end of method Switch::SwitchOverNullableEnum

  .method public hidebysig static string 
          SparseIntegerSwitch(int32 i) cil managed
  {
    // Code size       226 (0xe2)
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
    IL_002a:  beq.s      IL_0088

    IL_002c:  br.s       IL_002e

    IL_002e:  ldloc.0
    IL_002f:  ldc.i4.m1
    IL_0030:  sub
    IL_0031:  switch     ( 
                          IL_0090,
                          IL_0098,
                          IL_00a0,
                          IL_00a8,
                          IL_00d8,
                          IL_00b0)
    IL_004e:  br         IL_00d8

    IL_0053:  ldloc.0
    IL_0054:  ldc.i4     0x2710
    IL_0059:  bgt.s      IL_006c

    IL_005b:  ldloc.0
    IL_005c:  ldc.i4.s   100
    IL_005e:  beq.s      IL_00b8

    IL_0060:  br.s       IL_0062

    IL_0062:  ldloc.0
    IL_0063:  ldc.i4     0x2710
    IL_0068:  beq.s      IL_00c0

    IL_006a:  br.s       IL_00d8

    IL_006c:  ldloc.0
    IL_006d:  ldc.i4     0x2711
    IL_0072:  beq.s      IL_00c8

    IL_0074:  br.s       IL_0076

    IL_0076:  ldloc.0
    IL_0077:  ldc.i4     0x7fffffff
    IL_007c:  beq.s      IL_00d0

    IL_007e:  br.s       IL_00d8

    IL_0080:  ldstr      "-10 mln"
    IL_0085:  stloc.1
    IL_0086:  br.s       IL_00e0

    IL_0088:  ldstr      "-hundred"
    IL_008d:  stloc.1
    IL_008e:  br.s       IL_00e0

    IL_0090:  ldstr      "-1"
    IL_0095:  stloc.1
    IL_0096:  br.s       IL_00e0

    IL_0098:  ldstr      "0"
    IL_009d:  stloc.1
    IL_009e:  br.s       IL_00e0

    IL_00a0:  ldstr      "1"
    IL_00a5:  stloc.1
    IL_00a6:  br.s       IL_00e0

    IL_00a8:  ldstr      "2"
    IL_00ad:  stloc.1
    IL_00ae:  br.s       IL_00e0

    IL_00b0:  ldstr      "4"
    IL_00b5:  stloc.1
    IL_00b6:  br.s       IL_00e0

    IL_00b8:  ldstr      "hundred"
    IL_00bd:  stloc.1
    IL_00be:  br.s       IL_00e0

    IL_00c0:  ldstr      "ten thousand"
    IL_00c5:  stloc.1
    IL_00c6:  br.s       IL_00e0

    IL_00c8:  ldstr      "ten thousand and one"
    IL_00cd:  stloc.1
    IL_00ce:  br.s       IL_00e0

    IL_00d0:  ldstr      "int.MaxValue"
    IL_00d5:  stloc.1
    IL_00d6:  br.s       IL_00e0

    IL_00d8:  ldstr      "something else"
    IL_00dd:  stloc.1
    IL_00de:  br.s       IL_00e0

    IL_00e0:  ldloc.1
    IL_00e1:  ret
  } // end of method Switch::SparseIntegerSwitch

  .method public hidebysig static void  SparseIntegerSwitch2(int32 i) cil managed
  {
    // Code size       86 (0x56)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  ldc.i4.s   21
    IL_0006:  bgt.s      IL_002a

    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.s   11
    IL_000b:  bgt.s      IL_001c

    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.4
    IL_000f:  beq.s      IL_004d

    IL_0011:  br.s       IL_0013

    IL_0013:  ldloc.0
    IL_0014:  ldc.i4.s   10
    IL_0016:  sub
    IL_0017:  ldc.i4.1
    IL_0018:  ble.un.s   IL_004d

    IL_001a:  br.s       IL_0055

    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.s   13
    IL_001f:  beq.s      IL_004d

    IL_0021:  br.s       IL_0023

    IL_0023:  ldloc.0
    IL_0024:  ldc.i4.s   21
    IL_0026:  beq.s      IL_004d

    IL_0028:  br.s       IL_0055

    IL_002a:  ldloc.0
    IL_002b:  ldc.i4.s   33
    IL_002d:  bgt.s      IL_003d

    IL_002f:  ldloc.0
    IL_0030:  ldc.i4.s   29
    IL_0032:  beq.s      IL_004d

    IL_0034:  br.s       IL_0036

    IL_0036:  ldloc.0
    IL_0037:  ldc.i4.s   33
    IL_0039:  beq.s      IL_004d

    IL_003b:  br.s       IL_0055

    IL_003d:  ldloc.0
    IL_003e:  ldc.i4.s   49
    IL_0040:  sub
    IL_0041:  ldc.i4.1
    IL_0042:  ble.un.s   IL_004d

    IL_0044:  br.s       IL_0046

    IL_0046:  ldloc.0
    IL_0047:  ldc.i4.s   55
    IL_0049:  beq.s      IL_004d

    IL_004b:  br.s       IL_0055

    IL_004d:  call       void [mscorlib]System.Console::WriteLine()
    IL_0052:  nop
    IL_0053:  br.s       IL_0055

    IL_0055:  ret
  } // end of method Switch::SparseIntegerSwitch2

  .method public hidebysig static bool  SparseIntegerSwitch3(int32 i) cil managed
  {
    // Code size       51 (0x33)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  ldc.i4.s   12
    IL_0006:  bgt.s      IL_0016

    IL_0008:  ldloc.0
    IL_0009:  brfalse.s  IL_0029

    IL_000b:  br.s       IL_000d

    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.s   10
    IL_0010:  sub
    IL_0011:  ldc.i4.2
    IL_0012:  ble.un.s   IL_0029

    IL_0014:  br.s       IL_002d

    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.s   100
    IL_0019:  sub
    IL_001a:  ldc.i4.1
    IL_001b:  ble.un.s   IL_0029

    IL_001d:  br.s       IL_001f

    IL_001f:  ldloc.0
    IL_0020:  ldc.i4     0xc8
    IL_0025:  beq.s      IL_0029

    IL_0027:  br.s       IL_002d

    IL_0029:  ldc.i4.1
    IL_002a:  stloc.1
    IL_002b:  br.s       IL_0031

    IL_002d:  ldc.i4.0
    IL_002e:  stloc.1
    IL_002f:  br.s       IL_0031

    IL_0031:  ldloc.1
    IL_0032:  ret
  } // end of method Switch::SparseIntegerSwitch3

  .method public hidebysig static string 
          SwitchOverNullableInt(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       82 (0x52)
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
    IL_0017:  brfalse.s  IL_0030

    IL_0019:  br.s       IL_001b

    IL_001b:  ldloc.2
    IL_001c:  ldc.i4.5
    IL_001d:  beq.s      IL_0038

    IL_001f:  br.s       IL_0021

    IL_0021:  ldloc.2
    IL_0022:  ldc.i4.s   10
    IL_0024:  beq.s      IL_0040

    IL_0026:  br.s       IL_0048

    IL_0028:  ldstr      "null"
    IL_002d:  stloc.3
    IL_002e:  br.s       IL_0050

    IL_0030:  ldstr      "zero"
    IL_0035:  stloc.3
    IL_0036:  br.s       IL_0050

    IL_0038:  ldstr      "five"
    IL_003d:  stloc.3
    IL_003e:  br.s       IL_0050

    IL_0040:  ldstr      "ten"
    IL_0045:  stloc.3
    IL_0046:  br.s       IL_0050

    IL_0048:  ldstr      "large"
    IL_004d:  stloc.3
    IL_004e:  br.s       IL_0050

    IL_0050:  ldloc.3
    IL_0051:  ret
  } // end of method Switch::SwitchOverNullableInt

  .method public hidebysig static string 
          SwitchOverNullableIntNullCaseCombined(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       74 (0x4a)
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
    IL_0017:  brfalse.s  IL_0028

    IL_0019:  br.s       IL_001b

    IL_001b:  ldloc.2
    IL_001c:  ldc.i4.5
    IL_001d:  beq.s      IL_0030

    IL_001f:  br.s       IL_0021

    IL_0021:  ldloc.2
    IL_0022:  ldc.i4.s   10
    IL_0024:  beq.s      IL_0038

    IL_0026:  br.s       IL_0040

    IL_0028:  ldstr      "zero"
    IL_002d:  stloc.3
    IL_002e:  br.s       IL_0048

    IL_0030:  ldstr      "five"
    IL_0035:  stloc.3
    IL_0036:  br.s       IL_0048

    IL_0038:  ldstr      "ten"
    IL_003d:  stloc.3
    IL_003e:  br.s       IL_0048

    IL_0040:  ldstr      "large"
    IL_0045:  stloc.3
    IL_0046:  br.s       IL_0048

    IL_0048:  ldloc.3
    IL_0049:  ret
  } // end of method Switch::SwitchOverNullableIntNullCaseCombined

  .method public hidebysig static string 
          SwitchOverNullableIntShifted(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       127 (0x7f)
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
    IL_003c:  brfalse.s  IL_0058

    IL_003e:  br.s       IL_0040

    IL_0040:  ldloc.s    V_4
    IL_0042:  ldc.i4.5
    IL_0043:  beq.s      IL_0061

    IL_0045:  br.s       IL_0047

    IL_0047:  ldloc.s    V_4
    IL_0049:  ldc.i4.s   10
    IL_004b:  beq.s      IL_006a

    IL_004d:  br.s       IL_0073

    IL_004f:  ldstr      "null"
    IL_0054:  stloc.s    V_5
    IL_0056:  br.s       IL_007c

    IL_0058:  ldstr      "zero"
    IL_005d:  stloc.s    V_5
    IL_005f:  br.s       IL_007c

    IL_0061:  ldstr      "five"
    IL_0066:  stloc.s    V_5
    IL_0068:  br.s       IL_007c

    IL_006a:  ldstr      "ten"
    IL_006f:  stloc.s    V_5
    IL_0071:  br.s       IL_007c

    IL_0073:  ldstr      "large"
    IL_0078:  stloc.s    V_5
    IL_007a:  br.s       IL_007c

    IL_007c:  ldloc.s    V_5
    IL_007e:  ret
  } // end of method Switch::SwitchOverNullableIntShifted

  .method public hidebysig static string 
          SwitchOverNullableIntShiftedNullCaseCombined(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       118 (0x76)
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
    IL_003c:  brfalse.s  IL_004f

    IL_003e:  br.s       IL_0040

    IL_0040:  ldloc.s    V_4
    IL_0042:  ldc.i4.5
    IL_0043:  beq.s      IL_0058

    IL_0045:  br.s       IL_0047

    IL_0047:  ldloc.s    V_4
    IL_0049:  ldc.i4.s   10
    IL_004b:  beq.s      IL_0061

    IL_004d:  br.s       IL_006a

    IL_004f:  ldstr      "zero"
    IL_0054:  stloc.s    V_5
    IL_0056:  br.s       IL_0073

    IL_0058:  ldstr      "five"
    IL_005d:  stloc.s    V_5
    IL_005f:  br.s       IL_0073

    IL_0061:  ldstr      "ten"
    IL_0066:  stloc.s    V_5
    IL_0068:  br.s       IL_0073

    IL_006a:  ldstr      "large"
    IL_006f:  stloc.s    V_5
    IL_0071:  br.s       IL_0073

    IL_0073:  ldloc.s    V_5
    IL_0075:  ret
  } // end of method Switch::SwitchOverNullableIntShiftedNullCaseCombined

  .method public hidebysig static string 
          SwitchOverNullableIntNoNullCase(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       74 (0x4a)
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
    IL_000c:  brfalse.s  IL_0040

    IL_000e:  ldloca.s   V_0
    IL_0010:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0015:  stloc.2
    IL_0016:  ldloc.2
    IL_0017:  brfalse.s  IL_0028

    IL_0019:  br.s       IL_001b

    IL_001b:  ldloc.2
    IL_001c:  ldc.i4.5
    IL_001d:  beq.s      IL_0030

    IL_001f:  br.s       IL_0021

    IL_0021:  ldloc.2
    IL_0022:  ldc.i4.s   10
    IL_0024:  beq.s      IL_0038

    IL_0026:  br.s       IL_0040

    IL_0028:  ldstr      "zero"
    IL_002d:  stloc.3
    IL_002e:  br.s       IL_0048

    IL_0030:  ldstr      "five"
    IL_0035:  stloc.3
    IL_0036:  br.s       IL_0048

    IL_0038:  ldstr      "ten"
    IL_003d:  stloc.3
    IL_003e:  br.s       IL_0048

    IL_0040:  ldstr      "other"
    IL_0045:  stloc.3
    IL_0046:  br.s       IL_0048

    IL_0048:  ldloc.3
    IL_0049:  ret
  } // end of method Switch::SwitchOverNullableIntNoNullCase

  .method public hidebysig static string 
          SwitchOverNullableIntNoNullCaseShifted(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       118 (0x76)
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
    IL_002f:  brfalse.s  IL_006a

    IL_0031:  ldloca.s   V_0
    IL_0033:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0038:  stloc.s    V_4
    IL_003a:  ldloc.s    V_4
    IL_003c:  brfalse.s  IL_004f

    IL_003e:  br.s       IL_0040

    IL_0040:  ldloc.s    V_4
    IL_0042:  ldc.i4.5
    IL_0043:  beq.s      IL_0058

    IL_0045:  br.s       IL_0047

    IL_0047:  ldloc.s    V_4
    IL_0049:  ldc.i4.s   10
    IL_004b:  beq.s      IL_0061

    IL_004d:  br.s       IL_006a

    IL_004f:  ldstr      "zero"
    IL_0054:  stloc.s    V_5
    IL_0056:  br.s       IL_0073

    IL_0058:  ldstr      "five"
    IL_005d:  stloc.s    V_5
    IL_005f:  br.s       IL_0073

    IL_0061:  ldstr      "ten"
    IL_0066:  stloc.s    V_5
    IL_0068:  br.s       IL_0073

    IL_006a:  ldstr      "other"
    IL_006f:  stloc.s    V_5
    IL_0071:  br.s       IL_0073

    IL_0073:  ldloc.s    V_5
    IL_0075:  ret
  } // end of method Switch::SwitchOverNullableIntNoNullCaseShifted

  .method public hidebysig static void  SwitchOverInt(int32 i) cil managed
  {
    // Code size       151 (0x97)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  ldc.i4.s   10
    IL_0006:  bgt.s      IL_001a

    IL_0008:  ldloc.0
    IL_0009:  brfalse.s  IL_003b

    IL_000b:  br.s       IL_000d

    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.5
    IL_000f:  beq.s      IL_0048

    IL_0011:  br.s       IL_0013

    IL_0013:  ldloc.0
    IL_0014:  ldc.i4.s   10
    IL_0016:  beq.s      IL_0055

    IL_0018:  br.s       IL_0096

    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.s   20
    IL_001d:  bgt.s      IL_002d

    IL_001f:  ldloc.0
    IL_0020:  ldc.i4.s   15
    IL_0022:  beq.s      IL_0062

    IL_0024:  br.s       IL_0026

    IL_0026:  ldloc.0
    IL_0027:  ldc.i4.s   20
    IL_0029:  beq.s      IL_006f

    IL_002b:  br.s       IL_0096

    IL_002d:  ldloc.0
    IL_002e:  ldc.i4.s   25
    IL_0030:  beq.s      IL_007c

    IL_0032:  br.s       IL_0034

    IL_0034:  ldloc.0
    IL_0035:  ldc.i4.s   30
    IL_0037:  beq.s      IL_0089

    IL_0039:  br.s       IL_0096

    IL_003b:  ldstr      "zero"
    IL_0040:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0045:  nop
    IL_0046:  br.s       IL_0096

    IL_0048:  ldstr      "five"
    IL_004d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0052:  nop
    IL_0053:  br.s       IL_0096

    IL_0055:  ldstr      "ten"
    IL_005a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005f:  nop
    IL_0060:  br.s       IL_0096

    IL_0062:  ldstr      "fifteen"
    IL_0067:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006c:  nop
    IL_006d:  br.s       IL_0096

    IL_006f:  ldstr      "twenty"
    IL_0074:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0079:  nop
    IL_007a:  br.s       IL_0096

    IL_007c:  ldstr      "twenty-five"
    IL_0081:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0086:  nop
    IL_0087:  br.s       IL_0096

    IL_0089:  ldstr      "thirty"
    IL_008e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0093:  nop
    IL_0094:  br.s       IL_0096

    IL_0096:  ret
  } // end of method Switch::SwitchOverInt

  .method public hidebysig static void  CompactSwitchOverInt(int32 i) cil managed
  {
    // Code size       66 (0x42)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  ldc.i4.2
    IL_0005:  ble.un.s   IL_000f

    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ldc.i4.3
    IL_000b:  beq.s      IL_001c

    IL_000d:  br.s       IL_0029

    IL_000f:  ldstr      "012"
    IL_0014:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0019:  nop
    IL_001a:  br.s       IL_0036

    IL_001c:  ldstr      "3"
    IL_0021:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0026:  nop
    IL_0027:  br.s       IL_0036

    IL_0029:  ldstr      "default"
    IL_002e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0033:  nop
    IL_0034:  br.s       IL_0036

    IL_0036:  ldstr      "end"
    IL_003b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0040:  nop
    IL_0041:  ret
  } // end of method Switch::CompactSwitchOverInt

  .method public hidebysig static string 
          ShortSwitchOverString(string text) cil managed
  {
    // Code size       95 (0x5f)
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
    IL_002c:  brtrue.s   IL_0045

    IL_002e:  ldloc.0
    IL_002f:  ldstr      "Third case"
    IL_0034:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0039:  brtrue.s   IL_004d

    IL_003b:  br.s       IL_0055

    IL_003d:  ldstr      "Text1"
    IL_0042:  stloc.1
    IL_0043:  br.s       IL_005d

    IL_0045:  ldstr      "Text2"
    IL_004a:  stloc.1
    IL_004b:  br.s       IL_005d

    IL_004d:  ldstr      "Text3"
    IL_0052:  stloc.1
    IL_0053:  br.s       IL_005d

    IL_0055:  ldstr      "Default"
    IL_005a:  stloc.1
    IL_005b:  br.s       IL_005d

    IL_005d:  ldloc.1
    IL_005e:  ret
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
    IL_0013:  stloc.0
    IL_0014:  ldloc.0
    IL_0015:  ldstr      "First case"
    IL_001a:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_001f:  brtrue.s   IL_0033

    IL_0021:  ldloc.0
    IL_0022:  ldstr      "Second case"
    IL_0027:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_002c:  brtrue.s   IL_003b

    IL_002e:  ldloc.0
    IL_002f:  brfalse.s  IL_0043

    IL_0031:  br.s       IL_004b

    IL_0033:  ldstr      "Text1"
    IL_0038:  stloc.1
    IL_0039:  br.s       IL_0053

    IL_003b:  ldstr      "Text2"
    IL_0040:  stloc.1
    IL_0041:  br.s       IL_0053

    IL_0043:  ldstr      "null"
    IL_0048:  stloc.1
    IL_0049:  br.s       IL_0053

    IL_004b:  ldstr      "Default"
    IL_0050:  stloc.1
    IL_0051:  br.s       IL_0053

    IL_0053:  ldloc.1
    IL_0054:  ret
  } // end of method Switch::ShortSwitchOverStringWithNullCase

  .method public hidebysig static string 
          SwitchOverString1(string text) cil managed
  {
    // Code size       325 (0x145)
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

    IL_003e:  br         IL_013b

    IL_0043:  ldloc.1
    IL_0044:  ldc.i4     0xc9a8f4f
    IL_0049:  beq.s      IL_0093

    IL_004b:  br.s       IL_004d

    IL_004d:  ldloc.1
    IL_004e:  ldc.i4     0xf3d44a6
    IL_0053:  beq.s      IL_00c6

    IL_0055:  br         IL_013b

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

    IL_0077:  br         IL_013b

    IL_007c:  ldloc.1
    IL_007d:  ldc.i4     0xea3d096b
    IL_0082:  beq.s      IL_00a5

    IL_0084:  br.s       IL_0086

    IL_0086:  ldloc.1
    IL_0087:  ldc.i4     0xf701cc7f
    IL_008c:  beq.s      IL_00d5

    IL_008e:  br         IL_013b

    IL_0093:  ldloc.0
    IL_0094:  ldstr      "First case"
    IL_0099:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_009e:  brtrue.s   IL_0107

    IL_00a0:  br         IL_013b

    IL_00a5:  ldloc.0
    IL_00a6:  ldstr      "Second case"
    IL_00ab:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00b0:  brtrue.s   IL_010f

    IL_00b2:  br         IL_013b

    IL_00b7:  ldloc.0
    IL_00b8:  ldstr      "2nd case"
    IL_00bd:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00c2:  brtrue.s   IL_010f

    IL_00c4:  br.s       IL_013b

    IL_00c6:  ldloc.0
    IL_00c7:  ldstr      "Third case"
    IL_00cc:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00d1:  brtrue.s   IL_0117

    IL_00d3:  br.s       IL_013b

    IL_00d5:  ldloc.0
    IL_00d6:  ldstr      "Fourth case"
    IL_00db:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00e0:  brtrue.s   IL_011f

    IL_00e2:  br.s       IL_013b

    IL_00e4:  ldloc.0
    IL_00e5:  ldstr      "Fifth case"
    IL_00ea:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00ef:  brtrue.s   IL_0127

    IL_00f1:  br.s       IL_013b

    IL_00f3:  ldloc.0
    IL_00f4:  ldstr      "Sixth case"
    IL_00f9:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00fe:  brtrue.s   IL_012f

    IL_0100:  br.s       IL_013b

    IL_0102:  ldloc.0
    IL_0103:  brfalse.s  IL_0137

    IL_0105:  br.s       IL_013b

    IL_0107:  ldstr      "Text1"
    IL_010c:  stloc.2
    IL_010d:  br.s       IL_0143

    IL_010f:  ldstr      "Text2"
    IL_0114:  stloc.2
    IL_0115:  br.s       IL_0143

    IL_0117:  ldstr      "Text3"
    IL_011c:  stloc.2
    IL_011d:  br.s       IL_0143

    IL_011f:  ldstr      "Text4"
    IL_0124:  stloc.2
    IL_0125:  br.s       IL_0143

    IL_0127:  ldstr      "Text5"
    IL_012c:  stloc.2
    IL_012d:  br.s       IL_0143

    IL_012f:  ldstr      "Text6"
    IL_0134:  stloc.2
    IL_0135:  br.s       IL_0143

    IL_0137:  ldnull
    IL_0138:  stloc.2
    IL_0139:  br.s       IL_0143

    IL_013b:  ldstr      "Default"
    IL_0140:  stloc.2
    IL_0141:  br.s       IL_0143

    IL_0143:  ldloc.2
    IL_0144:  ret
  } // end of method Switch::SwitchOverString1

  .method public hidebysig static string 
          SwitchOverString2() cil managed
  {
    // Code size       500 (0x1f4)
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

    IL_0041:  br         IL_01ea

    IL_0046:  ldloc.1
    IL_0047:  ldc.i4     0xf3d44a6
    IL_004c:  beq        IL_00f0

    IL_0051:  br.s       IL_0053

    IL_0053:  ldloc.1
    IL_0054:  ldc.i4     0x20289804
    IL_0059:  beq        IL_0153

    IL_005e:  br.s       IL_0060

    IL_0060:  ldloc.1
    IL_0061:  ldc.i4     0x4c7c71f6
    IL_0066:  beq        IL_0165

    IL_006b:  br         IL_01ea

    IL_0070:  ldloc.1
    IL_0071:  ldc.i4     0xa151b28a
    IL_0076:  bgt.un.s   IL_00a2

    IL_0078:  ldloc.1
    IL_0079:  ldc.i4     0x4d0cea48
    IL_007e:  beq        IL_0183

    IL_0083:  br.s       IL_0085

    IL_0085:  ldloc.1
    IL_0086:  ldc.i4     0x51650fb9
    IL_008b:  beq        IL_012f

    IL_0090:  br.s       IL_0092

    IL_0092:  ldloc.1
    IL_0093:  ldc.i4     0xa151b28a
    IL_0098:  beq        IL_0141

    IL_009d:  br         IL_01ea

    IL_00a2:  ldloc.1
    IL_00a3:  ldc.i4     0xea3d096b
    IL_00a8:  beq.s      IL_00db

    IL_00aa:  br.s       IL_00ac

    IL_00ac:  ldloc.1
    IL_00ad:  ldc.i4     0xed5134d4
    IL_00b2:  beq        IL_0174

    IL_00b7:  br.s       IL_00b9

    IL_00b9:  ldloc.1
    IL_00ba:  ldc.i4     0xf701cc7f
    IL_00bf:  beq.s      IL_0105

    IL_00c1:  br         IL_01ea

    IL_00c6:  ldloc.0
    IL_00c7:  ldstr      "First case"
    IL_00cc:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00d1:  brtrue     IL_0192

    IL_00d6:  br         IL_01ea

    IL_00db:  ldloc.0
    IL_00dc:  ldstr      "Second case"
    IL_00e1:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00e6:  brtrue     IL_019a

    IL_00eb:  br         IL_01ea

    IL_00f0:  ldloc.0
    IL_00f1:  ldstr      "Third case"
    IL_00f6:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00fb:  brtrue     IL_01a2

    IL_0100:  br         IL_01ea

    IL_0105:  ldloc.0
    IL_0106:  ldstr      "Fourth case"
    IL_010b:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0110:  brtrue     IL_01aa

    IL_0115:  br         IL_01ea

    IL_011a:  ldloc.0
    IL_011b:  ldstr      "Fifth case"
    IL_0120:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0125:  brtrue     IL_01b2

    IL_012a:  br         IL_01ea

    IL_012f:  ldloc.0
    IL_0130:  ldstr      "Sixth case"
    IL_0135:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_013a:  brtrue.s   IL_01ba

    IL_013c:  br         IL_01ea

    IL_0141:  ldloc.0
    IL_0142:  ldstr      "Seventh case"
    IL_0147:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_014c:  brtrue.s   IL_01c2

    IL_014e:  br         IL_01ea

    IL_0153:  ldloc.0
    IL_0154:  ldstr      "Eighth case"
    IL_0159:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_015e:  brtrue.s   IL_01ca

    IL_0160:  br         IL_01ea

    IL_0165:  ldloc.0
    IL_0166:  ldstr      "Ninth case"
    IL_016b:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0170:  brtrue.s   IL_01d2

    IL_0172:  br.s       IL_01ea

    IL_0174:  ldloc.0
    IL_0175:  ldstr      "Tenth case"
    IL_017a:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_017f:  brtrue.s   IL_01da

    IL_0181:  br.s       IL_01ea

    IL_0183:  ldloc.0
    IL_0184:  ldstr      "Eleventh case"
    IL_0189:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_018e:  brtrue.s   IL_01e2

    IL_0190:  br.s       IL_01ea

    IL_0192:  ldstr      "Text1"
    IL_0197:  stloc.2
    IL_0198:  br.s       IL_01f2

    IL_019a:  ldstr      "Text2"
    IL_019f:  stloc.2
    IL_01a0:  br.s       IL_01f2

    IL_01a2:  ldstr      "Text3"
    IL_01a7:  stloc.2
    IL_01a8:  br.s       IL_01f2

    IL_01aa:  ldstr      "Text4"
    IL_01af:  stloc.2
    IL_01b0:  br.s       IL_01f2

    IL_01b2:  ldstr      "Text5"
    IL_01b7:  stloc.2
    IL_01b8:  br.s       IL_01f2

    IL_01ba:  ldstr      "Text6"
    IL_01bf:  stloc.2
    IL_01c0:  br.s       IL_01f2

    IL_01c2:  ldstr      "Text7"
    IL_01c7:  stloc.2
    IL_01c8:  br.s       IL_01f2

    IL_01ca:  ldstr      "Text8"
    IL_01cf:  stloc.2
    IL_01d0:  br.s       IL_01f2

    IL_01d2:  ldstr      "Text9"
    IL_01d7:  stloc.2
    IL_01d8:  br.s       IL_01f2

    IL_01da:  ldstr      "Text10"
    IL_01df:  stloc.2
    IL_01e0:  br.s       IL_01f2

    IL_01e2:  ldstr      "Text11"
    IL_01e7:  stloc.2
    IL_01e8:  br.s       IL_01f2

    IL_01ea:  ldstr      "Default"
    IL_01ef:  stloc.2
    IL_01f0:  br.s       IL_01f2

    IL_01f2:  ldloc.2
    IL_01f3:  ret
  } // end of method Switch::SwitchOverString2

  .method public hidebysig static string 
          SwitchOverBool(bool b) cil managed
  {
    // Code size       59 (0x3b)
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
    IL_001b:  brfalse.s  IL_002d

    IL_001d:  br.s       IL_001f

    IL_001f:  ldloc.0
    IL_0020:  ldc.i4.1
    IL_0021:  beq.s      IL_0025

    IL_0023:  br.s       IL_0035

    IL_0025:  ldsfld     string [mscorlib]System.Boolean::TrueString
    IL_002a:  stloc.1
    IL_002b:  br.s       IL_0039

    IL_002d:  ldsfld     string [mscorlib]System.Boolean::FalseString
    IL_0032:  stloc.1
    IL_0033:  br.s       IL_0039

    IL_0035:  ldnull
    IL_0036:  stloc.1
    IL_0037:  br.s       IL_0039

    IL_0039:  ldloc.1
    IL_003a:  ret
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
    // Code size       484 (0x1e4)
    .maxstack  2
    .locals init (string V_0,
             uint32 V_1)
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
    IL_0015:  call       uint32 '<PrivateImplementationDetails>'::ComputeStringHash(string)
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4     0x330ca589
    IL_0021:  bgt.un.s   IL_0065

    IL_0023:  ldloc.1
    IL_0024:  ldc.i4     0x310ca263
    IL_0029:  bgt.un.s   IL_0048

    IL_002b:  ldloc.1
    IL_002c:  ldc.i4     0x300ca0d0
    IL_0031:  beq        IL_00ff

    IL_0036:  br.s       IL_0038

    IL_0038:  ldloc.1
    IL_0039:  ldc.i4     0x310ca263
    IL_003e:  beq        IL_00ea

    IL_0043:  br         IL_01cb

    IL_0048:  ldloc.1
    IL_0049:  ldc.i4     0x320ca3f6
    IL_004e:  beq        IL_0123

    IL_0053:  br.s       IL_0055

    IL_0055:  ldloc.1
    IL_0056:  ldc.i4     0x330ca589
    IL_005b:  beq        IL_0111

    IL_0060:  br         IL_01cb

    IL_0065:  ldloc.1
    IL_0066:  ldc.i4     0x360caa42
    IL_006b:  bgt.un.s   IL_0084

    IL_006d:  ldloc.1
    IL_006e:  ldc.i4     0x340ca71c
    IL_0073:  beq.s      IL_00ab

    IL_0075:  br.s       IL_0077

    IL_0077:  ldloc.1
    IL_0078:  ldc.i4     0x360caa42
    IL_007d:  beq.s      IL_00d5

    IL_007f:  br         IL_01cb

    IL_0084:  ldloc.1
    IL_0085:  ldc.i4     0x370cabd5
    IL_008a:  beq.s      IL_00c0

    IL_008c:  br.s       IL_008e

    IL_008e:  ldloc.1
    IL_008f:  ldc.i4     0x3c0cb3b4
    IL_0094:  beq        IL_0147

    IL_0099:  br.s       IL_009b

    IL_009b:  ldloc.1
    IL_009c:  ldc.i4     0x3d0cb547
    IL_00a1:  beq        IL_0135

    IL_00a6:  br         IL_01cb

    IL_00ab:  ldloc.0
    IL_00ac:  ldstr      "1"
    IL_00b1:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00b6:  brtrue     IL_0156

    IL_00bb:  br         IL_01cb

    IL_00c0:  ldloc.0
    IL_00c1:  ldstr      "2"
    IL_00c6:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00cb:  brtrue     IL_0163

    IL_00d0:  br         IL_01cb

    IL_00d5:  ldloc.0
    IL_00d6:  ldstr      "3"
    IL_00db:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00e0:  brtrue     IL_0170

    IL_00e5:  br         IL_01cb

    IL_00ea:  ldloc.0
    IL_00eb:  ldstr      "4"
    IL_00f0:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00f5:  brtrue     IL_017d

    IL_00fa:  br         IL_01cb

    IL_00ff:  ldloc.0
    IL_0100:  ldstr      "5"
    IL_0105:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_010a:  brtrue.s   IL_018a

    IL_010c:  br         IL_01cb

    IL_0111:  ldloc.0
    IL_0112:  ldstr      "6"
    IL_0117:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_011c:  brtrue.s   IL_0197

    IL_011e:  br         IL_01cb

    IL_0123:  ldloc.0
    IL_0124:  ldstr      "7"
    IL_0129:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_012e:  brtrue.s   IL_01a4

    IL_0130:  br         IL_01cb

    IL_0135:  ldloc.0
    IL_0136:  ldstr      "8"
    IL_013b:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0140:  brtrue.s   IL_01b1

    IL_0142:  br         IL_01cb

    IL_0147:  ldloc.0
    IL_0148:  ldstr      "9"
    IL_014d:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0152:  brtrue.s   IL_01be

    IL_0154:  br.s       IL_01cb

    IL_0156:  ldstr      "one"
    IL_015b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0160:  nop
    IL_0161:  br.s       IL_01cb

    IL_0163:  ldstr      "two"
    IL_0168:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_016d:  nop
    IL_016e:  br.s       IL_0170

    IL_0170:  ldstr      "three"
    IL_0175:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_017a:  nop
    IL_017b:  br.s       IL_01d8

    IL_017d:  ldstr      "four"
    IL_0182:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0187:  nop
    IL_0188:  br.s       IL_01e3

    IL_018a:  ldstr      "five"
    IL_018f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0194:  nop
    IL_0195:  br.s       IL_01e3

    IL_0197:  ldstr      "six"
    IL_019c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_01a1:  nop
    IL_01a2:  br.s       IL_01e3

    IL_01a4:  ldstr      "seven"
    IL_01a9:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_01ae:  nop
    IL_01af:  br.s       IL_01e3

    IL_01b1:  ldstr      "eight"
    IL_01b6:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_01bb:  nop
    IL_01bc:  br.s       IL_01e3

    IL_01be:  ldstr      "nine"
    IL_01c3:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_01c8:  nop
    IL_01c9:  br.s       IL_01e3

    IL_01cb:  ldstr      "default"
    IL_01d0:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_01d5:  nop
    IL_01d6:  br.s       IL_01d8

    IL_01d8:  ldstr      "End of method"
    IL_01dd:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_01e2:  nop
    IL_01e3:  ret
  } // end of method Switch::SwitchWithGotoString

  .method public hidebysig static void  SwitchWithGotoComplex(string s) cil managed
  {
    // Code size       436 (0x1b4)
    .maxstack  2
    .locals init (string V_0,
             uint32 V_1,
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
    IL_0015:  call       uint32 '<PrivateImplementationDetails>'::ComputeStringHash(string)
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4     0x330ca589
    IL_0021:  bgt.un.s   IL_0065

    IL_0023:  ldloc.1
    IL_0024:  ldc.i4     0x310ca263
    IL_0029:  bgt.un.s   IL_0048

    IL_002b:  ldloc.1
    IL_002c:  ldc.i4     0x300ca0d0
    IL_0031:  beq        IL_00e9

    IL_0036:  br.s       IL_0038

    IL_0038:  ldloc.1
    IL_0039:  ldc.i4     0x310ca263
    IL_003e:  beq        IL_00d4

    IL_0043:  br         IL_0199

    IL_0048:  ldloc.1
    IL_0049:  ldc.i4     0x320ca3f6
    IL_004e:  beq        IL_011c

    IL_0053:  br.s       IL_0055

    IL_0055:  ldloc.1
    IL_0056:  ldc.i4     0x330ca589
    IL_005b:  beq        IL_00fb

    IL_0060:  br         IL_0199

    IL_0065:  ldloc.1
    IL_0066:  ldc.i4     0x360caa42
    IL_006b:  bgt.un.s   IL_0084

    IL_006d:  ldloc.1
    IL_006e:  ldc.i4     0x340ca71c
    IL_0073:  beq.s      IL_009b

    IL_0075:  br.s       IL_0077

    IL_0077:  ldloc.1
    IL_0078:  ldc.i4     0x360caa42
    IL_007d:  beq.s      IL_00c2

    IL_007f:  br         IL_0199

    IL_0084:  ldloc.1
    IL_0085:  ldc.i4     0x370cabd5
    IL_008a:  beq.s      IL_00b0

    IL_008c:  br.s       IL_008e

    IL_008e:  ldloc.1
    IL_008f:  ldc.i4     0x3d0cb547
    IL_0094:  beq.s      IL_010d

    IL_0096:  br         IL_0199

    IL_009b:  ldloc.0
    IL_009c:  ldstr      "1"
    IL_00a1:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00a6:  brtrue     IL_012b

    IL_00ab:  br         IL_0199

    IL_00b0:  ldloc.0
    IL_00b1:  ldstr      "2"
    IL_00b6:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00bb:  brtrue.s   IL_0138

    IL_00bd:  br         IL_0199

    IL_00c2:  ldloc.0
    IL_00c3:  ldstr      "3"
    IL_00c8:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00cd:  brtrue.s   IL_0145

    IL_00cf:  br         IL_0199

    IL_00d4:  ldloc.0
    IL_00d5:  ldstr      "4"
    IL_00da:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00df:  brtrue     IL_0165

    IL_00e4:  br         IL_0199

    IL_00e9:  ldloc.0
    IL_00ea:  ldstr      "5"
    IL_00ef:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00f4:  brtrue.s   IL_0172

    IL_00f6:  br         IL_0199

    IL_00fb:  ldloc.0
    IL_00fc:  ldstr      "6"
    IL_0101:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0106:  brtrue.s   IL_017f

    IL_0108:  br         IL_0199

    IL_010d:  ldloc.0
    IL_010e:  ldstr      "8"
    IL_0113:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0118:  brtrue.s   IL_018c

    IL_011a:  br.s       IL_0199

    IL_011c:  ldloc.0
    IL_011d:  ldstr      "7"
    IL_0122:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0127:  brtrue.s   IL_01a6

    IL_0129:  br.s       IL_0199

    IL_012b:  ldstr      "one"
    IL_0130:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0135:  nop
    IL_0136:  br.s       IL_018c

    IL_0138:  ldstr      "two"
    IL_013d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0142:  nop
    IL_0143:  br.s       IL_0145

    IL_0145:  ldstr      "three"
    IL_014a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_014f:  nop
    IL_0150:  ldarg.0
    IL_0151:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_0156:  ldc.i4.2
    IL_0157:  ceq
    IL_0159:  ldc.i4.0
    IL_015a:  ceq
    IL_015c:  stloc.2
    IL_015d:  ldloc.2
    IL_015e:  brfalse.s  IL_0163

    IL_0160:  nop
    IL_0161:  br.s       IL_01a8

    IL_0163:  br.s       IL_0172

    IL_0165:  ldstr      "four"
    IL_016a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_016f:  nop
    IL_0170:  br.s       IL_0172

    IL_0172:  ldstr      "five"
    IL_0177:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_017c:  nop
    IL_017d:  br.s       IL_018c

    IL_017f:  ldstr      "six"
    IL_0184:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0189:  nop
    IL_018a:  br.s       IL_0172

    IL_018c:  ldstr      "eight"
    IL_0191:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0196:  nop
    IL_0197:  br.s       IL_01b3

    IL_0199:  ldstr      "default"
    IL_019e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_01a3:  nop
    IL_01a4:  br.s       IL_01a8

    IL_01a6:  br.s       IL_01a8

    IL_01a8:  ldstr      "End of method"
    IL_01ad:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_01b2:  nop
    IL_01b3:  ret
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
    // Code size       266 (0x10a)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty> V_0,
             class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty> V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty[] V_2,
             int32 V_3,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty V_4,
             string V_5,
             bool V_6)
    IL_0000:  nop
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::.ctor()
    IL_0006:  stloc.0
    IL_0007:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::.ctor()
    IL_000c:  stloc.1
    IL_000d:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch::GetProperties()
    IL_0012:  stloc.2
    IL_0013:  ldc.i4.0
    IL_0014:  stloc.3
    IL_0015:  br         IL_00fa

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
    IL_003b:  ldstr      "Name1"
    IL_0040:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0045:  brtrue.s   IL_008f

    IL_0047:  ldloc.s    V_5
    IL_0049:  ldstr      "Name2"
    IL_004e:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0053:  brtrue.s   IL_00a3

    IL_0055:  ldloc.s    V_5
    IL_0057:  ldstr      "Name3"
    IL_005c:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0061:  brtrue.s   IL_00b7

    IL_0063:  ldloc.s    V_5
    IL_0065:  ldstr      "Name4"
    IL_006a:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_006f:  brtrue.s   IL_00cb

    IL_0071:  ldloc.s    V_5
    IL_0073:  ldstr      "Name5"
    IL_0078:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_007d:  brtrue.s   IL_00df

    IL_007f:  ldloc.s    V_5
    IL_0081:  ldstr      "Name6"
    IL_0086:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_008b:  brtrue.s   IL_00df

    IL_008d:  br.s       IL_00ea

    IL_008f:  ldloc.s    V_4
    IL_0091:  ldc.i4.1
    IL_0092:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_0097:  nop
    IL_0098:  ldloc.0
    IL_0099:  ldloc.s    V_4
    IL_009b:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00a0:  nop
    IL_00a1:  br.s       IL_00f5

    IL_00a3:  ldloc.s    V_4
    IL_00a5:  ldc.i4.2
    IL_00a6:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00ab:  nop
    IL_00ac:  ldloc.0
    IL_00ad:  ldloc.s    V_4
    IL_00af:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00b4:  nop
    IL_00b5:  br.s       IL_00f5

    IL_00b7:  ldloc.s    V_4
    IL_00b9:  ldc.i4.3
    IL_00ba:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00bf:  nop
    IL_00c0:  ldloc.0
    IL_00c1:  ldloc.s    V_4
    IL_00c3:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00c8:  nop
    IL_00c9:  br.s       IL_00f5

    IL_00cb:  ldloc.s    V_4
    IL_00cd:  ldc.i4.4
    IL_00ce:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00d3:  nop
    IL_00d4:  ldloc.0
    IL_00d5:  ldloc.s    V_4
    IL_00d7:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00dc:  nop
    IL_00dd:  br.s       IL_00f5

    IL_00df:  ldloc.0
    IL_00e0:  ldloc.s    V_4
    IL_00e2:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00e7:  nop
    IL_00e8:  br.s       IL_00f5

    IL_00ea:  ldloc.1
    IL_00eb:  ldloc.s    V_4
    IL_00ed:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00f2:  nop
    IL_00f3:  br.s       IL_00f5

    IL_00f5:  nop
    IL_00f6:  ldloc.3
    IL_00f7:  ldc.i4.1
    IL_00f8:  add
    IL_00f9:  stloc.3
    IL_00fa:  ldloc.3
    IL_00fb:  ldloc.2
    IL_00fc:  ldlen
    IL_00fd:  conv.i4
    IL_00fe:  clt
    IL_0100:  stloc.s    V_6
    IL_0102:  ldloc.s    V_6
    IL_0104:  brtrue     IL_001a

    IL_0109:  ret
  } // end of method Switch::SwitchOnStringInForLoop

  .method public hidebysig static void  SwitchInTryBlock(string 'value') cil managed
  {
    // Code size       188 (0xbc)
    .maxstack  2
    .locals init (string V_0)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldarg.0
      IL_0003:  ldc.i4.5
      IL_0004:  callvirt   instance string [mscorlib]System.String::Substring(int32)
      IL_0009:  stloc.0
      IL_000a:  ldloc.0
      IL_000b:  ldstr      "Name1"
      IL_0010:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                     string)
      IL_0015:  brtrue.s   IL_005a

      IL_0017:  ldloc.0
      IL_0018:  ldstr      "Name2"
      IL_001d:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                     string)
      IL_0022:  brtrue.s   IL_0067

      IL_0024:  ldloc.0
      IL_0025:  ldstr      "Name3"
      IL_002a:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                     string)
      IL_002f:  brtrue.s   IL_0074

      IL_0031:  ldloc.0
      IL_0032:  ldstr      "Name4"
      IL_0037:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                     string)
      IL_003c:  brtrue.s   IL_0081

      IL_003e:  ldloc.0
      IL_003f:  ldstr      "Name5"
      IL_0044:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                     string)
      IL_0049:  brtrue.s   IL_008e

      IL_004b:  ldloc.0
      IL_004c:  ldstr      "Name6"
      IL_0051:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                     string)
      IL_0056:  brtrue.s   IL_008e

      IL_0058:  br.s       IL_009b

      IL_005a:  ldstr      "1"
      IL_005f:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0064:  nop
      IL_0065:  br.s       IL_00a8

      IL_0067:  ldstr      "Name_2"
      IL_006c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0071:  nop
      IL_0072:  br.s       IL_00a8

      IL_0074:  ldstr      "Name_3"
      IL_0079:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_007e:  nop
      IL_007f:  br.s       IL_00a8

      IL_0081:  ldstr      "No. 4"
      IL_0086:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_008b:  nop
      IL_008c:  br.s       IL_00a8

      IL_008e:  ldstr      "5+6"
      IL_0093:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0098:  nop
      IL_0099:  br.s       IL_00a8

      IL_009b:  ldstr      "default"
      IL_00a0:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00a5:  nop
      IL_00a6:  br.s       IL_00a8

      IL_00a8:  nop
      IL_00a9:  leave.s    IL_00bb

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_00ab:  pop
      IL_00ac:  nop
      IL_00ad:  ldstr      "catch block"
      IL_00b2:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00b7:  nop
      IL_00b8:  nop
      IL_00b9:  leave.s    IL_00bb

    }  // end handler
    IL_00bb:  ret
  } // end of method Switch::SwitchInTryBlock

  .method public hidebysig static void  SwitchWithComplexCondition(string[] args) cil managed
  {
    // Code size       134 (0x86)
    .maxstack  2
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldlen
    IL_0003:  brfalse.s  IL_000a

    IL_0005:  ldarg.0
    IL_0006:  ldc.i4.0
    IL_0007:  ldelem.ref
    IL_0008:  br.s       IL_000f

    IL_000a:  ldstr      "dummy"
    IL_000f:  stloc.0
    IL_0010:  ldloc.0
    IL_0011:  ldstr      "a"
    IL_0016:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_001b:  brtrue.s   IL_0046

    IL_001d:  ldloc.0
    IL_001e:  ldstr      "b"
    IL_0023:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0028:  brtrue.s   IL_0053

    IL_002a:  ldloc.0
    IL_002b:  ldstr      "c"
    IL_0030:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0035:  brtrue.s   IL_0060

    IL_0037:  ldloc.0
    IL_0038:  ldstr      "d"
    IL_003d:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0042:  brtrue.s   IL_006d

    IL_0044:  br.s       IL_007a

    IL_0046:  ldstr      "a"
    IL_004b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0050:  nop
    IL_0051:  br.s       IL_007a

    IL_0053:  ldstr      "b"
    IL_0058:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005d:  nop
    IL_005e:  br.s       IL_007a

    IL_0060:  ldstr      "c"
    IL_0065:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006a:  nop
    IL_006b:  br.s       IL_007a

    IL_006d:  ldstr      "d"
    IL_0072:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0077:  nop
    IL_0078:  br.s       IL_007a

    IL_007a:  ldstr      "end"
    IL_007f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0084:  nop
    IL_0085:  ret
  } // end of method Switch::SwitchWithComplexCondition

  .method public hidebysig static void  SwitchWithArray(string[] args) cil managed
  {
    // Code size       123 (0x7b)
    .maxstack  2
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  ldelem.ref
    IL_0004:  stloc.0
    IL_0005:  ldloc.0
    IL_0006:  ldstr      "a"
    IL_000b:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0010:  brtrue.s   IL_003b

    IL_0012:  ldloc.0
    IL_0013:  ldstr      "b"
    IL_0018:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_001d:  brtrue.s   IL_0048

    IL_001f:  ldloc.0
    IL_0020:  ldstr      "c"
    IL_0025:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_002a:  brtrue.s   IL_0055

    IL_002c:  ldloc.0
    IL_002d:  ldstr      "d"
    IL_0032:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0037:  brtrue.s   IL_0062

    IL_0039:  br.s       IL_006f

    IL_003b:  ldstr      "a"
    IL_0040:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0045:  nop
    IL_0046:  br.s       IL_006f

    IL_0048:  ldstr      "b"
    IL_004d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0052:  nop
    IL_0053:  br.s       IL_006f

    IL_0055:  ldstr      "c"
    IL_005a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005f:  nop
    IL_0060:  br.s       IL_006f

    IL_0062:  ldstr      "d"
    IL_0067:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006c:  nop
    IL_006d:  br.s       IL_006f

    IL_006f:  ldstr      "end"
    IL_0074:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0079:  nop
    IL_007a:  ret
  } // end of method Switch::SwitchWithArray

  .method public hidebysig static void  SwitchWithContinue1(int32 i,
                                                            bool b) cil managed
  {
    // Code size       62 (0x3e)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1,
             bool V_2,
             bool V_3)
    IL_0000:  nop
    IL_0001:  br.s       IL_003a

    IL_0003:  nop
    IL_0004:  ldarg.0
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  switch     ( 
                          IL_001a,
                          IL_0031,
                          IL_0024)
    IL_0018:  br.s       IL_0033

    IL_001a:  ldarg.1
    IL_001b:  stloc.1
    IL_001c:  ldloc.1
    IL_001d:  brfalse.s  IL_0022

    IL_001f:  nop
    IL_0020:  br.s       IL_003a

    IL_0022:  br.s       IL_0033

    IL_0024:  ldarg.1
    IL_0025:  ldc.i4.0
    IL_0026:  ceq
    IL_0028:  stloc.2
    IL_0029:  ldloc.2
    IL_002a:  brfalse.s  IL_002f

    IL_002c:  nop
    IL_002d:  br.s       IL_003a

    IL_002f:  br.s       IL_0033

    IL_0031:  br.s       IL_003a

    IL_0033:  call       void [mscorlib]System.Console::WriteLine()
    IL_0038:  nop
    IL_0039:  nop
    IL_003a:  ldc.i4.1
    IL_003b:  stloc.3
    IL_003c:  br.s       IL_0003
  } // end of method Switch::SwitchWithContinue1

  .method public hidebysig static void  SwitchWithContinue2(int32 i,
                                                            bool b) cil managed
  {
    // Code size       147 (0x93)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1,
             bool V_2,
             bool V_3)
    IL_0000:  nop
    IL_0001:  br         IL_0086

    IL_0006:  nop
    IL_0007:  ldarg.0
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  switch     ( 
                          IL_0021,
                          IL_0073,
                          IL_0041,
                          IL_0071)
    IL_001f:  br.s       IL_0064

    IL_0021:  ldarg.1
    IL_0022:  stloc.1
    IL_0023:  ldloc.1
    IL_0024:  brfalse.s  IL_0034

    IL_0026:  nop
    IL_0027:  ldstr      "0b"
    IL_002c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0031:  nop
    IL_0032:  br.s       IL_0086

    IL_0034:  ldstr      "0!b"
    IL_0039:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003e:  nop
    IL_003f:  br.s       IL_0075

    IL_0041:  ldarg.1
    IL_0042:  ldc.i4.0
    IL_0043:  ceq
    IL_0045:  stloc.2
    IL_0046:  ldloc.2
    IL_0047:  brfalse.s  IL_0057

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
    IL_008b:  stloc.3
    IL_008c:  ldloc.3
    IL_008d:  brtrue     IL_0006

    IL_0092:  ret
  } // end of method Switch::SwitchWithContinue2

  .method public hidebysig static void  SwitchWithContinue3(bool b) cil managed
  {
    // Code size       147 (0x93)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1,
             bool V_2,
             bool V_3,
             bool V_4)
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
                          IL_0040,
                          IL_0070)
    IL_001e:  br.s       IL_0063

    IL_0020:  ldarg.0
    IL_0021:  stloc.2
    IL_0022:  ldloc.2
    IL_0023:  brfalse.s  IL_0033

    IL_0025:  nop
    IL_0026:  ldstr      "0b"
    IL_002b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0030:  nop
    IL_0031:  br.s       IL_0080

    IL_0033:  ldstr      "0!b"
    IL_0038:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003d:  nop
    IL_003e:  br.s       IL_0074

    IL_0040:  ldarg.0
    IL_0041:  ldc.i4.0
    IL_0042:  ceq
    IL_0044:  stloc.3
    IL_0045:  ldloc.3
    IL_0046:  brfalse.s  IL_0056

    IL_0048:  nop
    IL_0049:  ldstr      "2!b"
    IL_004e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0053:  nop
    IL_0054:  br.s       IL_0080

    IL_0056:  ldstr      "2b"
    IL_005b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0060:  nop
    IL_0061:  br.s       IL_0092

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
    IL_0089:  stloc.s    V_4
    IL_008b:  ldloc.s    V_4
    IL_008d:  brtrue     IL_0005

    IL_0092:  ret
  } // end of method Switch::SwitchWithContinue3

  .method public hidebysig static void  SwitchWithContinue4(bool b) cil managed
  {
    // Code size       261 (0x105)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_0,
             int32 V_1,
             int32 V_2,
             bool V_3,
             bool V_4,
             bool V_5,
             bool V_6,
             bool V_7,
             bool V_8)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldc.i4.0
    IL_0003:  ldc.i4.s   10
    IL_0005:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [System.Core]System.Linq.Enumerable::Range(int32,
                                                                                                                                    int32)
    IL_000a:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_000f:  stloc.0
    .try
    {
      IL_0010:  br         IL_00ec

      IL_0015:  ldloc.0
      IL_0016:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_001b:  stloc.1
      IL_001c:  nop
      IL_001d:  ldstr      "loop: "
      IL_0022:  ldloc.1
      IL_0023:  box        [mscorlib]System.Int32
      IL_0028:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_002d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0032:  nop
      IL_0033:  ldloc.1
      IL_0034:  stloc.2
      IL_0035:  ldloc.2
      IL_0036:  ldc.i4.1
      IL_0037:  sub
      IL_0038:  switch     ( 
                            IL_005f,
                            IL_00d3,
                            IL_006c,
                            IL_007e,
                            IL_0087,
                            IL_0090,
                            IL_009c,
                            IL_00ba)
      IL_005d:  br.s       IL_00c6

      IL_005f:  ldarg.0
      IL_0060:  stloc.3
      IL_0061:  ldloc.3
      IL_0062:  brfalse.s  IL_006a

      IL_0064:  nop
      IL_0065:  br         IL_00ec

      IL_006a:  br.s       IL_00d5

      IL_006c:  ldarg.0
      IL_006d:  ldc.i4.0
      IL_006e:  ceq
      IL_0070:  stloc.s    V_4
      IL_0072:  ldloc.s    V_4
      IL_0074:  brfalse.s  IL_0079

      IL_0076:  nop
      IL_0077:  br.s       IL_00ec

      IL_0079:  leave      IL_0104

      IL_007e:  ldc.i4.4
      IL_007f:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_0084:  nop
      IL_0085:  br.s       IL_009c

      IL_0087:  ldc.i4.5
      IL_0088:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_008d:  nop
      IL_008e:  br.s       IL_00c6

      IL_0090:  ldarg.0
      IL_0091:  stloc.s    V_5
      IL_0093:  ldloc.s    V_5
      IL_0095:  brfalse.s  IL_009a

      IL_0097:  nop
      IL_0098:  br.s       IL_00ec

      IL_009a:  br.s       IL_006c

      IL_009c:  ldloc.1
      IL_009d:  ldc.i4.2
      IL_009e:  rem
      IL_009f:  ldc.i4.0
      IL_00a0:  ceq
      IL_00a2:  stloc.s    V_6
      IL_00a4:  ldloc.s    V_6
      IL_00a6:  brfalse.s  IL_00ab

      IL_00a8:  nop
      IL_00a9:  br.s       IL_006c

      IL_00ab:  ldarg.0
      IL_00ac:  ldc.i4.0
      IL_00ad:  ceq
      IL_00af:  stloc.s    V_7
      IL_00b1:  ldloc.s    V_7
      IL_00b3:  brfalse.s  IL_00b8

      IL_00b5:  nop
      IL_00b6:  br.s       IL_00ec

      IL_00b8:  br.s       IL_00ba

      IL_00ba:  ldarg.0
      IL_00bb:  stloc.s    V_8
      IL_00bd:  ldloc.s    V_8
      IL_00bf:  brfalse.s  IL_00c4

      IL_00c1:  nop
      IL_00c2:  br.s       IL_00ec

      IL_00c4:  br.s       IL_0087

      IL_00c6:  ldstr      "default"
      IL_00cb:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00d0:  nop
      IL_00d1:  br.s       IL_00d5

      IL_00d3:  br.s       IL_00ec

      IL_00d5:  ldstr      "break: "
      IL_00da:  ldloc.1
      IL_00db:  box        [mscorlib]System.Int32
      IL_00e0:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_00e5:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00ea:  nop
      IL_00eb:  nop
      IL_00ec:  ldloc.0
      IL_00ed:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_00f2:  brtrue     IL_0015

      IL_00f7:  leave.s    IL_0104

    }  // end .try
    finally
    {
      IL_00f9:  ldloc.0
      IL_00fa:  brfalse.s  IL_0103

      IL_00fc:  ldloc.0
      IL_00fd:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0102:  nop
      IL_0103:  endfinally
    }  // end handler
    IL_0104:  ret
  } // end of method Switch::SwitchWithContinue4

  .method public hidebysig static void  SwitchWithContinue5(bool b) cil managed
  {
    // Code size       173 (0xad)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1,
             int32 V_2,
             bool V_3,
             bool V_4,
             bool V_5)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br         IL_009e

    IL_0008:  nop
    IL_0009:  ldloc.0
    IL_000a:  ldc.i4.5
    IL_000b:  clt
    IL_000d:  stloc.1
    IL_000e:  ldloc.1
    IL_000f:  brfalse.s  IL_008e

    IL_0011:  nop
    IL_0012:  ldloc.0
    IL_0013:  stloc.2
    IL_0014:  ldloc.2
    IL_0015:  switch     ( 
                          IL_002c,
                          IL_0080,
                          IL_004c,
                          IL_007e)
    IL_002a:  br.s       IL_0071

    IL_002c:  ldarg.0
    IL_002d:  stloc.3
    IL_002e:  ldloc.3
    IL_002f:  brfalse.s  IL_003f

    IL_0031:  nop
    IL_0032:  ldstr      "0b"
    IL_0037:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003c:  nop
    IL_003d:  br.s       IL_009a

    IL_003f:  ldstr      "0!b"
    IL_0044:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0049:  nop
    IL_004a:  br.s       IL_0082

    IL_004c:  ldarg.0
    IL_004d:  ldc.i4.0
    IL_004e:  ceq
    IL_0050:  stloc.s    V_4
    IL_0052:  ldloc.s    V_4
    IL_0054:  brfalse.s  IL_0064

    IL_0056:  nop
    IL_0057:  ldstr      "2!b"
    IL_005c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0061:  nop
    IL_0062:  br.s       IL_009a

    IL_0064:  ldstr      "2b"
    IL_0069:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006e:  nop
    IL_006f:  br.s       IL_00ac

    IL_0071:  ldstr      "default"
    IL_0076:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007b:  nop
    IL_007c:  br.s       IL_0082

    IL_007e:  br.s       IL_0082

    IL_0080:  br.s       IL_009a

    IL_0082:  ldstr      "break-target"
    IL_0087:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_008c:  nop
    IL_008d:  nop
    IL_008e:  ldstr      "loop-tail"
    IL_0093:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0098:  nop
    IL_0099:  nop
    IL_009a:  ldloc.0
    IL_009b:  ldc.i4.1
    IL_009c:  add
    IL_009d:  stloc.0
    IL_009e:  ldloc.0
    IL_009f:  ldc.i4.s   10
    IL_00a1:  clt
    IL_00a3:  stloc.s    V_5
    IL_00a5:  ldloc.s    V_5
    IL_00a7:  brtrue     IL_0008

    IL_00ac:  ret
  } // end of method Switch::SwitchWithContinue5

  .method public hidebysig static void  SwitchWithContinue6(int32 i,
                                                            bool b) cil managed
  {
    // Code size       142 (0x8e)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1,
             bool V_2,
             bool V_3)
    IL_0000:  nop
    IL_0001:  nop
    IL_0002:  ldarg.0
    IL_0003:  stloc.0
    IL_0004:  ldloc.0
    IL_0005:  switch     ( 
                          IL_001c,
                          IL_006e,
                          IL_003f,
                          IL_006c)
    IL_001a:  br.s       IL_005f

    IL_001c:  ldarg.1
    IL_001d:  ldc.i4.0
    IL_001e:  ceq
    IL_0020:  stloc.1
    IL_0021:  ldloc.1
    IL_0022:  brfalse.s  IL_0032

    IL_0024:  nop
    IL_0025:  ldstr      "0!b"
    IL_002a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002f:  nop
    IL_0030:  br.s       IL_0070

    IL_0032:  ldstr      "0b"
    IL_0037:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003c:  nop
    IL_003d:  br.s       IL_007c

    IL_003f:  ldarg.1
    IL_0040:  stloc.2
    IL_0041:  ldloc.2
    IL_0042:  brfalse.s  IL_0052

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
    IL_0086:  stloc.3
    IL_0087:  ldloc.3
    IL_0088:  brtrue     IL_0001

    IL_008d:  ret
  } // end of method Switch::SwitchWithContinue6

  .method public hidebysig static void  SwitchWithContinue7() cil managed
  {
    // Code size       76 (0x4c)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0035

    IL_0005:  nop
    IL_0006:  ldstr      "loop-head"
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0010:  nop
    IL_0011:  ldloc.0
    IL_0012:  stloc.1
    IL_0013:  ldloc.1
    IL_0014:  brfalse.s  IL_002b

    IL_0016:  br.s       IL_0018

    IL_0018:  ldloc.1
    IL_0019:  ldc.i4.1
    IL_001a:  beq.s      IL_002d

    IL_001c:  br.s       IL_001e

    IL_001e:  ldstr      "default"
    IL_0023:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0028:  nop
    IL_0029:  br.s       IL_002f

    IL_002b:  br.s       IL_0031

    IL_002d:  br.s       IL_002f

    IL_002f:  br.s       IL_0040

    IL_0031:  ldloc.0
    IL_0032:  ldc.i4.1
    IL_0033:  sub
    IL_0034:  stloc.0
    IL_0035:  ldloc.0
    IL_0036:  ldc.i4.0
    IL_0037:  clt
    IL_0039:  ldc.i4.0
    IL_003a:  ceq
    IL_003c:  stloc.2
    IL_003d:  ldloc.2
    IL_003e:  brtrue.s   IL_0005

    IL_0040:  ldstr      "end"
    IL_0045:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004a:  nop
    IL_004b:  ret
  } // end of method Switch::SwitchWithContinue7

  .method public hidebysig static void  SwitchWithContinueInDoubleLoop() cil managed
  {
    // Code size       128 (0x80)
    .maxstack  2
    .locals init (bool V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3,
             bool V_4,
             bool V_5)
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
    IL_0012:  ldc.i4.s   11
    IL_0014:  bgt.s      IL_0043

    IL_0016:  ldloc.3
    IL_0017:  ldc.i4.1
    IL_0018:  sub
    IL_0019:  switch     ( 
                          IL_0051,
                          IL_0053,
                          IL_0051,
                          IL_0053,
                          IL_0051,
                          IL_0053,
                          IL_0051)
    IL_003a:  br.s       IL_003c

    IL_003c:  ldloc.3
    IL_003d:  ldc.i4.s   11
    IL_003f:  beq.s      IL_0051

    IL_0041:  br.s       IL_0053

    IL_0043:  ldloc.3
    IL_0044:  ldc.i4.s   13
    IL_0046:  beq.s      IL_0051

    IL_0048:  br.s       IL_004a

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
    IL_0072:  stloc.s    V_5
    IL_0074:  ldloc.s    V_5
    IL_0076:  brtrue.s   IL_0007

    IL_0078:  ldloc.0
    IL_0079:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_007e:  nop
    IL_007f:  ret
  } // end of method Switch::SwitchWithContinueInDoubleLoop

  .method public hidebysig static void  SwitchLoopNesting() cil managed
  {
    // Code size       140 (0x8c)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1,
             bool V_2,
             bool V_3,
             bool V_4,
             bool V_5)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_007d

    IL_0005:  nop
    IL_0006:  ldloc.0
    IL_0007:  stloc.1
    IL_0008:  ldloc.1
    IL_0009:  brfalse.s  IL_0013

    IL_000b:  br.s       IL_000d

    IL_000d:  ldloc.1
    IL_000e:  ldc.i4.1
    IL_000f:  beq.s      IL_001c

    IL_0011:  br.s       IL_0025

    IL_0013:  ldc.i4.0
    IL_0014:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0019:  nop
    IL_001a:  br.s       IL_0052

    IL_001c:  ldc.i4.1
    IL_001d:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0022:  nop
    IL_0023:  br.s       IL_0052

    IL_0025:  ldloc.0
    IL_0026:  ldc.i4.2
    IL_0027:  rem
    IL_0028:  ldc.i4.0
    IL_0029:  ceq
    IL_002b:  stloc.2
    IL_002c:  ldloc.2
    IL_002d:  brfalse.s  IL_004a

    IL_002f:  nop
    IL_0030:  br.s       IL_003f

    IL_0032:  nop
    IL_0033:  ldloc.0
    IL_0034:  dup
    IL_0035:  ldc.i4.1
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_003d:  nop
    IL_003e:  nop
    IL_003f:  ldloc.0
    IL_0040:  ldc.i4.3
    IL_0041:  rem
    IL_0042:  ldc.i4.0
    IL_0043:  cgt.un
    IL_0045:  stloc.3
    IL_0046:  ldloc.3
    IL_0047:  brtrue.s   IL_0032

    IL_0049:  nop
    IL_004a:  call       void [mscorlib]System.Console::WriteLine()
    IL_004f:  nop
    IL_0050:  br.s       IL_0052

    IL_0052:  ldloc.0
    IL_0053:  ldc.i4.4
    IL_0054:  cgt
    IL_0056:  stloc.s    V_4
    IL_0058:  ldloc.s    V_4
    IL_005a:  brfalse.s  IL_006b

    IL_005c:  nop
    IL_005d:  ldstr      "high"
    IL_0062:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0067:  nop
    IL_0068:  nop
    IL_0069:  br.s       IL_0078

    IL_006b:  nop
    IL_006c:  ldstr      "low"
    IL_0071:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0076:  nop
    IL_0077:  nop
    IL_0078:  nop
    IL_0079:  ldloc.0
    IL_007a:  ldc.i4.1
    IL_007b:  add
    IL_007c:  stloc.0
    IL_007d:  ldloc.0
    IL_007e:  ldc.i4.s   10
    IL_0080:  clt
    IL_0082:  stloc.s    V_5
    IL_0084:  ldloc.s    V_5
    IL_0086:  brtrue     IL_0005

    IL_008b:  ret
  } // end of method Switch::SwitchLoopNesting

  .method public hidebysig static void  SingleIf1(int32 i,
                                                  bool a) cil managed
  {
    // Code size       35 (0x23)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.1
    IL_0003:  beq.s      IL_000d

    IL_0005:  ldarg.0
    IL_0006:  ldc.i4.2
    IL_0007:  ceq
    IL_0009:  ldarg.1
    IL_000a:  and
    IL_000b:  br.s       IL_000e

    IL_000d:  ldc.i4.1
    IL_000e:  stloc.0
    IL_000f:  ldloc.0
    IL_0010:  brfalse.s  IL_001b

    IL_0012:  nop
    IL_0013:  ldc.i4.1
    IL_0014:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0019:  nop
    IL_001a:  nop
    IL_001b:  ldc.i4.2
    IL_001c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0021:  nop
    IL_0022:  ret
  } // end of method Switch::SingleIf1

  .method public hidebysig static void  SingleIf2(int32 i,
                                                  bool a,
                                                  bool b) cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.1
    IL_0003:  beq.s      IL_0015

    IL_0005:  ldarg.0
    IL_0006:  ldc.i4.2
    IL_0007:  ceq
    IL_0009:  ldarg.1
    IL_000a:  and
    IL_000b:  brtrue.s   IL_0015

    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.3
    IL_000f:  ceq
    IL_0011:  ldarg.2
    IL_0012:  and
    IL_0013:  br.s       IL_0016

    IL_0015:  ldc.i4.1
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  brfalse.s  IL_0023

    IL_001a:  nop
    IL_001b:  ldc.i4.1
    IL_001c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0021:  nop
    IL_0022:  nop
    IL_0023:  ldc.i4.2
    IL_0024:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0029:  nop
    IL_002a:  ret
  } // end of method Switch::SingleIf2

  .method public hidebysig static void  SingleIf3(int32 i,
                                                  bool a,
                                                  bool b) cil managed
  {
    // Code size       38 (0x26)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  brtrue.s   IL_0010

    IL_0004:  ldarg.0
    IL_0005:  ldc.i4.1
    IL_0006:  beq.s      IL_0010

    IL_0008:  ldarg.0
    IL_0009:  ldc.i4.2
    IL_000a:  ceq
    IL_000c:  ldarg.2
    IL_000d:  and
    IL_000e:  br.s       IL_0011

    IL_0010:  ldc.i4.1
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  brfalse.s  IL_001e

    IL_0015:  nop
    IL_0016:  ldc.i4.1
    IL_0017:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001c:  nop
    IL_001d:  nop
    IL_001e:  ldc.i4.2
    IL_001f:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0024:  nop
    IL_0025:  ret
  } // end of method Switch::SingleIf3

  .method public hidebysig static void  SingleIf4(int32 i,
                                                  bool a) cil managed
  {
    // Code size       51 (0x33)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.1
    IL_0003:  beq.s      IL_001d

    IL_0005:  ldarg.0
    IL_0006:  ldc.i4.2
    IL_0007:  beq.s      IL_001d

    IL_0009:  ldarg.0
    IL_000a:  ldc.i4.3
    IL_000b:  ceq
    IL_000d:  ldc.i4.0
    IL_000e:  ceq
    IL_0010:  ldarg.1
    IL_0011:  and
    IL_0012:  brtrue.s   IL_001d

    IL_0014:  ldarg.0
    IL_0015:  ldc.i4.4
    IL_0016:  ceq
    IL_0018:  ldc.i4.0
    IL_0019:  ceq
    IL_001b:  br.s       IL_001e

    IL_001d:  ldc.i4.1
    IL_001e:  stloc.0
    IL_001f:  ldloc.0
    IL_0020:  brfalse.s  IL_002b

    IL_0022:  nop
    IL_0023:  ldc.i4.1
    IL_0024:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0029:  nop
    IL_002a:  nop
    IL_002b:  ldc.i4.2
    IL_002c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0031:  nop
    IL_0032:  ret
  } // end of method Switch::SingleIf4

  .method public hidebysig static void  NestedIf(int32 i) cil managed
  {
    // Code size       49 (0x31)
    .maxstack  2
    .locals init (bool V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.1
    IL_0003:  ceq
    IL_0005:  ldc.i4.0
    IL_0006:  ceq
    IL_0008:  stloc.0
    IL_0009:  ldloc.0
    IL_000a:  brfalse.s  IL_002a

    IL_000c:  nop
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.2
    IL_000f:  ceq
    IL_0011:  stloc.1
    IL_0012:  ldloc.1
    IL_0013:  brfalse.s  IL_001e

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
    // Code size       151 (0x97)
    .maxstack  2
    .locals init (bool V_0,
             bool V_1,
             bool V_2,
             bool V_3,
             bool V_4,
             bool V_5)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  ceq
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  brfalse.s  IL_0014

    IL_0009:  nop
    IL_000a:  ldc.i4.0
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0010:  nop
    IL_0011:  nop
    IL_0012:  br.s       IL_0090

    IL_0014:  ldarg.0
    IL_0015:  ldc.i4.1
    IL_0016:  ceq
    IL_0018:  stloc.1
    IL_0019:  ldloc.1
    IL_001a:  brfalse.s  IL_0027

    IL_001c:  nop
    IL_001d:  ldc.i4.1
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0023:  nop
    IL_0024:  nop
    IL_0025:  br.s       IL_0090

    IL_0027:  ldarg.0
    IL_0028:  ldc.i4.2
    IL_0029:  ceq
    IL_002b:  stloc.2
    IL_002c:  ldloc.2
    IL_002d:  brfalse.s  IL_003a

    IL_002f:  nop
    IL_0030:  ldc.i4.2
    IL_0031:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0036:  nop
    IL_0037:  nop
    IL_0038:  br.s       IL_0090

    IL_003a:  ldarg.0
    IL_003b:  ldc.i4.3
    IL_003c:  ceq
    IL_003e:  stloc.3
    IL_003f:  ldloc.3
    IL_0040:  brfalse.s  IL_004d

    IL_0042:  nop
    IL_0043:  ldc.i4.3
    IL_0044:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0049:  nop
    IL_004a:  nop
    IL_004b:  br.s       IL_0090

    IL_004d:  ldarg.0
    IL_004e:  ldc.i4.4
    IL_004f:  ceq
    IL_0051:  stloc.s    V_4
    IL_0053:  ldloc.s    V_4
    IL_0055:  brfalse.s  IL_0062

    IL_0057:  nop
    IL_0058:  ldc.i4.4
    IL_0059:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_005e:  nop
    IL_005f:  nop
    IL_0060:  br.s       IL_0090

    IL_0062:  ldarg.0
    IL_0063:  ldc.i4.5
    IL_0064:  bne.un.s   IL_006d

    IL_0066:  call       bool [mscorlib]System.Console::get_CapsLock()
    IL_006b:  br.s       IL_006e

    IL_006d:  ldc.i4.0
    IL_006e:  stloc.s    V_5
    IL_0070:  ldloc.s    V_5
    IL_0072:  brfalse.s  IL_0083

    IL_0074:  nop
    IL_0075:  ldstr      "5A"
    IL_007a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007f:  nop
    IL_0080:  nop
    IL_0081:  br.s       IL_0090

    IL_0083:  nop
    IL_0084:  ldstr      "default"
    IL_0089:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_008e:  nop
    IL_008f:  nop
    IL_0090:  call       void [mscorlib]System.Console::WriteLine()
    IL_0095:  nop
    IL_0096:  ret
  } // end of method Switch::IfChainWithCondition

  .method public hidebysig static bool  SwitchlikeIf(int32 i,
                                                     int32 j) cil managed
  {
    // Code size       270 (0x10e)
    .maxstack  2
    .locals init (bool V_0,
             bool V_1,
             bool V_2,
             bool V_3,
             bool V_4,
             bool V_5,
             bool V_6,
             bool V_7,
             bool V_8,
             bool V_9,
             bool V_10,
             bool V_11)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  brfalse.s  IL_000a

    IL_0004:  ldarg.1
    IL_0005:  ldc.i4.0
    IL_0006:  cgt.un
    IL_0008:  br.s       IL_000b

    IL_000a:  ldc.i4.0
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  brfalse.s  IL_008a

    IL_000f:  nop
    IL_0010:  ldarg.0
    IL_0011:  ldc.i4.m1
    IL_0012:  bne.un.s   IL_001a

    IL_0014:  ldarg.1
    IL_0015:  ldc.i4.m1
    IL_0016:  ceq
    IL_0018:  br.s       IL_001b

    IL_001a:  ldc.i4.0
    IL_001b:  stloc.1
    IL_001c:  ldloc.1
    IL_001d:  brfalse.s  IL_002c

    IL_001f:  nop
    IL_0020:  ldstr      "-1, -1"
    IL_0025:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002a:  nop
    IL_002b:  nop
    IL_002c:  ldarg.0
    IL_002d:  ldc.i4.m1
    IL_002e:  bne.un.s   IL_0036

    IL_0030:  ldarg.1
    IL_0031:  ldc.i4.1
    IL_0032:  ceq
    IL_0034:  br.s       IL_0037

    IL_0036:  ldc.i4.0
    IL_0037:  stloc.2
    IL_0038:  ldloc.2
    IL_0039:  brfalse.s  IL_0048

    IL_003b:  nop
    IL_003c:  ldstr      "-1, 1"
    IL_0041:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0046:  nop
    IL_0047:  nop
    IL_0048:  ldarg.0
    IL_0049:  ldc.i4.1
    IL_004a:  bne.un.s   IL_0052

    IL_004c:  ldarg.1
    IL_004d:  ldc.i4.m1
    IL_004e:  ceq
    IL_0050:  br.s       IL_0053

    IL_0052:  ldc.i4.0
    IL_0053:  stloc.3
    IL_0054:  ldloc.3
    IL_0055:  brfalse.s  IL_0064

    IL_0057:  nop
    IL_0058:  ldstr      "1, -1"
    IL_005d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0062:  nop
    IL_0063:  nop
    IL_0064:  ldarg.0
    IL_0065:  ldc.i4.1
    IL_0066:  bne.un.s   IL_006e

    IL_0068:  ldarg.1
    IL_0069:  ldc.i4.1
    IL_006a:  ceq
    IL_006c:  br.s       IL_006f

    IL_006e:  ldc.i4.0
    IL_006f:  stloc.s    V_4
    IL_0071:  ldloc.s    V_4
    IL_0073:  brfalse.s  IL_0082

    IL_0075:  nop
    IL_0076:  ldstr      "1, 1"
    IL_007b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0080:  nop
    IL_0081:  nop
    IL_0082:  ldc.i4.0
    IL_0083:  stloc.s    V_5
    IL_0085:  br         IL_010b

    IL_008a:  ldarg.0
    IL_008b:  ldc.i4.0
    IL_008c:  cgt.un
    IL_008e:  stloc.s    V_6
    IL_0090:  ldloc.s    V_6
    IL_0092:  brfalse.s  IL_00c8

    IL_0094:  nop
    IL_0095:  ldarg.0
    IL_0096:  ldc.i4.m1
    IL_0097:  ceq
    IL_0099:  stloc.s    V_7
    IL_009b:  ldloc.s    V_7
    IL_009d:  brfalse.s  IL_00ac

    IL_009f:  nop
    IL_00a0:  ldstr      "-1, 0"
    IL_00a5:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00aa:  nop
    IL_00ab:  nop
    IL_00ac:  ldarg.0
    IL_00ad:  ldc.i4.1
    IL_00ae:  ceq
    IL_00b0:  stloc.s    V_8
    IL_00b2:  ldloc.s    V_8
    IL_00b4:  brfalse.s  IL_00c3

    IL_00b6:  nop
    IL_00b7:  ldstr      "1, 0"
    IL_00bc:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00c1:  nop
    IL_00c2:  nop
    IL_00c3:  ldc.i4.0
    IL_00c4:  stloc.s    V_5
    IL_00c6:  br.s       IL_010b

    IL_00c8:  ldarg.1
    IL_00c9:  ldc.i4.0
    IL_00ca:  cgt.un
    IL_00cc:  stloc.s    V_9
    IL_00ce:  ldloc.s    V_9
    IL_00d0:  brfalse.s  IL_0106

    IL_00d2:  nop
    IL_00d3:  ldarg.1
    IL_00d4:  ldc.i4.m1
    IL_00d5:  ceq
    IL_00d7:  stloc.s    V_10
    IL_00d9:  ldloc.s    V_10
    IL_00db:  brfalse.s  IL_00ea

    IL_00dd:  nop
    IL_00de:  ldstr      "0, -1"
    IL_00e3:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00e8:  nop
    IL_00e9:  nop
    IL_00ea:  ldarg.1
    IL_00eb:  ldc.i4.1
    IL_00ec:  ceq
    IL_00ee:  stloc.s    V_11
    IL_00f0:  ldloc.s    V_11
    IL_00f2:  brfalse.s  IL_0101

    IL_00f4:  nop
    IL_00f5:  ldstr      "0, 1"
    IL_00fa:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00ff:  nop
    IL_0100:  nop
    IL_0101:  ldc.i4.0
    IL_0102:  stloc.s    V_5
    IL_0104:  br.s       IL_010b

    IL_0106:  ldc.i4.1
    IL_0107:  stloc.s    V_5
    IL_0109:  br.s       IL_010b

    IL_010b:  ldloc.s    V_5
    IL_010d:  ret
  } // end of method Switch::SwitchlikeIf

  .method public hidebysig static bool  SwitchlikeIf2(int32 i) cil managed
  {
    // Code size       74 (0x4a)
    .maxstack  2
    .locals init (bool V_0,
             bool V_1,
             bool V_2,
             bool V_3,
             bool V_4)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  cgt.un
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  brfalse.s  IL_0042

    IL_0009:  nop
    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.1
    IL_000c:  ceq
    IL_000e:  stloc.1
    IL_000f:  ldloc.1
    IL_0010:  brfalse.s  IL_001b

    IL_0012:  nop
    IL_0013:  ldc.i4.1
    IL_0014:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0019:  nop
    IL_001a:  nop
    IL_001b:  ldarg.0
    IL_001c:  ldc.i4.2
    IL_001d:  ceq
    IL_001f:  stloc.2
    IL_0020:  ldloc.2
    IL_0021:  brfalse.s  IL_002c

    IL_0023:  nop
    IL_0024:  ldc.i4.2
    IL_0025:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_002a:  nop
    IL_002b:  nop
    IL_002c:  ldarg.0
    IL_002d:  ldc.i4.3
    IL_002e:  ceq
    IL_0030:  stloc.3
    IL_0031:  ldloc.3
    IL_0032:  brfalse.s  IL_003d

    IL_0034:  nop
    IL_0035:  ldc.i4.3
    IL_0036:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_003b:  nop
    IL_003c:  nop
    IL_003d:  ldc.i4.0
    IL_003e:  stloc.s    V_4
    IL_0040:  br.s       IL_0047

    IL_0042:  ldc.i4.0
    IL_0043:  stloc.s    V_4
    IL_0045:  br.s       IL_0047

    IL_0047:  ldloc.s    V_4
    IL_0049:  ret
  } // end of method Switch::SwitchlikeIf2

  .method public hidebysig static void  SingleIntervalIf(char c) cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.s   65
    IL_0004:  blt.s      IL_0010

    IL_0006:  ldarg.0
    IL_0007:  ldc.i4.s   90
    IL_0009:  cgt
    IL_000b:  ldc.i4.0
    IL_000c:  ceq
    IL_000e:  br.s       IL_0011

    IL_0010:  ldc.i4.0
    IL_0011:  stloc.0
    IL_0012:  ldloc.0
    IL_0013:  brfalse.s  IL_0022

    IL_0015:  nop
    IL_0016:  ldstr      "alphabet"
    IL_001b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0020:  nop
    IL_0021:  nop
    IL_0022:  ldstr      "end"
    IL_0027:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002c:  nop
    IL_002d:  ret
  } // end of method Switch::SingleIntervalIf

  .method public hidebysig static bool  Loop8(char c,
                                              bool b,
                                              class [mscorlib]System.Func`1<char> getChar) cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  2
    .locals init (bool V_0,
             bool V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  brfalse.s  IL_0035

    IL_0006:  nop
    IL_0007:  br.s       IL_0013

    IL_0009:  nop
    IL_000a:  ldarg.2
    IL_000b:  callvirt   instance !0 class [mscorlib]System.Func`1<char>::Invoke()
    IL_0010:  starg.s    c
    IL_0012:  nop
    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.s   97
    IL_0016:  blt.s      IL_001d

    IL_0018:  ldarg.0
    IL_0019:  ldc.i4.s   122
    IL_001b:  ble.s      IL_002f

    IL_001d:  ldarg.0
    IL_001e:  ldc.i4.s   65
    IL_0020:  blt.s      IL_002c

    IL_0022:  ldarg.0
    IL_0023:  ldc.i4.s   90
    IL_0025:  cgt
    IL_0027:  ldc.i4.0
    IL_0028:  ceq
    IL_002a:  br.s       IL_002d

    IL_002c:  ldc.i4.0
    IL_002d:  br.s       IL_0030

    IL_002f:  ldc.i4.1
    IL_0030:  stloc.1
    IL_0031:  ldloc.1
    IL_0032:  brtrue.s   IL_0009

    IL_0034:  nop
    IL_0035:  ldc.i4.1
    IL_0036:  stloc.2
    IL_0037:  br.s       IL_0039

    IL_0039:  ldloc.2
    IL_003a:  ret
  } // end of method Switch::Loop8

  .method public hidebysig static void  Loop9(class [mscorlib]System.Func`1<char> getChar) cil managed
  {
    // Code size       46 (0x2e)
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
    IL_0029:  stloc.1
    IL_002a:  ldloc.1
    IL_002b:  brtrue.s   IL_0001

    IL_002d:  ret
  } // end of method Switch::Loop9

  .method public hidebysig static void  SwitchWithBreakCase(int32 i,
                                                            bool b) cil managed
  {
    // Code size       69 (0x45)
    .maxstack  2
    .locals init (bool V_0,
             int32 V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  brfalse.s  IL_0039

    IL_0006:  nop
    IL_0007:  ldarg.0
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldc.i4.1
    IL_000b:  beq.s      IL_0015

    IL_000d:  br.s       IL_000f

    IL_000f:  ldloc.1
    IL_0010:  ldc.i4.2
    IL_0011:  beq.s      IL_002b

    IL_0013:  br.s       IL_001e

    IL_0015:  ldc.i4.1
    IL_0016:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001b:  nop
    IL_001c:  br.s       IL_002d

    IL_001e:  ldstr      "default"
    IL_0023:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0028:  nop
    IL_0029:  br.s       IL_002d

    IL_002b:  br.s       IL_002d

    IL_002d:  ldstr      "b"
    IL_0032:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0037:  nop
    IL_0038:  nop
    IL_0039:  ldstr      "end"
    IL_003e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0043:  nop
    IL_0044:  ret
  } // end of method Switch::SwitchWithBreakCase

  .method public hidebysig static void  SwitchWithReturnAndBreak(int32 i,
                                                                 bool b) cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  brfalse.s  IL_000e

    IL_0006:  br.s       IL_0008

    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.1
    IL_000a:  beq.s      IL_0018

    IL_000c:  br.s       IL_0025

    IL_000e:  ldarg.1
    IL_000f:  stloc.1
    IL_0010:  ldloc.1
    IL_0011:  brfalse.s  IL_0016

    IL_0013:  nop
    IL_0014:  br.s       IL_002b

    IL_0016:  br.s       IL_0025

    IL_0018:  ldarg.1
    IL_0019:  ldc.i4.0
    IL_001a:  ceq
    IL_001c:  stloc.2
    IL_001d:  ldloc.2
    IL_001e:  brfalse.s  IL_0023

    IL_0020:  nop
    IL_0021:  br.s       IL_002b

    IL_0023:  br.s       IL_0025

    IL_0025:  call       void [mscorlib]System.Console::WriteLine()
    IL_002a:  nop
    IL_002b:  ret
  } // end of method Switch::SwitchWithReturnAndBreak

  .method public hidebysig static int32  SwitchWithReturnAndBreak2(int32 i,
                                                                   bool b) cil managed
  {
    // Code size       106 (0x6a)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  ldc.i4     0x14e
    IL_0009:  bgt.s      IL_0022

    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.4
    IL_000d:  beq.s      IL_0040

    IL_000f:  br.s       IL_0011

    IL_0011:  ldloc.0
    IL_0012:  ldc.i4.s   33
    IL_0014:  beq.s      IL_0040

    IL_0016:  br.s       IL_0018

    IL_0018:  ldloc.0
    IL_0019:  ldc.i4     0x14e
    IL_001e:  beq.s      IL_004a

    IL_0020:  br.s       IL_005e

    IL_0022:  ldloc.0
    IL_0023:  ldc.i4     0x18b
    IL_0028:  beq.s      IL_0056

    IL_002a:  br.s       IL_002c

    IL_002c:  ldloc.0
    IL_002d:  ldc.i4     0x19a
    IL_0032:  beq.s      IL_0056

    IL_0034:  br.s       IL_0036

    IL_0036:  ldloc.0
    IL_0037:  ldc.i4     0x1c7
    IL_003c:  beq.s      IL_0056

    IL_003e:  br.s       IL_005e

    IL_0040:  call       void [mscorlib]System.Console::WriteLine()
    IL_0045:  nop
    IL_0046:  ldc.i4.1
    IL_0047:  stloc.1
    IL_0048:  br.s       IL_0068

    IL_004a:  ldarg.1
    IL_004b:  stloc.2
    IL_004c:  ldloc.2
    IL_004d:  brfalse.s  IL_0054

    IL_004f:  nop
    IL_0050:  ldc.i4.2
    IL_0051:  stloc.1
    IL_0052:  br.s       IL_0068

    IL_0054:  br.s       IL_005e

    IL_0056:  call       void [mscorlib]System.Console::WriteLine()
    IL_005b:  nop
    IL_005c:  br.s       IL_005e

    IL_005e:  call       void [mscorlib]System.Console::WriteLine()
    IL_0063:  nop
    IL_0064:  ldc.i4.0
    IL_0065:  stloc.1
    IL_0066:  br.s       IL_0068

    IL_0068:  ldloc.1
    IL_0069:  ret
  } // end of method Switch::SwitchWithReturnAndBreak2

  .method public hidebysig static void  SwitchWithReturnAndBreak3(int32 i) cil managed
  {
    // Code size       41 (0x29)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  brfalse.s  IL_0010

    IL_0006:  br.s       IL_0008

    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.1
    IL_000a:  beq.s      IL_0019

    IL_000c:  br.s       IL_000e

    IL_000e:  br.s       IL_0028

    IL_0010:  ldc.i4.0
    IL_0011:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0016:  nop
    IL_0017:  br.s       IL_0022

    IL_0019:  ldc.i4.1
    IL_001a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001f:  nop
    IL_0020:  br.s       IL_0022

    IL_0022:  call       void [mscorlib]System.Console::WriteLine()
    IL_0027:  nop
    IL_0028:  ret
  } // end of method Switch::SwitchWithReturnAndBreak3

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
