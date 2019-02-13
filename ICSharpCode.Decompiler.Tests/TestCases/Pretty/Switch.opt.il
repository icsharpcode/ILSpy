



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
.assembly Switch.opt
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module Switch.opt.dll
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
    // Code size       44 (0x2c)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  ldarga.s   'value'
    IL_0002:  dup
    IL_0003:  call       instance !0 valuetype [mscorlib]System.Nullable`1<bool>::GetValueOrDefault()
    IL_0008:  stloc.0
    IL_0009:  call       instance bool valuetype [mscorlib]System.Nullable`1<bool>::get_HasValue()
    IL_000e:  brfalse.s  IL_0024

    IL_0010:  ldloc.0
    IL_0011:  switch     ( 
                          IL_0020,
                          IL_0022)
    IL_001e:  br.s       IL_0026

    IL_0020:  ldc.i4.0
    IL_0021:  ret

    IL_0022:  ldc.i4.1
    IL_0023:  ret

    IL_0024:  ldc.i4.2
    IL_0025:  ret

    IL_0026:  newobj     instance void [mscorlib]System.InvalidOperationException::.ctor()
    IL_002b:  throw
  } // end of method Switch::SwitchOverNullableBool

  .method public hidebysig static valuetype [mscorlib]System.Nullable`1<bool> 
          SwitchOverNullableEnum(valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State> state) cil managed
  {
    // Code size       66 (0x42)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State V_0,
             valuetype [mscorlib]System.Nullable`1<bool> V_1)
    IL_0000:  ldarga.s   state
    IL_0002:  dup
    IL_0003:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State>::GetValueOrDefault()
    IL_0008:  stloc.0
    IL_0009:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/State>::get_HasValue()
    IL_000e:  brfalse.s  IL_003c

    IL_0010:  ldloc.0
    IL_0011:  switch     ( 
                          IL_0024,
                          IL_002b,
                          IL_0032)
    IL_0022:  br.s       IL_003c

    IL_0024:  ldc.i4.0
    IL_0025:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_002a:  ret

    IL_002b:  ldc.i4.1
    IL_002c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<bool>::.ctor(!0)
    IL_0031:  ret

    IL_0032:  ldloca.s   V_1
    IL_0034:  initobj    valuetype [mscorlib]System.Nullable`1<bool>
    IL_003a:  ldloc.1
    IL_003b:  ret

    IL_003c:  newobj     instance void [mscorlib]System.InvalidOperationException::.ctor()
    IL_0041:  throw
  } // end of method Switch::SwitchOverNullableEnum

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

  .method public hidebysig static void  SparseIntegerSwitch2(int32 i) cil managed
  {
    // Code size       87 (0x57)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  ldc.i4.s   21
    IL_0005:  bgt.s      IL_002a

    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.4
    IL_0009:  beq.s      IL_0051

    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.s   10
    IL_000e:  sub
    IL_000f:  switch     ( 
                          IL_0051,
                          IL_0051,
                          IL_0056,
                          IL_0051)
    IL_0024:  ldloc.0
    IL_0025:  ldc.i4.s   21
    IL_0027:  beq.s      IL_0051

    IL_0029:  ret

    IL_002a:  ldloc.0
    IL_002b:  ldc.i4.s   33
    IL_002d:  bgt.s      IL_003a

    IL_002f:  ldloc.0
    IL_0030:  ldc.i4.s   29
    IL_0032:  beq.s      IL_0051

    IL_0034:  ldloc.0
    IL_0035:  ldc.i4.s   33
    IL_0037:  beq.s      IL_0051

    IL_0039:  ret

    IL_003a:  ldloc.0
    IL_003b:  ldc.i4.s   49
    IL_003d:  sub
    IL_003e:  switch     ( 
                          IL_0051,
                          IL_0051)
    IL_004b:  ldloc.0
    IL_004c:  ldc.i4.s   55
    IL_004e:  beq.s      IL_0051

    IL_0050:  ret

    IL_0051:  call       void [mscorlib]System.Console::WriteLine()
    IL_0056:  ret
  } // end of method Switch::SparseIntegerSwitch2

  .method public hidebysig static bool  SparseIntegerSwitch3(int32 i) cil managed
  {
    // Code size       63 (0x3f)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  ldc.i4.s   12
    IL_0005:  bgt.s      IL_0022

    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.0
    IL_0009:  beq.s      IL_003b

    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.s   10
    IL_000e:  sub
    IL_000f:  switch     ( 
                          IL_003b,
                          IL_003b,
                          IL_003b)
    IL_0020:  br.s       IL_003d

    IL_0022:  ldloc.0
    IL_0023:  ldc.i4.s   100
    IL_0025:  sub
    IL_0026:  switch     ( 
                          IL_003b,
                          IL_003b)
    IL_0033:  ldloc.0
    IL_0034:  ldc.i4     0xc8
    IL_0039:  bne.un.s   IL_003d

    IL_003b:  ldc.i4.1
    IL_003c:  ret

    IL_003d:  ldc.i4.0
    IL_003e:  ret
  } // end of method Switch::SparseIntegerSwitch3

  .method public hidebysig static string 
          SwitchOverNullableInt(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarga.s   i
    IL_0002:  dup
    IL_0003:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0008:  stloc.0
    IL_0009:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000e:  brfalse.s  IL_001f

    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.0
    IL_0012:  beq.s      IL_0025

    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.5
    IL_0016:  beq.s      IL_002b

    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.s   10
    IL_001b:  beq.s      IL_0031

    IL_001d:  br.s       IL_0037

    IL_001f:  ldstr      "null"
    IL_0024:  ret

    IL_0025:  ldstr      "zero"
    IL_002a:  ret

    IL_002b:  ldstr      "five"
    IL_0030:  ret

    IL_0031:  ldstr      "ten"
    IL_0036:  ret

    IL_0037:  ldstr      "large"
    IL_003c:  ret
  } // end of method Switch::SwitchOverNullableInt

  .method public hidebysig static string 
          SwitchOverNullableIntNullCaseCombined(valuetype [mscorlib]System.Nullable`1<int32> i) cil managed
  {
    // Code size       55 (0x37)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarga.s   i
    IL_0002:  dup
    IL_0003:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0008:  stloc.0
    IL_0009:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000e:  brfalse.s  IL_001f

    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.0
    IL_0012:  beq.s      IL_001f

    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.5
    IL_0016:  beq.s      IL_0025

    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.s   10
    IL_001b:  beq.s      IL_002b

    IL_001d:  br.s       IL_0031

    IL_001f:  ldstr      "zero"
    IL_0024:  ret

    IL_0025:  ldstr      "five"
    IL_002a:  ret

    IL_002b:  ldstr      "ten"
    IL_0030:  ret

    IL_0031:  ldstr      "large"
    IL_0036:  ret
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
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brtrue.s   IL_0016

    IL_000b:  ldloca.s   V_1
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0013:  ldloc.1
    IL_0014:  br.s       IL_0024

    IL_0016:  ldloca.s   V_0
    IL_0018:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001d:  ldc.i4.5
    IL_001e:  add
    IL_001f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0024:  stloc.2
    IL_0025:  ldloca.s   V_2
    IL_0027:  dup
    IL_0028:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_002d:  stloc.3
    IL_002e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0033:  brfalse.s  IL_0044

    IL_0035:  ldloc.3
    IL_0036:  ldc.i4.0
    IL_0037:  beq.s      IL_004a

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
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brtrue.s   IL_0016

    IL_000b:  ldloca.s   V_1
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0013:  ldloc.1
    IL_0014:  br.s       IL_0024

    IL_0016:  ldloca.s   V_0
    IL_0018:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001d:  ldc.i4.5
    IL_001e:  add
    IL_001f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0024:  stloc.2
    IL_0025:  ldloca.s   V_2
    IL_0027:  dup
    IL_0028:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_002d:  stloc.3
    IL_002e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0033:  brfalse.s  IL_0044

    IL_0035:  ldloc.3
    IL_0036:  ldc.i4.0
    IL_0037:  beq.s      IL_0044

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
    // Code size       55 (0x37)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarga.s   i
    IL_0002:  dup
    IL_0003:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_0008:  stloc.0
    IL_0009:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_000e:  brfalse.s  IL_0031

    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.0
    IL_0012:  beq.s      IL_001f

    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.5
    IL_0016:  beq.s      IL_0025

    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.s   10
    IL_001b:  beq.s      IL_002b

    IL_001d:  br.s       IL_0031

    IL_001f:  ldstr      "zero"
    IL_0024:  ret

    IL_0025:  ldstr      "five"
    IL_002a:  ret

    IL_002b:  ldstr      "ten"
    IL_0030:  ret

    IL_0031:  ldstr      "other"
    IL_0036:  ret
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
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0009:  brtrue.s   IL_0016

    IL_000b:  ldloca.s   V_1
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0013:  ldloc.1
    IL_0014:  br.s       IL_0024

    IL_0016:  ldloca.s   V_0
    IL_0018:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_001d:  ldc.i4.5
    IL_001e:  add
    IL_001f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0024:  stloc.2
    IL_0025:  ldloca.s   V_2
    IL_0027:  dup
    IL_0028:  call       instance !0 valuetype [mscorlib]System.Nullable`1<int32>::GetValueOrDefault()
    IL_002d:  stloc.3
    IL_002e:  call       instance bool valuetype [mscorlib]System.Nullable`1<int32>::get_HasValue()
    IL_0033:  brfalse.s  IL_0056

    IL_0035:  ldloc.3
    IL_0036:  ldc.i4.0
    IL_0037:  beq.s      IL_0044

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

  .method public hidebysig static void  CompactSwitchOverInt(int32 i) cil managed
  {
    // Code size       71 (0x47)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  switch     ( 
                          IL_001a,
                          IL_001a,
                          IL_001a,
                          IL_0026)
    IL_0018:  br.s       IL_0032

    IL_001a:  ldstr      "012"
    IL_001f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0024:  br.s       IL_003c

    IL_0026:  ldstr      "3"
    IL_002b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0030:  br.s       IL_003c

    IL_0032:  ldstr      "default"
    IL_0037:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003c:  ldstr      "end"
    IL_0041:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0046:  ret
  } // end of method Switch::CompactSwitchOverInt

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
          ShortSwitchOverStringWithNullCase(string text) cil managed
  {
    // Code size       73 (0x49)
    .maxstack  2
    .locals init (string V_0)
    IL_0000:  ldstr      "ShortSwitchOverStringWithNullCase: "
    IL_0005:  ldarg.0
    IL_0006:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0010:  ldarg.0
    IL_0011:  dup
    IL_0012:  stloc.0
    IL_0013:  brfalse.s  IL_003d

    IL_0015:  ldloc.0
    IL_0016:  ldstr      "First case"
    IL_001b:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0020:  brtrue.s   IL_0031

    IL_0022:  ldloc.0
    IL_0023:  ldstr      "Second case"
    IL_0028:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_002d:  brtrue.s   IL_0037

    IL_002f:  br.s       IL_0043

    IL_0031:  ldstr      "Text1"
    IL_0036:  ret

    IL_0037:  ldstr      "Text2"
    IL_003c:  ret

    IL_003d:  ldstr      "null"
    IL_0042:  ret

    IL_0043:  ldstr      "Default"
    IL_0048:  ret
  } // end of method Switch::ShortSwitchOverStringWithNullCase

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
    IL_001a:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000010-1'
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
    IL_007d:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000010-1'
    IL_0082:  volatile.
    IL_0084:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000010-1'
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
    IL_0018:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000011-1'
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
    IL_00b1:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000011-1'
    IL_00b6:  volatile.
    IL_00b8:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000011-1'
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

  .method public hidebysig static void  SwitchWithGotoString(string s) cil managed
  {
    // Code size       340 (0x154)
    .maxstack  4
    .locals init (string V_0,
             int32 V_1)
    IL_0000:  ldstr      "SwitchWithGotoString: "
    IL_0005:  ldarg.0
    IL_0006:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0010:  ldarg.0
    IL_0011:  dup
    IL_0012:  stloc.0
    IL_0013:  brfalse    IL_013f

    IL_0018:  volatile.
    IL_001a:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000015-1'
    IL_001f:  brtrue.s   IL_009b

    IL_0021:  ldc.i4.s   9
    IL_0023:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::.ctor(int32)
    IL_0028:  dup
    IL_0029:  ldstr      "1"
    IL_002e:  ldc.i4.0
    IL_002f:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0034:  dup
    IL_0035:  ldstr      "2"
    IL_003a:  ldc.i4.1
    IL_003b:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0040:  dup
    IL_0041:  ldstr      "3"
    IL_0046:  ldc.i4.2
    IL_0047:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_004c:  dup
    IL_004d:  ldstr      "4"
    IL_0052:  ldc.i4.3
    IL_0053:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0058:  dup
    IL_0059:  ldstr      "5"
    IL_005e:  ldc.i4.4
    IL_005f:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0064:  dup
    IL_0065:  ldstr      "6"
    IL_006a:  ldc.i4.5
    IL_006b:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0070:  dup
    IL_0071:  ldstr      "7"
    IL_0076:  ldc.i4.6
    IL_0077:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_007c:  dup
    IL_007d:  ldstr      "8"
    IL_0082:  ldc.i4.7
    IL_0083:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0088:  dup
    IL_0089:  ldstr      "9"
    IL_008e:  ldc.i4.8
    IL_008f:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0094:  volatile.
    IL_0096:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000015-1'
    IL_009b:  volatile.
    IL_009d:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000015-1'
    IL_00a2:  ldloc.0
    IL_00a3:  ldloca.s   V_1
    IL_00a5:  call       instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::TryGetValue(!0,
                                                                                                                          !1&)
    IL_00aa:  brfalse    IL_013f

    IL_00af:  ldloc.1
    IL_00b0:  switch     ( 
                          IL_00db,
                          IL_00e7,
                          IL_00f1,
                          IL_00fd,
                          IL_0108,
                          IL_0113,
                          IL_011e,
                          IL_0129,
                          IL_0134)
    IL_00d9:  br.s       IL_013f

    IL_00db:  ldstr      "one"
    IL_00e0:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00e5:  br.s       IL_013f

    IL_00e7:  ldstr      "two"
    IL_00ec:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00f1:  ldstr      "three"
    IL_00f6:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00fb:  br.s       IL_0149

    IL_00fd:  ldstr      "four"
    IL_0102:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0107:  ret

    IL_0108:  ldstr      "five"
    IL_010d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0112:  ret

    IL_0113:  ldstr      "six"
    IL_0118:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_011d:  ret

    IL_011e:  ldstr      "seven"
    IL_0123:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0128:  ret

    IL_0129:  ldstr      "eight"
    IL_012e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0133:  ret

    IL_0134:  ldstr      "nine"
    IL_0139:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_013e:  ret

    IL_013f:  ldstr      "default"
    IL_0144:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0149:  ldstr      "End of method"
    IL_014e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0153:  ret
  } // end of method Switch::SwitchWithGotoString

  .method public hidebysig static void  SwitchWithGotoComplex(string s) cil managed
  {
    // Code size       311 (0x137)
    .maxstack  4
    .locals init (string V_0,
             int32 V_1)
    IL_0000:  ldstr      "SwitchWithGotoComplex: "
    IL_0005:  ldarg.0
    IL_0006:  call       string [mscorlib]System.String::Concat(string,
                                                                string)
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0010:  ldarg.0
    IL_0011:  dup
    IL_0012:  stloc.0
    IL_0013:  brfalse    IL_0122

    IL_0018:  volatile.
    IL_001a:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000016-1'
    IL_001f:  brtrue.s   IL_008e

    IL_0021:  ldc.i4.8
    IL_0022:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::.ctor(int32)
    IL_0027:  dup
    IL_0028:  ldstr      "1"
    IL_002d:  ldc.i4.0
    IL_002e:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0033:  dup
    IL_0034:  ldstr      "2"
    IL_0039:  ldc.i4.1
    IL_003a:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_003f:  dup
    IL_0040:  ldstr      "3"
    IL_0045:  ldc.i4.2
    IL_0046:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_004b:  dup
    IL_004c:  ldstr      "4"
    IL_0051:  ldc.i4.3
    IL_0052:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0057:  dup
    IL_0058:  ldstr      "5"
    IL_005d:  ldc.i4.4
    IL_005e:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0063:  dup
    IL_0064:  ldstr      "6"
    IL_0069:  ldc.i4.5
    IL_006a:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_006f:  dup
    IL_0070:  ldstr      "8"
    IL_0075:  ldc.i4.6
    IL_0076:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_007b:  dup
    IL_007c:  ldstr      "7"
    IL_0081:  ldc.i4.7
    IL_0082:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0087:  volatile.
    IL_0089:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000016-1'
    IL_008e:  volatile.
    IL_0090:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000016-1'
    IL_0095:  ldloc.0
    IL_0096:  ldloca.s   V_1
    IL_0098:  call       instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::TryGetValue(!0,
                                                                                                                          !1&)
    IL_009d:  brfalse    IL_0122

    IL_00a2:  ldloc.1
    IL_00a3:  switch     ( 
                          IL_00ca,
                          IL_00d6,
                          IL_00e0,
                          IL_00f5,
                          IL_00ff,
                          IL_010b,
                          IL_0117,
                          IL_012c)
    IL_00c8:  br.s       IL_0122

    IL_00ca:  ldstr      "one"
    IL_00cf:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00d4:  br.s       IL_0117

    IL_00d6:  ldstr      "two"
    IL_00db:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00e0:  ldstr      "three"
    IL_00e5:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00ea:  ldarg.0
    IL_00eb:  callvirt   instance int32 [mscorlib]System.String::get_Length()
    IL_00f0:  ldc.i4.2
    IL_00f1:  beq.s      IL_00ff

    IL_00f3:  br.s       IL_012c

    IL_00f5:  ldstr      "four"
    IL_00fa:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_00ff:  ldstr      "five"
    IL_0104:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0109:  br.s       IL_0117

    IL_010b:  ldstr      "six"
    IL_0110:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0115:  br.s       IL_00ff

    IL_0117:  ldstr      "eight"
    IL_011c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0121:  ret

    IL_0122:  ldstr      "default"
    IL_0127:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_012c:  ldstr      "End of method"
    IL_0131:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0136:  ret
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
    // Code size       309 (0x135)
    .maxstack  4
    .locals init (class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty> V_0,
             class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty> V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty[] V_2,
             int32 V_3,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty V_4,
             string V_5,
             int32 V_6)
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::.ctor()
    IL_000b:  stloc.1
    IL_000c:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch::GetProperties()
    IL_0011:  stloc.2
    IL_0012:  ldc.i4.0
    IL_0013:  stloc.3
    IL_0014:  br         IL_012b

    IL_0019:  ldstr      "In for-loop"
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0023:  ldloc.2
    IL_0024:  ldloc.3
    IL_0025:  ldelem.ref
    IL_0026:  stloc.s    V_4
    IL_0028:  ldloc.s    V_4
    IL_002a:  ldfld      class [mscorlib]System.Reflection.PropertyInfo ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::Property
    IL_002f:  callvirt   instance string [mscorlib]System.Reflection.MemberInfo::get_Name()
    IL_0034:  dup
    IL_0035:  stloc.s    V_5
    IL_0037:  brfalse    IL_011f

    IL_003c:  volatile.
    IL_003e:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000018-1'
    IL_0043:  brtrue.s   IL_009a

    IL_0045:  ldc.i4.6
    IL_0046:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::.ctor(int32)
    IL_004b:  dup
    IL_004c:  ldstr      "Name1"
    IL_0051:  ldc.i4.0
    IL_0052:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0057:  dup
    IL_0058:  ldstr      "Name2"
    IL_005d:  ldc.i4.1
    IL_005e:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0063:  dup
    IL_0064:  ldstr      "Name3"
    IL_0069:  ldc.i4.2
    IL_006a:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_006f:  dup
    IL_0070:  ldstr      "Name4"
    IL_0075:  ldc.i4.3
    IL_0076:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_007b:  dup
    IL_007c:  ldstr      "Name5"
    IL_0081:  ldc.i4.4
    IL_0082:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0087:  dup
    IL_0088:  ldstr      "Name6"
    IL_008d:  ldc.i4.5
    IL_008e:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0093:  volatile.
    IL_0095:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000018-1'
    IL_009a:  volatile.
    IL_009c:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000018-1'
    IL_00a1:  ldloc.s    V_5
    IL_00a3:  ldloca.s   V_6
    IL_00a5:  call       instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::TryGetValue(!0,
                                                                                                                          !1&)
    IL_00aa:  brfalse.s  IL_011f

    IL_00ac:  ldloc.s    V_6
    IL_00ae:  switch     ( 
                          IL_00cd,
                          IL_00df,
                          IL_00f1,
                          IL_0103,
                          IL_0115,
                          IL_0115)
    IL_00cb:  br.s       IL_011f

    IL_00cd:  ldloc.s    V_4
    IL_00cf:  ldc.i4.1
    IL_00d0:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00d5:  ldloc.0
    IL_00d6:  ldloc.s    V_4
    IL_00d8:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00dd:  br.s       IL_0127

    IL_00df:  ldloc.s    V_4
    IL_00e1:  ldc.i4.2
    IL_00e2:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00e7:  ldloc.0
    IL_00e8:  ldloc.s    V_4
    IL_00ea:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_00ef:  br.s       IL_0127

    IL_00f1:  ldloc.s    V_4
    IL_00f3:  ldc.i4.3
    IL_00f4:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_00f9:  ldloc.0
    IL_00fa:  ldloc.s    V_4
    IL_00fc:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_0101:  br.s       IL_0127

    IL_0103:  ldloc.s    V_4
    IL_0105:  ldc.i4.4
    IL_0106:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty::set_Set(int32)
    IL_010b:  ldloc.0
    IL_010c:  ldloc.s    V_4
    IL_010e:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_0113:  br.s       IL_0127

    IL_0115:  ldloc.0
    IL_0116:  ldloc.s    V_4
    IL_0118:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_011d:  br.s       IL_0127

    IL_011f:  ldloc.1
    IL_0120:  ldloc.s    V_4
    IL_0122:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Switch/SetProperty>::Add(!0)
    IL_0127:  ldloc.3
    IL_0128:  ldc.i4.1
    IL_0129:  add
    IL_012a:  stloc.3
    IL_012b:  ldloc.3
    IL_012c:  ldloc.2
    IL_012d:  ldlen
    IL_012e:  conv.i4
    IL_012f:  blt        IL_0019

    IL_0134:  ret
  } // end of method Switch::SwitchOnStringInForLoop

  .method public hidebysig static void  SwitchInTryBlock(string 'value') cil managed
  {
    // Code size       243 (0xf3)
    .maxstack  4
    .locals init (string V_0,
             int32 V_1)
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.5
      IL_0002:  callvirt   instance string [mscorlib]System.String::Substring(int32)
      IL_0007:  dup
      IL_0008:  stloc.0
      IL_0009:  brfalse    IL_00d9

      IL_000e:  volatile.
      IL_0010:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000019-1'
      IL_0015:  brtrue.s   IL_006c

      IL_0017:  ldc.i4.6
      IL_0018:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::.ctor(int32)
      IL_001d:  dup
      IL_001e:  ldstr      "Name1"
      IL_0023:  ldc.i4.0
      IL_0024:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                    !1)
      IL_0029:  dup
      IL_002a:  ldstr      "Name2"
      IL_002f:  ldc.i4.1
      IL_0030:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                    !1)
      IL_0035:  dup
      IL_0036:  ldstr      "Name3"
      IL_003b:  ldc.i4.2
      IL_003c:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                    !1)
      IL_0041:  dup
      IL_0042:  ldstr      "Name4"
      IL_0047:  ldc.i4.3
      IL_0048:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                    !1)
      IL_004d:  dup
      IL_004e:  ldstr      "Name5"
      IL_0053:  ldc.i4.4
      IL_0054:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                    !1)
      IL_0059:  dup
      IL_005a:  ldstr      "Name6"
      IL_005f:  ldc.i4.5
      IL_0060:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                    !1)
      IL_0065:  volatile.
      IL_0067:  stsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000019-1'
      IL_006c:  volatile.
      IL_006e:  ldsfld     class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> '<PrivateImplementationDetails>'::'$$method0x6000019-1'
      IL_0073:  ldloc.0
      IL_0074:  ldloca.s   V_1
      IL_0076:  call       instance bool class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::TryGetValue(!0,
                                                                                                                            !1&)
      IL_007b:  brfalse.s  IL_00d9

      IL_007d:  ldloc.1
      IL_007e:  switch     ( 
                            IL_009d,
                            IL_00a9,
                            IL_00b5,
                            IL_00c1,
                            IL_00cd,
                            IL_00cd)
      IL_009b:  br.s       IL_00d9

      IL_009d:  ldstr      "1"
      IL_00a2:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00a7:  br.s       IL_00e3

      IL_00a9:  ldstr      "Name_2"
      IL_00ae:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00b3:  br.s       IL_00e3

      IL_00b5:  ldstr      "Name_3"
      IL_00ba:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00bf:  br.s       IL_00e3

      IL_00c1:  ldstr      "No. 4"
      IL_00c6:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00cb:  br.s       IL_00e3

      IL_00cd:  ldstr      "5+6"
      IL_00d2:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00d7:  br.s       IL_00e3

      IL_00d9:  ldstr      "default"
      IL_00de:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00e3:  leave.s    IL_00f2

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_00e5:  pop
      IL_00e6:  ldstr      "catch block"
      IL_00eb:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00f0:  leave.s    IL_00f2

    }  // end handler
    IL_00f2:  ret
  } // end of method Switch::SwitchInTryBlock

  .method public hidebysig static void  SwitchWithComplexCondition(string[] args) cil managed
  {
    // Code size       130 (0x82)
    .maxstack  2
    .locals init (string V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldlen
    IL_0002:  conv.i4
    IL_0003:  brfalse.s  IL_000a

    IL_0005:  ldarg.0
    IL_0006:  ldc.i4.0
    IL_0007:  ldelem.ref
    IL_0008:  br.s       IL_000f

    IL_000a:  ldstr      "dummy"
    IL_000f:  dup
    IL_0010:  stloc.0
    IL_0011:  brfalse.s  IL_0077

    IL_0013:  ldloc.0
    IL_0014:  ldstr      "a"
    IL_0019:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_001e:  brtrue.s   IL_0049

    IL_0020:  ldloc.0
    IL_0021:  ldstr      "b"
    IL_0026:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_002b:  brtrue.s   IL_0055

    IL_002d:  ldloc.0
    IL_002e:  ldstr      "c"
    IL_0033:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0038:  brtrue.s   IL_0061

    IL_003a:  ldloc.0
    IL_003b:  ldstr      "d"
    IL_0040:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0045:  brtrue.s   IL_006d

    IL_0047:  br.s       IL_0077

    IL_0049:  ldstr      "a"
    IL_004e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0053:  br.s       IL_0077

    IL_0055:  ldstr      "b"
    IL_005a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005f:  br.s       IL_0077

    IL_0061:  ldstr      "c"
    IL_0066:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006b:  br.s       IL_0077

    IL_006d:  ldstr      "d"
    IL_0072:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0077:  ldstr      "end"
    IL_007c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0081:  ret
  } // end of method Switch::SwitchWithComplexCondition

  .method public hidebysig static void  SwitchWithArray(string[] args) cil managed
  {
    // Code size       118 (0x76)
    .maxstack  2
    .locals init (string V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  ldelem.ref
    IL_0003:  dup
    IL_0004:  stloc.0
    IL_0005:  brfalse.s  IL_006b

    IL_0007:  ldloc.0
    IL_0008:  ldstr      "a"
    IL_000d:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0012:  brtrue.s   IL_003d

    IL_0014:  ldloc.0
    IL_0015:  ldstr      "b"
    IL_001a:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_001f:  brtrue.s   IL_0049

    IL_0021:  ldloc.0
    IL_0022:  ldstr      "c"
    IL_0027:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_002c:  brtrue.s   IL_0055

    IL_002e:  ldloc.0
    IL_002f:  ldstr      "d"
    IL_0034:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0039:  brtrue.s   IL_0061

    IL_003b:  br.s       IL_006b

    IL_003d:  ldstr      "a"
    IL_0042:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0047:  br.s       IL_006b

    IL_0049:  ldstr      "b"
    IL_004e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0053:  br.s       IL_006b

    IL_0055:  ldstr      "c"
    IL_005a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005f:  br.s       IL_006b

    IL_0061:  ldstr      "d"
    IL_0066:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006b:  ldstr      "end"
    IL_0070:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0075:  ret
  } // end of method Switch::SwitchWithArray

  .method public hidebysig static void  SwitchWithContinue1(int32 i,
                                                            bool b) cil managed
  {
    // Code size       37 (0x25)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  switch     ( 
                          IL_0016,
                          IL_0000,
                          IL_001b)
    IL_0014:  br.s       IL_001e

    IL_0016:  ldarg.1
    IL_0017:  brfalse.s  IL_001e

    IL_0019:  br.s       IL_0000

    IL_001b:  ldarg.1
    IL_001c:  brfalse.s  IL_0000

    IL_001e:  call       void [mscorlib]System.Console::WriteLine()
    IL_0023:  br.s       IL_0000
  } // end of method Switch::SwitchWithContinue1

  .method public hidebysig static void  SwitchWithContinue2(int32 i,
                                                            bool b) cil managed
  {
    // Code size       112 (0x70)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  br.s       IL_006a

    IL_0002:  ldarg.0
    IL_0003:  stloc.0
    IL_0004:  ldloc.0
    IL_0005:  switch     ( 
                          IL_001c,
                          IL_006a,
                          IL_0037,
                          IL_005b)
    IL_001a:  br.s       IL_0051

    IL_001c:  ldarg.1
    IL_001d:  brfalse.s  IL_002b

    IL_001f:  ldstr      "0b"
    IL_0024:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0029:  br.s       IL_006a

    IL_002b:  ldstr      "0!b"
    IL_0030:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0035:  br.s       IL_005b

    IL_0037:  ldarg.1
    IL_0038:  brfalse.s  IL_0045

    IL_003a:  ldstr      "2b"
    IL_003f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0044:  ret

    IL_0045:  ldstr      "2!b"
    IL_004a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004f:  br.s       IL_006a

    IL_0051:  ldstr      "default"
    IL_0056:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005b:  ldstr      "loop-tail"
    IL_0060:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0065:  ldarg.0
    IL_0066:  ldc.i4.1
    IL_0067:  add
    IL_0068:  starg.s    i
    IL_006a:  ldarg.0
    IL_006b:  ldc.i4.s   10
    IL_006d:  blt.s      IL_0002

    IL_006f:  ret
  } // end of method Switch::SwitchWithContinue2

  .method public hidebysig static void  SwitchWithContinue3(bool b) cil managed
  {
    // Code size       113 (0x71)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_006b

    IL_0004:  ldloc.0
    IL_0005:  stloc.1
    IL_0006:  ldloc.1
    IL_0007:  switch     ( 
                          IL_001e,
                          IL_0067,
                          IL_0039,
                          IL_005d)
    IL_001c:  br.s       IL_0053

    IL_001e:  ldarg.0
    IL_001f:  brfalse.s  IL_002d

    IL_0021:  ldstr      "0b"
    IL_0026:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002b:  br.s       IL_0067

    IL_002d:  ldstr      "0!b"
    IL_0032:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0037:  br.s       IL_005d

    IL_0039:  ldarg.0
    IL_003a:  brfalse.s  IL_0047

    IL_003c:  ldstr      "2b"
    IL_0041:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0046:  ret

    IL_0047:  ldstr      "2!b"
    IL_004c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0051:  br.s       IL_0067

    IL_0053:  ldstr      "default"
    IL_0058:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005d:  ldstr      "loop-tail"
    IL_0062:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0067:  ldloc.0
    IL_0068:  ldc.i4.1
    IL_0069:  add
    IL_006a:  stloc.0
    IL_006b:  ldloc.0
    IL_006c:  ldc.i4.s   10
    IL_006e:  blt.s      IL_0004

    IL_0070:  ret
  } // end of method Switch::SwitchWithContinue3

  .method public hidebysig static void  SwitchWithContinue4(bool b) cil managed
  {
    // Code size       190 (0xbe)
    .maxstack  2
    .locals init (int32 V_0,
             class [mscorlib]System.Collections.Generic.IEnumerator`1<int32> V_1,
             int32 V_2)
    IL_0000:  ldc.i4.0
    IL_0001:  ldc.i4.s   10
    IL_0003:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [System.Core]System.Linq.Enumerable::Range(int32,
                                                                                                                                    int32)
    IL_0008:  callvirt   instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!0> class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>::GetEnumerator()
    IL_000d:  stloc.1
    .try
    {
      IL_000e:  br         IL_00a6

      IL_0013:  ldloc.1
      IL_0014:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.IEnumerator`1<int32>::get_Current()
      IL_0019:  stloc.0
      IL_001a:  ldstr      "loop: "
      IL_001f:  ldloc.0
      IL_0020:  box        [mscorlib]System.Int32
      IL_0025:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_002a:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_002f:  ldloc.0
      IL_0030:  stloc.2
      IL_0031:  ldloc.2
      IL_0032:  ldc.i4.1
      IL_0033:  sub
      IL_0034:  switch     ( 
                            IL_005b,
                            IL_00a6,
                            IL_0060,
                            IL_0065,
                            IL_006d,
                            IL_0075,
                            IL_007a,
                            IL_0082)
      IL_0059:  br.s       IL_0087

      IL_005b:  ldarg.0
      IL_005c:  brfalse.s  IL_0091

      IL_005e:  br.s       IL_00a6

      IL_0060:  ldarg.0
      IL_0061:  brfalse.s  IL_00a6

      IL_0063:  leave.s    IL_00bd

      IL_0065:  ldc.i4.4
      IL_0066:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_006b:  br.s       IL_007a

      IL_006d:  ldc.i4.5
      IL_006e:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_0073:  br.s       IL_0087

      IL_0075:  ldarg.0
      IL_0076:  brfalse.s  IL_0060

      IL_0078:  br.s       IL_00a6

      IL_007a:  ldloc.0
      IL_007b:  ldc.i4.2
      IL_007c:  rem
      IL_007d:  brfalse.s  IL_0060

      IL_007f:  ldarg.0
      IL_0080:  brfalse.s  IL_00a6

      IL_0082:  ldarg.0
      IL_0083:  brfalse.s  IL_006d

      IL_0085:  br.s       IL_00a6

      IL_0087:  ldstr      "default"
      IL_008c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0091:  ldstr      "break: "
      IL_0096:  ldloc.0
      IL_0097:  box        [mscorlib]System.Int32
      IL_009c:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_00a1:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_00a6:  ldloc.1
      IL_00a7:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_00ac:  brtrue     IL_0013

      IL_00b1:  leave.s    IL_00bd

    }  // end .try
    finally
    {
      IL_00b3:  ldloc.1
      IL_00b4:  brfalse.s  IL_00bc

      IL_00b6:  ldloc.1
      IL_00b7:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_00bc:  endfinally
    }  // end handler
    IL_00bd:  ret
  } // end of method Switch::SwitchWithContinue4

  .method public hidebysig static void  SwitchWithContinue5(bool b) cil managed
  {
    // Code size       127 (0x7f)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_0079

    IL_0004:  ldloc.0
    IL_0005:  ldc.i4.5
    IL_0006:  bge.s      IL_006b

    IL_0008:  ldloc.0
    IL_0009:  stloc.1
    IL_000a:  ldloc.1
    IL_000b:  switch     ( 
                          IL_0022,
                          IL_0075,
                          IL_003d,
                          IL_0061)
    IL_0020:  br.s       IL_0057

    IL_0022:  ldarg.0
    IL_0023:  brfalse.s  IL_0031

    IL_0025:  ldstr      "0b"
    IL_002a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002f:  br.s       IL_0075

    IL_0031:  ldstr      "0!b"
    IL_0036:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003b:  br.s       IL_0061

    IL_003d:  ldarg.0
    IL_003e:  brfalse.s  IL_004b

    IL_0040:  ldstr      "2b"
    IL_0045:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004a:  ret

    IL_004b:  ldstr      "2!b"
    IL_0050:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0055:  br.s       IL_0075

    IL_0057:  ldstr      "default"
    IL_005c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0061:  ldstr      "break-target"
    IL_0066:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006b:  ldstr      "loop-tail"
    IL_0070:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0075:  ldloc.0
    IL_0076:  ldc.i4.1
    IL_0077:  add
    IL_0078:  stloc.0
    IL_0079:  ldloc.0
    IL_007a:  ldc.i4.s   10
    IL_007c:  blt.s      IL_0004

    IL_007e:  ret
  } // end of method Switch::SwitchWithContinue5

  .method public hidebysig static void  SwitchWithContinue6(int32 i,
                                                            bool b) cil managed
  {
    // Code size       110 (0x6e)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  switch     ( 
                          IL_001a,
                          IL_0063,
                          IL_0035,
                          IL_0059)
    IL_0018:  br.s       IL_004f

    IL_001a:  ldarg.1
    IL_001b:  brtrue.s   IL_0029

    IL_001d:  ldstr      "0!b"
    IL_0022:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0027:  br.s       IL_0059

    IL_0029:  ldstr      "0b"
    IL_002e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0033:  br.s       IL_0063

    IL_0035:  ldarg.1
    IL_0036:  brfalse.s  IL_0043

    IL_0038:  ldstr      "2b"
    IL_003d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0042:  ret

    IL_0043:  ldstr      "2!b"
    IL_0048:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004d:  br.s       IL_0063

    IL_004f:  ldstr      "default"
    IL_0054:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0059:  ldstr      "loop-tail"
    IL_005e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0063:  ldarg.0
    IL_0064:  ldc.i4.1
    IL_0065:  add
    IL_0066:  dup
    IL_0067:  starg.s    i
    IL_0069:  ldc.i4.s   10
    IL_006b:  blt.s      IL_0000

    IL_006d:  ret
  } // end of method Switch::SwitchWithContinue6

  .method public hidebysig static void  SwitchWithContinue7() cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  2
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_002e

    IL_0004:  ldstr      "loop-head"
    IL_0009:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000e:  ldloc.0
    IL_000f:  stloc.1
    IL_0010:  ldloc.1
    IL_0011:  switch     ( 
                          IL_002a,
                          IL_0032)
    IL_001e:  ldstr      "default"
    IL_0023:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0028:  br.s       IL_0032

    IL_002a:  ldloc.0
    IL_002b:  ldc.i4.1
    IL_002c:  sub
    IL_002d:  stloc.0
    IL_002e:  ldloc.0
    IL_002f:  ldc.i4.0
    IL_0030:  bge.s      IL_0004

    IL_0032:  ldstr      "end"
    IL_0037:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003c:  ret
  } // end of method Switch::SwitchWithContinue7

  .method public hidebysig static void  SwitchWithContinueInDoubleLoop() cil managed
  {
    // Code size       105 (0x69)
    .maxstack  2
    .locals init (bool V_0,
             int32 V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    IL_0004:  br.s       IL_005d

    IL_0006:  ldc.i4.0
    IL_0007:  stloc.2
    IL_0008:  br.s       IL_0054

    IL_000a:  ldloc.1
    IL_000b:  ldloc.2
    IL_000c:  add
    IL_000d:  stloc.3
    IL_000e:  ldloc.3
    IL_000f:  ldc.i4.1
    IL_0010:  sub
    IL_0011:  switch     ( 
                          IL_004c,
                          IL_0050,
                          IL_004c,
                          IL_0050,
                          IL_004c,
                          IL_0050,
                          IL_004c)
    IL_0032:  ldloc.3
    IL_0033:  ldc.i4.s   11
    IL_0035:  sub
    IL_0036:  switch     ( 
                          IL_004c,
                          IL_0050,
                          IL_004c)
    IL_0047:  ldloc.3
    IL_0048:  ldc.i4.s   17
    IL_004a:  bne.un.s   IL_0050

    IL_004c:  ldc.i4.1
    IL_004d:  stloc.0
    IL_004e:  br.s       IL_0059

    IL_0050:  ldloc.2
    IL_0051:  ldc.i4.1
    IL_0052:  add
    IL_0053:  stloc.2
    IL_0054:  ldloc.2
    IL_0055:  ldc.i4.s   10
    IL_0057:  blt.s      IL_000a

    IL_0059:  ldloc.1
    IL_005a:  ldc.i4.1
    IL_005b:  add
    IL_005c:  stloc.1
    IL_005d:  ldloc.1
    IL_005e:  ldc.i4.s   10
    IL_0060:  blt.s      IL_0006

    IL_0062:  ldloc.0
    IL_0063:  call       void [mscorlib]System.Console::WriteLine(bool)
    IL_0068:  ret
  } // end of method Switch::SwitchWithContinueInDoubleLoop

  .method public hidebysig static void  SwitchLoopNesting() cil managed
  {
    // Code size       101 (0x65)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  br.s       IL_005f

    IL_0004:  ldloc.0
    IL_0005:  stloc.1
    IL_0006:  ldloc.1
    IL_0007:  switch     ( 
                          IL_0016,
                          IL_001e)
    IL_0014:  br.s       IL_0026

    IL_0016:  ldc.i4.0
    IL_0017:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001c:  br.s       IL_0041

    IL_001e:  ldc.i4.1
    IL_001f:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0024:  br.s       IL_0041

    IL_0026:  ldloc.0
    IL_0027:  ldc.i4.2
    IL_0028:  rem
    IL_0029:  brtrue.s   IL_003c

    IL_002b:  br.s       IL_0037

    IL_002d:  ldloc.0
    IL_002e:  dup
    IL_002f:  ldc.i4.1
    IL_0030:  add
    IL_0031:  stloc.0
    IL_0032:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0037:  ldloc.0
    IL_0038:  ldc.i4.3
    IL_0039:  rem
    IL_003a:  brtrue.s   IL_002d

    IL_003c:  call       void [mscorlib]System.Console::WriteLine()
    IL_0041:  ldloc.0
    IL_0042:  ldc.i4.4
    IL_0043:  ble.s      IL_0051

    IL_0045:  ldstr      "high"
    IL_004a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004f:  br.s       IL_005b

    IL_0051:  ldstr      "low"
    IL_0056:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005b:  ldloc.0
    IL_005c:  ldc.i4.1
    IL_005d:  add
    IL_005e:  stloc.0
    IL_005f:  ldloc.0
    IL_0060:  ldc.i4.s   10
    IL_0062:  blt.s      IL_0004

    IL_0064:  ret
  } // end of method Switch::SwitchLoopNesting

  .method public hidebysig static void  SingleIf1(int32 i,
                                                  bool a) cil managed
  {
    // Code size       24 (0x18)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  beq.s      IL_000b

    IL_0004:  ldarg.0
    IL_0005:  ldc.i4.2
    IL_0006:  bne.un.s   IL_0011

    IL_0008:  ldarg.1
    IL_0009:  brfalse.s  IL_0011

    IL_000b:  ldc.i4.1
    IL_000c:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0011:  ldc.i4.2
    IL_0012:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0017:  ret
  } // end of method Switch::SingleIf1

  .method public hidebysig static void  SingleIf2(int32 i,
                                                  bool a,
                                                  bool b) cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  beq.s      IL_0012

    IL_0004:  ldarg.0
    IL_0005:  ldc.i4.2
    IL_0006:  bne.un.s   IL_000b

    IL_0008:  ldarg.1
    IL_0009:  brtrue.s   IL_0012

    IL_000b:  ldarg.0
    IL_000c:  ldc.i4.3
    IL_000d:  bne.un.s   IL_0018

    IL_000f:  ldarg.2
    IL_0010:  brfalse.s  IL_0018

    IL_0012:  ldc.i4.1
    IL_0013:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0018:  ldc.i4.2
    IL_0019:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001e:  ret
  } // end of method Switch::SingleIf2

  .method public hidebysig static void  SingleIf3(int32 i,
                                                  bool a,
                                                  bool b) cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000e

    IL_0003:  ldarg.0
    IL_0004:  ldc.i4.1
    IL_0005:  beq.s      IL_000e

    IL_0007:  ldarg.0
    IL_0008:  ldc.i4.2
    IL_0009:  bne.un.s   IL_0014

    IL_000b:  ldarg.2
    IL_000c:  brfalse.s  IL_0014

    IL_000e:  ldc.i4.1
    IL_000f:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0014:  ldc.i4.2
    IL_0015:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001a:  ret
  } // end of method Switch::SingleIf3

  .method public hidebysig static void  SingleIf4(int32 i,
                                                  bool a) cil managed
  {
    // Code size       32 (0x20)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  beq.s      IL_0013

    IL_0004:  ldarg.0
    IL_0005:  ldc.i4.2
    IL_0006:  beq.s      IL_0013

    IL_0008:  ldarg.0
    IL_0009:  ldc.i4.3
    IL_000a:  beq.s      IL_000f

    IL_000c:  ldarg.1
    IL_000d:  brtrue.s   IL_0013

    IL_000f:  ldarg.0
    IL_0010:  ldc.i4.4
    IL_0011:  beq.s      IL_0019

    IL_0013:  ldc.i4.1
    IL_0014:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0019:  ldc.i4.2
    IL_001a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001f:  ret
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
    // Code size       62 (0x3e)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.1
    IL_0001:  brfalse.s  IL_0033

    IL_0003:  ldarg.0
    IL_0004:  stloc.0
    IL_0005:  ldloc.0
    IL_0006:  ldc.i4.1
    IL_0007:  sub
    IL_0008:  switch     ( 
                          IL_0017,
                          IL_0029)
    IL_0015:  br.s       IL_001f

    IL_0017:  ldc.i4.1
    IL_0018:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001d:  br.s       IL_0029

    IL_001f:  ldstr      "default"
    IL_0024:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0029:  ldstr      "b"
    IL_002e:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0033:  ldstr      "end"
    IL_0038:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003d:  ret
  } // end of method Switch::SwitchWithBreakCase

  .method public hidebysig static void  SwitchWithReturnAndBreak(int32 i,
                                                                 bool b) cil managed
  {
    // Code size       32 (0x20)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  switch     ( 
                          IL_0012,
                          IL_0016)
    IL_0010:  br.s       IL_001a

    IL_0012:  ldarg.1
    IL_0013:  brfalse.s  IL_001a

    IL_0015:  ret

    IL_0016:  ldarg.1
    IL_0017:  brtrue.s   IL_001a

    IL_0019:  ret

    IL_001a:  call       void [mscorlib]System.Console::WriteLine()
    IL_001f:  ret
  } // end of method Switch::SwitchWithReturnAndBreak

  .method public hidebysig static int32  SwitchWithReturnAndBreak2(int32 i,
                                                                   bool b) cil managed
  {
    // Code size       79 (0x4f)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  ldc.i4     0x14e
    IL_0008:  bgt.s      IL_001d

    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.4
    IL_000c:  beq.s      IL_0037

    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.s   33
    IL_0011:  beq.s      IL_0037

    IL_0013:  ldloc.0
    IL_0014:  ldc.i4     0x14e
    IL_0019:  beq.s      IL_003e

    IL_001b:  br.s       IL_0048

    IL_001d:  ldloc.0
    IL_001e:  ldc.i4     0x18b
    IL_0023:  beq.s      IL_0043

    IL_0025:  ldloc.0
    IL_0026:  ldc.i4     0x19a
    IL_002b:  beq.s      IL_0043

    IL_002d:  ldloc.0
    IL_002e:  ldc.i4     0x1c7
    IL_0033:  beq.s      IL_0043

    IL_0035:  br.s       IL_0048

    IL_0037:  call       void [mscorlib]System.Console::WriteLine()
    IL_003c:  ldc.i4.1
    IL_003d:  ret

    IL_003e:  ldarg.1
    IL_003f:  brfalse.s  IL_0048

    IL_0041:  ldc.i4.2
    IL_0042:  ret

    IL_0043:  call       void [mscorlib]System.Console::WriteLine()
    IL_0048:  call       void [mscorlib]System.Console::WriteLine()
    IL_004d:  ldc.i4.0
    IL_004e:  ret
  } // end of method Switch::SwitchWithReturnAndBreak2

  .method public hidebysig static void  SwitchWithReturnAndBreak3(int32 i) cil managed
  {
    // Code size       37 (0x25)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  switch     ( 
                          IL_0011,
                          IL_0019)
    IL_0010:  ret

    IL_0011:  ldc.i4.0
    IL_0012:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0017:  br.s       IL_001f

    IL_0019:  ldc.i4.1
    IL_001a:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001f:  call       void [mscorlib]System.Console::WriteLine()
    IL_0024:  ret
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
