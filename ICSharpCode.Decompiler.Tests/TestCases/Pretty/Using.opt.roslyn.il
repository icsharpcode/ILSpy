
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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module Using.dll
// MVID: {AD3F5551-3D76-468D-A005-EEF5916A56A8}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x015E0000


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
      // Code size       7 (0x7)
      .maxstack  8
      IL_0000:  ldarg.1
      IL_0001:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_0006:  ret
    } // end of method UsingStruct::.ctor

    .method private hidebysig newslot virtual final 
            instance void  System.IDisposable.Dispose() cil managed
    {
      .override [mscorlib]System.IDisposable::Dispose
      // Code size       6 (0x6)
      .maxstack  8
      IL_0000:  newobj     instance void [mscorlib]System.NotImplementedException::.ctor()
      IL_0005:  throw
    } // end of method UsingStruct::System.IDisposable.Dispose

  } // end of class UsingStruct

  .method public hidebysig instance void 
          SimpleUsingExpressionStatement() cil managed
  {
    // Code size       29 (0x1d)
    .maxstack  1
    .locals init (class [mscorlib]System.IO.MemoryStream V_0)
    IL_0000:  newobj     instance void [mscorlib]System.IO.MemoryStream::.ctor()
    IL_0005:  stloc.0
    .try
    {
      IL_0006:  ldstr      "using-body"
      IL_000b:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0010:  leave.s    IL_001c

    }  // end .try
    finally
    {
      IL_0012:  ldloc.0
      IL_0013:  brfalse.s  IL_001b

      IL_0015:  ldloc.0
      IL_0016:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001b:  endfinally
    }  // end handler
    IL_001c:  ret
  } // end of method Using::SimpleUsingExpressionStatement

  .method public hidebysig instance void 
          SimpleUsingExpressionStatementWithDeclaration() cil managed
  {
    // Code size       53 (0x35)
    .maxstack  2
    .locals init (class [mscorlib]System.IO.MemoryStream V_0)
    IL_0000:  newobj     instance void [mscorlib]System.IO.MemoryStream::.ctor()
    IL_0005:  stloc.0
    .try
    {
      IL_0006:  ldloc.0
      IL_0007:  ldc.i4.s   42
      IL_0009:  callvirt   instance void [mscorlib]System.IO.Stream::WriteByte(uint8)
      IL_000e:  ldstr      "using-body: "
      IL_0013:  ldloc.0
      IL_0014:  callvirt   instance int64 [mscorlib]System.IO.Stream::get_Position()
      IL_0019:  box        [mscorlib]System.Int64
      IL_001e:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_0023:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0028:  leave.s    IL_0034

    }  // end .try
    finally
    {
      IL_002a:  ldloc.0
      IL_002b:  brfalse.s  IL_0033

      IL_002d:  ldloc.0
      IL_002e:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0033:  endfinally
    }  // end handler
    IL_0034:  ret
  } // end of method Using::SimpleUsingExpressionStatementWithDeclaration

  .method public hidebysig instance void 
          UsingStatementThatChangesTheVariable() cil managed
  {
    // Code size       33 (0x21)
    .maxstack  1
    .locals init (class [mscorlib]System.Threading.CancellationTokenSource V_0,
             class [mscorlib]System.Threading.CancellationTokenSource V_1)
    IL_0000:  newobj     instance void [mscorlib]System.Threading.CancellationTokenSource::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  stloc.1
    .try
    {
      IL_0008:  newobj     instance void [mscorlib]System.Threading.CancellationTokenSource::.ctor()
      IL_000d:  stloc.0
      IL_000e:  leave.s    IL_001a

    }  // end .try
    finally
    {
      IL_0010:  ldloc.1
      IL_0011:  brfalse.s  IL_0019

      IL_0013:  ldloc.1
      IL_0014:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0019:  endfinally
    }  // end handler
    IL_001a:  ldloc.0
    IL_001b:  callvirt   instance void [mscorlib]System.Threading.CancellationTokenSource::Cancel()
    IL_0020:  ret
  } // end of method Using::UsingStatementThatChangesTheVariable

  .method public hidebysig instance void 
          UsingStatementOnStruct() cil managed
  {
    // Code size       35 (0x23)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct V_0)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldc.i4.1
    IL_0003:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct::.ctor(int32)
    .try
    {
      IL_0008:  ldstr      "using-body"
      IL_000d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0012:  leave.s    IL_0022

    }  // end .try
    finally
    {
      IL_0014:  ldloca.s   V_0
      IL_0016:  constrained. ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct
      IL_001c:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0021:  endfinally
    }  // end handler
    IL_0022:  ret
  } // end of method Using::UsingStatementOnStruct

  .method public hidebysig instance void 
          UsingStatementOnStructWithVariable() cil managed
  {
    // Code size       46 (0x2e)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct V_0)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldc.i4.2
    IL_0003:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct::.ctor(int32)
    .try
    {
      IL_0008:  ldstr      "using-body: "
      IL_000d:  ldloc.0
      IL_000e:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct
      IL_0013:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_0018:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_001d:  leave.s    IL_002d

    }  // end .try
    finally
    {
      IL_001f:  ldloca.s   V_0
      IL_0021:  constrained. ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct
      IL_0027:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_002c:  endfinally
    }  // end handler
    IL_002d:  ret
  } // end of method Using::UsingStatementOnStructWithVariable

  .method private hidebysig instance void 
          UsingStatementOnNullableStruct(valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct> us) cil managed
  {
    // Code size       57 (0x39)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct> V_0,
             valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct V_1)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    .try
    {
      IL_0002:  ldstr      "using-body: "
      IL_0007:  ldarg.1
      IL_0008:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct>
      IL_000d:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_0012:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0017:  leave.s    IL_0038

    }  // end .try
    finally
    {
      IL_0019:  ldloca.s   V_0
      IL_001b:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct>::get_HasValue()
      IL_0020:  brfalse.s  IL_0037

      IL_0022:  ldloca.s   V_0
      IL_0024:  call       instance !0 valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct>::GetValueOrDefault()
      IL_0029:  stloc.1
      IL_002a:  ldloca.s   V_1
      IL_002c:  constrained. ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct
      IL_0032:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0037:  endfinally
    }  // end handler
    IL_0038:  ret
  } // end of method Using::UsingStatementOnNullableStruct

  .method public hidebysig instance void 
          GenericUsing<([mscorlib]System.IDisposable) T>(!!T t) cil managed
  {
    // Code size       38 (0x26)
    .maxstack  1
    .locals init (!!T V_0)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    .try
    {
      IL_0002:  ldarg.1
      IL_0003:  box        !!T
      IL_0008:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_000d:  leave.s    IL_0025

    }  // end .try
    finally
    {
      IL_000f:  ldloc.0
      IL_0010:  box        !!T
      IL_0015:  brfalse.s  IL_0024

      IL_0017:  ldloca.s   V_0
      IL_0019:  constrained. !!T
      IL_001f:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0024:  endfinally
    }  // end handler
    IL_0025:  ret
  } // end of method Using::GenericUsing

  .method public hidebysig instance void 
          GenericStructUsing<valuetype .ctor ([mscorlib]System.IDisposable, [mscorlib]System.ValueType) T>(!!T t) cil managed
  {
    // Code size       30 (0x1e)
    .maxstack  1
    .locals init (!!T V_0)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    .try
    {
      IL_0002:  ldarg.1
      IL_0003:  box        !!T
      IL_0008:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_000d:  leave.s    IL_001d

    }  // end .try
    finally
    {
      IL_000f:  ldloca.s   V_0
      IL_0011:  constrained. !!T
      IL_0017:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_001c:  endfinally
    }  // end handler
    IL_001d:  ret
  } // end of method Using::GenericStructUsing

  .method public hidebysig instance void 
          GenericClassUsing<class ([mscorlib]System.IDisposable) T>(!!T t) cil managed
  {
    // Code size       36 (0x24)
    .maxstack  1
    .locals init (!!T V_0)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    .try
    {
      IL_0002:  ldarg.1
      IL_0003:  box        !!T
      IL_0008:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_000d:  leave.s    IL_0023

    }  // end .try
    finally
    {
      IL_000f:  ldloc.0
      IL_0010:  box        !!T
      IL_0015:  brfalse.s  IL_0022

      IL_0017:  ldloc.0
      IL_0018:  box        !!T
      IL_001d:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0022:  endfinally
    }  // end handler
    IL_0023:  ret
  } // end of method Using::GenericClassUsing

  .method public hidebysig instance void 
          GenericNullableUsing<valuetype .ctor ([mscorlib]System.IDisposable, [mscorlib]System.ValueType) T>(valuetype [mscorlib]System.Nullable`1<!!T> t) cil managed
  {
    // Code size       47 (0x2f)
    .maxstack  1
    .locals init (valuetype [mscorlib]System.Nullable`1<!!T> V_0,
             !!T V_1)
    IL_0000:  ldarg.1
    IL_0001:  stloc.0
    .try
    {
      IL_0002:  ldarg.1
      IL_0003:  box        valuetype [mscorlib]System.Nullable`1<!!T>
      IL_0008:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_000d:  leave.s    IL_002e

    }  // end .try
    finally
    {
      IL_000f:  ldloca.s   V_0
      IL_0011:  call       instance bool valuetype [mscorlib]System.Nullable`1<!!T>::get_HasValue()
      IL_0016:  brfalse.s  IL_002d

      IL_0018:  ldloca.s   V_0
      IL_001a:  call       instance !0 valuetype [mscorlib]System.Nullable`1<!!T>::GetValueOrDefault()
      IL_001f:  stloc.1
      IL_0020:  ldloca.s   V_1
      IL_0022:  constrained. !!T
      IL_0028:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_002d:  endfinally
    }  // end handler
    IL_002e:  ret
  } // end of method Using::GenericNullableUsing

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Using::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
