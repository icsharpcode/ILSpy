



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
    // Code size       95 (0x5f)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  dup
    IL_0009:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_000e:  brtrue.s   IL_0016

    IL_0010:  ldloc.0
    IL_0011:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0016:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_001b:  ldc.i4.0
    IL_001c:  ceq
    IL_001e:  stloc.1
    IL_001f:  ldloc.1
    IL_0020:  brtrue.s   IL_0030

    IL_0022:  nop
    IL_0023:  ldloc.0
    IL_0024:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0029:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002e:  nop
    IL_002f:  nop
    IL_0030:  ldloc.0
    IL_0031:  dup
    IL_0032:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0037:  brtrue.s   IL_003f

    IL_0039:  ldloc.0
    IL_003a:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_003f:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_LogicalNot(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0044:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0049:  ldc.i4.0
    IL_004a:  ceq
    IL_004c:  stloc.1
    IL_004d:  ldloc.1
    IL_004e:  brtrue.s   IL_005e

    IL_0050:  nop
    IL_0051:  ldloc.0
    IL_0052:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0057:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_005c:  nop
    IL_005d:  nop
    IL_005e:  ret
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

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
