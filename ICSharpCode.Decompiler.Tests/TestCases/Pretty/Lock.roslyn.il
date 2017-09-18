
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly Lock
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
.module Lock.dll
// MVID: {3273D43A-61AF-4B6E-B6CF-5B3C9A7E807A}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x029B0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Lock
       extends [mscorlib]System.Object
{
  .method public hidebysig instance void 
          LockThis() cil managed
  {
    // Code size       36 (0x24)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Lock V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.0
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.0
    IL_0004:  stloc.1
    .try
    {
      IL_0005:  ldloc.0
      IL_0006:  ldloca.s   V_1
      IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object,
                                                                          bool&)
      IL_000d:  nop
      IL_000e:  nop
      IL_000f:  call       void [mscorlib]System.Console::WriteLine()
      IL_0014:  nop
      IL_0015:  nop
      IL_0016:  leave.s    IL_0023

    }  // end .try
    finally
    {
      IL_0018:  ldloc.1
      IL_0019:  brfalse.s  IL_0022

      IL_001b:  ldloc.0
      IL_001c:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0021:  nop
      IL_0022:  endfinally
    }  // end handler
    IL_0023:  ret
  } // end of method Lock::LockThis

  .method public hidebysig instance void 
          LockOnType() cil managed
  {
    // Code size       45 (0x2d)
    .maxstack  2
    .locals init (class [mscorlib]System.Type V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.Lock
    IL_0006:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
    IL_000b:  stloc.0
    IL_000c:  ldc.i4.0
    IL_000d:  stloc.1
    .try
    {
      IL_000e:  ldloc.0
      IL_000f:  ldloca.s   V_1
      IL_0011:  call       void [mscorlib]System.Threading.Monitor::Enter(object,
                                                                          bool&)
      IL_0016:  nop
      IL_0017:  nop
      IL_0018:  call       void [mscorlib]System.Console::WriteLine()
      IL_001d:  nop
      IL_001e:  nop
      IL_001f:  leave.s    IL_002c

    }  // end .try
    finally
    {
      IL_0021:  ldloc.1
      IL_0022:  brfalse.s  IL_002b

      IL_0024:  ldloc.0
      IL_0025:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_002a:  nop
      IL_002b:  endfinally
    }  // end handler
    IL_002c:  ret
  } // end of method Lock::LockOnType

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method Lock::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Lock


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
