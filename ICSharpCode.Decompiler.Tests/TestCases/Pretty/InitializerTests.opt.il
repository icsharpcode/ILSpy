



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
.assembly InitializerTests.opt
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
.module InitializerTests.opt.dll
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
    IL_000d:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '<PrivateImplementationDetails>'::'$$method0x600000b-1'
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
    // Code size       31 (0x1f)
    .maxstack  4
    .locals init (int32[] V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.5
    IL_0006:  newarr     [mscorlib]System.Int32
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.0
    IL_000e:  ldarg.0
    IL_000f:  stelem.i4
    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.2
    IL_0012:  ldarg.1
    IL_0013:  stelem.i4
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.4
    IL_0016:  ldarg.2
    IL_0017:  stelem.i4
    IL_0018:  ldloc.0
    IL_0019:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001e:  ret
  } // end of method TestCases::Array2

  .method public hidebysig static void  NestedArray(int32 a,
                                                    int32 b,
                                                    int32 c) cil managed
  {
    // Code size       83 (0x53)
    .maxstack  6
    .locals init (int32[][] V_0,
             int32[] V_1)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.3
    IL_0006:  newarr     int32[]
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.0
    IL_000e:  ldc.i4.s   10
    IL_0010:  newarr     [mscorlib]System.Int32
    IL_0015:  dup
    IL_0016:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '<PrivateImplementationDetails>'::'$$method0x600000d-1'
    IL_001b:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0020:  stelem.ref
    IL_0021:  ldloc.0
    IL_0022:  ldc.i4.1
    IL_0023:  ldc.i4.3
    IL_0024:  newarr     [mscorlib]System.Int32
    IL_0029:  stloc.1
    IL_002a:  ldloc.1
    IL_002b:  ldc.i4.0
    IL_002c:  ldarg.0
    IL_002d:  stelem.i4
    IL_002e:  ldloc.1
    IL_002f:  ldc.i4.1
    IL_0030:  ldarg.1
    IL_0031:  stelem.i4
    IL_0032:  ldloc.1
    IL_0033:  ldc.i4.2
    IL_0034:  ldarg.2
    IL_0035:  stelem.i4
    IL_0036:  ldloc.1
    IL_0037:  stelem.ref
    IL_0038:  ldloc.0
    IL_0039:  ldc.i4.2
    IL_003a:  ldc.i4.6
    IL_003b:  newarr     [mscorlib]System.Int32
    IL_0040:  dup
    IL_0041:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=24' '<PrivateImplementationDetails>'::'$$method0x600000d-2'
    IL_0046:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_004b:  stelem.ref
    IL_004c:  ldloc.0
    IL_004d:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0052:  ret
  } // end of method TestCases::NestedArray

  .method public hidebysig static void  NestedNullableArray(int32 a,
                                                            int32 b,
                                                            int32 c) cil managed
  {
    // Code size       473 (0x1d9)
    .maxstack  5
    .locals init (valuetype [mscorlib]System.Nullable`1<int32>[][] V_0,
             valuetype [mscorlib]System.Nullable`1<int32>[] V_1,
             valuetype [mscorlib]System.Nullable`1<int32> V_2,
             valuetype [mscorlib]System.Nullable`1<int32>[] V_3,
             valuetype [mscorlib]System.Nullable`1<int32> V_4,
             valuetype [mscorlib]System.Nullable`1<int32>[] V_5,
             valuetype [mscorlib]System.Nullable`1<int32> V_6)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.3
    IL_0006:  newarr     valuetype [mscorlib]System.Nullable`1<int32>[]
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.0
    IL_000e:  ldc.i4.s   11
    IL_0010:  newarr     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0015:  stloc.1
    IL_0016:  ldloc.1
    IL_0017:  ldc.i4.0
    IL_0018:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_001d:  ldc.i4.1
    IL_001e:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0023:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0028:  ldloc.1
    IL_0029:  ldc.i4.1
    IL_002a:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_002f:  ldc.i4.2
    IL_0030:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0035:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_003a:  ldloc.1
    IL_003b:  ldc.i4.2
    IL_003c:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0041:  ldc.i4.3
    IL_0042:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0047:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_004c:  ldloc.1
    IL_004d:  ldc.i4.3
    IL_004e:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0053:  ldc.i4.4
    IL_0054:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0059:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_005e:  ldloc.1
    IL_005f:  ldc.i4.4
    IL_0060:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0065:  ldc.i4.5
    IL_0066:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_006b:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0070:  ldloc.1
    IL_0071:  ldc.i4.5
    IL_0072:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0077:  ldc.i4.6
    IL_0078:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_007d:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0082:  ldloc.1
    IL_0083:  ldc.i4.6
    IL_0084:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0089:  ldc.i4.7
    IL_008a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_008f:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0094:  ldloc.1
    IL_0095:  ldc.i4.7
    IL_0096:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_009b:  ldc.i4.8
    IL_009c:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00a1:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_00a6:  ldloc.1
    IL_00a7:  ldc.i4.8
    IL_00a8:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00ad:  ldc.i4.s   9
    IL_00af:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00b4:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_00b9:  ldloc.1
    IL_00ba:  ldc.i4.s   9
    IL_00bc:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00c1:  ldc.i4.s   10
    IL_00c3:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00c8:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_00cd:  ldloc.1
    IL_00ce:  ldc.i4.s   10
    IL_00d0:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00d5:  ldloca.s   V_2
    IL_00d7:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00dd:  ldloc.2
    IL_00de:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_00e3:  ldloc.1
    IL_00e4:  stelem.ref
    IL_00e5:  ldloc.0
    IL_00e6:  ldc.i4.1
    IL_00e7:  ldc.i4.4
    IL_00e8:  newarr     valuetype [mscorlib]System.Nullable`1<int32>
    IL_00ed:  stloc.3
    IL_00ee:  ldloc.3
    IL_00ef:  ldc.i4.0
    IL_00f0:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_00f5:  ldarg.0
    IL_00f6:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_00fb:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0100:  ldloc.3
    IL_0101:  ldc.i4.1
    IL_0102:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0107:  ldarg.1
    IL_0108:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_010d:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0112:  ldloc.3
    IL_0113:  ldc.i4.2
    IL_0114:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0119:  ldarg.2
    IL_011a:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_011f:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0124:  ldloc.3
    IL_0125:  ldc.i4.3
    IL_0126:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_012b:  ldloca.s   V_4
    IL_012d:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0133:  ldloc.s    V_4
    IL_0135:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_013a:  ldloc.3
    IL_013b:  stelem.ref
    IL_013c:  ldloc.0
    IL_013d:  ldc.i4.2
    IL_013e:  ldc.i4.7
    IL_013f:  newarr     valuetype [mscorlib]System.Nullable`1<int32>
    IL_0144:  stloc.s    V_5
    IL_0146:  ldloc.s    V_5
    IL_0148:  ldc.i4.0
    IL_0149:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_014e:  ldc.i4.1
    IL_014f:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0154:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0159:  ldloc.s    V_5
    IL_015b:  ldc.i4.1
    IL_015c:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0161:  ldc.i4.2
    IL_0162:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_0167:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_016c:  ldloc.s    V_5
    IL_016e:  ldc.i4.2
    IL_016f:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0174:  ldc.i4.3
    IL_0175:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_017a:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_017f:  ldloc.s    V_5
    IL_0181:  ldc.i4.3
    IL_0182:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_0187:  ldc.i4.4
    IL_0188:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_018d:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_0192:  ldloc.s    V_5
    IL_0194:  ldc.i4.4
    IL_0195:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_019a:  ldc.i4.5
    IL_019b:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01a0:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_01a5:  ldloc.s    V_5
    IL_01a7:  ldc.i4.5
    IL_01a8:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01ad:  ldc.i4.6
    IL_01ae:  newobj     instance void valuetype [mscorlib]System.Nullable`1<int32>::.ctor(!0)
    IL_01b3:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_01b8:  ldloc.s    V_5
    IL_01ba:  ldc.i4.6
    IL_01bb:  ldelema    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01c0:  ldloca.s   V_6
    IL_01c2:  initobj    valuetype [mscorlib]System.Nullable`1<int32>
    IL_01c8:  ldloc.s    V_6
    IL_01ca:  stobj      valuetype [mscorlib]System.Nullable`1<int32>
    IL_01cf:  ldloc.s    V_5
    IL_01d1:  stelem.ref
    IL_01d2:  ldloc.0
    IL_01d3:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_01d8:  ret
  } // end of method TestCases::NestedNullableArray

  .method public hidebysig static void  NestedPointerArray(int32 a,
                                                           int32 b,
                                                           int32 c) cil managed
  {
    // Code size       82 (0x52)
    .maxstack  6
    .locals init (void*[][] V_0,
             void*[] V_1,
             void*[] V_2,
             void*[] V_3)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.3
    IL_0006:  newarr     void*[]
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.0
    IL_000e:  ldc.i4.1
    IL_000f:  newarr     void*
    IL_0014:  stloc.1
    IL_0015:  ldloc.1
    IL_0016:  ldc.i4.0
    IL_0017:  ldc.i4.0
    IL_0018:  conv.u
    IL_0019:  stelem.i
    IL_001a:  ldloc.1
    IL_001b:  stelem.ref
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.1
    IL_001e:  ldc.i4.2
    IL_001f:  newarr     void*
    IL_0024:  stloc.2
    IL_0025:  ldloc.2
    IL_0026:  ldc.i4.0
    IL_0027:  ldc.i4     0xc8
    IL_002c:  conv.i
    IL_002d:  stelem.i
    IL_002e:  ldloc.2
    IL_002f:  ldc.i4.1
    IL_0030:  ldc.i4.0
    IL_0031:  conv.u
    IL_0032:  stelem.i
    IL_0033:  ldloc.2
    IL_0034:  stelem.ref
    IL_0035:  ldloc.0
    IL_0036:  ldc.i4.2
    IL_0037:  ldc.i4.2
    IL_0038:  newarr     void*
    IL_003d:  stloc.3
    IL_003e:  ldloc.3
    IL_003f:  ldc.i4.0
    IL_0040:  ldc.i4.s   100
    IL_0042:  conv.i
    IL_0043:  stelem.i
    IL_0044:  ldloc.3
    IL_0045:  ldc.i4.1
    IL_0046:  ldc.i4.0
    IL_0047:  conv.u
    IL_0048:  stelem.i
    IL_0049:  ldloc.3
    IL_004a:  stelem.ref
    IL_004b:  ldloc.0
    IL_004c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0051:  ret
  } // end of method TestCases::NestedPointerArray

  .method public hidebysig static void  ArrayBoolean() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  8
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.8
    IL_0006:  newarr     [mscorlib]System.Boolean
    IL_000b:  dup
    IL_000c:  ldtoken    field int64 '<PrivateImplementationDetails>'::'$$method0x6000010-1'
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
    IL_000d:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=10' '<PrivateImplementationDetails>'::'$$method0x6000011-1'
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
    IL_000c:  ldtoken    field int64 '<PrivateImplementationDetails>'::'$$method0x6000012-1'
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
    IL_000c:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=10' '<PrivateImplementationDetails>'::'$$method0x6000013-1'
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
    IL_000c:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=12' '<PrivateImplementationDetails>'::'$$method0x6000014-1'
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
    IL_000d:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '<PrivateImplementationDetails>'::'$$method0x6000015-1'
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
    IL_000d:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '<PrivateImplementationDetails>'::'$$method0x6000016-1'
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
    IL_000c:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '<PrivateImplementationDetails>'::'$$method0x6000017-1'
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
    IL_000d:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=80' '<PrivateImplementationDetails>'::'$$method0x6000018-1'
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
    IL_000c:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=24' '<PrivateImplementationDetails>'::'$$method0x6000019-1'
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
    IL_000c:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=48' '<PrivateImplementationDetails>'::'$$method0x600001a-1'
    IL_0011:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0016:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001b:  ret
  } // end of method TestCases::ArrayDouble

  .method public hidebysig static void  ArrayDecimal() cil managed
  {
    // Code size       127 (0x7f)
    .maxstack  7
    .locals init (valuetype [mscorlib]System.Decimal[] V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.6
    IL_0006:  newarr     [mscorlib]System.Decimal
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.0
    IL_000e:  ldelema    [mscorlib]System.Decimal
    IL_0013:  ldc.i4.s   -100
    IL_0015:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_001a:  stobj      [mscorlib]System.Decimal
    IL_001f:  ldloc.0
    IL_0020:  ldc.i4.2
    IL_0021:  ldelema    [mscorlib]System.Decimal
    IL_0026:  ldc.i4.s   100
    IL_0028:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_002d:  stobj      [mscorlib]System.Decimal
    IL_0032:  ldloc.0
    IL_0033:  ldc.i4.3
    IL_0034:  ldelema    [mscorlib]System.Decimal
    IL_0039:  ldc.i4.m1
    IL_003a:  ldc.i4.m1
    IL_003b:  ldc.i4.m1
    IL_003c:  ldc.i4     0x80
    IL_0041:  ldc.i4.0
    IL_0042:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0047:  stobj      [mscorlib]System.Decimal
    IL_004c:  ldloc.0
    IL_004d:  ldc.i4.4
    IL_004e:  ldelema    [mscorlib]System.Decimal
    IL_0053:  ldc.i4.m1
    IL_0054:  ldc.i4.m1
    IL_0055:  ldc.i4.m1
    IL_0056:  ldc.i4.0
    IL_0057:  ldc.i4.0
    IL_0058:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_005d:  stobj      [mscorlib]System.Decimal
    IL_0062:  ldloc.0
    IL_0063:  ldc.i4.5
    IL_0064:  ldelema    [mscorlib]System.Decimal
    IL_0069:  ldc.i4.1
    IL_006a:  ldc.i4.0
    IL_006b:  ldc.i4.0
    IL_006c:  ldc.i4.0
    IL_006d:  ldc.i4.7
    IL_006e:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0073:  stobj      [mscorlib]System.Decimal
    IL_0078:  ldloc.0
    IL_0079:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_007e:  ret
  } // end of method TestCases::ArrayDecimal

  .method public hidebysig static void  ArrayString() cil managed
  {
    // Code size       43 (0x2b)
    .maxstack  4
    .locals init (string[] V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.4
    IL_0006:  newarr     [mscorlib]System.String
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.0
    IL_000e:  ldstr      ""
    IL_0013:  stelem.ref
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.2
    IL_0016:  ldstr      "Hello"
    IL_001b:  stelem.ref
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.3
    IL_001e:  ldstr      "World"
    IL_0023:  stelem.ref
    IL_0024:  ldloc.0
    IL_0025:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_002a:  ret
  } // end of method TestCases::ArrayString

  .method public hidebysig static void  ArrayEnum() cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  4
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum[] V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  ldc.i4.4
    IL_0006:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.1
    IL_000e:  ldc.i4.1
    IL_000f:  stelem.i4
    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.3
    IL_0012:  ldc.i4.1
    IL_0013:  stelem.i4
    IL_0014:  ldloc.0
    IL_0015:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001a:  ret
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
    IL_0009:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=256' '<PrivateImplementationDetails>'::'$$method0x600001e-1'
    IL_000e:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0013:  ret
  } // end of method TestCases::MultidimensionalInit

  .method public hidebysig instance int32[0...,0...][] 
          MultidimensionalInit2() cil managed
  {
    // Code size       93 (0x5d)
    .maxstack  5
    .locals init (int32[0...,0...][] V_0)
    IL_0000:  ldc.i4.4
    IL_0001:  newarr     int32[0...,0...]
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.i4.4
    IL_000a:  ldc.i4.4
    IL_000b:  newobj     instance void int32[0...,0...]::.ctor(int32,
                                                               int32)
    IL_0010:  dup
    IL_0011:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' '<PrivateImplementationDetails>'::'$$method0x600001f-1'
    IL_0016:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_001b:  stelem.ref
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.1
    IL_001e:  ldc.i4.4
    IL_001f:  ldc.i4.4
    IL_0020:  newobj     instance void int32[0...,0...]::.ctor(int32,
                                                               int32)
    IL_0025:  dup
    IL_0026:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' '<PrivateImplementationDetails>'::'$$method0x600001f-2'
    IL_002b:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0030:  stelem.ref
    IL_0031:  ldloc.0
    IL_0032:  ldc.i4.2
    IL_0033:  ldc.i4.4
    IL_0034:  ldc.i4.4
    IL_0035:  newobj     instance void int32[0...,0...]::.ctor(int32,
                                                               int32)
    IL_003a:  dup
    IL_003b:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' '<PrivateImplementationDetails>'::'$$method0x600001f-3'
    IL_0040:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0045:  stelem.ref
    IL_0046:  ldloc.0
    IL_0047:  ldc.i4.3
    IL_0048:  ldc.i4.4
    IL_0049:  ldc.i4.4
    IL_004a:  newobj     instance void int32[0...,0...]::.ctor(int32,
                                                               int32)
    IL_004f:  dup
    IL_0050:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' '<PrivateImplementationDetails>'::'$$method0x600001f-4'
    IL_0055:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_005a:  stelem.ref
    IL_005b:  ldloc.0
    IL_005c:  ret
  } // end of method TestCases::MultidimensionalInit2

  .method public hidebysig instance int32[0...,0...,0...][] 
          ArrayOfArrayOfArrayInit() cil managed
  {
    // Code size       53 (0x35)
    .maxstack  5
    .locals init (int32[0...,0...,0...][] V_0)
    IL_0000:  ldc.i4.2
    IL_0001:  newarr     int32[0...,0...,0...]
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.i4.2
    IL_000a:  ldc.i4.3
    IL_000b:  ldc.i4.3
    IL_000c:  newobj     instance void int32[0...,0...,0...]::.ctor(int32,
                                                                    int32,
                                                                    int32)
    IL_0011:  dup
    IL_0012:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=72' '<PrivateImplementationDetails>'::'$$method0x6000020-1'
    IL_0017:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_001c:  stelem.ref
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.1
    IL_001f:  ldc.i4.2
    IL_0020:  ldc.i4.3
    IL_0021:  ldc.i4.3
    IL_0022:  newobj     instance void int32[0...,0...,0...]::.ctor(int32,
                                                                    int32,
                                                                    int32)
    IL_0027:  dup
    IL_0028:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=72' '<PrivateImplementationDetails>'::'$$method0x6000020-2'
    IL_002d:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0032:  stelem.ref
    IL_0033:  ldloc.0
    IL_0034:  ret
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
    // Code size       37 (0x25)
    .maxstack  4
    .locals init (uint8[] V_0)
    IL_0000:  ldc.i4.4
    IL_0001:  newarr     [mscorlib]System.Byte
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.3
    IL_0009:  ldarg.0
    IL_000a:  conv.u1
    IL_000b:  stelem.i1
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.2
    IL_000e:  ldarg.0
    IL_000f:  ldc.i4.8
    IL_0010:  shr
    IL_0011:  conv.u1
    IL_0012:  stelem.i1
    IL_0013:  ldloc.0
    IL_0014:  ldc.i4.1
    IL_0015:  ldarg.0
    IL_0016:  ldc.i4.s   16
    IL_0018:  shr
    IL_0019:  conv.u1
    IL_001a:  stelem.i1
    IL_001b:  ldloc.0
    IL_001c:  ldc.i4.0
    IL_001d:  ldarg.0
    IL_001e:  ldc.i4.s   24
    IL_0020:  shr
    IL_0021:  conv.u1
    IL_0022:  stelem.i1
    IL_0023:  ldloc.0
    IL_0024:  ret
  } // end of method TestCases::ReverseInitializer

  .method public hidebysig static void  Issue953_MissingNullableSpecifierForArrayInitializer() cil managed
  {
    // Code size       36 (0x24)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Guid>[] V_0)
    IL_0000:  ldc.i4.1
    IL_0001:  newarr     valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Guid>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.0
    IL_0009:  ldelema    valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Guid>
    IL_000e:  ldsfld     valuetype [mscorlib]System.Guid [mscorlib]System.Guid::Empty
    IL_0013:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Guid>::.ctor(!0)
    IL_0018:  stobj      valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Guid>
    IL_001d:  ldloc.0
    IL_001e:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::NoOp(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Guid>[])
    IL_0023:  ret
  } // end of method TestCases::Issue953_MissingNullableSpecifierForArrayInitializer

  .method private hidebysig instance void 
          Issue907_Test3(string text) cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  4
    .locals init (class [mscorlib]System.Collections.Generic.Dictionary`2<string,object> V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,object>::.ctor()
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  ldstr      ""
    IL_0011:  ldarg.1
    IL_0012:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,object>::Add(!0,
                                                                                                                   !1)
    IL_0017:  ldloc.0
    IL_0018:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_001d:  ret
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
    // Code size       166 (0xa6)
    .maxstack  4
    .locals init (string[0...,0...] V_0)
    IL_0000:  ldc.i4.4
    IL_0001:  ldc.i4.4
    IL_0002:  newobj     instance void string[0...,0...]::.ctor(int32,
                                                                int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  ldc.i4.1
    IL_000b:  ldstr      "test"
    IL_0010:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.0
    IL_0017:  ldc.i4.2
    IL_0018:  ldstr      "hello"
    IL_001d:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0022:  ldloc.0
    IL_0023:  ldc.i4.0
    IL_0024:  ldc.i4.3
    IL_0025:  ldstr      "world"
    IL_002a:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_002f:  ldloc.0
    IL_0030:  ldc.i4.1
    IL_0031:  ldc.i4.0
    IL_0032:  ldstr      "test"
    IL_0037:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_003c:  ldloc.0
    IL_003d:  ldc.i4.1
    IL_003e:  ldc.i4.2
    IL_003f:  ldstr      "hello"
    IL_0044:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0049:  ldloc.0
    IL_004a:  ldc.i4.1
    IL_004b:  ldc.i4.3
    IL_004c:  ldstr      "world"
    IL_0051:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0056:  ldloc.0
    IL_0057:  ldc.i4.2
    IL_0058:  ldc.i4.0
    IL_0059:  ldstr      "test"
    IL_005e:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0063:  ldloc.0
    IL_0064:  ldc.i4.2
    IL_0065:  ldc.i4.1
    IL_0066:  ldstr      "hello"
    IL_006b:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0070:  ldloc.0
    IL_0071:  ldc.i4.2
    IL_0072:  ldc.i4.3
    IL_0073:  ldstr      "world"
    IL_0078:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_007d:  ldloc.0
    IL_007e:  ldc.i4.3
    IL_007f:  ldc.i4.0
    IL_0080:  ldstr      "test"
    IL_0085:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_008a:  ldloc.0
    IL_008b:  ldc.i4.3
    IL_008c:  ldc.i4.1
    IL_008d:  ldstr      "hello"
    IL_0092:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0097:  ldloc.0
    IL_0098:  ldc.i4.3
    IL_0099:  ldc.i4.2
    IL_009a:  ldstr      "world"
    IL_009f:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_00a4:  ldloc.0
    IL_00a5:  ret
  } // end of method TestCases::Issue1382a

  .method private hidebysig instance string[0...,0...] 
          Issue1382b() cil managed
  {
    // Code size       166 (0xa6)
    .maxstack  4
    .locals init (string[0...,0...] V_0)
    IL_0000:  ldc.i4.4
    IL_0001:  ldc.i4.4
    IL_0002:  newobj     instance void string[0...,0...]::.ctor(int32,
                                                                int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.0
    IL_000a:  ldc.i4.0
    IL_000b:  ldstr      "test"
    IL_0010:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.0
    IL_0017:  ldc.i4.1
    IL_0018:  ldstr      "hello"
    IL_001d:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0022:  ldloc.0
    IL_0023:  ldc.i4.0
    IL_0024:  ldc.i4.2
    IL_0025:  ldstr      "world"
    IL_002a:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_002f:  ldloc.0
    IL_0030:  ldc.i4.1
    IL_0031:  ldc.i4.0
    IL_0032:  ldstr      "test"
    IL_0037:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_003c:  ldloc.0
    IL_003d:  ldc.i4.1
    IL_003e:  ldc.i4.1
    IL_003f:  ldstr      "hello"
    IL_0044:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0049:  ldloc.0
    IL_004a:  ldc.i4.1
    IL_004b:  ldc.i4.3
    IL_004c:  ldstr      "world"
    IL_0051:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0056:  ldloc.0
    IL_0057:  ldc.i4.2
    IL_0058:  ldc.i4.0
    IL_0059:  ldstr      "test"
    IL_005e:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0063:  ldloc.0
    IL_0064:  ldc.i4.2
    IL_0065:  ldc.i4.2
    IL_0066:  ldstr      "hello"
    IL_006b:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0070:  ldloc.0
    IL_0071:  ldc.i4.2
    IL_0072:  ldc.i4.3
    IL_0073:  ldstr      "world"
    IL_0078:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_007d:  ldloc.0
    IL_007e:  ldc.i4.3
    IL_007f:  ldc.i4.1
    IL_0080:  ldstr      "test"
    IL_0085:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_008a:  ldloc.0
    IL_008b:  ldc.i4.3
    IL_008c:  ldc.i4.2
    IL_008d:  ldstr      "hello"
    IL_0092:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_0097:  ldloc.0
    IL_0098:  ldc.i4.3
    IL_0099:  ldc.i4.3
    IL_009a:  ldstr      "world"
    IL_009f:  call       instance void string[0...,0...]::Set(int32,
                                                              int32,
                                                              string)
    IL_00a4:  ldloc.0
    IL_00a5:  ret
  } // end of method TestCases::Issue1382b

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C 
          Test1() cil managed
  {
    // Code size       36 (0x24)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S>::.ctor()
    IL_000c:  stfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::L
    IL_0011:  ldloc.0
    IL_0012:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::L
    IL_0017:  ldc.i4.1
    IL_0018:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::.ctor(int32)
    IL_001d:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S>::Add(!0)
    IL_0022:  ldloc.0
    IL_0023:  ret
  } // end of method TestCases::Test1

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C 
          Test1Alternative() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C V_0,
             class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S> V_1)
    IL_0000:  ldc.i4.1
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S>::.ctor()
    IL_000d:  stloc.1
    IL_000e:  ldloc.1
    IL_000f:  ldc.i4.1
    IL_0010:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::.ctor(int32)
    IL_0015:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S>::Add(!0)
    IL_001a:  ldloc.1
    IL_001b:  stfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::L
    IL_0020:  ldloc.0
    IL_0021:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::TestCall(int32,
                                                                                                                                                                                             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C)
    IL_0026:  ret
  } // end of method TestCases::Test1Alternative

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C 
          Test2() cil managed
  {
    // Code size       22 (0x16)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Z
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.2
    IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Z
    IL_0014:  ldloc.0
    IL_0015:  ret
  } // end of method TestCases::Test2

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C 
          Test3() cil managed
  {
    // Code size       32 (0x20)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::.ctor(int32)
    IL_000d:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Y
    IL_0012:  ldloc.0
    IL_0013:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Y
    IL_0018:  ldc.i4.2
    IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::A
    IL_001e:  ldloc.0
    IL_001f:  ret
  } // end of method TestCases::Test3

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C 
          Test3b() cil managed
  {
    // Code size       33 (0x21)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Z
    IL_000e:  ldloc.0
    IL_000f:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Y
    IL_0014:  ldc.i4.2
    IL_0015:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::A
    IL_001a:  ldloc.0
    IL_001b:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::TestCall(int32,
                                                                                                                                                                                             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C)
    IL_0020:  ret
  } // end of method TestCases::Test3b

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C 
          Test4() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Y
    IL_000c:  ldc.i4.1
    IL_000d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::A
    IL_0012:  ldloc.0
    IL_0013:  ldc.i4.2
    IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Z
    IL_0019:  ldloc.0
    IL_001a:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Y
    IL_001f:  ldc.i4.3
    IL_0020:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/S::B
    IL_0025:  ldloc.0
    IL_0026:  ret
  } // end of method TestCases::Test4

  .method public hidebysig static void  ObjectInitializer() cil managed
  {
    // Code size       25 (0x19)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.0
    IL_000d:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0012:  ldloc.0
    IL_0013:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0018:  ret
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
    // Code size       53 (0x35)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldsfld     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'CS$<>9__CachedAnonymousMethodDelegate6'
    IL_000c:  brtrue.s   IL_001f

    IL_000e:  ldnull
    IL_000f:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'<NotAnObjectInitializerWithEvent>b__5'(object,
                                                                                                                                                class [mscorlib]System.EventArgs)
    IL_0015:  newobj     instance void [mscorlib]System.EventHandler::.ctor(object,
                                                                            native int)
    IL_001a:  stsfld     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'CS$<>9__CachedAnonymousMethodDelegate6'
    IL_001f:  ldsfld     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'CS$<>9__CachedAnonymousMethodDelegate6'
    IL_0024:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::add_TestEvent(class [mscorlib]System.EventHandler)
    IL_0029:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_002e:  ldloc.0
    IL_002f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0034:  ret
  } // end of method TestCases::NotAnObjectInitializerWithEvent

  .method public hidebysig static void  ObjectInitializerAssignCollectionToField() cil managed
  {
    // Code size       52 (0x34)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0,
             class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> V_1)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.0
    IL_000d:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0012:  ldloc.0
    IL_0013:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::.ctor()
    IL_0018:  stloc.1
    IL_0019:  ldloc.1
    IL_001a:  ldc.i4.0
    IL_001b:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_0020:  ldloc.1
    IL_0021:  ldc.i4.1
    IL_0022:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_0027:  ldloc.1
    IL_0028:  stfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::FieldList
    IL_002d:  ldloc.0
    IL_002e:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0033:  ret
  } // end of method TestCases::ObjectInitializerAssignCollectionToField

  .method public hidebysig static void  ObjectInitializerAddToCollectionInField() cil managed
  {
    // Code size       49 (0x31)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.0
    IL_000d:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0012:  ldloc.0
    IL_0013:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::FieldList
    IL_0018:  ldc.i4.0
    IL_0019:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_001e:  ldloc.0
    IL_001f:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::FieldList
    IL_0024:  ldc.i4.1
    IL_0025:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_002a:  ldloc.0
    IL_002b:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0030:  ret
  } // end of method TestCases::ObjectInitializerAddToCollectionInField

  .method public hidebysig static void  ObjectInitializerAssignCollectionToProperty() cil managed
  {
    // Code size       52 (0x34)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0,
             class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> V_1)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.0
    IL_000d:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0012:  ldloc.0
    IL_0013:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::.ctor()
    IL_0018:  stloc.1
    IL_0019:  ldloc.1
    IL_001a:  ldc.i4.0
    IL_001b:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_0020:  ldloc.1
    IL_0021:  ldc.i4.1
    IL_0022:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_0027:  ldloc.1
    IL_0028:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_PropertyList(class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>)
    IL_002d:  ldloc.0
    IL_002e:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0033:  ret
  } // end of method TestCases::ObjectInitializerAssignCollectionToProperty

  .method public hidebysig static void  ObjectInitializerAddToCollectionInProperty() cil managed
  {
    // Code size       49 (0x31)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.0
    IL_000d:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0012:  ldloc.0
    IL_0013:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_PropertyList()
    IL_0018:  ldc.i4.0
    IL_0019:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_001e:  ldloc.0
    IL_001f:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_PropertyList()
    IL_0024:  ldc.i4.1
    IL_0025:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_002a:  ldloc.0
    IL_002b:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0030:  ret
  } // end of method TestCases::ObjectInitializerAddToCollectionInProperty

  .method public hidebysig static void  ObjectInitializerWithInitializationOfNestedObjects() cil managed
  {
    // Code size       47 (0x2f)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0011:  ldc.i4.0
    IL_0012:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0017:  ldloc.0
    IL_0018:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_001d:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0022:  ldc.i4.1
    IL_0023:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0028:  ldloc.0
    IL_0029:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_002e:  ret
  } // end of method TestCases::ObjectInitializerWithInitializationOfNestedObjects

  .method public hidebysig static void  ObjectInitializerWithInitializationOfDeeplyNestedObjects() cil managed
  {
    // Code size       79 (0x4f)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.1
    IL_000d:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0012:  ldloc.0
    IL_0013:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0018:  ldc.i4.0
    IL_0019:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_001e:  ldloc.0
    IL_001f:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0024:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0029:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_002e:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0033:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0038:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_003d:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_MoreData()
    IL_0042:  ldc.i4.1
    IL_0043:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0048:  ldloc.0
    IL_0049:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_004e:  ret
  } // end of method TestCases::ObjectInitializerWithInitializationOfDeeplyNestedObjects

  .method public hidebysig static void  CollectionInitializerInsideObjectInitializers() cil managed
  {
    // Code size       57 (0x39)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_1)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_0011:  stloc.1
    IL_0012:  ldloc.1
    IL_0013:  ldc.i4.0
    IL_0014:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0019:  ldloc.1
    IL_001a:  ldc.i4.1
    IL_001b:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_b(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0020:  ldloc.1
    IL_0021:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::get_PropertyList()
    IL_0026:  ldc.i4.0
    IL_0027:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0)
    IL_002c:  ldloc.1
    IL_002d:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_MoreData(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data)
    IL_0032:  ldloc.0
    IL_0033:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0038:  ret
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
    // Code size       49 (0x31)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData V_1)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  ldloca.s   V_1
    IL_000e:  ldc.i4.2
    IL_000f:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::.ctor(int32)
    IL_0014:  ldloca.s   V_1
    IL_0016:  ldc.i4.1
    IL_0017:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::Field
    IL_001c:  ldloca.s   V_1
    IL_001e:  ldc.i4.2
    IL_001f:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData::set_Property(int32)
    IL_0024:  ldloc.1
    IL_0025:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_NestedStruct(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/StructData)
    IL_002a:  ldloc.0
    IL_002b:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0030:  ret
  } // end of method TestCases::StructInitializerWithinObjectInitializer

  .method public hidebysig static void  Issue270_NestedInitialisers() cil managed
  {
    // Code size       119 (0x77)
    .maxstack  6
    .locals init (class [mscorlib]System.Globalization.NumberFormatInfo[] V_0,
             class [mscorlib]System.Threading.Thread V_1,
             class [mscorlib]System.Globalization.CultureInfo V_2,
             class [mscorlib]System.Globalization.DateTimeFormatInfo V_3)
    IL_0000:  ldnull
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  ldnull
    IL_0004:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Issue270_NestedInitialisers()
    IL_000a:  newobj     instance void [mscorlib]System.Threading.ThreadStart::.ctor(object,
                                                                                     native int)
    IL_000f:  newobj     instance void [mscorlib]System.Threading.Thread::.ctor(class [mscorlib]System.Threading.ThreadStart)
    IL_0014:  stloc.1
    IL_0015:  ldloc.1
    IL_0016:  ldc.i4.1
    IL_0017:  callvirt   instance void [mscorlib]System.Threading.Thread::set_Priority(valuetype [mscorlib]System.Threading.ThreadPriority)
    IL_001c:  ldloc.1
    IL_001d:  ldc.i4.0
    IL_001e:  newobj     instance void [mscorlib]System.Globalization.CultureInfo::.ctor(int32)
    IL_0023:  stloc.2
    IL_0024:  ldloc.2
    IL_0025:  newobj     instance void [mscorlib]System.Globalization.DateTimeFormatInfo::.ctor()
    IL_002a:  stloc.3
    IL_002b:  ldloc.3
    IL_002c:  ldstr      "ddmmyy"
    IL_0031:  callvirt   instance void [mscorlib]System.Globalization.DateTimeFormatInfo::set_ShortDatePattern(string)
    IL_0036:  ldloc.3
    IL_0037:  callvirt   instance void [mscorlib]System.Globalization.CultureInfo::set_DateTimeFormat(class [mscorlib]System.Globalization.DateTimeFormatInfo)
    IL_003c:  ldloc.2
    IL_003d:  ldloc.0
    IL_003e:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'CS$<>9__CachedAnonymousMethodDelegate1a'
    IL_0043:  brtrue.s   IL_0056

    IL_0045:  ldnull
    IL_0046:  ldftn      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'<Issue270_NestedInitialisers>b__19'(class [mscorlib]System.Globalization.NumberFormatInfo)
    IL_004c:  newobj     instance void class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool>::.ctor(object,
                                                                                                                                        native int)
    IL_0051:  stsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'CS$<>9__CachedAnonymousMethodDelegate1a'
    IL_0056:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'CS$<>9__CachedAnonymousMethodDelegate1a'
    IL_005b:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class [mscorlib]System.Globalization.NumberFormatInfo>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                         class [mscorlib]System.Func`2<!!0,bool>)
    IL_0060:  call       !!0 [System.Core]System.Linq.Enumerable::First<class [mscorlib]System.Globalization.NumberFormatInfo>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0065:  callvirt   instance void [mscorlib]System.Globalization.CultureInfo::set_NumberFormat(class [mscorlib]System.Globalization.NumberFormatInfo)
    IL_006a:  ldloc.2
    IL_006b:  callvirt   instance void [mscorlib]System.Threading.Thread::set_CurrentCulture(class [mscorlib]System.Globalization.CultureInfo)
    IL_0070:  ldloc.1
    IL_0071:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::TestCall(int32,
                                                                                                                 class [mscorlib]System.Threading.Thread)
    IL_0076:  ret
  } // end of method TestCases::Issue270_NestedInitialisers

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2 
          Issue1345() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2 V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2::Data
    IL_000c:  ldc.i4.3
    IL_000d:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0012:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0017:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::set_Nullable(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>)
    IL_001c:  ldloc.0
    IL_001d:  ret
  } // end of method TestCases::Issue1345

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2 
          Issue1345b() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2 V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem2::get_Data2()
    IL_000c:  ldc.i4.3
    IL_000d:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0012:  newobj     instance void valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>::.ctor(!0)
    IL_0017:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::set_Nullable(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>)
    IL_001c:  ldloc.0
    IL_001d:  ret
  } // end of method TestCases::Issue1345b

  .method private hidebysig instance void 
          Issue1250_Test1(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum 'value') cil managed
  {
    // Code size       25 (0x19)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::.ctor()
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  ldarg.1
    IL_000d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/C::Z
    IL_0012:  ldloc.0
    IL_0013:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0018:  ret
  } // end of method TestCases::Issue1250_Test1

  .method private hidebysig instance uint8[] 
          Issue1314() cil managed
  {
    // Code size       18 (0x12)
    .maxstack  8
    IL_0000:  ldc.i4.4
    IL_0001:  newarr     [mscorlib]System.Byte
    IL_0006:  dup
    IL_0007:  ldtoken    field int32 '<PrivateImplementationDetails>'::'$$method0x6000045-1'
    IL_000c:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0011:  ret
  } // end of method TestCases::Issue1314

  .method private hidebysig instance void 
          Issue1251_Test(class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item> list,
                         class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem otherItem) cil managed
  {
    // Code size       154 (0x9a)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item V_0,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_1,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_2,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_3,
             valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> V_4)
    IL_0000:  ldarg.1
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldstr      "Text"
    IL_000d:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Text(string)
    IL_0012:  ldloc.0
    IL_0013:  ldarg.2
    IL_0014:  callvirt   instance valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Value()
    IL_0019:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value(valuetype [mscorlib]System.Decimal)
    IL_001e:  ldloc.0
    IL_001f:  ldarg.2
    IL_0020:  callvirt   instance valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Value2()
    IL_0025:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value2(valuetype [mscorlib]System.Decimal)
    IL_002a:  ldloc.0
    IL_002b:  ldarg.2
    IL_002c:  callvirt   instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Nullable()
    IL_0031:  stloc.1
    IL_0032:  ldloca.s   V_1
    IL_0034:  constrained. valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_003a:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_003f:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value3(string)
    IL_0044:  ldloc.0
    IL_0045:  ldarg.2
    IL_0046:  callvirt   instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Nullable2()
    IL_004b:  stloc.2
    IL_004c:  ldloca.s   V_2
    IL_004e:  constrained. valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0054:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0059:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value4(string)
    IL_005e:  ldloc.0
    IL_005f:  ldarg.2
    IL_0060:  callvirt   instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Nullable3()
    IL_0065:  stloc.3
    IL_0066:  ldloca.s   V_3
    IL_0068:  constrained. valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_006e:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0073:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value5(string)
    IL_0078:  ldloc.0
    IL_0079:  ldarg.2
    IL_007a:  callvirt   instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Nullable4()
    IL_007f:  stloc.s    V_4
    IL_0081:  ldloca.s   V_4
    IL_0083:  constrained. valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>
    IL_0089:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_008e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value6(string)
    IL_0093:  ldloc.0
    IL_0094:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item>::Add(!0)
    IL_0099:  ret
  } // end of method TestCases::Issue1251_Test

  .method private hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data 
          Issue1279(int32 p) cil managed
  {
    // Code size       39 (0x27)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data V_0)
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.1
    IL_0002:  bne.un.s   IL_0025

    IL_0004:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::.ctor()
    IL_0009:  stloc.0
    IL_000a:  ldloc.0
    IL_000b:  ldc.i4.0
    IL_000c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum)
    IL_0011:  ldloc.0
    IL_0012:  ldarg.0
    IL_0013:  ldftn      instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Data_TestEvent(object,
                                                                                                                                class [mscorlib]System.EventArgs)
    IL_0019:  newobj     instance void [mscorlib]System.EventHandler::.ctor(object,
                                                                            native int)
    IL_001e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Data::add_TestEvent(class [mscorlib]System.EventHandler)
    IL_0023:  ldloc.0
    IL_0024:  ret

    IL_0025:  ldnull
    IL_0026:  ret
  } // end of method TestCases::Issue1279

  .method public hidebysig static void  ExtensionMethodInCollectionInitializer() cil managed
  {
    // Code size       34 (0x22)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32> V_0)
    IL_0000:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "1"
    IL_000c:  ldstr      "2"
    IL_0011:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.Extensions::Add(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32>,
                                                                                                             string,
                                                                                                             string)
    IL_0016:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_001b:  ldloc.0
    IL_001c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0021:  ret
  } // end of method TestCases::ExtensionMethodInCollectionInitializer

  .method public hidebysig static void  NoCollectionInitializerBecauseOfTypeArguments() cil managed
  {
    // Code size       24 (0x18)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32> V_0)
    IL_0000:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "int"
    IL_000c:  callvirt   instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32>::Add<int32>(string)
    IL_0011:  ldloc.0
    IL_0012:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0017:  ret
  } // end of method TestCases::NoCollectionInitializerBecauseOfTypeArguments

  .method public hidebysig static void  CollectionInitializerWithParamsMethod() cil managed
  {
    // Code size       76 (0x4c)
    .maxstack  5
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32> V_0,
             int32[] V_1)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32>::.ctor()
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.s   10
    IL_000e:  newarr     [mscorlib]System.Int32
    IL_0013:  stloc.1
    IL_0014:  ldloc.1
    IL_0015:  ldc.i4.0
    IL_0016:  ldc.i4.1
    IL_0017:  stelem.i4
    IL_0018:  ldloc.1
    IL_0019:  ldc.i4.1
    IL_001a:  ldc.i4.2
    IL_001b:  stelem.i4
    IL_001c:  ldloc.1
    IL_001d:  ldc.i4.2
    IL_001e:  ldc.i4.3
    IL_001f:  stelem.i4
    IL_0020:  ldloc.1
    IL_0021:  ldc.i4.3
    IL_0022:  ldc.i4.4
    IL_0023:  stelem.i4
    IL_0024:  ldloc.1
    IL_0025:  ldc.i4.4
    IL_0026:  ldc.i4.5
    IL_0027:  stelem.i4
    IL_0028:  ldloc.1
    IL_0029:  ldc.i4.5
    IL_002a:  ldc.i4.6
    IL_002b:  stelem.i4
    IL_002c:  ldloc.1
    IL_002d:  ldc.i4.6
    IL_002e:  ldc.i4.7
    IL_002f:  stelem.i4
    IL_0030:  ldloc.1
    IL_0031:  ldc.i4.7
    IL_0032:  ldc.i4.8
    IL_0033:  stelem.i4
    IL_0034:  ldloc.1
    IL_0035:  ldc.i4.8
    IL_0036:  ldc.i4.s   9
    IL_0038:  stelem.i4
    IL_0039:  ldloc.1
    IL_003a:  ldc.i4.s   9
    IL_003c:  ldc.i4.s   10
    IL_003e:  stelem.i4
    IL_003f:  ldloc.1
    IL_0040:  callvirt   instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32>::Add(int32[])
    IL_0045:  ldloc.0
    IL_0046:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_004b:  ret
  } // end of method TestCases::CollectionInitializerWithParamsMethod

  .method public hidebysig static void  CollectionInitializerList() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.List`1<int32> V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.1
    IL_000d:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0012:  ldloc.0
    IL_0013:  ldc.i4.2
    IL_0014:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0019:  ldloc.0
    IL_001a:  ldc.i4.3
    IL_001b:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0020:  ldloc.0
    IL_0021:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0026:  ret
  } // end of method TestCases::CollectionInitializerList

  .method public hidebysig static object 
          RecursiveCollectionInitializer() cil managed
  {
    // Code size       15 (0xf)
    .maxstack  2
    .locals init (class [mscorlib]System.Collections.Generic.List`1<object> V_0)
    IL_0000:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<object>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldloc.0
    IL_0008:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<object>::Add(!0)
    IL_000d:  ldloc.0
    IL_000e:  ret
  } // end of method TestCases::RecursiveCollectionInitializer

  .method public hidebysig static void  CollectionInitializerDictionary() cil managed
  {
    // Code size       54 (0x36)
    .maxstack  4
    .locals init (class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::.ctor()
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  ldstr      "First"
    IL_0011:  ldc.i4.1
    IL_0012:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0017:  ldloc.0
    IL_0018:  ldstr      "Second"
    IL_001d:  ldc.i4.2
    IL_001e:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_0023:  ldloc.0
    IL_0024:  ldstr      "Third"
    IL_0029:  ldc.i4.3
    IL_002a:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::Add(!0,
                                                                                                                  !1)
    IL_002f:  ldloc.0
    IL_0030:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0035:  ret
  } // end of method TestCases::CollectionInitializerDictionary

  .method public hidebysig static void  CollectionInitializerDictionaryWithEnumTypes() cil managed
  {
    // Code size       34 (0x22)
    .maxstack  4
    .locals init (class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2> V_0)
    IL_0000:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0005:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::.ctor()
    IL_000a:  stloc.0
    IL_000b:  ldloc.0
    IL_000c:  ldc.i4.0
    IL_000d:  ldc.i4.0
    IL_000e:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0,
                                                                                                                                                                                                                                                                                          !1)
    IL_0013:  ldloc.0
    IL_0014:  ldc.i4.1
    IL_0015:  ldc.i4.1
    IL_0016:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/MyEnum2>::Add(!0,
                                                                                                                                                                                                                                                                                          !1)
    IL_001b:  ldloc.0
    IL_001c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0021:  ret
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
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  call       void [mscorlib]System.Console::WriteLine()
    IL_0005:  ret
  } // end of method TestCases::'<NotAnObjectInitializerWithEvent>b__5'

  .method private hidebysig static bool  '<Issue270_NestedInitialisers>b__19'(class [mscorlib]System.Globalization.NumberFormatInfo format) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance string [mscorlib]System.Globalization.NumberFormatInfo::get_CurrencySymbol()
    IL_0006:  ldstr      "$"
    IL_000b:  call       bool [mscorlib]System.String::op_Equality(string,
                                                                   string)
    IL_0010:  ret
  } // end of method TestCases::'<Issue270_NestedInitialisers>b__19'

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       2037 (0x7f5)
    .maxstack  8
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[] V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...] V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][] V_2,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[] V_3,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[] V_4,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[] V_5,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...] V_6,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[] V_7,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[] V_8,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[] V_9,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[] V_10,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[] V_11,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[] V_12,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[] V_13,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[] V_14,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[] V_15,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...][] V_16,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...] V_17,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...] V_18)
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
    IL_016b:  stloc.3
    IL_016c:  ldloc.3
    IL_016d:  ldc.i4.0
    IL_016e:  ldc.r4     1.
    IL_0173:  ldc.r4     1.
    IL_0178:  ldc.r4     1.
    IL_017d:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0182:  stelem.ref
    IL_0183:  ldloc.3
    IL_0184:  ldc.i4.1
    IL_0185:  ldc.r4     2.
    IL_018a:  ldc.r4     2.
    IL_018f:  ldc.r4     2.
    IL_0194:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0199:  stelem.ref
    IL_019a:  ldloc.3
    IL_019b:  ldc.i4.2
    IL_019c:  ldc.r4     3.
    IL_01a1:  ldc.r4     3.
    IL_01a6:  ldc.r4     3.
    IL_01ab:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_01b0:  stelem.ref
    IL_01b1:  ldloc.3
    IL_01b2:  stelem.ref
    IL_01b3:  ldloc.2
    IL_01b4:  ldc.i4.1
    IL_01b5:  ldc.i4.3
    IL_01b6:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_01bb:  stloc.s    V_4
    IL_01bd:  ldloc.s    V_4
    IL_01bf:  ldc.i4.0
    IL_01c0:  ldc.r4     2.
    IL_01c5:  ldc.r4     2.
    IL_01ca:  ldc.r4     2.
    IL_01cf:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_01d4:  stelem.ref
    IL_01d5:  ldloc.s    V_4
    IL_01d7:  ldc.i4.1
    IL_01d8:  ldc.r4     3.
    IL_01dd:  ldc.r4     3.
    IL_01e2:  ldc.r4     3.
    IL_01e7:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_01ec:  stelem.ref
    IL_01ed:  ldloc.s    V_4
    IL_01ef:  ldc.i4.2
    IL_01f0:  ldc.r4     4.
    IL_01f5:  ldc.r4     4.
    IL_01fa:  ldc.r4     4.
    IL_01ff:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0204:  stelem.ref
    IL_0205:  ldloc.s    V_4
    IL_0207:  stelem.ref
    IL_0208:  ldloc.2
    IL_0209:  ldc.i4.2
    IL_020a:  ldc.i4.3
    IL_020b:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_0210:  stloc.s    V_5
    IL_0212:  ldloc.s    V_5
    IL_0214:  ldc.i4.0
    IL_0215:  ldc.r4     3.
    IL_021a:  ldc.r4     3.
    IL_021f:  ldc.r4     3.
    IL_0224:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0229:  stelem.ref
    IL_022a:  ldloc.s    V_5
    IL_022c:  ldc.i4.1
    IL_022d:  ldc.r4     4.
    IL_0232:  ldc.r4     4.
    IL_0237:  ldc.r4     4.
    IL_023c:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0241:  stelem.ref
    IL_0242:  ldloc.s    V_5
    IL_0244:  ldc.i4.2
    IL_0245:  ldc.r4     5.
    IL_024a:  ldc.r4     5.
    IL_024f:  ldc.r4     5.
    IL_0254:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0259:  stelem.ref
    IL_025a:  ldloc.s    V_5
    IL_025c:  stelem.ref
    IL_025d:  ldloc.2
    IL_025e:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Issue1336_rg1b
    IL_0263:  ldc.i4.3
    IL_0264:  ldc.i4.3
    IL_0265:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::.ctor(int32,
                                                                                                                                              int32)
    IL_026a:  stloc.s    V_6
    IL_026c:  ldloc.s    V_6
    IL_026e:  ldc.i4.0
    IL_026f:  ldc.i4.0
    IL_0270:  ldc.i4.3
    IL_0271:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_0276:  stloc.s    V_7
    IL_0278:  ldloc.s    V_7
    IL_027a:  ldc.i4.0
    IL_027b:  ldc.r4     1.
    IL_0280:  ldc.r4     1.
    IL_0285:  ldc.r4     1.
    IL_028a:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_028f:  stelem.ref
    IL_0290:  ldloc.s    V_7
    IL_0292:  ldc.i4.1
    IL_0293:  ldc.r4     2.
    IL_0298:  ldc.r4     2.
    IL_029d:  ldc.r4     2.
    IL_02a2:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_02a7:  stelem.ref
    IL_02a8:  ldloc.s    V_7
    IL_02aa:  ldc.i4.2
    IL_02ab:  ldc.r4     3.
    IL_02b0:  ldc.r4     3.
    IL_02b5:  ldc.r4     3.
    IL_02ba:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_02bf:  stelem.ref
    IL_02c0:  ldloc.s    V_7
    IL_02c2:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_02c7:  ldloc.s    V_6
    IL_02c9:  ldc.i4.0
    IL_02ca:  ldc.i4.1
    IL_02cb:  ldc.i4.3
    IL_02cc:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_02d1:  stloc.s    V_8
    IL_02d3:  ldloc.s    V_8
    IL_02d5:  ldc.i4.0
    IL_02d6:  ldc.r4     2.
    IL_02db:  ldc.r4     2.
    IL_02e0:  ldc.r4     2.
    IL_02e5:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_02ea:  stelem.ref
    IL_02eb:  ldloc.s    V_8
    IL_02ed:  ldc.i4.1
    IL_02ee:  ldc.r4     3.
    IL_02f3:  ldc.r4     3.
    IL_02f8:  ldc.r4     3.
    IL_02fd:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0302:  stelem.ref
    IL_0303:  ldloc.s    V_8
    IL_0305:  ldc.i4.2
    IL_0306:  ldc.r4     4.
    IL_030b:  ldc.r4     4.
    IL_0310:  ldc.r4     4.
    IL_0315:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_031a:  stelem.ref
    IL_031b:  ldloc.s    V_8
    IL_031d:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_0322:  ldloc.s    V_6
    IL_0324:  ldc.i4.0
    IL_0325:  ldc.i4.2
    IL_0326:  ldc.i4.3
    IL_0327:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_032c:  stloc.s    V_9
    IL_032e:  ldloc.s    V_9
    IL_0330:  ldc.i4.0
    IL_0331:  ldc.r4     3.
    IL_0336:  ldc.r4     3.
    IL_033b:  ldc.r4     3.
    IL_0340:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0345:  stelem.ref
    IL_0346:  ldloc.s    V_9
    IL_0348:  ldc.i4.1
    IL_0349:  ldc.r4     4.
    IL_034e:  ldc.r4     4.
    IL_0353:  ldc.r4     4.
    IL_0358:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_035d:  stelem.ref
    IL_035e:  ldloc.s    V_9
    IL_0360:  ldc.i4.2
    IL_0361:  ldc.r4     5.
    IL_0366:  ldc.r4     5.
    IL_036b:  ldc.r4     5.
    IL_0370:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0375:  stelem.ref
    IL_0376:  ldloc.s    V_9
    IL_0378:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_037d:  ldloc.s    V_6
    IL_037f:  ldc.i4.1
    IL_0380:  ldc.i4.0
    IL_0381:  ldc.i4.3
    IL_0382:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_0387:  stloc.s    V_10
    IL_0389:  ldloc.s    V_10
    IL_038b:  ldc.i4.0
    IL_038c:  ldc.r4     1.
    IL_0391:  ldc.r4     1.
    IL_0396:  ldc.r4     1.
    IL_039b:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_03a0:  stelem.ref
    IL_03a1:  ldloc.s    V_10
    IL_03a3:  ldc.i4.1
    IL_03a4:  ldc.r4     2.
    IL_03a9:  ldc.r4     2.
    IL_03ae:  ldc.r4     2.
    IL_03b3:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_03b8:  stelem.ref
    IL_03b9:  ldloc.s    V_10
    IL_03bb:  ldc.i4.2
    IL_03bc:  ldc.r4     3.
    IL_03c1:  ldc.r4     3.
    IL_03c6:  ldc.r4     3.
    IL_03cb:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_03d0:  stelem.ref
    IL_03d1:  ldloc.s    V_10
    IL_03d3:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_03d8:  ldloc.s    V_6
    IL_03da:  ldc.i4.1
    IL_03db:  ldc.i4.1
    IL_03dc:  ldc.i4.3
    IL_03dd:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_03e2:  stloc.s    V_11
    IL_03e4:  ldloc.s    V_11
    IL_03e6:  ldc.i4.0
    IL_03e7:  ldc.r4     2.
    IL_03ec:  ldc.r4     2.
    IL_03f1:  ldc.r4     2.
    IL_03f6:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_03fb:  stelem.ref
    IL_03fc:  ldloc.s    V_11
    IL_03fe:  ldc.i4.1
    IL_03ff:  ldc.r4     3.
    IL_0404:  ldc.r4     3.
    IL_0409:  ldc.r4     3.
    IL_040e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0413:  stelem.ref
    IL_0414:  ldloc.s    V_11
    IL_0416:  ldc.i4.2
    IL_0417:  ldc.r4     4.
    IL_041c:  ldc.r4     4.
    IL_0421:  ldc.r4     4.
    IL_0426:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_042b:  stelem.ref
    IL_042c:  ldloc.s    V_11
    IL_042e:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_0433:  ldloc.s    V_6
    IL_0435:  ldc.i4.1
    IL_0436:  ldc.i4.2
    IL_0437:  ldc.i4.3
    IL_0438:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_043d:  stloc.s    V_12
    IL_043f:  ldloc.s    V_12
    IL_0441:  ldc.i4.0
    IL_0442:  ldc.r4     3.
    IL_0447:  ldc.r4     3.
    IL_044c:  ldc.r4     3.
    IL_0451:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0456:  stelem.ref
    IL_0457:  ldloc.s    V_12
    IL_0459:  ldc.i4.1
    IL_045a:  ldc.r4     4.
    IL_045f:  ldc.r4     4.
    IL_0464:  ldc.r4     4.
    IL_0469:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_046e:  stelem.ref
    IL_046f:  ldloc.s    V_12
    IL_0471:  ldc.i4.2
    IL_0472:  ldc.r4     5.
    IL_0477:  ldc.r4     5.
    IL_047c:  ldc.r4     5.
    IL_0481:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0486:  stelem.ref
    IL_0487:  ldloc.s    V_12
    IL_0489:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_048e:  ldloc.s    V_6
    IL_0490:  ldc.i4.2
    IL_0491:  ldc.i4.0
    IL_0492:  ldc.i4.3
    IL_0493:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_0498:  stloc.s    V_13
    IL_049a:  ldloc.s    V_13
    IL_049c:  ldc.i4.0
    IL_049d:  ldc.r4     1.
    IL_04a2:  ldc.r4     1.
    IL_04a7:  ldc.r4     1.
    IL_04ac:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_04b1:  stelem.ref
    IL_04b2:  ldloc.s    V_13
    IL_04b4:  ldc.i4.1
    IL_04b5:  ldc.r4     2.
    IL_04ba:  ldc.r4     2.
    IL_04bf:  ldc.r4     2.
    IL_04c4:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_04c9:  stelem.ref
    IL_04ca:  ldloc.s    V_13
    IL_04cc:  ldc.i4.2
    IL_04cd:  ldc.r4     3.
    IL_04d2:  ldc.r4     3.
    IL_04d7:  ldc.r4     3.
    IL_04dc:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_04e1:  stelem.ref
    IL_04e2:  ldloc.s    V_13
    IL_04e4:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_04e9:  ldloc.s    V_6
    IL_04eb:  ldc.i4.2
    IL_04ec:  ldc.i4.1
    IL_04ed:  ldc.i4.3
    IL_04ee:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_04f3:  stloc.s    V_14
    IL_04f5:  ldloc.s    V_14
    IL_04f7:  ldc.i4.0
    IL_04f8:  ldc.r4     2.
    IL_04fd:  ldc.r4     2.
    IL_0502:  ldc.r4     2.
    IL_0507:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_050c:  stelem.ref
    IL_050d:  ldloc.s    V_14
    IL_050f:  ldc.i4.1
    IL_0510:  ldc.r4     3.
    IL_0515:  ldc.r4     3.
    IL_051a:  ldc.r4     3.
    IL_051f:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0524:  stelem.ref
    IL_0525:  ldloc.s    V_14
    IL_0527:  ldc.i4.2
    IL_0528:  ldc.r4     4.
    IL_052d:  ldc.r4     4.
    IL_0532:  ldc.r4     4.
    IL_0537:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_053c:  stelem.ref
    IL_053d:  ldloc.s    V_14
    IL_053f:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_0544:  ldloc.s    V_6
    IL_0546:  ldc.i4.2
    IL_0547:  ldc.i4.2
    IL_0548:  ldc.i4.3
    IL_0549:  newarr     ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f
    IL_054e:  stloc.s    V_15
    IL_0550:  ldloc.s    V_15
    IL_0552:  ldc.i4.0
    IL_0553:  ldc.r4     3.
    IL_0558:  ldc.r4     3.
    IL_055d:  ldc.r4     3.
    IL_0562:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0567:  stelem.ref
    IL_0568:  ldloc.s    V_15
    IL_056a:  ldc.i4.1
    IL_056b:  ldc.r4     4.
    IL_0570:  ldc.r4     4.
    IL_0575:  ldc.r4     4.
    IL_057a:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_057f:  stelem.ref
    IL_0580:  ldloc.s    V_15
    IL_0582:  ldc.i4.2
    IL_0583:  ldc.r4     5.
    IL_0588:  ldc.r4     5.
    IL_058d:  ldc.r4     5.
    IL_0592:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0597:  stelem.ref
    IL_0598:  ldloc.s    V_15
    IL_059a:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...]::Set(int32,
                                                                                                                                            int32,
                                                                                                                                            class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[])
    IL_059f:  ldloc.s    V_6
    IL_05a1:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[][0...,0...] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Issue1336_rg1c
    IL_05a6:  ldc.i4.2
    IL_05a7:  newarr     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]
    IL_05ac:  stloc.s    V_16
    IL_05ae:  ldloc.s    V_16
    IL_05b0:  ldc.i4.0
    IL_05b1:  ldc.i4.3
    IL_05b2:  ldc.i4.3
    IL_05b3:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::.ctor(int32,
                                                                                                                                            int32)
    IL_05b8:  stloc.s    V_17
    IL_05ba:  ldloc.s    V_17
    IL_05bc:  ldc.i4.0
    IL_05bd:  ldc.i4.0
    IL_05be:  ldc.r4     1.
    IL_05c3:  ldc.r4     1.
    IL_05c8:  ldc.r4     1.
    IL_05cd:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_05d2:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_05d7:  ldloc.s    V_17
    IL_05d9:  ldc.i4.0
    IL_05da:  ldc.i4.1
    IL_05db:  ldc.r4     2.
    IL_05e0:  ldc.r4     2.
    IL_05e5:  ldc.r4     2.
    IL_05ea:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_05ef:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_05f4:  ldloc.s    V_17
    IL_05f6:  ldc.i4.0
    IL_05f7:  ldc.i4.2
    IL_05f8:  ldc.r4     3.
    IL_05fd:  ldc.r4     3.
    IL_0602:  ldc.r4     3.
    IL_0607:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_060c:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0611:  ldloc.s    V_17
    IL_0613:  ldc.i4.1
    IL_0614:  ldc.i4.0
    IL_0615:  ldc.r4     2.
    IL_061a:  ldc.r4     2.
    IL_061f:  ldc.r4     2.
    IL_0624:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0629:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_062e:  ldloc.s    V_17
    IL_0630:  ldc.i4.1
    IL_0631:  ldc.i4.1
    IL_0632:  ldc.r4     3.
    IL_0637:  ldc.r4     3.
    IL_063c:  ldc.r4     3.
    IL_0641:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0646:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_064b:  ldloc.s    V_17
    IL_064d:  ldc.i4.1
    IL_064e:  ldc.i4.2
    IL_064f:  ldc.r4     4.
    IL_0654:  ldc.r4     4.
    IL_0659:  ldc.r4     4.
    IL_065e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0663:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0668:  ldloc.s    V_17
    IL_066a:  ldc.i4.2
    IL_066b:  ldc.i4.0
    IL_066c:  ldc.r4     3.
    IL_0671:  ldc.r4     3.
    IL_0676:  ldc.r4     3.
    IL_067b:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0680:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0685:  ldloc.s    V_17
    IL_0687:  ldc.i4.2
    IL_0688:  ldc.i4.1
    IL_0689:  ldc.r4     4.
    IL_068e:  ldc.r4     4.
    IL_0693:  ldc.r4     4.
    IL_0698:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_069d:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_06a2:  ldloc.s    V_17
    IL_06a4:  ldc.i4.2
    IL_06a5:  ldc.i4.2
    IL_06a6:  ldc.r4     5.
    IL_06ab:  ldc.r4     5.
    IL_06b0:  ldc.r4     5.
    IL_06b5:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_06ba:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_06bf:  ldloc.s    V_17
    IL_06c1:  stelem.ref
    IL_06c2:  ldloc.s    V_16
    IL_06c4:  ldc.i4.1
    IL_06c5:  ldc.i4.3
    IL_06c6:  ldc.i4.3
    IL_06c7:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::.ctor(int32,
                                                                                                                                            int32)
    IL_06cc:  stloc.s    V_18
    IL_06ce:  ldloc.s    V_18
    IL_06d0:  ldc.i4.0
    IL_06d1:  ldc.i4.0
    IL_06d2:  ldc.r4     1.
    IL_06d7:  ldc.r4     1.
    IL_06dc:  ldc.r4     1.
    IL_06e1:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_06e6:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_06eb:  ldloc.s    V_18
    IL_06ed:  ldc.i4.0
    IL_06ee:  ldc.i4.1
    IL_06ef:  ldc.r4     2.
    IL_06f4:  ldc.r4     2.
    IL_06f9:  ldc.r4     2.
    IL_06fe:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0703:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0708:  ldloc.s    V_18
    IL_070a:  ldc.i4.0
    IL_070b:  ldc.i4.2
    IL_070c:  ldc.r4     3.
    IL_0711:  ldc.r4     3.
    IL_0716:  ldc.r4     3.
    IL_071b:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0720:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0725:  ldloc.s    V_18
    IL_0727:  ldc.i4.1
    IL_0728:  ldc.i4.0
    IL_0729:  ldc.r4     2.
    IL_072e:  ldc.r4     2.
    IL_0733:  ldc.r4     2.
    IL_0738:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_073d:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0742:  ldloc.s    V_18
    IL_0744:  ldc.i4.1
    IL_0745:  ldc.i4.1
    IL_0746:  ldc.r4     3.
    IL_074b:  ldc.r4     3.
    IL_0750:  ldc.r4     3.
    IL_0755:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_075a:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_075f:  ldloc.s    V_18
    IL_0761:  ldc.i4.1
    IL_0762:  ldc.i4.2
    IL_0763:  ldc.r4     4.
    IL_0768:  ldc.r4     4.
    IL_076d:  ldc.r4     4.
    IL_0772:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0777:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_077c:  ldloc.s    V_18
    IL_077e:  ldc.i4.2
    IL_077f:  ldc.i4.0
    IL_0780:  ldc.r4     3.
    IL_0785:  ldc.r4     3.
    IL_078a:  ldc.r4     3.
    IL_078f:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_0794:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_0799:  ldloc.s    V_18
    IL_079b:  ldc.i4.2
    IL_079c:  ldc.i4.1
    IL_079d:  ldc.r4     4.
    IL_07a2:  ldc.r4     4.
    IL_07a7:  ldc.r4     4.
    IL_07ac:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_07b1:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_07b6:  ldloc.s    V_18
    IL_07b8:  ldc.i4.2
    IL_07b9:  ldc.i4.2
    IL_07ba:  ldc.r4     5.
    IL_07bf:  ldc.r4     5.
    IL_07c4:  ldc.r4     5.
    IL_07c9:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f::.ctor(float32,
                                                                                                                           float32,
                                                                                                                           float32)
    IL_07ce:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...]::Set(int32,
                                                                                                                                          int32,
                                                                                                                                          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f)
    IL_07d3:  ldloc.s    V_18
    IL_07d5:  stelem.ref
    IL_07d6:  ldloc.s    V_16
    IL_07d8:  stsfld     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/V3f[0...,0...][] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Issue1336_rg1d
    IL_07dd:  ldc.i4.3
    IL_07de:  ldc.i4.3
    IL_07df:  newobj     instance void int32[0...,0...]::.ctor(int32,
                                                               int32)
    IL_07e4:  dup
    IL_07e5:  ldtoken    field valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=36' '<PrivateImplementationDetails>'::'$$method0x6000094-1'
    IL_07ea:  call       void [mscorlib]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [mscorlib]System.Array,
                                                                                                        valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_07ef:  stsfld     int32[0...,0...] ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Issue1336_rg2
    IL_07f4:  ret
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

  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '$$method0x600000b-1' at I_00002078
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '$$method0x600000d-1' at I_000020F0
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=24' '$$method0x600000d-2' at I_00002118
  .field static assembly int64 '$$method0x6000010-1' at I_000023D8
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=10' '$$method0x6000011-1' at I_00002400
  .field static assembly int64 '$$method0x6000012-1' at I_00002428
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=10' '$$method0x6000013-1' at I_00002450
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=12' '$$method0x6000014-1' at I_00002478
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '$$method0x6000015-1' at I_000024A8
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '$$method0x6000016-1' at I_000024F0
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=40' '$$method0x6000017-1' at I_00002538
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=80' '$$method0x6000018-1' at I_00002580
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=24' '$$method0x6000019-1' at I_000025F0
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=48' '$$method0x600001a-1' at I_00002628
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=256' '$$method0x600001e-1' at I_00002768
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' '$$method0x600001f-1' at I_00002880
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' '$$method0x600001f-2' at I_000028C0
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' '$$method0x600001f-3' at I_00002900
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=64' '$$method0x600001f-4' at I_00002940
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=72' '$$method0x6000020-1' at I_000029F0
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=72' '$$method0x6000020-2' at I_00002A38
  .field static assembly int32 '$$method0x6000045-1' at I_00003390
  .field static assembly valuetype '<PrivateImplementationDetails>'/'__StaticArrayInitTypeSize=36' '$$method0x6000094-1' at I_00003628
} // end of class '<PrivateImplementationDetails>'


// =============================================================

.data cil I_00002078 = bytearray (
                 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00
                 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00
                 09 00 00 00 0A 00 00 00) 
.data cil I_000020F0 = bytearray (
                 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00
                 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00
                 09 00 00 00 0A 00 00 00) 
.data cil I_00002118 = bytearray (
                 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00
                 05 00 00 00 06 00 00 00) 
.data cil I_000023D8 = bytearray (
                 01 00 01 00 00 00 01 01) 
.data cil I_000023E0 = int8[32]
.data cil I_00002400 = bytearray (
                 01 02 03 04 05 06 07 08 FE FF) 
.data cil I_0000240A = int8[6]
.data cil I_00002428 = bytearray (
                 80 81 00 01 02 03 04 7F) 
.data cil I_00002450 = bytearray (
                 00 80 FF FF 00 00 01 00 FF 7F) 
.data cil I_0000245A = int8[6]
.data cil I_00002478 = bytearray (
                 00 00 01 00 FF 7F 00 80 FE FF FF FF) 
.data cil I_00002484 = int8[4]
.data cil I_000024A8 = bytearray (
                 01 00 00 00 FE FF FF FF 00 94 35 77 04 00 00 00  // ..........5w....
                 05 00 00 00 FA FF FF FF 07 00 00 00 08 00 00 00
                 09 00 00 00 0A 00 00 00) 
.data cil I_000024F0 = bytearray (
                 01 00 00 00 00 94 35 77 00 5E D0 B2 04 00 00 00  // ......5w.^......
                 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00
                 09 00 00 00 0A 00 00 00) 
.data cil I_00002538 = bytearray (
                 01 00 0C BB 7D 6E 9C BA FF FF FF FF FF FF FF FF  // ....}n..........
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 FF FF F3 44 82 91 63 45)                         // ...D..cE
.data cil I_00002560 = int8[32]
.data cil I_00002580 = bytearray (
                 01 00 00 00 00 00 00 00 00 94 35 77 00 00 00 00  // ..........5w....
                 00 5E D0 B2 00 00 00 00 04 00 00 00 00 00 00 00  // .^..............
                 05 00 00 00 00 00 00 00 06 00 00 00 00 00 00 00
                 07 00 00 00 00 00 00 00 08 00 00 00 00 00 00 00
                 FF FF F3 44 82 91 63 45 FF FF E7 89 04 23 C7 8A) // ...D..cE.....#..
.data cil I_000025F0 = bytearray (
                 00 00 C0 BF 00 00 00 00 00 00 C0 3F 00 00 80 FF  // ...........?....
                 00 00 80 7F 00 00 C0 FF) 
.data cil I_00002628 = bytearray (
                 00 00 00 00 00 00 F8 BF 00 00 00 00 00 00 00 00
                 00 00 00 00 00 00 F8 3F 00 00 00 00 00 00 F0 FF  // .......?........
                 00 00 00 00 00 00 F0 7F 00 00 00 00 00 00 F8 FF) 
.data cil I_00002768 = bytearray (
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
.data cil I_00002868 = int8[24]
.data cil I_00002880 = bytearray (
                 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                 01 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00
                 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00) 
.data cil I_000028C0 = bytearray (
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00) 
.data cil I_00002900 = bytearray (
                 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                 01 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00
                 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00) 
.data cil I_00002940 = bytearray (
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00
                 00 00 00 00 00 00 00 00 01 00 00 00 00 00 00 00) 
.data cil I_000029F0 = bytearray (
                 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00
                 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00
                 09 00 00 00 0B 00 00 00 0C 00 00 00 0D 00 00 00
                 0E 00 00 00 0F 00 00 00 10 00 00 00 11 00 00 00
                 12 00 00 00 13 00 00 00) 
.data cil I_00002A38 = bytearray (
                 15 00 00 00 16 00 00 00 17 00 00 00 18 00 00 00
                 19 00 00 00 1A 00 00 00 1B 00 00 00 1C 00 00 00
                 1D 00 00 00 1F 00 00 00 20 00 00 00 21 00 00 00  // ........ ...!...
                 22 00 00 00 23 00 00 00 24 00 00 00 25 00 00 00  // "...#...$...%...
                 26 00 00 00 27 00 00 00)                         // &...'...
.data cil I_00003390 = bytearray (
                 00 01 02 FF) 
.data cil I_00003394 = int8[4]
.data cil I_00003628 = bytearray (
                 01 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00
                 01 00 00 00 01 00 00 00 01 00 00 00 01 00 00 00
                 01 00 00 00) 
// *********** DISASSEMBLY COMPLETE ***********************
