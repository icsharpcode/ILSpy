
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation. Alle Rechte vorbehalten.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly ecxzpbak
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module ecxzpbak.dll
// MVID: {F67EEE6A-7EFF-4261-9FC7-3F2C4A0972E9}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02430000


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
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
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
      // Code size       15 (0xf)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::A
      IL_0007:  ldarg.0
      IL_0008:  ldc.i4.0
      IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::B
      IL_000e:  ret
    } // end of method S::.ctor

  } // end of class S

  .method public hidebysig static class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C 
          TestCall(int32 a,
                   class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C c) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  ret
  } // end of method InitializerTests::TestCall

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C 
          Test1() cil managed
  {
    // Code size       36 (0x24)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S>::.ctor()
    IL_000c:  stfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::L
    IL_0011:  ldloc.0
    IL_0012:  ldfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::L
    IL_0017:  ldc.i4.1
    IL_0018:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::.ctor(int32)
    IL_001d:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S>::Add(!0)
    IL_0022:  ldloc.0
    IL_0023:  ret
  } // end of method InitializerTests::Test1

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C 
          Test1Alternative() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  4
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_0,
             class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S> V_1)
    IL_0000:  ldc.i4.1
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  newobj     instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S>::.ctor()
    IL_000d:  stloc.1
    IL_000e:  ldloc.1
    IL_000f:  ldc.i4.1
    IL_0010:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::.ctor(int32)
    IL_0015:  callvirt   instance void class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S>::Add(!0)
    IL_001a:  ldloc.1
    IL_001b:  stfld      class [mscorlib]System.Collections.Generic.List`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S> ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::L
    IL_0020:  ldloc.0
    IL_0021:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::TestCall(int32,
                                                                                                                                                                         class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C)
    IL_0026:  ret
  } // end of method InitializerTests::Test1Alternative

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C 
          Test2() cil managed
  {
    // Code size       22 (0x16)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Z
    IL_000d:  ldloc.0
    IL_000e:  ldc.i4.2
    IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Z
    IL_0014:  ldloc.0
    IL_0015:  ret
  } // end of method InitializerTests::Test2

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C 
          Test3() cil managed
  {
    // Code size       32 (0x20)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.1
    IL_0008:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::.ctor(int32)
    IL_000d:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Y
    IL_0012:  ldloc.0
    IL_0013:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Y
    IL_0018:  ldc.i4.2
    IL_0019:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::A
    IL_001e:  ldloc.0
    IL_001f:  ret
  } // end of method InitializerTests::Test3

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C 
          Test3b() cil managed
  {
    // Code size       33 (0x21)
    .maxstack  3
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  ldc.i4.1
    IL_0009:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Z
    IL_000e:  ldloc.0
    IL_000f:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Y
    IL_0014:  ldc.i4.2
    IL_0015:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::A
    IL_001a:  ldloc.0
    IL_001b:  call       class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests::TestCall(int32,
                                                                                                                                                                         class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C)
    IL_0020:  ret
  } // end of method InitializerTests::Test3b

  .method public hidebysig instance class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C 
          Test4() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C V_0)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Y
    IL_000c:  ldc.i4.1
    IL_000d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::A
    IL_0012:  ldloc.0
    IL_0013:  ldc.i4.2
    IL_0014:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Z
    IL_0019:  ldloc.0
    IL_001a:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/C::Y
    IL_001f:  ldc.i4.3
    IL_0020:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests/S::B
    IL_0025:  ldloc.0
    IL_0026:  ret
  } // end of method InitializerTests::Test4

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method InitializerTests::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InitializerTests


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// Warnung: Win32-Ressourcendatei "../../../TestCases/Pretty\InitializerTests.opt.res" wurde erstellt.
