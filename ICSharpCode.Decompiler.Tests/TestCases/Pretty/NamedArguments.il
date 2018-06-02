



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly NamedArguments
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module NamedArguments.dll
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
      // Code size       10 (0xa)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  nop
      IL_0008:  nop
      IL_0009:  ret
    } // end of method ClassWithNamedArgCtor::.ctor

    .method assembly hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       33 (0x21)
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
      IL_001d:  nop
      IL_001e:  nop
      IL_001f:  nop
      IL_0020:  ret
    } // end of method ClassWithNamedArgCtor::.ctor

  } // end of class ClassWithNamedArgCtor

  .method public hidebysig instance void 
          Use(int32 a,
              int32 b,
              int32 c) cil managed
  {
    // Code size       2 (0x2)
    .maxstack  8
    IL_0000:  nop
    IL_0001:  ret
  } // end of method NamedArguments::Use

  .method public hidebysig static int32  Get(int32 i) cil managed
  {
    // Code size       7 (0x7)
    .maxstack  1
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  br.s       IL_0005

    IL_0005:  ldloc.0
    IL_0006:  ret
  } // end of method NamedArguments::Get

  .method public hidebysig instance void 
          Test() cil managed
  {
    // Code size       81 (0x51)
    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  ldc.i4.1
    IL_0003:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_0008:  ldc.i4.2
    IL_0009:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_000e:  ldc.i4.3
    IL_000f:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_0014:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Use(int32,
                                                                                                         int32,
                                                                                                         int32)
    IL_0019:  nop
    IL_001a:  ldarg.0
    IL_001b:  ldc.i4.1
    IL_001c:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_0021:  ldc.i4.2
    IL_0022:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_0027:  stloc.0
    IL_0028:  ldc.i4.3
    IL_0029:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_002e:  ldloc.0
    IL_002f:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Use(int32,
                                                                                                         int32,
                                                                                                         int32)
    IL_0034:  nop
    IL_0035:  ldarg.0
    IL_0036:  ldc.i4.1
    IL_0037:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_003c:  stloc.0
    IL_003d:  ldc.i4.2
    IL_003e:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_0043:  ldloc.0
    IL_0044:  ldc.i4.3
    IL_0045:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_004a:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Use(int32,
                                                                                                         int32,
                                                                                                         int32)
    IL_004f:  nop
    IL_0050:  ret
  } // end of method NamedArguments::Test

  .method public hidebysig instance void 
          NotNamedArgs() cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_0007:  stloc.0
    IL_0008:  ldarg.0
    IL_0009:  ldc.i4.2
    IL_000a:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_000f:  ldloc.0
    IL_0010:  ldc.i4.3
    IL_0011:  call       int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Get(int32)
    IL_0016:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.NamedArguments::Use(int32,
                                                                                                         int32,
                                                                                                         int32)
    IL_001b:  nop
    IL_001c:  ret
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
