
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation. Alle Rechte vorbehalten.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly InitializerTests
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
.module InitializerTests.dll
// MVID: {7000C1D7-C4EC-4376-B053-4BE74428CE7A}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x005E0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests
       extends [mscorlib]System.Object
{
  .class auto ansi nested public beforefieldinit C
         extends [mscorlib]System.Object
  {
    .field public int32 Z
    .field public valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S Y
    .field public class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S> L
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method C::.ctor

  } // end of class C

  .class sequential ansi sealed nested public beforefieldinit S
         extends [mscorlib]System.ValueType
  {
    .field public int32 A
    .field public int32 B
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(int32 a) cil managed
    {
      // Code size       16 (0x10)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldarg.1
      IL_0003:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::A
      IL_0008:  ldarg.0
      IL_0009:  ldc.i4.0
      IL_000a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::B
      IL_000f:  ret
    } // end of method S::.ctor

  } // end of class S

  .method public hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C 
          TestCall(int32 a,
                   class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C c) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method InitializerTests::TestCall

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C 
          Test1() cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_1)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S>::.ctor()
    IL_000d:  stfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::L
    IL_0012:  ldloc.0
    IL_0013:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::L
    IL_0018:  ldc.i4.1
    IL_0019:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::.ctor(int32)
    IL_001e:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S>::Add(!0)
    IL_0023:  nop
    IL_0024:  ldloc.0
    IL_0025:  stloc.1
    IL_0026:  br.s       IL_0028

    IL_0028:  ldloc.1
    IL_0029:  ret
  } // end of method InitializerTests::Test1

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C 
          Test1Alternative() cil managed
  {
    // Code size       41 (0x29)
    .maxstack  6
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::.ctor()
    IL_0007:  dup
    IL_0008:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S>::.ctor()
    IL_000d:  dup
    IL_000e:  ldc.i4.1
    IL_000f:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::.ctor(int32)
    IL_0014:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S>::Add(!0)
    IL_0019:  nop
    IL_001a:  stfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::L
    IL_001f:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::TestCall(int32,
                                                                                                                                                                         class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C)
    IL_0024:  stloc.0
    IL_0025:  br.s       IL_0027

    IL_0027:  ldloc.0
    IL_0028:  ret
  } // end of method InitializerTests::Test1Alternative

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C 
          Test2() cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_1)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Z
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.2
    IL_0010:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Z
    IL_0015:  ldloc.0
    IL_0016:  stloc.1
    IL_0017:  br.s       IL_0019

    IL_0019:  ldloc.1
    IL_001a:  ret
  } // end of method InitializerTests::Test2

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C 
          Test3() cil managed
  {
    // Code size       37 (0x25)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_1)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::.ctor(int32)
    IL_000e:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Y
    IL_0013:  ldloc.0
    IL_0014:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Y
    IL_0019:  ldc.i4.2
    IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::A
    IL_001f:  ldloc.0
    IL_0020:  stloc.1
    IL_0021:  br.s       IL_0023

    IL_0023:  ldloc.1
    IL_0024:  ret
  } // end of method InitializerTests::Test3

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C 
          Test3b() cil managed
  {
    // Code size       36 (0x24)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::.ctor()
    IL_0007:  dup
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Z
    IL_000e:  dup
    IL_000f:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Y
    IL_0014:  ldc.i4.2
    IL_0015:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::A
    IL_001a:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::TestCall(int32,
                                                                                                                                                                         class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C)
    IL_001f:  stloc.0
    IL_0020:  br.s       IL_0022

    IL_0022:  ldloc.0
    IL_0023:  ret
  } // end of method InitializerTests::Test3b

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C 
          Test4() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_1)
    IL_0000:  nop
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Y
    IL_000d:  ldc.i4.1
    IL_000e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::A
    IL_0013:  ldloc.0
    IL_0014:  ldc.i4.2
    IL_0015:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Z
    IL_001a:  ldloc.0
    IL_001b:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Y
    IL_0020:  ldc.i4.3
    IL_0021:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::B
    IL_0026:  ldloc.0
    IL_0027:  stloc.1
    IL_0028:  br.s       IL_002a

    IL_002a:  ldloc.1
    IL_002b:  ret
  } // end of method InitializerTests::Test4

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method InitializerTests::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
