
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly c32aalgo
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module c32aalgo.dll
// MVID: {7AB7254E-121A-428F-A78B-253019D759D6}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00850000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling
       extends [mscorlib]System.Object
{
  .method public hidebysig newslot abstract virtual 
          instance bool  B(int32 i) cil managed
  {
  } // end of method ExceptionHandling::B

  .method public hidebysig newslot abstract virtual 
          instance void  M(int32 i) cil managed
  {
  } // end of method ExceptionHandling::M

  .method public hidebysig instance bool 
          ConditionalReturnInThrow() cil managed
  {
    // Code size       28 (0x1c)
    .maxstack  2
    .locals init (bool V_0)
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.0
      IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_0007:  brfalse.s  IL_0013

      IL_0009:  ldarg.0
      IL_000a:  ldc.i4.1
      IL_000b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_0010:  stloc.0
      IL_0011:  leave.s    IL_001a

      IL_0013:  leave.s    IL_0018

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_0015:  pop
      IL_0016:  leave.s    IL_0018

    }  // end handler
    IL_0018:  ldc.i4.0
    IL_0019:  ret

    IL_001a:  ldloc.0
    IL_001b:  ret
  } // end of method ExceptionHandling::ConditionalReturnInThrow

  .method public hidebysig instance bool 
          SimpleTryCatchException() cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  2
    .locals init (bool V_0)
    .try
    {
      IL_0000:  ldstr      "Try"
      IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_000a:  ldarg.0
      IL_000b:  newobj     instance void [mscorlib]System.Random::.ctor()
      IL_0010:  callvirt   instance int32 [mscorlib]System.Random::Next()
      IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_001a:  stloc.0
      IL_001b:  leave.s    IL_002c

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_001d:  pop
      IL_001e:  ldstr      "CatchException"
      IL_0023:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0028:  leave.s    IL_002a

    }  // end handler
    IL_002a:  ldc.i4.0
    IL_002b:  ret

    IL_002c:  ldloc.0
    IL_002d:  ret
  } // end of method ExceptionHandling::SimpleTryCatchException

  .method public hidebysig instance bool 
          SimpleTryCatchExceptionWithName() cil managed
  {
    // Code size       57 (0x39)
    .maxstack  2
    .locals init (class [mscorlib]System.Exception V_0,
             bool V_1)
    .try
    {
      IL_0000:  ldstr      "Try"
      IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_000a:  ldarg.0
      IL_000b:  newobj     instance void [mscorlib]System.Random::.ctor()
      IL_0010:  callvirt   instance int32 [mscorlib]System.Random::Next()
      IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_001a:  stloc.1
      IL_001b:  leave.s    IL_0037

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_001d:  stloc.0
      IL_001e:  ldstr      "CatchException ex: "
      IL_0023:  ldloc.0
      IL_0024:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_0029:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_002e:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0033:  leave.s    IL_0035

    }  // end handler
    IL_0035:  ldc.i4.0
    IL_0036:  ret

    IL_0037:  ldloc.1
    IL_0038:  ret
  } // end of method ExceptionHandling::SimpleTryCatchExceptionWithName

  .method public hidebysig instance bool 
          SimpleTryFinally() cil managed
  {
    // Code size       25 (0x19)
    .maxstack  1
    .try
    {
      IL_0000:  ldstr      "Try"
      IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_000a:  leave.s    IL_0017

    }  // end .try
    finally
    {
      IL_000c:  ldstr      "Finally"
      IL_0011:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0016:  endfinally
    }  // end handler
    IL_0017:  ldc.i4.0
    IL_0018:  ret
  } // end of method ExceptionHandling::SimpleTryFinally

  .method family hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method ExceptionHandling::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\ExceptionHandling.opt.res
