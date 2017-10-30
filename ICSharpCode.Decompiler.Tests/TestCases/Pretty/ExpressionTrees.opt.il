
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
.assembly bkzeipao
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module bkzeipao.dll
// MVID: {634B5BC8-C1F8-4368-9CEE-22DBE0986803}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x04EC0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ExpressionTrees
       extends [mscorlib]System.Object
{
  .class auto ansi nested private beforefieldinit XmlReaderSettings
         extends [mscorlib]System.Object
  {
    .field assembly bool CloseInput
    .field assembly bool CheckCharacters
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method XmlReaderSettings::.ctor

  } // end of class XmlReaderSettings

  .class auto ansi nested private beforefieldinit GenericClass`1<X>
         extends [mscorlib]System.Object
  {
    .field public static !X StaticField
    .field public !X InstanceField
    .field private static !X '<StaticProperty>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field private !X '<InstanceProperty>k__BackingField'
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .method public hidebysig specialname static 
            !X  get_StaticProperty() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  ldsfld     !0 class ExpressionTrees/GenericClass`1<!X>::'<StaticProperty>k__BackingField'
      IL_0005:  ret
    } // end of method GenericClass`1::get_StaticProperty

    .method public hidebysig specialname static 
            void  set_StaticProperty(!X 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  stsfld     !0 class ExpressionTrees/GenericClass`1<!X>::'<StaticProperty>k__BackingField'
      IL_0006:  ret
    } // end of method GenericClass`1::set_StaticProperty

    .method public hidebysig specialname 
            instance !X  get_InstanceProperty() cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class ExpressionTrees/GenericClass`1<!X>::'<InstanceProperty>k__BackingField'
      IL_0006:  ret
    } // end of method GenericClass`1::get_InstanceProperty

    .method public hidebysig specialname 
            instance void  set_InstanceProperty(!X 'value') cil managed
    {
      .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      !0 class ExpressionTrees/GenericClass`1<!X>::'<InstanceProperty>k__BackingField'
      IL_0007:  ret
    } // end of method GenericClass`1::set_InstanceProperty

    .method public hidebysig static bool 
            GenericMethod<Y>() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  ret
    } // end of method GenericClass`1::GenericMethod

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method GenericClass`1::.ctor

    .property !X StaticProperty()
    {
      .get !X ExpressionTrees/GenericClass`1::get_StaticProperty()
      .set void ExpressionTrees/GenericClass`1::set_StaticProperty(!X)
    } // end of property GenericClass`1::StaticProperty
    .property instance !X InstanceProperty()
    {
      .set instance void ExpressionTrees/GenericClass`1::set_InstanceProperty(!X)
      .get instance !X ExpressionTrees/GenericClass`1::get_InstanceProperty()
    } // end of property GenericClass`1::InstanceProperty
  } // end of class GenericClass`1

  .class auto ansi nested private beforefieldinit Magic
         extends [mscorlib]System.Object
  {
    .method public hidebysig instance bool 
            TestMagic(int32 test) cil managed
    {
      // Code size       5 (0x5)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldc.i4.5
      IL_0002:  cgt
      IL_0004:  ret
    } // end of method Magic::TestMagic

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method Magic::.ctor

  } // end of class Magic

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public bool a
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass0'::.ctor

  } // end of class '<>c__DisplayClass0'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass2'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public bool a
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass2'::.ctor

  } // end of class '<>c__DisplayClass2'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass4'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 x
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass4'::.ctor

  } // end of class '<>c__DisplayClass4'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass8'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> dict
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass8'::.ctor

  } // end of class '<>c__DisplayClass8'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClassa'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 i
    .field public string x
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClassa'::.ctor

  } // end of class '<>c__DisplayClassa'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClassc'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public uint8 z
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClassc'::.ctor

  } // end of class '<>c__DisplayClassc'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass10'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class ExpressionTrees/Magic test
    .field public class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> sink
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass10'::.ctor

  } // end of class '<>c__DisplayClass10'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass14'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> 'call'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass14'::.ctor

  } // end of class '<>c__DisplayClass14'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass16'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public bool x
    .field public int32 y
    .field public uint8 z
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass16'::.ctor

  } // end of class '<>c__DisplayClass16'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass19'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public class ExpressionTrees/XmlReaderSettings s
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass19'::.ctor

  } // end of class '<>c__DisplayClass19'

  .class auto ansi sealed nested private beforefieldinit '<>c__DisplayClass1b'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 i
    .field public string x
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method '<>c__DisplayClass1b'::.ctor

  } // end of class '<>c__DisplayClass1b'

  .field private int32 'field'
  .field private static class [mscorlib]System.Func`2<int32,string> 'CS$<>9__CachedAnonymousMethodDelegate7'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> 'CS$<>9__CachedAnonymousMethodDelegatef'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private static class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> 'CS$<>9__CachedAnonymousMethodDelegate13'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method private hidebysig static object 
          ToCode<R>(object x,
                    class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!R>> expr) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ret
  } // end of method ExpressionTrees::ToCode

  .method private hidebysig static object 
          ToCode<T,R>(object x,
                      class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!T,!!R>> expr) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ret
  } // end of method ExpressionTrees::ToCode

  .method private hidebysig static object 
          X() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ret
  } // end of method ExpressionTrees::X

  .method public hidebysig instance void 
          Parameter(bool a) cil managed
  {
    // Code size       57 (0x39)
    .maxstack  3
    .locals init (class ExpressionTrees/'<>c__DisplayClass0' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      bool ExpressionTrees/'<>c__DisplayClass0'::a
    IL_000d:  call       object ExpressionTrees::X()
    IL_0012:  ldloc.0
    IL_0013:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0018:  ldtoken    field bool ExpressionTrees/'<>c__DisplayClass0'::a
    IL_001d:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0022:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0027:  ldc.i4.0
    IL_0028:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_002d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0032:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0037:  pop
    IL_0038:  ret
  } // end of method ExpressionTrees::Parameter

  .method public hidebysig instance void 
          LocalVariable() cil managed
  {
    // Code size       57 (0x39)
    .maxstack  3
    .locals init (class ExpressionTrees/'<>c__DisplayClass2' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass2'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  stfld      bool ExpressionTrees/'<>c__DisplayClass2'::a
    IL_000d:  call       object ExpressionTrees::X()
    IL_0012:  ldloc.0
    IL_0013:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0018:  ldtoken    field bool ExpressionTrees/'<>c__DisplayClass2'::a
    IL_001d:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0022:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0027:  ldc.i4.0
    IL_0028:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_002d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0032:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0037:  pop
    IL_0038:  ret
  } // end of method ExpressionTrees::LocalVariable

  .method public hidebysig instance void 
          LambdaParameter() cil managed
  {
    // Code size       51 (0x33)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Boolean
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldstr      "a"
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.1
    IL_001c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0021:  stloc.1
    IL_0022:  ldloc.1
    IL_0023:  ldc.i4.0
    IL_0024:  ldloc.0
    IL_0025:  stelem.ref
    IL_0026:  ldloc.1
    IL_0027:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<bool,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                        class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_002c:  call       object ExpressionTrees::ToCode<bool,bool>(object,
                                                                   class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_0031:  pop
    IL_0032:  ret
  } // end of method ExpressionTrees::LambdaParameter

  .method public hidebysig instance void 
          AddOperator(int32 x) cil managed
  {
    // Code size       109 (0x6d)
    .maxstack  4
    .locals init (class ExpressionTrees/'<>c__DisplayClass4' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass4'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      int32 ExpressionTrees/'<>c__DisplayClass4'::x
    IL_000d:  call       object ExpressionTrees::X()
    IL_0012:  ldc.i4.1
    IL_0013:  box        [mscorlib]System.Int32
    IL_0018:  ldtoken    [mscorlib]System.Int32
    IL_001d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0022:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0027:  ldloc.0
    IL_0028:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_002d:  ldtoken    field int32 ExpressionTrees/'<>c__DisplayClass4'::x
    IL_0032:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0037:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_003c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0041:  ldc.i4.2
    IL_0042:  box        [mscorlib]System.Int32
    IL_0047:  ldtoken    [mscorlib]System.Int32
    IL_004c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0051:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0056:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_005b:  ldc.i4.0
    IL_005c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0061:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0066:  call       object ExpressionTrees::ToCode<int32>(object,
                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_006b:  pop
    IL_006c:  ret
  } // end of method ExpressionTrees::AddOperator

  .method public hidebysig instance void 
          AnonymousClasses() cil managed
  {
    // Code size       157 (0x9d)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [mscorlib]System.Reflection.MethodInfo[] V_1)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void class '<>f__AnonymousType0`2'<int32,string>::.ctor(!0,
                                                                                                 !1)
    IL_000a:  ldtoken    class '<>f__AnonymousType0`2'<int32,string>
    IL_000f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0014:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0019:  ldc.i4.2
    IL_001a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001f:  stloc.0
    IL_0020:  ldloc.0
    IL_0021:  ldc.i4.0
    IL_0022:  ldc.i4.3
    IL_0023:  box        [mscorlib]System.Int32
    IL_0028:  ldtoken    [mscorlib]System.Int32
    IL_002d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0032:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0037:  stelem.ref
    IL_0038:  ldloc.0
    IL_0039:  ldc.i4.1
    IL_003a:  ldstr      "a"
    IL_003f:  ldtoken    [mscorlib]System.String
    IL_0044:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0049:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004e:  stelem.ref
    IL_004f:  ldloc.0
    IL_0050:  ldc.i4.2
    IL_0051:  newarr     [mscorlib]System.Reflection.MethodInfo
    IL_0056:  stloc.1
    IL_0057:  ldloc.1
    IL_0058:  ldc.i4.0
    IL_0059:  ldtoken    method instance !0 class '<>f__AnonymousType0`2'<int32,string>::get_X()
    IL_005e:  ldtoken    class '<>f__AnonymousType0`2'<int32,string>
    IL_0063:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0068:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_006d:  stelem.ref
    IL_006e:  ldloc.1
    IL_006f:  ldc.i4.1
    IL_0070:  ldtoken    method instance !1 class '<>f__AnonymousType0`2'<int32,string>::get_A()
    IL_0075:  ldtoken    class '<>f__AnonymousType0`2'<int32,string>
    IL_007a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0084:  stelem.ref
    IL_0085:  ldloc.1
    IL_0086:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>,
                                                                                                                                       class [mscorlib]System.Reflection.MemberInfo[])
    IL_008b:  ldc.i4.0
    IL_008c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0091:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class '<>f__AnonymousType0`2'<int32,string>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0096:  call       object ExpressionTrees::ToCode<class '<>f__AnonymousType0`2'<int32,string>>(object,
                                                                                                     class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_009b:  pop
    IL_009c:  ret
  } // end of method ExpressionTrees::AnonymousClasses

  .method public hidebysig instance void 
          ArrayIndex() cil managed
  {
    // Code size       232 (0xe8)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Int32
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldc.i4.3
    IL_0010:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0015:  stloc.0
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.0
    IL_0018:  ldc.i4.3
    IL_0019:  box        [mscorlib]System.Int32
    IL_001e:  ldtoken    [mscorlib]System.Int32
    IL_0023:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0028:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002d:  stelem.ref
    IL_002e:  ldloc.0
    IL_002f:  ldc.i4.1
    IL_0030:  ldc.i4.4
    IL_0031:  box        [mscorlib]System.Int32
    IL_0036:  ldtoken    [mscorlib]System.Int32
    IL_003b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0040:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0045:  stelem.ref
    IL_0046:  ldloc.0
    IL_0047:  ldc.i4.2
    IL_0048:  ldc.i4.5
    IL_0049:  box        [mscorlib]System.Int32
    IL_004e:  ldtoken    [mscorlib]System.Int32
    IL_0053:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0058:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005d:  stelem.ref
    IL_005e:  ldloc.0
    IL_005f:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0064:  ldc.i4.0
    IL_0065:  box        [mscorlib]System.Int32
    IL_006a:  ldtoken    [mscorlib]System.Int32
    IL_006f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0074:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0079:  ldnull
    IL_007a:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_007f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0084:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0089:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_008e:  ldtoken    method instance int64 [mscorlib]System.DateTime::get_Ticks()
    IL_0093:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0098:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_009d:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00a2:  ldc.i4.3
    IL_00a3:  conv.i8
    IL_00a4:  box        [mscorlib]System.Int64
    IL_00a9:  ldtoken    [mscorlib]System.Int64
    IL_00ae:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b8:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Modulo(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_00bd:  ldtoken    [mscorlib]System.Int32
    IL_00c2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c7:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00cc:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_00d1:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_00d6:  ldc.i4.0
    IL_00d7:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00dc:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00e1:  call       object ExpressionTrees::ToCode<int32>(object,
                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00e6:  pop
    IL_00e7:  ret
  } // end of method ExpressionTrees::ArrayIndex

  .method public hidebysig instance void 
          ArrayLengthAndDoubles() cil managed
  {
    // Code size       301 (0x12d)
    .maxstack  14
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method !!0[] [System.Core]System.Linq.Enumerable::ToArray<float64>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldc.i4.1
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.0
    IL_001e:  ldnull
    IL_001f:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Concat<float64>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0024:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0029:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002e:  ldc.i4.2
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0034:  stloc.1
    IL_0035:  ldloc.1
    IL_0036:  ldc.i4.0
    IL_0037:  ldtoken    [mscorlib]System.Double
    IL_003c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0041:  ldc.i4.3
    IL_0042:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0047:  stloc.2
    IL_0048:  ldloc.2
    IL_0049:  ldc.i4.0
    IL_004a:  ldc.r8     1.
    IL_0053:  box        [mscorlib]System.Double
    IL_0058:  ldtoken    [mscorlib]System.Double
    IL_005d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0062:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0067:  stelem.ref
    IL_0068:  ldloc.2
    IL_0069:  ldc.i4.1
    IL_006a:  ldc.r8     2.0099999999999998
    IL_0073:  box        [mscorlib]System.Double
    IL_0078:  ldtoken    [mscorlib]System.Double
    IL_007d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0082:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0087:  stelem.ref
    IL_0088:  ldloc.2
    IL_0089:  ldc.i4.2
    IL_008a:  ldc.r8     3.5
    IL_0093:  box        [mscorlib]System.Double
    IL_0098:  ldtoken    [mscorlib]System.Double
    IL_009d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a7:  stelem.ref
    IL_00a8:  ldloc.2
    IL_00a9:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00ae:  stelem.ref
    IL_00af:  ldloc.1
    IL_00b0:  ldc.i4.1
    IL_00b1:  ldtoken    [mscorlib]System.Double
    IL_00b6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00bb:  ldc.i4.2
    IL_00bc:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00c1:  stloc.3
    IL_00c2:  ldloc.3
    IL_00c3:  ldc.i4.0
    IL_00c4:  ldc.r8     1.
    IL_00cd:  box        [mscorlib]System.Double
    IL_00d2:  ldtoken    [mscorlib]System.Double
    IL_00d7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00dc:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e1:  stelem.ref
    IL_00e2:  ldloc.3
    IL_00e3:  ldc.i4.1
    IL_00e4:  ldc.r8     2.
    IL_00ed:  box        [mscorlib]System.Double
    IL_00f2:  ldtoken    [mscorlib]System.Double
    IL_00f7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00fc:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0101:  stelem.ref
    IL_0102:  ldloc.3
    IL_0103:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0108:  stelem.ref
    IL_0109:  ldloc.1
    IL_010a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_010f:  stelem.ref
    IL_0110:  ldloc.0
    IL_0111:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0116:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayLength(class [System.Core]System.Linq.Expressions.Expression)
    IL_011b:  ldc.i4.0
    IL_011c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0121:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0126:  call       object ExpressionTrees::ToCode<int32>(object,
                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_012b:  pop
    IL_012c:  ret
  } // end of method ExpressionTrees::ArrayLengthAndDoubles

  .method public hidebysig instance void 
          AsOperator() cil managed
  {
    // Code size       64 (0x40)
    .maxstack  3
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_000a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_000f:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0014:  ldc.i4.0
    IL_0015:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001a:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_001f:  ldtoken    [mscorlib]System.String
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::TypeAs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Type)
    IL_002e:  ldc.i4.0
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0034:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0039:  call       object ExpressionTrees::ToCode<string>(object,
                                                                class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_003e:  pop
    IL_003f:  ret
  } // end of method ExpressionTrees::AsOperator

  .method public hidebysig instance void 
          ComplexGenericName() cil managed
  {
    // Code size       140 (0x8c)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Int32
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldstr      "x"
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0019:  stloc.0
    IL_001a:  ldloc.0
    IL_001b:  ldc.i4.0
    IL_001c:  box        [mscorlib]System.Int32
    IL_0021:  ldtoken    [mscorlib]System.Int32
    IL_0026:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0030:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0035:  ldc.i4.1
    IL_0036:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003b:  stloc.1
    IL_003c:  ldloc.1
    IL_003d:  ldc.i4.0
    IL_003e:  ldloc.0
    IL_003f:  stelem.ref
    IL_0040:  ldloc.1
    IL_0041:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0046:  ldtoken    class [mscorlib]System.Func`2<int32,bool>
    IL_004b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0050:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0055:  ldc.i4.1
    IL_0056:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_005b:  stloc.2
    IL_005c:  ldloc.2
    IL_005d:  ldc.i4.0
    IL_005e:  ldc.i4.0
    IL_005f:  box        [mscorlib]System.Int32
    IL_0064:  ldtoken    [mscorlib]System.Int32
    IL_0069:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0073:  stelem.ref
    IL_0074:  ldloc.2
    IL_0075:  call       class [System.Core]System.Linq.Expressions.InvocationExpression [System.Core]System.Linq.Expressions.Expression::Invoke(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression[])
    IL_007a:  ldc.i4.0
    IL_007b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0080:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0085:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008a:  pop
    IL_008b:  ret
  } // end of method ExpressionTrees::ComplexGenericName

  .method public hidebysig instance void 
          DefaultValue() cil managed
  {
    // Code size       173 (0xad)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             valuetype [mscorlib]System.TimeSpan V_1)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void [mscorlib]System.TimeSpan::.ctor(int32,
                                                                               int32,
                                                                               int32)
    IL_000a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_000f:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0014:  ldc.i4.3
    IL_0015:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001a:  stloc.0
    IL_001b:  ldloc.0
    IL_001c:  ldc.i4.0
    IL_001d:  ldc.i4.1
    IL_001e:  box        [mscorlib]System.Int32
    IL_0023:  ldtoken    [mscorlib]System.Int32
    IL_0028:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0032:  stelem.ref
    IL_0033:  ldloc.0
    IL_0034:  ldc.i4.1
    IL_0035:  ldc.i4.2
    IL_0036:  box        [mscorlib]System.Int32
    IL_003b:  ldtoken    [mscorlib]System.Int32
    IL_0040:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0045:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004a:  stelem.ref
    IL_004b:  ldloc.0
    IL_004c:  ldc.i4.2
    IL_004d:  ldc.i4.3
    IL_004e:  box        [mscorlib]System.Int32
    IL_0053:  ldtoken    [mscorlib]System.Int32
    IL_0058:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0062:  stelem.ref
    IL_0063:  ldloc.0
    IL_0064:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0069:  ldloca.s   V_1
    IL_006b:  initobj    [mscorlib]System.TimeSpan
    IL_0071:  ldloc.1
    IL_0072:  box        [mscorlib]System.TimeSpan
    IL_0077:  ldtoken    [mscorlib]System.TimeSpan
    IL_007c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0081:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0086:  ldc.i4.0
    IL_0087:  ldtoken    method bool [mscorlib]System.TimeSpan::op_Equality(valuetype [mscorlib]System.TimeSpan,
                                                                            valuetype [mscorlib]System.TimeSpan)
    IL_008c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0091:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0096:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            bool,
                                                                                                                                            class [mscorlib]System.Reflection.MethodInfo)
    IL_009b:  ldc.i4.0
    IL_009c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a1:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00a6:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00ab:  pop
    IL_00ac:  ret
  } // end of method ExpressionTrees::DefaultValue

  .method public hidebysig instance void 
          EnumConstant() cil managed
  {
    // Code size       116 (0x74)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_000a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_000f:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0014:  ldc.i4.0
    IL_0015:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001a:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_001f:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_0024:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0029:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002e:  ldc.i4.1
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0034:  stloc.0
    IL_0035:  ldloc.0
    IL_0036:  ldc.i4.0
    IL_0037:  ldc.i4.0
    IL_0038:  box        [mscorlib]System.MidpointRounding
    IL_003d:  ldtoken    [mscorlib]System.MidpointRounding
    IL_0042:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0047:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004c:  ldtoken    [mscorlib]System.Object
    IL_0051:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0056:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_005b:  stelem.ref
    IL_005c:  ldloc.0
    IL_005d:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0062:  ldc.i4.0
    IL_0063:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0068:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006d:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0072:  pop
    IL_0073:  ret
  } // end of method ExpressionTrees::EnumConstant

  .method public hidebysig instance void 
          IndexerAccess() cil managed
  {
    // Code size       180 (0xb4)
    .maxstack  7
    .locals init (class ExpressionTrees/'<>c__DisplayClass8' V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass8'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  ldc.i4.s   20
    IL_000a:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [System.Core]System.Linq.Enumerable::Range(int32,
                                                                                                                                    int32)
    IL_000f:  ldsfld     class [mscorlib]System.Func`2<int32,string> ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7'
    IL_0014:  brtrue.s   IL_0027

    IL_0016:  ldnull
    IL_0017:  ldftn      string ExpressionTrees::'<IndexerAccess>b__6'(int32)
    IL_001d:  newobj     instance void class [mscorlib]System.Func`2<int32,string>::.ctor(object,
                                                                                          native int)
    IL_0022:  stsfld     class [mscorlib]System.Func`2<int32,string> ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7'
    IL_0027:  ldsfld     class [mscorlib]System.Func`2<int32,string> ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7'
    IL_002c:  call       class [mscorlib]System.Collections.Generic.Dictionary`2<!!1,!!0> [System.Core]System.Linq.Enumerable::ToDictionary<int32,string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                          class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0031:  stfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> ExpressionTrees/'<>c__DisplayClass8'::dict
    IL_0036:  call       object ExpressionTrees::X()
    IL_003b:  ldloc.0
    IL_003c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0041:  ldtoken    field class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> ExpressionTrees/'<>c__DisplayClass8'::dict
    IL_0046:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_004b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0050:  ldtoken    method instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::get_Item(!0)
    IL_0055:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>
    IL_005a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0064:  ldc.i4.1
    IL_0065:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_006a:  stloc.1
    IL_006b:  ldloc.1
    IL_006c:  ldc.i4.0
    IL_006d:  ldstr      "3"
    IL_0072:  ldtoken    [mscorlib]System.String
    IL_0077:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0081:  stelem.ref
    IL_0082:  ldloc.1
    IL_0083:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0088:  ldc.i4.3
    IL_0089:  box        [mscorlib]System.Int32
    IL_008e:  ldtoken    [mscorlib]System.Int32
    IL_0093:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0098:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00a2:  ldc.i4.0
    IL_00a3:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a8:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ad:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00b2:  pop
    IL_00b3:  ret
  } // end of method ExpressionTrees::IndexerAccess

  .method public hidebysig instance void 
          IsOperator() cil managed
  {
    // Code size       64 (0x40)
    .maxstack  3
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_000a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_000f:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0014:  ldc.i4.0
    IL_0015:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001a:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_001f:  ldtoken    [mscorlib]System.String
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  call       class [System.Core]System.Linq.Expressions.TypeBinaryExpression [System.Core]System.Linq.Expressions.Expression::TypeIs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002e:  ldc.i4.0
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0034:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0039:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_003e:  pop
    IL_003f:  ret
  } // end of method ExpressionTrees::IsOperator

  .method public hidebysig instance void 
          ListInitializer() cil managed
  {
    // Code size       370 (0x172)
    .maxstack  9
    .locals init (class [System.Core]System.Linq.Expressions.ElementInit[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::.ctor()
    IL_000a:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_000f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0014:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0019:  ldc.i4.0
    IL_001a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001f:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0024:  ldc.i4.3
    IL_0025:  newarr     [System.Core]System.Linq.Expressions.ElementInit
    IL_002a:  stloc.0
    IL_002b:  ldloc.0
    IL_002c:  ldc.i4.0
    IL_002d:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::Add(!0,
                                                                                                                        !1)
    IL_0032:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_0037:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0041:  ldc.i4.2
    IL_0042:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0047:  stloc.1
    IL_0048:  ldloc.1
    IL_0049:  ldc.i4.0
    IL_004a:  ldc.i4.1
    IL_004b:  box        [mscorlib]System.Int32
    IL_0050:  ldtoken    [mscorlib]System.Int32
    IL_0055:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005f:  stelem.ref
    IL_0060:  ldloc.1
    IL_0061:  ldc.i4.1
    IL_0062:  ldc.i4.1
    IL_0063:  box        [mscorlib]System.Int32
    IL_0068:  ldtoken    [mscorlib]System.Int32
    IL_006d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0072:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0077:  stelem.ref
    IL_0078:  ldloc.1
    IL_0079:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_007e:  stelem.ref
    IL_007f:  ldloc.0
    IL_0080:  ldc.i4.1
    IL_0081:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::Add(!0,
                                                                                                                        !1)
    IL_0086:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_008b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0090:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0095:  ldc.i4.2
    IL_0096:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_009b:  stloc.2
    IL_009c:  ldloc.2
    IL_009d:  ldc.i4.0
    IL_009e:  ldc.i4.2
    IL_009f:  box        [mscorlib]System.Int32
    IL_00a4:  ldtoken    [mscorlib]System.Int32
    IL_00a9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ae:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b3:  stelem.ref
    IL_00b4:  ldloc.2
    IL_00b5:  ldc.i4.1
    IL_00b6:  ldc.i4.2
    IL_00b7:  box        [mscorlib]System.Int32
    IL_00bc:  ldtoken    [mscorlib]System.Int32
    IL_00c1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cb:  stelem.ref
    IL_00cc:  ldloc.2
    IL_00cd:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00d2:  stelem.ref
    IL_00d3:  ldloc.0
    IL_00d4:  ldc.i4.2
    IL_00d5:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::Add(!0,
                                                                                                                        !1)
    IL_00da:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_00df:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e4:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00e9:  ldc.i4.2
    IL_00ea:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00ef:  stloc.3
    IL_00f0:  ldloc.3
    IL_00f1:  ldc.i4.0
    IL_00f2:  ldc.i4.3
    IL_00f3:  box        [mscorlib]System.Int32
    IL_00f8:  ldtoken    [mscorlib]System.Int32
    IL_00fd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0102:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0107:  stelem.ref
    IL_0108:  ldloc.3
    IL_0109:  ldc.i4.1
    IL_010a:  ldc.i4.4
    IL_010b:  box        [mscorlib]System.Int32
    IL_0110:  ldtoken    [mscorlib]System.Int32
    IL_0115:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_011a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_011f:  stelem.ref
    IL_0120:  ldloc.3
    IL_0121:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_0126:  stelem.ref
    IL_0127:  ldloc.0
    IL_0128:  call       class [System.Core]System.Linq.Expressions.ListInitExpression [System.Core]System.Linq.Expressions.Expression::ListInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.ElementInit[])
    IL_012d:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::get_Count()
    IL_0132:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_0137:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_013c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0141:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0146:  ldc.i4.3
    IL_0147:  box        [mscorlib]System.Int32
    IL_014c:  ldtoken    [mscorlib]System.Int32
    IL_0151:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0156:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_015b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0160:  ldc.i4.0
    IL_0161:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0166:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_016b:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0170:  pop
    IL_0171:  ret
  } // end of method ExpressionTrees::ListInitializer

  .method public hidebysig instance void 
          ListInitializer2() cil managed
  {
    // Code size       328 (0x148)
    .maxstack  9
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.ElementInit[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3,
             class [System.Core]System.Linq.Expressions.Expression[] V_4)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor(int32)
    IL_000a:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_000f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0014:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0019:  ldc.i4.1
    IL_001a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001f:  stloc.0
    IL_0020:  ldloc.0
    IL_0021:  ldc.i4.0
    IL_0022:  ldc.i4.s   50
    IL_0024:  box        [mscorlib]System.Int32
    IL_0029:  ldtoken    [mscorlib]System.Int32
    IL_002e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0033:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0038:  stelem.ref
    IL_0039:  ldloc.0
    IL_003a:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_003f:  ldc.i4.3
    IL_0040:  newarr     [System.Core]System.Linq.Expressions.ElementInit
    IL_0045:  stloc.1
    IL_0046:  ldloc.1
    IL_0047:  ldc.i4.0
    IL_0048:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_004d:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0052:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0057:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005c:  ldc.i4.1
    IL_005d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0062:  stloc.2
    IL_0063:  ldloc.2
    IL_0064:  ldc.i4.0
    IL_0065:  ldc.i4.1
    IL_0066:  box        [mscorlib]System.Int32
    IL_006b:  ldtoken    [mscorlib]System.Int32
    IL_0070:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0075:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_007a:  stelem.ref
    IL_007b:  ldloc.2
    IL_007c:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_0081:  stelem.ref
    IL_0082:  ldloc.1
    IL_0083:  ldc.i4.1
    IL_0084:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0089:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_008e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0093:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0098:  ldc.i4.1
    IL_0099:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_009e:  stloc.3
    IL_009f:  ldloc.3
    IL_00a0:  ldc.i4.0
    IL_00a1:  ldc.i4.2
    IL_00a2:  box        [mscorlib]System.Int32
    IL_00a7:  ldtoken    [mscorlib]System.Int32
    IL_00ac:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b6:  stelem.ref
    IL_00b7:  ldloc.3
    IL_00b8:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00bd:  stelem.ref
    IL_00be:  ldloc.1
    IL_00bf:  ldc.i4.2
    IL_00c0:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_00c5:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00ca:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00cf:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00d4:  ldc.i4.1
    IL_00d5:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00da:  stloc.s    V_4
    IL_00dc:  ldloc.s    V_4
    IL_00de:  ldc.i4.0
    IL_00df:  ldc.i4.3
    IL_00e0:  box        [mscorlib]System.Int32
    IL_00e5:  ldtoken    [mscorlib]System.Int32
    IL_00ea:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ef:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00f4:  stelem.ref
    IL_00f5:  ldloc.s    V_4
    IL_00f7:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00fc:  stelem.ref
    IL_00fd:  ldloc.1
    IL_00fe:  call       class [System.Core]System.Linq.Expressions.ListInitExpression [System.Core]System.Linq.Expressions.Expression::ListInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.ElementInit[])
    IL_0103:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_0108:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_010d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0112:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0117:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_011c:  ldc.i4.3
    IL_011d:  box        [mscorlib]System.Int32
    IL_0122:  ldtoken    [mscorlib]System.Int32
    IL_0127:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_012c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0131:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0136:  ldc.i4.0
    IL_0137:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_013c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0141:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0146:  pop
    IL_0147:  ret
  } // end of method ExpressionTrees::ListInitializer2

  .method public hidebysig instance void 
          ListInitializer3() cil managed
  {
    // Code size       298 (0x12a)
    .maxstack  9
    .locals init (class [System.Core]System.Linq.Expressions.ElementInit[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
    IL_000a:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_000f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0014:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0019:  ldc.i4.0
    IL_001a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001f:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0024:  ldc.i4.3
    IL_0025:  newarr     [System.Core]System.Linq.Expressions.ElementInit
    IL_002a:  stloc.0
    IL_002b:  ldloc.0
    IL_002c:  ldc.i4.0
    IL_002d:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0032:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0037:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0041:  ldc.i4.1
    IL_0042:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0047:  stloc.1
    IL_0048:  ldloc.1
    IL_0049:  ldc.i4.0
    IL_004a:  ldc.i4.1
    IL_004b:  box        [mscorlib]System.Int32
    IL_0050:  ldtoken    [mscorlib]System.Int32
    IL_0055:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005f:  stelem.ref
    IL_0060:  ldloc.1
    IL_0061:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_0066:  stelem.ref
    IL_0067:  ldloc.0
    IL_0068:  ldc.i4.1
    IL_0069:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_006e:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0073:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0078:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007d:  ldc.i4.1
    IL_007e:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0083:  stloc.2
    IL_0084:  ldloc.2
    IL_0085:  ldc.i4.0
    IL_0086:  ldc.i4.2
    IL_0087:  box        [mscorlib]System.Int32
    IL_008c:  ldtoken    [mscorlib]System.Int32
    IL_0091:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0096:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009b:  stelem.ref
    IL_009c:  ldloc.2
    IL_009d:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00a2:  stelem.ref
    IL_00a3:  ldloc.0
    IL_00a4:  ldc.i4.2
    IL_00a5:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_00aa:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00af:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b4:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00b9:  ldc.i4.1
    IL_00ba:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00bf:  stloc.3
    IL_00c0:  ldloc.3
    IL_00c1:  ldc.i4.0
    IL_00c2:  ldc.i4.3
    IL_00c3:  box        [mscorlib]System.Int32
    IL_00c8:  ldtoken    [mscorlib]System.Int32
    IL_00cd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d7:  stelem.ref
    IL_00d8:  ldloc.3
    IL_00d9:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00de:  stelem.ref
    IL_00df:  ldloc.0
    IL_00e0:  call       class [System.Core]System.Linq.Expressions.ListInitExpression [System.Core]System.Linq.Expressions.Expression::ListInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.ElementInit[])
    IL_00e5:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_00ea:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00ef:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f4:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00f9:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00fe:  ldc.i4.3
    IL_00ff:  box        [mscorlib]System.Int32
    IL_0104:  ldtoken    [mscorlib]System.Int32
    IL_0109:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0113:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0118:  ldc.i4.0
    IL_0119:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_011e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0123:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0128:  pop
    IL_0129:  ret
  } // end of method ExpressionTrees::ListInitializer3

  .method public hidebysig instance void 
          LiteralCharAndProperty() cil managed
  {
    // Code size       146 (0x92)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void [mscorlib]System.String::.ctor(char,
                                                                             int32)
    IL_000a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_000f:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0014:  ldc.i4.2
    IL_0015:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001a:  stloc.0
    IL_001b:  ldloc.0
    IL_001c:  ldc.i4.0
    IL_001d:  ldc.i4.s   32
    IL_001f:  box        [mscorlib]System.Char
    IL_0024:  ldtoken    [mscorlib]System.Char
    IL_0029:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0033:  stelem.ref
    IL_0034:  ldloc.0
    IL_0035:  ldc.i4.1
    IL_0036:  ldc.i4.3
    IL_0037:  box        [mscorlib]System.Int32
    IL_003c:  ldtoken    [mscorlib]System.Int32
    IL_0041:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0046:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004b:  stelem.ref
    IL_004c:  ldloc.0
    IL_004d:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0052:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_0057:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_005c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0061:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0066:  ldc.i4.1
    IL_0067:  box        [mscorlib]System.Int32
    IL_006c:  ldtoken    [mscorlib]System.Int32
    IL_0071:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0076:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_007b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0080:  ldc.i4.0
    IL_0081:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0086:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008b:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0090:  pop
    IL_0091:  ret
  } // end of method ExpressionTrees::LiteralCharAndProperty

  .method public hidebysig instance void 
          CharNoCast() cil managed
  {
    // Code size       137 (0x89)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldstr      "abc"
    IL_000a:  ldtoken    [mscorlib]System.String
    IL_000f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0019:  ldtoken    method instance char [mscorlib]System.String::get_Chars(int32)
    IL_001e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0023:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0028:  ldc.i4.1
    IL_0029:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002e:  stloc.0
    IL_002f:  ldloc.0
    IL_0030:  ldc.i4.0
    IL_0031:  ldc.i4.1
    IL_0032:  box        [mscorlib]System.Int32
    IL_0037:  ldtoken    [mscorlib]System.Int32
    IL_003c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0041:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0046:  stelem.ref
    IL_0047:  ldloc.0
    IL_0048:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_004d:  ldtoken    [mscorlib]System.Int32
    IL_0052:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0057:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_005c:  ldc.i4.s   98
    IL_005e:  box        [mscorlib]System.Int32
    IL_0063:  ldtoken    [mscorlib]System.Int32
    IL_0068:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0072:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0077:  ldc.i4.0
    IL_0078:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0082:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0087:  pop
    IL_0088:  ret
  } // end of method ExpressionTrees::CharNoCast

  .method public hidebysig instance void 
          StringsImplicitCast() cil managed
  {
    // Code size       376 (0x178)
    .maxstack  8
    .locals init (class ExpressionTrees/'<>c__DisplayClassa' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClassa'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  stfld      int32 ExpressionTrees/'<>c__DisplayClassa'::i
    IL_000d:  ldloc.0
    IL_000e:  ldstr      "X"
    IL_0013:  stfld      string ExpressionTrees/'<>c__DisplayClassa'::x
    IL_0018:  call       object ExpressionTrees::X()
    IL_001d:  ldstr      "a\n\\b"
    IL_0022:  ldtoken    [mscorlib]System.String
    IL_0027:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0031:  ldloc.0
    IL_0032:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0037:  ldtoken    field string ExpressionTrees/'<>c__DisplayClassa'::x
    IL_003c:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0041:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0046:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Coalesce(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_004b:  ldloc.0
    IL_004c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0051:  ldtoken    field string ExpressionTrees/'<>c__DisplayClassa'::x
    IL_0056:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_005b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0060:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_0065:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_006a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_006f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_0074:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_0079:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_007e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0083:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0088:  ldc.i4.2
    IL_0089:  box        [mscorlib]System.Int32
    IL_008e:  ldtoken    [mscorlib]System.Int32
    IL_0093:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0098:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00a2:  ldc.i4.0
    IL_00a3:  box        [mscorlib]System.Boolean
    IL_00a8:  ldtoken    [mscorlib]System.Boolean
    IL_00ad:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b7:  ldc.i4.1
    IL_00b8:  box        [mscorlib]System.Boolean
    IL_00bd:  ldtoken    [mscorlib]System.Boolean
    IL_00c2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cc:  ldc.i4.1
    IL_00cd:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_00d2:  box        [mscorlib]System.Decimal
    IL_00d7:  ldtoken    [mscorlib]System.Decimal
    IL_00dc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e6:  ldloc.0
    IL_00e7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_00ec:  ldtoken    field int32 ExpressionTrees/'<>c__DisplayClassa'::i
    IL_00f1:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00f6:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00fb:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Negate(class [System.Core]System.Linq.Expressions.Expression)
    IL_0100:  ldtoken    [mscorlib]System.Decimal
    IL_0105:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010a:  ldtoken    method valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(int32)
    IL_010f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0114:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0119:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type,
                                                                                                                                             class [mscorlib]System.Reflection.MethodInfo)
    IL_011e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0123:  ldc.i4.0
    IL_0124:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0129:  box        [mscorlib]System.Decimal
    IL_012e:  ldtoken    [mscorlib]System.Decimal
    IL_0133:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0138:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_013d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0142:  ldc.i4.0
    IL_0143:  box        [mscorlib]System.Boolean
    IL_0148:  ldtoken    [mscorlib]System.Boolean
    IL_014d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0152:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0157:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::OrElse(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_015c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_0161:  call       class [System.Core]System.Linq.Expressions.ConditionalExpression [System.Core]System.Linq.Expressions.Expression::Condition(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression)
    IL_0166:  ldc.i4.0
    IL_0167:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_016c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0171:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0176:  pop
    IL_0177:  ret
  } // end of method ExpressionTrees::StringsImplicitCast

  .method public hidebysig instance void 
          NotImplicitCast() cil managed
  {
    // Code size       104 (0x68)
    .maxstack  4
    .locals init (class ExpressionTrees/'<>c__DisplayClassc' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClassc'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.s   42
    IL_0009:  stfld      uint8 ExpressionTrees/'<>c__DisplayClassc'::z
    IL_000e:  call       object ExpressionTrees::X()
    IL_0013:  ldloc.0
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0019:  ldtoken    field uint8 ExpressionTrees/'<>c__DisplayClassc'::z
    IL_001e:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0023:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0028:  ldtoken    [mscorlib]System.Int32
    IL_002d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0032:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0037:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_003c:  ldc.i4.0
    IL_003d:  box        [mscorlib]System.Int32
    IL_0042:  ldtoken    [mscorlib]System.Int32
    IL_0047:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0051:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0056:  ldc.i4.0
    IL_0057:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_005c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0061:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0066:  pop
    IL_0067:  ret
  } // end of method ExpressionTrees::NotImplicitCast

  .method public hidebysig instance void 
          MembersBuiltin() cil managed
  {
    // Code size       405 (0x195)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldc.i4.s   123
    IL_0007:  ldc.i4.0
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.i4.0
    IL_000a:  ldc.i4.2
    IL_000b:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0010:  box        [mscorlib]System.Decimal
    IL_0015:  ldtoken    [mscorlib]System.Decimal
    IL_001a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0024:  ldtoken    method instance string [mscorlib]System.Decimal::ToString()
    IL_0029:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0033:  ldc.i4.0
    IL_0034:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0039:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_003e:  ldc.i4.0
    IL_003f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0044:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0049:  call       object ExpressionTrees::ToCode<string>(object,
                                                                class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_004e:  pop
    IL_004f:  call       object ExpressionTrees::X()
    IL_0054:  ldc.i4     0x7fff
    IL_0059:  box        [mscorlib]System.AttributeTargets
    IL_005e:  ldtoken    [mscorlib]System.AttributeTargets
    IL_0063:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0068:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006d:  ldtoken    method instance bool [mscorlib]System.Enum::HasFlag(class [mscorlib]System.Enum)
    IL_0072:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0077:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007c:  ldc.i4.1
    IL_007d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0082:  stloc.0
    IL_0083:  ldloc.0
    IL_0084:  ldc.i4.0
    IL_0085:  ldc.i4.1
    IL_0086:  box        [mscorlib]System.AttributeTargets
    IL_008b:  ldtoken    [mscorlib]System.AttributeTargets
    IL_0090:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0095:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009a:  ldtoken    [mscorlib]System.Enum
    IL_009f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a4:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00a9:  stelem.ref
    IL_00aa:  ldloc.0
    IL_00ab:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00b0:  ldc.i4.0
    IL_00b1:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00b6:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00bb:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00c0:  pop
    IL_00c1:  call       object ExpressionTrees::X()
    IL_00c6:  ldstr      "abc"
    IL_00cb:  ldtoken    [mscorlib]System.String
    IL_00d0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00da:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_00df:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00e4:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00e9:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00ee:  ldc.i4.3
    IL_00ef:  box        [mscorlib]System.Int32
    IL_00f4:  ldtoken    [mscorlib]System.Int32
    IL_00f9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00fe:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0103:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0108:  ldc.i4.0
    IL_0109:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_010e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0113:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0118:  pop
    IL_0119:  call       object ExpressionTrees::X()
    IL_011e:  ldc.i4.s   97
    IL_0120:  box        [mscorlib]System.Char
    IL_0125:  ldtoken    [mscorlib]System.Char
    IL_012a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_012f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0134:  ldtoken    method instance int32 [mscorlib]System.Char::CompareTo(char)
    IL_0139:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_013e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0143:  ldc.i4.1
    IL_0144:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0149:  stloc.1
    IL_014a:  ldloc.1
    IL_014b:  ldc.i4.0
    IL_014c:  ldc.i4.s   98
    IL_014e:  box        [mscorlib]System.Char
    IL_0153:  ldtoken    [mscorlib]System.Char
    IL_0158:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_015d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0162:  stelem.ref
    IL_0163:  ldloc.1
    IL_0164:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0169:  ldc.i4.0
    IL_016a:  box        [mscorlib]System.Int32
    IL_016f:  ldtoken    [mscorlib]System.Int32
    IL_0174:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0179:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_017e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0183:  ldc.i4.0
    IL_0184:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0189:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_018e:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0193:  pop
    IL_0194:  ret
  } // end of method ExpressionTrees::MembersBuiltin

  .method public hidebysig instance void 
          MembersDefault() cil managed
  {
    // Code size       99 (0x63)
    .maxstack  4
    .locals init (valuetype [mscorlib]System.DateTime V_0)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldloca.s   V_0
    IL_0007:  initobj    [mscorlib]System.DateTime
    IL_000d:  ldloc.0
    IL_000e:  box        [mscorlib]System.DateTime
    IL_0013:  ldtoken    [mscorlib]System.DateTime
    IL_0018:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0022:  ldtoken    method instance int64 [mscorlib]System.DateTime::get_Ticks()
    IL_0027:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0031:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0036:  ldc.i4.0
    IL_0037:  conv.i8
    IL_0038:  box        [mscorlib]System.Int64
    IL_003d:  ldtoken    [mscorlib]System.Int64
    IL_0042:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0047:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0051:  ldc.i4.0
    IL_0052:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0057:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005c:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0061:  pop
    IL_0062:  ret
  } // end of method ExpressionTrees::MembersDefault

  .method public hidebysig instance void 
          Test() cil managed
  {
    // Code size       547 (0x223)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Int32
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldc.i4.0
    IL_0010:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0015:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_001a:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayLength(class [System.Core]System.Linq.Expressions.Expression)
    IL_001f:  ldc.i4.0
    IL_0020:  box        [mscorlib]System.Int32
    IL_0025:  ldtoken    [mscorlib]System.Int32
    IL_002a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0034:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0039:  ldc.i4.0
    IL_003a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0044:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0049:  pop
    IL_004a:  call       object ExpressionTrees::X()
    IL_004f:  ldarg.0
    IL_0050:  box        ExpressionTrees
    IL_0055:  ldtoken    ExpressionTrees
    IL_005a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0064:  ldtoken    method instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()
    IL_0069:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_006e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0073:  ldc.i4.0
    IL_0074:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0079:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_007e:  ldtoken    method instance bool [mscorlib]System.Type::get_IsLayoutSequential()
    IL_0083:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0088:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_008d:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0092:  ldc.i4.0
    IL_0093:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0098:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_009d:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00a2:  pop
    IL_00a3:  call       object ExpressionTrees::X()
    IL_00a8:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
    IL_00ad:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00b2:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b7:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_00bc:  ldc.i4.0
    IL_00bd:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00c2:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_00c7:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_00cc:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00d1:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d6:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00db:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00e0:  ldc.i4.0
    IL_00e1:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00e6:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00eb:  call       object ExpressionTrees::ToCode<int32>(object,
                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00f0:  pop
    IL_00f1:  call       object ExpressionTrees::X()
    IL_00f6:  ldtoken    [mscorlib]System.Int32
    IL_00fb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0100:  ldc.i4.0
    IL_0101:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0106:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_010b:  ldtoken    method instance object [mscorlib]System.Array::Clone()
    IL_0110:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0115:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_011a:  ldc.i4.0
    IL_011b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0120:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0125:  ldnull
    IL_0126:  box        [mscorlib]System.Object
    IL_012b:  ldtoken    [mscorlib]System.Object
    IL_0130:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0135:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_013a:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_013f:  ldc.i4.0
    IL_0140:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0145:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_014a:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_014f:  pop
    IL_0150:  call       object ExpressionTrees::X()
    IL_0155:  ldarg.0
    IL_0156:  box        ExpressionTrees
    IL_015b:  ldtoken    ExpressionTrees
    IL_0160:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0165:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_016a:  ldtoken    method instance class [mscorlib]System.Type [mscorlib]System.Object::GetType()
    IL_016f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0174:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0179:  ldc.i4.0
    IL_017a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_017f:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0184:  ldtoken    method instance bool [mscorlib]System.Type::IsInstanceOfType(object)
    IL_0189:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_018e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0193:  ldc.i4.1
    IL_0194:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0199:  stloc.0
    IL_019a:  ldloc.0
    IL_019b:  ldc.i4.0
    IL_019c:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_01a1:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01a6:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_01ab:  ldc.i4.0
    IL_01ac:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01b1:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_01b6:  stelem.ref
    IL_01b7:  ldloc.0
    IL_01b8:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_01bd:  ldc.i4.0
    IL_01be:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01c3:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01c8:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_01cd:  pop
    IL_01ce:  call       object ExpressionTrees::X()
    IL_01d3:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
    IL_01d8:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_01dd:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01e2:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_01e7:  ldc.i4.0
    IL_01e8:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01ed:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_01f2:  ldtoken    method instance class [mscorlib]System.Collections.ObjectModel.ReadOnlyCollection`1<!0> class [mscorlib]System.Collections.Generic.List`1<int32>::AsReadOnly()
    IL_01f7:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_01fc:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0201:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0206:  ldc.i4.0
    IL_0207:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_020c:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0211:  ldc.i4.0
    IL_0212:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0217:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Collections.ObjectModel.ReadOnlyCollection`1<int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_021c:  call       object ExpressionTrees::ToCode<class [mscorlib]System.Collections.ObjectModel.ReadOnlyCollection`1<int32>>(object,
                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0221:  pop
    IL_0222:  ret
  } // end of method ExpressionTrees::Test

  .method public hidebysig instance void 
          DoAssert() cil managed
  {
    // Code size       411 (0x19b)
    .maxstack  8
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.s   37
    IL_0003:  stfld      int32 ExpressionTrees::'field'
    IL_0008:  call       object ExpressionTrees::X()
    IL_000d:  ldarg.0
    IL_000e:  box        ExpressionTrees
    IL_0013:  ldtoken    ExpressionTrees
    IL_0018:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0022:  ldtoken    field int32 ExpressionTrees::'field'
    IL_0027:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_002c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0031:  ldarg.0
    IL_0032:  box        ExpressionTrees
    IL_0037:  ldtoken    ExpressionTrees
    IL_003c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0041:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0046:  ldtoken    method instance int32 ExpressionTrees::C()
    IL_004b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0050:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0055:  ldc.i4.0
    IL_0056:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_005b:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0060:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0065:  ldc.i4.0
    IL_0066:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_006b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0070:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0075:  pop
    IL_0076:  call       object ExpressionTrees::X()
    IL_007b:  ldnull
    IL_007c:  ldtoken    method bool [mscorlib]System.Object::ReferenceEquals(object,
                                                                              object)
    IL_0081:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0086:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_008b:  ldc.i4.2
    IL_008c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0091:  stloc.0
    IL_0092:  ldloc.0
    IL_0093:  ldc.i4.0
    IL_0094:  ldarg.0
    IL_0095:  box        ExpressionTrees
    IL_009a:  ldtoken    ExpressionTrees
    IL_009f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a9:  stelem.ref
    IL_00aa:  ldloc.0
    IL_00ab:  ldc.i4.1
    IL_00ac:  ldtoken    method instance void ExpressionTrees::.ctor()
    IL_00b1:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00b6:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_00bb:  ldc.i4.0
    IL_00bc:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00c1:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_00c6:  stelem.ref
    IL_00c7:  ldloc.0
    IL_00c8:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00cd:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_00d2:  ldc.i4.0
    IL_00d3:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00d8:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00dd:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00e2:  pop
    IL_00e3:  call       object ExpressionTrees::X()
    IL_00e8:  ldarg.0
    IL_00e9:  box        ExpressionTrees
    IL_00ee:  ldtoken    ExpressionTrees
    IL_00f3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f8:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00fd:  ldtoken    method instance bool ExpressionTrees::MyEquals(class ExpressionTrees)
    IL_0102:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0107:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_010c:  ldc.i4.1
    IL_010d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0112:  stloc.1
    IL_0113:  ldloc.1
    IL_0114:  ldc.i4.0
    IL_0115:  ldarg.0
    IL_0116:  box        ExpressionTrees
    IL_011b:  ldtoken    ExpressionTrees
    IL_0120:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0125:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_012a:  stelem.ref
    IL_012b:  ldloc.1
    IL_012c:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0131:  ldarg.0
    IL_0132:  box        ExpressionTrees
    IL_0137:  ldtoken    ExpressionTrees
    IL_013c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0141:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0146:  ldtoken    method instance bool ExpressionTrees::MyEquals(class ExpressionTrees)
    IL_014b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0150:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0155:  ldc.i4.1
    IL_0156:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_015b:  stloc.2
    IL_015c:  ldloc.2
    IL_015d:  ldc.i4.0
    IL_015e:  ldtoken    method instance void ExpressionTrees::.ctor()
    IL_0163:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0168:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_016d:  ldc.i4.0
    IL_016e:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0173:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0178:  stelem.ref
    IL_0179:  ldloc.2
    IL_017a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_017f:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_0184:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_0189:  ldc.i4.0
    IL_018a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_018f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0194:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0199:  pop
    IL_019a:  ret
  } // end of method ExpressionTrees::DoAssert

  .method private hidebysig instance int32 
          C() cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 ExpressionTrees::'field'
    IL_0006:  ldc.i4.5
    IL_0007:  add
    IL_0008:  ret
  } // end of method ExpressionTrees::C

  .method private hidebysig instance bool 
          MyEquals(class ExpressionTrees other) cil managed
  {
    // Code size       20 (0x14)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  brfalse.s  IL_0012

    IL_0003:  ldarg.0
    IL_0004:  ldfld      int32 ExpressionTrees::'field'
    IL_0009:  ldarg.1
    IL_000a:  ldfld      int32 ExpressionTrees::'field'
    IL_000f:  ceq
    IL_0011:  ret

    IL_0012:  ldc.i4.0
    IL_0013:  ret
  } // end of method ExpressionTrees::MyEquals

  .method public hidebysig instance void 
          MethodGroupAsExtensionMethod() cil managed
  {
    // Code size       272 (0x110)
    .maxstack  10
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldtoken    method bool [System.Core]System.Linq.Enumerable::Any<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_000f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0014:  box        [mscorlib]System.Reflection.MethodInfo
    IL_0019:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_001e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0023:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0028:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_002d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0032:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0037:  ldc.i4.2
    IL_0038:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_003d:  stloc.0
    IL_003e:  ldloc.0
    IL_003f:  ldc.i4.0
    IL_0040:  ldtoken    class [mscorlib]System.Func`1<bool>
    IL_0045:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004a:  box        [mscorlib]System.Type
    IL_004f:  ldtoken    [mscorlib]System.Type
    IL_0054:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0059:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005e:  stelem.ref
    IL_005f:  ldloc.0
    IL_0060:  ldc.i4.1
    IL_0061:  ldtoken    [mscorlib]System.Int32
    IL_0066:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006b:  ldc.i4.4
    IL_006c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0071:  stloc.1
    IL_0072:  ldloc.1
    IL_0073:  ldc.i4.0
    IL_0074:  ldc.i4     0x7d0
    IL_0079:  box        [mscorlib]System.Int32
    IL_007e:  ldtoken    [mscorlib]System.Int32
    IL_0083:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0088:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_008d:  stelem.ref
    IL_008e:  ldloc.1
    IL_008f:  ldc.i4.1
    IL_0090:  ldc.i4     0x7d4
    IL_0095:  box        [mscorlib]System.Int32
    IL_009a:  ldtoken    [mscorlib]System.Int32
    IL_009f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a9:  stelem.ref
    IL_00aa:  ldloc.1
    IL_00ab:  ldc.i4.2
    IL_00ac:  ldc.i4     0x7d8
    IL_00b1:  box        [mscorlib]System.Int32
    IL_00b6:  ldtoken    [mscorlib]System.Int32
    IL_00bb:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c0:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00c5:  stelem.ref
    IL_00c6:  ldloc.1
    IL_00c7:  ldc.i4.3
    IL_00c8:  ldc.i4     0x7dc
    IL_00cd:  box        [mscorlib]System.Int32
    IL_00d2:  ldtoken    [mscorlib]System.Int32
    IL_00d7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00dc:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e1:  stelem.ref
    IL_00e2:  ldloc.1
    IL_00e3:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00e8:  stelem.ref
    IL_00e9:  ldloc.0
    IL_00ea:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00ef:  ldtoken    class [mscorlib]System.Func`1<bool>
    IL_00f4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f9:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00fe:  ldc.i4.0
    IL_00ff:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0104:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Func`1<bool>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0109:  call       object ExpressionTrees::ToCode<class [mscorlib]System.Func`1<bool>>(object,
                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_010e:  pop
    IL_010f:  ret
  } // end of method ExpressionTrees::MethodGroupAsExtensionMethod

  .method public hidebysig instance void 
          MethodGroupConstant() cil managed
  {
    // Code size       873 (0x369)
    .maxstack  11
    .locals init (class ExpressionTrees/'<>c__DisplayClass10' V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3,
             class [System.Core]System.Linq.Expressions.Expression[] V_4,
             class [System.Core]System.Linq.Expressions.Expression[] V_5,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_6,
             class [System.Core]System.Linq.Expressions.Expression[] V_7,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_8,
             class [System.Core]System.Linq.Expressions.Expression[] V_9,
             class [System.Core]System.Linq.Expressions.Expression[] V_10)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass10'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  call       object ExpressionTrees::X()
    IL_000b:  ldnull
    IL_000c:  ldtoken    method bool [mscorlib]System.Array::TrueForAll<int32>(!!0[],
                                                                               class [mscorlib]System.Predicate`1<!!0>)
    IL_0011:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0016:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_001b:  ldc.i4.2
    IL_001c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0021:  stloc.1
    IL_0022:  ldloc.1
    IL_0023:  ldc.i4.0
    IL_0024:  ldtoken    [mscorlib]System.Int32
    IL_0029:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002e:  ldc.i4.4
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0034:  stloc.2
    IL_0035:  ldloc.2
    IL_0036:  ldc.i4.0
    IL_0037:  ldc.i4     0x7d0
    IL_003c:  box        [mscorlib]System.Int32
    IL_0041:  ldtoken    [mscorlib]System.Int32
    IL_0046:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0050:  stelem.ref
    IL_0051:  ldloc.2
    IL_0052:  ldc.i4.1
    IL_0053:  ldc.i4     0x7d4
    IL_0058:  box        [mscorlib]System.Int32
    IL_005d:  ldtoken    [mscorlib]System.Int32
    IL_0062:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0067:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006c:  stelem.ref
    IL_006d:  ldloc.2
    IL_006e:  ldc.i4.2
    IL_006f:  ldc.i4     0x7d8
    IL_0074:  box        [mscorlib]System.Int32
    IL_0079:  ldtoken    [mscorlib]System.Int32
    IL_007e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0083:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0088:  stelem.ref
    IL_0089:  ldloc.2
    IL_008a:  ldc.i4.3
    IL_008b:  ldc.i4     0x7dc
    IL_0090:  box        [mscorlib]System.Int32
    IL_0095:  ldtoken    [mscorlib]System.Int32
    IL_009a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a4:  stelem.ref
    IL_00a5:  ldloc.2
    IL_00a6:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00ab:  stelem.ref
    IL_00ac:  ldloc.1
    IL_00ad:  ldc.i4.1
    IL_00ae:  ldtoken    method bool [mscorlib]System.DateTime::IsLeapYear(int32)
    IL_00b3:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00b8:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00bd:  box        [mscorlib]System.Reflection.MethodInfo
    IL_00c2:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_00c7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00cc:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d1:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_00d6:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00db:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00e0:  ldc.i4.2
    IL_00e1:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00e6:  stloc.3
    IL_00e7:  ldloc.3
    IL_00e8:  ldc.i4.0
    IL_00e9:  ldtoken    class [mscorlib]System.Predicate`1<int32>
    IL_00ee:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f3:  box        [mscorlib]System.Type
    IL_00f8:  ldtoken    [mscorlib]System.Type
    IL_00fd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0102:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0107:  stelem.ref
    IL_0108:  ldloc.3
    IL_0109:  ldc.i4.1
    IL_010a:  ldnull
    IL_010b:  ldtoken    [mscorlib]System.Object
    IL_0110:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0115:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_011a:  stelem.ref
    IL_011b:  ldloc.3
    IL_011c:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0121:  ldtoken    class [mscorlib]System.Predicate`1<int32>
    IL_0126:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_012b:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0130:  stelem.ref
    IL_0131:  ldloc.1
    IL_0132:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0137:  ldc.i4.0
    IL_0138:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_013d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0142:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0147:  pop
    IL_0148:  ldloc.0
    IL_0149:  newobj     instance void ExpressionTrees/Magic::.ctor()
    IL_014e:  stfld      class ExpressionTrees/Magic ExpressionTrees/'<>c__DisplayClass10'::test
    IL_0153:  call       object ExpressionTrees::X()
    IL_0158:  ldnull
    IL_0159:  ldtoken    method bool [System.Core]System.Linq.Enumerable::All<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                     class [mscorlib]System.Func`2<!!0,bool>)
    IL_015e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0163:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0168:  ldc.i4.2
    IL_0169:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_016e:  stloc.s    V_4
    IL_0170:  ldloc.s    V_4
    IL_0172:  ldc.i4.0
    IL_0173:  ldtoken    [mscorlib]System.Int32
    IL_0178:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_017d:  ldc.i4.4
    IL_017e:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0183:  stloc.s    V_5
    IL_0185:  ldloc.s    V_5
    IL_0187:  ldc.i4.0
    IL_0188:  ldc.i4     0x7d0
    IL_018d:  box        [mscorlib]System.Int32
    IL_0192:  ldtoken    [mscorlib]System.Int32
    IL_0197:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_019c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01a1:  stelem.ref
    IL_01a2:  ldloc.s    V_5
    IL_01a4:  ldc.i4.1
    IL_01a5:  ldc.i4     0x7d4
    IL_01aa:  box        [mscorlib]System.Int32
    IL_01af:  ldtoken    [mscorlib]System.Int32
    IL_01b4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01b9:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01be:  stelem.ref
    IL_01bf:  ldloc.s    V_5
    IL_01c1:  ldc.i4.2
    IL_01c2:  ldc.i4     0x7d8
    IL_01c7:  box        [mscorlib]System.Int32
    IL_01cc:  ldtoken    [mscorlib]System.Int32
    IL_01d1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01d6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01db:  stelem.ref
    IL_01dc:  ldloc.s    V_5
    IL_01de:  ldc.i4.3
    IL_01df:  ldc.i4     0x7dc
    IL_01e4:  box        [mscorlib]System.Int32
    IL_01e9:  ldtoken    [mscorlib]System.Int32
    IL_01ee:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01f3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01f8:  stelem.ref
    IL_01f9:  ldloc.s    V_5
    IL_01fb:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0200:  stelem.ref
    IL_0201:  ldloc.s    V_4
    IL_0203:  ldc.i4.1
    IL_0204:  ldtoken    [mscorlib]System.Int32
    IL_0209:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_020e:  ldstr      "bob"
    IL_0213:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0218:  stloc.s    V_6
    IL_021a:  ldloc.0
    IL_021b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0220:  ldtoken    field class ExpressionTrees/Magic ExpressionTrees/'<>c__DisplayClass10'::test
    IL_0225:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_022a:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_022f:  ldtoken    method instance bool ExpressionTrees/Magic::TestMagic(int32)
    IL_0234:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0239:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_023e:  ldc.i4.1
    IL_023f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0244:  stloc.s    V_7
    IL_0246:  ldloc.s    V_7
    IL_0248:  ldc.i4.0
    IL_0249:  ldloc.s    V_6
    IL_024b:  stelem.ref
    IL_024c:  ldloc.s    V_7
    IL_024e:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0253:  ldc.i4.1
    IL_0254:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0259:  stloc.s    V_8
    IL_025b:  ldloc.s    V_8
    IL_025d:  ldc.i4.0
    IL_025e:  ldloc.s    V_6
    IL_0260:  stelem.ref
    IL_0261:  ldloc.s    V_8
    IL_0263:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0268:  stelem.ref
    IL_0269:  ldloc.s    V_4
    IL_026b:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0270:  ldc.i4.0
    IL_0271:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0276:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_027b:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0280:  pop
    IL_0281:  ldloc.0
    IL_0282:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegatef'
    IL_0287:  brtrue.s   IL_029a

    IL_0289:  ldnull
    IL_028a:  ldftn      bool ExpressionTrees::'<MethodGroupConstant>b__e'(class [mscorlib]System.Func`3<object,object,bool>)
    IL_0290:  newobj     instance void class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool>::.ctor(object,
                                                                                                                                    native int)
    IL_0295:  stsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegatef'
    IL_029a:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegatef'
    IL_029f:  stfld      class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ExpressionTrees/'<>c__DisplayClass10'::sink
    IL_02a4:  call       object ExpressionTrees::X()
    IL_02a9:  ldloc.0
    IL_02aa:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_02af:  ldtoken    field class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ExpressionTrees/'<>c__DisplayClass10'::sink
    IL_02b4:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_02b9:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_02be:  ldc.i4.1
    IL_02bf:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_02c4:  stloc.s    V_9
    IL_02c6:  ldloc.s    V_9
    IL_02c8:  ldc.i4.0
    IL_02c9:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_02ce:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_02d3:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_02d8:  box        [mscorlib]System.Reflection.MethodInfo
    IL_02dd:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_02e2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02e7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_02ec:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_02f1:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_02f6:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_02fb:  ldc.i4.2
    IL_02fc:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0301:  stloc.s    V_10
    IL_0303:  ldloc.s    V_10
    IL_0305:  ldc.i4.0
    IL_0306:  ldtoken    class [mscorlib]System.Func`3<object,object,bool>
    IL_030b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0310:  box        [mscorlib]System.Type
    IL_0315:  ldtoken    [mscorlib]System.Type
    IL_031a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_031f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0324:  stelem.ref
    IL_0325:  ldloc.s    V_10
    IL_0327:  ldc.i4.1
    IL_0328:  ldnull
    IL_0329:  ldtoken    [mscorlib]System.Object
    IL_032e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0333:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0338:  stelem.ref
    IL_0339:  ldloc.s    V_10
    IL_033b:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0340:  ldtoken    class [mscorlib]System.Func`3<object,object,bool>
    IL_0345:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_034a:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_034f:  stelem.ref
    IL_0350:  ldloc.s    V_9
    IL_0352:  call       class [System.Core]System.Linq.Expressions.InvocationExpression [System.Core]System.Linq.Expressions.Expression::Invoke(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression[])
    IL_0357:  ldc.i4.0
    IL_0358:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_035d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0362:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0367:  pop
    IL_0368:  ret
  } // end of method ExpressionTrees::MethodGroupConstant

  .method public hidebysig instance void 
          MultipleCasts() cil managed
  {
    // Code size       100 (0x64)
    .maxstack  4
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldc.i4.1
    IL_0006:  box        [mscorlib]System.Int32
    IL_000b:  ldtoken    [mscorlib]System.Int32
    IL_0010:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001a:  ldc.i4.1
    IL_001b:  box        [mscorlib]System.Int32
    IL_0020:  ldtoken    [mscorlib]System.Int32
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002f:  ldtoken    [mscorlib]System.Object
    IL_0034:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0039:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_003e:  ldtoken    [mscorlib]System.Int32
    IL_0043:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0048:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_004d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0052:  ldc.i4.0
    IL_0053:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0058:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005d:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0062:  pop
    IL_0063:  ret
  } // end of method ExpressionTrees::MultipleCasts

  .method public hidebysig instance void 
          MultipleDots() cil managed
  {
    // Code size       142 (0x8e)
    .maxstack  4
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldc.i4.3
    IL_0006:  box        [mscorlib]System.Int32
    IL_000b:  ldtoken    [mscorlib]System.Int32
    IL_0010:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001a:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
    IL_001f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0024:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0029:  ldc.i4.0
    IL_002a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002f:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0034:  ldtoken    method instance string [mscorlib]System.Object::ToString()
    IL_0039:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_003e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0043:  ldc.i4.0
    IL_0044:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0049:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_004e:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_0053:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0058:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005d:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0062:  ldc.i4.0
    IL_0063:  box        [mscorlib]System.Int32
    IL_0068:  ldtoken    [mscorlib]System.Int32
    IL_006d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0072:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0077:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_007c:  ldc.i4.0
    IL_007d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0082:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0087:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008c:  pop
    IL_008d:  ret
  } // end of method ExpressionTrees::MultipleDots

  .method public hidebysig instance void 
          NestedLambda() cil managed
  {
    // Code size       572 (0x23c)
    .maxstack  10
    .locals init (class ExpressionTrees/'<>c__DisplayClass14' V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_4,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_5,
             class [System.Core]System.Linq.Expressions.Expression[] V_6,
             class [System.Core]System.Linq.Expressions.Expression[] V_7,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_8,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_9,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_10)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass14'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate13'
    IL_000c:  brtrue.s   IL_001f

    IL_000e:  ldnull
    IL_000f:  ldftn      int32 ExpressionTrees::'<NestedLambda>b__12'(class [mscorlib]System.Func`1<int32>)
    IL_0015:  newobj     instance void class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32>::.ctor(object,
                                                                                                                        native int)
    IL_001a:  stsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate13'
    IL_001f:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate13'
    IL_0024:  stfld      class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ExpressionTrees/'<>c__DisplayClass14'::'call'
    IL_0029:  call       object ExpressionTrees::X()
    IL_002e:  ldloc.0
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0034:  ldtoken    field class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ExpressionTrees/'<>c__DisplayClass14'::'call'
    IL_0039:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_003e:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0043:  ldc.i4.1
    IL_0044:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0049:  stloc.1
    IL_004a:  ldloc.1
    IL_004b:  ldc.i4.0
    IL_004c:  ldc.i4.s   42
    IL_004e:  box        [mscorlib]System.Int32
    IL_0053:  ldtoken    [mscorlib]System.Int32
    IL_0058:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0062:  ldc.i4.0
    IL_0063:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0068:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006d:  stelem.ref
    IL_006e:  ldloc.1
    IL_006f:  call       class [System.Core]System.Linq.Expressions.InvocationExpression [System.Core]System.Linq.Expressions.Expression::Invoke(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression[])
    IL_0074:  ldc.i4.0
    IL_0075:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_007f:  call       object ExpressionTrees::ToCode<int32>(object,
                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0084:  pop
    IL_0085:  call       object ExpressionTrees::X()
    IL_008a:  ldnull
    IL_008b:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<int32,int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                       class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0090:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0095:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_009a:  ldc.i4.2
    IL_009b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00a0:  stloc.2
    IL_00a1:  ldloc.2
    IL_00a2:  ldc.i4.0
    IL_00a3:  ldtoken    [mscorlib]System.Int32
    IL_00a8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ad:  ldc.i4.2
    IL_00ae:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00b3:  stloc.3
    IL_00b4:  ldloc.3
    IL_00b5:  ldc.i4.0
    IL_00b6:  ldc.i4.s   37
    IL_00b8:  box        [mscorlib]System.Int32
    IL_00bd:  ldtoken    [mscorlib]System.Int32
    IL_00c2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cc:  stelem.ref
    IL_00cd:  ldloc.3
    IL_00ce:  ldc.i4.1
    IL_00cf:  ldc.i4.s   42
    IL_00d1:  box        [mscorlib]System.Int32
    IL_00d6:  ldtoken    [mscorlib]System.Int32
    IL_00db:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e0:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e5:  stelem.ref
    IL_00e6:  ldloc.3
    IL_00e7:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00ec:  stelem.ref
    IL_00ed:  ldloc.2
    IL_00ee:  ldc.i4.1
    IL_00ef:  ldtoken    [mscorlib]System.Int32
    IL_00f4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f9:  ldstr      "x"
    IL_00fe:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0103:  stloc.s    V_4
    IL_0105:  ldloc.s    V_4
    IL_0107:  ldc.i4.2
    IL_0108:  box        [mscorlib]System.Int32
    IL_010d:  ldtoken    [mscorlib]System.Int32
    IL_0112:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0117:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_011c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0121:  ldc.i4.1
    IL_0122:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0127:  stloc.s    V_5
    IL_0129:  ldloc.s    V_5
    IL_012b:  ldc.i4.0
    IL_012c:  ldloc.s    V_4
    IL_012e:  stelem.ref
    IL_012f:  ldloc.s    V_5
    IL_0131:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0136:  stelem.ref
    IL_0137:  ldloc.2
    IL_0138:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_013d:  ldc.i4.0
    IL_013e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0143:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                              class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0148:  call       object ExpressionTrees::ToCode<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>(object,
                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_014d:  pop
    IL_014e:  call       object ExpressionTrees::X()
    IL_0153:  ldnull
    IL_0154:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<int32,int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                       class [mscorlib]System.Func`3<!!0,int32,!!1>)
    IL_0159:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_015e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0163:  ldc.i4.2
    IL_0164:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0169:  stloc.s    V_6
    IL_016b:  ldloc.s    V_6
    IL_016d:  ldc.i4.0
    IL_016e:  ldtoken    [mscorlib]System.Int32
    IL_0173:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0178:  ldc.i4.2
    IL_0179:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_017e:  stloc.s    V_7
    IL_0180:  ldloc.s    V_7
    IL_0182:  ldc.i4.0
    IL_0183:  ldc.i4.s   37
    IL_0185:  box        [mscorlib]System.Int32
    IL_018a:  ldtoken    [mscorlib]System.Int32
    IL_018f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0194:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0199:  stelem.ref
    IL_019a:  ldloc.s    V_7
    IL_019c:  ldc.i4.1
    IL_019d:  ldc.i4.s   42
    IL_019f:  box        [mscorlib]System.Int32
    IL_01a4:  ldtoken    [mscorlib]System.Int32
    IL_01a9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ae:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01b3:  stelem.ref
    IL_01b4:  ldloc.s    V_7
    IL_01b6:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_01bb:  stelem.ref
    IL_01bc:  ldloc.s    V_6
    IL_01be:  ldc.i4.1
    IL_01bf:  ldtoken    [mscorlib]System.Int32
    IL_01c4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01c9:  ldstr      "x"
    IL_01ce:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01d3:  stloc.s    V_8
    IL_01d5:  ldtoken    [mscorlib]System.Int32
    IL_01da:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01df:  ldstr      "i"
    IL_01e4:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01e9:  stloc.s    V_9
    IL_01eb:  ldloc.s    V_8
    IL_01ed:  ldc.i4.2
    IL_01ee:  box        [mscorlib]System.Int32
    IL_01f3:  ldtoken    [mscorlib]System.Int32
    IL_01f8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01fd:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0202:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0207:  ldc.i4.2
    IL_0208:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_020d:  stloc.s    V_10
    IL_020f:  ldloc.s    V_10
    IL_0211:  ldc.i4.0
    IL_0212:  ldloc.s    V_8
    IL_0214:  stelem.ref
    IL_0215:  ldloc.s    V_10
    IL_0217:  ldc.i4.1
    IL_0218:  ldloc.s    V_9
    IL_021a:  stelem.ref
    IL_021b:  ldloc.s    V_10
    IL_021d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0222:  stelem.ref
    IL_0223:  ldloc.s    V_6
    IL_0225:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_022a:  ldc.i4.0
    IL_022b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0230:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                              class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0235:  call       object ExpressionTrees::ToCode<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>(object,
                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_023a:  pop
    IL_023b:  ret
  } // end of method ExpressionTrees::NestedLambda

  .method public hidebysig instance void 
          CurriedLambda() cil managed
  {
    // Code size       145 (0x91)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_2,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_3,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_4,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_5)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Int32
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldstr      "a"
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0019:  stloc.0
    IL_001a:  ldtoken    [mscorlib]System.Int32
    IL_001f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0024:  ldstr      "b"
    IL_0029:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_002e:  stloc.1
    IL_002f:  ldtoken    [mscorlib]System.Int32
    IL_0034:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0039:  ldstr      "c"
    IL_003e:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0043:  stloc.2
    IL_0044:  ldloc.0
    IL_0045:  ldloc.1
    IL_0046:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_004b:  ldloc.2
    IL_004c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0051:  ldc.i4.1
    IL_0052:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0057:  stloc.3
    IL_0058:  ldloc.3
    IL_0059:  ldc.i4.0
    IL_005a:  ldloc.2
    IL_005b:  stelem.ref
    IL_005c:  ldloc.3
    IL_005d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0062:  ldc.i4.1
    IL_0063:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0068:  stloc.s    V_4
    IL_006a:  ldloc.s    V_4
    IL_006c:  ldc.i4.0
    IL_006d:  ldloc.1
    IL_006e:  stelem.ref
    IL_006f:  ldloc.s    V_4
    IL_0071:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0076:  ldc.i4.1
    IL_0077:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007c:  stloc.s    V_5
    IL_007e:  ldloc.s    V_5
    IL_0080:  ldc.i4.0
    IL_0081:  ldloc.0
    IL_0082:  stelem.ref
    IL_0083:  ldloc.s    V_5
    IL_0085:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008a:  call       object ExpressionTrees::ToCode<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>(object,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_008f:  pop
    IL_0090:  ret
  } // end of method ExpressionTrees::CurriedLambda

  .method private hidebysig instance bool 
          Fizz(class [mscorlib]System.Func`2<int32,bool> a) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.s   42
    IL_0003:  callvirt   instance !1 class [mscorlib]System.Func`2<int32,bool>::Invoke(!0)
    IL_0008:  ret
  } // end of method ExpressionTrees::Fizz

  .method private hidebysig instance bool 
          Buzz(class [mscorlib]System.Func`2<int32,bool> a) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldc.i4.s   42
    IL_0003:  callvirt   instance !1 class [mscorlib]System.Func`2<int32,bool>::Invoke(!0)
    IL_0008:  ret
  } // end of method ExpressionTrees::Buzz

  .method private hidebysig instance bool 
          Fizz(class [mscorlib]System.Func`2<string,bool> a) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldstr      "42"
    IL_0006:  callvirt   instance !1 class [mscorlib]System.Func`2<string,bool>::Invoke(!0)
    IL_000b:  ret
  } // end of method ExpressionTrees::Fizz

  .method public hidebysig instance void 
          NestedLambda2() cil managed
  {
    // Code size       583 (0x247)
    .maxstack  9
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_4,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_5,
             class [System.Core]System.Linq.Expressions.Expression[] V_6,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_7,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_8,
             class [System.Core]System.Linq.Expressions.Expression[] V_9,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_10,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_11)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldarg.0
    IL_0006:  box        ExpressionTrees
    IL_000b:  ldtoken    ExpressionTrees
    IL_0010:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001a:  ldtoken    method instance bool ExpressionTrees::Fizz(class [mscorlib]System.Func`2<string,bool>)
    IL_001f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0024:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0029:  ldc.i4.1
    IL_002a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002f:  stloc.0
    IL_0030:  ldloc.0
    IL_0031:  ldc.i4.0
    IL_0032:  ldtoken    [mscorlib]System.String
    IL_0037:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003c:  ldstr      "x"
    IL_0041:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0046:  stloc.1
    IL_0047:  ldloc.1
    IL_0048:  ldstr      "a"
    IL_004d:  ldtoken    [mscorlib]System.String
    IL_0052:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0057:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005c:  ldc.i4.0
    IL_005d:  ldtoken    method bool [mscorlib]System.String::op_Equality(string,
                                                                          string)
    IL_0062:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0067:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_006c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            bool,
                                                                                                                                            class [mscorlib]System.Reflection.MethodInfo)
    IL_0071:  ldc.i4.1
    IL_0072:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0077:  stloc.2
    IL_0078:  ldloc.2
    IL_0079:  ldc.i4.0
    IL_007a:  ldloc.1
    IL_007b:  stelem.ref
    IL_007c:  ldloc.2
    IL_007d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<string,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0082:  stelem.ref
    IL_0083:  ldloc.0
    IL_0084:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0089:  ldc.i4.0
    IL_008a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_008f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0094:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0099:  pop
    IL_009a:  call       object ExpressionTrees::X()
    IL_009f:  ldarg.0
    IL_00a0:  box        ExpressionTrees
    IL_00a5:  ldtoken    ExpressionTrees
    IL_00aa:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00af:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b4:  ldtoken    method instance bool ExpressionTrees::Fizz(class [mscorlib]System.Func`2<int32,bool>)
    IL_00b9:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00be:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00c3:  ldc.i4.1
    IL_00c4:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00c9:  stloc.3
    IL_00ca:  ldloc.3
    IL_00cb:  ldc.i4.0
    IL_00cc:  ldtoken    [mscorlib]System.Int32
    IL_00d1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d6:  ldstr      "x"
    IL_00db:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00e0:  stloc.s    V_4
    IL_00e2:  ldloc.s    V_4
    IL_00e4:  ldc.i4.s   37
    IL_00e6:  box        [mscorlib]System.Int32
    IL_00eb:  ldtoken    [mscorlib]System.Int32
    IL_00f0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00fa:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00ff:  ldc.i4.1
    IL_0100:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0105:  stloc.s    V_5
    IL_0107:  ldloc.s    V_5
    IL_0109:  ldc.i4.0
    IL_010a:  ldloc.s    V_4
    IL_010c:  stelem.ref
    IL_010d:  ldloc.s    V_5
    IL_010f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0114:  stelem.ref
    IL_0115:  ldloc.3
    IL_0116:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_011b:  ldc.i4.0
    IL_011c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0121:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0126:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_012b:  pop
    IL_012c:  call       object ExpressionTrees::X()
    IL_0131:  ldarg.0
    IL_0132:  box        ExpressionTrees
    IL_0137:  ldtoken    ExpressionTrees
    IL_013c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0141:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0146:  ldtoken    method instance bool ExpressionTrees::Fizz(class [mscorlib]System.Func`2<int32,bool>)
    IL_014b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0150:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0155:  ldc.i4.1
    IL_0156:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_015b:  stloc.s    V_6
    IL_015d:  ldloc.s    V_6
    IL_015f:  ldc.i4.0
    IL_0160:  ldtoken    [mscorlib]System.Int32
    IL_0165:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_016a:  ldstr      "x"
    IL_016f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0174:  stloc.s    V_7
    IL_0176:  ldc.i4.1
    IL_0177:  box        [mscorlib]System.Boolean
    IL_017c:  ldtoken    [mscorlib]System.Boolean
    IL_0181:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0186:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_018b:  ldc.i4.1
    IL_018c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0191:  stloc.s    V_8
    IL_0193:  ldloc.s    V_8
    IL_0195:  ldc.i4.0
    IL_0196:  ldloc.s    V_7
    IL_0198:  stelem.ref
    IL_0199:  ldloc.s    V_8
    IL_019b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01a0:  stelem.ref
    IL_01a1:  ldloc.s    V_6
    IL_01a3:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_01a8:  ldc.i4.0
    IL_01a9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01ae:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01b3:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_01b8:  pop
    IL_01b9:  call       object ExpressionTrees::X()
    IL_01be:  ldarg.0
    IL_01bf:  box        ExpressionTrees
    IL_01c4:  ldtoken    ExpressionTrees
    IL_01c9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ce:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01d3:  ldtoken    method instance bool ExpressionTrees::Buzz(class [mscorlib]System.Func`2<int32,bool>)
    IL_01d8:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01dd:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01e2:  ldc.i4.1
    IL_01e3:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01e8:  stloc.s    V_9
    IL_01ea:  ldloc.s    V_9
    IL_01ec:  ldc.i4.0
    IL_01ed:  ldtoken    [mscorlib]System.Int32
    IL_01f2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01f7:  ldstr      "x"
    IL_01fc:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0201:  stloc.s    V_10
    IL_0203:  ldc.i4.1
    IL_0204:  box        [mscorlib]System.Boolean
    IL_0209:  ldtoken    [mscorlib]System.Boolean
    IL_020e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0213:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0218:  ldc.i4.1
    IL_0219:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_021e:  stloc.s    V_11
    IL_0220:  ldloc.s    V_11
    IL_0222:  ldc.i4.0
    IL_0223:  ldloc.s    V_10
    IL_0225:  stelem.ref
    IL_0226:  ldloc.s    V_11
    IL_0228:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_022d:  stelem.ref
    IL_022e:  ldloc.s    V_9
    IL_0230:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0235:  ldc.i4.0
    IL_0236:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_023b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0240:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0245:  pop
    IL_0246:  ret
  } // end of method ExpressionTrees::NestedLambda2

  .method public hidebysig instance void 
          NewArrayAndExtensionMethod() cil managed
  {
    // Code size       296 (0x128)
    .maxstack  10
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method bool [System.Core]System.Linq.Enumerable::SequenceEqual<float64>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldc.i4.2
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.0
    IL_001e:  ldtoken    [mscorlib]System.Double
    IL_0023:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0028:  ldc.i4.3
    IL_0029:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002e:  stloc.1
    IL_002f:  ldloc.1
    IL_0030:  ldc.i4.0
    IL_0031:  ldc.r8     1.
    IL_003a:  box        [mscorlib]System.Double
    IL_003f:  ldtoken    [mscorlib]System.Double
    IL_0044:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0049:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004e:  stelem.ref
    IL_004f:  ldloc.1
    IL_0050:  ldc.i4.1
    IL_0051:  ldc.r8     2.0099999999999998
    IL_005a:  box        [mscorlib]System.Double
    IL_005f:  ldtoken    [mscorlib]System.Double
    IL_0064:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0069:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006e:  stelem.ref
    IL_006f:  ldloc.1
    IL_0070:  ldc.i4.2
    IL_0071:  ldc.r8     3.5
    IL_007a:  box        [mscorlib]System.Double
    IL_007f:  ldtoken    [mscorlib]System.Double
    IL_0084:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0089:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_008e:  stelem.ref
    IL_008f:  ldloc.1
    IL_0090:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0095:  stelem.ref
    IL_0096:  ldloc.0
    IL_0097:  ldc.i4.1
    IL_0098:  ldtoken    [mscorlib]System.Double
    IL_009d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a2:  ldc.i4.3
    IL_00a3:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00a8:  stloc.2
    IL_00a9:  ldloc.2
    IL_00aa:  ldc.i4.0
    IL_00ab:  ldc.r8     1.
    IL_00b4:  box        [mscorlib]System.Double
    IL_00b9:  ldtoken    [mscorlib]System.Double
    IL_00be:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00c8:  stelem.ref
    IL_00c9:  ldloc.2
    IL_00ca:  ldc.i4.1
    IL_00cb:  ldc.r8     2.0099999999999998
    IL_00d4:  box        [mscorlib]System.Double
    IL_00d9:  ldtoken    [mscorlib]System.Double
    IL_00de:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e8:  stelem.ref
    IL_00e9:  ldloc.2
    IL_00ea:  ldc.i4.2
    IL_00eb:  ldc.r8     3.5
    IL_00f4:  box        [mscorlib]System.Double
    IL_00f9:  ldtoken    [mscorlib]System.Double
    IL_00fe:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0103:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0108:  stelem.ref
    IL_0109:  ldloc.2
    IL_010a:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_010f:  stelem.ref
    IL_0110:  ldloc.0
    IL_0111:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0116:  ldc.i4.0
    IL_0117:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_011c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0121:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0126:  pop
    IL_0127:  ret
  } // end of method ExpressionTrees::NewArrayAndExtensionMethod

  .method public hidebysig instance void 
          NewMultiDimArray() cil managed
  {
    // Code size       140 (0x8c)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Int32
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldc.i4.2
    IL_0010:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0015:  stloc.0
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.0
    IL_0018:  ldc.i4.3
    IL_0019:  box        [mscorlib]System.Int32
    IL_001e:  ldtoken    [mscorlib]System.Int32
    IL_0023:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0028:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002d:  stelem.ref
    IL_002e:  ldloc.0
    IL_002f:  ldc.i4.1
    IL_0030:  ldc.i4.4
    IL_0031:  box        [mscorlib]System.Int32
    IL_0036:  ldtoken    [mscorlib]System.Int32
    IL_003b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0040:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0045:  stelem.ref
    IL_0046:  ldloc.0
    IL_0047:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayBounds(class [mscorlib]System.Type,
                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_004c:  ldtoken    method instance int32 [mscorlib]System.Array::get_Length()
    IL_0051:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0056:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0060:  ldc.i4.1
    IL_0061:  box        [mscorlib]System.Int32
    IL_0066:  ldtoken    [mscorlib]System.Int32
    IL_006b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0070:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0075:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_007a:  ldc.i4.0
    IL_007b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0080:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0085:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008a:  pop
    IL_008b:  ret
  } // end of method ExpressionTrees::NewMultiDimArray

  .method public hidebysig instance void 
          NewObject() cil managed
  {
    // Code size       80 (0x50)
    .maxstack  4
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_000a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_000f:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0014:  ldc.i4.0
    IL_0015:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001a:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_001f:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_0024:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0029:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_002e:  ldc.i4.0
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0034:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0039:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_003e:  ldc.i4.0
    IL_003f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0044:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0049:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_004e:  pop
    IL_004f:  ret
  } // end of method ExpressionTrees::NewObject

  .method public hidebysig instance void 
          NotOperator() cil managed
  {
    // Code size       240 (0xf0)
    .maxstack  4
    .locals init (class ExpressionTrees/'<>c__DisplayClass16' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass16'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  stfld      bool ExpressionTrees/'<>c__DisplayClass16'::x
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.3
    IL_000f:  stfld      int32 ExpressionTrees/'<>c__DisplayClass16'::y
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.s   42
    IL_0017:  stfld      uint8 ExpressionTrees/'<>c__DisplayClass16'::z
    IL_001c:  call       object ExpressionTrees::X()
    IL_0021:  ldloc.0
    IL_0022:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0027:  ldtoken    field uint8 ExpressionTrees/'<>c__DisplayClass16'::z
    IL_002c:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0031:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0036:  ldtoken    [mscorlib]System.Int32
    IL_003b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0040:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0045:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_004a:  ldc.i4.0
    IL_004b:  box        [mscorlib]System.Int32
    IL_0050:  ldtoken    [mscorlib]System.Int32
    IL_0055:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0064:  ldc.i4.0
    IL_0065:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_006a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006f:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0074:  pop
    IL_0075:  call       object ExpressionTrees::X()
    IL_007a:  ldloc.0
    IL_007b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0080:  ldtoken    field int32 ExpressionTrees/'<>c__DisplayClass16'::y
    IL_0085:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_008a:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_008f:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_0094:  ldc.i4.0
    IL_0095:  box        [mscorlib]System.Int32
    IL_009a:  ldtoken    [mscorlib]System.Int32
    IL_009f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a9:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00ae:  ldc.i4.0
    IL_00af:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00b4:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b9:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00be:  pop
    IL_00bf:  call       object ExpressionTrees::X()
    IL_00c4:  ldloc.0
    IL_00c5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_00ca:  ldtoken    field bool ExpressionTrees/'<>c__DisplayClass16'::x
    IL_00cf:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00d4:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00d9:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_00de:  ldc.i4.0
    IL_00df:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00e4:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00e9:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00ee:  pop
    IL_00ef:  ret
  } // end of method ExpressionTrees::NotOperator

  .method public hidebysig instance void 
          ObjectInitializers() cil managed
  {
    // Code size       255 (0xff)
    .maxstack  7
    .locals init (class ExpressionTrees/XmlReaderSettings V_0,
             class ExpressionTrees/'<>c__DisplayClass19' V_1,
             class [System.Core]System.Linq.Expressions.MemberBinding[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass19'::.ctor()
    IL_0005:  stloc.1
    IL_0006:  ldloc.1
    IL_0007:  newobj     instance void ExpressionTrees/XmlReaderSettings::.ctor()
    IL_000c:  stloc.0
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.0
    IL_000f:  stfld      bool ExpressionTrees/XmlReaderSettings::CloseInput
    IL_0014:  ldloc.0
    IL_0015:  ldc.i4.0
    IL_0016:  stfld      bool ExpressionTrees/XmlReaderSettings::CheckCharacters
    IL_001b:  ldloc.0
    IL_001c:  stfld      class ExpressionTrees/XmlReaderSettings ExpressionTrees/'<>c__DisplayClass19'::s
    IL_0021:  call       object ExpressionTrees::X()
    IL_0026:  ldtoken    method instance void ExpressionTrees/XmlReaderSettings::.ctor()
    IL_002b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0030:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0035:  ldc.i4.0
    IL_0036:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_003b:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0040:  ldc.i4.2
    IL_0041:  newarr     [System.Core]System.Linq.Expressions.MemberBinding
    IL_0046:  stloc.2
    IL_0047:  ldloc.2
    IL_0048:  ldc.i4.0
    IL_0049:  ldtoken    field bool ExpressionTrees/XmlReaderSettings::CloseInput
    IL_004e:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0053:  ldloc.1
    IL_0054:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0059:  ldtoken    field class ExpressionTrees/XmlReaderSettings ExpressionTrees/'<>c__DisplayClass19'::s
    IL_005e:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0063:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0068:  ldtoken    field bool ExpressionTrees/XmlReaderSettings::CloseInput
    IL_006d:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0072:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0077:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MemberInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_007c:  stelem.ref
    IL_007d:  ldloc.2
    IL_007e:  ldc.i4.1
    IL_007f:  ldtoken    field bool ExpressionTrees/XmlReaderSettings::CheckCharacters
    IL_0084:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0089:  ldloc.1
    IL_008a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_008f:  ldtoken    field class ExpressionTrees/XmlReaderSettings ExpressionTrees/'<>c__DisplayClass19'::s
    IL_0094:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0099:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_009e:  ldtoken    field bool ExpressionTrees/XmlReaderSettings::CheckCharacters
    IL_00a3:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00a8:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00ad:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MemberInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_00b2:  stelem.ref
    IL_00b3:  ldloc.2
    IL_00b4:  call       class [System.Core]System.Linq.Expressions.MemberInitExpression [System.Core]System.Linq.Expressions.Expression::MemberInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.MemberBinding[])
    IL_00b9:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_00be:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00c3:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00c8:  ldc.i4.1
    IL_00c9:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00ce:  stloc.3
    IL_00cf:  ldloc.3
    IL_00d0:  ldc.i4.0
    IL_00d1:  ldloc.1
    IL_00d2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_00d7:  ldtoken    field class ExpressionTrees/XmlReaderSettings ExpressionTrees/'<>c__DisplayClass19'::s
    IL_00dc:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00e1:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00e6:  stelem.ref
    IL_00e7:  ldloc.3
    IL_00e8:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00ed:  ldc.i4.0
    IL_00ee:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00f3:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00f8:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00fd:  pop
    IL_00fe:  ret
  } // end of method ExpressionTrees::ObjectInitializers

  .method public hidebysig instance void 
          Quoted() cil managed
  {
    // Code size       180 (0xb4)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_2)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Int32
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldstr      "n"
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0019:  stloc.0
    IL_001a:  ldtoken    [mscorlib]System.String
    IL_001f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0024:  ldstr      "s"
    IL_0029:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_002e:  stloc.1
    IL_002f:  ldloc.1
    IL_0030:  ldloc.0
    IL_0031:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
    IL_0036:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_003b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0040:  ldc.i4.0
    IL_0041:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0046:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_004b:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_0050:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0055:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005a:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_005f:  ldc.i4.2
    IL_0060:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0065:  stloc.2
    IL_0066:  ldloc.2
    IL_0067:  ldc.i4.0
    IL_0068:  ldloc.0
    IL_0069:  stelem.ref
    IL_006a:  ldloc.2
    IL_006b:  ldc.i4.1
    IL_006c:  ldloc.1
    IL_006d:  stelem.ref
    IL_006e:  ldloc.2
    IL_006f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,string,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0074:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Quote(class [System.Core]System.Linq.Expressions.Expression)
    IL_0079:  ldtoken    class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<int32,string,string>>
    IL_007e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0083:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0088:  ldnull
    IL_0089:  box        [mscorlib]System.Object
    IL_008e:  ldtoken    [mscorlib]System.Object
    IL_0093:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0098:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_00a2:  ldc.i4.0
    IL_00a3:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a8:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ad:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00b2:  pop
    IL_00b3:  ret
  } // end of method ExpressionTrees::Quoted

  .method public hidebysig instance void 
          Quoted2() cil managed
  {
    // Code size       174 (0xae)
    .maxstack  8
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method object ExpressionTrees::ToCode<bool>(object,
                                                                     class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldc.i4.2
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.0
    IL_001e:  ldnull
    IL_001f:  ldtoken    method object ExpressionTrees::X()
    IL_0024:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0029:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002e:  ldc.i4.0
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0034:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0039:  stelem.ref
    IL_003a:  ldloc.0
    IL_003b:  ldc.i4.1
    IL_003c:  ldc.i4.1
    IL_003d:  box        [mscorlib]System.Boolean
    IL_0042:  ldtoken    [mscorlib]System.Boolean
    IL_0047:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0051:  ldc.i4.0
    IL_0052:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0057:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005c:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Quote(class [System.Core]System.Linq.Expressions.Expression)
    IL_0061:  stelem.ref
    IL_0062:  ldloc.0
    IL_0063:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0068:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_006d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0072:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0077:  ldc.i4.1
    IL_0078:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_007d:  stloc.1
    IL_007e:  ldloc.1
    IL_007f:  ldc.i4.0
    IL_0080:  ldnull
    IL_0081:  box        [mscorlib]System.Object
    IL_0086:  ldtoken    [mscorlib]System.Object
    IL_008b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0090:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0095:  stelem.ref
    IL_0096:  ldloc.1
    IL_0097:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_009c:  ldc.i4.0
    IL_009d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a2:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00a7:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00ac:  pop
    IL_00ad:  ret
  } // end of method ExpressionTrees::Quoted2

  .method public hidebysig instance void 
          QuotedWithAnonymous() cil managed
  {
    // Code size       370 (0x172)
    .maxstack  18
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3,
             class [mscorlib]System.Reflection.MethodInfo[] V_4,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_5,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_6)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method !!0 [System.Core]System.Linq.Enumerable::Single<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldc.i4.1
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.0
    IL_001e:  ldnull
    IL_001f:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType1`2'<string,string>,string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                               class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0024:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0029:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002e:  ldc.i4.2
    IL_002f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0034:  stloc.1
    IL_0035:  ldloc.1
    IL_0036:  ldc.i4.0
    IL_0037:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_003c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0041:  ldc.i4.1
    IL_0042:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0047:  stloc.2
    IL_0048:  ldloc.2
    IL_0049:  ldc.i4.0
    IL_004a:  ldtoken    method instance void class '<>f__AnonymousType1`2'<string,string>::.ctor(!0,
                                                                                                  !1)
    IL_004f:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_0054:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0059:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_005e:  ldc.i4.2
    IL_005f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0064:  stloc.3
    IL_0065:  ldloc.3
    IL_0066:  ldc.i4.0
    IL_0067:  ldstr      "a"
    IL_006c:  ldtoken    [mscorlib]System.String
    IL_0071:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0076:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_007b:  stelem.ref
    IL_007c:  ldloc.3
    IL_007d:  ldc.i4.1
    IL_007e:  ldstr      "b"
    IL_0083:  ldtoken    [mscorlib]System.String
    IL_0088:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0092:  stelem.ref
    IL_0093:  ldloc.3
    IL_0094:  ldc.i4.2
    IL_0095:  newarr     [mscorlib]System.Reflection.MethodInfo
    IL_009a:  stloc.s    V_4
    IL_009c:  ldloc.s    V_4
    IL_009e:  ldc.i4.0
    IL_009f:  ldtoken    method instance !0 class '<>f__AnonymousType1`2'<string,string>::get_X()
    IL_00a4:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00a9:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ae:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00b3:  stelem.ref
    IL_00b4:  ldloc.s    V_4
    IL_00b6:  ldc.i4.1
    IL_00b7:  ldtoken    method instance !1 class '<>f__AnonymousType1`2'<string,string>::get_Y()
    IL_00bc:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00c1:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c6:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00cb:  stelem.ref
    IL_00cc:  ldloc.s    V_4
    IL_00ce:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>,
                                                                                                                                       class [mscorlib]System.Reflection.MemberInfo[])
    IL_00d3:  stelem.ref
    IL_00d4:  ldloc.2
    IL_00d5:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00da:  stelem.ref
    IL_00db:  ldloc.1
    IL_00dc:  ldc.i4.1
    IL_00dd:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00e2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e7:  ldstr      "o"
    IL_00ec:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00f1:  stloc.s    V_5
    IL_00f3:  ldloc.s    V_5
    IL_00f5:  ldtoken    method instance !0 class '<>f__AnonymousType1`2'<string,string>::get_X()
    IL_00fa:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00ff:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0104:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0109:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_010e:  ldloc.s    V_5
    IL_0110:  ldtoken    method instance !1 class '<>f__AnonymousType1`2'<string,string>::get_Y()
    IL_0115:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_011a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_011f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0124:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0129:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_012e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0133:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0138:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_013d:  ldc.i4.1
    IL_013e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0143:  stloc.s    V_6
    IL_0145:  ldloc.s    V_6
    IL_0147:  ldc.i4.0
    IL_0148:  ldloc.s    V_5
    IL_014a:  stelem.ref
    IL_014b:  ldloc.s    V_6
    IL_014d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<string,string>,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0152:  stelem.ref
    IL_0153:  ldloc.1
    IL_0154:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0159:  stelem.ref
    IL_015a:  ldloc.0
    IL_015b:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0160:  ldc.i4.0
    IL_0161:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0166:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_016b:  call       object ExpressionTrees::ToCode<string>(object,
                                                                class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0170:  pop
    IL_0171:  ret
  } // end of method ExpressionTrees::QuotedWithAnonymous

  .method public hidebysig instance void 
          StaticCall() cil managed
  {
    // Code size       130 (0x82)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldc.i4.2
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.0
    IL_001e:  ldc.i4.3
    IL_001f:  box        [mscorlib]System.Int32
    IL_0024:  ldtoken    [mscorlib]System.Int32
    IL_0029:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0033:  ldtoken    [mscorlib]System.Object
    IL_0038:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0042:  stelem.ref
    IL_0043:  ldloc.0
    IL_0044:  ldc.i4.1
    IL_0045:  ldc.i4.0
    IL_0046:  box        [mscorlib]System.Int32
    IL_004b:  ldtoken    [mscorlib]System.Int32
    IL_0050:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0055:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005a:  ldtoken    [mscorlib]System.Object
    IL_005f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0064:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0069:  stelem.ref
    IL_006a:  ldloc.0
    IL_006b:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0070:  ldc.i4.0
    IL_0071:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0076:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_007b:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0080:  pop
    IL_0081:  ret
  } // end of method ExpressionTrees::StaticCall

  .method public hidebysig instance void 
          ThisCall() cil managed
  {
    // Code size       116 (0x74)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldarg.0
    IL_0006:  box        ExpressionTrees
    IL_000b:  ldtoken    ExpressionTrees
    IL_0010:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001a:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_001f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0024:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0029:  ldc.i4.1
    IL_002a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002f:  stloc.0
    IL_0030:  ldloc.0
    IL_0031:  ldc.i4.0
    IL_0032:  ldc.i4.3
    IL_0033:  box        [mscorlib]System.Int32
    IL_0038:  ldtoken    [mscorlib]System.Int32
    IL_003d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0042:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0047:  ldtoken    [mscorlib]System.Object
    IL_004c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0051:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0056:  stelem.ref
    IL_0057:  ldloc.0
    IL_0058:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_005d:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_0062:  ldc.i4.0
    IL_0063:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0068:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006d:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0072:  pop
    IL_0073:  ret
  } // end of method ExpressionTrees::ThisCall

  .method public hidebysig instance void 
          ThisExplicit() cil managed
  {
    // Code size       115 (0x73)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldc.i4.2
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.0
    IL_001e:  ldarg.0
    IL_001f:  box        ExpressionTrees
    IL_0024:  ldtoken    ExpressionTrees
    IL_0029:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0033:  stelem.ref
    IL_0034:  ldloc.0
    IL_0035:  ldc.i4.1
    IL_0036:  ldc.i4.3
    IL_0037:  box        [mscorlib]System.Int32
    IL_003c:  ldtoken    [mscorlib]System.Int32
    IL_0041:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0046:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004b:  ldtoken    [mscorlib]System.Object
    IL_0050:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0055:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_005a:  stelem.ref
    IL_005b:  ldloc.0
    IL_005c:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0061:  ldc.i4.0
    IL_0062:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0067:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006c:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0071:  pop
    IL_0072:  ret
  } // end of method ExpressionTrees::ThisExplicit

  .method public hidebysig instance void 
          TypedConstant() cil managed
  {
    // Code size       112 (0x70)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldtoken    [mscorlib]System.Type
    IL_000a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000f:  ldc.i4.2
    IL_0010:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0015:  stloc.0
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.0
    IL_0018:  ldtoken    [mscorlib]System.Int32
    IL_001d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0022:  box        [mscorlib]System.Type
    IL_0027:  ldtoken    [mscorlib]System.Type
    IL_002c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0031:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0036:  stelem.ref
    IL_0037:  ldloc.0
    IL_0038:  ldc.i4.1
    IL_0039:  ldtoken    [mscorlib]System.String
    IL_003e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0043:  box        [mscorlib]System.Type
    IL_0048:  ldtoken    [mscorlib]System.Type
    IL_004d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0052:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0057:  stelem.ref
    IL_0058:  ldloc.0
    IL_0059:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_005e:  ldc.i4.0
    IL_005f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0064:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type[]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0069:  call       object ExpressionTrees::ToCode<class [mscorlib]System.Type[]>(object,
                                                                                       class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_006e:  pop
    IL_006f:  ret
  } // end of method ExpressionTrees::TypedConstant

  .method public hidebysig instance void 
          StaticCallImplicitCast() cil managed
  {
    // Code size       130 (0x82)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  ldc.i4.2
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.0
    IL_001e:  ldc.i4.3
    IL_001f:  box        [mscorlib]System.Int32
    IL_0024:  ldtoken    [mscorlib]System.Int32
    IL_0029:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0033:  ldtoken    [mscorlib]System.Object
    IL_0038:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0042:  stelem.ref
    IL_0043:  ldloc.0
    IL_0044:  ldc.i4.1
    IL_0045:  ldc.i4.0
    IL_0046:  box        [mscorlib]System.Int32
    IL_004b:  ldtoken    [mscorlib]System.Int32
    IL_0050:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0055:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005a:  ldtoken    [mscorlib]System.Object
    IL_005f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0064:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0069:  stelem.ref
    IL_006a:  ldloc.0
    IL_006b:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0070:  ldc.i4.0
    IL_0071:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0076:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_007b:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0080:  pop
    IL_0081:  ret
  } // end of method ExpressionTrees::StaticCallImplicitCast

  .method public hidebysig instance void 
          StaticMembers() cil managed
  {
    // Code size       234 (0xea)
    .maxstack  9
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_001a:  ldnull
    IL_001b:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_0020:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0025:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002a:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_002f:  ldnull
    IL_0030:  ldtoken    method valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::FromMilliseconds(float64)
    IL_0035:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_003a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_003f:  ldc.i4.1
    IL_0040:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0045:  stloc.0
    IL_0046:  ldloc.0
    IL_0047:  ldc.i4.0
    IL_0048:  ldc.r8     10.000999999999999
    IL_0051:  box        [mscorlib]System.Double
    IL_0056:  ldtoken    [mscorlib]System.Double
    IL_005b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0060:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0065:  stelem.ref
    IL_0066:  ldloc.0
    IL_0067:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_006c:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::op_Addition(valuetype [mscorlib]System.DateTime,
                                                                                                           valuetype [mscorlib]System.TimeSpan)
    IL_0071:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0076:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_0080:  ldc.i4.0
    IL_0081:  ldtoken    method bool [mscorlib]System.DateTime::op_GreaterThan(valuetype [mscorlib]System.DateTime,
                                                                               valuetype [mscorlib]System.DateTime)
    IL_0086:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_008b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0090:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  bool,
                                                                                                                                                  class [mscorlib]System.Reflection.MethodInfo)
    IL_0095:  ldtoken    method instance string [mscorlib]System.Boolean::ToString()
    IL_009a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_009f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00a4:  ldc.i4.0
    IL_00a5:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00aa:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00af:  ldstr      "False"
    IL_00b4:  ldtoken    [mscorlib]System.String
    IL_00b9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00be:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00c3:  ldc.i4.0
    IL_00c4:  ldtoken    method bool [mscorlib]System.String::op_Equality(string,
                                                                          string)
    IL_00c9:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00ce:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00d3:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            bool,
                                                                                                                                            class [mscorlib]System.Reflection.MethodInfo)
    IL_00d8:  ldc.i4.0
    IL_00d9:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00de:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00e3:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00e8:  pop
    IL_00e9:  ret
  } // end of method ExpressionTrees::StaticMembers

  .method public hidebysig instance void 
          Strings() cil managed
  {
    // Code size       376 (0x178)
    .maxstack  8
    .locals init (class ExpressionTrees/'<>c__DisplayClass1b' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass1b'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  stfld      int32 ExpressionTrees/'<>c__DisplayClass1b'::i
    IL_000d:  ldloc.0
    IL_000e:  ldstr      "X"
    IL_0013:  stfld      string ExpressionTrees/'<>c__DisplayClass1b'::x
    IL_0018:  call       object ExpressionTrees::X()
    IL_001d:  ldstr      "a\n\\b"
    IL_0022:  ldtoken    [mscorlib]System.String
    IL_0027:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0031:  ldloc.0
    IL_0032:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0037:  ldtoken    field string ExpressionTrees/'<>c__DisplayClass1b'::x
    IL_003c:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0041:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0046:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Coalesce(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_004b:  ldloc.0
    IL_004c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0051:  ldtoken    field string ExpressionTrees/'<>c__DisplayClass1b'::x
    IL_0056:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_005b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0060:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_0065:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_006a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_006f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_0074:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_0079:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_007e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0083:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0088:  ldc.i4.2
    IL_0089:  box        [mscorlib]System.Int32
    IL_008e:  ldtoken    [mscorlib]System.Int32
    IL_0093:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0098:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00a2:  ldc.i4.0
    IL_00a3:  box        [mscorlib]System.Boolean
    IL_00a8:  ldtoken    [mscorlib]System.Boolean
    IL_00ad:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b7:  ldc.i4.1
    IL_00b8:  box        [mscorlib]System.Boolean
    IL_00bd:  ldtoken    [mscorlib]System.Boolean
    IL_00c2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cc:  ldc.i4.1
    IL_00cd:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_00d2:  box        [mscorlib]System.Decimal
    IL_00d7:  ldtoken    [mscorlib]System.Decimal
    IL_00dc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e6:  ldloc.0
    IL_00e7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_00ec:  ldtoken    field int32 ExpressionTrees/'<>c__DisplayClass1b'::i
    IL_00f1:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00f6:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00fb:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Negate(class [System.Core]System.Linq.Expressions.Expression)
    IL_0100:  ldtoken    [mscorlib]System.Decimal
    IL_0105:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010a:  ldtoken    method valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(int32)
    IL_010f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0114:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0119:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type,
                                                                                                                                             class [mscorlib]System.Reflection.MethodInfo)
    IL_011e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0123:  ldc.i4.0
    IL_0124:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_0129:  box        [mscorlib]System.Decimal
    IL_012e:  ldtoken    [mscorlib]System.Decimal
    IL_0133:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0138:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_013d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0142:  ldc.i4.0
    IL_0143:  box        [mscorlib]System.Boolean
    IL_0148:  ldtoken    [mscorlib]System.Boolean
    IL_014d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0152:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0157:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::OrElse(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_015c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_0161:  call       class [System.Core]System.Linq.Expressions.ConditionalExpression [System.Core]System.Linq.Expressions.Expression::Condition(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression)
    IL_0166:  ldc.i4.0
    IL_0167:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_016c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0171:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0176:  pop
    IL_0177:  ret
  } // end of method ExpressionTrees::Strings

  .method public hidebysig instance void 
          StringAccessor() cil managed
  {
    // Code size       137 (0x89)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldstr      "abc"
    IL_000a:  ldtoken    [mscorlib]System.String
    IL_000f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0019:  ldtoken    method instance char [mscorlib]System.String::get_Chars(int32)
    IL_001e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0023:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0028:  ldc.i4.1
    IL_0029:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002e:  stloc.0
    IL_002f:  ldloc.0
    IL_0030:  ldc.i4.0
    IL_0031:  ldc.i4.1
    IL_0032:  box        [mscorlib]System.Int32
    IL_0037:  ldtoken    [mscorlib]System.Int32
    IL_003c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0041:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0046:  stelem.ref
    IL_0047:  ldloc.0
    IL_0048:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_004d:  ldtoken    [mscorlib]System.Int32
    IL_0052:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0057:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_005c:  ldc.i4.s   98
    IL_005e:  box        [mscorlib]System.Int32
    IL_0063:  ldtoken    [mscorlib]System.Int32
    IL_0068:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0072:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0077:  ldc.i4.0
    IL_0078:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0082:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0087:  pop
    IL_0088:  ret
  } // end of method ExpressionTrees::StringAccessor

  .method public hidebysig instance void 
          GenericClassInstance() cil managed
  {
    // Code size       150 (0x96)
    .maxstack  5
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldtoken    method instance void class ExpressionTrees/GenericClass`1<int32>::.ctor()
    IL_000a:  ldtoken    class ExpressionTrees/GenericClass`1<int32>
    IL_000f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0014:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0019:  ldc.i4.0
    IL_001a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001f:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0024:  ldtoken    field !0 class ExpressionTrees/GenericClass`1<int32>::InstanceField
    IL_0029:  ldtoken    class ExpressionTrees/GenericClass`1<int32>
    IL_002e:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle,
                                                                                                                               valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0033:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0038:  ldtoken    [mscorlib]System.Double
    IL_003d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0042:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0047:  ldtoken    method instance void class ExpressionTrees/GenericClass`1<float64>::.ctor()
    IL_004c:  ldtoken    class ExpressionTrees/GenericClass`1<float64>
    IL_0051:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0056:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_005b:  ldc.i4.0
    IL_005c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0061:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0066:  ldtoken    method instance !0 class ExpressionTrees/GenericClass`1<float64>::get_InstanceProperty()
    IL_006b:  ldtoken    class ExpressionTrees/GenericClass`1<float64>
    IL_0070:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0075:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007a:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_007f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0084:  ldc.i4.0
    IL_0085:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_008a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<float64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                      class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008f:  call       object ExpressionTrees::ToCode<float64>(object,
                                                                 class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0094:  pop
    IL_0095:  ret
  } // end of method ExpressionTrees::GenericClassInstance

  .method public hidebysig instance void 
          GenericClassStatic() cil managed
  {
    // Code size       90 (0x5a)
    .maxstack  5
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    field !0 class ExpressionTrees/GenericClass`1<int32>::StaticField
    IL_000b:  ldtoken    class ExpressionTrees/GenericClass`1<int32>
    IL_0010:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle,
                                                                                                                               valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_001a:  ldtoken    [mscorlib]System.Double
    IL_001f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0024:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0029:  ldnull
    IL_002a:  ldtoken    method !0 class ExpressionTrees/GenericClass`1<float64>::get_StaticProperty()
    IL_002f:  ldtoken    class ExpressionTrees/GenericClass`1<float64>
    IL_0034:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0039:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_003e:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0043:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0048:  ldc.i4.0
    IL_0049:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_004e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<float64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                      class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0053:  call       object ExpressionTrees::ToCode<float64>(object,
                                                                 class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0058:  pop
    IL_0059:  ret
  } // end of method ExpressionTrees::GenericClassStatic

  .method public hidebysig instance void 
          InvokeGenericMethod() cil managed
  {
    // Code size       55 (0x37)
    .maxstack  8
    IL_0000:  call       object ExpressionTrees::X()
    IL_0005:  ldnull
    IL_0006:  ldtoken    method bool class ExpressionTrees/GenericClass`1<int32>::GenericMethod<float64>()
    IL_000b:  ldtoken    class ExpressionTrees/GenericClass`1<int32>
    IL_0010:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_001a:  ldc.i4.0
    IL_001b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0020:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0025:  ldc.i4.0
    IL_0026:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_002b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0030:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0035:  pop
    IL_0036:  ret
  } // end of method ExpressionTrees::InvokeGenericMethod

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method ExpressionTrees::.ctor

  .method private hidebysig static string 
          '<IndexerAccess>b__6'(int32 n) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarga.s   n
    IL_0002:  call       instance string [mscorlib]System.Int32::ToString()
    IL_0007:  ret
  } // end of method ExpressionTrees::'<IndexerAccess>b__6'

  .method private hidebysig static bool  '<MethodGroupConstant>b__e'(class [mscorlib]System.Func`3<object,object,bool> f) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !2 class [mscorlib]System.Func`3<object,object,bool>::Invoke(!0,
                                                                                               !1)
    IL_0008:  ret
  } // end of method ExpressionTrees::'<MethodGroupConstant>b__e'

  .method private hidebysig static int32 
          '<NestedLambda>b__12'(class [mscorlib]System.Func`1<int32> f) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0006:  ret
  } // end of method ExpressionTrees::'<NestedLambda>b__12'

} // end of class ExpressionTrees

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType0`2'<'<X>j__TPar','<A>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<X>j__TPar' '<X>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<A>j__TPar' '<A>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<X>j__TPar' X,
                               !'<A>j__TPar' A) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType0`2'::.ctor

  .method public hidebysig specialname instance !'<X>j__TPar' 
          get_X() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType0`2'::get_X

  .method public hidebysig specialname instance !'<A>j__TPar' 
          get_A() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType0`2'::get_A

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       85 (0x55)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ X = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_0019:  box        !'<X>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", A = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_0037:  box        !'<A>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      " }"
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0054:  ret
  } // end of method '<>f__AnonymousType0`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType0`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4     0xed16d9c
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ret
  } // end of method '<>f__AnonymousType0`2'::GetHashCode

  .property instance !'<X>j__TPar' X()
  {
    .get instance !'<X>j__TPar' '<>f__AnonymousType0`2'::get_X()
  } // end of property '<>f__AnonymousType0`2'::X
  .property instance !'<A>j__TPar' A()
  {
    .get instance !'<A>j__TPar' '<>f__AnonymousType0`2'::get_A()
  } // end of property '<>f__AnonymousType0`2'::A
} // end of class '<>f__AnonymousType0`2'

.class private auto ansi sealed beforefieldinit '<>f__AnonymousType1`2'<'<X>j__TPar','<Y>j__TPar'>
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .field private initonly !'<X>j__TPar' '<X>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .field private initonly !'<Y>j__TPar' '<Y>i__Field'
  .custom instance void [mscorlib]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(!'<X>j__TPar' X,
                               !'<Y>j__TPar' Y) cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_000d:  ldarg.0
    IL_000e:  ldarg.2
    IL_000f:  stfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0014:  ret
  } // end of method '<>f__AnonymousType1`2'::.ctor

  .method public hidebysig specialname instance !'<X>j__TPar' 
          get_X() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType1`2'::get_X

  .method public hidebysig specialname instance !'<Y>j__TPar' 
          get_Y() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0006:  ret
  } // end of method '<>f__AnonymousType1`2'::get_Y

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       85 (0x55)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0)
    IL_0000:  newobj     instance void [mscorlib]System.Text.StringBuilder::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldstr      "{ X = "
    IL_000c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_0011:  pop
    IL_0012:  ldloc.0
    IL_0013:  ldarg.0
    IL_0014:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0019:  box        !'<X>j__TPar'
    IL_001e:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0023:  pop
    IL_0024:  ldloc.0
    IL_0025:  ldstr      ", Y = "
    IL_002a:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_002f:  pop
    IL_0030:  ldloc.0
    IL_0031:  ldarg.0
    IL_0032:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0037:  box        !'<Y>j__TPar'
    IL_003c:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(object)
    IL_0041:  pop
    IL_0042:  ldloc.0
    IL_0043:  ldstr      " }"
    IL_0048:  callvirt   instance class [mscorlib]System.Text.StringBuilder [mscorlib]System.Text.StringBuilder::Append(string)
    IL_004d:  pop
    IL_004e:  ldloc.0
    IL_004f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0054:  ret
  } // end of method '<>f__AnonymousType1`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'> V_0)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_0039

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_0039

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  ret

    IL_0039:  ldc.i4.0
    IL_003a:  ret
  } // end of method '<>f__AnonymousType1`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       58 (0x3a)
    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4     0xc18f39dd
    IL_0005:  stloc.0
    IL_0006:  ldc.i4     0xa5555529
    IL_000b:  ldloc.0
    IL_000c:  mul
    IL_000d:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_0012:  ldarg.0
    IL_0013:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0018:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::GetHashCode(!0)
    IL_001d:  add
    IL_001e:  stloc.0
    IL_001f:  ldc.i4     0xa5555529
    IL_0024:  ldloc.0
    IL_0025:  mul
    IL_0026:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::get_Default()
    IL_002b:  ldarg.0
    IL_002c:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0031:  callvirt   instance int32 class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::GetHashCode(!0)
    IL_0036:  add
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  ret
  } // end of method '<>f__AnonymousType1`2'::GetHashCode

  .property instance !'<X>j__TPar' X()
  {
    .get instance !'<X>j__TPar' '<>f__AnonymousType1`2'::get_X()
  } // end of property '<>f__AnonymousType1`2'::X
  .property instance !'<Y>j__TPar' Y()
  {
    .get instance !'<Y>j__TPar' '<>f__AnonymousType1`2'::get_Y()
  } // end of property '<>f__AnonymousType1`2'::Y
} // end of class '<>f__AnonymousType1`2'


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\ExpressionTrees.opt.res
