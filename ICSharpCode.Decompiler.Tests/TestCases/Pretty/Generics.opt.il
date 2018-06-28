



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly Generics.opt
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module Generics.opt.dll
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
    // Code size       32 (0x20)
    .maxstack  1
    .locals init (!!T V_0,
             !!T V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  initobj    !!T
    IL_0008:  ldloc.0
    IL_0009:  box        !!T
    IL_000e:  brfalse.s  IL_001a

    IL_0010:  ldloca.s   V_1
    IL_0012:  initobj    !!T
    IL_0018:  ldloc.1
    IL_0019:  ret

    IL_001a:  call       !!0 [mscorlib]System.Activator::CreateInstance<!!0>()
    IL_001f:  ret
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

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Generics::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Generics


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
