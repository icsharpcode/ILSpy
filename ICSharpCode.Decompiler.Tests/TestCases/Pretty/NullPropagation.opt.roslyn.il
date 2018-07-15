



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
.assembly extern Microsoft.CSharp
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
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
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


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

  .class interface abstract auto ansi nested public ITest
  {
    .method public hidebysig newslot abstract virtual 
            instance int32  Int() cil managed
    {
    } // end of method ITest::Int

    .method public hidebysig newslot abstract virtual 
            instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/ITest 
            Next() cil managed
    {
    } // end of method ITest::Next

  } // end of class ITest

  .class abstract auto ansi sealed nested private beforefieldinit '<>o__27'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> '<>p__0'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>> '<>p__1'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> '<>p__2'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>> '<>p__3'
  } // end of class '<>o__27'

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
    // Code size       388 (0x184)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct V_1,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_2)
    IL_0000:  ldarg.0
    IL_0001:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_0006:  stloc.0
    IL_0007:  ldloca.s   V_0
    IL_0009:  dup
    IL_000a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_000f:  brtrue.s   IL_0014

    IL_0011:  pop
    IL_0012:  br.s       IL_0021

    IL_0014:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_0019:  stloc.1
    IL_001a:  ldloca.s   V_1
    IL_001c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Done()
    IL_0021:  ldarg.0
    IL_0022:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_0027:  stloc.0
    IL_0028:  ldloca.s   V_0
    IL_002a:  dup
    IL_002b:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0030:  brtrue.s   IL_0035

    IL_0032:  pop
    IL_0033:  br.s       IL_004a

    IL_0035:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_003a:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Field
    IL_003f:  dup
    IL_0040:  brtrue.s   IL_0045

    IL_0042:  pop
    IL_0043:  br.s       IL_004a

    IL_0045:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_004a:  ldarg.0
    IL_004b:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_0050:  stloc.0
    IL_0051:  ldloca.s   V_0
    IL_0053:  dup
    IL_0054:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0059:  brtrue.s   IL_005e

    IL_005b:  pop
    IL_005c:  br.s       IL_006d

    IL_005e:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_0063:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Field
    IL_0068:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_006d:  ldarg.0
    IL_006e:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_0073:  stloc.0
    IL_0074:  ldloca.s   V_0
    IL_0076:  dup
    IL_0077:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_007c:  brtrue.s   IL_0081

    IL_007e:  pop
    IL_007f:  br.s       IL_00a9

    IL_0081:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_0086:  stloc.1
    IL_0087:  ldloca.s   V_1
    IL_0089:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::get_Property1()
    IL_008e:  stloc.2
    IL_008f:  ldloca.s   V_2
    IL_0091:  dup
    IL_0092:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0097:  brtrue.s   IL_009c

    IL_0099:  pop
    IL_009a:  br.s       IL_00a9

    IL_009c:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_00a1:  stloc.1
    IL_00a2:  ldloca.s   V_1
    IL_00a4:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Done()
    IL_00a9:  ldarg.0
    IL_00aa:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_00af:  stloc.0
    IL_00b0:  ldloca.s   V_0
    IL_00b2:  dup
    IL_00b3:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_00b8:  brtrue.s   IL_00bd

    IL_00ba:  pop
    IL_00bb:  br.s       IL_00d2

    IL_00bd:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_00c2:  stloc.1
    IL_00c3:  ldloca.s   V_1
    IL_00c5:  call       instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::get_Property2()
    IL_00ca:  stloc.1
    IL_00cb:  ldloca.s   V_1
    IL_00cd:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Done()
    IL_00d2:  ldarg.0
    IL_00d3:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_00d8:  stloc.0
    IL_00d9:  ldloca.s   V_0
    IL_00db:  dup
    IL_00dc:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_00e1:  brtrue.s   IL_00e6

    IL_00e3:  pop
    IL_00e4:  br.s       IL_0114

    IL_00e6:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_00eb:  stloc.1
    IL_00ec:  ldloca.s   V_1
    IL_00ee:  ldarg.0
    IL_00ef:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_00f4:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Method1(int32)
    IL_00f9:  stloc.2
    IL_00fa:  ldloca.s   V_2
    IL_00fc:  dup
    IL_00fd:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0102:  brtrue.s   IL_0107

    IL_0104:  pop
    IL_0105:  br.s       IL_0114

    IL_0107:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_010c:  stloc.1
    IL_010d:  ldloca.s   V_1
    IL_010f:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Done()
    IL_0114:  ldarg.0
    IL_0115:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_011a:  stloc.0
    IL_011b:  ldloca.s   V_0
    IL_011d:  dup
    IL_011e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0123:  brtrue.s   IL_0128

    IL_0125:  pop
    IL_0126:  br.s       IL_0143

    IL_0128:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_012d:  stloc.1
    IL_012e:  ldloca.s   V_1
    IL_0130:  ldarg.0
    IL_0131:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_0136:  call       instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Method2(int32)
    IL_013b:  stloc.1
    IL_013c:  ldloca.s   V_1
    IL_013e:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Done()
    IL_0143:  ldarg.0
    IL_0144:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_0149:  stloc.0
    IL_014a:  ldloca.s   V_0
    IL_014c:  dup
    IL_014d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0152:  brtrue.s   IL_0156

    IL_0154:  pop
    IL_0155:  ret

    IL_0156:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_015b:  stloc.1
    IL_015c:  ldloca.s   V_1
    IL_015e:  ldarg.0
    IL_015f:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_0164:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::get_Item(int32)
    IL_0169:  stloc.2
    IL_016a:  ldloca.s   V_2
    IL_016c:  dup
    IL_016d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0172:  brtrue.s   IL_0176

    IL_0174:  pop
    IL_0175:  ret

    IL_0176:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_017b:  stloc.1
    IL_017c:  ldloca.s   V_1
    IL_017e:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Done()
    IL_0183:  ret
  } // end of method NullPropagation::CallDoneStruct

  .method public hidebysig instance void 
          RequiredParentheses() cil managed
  {
    // Code size       126 (0x7e)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_0,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_1,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct V_2)
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
    IL_0034:  ldarg.0
    IL_0035:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_003a:  stloc.0
    IL_003b:  ldloca.s   V_0
    IL_003d:  dup
    IL_003e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0043:  brtrue.s   IL_0051

    IL_0045:  pop
    IL_0046:  ldloca.s   V_1
    IL_0048:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>
    IL_004e:  ldloc.1
    IL_004f:  br.s       IL_0063

    IL_0051:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_0056:  stloc.2
    IL_0057:  ldloca.s   V_2
    IL_0059:  call       instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::get_Property2()
    IL_005e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::.ctor(!0)
    IL_0063:  stloc.0
    IL_0064:  ldloca.s   V_0
    IL_0066:  dup
    IL_0067:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_006c:  brtrue.s   IL_0070

    IL_006e:  pop
    IL_006f:  ret

    IL_0070:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_0075:  stloc.2
    IL_0076:  ldloca.s   V_2
    IL_0078:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Done()
    IL_007d:  ret
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

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int32>[] 
          ChainsStruct() cil managed
  {
    // Code size       582 (0x246)
    .maxstack  5
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct V_2,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_3)
    IL_0000:  ldc.i4.8
    IL_0001:  newarr     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0006:  dup
    IL_0007:  ldc.i4.0
    IL_0008:  ldarg.0
    IL_0009:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_000e:  stloc.0
    IL_000f:  ldloca.s   V_0
    IL_0011:  dup
    IL_0012:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0017:  brtrue.s   IL_0025

    IL_0019:  pop
    IL_001a:  ldloca.s   V_1
    IL_001c:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0022:  ldloc.1
    IL_0023:  br.s       IL_0034

    IL_0025:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_002a:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::IntVal
    IL_002f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0034:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0039:  dup
    IL_003a:  ldc.i4.1
    IL_003b:  ldarg.0
    IL_003c:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_0041:  stloc.0
    IL_0042:  ldloca.s   V_0
    IL_0044:  dup
    IL_0045:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_004a:  brtrue.s   IL_0058

    IL_004c:  pop
    IL_004d:  ldloca.s   V_1
    IL_004f:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0055:  ldloc.1
    IL_0056:  br.s       IL_006c

    IL_0058:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_005d:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Field
    IL_0062:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_0067:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_006c:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0071:  dup
    IL_0072:  ldc.i4.2
    IL_0073:  ldarg.0
    IL_0074:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_0079:  stloc.0
    IL_007a:  ldloca.s   V_0
    IL_007c:  dup
    IL_007d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0082:  brtrue.s   IL_0090

    IL_0084:  pop
    IL_0085:  ldloca.s   V_1
    IL_0087:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_008d:  ldloc.1
    IL_008e:  br.s       IL_00b3

    IL_0090:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_0095:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Field
    IL_009a:  dup
    IL_009b:  brtrue.s   IL_00a9

    IL_009d:  pop
    IL_009e:  ldloca.s   V_1
    IL_00a0:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00a6:  ldloc.1
    IL_00a7:  br.s       IL_00b3

    IL_00a9:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_00ae:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00b3:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_00b8:  dup
    IL_00b9:  ldc.i4.3
    IL_00ba:  ldarg.0
    IL_00bb:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_00c0:  stloc.0
    IL_00c1:  ldloca.s   V_0
    IL_00c3:  dup
    IL_00c4:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_00c9:  brtrue.s   IL_00d7

    IL_00cb:  pop
    IL_00cc:  ldloca.s   V_1
    IL_00ce:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00d4:  ldloc.1
    IL_00d5:  br.s       IL_00ee

    IL_00d7:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_00dc:  stloc.2
    IL_00dd:  ldloca.s   V_2
    IL_00df:  call       instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::get_Property2()
    IL_00e4:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::IntVal
    IL_00e9:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00ee:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_00f3:  dup
    IL_00f4:  ldc.i4.4
    IL_00f5:  ldarg.0
    IL_00f6:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_00fb:  stloc.0
    IL_00fc:  ldloca.s   V_0
    IL_00fe:  dup
    IL_00ff:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0104:  brtrue.s   IL_0112

    IL_0106:  pop
    IL_0107:  ldloca.s   V_1
    IL_0109:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_010f:  ldloc.1
    IL_0110:  br.s       IL_0145

    IL_0112:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_0117:  stloc.2
    IL_0118:  ldloca.s   V_2
    IL_011a:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::get_Property1()
    IL_011f:  stloc.3
    IL_0120:  ldloca.s   V_3
    IL_0122:  dup
    IL_0123:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0128:  brtrue.s   IL_0136

    IL_012a:  pop
    IL_012b:  ldloca.s   V_1
    IL_012d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0133:  ldloc.1
    IL_0134:  br.s       IL_0145

    IL_0136:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_013b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::IntVal
    IL_0140:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0145:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_014a:  dup
    IL_014b:  ldc.i4.5
    IL_014c:  ldarg.0
    IL_014d:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_0152:  stloc.0
    IL_0153:  ldloca.s   V_0
    IL_0155:  dup
    IL_0156:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_015b:  brtrue.s   IL_0169

    IL_015d:  pop
    IL_015e:  ldloca.s   V_1
    IL_0160:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0166:  ldloc.1
    IL_0167:  br.s       IL_0186

    IL_0169:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_016e:  stloc.2
    IL_016f:  ldloca.s   V_2
    IL_0171:  ldarg.0
    IL_0172:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_0177:  call       instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Method2(int32)
    IL_017c:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::IntVal
    IL_0181:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0186:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_018b:  dup
    IL_018c:  ldc.i4.6
    IL_018d:  ldarg.0
    IL_018e:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_0193:  stloc.0
    IL_0194:  ldloca.s   V_0
    IL_0196:  dup
    IL_0197:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_019c:  brtrue.s   IL_01aa

    IL_019e:  pop
    IL_019f:  ldloca.s   V_1
    IL_01a1:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01a7:  ldloc.1
    IL_01a8:  br.s       IL_01e3

    IL_01aa:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_01af:  stloc.2
    IL_01b0:  ldloca.s   V_2
    IL_01b2:  ldarg.0
    IL_01b3:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_01b8:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Method1(int32)
    IL_01bd:  stloc.3
    IL_01be:  ldloca.s   V_3
    IL_01c0:  dup
    IL_01c1:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_01c6:  brtrue.s   IL_01d4

    IL_01c8:  pop
    IL_01c9:  ldloca.s   V_1
    IL_01cb:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01d1:  ldloc.1
    IL_01d2:  br.s       IL_01e3

    IL_01d4:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_01d9:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::IntVal
    IL_01de:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01e3:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_01e8:  dup
    IL_01e9:  ldc.i4.7
    IL_01ea:  ldarg.0
    IL_01eb:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_01f0:  stloc.0
    IL_01f1:  ldloca.s   V_0
    IL_01f3:  dup
    IL_01f4:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_01f9:  brtrue.s   IL_0207

    IL_01fb:  pop
    IL_01fc:  ldloca.s   V_1
    IL_01fe:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0204:  ldloc.1
    IL_0205:  br.s       IL_0240

    IL_0207:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_020c:  stloc.2
    IL_020d:  ldloca.s   V_2
    IL_020f:  ldarg.0
    IL_0210:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_0215:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::get_Item(int32)
    IL_021a:  stloc.3
    IL_021b:  ldloca.s   V_3
    IL_021d:  dup
    IL_021e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0223:  brtrue.s   IL_0231

    IL_0225:  pop
    IL_0226:  ldloca.s   V_1
    IL_0228:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_022e:  ldloc.1
    IL_022f:  br.s       IL_0240

    IL_0231:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_0236:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::IntVal
    IL_023b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0240:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0245:  ret
  } // end of method NullPropagation::ChainsStruct

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

  .method private hidebysig static valuetype [mscorlib]System.Nullable`1<int32> 
          GenericUnconstrainedInt<(ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/ITest) T>(!!T t) cil managed
  {
    // Code size       37 (0x25)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0)
    IL_0000:  ldarg.0
    IL_0001:  box        !!T
    IL_0006:  brtrue.s   IL_0012

    IL_0008:  ldloca.s   V_0
    IL_000a:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0010:  ldloc.0
    IL_0011:  ret

    IL_0012:  ldarga.s   t
    IL_0014:  constrained. !!T
    IL_001a:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/ITest::Int()
    IL_001f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0024:  ret
  } // end of method NullPropagation::GenericUnconstrainedInt

  .method private hidebysig static valuetype [mscorlib]System.Nullable`1<int32> 
          GenericClassConstraintInt<class (ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/ITest) T>(!!T t) cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0)
    IL_0000:  ldarg.0
    IL_0001:  box        !!T
    IL_0006:  dup
    IL_0007:  brtrue.s   IL_0014

    IL_0009:  pop
    IL_000a:  ldloca.s   V_0
    IL_000c:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0012:  ldloc.0
    IL_0013:  ret

    IL_0014:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/ITest::Int()
    IL_0019:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_001e:  ret
  } // end of method NullPropagation::GenericClassConstraintInt

  .method private hidebysig static valuetype [mscorlib]System.Nullable`1<int32> 
          GenericStructConstraintInt<valuetype .ctor (ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/ITest, [mscorlib]System.ValueType) T>(valuetype [mscorlib]System.Nullable`1<!!T> t) cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             !!T V_1)
    IL_0000:  ldarga.s   t
    IL_0002:  call       instance bool valuetype [mscorlib]System.Nullable`1<!!T>::get_HasValue()
    IL_0007:  brtrue.s   IL_0013

    IL_0009:  ldloca.s   V_0
    IL_000b:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0011:  ldloc.0
    IL_0012:  ret

    IL_0013:  ldarga.s   t
    IL_0015:  call       instance !0 valuetype [mscorlib]System.Nullable`1<!!T>::GetValueOrDefault()
    IL_001a:  stloc.1
    IL_001b:  ldloca.s   V_1
    IL_001d:  constrained. !!T
    IL_0023:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/ITest::Int()
    IL_0028:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_002d:  ret
  } // end of method NullPropagation::GenericStructConstraintInt

  .method private hidebysig static valuetype [mscorlib]System.Nullable`1<int32> 
          GenericRefClassConstraintInt<class (ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/ITest) T>(!!T& t) cil managed
  {
    // Code size       36 (0x24)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldobj      !!T
    IL_0006:  box        !!T
    IL_000b:  dup
    IL_000c:  brtrue.s   IL_0019

    IL_000e:  pop
    IL_000f:  ldloca.s   V_0
    IL_0011:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0017:  ldloc.0
    IL_0018:  ret

    IL_0019:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/ITest::Int()
    IL_001e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0023:  ret
  } // end of method NullPropagation::GenericRefClassConstraintInt

  .method private hidebysig static valuetype [mscorlib]System.Nullable`1<int32> 
          GenericRefStructConstraintInt<valuetype .ctor (ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/ITest, [mscorlib]System.ValueType) T>(valuetype [mscorlib]System.Nullable`1<!!T>& t) cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             !!T V_1)
    IL_0000:  ldarg.0
    IL_0001:  dup
    IL_0002:  call       instance bool valuetype [mscorlib]System.Nullable`1<!!T>::get_HasValue()
    IL_0007:  brtrue.s   IL_0014

    IL_0009:  pop
    IL_000a:  ldloca.s   V_0
    IL_000c:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0012:  ldloc.0
    IL_0013:  ret

    IL_0014:  call       instance !0 valuetype [mscorlib]System.Nullable`1<!!T>::GetValueOrDefault()
    IL_0019:  stloc.1
    IL_001a:  ldloca.s   V_1
    IL_001c:  constrained. !!T
    IL_0022:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/ITest::Int()
    IL_0027:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_002c:  ret
  } // end of method NullPropagation::GenericRefStructConstraintInt

  .method private hidebysig static object 
          DynamicNullProp(object a) cil managed
  {
    .param [0]
    .custom instance void [System.Core]System.Runtime.CompilerServices.DynamicAttribute::.ctor() = ( 01 00 00 00 ) 
    .param [1]
    .custom instance void [System.Core]System.Runtime.CompilerServices.DynamicAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       332 (0x14c)
    .maxstack  10
    .locals init (object V_0,
             object V_1)
    IL_0000:  ldarg.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  brtrue.s   IL_0007

    IL_0005:  ldnull
    IL_0006:  ret

    IL_0007:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__1'
    IL_000c:  brtrue.s   IL_0048

    IL_000e:  ldc.i4.0
    IL_000f:  ldstr      "c"
    IL_0014:  ldnull
    IL_0015:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation
    IL_001a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001f:  ldc.i4.2
    IL_0020:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0025:  dup
    IL_0026:  ldc.i4.0
    IL_0027:  ldc.i4.0
    IL_0028:  ldnull
    IL_0029:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_002e:  stelem.ref
    IL_002f:  dup
    IL_0030:  ldc.i4.1
    IL_0031:  ldc.i4.3
    IL_0032:  ldnull
    IL_0033:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0038:  stelem.ref
    IL_0039:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::InvokeMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                 string,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Type>,
                                                                                                                                                                 class [mscorlib]System.Type,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_003e:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0043:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__1'
    IL_0048:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__1'
    IL_004d:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>>::Target
    IL_0052:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__1'
    IL_0057:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__0'
    IL_005c:  brtrue.s   IL_008d

    IL_005e:  ldc.i4.0
    IL_005f:  ldstr      "b"
    IL_0064:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation
    IL_0069:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006e:  ldc.i4.1
    IL_006f:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0074:  dup
    IL_0075:  ldc.i4.0
    IL_0076:  ldc.i4.0
    IL_0077:  ldnull
    IL_0078:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_007d:  stelem.ref
    IL_007e:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0083:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0088:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__0'
    IL_008d:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__0'
    IL_0092:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_0097:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__0'
    IL_009c:  ldloc.0
    IL_009d:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_00a2:  ldc.i4.1
    IL_00a3:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>::Invoke(!0,
                                                                                                                                                            !1,
                                                                                                                                                            !2)
    IL_00a8:  stloc.1
    IL_00a9:  ldloc.1
    IL_00aa:  brtrue.s   IL_00ae

    IL_00ac:  ldnull
    IL_00ad:  ret

    IL_00ae:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__3'
    IL_00b3:  brtrue.s   IL_00e9

    IL_00b5:  ldc.i4.0
    IL_00b6:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation
    IL_00bb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c0:  ldc.i4.2
    IL_00c1:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_00c6:  dup
    IL_00c7:  ldc.i4.0
    IL_00c8:  ldc.i4.0
    IL_00c9:  ldnull
    IL_00ca:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_00cf:  stelem.ref
    IL_00d0:  dup
    IL_00d1:  ldc.i4.1
    IL_00d2:  ldc.i4.3
    IL_00d3:  ldnull
    IL_00d4:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_00d9:  stelem.ref
    IL_00da:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetIndex(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                             class [mscorlib]System.Type,
                                                                                                                                                             class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_00df:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_00e4:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__3'
    IL_00e9:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__3'
    IL_00ee:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>>::Target
    IL_00f3:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__3'
    IL_00f8:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__2'
    IL_00fd:  brtrue.s   IL_012f

    IL_00ff:  ldc.i4.s   64
    IL_0101:  ldstr      "d"
    IL_0106:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation
    IL_010b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0110:  ldc.i4.1
    IL_0111:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0116:  dup
    IL_0117:  ldc.i4.0
    IL_0118:  ldc.i4.0
    IL_0119:  ldnull
    IL_011a:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_011f:  stelem.ref
    IL_0120:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0125:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_012a:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__2'
    IL_012f:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__2'
    IL_0134:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_0139:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__2'
    IL_013e:  ldloc.1
    IL_013f:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_0144:  ldc.i4.s   10
    IL_0146:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>::Invoke(!0,
                                                                                                                                                            !1,
                                                                                                                                                            !2)
    IL_014b:  ret
  } // end of method NullPropagation::DynamicNullProp

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
