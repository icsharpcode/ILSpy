
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly titpuxma
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module titpuxma.dll
// MVID: {8B90EB29-A139-4376-848E-C5D4D42952F1}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02AF0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Lock
       extends [mscorlib]System.Object
{
  .method public hidebysig instance void 
          LockThis() cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  2
    .locals init (bool V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Lock V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    .try
    {
      IL_0002:  ldarg.0
      IL_0003:  dup
      IL_0004:  stloc.1
      IL_0005:  ldloca.s   V_0
      IL_0007:  call       void [mscorlib]System.Threading.Monitor::Enter(object,
                                                                          bool&)
      IL_000c:  call       void [mscorlib]System.Console::WriteLine()
      IL_0011:  leave.s    IL_001d

    }  // end .try
    finally
    {
      IL_0013:  ldloc.0
      IL_0014:  brfalse.s  IL_001c

      IL_0016:  ldloc.1
      IL_0017:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_001c:  endfinally
    }  // end handler
    IL_001d:  ret
  } // end of method Lock::LockThis

  .method public hidebysig instance void 
          LockOnType() cil managed
  {
    // Code size       39 (0x27)
    .maxstack  2
    .locals init (bool V_0,
             class [mscorlib]System.Type V_1)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    .try
    {
      IL_0002:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.Lock
      IL_0007:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
      IL_000c:  dup
      IL_000d:  stloc.1
      IL_000e:  ldloca.s   V_0
      IL_0010:  call       void [mscorlib]System.Threading.Monitor::Enter(object,
                                                                          bool&)
      IL_0015:  call       void [mscorlib]System.Console::WriteLine()
      IL_001a:  leave.s    IL_0026

    }  // end .try
    finally
    {
      IL_001c:  ldloc.0
      IL_001d:  brfalse.s  IL_0025

      IL_001f:  ldloc.1
      IL_0020:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0025:  endfinally
    }  // end handler
    IL_0026:  ret
  } // end of method Lock::LockOnType

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Lock::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Lock


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\Lock.opt.res
