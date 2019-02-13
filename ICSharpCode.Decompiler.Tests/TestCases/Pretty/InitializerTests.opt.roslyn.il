



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
  .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
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
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method Extensions::Add

  .method public hidebysig static void  Add<T>(class [mscorlib]System.Collections.Generic.IList`1<valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<string,string>> collection,
                                               string key,
                                               !!T 'value',
                                               [opt] class [mscorlib]System.Func`2<!!T,string> convert) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
    .param [4] = nullref
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
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
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomList`1::GetEnumerator

    .method private hidebysig newslot virtual final 
            instance class [mscorlib]System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() cil managed
    {
      .override [mscorlib]System.Collections.IEnumerable::GetEnumerator
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method CustomList`1::System.Collections.IEnumerable.GetEnumerator

    .method public hidebysig instance void 
            Add<T2>(string name) cil managed
    {
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,class [mscorlib]System.Type>::.ctor()
      IL_0005:  ldarg.1
      IL_0006:  ldtoken    !!T2
      IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
      IL_0010:  call       instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,class [mscorlib]System.Type>::Add(!0,
                                                                                                                                          !1)
      IL_0015:  ret
    } // end of method CustomList`1::Add

    .method public hidebysig instance void 
            Add(int32[] ints) cil managed
    {
      .param [1]
      .custom instance void [mscorlib]System.ParamArrayAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
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
      // Code size       10 (0xa)
      .maxstack  1
      .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S V_0)
      IL_0000:  ldloca.s   V_0
      IL_0002:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S
      IL_0008:  ldloc.0
      IL_0009:  ret
    } // end of method C::get_Item

    .method public hidebysig specialname 
            instance void  set_Item(int32 index,
                                    valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S 'value') cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method C::set_Item

    .method public hidebysig specialname 
            instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S 
            get_Item(object key) cil managed
    {
      // Code size       10 (0xa)
      .maxstack  1
      .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S V_0)
      IL_0000:  ldloca.s   V_0
      IL_0002:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S
      IL_0008:  ldloc.0
      IL_0009:  ret
    } // end of method C::get_Item

    .method public hidebysig specialname 
            instance void  set_Item(object key,
                                    valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S 'value') cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
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
      // Code size       15 (0xf)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::A
      IL_0007:  ldarg.0
      IL_0008:  ldc.i4.0
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::B
      IL_000e:  ret
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
    .field private valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum '<a>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum '<b>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> '<PropertyList>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private initonly class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> '<ReadOnlyPropertyList>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data '<MoreData>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData '<NestedStruct>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private class [mscorlib]System.EventHandler TestEvent
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig specialname 
            instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum 
            get_a() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::'<a>k__BackingField'
      IL_0006:  ret
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
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::'<b>k__BackingField'
      IL_0006:  ret
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
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::'<PropertyList>k__BackingField'
      IL_0006:  ret
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
            instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> 
            get_ReadOnlyPropertyList() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::'<ReadOnlyPropertyList>k__BackingField'
      IL_0006:  ret
    } // end of method Data::get_ReadOnlyPropertyList

    .method public hidebysig specialname 
            instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data 
            get_MoreData() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::'<MoreData>k__BackingField'
      IL_0006:  ret
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
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::'<NestedStruct>k__BackingField'
      IL_0006:  ret
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
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ret
    } // end of method Data::get_Item

    .method public hidebysig specialname 
            instance void  set_Item(int32 i,
                                    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data 'value') cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method Data::set_Item

    .method public hidebysig specialname 
            instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data 
            get_Item(int32 i,
                     string j) cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldnull
      IL_0001:  ret
    } // end of method Data::get_Item

    .method public hidebysig specialname 
            instance void  set_Item(int32 i,
                                    string j,
                                    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data 'value') cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method Data::set_Item

    .method public hidebysig specialname 
            instance void  add_TestEvent(class [mscorlib]System.EventHandler 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       41 (0x29)
      .maxstack  3
      .locals init (class [mscorlib]System.EventHandler V_0,
               class [mscorlib]System.EventHandler V_1,
               class [mscorlib]System.EventHandler V_2)
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
      IL_0026:  bne.un.s   IL_0007

      IL_0028:  ret
    } // end of method Data::add_TestEvent

    .method public hidebysig specialname 
            instance void  remove_TestEvent(class [mscorlib]System.EventHandler 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       41 (0x29)
      .maxstack  3
      .locals init (class [mscorlib]System.EventHandler V_0,
               class [mscorlib]System.EventHandler V_1,
               class [mscorlib]System.EventHandler V_2)
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
      IL_0026:  bne.un.s   IL_0007

      IL_0028:  ret
    } // end of method Data::remove_TestEvent

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       18 (0x12)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::.ctor()
      IL_0006:  stfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::FieldList
      IL_000b:  ldarg.0
      IL_000c:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0011:  ret
    } // end of method Data::.ctor

    .event [mscorlib]System.EventHandler TestEvent
    {
      .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::add_TestEvent(class [mscorlib]System.EventHandler)
      .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::remove_TestEvent(class [mscorlib]System.EventHandler)
    } // end of event Data::TestEvent
    .property instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum
            a()
    {
      .get instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_a()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
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
      .get instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_PropertyList()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_PropertyList(class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>)
    } // end of property Data::PropertyList
    .property instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>
            ReadOnlyPropertyList()
    {
      .get instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_ReadOnlyPropertyList()
    } // end of property Data::ReadOnlyPropertyList
    .property instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data
            MoreData()
    {
      .get instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_MoreData(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data)
    } // end of property Data::MoreData
    .property instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData
            NestedStruct()
    {
      .get instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_NestedStruct()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_NestedStruct(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData)
    } // end of property Data::NestedStruct
    .property instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data
            Item(int32)
    {
      .get instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_Item(int32)
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_Item(int32,
                                                                                                                 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data)
    } // end of property Data::Item
    .property instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data
            Item(int32,
                 string)
    {
      .get instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_Item(int32,
                                                                                                                                                                                                string)
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_Item(int32,
                                                                                                                 string,
                                                                                                                 class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data)
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
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::'<Property>k__BackingField'
      IL_0006:  ret
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
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::'<MoreData>k__BackingField'
      IL_0006:  ret
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
      // Code size       22 (0x16)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData
      IL_0007:  ldarg.0
      IL_0008:  ldarg.1
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::Field
      IL_000e:  ldarg.0
      IL_000f:  ldarg.1
      IL_0010:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::set_Property(int32)
      IL_0015:  ret
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
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::'<Text>k__BackingField'
      IL_0006:  ret
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
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::'<Value>k__BackingField'
      IL_0006:  ret
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
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::'<Value2>k__BackingField'
      IL_0006:  ret
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
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::'<Value3>k__BackingField'
      IL_0006:  ret
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
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::'<Value4>k__BackingField'
      IL_0006:  ret
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
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::'<Value5>k__BackingField'
      IL_0006:  ret
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
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::'<Value6>k__BackingField'
      IL_0006:  ret
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
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::get_Text()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Text(string)
    } // end of property Item::Text
    .property instance valuetype [mscorlib]System.Decimal
            Value()
    {
      .get instance valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::get_Value()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value(valuetype [mscorlib]System.Decimal)
    } // end of property Item::Value
    .property instance valuetype [mscorlib]System.Decimal
            Value2()
    {
      .get instance valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::get_Value2()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value2(valuetype [mscorlib]System.Decimal)
    } // end of property Item::Value2
    .property instance string Value3()
    {
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::get_Value3()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value3(string)
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
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::'<Value>k__BackingField'
      IL_0006:  ret
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
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::'<Value2>k__BackingField'
      IL_0006:  ret
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
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::'<Nullable>k__BackingField'
      IL_0006:  ret
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
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::'<Nullable2>k__BackingField'
      IL_0006:  ret
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
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::'<Nullable3>k__BackingField'
      IL_0006:  ret
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
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::'<Nullable4>k__BackingField'
      IL_0006:  ret
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
      .get instance valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Value2()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::set_Value2(valuetype [mscorlib]System.Decimal)
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
      .get instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Nullable2()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::set_Nullable2(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>)
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
    .field private initonly class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem '<Data3>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig specialname 
            instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem 
            get_Data2() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2::'<Data2>k__BackingField'
      IL_0006:  ret
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

    .method public hidebysig specialname 
            instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem 
            get_Data3() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2::'<Data3>k__BackingField'
      IL_0006:  ret
    } // end of method OtherItem2::get_Data3

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
    .property instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem
            Data3()
    {
      .get instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2::get_Data3()
    } // end of property OtherItem2::Data3
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
      // Code size       28 (0x1c)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  stfld      float32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::x
      IL_000d:  ldarg.0
      IL_000e:  ldarg.2
      IL_000f:  stfld      float32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::y
      IL_0014:  ldarg.0
      IL_0015:  ldarg.3
      IL_0016:  stfld      float32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::z
      IL_001b:  ret
    } // end of method V3f::.ctor

  } // end of class V3f

  .class auto ansi serializable sealed nested private beforefieldinit '<>c'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static initonly class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/'<>c' '<>9'
    .field public static class [mscorlib]System.EventHandler '<>9__65_0'
    .field public static class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool> '<>9__79_0'
    .method private hidebysig specialname rtspecialname static 
            void  .cctor() cil managed
    {
      // Code size       11 (0xb)
      .maxstack  8
      IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/'<>c'::.ctor()
      IL_0005:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/'<>c'::'<>9'
      IL_000a:  ret
    } // end of method '<>c'::.cctor

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c'::.ctor

    .method assembly hidebysig instance void 
            '<NotAnObjectInitializerWithEvent>b__65_0'(object '<p0>',
                                                       class [mscorlib]System.EventArgs '<p1>') cil managed
    {
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  call       void [mscorlib]System.Console::WriteLine()
      IL_0005:  ret
    } // end of method '<>c'::'<NotAnObjectInitializerWithEvent>b__65_0'

    .method assembly hidebysig instance bool 
            '<Issue270_NestedInitialisers>b__79_0'(class [mscorlib]System.Globalization.NumberFormatInfo format) cil managed
    {
      // Code size       17 (0x11)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  callvirt   instance string [mscorlib]System.Globalization.NumberFormatInfo::get_CurrencySymbol()
      IL_0006:  ldstr      "$"
      IL_000b:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                     string)
      IL_0010:  ret
    } // end of method '<>c'::'<Issue270_NestedInitialisers>b__79_0'

  } // end of class '<>c'

  .field private static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[] Issue1336_rg0
  .field private static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...] Issue1336_rg1
  .field private static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][] Issue1336_rg1b
  .field private static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...] Issue1336_rg1c
  .field private static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...][] Issue1336_rg1d
  .field private static int32[0...,0...] Issue1336_rg2
  .method private hidebysig static void  X(object a,
                                           object b) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method TestCases::X

  .method private hidebysig static object 
          Y() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ret
  } // end of method TestCases::Y

  .method public hidebysig static void  TestCall(int32 a,
                                                 class [mscorlib]System.Threading.Thread thread) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method TestCases::TestCall

  .method public hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C 
          TestCall(int32 a,
                   class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C c) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ret
  } // end of method TestCases::TestCall

  .method private hidebysig static int32 
          GetInt() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  ret
  } // end of method TestCases::GetInt

  .method private hidebysig static string 
          GetString() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldstr      "Test"
    IL_0005:  ret
  } // end of method TestCases::GetString

  .method private hidebysig static void  NoOp(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Guid>[] 'array') cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method TestCases::NoOp

  .method private hidebysig instance void 
          Data_TestEvent(object sender,
                         class [mscorlib]System.EventArgs e) cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0005:  throw
  } // end of method TestCases::Data_TestEvent

  .method public hidebysig static void  Array1() cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.s   10
    IL_0007:  newarr     [mscorlib]System.Int32
    IL_000c:  dup
    IL_000d:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '<PrivateImplementationDetails>'::E0D2592373A0C161E56E266306CD8405CD719D19
    IL_0012:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0017:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001c:  ret
  } // end of method TestCases::Array1

  .method public hidebysig static void  Array2(int32 a,
                                               int32 b,
                                               int32 c) cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.5
    IL_0006:  newarr     [mscorlib]System.Int32
    IL_000b:  dup
    IL_000c:  ldc.i4.0
    IL_000d:  ldarg.0
    IL_000e:  stelem.i4
    IL_000f:  dup
    IL_0010:  ldc.i4.2
    IL_0011:  ldarg.1
    IL_0012:  stelem.i4
    IL_0013:  dup
    IL_0014:  ldc.i4.4
    IL_0015:  ldarg.2
    IL_0016:  stelem.i4
    IL_0017:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001c:  ret
  } // end of method TestCases::Array2

  .method public hidebysig static void  NestedArray(int32 a,
                                                    int32 b,
                                                    int32 c) cil managed
  {
    // Code size       79 (0x4f)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.3
    IL_0006:  newarr     int32[]
    IL_000b:  dup
    IL_000c:  ldc.i4.0
    IL_000d:  ldc.i4.s   10
    IL_000f:  newarr     [mscorlib]System.Int32
    IL_0014:  dup
    IL_0015:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '<PrivateImplementationDetails>'::E0D2592373A0C161E56E266306CD8405CD719D19
    IL_001a:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_001f:  stelem.ref
    IL_0020:  dup
    IL_0021:  ldc.i4.1
    IL_0022:  ldc.i4.3
    IL_0023:  newarr     [mscorlib]System.Int32
    IL_0028:  dup
    IL_0029:  ldc.i4.0
    IL_002a:  ldarg.0
    IL_002b:  stelem.i4
    IL_002c:  dup
    IL_002d:  ldc.i4.1
    IL_002e:  ldarg.1
    IL_002f:  stelem.i4
    IL_0030:  dup
    IL_0031:  ldc.i4.2
    IL_0032:  ldarg.2
    IL_0033:  stelem.i4
    IL_0034:  stelem.ref
    IL_0035:  dup
    IL_0036:  ldc.i4.2
    IL_0037:  ldc.i4.6
    IL_0038:  newarr     [mscorlib]System.Int32
    IL_003d:  dup
    IL_003e:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=24' '<PrivateImplementationDetails>'::C4E70AB31EF6C8908F896CAD1C6BC75F7FA65E27
    IL_0043:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0048:  stelem.ref
    IL_0049:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_004e:  ret
  } // end of method TestCases::NestedArray

  .method public hidebysig static void  NestedNullableArray(int32 a,
                                                            int32 b,
                                                            int32 c) cil managed
  {
    // Code size       295 (0x127)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.3
    IL_0006:  newarr     valuetype [mscorlib]System.Nullable`1<int32>[]
    IL_000b:  dup
    IL_000c:  ldc.i4.0
    IL_000d:  ldc.i4.s   11
    IL_000f:  newarr     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0014:  dup
    IL_0015:  ldc.i4.0
    IL_0016:  ldc.i4.1
    IL_0017:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_001c:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0021:  dup
    IL_0022:  ldc.i4.1
    IL_0023:  ldc.i4.2
    IL_0024:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0029:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_002e:  dup
    IL_002f:  ldc.i4.2
    IL_0030:  ldc.i4.3
    IL_0031:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0036:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_003b:  dup
    IL_003c:  ldc.i4.3
    IL_003d:  ldc.i4.4
    IL_003e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0043:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0048:  dup
    IL_0049:  ldc.i4.4
    IL_004a:  ldc.i4.5
    IL_004b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0050:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0055:  dup
    IL_0056:  ldc.i4.5
    IL_0057:  ldc.i4.6
    IL_0058:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_005d:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0062:  dup
    IL_0063:  ldc.i4.6
    IL_0064:  ldc.i4.7
    IL_0065:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_006a:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_006f:  dup
    IL_0070:  ldc.i4.7
    IL_0071:  ldc.i4.8
    IL_0072:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0077:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_007c:  dup
    IL_007d:  ldc.i4.8
    IL_007e:  ldc.i4.s   9
    IL_0080:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0085:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_008a:  dup
    IL_008b:  ldc.i4.s   9
    IL_008d:  ldc.i4.s   10
    IL_008f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0094:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0099:  stelem.ref
    IL_009a:  dup
    IL_009b:  ldc.i4.1
    IL_009c:  ldc.i4.4
    IL_009d:  newarr     valuetype [mscorlib]System.Nullable`1<int32>
    IL_00a2:  dup
    IL_00a3:  ldc.i4.0
    IL_00a4:  ldarg.0
    IL_00a5:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00aa:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_00af:  dup
    IL_00b0:  ldc.i4.1
    IL_00b1:  ldarg.1
    IL_00b2:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00b7:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_00bc:  dup
    IL_00bd:  ldc.i4.2
    IL_00be:  ldarg.2
    IL_00bf:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00c4:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_00c9:  stelem.ref
    IL_00ca:  dup
    IL_00cb:  ldc.i4.2
    IL_00cc:  ldc.i4.7
    IL_00cd:  newarr     valuetype [mscorlib]System.Nullable`1<int32>
    IL_00d2:  dup
    IL_00d3:  ldc.i4.0
    IL_00d4:  ldc.i4.1
    IL_00d5:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00da:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_00df:  dup
    IL_00e0:  ldc.i4.1
    IL_00e1:  ldc.i4.2
    IL_00e2:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00e7:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_00ec:  dup
    IL_00ed:  ldc.i4.2
    IL_00ee:  ldc.i4.3
    IL_00ef:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00f4:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_00f9:  dup
    IL_00fa:  ldc.i4.3
    IL_00fb:  ldc.i4.4
    IL_00fc:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0101:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0106:  dup
    IL_0107:  ldc.i4.4
    IL_0108:  ldc.i4.5
    IL_0109:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_010e:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0113:  dup
    IL_0114:  ldc.i4.5
    IL_0115:  ldc.i4.6
    IL_0116:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_011b:  stelem     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0120:  stelem.ref
    IL_0121:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0126:  ret
  } // end of method TestCases::NestedNullableArray

  .method public hidebysig static void  NestedPointerArray(int32 a,
                                                           int32 b,
                                                           int32 c) cil managed
  {
    // Code size       74 (0x4a)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.3
    IL_0006:  newarr     void*[]
    IL_000b:  dup
    IL_000c:  ldc.i4.0
    IL_000d:  ldc.i4.1
    IL_000e:  newarr     void*
    IL_0013:  dup
    IL_0014:  ldc.i4.0
    IL_0015:  ldc.i4.0
    IL_0016:  conv.u
    IL_0017:  stelem.i
    IL_0018:  stelem.ref
    IL_0019:  dup
    IL_001a:  ldc.i4.1
    IL_001b:  ldc.i4.2
    IL_001c:  newarr     void*
    IL_0021:  dup
    IL_0022:  ldc.i4.0
    IL_0023:  ldc.i4     0xc8
    IL_0028:  conv.i
    IL_0029:  stelem.i
    IL_002a:  dup
    IL_002b:  ldc.i4.1
    IL_002c:  ldc.i4.0
    IL_002d:  conv.u
    IL_002e:  stelem.i
    IL_002f:  stelem.ref
    IL_0030:  dup
    IL_0031:  ldc.i4.2
    IL_0032:  ldc.i4.2
    IL_0033:  newarr     void*
    IL_0038:  dup
    IL_0039:  ldc.i4.0
    IL_003a:  ldc.i4.s   100
    IL_003c:  conv.i
    IL_003d:  stelem.i
    IL_003e:  dup
    IL_003f:  ldc.i4.1
    IL_0040:  ldc.i4.0
    IL_0041:  conv.u
    IL_0042:  stelem.i
    IL_0043:  stelem.ref
    IL_0044:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0049:  ret
  } // end of method TestCases::NestedPointerArray

  .method public hidebysig static void  ArrayBoolean() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.8
    IL_0006:  newarr     [mscorlib]System.Boolean
    IL_000b:  dup
    IL_000c:  ldtoken    field int64 '<PrivateImplementationDetails>'::EB0715DBB235F3F696F2C404F5839C6650640898
    IL_0011:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0016:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001b:  ret
  } // end of method TestCases::ArrayBoolean

  .method public hidebysig static void  ArrayByte() cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.s   10
    IL_0007:  newarr     [mscorlib]System.Byte
    IL_000c:  dup
    IL_000d:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=10' '<PrivateImplementationDetails>'::'20E3FF489634E18F3F7EB292AD504DBAE9519293'
    IL_0012:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0017:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001c:  ret
  } // end of method TestCases::ArrayByte

  .method public hidebysig static void  ArraySByte() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.8
    IL_0006:  newarr     [mscorlib]System.SByte
    IL_000b:  dup
    IL_000c:  ldtoken    field int64 '<PrivateImplementationDetails>'::A6296CAC471BE2954899600137940479D8073C7C
    IL_0011:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0016:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001b:  ret
  } // end of method TestCases::ArraySByte

  .method public hidebysig static void  ArrayShort() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.5
    IL_0006:  newarr     [mscorlib]System.Int16
    IL_000b:  dup
    IL_000c:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=10' '<PrivateImplementationDetails>'::'56D9EEC8EF899644C40B9BE9D886DF2367A5D078'
    IL_0011:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0016:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001b:  ret
  } // end of method TestCases::ArrayShort

  .method public hidebysig static void  ArrayUShort() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.6
    IL_0006:  newarr     [mscorlib]System.UInt16
    IL_000b:  dup
    IL_000c:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=12' '<PrivateImplementationDetails>'::'735E5A21849E86F68D220F06163E8C5C6376B9C9'
    IL_0011:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0016:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001b:  ret
  } // end of method TestCases::ArrayUShort

  .method public hidebysig static void  ArrayInt() cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.s   10
    IL_0007:  newarr     [mscorlib]System.Int32
    IL_000c:  dup
    IL_000d:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '<PrivateImplementationDetails>'::F514FF55B79BCAA2CEC9B56C062D976E45F89AB7
    IL_0012:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0017:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001c:  ret
  } // end of method TestCases::ArrayInt

  .method public hidebysig static void  ArrayUInt() cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.s   10
    IL_0007:  newarr     [mscorlib]System.UInt32
    IL_000c:  dup
    IL_000d:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '<PrivateImplementationDetails>'::B9583930B842DBCEF0D7B8E57D4D3F1E8055C39E
    IL_0012:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0017:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001c:  ret
  } // end of method TestCases::ArrayUInt

  .method public hidebysig static void  ArrayLong() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.5
    IL_0006:  newarr     [mscorlib]System.Int64
    IL_000b:  dup
    IL_000c:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '<PrivateImplementationDetails>'::'8D903ECAD8D9D75B3183B23AF79F6D2E607369E3'
    IL_0011:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0016:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001b:  ret
  } // end of method TestCases::ArrayLong

  .method public hidebysig static void  ArrayULong() cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.s   10
    IL_0007:  newarr     [mscorlib]System.UInt64
    IL_000c:  dup
    IL_000d:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=80' '<PrivateImplementationDetails>'::'9B1F6E56D755443CC39C1969CE38FD41FD4EF4B7'
    IL_0012:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0017:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001c:  ret
  } // end of method TestCases::ArrayULong

  .method public hidebysig static void  ArrayFloat() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.6
    IL_0006:  newarr     [mscorlib]System.Single
    IL_000b:  dup
    IL_000c:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=24' '<PrivateImplementationDetails>'::FBCB49C1A244C1B5781AA1DB02C5A11F68908526
    IL_0011:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0016:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001b:  ret
  } // end of method TestCases::ArrayFloat

  .method public hidebysig static void  ArrayDouble() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.6
    IL_0006:  newarr     [mscorlib]System.Double
    IL_000b:  dup
    IL_000c:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=48' '<PrivateImplementationDetails>'::DC7043B0114737ACE19A23DD755893795FD48A23
    IL_0011:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0016:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001b:  ret
  } // end of method TestCases::ArrayDouble

  .method public hidebysig static void  ArrayDecimal() cil managed
  {
    // Code size       96 (0x60)
    .maxstack  9
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.6
    IL_0006:  newarr     [mscorlib]System.Decimal
    IL_000b:  dup
    IL_000c:  ldc.i4.0
    IL_000d:  ldc.i4.s   -100
    IL_000f:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0014:  stelem     [mscorlib]System.Decimal
    IL_0019:  dup
    IL_001a:  ldc.i4.2
    IL_001b:  ldc.i4.s   100
    IL_001d:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0022:  stelem     [mscorlib]System.Decimal
    IL_0027:  dup
    IL_0028:  ldc.i4.3
    IL_0029:  ldc.i4.m1
    IL_002a:  ldc.i4.m1
    IL_002b:  ldc.i4.m1
    IL_002c:  ldc.i4.1
    IL_002d:  ldc.i4.0
    IL_002e:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0033:  stelem     [mscorlib]System.Decimal
    IL_0038:  dup
    IL_0039:  ldc.i4.4
    IL_003a:  ldc.i4.m1
    IL_003b:  ldc.i4.m1
    IL_003c:  ldc.i4.m1
    IL_003d:  ldc.i4.0
    IL_003e:  ldc.i4.0
    IL_003f:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0044:  stelem     [mscorlib]System.Decimal
    IL_0049:  dup
    IL_004a:  ldc.i4.5
    IL_004b:  ldc.i4.1
    IL_004c:  ldc.i4.0
    IL_004d:  ldc.i4.0
    IL_004e:  ldc.i4.0
    IL_004f:  ldc.i4.7
    IL_0050:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0055:  stelem     [mscorlib]System.Decimal
    IL_005a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_005f:  ret
  } // end of method TestCases::ArrayDecimal

  .method public hidebysig static void  ArrayString() cil managed
  {
    // Code size       41 (0x29)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.4
    IL_0006:  newarr     [mscorlib]System.String
    IL_000b:  dup
    IL_000c:  ldc.i4.0
    IL_000d:  ldstr      ""
    IL_0012:  stelem.ref
    IL_0013:  dup
    IL_0014:  ldc.i4.2
    IL_0015:  ldstr      "Hello"
    IL_001a:  stelem.ref
    IL_001b:  dup
    IL_001c:  ldc.i4.3
    IL_001d:  ldstr      "World"
    IL_0022:  stelem.ref
    IL_0023:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0028:  ret
  } // end of method TestCases::ArrayString

  .method public hidebysig static void  ArrayEnum() cil managed
  {
    // Code size       25 (0x19)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.4
    IL_0006:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum
    IL_000b:  dup
    IL_000c:  ldc.i4.1
    IL_000d:  ldc.i4.1
    IL_000e:  stelem.i4
    IL_000f:  dup
    IL_0010:  ldc.i4.3
    IL_0011:  ldc.i4.1
    IL_0012:  stelem.i4
    IL_0013:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0018:  ret
  } // end of method TestCases::ArrayEnum

  .method public hidebysig instance int32[0...,0...] 
          MultidimensionalInit() cil managed
  {
    // Code size       20 (0x14)
    .maxstack  8
    IL_0000:  ldc.i4.s   16
    IL_0002:  ldc.i4.4
    IL_0003:  newobj     instance void int32[0...,0...]::.ctor(int32,
                                                               int32)
    IL_0008:  dup
    IL_0009:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=256' '<PrivateImplementationDetails>'::A1EA7DC3FE43B3A54F5B729A92B92AF54181A3EB
    IL_000e:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0013:  ret
  } // end of method TestCases::MultidimensionalInit

  .method public hidebysig instance int32[0...,0...][] 
          MultidimensionalInit2() cil managed
  {
    // Code size       91 (0x5b)
    .maxstack  6
    IL_0000:  ldc.i4.4
    IL_0001:  newarr     int32[0...,0...]
    IL_0006:  dup
    IL_0007:  ldc.i4.0
    IL_0008:  ldc.i4.4
    IL_0009:  ldc.i4.4
    IL_000a:  newobj     instance void int32[0...,0...]::.ctor(int32,
                                                               int32)
    IL_000f:  dup
    IL_0010:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' '<PrivateImplementationDetails>'::DCF557B883E6FE0AEC05B7F0290F0EF47D0AC2E3
    IL_0015:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_001a:  stelem.ref
    IL_001b:  dup
    IL_001c:  ldc.i4.1
    IL_001d:  ldc.i4.4
    IL_001e:  ldc.i4.4
    IL_001f:  newobj     instance void int32[0...,0...]::.ctor(int32,
                                                               int32)
    IL_0024:  dup
    IL_0025:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' '<PrivateImplementationDetails>'::'7C39B7B06DD624A17F875AB8E9651554BE6E74D2'
    IL_002a:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_002f:  stelem.ref
    IL_0030:  dup
    IL_0031:  ldc.i4.2
    IL_0032:  ldc.i4.4
    IL_0033:  ldc.i4.4
    IL_0034:  newobj     instance void int32[0...,0...]::.ctor(int32,
                                                               int32)
    IL_0039:  dup
    IL_003a:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' '<PrivateImplementationDetails>'::DCF557B883E6FE0AEC05B7F0290F0EF47D0AC2E3
    IL_003f:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0044:  stelem.ref
    IL_0045:  dup
    IL_0046:  ldc.i4.3
    IL_0047:  ldc.i4.4
    IL_0048:  ldc.i4.4
    IL_0049:  newobj     instance void int32[0...,0...]::.ctor(int32,
                                                               int32)
    IL_004e:  dup
    IL_004f:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' '<PrivateImplementationDetails>'::'7C39B7B06DD624A17F875AB8E9651554BE6E74D2'
    IL_0054:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0059:  stelem.ref
    IL_005a:  ret
  } // end of method TestCases::MultidimensionalInit2

  .method public hidebysig instance int32[0...,0...,0...][] 
          ArrayOfArrayOfArrayInit() cil managed
  {
    // Code size       51 (0x33)
    .maxstack  8
    IL_0000:  ldc.i4.2
    IL_0001:  newarr     int32[0...,0...,0...]
    IL_0006:  dup
    IL_0007:  ldc.i4.0
    IL_0008:  ldc.i4.2
    IL_0009:  ldc.i4.3
    IL_000a:  ldc.i4.3
    IL_000b:  newobj     instance void int32[0...,0...,0...]::.ctor(int32,
                                                                    int32,
                                                                    int32)
    IL_0010:  dup
    IL_0011:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=72' '<PrivateImplementationDetails>'::'1535117EC92E41D4A6B7CA00F965357B05B5DC35'
    IL_0016:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_001b:  stelem.ref
    IL_001c:  dup
    IL_001d:  ldc.i4.1
    IL_001e:  ldc.i4.2
    IL_001f:  ldc.i4.3
    IL_0020:  ldc.i4.3
    IL_0021:  newobj     instance void int32[0...,0...,0...]::.ctor(int32,
                                                                    int32,
                                                                    int32)
    IL_0026:  dup
    IL_0027:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=72' '<PrivateImplementationDetails>'::'39E94835525CF7B71CD4595742EF462642FBF1B2'
    IL_002c:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0031:  stelem.ref
    IL_0032:  ret
  } // end of method TestCases::ArrayOfArrayOfArrayInit

  .method public hidebysig static void  RecursiveArrayInitializer() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  4
    .locals init (int32[] V_0)
    IL_0000:  ldc.i4.3
    IL_0001:  newarr     [mscorlib]System.Int32
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.i4.1
    IL_000a:  stelem.i4
    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.1
    IL_000d:  ldc.i4.2
    IL_000e:  stelem.i4
    IL_000f:  ldloc.0
    IL_0010:  ldc.i4.2
    IL_0011:  ldloc.0
    IL_0012:  ldc.i4.1
    IL_0013:  ldelem.i4
    IL_0014:  ldc.i4.1
    IL_0015:  add
    IL_0016:  stelem.i4
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4.0
    IL_0019:  ldc.i4.0
    IL_001a:  stelem.i4
    IL_001b:  ret
  } // end of method TestCases::RecursiveArrayInitializer

  .method public hidebysig static void  InvalidIndices(int32 a) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  3
    .locals init (int32[] V_0)
    IL_0000:  ldc.i4.1
    IL_0001:  newarr     [mscorlib]System.Int32
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  ldarg.0
    IL_000a:  stelem.i4
    IL_000b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0010:  ldloc.0
    IL_0011:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0016:  ret
  } // end of method TestCases::InvalidIndices

  .method public hidebysig static void  InvalidIndices2(int32 a) cil managed
  {
    // Code size       23 (0x17)
    .maxstack  3
    .locals init (int32[] V_0)
    IL_0000:  ldc.i4.1
    IL_0001:  newarr     [mscorlib]System.Int32
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.m1
    IL_0009:  ldarg.0
    IL_000a:  stelem.i4
    IL_000b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0010:  ldloc.0
    IL_0011:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0016:  ret
  } // end of method TestCases::InvalidIndices2

  .method public hidebysig static void  IndicesInWrongOrder(int32 a,
                                                            int32 b) cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  3
    .locals init (int32[] V_0)
    IL_0000:  ldc.i4.5
    IL_0001:  newarr     [mscorlib]System.Int32
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.2
    IL_0009:  ldarg.1
    IL_000a:  stelem.i4
    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.1
    IL_000d:  ldarg.0
    IL_000e:  stelem.i4
    IL_000f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0014:  ldloc.0
    IL_0015:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001a:  ret
  } // end of method TestCases::IndicesInWrongOrder

  .method public hidebysig static uint8[] 
          ReverseInitializer(int32 i) cil managed
  {
    // Code size       35 (0x23)
    .maxstack  8
    IL_0000:  ldc.i4.4
    IL_0001:  newarr     [mscorlib]System.Byte
    IL_0006:  dup
    IL_0007:  ldc.i4.3
    IL_0008:  ldarg.0
    IL_0009:  conv.u1
    IL_000a:  stelem.i1
    IL_000b:  dup
    IL_000c:  ldc.i4.2
    IL_000d:  ldarg.0
    IL_000e:  ldc.i4.8
    IL_000f:  shr
    IL_0010:  conv.u1
    IL_0011:  stelem.i1
    IL_0012:  dup
    IL_0013:  ldc.i4.1
    IL_0014:  ldarg.0
    IL_0015:  ldc.i4.s   16
    IL_0017:  shr
    IL_0018:  conv.u1
    IL_0019:  stelem.i1
    IL_001a:  dup
    IL_001b:  ldc.i4.0
    IL_001c:  ldarg.0
    IL_001d:  ldc.i4.s   24
    IL_001f:  shr
    IL_0020:  conv.u1
    IL_0021:  stelem.i1
    IL_0022:  ret
  } // end of method TestCases::ReverseInitializer

  .method public hidebysig static void  Issue953_MissingNullableSpecifierForArrayInitializer() cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  newarr     valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Guid>
    IL_0006:  dup
    IL_0007:  ldc.i4.0
    IL_0008:  ldsfld     valuetype [mscorlib]System.Guid [mscorlib]System.Guid::Empty
    IL_000d:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Guid>::.ctor(!0)
    IL_0012:  stelem     valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Guid>
    IL_0017:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::NoOp(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Guid>[])
    IL_001c:  ret
  } // end of method TestCases::Issue953_MissingNullableSpecifierForArrayInitializer

  .method private hidebysig instance void 
          Issue907_Test3(string text) cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,object>::.ctor()
    IL_000a:  dup
    IL_000b:  ldstr      ""
    IL_0010:  ldarg.1
    IL_0011:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,object>::Add(!0,
                                                                                                                   !1)
    IL_0016:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001b:  ret
  } // end of method TestCases::Issue907_Test3

  .method private hidebysig instance int32[] 
          Issue1383(int32 i,
                    int32[] 'array') cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  ldc.i4.4
    IL_0001:  newarr     [mscorlib]System.Int32
    IL_0006:  starg.s    'array'
    IL_0008:  ldarg.2
    IL_0009:  ldarg.1
    IL_000a:  dup
    IL_000b:  ldc.i4.1
    IL_000c:  add
    IL_000d:  starg.s    i
    IL_000f:  ldc.i4.1
    IL_0010:  stelem.i4
    IL_0011:  ldarg.2
    IL_0012:  ldarg.1
    IL_0013:  dup
    IL_0014:  ldc.i4.1
    IL_0015:  add
    IL_0016:  starg.s    i
    IL_0018:  ldc.i4.2
    IL_0019:  stelem.i4
    IL_001a:  ldarg.2
    IL_001b:  ret
  } // end of method TestCases::Issue1383

  .method private hidebysig instance string[0...,0...] 
          Issue1382a() cil managed
  {
    // Code size       164 (0xa4)
    .maxstack  5
    IL_0000:  ldc.i4.4
    IL_0001:  ldc.i4.4
    IL_0002:  newobj     instance void string[0...,0...]::.ctor(int32,
                                                                int32)
    IL_0007:  dup
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.i4.1
    IL_000a:  ldstr      "test"
    IL_000f:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0014:  dup
    IL_0015:  ldc.i4.0
    IL_0016:  ldc.i4.2
    IL_0017:  ldstr      "hello"
    IL_001c:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0021:  dup
    IL_0022:  ldc.i4.0
    IL_0023:  ldc.i4.3
    IL_0024:  ldstr      "world"
    IL_0029:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_002e:  dup
    IL_002f:  ldc.i4.1
    IL_0030:  ldc.i4.0
    IL_0031:  ldstr      "test"
    IL_0036:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_003b:  dup
    IL_003c:  ldc.i4.1
    IL_003d:  ldc.i4.2
    IL_003e:  ldstr      "hello"
    IL_0043:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0048:  dup
    IL_0049:  ldc.i4.1
    IL_004a:  ldc.i4.3
    IL_004b:  ldstr      "world"
    IL_0050:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0055:  dup
    IL_0056:  ldc.i4.2
    IL_0057:  ldc.i4.0
    IL_0058:  ldstr      "test"
    IL_005d:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0062:  dup
    IL_0063:  ldc.i4.2
    IL_0064:  ldc.i4.1
    IL_0065:  ldstr      "hello"
    IL_006a:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_006f:  dup
    IL_0070:  ldc.i4.2
    IL_0071:  ldc.i4.3
    IL_0072:  ldstr      "world"
    IL_0077:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_007c:  dup
    IL_007d:  ldc.i4.3
    IL_007e:  ldc.i4.0
    IL_007f:  ldstr      "test"
    IL_0084:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0089:  dup
    IL_008a:  ldc.i4.3
    IL_008b:  ldc.i4.1
    IL_008c:  ldstr      "hello"
    IL_0091:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0096:  dup
    IL_0097:  ldc.i4.3
    IL_0098:  ldc.i4.2
    IL_0099:  ldstr      "world"
    IL_009e:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_00a3:  ret
  } // end of method TestCases::Issue1382a

  .method private hidebysig instance string[0...,0...] 
          Issue1382b() cil managed
  {
    // Code size       164 (0xa4)
    .maxstack  5
    IL_0000:  ldc.i4.4
    IL_0001:  ldc.i4.4
    IL_0002:  newobj     instance void string[0...,0...]::.ctor(int32,
                                                                int32)
    IL_0007:  dup
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.i4.0
    IL_000a:  ldstr      "test"
    IL_000f:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0014:  dup
    IL_0015:  ldc.i4.0
    IL_0016:  ldc.i4.1
    IL_0017:  ldstr      "hello"
    IL_001c:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0021:  dup
    IL_0022:  ldc.i4.0
    IL_0023:  ldc.i4.2
    IL_0024:  ldstr      "world"
    IL_0029:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_002e:  dup
    IL_002f:  ldc.i4.1
    IL_0030:  ldc.i4.0
    IL_0031:  ldstr      "test"
    IL_0036:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_003b:  dup
    IL_003c:  ldc.i4.1
    IL_003d:  ldc.i4.1
    IL_003e:  ldstr      "hello"
    IL_0043:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0048:  dup
    IL_0049:  ldc.i4.1
    IL_004a:  ldc.i4.3
    IL_004b:  ldstr      "world"
    IL_0050:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0055:  dup
    IL_0056:  ldc.i4.2
    IL_0057:  ldc.i4.0
    IL_0058:  ldstr      "test"
    IL_005d:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0062:  dup
    IL_0063:  ldc.i4.2
    IL_0064:  ldc.i4.2
    IL_0065:  ldstr      "hello"
    IL_006a:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_006f:  dup
    IL_0070:  ldc.i4.2
    IL_0071:  ldc.i4.3
    IL_0072:  ldstr      "world"
    IL_0077:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_007c:  dup
    IL_007d:  ldc.i4.3
    IL_007e:  ldc.i4.1
    IL_007f:  ldstr      "test"
    IL_0084:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0089:  dup
    IL_008a:  ldc.i4.3
    IL_008b:  ldc.i4.2
    IL_008c:  ldstr      "hello"
    IL_0091:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0096:  dup
    IL_0097:  ldc.i4.3
    IL_0098:  ldc.i4.3
    IL_0099:  ldstr      "world"
    IL_009e:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_00a3:  ret
  } // end of method TestCases::Issue1382b

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C 
          Test1() cil managed
  {
    // Code size       34 (0x22)
    .maxstack  8
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_0005:  dup
    IL_0006:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S>::.ctor()
    IL_000b:  stfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::L
    IL_0010:  dup
    IL_0011:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::L
    IL_0016:  ldc.i4.1
    IL_0017:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::.ctor(int32)
    IL_001c:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S>::Add(!0)
    IL_0021:  ret
  } // end of method TestCases::Test1

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C 
          Test1Alternative() cil managed
  {
    // Code size       35 (0x23)
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_0006:  dup
    IL_0007:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S>::.ctor()
    IL_000c:  dup
    IL_000d:  ldc.i4.1
    IL_000e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::.ctor(int32)
    IL_0013:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S>::Add(!0)
    IL_0018:  stfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::L
    IL_001d:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::TestCall(int32,
                                                                                                                                                                                             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C)
    IL_0022:  ret
  } // end of method TestCases::Test1Alternative

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C 
          Test2() cil managed
  {
    // Code size       20 (0x14)
    .maxstack  8
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_0005:  dup
    IL_0006:  ldc.i4.1
    IL_0007:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Z
    IL_000c:  dup
    IL_000d:  ldc.i4.2
    IL_000e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Z
    IL_0013:  ret
  } // end of method TestCases::Test2

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C 
          Test3() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  8
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_0005:  dup
    IL_0006:  ldc.i4.1
    IL_0007:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::.ctor(int32)
    IL_000c:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Y
    IL_0011:  dup
    IL_0012:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Y
    IL_0017:  ldc.i4.2
    IL_0018:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::A
    IL_001d:  ret
  } // end of method TestCases::Test3

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C 
          Test3b() cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_0006:  dup
    IL_0007:  ldc.i4.1
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Z
    IL_000d:  dup
    IL_000e:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Y
    IL_0013:  ldc.i4.2
    IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::A
    IL_0019:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::TestCall(int32,
                                                                                                                                                                                             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C)
    IL_001e:  ret
  } // end of method TestCases::Test3b

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C 
          Test4() cil managed
  {
    // Code size       37 (0x25)
    .maxstack  8
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_0005:  dup
    IL_0006:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Y
    IL_000b:  ldc.i4.1
    IL_000c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::A
    IL_0011:  dup
    IL_0012:  ldc.i4.2
    IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Z
    IL_0018:  dup
    IL_0019:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Y
    IL_001e:  ldc.i4.3
    IL_001f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::B
    IL_0024:  ret
  } // end of method TestCases::Test4

  .method public hidebysig static void  ObjectInitializer() cil managed
  {
    // Code size       23 (0x17)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000a:  dup
    IL_000b:  ldc.i4.0
    IL_000c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0011:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0016:  ret
  } // end of method TestCases::ObjectInitializer

  .method public hidebysig static void  NotAnObjectInitializer() cil managed
  {
    // Code size       25 (0x19)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.0
    IL_0008:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_000d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0012:  ldloc.0
    IL_0013:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0018:  ret
  } // end of method TestCases::NotAnObjectInitializer

  .method public hidebysig static void  NotAnObjectInitializerWithEvent() cil managed
  {
    // Code size       55 (0x37)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldsfld     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/'<>c'::'<>9__65_0'
    IL_000c:  dup
    IL_000d:  brtrue.s   IL_0026

    IL_000f:  pop
    IL_0010:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/'<>c'::'<>9'
    IL_0015:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/'<>c'::'<NotAnObjectInitializerWithEvent>b__65_0'(object,
                                                                                                                                                                  class [mscorlib]System.EventArgs)
    IL_001b:  newobj     instance void [mscorlib]System.EventHandler::.ctor(object,
                                                                            native int)
    IL_0020:  dup
    IL_0021:  stsfld     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/'<>c'::'<>9__65_0'
    IL_0026:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::add_TestEvent(class [mscorlib]System.EventHandler)
    IL_002b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0030:  ldloc.0
    IL_0031:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0036:  ret
  } // end of method TestCases::NotAnObjectInitializerWithEvent

  .method public hidebysig static void  ObjectInitializerAssignCollectionToField() cil managed
  {
    // Code size       48 (0x30)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000a:  dup
    IL_000b:  ldc.i4.0
    IL_000c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0011:  dup
    IL_0012:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::.ctor()
    IL_0017:  dup
    IL_0018:  ldc.i4.0
    IL_0019:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_001e:  dup
    IL_001f:  ldc.i4.1
    IL_0020:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_0025:  stfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::FieldList
    IL_002a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_002f:  ret
  } // end of method TestCases::ObjectInitializerAssignCollectionToField

  .method public hidebysig static void  ObjectInitializerAddToCollectionInField() cil managed
  {
    // Code size       47 (0x2f)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000a:  dup
    IL_000b:  ldc.i4.0
    IL_000c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0011:  dup
    IL_0012:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::FieldList
    IL_0017:  ldc.i4.0
    IL_0018:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_001d:  dup
    IL_001e:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::FieldList
    IL_0023:  ldc.i4.1
    IL_0024:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_0029:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_002e:  ret
  } // end of method TestCases::ObjectInitializerAddToCollectionInField

  .method public hidebysig static void  ObjectInitializerAssignCollectionToProperty() cil managed
  {
    // Code size       48 (0x30)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000a:  dup
    IL_000b:  ldc.i4.0
    IL_000c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0011:  dup
    IL_0012:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::.ctor()
    IL_0017:  dup
    IL_0018:  ldc.i4.0
    IL_0019:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_001e:  dup
    IL_001f:  ldc.i4.1
    IL_0020:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_0025:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_PropertyList(class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>)
    IL_002a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_002f:  ret
  } // end of method TestCases::ObjectInitializerAssignCollectionToProperty

  .method public hidebysig static void  ObjectInitializerAddToCollectionInProperty() cil managed
  {
    // Code size       47 (0x2f)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000a:  dup
    IL_000b:  ldc.i4.0
    IL_000c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0011:  dup
    IL_0012:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_PropertyList()
    IL_0017:  ldc.i4.0
    IL_0018:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_001d:  dup
    IL_001e:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_PropertyList()
    IL_0023:  ldc.i4.1
    IL_0024:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_0029:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_002e:  ret
  } // end of method TestCases::ObjectInitializerAddToCollectionInProperty

  .method public hidebysig static void  ObjectInitializerWithInitializationOfNestedObjects() cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000a:  dup
    IL_000b:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0010:  ldc.i4.0
    IL_0011:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0016:  dup
    IL_0017:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_001c:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0021:  ldc.i4.1
    IL_0022:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0027:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_002c:  ret
  } // end of method TestCases::ObjectInitializerWithInitializationOfNestedObjects

  .method public hidebysig static void  ObjectInitializerWithInitializationOfDeeplyNestedObjects() cil managed
  {
    // Code size       77 (0x4d)
    .maxstack  4
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000a:  dup
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0011:  dup
    IL_0012:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0017:  ldc.i4.0
    IL_0018:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_001d:  dup
    IL_001e:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0023:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0028:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_002d:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0032:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0037:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_003c:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0041:  ldc.i4.1
    IL_0042:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0047:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_004c:  ret
  } // end of method TestCases::ObjectInitializerWithInitializationOfDeeplyNestedObjects

  .method public hidebysig static void  CollectionInitializerInsideObjectInitializers() cil managed
  {
    // Code size       53 (0x35)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000a:  dup
    IL_000b:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_0010:  dup
    IL_0011:  ldc.i4.0
    IL_0012:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0017:  dup
    IL_0018:  ldc.i4.1
    IL_0019:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_b(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_001e:  dup
    IL_001f:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_PropertyList()
    IL_0024:  ldc.i4.0
    IL_0025:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_002a:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_MoreData(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data)
    IL_002f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0034:  ret
  } // end of method TestCases::CollectionInitializerInsideObjectInitializers

  .method public hidebysig static void  NotAStructInitializer_DefaultConstructor() cil managed
  {
    // Code size       41 (0x29)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData V_0)
    IL_0000:  ldloca.s   V_0
    IL_0002:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData
    IL_0008:  ldloca.s   V_0
    IL_000a:  ldc.i4.1
    IL_000b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::Field
    IL_0010:  ldloca.s   V_0
    IL_0012:  ldc.i4.2
    IL_0013:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::set_Property(int32)
    IL_0018:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_001d:  ldloc.0
    IL_001e:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData
    IL_0023:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0028:  ret
  } // end of method TestCases::NotAStructInitializer_DefaultConstructor

  .method public hidebysig static void  StructInitializer_DefaultConstructor() cil managed
  {
    // Code size       41 (0x29)
    .maxstack  3
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldloca.s   V_0
    IL_0007:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData
    IL_000d:  ldloca.s   V_0
    IL_000f:  ldc.i4.1
    IL_0010:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::Field
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldc.i4.2
    IL_0018:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::set_Property(int32)
    IL_001d:  ldloc.0
    IL_001e:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData
    IL_0023:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0028:  ret
  } // end of method TestCases::StructInitializer_DefaultConstructor

  .method public hidebysig static void  NotAStructInitializer_ExplicitConstructor() cil managed
  {
    // Code size       41 (0x29)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData V_0)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldc.i4.0
    IL_0003:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::.ctor(int32)
    IL_0008:  ldloca.s   V_0
    IL_000a:  ldc.i4.1
    IL_000b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::Field
    IL_0010:  ldloca.s   V_0
    IL_0012:  ldc.i4.2
    IL_0013:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::set_Property(int32)
    IL_0018:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_001d:  ldloc.0
    IL_001e:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData
    IL_0023:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0028:  ret
  } // end of method TestCases::NotAStructInitializer_ExplicitConstructor

  .method public hidebysig static void  StructInitializer_ExplicitConstructor() cil managed
  {
    // Code size       41 (0x29)
    .maxstack  3
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldloca.s   V_0
    IL_0007:  ldc.i4.0
    IL_0008:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::.ctor(int32)
    IL_000d:  ldloca.s   V_0
    IL_000f:  ldc.i4.1
    IL_0010:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::Field
    IL_0015:  ldloca.s   V_0
    IL_0017:  ldc.i4.2
    IL_0018:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::set_Property(int32)
    IL_001d:  ldloc.0
    IL_001e:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData
    IL_0023:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0028:  ret
  } // end of method TestCases::StructInitializer_ExplicitConstructor

  .method public hidebysig static void  StructInitializerWithInitializationOfNestedObjects() cil managed
  {
    // Code size       74 (0x4a)
    .maxstack  3
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldloca.s   V_0
    IL_0007:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData
    IL_000d:  ldloca.s   V_0
    IL_000f:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::get_MoreData()
    IL_0014:  ldc.i4.0
    IL_0015:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_001a:  ldloca.s   V_0
    IL_001c:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::get_MoreData()
    IL_0021:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::FieldList
    IL_0026:  ldc.i4.0
    IL_0027:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_002c:  ldloca.s   V_0
    IL_002e:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::get_MoreData()
    IL_0033:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::FieldList
    IL_0038:  ldc.i4.1
    IL_0039:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_003e:  ldloc.0
    IL_003f:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData
    IL_0044:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0049:  ret
  } // end of method TestCases::StructInitializerWithInitializationOfNestedObjects

  .method public hidebysig static void  StructInitializerWithinObjectInitializer() cil managed
  {
    // Code size       47 (0x2f)
    .maxstack  5
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000a:  dup
    IL_000b:  ldloca.s   V_0
    IL_000d:  ldc.i4.2
    IL_000e:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::.ctor(int32)
    IL_0013:  ldloca.s   V_0
    IL_0015:  ldc.i4.1
    IL_0016:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::Field
    IL_001b:  ldloca.s   V_0
    IL_001d:  ldc.i4.2
    IL_001e:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::set_Property(int32)
    IL_0023:  ldloc.0
    IL_0024:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_NestedStruct(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData)
    IL_0029:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_002e:  ret
  } // end of method TestCases::StructInitializerWithinObjectInitializer

  .method public hidebysig static void  Issue270_NestedInitialisers() cil managed
  {
    // Code size       115 (0x73)
    .maxstack  8
    .locals init (class [mscorlib]System.Globalization.NumberFormatInfo[] V_0)
    IL_0000:  ldnull
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  ldnull
    IL_0004:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Issue270_NestedInitialisers()
    IL_000a:  newobj     instance void [mscorlib]System.Threading.ThreadStart::.ctor(object,
                                                                                     native int)
    IL_000f:  newobj     instance void [mscorlib]System.Threading.Thread::.ctor(class [mscorlib]System.Threading.ThreadStart)
    IL_0014:  dup
    IL_0015:  ldc.i4.1
    IL_0016:  callvirt   instance void [mscorlib]System.Threading.Thread::set_Priority(valuetype [mscorlib]System.Threading.ThreadPriority)
    IL_001b:  dup
    IL_001c:  ldc.i4.0
    IL_001d:  newobj     instance void [mscorlib]System.Globalization.CultureInfo::.ctor(int32)
    IL_0022:  dup
    IL_0023:  newobj     instance void [mscorlib]System.Globalization.DateTimeFormatInfo::.ctor()
    IL_0028:  dup
    IL_0029:  ldstr      "ddmmyy"
    IL_002e:  callvirt   instance void [mscorlib]System.Globalization.DateTimeFormatInfo::set_ShortDatePattern(string)
    IL_0033:  callvirt   instance void [mscorlib]System.Globalization.CultureInfo::set_DateTimeFormat(class [mscorlib]System.Globalization.DateTimeFormatInfo)
    IL_0038:  dup
    IL_0039:  ldloc.0
    IL_003a:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/'<>c'::'<>9__79_0'
    IL_003f:  dup
    IL_0040:  brtrue.s   IL_0059

    IL_0042:  pop
    IL_0043:  ldsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/'<>c' ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/'<>c'::'<>9'
    IL_0048:  ldftn      instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/'<>c'::'<Issue270_NestedInitialisers>b__79_0'(class [mscorlib]System.Globalization.NumberFormatInfo)
    IL_004e:  newobj     instance void class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool>::.ctor(object,
                                                                                                                                        native int)
    IL_0053:  dup
    IL_0054:  stsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/'<>c'::'<>9__79_0'
    IL_0059:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class [mscorlib]System.Globalization.NumberFormatInfo>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                         class [mscorlib]System.Func`2<!!0,bool>)
    IL_005e:  call       !!0 [System.Core]System.Linq.Enumerable::First<class [mscorlib]System.Globalization.NumberFormatInfo>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0063:  callvirt   instance void [mscorlib]System.Globalization.CultureInfo::set_NumberFormat(class [mscorlib]System.Globalization.NumberFormatInfo)
    IL_0068:  callvirt   instance void [mscorlib]System.Threading.Thread::set_CurrentCulture(class [mscorlib]System.Globalization.CultureInfo)
    IL_006d:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::TestCall(int32,
                                                                                                                 class [mscorlib]System.Threading.Thread)
    IL_0072:  ret
  } // end of method TestCases::Issue270_NestedInitialisers

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2 
          Issue1345() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2::.ctor()
    IL_0005:  dup
    IL_0006:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2::Data
    IL_000b:  ldc.i4.3
    IL_000c:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0011:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0016:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::set_Nullable(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>)
    IL_001b:  ret
  } // end of method TestCases::Issue1345

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2 
          Issue1345b() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2::.ctor()
    IL_0005:  dup
    IL_0006:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2::get_Data2()
    IL_000b:  ldc.i4.3
    IL_000c:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0011:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0016:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::set_Nullable(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>)
    IL_001b:  ret
  } // end of method TestCases::Issue1345b

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2 
          Issue1345c() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2::.ctor()
    IL_0005:  dup
    IL_0006:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2::get_Data3()
    IL_000b:  ldc.i4.3
    IL_000c:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0011:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0016:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::set_Nullable(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>)
    IL_001b:  ret
  } // end of method TestCases::Issue1345c

  .method private hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data 
          Issue1345_FalsePositive() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  8
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_0005:  dup
    IL_0006:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_ReadOnlyPropertyList()
    IL_000b:  ldc.i4.0
    IL_000c:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_0011:  dup
    IL_0012:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_ReadOnlyPropertyList()
    IL_0017:  ldc.i4.1
    IL_0018:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_001d:  ret
  } // end of method TestCases::Issue1345_FalsePositive

  .method private hidebysig instance void 
          Issue1250_Test1(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum 'value') cil managed
  {
    // Code size       23 (0x17)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_000a:  dup
    IL_000b:  ldarg.1
    IL_000c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Z
    IL_0011:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0016:  ret
  } // end of method TestCases::Issue1250_Test1

  .method private hidebysig instance uint8[] 
          Issue1314() cil managed
  {
    // Code size       18 (0x12)
    .maxstack  8
    IL_0000:  ldc.i4.4
    IL_0001:  newarr     [mscorlib]System.Byte
    IL_0006:  dup
    IL_0007:  ldtoken    field int32 '<PrivateImplementationDetails>'::C62C27924F4C967F5EDDB1850C091D54C7A2AB58
    IL_000c:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0011:  ret
  } // end of method TestCases::Issue1314

  .method private hidebysig instance void 
          Issue1251_Test(class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item> list,
                         class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem otherItem) cil managed
  {
    // Code size       151 (0x97)
    .maxstack  4
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_0)
    IL_0000:  ldarg.1
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::.ctor()
    IL_0006:  dup
    IL_0007:  ldstr      "Text"
    IL_000c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Text(string)
    IL_0011:  dup
    IL_0012:  ldarg.2
    IL_0013:  callvirt   instance valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Value()
    IL_0018:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value(valuetype [mscorlib]System.Decimal)
    IL_001d:  dup
    IL_001e:  ldarg.2
    IL_001f:  callvirt   instance valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Value2()
    IL_0024:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value2(valuetype [mscorlib]System.Decimal)
    IL_0029:  dup
    IL_002a:  ldarg.2
    IL_002b:  callvirt   instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Nullable()
    IL_0030:  stloc.0
    IL_0031:  ldloca.s   V_0
    IL_0033:  constrained. valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0039:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_003e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value3(string)
    IL_0043:  dup
    IL_0044:  ldarg.2
    IL_0045:  callvirt   instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Nullable2()
    IL_004a:  stloc.0
    IL_004b:  ldloca.s   V_0
    IL_004d:  constrained. valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0053:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0058:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value4(string)
    IL_005d:  dup
    IL_005e:  ldarg.2
    IL_005f:  callvirt   instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Nullable3()
    IL_0064:  stloc.0
    IL_0065:  ldloca.s   V_0
    IL_0067:  constrained. valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_006d:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0072:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value5(string)
    IL_0077:  dup
    IL_0078:  ldarg.2
    IL_0079:  callvirt   instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Nullable4()
    IL_007e:  stloc.0
    IL_007f:  ldloca.s   V_0
    IL_0081:  constrained. valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0087:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_008c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value6(string)
    IL_0091:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item>::Add(!0)
    IL_0096:  ret
  } // end of method TestCases::Issue1251_Test

  .method private hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data 
          Issue1279(int32 p) cil managed
  {
    // Code size       37 (0x25)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.1
    IL_0002:  bne.un.s   IL_0023

    IL_0004:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_0009:  dup
    IL_000a:  ldc.i4.0
    IL_000b:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0010:  dup
    IL_0011:  ldarg.0
    IL_0012:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Data_TestEvent(object,
                                                                                                                                class [mscorlib]System.EventArgs)
    IL_0018:  newobj     instance void [mscorlib]System.EventHandler::.ctor(object,
                                                                            native int)
    IL_001d:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::add_TestEvent(class [mscorlib]System.EventHandler)
    IL_0022:  ret

    IL_0023:  ldnull
    IL_0024:  ret
  } // end of method TestCases::Issue1279

  .method public hidebysig static void  ExtensionMethodInCollectionInitializer() cil managed
  {
    // Code size       32 (0x20)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32>::.ctor()
    IL_000a:  dup
    IL_000b:  ldstr      "1"
    IL_0010:  ldstr      "2"
    IL_0015:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.Extensions::Add(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32>,
                                                                                                             string,
                                                                                                             string)
    IL_001a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001f:  ret
  } // end of method TestCases::ExtensionMethodInCollectionInitializer

  .method public hidebysig static void  NoCollectionInitializerBecauseOfTypeArguments() cil managed
  {
    // Code size       22 (0x16)
    .maxstack  8
    IL_0000:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32>::.ctor()
    IL_0005:  dup
    IL_0006:  ldstr      "int"
    IL_000b:  callvirt   instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32>::Add<int32>(string)
    IL_0010:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0015:  ret
  } // end of method TestCases::NoCollectionInitializerBecauseOfTypeArguments

  .method public hidebysig static void  CollectionInitializerWithParamsMethod() cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  5
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32> V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32>::.ctor()
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.s   10
    IL_000e:  newarr     [mscorlib]System.Int32
    IL_0013:  dup
    IL_0014:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '<PrivateImplementationDetails>'::E0D2592373A0C161E56E266306CD8405CD719D19
    IL_0019:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_001e:  callvirt   instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32>::Add(int32[])
    IL_0023:  ldloc.0
    IL_0024:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0029:  ret
  } // end of method TestCases::CollectionInitializerWithParamsMethod

  .method public hidebysig static void  CollectionInitializerList() cil managed
  {
    // Code size       37 (0x25)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
    IL_000a:  dup
    IL_000b:  ldc.i4.1
    IL_000c:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0011:  dup
    IL_0012:  ldc.i4.2
    IL_0013:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0018:  dup
    IL_0019:  ldc.i4.3
    IL_001a:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_001f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0024:  ret
  } // end of method TestCases::CollectionInitializerList

  .method public hidebysig static object 
          RecursiveCollectionInitializer() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  8
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<object>::.ctor()
    IL_0005:  dup
    IL_0006:  dup
    IL_0007:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<object>::Add(!0)
    IL_000c:  ret
  } // end of method TestCases::RecursiveCollectionInitializer

  .method public hidebysig static void  CollectionInitializerDictionary() cil managed
  {
    // Code size       52 (0x34)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::.ctor()
    IL_000a:  dup
    IL_000b:  ldstr      "First"
    IL_0010:  ldc.i4.1
    IL_0011:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0016:  dup
    IL_0017:  ldstr      "Second"
    IL_001c:  ldc.i4.2
    IL_001d:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0022:  dup
    IL_0023:  ldstr      "Third"
    IL_0028:  ldc.i4.3
    IL_0029:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_002e:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0033:  ret
  } // end of method TestCases::CollectionInitializerDictionary

  .method public hidebysig static void  CollectionInitializerDictionaryWithEnumTypes() cil managed
  {
    // Code size       32 (0x20)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::.ctor()
    IL_000a:  dup
    IL_000b:  ldc.i4.0
    IL_000c:  ldc.i4.0
    IL_000d:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0,
                                                                                                                                                                                                                                                                                          !1)
    IL_0012:  dup
    IL_0013:  ldc.i4.1
    IL_0014:  ldc.i4.1
    IL_0015:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0,
                                                                                                                                                                                                                                                                                          !1)
    IL_001a:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001f:  ret
  } // end of method TestCases::CollectionInitializerDictionaryWithEnumTypes

  .method public hidebysig static void  NotACollectionInitializer() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.Generic.List`1<int32> V_0)
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.2
    IL_000f:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.3
    IL_0016:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_001b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0020:  ldloc.0
    IL_0021:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0026:  ret
  } // end of method TestCases::NotACollectionInitializer

  .method public hidebysig static void  SimpleDictInitializer() cil managed
  {
    // Code size       41 (0x29)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000a:  dup
    IL_000b:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0010:  ldc.i4.0
    IL_0011:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0016:  dup
    IL_0017:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_001c:  ldc.i4.2
    IL_001d:  ldnull
    IL_001e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_Item(int32,
                                                                                                                               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data)
    IL_0023:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0028:  ret
  } // end of method TestCases::SimpleDictInitializer

  .method public hidebysig static void  MixedObjectAndDictInitializer() cil managed
  {
    // Code size       130 (0x82)
    .maxstack  6
    .locals init (int32 V_0,
             int32 V_1,
             string V_2)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000a:  dup
    IL_000b:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0010:  ldc.i4.0
    IL_0011:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0016:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::GetInt()
    IL_001b:  stloc.0
    IL_001c:  dup
    IL_001d:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0022:  ldloc.0
    IL_0023:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_Item(int32)
    IL_0028:  ldc.i4.1
    IL_0029:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_002e:  dup
    IL_002f:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0034:  ldloc.0
    IL_0035:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_Item(int32)
    IL_003a:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::FieldList
    IL_003f:  ldc.i4.0
    IL_0040:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_0045:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::GetInt()
    IL_004a:  stloc.1
    IL_004b:  call       string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::GetString()
    IL_0050:  stloc.2
    IL_0051:  dup
    IL_0052:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0057:  ldloc.0
    IL_0058:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_Item(int32)
    IL_005d:  ldloc.1
    IL_005e:  ldloc.2
    IL_005f:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_0064:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_Item(int32,
                                                                                                                               string,
                                                                                                                               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data)
    IL_0069:  dup
    IL_006a:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_006f:  ldloc.0
    IL_0070:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_Item(int32)
    IL_0075:  ldc.i4.2
    IL_0076:  ldnull
    IL_0077:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_Item(int32,
                                                                                                                               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data)
    IL_007c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0081:  ret
  } // end of method TestCases::MixedObjectAndDictInitializer

  .method private hidebysig instance void 
          NestedListWithIndexInitializer(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum myEnum) cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  8
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Collections.Generic.List`1<int32>>::.ctor()
    IL_0005:  dup
    IL_0006:  ldc.i4.0
    IL_0007:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Collections.Generic.List`1<int32>>::get_Item(int32)
    IL_000c:  ldc.i4.1
    IL_000d:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0012:  dup
    IL_0013:  ldc.i4.0
    IL_0014:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Collections.Generic.List`1<int32>>::get_Item(int32)
    IL_0019:  ldc.i4.2
    IL_001a:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_001f:  dup
    IL_0020:  ldc.i4.0
    IL_0021:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Collections.Generic.List`1<int32>>::get_Item(int32)
    IL_0026:  ldc.i4.3
    IL_0027:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_002c:  dup
    IL_002d:  ldc.i4.1
    IL_002e:  callvirt   instance !0 class [mscorlib]System.Collections.Generic.List`1<class [mscorlib]System.Collections.Generic.List`1<int32>>::get_Item(int32)
    IL_0033:  ldarg.1
    IL_0034:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0039:  pop
    IL_003a:  ret
  } // end of method TestCases::NestedListWithIndexInitializer

  .method private hidebysig instance void 
          Issue1250_Test2(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum 'value') cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  5
    .locals init (int32 V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_000a:  ldarg.1
    IL_000b:  stloc.0
    IL_000c:  dup
    IL_000d:  ldloc.0
    IL_000e:  ldarg.1
    IL_000f:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::.ctor(int32)
    IL_0014:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::set_Item(int32,
                                                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S)
    IL_0019:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001e:  ret
  } // end of method TestCases::Issue1250_Test2

  .method private hidebysig instance void 
          Issue1250_Test3(int32 'value') cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  5
    .locals init (int32 V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_000a:  ldarg.1
    IL_000b:  stloc.0
    IL_000c:  dup
    IL_000d:  ldloc.0
    IL_000e:  ldarg.1
    IL_000f:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::.ctor(int32)
    IL_0014:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::set_Item(int32,
                                                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S)
    IL_0019:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001e:  ret
  } // end of method TestCases::Issue1250_Test3

  .method private hidebysig instance void 
          Issue1250_Test4(int32 'value') cil managed
  {
    // Code size       36 (0x24)
    .maxstack  5
    .locals init (object V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_000a:  ldarg.1
    IL_000b:  box        [mscorlib]System.Int32
    IL_0010:  stloc.0
    IL_0011:  dup
    IL_0012:  ldloc.0
    IL_0013:  ldarg.1
    IL_0014:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::.ctor(int32)
    IL_0019:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::set_Item(object,
                                                                                                                            valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S)
    IL_001e:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0023:  ret
  } // end of method TestCases::Issue1250_Test4

  .method public hidebysig static void  Issue1390(class [mscorlib]System.Collections.Generic.IEnumerable`1<string> tokens,
                                                  bool alwaysAllowAdministrators,
                                                  char wireDelimiter) cil managed
  {
    // Code size       70 (0x46)
    .maxstack  5
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<string,string>>::.ctor()
    IL_0005:  dup
    IL_0006:  ldstr      "tokens"
    IL_000b:  ldarga.s   wireDelimiter
    IL_000d:  call       instance string [mscorlib]System.Char::ToString()
    IL_0012:  ldarg.0
    IL_0013:  call       string [mscorlib]System.String::Join(string,
                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<string>)
    IL_0018:  ldnull
    IL_0019:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.Extensions::Add<string>(class [mscorlib]System.Collections.Generic.IList`1<valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<string,string>>,
                                                                                                                     string,
                                                                                                                     !!0,
                                                                                                                     class [mscorlib]System.Func`2<!!0,string>)
    IL_001e:  dup
    IL_001f:  ldstr      "alwaysAllowAdministrators"
    IL_0024:  ldarga.s   alwaysAllowAdministrators
    IL_0026:  call       instance string [mscorlib]System.Boolean::ToString()
    IL_002b:  ldnull
    IL_002c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.Extensions::Add<string>(class [mscorlib]System.Collections.Generic.IList`1<valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<string,string>>,
                                                                                                                     string,
                                                                                                                     !!0,
                                                                                                                     class [mscorlib]System.Func`2<!!0,string>)
    IL_0031:  dup
    IL_0032:  ldstr      "delimiter"
    IL_0037:  ldarga.s   wireDelimiter
    IL_0039:  call       instance string [mscorlib]System.Char::ToString()
    IL_003e:  ldnull
    IL_003f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.Extensions::Add<string>(class [mscorlib]System.Collections.Generic.IList`1<valuetype [mscorlib]System.Collections.Generic.KeyValuePair`2<string,string>>,
                                                                                                                     string,
                                                                                                                     !!0,
                                                                                                                     class [mscorlib]System.Func`2<!!0,string>)
    IL_0044:  pop
    IL_0045:  ret
  } // end of method TestCases::Issue1390

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method TestCases::.ctor

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       1907 (0x773)
    .maxstack  10
    IL_0000:  ldc.i4.3
    IL_0001:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_0006:  dup
    IL_0007:  ldc.i4.0
    IL_0008:  ldc.r4     1.
    IL_000d:  ldc.r4     1.
    IL_0012:  ldc.r4     1.
    IL_0017:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_001c:  stelem.ref
    IL_001d:  dup
    IL_001e:  ldc.i4.1
    IL_001f:  ldc.r4     2.
    IL_0024:  ldc.r4     2.
    IL_0029:  ldc.r4     2.
    IL_002e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0033:  stelem.ref
    IL_0034:  dup
    IL_0035:  ldc.i4.2
    IL_0036:  ldc.r4     3.
    IL_003b:  ldc.r4     3.
    IL_0040:  ldc.r4     3.
    IL_0045:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_004a:  stelem.ref
    IL_004b:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Issue1336_rg0
    IL_0050:  ldc.i4.3
    IL_0051:  ldc.i4.3
    IL_0052:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::.ctor(int32,
                                                                                                                                            int32)
    IL_0057:  dup
    IL_0058:  ldc.i4.0
    IL_0059:  ldc.i4.0
    IL_005a:  ldc.r4     1.
    IL_005f:  ldc.r4     1.
    IL_0064:  ldc.r4     1.
    IL_0069:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_006e:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0073:  dup
    IL_0074:  ldc.i4.0
    IL_0075:  ldc.i4.1
    IL_0076:  ldc.r4     2.
    IL_007b:  ldc.r4     2.
    IL_0080:  ldc.r4     2.
    IL_0085:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_008a:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_008f:  dup
    IL_0090:  ldc.i4.0
    IL_0091:  ldc.i4.2
    IL_0092:  ldc.r4     3.
    IL_0097:  ldc.r4     3.
    IL_009c:  ldc.r4     3.
    IL_00a1:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_00a6:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_00ab:  dup
    IL_00ac:  ldc.i4.1
    IL_00ad:  ldc.i4.0
    IL_00ae:  ldc.r4     2.
    IL_00b3:  ldc.r4     2.
    IL_00b8:  ldc.r4     2.
    IL_00bd:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_00c2:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_00c7:  dup
    IL_00c8:  ldc.i4.1
    IL_00c9:  ldc.i4.1
    IL_00ca:  ldc.r4     3.
    IL_00cf:  ldc.r4     3.
    IL_00d4:  ldc.r4     3.
    IL_00d9:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_00de:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_00e3:  dup
    IL_00e4:  ldc.i4.1
    IL_00e5:  ldc.i4.2
    IL_00e6:  ldc.r4     4.
    IL_00eb:  ldc.r4     4.
    IL_00f0:  ldc.r4     4.
    IL_00f5:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_00fa:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_00ff:  dup
    IL_0100:  ldc.i4.2
    IL_0101:  ldc.i4.0
    IL_0102:  ldc.r4     3.
    IL_0107:  ldc.r4     3.
    IL_010c:  ldc.r4     3.
    IL_0111:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0116:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_011b:  dup
    IL_011c:  ldc.i4.2
    IL_011d:  ldc.i4.1
    IL_011e:  ldc.r4     4.
    IL_0123:  ldc.r4     4.
    IL_0128:  ldc.r4     4.
    IL_012d:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0132:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0137:  dup
    IL_0138:  ldc.i4.2
    IL_0139:  ldc.i4.2
    IL_013a:  ldc.r4     5.
    IL_013f:  ldc.r4     5.
    IL_0144:  ldc.r4     5.
    IL_0149:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_014e:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0153:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Issue1336_rg1
    IL_0158:  ldc.i4.3
    IL_0159:  newarr     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[]
    IL_015e:  dup
    IL_015f:  ldc.i4.0
    IL_0160:  ldc.i4.3
    IL_0161:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_0166:  dup
    IL_0167:  ldc.i4.0
    IL_0168:  ldc.r4     1.
    IL_016d:  ldc.r4     1.
    IL_0172:  ldc.r4     1.
    IL_0177:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_017c:  stelem.ref
    IL_017d:  dup
    IL_017e:  ldc.i4.1
    IL_017f:  ldc.r4     2.
    IL_0184:  ldc.r4     2.
    IL_0189:  ldc.r4     2.
    IL_018e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0193:  stelem.ref
    IL_0194:  dup
    IL_0195:  ldc.i4.2
    IL_0196:  ldc.r4     3.
    IL_019b:  ldc.r4     3.
    IL_01a0:  ldc.r4     3.
    IL_01a5:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_01aa:  stelem.ref
    IL_01ab:  stelem.ref
    IL_01ac:  dup
    IL_01ad:  ldc.i4.1
    IL_01ae:  ldc.i4.3
    IL_01af:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_01b4:  dup
    IL_01b5:  ldc.i4.0
    IL_01b6:  ldc.r4     2.
    IL_01bb:  ldc.r4     2.
    IL_01c0:  ldc.r4     2.
    IL_01c5:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_01ca:  stelem.ref
    IL_01cb:  dup
    IL_01cc:  ldc.i4.1
    IL_01cd:  ldc.r4     3.
    IL_01d2:  ldc.r4     3.
    IL_01d7:  ldc.r4     3.
    IL_01dc:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_01e1:  stelem.ref
    IL_01e2:  dup
    IL_01e3:  ldc.i4.2
    IL_01e4:  ldc.r4     4.
    IL_01e9:  ldc.r4     4.
    IL_01ee:  ldc.r4     4.
    IL_01f3:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_01f8:  stelem.ref
    IL_01f9:  stelem.ref
    IL_01fa:  dup
    IL_01fb:  ldc.i4.2
    IL_01fc:  ldc.i4.3
    IL_01fd:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_0202:  dup
    IL_0203:  ldc.i4.0
    IL_0204:  ldc.r4     3.
    IL_0209:  ldc.r4     3.
    IL_020e:  ldc.r4     3.
    IL_0213:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0218:  stelem.ref
    IL_0219:  dup
    IL_021a:  ldc.i4.1
    IL_021b:  ldc.r4     4.
    IL_0220:  ldc.r4     4.
    IL_0225:  ldc.r4     4.
    IL_022a:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_022f:  stelem.ref
    IL_0230:  dup
    IL_0231:  ldc.i4.2
    IL_0232:  ldc.r4     5.
    IL_0237:  ldc.r4     5.
    IL_023c:  ldc.r4     5.
    IL_0241:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0246:  stelem.ref
    IL_0247:  stelem.ref
    IL_0248:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Issue1336_rg1b
    IL_024d:  ldc.i4.3
    IL_024e:  ldc.i4.3
    IL_024f:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::.ctor(int32,
                                                                                                                                              int32)
    IL_0254:  dup
    IL_0255:  ldc.i4.0
    IL_0256:  ldc.i4.0
    IL_0257:  ldc.i4.3
    IL_0258:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_025d:  dup
    IL_025e:  ldc.i4.0
    IL_025f:  ldc.r4     1.
    IL_0264:  ldc.r4     1.
    IL_0269:  ldc.r4     1.
    IL_026e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0273:  stelem.ref
    IL_0274:  dup
    IL_0275:  ldc.i4.1
    IL_0276:  ldc.r4     2.
    IL_027b:  ldc.r4     2.
    IL_0280:  ldc.r4     2.
    IL_0285:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_028a:  stelem.ref
    IL_028b:  dup
    IL_028c:  ldc.i4.2
    IL_028d:  ldc.r4     3.
    IL_0292:  ldc.r4     3.
    IL_0297:  ldc.r4     3.
    IL_029c:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_02a1:  stelem.ref
    IL_02a2:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_02a7:  dup
    IL_02a8:  ldc.i4.0
    IL_02a9:  ldc.i4.1
    IL_02aa:  ldc.i4.3
    IL_02ab:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_02b0:  dup
    IL_02b1:  ldc.i4.0
    IL_02b2:  ldc.r4     2.
    IL_02b7:  ldc.r4     2.
    IL_02bc:  ldc.r4     2.
    IL_02c1:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_02c6:  stelem.ref
    IL_02c7:  dup
    IL_02c8:  ldc.i4.1
    IL_02c9:  ldc.r4     3.
    IL_02ce:  ldc.r4     3.
    IL_02d3:  ldc.r4     3.
    IL_02d8:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_02dd:  stelem.ref
    IL_02de:  dup
    IL_02df:  ldc.i4.2
    IL_02e0:  ldc.r4     4.
    IL_02e5:  ldc.r4     4.
    IL_02ea:  ldc.r4     4.
    IL_02ef:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_02f4:  stelem.ref
    IL_02f5:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_02fa:  dup
    IL_02fb:  ldc.i4.0
    IL_02fc:  ldc.i4.2
    IL_02fd:  ldc.i4.3
    IL_02fe:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_0303:  dup
    IL_0304:  ldc.i4.0
    IL_0305:  ldc.r4     3.
    IL_030a:  ldc.r4     3.
    IL_030f:  ldc.r4     3.
    IL_0314:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0319:  stelem.ref
    IL_031a:  dup
    IL_031b:  ldc.i4.1
    IL_031c:  ldc.r4     4.
    IL_0321:  ldc.r4     4.
    IL_0326:  ldc.r4     4.
    IL_032b:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0330:  stelem.ref
    IL_0331:  dup
    IL_0332:  ldc.i4.2
    IL_0333:  ldc.r4     5.
    IL_0338:  ldc.r4     5.
    IL_033d:  ldc.r4     5.
    IL_0342:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0347:  stelem.ref
    IL_0348:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_034d:  dup
    IL_034e:  ldc.i4.1
    IL_034f:  ldc.i4.0
    IL_0350:  ldc.i4.3
    IL_0351:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_0356:  dup
    IL_0357:  ldc.i4.0
    IL_0358:  ldc.r4     1.
    IL_035d:  ldc.r4     1.
    IL_0362:  ldc.r4     1.
    IL_0367:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_036c:  stelem.ref
    IL_036d:  dup
    IL_036e:  ldc.i4.1
    IL_036f:  ldc.r4     2.
    IL_0374:  ldc.r4     2.
    IL_0379:  ldc.r4     2.
    IL_037e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0383:  stelem.ref
    IL_0384:  dup
    IL_0385:  ldc.i4.2
    IL_0386:  ldc.r4     3.
    IL_038b:  ldc.r4     3.
    IL_0390:  ldc.r4     3.
    IL_0395:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_039a:  stelem.ref
    IL_039b:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_03a0:  dup
    IL_03a1:  ldc.i4.1
    IL_03a2:  ldc.i4.1
    IL_03a3:  ldc.i4.3
    IL_03a4:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_03a9:  dup
    IL_03aa:  ldc.i4.0
    IL_03ab:  ldc.r4     2.
    IL_03b0:  ldc.r4     2.
    IL_03b5:  ldc.r4     2.
    IL_03ba:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_03bf:  stelem.ref
    IL_03c0:  dup
    IL_03c1:  ldc.i4.1
    IL_03c2:  ldc.r4     3.
    IL_03c7:  ldc.r4     3.
    IL_03cc:  ldc.r4     3.
    IL_03d1:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_03d6:  stelem.ref
    IL_03d7:  dup
    IL_03d8:  ldc.i4.2
    IL_03d9:  ldc.r4     4.
    IL_03de:  ldc.r4     4.
    IL_03e3:  ldc.r4     4.
    IL_03e8:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_03ed:  stelem.ref
    IL_03ee:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_03f3:  dup
    IL_03f4:  ldc.i4.1
    IL_03f5:  ldc.i4.2
    IL_03f6:  ldc.i4.3
    IL_03f7:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_03fc:  dup
    IL_03fd:  ldc.i4.0
    IL_03fe:  ldc.r4     3.
    IL_0403:  ldc.r4     3.
    IL_0408:  ldc.r4     3.
    IL_040d:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0412:  stelem.ref
    IL_0413:  dup
    IL_0414:  ldc.i4.1
    IL_0415:  ldc.r4     4.
    IL_041a:  ldc.r4     4.
    IL_041f:  ldc.r4     4.
    IL_0424:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0429:  stelem.ref
    IL_042a:  dup
    IL_042b:  ldc.i4.2
    IL_042c:  ldc.r4     5.
    IL_0431:  ldc.r4     5.
    IL_0436:  ldc.r4     5.
    IL_043b:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0440:  stelem.ref
    IL_0441:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_0446:  dup
    IL_0447:  ldc.i4.2
    IL_0448:  ldc.i4.0
    IL_0449:  ldc.i4.3
    IL_044a:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_044f:  dup
    IL_0450:  ldc.i4.0
    IL_0451:  ldc.r4     1.
    IL_0456:  ldc.r4     1.
    IL_045b:  ldc.r4     1.
    IL_0460:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0465:  stelem.ref
    IL_0466:  dup
    IL_0467:  ldc.i4.1
    IL_0468:  ldc.r4     2.
    IL_046d:  ldc.r4     2.
    IL_0472:  ldc.r4     2.
    IL_0477:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_047c:  stelem.ref
    IL_047d:  dup
    IL_047e:  ldc.i4.2
    IL_047f:  ldc.r4     3.
    IL_0484:  ldc.r4     3.
    IL_0489:  ldc.r4     3.
    IL_048e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0493:  stelem.ref
    IL_0494:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_0499:  dup
    IL_049a:  ldc.i4.2
    IL_049b:  ldc.i4.1
    IL_049c:  ldc.i4.3
    IL_049d:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_04a2:  dup
    IL_04a3:  ldc.i4.0
    IL_04a4:  ldc.r4     2.
    IL_04a9:  ldc.r4     2.
    IL_04ae:  ldc.r4     2.
    IL_04b3:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_04b8:  stelem.ref
    IL_04b9:  dup
    IL_04ba:  ldc.i4.1
    IL_04bb:  ldc.r4     3.
    IL_04c0:  ldc.r4     3.
    IL_04c5:  ldc.r4     3.
    IL_04ca:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_04cf:  stelem.ref
    IL_04d0:  dup
    IL_04d1:  ldc.i4.2
    IL_04d2:  ldc.r4     4.
    IL_04d7:  ldc.r4     4.
    IL_04dc:  ldc.r4     4.
    IL_04e1:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_04e6:  stelem.ref
    IL_04e7:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_04ec:  dup
    IL_04ed:  ldc.i4.2
    IL_04ee:  ldc.i4.2
    IL_04ef:  ldc.i4.3
    IL_04f0:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_04f5:  dup
    IL_04f6:  ldc.i4.0
    IL_04f7:  ldc.r4     3.
    IL_04fc:  ldc.r4     3.
    IL_0501:  ldc.r4     3.
    IL_0506:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_050b:  stelem.ref
    IL_050c:  dup
    IL_050d:  ldc.i4.1
    IL_050e:  ldc.r4     4.
    IL_0513:  ldc.r4     4.
    IL_0518:  ldc.r4     4.
    IL_051d:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0522:  stelem.ref
    IL_0523:  dup
    IL_0524:  ldc.i4.2
    IL_0525:  ldc.r4     5.
    IL_052a:  ldc.r4     5.
    IL_052f:  ldc.r4     5.
    IL_0534:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0539:  stelem.ref
    IL_053a:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_053f:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Issue1336_rg1c
    IL_0544:  ldc.i4.2
    IL_0545:  newarr     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]
    IL_054a:  dup
    IL_054b:  ldc.i4.0
    IL_054c:  ldc.i4.3
    IL_054d:  ldc.i4.3
    IL_054e:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::.ctor(int32,
                                                                                                                                            int32)
    IL_0553:  dup
    IL_0554:  ldc.i4.0
    IL_0555:  ldc.i4.0
    IL_0556:  ldc.r4     1.
    IL_055b:  ldc.r4     1.
    IL_0560:  ldc.r4     1.
    IL_0565:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_056a:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_056f:  dup
    IL_0570:  ldc.i4.0
    IL_0571:  ldc.i4.1
    IL_0572:  ldc.r4     2.
    IL_0577:  ldc.r4     2.
    IL_057c:  ldc.r4     2.
    IL_0581:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0586:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_058b:  dup
    IL_058c:  ldc.i4.0
    IL_058d:  ldc.i4.2
    IL_058e:  ldc.r4     3.
    IL_0593:  ldc.r4     3.
    IL_0598:  ldc.r4     3.
    IL_059d:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_05a2:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_05a7:  dup
    IL_05a8:  ldc.i4.1
    IL_05a9:  ldc.i4.0
    IL_05aa:  ldc.r4     2.
    IL_05af:  ldc.r4     2.
    IL_05b4:  ldc.r4     2.
    IL_05b9:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_05be:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_05c3:  dup
    IL_05c4:  ldc.i4.1
    IL_05c5:  ldc.i4.1
    IL_05c6:  ldc.r4     3.
    IL_05cb:  ldc.r4     3.
    IL_05d0:  ldc.r4     3.
    IL_05d5:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_05da:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_05df:  dup
    IL_05e0:  ldc.i4.1
    IL_05e1:  ldc.i4.2
    IL_05e2:  ldc.r4     4.
    IL_05e7:  ldc.r4     4.
    IL_05ec:  ldc.r4     4.
    IL_05f1:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_05f6:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_05fb:  dup
    IL_05fc:  ldc.i4.2
    IL_05fd:  ldc.i4.0
    IL_05fe:  ldc.r4     3.
    IL_0603:  ldc.r4     3.
    IL_0608:  ldc.r4     3.
    IL_060d:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0612:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0617:  dup
    IL_0618:  ldc.i4.2
    IL_0619:  ldc.i4.1
    IL_061a:  ldc.r4     4.
    IL_061f:  ldc.r4     4.
    IL_0624:  ldc.r4     4.
    IL_0629:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_062e:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0633:  dup
    IL_0634:  ldc.i4.2
    IL_0635:  ldc.i4.2
    IL_0636:  ldc.r4     5.
    IL_063b:  ldc.r4     5.
    IL_0640:  ldc.r4     5.
    IL_0645:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_064a:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_064f:  stelem.ref
    IL_0650:  dup
    IL_0651:  ldc.i4.1
    IL_0652:  ldc.i4.3
    IL_0653:  ldc.i4.3
    IL_0654:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::.ctor(int32,
                                                                                                                                            int32)
    IL_0659:  dup
    IL_065a:  ldc.i4.0
    IL_065b:  ldc.i4.0
    IL_065c:  ldc.r4     1.
    IL_0661:  ldc.r4     1.
    IL_0666:  ldc.r4     1.
    IL_066b:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0670:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0675:  dup
    IL_0676:  ldc.i4.0
    IL_0677:  ldc.i4.1
    IL_0678:  ldc.r4     2.
    IL_067d:  ldc.r4     2.
    IL_0682:  ldc.r4     2.
    IL_0687:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_068c:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0691:  dup
    IL_0692:  ldc.i4.0
    IL_0693:  ldc.i4.2
    IL_0694:  ldc.r4     3.
    IL_0699:  ldc.r4     3.
    IL_069e:  ldc.r4     3.
    IL_06a3:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_06a8:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_06ad:  dup
    IL_06ae:  ldc.i4.1
    IL_06af:  ldc.i4.0
    IL_06b0:  ldc.r4     2.
    IL_06b5:  ldc.r4     2.
    IL_06ba:  ldc.r4     2.
    IL_06bf:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_06c4:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_06c9:  dup
    IL_06ca:  ldc.i4.1
    IL_06cb:  ldc.i4.1
    IL_06cc:  ldc.r4     3.
    IL_06d1:  ldc.r4     3.
    IL_06d6:  ldc.r4     3.
    IL_06db:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_06e0:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_06e5:  dup
    IL_06e6:  ldc.i4.1
    IL_06e7:  ldc.i4.2
    IL_06e8:  ldc.r4     4.
    IL_06ed:  ldc.r4     4.
    IL_06f2:  ldc.r4     4.
    IL_06f7:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_06fc:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0701:  dup
    IL_0702:  ldc.i4.2
    IL_0703:  ldc.i4.0
    IL_0704:  ldc.r4     3.
    IL_0709:  ldc.r4     3.
    IL_070e:  ldc.r4     3.
    IL_0713:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0718:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_071d:  dup
    IL_071e:  ldc.i4.2
    IL_071f:  ldc.i4.1
    IL_0720:  ldc.r4     4.
    IL_0725:  ldc.r4     4.
    IL_072a:  ldc.r4     4.
    IL_072f:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0734:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0739:  dup
    IL_073a:  ldc.i4.2
    IL_073b:  ldc.i4.2
    IL_073c:  ldc.r4     5.
    IL_0741:  ldc.r4     5.
    IL_0746:  ldc.r4     5.
    IL_074b:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0750:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0755:  stelem.ref
    IL_0756:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...][] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Issue1336_rg1d
    IL_075b:  ldc.i4.3
    IL_075c:  ldc.i4.3
    IL_075d:  newobj     instance void int32[0...,0...]::.ctor(int32,
                                                               int32)
    IL_0762:  dup
    IL_0763:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=36' '<PrivateImplementationDetails>'::B62E59D20E3D69F06A6D9BD5E3C518FF7093EDAB
    IL_0768:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_076d:  stsfld     int32[0...,0...] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Issue1336_rg2
    IL_0772:  ret
  } // end of method TestCases::.cctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases

.class private auto ansi sealed '<PrivateImplementationDetails>'
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
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

  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=24'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 24
  } // end of class '__StaticArrayInitTypeSize=24'

  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=36'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 36
  } // end of class '__StaticArrayInitTypeSize=36'

  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=40'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 40
  } // end of class '__StaticArrayInitTypeSize=40'

  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=48'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 48
  } // end of class '__StaticArrayInitTypeSize=48'

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

  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=80'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 80
  } // end of class '__StaticArrayInitTypeSize=80'

  .class explicit ansi sealed nested private '__StaticArrayInitTypeSize=256'
         extends [mscorlib]System.ValueType
  {
    .pack 1
    .size 256
  } // end of class '__StaticArrayInitTypeSize=256'

  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=72' '1535117EC92E41D4A6B7CA00F965357B05B5DC35' at I_00007040
  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=10' '20E3FF489634E18F3F7EB292AD504DBAE9519293' at I_00007088
  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=72' '39E94835525CF7B71CD4595742EF462642FBF1B2' at I_00007098
  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=10' '56D9EEC8EF899644C40B9BE9D886DF2367A5D078' at I_000070E0
  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=12' '735E5A21849E86F68D220F06163E8C5C6376B9C9' at I_000070F0
  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' '7C39B7B06DD624A17F875AB8E9651554BE6E74D2' at I_00007100
  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '8D903ECAD8D9D75B3183B23AF79F6D2E607369E3' at I_00007140
  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=80' '9B1F6E56D755443CC39C1969CE38FD41FD4EF4B7' at I_00007168
  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=256' A1EA7DC3FE43B3A54F5B729A92B92AF54181A3EB at I_000071B8
  .field static assembly initonly int64 A6296CAC471BE2954899600137940479D8073C7C at I_000072B8
  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=36' B62E59D20E3D69F06A6D9BD5E3C518FF7093EDAB at I_000072C0
  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' B9583930B842DBCEF0D7B8E57D4D3F1E8055C39E at I_000072E8
  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=24' C4E70AB31EF6C8908F896CAD1C6BC75F7FA65E27 at I_00007310
  .field static assembly initonly int32 C62C27924F4C967F5EDDB1850C091D54C7A2AB58 at I_00007328
  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=48' DC7043B0114737ACE19A23DD755893795FD48A23 at I_00007330
  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' DCF557B883E6FE0AEC05B7F0290F0EF47D0AC2E3 at I_00007360
  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' E0D2592373A0C161E56E266306CD8405CD719D19 at I_000073A0
  .field static assembly initonly int64 EB0715DBB235F3F696F2C404F5839C6650640898 at I_000073C8
  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' F514FF55B79BCAA2CEC9B56C062D976E45F89AB7 at I_000073D0
  .field static assembly initonly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=24' FBCB49C1A244C1B5781AA1DB02C5A11F68908526 at I_000073F8
} // end of class '<PrivateImplementationDetails>'


// =============================================================

.data cil I_00007040 = bytearray (
                 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00
                 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00
                 09 00 00 00 0B 00 00 00 0C 00 00 00 0D 00 00 00
                 0E 00 00 00 0F 00 00 00 10 00 00 00 11 00 00 00
                 12 00 00 00 13 00 00 00) 
.data cil I_00007088 = bytearray (
                 01 02 03 04 05 06 07 08 FE FF) 
.data cil I_00007092 = int8[6]
.data cil I_00007098 = bytearray (
                 15 00 00 00 16 00 00 00 17 00 00 00 18 00 00 00
                 19 00 00 00 1A 00 00 00 1B 00 00 00 1C 00 00 00
                 1D 00 00 00 1F 00 00 00 20 00 00 00 21 00 00 00  // ........ ...!...
                 22 00 00 00 23 00 00 00 24 00 00 00 25 00 00 00  // "...#...$...%...
                 26 00 00 00 27 00 00 00)                         // &...'...
.data cil I_000070E0 = bytearray (
                 00 80 FF FF 00 00 01 00 FF 7F) 
.data cil I_000070EA = int8[6]
.data cil I_000070F0 = bytearray (
                 00 00 01 00 FF 7F 00 80 FE FF FF FF) 
.data cil I_000070FC = int8[4]
.data cil I_00007100 = bytearray (
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00) 
.data cil I_00007140 = bytearray (
                 01 00 0C BB 7D 6E 9C BA FF FF FF FF FF FF FF FF  // ....}n..........
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 FF FF F3 44 82 91 63 45)                         // ...D..cE
.data cil I_00007168 = bytearray (
                 01 00 00 00 00 00 00 00 00 94 35 77 00 00 00 00  // ..........5w....
                 00 5E D0 B2 00 00 00 00 04 00 00 00 00 00 00 00  // .^..............
                 05 00 00 00 00 00 00 00 06 00 00 00 00 00 00 00
                 07 00 00 00 00 00 00 00 08 00 00 00 00 00 00 00
                 FF FF F3 44 82 91 63 45 FF FF E7 89 04 23 C7 8A) // ...D..cE.....#..
.data cil I_000071B8 = bytearray (
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
.data cil I_000072B8 = bytearray (
                 80 81 00 01 02 03 04 7F) 
.data cil I_000072C0 = bytearray (
                 01 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00
                 01 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00
                 01 00 00 00) 
.data cil I_000072E4 = int8[4]
.data cil I_000072E8 = bytearray (
                 01 00 00 00 00 94 35 77 00 5E D0 B2 04 00 00 00  // ......5w.^......
                 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00
                 09 00 00 00 0A 00 00 00) 
.data cil I_00007310 = bytearray (
                 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00
                 05 00 00 00 06 00 00 00) 
.data cil I_00007328 = bytearray (
                 00 01 02 FF) 
.data cil I_0000732C = int8[4]
.data cil I_00007330 = bytearray (
                 00 00 00 00 00 00 F8 BF 00 00 00 00 00 00 00 00
                 00 00 00 00 00 00 F8 3F 00 00 00 00 00 00 F0 FF  // .......?........
                 00 00 00 00 00 00 F0 7F 00 00 00 00 00 00 F8 FF) 
.data cil I_00007360 = bytearray (
                 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                 01 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00
                 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00) 
.data cil I_000073A0 = bytearray (
                 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00
                 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00
                 09 00 00 00 0A 00 00 00) 
.data cil I_000073C8 = bytearray (
                 01 00 01 00 00 00 01 01) 
.data cil I_000073D0 = bytearray (
                 01 00 00 00 FE FF FF FF 00 94 35 77 04 00 00 00  // ..........5w....
                 05 00 00 00 FA FF FF FF 07 00 00 00 08 00 00 00
                 09 00 00 00 0A 00 00 00) 
.data cil I_000073F8 = bytearray (
                 00 00 C0 BF 00 00 00 00 00 00 C0 3F 00 00 80 FF  // ...........?....
                 00 00 80 7F 00 00 C0 FF) 
// *********** DISASSEMBLY COMPLETE ***********************
