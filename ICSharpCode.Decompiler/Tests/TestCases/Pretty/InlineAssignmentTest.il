
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly '3z5tjxm2'
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module '3z5tjxm2.exe'
// MVID: {3B1EB063-50AF-4CD4-83C3-9D26A66D9510}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x011D0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest
       extends [mscorlib]System.Object
{
  .field private int32 field1
  .field private class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest field2
  .method public hidebysig static void  Main() cil managed
  {
    .entrypoint
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method InlineAssignmentTest::Main

  .method public hidebysig instance void 
          SimpleInlineWithLocals() cil managed
  {
    // Code size       38 (0x26)
    .maxstack  2
    .locals init (int32 V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.5
    IL_0002:  dup
    IL_0003:  stloc.0
    IL_0004:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0009:  nop
    IL_000a:  ldloc.0
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0010:  nop
    IL_0011:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::.ctor()
    IL_0016:  dup
    IL_0017:  stloc.1
    IL_0018:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_001d:  nop
    IL_001e:  ldloc.1
    IL_001f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0024:  nop
    IL_0025:  ret
  } // end of method InlineAssignmentTest::SimpleInlineWithLocals

  .method public hidebysig instance void 
          SimpleInlineWithFields() cil managed
  {
    // Code size       38 (0x26)
    .maxstack  3
    .locals init (int32 V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.5
    IL_0003:  dup
    IL_0004:  stloc.0
    IL_0005:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field1
    IL_000a:  ldloc.0
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0010:  nop
    IL_0011:  ldarg.0
    IL_0012:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::.ctor()
    IL_0017:  dup
    IL_0018:  stloc.1
    IL_0019:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field2
    IL_001e:  ldloc.1
    IL_001f:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0024:  nop
    IL_0025:  ret
  } // end of method InlineAssignmentTest::SimpleInlineWithFields

  .method public hidebysig instance void 
          SimpleInlineWithFields2() cil managed
  {
    // Code size       62 (0x3e)
    .maxstack  3
    .locals init (int32 V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.5
    IL_0003:  dup
    IL_0004:  stloc.0
    IL_0005:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field1
    IL_000a:  ldloc.0
    IL_000b:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_0010:  nop
    IL_0011:  ldarg.0
    IL_0012:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field1
    IL_0017:  call       void [mscorlib]System.Console::WriteLine(int32)
    IL_001c:  nop
    IL_001d:  ldarg.0
    IL_001e:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::.ctor()
    IL_0023:  dup
    IL_0024:  stloc.1
    IL_0025:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field2
    IL_002a:  ldloc.1
    IL_002b:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0030:  nop
    IL_0031:  ldarg.0
    IL_0032:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest::field2
    IL_0037:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_003c:  nop
    IL_003d:  ret
  } // end of method InlineAssignmentTest::SimpleInlineWithFields2

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method InlineAssignmentTest::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.InlineAssignmentTest


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../Tests/TestCases/Pretty\InlineAssignmentTest.res
