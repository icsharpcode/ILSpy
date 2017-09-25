
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly wgewqkaz
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module wgewqkaz.dll
// MVID: {298A3373-9EF1-4D76-BCEC-2E82A945AF55}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00550000


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

  .method public hidebysig instance void 
          SimpleUsingNullStatement() cil managed
  {
    // Code size       36 (0x24)
    .maxstack  2
    .locals init (object V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  stloc.0
    .try
    {
      IL_0003:  nop
      IL_0004:  ldstr      "using (null)"
      IL_0009:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_000e:  nop
      IL_000f:  nop
      IL_0010:  leave.s    IL_0022

    }  // end .try
    finally
    {
      IL_0012:  ldloc.0
      IL_0013:  ldnull
      IL_0014:  ceq
      IL_0016:  stloc.1
      IL_0017:  ldloc.1
      IL_0018:  brtrue.s   IL_0021

      IL_001a:  ldnull
      IL_001b:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0020:  nop
      IL_0021:  endfinally
    }  // end handler
    IL_0022:  nop
    IL_0023:  ret
  } // end of method Using::SimpleUsingNullStatement

  .method public hidebysig instance void 
          SimpleUsingExpressionStatement() cil managed
  {
    // Code size       40 (0x28)
    .maxstack  2
    .locals init (class [mscorlib]System.IO.MemoryStream V_0,
             bool V_1)
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
      IL_0014:  leave.s    IL_0026

    }  // end .try
    finally
    {
      IL_0016:  ldloc.0
      IL_0017:  ldnull
      IL_0018:  ceq
      IL_001a:  stloc.1
      IL_001b:  ldloc.1
      IL_001c:  brtrue.s   IL_0025

      IL_001e:  ldloc.0
      IL_001f:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0024:  nop
      IL_0025:  endfinally
    }  // end handler
    IL_0026:  nop
    IL_0027:  ret
  } // end of method Using::SimpleUsingExpressionStatement

  .method public hidebysig instance void 
          SimpleUsingExpressionStatementWithDeclaration() cil managed
  {
    // Code size       65 (0x41)
    .maxstack  2
    .locals init (class [mscorlib]System.IO.MemoryStream V_0,
             bool V_1)
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
      IL_002d:  leave.s    IL_003f

    }  // end .try
    finally
    {
      IL_002f:  ldloc.0
      IL_0030:  ldnull
      IL_0031:  ceq
      IL_0033:  stloc.1
      IL_0034:  ldloc.1
      IL_0035:  brtrue.s   IL_003e

      IL_0037:  ldloc.0
      IL_0038:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_003d:  nop
      IL_003e:  endfinally
    }  // end handler
    IL_003f:  nop
    IL_0040:  ret
  } // end of method Using::SimpleUsingExpressionStatementWithDeclaration

  .method public hidebysig instance void 
          UsingStatementThatChangesTheVariable() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  2
    .locals init (class [mscorlib]System.Threading.CancellationTokenSource V_0,
             class [mscorlib]System.Threading.CancellationTokenSource V_1,
             bool V_2)
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
      IL_0011:  leave.s    IL_0023

    }  // end .try
    finally
    {
      IL_0013:  ldloc.1
      IL_0014:  ldnull
      IL_0015:  ceq
      IL_0017:  stloc.2
      IL_0018:  ldloc.2
      IL_0019:  brtrue.s   IL_0022

      IL_001b:  ldloc.1
      IL_001c:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0021:  nop
      IL_0022:  endfinally
    }  // end handler
    IL_0023:  nop
    IL_0024:  ldloc.0
    IL_0025:  callvirt   instance void [mscorlib]System.Threading.CancellationTokenSource::Cancel()
    IL_002a:  nop
    IL_002b:  ret
  } // end of method Using::UsingStatementThatChangesTheVariable

  .method public hidebysig instance void 
          UsingStatementOnStruct() cil managed
  {
    // Code size       40 (0x28)
    .maxstack  1
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct V_0)
    IL_0000:  nop
    IL_0001:  ldc.i4.1
    IL_0002:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct::.ctor(int32)
    IL_0007:  stloc.0
    .try
    {
      IL_0008:  nop
      IL_0009:  ldstr      "using-body"
      IL_000e:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0013:  nop
      IL_0014:  nop
      IL_0015:  leave.s    IL_0026

    }  // end .try
    finally
    {
      IL_0017:  ldloca.s   V_0
      IL_0019:  constrained. ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct
      IL_001f:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0024:  nop
      IL_0025:  endfinally
    }  // end handler
    IL_0026:  nop
    IL_0027:  ret
  } // end of method Using::UsingStatementOnStruct

  .method public hidebysig instance void 
          UsingStatementOnStructWithVariable() cil managed
  {
    // Code size       53 (0x35)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct V_0)
    IL_0000:  nop
    IL_0001:  ldloca.s   V_0
    IL_0003:  ldc.i4.2
    IL_0004:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct::.ctor(int32)
    IL_0009:  nop
    .try
    {
      IL_000a:  nop
      IL_000b:  ldstr      "using-body: "
      IL_0010:  ldloc.0
      IL_0011:  box        ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct
      IL_0016:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_001b:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0020:  nop
      IL_0021:  nop
      IL_0022:  leave.s    IL_0033

    }  // end .try
    finally
    {
      IL_0024:  ldloca.s   V_0
      IL_0026:  constrained. ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct
      IL_002c:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0031:  nop
      IL_0032:  endfinally
    }  // end handler
    IL_0033:  nop
    IL_0034:  ret
  } // end of method Using::UsingStatementOnStructWithVariable

  .method private hidebysig instance void 
          UsingStatementOnNullableStruct(valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct> us) cil managed
  {
    // Code size       63 (0x3f)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct> V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    .try
    {
      IL_0003:  nop
      IL_0004:  ldstr      "using-body: "
      IL_0009:  ldarg.1
      IL_000a:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct>
      IL_000f:  call       string [mscorlib]System.String::Concat(object,
                                                                  object)
      IL_0014:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0019:  nop
      IL_001a:  nop
      IL_001b:  leave.s    IL_003d

    }  // end .try
    finally
    {
      IL_001d:  ldloca.s   V_0
      IL_001f:  call       instance bool valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct>::get_HasValue()
      IL_0024:  ldc.i4.0
      IL_0025:  ceq
      IL_0027:  stloc.1
      IL_0028:  ldloc.1
      IL_0029:  brtrue.s   IL_003c

      IL_002b:  ldloc.0
      IL_002c:  box        valuetype [mscorlib]System.Nullable`1<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Using/UsingStruct>
      IL_0031:  unbox.any  [mscorlib]System.IDisposable
      IL_0036:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_003b:  nop
      IL_003c:  endfinally
    }  // end handler
    IL_003d:  nop
    IL_003e:  ret
  } // end of method Using::UsingStatementOnNullableStruct

  .method public hidebysig instance void 
          GenericUsing<([mscorlib]System.IDisposable) T>(!!T t) cil managed
  {
    // Code size       49 (0x31)
    .maxstack  2
    .locals init (!!T V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    .try
    {
      IL_0003:  nop
      IL_0004:  ldarg.1
      IL_0005:  box        !!T
      IL_000a:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_000f:  nop
      IL_0010:  nop
      IL_0011:  leave.s    IL_002f

    }  // end .try
    finally
    {
      IL_0013:  ldloc.0
      IL_0014:  box        !!T
      IL_0019:  ldnull
      IL_001a:  ceq
      IL_001c:  stloc.1
      IL_001d:  ldloc.1
      IL_001e:  brtrue.s   IL_002e

      IL_0020:  ldloca.s   V_0
      IL_0022:  constrained. !!T
      IL_0028:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_002d:  nop
      IL_002e:  endfinally
    }  // end handler
    IL_002f:  nop
    IL_0030:  ret
  } // end of method Using::GenericUsing

  .method public hidebysig instance void 
          GenericStructUsing<valuetype .ctor ([mscorlib]System.ValueType, [mscorlib]System.IDisposable) T>(!!T t) cil managed
  {
    // Code size       36 (0x24)
    .maxstack  1
    .locals init (!!T V_0)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    .try
    {
      IL_0003:  nop
      IL_0004:  ldarg.1
      IL_0005:  box        !!T
      IL_000a:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_000f:  nop
      IL_0010:  nop
      IL_0011:  leave.s    IL_0022

    }  // end .try
    finally
    {
      IL_0013:  ldloca.s   V_0
      IL_0015:  constrained. !!T
      IL_001b:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0020:  nop
      IL_0021:  endfinally
    }  // end handler
    IL_0022:  nop
    IL_0023:  ret
  } // end of method Using::GenericStructUsing

  .method public hidebysig instance void 
          GenericClassUsing<class ([mscorlib]System.IDisposable) T>(!!T t) cil managed
  {
    // Code size       49 (0x31)
    .maxstack  2
    .locals init (!!T V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    .try
    {
      IL_0003:  nop
      IL_0004:  ldarg.1
      IL_0005:  box        !!T
      IL_000a:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_000f:  nop
      IL_0010:  nop
      IL_0011:  leave.s    IL_002f

    }  // end .try
    finally
    {
      IL_0013:  ldloc.0
      IL_0014:  box        !!T
      IL_0019:  ldnull
      IL_001a:  ceq
      IL_001c:  stloc.1
      IL_001d:  ldloc.1
      IL_001e:  brtrue.s   IL_002e

      IL_0020:  ldloca.s   V_0
      IL_0022:  constrained. !!T
      IL_0028:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_002d:  nop
      IL_002e:  endfinally
    }  // end handler
    IL_002f:  nop
    IL_0030:  ret
  } // end of method Using::GenericClassUsing

  .method public hidebysig instance void 
          GenericNullableUsing<valuetype .ctor ([mscorlib]System.ValueType, [mscorlib]System.IDisposable) T>(valuetype [mscorlib]System.Nullable`1<!!T> t) cil managed
  {
    // Code size       53 (0x35)
    .maxstack  2
    .locals init (valuetype [mscorlib]System.Nullable`1<!!T> V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldarg.1
    IL_0002:  stloc.0
    .try
    {
      IL_0003:  nop
      IL_0004:  ldarg.1
      IL_0005:  box        valuetype [mscorlib]System.Nullable`1<!!T>
      IL_000a:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_000f:  nop
      IL_0010:  nop
      IL_0011:  leave.s    IL_0033

    }  // end .try
    finally
    {
      IL_0013:  ldloca.s   V_0
      IL_0015:  call       instance bool valuetype [mscorlib]System.Nullable`1<!!T>::get_HasValue()
      IL_001a:  ldc.i4.0
      IL_001b:  ceq
      IL_001d:  stloc.1
      IL_001e:  ldloc.1
      IL_001f:  brtrue.s   IL_0032

      IL_0021:  ldloc.0
      IL_0022:  box        valuetype [mscorlib]System.Nullable`1<!!T>
      IL_0027:  unbox.any  [mscorlib]System.IDisposable
      IL_002c:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
      IL_0031:  nop
      IL_0032:  endfinally
    }  // end handler
    IL_0033:  nop
    IL_0034:  ret
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
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\Using.res
