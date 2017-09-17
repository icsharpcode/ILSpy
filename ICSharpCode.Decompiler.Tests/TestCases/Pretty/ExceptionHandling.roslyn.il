
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly ExceptionHandling
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
.module ExceptionHandling.dll
// MVID: {132EAF1E-6B6D-4E21-B000-D06E1A4188E3}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x03070000


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
    // Code size       38 (0x26)
    .maxstack  2
    .locals init (bool V_0,
             bool V_1)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldarg.0
      IL_0003:  ldc.i4.0
      IL_0004:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_0009:  stloc.0
      IL_000a:  ldloc.0
      IL_000b:  brfalse.s  IL_0018

      IL_000d:  nop
      IL_000e:  ldarg.0
      IL_000f:  ldc.i4.1
      IL_0010:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_0015:  stloc.1
      IL_0016:  leave.s    IL_0024

      IL_0018:  nop
      IL_0019:  leave.s    IL_0020

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_001b:  pop
      IL_001c:  nop
      IL_001d:  nop
      IL_001e:  leave.s    IL_0020

    }  // end handler
    IL_0020:  ldc.i4.0
    IL_0021:  stloc.1
    IL_0022:  br.s       IL_0024

    IL_0024:  ldloc.1
    IL_0025:  ret
  } // end of method ExceptionHandling::ConditionalReturnInThrow

  .method public hidebysig instance bool 
          SimpleTryCatchException() cil managed
  {
    // Code size       54 (0x36)
    .maxstack  2
    .locals init (bool V_0)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldstr      "Try"
      IL_0007:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_000c:  nop
      IL_000d:  ldarg.0
      IL_000e:  newobj     instance void [mscorlib]System.Random::.ctor()
      IL_0013:  callvirt   instance int32 [mscorlib]System.Random::Next()
      IL_0018:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_001d:  stloc.0
      IL_001e:  leave.s    IL_0034

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0020:  pop
      IL_0021:  nop
      IL_0022:  ldstr      "CatchException"
      IL_0027:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_002c:  nop
      IL_002d:  nop
      IL_002e:  leave.s    IL_0030

    }  // end handler
    IL_0030:  ldc.i4.0
    IL_0031:  stloc.0
    IL_0032:  br.s       IL_0034

    IL_0034:  ldloc.0
    IL_0035:  ret
  } // end of method ExceptionHandling::SimpleTryCatchException

  .method public hidebysig instance bool 
          SimpleTryCatchExceptionWithName() cil managed
  {
    // Code size       65 (0x41)
    .maxstack  2
    .locals init (bool V_0,
             class [mscorlib]System.Exception V_1)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldstr      "Try"
      IL_0007:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_000c:  nop
      IL_000d:  ldarg.0
      IL_000e:  newobj     instance void [mscorlib]System.Random::.ctor()
      IL_0013:  callvirt   instance int32 [mscorlib]System.Random::Next()
      IL_0018:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_001d:  stloc.0
      IL_001e:  leave.s    IL_003f

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0020:  stloc.1
      IL_0021:  nop
      IL_0022:  ldstr      "CatchException ex: "
      IL_0027:  ldloc.1
      IL_0028:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_002d:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_0032:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0037:  nop
      IL_0038:  nop
      IL_0039:  leave.s    IL_003b

    }  // end handler
    IL_003b:  ldc.i4.0
    IL_003c:  stloc.0
    IL_003d:  br.s       IL_003f

    IL_003f:  ldloc.0
    IL_0040:  ret
  } // end of method ExceptionHandling::SimpleTryCatchExceptionWithName

  .method public hidebysig instance bool 
          SimpleTryCatchExceptionWithNameAndCondition() cil managed
  {
    // Code size       101 (0x65)
    .maxstack  2
    .locals init (bool V_0,
             class [mscorlib]System.Exception V_1,
             bool V_2)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldstr      "Try"
      IL_0007:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_000c:  nop
      IL_000d:  ldarg.0
      IL_000e:  newobj     instance void [mscorlib]System.Random::.ctor()
      IL_0013:  callvirt   instance int32 [mscorlib]System.Random::Next()
      IL_0018:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_001d:  stloc.0
      IL_001e:  leave.s    IL_0063

    }  // end .try
    filter
    {
      IL_0020:  isinst     [mscorlib]System.Exception
      IL_0025:  dup
      IL_0026:  brtrue.s   IL_002c

      IL_0028:  pop
      IL_0029:  ldc.i4.0
      IL_002a:  br.s       IL_0042

      IL_002c:  stloc.1
      IL_002d:  ldloc.1
      IL_002e:  callvirt   instance string [mscorlib]System.Exception::get_Message()
      IL_0033:  ldstr      "test"
      IL_0038:  callvirt   instance bool [mscorlib]System.String::Contains(string)
      IL_003d:  stloc.2
      IL_003e:  ldloc.2
      IL_003f:  ldc.i4.0
      IL_0040:  cgt.un
      IL_0042:  endfilter
    }  // end filter
    {  // handler
      IL_0044:  pop
      IL_0045:  nop
      IL_0046:  ldstr      "CatchException ex: "
      IL_004b:  ldloc.1
      IL_004c:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_0051:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_0056:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_005b:  nop
      IL_005c:  nop
      IL_005d:  leave.s    IL_005f

    }  // end handler
    IL_005f:  ldc.i4.0
    IL_0060:  stloc.0
    IL_0061:  br.s       IL_0063

    IL_0063:  ldloc.0
    IL_0064:  ret
  } // end of method ExceptionHandling::SimpleTryCatchExceptionWithNameAndCondition

  .method public hidebysig instance bool 
          SimpleTryFinally() cil managed
  {
    // Code size       36 (0x24)
    .maxstack  1
    .locals init (bool V_0)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldstr      "Try"
      IL_0007:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_000c:  nop
      IL_000d:  nop
      IL_000e:  leave.s    IL_001e

    }  // end .try
    finally
    {
      IL_0010:  nop
      IL_0011:  ldstr      "Finally"
      IL_0016:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_001b:  nop
      IL_001c:  nop
      IL_001d:  endfinally
    }  // end handler
    IL_001e:  ldc.i4.0
    IL_001f:  stloc.0
    IL_0020:  br.s       IL_0022

    IL_0022:  ldloc.0
    IL_0023:  ret
  } // end of method ExceptionHandling::SimpleTryFinally

  .method family hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method ExceptionHandling::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
