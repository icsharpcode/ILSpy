



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
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
    // Code size       99 (0x63)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0,
             bool V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_2,
             bool V_3)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.2
    IL_0009:  ldloc.2
    IL_000a:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_000f:  brtrue.s   IL_001a

    IL_0011:  ldloc.2
    IL_0012:  ldloc.0
    IL_0013:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0018:  br.s       IL_001b

    IL_001a:  ldloc.2
    IL_001b:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0020:  stloc.1
    IL_0021:  ldloc.1
    IL_0022:  brfalse.s  IL_0032

    IL_0024:  nop
    IL_0025:  ldloc.0
    IL_0026:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_002b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0030:  nop
    IL_0031:  nop
    IL_0032:  ldloc.0
    IL_0033:  stloc.2
    IL_0034:  ldloc.2
    IL_0035:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_003a:  brtrue.s   IL_0045

    IL_003c:  ldloc.2
    IL_003d:  ldloc.0
    IL_003e:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0043:  br.s       IL_0046

    IL_0045:  ldloc.2
    IL_0046:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_LogicalNot(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_004b:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0050:  stloc.3
    IL_0051:  ldloc.3
    IL_0052:  brfalse.s  IL_0062

    IL_0054:  nop
    IL_0055:  ldloc.0
    IL_0056:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_005b:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0060:  nop
    IL_0061:  nop
    IL_0062:  ret
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

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
