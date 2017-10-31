
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



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
.assembly '31sn40gd'
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module '31sn40gd.dll'
// MVID: {EE7CB403-8040-4C94-B17A-F697CE60CA23}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02FB0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests
       extends [mscorlib]System.Object
{
  .class auto ansi nested public beforefieldinit C
         extends [mscorlib]System.Object
  {
    .field public int32 Z
    .field public valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S Y
    .field public class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S> L
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method C::.ctor

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
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::A
      IL_0008:  ldarg.0
      IL_0009:  ldc.i4.0
      IL_000a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::B
      IL_000f:  ret
    } // end of method S::.ctor

  } // end of class S

  .class auto ansi sealed nested private MyEnum
         extends [mscorlib]System.Enum
  {
    .field public specialname rtspecialname int32 value__
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum a = int32(0x00000000)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum b = int32(0x00000001)
  } // end of class MyEnum

  .class auto ansi sealed nested private MyEnum2
         extends [mscorlib]System.Enum
  {
    .field public specialname rtspecialname int32 value__
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2 c = int32(0x00000000)
    .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2 d = int32(0x00000001)
  } // end of class MyEnum2

  .class auto ansi nested private beforefieldinit Data
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 04 49 74 65 6D 00 00 )                      // ...Item..
    .field public class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2> FieldList
    .field private class [mscorlib]System.EventHandler TestEvent
    .field private valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum '<a>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum '<b>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2> '<PropertyList>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data '<MoreData>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData '<NestedStruct>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig specialname 
            instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum 
            get_a() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::'<a>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method Data::get_a

    .method public hidebysig specialname 
            instance void  set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::'<a>k__BackingField'
      IL_0007:  ret
    } // end of method Data::set_a

    .method public hidebysig specialname 
            instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum 
            get_b() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::'<b>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method Data::get_b

    .method public hidebysig specialname 
            instance void  set_b(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::'<b>k__BackingField'
      IL_0007:  ret
    } // end of method Data::set_b

    .method public hidebysig specialname 
            instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2> 
            get_PropertyList() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2> V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::'<PropertyList>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method Data::get_PropertyList

    .method public hidebysig specialname 
            instance void  set_PropertyList(class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2> 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::'<PropertyList>k__BackingField'
      IL_0007:  ret
    } // end of method Data::set_PropertyList

    .method public hidebysig specialname 
            instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data 
            get_MoreData() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::'<MoreData>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method Data::get_MoreData

    .method public hidebysig specialname 
            instance void  set_MoreData(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::'<MoreData>k__BackingField'
      IL_0007:  ret
    } // end of method Data::set_MoreData

    .method public hidebysig specialname 
            instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData 
            get_NestedStruct() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::'<NestedStruct>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method Data::get_NestedStruct

    .method public hidebysig specialname 
            instance void  set_NestedStruct(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::'<NestedStruct>k__BackingField'
      IL_0007:  ret
    } // end of method Data::set_NestedStruct

    .method public hidebysig specialname 
            instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data 
            get_Item(int32 i) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  1
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data V_0)
      IL_0000:  nop
      IL_0001:  ldnull
      IL_0002:  stloc.0
      IL_0003:  br.s       IL_0005

      IL_0005:  ldloc.0
      IL_0006:  ret
    } // end of method Data::get_Item

    .method public hidebysig specialname 
            instance void  set_Item(int32 i,
                                    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data 'value') cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method Data::set_Item

    .method public hidebysig specialname 
            instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data 
            get_Item(int32 i,
                     string j) cil managed
    {
      // Code size       7 (0x7)
      .maxstack  1
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data V_0)
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
                                    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data 'value') cil managed
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
      IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::TestEvent
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
      IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::TestEvent
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
      IL_0001:  ldfld      class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::TestEvent
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
      IL_0017:  ldflda     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::TestEvent
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
      IL_0001:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2>::.ctor()
      IL_0006:  stfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::FieldList
      IL_000b:  ldarg.0
      IL_000c:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0011:  nop
      IL_0012:  ret
    } // end of method Data::.ctor

    .event [mscorlib]System.EventHandler TestEvent
    {
      .removeon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::remove_TestEvent(class [mscorlib]System.EventHandler)
      .addon instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::add_TestEvent(class [mscorlib]System.EventHandler)
    } // end of event Data::TestEvent
    .property instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum
            a()
    {
      .get instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::get_a()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum)
    } // end of property Data::a
    .property instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum
            b()
    {
      .get instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::get_b()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_b(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum)
    } // end of property Data::b
    .property instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2>
            PropertyList()
    {
      .get instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::get_PropertyList()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_PropertyList(class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2>)
    } // end of property Data::PropertyList
    .property instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data
            MoreData()
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_MoreData(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data)
      .get instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::get_MoreData()
    } // end of property Data::MoreData
    .property instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData
            NestedStruct()
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_NestedStruct(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData)
      .get instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::get_NestedStruct()
    } // end of property Data::NestedStruct
    .property instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data
            Item(int32)
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_Item(int32,
                                                                                                       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data)
      .get instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::get_Item(int32)
    } // end of property Data::Item
    .property instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data
            Item(int32,
                 string)
    {
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_Item(int32,
                                                                                                       string,
                                                                                                       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data)
      .get instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::get_Item(int32,
                                                                                                                                                                            string)
    } // end of property Data::Item
  } // end of class Data

  .class sequential ansi sealed nested private beforefieldinit StructData
         extends [mscorlib]System.ValueType
  {
    .field public int32 Field
    .field private int32 '<Property>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data '<MoreData>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig specialname 
            instance int32  get_Property() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (int32 V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::'<Property>k__BackingField'
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
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::'<Property>k__BackingField'
      IL_0007:  ret
    } // end of method StructData::set_Property

    .method public hidebysig specialname 
            instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data 
            get_MoreData() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::'<MoreData>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
    } // end of method StructData::get_MoreData

    .method public hidebysig specialname 
            instance void  set_MoreData(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::'<MoreData>k__BackingField'
      IL_0007:  ret
    } // end of method StructData::set_MoreData

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(int32 initialValue) cil managed
    {
      // Code size       24 (0x18)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData
      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::Field
      IL_000f:  ldarg.0
      IL_0010:  ldarg.1
      IL_0011:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::set_Property(int32)
      IL_0016:  nop
      IL_0017:  ret
    } // end of method StructData::.ctor

    .property instance int32 Property()
    {
      .get instance int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::get_Property()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::set_Property(int32)
    } // end of property StructData::Property
    .property instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data
            MoreData()
    {
      .get instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::get_MoreData()
      .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::set_MoreData(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data)
    } // end of property StructData::MoreData
  } // end of class StructData

  .field private static class [mscorlib]System.EventHandler 'CS$<>9__CachedAnonymousMethodDelegate8'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool> 'CS$<>9__CachedAnonymousMethodDelegate1c'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private hidebysig static void  X(object a,
                                           object b) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method InitializerTests::X

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
  } // end of method InitializerTests::Y

  .method public hidebysig static void  TestCall(int32 a,
                                                 class [mscorlib]System.Threading.Thread thread) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method InitializerTests::TestCall

  .method public hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C 
          TestCall(int32 a,
                   class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C c) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method InitializerTests::TestCall

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C 
          Test1() cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_1)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S>::.ctor()
    IL_000d:  stfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::L
    IL_0012:  ldloc.0
    IL_0013:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::L
    IL_0018:  ldc.i4.1
    IL_0019:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::.ctor(int32)
    IL_001e:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S>::Add(!0)
    IL_0023:  nop
    IL_0024:  ldloc.0
    IL_0025:  stloc.1
    IL_0026:  br.s       IL_0028

    IL_0028:  ldloc.1
    IL_0029:  ret
  } // end of method InitializerTests::Test1

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C 
          Test1Alternative() cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_0,
             class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S> V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_2)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::.ctor()
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S>::.ctor()
    IL_000e:  stloc.1
    IL_000f:  ldloc.1
    IL_0010:  ldc.i4.1
    IL_0011:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::.ctor(int32)
    IL_0016:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S>::Add(!0)
    IL_001b:  nop
    IL_001c:  ldloc.1
    IL_001d:  stfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::L
    IL_0022:  ldloc.0
    IL_0023:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::TestCall(int32,
                                                                                                                                                                         class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C)
    IL_0028:  stloc.2
    IL_0029:  br.s       IL_002b

    IL_002b:  ldloc.2
    IL_002c:  ret
  } // end of method InitializerTests::Test1Alternative

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C 
          Test2() cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_1)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Z
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.2
    IL_0010:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Z
    IL_0015:  ldloc.0
    IL_0016:  stloc.1
    IL_0017:  br.s       IL_0019

    IL_0019:  ldloc.1
    IL_001a:  ret
  } // end of method InitializerTests::Test2

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C 
          Test3() cil managed
  {
    // Code size       37 (0x25)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_1)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::.ctor(int32)
    IL_000e:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Y
    IL_0013:  ldloc.0
    IL_0014:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Y
    IL_0019:  ldc.i4.2
    IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::A
    IL_001f:  ldloc.0
    IL_0020:  stloc.1
    IL_0021:  br.s       IL_0023

    IL_0023:  ldloc.1
    IL_0024:  ret
  } // end of method InitializerTests::Test3

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C 
          Test3b() cil managed
  {
    // Code size       38 (0x26)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::.ctor()
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldc.i4.1
    IL_000a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Z
    IL_000f:  ldloc.0
    IL_0010:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Y
    IL_0015:  ldc.i4.2
    IL_0016:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::A
    IL_001b:  ldloc.0
    IL_001c:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::TestCall(int32,
                                                                                                                                                                         class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C)
    IL_0021:  stloc.1
    IL_0022:  br.s       IL_0024

    IL_0024:  ldloc.1
    IL_0025:  ret
  } // end of method InitializerTests::Test3b

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C 
          Test4() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_1)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Y
    IL_000d:  ldc.i4.1
    IL_000e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::A
    IL_0013:  ldloc.0
    IL_0014:  ldc.i4.2
    IL_0015:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Z
    IL_001a:  ldloc.0
    IL_001b:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Y
    IL_0020:  ldc.i4.3
    IL_0021:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::B
    IL_0026:  ldloc.0
    IL_0027:  stloc.1
    IL_0028:  br.s       IL_002a

    IL_002a:  ldloc.1
    IL_002b:  ret
  } // end of method InitializerTests::Test4

  .method public hidebysig static void  CollectionInitializerList() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  3
    .locals init (class [mscorlib]System.Collections.Generic.List`1<int32> V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::Y()
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
    IL_0025:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::X(object,
                                                                                                object)
    IL_002a:  nop
    IL_002b:  ret
  } // end of method InitializerTests::CollectionInitializerList

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
  } // end of method InitializerTests::RecursiveCollectionInitializer

  .method public hidebysig static void  CollectionInitializerDictionary() cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  4
    .locals init (class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::Y()
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
    IL_0034:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::X(object,
                                                                                                object)
    IL_0039:  nop
    IL_003a:  ret
  } // end of method InitializerTests::CollectionInitializerDictionary

  .method public hidebysig static void  CollectionInitializerDictionaryWithEnumTypes() cil managed
  {
    // Code size       38 (0x26)
    .maxstack  4
    .locals init (class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2> V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::Y()
    IL_0006:  newobj     instance void class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2>::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.0
    IL_000e:  ldc.i4.0
    IL_000f:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2>::Add(!0,
                                                                                                                                                                                                                                                                      !1)
    IL_0014:  nop
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.1
    IL_0017:  ldc.i4.1
    IL_0018:  callvirt   instance void class [mscorlib]System.Collections.Generic.Dictionary`2<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2>::Add(!0,
                                                                                                                                                                                                                                                                      !1)
    IL_001d:  nop
    IL_001e:  ldloc.0
    IL_001f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::X(object,
                                                                                                object)
    IL_0024:  nop
    IL_0025:  ret
  } // end of method InitializerTests::CollectionInitializerDictionaryWithEnumTypes

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
    IL_001f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::Y()
    IL_0024:  ldloc.0
    IL_0025:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::X(object,
                                                                                                object)
    IL_002a:  nop
    IL_002b:  ret
  } // end of method InitializerTests::NotACollectionInitializer

  .method public hidebysig static void  ObjectInitializer() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::Y()
    IL_0006:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.0
    IL_000e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum)
    IL_0013:  nop
    IL_0014:  ldloc.0
    IL_0015:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::X(object,
                                                                                                object)
    IL_001a:  nop
    IL_001b:  ret
  } // end of method InitializerTests::ObjectInitializer

  .method public hidebysig static void  NotAnObjectInitializer() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data V_0)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.0
    IL_0009:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum)
    IL_000e:  nop
    IL_000f:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::Y()
    IL_0014:  ldloc.0
    IL_0015:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::X(object,
                                                                                                object)
    IL_001a:  nop
    IL_001b:  ret
  } // end of method InitializerTests::NotAnObjectInitializer

  .method public hidebysig static void  NotAnObjectInitializerWithEvent() cil managed
  {
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data V_0)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldsfld     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::'CS$<>9__CachedAnonymousMethodDelegate8'
    IL_000d:  brtrue.s   IL_0022

    IL_000f:  ldnull
    IL_0010:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::'<NotAnObjectInitializerWithEvent>b__7'(object,
                                                                                                                                      class [mscorlib]System.EventArgs)
    IL_0016:  newobj     instance void [mscorlib]System.EventHandler::.ctor(object,
                                                                            native int)
    IL_001b:  stsfld     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::'CS$<>9__CachedAnonymousMethodDelegate8'
    IL_0020:  br.s       IL_0022

    IL_0022:  ldsfld     class [mscorlib]System.EventHandler ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::'CS$<>9__CachedAnonymousMethodDelegate8'
    IL_0027:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::add_TestEvent(class [mscorlib]System.EventHandler)
    IL_002c:  nop
    IL_002d:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::Y()
    IL_0032:  ldloc.0
    IL_0033:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::X(object,
                                                                                                object)
    IL_0038:  nop
    IL_0039:  ret
  } // end of method InitializerTests::NotAnObjectInitializerWithEvent

  .method public hidebysig static void  ObjectInitializerAssignCollectionToField() cil managed
  {
    // Code size       57 (0x39)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data V_0,
             class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2> V_1)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::Y()
    IL_0006:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.0
    IL_000e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum)
    IL_0013:  nop
    IL_0014:  ldloc.0
    IL_0015:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2>::.ctor()
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4.0
    IL_001d:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2>::Add(!0)
    IL_0022:  nop
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.1
    IL_0025:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2>::Add(!0)
    IL_002a:  nop
    IL_002b:  ldloc.1
    IL_002c:  stfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::FieldList
    IL_0031:  ldloc.0
    IL_0032:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::X(object,
                                                                                                object)
    IL_0037:  nop
    IL_0038:  ret
  } // end of method InitializerTests::ObjectInitializerAssignCollectionToField

  .method public hidebysig static void  ObjectInitializerAddToCollectionInField() cil managed
  {
    // Code size       54 (0x36)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::Y()
    IL_0006:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.0
    IL_000e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum)
    IL_0013:  nop
    IL_0014:  ldloc.0
    IL_0015:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::FieldList
    IL_001a:  ldc.i4.0
    IL_001b:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2>::Add(!0)
    IL_0020:  nop
    IL_0021:  ldloc.0
    IL_0022:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::FieldList
    IL_0027:  ldc.i4.1
    IL_0028:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2>::Add(!0)
    IL_002d:  nop
    IL_002e:  ldloc.0
    IL_002f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::X(object,
                                                                                                object)
    IL_0034:  nop
    IL_0035:  ret
  } // end of method InitializerTests::ObjectInitializerAddToCollectionInField

  .method public hidebysig static void  ObjectInitializerAssignCollectionToProperty() cil managed
  {
    // Code size       58 (0x3a)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data V_0,
             class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2> V_1)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::Y()
    IL_0006:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.0
    IL_000e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum)
    IL_0013:  nop
    IL_0014:  ldloc.0
    IL_0015:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2>::.ctor()
    IL_001a:  stloc.1
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4.0
    IL_001d:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2>::Add(!0)
    IL_0022:  nop
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.1
    IL_0025:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2>::Add(!0)
    IL_002a:  nop
    IL_002b:  ldloc.1
    IL_002c:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_PropertyList(class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2>)
    IL_0031:  nop
    IL_0032:  ldloc.0
    IL_0033:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::X(object,
                                                                                                object)
    IL_0038:  nop
    IL_0039:  ret
  } // end of method InitializerTests::ObjectInitializerAssignCollectionToProperty

  .method public hidebysig static void  ObjectInitializerAddToCollectionInProperty() cil managed
  {
    // Code size       54 (0x36)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::Y()
    IL_0006:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.0
    IL_000e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum)
    IL_0013:  nop
    IL_0014:  ldloc.0
    IL_0015:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::get_PropertyList()
    IL_001a:  ldc.i4.0
    IL_001b:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2>::Add(!0)
    IL_0020:  nop
    IL_0021:  ldloc.0
    IL_0022:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::get_PropertyList()
    IL_0027:  ldc.i4.1
    IL_0028:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2>::Add(!0)
    IL_002d:  nop
    IL_002e:  ldloc.0
    IL_002f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::X(object,
                                                                                                object)
    IL_0034:  nop
    IL_0035:  ret
  } // end of method InitializerTests::ObjectInitializerAddToCollectionInProperty

  .method public hidebysig static void  ObjectInitializerWithInitializationOfNestedObjects() cil managed
  {
    // Code size       51 (0x33)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::Y()
    IL_0006:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::get_MoreData()
    IL_0012:  ldc.i4.0
    IL_0013:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum)
    IL_0018:  nop
    IL_0019:  ldloc.0
    IL_001a:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::get_MoreData()
    IL_001f:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::get_MoreData()
    IL_0024:  ldc.i4.1
    IL_0025:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum)
    IL_002a:  nop
    IL_002b:  ldloc.0
    IL_002c:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::X(object,
                                                                                                object)
    IL_0031:  nop
    IL_0032:  ret
  } // end of method InitializerTests::ObjectInitializerWithInitializationOfNestedObjects

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
  } // end of method InitializerTests::GetInt

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
  } // end of method InitializerTests::GetString

  .method private hidebysig static void  NoOp(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Guid>[] 'array') cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method InitializerTests::NoOp

  .method public hidebysig static void  ObjectInitializerWithInitializationOfDeeplyNestedObjects() cil managed
  {
    // Code size       84 (0x54)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::Y()
    IL_0006:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldc.i4.1
    IL_000e:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum)
    IL_0013:  nop
    IL_0014:  ldloc.0
    IL_0015:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::get_MoreData()
    IL_001a:  ldc.i4.0
    IL_001b:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum)
    IL_0020:  nop
    IL_0021:  ldloc.0
    IL_0022:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::get_MoreData()
    IL_0027:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::get_MoreData()
    IL_002c:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::get_MoreData()
    IL_0031:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::get_MoreData()
    IL_0036:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::get_MoreData()
    IL_003b:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::get_MoreData()
    IL_0040:  callvirt   instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::get_MoreData()
    IL_0045:  ldc.i4.1
    IL_0046:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum)
    IL_004b:  nop
    IL_004c:  ldloc.0
    IL_004d:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::X(object,
                                                                                                object)
    IL_0052:  nop
    IL_0053:  ret
  } // end of method InitializerTests::ObjectInitializerWithInitializationOfDeeplyNestedObjects

  .method public hidebysig static void  CollectionInitializerInsideObjectInitializers() cil managed
  {
    // Code size       63 (0x3f)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data V_1)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::Y()
    IL_0006:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::.ctor()
    IL_0012:  stloc.1
    IL_0013:  ldloc.1
    IL_0014:  ldc.i4.0
    IL_0015:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum)
    IL_001a:  nop
    IL_001b:  ldloc.1
    IL_001c:  ldc.i4.1
    IL_001d:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_b(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum)
    IL_0022:  nop
    IL_0023:  ldloc.1
    IL_0024:  callvirt   instance class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::get_PropertyList()
    IL_0029:  ldc.i4.0
    IL_002a:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2>::Add(!0)
    IL_002f:  nop
    IL_0030:  ldloc.1
    IL_0031:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_MoreData(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data)
    IL_0036:  nop
    IL_0037:  ldloc.0
    IL_0038:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::X(object,
                                                                                                object)
    IL_003d:  nop
    IL_003e:  ret
  } // end of method InitializerTests::CollectionInitializerInsideObjectInitializers

  .method public hidebysig static void  NotAStructInitializer_DefaultConstructor() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData V_0)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_0
    IL_0003:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData
    IL_0009:  ldloca.s   V_0
    IL_000b:  ldc.i4.1
    IL_000c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::Field
    IL_0011:  ldloca.s   V_0
    IL_0013:  ldc.i4.2
    IL_0014:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::set_Property(int32)
    IL_0019:  nop
    IL_001a:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::Y()
    IL_001f:  ldloc.0
    IL_0020:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData
    IL_0025:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::X(object,
                                                                                                object)
    IL_002a:  nop
    IL_002b:  ret
  } // end of method InitializerTests::NotAStructInitializer_DefaultConstructor

  .method public hidebysig static void  StructInitializer_DefaultConstructor() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  3
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::Y()
    IL_0006:  ldloca.s   V_0
    IL_0008:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData
    IL_000e:  ldloca.s   V_0
    IL_0010:  ldc.i4.1
    IL_0011:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::Field
    IL_0016:  ldloca.s   V_0
    IL_0018:  ldc.i4.2
    IL_0019:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::set_Property(int32)
    IL_001e:  nop
    IL_001f:  ldloc.0
    IL_0020:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData
    IL_0025:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::X(object,
                                                                                                object)
    IL_002a:  nop
    IL_002b:  ret
  } // end of method InitializerTests::StructInitializer_DefaultConstructor

  .method public hidebysig static void  NotAStructInitializer_ExplicitConstructor() cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData V_0)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_0
    IL_0003:  ldc.i4.0
    IL_0004:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::.ctor(int32)
    IL_0009:  nop
    IL_000a:  ldloca.s   V_0
    IL_000c:  ldc.i4.1
    IL_000d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::Field
    IL_0012:  ldloca.s   V_0
    IL_0014:  ldc.i4.2
    IL_0015:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::set_Property(int32)
    IL_001a:  nop
    IL_001b:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::Y()
    IL_0020:  ldloc.0
    IL_0021:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData
    IL_0026:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::X(object,
                                                                                                object)
    IL_002b:  nop
    IL_002c:  ret
  } // end of method InitializerTests::NotAStructInitializer_ExplicitConstructor

  .method public hidebysig static void  StructInitializer_ExplicitConstructor() cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  3
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::Y()
    IL_0006:  ldloca.s   V_0
    IL_0008:  ldc.i4.0
    IL_0009:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::.ctor(int32)
    IL_000e:  nop
    IL_000f:  ldloca.s   V_0
    IL_0011:  ldc.i4.1
    IL_0012:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::Field
    IL_0017:  ldloca.s   V_0
    IL_0019:  ldc.i4.2
    IL_001a:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::set_Property(int32)
    IL_001f:  nop
    IL_0020:  ldloc.0
    IL_0021:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData
    IL_0026:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::X(object,
                                                                                                object)
    IL_002b:  nop
    IL_002c:  ret
  } // end of method InitializerTests::StructInitializer_ExplicitConstructor

  .method public hidebysig static void  StructInitializerWithInitializationOfNestedObjects() cil managed
  {
    // Code size       79 (0x4f)
    .maxstack  3
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData V_0)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::Y()
    IL_0006:  ldloca.s   V_0
    IL_0008:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData
    IL_000e:  ldloca.s   V_0
    IL_0010:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::get_MoreData()
    IL_0015:  ldc.i4.0
    IL_0016:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_a(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum)
    IL_001b:  nop
    IL_001c:  ldloca.s   V_0
    IL_001e:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::get_MoreData()
    IL_0023:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::FieldList
    IL_0028:  ldc.i4.0
    IL_0029:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2>::Add(!0)
    IL_002e:  nop
    IL_002f:  ldloca.s   V_0
    IL_0031:  call       instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::get_MoreData()
    IL_0036:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::FieldList
    IL_003b:  ldc.i4.1
    IL_003c:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/MyEnum2>::Add(!0)
    IL_0041:  nop
    IL_0042:  ldloc.0
    IL_0043:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData
    IL_0048:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::X(object,
                                                                                                object)
    IL_004d:  nop
    IL_004e:  ret
  } // end of method InitializerTests::StructInitializerWithInitializationOfNestedObjects

  .method public hidebysig static void  StructInitializerWithinObjectInitializer() cil managed
  {
    // Code size       54 (0x36)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData V_1)
    IL_0000:  nop
    IL_0001:  call       object ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::Y()
    IL_0006:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::.ctor()
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  ldloca.s   V_1
    IL_000f:  ldc.i4.2
    IL_0010:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::.ctor(int32)
    IL_0015:  nop
    IL_0016:  ldloca.s   V_1
    IL_0018:  ldc.i4.1
    IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::Field
    IL_001e:  ldloca.s   V_1
    IL_0020:  ldc.i4.2
    IL_0021:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData::set_Property(int32)
    IL_0026:  nop
    IL_0027:  ldloc.1
    IL_0028:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/Data::set_NestedStruct(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/StructData)
    IL_002d:  nop
    IL_002e:  ldloc.0
    IL_002f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::X(object,
                                                                                                object)
    IL_0034:  nop
    IL_0035:  ret
  } // end of method InitializerTests::StructInitializerWithinObjectInitializer

  .method public hidebysig static void  Bug270_NestedInitialisers() cil managed
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
    IL_0005:  ldftn      void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::Bug270_NestedInitialisers()
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
    IL_0042:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::'CS$<>9__CachedAnonymousMethodDelegate1c'
    IL_0047:  brtrue.s   IL_005c

    IL_0049:  ldnull
    IL_004a:  ldftn      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::'<Bug270_NestedInitialisers>b__1b'(class [mscorlib]System.Globalization.NumberFormatInfo)
    IL_0050:  newobj     instance void class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool>::.ctor(object,
                                                                                                                                        native int)
    IL_0055:  stsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::'CS$<>9__CachedAnonymousMethodDelegate1c'
    IL_005a:  br.s       IL_005c

    IL_005c:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Globalization.NumberFormatInfo,bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::'CS$<>9__CachedAnonymousMethodDelegate1c'
    IL_0061:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Where<class [mscorlib]System.Globalization.NumberFormatInfo>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                         class [mscorlib]System.Func`2<!!0,bool>)
    IL_0066:  call       !!0 [System.Core]System.Linq.Enumerable::First<class [mscorlib]System.Globalization.NumberFormatInfo>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_006b:  callvirt   instance void [mscorlib]System.Globalization.CultureInfo::set_NumberFormat(class [mscorlib]System.Globalization.NumberFormatInfo)
    IL_0070:  nop
    IL_0071:  ldloc.2
    IL_0072:  callvirt   instance void [mscorlib]System.Threading.Thread::set_CurrentCulture(class [mscorlib]System.Globalization.CultureInfo)
    IL_0077:  nop
    IL_0078:  ldloc.1
    IL_0079:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::TestCall(int32,
                                                                                                       class [mscorlib]System.Threading.Thread)
    IL_007e:  nop
    IL_007f:  ret
  } // end of method InitializerTests::Bug270_NestedInitialisers

  .method public hidebysig static void  Bug953_MissingNullableSpecifierForArrayInitializer() cil managed
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
    IL_001f:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::NoOp(valuetype [mscorlib]System.Nullable`1<valuetype [mscorlib]System.Guid>[])
    IL_0024:  nop
    IL_0025:  ret
  } // end of method InitializerTests::Bug953_MissingNullableSpecifierForArrayInitializer

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method InitializerTests::.ctor

  .method private hidebysig static void  '<NotAnObjectInitializerWithEvent>b__7'(object sender,
                                                                                 class [mscorlib]System.EventArgs e) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       void [mscorlib]System.Console::WriteLine()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method InitializerTests::'<NotAnObjectInitializerWithEvent>b__7'

  .method private hidebysig static bool  '<Bug270_NestedInitialisers>b__1b'(class [mscorlib]System.Globalization.NumberFormatInfo format) cil managed
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
  } // end of method InitializerTests::'<Bug270_NestedInitialisers>b__1b'

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\InitializerTests.res
