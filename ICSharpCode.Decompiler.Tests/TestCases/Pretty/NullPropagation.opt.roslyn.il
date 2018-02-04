
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly NullPropagation
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
.module NullPropagation.dll
// MVID: {DDAB2C82-C901-450A-B55D-2D0D7BC34C48}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x003E0000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation
       extends [mscorlib]System.Object
{
  .class auto ansi nested private beforefieldinit MyClass
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
    .field public int32 IntVal
    .field public string Text
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass Field
    .field private class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass '<Property>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig specialname 
            instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass 
            get_Property() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::'<Property>k__BackingField'
      IL_0006:  ret
    } // end of method MyClass::get_Property

    .method public hidebysig specialname 
            instance void  set_Property(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::'<Property>k__BackingField'
      IL_0007:  ret
    } // end of method MyClass::set_Property

    .method public hidebysig specialname 
            instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass 
            get_Item(int32 index) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ret
    } // end of method MyClass::get_Item

    .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass 
            Method(int32 arg) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ret
    } // end of method MyClass::Method

    .method public hidebysig instance void 
            Done() cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method MyClass::Done

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method MyClass::.ctor

    .property instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass
            Property()
    {
      .get instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::get_Property()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::set_Property(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass)
    } // end of property MyClass::Property
    .property instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass
            Item(int32)
    {
      .get instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::get_Item(int32)
    } // end of property MyClass::Item
  } // end of class MyClass

  .class sequential ansi sealed nested private beforefieldinit MyStruct
         extends [mscorlib]System.ValueType
  {
    .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
    .field public int32 IntVal
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass Field
    .method public hidebysig specialname 
            instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> 
            get_Property1() cil managed
    {
      // Code size       10 (0xa)
      .maxstack  1
      .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_0)
      IL_0000:  ldloca.s   V_0
      IL_0002:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>
      IL_0008:  ldloc.0
      IL_0009:  ret
    } // end of method MyStruct::get_Property1

    .method public hidebysig specialname 
            instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct 
            get_Property2() cil managed
    {
      // Code size       10 (0xa)
      .maxstack  1
      .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct V_0)
      IL_0000:  ldloca.s   V_0
      IL_0002:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct
      IL_0008:  ldloc.0
      IL_0009:  ret
    } // end of method MyStruct::get_Property2

    .method public hidebysig specialname 
            instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> 
            get_Item(int32 index) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  1
      .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_0)
      IL_0000:  ldloca.s   V_0
      IL_0002:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>
      IL_0008:  ldloc.0
      IL_0009:  ret
    } // end of method MyStruct::get_Item

    .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> 
            Method1(int32 arg) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  1
      .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_0)
      IL_0000:  ldloca.s   V_0
      IL_0002:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>
      IL_0008:  ldloc.0
      IL_0009:  ret
    } // end of method MyStruct::Method1

    .method public hidebysig instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct 
            Method2(int32 arg) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  1
      .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct V_0)
      IL_0000:  ldloca.s   V_0
      IL_0002:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct
      IL_0008:  ldloc.0
      IL_0009:  ret
    } // end of method MyStruct::Method2

    .method public hidebysig instance void 
            Done() cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method MyStruct::Done

    .property instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>
            Property1()
    {
      .get instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::get_Property1()
    } // end of property MyStruct::Property1
    .property instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct
            Property2()
    {
      .get instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::get_Property2()
    } // end of property MyStruct::Property2
    .property instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>
            Item(int32)
    {
      .get instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::get_Item(int32)
    } // end of property MyStruct::Item
  } // end of class MyStruct

  .method private hidebysig instance int32 
          GetInt() cil managed
  {
    // Code size       3 (0x3)
    .maxstack  8
    IL_0000:  ldc.i4.s   9
    IL_0002:  ret
  } // end of method NullPropagation::GetInt

  .method private hidebysig instance string 
          GetString() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ret
  } // end of method NullPropagation::GetString

  .method private hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass 
          GetMyClass() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ret
  } // end of method NullPropagation::GetMyClass

  .method private hidebysig instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> 
          GetMyStruct() cil managed
  {
    // Code size       10 (0xa)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_0)
    IL_0000:  ldloca.s   V_0
    IL_0002:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>
    IL_0008:  ldloc.0
    IL_0009:  ret
  } // end of method NullPropagation::GetMyStruct

  .method public hidebysig instance string 
          Substring() cil managed
  {
    // Code size       24 (0x18)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetString()
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_000c

    IL_0009:  pop
    IL_000a:  ldnull
    IL_000b:  ret

    IL_000c:  ldarg.0
    IL_000d:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_0012:  call       instance string [mscorlib]System.String::Substring(int32)
    IL_0017:  ret
  } // end of method NullPropagation::Substring

  .method public hidebysig instance void 
          CallSubstringAndIgnoreResult() cil managed
  {
    // Code size       24 (0x18)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetString()
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_000b

    IL_0009:  pop
    IL_000a:  ret

    IL_000b:  ldarg.0
    IL_000c:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_0011:  call       instance string [mscorlib]System.String::Substring(int32)
    IL_0016:  pop
    IL_0017:  ret
  } // end of method NullPropagation::CallSubstringAndIgnoreResult

  .method private hidebysig instance void 
          Use<T>(!!T t) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method NullPropagation::Use

  .method public hidebysig instance void 
          CallDone() cil managed
  {
    // Code size       241 (0xf1)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_000c

    IL_0009:  pop
    IL_000a:  br.s       IL_0011

    IL_000c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_0011:  ldarg.0
    IL_0012:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0017:  dup
    IL_0018:  brtrue.s   IL_001d

    IL_001a:  pop
    IL_001b:  br.s       IL_002d

    IL_001d:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Field
    IL_0022:  dup
    IL_0023:  brtrue.s   IL_0028

    IL_0025:  pop
    IL_0026:  br.s       IL_002d

    IL_0028:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_002d:  ldarg.0
    IL_002e:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0033:  dup
    IL_0034:  brtrue.s   IL_0039

    IL_0036:  pop
    IL_0037:  br.s       IL_0043

    IL_0039:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Field
    IL_003e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_0043:  ldarg.0
    IL_0044:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0049:  dup
    IL_004a:  brtrue.s   IL_004f

    IL_004c:  pop
    IL_004d:  br.s       IL_005f

    IL_004f:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::get_Property()
    IL_0054:  dup
    IL_0055:  brtrue.s   IL_005a

    IL_0057:  pop
    IL_0058:  br.s       IL_005f

    IL_005a:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_005f:  ldarg.0
    IL_0060:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0065:  dup
    IL_0066:  brtrue.s   IL_006b

    IL_0068:  pop
    IL_0069:  br.s       IL_0075

    IL_006b:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::get_Property()
    IL_0070:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_0075:  ldarg.0
    IL_0076:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_007b:  dup
    IL_007c:  brtrue.s   IL_0081

    IL_007e:  pop
    IL_007f:  br.s       IL_0097

    IL_0081:  ldarg.0
    IL_0082:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_0087:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Method(int32)
    IL_008c:  dup
    IL_008d:  brtrue.s   IL_0092

    IL_008f:  pop
    IL_0090:  br.s       IL_0097

    IL_0092:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_0097:  ldarg.0
    IL_0098:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_009d:  dup
    IL_009e:  brtrue.s   IL_00a3

    IL_00a0:  pop
    IL_00a1:  br.s       IL_00b3

    IL_00a3:  ldarg.0
    IL_00a4:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_00a9:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Method(int32)
    IL_00ae:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_00b3:  ldarg.0
    IL_00b4:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_00b9:  dup
    IL_00ba:  brtrue.s   IL_00bf

    IL_00bc:  pop
    IL_00bd:  br.s       IL_00d5

    IL_00bf:  ldarg.0
    IL_00c0:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_00c5:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::get_Item(int32)
    IL_00ca:  dup
    IL_00cb:  brtrue.s   IL_00d0

    IL_00cd:  pop
    IL_00ce:  br.s       IL_00d5

    IL_00d0:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_00d5:  ldarg.0
    IL_00d6:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_00db:  dup
    IL_00dc:  brtrue.s   IL_00e0

    IL_00de:  pop
    IL_00df:  ret

    IL_00e0:  ldarg.0
    IL_00e1:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_00e6:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::get_Item(int32)
    IL_00eb:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_00f0:  ret
  } // end of method NullPropagation::CallDone

  .method public hidebysig instance void 
          CallDoneStruct() cil managed
  {
    // Code size       33 (0x21)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct V_1)
    IL_0000:  ldarg.0
    IL_0001:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_0006:  stloc.0
    IL_0007:  ldloca.s   V_0
    IL_0009:  dup
    IL_000a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_000f:  brtrue.s   IL_0013

    IL_0011:  pop
    IL_0012:  ret

    IL_0013:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_0018:  stloc.1
    IL_0019:  ldloca.s   V_1
    IL_001b:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Done()
    IL_0020:  ret
  } // end of method NullPropagation::CallDoneStruct

  .method public hidebysig instance void 
          RequiredParentheses() cil managed
  {
    // Code size       53 (0x35)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_000d

    IL_0009:  pop
    IL_000a:  ldnull
    IL_000b:  br.s       IL_0012

    IL_000d:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Field
    IL_0012:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_0017:  ldarg.0
    IL_0018:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_001d:  dup
    IL_001e:  brtrue.s   IL_0024

    IL_0020:  pop
    IL_0021:  ldnull
    IL_0022:  br.s       IL_002f

    IL_0024:  ldarg.0
    IL_0025:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_002a:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Method(int32)
    IL_002f:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_0034:  ret
  } // end of method NullPropagation::RequiredParentheses

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int32>[] 
          ChainsOnClass() cil managed
  {
    // Code size       474 (0x1da)
    .maxstack  5
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0)
    IL_0000:  ldc.i4.s   9
    IL_0002:  newarr     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0007:  dup
    IL_0008:  ldc.i4.0
    IL_0009:  ldarg.0
    IL_000a:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_000f:  dup
    IL_0010:  brtrue.s   IL_001e

    IL_0012:  pop
    IL_0013:  ldloca.s   V_0
    IL_0015:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_001b:  ldloc.0
    IL_001c:  br.s       IL_0028

    IL_001e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_0023:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0028:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_002d:  dup
    IL_002e:  ldc.i4.1
    IL_002f:  ldarg.0
    IL_0030:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0035:  dup
    IL_0036:  brtrue.s   IL_0044

    IL_0038:  pop
    IL_0039:  ldloca.s   V_0
    IL_003b:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0041:  ldloc.0
    IL_0042:  br.s       IL_0053

    IL_0044:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Field
    IL_0049:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_004e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0053:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0058:  dup
    IL_0059:  ldc.i4.2
    IL_005a:  ldarg.0
    IL_005b:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0060:  dup
    IL_0061:  brtrue.s   IL_006f

    IL_0063:  pop
    IL_0064:  ldloca.s   V_0
    IL_0066:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_006c:  ldloc.0
    IL_006d:  br.s       IL_008d

    IL_006f:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Field
    IL_0074:  dup
    IL_0075:  brtrue.s   IL_0083

    IL_0077:  pop
    IL_0078:  ldloca.s   V_0
    IL_007a:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0080:  ldloc.0
    IL_0081:  br.s       IL_008d

    IL_0083:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_0088:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_008d:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0092:  dup
    IL_0093:  ldc.i4.3
    IL_0094:  ldarg.0
    IL_0095:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_009a:  dup
    IL_009b:  brtrue.s   IL_00a9

    IL_009d:  pop
    IL_009e:  ldloca.s   V_0
    IL_00a0:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00a6:  ldloc.0
    IL_00a7:  br.s       IL_00b8

    IL_00a9:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::get_Property()
    IL_00ae:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_00b3:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00b8:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_00bd:  dup
    IL_00be:  ldc.i4.4
    IL_00bf:  ldarg.0
    IL_00c0:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_00c5:  dup
    IL_00c6:  brtrue.s   IL_00d4

    IL_00c8:  pop
    IL_00c9:  ldloca.s   V_0
    IL_00cb:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00d1:  ldloc.0
    IL_00d2:  br.s       IL_00f2

    IL_00d4:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::get_Property()
    IL_00d9:  dup
    IL_00da:  brtrue.s   IL_00e8

    IL_00dc:  pop
    IL_00dd:  ldloca.s   V_0
    IL_00df:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00e5:  ldloc.0
    IL_00e6:  br.s       IL_00f2

    IL_00e8:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_00ed:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00f2:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_00f7:  dup
    IL_00f8:  ldc.i4.5
    IL_00f9:  ldarg.0
    IL_00fa:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_00ff:  dup
    IL_0100:  brtrue.s   IL_010e

    IL_0102:  pop
    IL_0103:  ldloca.s   V_0
    IL_0105:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_010b:  ldloc.0
    IL_010c:  br.s       IL_0123

    IL_010e:  ldarg.0
    IL_010f:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_0114:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Method(int32)
    IL_0119:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_011e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0123:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0128:  dup
    IL_0129:  ldc.i4.6
    IL_012a:  ldarg.0
    IL_012b:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0130:  dup
    IL_0131:  brtrue.s   IL_013f

    IL_0133:  pop
    IL_0134:  ldloca.s   V_0
    IL_0136:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_013c:  ldloc.0
    IL_013d:  br.s       IL_0163

    IL_013f:  ldarg.0
    IL_0140:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_0145:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Method(int32)
    IL_014a:  dup
    IL_014b:  brtrue.s   IL_0159

    IL_014d:  pop
    IL_014e:  ldloca.s   V_0
    IL_0150:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0156:  ldloc.0
    IL_0157:  br.s       IL_0163

    IL_0159:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_015e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0163:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0168:  dup
    IL_0169:  ldc.i4.7
    IL_016a:  ldarg.0
    IL_016b:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0170:  dup
    IL_0171:  brtrue.s   IL_017f

    IL_0173:  pop
    IL_0174:  ldloca.s   V_0
    IL_0176:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_017c:  ldloc.0
    IL_017d:  br.s       IL_0194

    IL_017f:  ldarg.0
    IL_0180:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_0185:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::get_Item(int32)
    IL_018a:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_018f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0194:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0199:  dup
    IL_019a:  ldc.i4.8
    IL_019b:  ldarg.0
    IL_019c:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_01a1:  dup
    IL_01a2:  brtrue.s   IL_01b0

    IL_01a4:  pop
    IL_01a5:  ldloca.s   V_0
    IL_01a7:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01ad:  ldloc.0
    IL_01ae:  br.s       IL_01d4

    IL_01b0:  ldarg.0
    IL_01b1:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_01b6:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::get_Item(int32)
    IL_01bb:  dup
    IL_01bc:  brtrue.s   IL_01ca

    IL_01be:  pop
    IL_01bf:  ldloca.s   V_0
    IL_01c1:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01c7:  ldloc.0
    IL_01c8:  br.s       IL_01d4

    IL_01ca:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_01cf:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01d4:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_01d9:  ret
  } // end of method NullPropagation::ChainsOnClass

  .method public hidebysig instance int32 
          CoalescingReturn() cil managed
  {
    // Code size       18 (0x12)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_000c

    IL_0009:  pop
    IL_000a:  ldc.i4.1
    IL_000b:  ret

    IL_000c:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_0011:  ret
  } // end of method NullPropagation::CoalescingReturn

  .method public hidebysig instance void 
          Coalescing() cil managed
  {
    // Code size       25 (0x19)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0007:  dup
    IL_0008:  brtrue.s   IL_000e

    IL_000a:  pop
    IL_000b:  ldc.i4.1
    IL_000c:  br.s       IL_0013

    IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_0013:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::Use<int32>(!!0)
    IL_0018:  ret
  } // end of method NullPropagation::Coalescing

  .method public hidebysig instance void 
          CoalescingString() cil managed
  {
    // Code size       34 (0x22)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.0
    IL_0002:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0007:  dup
    IL_0008:  brtrue.s   IL_000e

    IL_000a:  pop
    IL_000b:  ldnull
    IL_000c:  br.s       IL_0013

    IL_000e:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Text
    IL_0013:  dup
    IL_0014:  brtrue.s   IL_001c

    IL_0016:  pop
    IL_0017:  ldstr      "Hello"
    IL_001c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::Use<string>(!!0)
    IL_0021:  ret
  } // end of method NullPropagation::CoalescingString

  .method public hidebysig instance void 
          InvokeDelegate(class [mscorlib]System.EventHandler eh) cil managed
  {
    // Code size       16 (0x10)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  brfalse.s  IL_000f

    IL_0003:  ldarg.1
    IL_0004:  ldnull
    IL_0005:  ldsfld     class [mscorlib]System.EventArgs [mscorlib]System.EventArgs::Empty
    IL_000a:  callvirt   instance void [mscorlib]System.EventHandler::Invoke(object,
                                                                             class [mscorlib]System.EventArgs)
    IL_000f:  ret
  } // end of method NullPropagation::InvokeDelegate

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int32> 
          InvokeDelegate(class [mscorlib]System.Func`1<int32> f) cil managed
  {
    // Code size       25 (0x19)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0)
    IL_0000:  ldarg.1
    IL_0001:  brtrue.s   IL_000d

    IL_0003:  ldloca.s   V_0
    IL_0005:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_000b:  ldloc.0
    IL_000c:  ret

    IL_000d:  ldarg.1
    IL_000e:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0013:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0018:  ret
  } // end of method NullPropagation::InvokeDelegate

  .method private hidebysig instance void 
          NotNullPropagation(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass c) cil managed
  {
    // Code size       53 (0x35)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  brfalse.s  IL_0015

    IL_0003:  ldarg.1
    IL_0004:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_0009:  brfalse.s  IL_0015

    IL_000b:  ldstr      "non-zero"
    IL_0010:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0015:  ldarg.1
    IL_0016:  brfalse.s  IL_0020

    IL_0018:  ldarg.1
    IL_0019:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_001e:  brtrue.s   IL_002a

    IL_0020:  ldstr      "null or zero"
    IL_0025:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002a:  ldstr      "end of method"
    IL_002f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0034:  ret
  } // end of method NullPropagation::NotNullPropagation

  .method private hidebysig instance void 
          Setter(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass c) cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  brfalse.s  IL_000a

    IL_0003:  ldarg.1
    IL_0004:  ldc.i4.1
    IL_0005:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_000a:  call       void [mscorlib]System.Console::WriteLine()
    IL_000f:  ldarg.1
    IL_0010:  brfalse.s  IL_0019

    IL_0012:  ldarg.1
    IL_0013:  ldnull
    IL_0014:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::set_Property(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass)
    IL_0019:  ret
  } // end of method NullPropagation::Setter

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method NullPropagation::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
