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
.assembly extern System.Core
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly ConsoleApp11
{
  .ver 1:0:0:0
}
.module ConsoleApp11.exe
// MVID: {B973FCD6-A9C4-48A9-8291-26DDC248E208}
.imagebase 0x00400000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00020003    //  ILONLY 32BITPREFERRED
// Image base: 0x000001C4B6C90000

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1047
{
	.field private static bool dummy;
	.method private hidebysig instance void ProblemMethod() cil managed 
	{
		IL_0000: ldfld bool ICSharpCode.Decompiler.Tests.TestCases.ILPretty.Issue1047::dummy;
		brfalse L_tgt1
		br L_exit
		L_tgt1: br IL_0000
		br IL_0000
		L_exit: ret
	}
}