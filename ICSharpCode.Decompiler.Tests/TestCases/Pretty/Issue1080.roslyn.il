



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly Issue1080
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
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
.module Issue1080.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class private abstract auto ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.ExtensionsTest
       extends [mscorlib]System.Object
{
  .method private hidebysig static void  Dummy(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.SpaceB.Type2 intf) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method ExtensionsTest::Dummy

  .method private hidebysig static void  Test(object obj) cil managed
  {
    // Code size       26 (0x1a)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.Type2 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  isinst     ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.Type2
    IL_0007:  stloc.0
    IL_0008:  ldloc.0
    IL_0009:  ldnull
    IL_000a:  cgt.un
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  brfalse.s  IL_0019

    IL_0010:  nop
    IL_0011:  ldloc.0
    IL_0012:  call       void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceC.Extensions::Extension(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.SpaceB.Type1)
    IL_0017:  nop
    IL_0018:  nop
    IL_0019:  ret
  } // end of method ExtensionsTest::Test

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.ExtensionsTest

.class private abstract auto ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceC.Extensions
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig static void  Extension(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.SpaceB.Type1 obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method Extensions::Extension

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceC.Extensions

.class interface private abstract auto ansi ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.Type2
       implements ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.SpaceB.Type2,
                  ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.SpaceB.Type1
{
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.Type2

.class interface private abstract auto ansi ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.SpaceB.Type2
       implements ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.SpaceB.Type1
{
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.SpaceB.Type2

.class interface private abstract auto ansi ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.SpaceB.Type1
{
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.SpaceB.Type1

.class private abstract auto ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.SpaceB.Extensions
       extends [mscorlib]System.Object
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig static void  Extension(class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.SpaceB.Type1 obj) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.ExtensionAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method Extensions::Extension

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1080.SpaceA.SpaceB.Extensions


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
