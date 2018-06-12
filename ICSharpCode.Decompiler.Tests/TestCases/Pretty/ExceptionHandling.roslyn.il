



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
  .class auto ansi sealed nested private beforefieldinit '<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'
         extends [mscorlib]System.Object
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling '<>4__this'
    .field private bool '<>s__1'
    .field private class [mscorlib]System.Exception '<ex>5__2'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> '<>u__1'
    .method public hidebysig specialname rtspecialname 
            instance void  .ctor() cil managed
    {
      // Code size       8 (0x8)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
      IL_0006:  nop
      IL_0007:  ret
    } // end of method '<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::.ctor

    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       295 (0x127)
      .maxstack  3
      .locals init (int32 V_0,
               bool V_1,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> V_2,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8' V_3,
               class [mscorlib]System.Exception V_4,
               bool V_5)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_000c

        IL_000a:  br.s       IL_000e

        IL_000c:  br.s       IL_000f

        IL_000e:  nop
        IL_000f:  nop
        .try
        {
          IL_0010:  ldloc.0
          IL_0011:  brfalse.s  IL_0015

          IL_0013:  br.s       IL_0017

          IL_0015:  br.s       IL_0064

          IL_0017:  nop
          IL_0018:  ldstr      "Try"
          IL_001d:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_0022:  nop
          IL_0023:  ldarg.0
          IL_0024:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>4__this'
          IL_0029:  callvirt   instance class [mscorlib]System.Threading.Tasks.Task`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::T()
          IL_002e:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [mscorlib]System.Threading.Tasks.Task`1<bool>::GetAwaiter()
          IL_0033:  stloc.2
          IL_0034:  ldloca.s   V_2
          IL_0036:  call       instance bool valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>::get_IsCompleted()
          IL_003b:  brtrue.s   IL_0080

          IL_003d:  ldarg.0
          IL_003e:  ldc.i4.0
          IL_003f:  dup
          IL_0040:  stloc.0
          IL_0041:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>1__state'
          IL_0046:  ldarg.0
          IL_0047:  ldloc.2
          IL_0048:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>u__1'
          IL_004d:  ldarg.0
          IL_004e:  stloc.3
          IL_004f:  ldarg.0
          IL_0050:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>t__builder'
          IL_0055:  ldloca.s   V_2
          IL_0057:  ldloca.s   V_3
          IL_0059:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>,class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'>(!!0&,
                                                                                                                                                                                                                                                                                                                                                                      !!1&)
          IL_005e:  nop
          IL_005f:  leave      IL_0126

          IL_0064:  ldarg.0
          IL_0065:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>u__1'
          IL_006a:  stloc.2
          IL_006b:  ldarg.0
          IL_006c:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>u__1'
          IL_0071:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>
          IL_0077:  ldarg.0
          IL_0078:  ldc.i4.m1
          IL_0079:  dup
          IL_007a:  stloc.0
          IL_007b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>1__state'
          IL_0080:  ldarg.0
          IL_0081:  ldloca.s   V_2
          IL_0083:  call       instance !0 valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>::GetResult()
          IL_0088:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>s__1'
          IL_008d:  ldarg.0
          IL_008e:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>s__1'
          IL_0093:  stloc.1
          IL_0094:  leave.s    IL_0111

        }  // end .try
        filter
        {
          IL_0096:  isinst     [mscorlib]System.Exception
          IL_009b:  dup
          IL_009c:  brtrue.s   IL_00a2

          IL_009e:  pop
          IL_009f:  ldc.i4.0
          IL_00a0:  br.s       IL_00d1

          IL_00a2:  stloc.s    V_4
          IL_00a4:  ldarg.0
          IL_00a5:  ldloc.s    V_4
          IL_00a7:  stfld      class [mscorlib]System.Exception ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<ex>5__2'
          IL_00ac:  ldarg.0
          IL_00ad:  ldfld      class [mscorlib]System.Exception ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<ex>5__2'
          IL_00b2:  isinst     [mscorlib]System.ArgumentException
          IL_00b7:  brtrue.s   IL_00c9

          IL_00b9:  ldarg.0
          IL_00ba:  ldfld      class [mscorlib]System.Exception ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<ex>5__2'
          IL_00bf:  isinst     [mscorlib]System.IO.IOException
          IL_00c4:  ldnull
          IL_00c5:  cgt.un
          IL_00c7:  br.s       IL_00ca

          IL_00c9:  ldc.i4.1
          IL_00ca:  stloc.s    V_5
          IL_00cc:  ldloc.s    V_5
          IL_00ce:  ldc.i4.0
          IL_00cf:  cgt.un
          IL_00d1:  endfilter
        }  // end filter
        {  // handler
          IL_00d3:  pop
          IL_00d4:  nop
          IL_00d5:  ldstr      "CatchException ex: "
          IL_00da:  ldarg.0
          IL_00db:  ldfld      class [mscorlib]System.Exception ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<ex>5__2'
          IL_00e0:  callvirt   instance string [mscorlib]System.Object::ToString()
          IL_00e5:  call       string [mscorlib]System.String::Concat(string,
                                                                      string)
          IL_00ea:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_00ef:  nop
          IL_00f0:  nop
          IL_00f1:  leave.s    IL_00f3

        }  // end handler
        IL_00f3:  ldc.i4.0
        IL_00f4:  stloc.1
        IL_00f5:  leave.s    IL_0111

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_00f7:  stloc.s    V_4
        IL_00f9:  ldarg.0
        IL_00fa:  ldc.i4.s   -2
        IL_00fc:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>1__state'
        IL_0101:  ldarg.0
        IL_0102:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>t__builder'
        IL_0107:  ldloc.s    V_4
        IL_0109:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::SetException(class [mscorlib]System.Exception)
        IL_010e:  nop
        IL_010f:  leave.s    IL_0126

      }  // end handler
      IL_0111:  ldarg.0
      IL_0112:  ldc.i4.s   -2
      IL_0114:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>1__state'
      IL_0119:  ldarg.0
      IL_011a:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>t__builder'
      IL_011f:  ldloc.1
      IL_0120:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::SetResult(!0)
      IL_0125:  nop
      IL_0126:  ret
    } // end of method '<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       1 (0x1)
      .maxstack  8
      IL_0000:  ret
    } // end of method '<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::SetStateMachine

  } // end of class '<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'

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
          SimpleTryCatchExceptionWithNameAndConditionWithOr() cil managed
  {
    // Code size       105 (0x69)
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
      IL_001e:  leave.s    IL_0067

    }  // end .try
    filter
    {
      IL_0020:  isinst     [mscorlib]System.Exception
      IL_0025:  dup
      IL_0026:  brtrue.s   IL_002c

      IL_0028:  pop
      IL_0029:  ldc.i4.0
      IL_002a:  br.s       IL_0046

      IL_002c:  stloc.1
      IL_002d:  ldloc.1
      IL_002e:  isinst     [mscorlib]System.ArgumentException
      IL_0033:  brtrue.s   IL_0040

      IL_0035:  ldloc.1
      IL_0036:  isinst     [mscorlib]System.IO.IOException
      IL_003b:  ldnull
      IL_003c:  cgt.un
      IL_003e:  br.s       IL_0041

      IL_0040:  ldc.i4.1
      IL_0041:  stloc.2
      IL_0042:  ldloc.2
      IL_0043:  ldc.i4.0
      IL_0044:  cgt.un
      IL_0046:  endfilter
    }  // end filter
    {  // handler
      IL_0048:  pop
      IL_0049:  nop
      IL_004a:  ldstr      "CatchException ex: "
      IL_004f:  ldloc.1
      IL_0050:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_0055:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_005a:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_005f:  nop
      IL_0060:  nop
      IL_0061:  leave.s    IL_0063

    }  // end handler
    IL_0063:  ldc.i4.0
    IL_0064:  stloc.0
    IL_0065:  br.s       IL_0067

    IL_0067:  ldloc.0
    IL_0068:  ret
  } // end of method ExceptionHandling::SimpleTryCatchExceptionWithNameAndConditionWithOr

  .method public hidebysig instance class [mscorlib]System.Threading.Tasks.Task`1<bool> 
          SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 7C 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..|ICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 45 78 63 65 70 74 69 6F 6E 48 61 6E 64 6C 69   // .ExceptionHandli
                                                                                                                                       6E 67 2B 3C 53 69 6D 70 6C 65 41 73 79 6E 63 54   // ng+<SimpleAsyncT
                                                                                                                                       72 79 43 61 74 63 68 45 78 63 65 70 74 69 6F 6E   // ryCatchException
                                                                                                                                       57 69 74 68 4E 61 6D 65 41 6E 64 43 6F 6E 64 69   // WithNameAndCondi
                                                                                                                                       74 69 6F 6E 57 69 74 68 4F 72 3E 64 5F 5F 38 00   // tionWithOr>d__8.
                                                                                                                                       00 ) 
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       59 (0x3b)
    .maxstack  2
    .locals init (class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> V_1)
    IL_0000:  newobj     instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldarg.0
    IL_0008:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>4__this'
    IL_000d:  ldloc.0
    IL_000e:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::Create()
    IL_0013:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>t__builder'
    IL_0018:  ldloc.0
    IL_0019:  ldc.i4.m1
    IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>1__state'
    IL_001f:  ldloc.0
    IL_0020:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>t__builder'
    IL_0025:  stloc.1
    IL_0026:  ldloca.s   V_1
    IL_0028:  ldloca.s   V_0
    IL_002a:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::Start<class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'>(!!0&)
    IL_002f:  ldloc.0
    IL_0030:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>t__builder'
    IL_0035:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<!0> valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::get_Task()
    IL_003a:  ret
  } // end of method ExceptionHandling::SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr

  .method public hidebysig instance void 
          CatchWhenWithConditionWithoutExceptionVar() cil managed
  {
    // Code size       51 (0x33)
    .maxstack  2
    .locals init (int32 V_0,
             bool V_1)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    .try
    {
      IL_0003:  nop
      IL_0004:  newobj     instance void [mscorlib]System.Exception::.ctor()
      IL_0009:  throw

    }  // end .try
    filter
    {
      IL_000a:  isinst     [mscorlib]System.Exception
      IL_000f:  dup
      IL_0010:  brtrue.s   IL_0016

      IL_0012:  pop
      IL_0013:  ldc.i4.0
      IL_0014:  br.s       IL_0020

      IL_0016:  pop
      IL_0017:  ldloc.0
      IL_0018:  ldc.i4.0
      IL_0019:  ceq
      IL_001b:  stloc.1
      IL_001c:  ldloc.1
      IL_001d:  ldc.i4.0
      IL_001e:  cgt.un
      IL_0020:  endfilter
    }  // end filter
    {  // handler
      IL_0022:  pop
      IL_0023:  nop
      IL_0024:  ldstr      "jo"
      IL_0029:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_002e:  nop
      IL_002f:  nop
      IL_0030:  leave.s    IL_0032

    }  // end handler
    IL_0032:  ret
  } // end of method ExceptionHandling::CatchWhenWithConditionWithoutExceptionVar

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
    // Code size       50 (0x32)
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
      IL_0021:  leave.s    IL_0031

    }  // end .try
    finally
    {
      IL_0023:  nop
      IL_0024:  ldstr      "Finally"
      IL_0029:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_002e:  nop
      IL_002f:  nop
      IL_0030:  endfinally
    }  // end handler
    IL_0031:  ret
  } // end of method ExceptionHandling::TryCatchFinally

  .method public hidebysig instance void 
          TryCatchMultipleHandlers() cil managed
  {
    // Code size       67 (0x43)
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
    IL_0042:  ret
  } // end of method ExceptionHandling::TryCatchMultipleHandlers

  .method public hidebysig instance void 
          NoUsingStatementBecauseTheVariableIsAssignedTo() cil managed
  {
    // Code size       34 (0x22)
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
      IL_0010:  cgt.un
      IL_0012:  stloc.1
      IL_0013:  ldloc.1
      IL_0014:  brfalse.s  IL_001f

      IL_0016:  nop
      IL_0017:  ldloc.0
      IL_0018:  callvirt   instance void [mscorlib]System.Threading.CancellationTokenSource::Dispose()
      IL_001d:  nop
      IL_001e:  nop
      IL_001f:  nop
      IL_0020:  endfinally
    }  // end handler
    IL_0021:  ret
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
    // Code size       45 (0x2d)
    .maxstack  2
    .locals init (bool V_0,
             bool V_1,
             bool V_2)
    IL_0000:  nop
    IL_0001:  br.s       IL_0027

    IL_0003:  nop
    .try
    {
      IL_0004:  nop
      IL_0005:  ldarg.0
      IL_0006:  ldc.i4.0
      IL_0007:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_000c:  ldc.i4.0
      IL_000d:  ceq
      IL_000f:  stloc.0
      IL_0010:  ldloc.0
      IL_0011:  brfalse.s  IL_0018

      IL_0013:  nop
      IL_0014:  ldc.i4.0
      IL_0015:  stloc.1
      IL_0016:  leave.s    IL_002b

      IL_0018:  call       void [mscorlib]System.Console::WriteLine()
      IL_001d:  nop
      IL_001e:  nop
      IL_001f:  leave.s    IL_0026

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_0021:  pop
      IL_0022:  nop
      IL_0023:  nop
      IL_0024:  leave.s    IL_0026

    }  // end handler
    IL_0026:  nop
    IL_0027:  ldc.i4.1
    IL_0028:  stloc.2
    IL_0029:  br.s       IL_0003

    IL_002b:  ldloc.1
    IL_002c:  ret
  } // end of method ExceptionHandling::EarlyExitInLoopTry

  .method public hidebysig instance bool 
          ComplexConditionalReturnInThrow() cil managed
  {
    // Code size       327 (0x147)
    .maxstack  2
    .locals init (bool V_0,
             bool V_1,
             bool V_2,
             bool V_3,
             bool V_4,
             bool V_5)
    IL_0000:  nop
    .try
    {
      IL_0001:  nop
      IL_0002:  ldarg.0
      IL_0003:  ldc.i4.0
      IL_0004:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_0009:  stloc.0
      IL_000a:  ldloc.0
      IL_000b:  brfalse.s  IL_0065

      IL_000d:  nop
      IL_000e:  ldarg.0
      IL_000f:  ldc.i4.1
      IL_0010:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_0015:  stloc.1
      IL_0016:  ldloc.1
      IL_0017:  brfalse.s  IL_0032

      IL_0019:  nop
      IL_001a:  ldstr      "0 && 1"
      IL_001f:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0024:  nop
      IL_0025:  ldarg.0
      IL_0026:  ldc.i4.2
      IL_0027:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_002c:  stloc.2
      IL_002d:  leave      IL_0145

      IL_0032:  ldarg.0
      IL_0033:  ldc.i4.3
      IL_0034:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_0039:  stloc.3
      IL_003a:  ldloc.3
      IL_003b:  brfalse.s  IL_0059

      IL_003d:  nop
      IL_003e:  ldstr      "0 && 3"
      IL_0043:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0048:  nop
      IL_0049:  ldarg.0
      IL_004a:  ldc.i4.2
      IL_004b:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_0050:  ldc.i4.0
      IL_0051:  ceq
      IL_0053:  stloc.2
      IL_0054:  leave      IL_0145

      IL_0059:  ldstr      "0"
      IL_005e:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0063:  nop
      IL_0064:  nop
      IL_0065:  ldstr      "End Try"
      IL_006a:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_006f:  nop
      IL_0070:  nop
      IL_0071:  leave      IL_0141

    }  // end .try
    catch [mscorlib]System.Object 
    {
      IL_0076:  pop
      IL_0077:  nop
      .try
      {
        .try
        {
          IL_0078:  nop
          .try
          {
            IL_0079:  nop
            IL_007a:  ldarg.0
            IL_007b:  ldc.i4.0
            IL_007c:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_0081:  brtrue.s   IL_008c

            IL_0083:  ldarg.0
            IL_0084:  ldc.i4.1
            IL_0085:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_008a:  brfalse.s  IL_0095

            IL_008c:  ldarg.0
            IL_008d:  ldc.i4.2
            IL_008e:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_0093:  brtrue.s   IL_009e

            IL_0095:  ldarg.0
            IL_0096:  ldc.i4.3
            IL_0097:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_009c:  br.s       IL_009f

            IL_009e:  ldc.i4.1
            IL_009f:  stloc.s    V_4
            IL_00a1:  ldloc.s    V_4
            IL_00a3:  brfalse.s  IL_00c2

            IL_00a5:  nop
            IL_00a6:  ldarg.0
            IL_00a7:  ldc.i4.4
            IL_00a8:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_00ad:  brfalse.s  IL_00bb

            IL_00af:  ldarg.0
            IL_00b0:  ldc.i4.5
            IL_00b1:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_00b6:  ldc.i4.0
            IL_00b7:  ceq
            IL_00b9:  br.s       IL_00bc

            IL_00bb:  ldc.i4.0
            IL_00bc:  stloc.2
            IL_00bd:  leave      IL_0145

            IL_00c2:  ldarg.0
            IL_00c3:  ldc.i4.6
            IL_00c4:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_00c9:  brtrue.s   IL_00d4

            IL_00cb:  ldarg.0
            IL_00cc:  ldc.i4.7
            IL_00cd:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_00d2:  br.s       IL_00d5

            IL_00d4:  ldc.i4.1
            IL_00d5:  stloc.s    V_5
            IL_00d7:  ldloc.s    V_5
            IL_00d9:  brfalse.s  IL_00f3

            IL_00db:  nop
            IL_00dc:  ldarg.0
            IL_00dd:  ldc.i4.8
            IL_00de:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_00e3:  brtrue.s   IL_00ef

            IL_00e5:  ldarg.0
            IL_00e6:  ldc.i4.s   9
            IL_00e8:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
            IL_00ed:  br.s       IL_00f0

            IL_00ef:  ldc.i4.1
            IL_00f0:  stloc.2
            IL_00f1:  leave.s    IL_0145

            IL_00f3:  nop
            IL_00f4:  leave.s    IL_0106

          }  // end .try
          catch [mscorlib]System.Object 
          {
            IL_00f6:  pop
            IL_00f7:  nop
            IL_00f8:  ldstr      "Catch2"
            IL_00fd:  call       void [mscorlib]System.Console::WriteLine(string)
            IL_0102:  nop
            IL_0103:  nop
            IL_0104:  leave.s    IL_0106

          }  // end handler
          IL_0106:  ldarg.0
          IL_0107:  ldc.i4.s   10
          IL_0109:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
          IL_010e:  brfalse.s  IL_011a

          IL_0110:  ldarg.0
          IL_0111:  ldc.i4.s   11
          IL_0113:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
          IL_0118:  br.s       IL_011b

          IL_011a:  ldc.i4.0
          IL_011b:  stloc.2
          IL_011c:  leave.s    IL_0145

        }  // end .try
        catch [mscorlib]System.Object 
        {
          IL_011e:  pop
          IL_011f:  nop
          IL_0120:  ldstr      "Catch"
          IL_0125:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_012a:  nop
          IL_012b:  nop
          IL_012c:  leave.s    IL_012e

        }  // end handler
        IL_012e:  leave.s    IL_013e

      }  // end .try
      finally
      {
        IL_0130:  nop
        IL_0131:  ldstr      "Finally"
        IL_0136:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_013b:  nop
        IL_013c:  nop
        IL_013d:  endfinally
      }  // end handler
      IL_013e:  nop
      IL_013f:  leave.s    IL_0141

    }  // end handler
    IL_0141:  ldc.i4.0
    IL_0142:  stloc.2
    IL_0143:  br.s       IL_0145

    IL_0145:  ldloc.2
    IL_0146:  ret
  } // end of method ExceptionHandling::ComplexConditionalReturnInThrow

  .method public hidebysig instance void 
          AppropriateLockExit() cil managed
  {
    // Code size       112 (0x70)
    .maxstack  2
    .locals init (int32 V_0,
             class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling V_1,
             bool V_2,
             bool V_3,
             bool V_4,
             bool V_5)
    IL_0000:  nop
    IL_0001:  ldc.i4.0
    IL_0002:  stloc.0
    IL_0003:  ldarg.0
    IL_0004:  stloc.1
    IL_0005:  ldc.i4.0
    IL_0006:  stloc.2
    .try
    {
      IL_0007:  ldloc.1
      IL_0008:  ldloca.s   V_2
      IL_000a:  call       void [mscorlib]System.Threading.Monitor::Enter(object,
                                                                          bool&)
      IL_000f:  nop
      IL_0010:  nop
      IL_0011:  ldloc.0
      IL_0012:  ldc.i4     0x100
      IL_0017:  cgt
      IL_0019:  ldc.i4.0
      IL_001a:  ceq
      IL_001c:  stloc.3
      IL_001d:  ldloc.3
      IL_001e:  brfalse.s  IL_002b

      IL_0020:  nop
      IL_0021:  ldc.i4.0
      IL_0022:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_0027:  nop
      IL_0028:  nop
      IL_0029:  br.s       IL_0061

      IL_002b:  ldloc.0
      IL_002c:  ldc.i4     0x400
      IL_0031:  cgt
      IL_0033:  ldc.i4.0
      IL_0034:  ceq
      IL_0036:  stloc.s    V_4
      IL_0038:  ldloc.s    V_4
      IL_003a:  brfalse.s  IL_0047

      IL_003c:  nop
      IL_003d:  ldc.i4.1
      IL_003e:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_0043:  nop
      IL_0044:  nop
      IL_0045:  br.s       IL_0061

      IL_0047:  ldloc.0
      IL_0048:  ldc.i4     0x4000
      IL_004d:  cgt
      IL_004f:  ldc.i4.0
      IL_0050:  ceq
      IL_0052:  stloc.s    V_5
      IL_0054:  ldloc.s    V_5
      IL_0056:  brfalse.s  IL_0061

      IL_0058:  nop
      IL_0059:  ldc.i4.2
      IL_005a:  call       void [mscorlib]System.Console::WriteLine(int32)
      IL_005f:  nop
      IL_0060:  nop
      IL_0061:  nop
      IL_0062:  leave.s    IL_006f

    }  // end .try
    finally
    {
      IL_0064:  ldloc.2
      IL_0065:  brfalse.s  IL_006e

      IL_0067:  ldloc.1
      IL_0068:  call       void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_006d:  nop
      IL_006e:  endfinally
    }  // end handler
    IL_006f:  ret
  } // end of method ExceptionHandling::AppropriateLockExit

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
