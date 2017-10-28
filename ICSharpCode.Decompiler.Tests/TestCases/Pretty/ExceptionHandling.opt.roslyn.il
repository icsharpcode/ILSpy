
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
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
  //  .custom instance void [mscorlib]System.Diagnostics.DebuggableAttribute::.ctor(valuetype [mscorlib]System.Diagnostics.DebuggableAttribute/DebuggingModes) = ( 01 00 02 00 00 00 00 00 ) 

  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module ExceptionHandling.dll
// MVID: {A4875794-8C32-4802-A38C-B190A38BD06B}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x029B0000


// =============== CLASS MEMBERS DECLARATION ===================

.class public abstract auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling
       extends [mscorlib]System.Object
{
  .class auto ansi sealed nested private beforefieldinit '<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling '<>4__this'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> '<>u__1'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       240 (0xf0)
      .maxstack  3
      .locals init (int32 V_0,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling V_1,
               bool V_2,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> V_3,
               class [mscorlib]System.Exception V_4,
               class [mscorlib]System.Exception V_5)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>4__this'
      IL_000d:  stloc.1
      .try
      {
        IL_000e:  ldloc.0
        IL_000f:  pop
        IL_0010:  nop
        .try
        {
          IL_0011:  ldloc.0
          IL_0012:  brfalse.s  IL_0056

          IL_0014:  ldstr      "Try"
          IL_0019:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_001e:  ldloc.1
          IL_001f:  callvirt   instance class [mscorlib]System.Threading.Tasks.Task`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::T()
          IL_0024:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [mscorlib]System.Threading.Tasks.Task`1<bool>::GetAwaiter()
          IL_0029:  stloc.3
          IL_002a:  ldloca.s   V_3
          IL_002c:  call       instance bool valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>::get_IsCompleted()
          IL_0031:  brtrue.s   IL_0072

          IL_0033:  ldarg.0
          IL_0034:  ldc.i4.0
          IL_0035:  dup
          IL_0036:  stloc.0
          IL_0037:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>1__state'
          IL_003c:  ldarg.0
          IL_003d:  ldloc.3
          IL_003e:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>u__1'
          IL_0043:  ldarg.0
          IL_0044:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>t__builder'
          IL_0049:  ldloca.s   V_3
          IL_004b:  ldarg.0
          IL_004c:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'>(!!0&,
                                                                                                                                                                                                                                                                                                                                                                          !!1&)
          IL_0051:  leave      IL_00ef

          IL_0056:  ldarg.0
          IL_0057:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>u__1'
          IL_005c:  stloc.3
          IL_005d:  ldarg.0
          IL_005e:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>u__1'
          IL_0063:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>
          IL_0069:  ldarg.0
          IL_006a:  ldc.i4.m1
          IL_006b:  dup
          IL_006c:  stloc.0
          IL_006d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>1__state'
          IL_0072:  ldloca.s   V_3
          IL_0074:  call       instance !0 valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>::GetResult()
          IL_0079:  stloc.2
          IL_007a:  leave.s    IL_00db

        }  // end .try
        filter
        {
          IL_007c:  isinst     [mscorlib]System.Exception
          IL_0081:  dup
          IL_0082:  brtrue.s   IL_0088

          IL_0084:  pop
          IL_0085:  ldc.i4.0
          IL_0086:  br.s       IL_00a3

          IL_0088:  stloc.s    V_4
          IL_008a:  ldloc.s    V_4
          IL_008c:  isinst     [mscorlib]System.ArgumentException
          IL_0091:  brtrue.s   IL_009f

          IL_0093:  ldloc.s    V_4
          IL_0095:  isinst     [mscorlib]System.IO.IOException
          IL_009a:  ldnull
          IL_009b:  cgt.un
          IL_009d:  br.s       IL_00a0

          IL_009f:  ldc.i4.1
          IL_00a0:  ldc.i4.0
          IL_00a1:  cgt.un
          IL_00a3:  endfilter
        }  // end filter
        {  // handler
          IL_00a5:  pop
          IL_00a6:  ldstr      "CatchException ex: "
          IL_00ab:  ldloc.s    V_4
          IL_00ad:  callvirt   instance string [mscorlib]System.Object::ToString()
          IL_00b2:  call       string [mscorlib]System.String::Concat(string,
                                                                      string)
          IL_00b7:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_00bc:  leave.s    IL_00be

        }  // end handler
        IL_00be:  ldc.i4.0
        IL_00bf:  stloc.2
        IL_00c0:  leave.s    IL_00db

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_00c2:  stloc.s    V_5
        IL_00c4:  ldarg.0
        IL_00c5:  ldc.i4.s   -2
        IL_00c7:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>1__state'
        IL_00cc:  ldarg.0
        IL_00cd:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>t__builder'
        IL_00d2:  ldloc.s    V_5
        IL_00d4:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::SetException(class [mscorlib]System.Exception)
        IL_00d9:  leave.s    IL_00ef

      }  // end handler
      IL_00db:  ldarg.0
      IL_00dc:  ldc.i4.s   -2
      IL_00de:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>1__state'
      IL_00e3:  ldarg.0
      IL_00e4:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>t__builder'
      IL_00e9:  ldloc.2
      IL_00ea:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::SetResult(!0)
      IL_00ef:  ret
    } // end of method '<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
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
    .locals init (bool V_0,
             class [mscorlib]System.Exception V_1)
    .try
    {
      IL_0000:  ldstr      "Try"
      IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_000a:  ldarg.0
      IL_000b:  newobj     instance void [mscorlib]System.Random::.ctor()
      IL_0010:  callvirt   instance int32 [mscorlib]System.Random::Next()
      IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_001a:  stloc.0
      IL_001b:  leave.s    IL_0037

    }  // end .try
    catch [mscorlib]System.Exception 
    {
      IL_001d:  stloc.1
      IL_001e:  ldstr      "CatchException ex: "
      IL_0023:  ldloc.1
      IL_0024:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_0029:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_002e:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0033:  leave.s    IL_0035

    }  // end handler
    IL_0035:  ldc.i4.0
    IL_0036:  ret

    IL_0037:  ldloc.0
    IL_0038:  ret
  } // end of method ExceptionHandling::SimpleTryCatchExceptionWithName

  .method public hidebysig instance bool 
          SimpleTryCatchExceptionWithNameAndCondition() cil managed
  {
    // Code size       91 (0x5b)
    .maxstack  2
    .locals init (bool V_0,
             class [mscorlib]System.Exception V_1)
    .try
    {
      IL_0000:  ldstr      "Try"
      IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_000a:  ldarg.0
      IL_000b:  newobj     instance void [mscorlib]System.Random::.ctor()
      IL_0010:  callvirt   instance int32 [mscorlib]System.Random::Next()
      IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_001a:  stloc.0
      IL_001b:  leave.s    IL_0059

    }  // end .try
    filter
    {
      IL_001d:  isinst     [mscorlib]System.Exception
      IL_0022:  dup
      IL_0023:  brtrue.s   IL_0029

      IL_0025:  pop
      IL_0026:  ldc.i4.0
      IL_0027:  br.s       IL_003d

      IL_0029:  stloc.1
      IL_002a:  ldloc.1
      IL_002b:  callvirt   instance string [mscorlib]System.Exception::get_Message()
      IL_0030:  ldstr      "test"
      IL_0035:  callvirt   instance bool [mscorlib]System.String::Contains(string)
      IL_003a:  ldc.i4.0
      IL_003b:  cgt.un
      IL_003d:  endfilter
    }  // end filter
    {  // handler
      IL_003f:  pop
      IL_0040:  ldstr      "CatchException ex: "
      IL_0045:  ldloc.1
      IL_0046:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_004b:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_0050:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0055:  leave.s    IL_0057

    }  // end handler
    IL_0057:  ldc.i4.0
    IL_0058:  ret

    IL_0059:  ldloc.0
    IL_005a:  ret
  } // end of method ExceptionHandling::SimpleTryCatchExceptionWithNameAndCondition

  .method public hidebysig instance bool 
          SimpleTryCatchExceptionWithNameAndConditionWithOr() cil managed
  {
    // Code size       95 (0x5f)
    .maxstack  2
    .locals init (bool V_0,
             class [mscorlib]System.Exception V_1)
    .try
    {
      IL_0000:  ldstr      "Try"
      IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_000a:  ldarg.0
      IL_000b:  newobj     instance void [mscorlib]System.Random::.ctor()
      IL_0010:  callvirt   instance int32 [mscorlib]System.Random::Next()
      IL_0015:  callvirt   instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling::B(int32)
      IL_001a:  stloc.0
      IL_001b:  leave.s    IL_005d

    }  // end .try
    filter
    {
      IL_001d:  isinst     [mscorlib]System.Exception
      IL_0022:  dup
      IL_0023:  brtrue.s   IL_0029

      IL_0025:  pop
      IL_0026:  ldc.i4.0
      IL_0027:  br.s       IL_0041

      IL_0029:  stloc.1
      IL_002a:  ldloc.1
      IL_002b:  isinst     [mscorlib]System.ArgumentException
      IL_0030:  brtrue.s   IL_003d

      IL_0032:  ldloc.1
      IL_0033:  isinst     [mscorlib]System.IO.IOException
      IL_0038:  ldnull
      IL_0039:  cgt.un
      IL_003b:  br.s       IL_003e

      IL_003d:  ldc.i4.1
      IL_003e:  ldc.i4.0
      IL_003f:  cgt.un
      IL_0041:  endfilter
    }  // end filter
    {  // handler
      IL_0043:  pop
      IL_0044:  ldstr      "CatchException ex: "
      IL_0049:  ldloc.1
      IL_004a:  callvirt   instance string [mscorlib]System.Object::ToString()
      IL_004f:  call       string [mscorlib]System.String::Concat(string,
                                                                  string)
      IL_0054:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0059:  leave.s    IL_005b

    }  // end handler
    IL_005b:  ldc.i4.0
    IL_005c:  ret

    IL_005d:  ldloc.0
    IL_005e:  ret
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
    // Code size       57 (0x39)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldarg.0
    IL_0003:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>4__this'
    IL_0008:  ldloca.s   V_0
    IL_000a:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::Create()
    IL_000f:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>t__builder'
    IL_0014:  ldloca.s   V_0
    IL_0016:  ldc.i4.m1
    IL_0017:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>1__state'
    IL_001c:  ldloc.0
    IL_001d:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>t__builder'
    IL_0022:  stloc.1
    IL_0023:  ldloca.s   V_1
    IL_0025:  ldloca.s   V_0
    IL_0027:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'>(!!0&)
    IL_002c:  ldloca.s   V_0
    IL_002e:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.ExceptionHandling/'<SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr>d__8'::'<>t__builder'
    IL_0033:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<!0> valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::get_Task()
    IL_0038:  ret
  } // end of method ExceptionHandling::SimpleAsyncTryCatchExceptionWithNameAndConditionWithOr

  .method public hidebysig instance void 
          CatchWhenWithConditionWithoutExceptionVar() cil managed
  {
    // Code size       44 (0x2c)
    .maxstack  2
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    .try
    {
      IL_0002:  newobj     instance void [mscorlib]System.Exception::.ctor()
      IL_0007:  throw

    }  // end .try
    filter
    {
      IL_0008:  isinst     [mscorlib]System.Exception
      IL_000d:  dup
      IL_000e:  brtrue.s   IL_0014

      IL_0010:  pop
      IL_0011:  ldc.i4.0
      IL_0012:  br.s       IL_001c

      IL_0014:  pop
      IL_0015:  ldloc.0
      IL_0016:  ldc.i4.0
      IL_0017:  ceq
      IL_0019:  ldc.i4.0
      IL_001a:  cgt.un
      IL_001c:  endfilter
    }  // end filter
    {  // handler
      IL_001e:  pop
      IL_001f:  ldstr      "jo"
      IL_0024:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0029:  leave.s    IL_002b

    }  // end handler
    IL_002b:  ret
  } // end of method ExceptionHandling::CatchWhenWithConditionWithoutExceptionVar

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
    // Code size       36 (0x24)
    .maxstack  1
    .try
    {
      .try
      {
        IL_0000:  ldstr      "Try"
        IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_000a:  leave.s    IL_0023

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_000c:  callvirt   instance string [mscorlib]System.Exception::get_Message()
        IL_0011:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0016:  leave.s    IL_0023

      }  // end handler
    }  // end .try
    finally
    {
      IL_0018:  ldstr      "Finally"
      IL_001d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0022:  endfinally
    }  // end handler
    IL_0023:  ret
  } // end of method ExceptionHandling::TryCatchFinally

  .method public hidebysig instance void 
          TryCatchMultipleHandlers() cil managed
  {
    // Code size       50 (0x32)
    .maxstack  1
    .try
    {
      IL_0000:  ldstr      "Try"
      IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_000a:  leave.s    IL_0031

    }  // end .try
    catch [mscorlib]System.InvalidOperationException 
    {
      IL_000c:  callvirt   instance string [mscorlib]System.Exception::get_Message()
      IL_0011:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0016:  leave.s    IL_0031

    }  // end handler
    catch [mscorlib]System.SystemException 
    {
      IL_0018:  callvirt   instance string [mscorlib]System.Exception::get_Message()
      IL_001d:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_0022:  leave.s    IL_0031

    }  // end handler
    catch [mscorlib]System.Object 
    {
      IL_0024:  pop
      IL_0025:  ldstr      "other"
      IL_002a:  call       void [mscorlib]System.Console::WriteLine(string)
      IL_002f:  leave.s    IL_0031

    }  // end handler
    IL_0031:  ret
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
