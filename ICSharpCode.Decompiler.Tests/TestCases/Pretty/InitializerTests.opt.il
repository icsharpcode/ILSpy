



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
  .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
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
                                            int32 a,
                                            int32 b) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
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
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::get_Text()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Text(string)
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
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::get_Value3()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value3(string)
    } // end of property Item::Value3
    .property instance string Value4()
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value4(string)
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::get_Value4()
    } // end of property Item::Value4
    .property instance string Value5()
    {
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::get_Value5()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value5(string)
    } // end of property Item::Value5
    .property instance string Value6()
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::set_Value6(string)
      .get instance string ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/Item::get_Value6()
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
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::set_Value(valuetype [mscorlib]System.Decimal)
      .get instance valuetype [mscorlib]System.Decimal ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Value()
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
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::set_Nullable(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal>)
      .get instance valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Decimal> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/OtherItem::get_Nullable()
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

  .field private static class [mscorlib]System.EventHandler 'CS$<>9__CachedAnonymousMethodDelegate9'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool> 'CS$<>9__CachedAnonymousMethodDelegate1d'
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

  .method public hidebysig static void  ExtensionMethodInCollectionInitializer() cil managed
  {
    // Code size       40 (0x28)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32> V_0,
             int32[] V_1)
    IL_0000:  newobj     instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32>::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.2
    IL_0008:  newarr     [mscorlib]System.Int32
    IL_000d:  stloc.1
    IL_000e:  ldloc.1
    IL_000f:  ldc.i4.0
    IL_0010:  ldc.i4.1
    IL_0011:  stelem.i4
    IL_0012:  ldloc.1
    IL_0013:  ldc.i4.1
    IL_0014:  ldc.i4.2
    IL_0015:  stelem.i4
    IL_0016:  ldloc.1
    IL_0017:  callvirt   instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases/CustomList`1<int32>::Add(int32[])
    IL_001c:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Y()
    IL_0021:  ldloc.0
    IL_0022:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::X(object,
                                                                                                          object)
    IL_0027:  ret
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
    IL_0007:  ldsfld     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'CS$<>9__CachedAnonymousMethodDelegate9'
    IL_000c:  brtrue.s   IL_001f

    IL_000e:  ldnull
    IL_000f:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'<NotAnObjectInitializerWithEvent>b__8'(object,
                                                                                                                                                class [mscorlib]System.EventArgs)
    IL_0015:  newobj     instance void [mscorlib]System.EventHandler::.ctor(object,
                                                                            native int)
    IL_001a:  stsfld     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'CS$<>9__CachedAnonymousMethodDelegate9'
    IL_001f:  ldsfld     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'CS$<>9__CachedAnonymousMethodDelegate9'
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

  .method public hidebysig static void  Bug270_NestedInitialisers() cil managed
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
    IL_0004:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::Bug270_NestedInitialisers()
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
    IL_003e:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'CS$<>9__CachedAnonymousMethodDelegate1d'
    IL_0043:  brtrue.s   IL_0056

    IL_0045:  ldnull
    IL_0046:  ldftn      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'<Bug270_NestedInitialisers>b__1c'(class [mscorlib]System.Globalization.NumberFormatInfo)
    IL_004c:  newobj     instance void class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool>::.ctor(object,
                                                                                                                                        native int)
    IL_0051:  stsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'CS$<>9__CachedAnonymousMethodDelegate1d'
    IL_0056:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases::'CS$<>9__CachedAnonymousMethodDelegate1d'
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
  } // end of method TestCases::Bug270_NestedInitialisers

  .method public hidebysig static void  Bug953_MissingNullableSpecifierForArrayInitializer() cil managed
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
  } // end of method TestCases::Bug953_MissingNullableSpecifierForArrayInitializer

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

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method TestCases::.ctor

  .method private hidebysig static void  '<NotAnObjectInitializerWithEvent>b__8'(object param0,
                                                                                 class [mscorlib]System.EventArgs param1) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  call       void [mscorlib]System.Console::WriteLine()
    IL_0005:  ret
  } // end of method TestCases::'<NotAnObjectInitializerWithEvent>b__8'

  .method private hidebysig static bool  '<Bug270_NestedInitialisers>b__1c'(class [mscorlib]System.Globalization.NumberFormatInfo format) cil managed
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
  } // end of method TestCases::'<Bug270_NestedInitialisers>b__1c'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests.TestCases


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
