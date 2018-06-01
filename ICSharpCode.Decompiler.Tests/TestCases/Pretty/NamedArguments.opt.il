



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly NamedArguments.opt
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module NamedArguments.opt.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments
       extends [mscorlib]System.Object
{
  .class auto ansi nested private beforefieldinit ClassWithNamedArgCtor
         extends [mscorlib]System.Object
  {
    .method assembly hidebysig specialname rtspecialname 
            instance void  .ctor([opt] bool arg1,
                                 [opt] bool arg2) cil managed
    {
      .param [1] = bool(false)
      .param [2] = bool(false)
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ret
    } // end of method ClassWithNamedArgCtor::.ctor

    .method assembly hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       30 (0x1e)
      .maxstack  3
      .locals init (bool V_0)
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.1
      IL_0002:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
      IL_0007:  ldc.i4.1
      IL_0008:  ceq
      IL_000a:  ldc.i4.0
      IL_000b:  ceq
      IL_000d:  stloc.0
      IL_000e:  ldc.i4.2
      IL_000f:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
      IL_0014:  ldc.i4.2
      IL_0015:  ceq
      IL_0017:  ldloc.0
      IL_0018:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments/ClassWithNamedArgCtor::.ctor(bool,
                                                                                                                                   bool)
      IL_001d:  ret
    } // end of method ClassWithNamedArgCtor::.ctor

  } // end of class ClassWithNamedArgCtor

  .method public hidebysig instance void 
          Use(int32 a,
              int32 b,
              int32 c) cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method NamedArguments::Use

  .method public hidebysig static int32  Get(int32 i) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ret
  } // end of method NamedArguments::Get

  .method public hidebysig instance void 
          Test() cil managed
  {
    // Code size       77 (0x4d)
    .maxstack  4
    .locals init (int32 V_0,
             int32 V_1)
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_0007:  ldc.i4.2
    IL_0008:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_000d:  ldc.i4.3
    IL_000e:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_0013:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Use(int32,
                                                                                                         int32,
                                                                                                         int32)
    IL_0018:  ldarg.0
    IL_0019:  ldc.i4.1
    IL_001a:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_001f:  ldc.i4.2
    IL_0020:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_0025:  stloc.0
    IL_0026:  ldc.i4.3
    IL_0027:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_002c:  ldloc.0
    IL_002d:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Use(int32,
                                                                                                         int32,
                                                                                                         int32)
    IL_0032:  ldarg.0
    IL_0033:  ldc.i4.1
    IL_0034:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_0039:  stloc.1
    IL_003a:  ldc.i4.2
    IL_003b:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_0040:  ldloc.1
    IL_0041:  ldc.i4.3
    IL_0042:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_0047:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Use(int32,
                                                                                                         int32,
                                                                                                         int32)
    IL_004c:  ret
  } // end of method NamedArguments::Test

  .method public hidebysig instance void 
          NotNamedArgs() cil managed
  {
    // Code size       27 (0x1b)
    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.1
    IL_0001:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_0006:  stloc.0
    IL_0007:  ldarg.0
    IL_0008:  ldc.i4.2
    IL_0009:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_000e:  ldloc.0
    IL_000f:  ldc.i4.3
    IL_0010:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_0015:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Use(int32,
                                                                                                         int32,
                                                                                                         int32)
    IL_001a:  ret
  } // end of method NamedArguments::NotNamedArgs

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method NamedArguments::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
