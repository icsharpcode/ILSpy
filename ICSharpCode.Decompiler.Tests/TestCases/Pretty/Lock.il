
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly '1c2baaro'
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module '1c2baaro.dll'
// MVID: {5FC5B58D-AAD7-4436-A58F-6F3D75486C66}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00C60000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Lock
       extends [mscorlib]System.Object
{
  .method public hidebysig instance void 
          LockThis() cil managed
  {
    // Code size       42 (0x2a)
    .maxstack  2
    .locals init (bool V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Lock V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    .try
    {
      IL_0003:  ldarg.0
      IL_0004:  dup
      IL_0005:  stloc.1
      IL_0006:  ldloca.s   V_0
      IL_0008:  call       void [mscorlib]System.Threading.Monitor::Enter(object,
                                                                          bool&)
      IL_000d:  nop
      IL_000e:  nop
      IL_000f:  call       void [mscorlib]System.Console::WriteLine()
      IL_0014:  nop
      IL_0015:  nop
      IL_0016:  leave.s    IL_0028

    }  // end .try
    finally
    {
      IL_0018:  ldloc.0
      IL_0019:  ldc.i4.0
      IL_001a:  ceq
      IL_001c:  stloc.2
      IL_001d:  ldloc.2
      IL_001e:  brtrue.s   IL_0027

      IL_0020:  ldloc.1
      IL_0021:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0026:  nop
      IL_0027:  endfinally
    }  // end handler
    IL_0028:  nop
    IL_0029:  ret
  } // end of method Lock::LockThis

  .method public hidebysig instance void 
          LockOnType() cil managed
  {
    // Code size       51 (0x33)
    .maxstack  2
    .locals init (bool V_0,
             class [mscorlib]System.Type V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    .try
    {
      IL_0003:  ldtoken    ICSharpCode.Decompiler.Tests.TestCases.Pretty.Lock
      IL_0008:  call       class [mscorlib]System.Type [mscorlib]System.Type::GetTypeFromHandle(valuetype [mscorlib]System.RuntimeTypeHandle)
      IL_000d:  dup
      IL_000e:  stloc.1
      IL_000f:  ldloca.s   V_0
      IL_0011:  call       void [mscorlib]System.Threading.Monitor::Enter(object,
                                                                          bool&)
      IL_0016:  nop
      IL_0017:  nop
      IL_0018:  call       void [mscorlib]System.Console::WriteLine()
      IL_001d:  nop
      IL_001e:  nop
      IL_001f:  leave.s    IL_0031

    }  // end .try
    finally
    {
      IL_0021:  ldloc.0
      IL_0022:  ldc.i4.0
      IL_0023:  ceq
      IL_0025:  stloc.2
      IL_0026:  ldloc.2
      IL_0027:  brtrue.s   IL_0030

      IL_0029:  ldloc.1
      IL_002a:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_002f:  nop
      IL_0030:  endfinally
    }  // end handler
    IL_0031:  nop
    IL_0032:  ret
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
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\Lock.res
