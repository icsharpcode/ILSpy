



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
.assembly extern Microsoft.CSharp
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 4:0:0:0
}
.assembly CustomShortCircuitOperators.opt
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module CustomShortCircuitOperators.opt.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass
       extends [mscorlib]System.Object
{
  .method public hidebysig specialname static 
          bool  op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass x) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  ret
  } // end of method BaseClass::op_True

  .method public hidebysig specialname static 
          bool  op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass x) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldc.i4.0
    IL_0001:  ret
  } // end of method BaseClass::op_False

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method BaseClass::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass
{
  .class abstract auto ansi sealed nested private beforefieldinit '<WithDynamic>o__SiteContainer0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> '<>p__Site1'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> '<>p__Site2'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> '<>p__Site3'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> '<>p__Site4'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> '<>p__Site5'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> '<>p__Site6'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> '<>p__Site7'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> '<>p__Site8'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> '<>p__Site9'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> '<>p__Sitea'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> '<>p__Siteb'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> '<>p__Sitec'
  } // end of class '<WithDynamic>o__SiteContainer0'

  .method public hidebysig specialname static 
          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C 
          op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C x,
                        class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C y) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ret
  } // end of method C::op_BitwiseAnd

  .method public hidebysig specialname static 
          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C 
          op_BitwiseOr(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C x,
                       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C y) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldnull
    IL_0001:  ret
  } // end of method C::op_BitwiseOr

  .method public hidebysig specialname static 
          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C 
          op_LogicalNot(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C x) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ret
  } // end of method C::op_LogicalNot

  .method public hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C 
          GetC(int32 a) cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::.ctor()
    IL_0005:  ret
  } // end of method C::GetC

  .method public hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C 
          LogicAnd() cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0006:  dup
    IL_0007:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_000c:  brtrue.s   IL_0019

    IL_000e:  ldc.i4.2
    IL_000f:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0014:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0019:  ret
  } // end of method C::LogicAnd

  .method public hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C 
          LogicOr() cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0006:  dup
    IL_0007:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_000c:  brtrue.s   IL_0019

    IL_000e:  ldc.i4.2
    IL_000f:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0014:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseOr(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0019:  ret
  } // end of method C::LogicOr

  .method public hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C 
          Complex() cil managed
  {
    // Code size       88 (0x58)
    .maxstack  3
    IL_0000:  ldc.i4.1
    IL_0001:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0006:  dup
    IL_0007:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_000c:  brtrue.s   IL_0019

    IL_000e:  ldc.i4.2
    IL_000f:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0014:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseOr(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0019:  dup
    IL_001a:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_001f:  brtrue.s   IL_002c

    IL_0021:  ldc.i4.3
    IL_0022:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0027:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_002c:  dup
    IL_002d:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0032:  brtrue.s   IL_0057

    IL_0034:  ldc.i4.4
    IL_0035:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_003a:  dup
    IL_003b:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0040:  brtrue.s   IL_004d

    IL_0042:  ldc.i4.5
    IL_0043:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0048:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseOr(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_004d:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_LogicalNot(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0052:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0057:  ret
  } // end of method C::Complex

  .method private hidebysig static void  Main() cil managed
  {
    // Code size       67 (0x43)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_2,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_3)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::.ctor()
    IL_0005:  stloc.0
    IL_0006:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::.ctor()
    IL_000b:  stloc.1
    IL_000c:  ldloc.0
    IL_000d:  dup
    IL_000e:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0013:  brtrue.s   IL_001b

    IL_0015:  ldloc.1
    IL_0016:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_001b:  stloc.2
    IL_001c:  ldloc.0
    IL_001d:  dup
    IL_001e:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0023:  brtrue.s   IL_002b

    IL_0025:  ldloc.1
    IL_0026:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseOr(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_002b:  stloc.3
    IL_002c:  ldloc.2
    IL_002d:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0032:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0037:  ldloc.3
    IL_0038:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_003d:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0042:  ret
  } // end of method C::Main

  .method private hidebysig static void  Test2() cil managed
  {
    // Code size       135 (0x87)
    .maxstack  2
    IL_0000:  ldc.i4.1
    IL_0001:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0006:  dup
    IL_0007:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_000c:  brtrue.s   IL_0019

    IL_000e:  ldc.i4.2
    IL_000f:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0014:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0019:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_001e:  brfalse.s  IL_002b

    IL_0020:  ldc.i4.3
    IL_0021:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0026:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_002b:  ldc.i4.1
    IL_002c:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0031:  dup
    IL_0032:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0037:  brtrue.s   IL_0044

    IL_0039:  ldc.i4.2
    IL_003a:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_003f:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseOr(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0044:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0049:  brfalse.s  IL_0056

    IL_004b:  ldc.i4.3
    IL_004c:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0051:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0056:  ldc.i4.1
    IL_0057:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_005c:  dup
    IL_005d:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0062:  brtrue.s   IL_006f

    IL_0064:  ldc.i4.2
    IL_0065:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_006a:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_006f:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_LogicalNot(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0074:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0079:  brfalse.s  IL_0086

    IL_007b:  ldc.i4.3
    IL_007c:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0081:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0086:  ret
  } // end of method C::Test2

  .method private hidebysig static void  Test3() cil managed
  {
    // Code size       50 (0x32)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_000c:  brfalse.s  IL_0019

    IL_000e:  ldloc.0
    IL_000f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0014:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0019:  ldloc.0
    IL_001a:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_LogicalNot(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_001f:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0024:  brfalse.s  IL_0031

    IL_0026:  ldloc.0
    IL_0027:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_002c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0031:  ret
  } // end of method C::Test3

  .method public hidebysig instance void 
          WithDynamic(object d) cil managed
  {
    .param [1]
    .custom instance void [System.Core]System.Runtime.CompilerServices.DynamicAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1146 (0x47a)
    .maxstack  13
    .locals init (class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_1,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_2,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_3,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_4,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_5,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_6,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_7,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_8,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_9,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_10,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_11,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_12,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_13,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_14,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_15)
    IL_0000:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site1'
    IL_0005:  brtrue.s   IL_0048

    IL_0007:  ldc.i4     0x100
    IL_000c:  ldstr      "WriteLine"
    IL_0011:  ldnull
    IL_0012:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_0017:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001c:  ldc.i4.2
    IL_001d:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0022:  stloc.0
    IL_0023:  ldloc.0
    IL_0024:  ldc.i4.0
    IL_0025:  ldc.i4.s   33
    IL_0027:  ldnull
    IL_0028:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_002d:  stelem.ref
    IL_002e:  ldloc.0
    IL_002f:  ldc.i4.1
    IL_0030:  ldc.i4.0
    IL_0031:  ldnull
    IL_0032:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0037:  stelem.ref
    IL_0038:  ldloc.0
    IL_0039:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::InvokeMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                 string,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Type>,
                                                                                                                                                                 class [mscorlib]System.Type,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_003e:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0043:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site1'
    IL_0048:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site1'
    IL_004d:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Target
    IL_0052:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site1'
    IL_0057:  ldtoken    [mscorlib]System.Console
    IL_005c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0061:  ldc.i4.1
    IL_0062:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0067:  stloc.1
    IL_0068:  ldloc.1
    IL_0069:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_006e:  brtrue     IL_0115

    IL_0073:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site2'
    IL_0078:  brtrue.s   IL_00b1

    IL_007a:  ldc.i4.8
    IL_007b:  ldc.i4.2
    IL_007c:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_0081:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0086:  ldc.i4.2
    IL_0087:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_008c:  stloc.2
    IL_008d:  ldloc.2
    IL_008e:  ldc.i4.0
    IL_008f:  ldc.i4.1
    IL_0090:  ldnull
    IL_0091:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0096:  stelem.ref
    IL_0097:  ldloc.2
    IL_0098:  ldc.i4.1
    IL_0099:  ldc.i4.0
    IL_009a:  ldnull
    IL_009b:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_00a0:  stelem.ref
    IL_00a1:  ldloc.2
    IL_00a2:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_00a7:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_00ac:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site2'
    IL_00b1:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site2'
    IL_00b6:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Target
    IL_00bb:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site2'
    IL_00c0:  ldloc.1
    IL_00c1:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site3'
    IL_00c6:  brtrue.s   IL_00f9

    IL_00c8:  ldc.i4.0
    IL_00c9:  ldstr      "P"
    IL_00ce:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_00d3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d8:  ldc.i4.1
    IL_00d9:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_00de:  stloc.3
    IL_00df:  ldloc.3
    IL_00e0:  ldc.i4.0
    IL_00e1:  ldc.i4.0
    IL_00e2:  ldnull
    IL_00e3:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_00e8:  stelem.ref
    IL_00e9:  ldloc.3
    IL_00ea:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_00ef:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_00f4:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site3'
    IL_00f9:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site3'
    IL_00fe:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_0103:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site3'
    IL_0108:  ldarg.1
    IL_0109:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_010e:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>::Invoke(!0,
                                                                                                                                                                                                                                        !1,
                                                                                                                                                                                                                                        !2)
    IL_0113:  br.s       IL_0116

    IL_0115:  ldloc.1
    IL_0116:  callvirt   instance void class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>::Invoke(!0,
                                                                                                                                                                               !1,
                                                                                                                                                                               !2)
    IL_011b:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site4'
    IL_0120:  brtrue.s   IL_0167

    IL_0122:  ldc.i4     0x100
    IL_0127:  ldstr      "WriteLine"
    IL_012c:  ldnull
    IL_012d:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_0132:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0137:  ldc.i4.2
    IL_0138:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_013d:  stloc.s    V_4
    IL_013f:  ldloc.s    V_4
    IL_0141:  ldc.i4.0
    IL_0142:  ldc.i4.s   33
    IL_0144:  ldnull
    IL_0145:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_014a:  stelem.ref
    IL_014b:  ldloc.s    V_4
    IL_014d:  ldc.i4.1
    IL_014e:  ldc.i4.0
    IL_014f:  ldnull
    IL_0150:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0155:  stelem.ref
    IL_0156:  ldloc.s    V_4
    IL_0158:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::InvokeMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                 string,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Type>,
                                                                                                                                                                 class [mscorlib]System.Type,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_015d:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0162:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site4'
    IL_0167:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site4'
    IL_016c:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Target
    IL_0171:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site4'
    IL_0176:  ldtoken    [mscorlib]System.Console
    IL_017b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0180:  ldc.i4.2
    IL_0181:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0186:  stloc.s    V_5
    IL_0188:  ldloc.s    V_5
    IL_018a:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_018f:  brtrue     IL_023f

    IL_0194:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site5'
    IL_0199:  brtrue.s   IL_01d7

    IL_019b:  ldc.i4.8
    IL_019c:  ldc.i4.s   36
    IL_019e:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_01a3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01a8:  ldc.i4.2
    IL_01a9:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_01ae:  stloc.s    V_6
    IL_01b0:  ldloc.s    V_6
    IL_01b2:  ldc.i4.0
    IL_01b3:  ldc.i4.1
    IL_01b4:  ldnull
    IL_01b5:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_01ba:  stelem.ref
    IL_01bb:  ldloc.s    V_6
    IL_01bd:  ldc.i4.1
    IL_01be:  ldc.i4.0
    IL_01bf:  ldnull
    IL_01c0:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_01c5:  stelem.ref
    IL_01c6:  ldloc.s    V_6
    IL_01c8:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_01cd:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_01d2:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site5'
    IL_01d7:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site5'
    IL_01dc:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Target
    IL_01e1:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site5'
    IL_01e6:  ldloc.s    V_5
    IL_01e8:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site6'
    IL_01ed:  brtrue.s   IL_0223

    IL_01ef:  ldc.i4.0
    IL_01f0:  ldstr      "P"
    IL_01f5:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_01fa:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ff:  ldc.i4.1
    IL_0200:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0205:  stloc.s    V_7
    IL_0207:  ldloc.s    V_7
    IL_0209:  ldc.i4.0
    IL_020a:  ldc.i4.0
    IL_020b:  ldnull
    IL_020c:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0211:  stelem.ref
    IL_0212:  ldloc.s    V_7
    IL_0214:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0219:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_021e:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site6'
    IL_0223:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site6'
    IL_0228:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_022d:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site6'
    IL_0232:  ldarg.1
    IL_0233:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_0238:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>::Invoke(!0,
                                                                                                                                                                                                                                        !1,
                                                                                                                                                                                                                                        !2)
    IL_023d:  br.s       IL_0241

    IL_023f:  ldloc.s    V_5
    IL_0241:  callvirt   instance void class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>::Invoke(!0,
                                                                                                                                                                               !1,
                                                                                                                                                                               !2)
    IL_0246:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site7'
    IL_024b:  brtrue.s   IL_027e

    IL_024d:  ldc.i4.0
    IL_024e:  ldc.i4.s   83
    IL_0250:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_0255:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_025a:  ldc.i4.1
    IL_025b:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0260:  stloc.s    V_8
    IL_0262:  ldloc.s    V_8
    IL_0264:  ldc.i4.0
    IL_0265:  ldc.i4.0
    IL_0266:  ldnull
    IL_0267:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_026c:  stelem.ref
    IL_026d:  ldloc.s    V_8
    IL_026f:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::UnaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                   valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                   class [mscorlib]System.Type,
                                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0274:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0279:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site7'
    IL_027e:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site7'
    IL_0283:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Target
    IL_0288:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site7'
    IL_028d:  ldc.i4.3
    IL_028e:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0293:  stloc.s    V_9
    IL_0295:  ldloc.s    V_9
    IL_0297:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_029c:  brtrue     IL_034b

    IL_02a1:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site8'
    IL_02a6:  brtrue.s   IL_02e3

    IL_02a8:  ldc.i4.8
    IL_02a9:  ldc.i4.2
    IL_02aa:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_02af:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02b4:  ldc.i4.2
    IL_02b5:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_02ba:  stloc.s    V_10
    IL_02bc:  ldloc.s    V_10
    IL_02be:  ldc.i4.0
    IL_02bf:  ldc.i4.1
    IL_02c0:  ldnull
    IL_02c1:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_02c6:  stelem.ref
    IL_02c7:  ldloc.s    V_10
    IL_02c9:  ldc.i4.1
    IL_02ca:  ldc.i4.0
    IL_02cb:  ldnull
    IL_02cc:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_02d1:  stelem.ref
    IL_02d2:  ldloc.s    V_10
    IL_02d4:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_02d9:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_02de:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site8'
    IL_02e3:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site8'
    IL_02e8:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Target
    IL_02ed:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site8'
    IL_02f2:  ldloc.s    V_9
    IL_02f4:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site9'
    IL_02f9:  brtrue.s   IL_032f

    IL_02fb:  ldc.i4.0
    IL_02fc:  ldstr      "P"
    IL_0301:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_0306:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_030b:  ldc.i4.1
    IL_030c:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0311:  stloc.s    V_11
    IL_0313:  ldloc.s    V_11
    IL_0315:  ldc.i4.0
    IL_0316:  ldc.i4.0
    IL_0317:  ldnull
    IL_0318:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_031d:  stelem.ref
    IL_031e:  ldloc.s    V_11
    IL_0320:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0325:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_032a:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site9'
    IL_032f:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site9'
    IL_0334:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_0339:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site9'
    IL_033e:  ldarg.1
    IL_033f:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_0344:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>::Invoke(!0,
                                                                                                                                                                                                                                        !1,
                                                                                                                                                                                                                                        !2)
    IL_0349:  br.s       IL_034d

    IL_034b:  ldloc.s    V_9
    IL_034d:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>::Invoke(!0,
                                                                                                                                                    !1)
    IL_0352:  brfalse.s  IL_035f

    IL_0354:  ldc.i4.4
    IL_0355:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_035a:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_035f:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Sitea'
    IL_0364:  brtrue.s   IL_0397

    IL_0366:  ldc.i4.0
    IL_0367:  ldc.i4.s   83
    IL_0369:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_036e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0373:  ldc.i4.1
    IL_0374:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0379:  stloc.s    V_12
    IL_037b:  ldloc.s    V_12
    IL_037d:  ldc.i4.0
    IL_037e:  ldc.i4.0
    IL_037f:  ldnull
    IL_0380:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0385:  stelem.ref
    IL_0386:  ldloc.s    V_12
    IL_0388:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::UnaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                   valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                   class [mscorlib]System.Type,
                                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_038d:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0392:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Sitea'
    IL_0397:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Sitea'
    IL_039c:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Target
    IL_03a1:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Sitea'
    IL_03a6:  ldc.i4.5
    IL_03a7:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_03ac:  stloc.s    V_13
    IL_03ae:  ldloc.s    V_13
    IL_03b0:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_03b5:  brtrue     IL_0465

    IL_03ba:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Siteb'
    IL_03bf:  brtrue.s   IL_03fd

    IL_03c1:  ldc.i4.8
    IL_03c2:  ldc.i4.s   36
    IL_03c4:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_03c9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03ce:  ldc.i4.2
    IL_03cf:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_03d4:  stloc.s    V_14
    IL_03d6:  ldloc.s    V_14
    IL_03d8:  ldc.i4.0
    IL_03d9:  ldc.i4.1
    IL_03da:  ldnull
    IL_03db:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_03e0:  stelem.ref
    IL_03e1:  ldloc.s    V_14
    IL_03e3:  ldc.i4.1
    IL_03e4:  ldc.i4.0
    IL_03e5:  ldnull
    IL_03e6:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_03eb:  stelem.ref
    IL_03ec:  ldloc.s    V_14
    IL_03ee:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_03f3:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_03f8:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Siteb'
    IL_03fd:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Siteb'
    IL_0402:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Target
    IL_0407:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Siteb'
    IL_040c:  ldloc.s    V_13
    IL_040e:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Sitec'
    IL_0413:  brtrue.s   IL_0449

    IL_0415:  ldc.i4.0
    IL_0416:  ldstr      "P"
    IL_041b:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_0420:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0425:  ldc.i4.1
    IL_0426:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_042b:  stloc.s    V_15
    IL_042d:  ldloc.s    V_15
    IL_042f:  ldc.i4.0
    IL_0430:  ldc.i4.0
    IL_0431:  ldnull
    IL_0432:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0437:  stelem.ref
    IL_0438:  ldloc.s    V_15
    IL_043a:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_043f:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0444:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Sitec'
    IL_0449:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Sitec'
    IL_044e:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_0453:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Sitec'
    IL_0458:  ldarg.1
    IL_0459:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_045e:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>::Invoke(!0,
                                                                                                                                                                                                                                        !1,
                                                                                                                                                                                                                                        !2)
    IL_0463:  br.s       IL_0467

    IL_0465:  ldloc.s    V_13
    IL_0467:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>::Invoke(!0,
                                                                                                                                                    !1)
    IL_046c:  brfalse.s  IL_0479

    IL_046e:  ldc.i4.6
    IL_046f:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0474:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0479:  ret
  } // end of method C::WithDynamic

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::.ctor()
    IL_0006:  ret
  } // end of method C::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C

.class private sequential ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
       extends [mscorlib]System.ValueType
{
  .class abstract auto ansi sealed nested private beforefieldinit '<WithDynamic>o__SiteContainer0'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> '<>p__Site1'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> '<>p__Site2'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> '<>p__Site3'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> '<>p__Site4'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> '<>p__Site5'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> '<>p__Site6'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> '<>p__Site7'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> '<>p__Site8'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> '<>p__Site9'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> '<>p__Sitea'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> '<>p__Siteb'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> '<>p__Sitec'
  } // end of class '<WithDynamic>o__SiteContainer0'

  .field private initonly bool val
  .method public hidebysig specialname rtspecialname 
          instance void  .ctor(bool val) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0007:  ret
  } // end of method S::.ctor

  .method public hidebysig specialname static 
          bool  op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S x) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarga.s   x
    IL_0002:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0007:  ret
  } // end of method S::op_True

  .method public hidebysig specialname static 
          bool  op_False(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S x) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarga.s   x
    IL_0002:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0007:  ret
  } // end of method S::op_False

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S 
          op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S x,
                        valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S y) cil managed
  {
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarga.s   x
    IL_0002:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0007:  ldarga.s   y
    IL_0009:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_000e:  and
    IL_000f:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::.ctor(bool)
    IL_0014:  ret
  } // end of method S::op_BitwiseAnd

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S 
          op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S x,
                       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S y) cil managed
  {
    // Code size       21 (0x15)
    .maxstack  8
    IL_0000:  ldarga.s   x
    IL_0002:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0007:  ldarga.s   y
    IL_0009:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_000e:  or
    IL_000f:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::.ctor(bool)
    IL_0014:  ret
  } // end of method S::op_BitwiseOr

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S 
          op_LogicalNot(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S x) cil managed
  {
    // Code size       16 (0x10)
    .maxstack  8
    IL_0000:  ldarga.s   x
    IL_0002:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0007:  ldc.i4.0
    IL_0008:  ceq
    IL_000a:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::.ctor(bool)
    IL_000f:  ret
  } // end of method S::op_LogicalNot

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S 
          Get(int32 i) cil managed
  {
    // Code size       10 (0xa)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.0
    IL_0002:  cgt
    IL_0004:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::.ctor(bool)
    IL_0009:  ret
  } // end of method S::Get

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S 
          LogicAnd() cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0006:  dup
    IL_0007:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_False(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_000c:  brtrue.s   IL_0019

    IL_000e:  ldc.i4.2
    IL_000f:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0014:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,
                                                                                                                                                                                                          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0019:  ret
  } // end of method S::LogicAnd

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S 
          LogicOr() cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0006:  dup
    IL_0007:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_000c:  brtrue.s   IL_0019

    IL_000e:  ldc.i4.2
    IL_000f:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0014:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,
                                                                                                                                                                                                         valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0019:  ret
  } // end of method S::LogicOr

  .method public hidebysig instance void 
          InConditionDetection() cil managed
  {
    // Code size       118 (0x76)
    .maxstack  2
    IL_0000:  ldstr      "a"
    IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000a:  ldc.i4.1
    IL_000b:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0010:  dup
    IL_0011:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_False(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0016:  brtrue.s   IL_0023

    IL_0018:  ldc.i4.2
    IL_0019:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_001e:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,
                                                                                                                                                                                                          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0023:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0028:  brfalse.s  IL_0036

    IL_002a:  ldstr      "b"
    IL_002f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0034:  br.s       IL_0040

    IL_0036:  ldstr      "c"
    IL_003b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0040:  ldc.i4.1
    IL_0041:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0046:  dup
    IL_0047:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_004c:  brtrue.s   IL_0059

    IL_004e:  ldc.i4.2
    IL_004f:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0054:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,
                                                                                                                                                                                                         valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0059:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_005e:  brfalse.s  IL_006b

    IL_0060:  ldstr      "d"
    IL_0065:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_006a:  ret

    IL_006b:  ldstr      "e"
    IL_0070:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0075:  ret
  } // end of method S::InConditionDetection

  .method public hidebysig instance void 
          WithDynamic(object d) cil managed
  {
    .param [1]
    .custom instance void [System.Core]System.Runtime.CompilerServices.DynamicAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1176 (0x498)
    .maxstack  13
    .locals init (class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_1,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_2,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_3,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_4,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_5,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_6,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_7,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_8,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_9,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_10,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_11,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_12,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_13,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_14,
             class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_15)
    IL_0000:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site1'
    IL_0005:  brtrue.s   IL_0048

    IL_0007:  ldc.i4     0x100
    IL_000c:  ldstr      "WriteLine"
    IL_0011:  ldnull
    IL_0012:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0017:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001c:  ldc.i4.2
    IL_001d:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0022:  stloc.0
    IL_0023:  ldloc.0
    IL_0024:  ldc.i4.0
    IL_0025:  ldc.i4.s   33
    IL_0027:  ldnull
    IL_0028:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_002d:  stelem.ref
    IL_002e:  ldloc.0
    IL_002f:  ldc.i4.1
    IL_0030:  ldc.i4.0
    IL_0031:  ldnull
    IL_0032:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0037:  stelem.ref
    IL_0038:  ldloc.0
    IL_0039:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::InvokeMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                 string,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Type>,
                                                                                                                                                                 class [mscorlib]System.Type,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_003e:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0043:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site1'
    IL_0048:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site1'
    IL_004d:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Target
    IL_0052:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site1'
    IL_0057:  ldtoken    [mscorlib]System.Console
    IL_005c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0061:  ldc.i4.1
    IL_0062:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0067:  stloc.1
    IL_0068:  ldloc.1
    IL_0069:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_False(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_006e:  brtrue     IL_0115

    IL_0073:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site2'
    IL_0078:  brtrue.s   IL_00b1

    IL_007a:  ldc.i4.8
    IL_007b:  ldc.i4.2
    IL_007c:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0081:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0086:  ldc.i4.2
    IL_0087:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_008c:  stloc.2
    IL_008d:  ldloc.2
    IL_008e:  ldc.i4.0
    IL_008f:  ldc.i4.1
    IL_0090:  ldnull
    IL_0091:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0096:  stelem.ref
    IL_0097:  ldloc.2
    IL_0098:  ldc.i4.1
    IL_0099:  ldc.i4.0
    IL_009a:  ldnull
    IL_009b:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_00a0:  stelem.ref
    IL_00a1:  ldloc.2
    IL_00a2:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_00a7:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_00ac:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site2'
    IL_00b1:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site2'
    IL_00b6:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Target
    IL_00bb:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site2'
    IL_00c0:  ldloc.1
    IL_00c1:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site3'
    IL_00c6:  brtrue.s   IL_00f9

    IL_00c8:  ldc.i4.0
    IL_00c9:  ldstr      "P"
    IL_00ce:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_00d3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00d8:  ldc.i4.1
    IL_00d9:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_00de:  stloc.3
    IL_00df:  ldloc.3
    IL_00e0:  ldc.i4.0
    IL_00e1:  ldc.i4.0
    IL_00e2:  ldnull
    IL_00e3:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_00e8:  stelem.ref
    IL_00e9:  ldloc.3
    IL_00ea:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_00ef:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_00f4:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site3'
    IL_00f9:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site3'
    IL_00fe:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_0103:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site3'
    IL_0108:  ldarg.1
    IL_0109:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_010e:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>::Invoke(!0,
                                                                                                                                                                                                                                            !1,
                                                                                                                                                                                                                                            !2)
    IL_0113:  br.s       IL_011b

    IL_0115:  ldloc.1
    IL_0116:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_011b:  callvirt   instance void class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>::Invoke(!0,
                                                                                                                                                                               !1,
                                                                                                                                                                               !2)
    IL_0120:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site4'
    IL_0125:  brtrue.s   IL_016c

    IL_0127:  ldc.i4     0x100
    IL_012c:  ldstr      "WriteLine"
    IL_0131:  ldnull
    IL_0132:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0137:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_013c:  ldc.i4.2
    IL_013d:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0142:  stloc.s    V_4
    IL_0144:  ldloc.s    V_4
    IL_0146:  ldc.i4.0
    IL_0147:  ldc.i4.s   33
    IL_0149:  ldnull
    IL_014a:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_014f:  stelem.ref
    IL_0150:  ldloc.s    V_4
    IL_0152:  ldc.i4.1
    IL_0153:  ldc.i4.0
    IL_0154:  ldnull
    IL_0155:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_015a:  stelem.ref
    IL_015b:  ldloc.s    V_4
    IL_015d:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::InvokeMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                 string,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Type>,
                                                                                                                                                                 class [mscorlib]System.Type,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0162:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0167:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site4'
    IL_016c:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site4'
    IL_0171:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Target
    IL_0176:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site4'
    IL_017b:  ldtoken    [mscorlib]System.Console
    IL_0180:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0185:  ldc.i4.2
    IL_0186:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_018b:  stloc.s    V_5
    IL_018d:  ldloc.s    V_5
    IL_018f:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0194:  brtrue     IL_0244

    IL_0199:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site5'
    IL_019e:  brtrue.s   IL_01dc

    IL_01a0:  ldc.i4.8
    IL_01a1:  ldc.i4.s   36
    IL_01a3:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_01a8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ad:  ldc.i4.2
    IL_01ae:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_01b3:  stloc.s    V_6
    IL_01b5:  ldloc.s    V_6
    IL_01b7:  ldc.i4.0
    IL_01b8:  ldc.i4.1
    IL_01b9:  ldnull
    IL_01ba:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_01bf:  stelem.ref
    IL_01c0:  ldloc.s    V_6
    IL_01c2:  ldc.i4.1
    IL_01c3:  ldc.i4.0
    IL_01c4:  ldnull
    IL_01c5:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_01ca:  stelem.ref
    IL_01cb:  ldloc.s    V_6
    IL_01cd:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_01d2:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_01d7:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site5'
    IL_01dc:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site5'
    IL_01e1:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Target
    IL_01e6:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site5'
    IL_01eb:  ldloc.s    V_5
    IL_01ed:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site6'
    IL_01f2:  brtrue.s   IL_0228

    IL_01f4:  ldc.i4.0
    IL_01f5:  ldstr      "P"
    IL_01fa:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_01ff:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0204:  ldc.i4.1
    IL_0205:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_020a:  stloc.s    V_7
    IL_020c:  ldloc.s    V_7
    IL_020e:  ldc.i4.0
    IL_020f:  ldc.i4.0
    IL_0210:  ldnull
    IL_0211:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0216:  stelem.ref
    IL_0217:  ldloc.s    V_7
    IL_0219:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_021e:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0223:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site6'
    IL_0228:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site6'
    IL_022d:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_0232:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site6'
    IL_0237:  ldarg.1
    IL_0238:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_023d:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>::Invoke(!0,
                                                                                                                                                                                                                                            !1,
                                                                                                                                                                                                                                            !2)
    IL_0242:  br.s       IL_024b

    IL_0244:  ldloc.s    V_5
    IL_0246:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_024b:  callvirt   instance void class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>::Invoke(!0,
                                                                                                                                                                               !1,
                                                                                                                                                                               !2)
    IL_0250:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site7'
    IL_0255:  brtrue.s   IL_0288

    IL_0257:  ldc.i4.0
    IL_0258:  ldc.i4.s   83
    IL_025a:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_025f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0264:  ldc.i4.1
    IL_0265:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_026a:  stloc.s    V_8
    IL_026c:  ldloc.s    V_8
    IL_026e:  ldc.i4.0
    IL_026f:  ldc.i4.0
    IL_0270:  ldnull
    IL_0271:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0276:  stelem.ref
    IL_0277:  ldloc.s    V_8
    IL_0279:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::UnaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                   valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                   class [mscorlib]System.Type,
                                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_027e:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0283:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site7'
    IL_0288:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site7'
    IL_028d:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Target
    IL_0292:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site7'
    IL_0297:  ldc.i4.3
    IL_0298:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_029d:  stloc.s    V_9
    IL_029f:  ldloc.s    V_9
    IL_02a1:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_False(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_02a6:  brtrue     IL_0355

    IL_02ab:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site8'
    IL_02b0:  brtrue.s   IL_02ed

    IL_02b2:  ldc.i4.8
    IL_02b3:  ldc.i4.2
    IL_02b4:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_02b9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02be:  ldc.i4.2
    IL_02bf:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_02c4:  stloc.s    V_10
    IL_02c6:  ldloc.s    V_10
    IL_02c8:  ldc.i4.0
    IL_02c9:  ldc.i4.1
    IL_02ca:  ldnull
    IL_02cb:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_02d0:  stelem.ref
    IL_02d1:  ldloc.s    V_10
    IL_02d3:  ldc.i4.1
    IL_02d4:  ldc.i4.0
    IL_02d5:  ldnull
    IL_02d6:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_02db:  stelem.ref
    IL_02dc:  ldloc.s    V_10
    IL_02de:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_02e3:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_02e8:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site8'
    IL_02ed:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site8'
    IL_02f2:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Target
    IL_02f7:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site8'
    IL_02fc:  ldloc.s    V_9
    IL_02fe:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site9'
    IL_0303:  brtrue.s   IL_0339

    IL_0305:  ldc.i4.0
    IL_0306:  ldstr      "P"
    IL_030b:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0310:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0315:  ldc.i4.1
    IL_0316:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_031b:  stloc.s    V_11
    IL_031d:  ldloc.s    V_11
    IL_031f:  ldc.i4.0
    IL_0320:  ldc.i4.0
    IL_0321:  ldnull
    IL_0322:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0327:  stelem.ref
    IL_0328:  ldloc.s    V_11
    IL_032a:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_032f:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0334:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site9'
    IL_0339:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site9'
    IL_033e:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_0343:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site9'
    IL_0348:  ldarg.1
    IL_0349:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_034e:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>::Invoke(!0,
                                                                                                                                                                                                                                            !1,
                                                                                                                                                                                                                                            !2)
    IL_0353:  br.s       IL_035c

    IL_0355:  ldloc.s    V_9
    IL_0357:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_035c:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>::Invoke(!0,
                                                                                                                                                    !1)
    IL_0361:  brfalse.s  IL_0373

    IL_0363:  ldc.i4.4
    IL_0364:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0369:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_036e:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0373:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Sitea'
    IL_0378:  brtrue.s   IL_03ab

    IL_037a:  ldc.i4.0
    IL_037b:  ldc.i4.s   83
    IL_037d:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0382:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0387:  ldc.i4.1
    IL_0388:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_038d:  stloc.s    V_12
    IL_038f:  ldloc.s    V_12
    IL_0391:  ldc.i4.0
    IL_0392:  ldc.i4.0
    IL_0393:  ldnull
    IL_0394:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0399:  stelem.ref
    IL_039a:  ldloc.s    V_12
    IL_039c:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::UnaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                   valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                   class [mscorlib]System.Type,
                                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_03a1:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_03a6:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Sitea'
    IL_03ab:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Sitea'
    IL_03b0:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Target
    IL_03b5:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Sitea'
    IL_03ba:  ldc.i4.5
    IL_03bb:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_03c0:  stloc.s    V_13
    IL_03c2:  ldloc.s    V_13
    IL_03c4:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_03c9:  brtrue     IL_0479

    IL_03ce:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Siteb'
    IL_03d3:  brtrue.s   IL_0411

    IL_03d5:  ldc.i4.8
    IL_03d6:  ldc.i4.s   36
    IL_03d8:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_03dd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03e2:  ldc.i4.2
    IL_03e3:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_03e8:  stloc.s    V_14
    IL_03ea:  ldloc.s    V_14
    IL_03ec:  ldc.i4.0
    IL_03ed:  ldc.i4.1
    IL_03ee:  ldnull
    IL_03ef:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_03f4:  stelem.ref
    IL_03f5:  ldloc.s    V_14
    IL_03f7:  ldc.i4.1
    IL_03f8:  ldc.i4.0
    IL_03f9:  ldnull
    IL_03fa:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_03ff:  stelem.ref
    IL_0400:  ldloc.s    V_14
    IL_0402:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0407:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_040c:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Siteb'
    IL_0411:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Siteb'
    IL_0416:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Target
    IL_041b:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Siteb'
    IL_0420:  ldloc.s    V_13
    IL_0422:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Sitec'
    IL_0427:  brtrue.s   IL_045d

    IL_0429:  ldc.i4.0
    IL_042a:  ldstr      "P"
    IL_042f:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0434:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0439:  ldc.i4.1
    IL_043a:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_043f:  stloc.s    V_15
    IL_0441:  ldloc.s    V_15
    IL_0443:  ldc.i4.0
    IL_0444:  ldc.i4.0
    IL_0445:  ldnull
    IL_0446:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_044b:  stelem.ref
    IL_044c:  ldloc.s    V_15
    IL_044e:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0453:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0458:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Sitec'
    IL_045d:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Sitec'
    IL_0462:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_0467:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Sitec'
    IL_046c:  ldarg.1
    IL_046d:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_0472:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>::Invoke(!0,
                                                                                                                                                                                                                                            !1,
                                                                                                                                                                                                                                            !2)
    IL_0477:  br.s       IL_0480

    IL_0479:  ldloc.s    V_13
    IL_047b:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0480:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>::Invoke(!0,
                                                                                                                                                    !1)
    IL_0485:  brfalse.s  IL_0497

    IL_0487:  ldc.i4.6
    IL_0488:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_048d:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0492:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0497:  ret
  } // end of method S::WithDynamic

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
