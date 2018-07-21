



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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

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
    // Code size       30 (0x1e)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0)
    IL_0000:  ldc.i4.1
    IL_0001:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_000d:  brtrue.s   IL_001c

    IL_000f:  ldloc.0
    IL_0010:  ldc.i4.2
    IL_0011:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0016:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_001b:  ret

    IL_001c:  ldloc.0
    IL_001d:  ret
  } // end of method C::LogicAnd

  .method public hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C 
          LogicOr() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0)
    IL_0000:  ldc.i4.1
    IL_0001:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_000d:  brtrue.s   IL_001c

    IL_000f:  ldloc.0
    IL_0010:  ldc.i4.2
    IL_0011:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0016:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseOr(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_001b:  ret

    IL_001c:  ldloc.0
    IL_001d:  ret
  } // end of method C::LogicOr

  .method public hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C 
          Complex() cil managed
  {
    // Code size       107 (0x6b)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_2)
    IL_0000:  ldc.i4.1
    IL_0001:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0006:  stloc.2
    IL_0007:  ldloc.2
    IL_0008:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_000d:  brtrue.s   IL_001d

    IL_000f:  ldloc.2
    IL_0010:  ldc.i4.2
    IL_0011:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0016:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseOr(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_001b:  br.s       IL_001e

    IL_001d:  ldloc.2
    IL_001e:  stloc.1
    IL_001f:  ldloc.1
    IL_0020:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0025:  brtrue.s   IL_0035

    IL_0027:  ldloc.1
    IL_0028:  ldc.i4.3
    IL_0029:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_002e:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0033:  br.s       IL_0036

    IL_0035:  ldloc.1
    IL_0036:  stloc.0
    IL_0037:  ldloc.0
    IL_0038:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_003d:  brtrue.s   IL_0069

    IL_003f:  ldloc.0
    IL_0040:  ldc.i4.4
    IL_0041:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0046:  stloc.1
    IL_0047:  ldloc.1
    IL_0048:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_004d:  brtrue.s   IL_005d

    IL_004f:  ldloc.1
    IL_0050:  ldc.i4.5
    IL_0051:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::GetC(int32)
    IL_0056:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseOr(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_005b:  br.s       IL_005e

    IL_005d:  ldloc.1
    IL_005e:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_LogicalNot(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0063:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0068:  ret

    IL_0069:  ldloc.0
    IL_006a:  ret
  } // end of method C::Complex

  .method private hidebysig static void  Main() cil managed
  {
    // Code size       73 (0x49)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_2)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::.ctor()
    IL_0005:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::.ctor()
    IL_000a:  stloc.0
    IL_000b:  dup
    IL_000c:  stloc.2
    IL_000d:  ldloc.2
    IL_000e:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0013:  brtrue.s   IL_001e

    IL_0015:  ldloc.2
    IL_0016:  ldloc.0
    IL_0017:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_001c:  br.s       IL_001f

    IL_001e:  ldloc.2
    IL_001f:  stloc.1
    IL_0020:  stloc.2
    IL_0021:  ldloc.2
    IL_0022:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0027:  brtrue.s   IL_0032

    IL_0029:  ldloc.2
    IL_002a:  ldloc.0
    IL_002b:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseOr(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                     class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0030:  br.s       IL_0033

    IL_0032:  ldloc.2
    IL_0033:  ldloc.1
    IL_0034:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0039:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_003e:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0043:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0048:  ret
  } // end of method C::Main

  .method private hidebysig static void  Test2() cil managed
  {
    // Code size       88 (0x58)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  stloc.1
    IL_0008:  ldloc.1
    IL_0009:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_000e:  brtrue.s   IL_0019

    IL_0010:  ldloc.1
    IL_0011:  ldloc.0
    IL_0012:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0017:  br.s       IL_001a

    IL_0019:  ldloc.1
    IL_001a:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_001f:  brfalse.s  IL_002c

    IL_0021:  ldloc.0
    IL_0022:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0027:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_002c:  ldloc.0
    IL_002d:  stloc.1
    IL_002e:  ldloc.1
    IL_002f:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_False(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_0034:  brtrue.s   IL_003f

    IL_0036:  ldloc.1
    IL_0037:  ldloc.0
    IL_0038:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_BitwiseAnd(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C,
                                                                                                                                                                                                      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_003d:  br.s       IL_0040

    IL_003f:  ldloc.1
    IL_0040:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C::op_LogicalNot(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.C)
    IL_0045:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass::op_True(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.BaseClass)
    IL_004a:  brfalse.s  IL_0057

    IL_004c:  ldloc.0
    IL_004d:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0052:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0057:  ret
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
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0006:  ret
  } // end of method S::op_True

  .method public hidebysig specialname static 
          bool  op_False(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S x) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0006:  ret
  } // end of method S::op_False

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S 
          op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S x,
                        valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S y) cil managed
  {
    // Code size       19 (0x13)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0006:  ldarg.1
    IL_0007:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_000c:  and
    IL_000d:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::.ctor(bool)
    IL_0012:  ret
  } // end of method S::op_BitwiseAnd

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S 
          op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S x,
                       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S y) cil managed
  {
    // Code size       19 (0x13)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0006:  ldarg.1
    IL_0007:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_000c:  or
    IL_000d:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::.ctor(bool)
    IL_0012:  ret
  } // end of method S::op_BitwiseOr

  .method public hidebysig specialname static 
          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S 
          op_LogicalNot(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S x) cil managed
  {
    // Code size       15 (0xf)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::val
    IL_0006:  ldc.i4.0
    IL_0007:  ceq
    IL_0009:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::.ctor(bool)
    IL_000e:  ret
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
    // Code size       30 (0x1e)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_0)
    IL_0000:  ldc.i4.1
    IL_0001:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_False(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_000d:  brtrue.s   IL_001c

    IL_000f:  ldloc.0
    IL_0010:  ldc.i4.2
    IL_0011:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0016:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,
                                                                                                                                                                                                          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_001b:  ret

    IL_001c:  ldloc.0
    IL_001d:  ret
  } // end of method S::LogicAnd

  .method public hidebysig static valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S 
          LogicOr() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_0)
    IL_0000:  ldc.i4.1
    IL_0001:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_000d:  brtrue.s   IL_001c

    IL_000f:  ldloc.0
    IL_0010:  ldc.i4.2
    IL_0011:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0016:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,
                                                                                                                                                                                                         valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_001b:  ret

    IL_001c:  ldloc.0
    IL_001d:  ret
  } // end of method S::LogicOr

  .method public hidebysig instance void 
          InConditionDetection() cil managed
  {
    // Code size       128 (0x80)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S V_0)
    IL_0000:  ldstr      "a"
    IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_000a:  ldc.i4.1
    IL_000b:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0010:  stloc.0
    IL_0011:  ldloc.0
    IL_0012:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_False(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0017:  brtrue.s   IL_0027

    IL_0019:  ldloc.0
    IL_001a:  ldc.i4.2
    IL_001b:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_0020:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_BitwiseAnd(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,
                                                                                                                                                                                                          valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0025:  br.s       IL_0028

    IL_0027:  ldloc.0
    IL_0028:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_002d:  brfalse.s  IL_003b

    IL_002f:  ldstr      "b"
    IL_0034:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0039:  br.s       IL_0045

    IL_003b:  ldstr      "c"
    IL_0040:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0045:  ldc.i4.1
    IL_0046:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_004b:  stloc.0
    IL_004c:  ldloc.0
    IL_004d:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0052:  brtrue.s   IL_0062

    IL_0054:  ldloc.0
    IL_0055:  ldc.i4.2
    IL_0056:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::Get(int32)
    IL_005b:  call       valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_BitwiseOr(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S,
                                                                                                                                                                                                         valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0060:  br.s       IL_0063

    IL_0062:  ldloc.0
    IL_0063:  call       bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S::op_True(valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S)
    IL_0068:  brfalse.s  IL_0075

    IL_006a:  ldstr      "d"
    IL_006f:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_0074:  ret

    IL_0075:  ldstr      "e"
    IL_007a:  call       void [mscorlib]System.Console::WriteLine(string)
    IL_007f:  ret
  } // end of method S::InConditionDetection

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.CustomShortCircuitOperators.S


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
