



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
.assembly CustomShortCircuitOperators
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module CustomShortCircuitOperators.dll
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
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method BaseClass::op_True

  .method public hidebysig specialname static 
          bool  op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass x) cil managed
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
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method C::op_BitwiseAnd

  .method public hidebysig specialname static 
          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C 
          op_BitwiseOr(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C x,
                       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C y) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method C::op_BitwiseOr

  .method public hidebysig specialname static 
          class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C 
          op_LogicalNot(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C x) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method C::op_LogicalNot

  .method public hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C 
          GetC(int32 a) cil managed
  {
    // Code size       11 (0xb)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  br.s       IL_0009

    IL_0009:  ldloc.0
    IL_000a:  ret
  } // end of method C::GetC

  .method public hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C 
          LogicAnd() cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0007:  dup
    IL_0008:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_000d:  brtrue.s   IL_001a

    IL_000f:  ldc.i4.2
    IL_0010:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0015:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_001a:  stloc.0
    IL_001b:  br.s       IL_001d

    IL_001d:  ldloc.0
    IL_001e:  ret
  } // end of method C::LogicAnd

  .method public hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C 
          LogicOr() cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0007:  dup
    IL_0008:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_000d:  brtrue.s   IL_001a

    IL_000f:  ldc.i4.2
    IL_0010:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0015:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseOr(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_001a:  stloc.0
    IL_001b:  br.s       IL_001d

    IL_001d:  ldloc.0
    IL_001e:  ret
  } // end of method C::LogicOr

  .method public hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C 
          Complex() cil managed
  {
    // Code size       93 (0x5d)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0007:  dup
    IL_0008:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_000d:  brtrue.s   IL_001a

    IL_000f:  ldc.i4.2
    IL_0010:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0015:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseOr(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_001a:  dup
    IL_001b:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0020:  brtrue.s   IL_002d

    IL_0022:  ldc.i4.3
    IL_0023:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0028:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_002d:  dup
    IL_002e:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0033:  brtrue.s   IL_0058

    IL_0035:  ldc.i4.4
    IL_0036:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_003b:  dup
    IL_003c:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0041:  brtrue.s   IL_004e

    IL_0043:  ldc.i4.5
    IL_0044:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0049:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseOr(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_004e:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_LogicalNot(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0053:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0058:  stloc.0
    IL_0059:  br.s       IL_005b

    IL_005b:  ldloc.0
    IL_005c:  ret
  } // end of method C::Complex

  .method private hidebysig static void  Main() cil managed
  {
    // Code size       70 (0x46)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_2,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_3)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::.ctor()
    IL_000c:  stloc.1
    IL_000d:  ldloc.0
    IL_000e:  dup
    IL_000f:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0014:  brtrue.s   IL_001c

    IL_0016:  ldloc.1
    IL_0017:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_001c:  stloc.2
    IL_001d:  ldloc.0
    IL_001e:  dup
    IL_001f:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0024:  brtrue.s   IL_002c

    IL_0026:  ldloc.1
    IL_0027:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseOr(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_002c:  stloc.3
    IL_002d:  ldloc.2
    IL_002e:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0033:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0038:  nop
    IL_0039:  ldloc.3
    IL_003a:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_003f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0044:  nop
    IL_0045:  ret
  } // end of method C::Main

  .method private hidebysig static void  Test2() cil managed
  {
    // Code size       160 (0xa0)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0007:  dup
    IL_0008:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_000d:  brtrue.s   IL_001a

    IL_000f:  ldc.i4.2
    IL_0010:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0015:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_001a:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_001f:  ldc.i4.0
    IL_0020:  ceq
    IL_0022:  stloc.0
    IL_0023:  ldloc.0
    IL_0024:  brtrue.s   IL_0034

    IL_0026:  nop
    IL_0027:  ldc.i4.3
    IL_0028:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_002d:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0032:  nop
    IL_0033:  nop
    IL_0034:  ldc.i4.1
    IL_0035:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_003a:  dup
    IL_003b:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0040:  brtrue.s   IL_004d

    IL_0042:  ldc.i4.2
    IL_0043:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0048:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseOr(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_004d:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0052:  ldc.i4.0
    IL_0053:  ceq
    IL_0055:  stloc.0
    IL_0056:  ldloc.0
    IL_0057:  brtrue.s   IL_0067

    IL_0059:  nop
    IL_005a:  ldc.i4.3
    IL_005b:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0060:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0065:  nop
    IL_0066:  nop
    IL_0067:  ldc.i4.1
    IL_0068:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_006d:  dup
    IL_006e:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0073:  brtrue.s   IL_0080

    IL_0075:  ldc.i4.2
    IL_0076:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_007b:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0080:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_LogicalNot(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0085:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_008a:  ldc.i4.0
    IL_008b:  ceq
    IL_008d:  stloc.0
    IL_008e:  ldloc.0
    IL_008f:  brtrue.s   IL_009f

    IL_0091:  nop
    IL_0092:  ldc.i4.3
    IL_0093:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0098:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_009d:  nop
    IL_009e:  nop
    IL_009f:  ret
  } // end of method C::Test2

  .method private hidebysig static void  Test3() cil managed
  {
    // Code size       67 (0x43)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_000d:  ldc.i4.0
    IL_000e:  ceq
    IL_0010:  stloc.1
    IL_0011:  ldloc.1
    IL_0012:  brtrue.s   IL_0022

    IL_0014:  nop
    IL_0015:  ldloc.0
    IL_0016:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_001b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0020:  nop
    IL_0021:  nop
    IL_0022:  ldloc.0
    IL_0023:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_LogicalNot(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0028:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_002d:  ldc.i4.0
    IL_002e:  ceq
    IL_0030:  stloc.1
    IL_0031:  ldloc.1
    IL_0032:  brtrue.s   IL_0042

    IL_0034:  nop
    IL_0035:  ldloc.0
    IL_0036:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_003b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0040:  nop
    IL_0041:  nop
    IL_0042:  ret
  } // end of method C::Test3

  .method public hidebysig instance void 
          WithDynamic(object d) cil managed
  {
    .param [1]
    .custom instance void [System.Core]System.Runtime.CompilerServices.DynamicAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1150 (0x47e)
    .maxstack  13
    .locals init (class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site1'
    IL_0006:  brtrue.s   IL_004b

    IL_0008:  ldc.i4     0x100
    IL_000d:  ldstr      "WriteLine"
    IL_0012:  ldnull
    IL_0013:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_0018:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001d:  ldc.i4.2
    IL_001e:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldc.i4.0
    IL_0026:  ldc.i4.s   33
    IL_0028:  ldnull
    IL_0029:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_002e:  stelem.ref
    IL_002f:  ldloc.0
    IL_0030:  ldc.i4.1
    IL_0031:  ldc.i4.0
    IL_0032:  ldnull
    IL_0033:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0038:  stelem.ref
    IL_0039:  ldloc.0
    IL_003a:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::InvokeMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                 string,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Type>,
                                                                                                                                                                 class [mscorlib]System.Type,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_003f:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0044:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site1'
    IL_0049:  br.s       IL_004b

    IL_004b:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site1'
    IL_0050:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Target
    IL_0055:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site1'
    IL_005a:  ldtoken    [mscorlib]System.Console
    IL_005f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0064:  ldc.i4.1
    IL_0065:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_006a:  stloc.1
    IL_006b:  ldloc.1
    IL_006c:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0071:  brtrue     IL_011c

    IL_0076:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site2'
    IL_007b:  brtrue.s   IL_00b6

    IL_007d:  ldc.i4.8
    IL_007e:  ldc.i4.2
    IL_007f:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_0084:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0089:  ldc.i4.2
    IL_008a:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_008f:  stloc.0
    IL_0090:  ldloc.0
    IL_0091:  ldc.i4.0
    IL_0092:  ldc.i4.1
    IL_0093:  ldnull
    IL_0094:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0099:  stelem.ref
    IL_009a:  ldloc.0
    IL_009b:  ldc.i4.1
    IL_009c:  ldc.i4.0
    IL_009d:  ldnull
    IL_009e:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_00a3:  stelem.ref
    IL_00a4:  ldloc.0
    IL_00a5:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_00aa:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_00af:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site2'
    IL_00b4:  br.s       IL_00b6

    IL_00b6:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site2'
    IL_00bb:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Target
    IL_00c0:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site2'
    IL_00c5:  ldloc.1
    IL_00c6:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site3'
    IL_00cb:  brtrue.s   IL_0100

    IL_00cd:  ldc.i4.0
    IL_00ce:  ldstr      "P"
    IL_00d3:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_00d8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00dd:  ldc.i4.1
    IL_00de:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_00e3:  stloc.0
    IL_00e4:  ldloc.0
    IL_00e5:  ldc.i4.0
    IL_00e6:  ldc.i4.0
    IL_00e7:  ldnull
    IL_00e8:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_00ed:  stelem.ref
    IL_00ee:  ldloc.0
    IL_00ef:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_00f4:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_00f9:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site3'
    IL_00fe:  br.s       IL_0100

    IL_0100:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site3'
    IL_0105:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_010a:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site3'
    IL_010f:  ldarg.1
    IL_0110:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_0115:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>::Invoke(!0,
                                                                                                                                                                                                                                        !1,
                                                                                                                                                                                                                                        !2)
    IL_011a:  br.s       IL_011d

    IL_011c:  ldloc.1
    IL_011d:  nop
    IL_011e:  callvirt   instance void class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>::Invoke(!0,
                                                                                                                                                                               !1,
                                                                                                                                                                               !2)
    IL_0123:  nop
    IL_0124:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site4'
    IL_0129:  brtrue.s   IL_016e

    IL_012b:  ldc.i4     0x100
    IL_0130:  ldstr      "WriteLine"
    IL_0135:  ldnull
    IL_0136:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_013b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0140:  ldc.i4.2
    IL_0141:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0146:  stloc.0
    IL_0147:  ldloc.0
    IL_0148:  ldc.i4.0
    IL_0149:  ldc.i4.s   33
    IL_014b:  ldnull
    IL_014c:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0151:  stelem.ref
    IL_0152:  ldloc.0
    IL_0153:  ldc.i4.1
    IL_0154:  ldc.i4.0
    IL_0155:  ldnull
    IL_0156:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_015b:  stelem.ref
    IL_015c:  ldloc.0
    IL_015d:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::InvokeMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                 string,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Type>,
                                                                                                                                                                 class [mscorlib]System.Type,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0162:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0167:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site4'
    IL_016c:  br.s       IL_016e

    IL_016e:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site4'
    IL_0173:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Target
    IL_0178:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site4'
    IL_017d:  ldtoken    [mscorlib]System.Console
    IL_0182:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0187:  ldc.i4.2
    IL_0188:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_018d:  stloc.1
    IL_018e:  ldloc.1
    IL_018f:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0194:  brtrue     IL_0240

    IL_0199:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site5'
    IL_019e:  brtrue.s   IL_01da

    IL_01a0:  ldc.i4.8
    IL_01a1:  ldc.i4.s   36
    IL_01a3:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_01a8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ad:  ldc.i4.2
    IL_01ae:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_01b3:  stloc.0
    IL_01b4:  ldloc.0
    IL_01b5:  ldc.i4.0
    IL_01b6:  ldc.i4.1
    IL_01b7:  ldnull
    IL_01b8:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_01bd:  stelem.ref
    IL_01be:  ldloc.0
    IL_01bf:  ldc.i4.1
    IL_01c0:  ldc.i4.0
    IL_01c1:  ldnull
    IL_01c2:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_01c7:  stelem.ref
    IL_01c8:  ldloc.0
    IL_01c9:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_01ce:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_01d3:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site5'
    IL_01d8:  br.s       IL_01da

    IL_01da:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site5'
    IL_01df:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Target
    IL_01e4:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site5'
    IL_01e9:  ldloc.1
    IL_01ea:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site6'
    IL_01ef:  brtrue.s   IL_0224

    IL_01f1:  ldc.i4.0
    IL_01f2:  ldstr      "P"
    IL_01f7:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_01fc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0201:  ldc.i4.1
    IL_0202:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0207:  stloc.0
    IL_0208:  ldloc.0
    IL_0209:  ldc.i4.0
    IL_020a:  ldc.i4.0
    IL_020b:  ldnull
    IL_020c:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0211:  stelem.ref
    IL_0212:  ldloc.0
    IL_0213:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0218:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_021d:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site6'
    IL_0222:  br.s       IL_0224

    IL_0224:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site6'
    IL_0229:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_022e:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site6'
    IL_0233:  ldarg.1
    IL_0234:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_0239:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>::Invoke(!0,
                                                                                                                                                                                                                                        !1,
                                                                                                                                                                                                                                        !2)
    IL_023e:  br.s       IL_0241

    IL_0240:  ldloc.1
    IL_0241:  nop
    IL_0242:  callvirt   instance void class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>::Invoke(!0,
                                                                                                                                                                               !1,
                                                                                                                                                                               !2)
    IL_0247:  nop
    IL_0248:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site7'
    IL_024d:  brtrue.s   IL_027f

    IL_024f:  ldc.i4.0
    IL_0250:  ldc.i4.s   83
    IL_0252:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_0257:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_025c:  ldc.i4.1
    IL_025d:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0262:  stloc.0
    IL_0263:  ldloc.0
    IL_0264:  ldc.i4.0
    IL_0265:  ldc.i4.0
    IL_0266:  ldnull
    IL_0267:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_026c:  stelem.ref
    IL_026d:  ldloc.0
    IL_026e:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::UnaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                   valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                   class [mscorlib]System.Type,
                                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0273:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0278:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site7'
    IL_027d:  br.s       IL_027f

    IL_027f:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site7'
    IL_0284:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Target
    IL_0289:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site7'
    IL_028e:  ldc.i4.3
    IL_028f:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0294:  stloc.1
    IL_0295:  ldloc.1
    IL_0296:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_029b:  brtrue     IL_0346

    IL_02a0:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site8'
    IL_02a5:  brtrue.s   IL_02e0

    IL_02a7:  ldc.i4.8
    IL_02a8:  ldc.i4.2
    IL_02a9:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_02ae:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02b3:  ldc.i4.2
    IL_02b4:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_02b9:  stloc.0
    IL_02ba:  ldloc.0
    IL_02bb:  ldc.i4.0
    IL_02bc:  ldc.i4.1
    IL_02bd:  ldnull
    IL_02be:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_02c3:  stelem.ref
    IL_02c4:  ldloc.0
    IL_02c5:  ldc.i4.1
    IL_02c6:  ldc.i4.0
    IL_02c7:  ldnull
    IL_02c8:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_02cd:  stelem.ref
    IL_02ce:  ldloc.0
    IL_02cf:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_02d4:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_02d9:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site8'
    IL_02de:  br.s       IL_02e0

    IL_02e0:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site8'
    IL_02e5:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Target
    IL_02ea:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site8'
    IL_02ef:  ldloc.1
    IL_02f0:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site9'
    IL_02f5:  brtrue.s   IL_032a

    IL_02f7:  ldc.i4.0
    IL_02f8:  ldstr      "P"
    IL_02fd:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_0302:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0307:  ldc.i4.1
    IL_0308:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_030d:  stloc.0
    IL_030e:  ldloc.0
    IL_030f:  ldc.i4.0
    IL_0310:  ldc.i4.0
    IL_0311:  ldnull
    IL_0312:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0317:  stelem.ref
    IL_0318:  ldloc.0
    IL_0319:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_031e:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0323:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site9'
    IL_0328:  br.s       IL_032a

    IL_032a:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site9'
    IL_032f:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_0334:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Site9'
    IL_0339:  ldarg.1
    IL_033a:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_033f:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>::Invoke(!0,
                                                                                                                                                                                                                                        !1,
                                                                                                                                                                                                                                        !2)
    IL_0344:  br.s       IL_0347

    IL_0346:  ldloc.1
    IL_0347:  nop
    IL_0348:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>::Invoke(!0,
                                                                                                                                                    !1)
    IL_034d:  ldc.i4.0
    IL_034e:  ceq
    IL_0350:  stloc.2
    IL_0351:  ldloc.2
    IL_0352:  brtrue.s   IL_0362

    IL_0354:  nop
    IL_0355:  ldc.i4.4
    IL_0356:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_035b:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0360:  nop
    IL_0361:  nop
    IL_0362:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Sitea'
    IL_0367:  brtrue.s   IL_0399

    IL_0369:  ldc.i4.0
    IL_036a:  ldc.i4.s   83
    IL_036c:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_0371:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0376:  ldc.i4.1
    IL_0377:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_037c:  stloc.0
    IL_037d:  ldloc.0
    IL_037e:  ldc.i4.0
    IL_037f:  ldc.i4.0
    IL_0380:  ldnull
    IL_0381:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0386:  stelem.ref
    IL_0387:  ldloc.0
    IL_0388:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::UnaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                   valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                   class [mscorlib]System.Type,
                                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_038d:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0392:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Sitea'
    IL_0397:  br.s       IL_0399

    IL_0399:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Sitea'
    IL_039e:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Target
    IL_03a3:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Sitea'
    IL_03a8:  ldc.i4.5
    IL_03a9:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_03ae:  stloc.1
    IL_03af:  ldloc.1
    IL_03b0:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_03b5:  brtrue     IL_0461

    IL_03ba:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Siteb'
    IL_03bf:  brtrue.s   IL_03fb

    IL_03c1:  ldc.i4.8
    IL_03c2:  ldc.i4.s   36
    IL_03c4:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_03c9:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03ce:  ldc.i4.2
    IL_03cf:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_03d4:  stloc.0
    IL_03d5:  ldloc.0
    IL_03d6:  ldc.i4.0
    IL_03d7:  ldc.i4.1
    IL_03d8:  ldnull
    IL_03d9:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_03de:  stelem.ref
    IL_03df:  ldloc.0
    IL_03e0:  ldc.i4.1
    IL_03e1:  ldc.i4.0
    IL_03e2:  ldnull
    IL_03e3:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_03e8:  stelem.ref
    IL_03e9:  ldloc.0
    IL_03ea:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_03ef:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_03f4:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Siteb'
    IL_03f9:  br.s       IL_03fb

    IL_03fb:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Siteb'
    IL_0400:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Target
    IL_0405:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Siteb'
    IL_040a:  ldloc.1
    IL_040b:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Sitec'
    IL_0410:  brtrue.s   IL_0445

    IL_0412:  ldc.i4.0
    IL_0413:  ldstr      "P"
    IL_0418:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_041d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0422:  ldc.i4.1
    IL_0423:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0428:  stloc.0
    IL_0429:  ldloc.0
    IL_042a:  ldc.i4.0
    IL_042b:  ldc.i4.0
    IL_042c:  ldnull
    IL_042d:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0432:  stelem.ref
    IL_0433:  ldloc.0
    IL_0434:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0439:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_043e:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Sitec'
    IL_0443:  br.s       IL_0445

    IL_0445:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Sitec'
    IL_044a:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_044f:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<WithDynamic>o__SiteContainer0'::'<>p__Sitec'
    IL_0454:  ldarg.1
    IL_0455:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_045a:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>::Invoke(!0,
                                                                                                                                                                                                                                        !1,
                                                                                                                                                                                                                                        !2)
    IL_045f:  br.s       IL_0462

    IL_0461:  ldloc.1
    IL_0462:  nop
    IL_0463:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>::Invoke(!0,
                                                                                                                                                    !1)
    IL_0468:  ldc.i4.0
    IL_0469:  ceq
    IL_046b:  stloc.2
    IL_046c:  ldloc.2
    IL_046d:  brtrue.s   IL_047d

    IL_046f:  nop
    IL_0470:  ldc.i4.6
    IL_0471:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0476:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_047b:  nop
    IL_047c:  nop
    IL_047d:  ret
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
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.1
    IL_0003:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0008:  ret
  } // end of method S::.ctor

  .method public hidebysig specialname static 
          bool  op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S x) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  1
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarga.s   x
    IL_0003:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method S::op_True

  .method public hidebysig specialname static 
          bool  op_False(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S x) cil managed
  {
    // Code size       13 (0xd)
    .maxstack  1
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarga.s   x
    IL_0003:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0008:  stloc.0
    IL_0009:  br.s       IL_000b

    IL_000b:  ldloc.0
    IL_000c:  ret
  } // end of method S::op_False

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S 
          op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S x,
                        valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S y) cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_0)
    IL_0000:  nop
    IL_0001:  ldarga.s   x
    IL_0003:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0008:  ldarga.s   y
    IL_000a:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_000f:  and
    IL_0010:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::.ctor(bool)
    IL_0015:  stloc.0
    IL_0016:  br.s       IL_0018

    IL_0018:  ldloc.0
    IL_0019:  ret
  } // end of method S::op_BitwiseAnd

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S 
          op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S x,
                       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S y) cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_0)
    IL_0000:  nop
    IL_0001:  ldarga.s   x
    IL_0003:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0008:  ldarga.s   y
    IL_000a:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_000f:  or
    IL_0010:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::.ctor(bool)
    IL_0015:  stloc.0
    IL_0016:  br.s       IL_0018

    IL_0018:  ldloc.0
    IL_0019:  ret
  } // end of method S::op_BitwiseOr

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S 
          op_LogicalNot(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S x) cil managed
  {
    // Code size       21 (0x15)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_0)
    IL_0000:  nop
    IL_0001:  ldarga.s   x
    IL_0003:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0008:  ldc.i4.0
    IL_0009:  ceq
    IL_000b:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::.ctor(bool)
    IL_0010:  stloc.0
    IL_0011:  br.s       IL_0013

    IL_0013:  ldloc.0
    IL_0014:  ret
  } // end of method S::op_LogicalNot

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S 
          Get(int32 i) cil managed
  {
    // Code size       15 (0xf)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.0
    IL_0003:  cgt
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::.ctor(bool)
    IL_000a:  stloc.0
    IL_000b:  br.s       IL_000d

    IL_000d:  ldloc.0
    IL_000e:  ret
  } // end of method S::Get

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S 
          LogicAnd() cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0007:  dup
    IL_0008:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_False(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_000d:  brtrue.s   IL_001a

    IL_000f:  ldc.i4.2
    IL_0010:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0015:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,
                                                                                                                                                                                                          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_001a:  stloc.0
    IL_001b:  br.s       IL_001d

    IL_001d:  ldloc.0
    IL_001e:  ret
  } // end of method S::LogicAnd

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S 
          LogicOr() cil managed
  {
    // Code size       31 (0x1f)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0007:  dup
    IL_0008:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_000d:  brtrue.s   IL_001a

    IL_000f:  ldc.i4.2
    IL_0010:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0015:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,
                                                                                                                                                                                                         valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_001a:  stloc.0
    IL_001b:  br.s       IL_001d

    IL_001d:  ldloc.0
    IL_001e:  ret
  } // end of method S::LogicOr

  .method public hidebysig instance void 
          InConditionDetection() cil managed
  {
    // Code size       143 (0x8f)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldstr      "a"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  ldc.i4.1
    IL_000d:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0012:  dup
    IL_0013:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_False(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0018:  brtrue.s   IL_0025

    IL_001a:  ldc.i4.2
    IL_001b:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0020:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,
                                                                                                                                                                                                          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0025:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_002a:  ldc.i4.0
    IL_002b:  ceq
    IL_002d:  stloc.0
    IL_002e:  ldloc.0
    IL_002f:  brtrue.s   IL_0040

    IL_0031:  nop
    IL_0032:  ldstr      "b"
    IL_0037:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003c:  nop
    IL_003d:  nop
    IL_003e:  br.s       IL_004d

    IL_0040:  nop
    IL_0041:  ldstr      "c"
    IL_0046:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004b:  nop
    IL_004c:  nop
    IL_004d:  ldc.i4.1
    IL_004e:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0053:  dup
    IL_0054:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0059:  brtrue.s   IL_0066

    IL_005b:  ldc.i4.2
    IL_005c:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0061:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,
                                                                                                                                                                                                         valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0066:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_006b:  ldc.i4.0
    IL_006c:  ceq
    IL_006e:  stloc.0
    IL_006f:  ldloc.0
    IL_0070:  brtrue.s   IL_0081

    IL_0072:  nop
    IL_0073:  ldstr      "d"
    IL_0078:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007d:  nop
    IL_007e:  nop
    IL_007f:  br.s       IL_008e

    IL_0081:  nop
    IL_0082:  ldstr      "e"
    IL_0087:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_008c:  nop
    IL_008d:  nop
    IL_008e:  ret
  } // end of method S::InConditionDetection

  .method public hidebysig instance void 
          WithDynamic(object d) cil managed
  {
    .param [1]
    .custom instance void [System.Core]System.Runtime.CompilerServices.DynamicAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1180 (0x49c)
    .maxstack  13
    .locals init (class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo[] V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site1'
    IL_0006:  brtrue.s   IL_004b

    IL_0008:  ldc.i4     0x100
    IL_000d:  ldstr      "WriteLine"
    IL_0012:  ldnull
    IL_0013:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0018:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001d:  ldc.i4.2
    IL_001e:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0023:  stloc.0
    IL_0024:  ldloc.0
    IL_0025:  ldc.i4.0
    IL_0026:  ldc.i4.s   33
    IL_0028:  ldnull
    IL_0029:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_002e:  stelem.ref
    IL_002f:  ldloc.0
    IL_0030:  ldc.i4.1
    IL_0031:  ldc.i4.0
    IL_0032:  ldnull
    IL_0033:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0038:  stelem.ref
    IL_0039:  ldloc.0
    IL_003a:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::InvokeMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                 string,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Type>,
                                                                                                                                                                 class [mscorlib]System.Type,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_003f:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0044:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site1'
    IL_0049:  br.s       IL_004b

    IL_004b:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site1'
    IL_0050:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Target
    IL_0055:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site1'
    IL_005a:  ldtoken    [mscorlib]System.Console
    IL_005f:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0064:  ldc.i4.1
    IL_0065:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_006a:  stloc.1
    IL_006b:  ldloc.1
    IL_006c:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_False(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0071:  brtrue     IL_011c

    IL_0076:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site2'
    IL_007b:  brtrue.s   IL_00b6

    IL_007d:  ldc.i4.8
    IL_007e:  ldc.i4.2
    IL_007f:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0084:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0089:  ldc.i4.2
    IL_008a:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_008f:  stloc.0
    IL_0090:  ldloc.0
    IL_0091:  ldc.i4.0
    IL_0092:  ldc.i4.1
    IL_0093:  ldnull
    IL_0094:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0099:  stelem.ref
    IL_009a:  ldloc.0
    IL_009b:  ldc.i4.1
    IL_009c:  ldc.i4.0
    IL_009d:  ldnull
    IL_009e:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_00a3:  stelem.ref
    IL_00a4:  ldloc.0
    IL_00a5:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_00aa:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_00af:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site2'
    IL_00b4:  br.s       IL_00b6

    IL_00b6:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site2'
    IL_00bb:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Target
    IL_00c0:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site2'
    IL_00c5:  ldloc.1
    IL_00c6:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site3'
    IL_00cb:  brtrue.s   IL_0100

    IL_00cd:  ldc.i4.0
    IL_00ce:  ldstr      "P"
    IL_00d3:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_00d8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00dd:  ldc.i4.1
    IL_00de:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_00e3:  stloc.0
    IL_00e4:  ldloc.0
    IL_00e5:  ldc.i4.0
    IL_00e6:  ldc.i4.0
    IL_00e7:  ldnull
    IL_00e8:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_00ed:  stelem.ref
    IL_00ee:  ldloc.0
    IL_00ef:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_00f4:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_00f9:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site3'
    IL_00fe:  br.s       IL_0100

    IL_0100:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site3'
    IL_0105:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_010a:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site3'
    IL_010f:  ldarg.1
    IL_0110:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_0115:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>::Invoke(!0,
                                                                                                                                                                                                                                            !1,
                                                                                                                                                                                                                                            !2)
    IL_011a:  br.s       IL_0122

    IL_011c:  ldloc.1
    IL_011d:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0122:  nop
    IL_0123:  callvirt   instance void class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>::Invoke(!0,
                                                                                                                                                                               !1,
                                                                                                                                                                               !2)
    IL_0128:  nop
    IL_0129:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site4'
    IL_012e:  brtrue.s   IL_0173

    IL_0130:  ldc.i4     0x100
    IL_0135:  ldstr      "WriteLine"
    IL_013a:  ldnull
    IL_013b:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0140:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0145:  ldc.i4.2
    IL_0146:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_014b:  stloc.0
    IL_014c:  ldloc.0
    IL_014d:  ldc.i4.0
    IL_014e:  ldc.i4.s   33
    IL_0150:  ldnull
    IL_0151:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0156:  stelem.ref
    IL_0157:  ldloc.0
    IL_0158:  ldc.i4.1
    IL_0159:  ldc.i4.0
    IL_015a:  ldnull
    IL_015b:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0160:  stelem.ref
    IL_0161:  ldloc.0
    IL_0162:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::InvokeMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                 string,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Type>,
                                                                                                                                                                 class [mscorlib]System.Type,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0167:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_016c:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site4'
    IL_0171:  br.s       IL_0173

    IL_0173:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site4'
    IL_0178:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Target
    IL_017d:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site4'
    IL_0182:  ldtoken    [mscorlib]System.Console
    IL_0187:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_018c:  ldc.i4.2
    IL_018d:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0192:  stloc.1
    IL_0193:  ldloc.1
    IL_0194:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0199:  brtrue     IL_0245

    IL_019e:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site5'
    IL_01a3:  brtrue.s   IL_01df

    IL_01a5:  ldc.i4.8
    IL_01a6:  ldc.i4.s   36
    IL_01a8:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_01ad:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01b2:  ldc.i4.2
    IL_01b3:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_01b8:  stloc.0
    IL_01b9:  ldloc.0
    IL_01ba:  ldc.i4.0
    IL_01bb:  ldc.i4.1
    IL_01bc:  ldnull
    IL_01bd:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_01c2:  stelem.ref
    IL_01c3:  ldloc.0
    IL_01c4:  ldc.i4.1
    IL_01c5:  ldc.i4.0
    IL_01c6:  ldnull
    IL_01c7:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_01cc:  stelem.ref
    IL_01cd:  ldloc.0
    IL_01ce:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_01d3:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_01d8:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site5'
    IL_01dd:  br.s       IL_01df

    IL_01df:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site5'
    IL_01e4:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Target
    IL_01e9:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site5'
    IL_01ee:  ldloc.1
    IL_01ef:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site6'
    IL_01f4:  brtrue.s   IL_0229

    IL_01f6:  ldc.i4.0
    IL_01f7:  ldstr      "P"
    IL_01fc:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0201:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0206:  ldc.i4.1
    IL_0207:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_020c:  stloc.0
    IL_020d:  ldloc.0
    IL_020e:  ldc.i4.0
    IL_020f:  ldc.i4.0
    IL_0210:  ldnull
    IL_0211:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0216:  stelem.ref
    IL_0217:  ldloc.0
    IL_0218:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_021d:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0222:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site6'
    IL_0227:  br.s       IL_0229

    IL_0229:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site6'
    IL_022e:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_0233:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site6'
    IL_0238:  ldarg.1
    IL_0239:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_023e:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>::Invoke(!0,
                                                                                                                                                                                                                                            !1,
                                                                                                                                                                                                                                            !2)
    IL_0243:  br.s       IL_024b

    IL_0245:  ldloc.1
    IL_0246:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_024b:  nop
    IL_024c:  callvirt   instance void class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>::Invoke(!0,
                                                                                                                                                                               !1,
                                                                                                                                                                               !2)
    IL_0251:  nop
    IL_0252:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site7'
    IL_0257:  brtrue.s   IL_0289

    IL_0259:  ldc.i4.0
    IL_025a:  ldc.i4.s   83
    IL_025c:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0261:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0266:  ldc.i4.1
    IL_0267:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_026c:  stloc.0
    IL_026d:  ldloc.0
    IL_026e:  ldc.i4.0
    IL_026f:  ldc.i4.0
    IL_0270:  ldnull
    IL_0271:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0276:  stelem.ref
    IL_0277:  ldloc.0
    IL_0278:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::UnaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                   valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                   class [mscorlib]System.Type,
                                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_027d:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0282:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site7'
    IL_0287:  br.s       IL_0289

    IL_0289:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site7'
    IL_028e:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Target
    IL_0293:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site7'
    IL_0298:  ldc.i4.3
    IL_0299:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_029e:  stloc.1
    IL_029f:  ldloc.1
    IL_02a0:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_False(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_02a5:  brtrue     IL_0350

    IL_02aa:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site8'
    IL_02af:  brtrue.s   IL_02ea

    IL_02b1:  ldc.i4.8
    IL_02b2:  ldc.i4.2
    IL_02b3:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_02b8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02bd:  ldc.i4.2
    IL_02be:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_02c3:  stloc.0
    IL_02c4:  ldloc.0
    IL_02c5:  ldc.i4.0
    IL_02c6:  ldc.i4.1
    IL_02c7:  ldnull
    IL_02c8:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_02cd:  stelem.ref
    IL_02ce:  ldloc.0
    IL_02cf:  ldc.i4.1
    IL_02d0:  ldc.i4.0
    IL_02d1:  ldnull
    IL_02d2:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_02d7:  stelem.ref
    IL_02d8:  ldloc.0
    IL_02d9:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_02de:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_02e3:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site8'
    IL_02e8:  br.s       IL_02ea

    IL_02ea:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site8'
    IL_02ef:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Target
    IL_02f4:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site8'
    IL_02f9:  ldloc.1
    IL_02fa:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site9'
    IL_02ff:  brtrue.s   IL_0334

    IL_0301:  ldc.i4.0
    IL_0302:  ldstr      "P"
    IL_0307:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_030c:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0311:  ldc.i4.1
    IL_0312:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0317:  stloc.0
    IL_0318:  ldloc.0
    IL_0319:  ldc.i4.0
    IL_031a:  ldc.i4.0
    IL_031b:  ldnull
    IL_031c:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0321:  stelem.ref
    IL_0322:  ldloc.0
    IL_0323:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0328:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_032d:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site9'
    IL_0332:  br.s       IL_0334

    IL_0334:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site9'
    IL_0339:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_033e:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Site9'
    IL_0343:  ldarg.1
    IL_0344:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_0349:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>::Invoke(!0,
                                                                                                                                                                                                                                            !1,
                                                                                                                                                                                                                                            !2)
    IL_034e:  br.s       IL_0356

    IL_0350:  ldloc.1
    IL_0351:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0356:  nop
    IL_0357:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>::Invoke(!0,
                                                                                                                                                    !1)
    IL_035c:  ldc.i4.0
    IL_035d:  ceq
    IL_035f:  stloc.2
    IL_0360:  ldloc.2
    IL_0361:  brtrue.s   IL_0376

    IL_0363:  nop
    IL_0364:  ldc.i4.4
    IL_0365:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_036a:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_036f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0374:  nop
    IL_0375:  nop
    IL_0376:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Sitea'
    IL_037b:  brtrue.s   IL_03ad

    IL_037d:  ldc.i4.0
    IL_037e:  ldc.i4.s   83
    IL_0380:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0385:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_038a:  ldc.i4.1
    IL_038b:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0390:  stloc.0
    IL_0391:  ldloc.0
    IL_0392:  ldc.i4.0
    IL_0393:  ldc.i4.0
    IL_0394:  ldnull
    IL_0395:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_039a:  stelem.ref
    IL_039b:  ldloc.0
    IL_039c:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::UnaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                   valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                   class [mscorlib]System.Type,
                                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_03a1:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_03a6:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Sitea'
    IL_03ab:  br.s       IL_03ad

    IL_03ad:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Sitea'
    IL_03b2:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Target
    IL_03b7:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Sitea'
    IL_03bc:  ldc.i4.5
    IL_03bd:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_03c2:  stloc.1
    IL_03c3:  ldloc.1
    IL_03c4:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_03c9:  brtrue     IL_0475

    IL_03ce:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Siteb'
    IL_03d3:  brtrue.s   IL_040f

    IL_03d5:  ldc.i4.8
    IL_03d6:  ldc.i4.s   36
    IL_03d8:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_03dd:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03e2:  ldc.i4.2
    IL_03e3:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_03e8:  stloc.0
    IL_03e9:  ldloc.0
    IL_03ea:  ldc.i4.0
    IL_03eb:  ldc.i4.1
    IL_03ec:  ldnull
    IL_03ed:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_03f2:  stelem.ref
    IL_03f3:  ldloc.0
    IL_03f4:  ldc.i4.1
    IL_03f5:  ldc.i4.0
    IL_03f6:  ldnull
    IL_03f7:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_03fc:  stelem.ref
    IL_03fd:  ldloc.0
    IL_03fe:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0403:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0408:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Siteb'
    IL_040d:  br.s       IL_040f

    IL_040f:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Siteb'
    IL_0414:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Target
    IL_0419:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Siteb'
    IL_041e:  ldloc.1
    IL_041f:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Sitec'
    IL_0424:  brtrue.s   IL_0459

    IL_0426:  ldc.i4.0
    IL_0427:  ldstr      "P"
    IL_042c:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0431:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0436:  ldc.i4.1
    IL_0437:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_043c:  stloc.0
    IL_043d:  ldloc.0
    IL_043e:  ldc.i4.0
    IL_043f:  ldc.i4.0
    IL_0440:  ldnull
    IL_0441:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0446:  stelem.ref
    IL_0447:  ldloc.0
    IL_0448:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_044d:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0452:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Sitec'
    IL_0457:  br.s       IL_0459

    IL_0459:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Sitec'
    IL_045e:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_0463:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<WithDynamic>o__SiteContainer0'::'<>p__Sitec'
    IL_0468:  ldarg.1
    IL_0469:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_046e:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>::Invoke(!0,
                                                                                                                                                                                                                                            !1,
                                                                                                                                                                                                                                            !2)
    IL_0473:  br.s       IL_047b

    IL_0475:  ldloc.1
    IL_0476:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_047b:  nop
    IL_047c:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>::Invoke(!0,
                                                                                                                                                    !1)
    IL_0481:  ldc.i4.0
    IL_0482:  ceq
    IL_0484:  stloc.2
    IL_0485:  ldloc.2
    IL_0486:  brtrue.s   IL_049b

    IL_0488:  nop
    IL_0489:  ldc.i4.6
    IL_048a:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_048f:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0494:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0499:  nop
    IL_049a:  nop
    IL_049b:  ret
  } // end of method S::WithDynamic

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
