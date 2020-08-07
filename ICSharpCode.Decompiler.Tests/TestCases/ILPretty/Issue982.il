
//  Microsoft (R) .NET Framework IL Disassembler.  Version 3.5.30729.1
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern System
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly extern Microsoft.VisualBasic
{
  .publickeytoken = (B0 3F 5F 7F 11 D5 0A 3A )                         // .?_....:
  .ver 10:0:0:0
}
.assembly ICSharpCode.Decompiler.Tests.TestCases.ILPretty
{
  .hash algorithm 0x00008004
  .ver 1:0:0:0
}

.module Issue982.exe
// MVID: {1DAED9CF-459B-4BA6-A091-CB3BDEF963F8}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00020003    //  ILONLY 32BITREQUIRED
// Image base: 0x06E80000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue982
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Reflection.DefaultMemberAttribute::.ctor(string) = ( 01 00 05 54 65 78 74 32 00 00 )                   // ...Text2..
  .field private string textStr
  .field private string textStr2
  .method public specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Issue982::.ctor

  .method public specialname instance string 
          get_Text() cil managed
  {
    // Code size       12 (0xc)
    .maxstack  1
    .locals init ([0] string Text)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue982::textStr
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method Issue982::get_Text

  .method public specialname instance void 
          set_Text(string str) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.1
    IL_0003:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue982::textStr
    IL_0008:  ret
  } // end of method Issue982::set_Text

  .method public specialname instance string 
          get_Text2(int32 index) cil managed
  {
    // Code size       12 (0xc)
    .maxstack  1
    .locals init ([0] string Text2)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue982::textStr2
    IL_0007:  stloc.0
    IL_0008:  br.s       IL_000a

    IL_000a:  ldloc.0
    IL_000b:  ret
  } // end of method Issue982::get_Text2

  .method public specialname instance void 
          set_Text2(int32 index,
                    string str) cil managed
  {
    // Code size       9 (0x9)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldarg.2
    IL_0003:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue982::textStr2
    IL_0008:  ret
  } // end of method Issue982::set_Text2

  .property instance string Text()
  {
    .get instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue982::get_Text()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue982::set_Text(string)
  } // end of property Issue982::Text
  .property instance string Text2(int32)
  {
    .get instance string ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue982::get_Text2(int32)
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue982::set_Text2(int32,
                                                      string)
  } // end of property Issue982::Text2
} // end of class ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue982