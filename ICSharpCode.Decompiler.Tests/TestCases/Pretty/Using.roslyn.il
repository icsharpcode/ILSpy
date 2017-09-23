
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly Using
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
.module Using.dll
// MVID: {3686D0B4-0A65-4B60-A432-7042528E2212}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x02E50000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using
       extends [mscorlib]System.Object
{
  .class sequential ansi sealed nested private beforefieldinit UsingStruct
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.IDisposable
  {
    .pack 0
    .size 1
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(int32 i) cil managed
    {
      // Code size       9 (0x9)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.1
      IL_0002:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_0007:  nop
      IL_0008:  ret
    } // end of method UsingStruct::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0006:  throw
    } // end of method UsingStruct::System.IDisposable.Dispose

  } // end of class UsingStruct

  .class auto ansi nested private beforefieldinit PrintOnDispose
         extends [mscorlib]System.Object
         implements [mscorlib]System.IDisposable
  {
    .field private string v
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor(string v) cil managed
    {
      // Code size       16 (0x10)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  nop
      IL_0008:  ldarg.0
      IL_0009:  ldarg.1
      IL_000a:  stfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/PrintOnDispose::v
      IL_000f:  ret
    } // end of method PrintOnDispose::.ctor

    .method public hidebysig newslot virtual final 
            instance void  Dispose() cil managed
    {
      // Code size       14 (0xe)
      .maxstack  8
      IL_0000:  nop
      IL_0001:  ldarg.0
      IL_0002:  ldfld      string ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/PrintOnDispose::v
      IL_0007:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_000c:  nop
      IL_000d:  ret
    } // end of method PrintOnDispose::Dispose

  } // end of class PrintOnDispose

  .method public hidebysig instance void 
          SimpleUsingExpressionStatement() cil managed
  {
    // Code size       34 (0x22)
    .maxstack  1
    .locals init (class [mscorlib]System.IO.MemoryStream V_0)
    IL_0000:  nop
    IL_0001:  newobj     instance void [mscorlib]System.IO.MemoryStream::.ctor()
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  nop
      IL_0008:  ldstr      "using-body"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  nop
      IL_0013:  nop
      IL_0014:  leave.s    IL_0021

    }  // end .try
    finally
    {
      IL_0016:  ldloc.0
      IL_0017:  brfalse.s  IL_0020

      IL_0019:  ldloc.0
      IL_001a:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001f:  nop
      IL_0020:  endfinally
    }  // end handler
    IL_0021:  ret
  } // end of method Using::SimpleUsingExpressionStatement

  .method public hidebysig instance void 
          SimpleUsingExpressionStatementWithDeclaration() cil managed
  {
    // Code size       59 (0x3b)
    .maxstack  2
    .locals init (class [mscorlib]System.IO.MemoryStream V_0)
    IL_0000:  nop
    IL_0001:  newobj     instance void [mscorlib]System.IO.MemoryStream::.ctor()
    IL_0006:  stloc.0
    .try
    {
      IL_0007:  nop
      IL_0008:  ldloc.0
      IL_0009:  ldc.i4.s   42
      IL_000b:  callvirt   instance void [mscorlib]System.IO.Stream::WriteByte(uint8)
      IL_0010:  nop
      IL_0011:  ldstr      "using-body: "
      IL_0016:  ldloc.0
      IL_0017:  callvirt   instance int64 [mscorlib]System.IO.Stream::get_Position()
      IL_001c:  box        [mscorlib]System.Int64
      IL_0021:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_0026:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_002b:  nop
      IL_002c:  nop
      IL_002d:  leave.s    IL_003a

    }  // end .try
    finally
    {
      IL_002f:  ldloc.0
      IL_0030:  brfalse.s  IL_0039

      IL_0032:  ldloc.0
      IL_0033:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0038:  nop
      IL_0039:  endfinally
    }  // end handler
    IL_003a:  ret
  } // end of method Using::SimpleUsingExpressionStatementWithDeclaration

  .method public hidebysig instance void 
          UsingStatementThatChangesTheVariable() cil managed
  {
    // Code size       38 (0x26)
    .maxstack  1
    .locals init (class [mscorlib]System.Threading.CancellationTokenSource V_0,
             class [mscorlib]System.Threading.CancellationTokenSource V_1)
    IL_0000:  nop
    IL_0001:  newobj     instance void [mscorlib]System.Threading.CancellationTokenSource::.ctor()
    IL_0006:  stloc.0
    IL_0007:  ldloc.0
    IL_0008:  stloc.1
    .try
    {
      IL_0009:  nop
      IL_000a:  newobj     instance void [mscorlib]System.Threading.CancellationTokenSource::.ctor()
      IL_000f:  stloc.0
      IL_0010:  nop
      IL_0011:  leave.s    IL_001e

    }  // end .try
    finally
    {
      IL_0013:  ldloc.1
      IL_0014:  brfalse.s  IL_001d

      IL_0016:  ldloc.1
      IL_0017:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001c:  nop
      IL_001d:  endfinally
    }  // end handler
    IL_001e:  ldloc.0
    IL_001f:  callvirt   instance void [mscorlib]System.Threading.CancellationTokenSource::Cancel()
    IL_0024:  nop
    IL_0025:  ret
  } // end of method Using::UsingStatementThatChangesTheVariable

  .method public hidebysig instance void 
          UsingStatementOnStruct() cil managed
  {
    // Code size       40 (0x28)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct V_0)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_0
    IL_0003:  ldc.i4.1
    IL_0004:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct::.ctor(int32)
    .try
    {
      IL_0009:  nop
      IL_000a:  ldstr      "using-body"
      IL_000f:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0014:  nop
      IL_0015:  nop
      IL_0016:  leave.s    IL_0027

    }  // end .try
    finally
    {
      IL_0018:  ldloca.s   V_0
      IL_001a:  constrained. ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct
      IL_0020:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0025:  nop
      IL_0026:  endfinally
    }  // end handler
    IL_0027:  ret
  } // end of method Using::UsingStatementOnStruct

  .method public hidebysig instance void 
          UsingStatementOnStructWithVariable() cil managed
  {
    // Code size       51 (0x33)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct V_0)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_0
    IL_0003:  ldc.i4.2
    IL_0004:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct::.ctor(int32)
    .try
    {
      IL_0009:  nop
      IL_000a:  ldstr      "using-body: "
      IL_000f:  ldloc.0
      IL_0010:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct
      IL_0015:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_001a:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_001f:  nop
      IL_0020:  nop
      IL_0021:  leave.s    IL_0032

    }  // end .try
    finally
    {
      IL_0023:  ldloca.s   V_0
      IL_0025:  constrained. ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct
      IL_002b:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0030:  nop
      IL_0031:  endfinally
    }  // end handler
    IL_0032:  ret
  } // end of method Using::UsingStatementOnStructWithVariable

  .method public hidebysig instance void 
          NoUsing() cil managed
  {
    // Code size       38 (0x26)
    .maxstack  1
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/PrintOnDispose V_0)
    IL_0000:  nop
    IL_0001:  ldstr      "Wrong"
    IL_0006:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/PrintOnDispose::.ctor(string)
    IL_000b:  stloc.0
    .try
    {
      IL_000c:  nop
      IL_000d:  ldstr      "Correct"
      IL_0012:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/PrintOnDispose::.ctor(string)
      IL_0017:  stloc.0
      IL_0018:  nop
      IL_0019:  leave.s    IL_0025

    }  // end .try
    finally
    {
      IL_001b:  nop
      IL_001c:  ldloc.0
      IL_001d:  callvirt   instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/PrintOnDispose::Dispose()
      IL_0022:  nop
      IL_0023:  nop
      IL_0024:  endfinally
    }  // end handler
    IL_0025:  ret
  } // end of method Using::NoUsing

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  nop
    IL_0007:  ret
  } // end of method Using::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
