



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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

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

  .method public hidebysig static void  SparseIntegerSwitch2(int32 i) cil managed
  {
    // Code size       67 (0x43)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.s   21
    IL_0003:  bgt.s      IL_0021

    IL_0005:  ldarg.0
    IL_0006:  ldc.i4.s   11
    IL_0008:  bgt.s      IL_0016

    IL_000a:  ldarg.0
    IL_000b:  ldc.i4.4
    IL_000c:  beq.s      IL_003d

    IL_000e:  ldarg.0
    IL_000f:  ldc.i4.s   10
    IL_0011:  sub
    IL_0012:  ldc.i4.1
    IL_0013:  ble.un.s   IL_003d

    IL_0015:  ret

    IL_0016:  ldarg.0
    IL_0017:  ldc.i4.s   13
    IL_0019:  beq.s      IL_003d

    IL_001b:  ldarg.0
    IL_001c:  ldc.i4.s   21
    IL_001e:  beq.s      IL_003d

    IL_0020:  ret

    IL_0021:  ldarg.0
    IL_0022:  ldc.i4.s   33
    IL_0024:  bgt.s      IL_0031

    IL_0026:  ldarg.0
    IL_0027:  ldc.i4.s   29
    IL_0029:  beq.s      IL_003d

    IL_002b:  ldarg.0
    IL_002c:  ldc.i4.s   33
    IL_002e:  beq.s      IL_003d

    IL_0030:  ret

    IL_0031:  ldarg.0
    IL_0032:  ldc.i4.s   49
    IL_0034:  sub
    IL_0035:  ldc.i4.1
    IL_0036:  ble.un.s   IL_003d

    IL_0038:  ldarg.0
    IL_0039:  ldc.i4.s   55
    IL_003b:  bne.un.s   IL_0042

    IL_003d:  call       void [mscorlib]System.Console::WriteLine()
    IL_0042:  ret
  } // end of method Switch::SparseIntegerSwitch2

  .method public hidebysig static bool  SparseIntegerSwitch3(int32 i) cil managed
  {
    // Code size       36 (0x24)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.s   12
    IL_0003:  bgt.s      IL_0011

    IL_0005:  ldarg.0
    IL_0006:  brfalse.s  IL_0020

    IL_0008:  ldarg.0
    IL_0009:  ldc.i4.s   10
    IL_000b:  sub
    IL_000c:  ldc.i4.2
    IL_000d:  ble.un.s   IL_0020

    IL_000f:  br.s       IL_0022

    IL_0011:  ldarg.0
    IL_0012:  ldc.i4.s   100
    IL_0014:  sub
    IL_0015:  ldc.i4.1
    IL_0016:  ble.un.s   IL_0020

    IL_0018:  ldarg.0
    IL_0019:  ldc.i4     0xc8
    IL_001e:  bne.un.s   IL_0022

    IL_0020:  ldc.i4.1
    IL_0021:  ret

    IL_0022:  ldc.i4.0
    IL_0023:  ret
  } // end of method Switch::SparseIntegerSwitch3

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

  .method public hidebysig static void  CompactSwitchOverInt(int32 i) cil managed
  {
    // Code size       55 (0x37)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.2
    IL_0002:  ble.un.s   IL_000a

    IL_0004:  ldarg.0
    IL_0005:  ldc.i4.3
    IL_0006:  beq.s      IL_0016

    IL_0008:  br.s       IL_0022

    IL_000a:  ldstr      "012"
    IL_000f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0014:  br.s       IL_002c

    IL_0016:  ldstr      "3"
    IL_001b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0020:  br.s       IL_002c

    IL_0022:  ldstr      "default"
    IL_0027:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002c:  ldstr      "end"
    IL_0031:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0036:  ret
  } // end of method Switch::CompactSwitchOverInt

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

  .method public hidebysig static void  SwitchWithGotoString(string s) cil managed
  {
    // Code size       443 (0x1bb)
    .maxstack  2
    .locals init (uint32 V_0)
    IL_0000:  ldstr      "SwitchWithGotoString: "
    IL_0005:  ldarg.0
    IL_0006:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0010:  ldarg.0
    IL_0011:  call       uint32 '<PrivateImplementationDetails>'::ComputeStringHash(string)
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4     0x330ca589
    IL_001d:  bgt.un.s   IL_005d

    IL_001f:  ldloc.0
    IL_0020:  ldc.i4     0x310ca263
    IL_0025:  bgt.un.s   IL_0042

    IL_0027:  ldloc.0
    IL_0028:  ldc.i4     0x300ca0d0
    IL_002d:  beq        IL_00ee

    IL_0032:  ldloc.0
    IL_0033:  ldc.i4     0x310ca263
    IL_0038:  beq        IL_00dc

    IL_003d:  br         IL_01a6

    IL_0042:  ldloc.0
    IL_0043:  ldc.i4     0x320ca3f6
    IL_0048:  beq        IL_0112

    IL_004d:  ldloc.0
    IL_004e:  ldc.i4     0x330ca589
    IL_0053:  beq        IL_0100

    IL_0058:  br         IL_01a6

    IL_005d:  ldloc.0
    IL_005e:  ldc.i4     0x360caa42
    IL_0063:  bgt.un.s   IL_007a

    IL_0065:  ldloc.0
    IL_0066:  ldc.i4     0x340ca71c
    IL_006b:  beq.s      IL_009d

    IL_006d:  ldloc.0
    IL_006e:  ldc.i4     0x360caa42
    IL_0073:  beq.s      IL_00c7

    IL_0075:  br         IL_01a6

    IL_007a:  ldloc.0
    IL_007b:  ldc.i4     0x370cabd5
    IL_0080:  beq.s      IL_00b2

    IL_0082:  ldloc.0
    IL_0083:  ldc.i4     0x3c0cb3b4
    IL_0088:  beq        IL_0133

    IL_008d:  ldloc.0
    IL_008e:  ldc.i4     0x3d0cb547
    IL_0093:  beq        IL_0124

    IL_0098:  br         IL_01a6

    IL_009d:  ldarg.0
    IL_009e:  ldstr      "1"
    IL_00a3:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00a8:  brtrue     IL_0142

    IL_00ad:  br         IL_01a6

    IL_00b2:  ldarg.0
    IL_00b3:  ldstr      "2"
    IL_00b8:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00bd:  brtrue     IL_014e

    IL_00c2:  br         IL_01a6

    IL_00c7:  ldarg.0
    IL_00c8:  ldstr      "3"
    IL_00cd:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00d2:  brtrue     IL_0158

    IL_00d7:  br         IL_01a6

    IL_00dc:  ldarg.0
    IL_00dd:  ldstr      "4"
    IL_00e2:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00e7:  brtrue.s   IL_0164

    IL_00e9:  br         IL_01a6

    IL_00ee:  ldarg.0
    IL_00ef:  ldstr      "5"
    IL_00f4:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00f9:  brtrue.s   IL_016f

    IL_00fb:  br         IL_01a6

    IL_0100:  ldarg.0
    IL_0101:  ldstr      "6"
    IL_0106:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_010b:  brtrue.s   IL_017a

    IL_010d:  br         IL_01a6

    IL_0112:  ldarg.0
    IL_0113:  ldstr      "7"
    IL_0118:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_011d:  brtrue.s   IL_0185

    IL_011f:  br         IL_01a6

    IL_0124:  ldarg.0
    IL_0125:  ldstr      "8"
    IL_012a:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_012f:  brtrue.s   IL_0190

    IL_0131:  br.s       IL_01a6

    IL_0133:  ldarg.0
    IL_0134:  ldstr      "9"
    IL_0139:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_013e:  brtrue.s   IL_019b

    IL_0140:  br.s       IL_01a6

    IL_0142:  ldstr      "one"
    IL_0147:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_014c:  br.s       IL_01a6

    IL_014e:  ldstr      "two"
    IL_0153:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0158:  ldstr      "three"
    IL_015d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0162:  br.s       IL_01b0

    IL_0164:  ldstr      "four"
    IL_0169:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_016e:  ret

    IL_016f:  ldstr      "five"
    IL_0174:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0179:  ret

    IL_017a:  ldstr      "six"
    IL_017f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0184:  ret

    IL_0185:  ldstr      "seven"
    IL_018a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_018f:  ret

    IL_0190:  ldstr      "eight"
    IL_0195:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_019a:  ret

    IL_019b:  ldstr      "nine"
    IL_01a0:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_01a5:  ret

    IL_01a6:  ldstr      "default"
    IL_01ab:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_01b0:  ldstr      "End of method"
    IL_01b5:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_01ba:  ret
  } // end of method Switch::SwitchWithGotoString

  .method public hidebysig static void  SwitchWithGotoComplex(string s) cil managed
  {
    // Code size       387 (0x183)
    .maxstack  2
    .locals init (uint32 V_0)
    IL_0000:  ldstr      "SwitchWithGotoComplex: "
    IL_0005:  ldarg.0
    IL_0006:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0010:  ldarg.0
    IL_0011:  call       uint32 '<PrivateImplementationDetails>'::ComputeStringHash(string)
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4     0x330ca589
    IL_001d:  bgt.un.s   IL_005d

    IL_001f:  ldloc.0
    IL_0020:  ldc.i4     0x310ca263
    IL_0025:  bgt.un.s   IL_0042

    IL_0027:  ldloc.0
    IL_0028:  ldc.i4     0x300ca0d0
    IL_002d:  beq        IL_00d7

    IL_0032:  ldloc.0
    IL_0033:  ldc.i4     0x310ca263
    IL_0038:  beq        IL_00c5

    IL_003d:  br         IL_016e

    IL_0042:  ldloc.0
    IL_0043:  ldc.i4     0x320ca3f6
    IL_0048:  beq        IL_0107

    IL_004d:  ldloc.0
    IL_004e:  ldc.i4     0x330ca589
    IL_0053:  beq        IL_00e9

    IL_0058:  br         IL_016e

    IL_005d:  ldloc.0
    IL_005e:  ldc.i4     0x360caa42
    IL_0063:  bgt.un.s   IL_007a

    IL_0065:  ldloc.0
    IL_0066:  ldc.i4     0x340ca71c
    IL_006b:  beq.s      IL_008f

    IL_006d:  ldloc.0
    IL_006e:  ldc.i4     0x360caa42
    IL_0073:  beq.s      IL_00b3

    IL_0075:  br         IL_016e

    IL_007a:  ldloc.0
    IL_007b:  ldc.i4     0x370cabd5
    IL_0080:  beq.s      IL_00a1

    IL_0082:  ldloc.0
    IL_0083:  ldc.i4     0x3d0cb547
    IL_0088:  beq.s      IL_00f8

    IL_008a:  br         IL_016e

    IL_008f:  ldarg.0
    IL_0090:  ldstr      "1"
    IL_0095:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_009a:  brtrue.s   IL_0116

    IL_009c:  br         IL_016e

    IL_00a1:  ldarg.0
    IL_00a2:  ldstr      "2"
    IL_00a7:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00ac:  brtrue.s   IL_0122

    IL_00ae:  br         IL_016e

    IL_00b3:  ldarg.0
    IL_00b4:  ldstr      "3"
    IL_00b9:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00be:  brtrue.s   IL_012c

    IL_00c0:  br         IL_016e

    IL_00c5:  ldarg.0
    IL_00c6:  ldstr      "4"
    IL_00cb:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00d0:  brtrue.s   IL_0141

    IL_00d2:  br         IL_016e

    IL_00d7:  ldarg.0
    IL_00d8:  ldstr      "5"
    IL_00dd:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00e2:  brtrue.s   IL_014b

    IL_00e4:  br         IL_016e

    IL_00e9:  ldarg.0
    IL_00ea:  ldstr      "6"
    IL_00ef:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_00f4:  brtrue.s   IL_0157

    IL_00f6:  br.s       IL_016e

    IL_00f8:  ldarg.0
    IL_00f9:  ldstr      "8"
    IL_00fe:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0103:  brtrue.s   IL_0163

    IL_0105:  br.s       IL_016e

    IL_0107:  ldarg.0
    IL_0108:  ldstr      "7"
    IL_010d:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0112:  brtrue.s   IL_0178

    IL_0114:  br.s       IL_016e

    IL_0116:  ldstr      "one"
    IL_011b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0120:  br.s       IL_0163

    IL_0122:  ldstr      "two"
    IL_0127:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_012c:  ldstr      "three"
    IL_0131:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0136:  ldarg.0
    IL_0137:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_013c:  ldc.i4.2
    IL_013d:  beq.s      IL_014b

    IL_013f:  br.s       IL_0178

    IL_0141:  ldstr      "four"
    IL_0146:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_014b:  ldstr      "five"
    IL_0150:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0155:  br.s       IL_0163

    IL_0157:  ldstr      "six"
    IL_015c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0161:  br.s       IL_014b

    IL_0163:  ldstr      "eight"
    IL_0168:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_016d:  ret

    IL_016e:  ldstr      "default"
    IL_0173:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0178:  ldstr      "End of method"
    IL_017d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0182:  ret
  } // end of method Switch::SwitchWithGotoComplex

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

  .method public hidebysig static void  SwitchInTryBlock(string 'value') cil managed
  {
    // Code size       174 (0xae)
    .maxstack  2
    .locals init (string V_0)
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.5
      IL_0002:  callvirt   instance string [mscorlib]System.String::Substring(int32)
      IL_0007:  stloc.0
      IL_0008:  ldloc.0
      IL_0009:  ldstr      "Name1"
      IL_000e:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                     string)
      IL_0013:  brtrue.s   IL_0058

      IL_0015:  ldloc.0
      IL_0016:  ldstr      "Name2"
      IL_001b:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                     string)
      IL_0020:  brtrue.s   IL_0064

      IL_0022:  ldloc.0
      IL_0023:  ldstr      "Name3"
      IL_0028:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                     string)
      IL_002d:  brtrue.s   IL_0070

      IL_002f:  ldloc.0
      IL_0030:  ldstr      "Name4"
      IL_0035:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                     string)
      IL_003a:  brtrue.s   IL_007c

      IL_003c:  ldloc.0
      IL_003d:  ldstr      "Name5"
      IL_0042:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                     string)
      IL_0047:  brtrue.s   IL_0088

      IL_0049:  ldloc.0
      IL_004a:  ldstr      "Name6"
      IL_004f:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                     string)
      IL_0054:  brtrue.s   IL_0088

      IL_0056:  br.s       IL_0094

      IL_0058:  ldstr      "1"
      IL_005d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0062:  br.s       IL_009e

      IL_0064:  ldstr      "Name_2"
      IL_0069:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_006e:  br.s       IL_009e

      IL_0070:  ldstr      "Name_3"
      IL_0075:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_007a:  br.s       IL_009e

      IL_007c:  ldstr      "No. 4"
      IL_0081:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0086:  br.s       IL_009e

      IL_0088:  ldstr      "5+6"
      IL_008d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0092:  br.s       IL_009e

      IL_0094:  ldstr      "default"
      IL_0099:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_009e:  leave.s    IL_00ad

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_00a0:  pop
      IL_00a1:  ldstr      "catch block"
      IL_00a6:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00ab:  leave.s    IL_00ad

    }  // end handler
    IL_00ad:  ret
  } // end of method Switch::SwitchInTryBlock

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

  .method public hidebysig static void  SwitchWithContinue1(int32 i,
                                                            bool b) cil managed
  {
    // Code size       35 (0x23)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  switch     ( 
                          IL_0014,
                          IL_0000,
                          IL_0019)
    IL_0012:  br.s       IL_001c

    IL_0014:  ldarg.1
    IL_0015:  brfalse.s  IL_001c

    IL_0017:  br.s       IL_0000

    IL_0019:  ldarg.1
    IL_001a:  brfalse.s  IL_0000

    IL_001c:  call       void [mscorlib]System.Console::WriteLine()
    IL_0021:  br.s       IL_0000
  } // end of method Switch::SwitchWithContinue1

  .method public hidebysig static void  SwitchWithContinue2(int32 i,
                                                            bool b) cil managed
  {
    // Code size       110 (0x6e)
    .maxstack  2
    IL_0000:  br.s       IL_0068

    IL_0002:  ldarg.0
    IL_0003:  switch     ( 
                          IL_001a,
                          IL_0068,
                          IL_0035,
                          IL_0059)
    IL_0018:  br.s       IL_004f

    IL_001a:  ldarg.1
    IL_001b:  brfalse.s  IL_0029

    IL_001d:  ldstr      "0b"
    IL_0022:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0027:  br.s       IL_0068

    IL_0029:  ldstr      "0!b"
    IL_002e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0033:  br.s       IL_0059

    IL_0035:  ldarg.1
    IL_0036:  brfalse.s  IL_0043

    IL_0038:  ldstr      "2b"
    IL_003d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0042:  ret

    IL_0043:  ldstr      "2!b"
    IL_0048:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004d:  br.s       IL_0068

    IL_004f:  ldstr      "default"
    IL_0054:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0059:  ldstr      "loop-tail"
    IL_005e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0063:  ldarg.0
    IL_0064:  ldc.i4.1
    IL_0065:  add
    IL_0066:  starg.s    i
    IL_0068:  ldarg.0
    IL_0069:  ldc.i4.s   10
    IL_006b:  blt.s      IL_0002

    IL_006d:  ret
  } // end of method Switch::SwitchWithContinue2

  .method public hidebysig static void  SwitchWithContinue3(bool b) cil managed
  {
    // Code size       111 (0x6f)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0069

    IL_0004:  ldloc.0
    IL_0005:  switch     ( 
                          IL_001c,
                          IL_0065,
                          IL_0037,
                          IL_005b)
    IL_001a:  br.s       IL_0051

    IL_001c:  ldarg.0
    IL_001d:  brfalse.s  IL_002b

    IL_001f:  ldstr      "0b"
    IL_0024:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0029:  br.s       IL_0065

    IL_002b:  ldstr      "0!b"
    IL_0030:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0035:  br.s       IL_005b

    IL_0037:  ldarg.0
    IL_0038:  brfalse.s  IL_0045

    IL_003a:  ldstr      "2b"
    IL_003f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0044:  ret

    IL_0045:  ldstr      "2!b"
    IL_004a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004f:  br.s       IL_0065

    IL_0051:  ldstr      "default"
    IL_0056:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005b:  ldstr      "loop-tail"
    IL_0060:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0065:  ldloc.0
    IL_0066:  ldc.i4.1
    IL_0067:  add
    IL_0068:  stloc.0
    IL_0069:  ldloc.0
    IL_006a:  ldc.i4.s   10
    IL_006c:  blt.s      IL_0004

    IL_006e:  ret
  } // end of method Switch::SwitchWithContinue3

  .method public hidebysig static void  SwitchWithContinue4(bool b) cil managed
  {
    // Code size       188 (0xbc)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_0,
             int32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  ldc.i4.s   10
    IL_0003:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [System.Core]System.Linq.Enumerable::Range(int32,
                                                                                                                                    int32)
    IL_0008:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_000d:  stloc.0
    .try
    {
      IL_000e:  br         IL_00a4

      IL_0013:  ldloc.0
      IL_0014:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0019:  stloc.1
      IL_001a:  ldstr      "loop: "
      IL_001f:  ldloc.1
      IL_0020:  box        [mscorlib]System.Int32
      IL_0025:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_002a:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_002f:  ldloc.1
      IL_0030:  ldc.i4.1
      IL_0031:  sub
      IL_0032:  switch     ( 
                            IL_0059,
                            IL_00a4,
                            IL_005e,
                            IL_0063,
                            IL_006b,
                            IL_0073,
                            IL_0078,
                            IL_0080)
      IL_0057:  br.s       IL_0085

      IL_0059:  ldarg.0
      IL_005a:  brfalse.s  IL_008f

      IL_005c:  br.s       IL_00a4

      IL_005e:  ldarg.0
      IL_005f:  brfalse.s  IL_00a4

      IL_0061:  leave.s    IL_00bb

      IL_0063:  ldc.i4.4
      IL_0064:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_0069:  br.s       IL_0078

      IL_006b:  ldc.i4.5
      IL_006c:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_0071:  br.s       IL_0085

      IL_0073:  ldarg.0
      IL_0074:  brfalse.s  IL_005e

      IL_0076:  br.s       IL_00a4

      IL_0078:  ldloc.1
      IL_0079:  ldc.i4.2
      IL_007a:  rem
      IL_007b:  brfalse.s  IL_005e

      IL_007d:  ldarg.0
      IL_007e:  brfalse.s  IL_00a4

      IL_0080:  ldarg.0
      IL_0081:  brfalse.s  IL_006b

      IL_0083:  br.s       IL_00a4

      IL_0085:  ldstr      "default"
      IL_008a:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_008f:  ldstr      "break: "
      IL_0094:  ldloc.1
      IL_0095:  box        [mscorlib]System.Int32
      IL_009a:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_009f:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00a4:  ldloc.0
      IL_00a5:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_00aa:  brtrue     IL_0013

      IL_00af:  leave.s    IL_00bb

    }  // end .try
    finally
    {
      IL_00b1:  ldloc.0
      IL_00b2:  brfalse.s  IL_00ba

      IL_00b4:  ldloc.0
      IL_00b5:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_00ba:  endfinally
    }  // end handler
    IL_00bb:  ret
  } // end of method Switch::SwitchWithContinue4

  .method public hidebysig static void  SwitchWithContinue5(bool b) cil managed
  {
    // Code size       125 (0x7d)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0077

    IL_0004:  ldloc.0
    IL_0005:  ldc.i4.5
    IL_0006:  bge.s      IL_0069

    IL_0008:  ldloc.0
    IL_0009:  switch     ( 
                          IL_0020,
                          IL_0073,
                          IL_003b,
                          IL_005f)
    IL_001e:  br.s       IL_0055

    IL_0020:  ldarg.0
    IL_0021:  brfalse.s  IL_002f

    IL_0023:  ldstr      "0b"
    IL_0028:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002d:  br.s       IL_0073

    IL_002f:  ldstr      "0!b"
    IL_0034:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0039:  br.s       IL_005f

    IL_003b:  ldarg.0
    IL_003c:  brfalse.s  IL_0049

    IL_003e:  ldstr      "2b"
    IL_0043:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0048:  ret

    IL_0049:  ldstr      "2!b"
    IL_004e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0053:  br.s       IL_0073

    IL_0055:  ldstr      "default"
    IL_005a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005f:  ldstr      "break-target"
    IL_0064:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0069:  ldstr      "loop-tail"
    IL_006e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0073:  ldloc.0
    IL_0074:  ldc.i4.1
    IL_0075:  add
    IL_0076:  stloc.0
    IL_0077:  ldloc.0
    IL_0078:  ldc.i4.s   10
    IL_007a:  blt.s      IL_0004

    IL_007c:  ret
  } // end of method Switch::SwitchWithContinue5

  .method public hidebysig static void  SwitchWithContinue6(int32 i,
                                                            bool b) cil managed
  {
    // Code size       108 (0x6c)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  switch     ( 
                          IL_0018,
                          IL_0061,
                          IL_0033,
                          IL_0057)
    IL_0016:  br.s       IL_004d

    IL_0018:  ldarg.1
    IL_0019:  brtrue.s   IL_0027

    IL_001b:  ldstr      "0!b"
    IL_0020:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0025:  br.s       IL_0057

    IL_0027:  ldstr      "0b"
    IL_002c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0031:  br.s       IL_0061

    IL_0033:  ldarg.1
    IL_0034:  brfalse.s  IL_0041

    IL_0036:  ldstr      "2b"
    IL_003b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0040:  ret

    IL_0041:  ldstr      "2!b"
    IL_0046:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004b:  br.s       IL_0061

    IL_004d:  ldstr      "default"
    IL_0052:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0057:  ldstr      "loop-tail"
    IL_005c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0061:  ldarg.0
    IL_0062:  ldc.i4.1
    IL_0063:  add
    IL_0064:  dup
    IL_0065:  starg.s    i
    IL_0067:  ldc.i4.s   10
    IL_0069:  blt.s      IL_0000

    IL_006b:  ret
  } // end of method Switch::SwitchWithContinue6

  .method public hidebysig static void  SwitchWithContinue7() cil managed
  {
    // Code size       52 (0x34)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0025

    IL_0004:  ldstr      "loop-head"
    IL_0009:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000e:  ldloc.0
    IL_000f:  brfalse.s  IL_0021

    IL_0011:  ldloc.0
    IL_0012:  ldc.i4.1
    IL_0013:  beq.s      IL_0029

    IL_0015:  ldstr      "default"
    IL_001a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001f:  br.s       IL_0029

    IL_0021:  ldloc.0
    IL_0022:  ldc.i4.1
    IL_0023:  sub
    IL_0024:  stloc.0
    IL_0025:  ldloc.0
    IL_0026:  ldc.i4.0
    IL_0027:  bge.s      IL_0004

    IL_0029:  ldstr      "end"
    IL_002e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0033:  ret
  } // end of method Switch::SwitchWithContinue7

  .method public hidebysig static void  SwitchWithContinueInDoubleLoop() cil managed
  {
    // Code size       101 (0x65)
    .maxstack  2
    .locals init (bool V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_0059

    IL_0006:  ldc.i4.0
    IL_0007:  stloc.2
    IL_0008:  br.s       IL_0050

    IL_000a:  ldloc.1
    IL_000b:  ldloc.2
    IL_000c:  add
    IL_000d:  stloc.3
    IL_000e:  ldloc.3
    IL_000f:  ldc.i4.s   11
    IL_0011:  bgt.s      IL_003e

    IL_0013:  ldloc.3
    IL_0014:  ldc.i4.1
    IL_0015:  sub
    IL_0016:  switch     ( 
                          IL_0048,
                          IL_004c,
                          IL_0048,
                          IL_004c,
                          IL_0048,
                          IL_004c,
                          IL_0048)
    IL_0037:  ldloc.3
    IL_0038:  ldc.i4.s   11
    IL_003a:  beq.s      IL_0048

    IL_003c:  br.s       IL_004c

    IL_003e:  ldloc.3
    IL_003f:  ldc.i4.s   13
    IL_0041:  beq.s      IL_0048

    IL_0043:  ldloc.3
    IL_0044:  ldc.i4.s   17
    IL_0046:  bne.un.s   IL_004c

    IL_0048:  ldc.i4.1
    IL_0049:  stloc.0
    IL_004a:  br.s       IL_0055

    IL_004c:  ldloc.2
    IL_004d:  ldc.i4.1
    IL_004e:  add
    IL_004f:  stloc.2
    IL_0050:  ldloc.2
    IL_0051:  ldc.i4.s   10
    IL_0053:  blt.s      IL_000a

    IL_0055:  ldloc.1
    IL_0056:  ldc.i4.1
    IL_0057:  add
    IL_0058:  stloc.1
    IL_0059:  ldloc.1
    IL_005a:  ldc.i4.s   10
    IL_005c:  blt.s      IL_0006

    IL_005e:  ldloc.0
    IL_005f:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0064:  ret
  } // end of method Switch::SwitchWithContinueInDoubleLoop

  .method public hidebysig static void  SwitchLoopNesting() cil managed
  {
    // Code size       92 (0x5c)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0056

    IL_0004:  ldloc.0
    IL_0005:  brfalse.s  IL_000d

    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  beq.s      IL_0015

    IL_000b:  br.s       IL_001d

    IL_000d:  ldc.i4.0
    IL_000e:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0013:  br.s       IL_0038

    IL_0015:  ldc.i4.1
    IL_0016:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001b:  br.s       IL_0038

    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.2
    IL_001f:  rem
    IL_0020:  brtrue.s   IL_0033

    IL_0022:  br.s       IL_002e

    IL_0024:  ldloc.0
    IL_0025:  dup
    IL_0026:  ldc.i4.1
    IL_0027:  add
    IL_0028:  stloc.0
    IL_0029:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_002e:  ldloc.0
    IL_002f:  ldc.i4.3
    IL_0030:  rem
    IL_0031:  brtrue.s   IL_0024

    IL_0033:  call       void [mscorlib]System.Console::WriteLine()
    IL_0038:  ldloc.0
    IL_0039:  ldc.i4.4
    IL_003a:  ble.s      IL_0048

    IL_003c:  ldstr      "high"
    IL_0041:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0046:  br.s       IL_0052

    IL_0048:  ldstr      "low"
    IL_004d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0052:  ldloc.0
    IL_0053:  ldc.i4.1
    IL_0054:  add
    IL_0055:  stloc.0
    IL_0056:  ldloc.0
    IL_0057:  ldc.i4.s   10
    IL_0059:  blt.s      IL_0004

    IL_005b:  ret
  } // end of method Switch::SwitchLoopNesting

  .method public hidebysig static void  SingleIf1(int32 i,
                                                  bool a) cil managed
  {
    // Code size       25 (0x19)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  beq.s      IL_000c

    IL_0004:  ldarg.0
    IL_0005:  ldc.i4.2
    IL_0006:  ceq
    IL_0008:  ldarg.1
    IL_0009:  and
    IL_000a:  brfalse.s  IL_0012

    IL_000c:  ldc.i4.1
    IL_000d:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0012:  ldc.i4.2
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0018:  ret
  } // end of method Switch::SingleIf1

  .method public hidebysig static void  SingleIf2(int32 i,
                                                  bool a,
                                                  bool b) cil managed
  {
    // Code size       33 (0x21)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  beq.s      IL_0014

    IL_0004:  ldarg.0
    IL_0005:  ldc.i4.2
    IL_0006:  ceq
    IL_0008:  ldarg.1
    IL_0009:  and
    IL_000a:  brtrue.s   IL_0014

    IL_000c:  ldarg.0
    IL_000d:  ldc.i4.3
    IL_000e:  ceq
    IL_0010:  ldarg.2
    IL_0011:  and
    IL_0012:  brfalse.s  IL_001a

    IL_0014:  ldc.i4.1
    IL_0015:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001a:  ldc.i4.2
    IL_001b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0020:  ret
  } // end of method Switch::SingleIf2

  .method public hidebysig static void  SingleIf3(int32 i,
                                                  bool a,
                                                  bool b) cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000f

    IL_0003:  ldarg.0
    IL_0004:  ldc.i4.1
    IL_0005:  beq.s      IL_000f

    IL_0007:  ldarg.0
    IL_0008:  ldc.i4.2
    IL_0009:  ceq
    IL_000b:  ldarg.2
    IL_000c:  and
    IL_000d:  brfalse.s  IL_0015

    IL_000f:  ldc.i4.1
    IL_0010:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0015:  ldc.i4.2
    IL_0016:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001b:  ret
  } // end of method Switch::SingleIf3

  .method public hidebysig static void  SingleIf4(int32 i,
                                                  bool a) cil managed
  {
    // Code size       36 (0x24)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  beq.s      IL_0017

    IL_0004:  ldarg.0
    IL_0005:  ldc.i4.2
    IL_0006:  beq.s      IL_0017

    IL_0008:  ldarg.0
    IL_0009:  ldc.i4.3
    IL_000a:  ceq
    IL_000c:  ldc.i4.0
    IL_000d:  ceq
    IL_000f:  ldarg.1
    IL_0010:  and
    IL_0011:  brtrue.s   IL_0017

    IL_0013:  ldarg.0
    IL_0014:  ldc.i4.4
    IL_0015:  beq.s      IL_001d

    IL_0017:  ldc.i4.1
    IL_0018:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001d:  ldc.i4.2
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0023:  ret
  } // end of method Switch::SingleIf4

  .method public hidebysig static void  NestedIf(int32 i) cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  beq.s      IL_0018

    IL_0004:  ldarg.0
    IL_0005:  ldc.i4.2
    IL_0006:  bne.un.s   IL_000e

    IL_0008:  ldc.i4.2
    IL_0009:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000e:  ldstr      "default"
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0018:  call       void [mscorlib]System.Console::WriteLine()
    IL_001d:  ret
  } // end of method Switch::NestedIf

  .method public hidebysig static void  IfChainWithCondition(int32 i) cil managed
  {
    // Code size       98 (0x62)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  brtrue.s   IL_000b

    IL_0003:  ldc.i4.0
    IL_0004:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0009:  br.s       IL_005c

    IL_000b:  ldarg.0
    IL_000c:  ldc.i4.1
    IL_000d:  bne.un.s   IL_0017

    IL_000f:  ldc.i4.1
    IL_0010:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0015:  br.s       IL_005c

    IL_0017:  ldarg.0
    IL_0018:  ldc.i4.2
    IL_0019:  bne.un.s   IL_0023

    IL_001b:  ldc.i4.2
    IL_001c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0021:  br.s       IL_005c

    IL_0023:  ldarg.0
    IL_0024:  ldc.i4.3
    IL_0025:  bne.un.s   IL_002f

    IL_0027:  ldc.i4.3
    IL_0028:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_002d:  br.s       IL_005c

    IL_002f:  ldarg.0
    IL_0030:  ldc.i4.4
    IL_0031:  bne.un.s   IL_003b

    IL_0033:  ldc.i4.4
    IL_0034:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0039:  br.s       IL_005c

    IL_003b:  ldarg.0
    IL_003c:  ldc.i4.5
    IL_003d:  bne.un.s   IL_0052

    IL_003f:  call       bool [mscorlib]System.Console::get_CapsLock()
    IL_0044:  brfalse.s  IL_0052

    IL_0046:  ldstr      "5A"
    IL_004b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0050:  br.s       IL_005c

    IL_0052:  ldstr      "default"
    IL_0057:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005c:  call       void [mscorlib]System.Console::WriteLine()
    IL_0061:  ret
  } // end of method Switch::IfChainWithCondition

  .method public hidebysig static bool  SwitchlikeIf(int32 i,
                                                     int32 j) cil managed
  {
    // Code size       148 (0x94)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0050

    IL_0003:  ldarg.1
    IL_0004:  brfalse.s  IL_0050

    IL_0006:  ldarg.0
    IL_0007:  ldc.i4.m1
    IL_0008:  bne.un.s   IL_0018

    IL_000a:  ldarg.1
    IL_000b:  ldc.i4.m1
    IL_000c:  bne.un.s   IL_0018

    IL_000e:  ldstr      "-1, -1"
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0018:  ldarg.0
    IL_0019:  ldc.i4.m1
    IL_001a:  bne.un.s   IL_002a

    IL_001c:  ldarg.1
    IL_001d:  ldc.i4.1
    IL_001e:  bne.un.s   IL_002a

    IL_0020:  ldstr      "-1, 1"
    IL_0025:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002a:  ldarg.0
    IL_002b:  ldc.i4.1
    IL_002c:  bne.un.s   IL_003c

    IL_002e:  ldarg.1
    IL_002f:  ldc.i4.m1
    IL_0030:  bne.un.s   IL_003c

    IL_0032:  ldstr      "1, -1"
    IL_0037:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003c:  ldarg.0
    IL_003d:  ldc.i4.1
    IL_003e:  bne.un.s   IL_004e

    IL_0040:  ldarg.1
    IL_0041:  ldc.i4.1
    IL_0042:  bne.un.s   IL_004e

    IL_0044:  ldstr      "1, 1"
    IL_0049:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004e:  ldc.i4.0
    IL_004f:  ret

    IL_0050:  ldarg.0
    IL_0051:  brfalse.s  IL_0071

    IL_0053:  ldarg.0
    IL_0054:  ldc.i4.m1
    IL_0055:  bne.un.s   IL_0061

    IL_0057:  ldstr      "-1, 0"
    IL_005c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0061:  ldarg.0
    IL_0062:  ldc.i4.1
    IL_0063:  bne.un.s   IL_006f

    IL_0065:  ldstr      "1, 0"
    IL_006a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006f:  ldc.i4.0
    IL_0070:  ret

    IL_0071:  ldarg.1
    IL_0072:  brfalse.s  IL_0092

    IL_0074:  ldarg.1
    IL_0075:  ldc.i4.m1
    IL_0076:  bne.un.s   IL_0082

    IL_0078:  ldstr      "0, -1"
    IL_007d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0082:  ldarg.1
    IL_0083:  ldc.i4.1
    IL_0084:  bne.un.s   IL_0090

    IL_0086:  ldstr      "0, 1"
    IL_008b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0090:  ldc.i4.0
    IL_0091:  ret

    IL_0092:  ldc.i4.1
    IL_0093:  ret
  } // end of method Switch::SwitchlikeIf

  .method public hidebysig static bool  SwitchlikeIf2(int32 i) cil managed
  {
    // Code size       37 (0x25)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0023

    IL_0003:  ldarg.0
    IL_0004:  ldc.i4.1
    IL_0005:  bne.un.s   IL_000d

    IL_0007:  ldc.i4.1
    IL_0008:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.2
    IL_000f:  bne.un.s   IL_0017

    IL_0011:  ldc.i4.2
    IL_0012:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0017:  ldarg.0
    IL_0018:  ldc.i4.3
    IL_0019:  bne.un.s   IL_0021

    IL_001b:  ldc.i4.3
    IL_001c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0021:  ldc.i4.0
    IL_0022:  ret

    IL_0023:  ldc.i4.0
    IL_0024:  ret
  } // end of method Switch::SwitchlikeIf2

  .method public hidebysig static void  SingleIntervalIf(char c) cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.s   65
    IL_0003:  blt.s      IL_0014

    IL_0005:  ldarg.0
    IL_0006:  ldc.i4.s   90
    IL_0008:  bgt.s      IL_0014

    IL_000a:  ldstr      "alphabet"
    IL_000f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0014:  ldstr      "end"
    IL_0019:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001e:  ret
  } // end of method Switch::SingleIntervalIf

  .method public hidebysig static bool  Loop8(char c,
                                              bool b,
                                              class [mscorlib]System.Func`1<char> getChar) cil managed
  {
    // Code size       35 (0x23)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  brfalse.s  IL_0021

    IL_0003:  br.s       IL_000d

    IL_0005:  ldarg.2
    IL_0006:  callvirt   instance !0 class [mscorlib]System.Func`1<char>::Invoke()
    IL_000b:  starg.s    c
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.s   97
    IL_0010:  blt.s      IL_0017

    IL_0012:  ldarg.0
    IL_0013:  ldc.i4.s   122
    IL_0015:  ble.s      IL_0005

    IL_0017:  ldarg.0
    IL_0018:  ldc.i4.s   65
    IL_001a:  blt.s      IL_0021

    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.s   90
    IL_001f:  ble.s      IL_0005

    IL_0021:  ldc.i4.1
    IL_0022:  ret
  } // end of method Switch::Loop8

  .method public hidebysig static void  Loop9(class [mscorlib]System.Func`1<char> getChar) cil managed
  {
    // Code size       33 (0x21)
    .maxstack  2
    .locals init (char V_0)
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !0 class [mscorlib]System.Func`1<char>::Invoke()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.m1
    IL_0009:  beq.s      IL_0020

    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.s   10
    IL_000e:  beq.s      IL_0020

    IL_0010:  ldloc.0
    IL_0011:  ldc.i4     0x2028
    IL_0016:  beq.s      IL_0020

    IL_0018:  ldloc.0
    IL_0019:  ldc.i4     0x2029
    IL_001e:  bne.un.s   IL_0000

    IL_0020:  ret
  } // end of method Switch::Loop9

  .method public hidebysig static void  SwitchWithBreakCase(int32 i,
                                                            bool b) cil managed
  {
    // Code size       52 (0x34)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  brfalse.s  IL_0029

    IL_0003:  ldarg.0
    IL_0004:  ldc.i4.1
    IL_0005:  beq.s      IL_000d

    IL_0007:  ldarg.0
    IL_0008:  ldc.i4.2
    IL_0009:  beq.s      IL_001f

    IL_000b:  br.s       IL_0015

    IL_000d:  ldc.i4.1
    IL_000e:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0013:  br.s       IL_001f

    IL_0015:  ldstr      "default"
    IL_001a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001f:  ldstr      "b"
    IL_0024:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0029:  ldstr      "end"
    IL_002e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0033:  ret
  } // end of method Switch::SwitchWithBreakCase

  .method public hidebysig static void  SwitchWithReturnAndBreak(int32 i,
                                                                 bool b) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0009

    IL_0003:  ldarg.0
    IL_0004:  ldc.i4.1
    IL_0005:  beq.s      IL_000d

    IL_0007:  br.s       IL_0011

    IL_0009:  ldarg.1
    IL_000a:  brfalse.s  IL_0011

    IL_000c:  ret

    IL_000d:  ldarg.1
    IL_000e:  brtrue.s   IL_0011

    IL_0010:  ret

    IL_0011:  call       void [mscorlib]System.Console::WriteLine()
    IL_0016:  ret
  } // end of method Switch::SwitchWithReturnAndBreak

  .method public hidebysig static int32  SwitchWithReturnAndBreak2(int32 i,
                                                                   bool b) cil managed
  {
    // Code size       77 (0x4d)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4     0x14e
    IL_0006:  bgt.s      IL_001b

    IL_0008:  ldarg.0
    IL_0009:  ldc.i4.4
    IL_000a:  beq.s      IL_0035

    IL_000c:  ldarg.0
    IL_000d:  ldc.i4.s   33
    IL_000f:  beq.s      IL_0035

    IL_0011:  ldarg.0
    IL_0012:  ldc.i4     0x14e
    IL_0017:  beq.s      IL_003c

    IL_0019:  br.s       IL_0046

    IL_001b:  ldarg.0
    IL_001c:  ldc.i4     0x18b
    IL_0021:  beq.s      IL_0041

    IL_0023:  ldarg.0
    IL_0024:  ldc.i4     0x19a
    IL_0029:  beq.s      IL_0041

    IL_002b:  ldarg.0
    IL_002c:  ldc.i4     0x1c7
    IL_0031:  beq.s      IL_0041

    IL_0033:  br.s       IL_0046

    IL_0035:  call       void [mscorlib]System.Console::WriteLine()
    IL_003a:  ldc.i4.1
    IL_003b:  ret

    IL_003c:  ldarg.1
    IL_003d:  brfalse.s  IL_0046

    IL_003f:  ldc.i4.2
    IL_0040:  ret

    IL_0041:  call       void [mscorlib]System.Console::WriteLine()
    IL_0046:  call       void [mscorlib]System.Console::WriteLine()
    IL_004b:  ldc.i4.0
    IL_004c:  ret
  } // end of method Switch::SwitchWithReturnAndBreak2

  .method public hidebysig static void  SwitchWithReturnAndBreak3(int32 i) cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  brfalse.s  IL_0008

    IL_0003:  ldarg.0
    IL_0004:  ldc.i4.1
    IL_0005:  beq.s      IL_0010

    IL_0007:  ret

    IL_0008:  ldc.i4.0
    IL_0009:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_000e:  br.s       IL_0016

    IL_0010:  ldc.i4.1
    IL_0011:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0016:  call       void [mscorlib]System.Console::WriteLine()
    IL_001b:  ret
  } // end of method Switch::SwitchWithReturnAndBreak3

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
