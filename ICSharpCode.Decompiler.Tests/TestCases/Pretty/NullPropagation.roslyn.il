



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
    // Code size       36 (0x24)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct> V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.NullPropagation/MyStruct V_1)
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
    IL_0023:  ret
  } // end of method NullPropagation::CallDoneStruct

  .method public hidebysig instance void 
          RequiredParentheses() cil managed
  {
    // Code size       56 (0x38)
    .maxstack  8
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
    IL_0037:  ret
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
