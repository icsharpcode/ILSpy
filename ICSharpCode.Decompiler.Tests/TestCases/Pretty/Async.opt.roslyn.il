



// Metadata version: v4.0.30319
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 4:0:0:0
}
.assembly Async
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
.module Async.dll
.custom instance void [mscorlib]System.Security.UnverifiableCodeAttribute::.ctor() = ( 01 00 00 00 ) 
.imagebase 0x10000000
.file alignment 0x00000200
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY


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
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter '<>u__1'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       176 (0xb0)
      .maxstack  3
      .locals init (int32 V_0,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter V_1,
               class [mscorlib]System.Exception V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0056

        IL_000a:  ldstr      "Before"
        IL_000f:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0014:  ldc.r8     1.
        IL_001d:  call       valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::FromSeconds(float64)
        IL_0022:  call       class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Threading.Tasks.Task::Delay(valuetype [mscorlib]System.TimeSpan)
        IL_0027:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter [mscorlib]System.Threading.Tasks.Task::GetAwaiter()
        IL_002c:  stloc.1
        IL_002d:  ldloca.s   V_1
        IL_002f:  call       instance bool [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::get_IsCompleted()
        IL_0034:  brtrue.s   IL_0072

        IL_0036:  ldarg.0
        IL_0037:  ldc.i4.0
        IL_0038:  dup
        IL_0039:  stloc.0
        IL_003a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>1__state'
        IL_003f:  ldarg.0
        IL_0040:  ldloc.1
        IL_0041:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>u__1'
        IL_0046:  ldarg.0
        IL_0047:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>t__builder'
        IL_004c:  ldloca.s   V_1
        IL_004e:  ldarg.0
        IL_004f:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'>(!!0&,
                                                                                                                                                                                                                                                                                            !!1&)
        IL_0054:  leave.s    IL_00af

        IL_0056:  ldarg.0
        IL_0057:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>u__1'
        IL_005c:  stloc.1
        IL_005d:  ldarg.0
        IL_005e:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>u__1'
        IL_0063:  initobj    [mscorlib]System.Runtime.CompilerServices.TaskAwaiter
        IL_0069:  ldarg.0
        IL_006a:  ldc.i4.m1
        IL_006b:  dup
        IL_006c:  stloc.0
        IL_006d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>1__state'
        IL_0072:  ldloca.s   V_1
        IL_0074:  call       instance void [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::GetResult()
        IL_0079:  ldstr      "After"
        IL_007e:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0083:  leave.s    IL_009c

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0085:  stloc.2
        IL_0086:  ldarg.0
        IL_0087:  ldc.i4.s   -2
        IL_0089:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>1__state'
        IL_008e:  ldarg.0
        IL_008f:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>t__builder'
        IL_0094:  ldloc.2
        IL_0095:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_009a:  leave.s    IL_00af

      }  // end handler
      IL_009c:  ldarg.0
      IL_009d:  ldc.i4.s   -2
      IL_009f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>1__state'
      IL_00a4:  ldarg.0
      IL_00a5:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>t__builder'
      IL_00aa:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_00af:  ret
    } // end of method '<SimpleVoidMethod>d__0'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
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

  .class auto ansi sealed nested private beforefieldinit '<VoidMethodWithoutAwait>d__1'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       55 (0x37)
      .maxstack  2
      .locals init (class [mscorlib]System.Exception V_0)
      .try
      {
        IL_0000:  ldstr      "No Await"
        IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_000a:  leave.s    IL_0023

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_000c:  stloc.0
        IL_000d:  ldarg.0
        IL_000e:  ldc.i4.s   -2
        IL_0010:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1'::'<>1__state'
        IL_0015:  ldarg.0
        IL_0016:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1'::'<>t__builder'
        IL_001b:  ldloc.0
        IL_001c:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_0021:  leave.s    IL_0036

      }  // end handler
      IL_0023:  ldarg.0
      IL_0024:  ldc.i4.s   -2
      IL_0026:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1'::'<>1__state'
      IL_002b:  ldarg.0
      IL_002c:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1'::'<>t__builder'
      IL_0031:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_0036:  ret
    } // end of method '<VoidMethodWithoutAwait>d__1'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<VoidMethodWithoutAwait>d__1'::SetStateMachine

  } // end of class '<VoidMethodWithoutAwait>d__1'

  .class auto ansi sealed nested private beforefieldinit '<EmptyVoidMethod>d__2'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       45 (0x2d)
      .maxstack  2
      .locals init (class [mscorlib]System.Exception V_0)
      .try
      {
        IL_0000:  leave.s    IL_0019

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0002:  stloc.0
        IL_0003:  ldarg.0
        IL_0004:  ldc.i4.s   -2
        IL_0006:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2'::'<>1__state'
        IL_000b:  ldarg.0
        IL_000c:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2'::'<>t__builder'
        IL_0011:  ldloc.0
        IL_0012:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_0017:  leave.s    IL_002c

      }  // end handler
      IL_0019:  ldarg.0
      IL_001a:  ldc.i4.s   -2
      IL_001c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2'::'<>1__state'
      IL_0021:  ldarg.0
      IL_0022:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2'::'<>t__builder'
      IL_0027:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_002c:  ret
    } // end of method '<EmptyVoidMethod>d__2'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<EmptyVoidMethod>d__2'::SetStateMachine

  } // end of class '<EmptyVoidMethod>d__2'

  .class auto ansi sealed nested private beforefieldinit '<AwaitYield>d__3'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter '<>u__1'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       145 (0x91)
      .maxstack  3
      .locals init (int32 V_0,
               valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter V_1,
               valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable V_2,
               class [mscorlib]System.Exception V_3)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0041

        IL_000a:  call       valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable [mscorlib]System.Threading.Tasks.Task::Yield()
        IL_000f:  stloc.2
        IL_0010:  ldloca.s   V_2
        IL_0012:  call       instance valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter [mscorlib]System.Runtime.CompilerServices.YieldAwaitable::GetAwaiter()
        IL_0017:  stloc.1
        IL_0018:  ldloca.s   V_1
        IL_001a:  call       instance bool [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter::get_IsCompleted()
        IL_001f:  brtrue.s   IL_005d

        IL_0021:  ldarg.0
        IL_0022:  ldc.i4.0
        IL_0023:  dup
        IL_0024:  stloc.0
        IL_0025:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>1__state'
        IL_002a:  ldarg.0
        IL_002b:  ldloc.1
        IL_002c:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>u__1'
        IL_0031:  ldarg.0
        IL_0032:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>t__builder'
        IL_0037:  ldloca.s   V_1
        IL_0039:  ldarg.0
        IL_003a:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'>(!!0&,
                                                                                                                                                                                                                                                                                                      !!1&)
        IL_003f:  leave.s    IL_0090

        IL_0041:  ldarg.0
        IL_0042:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>u__1'
        IL_0047:  stloc.1
        IL_0048:  ldarg.0
        IL_0049:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>u__1'
        IL_004e:  initobj    [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter
        IL_0054:  ldarg.0
        IL_0055:  ldc.i4.m1
        IL_0056:  dup
        IL_0057:  stloc.0
        IL_0058:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>1__state'
        IL_005d:  ldloca.s   V_1
        IL_005f:  call       instance void [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter::GetResult()
        IL_0064:  leave.s    IL_007d

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0066:  stloc.3
        IL_0067:  ldarg.0
        IL_0068:  ldc.i4.s   -2
        IL_006a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>1__state'
        IL_006f:  ldarg.0
        IL_0070:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>t__builder'
        IL_0075:  ldloc.3
        IL_0076:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_007b:  leave.s    IL_0090

      }  // end handler
      IL_007d:  ldarg.0
      IL_007e:  ldc.i4.s   -2
      IL_0080:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>1__state'
      IL_0085:  ldarg.0
      IL_0086:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>t__builder'
      IL_008b:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_0090:  ret
    } // end of method '<AwaitYield>d__3'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<AwaitYield>d__3'::SetStateMachine

  } // end of class '<AwaitYield>d__3'

  .class auto ansi sealed nested private beforefieldinit '<AwaitDefaultYieldAwaitable>d__4'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter '<>u__1'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       146 (0x92)
      .maxstack  3
      .locals init (int32 V_0,
               valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter V_1,
               valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable V_2,
               class [mscorlib]System.Exception V_3)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0042

        IL_000a:  ldloca.s   V_2
        IL_000c:  dup
        IL_000d:  initobj    [mscorlib]System.Runtime.CompilerServices.YieldAwaitable
        IL_0013:  call       instance valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter [mscorlib]System.Runtime.CompilerServices.YieldAwaitable::GetAwaiter()
        IL_0018:  stloc.1
        IL_0019:  ldloca.s   V_1
        IL_001b:  call       instance bool [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter::get_IsCompleted()
        IL_0020:  brtrue.s   IL_005e

        IL_0022:  ldarg.0
        IL_0023:  ldc.i4.0
        IL_0024:  dup
        IL_0025:  stloc.0
        IL_0026:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>1__state'
        IL_002b:  ldarg.0
        IL_002c:  ldloc.1
        IL_002d:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>u__1'
        IL_0032:  ldarg.0
        IL_0033:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>t__builder'
        IL_0038:  ldloca.s   V_1
        IL_003a:  ldarg.0
        IL_003b:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'>(!!0&,
                                                                                                                                                                                                                                                                                                                      !!1&)
        IL_0040:  leave.s    IL_0091

        IL_0042:  ldarg.0
        IL_0043:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>u__1'
        IL_0048:  stloc.1
        IL_0049:  ldarg.0
        IL_004a:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>u__1'
        IL_004f:  initobj    [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter
        IL_0055:  ldarg.0
        IL_0056:  ldc.i4.m1
        IL_0057:  dup
        IL_0058:  stloc.0
        IL_0059:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>1__state'
        IL_005e:  ldloca.s   V_1
        IL_0060:  call       instance void [mscorlib]System.Runtime.CompilerServices.YieldAwaitable/YieldAwaiter::GetResult()
        IL_0065:  leave.s    IL_007e

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0067:  stloc.3
        IL_0068:  ldarg.0
        IL_0069:  ldc.i4.s   -2
        IL_006b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>1__state'
        IL_0070:  ldarg.0
        IL_0071:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>t__builder'
        IL_0076:  ldloc.3
        IL_0077:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_007c:  leave.s    IL_0091

      }  // end handler
      IL_007e:  ldarg.0
      IL_007f:  ldc.i4.s   -2
      IL_0081:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>1__state'
      IL_0086:  ldarg.0
      IL_0087:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>t__builder'
      IL_008c:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_0091:  ret
    } // end of method '<AwaitDefaultYieldAwaitable>d__4'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<AwaitDefaultYieldAwaitable>d__4'::SetStateMachine

  } // end of class '<AwaitDefaultYieldAwaitable>d__4'

  .class auto ansi sealed nested private beforefieldinit '<AwaitDefaultHopToThreadPool>d__5'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .field private valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable '<>u__1'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       146 (0x92)
      .maxstack  3
      .locals init (int32 V_0,
               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable V_1,
               valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable V_2,
               class [mscorlib]System.Exception V_3)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0042

        IL_000a:  ldloca.s   V_2
        IL_000c:  dup
        IL_000d:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable
        IL_0013:  call       instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable::GetAwaiter()
        IL_0018:  stloc.1
        IL_0019:  ldloca.s   V_1
        IL_001b:  call       instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable::get_IsCompleted()
        IL_0020:  brtrue.s   IL_005e

        IL_0022:  ldarg.0
        IL_0023:  ldc.i4.0
        IL_0024:  dup
        IL_0025:  stloc.0
        IL_0026:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>1__state'
        IL_002b:  ldarg.0
        IL_002c:  ldloc.1
        IL_002d:  stfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>u__1'
        IL_0032:  ldarg.0
        IL_0033:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>t__builder'
        IL_0038:  ldloca.s   V_1
        IL_003a:  ldarg.0
        IL_003b:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitOnCompleted<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'>(!!0&,
                                                                                                                                                                                                                                                                                                                  !!1&)
        IL_0040:  leave.s    IL_0091

        IL_0042:  ldarg.0
        IL_0043:  ldfld      valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>u__1'
        IL_0048:  stloc.1
        IL_0049:  ldarg.0
        IL_004a:  ldflda     valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>u__1'
        IL_004f:  initobj    ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable
        IL_0055:  ldarg.0
        IL_0056:  ldc.i4.m1
        IL_0057:  dup
        IL_0058:  stloc.0
        IL_0059:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>1__state'
        IL_005e:  ldloca.s   V_1
        IL_0060:  call       instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable::GetResult()
        IL_0065:  leave.s    IL_007e

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0067:  stloc.3
        IL_0068:  ldarg.0
        IL_0069:  ldc.i4.s   -2
        IL_006b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>1__state'
        IL_0070:  ldarg.0
        IL_0071:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>t__builder'
        IL_0076:  ldloc.3
        IL_0077:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_007c:  leave.s    IL_0091

      }  // end handler
      IL_007e:  ldarg.0
      IL_007f:  ldc.i4.s   -2
      IL_0081:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>1__state'
      IL_0086:  ldarg.0
      IL_0087:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>t__builder'
      IL_008c:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_0091:  ret
    } // end of method '<AwaitDefaultHopToThreadPool>d__5'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<AwaitDefaultHopToThreadPool>d__5'::SetStateMachine

  } // end of class '<AwaitDefaultHopToThreadPool>d__5'

  .class auto ansi sealed nested private beforefieldinit '<SimpleVoidTaskMethod>d__6'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder '<>t__builder'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter '<>u__1'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       176 (0xb0)
      .maxstack  3
      .locals init (int32 V_0,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter V_1,
               class [mscorlib]System.Exception V_2)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0056

        IL_000a:  ldstr      "Before"
        IL_000f:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0014:  ldc.r8     1.
        IL_001d:  call       valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::FromSeconds(float64)
        IL_0022:  call       class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Threading.Tasks.Task::Delay(valuetype [mscorlib]System.TimeSpan)
        IL_0027:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter [mscorlib]System.Threading.Tasks.Task::GetAwaiter()
        IL_002c:  stloc.1
        IL_002d:  ldloca.s   V_1
        IL_002f:  call       instance bool [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::get_IsCompleted()
        IL_0034:  brtrue.s   IL_0072

        IL_0036:  ldarg.0
        IL_0037:  ldc.i4.0
        IL_0038:  dup
        IL_0039:  stloc.0
        IL_003a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>1__state'
        IL_003f:  ldarg.0
        IL_0040:  ldloc.1
        IL_0041:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>u__1'
        IL_0046:  ldarg.0
        IL_0047:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>t__builder'
        IL_004c:  ldloca.s   V_1
        IL_004e:  ldarg.0
        IL_004f:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'>(!!0&,
                                                                                                                                                                                                                                                                                                !!1&)
        IL_0054:  leave.s    IL_00af

        IL_0056:  ldarg.0
        IL_0057:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>u__1'
        IL_005c:  stloc.1
        IL_005d:  ldarg.0
        IL_005e:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>u__1'
        IL_0063:  initobj    [mscorlib]System.Runtime.CompilerServices.TaskAwaiter
        IL_0069:  ldarg.0
        IL_006a:  ldc.i4.m1
        IL_006b:  dup
        IL_006c:  stloc.0
        IL_006d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>1__state'
        IL_0072:  ldloca.s   V_1
        IL_0074:  call       instance void [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::GetResult()
        IL_0079:  ldstr      "After"
        IL_007e:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0083:  leave.s    IL_009c

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0085:  stloc.2
        IL_0086:  ldarg.0
        IL_0087:  ldc.i4.s   -2
        IL_0089:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>1__state'
        IL_008e:  ldarg.0
        IL_008f:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>t__builder'
        IL_0094:  ldloc.2
        IL_0095:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_009a:  leave.s    IL_00af

      }  // end handler
      IL_009c:  ldarg.0
      IL_009d:  ldc.i4.s   -2
      IL_009f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>1__state'
      IL_00a4:  ldarg.0
      IL_00a5:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>t__builder'
      IL_00aa:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetResult()
      IL_00af:  ret
    } // end of method '<SimpleVoidTaskMethod>d__6'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<SimpleVoidTaskMethod>d__6'::SetStateMachine

  } // end of class '<SimpleVoidTaskMethod>d__6'

  .class auto ansi sealed nested private beforefieldinit '<TaskMethodWithoutAwait>d__7'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder '<>t__builder'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       55 (0x37)
      .maxstack  2
      .locals init (class [mscorlib]System.Exception V_0)
      .try
      {
        IL_0000:  ldstr      "No Await"
        IL_0005:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_000a:  leave.s    IL_0023

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_000c:  stloc.0
        IL_000d:  ldarg.0
        IL_000e:  ldc.i4.s   -2
        IL_0010:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'::'<>1__state'
        IL_0015:  ldarg.0
        IL_0016:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'::'<>t__builder'
        IL_001b:  ldloc.0
        IL_001c:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_0021:  leave.s    IL_0036

      }  // end handler
      IL_0023:  ldarg.0
      IL_0024:  ldc.i4.s   -2
      IL_0026:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'::'<>1__state'
      IL_002b:  ldarg.0
      IL_002c:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'::'<>t__builder'
      IL_0031:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetResult()
      IL_0036:  ret
    } // end of method '<TaskMethodWithoutAwait>d__7'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<TaskMethodWithoutAwait>d__7'::SetStateMachine

  } // end of class '<TaskMethodWithoutAwait>d__7'

  .class auto ansi sealed nested private beforefieldinit '<SimpleBoolTaskMethod>d__8'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> '<>t__builder'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter '<>u__1'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       179 (0xb3)
      .maxstack  3
      .locals init (int32 V_0,
               bool V_1,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter V_2,
               class [mscorlib]System.Exception V_3)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0056

        IL_000a:  ldstr      "Before"
        IL_000f:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0014:  ldc.r8     1.
        IL_001d:  call       valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::FromSeconds(float64)
        IL_0022:  call       class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Threading.Tasks.Task::Delay(valuetype [mscorlib]System.TimeSpan)
        IL_0027:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter [mscorlib]System.Threading.Tasks.Task::GetAwaiter()
        IL_002c:  stloc.2
        IL_002d:  ldloca.s   V_2
        IL_002f:  call       instance bool [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::get_IsCompleted()
        IL_0034:  brtrue.s   IL_0072

        IL_0036:  ldarg.0
        IL_0037:  ldc.i4.0
        IL_0038:  dup
        IL_0039:  stloc.0
        IL_003a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>1__state'
        IL_003f:  ldarg.0
        IL_0040:  ldloc.2
        IL_0041:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>u__1'
        IL_0046:  ldarg.0
        IL_0047:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>t__builder'
        IL_004c:  ldloca.s   V_2
        IL_004e:  ldarg.0
        IL_004f:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'>(!!0&,
                                                                                                                                                                                                                                                                                                                  !!1&)
        IL_0054:  leave.s    IL_00b2

        IL_0056:  ldarg.0
        IL_0057:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>u__1'
        IL_005c:  stloc.2
        IL_005d:  ldarg.0
        IL_005e:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>u__1'
        IL_0063:  initobj    [mscorlib]System.Runtime.CompilerServices.TaskAwaiter
        IL_0069:  ldarg.0
        IL_006a:  ldc.i4.m1
        IL_006b:  dup
        IL_006c:  stloc.0
        IL_006d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>1__state'
        IL_0072:  ldloca.s   V_2
        IL_0074:  call       instance void [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::GetResult()
        IL_0079:  ldstr      "After"
        IL_007e:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0083:  ldc.i4.1
        IL_0084:  stloc.1
        IL_0085:  leave.s    IL_009e

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0087:  stloc.3
        IL_0088:  ldarg.0
        IL_0089:  ldc.i4.s   -2
        IL_008b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>1__state'
        IL_0090:  ldarg.0
        IL_0091:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>t__builder'
        IL_0096:  ldloc.3
        IL_0097:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::SetException(class [mscorlib]System.Exception)
        IL_009c:  leave.s    IL_00b2

      }  // end handler
      IL_009e:  ldarg.0
      IL_009f:  ldc.i4.s   -2
      IL_00a1:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>1__state'
      IL_00a6:  ldarg.0
      IL_00a7:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>t__builder'
      IL_00ac:  ldloc.1
      IL_00ad:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::SetResult(!0)
      IL_00b2:  ret
    } // end of method '<SimpleBoolTaskMethod>d__8'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<SimpleBoolTaskMethod>d__8'::SetStateMachine

  } // end of class '<SimpleBoolTaskMethod>d__8'

  .class auto ansi sealed nested private beforefieldinit '<TwoAwaitsWithDifferentAwaiterTypes>d__9'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> '<>u__1'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter '<>u__2'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       285 (0x11d)
      .maxstack  3
      .locals init (int32 V_0,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async V_1,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> V_2,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter V_3,
               class [mscorlib]System.Exception V_4)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>4__this'
      IL_000d:  stloc.1
      .try
      {
        IL_000e:  ldloc.0
        IL_000f:  brfalse.s  IL_005a

        IL_0011:  ldloc.0
        IL_0012:  ldc.i4.1
        IL_0013:  beq        IL_00c1

        IL_0018:  ldstr      "Before"
        IL_001d:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0022:  ldloc.1
        IL_0023:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async::SimpleBoolTaskMethod()
        IL_0028:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [mscorlib]System.Threading.Tasks.Task`1<bool>::GetAwaiter()
        IL_002d:  stloc.2
        IL_002e:  ldloca.s   V_2
        IL_0030:  call       instance bool valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>::get_IsCompleted()
        IL_0035:  brtrue.s   IL_0076

        IL_0037:  ldarg.0
        IL_0038:  ldc.i4.0
        IL_0039:  dup
        IL_003a:  stloc.0
        IL_003b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>1__state'
        IL_0040:  ldarg.0
        IL_0041:  ldloc.2
        IL_0042:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>u__1'
        IL_0047:  ldarg.0
        IL_0048:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>t__builder'
        IL_004d:  ldloca.s   V_2
        IL_004f:  ldarg.0
        IL_0050:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'>(!!0&,
                                                                                                                                                                                                                                                                                                                      !!1&)
        IL_0055:  leave      IL_011c

        IL_005a:  ldarg.0
        IL_005b:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>u__1'
        IL_0060:  stloc.2
        IL_0061:  ldarg.0
        IL_0062:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>u__1'
        IL_0067:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>
        IL_006d:  ldarg.0
        IL_006e:  ldc.i4.m1
        IL_006f:  dup
        IL_0070:  stloc.0
        IL_0071:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>1__state'
        IL_0076:  ldloca.s   V_2
        IL_0078:  call       instance !0 valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>::GetResult()
        IL_007d:  brfalse.s  IL_00e4

        IL_007f:  ldc.r8     1.
        IL_0088:  call       valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::FromSeconds(float64)
        IL_008d:  call       class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Threading.Tasks.Task::Delay(valuetype [mscorlib]System.TimeSpan)
        IL_0092:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter [mscorlib]System.Threading.Tasks.Task::GetAwaiter()
        IL_0097:  stloc.3
        IL_0098:  ldloca.s   V_3
        IL_009a:  call       instance bool [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::get_IsCompleted()
        IL_009f:  brtrue.s   IL_00dd

        IL_00a1:  ldarg.0
        IL_00a2:  ldc.i4.1
        IL_00a3:  dup
        IL_00a4:  stloc.0
        IL_00a5:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>1__state'
        IL_00aa:  ldarg.0
        IL_00ab:  ldloc.3
        IL_00ac:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>u__2'
        IL_00b1:  ldarg.0
        IL_00b2:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>t__builder'
        IL_00b7:  ldloca.s   V_3
        IL_00b9:  ldarg.0
        IL_00ba:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'>(!!0&,
                                                                                                                                                                                                                                                                                                              !!1&)
        IL_00bf:  leave.s    IL_011c

        IL_00c1:  ldarg.0
        IL_00c2:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>u__2'
        IL_00c7:  stloc.3
        IL_00c8:  ldarg.0
        IL_00c9:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>u__2'
        IL_00ce:  initobj    [mscorlib]System.Runtime.CompilerServices.TaskAwaiter
        IL_00d4:  ldarg.0
        IL_00d5:  ldc.i4.m1
        IL_00d6:  dup
        IL_00d7:  stloc.0
        IL_00d8:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>1__state'
        IL_00dd:  ldloca.s   V_3
        IL_00df:  call       instance void [mscorlib]System.Runtime.CompilerServices.TaskAwaiter::GetResult()
        IL_00e4:  ldstr      "After"
        IL_00e9:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_00ee:  leave.s    IL_0109

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_00f0:  stloc.s    V_4
        IL_00f2:  ldarg.0
        IL_00f3:  ldc.i4.s   -2
        IL_00f5:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>1__state'
        IL_00fa:  ldarg.0
        IL_00fb:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>t__builder'
        IL_0100:  ldloc.s    V_4
        IL_0102:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_0107:  leave.s    IL_011c

      }  // end handler
      IL_0109:  ldarg.0
      IL_010a:  ldc.i4.s   -2
      IL_010c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>1__state'
      IL_0111:  ldarg.0
      IL_0112:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>t__builder'
      IL_0117:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_011c:  ret
    } // end of method '<TwoAwaitsWithDifferentAwaiterTypes>d__9'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<TwoAwaitsWithDifferentAwaiterTypes>d__9'::SetStateMachine

  } // end of class '<TwoAwaitsWithDifferentAwaiterTypes>d__9'

  .class auto ansi sealed nested private beforefieldinit '<AwaitInLoopCondition>d__10'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder '<>t__builder'
    .field public class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async '<>4__this'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> '<>u__1'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       164 (0xa4)
      .maxstack  3
      .locals init (int32 V_0,
               class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async V_1,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> V_2,
               class [mscorlib]System.Exception V_3)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>1__state'
      IL_0006:  stloc.0
      IL_0007:  ldarg.0
      IL_0008:  ldfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>4__this'
      IL_000d:  stloc.1
      .try
      {
        IL_000e:  ldloc.0
        IL_000f:  brfalse.s  IL_0052

        IL_0011:  br.s       IL_001d

        IL_0013:  ldstr      "Body"
        IL_0018:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_001d:  ldloc.1
        IL_001e:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async::SimpleBoolTaskMethod()
        IL_0023:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [mscorlib]System.Threading.Tasks.Task`1<bool>::GetAwaiter()
        IL_0028:  stloc.2
        IL_0029:  ldloca.s   V_2
        IL_002b:  call       instance bool valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>::get_IsCompleted()
        IL_0030:  brtrue.s   IL_006e

        IL_0032:  ldarg.0
        IL_0033:  ldc.i4.0
        IL_0034:  dup
        IL_0035:  stloc.0
        IL_0036:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>1__state'
        IL_003b:  ldarg.0
        IL_003c:  ldloc.2
        IL_003d:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>u__1'
        IL_0042:  ldarg.0
        IL_0043:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>t__builder'
        IL_0048:  ldloca.s   V_2
        IL_004a:  ldarg.0
        IL_004b:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'>(!!0&,
                                                                                                                                                                                                                                                                                                         !!1&)
        IL_0050:  leave.s    IL_00a3

        IL_0052:  ldarg.0
        IL_0053:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>u__1'
        IL_0058:  stloc.2
        IL_0059:  ldarg.0
        IL_005a:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>u__1'
        IL_005f:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>
        IL_0065:  ldarg.0
        IL_0066:  ldc.i4.m1
        IL_0067:  dup
        IL_0068:  stloc.0
        IL_0069:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>1__state'
        IL_006e:  ldloca.s   V_2
        IL_0070:  call       instance !0 valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<bool>::GetResult()
        IL_0075:  brtrue.s   IL_0013

        IL_0077:  leave.s    IL_0090

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0079:  stloc.3
        IL_007a:  ldarg.0
        IL_007b:  ldc.i4.s   -2
        IL_007d:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>1__state'
        IL_0082:  ldarg.0
        IL_0083:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>t__builder'
        IL_0088:  ldloc.3
        IL_0089:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_008e:  leave.s    IL_00a3

      }  // end handler
      IL_0090:  ldarg.0
      IL_0091:  ldc.i4.s   -2
      IL_0093:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>1__state'
      IL_0098:  ldarg.0
      IL_0099:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>t__builder'
      IL_009e:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetResult()
      IL_00a3:  ret
    } // end of method '<AwaitInLoopCondition>d__10'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<AwaitInLoopCondition>d__10'::SetStateMachine

  } // end of class '<AwaitInLoopCondition>d__10'

  .class auto ansi sealed nested private beforefieldinit '<AwaitInCatch>d__11'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder '<>t__builder'
    .field public class [mscorlib]System.Threading.Tasks.Task`1<int32> task1
    .field public bool b
    .field public class [mscorlib]System.Threading.Tasks.Task`1<int32> task2
    .field private int32 '<>7__wrap1'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> '<>u__1'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       317 (0x13d)
      .maxstack  3
      .locals init (int32 V_0,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> V_1,
               int32 V_2,
               class [mscorlib]System.Exception V_3)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_0018

        IL_000a:  ldloc.0
        IL_000b:  ldc.i4.1
        IL_000c:  beq        IL_00e0

        IL_0011:  ldarg.0
        IL_0012:  ldc.i4.0
        IL_0013:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>7__wrap1'
        IL_0018:  nop
        .try
        {
          IL_0019:  ldloc.0
          IL_001a:  brfalse.s  IL_005e

          IL_001c:  ldstr      "Start try"
          IL_0021:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_0026:  ldarg.0
          IL_0027:  ldfld      class [mscorlib]System.Threading.Tasks.Task`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::task1
          IL_002c:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [mscorlib]System.Threading.Tasks.Task`1<int32>::GetAwaiter()
          IL_0031:  stloc.1
          IL_0032:  ldloca.s   V_1
          IL_0034:  call       instance bool valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::get_IsCompleted()
          IL_0039:  brtrue.s   IL_007a

          IL_003b:  ldarg.0
          IL_003c:  ldc.i4.0
          IL_003d:  dup
          IL_003e:  stloc.0
          IL_003f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>1__state'
          IL_0044:  ldarg.0
          IL_0045:  ldloc.1
          IL_0046:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>u__1'
          IL_004b:  ldarg.0
          IL_004c:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>t__builder'
          IL_0051:  ldloca.s   V_1
          IL_0053:  ldarg.0
          IL_0054:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'>(!!0&,
                                                                                                                                                                                                                                                                                                    !!1&)
          IL_0059:  leave      IL_013c

          IL_005e:  ldarg.0
          IL_005f:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>u__1'
          IL_0064:  stloc.1
          IL_0065:  ldarg.0
          IL_0066:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>u__1'
          IL_006b:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>
          IL_0071:  ldarg.0
          IL_0072:  ldc.i4.m1
          IL_0073:  dup
          IL_0074:  stloc.0
          IL_0075:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>1__state'
          IL_007a:  ldloca.s   V_1
          IL_007c:  call       instance !0 valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::GetResult()
          IL_0081:  pop
          IL_0082:  ldstr      "End try"
          IL_0087:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_008c:  leave.s    IL_0098

        }  // end .try
        catch [mscorlib]System.Exception 
        {
          IL_008e:  pop
          IL_008f:  ldarg.0
          IL_0090:  ldc.i4.1
          IL_0091:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>7__wrap1'
          IL_0096:  leave.s    IL_0098

        }  // end handler
        IL_0098:  ldarg.0
        IL_0099:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>7__wrap1'
        IL_009e:  stloc.2
        IL_009f:  ldloc.2
        IL_00a0:  ldc.i4.1
        IL_00a1:  bne.un.s   IL_0110

        IL_00a3:  ldarg.0
        IL_00a4:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::b
        IL_00a9:  brtrue.s   IL_0106

        IL_00ab:  ldarg.0
        IL_00ac:  ldfld      class [mscorlib]System.Threading.Tasks.Task`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::task2
        IL_00b1:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [mscorlib]System.Threading.Tasks.Task`1<int32>::GetAwaiter()
        IL_00b6:  stloc.1
        IL_00b7:  ldloca.s   V_1
        IL_00b9:  call       instance bool valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::get_IsCompleted()
        IL_00be:  brtrue.s   IL_00fc

        IL_00c0:  ldarg.0
        IL_00c1:  ldc.i4.1
        IL_00c2:  dup
        IL_00c3:  stloc.0
        IL_00c4:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>1__state'
        IL_00c9:  ldarg.0
        IL_00ca:  ldloc.1
        IL_00cb:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>u__1'
        IL_00d0:  ldarg.0
        IL_00d1:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>t__builder'
        IL_00d6:  ldloca.s   V_1
        IL_00d8:  ldarg.0
        IL_00d9:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'>(!!0&,
                                                                                                                                                                                                                                                                                                  !!1&)
        IL_00de:  leave.s    IL_013c

        IL_00e0:  ldarg.0
        IL_00e1:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>u__1'
        IL_00e6:  stloc.1
        IL_00e7:  ldarg.0
        IL_00e8:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>u__1'
        IL_00ed:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>
        IL_00f3:  ldarg.0
        IL_00f4:  ldc.i4.m1
        IL_00f5:  dup
        IL_00f6:  stloc.0
        IL_00f7:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>1__state'
        IL_00fc:  ldloca.s   V_1
        IL_00fe:  call       instance !0 valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::GetResult()
        IL_0103:  pop
        IL_0104:  br.s       IL_0110

        IL_0106:  ldstr      "No await"
        IL_010b:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_0110:  leave.s    IL_0129

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0112:  stloc.3
        IL_0113:  ldarg.0
        IL_0114:  ldc.i4.s   -2
        IL_0116:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>1__state'
        IL_011b:  ldarg.0
        IL_011c:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>t__builder'
        IL_0121:  ldloc.3
        IL_0122:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_0127:  leave.s    IL_013c

      }  // end handler
      IL_0129:  ldarg.0
      IL_012a:  ldc.i4.s   -2
      IL_012c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>1__state'
      IL_0131:  ldarg.0
      IL_0132:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>t__builder'
      IL_0137:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetResult()
      IL_013c:  ret
    } // end of method '<AwaitInCatch>d__11'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<AwaitInCatch>d__11'::SetStateMachine

  } // end of class '<AwaitInCatch>d__11'

  .class auto ansi sealed nested private beforefieldinit '<AwaitInFinally>d__12'
         extends [mscorlib]System.ValueType
         implements [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    .field public int32 '<>1__state'
    .field public valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder '<>t__builder'
    .field public class [mscorlib]System.Threading.Tasks.Task`1<int32> task1
    .field public bool b
    .field public class [mscorlib]System.Threading.Tasks.Task`1<int32> task2
    .field private object '<>7__wrap1'
    .field private int32 '<>7__wrap2'
    .field private valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> '<>u__1'
    .method private hidebysig newslot virtual final 
            instance void  MoveNext() cil managed
    {
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::MoveNext
      // Code size       354 (0x162)
      .maxstack  3
      .locals init (int32 V_0,
               valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> V_1,
               object V_2,
               class [mscorlib]System.Exception V_3)
      IL_0000:  ldarg.0
      IL_0001:  ldfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>1__state'
      IL_0006:  stloc.0
      .try
      {
        IL_0007:  ldloc.0
        IL_0008:  brfalse.s  IL_001f

        IL_000a:  ldloc.0
        IL_000b:  ldc.i4.1
        IL_000c:  beq        IL_00df

        IL_0011:  ldarg.0
        IL_0012:  ldnull
        IL_0013:  stfld      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>7__wrap1'
        IL_0018:  ldarg.0
        IL_0019:  ldc.i4.0
        IL_001a:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>7__wrap2'
        IL_001f:  nop
        .try
        {
          IL_0020:  ldloc.0
          IL_0021:  brfalse.s  IL_0065

          IL_0023:  ldstr      "Start try"
          IL_0028:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_002d:  ldarg.0
          IL_002e:  ldfld      class [mscorlib]System.Threading.Tasks.Task`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::task1
          IL_0033:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [mscorlib]System.Threading.Tasks.Task`1<int32>::GetAwaiter()
          IL_0038:  stloc.1
          IL_0039:  ldloca.s   V_1
          IL_003b:  call       instance bool valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::get_IsCompleted()
          IL_0040:  brtrue.s   IL_0081

          IL_0042:  ldarg.0
          IL_0043:  ldc.i4.0
          IL_0044:  dup
          IL_0045:  stloc.0
          IL_0046:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>1__state'
          IL_004b:  ldarg.0
          IL_004c:  ldloc.1
          IL_004d:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>u__1'
          IL_0052:  ldarg.0
          IL_0053:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>t__builder'
          IL_0058:  ldloca.s   V_1
          IL_005a:  ldarg.0
          IL_005b:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'>(!!0&,
                                                                                                                                                                                                                                                                                                      !!1&)
          IL_0060:  leave      IL_0161

          IL_0065:  ldarg.0
          IL_0066:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>u__1'
          IL_006b:  stloc.1
          IL_006c:  ldarg.0
          IL_006d:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>u__1'
          IL_0072:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>
          IL_0078:  ldarg.0
          IL_0079:  ldc.i4.m1
          IL_007a:  dup
          IL_007b:  stloc.0
          IL_007c:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>1__state'
          IL_0081:  ldloca.s   V_1
          IL_0083:  call       instance !0 valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::GetResult()
          IL_0088:  pop
          IL_0089:  ldstr      "End try"
          IL_008e:  call       void [mscorlib]System.Console::WriteLine(string)
          IL_0093:  leave.s    IL_009f

        }  // end .try
        catch [mscorlib]System.Object 
        {
          IL_0095:  stloc.2
          IL_0096:  ldarg.0
          IL_0097:  ldloc.2
          IL_0098:  stfld      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>7__wrap1'
          IL_009d:  leave.s    IL_009f

        }  // end handler
        IL_009f:  ldarg.0
        IL_00a0:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::b
        IL_00a5:  brtrue.s   IL_0105

        IL_00a7:  ldarg.0
        IL_00a8:  ldfld      class [mscorlib]System.Threading.Tasks.Task`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::task2
        IL_00ad:  callvirt   instance valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<!0> class [mscorlib]System.Threading.Tasks.Task`1<int32>::GetAwaiter()
        IL_00b2:  stloc.1
        IL_00b3:  ldloca.s   V_1
        IL_00b5:  call       instance bool valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::get_IsCompleted()
        IL_00ba:  brtrue.s   IL_00fb

        IL_00bc:  ldarg.0
        IL_00bd:  ldc.i4.1
        IL_00be:  dup
        IL_00bf:  stloc.0
        IL_00c0:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>1__state'
        IL_00c5:  ldarg.0
        IL_00c6:  ldloc.1
        IL_00c7:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>u__1'
        IL_00cc:  ldarg.0
        IL_00cd:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>t__builder'
        IL_00d2:  ldloca.s   V_1
        IL_00d4:  ldarg.0
        IL_00d5:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::AwaitUnsafeOnCompleted<valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>,valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'>(!!0&,
                                                                                                                                                                                                                                                                                                    !!1&)
        IL_00da:  leave      IL_0161

        IL_00df:  ldarg.0
        IL_00e0:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>u__1'
        IL_00e5:  stloc.1
        IL_00e6:  ldarg.0
        IL_00e7:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>u__1'
        IL_00ec:  initobj    valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>
        IL_00f2:  ldarg.0
        IL_00f3:  ldc.i4.m1
        IL_00f4:  dup
        IL_00f5:  stloc.0
        IL_00f6:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>1__state'
        IL_00fb:  ldloca.s   V_1
        IL_00fd:  call       instance !0 valuetype [mscorlib]System.Runtime.CompilerServices.TaskAwaiter`1<int32>::GetResult()
        IL_0102:  pop
        IL_0103:  br.s       IL_010f

        IL_0105:  ldstr      "No await"
        IL_010a:  call       void [mscorlib]System.Console::WriteLine(string)
        IL_010f:  ldarg.0
        IL_0110:  ldfld      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>7__wrap1'
        IL_0115:  stloc.2
        IL_0116:  ldloc.2
        IL_0117:  brfalse.s  IL_012e

        IL_0119:  ldloc.2
        IL_011a:  isinst     [mscorlib]System.Exception
        IL_011f:  dup
        IL_0120:  brtrue.s   IL_0124

        IL_0122:  ldloc.2
        IL_0123:  throw

        IL_0124:  call       class [mscorlib]System.Runtime.ExceptionServices.ExceptionDispatchInfo [mscorlib]System.Runtime.ExceptionServices.ExceptionDispatchInfo::Capture(class [mscorlib]System.Exception)
        IL_0129:  callvirt   instance void [mscorlib]System.Runtime.ExceptionServices.ExceptionDispatchInfo::Throw()
        IL_012e:  ldarg.0
        IL_012f:  ldnull
        IL_0130:  stfld      object ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>7__wrap1'
        IL_0135:  leave.s    IL_014e

      }  // end .try
      catch [mscorlib]System.Exception 
      {
        IL_0137:  stloc.3
        IL_0138:  ldarg.0
        IL_0139:  ldc.i4.s   -2
        IL_013b:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>1__state'
        IL_0140:  ldarg.0
        IL_0141:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>t__builder'
        IL_0146:  ldloc.3
        IL_0147:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetException(class [mscorlib]System.Exception)
        IL_014c:  leave.s    IL_0161

      }  // end handler
      IL_014e:  ldarg.0
      IL_014f:  ldc.i4.s   -2
      IL_0151:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>1__state'
      IL_0156:  ldarg.0
      IL_0157:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>t__builder'
      IL_015c:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetResult()
      IL_0161:  ret
    } // end of method '<AwaitInFinally>d__12'::MoveNext

    .method private hidebysig newslot virtual final 
            instance void  SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine stateMachine) cil managed
    {
      .custom instance void [mscorlib]System.Diagnostics.DebuggerHiddenAttribute::.ctor() = ( 01 00 00 00 ) 
      .override [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine::SetStateMachine
      // Code size       13 (0xd)
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>t__builder'
      IL_0006:  ldarg.1
      IL_0007:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::SetStateMachine(class [mscorlib]System.Runtime.CompilerServices.IAsyncStateMachine)
      IL_000c:  ret
    } // end of method '<AwaitInFinally>d__12'::SetStateMachine

  } // end of class '<AwaitInFinally>d__12'

  .method public hidebysig instance void 
          SimpleVoidMethod() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 4A 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..JICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 53 69 6D 70 6C 65 56 6F   // .Async+<SimpleVo
                                                                                                                                       69 64 4D 65 74 68 6F 64 3E 64 5F 5F 30 00 00 )    // idMethod>d__0..
    // Code size       37 (0x25)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_0007:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>t__builder'
    IL_000c:  ldloca.s   V_0
    IL_000e:  ldc.i4.m1
    IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>1__state'
    IL_0014:  ldloc.0
    IL_0015:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'::'<>t__builder'
    IL_001a:  stloc.1
    IL_001b:  ldloca.s   V_1
    IL_001d:  ldloca.s   V_0
    IL_001f:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidMethod>d__0'>(!!0&)
    IL_0024:  ret
  } // end of method Async::SimpleVoidMethod

  .method public hidebysig instance void 
          VoidMethodWithoutAwait() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 50 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..PICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 56 6F 69 64 4D 65 74 68   // .Async+<VoidMeth
                                                                                                                                       6F 64 57 69 74 68 6F 75 74 41 77 61 69 74 3E 64   // odWithoutAwait>d
                                                                                                                                       5F 5F 31 00 00 )                                  // __1..
    // Code size       37 (0x25)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_0007:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1'::'<>t__builder'
    IL_000c:  ldloca.s   V_0
    IL_000e:  ldc.i4.m1
    IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1'::'<>1__state'
    IL_0014:  ldloc.0
    IL_0015:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1'::'<>t__builder'
    IL_001a:  stloc.1
    IL_001b:  ldloca.s   V_1
    IL_001d:  ldloca.s   V_0
    IL_001f:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<VoidMethodWithoutAwait>d__1'>(!!0&)
    IL_0024:  ret
  } // end of method Async::VoidMethodWithoutAwait

  .method public hidebysig instance void 
          EmptyVoidMethod() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 49 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..IICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 45 6D 70 74 79 56 6F 69   // .Async+<EmptyVoi
                                                                                                                                       64 4D 65 74 68 6F 64 3E 64 5F 5F 32 00 00 )       // dMethod>d__2..
    // Code size       37 (0x25)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_0007:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2'::'<>t__builder'
    IL_000c:  ldloca.s   V_0
    IL_000e:  ldc.i4.m1
    IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2'::'<>1__state'
    IL_0014:  ldloc.0
    IL_0015:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2'::'<>t__builder'
    IL_001a:  stloc.1
    IL_001b:  ldloca.s   V_1
    IL_001d:  ldloca.s   V_0
    IL_001f:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<EmptyVoidMethod>d__2'>(!!0&)
    IL_0024:  ret
  } // end of method Async::EmptyVoidMethod

  .method public hidebysig instance void 
          AwaitYield() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 44 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..DICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 41 77 61 69 74 59 69 65   // .Async+<AwaitYie
                                                                                                                                       6C 64 3E 64 5F 5F 33 00 00 )                      // ld>d__3..
    // Code size       37 (0x25)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_0007:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>t__builder'
    IL_000c:  ldloca.s   V_0
    IL_000e:  ldc.i4.m1
    IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>1__state'
    IL_0014:  ldloc.0
    IL_0015:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'::'<>t__builder'
    IL_001a:  stloc.1
    IL_001b:  ldloca.s   V_1
    IL_001d:  ldloca.s   V_0
    IL_001f:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitYield>d__3'>(!!0&)
    IL_0024:  ret
  } // end of method Async::AwaitYield

  .method public hidebysig instance void 
          AwaitDefaultYieldAwaitable() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 54 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..TICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 41 77 61 69 74 44 65 66   // .Async+<AwaitDef
                                                                                                                                       61 75 6C 74 59 69 65 6C 64 41 77 61 69 74 61 62   // aultYieldAwaitab
                                                                                                                                       6C 65 3E 64 5F 5F 34 00 00 )                      // le>d__4..
    // Code size       37 (0x25)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_0007:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>t__builder'
    IL_000c:  ldloca.s   V_0
    IL_000e:  ldc.i4.m1
    IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>1__state'
    IL_0014:  ldloc.0
    IL_0015:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'::'<>t__builder'
    IL_001a:  stloc.1
    IL_001b:  ldloca.s   V_1
    IL_001d:  ldloca.s   V_0
    IL_001f:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultYieldAwaitable>d__4'>(!!0&)
    IL_0024:  ret
  } // end of method Async::AwaitDefaultYieldAwaitable

  .method public hidebysig instance void 
          AwaitDefaultHopToThreadPool() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 55 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..UICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 41 77 61 69 74 44 65 66   // .Async+<AwaitDef
                                                                                                                                       61 75 6C 74 48 6F 70 54 6F 54 68 72 65 61 64 50   // aultHopToThreadP
                                                                                                                                       6F 6F 6C 3E 64 5F 5F 35 00 00 )                   // ool>d__5..
    // Code size       37 (0x25)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_0007:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>t__builder'
    IL_000c:  ldloca.s   V_0
    IL_000e:  ldc.i4.m1
    IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>1__state'
    IL_0014:  ldloc.0
    IL_0015:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'::'<>t__builder'
    IL_001a:  stloc.1
    IL_001b:  ldloca.s   V_1
    IL_001d:  ldloca.s   V_0
    IL_001f:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitDefaultHopToThreadPool>d__5'>(!!0&)
    IL_0024:  ret
  } // end of method Async::AwaitDefaultHopToThreadPool

  .method public hidebysig instance class [mscorlib]System.Threading.Tasks.Task 
          SimpleVoidTaskMethod() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 4E 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..NICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 53 69 6D 70 6C 65 56 6F   // .Async+<SimpleVo
                                                                                                                                       69 64 54 61 73 6B 4D 65 74 68 6F 64 3E 64 5F 5F   // idTaskMethod>d__
                                                                                                                                       36 00 00 )                                        // 6..
    // Code size       49 (0x31)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Create()
    IL_0007:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>t__builder'
    IL_000c:  ldloca.s   V_0
    IL_000e:  ldc.i4.m1
    IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>1__state'
    IL_0014:  ldloc.0
    IL_0015:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>t__builder'
    IL_001a:  stloc.1
    IL_001b:  ldloca.s   V_1
    IL_001d:  ldloca.s   V_0
    IL_001f:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'>(!!0&)
    IL_0024:  ldloca.s   V_0
    IL_0026:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleVoidTaskMethod>d__6'::'<>t__builder'
    IL_002b:  call       instance class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::get_Task()
    IL_0030:  ret
  } // end of method Async::SimpleVoidTaskMethod

  .method public hidebysig instance class [mscorlib]System.Threading.Tasks.Task 
          TaskMethodWithoutAwait() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 50 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..PICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 54 61 73 6B 4D 65 74 68   // .Async+<TaskMeth
                                                                                                                                       6F 64 57 69 74 68 6F 75 74 41 77 61 69 74 3E 64   // odWithoutAwait>d
                                                                                                                                       5F 5F 37 00 00 )                                  // __7..
    // Code size       49 (0x31)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Create()
    IL_0007:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'::'<>t__builder'
    IL_000c:  ldloca.s   V_0
    IL_000e:  ldc.i4.m1
    IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'::'<>1__state'
    IL_0014:  ldloc.0
    IL_0015:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'::'<>t__builder'
    IL_001a:  stloc.1
    IL_001b:  ldloca.s   V_1
    IL_001d:  ldloca.s   V_0
    IL_001f:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'>(!!0&)
    IL_0024:  ldloca.s   V_0
    IL_0026:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TaskMethodWithoutAwait>d__7'::'<>t__builder'
    IL_002b:  call       instance class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::get_Task()
    IL_0030:  ret
  } // end of method Async::TaskMethodWithoutAwait

  .method public hidebysig instance class [mscorlib]System.Threading.Tasks.Task`1<bool> 
          SimpleBoolTaskMethod() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 4E 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..NICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 53 69 6D 70 6C 65 42 6F   // .Async+<SimpleBo
                                                                                                                                       6F 6C 54 61 73 6B 4D 65 74 68 6F 64 3E 64 5F 5F   // olTaskMethod>d__
                                                                                                                                       38 00 00 )                                        // 8..
    // Code size       49 (0x31)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<!0> valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::Create()
    IL_0007:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>t__builder'
    IL_000c:  ldloca.s   V_0
    IL_000e:  ldc.i4.m1
    IL_000f:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>1__state'
    IL_0014:  ldloc.0
    IL_0015:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>t__builder'
    IL_001a:  stloc.1
    IL_001b:  ldloca.s   V_1
    IL_001d:  ldloca.s   V_0
    IL_001f:  call       instance void valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'>(!!0&)
    IL_0024:  ldloca.s   V_0
    IL_0026:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<SimpleBoolTaskMethod>d__8'::'<>t__builder'
    IL_002b:  call       instance class [mscorlib]System.Threading.Tasks.Task`1<!0> valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1<bool>::get_Task()
    IL_0030:  ret
  } // end of method Async::SimpleBoolTaskMethod

  .method public hidebysig instance void 
          TwoAwaitsWithDifferentAwaiterTypes() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 5C 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..\ICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 54 77 6F 41 77 61 69 74   // .Async+<TwoAwait
                                                                                                                                       73 57 69 74 68 44 69 66 66 65 72 65 6E 74 41 77   // sWithDifferentAw
                                                                                                                                       61 69 74 65 72 54 79 70 65 73 3E 64 5F 5F 39 00   // aiterTypes>d__9.
                                                                                                                                       00 ) 
    // Code size       45 (0x2d)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldarg.0
    IL_0003:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>4__this'
    IL_0008:  ldloca.s   V_0
    IL_000a:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_000f:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>t__builder'
    IL_0014:  ldloca.s   V_0
    IL_0016:  ldc.i4.m1
    IL_0017:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>1__state'
    IL_001c:  ldloc.0
    IL_001d:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'::'<>t__builder'
    IL_0022:  stloc.1
    IL_0023:  ldloca.s   V_1
    IL_0025:  ldloca.s   V_0
    IL_0027:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<TwoAwaitsWithDifferentAwaiterTypes>d__9'>(!!0&)
    IL_002c:  ret
  } // end of method Async::TwoAwaitsWithDifferentAwaiterTypes

  .method public hidebysig instance void 
          AwaitInLoopCondition() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 4F 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..OICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 41 77 61 69 74 49 6E 4C   // .Async+<AwaitInL
                                                                                                                                       6F 6F 70 43 6F 6E 64 69 74 69 6F 6E 3E 64 5F 5F   // oopCondition>d__
                                                                                                                                       31 30 00 00 )                                     // 10..
    // Code size       45 (0x2d)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldarg.0
    IL_0003:  stfld      class ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>4__this'
    IL_0008:  ldloca.s   V_0
    IL_000a:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Create()
    IL_000f:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>t__builder'
    IL_0014:  ldloca.s   V_0
    IL_0016:  ldc.i4.m1
    IL_0017:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>1__state'
    IL_001c:  ldloc.0
    IL_001d:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'::'<>t__builder'
    IL_0022:  stloc.1
    IL_0023:  ldloca.s   V_1
    IL_0025:  ldloca.s   V_0
    IL_0027:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncVoidMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInLoopCondition>d__10'>(!!0&)
    IL_002c:  ret
  } // end of method Async::AwaitInLoopCondition

  .method public hidebysig instance class [mscorlib]System.Threading.Tasks.Task 
          AwaitInCatch(bool b,
                       class [mscorlib]System.Threading.Tasks.Task`1<int32> task1,
                       class [mscorlib]System.Threading.Tasks.Task`1<int32> task2) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 47 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..GICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 41 77 61 69 74 49 6E 43   // .Async+<AwaitInC
                                                                                                                                       61 74 63 68 3E 64 5F 5F 31 31 00 00 )             // atch>d__11..
    // Code size       73 (0x49)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldarg.1
    IL_0003:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::b
    IL_0008:  ldloca.s   V_0
    IL_000a:  ldarg.2
    IL_000b:  stfld      class [mscorlib]System.Threading.Tasks.Task`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::task1
    IL_0010:  ldloca.s   V_0
    IL_0012:  ldarg.3
    IL_0013:  stfld      class [mscorlib]System.Threading.Tasks.Task`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::task2
    IL_0018:  ldloca.s   V_0
    IL_001a:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Create()
    IL_001f:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>t__builder'
    IL_0024:  ldloca.s   V_0
    IL_0026:  ldc.i4.m1
    IL_0027:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>1__state'
    IL_002c:  ldloc.0
    IL_002d:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>t__builder'
    IL_0032:  stloc.1
    IL_0033:  ldloca.s   V_1
    IL_0035:  ldloca.s   V_0
    IL_0037:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'>(!!0&)
    IL_003c:  ldloca.s   V_0
    IL_003e:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInCatch>d__11'::'<>t__builder'
    IL_0043:  call       instance class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::get_Task()
    IL_0048:  ret
  } // end of method Async::AwaitInCatch

  .method public hidebysig instance class [mscorlib]System.Threading.Tasks.Task 
          AwaitInFinally(bool b,
                         class [mscorlib]System.Threading.Tasks.Task`1<int32> task1,
                         class [mscorlib]System.Threading.Tasks.Task`1<int32> task2) cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.AsyncStateMachineAttribute::.ctor(class [mscorlib]System.Type) = ( 01 00 49 49 43 53 68 61 72 70 43 6F 64 65 2E 44   // ..IICSharpCode.D
                                                                                                                                       65 63 6F 6D 70 69 6C 65 72 2E 54 65 73 74 73 2E   // ecompiler.Tests.
                                                                                                                                       54 65 73 74 43 61 73 65 73 2E 50 72 65 74 74 79   // TestCases.Pretty
                                                                                                                                       2E 41 73 79 6E 63 2B 3C 41 77 61 69 74 49 6E 46   // .Async+<AwaitInF
                                                                                                                                       69 6E 61 6C 6C 79 3E 64 5F 5F 31 32 00 00 )       // inally>d__12..
    // Code size       73 (0x49)
    .maxstack  2
    .locals init (valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12' V_0,
             valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder V_1)
    IL_0000:  ldloca.s   V_0
    IL_0002:  ldarg.1
    IL_0003:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::b
    IL_0008:  ldloca.s   V_0
    IL_000a:  ldarg.2
    IL_000b:  stfld      class [mscorlib]System.Threading.Tasks.Task`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::task1
    IL_0010:  ldloca.s   V_0
    IL_0012:  ldarg.3
    IL_0013:  stfld      class [mscorlib]System.Threading.Tasks.Task`1<int32> ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::task2
    IL_0018:  ldloca.s   V_0
    IL_001a:  call       valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Create()
    IL_001f:  stfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>t__builder'
    IL_0024:  ldloca.s   V_0
    IL_0026:  ldc.i4.m1
    IL_0027:  stfld      int32 ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>1__state'
    IL_002c:  ldloc.0
    IL_002d:  ldfld      valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>t__builder'
    IL_0032:  stloc.1
    IL_0033:  ldloca.s   V_1
    IL_0035:  ldloca.s   V_0
    IL_0037:  call       instance void [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::Start<valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'>(!!0&)
    IL_003c:  ldloca.s   V_0
    IL_003e:  ldflda     valuetype [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder ICSharpCode.Decompiler.Tests.TestCases.Pretty.Async/'<AwaitInFinally>d__12'::'<>t__builder'
    IL_0043:  call       instance class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Runtime.CompilerServices.AsyncTaskMethodBuilder::get_Task()
    IL_0048:  ret
  } // end of method Async::AwaitInFinally

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

.class public sequential ansi sealed beforefieldinit ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable
       extends [mscorlib]System.ValueType
       implements [mscorlib]System.Runtime.CompilerServices.INotifyCompletion
{
  .field private bool '<IsCompleted>k__BackingField'
  .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
  .method public hidebysig specialname instance bool 
          get_IsCompleted() cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable::'<IsCompleted>k__BackingField'
    IL_0006:  ret
  } // end of method HopToThreadPoolAwaitable::get_IsCompleted

  .method public hidebysig specialname instance void 
          set_IsCompleted(bool 'value') cil managed
  {
    .custom instance void [mscorlib]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldarg.1
    IL_0002:  stfld      bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable::'<IsCompleted>k__BackingField'
    IL_0007:  ret
  } // end of method HopToThreadPoolAwaitable::set_IsCompleted

  .method public hidebysig instance valuetype ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable 
          GetAwaiter() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldobj      ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable
    IL_0006:  ret
  } // end of method HopToThreadPoolAwaitable::GetAwaiter

  .method public hidebysig newslot virtual final 
          instance void  OnCompleted(class [mscorlib]System.Action continuation) cil managed
  {
    // Code size       8 (0x8)
    .maxstack  8
    IL_0000:  ldarg.1
    IL_0001:  call       class [mscorlib]System.Threading.Tasks.Task [mscorlib]System.Threading.Tasks.Task::Run(class [mscorlib]System.Action)
    IL_0006:  pop
    IL_0007:  ret
  } // end of method HopToThreadPoolAwaitable::OnCompleted

  .method public hidebysig instance void 
          GetResult() cil managed
  {
    // Code size       1 (0x1)
    .maxstack  8
    IL_0000:  ret
  } // end of method HopToThreadPoolAwaitable::GetResult

  .property instance bool IsCompleted()
  {
    .get instance bool ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable::get_IsCompleted()
    .set instance void ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable::set_IsCompleted(bool)
  } // end of property HopToThreadPoolAwaitable::IsCompleted
} // end of class ICSharpCode.Decompiler.Tests.TestCases.Pretty.HopToThreadPoolAwaitable


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
