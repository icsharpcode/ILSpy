



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
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module ExceptionHandling.dll
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
    // Code size       43 (0x2b)
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
      IL_0009:  ldc.i4.0
      IL_000a:  ceq
      IL_000c:  stloc.1
      IL_000d:  ldloc.1
      IL_000e:  brtrue.s   IL_001b

      IL_0010:  nop
      IL_0011:  ldarg.0
      IL_0012:  ldc.i4.1
      IL_0013:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_0018:  stloc.0
      IL_0019:  leave.s    IL_0028

      IL_001b:  nop
      IL_001c:  leave.s    IL_0023

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_001e:  pop
      IL_001f:  nop
      IL_0020:  nop
      IL_0021:  leave.s    IL_0023

    }  // end handler
    IL_0023:  nop
    IL_0024:  ldc.i4.0
    IL_0025:  stloc.0
    IL_0026:  br.s       IL_0028

    IL_0028:  nop
    IL_0029:  ldloc.0
    IL_002a:  ret
  } // end of method ExceptionHandling::ConditionalReturnInThrow

  .method public hidebysig instance bool 
          SimpleTryCatchException() cil managed
  {
    // Code size       56 (0x38)
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
      IL_001e:  leave.s    IL_0035

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
    IL_0030:  nop
    IL_0031:  ldc.i4.0
    IL_0032:  stloc.0
    IL_0033:  br.s       IL_0035

    IL_0035:  nop
    IL_0036:  ldloc.0
    IL_0037:  ret
  } // end of method ExceptionHandling::SimpleTryCatchException

  .method public hidebysig instance bool 
          SimpleTryCatchExceptionWithName() cil managed
  {
    // Code size       67 (0x43)
    .maxstack  2
    .locals init (class [mscorlib]System.Exception V_0,
             bool V_1)
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
      IL_001d:  stloc.1
      IL_001e:  leave.s    IL_0040

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_0020:  stloc.0
      IL_0021:  nop
      IL_0022:  ldstr      "CatchException ex: "
      IL_0027:  ldloc.0
      IL_0028:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_002d:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_0032:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0037:  nop
      IL_0038:  nop
      IL_0039:  leave.s    IL_003b

    }  // end handler
    IL_003b:  nop
    IL_003c:  ldc.i4.0
    IL_003d:  stloc.1
    IL_003e:  br.s       IL_0040

    IL_0040:  nop
    IL_0041:  ldloc.1
    IL_0042:  ret
  } // end of method ExceptionHandling::SimpleTryCatchExceptionWithName

  .method public hidebysig instance bool 
          SimpleTryFinally() cil managed
  {
    // Code size       37 (0x25)
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
    IL_001e:  nop
    IL_001f:  ldc.i4.0
    IL_0020:  stloc.0
    IL_0021:  br.s       IL_0023

    IL_0023:  ldloc.0
    IL_0024:  ret
  } // end of method ExceptionHandling::SimpleTryFinally

  .method public hidebysig instance void 
          MethodEndingWithEndFinally() cil managed
  {
    // Code size       13 (0xd)
    .maxstack  1
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldnull
      IL_0003:  throw

    }  // end .try
    finally
    {
      IL_0004:  nop
      IL_0005:  call       void [mscorlib]System.Console::WriteLine()
      IL_000a:  nop
      IL_000b:  nop
      IL_000c:  endfinally
    }  // end handler
  } // end of method ExceptionHandling::MethodEndingWithEndFinally

  .method public hidebysig instance void 
          MethodEndingWithRethrow() cil managed
  {
    // Code size       8 (0x8)
    .maxstack  1
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldnull
      IL_0003:  throw

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_0004:  pop
      IL_0005:  nop
      IL_0006:  rethrow
    }  // end handler
  } // end of method ExceptionHandling::MethodEndingWithRethrow

  .method public hidebysig instance void 
          TryCatchFinally() cil managed
  {
    // Code size       52 (0x34)
    .maxstack  1
    .locals init (class [mscorlib]System.Exception V_0)
    IL_0000:  nop
    .try
    {
      .try
      {
        IL_0001:  nop
        IL_0002:  ldstr      "Try"
        IL_0007:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_000c:  nop
        IL_000d:  nop
        IL_000e:  leave.s    IL_0021

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0010:  stloc.0
        IL_0011:  nop
        IL_0012:  ldloc.0
        IL_0013:  callvirt   instance string [mscorlib]System.Exception::get_Message()
        IL_0018:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_001d:  nop
        IL_001e:  nop
        IL_001f:  leave.s    IL_0021

      }  // end handler
      IL_0021:  nop
      IL_0022:  leave.s    IL_0032

    }  // end .try
    finally
    {
      IL_0024:  nop
      IL_0025:  ldstr      "Finally"
      IL_002a:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_002f:  nop
      IL_0030:  nop
      IL_0031:  endfinally
    }  // end handler
    IL_0032:  nop
    IL_0033:  ret
  } // end of method ExceptionHandling::TryCatchFinally

  .method public hidebysig instance void 
          TryCatchMultipleHandlers() cil managed
  {
    // Code size       68 (0x44)
    .maxstack  1
    .locals init (class [mscorlib]System.InvalidOperationException V_0,
             class [mscorlib]System.SystemException V_1)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldstr      "Try"
      IL_0007:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_000c:  nop
      IL_000d:  nop
      IL_000e:  leave.s    IL_0042

    }  // end .try
    catch [mscorlib]System.InvalidOperationException 
    {
      IL_0010:  stloc.0
      IL_0011:  nop
      IL_0012:  ldloc.0
      IL_0013:  callvirt   instance string [mscorlib]System.Exception::get_Message()
      IL_0018:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_001d:  nop
      IL_001e:  nop
      IL_001f:  leave.s    IL_0042

    }  // end handler
    catch [mscorlib]System.SystemException 
    {
      IL_0021:  stloc.1
      IL_0022:  nop
      IL_0023:  ldloc.1
      IL_0024:  callvirt   instance string [mscorlib]System.Exception::get_Message()
      IL_0029:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_002e:  nop
      IL_002f:  nop
      IL_0030:  leave.s    IL_0042

    }  // end handler
    catch [mscorlib]System.Object 
    {
      IL_0032:  pop
      IL_0033:  nop
      IL_0034:  ldstr      "other"
      IL_0039:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_003e:  nop
      IL_003f:  nop
      IL_0040:  leave.s    IL_0042

    }  // end handler
    IL_0042:  nop
    IL_0043:  ret
  } // end of method ExceptionHandling::TryCatchMultipleHandlers

  .method public hidebysig instance void 
          NoUsingStatementBecauseTheVariableIsAssignedTo() cil managed
  {
    // Code size       35 (0x23)
    .maxstack  2
    .locals init (class [mscorlib]System.Threading.CancellationTokenSource V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldnull
    IL_0002:  stloc.0
    .try
    {
      IL_0003:  nop
      IL_0004:  newobj     instance void [mscorlib]System.Threading.CancellationTokenSource::.ctor()
      IL_0009:  stloc.0
      IL_000a:  nop
      IL_000b:  leave.s    IL_0021

    }  // end .try
    finally
    {
      IL_000d:  nop
      IL_000e:  ldloc.0
      IL_000f:  ldnull
      IL_0010:  ceq
      IL_0012:  stloc.1
      IL_0013:  ldloc.1
      IL_0014:  brtrue.s   IL_001f

      IL_0016:  nop
      IL_0017:  ldloc.0
      IL_0018:  callvirt   instance void [mscorlib]System.Threading.CancellationTokenSource::Dispose()
      IL_001d:  nop
      IL_001e:  nop
      IL_001f:  nop
      IL_0020:  endfinally
    }  // end handler
    IL_0021:  nop
    IL_0022:  ret
  } // end of method ExceptionHandling::NoUsingStatementBecauseTheVariableIsAssignedTo

  .method public hidebysig instance void 
          ThrowInFinally() cil managed
  {
    // Code size       14 (0xe)
    .maxstack  1
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  nop
      IL_0003:  leave.s    IL_000c

    }  // end .try
    finally
    {
      IL_0005:  nop
      IL_0006:  newobj     instance void [mscorlib]System.Exception::.ctor()
      IL_000b:  throw

    }  // end handler
    IL_000c:  br.s       IL_000c
  } // end of method ExceptionHandling::ThrowInFinally

  .method public hidebysig instance bool 
          EarlyExitInLoopTry() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  2
    .locals init (bool V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  br.s       IL_0025

    IL_0003:  nop
    .try
    {
      IL_0004:  nop
      IL_0005:  ldarg.0
      IL_0006:  ldc.i4.0
      IL_0007:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_000c:  stloc.1
      IL_000d:  ldloc.1
      IL_000e:  brtrue.s   IL_0015

      IL_0010:  nop
      IL_0011:  ldc.i4.0
      IL_0012:  stloc.0
      IL_0013:  leave.s    IL_0029

      IL_0015:  call       void [mscorlib]System.Console::WriteLine()
      IL_001a:  nop
      IL_001b:  nop
      IL_001c:  leave.s    IL_0023

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_001e:  pop
      IL_001f:  nop
      IL_0020:  nop
      IL_0021:  leave.s    IL_0023

    }  // end handler
    IL_0023:  nop
    IL_0024:  nop
    IL_0025:  ldc.i4.1
    IL_0026:  stloc.1
    IL_0027:  br.s       IL_0003

    IL_0029:  nop
    IL_002a:  ldloc.0
    IL_002b:  ret
  } // end of method ExceptionHandling::EarlyExitInLoopTry

  .method public hidebysig instance bool 
          ComplexConditionalReturnInThrow() cil managed
  {
    // Code size       348 (0x15c)
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
      IL_0009:  ldc.i4.0
      IL_000a:  ceq
      IL_000c:  stloc.1
      IL_000d:  ldloc.1
      IL_000e:  brtrue.s   IL_006e

      IL_0010:  nop
      IL_0011:  ldarg.0
      IL_0012:  ldc.i4.1
      IL_0013:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_0018:  ldc.i4.0
      IL_0019:  ceq
      IL_001b:  stloc.1
      IL_001c:  ldloc.1
      IL_001d:  brtrue.s   IL_0038

      IL_001f:  nop
      IL_0020:  ldstr      "0 && 1"
      IL_0025:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_002a:  nop
      IL_002b:  ldarg.0
      IL_002c:  ldc.i4.2
      IL_002d:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_0032:  stloc.0
      IL_0033:  leave      IL_0159

      IL_0038:  ldarg.0
      IL_0039:  ldc.i4.3
      IL_003a:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_003f:  ldc.i4.0
      IL_0040:  ceq
      IL_0042:  stloc.1
      IL_0043:  ldloc.1
      IL_0044:  brtrue.s   IL_0062

      IL_0046:  nop
      IL_0047:  ldstr      "0 && 3"
      IL_004c:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0051:  nop
      IL_0052:  ldarg.0
      IL_0053:  ldc.i4.2
      IL_0054:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_0059:  ldc.i4.0
      IL_005a:  ceq
      IL_005c:  stloc.0
      IL_005d:  leave      IL_0159

      IL_0062:  ldstr      "0"
      IL_0067:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_006c:  nop
      IL_006d:  nop
      IL_006e:  ldstr      "End Try"
      IL_0073:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0078:  nop
      IL_0079:  nop
      IL_007a:  leave      IL_0154

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_007f:  pop
      IL_0080:  nop
      .try
      {
        .try
        {
          IL_0081:  nop
          .try
          {
            IL_0082:  nop
            IL_0083:  ldarg.0
            IL_0084:  ldc.i4.0
            IL_0085:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_008a:  brtrue.s   IL_0095

            IL_008c:  ldarg.0
            IL_008d:  ldc.i4.1
            IL_008e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_0093:  brfalse.s  IL_009e

            IL_0095:  ldarg.0
            IL_0096:  ldc.i4.2
            IL_0097:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_009c:  brtrue.s   IL_00aa

            IL_009e:  ldarg.0
            IL_009f:  ldc.i4.3
            IL_00a0:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_00a5:  ldc.i4.0
            IL_00a6:  ceq
            IL_00a8:  br.s       IL_00ab

            IL_00aa:  ldc.i4.0
            IL_00ab:  nop
            IL_00ac:  stloc.1
            IL_00ad:  ldloc.1
            IL_00ae:  brtrue.s   IL_00ce

            IL_00b0:  nop
            IL_00b1:  ldarg.0
            IL_00b2:  ldc.i4.4
            IL_00b3:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_00b8:  brfalse.s  IL_00c6

            IL_00ba:  ldarg.0
            IL_00bb:  ldc.i4.5
            IL_00bc:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_00c1:  ldc.i4.0
            IL_00c2:  ceq
            IL_00c4:  br.s       IL_00c7

            IL_00c6:  ldc.i4.0
            IL_00c7:  nop
            IL_00c8:  stloc.0
            IL_00c9:  leave      IL_0159

            IL_00ce:  ldarg.0
            IL_00cf:  ldc.i4.6
            IL_00d0:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_00d5:  brtrue.s   IL_00e3

            IL_00d7:  ldarg.0
            IL_00d8:  ldc.i4.7
            IL_00d9:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_00de:  ldc.i4.0
            IL_00df:  ceq
            IL_00e1:  br.s       IL_00e4

            IL_00e3:  ldc.i4.0
            IL_00e4:  nop
            IL_00e5:  stloc.1
            IL_00e6:  ldloc.1
            IL_00e7:  brtrue.s   IL_0102

            IL_00e9:  nop
            IL_00ea:  ldarg.0
            IL_00eb:  ldc.i4.8
            IL_00ec:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_00f1:  brtrue.s   IL_00fd

            IL_00f3:  ldarg.0
            IL_00f4:  ldc.i4.s   9
            IL_00f6:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_00fb:  br.s       IL_00fe

            IL_00fd:  ldc.i4.1
            IL_00fe:  nop
            IL_00ff:  stloc.0
            IL_0100:  leave.s    IL_0159

            IL_0102:  nop
            IL_0103:  leave.s    IL_0115

          }  // end .try
          catch [mscorlib]System.Object 
          {
            IL_0105:  pop
            IL_0106:  nop
            IL_0107:  ldstr      "Catch2"
            IL_010c:  call       void [mscorlib]System.Console::WriteLine(string)
            IL_0111:  nop
            IL_0112:  nop
            IL_0113:  leave.s    IL_0115

          }  // end handler
          IL_0115:  nop
          IL_0116:  ldarg.0
          IL_0117:  ldc.i4.s   10
          IL_0119:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
          IL_011e:  brfalse.s  IL_012a

          IL_0120:  ldarg.0
          IL_0121:  ldc.i4.s   11
          IL_0123:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
          IL_0128:  br.s       IL_012b

          IL_012a:  ldc.i4.0
          IL_012b:  nop
          IL_012c:  stloc.0
          IL_012d:  leave.s    IL_0159

        }  // end .try
        catch [mscorlib]System.Object 
        {
          IL_012f:  pop
          IL_0130:  nop
          IL_0131:  ldstr      "Catch"
          IL_0136:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_013b:  nop
          IL_013c:  nop
          IL_013d:  leave.s    IL_013f

        }  // end handler
        IL_013f:  nop
        IL_0140:  leave.s    IL_0150

      }  // end .try
      finally
      {
        IL_0142:  nop
        IL_0143:  ldstr      "Finally"
        IL_0148:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_014d:  nop
        IL_014e:  nop
        IL_014f:  endfinally
      }  // end handler
      IL_0150:  nop
      IL_0151:  nop
      IL_0152:  leave.s    IL_0154

    }  // end handler
    IL_0154:  nop
    IL_0155:  ldc.i4.0
    IL_0156:  stloc.0
    IL_0157:  br.s       IL_0159

    IL_0159:  nop
    IL_015a:  ldloc.0
    IL_015b:  ret
  } // end of method ExceptionHandling::ComplexConditionalReturnInThrow

  .method public hidebysig instance void 
          AppropriateLockExit() cil managed
  {
    // Code size       105 (0x69)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling V_2,
             bool V_3)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  ldc.i4.0
    IL_0004:  stloc.1
    .try
    {
      IL_0005:  ldarg.0
      IL_0006:  dup
      IL_0007:  stloc.2
      IL_0008:  ldloca.s   V_1
      IL_000a:  call       void [mscorlib]System.Threading.Monitor::Enter(object,
                                                                          bool&)
      IL_000f:  nop
      IL_0010:  nop
      IL_0011:  ldloc.0
      IL_0012:  ldc.i4     0x100
      IL_0017:  cgt
      IL_0019:  stloc.3
      IL_001a:  ldloc.3
      IL_001b:  brtrue.s   IL_0028

      IL_001d:  nop
      IL_001e:  ldc.i4.0
      IL_001f:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_0024:  nop
      IL_0025:  nop
      IL_0026:  br.s       IL_0054

      IL_0028:  ldloc.0
      IL_0029:  ldc.i4     0x400
      IL_002e:  cgt
      IL_0030:  stloc.3
      IL_0031:  ldloc.3
      IL_0032:  brtrue.s   IL_003f

      IL_0034:  nop
      IL_0035:  ldc.i4.1
      IL_0036:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_003b:  nop
      IL_003c:  nop
      IL_003d:  br.s       IL_0054

      IL_003f:  ldloc.0
      IL_0040:  ldc.i4     0x4000
      IL_0045:  cgt
      IL_0047:  stloc.3
      IL_0048:  ldloc.3
      IL_0049:  brtrue.s   IL_0054

      IL_004b:  nop
      IL_004c:  ldc.i4.2
      IL_004d:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_0052:  nop
      IL_0053:  nop
      IL_0054:  nop
      IL_0055:  leave.s    IL_0067

    }  // end .try
    finally
    {
      IL_0057:  ldloc.1
      IL_0058:  ldc.i4.0
      IL_0059:  ceq
      IL_005b:  stloc.3
      IL_005c:  ldloc.3
      IL_005d:  brtrue.s   IL_0066

      IL_005f:  ldloc.2
      IL_0060:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_0065:  nop
      IL_0066:  endfinally
    }  // end handler
    IL_0067:  nop
    IL_0068:  ret
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
