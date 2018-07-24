



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

  // --- The following custom attribute is added automatically, do not uncomment -------
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 07 01 00 00 00 00 ) 

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
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method BaseClass::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
       extends ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass
{
  .class abstract auto ansi sealed nested private beforefieldinit '<>o__10'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> '<>p__0'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> '<>p__1'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> '<>p__2'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> '<>p__3'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> '<>p__4'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> '<>p__5'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> '<>p__6'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> '<>p__7'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> '<>p__8'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> '<>p__9'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> '<>p__10'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> '<>p__11'
  } // end of class '<>o__10'

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
    // Code size       36 (0x24)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_000e:  brtrue.s   IL_001e

    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.2
    IL_0012:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0017:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_001c:  br.s       IL_001f

    IL_001e:  ldloc.0
    IL_001f:  stloc.1
    IL_0020:  br.s       IL_0022

    IL_0022:  ldloc.1
    IL_0023:  ret
  } // end of method C::LogicAnd

  .method public hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C 
          LogicOr() cil managed
  {
    // Code size       36 (0x24)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_000e:  brtrue.s   IL_001e

    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.2
    IL_0012:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0017:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseOr(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_001c:  br.s       IL_001f

    IL_001e:  ldloc.0
    IL_001f:  stloc.1
    IL_0020:  br.s       IL_0022

    IL_0022:  ldloc.1
    IL_0023:  ret
  } // end of method C::LogicOr

  .method public hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C 
          Complex() cil managed
  {
    // Code size       113 (0x71)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_2,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_3)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0007:  stloc.2
    IL_0008:  ldloc.2
    IL_0009:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_000e:  brtrue.s   IL_001e

    IL_0010:  ldloc.2
    IL_0011:  ldc.i4.2
    IL_0012:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0017:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseOr(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_001c:  br.s       IL_001f

    IL_001e:  ldloc.2
    IL_001f:  stloc.1
    IL_0020:  ldloc.1
    IL_0021:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0026:  brtrue.s   IL_0036

    IL_0028:  ldloc.1
    IL_0029:  ldc.i4.3
    IL_002a:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_002f:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0034:  br.s       IL_0037

    IL_0036:  ldloc.1
    IL_0037:  stloc.0
    IL_0038:  ldloc.0
    IL_0039:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_003e:  brtrue.s   IL_006b

    IL_0040:  ldloc.0
    IL_0041:  ldc.i4.4
    IL_0042:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0047:  stloc.1
    IL_0048:  ldloc.1
    IL_0049:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_004e:  brtrue.s   IL_005e

    IL_0050:  ldloc.1
    IL_0051:  ldc.i4.5
    IL_0052:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0057:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseOr(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_005c:  br.s       IL_005f

    IL_005e:  ldloc.1
    IL_005f:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_LogicalNot(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0064:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0069:  br.s       IL_006c

    IL_006b:  ldloc.0
    IL_006c:  stloc.3
    IL_006d:  br.s       IL_006f

    IL_006f:  ldloc.3
    IL_0070:  ret
  } // end of method C::Complex

  .method private hidebysig static void  Main() cil managed
  {
    // Code size       88 (0x58)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_2,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_3,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_4)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::.ctor()
    IL_000c:  stloc.1
    IL_000d:  ldloc.0
    IL_000e:  stloc.s    V_4
    IL_0010:  ldloc.s    V_4
    IL_0012:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0017:  brtrue.s   IL_0023

    IL_0019:  ldloc.s    V_4
    IL_001b:  ldloc.1
    IL_001c:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0021:  br.s       IL_0025

    IL_0023:  ldloc.s    V_4
    IL_0025:  stloc.2
    IL_0026:  ldloc.0
    IL_0027:  stloc.s    V_4
    IL_0029:  ldloc.s    V_4
    IL_002b:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0030:  brtrue.s   IL_003c

    IL_0032:  ldloc.s    V_4
    IL_0034:  ldloc.1
    IL_0035:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseOr(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_003a:  br.s       IL_003e

    IL_003c:  ldloc.s    V_4
    IL_003e:  stloc.3
    IL_003f:  ldloc.2
    IL_0040:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0045:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004a:  nop
    IL_004b:  ldloc.3
    IL_004c:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0051:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0056:  nop
    IL_0057:  ret
  } // end of method C::Main

  .method private hidebysig static void  Test2() cil managed
  {
    // Code size       166 (0xa6)
    .maxstack  2
    .locals init (bool V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_1,
             bool V_2,
             bool V_3)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0007:  stloc.1
    IL_0008:  ldloc.1
    IL_0009:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_000e:  brtrue.s   IL_001e

    IL_0010:  ldloc.1
    IL_0011:  ldc.i4.2
    IL_0012:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0017:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_001c:  br.s       IL_001f

    IL_001e:  ldloc.1
    IL_001f:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0024:  stloc.0
    IL_0025:  ldloc.0
    IL_0026:  brfalse.s  IL_0036

    IL_0028:  nop
    IL_0029:  ldc.i4.3
    IL_002a:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_002f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0034:  nop
    IL_0035:  nop
    IL_0036:  ldc.i4.1
    IL_0037:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_003c:  stloc.1
    IL_003d:  ldloc.1
    IL_003e:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0043:  brtrue.s   IL_0053

    IL_0045:  ldloc.1
    IL_0046:  ldc.i4.2
    IL_0047:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_004c:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseOr(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0051:  br.s       IL_0054

    IL_0053:  ldloc.1
    IL_0054:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0059:  stloc.2
    IL_005a:  ldloc.2
    IL_005b:  brfalse.s  IL_006b

    IL_005d:  nop
    IL_005e:  ldc.i4.3
    IL_005f:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0064:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0069:  nop
    IL_006a:  nop
    IL_006b:  ldc.i4.1
    IL_006c:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0071:  stloc.1
    IL_0072:  ldloc.1
    IL_0073:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0078:  brtrue.s   IL_0088

    IL_007a:  ldloc.1
    IL_007b:  ldc.i4.2
    IL_007c:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0081:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0086:  br.s       IL_0089

    IL_0088:  ldloc.1
    IL_0089:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_LogicalNot(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_008e:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0093:  stloc.3
    IL_0094:  ldloc.3
    IL_0095:  brfalse.s  IL_00a5

    IL_0097:  nop
    IL_0098:  ldc.i4.3
    IL_0099:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_009e:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_00a3:  nop
    IL_00a4:  nop
    IL_00a5:  ret
  } // end of method C::Test2

  .method private hidebysig static void  Test3() cil managed
  {
    // Code size       61 (0x3d)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0,
             bool V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_000d:  stloc.1
    IL_000e:  ldloc.1
    IL_000f:  brfalse.s  IL_001f

    IL_0011:  nop
    IL_0012:  ldloc.0
    IL_0013:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0018:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_001d:  nop
    IL_001e:  nop
    IL_001f:  ldloc.0
    IL_0020:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_LogicalNot(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0025:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_002a:  stloc.2
    IL_002b:  ldloc.2
    IL_002c:  brfalse.s  IL_003c

    IL_002e:  nop
    IL_002f:  ldloc.0
    IL_0030:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0035:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003a:  nop
    IL_003b:  nop
    IL_003c:  ret
  } // end of method C::Test3

  .method public hidebysig instance void 
          WithDynamic(object d) cil managed
  {
    .param [1]
    .custom instance void [System.Core]System.Runtime.CompilerServices.DynamicAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1116 (0x45c)
    .maxstack  14
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0,
             bool V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__2'
    IL_0006:  brfalse.s  IL_000a

    IL_0008:  br.s       IL_0049

    IL_000a:  ldc.i4     0x100
    IL_000f:  ldstr      "WriteLine"
    IL_0014:  ldnull
    IL_0015:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_001a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001f:  ldc.i4.2
    IL_0020:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0025:  dup
    IL_0026:  ldc.i4.0
    IL_0027:  ldc.i4.s   33
    IL_0029:  ldnull
    IL_002a:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_002f:  stelem.ref
    IL_0030:  dup
    IL_0031:  ldc.i4.1
    IL_0032:  ldc.i4.0
    IL_0033:  ldnull
    IL_0034:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0039:  stelem.ref
    IL_003a:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::InvokeMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                 string,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Type>,
                                                                                                                                                                 class [mscorlib]System.Type,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_003f:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0044:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__2'
    IL_0049:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__2'
    IL_004e:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Target
    IL_0053:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__2'
    IL_0058:  ldtoken    [mscorlib]System.Console
    IL_005d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0062:  ldc.i4.1
    IL_0063:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0068:  stloc.0
    IL_0069:  ldloc.0
    IL_006a:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_006f:  brtrue     IL_0116

    IL_0074:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__1'
    IL_0079:  brfalse.s  IL_007d

    IL_007b:  br.s       IL_00b2

    IL_007d:  ldc.i4.8
    IL_007e:  ldc.i4.2
    IL_007f:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_0084:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0089:  ldc.i4.2
    IL_008a:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_008f:  dup
    IL_0090:  ldc.i4.0
    IL_0091:  ldc.i4.1
    IL_0092:  ldnull
    IL_0093:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0098:  stelem.ref
    IL_0099:  dup
    IL_009a:  ldc.i4.1
    IL_009b:  ldc.i4.0
    IL_009c:  ldnull
    IL_009d:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_00a2:  stelem.ref
    IL_00a3:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_00a8:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_00ad:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__1'
    IL_00b2:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__1'
    IL_00b7:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Target
    IL_00bc:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__1'
    IL_00c1:  ldloc.0
    IL_00c2:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__0'
    IL_00c7:  brfalse.s  IL_00cb

    IL_00c9:  br.s       IL_00fa

    IL_00cb:  ldc.i4.0
    IL_00cc:  ldstr      "P"
    IL_00d1:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_00d6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00db:  ldc.i4.1
    IL_00dc:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_00e1:  dup
    IL_00e2:  ldc.i4.0
    IL_00e3:  ldc.i4.0
    IL_00e4:  ldnull
    IL_00e5:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_00ea:  stelem.ref
    IL_00eb:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_00f0:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_00f5:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__0'
    IL_00fa:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__0'
    IL_00ff:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_0104:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__0'
    IL_0109:  ldarg.1
    IL_010a:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_010f:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>::Invoke(!0,
                                                                                                                                                                                                                                        !1,
                                                                                                                                                                                                                                        !2)
    IL_0114:  br.s       IL_0117

    IL_0116:  ldloc.0
    IL_0117:  callvirt   instance void class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>::Invoke(!0,
                                                                                                                                                                               !1,
                                                                                                                                                                               !2)
    IL_011c:  nop
    IL_011d:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__5'
    IL_0122:  brfalse.s  IL_0126

    IL_0124:  br.s       IL_0165

    IL_0126:  ldc.i4     0x100
    IL_012b:  ldstr      "WriteLine"
    IL_0130:  ldnull
    IL_0131:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_0136:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_013b:  ldc.i4.2
    IL_013c:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0141:  dup
    IL_0142:  ldc.i4.0
    IL_0143:  ldc.i4.s   33
    IL_0145:  ldnull
    IL_0146:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_014b:  stelem.ref
    IL_014c:  dup
    IL_014d:  ldc.i4.1
    IL_014e:  ldc.i4.0
    IL_014f:  ldnull
    IL_0150:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0155:  stelem.ref
    IL_0156:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::InvokeMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                 string,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Type>,
                                                                                                                                                                 class [mscorlib]System.Type,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_015b:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0160:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__5'
    IL_0165:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__5'
    IL_016a:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Target
    IL_016f:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__5'
    IL_0174:  ldtoken    [mscorlib]System.Console
    IL_0179:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_017e:  ldc.i4.2
    IL_017f:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0184:  stloc.0
    IL_0185:  ldloc.0
    IL_0186:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_018b:  brtrue     IL_0233

    IL_0190:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__4'
    IL_0195:  brfalse.s  IL_0199

    IL_0197:  br.s       IL_01cf

    IL_0199:  ldc.i4.8
    IL_019a:  ldc.i4.s   36
    IL_019c:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_01a1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01a6:  ldc.i4.2
    IL_01a7:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_01ac:  dup
    IL_01ad:  ldc.i4.0
    IL_01ae:  ldc.i4.1
    IL_01af:  ldnull
    IL_01b0:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_01b5:  stelem.ref
    IL_01b6:  dup
    IL_01b7:  ldc.i4.1
    IL_01b8:  ldc.i4.0
    IL_01b9:  ldnull
    IL_01ba:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_01bf:  stelem.ref
    IL_01c0:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_01c5:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_01ca:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__4'
    IL_01cf:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__4'
    IL_01d4:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Target
    IL_01d9:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__4'
    IL_01de:  ldloc.0
    IL_01df:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__3'
    IL_01e4:  brfalse.s  IL_01e8

    IL_01e6:  br.s       IL_0217

    IL_01e8:  ldc.i4.0
    IL_01e9:  ldstr      "P"
    IL_01ee:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_01f3:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01f8:  ldc.i4.1
    IL_01f9:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_01fe:  dup
    IL_01ff:  ldc.i4.0
    IL_0200:  ldc.i4.0
    IL_0201:  ldnull
    IL_0202:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0207:  stelem.ref
    IL_0208:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_020d:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0212:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__3'
    IL_0217:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__3'
    IL_021c:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_0221:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__3'
    IL_0226:  ldarg.1
    IL_0227:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_022c:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>::Invoke(!0,
                                                                                                                                                                                                                                        !1,
                                                                                                                                                                                                                                        !2)
    IL_0231:  br.s       IL_0234

    IL_0233:  ldloc.0
    IL_0234:  callvirt   instance void class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>::Invoke(!0,
                                                                                                                                                                               !1,
                                                                                                                                                                               !2)
    IL_0239:  nop
    IL_023a:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__8'
    IL_023f:  brfalse.s  IL_0243

    IL_0241:  br.s       IL_026f

    IL_0243:  ldc.i4.0
    IL_0244:  ldc.i4.s   83
    IL_0246:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_024b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0250:  ldc.i4.1
    IL_0251:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0256:  dup
    IL_0257:  ldc.i4.0
    IL_0258:  ldc.i4.0
    IL_0259:  ldnull
    IL_025a:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_025f:  stelem.ref
    IL_0260:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::UnaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                   valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                   class [mscorlib]System.Type,
                                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0265:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_026a:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__8'
    IL_026f:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__8'
    IL_0274:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Target
    IL_0279:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__8'
    IL_027e:  ldc.i4.3
    IL_027f:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0284:  stloc.0
    IL_0285:  ldloc.0
    IL_0286:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_028b:  brtrue     IL_0332

    IL_0290:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__7'
    IL_0295:  brfalse.s  IL_0299

    IL_0297:  br.s       IL_02ce

    IL_0299:  ldc.i4.8
    IL_029a:  ldc.i4.2
    IL_029b:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_02a0:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02a5:  ldc.i4.2
    IL_02a6:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_02ab:  dup
    IL_02ac:  ldc.i4.0
    IL_02ad:  ldc.i4.1
    IL_02ae:  ldnull
    IL_02af:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_02b4:  stelem.ref
    IL_02b5:  dup
    IL_02b6:  ldc.i4.1
    IL_02b7:  ldc.i4.0
    IL_02b8:  ldnull
    IL_02b9:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_02be:  stelem.ref
    IL_02bf:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_02c4:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_02c9:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__7'
    IL_02ce:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__7'
    IL_02d3:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Target
    IL_02d8:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__7'
    IL_02dd:  ldloc.0
    IL_02de:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__6'
    IL_02e3:  brfalse.s  IL_02e7

    IL_02e5:  br.s       IL_0316

    IL_02e7:  ldc.i4.0
    IL_02e8:  ldstr      "P"
    IL_02ed:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_02f2:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02f7:  ldc.i4.1
    IL_02f8:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_02fd:  dup
    IL_02fe:  ldc.i4.0
    IL_02ff:  ldc.i4.0
    IL_0300:  ldnull
    IL_0301:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0306:  stelem.ref
    IL_0307:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_030c:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0311:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__6'
    IL_0316:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__6'
    IL_031b:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_0320:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__6'
    IL_0325:  ldarg.1
    IL_0326:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_032b:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>::Invoke(!0,
                                                                                                                                                                                                                                        !1,
                                                                                                                                                                                                                                        !2)
    IL_0330:  br.s       IL_0333

    IL_0332:  ldloc.0
    IL_0333:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>::Invoke(!0,
                                                                                                                                                    !1)
    IL_0338:  stloc.1
    IL_0339:  ldloc.1
    IL_033a:  brfalse.s  IL_034a

    IL_033c:  nop
    IL_033d:  ldc.i4.4
    IL_033e:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0343:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0348:  nop
    IL_0349:  nop
    IL_034a:  ldc.i4.5
    IL_034b:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0350:  stloc.0
    IL_0351:  ldloc.0
    IL_0352:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0357:  brtrue     IL_0448

    IL_035c:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__11'
    IL_0361:  brfalse.s  IL_0365

    IL_0363:  br.s       IL_0391

    IL_0365:  ldc.i4.0
    IL_0366:  ldc.i4.s   83
    IL_0368:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_036d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0372:  ldc.i4.1
    IL_0373:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0378:  dup
    IL_0379:  ldc.i4.0
    IL_037a:  ldc.i4.0
    IL_037b:  ldnull
    IL_037c:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0381:  stelem.ref
    IL_0382:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::UnaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                   valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                   class [mscorlib]System.Type,
                                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0387:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_038c:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__11'
    IL_0391:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__11'
    IL_0396:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Target
    IL_039b:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__11'
    IL_03a0:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__10'
    IL_03a5:  brfalse.s  IL_03a9

    IL_03a7:  br.s       IL_03df

    IL_03a9:  ldc.i4.8
    IL_03aa:  ldc.i4.s   36
    IL_03ac:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_03b1:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03b6:  ldc.i4.2
    IL_03b7:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_03bc:  dup
    IL_03bd:  ldc.i4.0
    IL_03be:  ldc.i4.1
    IL_03bf:  ldnull
    IL_03c0:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_03c5:  stelem.ref
    IL_03c6:  dup
    IL_03c7:  ldc.i4.1
    IL_03c8:  ldc.i4.0
    IL_03c9:  ldnull
    IL_03ca:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_03cf:  stelem.ref
    IL_03d0:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_03d5:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_03da:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__10'
    IL_03df:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__10'
    IL_03e4:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>>::Target
    IL_03e9:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__10'
    IL_03ee:  ldloc.0
    IL_03ef:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__9'
    IL_03f4:  brfalse.s  IL_03f8

    IL_03f6:  br.s       IL_0427

    IL_03f8:  ldc.i4.0
    IL_03f9:  ldstr      "P"
    IL_03fe:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C
    IL_0403:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0408:  ldc.i4.1
    IL_0409:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_040e:  dup
    IL_040f:  ldc.i4.0
    IL_0410:  ldc.i4.0
    IL_0411:  ldnull
    IL_0412:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0417:  stelem.ref
    IL_0418:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_041d:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0422:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__9'
    IL_0427:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__9'
    IL_042c:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_0431:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C/'<>o__10'::'<>p__9'
    IL_0436:  ldarg.1
    IL_0437:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_043c:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,object,object>::Invoke(!0,
                                                                                                                                                                                                                                        !1,
                                                                                                                                                                                                                                        !2)
    IL_0441:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>::Invoke(!0,
                                                                                                                                                    !1)
    IL_0446:  br.s       IL_0449

    IL_0448:  ldc.i4.1
    IL_0449:  stloc.2
    IL_044a:  ldloc.2
    IL_044b:  brfalse.s  IL_045b

    IL_044d:  nop
    IL_044e:  ldc.i4.6
    IL_044f:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0454:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0459:  nop
    IL_045a:  nop
    IL_045b:  ret
  } // end of method C::WithDynamic

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method C::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C

.class private sequential ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
       extends [mscorlib]System.ValueType
{
  .class abstract auto ansi sealed nested private beforefieldinit '<>o__11'
         extends [mscorlib]System.Object
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> '<>p__0'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> '<>p__1'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> '<>p__2'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> '<>p__3'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> '<>p__4'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> '<>p__5'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> '<>p__6'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> '<>p__7'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> '<>p__8'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> '<>p__9'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> '<>p__10'
    .field public static class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> '<>p__11'
  } // end of class '<>o__11'

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
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method S::op_True

  .method public hidebysig specialname static 
          bool  op_False(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S x) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  1
    .locals init (bool V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method S::op_False

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S 
          op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S x,
                        valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S y) cil managed
  {
    // Code size       24 (0x18)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0007:  ldarg.1
    IL_0008:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_000d:  and
    IL_000e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::.ctor(bool)
    IL_0013:  stloc.0
    IL_0014:  br.s       IL_0016

    IL_0016:  ldloc.0
    IL_0017:  ret
  } // end of method S::op_BitwiseAnd

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S 
          op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S x,
                       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S y) cil managed
  {
    // Code size       24 (0x18)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0007:  ldarg.1
    IL_0008:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_000d:  or
    IL_000e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::.ctor(bool)
    IL_0013:  stloc.0
    IL_0014:  br.s       IL_0016

    IL_0016:  ldloc.0
    IL_0017:  ret
  } // end of method S::op_BitwiseOr

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S 
          op_LogicalNot(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S x) cil managed
  {
    // Code size       20 (0x14)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0007:  ldc.i4.0
    IL_0008:  ceq
    IL_000a:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::.ctor(bool)
    IL_000f:  stloc.0
    IL_0010:  br.s       IL_0012

    IL_0012:  ldloc.0
    IL_0013:  ret
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
    // Code size       36 (0x24)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_False(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_000e:  brtrue.s   IL_001e

    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.2
    IL_0012:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0017:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,
                                                                                                                                                                                                          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_001c:  br.s       IL_001f

    IL_001e:  ldloc.0
    IL_001f:  stloc.1
    IL_0020:  br.s       IL_0022

    IL_0022:  ldloc.1
    IL_0023:  ret
  } // end of method S::LogicAnd

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S 
          LogicOr() cil managed
  {
    // Code size       36 (0x24)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_000e:  brtrue.s   IL_001e

    IL_0010:  ldloc.0
    IL_0011:  ldc.i4.2
    IL_0012:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0017:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,
                                                                                                                                                                                                         valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_001c:  br.s       IL_001f

    IL_001e:  ldloc.0
    IL_001f:  stloc.1
    IL_0020:  br.s       IL_0022

    IL_0022:  ldloc.1
    IL_0023:  ret
  } // end of method S::LogicOr

  .method public hidebysig instance void 
          InConditionDetection() cil managed
  {
    // Code size       147 (0x93)
    .maxstack  2
    .locals init (bool V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldstr      "a"
    IL_0006:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000b:  nop
    IL_000c:  ldc.i4.1
    IL_000d:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0012:  stloc.1
    IL_0013:  ldloc.1
    IL_0014:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_False(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0019:  brtrue.s   IL_0029

    IL_001b:  ldloc.1
    IL_001c:  ldc.i4.2
    IL_001d:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0022:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,
                                                                                                                                                                                                          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0027:  br.s       IL_002a

    IL_0029:  ldloc.1
    IL_002a:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_002f:  stloc.0
    IL_0030:  ldloc.0
    IL_0031:  brfalse.s  IL_0042

    IL_0033:  nop
    IL_0034:  ldstr      "b"
    IL_0039:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003e:  nop
    IL_003f:  nop
    IL_0040:  br.s       IL_004f

    IL_0042:  nop
    IL_0043:  ldstr      "c"
    IL_0048:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_004d:  nop
    IL_004e:  nop
    IL_004f:  ldc.i4.1
    IL_0050:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0055:  stloc.1
    IL_0056:  ldloc.1
    IL_0057:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_005c:  brtrue.s   IL_006c

    IL_005e:  ldloc.1
    IL_005f:  ldc.i4.2
    IL_0060:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0065:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,
                                                                                                                                                                                                         valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_006a:  br.s       IL_006d

    IL_006c:  ldloc.1
    IL_006d:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0072:  stloc.2
    IL_0073:  ldloc.2
    IL_0074:  brfalse.s  IL_0085

    IL_0076:  nop
    IL_0077:  ldstr      "d"
    IL_007c:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0081:  nop
    IL_0082:  nop
    IL_0083:  br.s       IL_0092

    IL_0085:  nop
    IL_0086:  ldstr      "e"
    IL_008b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0090:  nop
    IL_0091:  nop
    IL_0092:  ret
  } // end of method S::InConditionDetection

  .method public hidebysig instance void 
          WithDynamic(object d) cil managed
  {
    .param [1]
    .custom instance void [System.Core]System.Runtime.CompilerServices.DynamicAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       1141 (0x475)
    .maxstack  14
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_0,
             bool V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__2'
    IL_0006:  brfalse.s  IL_000a

    IL_0008:  br.s       IL_0049

    IL_000a:  ldc.i4     0x100
    IL_000f:  ldstr      "WriteLine"
    IL_0014:  ldnull
    IL_0015:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_001a:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_001f:  ldc.i4.2
    IL_0020:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0025:  dup
    IL_0026:  ldc.i4.0
    IL_0027:  ldc.i4.s   33
    IL_0029:  ldnull
    IL_002a:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_002f:  stelem.ref
    IL_0030:  dup
    IL_0031:  ldc.i4.1
    IL_0032:  ldc.i4.0
    IL_0033:  ldnull
    IL_0034:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0039:  stelem.ref
    IL_003a:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::InvokeMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                 string,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Type>,
                                                                                                                                                                 class [mscorlib]System.Type,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_003f:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0044:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__2'
    IL_0049:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__2'
    IL_004e:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Target
    IL_0053:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__2'
    IL_0058:  ldtoken    [mscorlib]System.Console
    IL_005d:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0062:  ldc.i4.1
    IL_0063:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0068:  stloc.0
    IL_0069:  ldloc.0
    IL_006a:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_False(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_006f:  brtrue     IL_0116

    IL_0074:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__1'
    IL_0079:  brfalse.s  IL_007d

    IL_007b:  br.s       IL_00b2

    IL_007d:  ldc.i4.8
    IL_007e:  ldc.i4.2
    IL_007f:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0084:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0089:  ldc.i4.2
    IL_008a:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_008f:  dup
    IL_0090:  ldc.i4.0
    IL_0091:  ldc.i4.1
    IL_0092:  ldnull
    IL_0093:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0098:  stelem.ref
    IL_0099:  dup
    IL_009a:  ldc.i4.1
    IL_009b:  ldc.i4.0
    IL_009c:  ldnull
    IL_009d:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_00a2:  stelem.ref
    IL_00a3:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_00a8:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_00ad:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__1'
    IL_00b2:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__1'
    IL_00b7:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Target
    IL_00bc:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__1'
    IL_00c1:  ldloc.0
    IL_00c2:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__0'
    IL_00c7:  brfalse.s  IL_00cb

    IL_00c9:  br.s       IL_00fa

    IL_00cb:  ldc.i4.0
    IL_00cc:  ldstr      "P"
    IL_00d1:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_00d6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_00db:  ldc.i4.1
    IL_00dc:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_00e1:  dup
    IL_00e2:  ldc.i4.0
    IL_00e3:  ldc.i4.0
    IL_00e4:  ldnull
    IL_00e5:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_00ea:  stelem.ref
    IL_00eb:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_00f0:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_00f5:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__0'
    IL_00fa:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__0'
    IL_00ff:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_0104:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__0'
    IL_0109:  ldarg.1
    IL_010a:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_010f:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>::Invoke(!0,
                                                                                                                                                                                                                                            !1,
                                                                                                                                                                                                                                            !2)
    IL_0114:  br.s       IL_011c

    IL_0116:  ldloc.0
    IL_0117:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_011c:  callvirt   instance void class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>::Invoke(!0,
                                                                                                                                                                               !1,
                                                                                                                                                                               !2)
    IL_0121:  nop
    IL_0122:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__5'
    IL_0127:  brfalse.s  IL_012b

    IL_0129:  br.s       IL_016a

    IL_012b:  ldc.i4     0x100
    IL_0130:  ldstr      "WriteLine"
    IL_0135:  ldnull
    IL_0136:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_013b:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0140:  ldc.i4.2
    IL_0141:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0146:  dup
    IL_0147:  ldc.i4.0
    IL_0148:  ldc.i4.s   33
    IL_014a:  ldnull
    IL_014b:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0150:  stelem.ref
    IL_0151:  dup
    IL_0152:  ldc.i4.1
    IL_0153:  ldc.i4.0
    IL_0154:  ldnull
    IL_0155:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_015a:  stelem.ref
    IL_015b:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::InvokeMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                 string,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [mscorlib]System.Type>,
                                                                                                                                                                 class [mscorlib]System.Type,
                                                                                                                                                                 class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0160:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0165:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__5'
    IL_016a:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__5'
    IL_016f:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>>::Target
    IL_0174:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__5'
    IL_0179:  ldtoken    [mscorlib]System.Console
    IL_017e:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0183:  ldc.i4.2
    IL_0184:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0189:  stloc.0
    IL_018a:  ldloc.0
    IL_018b:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0190:  brtrue     IL_0238

    IL_0195:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__4'
    IL_019a:  brfalse.s  IL_019e

    IL_019c:  br.s       IL_01d4

    IL_019e:  ldc.i4.8
    IL_019f:  ldc.i4.s   36
    IL_01a1:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_01a6:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01ab:  ldc.i4.2
    IL_01ac:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_01b1:  dup
    IL_01b2:  ldc.i4.0
    IL_01b3:  ldc.i4.1
    IL_01b4:  ldnull
    IL_01b5:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_01ba:  stelem.ref
    IL_01bb:  dup
    IL_01bc:  ldc.i4.1
    IL_01bd:  ldc.i4.0
    IL_01be:  ldnull
    IL_01bf:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_01c4:  stelem.ref
    IL_01c5:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_01ca:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_01cf:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__4'
    IL_01d4:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__4'
    IL_01d9:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Target
    IL_01de:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__4'
    IL_01e3:  ldloc.0
    IL_01e4:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__3'
    IL_01e9:  brfalse.s  IL_01ed

    IL_01eb:  br.s       IL_021c

    IL_01ed:  ldc.i4.0
    IL_01ee:  ldstr      "P"
    IL_01f3:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_01f8:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_01fd:  ldc.i4.1
    IL_01fe:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0203:  dup
    IL_0204:  ldc.i4.0
    IL_0205:  ldc.i4.0
    IL_0206:  ldnull
    IL_0207:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_020c:  stelem.ref
    IL_020d:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0212:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0217:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__3'
    IL_021c:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__3'
    IL_0221:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_0226:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__3'
    IL_022b:  ldarg.1
    IL_022c:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_0231:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>::Invoke(!0,
                                                                                                                                                                                                                                            !1,
                                                                                                                                                                                                                                            !2)
    IL_0236:  br.s       IL_023e

    IL_0238:  ldloc.0
    IL_0239:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_023e:  callvirt   instance void class [mscorlib]System.Action`3<class [System.Core]System.Runtime.CompilerServices.CallSite,class [mscorlib]System.Type,object>::Invoke(!0,
                                                                                                                                                                               !1,
                                                                                                                                                                               !2)
    IL_0243:  nop
    IL_0244:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__8'
    IL_0249:  brfalse.s  IL_024d

    IL_024b:  br.s       IL_0279

    IL_024d:  ldc.i4.0
    IL_024e:  ldc.i4.s   83
    IL_0250:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0255:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_025a:  ldc.i4.1
    IL_025b:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0260:  dup
    IL_0261:  ldc.i4.0
    IL_0262:  ldc.i4.0
    IL_0263:  ldnull
    IL_0264:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0269:  stelem.ref
    IL_026a:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::UnaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                   valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                   class [mscorlib]System.Type,
                                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_026f:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0274:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__8'
    IL_0279:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__8'
    IL_027e:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Target
    IL_0283:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__8'
    IL_0288:  ldc.i4.3
    IL_0289:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_028e:  stloc.0
    IL_028f:  ldloc.0
    IL_0290:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_False(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0295:  brtrue     IL_033c

    IL_029a:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__7'
    IL_029f:  brfalse.s  IL_02a3

    IL_02a1:  br.s       IL_02d8

    IL_02a3:  ldc.i4.8
    IL_02a4:  ldc.i4.2
    IL_02a5:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_02aa:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_02af:  ldc.i4.2
    IL_02b0:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_02b5:  dup
    IL_02b6:  ldc.i4.0
    IL_02b7:  ldc.i4.1
    IL_02b8:  ldnull
    IL_02b9:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_02be:  stelem.ref
    IL_02bf:  dup
    IL_02c0:  ldc.i4.1
    IL_02c1:  ldc.i4.0
    IL_02c2:  ldnull
    IL_02c3:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_02c8:  stelem.ref
    IL_02c9:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_02ce:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_02d3:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__7'
    IL_02d8:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__7'
    IL_02dd:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Target
    IL_02e2:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__7'
    IL_02e7:  ldloc.0
    IL_02e8:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__6'
    IL_02ed:  brfalse.s  IL_02f1

    IL_02ef:  br.s       IL_0320

    IL_02f1:  ldc.i4.0
    IL_02f2:  ldstr      "P"
    IL_02f7:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_02fc:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0301:  ldc.i4.1
    IL_0302:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0307:  dup
    IL_0308:  ldc.i4.0
    IL_0309:  ldc.i4.0
    IL_030a:  ldnull
    IL_030b:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0310:  stelem.ref
    IL_0311:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0316:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_031b:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__6'
    IL_0320:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__6'
    IL_0325:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_032a:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__6'
    IL_032f:  ldarg.1
    IL_0330:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_0335:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>::Invoke(!0,
                                                                                                                                                                                                                                            !1,
                                                                                                                                                                                                                                            !2)
    IL_033a:  br.s       IL_0342

    IL_033c:  ldloc.0
    IL_033d:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0342:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>::Invoke(!0,
                                                                                                                                                    !1)
    IL_0347:  stloc.1
    IL_0348:  ldloc.1
    IL_0349:  brfalse.s  IL_035e

    IL_034b:  nop
    IL_034c:  ldc.i4.4
    IL_034d:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0352:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0357:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_035c:  nop
    IL_035d:  nop
    IL_035e:  ldc.i4.5
    IL_035f:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0364:  stloc.0
    IL_0365:  ldloc.0
    IL_0366:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_036b:  brtrue     IL_045c

    IL_0370:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__11'
    IL_0375:  brfalse.s  IL_0379

    IL_0377:  br.s       IL_03a5

    IL_0379:  ldc.i4.0
    IL_037a:  ldc.i4.s   83
    IL_037c:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0381:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_0386:  ldc.i4.1
    IL_0387:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_038c:  dup
    IL_038d:  ldc.i4.0
    IL_038e:  ldc.i4.0
    IL_038f:  ldnull
    IL_0390:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_0395:  stelem.ref
    IL_0396:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::UnaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                   valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                   class [mscorlib]System.Type,
                                                                                                                                                                   class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_039b:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_03a0:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__11'
    IL_03a5:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__11'
    IL_03aa:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>>::Target
    IL_03af:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__11'
    IL_03b4:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__10'
    IL_03b9:  brfalse.s  IL_03bd

    IL_03bb:  br.s       IL_03f3

    IL_03bd:  ldc.i4.8
    IL_03be:  ldc.i4.s   36
    IL_03c0:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_03c5:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_03ca:  ldc.i4.2
    IL_03cb:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_03d0:  dup
    IL_03d1:  ldc.i4.0
    IL_03d2:  ldc.i4.1
    IL_03d3:  ldnull
    IL_03d4:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_03d9:  stelem.ref
    IL_03da:  dup
    IL_03db:  ldc.i4.1
    IL_03dc:  ldc.i4.0
    IL_03dd:  ldnull
    IL_03de:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_03e3:  stelem.ref
    IL_03e4:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::BinaryOperation(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                                    valuetype [System.Core]System.Linq.Expressions.ExpressionType,
                                                                                                                                                                    class [mscorlib]System.Type,
                                                                                                                                                                    class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_03e9:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_03ee:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__10'
    IL_03f3:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__10'
    IL_03f8:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>>::Target
    IL_03fd:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__10'
    IL_0402:  ldloc.0
    IL_0403:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__9'
    IL_0408:  brfalse.s  IL_040c

    IL_040a:  br.s       IL_043b

    IL_040c:  ldc.i4.0
    IL_040d:  ldstr      "P"
    IL_0412:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_0417:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_041c:  ldc.i4.1
    IL_041d:  newarr     [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo
    IL_0422:  dup
    IL_0423:  ldc.i4.0
    IL_0424:  ldc.i4.0
    IL_0425:  ldnull
    IL_0426:  call       class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo::Create(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfoFlags,
                                                                                                                                                                               string)
    IL_042b:  stelem.ref
    IL_042c:  call       class [System.Core]System.Runtime.CompilerServices.CallSiteBinder [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.Binder::GetMember(valuetype [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpBinderFlags,
                                                                                                                                                              string,
                                                                                                                                                              class [mscorlib]System.Type,
                                                                                                                                                              class [mscorlib]System.Collections.Generic.IEnumerable`1<class [Microsoft.CSharp]Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>)
    IL_0431:  call       class [System.Core]System.Runtime.CompilerServices.CallSite`1<!0> class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Create(class [System.Core]System.Runtime.CompilerServices.CallSiteBinder)
    IL_0436:  stsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__9'
    IL_043b:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__9'
    IL_0440:  ldfld      !0 class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>>::Target
    IL_0445:  ldsfld     class [System.Core]System.Runtime.CompilerServices.CallSite`1<class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>> ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S/'<>o__11'::'<>p__9'
    IL_044a:  ldarg.1
    IL_044b:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,object>::Invoke(!0,
                                                                                                                                                      !1)
    IL_0450:  callvirt   instance !3 class [mscorlib]System.Func`4<class [System.Core]System.Runtime.CompilerServices.CallSite,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,object,object>::Invoke(!0,
                                                                                                                                                                                                                                            !1,
                                                                                                                                                                                                                                            !2)
    IL_0455:  callvirt   instance !2 class [mscorlib]System.Func`3<class [System.Core]System.Runtime.CompilerServices.CallSite,object,bool>::Invoke(!0,
                                                                                                                                                    !1)
    IL_045a:  br.s       IL_045d

    IL_045c:  ldc.i4.1
    IL_045d:  stloc.2
    IL_045e:  ldloc.2
    IL_045f:  brfalse.s  IL_0474

    IL_0461:  nop
    IL_0462:  ldc.i4.6
    IL_0463:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0468:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S
    IL_046d:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0472:  nop
    IL_0473:  nop
    IL_0474:  ret
  } // end of method S::WithDynamic

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
