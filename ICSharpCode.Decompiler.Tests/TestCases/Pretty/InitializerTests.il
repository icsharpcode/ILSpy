



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
.assembly InitializerTests
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module InitializerTests.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.Extensions
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig static void  Add(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32> inst,
                                            string a,
                                            string b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method Extensions::Add

  .method public hidebysig static void  Add<T>(class [mscorlib]System.Collections.Generic.IList`1<valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<string,string>> collection,
                                               string key,
                                               !!T 'value',
                                               [opt] class [mscorlib]System.Func`2<!!T,string> convert) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
    .param [4] = nullref
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method Extensions::Add

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.Extensions

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases
       extends [mscorlib]System.Object
{
  .class auto ansi nested public beforefieldinit CustomList`1<T>
         extends [mscorlib]System.Object
         implements class [mscorlib]System.Collections.Generic.IEnumerable`1<!T>,
                    [mscorlib]System.Collections.IEnumerable
  {
    .method public hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.Generic.IEnumerator`1<!T> 
            GetEnumerator() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomList`1::GetEnumerator

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method CustomList`1::System.Collections.IEnumerable.GetEnumerator

    .method public hidebysig instance void 
            Add<T2>(string name) cil managed
    {
      // Code size       24 (0x18)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,class [mscorlib]System.Type>::.ctor()
      IL_0006:  ldarg.1
      IL_0007:  ldtoken    !!T2
      IL_000c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
      IL_0011:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,class [mscorlib]System.Type>::Add(!0,
                                                                                                                                          !1)
      IL_0016:  nop
      IL_0017:  ret
    } // end of method CustomList`1::Add

    .method public hidebysig instance void 
            Add(int32[] ints) cil managed
    {
      .param [1]
      .custom instance void [mscorlib]System.ParamArrayAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method CustomList`1::Add

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method CustomList`1::.ctor

  } // end of class CustomList`1

  .class auto ansi nested public beforefieldinit C
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
    .field public int32 Z
    .field public valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S Y
    .field public class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S> L
    .method public hidebysig specialname 
            instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S 
            get_Item(int32 index) cil managed
    {
      // Code size       15 (0xf)
      .maxstack  1
      .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S V_0,
               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S V_1)
      IL_0000:  nop
      IL_0001:  ldloca.s   V_1
      IL_0003:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S
      IL_0009:  ldloc.1
      IL_000a:  stloc.0
      IL_000b:  br.s       IL_000d

      IL_000d:  ldloc.0
      IL_000e:  ret
    } // end of method C::get_Item

    .method public hidebysig specialname 
            instance void  set_Item(int32 index,
                                    valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S 'value') cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method C::set_Item

    .method public hidebysig specialname 
            instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S 
            get_Item(object key) cil managed
    {
      // Code size       15 (0xf)
      .maxstack  1
      .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S V_0,
               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S V_1)
      IL_0000:  nop
      IL_0001:  ldloca.s   V_1
      IL_0003:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S
      IL_0009:  ldloc.1
      IL_000a:  stloc.0
      IL_000b:  br.s       IL_000d

      IL_000d:  ldloc.0
      IL_000e:  ret
    } // end of method C::get_Item

    .method public hidebysig specialname 
            instance void  set_Item(object key,
                                    valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S 'value') cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method C::set_Item

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method C::.ctor

    .property instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S
            Item(int32)
    {
      .get instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::get_Item(int32)
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::set_Item(int32,
                                                                                                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S)
    } // end of property C::Item
    .property instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S
            Item(object)
    {
      .get instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::get_Item(object)
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::set_Item(object,
                                                                                                              valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S)
    } // end of property C::Item
  } // end of class C

  .class sequential ansi sealed nested public beforefieldinit S
         extends [mscorlib]System.ValueType
  {
    .field public int32 A
    .field public int32 B
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(int32 a) cil managed
    {
      // Code size       16 (0x10)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldarg.1
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::A
      IL_0008:  ldarg.0
      IL_0009:  ldc.i4.0
      IL_000a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::B
      IL_000f:  ret
    } // end of method S::.ctor

  } // end of class S

  .class auto ansi sealed nested private MyEnum
         extends [mscorlib]System.Enum
  {
    .field public specialname rtspecialname int32 value__
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum a = int32(0x00000000)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum b = int32(0x00000001)
  } // end of class MyEnum

  .class auto ansi sealed nested private MyEnum2
         extends [mscorlib]System.Enum
  {
    .field public specialname rtspecialname int32 value__
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2 c = int32(0x00000000)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2 d = int32(0x00000001)
  } // end of class MyEnum2

  .class auto ansi nested private beforefieldinit Data
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
    .field public class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> FieldList
    .field private class [mscorlib]System.EventHandler TestEvent
    .field private valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum '<a>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum '<b>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> '<PropertyList>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data '<MoreData>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData '<NestedStruct>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig specialname 
            instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum 
            get_a() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::'<a>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method Data::get_a

    .method public hidebysig specialname 
            instance void  set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::'<a>k__BackingField'
      IL_0007:  ret
    } // end of method Data::set_a

    .method public hidebysig specialname 
            instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum 
            get_b() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::'<b>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method Data::get_b

    .method public hidebysig specialname 
            instance void  set_b(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::'<b>k__BackingField'
      IL_0007:  ret
    } // end of method Data::set_b

    .method public hidebysig specialname 
            instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> 
            get_PropertyList() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::'<PropertyList>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method Data::get_PropertyList

    .method public hidebysig specialname 
            instance void  set_PropertyList(class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::'<PropertyList>k__BackingField'
      IL_0007:  ret
    } // end of method Data::set_PropertyList

    .method public hidebysig specialname 
            instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data 
            get_MoreData() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::'<MoreData>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method Data::get_MoreData

    .method public hidebysig specialname 
            instance void  set_MoreData(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::'<MoreData>k__BackingField'
      IL_0007:  ret
    } // end of method Data::set_MoreData

    .method public hidebysig specialname 
            instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData 
            get_NestedStruct() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::'<NestedStruct>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method Data::get_NestedStruct

    .method public hidebysig specialname 
            instance void  set_NestedStruct(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::'<NestedStruct>k__BackingField'
      IL_0007:  ret
    } // end of method Data::set_NestedStruct

    .method public hidebysig specialname 
            instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data 
            get_Item(int32 i) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  1
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0)
      IL_0000:  nop
      IL_0001:  ldnull
      IL_0002:  stloc.0
      IL_0003:  br.s       IL_0005

      IL_0005:  ldloc.0
      IL_0006:  ret
    } // end of method Data::get_Item

    .method public hidebysig specialname 
            instance void  set_Item(int32 i,
                                    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data 'value') cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method Data::set_Item

    .method public hidebysig specialname 
            instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data 
            get_Item(int32 i,
                     string j) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  1
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0)
      IL_0000:  nop
      IL_0001:  ldnull
      IL_0002:  stloc.0
      IL_0003:  br.s       IL_0005

      IL_0005:  ldloc.0
      IL_0006:  ret
    } // end of method Data::get_Item

    .method public hidebysig specialname 
            instance void  set_Item(int32 i,
                                    string j,
                                    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data 'value') cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method Data::set_Item

    .method public hidebysig specialname 
            instance void  add_TestEvent(class [mscorlib]System.EventHandler 'value') cil managed
    {
      // Code size       48 (0x30)
      .maxstack  3
      .locals init (class [mscorlib]System.EventHandler V_0,
               class [mscorlib]System.EventHandler V_1,
               class [mscorlib]System.EventHandler V_2,
               bool V_3)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::TestEvent
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  stloc.1
      IL_0009:  ldloc.1
      IL_000a:  ldarg.1
      IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Combine(class [mscorlib]System.Delegate,
                                                                                              class [mscorlib]System.Delegate)
      IL_0010:  castclass  [mscorlib]System.EventHandler
      IL_0015:  stloc.2
      IL_0016:  ldarg.0
      IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::TestEvent
      IL_001c:  ldloc.2
      IL_001d:  ldloc.1
      IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [mscorlib]System.EventHandler>(!!0&,
                                                                                                                            !!0,
                                                                                                                            !!0)
      IL_0023:  stloc.0
      IL_0024:  ldloc.0
      IL_0025:  ldloc.1
      IL_0026:  ceq
      IL_0028:  ldc.i4.0
      IL_0029:  ceq
      IL_002b:  stloc.3
      IL_002c:  ldloc.3
      IL_002d:  brtrue.s   IL_0007

      IL_002f:  ret
    } // end of method Data::add_TestEvent

    .method public hidebysig specialname 
            instance void  remove_TestEvent(class [mscorlib]System.EventHandler 'value') cil managed
    {
      // Code size       48 (0x30)
      .maxstack  3
      .locals init (class [mscorlib]System.EventHandler V_0,
               class [mscorlib]System.EventHandler V_1,
               class [mscorlib]System.EventHandler V_2,
               bool V_3)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::TestEvent
      IL_0006:  stloc.0
      IL_0007:  ldloc.0
      IL_0008:  stloc.1
      IL_0009:  ldloc.1
      IL_000a:  ldarg.1
      IL_000b:  call       class [mscorlib]System.Delegate [mscorlib]System.Delegate::Remove(class [mscorlib]System.Delegate,
                                                                                             class [mscorlib]System.Delegate)
      IL_0010:  castclass  [mscorlib]System.EventHandler
      IL_0015:  stloc.2
      IL_0016:  ldarg.0
      IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::TestEvent
      IL_001c:  ldloc.2
      IL_001d:  ldloc.1
      IL_001e:  call       !!0 [mscorlib]System.Threading.Interlocked::CompareExchange<class [mscorlib]System.EventHandler>(!!0&,
                                                                                                                            !!0,
                                                                                                                            !!0)
      IL_0023:  stloc.0
      IL_0024:  ldloc.0
      IL_0025:  ldloc.1
      IL_0026:  ceq
      IL_0028:  ldc.i4.0
      IL_0029:  ceq
      IL_002b:  stloc.3
      IL_002c:  ldloc.3
      IL_002d:  brtrue.s   IL_0007

      IL_002f:  ret
    } // end of method Data::remove_TestEvent

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       19 (0x13)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::.ctor()
      IL_0006:  stfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::FieldList
      IL_000b:  ldarg.0
      IL_000c:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0011:  nop
      IL_0012:  ret
    } // end of method Data::.ctor

    .event [mscorlib]System.EventHandler TestEvent
    {
      .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::remove_TestEvent(class [mscorlib]System.EventHandler)
      .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::add_TestEvent(class [mscorlib]System.EventHandler)
    } // end of event Data::TestEvent
    .property instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum
            a()
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
      .get instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_a()
    } // end of property Data::a
    .property instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum
            b()
    {
      .get instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_b()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_b(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    } // end of property Data::b
    .property instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>
            PropertyList()
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_PropertyList(class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>)
      .get instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_PropertyList()
    } // end of property Data::PropertyList
    .property instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data
            MoreData()
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_MoreData(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data)
      .get instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    } // end of property Data::MoreData
    .property instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData
            NestedStruct()
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_NestedStruct(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData)
      .get instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_NestedStruct()
    } // end of property Data::NestedStruct
    .property instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data
            Item(int32)
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_Item(int32,
                                                                                                                 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data)
      .get instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_Item(int32)
    } // end of property Data::Item
    .property instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data
            Item(int32,
                 string)
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_Item(int32,
                                                                                                                 string,
                                                                                                                 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data)
      .get instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_Item(int32,
                                                                                                                                                                                                string)
    } // end of property Data::Item
  } // end of class Data

  .class sequential ansi sealed nested private beforefieldinit StructData
         extends [mscorlib]System.ValueType
  {
    .field public int32 Field
    .field private int32 '<Property>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data '<MoreData>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig specialname 
            instance int32  get_Property() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::'<Property>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method StructData::get_Property

    .method public hidebysig specialname 
            instance void  set_Property(int32 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::'<Property>k__BackingField'
      IL_0007:  ret
    } // end of method StructData::set_Property

    .method public hidebysig specialname 
            instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data 
            get_MoreData() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::'<MoreData>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method StructData::get_MoreData

    .method public hidebysig specialname 
            instance void  set_MoreData(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::'<MoreData>k__BackingField'
      IL_0007:  ret
    } // end of method StructData::set_MoreData

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(int32 initialValue) cil managed
    {
      // Code size       24 (0x18)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData
      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::Field
      IL_000f:  ldarg.0
      IL_0010:  ldarg.1
      IL_0011:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::set_Property(int32)
      IL_0016:  nop
      IL_0017:  ret
    } // end of method StructData::.ctor

    .property instance int32 Property()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::get_Property()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::set_Property(int32)
    } // end of property StructData::Property
    .property instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data
            MoreData()
    {
      .get instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::get_MoreData()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::set_MoreData(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data)
    } // end of property StructData::MoreData
  } // end of class StructData

  .class auto ansi nested public beforefieldinit Item
         extends [mscorlib]System.Object
  {
    .field private string '<Text>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private valuetype [mscorlib]System.Decimal '<Value>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private valuetype [mscorlib]System.Decimal '<Value2>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private string '<Value3>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private string '<Value4>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private string '<Value5>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private string '<Value6>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig specialname 
            instance string  get_Text() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (string V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::'<Text>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method Item::get_Text

    .method public hidebysig specialname 
            instance void  set_Text(string 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::'<Text>k__BackingField'
      IL_0007:  ret
    } // end of method Item::set_Text

    .method public hidebysig specialname 
            instance valuetype [mscorlib]System.Decimal 
            get_Value() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (valuetype [mscorlib]System.Decimal V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::'<Value>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method Item::get_Value

    .method public hidebysig specialname 
            instance void  set_Value(valuetype [mscorlib]System.Decimal 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::'<Value>k__BackingField'
      IL_0007:  ret
    } // end of method Item::set_Value

    .method public hidebysig specialname 
            instance valuetype [mscorlib]System.Decimal 
            get_Value2() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (valuetype [mscorlib]System.Decimal V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::'<Value2>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method Item::get_Value2

    .method public hidebysig specialname 
            instance void  set_Value2(valuetype [mscorlib]System.Decimal 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::'<Value2>k__BackingField'
      IL_0007:  ret
    } // end of method Item::set_Value2

    .method public hidebysig specialname 
            instance string  get_Value3() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (string V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::'<Value3>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method Item::get_Value3

    .method public hidebysig specialname 
            instance void  set_Value3(string 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::'<Value3>k__BackingField'
      IL_0007:  ret
    } // end of method Item::set_Value3

    .method public hidebysig specialname 
            instance string  get_Value4() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (string V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::'<Value4>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method Item::get_Value4

    .method public hidebysig specialname 
            instance void  set_Value4(string 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::'<Value4>k__BackingField'
      IL_0007:  ret
    } // end of method Item::set_Value4

    .method public hidebysig specialname 
            instance string  get_Value5() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (string V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::'<Value5>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method Item::get_Value5

    .method public hidebysig specialname 
            instance void  set_Value5(string 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::'<Value5>k__BackingField'
      IL_0007:  ret
    } // end of method Item::set_Value5

    .method public hidebysig specialname 
            instance string  get_Value6() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (string V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::'<Value6>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method Item::get_Value6

    .method public hidebysig specialname 
            instance void  set_Value6(string 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::'<Value6>k__BackingField'
      IL_0007:  ret
    } // end of method Item::set_Value6

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method Item::.ctor

    .property instance string Text()
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Text(string)
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::get_Text()
    } // end of property Item::Text
    .property instance valuetype [mscorlib]System.Decimal
            Value()
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value(valuetype [mscorlib]System.Decimal)
      .get instance valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::get_Value()
    } // end of property Item::Value
    .property instance valuetype [mscorlib]System.Decimal
            Value2()
    {
      .get instance valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::get_Value2()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value2(valuetype [mscorlib]System.Decimal)
    } // end of property Item::Value2
    .property instance string Value3()
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value3(string)
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::get_Value3()
    } // end of property Item::Value3
    .property instance string Value4()
    {
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::get_Value4()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value4(string)
    } // end of property Item::Value4
    .property instance string Value5()
    {
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::get_Value5()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value5(string)
    } // end of property Item::Value5
    .property instance string Value6()
    {
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::get_Value6()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value6(string)
    } // end of property Item::Value6
  } // end of class Item

  .class auto ansi nested public beforefieldinit OtherItem
         extends [mscorlib]System.Object
  {
    .field private valuetype [mscorlib]System.Decimal '<Value>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private valuetype [mscorlib]System.Decimal '<Value2>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> '<Nullable>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> '<Nullable2>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> '<Nullable3>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> '<Nullable4>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig specialname 
            instance valuetype [mscorlib]System.Decimal 
            get_Value() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (valuetype [mscorlib]System.Decimal V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::'<Value>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method OtherItem::get_Value

    .method public hidebysig specialname 
            instance void  set_Value(valuetype [mscorlib]System.Decimal 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::'<Value>k__BackingField'
      IL_0007:  ret
    } // end of method OtherItem::set_Value

    .method public hidebysig specialname 
            instance valuetype [mscorlib]System.Decimal 
            get_Value2() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (valuetype [mscorlib]System.Decimal V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::'<Value2>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method OtherItem::get_Value2

    .method public hidebysig specialname 
            instance void  set_Value2(valuetype [mscorlib]System.Decimal 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::'<Value2>k__BackingField'
      IL_0007:  ret
    } // end of method OtherItem::set_Value2

    .method public hidebysig specialname 
            instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> 
            get_Nullable() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::'<Nullable>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method OtherItem::get_Nullable

    .method public hidebysig specialname 
            instance void  set_Nullable(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::'<Nullable>k__BackingField'
      IL_0007:  ret
    } // end of method OtherItem::set_Nullable

    .method public hidebysig specialname 
            instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> 
            get_Nullable2() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::'<Nullable2>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method OtherItem::get_Nullable2

    .method public hidebysig specialname 
            instance void  set_Nullable2(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::'<Nullable2>k__BackingField'
      IL_0007:  ret
    } // end of method OtherItem::set_Nullable2

    .method public hidebysig specialname 
            instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> 
            get_Nullable3() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::'<Nullable3>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method OtherItem::get_Nullable3

    .method public hidebysig specialname 
            instance void  set_Nullable3(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::'<Nullable3>k__BackingField'
      IL_0007:  ret
    } // end of method OtherItem::set_Nullable3

    .method public hidebysig specialname 
            instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> 
            get_Nullable4() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::'<Nullable4>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method OtherItem::get_Nullable4

    .method public hidebysig specialname 
            instance void  set_Nullable4(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::'<Nullable4>k__BackingField'
      IL_0007:  ret
    } // end of method OtherItem::set_Nullable4

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method OtherItem::.ctor

    .property instance valuetype [mscorlib]System.Decimal
            Value()
    {
      .get instance valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Value()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::set_Value(valuetype [mscorlib]System.Decimal)
    } // end of property OtherItem::Value
    .property instance valuetype [mscorlib]System.Decimal
            Value2()
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::set_Value2(valuetype [mscorlib]System.Decimal)
      .get instance valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Value2()
    } // end of property OtherItem::Value2
    .property instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
            Nullable()
    {
      .get instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Nullable()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::set_Nullable(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>)
    } // end of property OtherItem::Nullable
    .property instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
            Nullable2()
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::set_Nullable2(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>)
      .get instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Nullable2()
    } // end of property OtherItem::Nullable2
    .property instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
            Nullable3()
    {
      .get instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Nullable3()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::set_Nullable3(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>)
    } // end of property OtherItem::Nullable3
    .property instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
            Nullable4()
    {
      .get instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Nullable4()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::set_Nullable4(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>)
    } // end of property OtherItem::Nullable4
  } // end of class OtherItem

  .class auto ansi nested public beforefieldinit OtherItem2
         extends [mscorlib]System.Object
  {
    .field public initonly class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem Data
    .field private class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem '<Data2>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig specialname 
            instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem 
            get_Data2() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2::'<Data2>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method OtherItem2::get_Data2

    .method private hidebysig specialname 
            instance void  set_Data2(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2::'<Data2>k__BackingField'
      IL_0007:  ret
    } // end of method OtherItem2::set_Data2

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method OtherItem2::.ctor

    .property instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem
            Data2()
    {
      .get instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2::get_Data2()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2::set_Data2(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem)
    } // end of property OtherItem2::Data2
  } // end of class OtherItem2

  .class auto ansi nested public beforefieldinit V3f
         extends [mscorlib]System.Object
  {
    .field private float32 x
    .field private float32 y
    .field private float32 z
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(float32 _x,
                                 float32 _y,
                                 float32 _z) cil managed
    {
      // Code size       31 (0x1f)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  nop
      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  stfld      float32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::x
      IL_000f:  ldarg.0
      IL_0010:  ldarg.2
      IL_0011:  stfld      float32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::y
      IL_0016:  ldarg.0
      IL_0017:  ldarg.3
      IL_0018:  stfld      float32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::z
      IL_001d:  nop
      IL_001e:  ret
    } // end of method V3f::.ctor

  } // end of class V3f

  .field private static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[] Issue1336_rg0
  .field private static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...] Issue1336_rg1
  .field private static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][] Issue1336_rg1b
  .field private static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...] Issue1336_rg1c
  .field private static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...][] Issue1336_rg1d
  .field private static int32[0...,0...] Issue1336_rg2
  .field private static class [mscorlib]System.EventHandler 'CS$<>9__CachedAnonymousMethodDelegate6'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool> 'CS$<>9__CachedAnonymousMethodDelegate1a'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private hidebysig static void  X(object a,
                                           object b) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method TestCases::X

  .method private hidebysig static object 
          Y() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (object V_0)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method TestCases::Y

  .method public hidebysig static void  TestCall(int32 a,
                                                 class [mscorlib]System.Threading.Thread thread) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method TestCases::TestCall

  .method public hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C 
          TestCall(int32 a,
                   class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C c) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method TestCases::TestCall

  .method private hidebysig static int32 
          GetInt() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method TestCases::GetInt

  .method private hidebysig static string 
          GetString() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldstr      "Test"
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method TestCases::GetString

  .method private hidebysig static void  NoOp(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Guid>[] 'array') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method TestCases::NoOp

  .method private hidebysig instance void 
          Data_TestEvent(object sender,
                         class [mscorlib]System.EventArgs e) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0006:  throw
  } // end of method TestCases::Data_TestEvent

  .method public hidebysig static void  Array1() cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldc.i4.s   10
    IL_0008:  newarr     [mscorlib]System.Int32
    IL_000d:  dup
    IL_000e:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '<PrivateImplementationDetails>'::'$$method0x600000b-1'
    IL_0013:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0018:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001d:  nop
    IL_001e:  ret
  } // end of method TestCases::Array1

  .method public hidebysig static void  Array2(int32 a,
                                               int32 b,
                                               int32 c) cil managed
  {
    // Code size       33 (0x21)
    .maxstack  4
    .locals init (int32[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldc.i4.5
    IL_0007:  newarr     [mscorlib]System.Int32
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.0
    IL_000f:  ldarg.0
    IL_0010:  stelem.i4
    IL_0011:  ldloc.0
    IL_0012:  ldc.i4.2
    IL_0013:  ldarg.1
    IL_0014:  stelem.i4
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.4
    IL_0017:  ldarg.2
    IL_0018:  stelem.i4
    IL_0019:  ldloc.0
    IL_001a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001f:  nop
    IL_0020:  ret
  } // end of method TestCases::Array2

  .method public hidebysig static void  NestedArray(int32 a,
                                                    int32 b,
                                                    int32 c) cil managed
  {
    // Code size       85 (0x55)
    .maxstack  6
    .locals init (int32[][] V_0,
             int32[] V_1)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldc.i4.3
    IL_0007:  newarr     int32[]
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.0
    IL_000f:  ldc.i4.s   10
    IL_0011:  newarr     [mscorlib]System.Int32
    IL_0016:  dup
    IL_0017:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '<PrivateImplementationDetails>'::'$$method0x600000d-1'
    IL_001c:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0021:  stelem.ref
    IL_0022:  ldloc.0
    IL_0023:  ldc.i4.1
    IL_0024:  ldc.i4.3
    IL_0025:  newarr     [mscorlib]System.Int32
    IL_002a:  stloc.1
    IL_002b:  ldloc.1
    IL_002c:  ldc.i4.0
    IL_002d:  ldarg.0
    IL_002e:  stelem.i4
    IL_002f:  ldloc.1
    IL_0030:  ldc.i4.1
    IL_0031:  ldarg.1
    IL_0032:  stelem.i4
    IL_0033:  ldloc.1
    IL_0034:  ldc.i4.2
    IL_0035:  ldarg.2
    IL_0036:  stelem.i4
    IL_0037:  ldloc.1
    IL_0038:  stelem.ref
    IL_0039:  ldloc.0
    IL_003a:  ldc.i4.2
    IL_003b:  ldc.i4.6
    IL_003c:  newarr     [mscorlib]System.Int32
    IL_0041:  dup
    IL_0042:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=24' '<PrivateImplementationDetails>'::'$$method0x600000d-2'
    IL_0047:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_004c:  stelem.ref
    IL_004d:  ldloc.0
    IL_004e:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0053:  nop
    IL_0054:  ret
  } // end of method TestCases::NestedArray

  .method public hidebysig static void  NestedNullableArray(int32 a,
                                                            int32 b,
                                                            int32 c) cil managed
  {
    // Code size       464 (0x1d0)
    .maxstack  5
    .locals init (valuetype [mscorlib]System.Nullable`1<int32>[][] V_0,
             valuetype [mscorlib]System.Nullable`1<int32>[] V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldc.i4.3
    IL_0007:  newarr     valuetype [mscorlib]System.Nullable`1<int32>[]
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.0
    IL_000f:  ldc.i4.s   11
    IL_0011:  newarr     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0016:  stloc.1
    IL_0017:  ldloc.1
    IL_0018:  ldc.i4.0
    IL_0019:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_001e:  ldc.i4.1
    IL_001f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0024:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0029:  ldloc.1
    IL_002a:  ldc.i4.1
    IL_002b:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0030:  ldc.i4.2
    IL_0031:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0036:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_003b:  ldloc.1
    IL_003c:  ldc.i4.2
    IL_003d:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0042:  ldc.i4.3
    IL_0043:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0048:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_004d:  ldloc.1
    IL_004e:  ldc.i4.3
    IL_004f:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0054:  ldc.i4.4
    IL_0055:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_005a:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_005f:  ldloc.1
    IL_0060:  ldc.i4.4
    IL_0061:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0066:  ldc.i4.5
    IL_0067:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_006c:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0071:  ldloc.1
    IL_0072:  ldc.i4.5
    IL_0073:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0078:  ldc.i4.6
    IL_0079:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_007e:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0083:  ldloc.1
    IL_0084:  ldc.i4.6
    IL_0085:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_008a:  ldc.i4.7
    IL_008b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0090:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0095:  ldloc.1
    IL_0096:  ldc.i4.7
    IL_0097:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_009c:  ldc.i4.8
    IL_009d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00a2:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_00a7:  ldloc.1
    IL_00a8:  ldc.i4.8
    IL_00a9:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00ae:  ldc.i4.s   9
    IL_00b0:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00b5:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_00ba:  ldloc.1
    IL_00bb:  ldc.i4.s   9
    IL_00bd:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00c2:  ldc.i4.s   10
    IL_00c4:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00c9:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_00ce:  ldloc.1
    IL_00cf:  ldc.i4.s   10
    IL_00d1:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00d6:  ldloca.s   V_2
    IL_00d8:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00de:  ldloc.2
    IL_00df:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_00e4:  ldloc.1
    IL_00e5:  stelem.ref
    IL_00e6:  ldloc.0
    IL_00e7:  ldc.i4.1
    IL_00e8:  ldc.i4.4
    IL_00e9:  newarr     valuetype [mscorlib]System.Nullable`1<int32>
    IL_00ee:  stloc.1
    IL_00ef:  ldloc.1
    IL_00f0:  ldc.i4.0
    IL_00f1:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00f6:  ldarg.0
    IL_00f7:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00fc:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0101:  ldloc.1
    IL_0102:  ldc.i4.1
    IL_0103:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0108:  ldarg.1
    IL_0109:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_010e:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0113:  ldloc.1
    IL_0114:  ldc.i4.2
    IL_0115:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_011a:  ldarg.2
    IL_011b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0120:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0125:  ldloc.1
    IL_0126:  ldc.i4.3
    IL_0127:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_012c:  ldloca.s   V_2
    IL_012e:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0134:  ldloc.2
    IL_0135:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_013a:  ldloc.1
    IL_013b:  stelem.ref
    IL_013c:  ldloc.0
    IL_013d:  ldc.i4.2
    IL_013e:  ldc.i4.7
    IL_013f:  newarr     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0144:  stloc.1
    IL_0145:  ldloc.1
    IL_0146:  ldc.i4.0
    IL_0147:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_014c:  ldc.i4.1
    IL_014d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0152:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0157:  ldloc.1
    IL_0158:  ldc.i4.1
    IL_0159:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_015e:  ldc.i4.2
    IL_015f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0164:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0169:  ldloc.1
    IL_016a:  ldc.i4.2
    IL_016b:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0170:  ldc.i4.3
    IL_0171:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0176:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_017b:  ldloc.1
    IL_017c:  ldc.i4.3
    IL_017d:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0182:  ldc.i4.4
    IL_0183:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0188:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_018d:  ldloc.1
    IL_018e:  ldc.i4.4
    IL_018f:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0194:  ldc.i4.5
    IL_0195:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_019a:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_019f:  ldloc.1
    IL_01a0:  ldc.i4.5
    IL_01a1:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01a6:  ldc.i4.6
    IL_01a7:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01ac:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_01b1:  ldloc.1
    IL_01b2:  ldc.i4.6
    IL_01b3:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01b8:  ldloca.s   V_2
    IL_01ba:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01c0:  ldloc.2
    IL_01c1:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_01c6:  ldloc.1
    IL_01c7:  stelem.ref
    IL_01c8:  ldloc.0
    IL_01c9:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_01ce:  nop
    IL_01cf:  ret
  } // end of method TestCases::NestedNullableArray

  .method public hidebysig static void  NestedPointerArray(int32 a,
                                                           int32 b,
                                                           int32 c) cil managed
  {
    // Code size       84 (0x54)
    .maxstack  6
    .locals init (void*[][] V_0,
             void*[] V_1)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldc.i4.3
    IL_0007:  newarr     void*[]
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.0
    IL_000f:  ldc.i4.1
    IL_0010:  newarr     void*
    IL_0015:  stloc.1
    IL_0016:  ldloc.1
    IL_0017:  ldc.i4.0
    IL_0018:  ldc.i4.0
    IL_0019:  conv.u
    IL_001a:  stelem.i
    IL_001b:  ldloc.1
    IL_001c:  stelem.ref
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.1
    IL_001f:  ldc.i4.2
    IL_0020:  newarr     void*
    IL_0025:  stloc.1
    IL_0026:  ldloc.1
    IL_0027:  ldc.i4.0
    IL_0028:  ldc.i4     0xc8
    IL_002d:  conv.i
    IL_002e:  stelem.i
    IL_002f:  ldloc.1
    IL_0030:  ldc.i4.1
    IL_0031:  ldc.i4.0
    IL_0032:  conv.u
    IL_0033:  stelem.i
    IL_0034:  ldloc.1
    IL_0035:  stelem.ref
    IL_0036:  ldloc.0
    IL_0037:  ldc.i4.2
    IL_0038:  ldc.i4.2
    IL_0039:  newarr     void*
    IL_003e:  stloc.1
    IL_003f:  ldloc.1
    IL_0040:  ldc.i4.0
    IL_0041:  ldc.i4.s   100
    IL_0043:  conv.i
    IL_0044:  stelem.i
    IL_0045:  ldloc.1
    IL_0046:  ldc.i4.1
    IL_0047:  ldc.i4.0
    IL_0048:  conv.u
    IL_0049:  stelem.i
    IL_004a:  ldloc.1
    IL_004b:  stelem.ref
    IL_004c:  ldloc.0
    IL_004d:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0052:  nop
    IL_0053:  ret
  } // end of method TestCases::NestedPointerArray

  .method public hidebysig static void  ArrayBoolean() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldc.i4.8
    IL_0007:  newarr     [mscorlib]System.Boolean
    IL_000c:  dup
    IL_000d:  ldtoken    field int64 '<PrivateImplementationDetails>'::'$$method0x6000010-1'
    IL_0012:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0017:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001c:  nop
    IL_001d:  ret
  } // end of method TestCases::ArrayBoolean

  .method public hidebysig static void  ArrayByte() cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldc.i4.s   10
    IL_0008:  newarr     [mscorlib]System.Byte
    IL_000d:  dup
    IL_000e:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=10' '<PrivateImplementationDetails>'::'$$method0x6000011-1'
    IL_0013:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0018:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001d:  nop
    IL_001e:  ret
  } // end of method TestCases::ArrayByte

  .method public hidebysig static void  ArraySByte() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldc.i4.8
    IL_0007:  newarr     [mscorlib]System.SByte
    IL_000c:  dup
    IL_000d:  ldtoken    field int64 '<PrivateImplementationDetails>'::'$$method0x6000012-1'
    IL_0012:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0017:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001c:  nop
    IL_001d:  ret
  } // end of method TestCases::ArraySByte

  .method public hidebysig static void  ArrayShort() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldc.i4.5
    IL_0007:  newarr     [mscorlib]System.Int16
    IL_000c:  dup
    IL_000d:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=10' '<PrivateImplementationDetails>'::'$$method0x6000013-1'
    IL_0012:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0017:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001c:  nop
    IL_001d:  ret
  } // end of method TestCases::ArrayShort

  .method public hidebysig static void  ArrayUShort() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldc.i4.6
    IL_0007:  newarr     [mscorlib]System.UInt16
    IL_000c:  dup
    IL_000d:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=12' '<PrivateImplementationDetails>'::'$$method0x6000014-1'
    IL_0012:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0017:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001c:  nop
    IL_001d:  ret
  } // end of method TestCases::ArrayUShort

  .method public hidebysig static void  ArrayInt() cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldc.i4.s   10
    IL_0008:  newarr     [mscorlib]System.Int32
    IL_000d:  dup
    IL_000e:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '<PrivateImplementationDetails>'::'$$method0x6000015-1'
    IL_0013:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0018:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001d:  nop
    IL_001e:  ret
  } // end of method TestCases::ArrayInt

  .method public hidebysig static void  ArrayUInt() cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldc.i4.s   10
    IL_0008:  newarr     [mscorlib]System.UInt32
    IL_000d:  dup
    IL_000e:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '<PrivateImplementationDetails>'::'$$method0x6000016-1'
    IL_0013:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0018:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001d:  nop
    IL_001e:  ret
  } // end of method TestCases::ArrayUInt

  .method public hidebysig static void  ArrayLong() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldc.i4.5
    IL_0007:  newarr     [mscorlib]System.Int64
    IL_000c:  dup
    IL_000d:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '<PrivateImplementationDetails>'::'$$method0x6000017-1'
    IL_0012:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0017:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001c:  nop
    IL_001d:  ret
  } // end of method TestCases::ArrayLong

  .method public hidebysig static void  ArrayULong() cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldc.i4.s   10
    IL_0008:  newarr     [mscorlib]System.UInt64
    IL_000d:  dup
    IL_000e:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=80' '<PrivateImplementationDetails>'::'$$method0x6000018-1'
    IL_0013:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0018:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001d:  nop
    IL_001e:  ret
  } // end of method TestCases::ArrayULong

  .method public hidebysig static void  ArrayFloat() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldc.i4.6
    IL_0007:  newarr     [mscorlib]System.Single
    IL_000c:  dup
    IL_000d:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=24' '<PrivateImplementationDetails>'::'$$method0x6000019-1'
    IL_0012:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0017:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001c:  nop
    IL_001d:  ret
  } // end of method TestCases::ArrayFloat

  .method public hidebysig static void  ArrayDouble() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldc.i4.6
    IL_0007:  newarr     [mscorlib]System.Double
    IL_000c:  dup
    IL_000d:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=48' '<PrivateImplementationDetails>'::'$$method0x600001a-1'
    IL_0012:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0017:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001c:  nop
    IL_001d:  ret
  } // end of method TestCases::ArrayDouble

  .method public hidebysig static void  ArrayDecimal() cil managed
  {
    // Code size       129 (0x81)
    .maxstack  7
    .locals init (valuetype [mscorlib]System.Decimal[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldc.i4.6
    IL_0007:  newarr     [mscorlib]System.Decimal
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.0
    IL_000f:  ldelema    [mscorlib]System.Decimal
    IL_0014:  ldc.i4.s   -100
    IL_0016:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_001b:  stobj      [mscorlib]System.Decimal
    IL_0020:  ldloc.0
    IL_0021:  ldc.i4.2
    IL_0022:  ldelema    [mscorlib]System.Decimal
    IL_0027:  ldc.i4.s   100
    IL_0029:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_002e:  stobj      [mscorlib]System.Decimal
    IL_0033:  ldloc.0
    IL_0034:  ldc.i4.3
    IL_0035:  ldelema    [mscorlib]System.Decimal
    IL_003a:  ldc.i4.m1
    IL_003b:  ldc.i4.m1
    IL_003c:  ldc.i4.m1
    IL_003d:  ldc.i4     0x80
    IL_0042:  ldc.i4.0
    IL_0043:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0048:  stobj      [mscorlib]System.Decimal
    IL_004d:  ldloc.0
    IL_004e:  ldc.i4.4
    IL_004f:  ldelema    [mscorlib]System.Decimal
    IL_0054:  ldc.i4.m1
    IL_0055:  ldc.i4.m1
    IL_0056:  ldc.i4.m1
    IL_0057:  ldc.i4.0
    IL_0058:  ldc.i4.0
    IL_0059:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_005e:  stobj      [mscorlib]System.Decimal
    IL_0063:  ldloc.0
    IL_0064:  ldc.i4.5
    IL_0065:  ldelema    [mscorlib]System.Decimal
    IL_006a:  ldc.i4.1
    IL_006b:  ldc.i4.0
    IL_006c:  ldc.i4.0
    IL_006d:  ldc.i4.0
    IL_006e:  ldc.i4.7
    IL_006f:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0074:  stobj      [mscorlib]System.Decimal
    IL_0079:  ldloc.0
    IL_007a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_007f:  nop
    IL_0080:  ret
  } // end of method TestCases::ArrayDecimal

  .method public hidebysig static void  ArrayString() cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  4
    .locals init (string[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldc.i4.4
    IL_0007:  newarr     [mscorlib]System.String
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.0
    IL_000f:  ldstr      ""
    IL_0014:  stelem.ref
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.2
    IL_0017:  ldstr      "Hello"
    IL_001c:  stelem.ref
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.3
    IL_001f:  ldstr      "World"
    IL_0024:  stelem.ref
    IL_0025:  ldloc.0
    IL_0026:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_002b:  nop
    IL_002c:  ret
  } // end of method TestCases::ArrayString

  .method public hidebysig static void  ArrayEnum() cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  4
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldc.i4.4
    IL_0007:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.1
    IL_000f:  ldc.i4.1
    IL_0010:  stelem.i4
    IL_0011:  ldloc.0
    IL_0012:  ldc.i4.3
    IL_0013:  ldc.i4.1
    IL_0014:  stelem.i4
    IL_0015:  ldloc.0
    IL_0016:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001b:  nop
    IL_001c:  ret
  } // end of method TestCases::ArrayEnum

  .method public hidebysig instance int32[0...,0...] 
          MultidimensionalInit() cil managed
  {
    // Code size       25 (0x19)
    .maxstack  3
    .locals init (int32[0...,0...] V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.s   16
    IL_0003:  ldc.i4.4
    IL_0004:  newobj     instance void int32[0...,0...]::.ctor(int32,
                                                               int32)
    IL_0009:  dup
    IL_000a:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=256' '<PrivateImplementationDetails>'::'$$method0x600001e-1'
    IL_000f:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0014:  stloc.0
    IL_0015:  br.s       IL_0017

    IL_0017:  ldloc.0
    IL_0018:  ret
  } // end of method TestCases::MultidimensionalInit

  .method public hidebysig instance int32[0...,0...][] 
          MultidimensionalInit2() cil managed
  {
    // Code size       98 (0x62)
    .maxstack  5
    .locals init (int32[0...,0...][] V_0,
             int32[0...,0...][] V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.4
    IL_0002:  newarr     int32[0...,0...]
    IL_0007:  stloc.1
    IL_0008:  ldloc.1
    IL_0009:  ldc.i4.0
    IL_000a:  ldc.i4.4
    IL_000b:  ldc.i4.4
    IL_000c:  newobj     instance void int32[0...,0...]::.ctor(int32,
                                                               int32)
    IL_0011:  dup
    IL_0012:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' '<PrivateImplementationDetails>'::'$$method0x600001f-1'
    IL_0017:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_001c:  stelem.ref
    IL_001d:  ldloc.1
    IL_001e:  ldc.i4.1
    IL_001f:  ldc.i4.4
    IL_0020:  ldc.i4.4
    IL_0021:  newobj     instance void int32[0...,0...]::.ctor(int32,
                                                               int32)
    IL_0026:  dup
    IL_0027:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' '<PrivateImplementationDetails>'::'$$method0x600001f-2'
    IL_002c:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0031:  stelem.ref
    IL_0032:  ldloc.1
    IL_0033:  ldc.i4.2
    IL_0034:  ldc.i4.4
    IL_0035:  ldc.i4.4
    IL_0036:  newobj     instance void int32[0...,0...]::.ctor(int32,
                                                               int32)
    IL_003b:  dup
    IL_003c:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' '<PrivateImplementationDetails>'::'$$method0x600001f-3'
    IL_0041:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0046:  stelem.ref
    IL_0047:  ldloc.1
    IL_0048:  ldc.i4.3
    IL_0049:  ldc.i4.4
    IL_004a:  ldc.i4.4
    IL_004b:  newobj     instance void int32[0...,0...]::.ctor(int32,
                                                               int32)
    IL_0050:  dup
    IL_0051:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' '<PrivateImplementationDetails>'::'$$method0x600001f-4'
    IL_0056:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_005b:  stelem.ref
    IL_005c:  ldloc.1
    IL_005d:  stloc.0
    IL_005e:  br.s       IL_0060

    IL_0060:  ldloc.0
    IL_0061:  ret
  } // end of method TestCases::MultidimensionalInit2

  .method public hidebysig instance int32[0...,0...,0...][] 
          ArrayOfArrayOfArrayInit() cil managed
  {
    // Code size       58 (0x3a)
    .maxstack  5
    .locals init (int32[0...,0...,0...][] V_0,
             int32[0...,0...,0...][] V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.2
    IL_0002:  newarr     int32[0...,0...,0...]
    IL_0007:  stloc.1
    IL_0008:  ldloc.1
    IL_0009:  ldc.i4.0
    IL_000a:  ldc.i4.2
    IL_000b:  ldc.i4.3
    IL_000c:  ldc.i4.3
    IL_000d:  newobj     instance void int32[0...,0...,0...]::.ctor(int32,
                                                                    int32,
                                                                    int32)
    IL_0012:  dup
    IL_0013:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=72' '<PrivateImplementationDetails>'::'$$method0x6000020-1'
    IL_0018:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_001d:  stelem.ref
    IL_001e:  ldloc.1
    IL_001f:  ldc.i4.1
    IL_0020:  ldc.i4.2
    IL_0021:  ldc.i4.3
    IL_0022:  ldc.i4.3
    IL_0023:  newobj     instance void int32[0...,0...,0...]::.ctor(int32,
                                                                    int32,
                                                                    int32)
    IL_0028:  dup
    IL_0029:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=72' '<PrivateImplementationDetails>'::'$$method0x6000020-2'
    IL_002e:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0033:  stelem.ref
    IL_0034:  ldloc.1
    IL_0035:  stloc.0
    IL_0036:  br.s       IL_0038

    IL_0038:  ldloc.0
    IL_0039:  ret
  } // end of method TestCases::ArrayOfArrayOfArrayInit

  .method public hidebysig static void  RecursiveArrayInitializer() cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  4
    .locals init (int32[] V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.3
    IL_0002:  newarr     [mscorlib]System.Int32
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  ldc.i4.1
    IL_000b:  stelem.i4
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.1
    IL_000e:  ldc.i4.2
    IL_000f:  stelem.i4
    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.2
    IL_0012:  ldloc.0
    IL_0013:  ldc.i4.1
    IL_0014:  ldelem.i4
    IL_0015:  ldc.i4.1
    IL_0016:  add
    IL_0017:  stelem.i4
    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.0
    IL_001a:  ldc.i4.0
    IL_001b:  stelem.i4
    IL_001c:  ret
  } // end of method TestCases::RecursiveArrayInitializer

  .method public hidebysig static void  InvalidIndices(int32 a) cil managed
  {
    // Code size       25 (0x19)
    .maxstack  3
    .locals init (int32[] V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  newarr     [mscorlib]System.Int32
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.1
    IL_000a:  ldarg.0
    IL_000b:  stelem.i4
    IL_000c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0011:  ldloc.0
    IL_0012:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0017:  nop
    IL_0018:  ret
  } // end of method TestCases::InvalidIndices

  .method public hidebysig static void  InvalidIndices2(int32 a) cil managed
  {
    // Code size       25 (0x19)
    .maxstack  3
    .locals init (int32[] V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  newarr     [mscorlib]System.Int32
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.m1
    IL_000a:  ldarg.0
    IL_000b:  stelem.i4
    IL_000c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0011:  ldloc.0
    IL_0012:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0017:  nop
    IL_0018:  ret
  } // end of method TestCases::InvalidIndices2

  .method public hidebysig static void  IndicesInWrongOrder(int32 a,
                                                            int32 b) cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  3
    .locals init (int32[] V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.5
    IL_0002:  newarr     [mscorlib]System.Int32
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.2
    IL_000a:  ldarg.1
    IL_000b:  stelem.i4
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.1
    IL_000e:  ldarg.0
    IL_000f:  stelem.i4
    IL_0010:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0015:  ldloc.0
    IL_0016:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001b:  nop
    IL_001c:  ret
  } // end of method TestCases::IndicesInWrongOrder

  .method public hidebysig static uint8[] 
          ReverseInitializer(int32 i) cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  4
    .locals init (uint8[] V_0,
             uint8[] V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.4
    IL_0002:  newarr     [mscorlib]System.Byte
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.3
    IL_000a:  ldarg.0
    IL_000b:  conv.u1
    IL_000c:  stelem.i1
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.2
    IL_000f:  ldarg.0
    IL_0010:  ldc.i4.8
    IL_0011:  shr
    IL_0012:  conv.u1
    IL_0013:  stelem.i1
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.1
    IL_0016:  ldarg.0
    IL_0017:  ldc.i4.s   16
    IL_0019:  shr
    IL_001a:  conv.u1
    IL_001b:  stelem.i1
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.0
    IL_001e:  ldarg.0
    IL_001f:  ldc.i4.s   24
    IL_0021:  shr
    IL_0022:  conv.u1
    IL_0023:  stelem.i1
    IL_0024:  ldloc.0
    IL_0025:  stloc.1
    IL_0026:  br.s       IL_0028

    IL_0028:  ldloc.1
    IL_0029:  ret
  } // end of method TestCases::ReverseInitializer

  .method public hidebysig static void  Issue953_MissingNullableSpecifierForArrayInitializer() cil managed
  {
    // Code size       38 (0x26)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Guid>[] V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  newarr     valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Guid>
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  ldelema    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Guid>
    IL_000f:  ldsfld     valuetype [mscorlib]System.Guid [mscorlib]System.Guid::Empty
    IL_0014:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Guid>::.ctor(!0)
    IL_0019:  stobj      valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Guid>
    IL_001e:  ldloc.0
    IL_001f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::NoOp(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Guid>[])
    IL_0024:  nop
    IL_0025:  ret
  } // end of method TestCases::Issue953_MissingNullableSpecifierForArrayInitializer

  .method private hidebysig instance void 
          Issue907_Test3(string text) cil managed
  {
    // Code size       33 (0x21)
    .maxstack  4
    .locals init (class [mscorlib]System.Collections.Generic.Dictionary`2<string,object> V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,object>::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldstr      ""
    IL_0012:  ldarg.1
    IL_0013:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,object>::Add(!0,
                                                                                                                   !1)
    IL_0018:  nop
    IL_0019:  ldloc.0
    IL_001a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001f:  nop
    IL_0020:  ret
  } // end of method TestCases::Issue907_Test3

  .method private hidebysig instance int32[] 
          Issue1383(int32 i,
                    int32[] 'array') cil managed
  {
    // Code size       33 (0x21)
    .maxstack  4
    .locals init (int32[] V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.4
    IL_0002:  newarr     [mscorlib]System.Int32
    IL_0007:  starg.s    'array'
    IL_0009:  ldarg.2
    IL_000a:  ldarg.1
    IL_000b:  dup
    IL_000c:  ldc.i4.1
    IL_000d:  add
    IL_000e:  starg.s    i
    IL_0010:  ldc.i4.1
    IL_0011:  stelem.i4
    IL_0012:  ldarg.2
    IL_0013:  ldarg.1
    IL_0014:  dup
    IL_0015:  ldc.i4.1
    IL_0016:  add
    IL_0017:  starg.s    i
    IL_0019:  ldc.i4.2
    IL_001a:  stelem.i4
    IL_001b:  ldarg.2
    IL_001c:  stloc.0
    IL_001d:  br.s       IL_001f

    IL_001f:  ldloc.0
    IL_0020:  ret
  } // end of method TestCases::Issue1383

  .method private hidebysig instance string[0...,0...] 
          Issue1382a() cil managed
  {
    // Code size       171 (0xab)
    .maxstack  4
    .locals init (string[0...,0...] V_0,
             string[0...,0...] V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.4
    IL_0002:  ldc.i4.4
    IL_0003:  newobj     instance void string[0...,0...]::.ctor(int32,
                                                                int32)
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldc.i4.0
    IL_000b:  ldc.i4.1
    IL_000c:  ldstr      "test"
    IL_0011:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0016:  ldloc.1
    IL_0017:  ldc.i4.0
    IL_0018:  ldc.i4.2
    IL_0019:  ldstr      "hello"
    IL_001e:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.0
    IL_0025:  ldc.i4.3
    IL_0026:  ldstr      "world"
    IL_002b:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0030:  ldloc.1
    IL_0031:  ldc.i4.1
    IL_0032:  ldc.i4.0
    IL_0033:  ldstr      "test"
    IL_0038:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_003d:  ldloc.1
    IL_003e:  ldc.i4.1
    IL_003f:  ldc.i4.2
    IL_0040:  ldstr      "hello"
    IL_0045:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_004a:  ldloc.1
    IL_004b:  ldc.i4.1
    IL_004c:  ldc.i4.3
    IL_004d:  ldstr      "world"
    IL_0052:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0057:  ldloc.1
    IL_0058:  ldc.i4.2
    IL_0059:  ldc.i4.0
    IL_005a:  ldstr      "test"
    IL_005f:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0064:  ldloc.1
    IL_0065:  ldc.i4.2
    IL_0066:  ldc.i4.1
    IL_0067:  ldstr      "hello"
    IL_006c:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0071:  ldloc.1
    IL_0072:  ldc.i4.2
    IL_0073:  ldc.i4.3
    IL_0074:  ldstr      "world"
    IL_0079:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_007e:  ldloc.1
    IL_007f:  ldc.i4.3
    IL_0080:  ldc.i4.0
    IL_0081:  ldstr      "test"
    IL_0086:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_008b:  ldloc.1
    IL_008c:  ldc.i4.3
    IL_008d:  ldc.i4.1
    IL_008e:  ldstr      "hello"
    IL_0093:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0098:  ldloc.1
    IL_0099:  ldc.i4.3
    IL_009a:  ldc.i4.2
    IL_009b:  ldstr      "world"
    IL_00a0:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_00a5:  ldloc.1
    IL_00a6:  stloc.0
    IL_00a7:  br.s       IL_00a9

    IL_00a9:  ldloc.0
    IL_00aa:  ret
  } // end of method TestCases::Issue1382a

  .method private hidebysig instance string[0...,0...] 
          Issue1382b() cil managed
  {
    // Code size       171 (0xab)
    .maxstack  4
    .locals init (string[0...,0...] V_0,
             string[0...,0...] V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.4
    IL_0002:  ldc.i4.4
    IL_0003:  newobj     instance void string[0...,0...]::.ctor(int32,
                                                                int32)
    IL_0008:  stloc.1
    IL_0009:  ldloc.1
    IL_000a:  ldc.i4.0
    IL_000b:  ldc.i4.0
    IL_000c:  ldstr      "test"
    IL_0011:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0016:  ldloc.1
    IL_0017:  ldc.i4.0
    IL_0018:  ldc.i4.1
    IL_0019:  ldstr      "hello"
    IL_001e:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.0
    IL_0025:  ldc.i4.2
    IL_0026:  ldstr      "world"
    IL_002b:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0030:  ldloc.1
    IL_0031:  ldc.i4.1
    IL_0032:  ldc.i4.0
    IL_0033:  ldstr      "test"
    IL_0038:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_003d:  ldloc.1
    IL_003e:  ldc.i4.1
    IL_003f:  ldc.i4.1
    IL_0040:  ldstr      "hello"
    IL_0045:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_004a:  ldloc.1
    IL_004b:  ldc.i4.1
    IL_004c:  ldc.i4.3
    IL_004d:  ldstr      "world"
    IL_0052:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0057:  ldloc.1
    IL_0058:  ldc.i4.2
    IL_0059:  ldc.i4.0
    IL_005a:  ldstr      "test"
    IL_005f:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0064:  ldloc.1
    IL_0065:  ldc.i4.2
    IL_0066:  ldc.i4.2
    IL_0067:  ldstr      "hello"
    IL_006c:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0071:  ldloc.1
    IL_0072:  ldc.i4.2
    IL_0073:  ldc.i4.3
    IL_0074:  ldstr      "world"
    IL_0079:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_007e:  ldloc.1
    IL_007f:  ldc.i4.3
    IL_0080:  ldc.i4.1
    IL_0081:  ldstr      "test"
    IL_0086:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_008b:  ldloc.1
    IL_008c:  ldc.i4.3
    IL_008d:  ldc.i4.2
    IL_008e:  ldstr      "hello"
    IL_0093:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0098:  ldloc.1
    IL_0099:  ldc.i4.3
    IL_009a:  ldc.i4.3
    IL_009b:  ldstr      "world"
    IL_00a0:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_00a5:  ldloc.1
    IL_00a6:  stloc.0
    IL_00a7:  br.s       IL_00a9

    IL_00a9:  ldloc.0
    IL_00aa:  ret
  } // end of method TestCases::Issue1382b

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C 
          Test1() cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C V_1)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S>::.ctor()
    IL_000d:  stfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::L
    IL_0012:  ldloc.0
    IL_0013:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::L
    IL_0018:  ldc.i4.1
    IL_0019:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::.ctor(int32)
    IL_001e:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S>::Add(!0)
    IL_0023:  nop
    IL_0024:  ldloc.0
    IL_0025:  stloc.1
    IL_0026:  br.s       IL_0028

    IL_0028:  ldloc.1
    IL_0029:  ret
  } // end of method TestCases::Test1

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C 
          Test1Alternative() cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C V_0,
             class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S> V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C V_2)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S>::.ctor()
    IL_000e:  stloc.1
    IL_000f:  ldloc.1
    IL_0010:  ldc.i4.1
    IL_0011:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::.ctor(int32)
    IL_0016:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S>::Add(!0)
    IL_001b:  nop
    IL_001c:  ldloc.1
    IL_001d:  stfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::L
    IL_0022:  ldloc.0
    IL_0023:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::TestCall(int32,
                                                                                                                                                                                             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C)
    IL_0028:  stloc.2
    IL_0029:  br.s       IL_002b

    IL_002b:  ldloc.2
    IL_002c:  ret
  } // end of method TestCases::Test1Alternative

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C 
          Test2() cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C V_1)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Z
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.2
    IL_0010:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Z
    IL_0015:  ldloc.0
    IL_0016:  stloc.1
    IL_0017:  br.s       IL_0019

    IL_0019:  ldloc.1
    IL_001a:  ret
  } // end of method TestCases::Test2

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C 
          Test3() cil managed
  {
    // Code size       37 (0x25)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C V_1)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::.ctor(int32)
    IL_000e:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Y
    IL_0013:  ldloc.0
    IL_0014:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Y
    IL_0019:  ldc.i4.2
    IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::A
    IL_001f:  ldloc.0
    IL_0020:  stloc.1
    IL_0021:  br.s       IL_0023

    IL_0023:  ldloc.1
    IL_0024:  ret
  } // end of method TestCases::Test3

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C 
          Test3b() cil managed
  {
    // Code size       38 (0x26)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.1
    IL_000a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Z
    IL_000f:  ldloc.0
    IL_0010:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Y
    IL_0015:  ldc.i4.2
    IL_0016:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::A
    IL_001b:  ldloc.0
    IL_001c:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::TestCall(int32,
                                                                                                                                                                                             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C)
    IL_0021:  stloc.1
    IL_0022:  br.s       IL_0024

    IL_0024:  ldloc.1
    IL_0025:  ret
  } // end of method TestCases::Test3b

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C 
          Test4() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C V_1)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Y
    IL_000d:  ldc.i4.1
    IL_000e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::A
    IL_0013:  ldloc.0
    IL_0014:  ldc.i4.2
    IL_0015:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Z
    IL_001a:  ldloc.0
    IL_001b:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Y
    IL_0020:  ldc.i4.3
    IL_0021:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::B
    IL_0026:  ldloc.0
    IL_0027:  stloc.1
    IL_0028:  br.s       IL_002a

    IL_002a:  ldloc.1
    IL_002b:  ret
  } // end of method TestCases::Test4

  .method public hidebysig static void  ObjectInitializer() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.0
    IL_000e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0013:  nop
    IL_0014:  ldloc.0
    IL_0015:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001a:  nop
    IL_001b:  ret
  } // end of method TestCases::ObjectInitializer

  .method public hidebysig static void  NotAnObjectInitializer() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.0
    IL_0009:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_000e:  nop
    IL_000f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0014:  ldloc.0
    IL_0015:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001a:  nop
    IL_001b:  ret
  } // end of method TestCases::NotAnObjectInitializer

  .method public hidebysig static void  NotAnObjectInitializerWithEvent() cil managed
  {
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldsfld     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'CS$<>9__CachedAnonymousMethodDelegate6'
    IL_000d:  brtrue.s   IL_0022

    IL_000f:  ldnull
    IL_0010:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'<NotAnObjectInitializerWithEvent>b__5'(object,
                                                                                                                                                class [mscorlib]System.EventArgs)
    IL_0016:  newobj     instance void [mscorlib]System.EventHandler::.ctor(object,
                                                                            native int)
    IL_001b:  stsfld     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'CS$<>9__CachedAnonymousMethodDelegate6'
    IL_0020:  br.s       IL_0022

    IL_0022:  ldsfld     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'CS$<>9__CachedAnonymousMethodDelegate6'
    IL_0027:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::add_TestEvent(class [mscorlib]System.EventHandler)
    IL_002c:  nop
    IL_002d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0032:  ldloc.0
    IL_0033:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0038:  nop
    IL_0039:  ret
  } // end of method TestCases::NotAnObjectInitializerWithEvent

  .method public hidebysig static void  ObjectInitializerAssignCollectionToField() cil managed
  {
    // Code size       57 (0x39)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0,
             class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> V_1)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.0
    IL_000e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0013:  nop
    IL_0014:  ldloc.0
    IL_0015:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::.ctor()
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4.0
    IL_001d:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_0022:  nop
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.1
    IL_0025:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_002a:  nop
    IL_002b:  ldloc.1
    IL_002c:  stfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::FieldList
    IL_0031:  ldloc.0
    IL_0032:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0037:  nop
    IL_0038:  ret
  } // end of method TestCases::ObjectInitializerAssignCollectionToField

  .method public hidebysig static void  ObjectInitializerAddToCollectionInField() cil managed
  {
    // Code size       54 (0x36)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.0
    IL_000e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0013:  nop
    IL_0014:  ldloc.0
    IL_0015:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::FieldList
    IL_001a:  ldc.i4.0
    IL_001b:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_0020:  nop
    IL_0021:  ldloc.0
    IL_0022:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::FieldList
    IL_0027:  ldc.i4.1
    IL_0028:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_002d:  nop
    IL_002e:  ldloc.0
    IL_002f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0034:  nop
    IL_0035:  ret
  } // end of method TestCases::ObjectInitializerAddToCollectionInField

  .method public hidebysig static void  ObjectInitializerAssignCollectionToProperty() cil managed
  {
    // Code size       58 (0x3a)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0,
             class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> V_1)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.0
    IL_000e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0013:  nop
    IL_0014:  ldloc.0
    IL_0015:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::.ctor()
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4.0
    IL_001d:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_0022:  nop
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.1
    IL_0025:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_002a:  nop
    IL_002b:  ldloc.1
    IL_002c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_PropertyList(class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>)
    IL_0031:  nop
    IL_0032:  ldloc.0
    IL_0033:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0038:  nop
    IL_0039:  ret
  } // end of method TestCases::ObjectInitializerAssignCollectionToProperty

  .method public hidebysig static void  ObjectInitializerAddToCollectionInProperty() cil managed
  {
    // Code size       54 (0x36)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.0
    IL_000e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0013:  nop
    IL_0014:  ldloc.0
    IL_0015:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_PropertyList()
    IL_001a:  ldc.i4.0
    IL_001b:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_0020:  nop
    IL_0021:  ldloc.0
    IL_0022:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_PropertyList()
    IL_0027:  ldc.i4.1
    IL_0028:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_002d:  nop
    IL_002e:  ldloc.0
    IL_002f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0034:  nop
    IL_0035:  ret
  } // end of method TestCases::ObjectInitializerAddToCollectionInProperty

  .method public hidebysig static void  ObjectInitializerWithInitializationOfNestedObjects() cil managed
  {
    // Code size       51 (0x33)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0012:  ldc.i4.0
    IL_0013:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0018:  nop
    IL_0019:  ldloc.0
    IL_001a:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_001f:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0024:  ldc.i4.1
    IL_0025:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_002a:  nop
    IL_002b:  ldloc.0
    IL_002c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0031:  nop
    IL_0032:  ret
  } // end of method TestCases::ObjectInitializerWithInitializationOfNestedObjects

  .method public hidebysig static void  ObjectInitializerWithInitializationOfDeeplyNestedObjects() cil managed
  {
    // Code size       84 (0x54)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.1
    IL_000e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0013:  nop
    IL_0014:  ldloc.0
    IL_0015:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_001a:  ldc.i4.0
    IL_001b:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0020:  nop
    IL_0021:  ldloc.0
    IL_0022:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0027:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_002c:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0031:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0036:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_003b:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0040:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0045:  ldc.i4.1
    IL_0046:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_004b:  nop
    IL_004c:  ldloc.0
    IL_004d:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0052:  nop
    IL_0053:  ret
  } // end of method TestCases::ObjectInitializerWithInitializationOfDeeplyNestedObjects

  .method public hidebysig static void  CollectionInitializerInsideObjectInitializers() cil managed
  {
    // Code size       63 (0x3f)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_1)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_0012:  stloc.1
    IL_0013:  ldloc.1
    IL_0014:  ldc.i4.0
    IL_0015:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_001a:  nop
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4.1
    IL_001d:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_b(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0022:  nop
    IL_0023:  ldloc.1
    IL_0024:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_PropertyList()
    IL_0029:  ldc.i4.0
    IL_002a:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_002f:  nop
    IL_0030:  ldloc.1
    IL_0031:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_MoreData(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data)
    IL_0036:  nop
    IL_0037:  ldloc.0
    IL_0038:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_003d:  nop
    IL_003e:  ret
  } // end of method TestCases::CollectionInitializerInsideObjectInitializers

  .method public hidebysig static void  NotAStructInitializer_DefaultConstructor() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData V_0)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_0
    IL_0003:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData
    IL_0009:  ldloca.s   V_0
    IL_000b:  ldc.i4.1
    IL_000c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::Field
    IL_0011:  ldloca.s   V_0
    IL_0013:  ldc.i4.2
    IL_0014:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::set_Property(int32)
    IL_0019:  nop
    IL_001a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_001f:  ldloc.0
    IL_0020:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData
    IL_0025:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_002a:  nop
    IL_002b:  ret
  } // end of method TestCases::NotAStructInitializer_DefaultConstructor

  .method public hidebysig static void  StructInitializer_DefaultConstructor() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  3
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldloca.s   V_0
    IL_0008:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData
    IL_000e:  ldloca.s   V_0
    IL_0010:  ldc.i4.1
    IL_0011:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::Field
    IL_0016:  ldloca.s   V_0
    IL_0018:  ldc.i4.2
    IL_0019:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::set_Property(int32)
    IL_001e:  nop
    IL_001f:  ldloc.0
    IL_0020:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData
    IL_0025:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_002a:  nop
    IL_002b:  ret
  } // end of method TestCases::StructInitializer_DefaultConstructor

  .method public hidebysig static void  NotAStructInitializer_ExplicitConstructor() cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData V_0)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_0
    IL_0003:  ldc.i4.0
    IL_0004:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::.ctor(int32)
    IL_0009:  nop
    IL_000a:  ldloca.s   V_0
    IL_000c:  ldc.i4.1
    IL_000d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::Field
    IL_0012:  ldloca.s   V_0
    IL_0014:  ldc.i4.2
    IL_0015:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::set_Property(int32)
    IL_001a:  nop
    IL_001b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0020:  ldloc.0
    IL_0021:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData
    IL_0026:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_002b:  nop
    IL_002c:  ret
  } // end of method TestCases::NotAStructInitializer_ExplicitConstructor

  .method public hidebysig static void  StructInitializer_ExplicitConstructor() cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  3
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldloca.s   V_0
    IL_0008:  ldc.i4.0
    IL_0009:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::.ctor(int32)
    IL_000e:  nop
    IL_000f:  ldloca.s   V_0
    IL_0011:  ldc.i4.1
    IL_0012:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::Field
    IL_0017:  ldloca.s   V_0
    IL_0019:  ldc.i4.2
    IL_001a:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::set_Property(int32)
    IL_001f:  nop
    IL_0020:  ldloc.0
    IL_0021:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData
    IL_0026:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_002b:  nop
    IL_002c:  ret
  } // end of method TestCases::StructInitializer_ExplicitConstructor

  .method public hidebysig static void  StructInitializerWithInitializationOfNestedObjects() cil managed
  {
    // Code size       79 (0x4f)
    .maxstack  3
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  ldloca.s   V_0
    IL_0008:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData
    IL_000e:  ldloca.s   V_0
    IL_0010:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::get_MoreData()
    IL_0015:  ldc.i4.0
    IL_0016:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_001b:  nop
    IL_001c:  ldloca.s   V_0
    IL_001e:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::get_MoreData()
    IL_0023:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::FieldList
    IL_0028:  ldc.i4.0
    IL_0029:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_002e:  nop
    IL_002f:  ldloca.s   V_0
    IL_0031:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::get_MoreData()
    IL_0036:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::FieldList
    IL_003b:  ldc.i4.1
    IL_003c:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_0041:  nop
    IL_0042:  ldloc.0
    IL_0043:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData
    IL_0048:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_004d:  nop
    IL_004e:  ret
  } // end of method TestCases::StructInitializerWithInitializationOfNestedObjects

  .method public hidebysig static void  StructInitializerWithinObjectInitializer() cil managed
  {
    // Code size       54 (0x36)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData V_1)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldloca.s   V_1
    IL_000f:  ldc.i4.2
    IL_0010:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::.ctor(int32)
    IL_0015:  nop
    IL_0016:  ldloca.s   V_1
    IL_0018:  ldc.i4.1
    IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::Field
    IL_001e:  ldloca.s   V_1
    IL_0020:  ldc.i4.2
    IL_0021:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::set_Property(int32)
    IL_0026:  nop
    IL_0027:  ldloc.1
    IL_0028:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_NestedStruct(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData)
    IL_002d:  nop
    IL_002e:  ldloc.0
    IL_002f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0034:  nop
    IL_0035:  ret
  } // end of method TestCases::StructInitializerWithinObjectInitializer

  .method public hidebysig static void  Issue270_NestedInitialisers() cil managed
  {
    // Code size       128 (0x80)
    .maxstack  6
    .locals init (class [mscorlib]System.Globalization.NumberFormatInfo[] V_0,
             class [mscorlib]System.Threading.Thread V_1,
             class [mscorlib]System.Globalization.CultureInfo V_2,
             class [mscorlib]System.Globalization.DateTimeFormatInfo V_3)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.0
    IL_0004:  ldnull
    IL_0005:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Issue270_NestedInitialisers()
    IL_000b:  newobj     instance void [mscorlib]System.Threading.ThreadStart::.ctor(object,
                                                                                     native int)
    IL_0010:  newobj     instance void [mscorlib]System.Threading.Thread::.ctor(class [mscorlib]System.Threading.ThreadStart)
    IL_0015:  stloc.1
    IL_0016:  ldloc.1
    IL_0017:  ldc.i4.1
    IL_0018:  callvirt   instance void [mscorlib]System.Threading.Thread::set_Priority(valuetype [mscorlib]System.Threading.ThreadPriority)
    IL_001d:  nop
    IL_001e:  ldloc.1
    IL_001f:  ldc.i4.0
    IL_0020:  newobj     instance void [mscorlib]System.Globalization.CultureInfo::.ctor(int32)
    IL_0025:  stloc.2
    IL_0026:  ldloc.2
    IL_0027:  newobj     instance void [mscorlib]System.Globalization.DateTimeFormatInfo::.ctor()
    IL_002c:  stloc.3
    IL_002d:  ldloc.3
    IL_002e:  ldstr      "ddmmyy"
    IL_0033:  callvirt   instance void [mscorlib]System.Globalization.DateTimeFormatInfo::set_ShortDatePattern(string)
    IL_0038:  nop
    IL_0039:  ldloc.3
    IL_003a:  callvirt   instance void [mscorlib]System.Globalization.CultureInfo::set_DateTimeFormat(class [mscorlib]System.Globalization.DateTimeFormatInfo)
    IL_003f:  nop
    IL_0040:  ldloc.2
    IL_0041:  ldloc.0
    IL_0042:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'CS$<>9__CachedAnonymousMethodDelegate1a'
    IL_0047:  brtrue.s   IL_005c

    IL_0049:  ldnull
    IL_004a:  ldftn      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'<Issue270_NestedInitialisers>b__19'(class [mscorlib]System.Globalization.NumberFormatInfo)
    IL_0050:  newobj     instance void class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool>::.ctor(object,
                                                                                                                                        native int)
    IL_0055:  stsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'CS$<>9__CachedAnonymousMethodDelegate1a'
    IL_005a:  br.s       IL_005c

    IL_005c:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'CS$<>9__CachedAnonymousMethodDelegate1a'
    IL_0061:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class [mscorlib]System.Globalization.NumberFormatInfo>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                         class [mscorlib]System.Func`2<!!0,bool>)
    IL_0066:  call       !!0 [System.Core]System.Linq.Enumerable::First<class [mscorlib]System.Globalization.NumberFormatInfo>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_006b:  callvirt   instance void [mscorlib]System.Globalization.CultureInfo::set_NumberFormat(class [mscorlib]System.Globalization.NumberFormatInfo)
    IL_0070:  nop
    IL_0071:  ldloc.2
    IL_0072:  callvirt   instance void [mscorlib]System.Threading.Thread::set_CurrentCulture(class [mscorlib]System.Globalization.CultureInfo)
    IL_0077:  nop
    IL_0078:  ldloc.1
    IL_0079:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::TestCall(int32,
                                                                                                                 class [mscorlib]System.Threading.Thread)
    IL_007e:  nop
    IL_007f:  ret
  } // end of method TestCases::Issue270_NestedInitialisers

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2 
          Issue1345() cil managed
  {
    // Code size       36 (0x24)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2 V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2 V_1)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2::Data
    IL_000d:  ldc.i4.3
    IL_000e:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0013:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0018:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::set_Nullable(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>)
    IL_001d:  nop
    IL_001e:  ldloc.0
    IL_001f:  stloc.1
    IL_0020:  br.s       IL_0022

    IL_0022:  ldloc.1
    IL_0023:  ret
  } // end of method TestCases::Issue1345

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2 
          Issue1345b() cil managed
  {
    // Code size       36 (0x24)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2 V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2 V_1)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2::get_Data2()
    IL_000d:  ldc.i4.3
    IL_000e:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0013:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0018:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::set_Nullable(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>)
    IL_001d:  nop
    IL_001e:  ldloc.0
    IL_001f:  stloc.1
    IL_0020:  br.s       IL_0022

    IL_0022:  ldloc.1
    IL_0023:  ret
  } // end of method TestCases::Issue1345b

  .method private hidebysig instance void 
          Issue1250_Test1(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum 'value') cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldarg.1
    IL_000e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Z
    IL_0013:  ldloc.0
    IL_0014:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0019:  nop
    IL_001a:  ret
  } // end of method TestCases::Issue1250_Test1

  .method private hidebysig instance uint8[] 
          Issue1314() cil managed
  {
    // Code size       23 (0x17)
    .maxstack  3
    .locals init (uint8[] V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.4
    IL_0002:  newarr     [mscorlib]System.Byte
    IL_0007:  dup
    IL_0008:  ldtoken    field int32 '<PrivateImplementationDetails>'::'$$method0x6000045-1'
    IL_000d:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0012:  stloc.0
    IL_0013:  br.s       IL_0015

    IL_0015:  ldloc.0
    IL_0016:  ret
  } // end of method TestCases::Issue1314

  .method private hidebysig instance void 
          Issue1251_Test(class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item> list,
                         class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem otherItem) cil managed
  {
    // Code size       162 (0xa2)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item V_0,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::.ctor()
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldstr      "Text"
    IL_000e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Text(string)
    IL_0013:  nop
    IL_0014:  ldloc.0
    IL_0015:  ldarg.2
    IL_0016:  callvirt   instance valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Value()
    IL_001b:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value(valuetype [mscorlib]System.Decimal)
    IL_0020:  nop
    IL_0021:  ldloc.0
    IL_0022:  ldarg.2
    IL_0023:  callvirt   instance valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Value2()
    IL_0028:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value2(valuetype [mscorlib]System.Decimal)
    IL_002d:  nop
    IL_002e:  ldloc.0
    IL_002f:  ldarg.2
    IL_0030:  callvirt   instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Nullable()
    IL_0035:  stloc.1
    IL_0036:  ldloca.s   V_1
    IL_0038:  constrained. valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_003e:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0043:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value3(string)
    IL_0048:  nop
    IL_0049:  ldloc.0
    IL_004a:  ldarg.2
    IL_004b:  callvirt   instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Nullable2()
    IL_0050:  stloc.1
    IL_0051:  ldloca.s   V_1
    IL_0053:  constrained. valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0059:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_005e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value4(string)
    IL_0063:  nop
    IL_0064:  ldloc.0
    IL_0065:  ldarg.2
    IL_0066:  callvirt   instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Nullable3()
    IL_006b:  stloc.1
    IL_006c:  ldloca.s   V_1
    IL_006e:  constrained. valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0074:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0079:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value5(string)
    IL_007e:  nop
    IL_007f:  ldloc.0
    IL_0080:  ldarg.2
    IL_0081:  callvirt   instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Nullable4()
    IL_0086:  stloc.1
    IL_0087:  ldloca.s   V_1
    IL_0089:  constrained. valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_008f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0094:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value6(string)
    IL_0099:  nop
    IL_009a:  ldloc.0
    IL_009b:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item>::Add(!0)
    IL_00a0:  nop
    IL_00a1:  ret
  } // end of method TestCases::Issue1251_Test

  .method private hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data 
          Issue1279(int32 p) cil managed
  {
    // Code size       56 (0x38)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.1
    IL_0003:  ceq
    IL_0005:  ldc.i4.0
    IL_0006:  ceq
    IL_0008:  stloc.2
    IL_0009:  ldloc.2
    IL_000a:  brtrue.s   IL_0032

    IL_000c:  nop
    IL_000d:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_0012:  stloc.0
    IL_0013:  ldloc.0
    IL_0014:  ldc.i4.0
    IL_0015:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_001a:  nop
    IL_001b:  ldloc.0
    IL_001c:  ldarg.0
    IL_001d:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Data_TestEvent(object,
                                                                                                                                class [mscorlib]System.EventArgs)
    IL_0023:  newobj     instance void [mscorlib]System.EventHandler::.ctor(object,
                                                                            native int)
    IL_0028:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::add_TestEvent(class [mscorlib]System.EventHandler)
    IL_002d:  nop
    IL_002e:  ldloc.0
    IL_002f:  stloc.1
    IL_0030:  br.s       IL_0036

    IL_0032:  ldnull
    IL_0033:  stloc.1
    IL_0034:  br.s       IL_0036

    IL_0036:  ldloc.1
    IL_0037:  ret
  } // end of method TestCases::Issue1279

  .method public hidebysig static void  ExtensionMethodInCollectionInitializer() cil managed
  {
    // Code size       37 (0x25)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32> V_0)
    IL_0000:  nop
    IL_0001:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32>::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldstr      "1"
    IL_000d:  ldstr      "2"
    IL_0012:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.Extensions::Add(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32>,
                                                                                                             string,
                                                                                                             string)
    IL_0017:  nop
    IL_0018:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_001d:  ldloc.0
    IL_001e:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0023:  nop
    IL_0024:  ret
  } // end of method TestCases::ExtensionMethodInCollectionInitializer

  .method public hidebysig static void  NoCollectionInitializerBecauseOfTypeArguments() cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32> V_0)
    IL_0000:  nop
    IL_0001:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32>::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldstr      "int"
    IL_000d:  callvirt   instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32>::Add<int32>(string)
    IL_0012:  nop
    IL_0013:  ldloc.0
    IL_0014:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0019:  nop
    IL_001a:  ret
  } // end of method TestCases::NoCollectionInitializerBecauseOfTypeArguments

  .method public hidebysig static void  CollectionInitializerWithParamsMethod() cil managed
  {
    // Code size       79 (0x4f)
    .maxstack  5
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32> V_0,
             int32[] V_1)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32>::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.s   10
    IL_000f:  newarr     [mscorlib]System.Int32
    IL_0014:  stloc.1
    IL_0015:  ldloc.1
    IL_0016:  ldc.i4.0
    IL_0017:  ldc.i4.1
    IL_0018:  stelem.i4
    IL_0019:  ldloc.1
    IL_001a:  ldc.i4.1
    IL_001b:  ldc.i4.2
    IL_001c:  stelem.i4
    IL_001d:  ldloc.1
    IL_001e:  ldc.i4.2
    IL_001f:  ldc.i4.3
    IL_0020:  stelem.i4
    IL_0021:  ldloc.1
    IL_0022:  ldc.i4.3
    IL_0023:  ldc.i4.4
    IL_0024:  stelem.i4
    IL_0025:  ldloc.1
    IL_0026:  ldc.i4.4
    IL_0027:  ldc.i4.5
    IL_0028:  stelem.i4
    IL_0029:  ldloc.1
    IL_002a:  ldc.i4.5
    IL_002b:  ldc.i4.6
    IL_002c:  stelem.i4
    IL_002d:  ldloc.1
    IL_002e:  ldc.i4.6
    IL_002f:  ldc.i4.7
    IL_0030:  stelem.i4
    IL_0031:  ldloc.1
    IL_0032:  ldc.i4.7
    IL_0033:  ldc.i4.8
    IL_0034:  stelem.i4
    IL_0035:  ldloc.1
    IL_0036:  ldc.i4.8
    IL_0037:  ldc.i4.s   9
    IL_0039:  stelem.i4
    IL_003a:  ldloc.1
    IL_003b:  ldc.i4.s   9
    IL_003d:  ldc.i4.s   10
    IL_003f:  stelem.i4
    IL_0040:  ldloc.1
    IL_0041:  callvirt   instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32>::Add(int32[])
    IL_0046:  nop
    IL_0047:  ldloc.0
    IL_0048:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_004d:  nop
    IL_004e:  ret
  } // end of method TestCases::CollectionInitializerWithParamsMethod

  .method public hidebysig static void  CollectionInitializerList() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.List`1<int32> V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.1
    IL_000e:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0013:  nop
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.2
    IL_0016:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_001b:  nop
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.3
    IL_001e:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0023:  nop
    IL_0024:  ldloc.0
    IL_0025:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_002a:  nop
    IL_002b:  ret
  } // end of method TestCases::CollectionInitializerList

  .method public hidebysig static object 
          RecursiveCollectionInitializer() cil managed
  {
    // Code size       21 (0x15)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.Generic.List`1<object> V_0,
             object V_1)
    IL_0000:  nop
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<object>::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldloc.0
    IL_0009:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<object>::Add(!0)
    IL_000e:  nop
    IL_000f:  ldloc.0
    IL_0010:  stloc.1
    IL_0011:  br.s       IL_0013

    IL_0013:  ldloc.1
    IL_0014:  ret
  } // end of method TestCases::RecursiveCollectionInitializer

  .method public hidebysig static void  CollectionInitializerDictionary() cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  4
    .locals init (class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldstr      "First"
    IL_0012:  ldc.i4.1
    IL_0013:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0018:  nop
    IL_0019:  ldloc.0
    IL_001a:  ldstr      "Second"
    IL_001f:  ldc.i4.2
    IL_0020:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0025:  nop
    IL_0026:  ldloc.0
    IL_0027:  ldstr      "Third"
    IL_002c:  ldc.i4.3
    IL_002d:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0032:  nop
    IL_0033:  ldloc.0
    IL_0034:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0039:  nop
    IL_003a:  ret
  } // end of method TestCases::CollectionInitializerDictionary

  .method public hidebysig static void  CollectionInitializerDictionaryWithEnumTypes() cil managed
  {
    // Code size       38 (0x26)
    .maxstack  4
    .locals init (class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0006:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.0
    IL_000e:  ldc.i4.0
    IL_000f:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0,
                                                                                                                                                                                                                                                                                          !1)
    IL_0014:  nop
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.1
    IL_0017:  ldc.i4.1
    IL_0018:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0,
                                                                                                                                                                                                                                                                                          !1)
    IL_001d:  nop
    IL_001e:  ldloc.0
    IL_001f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0024:  nop
    IL_0025:  ret
  } // end of method TestCases::CollectionInitializerDictionaryWithEnumTypes

  .method public hidebysig static void  NotACollectionInitializer() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.Generic.List`1<int32> V_0)
    IL_0000:  nop
    IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_000e:  nop
    IL_000f:  ldloc.0
    IL_0010:  ldc.i4.2
    IL_0011:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0016:  nop
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4.3
    IL_0019:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_001e:  nop
    IL_001f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0024:  ldloc.0
    IL_0025:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_002a:  nop
    IL_002b:  ret
  } // end of method TestCases::NotACollectionInitializer

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method TestCases::.ctor

  .method private hidebysig static void  '<NotAnObjectInitializerWithEvent>b__5'(object param0,
                                                                                 class [mscorlib]System.EventArgs param1) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       void [mscorlib]System.Console::WriteLine()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method TestCases::'<NotAnObjectInitializerWithEvent>b__5'

  .method private hidebysig static bool  '<Issue270_NestedInitialisers>b__19'(class [mscorlib]System.Globalization.NumberFormatInfo format) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance string [mscorlib]System.Globalization.NumberFormatInfo::get_CurrencySymbol()
    IL_0006:  ldstr      "$"
    IL_000b:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0010:  stloc.0
    IL_0011:  br.s       IL_0013

    IL_0013:  ldloc.0
    IL_0014:  ret
  } // end of method TestCases::'<Issue270_NestedInitialisers>b__19'

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       1949 (0x79d)
    .maxstack  8
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[] V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...] V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][] V_2,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...] V_3,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...][] V_4)
    IL_0000:  ldc.i4.3
    IL_0001:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.r4     1.
    IL_000e:  ldc.r4     1.
    IL_0013:  ldc.r4     1.
    IL_0018:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_001d:  stelem.ref
    IL_001e:  ldloc.0
    IL_001f:  ldc.i4.1
    IL_0020:  ldc.r4     2.
    IL_0025:  ldc.r4     2.
    IL_002a:  ldc.r4     2.
    IL_002f:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0034:  stelem.ref
    IL_0035:  ldloc.0
    IL_0036:  ldc.i4.2
    IL_0037:  ldc.r4     3.
    IL_003c:  ldc.r4     3.
    IL_0041:  ldc.r4     3.
    IL_0046:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_004b:  stelem.ref
    IL_004c:  ldloc.0
    IL_004d:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Issue1336_rg0
    IL_0052:  ldc.i4.3
    IL_0053:  ldc.i4.3
    IL_0054:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::.ctor(int32,
                                                                                                                                            int32)
    IL_0059:  stloc.1
    IL_005a:  ldloc.1
    IL_005b:  ldc.i4.0
    IL_005c:  ldc.i4.0
    IL_005d:  ldc.r4     1.
    IL_0062:  ldc.r4     1.
    IL_0067:  ldc.r4     1.
    IL_006c:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0071:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0076:  ldloc.1
    IL_0077:  ldc.i4.0
    IL_0078:  ldc.i4.1
    IL_0079:  ldc.r4     2.
    IL_007e:  ldc.r4     2.
    IL_0083:  ldc.r4     2.
    IL_0088:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_008d:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0092:  ldloc.1
    IL_0093:  ldc.i4.0
    IL_0094:  ldc.i4.2
    IL_0095:  ldc.r4     3.
    IL_009a:  ldc.r4     3.
    IL_009f:  ldc.r4     3.
    IL_00a4:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_00a9:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_00ae:  ldloc.1
    IL_00af:  ldc.i4.1
    IL_00b0:  ldc.i4.0
    IL_00b1:  ldc.r4     2.
    IL_00b6:  ldc.r4     2.
    IL_00bb:  ldc.r4     2.
    IL_00c0:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_00c5:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_00ca:  ldloc.1
    IL_00cb:  ldc.i4.1
    IL_00cc:  ldc.i4.1
    IL_00cd:  ldc.r4     3.
    IL_00d2:  ldc.r4     3.
    IL_00d7:  ldc.r4     3.
    IL_00dc:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_00e1:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_00e6:  ldloc.1
    IL_00e7:  ldc.i4.1
    IL_00e8:  ldc.i4.2
    IL_00e9:  ldc.r4     4.
    IL_00ee:  ldc.r4     4.
    IL_00f3:  ldc.r4     4.
    IL_00f8:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_00fd:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0102:  ldloc.1
    IL_0103:  ldc.i4.2
    IL_0104:  ldc.i4.0
    IL_0105:  ldc.r4     3.
    IL_010a:  ldc.r4     3.
    IL_010f:  ldc.r4     3.
    IL_0114:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0119:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_011e:  ldloc.1
    IL_011f:  ldc.i4.2
    IL_0120:  ldc.i4.1
    IL_0121:  ldc.r4     4.
    IL_0126:  ldc.r4     4.
    IL_012b:  ldc.r4     4.
    IL_0130:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0135:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_013a:  ldloc.1
    IL_013b:  ldc.i4.2
    IL_013c:  ldc.i4.2
    IL_013d:  ldc.r4     5.
    IL_0142:  ldc.r4     5.
    IL_0147:  ldc.r4     5.
    IL_014c:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0151:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0156:  ldloc.1
    IL_0157:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Issue1336_rg1
    IL_015c:  ldc.i4.3
    IL_015d:  newarr     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[]
    IL_0162:  stloc.2
    IL_0163:  ldloc.2
    IL_0164:  ldc.i4.0
    IL_0165:  ldc.i4.3
    IL_0166:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_016b:  stloc.0
    IL_016c:  ldloc.0
    IL_016d:  ldc.i4.0
    IL_016e:  ldc.r4     1.
    IL_0173:  ldc.r4     1.
    IL_0178:  ldc.r4     1.
    IL_017d:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0182:  stelem.ref
    IL_0183:  ldloc.0
    IL_0184:  ldc.i4.1
    IL_0185:  ldc.r4     2.
    IL_018a:  ldc.r4     2.
    IL_018f:  ldc.r4     2.
    IL_0194:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0199:  stelem.ref
    IL_019a:  ldloc.0
    IL_019b:  ldc.i4.2
    IL_019c:  ldc.r4     3.
    IL_01a1:  ldc.r4     3.
    IL_01a6:  ldc.r4     3.
    IL_01ab:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_01b0:  stelem.ref
    IL_01b1:  ldloc.0
    IL_01b2:  stelem.ref
    IL_01b3:  ldloc.2
    IL_01b4:  ldc.i4.1
    IL_01b5:  ldc.i4.3
    IL_01b6:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_01bb:  stloc.0
    IL_01bc:  ldloc.0
    IL_01bd:  ldc.i4.0
    IL_01be:  ldc.r4     2.
    IL_01c3:  ldc.r4     2.
    IL_01c8:  ldc.r4     2.
    IL_01cd:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_01d2:  stelem.ref
    IL_01d3:  ldloc.0
    IL_01d4:  ldc.i4.1
    IL_01d5:  ldc.r4     3.
    IL_01da:  ldc.r4     3.
    IL_01df:  ldc.r4     3.
    IL_01e4:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_01e9:  stelem.ref
    IL_01ea:  ldloc.0
    IL_01eb:  ldc.i4.2
    IL_01ec:  ldc.r4     4.
    IL_01f1:  ldc.r4     4.
    IL_01f6:  ldc.r4     4.
    IL_01fb:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0200:  stelem.ref
    IL_0201:  ldloc.0
    IL_0202:  stelem.ref
    IL_0203:  ldloc.2
    IL_0204:  ldc.i4.2
    IL_0205:  ldc.i4.3
    IL_0206:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_020b:  stloc.0
    IL_020c:  ldloc.0
    IL_020d:  ldc.i4.0
    IL_020e:  ldc.r4     3.
    IL_0213:  ldc.r4     3.
    IL_0218:  ldc.r4     3.
    IL_021d:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0222:  stelem.ref
    IL_0223:  ldloc.0
    IL_0224:  ldc.i4.1
    IL_0225:  ldc.r4     4.
    IL_022a:  ldc.r4     4.
    IL_022f:  ldc.r4     4.
    IL_0234:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0239:  stelem.ref
    IL_023a:  ldloc.0
    IL_023b:  ldc.i4.2
    IL_023c:  ldc.r4     5.
    IL_0241:  ldc.r4     5.
    IL_0246:  ldc.r4     5.
    IL_024b:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0250:  stelem.ref
    IL_0251:  ldloc.0
    IL_0252:  stelem.ref
    IL_0253:  ldloc.2
    IL_0254:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Issue1336_rg1b
    IL_0259:  ldc.i4.3
    IL_025a:  ldc.i4.3
    IL_025b:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::.ctor(int32,
                                                                                                                                              int32)
    IL_0260:  stloc.3
    IL_0261:  ldloc.3
    IL_0262:  ldc.i4.0
    IL_0263:  ldc.i4.0
    IL_0264:  ldc.i4.3
    IL_0265:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_026a:  stloc.0
    IL_026b:  ldloc.0
    IL_026c:  ldc.i4.0
    IL_026d:  ldc.r4     1.
    IL_0272:  ldc.r4     1.
    IL_0277:  ldc.r4     1.
    IL_027c:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0281:  stelem.ref
    IL_0282:  ldloc.0
    IL_0283:  ldc.i4.1
    IL_0284:  ldc.r4     2.
    IL_0289:  ldc.r4     2.
    IL_028e:  ldc.r4     2.
    IL_0293:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0298:  stelem.ref
    IL_0299:  ldloc.0
    IL_029a:  ldc.i4.2
    IL_029b:  ldc.r4     3.
    IL_02a0:  ldc.r4     3.
    IL_02a5:  ldc.r4     3.
    IL_02aa:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_02af:  stelem.ref
    IL_02b0:  ldloc.0
    IL_02b1:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_02b6:  ldloc.3
    IL_02b7:  ldc.i4.0
    IL_02b8:  ldc.i4.1
    IL_02b9:  ldc.i4.3
    IL_02ba:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_02bf:  stloc.0
    IL_02c0:  ldloc.0
    IL_02c1:  ldc.i4.0
    IL_02c2:  ldc.r4     2.
    IL_02c7:  ldc.r4     2.
    IL_02cc:  ldc.r4     2.
    IL_02d1:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_02d6:  stelem.ref
    IL_02d7:  ldloc.0
    IL_02d8:  ldc.i4.1
    IL_02d9:  ldc.r4     3.
    IL_02de:  ldc.r4     3.
    IL_02e3:  ldc.r4     3.
    IL_02e8:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_02ed:  stelem.ref
    IL_02ee:  ldloc.0
    IL_02ef:  ldc.i4.2
    IL_02f0:  ldc.r4     4.
    IL_02f5:  ldc.r4     4.
    IL_02fa:  ldc.r4     4.
    IL_02ff:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0304:  stelem.ref
    IL_0305:  ldloc.0
    IL_0306:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_030b:  ldloc.3
    IL_030c:  ldc.i4.0
    IL_030d:  ldc.i4.2
    IL_030e:  ldc.i4.3
    IL_030f:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_0314:  stloc.0
    IL_0315:  ldloc.0
    IL_0316:  ldc.i4.0
    IL_0317:  ldc.r4     3.
    IL_031c:  ldc.r4     3.
    IL_0321:  ldc.r4     3.
    IL_0326:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_032b:  stelem.ref
    IL_032c:  ldloc.0
    IL_032d:  ldc.i4.1
    IL_032e:  ldc.r4     4.
    IL_0333:  ldc.r4     4.
    IL_0338:  ldc.r4     4.
    IL_033d:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0342:  stelem.ref
    IL_0343:  ldloc.0
    IL_0344:  ldc.i4.2
    IL_0345:  ldc.r4     5.
    IL_034a:  ldc.r4     5.
    IL_034f:  ldc.r4     5.
    IL_0354:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0359:  stelem.ref
    IL_035a:  ldloc.0
    IL_035b:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_0360:  ldloc.3
    IL_0361:  ldc.i4.1
    IL_0362:  ldc.i4.0
    IL_0363:  ldc.i4.3
    IL_0364:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_0369:  stloc.0
    IL_036a:  ldloc.0
    IL_036b:  ldc.i4.0
    IL_036c:  ldc.r4     1.
    IL_0371:  ldc.r4     1.
    IL_0376:  ldc.r4     1.
    IL_037b:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0380:  stelem.ref
    IL_0381:  ldloc.0
    IL_0382:  ldc.i4.1
    IL_0383:  ldc.r4     2.
    IL_0388:  ldc.r4     2.
    IL_038d:  ldc.r4     2.
    IL_0392:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0397:  stelem.ref
    IL_0398:  ldloc.0
    IL_0399:  ldc.i4.2
    IL_039a:  ldc.r4     3.
    IL_039f:  ldc.r4     3.
    IL_03a4:  ldc.r4     3.
    IL_03a9:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_03ae:  stelem.ref
    IL_03af:  ldloc.0
    IL_03b0:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_03b5:  ldloc.3
    IL_03b6:  ldc.i4.1
    IL_03b7:  ldc.i4.1
    IL_03b8:  ldc.i4.3
    IL_03b9:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_03be:  stloc.0
    IL_03bf:  ldloc.0
    IL_03c0:  ldc.i4.0
    IL_03c1:  ldc.r4     2.
    IL_03c6:  ldc.r4     2.
    IL_03cb:  ldc.r4     2.
    IL_03d0:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_03d5:  stelem.ref
    IL_03d6:  ldloc.0
    IL_03d7:  ldc.i4.1
    IL_03d8:  ldc.r4     3.
    IL_03dd:  ldc.r4     3.
    IL_03e2:  ldc.r4     3.
    IL_03e7:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_03ec:  stelem.ref
    IL_03ed:  ldloc.0
    IL_03ee:  ldc.i4.2
    IL_03ef:  ldc.r4     4.
    IL_03f4:  ldc.r4     4.
    IL_03f9:  ldc.r4     4.
    IL_03fe:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0403:  stelem.ref
    IL_0404:  ldloc.0
    IL_0405:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_040a:  ldloc.3
    IL_040b:  ldc.i4.1
    IL_040c:  ldc.i4.2
    IL_040d:  ldc.i4.3
    IL_040e:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_0413:  stloc.0
    IL_0414:  ldloc.0
    IL_0415:  ldc.i4.0
    IL_0416:  ldc.r4     3.
    IL_041b:  ldc.r4     3.
    IL_0420:  ldc.r4     3.
    IL_0425:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_042a:  stelem.ref
    IL_042b:  ldloc.0
    IL_042c:  ldc.i4.1
    IL_042d:  ldc.r4     4.
    IL_0432:  ldc.r4     4.
    IL_0437:  ldc.r4     4.
    IL_043c:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0441:  stelem.ref
    IL_0442:  ldloc.0
    IL_0443:  ldc.i4.2
    IL_0444:  ldc.r4     5.
    IL_0449:  ldc.r4     5.
    IL_044e:  ldc.r4     5.
    IL_0453:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0458:  stelem.ref
    IL_0459:  ldloc.0
    IL_045a:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_045f:  ldloc.3
    IL_0460:  ldc.i4.2
    IL_0461:  ldc.i4.0
    IL_0462:  ldc.i4.3
    IL_0463:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_0468:  stloc.0
    IL_0469:  ldloc.0
    IL_046a:  ldc.i4.0
    IL_046b:  ldc.r4     1.
    IL_0470:  ldc.r4     1.
    IL_0475:  ldc.r4     1.
    IL_047a:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_047f:  stelem.ref
    IL_0480:  ldloc.0
    IL_0481:  ldc.i4.1
    IL_0482:  ldc.r4     2.
    IL_0487:  ldc.r4     2.
    IL_048c:  ldc.r4     2.
    IL_0491:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0496:  stelem.ref
    IL_0497:  ldloc.0
    IL_0498:  ldc.i4.2
    IL_0499:  ldc.r4     3.
    IL_049e:  ldc.r4     3.
    IL_04a3:  ldc.r4     3.
    IL_04a8:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_04ad:  stelem.ref
    IL_04ae:  ldloc.0
    IL_04af:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_04b4:  ldloc.3
    IL_04b5:  ldc.i4.2
    IL_04b6:  ldc.i4.1
    IL_04b7:  ldc.i4.3
    IL_04b8:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_04bd:  stloc.0
    IL_04be:  ldloc.0
    IL_04bf:  ldc.i4.0
    IL_04c0:  ldc.r4     2.
    IL_04c5:  ldc.r4     2.
    IL_04ca:  ldc.r4     2.
    IL_04cf:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_04d4:  stelem.ref
    IL_04d5:  ldloc.0
    IL_04d6:  ldc.i4.1
    IL_04d7:  ldc.r4     3.
    IL_04dc:  ldc.r4     3.
    IL_04e1:  ldc.r4     3.
    IL_04e6:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_04eb:  stelem.ref
    IL_04ec:  ldloc.0
    IL_04ed:  ldc.i4.2
    IL_04ee:  ldc.r4     4.
    IL_04f3:  ldc.r4     4.
    IL_04f8:  ldc.r4     4.
    IL_04fd:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0502:  stelem.ref
    IL_0503:  ldloc.0
    IL_0504:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_0509:  ldloc.3
    IL_050a:  ldc.i4.2
    IL_050b:  ldc.i4.2
    IL_050c:  ldc.i4.3
    IL_050d:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_0512:  stloc.0
    IL_0513:  ldloc.0
    IL_0514:  ldc.i4.0
    IL_0515:  ldc.r4     3.
    IL_051a:  ldc.r4     3.
    IL_051f:  ldc.r4     3.
    IL_0524:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0529:  stelem.ref
    IL_052a:  ldloc.0
    IL_052b:  ldc.i4.1
    IL_052c:  ldc.r4     4.
    IL_0531:  ldc.r4     4.
    IL_0536:  ldc.r4     4.
    IL_053b:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0540:  stelem.ref
    IL_0541:  ldloc.0
    IL_0542:  ldc.i4.2
    IL_0543:  ldc.r4     5.
    IL_0548:  ldc.r4     5.
    IL_054d:  ldc.r4     5.
    IL_0552:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0557:  stelem.ref
    IL_0558:  ldloc.0
    IL_0559:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_055e:  ldloc.3
    IL_055f:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Issue1336_rg1c
    IL_0564:  ldc.i4.2
    IL_0565:  newarr     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]
    IL_056a:  stloc.s    V_4
    IL_056c:  ldloc.s    V_4
    IL_056e:  ldc.i4.0
    IL_056f:  ldc.i4.3
    IL_0570:  ldc.i4.3
    IL_0571:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::.ctor(int32,
                                                                                                                                            int32)
    IL_0576:  stloc.1
    IL_0577:  ldloc.1
    IL_0578:  ldc.i4.0
    IL_0579:  ldc.i4.0
    IL_057a:  ldc.r4     1.
    IL_057f:  ldc.r4     1.
    IL_0584:  ldc.r4     1.
    IL_0589:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_058e:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0593:  ldloc.1
    IL_0594:  ldc.i4.0
    IL_0595:  ldc.i4.1
    IL_0596:  ldc.r4     2.
    IL_059b:  ldc.r4     2.
    IL_05a0:  ldc.r4     2.
    IL_05a5:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_05aa:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_05af:  ldloc.1
    IL_05b0:  ldc.i4.0
    IL_05b1:  ldc.i4.2
    IL_05b2:  ldc.r4     3.
    IL_05b7:  ldc.r4     3.
    IL_05bc:  ldc.r4     3.
    IL_05c1:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_05c6:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_05cb:  ldloc.1
    IL_05cc:  ldc.i4.1
    IL_05cd:  ldc.i4.0
    IL_05ce:  ldc.r4     2.
    IL_05d3:  ldc.r4     2.
    IL_05d8:  ldc.r4     2.
    IL_05dd:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_05e2:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_05e7:  ldloc.1
    IL_05e8:  ldc.i4.1
    IL_05e9:  ldc.i4.1
    IL_05ea:  ldc.r4     3.
    IL_05ef:  ldc.r4     3.
    IL_05f4:  ldc.r4     3.
    IL_05f9:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_05fe:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0603:  ldloc.1
    IL_0604:  ldc.i4.1
    IL_0605:  ldc.i4.2
    IL_0606:  ldc.r4     4.
    IL_060b:  ldc.r4     4.
    IL_0610:  ldc.r4     4.
    IL_0615:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_061a:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_061f:  ldloc.1
    IL_0620:  ldc.i4.2
    IL_0621:  ldc.i4.0
    IL_0622:  ldc.r4     3.
    IL_0627:  ldc.r4     3.
    IL_062c:  ldc.r4     3.
    IL_0631:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0636:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_063b:  ldloc.1
    IL_063c:  ldc.i4.2
    IL_063d:  ldc.i4.1
    IL_063e:  ldc.r4     4.
    IL_0643:  ldc.r4     4.
    IL_0648:  ldc.r4     4.
    IL_064d:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0652:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0657:  ldloc.1
    IL_0658:  ldc.i4.2
    IL_0659:  ldc.i4.2
    IL_065a:  ldc.r4     5.
    IL_065f:  ldc.r4     5.
    IL_0664:  ldc.r4     5.
    IL_0669:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_066e:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0673:  ldloc.1
    IL_0674:  stelem.ref
    IL_0675:  ldloc.s    V_4
    IL_0677:  ldc.i4.1
    IL_0678:  ldc.i4.3
    IL_0679:  ldc.i4.3
    IL_067a:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::.ctor(int32,
                                                                                                                                            int32)
    IL_067f:  stloc.1
    IL_0680:  ldloc.1
    IL_0681:  ldc.i4.0
    IL_0682:  ldc.i4.0
    IL_0683:  ldc.r4     1.
    IL_0688:  ldc.r4     1.
    IL_068d:  ldc.r4     1.
    IL_0692:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0697:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_069c:  ldloc.1
    IL_069d:  ldc.i4.0
    IL_069e:  ldc.i4.1
    IL_069f:  ldc.r4     2.
    IL_06a4:  ldc.r4     2.
    IL_06a9:  ldc.r4     2.
    IL_06ae:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_06b3:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_06b8:  ldloc.1
    IL_06b9:  ldc.i4.0
    IL_06ba:  ldc.i4.2
    IL_06bb:  ldc.r4     3.
    IL_06c0:  ldc.r4     3.
    IL_06c5:  ldc.r4     3.
    IL_06ca:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_06cf:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_06d4:  ldloc.1
    IL_06d5:  ldc.i4.1
    IL_06d6:  ldc.i4.0
    IL_06d7:  ldc.r4     2.
    IL_06dc:  ldc.r4     2.
    IL_06e1:  ldc.r4     2.
    IL_06e6:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_06eb:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_06f0:  ldloc.1
    IL_06f1:  ldc.i4.1
    IL_06f2:  ldc.i4.1
    IL_06f3:  ldc.r4     3.
    IL_06f8:  ldc.r4     3.
    IL_06fd:  ldc.r4     3.
    IL_0702:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0707:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_070c:  ldloc.1
    IL_070d:  ldc.i4.1
    IL_070e:  ldc.i4.2
    IL_070f:  ldc.r4     4.
    IL_0714:  ldc.r4     4.
    IL_0719:  ldc.r4     4.
    IL_071e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0723:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0728:  ldloc.1
    IL_0729:  ldc.i4.2
    IL_072a:  ldc.i4.0
    IL_072b:  ldc.r4     3.
    IL_0730:  ldc.r4     3.
    IL_0735:  ldc.r4     3.
    IL_073a:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_073f:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0744:  ldloc.1
    IL_0745:  ldc.i4.2
    IL_0746:  ldc.i4.1
    IL_0747:  ldc.r4     4.
    IL_074c:  ldc.r4     4.
    IL_0751:  ldc.r4     4.
    IL_0756:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_075b:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0760:  ldloc.1
    IL_0761:  ldc.i4.2
    IL_0762:  ldc.i4.2
    IL_0763:  ldc.r4     5.
    IL_0768:  ldc.r4     5.
    IL_076d:  ldc.r4     5.
    IL_0772:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0777:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_077c:  ldloc.1
    IL_077d:  stelem.ref
    IL_077e:  ldloc.s    V_4
    IL_0780:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...][] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Issue1336_rg1d
    IL_0785:  ldc.i4.3
    IL_0786:  ldc.i4.3
    IL_0787:  newobj     instance void int32[0...,0...]::.ctor(int32,
                                                               int32)
    IL_078c:  dup
    IL_078d:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=36' '<PrivateImplementationDetails>'::'$$method0x6000094-1'
    IL_0792:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0797:  stsfld     int32[0...,0...] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Issue1336_rg2
    IL_079c:  ret
  } // end of method TestCases::.cctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases

.class private auto ansi '<PrivateImplementationDetails>'
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=40'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 40
  } // end of class '__StaticArrayInitTypeSize=40'

  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=24'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 24
  } // end of class '__StaticArrayInitTypeSize=24'

  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=10'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 10
  } // end of class '__StaticArrayInitTypeSize=10'

  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=12'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 12
  } // end of class '__StaticArrayInitTypeSize=12'

  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=80'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 80
  } // end of class '__StaticArrayInitTypeSize=80'

  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=48'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 48
  } // end of class '__StaticArrayInitTypeSize=48'

  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=256'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 256
  } // end of class '__StaticArrayInitTypeSize=256'

  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=64'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 64
  } // end of class '__StaticArrayInitTypeSize=64'

  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=72'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 72
  } // end of class '__StaticArrayInitTypeSize=72'

  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=36'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 36
  } // end of class '__StaticArrayInitTypeSize=36'

  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '$$method0x600000b-1' at I_000020C0
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '$$method0x600000d-1' at I_00002138
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=24' '$$method0x600000d-2' at I_00002160
  .field static assembly int64 '$$method0x6000010-1' at I_00002418
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=10' '$$method0x6000011-1' at I_00002440
  .field static assembly int64 '$$method0x6000012-1' at I_00002470
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=10' '$$method0x6000013-1' at I_00002498
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=12' '$$method0x6000014-1' at I_000024C8
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '$$method0x6000015-1' at I_000024F8
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '$$method0x6000016-1' at I_00002540
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '$$method0x6000017-1' at I_00002588
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=80' '$$method0x6000018-1' at I_000025D0
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=24' '$$method0x6000019-1' at I_00002640
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=48' '$$method0x600001a-1' at I_00002678
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=256' '$$method0x600001e-1' at I_000027C0
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' '$$method0x600001f-1' at I_000028E8
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' '$$method0x600001f-2' at I_00002928
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' '$$method0x600001f-3' at I_00002968
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' '$$method0x600001f-4' at I_000029A8
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=72' '$$method0x6000020-1' at I_00002A58
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=72' '$$method0x6000020-2' at I_00002AA0
  .field static assembly int32 '$$method0x6000045-1' at I_000034B8
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=36' '$$method0x6000094-1' at I_000037A0
} // end of class '<PrivateImplementationDetails>'


// =============================================================

.data cil I_000020C0 = bytearray (
                 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00
                 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00
                 09 00 00 00 0A 00 00 00) 
.data cil I_00002138 = bytearray (
                 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00
                 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00
                 09 00 00 00 0A 00 00 00) 
.data cil I_00002160 = bytearray (
                 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00
                 05 00 00 00 06 00 00 00) 
.data cil I_00002418 = bytearray (
                 01 00 01 00 00 00 01 01) 
.data cil I_00002420 = int8[32]
.data cil I_00002440 = bytearray (
                 01 02 03 04 05 06 07 08 FE FF) 
.data cil I_0000244A = int8[6]
.data cil I_00002470 = bytearray (
                 80 81 00 01 02 03 04 7F) 
.data cil I_00002498 = bytearray (
                 00 80 FF FF 00 00 01 00 FF 7F) 
.data cil I_000024A2 = int8[6]
.data cil I_000024C8 = bytearray (
                 00 00 01 00 FF 7F 00 80 FE FF FF FF) 
.data cil I_000024D4 = int8[4]
.data cil I_000024F8 = bytearray (
                 01 00 00 00 FE FF FF FF 00 94 35 77 04 00 00 00  // ..........5w....
                 05 00 00 00 FA FF FF FF 07 00 00 00 08 00 00 00
                 09 00 00 00 0A 00 00 00) 
.data cil I_00002520 = int8[32]
.data cil I_00002540 = bytearray (
                 01 00 00 00 00 94 35 77 00 5E D0 B2 04 00 00 00  // ......5w.^......
                 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00
                 09 00 00 00 0A 00 00 00) 
.data cil I_00002588 = bytearray (
                 01 00 0C BB 7D 6E 9C BA FF FF FF FF FF FF FF FF  // ....}n..........
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 FF FF F3 44 82 91 63 45)                         // ...D..cE
.data cil I_000025D0 = bytearray (
                 01 00 00 00 00 00 00 00 00 94 35 77 00 00 00 00  // ..........5w....
                 00 5E D0 B2 00 00 00 00 04 00 00 00 00 00 00 00  // .^..............
                 05 00 00 00 00 00 00 00 06 00 00 00 00 00 00 00
                 07 00 00 00 00 00 00 00 08 00 00 00 00 00 00 00
                 FF FF F3 44 82 91 63 45 FF FF E7 89 04 23 C7 8A) // ...D..cE.....#..
.data cil I_00002620 = int8[32]
.data cil I_00002640 = bytearray (
                 00 00 C0 BF 00 00 00 00 00 00 C0 3F 00 00 80 FF  // ...........?....
                 00 00 80 7F 00 00 C0 FF) 
.data cil I_00002678 = bytearray (
                 00 00 00 00 00 00 F8 BF 00 00 00 00 00 00 00 00
                 00 00 00 00 00 00 F8 3F 00 00 00 00 00 00 F0 FF  // .......?........
                 00 00 00 00 00 00 F0 7F 00 00 00 00 00 00 F8 FF) 
.data cil I_000026A8 = int8[24]
.data cil I_000027C0 = bytearray (
                 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                 01 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00
                 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                 01 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00
                 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00) 
.data cil I_000028E8 = bytearray (
                 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                 01 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00
                 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00) 
.data cil I_00002928 = bytearray (
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00) 
.data cil I_00002968 = bytearray (
                 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                 01 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00
                 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00) 
.data cil I_000029A8 = bytearray (
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00) 
.data cil I_00002A58 = bytearray (
                 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00
                 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00
                 09 00 00 00 0B 00 00 00 0C 00 00 00 0D 00 00 00
                 0E 00 00 00 0F 00 00 00 10 00 00 00 11 00 00 00
                 12 00 00 00 13 00 00 00) 
.data cil I_00002AA0 = bytearray (
                 15 00 00 00 16 00 00 00 17 00 00 00 18 00 00 00
                 19 00 00 00 1A 00 00 00 1B 00 00 00 1C 00 00 00
                 1D 00 00 00 1F 00 00 00 20 00 00 00 21 00 00 00  // ........ ...!...
                 22 00 00 00 23 00 00 00 24 00 00 00 25 00 00 00  // "...#...$...%...
                 26 00 00 00 27 00 00 00)                         // &...'...
.data cil I_000034B8 = bytearray (
                 00 01 02 FF) 
.data cil I_000034BC = int8[4]
.data cil I_000037A0 = bytearray (
                 01 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00
                 01 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00
                 01 00 00 00) 
// *********** DISASSEMBLY COMPLETE ***********************
