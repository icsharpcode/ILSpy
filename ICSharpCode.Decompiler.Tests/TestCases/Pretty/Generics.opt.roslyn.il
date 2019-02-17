



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly Generics
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
.module Generics.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics
       extends [mscorlib]System.Object
{
  .class auto ansi nested private beforefieldinit GenericClass`1<T>
         extends [mscorlib]System.Object
  {
    .method public hidebysig instance void 
            M([out] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/GenericClass`1<!T>& self) cil managed
    {
      // Code size       4 (0x4)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.0
      IL_0002:  stind.ref
      IL_0003:  ret
    } // end of method GenericClass`1::M

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method GenericClass`1::.ctor

  } // end of class GenericClass`1

  .class auto ansi nested public beforefieldinit BaseClass
         extends [mscorlib]System.Object
  {
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method BaseClass::.ctor

  } // end of class BaseClass

  .class auto ansi nested public beforefieldinit DerivedClass
         extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/BaseClass
  {
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/BaseClass::.ctor()
      IL_0006:  ret
    } // end of method DerivedClass::.ctor

  } // end of class DerivedClass

  .class auto ansi nested public beforefieldinit MyArray`1<T>
         extends [mscorlib]System.Object
  {
    .class auto ansi nested public beforefieldinit NestedClass`1<T,Y>
           extends [mscorlib]System.Object
    {
      .field public !T Item1
      .field public !Y Item2
      .method public hidebysig specialname rtspecialname 
              instance void  .ctor() cil managed
      {
        // Code size       7 (0x7)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  ret
      } // end of method NestedClass`1::.ctor

    } // end of class NestedClass`1

    .class auto ansi sealed nested public NestedEnum<T>
           extends [mscorlib]System.Enum
    {
      .field public specialname rtspecialname int32 value__
      .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/MyArray`1/NestedEnum<!T> A = int32(0x00000000)
      .field public static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/MyArray`1/NestedEnum<!T> B = int32(0x00000001)
    } // end of class NestedEnum

    .field private !T[] arr
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(int32 capacity) cil managed
    {
      // Code size       19 (0x13)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  ldarg.1
      IL_0008:  newarr     !T
      IL_000d:  stfld      !0[] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/MyArray`1<!T>::arr
      IL_0012:  ret
    } // end of method MyArray`1::.ctor

    .method public hidebysig instance void 
            Size(int32 capacity) cil managed
    {
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     !0[] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/MyArray`1<!T>::arr
      IL_0006:  ldarg.1
      IL_0007:  call       void [mscorlib]System.Array::Resize<!T>(!!0[]&,
                                                                   int32)
      IL_000c:  ret
    } // end of method MyArray`1::Size

    .method public hidebysig instance void 
            Grow(int32 capacity) cil managed
    {
      // Code size       19 (0x13)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  ldarg.0
      IL_0002:  ldfld      !0[] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/MyArray`1<!T>::arr
      IL_0007:  ldlen
      IL_0008:  conv.i4
      IL_0009:  blt.s      IL_0012

      IL_000b:  ldarg.0
      IL_000c:  ldarg.1
      IL_000d:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/MyArray`1<!T>::Size(int32)
      IL_0012:  ret
    } // end of method MyArray`1::Grow

  } // end of class MyArray`1

  .class interface abstract auto ansi nested public IInterface
  {
    .method public hidebysig newslot abstract virtual 
            instance void  Method1<class T>() cil managed
    {
    } // end of method IInterface::Method1

    .method public hidebysig newslot abstract virtual 
            instance void  Method2<class T>() cil managed
    {
    } // end of method IInterface::Method2

  } // end of class IInterface

  .class abstract auto ansi nested public beforefieldinit Base
         extends [mscorlib]System.Object
         implements ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/IInterface
  {
    .method public hidebysig newslot abstract virtual 
            instance void  Method1<class T>() cil managed
    {
    } // end of method Base::Method1

    .method private hidebysig newslot virtual final 
            instance void  ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics.IInterface.Method2<class T>() cil managed
    {
      .override ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/IInterface::Method2
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method Base::ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics.IInterface.Method2

    .method family hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method Base::.ctor

  } // end of class Base

  .class auto ansi nested public beforefieldinit Derived
         extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/Base
  {
    .method public hidebysig virtual instance void 
            Method1<class T>() cil managed
    {
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method Derived::Method1

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/Base::.ctor()
      IL_0006:  ret
    } // end of method Derived::.ctor

  } // end of class Derived

  .field private static literal valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/MyArray`1/NestedEnum<string> enumVal = int32(0x00000000)
  .field private static class [mscorlib]System.Type type1
  .field private static class [mscorlib]System.Type type2
  .field private static class [mscorlib]System.Type type3
  .field private static class [mscorlib]System.Type type4
  .field private static class [mscorlib]System.Type type5
  .field private static class [mscorlib]System.Type type6
  .method public hidebysig instance !!T  CastToTypeParameter<(ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/BaseClass) T>(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/DerivedClass d) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  unbox.any  !!T
    IL_0006:  ret
  } // end of method Generics::CastToTypeParameter

  .method public hidebysig instance !!TTarget 
          GenericAsGeneric<TSource,class TTarget>(!!TSource source) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  box        !!TSource
    IL_0006:  isinst     !!TTarget
    IL_000b:  unbox.any  !!TTarget
    IL_0010:  ret
  } // end of method Generics::GenericAsGeneric

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<!!TTarget> 
          GenericAsNullable<TSource,valuetype .ctor ([mscorlib]System.ValueType) TTarget>(!!TSource source) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  box        !!TSource
    IL_0006:  isinst     valuetype [mscorlib]System.Nullable`1<!!TTarget>
    IL_000b:  unbox.any  valuetype [mscorlib]System.Nullable`1<!!TTarget>
    IL_0010:  ret
  } // end of method Generics::GenericAsNullable

  .method public hidebysig instance !!TTarget 
          ObjectAsGeneric<class TTarget>(object source) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  isinst     !!TTarget
    IL_0006:  unbox.any  !!TTarget
    IL_000b:  ret
  } // end of method Generics::ObjectAsGeneric

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<!!TTarget> 
          ObjectAsNullable<valuetype .ctor ([mscorlib]System.ValueType) TTarget>(object source) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  isinst     valuetype [mscorlib]System.Nullable`1<!!TTarget>
    IL_0006:  unbox.any  valuetype [mscorlib]System.Nullable`1<!!TTarget>
    IL_000b:  ret
  } // end of method Generics::ObjectAsNullable

  .method public hidebysig instance !!TTarget 
          IntAsGeneric<class TTarget>(int32 source) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  box        [mscorlib]System.Int32
    IL_0006:  isinst     !!TTarget
    IL_000b:  unbox.any  !!TTarget
    IL_0010:  ret
  } // end of method Generics::IntAsGeneric

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<!!TTarget> 
          IntAsNullable<valuetype .ctor ([mscorlib]System.ValueType) TTarget>(int32 source) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  box        [mscorlib]System.Int32
    IL_0006:  isinst     valuetype [mscorlib]System.Nullable`1<!!TTarget>
    IL_000b:  unbox.any  valuetype [mscorlib]System.Nullable`1<!!TTarget>
    IL_0010:  ret
  } // end of method Generics::IntAsNullable

  .method public hidebysig instance !!T  New<.ctor T>() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  call       !!0 [mscorlib]System.Activator::CreateInstance<!!0>()
    IL_0005:  ret
  } // end of method Generics::New

  .method public hidebysig instance !!T  NotNew<T>() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  call       !!0 [mscorlib]System.Activator::CreateInstance<!!0>()
    IL_0005:  ret
  } // end of method Generics::NotNew

  .method public hidebysig instance bool 
          IsNull<T>(!!T t) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  box        !!T
    IL_0006:  ldnull
    IL_0007:  ceq
    IL_0009:  ret
  } // end of method Generics::IsNull

  .method public hidebysig instance !!T[] 
          NewArray<T>(int32 size) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  newarr     !!T
    IL_0006:  ret
  } // end of method Generics::NewArray

  .method public hidebysig instance !!T[0...,0...] 
          NewArray<T>(int32 size1,
                      int32 size2) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ldarg.2
    IL_0002:  newobj     instance void !!T[0...,0...]::.ctor(int32,
                                                             int32)
    IL_0007:  ret
  } // end of method Generics::NewArray

  .method public hidebysig instance class [mscorlib]System.Type[] 
          TestTypeOf() cil managed
  {
    // Code size       111 (0x6f)
    .maxstack  4
    IL_0000:  ldc.i4.8
    IL_0001:  newarr     [mscorlib]System.Type
    IL_0006:  dup
    IL_0007:  ldc.i4.0
    IL_0008:  ldtoken    [mscorlib]System.Int32
    IL_000d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0012:  stelem.ref
    IL_0013:  dup
    IL_0014:  ldc.i4.1
    IL_0015:  ldtoken    int32[]
    IL_001a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001f:  stelem.ref
    IL_0020:  dup
    IL_0021:  ldc.i4.2
    IL_0022:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/GenericClass`1
    IL_0027:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002c:  stelem.ref
    IL_002d:  dup
    IL_002e:  ldc.i4.3
    IL_002f:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/GenericClass`1<int32>
    IL_0034:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0039:  stelem.ref
    IL_003a:  dup
    IL_003b:  ldc.i4.4
    IL_003c:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/GenericClass`1<int32[]>
    IL_0041:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0046:  stelem.ref
    IL_0047:  dup
    IL_0048:  ldc.i4.5
    IL_0049:  ldtoken    [mscorlib]System.Collections.Generic.Dictionary`2
    IL_004e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0053:  stelem.ref
    IL_0054:  dup
    IL_0055:  ldc.i4.6
    IL_0056:  ldtoken    valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>
    IL_005b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0060:  stelem.ref
    IL_0061:  dup
    IL_0062:  ldc.i4.7
    IL_0063:  ldtoken    [mscorlib]System.Collections.Generic.List`1/Enumerator
    IL_0068:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006d:  stelem.ref
    IL_006e:  ret
  } // end of method Generics::TestTypeOf

  .method public hidebysig static void  MethodWithConstraint<class (!!S) T,.ctor ([mscorlib]System.ICloneable) S>() cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method Generics::MethodWithConstraint

  .method public hidebysig static void  MethodWithStructConstraint<valuetype .ctor ([mscorlib]System.ValueType) T>() cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method Generics::MethodWithStructConstraint

  .method private hidebysig static void  MultidimensionalArray<T>(!!T[0...,0...] 'array') cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  ldc.i4.0
    IL_0003:  ldarg.0
    IL_0004:  ldc.i4.0
    IL_0005:  ldc.i4.1
    IL_0006:  call       instance !!T !!T[0...,0...]::Get(int32,
                                                          int32)
    IL_000b:  call       instance void !!T[0...,0...]::Set(int32,
                                                           int32,
                                                           !!T)
    IL_0010:  ret
  } // end of method Generics::MultidimensionalArray

  .method public hidebysig static valuetype [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<string,string> 
          GetEnumerator(class [mscorlib]System.Collections.Generic.Dictionary`2<string,string> d,
                        class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/MyArray`1/NestedClass`1<string,int32> nc) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  callvirt   instance class [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection<!0,!1> class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Keys()
    IL_0006:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<!0,!1> class [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection<string,string>::GetEnumerator()
    IL_000b:  ret
  } // end of method Generics::GetEnumerator

  .method public hidebysig static string 
          AsString<T>(!!T input) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  box        !!T
    IL_0006:  isinst     [mscorlib]System.String
    IL_000b:  ret
  } // end of method Generics::AsString

  .method public hidebysig static string 
          CastToString<T>(!!T input) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  box        !!T
    IL_0006:  castclass  [mscorlib]System.String
    IL_000b:  ret
  } // end of method Generics::CastToString

  .method public hidebysig static !!T  CastFromString<T>(string input) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  unbox.any  !!T
    IL_0006:  ret
  } // end of method Generics::CastFromString

  .method public hidebysig static int32  CastToInt<T>(!!T input) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  box        !!T
    IL_0006:  unbox.any  [mscorlib]System.Int32
    IL_000b:  ret
  } // end of method Generics::CastToInt

  .method public hidebysig static !!T  CastFromInt<T>(int32 input) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  box        [mscorlib]System.Int32
    IL_0006:  unbox.any  !!T
    IL_000b:  ret
  } // end of method Generics::CastFromInt

  .method public hidebysig static valuetype [mscorlib]System.Nullable`1<int32> 
          AsNullableInt<T>(!!T input) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  box        !!T
    IL_0006:  isinst     valuetype [mscorlib]System.Nullable`1<int32>
    IL_000b:  unbox.any  valuetype [mscorlib]System.Nullable`1<int32>
    IL_0010:  ret
  } // end of method Generics::AsNullableInt

  .method public hidebysig static valuetype [mscorlib]System.Nullable`1<int32> 
          CastToNullableInt<T>(!!T input) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  box        !!T
    IL_0006:  unbox.any  valuetype [mscorlib]System.Nullable`1<int32>
    IL_000b:  ret
  } // end of method Generics::CastToNullableInt

  .method public hidebysig static !!T  CastFromNullableInt<T>(valuetype [mscorlib]System.Nullable`1<int32> input) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0006:  unbox.any  !!T
    IL_000b:  ret
  } // end of method Generics::CastFromNullableInt

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Generics::.ctor

  .method private hidebysig specialname rtspecialname static 
          void  .cctor() cil managed
  {
    // Code size       91 (0x5b)
    .maxstack  1
    IL_0000:  ldtoken    [mscorlib]System.Collections.Generic.List`1
    IL_0005:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000a:  stsfld     class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics::type1
    IL_000f:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/MyArray`1
    IL_0014:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0019:  stsfld     class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics::type2
    IL_001e:  ldtoken    [mscorlib]System.Collections.Generic.List`1/Enumerator
    IL_0023:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0028:  stsfld     class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics::type3
    IL_002d:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/MyArray`1/NestedClass`1
    IL_0032:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0037:  stsfld     class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics::type4
    IL_003c:  ldtoken    class [mscorlib]System.Collections.Generic.List`1<int32>[]
    IL_0041:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0046:  stsfld     class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics::type5
    IL_004b:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/MyArray`1/NestedEnum
    IL_0050:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0055:  stsfld     class [mscorlib]System.Type ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics::type6
    IL_005a:  ret
  } // end of method Generics::.cctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
