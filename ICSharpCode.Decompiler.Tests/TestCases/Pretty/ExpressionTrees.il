
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
.assembly extern System.Xml
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly '05r5is52'
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module '05r5is52.dll'
// MVID: {F70BBCF1-4F46-4420-BD20-CAE864CFD4A5}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00F10000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ExpressionTrees
       extends [mscorlib]System.Object
{
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
      // Code size       10 (0xa)
      .maxstack  1
      .locals init (!X V_0)
      IL_0000:  ldsfld     !0 class ExpressionTrees/GenericClass`1<!X>::'<StaticProperty>k__BackingField'
      IL_0005:  stloc.0
      IL_0006:  br.s       IL_0008

      IL_0008:  ldloc.0
      IL_0009:  ret
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
      // Code size       11 (0xb)
      .maxstack  1
      .locals init (!X V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      !0 class ExpressionTrees/GenericClass`1<!X>::'<InstanceProperty>k__BackingField'
      IL_0006:  stloc.0
      IL_0007:  br.s       IL_0009

      IL_0009:  ldloc.0
      IL_000a:  ret
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
      // Code size       7 (0x7)
      .maxstack  1
      .locals init (bool V_0)
      IL_0000:  nop
      IL_0001:  ldc.i4.0
      IL_0002:  stloc.0
      IL_0003:  br.s       IL_0005

      IL_0005:  ldloc.0
      IL_0006:  ret
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
    .field public class [System.Core]System.Collections.Generic.HashSet`1<int32> set
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
    .field public class [System.Xml]System.Xml.XmlReaderSettings s
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
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (object V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method ExpressionTrees::ToCode

  .method private hidebysig static object 
          ToCode<T,R>(object x,
                      class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!T,!!R>> expr) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (object V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method ExpressionTrees::ToCode

  .method private hidebysig static object 
          X() cil managed
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
  } // end of method ExpressionTrees::X

  .method public hidebysig instance void 
          Parameter(bool a) cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class ExpressionTrees/'<>c__DisplayClass0' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass0'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      bool ExpressionTrees/'<>c__DisplayClass0'::a
    IL_000d:  nop
    IL_000e:  call       object ExpressionTrees::X()
    IL_0013:  ldloc.0
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0019:  ldtoken    field bool ExpressionTrees/'<>c__DisplayClass0'::a
    IL_001e:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0023:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0028:  ldc.i4.0
    IL_0029:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_002e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0033:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0038:  pop
    IL_0039:  nop
    IL_003a:  ret
  } // end of method ExpressionTrees::Parameter

  .method public hidebysig instance void 
          LocalVariable() cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  3
    .locals init (class ExpressionTrees/'<>c__DisplayClass2' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass2'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      bool ExpressionTrees/'<>c__DisplayClass2'::a
    IL_000e:  call       object ExpressionTrees::X()
    IL_0013:  ldloc.0
    IL_0014:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0019:  ldtoken    field bool ExpressionTrees/'<>c__DisplayClass2'::a
    IL_001e:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0023:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0028:  ldc.i4.0
    IL_0029:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_002e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0033:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0038:  pop
    IL_0039:  nop
    IL_003a:  ret
  } // end of method ExpressionTrees::LocalVariable

  .method public hidebysig instance void 
          LambdaParameter() cil managed
  {
    // Code size       52 (0x34)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Boolean
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldstr      "a"
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_001a:  stloc.0
    IL_001b:  ldloc.0
    IL_001c:  ldc.i4.1
    IL_001d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0022:  stloc.1
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.0
    IL_0025:  ldloc.0
    IL_0026:  stelem.ref
    IL_0027:  ldloc.1
    IL_0028:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<bool,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                        class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_002d:  call       object ExpressionTrees::ToCode<bool,bool>(object,
                                                                   class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_0032:  pop
    IL_0033:  ret
  } // end of method ExpressionTrees::LambdaParameter

  .method public hidebysig instance void 
          AddOperator(int32 x) cil managed
  {
    // Code size       111 (0x6f)
    .maxstack  4
    .locals init (class ExpressionTrees/'<>c__DisplayClass4' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass4'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.1
    IL_0008:  stfld      int32 ExpressionTrees/'<>c__DisplayClass4'::x
    IL_000d:  nop
    IL_000e:  call       object ExpressionTrees::X()
    IL_0013:  ldc.i4.1
    IL_0014:  box        [mscorlib]System.Int32
    IL_0019:  ldtoken    [mscorlib]System.Int32
    IL_001e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0023:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0028:  ldloc.0
    IL_0029:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_002e:  ldtoken    field int32 ExpressionTrees/'<>c__DisplayClass4'::x
    IL_0033:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0038:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_003d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0042:  ldc.i4.2
    IL_0043:  box        [mscorlib]System.Int32
    IL_0048:  ldtoken    [mscorlib]System.Int32
    IL_004d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0052:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0057:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_005c:  ldc.i4.0
    IL_005d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0062:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0067:  call       object ExpressionTrees::ToCode<int32>(object,
                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_006c:  pop
    IL_006d:  nop
    IL_006e:  ret
  } // end of method ExpressionTrees::AddOperator

  .method public hidebysig instance void 
          AnonymousClasses() cil managed
  {
    // Code size       158 (0x9e)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [mscorlib]System.Reflection.MethodInfo[] V_1)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void class '<>f__AnonymousType0`2'<int32,string>::.ctor(!0,
                                                                                                 !1)
    IL_000b:  ldtoken    class '<>f__AnonymousType0`2'<int32,string>
    IL_0010:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_001a:  ldc.i4.2
    IL_001b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0020:  stloc.0
    IL_0021:  ldloc.0
    IL_0022:  ldc.i4.0
    IL_0023:  ldc.i4.3
    IL_0024:  box        [mscorlib]System.Int32
    IL_0029:  ldtoken    [mscorlib]System.Int32
    IL_002e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0033:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0038:  stelem.ref
    IL_0039:  ldloc.0
    IL_003a:  ldc.i4.1
    IL_003b:  ldstr      "a"
    IL_0040:  ldtoken    [mscorlib]System.String
    IL_0045:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004f:  stelem.ref
    IL_0050:  ldloc.0
    IL_0051:  ldc.i4.2
    IL_0052:  newarr     [mscorlib]System.Reflection.MethodInfo
    IL_0057:  stloc.1
    IL_0058:  ldloc.1
    IL_0059:  ldc.i4.0
    IL_005a:  ldtoken    method instance !0 class '<>f__AnonymousType0`2'<int32,string>::get_X()
    IL_005f:  ldtoken    class '<>f__AnonymousType0`2'<int32,string>
    IL_0064:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0069:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_006e:  stelem.ref
    IL_006f:  ldloc.1
    IL_0070:  ldc.i4.1
    IL_0071:  ldtoken    method instance !1 class '<>f__AnonymousType0`2'<int32,string>::get_A()
    IL_0076:  ldtoken    class '<>f__AnonymousType0`2'<int32,string>
    IL_007b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0080:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0085:  stelem.ref
    IL_0086:  ldloc.1
    IL_0087:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>,
                                                                                                                                       class [mscorlib]System.Reflection.MemberInfo[])
    IL_008c:  ldc.i4.0
    IL_008d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0092:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class '<>f__AnonymousType0`2'<int32,string>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0097:  call       object ExpressionTrees::ToCode<class '<>f__AnonymousType0`2'<int32,string>>(object,
                                                                                                     class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_009c:  pop
    IL_009d:  ret
  } // end of method ExpressionTrees::AnonymousClasses

  .method public hidebysig instance void 
          ArrayIndex() cil managed
  {
    // Code size       233 (0xe9)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Int32
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldc.i4.3
    IL_0011:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4.0
    IL_0019:  ldc.i4.3
    IL_001a:  box        [mscorlib]System.Int32
    IL_001f:  ldtoken    [mscorlib]System.Int32
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002e:  stelem.ref
    IL_002f:  ldloc.0
    IL_0030:  ldc.i4.1
    IL_0031:  ldc.i4.4
    IL_0032:  box        [mscorlib]System.Int32
    IL_0037:  ldtoken    [mscorlib]System.Int32
    IL_003c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0041:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0046:  stelem.ref
    IL_0047:  ldloc.0
    IL_0048:  ldc.i4.2
    IL_0049:  ldc.i4.5
    IL_004a:  box        [mscorlib]System.Int32
    IL_004f:  ldtoken    [mscorlib]System.Int32
    IL_0054:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0059:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005e:  stelem.ref
    IL_005f:  ldloc.0
    IL_0060:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0065:  ldc.i4.0
    IL_0066:  box        [mscorlib]System.Int32
    IL_006b:  ldtoken    [mscorlib]System.Int32
    IL_0070:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0075:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_007a:  ldnull
    IL_007b:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_0080:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0085:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_008a:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_008f:  ldtoken    method instance int64 [mscorlib]System.DateTime::get_Ticks()
    IL_0094:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0099:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_009e:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00a3:  ldc.i4.3
    IL_00a4:  conv.i8
    IL_00a5:  box        [mscorlib]System.Int64
    IL_00aa:  ldtoken    [mscorlib]System.Int64
    IL_00af:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b9:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Modulo(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_00be:  ldtoken    [mscorlib]System.Int32
    IL_00c3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c8:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00cd:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_00d2:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayIndex(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression)
    IL_00d7:  ldc.i4.0
    IL_00d8:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00dd:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00e2:  call       object ExpressionTrees::ToCode<int32>(object,
                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00e7:  pop
    IL_00e8:  ret
  } // end of method ExpressionTrees::ArrayIndex

  .method public hidebysig instance void 
          ArrayLengthAndDoubles() cil managed
  {
    // Code size       302 (0x12e)
    .maxstack  14
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method !!0[] [System.Core]System.Linq.Enumerable::ToArray<float64>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.1
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.0
    IL_001f:  ldnull
    IL_0020:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0> [System.Core]System.Linq.Enumerable::Concat<float64>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_0025:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002f:  ldc.i4.2
    IL_0030:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0035:  stloc.1
    IL_0036:  ldloc.1
    IL_0037:  ldc.i4.0
    IL_0038:  ldtoken    [mscorlib]System.Double
    IL_003d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0042:  ldc.i4.3
    IL_0043:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0048:  stloc.2
    IL_0049:  ldloc.2
    IL_004a:  ldc.i4.0
    IL_004b:  ldc.r8     1.
    IL_0054:  box        [mscorlib]System.Double
    IL_0059:  ldtoken    [mscorlib]System.Double
    IL_005e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0063:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0068:  stelem.ref
    IL_0069:  ldloc.2
    IL_006a:  ldc.i4.1
    IL_006b:  ldc.r8     2.0099999999999998
    IL_0074:  box        [mscorlib]System.Double
    IL_0079:  ldtoken    [mscorlib]System.Double
    IL_007e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0083:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0088:  stelem.ref
    IL_0089:  ldloc.2
    IL_008a:  ldc.i4.2
    IL_008b:  ldc.r8     3.5
    IL_0094:  box        [mscorlib]System.Double
    IL_0099:  ldtoken    [mscorlib]System.Double
    IL_009e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a8:  stelem.ref
    IL_00a9:  ldloc.2
    IL_00aa:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00af:  stelem.ref
    IL_00b0:  ldloc.1
    IL_00b1:  ldc.i4.1
    IL_00b2:  ldtoken    [mscorlib]System.Double
    IL_00b7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00bc:  ldc.i4.2
    IL_00bd:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00c2:  stloc.2
    IL_00c3:  ldloc.2
    IL_00c4:  ldc.i4.0
    IL_00c5:  ldc.r8     1.
    IL_00ce:  box        [mscorlib]System.Double
    IL_00d3:  ldtoken    [mscorlib]System.Double
    IL_00d8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00dd:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e2:  stelem.ref
    IL_00e3:  ldloc.2
    IL_00e4:  ldc.i4.1
    IL_00e5:  ldc.r8     2.
    IL_00ee:  box        [mscorlib]System.Double
    IL_00f3:  ldtoken    [mscorlib]System.Double
    IL_00f8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00fd:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0102:  stelem.ref
    IL_0103:  ldloc.2
    IL_0104:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0109:  stelem.ref
    IL_010a:  ldloc.1
    IL_010b:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0110:  stelem.ref
    IL_0111:  ldloc.0
    IL_0112:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0117:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::ArrayLength(class [System.Core]System.Linq.Expressions.Expression)
    IL_011c:  ldc.i4.0
    IL_011d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0122:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0127:  call       object ExpressionTrees::ToCode<int32>(object,
                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_012c:  pop
    IL_012d:  ret
  } // end of method ExpressionTrees::ArrayLengthAndDoubles

  .method public hidebysig instance void 
          AsOperator() cil managed
  {
    // Code size       65 (0x41)
    .maxstack  3
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0015:  ldc.i4.0
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0020:  ldtoken    [mscorlib]System.String
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::TypeAs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Type)
    IL_002f:  ldc.i4.0
    IL_0030:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0035:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_003a:  call       object ExpressionTrees::ToCode<string>(object,
                                                                class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_003f:  pop
    IL_0040:  ret
  } // end of method ExpressionTrees::AsOperator

  .method public hidebysig instance void 
          ComplexGenericName() cil managed
  {
    // Code size       141 (0x8d)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Int32
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldstr      "x"
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_001a:  stloc.0
    IL_001b:  ldloc.0
    IL_001c:  ldc.i4.0
    IL_001d:  box        [mscorlib]System.Int32
    IL_0022:  ldtoken    [mscorlib]System.Int32
    IL_0027:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0031:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0036:  ldc.i4.1
    IL_0037:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_003c:  stloc.1
    IL_003d:  ldloc.1
    IL_003e:  ldc.i4.0
    IL_003f:  ldloc.0
    IL_0040:  stelem.ref
    IL_0041:  ldloc.1
    IL_0042:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0047:  ldtoken    class [mscorlib]System.Func`2<int32,bool>
    IL_004c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0051:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0056:  ldc.i4.1
    IL_0057:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_005c:  stloc.2
    IL_005d:  ldloc.2
    IL_005e:  ldc.i4.0
    IL_005f:  ldc.i4.0
    IL_0060:  box        [mscorlib]System.Int32
    IL_0065:  ldtoken    [mscorlib]System.Int32
    IL_006a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0074:  stelem.ref
    IL_0075:  ldloc.2
    IL_0076:  call       class [System.Core]System.Linq.Expressions.InvocationExpression [System.Core]System.Linq.Expressions.Expression::Invoke(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression[])
    IL_007b:  ldc.i4.0
    IL_007c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0081:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0086:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008b:  pop
    IL_008c:  ret
  } // end of method ExpressionTrees::ComplexGenericName

  .method public hidebysig instance void 
          DefaultValue() cil managed
  {
    // Code size       174 (0xae)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             valuetype [mscorlib]System.TimeSpan V_1)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void [mscorlib]System.TimeSpan::.ctor(int32,
                                                                               int32,
                                                                               int32)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0015:  ldc.i4.3
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.0
    IL_001e:  ldc.i4.1
    IL_001f:  box        [mscorlib]System.Int32
    IL_0024:  ldtoken    [mscorlib]System.Int32
    IL_0029:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0033:  stelem.ref
    IL_0034:  ldloc.0
    IL_0035:  ldc.i4.1
    IL_0036:  ldc.i4.2
    IL_0037:  box        [mscorlib]System.Int32
    IL_003c:  ldtoken    [mscorlib]System.Int32
    IL_0041:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0046:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004b:  stelem.ref
    IL_004c:  ldloc.0
    IL_004d:  ldc.i4.2
    IL_004e:  ldc.i4.3
    IL_004f:  box        [mscorlib]System.Int32
    IL_0054:  ldtoken    [mscorlib]System.Int32
    IL_0059:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0063:  stelem.ref
    IL_0064:  ldloc.0
    IL_0065:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_006a:  ldloca.s   V_1
    IL_006c:  initobj    [mscorlib]System.TimeSpan
    IL_0072:  ldloc.1
    IL_0073:  box        [mscorlib]System.TimeSpan
    IL_0078:  ldtoken    [mscorlib]System.TimeSpan
    IL_007d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0082:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0087:  ldc.i4.0
    IL_0088:  ldtoken    method bool [mscorlib]System.TimeSpan::op_Equality(valuetype [mscorlib]System.TimeSpan,
                                                                            valuetype [mscorlib]System.TimeSpan)
    IL_008d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0092:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0097:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            bool,
                                                                                                                                            class [mscorlib]System.Reflection.MethodInfo)
    IL_009c:  ldc.i4.0
    IL_009d:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a2:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00a7:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00ac:  pop
    IL_00ad:  ret
  } // end of method ExpressionTrees::DefaultValue

  .method public hidebysig instance void 
          EnumConstant() cil managed
  {
    // Code size       117 (0x75)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0015:  ldc.i4.0
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0020:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_0025:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002f:  ldc.i4.1
    IL_0030:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0035:  stloc.0
    IL_0036:  ldloc.0
    IL_0037:  ldc.i4.0
    IL_0038:  ldc.i4.0
    IL_0039:  box        [mscorlib]System.MidpointRounding
    IL_003e:  ldtoken    [mscorlib]System.MidpointRounding
    IL_0043:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0048:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004d:  ldtoken    [mscorlib]System.Object
    IL_0052:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0057:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_005c:  stelem.ref
    IL_005d:  ldloc.0
    IL_005e:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0063:  ldc.i4.0
    IL_0064:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0069:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006e:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0073:  pop
    IL_0074:  ret
  } // end of method ExpressionTrees::EnumConstant

  .method public hidebysig instance void 
          IndexerAccess() cil managed
  {
    // Code size       184 (0xb8)
    .maxstack  7
    .locals init (class ExpressionTrees/'<>c__DisplayClass8' V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass8'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  ldc.i4.s   20
    IL_000b:  call       class [mscorlib]System.Collections.Generic.IEnumerable`1<int32> [System.Core]System.Linq.Enumerable::Range(int32,
                                                                                                                                    int32)
    IL_0010:  ldsfld     class [mscorlib]System.Func`2<int32,string> ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7'
    IL_0015:  brtrue.s   IL_002a

    IL_0017:  ldnull
    IL_0018:  ldftn      string ExpressionTrees::'<IndexerAccess>b__6'(int32)
    IL_001e:  newobj     instance void class [mscorlib]System.Func`2<int32,string>::.ctor(object,
                                                                                          native int)
    IL_0023:  stsfld     class [mscorlib]System.Func`2<int32,string> ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7'
    IL_0028:  br.s       IL_002a

    IL_002a:  ldsfld     class [mscorlib]System.Func`2<int32,string> ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate7'
    IL_002f:  call       class [mscorlib]System.Collections.Generic.Dictionary`2<!!1,!!0> [System.Core]System.Linq.Enumerable::ToDictionary<int32,string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                          class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0034:  stfld      class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> ExpressionTrees/'<>c__DisplayClass8'::dict
    IL_0039:  call       object ExpressionTrees::X()
    IL_003e:  ldloc.0
    IL_003f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0044:  ldtoken    field class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32> ExpressionTrees/'<>c__DisplayClass8'::dict
    IL_0049:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_004e:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0053:  ldtoken    method instance !1 class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>::get_Item(!0)
    IL_0058:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<string,int32>
    IL_005d:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0062:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0067:  ldc.i4.1
    IL_0068:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_006d:  stloc.1
    IL_006e:  ldloc.1
    IL_006f:  ldc.i4.0
    IL_0070:  ldstr      "3"
    IL_0075:  ldtoken    [mscorlib]System.String
    IL_007a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_007f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0084:  stelem.ref
    IL_0085:  ldloc.1
    IL_0086:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_008b:  ldc.i4.3
    IL_008c:  box        [mscorlib]System.Int32
    IL_0091:  ldtoken    [mscorlib]System.Int32
    IL_0096:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a0:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00a5:  ldc.i4.0
    IL_00a6:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00ab:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b0:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00b5:  pop
    IL_00b6:  nop
    IL_00b7:  ret
  } // end of method ExpressionTrees::IndexerAccess

  .method public hidebysig instance void 
          IsOperator() cil managed
  {
    // Code size       65 (0x41)
    .maxstack  3
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0015:  ldc.i4.0
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0020:  ldtoken    [mscorlib]System.String
    IL_0025:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002a:  call       class [System.Core]System.Linq.Expressions.TypeBinaryExpression [System.Core]System.Linq.Expressions.Expression::TypeIs(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002f:  ldc.i4.0
    IL_0030:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0035:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_003a:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_003f:  pop
    IL_0040:  ret
  } // end of method ExpressionTrees::IsOperator

  .method public hidebysig instance void 
          ListInitializer() cil managed
  {
    // Code size       371 (0x173)
    .maxstack  9
    .locals init (class [System.Core]System.Linq.Expressions.ElementInit[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::.ctor()
    IL_000b:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_0010:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_001a:  ldc.i4.0
    IL_001b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0020:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0025:  ldc.i4.3
    IL_0026:  newarr     [System.Core]System.Linq.Expressions.ElementInit
    IL_002b:  stloc.0
    IL_002c:  ldloc.0
    IL_002d:  ldc.i4.0
    IL_002e:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::Add(!0,
                                                                                                                        !1)
    IL_0033:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_0038:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0042:  ldc.i4.2
    IL_0043:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0048:  stloc.1
    IL_0049:  ldloc.1
    IL_004a:  ldc.i4.0
    IL_004b:  ldc.i4.1
    IL_004c:  box        [mscorlib]System.Int32
    IL_0051:  ldtoken    [mscorlib]System.Int32
    IL_0056:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0060:  stelem.ref
    IL_0061:  ldloc.1
    IL_0062:  ldc.i4.1
    IL_0063:  ldc.i4.1
    IL_0064:  box        [mscorlib]System.Int32
    IL_0069:  ldtoken    [mscorlib]System.Int32
    IL_006e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0073:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0078:  stelem.ref
    IL_0079:  ldloc.1
    IL_007a:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_007f:  stelem.ref
    IL_0080:  ldloc.0
    IL_0081:  ldc.i4.1
    IL_0082:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::Add(!0,
                                                                                                                        !1)
    IL_0087:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_008c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0091:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0096:  ldc.i4.2
    IL_0097:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_009c:  stloc.1
    IL_009d:  ldloc.1
    IL_009e:  ldc.i4.0
    IL_009f:  ldc.i4.2
    IL_00a0:  box        [mscorlib]System.Int32
    IL_00a5:  ldtoken    [mscorlib]System.Int32
    IL_00aa:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00af:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b4:  stelem.ref
    IL_00b5:  ldloc.1
    IL_00b6:  ldc.i4.1
    IL_00b7:  ldc.i4.2
    IL_00b8:  box        [mscorlib]System.Int32
    IL_00bd:  ldtoken    [mscorlib]System.Int32
    IL_00c2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c7:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cc:  stelem.ref
    IL_00cd:  ldloc.1
    IL_00ce:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00d3:  stelem.ref
    IL_00d4:  ldloc.0
    IL_00d5:  ldc.i4.2
    IL_00d6:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::Add(!0,
                                                                                                                        !1)
    IL_00db:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_00e0:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e5:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00ea:  ldc.i4.2
    IL_00eb:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00f0:  stloc.1
    IL_00f1:  ldloc.1
    IL_00f2:  ldc.i4.0
    IL_00f3:  ldc.i4.3
    IL_00f4:  box        [mscorlib]System.Int32
    IL_00f9:  ldtoken    [mscorlib]System.Int32
    IL_00fe:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0103:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0108:  stelem.ref
    IL_0109:  ldloc.1
    IL_010a:  ldc.i4.1
    IL_010b:  ldc.i4.4
    IL_010c:  box        [mscorlib]System.Int32
    IL_0111:  ldtoken    [mscorlib]System.Int32
    IL_0116:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_011b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0120:  stelem.ref
    IL_0121:  ldloc.1
    IL_0122:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_0127:  stelem.ref
    IL_0128:  ldloc.0
    IL_0129:  call       class [System.Core]System.Linq.Expressions.ListInitExpression [System.Core]System.Linq.Expressions.Expression::ListInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.ElementInit[])
    IL_012e:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>::get_Count()
    IL_0133:  ldtoken    class [mscorlib]System.Collections.Generic.Dictionary`2<int32,int32>
    IL_0138:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_013d:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0142:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0147:  ldc.i4.3
    IL_0148:  box        [mscorlib]System.Int32
    IL_014d:  ldtoken    [mscorlib]System.Int32
    IL_0152:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0157:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_015c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0161:  ldc.i4.0
    IL_0162:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0167:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_016c:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0171:  pop
    IL_0172:  ret
  } // end of method ExpressionTrees::ListInitializer

  .method public hidebysig instance void 
          ListInitializer2() cil managed
  {
    // Code size       326 (0x146)
    .maxstack  9
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.ElementInit[] V_1)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor(int32)
    IL_000b:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0010:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_001a:  ldc.i4.1
    IL_001b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0020:  stloc.0
    IL_0021:  ldloc.0
    IL_0022:  ldc.i4.0
    IL_0023:  ldc.i4.s   50
    IL_0025:  box        [mscorlib]System.Int32
    IL_002a:  ldtoken    [mscorlib]System.Int32
    IL_002f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0034:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0039:  stelem.ref
    IL_003a:  ldloc.0
    IL_003b:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0040:  ldc.i4.3
    IL_0041:  newarr     [System.Core]System.Linq.Expressions.ElementInit
    IL_0046:  stloc.1
    IL_0047:  ldloc.1
    IL_0048:  ldc.i4.0
    IL_0049:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_004e:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0053:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0058:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005d:  ldc.i4.1
    IL_005e:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0063:  stloc.0
    IL_0064:  ldloc.0
    IL_0065:  ldc.i4.0
    IL_0066:  ldc.i4.1
    IL_0067:  box        [mscorlib]System.Int32
    IL_006c:  ldtoken    [mscorlib]System.Int32
    IL_0071:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0076:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_007b:  stelem.ref
    IL_007c:  ldloc.0
    IL_007d:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_0082:  stelem.ref
    IL_0083:  ldloc.1
    IL_0084:  ldc.i4.1
    IL_0085:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_008a:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_008f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0094:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0099:  ldc.i4.1
    IL_009a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_009f:  stloc.0
    IL_00a0:  ldloc.0
    IL_00a1:  ldc.i4.0
    IL_00a2:  ldc.i4.2
    IL_00a3:  box        [mscorlib]System.Int32
    IL_00a8:  ldtoken    [mscorlib]System.Int32
    IL_00ad:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b7:  stelem.ref
    IL_00b8:  ldloc.0
    IL_00b9:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00be:  stelem.ref
    IL_00bf:  ldloc.1
    IL_00c0:  ldc.i4.2
    IL_00c1:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_00c6:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00cb:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d0:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00d5:  ldc.i4.1
    IL_00d6:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00db:  stloc.0
    IL_00dc:  ldloc.0
    IL_00dd:  ldc.i4.0
    IL_00de:  ldc.i4.3
    IL_00df:  box        [mscorlib]System.Int32
    IL_00e4:  ldtoken    [mscorlib]System.Int32
    IL_00e9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ee:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00f3:  stelem.ref
    IL_00f4:  ldloc.0
    IL_00f5:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00fa:  stelem.ref
    IL_00fb:  ldloc.1
    IL_00fc:  call       class [System.Core]System.Linq.Expressions.ListInitExpression [System.Core]System.Linq.Expressions.Expression::ListInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.ElementInit[])
    IL_0101:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_0106:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_010b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0110:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0115:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_011a:  ldc.i4.3
    IL_011b:  box        [mscorlib]System.Int32
    IL_0120:  ldtoken    [mscorlib]System.Int32
    IL_0125:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_012a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_012f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0134:  ldc.i4.0
    IL_0135:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_013a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_013f:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0144:  pop
    IL_0145:  ret
  } // end of method ExpressionTrees::ListInitializer2

  .method public hidebysig instance void 
          ListInitializer3() cil managed
  {
    // Code size       299 (0x12b)
    .maxstack  9
    .locals init (class [System.Core]System.Linq.Expressions.ElementInit[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::.ctor()
    IL_000b:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0010:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_001a:  ldc.i4.0
    IL_001b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0020:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0025:  ldc.i4.3
    IL_0026:  newarr     [System.Core]System.Linq.Expressions.ElementInit
    IL_002b:  stloc.0
    IL_002c:  ldloc.0
    IL_002d:  ldc.i4.0
    IL_002e:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_0033:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0038:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0042:  ldc.i4.1
    IL_0043:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0048:  stloc.1
    IL_0049:  ldloc.1
    IL_004a:  ldc.i4.0
    IL_004b:  ldc.i4.1
    IL_004c:  box        [mscorlib]System.Int32
    IL_0051:  ldtoken    [mscorlib]System.Int32
    IL_0056:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0060:  stelem.ref
    IL_0061:  ldloc.1
    IL_0062:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_0067:  stelem.ref
    IL_0068:  ldloc.0
    IL_0069:  ldc.i4.1
    IL_006a:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_006f:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0074:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0079:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007e:  ldc.i4.1
    IL_007f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0084:  stloc.1
    IL_0085:  ldloc.1
    IL_0086:  ldc.i4.0
    IL_0087:  ldc.i4.2
    IL_0088:  box        [mscorlib]System.Int32
    IL_008d:  ldtoken    [mscorlib]System.Int32
    IL_0092:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0097:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009c:  stelem.ref
    IL_009d:  ldloc.1
    IL_009e:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00a3:  stelem.ref
    IL_00a4:  ldloc.0
    IL_00a5:  ldc.i4.2
    IL_00a6:  ldtoken    method instance void class [mscorlib]System.Collections.Generic.List`1<int32>::Add(!0)
    IL_00ab:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00b0:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b5:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00ba:  ldc.i4.1
    IL_00bb:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00c0:  stloc.1
    IL_00c1:  ldloc.1
    IL_00c2:  ldc.i4.0
    IL_00c3:  ldc.i4.3
    IL_00c4:  box        [mscorlib]System.Int32
    IL_00c9:  ldtoken    [mscorlib]System.Int32
    IL_00ce:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d8:  stelem.ref
    IL_00d9:  ldloc.1
    IL_00da:  call       class [System.Core]System.Linq.Expressions.ElementInit [System.Core]System.Linq.Expressions.Expression::ElementInit(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression[])
    IL_00df:  stelem.ref
    IL_00e0:  ldloc.0
    IL_00e1:  call       class [System.Core]System.Linq.Expressions.ListInitExpression [System.Core]System.Linq.Expressions.Expression::ListInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.ElementInit[])
    IL_00e6:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_00eb:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_00f0:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f5:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00fa:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00ff:  ldc.i4.3
    IL_0100:  box        [mscorlib]System.Int32
    IL_0105:  ldtoken    [mscorlib]System.Int32
    IL_010a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0114:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0119:  ldc.i4.0
    IL_011a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_011f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0124:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0129:  pop
    IL_012a:  ret
  } // end of method ExpressionTrees::ListInitializer3

  .method public hidebysig instance void 
          LiteralCharAndProperty() cil managed
  {
    // Code size       147 (0x93)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void [mscorlib]System.String::.ctor(char,
                                                                             int32)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0015:  ldc.i4.2
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  stloc.0
    IL_001c:  ldloc.0
    IL_001d:  ldc.i4.0
    IL_001e:  ldc.i4.s   32
    IL_0020:  box        [mscorlib]System.Char
    IL_0025:  ldtoken    [mscorlib]System.Char
    IL_002a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0034:  stelem.ref
    IL_0035:  ldloc.0
    IL_0036:  ldc.i4.1
    IL_0037:  ldc.i4.3
    IL_0038:  box        [mscorlib]System.Int32
    IL_003d:  ldtoken    [mscorlib]System.Int32
    IL_0042:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0047:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004c:  stelem.ref
    IL_004d:  ldloc.0
    IL_004e:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0053:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_0058:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_005d:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0062:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0067:  ldc.i4.1
    IL_0068:  box        [mscorlib]System.Int32
    IL_006d:  ldtoken    [mscorlib]System.Int32
    IL_0072:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0077:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_007c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0081:  ldc.i4.0
    IL_0082:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0087:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_008c:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0091:  pop
    IL_0092:  ret
  } // end of method ExpressionTrees::LiteralCharAndProperty

  .method public hidebysig instance void 
          CharNoCast() cil managed
  {
    // Code size       138 (0x8a)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldstr      "abc"
    IL_000b:  ldtoken    [mscorlib]System.String
    IL_0010:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001a:  ldtoken    method instance char [mscorlib]System.String::get_Chars(int32)
    IL_001f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0024:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0029:  ldc.i4.1
    IL_002a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002f:  stloc.0
    IL_0030:  ldloc.0
    IL_0031:  ldc.i4.0
    IL_0032:  ldc.i4.1
    IL_0033:  box        [mscorlib]System.Int32
    IL_0038:  ldtoken    [mscorlib]System.Int32
    IL_003d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0042:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0047:  stelem.ref
    IL_0048:  ldloc.0
    IL_0049:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_004e:  ldtoken    [mscorlib]System.Int32
    IL_0053:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0058:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_005d:  ldc.i4.s   98
    IL_005f:  box        [mscorlib]System.Int32
    IL_0064:  ldtoken    [mscorlib]System.Int32
    IL_0069:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0073:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0078:  ldc.i4.0
    IL_0079:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0083:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0088:  pop
    IL_0089:  ret
  } // end of method ExpressionTrees::CharNoCast

  .method public hidebysig instance void 
          StringsImplicitCast() cil managed
  {
    // Code size       378 (0x17a)
    .maxstack  8
    .locals init (class ExpressionTrees/'<>c__DisplayClassa' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClassa'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      int32 ExpressionTrees/'<>c__DisplayClassa'::i
    IL_000e:  ldloc.0
    IL_000f:  ldstr      "X"
    IL_0014:  stfld      string ExpressionTrees/'<>c__DisplayClassa'::x
    IL_0019:  call       object ExpressionTrees::X()
    IL_001e:  ldstr      "a\n\\b"
    IL_0023:  ldtoken    [mscorlib]System.String
    IL_0028:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0032:  ldloc.0
    IL_0033:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0038:  ldtoken    field string ExpressionTrees/'<>c__DisplayClassa'::x
    IL_003d:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0042:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0047:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Coalesce(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_004c:  ldloc.0
    IL_004d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0052:  ldtoken    field string ExpressionTrees/'<>c__DisplayClassa'::x
    IL_0057:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_005c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0061:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_0066:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_006b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0070:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_0075:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_007a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_007f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0084:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0089:  ldc.i4.2
    IL_008a:  box        [mscorlib]System.Int32
    IL_008f:  ldtoken    [mscorlib]System.Int32
    IL_0094:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0099:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00a3:  ldc.i4.0
    IL_00a4:  box        [mscorlib]System.Boolean
    IL_00a9:  ldtoken    [mscorlib]System.Boolean
    IL_00ae:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b8:  ldc.i4.1
    IL_00b9:  box        [mscorlib]System.Boolean
    IL_00be:  ldtoken    [mscorlib]System.Boolean
    IL_00c3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c8:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cd:  ldc.i4.1
    IL_00ce:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_00d3:  box        [mscorlib]System.Decimal
    IL_00d8:  ldtoken    [mscorlib]System.Decimal
    IL_00dd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e7:  ldloc.0
    IL_00e8:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_00ed:  ldtoken    field int32 ExpressionTrees/'<>c__DisplayClassa'::i
    IL_00f2:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00f7:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00fc:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Negate(class [System.Core]System.Linq.Expressions.Expression)
    IL_0101:  ldtoken    [mscorlib]System.Decimal
    IL_0106:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010b:  ldtoken    method valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(int32)
    IL_0110:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0115:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_011a:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type,
                                                                                                                                             class [mscorlib]System.Reflection.MethodInfo)
    IL_011f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0124:  ldc.i4.0
    IL_0125:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_012a:  box        [mscorlib]System.Decimal
    IL_012f:  ldtoken    [mscorlib]System.Decimal
    IL_0134:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0139:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_013e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0143:  ldc.i4.0
    IL_0144:  box        [mscorlib]System.Boolean
    IL_0149:  ldtoken    [mscorlib]System.Boolean
    IL_014e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0153:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0158:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::OrElse(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_015d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_0162:  call       class [System.Core]System.Linq.Expressions.ConditionalExpression [System.Core]System.Linq.Expressions.Expression::Condition(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression)
    IL_0167:  ldc.i4.0
    IL_0168:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_016d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0172:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0177:  pop
    IL_0178:  nop
    IL_0179:  ret
  } // end of method ExpressionTrees::StringsImplicitCast

  .method public hidebysig instance void 
          NotImplicitCast() cil managed
  {
    // Code size       106 (0x6a)
    .maxstack  4
    .locals init (class ExpressionTrees/'<>c__DisplayClassc' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClassc'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.s   42
    IL_000a:  stfld      uint8 ExpressionTrees/'<>c__DisplayClassc'::z
    IL_000f:  call       object ExpressionTrees::X()
    IL_0014:  ldloc.0
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_001a:  ldtoken    field uint8 ExpressionTrees/'<>c__DisplayClassc'::z
    IL_001f:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0024:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0029:  ldtoken    [mscorlib]System.Int32
    IL_002e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0033:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0038:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_003d:  ldc.i4.0
    IL_003e:  box        [mscorlib]System.Int32
    IL_0043:  ldtoken    [mscorlib]System.Int32
    IL_0048:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0052:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0057:  ldc.i4.0
    IL_0058:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_005d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0062:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0067:  pop
    IL_0068:  nop
    IL_0069:  ret
  } // end of method ExpressionTrees::NotImplicitCast

  .method public hidebysig instance void 
          MembersBuiltin() cil managed
  {
    // Code size       406 (0x196)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldc.i4.s   123
    IL_0008:  ldc.i4.0
    IL_0009:  ldc.i4.0
    IL_000a:  ldc.i4.0
    IL_000b:  ldc.i4.2
    IL_000c:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32,
                                                                       int32,
                                                                       int32,
                                                                       bool,
                                                                       uint8)
    IL_0011:  box        [mscorlib]System.Decimal
    IL_0016:  ldtoken    [mscorlib]System.Decimal
    IL_001b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0020:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0025:  ldtoken    method instance string [mscorlib]System.Decimal::ToString()
    IL_002a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0034:  ldc.i4.0
    IL_0035:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_003a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_003f:  ldc.i4.0
    IL_0040:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0045:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_004a:  call       object ExpressionTrees::ToCode<string>(object,
                                                                class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_004f:  pop
    IL_0050:  call       object ExpressionTrees::X()
    IL_0055:  ldc.i4     0x7fff
    IL_005a:  box        [mscorlib]System.AttributeTargets
    IL_005f:  ldtoken    [mscorlib]System.AttributeTargets
    IL_0064:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0069:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006e:  ldtoken    method instance bool [mscorlib]System.Enum::HasFlag(class [mscorlib]System.Enum)
    IL_0073:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0078:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007d:  ldc.i4.1
    IL_007e:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0083:  stloc.0
    IL_0084:  ldloc.0
    IL_0085:  ldc.i4.0
    IL_0086:  ldc.i4.1
    IL_0087:  box        [mscorlib]System.AttributeTargets
    IL_008c:  ldtoken    [mscorlib]System.AttributeTargets
    IL_0091:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0096:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009b:  ldtoken    [mscorlib]System.Enum
    IL_00a0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a5:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00aa:  stelem.ref
    IL_00ab:  ldloc.0
    IL_00ac:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00b1:  ldc.i4.0
    IL_00b2:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00b7:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00bc:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00c1:  pop
    IL_00c2:  call       object ExpressionTrees::X()
    IL_00c7:  ldstr      "abc"
    IL_00cc:  ldtoken    [mscorlib]System.String
    IL_00d1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00db:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_00e0:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00e5:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00ea:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00ef:  ldc.i4.3
    IL_00f0:  box        [mscorlib]System.Int32
    IL_00f5:  ldtoken    [mscorlib]System.Int32
    IL_00fa:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ff:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0104:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0109:  ldc.i4.0
    IL_010a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_010f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0114:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0119:  pop
    IL_011a:  call       object ExpressionTrees::X()
    IL_011f:  ldc.i4.s   97
    IL_0121:  box        [mscorlib]System.Char
    IL_0126:  ldtoken    [mscorlib]System.Char
    IL_012b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0130:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0135:  ldtoken    method instance int32 [mscorlib]System.Char::CompareTo(char)
    IL_013a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_013f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0144:  ldc.i4.1
    IL_0145:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_014a:  stloc.0
    IL_014b:  ldloc.0
    IL_014c:  ldc.i4.0
    IL_014d:  ldc.i4.s   98
    IL_014f:  box        [mscorlib]System.Char
    IL_0154:  ldtoken    [mscorlib]System.Char
    IL_0159:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_015e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0163:  stelem.ref
    IL_0164:  ldloc.0
    IL_0165:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_016a:  ldc.i4.0
    IL_016b:  box        [mscorlib]System.Int32
    IL_0170:  ldtoken    [mscorlib]System.Int32
    IL_0175:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_017a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_017f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::LessThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0184:  ldc.i4.0
    IL_0185:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_018a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_018f:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0194:  pop
    IL_0195:  ret
  } // end of method ExpressionTrees::MembersBuiltin

  .method public hidebysig instance void 
          MembersDefault() cil managed
  {
    // Code size       589 (0x24d)
    .maxstack  7
    .locals init (valuetype [mscorlib]System.DateTime V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldloca.s   V_0
    IL_0008:  initobj    [mscorlib]System.DateTime
    IL_000e:  ldloc.0
    IL_000f:  box        [mscorlib]System.DateTime
    IL_0014:  ldtoken    [mscorlib]System.DateTime
    IL_0019:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0023:  ldtoken    method instance int64 [mscorlib]System.DateTime::get_Ticks()
    IL_0028:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002d:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0032:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0037:  ldc.i4.0
    IL_0038:  conv.i8
    IL_0039:  box        [mscorlib]System.Int64
    IL_003e:  ldtoken    [mscorlib]System.Int64
    IL_0043:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0048:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
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
    IL_0063:  call       object ExpressionTrees::X()
    IL_0068:  ldnull
    IL_0069:  box        [mscorlib]System.Array
    IL_006e:  ldtoken    [mscorlib]System.Array
    IL_0073:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0078:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_007d:  ldtoken    method instance int32 [mscorlib]System.Array::get_Length()
    IL_0082:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0087:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_008c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0091:  ldc.i4.0
    IL_0092:  box        [mscorlib]System.Int32
    IL_0097:  ldtoken    [mscorlib]System.Int32
    IL_009c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a6:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00ab:  ldc.i4.0
    IL_00ac:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00b1:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00b6:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00bb:  pop
    IL_00bc:  call       object ExpressionTrees::X()
    IL_00c1:  ldnull
    IL_00c2:  box        [mscorlib]System.Type
    IL_00c7:  ldtoken    [mscorlib]System.Type
    IL_00cc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d6:  ldtoken    method instance bool [mscorlib]System.Type::get_IsLayoutSequential()
    IL_00db:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00e0:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00e5:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00ea:  ldc.i4.0
    IL_00eb:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00f0:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00f5:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00fa:  pop
    IL_00fb:  call       object ExpressionTrees::X()
    IL_0100:  ldnull
    IL_0101:  box        class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0106:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_010b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0110:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0115:  ldtoken    method instance int32 class [mscorlib]System.Collections.Generic.List`1<int32>::get_Count()
    IL_011a:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_011f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0124:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0129:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_012e:  ldc.i4.0
    IL_012f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0134:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0139:  call       object ExpressionTrees::ToCode<int32>(object,
                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_013e:  pop
    IL_013f:  call       object ExpressionTrees::X()
    IL_0144:  ldnull
    IL_0145:  box        [mscorlib]System.Array
    IL_014a:  ldtoken    [mscorlib]System.Array
    IL_014f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0154:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0159:  ldtoken    method instance object [mscorlib]System.Array::Clone()
    IL_015e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0163:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0168:  ldc.i4.0
    IL_0169:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_016e:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0173:  ldnull
    IL_0174:  box        [mscorlib]System.Object
    IL_0179:  ldtoken    [mscorlib]System.Object
    IL_017e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0183:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0188:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_018d:  ldc.i4.0
    IL_018e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0193:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0198:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_019d:  pop
    IL_019e:  call       object ExpressionTrees::X()
    IL_01a3:  ldnull
    IL_01a4:  box        [mscorlib]System.Type
    IL_01a9:  ldtoken    [mscorlib]System.Type
    IL_01ae:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01b3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01b8:  ldtoken    method instance bool [mscorlib]System.Type::IsInstanceOfType(object)
    IL_01bd:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01c2:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01c7:  ldc.i4.1
    IL_01c8:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01cd:  stloc.1
    IL_01ce:  ldloc.1
    IL_01cf:  ldc.i4.0
    IL_01d0:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_01d5:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01da:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_01df:  ldc.i4.0
    IL_01e0:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01e5:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_01ea:  stelem.ref
    IL_01eb:  ldloc.1
    IL_01ec:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_01f1:  ldc.i4.0
    IL_01f2:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01f7:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01fc:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0201:  pop
    IL_0202:  call       object ExpressionTrees::X()
    IL_0207:  ldnull
    IL_0208:  box        class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_020d:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0212:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0217:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_021c:  ldtoken    method instance class [mscorlib]System.Collections.ObjectModel.ReadOnlyCollection`1<!0> class [mscorlib]System.Collections.Generic.List`1<int32>::AsReadOnly()
    IL_0221:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>
    IL_0226:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_022b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0230:  ldc.i4.0
    IL_0231:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0236:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_023b:  ldc.i4.0
    IL_023c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0241:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Collections.ObjectModel.ReadOnlyCollection`1<int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0246:  call       object ExpressionTrees::ToCode<class [mscorlib]System.Collections.ObjectModel.ReadOnlyCollection`1<int32>>(object,
                                                                                                                                    class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_024b:  pop
    IL_024c:  ret
  } // end of method ExpressionTrees::MembersDefault

  .method public hidebysig instance void 
          DoAssert() cil managed
  {
    // Code size       399 (0x18f)
    .maxstack  8
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldarg.0
    IL_0007:  box        ExpressionTrees
    IL_000c:  ldtoken    ExpressionTrees
    IL_0011:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0016:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001b:  ldtoken    field int32 ExpressionTrees::'field'
    IL_0020:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0025:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_002a:  ldarg.0
    IL_002b:  box        ExpressionTrees
    IL_0030:  ldtoken    ExpressionTrees
    IL_0035:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_003f:  ldtoken    method instance int32 ExpressionTrees::C()
    IL_0044:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0049:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_004e:  ldc.i4.0
    IL_004f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0054:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0059:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_005e:  ldc.i4.0
    IL_005f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0064:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0069:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_006e:  pop
    IL_006f:  call       object ExpressionTrees::X()
    IL_0074:  ldnull
    IL_0075:  ldtoken    method bool [mscorlib]System.Object::ReferenceEquals(object,
                                                                              object)
    IL_007a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_007f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0084:  ldc.i4.2
    IL_0085:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_008a:  stloc.0
    IL_008b:  ldloc.0
    IL_008c:  ldc.i4.0
    IL_008d:  ldarg.0
    IL_008e:  box        ExpressionTrees
    IL_0093:  ldtoken    ExpressionTrees
    IL_0098:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_009d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a2:  stelem.ref
    IL_00a3:  ldloc.0
    IL_00a4:  ldc.i4.1
    IL_00a5:  ldtoken    method instance void ExpressionTrees::.ctor()
    IL_00aa:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00af:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_00b4:  ldc.i4.0
    IL_00b5:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00ba:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_00bf:  stelem.ref
    IL_00c0:  ldloc.0
    IL_00c1:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00c6:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_00cb:  ldc.i4.0
    IL_00cc:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00d1:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00d6:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00db:  pop
    IL_00dc:  call       object ExpressionTrees::X()
    IL_00e1:  ldarg.0
    IL_00e2:  box        ExpressionTrees
    IL_00e7:  ldtoken    ExpressionTrees
    IL_00ec:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00f6:  ldtoken    method instance bool ExpressionTrees::MyEquals(class ExpressionTrees)
    IL_00fb:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0100:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0105:  ldc.i4.1
    IL_0106:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_010b:  stloc.0
    IL_010c:  ldloc.0
    IL_010d:  ldc.i4.0
    IL_010e:  ldarg.0
    IL_010f:  box        ExpressionTrees
    IL_0114:  ldtoken    ExpressionTrees
    IL_0119:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_011e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0123:  stelem.ref
    IL_0124:  ldloc.0
    IL_0125:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_012a:  ldarg.0
    IL_012b:  box        ExpressionTrees
    IL_0130:  ldtoken    ExpressionTrees
    IL_0135:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_013a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_013f:  ldtoken    method instance bool ExpressionTrees::MyEquals(class ExpressionTrees)
    IL_0144:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0149:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_014e:  ldc.i4.1
    IL_014f:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0154:  stloc.0
    IL_0155:  ldloc.0
    IL_0156:  ldc.i4.0
    IL_0157:  ldnull
    IL_0158:  box        ExpressionTrees
    IL_015d:  ldtoken    ExpressionTrees
    IL_0162:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0167:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_016c:  stelem.ref
    IL_016d:  ldloc.0
    IL_016e:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0173:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_0178:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_017d:  ldc.i4.0
    IL_017e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0183:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0188:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_018d:  pop
    IL_018e:  ret
  } // end of method ExpressionTrees::DoAssert

  .method private hidebysig instance int32 
          C() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0006:  throw
  } // end of method ExpressionTrees::C

  .method private hidebysig instance bool 
          MyEquals(class ExpressionTrees other) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
    IL_0006:  throw
  } // end of method ExpressionTrees::MyEquals

  .method public hidebysig instance void 
          MethodGroupAsExtensionMethod() cil managed
  {
    // Code size       288 (0x120)
    .maxstack  10
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldtoken    method bool [System.Core]System.Linq.Enumerable::Any<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0015:  box        [mscorlib]System.Reflection.MethodInfo
    IL_001a:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_001f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0024:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0029:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_002e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0033:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0038:  ldc.i4.2
    IL_0039:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_003e:  stloc.0
    IL_003f:  ldloc.0
    IL_0040:  ldc.i4.0
    IL_0041:  ldtoken    class [mscorlib]System.Func`1<bool>
    IL_0046:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004b:  box        [mscorlib]System.Type
    IL_0050:  ldtoken    [mscorlib]System.Type
    IL_0055:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005f:  stelem.ref
    IL_0060:  ldloc.0
    IL_0061:  ldc.i4.1
    IL_0062:  ldtoken    [mscorlib]System.Int32
    IL_0067:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006c:  ldc.i4.4
    IL_006d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0072:  stloc.1
    IL_0073:  ldloc.1
    IL_0074:  ldc.i4.0
    IL_0075:  ldc.i4     0x7d0
    IL_007a:  box        [mscorlib]System.Int32
    IL_007f:  ldtoken    [mscorlib]System.Int32
    IL_0084:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0089:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_008e:  stelem.ref
    IL_008f:  ldloc.1
    IL_0090:  ldc.i4.1
    IL_0091:  ldc.i4     0x7d4
    IL_0096:  box        [mscorlib]System.Int32
    IL_009b:  ldtoken    [mscorlib]System.Int32
    IL_00a0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00aa:  stelem.ref
    IL_00ab:  ldloc.1
    IL_00ac:  ldc.i4.2
    IL_00ad:  ldc.i4     0x7d8
    IL_00b2:  box        [mscorlib]System.Int32
    IL_00b7:  ldtoken    [mscorlib]System.Int32
    IL_00bc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00c6:  stelem.ref
    IL_00c7:  ldloc.1
    IL_00c8:  ldc.i4.3
    IL_00c9:  ldc.i4     0x7dc
    IL_00ce:  box        [mscorlib]System.Int32
    IL_00d3:  ldtoken    [mscorlib]System.Int32
    IL_00d8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00dd:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e2:  stelem.ref
    IL_00e3:  ldloc.1
    IL_00e4:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00e9:  ldtoken    class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>
    IL_00ee:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f3:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_00f8:  stelem.ref
    IL_00f9:  ldloc.0
    IL_00fa:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00ff:  ldtoken    class [mscorlib]System.Func`1<bool>
    IL_0104:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0109:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_010e:  ldc.i4.0
    IL_010f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0114:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Func`1<bool>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0119:  call       object ExpressionTrees::ToCode<class [mscorlib]System.Func`1<bool>>(object,
                                                                                             class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_011e:  pop
    IL_011f:  ret
  } // end of method ExpressionTrees::MethodGroupAsExtensionMethod

  .method public hidebysig instance void 
          MethodGroupConstant() cil managed
  {
    // Code size       900 (0x384)
    .maxstack  11
    .locals init (class ExpressionTrees/'<>c__DisplayClass10' V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass10'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  call       object ExpressionTrees::X()
    IL_000c:  ldnull
    IL_000d:  ldtoken    method bool [mscorlib]System.Array::TrueForAll<int32>(!!0[],
                                                                               class [mscorlib]System.Predicate`1<!!0>)
    IL_0012:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0017:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_001c:  ldc.i4.2
    IL_001d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0022:  stloc.1
    IL_0023:  ldloc.1
    IL_0024:  ldc.i4.0
    IL_0025:  ldtoken    [mscorlib]System.Int32
    IL_002a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002f:  ldc.i4.4
    IL_0030:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0035:  stloc.2
    IL_0036:  ldloc.2
    IL_0037:  ldc.i4.0
    IL_0038:  ldc.i4     0x7d0
    IL_003d:  box        [mscorlib]System.Int32
    IL_0042:  ldtoken    [mscorlib]System.Int32
    IL_0047:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0051:  stelem.ref
    IL_0052:  ldloc.2
    IL_0053:  ldc.i4.1
    IL_0054:  ldc.i4     0x7d4
    IL_0059:  box        [mscorlib]System.Int32
    IL_005e:  ldtoken    [mscorlib]System.Int32
    IL_0063:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0068:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006d:  stelem.ref
    IL_006e:  ldloc.2
    IL_006f:  ldc.i4.2
    IL_0070:  ldc.i4     0x7d8
    IL_0075:  box        [mscorlib]System.Int32
    IL_007a:  ldtoken    [mscorlib]System.Int32
    IL_007f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0084:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0089:  stelem.ref
    IL_008a:  ldloc.2
    IL_008b:  ldc.i4.3
    IL_008c:  ldc.i4     0x7dc
    IL_0091:  box        [mscorlib]System.Int32
    IL_0096:  ldtoken    [mscorlib]System.Int32
    IL_009b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a0:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00a5:  stelem.ref
    IL_00a6:  ldloc.2
    IL_00a7:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00ac:  stelem.ref
    IL_00ad:  ldloc.1
    IL_00ae:  ldc.i4.1
    IL_00af:  ldtoken    method bool [mscorlib]System.DateTime::IsLeapYear(int32)
    IL_00b4:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00b9:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00be:  box        [mscorlib]System.Reflection.MethodInfo
    IL_00c3:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_00c8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00cd:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00d2:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_00d7:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00dc:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00e1:  ldc.i4.2
    IL_00e2:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00e7:  stloc.2
    IL_00e8:  ldloc.2
    IL_00e9:  ldc.i4.0
    IL_00ea:  ldtoken    class [mscorlib]System.Predicate`1<int32>
    IL_00ef:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f4:  box        [mscorlib]System.Type
    IL_00f9:  ldtoken    [mscorlib]System.Type
    IL_00fe:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0103:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0108:  stelem.ref
    IL_0109:  ldloc.2
    IL_010a:  ldc.i4.1
    IL_010b:  ldnull
    IL_010c:  ldtoken    [mscorlib]System.Object
    IL_0111:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0116:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_011b:  stelem.ref
    IL_011c:  ldloc.2
    IL_011d:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0122:  ldtoken    class [mscorlib]System.Predicate`1<int32>
    IL_0127:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_012c:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0131:  stelem.ref
    IL_0132:  ldloc.1
    IL_0133:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0138:  ldc.i4.0
    IL_0139:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_013e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0143:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0148:  pop
    IL_0149:  ldloc.0
    IL_014a:  newobj     instance void class [System.Core]System.Collections.Generic.HashSet`1<int32>::.ctor()
    IL_014f:  stfld      class [System.Core]System.Collections.Generic.HashSet`1<int32> ExpressionTrees/'<>c__DisplayClass10'::set
    IL_0154:  call       object ExpressionTrees::X()
    IL_0159:  ldnull
    IL_015a:  ldtoken    method bool [System.Core]System.Linq.Enumerable::All<int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                     class [mscorlib]System.Func`2<!!0,bool>)
    IL_015f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0164:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0169:  ldc.i4.2
    IL_016a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_016f:  stloc.1
    IL_0170:  ldloc.1
    IL_0171:  ldc.i4.0
    IL_0172:  ldtoken    [mscorlib]System.Int32
    IL_0177:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_017c:  ldc.i4.4
    IL_017d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0182:  stloc.2
    IL_0183:  ldloc.2
    IL_0184:  ldc.i4.0
    IL_0185:  ldc.i4     0x7d0
    IL_018a:  box        [mscorlib]System.Int32
    IL_018f:  ldtoken    [mscorlib]System.Int32
    IL_0194:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0199:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_019e:  stelem.ref
    IL_019f:  ldloc.2
    IL_01a0:  ldc.i4.1
    IL_01a1:  ldc.i4     0x7d4
    IL_01a6:  box        [mscorlib]System.Int32
    IL_01ab:  ldtoken    [mscorlib]System.Int32
    IL_01b0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01b5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01ba:  stelem.ref
    IL_01bb:  ldloc.2
    IL_01bc:  ldc.i4.2
    IL_01bd:  ldc.i4     0x7d8
    IL_01c2:  box        [mscorlib]System.Int32
    IL_01c7:  ldtoken    [mscorlib]System.Int32
    IL_01cc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01d1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01d6:  stelem.ref
    IL_01d7:  ldloc.2
    IL_01d8:  ldc.i4.3
    IL_01d9:  ldc.i4     0x7dc
    IL_01de:  box        [mscorlib]System.Int32
    IL_01e3:  ldtoken    [mscorlib]System.Int32
    IL_01e8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ed:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01f2:  stelem.ref
    IL_01f3:  ldloc.2
    IL_01f4:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_01f9:  stelem.ref
    IL_01fa:  ldloc.1
    IL_01fb:  ldc.i4.1
    IL_01fc:  ldtoken    method instance bool class [System.Core]System.Collections.Generic.HashSet`1<int32>::Add(!0)
    IL_0201:  ldtoken    class [System.Core]System.Collections.Generic.HashSet`1<int32>
    IL_0206:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_020b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0210:  box        [mscorlib]System.Reflection.MethodInfo
    IL_0215:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_021a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_021f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0224:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_0229:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_022e:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0233:  ldc.i4.2
    IL_0234:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0239:  stloc.2
    IL_023a:  ldloc.2
    IL_023b:  ldc.i4.0
    IL_023c:  ldtoken    class [mscorlib]System.Func`2<int32,bool>
    IL_0241:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0246:  box        [mscorlib]System.Type
    IL_024b:  ldtoken    [mscorlib]System.Type
    IL_0250:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0255:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_025a:  stelem.ref
    IL_025b:  ldloc.2
    IL_025c:  ldc.i4.1
    IL_025d:  ldloc.0
    IL_025e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0263:  ldtoken    field class [System.Core]System.Collections.Generic.HashSet`1<int32> ExpressionTrees/'<>c__DisplayClass10'::set
    IL_0268:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_026d:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0272:  stelem.ref
    IL_0273:  ldloc.2
    IL_0274:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0279:  ldtoken    class [mscorlib]System.Func`2<int32,bool>
    IL_027e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0283:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0288:  stelem.ref
    IL_0289:  ldloc.1
    IL_028a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_028f:  ldc.i4.0
    IL_0290:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0295:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_029a:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_029f:  pop
    IL_02a0:  ldloc.0
    IL_02a1:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegatef'
    IL_02a6:  brtrue.s   IL_02bb

    IL_02a8:  ldnull
    IL_02a9:  ldftn      bool ExpressionTrees::'<MethodGroupConstant>b__e'(class [mscorlib]System.Func`3<object,object,bool>)
    IL_02af:  newobj     instance void class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool>::.ctor(object,
                                                                                                                                    native int)
    IL_02b4:  stsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegatef'
    IL_02b9:  br.s       IL_02bb

    IL_02bb:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegatef'
    IL_02c0:  stfld      class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ExpressionTrees/'<>c__DisplayClass10'::sink
    IL_02c5:  call       object ExpressionTrees::X()
    IL_02ca:  ldloc.0
    IL_02cb:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_02d0:  ldtoken    field class [mscorlib]System.Func`2<class [mscorlib]System.Func`3<object,object,bool>,bool> ExpressionTrees/'<>c__DisplayClass10'::sink
    IL_02d5:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_02da:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_02df:  ldc.i4.1
    IL_02e0:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_02e5:  stloc.1
    IL_02e6:  ldloc.1
    IL_02e7:  ldc.i4.0
    IL_02e8:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_02ed:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_02f2:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_02f7:  box        [mscorlib]System.Reflection.MethodInfo
    IL_02fc:  ldtoken    [mscorlib]System.Reflection.MethodInfo
    IL_0301:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0306:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_030b:  ldtoken    method instance class [mscorlib]System.Delegate [mscorlib]System.Reflection.MethodInfo::CreateDelegate(class [mscorlib]System.Type,
                                                                                                                                object)
    IL_0310:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0315:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_031a:  ldc.i4.2
    IL_031b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0320:  stloc.2
    IL_0321:  ldloc.2
    IL_0322:  ldc.i4.0
    IL_0323:  ldtoken    class [mscorlib]System.Func`3<object,object,bool>
    IL_0328:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_032d:  box        [mscorlib]System.Type
    IL_0332:  ldtoken    [mscorlib]System.Type
    IL_0337:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_033c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0341:  stelem.ref
    IL_0342:  ldloc.2
    IL_0343:  ldc.i4.1
    IL_0344:  ldnull
    IL_0345:  ldtoken    [mscorlib]System.Object
    IL_034a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_034f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0354:  stelem.ref
    IL_0355:  ldloc.2
    IL_0356:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_035b:  ldtoken    class [mscorlib]System.Func`3<object,object,bool>
    IL_0360:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0365:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_036a:  stelem.ref
    IL_036b:  ldloc.1
    IL_036c:  call       class [System.Core]System.Linq.Expressions.InvocationExpression [System.Core]System.Linq.Expressions.Expression::Invoke(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression[])
    IL_0371:  ldc.i4.0
    IL_0372:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0377:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_037c:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0381:  pop
    IL_0382:  nop
    IL_0383:  ret
  } // end of method ExpressionTrees::MethodGroupConstant

  .method public hidebysig instance void 
          MultipleCasts() cil managed
  {
    // Code size       101 (0x65)
    .maxstack  4
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldc.i4.1
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  ldtoken    [mscorlib]System.Int32
    IL_0011:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0016:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001b:  ldc.i4.1
    IL_001c:  box        [mscorlib]System.Int32
    IL_0021:  ldtoken    [mscorlib]System.Int32
    IL_0026:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0030:  ldtoken    [mscorlib]System.Object
    IL_0035:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003a:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_003f:  ldtoken    [mscorlib]System.Int32
    IL_0044:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0049:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_004e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0053:  ldc.i4.0
    IL_0054:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0059:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005e:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0063:  pop
    IL_0064:  ret
  } // end of method ExpressionTrees::MultipleCasts

  .method public hidebysig instance void 
          MultipleDots() cil managed
  {
    // Code size       143 (0x8f)
    .maxstack  4
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldc.i4.3
    IL_0007:  box        [mscorlib]System.Int32
    IL_000c:  ldtoken    [mscorlib]System.Int32
    IL_0011:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0016:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001b:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
    IL_0020:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0025:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002a:  ldc.i4.0
    IL_002b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0030:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0035:  ldtoken    method instance string [mscorlib]System.Object::ToString()
    IL_003a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_003f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0044:  ldc.i4.0
    IL_0045:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_004a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_004f:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_0054:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0059:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005e:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0063:  ldc.i4.0
    IL_0064:  box        [mscorlib]System.Int32
    IL_0069:  ldtoken    [mscorlib]System.Int32
    IL_006e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0073:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0078:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_007d:  ldc.i4.0
    IL_007e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0083:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0088:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008d:  pop
    IL_008e:  ret
  } // end of method ExpressionTrees::MultipleDots

  .method public hidebysig instance void 
          NestedLambda() cil managed
  {
    // Code size       562 (0x232)
    .maxstack  10
    .locals init (class ExpressionTrees/'<>c__DisplayClass14' V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_3,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_4,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_5)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass14'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate13'
    IL_000d:  brtrue.s   IL_0022

    IL_000f:  ldnull
    IL_0010:  ldftn      int32 ExpressionTrees::'<NestedLambda>b__12'(class [mscorlib]System.Func`1<int32>)
    IL_0016:  newobj     instance void class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32>::.ctor(object,
                                                                                                                        native int)
    IL_001b:  stsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate13'
    IL_0020:  br.s       IL_0022

    IL_0022:  ldsfld     class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ExpressionTrees::'CS$<>9__CachedAnonymousMethodDelegate13'
    IL_0027:  stfld      class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ExpressionTrees/'<>c__DisplayClass14'::'call'
    IL_002c:  call       object ExpressionTrees::X()
    IL_0031:  ldloc.0
    IL_0032:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0037:  ldtoken    field class [mscorlib]System.Func`2<class [mscorlib]System.Func`1<int32>,int32> ExpressionTrees/'<>c__DisplayClass14'::'call'
    IL_003c:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0041:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0046:  ldc.i4.1
    IL_0047:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_004c:  stloc.1
    IL_004d:  ldloc.1
    IL_004e:  ldc.i4.0
    IL_004f:  ldc.i4.s   42
    IL_0051:  box        [mscorlib]System.Int32
    IL_0056:  ldtoken    [mscorlib]System.Int32
    IL_005b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0060:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0065:  ldc.i4.0
    IL_0066:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_006b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0070:  stelem.ref
    IL_0071:  ldloc.1
    IL_0072:  call       class [System.Core]System.Linq.Expressions.InvocationExpression [System.Core]System.Linq.Expressions.Expression::Invoke(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                 class [System.Core]System.Linq.Expressions.Expression[])
    IL_0077:  ldc.i4.0
    IL_0078:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0082:  call       object ExpressionTrees::ToCode<int32>(object,
                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0087:  pop
    IL_0088:  call       object ExpressionTrees::X()
    IL_008d:  ldnull
    IL_008e:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<int32,int32>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                       class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0093:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0098:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_009d:  ldc.i4.2
    IL_009e:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00a3:  stloc.1
    IL_00a4:  ldloc.1
    IL_00a5:  ldc.i4.0
    IL_00a6:  ldtoken    [mscorlib]System.Int32
    IL_00ab:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b0:  ldc.i4.2
    IL_00b1:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00b6:  stloc.2
    IL_00b7:  ldloc.2
    IL_00b8:  ldc.i4.0
    IL_00b9:  ldc.i4.s   37
    IL_00bb:  box        [mscorlib]System.Int32
    IL_00c0:  ldtoken    [mscorlib]System.Int32
    IL_00c5:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00ca:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cf:  stelem.ref
    IL_00d0:  ldloc.2
    IL_00d1:  ldc.i4.1
    IL_00d2:  ldc.i4.s   42
    IL_00d4:  box        [mscorlib]System.Int32
    IL_00d9:  ldtoken    [mscorlib]System.Int32
    IL_00de:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e8:  stelem.ref
    IL_00e9:  ldloc.2
    IL_00ea:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00ef:  stelem.ref
    IL_00f0:  ldloc.1
    IL_00f1:  ldc.i4.1
    IL_00f2:  ldtoken    [mscorlib]System.Int32
    IL_00f7:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00fc:  ldstr      "x"
    IL_0101:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0106:  stloc.3
    IL_0107:  ldloc.3
    IL_0108:  ldc.i4.2
    IL_0109:  box        [mscorlib]System.Int32
    IL_010e:  ldtoken    [mscorlib]System.Int32
    IL_0113:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0118:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_011d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_0122:  ldc.i4.1
    IL_0123:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0128:  stloc.s    V_4
    IL_012a:  ldloc.s    V_4
    IL_012c:  ldc.i4.0
    IL_012d:  ldloc.3
    IL_012e:  stelem.ref
    IL_012f:  ldloc.s    V_4
    IL_0131:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0136:  stelem.ref
    IL_0137:  ldloc.1
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
    IL_0169:  stloc.1
    IL_016a:  ldloc.1
    IL_016b:  ldc.i4.0
    IL_016c:  ldtoken    [mscorlib]System.Int32
    IL_0171:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0176:  ldc.i4.2
    IL_0177:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_017c:  stloc.2
    IL_017d:  ldloc.2
    IL_017e:  ldc.i4.0
    IL_017f:  ldc.i4.s   37
    IL_0181:  box        [mscorlib]System.Int32
    IL_0186:  ldtoken    [mscorlib]System.Int32
    IL_018b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0190:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0195:  stelem.ref
    IL_0196:  ldloc.2
    IL_0197:  ldc.i4.1
    IL_0198:  ldc.i4.s   42
    IL_019a:  box        [mscorlib]System.Int32
    IL_019f:  ldtoken    [mscorlib]System.Int32
    IL_01a4:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01a9:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01ae:  stelem.ref
    IL_01af:  ldloc.2
    IL_01b0:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_01b5:  stelem.ref
    IL_01b6:  ldloc.1
    IL_01b7:  ldc.i4.1
    IL_01b8:  ldtoken    [mscorlib]System.Int32
    IL_01bd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01c2:  ldstr      "x"
    IL_01c7:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01cc:  stloc.3
    IL_01cd:  ldtoken    [mscorlib]System.Int32
    IL_01d2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01d7:  ldstr      "i"
    IL_01dc:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01e1:  stloc.s    V_5
    IL_01e3:  ldloc.3
    IL_01e4:  ldc.i4.2
    IL_01e5:  box        [mscorlib]System.Int32
    IL_01ea:  ldtoken    [mscorlib]System.Int32
    IL_01ef:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01f4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01f9:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Multiply(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_01fe:  ldc.i4.2
    IL_01ff:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0204:  stloc.s    V_4
    IL_0206:  ldloc.s    V_4
    IL_0208:  ldc.i4.0
    IL_0209:  ldloc.3
    IL_020a:  stelem.ref
    IL_020b:  ldloc.s    V_4
    IL_020d:  ldc.i4.1
    IL_020e:  ldloc.s    V_5
    IL_0210:  stelem.ref
    IL_0211:  ldloc.s    V_4
    IL_0213:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0218:  stelem.ref
    IL_0219:  ldloc.1
    IL_021a:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_021f:  ldc.i4.0
    IL_0220:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0225:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                              class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_022a:  call       object ExpressionTrees::ToCode<class [mscorlib]System.Collections.Generic.IEnumerable`1<int32>>(object,
                                                                                                                         class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_022f:  pop
    IL_0230:  nop
    IL_0231:  ret
  } // end of method ExpressionTrees::NestedLambda

  .method public hidebysig instance void 
          CurriedLambda() cil managed
  {
    // Code size       140 (0x8c)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_2,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_3)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Int32
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldstr      "a"
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_001a:  stloc.0
    IL_001b:  ldtoken    [mscorlib]System.Int32
    IL_0020:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0025:  ldstr      "b"
    IL_002a:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_002f:  stloc.1
    IL_0030:  ldtoken    [mscorlib]System.Int32
    IL_0035:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003a:  ldstr      "c"
    IL_003f:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0044:  stloc.2
    IL_0045:  ldloc.0
    IL_0046:  ldloc.1
    IL_0047:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_004c:  ldloc.2
    IL_004d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0052:  ldc.i4.1
    IL_0053:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0058:  stloc.3
    IL_0059:  ldloc.3
    IL_005a:  ldc.i4.0
    IL_005b:  ldloc.2
    IL_005c:  stelem.ref
    IL_005d:  ldloc.3
    IL_005e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,int32>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0063:  ldc.i4.1
    IL_0064:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0069:  stloc.3
    IL_006a:  ldloc.3
    IL_006b:  ldc.i4.0
    IL_006c:  ldloc.1
    IL_006d:  stelem.ref
    IL_006e:  ldloc.3
    IL_006f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                               class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0074:  ldc.i4.1
    IL_0075:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_007a:  stloc.3
    IL_007b:  ldloc.3
    IL_007c:  ldc.i4.0
    IL_007d:  ldloc.0
    IL_007e:  stelem.ref
    IL_007f:  ldloc.3
    IL_0080:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                                                    class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0085:  call       object ExpressionTrees::ToCode<int32,class [mscorlib]System.Func`2<int32,class [mscorlib]System.Func`2<int32,int32>>>(object,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`2<!!0,!!1>>)
    IL_008a:  pop
    IL_008b:  ret
  } // end of method ExpressionTrees::CurriedLambda

  .method private hidebysig instance bool 
          Fizz(class [mscorlib]System.Func`2<int32,bool> a) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.s   42
    IL_0004:  callvirt   instance !1 class [mscorlib]System.Func`2<int32,bool>::Invoke(!0)
    IL_0009:  stloc.0
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.0
    IL_000d:  ret
  } // end of method ExpressionTrees::Fizz

  .method private hidebysig instance bool 
          Buzz(class [mscorlib]System.Func`2<int32,bool> a) cil managed
  {
    // Code size       14 (0xe)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldc.i4.s   42
    IL_0004:  callvirt   instance !1 class [mscorlib]System.Func`2<int32,bool>::Invoke(!0)
    IL_0009:  stloc.0
    IL_000a:  br.s       IL_000c

    IL_000c:  ldloc.0
    IL_000d:  ret
  } // end of method ExpressionTrees::Buzz

  .method private hidebysig instance bool 
          Fizz(class [mscorlib]System.Func`2<string,bool> a) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldstr      "42"
    IL_0007:  callvirt   instance !1 class [mscorlib]System.Func`2<string,bool>::Invoke(!0)
    IL_000c:  stloc.0
    IL_000d:  br.s       IL_000f

    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method ExpressionTrees::Fizz

  .method public hidebysig instance void 
          NestedLambda2() cil managed
  {
    // Code size       562 (0x232)
    .maxstack  9
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_2)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldarg.0
    IL_0007:  box        ExpressionTrees
    IL_000c:  ldtoken    ExpressionTrees
    IL_0011:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0016:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001b:  ldtoken    method instance bool ExpressionTrees::Fizz(class [mscorlib]System.Func`2<string,bool>)
    IL_0020:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0025:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002a:  ldc.i4.1
    IL_002b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0030:  stloc.0
    IL_0031:  ldloc.0
    IL_0032:  ldc.i4.0
    IL_0033:  ldtoken    [mscorlib]System.String
    IL_0038:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003d:  ldstr      "x"
    IL_0042:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_0047:  stloc.1
    IL_0048:  ldloc.1
    IL_0049:  ldstr      "a"
    IL_004e:  ldtoken    [mscorlib]System.String
    IL_0053:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0058:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005d:  ldc.i4.0
    IL_005e:  ldtoken    method bool [mscorlib]System.String::op_Equality(string,
                                                                          string)
    IL_0063:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0068:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_006d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            bool,
                                                                                                                                            class [mscorlib]System.Reflection.MethodInfo)
    IL_0072:  ldc.i4.1
    IL_0073:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0078:  stloc.2
    IL_0079:  ldloc.2
    IL_007a:  ldc.i4.0
    IL_007b:  ldloc.1
    IL_007c:  stelem.ref
    IL_007d:  ldloc.2
    IL_007e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<string,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                          class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0083:  stelem.ref
    IL_0084:  ldloc.0
    IL_0085:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_008a:  ldc.i4.0
    IL_008b:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0090:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0095:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_009a:  pop
    IL_009b:  call       object ExpressionTrees::X()
    IL_00a0:  ldarg.0
    IL_00a1:  box        ExpressionTrees
    IL_00a6:  ldtoken    ExpressionTrees
    IL_00ab:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b0:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b5:  ldtoken    method instance bool ExpressionTrees::Fizz(class [mscorlib]System.Func`2<int32,bool>)
    IL_00ba:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00bf:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00c4:  ldc.i4.1
    IL_00c5:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00ca:  stloc.0
    IL_00cb:  ldloc.0
    IL_00cc:  ldc.i4.0
    IL_00cd:  ldtoken    [mscorlib]System.Int32
    IL_00d2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d7:  ldstr      "x"
    IL_00dc:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00e1:  stloc.1
    IL_00e2:  ldloc.1
    IL_00e3:  ldc.i4.s   37
    IL_00e5:  box        [mscorlib]System.Int32
    IL_00ea:  ldtoken    [mscorlib]System.Int32
    IL_00ef:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00f4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00f9:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00fe:  ldc.i4.1
    IL_00ff:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0104:  stloc.2
    IL_0105:  ldloc.2
    IL_0106:  ldc.i4.0
    IL_0107:  ldloc.1
    IL_0108:  stelem.ref
    IL_0109:  ldloc.2
    IL_010a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
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
    IL_0127:  call       object ExpressionTrees::X()
    IL_012c:  ldarg.0
    IL_012d:  box        ExpressionTrees
    IL_0132:  ldtoken    ExpressionTrees
    IL_0137:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_013c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0141:  ldtoken    method instance bool ExpressionTrees::Fizz(class [mscorlib]System.Func`2<int32,bool>)
    IL_0146:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_014b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0150:  ldc.i4.1
    IL_0151:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0156:  stloc.0
    IL_0157:  ldloc.0
    IL_0158:  ldc.i4.0
    IL_0159:  ldtoken    [mscorlib]System.Int32
    IL_015e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0163:  ldstr      "x"
    IL_0168:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_016d:  stloc.1
    IL_016e:  ldc.i4.1
    IL_016f:  box        [mscorlib]System.Boolean
    IL_0174:  ldtoken    [mscorlib]System.Boolean
    IL_0179:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_017e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0183:  ldc.i4.1
    IL_0184:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0189:  stloc.2
    IL_018a:  ldloc.2
    IL_018b:  ldc.i4.0
    IL_018c:  ldloc.1
    IL_018d:  stelem.ref
    IL_018e:  ldloc.2
    IL_018f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0194:  stelem.ref
    IL_0195:  ldloc.0
    IL_0196:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_019b:  ldc.i4.0
    IL_019c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_01a1:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_01a6:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_01ab:  pop
    IL_01ac:  call       object ExpressionTrees::X()
    IL_01b1:  ldarg.0
    IL_01b2:  box        ExpressionTrees
    IL_01b7:  ldtoken    ExpressionTrees
    IL_01bc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01c1:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_01c6:  ldtoken    method instance bool ExpressionTrees::Buzz(class [mscorlib]System.Func`2<int32,bool>)
    IL_01cb:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_01d0:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_01d5:  ldc.i4.1
    IL_01d6:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_01db:  stloc.0
    IL_01dc:  ldloc.0
    IL_01dd:  ldc.i4.0
    IL_01de:  ldtoken    [mscorlib]System.Int32
    IL_01e3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01e8:  ldstr      "x"
    IL_01ed:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_01f2:  stloc.1
    IL_01f3:  ldc.i4.1
    IL_01f4:  box        [mscorlib]System.Boolean
    IL_01f9:  ldtoken    [mscorlib]System.Boolean
    IL_01fe:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0203:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0208:  ldc.i4.1
    IL_0209:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_020e:  stloc.2
    IL_020f:  ldloc.2
    IL_0210:  ldc.i4.0
    IL_0211:  ldloc.1
    IL_0212:  stelem.ref
    IL_0213:  ldloc.2
    IL_0214:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<int32,bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                         class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0219:  stelem.ref
    IL_021a:  ldloc.0
    IL_021b:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0220:  ldc.i4.0
    IL_0221:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0226:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_022b:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0230:  pop
    IL_0231:  ret
  } // end of method ExpressionTrees::NestedLambda2

  .method public hidebysig instance void 
          NewArrayAndExtensionMethod() cil managed
  {
    // Code size       297 (0x129)
    .maxstack  10
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method bool [System.Core]System.Linq.Enumerable::SequenceEqual<float64>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.2
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.0
    IL_001f:  ldtoken    [mscorlib]System.Double
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  ldc.i4.3
    IL_002a:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_002f:  stloc.1
    IL_0030:  ldloc.1
    IL_0031:  ldc.i4.0
    IL_0032:  ldc.r8     1.
    IL_003b:  box        [mscorlib]System.Double
    IL_0040:  ldtoken    [mscorlib]System.Double
    IL_0045:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_004f:  stelem.ref
    IL_0050:  ldloc.1
    IL_0051:  ldc.i4.1
    IL_0052:  ldc.r8     2.0099999999999998
    IL_005b:  box        [mscorlib]System.Double
    IL_0060:  ldtoken    [mscorlib]System.Double
    IL_0065:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_006f:  stelem.ref
    IL_0070:  ldloc.1
    IL_0071:  ldc.i4.2
    IL_0072:  ldc.r8     3.5
    IL_007b:  box        [mscorlib]System.Double
    IL_0080:  ldtoken    [mscorlib]System.Double
    IL_0085:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008a:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_008f:  stelem.ref
    IL_0090:  ldloc.1
    IL_0091:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0096:  stelem.ref
    IL_0097:  ldloc.0
    IL_0098:  ldc.i4.1
    IL_0099:  ldtoken    [mscorlib]System.Double
    IL_009e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a3:  ldc.i4.3
    IL_00a4:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00a9:  stloc.1
    IL_00aa:  ldloc.1
    IL_00ab:  ldc.i4.0
    IL_00ac:  ldc.r8     1.
    IL_00b5:  box        [mscorlib]System.Double
    IL_00ba:  ldtoken    [mscorlib]System.Double
    IL_00bf:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00c9:  stelem.ref
    IL_00ca:  ldloc.1
    IL_00cb:  ldc.i4.1
    IL_00cc:  ldc.r8     2.0099999999999998
    IL_00d5:  box        [mscorlib]System.Double
    IL_00da:  ldtoken    [mscorlib]System.Double
    IL_00df:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e4:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e9:  stelem.ref
    IL_00ea:  ldloc.1
    IL_00eb:  ldc.i4.2
    IL_00ec:  ldc.r8     3.5
    IL_00f5:  box        [mscorlib]System.Double
    IL_00fa:  ldtoken    [mscorlib]System.Double
    IL_00ff:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0104:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0109:  stelem.ref
    IL_010a:  ldloc.1
    IL_010b:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_0110:  stelem.ref
    IL_0111:  ldloc.0
    IL_0112:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0117:  ldc.i4.0
    IL_0118:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_011d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0122:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0127:  pop
    IL_0128:  ret
  } // end of method ExpressionTrees::NewArrayAndExtensionMethod

  .method public hidebysig instance void 
          NewMultiDimArray() cil managed
  {
    // Code size       141 (0x8d)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Int32
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldc.i4.2
    IL_0011:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4.0
    IL_0019:  ldc.i4.3
    IL_001a:  box        [mscorlib]System.Int32
    IL_001f:  ldtoken    [mscorlib]System.Int32
    IL_0024:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0029:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_002e:  stelem.ref
    IL_002f:  ldloc.0
    IL_0030:  ldc.i4.1
    IL_0031:  ldc.i4.4
    IL_0032:  box        [mscorlib]System.Int32
    IL_0037:  ldtoken    [mscorlib]System.Int32
    IL_003c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0041:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0046:  stelem.ref
    IL_0047:  ldloc.0
    IL_0048:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayBounds(class [mscorlib]System.Type,
                                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_004d:  ldtoken    method instance int32 [mscorlib]System.Array::get_Length()
    IL_0052:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0057:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0061:  ldc.i4.1
    IL_0062:  box        [mscorlib]System.Int32
    IL_0067:  ldtoken    [mscorlib]System.Int32
    IL_006c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0071:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0076:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_007b:  ldc.i4.0
    IL_007c:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0081:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0086:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_008b:  pop
    IL_008c:  ret
  } // end of method ExpressionTrees::NewMultiDimArray

  .method public hidebysig instance void 
          NewObject() cil managed
  {
    // Code size       81 (0x51)
    .maxstack  4
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_000b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0010:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0015:  ldc.i4.0
    IL_0016:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001b:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0020:  ldtoken    method instance void [mscorlib]System.Object::.ctor()
    IL_0025:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002a:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_002f:  ldc.i4.0
    IL_0030:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0035:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_003a:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_003f:  ldc.i4.0
    IL_0040:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0045:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_004a:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_004f:  pop
    IL_0050:  ret
  } // end of method ExpressionTrees::NewObject

  .method public hidebysig instance void 
          NotOperator() cil managed
  {
    // Code size       242 (0xf2)
    .maxstack  4
    .locals init (class ExpressionTrees/'<>c__DisplayClass16' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass16'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      bool ExpressionTrees/'<>c__DisplayClass16'::x
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.3
    IL_0010:  stfld      int32 ExpressionTrees/'<>c__DisplayClass16'::y
    IL_0015:  ldloc.0
    IL_0016:  ldc.i4.s   42
    IL_0018:  stfld      uint8 ExpressionTrees/'<>c__DisplayClass16'::z
    IL_001d:  call       object ExpressionTrees::X()
    IL_0022:  ldloc.0
    IL_0023:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0028:  ldtoken    field uint8 ExpressionTrees/'<>c__DisplayClass16'::z
    IL_002d:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0032:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0037:  ldtoken    [mscorlib]System.Int32
    IL_003c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0041:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0046:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_004b:  ldc.i4.0
    IL_004c:  box        [mscorlib]System.Int32
    IL_0051:  ldtoken    [mscorlib]System.Int32
    IL_0056:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005b:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0060:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_0065:  ldc.i4.0
    IL_0066:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_006b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0070:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0075:  pop
    IL_0076:  call       object ExpressionTrees::X()
    IL_007b:  ldloc.0
    IL_007c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0081:  ldtoken    field int32 ExpressionTrees/'<>c__DisplayClass16'::y
    IL_0086:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_008b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0090:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_0095:  ldc.i4.0
    IL_0096:  box        [mscorlib]System.Int32
    IL_009b:  ldtoken    [mscorlib]System.Int32
    IL_00a0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00a5:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00aa:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00af:  ldc.i4.0
    IL_00b0:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00b5:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ba:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00bf:  pop
    IL_00c0:  call       object ExpressionTrees::X()
    IL_00c5:  ldloc.0
    IL_00c6:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_00cb:  ldtoken    field bool ExpressionTrees/'<>c__DisplayClass16'::x
    IL_00d0:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00d5:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00da:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_00df:  ldc.i4.0
    IL_00e0:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00e5:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ea:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00ef:  pop
    IL_00f0:  nop
    IL_00f1:  ret
  } // end of method ExpressionTrees::NotOperator

  .method public hidebysig instance void 
          ObjectInitializers() cil managed
  {
    // Code size       279 (0x117)
    .maxstack  7
    .locals init (class [System.Xml]System.Xml.XmlReaderSettings V_0,
             class ExpressionTrees/'<>c__DisplayClass19' V_1,
             class [System.Core]System.Linq.Expressions.MemberBinding[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass19'::.ctor()
    IL_0005:  stloc.1
    IL_0006:  nop
    IL_0007:  ldloc.1
    IL_0008:  newobj     instance void [System.Xml]System.Xml.XmlReaderSettings::.ctor()
    IL_000d:  stloc.0
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.0
    IL_0010:  callvirt   instance void [System.Xml]System.Xml.XmlReaderSettings::set_CloseInput(bool)
    IL_0015:  nop
    IL_0016:  ldloc.0
    IL_0017:  ldc.i4.0
    IL_0018:  callvirt   instance void [System.Xml]System.Xml.XmlReaderSettings::set_CheckCharacters(bool)
    IL_001d:  nop
    IL_001e:  ldloc.0
    IL_001f:  stfld      class [System.Xml]System.Xml.XmlReaderSettings ExpressionTrees/'<>c__DisplayClass19'::s
    IL_0024:  call       object ExpressionTrees::X()
    IL_0029:  ldtoken    method instance void [System.Xml]System.Xml.XmlReaderSettings::.ctor()
    IL_002e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0033:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_0038:  ldc.i4.0
    IL_0039:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_003e:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0043:  ldc.i4.2
    IL_0044:  newarr     [System.Core]System.Linq.Expressions.MemberBinding
    IL_0049:  stloc.2
    IL_004a:  ldloc.2
    IL_004b:  ldc.i4.0
    IL_004c:  ldtoken    method instance void [System.Xml]System.Xml.XmlReaderSettings::set_CloseInput(bool)
    IL_0051:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0056:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005b:  ldloc.1
    IL_005c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0061:  ldtoken    field class [System.Xml]System.Xml.XmlReaderSettings ExpressionTrees/'<>c__DisplayClass19'::s
    IL_0066:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_006b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0070:  ldtoken    method instance bool [System.Xml]System.Xml.XmlReaderSettings::get_CloseInput()
    IL_0075:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_007a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007f:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0084:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_0089:  stelem.ref
    IL_008a:  ldloc.2
    IL_008b:  ldc.i4.1
    IL_008c:  ldtoken    method instance void [System.Xml]System.Xml.XmlReaderSettings::set_CheckCharacters(bool)
    IL_0091:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0096:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_009b:  ldloc.1
    IL_009c:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_00a1:  ldtoken    field class [System.Xml]System.Xml.XmlReaderSettings ExpressionTrees/'<>c__DisplayClass19'::s
    IL_00a6:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00ab:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00b0:  ldtoken    method instance bool [System.Xml]System.Xml.XmlReaderSettings::get_CheckCharacters()
    IL_00b5:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00ba:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00bf:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_00c4:  call       class [System.Core]System.Linq.Expressions.MemberAssignment [System.Core]System.Linq.Expressions.Expression::Bind(class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                           class [System.Core]System.Linq.Expressions.Expression)
    IL_00c9:  stelem.ref
    IL_00ca:  ldloc.2
    IL_00cb:  call       class [System.Core]System.Linq.Expressions.MemberInitExpression [System.Core]System.Linq.Expressions.Expression::MemberInit(class [System.Core]System.Linq.Expressions.NewExpression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.MemberBinding[])
    IL_00d0:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_00d5:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00da:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00df:  ldc.i4.1
    IL_00e0:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00e5:  stloc.3
    IL_00e6:  ldloc.3
    IL_00e7:  ldc.i4.0
    IL_00e8:  ldloc.1
    IL_00e9:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_00ee:  ldtoken    field class [System.Xml]System.Xml.XmlReaderSettings ExpressionTrees/'<>c__DisplayClass19'::s
    IL_00f3:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00f8:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00fd:  stelem.ref
    IL_00fe:  ldloc.3
    IL_00ff:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0104:  ldc.i4.0
    IL_0105:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_010a:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_010f:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0114:  pop
    IL_0115:  nop
    IL_0116:  ret
  } // end of method ExpressionTrees::ObjectInitializers

  .method public hidebysig instance void 
          Quoted() cil managed
  {
    // Code size       181 (0xb5)
    .maxstack  5
    .locals init (class [System.Core]System.Linq.Expressions.ParameterExpression V_0,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_1,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_2)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Int32
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldstr      "n"
    IL_0015:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_001a:  stloc.0
    IL_001b:  ldtoken    [mscorlib]System.String
    IL_0020:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0025:  ldstr      "s"
    IL_002a:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_002f:  stloc.1
    IL_0030:  ldloc.1
    IL_0031:  ldloc.0
    IL_0032:  ldtoken    method instance string [mscorlib]System.Int32::ToString()
    IL_0037:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_003c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0041:  ldc.i4.0
    IL_0042:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0047:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_004c:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_0051:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0056:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_005b:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_0060:  ldc.i4.2
    IL_0061:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0066:  stloc.2
    IL_0067:  ldloc.2
    IL_0068:  ldc.i4.0
    IL_0069:  ldloc.0
    IL_006a:  stelem.ref
    IL_006b:  ldloc.2
    IL_006c:  ldc.i4.1
    IL_006d:  ldloc.1
    IL_006e:  stelem.ref
    IL_006f:  ldloc.2
    IL_0070:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`3<int32,string,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0075:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Quote(class [System.Core]System.Linq.Expressions.Expression)
    IL_007a:  ldtoken    class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`3<int32,string,string>>
    IL_007f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0084:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0089:  ldnull
    IL_008a:  box        [mscorlib]System.Object
    IL_008f:  ldtoken    [mscorlib]System.Object
    IL_0094:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0099:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::NotEqual(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_00a3:  ldc.i4.0
    IL_00a4:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a9:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00ae:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00b3:  pop
    IL_00b4:  ret
  } // end of method ExpressionTrees::Quoted

  .method public hidebysig instance void 
          Quoted2() cil managed
  {
    // Code size       175 (0xaf)
    .maxstack  8
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method object ExpressionTrees::ToCode<bool>(object,
                                                                     class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.2
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.0
    IL_001f:  ldnull
    IL_0020:  ldtoken    method object ExpressionTrees::X()
    IL_0025:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002f:  ldc.i4.0
    IL_0030:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0035:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_003a:  stelem.ref
    IL_003b:  ldloc.0
    IL_003c:  ldc.i4.1
    IL_003d:  ldc.i4.1
    IL_003e:  box        [mscorlib]System.Boolean
    IL_0043:  ldtoken    [mscorlib]System.Boolean
    IL_0048:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_004d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0052:  ldc.i4.0
    IL_0053:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0058:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_005d:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Quote(class [System.Core]System.Linq.Expressions.Expression)
    IL_0062:  stelem.ref
    IL_0063:  ldloc.0
    IL_0064:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0069:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_006e:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0073:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0078:  ldc.i4.1
    IL_0079:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_007e:  stloc.0
    IL_007f:  ldloc.0
    IL_0080:  ldc.i4.0
    IL_0081:  ldnull
    IL_0082:  box        [mscorlib]System.Object
    IL_0087:  ldtoken    [mscorlib]System.Object
    IL_008c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0091:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0096:  stelem.ref
    IL_0097:  ldloc.0
    IL_0098:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_009d:  ldc.i4.0
    IL_009e:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00a3:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00a8:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00ad:  pop
    IL_00ae:  ret
  } // end of method ExpressionTrees::Quoted2

  .method public hidebysig instance void 
          QuotedWithAnonymous() cil managed
  {
    // Code size       371 (0x173)
    .maxstack  18
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0,
             class [System.Core]System.Linq.Expressions.Expression[] V_1,
             class [System.Core]System.Linq.Expressions.Expression[] V_2,
             class [System.Core]System.Linq.Expressions.Expression[] V_3,
             class [mscorlib]System.Reflection.MethodInfo[] V_4,
             class [System.Core]System.Linq.Expressions.ParameterExpression V_5,
             class [System.Core]System.Linq.Expressions.ParameterExpression[] V_6)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method !!0 [System.Core]System.Linq.Enumerable::Single<string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.1
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.0
    IL_001f:  ldnull
    IL_0020:  ldtoken    method class [mscorlib]System.Collections.Generic.IEnumerable`1<!!1> [System.Core]System.Linq.Enumerable::Select<class '<>f__AnonymousType1`2'<string,string>,string>(class [mscorlib]System.Collections.Generic.IEnumerable`1<!!0>,
                                                                                                                                                                                               class [mscorlib]System.Func`2<!!0,!!1>)
    IL_0025:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_002a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002f:  ldc.i4.2
    IL_0030:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0035:  stloc.1
    IL_0036:  ldloc.1
    IL_0037:  ldc.i4.0
    IL_0038:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_003d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0042:  ldc.i4.1
    IL_0043:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0048:  stloc.2
    IL_0049:  ldloc.2
    IL_004a:  ldc.i4.0
    IL_004b:  ldtoken    method instance void class '<>f__AnonymousType1`2'<string,string>::.ctor(!0,
                                                                                                  !1)
    IL_0050:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_0055:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_005a:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_005f:  ldc.i4.2
    IL_0060:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0065:  stloc.3
    IL_0066:  ldloc.3
    IL_0067:  ldc.i4.0
    IL_0068:  ldstr      "a"
    IL_006d:  ldtoken    [mscorlib]System.String
    IL_0072:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0077:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_007c:  stelem.ref
    IL_007d:  ldloc.3
    IL_007e:  ldc.i4.1
    IL_007f:  ldstr      "b"
    IL_0084:  ldtoken    [mscorlib]System.String
    IL_0089:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_008e:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0093:  stelem.ref
    IL_0094:  ldloc.3
    IL_0095:  ldc.i4.2
    IL_0096:  newarr     [mscorlib]System.Reflection.MethodInfo
    IL_009b:  stloc.s    V_4
    IL_009d:  ldloc.s    V_4
    IL_009f:  ldc.i4.0
    IL_00a0:  ldtoken    method instance !0 class '<>f__AnonymousType1`2'<string,string>::get_X()
    IL_00a5:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00aa:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00af:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00b4:  stelem.ref
    IL_00b5:  ldloc.s    V_4
    IL_00b7:  ldc.i4.1
    IL_00b8:  ldtoken    method instance !1 class '<>f__AnonymousType1`2'<string,string>::get_Y()
    IL_00bd:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00c2:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c7:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00cc:  stelem.ref
    IL_00cd:  ldloc.s    V_4
    IL_00cf:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [mscorlib]System.Collections.Generic.IEnumerable`1<class [System.Core]System.Linq.Expressions.Expression>,
                                                                                                                                       class [mscorlib]System.Reflection.MemberInfo[])
    IL_00d4:  stelem.ref
    IL_00d5:  ldloc.2
    IL_00d6:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_00db:  stelem.ref
    IL_00dc:  ldloc.1
    IL_00dd:  ldc.i4.1
    IL_00de:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_00e3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e8:  ldstr      "o"
    IL_00ed:  call       class [System.Core]System.Linq.Expressions.ParameterExpression [System.Core]System.Linq.Expressions.Expression::Parameter(class [mscorlib]System.Type,
                                                                                                                                                   string)
    IL_00f2:  stloc.s    V_5
    IL_00f4:  ldloc.s    V_5
    IL_00f6:  ldtoken    method instance !0 class '<>f__AnonymousType1`2'<string,string>::get_X()
    IL_00fb:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_0100:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0105:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_010a:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_010f:  ldloc.s    V_5
    IL_0111:  ldtoken    method instance !1 class '<>f__AnonymousType1`2'<string,string>::get_Y()
    IL_0116:  ldtoken    class '<>f__AnonymousType1`2'<string,string>
    IL_011b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0120:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0125:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_012a:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_012f:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0134:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0139:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_013e:  ldc.i4.1
    IL_013f:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0144:  stloc.s    V_6
    IL_0146:  ldloc.s    V_6
    IL_0148:  ldc.i4.0
    IL_0149:  ldloc.s    V_5
    IL_014b:  stelem.ref
    IL_014c:  ldloc.s    V_6
    IL_014e:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`2<class '<>f__AnonymousType1`2'<string,string>,string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                                                  class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0153:  stelem.ref
    IL_0154:  ldloc.1
    IL_0155:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_015a:  stelem.ref
    IL_015b:  ldloc.0
    IL_015c:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0161:  ldc.i4.0
    IL_0162:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0167:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<string>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                     class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_016c:  call       object ExpressionTrees::ToCode<string>(object,
                                                                class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0171:  pop
    IL_0172:  ret
  } // end of method ExpressionTrees::QuotedWithAnonymous

  .method public hidebysig instance void 
          StaticCall() cil managed
  {
    // Code size       131 (0x83)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.2
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.0
    IL_001f:  ldc.i4.3
    IL_0020:  box        [mscorlib]System.Int32
    IL_0025:  ldtoken    [mscorlib]System.Int32
    IL_002a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0034:  ldtoken    [mscorlib]System.Object
    IL_0039:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003e:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0043:  stelem.ref
    IL_0044:  ldloc.0
    IL_0045:  ldc.i4.1
    IL_0046:  ldc.i4.0
    IL_0047:  box        [mscorlib]System.Int32
    IL_004c:  ldtoken    [mscorlib]System.Int32
    IL_0051:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0056:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005b:  ldtoken    [mscorlib]System.Object
    IL_0060:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0065:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_006a:  stelem.ref
    IL_006b:  ldloc.0
    IL_006c:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0071:  ldc.i4.0
    IL_0072:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0077:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_007c:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0081:  pop
    IL_0082:  ret
  } // end of method ExpressionTrees::StaticCall

  .method public hidebysig instance void 
          ThisCall() cil managed
  {
    // Code size       117 (0x75)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldarg.0
    IL_0007:  box        ExpressionTrees
    IL_000c:  ldtoken    ExpressionTrees
    IL_0011:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0016:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_001b:  ldtoken    method instance bool [mscorlib]System.Object::Equals(object)
    IL_0020:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0025:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002a:  ldc.i4.1
    IL_002b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0030:  stloc.0
    IL_0031:  ldloc.0
    IL_0032:  ldc.i4.0
    IL_0033:  ldc.i4.3
    IL_0034:  box        [mscorlib]System.Int32
    IL_0039:  ldtoken    [mscorlib]System.Int32
    IL_003e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0043:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0048:  ldtoken    [mscorlib]System.Object
    IL_004d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0052:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0057:  stelem.ref
    IL_0058:  ldloc.0
    IL_0059:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_005e:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Not(class [System.Core]System.Linq.Expressions.Expression)
    IL_0063:  ldc.i4.0
    IL_0064:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0069:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006e:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0073:  pop
    IL_0074:  ret
  } // end of method ExpressionTrees::ThisCall

  .method public hidebysig instance void 
          ThisExplicit() cil managed
  {
    // Code size       116 (0x74)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.2
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.0
    IL_001f:  ldarg.0
    IL_0020:  box        ExpressionTrees
    IL_0025:  ldtoken    ExpressionTrees
    IL_002a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0034:  stelem.ref
    IL_0035:  ldloc.0
    IL_0036:  ldc.i4.1
    IL_0037:  ldc.i4.3
    IL_0038:  box        [mscorlib]System.Int32
    IL_003d:  ldtoken    [mscorlib]System.Int32
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
  } // end of method ExpressionTrees::ThisExplicit

  .method public hidebysig instance void 
          TypedConstant() cil managed
  {
    // Code size       113 (0x71)
    .maxstack  6
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldtoken    [mscorlib]System.Type
    IL_000b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0010:  ldc.i4.2
    IL_0011:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0016:  stloc.0
    IL_0017:  ldloc.0
    IL_0018:  ldc.i4.0
    IL_0019:  ldtoken    [mscorlib]System.Int32
    IL_001e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0023:  box        [mscorlib]System.Type
    IL_0028:  ldtoken    [mscorlib]System.Type
    IL_002d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0032:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0037:  stelem.ref
    IL_0038:  ldloc.0
    IL_0039:  ldc.i4.1
    IL_003a:  ldtoken    [mscorlib]System.String
    IL_003f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0044:  box        [mscorlib]System.Type
    IL_0049:  ldtoken    [mscorlib]System.Type
    IL_004e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0053:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0058:  stelem.ref
    IL_0059:  ldloc.0
    IL_005a:  call       class [System.Core]System.Linq.Expressions.NewArrayExpression [System.Core]System.Linq.Expressions.Expression::NewArrayInit(class [mscorlib]System.Type,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression[])
    IL_005f:  ldc.i4.0
    IL_0060:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0065:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<class [mscorlib]System.Type[]>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                                            class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_006a:  call       object ExpressionTrees::ToCode<class [mscorlib]System.Type[]>(object,
                                                                                       class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_006f:  pop
    IL_0070:  ret
  } // end of method ExpressionTrees::TypedConstant

  .method public hidebysig instance void 
          StaticCallImplicitCast() cil managed
  {
    // Code size       131 (0x83)
    .maxstack  7
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method bool [mscorlib]System.Object::Equals(object,
                                                                     object)
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  ldc.i4.2
    IL_0017:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_001c:  stloc.0
    IL_001d:  ldloc.0
    IL_001e:  ldc.i4.0
    IL_001f:  ldc.i4.3
    IL_0020:  box        [mscorlib]System.Int32
    IL_0025:  ldtoken    [mscorlib]System.Int32
    IL_002a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002f:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0034:  ldtoken    [mscorlib]System.Object
    IL_0039:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003e:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0043:  stelem.ref
    IL_0044:  ldloc.0
    IL_0045:  ldc.i4.1
    IL_0046:  ldc.i4.0
    IL_0047:  box        [mscorlib]System.Int32
    IL_004c:  ldtoken    [mscorlib]System.Int32
    IL_0051:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0056:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_005b:  ldtoken    [mscorlib]System.Object
    IL_0060:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0065:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_006a:  stelem.ref
    IL_006b:  ldloc.0
    IL_006c:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0071:  ldc.i4.0
    IL_0072:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_0077:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_007c:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0081:  pop
    IL_0082:  ret
  } // end of method ExpressionTrees::StaticCallImplicitCast

  .method public hidebysig instance void 
          StaticMembers() cil managed
  {
    // Code size       235 (0xeb)
    .maxstack  9
    .locals init (class [System.Core]System.Linq.Expressions.Expression[] V_0)
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_000c:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0011:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0016:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_001b:  ldnull
    IL_001c:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_0021:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0026:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_002b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0030:  ldnull
    IL_0031:  ldtoken    method valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::FromMilliseconds(float64)
    IL_0036:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_003b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0040:  ldc.i4.1
    IL_0041:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0046:  stloc.0
    IL_0047:  ldloc.0
    IL_0048:  ldc.i4.0
    IL_0049:  ldc.r8     10.000999999999999
    IL_0052:  box        [mscorlib]System.Double
    IL_0057:  ldtoken    [mscorlib]System.Double
    IL_005c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0061:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0066:  stelem.ref
    IL_0067:  ldloc.0
    IL_0068:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_006d:  ldtoken    method valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::op_Addition(valuetype [mscorlib]System.DateTime,
                                                                                                           valuetype [mscorlib]System.TimeSpan)
    IL_0072:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0077:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007c:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_0081:  ldc.i4.0
    IL_0082:  ldtoken    method bool [mscorlib]System.DateTime::op_GreaterThan(valuetype [mscorlib]System.DateTime,
                                                                               valuetype [mscorlib]System.DateTime)
    IL_0087:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_008c:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0091:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  bool,
                                                                                                                                                  class [mscorlib]System.Reflection.MethodInfo)
    IL_0096:  ldtoken    method instance string [mscorlib]System.Boolean::ToString()
    IL_009b:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00a0:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00a5:  ldc.i4.0
    IL_00a6:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_00ab:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_00b0:  ldstr      "False"
    IL_00b5:  ldtoken    [mscorlib]System.String
    IL_00ba:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00bf:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00c4:  ldc.i4.0
    IL_00c5:  ldtoken    method bool [mscorlib]System.String::op_Equality(string,
                                                                          string)
    IL_00ca:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_00cf:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_00d4:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            bool,
                                                                                                                                            class [mscorlib]System.Reflection.MethodInfo)
    IL_00d9:  ldc.i4.0
    IL_00da:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_00df:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_00e4:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_00e9:  pop
    IL_00ea:  ret
  } // end of method ExpressionTrees::StaticMembers

  .method public hidebysig instance void 
          Strings() cil managed
  {
    // Code size       378 (0x17a)
    .maxstack  8
    .locals init (class ExpressionTrees/'<>c__DisplayClass1b' V_0)
    IL_0000:  newobj     instance void ExpressionTrees/'<>c__DisplayClass1b'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  nop
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      int32 ExpressionTrees/'<>c__DisplayClass1b'::i
    IL_000e:  ldloc.0
    IL_000f:  ldstr      "X"
    IL_0014:  stfld      string ExpressionTrees/'<>c__DisplayClass1b'::x
    IL_0019:  call       object ExpressionTrees::X()
    IL_001e:  ldstr      "a\n\\b"
    IL_0023:  ldtoken    [mscorlib]System.String
    IL_0028:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0032:  ldloc.0
    IL_0033:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0038:  ldtoken    field string ExpressionTrees/'<>c__DisplayClass1b'::x
    IL_003d:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_0042:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0047:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Coalesce(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression)
    IL_004c:  ldloc.0
    IL_004d:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_0052:  ldtoken    field string ExpressionTrees/'<>c__DisplayClass1b'::x
    IL_0057:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_005c:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0061:  ldtoken    method string [mscorlib]System.String::Concat(string,
                                                                       string)
    IL_0066:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_006b:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0070:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [mscorlib]System.Reflection.MethodInfo)
    IL_0075:  ldtoken    method instance int32 [mscorlib]System.String::get_Length()
    IL_007a:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_007f:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_0084:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0089:  ldc.i4.2
    IL_008a:  box        [mscorlib]System.Int32
    IL_008f:  ldtoken    [mscorlib]System.Int32
    IL_0094:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0099:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_009e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Equal(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [System.Core]System.Linq.Expressions.Expression)
    IL_00a3:  ldc.i4.0
    IL_00a4:  box        [mscorlib]System.Boolean
    IL_00a9:  ldtoken    [mscorlib]System.Boolean
    IL_00ae:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00b3:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00b8:  ldc.i4.1
    IL_00b9:  box        [mscorlib]System.Boolean
    IL_00be:  ldtoken    [mscorlib]System.Boolean
    IL_00c3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00c8:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00cd:  ldc.i4.1
    IL_00ce:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_00d3:  box        [mscorlib]System.Decimal
    IL_00d8:  ldtoken    [mscorlib]System.Decimal
    IL_00dd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00e2:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_00e7:  ldloc.0
    IL_00e8:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object)
    IL_00ed:  ldtoken    field int32 ExpressionTrees/'<>c__DisplayClass1b'::i
    IL_00f2:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle)
    IL_00f7:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_00fc:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Negate(class [System.Core]System.Linq.Expressions.Expression)
    IL_0101:  ldtoken    [mscorlib]System.Decimal
    IL_0106:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_010b:  ldtoken    method valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::op_Implicit(int32)
    IL_0110:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle)
    IL_0115:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_011a:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type,
                                                                                                                                             class [mscorlib]System.Reflection.MethodInfo)
    IL_011f:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0124:  ldc.i4.0
    IL_0125:  newobj     instance void [mscorlib]System.Decimal::.ctor(int32)
    IL_012a:  box        [mscorlib]System.Decimal
    IL_012f:  ldtoken    [mscorlib]System.Decimal
    IL_0134:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0139:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_013e:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::GreaterThan(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                  class [System.Core]System.Linq.Expressions.Expression)
    IL_0143:  ldc.i4.0
    IL_0144:  box        [mscorlib]System.Boolean
    IL_0149:  ldtoken    [mscorlib]System.Boolean
    IL_014e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0153:  call       class [System.Core]System.Linq.Expressions.ConstantExpression [System.Core]System.Linq.Expressions.Expression::Constant(object,
                                                                                                                                                 class [mscorlib]System.Type)
    IL_0158:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::OrElse(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [System.Core]System.Linq.Expressions.Expression)
    IL_015d:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::AndAlso(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                              class [System.Core]System.Linq.Expressions.Expression)
    IL_0162:  call       class [System.Core]System.Linq.Expressions.ConditionalExpression [System.Core]System.Linq.Expressions.Expression::Condition(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                     class [System.Core]System.Linq.Expressions.Expression)
    IL_0167:  ldc.i4.0
    IL_0168:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_016d:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0172:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0177:  pop
    IL_0178:  nop
    IL_0179:  ret
  } // end of method ExpressionTrees::Strings

  .method public hidebysig instance void 
          GenericClassInstance() cil managed
  {
    // Code size       151 (0x97)
    .maxstack  5
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldtoken    method instance void class ExpressionTrees/GenericClass`1<int32>::.ctor()
    IL_000b:  ldtoken    class ExpressionTrees/GenericClass`1<int32>
    IL_0010:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0015:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_001a:  ldc.i4.0
    IL_001b:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0020:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0025:  ldtoken    field !0 class ExpressionTrees/GenericClass`1<int32>::InstanceField
    IL_002a:  ldtoken    class ExpressionTrees/GenericClass`1<int32>
    IL_002f:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle,
                                                                                                                               valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0034:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_0039:  ldtoken    [mscorlib]System.Double
    IL_003e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0043:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_0048:  ldtoken    method instance void class ExpressionTrees/GenericClass`1<float64>::.ctor()
    IL_004d:  ldtoken    class ExpressionTrees/GenericClass`1<float64>
    IL_0052:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0057:  castclass  [mscorlib]System.Reflection.ConstructorInfo
    IL_005c:  ldc.i4.0
    IL_005d:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0062:  call       class [System.Core]System.Linq.Expressions.NewExpression [System.Core]System.Linq.Expressions.Expression::New(class [mscorlib]System.Reflection.ConstructorInfo,
                                                                                                                                       class [System.Core]System.Linq.Expressions.Expression[])
    IL_0067:  ldtoken    method instance !0 class ExpressionTrees/GenericClass`1<float64>::get_InstanceProperty()
    IL_006c:  ldtoken    class ExpressionTrees/GenericClass`1<float64>
    IL_0071:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0076:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_007b:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0080:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0085:  ldc.i4.0
    IL_0086:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_008b:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<float64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                      class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0090:  call       object ExpressionTrees::ToCode<float64>(object,
                                                                 class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0095:  pop
    IL_0096:  ret
  } // end of method ExpressionTrees::GenericClassInstance

  .method public hidebysig instance void 
          GenericClassStatic() cil managed
  {
    // Code size       91 (0x5b)
    .maxstack  5
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    field !0 class ExpressionTrees/GenericClass`1<int32>::StaticField
    IL_000c:  ldtoken    class ExpressionTrees/GenericClass`1<int32>
    IL_0011:  call       class [mscorlib]System.Reflection.FieldInfo [mscorlib]System.Reflection.FieldInfo::GetFieldFromHandle(valuetype [mscorlib]System.RuntimeFieldHandle,
                                                                                                                               valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0016:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Field(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                            class [mscorlib]System.Reflection.FieldInfo)
    IL_001b:  ldtoken    [mscorlib]System.Double
    IL_0020:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0025:  call       class [System.Core]System.Linq.Expressions.UnaryExpression [System.Core]System.Linq.Expressions.Expression::Convert(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                             class [mscorlib]System.Type)
    IL_002a:  ldnull
    IL_002b:  ldtoken    method !0 class ExpressionTrees/GenericClass`1<float64>::get_StaticProperty()
    IL_0030:  ldtoken    class ExpressionTrees/GenericClass`1<float64>
    IL_0035:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003a:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_003f:  call       class [System.Core]System.Linq.Expressions.MemberExpression [System.Core]System.Linq.Expressions.Expression::Property(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo)
    IL_0044:  call       class [System.Core]System.Linq.Expressions.BinaryExpression [System.Core]System.Linq.Expressions.Expression::Add(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                          class [System.Core]System.Linq.Expressions.Expression)
    IL_0049:  ldc.i4.0
    IL_004a:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_004f:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<float64>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                      class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0054:  call       object ExpressionTrees::ToCode<float64>(object,
                                                                 class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0059:  pop
    IL_005a:  ret
  } // end of method ExpressionTrees::GenericClassStatic

  .method public hidebysig instance void 
          InvokeGenericMethod() cil managed
  {
    // Code size       56 (0x38)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  call       object ExpressionTrees::X()
    IL_0006:  ldnull
    IL_0007:  ldtoken    method bool class ExpressionTrees/GenericClass`1<int32>::GenericMethod<float64>()
    IL_000c:  ldtoken    class ExpressionTrees/GenericClass`1<int32>
    IL_0011:  call       class [mscorlib]System.Reflection.MethodBase [mscorlib]System.Reflection.MethodBase::GetMethodFromHandle(valuetype [mscorlib]System.RuntimeMethodHandle,
                                                                                                                                  valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0016:  castclass  [mscorlib]System.Reflection.MethodInfo
    IL_001b:  ldc.i4.0
    IL_001c:  newarr     [System.Core]System.Linq.Expressions.Expression
    IL_0021:  call       class [System.Core]System.Linq.Expressions.MethodCallExpression [System.Core]System.Linq.Expressions.Expression::Call(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                               class [mscorlib]System.Reflection.MethodInfo,
                                                                                                                                               class [System.Core]System.Linq.Expressions.Expression[])
    IL_0026:  ldc.i4.0
    IL_0027:  newarr     [System.Core]System.Linq.Expressions.ParameterExpression
    IL_002c:  call       class [System.Core]System.Linq.Expressions.Expression`1<!!0> [System.Core]System.Linq.Expressions.Expression::Lambda<class [mscorlib]System.Func`1<bool>>(class [System.Core]System.Linq.Expressions.Expression,
                                                                                                                                                                                   class [System.Core]System.Linq.Expressions.ParameterExpression[])
    IL_0031:  call       object ExpressionTrees::ToCode<bool>(object,
                                                              class [System.Core]System.Linq.Expressions.Expression`1<class [mscorlib]System.Func`1<!!0>>)
    IL_0036:  pop
    IL_0037:  ret
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
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  ldarga.s   n
    IL_0002:  call       instance string [mscorlib]System.Int32::ToString()
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method ExpressionTrees::'<IndexerAccess>b__6'

  .method private hidebysig static bool  '<MethodGroupConstant>b__e'(class [mscorlib]System.Func`3<object,object,bool> f) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       13 (0xd)
    .maxstack  3
    .locals init (bool V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldnull
    IL_0002:  ldnull
    IL_0003:  callvirt   instance !2 class [mscorlib]System.Func`3<object,object,bool>::Invoke(!0,
                                                                                               !1)
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method ExpressionTrees::'<MethodGroupConstant>b__e'

  .method private hidebysig static int32 
          '<NestedLambda>b__12'(class [mscorlib]System.Func`1<int32> f) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance !0 class [mscorlib]System.Func`1<int32>::Invoke()
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
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
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!'<X>j__TPar' V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method '<>f__AnonymousType0`2'::get_X

  .method public hidebysig specialname instance !'<A>j__TPar' 
          get_A() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!'<A>j__TPar' V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method '<>f__AnonymousType0`2'::get_A

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       89 (0x59)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0,
             string V_1)
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
    IL_0054:  stloc.1
    IL_0055:  br.s       IL_0057

    IL_0057:  ldloc.1
    IL_0058:  ret
  } // end of method '<>f__AnonymousType0`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       65 (0x41)
    .maxstack  3
    .locals init (class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'> V_0,
             bool V_1)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_003a

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<X>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_003a

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType0`2'<!'<X>j__TPar',!'<A>j__TPar'>::'<A>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<A>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  br.s       IL_003b

    IL_003a:  ldc.i4.0
    IL_003b:  nop
    IL_003c:  stloc.1
    IL_003d:  br.s       IL_003f

    IL_003f:  ldloc.1
    IL_0040:  ret
  } // end of method '<>f__AnonymousType0`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       62 (0x3e)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1)
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
    IL_0039:  stloc.1
    IL_003a:  br.s       IL_003c

    IL_003c:  ldloc.1
    IL_003d:  ret
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
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!'<X>j__TPar' V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method '<>f__AnonymousType1`2'::get_X

  .method public hidebysig specialname instance !'<Y>j__TPar' 
          get_Y() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!'<Y>j__TPar' V_0)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method '<>f__AnonymousType1`2'::get_Y

  .method public hidebysig virtual instance string 
          ToString() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       89 (0x59)
    .maxstack  2
    .locals init (class [mscorlib]System.Text.StringBuilder V_0,
             string V_1)
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
    IL_0054:  stloc.1
    IL_0055:  br.s       IL_0057

    IL_0057:  ldloc.1
    IL_0058:  ret
  } // end of method '<>f__AnonymousType1`2'::ToString

  .method public hidebysig virtual instance bool 
          Equals(object 'value') cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       65 (0x41)
    .maxstack  3
    .locals init (class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'> V_0,
             bool V_1)
    IL_0000:  ldarg.1
    IL_0001:  isinst     class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  brfalse.s  IL_003a

    IL_000a:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::get_Default()
    IL_000f:  ldarg.0
    IL_0010:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_0015:  ldloc.0
    IL_0016:  ldfld      !0 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<X>i__Field'
    IL_001b:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<X>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0020:  brfalse.s  IL_003a

    IL_0022:  call       class [mscorlib]System.Collections.Generic.EqualityComparer`1<!0> class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::get_Default()
    IL_0027:  ldarg.0
    IL_0028:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_002d:  ldloc.0
    IL_002e:  ldfld      !1 class '<>f__AnonymousType1`2'<!'<X>j__TPar',!'<Y>j__TPar'>::'<Y>i__Field'
    IL_0033:  callvirt   instance bool class [mscorlib]System.Collections.Generic.EqualityComparer`1<!'<Y>j__TPar'>::Equals(!0,
                                                                                                                            !0)
    IL_0038:  br.s       IL_003b

    IL_003a:  ldc.i4.0
    IL_003b:  nop
    IL_003c:  stloc.1
    IL_003d:  br.s       IL_003f

    IL_003f:  ldloc.1
    IL_0040:  ret
  } // end of method '<>f__AnonymousType1`2'::Equals

  .method public hidebysig virtual instance int32 
          GetHashCode() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       62 (0x3e)
    .maxstack  3
    .locals init (int32 V_0,
             int32 V_1)
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
    IL_0039:  stloc.1
    IL_003a:  br.s       IL_003c

    IL_003c:  ldloc.1
    IL_003d:  ret
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
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\ExpressionTrees.res
