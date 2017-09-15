
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.0.30319.17929
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly '0bl3wgec'
{
  .custom instance void [mscorlib]System.Runtime.CompilerServices.RuntimeCompatibilityAttribute::.ctor() = ( 01 00 01 00 54 02 16 57 72 61 70 4E 6F 6E 45 78   // ....T..WrapNonEx
                                                                                                             63 65 70 74 69 6F 6E 54 68 72 6F 77 73 01 )       // ceptionThrows.
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilationRelaxationsAttribute::.ctor(int32) = ( 01 00 08 00 00 00 00 00 ) 
  .permissionset reqmin
             = {[mscorlib]System.Security.Permissions.SecurityPermissionAttribute = {property bool 'SkipVerification' = bool(true)}}
  .hash algorithm 0x00008004
  .ver 0:0:0:0
}
.module '0bl3wgec.dll'
// MVID: {D5634CE0-620A-4F18-AC07-9132CB05EFF7}
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00930000


// =============== CLASS MEMBERS DECLARATION ===================

.class public auto ansi beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async
       extends [mscorlib]System.Object
{
  .class auto ansi sealed nested private beforefieldinit '<SimpleVoidMethod>d__0'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter '<>u__$awaiter1'
    .field private object '<>t__stack'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       204 (0xcc)
      .maxstack  3
      .locals init (bool V_0,
               class [mscorlib]System.Exception V_1,
               int32 V_2,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter V_3,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter V_4)
      .try
      {
        IL_0000:  ldc.i4.1
        IL_0001:  stloc.0
        IL_0002:  ldarg.0
        IL_0003:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>1__state'
        IL_0008:  stloc.2
        IL_0009:  ldloc.2
        IL_000a:  ldc.i4.0
        IL_000b:  beq.s      IL_000f

        IL_000d:  br.s       IL_0011

        IL_000f:  br.s       IL_0062

        IL_0011:  br.s       IL_0013

        IL_0013:  nop
        IL_0014:  ldstr      "Before"
        IL_0019:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_001e:  nop
        IL_001f:  ldc.r8     1.
        IL_0028:  call       valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::FromSeconds(float64)
        IL_002d:  call       class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Threading.Tasks.Task::Delay(valuetype [mscorlib]System.TimeSpan)
        IL_0032:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter [mscorlib]System.Threading.Tasks.Task::GetAwaiter()
        IL_0037:  stloc.3
        IL_0038:  ldloca.s   V_3
        IL_003a:  call       instance bool [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::get_IsCompleted()
        IL_003f:  brtrue.s   IL_0080

        IL_0041:  ldarg.0
        IL_0042:  ldc.i4.0
        IL_0043:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>1__state'
        IL_0048:  ldarg.0
        IL_0049:  ldloc.3
        IL_004a:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>u__$awaiter1'
        IL_004f:  ldarg.0
        IL_0050:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>t__builder'
        IL_0055:  ldloca.s   V_3
        IL_0057:  ldarg.0
        IL_0058:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'>(!!0&,
                                                                                                                                                                                                                                                                                            !!1&)
        IL_005d:  nop
        IL_005e:  ldc.i4.0
        IL_005f:  stloc.0
        IL_0060:  leave.s    IL_00ca

        IL_0062:  ldarg.0
        IL_0063:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>u__$awaiter1'
        IL_0068:  stloc.3
        IL_0069:  ldarg.0
        IL_006a:  ldloca.s   V_4
        IL_006c:  initobj    [mscorlib]System.Runtime.CompilerServices.TaskAwaiter
        IL_0072:  ldloc.s    V_4
        IL_0074:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>u__$awaiter1'
        IL_0079:  ldarg.0
        IL_007a:  ldc.i4.m1
        IL_007b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>1__state'
        IL_0080:  ldloca.s   V_3
        IL_0082:  call       instance void [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::GetResult()
        IL_0087:  nop
        IL_0088:  ldloca.s   V_3
        IL_008a:  initobj    [mscorlib]System.Runtime.CompilerServices.TaskAwaiter
        IL_0090:  ldstr      "After"
        IL_0095:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_009a:  nop
        IL_009b:  leave.s    IL_00b5

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_009d:  stloc.1
        IL_009e:  ldarg.0
        IL_009f:  ldc.i4.s   -2
        IL_00a1:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>1__state'
        IL_00a6:  ldarg.0
        IL_00a7:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>t__builder'
        IL_00ac:  ldloc.1
        IL_00ad:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_00b2:  nop
        IL_00b3:  leave.s    IL_00ca

      }  // end handler
      IL_00b5:  nop
      IL_00b6:  ldarg.0
      IL_00b7:  ldc.i4.s   -2
      IL_00b9:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>1__state'
      IL_00be:  ldarg.0
      IL_00bf:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>t__builder'
      IL_00c4:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_00c9:  nop
      IL_00ca:  nop
      IL_00cb:  ret
    } // end of method '<SimpleVoidMethod>d__0'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine param0) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<SimpleVoidMethod>d__0'::SetStateMachine

  } // end of class '<SimpleVoidMethod>d__0'

  .class auto ansi sealed nested private beforefieldinit '<VoidMethodWithoutAwait>d__3'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       67 (0x43)
      .maxstack  2
      .locals init (bool V_0,
               class [mscorlib]System.Exception V_1)
      .try
      {
        IL_0000:  ldc.i4.1
        IL_0001:  stloc.0
        IL_0002:  br.s       IL_0004

        IL_0004:  br.s       IL_0006

        IL_0006:  nop
        IL_0007:  ldstr      "No Await"
        IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0011:  nop
        IL_0012:  leave.s    IL_002c

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0014:  stloc.1
        IL_0015:  ldarg.0
        IL_0016:  ldc.i4.s   -2
        IL_0018:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__3'::'<>1__state'
        IL_001d:  ldarg.0
        IL_001e:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__3'::'<>t__builder'
        IL_0023:  ldloc.1
        IL_0024:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_0029:  nop
        IL_002a:  leave.s    IL_0041

      }  // end handler
      IL_002c:  nop
      IL_002d:  ldarg.0
      IL_002e:  ldc.i4.s   -2
      IL_0030:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__3'::'<>1__state'
      IL_0035:  ldarg.0
      IL_0036:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__3'::'<>t__builder'
      IL_003b:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_0040:  nop
      IL_0041:  nop
      IL_0042:  ret
    } // end of method '<VoidMethodWithoutAwait>d__3'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine param0) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__3'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<VoidMethodWithoutAwait>d__3'::SetStateMachine

  } // end of class '<VoidMethodWithoutAwait>d__3'

  .class auto ansi sealed nested private beforefieldinit '<EmptyVoidMethod>d__5'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       56 (0x38)
      .maxstack  2
      .locals init (bool V_0,
               class [mscorlib]System.Exception V_1)
      .try
      {
        IL_0000:  ldc.i4.1
        IL_0001:  stloc.0
        IL_0002:  br.s       IL_0004

        IL_0004:  br.s       IL_0006

        IL_0006:  nop
        IL_0007:  leave.s    IL_0021

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0009:  stloc.1
        IL_000a:  ldarg.0
        IL_000b:  ldc.i4.s   -2
        IL_000d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__5'::'<>1__state'
        IL_0012:  ldarg.0
        IL_0013:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__5'::'<>t__builder'
        IL_0018:  ldloc.1
        IL_0019:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_001e:  nop
        IL_001f:  leave.s    IL_0036

      }  // end handler
      IL_0021:  nop
      IL_0022:  ldarg.0
      IL_0023:  ldc.i4.s   -2
      IL_0025:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__5'::'<>1__state'
      IL_002a:  ldarg.0
      IL_002b:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__5'::'<>t__builder'
      IL_0030:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_0035:  nop
      IL_0036:  nop
      IL_0037:  ret
    } // end of method '<EmptyVoidMethod>d__5'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine param0) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__5'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<EmptyVoidMethod>d__5'::SetStateMachine

  } // end of class '<EmptyVoidMethod>d__5'

  .class auto ansi sealed nested private beforefieldinit '<AwaitYield>d__7'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter '<>u__$awaiter8'
    .field private object '<>t__stack'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       174 (0xae)
      .maxstack  3
      .locals init (bool V_0,
               class [mscorlib]System.Exception V_1,
               int32 V_2,
               valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable V_3,
               valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter V_4,
               valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter V_5)
      .try
      {
        IL_0000:  ldc.i4.1
        IL_0001:  stloc.0
        IL_0002:  ldarg.0
        IL_0003:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__7'::'<>1__state'
        IL_0008:  stloc.2
        IL_0009:  ldloc.2
        IL_000a:  ldc.i4.0
        IL_000b:  beq.s      IL_000f

        IL_000d:  br.s       IL_0011

        IL_000f:  br.s       IL_004e

        IL_0011:  br.s       IL_0013

        IL_0013:  nop
        IL_0014:  call       valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable [mscorlib]System.Threading.Tasks.Task::Yield()
        IL_0019:  stloc.3
        IL_001a:  ldloca.s   V_3
        IL_001c:  call       instance valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter [mscorlib]System.Runtime.CompilerServices.YieldAwaitable::GetAwaiter()
        IL_0021:  stloc.s    V_4
        IL_0023:  ldloca.s   V_4
        IL_0025:  call       instance bool [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter::get_IsCompleted()
        IL_002a:  brtrue.s   IL_006d

        IL_002c:  ldarg.0
        IL_002d:  ldc.i4.0
        IL_002e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__7'::'<>1__state'
        IL_0033:  ldarg.0
        IL_0034:  ldloc.s    V_4
        IL_0036:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__7'::'<>u__$awaiter8'
        IL_003b:  ldarg.0
        IL_003c:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__7'::'<>t__builder'
        IL_0041:  ldloca.s   V_4
        IL_0043:  ldarg.0
        IL_0044:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__7'>(!!0&,
                                                                                                                                                                                                                                                                                                      !!1&)
        IL_0049:  nop
        IL_004a:  ldc.i4.0
        IL_004b:  stloc.0
        IL_004c:  leave.s    IL_00ac

        IL_004e:  ldarg.0
        IL_004f:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__7'::'<>u__$awaiter8'
        IL_0054:  stloc.s    V_4
        IL_0056:  ldarg.0
        IL_0057:  ldloca.s   V_5
        IL_0059:  initobj    [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter
        IL_005f:  ldloc.s    V_5
        IL_0061:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__7'::'<>u__$awaiter8'
        IL_0066:  ldarg.0
        IL_0067:  ldc.i4.m1
        IL_0068:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__7'::'<>1__state'
        IL_006d:  ldloca.s   V_4
        IL_006f:  call       instance void [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter::GetResult()
        IL_0074:  nop
        IL_0075:  ldloca.s   V_4
        IL_0077:  initobj    [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter
        IL_007d:  leave.s    IL_0097

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_007f:  stloc.1
        IL_0080:  ldarg.0
        IL_0081:  ldc.i4.s   -2
        IL_0083:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__7'::'<>1__state'
        IL_0088:  ldarg.0
        IL_0089:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__7'::'<>t__builder'
        IL_008e:  ldloc.1
        IL_008f:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_0094:  nop
        IL_0095:  leave.s    IL_00ac

      }  // end handler
      IL_0097:  nop
      IL_0098:  ldarg.0
      IL_0099:  ldc.i4.s   -2
      IL_009b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__7'::'<>1__state'
      IL_00a0:  ldarg.0
      IL_00a1:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__7'::'<>t__builder'
      IL_00a6:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_00ab:  nop
      IL_00ac:  nop
      IL_00ad:  ret
    } // end of method '<AwaitYield>d__7'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine param0) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__7'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<AwaitYield>d__7'::SetStateMachine

  } // end of class '<AwaitYield>d__7'

  .class auto ansi sealed nested private beforefieldinit '<AwaitDefaultYieldAwaitable>d__a'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter '<>u__$awaiterb'
    .field private object '<>t__stack'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       178 (0xb2)
      .maxstack  3
      .locals init (bool V_0,
               class [mscorlib]System.Exception V_1,
               int32 V_2,
               valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable V_3,
               valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter V_4,
               valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter V_5)
      .try
      {
        IL_0000:  ldc.i4.1
        IL_0001:  stloc.0
        IL_0002:  ldarg.0
        IL_0003:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__a'::'<>1__state'
        IL_0008:  stloc.2
        IL_0009:  ldloc.2
        IL_000a:  ldc.i4.0
        IL_000b:  beq.s      IL_000f

        IL_000d:  br.s       IL_0011

        IL_000f:  br.s       IL_0052

        IL_0011:  br.s       IL_0013

        IL_0013:  nop
        IL_0014:  ldloca.s   V_3
        IL_0016:  initobj    [mscorlib]System.Runtime.CompilerServices.YieldAwaitable
        IL_001c:  ldloc.3
        IL_001d:  stloc.3
        IL_001e:  ldloca.s   V_3
        IL_0020:  call       instance valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter [mscorlib]System.Runtime.CompilerServices.YieldAwaitable::GetAwaiter()
        IL_0025:  stloc.s    V_4
        IL_0027:  ldloca.s   V_4
        IL_0029:  call       instance bool [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter::get_IsCompleted()
        IL_002e:  brtrue.s   IL_0071

        IL_0030:  ldarg.0
        IL_0031:  ldc.i4.0
        IL_0032:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__a'::'<>1__state'
        IL_0037:  ldarg.0
        IL_0038:  ldloc.s    V_4
        IL_003a:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__a'::'<>u__$awaiterb'
        IL_003f:  ldarg.0
        IL_0040:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__a'::'<>t__builder'
        IL_0045:  ldloca.s   V_4
        IL_0047:  ldarg.0
        IL_0048:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__a'>(!!0&,
                                                                                                                                                                                                                                                                                                                      !!1&)
        IL_004d:  nop
        IL_004e:  ldc.i4.0
        IL_004f:  stloc.0
        IL_0050:  leave.s    IL_00b0

        IL_0052:  ldarg.0
        IL_0053:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__a'::'<>u__$awaiterb'
        IL_0058:  stloc.s    V_4
        IL_005a:  ldarg.0
        IL_005b:  ldloca.s   V_5
        IL_005d:  initobj    [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter
        IL_0063:  ldloc.s    V_5
        IL_0065:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__a'::'<>u__$awaiterb'
        IL_006a:  ldarg.0
        IL_006b:  ldc.i4.m1
        IL_006c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__a'::'<>1__state'
        IL_0071:  ldloca.s   V_4
        IL_0073:  call       instance void [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter::GetResult()
        IL_0078:  nop
        IL_0079:  ldloca.s   V_4
        IL_007b:  initobj    [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter
        IL_0081:  leave.s    IL_009b

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0083:  stloc.1
        IL_0084:  ldarg.0
        IL_0085:  ldc.i4.s   -2
        IL_0087:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__a'::'<>1__state'
        IL_008c:  ldarg.0
        IL_008d:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__a'::'<>t__builder'
        IL_0092:  ldloc.1
        IL_0093:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_0098:  nop
        IL_0099:  leave.s    IL_00b0

      }  // end handler
      IL_009b:  nop
      IL_009c:  ldarg.0
      IL_009d:  ldc.i4.s   -2
      IL_009f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__a'::'<>1__state'
      IL_00a4:  ldarg.0
      IL_00a5:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__a'::'<>t__builder'
      IL_00aa:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_00af:  nop
      IL_00b0:  nop
      IL_00b1:  ret
    } // end of method '<AwaitDefaultYieldAwaitable>d__a'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine param0) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__a'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<AwaitDefaultYieldAwaitable>d__a'::SetStateMachine

  } // end of class '<AwaitDefaultYieldAwaitable>d__a'

  .class auto ansi sealed nested private beforefieldinit '<SimpleVoidTaskMethod>d__d'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter '<>u__$awaitere'
    .field private object '<>t__stack'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       204 (0xcc)
      .maxstack  3
      .locals init (bool V_0,
               class [mscorlib]System.Exception V_1,
               int32 V_2,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter V_3,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter V_4)
      .try
      {
        IL_0000:  ldc.i4.1
        IL_0001:  stloc.0
        IL_0002:  ldarg.0
        IL_0003:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__d'::'<>1__state'
        IL_0008:  stloc.2
        IL_0009:  ldloc.2
        IL_000a:  ldc.i4.0
        IL_000b:  beq.s      IL_000f

        IL_000d:  br.s       IL_0011

        IL_000f:  br.s       IL_0062

        IL_0011:  br.s       IL_0013

        IL_0013:  nop
        IL_0014:  ldstr      "Before"
        IL_0019:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_001e:  nop
        IL_001f:  ldc.r8     1.
        IL_0028:  call       valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::FromSeconds(float64)
        IL_002d:  call       class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Threading.Tasks.Task::Delay(valuetype [mscorlib]System.TimeSpan)
        IL_0032:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter [mscorlib]System.Threading.Tasks.Task::GetAwaiter()
        IL_0037:  stloc.3
        IL_0038:  ldloca.s   V_3
        IL_003a:  call       instance bool [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::get_IsCompleted()
        IL_003f:  brtrue.s   IL_0080

        IL_0041:  ldarg.0
        IL_0042:  ldc.i4.0
        IL_0043:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__d'::'<>1__state'
        IL_0048:  ldarg.0
        IL_0049:  ldloc.3
        IL_004a:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__d'::'<>u__$awaitere'
        IL_004f:  ldarg.0
        IL_0050:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__d'::'<>t__builder'
        IL_0055:  ldloca.s   V_3
        IL_0057:  ldarg.0
        IL_0058:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__d'>(!!0&,
                                                                                                                                                                                                                                                                                                !!1&)
        IL_005d:  nop
        IL_005e:  ldc.i4.0
        IL_005f:  stloc.0
        IL_0060:  leave.s    IL_00ca

        IL_0062:  ldarg.0
        IL_0063:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__d'::'<>u__$awaitere'
        IL_0068:  stloc.3
        IL_0069:  ldarg.0
        IL_006a:  ldloca.s   V_4
        IL_006c:  initobj    [mscorlib]System.Runtime.CompilerServices.TaskAwaiter
        IL_0072:  ldloc.s    V_4
        IL_0074:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__d'::'<>u__$awaitere'
        IL_0079:  ldarg.0
        IL_007a:  ldc.i4.m1
        IL_007b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__d'::'<>1__state'
        IL_0080:  ldloca.s   V_3
        IL_0082:  call       instance void [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::GetResult()
        IL_0087:  nop
        IL_0088:  ldloca.s   V_3
        IL_008a:  initobj    [mscorlib]System.Runtime.CompilerServices.TaskAwaiter
        IL_0090:  ldstr      "After"
        IL_0095:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_009a:  nop
        IL_009b:  leave.s    IL_00b5

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_009d:  stloc.1
        IL_009e:  ldarg.0
        IL_009f:  ldc.i4.s   -2
        IL_00a1:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__d'::'<>1__state'
        IL_00a6:  ldarg.0
        IL_00a7:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__d'::'<>t__builder'
        IL_00ac:  ldloc.1
        IL_00ad:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_00b2:  nop
        IL_00b3:  leave.s    IL_00ca

      }  // end handler
      IL_00b5:  nop
      IL_00b6:  ldarg.0
      IL_00b7:  ldc.i4.s   -2
      IL_00b9:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__d'::'<>1__state'
      IL_00be:  ldarg.0
      IL_00bf:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__d'::'<>t__builder'
      IL_00c4:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetResult()
      IL_00c9:  nop
      IL_00ca:  nop
      IL_00cb:  ret
    } // end of method '<SimpleVoidTaskMethod>d__d'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine param0) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__d'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<SimpleVoidTaskMethod>d__d'::SetStateMachine

  } // end of class '<SimpleVoidTaskMethod>d__d'

  .class auto ansi sealed nested private beforefieldinit '<TaskMethodWithoutAwait>d__10'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       67 (0x43)
      .maxstack  2
      .locals init (bool V_0,
               class [mscorlib]System.Exception V_1)
      .try
      {
        IL_0000:  ldc.i4.1
        IL_0001:  stloc.0
        IL_0002:  br.s       IL_0004

        IL_0004:  br.s       IL_0006

        IL_0006:  nop
        IL_0007:  ldstr      "No Await"
        IL_000c:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0011:  nop
        IL_0012:  leave.s    IL_002c

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0014:  stloc.1
        IL_0015:  ldarg.0
        IL_0016:  ldc.i4.s   -2
        IL_0018:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__10'::'<>1__state'
        IL_001d:  ldarg.0
        IL_001e:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__10'::'<>t__builder'
        IL_0023:  ldloc.1
        IL_0024:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_0029:  nop
        IL_002a:  leave.s    IL_0041

      }  // end handler
      IL_002c:  nop
      IL_002d:  ldarg.0
      IL_002e:  ldc.i4.s   -2
      IL_0030:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__10'::'<>1__state'
      IL_0035:  ldarg.0
      IL_0036:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__10'::'<>t__builder'
      IL_003b:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetResult()
      IL_0040:  nop
      IL_0041:  nop
      IL_0042:  ret
    } // end of method '<TaskMethodWithoutAwait>d__10'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine param0) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__10'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<TaskMethodWithoutAwait>d__10'::SetStateMachine

  } // end of class '<TaskMethodWithoutAwait>d__10'

  .class auto ansi sealed nested private beforefieldinit '<SimpleBoolTaskMethod>d__12'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter '<>u__$awaiter13'
    .field private object '<>t__stack'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       210 (0xd2)
      .maxstack  3
      .locals init (bool V_0,
               bool V_1,
               class [mscorlib]System.Exception V_2,
               int32 V_3,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter V_4,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter V_5)
      .try
      {
        IL_0000:  ldc.i4.1
        IL_0001:  stloc.0
        IL_0002:  ldarg.0
        IL_0003:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__12'::'<>1__state'
        IL_0008:  stloc.3
        IL_0009:  ldloc.3
        IL_000a:  ldc.i4.0
        IL_000b:  beq.s      IL_000f

        IL_000d:  br.s       IL_0011

        IL_000f:  br.s       IL_0064

        IL_0011:  br.s       IL_0013

        IL_0013:  nop
        IL_0014:  ldstr      "Before"
        IL_0019:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_001e:  nop
        IL_001f:  ldc.r8     1.
        IL_0028:  call       valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::FromSeconds(float64)
        IL_002d:  call       class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Threading.Tasks.Task::Delay(valuetype [mscorlib]System.TimeSpan)
        IL_0032:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter [mscorlib]System.Threading.Tasks.Task::GetAwaiter()
        IL_0037:  stloc.s    V_4
        IL_0039:  ldloca.s   V_4
        IL_003b:  call       instance bool [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::get_IsCompleted()
        IL_0040:  brtrue.s   IL_0083

        IL_0042:  ldarg.0
        IL_0043:  ldc.i4.0
        IL_0044:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__12'::'<>1__state'
        IL_0049:  ldarg.0
        IL_004a:  ldloc.s    V_4
        IL_004c:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__12'::'<>u__$awaiter13'
        IL_0051:  ldarg.0
        IL_0052:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__12'::'<>t__builder'
        IL_0057:  ldloca.s   V_4
        IL_0059:  ldarg.0
        IL_005a:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__12'>(!!0&,
                                                                                                                                                                                                                                                                                                                   !!1&)
        IL_005f:  nop
        IL_0060:  ldc.i4.0
        IL_0061:  stloc.0
        IL_0062:  leave.s    IL_00d0

        IL_0064:  ldarg.0
        IL_0065:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__12'::'<>u__$awaiter13'
        IL_006a:  stloc.s    V_4
        IL_006c:  ldarg.0
        IL_006d:  ldloca.s   V_5
        IL_006f:  initobj    [mscorlib]System.Runtime.CompilerServices.TaskAwaiter
        IL_0075:  ldloc.s    V_5
        IL_0077:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__12'::'<>u__$awaiter13'
        IL_007c:  ldarg.0
        IL_007d:  ldc.i4.m1
        IL_007e:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__12'::'<>1__state'
        IL_0083:  ldloca.s   V_4
        IL_0085:  call       instance void [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::GetResult()
        IL_008a:  nop
        IL_008b:  ldloca.s   V_4
        IL_008d:  initobj    [mscorlib]System.Runtime.CompilerServices.TaskAwaiter
        IL_0093:  ldstr      "After"
        IL_0098:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_009d:  nop
        IL_009e:  ldc.i4.1
        IL_009f:  stloc.1
        IL_00a0:  leave.s    IL_00ba

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_00a2:  stloc.2
        IL_00a3:  ldarg.0
        IL_00a4:  ldc.i4.s   -2
        IL_00a6:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__12'::'<>1__state'
        IL_00ab:  ldarg.0
        IL_00ac:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__12'::'<>t__builder'
        IL_00b1:  ldloc.2
        IL_00b2:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::SetException(class [mscorlib]System.Exception)
        IL_00b7:  nop
        IL_00b8:  leave.s    IL_00d0

      }  // end handler
      IL_00ba:  nop
      IL_00bb:  ldarg.0
      IL_00bc:  ldc.i4.s   -2
      IL_00be:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__12'::'<>1__state'
      IL_00c3:  ldarg.0
      IL_00c4:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__12'::'<>t__builder'
      IL_00c9:  ldloc.1
      IL_00ca:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::SetResult(!0)
      IL_00cf:  nop
      IL_00d0:  nop
      IL_00d1:  ret
    } // end of method '<SimpleBoolTaskMethod>d__12'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine param0) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__12'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<SimpleBoolTaskMethod>d__12'::SetStateMachine

  } // end of class '<SimpleBoolTaskMethod>d__12'

  .class auto ansi sealed nested private beforefieldinit '<TwoAwaitsWithDifferentAwaiterTypes>d__15'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> '<>u__$awaiter16'
    .field private object '<>t__stack'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter '<>u__$awaiter17'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       340 (0x154)
      .maxstack  3
      .locals init (bool V_0,
               class [mscorlib]System.Exception V_1,
               int32 V_2,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> V_3,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> V_4,
               bool V_5,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter V_6,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter V_7)
      .try
      {
        IL_0000:  ldc.i4.1
        IL_0001:  stloc.0
        IL_0002:  ldarg.0
        IL_0003:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>1__state'
        IL_0008:  stloc.2
        IL_0009:  ldloc.2
        IL_000a:  switch     ( 
                              IL_0019,
                              IL_001b)
        IL_0017:  br.s       IL_0020

        IL_0019:  br.s       IL_006c

        IL_001b:  br         IL_00e8

        IL_0020:  br.s       IL_0022

        IL_0022:  nop
        IL_0023:  ldstr      "Before"
        IL_0028:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_002d:  nop
        IL_002e:  ldarg.0
        IL_002f:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>4__this'
        IL_0034:  callvirt   instance class [mscorlib]System.Threading.Tasks.Task`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async::SimpleBoolTaskMethod()
        IL_0039:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [mscorlib]System.Threading.Tasks.Task`1<bool>::GetAwaiter()
        IL_003e:  stloc.3
        IL_003f:  ldloca.s   V_3
        IL_0041:  call       instance bool valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>::get_IsCompleted()
        IL_0046:  brtrue.s   IL_008a

        IL_0048:  ldarg.0
        IL_0049:  ldc.i4.0
        IL_004a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>1__state'
        IL_004f:  ldarg.0
        IL_0050:  ldloc.3
        IL_0051:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>u__$awaiter16'
        IL_0056:  ldarg.0
        IL_0057:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>t__builder'
        IL_005c:  ldloca.s   V_3
        IL_005e:  ldarg.0
        IL_005f:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'>(!!0&,
                                                                                                                                                                                                                                                                                                                       !!1&)
        IL_0064:  nop
        IL_0065:  ldc.i4.0
        IL_0066:  stloc.0
        IL_0067:  leave      IL_0152

        IL_006c:  ldarg.0
        IL_006d:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>u__$awaiter16'
        IL_0072:  stloc.3
        IL_0073:  ldarg.0
        IL_0074:  ldloca.s   V_4
        IL_0076:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>
        IL_007c:  ldloc.s    V_4
        IL_007e:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>u__$awaiter16'
        IL_0083:  ldarg.0
        IL_0084:  ldc.i4.m1
        IL_0085:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>1__state'
        IL_008a:  ldloca.s   V_3
        IL_008c:  call       instance !0 valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>::GetResult()
        IL_0091:  ldloca.s   V_3
        IL_0093:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>
        IL_0099:  ldc.i4.0
        IL_009a:  ceq
        IL_009c:  stloc.s    V_5
        IL_009e:  ldloc.s    V_5
        IL_00a0:  brtrue.s   IL_0118

        IL_00a2:  nop
        IL_00a3:  ldc.r8     1.
        IL_00ac:  call       valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::FromSeconds(float64)
        IL_00b1:  call       class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Threading.Tasks.Task::Delay(valuetype [mscorlib]System.TimeSpan)
        IL_00b6:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter [mscorlib]System.Threading.Tasks.Task::GetAwaiter()
        IL_00bb:  stloc.s    V_6
        IL_00bd:  ldloca.s   V_6
        IL_00bf:  call       instance bool [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::get_IsCompleted()
        IL_00c4:  brtrue.s   IL_0107

        IL_00c6:  ldarg.0
        IL_00c7:  ldc.i4.1
        IL_00c8:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>1__state'
        IL_00cd:  ldarg.0
        IL_00ce:  ldloc.s    V_6
        IL_00d0:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>u__$awaiter17'
        IL_00d5:  ldarg.0
        IL_00d6:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>t__builder'
        IL_00db:  ldloca.s   V_6
        IL_00dd:  ldarg.0
        IL_00de:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'>(!!0&,
                                                                                                                                                                                                                                                                                                               !!1&)
        IL_00e3:  nop
        IL_00e4:  ldc.i4.0
        IL_00e5:  stloc.0
        IL_00e6:  leave.s    IL_0152

        IL_00e8:  ldarg.0
        IL_00e9:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>u__$awaiter17'
        IL_00ee:  stloc.s    V_6
        IL_00f0:  ldarg.0
        IL_00f1:  ldloca.s   V_7
        IL_00f3:  initobj    [mscorlib]System.Runtime.CompilerServices.TaskAwaiter
        IL_00f9:  ldloc.s    V_7
        IL_00fb:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>u__$awaiter17'
        IL_0100:  ldarg.0
        IL_0101:  ldc.i4.m1
        IL_0102:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>1__state'
        IL_0107:  ldloca.s   V_6
        IL_0109:  call       instance void [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::GetResult()
        IL_010e:  nop
        IL_010f:  ldloca.s   V_6
        IL_0111:  initobj    [mscorlib]System.Runtime.CompilerServices.TaskAwaiter
        IL_0117:  nop
        IL_0118:  ldstr      "After"
        IL_011d:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0122:  nop
        IL_0123:  leave.s    IL_013d

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0125:  stloc.1
        IL_0126:  ldarg.0
        IL_0127:  ldc.i4.s   -2
        IL_0129:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>1__state'
        IL_012e:  ldarg.0
        IL_012f:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>t__builder'
        IL_0134:  ldloc.1
        IL_0135:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_013a:  nop
        IL_013b:  leave.s    IL_0152

      }  // end handler
      IL_013d:  nop
      IL_013e:  ldarg.0
      IL_013f:  ldc.i4.s   -2
      IL_0141:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>1__state'
      IL_0146:  ldarg.0
      IL_0147:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>t__builder'
      IL_014c:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_0151:  nop
      IL_0152:  nop
      IL_0153:  ret
    } // end of method '<TwoAwaitsWithDifferentAwaiterTypes>d__15'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine param0) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<TwoAwaitsWithDifferentAwaiterTypes>d__15'::SetStateMachine

  } // end of class '<TwoAwaitsWithDifferentAwaiterTypes>d__15'

  .class auto ansi sealed nested private beforefieldinit '<AwaitInLoopCondition>d__19'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> '<>u__$awaiter1a'
    .field private object '<>t__stack'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       194 (0xc2)
      .maxstack  3
      .locals init (bool V_0,
               class [mscorlib]System.Exception V_1,
               int32 V_2,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> V_3,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> V_4,
               bool V_5)
      .try
      {
        IL_0000:  ldc.i4.1
        IL_0001:  stloc.0
        IL_0002:  ldarg.0
        IL_0003:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__19'::'<>1__state'
        IL_0008:  stloc.2
        IL_0009:  ldloc.2
        IL_000a:  ldc.i4.0
        IL_000b:  beq.s      IL_000f

        IL_000d:  br.s       IL_0011

        IL_000f:  br.s       IL_005e

        IL_0011:  br.s       IL_0013

        IL_0013:  nop
        IL_0014:  br.s       IL_0023

        IL_0016:  nop
        IL_0017:  ldstr      "Body"
        IL_001c:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0021:  nop
        IL_0022:  nop
        IL_0023:  ldarg.0
        IL_0024:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__19'::'<>4__this'
        IL_0029:  callvirt   instance class [mscorlib]System.Threading.Tasks.Task`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async::SimpleBoolTaskMethod()
        IL_002e:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [mscorlib]System.Threading.Tasks.Task`1<bool>::GetAwaiter()
        IL_0033:  stloc.3
        IL_0034:  ldloca.s   V_3
        IL_0036:  call       instance bool valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>::get_IsCompleted()
        IL_003b:  brtrue.s   IL_007c

        IL_003d:  ldarg.0
        IL_003e:  ldc.i4.0
        IL_003f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__19'::'<>1__state'
        IL_0044:  ldarg.0
        IL_0045:  ldloc.3
        IL_0046:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__19'::'<>u__$awaiter1a'
        IL_004b:  ldarg.0
        IL_004c:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__19'::'<>t__builder'
        IL_0051:  ldloca.s   V_3
        IL_0053:  ldarg.0
        IL_0054:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__19'>(!!0&,
                                                                                                                                                                                                                                                                                                         !!1&)
        IL_0059:  nop
        IL_005a:  ldc.i4.0
        IL_005b:  stloc.0
        IL_005c:  leave.s    IL_00c0

        IL_005e:  ldarg.0
        IL_005f:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__19'::'<>u__$awaiter1a'
        IL_0064:  stloc.3
        IL_0065:  ldarg.0
        IL_0066:  ldloca.s   V_4
        IL_0068:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>
        IL_006e:  ldloc.s    V_4
        IL_0070:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__19'::'<>u__$awaiter1a'
        IL_0075:  ldarg.0
        IL_0076:  ldc.i4.m1
        IL_0077:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__19'::'<>1__state'
        IL_007c:  ldloca.s   V_3
        IL_007e:  call       instance !0 valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>::GetResult()
        IL_0083:  ldloca.s   V_3
        IL_0085:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>
        IL_008b:  stloc.s    V_5
        IL_008d:  ldloc.s    V_5
        IL_008f:  brtrue.s   IL_0016

        IL_0091:  leave.s    IL_00ab

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0093:  stloc.1
        IL_0094:  ldarg.0
        IL_0095:  ldc.i4.s   -2
        IL_0097:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__19'::'<>1__state'
        IL_009c:  ldarg.0
        IL_009d:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__19'::'<>t__builder'
        IL_00a2:  ldloc.1
        IL_00a3:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_00a8:  nop
        IL_00a9:  leave.s    IL_00c0

      }  // end handler
      IL_00ab:  nop
      IL_00ac:  ldarg.0
      IL_00ad:  ldc.i4.s   -2
      IL_00af:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__19'::'<>1__state'
      IL_00b4:  ldarg.0
      IL_00b5:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__19'::'<>t__builder'
      IL_00ba:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_00bf:  nop
      IL_00c0:  nop
      IL_00c1:  ret
    } // end of method '<AwaitInLoopCondition>d__19'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine param0) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__19'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<AwaitInLoopCondition>d__19'::SetStateMachine

  } // end of class '<AwaitInLoopCondition>d__19'

  .method public hidebysig instance void 
          SimpleVoidMethod() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 4A 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..JICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 53 69 6D 70 6C 65 56 6F   // .Async+<SimpleVo
                                                                                                                                       69 64 4D 65 74 68 6F 64 3E 64 5F 5F 30 00 00 )    // idMethod>d__0..
    // Code size       48 (0x30)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldarg.0
    IL_0003:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>4__this'
    IL_0008:  ldloca.s   V_0
    IL_000a:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_000f:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>t__builder'
    IL_0014:  ldloca.s   V_0
    IL_0016:  ldc.i4.m1
    IL_0017:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>1__state'
    IL_001c:  ldloca.s   V_0
    IL_001e:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>t__builder'
    IL_0023:  stloc.1
    IL_0024:  ldloca.s   V_1
    IL_0026:  ldloca.s   V_0
    IL_0028:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'>(!!0&)
    IL_002d:  br.s       IL_002f

    IL_002f:  ret
  } // end of method Async::SimpleVoidMethod

  .method public hidebysig instance void 
          VoidMethodWithoutAwait() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 50 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..PICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 56 6F 69 64 4D 65 74 68   // .Async+<VoidMeth
                                                                                                                                       6F 64 57 69 74 68 6F 75 74 41 77 61 69 74 3E 64   // odWithoutAwait>d
                                                                                                                                       5F 5F 33 00 00 )                                  // __3..
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       48 (0x30)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__3' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldarg.0
    IL_0003:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__3'::'<>4__this'
    IL_0008:  ldloca.s   V_0
    IL_000a:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_000f:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__3'::'<>t__builder'
    IL_0014:  ldloca.s   V_0
    IL_0016:  ldc.i4.m1
    IL_0017:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__3'::'<>1__state'
    IL_001c:  ldloca.s   V_0
    IL_001e:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__3'::'<>t__builder'
    IL_0023:  stloc.1
    IL_0024:  ldloca.s   V_1
    IL_0026:  ldloca.s   V_0
    IL_0028:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__3'>(!!0&)
    IL_002d:  br.s       IL_002f

    IL_002f:  ret
  } // end of method Async::VoidMethodWithoutAwait

  .method public hidebysig instance void 
          EmptyVoidMethod() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 49 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..IICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 45 6D 70 74 79 56 6F 69   // .Async+<EmptyVoi
                                                                                                                                       64 4D 65 74 68 6F 64 3E 64 5F 5F 35 00 00 )       // dMethod>d__5..
    // Code size       48 (0x30)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__5' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldarg.0
    IL_0003:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__5'::'<>4__this'
    IL_0008:  ldloca.s   V_0
    IL_000a:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_000f:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__5'::'<>t__builder'
    IL_0014:  ldloca.s   V_0
    IL_0016:  ldc.i4.m1
    IL_0017:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__5'::'<>1__state'
    IL_001c:  ldloca.s   V_0
    IL_001e:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__5'::'<>t__builder'
    IL_0023:  stloc.1
    IL_0024:  ldloca.s   V_1
    IL_0026:  ldloca.s   V_0
    IL_0028:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__5'>(!!0&)
    IL_002d:  br.s       IL_002f

    IL_002f:  ret
  } // end of method Async::EmptyVoidMethod

  .method public hidebysig instance void 
          AwaitYield() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 44 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..DICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 41 77 61 69 74 59 69 65   // .Async+<AwaitYie
                                                                                                                                       6C 64 3E 64 5F 5F 37 00 00 )                      // ld>d__7..
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       48 (0x30)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__7' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldarg.0
    IL_0003:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__7'::'<>4__this'
    IL_0008:  ldloca.s   V_0
    IL_000a:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_000f:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__7'::'<>t__builder'
    IL_0014:  ldloca.s   V_0
    IL_0016:  ldc.i4.m1
    IL_0017:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__7'::'<>1__state'
    IL_001c:  ldloca.s   V_0
    IL_001e:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__7'::'<>t__builder'
    IL_0023:  stloc.1
    IL_0024:  ldloca.s   V_1
    IL_0026:  ldloca.s   V_0
    IL_0028:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__7'>(!!0&)
    IL_002d:  br.s       IL_002f

    IL_002f:  ret
  } // end of method Async::AwaitYield

  .method public hidebysig instance void 
          AwaitDefaultYieldAwaitable() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 54 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..TICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 41 77 61 69 74 44 65 66   // .Async+<AwaitDef
                                                                                                                                       61 75 6C 74 59 69 65 6C 64 41 77 61 69 74 61 62   // aultYieldAwaitab
                                                                                                                                       6C 65 3E 64 5F 5F 61 00 00 )                      // le>d__a..
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       48 (0x30)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__a' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldarg.0
    IL_0003:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__a'::'<>4__this'
    IL_0008:  ldloca.s   V_0
    IL_000a:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_000f:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__a'::'<>t__builder'
    IL_0014:  ldloca.s   V_0
    IL_0016:  ldc.i4.m1
    IL_0017:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__a'::'<>1__state'
    IL_001c:  ldloca.s   V_0
    IL_001e:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__a'::'<>t__builder'
    IL_0023:  stloc.1
    IL_0024:  ldloca.s   V_1
    IL_0026:  ldloca.s   V_0
    IL_0028:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__a'>(!!0&)
    IL_002d:  br.s       IL_002f

    IL_002f:  ret
  } // end of method Async::AwaitDefaultYieldAwaitable

  .method public hidebysig instance class [mscorlib]System.Threading.Tasks.Task 
          SimpleVoidTaskMethod() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 4E 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..NICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 53 69 6D 70 6C 65 56 6F   // .Async+<SimpleVo
                                                                                                                                       69 64 54 61 73 6B 4D 65 74 68 6F 64 3E 64 5F 5F   // idTaskMethod>d__
                                                                                                                                       64 00 00 )                                        // d..
    // Code size       62 (0x3e)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__d' V_0,
             class [mscorlib]System.Threading.Tasks.Task V_1,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder V_2)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldarg.0
    IL_0003:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__d'::'<>4__this'
    IL_0008:  ldloca.s   V_0
    IL_000a:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Create()
    IL_000f:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__d'::'<>t__builder'
    IL_0014:  ldloca.s   V_0
    IL_0016:  ldc.i4.m1
    IL_0017:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__d'::'<>1__state'
    IL_001c:  ldloca.s   V_0
    IL_001e:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__d'::'<>t__builder'
    IL_0023:  stloc.2
    IL_0024:  ldloca.s   V_2
    IL_0026:  ldloca.s   V_0
    IL_0028:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__d'>(!!0&)
    IL_002d:  ldloca.s   V_0
    IL_002f:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__d'::'<>t__builder'
    IL_0034:  call       instance class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::get_Task()
    IL_0039:  stloc.1
    IL_003a:  br.s       IL_003c

    IL_003c:  ldloc.1
    IL_003d:  ret
  } // end of method Async::SimpleVoidTaskMethod

  .method public hidebysig instance class [mscorlib]System.Threading.Tasks.Task 
          TaskMethodWithoutAwait() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 51 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..QICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 54 61 73 6B 4D 65 74 68   // .Async+<TaskMeth
                                                                                                                                       6F 64 57 69 74 68 6F 75 74 41 77 61 69 74 3E 64   // odWithoutAwait>d
                                                                                                                                       5F 5F 31 30 00 00 )                               // __10..
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       62 (0x3e)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__10' V_0,
             class [mscorlib]System.Threading.Tasks.Task V_1,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder V_2)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldarg.0
    IL_0003:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__10'::'<>4__this'
    IL_0008:  ldloca.s   V_0
    IL_000a:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Create()
    IL_000f:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__10'::'<>t__builder'
    IL_0014:  ldloca.s   V_0
    IL_0016:  ldc.i4.m1
    IL_0017:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__10'::'<>1__state'
    IL_001c:  ldloca.s   V_0
    IL_001e:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__10'::'<>t__builder'
    IL_0023:  stloc.2
    IL_0024:  ldloca.s   V_2
    IL_0026:  ldloca.s   V_0
    IL_0028:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__10'>(!!0&)
    IL_002d:  ldloca.s   V_0
    IL_002f:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__10'::'<>t__builder'
    IL_0034:  call       instance class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::get_Task()
    IL_0039:  stloc.1
    IL_003a:  br.s       IL_003c

    IL_003c:  ldloc.1
    IL_003d:  ret
  } // end of method Async::TaskMethodWithoutAwait

  .method public hidebysig instance class [mscorlib]System.Threading.Tasks.Task`1<bool> 
          SimpleBoolTaskMethod() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 4F 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..OICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 53 69 6D 70 6C 65 42 6F   // .Async+<SimpleBo
                                                                                                                                       6F 6C 54 61 73 6B 4D 65 74 68 6F 64 3E 64 5F 5F   // olTaskMethod>d__
                                                                                                                                       31 32 00 00 )                                     // 12..
    // Code size       62 (0x3e)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__12' V_0,
             class [mscorlib]System.Threading.Tasks.Task`1<bool> V_1,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> V_2)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldarg.0
    IL_0003:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__12'::'<>4__this'
    IL_0008:  ldloca.s   V_0
    IL_000a:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::Create()
    IL_000f:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__12'::'<>t__builder'
    IL_0014:  ldloca.s   V_0
    IL_0016:  ldc.i4.m1
    IL_0017:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__12'::'<>1__state'
    IL_001c:  ldloca.s   V_0
    IL_001e:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__12'::'<>t__builder'
    IL_0023:  stloc.2
    IL_0024:  ldloca.s   V_2
    IL_0026:  ldloca.s   V_0
    IL_0028:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__12'>(!!0&)
    IL_002d:  ldloca.s   V_0
    IL_002f:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__12'::'<>t__builder'
    IL_0034:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<!0> valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::get_Task()
    IL_0039:  stloc.1
    IL_003a:  br.s       IL_003c

    IL_003c:  ldloc.1
    IL_003d:  ret
  } // end of method Async::SimpleBoolTaskMethod

  .method public hidebysig instance void 
          TwoAwaitsWithDifferentAwaiterTypes() cil managed
  {
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 5D 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..]ICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 54 77 6F 41 77 61 69 74   // .Async+<TwoAwait
                                                                                                                                       73 57 69 74 68 44 69 66 66 65 72 65 6E 74 41 77   // sWithDifferentAw
                                                                                                                                       61 69 74 65 72 54 79 70 65 73 3E 64 5F 5F 31 35   // aiterTypes>d__15
                                                                                                                                       00 00 ) 
    // Code size       48 (0x30)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldarg.0
    IL_0003:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>4__this'
    IL_0008:  ldloca.s   V_0
    IL_000a:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_000f:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>t__builder'
    IL_0014:  ldloca.s   V_0
    IL_0016:  ldc.i4.m1
    IL_0017:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>1__state'
    IL_001c:  ldloca.s   V_0
    IL_001e:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'::'<>t__builder'
    IL_0023:  stloc.1
    IL_0024:  ldloca.s   V_1
    IL_0026:  ldloca.s   V_0
    IL_0028:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__15'>(!!0&)
    IL_002d:  br.s       IL_002f

    IL_002f:  ret
  } // end of method Async::TwoAwaitsWithDifferentAwaiterTypes

  .method public hidebysig instance void 
          AwaitInLoopCondition() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 4F 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..OICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 41 77 61 69 74 49 6E 4C   // .Async+<AwaitInL
                                                                                                                                       6F 6F 70 43 6F 6E 64 69 74 69 6F 6E 3E 64 5F 5F   // oopCondition>d__
                                                                                                                                       31 39 00 00 )                                     // 19..
    .custom instance void [mscorlib]System.Diagnostics.DebuggerStepThroughAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       48 (0x30)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__19' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldarg.0
    IL_0003:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__19'::'<>4__this'
    IL_0008:  ldloca.s   V_0
    IL_000a:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_000f:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__19'::'<>t__builder'
    IL_0014:  ldloca.s   V_0
    IL_0016:  ldc.i4.m1
    IL_0017:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__19'::'<>1__state'
    IL_001c:  ldloca.s   V_0
    IL_001e:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__19'::'<>t__builder'
    IL_0023:  stloc.1
    IL_0024:  ldloca.s   V_1
    IL_0026:  ldloca.s   V_0
    IL_0028:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__19'>(!!0&)
    IL_002d:  br.s       IL_002f

    IL_002f:  ret
  } // end of method Async::AwaitInLoopCondition

  .method public hidebysig specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [mscorlib]System.Object::.ctor()
    IL_0006:  ret
  } // end of method Async::.ctor

} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
// WARNING: Created Win32 resource file ../../../TestCases/Pretty\Async.res
