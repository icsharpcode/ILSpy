
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly VariableNaming
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
.module VariableNaming.dll
// MVID: {E0E0C356-381B-487C-973F-FB9568F9D931}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03420000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.VariableNaming
       extends [mscorlib]System.Object
{
  .class auto ansi nested private beforefieldinit C
         extends [mscorlib]System.Object
  {
    .field public string Name
    .field public string Text
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

  .method private hidebysig instance void 
          Test(string text,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.VariableNaming/C c) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldarg.2
    IL_0002:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.VariableNaming/C::Name
    IL_0007:  stloc.0
    IL_0008:  ret
  } // end of method VariableNaming::Test

  .method private hidebysig instance void 
          Test2(string text,
                class ICSharpCode.Decompiler.Tests.TestCases.Pretty.VariableNaming/C c) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  1
    .locals init (string V_0)
    IL_0000:  nop
    IL_0001:  ldarg.2
    IL_0002:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.VariableNaming/C::Text
    IL_0007:  stloc.0
    IL_0008:  ret
  } // end of method VariableNaming::Test2

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method VariableNaming::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.VariableNaming


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
