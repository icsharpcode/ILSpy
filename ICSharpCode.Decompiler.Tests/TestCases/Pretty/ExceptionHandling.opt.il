



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly ExceptionHandling.opt
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module ExceptionHandling.opt.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling
       extends [mscorlib]System.Object
{
  .method public hidebysig newslot abstract virtual 
          instance bool  B(int32 i) cil managed
  {
  } // end of method ExceptionHandling::B

  .method public hidebysig newslot abstract virtual 
          instance class [mscorlib]System.Threading.Tasks.Task`1<bool> 
          T() cil managed
  {
  } // end of method ExceptionHandling::T

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

  .method public hidebysig instance void 
          MethodEndingWithEndFinally() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  1
    .try
    {
      IL_0000:  ldnull
      IL_0001:  throw

    }  // end .try
    finally
    {
      IL_0002:  call       void [mscorlib]System.Console::WriteLine()
      IL_0007:  endfinally
    }  // end handler
  } // end of method ExceptionHandling::MethodEndingWithEndFinally

  .method public hidebysig instance void 
          MethodEndingWithRethrow() cil managed
  {
    // Code size       5 (0x5)
    .maxstack  1
    .try
    {
      IL_0000:  ldnull
      IL_0001:  throw

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_0002:  pop
      IL_0003:  rethrow
    }  // end handler
  } // end of method ExceptionHandling::MethodEndingWithRethrow

  .method public hidebysig instance void 
          TryCatchFinally() cil managed
  {
    // Code size       40 (0x28)
    .maxstack  1
    .locals init (class [mscorlib]System.Exception V_0)
    .try
    {
      .try
      {
        IL_0000:  ldstr      "Try"
        IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_000a:  leave.s    IL_001a

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_000c:  stloc.0
        IL_000d:  ldloc.0
        IL_000e:  callvirt   instance string [mscorlib]System.Exception::get_Message()
        IL_0013:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0018:  leave.s    IL_001a

      }  // end handler
      IL_001a:  leave.s    IL_0027

    }  // end .try
    finally
    {
      IL_001c:  ldstr      "Finally"
      IL_0021:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0026:  endfinally
    }  // end handler
    IL_0027:  ret
  } // end of method ExceptionHandling::TryCatchFinally

  .method public hidebysig instance void 
          TryCatchMultipleHandlers() cil managed
  {
    // Code size       54 (0x36)
    .maxstack  1
    .locals init (class [mscorlib]System.InvalidOperationException V_0,
             class [mscorlib]System.SystemException V_1)
    .try
    {
      IL_0000:  ldstr      "Try"
      IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_000a:  leave.s    IL_0035

    }  // end .try
    catch [mscorlib]System.InvalidOperationException 
    {
      IL_000c:  stloc.0
      IL_000d:  ldloc.0
      IL_000e:  callvirt   instance string [mscorlib]System.Exception::get_Message()
      IL_0013:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0018:  leave.s    IL_0035

    }  // end handler
    catch [mscorlib]System.SystemException 
    {
      IL_001a:  stloc.1
      IL_001b:  ldloc.1
      IL_001c:  callvirt   instance string [mscorlib]System.Exception::get_Message()
      IL_0021:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0026:  leave.s    IL_0035

    }  // end handler
    catch [mscorlib]System.Object 
    {
      IL_0028:  pop
      IL_0029:  ldstr      "other"
      IL_002e:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0033:  leave.s    IL_0035

    }  // end handler
    IL_0035:  ret
  } // end of method ExceptionHandling::TryCatchMultipleHandlers

  .method public hidebysig instance void 
          NoUsingStatementBecauseTheVariableIsAssignedTo() cil managed
  {
    // Code size       21 (0x15)
    .maxstack  1
    .locals init (class [mscorlib]System.Threading.CancellationTokenSource V_0)
    IL_0000:  ldnull
    IL_0001:  stloc.0
    .try
    {
      IL_0002:  newobj     instance void [mscorlib]System.Threading.CancellationTokenSource::.ctor()
      IL_0007:  stloc.0
      IL_0008:  leave.s    IL_0014

    }  // end .try
    finally
    {
      IL_000a:  ldloc.0
      IL_000b:  brfalse.s  IL_0013

      IL_000d:  ldloc.0
      IL_000e:  callvirt   instance void [mscorlib]System.Threading.CancellationTokenSource::Dispose()
      IL_0013:  endfinally
    }  // end handler
    IL_0014:  ret
  } // end of method ExceptionHandling::NoUsingStatementBecauseTheVariableIsAssignedTo

  .method public hidebysig instance void 
          ThrowInFinally() cil managed
  {
    // Code size       10 (0xa)
    .maxstack  1
    .try
    {
      IL_0000:  leave.s    IL_0008

    }  // end .try
    finally
    {
      IL_0002:  newobj     instance void [mscorlib]System.Exception::.ctor()
      IL_0007:  throw

    }  // end handler
    IL_0008:  br.s       IL_0008
  } // end of method ExceptionHandling::ThrowInFinally

  .method public hidebysig instance bool 
          ComplexConditionalReturnInThrow() cil managed
  {
    // Code size       275 (0x113)
    .maxstack  2
    .locals init (bool V_0)
    .try
    {
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.0
      IL_0002:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_0007:  brfalse.s  IL_0056

      IL_0009:  ldarg.0
      IL_000a:  ldc.i4.1
      IL_000b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_0010:  brfalse.s  IL_0029

      IL_0012:  ldstr      "0 && 1"
      IL_0017:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_001c:  ldarg.0
      IL_001d:  ldc.i4.2
      IL_001e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_0023:  stloc.0
      IL_0024:  leave      IL_0111

      IL_0029:  ldarg.0
      IL_002a:  ldc.i4.3
      IL_002b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_0030:  brfalse.s  IL_004c

      IL_0032:  ldstr      "0 && 3"
      IL_0037:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_003c:  ldarg.0
      IL_003d:  ldc.i4.2
      IL_003e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_0043:  ldc.i4.0
      IL_0044:  ceq
      IL_0046:  stloc.0
      IL_0047:  leave      IL_0111

      IL_004c:  ldstr      "0"
      IL_0051:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0056:  ldstr      "End Try"
      IL_005b:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0060:  leave      IL_010f

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_0065:  pop
      .try
      {
        .try
        {
          .try
          {
            IL_0066:  ldarg.0
            IL_0067:  ldc.i4.0
            IL_0068:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_006d:  brtrue.s   IL_0078

            IL_006f:  ldarg.0
            IL_0070:  ldc.i4.1
            IL_0071:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_0076:  brfalse.s  IL_0081

            IL_0078:  ldarg.0
            IL_0079:  ldc.i4.2
            IL_007a:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_007f:  brtrue.s   IL_008a

            IL_0081:  ldarg.0
            IL_0082:  ldc.i4.3
            IL_0083:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_0088:  brfalse.s  IL_00a3

            IL_008a:  ldarg.0
            IL_008b:  ldc.i4.4
            IL_008c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_0091:  brfalse.s  IL_009f

            IL_0093:  ldarg.0
            IL_0094:  ldc.i4.5
            IL_0095:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_009a:  ldc.i4.0
            IL_009b:  ceq
            IL_009d:  br.s       IL_00a0

            IL_009f:  ldc.i4.0
            IL_00a0:  stloc.0
            IL_00a1:  leave.s    IL_0111

            IL_00a3:  ldarg.0
            IL_00a4:  ldc.i4.6
            IL_00a5:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_00aa:  brtrue.s   IL_00b5

            IL_00ac:  ldarg.0
            IL_00ad:  ldc.i4.7
            IL_00ae:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_00b3:  brfalse.s  IL_00cc

            IL_00b5:  ldarg.0
            IL_00b6:  ldc.i4.8
            IL_00b7:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_00bc:  brtrue.s   IL_00c8

            IL_00be:  ldarg.0
            IL_00bf:  ldc.i4.s   9
            IL_00c1:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_00c6:  br.s       IL_00c9

            IL_00c8:  ldc.i4.1
            IL_00c9:  stloc.0
            IL_00ca:  leave.s    IL_0111

            IL_00cc:  leave.s    IL_00db

          }  // end .try
          catch [mscorlib]System.Object 
          {
            IL_00ce:  pop
            IL_00cf:  ldstr      "Catch2"
            IL_00d4:  call       void [mscorlib]System.Console::WriteLine(string)
            IL_00d9:  leave.s    IL_00db

          }  // end handler
          IL_00db:  ldarg.0
          IL_00dc:  ldc.i4.s   10
          IL_00de:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
          IL_00e3:  brfalse.s  IL_00ef

          IL_00e5:  ldarg.0
          IL_00e6:  ldc.i4.s   11
          IL_00e8:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
          IL_00ed:  br.s       IL_00f0

          IL_00ef:  ldc.i4.0
          IL_00f0:  stloc.0
          IL_00f1:  leave.s    IL_0111

        }  // end .try
        catch [mscorlib]System.Object 
        {
          IL_00f3:  pop
          IL_00f4:  ldstr      "Catch"
          IL_00f9:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_00fe:  leave.s    IL_0100

        }  // end handler
        IL_0100:  leave.s    IL_010d

      }  // end .try
      finally
      {
        IL_0102:  ldstr      "Finally"
        IL_0107:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_010c:  endfinally
      }  // end handler
      IL_010d:  leave.s    IL_010f

    }  // end handler
    IL_010f:  ldc.i4.0
    IL_0110:  ret

    IL_0111:  ldloc.0
    IL_0112:  ret
  } // end of method ExceptionHandling::ComplexConditionalReturnInThrow

  .method public hidebysig instance void 
          AppropriateLockExit() cil managed
  {
    // Code size       73 (0x49)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling V_2)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldc.i4.0
    IL_0003:  stloc.1
    .try
    {
      IL_0004:  ldarg.0
      IL_0005:  dup
      IL_0006:  stloc.2
      IL_0007:  ldloca.s   V_1
      IL_0009:  call       void [mscorlib]System.Threading.Monitor::Enter(object,
                                                                          bool&)
      IL_000e:  ldloc.0
      IL_000f:  ldc.i4     0x100
      IL_0014:  bgt.s      IL_001e

      IL_0016:  ldc.i4.0
      IL_0017:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_001c:  br.s       IL_003c

      IL_001e:  ldloc.0
      IL_001f:  ldc.i4     0x400
      IL_0024:  bgt.s      IL_002e

      IL_0026:  ldc.i4.1
      IL_0027:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_002c:  br.s       IL_003c

      IL_002e:  ldloc.0
      IL_002f:  ldc.i4     0x4000
      IL_0034:  bgt.s      IL_003c

      IL_0036:  ldc.i4.2
      IL_0037:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_003c:  leave.s    IL_0048

    }  // end .try
    finally
    {
      IL_003e:  ldloc.1
      IL_003f:  brfalse.s  IL_0047

      IL_0041:  ldloc.2
      IL_0042:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0047:  endfinally
    }  // end handler
    IL_0048:  ret
  } // end of method ExceptionHandling::AppropriateLockExit

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
