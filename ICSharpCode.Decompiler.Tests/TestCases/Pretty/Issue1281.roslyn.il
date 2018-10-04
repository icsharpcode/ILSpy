



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly Issue1281
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
.module Issue1281.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1281.Issue1281
       extends [mscorlib]System.Object
{
  .method assembly hidebysig instance void 
          MethodWithParams(object[] args) cil managed
  {
    .param [1]
    .custom instance void [mscorlib]System.ParamArrayAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       34 (0x22)
    .maxstack  5
    .locals init (object[] V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  newarr     [mscorlib]System.Object
    IL_0007:  stloc.0
    IL_0008:  ldarg.0
    IL_0009:  ldloc.0
    IL_000a:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1281.Issue1281::MethodWithParams(object[])
    IL_000f:  nop
    IL_0010:  ldarg.0
    IL_0011:  ldc.i4.1
    IL_0012:  newarr     [mscorlib]System.Object
    IL_0017:  dup
    IL_0018:  ldc.i4.0
    IL_0019:  ldloc.0
    IL_001a:  stelem.ref
    IL_001b:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1281.Issue1281::MethodWithParams(object[])
    IL_0020:  nop
    IL_0021:  ret
  } // end of method Issue1281::MethodWithParams

  .method assembly hidebysig instance void 
          MethodWithIntAndParams(int32 arg,
                                 object[] args) cil managed
  {
    .param [2]
    .custom instance void [mscorlib]System.ParamArrayAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       38 (0x26)
    .maxstack  6
    .locals init (object[] V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  newarr     [mscorlib]System.Object
    IL_0007:  stloc.0
    IL_0008:  ldarg.0
    IL_0009:  ldc.i4.s   10
    IL_000b:  ldloc.0
    IL_000c:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1281.Issue1281::MethodWithIntAndParams(int32,
                                                                                                                                 object[])
    IL_0011:  nop
    IL_0012:  ldarg.0
    IL_0013:  ldc.i4.s   10
    IL_0015:  ldc.i4.1
    IL_0016:  newarr     [mscorlib]System.Object
    IL_001b:  dup
    IL_001c:  ldc.i4.0
    IL_001d:  ldloc.0
    IL_001e:  stelem.ref
    IL_001f:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1281.Issue1281::MethodWithIntAndParams(int32,
                                                                                                                                 object[])
    IL_0024:  nop
    IL_0025:  ret
  } // end of method Issue1281::MethodWithIntAndParams

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method Issue1281::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Issue1281.Issue1281


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
