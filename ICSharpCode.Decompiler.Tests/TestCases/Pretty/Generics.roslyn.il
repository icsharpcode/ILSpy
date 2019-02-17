



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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

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
      // Code size       5 (0x5)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ldarg.0
      IL_0003:  stind.ref
      IL_0004:  ret
    } // end of method GenericClass`1::M

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method GenericClass`1::.ctor

  } // end of class GenericClass`1

  .class auto ansi nested public beforefieldinit BaseClass
         extends [mscorlib]System.Object
  {
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method BaseClass::.ctor

  } // end of class BaseClass

  .class auto ansi nested public beforefieldinit DerivedClass
         extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/BaseClass
  {
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/BaseClass::.ctor()
      IL_0006:  nop
      IL_0007:  ret
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
        // Code size       8 (0x8)
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
        IL_0006:  nop
        IL_0007:  ret
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
      // Code size       21 (0x15)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  nop
      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  newarr     !T
      IL_000f:  stfld      !0[] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/MyArray`1<!T>::arr
      IL_0014:  ret
    } // end of method MyArray`1::.ctor

    .method public hidebysig instance void 
            Size(int32 capacity) cil managed
    {
      // Code size       15 (0xf)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldflda     !0[] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/MyArray`1<!T>::arr
      IL_0007:  ldarg.1
      IL_0008:  call       void [mscorlib]System.Array::Resize<!T>(!!0[]&,
                                                                   int32)
      IL_000d:  nop
      IL_000e:  ret
    } // end of method MyArray`1::Size

    .method public hidebysig instance void 
            Grow(int32 capacity) cil managed
    {
      // Code size       30 (0x1e)
      .maxstack  2
      .locals init (bool V_0)
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  ldarg.0
      IL_0003:  ldfld      !0[] class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/MyArray`1<!T>::arr
      IL_0008:  ldlen
      IL_0009:  conv.i4
      IL_000a:  clt
      IL_000c:  ldc.i4.0
      IL_000d:  ceq
      IL_000f:  stloc.0
      IL_0010:  ldloc.0
      IL_0011:  brfalse.s  IL_001d

      IL_0013:  nop
      IL_0014:  ldarg.0
      IL_0015:  ldarg.1
      IL_0016:  call       instance void class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/MyArray`1<!T>::Size(int32)
      IL_001b:  nop
      IL_001c:  nop
      IL_001d:  ret
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
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method Base::ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics.IInterface.Method2

    .method family hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method Base::.ctor

  } // end of class Base

  .class auto ansi nested public beforefieldinit Derived
         extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/Base
  {
    .method public hidebysig virtual instance void 
            Method1<class T>() cil managed
    {
      // Code size       2 (0x2)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ret
    } // end of method Derived::Method1

    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/Base::.ctor()
      IL_0006:  nop
      IL_0007:  ret
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
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (!!T V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  unbox.any  !!T
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method Generics::CastToTypeParameter

  .method public hidebysig instance !!TTarget 
          GenericAsGeneric<TSource,class TTarget>(!!TSource source) cil managed
  {
    // Code size       22 (0x16)
    .maxstack  1
    .locals init (!!TTarget V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  box        !!TSource
    IL_0007:  isinst     !!TTarget
    IL_000c:  unbox.any  !!TTarget
    IL_0011:  stloc.0
    IL_0012:  br.s       IL_0014

    IL_0014:  ldloc.0
    IL_0015:  ret
  } // end of method Generics::GenericAsGeneric

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<!!TTarget> 
          GenericAsNullable<TSource,valuetype .ctor ([mscorlib]System.ValueType) TTarget>(!!TSource source) cil managed
  {
    // Code size       22 (0x16)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<!!TTarget> V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  box        !!TSource
    IL_0007:  isinst     valuetype [mscorlib]System.Nullable`1<!!TTarget>
    IL_000c:  unbox.any  valuetype [mscorlib]System.Nullable`1<!!TTarget>
    IL_0011:  stloc.0
    IL_0012:  br.s       IL_0014

    IL_0014:  ldloc.0
    IL_0015:  ret
  } // end of method Generics::GenericAsNullable

  .method public hidebysig instance !!TTarget 
          ObjectAsGeneric<class TTarget>(object source) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  1
    .locals init (!!TTarget V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  isinst     !!TTarget
    IL_0007:  unbox.any  !!TTarget
    IL_000c:  stloc.0
    IL_000d:  br.s       IL_000f

    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method Generics::ObjectAsGeneric

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<!!TTarget> 
          ObjectAsNullable<valuetype .ctor ([mscorlib]System.ValueType) TTarget>(object source) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<!!TTarget> V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  isinst     valuetype [mscorlib]System.Nullable`1<!!TTarget>
    IL_0007:  unbox.any  valuetype [mscorlib]System.Nullable`1<!!TTarget>
    IL_000c:  stloc.0
    IL_000d:  br.s       IL_000f

    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method Generics::ObjectAsNullable

  .method public hidebysig instance !!TTarget 
          IntAsGeneric<class TTarget>(int32 source) cil managed
  {
    // Code size       22 (0x16)
    .maxstack  1
    .locals init (!!TTarget V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  box        [mscorlib]System.Int32
    IL_0007:  isinst     !!TTarget
    IL_000c:  unbox.any  !!TTarget
    IL_0011:  stloc.0
    IL_0012:  br.s       IL_0014

    IL_0014:  ldloc.0
    IL_0015:  ret
  } // end of method Generics::IntAsGeneric

  .method public hidebysig instance valuetype [mscorlib]System.Nullable`1<!!TTarget> 
          IntAsNullable<valuetype .ctor ([mscorlib]System.ValueType) TTarget>(int32 source) cil managed
  {
    // Code size       22 (0x16)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<!!TTarget> V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  box        [mscorlib]System.Int32
    IL_0007:  isinst     valuetype [mscorlib]System.Nullable`1<!!TTarget>
    IL_000c:  unbox.any  valuetype [mscorlib]System.Nullable`1<!!TTarget>
    IL_0011:  stloc.0
    IL_0012:  br.s       IL_0014

    IL_0014:  ldloc.0
    IL_0015:  ret
  } // end of method Generics::IntAsNullable

  .method public hidebysig instance !!T  New<.ctor T>() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!!T V_0)
    IL_0000:  nop
    IL_0001:  call       !!0 [mscorlib]System.Activator::CreateInstance<!!0>()
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method Generics::New

  .method public hidebysig instance !!T  NotNew<T>() cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (!!T V_0)
    IL_0000:  nop
    IL_0001:  call       !!0 [mscorlib]System.Activator::CreateInstance<!!0>()
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method Generics::NotNew

  .method public hidebysig instance bool 
          IsNull<T>(!!T t) cil managed
  {
    // Code size       15 (0xf)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  box        !!T
    IL_0007:  ldnull
    IL_0008:  ceq
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_000d

    IL_000d:  ldloc.0
    IL_000e:  ret
  } // end of method Generics::IsNull

  .method public hidebysig instance !!T[] 
          NewArray<T>(int32 size) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (!!T[] V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  newarr     !!T
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method Generics::NewArray

  .method public hidebysig instance !!T[0...,0...] 
          NewArray<T>(int32 size1,
                      int32 size2) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  2
    .locals init (!!T[0...,0...] V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  ldarg.2
    IL_0003:  newobj     instance void !!T[0...,0...]::.ctor(int32,
                                                             int32)
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method Generics::NewArray

  .method public hidebysig instance class [mscorlib]System.Type[] 
          TestTypeOf() cil managed
  {
    // Code size       116 (0x74)
    .maxstack  4
    .locals init (class [mscorlib]System.Type[] V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.8
    IL_0002:  newarr     [mscorlib]System.Type
    IL_0007:  dup
    IL_0008:  ldc.i4.0
    IL_0009:  ldtoken    [mscorlib]System.Int32
    IL_000e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0013:  stelem.ref
    IL_0014:  dup
    IL_0015:  ldc.i4.1
    IL_0016:  ldtoken    int32[]
    IL_001b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0020:  stelem.ref
    IL_0021:  dup
    IL_0022:  ldc.i4.2
    IL_0023:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/GenericClass`1
    IL_0028:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_002d:  stelem.ref
    IL_002e:  dup
    IL_002f:  ldc.i4.3
    IL_0030:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/GenericClass`1<int32>
    IL_0035:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_003a:  stelem.ref
    IL_003b:  dup
    IL_003c:  ldc.i4.4
    IL_003d:  ldtoken    class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/GenericClass`1<int32[]>
    IL_0042:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0047:  stelem.ref
    IL_0048:  dup
    IL_0049:  ldc.i4.5
    IL_004a:  ldtoken    [mscorlib]System.Collections.Generic.Dictionary`2
    IL_004f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0054:  stelem.ref
    IL_0055:  dup
    IL_0056:  ldc.i4.6
    IL_0057:  ldtoken    valuetype [mscorlib]System.Collections.Generic.List`1/Enumerator<int32>
    IL_005c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0061:  stelem.ref
    IL_0062:  dup
    IL_0063:  ldc.i4.7
    IL_0064:  ldtoken    [mscorlib]System.Collections.Generic.List`1/Enumerator
    IL_0069:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_006e:  stelem.ref
    IL_006f:  stloc.0
    IL_0070:  br.s       IL_0072

    IL_0072:  ldloc.0
    IL_0073:  ret
  } // end of method Generics::TestTypeOf

  .method public hidebysig static void  MethodWithConstraint<class (!!S) T,.ctor ([mscorlib]System.ICloneable) S>() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method Generics::MethodWithConstraint

  .method public hidebysig static void  MethodWithStructConstraint<valuetype .ctor ([mscorlib]System.ValueType) T>() cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method Generics::MethodWithStructConstraint

  .method private hidebysig static void  MultidimensionalArray<T>(!!T[0...,0...] 'array') cil managed
  {
    // Code size       18 (0x12)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  ldc.i4.0
    IL_0004:  ldarg.0
    IL_0005:  ldc.i4.0
    IL_0006:  ldc.i4.1
    IL_0007:  call       instance !!T !!T[0...,0...]::Get(int32,
                                                          int32)
    IL_000c:  call       instance void !!T[0...,0...]::Set(int32,
                                                           int32,
                                                           !!T)
    IL_0011:  ret
  } // end of method Generics::MultidimensionalArray

  .method public hidebysig static valuetype [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<string,string> 
          GetEnumerator(class [mscorlib]System.Collections.Generic.Dictionary`2<string,string> d,
                        class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics/MyArray`1/NestedClass`1<string,int32> nc) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<string,string> V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  callvirt   instance class [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection<!0,!1> class [mscorlib]System.Collections.Generic.Dictionary`2<string,string>::get_Keys()
    IL_0007:  callvirt   instance valuetype [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection/Enumerator<!0,!1> class [mscorlib]System.Collections.Generic.Dictionary`2/KeyCollection<string,string>::GetEnumerator()
    IL_000c:  stloc.0
    IL_000d:  br.s       IL_000f

    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method Generics::GetEnumerator

  .method public hidebysig static bool  IsString<T>(!!T input) cil managed
  {
    // Code size       20 (0x14)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  box        !!T
    IL_0007:  isinst     [mscorlib]System.String
    IL_000c:  ldnull
    IL_000d:  cgt.un
    IL_000f:  stloc.0
    IL_0010:  br.s       IL_0012

    IL_0012:  ldloc.0
    IL_0013:  ret
  } // end of method Generics::IsString

  .method public hidebysig static string 
          AsString<T>(!!T input) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  box        !!T
    IL_0007:  isinst     [mscorlib]System.String
    IL_000c:  stloc.0
    IL_000d:  br.s       IL_000f

    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method Generics::AsString

  .method public hidebysig static string 
          CastToString<T>(!!T input) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  box        !!T
    IL_0007:  castclass  [mscorlib]System.String
    IL_000c:  stloc.0
    IL_000d:  br.s       IL_000f

    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method Generics::CastToString

  .method public hidebysig static !!T  CastFromString<T>(string input) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (!!T V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  unbox.any  !!T
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method Generics::CastFromString

  .method public hidebysig static bool  IsInt<T>(!!T input) cil managed
  {
    // Code size       20 (0x14)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  box        !!T
    IL_0007:  isinst     [mscorlib]System.Int32
    IL_000c:  ldnull
    IL_000d:  cgt.un
    IL_000f:  stloc.0
    IL_0010:  br.s       IL_0012

    IL_0012:  ldloc.0
    IL_0013:  ret
  } // end of method Generics::IsInt

  .method public hidebysig static int32  CastToInt<T>(!!T input) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  box        !!T
    IL_0007:  unbox.any  [mscorlib]System.Int32
    IL_000c:  stloc.0
    IL_000d:  br.s       IL_000f

    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method Generics::CastToInt

  .method public hidebysig static !!T  CastFromInt<T>(int32 input) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  1
    .locals init (!!T V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  box        [mscorlib]System.Int32
    IL_0007:  unbox.any  !!T
    IL_000c:  stloc.0
    IL_000d:  br.s       IL_000f

    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method Generics::CastFromInt

  .method public hidebysig static bool  IsNullableInt<T>(!!T input) cil managed
  {
    // Code size       20 (0x14)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  box        !!T
    IL_0007:  isinst     valuetype [mscorlib]System.Nullable`1<int32>
    IL_000c:  ldnull
    IL_000d:  cgt.un
    IL_000f:  stloc.0
    IL_0010:  br.s       IL_0012

    IL_0012:  ldloc.0
    IL_0013:  ret
  } // end of method Generics::IsNullableInt

  .method public hidebysig static valuetype [mscorlib]System.Nullable`1<int32> 
          AsNullableInt<T>(!!T input) cil managed
  {
    // Code size       22 (0x16)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  box        !!T
    IL_0007:  isinst     valuetype [mscorlib]System.Nullable`1<int32>
    IL_000c:  unbox.any  valuetype [mscorlib]System.Nullable`1<int32>
    IL_0011:  stloc.0
    IL_0012:  br.s       IL_0014

    IL_0014:  ldloc.0
    IL_0015:  ret
  } // end of method Generics::AsNullableInt

  .method public hidebysig static valuetype [mscorlib]System.Nullable`1<int32> 
          CastToNullableInt<T>(!!T input) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<int32> V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  box        !!T
    IL_0007:  unbox.any  valuetype [mscorlib]System.Nullable`1<int32>
    IL_000c:  stloc.0
    IL_000d:  br.s       IL_000f

    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method Generics::CastToNullableInt

  .method public hidebysig static !!T  CastFromNullableInt<T>(valuetype [mscorlib]System.Nullable`1<int32> input) cil managed
  {
    // Code size       17 (0x11)
    .maxstack  1
    .locals init (!!T V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  box        valuetype [mscorlib]System.Nullable`1<int32>
    IL_0007:  unbox.any  !!T
    IL_000c:  stloc.0
    IL_000d:  br.s       IL_000f

    IL_000f:  ldloc.0
    IL_0010:  ret
  } // end of method Generics::CastFromNullableInt

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
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
