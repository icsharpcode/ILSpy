



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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

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
    .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
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
      // Code size       7 (0x7)
      .maxstack  1
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass V_0)
      IL_0000:  nop
      IL_0001:  ldnull
      IL_0002:  stloc.0
      IL_0003:  br.s       IL_0005

      IL_0005:  ldloc.0
      IL_0006:  ret
    } // end of method MyClass::get_Item

    .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass 
            Method(int32 arg) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  1
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass V_0)
      IL_0000:  nop
      IL_0001:  ldnull
      IL_0002:  stloc.0
      IL_0003:  br.s       IL_0005

      IL_0005:  ldloc.0
      IL_0006:  ret
    } // end of method MyClass::Method

    .method public hidebysig instance void 
            Done() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method MyClass::Done

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
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
      // Code size       15 (0xf)
      .maxstack  1
      .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_0,
               valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_1)
      IL_0000:  nop
      IL_0001:  ldloca.s   V_0
      IL_0003:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>
      IL_0009:  ldloc.0
      IL_000a:  stloc.1
      IL_000b:  br.s       IL_000d

      IL_000d:  ldloc.1
      IL_000e:  ret
    } // end of method MyStruct::get_Item

    .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> 
            Method1(int32 arg) cil managed
    {
      // Code size       15 (0xf)
      .maxstack  1
      .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_0,
               valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_1)
      IL_0000:  nop
      IL_0001:  ldloca.s   V_0
      IL_0003:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>
      IL_0009:  ldloc.0
      IL_000a:  stloc.1
      IL_000b:  br.s       IL_000d

      IL_000d:  ldloc.1
      IL_000e:  ret
    } // end of method MyStruct::Method1

    .method public hidebysig instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct 
            Method2(int32 arg) cil managed
    {
      // Code size       15 (0xf)
      .maxstack  1
      .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct V_0,
               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct V_1)
      IL_0000:  nop
      IL_0001:  ldloca.s   V_0
      IL_0003:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct
      IL_0009:  ldloc.0
      IL_000a:  stloc.1
      IL_000b:  br.s       IL_000d

      IL_000d:  ldloc.1
      IL_000e:  ret
    } // end of method MyStruct::Method2

    .method public hidebysig instance void 
            Done() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
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
    // Code size       8 (0x8)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   9
    IL_0003:  stloc.0
    IL_0004:  br.s       IL_0006

    IL_0006:  ldloc.0
    IL_0007:  ret
  } // end of method NullPropagation::GetInt

  .method private hidebysig instance string 
          GetString() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method NullPropagation::GetString

  .method private hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass 
          GetMyClass() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass V_0)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method NullPropagation::GetMyClass

  .method private hidebysig instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> 
          GetMyStruct() cil managed
  {
    // Code size       15 (0xf)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_0,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_1)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_0
    IL_0003:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>
    IL_0009:  ldloc.0
    IL_000a:  stloc.1
    IL_000b:  br.s       IL_000d

    IL_000d:  ldloc.1
    IL_000e:  ret
  } // end of method NullPropagation::GetMyStruct

  .method public hidebysig instance string 
          Substring() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  2
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetString()
    IL_0007:  dup
    IL_0008:  brtrue.s   IL_000e

    IL_000a:  pop
    IL_000b:  ldnull
    IL_000c:  br.s       IL_0019

    IL_000e:  ldarg.0
    IL_000f:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_0014:  call       instance string [mscorlib]System.String::Substring(int32)
    IL_0019:  stloc.0
    IL_001a:  br.s       IL_001c

    IL_001c:  ldloc.0
    IL_001d:  ret
  } // end of method NullPropagation::Substring

  .method public hidebysig instance void 
          CallSubstringAndIgnoreResult() cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetString()
    IL_0007:  dup
    IL_0008:  brtrue.s   IL_000d

    IL_000a:  pop
    IL_000b:  br.s       IL_0019

    IL_000d:  ldarg.0
    IL_000e:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_0013:  call       instance string [mscorlib]System.String::Substring(int32)
    IL_0018:  pop
    IL_0019:  ret
  } // end of method NullPropagation::CallSubstringAndIgnoreResult

  .method private hidebysig instance void 
          Use<T>(!!T t) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method NullPropagation::Use

  .method public hidebysig instance void 
          CallDone() cil managed
  {
    // Code size       252 (0xfc)
    .maxstack  2
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0007:  dup
    IL_0008:  brtrue.s   IL_000d

    IL_000a:  pop
    IL_000b:  br.s       IL_0013

    IL_000d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_0012:  nop
    IL_0013:  ldarg.0
    IL_0014:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0019:  dup
    IL_001a:  brtrue.s   IL_001f

    IL_001c:  pop
    IL_001d:  br.s       IL_0030

    IL_001f:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Field
    IL_0024:  dup
    IL_0025:  brtrue.s   IL_002a

    IL_0027:  pop
    IL_0028:  br.s       IL_0030

    IL_002a:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_002f:  nop
    IL_0030:  ldarg.0
    IL_0031:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0036:  dup
    IL_0037:  brtrue.s   IL_003c

    IL_0039:  pop
    IL_003a:  br.s       IL_0047

    IL_003c:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Field
    IL_0041:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_0046:  nop
    IL_0047:  ldarg.0
    IL_0048:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_004d:  dup
    IL_004e:  brtrue.s   IL_0053

    IL_0050:  pop
    IL_0051:  br.s       IL_0064

    IL_0053:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::get_Property()
    IL_0058:  dup
    IL_0059:  brtrue.s   IL_005e

    IL_005b:  pop
    IL_005c:  br.s       IL_0064

    IL_005e:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_0063:  nop
    IL_0064:  ldarg.0
    IL_0065:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_006a:  dup
    IL_006b:  brtrue.s   IL_0070

    IL_006d:  pop
    IL_006e:  br.s       IL_007b

    IL_0070:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::get_Property()
    IL_0075:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_007a:  nop
    IL_007b:  ldarg.0
    IL_007c:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0081:  dup
    IL_0082:  brtrue.s   IL_0087

    IL_0084:  pop
    IL_0085:  br.s       IL_009e

    IL_0087:  ldarg.0
    IL_0088:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_008d:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Method(int32)
    IL_0092:  dup
    IL_0093:  brtrue.s   IL_0098

    IL_0095:  pop
    IL_0096:  br.s       IL_009e

    IL_0098:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_009d:  nop
    IL_009e:  ldarg.0
    IL_009f:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_00a4:  dup
    IL_00a5:  brtrue.s   IL_00aa

    IL_00a7:  pop
    IL_00a8:  br.s       IL_00bb

    IL_00aa:  ldarg.0
    IL_00ab:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_00b0:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Method(int32)
    IL_00b5:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_00ba:  nop
    IL_00bb:  ldarg.0
    IL_00bc:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_00c1:  dup
    IL_00c2:  brtrue.s   IL_00c7

    IL_00c4:  pop
    IL_00c5:  br.s       IL_00de

    IL_00c7:  ldarg.0
    IL_00c8:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_00cd:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::get_Item(int32)
    IL_00d2:  dup
    IL_00d3:  brtrue.s   IL_00d8

    IL_00d5:  pop
    IL_00d6:  br.s       IL_00de

    IL_00d8:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_00dd:  nop
    IL_00de:  ldarg.0
    IL_00df:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_00e4:  dup
    IL_00e5:  brtrue.s   IL_00ea

    IL_00e7:  pop
    IL_00e8:  br.s       IL_00fb

    IL_00ea:  ldarg.0
    IL_00eb:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_00f0:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::get_Item(int32)
    IL_00f5:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_00fa:  nop
    IL_00fb:  ret
  } // end of method NullPropagation::CallDone

  .method public hidebysig instance void 
          CallDoneStruct() cil managed
  {
    // Code size       399 (0x18f)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct V_1,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_0007:  stloc.0
    IL_0008:  ldloca.s   V_0
    IL_000a:  dup
    IL_000b:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0010:  brtrue.s   IL_0015

    IL_0012:  pop
    IL_0013:  br.s       IL_0023

    IL_0015:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_001a:  stloc.1
    IL_001b:  ldloca.s   V_1
    IL_001d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Done()
    IL_0022:  nop
    IL_0023:  ldarg.0
    IL_0024:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_0029:  stloc.0
    IL_002a:  ldloca.s   V_0
    IL_002c:  dup
    IL_002d:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0032:  brtrue.s   IL_0037

    IL_0034:  pop
    IL_0035:  br.s       IL_004d

    IL_0037:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_003c:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Field
    IL_0041:  dup
    IL_0042:  brtrue.s   IL_0047

    IL_0044:  pop
    IL_0045:  br.s       IL_004d

    IL_0047:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_004c:  nop
    IL_004d:  ldarg.0
    IL_004e:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_0053:  stloc.0
    IL_0054:  ldloca.s   V_0
    IL_0056:  dup
    IL_0057:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_005c:  brtrue.s   IL_0061

    IL_005e:  pop
    IL_005f:  br.s       IL_0071

    IL_0061:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_0066:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Field
    IL_006b:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_0070:  nop
    IL_0071:  ldarg.0
    IL_0072:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_0077:  stloc.0
    IL_0078:  ldloca.s   V_0
    IL_007a:  dup
    IL_007b:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0080:  brtrue.s   IL_0085

    IL_0082:  pop
    IL_0083:  br.s       IL_00ae

    IL_0085:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_008a:  stloc.1
    IL_008b:  ldloca.s   V_1
    IL_008d:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::get_Property1()
    IL_0092:  stloc.2
    IL_0093:  ldloca.s   V_2
    IL_0095:  dup
    IL_0096:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_009b:  brtrue.s   IL_00a0

    IL_009d:  pop
    IL_009e:  br.s       IL_00ae

    IL_00a0:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_00a5:  stloc.1
    IL_00a6:  ldloca.s   V_1
    IL_00a8:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Done()
    IL_00ad:  nop
    IL_00ae:  ldarg.0
    IL_00af:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_00b4:  stloc.0
    IL_00b5:  ldloca.s   V_0
    IL_00b7:  dup
    IL_00b8:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_00bd:  brtrue.s   IL_00c2

    IL_00bf:  pop
    IL_00c0:  br.s       IL_00d8

    IL_00c2:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_00c7:  stloc.1
    IL_00c8:  ldloca.s   V_1
    IL_00ca:  call       instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::get_Property2()
    IL_00cf:  stloc.1
    IL_00d0:  ldloca.s   V_1
    IL_00d2:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Done()
    IL_00d7:  nop
    IL_00d8:  ldarg.0
    IL_00d9:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_00de:  stloc.0
    IL_00df:  ldloca.s   V_0
    IL_00e1:  dup
    IL_00e2:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_00e7:  brtrue.s   IL_00ec

    IL_00e9:  pop
    IL_00ea:  br.s       IL_011b

    IL_00ec:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_00f1:  stloc.1
    IL_00f2:  ldloca.s   V_1
    IL_00f4:  ldarg.0
    IL_00f5:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_00fa:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Method1(int32)
    IL_00ff:  stloc.2
    IL_0100:  ldloca.s   V_2
    IL_0102:  dup
    IL_0103:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0108:  brtrue.s   IL_010d

    IL_010a:  pop
    IL_010b:  br.s       IL_011b

    IL_010d:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_0112:  stloc.1
    IL_0113:  ldloca.s   V_1
    IL_0115:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Done()
    IL_011a:  nop
    IL_011b:  ldarg.0
    IL_011c:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_0121:  stloc.0
    IL_0122:  ldloca.s   V_0
    IL_0124:  dup
    IL_0125:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_012a:  brtrue.s   IL_012f

    IL_012c:  pop
    IL_012d:  br.s       IL_014b

    IL_012f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_0134:  stloc.1
    IL_0135:  ldloca.s   V_1
    IL_0137:  ldarg.0
    IL_0138:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_013d:  call       instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Method2(int32)
    IL_0142:  stloc.1
    IL_0143:  ldloca.s   V_1
    IL_0145:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Done()
    IL_014a:  nop
    IL_014b:  ldarg.0
    IL_014c:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_0151:  stloc.0
    IL_0152:  ldloca.s   V_0
    IL_0154:  dup
    IL_0155:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_015a:  brtrue.s   IL_015f

    IL_015c:  pop
    IL_015d:  br.s       IL_018e

    IL_015f:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_0164:  stloc.1
    IL_0165:  ldloca.s   V_1
    IL_0167:  ldarg.0
    IL_0168:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_016d:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::get_Item(int32)
    IL_0172:  stloc.2
    IL_0173:  ldloca.s   V_2
    IL_0175:  dup
    IL_0176:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_017b:  brtrue.s   IL_0180

    IL_017d:  pop
    IL_017e:  br.s       IL_018e

    IL_0180:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_0185:  stloc.1
    IL_0186:  ldloca.s   V_1
    IL_0188:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Done()
    IL_018d:  nop
    IL_018e:  ret
  } // end of method NullPropagation::CallDoneStruct

  .method public hidebysig instance void 
          RequiredParentheses() cil managed
  {
    // Code size       131 (0x83)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_0,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_1,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0007:  dup
    IL_0008:  brtrue.s   IL_000e

    IL_000a:  pop
    IL_000b:  ldnull
    IL_000c:  br.s       IL_0013

    IL_000e:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Field
    IL_0013:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_0018:  nop
    IL_0019:  ldarg.0
    IL_001a:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_001f:  dup
    IL_0020:  brtrue.s   IL_0026

    IL_0022:  pop
    IL_0023:  ldnull
    IL_0024:  br.s       IL_0031

    IL_0026:  ldarg.0
    IL_0027:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_002c:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Method(int32)
    IL_0031:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Done()
    IL_0036:  nop
    IL_0037:  ldarg.0
    IL_0038:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_003d:  stloc.0
    IL_003e:  ldloca.s   V_0
    IL_0040:  dup
    IL_0041:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0046:  brtrue.s   IL_0054

    IL_0048:  pop
    IL_0049:  ldloca.s   V_1
    IL_004b:  initobj    valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>
    IL_0051:  ldloc.1
    IL_0052:  br.s       IL_0066

    IL_0054:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_0059:  stloc.2
    IL_005a:  ldloca.s   V_2
    IL_005c:  call       instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::get_Property2()
    IL_0061:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::.ctor(!0)
    IL_0066:  stloc.0
    IL_0067:  ldloca.s   V_0
    IL_0069:  dup
    IL_006a:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_006f:  brtrue.s   IL_0074

    IL_0071:  pop
    IL_0072:  br.s       IL_0082

    IL_0074:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_0079:  stloc.2
    IL_007a:  ldloca.s   V_2
    IL_007c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Done()
    IL_0081:  nop
    IL_0082:  ret
  } // end of method NullPropagation::RequiredParentheses

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int32>[] 
          ChainsOnClass() cil managed
  {
    // Code size       479 (0x1df)
    .maxstack  5
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32>[] V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   9
    IL_0003:  newarr     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0008:  dup
    IL_0009:  ldc.i4.0
    IL_000a:  ldarg.0
    IL_000b:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0010:  dup
    IL_0011:  brtrue.s   IL_001f

    IL_0013:  pop
    IL_0014:  ldloca.s   V_0
    IL_0016:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_001c:  ldloc.0
    IL_001d:  br.s       IL_0029

    IL_001f:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_0024:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0029:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_002e:  dup
    IL_002f:  ldc.i4.1
    IL_0030:  ldarg.0
    IL_0031:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0036:  dup
    IL_0037:  brtrue.s   IL_0045

    IL_0039:  pop
    IL_003a:  ldloca.s   V_0
    IL_003c:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0042:  ldloc.0
    IL_0043:  br.s       IL_0054

    IL_0045:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Field
    IL_004a:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_004f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0054:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0059:  dup
    IL_005a:  ldc.i4.2
    IL_005b:  ldarg.0
    IL_005c:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0061:  dup
    IL_0062:  brtrue.s   IL_0070

    IL_0064:  pop
    IL_0065:  ldloca.s   V_0
    IL_0067:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_006d:  ldloc.0
    IL_006e:  br.s       IL_008e

    IL_0070:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Field
    IL_0075:  dup
    IL_0076:  brtrue.s   IL_0084

    IL_0078:  pop
    IL_0079:  ldloca.s   V_0
    IL_007b:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0081:  ldloc.0
    IL_0082:  br.s       IL_008e

    IL_0084:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_0089:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_008e:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0093:  dup
    IL_0094:  ldc.i4.3
    IL_0095:  ldarg.0
    IL_0096:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_009b:  dup
    IL_009c:  brtrue.s   IL_00aa

    IL_009e:  pop
    IL_009f:  ldloca.s   V_0
    IL_00a1:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00a7:  ldloc.0
    IL_00a8:  br.s       IL_00b9

    IL_00aa:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::get_Property()
    IL_00af:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_00b4:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00b9:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_00be:  dup
    IL_00bf:  ldc.i4.4
    IL_00c0:  ldarg.0
    IL_00c1:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_00c6:  dup
    IL_00c7:  brtrue.s   IL_00d5

    IL_00c9:  pop
    IL_00ca:  ldloca.s   V_0
    IL_00cc:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00d2:  ldloc.0
    IL_00d3:  br.s       IL_00f3

    IL_00d5:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::get_Property()
    IL_00da:  dup
    IL_00db:  brtrue.s   IL_00e9

    IL_00dd:  pop
    IL_00de:  ldloca.s   V_0
    IL_00e0:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00e6:  ldloc.0
    IL_00e7:  br.s       IL_00f3

    IL_00e9:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_00ee:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00f3:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_00f8:  dup
    IL_00f9:  ldc.i4.5
    IL_00fa:  ldarg.0
    IL_00fb:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0100:  dup
    IL_0101:  brtrue.s   IL_010f

    IL_0103:  pop
    IL_0104:  ldloca.s   V_0
    IL_0106:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_010c:  ldloc.0
    IL_010d:  br.s       IL_0124

    IL_010f:  ldarg.0
    IL_0110:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_0115:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Method(int32)
    IL_011a:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_011f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0124:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0129:  dup
    IL_012a:  ldc.i4.6
    IL_012b:  ldarg.0
    IL_012c:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0131:  dup
    IL_0132:  brtrue.s   IL_0140

    IL_0134:  pop
    IL_0135:  ldloca.s   V_0
    IL_0137:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_013d:  ldloc.0
    IL_013e:  br.s       IL_0164

    IL_0140:  ldarg.0
    IL_0141:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_0146:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Method(int32)
    IL_014b:  dup
    IL_014c:  brtrue.s   IL_015a

    IL_014e:  pop
    IL_014f:  ldloca.s   V_0
    IL_0151:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0157:  ldloc.0
    IL_0158:  br.s       IL_0164

    IL_015a:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_015f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0164:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0169:  dup
    IL_016a:  ldc.i4.7
    IL_016b:  ldarg.0
    IL_016c:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0171:  dup
    IL_0172:  brtrue.s   IL_0180

    IL_0174:  pop
    IL_0175:  ldloca.s   V_0
    IL_0177:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_017d:  ldloc.0
    IL_017e:  br.s       IL_0195

    IL_0180:  ldarg.0
    IL_0181:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_0186:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::get_Item(int32)
    IL_018b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_0190:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0195:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_019a:  dup
    IL_019b:  ldc.i4.8
    IL_019c:  ldarg.0
    IL_019d:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_01a2:  dup
    IL_01a3:  brtrue.s   IL_01b1

    IL_01a5:  pop
    IL_01a6:  ldloca.s   V_0
    IL_01a8:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01ae:  ldloc.0
    IL_01af:  br.s       IL_01d5

    IL_01b1:  ldarg.0
    IL_01b2:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_01b7:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::get_Item(int32)
    IL_01bc:  dup
    IL_01bd:  brtrue.s   IL_01cb

    IL_01bf:  pop
    IL_01c0:  ldloca.s   V_0
    IL_01c2:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01c8:  ldloc.0
    IL_01c9:  br.s       IL_01d5

    IL_01cb:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_01d0:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01d5:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_01da:  stloc.1
    IL_01db:  br.s       IL_01dd

    IL_01dd:  ldloc.1
    IL_01de:  ret
  } // end of method NullPropagation::ChainsOnClass

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int32>[] 
          ChainsStruct() cil managed
  {
    // Code size       589 (0x24d)
    .maxstack  5
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct V_2,
             valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_3,
             valuetype [mscorlib]System.Nullable`1<int32>[] V_4)
    IL_0000:  nop
    IL_0001:  ldc.i4.8
    IL_0002:  newarr     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0007:  dup
    IL_0008:  ldc.i4.0
    IL_0009:  ldarg.0
    IL_000a:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_000f:  stloc.0
    IL_0010:  ldloca.s   V_0
    IL_0012:  dup
    IL_0013:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0018:  brtrue.s   IL_0026

    IL_001a:  pop
    IL_001b:  ldloca.s   V_1
    IL_001d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0023:  ldloc.1
    IL_0024:  br.s       IL_0035

    IL_0026:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_002b:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::IntVal
    IL_0030:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0035:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_003a:  dup
    IL_003b:  ldc.i4.1
    IL_003c:  ldarg.0
    IL_003d:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_0042:  stloc.0
    IL_0043:  ldloca.s   V_0
    IL_0045:  dup
    IL_0046:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_004b:  brtrue.s   IL_0059

    IL_004d:  pop
    IL_004e:  ldloca.s   V_1
    IL_0050:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0056:  ldloc.1
    IL_0057:  br.s       IL_006d

    IL_0059:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_005e:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Field
    IL_0063:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_0068:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_006d:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0072:  dup
    IL_0073:  ldc.i4.2
    IL_0074:  ldarg.0
    IL_0075:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_007a:  stloc.0
    IL_007b:  ldloca.s   V_0
    IL_007d:  dup
    IL_007e:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0083:  brtrue.s   IL_0091

    IL_0085:  pop
    IL_0086:  ldloca.s   V_1
    IL_0088:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_008e:  ldloc.1
    IL_008f:  br.s       IL_00b4

    IL_0091:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_0096:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Field
    IL_009b:  dup
    IL_009c:  brtrue.s   IL_00aa

    IL_009e:  pop
    IL_009f:  ldloca.s   V_1
    IL_00a1:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00a7:  ldloc.1
    IL_00a8:  br.s       IL_00b4

    IL_00aa:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_00af:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00b4:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_00b9:  dup
    IL_00ba:  ldc.i4.3
    IL_00bb:  ldarg.0
    IL_00bc:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_00c1:  stloc.0
    IL_00c2:  ldloca.s   V_0
    IL_00c4:  dup
    IL_00c5:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_00ca:  brtrue.s   IL_00d8

    IL_00cc:  pop
    IL_00cd:  ldloca.s   V_1
    IL_00cf:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00d5:  ldloc.1
    IL_00d6:  br.s       IL_00ef

    IL_00d8:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_00dd:  stloc.2
    IL_00de:  ldloca.s   V_2
    IL_00e0:  call       instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::get_Property2()
    IL_00e5:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::IntVal
    IL_00ea:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00ef:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_00f4:  dup
    IL_00f5:  ldc.i4.4
    IL_00f6:  ldarg.0
    IL_00f7:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_00fc:  stloc.0
    IL_00fd:  ldloca.s   V_0
    IL_00ff:  dup
    IL_0100:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0105:  brtrue.s   IL_0113

    IL_0107:  pop
    IL_0108:  ldloca.s   V_1
    IL_010a:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0110:  ldloc.1
    IL_0111:  br.s       IL_0146

    IL_0113:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_0118:  stloc.2
    IL_0119:  ldloca.s   V_2
    IL_011b:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::get_Property1()
    IL_0120:  stloc.3
    IL_0121:  ldloca.s   V_3
    IL_0123:  dup
    IL_0124:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0129:  brtrue.s   IL_0137

    IL_012b:  pop
    IL_012c:  ldloca.s   V_1
    IL_012e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0134:  ldloc.1
    IL_0135:  br.s       IL_0146

    IL_0137:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_013c:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::IntVal
    IL_0141:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0146:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_014b:  dup
    IL_014c:  ldc.i4.5
    IL_014d:  ldarg.0
    IL_014e:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_0153:  stloc.0
    IL_0154:  ldloca.s   V_0
    IL_0156:  dup
    IL_0157:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_015c:  brtrue.s   IL_016a

    IL_015e:  pop
    IL_015f:  ldloca.s   V_1
    IL_0161:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0167:  ldloc.1
    IL_0168:  br.s       IL_0187

    IL_016a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_016f:  stloc.2
    IL_0170:  ldloca.s   V_2
    IL_0172:  ldarg.0
    IL_0173:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_0178:  call       instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Method2(int32)
    IL_017d:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::IntVal
    IL_0182:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0187:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_018c:  dup
    IL_018d:  ldc.i4.6
    IL_018e:  ldarg.0
    IL_018f:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_0194:  stloc.0
    IL_0195:  ldloca.s   V_0
    IL_0197:  dup
    IL_0198:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_019d:  brtrue.s   IL_01ab

    IL_019f:  pop
    IL_01a0:  ldloca.s   V_1
    IL_01a2:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01a8:  ldloc.1
    IL_01a9:  br.s       IL_01e4

    IL_01ab:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_01b0:  stloc.2
    IL_01b1:  ldloca.s   V_2
    IL_01b3:  ldarg.0
    IL_01b4:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_01b9:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::Method1(int32)
    IL_01be:  stloc.3
    IL_01bf:  ldloca.s   V_3
    IL_01c1:  dup
    IL_01c2:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_01c7:  brtrue.s   IL_01d5

    IL_01c9:  pop
    IL_01ca:  ldloca.s   V_1
    IL_01cc:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01d2:  ldloc.1
    IL_01d3:  br.s       IL_01e4

    IL_01d5:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_01da:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::IntVal
    IL_01df:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01e4:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_01e9:  dup
    IL_01ea:  ldc.i4.7
    IL_01eb:  ldarg.0
    IL_01ec:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyStruct()
    IL_01f1:  stloc.0
    IL_01f2:  ldloca.s   V_0
    IL_01f4:  dup
    IL_01f5:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_01fa:  brtrue.s   IL_0208

    IL_01fc:  pop
    IL_01fd:  ldloca.s   V_1
    IL_01ff:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0205:  ldloc.1
    IL_0206:  br.s       IL_0241

    IL_0208:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_020d:  stloc.2
    IL_020e:  ldloca.s   V_2
    IL_0210:  ldarg.0
    IL_0211:  call       instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetInt()
    IL_0216:  call       instance valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::get_Item(int32)
    IL_021b:  stloc.3
    IL_021c:  ldloca.s   V_3
    IL_021e:  dup
    IL_021f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::get_HasValue()
    IL_0224:  brtrue.s   IL_0232

    IL_0226:  pop
    IL_0227:  ldloca.s   V_1
    IL_0229:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_022f:  ldloc.1
    IL_0230:  br.s       IL_0241

    IL_0232:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct>::GetValueOrDefault()
    IL_0237:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct::IntVal
    IL_023c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0241:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0246:  stloc.s    V_4
    IL_0248:  br.s       IL_024a

    IL_024a:  ldloc.s    V_4
    IL_024c:  ret
  } // end of method NullPropagation::ChainsStruct

  .method public hidebysig instance int32 
          CoalescingReturn() cil managed
  {
    // Code size       24 (0x18)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0007:  dup
    IL_0008:  brtrue.s   IL_000e

    IL_000a:  pop
    IL_000b:  ldc.i4.1
    IL_000c:  br.s       IL_0013

    IL_000e:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_0013:  stloc.0
    IL_0014:  br.s       IL_0016

    IL_0016:  ldloc.0
    IL_0017:  ret
  } // end of method NullPropagation::CoalescingReturn

  .method public hidebysig instance void 
          Coalescing() cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.0
    IL_0003:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0008:  dup
    IL_0009:  brtrue.s   IL_000f

    IL_000b:  pop
    IL_000c:  ldc.i4.1
    IL_000d:  br.s       IL_0014

    IL_000f:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_0014:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::Use<int32>(!!0)
    IL_0019:  nop
    IL_001a:  ret
  } // end of method NullPropagation::Coalescing

  .method public hidebysig instance void 
          CoalescingString() cil managed
  {
    // Code size       36 (0x24)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.0
    IL_0003:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::GetMyClass()
    IL_0008:  dup
    IL_0009:  brtrue.s   IL_000f

    IL_000b:  pop
    IL_000c:  ldnull
    IL_000d:  br.s       IL_0014

    IL_000f:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::Text
    IL_0014:  dup
    IL_0015:  brtrue.s   IL_001d

    IL_0017:  pop
    IL_0018:  ldstr      "Hello"
    IL_001d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation::Use<string>(!!0)
    IL_0022:  nop
    IL_0023:  ret
  } // end of method NullPropagation::CoalescingString

  .method public hidebysig instance void 
          InvokeDelegate(class [mscorlib]System.EventHandler eh) cil managed
  {
    // Code size       20 (0x14)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  brtrue.s   IL_0006

    IL_0004:  br.s       IL_0013

    IL_0006:  ldarg.1
    IL_0007:  ldnull
    IL_0008:  ldsfld     class [mscorlib]System.EventArgs [mscorlib]System.EventArgs::Empty
    IL_000d:  callvirt   instance void [mscorlib]System.EventHandler::Invoke(object,
                                                                             class [mscorlib]System.EventArgs)
    IL_0012:  nop
    IL_0013:  ret
  } // end of method NullPropagation::InvokeDelegate

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<int32> 
          InvokeDelegate(class [mscorlib]System.Func`1<int32> f) cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  brtrue.s   IL_000f

    IL_0004:  ldloca.s   V_0
    IL_0006:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_000c:  ldloc.0
    IL_000d:  br.s       IL_001a

    IL_000f:  ldarg.1
    IL_0010:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0015:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_001a:  stloc.1
    IL_001b:  br.s       IL_001d

    IL_001d:  ldloc.1
    IL_001e:  ret
  } // end of method NullPropagation::InvokeDelegate

  .method private hidebysig instance void 
          NotNullPropagation(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass c) cil managed
  {
    // Code size       77 (0x4d)
    .maxstack  2
    .locals init (bool V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  brfalse.s  IL_000f

    IL_0004:  ldarg.1
    IL_0005:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_000a:  ldc.i4.0
    IL_000b:  cgt.un
    IL_000d:  br.s       IL_0010

    IL_000f:  ldc.i4.0
    IL_0010:  stloc.0
    IL_0011:  ldloc.0
    IL_0012:  brfalse.s  IL_0021

    IL_0014:  nop
    IL_0015:  ldstr      "non-zero"
    IL_001a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001f:  nop
    IL_0020:  nop
    IL_0021:  ldarg.1
    IL_0022:  brfalse.s  IL_002f

    IL_0024:  ldarg.1
    IL_0025:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_002a:  ldc.i4.0
    IL_002b:  ceq
    IL_002d:  br.s       IL_0030

    IL_002f:  ldc.i4.1
    IL_0030:  stloc.1
    IL_0031:  ldloc.1
    IL_0032:  brfalse.s  IL_0041

    IL_0034:  nop
    IL_0035:  ldstr      "null or zero"
    IL_003a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003f:  nop
    IL_0040:  nop
    IL_0041:  ldstr      "end of method"
    IL_0046:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004b:  nop
    IL_004c:  ret
  } // end of method NullPropagation::NotNullPropagation

  .method private hidebysig instance void 
          Setter(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass c) cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  2
    .locals init (bool V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldnull
    IL_0003:  cgt.un
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  brfalse.s  IL_0012

    IL_0009:  nop
    IL_000a:  ldarg.1
    IL_000b:  ldc.i4.1
    IL_000c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::IntVal
    IL_0011:  nop
    IL_0012:  call       void [mscorlib]System.Console::WriteLine()
    IL_0017:  nop
    IL_0018:  ldarg.1
    IL_0019:  ldnull
    IL_001a:  cgt.un
    IL_001c:  stloc.1
    IL_001d:  ldloc.1
    IL_001e:  brfalse.s  IL_002a

    IL_0020:  nop
    IL_0021:  ldarg.1
    IL_0022:  ldnull
    IL_0023:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass::set_Property(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyClass)
    IL_0028:  nop
    IL_0029:  nop
    IL_002a:  ret
  } // end of method NullPropagation::Setter

  .method private hidebysig static valuetype [mscorlib]System.Nullable`1<int32> 
          GenericUnconstrainedInt<(ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/ITest) T>(!!T t) cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  box        !!T
    IL_0007:  brtrue.s   IL_0014

    IL_0009:  ldloca.s   V_0
    IL_000b:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0011:  ldloc.0
    IL_0012:  br.s       IL_0026

    IL_0014:  ldarga.s   t
    IL_0016:  constrained. !!T
    IL_001c:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/ITest::Int()
    IL_0021:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0026:  stloc.1
    IL_0027:  br.s       IL_0029

    IL_0029:  ldloc.1
    IL_002a:  ret
  } // end of method NullPropagation::GenericUnconstrainedInt

  .method private hidebysig static valuetype [mscorlib]System.Nullable`1<int32> 
          GenericClassConstraintInt<class (ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/ITest) T>(!!T t) cil managed
  {
    // Code size       37 (0x25)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  box        !!T
    IL_0007:  dup
    IL_0008:  brtrue.s   IL_0016

    IL_000a:  pop
    IL_000b:  ldloca.s   V_0
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0013:  ldloc.0
    IL_0014:  br.s       IL_0020

    IL_0016:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/ITest::Int()
    IL_001b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0020:  stloc.1
    IL_0021:  br.s       IL_0023

    IL_0023:  ldloc.1
    IL_0024:  ret
  } // end of method NullPropagation::GenericClassConstraintInt

  .method private hidebysig static valuetype [mscorlib]System.Nullable`1<int32> 
          GenericStructConstraintInt<valuetype .ctor (ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/ITest, [mscorlib]System.ValueType) T>(valuetype [mscorlib]System.Nullable`1<!!T> t) cil managed
  {
    // Code size       52 (0x34)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             !!T V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  nop
    IL_0001:  ldarga.s   t
    IL_0003:  call       instance bool valuetype [mscorlib]System.Nullable`1<!!T>::get_HasValue()
    IL_0008:  brtrue.s   IL_0015

    IL_000a:  ldloca.s   V_0
    IL_000c:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0012:  ldloc.0
    IL_0013:  br.s       IL_002f

    IL_0015:  ldarga.s   t
    IL_0017:  call       instance !0 valuetype [mscorlib]System.Nullable`1<!!T>::GetValueOrDefault()
    IL_001c:  stloc.1
    IL_001d:  ldloca.s   V_1
    IL_001f:  constrained. !!T
    IL_0025:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/ITest::Int()
    IL_002a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_002f:  stloc.2
    IL_0030:  br.s       IL_0032

    IL_0032:  ldloc.2
    IL_0033:  ret
  } // end of method NullPropagation::GenericStructConstraintInt

  .method private hidebysig static valuetype [mscorlib]System.Nullable`1<int32> 
          GenericRefClassConstraintInt<class (ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/ITest) T>(!!T& t) cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             valuetype [mscorlib]System.Nullable`1<int32> V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldobj      !!T
    IL_0007:  box        !!T
    IL_000c:  dup
    IL_000d:  brtrue.s   IL_001b

    IL_000f:  pop
    IL_0010:  ldloca.s   V_0
    IL_0012:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0018:  ldloc.0
    IL_0019:  br.s       IL_0025

    IL_001b:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/ITest::Int()
    IL_0020:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0025:  stloc.1
    IL_0026:  br.s       IL_0028

    IL_0028:  ldloc.1
    IL_0029:  ret
  } // end of method NullPropagation::GenericRefClassConstraintInt

  .method private hidebysig static valuetype [mscorlib]System.Nullable`1<int32> 
          GenericRefStructConstraintInt<valuetype .ctor (ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/ITest, [mscorlib]System.ValueType) T>(valuetype [mscorlib]System.Nullable`1<!!T>& t) cil managed
  {
    // Code size       51 (0x33)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0,
             !!T V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  dup
    IL_0003:  call       instance bool valuetype [mscorlib]System.Nullable`1<!!T>::get_HasValue()
    IL_0008:  brtrue.s   IL_0016

    IL_000a:  pop
    IL_000b:  ldloca.s   V_0
    IL_000d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0013:  ldloc.0
    IL_0014:  br.s       IL_002e

    IL_0016:  call       instance !0 valuetype [mscorlib]System.Nullable`1<!!T>::GetValueOrDefault()
    IL_001b:  stloc.1
    IL_001c:  ldloca.s   V_1
    IL_001e:  constrained. !!T
    IL_0024:  callvirt   instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/ITest::Int()
    IL_0029:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_002e:  stloc.2
    IL_002f:  br.s       IL_0031

    IL_0031:  ldloc.2
    IL_0032:  ret
  } // end of method NullPropagation::GenericRefStructConstraintInt

  .method private hidebysig static object 
          DynamicNullProp(object a) cil managed
  {
    .param [0]
    .custom instance void [System.Core]System.Runtime.CompilerServices.DynamicAttribute::.ctor() = ( 01 00 00 00 ) 
    .param [1]
    .custom instance void [System.Core]System.Runtime.CompilerServices.DynamicAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       353 (0x161)
    .maxstack  10
    .locals init (object V_0,
             object V_1,
             object V_2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldloc.0
    IL_0004:  brtrue.s   IL_000c

    IL_0006:  ldnull
    IL_0007:  br         IL_015c

    IL_000c:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__1'
    IL_0011:  brfalse.s  IL_0015

    IL_0013:  br.s       IL_004f

    IL_0015:  ldc.i4.0
    IL_0016:  ldstr      "c"
    IL_001b:  ldnull
    IL_001c:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation
    IL_0021:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0026:  ldc.i4.2
    IL_0027:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_002c:  dup
    IL_002d:  ldc.i4.0
    IL_002e:  ldc.i4.0
    IL_002f:  ldnull
    IL_0030:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0035:  stelem.ref
    IL_0036:  dup
    IL_0037:  ldc.i4.1
    IL_0038:  ldc.i4.3
    IL_0039:  ldnull
    IL_003a:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_003f:  stelem.ref
    IL_0040:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::InvokeMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                 string,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Type>,
                                                                                                                                                                 class [mscorlib]System.Type,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0045:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_004a:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__1'
    IL_004f:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__1'
    IL_0054:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>>::Target
    IL_0059:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__1'
    IL_005e:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__0'
    IL_0063:  brfalse.s  IL_0067

    IL_0065:  br.s       IL_0096

    IL_0067:  ldc.i4.0
    IL_0068:  ldstr      "b"
    IL_006d:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation
    IL_0072:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0077:  ldc.i4.1
    IL_0078:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_007d:  dup
    IL_007e:  ldc.i4.0
    IL_007f:  ldc.i4.0
    IL_0080:  ldnull
    IL_0081:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0086:  stelem.ref
    IL_0087:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_008c:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0091:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__0'
    IL_0096:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__0'
    IL_009b:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_00a0:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__0'
    IL_00a5:  ldloc.0
    IL_00a6:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_00ab:  ldc.i4.1
    IL_00ac:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>::Invoke(!0,
                                                                                                                                                            !1,
                                                                                                                                                            !2)
    IL_00b1:  stloc.1
    IL_00b2:  ldloc.1
    IL_00b3:  brtrue.s   IL_00bb

    IL_00b5:  ldnull
    IL_00b6:  br         IL_015c

    IL_00bb:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__3'
    IL_00c0:  brfalse.s  IL_00c4

    IL_00c2:  br.s       IL_00f8

    IL_00c4:  ldc.i4.0
    IL_00c5:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation
    IL_00ca:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00cf:  ldc.i4.2
    IL_00d0:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_00d5:  dup
    IL_00d6:  ldc.i4.0
    IL_00d7:  ldc.i4.0
    IL_00d8:  ldnull
    IL_00d9:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_00de:  stelem.ref
    IL_00df:  dup
    IL_00e0:  ldc.i4.1
    IL_00e1:  ldc.i4.3
    IL_00e2:  ldnull
    IL_00e3:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_00e8:  stelem.ref
    IL_00e9:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetIndex(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                             class [mscorlib]System.Type,
                                                                                                                                                             class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_00ee:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_00f3:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__3'
    IL_00f8:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__3'
    IL_00fd:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>>::Target
    IL_0102:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__3'
    IL_0107:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__2'
    IL_010c:  brfalse.s  IL_0110

    IL_010e:  br.s       IL_0140

    IL_0110:  ldc.i4.s   64
    IL_0112:  ldstr      "d"
    IL_0117:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation
    IL_011c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0121:  ldc.i4.1
    IL_0122:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0127:  dup
    IL_0128:  ldc.i4.0
    IL_0129:  ldc.i4.0
    IL_012a:  ldnull
    IL_012b:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0130:  stelem.ref
    IL_0131:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0136:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_013b:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__2'
    IL_0140:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__2'
    IL_0145:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_014a:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/'<>o__27'::'<>p__2'
    IL_014f:  ldloc.1
    IL_0150:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_0155:  ldc.i4.s   10
    IL_0157:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,object,int32,object>::Invoke(!0,
                                                                                                                                                            !1,
                                                                                                                                                            !2)
    IL_015c:  stloc.2
    IL_015d:  br.s       IL_015f

    IL_015f:  ldloc.2
    IL_0160:  ret
  } // end of method NullPropagation::DynamicNullProp

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method NullPropagation::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
